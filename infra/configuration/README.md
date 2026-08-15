# Azure configuration artefacts

Checked-in, **non-secret** files used to configure Azure / Entra so GitHub Actions can deploy. They are not local app secrets ([`infra/secrets/`](../secrets/README.md)) and not Bicep parameters ([`infra/bicep/`](../bicep/)).

| File | What it is |
|------|------------|
| `github-azure-dev-credential.json` | Federated credential body for GitHub Environment **`development`** |
| `github-azure-prd-credential.json` | Federated credential body for GitHub Environment **`production`** |

These JSON files describe the **trust relationship** (issuer, subject, audience) applied to an Entra app registration. GitHub Actions then signs in with OIDC instead of an Azure client secret.

**How to create the Entra apps, apply these files, and set GitHub Environment variables:** [GitHub Actions OIDC to Azure (federated credentials)](../../docs/technical/guides/github-azure-federated-credentials.md).
