# Module Template

## Add module project to solution
- `dotnet sln add <path-to-module-csproj>`

## Add Host reference
- `dotnet add src/<HostProject>.csproj reference <path-to-module-csproj>`

## Register module
- `builder.Services.Add<ModuleName>Module(builder.Configuration);`

## Create migration
- `dotnet ef migrations add Initial<ModuleName> --project <module-csproj> --startup-project <host-csproj>`
