# 3. Project Structure

## Solution

```text
swas.sln
├── swas.UI
├── swas.BAL
└── swas.DAL
```

## `swas.UI`

| Path | Responsibility |
|---|---|
| `Controllers` | MVC request handling, JSON/partial responses, redirects, file responses |
| `Views` | Razor MVC pages and shared partials/layouts |
| `Areas/Identity/Pages` | Identity login, registration, account management, lockout, and confirmation pages |
| `Program.cs` | Dependency injection, middleware, authentication, authorization, endpoints |
| `Middleware` | Security headers and application-session validation |
| `Helpers` / `Services` | Permission authorization, claims transformation, encryption helpers and UI support |
| `Models` | UI-specific models and PDF builder |
| `Saml` | SAML response parsing/signature validation support |
| `wwwroot` | JavaScript, CSS, images, fonts, PDFs, upload/temp folders and client libraries |
| `appsettings*.json` | Non-secret application settings and logging configuration |
| `Properties` | Launch settings and many filesystem publish profiles |

## `swas.BAL`

- `Interfaces`: repository/service contracts.
- `Repository`: implementations for projects, actions, status, stakeholders, chat, permissions, audit, certificates, notifications, and related workflows.
- `DTO`: transport and report models.
- `Helpers`: session, dropdown, crypto, role, file, sanitization, and business helpers.
- `Utility`: SignalR hub, policy/session/security utilities, enums, and extensions.
- `Exceptions`: application exception helpers.

## `swas.DAL`

- `ApplicationDbContext.cs`: Identity DbContext and application `DbSet`s.
- `ApplicationUser.cs`: Identity user extension.
- `Models`: persistence and report models.
- `Migrations`: EF Core initial migration and model snapshot.
- `Logger`: database log model/provider types.
- `Mapper`: keyless/query mapping types.

Generated folders (`bin`, `obj`) and third-party static assets are not architectural source and are excluded from this structure summary.
