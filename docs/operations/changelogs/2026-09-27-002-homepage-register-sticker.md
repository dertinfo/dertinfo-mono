# Hide the register sticker and punctuate the app welcome

## Summary of the work completed

The DERT 2026 homepage sticker that says Register Now is hidden with CSS, and the markup is still in the page. The app homepage welcome line now ends with an exclamation mark. [Pull request 60](https://github.com/dertinfo/dertinfo-mono/pull/60).

## Why the work was completed

The sticker should stay off until the next event is close, and it should be easy to turn back on. Hiding it in CSS keeps the content in place. The exclamation mark is a small, visible change on the app. Together the two edits show whether the new production stack is what `https://www.dertinfo.co.uk` and `https://app.dertinfo.co.uk` are serving after the production switch.

## Date the work was started

2026-09-27

## Date the work was completed

2026-09-27

## Issues that were encountered on the way

- The first edit removed the sticker from the template. That was replaced with `display: none` on `.intro-product-link` in the 2026 intro banner so the sticker can be shown again by deleting that line.
- The change log was not in pull request 60. This page is the follow-up.

## References to any best practices that we found

- None beyond keeping the markup and hiding it in CSS, so restoring the sticker does not mean reconstructing it.

## Any remaining issues that we may wish to address

- Remove `display: none` from `.intro-product-link` in `dert2026-intro-banner.component.scss` when the next event approaches.
- Confirm Web Src CD and App Src CD have published production, and that the public domains show the hidden sticker and the exclamation mark.
