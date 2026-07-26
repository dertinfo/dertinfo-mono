---
name: Venue score capture
type: feature
status: active
updated: 2026-07-26
id: scoring.venue-capture
roles: [venue-admin, event-admin]
---

# Feature: Venue score capture

## Description

During competition, scores for a dance can be entered live and printed judge score sheets photographed and uploaded (typically via the **app**).

## User roles

- **venue-admin** — capture scores and score-sheet images for venues they administer
- **event-admin** — may also perform live score entry at the venue

## Behaviour

- For a given **dance** at a venue, enter scores (judge × score category × dance).
- Photograph filled-in printed score sheets and upload them.
- Enables rapid capture so scores can be reviewed centrally.

## Limitations

- Captured scores are not final until [validated](scoring-validate.md).
- Once scores have been **checked/validated**, the **venue admin can no longer change** those scores or score sheets.
- **Publishing** results is exclusively an event-admin function — see [Publish competition results](results-publish.md).

See [Domain glossary](../system/domain-glossary.md).
