# نقش و مأموریت

تو یک Software Architect و Senior .NET Engineer هستی. وظیفه تو ساخت یک قالب معماری حرفه‌ای، قابل نصب، قابل توسعه و قابل استفاده مجدد برای پروژه‌های بیزنسی مبتنی بر .NET است.

این مأموریت صرفاً تولید یک ساختار پوشه‌ای یا تعدادی فایل نمونه نیست. باید یک Repository واقعی، کامپایل‌شونده، تست‌شده و قابل نصب به‌عنوان `dotnet new template` بسازی.

بدون دریافت تأیید مرحله‌ای، تمام مراحل را از ابتدا تا انتها انجام بده. فقط در صورت وجود مانع فنی غیرقابل‌حل، آن را در گزارش نهایی ثبت کن و بهترین راه‌حل جایگزین را اجرا کن.

---

# هدف نهایی

یک Template Package با نام پیشنهادی زیر ایجاد کن:

```text
BusinessArchitecture.Templates
```

این پکیج باید حداقل چهار Template قابل نصب ارائه دهد:

```powershell
dotnet new bma -n MyBusiness

dotnet new bma-module -n Billing --rootNamespace MyBusiness

dotnet new bma-command -n CreateInvoice --module Billing --entity Invoice

dotnet new bma-query -n GetInvoiceById --module Billing --entity Invoice
```

Template اصلی `bma` باید یک Solution کامل و قابل اجرا با معماری زیر ایجاد کند:

```text
Modular Monolith
+ Vertical Slice Architecture
+ CQRS
+ DDD-Lite
+ FastEndpoints
+ EF Core
+ PostgreSQL
+ Domain Events
+ Integration Events
+ Outbox / Inbox
+ Idempotency
+ Architecture Tests
```

---

# فناوری‌های اصلی

از فناوری‌های زیر استفاده کن:

```text
.NET 10
ASP.NET Core
FastEndpoints
FastEndpoints.Swagger
Entity Framework Core
PostgreSQL
Npgsql
xUnit
FluentAssertions
NetArchTest یا ابزار مناسب Architecture Testing
Testcontainers for PostgreSQL
Microsoft.AspNetCore.Mvc.Testing
Central Package Management
Docker Compose
```

قوانین نسخه‌ها:

1. فقط از نسخه‌های Stable و سازگار استفاده کن.
2. از نسخه Preview، RC و نسخه‌های شناور مانند `*` استفاده نکن.
3. تمام نسخه‌های NuGet را در `Directory.Packages.props` متمرکز کن.
4. قبل از انتخاب نسخه‌ها، نسخه SDK نصب‌شده در محیط را بررسی کن.
5. هدف اصلی `net10.0` است.
6. اگر .NET 10 در محیط موجود نبود و نصب آن ممکن نبود، از جدیدترین LTS نصب‌شده استفاده کن و این انحراف را در گزارش نهایی ثبت کن.
7. کد نهایی نباید به نسخه‌ای که در محیط قابل Restore نیست وابسته باشد.

---

# اصول معماری غیرقابل‌تغییر

## ۱. مرز ماژول

هر Module باید نماینده یک Business Capability یا Bounded Context باشد، نه یک جدول ساده.

نمونه درست:

```text
Identity
Catalog
Inventory
Reservations
Billing
Payments
Notifications
Audit
```

نمونه نادرست:

```text
UsersModule
UserAddressModule
UserPhoneModule
UserSessionModule
```

Entityهای مرتبط باید داخل ماژول بیزنسی مناسب خود قرار بگیرند.

---

## ۲. معماری داخلی ماژول

داخل هر Module از Vertical Slice Architecture استفاده کن.

تمام فایل‌های مربوط به یک Use Case باید کنار یکدیگر باشند:

```text
Features/
└── Users/
    └── RegisterUser/
        ├── Endpoint.cs
        ├── Request.cs
        ├── Response.cs
        ├── Validator.cs
        ├── Command.cs
        ├── Handler.cs
        └── Mapper.cs
```

فایل‌ها را بر اساس نوع فنی سراسری مانند Controllers، Services، DTOs و Validators پراکنده نکن.

---

## ۳. CQRS

عملیات تغییر‌دهنده وضعیت باید Command باشند:

```text
RegisterUserCommand
UpdateUserCommand
SuspendUserCommand
DeleteUserCommand
```

عملیات فقط‌خواندنی باید Query باشند:

```text
GetUserByIdQuery
SearchUsersQuery
```

