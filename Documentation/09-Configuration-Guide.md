# 9. Configuration Guide

## Configuration sources

`Program.cs` reads the database connection from the process environment variable `ConnectionStrings` (not from the commented `ConnectionStrings:DB` configuration key). Standard ASP.NET Core configuration also loads `appsettings.json`, environment-specific settings, command-line values, and environment variables.

| Setting | Source | Purpose |
|---|---|---|
| `ConnectionStrings` | Environment variable | SQL Server connection string; value intentionally omitted |
| `ASPNETCORE_ENVIRONMENT` | Launch/deployment environment | Selects environment behavior |
| `TestCredentials` | Environment variable | Referenced by the SAML login path; secret value omitted |
| `SiteSettings` | `appsettings.json` | Banner messages |
| `NotificationContent` | `appsettings.json` | Project notification title and HTML |
| Logging settings | `appsettings*.json` | Console/database logging category levels and field names |

## Local run

The launch profile sets `ASPNETCORE_ENVIRONMENT=Development` and uses HTTPS URLs `https://localhost:7152` and `https://localhost:5105`. IIS Express is also configured for local development. These values are development-only evidence, not production requirements.

## Secrets and deployment values

Set SQL credentials, SAML credentials, certificate locations, encryption keys, allowed origins, and any mail/IAM values through a protected secret store or server environment. Do not commit them to JSON, publish profiles, source, or this documentation. Verify that the production certificate paths used by SAML are valid on the target server.

## Configuration caveats

The checked-in `appsettings.json` contains operational banner and HTML content. Review this content per environment. No complete production configuration contract was found in the repository.
