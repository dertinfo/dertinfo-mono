<#
.SYNOPSIS
  Bind the database access Entra group as a user in the Azure SQL database.

.DESCRIPTION
  Run this after storage CD has created the SQL server. Creates the
  dertinfo-sql-db-access-<environment> group as a database user and grants
  db_datareader, db_datawriter, db_ddladmin so the hosted API can run EF Migrate().

  You must already be able to connect as a SQL Entra admin (member of
  dertinfo-sql-admins-<environment>). Uses sqlcmd -G. Does not add Entra group members.
  The GitHub storage pipeline cannot do this.

.PARAMETER GitHubEnvironment
  development or production (selects default server/database names and group display name).

.PARAMETER SqlServerFqdn
  Override the logical server FQDN. Default sql-<dev|prd>-dertinfo-storage-uks.database.windows.net

.PARAMETER DatabaseName
  Override the database name. Default sqldb-<dev|prd>-dertinfo-storage-uks

.EXAMPLE
  .\New-DertInfoSqlDbAccessUser.ps1 -GitHubEnvironment development
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory = $true)]
  [ValidateSet('development', 'production')]
  [string] $GitHubEnvironment,

  [string] $SqlServerFqdn = '',
  [string] $DatabaseName = ''
)

$ErrorActionPreference = 'Stop'

if (-not (Get-Command sqlcmd -ErrorAction SilentlyContinue)) {
  throw 'sqlcmd is required (SQL command-line tools) and you must be signed in as an Entra SQL admin.'
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

Write-Host "Connecting to $SqlServerFqdn / $DatabaseName as Entra user"
Write-Host "Creating or updating database user [$groupName]"

sqlcmd -S $SqlServerFqdn -d $DatabaseName -G -C -Q $sql
if ($LASTEXITCODE -ne 0) {
  throw "sqlcmd failed with exit code $LASTEXITCODE"
}

Write-Host 'Database access group is bound. Add operators and the App Service MI to the Entra groups when you need access.'
