<#
.SYNOPSIS
  Create isolated Entra app registrations + service principals for subscription-scope GitHub Actions OIDC (development and production).

.DESCRIPTION
  For each environment:
  - Creates an app registration and service principal (reuses them if the display name already exists)
  - Applies the checked-in federated credential JSON from infra/configuration/ (skips if that credential name exists)
  - Grants Contributor and User Access Administrator on the matching subscription only (skips if already assigned)

  Safe to re-run. Does not delete existing apps. Throws if multiple apps share the same display name.
  Update GitHub Environment variables with the printed client ids.

  See: docs/technical/guides/github-azure-federated-credentials.md

.PARAMETER DevSubscriptionId
  Azure subscription id for DertInfo Development (GitHub Environment "development").

.PARAMETER PrdSubscriptionId
  Azure subscription id for DertInfo Production (GitHub Environment "production").

.PARAMETER RepoRoot
  Monorepo root containing infra/configuration. Defaults to two levels above this script.

.EXAMPLE
  .\New-DertInfoSubscriptionOidcIdentities.ps1 `
    -DevSubscriptionId '00000000-0000-0000-0000-000000000000' `
    -PrdSubscriptionId '11111111-1111-1111-1111-111111111111'
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory = $true)]
  [ValidatePattern('^[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}$')]
  [string] $DevSubscriptionId,

  [Parameter(Mandatory = $true)]
  [ValidatePattern('^[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}$')]
  [string] $PrdSubscriptionId,

  [Parameter(Mandatory = $false)]
  [string] $RepoRoot = ''
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

function Invoke-Az {
  param(
    [Parameter(Mandatory = $true)][string[]] $Args,
    [switch] $AllowFailure
  )
  if ($AllowFailure) {
    $output = & az @Args 2>$null
  }
  else {
    $output = & az @Args
  }
  if ($LASTEXITCODE -ne 0 -and -not $AllowFailure) {
    throw "az $($Args -join ' ') failed with exit code $LASTEXITCODE"
  }
  if ($LASTEXITCODE -ne 0) {
    return $null
  }
  return $output
}

function New-SubscriptionOidcIdentity {
  param(
    [Parameter(Mandatory = $true)][string] $DisplayName,
    [Parameter(Mandatory = $true)][string] $SubscriptionId,
    [Parameter(Mandatory = $true)][string] $FederatedCredentialJsonPath,
    [Parameter(Mandatory = $true)][string] $GitHubEnvironmentName
  )

  if (-not (Test-Path -LiteralPath $FederatedCredentialJsonPath)) {
    throw "Federated credential file not found: $FederatedCredentialJsonPath"
  }

  Write-Host ""
  Write-Host "=== $DisplayName (GitHub Environment: $GitHubEnvironmentName) ===" -ForegroundColor Cyan

  $existingApps = @(
    Invoke-Az @('ad', 'app', 'list', '--display-name', $DisplayName, '--query', '[].appId', '-o', 'tsv') |
      ForEach-Object { $_.Trim() } |
      Where-Object { $_ }
  )

  if ($existingApps.Count -gt 1) {
    throw "Multiple app registrations named '$DisplayName'. Delete extras, then re-run."
  }

  if ($existingApps.Count -eq 1) {
    $clientId = $existingApps[0]
    Write-Host "App already exists. Reusing client id: $clientId"
  }
  else {
    $clientId = (Invoke-Az @('ad', 'app', 'create', '--display-name', $DisplayName, '--query', 'appId', '-o', 'tsv')).Trim()
    Write-Host "Created app (client) id: $clientId"
  }

  $spObjectId = Invoke-Az @('ad', 'sp', 'show', '--id', $clientId, '--query', 'id', '-o', 'tsv') -AllowFailure
  if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($spObjectId)) {
    Invoke-Az @('ad', 'sp', 'create', '--id', $clientId) | Out-Null
    $spObjectId = (Invoke-Az @('ad', 'sp', 'show', '--id', $clientId, '--query', 'id', '-o', 'tsv')).Trim()
    Write-Host "Created service principal object id: $spObjectId"
  }
  else {
    $spObjectId = $spObjectId.Trim()
    Write-Host "Service principal already exists: $spObjectId"
  }

  $jsonAbs = (Resolve-Path -LiteralPath $FederatedCredentialJsonPath).Path
  $desiredName = (Get-Content -LiteralPath $jsonAbs -Raw | ConvertFrom-Json).name
  $existingCredNames = @(
    Invoke-Az @('ad', 'app', 'federated-credential', 'list', '--id', $clientId, '--query', '[].name', '-o', 'tsv') |
      ForEach-Object { $_.Trim() } |
      Where-Object { $_ }
  )

  if ($existingCredNames -contains $desiredName) {
    Write-Host "Federated credential '$desiredName' already exists. Skipping create."
  }
  else {
    Invoke-Az @('ad', 'app', 'federated-credential', 'create', '--id', $clientId, '--parameters', "@$jsonAbs") | Out-Null
    Write-Host "Federated credential applied from: $jsonAbs"
  }

  $scope = "/subscriptions/$SubscriptionId"
  foreach ($role in @('Contributor', 'User Access Administrator')) {
    $existingRole = Invoke-Az @(
      'role', 'assignment', 'list',
      '--assignee-object-id', $spObjectId,
      '--role', $role,
      '--scope', $scope,
      '--subscription', $SubscriptionId,
      '--query', '[0].id',
      '-o', 'tsv'
    )
    if (-not [string]::IsNullOrWhiteSpace($existingRole)) {
      Write-Host "Role '$role' already assigned on $scope. Skipping."
      continue
    }
    Write-Host "Assigning role '$role' on $scope"
    Invoke-Az @(
      'role', 'assignment', 'create',
      '--assignee-object-id', $spObjectId,
      '--assignee-principal-type', 'ServicePrincipal',
      '--role', $role,
      '--scope', $scope,
      '--subscription', $SubscriptionId
    ) | Out-Null
  }

  Invoke-Az @('ad', 'app', 'federated-credential', 'list', '--id', $clientId, '-o', 'table')

  [pscustomobject]@{
    GitHubEnvironment = $GitHubEnvironmentName
    DisplayName       = $DisplayName
    ClientId          = $clientId
    SpObjectId        = $spObjectId
    SubscriptionId    = $SubscriptionId
  }
}