از FastEndpoints برای HTTP Endpoint، Validation، Mapping و Command/Event Bus استفاده کن.

به‌صورت پیش‌فرض MediatR اضافه نکن. وجود هم‌زمان FastEndpoints Command Bus و MediatR بدون نیاز واقعی ممنوع است.

---

## ۴. Endpointهای نازک

Endpoint فقط باید مسئول موارد زیر باشد:

```text
HTTP route
Authentication / Authorization
دریافت Request
ارسال Command یا Query
تبدیل Result به HTTP Response
```

این موارد نباید داخل Endpoint باشند:

```text
منطق بیزنسی
دسترسی مستقیم به DbContext
کوئری EF Core
مدیریت Transaction
ارسال مستقیم پیامک
انتشار مستقیم Integration Event
```

---

## ۵. Domain

Domain باید شامل موارد زیر باشد:

```text
Entities
Aggregate Roots
Value Objects
Domain Errors
Domain Services در صورت نیاز
Domain Events
Repository Interfaces برای Aggregate Rootها
Business Rules
```

Domain نباید به موارد زیر وابستگی داشته باشد:

```text
EF Core
FastEndpoints
ASP.NET Core
PostgreSQL
Infrastructure
HTTP
Swagger
```

---

## ۶. Repository

Repositoryهای اختصاصی و بیزنسی ایجاد کن:

```csharp
public interface IUserRepository
{
    Task<User?> GetByIdAsync(
        UserId id,
        CancellationToken cancellationToken);

    Task<bool> PhoneNumberExistsAsync(
        string phoneNumber,
        CancellationToken cancellationToken);

    Task AddAsync(
        User user,
        CancellationToken cancellationToken);
}
```

استفاده مستقیم از `IGenericRepository<TEntity>` در Handlerها ممنوع است.

در Infrastructure می‌توان یک Base Repository داخلی و Generic برای کاهش کدهای تکراری داشت، اما این کلاس باید:

```text
internal یا abstract باشد
در لایه Infrastructure باقی بماند
منطق بیزنسی نداشته باشد
به Handlerها تزریق نشود
```

Generic CRUD Endpoint، Generic Business Service و Generic Controller نساز.

---

## ۷. Query Side

در Query Handlerها استفاده مستقیم از EF Core Projection مجاز است:

```text
AsNoTracking
Where
Select به Response DTO
Pagination
Sorting
Filtering
```

برای Queryهای ساده Repository اجباری نیست.

Entity دامنه را مستقیماً به API Response تبدیل نکن. Response DTO مستقل ایجاد کن.

---

## ۸. دیتابیس

هر ماژول باید مالک DbContext خودش باشد:

```text
IdentityDbContext
CatalogDbContext
ReservationDbContext
BillingDbContext
```

در نسخه Monolith، همه DbContextها می‌توانند از یک PostgreSQL Database استفاده کنند، اما هر ماژول باید Schema مستقل داشته باشد:

```text
identity
catalog
reservation
billing
```

هیچ ماژولی حق ندارد مستقیماً از DbContext، Entity، Repository یا جدول داخلی ماژول دیگر استفاده کند.

تنظیمات EF Core هر Entity باید در فایل مستقل باشد:

```text
Infrastructure/Persistence/Configurations/UserConfiguration.cs
```

Migrationهای هر ماژول باید داخل همان ماژول نگهداری شوند.

---

## ۹. ارتباط بین ماژول‌ها

ارتباط مستقیم با Entity یا DbContext ماژول دیگر ممنوع است.

ارتباط هم‌زمان فقط از طریق Contract عمومی ماژول انجام شود.

ارتباط غیرهم‌زمان از طریق Integration Event انجام شود.

Domain Event و Integration Event را از یکدیگر جدا نگه دار:

```text
Domain Event:
داخل همان ماژول
نماینده یک رخداد مهم دامنه
معمولاً داخل همان Process

Integration Event:
بین ماژول‌ها
قابل نسخه‌بندی
قابل Retry
ذخیره‌شده در Outbox
```

زیرساخت Domain Event را حذف نکن، اما استفاده از آن را برای تمام عملیات اجباری نکن.

---

## ۱۰. Outbox، Inbox و Idempotency

یک پیاده‌سازی واقعی و حداقلی اما قابل اجرا برای موارد زیر بساز:

```text
OutboxMessage
InboxMessage
IdempotencyKey
Outbox Processor
Inbox Deduplication
Background Worker
```

