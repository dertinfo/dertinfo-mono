<#
.SYNOPSIS
  Shared catalog and secrets-file helpers for the App Configuration scripts.

.DESCRIPTION
  Dot-sourced by Import-DertInfoAppConfiguration.ps1, Export-DertInfoAppConfiguration.ps1,
  and New-DertInfoConfigKeyVaultSecrets.ps1. It does not call Azure.

  Resolves infra/configuration/app-config.<environment>.json and
  kv-secrets.<environment>.json, or an explicit path. Loads the catalog and
  requires appConfigurationName, appConfigurationLabel, keyVaultName, secrets,
  and keyVaultReferences. Loads the gitignored secrets JSON, or throws with
  the .example copy hint.

  At dot-source time, $PSScriptRoot is infra/scripts/Configuration. Running this file defines
  the helpers for that invocation only.

  As an operator I load this script from the configuration scripts to resolve the catalog and secrets files because I want those scripts to share one implementation.

.EXAMPLE
  . .\Configuration\Shared-DertInfoAppConfigCatalog.ps1
#>

$script:DertInfoScriptsDir = $PSScriptRoot

function Resolve-DertInfoAppConfigCatalogPath {
  param(
    [string] $GitHubEnvironment,
    [string] $ConfigFile
  )

  if (-not [string]::IsNullOrWhiteSpace($ConfigFile)) {
    return $ConfigFile
  }
  if ([string]::IsNullOrWhiteSpace($GitHubEnvironment)) {
    throw 'Specify -ConfigFile or -GitHubEnvironment.'
  }
  $root = Split-Path -Parent (Split-Path -Parent $script:DertInfoScriptsDir)
  return (Join-Path $root "configuration\app-config.$GitHubEnvironment.json")
}

function Resolve-DertInfoAppConfigSecretsPath {
  param(
    [string] $GitHubEnvironment,
    [string] $SecretsFile
  )

  if (-not [string]::IsNullOrWhiteSpace($SecretsFile)) {
    return $SecretsFile
  }
  if ([string]::IsNullOrWhiteSpace($GitHubEnvironment)) {
    throw 'Specify -SecretsFile or -GitHubEnvironment.'
  }

  $root = Split-Path -Parent (Split-Path -Parent $script:DertInfoScriptsDir)
  return (Join-Path $root "configuration\kv-secrets.$GitHubEnvironment.json")
}

function Get-DertInfoAppConfigSecrets {
  param(
    [string] $GitHubEnvironment,
    [string] $SecretsFile
  )

  $path = Resolve-DertInfoAppConfigSecretsPath -GitHubEnvironment $GitHubEnvironment -SecretsFile $SecretsFile
  $examplePath = "$path.example"
  if (-not (Test-Path -LiteralPath $path)) {
    $hint = "Copy the example and fill in values, then re-run."
    if (Test-Path -LiteralPath $examplePath) {
      $hint = "Copy `"$examplePath`" to `"$path`" and fill in values, then re-run."
    }
    throw "Secrets JSON not found: $path. $hint"
  }

  $secrets = Get-Content -LiteralPath $path -Raw | ConvertFrom-Json
  Write-Host "Secrets file: $path"
  return $secrets
}

function Get-DertInfoAppConfigCatalog {
  param(
    [string] $GitHubEnvironment,
    [string] $ConfigFile
  )

  $path = Resolve-DertInfoAppConfigCatalogPath -GitHubEnvironment $GitHubEnvironment -ConfigFile $ConfigFile
  if (-not (Test-Path -LiteralPath $path)) {
    throw "Catalog JSON not found: $path"
  }

  $catalog = Get-Content -LiteralPath $path -Raw | ConvertFrom-Json
  foreach ($required in @('appConfigurationName', 'appConfigurationLabel', 'keyVaultName')) {
    if ([string]::IsNullOrWhiteSpace("$($catalog.$required)")) {
      throw "Catalog $path is missing '$required'."
    }
  }
  if ($null -eq $catalog.secrets) {
    throw "Catalog $path is missing 'secrets'."
  }
  if ($null -eq $catalog.keyVaultReferences) {
    throw "Catalog $path is missing 'keyVaultReferences'."
  }

  Write-Host "Catalog: $path"
  return $catalog
}
