# SWAS Technical Documentation

This documentation describes the repository implementation as inspected on the current `dependency-updates-net9` branch.

## Contents

1. [Project Overview](01-Project-Overview.md)
2. [System Architecture](02-System-Architecture.md)
3. [Project Structure](03-Project-Structure.md)
4. [Module Documentation](04-Module-Documentation.md)
5. [Database Documentation](05-Database-Documentation.md)
6. [API Documentation](06-API-Documentation.md)
7. [Business Logic](07-Business-Logic.md)
8. [Security Documentation](08-Security-Documentation.md)
9. [Configuration Guide](09-Configuration-Guide.md)
10. [Deployment Guide](10-Deployment-Guide.md)
11. [Troubleshooting](11-Troubleshooting.md)

## Verified scope and limitations

The repository contains three .NET 9 projects, application source, a checked-in SQL script, and an EF Core migration. The live SQL Server schema, all stored-procedure bodies, production secrets, IIS server configuration, and external IAM deployment were not available for verification. Those limits are explicitly called out in the individual documents.
