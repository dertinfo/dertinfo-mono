# Documentation guide

Defines the documentation system for this monorepo: three tracks, templates, metadata, and rules for humans and AI agents.

## Purpose

| Track | Answers | Lives under |
|-------|---------|-------------|
| Capabilities | WHAT the platform can do | `docs/capabilities/` |
| Technical | HOW it is built and operated | `docs/technical/` |
| Operations | Working history while solving things in-repo | `docs/operations/` |

Docs explain the project in a discoverable way. They are **not** a replacement for GitHub Issues/Projects.

Per-app **getting started** stays in each app README; shared and estate-level material belongs here, with READMEs linking in.

## Folder structure

```
docs/
  README.md
  documentation-guide.md
  capabilities/
    README.md
    capabilities.yaml
    roles/
    features/
    system/
  technical/
    architecture/
    guides/
    infra/
    subsystems/            # Auth0, security, …
    standards/             # Angular, .NET, … technology conventions
  operations/
    changelogs/
    planned-fixes/
    investigations/
    security/              # Point-in-time operational security reviews
    agents/
```

### Create on first use (do not scaffold empty)

| Folder | When to create |
|--------|----------------|
| Further `technical/subsystems/` pages | When a new named subsystem needs a deep dive |
| Further `technical/standards/<tech>/` | When documenting house rules for another stack |
| Further capability domains | As reverse-engineering fills the catalogue |
| `operations/security/` | When recording an operational security review (e.g. workflow / OIDC gates) |

### Do not create

| Folder | Why |
|--------|-----|
| `technical/api/` | OpenAPI / Swagger (`packages/shared-contracts/`, API Swagger UI) is the source of truth |
| `technical/data-model/` | Same — avoid duplicating schemas |
| `operations/decisions/` | Outcomes live in changelogs, investigations, and code |

## Technical split

| Folder | Owns |
|--------|------|
| `technical/architecture/` | Estate structure and subsystem responsibilities |
| `technical/subsystems/` | Deep technical topics (Auth0, security, …) |
| `technical/standards/` | Stack-wide conventions (Angular constructs, style, …) |
| `technical/guides/` | Cross-cutting how-tos (e.g. run the whole estate, contributing) |
| `technical/infra/` | Configuration model, CI/CD, secrets rotation, tooling setup |
| App README | That component’s local install / F5 / debug only |

## Capabilities and `capabilities.yaml`

Capabilities are **implementation-free**: describe user-visible behaviour, not Auth0, Angular, Azure Functions, etc.

- **Markdown** = narrative source of truth.
- **`capabilities.yaml`** = thin machine inventory for gap detection during reverse-engineer / upgrades.
- **Do not regenerate** markdown from YAML.

Each inventory entry:

```yaml
- id: auth.login
  title: Log in
  status: active          # active | planned | deprecated
  path: features/auth-login.md
  roles: [public-user, member]
```

Adding a capability = YAML row **and** markdown page (same `id` in frontmatter). Features list applicable **roles**; roles link back to features.

## Frontmatter (light)

```yaml
---
name: Log in
type: feature          # role | feature | system | architecture | guide | …
status: active
updated: YYYY-MM-DD
id: auth.login         # required for capability pages
---
```

Do not maintain heavy `depends_on` / `provides` graphs unless they stay accurate.

## Page templates

### Role (`capabilities/roles/<role>.md`)

```markdown
---
name: Member
type: role
status: active
updated: YYYY-MM-DD
id: member
---

# Role: Member

## Summary
…

## Capabilities
- [Log in](../features/auth-login.md)

## Limitations
…
```

### Feature (`capabilities/features/<feature>.md`)

```markdown
---
name: Log in
type: feature
status: active
updated: YYYY-MM-DD
id: auth.login
roles: [public-user, member]
---

# Feature: Log in

## Description
…

## User roles
…

## Behaviour
…

## Limitations
…
```

### System capability (`capabilities/system/<capability>.md`)

Same shape as features; use for cross-cutting platform behaviour that is not a single user-facing feature page.

### Investigation (`operations/investigations/<topic>.md`)

Summary, symptoms, evidence, root cause, resolution, follow-ups.

### Planned fix (`operations/planned-fixes/<topic>.md`)

Problem, proposed approach, status, links to related investigations/changelogs. Prefer a GitHub issue for tracking; keep this file for in-solution notes.

### Changelog

See [operations/changelogs/README.md](operations/changelogs/README.md). Filenames are `YYYY-MM-DD-NNN-short-slug.md` (`NNN` = `001`, `002`, … for that completion date) so several entries on one day stay ordered in the file list.

## Agent rules

1. Never invent new top-level tracks or undeclared folders.
2. Never put technical stack detail in capability pages.
3. Update frontmatter `updated` when editing.
4. Update indexes when adding or removing documents.
5. When moving files, fix all links and `.cursor/rules` paths.
6. Preserve human-written content unless explicitly asked to delete it.
7. GitHub = tickets; `operations/*` = durable notes next to the code.

See also: [operations/agents/](operations/agents/).

## Non-goals

- Replacing GitHub Projects / Issues
- Duplicating OpenAPI / Swagger as hand-written API docs
- Mirroring every app README into `docs/technical/`
