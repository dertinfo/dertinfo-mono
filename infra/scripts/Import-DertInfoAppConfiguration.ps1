<#
.SYNOPSIS
  Import App Configuration keys from the catalog (and optionally a dump), then apply Key Vault references.

.DESCRIPTION
  1. If -Path is set, az appconfig kv import from that dump (dry-run unless -Force).
  2. If the catalog has keyValues, set those keys with the catalog label (printed unless -Force).
  3. After -Force, set Key Vault references from the catalog so URIs point at this Environment's vault.

  All az appconfig calls use --auth-mode login (Entra). You need App Configuration Data Owner
  (or equivalent) on the store; access keys are not used.

.PARAMETER GitHubEnvironment
  Selects infra/configuration/app-config.<environment>.json (destination store).

.PARAMETER ConfigFile
  Catalog JSON path. Overrides GitHubEnvironment default path.

.PARAMETER Path
  Optional dump from Export-DertInfoAppConfiguration.ps1. If omitted, only catalog
  keyValues and Key Vault references are applied.

.PARAMETER Force
  Apply key-values and Key Vault references (without this switch, dump import is
  --dry-run and catalog keyValues are listed only).

.EXAMPLE
  .\Import-DertInfoAppConfiguration.ps1 -GitHubEnvironment development
  .\Import-DertInfoAppConfiguration.ps1 -GitHubEnvironment development -Force
#>
[CmdletBinding()]
param(
  [ValidateSet('development', 'production')]
  [string] $GitHubEnvironment = '',

  [string] $ConfigFile = '',

  [string] $Path = '',

  [switch] $Force
)

$ErrorActionPreference = 'Stop'

. "$PSScriptRoot\DertInfoAppConfigCatalog.ps1"

function Assert-AzCli {
  if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    throw 'Azure CLI (az) is required on PATH.'
  }
  az account show 1>$null 2>$null
  if ($LASTEXITCODE -ne 0) {
    throw 'Not logged in to Azure CLI. Run: az login'
  }
}

function Test-DertInfoSecretAppConfigKey {
  param(
    [string] $Key,
    $Catalog
  )
  if ($Key -like 'StorageConnection:*') {
    return $true
  }
  foreach ($ref in $Catalog.keyVaultReferences) {
    if ("$($ref.key)" -eq $Key) {
      return $true
    }
  }
  return $false
}

function ConvertTo-AzNativeJsonArgument {
  param([Parameter(Mandatory = $true)]$InputObject)
  # Windows PowerShell strips " when calling az.exe; escape so the CLI receives real JSON.
  $json = $InputObject | ConvertTo-Json -Compress -Depth 5
  return (($json -replace '\\', '\\') -replace '"', '\"')
}

$kvRefContentType = 'application/vnd.microsoft.appconfig.keyvaultref+json;charset=utf-8'

Assert-AzCli
$catalog = Get-DertInfoAppConfigCatalog -GitHubEnvironment $GitHubEnvironment -ConfigFile $ConfigFile
$store = $catalog.appConfigurationName
$label = $catalog.appConfigurationLabel
$vault = $catalog.keyVaultName

$hasDump = -not [string]::IsNullOrWhiteSpace($Path)
$hasKeyValues = $null -ne $catalog.keyValues
if ($hasDump -and -not (Test-Path -LiteralPath $Path)) {
  throw "Dump not found: $Path"
}
if (-not $hasDump -and -not $hasKeyValues) {
  throw 'Catalog has no keyValues and -Path was not set. Add keyValues to the catalog or pass -Path to a dump.'
}

if ($hasDump) {
  $importArgs = @(
    'appconfig', 'kv', 'import',
    '--name', $store,
    '--source', 'file',
    '--path', $Path,
    '--format', 'json',
    '--auth-mode', 'login',
    '--yes'
  )
  if (-not $Force) {
    $importArgs += '--dry-run'
    Write-Host "Dry-run import into $store from $Path (pass -Force to apply)"
  }
  else {
    Write-Host "Importing $Path -> $store"
  }

  az @importArgs
  if ($LASTEXITCODE -ne 0) {
    throw "az appconfig kv import failed with exit $LASTEXITCODE"
  }
}

$keyValueProps = @()
if ($hasKeyValues) {
  $keyValueProps = @($catalog.keyValues.PSObject.Properties)
}

if (-not $Force) {
  if ($hasKeyValues) {
    Write-Host "Dry-run catalog keyValues for $store (label $label):"
    foreach ($p in $keyValueProps) {
      if (Test-DertInfoSecretAppConfigKey -Key $p.Name -Catalog $catalog) {
        Write-Warning "Skipping secret key in catalog: $($p.Name)"
        continue
      }
      Write-Host "  $($p.Name)"
    }
  }
  Write-Host 'Dry-run only. Re-run with -Force to apply, then Key Vault references will be set.'
  return
}

if ($hasKeyValues) {
  Write-Host "Setting catalog keyValues on $store (label $label)"
  foreach ($p in $keyValueProps) {
    if (Test-DertInfoSecretAppConfigKey -Key $p.Name -Catalog $catalog) {
      Write-Warning "Skipping secret key in catalog: $($p.Name)"
      continue
    }
    $value = "$($p.Value)"
    Write-Host "  $($p.Name)"
    az appconfig kv set `
      --name $store `
      --key $p.Name `
      --label $label `
      --value $value `
      --auth-mode login `
      --yes `
      --only-show-errors `
      -o none
    if ($LASTEXITCODE -ne 0) {
      throw "az appconfig kv set failed for $($p.Name) (exit $LASTEXITCODE)"
    }
  }
}

Write-Host "Setting Key Vault references on $store (label $label) -> vault $vault"
foreach ($ref in $catalog.keyVaultReferences) {
  $uriJson = ConvertTo-AzNativeJsonArgument -InputObject @{
    uri = "https://$vault.vault.azure.net/secrets/$($ref.secretName)"
  }
  Write-Host "  $($ref.key) -> $($ref.secretName)"
  az appconfig kv set `
    --name $store `
    --key $ref.key `
    --label $label `
    --value $uriJson `
    --content-type $kvRefContentType `
    --auth-mode login `
    --yes `
    --only-show-errors `
    -o none
  if ($LASTEXITCODE -ne 0) {
    throw "az appconfig kv set failed for $($ref.key) (exit $LASTEXITCODE)"
  }
}

Write-Host 'Done. Set Key Vault secret values with New-DertInfoConfigKeyVaultSecrets.ps1 if needed.'
