# 4. Module Documentation

## Project workflow

`ProjectsController`, `ProjectsRepository`, project models, stakeholder movement repositories, and project views form the main business area. The code supports project creation/editing, forwarding between units/stakeholders, project history, pending/delayed views, comments, attachments, whitelisting, dashboards, and reports. The exact state-transition rules are largely implemented in stored procedures and repository methods; the live procedure bodies were not available.

## Master data

Controllers and repositories exist for actions, stages, statuses, software types, ranks, units, stakeholders, dropdown values, and status/action mappings. These modules supply the project forms and workflow configuration.

## Permissions and administration

Permission master, role permission, user permission, unit permission, and permission-control controllers expose administrative screens and JSON/partial actions. Permission keys are loaded from `UnitClaims` and evaluated as claims.

## Documents and certificates

`DocumentsController`, attachment models, upload paths under `wwwroot`, and project views support document access/history. `CertificateController`, `CertificateService`, `PdfCertificateBuilder`, and iText libraries generate or process PDF certificates. Repository inspection does not establish a complete retention or cleanup policy for uploaded files.

## Dashboard, history, and audit

`HomeController` loads dashboard/project data and charts. `ChartService` invokes `getMonthInOutSummary` and `getAppTypeCounts`. `AuditlogRepository` invokes `USP_GetAuditLogHistory` and maps change flags into readable field changes.

## Chat and notifications

`ChatController` exposes MVC JSON actions for user mapping and chat history. `ChatHub` broadcasts messages to all clients or sends to a specific SignalR user. Notifications and reminders have dedicated repositories and models.

## Identity

Identity Razor Pages implement login, registration, email confirmation, password management, lockout, two-factor management, external-login scaffolding, and personal-data operations. `AccountController` also contains application-specific login/logout and user administration paths.
