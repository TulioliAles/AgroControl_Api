using AgroControl.Domain.Common;

namespace AgroControl.Api.Extensions;

public static class ErrorExtensions
{
    public static IResult ToProblemResult(this Error error)
    {
        var statusCode = error.Code switch
        {
            "Catalog.AgriculturalInput.NameAlreadyExists" => StatusCodes.Status409Conflict,
            "Catalog.InputCategory.NameAlreadyExists" => StatusCodes.Status409Conflict,
            "Catalog.Manufacturer.NameAlreadyExists" => StatusCodes.Status409Conflict,
            "Catalog.MeasurementUnit.SymbolAlreadyExists" => StatusCodes.Status409Conflict,
            "Inventory.StockLot.AlreadyExists" => StatusCodes.Status409Conflict,
            "Inventory.Movement.InsufficientStock" => StatusCodes.Status409Conflict,
            "Inventory.StockLot.Inactive" => StatusCodes.Status409Conflict,
            "Catalog.AgriculturalInput.NotFound" => StatusCodes.Status404NotFound,
            "Catalog.InputCategory.NotFound" => StatusCodes.Status404NotFound,
            "Catalog.Manufacturer.NotFound" => StatusCodes.Status404NotFound,
            "Catalog.MeasurementUnit.NotFound" => StatusCodes.Status404NotFound,
            "Inventory.StockLot.NotFound" => StatusCodes.Status404NotFound,
            "Inventory.AgriculturalInput.NotAvailable" => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status400BadRequest
        };

        var title = statusCode switch
        {
            StatusCodes.Status404NotFound => "Resource not found",
            StatusCodes.Status409Conflict => "Resource conflict",
            _ => "Request validation failed"
        };

        return Results.Problem(
            statusCode: statusCode,
            title: title,
            detail: error.Description,
            extensions: new Dictionary<string, object?> { ["code"] = error.Code });
    }

    public static IResult ToValidationProblem(this ArgumentException exception)
    {
        var field = exception.ParamName ?? "request";
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            [field] = [exception.Message]
        });
    }
}
