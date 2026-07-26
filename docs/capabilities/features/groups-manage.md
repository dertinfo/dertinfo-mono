---
name: Create and manage group
type: feature
status: active
updated: 2026-07-26
id: groups.manage
roles: [member, group-admin]
---

# Feature: Create and manage group

## Description

A signed-in user can **create** a **group** (sword-dancing club). Only **group administrators** can manage that group afterwards (members, teams, registrations, images, and related details).

## User roles

- **member** — any logged-in website user may **create** a new group (no extra restriction). Creating a group typically makes them a group admin for it.
- **group-admin** — **manage** groups they are authorised for. Ordinary group **members** cannot manage the group.

## Behaviour (group admin)

- Add and remove **members**.
- Add and remove **teams**; update team bio and other team information; delete a team.
- Upload images for the group / teams (processed by [Image handling](../system/image-handling.md)).
- View registrations for the group.
- **Edit registrations** while they are still open — registrations stop being editable by the group once an **event admin confirms** the registration.
- Cannot create an **event** — see [Create event](events-create.md).

## Limitations

- Group members without admin rights cannot manage members, teams, images, or registrations.
- Confirmed registrations are no longer editable by the group admin.

See [Domain glossary](../system/domain-glossary.md).
