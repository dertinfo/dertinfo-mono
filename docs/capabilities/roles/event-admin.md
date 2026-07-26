---
name: Event administrator
type: role
status: active
updated: 2026-07-26
id: event-admin
---

# Role: Event administrator

## Summary

Administers one or more **events** (e.g. Dert): create event, registrations, invoicing, competitions, score validation, reporting, and publishing results. Typical website “organiser” role.

## Capabilities

- [Create event](../features/events-create.md)
- [Confirm registrations](../features/registration-confirm.md)
- [Invoicing and payment status](../features/invoicing.md) — mark invoices paid
- [Registration email templates](../features/registration-email-templates.md)
- [Run competitions](../features/competitions-run.md)
- [Venue score capture](../features/venue-score-capture.md) — may also enter scores live at a venue
- [Validate scores](../features/scoring-validate.md)
- [Publish competition results](../features/results-publish.md)
- [Send email](../features/send-email.md) — registration and related outbound mail
- [In-app notifications](../features/in-app-notifications.md) — rarely used
- Auth: [Log out](../features/auth-logout.md), [Stay signed in](../features/auth-session-continuity.md)

## Limitations

- After scores are checked, venue admins can no longer edit those scores/sheets.
- A separate “competition admin” role is not used in current practice.
- As **data owners** for event-related personal data, event admins hold organisational responsibility under GDPR — see [User account and GDPR consent](../features/account-gdpr-consent.md).

See [Domain glossary](../system/domain-glossary.md).
