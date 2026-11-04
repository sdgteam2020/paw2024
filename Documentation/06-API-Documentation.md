# 6. API Documentation

## API style

No separate REST API project or `ApiController` classes were found. The application uses MVC controller actions that return Razor views, partial views, JSON, files, redirects, and `Ok(...)` results. Exact routes follow conventional routing unless an action has an explicit route attribute.

Default MVC route:

```text
{controller=Home}/{action=Index}/{id?}
```

## Important endpoint groups

| Area | Representative controller/actions | Response/use |
|---|---|---|
| Authentication | `AccountController`, Identity `/Identity/Account/*` | Redirects, forms, session and Identity cookies |
| Projects | `ProjectsController` | Views, JSON, partials, uploads, reports |
| Dashboard | `HomeController` | Views, dashboard JSON/chart calls |
| Master data | Actions, Stages, Status, Ddl, Rank, SoftwareType, StakeHolder, Unit controllers | CRUD views and JSON/partials |
| Permissions | PermissionMaster, RolePermission, UserPermission, UnitPermission, PermissionControl | Admin views, partials, JSON save/load |
| Chat | `ChatController` | JSON user/chat operations |
| Audit/history | AuditLog, AttHistory, Search controllers | Partial views and history views |
| Certificates | `CertificateController` | PDF/file/JSON responses |

## SignalR

The hub is mapped at `/chatHub`. `ChatHub.SendMessage` broadcasts `ReceiveMessage` to all clients. `SendMessageToUser` sends `ReceiveMessage` to a selected SignalR user.

## Request protections

Global MVC registration adds `AutoValidateAntiforgeryTokenAttribute` and `SanitizeActionFilter`. Razor Pages also receive antiforgery validation. JSON actions should therefore send the configured antiforgery token header when required; the CORS policy allows `RequestVerificationToken`.

## Contract limitations

The repository does not provide an OpenAPI document, formal API versioning, or a complete request/response catalog. Consumers should inspect the specific controller action and JavaScript caller for field names and payload formats.