الزامات:

1. Integration Event باید ابتدا در Outbox همان Transaction ذخیره شود.
2. Processor باید پیام‌های Outbox را پردازش کند.
3. وضعیت پردازش، زمان پردازش و خطا ثبت شود.
4. Retry محدود و قابل تنظیم وجود داشته باشد.
5. پردازش مجدد یک Message نباید باعث اجرای تکراری عملیات شود.
6. در Template پایه نیازی به RabbitMQ یا Kafka نیست.
7. Event Transport را طوری Interface-based طراحی کن که بعداً قابل جایگزینی باشد.
8. پیاده‌سازی پیش‌فرض می‌تواند In-Process باشد.
9. قابلیت جایگزینی با Broker خارجی را در README توضیح بده.

---

# ساختار Solution اصلی

ساختار خروجی Template اصلی باید مشابه زیر باشد:

```text
BusinessTemplate/
├── BusinessTemplate.slnx
├── Directory.Build.props
├── Directory.Packages.props
├── global.json
├── .editorconfig
├── .gitignore
├── README.md
├── docker-compose.yml
├── compose.yaml
│
├── src/
│   ├── BusinessTemplate.Host/
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   ├── Middleware/
│   │   ├── OpenApi/
│   │   └── DependencyInjection/
│   │
│   ├── BuildingBlocks/
│   │   ├── BusinessTemplate.BuildingBlocks.Domain/
│   │   ├── BusinessTemplate.BuildingBlocks.Application/
│   │   ├── BusinessTemplate.BuildingBlocks.Infrastructure/
│   │   └── BusinessTemplate.BuildingBlocks.Contracts/
│   │
│   └── Modules/
│       └── Identity/
│           └── BusinessTemplate.Modules.Identity/
│
└── tests/
    ├── BusinessTemplate.ArchitectureTests/
    ├── BusinessTemplate.IntegrationTests/
    └── Modules/
        └── Identity/
            └── BusinessTemplate.Modules.Identity.Tests/
```

`BusinessTemplate` باید مقدار `sourceName` اصلی Template باشد تا با نام پروژه جایگزین شود.

مثلاً:

```powershell
dotnet new bma -n RentalSystem
```

باید نام‌ها و Namespaceها را به این شکل تغییر دهد:

```text
BusinessTemplate.Host
→ RentalSystem.Host

BusinessTemplate.Modules.Identity
→ RentalSystem.Modules.Identity

namespace BusinessTemplate.Modules.Identity
→ namespace RentalSystem.Modules.Identity
```

---

# Building Blocks موردنیاز

## BuildingBlocks.Domain

حداقل موارد زیر را پیاده‌سازی کن:

```text
Entity<TId>
AggregateRoot<TId>
ValueObject
IDomainEvent
DomainEvent
Result
Result<T>
Error
ErrorType
DomainException
BusinessRule
```

ویژگی‌های `Entity<TId>`:

```text
Id Strongly Typed
Equality مناسب
عدم وابستگی به EF Core
```

ویژگی‌های `AggregateRoot<TId>`:

```text
Domain Events collection
RaiseDomainEvent
ClearDomainEvents
Read-only events
```

---

## BuildingBlocks.Application

حداقل موارد زیر:

```text
ICommand<TResult>
ICommandHandler<TCommand, TResult>
IQuery<TResult>
IQueryHandler<TQuery, TResult>
IUnitOfWork
PagedRequest
PagedResult<T>
Sorting models
Filtering abstractions
ICurrentUser
IClock
```

در صورت استفاده از FastEndpoints Command Bus، Interfaceها را با FastEndpoints سازگار طراحی کن و از ایجاد Dispatcher اضافی خودداری کن.

---

## BuildingBlocks.Contracts

حداقل موارد زیر:

```text
IIntegrationEvent
IntegrationEvent
ContractVersion
EventEnvelope<T>
CorrelationId
CausationId
```

---

## BuildingBlocks.Infrastructure

حداقل موارد زیر:

```text
Generic internal Repository base
Outbox
Inbox
Idempotency
Event serialization
Event dispatcher
Background processor
SystemClock
Current user adapter
Database conventions
Health checks
Observability extensions
```

---

# ماژول نمونه Identity

یک ماژول واقعی و کامل Identity بساز که صرفاً نمایشی و ناقص نباشد.

حداقل Entityها:

```text
User
Role
Permission
UserRole
RolePermission
```

حداقل Value Objectها:

