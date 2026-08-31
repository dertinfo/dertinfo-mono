<#
.SYNOPSIS
  Insert hosted API secrets into the config Key Vault from the catalog JSON.

.DESCRIPTION
  Secret names and vault name come from
  infra/configuration/app-config.<environment>.json (or -ConfigFile).
  Values come from the gitignored
  kv-secrets.<environment>.json (copy from kv-secrets.<environment>.json.example).
  Values are not printed. Skips a name that already exists unless -Force.
  Run after config infra CD has created the vault. Uses az login (Key Vault
  RBAC: Key Vault Secrets Officer). Does not use access policies or keys.

.PARAMETER GitHubEnvironment
  Selects infra/configuration/app-config.<environment>.json and
  kv-secrets.<environment>.json

.PARAMETER ConfigFile
  Catalog JSON path. Overrides GitHubEnvironment default path.

.PARAMETER SecretsFile
  Secrets JSON path. Default infra/configuration/kv-secrets.<environment>.json.

.PARAMETER VaultName
  Override keyVaultName from the catalog.

.PARAMETER Force
  Overwrite secrets that already exist.

.EXAMPLE
  Copy-Item infra/configuration/kv-secrets.development.json.example `
    infra/configuration/kv-secrets.development.json
  .\New-DertInfoConfigKeyVaultSecrets.ps1 -GitHubEnvironment development
#>
[CmdletBinding()]
param(
  [ValidateSet('development', 'production')]
  [string] $GitHubEnvironment = '',

  [string] $ConfigFile = '',

  [string] $SecretsFile = '',

  [string] $VaultName = '',

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

function Test-KeyVaultSecretExists {
  param(
    [Parameter(Mandatory = $true)][string] $Vault,
    [Parameter(Mandatory = $true)][string] $Name
  )
  $previousEap = $ErrorActionPreference
  $ErrorActionPreference = 'Continue'
  try {
    az keyvault secret show --vault-name $Vault --name $Name --query id -o tsv 1>$null 2>$null
    return ($LASTEXITCODE -eq 0)
  }
  finally {
    $ErrorActionPreference = $previousEap
  }
}

function Get-DertInfoSecretFileValue {
  param(
    $Secrets,
    [string] $Name
  )

  $prop = $Secrets.PSObject.Properties[$Name]
  if ($null -eq $prop) {
    throw "Secrets file is missing '$Name'."
  }
  $value = "$($prop.Value)"
  if ([string]::IsNullOrWhiteSpace($value)) {
    throw "Secrets file has an empty value for '$Name'. Fill it in and re-run."
  }
  return $value
}

Assert-AzCli
$catalog = Get-DertInfoAppConfigCatalog -GitHubEnvironment $GitHubEnvironment -ConfigFile $ConfigFile
$secretsEnvironment = $GitHubEnvironment
if ([string]::IsNullOrWhiteSpace($secretsEnvironment)) {
  $secretsEnvironment = "$($catalog.githubEnvironment)"
}
$secrets = Get-DertInfoAppConfigSecrets -GitHubEnvironment $secretsEnvironment -SecretsFile $SecretsFile

if ([string]::IsNullOrWhiteSpace($VaultName)) {
  $VaultName = $catalog.keyVaultName
}

Write-Host "Key Vault: $VaultName"
Write-Host 'Values are read from the secrets file and are not echoed. Existing secrets are skipped unless -Force.'
Write-Host ''

foreach ($item in $catalog.secrets) {
  $name = $item.name
  if ([string]::IsNullOrWhiteSpace($name)) {
    throw 'Catalog secrets[] entry is missing name.'
  }

  $exists = Test-KeyVaultSecretExists -Vault $VaultName -Name $name
  if ($exists -and -not $Force) {
    Write-Host "Skipping $name (already exists)."
    continue
  }

  $plain = Get-DertInfoSecretFileValue -Secrets $secrets -Name $name
  Write-Host "Setting $name"
  az keyvault secret set --vault-name $VaultName --name $name --value $plain --only-show-errors -o none
  $plain = $null
  if ($LASTEXITCODE -ne 0) {
    throw "az keyvault secret set failed for $name (exit $LASTEXITCODE)."
  }
}

Write-Host ''
Write-Host 'Done. App Configuration Key Vault references use these names. Restart the API after values change.'
