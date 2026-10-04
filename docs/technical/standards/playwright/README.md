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
| `tests/e2e/web/smoke/scenarios/` | One module per smoke flow |
| `tests/e2e/web/smoke/steps/` | Page actions a flow calls, in order. The runner does not load this folder |
| `tests/e2e/web/smoke/scenarios/run.mjs` | Reads feature contracts and runs the suite |
| `tests/e2e/web/smoke/helpers.mjs` | Shared mechanics used by more than one flow |
| `tests/e2e/web/smoke/state/` | Gitignored session files and the chain of provided tokens |
| `tests/e2e/.env` | Gitignored persona emails and passwords |
| `tests/e2e/auth0-personas.json` | `app_metadata` to paste onto Auth0 users. No passwords |

`npm run test:web:smoke` from `tests/e2e` runs every smoke flow. It does not run `auth.session-continuity.rejected-cache-returns-to-sign-in`. Pass that id to run it on its own.

`node web/smoke/scenarios/run.mjs <scenario-id>` runs that flow only. It does not run the flows it requires, and it does not reset the database. The required tokens must already be in the chain. When they are missing, the flow fails with `prerequisites not available` and does not start.

The website is already running. A full smoke command removes whatever container is publishing port `44000`, deletes its SQL data volume and the Compose volume `sqlserver-data`, starts a new SQL Server container, and restarts the API so migrations run on that empty database. The previous smoke chain is discarded. Auth0 session files are left in place. SQL is Docker. The API, image resize, Azurite, website, and PWA are native. `infra/dev/runtime.json` must set `sql.mode` to `docker` and `api.mode` to `native` or `docker`. See [tests/e2e/README.md](../../../../tests/e2e/README.md).

## Scenario id

A smoke flow names the area, the actor, the action, and the path number: `<area>.<actor>.<action>-scenarioN`.

`group.groupadmin.createandconfigure-scenario1` is the group administrator creating and configuring one group. `scenario2` would be a different path through that same action, not a second copy of `scenario1`.

A flow that is still a single behaviour keeps `<feature id>.<actor>-<verb>`. `public.cookie-consent.visitor-accepts` accepts the cookie banner. `public.content.visitor-browses` is a visitor browsing public pages.

The scenario id is never given a `[n]` suffix. The action name stays the same when a second object is created. The path number is `-scenarioN`, not `[n]`.

## Contract

Each feature page that the smoke suite covers has a `## Scenarios` section. Gherkin (Given, When, Then, And, But) stays in one block. The runner does not read those sentences.

The runner reads the YAML contract on that page:

```yaml
scenarios:
  - id: event.eventadmin.createandconfigure-scenario1
    persona: event-admin
    sequence: 20
    requires:
      - public.cookie-consent.visitor-accepts
    provides:
      - configuredEventId
      - addedIndividualActivityId
      - addedTeamActivityId
  - id: events.create.event-admin-creates
    persona: event-admin
    sequence: 20
    requires:
      - auth.login.event-admin
    provides:
      - createdEventId
    coveredBy: event.eventadmin.createandconfigure-scenario1
```

`persona` must be one of the feature's `roles`. `requires` names other scenario ids. `provides` names the tokens that scenario leaves behind. `sequence` is only a tie-break when `requires` does not decide the order. Lower numbers run first.

`coveredBy` names the flow that now performs that behaviour. The Gherkin stays on the feature page. The runner does not execute a `coveredBy` entry. The flow's own YAML, without `coveredBy`, is the contract that runs.

A name with no brackets is instance 1. `configuredEventId` and `configuredEventId[1]` are the same value. A second event provides `configuredEventId[2]` and does not replace the first. A later scenario requires that second flow when it must use the second event. Omit `[n]` when there is only one. `-scenarioN` is a different path, not an instance number.

Provided names in this suite: `cookie-consent`, `session:event-admin`, `session:group-admin`, `createdEventId`, `createdGroupId`, `configuredEventId`, `configuredGroupId`, `addedMemberId`, `addedGuestId`, `addedTeamId`, `addedIndividualActivityId`, `addedTeamActivityId`, `pendingRegistrationId`, `submittedRegistrationId`.

`public.cookie-consent.visitor-accepts` runs before sign-in and public browsing. It clicks Accept Cookies and stores that cookie. Later browser contexts in the same run start with the cookie already set. They do not click the banner again.

## Prerequisites

The first action of a scenario checks that every required scenario has passed in this run and that its tokens are present. The check does nothing else.

When they are present, the scenario continues. When any are missing, the scenario fails immediately with `prerequisites not available` and the missing ids or tokens. Its own steps do not run. That failure is how a full-suite log shows a missing predecessor, separate from a failure of the scenario's own checks.

A named scenario id is the exception. It does not re-run the flows it requires. It continues when those flows' tokens are already in the chain, and it fails the same way when they are not.

Do not delete a `requires` entry to make a later scenario pass. Fix the scenario that should have provided the token.

## Personas

Auth0 users for the local tenant `dertinfodev.eu.auth0.com` use `{role-id}-1@dertinfo.co.uk`. This suite signs in `event-admin-1@dertinfo.co.uk` and `group-admin-1@dertinfo.co.uk`. Passwords stay in `tests/e2e/.env`. Payloads are in `tests/e2e/auth0-personas.json`.

Each persona has one saved browser session under `tests/e2e/web/smoke/state/sessions/`. A later flow reuses that session until it expires. When that persona's browser closes, the suite writes the session file again so the saved tokens match the browser that just finished.

A flow that signs in does that once at the start, then keeps one browser for the rest of that actor's steps. A later part logs `PASS <flow id> > <part>` as soon as it finishes, or `FAIL <flow id> > <part>` and the error when it does not. The flow then fails, and the runner still prints `FAIL` for the flow id.

The registration review flow opens another browser when the actor changes. The rejected-cache scenario does not write those session files. Contexts use locale `en-GB` and timezone `Europe/London`.

## Helpers

Put a behaviour in `helpers.mjs` when more than one scenario needs it, and make that helper do one job:

- Load a persona email and password.
- Open a browser context, including one restored from a saved session.
- Sign in through Universal Login and write that persona's session.
- Carry the accepted cookie-consent cookie into a new browser context, read the page text, and recognise the session-error page.
- Read and write provided tokens.

A page action used by one flow lives in `tests/e2e/web/smoke/steps/`. The flow calls those actions in order. Do not copy the clicks into the flow. Signing in does not also create a group. Opening a public page does not also upload a photo. Do not add a mode flag that makes one function run several flows.

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
