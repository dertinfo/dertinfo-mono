---
name: Member
type: role
status: active
updated: 2026-07-26
id: member
---

# Role: Member

## Summary

A signed-in user of the platform (authenticated account), with or without additional scoped roles.

## Capabilities

- [Log in](../features/auth-login.md)
- [Log out](../features/auth-logout.md)
- [Stay signed in](../features/auth-session-continuity.md)

## Limitations

- Group, event, venue, DoD, and platform administration require the corresponding roles:
  - [Group member](group-member.md) / [Group administrator](group-admin.md)
  - [Event administrator](event-admin.md)
  - [Venue administrator](venue-admin.md)
  - [Dert of Derts administrator](dod-admin.md)
  - [Super administrator](super-admin.md)

> Starter stub for reverse-engineering — expand from product behaviour. Roles may combine on one account.
