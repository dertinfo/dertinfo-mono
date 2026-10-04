# Event admin visual pass

Working notes for the Material 15 look on the event admin homepage. Same idea as the group admin pass: get this screen right, then reuse the rules.

Reference pages:

- Event overview: `/event/City%20Zoo%20Gathering/1/overview`
- Activities (addition header): `/event/City%20Zoo%20Gathering/1/activities`

## 2026-10-04 — overview menu, header, toolbar, click circle, registrations

1. **Event submenu icons are fully visible, and the labels sit on the same line.** `.profile-sidebar .profile-nav` still had a −24px side margin from when the card itself was padded 24px. The card padding is now 0 and the card clips overflow, so that margin pulled the icons outside the card. The margin is 0, and the icon and label are a centered flex row. This rule is shared with the group admin profile menu.
2. **The addition header is a padded bar again.** `app-addition-header` only holds a title, so the Material 15 card collapsed to the title line and the plus button sat on a thin strip. The card is at least 72px with 16px vertical padding, 24px on the left, and room on the right for the button. The title stays left aligned. The plus button still overlaps the lower edge. This component is also used on other add screens.
3. **The signed-in picture is a circle, level with the alerts icon.** The toolbar button is 48px with 12px of padding, so `max-width: 100%` squeezed the 36px-tall image down to 24px wide. The image is 36×36, `max-width` is left unset, and the button centers it. The alerts icon and the circle share the same vertical center.
4. **The menu click circle fades out again.** Holding the ripple at `transform: scale(0.25)` stopped Material’s own scale animation, so the fade-out never finished and the circle stayed. The circle is clipped to a quarter of the ripple (`circle(12.5% at 50% 50%)`) and the animation is left alone. Still only the main sidebar.
5. **Active registration rows show the picture and the row detail.** `mat-list-avatar` is the old list directive. Material 15 only recognises `matListItemAvatar`, `matListItemTitle`, and `matListItemMeta`, so the picture stayed a 100px square inside a 48px row and the name, counts, and status were clipped. The event registration list uses those directives: a 40px round picture, the group name, the people and team counts, and the status chip.

## 2026-10-04 — shared list, card, and menu rules

These live in the site stylesheet, so the same layout applies wherever the old list and menu markup is used.

1. **Icon and label share a center line.** A list row’s content is a flex row, and the icon is a 24px centered glyph. That covers Contact Information and the activity cards, and the same card-title row.
2. **A list fills the width it is given.** Lists, their children, and card content fill the parent. On the overview the registration row stays inside the half-width card. On the registrations page the same row stretches across the full card, with the status at the right. The group name is kept at its full width instead of being cut short.
3. **Avatar rows stay on one line and show the trailing chip.** A picture marked `mat-list-avatar` is a 40px circle. The row grows with its content, the name and values stay in the row, and a chip at the end stays visible. Invoices use this: round picture, date and name, totals, and the received / not yet received chip.
4. **Select menus grow to their labels.** The dropdown panel is as wide as its longest option, and the option text stays on one line. Registration Submission and Registration Confirmation no longer wrap.

## 2026-10-04 — dialog padding and profile image inset

Shared rules, so other dialogs and profile sidebars pick them up too.

1. **Dialog content sits 10px in from the edge, and a short dialog does not scroll.** The dialog surface is padded 10px and sizes to its content. The title and the Cancel / Add Event buttons use that inset. The old 70vw minimum stays on the overlay, while the inner box fills the padded surface, so the outline of the fields no longer spills into a scrollbar.
2. **The profile picture is inset by 5px.** `.profile-sidebar .grouppic` has 5px of padding, so the event image (and the same picture on a group profile) is not flush with the card edge.

## 2026-10-04 — section headers and dashboard card menus

1. **Section headers match Activities.** Settings, Registrations, Invoices & Payments, Email Templates, Downloads, Competitions, and Paperwork use the same header bar: the menu icon and the menu name, 72px tall, padded. The plus button is only on Activities and Gallery, where something can be added. Email Templates no longer uses a separate unpadded bar. Gallery’s header says Gallery.
2. **“Uploaded Photos” lines up with the other card titles.** Default card titles keep 16px of padding, including under the title when the card content has no padding of its own.
3. **Dashboard card menus sit on the title line.** The event card title row is shorter, the name stays on the left, and the three-dot button is 40px and centered on that same line at the right. Group cards on the dashboard use the same title row.
