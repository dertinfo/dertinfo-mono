---
name: Group member
type: entity
status: active
updated: 2026-09-27
id: entity.group-member
---

# Entity: Group member

## Description

A person on a [group](group.md)'s list. Members are not permanently assigned to teams. A guest is the same object with type guest: booked like a member, labelled as a non-dancer companion (for example a spouse), and the usual first cut when capacity is tight.

## What can be done

- **Add** — group-admin, via [Create and manage group](../features/groups-manage.md).
- **Update** — group-admin. Change name and contact details.
- **Remove** — group-admin.
- **Include on a registration** — group-admin, while a [registration](registration.md) is still editable. Guests and dancers are both lines on that registration.

## States

This entity has no lifecycle states.

**Member type** is a setting: active member, or guest.

See [Domain glossary](../system/domain-glossary.md).
