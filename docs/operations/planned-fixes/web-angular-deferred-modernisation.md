---
name: Deferred Angular modernisation
type: planned-fix
status: active
updated: 2026-10-05
---

# Deferred Angular modernisation (website, 14 to 22)

**Status:** Noted — not scheduled. Pick these up as separate pieces of work. Each one is large enough to fail the smoke suite on its own.

**Related:** [Upgrading Angular](../../technical/guides/upgrading-angular.md), [Website smoke tests](../../technical/guides/website-smoke-tests.md). The hop records are the change logs linked from that guide, Angular 14 through 22.

The Ionic app (`apps/dert-app`) stayed on Angular 13. This page is the website only.

## Why these were left

Each hop moved one Angular major, then stopped at the smallest change that still installed, built, and passed the existing smoke suite. Optional migrations were declined. When a required migration would have changed behaviour, the hop kept the old behaviour: `standalone: false`, `provideZoneChangeDetection()`, `ChangeDetectionStrategy.Eager`, `withXhr()`, and `paramsInheritanceStrategy: 'emptyOnly'`. Packages whose peers are an open `>=` range were left alone.

That rule got the site onto the current Angular. It also left the modernisations Angular now recommends. They are listed below in an order that keeps later steps from fighting earlier ones.

Two things that look like skips are already done, or have nothing to convert:

- Material's MDC components were adopted on the 14 to 15 hop. The legacy `MatLegacy*` components are gone. What remains is the Material 2 theme, not the component set.
- Website Karma, Jasmine, and Protractor were removed on the 16 to 17 hop. The Playwright smoke suite is the test. Angular 22 offers `migrate-karma-to-vitest`. There is no Karma project on the website for it to move. `Router.getCurrentNavigation()` is not used, so the `router-current-navigation` migration has nothing to rewrite.

## 1. Strict TypeScript and strict templates

TypeScript 6 defaults `strict` and `esModuleInterop` to true. The 21 to 22 hop set both to `false` in [apps/dert-web/src/client/tsconfig.json](../../../apps/dert-web/src/client/tsconfig.json), and set `ignoreDeprecations` to `"6.0"`. The first production build reported about a thousand errors under the new defaults (`noImplicitAny`, `strictNullChecks`, `strictPropertyInitialization`, and `import * as moment` no longer being callable). The site had been compiling without `strict` since Angular 14.

The same hop's migration wrote `strictTemplates: false` in `src/tsconfig.app.json`. It also wrote an `extendedDiagnostics` suppress for the optional-chaining checks. Those two settings together fail the build (`NG4003`), so the suppress was removed and `strictTemplates` stayed false. The project had never turned strict templates on. The workspace standard asks for them; the hops did not adopt them because a typing pass is separate from a version bump.

Turn `strict` on, then `strictTemplates`. Fix the errors in that order. `esModuleInterop: true` can land with `strict`, and the `import * as moment` call sites need a default import when it does.

`baseUrl` and `downlevelIteration` are deprecated and stop working in TypeScript 7. `ignoreDeprecations: "6.0"` is what keeps today's `tsconfig` legal. Remove those two options, and then remove `ignoreDeprecations`, before a TypeScript 7 compiler is required.

The checked-in `target` is still `es2020`. The CLI has applied ES2022 at build time since Angular 15 and still warns. Setting `target` to `ES2022` removes that warning. It does not change what the CLI already emits.

## 2. Control flow (`*ngIf`, `*ngFor`, `*ngSwitch`)

Angular 17 introduced `@if`, `@for`, and `@switch`. Angular 20 deprecated the structural directives. On the 20 hop the control-flow migration was optional and was not run. On the 21 hop it was required: it rewrote 111 templates, and those edits were restored so the directives stayed. On the 22 hop the migration was not offered again. About 100 templates still use `*ngIf`.

They were restored because the rewrite was a large diff with no behaviour change the smoke suite needed, and the directives still compile. They are deprecated. Run the control-flow migration and keep the result. Re-run the smoke suite. `@for` needs a `track` expression; that is the part most likely to change rendering.

## 3. Optional chaining wrappers

Angular 22 aligns template `?.` with TypeScript: a null receiver now produces `undefined`, where templates used to produce `null`. The migration wrapped those expressions in `$safeNavigationMigration()` in nine templates so the old null result stays. The wrappers were kept so pipes and `*ngIf` checks did not change during the hop.

