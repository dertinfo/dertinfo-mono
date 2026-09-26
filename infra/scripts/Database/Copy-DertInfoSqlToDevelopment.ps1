<#
.SYNOPSIS
  Copy a production Azure SQL database onto the development server and swap it into the Bicep name.

.DESCRIPTION
  ARM-copies the source database to a sidecar on sql-dev-dertinfo-storage-uks, stops the
  development API, renames the current development database aside, promotes the copy to
  sqldb-dev-dertinfo-storage-uks, binds DEV Entra users, and starts the API. Then pauses
  for Continue (delete the previous DEV database) or Revert (swap back and delete the copy).

  -Source production uses new-stack names from storage Bicep (no subscription ids).
  -Source live requires old-stack server, database, and resource group. Subscriptions are
  resolved by looking up those resource groups under az login.

  Copy is control-plane (does not need Allow Azure services). User bind uses sqlcmd -G
  from this machine (administrator IP). Each bind opens an Entra MFA browser or Windows
  prompt (often behind the terminal). Do not use T-SQL CREATE DATABASE AS COPY OF.

  As an operator I run this script to copy production SQL onto development because I want the development database refreshed and the previous copy still reversible.

.PARAMETER Source
  production = new-stack PRD SQL. live = old-stack production (requires source names).

.PARAMETER SourceResourceGroup
  Required when -Source live. Old-stack SQL resource group.

.PARAMETER SourceServer
  Required when -Source live. Logical server name, with or without
  .database.windows.net (az needs the short name).

.PARAMETER SourceDatabase
  Required when -Source live. Old-stack database name.

.PARAMETER UserName
  Entra login passed to the user-bind scripts. Default: az account show user.name.

.EXAMPLE
  .\Database\Copy-DertInfoSqlToDevelopment.ps1 -Source production

