<#
.SYNOPSIS
  Register the Azure resource providers required by new-stack workload Bicep.

.DESCRIPTION
  Runs az provider register for each namespace. Idempotent. Does not wait
  for every region to finish; ARM allows creates while a provider is Registering.

  Subscription CD registers the same list before az deployment sub create.
  Use this script for a local / break-glass subscription deploy.

  ARM has no PUT for provider registration (POST .../register only), so this
  cannot live as a Bicep resource. Workload SPs have RG Contributor only and
  cannot register providers themselves.

.EXAMPLE
  .\Register-DertInfoResourceProviders.ps1
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'

function Assert-AzCli {
  if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    throw 'Azure CLI (az) is required.'
  }
}

Assert-AzCli

# Keep in sync with reusable-infra-deploy-bicep-subscription.yml
$namespaces = @(
  'Microsoft.AppConfiguration'
  'Microsoft.Authorization'
  'Microsoft.Insights'
  'Microsoft.KeyVault'
  'Microsoft.ManagedIdentity'
  'Microsoft.OperationalInsights'
  'Microsoft.Security'
  'Microsoft.Sql'
  'Microsoft.Storage'
  'Microsoft.Web'
)

foreach ($namespace in $namespaces) {
  Write-Host "Registering $namespace"
  az provider register --namespace $namespace
  if ($LASTEXITCODE -ne 0) {
    throw "az provider register failed for $namespace"
  }
}

Write-Host "Registered $($namespaces.Count) resource provider namespace(s)."
