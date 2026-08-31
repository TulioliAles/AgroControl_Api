using AgroControl.Api.Contracts.Catalog;
using AgroControl.Api.Extensions;
using AgroControl.Api.Validation;
using AgroControl.Application.Catalog.CreateReferenceData;
using AgroControl.Application.Catalog.MaintainReferenceData;

namespace AgroControl.Api.Endpoints;

public static class CatalogReferenceDataEndpoints
{
    public static IEndpointRouteBuilder MapCatalogReferenceDataEndpoints(this IEndpointRouteBuilder endpoints)
    {
        MapInputCategories(endpoints.MapGroup("/api/input-categories").WithTags("Input Categories"));
        MapManufacturers(endpoints.MapGroup("/api/manufacturers").WithTags("Manufacturers"));
        MapMeasurementUnits(endpoints.MapGroup("/api/measurement-units").WithTags("Measurement Units"));
        return endpoints;
    }

    private static void MapInputCategories(RouteGroupBuilder group)
    {
        group.MapGet("/", ListInputCategoriesAsync);
        group.MapGet("/{id:guid}", GetInputCategoryAsync).ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPost("/", CreateInputCategoryAsync)
            .Validate<CreateInputCategoryRequest>()
            .Produces<CreateCatalogReferenceResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
        group.MapPut("/{id:guid}", UpdateInputCategoryAsync)
            .Validate<UpdateInputCategoryRequest>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();
        group.MapPatch("/{id:guid}/activate", ActivateInputCategoryAsync).Produces(StatusCodes.Status204NoContent);
        group.MapPatch("/{id:guid}/deactivate", DeactivateInputCategoryAsync).Produces(StatusCodes.Status204NoContent);
    }

    private static void MapManufacturers(RouteGroupBuilder group)
    {
        group.MapGet("/", ListManufacturersAsync);
        group.MapGet("/{id:guid}", GetManufacturerAsync).ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPost("/", CreateManufacturerAsync)
            .Validate<CreateManufacturerRequest>()
            .Produces<CreateCatalogReferenceResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
        group.MapPut("/{id:guid}", UpdateManufacturerAsync)
            .Validate<UpdateManufacturerRequest>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();
        group.MapPatch("/{id:guid}/activate", ActivateManufacturerAsync).Produces(StatusCodes.Status204NoContent);
        group.MapPatch("/{id:guid}/deactivate", DeactivateManufacturerAsync).Produces(StatusCodes.Status204NoContent);
    }

    private static void MapMeasurementUnits(RouteGroupBuilder group)
    {
        group.MapGet("/", ListMeasurementUnitsAsync);
        group.MapGet("/{id:guid}", GetMeasurementUnitAsync).ProducesProblem(StatusCodes.Status404NotFound);
        group.MapPost("/", CreateMeasurementUnitAsync)
            .Validate<CreateMeasurementUnitRequest>()
            .Produces<CreateCatalogReferenceResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
        group.MapPut("/{id:guid}", UpdateMeasurementUnitAsync)
            .Validate<UpdateMeasurementUnitRequest>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();
        group.MapPatch("/{id:guid}/activate", ActivateMeasurementUnitAsync).Produces(StatusCodes.Status204NoContent);
        group.MapPatch("/{id:guid}/deactivate", DeactivateMeasurementUnitAsync).Produces(StatusCodes.Status204NoContent);
    }

    private static ReferenceDataQuery ToQuery(ReferenceDataListRequest request) =>
        new(request.Page, request.PageSize, request.Search, request.IsActive);

    private static async Task<IResult> ListInputCategoriesAsync([AsParameters] ReferenceDataListRequest request, InputCategoryMaintenanceHandler handler, CancellationToken ct) =>
        Results.Ok(await handler.ListAsync(ToQuery(request), ct));

    private static async Task<IResult> GetInputCategoryAsync(Guid id, InputCategoryMaintenanceHandler handler, CancellationToken ct)
    {
        var result = await handler.GetAsync(id, ct);
        return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToProblemResult();
    }

    private static async Task<IResult> UpdateInputCategoryAsync(Guid id, UpdateInputCategoryRequest request, InputCategoryMaintenanceHandler handler, CancellationToken ct)
    {
        var result = await handler.UpdateAsync(new(id, request.Name, request.Description), ct);
        return result.IsSuccess ? Results.NoContent() : result.Error.ToProblemResult();
    }

