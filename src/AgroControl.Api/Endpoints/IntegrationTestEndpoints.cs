namespace AgroControl.Api.Endpoints;

public static class IntegrationTestEndpoints
{
    public static IEndpointRouteBuilder MapIntegrationTestEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
            "/api/integration-tests/unhandled-exception",
            ThrowUnhandledException);

        return endpoints;
    }

    private static IResult ThrowUnhandledException() =>
        throw new InvalidOperationException("Intentional integration test exception.");
}
