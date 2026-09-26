<#
.SYNOPSIS
  Copy the live Azure SQL database onto the production server and swap it into the Bicep name.

.DESCRIPTION
  ARM-copies the source database to a sidecar on sql-prd-dertinfo-storage-uks, stops
  app-prd-dertinfo-api-uks when that site exists, renames the current production
  database aside, promotes the copy to sqldb-prd-dertinfo-storage-uks, binds production
  Entra users, and starts the API again when it was stopped. Then pauses for Continue
  (delete the previous production database) or Revert (swap back and delete the copy).

  The default source is the old-stack live database. This script does not read or
  write the development server. It refuses a source larger than the Basic 2 GB cap
  on the production database. Copy is control-plane. User bind uses sqlcmd -G from
  this machine. Each bind opens an Entra MFA prompt (often behind the terminal).

  As an operator I run this script to copy live SQL onto production because I want the new production database to hold live data and the previous database still reversible.

.PARAMETER SourceResourceGroup
  Source SQL resource group. Default dertinfo-live-rg.

.PARAMETER SourceServer
  Source logical server name, with or without .database.windows.net. Default dertinfo-live-sqlsvr.

.PARAMETER SourceDatabase
  Source database name. Default dertinfo-live-sqldb.

.PARAMETER UserName
  Entra login passed to the user-bind scripts. Default: az account show user.name.

.EXAMPLE
  .\Database\Copy-DertInfoSqlToProduction.ps1

