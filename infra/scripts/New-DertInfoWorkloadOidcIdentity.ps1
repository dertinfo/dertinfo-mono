<#
.SYNOPSIS
  Create or reuse one Entra app + service principal for a single workload GitHub Actions OIDC identity (one Environment / subscription).

.DESCRIPTION
  Creates (or reuses) an app registration named dertinfo-github-workload-<part>-development
  or dertinfo-github-workload-<part>-production, creates the service principal if missing,
  and applies the matching federated credential JSON from infra/configuration/.

  Does not assign Azure RBAC. Subscription foundation Bicep grants Contributor (and any
  extra RG-scoped roles) after you paste AZURE_ENTRA_OIDC_PRINCIPALID_WORKLOAD_* into the
  GitHub Environment.

  Safe to re-run. Throws if multiple apps share the same display name.

  See: docs/technical/guides/github-azure-federated-credentials.md

.PARAMETER GitHubEnvironment
  GitHub Environment name: development or production.

.PARAMETER SubscriptionId
  Azure subscription id for that Environment (recorded on the result; not used for RBAC).

.PARAMETER Workload
  Workload part token: config, monitoring, storage, api, web, app, or functions.

.PARAMETER RepoRoot
  Monorepo root containing infra/configuration. Defaults to two levels above this script.

.EXAMPLE
  .\New-DertInfoWorkloadOidcIdentity.ps1 `
    -GitHubEnvironment development `
    -SubscriptionId '00000000-0000-0000-0000-000000000000' `
    -Workload api
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory = $true)]
  [ValidateSet('development', 'production')]
  [string] $GitHubEnvironment,

  [Parameter(Mandatory = $true)]
  [ValidatePattern('^[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}$')]
  [string] $SubscriptionId,

  [Parameter(Mandatory = $true)]
  [ValidateSet('config', 'monitoring', 'storage', 'api', 'web', 'app', 'functions')]
  [string] $Workload,

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

Assert-AzCli

if ([string]::IsNullOrWhiteSpace($RepoRoot)) {
  $RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
}
else {
  $RepoRoot = (Resolve-Path -LiteralPath $RepoRoot).Path
}

$jsonName = if ($GitHubEnvironment -eq 'production') { 'github-azure-prd-credential.json' } else { 'github-azure-dev-credential.json' }
$federatedJsonPath = Join-Path $RepoRoot "infra\configuration\$jsonName"
if (-not (Test-Path -LiteralPath $federatedJsonPath)) {
  throw "Federated credential file not found: $federatedJsonPath"
}

$displayName = "dertinfo-github-workload-$Workload-$GitHubEnvironment"
$suffix = $Workload.ToUpperInvariant()
$clientVar = "AZURE_ENTRA_OIDC_CLIENTID_WORKLOAD_$suffix"
$principalVar = "AZURE_ENTRA_OIDC_PRINCIPALID_WORKLOAD_$suffix"

Write-Host ""
Write-Host "=== $displayName (GitHub Environment: $GitHubEnvironment) ===" -ForegroundColor Cyan

$existingApps = @(
  Invoke-Az @('ad', 'app', 'list', '--display-name', $displayName, '--query', '[].appId', '-o', 'tsv') |
    ForEach-Object { $_.Trim() } |
    Where-Object { $_ }
)

if ($existingApps.Count -gt 1) {
  throw "Multiple app registrations named '$displayName'. Delete extras, then re-run."
}

if ($existingApps.Count -eq 1) {
  $clientId = $existingApps[0]
  Write-Host "App already exists. Reusing client id: $clientId"
}
else {
  $clientId = (Invoke-Az @('ad', 'app', 'create', '--display-name', $displayName, '--query', 'appId', '-o', 'tsv')).Trim()
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

$jsonAbs = (Resolve-Path -LiteralPath $federatedJsonPath).Path
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

Write-Host "No Azure RBAC assigned (workload identities are granted RG roles by subscription Bicep)."

[pscustomobject]@{
  GitHubEnvironment = $GitHubEnvironment
  Workload          = $Workload
  DisplayName       = $displayName
  ClientId          = $clientId
  SpObjectId        = $spObjectId
  SubscriptionId    = $SubscriptionId
  ClientIdVar       = $clientVar
  PrincipalIdVar    = $principalVar
}
