using AgroControl.Api.Authentication;
using AgroControl.Api.Endpoints;
using AgroControl.Api.Errors;
using AgroControl.Api.Observability;
using AgroControl.Application.Catalog.CreateAgriculturalInput;
using AgroControl.Application.Catalog.CreateReferenceData;
using AgroControl.Application.Catalog.GetAgriculturalInputs;
using AgroControl.Application.Catalog.MaintainReferenceData;
using AgroControl.Application.Catalog.UpdateAgriculturalInput;
using AgroControl.Application.Identity;
using AgroControl.Application.Inventory;
using AgroControl.Infrastructure;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.AddObservability();

    builder.Services.AddOpenApi();
    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddHealthChecks();
    builder.Services.AddJwtAuthentication(builder.Configuration);
    builder.Services.AddScoped<RegisterUserHandler>();
    builder.Services.AddScoped<LoginHandler>();
    builder.Services.AddScoped<CreateAgriculturalInputHandler>();
    builder.Services.AddScoped<GetAgriculturalInputByIdHandler>();
    builder.Services.AddScoped<ListAgriculturalInputsHandler>();
    builder.Services.AddScoped<UpdateAgriculturalInputHandler>();
    builder.Services.AddScoped<ChangeAgriculturalInputStatusHandler>();
    builder.Services.AddScoped<CreateInputCategoryHandler>();
    builder.Services.AddScoped<CreateManufacturerHandler>();
    builder.Services.AddScoped<CreateMeasurementUnitHandler>();
    builder.Services.AddScoped<InputCategoryMaintenanceHandler>();
    builder.Services.AddScoped<ManufacturerMaintenanceHandler>();
    builder.Services.AddScoped<MeasurementUnitMaintenanceHandler>();
    builder.Services.AddScoped<CreateStockLotHandler>();
    builder.Services.AddScoped<RecordStockMovementHandler>();
    builder.Services.AddScoped<StockLotQueryHandler>();
    builder.Services.AddInfrastructure(builder.Configuration);

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi().AllowAnonymous();
    }

    app.UseObservability();
    app.UseExceptionHandler();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHealthChecks("/health").AllowAnonymous();
    app.MapIdentityEndpoints();
    app.MapAgriculturalInputEndpoints();
    app.MapCatalogReferenceDataEndpoints();
    app.MapStockLotEndpoints();

    if (app.Environment.IsEnvironment("IntegrationTests"))
    {
        app.MapIntegrationTestEndpoints();
    }

    app.MapGet("/", () => Results.Ok(new
    {
        service = "AgroControl.Api",
        status = "running",
        version = "v1"
    })).AllowAnonymous();

    Log.Information("Starting AgroControl API");
    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "AgroControl API terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;
