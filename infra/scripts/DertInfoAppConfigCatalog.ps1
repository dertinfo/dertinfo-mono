# Dot-source from the same folder: . "$PSScriptRoot\DertInfoAppConfigCatalog.ps1"
# $PSScriptRoot here is this file's directory (infra/scripts) at dot-source time.

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
  $root = Split-Path -Parent $script:DertInfoScriptsDir
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

  $root = Split-Path -Parent $script:DertInfoScriptsDir
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