```text
UserId
RoleId
PhoneNumber
EmailAddress
```

حداقل وضعیت‌های کاربر:

```text
Active
Suspended
Deleted
```

قوانین نمونه:

```text
شماره تلفن یکتا باشد
ایمیل در صورت وجود معتبر باشد
کاربر Suspended نتواند Login شود
کاربر حذف‌شده دوباره فعال نشود
نام کاربر خالی نباشد
```

---

# Featureهای کامل ماژول Identity

حداقل Featureهای زیر را کامل پیاده‌سازی کن:

```text
RegisterUser
GetUserById
SearchUsers
UpdateUser
SuspendUser
ActivateUser
DeleteUser
AssignRoleToUser
RemoveRoleFromUser
```

برای هر Feature فایل‌های موردنیاز را کنار هم قرار بده:

```text
Endpoint
Request
Response
Validator
Command یا Query
Handler
Mapper در صورت نیاز
Errors در صورت نیاز
```

---

# RegisterUser

این Feature باید نشان‌دهنده استاندارد معماری باشد.

جریان:

```text
HTTP Request
→ Validator
→ Command
→ Command Handler
→ Business validation
→ User Aggregate
→ IUserRepository
→ IdentityDbContext
→ Unit of Work
→ Domain Event
→ Outbox Integration Event
→ HTTP Response
```

در ثبت کاربر:

1. نام الزامی باشد.
2. شماره تلفن معتبر و یکتا باشد.
3. ایمیل اختیاری ولی معتبر باشد.
4. User Aggregate ساخته شود.
5. `UserRegisteredDomainEvent` ایجاد شود.
6. یک Integration Event مناسب داخل Outbox ذخیره شود.
7. عملیات در Transaction واحد Commit شود.
8. پاسخ `201 Created` به همراه شناسه کاربر ارسال شود.

---

# SearchUsers

ویژگی‌های SearchUsers:

```text
Pagination
Sorting
Search term
Filter by status
Filter by role
Created date range
AsNoTracking
Projection مستقیم به DTO
حداکثر PageSize
```

یک Response استاندارد از نوع `PagedResult<UserListItemResponse>` برگردان.

---

# Error Handling

یک مدل استاندارد خطا ایجاد کن.

نمونه:

```csharp
public sealed record Error(
    string Code,
    string Message,
    ErrorType Type);
```

انواع خطا:

```text
Validation
NotFound
Conflict
Unauthorized
Forbidden
Failure
```

نگاشت Result به HTTP:

```text
Validation → 400
Unauthorized → 401
Forbidden → 403
NotFound → 404
Conflict → 409
Failure → 500
```

از Problem Details استاندارد استفاده کن.

هیچ Exception خام یا Stack Trace در Production Response برنگردان.

---

# Validation

برای Request Validation از Validatorهای FastEndpoints مبتنی بر FluentValidation استفاده کن.

Validation را به دو بخش تقسیم کن:

```text
Request Validation:
فرمت، مقدار خالی، طول، محدوده

Business Validation:
یکتا بودن شماره، وضعیت کاربر، قوانین دامنه
```

Business Validation را فقط در HTTP Validator قرار نده.

---

# امنیت و احراز هویت

برای Template پایه یک Authentication قابل اجرا ایجاد کن.

حداقل:

```text
JWT Bearer Authentication
Access Token
Refresh Token entity یا model
Password hashing abstraction
Current user abstraction
Role/Permission authorization
```

الزامات:

1. Secret واقعی Hard-code نکن.
2. Secret فقط از Configuration یا Environment Variable دریافت شود.
3. Development configuration مقدار نمونه غیرحساس داشته باشد.
4. Production بدون Secret معتبر Startup Failure بدهد.
5. Permissionها به‌صورت ثابت و قابل استفاده در Endpointها تعریف شوند.
6. حداقل Login و Refresh Token Feature ایجاد کن.
7. Password Hashing از پیاده‌سازی امن استاندارد استفاده کند.
8. رمز عبور خام هرگز ذخیره یا Log نشود.

اگر پیچیدگی Identity کامل ASP.NET Core Identity با معماری ماژولار تضاد ایجاد کرد، یک Authentication ساده و امن مبتنی بر Entityهای خود ماژول بساز و تصمیم را مستند کن.

---

# Multi-Tenancy Readiness

Template باید برای Multi-Tenant شدن آماده باشد، اما نسخه پایه الزاماً Tenant Management کامل ندارد.