.EXAMPLE
  .\Database\Copy-DertInfoSqlToDevelopment.ps1 -Source live `
    -SourceResourceGroup 'my-live-sql-rg' `
    -SourceServer 'my-live-sql-server' `
    -SourceDatabase 'my-live-database'
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory = $true)]
  [ValidateSet('production', 'live')]
  [string] $Source,

  [string] $SourceResourceGroup = '',
  [string] $SourceServer = '',
  [string] $SourceDatabase = '',
  [string] $UserName = ''
)

$ErrorActionPreference = 'Stop'

$destRg = 'rg-dev-dertinfo-storage-uks'
$destServer = 'sql-dev-dertinfo-storage-uks'
$officialName = 'sqldb-dev-dertinfo-storage-uks'
$copyName = 'sqldb-dev-dertinfo-storage-uks-copy'
$oldName = 'sqldb-dev-dertinfo-storage-uks-old'
$webAppRg = 'rg-dev-dertinfo-api-uks'
$webAppName = 'app-dev-dertinfo-api-uks'
$swaggerUrl = 'https://app-dev-dertinfo-api-uks.azurewebsites.net/swagger/index.html'
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
    [string] $ResourceGroupName
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
    throw 'Could not list Azure subscriptions. Run az login with access to the development and source subscriptions.'
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

if ($Source -eq 'live') {
  if ([string]::IsNullOrWhiteSpace($SourceResourceGroup) -or
    [string]::IsNullOrWhiteSpace($SourceServer) -or
    [string]::IsNullOrWhiteSpace($SourceDatabase)) {
    throw '-Source live requires -SourceResourceGroup, -SourceServer, and -SourceDatabase.'
  }
  $srcRg = $SourceResourceGroup.Trim()
  $srcServer = $SourceServer.Trim()
  $srcDb = $SourceDatabase.Trim()
  if ($srcDb.StartsWith('-')) {
    throw 'Source database name looks like a switch (for example -SourceDatabase). Put each parameter on its own line with a backtick at the end of the previous line, not immediately before -SourceDatabase.'
  }
  if ($srcServer -like '*.database.windows.net') {
    $srcServer = $srcServer.Substring(0, $srcServer.Length - '.database.windows.net'.Length)
  }
}
else {
  if (-not [string]::IsNullOrWhiteSpace($SourceResourceGroup) -or
    -not [string]::IsNullOrWhiteSpace($SourceServer) -or
    -not [string]::IsNullOrWhiteSpace($SourceDatabase)) {
    throw '-Source production does not take -SourceResourceGroup, -SourceServer, or -SourceDatabase. Those names come from storage Bicep.'
  }
  $srcRg = 'rg-prd-dertinfo-storage-uks'
  $srcServer = 'sql-prd-dertinfo-storage-uks'
  $srcDb = 'sqldb-prd-dertinfo-storage-uks'
}

Write-Host "Resolving resource groups across every subscription this az login can see (not only the current default)."
$destSub = Get-DertInfoSubscriptionForResourceGroup -ResourceGroupName $destRg
$srcSub = Get-DertInfoSubscriptionForResourceGroup -ResourceGroupName $srcRg
$apiSub = Get-DertInfoSubscriptionForResourceGroup -ResourceGroupName $webAppRg
Write-Host "Source $srcRg / $srcServer / $srcDb -> subscription $srcSub"
Write-Host "Dest   $destRg / $destServer -> subscription $destSub"
if ($apiSub -ne $destSub) {
  throw "API resource group $webAppRg is not in the same subscription as $destRg."
}

if (-not (Test-DertInfoSqlDatabase -SubscriptionId $srcSub -ResourceGroupName $srcRg -ServerName $srcServer -DatabaseName $srcDb)) {
  throw "Source database $srcDb was not found on $srcServer in $srcRg."
}
if (-not (Test-DertInfoSqlDatabase -SubscriptionId $destSub -ResourceGroupName $destRg -ServerName $destServer -DatabaseName $officialName)) {
  throw "Expected existing development database $officialName on $destServer. Storage infra CD must have created it."
}
if (Test-DertInfoSqlDatabase -SubscriptionId $destSub -ResourceGroupName $destRg -ServerName $destServer -DatabaseName $copyName) {
  throw "Sidecar $copyName already exists. Delete or rename it before running this script."
}
if (Test-DertInfoSqlDatabase -SubscriptionId $destSub -ResourceGroupName $destRg -ServerName $destServer -DatabaseName $oldName) {
  throw "Previous-DEV database $oldName already exists. Finish or clean up the last run before starting another."
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
$bodyFile = Join-Path ([System.IO.Path]::GetTempPath()) ("dertinfo-sql-copy-" + [Guid]::NewGuid().ToString('n') + '.json')
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

Set-DertInfoWebAppState -Action stop -SubscriptionId $destSub
Rename-DertInfoSqlDatabase -SubscriptionId $destSub -FromName $officialName -ToName $oldName
Wait-DertInfoSqlDatabaseOnline -SubscriptionId $destSub -ResourceGroupName $destRg -ServerName $destServer -DatabaseName $oldName
Rename-DertInfoSqlDatabase -SubscriptionId $destSub -FromName $copyName -ToName $officialName
Wait-DertInfoSqlDatabaseOnline -SubscriptionId $destSub -ResourceGroupName $destRg -ServerName $destServer -DatabaseName $officialName

$bindArgs = @{
  GitHubEnvironment = 'development'
}
if (-not [string]::IsNullOrWhiteSpace($UserName)) {
  $bindArgs['UserName'] = $UserName
}

Write-Host 'Binding development Entra database users on the restored database.'
& "$PSScriptRoot\New-DertInfoSqlDbAccessUser.ps1" @bindArgs
& "$PSScriptRoot\New-DertInfoSqlAppServiceUser.ps1" @bindArgs

Set-DertInfoWebAppState -Action start -SubscriptionId $destSub

Write-Host ''
Write-Host "The API now uses the copied production data as $officialName."
Write-Host "Smoke-test: $swaggerUrl"
Write-Host 'Continue = keep the restored database and delete the previous development database.'
Write-Host 'Revert    = put the previous development database back and delete the copy.'
$choice = ''
while ($choice -notin @('Continue', 'Revert')) {
  $choice = (Read-Host 'Type Continue or Revert').Trim()
}

if ($choice -eq 'Continue') {
  Remove-DertInfoSqlDatabase -SubscriptionId $destSub -DatabaseName $oldName
  Write-Host "Done. $officialName is the production copy. Previous development database deleted."
  return
}

Set-DertInfoWebAppState -Action stop -SubscriptionId $destSub
Rename-DertInfoSqlDatabase -SubscriptionId $destSub -FromName $officialName -ToName $copyName
Wait-DertInfoSqlDatabaseOnline -SubscriptionId $destSub -ResourceGroupName $destRg -ServerName $destServer -DatabaseName $copyName
Rename-DertInfoSqlDatabase -SubscriptionId $destSub -FromName $oldName -ToName $officialName
Wait-DertInfoSqlDatabaseOnline -SubscriptionId $destSub -ResourceGroupName $destRg -ServerName $destServer -DatabaseName $officialName
Set-DertInfoWebAppState -Action start -SubscriptionId $destSub
Remove-DertInfoSqlDatabase -SubscriptionId $destSub -DatabaseName $copyName
Write-Host "Reverted. $officialName is the previous development database. Production copy deleted."
