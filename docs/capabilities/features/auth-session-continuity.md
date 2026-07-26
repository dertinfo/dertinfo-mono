---
name: Stay signed in
type: feature
status: active
updated: 2026-07-26
id: auth.session-continuity
roles: [member, group-member, group-admin, event-admin, venue-admin, dod-admin, super-admin]
---

# Feature: Stay signed in

## Description

A signed-in user can continue using the platform across page loads and within a session window without signing in again every time, until the session ends or they sign out.

## User roles

- **member**, **group-member**, **group-admin**, **event-admin**, **venue-admin**, **dod-admin**, **super-admin**

## Behaviour

- Returning to the client within a valid session keeps the user authenticated.
- Session end or [log out](auth-logout.md) requires sign-in again for protected actions.

## Limitations

- Does not grant roles the user does not have; only preserves an existing authenticated state.

> Starter stub — implementation-free; expand during reverse-engineering.
