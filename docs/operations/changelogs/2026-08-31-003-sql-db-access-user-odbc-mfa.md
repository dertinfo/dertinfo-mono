# Azure SQL database access user script (ODBC Entra MFA)

## Summary of the work completed

[`New-DertInfoSqlDbAccessUser.ps1`](../../../infra/scripts/New-DertInfoSqlDbAccessUser.ps1) now binds `dertinfo-sql-db-access-<environment>` in the **user** database using ODBC `sqlcmd -G` (Microsoft Entra MFA, same class of login as SSMS). Development was run successfully against `sqldb-dev-dertinfo-storage-uks`. The script prefers ODBC 17/18 binaries so the ODBC 13 `sqlcmd` on PATH is not used. `-UserName` defaults from `az account show`. Docs: [`infra/scripts/README.md`](../../../infra/scripts/README.md), [`docs/technical/infra/secrets-and-rotation.md`](../../technical/infra/secrets-and-rotation.md).

## Why the work was completed

The operator script has to work for anyone setting up an environment, not as a one-off SSMS batch. Silent go-sqlcmd (`ActiveDirectoryDefault` / `ActiveDirectoryAzCli`) and go-sqlcmd interactive did not match SSMS Entra MFA on this tenant. ODBC `sqlcmd -G` against the app database is the path that succeeded and is what the script now documents.

## Date the work was started

2026-08-31

## Date the work was completed

2026-08-31

## Issues that were encountered on the way

- PATH `sqlcmd` was ODBC 13 (`-G` unknown). ODBC 170 already had `-G`; the script now selects 17/18 explicitly.
- go-sqlcmd `ActiveDirectoryDefault` and `ActiveDirectoryAzCli` authenticated but Azure SQL returned `Login failed for user '<token-identified principal>'`.
- go-sqlcmd `ActiveDirectoryInteractive` opened a browser against tenant **Microsoft Services** (first-party app `a94f9c62-97fe-4d19-b06d-472bed8d2bcf`), not the operator directory. That client does not apply the SQL server tenant the way SSMS / ODBC `-G` does.
- Connecting to `master` fails for this Entra admin path; `CREATE USER ... FROM EXTERNAL PROVIDER` must run in `sqldb-<dev|prd>-dertinfo-storage-uks`.

## References to any best practices that we found

- [Microsoft Entra authentication for Azure SQL](https://learn.microsoft.com/en-us/azure/azure-sql/database/authentication-aad-overview)
- [sqlcmd `-G` (Azure Active Directory)](https://learn.microsoft.com/en-us/sql/tools/sqlcmd/sqlcmd-utility)
- Earlier Entra-only SQL groups work: [2026-08-30-003-entra-only-sql-groups.md](./2026-08-30-003-entra-only-sql-groups.md)

## Any remaining issues that we may wish to address

- Production still needs the same script (`-GitHubEnvironment production`) after that SQL server exists.
- Operators need ODBC sqlcmd 17+ and membership of `dertinfo-sql-admins-<environment>` (group **member**, not only owner), plus a SQL firewall rule for their client IP.
- The hosted API still uses `Authentication=Active Directory Default` (managed identity). That is separate from this operator MFA script.
