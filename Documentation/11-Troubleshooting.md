# 11. Troubleshooting

| Symptom | Checks |
|---|---|
| Startup fails with database error | Confirm `ConnectionStrings` is an environment variable visible to the IIS/app process; verify SQL Server, database, credentials, firewall, and schema. |
| Login redirects repeatedly | Check Identity cookie configuration, HTTPS, the `User` session object, session middleware order, and `SessionValidationMiddleware` public-route list. |
| SAML login fails | Verify `SAMLResponse`, `TestCredentials`, certificate paths, certificate permissions, signature certificate, assertion time, and registered Identity user. |
| Permission returns Access Denied | Confirm the user has a valid `unitid`; verify active `UnitClaims` and `PermissionMaster` rows, permission key spelling, claims transformation logs, and cache expiry. |
| Upload fails or returns 413 | Check Kestrel and multipart limits (both are configured for 100 MB), IIS request filtering limits, disk permissions, and free space. |
| Dashboard/chart is empty | Confirm stored procedures and result columns exist and match `ChartModel`/`ChartModelS`; inspect repository exception logging. |
| Project history or movement fails | Verify the required stored procedures and parameters; many procedures are called by code but are not defined in the checked-in SQL script. |
| SignalR does not connect | Confirm HTTPS/WSS, CORS origin, CSP `connect-src`, `/chatHub`, proxy/WebSocket support, and authentication/session state. |
| Publish fails or runs the wrong runtime | Use the project `net9.0` target; review profiles that still contain `TargetFramework=net6.0`. |
| Static files are missing | Verify the publish output includes `wwwroot`, IIS static-file handling, and correct case/path names. |

## Diagnostic locations

Application logging is configured for console output. A database logger model/provider and `dbo.Errors` configuration are present, but the active provider registration is commented in `Program.cs`; do not assume database error logging is active.

## Items requiring external verification

- Current SQL Server schema, stored procedure definitions, indexes, and permissions.
- IIS hosting module, bindings, application-pool identity, and request filtering.
- SAML/IAM certificate installation and trust chain.
- Production secret store and backup/restore procedures.

## Future improvements

- Align or remove stale net6 publish profiles.
- Add OpenAPI documentation or a formal endpoint contract for JSON actions.
- Add automated integration tests against representative SQL procedures and authorization policies.
- Move secrets and certificate paths to strongly typed configuration and a managed secret store.
- Establish a database migration/release process that matches the current model and procedure set.
- Define upload scanning, retention, access control, and cleanup policies.
- Centralize error handling and structured logging, including a deliberate decision on database logging.
