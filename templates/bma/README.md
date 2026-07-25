# BusinessTemplate

Modular monolith template with Vertical Slice, CQRS, DDD-lite, FastEndpoints, EF Core, PostgreSQL and Outbox.

## Prerequisites
- .NET SDK 10.0.301+
- Docker

## Run PostgreSQL
```bash
docker compose up -d
```

## Build and test
```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
```
