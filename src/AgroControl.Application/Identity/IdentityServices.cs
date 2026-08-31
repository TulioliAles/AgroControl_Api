using AgroControl.Application.Abstractions.Data;
using AgroControl.Domain.Common;
using AgroControl.Domain.Identity;

namespace AgroControl.Application.Identity;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
    void Add(User user);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}

public interface IAccessTokenProvider
{
    AccessTokenResponse Create(User user);
}

public sealed record RegisterUserCommand(string Name, string Email, string Password);
public sealed record LoginCommand(string Email, string Password);
public sealed record RegisteredUserResponse(Guid Id);
public sealed record AccessTokenResponse(string AccessToken, DateTimeOffset ExpiresAt);

public static class IdentityErrors
{
    public static readonly Error EmailAlreadyExists = Error.Conflict(
        "Identity.User.EmailAlreadyExists",
        "A user with this email address already exists.");

    public static readonly Error InvalidCredentials = Error.Validation(
        "Identity.Login.InvalidCredentials",
        "The email address or password is invalid.");

    public static readonly Error InactiveUser = Error.Validation(
        "Identity.Login.InactiveUser",
        "The user account is inactive.");
}

public sealed class RegisterUserHandler(
    IUserRepository repository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork)
{
    public async Task<Result<RegisteredUserResponse>> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (string.IsNullOrWhiteSpace(command.Password) || command.Password.Length < 8)
        {
            throw new ArgumentException(
                "The password must contain at least 8 characters.",
                nameof(command.Password));
        }

        if (await repository.ExistsByEmailAsync(command.Email, cancellationToken))
        {
            return Result<RegisteredUserResponse>.Failure(IdentityErrors.EmailAlreadyExists);
        }

        var user = User.Create(
            command.Name,
            command.Email,
            passwordHasher.Hash(command.Password));

        repository.Add(user);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<RegisteredUserResponse>.Success(new(user.Id));
    }
}

public sealed class LoginHandler(
    IUserRepository repository,
    IPasswordHasher passwordHasher,
    IAccessTokenProvider accessTokenProvider)
{
    public async Task<Result<AccessTokenResponse>> HandleAsync(
        LoginCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var user = await repository.GetByEmailAsync(
            command.Email.Trim().ToLowerInvariant(),
            cancellationToken);

        if (user is null || !passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            return Result<AccessTokenResponse>.Failure(IdentityErrors.InvalidCredentials);
        }

        if (!user.IsActive)
        {
            return Result<AccessTokenResponse>.Failure(IdentityErrors.InactiveUser);
        }

        return Result<AccessTokenResponse>.Success(accessTokenProvider.Create(user));
    }
}
