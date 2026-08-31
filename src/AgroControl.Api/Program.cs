using AgroControl.Api.Endpoints;
using AgroControl.Application.Catalog.CreateAgriculturalInput;
using AgroControl.Application.Catalog.CreateReferenceData;
using AgroControl.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
builder.Services.AddScoped<CreateAgriculturalInputHandler>();
builder.Services.AddScoped<CreateInputCategoryHandler>();
builder.Services.AddScoped<CreateManufacturerHandler>();
builder.Services.AddScoped<CreateMeasurementUnitHandler>();
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
app.MapCatalogReferenceDataEndpoints();
app.MapGet("/", () => Results.Ok(new
{
    service = "AgroControl.Api",
    status = "running",
    version = "v1"
}));

app.Run();

public partial class Program;
