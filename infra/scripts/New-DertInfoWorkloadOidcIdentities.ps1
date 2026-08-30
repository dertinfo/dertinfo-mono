<#
.SYNOPSIS
  Create workload GitHub Actions OIDC identities for every DertInfo part on one Environment / subscription.

.DESCRIPTION
  Holds the workload catalog (config, monitoring, storage, api, web, app, functions) and calls
  New-DertInfoWorkloadOidcIdentity.ps1 once per part. Run separately for development and production.

  Prints a copy-paste block of GitHub Environment variable names and values for that Environment.

  Does not assign Azure RBAC. After you paste the variables, re-run subscription infra CD.

  See: docs/technical/guides/github-azure-federated-credentials.md

.PARAMETER GitHubEnvironment
  GitHub Environment name: development or production.

.PARAMETER SubscriptionId
  Azure subscription id for that Environment.

.PARAMETER RepoRoot
  Monorepo root. Defaults to two levels above this script.

.EXAMPLE
  .\New-DertInfoWorkloadOidcIdentities.ps1 `
    -GitHubEnvironment development `
    -SubscriptionId '00000000-0000-0000-0000-000000000000'
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory = $true)]
  [ValidateSet('development', 'production')]
  [string] $GitHubEnvironment,

  [Parameter(Mandatory = $true)]
  [ValidatePattern('^[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}$')]
  [string] $SubscriptionId,

  [Parameter(Mandatory = $false)]
  [string] $RepoRoot = ''
)

$ErrorActionPreference = 'Stop'

# Catalog of solution workloads. Display names are dertinfo-github-workload-<part>-<environment>.
$script:WorkloadParts = @(
  'config'
  'monitoring'
  'storage'
  'api'
  'web'
  'app'
  'functions'
)

if ([string]::IsNullOrWhiteSpace($RepoRoot)) {
  $RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
}
else {
  $RepoRoot = (Resolve-Path -LiteralPath $RepoRoot).Path
}

$newOne = Join-Path $PSScriptRoot 'New-DertInfoWorkloadOidcIdentity.ps1'
if (-not (Test-Path -LiteralPath $newOne)) {
  throw "Missing $newOne"
}

$results = @()
foreach ($part in $script:WorkloadParts) {
  $identity = & $newOne `
    -GitHubEnvironment $GitHubEnvironment `
    -SubscriptionId $SubscriptionId `
    -Workload $part `
    -RepoRoot $RepoRoot
  $results += $identity
}

$tenantId = (& az account show --query tenantId -o tsv)
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($tenantId)) {
  throw 'Could not read tenant id from az account show.'
}
$tenantId = $tenantId.Trim()

Write-Host ""
Write-Host "=== GitHub Environment variables (copy and paste) ===" -ForegroundColor Green
Write-Host ""
Write-Host "Environment: $GitHubEnvironment"
Write-Host "  AZURE_ENTRA_OIDC_TENANTID                    = $tenantId"
Write-Host "  AZURE_SUBSCRIPTION_DEPLOY_SUBSCRIPTIONID     = $SubscriptionId"
foreach ($item in $results) {
  Write-Host "  $($item.ClientIdVar)   = $($item.ClientId)"
  Write-Host "  $($item.PrincipalIdVar) = $($item.SpObjectId)"
}
Write-Host ""
Write-Host "App registrations: dertinfo-github-workload-<part>-$GitHubEnvironment"
Write-Host "Next: paste the variables on the $GitHubEnvironment GitHub Environment, then re-run subscription infra CD."
Write-Host "Guide: docs/technical/guides/github-azure-federated-credentials.md"
