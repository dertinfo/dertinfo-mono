# EF string nullability aligned to cleaned database

## Summary of the work completed

Corrected the earlier blanket `string?` approach. Using the cleaned database `20260307-DertInfo-Cleaned` as evidence:

1. **Entity models** — `string?` only where the cleaned data still has NULL (or is intentionally empty); `string` (non-nullable) where every populated row had a value.
2. **EF migration** — `StringNullabilityFromCleanedData` alters SQL columns to `NOT NULL` for the 100%-populated fields (with empty-string default during alter), while leaving nullable columns that still contain NULLs.

**Must remain nullable (`string?` / SQL NULL allowed):**

| Entity | Properties |
|--------|------------|
| `DatabaseEntity` | `CreatedBy`, `ModifiedBy` (nulls on `DanceScores` / `ActivityTeamAttendances`) |
| `DatabaseEntity_WithPermissions` | `AccessToken` (e.g. all `EmailTemplates` rows) |
| `Activity` | `Description` |
| `DanceScore` | `CommentGiven` |
| `Event` | `EventSynopsis`, `LocationTown`, `LocationPostcode` |
| `EventSetting` | `Value` |
| `Image` | `Container`, `Extension` |
| `Registration` | `SpecialRequirements` (all empty string in cleaned DB) |
| `ScoreCategory` | `Description` |
| `Venue` | `Auth0Username` |
| Empty / unused tables in cleaned DB | DoD entities, `NavigationItem`, `Spectator`, `Steward`, `StaticResult`, `BreadcrumbItem`, `AccessKeyUser`, `DatabaseCacheItem` — kept nullable (no 100% evidence) |

**Required (`string` / migration sets NOT NULL):** identity and fully populated fields such as `Activity.Title`, `AttendanceClassification.ClassificationName`, `Event.Name` / contacts / `SentEmailsBcc`, `Group.*` filled columns, `Image.BlobPath` / `BlobName` / `ImageAlt`, etc.

**Impacted app:** `apps/dert-api` (models + EF migration).

## Date the work was started

2026-07-03

## Date the work was completed

2026-07-18

## Issues that were encountered on the way

1. **Blanket `string?` was the wrong domain fix**  
   PR approach made every entity string nullable. Reverted that strategy in favour of cleaned-DB evidence.

2. **EF migration scaffold noise**  
   Scaffold also tried to drop legacy `UserProfile` / `webpages_*` tables (present in the old snapshot but not as CLR entities) and, earlier, to re-add Image blob columns missing from a stale snapshot. Those ops were removed from the migration; Image blob columns were first added to the model snapshot so only nullability alters remain.

3. **Base audit / AccessToken columns**  
   Cannot be non-nullable on the shared base types because some tables still have NULL; nullability stays on the base.

## References to any best practices that we found

- Align C# NRT (`string` vs `string?`) with real data and intended constraints; use a cleaned dataset to decide, then encode in the model and a migration.
- [EF Core nullable reference types](https://learn.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types)

## Any remaining issues that we may wish to address

- Apply the migration only to environments that match the cleaned nullability profile (or backfill first); non-cleaned DBs may fail `NOT NULL` alters.
- Empty-table entities (DoD, etc.) still use `string?` until real data exists — revisit then.
- Consider adding CLR types for legacy `UserProfile` / `webpages_*` so future scaffolds do not propose dropping them.
- Follow up: convert cleaned empty strings (e.g. `SpecialRequirements`) to NULL if preferred.
