<#
.SYNOPSIS
  Bind the API App Service system-assigned identity as a user in the Azure SQL database.

.DESCRIPTION
  Run this after API infra CD has created the App Service (system-assigned MI exists).
  Creates a contained database user named after the site (default
  app-<dev|prd>-dertinfo-api-uks) and grants db_datareader, db_datawriter,
  db_ddladmin so EF Migrate() can run.

  Azure SQL does not treat a managed identity as a member of an Entra group for
  login. Adding the MI to dertinfo-sql-db-access-<environment> does not grant
  SQL access. This script is required. The group user (New-DertInfoSqlDbAccessUser.ps1)
  is for people.

  You must already be able to connect as a SQL Entra admin (member of
  dertinfo-sql-admins-<environment>). Uses ODBC sqlcmd -G against the user
  database, not master. Restart the App Service after a successful run.

.PARAMETER GitHubEnvironment
  development or production (selects default server, database, and web app names).

.PARAMETER WebAppName
  Override the App Service name (SQL user name). Default app-<dev|prd>-dertinfo-api-uks.

.PARAMETER SqlServerFqdn
  Override the logical server FQDN. Default sql-<dev|prd>-dertinfo-storage-uks.database.windows.net

.PARAMETER DatabaseName
  Override the database name. Default sqldb-<dev|prd>-dertinfo-storage-uks

.PARAMETER UserName
  Entra login for sqlcmd -U. Default: az account show user.name (the account you use in SSMS).

.EXAMPLE
  .\New-DertInfoSqlAppServiceUser.ps1 -GitHubEnvironment development

.EXAMPLE
  .\New-DertInfoSqlAppServiceUser.ps1 -GitHubEnvironment production -UserName 'someone@contoso.com'
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory = $true)]
  [ValidateSet('development', 'production')]
  [string] $GitHubEnvironment,

  [string] $WebAppName = '',
  [string] $SqlServerFqdn = '',
  [string] $DatabaseName = '',
  [string] $UserName = ''
)

$ErrorActionPreference = 'Stop'

function Get-OdbcSqlCmdWithAzureAd {
  $wellKnown = @(
    (Join-Path ${env:ProgramFiles} 'Microsoft SQL Server\Client SDK\ODBC\180\Tools\Binn\SQLCMD.EXE'),
    (Join-Path ${env:ProgramFiles} 'Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE')
  )
  foreach ($path in $wellKnown) {
    if (Test-Path -LiteralPath $path) {
      return $path
    }
  }

  foreach ($cmd in @(Get-Command sqlcmd -All -ErrorAction SilentlyContinue)) {
    if ($cmd.CommandType -ne 'Application' -or -not $cmd.Source) {
      continue
    }
    $help = & $cmd.Source -? 2>&1 | Out-String
    if ($help -match '--authentication-method') {
      continue
    }
    if ($help -match 'Azure Active Directory') {
      return $cmd.Source
    }
  }

  throw 'ODBC sqlcmd 17 or later is required (sqlcmd -G). The ODBC 13 sqlcmd on PATH does not support Entra MFA.'
}

if ([string]::IsNullOrWhiteSpace($UserName)) {
  if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    throw 'Pass -UserName (the Entra account you use in SSMS) or install Azure CLI and run az login.'
  }
  $previousEap = $ErrorActionPreference
  $ErrorActionPreference = 'Continue'
  try {
    $UserName = (& az account show --query user.name -o tsv)
    $azExit = $LASTEXITCODE
  }
  finally {
    $ErrorActionPreference = $previousEap
  }
  if ($azExit -ne 0 -or [string]::IsNullOrWhiteSpace("$UserName")) {
    throw 'Could not read az account show user.name. Run az login or pass -UserName.'
  }
  $UserName = $UserName.Trim()
}

$envTag = if ($GitHubEnvironment -eq 'production') { 'prd' } else { 'dev' }

if ([string]::IsNullOrWhiteSpace($WebAppName)) {
  $WebAppName = "app-$envTag-dertinfo-api-uks"
}
if ([string]::IsNullOrWhiteSpace($SqlServerFqdn)) {
  $SqlServerFqdn = "sql-$envTag-dertinfo-storage-uks.database.windows.net"
}
if ([string]::IsNullOrWhiteSpace($DatabaseName)) {
  $DatabaseName = "sqldb-$envTag-dertinfo-storage-uks"
}

$sql = @"
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'$WebAppName')
BEGIN
  CREATE USER [$WebAppName] FROM EXTERNAL PROVIDER;
END
ALTER ROLE db_datareader ADD MEMBER [$WebAppName];
ALTER ROLE db_datawriter ADD MEMBER [$WebAppName];
ALTER ROLE db_ddladmin ADD MEMBER [$WebAppName];
"@

$sqlcmd = Get-OdbcSqlCmdWithAzureAd

Write-Host "Connecting to $SqlServerFqdn / $DatabaseName as $UserName"
Write-Host "Creating or updating database user [$WebAppName] (App Service system-assigned MI)"
Write-Host "Using $sqlcmd -G (ODBC Microsoft Entra MFA). A browser prompt may appear."

$argList = @(
  '-S', $SqlServerFqdn
  '-d', $DatabaseName
  '-G'
  '-U', $UserName
  '-C'
  '-Q', $sql
)
& $sqlcmd @argList
if ($LASTEXITCODE -ne 0) {
  throw "sqlcmd failed with exit code $LASTEXITCODE. Connect to the user database (not master) as a member of dertinfo-sql-admins-$GitHubEnvironment. The App Service $WebAppName must already exist with a system-assigned identity."
}

Write-Host "Database user [$WebAppName] is bound. Restart the App Service so pooled connections pick up the new principal."
Write-Host 'Do not expect SQL login via Entra group membership for this managed identity.'
