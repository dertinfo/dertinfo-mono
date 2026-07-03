# GitHub MCP setup (Cursor)

Lets the Cursor agent read **pull requests** and **GitHub Actions** workflow runs for this repository via the [official GitHub MCP server](https://github.com/github/github-mcp-server).

## 1. Create a fine-grained PAT (this repo only)

1. Open [GitHub → Settings → Developer settings → Fine-grained tokens](https://github.com/settings/personal-access-tokens/new).
2. **Token name:** e.g. `cursor-mcp-dertinfo-mono`
3. **Expiration:** choose a sensible expiry (90 days or custom).
4. **Resource owner:** `dertinfo` (or your user if the repo is under your account).
5. **Repository access:** **Only select repositories** → choose **`dertinfo-mono`** only.
6. **Repository permissions:**

   | Permission        | Access   | Why |
   |-------------------|----------|-----|
   | **Actions**       | Read-only | Workflow runs, jobs, logs |
   | **Contents**      | Read-only | Read workflow YAML and repo files |
   | **Metadata**      | Read-only | Required by GitHub |
   | **Pull requests** | Read-only | List/review PRs |

   Leave all other permissions at **No access** unless you explicitly want the agent to open PRs or change issues (then grant **Pull requests: Read and write** and remove read-only mode in MCP config — see below).

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
| `pull_requests`  | PR list, diff, checks |
| `actions`        | Workflow runs and job logs |

`X-MCP-Readonly: true` prevents the agent from creating or modifying issues, PRs, or repo content. Remove that header if you want write access.

## 3. Restart Cursor

1. Fully quit and reopen Cursor (MCP loads at startup).
2. Open **Settings → Tools & Integrations → MCP**.
3. Confirm **github** shows a green connected status.

## 4. Verify

In Agent chat, ask for example:

> List the latest failed GitHub Actions runs for `dertinfo/dertinfo-mono` on `main`.

Or:

> Show open pull requests on `dertinfo/dertinfo-mono`.

If tools return live data, setup is complete.

## Troubleshooting

| Symptom | Fix |
|---------|-----|
| MCP server red / not loading | Check JSON syntax in `.cursor/mcp.json`; restart Cursor |
| `Bad credentials` / empty results | PAT wrong or expired; regenerate |
| `403` / resource not accessible | Token missing **Actions** or **Pull requests** read; or org SSO not authorized |
| Actions tools missing | Ensure `X-MCP-Toolsets` includes `actions` |
| Too many tools in Cursor | Keep toolsets minimal (as in the example) |

## Security

- **Never commit** `.cursor/mcp.json` — it is listed in `.gitignore`.
- Only commit `.cursor/mcp.json.example` (placeholder token).
- Revoke the token at [GitHub token settings](https://github.com/settings/tokens?type=beta) when no longer needed.

## Reference

- Official install guide: [github-mcp-server — Cursor](https://github.com/github/github-mcp-server/blob/main/docs/installation-guides/install-cursor.md)
- Remote toolsets: [remote-server.md](https://github.com/github/github-mcp-server/blob/main/docs/remote-server.md)
