# GitHub MCP setup (Cursor)

Lets the Cursor agent read **GitHub Actions** runs, **pull requests**, and **create PRs** for this repository via the [official GitHub MCP server](https://github.com/github/github-mcp-server).

## 1. Create a fine-grained PAT (this repo only)

1. Open [GitHub → Settings → Developer settings → Fine-grained tokens](https://github.com/settings/personal-access-tokens/new).
2. **Token name:** e.g. `cursor-mcp-dertinfo-mono`
3. **Expiration:** choose a sensible expiry (90 days or custom).
4. **Resource owner:** `dertinfo` (or your user if the repo is under your account).
5. **Repository access:** **Only select repositories** → choose **`dertinfo-mono`** only.
6. **Repository permissions:**

   | Permission        | Access        | Why |
   |-------------------|---------------|-----|
   | **Actions**       | Read-only     | Workflow runs, jobs, logs |
   | **Contents**      | Read-only     | Read workflow YAML and repo files |
   | **Metadata**      | Read-only     | Required by GitHub |
   | **Pull requests** | Read and write | List/review PRs and **create PRs** |

   Leave all other permissions at **No access** unless you explicitly want the agent to push commits or edit issues.

7. Click **Generate token** and copy the token (`github_pat_...`). You will not see it again.

If the org uses SSO, **authorize** the token for the `dertinfo` organization after creation.

## 2. Configure Cursor MCP

**Recommended:** project-local config (gitignored; token never committed).

```powershell
cd C:\Projects\Cursor\DertInfo
copy .cursor\mcp.json.example .cursor\mcp.json
```

Edit `.cursor/mcp.json` and replace `YOUR_GITHUB_PAT` with your token:

```json
"Authorization": "Bearer github_pat_xxxxxxxx"
```

**Alternative:** global config at `%USERPROFILE%\.cursor\mcp.json` (same JSON shape) if you want GitHub MCP in every project.

### Toolsets enabled

The example config enables:

| Toolset          | Use |
|------------------|-----|
| `context`        | Current user / org context |
| `repos`          | Repository and file access |
| `pull_requests`  | PR list, diff, checks, **create PR** |
| `actions`        | Workflow runs and job logs |

**Write access:** do **not** set `X-MCP-Readonly`. Omitting it allows `create_pull_request` and related write tools. To lock the agent to read-only triage only, add `"X-MCP-Readonly": "true"` (PR creation will not work).

## 3. Restart Cursor and confirm MCP

1. Fully quit and reopen Cursor (MCP loads at startup).
2. Open **Customize** in the sidebar → **MCPs** (Cursor’s MCP UI; older docs said Settings → Tools & Integrations → MCP).
3. Confirm **github** shows connected / enabled (toggle on if needed).
4. Confirm write tools are available to the agent (e.g. `create_pull_request`).

If the server needs interactive auth, use **Connect** / **Authenticate** on that entry under **Customize → MCPs**, then finish the browser prompt. Agent `mcp_auth` prompts time out if you do not complete them promptly — prefer the Customize UI.

Optional: Output panel (`Ctrl+Shift+U`) → **MCP Logs** for connection errors.

## 4. Verify

**Read access:**

> List the latest failed GitHub Actions runs for `dertinfo/dertinfo-mono` on `main`.

**Write access (after PR permission on PAT and no read-only header):**

> Create a pull request from `feature/fix-cd-pipelines` to `main` on `dertinfo/dertinfo-mono`.

## Troubleshooting

| Symptom | Fix |
|---------|-----|
| MCP server red / not loading | Check JSON syntax in `.cursor/mcp.json`; restart Cursor; check **Customize → MCPs** and **MCP Logs** |
| Cannot find MCP in Settings | Use **Customize → MCPs** in the sidebar ([Cursor MCP help](https://cursor.com/help/customization/mcp)) |
| `Bad credentials` / empty results | PAT wrong or expired; regenerate |
| `403` / resource not accessible | Token missing **Actions** or **Pull requests** scope; or org SSO not authorized |
| `create_pull_request` tool missing | Remove `X-MCP-Readonly`; restart Cursor |
| PR create fails with 403 | PAT needs **Pull requests: Read and write** |
| Actions tools missing | Ensure `X-MCP-Toolsets` includes `actions` |
| Auth / `mcp_auth` times out | Complete Connect under **Customize → MCPs**; restart Cursor after success |
| Too many tools in Cursor | Keep toolsets minimal (as in the example) |

## Security

- **Never commit** `.cursor/mcp.json` — it is listed in `.gitignore`.
- Only commit `.cursor/mcp.json.example` (placeholder token).
- Revoke the token at [GitHub token settings](https://github.com/settings/tokens?type=beta) when no longer needed.

## Reference

- Cursor: [MCP integrations (Customize → MCPs)](https://cursor.com/help/customization/mcp) · [MCP reference](https://cursor.com/docs/mcp)
- Official install guide: [github-mcp-server — Cursor](https://github.com/github/github-mcp-server/blob/main/docs/installation-guides/install-cursor.md)
- Remote toolsets: [remote-server.md](https://github.com/github/github-mcp-server/blob/main/docs/remote-server.md)
