---
name: Playwright standards
type: standards
status: active
updated: 2026-10-03
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

The website is already running. Before scenarios, the runner removes whatever container is publishing port `44000`, deletes its SQL data volume and the Compose volume `sqlserver-data`, starts a new SQL Server container, and restarts the API so migrations run on that empty database. The previous smoke chain is discarded. Auth0 session files are left in place. SQL is Docker. The API, image resize, Azurite, website, and PWA are native. `infra/dev/runtime.json` must set `sql.mode` to `docker` and `api.mode` to `native` or `docker`. See [tests/e2e/README.md](../../../../tests/e2e/README.md).

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

Provided names in this suite: `cookie-consent`, `session:event-admin`, `session:group-admin`, `createdEventId`, `createdGroupId`, `configuredEventId`, `configuredGroupId`, `addedMemberId`, `addedGuestId`, `addedTeamId`, `addedIndividualActivityId`, `addedTeamActivityId`, `pendingRegistrationId`, `submittedRegistrationId`.

`public.cookie-consent.visitor-accepts` runs before sign-in and public browsing. It clicks Accept Cookies and stores that cookie. Later browser contexts in the same run start with the cookie already set. They do not click the banner again.

## Prerequisites

The first action of a scenario checks that every required scenario has passed and that its tokens are present. The check does nothing else.

When they are present, the scenario continues. When any are missing, the scenario fails immediately with `prerequisites not available` and the missing ids or tokens. Its own steps do not run. That failure is how a full-suite log shows a missing predecessor, separate from a failure of the scenario's own checks.

Do not delete a `requires` entry to make a later scenario pass. Fix the scenario that should have provided the token.

## Personas

Auth0 users for the local tenant `dertinfodev.eu.auth0.com` use `{role-id}-1@dertinfo.co.uk`. This suite signs in `event-admin-1@dertinfo.co.uk` and `group-admin-1@dertinfo.co.uk`. Passwords stay in `tests/e2e/.env`. Payloads are in `tests/e2e/auth0-personas.json`.

Each persona has one saved browser session under `tests/e2e/web/smoke/state/sessions/`. A later scenario reuses that session until it expires. Auth0 is not called again for a still-valid session. When that persona's browser closes, the suite writes the session file again so the saved tokens match the browser that just finished.

Each scenario opens its own browser context. The rejected-cache scenario does not write those session files. Contexts use locale `en-GB` and timezone `Europe/London`.

## Helpers

Put a behaviour in `helpers.mjs` when more than one scenario needs it, and make that helper do one job:

- Load a persona email and password.
- Open a browser context, including one restored from a saved session.
- Sign in through Universal Login and write that persona's session.
- Carry the accepted cookie-consent cookie into a new browser context, read the page text, and recognise the session-error page.
- Read and write provided tokens.

A helper used by one scenario stays in that scenario file. Signing in does not also create a group. Opening a public page does not also upload a photo. Do not add a mode flag that makes one function run several scenarios. The scenario module owns its own assertions.

## Locators

Controls the suite must find, and that have no stable accessible name, carry `data-testid`. Playwright reads that attribute with `getByTestId`. Buttons that already have a unique role and name stay on `getByRole`.

List rows and picker options also carry the id or name the scenario needs: `data-registration-id`, `data-card-title`, `data-option-name`, `data-selected`, `data-event-type`, `data-photo-name`. The signed-in shell exposes `app-ready` after warmup. The warmup screen exposes `app-warming`. The cookie banner exposes `cookie-consent` while it is open. After Accept Cookies, that element is removed. A page load accepts it when it is visible, and skips that step when it is already gone. Public pages expose `public-home`, `public-results`, `public-history`, `public-community`, `public-notations`, and `public-dertofderts`.

Do not locate rows with `ng-reflect-*`. Angular emits those attributes only in development. Do not click with `force: true`. A click waits until the control is visible and enabled.

## Test data

Smoke data lives in `tests/e2e/web/smoke/config/`. `groups.json` holds two groups. `events.json` holds two events. Keys match the database entities (`GroupName`, `GroupBio`, `GroupMembers`, `Teams`, `Activities`, and so on). Enum values use the C# names (`activeMember`, `guest`, `INDIVIDUAL`, `TEAM`, `StandardDert`).

The current scenarios enter the first group and the first event. Groups are named for The Simpsons. Events are named for a zoo. The first group is `The Simpsons`. The first event is `City Zoo Gathering`.

Each record has an `Image` file name under `tests/e2e/web/smoke/fixtures/`. Groups use `family.jpg`. Events use `giraffe.jpg`.

## When a run fails

Correct the test when the test is wrong, and run it again. Do not change the website, the API, or Auth0 from a smoke failure. Do not weaken a check so that an application failure counts as a pass.