موارد زیر را آماده کن:

```text
ITenantContext
TenantId value object
Tenant-aware entity interface
Global query filter extension
Tenant resolution abstraction
Tenant header resolver نمونه
```

الزامات:

1. ماژول Identity بتواند User سراسری یا Tenant-aware بودن را از طریق Design مشخص کنترل کند.
2. Cross-tenant data leakage با Query Filter جلوگیری شود.
3. TenantId از Request Body قابل اعتماد دریافت نشود.
4. Tenant از Context معتبر استخراج شود.
5. نحوه فعال یا غیرفعال کردن Multi-Tenancy در README توضیح داده شود.

---

# ثبت ماژول‌ها

هر Module باید Extension Method خودش را داشته باشد:

```csharp
builder.Services.AddIdentityModule(
    builder.Configuration);
```

و در Host:

```csharp
builder.Services
    .AddIdentityModule(builder.Configuration);
```

ماژول باید موارد زیر را خودش ثبت کند:

```text
DbContext
Repositories
Handlers
Validators
Options
Authentication services
Hosted services مرتبط
Health checks
```

در Host نباید جزئیات داخلی ماژول ثبت شود.

---

# Options Pattern

برای تنظیمات از Strongly Typed Options استفاده کن:

```text
DatabaseOptions
JwtOptions
OutboxOptions
IdentityOptions
MultiTenancyOptions
```

الزامات:

```text
BindConfiguration
ValidateDataAnnotations
ValidateOnStart
Defaultهای منطقی
عدم Hard-code تنظیمات محیطی
```

---

# Observability

حداقل موارد زیر را اضافه کن:

```text
Structured logging
Correlation ID
Request logging
Health checks
Database health check
Readiness endpoint
Liveness endpoint
Basic metrics/tracing extension points
```

اگر OpenTelemetry را اضافه می‌کنی، آن را قابل غیرفعال‌سازی و بدون وابستگی اجباری به Collector خارجی طراحی کن.

---

# Docker

یک Docker Compose قابل اجرا برای PostgreSQL بساز.

حداقل سرویس‌ها:

```text
postgres
application در صورت امکان
```

موارد لازم:

```text
Volume
Health check
Environment variables
Local development defaults
Connection string
عدم استفاده از Credential واقعی
```

فرمان اجرای مستند:

```powershell
docker compose up -d
```

---

# Migration

Migration اولیه ماژول Identity را ایجاد کن.

الزامات:

```text
Schema identity
Users
Roles
Permissions
UserRoles
RolePermissions
OutboxMessages
InboxMessages در Schema مناسب
Indexes
Unique constraints
Foreign keys
Concurrency token در نقاط لازم
```

فرمان‌های ایجاد و اجرای Migration را در README ثبت کن.

در صورت امکان Startup Migration خودکار را فقط برای Development فعال کن و برای Production استفاده مستقیم و بدون کنترل را ممنوع کن.

---

# تست‌ها

فقط Unit Test نمونه نساز. مجموعه تست‌های واقعی ایجاد کن.

## ۱. Domain Unit Tests

موارد نمونه:

```text
User creation
Invalid phone number
Suspend active user
Cannot suspend deleted user
Cannot activate deleted user
Domain event creation
Role assignment
Duplicate role assignment
```

---

## ۲. Handler Tests

حداقل:

```text
RegisterUser موفق
RegisterUser با شماره تکراری
GetUserById موجود
GetUserById ناموجود
SearchUsers pagination
SuspendUser
AssignRoleToUser
```

---

## ۳. Integration Tests

با PostgreSQL واقعی از طریق Testcontainers:

```text
EF configurations
Unique constraints
Repository behavior
Transaction behavior
Outbox persistence
Query projection
Migration application
```

از EF Core InMemory برای تست رفتار دیتابیس استفاده نکن.

---

## ۴. Functional / Endpoint Tests

با `WebApplicationFactory`:

```text
POST register user
Validation failure
Duplicate phone conflict
GET user by id
Search users
Authentication
Authorization
Problem Details response
```

---

## ۵. Architecture Tests

Architecture Testها باید حداقل قوانین زیر را enforce کنند:

```text
Domain به Infrastructure وابسته نباشد
Domain به EF Core وابسته نباشد
Domain به FastEndpoints وابسته نباشد
Application به Host وابسته نباشد
Modules به DbContext یکدیگر وابسته نباشند
Endpoints مستقیم DbContext را Inject نکنند
Endpoints مستقیم Repository را Inject نکنند
Entityها به‌عنوان API Response استفاده نشوند
Repository implementationها internal باشند
Integration Events داخل Contracts باشند
```

