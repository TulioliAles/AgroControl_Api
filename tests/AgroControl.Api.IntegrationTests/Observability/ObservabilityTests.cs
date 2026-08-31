using System.Data;
using System.Net;
using System.Text.Json;
using AgroControl.Api.IntegrationTests.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace AgroControl.Api.IntegrationTests.Observability;

public sealed class ObservabilityTests(AgroControlApiFactory factory)
    : IClassFixture<AgroControlApiFactory>
{
    [Fact]
    public async Task UnhandledException_ShouldReturnProblemDetailsAndPersistCorrelatedErrorLog()
    {
        var correlationId = $"integration-{Guid.NewGuid():N}";
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "/api/integration-tests/unhandled-exception");
        request.Headers.Add("X-Correlation-ID", correlationId);

        var response = await factory.Client.SendAsync(request);

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.True(response.Headers.TryGetValues("X-Correlation-ID", out var values));
        Assert.Contains(correlationId, values);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;
        Assert.Equal("Unexpected error", root.GetProperty("title").GetString());
        Assert.Equal(500, root.GetProperty("status").GetInt32());
        Assert.Equal("Api.UnexpectedError", root.GetProperty("code").GetString());
        Assert.Equal(correlationId, root.GetProperty("correlationId").GetString());

        var logWasPersisted = await WaitForErrorLogAsync(correlationId);

        Assert.True(logWasPersisted);
    }

    private async Task<bool> WaitForErrorLogAsync(string correlationId)
    {
        var deadline = DateTimeOffset.UtcNow.AddSeconds(15);

        while (DateTimeOffset.UtcNow < deadline)
        {
            var exists = await factory.ExecuteDbContextAsync(async dbContext =>
            {
                var connection = dbContext.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                await using var command = connection.CreateCommand();
                command.CommandText = """
                    SELECT COUNT(1)
                    FROM [dbo].[ApplicationLogs]
                    WHERE [CorrelationId] = @correlationId
                      AND [Level] = 'Error';
                    """;

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@correlationId";
                parameter.Value = correlationId;
                command.Parameters.Add(parameter);

                var count = Convert.ToInt32(await command.ExecuteScalarAsync());
                return count > 0;
            });

            if (exists)
            {
                return true;
            }

            await Task.Delay(250);
        }

        return false;
    }
}