.EXAMPLE
  .\Database\Copy-DertInfoSqlToProduction.ps1 `
    -SourceResourceGroup 'my-live-sql-rg' `
    -SourceServer 'my-live-sql-server' `
    -SourceDatabase 'my-live-database'
#>
[CmdletBinding()]
param(
  [string] $SourceResourceGroup = 'dertinfo-live-rg',
  [string] $SourceServer = 'dertinfo-live-sqlsvr',
  [string] $SourceDatabase = 'dertinfo-live-sqldb',
  [string] $UserName = ''
)

$ErrorActionPreference = 'Stop'

$destRg = 'rg-prd-dertinfo-storage-uks'
$destServer = 'sql-prd-dertinfo-storage-uks'
$officialName = 'sqldb-prd-dertinfo-storage-uks'
$copyName = 'sqldb-prd-dertinfo-storage-uks-copy'
$oldName = 'sqldb-prd-dertinfo-storage-uks-old'
$webAppRg = 'rg-prd-dertinfo-api-uks'
$webAppName = 'app-prd-dertinfo-api-uks'
$swaggerUrl = 'https://app-prd-dertinfo-api-uks.azurewebsites.net/swagger/index.html'
$basicMaxBytes = [int64]2147483648
$copyTimeout = [TimeSpan]::FromMinutes(60)
$copyPollSeconds = 15

function Assert-AzCli {
  if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    throw 'Azure CLI (az) is required on PATH. Run az login.'
  }
  $previousEap = $ErrorActionPreference
  $ErrorActionPreference = 'Continue'
  try {
    az account show 1>$null 2>$null
    $azExit = $LASTEXITCODE
  }
  finally {
    $ErrorActionPreference = $previousEap
  }
  if ($azExit -ne 0) {
    throw 'Not logged in to Azure CLI. Run: az login'
  }
}

function Invoke-AzCli {
  param(
    [Parameter(Mandatory = $true)]
    [string[]] $AzArgs
  )
  $previousEap = $ErrorActionPreference
  $ErrorActionPreference = 'Continue'
  try {
    $output = & az @AzArgs --only-show-errors
    $azExit = $LASTEXITCODE
  }
  finally {
    $ErrorActionPreference = $previousEap
  }
  if ($azExit -ne 0) {
    throw "az $($AzArgs -join ' ') failed with exit code $azExit."
  }
  return $output
}

function Get-DertInfoSubscriptionForResourceGroup {
  param(
    [Parameter(Mandatory = $true)]
    [string] $ResourceGroupName,

    [switch] $Optional
  )

  $previousEap = $ErrorActionPreference
  $ErrorActionPreference = 'Continue'
  try {
    $subscriptionIds = @(az account list --query '[].id' -o tsv)
    $listExit = $LASTEXITCODE
  }
  finally {
    $ErrorActionPreference = $previousEap
  }
  if ($listExit -ne 0 -or $subscriptionIds.Count -eq 0) {
    throw 'Could not list Azure subscriptions. Run az login with access to the production and source subscriptions.'
  }

  $matches = @()
  foreach ($subscriptionId in $subscriptionIds) {
    $id = "$subscriptionId".Trim()
    if ([string]::IsNullOrWhiteSpace($id)) {
      continue
    }
    $previousEap = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
      $shown = az group show --name $ResourceGroupName --subscription $id --query id -o tsv 2>$null
      $showExit = $LASTEXITCODE
    }
    finally {
      $ErrorActionPreference = $previousEap
    }
    if ($showExit -eq 0 -and -not [string]::IsNullOrWhiteSpace("$shown")) {
      $matches += $id
    }
  }

  if ($matches.Count -eq 0) {
    if ($Optional) {
      return $null
    }
    throw "Resource group $ResourceGroupName was not found in any subscription this account can see."
  }
  if ($matches.Count -gt 1) {
    throw "Resource group $ResourceGroupName was found in more than one subscription. Refuse to guess."
  }
  return $matches[0]
}

function Test-DertInfoSqlDatabase {
  param(
    [Parameter(Mandatory = $true)][string] $SubscriptionId,
    [Parameter(Mandatory = $true)][string] $ResourceGroupName,
    [Parameter(Mandatory = $true)][string] $ServerName,
    [Parameter(Mandatory = $true)][string] $DatabaseName
  )
  $previousEap = $ErrorActionPreference
  $ErrorActionPreference = 'Continue'
  try {
    az sql db show --subscription $SubscriptionId --resource-group $ResourceGroupName --server $ServerName --name $DatabaseName --query name -o tsv 1>$null 2>$null
    return ($LASTEXITCODE -eq 0)
  }
  finally {
    $ErrorActionPreference = $previousEap
  }
}

function Get-DertInfoSqlDatabaseStatus {
  param(
    [Parameter(Mandatory = $true)][string] $SubscriptionId,
    [Parameter(Mandatory = $true)][string] $ResourceGroupName,
    [Parameter(Mandatory = $true)][string] $ServerName,
    [Parameter(Mandatory = $true)][string] $DatabaseName
  )
  $status = Invoke-AzCli -AzArgs @(
    'sql', 'db', 'show',
    '--subscription', $SubscriptionId,
    '--resource-group', $ResourceGroupName,
    '--server', $ServerName,
    '--name', $DatabaseName,
    '--query', 'status',
    '-o', 'tsv'
  )
  return "$status".Trim()
}

function Wait-DertInfoSqlDatabaseOnline {
  param(
    [Parameter(Mandatory = $true)][string] $SubscriptionId,
    [Parameter(Mandatory = $true)][string] $ResourceGroupName,
    [Parameter(Mandatory = $true)][string] $ServerName,
    [Parameter(Mandatory = $true)][string] $DatabaseName
  )
  $deadline = [DateTime]::UtcNow.Add($copyTimeout)
  Write-Host "Waiting until $DatabaseName is Online (timeout $($copyTimeout.TotalMinutes) minutes)."
  while ([DateTime]::UtcNow -lt $deadline) {
    if (-not (Test-DertInfoSqlDatabase -SubscriptionId $SubscriptionId -ResourceGroupName $ResourceGroupName -ServerName $ServerName -DatabaseName $DatabaseName)) {
      Start-Sleep -Seconds $copyPollSeconds
      continue
    }
    $status = Get-DertInfoSqlDatabaseStatus -SubscriptionId $SubscriptionId -ResourceGroupName $ResourceGroupName -ServerName $ServerName -DatabaseName $DatabaseName
    Write-Host "  status: $status"
    if ($status -eq 'Online') {
      return
    }
    if ($status -match 'Fail') {
      throw "Database $DatabaseName entered status $status."
    }
    Start-Sleep -Seconds $copyPollSeconds
  }
  throw "Timed out waiting for $DatabaseName to become Online."
}

function Test-DertInfoWebApp {
  param(
    [Parameter(Mandatory = $true)][string] $SubscriptionId
  )
  $previousEap = $ErrorActionPreference
  $ErrorActionPreference = 'Continue'
  try {
    az webapp show --subscription $SubscriptionId --resource-group $webAppRg --name $webAppName --query name -o tsv 1>$null 2>$null
    return ($LASTEXITCODE -eq 0)
  }
  finally {
    $ErrorActionPreference = $previousEap
  }
}

function Set-DertInfoWebAppState {
  param(
    [Parameter(Mandatory = $true)][ValidateSet('stop', 'start')][string] $Action,
    [Parameter(Mandatory = $true)][string] $SubscriptionId
  )
  Write-Host "$Action $webAppName"
  $null = Invoke-AzCli -AzArgs @(
    'webapp', $Action,
    '--subscription', $SubscriptionId,
    '--resource-group', $webAppRg,
    '--name', $webAppName
  )
}

function Rename-DertInfoSqlDatabase {
  param(
    [Parameter(Mandatory = $true)][string] $SubscriptionId,
    [Parameter(Mandatory = $true)][string] $FromName,
    [Parameter(Mandatory = $true)][string] $ToName
  )
  Write-Host "Rename $FromName -> $ToName"
  $null = Invoke-AzCli -AzArgs @(
    'sql', 'db', 'rename',
    '--subscription', $SubscriptionId,
    '--resource-group', $destRg,
    '--server', $destServer,
    '--name', $FromName,
    '--new-name', $ToName
  )
}

function Remove-DertInfoSqlDatabase {
  param(
    [Parameter(Mandatory = $true)][string] $SubscriptionId,
    [Parameter(Mandatory = $true)][string] $DatabaseName
  )
  Write-Host "Delete $DatabaseName"
  $null = Invoke-AzCli -AzArgs @(
    'sql', 'db', 'delete',
    '--subscription', $SubscriptionId,
    '--resource-group', $destRg,
    '--server', $destServer,
    '--name', $DatabaseName,
    '--yes'
  )
}

Assert-AzCli

$srcRg = $SourceResourceGroup.Trim()
$srcServer = $SourceServer.Trim()
$srcDb = $SourceDatabase.Trim()
if ([string]::IsNullOrWhiteSpace($srcRg) -or [string]::IsNullOrWhiteSpace($srcServer) -or [string]::IsNullOrWhiteSpace($srcDb)) {
  throw 'SourceResourceGroup, SourceServer, and SourceDatabase are required.'
}
if ($srcDb.StartsWith('-')) {
  throw 'Source database name looks like a switch (for example -SourceDatabase). Put each parameter on its own line with a backtick at the end of the previous line, not immediately before -SourceDatabase.'
}
if ($srcServer -like '*.database.windows.net') {
  $srcServer = $srcServer.Substring(0, $srcServer.Length - '.database.windows.net'.Length)
}
if ($srcServer -eq $destServer -and $srcDb -eq $officialName) {
  throw "Source $srcServer/$srcDb is the production database. Refusing to copy a database onto itself."
}

Write-Host "Resolving resource groups across every subscription this az login can see (not only the current default)."
$destSub = Get-DertInfoSubscriptionForResourceGroup -ResourceGroupName $destRg
$srcSub = Get-DertInfoSubscriptionForResourceGroup -ResourceGroupName $srcRg
$apiSub = Get-DertInfoSubscriptionForResourceGroup -ResourceGroupName $webAppRg -Optional
Write-Host "Source $srcRg / $srcServer / $srcDb -> subscription $srcSub"
Write-Host "Dest   $destRg / $destServer -> subscription $destSub"

$apiExists = $false
if ($null -ne $apiSub) {
  if ("$apiSub" -ne "$destSub") {
    throw "API resource group $webAppRg is not in the same subscription as $destRg."
  }
  $apiExists = Test-DertInfoWebApp -SubscriptionId $apiSub
}
if ($apiExists) {
  Write-Host "API $webAppName exists. It will be stopped for the rename and started after the user bind."
}
else {
  Write-Host "API $webAppName was not found. Copy continues. Bind the App Service user after API infra CD."
}

if (-not (Test-DertInfoSqlDatabase -SubscriptionId $srcSub -ResourceGroupName $srcRg -ServerName $srcServer -DatabaseName $srcDb)) {
  throw "Source database $srcDb was not found on $srcServer in $srcRg."
}
if (-not (Test-DertInfoSqlDatabase -SubscriptionId $destSub -ResourceGroupName $destRg -ServerName $destServer -DatabaseName $officialName)) {
  throw "Expected existing production database $officialName on $destServer. Storage infra CD must have created it."
}
if (Test-DertInfoSqlDatabase -SubscriptionId $destSub -ResourceGroupName $destRg -ServerName $destServer -DatabaseName $copyName) {
  throw "Sidecar $copyName already exists. Delete or rename it before running this script."
}
if (Test-DertInfoSqlDatabase -SubscriptionId $destSub -ResourceGroupName $destRg -ServerName $destServer -DatabaseName $oldName) {
  throw "Previous production database $oldName already exists. Finish or clean up the last run before starting another."
}

$usedBytesText = Invoke-AzCli -AzArgs @(
  'sql', 'db', 'list-usages',
  '--subscription', $srcSub,
  '--resource-group', $srcRg,
  '--server', $srcServer,
  '--name', $srcDb,
  '--query', "[?name=='database_size'].currentValue | [0]",
  '-o', 'tsv'
)
$usedBytesText = "$usedBytesText".Trim()
if ($usedBytesText -match '^\d+(\.\d+)?$') {
  $usedBytes = [int64][Math]::Floor([double]$usedBytesText)
  Write-Host "Source database_size $usedBytes bytes (Basic cap $basicMaxBytes)."
  if ($usedBytes -gt $basicMaxBytes) {
    throw "Source database is $usedBytes bytes. Production SQL is Basic with a 2 GB cap ($basicMaxBytes). Refusing to copy."
  }
}
else {
  Write-Host 'Could not read source database_size. Continuing. The Basic 2 GB cap can still reject the copy.'
}

$sourceDatabaseId = Invoke-AzCli -AzArgs @(
  'sql', 'db', 'show',
  '--subscription', $srcSub,
  '--resource-group', $srcRg,
  '--server', $srcServer,
  '--name', $srcDb,
  '--query', 'id',
  '-o', 'tsv'
)
$sourceDatabaseId = "$sourceDatabaseId".Trim()
$location = Invoke-AzCli -AzArgs @(
  'sql', 'server', 'show',
  '--subscription', $destSub,
  '--resource-group', $destRg,
  '--name', $destServer,
  '--query', 'location',
  '-o', 'tsv'
)
$location = "$location".Trim()

Write-Host "Copy $srcServer/$srcDb -> $destServer/$copyName (createMode Copy, Basic)."
$copyUri = "https://management.azure.com/subscriptions/$destSub/resourceGroups/$destRg/providers/Microsoft.Sql/servers/$destServer/databases/${copyName}?api-version=2023-08-01"
$copyBody = @"
{
  "location": "$location",
  "sku": { "name": "Basic", "tier": "Basic", "capacity": 5 },
  "properties": {
    "createMode": "Copy",
    "sourceDatabaseId": "$sourceDatabaseId",
    "requestedBackupStorageRedundancy": "Local"
  }
}
"@
$bodyFile = Join-Path ([System.IO.Path]::GetTempPath()) ("dertinfo-sql-copy-prd-" + [Guid]::NewGuid().ToString('n') + '.json')
try {
  $utf8NoBom = New-Object System.Text.UTF8Encoding $false
  [System.IO.File]::WriteAllText($bodyFile, $copyBody, $utf8NoBom)
  $null = Invoke-AzCli -AzArgs @(
    'rest',
    '--method', 'put',
    '--uri', $copyUri,
    '--headers', 'Content-Type=application/json',
    '--body', "@$bodyFile"
  )
}
finally {
  if (Test-Path -LiteralPath $bodyFile) {
    Remove-Item -LiteralPath $bodyFile -Force
  }
}

Wait-DertInfoSqlDatabaseOnline -SubscriptionId $destSub -ResourceGroupName $destRg -ServerName $destServer -DatabaseName $copyName

if ($apiExists) {
  Set-DertInfoWebAppState -Action stop -SubscriptionId $apiSub
}
Rename-DertInfoSqlDatabase -SubscriptionId $destSub -FromName $officialName -ToName $oldName
Wait-DertInfoSqlDatabaseOnline -SubscriptionId $destSub -ResourceGroupName $destRg -ServerName $destServer -DatabaseName $oldName
Rename-DertInfoSqlDatabase -SubscriptionId $destSub -FromName $copyName -ToName $officialName
Wait-DertInfoSqlDatabaseOnline -SubscriptionId $destSub -ResourceGroupName $destRg -ServerName $destServer -DatabaseName $officialName

$bindArgs = @{
  GitHubEnvironment = 'production'
}
if (-not [string]::IsNullOrWhiteSpace($UserName)) {
  $bindArgs['UserName'] = $UserName
}

Write-Host 'Binding production Entra database users on the restored database.'
& "$PSScriptRoot\New-DertInfoSqlDbAccessUser.ps1" @bindArgs
if ($apiExists) {
  & "$PSScriptRoot\New-DertInfoSqlAppServiceUser.ps1" @bindArgs
  Set-DertInfoWebAppState -Action start -SubscriptionId $apiSub
}

Write-Host ''
Write-Host "Production database name is now the copy: $officialName."
Write-Host "Previous database is $oldName."
if ($apiExists) {
  Write-Host "Smoke-test after API Src CD: $swaggerUrl"
}
else {
  Write-Host "After API infra CD, run .\Database\New-DertInfoSqlAppServiceUser.ps1 -GitHubEnvironment production"
}
Write-Host 'Continue = keep the copy and delete the previous production database.'
Write-Host 'Revert    = put the previous production database back and delete the copy.'
$choice = ''
while ($choice -notin @('Continue', 'Revert')) {
  $choice = (Read-Host 'Type Continue or Revert').Trim()
}

if ($choice -eq 'Continue') {
  Remove-DertInfoSqlDatabase -SubscriptionId $destSub -DatabaseName $oldName
  Write-Host "Done. $officialName on $destServer is the live copy. Previous production database deleted."
  return
}

if ($apiExists) {
  Set-DertInfoWebAppState -Action stop -SubscriptionId $apiSub
}
Rename-DertInfoSqlDatabase -SubscriptionId $destSub -FromName $officialName -ToName $copyName
Wait-DertInfoSqlDatabaseOnline -SubscriptionId $destSub -ResourceGroupName $destRg -ServerName $destServer -DatabaseName $copyName
Rename-DertInfoSqlDatabase -SubscriptionId $destSub -FromName $oldName -ToName $officialName
Wait-DertInfoSqlDatabaseOnline -SubscriptionId $destSub -ResourceGroupName $destRg -ServerName $destServer -DatabaseName $officialName
if ($apiExists) {
  Set-DertInfoWebAppState -Action start -SubscriptionId $apiSub
}
Remove-DertInfoSqlDatabase -SubscriptionId $destSub -DatabaseName $copyName
Write-Host "Reverted. $officialName is the previous production database. Live copy deleted."
