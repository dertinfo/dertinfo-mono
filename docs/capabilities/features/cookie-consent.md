---
name: Cookie consent
type: feature
status: active
updated: 2026-10-03
id: public.cookie-consent
roles: [public-user]
---

# Feature: Cookie consent

## Description

On the public website, visitors are asked for **cookie consent**. This is required under GDPR because the site uses cookies.

## User roles

- **public-user** — see and respond to the cookie consent experience on public pages

## Behaviour

- Cookie consent is presented on the public part of the site.
- Supports lawful use of cookies for site functionality/analytics as configured.

## Limitations

- Distinct from signed-in [GDPR / terms consent](account-gdpr-consent.md) on the user account.
- Cookie and privacy policy text is part of [public content](public-content.md) (e.g. cookie policy pages).
- The banner can sit underneath the signed-in navigation, so the rest of the site can be used without accepting cookies. That layout is future work. The smoke suite accepts cookies once, then continues as if the banner is gone.

## Scenarios

```gherkin
Feature: Cookie consent
  As a visitor
  I want to accept cookies when the site opens
  So that the rest of the site can be used without the banner

  Scenario: Visitor accepts cookies
    Given the visitor has not accepted cookies
    When the visitor opens the site and accepts cookies
    Then the banner is dismissed
    And later visits in this run start with that consent
```

```yaml
scenarios:
  - id: public.cookie-consent.visitor-accepts
    persona: public-user
    sequence: 1
    provides:
      - cookie-consent
```
