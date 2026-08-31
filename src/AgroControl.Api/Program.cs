using AgroControl.Api.Endpoints;
using AgroControl.Application.Catalog.CreateAgriculturalInput;
using AgroControl.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddScoped<CreateAgriculturalInputHandler>();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.MapHealthChecks("/health");
app.MapAgriculturalInputEndpoints();
app.MapGet("/", () => Results.Ok(new
{
    service = "AgroControl.Api",
    status = "running",
    version = "v1"
}));

app.Run();

public partial class Program;
