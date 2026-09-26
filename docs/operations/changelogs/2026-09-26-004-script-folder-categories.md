# Script folder categories

## Summary of the work completed

Operator scripts under [`infra/scripts`](../../../infra/scripts/) now live in a category folder named for the resource they change: `Configuration`, `Database`, `Entra`, and `Subscription`. [`Start-DertInfoControlPlane.ps1`](../../../infra/scripts/Start-DertInfoControlPlane.ps1) stays at the scripts root. The menu labels each option `Category > Script.ps1` and skips files whose names start with `Shared-`.

[`DertInfoAppConfigCatalog.ps1`](../../../infra/scripts/Configuration/Shared-DertInfoAppConfigCatalog.ps1) is renamed [`Shared-DertInfoAppConfigCatalog.ps1`](../../../infra/scripts/Configuration/Shared-DertInfoAppConfigCatalog.ps1). Each script’s comment-based help keeps its existing description and adds a user-story sentence. House style is recorded in [PowerShell standards](../../technical/standards/powershell/). How to start: [`infra/scripts/README.md`](../../../infra/scripts/README.md).

## Why the work was completed

The scripts folder was a single list. As more scripts are added, the name alone does not say which resource a script changes, or whether it is meant to be run. Categories make the menu show what is available. The `Shared-` prefix marks the catalog helper as a library the other configuration scripts load, not a script an operator starts.

## Date the work was started

2026-09-26

## Date the work was completed

2026-09-26

## Issues that were encountered on the way

- Moving a script into a category folder adds one directory between the file and `infra`. Catalog and export paths that treated the parent of the script folder as `infra` now climb one more level. Entra scripts that default `RepoRoot` to two levels above the file now use three.

## References to any best practices that we found

- [about_Comment_Based_Help](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_comment_based_help) — `.SYNOPSIS`, `.DESCRIPTION`, `.PARAMETER`, and `.EXAMPLE` are the help the menu reads.
- [PowerShell standards](../../technical/standards/powershell/) — header order and the operator-script body (functions, variable setup, progress, closing summary).

## Any remaining issues that we may wish to address

- Historical changelog pages still link to the script paths from the day that work was written. Living docs use the category folders.
