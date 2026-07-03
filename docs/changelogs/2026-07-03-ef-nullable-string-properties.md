# EF Core nullable string properties on database entities

## Summary of the work completed

Fixed `SqlNullValueException` on `GET /api/event/web` (and any other query that materializes entities with optional string columns) by aligning C# nullability with the database.

With `<Nullable>enable</Nullable>` and EF Core 8, non-nullable `string` properties are treated as required and use `SqlDataReader.GetString`, which throws when the column is NULL. Most entity string columns are nullable in SQL (no `.IsRequired()` in the model snapshot).

**Change:** Marked database entity string properties as `string?` where the column is nullable, including shared base fields (`CreatedBy`, `ModifiedBy`, `AccessToken`) and entity-specific optional fields on `Event`, `Registration`, `Image`, and the other models under `apps/dert-api/src/dertinfo-models/Database/`.

Legacy membership tables that declare required strings in the snapshot (`UserProfile`, `webpages_Membership`, `webpages_Roles`) were left unchanged (those types are not present as active entity classes in the Database folder).

Verified locally against Docker SQL: an `Event` row with NULL optional strings failed materialization before the change and loaded successfully after.

**Impacted app:** `apps/dert-api` (models).

## Date the work was started

2026-07-03

## Date the work was completed

2026-07-03

## Issues that were encountered on the way

1. **500 on test web event list**  
   Calling `https://dertinfo-test-api-wa.azurewebsites.net/api/event/web` returned `System.Data.SqlTypes.SqlNullValueException` during EF materialization (`GetString` on a null column).  
   **Resolution:** Reproduced on the local Docker SQL rig with an event whose optional strings were NULL; confirmed failure on `Events.Full` (entity load alone), not only on includes. Applied `string?` so EF reads nulls safely.

2. **Optional columns vs non-nullable C# types**  
   Fields such as `AccessToken`, `SentEmailsBcc`, `ContactTelephone`, `EventSynopsis`, and location fields are legitimately NULL for incomplete or legacy rows, but were declared as `string`.  
   **Resolution:** Treat nullable DB columns as `string?` rather than backfilling data for optional fields.

## References to any best practices that we found

- **EF Core and nullable reference types:** When NRT is enabled, map nullable columns to `string?` (or configure the property as optional) so materialization uses null-safe readers. See [EF Core nullable reference types](https://learn.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types).
- **Model matches schema:** Prefer aligning the entity model with existing nullable columns over forcing non-null data for optional business fields.

## Any remaining issues that we may wish to address

- **Downstream nullability:** Call sites and DTOs may still treat some of these values as non-null; tighten or null-check where missing data would cause incorrect UI or business behaviour.
- **Required business fields:** If some columns should never be NULL (e.g. `Event.Name`), consider a data migration plus `.IsRequired()` / non-nullable `string` rather than leaving them optional forever.
- **Deploy to test:** Confirm `/api/event/web` on the test App Service after this change is released.
