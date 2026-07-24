# Repository Instructions

This repository contains reusable .NET architecture templates.

## Mandatory architecture

- Modular Monolith
- Vertical Slice Architecture inside each module
- CQRS
- DDD-Lite
- FastEndpoints
- EF Core
- PostgreSQL
- Domain Events
- Integration Events
- Outbox, Inbox and Idempotency
- Architecture Tests

## Non-negotiable rules

- A module represents a business capability, not a database table.
- Endpoints must remain thin.
- Endpoints must not inject DbContext or repositories.
- Domain must not depend on EF Core, FastEndpoints, ASP.NET Core or Infrastructure.
- Do not expose domain entities as API responses.
- Do not create generic CRUD endpoints, controllers or business services.
- Generic repositories may exist only as internal Infrastructure base classes.
- Each module owns its DbContext, schema, migrations and repositories.
- Modules must not access another module's DbContext or internal entities.
- Commands and queries must remain separated.
- Use stable package versions only.
- Keep package versions in Directory.Packages.props.
- Nullable must be enabled.
- Treat warnings as errors.
- Use CancellationToken in asynchronous operations.
- Do not hard-code secrets.
- Do not leave NotImplementedException in generated code.
- Do not delete tests merely to make the build pass.

## Verification

After meaningful changes, run:

1. dotnet restore
2. dotnet build --configuration Release
3. dotnet test --configuration Release

The task is not complete until the generated templates are packed, installed and tested by generating a clean sample project outside the template source directory.
