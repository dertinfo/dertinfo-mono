---
name: Upgrading Angular
type: guide
status: active
updated: 2026-10-04
---

# Upgrading Angular

How to take the website (`apps/dert-web`) up one Angular major. The Ionic app (`apps/dert-app`) is a separate client. This page does not cover that upgrade.

Do one major at a time, on a branch from `main`. The client lives in `apps/dert-web/src/client`. After the official steps, read the watch-list below before treating the hop as done. The existing website smoke suite is the functional check. The hop itself is recorded in a [change log](../../operations/changelogs/README.md).

## Official guidance

Use the Angular sites for the compiler, the supported Node, TypeScript, and RxJS ranges, and the `ng update` steps for the versions you are leaving and joining. Open the update guide on the **advanced** path and set the From and To versions for this hop.

| What you need | Where |
|---------------|--------|
| Step-by-step update for the versions you are moving between | [Angular Update Guide](https://angular.dev/update-guide) |
| When `ng update` is enough, and where the other update pages sit | [Keeping your Angular projects up-to-date](https://angular.dev/update) |
| `ng update` command | [`ng update`](https://angular.dev/cli/update) |
| One major at a time, and what a breaking change means | [Versioning and releases](https://angular.dev/reference/releases) |
| Node, TypeScript, and RxJS for each Angular version | [Version compatibility](https://angular.dev/reference/versions) |
| Material 15 MDC components and the migration schematic | [Migrating to MDC-based Angular Material](https://material.angular.io/guide/mdc-migration) |

`ng update` with no arguments lists what is available. To move one major, pass that major for the CLI and the framework, and take the latest patch of that major:

```bash
ng update @angular/cli@^<major> @angular/core@^<major>
```

Material, CDK, and the other Angular packages follow the same major as `@angular/core`. Third-party packages are aligned only to a release that declares that Angular major.

## Watch-list from the Angular 14 to 15 upgrade

These showed up on this website during the move to Angular 15.2. The update guide does not list them as application steps. Check them again on later hops. The full record of that hop, including package versions, is [Website upgraded from Angular 14 to Angular 15](../../operations/changelogs/2026-10-04-002-web-angular-15.md).

### A package that peers the new Angular may still be the wrong release for this site

Angular 15 still allows RxJS 6, and this site stayed on RxJS 6. That choice fixed several package versions:

- `ngx-quill` 18 and the 20/21 line peer Angular 15 and require RxJS 7. The site stayed on **ngx-quill 17.0.0**, which peers `@angular/core >=13` and `rxjs ^6.5.3 || ^7.4.0`.
- `@swimlane/ngx-datatable` 20.1.0 peers `rxjs ^6.6.3`. RxJS moved from 6.5.5 to **6.6.7**, still on RxJS 6.
- `@auth0/auth0-angular` stays at **2.2.3**. A newer 2.x uses Angular 15-only APIs (`makeEnvironmentProviders`).
- `ng2-charts` 2.4.3 is still a View Engine build (`ngcc` processes it). Its peer range is `@angular/core >=7.2.0`, so it was left in place with `chart.js` 2.

`ng update` refused to start until `--force`, because `ngx-cookie-service` 13 and `ngx-quill` 15 do not peer Angular 15. The schematics were applied with that flag. After the packages were aligned, `npm install` succeeded with no `force` and no peer errors, and `apps/dert-web/src/client/.npmrc` (`force = true`) was removed. On a later hop, use `--force` only to get the schematic through a known peer block, then install cleanly and drop `force` again.

`@angular/flex-layout` **15.0.0-beta.42** is the last release. Angular's own note is [Modern CSS in Angular layouts](https://blog.angular.io/modern-css-in-angular-layouts-4a259dca9127). There is no Angular 16 build, so the next hop replaces it.

### One deep RxJS import was still living on `rxjs-compat`

Nothing imported the `rxjs-compat` package by name. `group-members.component.ts` imported `Observable` from `rxjs/Observable`, which only that package provides. Removing `rxjs-compat` meant changing that import to `rxjs`. Search for `rxjs/` deep imports before deleting a compat package on a later hop.

### The stylesheet resolver follows package `exports`

The only `~` imports were the three ngx-datatable CSS files in `src/assets/styles/styles.scss`. Angular 15 honours the package `exports` field, and those CSS files are not exported, so the build failed. They are now relative paths under `node_modules`. On a later hop, search stylesheets for `~` imports before assuming the production build is clean.

### Reverting the CLI's `tsconfig` edit does not restore the old emit

The Angular 15 schematic set `tsconfig.json` `target` to `ES2022` and added `useDefineForClassFields: false`. Those edits were reverted so the checked-in target stayed `es2020`. The CLI still applies ES2022 at build time and still warns. Browserslist entries that need ES5 (`kaios 2.5`, `op_mini all`) are ignored. `.browserslistrc` and `src/polyfills.ts` were left as they were. If a later schematic rewrites `target`, decide whether the checked-in file should match what the CLI already emits.

### This machine's default Node can be outside the Angular range

Angular 15.2 supports Node `^14.20 || ^16.13 || ^18.10`. The Angular 15 CLI does not run on Node 24. Update and `ng build` used Node **16.20.2**. Node 18's npm writes `lockfileVersion` 3, so installs stayed on Node 16 to keep lockfile version 2. The Static Web Apps CLI on this machine requires Node 18 or newer, so the local proxy on port 44200 ran on Node 24 while `ng serve` used Node 16. CI and the website Docker image stay on Node 16. The engine floor is recorded in [Configuration](../infra/configuration.md). On a later hop, read the compatibility table first and keep the lockfile's npm major on the Node version that wrote it.

### Material MDC replaced the DOM this site's CSS and smoke suite expect

Material 15 both introduces the legacy components and ships `ng generate @angular/material:mdc-migration`. Material 17 removes the legacy entry points and that schematic. This hop ran the schematic in the same upgrade so the site did not stay on `MatLegacy*`. The themes still build with `mat.define-light-theme($primary, $accent)`, and the production build still warns that the theme is not a color, typography, and density map.

What that markup change did here:

- A `placeholder` on `mat-select`, `matInput`, or a textarea inside `mat-form-field` is not a floating label. Those placeholders became `<mat-label>`. An empty `placeholder=""` on the top-bar select was left.
- A resting `mat-label` covers the input, so a Playwright click lands on the label. Date fields and other controls in the smoke suite use `click` and `fill` with `force: true`. The long label "Tickets Available / Registration Open" covered `registrationOpenDate`.
- `placeholder` on `mat-select` is no longer the accessible name. The first smoke failure after the migration was the Target Audience combobox.
- List content is `mdc-list-item__content` and `mat-mdc-list-item-unscoped-content`. The avatar slot is `[matListItemAvatar]` only. `[mat-list-avatar]` stays in the unscoped content, so pictures sat in the wrong place until the registration lists used the new directives. Rules that only targeted `.mat-list-item-content` or `.mat-list-text` matched nothing, which is why the sidebar wrapped and clipped.
- The accordion still finds `mat-list-item` as an element. The old host class is gone.
- A ripple rule that sets `transform: scale(...)` stops Material's fade-out. The sidebar ripple is clipped with `clip-path` only, and only under `.sidebar-panel`.
- `app-addition-header` styles are encapsulated. Copying the same markup into a page template does not pick up that padding. The card is a column flex container, so `align-items: center` centers the title sideways. The title's text includes the icon name, so an exact text match for "Invoices & Payments" misses the header. The smoke suite waits on `app-addition-header` filtered by that text.
- A section header inside `fxLayout="row"` shrinks and wraps. The header sits above the row.
- The dialog overlay pane is `cdk-overlay-pane`. It does not carry `mat-mdc-dialog-panel`. Width for `.modal-min` is set with `:has(.modal-min)` on that pane.
- `data-testid="public-history"` collapsed to height 0 when `showcase/events` returned an empty list. Playwright treats height 0 as hidden. A static "History" heading inside that container keeps the page visible on a clean database.

Leave Material's own CSS in `node_modules` alone. Shared layout belongs in `styles.scss` and the shared header components. Working notes from the layout pass are under `docs/operations/visual-iterations/`.

### The next hop is already constrained by what this one left in place

The list written at the end of the 14 to 15 hop said Angular 16 required RxJS 7, that flex-layout had to be replaced, and that `ng2-charts` 2 was still View Engine. Checked against the [version compatibility](https://angular.dev/reference/versions) table, Angular 16.2 still allows RxJS `^6.5.3 || ^7.4.0` and Node `^16.14.0 || ^18.10.0`. The RxJS line was wrong. Flex-layout and the View Engine chart were real stops, and they were cleared before `ng update`. What the 15 to 16 hop left is in the watch-list below.

## Watch-list from the Angular 15 to 16 upgrade

These showed up on this website during the move to Angular 16.2. The update guide does not list them as application steps. Check them again on later hops. The full record, including package versions, is [Website upgraded from Angular 15 to Angular 16](../../operations/changelogs/2026-10-04-003-web-angular-16.md).

### Read the compatibility table before treating a leftover as a requirement

The 14 to 15 notes said Angular 16 requires RxJS 7. The table does not. This site stayed on RxJS **6.6.7** and `ngx-quill` **17.0.0**. Later `ngx-quill` releases are what want RxJS 7. On the next hop, re-read the table for that major. Do not copy a constraint forward because the previous notes said so.

### `ng update` can succeed while the next install still fails

`ng update @angular/cli@16 @angular/core@16` and `ng update @angular/material@16` completed with no `--force`. A later `npm install` then failed because `ng2-file-upload` 4 peers `@angular/common` `^15` and `ngx-cookie-service` 15 peers `^15`. Those moved to **5.0.0** and **16.1.0**. After that, `npm install` had no peer errors and no `force`. Packages whose peers are an open `>=` range (`ngx-quill` 17.0.0, `@ngx-translate/core` 14, `@swimlane/ngx-datatable` 20.1.0, `@auth0/auth0-angular` 2.2.3) did not need a bump.

`@ngbracket/ngx-layout` and `ng2-charts` are not carried by `ng update`. They moved to **16.1.3** and **5.0.4** in the same hop. `ng2-charts` 5.0.0 through 5.0.2 published a schematics peer that does not resolve; **5.0.4** installs cleanly and still allows RxJS 6. The layout fork's RxJS 7 range is `^7.8.0`, which is narrower than Angular's `^7.4.0`. RxJS 6.6.7 satisfies the fork's `^6.5.3` side.

### Angular 16 removes `ngcc`, so the chart moved first

`ng2-charts` 2.4.3 is View Engine. It was replaced while the site was still on Angular 15, with `ng2-charts` 4.1.1 and Chart.js 4, then bumped to `ng2-charts` 5.0.4 with Angular. The live chart is the judge range report. Chart.js 3+ uses `x` / `y` scales, `title.text`, and `grid`, and the canvas input is `[type]`. The dashboard chart is still inside an HTML comment and still uses the old `[chartType]` inputs. Uncommenting it needs the same options shape.

### The guard schematic removes `implements`, not the methods

The core migration removed deprecated guard and resolver `implements` clauses from 31 files. `canActivate` and `resolve` stayed. Nothing in the app used `ReflectiveInjector`, `BrowserTransferStateModule`, `ComponentFactoryResolver`, or `router.navigate([], { relativeTo })`. `entryComponents` is still set on `AppLoaderModule`. The production build accepts it.

### This schematic did not rewrite `tsconfig` target

The Angular 15 schematic had set `target` to `ES2022`. That edit was reverted, and the checked-in target is still `es2020`. The Angular 16 schematic left it there. The CLI still applies ES2022 at build time and still warns. Browserslist entries that need ES5 (`kaios 2.5`, `op_mini all`) are still ignored.

### This machine's default Node is still outside the Angular range

Angular 16.2 supports Node `^16.14.0 || ^18.10.0`. Update and `ng build` used Node **16.20.2**, so the lockfile stayed `lockfileVersion` 2. The Angular 16 CLI does not run on Node 24. The Static Web Apps CLI on this machine requires Node 18 or newer, so the local proxy on port 44200 ran on Node 24 while `ng serve` used Node 16. CI and the website Docker image stay on Node 16. The global CLI pin in the website Dockerfile is `@angular/cli@16`.

### The next hop is already constrained by what this one left in place

Checked against the compatibility table for Angular 17, not copied from the 14 to 15 list.

- Angular 17 requires Node `^18.13.0 || ^20.9.0`. Node 16 remains valid for this website on Angular 16. CI (`web-src-ci.yml`, `web-src-cd.yml`) and the website image are Node 16. Installing on Node 18 can rewrite the lockfile npm major. Read the npm version that ships with the Node you choose before changing it.
- Angular 17.0 needs TypeScript `>=5.2.0`. This site is on **5.1.6**, which is inside the Angular 16.2 range (`>=4.9.3 <5.2`). Let `ng update` move it.
- Angular 17 still allows RxJS `^6.5.3 || ^7.4.0`. RxJS stays at **6.6.7** unless a package we bump refuses it. `ngx-quill` **17.0.0** is the release that still allows RxJS 6. Later `ngx-quill` releases require RxJS 7.
- `@ngbracket/ngx-layout`, `ng2-charts`, `ng2-file-upload`, and `ngx-cookie-service` must move with the Angular major. They are not carried by `ng update`. Confirm the peer range of the release you take. `ng2-charts` 5.0.0–5.0.2 did not install cleanly.
- The Material theme is still `define-light-theme($primary, $accent)`. The production build still warns that it is not a color, typography, and density map.
- Competition admin, Dert of Derts, notifications, and system admin were not part of the layout pass.
- Standalone components and typed forms were not adopted. `ng generate @angular/core:standalone` ships with Angular 16 and was not run.
- `entryComponents` remains on `AppLoaderModule`.
- The commented dashboard chart still uses the Chart.js 2 inputs.
- These packages are still referenced and were left in place: `angular-csv-ext`, `hopscotch`, `perfect-scrollbar`, `moment`, `tslint`, `codelyzer`, `protractor` (`dertinfo-e2e`). TSLint-to-ESLint and dropping Protractor are separate work.

## Related

- [Angular standards](../standards/angular/README.md)
- [Configuration](../infra/configuration.md)
- [Website smoke tests](website-smoke-tests.md)
- [Website upgraded from Angular 14 to Angular 15](../../operations/changelogs/2026-10-04-002-web-angular-15.md)
- [Website upgraded from Angular 15 to Angular 16](../../operations/changelogs/2026-10-04-003-web-angular-16.md)