After control flow and strict templates, read each wrapper. Remove it where `undefined` and `null` are the same for that binding. Where a pipe or comparison treats them differently, keep an explicit null check and then drop the wrapper.

## 4. Application builder

From Angular 18 the CLI offers `use-application-builder` and does not run it unless asked. The webpack `browser` builder stayed. Angular 22 deprecates Webpack, `@angular-devkit/build-angular:browser`, and `@ngtools/webpack`. The production build prints that deprecation. The builder still runs.

It was left because the migration is optional, the webpack build is what CI and the smoke suite already use, and a new builder changes bundling. Run `ng update @angular/cli --name use-application-builder` on its own, then `npm run build:hosted` and the smoke suite. Budgets, scripts, and styles in `angular.json` are the usual places it disagrees with the webpack build.

## 5. Material 3 theme

The 14 to 15 hop moved components onto MDC and kept `mat.define-light-theme($primary, $accent)`. Angular 18 renamed those helpers to `m2-define-light-theme` and `m2-define-dark-theme`. Angular 19 replaced `mat.core()` with `mat.elevation-classes()` and `mat.app-background()`. The five theme files still use those Material 2 helpers. The production build warns that a theme should be a map of color, typography, and density.

Material 3 was not adopted because it is a visual change across every screen, and the hops were trying to keep the look the smoke suite already knew. The MDC DOM work was the part that had to happen before Material 17 removed the legacy components.

Move the five themes under `src/assets/styles/themes/` to the Material 3 `color` / `typography` / `density` map. Re-check shared chrome first (top bar, sidenav, dialogs, steppers, form fields). Competition admin, Dert of Derts, notifications, and system admin were not part of the Material 15 layout pass, so include them in this visual check. The smoke suite covers event setup, group setup, registration, and public pages. It does not cover those four areas.

## 6. Replace `@ngbracket/ngx-layout` with CSS

`@angular/flex-layout` stopped at 15.0.0-beta.42. Angular's note is [Modern CSS in Angular layouts](https://blog.angular.io/modern-css-in-angular-layouts-4a259dca9127). The 15 to 16 hop replaced it with `@ngbracket/ngx-layout` **16.1.3** before `ng update`, because there was no Angular 16 build and the templates use `fxLayout`, `fxFlex`, and `fxLayoutAlign` throughout. Later hops left 16.1.3 in place because its peer is `@angular/core >=16` and the build accepted it. A newer major of the fork exists. Angular's recommendation is to stop using the layout library.

Replace the `fx*` attributes with CSS. Do this as its own pass, with the smoke suite, and do it before or with the Material 3 visual pass so layout is not restyled twice.

## 7. Standalone components, and `provideAppInitializer`

Angular 19 defaults `standalone` to true. The required migration added `standalone: false` on the NgModule components, directives, and pipes (225 files) so the modules kept working. `ng generate @angular/core:standalone` was not run. The flags were kept because converting the module graph is a separate change from making the new default explicit.

Angular 19 also offers `provide-initializer`, which replaces `APP_INITIALIZER` with `provideAppInitializer`. It was optional and was not run. `AppModule` still registers `APP_INITIALIZER` for client settings, Application Insights, and the Auth0 client config. It was left because that multi-provider still runs.

Convert to standalone after the template and builder work, then replace `APP_INITIALIZER`. The Auth0 setup in `app.module.ts` is the piece to re-test by hand as well as with the smoke suite.

## 8. HttpClient on Fetch

Angular 22 uses the Fetch backend unless the app opts into XHR. The migration added `withXhr()` next to `withInterceptorsFromDi()` in `app.module.ts`. It was kept because Fetch is a behaviour change for upload progress and for any interceptor that assumes `XMLHttpRequest`. The Auth0 interceptor is registered through `HTTP_INTERCEPTORS`.

Drop `withXhr()` in a dedicated change. Sign-in, authenticated API calls, and file upload are the flows to re-test. `ng2-file-upload` uses its own XHR, so upload progress on that directive is a separate check from `HttpClient`.

## 9. Route parameter inheritance

Angular 22 defaults `paramsInheritanceStrategy` to `'always'`, so a child route inherits parent params and data. There is no migration. `RouterModule.forRoot` in `app.module.ts` sets `'emptyOnly'`, which is the previous behaviour: inherit only when the child route has no params of its own.

The opt-out was added so child pages did not start seeing parent ids during the version bump. Remove `'emptyOnly'` only after checking pages that read `ActivatedRoute` params or data, then run the registration and admin flows in the smoke suite.

