# 1. Project Overview

## Purpose

SWAS is an authenticated web application for managing software-whitelisting/project workflows. The implemented UI contains project creation and movement, stages/status/actions, stakeholder coordination, document and attachment handling, dashboards/reports, audit history, permissions, notifications, reminders, certificates, and chat features.

The exact organizational meaning of every workflow state is not fully defined in the repository; the descriptions above are based on controller, view, model, and repository names.

## Technology baseline

| Area | Verified implementation |
|---|---|
| Runtime | .NET 9 (`net9.0`) |
| Web host | ASP.NET Core web project with MVC controllers, Razor Views, and Identity Razor Pages |
| Database | SQL Server through EF Core 9, ADO.NET, and Dapper |
| UI | Razor Views/Pages, JavaScript, jQuery, Bootstrap, DataTables, PDF assets |
| Authentication | ASP.NET Core Identity; an additional SAML response path exists |
| Real-time | SignalR hub at `/chatHub` |
| Documents | File uploads and iText-based PDF/certificate code |

## Solution composition

- `swas.UI`: executable web host and presentation layer.
- `swas.BAL`: interfaces, DTOs, helpers, and repositories/services.
- `swas.DAL`: EF Core context, Identity user type, models, migration, logging, and mapper types.

## Typical request flow

A browser request reaches an MVC controller or Identity Razor Page, which uses a BAL interface. The repository/service uses `ApplicationDbContext`, ADO.NET, or Dapper against SQL Server and returns a view model, partial view, JSON response, file, or redirect.
