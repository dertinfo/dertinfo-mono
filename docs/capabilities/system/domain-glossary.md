---
name: Domain glossary
type: system
status: active
updated: 2026-07-26
id: system.domain-glossary
---

# Domain glossary

Product language for DertInfo. The platform supports **sword-dancing clubs** (groups) entering **Dert** and similar events. Implementation detail lives under technical docs — this page is capability vocabulary only.

| Term | Meaning |
|------|---------|
| **Group** | A club / organisation with members (e.g. a sword-dancing side). Any signed-in website user may create a group. |
| **Member** | A person belonging to a group. Members are not permanently assigned to teams in the software. |
| **Guest** | An attendee on a registration treated like a member for booking, but labelled as a non-dancer companion (e.g. spouse). When capacity is tight, guests are typically reduced before dancers. |
| **Team** | A subset of group members formed to enter team activities / competitions (often around five dancers). |
| **Event** | A gathering (notably **Dert**) that hosts activities and competitions. Created by an event administrator via a wizard. |
| **Event type** | Template chosen when creating an event; defines the predefined activity and competition structure. |
| **Activity** | A bookable item on an event. Two kinds: **team activity** and **member activity**. |
| **Team activity** | Competition-style entry for a team (may have a price). Competitions are team activities that are judged. |
| **Member activity** | Ticket-style entry for people attending (e.g. adult, junior, concession). |
| **Competition** | A judged team activity within an event. A typical **Dert** has four: **Main** (five venues), **Traditional** (different marking set), **Spotlight**, and **DERTY** (Dancing England Rapper Tournament Youth; different marking set). A team may enter one or more. Multi-venue competitions aggregate scores across venues. |
| **Award** | A prize within a competition. For main competition typically includes best newcomer, veterans, **Steve Marris** (includes characters), and **overall winner** (excludes characters). |
| **Venue** | Place where (part of) a competition is judged. |
| **Dance** | A team's performance at a venue within a competition — the unit being scored. |
| **Judge** | Person scoring at a venue (typically two per venue). |
| **Score category** | A marking criterion. Typical main set: music, stepping, sword handling, dance technique, presentation, characters, buzz. Traditional and DERTY use different marking sets. |
| **Score set** | A set of score categories allocated to a judge. |
| **Score** | A judge's mark against a score category for a particular dance. |
| **Registration** | A group's submitted request for an event (activities and tickets). Remains **unconfirmed** until the event organiser confirms after the registration period. |
| **Invoice** | Created when registrations are confirmed; sent to the group secretary with pricing and payment instructions. Event admins can mark invoices paid; group admins can see that status. |
| **Results published** | Competition flag set by the event admin after the awards ceremony. Publishes award outcomes publicly and unlocks own-team scores/sheets in the app. |
| **Dert of Derts (DoD)** | Separate online programme: public viewing of uploaded dance videos with scoring and feedback against marking criteria — isolated from core event/competition administration. |

## Client entry points (not capabilities, but context)

| Client | Typical use |
|--------|-------------|
| **Website** | Group and event administration, registration, invoicing, score validation, reporting, publish results |
| **App** | Venue score capture and score-sheet photos; after publish, own-team scores and sheets |

## Data roles (GDPR)

| Term | Meaning |
|------|---------|
| **Data processor** | The platform — processes personal data on behalf of organisers. |
| **Data owner / controller** | Event administrators — responsible for personal data in the context of their events. |

See [User account and GDPR consent](../features/account-gdpr-consent.md) and [Cookie consent](../features/cookie-consent.md).

## Deprecated

- **Competition administrator** as a separate role — not used in current practice; event administrators perform score checking.
- **Super administrator** — platform-wide privilege; **deprecated and should be removed** — see [Super administrator](../roles/super-admin.md).
