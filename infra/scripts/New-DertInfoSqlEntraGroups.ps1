<#
.SYNOPSIS
  Create or reuse the two Entra groups used for Entra-only Azure SQL (one GitHub Environment).

.DESCRIPTION
  Run this before storage CD creates the SQL server. Groups:
  - dertinfo-sql-admins-<environment>     server Entra admin (operators only)
  - dertinfo-sql-db-access-<environment>  database principal for the API (and later Functions)

  Safe to re-run. Reuses a group when the display name already exists (throws if more than one).
  Prints tenant id and both group object ids for GitHub Environment / CLI placeholders.
  Does not add members, assign Azure RBAC, or run T-SQL. After SQL exists, run New-DertInfoSqlDbAccessUser.ps1.

  See: docs/technical/standards/bicep/README.md

.PARAMETER GitHubEnvironment
  GitHub Environment name: development or production.

.EXAMPLE
  .\New-DertInfoSqlEntraGroups.ps1 -GitHubEnvironment development
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory = $true)]
  [ValidateSet('development', 'production')]
  [string] $GitHubEnvironment
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
    [Parameter(Mandatory = $true)][string[]] $AzArgs
  )
  $previousEap = $ErrorActionPreference
  $ErrorActionPreference = 'Continue'
  try {
    $output = & az @AzArgs
    $exitCode = $LASTEXITCODE
  }
  finally {
    $ErrorActionPreference = $previousEap
  }
  if ($exitCode -ne 0) {
    throw "az $($AzArgs -join ' ') failed with exit code $exitCode"
  }
  return $output
}

function Get-OrCreateAdGroup {
  param([Parameter(Mandatory = $true)][string] $DisplayName)

  $mailNickname = ($DisplayName -replace '[^A-Za-z0-9]', '').ToLowerInvariant()
  if ($mailNickname.Length -gt 64) {
    $mailNickname = $mailNickname.Substring(0, 64)
  }

  $existingJson = Invoke-Az -AzArgs @('ad', 'group', 'list', '--display-name', $DisplayName, '-o', 'json')
  $existing = @()
  if (-not [string]::IsNullOrWhiteSpace("$existingJson")) {
    $parsed = $existingJson | ConvertFrom-Json
    if ($parsed -is [System.Array]) {
      $existing = @($parsed)
    }
    elseif ($null -ne $parsed) {
      $existing = @($parsed)
    }
  }

  if ($existing.Count -gt 1) {
    throw "More than one Entra group is named '$DisplayName'. Rename or delete extras, then re-run."
  }

  if ($existing.Count -eq 1) {
    Write-Host "Reusing group $DisplayName"
    return $existing[0]
  }

  Write-Host "Creating group $DisplayName"
  $createdJson = Invoke-Az -AzArgs @(
    'ad', 'group', 'create',
    '--display-name', $DisplayName,
    '--mail-nickname', $mailNickname,
    '-o', 'json'
  )
  return ($createdJson | ConvertFrom-Json)
}

Assert-AzCli

$adminsName = "dertinfo-sql-admins-$GitHubEnvironment"
$accessName = "dertinfo-sql-db-access-$GitHubEnvironment"

$admins = Get-OrCreateAdGroup -DisplayName $adminsName
$access = Get-OrCreateAdGroup -DisplayName $accessName

$tenantId = (Invoke-Az -AzArgs @('account', 'show', '--query', 'tenantId', '-o', 'tsv'))
if ([string]::IsNullOrWhiteSpace("$tenantId")) {
  throw 'Could not read tenant id from az account show.'
}

Write-Host ''
Write-Host "Set these on GitHub Environment '$GitHubEnvironment' (or pass as az --parameters):"
Write-Host "AZURE_ENTRA_OIDC_TENANTID=$tenantId"
Write-Host "AZURE_ENTRA_SQL_ADMIN_GROUP_NAME=$adminsName"
Write-Host "AZURE_ENTRA_SQL_ADMIN_GROUP_OBJECTID=$($admins.id)"
Write-Host "AZURE_ENTRA_SQL_DBACCESS_GROUP_NAME=$accessName"
Write-Host "AZURE_ENTRA_SQL_DBACCESS_GROUP_OBJECTID=$($access.id)"
Write-Host ''
Write-Host 'Do not commit these object ids. Flip storage prerequisitesExist after the admin group id is set, then run storage infra CD.'
