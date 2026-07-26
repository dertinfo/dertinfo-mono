---
name: Agent documentation guidance
type: agents
status: active
updated: 2026-07-26
---

# Agents

Guidance for AI agents working in this monorepo’s documentation library.

## Maintain the library

1. Read [Documentation guide](../../documentation-guide.md) before adding or moving docs.
2. **Never invent folders** — only use folders defined in the guide; create a folder when adding its first real page.
3. Capabilities stay **implementation-free** (no stack names). Technical detail belongs under `docs/technical/`.
4. When adding a capability: update `capabilities/capabilities.yaml` **and** the markdown page (same `id`). Do not regenerate markdown from YAML.
5. Update light frontmatter (`status`, `updated`) when editing a page.
6. Update indexes (`docs/README.md`, track READMEs, changelog index) when adding or removing pages.
7. When moving docs, update all in-repo links and [`.cursor/rules`](../../../.cursor/rules/).

## Operations vs GitHub

| Place | Use for |
|-------|---------|
| GitHub Issues / Projects | Tickets, prioritisation, assignment |
| `operations/planned-fixes/` | Working notes while implementing in this repo |
| `operations/investigations/` | Finished deep-dives and historical write-ups |
| `operations/changelogs/` | Completed workstreams (required when a stream finishes) |

## Cursor-specific behaviour

Short agent rules live under [`.cursor/rules/`](../../../.cursor/rules/). Prefer pointing those rules at canonical docs under `docs/` rather than duplicating long prose.

## Durable agent plans

If a multi-step agent plan should survive beyond a chat, add a page under this folder later. Do not create empty stubs.