تست‌ها باید در CI قابل اجرا باشند.

---

# Template اصلی bma

در این مسیر Template اصلی را بساز:

```text
templates/bma/
```

فایل موردنیاز:

```text
templates/bma/.template.config/template.json
```

ویژگی‌های Template:

```json
{
  "identity": "BusinessArchitecture.Solution",
  "name": "Business Modular Architecture",
  "shortName": "bma",
  "sourceName": "BusinessTemplate",
  "preferNameDirectory": true
}
```

موارد زیر را از خروجی Template حذف کن:

```text
bin
obj
.git
.vs
.idea
TestResults
coverage
artifacts
```

پارامترهای اختیاری مناسب اضافه کن:

```text
--database postgres
--includeDocker true/false
--includeSampleModule true/false
--includeAuth true/false
--includeOutbox true/false
--multiTenant true/false
```

مقدارهای پیش‌فرض باید معماری کامل و قابل اجرا تولید کنند.

حداقل مقدار پیش‌فرض:

```text
database = postgres
includeDocker = true
includeSampleModule = true
includeAuth = true
includeOutbox = true
multiTenant = false
```

تمام شرط‌های Template را واقعی و تست‌شده پیاده‌سازی کن. فایل یا Reference شکسته باقی نگذار.

---

# Template ماژول bma-module

در مسیر زیر بساز:

```text
templates/bma-module/
```

فرمان مورد انتظار:

```powershell
dotnet new bma-module \
  -n Billing \
  --rootNamespace RentalSystem
```

خروجی باید شامل موارد زیر باشد:

```text
RentalSystem.Modules.Billing.csproj
Domain/
Features/
Infrastructure/
Contracts/
DependencyInjection.cs
BillingModule.cs
README.md
```

پارامترهای لازم:

```text
name
rootNamespace
databaseSchema
includeTests
includeOutbox
```

قالب ماژول باید Namespaceها را درست جایگزین کند.

در README توضیح بده که بعد از ایجاد ماژول چگونه:

```text
پروژه به Solution اضافه شود
Reference به Host اضافه شود
Module در Program.cs ثبت شود
Migration ساخته شود
```

اگر Template Engine به‌تنهایی امکان ویرایش خودکار Solution و Program.cs را نداشت، یک Script امن PowerShell و Bash برای این کار ایجاد کن.

---

# Template Command

مسیر:

```text
templates/bma-command/
```

فرمان:

```powershell
dotnet new bma-command \
  -n CreateInvoice \
  --module Billing \
  --entity Invoice \
  --rootNamespace RentalSystem
```

خروجی:

```text
CreateInvoice/
├── Endpoint.cs
├── Request.cs
├── Response.cs
├── Validator.cs
├── Command.cs
├── Handler.cs
└── Mapper.cs
```

Command Template نباید دارای Generic CRUD logic باشد.

کد تولیدی باید Compile شود یا حداقل Placeholderهای مشخص و محدود داشته باشد.

از `NotImplementedException` در کد نهایی تولیدی استفاده نکن.

در جاهایی که اطلاعات بیزنسی مشخص نیست، یک Feature نمونه حداقلی و Compile-safe تولید کن و محل توسعه را با Comment روشن مشخص کن.

---

# Template Query

مسیر:

```text
templates/bma-query/
```

فرمان:

```powershell
dotnet new bma-query \
  -n GetInvoiceById \
  --module Billing \
  --entity Invoice \
  --rootNamespace RentalSystem
```

خروجی:

```text
GetInvoiceById/
├── Endpoint.cs
├── Request.cs
├── Response.cs
├── Query.cs
├── Handler.cs
└── Mapper.cs
```

Query تولیدی باید:

```text
AsNoTracking
Projection
CancellationToken
Result handling
NotFound handling
```

داشته باشد.

---

# Template Package

یک پروژه NuGet Template Package ایجاد کن:

```text
BusinessArchitecture.Templates.csproj
```

الزامات:

```text
PackageType = Template
IncludeContentInPack = true
IncludeBuildOutput = false
ContentTargetFolders = content
```

پکیج باید تمام Templateها را شامل شود:

```text
bma
bma-module
bma-command
bma-query
```

فرمان Pack:

