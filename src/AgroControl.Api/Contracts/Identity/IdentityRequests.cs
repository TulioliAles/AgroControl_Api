namespace AgroControl.Api.Contracts.Identity;

public sealed record RegisterRequest(string Name, string Email, string Password);

public sealed record LoginRequest(string Email, string Password);
