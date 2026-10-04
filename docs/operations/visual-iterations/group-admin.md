# Group admin visual pass

Working notes for the Material 15 MDC look on the group admin area. Fix one screen here, then reuse the same rules elsewhere.

Reference page: group settings (`/group/The%20Simpsons/1/settings`), with the main sidebar in view.

## 2026-10-04 — sidebar labels and click circle

Scoped to the main sidebar (`.sidebar-panel`). Not rolled out to other lists yet.

1. **Labels sit on the same line as the icons.** The menu rules targeted `.mat-nav-list`, which Material 15 no longer puts on `mat-nav-list`. The link layout never applied, so each label was a 48px block with a 24px line and the words sat high. Selectors now use the `mat-nav-list` element. The label is a centered flex row, and the icon is a centered 24px glyph in the 48px slot.
2. **Click circle is about a quarter of the size, and it clears when the button is released.** The list ripple was drawn large enough to cover the whole row (about 220–280px). Scaling it with `transform` stopped Material’s fade-out, so the circle stayed on the row. It is now clipped with `circle(12.5% at 50% 50%)`, which leaves the animation free to finish.

Still open on this page: anything in the group settings form and the in-page menu (Overview, Settings, Teams, and the rest) that we have not walked yet.
