# 8. Security Documentation

## Authentication

ASP.NET Core Identity uses `ApplicationUser` and `ApplicationDbContext`. Cookies use `/Identity/Account/Login` and `/Identity/Account/AccessDenied` as login/denied paths. The configured lockout is 60 minutes after three failed attempts. Confirmed account sign-in is required; confirmed email sign-in is not required. The login Razor Page also supports encrypted client inputs before Identity password sign-in.

A separate `AccountController.Login` path accepts a SAML response, decrypts it, validates the signature/conditions using configured certificate files and the `SAMLResponse` data, signs in the corresponding Identity user, writes login/session data, and redirects. Paths and credentials are environment/server dependent and must not be copied from source into documentation or deployment scripts.

## Authorization

- `[Authorize]` protects controllers and actions.
- Role policies: `Admin` requires `Dte`; `Unit` requires `Unit`; `StakeHolders` requires either role.
- `AuthorizePermission` evaluates dynamic `Permission:<key>` policies.
- Several administrative actions explicitly require the `Dte` role.
- `SessionValidationMiddleware` additionally requires an application session object named `User` for non-public routes.

## Transport and browser protections

The application enables HTTPS redirection, secure/HTTP-only cookies, strict session SameSite behavior, HSTS configuration, antiforgery validation, a restricted CORS origin list, request-size limits, and response headers including CSP, `X-Content-Type-Options`, `X-Frame-Options`, Referrer-Policy, and Permissions-Policy. Dangerous HTTP methods TRACE, TRACK, and CONNECT are rejected.

## Security considerations

The repository contains certificate paths, SAML-related files, encrypted-input code, upload/temp directories, and a checked-in SQL script. Treat all credentials, certificate passwords, connection strings, uploaded files, and private keys as deployment secrets. The code's CryptoJS-compatible AES routine derives keys using the legacy MD5-based OpenSSL-compatible process; review this design before extending it. This document does not reproduce any secret values.
