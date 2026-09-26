---
name: PowerShell standards
type: standards
status: active
updated: 2026-09-26
---

# PowerShell standards

House conventions for operator scripts under [`infra/scripts/`](../../../../infra/scripts/). Follow these when you add or change a script there.

**Primary references:** [`New-DertInfoSqlEntraGroups.ps1`](../../../../infra/scripts/Entra/New-DertInfoSqlEntraGroups.ps1) and [`New-DertInfoSubscriptionOidcIdentities.ps1`](../../../../infra/scripts/Entra/New-DertInfoSubscriptionOidcIdentities.ps1). How to run them: [`infra/scripts/README.md`](../../../../infra/scripts/README.md).

## Where scripts live

[`Start-DertInfoControlPlane.ps1`](../../../../infra/scripts/Start-DertInfoControlPlane.ps1) stays at the root of `infra/scripts`. Every other runnable script lives in a category folder named for the resource it changes (`Configuration`, `Database`, `Entra`, `Subscription`). A script left at the root is not listed in the menu until it is filed in a category.

Files whose names start with `Shared-` are libraries. Other scripts dot-source them. They are not run on their own, and the control plane does not list them.

The menu labels each option `Category > Script.ps1`.

## Header

Comment-based help, in this order:

1. `.SYNOPSIS` — one line.
2. `.DESCRIPTION` — what the script does, then one user-story sentence: `As a <actor> I run this script to <achieve> because I want <outcome>`.
3. `.PARAMETER` — one block per parameter.
4. `.EXAMPLE` — at least one. Paths are from `infra/scripts` (for example `.\Database\Copy-DertInfoSqlToDevelopment.ps1`).

A `Shared-*` file uses the same header. Its user-story sentence says **load** rather than **run**, because it is not started on its own.

## Body

After `[CmdletBinding()]`, `param()`, and `$ErrorActionPreference = 'Stop'`:

1. **Dot-source** a `Shared-*` file here when the script needs it.
2. **Functions** — helpers such as `Assert-AzCli` and `Invoke-Az`.
3. **Variable setup** — resolve defaults, load the catalog, compute names.
4. **Work** — `Write-Host` says what is happening as it happens (creating, reusing, skipping, assigning). Do not print secret values.
5. **Close** — `Write-Host` prints the variables that were used and the outcome: what was created or changed, and what the operator does next.

The control plane is the menu. It keeps the header, and its body is the menu rather than this operator-script sequence. `Shared-*` files stop after the functions; they do not perform the work themselves.
