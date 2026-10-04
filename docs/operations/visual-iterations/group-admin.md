# Group admin visual pass

Working notes for the Material 15 MDC look on the group admin area. Fix one screen here, then reuse the same rules elsewhere.

Reference page: group settings (`/group/The%20Simpsons/1/settings`), with the main sidebar in view.

## 2026-10-04 — sidebar labels and click circle

Scoped to the main sidebar (`.sidebar-panel`). Not rolled out to other lists yet.

1. **Labels sit on the same line as the icons.** The menu rules targeted `.mat-nav-list`, which Material 15 no longer puts on `mat-nav-list`. The link layout never applied, so each label was a 48px block with a 24px line and the words sat high. Selectors now use the `mat-nav-list` element. The label is a centered flex row, and the icon is a centered 24px glyph in the 48px slot.
2. **Click circle is about a quarter of the size, and it clears when the button is released.** The list ripple was drawn large enough to cover the whole row (about 220–280px). Scaling it with `transform` stopped Material’s fade-out, so the circle stayed on the row. It is now clipped with `circle(12.5% at 50% 50%)`, which leaves the animation free to finish.

Still open on this page: anything in the group settings form and the in-page menu (Overview, Settings, Teams, and the rest) that we have not walked yet.

## 2026-10-04 — section headers and group view bars

Same bar as the event admin sections. No add button, because these pages have no add action.

1. **Group settings, registrations, and invoices & payments have the section bar.** Title and icon match the left menu (`settings`, `assignment`, `attach_money`). The invoices title moved out of the card so it is not a second heading.
2. **Group view (dashboard View, not group admin) uses the same padding on both bars.** Switch To Admin and the group name are `app-switch-header`. The card is at least 72px with 16px vertical padding and 24px on the left. The switch button overlaps the lower edge, the same way the add button does on an addition header. The group name bar has no button.
3. **The event page on group view uses the same bar, with the event picture inside it.** Choosing an event in the left menu opens `app-avatar-header`. That card was only as tall as the title, and the picture sat on the lower edge. The bar is 72px with the same padding, and the 40px round picture is centered inside the right side of the bar.

## 2026-10-04 — member detail alignment

1. **Attendances and Joined sit below the name bar, and the icon lines up with the words.** The first line’s margin was collapsing, so it sat against the blue bar. `.user-details` now has 16px above it. Each line is a centered row: 24px icon, then the label.
2. **An attendance row lines up the picture, the event name, and the ticket text.** The picture was a list row, so it sat lower than the words. It is now a 40px circle in the row, and the attendance table cells share one vertical center. The team detail attendance table uses the same markup.
3. **Group registration rows keep the event picture clear of the people counts.** The picture, event name, counts, and status use the same list slots as the event registration rows, with space between the picture and the name, and between the name and the counts.

## 2026-10-04 — unused custom CSS

Removed only rules that no longer match anything. Material’s own styles were left alone.

- Email templates and event registrations no longer load a stylesheet. Those files only styled the old hand-built header, or classes the templates no longer use. The unused group registrations stylesheet went with them.
- The event registration list no longer repeats the shared 40px avatar rule.
- `.pad-up` float rules were dropped where the shared list rule already turns that float off. The class can stay in the markup; it has no effect inside a list row.
- Old `.mat-list-item-content` and `.mat-list-text` selectors were removed. Where a rule still had a live Material 15 selector, that half stayed.