```powershell
dotnet pack \
  BusinessArchitecture.Templates.csproj \
  --configuration Release \
  --output artifacts
```

---

# README اصلی

README باید دقیق، قابل اجرا و غیرکلی باشد.

حداقل بخش‌ها:

```text
معرفی معماری
پیش‌نیازها
نصب Template
ساخت پروژه جدید
ساخت Module
ساخت Command
ساخت Query
اجرای PostgreSQL
Connection String
اجرای Migration
اجرای برنامه
Swagger
اجرای تست‌ها
ساخت NuGet Package
نصب فایل nupkg
حذف Template
قوانین معماری
روش اضافه کردن Module جدید
Domain Event vs Integration Event
Outbox و Idempotency
Multi-Tenancy
محدودیت‌ها
Troubleshooting
```

فرمان‌ها را برای PowerShell و Bash ارائه کن.

---

# GitHub Actions یا CI

یک Workflow بساز که در هر Push و Pull Request موارد زیر را اجرا کند:

```text
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
dotnet pack
Template install
ساخت پروژه نمونه از bma
Build پروژه نمونه
Test پروژه نمونه
ایجاد Module نمونه
ایجاد Command نمونه
ایجاد Query نمونه
Build مجدد Solution نمونه
```

CI باید ثابت کند Template فقط Package نمی‌شود، بلکه خروجی آن نیز Compile می‌شود.

---

# روند اجرایی اجباری

کار را به ترتیب زیر انجام بده:

## فاز ۱: بررسی محیط

```text
نسخه dotnet
نسخه Docker
Git status
ساختار Repository
SDKهای نصب‌شده
```

اگر Repository خالی نیست، ابتدا فایل‌ها را بررسی کن و بدون حذف غیرضروری، ساختار را تکمیل کن.

---

## فاز ۲: طراحی و ایجاد Solution مرجع

ابتدا یک Solution مرجع با نام `BusinessTemplate` بساز.

تا زمانی که Solution مرجع کامپایل و تست نشده، آن را به Template تبدیل نکن.

---

## فاز ۳: پیاده‌سازی Building Blocks

تمام Building Blockهای موردنیاز را ایجاد و تست کن.

---

## فاز ۴: پیاده‌سازی Identity Module

Featureهای تعیین‌شده، EF Core، PostgreSQL، Domain Event و Outbox را کامل کن.

---

## فاز ۵: تست Solution مرجع

فرمان‌های زیر باید موفق باشند:

```powershell
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
```

---

## فاز ۶: ساخت Templateها

چهار Template زیر را ایجاد کن:

```text
bma
bma-module
bma-command
bma-query
```

---

## فاز ۷: Pack و نصب محلی

پکیج را Pack و از فایل `.nupkg` نصب کن.

قبل از نصب نسخه جدید، Template قبلی با همین Identity را حذف کن تا Conflict ایجاد نشود.

---

## فاز ۸: آزمون End-to-End Template

در پوشه‌ای خارج از Source Template، پروژه نمونه بساز:

```powershell
dotnet new bma -n SampleBusiness
```

سپس:

```powershell
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
```

PostgreSQL را اجرا کن و Migration را اعمال کن.

بعد برنامه را اجرا و Endpointهای اصلی را Smoke Test کن.

حداقل بررسی:

```text
Health endpoint
Swagger/OpenAPI
Register user
Duplicate phone
Get user
Search users
Login
Authorization failure
Outbox processing
```

---

## فاز ۹: آزمون Generatorها

داخل پروژه `SampleBusiness`:

```powershell
dotnet new bma-module \
  -n Billing \
  --rootNamespace SampleBusiness

dotnet new bma-command \
  -n CreateInvoice \
  --module Billing \
  --entity Invoice \
  --rootNamespace SampleBusiness

dotnet new bma-query \
  -n GetInvoiceById \
  --module Billing \
  --entity Invoice \
  --rootNamespace SampleBusiness
```

فایل‌ها را به مسیر صحیح منتقل یا مستقیماً در مسیر مناسب تولید کن.

Project Referenceها و Solution را تنظیم کن.

سپس دوباره:

```powershell
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
```

هیچ فایل تولیدی نباید باعث شکستن Build شود.

---

# معیارهای پذیرش نهایی

کار فقط زمانی کامل است که تمام شرایط زیر برقرار باشند:

