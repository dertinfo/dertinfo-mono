---
name: Invoicing and payment status
type: feature
status: active
updated: 2026-07-26
id: invoicing.payment-status
roles: [event-admin, group-admin]
---

# Feature: Invoicing and payment status

## Description

When registrations are confirmed, invoices are created for groups. Event administrators mark invoices paid; group administrators can see that payment has been received.

## User roles

- **event-admin** — mark invoices paid (or unpaid) for their events
- **group-admin** — view invoice and payment status for their group

## Behaviour

- Invoices are produced on registration confirmation (with pricing and payment instructions in the confirmed email).
- Event admin marks an invoice as **paid** when payment is received.
- Group admin can see that the invoice is marked paid.

## Limitations

- This capability describes status tracking in the platform, not a specific payment provider.

See [Confirm registrations](registration-confirm.md) and [Domain glossary](../system/domain-glossary.md).
