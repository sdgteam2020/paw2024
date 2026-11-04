# PAW Use Case Documentation

## Scope and evidence standard

This package documents behavior verified in the PAW source tree (`swas.UI`, `swas.BAL`, `swas.DAL`) and `swas.UI/data_swas_new.sql`. It follows the implementation from Razor/MVC UI to controller, BAL repository/service, EF Core/Dapper/ADO.NET, stored-procedure reference, and database entity. It does not treat undocumented deployed SQL behavior as fact.

**Important limitation:** the application references many stored procedures, but the checked-in SQL file defines only a subset. Procedure result columns, hidden transition rules, and production seed mappings must be verified against the deployed `DBSWAS` database.

## Document index

| Document | Coverage |
|---|---|
| [PAW overall workflow](PAW-Overall-Workflow.md) | Architecture, actors, end-to-end lifecycle, sequence/flow diagrams |
| [Roles and responsibilities](User-Roles-and-Responsibilities.md) | Identity, roles, units, claims, permissions, responsibility routing |
| [Project lifecycle](Project-Lifecycle.md) | Lifecycle states, movement records, flags, and verified/unknown transitions |
| [Creation and submission](Project-Creation-and-Submission.md) | Form validation, project persistence, attachments, submission gate |
| [Forwarding](Project-Forwarding-Workflow.md) | Unit-to-unit movement, CC, attachments, inbox completion |
| [Approval and rejection](Approval-and-Rejection-Workflow.md) | Action/mapping-driven decisions, approval checks, whitelist effects, gaps |
| [Observation and resubmission](Observation-and-Resubmission.md) | Comments, pullback/undo, correction and resubmission behavior |
| [Closure and reopening](Project-Closure-and-Reopening.md) | Foreclosure and reopening transactions |
| [Status/action mapping](Status-and-Action-Mapping.md) | Actual entity relationships, known IDs, mapping limitations |
| [Database workflow](Database-Workflow.md) | Tables, joins, writes, stored procedures, transactions |
| [Exceptions and alternatives](Exception-and-Alternative-Scenarios.md) | Validation, authorization, error and alternative paths |

## Use-case catalogue

| ID | Use case | Implementation evidence |
|---|---|---|
| UC-01 | Authenticate and establish unit permissions | ASP.NET Core Identity pages, `Program.cs`, `UnitClaimsTransformation` |
| UC-02 | Create or edit project | `ProjectsController.AddProject`, `ProjectsRepository` |
| UC-03 | Submit or return project to draft | `ProjectsController.ProjectSubmited` |
| UC-04 | Forward project to a unit | `ProjectsController.FwdToProject`, movement repositories |
| UC-05 | Process/comment routing | `ProcessMail`, `ProcessNotification`, comment repositories |
| UC-06 | Mark movement read/complete | `IsReadInbox`, `FwdProjConfirm` |
| UC-07 | Pull back or undo movement | `PullBAckProject`, `UndoProject` |
| UC-08 | Add and view project documents | attachment actions, `AttHistory`, `DocumentsController` |
| UC-09 | Check approvals and perform decision actions | `CheckFwdCondition`, `CheckPreviousApprovals`, mappings |
| UC-10 | Close or reopen project | foreclosure controller/repository methods |
| UC-11 | Request date approval | `UpdateProjectAsync`, `DateApproval`, `LegacyHistory` |
| UC-12 | View history, dashboards, reports, and search | history/report procedures and `HomeController`/`SearchController` |
| UC-13 | Maintain master data and workflow configuration | actions, stages, statuses, units, stakeholders, mappings |
| UC-14 | Administer permissions | permission master, role/user/unit permission controllers |
| UC-15 | Communicate and notify | chat, notifications, reminders, SignalR hub |
| UC-16 | Generate/view whitelist certificates | certificate service/controller and whitelist updates |

## Reading conventions

- **Verified** means directly visible in code or checked-in SQL.
- **Referenced** means a procedure/table is called or modeled but its deployed definition is not available locally.
- **Not established** means the source does not prove the claimed business meaning.
