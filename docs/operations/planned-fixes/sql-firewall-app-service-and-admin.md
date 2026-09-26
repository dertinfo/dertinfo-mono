# Planned: Azure SQL firewall — App Service IPs and administrator IP

**Status:** Not started — Entra-only is already in place; SQL still allows all Azure public IPs.

**Related:** [`infra/bicep/storage/sql.bicep`](../../../infra/bicep/storage/sql.bicep), [Secrets and rotation — hosted Azure SQL](../../technical/infra/secrets-and-rotation.md#hosted-azure-sql-entra-only), operator copy script [`Copy-DertInfoSqlToDevelopment.ps1`](../../../infra/scripts/Database/Copy-DertInfoSqlToDevelopment.ps1), [Hosting and cost decisions](hosting-cost-decisions.md).

---

## Intent

Stop the SQL public endpoint from accepting TDS from **any Azure resource in any subscription**. Keep Entra-only. Allow only:

1. That environment’s API App Service outbound IP pool (`possibleOutboundIpAddresses` on `app-<dev|prd>-dertinfo-api-uks`).
2. The administrator’s client IP (SSMS, `sqlcmd -G`, the copy script’s user-bind step).

Remove `AllowAllWindowsAzureIps` (`0.0.0.0`–`0.0.0.0`). Keep `publicNetworkAccess: Enabled`.

## Why

The logical server FQDN is already in public Bicep (`sql-<dev|prd>-dertinfo-storage-uks.database.windows.net`). That name is not treated as a secret. The hole is **Allow Azure services**: Microsoft documents that it allows connections from all Azure services, including other customers’ subscriptions. Entra-only still requires a valid principal to log in; it does not block a connection attempt.

DEV App Service is **F1** and PRD is **D1**. Those plans cannot use VNet integration, NAT Gateway, or private endpoints. The App Service therefore has no unique public IP — only a **shared stamp outbound pool**. Locking the firewall to that pool plus the admin IP is the tightest network control on these SKUs.

## Decisions (accepted bar)

| Decision | Detail |
|----------|--------|
| Entra-only + this firewall is **secure enough** | No SQL passwords; network path limited to App Service stamp IPs and the administrator IP |
| Private endpoints are **not** required for this bar | F1/D1 cannot reach a private SQL endpoint; PE would need App Service B1+ and VNet integration |
| App Service IPs are a shared pool | Other apps on the same scale unit share those outbound addresses; accepted together with Entra-only |
| Administrator IP is a standing rule | Not a temporary add/remove in the copy script. User bind and SSMS keep working |
| ARM copy stays valid | `az sql db copy` / ARM `createMode: Copy` is control plane and does not use the SQL data-plane firewall |

## Scope (when scheduled)

1. Read `possibleOutboundIpAddresses` for `app-dev-dertinfo-api-uks` and (when it exists) `app-prd-dertinfo-api-uks`.
2. In storage Bicep, replace `AllowAllWindowsAzureIps` with per-IP firewall rules for that pool, plus a parameter (not committed) for the administrator client IP — or an operator script that sets the admin rule without putting the IP in git.
3. Re-run storage infra CD for development, then production when that SQL server exists.
4. Confirm the hosted API can still `Migrate()` (App Service IPs allowed) and that `Copy-DertInfoSqlToDevelopment.ps1` user bind still works from the administrator IP.
5. Re-apply App Service IP rules after a stamp move or SKU change (outbound IPs can change).

## Out of scope

- Deploying private endpoints or disabling public network access.
- Changing App Service SKU to B1 for VNet integration.
- Implementing this firewall change in the copy-script workstream.
