# AgroControl.Api.IntegrationTests

Integration tests start a disposable SQL Server container, apply Entity Framework migrations, boot the real API with `WebApplicationFactory`, call the HTTP endpoints and verify persisted data.

## Requirements

- Docker Desktop or another compatible Docker engine running.
- .NET 10 SDK.

Run with:

```powershell
dotnet test tests/AgroControl.Api.IntegrationTests
```
