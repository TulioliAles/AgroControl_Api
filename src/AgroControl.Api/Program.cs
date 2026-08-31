var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Ok(new
{
    application = "AgroControl API",
    status = "running",
    environment = app.Environment.EnvironmentName
}));

app.Run();

public partial class Program;
