using AgroControl.Api.Contracts.Identity;
using AgroControl.Api.Extensions;
using AgroControl.Api.Validation;
using AgroControl.Application.Identity;

namespace AgroControl.Api.Endpoints;

public static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/register", RegisterAsync)
            .Validate<RegisterRequest>()
            .WithName("RegisterUser")
            .AllowAnonymous()
            .Produces<RegisteredUserResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/login", LoginAsync)
            .Validate<LoginRequest>()
            .WithName("Login")
            .AllowAnonymous()
            .Produces<AccessTokenResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem();

        return endpoints;
    }

    private static async Task<IResult> RegisterAsync(
        RegisterRequest request,
        RegisterUserHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new RegisterUserCommand(request.Name, request.Email, request.Password),
            cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/users/{result.Value.Id}", result.Value)
            : result.Error.ToProblemResult();
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        LoginHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new LoginCommand(request.Email, request.Password),
            cancellationToken);

        return result.IsSuccess
            ? Results.Ok(result.Value)
            : result.Error.ToProblemResult();
    }
}