Assert-AzCli

if ([string]::IsNullOrWhiteSpace($RepoRoot)) {
  $RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
}
else {
  $RepoRoot = (Resolve-Path -LiteralPath $RepoRoot).Path
}

$devJson = Join-Path $RepoRoot 'infra\configuration\github-azure-dev-credential.json'
$prdJson = Join-Path $RepoRoot 'infra\configuration\github-azure-prd-credential.json'

$tenantId = (Invoke-Az @('account', 'show', '--query', 'tenantId', '-o', 'tsv')).Trim()

$dev = New-SubscriptionOidcIdentity `
  -DisplayName 'dertinfo-github-subscription-development' `
  -SubscriptionId $DevSubscriptionId `
  -FederatedCredentialJsonPath $devJson `
  -GitHubEnvironmentName 'development'

$prd = New-SubscriptionOidcIdentity `
  -DisplayName 'dertinfo-github-subscription-production' `
  -SubscriptionId $PrdSubscriptionId `
  -FederatedCredentialJsonPath $prdJson `
  -GitHubEnvironmentName 'production'

Write-Host ""
Write-Host "=== GitHub Environment variables (set these next) ===" -ForegroundColor Green
Write-Host ""
Write-Host "Environment: development"
Write-Host "  AZURE_ENTRA_OIDC_CLIENTID_SUBSCRIPTION     = $($dev.ClientId)"
Write-Host "  AZURE_ENTRA_OIDC_TENANTID                  = $tenantId"
Write-Host "  AZURE_SUBSCRIPTION_DEPLOY_SUBSCRIPTIONID   = $($dev.SubscriptionId)"
Write-Host ""
Write-Host "Environment: production"
Write-Host "  AZURE_ENTRA_OIDC_CLIENTID_SUBSCRIPTION     = $($prd.ClientId)"
Write-Host "  AZURE_ENTRA_OIDC_TENANTID                  = $tenantId"
Write-Host "  AZURE_SUBSCRIPTION_DEPLOY_SUBSCRIPTIONID   = $($prd.SubscriptionId)"
Write-Host ""
Write-Host "Isolation: development SP is only on the development subscription; production SP only on production."
Write-Host "Guide: docs/technical/guides/github-azure-federated-credentials.md"
