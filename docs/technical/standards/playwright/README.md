---
name: Playwright standards
type: standards
status: active
updated: 2026-09-27
---

# Playwright standards

How website smoke tests are named, ordered, and shared. Further Playwright tests follow this page.

Capability pages say what the product does. This page says how a smoke scenario is wired to those pages. It is not a capability description.

## Where tests live

| Path | Role |
|------|------|
| `tests/e2e/web/smoke/scenarios/` | One module per scenario id |
| `tests/e2e/web/smoke/scenarios/run.mjs` | Reads feature contracts and runs the suite |
| `tests/e2e/web/smoke/helpers.mjs` | Shared mechanics used by more than one scenario |
| `tests/e2e/web/smoke/state/` | Gitignored session files and the chain of provided tokens |
| `tests/e2e/.env` | Gitignored persona emails and passwords |
| `tests/e2e/auth0-personas.json` | `app_metadata` to paste onto Auth0 users. No passwords |

`npm run test:web:smoke` from `tests/e2e` runs every scenario. `node web/smoke/scenarios/run.mjs <scenario-id>` runs that scenario and the scenarios it requires.

The site is already running. SQL is Docker on `localhost,44000` with an empty database for a clean run. The API, image resize, Azurite, website, and PWA are native. See [tests/e2e/README.md](../../../../tests/e2e/README.md).

## Scenario id

A login scenario is `<feature id>.<persona>`, because the feature is already the action.

`auth.login.event-admin` logs in as the event administrator and provides `session:event-admin`.

Any other scenario is `<feature id>.<actor>-<verb>`.

`events.create.event-admin-creates` is the event administrator creating an event. `public.content.visitor-browses` is a visitor browsing public pages.

The scenario id is never given a `[n]` suffix. The action name stays the same when a second object is created.

## Contract

Each feature page that the smoke suite covers has a `## Scenarios` section. Gherkin (Given, When, Then, And, But) stays in one block. The runner does not read those sentences.

The runner reads the YAML contract on that page:

```yaml
scenarios:
  - id: events.create.event-admin-creates
    persona: event-admin
    sequence: 20
    requires:
      - auth.login.event-admin
    provides:
      - createdEventId
```

`persona` must be one of the feature's `roles`. `requires` names other scenario ids. `provides` names the tokens that scenario leaves behind. `sequence` is only a tie-break when `requires` does not decide the order. Lower numbers run first.

A name with no brackets is instance 1. `createdEventId` and `createdEventId[1]` are the same value. `auth.login.event-admin` and `auth.login.event-admin[1]` are the same login. A second event provides `createdEventId[2]` and does not replace the first. A later scenario requires `events.create.event-admin-creates[2]` when it must use that second event. Omit `[n]` when there is only one.

Provided names in this suite: `session:event-admin`, `session:group-admin`, `createdEventId`, `createdGroupId`, `configuredEventId`, `configuredGroupId`, `addedMemberId`, `addedGuestId`, `addedTeamId`, `addedIndividualActivityId`, `addedTeamActivityId`, `pendingRegistrationId`, `submittedRegistrationId`.

## Prerequisites

The first action of a scenario checks that every required scenario has passed and that its tokens are present. The check does nothing else.

When they are present, the scenario continues. When any are missing, the scenario fails immediately with `prerequisites not available` and the missing ids or tokens. Its own steps do not run. That failure is how a full-suite log shows a missing predecessor, separate from a failure of the scenario's own checks.

Do not delete a `requires` entry to make a later scenario pass. Fix the scenario that should have provided the token.

## Personas

Auth0 users for the local tenant `dertinfodev.eu.auth0.com` use `{role-id}-1@dertinfo.co.uk`. This suite signs in `event-admin-1@dertinfo.co.uk` and `group-admin-1@dertinfo.co.uk`. Passwords stay in `tests/e2e/.env`. Payloads are in `tests/e2e/auth0-personas.json`.

Each persona has one saved browser session under `tests/e2e/web/smoke/state/sessions/`. A later scenario reuses that session until it expires. Auth0 is not called again for a still-valid session.

Each scenario opens its own browser context. The rejected-cache scenario does not write those session files.

## Helpers

Put a behaviour in `helpers.mjs` when more than one scenario needs it, and make that helper do one job:

- Load a persona email and password.
- Open a browser context, including one restored from a saved session.
- Sign in through Universal Login and write that persona's session.
- Accept the cookie banner, read the page text, and recognise the session-error page.
- Read and write provided tokens.

A helper used by one scenario stays in that scenario file. Signing in does not also create a group. Opening a public page does not also upload a photo. Do not add a mode flag that makes one function run several scenarios. The scenario module owns its own assertions.

## Test data

Groups created by this suite are named for The Simpsons. Events are named for a zoo or animals. The first group is `The Simpsons`. The first event is `City Zoo Gathering`.

## When a run fails

Correct the test when the test is wrong, and run it again. Do not change the website, the API, or Auth0 from a smoke failure. Do not weaken a check so that an application failure counts as a pass.
