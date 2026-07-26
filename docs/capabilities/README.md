---
name: Platform capability catalogue
type: index
status: active
updated: 2026-07-26
---

# Platform capability catalogue

Describes **WHAT** the platform can do, organised by roles, features, and system capabilities. Detached from technical implementation (no stack-specific detail here).

This catalogue is a **starter skeleton** for a later reverse-engineering pass (e.g. major UI upgrades). Expand it from the product, not from guesswork.

## Structure

- [Roles](roles/) — who can act in the platform (public user, member, group member/admin, event admin, venue admin, DoD admin, super admin)
- [Features](features/) — user-facing capabilities
- [System](system/) — cross-cutting platform behaviour
- Inventory: [`capabilities.yaml`](capabilities.yaml)

Roles may combine on one account (scoped permissions are not mutually exclusive).

Technical mapping of Auth0 `app_metadata` → JWT claims: [Authentication](../technical/subsystems/authentication.md).

## How to update

1. Add or edit the markdown page (role / feature / system).
2. Add or update the matching row in `capabilities.yaml` (same `id`).
3. Do **not** regenerate pages from YAML.
4. Keep descriptions implementation-free.

See [Documentation guide](../documentation-guide.md).
