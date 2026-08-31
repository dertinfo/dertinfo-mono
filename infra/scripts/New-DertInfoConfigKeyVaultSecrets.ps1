<#
.SYNOPSIS
  Insert the four hosted API secrets into the config Key Vault (one Environment).

.DESCRIPTION
  Creates or updates:
  - auth0-managementclientsecret
  - az-storage-accountkey
  - mailgun-apikey
  - sendgrid-apikey

  Prompts for each value (secure input). Does not print the values.
  Skips a name that already exists unless -Force (rotation).
  Run after config infra CD has created kv-<dev|prd>-dertinfo-uks.
  App Configuration Key Vault references are deployed by config Bicep; this script only sets secret values.

.PARAMETER GitHubEnvironment
  GitHub Environment name: development or production.

.PARAMETER VaultName
  Override the Key Vault name. Default kv-dev-dertinfo-uks or kv-prd-dertinfo-uks.

.PARAMETER Force
  Overwrite secrets that already exist.

.EXAMPLE
  .\New-DertInfoConfigKeyVaultSecrets.ps1 -GitHubEnvironment development
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory = $true)]
  [ValidateSet('development', 'production')]
  [string] $GitHubEnvironment,

  [string] $VaultName = '',

  [switch] $Force
)

$ErrorActionPreference = 'Stop'

function Assert-AzCli {
  if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    throw 'Azure CLI (az) is required on PATH.'
  }
  az account show 1>$null 2>$null
  if ($LASTEXITCODE -ne 0) {
    throw 'Not logged in to Azure CLI. Run: az login'
  }
}

function ConvertFrom-SecureStringPlain {
  param([Parameter(Mandatory = $true)][securestring] $Secure)
  $bstr = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($Secure)
  try {
    return [Runtime.InteropServices.Marshal]::PtrToStringBSTR($bstr)
  }
  finally {
    [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($bstr)
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

Assert-AzCli

$envTag = if ($GitHubEnvironment -eq 'production') { 'prd' } else { 'dev' }
if ([string]::IsNullOrWhiteSpace($VaultName)) {
  $VaultName = "kv-$envTag-dertinfo-uks"
}

$secrets = @(
  @{ Name = 'auth0-managementclientsecret'; Prompt = 'Auth0 Management Client secret' }
  @{ Name = 'az-storage-accountkey'; Prompt = 'Images storage account key' }
  @{ Name = 'mailgun-apikey'; Prompt = 'Mailgun API key' }
  @{ Name = 'sendgrid-apikey'; Prompt = 'SendGrid API key' }
)

Write-Host "Key Vault: $VaultName (Environment $GitHubEnvironment)"
Write-Host 'Values are not echoed. Existing secrets are skipped unless -Force.'
Write-Host ''

foreach ($item in $secrets) {
  $name = $item.Name
  $exists = Test-KeyVaultSecretExists -Vault $VaultName -Name $name
  if ($exists -and -not $Force) {
    Write-Host "Skipping $name (already exists)."
    continue
  }

  $secure = Read-Host -AsSecureString -Prompt $item.Prompt
  $plain = ConvertFrom-SecureStringPlain -Secure $secure
  if ([string]::IsNullOrWhiteSpace($plain)) {
    throw "Empty value for $name. Aborting."
  }

  Write-Host "Setting $name"
  az keyvault secret set --vault-name $VaultName --name $name --value $plain --only-show-errors -o none
  $plain = $null
  if ($LASTEXITCODE -ne 0) {
    throw "az keyvault secret set failed for $name (exit $LASTEXITCODE)."
  }
}

Write-Host ''
Write-Host 'Done. Config Bicep Key Vault references use these names. Restart the API after values change.'
