---
name: Production environment setup
type: guide
status: active
updated: 2026-09-26
---

# Set up the production environment

Bring new-stack production up on Azure hostnames, then change DNS. Do one milestone at a time. Start the next milestone only after **Done when** is true. If the check fails, fix that milestone and run the check again. Live DNS stays as it is until milestone 9.

Workflow names: [CI/CD](../infra/cicd.md). Scripts: [`infra/scripts/README.md`](../../../infra/scripts/README.md). Auth0 URL fields: [Authentication](../subsystems/authentication.md#hosted-development-auth0-application-urls).

## Before you start

- Development already deploys. GitHub Environment `production` exists, has required reviewers, and already has `AZURE_ENTRA_OIDC_CLIENTID_SUBSCRIPTION`, `AZURE_ENTRA_OIDC_TENANTID`, and `AZURE_SUBSCRIPTION_DEPLOY_SUBSCRIPTIONID`.
- You will use the live Auth0 tenant `dertinfo.eu.auth0.com` and audience `api.dertinfo.co.uk`. You have the live management client id and secret, both SPA client ids, and the live Mailgun and SendGrid keys. Do not point production at `dertinfotest`.
- `az login` can see the production subscription and the live SQL subscription. Commands run in PowerShell from `infra\scripts` after `az account set --subscription <production-subscription-id>`. ODBC Driver 17 or newer provides `sqlcmd -G`. AzCopy v10 is on PATH for milestone 8 (`azcopy login` once before that milestone).
- Repository secret `DOCKERHUB_TOKEN` and variable `DOCKERHUB_USERNAME` are set.

## How to run a pipeline

GitHub Actions → the named workflow → Run workflow → branch `main` → target **`full`**. Approve `development`, then `production`. Production does not start until that development job succeeds. Merge a `main.prod.bicepparam` change to `main` before the dispatch that needs it. Leave `customDomainReady` false until milestone 9. Leave `flagImagesEventGridReady` false until milestone 8.

## Write these down

| When | Write down | Used in |
|------|------------|---------|
| Milestone 1 | Workload client id and principal id for config, monitoring, storage, api, web, app, functions | Milestone 1 paste, then Subscription infra CD |
| Milestone 1 | SQL admin group name and object id | Milestone 1 paste, then milestone 3 |
| Milestone 2 | Application Insights instrumentation key | Milestone 5 |
| Milestone 2 | Images account key for `stprddertinfoimagesuks` | Milestone 5 |
| Milestone 4 | Web and app `defaultHostname`, and both deployment tokens | Milestones 5, 7, and 9 |

Fixed names: API `app-prd-dertinfo-api-uks`, Functions `func-prd-dertinfo-functions-uks`, SQL `sql-prd-dertinfo-storage-uks.database.windows.net` / `sqldb-prd-dertinfo-storage-uks`, images `stprddertinfoimagesuks`, website `swa-prd-dertinfo-web-uks`, PWA `swa-prd-dertinfo-app-uks`. Public names in milestone 9 are `www.dertinfo.co.uk` and `app.dertinfo.co.uk`.

## Milestone 0 — Scripts are in the repo

1. Confirm [`Copy-DertInfoSqlToProduction.ps1`](../../../infra/scripts/Database/Copy-DertInfoSqlToProduction.ps1) and [`Copy-DertInfoImagesToProduction.ps1`](../../../infra/scripts/Storage/Copy-DertInfoImagesToProduction.ps1) are on the `main` you will deploy, because later milestones run those scripts instead of one-off commands.

**Done when:** `.\Start-DertInfoControlPlane.ps1` lists `Database > Copy-DertInfoSqlToProduction.ps1` and `Storage > Copy-DertInfoImagesToProduction.ps1`, and each synopsis names the production destination.

## Milestone 1 — GitHub can deploy production

1. **Machine.** Run `.\Entra\New-DertInfoWorkloadOidcIdentities.ps1 -GitHubEnvironment production -SubscriptionId '<production-subscription-id>'` and record each client id and principal id, because Subscription infra CD grants resource-group roles from those ids.
2. **GitHub.** Paste every printed `AZURE_ENTRA_OIDC_*` line onto Environment `production`, and set variable `AZURE_MONITOR_FUNCTIONS_EXCESSIVEUSE_EMAIL`, because Functions infra CD fails when that address is empty.
3. **Machine.** Run `.\Entra\New-DertInfoSqlEntraGroups.ps1 -GitHubEnvironment production` and record the admin group name and object id, because the SQL server admin must exist before storage creates the server.
4. **GitHub.** Paste `AZURE_ENTRA_SQL_ADMIN_GROUP_NAME` and `AZURE_ENTRA_SQL_ADMIN_GROUP_OBJECTID` onto Environment `production`.
5. **Machine.** Add yourself to `dertinfo-sql-admins-production`, because the database scripts sign in as a member of that group.

```powershell
$me = az ad signed-in-user show --query id -o tsv
az ad group member add --group dertinfo-sql-admins-production --member-id $me
```

**Done when:** Environment `production` shows the workload client ids and principal ids, the excess-use email, and the two SQL admin variables, and you are a member of `dertinfo-sql-admins-production`.

## Milestone 2 — Config, monitoring, and the images account exist

1. **Azure.** Confirm App Service **D1** quota in UK South on the production subscription (App Service quotas, not Compute), because API infra CD creates a D1 plan and a missing quota fails that deploy.
2. **Machine.** Confirm Flex Consumption includes `uksouth` (`az functionapp list-flexconsumption-locations`), because Functions infra CD creates a Flex app in that region.
3. **GitHub.** Dispatch **Subscription infra CD**, because this creates the production resource groups and workload RBAC.
4. **GitHub.** Dispatch **Config infra CD** and **Monitoring infra CD**, because the API needs the vault, App Configuration, and Application Insights.
5. **Machine.** Grant yourself Key Vault Secrets Officer and App Configuration Data Owner on `rg-prd-dertinfo-config-uks`, because the secret and import scripts use your `az login`.

```powershell
$me = az ad signed-in-user show --query id -o tsv
$scope = az group show --name rg-prd-dertinfo-config-uks --query id -o tsv
az role assignment create --assignee-object-id $me --assignee-principal-type User --role "Key Vault Secrets Officer" --scope $scope
az role assignment create --assignee-object-id $me --assignee-principal-type User --role "App Configuration Data Owner" --scope $scope
```

6. **Machine.** Record the instrumentation key, because Key Vault must hold it before the API starts.

```powershell
az monitor app-insights component show --resource-group rg-prd-dertinfo-monitoring-uks --app appi-prd-dertinfo-monitoring-uks --query instrumentationKey -o tsv
```

7. **GitHub.** Dispatch **Storage infra CD** while `flagSqlServerIsReady`, `flagImagesFunctionAppReady`, and `flagImagesEventGridReady` in [`infra/bicep/storage/main.prod.bicepparam`](../../../infra/bicep/storage/main.prod.bicepparam) are still `false`, because this run creates only the images account.
8. **Machine.** Record the images account key, because the API still authenticates to that account with a key.

```powershell
az storage account keys list --resource-group rg-prd-dertinfo-storage-uks --account-name stprddertinfoimagesuks --query "[0].value" -o tsv
```

**Done when:** `kv-prd-dertinfo-uks`, `appcs-prd-dertinfo-config-uks`, `appi-prd-dertinfo-monitoring-uks`, and `stprddertinfoimagesuks` exist, and both keys are written down.

## Milestone 3 — Production SQL server exists

1. **GitHub.** Merge `flagSqlServerIsReady = true` in [`infra/bicep/storage/main.prod.bicepparam`](../../../infra/bicep/storage/main.prod.bicepparam) and dispatch **Storage infra CD**, because that flag creates Entra-only SQL.
2. **Machine.** Add your client IP as a firewall rule on `sql-prd-dertinfo-storage-uks`, because the bind scripts connect from this machine. Leave `AllowAllWindowsAzureIps` in place, because the D1 plan has no stable outbound IP.

```powershell
$ip = (Invoke-RestMethod https://api.ipify.org)
az sql server firewall-rule create --resource-group rg-prd-dertinfo-storage-uks --server sql-prd-dertinfo-storage-uks --name operator-client --start-ip-address $ip --end-ip-address $ip
```

**Done when:** `sql-prd-dertinfo-storage-uks.database.windows.net` exists and `sqldb-prd-dertinfo-storage-uks` exists. The database is still empty.

## Milestone 4 — API, Functions, and both Static Web Apps exist

1. **GitHub.** Merge `prerequisitesExist = true` in [`infra/bicep/api/main.prod.bicepparam`](../../../infra/bicep/api/main.prod.bicepparam) and dispatch **API infra CD**, because the plan and site are skipped until the vault, App Configuration, and Application Insights exist.
2. **GitHub.** Merge `prerequisitesExist = true` in [`infra/bicep/functions/main.prod.bicepparam`](../../../infra/bicep/functions/main.prod.bicepparam) and dispatch **Functions infra CD**, because the Function App is skipped until the images account and Application Insights exist.
3. **GitHub.** Set `AZURE_FUNCTIONAPP_FUNCTIONS_RESOURCENAME` to `func-prd-dertinfo-functions-uks`, merge `flagImagesFunctionAppReady = true` in the storage production leaf, and dispatch **Storage infra CD**, because that flag assigns Blob Data Contributor to the Function App identity.
4. **GitHub.** Dispatch **Functions Src CD**, because the host must exist before images are copied.
5. **GitHub.** Merge `prerequisitesExist = true` and leave `customDomainReady = false` in [`infra/bicep/web/main.prod.bicepparam`](../../../infra/bicep/web/main.prod.bicepparam) and [`infra/bicep/app/main.prod.bicepparam`](../../../infra/bicep/app/main.prod.bicepparam), then dispatch **Web infra CD** and **App infra CD**, because Azure assigns `defaultHostname` only once the sites exist.
6. **Machine.** Record each default hostname and deployment token, because configuration and source deploy cannot see them until you write them down.

```powershell
az staticwebapp show --resource-group rg-prd-dertinfo-web-uks --name swa-prd-dertinfo-web-uks --query defaultHostname -o tsv
az staticwebapp secrets list --resource-group rg-prd-dertinfo-web-uks --name swa-prd-dertinfo-web-uks --query properties.apiKey -o tsv
az staticwebapp show --resource-group rg-prd-dertinfo-app-uks --name swa-prd-dertinfo-app-uks --query defaultHostname -o tsv
az staticwebapp secrets list --resource-group rg-prd-dertinfo-app-uks --name swa-prd-dertinfo-app-uks --query properties.apiKey -o tsv
```

**Done when:** `app-prd-dertinfo-api-uks`, `func-prd-dertinfo-functions-uks`, `swa-prd-dertinfo-web-uks`, and `swa-prd-dertinfo-app-uks` exist, both default hostnames and both tokens are written down, and neither Static Web App has a custom domain.

## Milestone 5 — Configuration points at the Azure hostnames

Use the hostnames and tokens from milestone 4. `Cors:AllowedOrigins` has no space after commas, because the API splits on `,` and does not trim.

1. **Machine.** Copy [`kv-secrets.production.json.example`](../../../infra/configuration/kv-secrets.production.json.example) to gitignored `kv-secrets.production.json` and fill the recorded insights key, images key, `sql-prd-dertinfo-storage-uks.database.windows.net`, `sqldb-prd-dertinfo-storage-uks`, the live Auth0 ids and management secret, the live email keys, and any non-empty placeholder for `az-storage-functions-accountkey`, because the Functions host storage has shared keys disabled and the loader rejects an empty secret.
2. **Machine.** Run `.\Configuration\New-DertInfoConfigKeyVaultSecrets.ps1 -GitHubEnvironment production`, because App Configuration references those vault secrets.
3. **Machine.** Add `keyValues` to [`app-config.production.json`](../../../infra/configuration/app-config.production.json) for Auth0 domain `dertinfo.eu.auth0.com`, audience `api.dertinfo.co.uk`, image account `stprddertinfoimagesuks` and its endpoints, and API URIs `https://app-prd-dertinfo-api-uks.azurewebsites.net/api`. Set `MailGun:Enabled` and `SendGrid:Enabled` to the live values. Set callback and CORS to the API hostname for this first import. Run `.\Configuration\Import-DertInfoAppConfiguration.ps1 -GitHubEnvironment production -Force`.
4. **GitHub.** Store the tokens as secrets `AZURE_STATICWEBAPP_WEB_DEPLOYTOKEN_PRD` and `AZURE_STATICWEBAPP_APP_DEPLOYTOKEN_PRD`. Set `AZURE_WEBAPP_API_RESOURCENAME` to `app-prd-dertinfo-api-uks`. Set `AZURE_STATICWEBAPP_WEB_CALLBACKURL` and `AZURE_STATICWEBAPP_APP_CALLBACKURL` to `https://` plus the recorded hostname, with no trailing slash, because source CD writes those origins into `app.config.json`.
5. **Auth0.** On the website application, add callback `https://<web-host>/callback`, logout `https://<web-host>`, web origins, and CORS for the recorded website host. On the PWA application, add callback `https://<app-host>/auth/callback`, logout `https://<app-host>/home`, web origins, and CORS for the recorded PWA host. Leave the current production URLs on both applications, because the live site still uses them.
6. **Machine.** Set `Cors:AllowedOrigins` to `https://<web-host>,https://<app-host>` with no spaces, set both Auth0 callback keys to those origins with no path, run `.\Configuration\Import-DertInfoAppConfiguration.ps1 -GitHubEnvironment production -Force`, and restart the API, because the browser test is rejected until the API allows those hosts.

```powershell
az webapp restart --resource-group rg-prd-dertinfo-api-uks --name app-prd-dertinfo-api-uks
```

**Done when:** App Configuration label `Production` has domain `dertinfo.eu.auth0.com`, the Azure callback origins, and CORS with no spaces, and the two GitHub callback variables match the hostnames from milestone 4.

## Milestone 6 — Production data is on the new database and the API serves it

1. **Machine.** Run `.\Database\Copy-DertInfoSqlToProduction.ps1`, because that script copies the live database onto `sql-prd-dertinfo-storage-uks`, binds the production users, and refuses a source over the Basic 2 GB cap. At the prompt, type **Continue** to delete the previous empty database. The development database is not changed. Swagger is the check after API Src CD, not during that prompt.
2. **GitHub.** Dispatch **API Src CD** from current `main`, because that publish runs `Migrate()` on the copied database.

**Done when:** `https://app-prd-dertinfo-api-uks.azurewebsites.net/swagger/index.html` loads and shows live data, not an empty database.

## Milestone 7 — Website and PWA run on the Azure hostnames

1. **GitHub.** Dispatch **Web Src CD** and **App Src CD** from that same `main`, because that publish puts the SPAs on the recorded hostnames with the API URL and callback from milestone 5.
2. **Browser.** Open both hostnames. If login or the API URL is wrong, fix the GitHub variable or the catalog, import or redeploy, and test again.

**Done when:** on both Azure hostnames you can load the site, log in, refresh, sign out, and see production data. Images may still be missing. Live DNS is unchanged.

## Milestone 8 — Images are on the new account

1. **Machine.** Run `azcopy login`, then `.\Storage\Copy-DertInfoImagesToProduction.ps1 -SourceStorageAccount '<account>','<eventbrite-account-if-separate>'`, because the old estate used more than one account and this repo does not name them. The script stops the Function App, copies the four image containers, and starts the app. It does not enable Event Grid.
2. **GitHub.** Merge `flagImagesEventGridReady = true` in the storage production leaf and dispatch **Storage infra CD**, because Flex resizes new blobs only after Event Grid is on, and the webhook handshake needs the running host.

**Done when:** a known live image opens from `https://stprddertinfoimagesuks.blob.core.windows.net`, and a new upload is resized into `100x100` and `480x360`.

## Milestone 9 — Public DNS

The public names are `www.dertinfo.co.uk` and `app.dertinfo.co.uk`. The apex `dertinfo.co.uk` cannot be a CNAME. Decide that before this milestone.

1. **DNS.** Create the CNAME, and any TXT Azure shows, for those two names to the hostnames from milestone 4, because Static Web Apps will not finish a custom domain until those records exist.
2. **GitHub.** Merge `customDomainReady = true` in both production web and app leaves, dispatch **Web infra CD** and **App infra CD**, and wait until managed TLS shows each domain ready, because that flag binds the public names.
3. **Auth0, machine, GitHub.** Point the Auth0 URL fields, `Cors:AllowedOrigins`, both callback keys, and `AZURE_STATICWEBAPP_WEB_CALLBACKURL` / `AZURE_STATICWEBAPP_APP_CALLBACKURL` at `https://www.dertinfo.co.uk` and `https://app.dertinfo.co.uk` (PWA callback `/auth/callback`, PWA logout `/home`, no spaces in CORS). Import with `-Force`, restart the API, and dispatch **Web Src CD** and **App Src CD**, because the deployed SPAs still have the Azure hostnames from milestone 7.
4. **Browser.** Repeat the milestone 7 checks, plus an image, on the public hosts.

**Done when:** load, login, refresh, sign-out, production data, and an image pass on `https://www.dertinfo.co.uk` and `https://app.dertinfo.co.uk`.
