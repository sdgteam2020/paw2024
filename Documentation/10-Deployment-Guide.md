# 10. Build, Publish, and IIS Deployment

## Build

From the solution root:

```powershell
dotnet restore swas.sln
dotnet build swas.sln --configuration Release
```

The solution targets .NET 9, so install the .NET 9 SDK and the ASP.NET Core Hosting Bundle/runtime on an IIS server.

## Publish

A filesystem publish can be produced with:

```powershell
dotnet publish swas.UI\swas.UI.csproj --configuration Release --output .\publish
```

The repository contains many filesystem profiles. Some point to `C:\PAWPUB`; one points to `C:\inetpub\wwwroot\swas`. However, the checked-in profiles declare `net6.0` while the project declares `net9.0`. This mismatch must be resolved before relying on a profile; the command above uses the project target directly.

## IIS deployment checklist

1. Install the .NET 9 ASP.NET Core Hosting Bundle.
2. Publish the `swas.UI` project in Release mode.
3. Copy the publish output to the IIS site directory.
4. Configure an IIS site/application pool using the ASP.NET Core Module and a compatible process identity.
5. Set `ConnectionStrings`, `TestCredentials`, certificate locations, allowed origins, and other secrets on the server.
6. Ensure the application identity can read the certificate and write only to intended upload/temp directories.
7. Configure HTTPS certificates and bindings.
8. Verify SQL Server connectivity and required stored procedures.
9. Test login, session expiry, permissions, project upload, dashboards, chat, and certificate operations.
10. Review logs after startup and first authenticated request.

No committed `web.config`, IIS binding file, application-pool settings, or production runbook was found; the exact server configuration must be supplied by operations.
