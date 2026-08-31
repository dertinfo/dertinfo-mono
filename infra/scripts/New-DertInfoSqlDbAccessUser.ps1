<#
.SYNOPSIS
  Bind the database access Entra group as a user in the Azure SQL database.

.DESCRIPTION
  Run this after storage CD has created the SQL server. Creates the
  dertinfo-sql-db-access-<environment> group as a database user and grants
  db_datareader, db_datawriter, db_ddladmin so the hosted API can run EF Migrate().

  You must already be able to connect as a SQL Entra admin (member of
  dertinfo-sql-admins-<environment>). Uses ODBC sqlcmd -G (SSMS Microsoft Entra MFA)
  against the user database, not master. Does not add Entra group members.
  The GitHub storage pipeline cannot do this.

.PARAMETER GitHubEnvironment
  development or production (selects default server/database names and group display name).

.PARAMETER SqlServerFqdn
  Override the logical server FQDN. Default sql-<dev|prd>-dertinfo-storage-uks.database.windows.net

.PARAMETER DatabaseName
  Override the database name. Default sqldb-<dev|prd>-dertinfo-storage-uks

.PARAMETER UserName
  Entra login for sqlcmd -U. Default: az account show user.name (the account you use in SSMS).

.EXAMPLE
  .\New-DertInfoSqlDbAccessUser.ps1 -GitHubEnvironment development

.EXAMPLE
  .\New-DertInfoSqlDbAccessUser.ps1 -GitHubEnvironment development -UserName 'someone@contoso.com'
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory = $true)]
  [ValidateSet('development', 'production')]
  [string] $GitHubEnvironment,

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
$groupName = "dertinfo-sql-db-access-$GitHubEnvironment"

if ([string]::IsNullOrWhiteSpace($SqlServerFqdn)) {
  $SqlServerFqdn = "sql-$envTag-dertinfo-storage-uks.database.windows.net"
}
if ([string]::IsNullOrWhiteSpace($DatabaseName)) {
  $DatabaseName = "sqldb-$envTag-dertinfo-storage-uks"
}

$sql = @"
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'$groupName')
BEGIN
  CREATE USER [$groupName] FROM EXTERNAL PROVIDER;
END
ALTER ROLE db_datareader ADD MEMBER [$groupName];
ALTER ROLE db_datawriter ADD MEMBER [$groupName];
ALTER ROLE db_ddladmin ADD MEMBER [$groupName];
"@

$sqlcmd = Get-OdbcSqlCmdWithAzureAd

Write-Host "Connecting to $SqlServerFqdn / $DatabaseName as $UserName"
Write-Host "Creating or updating database user [$groupName]"
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
  throw "sqlcmd failed with exit code $LASTEXITCODE. Connect to the user database (not master) as a member of dertinfo-sql-admins-$GitHubEnvironment."
}

Write-Host 'Database access group is bound. Add operators and the App Service MI to the Entra groups when you need access.'