```text
Solution مرجع Build می‌شود
تمام Testها Pass می‌شوند
Template Package ساخته می‌شود
Template از nupkg نصب می‌شود
dotnet new bma پروژه معتبر تولید می‌کند
پروژه تولیدشده Build می‌شود
پروژه تولیدشده Test می‌شود
PostgreSQL و Migration کار می‌کنند
Swagger باز می‌شود
Endpointهای نمونه کار می‌کنند
Module Template کار می‌کند
Command Template کار می‌کند
Query Template کار می‌کند
خروجی Generatorها Build را نمی‌شکند
Architecture Testها فعال‌اند
هیچ Secret واقعی وجود ندارد
هیچ TODO حیاتی وجود ندارد
هیچ NotImplementedException وجود ندارد
هیچ Generic CRUD Endpoint وجود ندارد
هیچ Entity به‌طور مستقیم API Response نیست
هیچ Module به DbContext ماژول دیگر وابسته نیست
```

---

# ممنوعیت‌ها

این موارد ممنوع هستند:

```text
ساخت فقط پوشه‌های خالی
تحویل کد Compile نشده
استفاده از Preview package
استفاده از نسخه NuGet شناور
استفاده از EF Core InMemory برای Integration Test
Generic CRUD Controller
Generic CRUD Endpoint
Generic Business Service
تزریق DbContext به Endpoint
تزریق Repository به Endpoint
منطق بیزنسی داخل Endpoint
استفاده مستقیم از Entity به‌عنوان DTO
وابستگی Domain به EF Core
وابستگی مستقیم Moduleها به یکدیگر
Hard-coded secrets
حذف تست برای سبز کردن Build
نادیده گرفتن Warningهای مهم
قرار دادن همه چیز در Shared یا Common
ترک کردن TODO برای بخش‌های اصلی
```

---

# کیفیت کد

الزامات کیفیت:

```text
Nullable enabled
Implicit usings enabled
Warnings as errors
CancellationToken در عملیات async
Async suffix
sealed class در صورت مناسب بودن
internal implementationها
Strongly typed IDs
DateTimeOffset یا UTC abstraction
No DateTime.Now مستقیم در Domain
No service locator
No static mutable state
No hidden global dependency
XML documentation فقط برای APIهای عمومی مهم
Code formatting استاندارد
```

از `IClock` برای زمان استفاده کن.

از `Guid.NewGuid()` مستقیم در Domain فقط در صورت تصمیم مستند استفاده کن؛ ترجیحاً `IIdGenerator` یا Factory مناسب ایجاد کن.

---

# گزارش نهایی Codex

پس از انجام کار، یک گزارش نهایی دقیق ارائه کن که شامل موارد زیر باشد:

```text
۱. خلاصه معماری ایجادشده
۲. ساختار فایل‌ها و پروژه‌ها
۳. نسخه SDK و Packageهای اصلی
۴. Templateهای ایجادشده
۵. فرمان نصب Template
۶. فرمان ساخت پروژه جدید
۷. فرمان اجرای برنامه
۸. فرمان اجرای Migration
۹. فرمان اجرای Testها
۱۰. نتیجه واقعی Restore
۱۱. نتیجه واقعی Build
۱۲. نتیجه واقعی Test
۱۳. نتیجه واقعی Pack
۱۴. نتیجه آزمون پروژه SampleBusiness
۱۵. نتیجه آزمون Module/Command/Query generatorها
۱۶. مشکلات یا محدودیت‌های باقی‌مانده
۱۷. فایل‌ها یا تصمیم‌های مهم معماری
```

نتایج را حدس نزن. فقط نتیجه واقعی فرمان‌هایی که اجرا کرده‌ای گزارش کن.

در پایان، مسیر دقیق فایل `.nupkg` تولیدشده را اعلام کن.

---

# شیوه کار

1. ابتدا Repository را بررسی کن.
2. برنامه اجرایی کوتاه تهیه کن.
3. سپس مستقیماً پیاده‌سازی را آغاز کن.
4. بدون درخواست تأیید مرحله‌ای ادامه بده.
5. بعد از هر بخش مهم Build یا Test اجرا کن.
6. خطاها را واقعاً برطرف کن، نه اینکه Test یا قابلیت مربوطه را حذف کنی.
7. هیچ ادعای موفقیتی بدون اجرای فرمان مربوطه نداشته باش.
8. در صورت شکست یک ابزار خارجی مانند Docker، بخش‌های مستقل را کامل کن و دلیل دقیق شکست را گزارش بده.
9. خروجی نهایی باید برای Commit و استفاده واقعی آماده باشد.
