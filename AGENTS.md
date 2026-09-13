# AGENTS.md

## Commands
- Full stack (preferred): `docker-compose up --build -d` — API `:5000`, frontend `:8080`, MySQL host `:3307` → container `:3306`.
- Local API: `dotnet run --project MechanicalSheets.Api` (Development, `http://localhost:5063`). Swagger exists **only** in Development (`Program.cs` gates on `IsDevelopment()`; Compose sets `Production`, so no Swagger in Docker).
- E2E check: `chmod +x Scripts/test_workflow.sh && ./Scripts/test_workflow.sh` — needs API on `:5000` plus `curl` + `jq`; covers `create → submit → reject → fix → resubmit → approve → close`.
- No test project, linter, formatter, or CI — `dotnet test` is N/A. Verify with `dotnet build MechanicalSheets.sln` + the workflow script.
- Migrations: `dotnet ef migrations add <Name> --project MechanicalSheets.Api` (Migrations/ is EF-generated, committed).

## Ports / env
- Compose DB: `mechanical_sheets` / `app_user` / `app_pass`, host port `3307`. Local `appsettings*.json` points at `localhost:3307`; Compose overrides to `Server=db;Port=3306` plus `Jwt__*` and `Storage__UploadPath=/app/uploads`.
- JWT key/connection string are committed intentionally for zero-config setup — do not "fix" without asking.

## Architecture
- Single-project .NET 8 API (`MechanicalSheets.Api/`, `MechanicalSheets.sln`). Entrypoint `Program.cs` (DI, JWT, CORS `FrontendPolicy`, `ExceptionHandlingMiddleware` + `ApiKeyMiddleware`, auto-migrate+seed with 5× retry).
- Thin controllers → `Services/SheetService.cs` holds workflow/state rules → `Data/AppDbContext.cs`. Error mapping: `KeyNotFoundException→404`, `UnauthorizedAccessException→401`, `InvalidOperationException→400`; some controllers return `403` directly for ownership failures.
- Enums (`SheetStatusEnum`, `DefectCategoryEnum`) serialize as strings (`JsonStringEnumConverter`) and are stored as strings (`HasConversion<string>`).
- Roles are plain strings `mechanic`/`manager` (intentional, no enum). `GET /api/sheets` scopes mechanics to `CreatedById`; `GET /api/sheets/{id}` returns `404` (not `403`) for another mechanic's sheet.

## Sheet workflow (do not guess)
- `Draft`/`Rejected` = only editable states (PUT sheet, add/remove defects, upload photo). `Submitted` = manager-only approve/reject. `Approved` = immutable. `Closed` = terminal, only via `PUT /api/integration/sheets/{id}/status` with `X-Api-Key` (SHA256-hashed in DB, seeded as `test-api-key-12345`).
- `POST .../submit` requires ≥1 defect item. `POST .../reject` requires `rejectionNote`.
- Integration transitions (`IntegrationController.cs`): `Closed→*` blocked; `Draft→Closed` blocked; `Rejected→` only `Draft` or `Closed`; `→Rejected` requires note.
- `Sheet.Code` and `DefectCatalog.Code` are unique — test scripts must use unique `code` values.

## DB / seed
- Startup auto-runs `Migrate()` + `DbSeeder.SeedAsync()`; seeder is idempotent (`Any()` guards). Never seed manually.
- Seed logins (all `password123`): `mario.rossi@test.com`, `john.doe@test.com`, `luca.bianchi@test.com`, `anna.verdi@test.com` (mechanic); `manager@test.com`, `manager2@test.com` (manager).

## Uploads / frontend
- Attachments: `image/jpeg,png,webp` only, 5MB max (`FormOptions.MultipartBodyLengthLimit`). `Storage__UploadPath` is `/app/uploads` in Docker, `/tmp/mechanical_sheets_uploads` locally. `HasPhoto` is auto-toggled on upload/delete — never set it manually. Download via `GET /api/files/{fileName}` with JWT (served as blob).
- `frontend/` is a single `index.html` (vanilla JS, no build) with hardcoded `API='http://localhost:5000/api'`; nginx `:8080` proxies `/api/` → `api:8080`. JWT role parsing uses MS claim URIs (`.../claims/role`, `emailaddress`, `givenname`).