## 10. OnPush, then zoneless

Angular 20 makes `zone.js` optional. Angular 21 does not schedule Zone change detection unless `provideZoneChangeDetection()` is provided. The 21 hop added that call in `main.ts` and left the `zone.js` import in `src/polyfills.ts`. Angular 22 defaults a component with no strategy to `OnPush`, and renames the old `Default` strategy to `Eager`. The migration added `ChangeDetectionStrategy.Eager` on 208 components. One component already set `OnPush` and was left on `OnPush`.

`Eager` and `provideZoneChangeDetection()` were kept so the UI still updates on the same asynchronous work as before. Removing them is the move toward the current defaults.

Remove `Eager` component by component, starting where the template already uses signals or `async` pipe, until the default `OnPush` is enough. Zoneless comes after that, by removing `provideZoneChangeDetection()` and the `zone.js` polyfill. The smoke suite is the check for missed updates.

## 11. Typed forms, then signal forms

Typed forms have been available since Angular 14. Signal forms are stable in Angular 22. Neither was adopted. Existing forms are untyped `FormControl` and `FormGroup`, and they still work. A form migration was out of scope for a version bump.

Type the existing reactive forms after `strict` is on. Signal forms are a later rewrite of those forms, not a requirement for staying on Angular 22.

## 12. Browserslist

`.browserslistrc` was left unchanged on every hop. The CLI ignores the ES5 entries (`kaios 2.5`, `op_mini all`). From Angular 20 it also warns that `and_qq 14.9`, `and_uc 15.5`, `android 154`, `chrome 109`, `kaios 3.0-3.1`, `op_mob 80`, `opera 135`, `opera 134`, `samsung 30`, and `samsung 29` fall outside the supported browser set. `chrome 109` joined that list on the 20 to 21 hop.

The file was left because changing it changes the build output, and the hops only needed a successful build. Trim it to Angular's current browser set when the application builder work lands, so the warning and the ES2022 target tell the same story.

## 13. Small cleanups

These compile today. They were left because removing them does not change runtime behaviour.

- `entryComponents` is still set on `AppLoaderModule`. Ivy does not use it. The build accepts it.
- The dashboard chart in `dashboard.template.html` is inside an HTML comment and still uses the Chart.js 2 input `[chartType]`. The live chart is the judge range report, which already uses `ng2-charts` 5 and Chart.js 4. Uncommenting the dashboard chart means rewriting those options to the Chart.js 4 shape (`x` / `y` scales, `title.text`, `grid`, and `[type]`).

## 14. Packages kept because the peer range was already wide enough

`ng update` does not carry these. A newer major was not taken when the installed release still installed and compiled.

| Package | Still at | Why it stayed |
|---------|----------|----------------|
| `rxjs` | 6.6.7 | Angular 22 still allows `^6.5.3 \|\| ^7.4.0`. A newer `ngx-quill` is what would force RxJS 7. |
| `ngx-quill` | 17.0.0 | Later releases peer Angular and require RxJS 7. Quill stays on 1.3.x with it. |
| `ng2-charts` | 5.0.4 | Peers `@angular/core >=16`. Later majors exist. The live chart already uses Chart.js 4. |
| `@ngx-translate/core` | 14.0.0 | Open peer range. `http-loader` stays at 7.0.0 with it. |
| `@swimlane/ngx-datatable` | 20.1.0 | Open peer range. Its CSS is imported by relative path under `node_modules` because the package does not export those files. |
| `@auth0/auth0-angular` | 2.2.3 | Left on 2.2.3 from the 14 to 15 hop, when a newer 2.x called `makeEnvironmentProviders`. Re-read the current 2.x release notes before moving it. The smoke suite's sign-in flow is the check. |
| `zone.js` | 0.15.1 | Angular 22 accepts `~0.15.0` or `~0.16.0`. Leave the version until the zoneless work in section 10. |

`angular-csv-ext`, `hopscotch`, `perfect-scrollbar`, and `moment` are still referenced. They were left because nothing in the Angular update required them to move. `moment` is the one the `esModuleInterop` change will touch.

`tslint` and `codelyzer` are still dev dependencies. `codelyzer` peers Angular below 13. TSLint-to-ESLint was called out as separate work on every hop from 15 onward. `ng lint` is not a useful gate until that replacement exists. Website CI runs it with `continue-on-error`.
