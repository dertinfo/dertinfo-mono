# Planned: Hosting and cost decisions (durable technical doc)

**Status:** Parked — blocked on subscription foundation ([agent-safe-subscription-foundation.md](agent-safe-subscription-foundation.md)). Decisions recorded here; publish a durable technical page when this work is executed.

**Related:** [Azure estate (dev/prd)](azure-estate-dev-prd.md), [Bicep AVM + infra pipelines](bicep-avm-infra-pipelines.md)

---

## Intent

Write down **why** the Azure hosting shape is what it is (usage profile → SKUs → rejects), so future changes do not re-litigate cost without context.

When completing this planned fix, publish a durable page and link it from the architecture overview / docs hub. **Do not** put it under `docs/operations/decisions/` (forbidden by the [documentation guide](../../documentation-guide.md)).

### Target location (choose on execution)

Preferred:

- [`docs/technical/architecture/hosting-and-cost.md`](../../technical/architecture/) (new file), **or**
- `docs/technical/decisions/` on first use (e.g. `hosting-and-cost.md`) if a decisions subsection is introduced deliberately

Also link from [`docs/technical/architecture/overview.md`](../../technical/architecture/overview.md) and optionally [`docs/README.md`](../../README.md).

---

## Usage profile (drives cost)

DertInfo is **not** steady SaaS load:

| Period | Behaviour | Hosting implication |
|--------|-----------|---------------------|
| Most of the year | Light / idle — browse past results and history | Cheapest **always-available** tiers; cold starts acceptable |
| Competition weekend | Heavy — scores entered, live viewing; especially **one live day** | Temporary API (and optionally SQL) scale-up |
| After the event | Back to idle | Scale back down |

Scale-to-zero platforms are a **poor fit** for a year-round browsable archive (wake + API migrate-on-start). Year-round dedicated Basic App Service would **waste money** for ~50 idle weeks.

---

## Confirmed / agreed SKUs (current and new-stack intent)

| Resource | Test / current notes | New-stack intent |
|----------|----------------------|------------------|
| Static Web Apps | Free (test confirmed) | **Free** for `dev` and `prd` |
| App Service (API) | Test **F1**; prod **D1** most of year; **Basic (B1)** for competition weekend | Same seasonal pattern: F1 `dev`, D1 `prd` off-season, B1 for event |
| Azure SQL | Logical server free; database **Basic** (test + prod); Basic **just manages** the event day | Keep **Basic**; optional short **S0** for event if DTU pressure |
| Key Vault | **Standard** | Standard |
| Functions | Consumption **Y1** | Keep Y1 |
| Image storage | Existing blob accounts | Standard_LRS (or equivalent) in `storage` part |

Indicative idle Azure bill (both envs, order of magnitude): roughly **£25–40/mo** before Auth0/SendGrid; event weekend is a small pro-rata bump (B1 days ± optional SQL S0), not a second full-price tier all year.

---

## Decisions

| Decision | Rationale |
|----------|-----------|
| Keep **SWA Free** for web and app | Custom domains + free SSL; Blob alone cannot do HTTPS custom domains; Blob+CDN costs more than free SWA |
| Keep **App Service** (not ACI / Container Apps / Functions-as-API) | Long-lived ASP.NET + EF migrate-on-start; F1/D1 already cheapest sensible path; always-on ACI ≥ B1 cost |
| Seasonal **D1 → B1** for prod API | Removes slow warmup and absorbs event traffic only when needed |
| Stay on **Azure SQL Basic** | Engine migration (Postgres/MySQL/Cosmos) does not pay back; Basic fits idle months |
| Keep **Functions Consumption** | Bursty image resize; near-zero off-season |
| New stack RGs per part | Cost-neutral; enables RG-scoped permissions (see estate planned fix) |

---

## Explicitly rejected (for cost / fit)

| Option | Why not |
|--------|---------|
| Blob static website + Azure CDN / Front Door instead of SWA | More cost and ops; SWA Free already covers domains/HTTPS |
| Always-on ACI for API | Not cheaper than D1 off-season; weak HTTP platform |
| Container Apps scale-to-zero for prod API | Hurts daily archive browsing + cold start / migrate |
| Year-round B1 “just in case” | Wastes ~11 months of dedicated compute |
| Postgres / MySQL / Cosmos as primary DB | High migration cost vs small SKU savings at this scale |
| Main API as HTTP Azure Functions | Architecture mismatch |

---

## Checklist (when ready to execute)

- [ ] Draft `hosting-and-cost.md` (or decisions page) from the content above; refresh SKU/£ figures from portal if needed
- [ ] Link from architecture overview and docs hub
- [ ] Cross-link estate + Bicep planned fixes (or their completed technical docs)
- [ ] Do not duplicate full CI/CD or configuration guides — link [`docs/technical/infra/`](../../technical/infra/) instead

---

## Remaining / follow-up

- **API App Service Linux** — this stack ships Windows + `win-x86` to match current API CD. A later move to Linux (cost and interoperability) is recorded in [cicd-future-phase.md](cicd-future-phase.md). Do not treat year-round Windows as a long-term hosting decision.

## Out of scope for this planned fix

- Creating RGs or Bicep
- Changing live SKUs
- Auth0 / SendGrid commercial tier analysis
