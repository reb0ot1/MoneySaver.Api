# Copilot instructions for MoneySaver.Api

## Project purpose
MoneySaver.Api is the .NET 9 backend for a personal finance application. It manages user-scoped transactions, transaction categories, budgets with budget items and limits, spending reports, and basic per-user app configuration.

## Repository layout
- `MoneySaver.Api`: ASP.NET Core Web API host, controllers, middleware, AutoMapper profile, metrics setup
- `MoneySaver.Api.Services`: application/business logic
- `MoneySaver.Api.Data`: EF Core entities, `MoneySaverApiContext`, generic repository
- `MoneySaver.Api.Models`: request/response DTOs, filters, enums, shared models
- `MoneySaver.API.Test`: xUnit test project

The solution file also references sibling projects outside this repository, including `MoneySaver.Identity`, `MoneySaver.SPA`, `MoneySaver.System`, and `MoneySaver.Api.Mcp`. For tasks limited to this repo, focus on the API, services, data, models, and tests here. If authentication, shared infrastructure, or cross-app integration looks incomplete, check those sibling projects before assuming the code in this repo is the full implementation.

## Core domain
- **Transactions**: CRUD plus paged listing with optional search text
- **Categories**: CRUD for transaction categories, with optional parent category
- **Budgets**: create, update, copy, set current budget in use, manage budget items
- **Reports**: expense totals by category, by period, and category-over-time chart data
- **App configuration**: user-level currency and budget defaults

## Request flow and architecture
- The web app uses `Program.cs` + `Startup.cs`, not minimal API hosting.
- Controllers live in `MoneySaver.Api\Controllers`.
- Most controllers are marked with `[Authorize]`.
- `UserPackageMiddleware` reads `ClaimTypes.NameIdentifier` from the authenticated principal and stores it in `UserPackage.UserId`.
- `Repository<TEntity>` automatically filters reads by `UserPackage.UserId` and `!IsDeleted`, and stamps `UserId` on inserts.
- Soft delete is the standard deletion model via `IsDeleted` and `DeletedOnUtc`.
- `TransactionService` is an older service and often returns nullable models/page models directly.
- Budget, category, and app-configuration services commonly return `MoneySaver.System.Services.Result<T>`.

## Important implementation constraints
- Preserve per-user scoping. Do not bypass `UserPackage` or the repository filter unless the task explicitly requires admin or cross-user behavior.
- If you change authentication, keep `ClaimTypes.NameIdentifier` available or update the user-context flow consistently. Repository behavior depends on it.
- Prefer making behavior changes in the service layer, not in controllers.
- Reuse existing DTOs, filters, request models, and AutoMapper mappings before adding new models.
- Keep soft-delete behavior consistent across entities and repository calls.
- Do not add secrets or environment-specific credentials to source control. Use configuration overrides or environment variables.

## Observability and infrastructure
- EF Core uses SQL Server through shared infrastructure registration.
- Logging uses Serilog and is configured from `appsettings.json`.
- OpenTelemetry tracing and metrics are enabled.
- `MetricsMiddleware` records custom request count and request duration metrics.
- `/health` is exposed for health checks.
- Prometheus scraping is enabled for metrics.

## Current auth note
`Startup.cs` still calls `UseAuthentication()` and controllers require authorization, but the JWT bearer registration in `ConfigureServices` is commented out. If auth behavior is part of a task, inspect the wider solution and shared infrastructure before changing controller authorization attributes.

## Main API surface
- `TransactionController`: paged listing, get by id, create, update, soft delete
- `CategoryController`: list, get, create, update, remove
- `BudgetController`: list, get current in-use budget, get items with spent amounts, create, update, copy, add/edit/remove items
- `ReportsController`: expense breakdowns and time-series reporting
- `AppConfigurationController`: get or initialize user configuration

## Common commands
- Run tests: `dotnet test .\MoneySaver.Api.sln --nologo`
- Run the API: `dotnet run --project .\MoneySaver.Api\MoneySaver.Api.csproj`

## Good starting points for investigation
- API setup: `MoneySaver.Api\Startup.cs`
- User scoping: `MoneySaver.Api\Middlewares\UserPackageMiddleware.cs`
- Repository behavior: `MoneySaver.Api.Data\Repositories\Repository.cs`
- Budget logic: `MoneySaver.Api.Services\Implementation\BudgetService.cs`
- Reporting logic: `MoneySaver.Api.Services\Implementation\ReportsService.cs`
