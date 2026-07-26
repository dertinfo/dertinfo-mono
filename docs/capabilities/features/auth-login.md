---
name: Log in
type: feature
status: active
updated: 2026-07-26
id: auth.login
roles: [public-user, member]
---

# Feature: Log in

## Description

A visitor can authenticate and become a signed-in user of the platform.

## User roles

- **public-user** — can start sign-in
- **member** — result of a successful sign-in (and higher roles inherit signed-in behaviour)

## Behaviour

- User can initiate sign-in from the client experiences that support it.
- After successful sign-in, the user is treated as authenticated for subsequent actions that require it.

## Limitations

- Unauthenticated users cannot access member-only or administrator capabilities.

> Starter stub — implementation-free; expand during reverse-engineering.
