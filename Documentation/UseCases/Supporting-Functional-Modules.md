# Supporting Functional Modules

This document covers implemented modules that support the project lifecycle but are not separate lifecycle stages.

## UC-08: Documents and attachments

`ProjectsController` and `DocumentsController` expose document-type lookup, upload/history, and document retrieval. `AttHistory` links evidence to a movement using `PsmId`, stores generated path, original file name, remarks, action/timestamp, active/deleted flags, and optional `DocumentTypeId`. Forwarding accepts PDF files only, limits size to 10 MB, validates the PDF signature, writes to `wwwroot/Uploads`, and inserts metadata. Submission compares active required `DocumentTypes` with uploaded document types for the current PSM. The source does not establish a cleanup/retention policy.

## UC-11: Date approval and legacy history

When `UpdateProjectAsync` sees an editable date change on a submitted project (`Date_type == 1`, submitted, dates not locked, and remarks not `"1"`), it inserts or updates `DateApproval` with project/unit/request date/user request/read/request type. It also inserts or updates the latest `LegacyHistory` row with action type `1`, remarks, actor and date. The approval decision and downstream date mutation are not fully visible in the checked-in source.

## UC-12: Dashboard, reports, history, and search

`HomeController`, `SearchController`, project history actions, and JavaScript dashboard scripts display counts, charts, current searches, project history, project-wise status, delayed/complete/draft/inbox/outbox/CC views, and log reports. BAL services call configured procedures including `usp_DashboardCount`, `usp_DashboardChartSummary`, `getMonthInOutSummary`, `getAppTypeCounts`, `usp_GetProjectWiseStatus`, `GetProjectHistoryByID_Dapper`, and `usp_GetProjLogView`. The two chart procedures defined locally aggregate `Projects`, `ProjStakeHolderMov`, and `mAppType`; many other result contracts come only from deployed SQL.

## UC-13: Master data and workflow configuration

Controllers/repositories exist for actions, stages, statuses, stakeholders, units, ranks, software types, dropdowns, and unit-status/mapping data. These screens provide the values used by project forms and movement display. Status model validation requires a stage, status text, sequence, and update user; action model validation requires action text. CRUD semantics are generally EF repository operations with active/deleted flags. A configuration change can alter future workflow behavior; historical movement rows retain their mapping ID.

## UC-14: Permissions administration

Permission master, role permission, user permission, unit permission, and permission-control controllers expose list/save/partial views. Runtime authorization is based on active unit claims and permission keys; the source does not establish a complete role-to-permission seed matrix.

## UC-15: Notifications, reminders, and chat

Notification and reminder repositories support notification persistence, reminder creation/read updates, and project reminder history. Forwarding and pullback update reminder read status in specific branches. `ChatController`/`ChatRepository` persist user mappings and messages; `ChatHub` broadcasts SignalR `ReceiveMessage` notifications. These are collaboration functions and do not themselves establish a project status transition.

## UC-16: Whitelist and certificates

`CertificateController`, `CertificateService`, and `PdfCertificateBuilder` generate/read certificate content. Forwarding mapping IDs 78 and 88 perform direct whitelist updates when a matching `trnWhiteListed` row exists. `GetWhiteListedActionProj` optionally invokes `InsertWhiteListedProjects` when the `AllowWhiteListedProjSync` environment variable equals `1`, then reads `GetLatestWhiteListedProjects(@TypeId)`. Procedure definitions and certificate eligibility rules are not fully present locally.

## UC-17: Audit

Movement edits create `AuditLog` rows containing serialized old/new values and the authenticated username/time. `AuditlogRepository` also calls `USP_GetAuditLogHistory` and maps change flags into field-level history. The deployed procedure is required to verify full audit coverage beyond movement edits.
