---
name: Registration email templates
type: feature
status: active
updated: 2026-07-26
id: registration.email-templates
roles: [event-admin, group-admin]
---

# Feature: Registration email templates (additional)

Supporting capability for registration and invoicing — important and widely used, documented here as an **additional** feature rather than a primary domain.

## Description

Events use email templates that are sent automatically (or available when needed) around registration lifecycle.

## Templates

| Template | When | Content focus |
|----------|------|----------------|
| **Registration submitted** | Automatic when a group submits a registration | Summary of requested team/member activities and tickets |
| **Registration confirmed** | Automatic when an event admin confirms a registration | Invoice, pricing, and how the group admin should pay |
| **Event cancelled** | When an event must be cancelled | Cancellation communication |

## User roles

- **group-admin** — receives submitted/confirmed (and cancellation) emails for their registrations
- **event-admin** — confirmation and cancellation flows that trigger templates; may maintain template content as the product allows

## Behaviour

- Submitted and confirmed emails are tied to [Submit event registration](registration-submit.md) and [Confirm registrations](registration-confirm.md).
- Confirmed email supports the [Invoicing](invoicing.md) path.
- Delivery uses the platform [Send email](send-email.md) capability (SendGrid or Mailgun).

## Limitations

- Exact editable fields and branding are product/configuration detail beyond this overview.
