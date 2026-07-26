---
name: System architecture overview
type: architecture
status: active
updated: 2026-07-26
---

# System architecture overview

## Purpose

High-level view of the DertInfo platform as a set of deployable subsystems. This page describes **what** each part is responsible for and how they relate. Per-app install and debug steps stay in each app’s README.

## Subsystems

| Subsystem | Path | Responsibility |
|-----------|------|----------------|
| API | [`apps/dert-api/`](../../../apps/dert-api/) | HTTP API, domain logic, EF Core / SQL Server, Auth0 JWT validation |
| Website | [`apps/dert-web/`](../../../apps/dert-web/) | Public / organiser Angular SPA (Static Web Apps locally via SWA CLI) |
| App (PWA) | [`apps/dert-app/`](../../../apps/dert-app/) | Ionic + Angular progressive web app |
| Image resize | [`apps/dert-functions/`](../../../apps/dert-functions/) | Azure Functions worker for image processing against blob storage |
| Shared contracts | [`packages/shared-contracts/`](../../../packages/shared-contracts/) | OpenAPI / Swagger contract and generated TypeScript clients |

Supporting local infrastructure (not product features): SQL Server, Azurite (blob/queue/table), and optional Docker Compose services. See [Local development](../guides/local-development.md) and [Configuration](../infra/configuration.md).

## Data and API contracts

Endpoint and schema documentation lives in the **OpenAPI / Swagger** definition under shared contracts (and the API’s Swagger UI when running). Do not duplicate endpoint or table catalogues under `docs/technical/`.

## Local estate topology

Fixed local ports (native or Docker):

| Port | Service |
|------|---------|
| 44100 | API |
| 44200 | Website (SWA) |
| 44300 | App / PWA (SWA) |
| 44400 | Image resize Functions |
| 10000–10002 | Azurite |

Orchestration: `npm run doctor` / `start` / `stop` / `status` — see [Local development](../guides/local-development.md).

## Getting started per subsystem

| Subsystem | Getting started |
|-----------|-----------------|
| API | [`apps/dert-api/README.md`](../../../apps/dert-api/README.md) |
| Website | [`apps/dert-web/README.md`](../../../apps/dert-web/README.md) |
| App | [`apps/dert-app/README.md`](../../../apps/dert-app/README.md) |
| Functions | [`apps/dert-functions/README.md`](../../../apps/dert-functions/README.md) |
| Whole estate | [Root README — first-time setup](../../../README.md#first-time-setup-clone--running-locally) |

## Delivery

CI/CD and environments: [CI/CD](../infra/cicd.md).
