<#
.SYNOPSIS
  Export non-secret App Configuration key-values to a JSON file (--skip-keyvault).

.DESCRIPTION
  Wraps: az appconfig kv export --destination file --format json --skip-keyvault
  --auth-mode login (Entra). Does not resolve Key Vault references (no secret
  values on disk). Store name and optional label come from the catalog JSON.

.PARAMETER GitHubEnvironment
  Selects infra/configuration/app-config.<environment>.json

.PARAMETER ConfigFile
  Catalog JSON path. Overrides GitHubEnvironment default path.

.PARAMETER Path
  Output dump path. Default infra/secrets/appconfig-export.<environment>.json

.PARAMETER Name
  Override source App Configuration store name (for exporting an older store).

.PARAMETER AllLabels
  Export every label. Default uses appConfigurationLabel from the catalog.

.EXAMPLE
  .\Export-DertInfoAppConfiguration.ps1 -GitHubEnvironment development
#>
[CmdletBinding()]
param(
  [ValidateSet('development', 'production')]
  [string] $GitHubEnvironment = '',

  [string] $ConfigFile = '',

  [string] $Path = '',

  [string] $Name = '',

  [switch] $AllLabels
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

Assert-AzCli
$catalog = Get-DertInfoAppConfigCatalog -GitHubEnvironment $GitHubEnvironment -ConfigFile $ConfigFile

$store = $Name
if ([string]::IsNullOrWhiteSpace($store)) {
  $store = $catalog.appConfigurationName
}

if ([string]::IsNullOrWhiteSpace($Path)) {
  $envSlug = $catalog.githubEnvironment
  if ([string]::IsNullOrWhiteSpace($envSlug)) {
    $envSlug = $GitHubEnvironment
  }
  if ([string]::IsNullOrWhiteSpace($envSlug)) {
    $envSlug = 'export'
  }
  $secretsDir = Join-Path (Split-Path -Parent $script:DertInfoScriptsDir) 'secrets'
  $Path = Join-Path $secretsDir "appconfig-export.$envSlug.json"
}

$dir = Split-Path -Parent $Path
if (-not [string]::IsNullOrWhiteSpace($dir) -and -not (Test-Path -LiteralPath $dir)) {
  New-Item -ItemType Directory -Path $dir | Out-Null
}

$azArgs = @(
  'appconfig', 'kv', 'export',
  '--name', $store,
  '--destination', 'file',
  '--path', $Path,
  '--format', 'json',
  '--skip-keyvault',
  '--auth-mode', 'login',
  '--yes'
)
if (-not $AllLabels) {
  $azArgs += @('--label', $catalog.appConfigurationLabel)
}

Write-Host "Exporting $store -> $Path (skip-keyvault)"
az @azArgs
if ($LASTEXITCODE -ne 0) {
  throw "az appconfig kv export failed with exit $LASTEXITCODE"
}
Write-Host "Wrote $Path"