    private static Task<IResult> ActivateInputCategoryAsync(Guid id, InputCategoryMaintenanceHandler handler, CancellationToken ct) => ChangeStatusAsync(handler.ActivateAsync(id, ct));
    private static Task<IResult> DeactivateInputCategoryAsync(Guid id, InputCategoryMaintenanceHandler handler, CancellationToken ct) => ChangeStatusAsync(handler.DeactivateAsync(id, ct));

    private static async Task<IResult> ListManufacturersAsync([AsParameters] ReferenceDataListRequest request, ManufacturerMaintenanceHandler handler, CancellationToken ct) =>
        Results.Ok(await handler.ListAsync(ToQuery(request), ct));

    private static async Task<IResult> GetManufacturerAsync(Guid id, ManufacturerMaintenanceHandler handler, CancellationToken ct)
    {
        var result = await handler.GetAsync(id, ct);
        return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToProblemResult();
    }

    private static async Task<IResult> UpdateManufacturerAsync(Guid id, UpdateManufacturerRequest request, ManufacturerMaintenanceHandler handler, CancellationToken ct)
    {
        var result = await handler.UpdateAsync(new(id, request.Name, request.RegistrationNumber), ct);
        return result.IsSuccess ? Results.NoContent() : result.Error.ToProblemResult();
    }

    private static Task<IResult> ActivateManufacturerAsync(Guid id, ManufacturerMaintenanceHandler handler, CancellationToken ct) => ChangeStatusAsync(handler.ActivateAsync(id, ct));
    private static Task<IResult> DeactivateManufacturerAsync(Guid id, ManufacturerMaintenanceHandler handler, CancellationToken ct) => ChangeStatusAsync(handler.DeactivateAsync(id, ct));

    private static async Task<IResult> ListMeasurementUnitsAsync([AsParameters] ReferenceDataListRequest request, MeasurementUnitMaintenanceHandler handler, CancellationToken ct) =>
        Results.Ok(await handler.ListAsync(ToQuery(request), ct));

    private static async Task<IResult> GetMeasurementUnitAsync(Guid id, MeasurementUnitMaintenanceHandler handler, CancellationToken ct)
    {
        var result = await handler.GetAsync(id, ct);
        return result.IsSuccess ? Results.Ok(result.Value) : result.Error.ToProblemResult();
    }

    private static async Task<IResult> UpdateMeasurementUnitAsync(Guid id, UpdateMeasurementUnitRequest request, MeasurementUnitMaintenanceHandler handler, CancellationToken ct)
    {
        var result = await handler.UpdateAsync(new(id, request.Name, request.Symbol, request.ConversionFactor), ct);
        return result.IsSuccess ? Results.NoContent() : result.Error.ToProblemResult();
    }

    private static Task<IResult> ActivateMeasurementUnitAsync(Guid id, MeasurementUnitMaintenanceHandler handler, CancellationToken ct) => ChangeStatusAsync(handler.ActivateAsync(id, ct));
    private static Task<IResult> DeactivateMeasurementUnitAsync(Guid id, MeasurementUnitMaintenanceHandler handler, CancellationToken ct) => ChangeStatusAsync(handler.DeactivateAsync(id, ct));

    private static async Task<IResult> ChangeStatusAsync(Task<AgroControl.Domain.Common.Result> resultTask)
    {
        var result = await resultTask;
        return result.IsSuccess ? Results.NoContent() : result.Error.ToProblemResult();
    }

    private static async Task<IResult> CreateInputCategoryAsync(CreateInputCategoryRequest request, CreateInputCategoryHandler handler, CancellationToken ct)
    {
        var result = await handler.HandleAsync(new(request.Name, request.Description), ct);
        return result.IsSuccess ? Results.Created($"/api/input-categories/{result.Value.Id}", result.Value) : result.Error.ToProblemResult();
    }

    private static async Task<IResult> CreateManufacturerAsync(CreateManufacturerRequest request, CreateManufacturerHandler handler, CancellationToken ct)
    {
        var result = await handler.HandleAsync(new(request.Name, request.RegistrationNumber), ct);
        return result.IsSuccess ? Results.Created($"/api/manufacturers/{result.Value.Id}", result.Value) : result.Error.ToProblemResult();
    }

    private static async Task<IResult> CreateMeasurementUnitAsync(CreateMeasurementUnitRequest request, CreateMeasurementUnitHandler handler, CancellationToken ct)
    {
        var result = await handler.HandleAsync(new(request.Name, request.Symbol, request.ConversionFactor), ct);
        return result.IsSuccess ? Results.Created($"/api/measurement-units/{result.Value.Id}", result.Value) : result.Error.ToProblemResult();
    }
}
