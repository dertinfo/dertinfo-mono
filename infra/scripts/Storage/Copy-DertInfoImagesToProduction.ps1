<#
.SYNOPSIS
  Copy image containers from one or more storage accounts onto stprddertinfoimagesuks.

.DESCRIPTION
  Stops func-prd-dertinfo-functions-uks, copies groupimages, eventimages, sheetimages,
  and defaultimages from each -SourceStorageAccount onto stprddertinfoimagesuks, then
  starts the Function App again. A missing container on a source account is skipped.
  This script does not enable Event Grid. Source account names are parameters because
  the old estate used more than one account, and an Eventbrite store is just another
  name in that list. Run azcopy login before this script.

  As an operator I run this script to copy production images onto the new account because I want the Azure-hostname site to show live pictures before DNS changes.

.PARAMETER SourceStorageAccount
  One or more source storage account names (no URL, no hyphens required beyond the account name).

.EXAMPLE
  .\Storage\Copy-DertInfoImagesToProduction.ps1 -SourceStorageAccount 'myimagesaccount'

.EXAMPLE
  .\Storage\Copy-DertInfoImagesToProduction.ps1 -SourceStorageAccount 'myimagesaccount','myeventbriteaccount'
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory = $true)]
  [string[]] $SourceStorageAccount
)

$ErrorActionPreference = 'Stop'

$destAccount = 'stprddertinfoimagesuks'
$functionRg = 'rg-prd-dertinfo-functions-uks'
$functionApp = 'func-prd-dertinfo-functions-uks'
$containers = @('groupimages', 'eventimages', 'sheetimages', 'defaultimages')

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
    throw 'Could not list Azure subscriptions. Run az login.'
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
    throw "Resource group $ResourceGroupName was not found. Functions infra CD must have created it."
  }
  if ($matches.Count -gt 1) {
    throw "Resource group $ResourceGroupName was found in more than one subscription. Refuse to guess."
  }
  return $matches[0]
}

function Test-DertInfoBlobContainer {
  param(
    [Parameter(Mandatory = $true)][string] $AccountName,
    [Parameter(Mandatory = $true)][string] $ContainerName
  )
  $previousEap = $ErrorActionPreference
  $ErrorActionPreference = 'Continue'
  try {
    $exists = az storage container exists --account-name $AccountName --name $ContainerName --auth-mode login --query exists -o tsv 2>$null
    $azExit = $LASTEXITCODE
  }
  finally {
    $ErrorActionPreference = $previousEap
  }
  if ($azExit -ne 0) {
    return $null
  }
  return ("$exists".Trim() -eq 'true')
}

Assert-AzCli
if (-not (Get-Command azcopy -ErrorAction SilentlyContinue)) {
  throw 'AzCopy (azcopy) is required on PATH. Install AzCopy v10, then run: azcopy login'
}

$sources = @()
foreach ($name in $SourceStorageAccount) {
  $trimmed = "$name".Trim()
  if ([string]::IsNullOrWhiteSpace($trimmed)) {
    continue
  }
  if ($trimmed -eq $destAccount) {
    throw "Source account $trimmed is the production images account. Refusing to copy an account onto itself."
  }
  $sources += $trimmed
}
if ($sources.Count -eq 0) {
  throw 'Pass at least one -SourceStorageAccount name.'
}

$functionSub = Get-DertInfoSubscriptionForResourceGroup -ResourceGroupName $functionRg
Write-Host "Destination account: $destAccount"
Write-Host "Source accounts: $($sources -join ', ')"
Write-Host "Stop $functionApp"
$null = Invoke-AzCli -AzArgs @(
  'functionapp', 'stop',
  '--subscription', $functionSub,
  '--resource-group', $functionRg,
  '--name', $functionApp
)

$failures = New-Object System.Collections.Generic.List[string]
try {
  foreach ($source in $sources) {
    foreach ($container in $containers) {
      $exists = Test-DertInfoBlobContainer -AccountName $source -ContainerName $container
      if ($exists -eq $false) {
        Write-Host "Skip $source/$container (container does not exist)."
        continue
      }
      if ($null -eq $exists) {
        Write-Host "Could not check $source/$container. Attempting the copy."
      }
      $from = "https://$source.blob.core.windows.net/$container"
      $to = "https://$destAccount.blob.core.windows.net/$container"
      Write-Host "Copy $from -> $to"
      & azcopy copy $from $to --recursive
      if ($LASTEXITCODE -ne 0) {
        $failures.Add("$source/$container (azcopy exit $LASTEXITCODE)")
        Write-Host "Failed $source/$container."
      }
    }
  }
}
finally {
  Write-Host "Start $functionApp"
  $null = Invoke-AzCli -AzArgs @(
    'functionapp', 'start',
    '--subscription', $functionSub,
    '--resource-group', $functionRg,
    '--name', $functionApp
  )
}

Write-Host ''
Write-Host "Destination: $destAccount"
Write-Host "Function app $functionApp was started."
Write-Host 'Event Grid was not changed. After every source account has been copied, set flagImagesEventGridReady and run Storage infra CD.'
if ($failures.Count -gt 0) {
  throw ("AzCopy failed for: " + ($failures -join '; ') + '. If the error is login, run: azcopy login')
}
Write-Host 'Copy finished.'
