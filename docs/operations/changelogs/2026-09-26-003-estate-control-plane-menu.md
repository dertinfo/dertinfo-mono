# Estate control plane menu

## Summary of the work completed

[`Start-DertInfoControlPlane.ps1`](../../../infra/scripts/Start-DertInfoControlPlane.ps1) is the entry point for the operator scripts in [`infra/scripts`](../../../infra/scripts/). It lists every sibling script whose comment-based help has a `.SYNOPSIS`, shows that help, collects parameters from the script `param()` block, and runs the script only after confirmation. Optional values left blank are omitted from the call. Yes and No prompts accept `1`/`2` and `Y`/`N`.

[`DertInfoAppConfigCatalog.ps1`](../../../infra/scripts/DertInfoAppConfigCatalog.ps1) now has the same style of comment-based help, so the menu includes it. Its functions are unchanged. How to start: [`infra/scripts/README.md`](../../../infra/scripts/README.md).

## Why the work was completed

The operator scripts were already documented in their own comment-based help, but the only index was the scripts README. A single menu can present that help, take the parameters each script already declares, and start the one script the operator confirms, without a separate metadata file and without replacing the scripts.

## Date the work was started

2026-09-26

## Date the work was completed

2026-09-26

## Issues that were encountered on the way

- The PowerShell language parser returns `.PARAMETER` names in uppercase. The menu matches them to `param()` names without regard to case, so the help text still lines up with `GitHubEnvironment`, `Force`, and the rest.
- Yes and No were numbers only, so typing `y` was rejected. Those prompts now accept `Y` and `N` as well as `1` and `2`.

## References to any best practices that we found

- [about_Comment_Based_Help](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_comment_based_help) — `.SYNOPSIS`, `.DESCRIPTION`, `.PARAMETER`, and `.EXAMPLE` are the help the menu reads. Script `param()` attributes (`Mandatory`, `ValidateSet`, `[switch]`) supply the rest.

## Any remaining issues that we may wish to address

- None for this menu. Pattern checks such as subscription and client id shapes stay on the scripts. A value the script rejects is shown, and the menu then asks whether to return home.
