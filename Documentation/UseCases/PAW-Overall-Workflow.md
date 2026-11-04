# PAW Overall Workflow

## Purpose

PAW is an authenticated ASP.NET Core application for registering software/application projects, routing them between organizational units, recording actions and comments, storing evidence, and tracking closure/whitelist outcomes.

## Layered execution model

```mermaid
sequenceDiagram
	actor U as Authenticated user
	participant V as Razor/MVC view + JavaScript
	participant C as MVC controller / Identity page
	participant B as BAL repository/service
	participant D as EF Core / Dapper / ADO.NET
	participant S as SQL Server
	U->>V: Complete form or select workflow action
	V->>C: HTTP GET/POST/JSON (often antiforgery + protected IDs)
	C->>C: Validate model, session, claims and business fields
	C->>B: Delegate application operation
	B->>D: Entity query/update or stored procedure call
	D->>S: SQL command
	S-->>D: Entity rows/result sets
	D-->>B: Entity/DTO/result
	B-->>C: Success, failure or mapped result
	C-->>V: View, JSON, redirect or file
```

## Main workflow

```mermaid
flowchart TD
	A[Identity login] --> B[Unit and permission claims loaded]
	B --> C[Create project draft]
	C --> D{Submit?}
	D -- No --> C
	D -- Yes --> E[Required document check]
	E -- Missing --> C
	E -- Complete --> F[Projects.IsSubmited = true]
	F --> G[Initial/current movement]
	G --> H[Responsible unit inbox]
	H --> I{Action selected from status/action mapping}
	I --> J[Forward / comment / approve / reject / pullback]
	J --> K[New or updated ProjStakeHolderMov row]
	K --> L{Further routing?}
	L -- Yes --> H
	L -- No --> M[Close/foreclose]
	M --> N{Reopen?}
	N -- Yes --> O[Reopen foreclosure record]
	N -- No --> P[Project remains closed]
```

## Major actors

| Actor | Evidence-backed responsibility |
|---|---|
| Authenticated Identity user | Signs in and receives an application session. |
| Unit/stakeholder user | Creates projects, receives movements, forwards, comments, uploads evidence, and may request date changes subject to permissions. |
| Dte / central unit | Special processing paths are restricted in code to unit IDs 1 or 2 for `ProcessMail`; exact organizational meaning is data-driven. |
| Receiving unit | Determined by `ProjStakeHolderMov.ToUnitId`; can read, process, forward, comment, or pull back according to available actions and authorization. |
| Application administrator | Maintains actions, stages, statuses, units, stakeholders, mappings, and permissions through administration controllers. |
| SQL Server | Stores workflow entities and supplies configured procedure results/mappings. |

## Verified boundaries

The source proves movement rows and their flags, but does not provide a complete business transition matrix. Status descriptions, action descriptions, and mapping IDs are database data. The local SQL script contains legacy/core schema and some reporting procedures, but not all procedures referenced by the current repositories.
