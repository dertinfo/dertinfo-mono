---
name: User account and GDPR consent
type: feature
status: active
updated: 2026-07-26
id: account.gdpr-consent
roles: [member]
---

# Feature: User account and GDPR consent

## Description

Signed-in users manage account-related information and must accept the website terms and give GDPR consent. The platform operates as a **data processor**; **event administrators** are **data owners** (controllers) for event-related personal data. Users agree to the site’s terms and conditions, which are available in the [public content](public-content.md) section of the website.

## User roles

- **member** (and other signed-in users) — provide / maintain consent and account settings as required
- **event-admin** — data owners for personal data processed in the context of their events (organisational responsibility; not a separate “consent UI” for them)

## Behaviour

- Users are asked for **GDPR consent** consistent with the platform’s processor role.
- Consent is tied to acceptance of the website **terms and conditions** (publicly available).
- Account settings / profile areas support the signed-in user’s account information as the product provides.

## Limitations

- Cookie consent for anonymous browsing is separate — see [Cookie consent](cookie-consent.md).
- Exact legal wording lives in the public terms / data policy pages.

See [Domain glossary](../system/domain-glossary.md).
