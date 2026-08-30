<#
.SYNOPSIS
  Remove one Entra app registration used for a workload GitHub Actions OIDC identity, including federated credentials and its service principal.

.DESCRIPTION
  Given the app (client) id:
  - Deletes every federated identity credential on the app
  - Deletes Azure role assignments for the service principal (if present), including RG-scoped assignments
  - Deletes the service principal
  - Deletes the app registration

  Missing items are skipped so the script is safe to re-run. Does not change GitHub Environment variables.
  Prints display name, federated credential names, and each role/scope, then prompts unless -Force.

  See: docs/technical/guides/github-azure-federated-credentials.md

.PARAMETER ClientId
  App registration application (client) id.

.PARAMETER Force
  Do not prompt for confirmation.

.EXAMPLE
  .\Remove-DertInfoWorkloadOidcIdentity.ps1 -ClientId '11111111-2222-3333-4444-555555555555'

.EXAMPLE
  .\Remove-DertInfoWorkloadOidcIdentity.ps1 -ClientId '11111111-2222-3333-4444-555555555555' -Force
#>
[CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'High')]
param(
  [Parameter(Mandatory = $true)]
  [ValidatePattern('^[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}$')]
  [string] $ClientId,

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

function Invoke-Az {
  param(
    [Parameter(Mandatory = $true)][string[]] $Args,
    [switch] $AllowFailure
  )
  $previousEap = $ErrorActionPreference
  $ErrorActionPreference = 'Continue'
  try {
    if ($AllowFailure) {
      $output = & az @Args 2>$null
    }
    else {
      $output = & az @Args
    }
    $exitCode = $LASTEXITCODE
  }
  finally {
    $ErrorActionPreference = $previousEap
  }
  if ($exitCode -ne 0 -and -not $AllowFailure) {
    throw "az $($Args -join ' ') failed with exit code $exitCode"
  }
  if ($exitCode -ne 0) {
    return $null
  }
  return $output
}

function ConvertFrom-AzJson {
  param([AllowNull()][object] $Raw)
  if ($null -eq $Raw -or [string]::IsNullOrWhiteSpace("$Raw")) {
    return @()
  }
  $text = if ($Raw -is [array]) { $Raw -join "`n" } else { [string]$Raw }
  $parsed = $text | ConvertFrom-Json
  return @($parsed)
}

function Get-RoleAssignmentsForSp {
  param([Parameter(Mandatory = $true)][string] $SpObjectId)

  $subIds = @(
    Invoke-Az @('account', 'list', '--query', '[].id', '-o', 'tsv') |
      ForEach-Object { $_.Trim() } |
      Where-Object { $_ }
  )
  $assignments = @()
  foreach ($subId in $subIds) {
    $found = ConvertFrom-AzJson (Invoke-Az @(
      'role', 'assignment', 'list',
      '--assignee', $SpObjectId,
      '--subscription', $subId,
      '-o', 'json'
    ) -AllowFailure)
    $assignments += $found
  }
  return @($assignments)
}

Assert-AzCli

$displayName = Invoke-Az @('ad', 'app', 'show', '--id', $ClientId, '--query', 'displayName', '-o', 'tsv') -AllowFailure
$appExists = -not [string]::IsNullOrWhiteSpace($displayName)
if ($appExists) {
  $displayName = $displayName.Trim()
}

$spObjectId = Invoke-Az @('ad', 'sp', 'show', '--id', $ClientId, '--query', 'id', '-o', 'tsv') -AllowFailure
$spExists = -not [string]::IsNullOrWhiteSpace($spObjectId)
if ($spExists) {
  $spObjectId = $spObjectId.Trim()
}

if (-not $appExists -and -not $spExists) {
  Write-Host "No app registration or service principal found for client id $ClientId. Nothing to do."
  return
}

$creds = @()
if ($appExists) {
  $creds = ConvertFrom-AzJson (Invoke-Az @('ad', 'app', 'federated-credential', 'list', '--id', $ClientId, '-o', 'json') -AllowFailure)
}

$assignments = @()
if ($spExists) {
  $assignments = Get-RoleAssignmentsForSp -SpObjectId $spObjectId
}

Write-Host ""
Write-Host "=== Planned removal ===" -ForegroundColor Cyan
Write-Host "Client id:                    $ClientId"
Write-Host "Display name:                 $(if ($appExists) { $displayName } else { '(app registration not found)' })"
Write-Host "Service principal object id:  $(if ($spExists) { $spObjectId } else { '(service principal not found)' })"
Write-Host ""
Write-Host "Federated credentials:"
if ($creds.Count -eq 0) {
  Write-Host "  (none)"
}
else {
  foreach ($cred in $creds) {
    Write-Host "  - $($cred.name)  ($($cred.id))"
  }
}
Write-Host ""
Write-Host "Role assignments:"
if ($assignments.Count -eq 0) {
  Write-Host "  (none)"
}
else {
  foreach ($assignment in $assignments) {
    Write-Host "  - $($assignment.roleDefinitionName)  $($assignment.scope)"
  }
}
Write-Host ""
Write-Host "Will also delete the service principal (if present) and the app registration (if present)."
Write-Host ""

$target = if ($appExists) { "$displayName ($ClientId)" } else { "service principal for $ClientId" }
$action = 'Remove the federated credentials, role assignments, service principal, and app registration listed above'
if (-not $Force -and -not $PSCmdlet.ShouldProcess($target, $action)) {
  return
}

Write-Host "=== Removing workload OIDC identity $ClientId ===" -ForegroundColor Cyan

if ($appExists) {
  if ($creds.Count -eq 0) {
    Write-Host "No federated credentials. Skipping."
  }
  else {
    foreach ($cred in $creds) {
      Write-Host "Deleting federated credential '$($cred.name)' ($($cred.id))"
      Invoke-Az @('ad', 'app', 'federated-credential', 'delete', '--id', $ClientId, '--federated-credential-id', [string]$cred.id) | Out-Null
    }
  }
}
else {
  Write-Host "App registration not found. Skipping federated credentials."
}

if ($spExists) {
  if ($assignments.Count -eq 0) {
    Write-Host "No role assignments. Skipping."
  }
  else {
    foreach ($assignment in $assignments) {
      Write-Host "Deleting role assignment $($assignment.roleDefinitionName) on $($assignment.scope)"
      Invoke-Az @('role', 'assignment', 'delete', '--ids', [string]$assignment.id) | Out-Null
    }
  }

  Write-Host "Deleting service principal $spObjectId"
  Invoke-Az @('ad', 'sp', 'delete', '--id', $ClientId) | Out-Null
}
else {
  Write-Host "Service principal not found. Skipping role assignments and SP delete."
}

if ($appExists) {
  Write-Host "Deleting app registration $ClientId"
  Invoke-Az @('ad', 'app', 'delete', '--id', $ClientId) | Out-Null
}
else {
  Write-Host "App registration not found. Skipping app delete."
}

Write-Host ""
Write-Host "Done. Remove or update GitHub Environment variables that still reference this client id." -ForegroundColor Green
Write-Host "Guide: docs/technical/guides/github-azure-federated-credentials.md"
