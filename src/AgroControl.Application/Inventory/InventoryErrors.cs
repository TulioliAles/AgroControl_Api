using AgroControl.Domain.Common;

namespace AgroControl.Application.Inventory;

public static class InventoryErrors
{
    public static readonly Error StockLotNotFound = Error.NotFound(
        "Inventory.StockLot.NotFound",
        "The stock lot was not found.");

    public static readonly Error AgriculturalInputNotAvailable = Error.NotFound(
        "Inventory.AgriculturalInput.NotAvailable",
        "The agricultural input was not found or is inactive.");

    public static readonly Error StockLotAlreadyExists = Error.Conflict(
        "Inventory.StockLot.AlreadyExists",
        "A stock lot with the same number already exists for this agricultural input.");

    public static readonly Error InvalidQuantity = Error.Validation(
        "Inventory.Movement.InvalidQuantity",
        "The movement quantity must be greater than zero.");

    public static readonly Error InsufficientStock = Error.Conflict(
        "Inventory.Movement.InsufficientStock",
        "The available stock is insufficient for this exit.");

    public static readonly Error InactiveStockLot = Error.Conflict(
        "Inventory.StockLot.Inactive",
        "Stock movements cannot be recorded for an inactive lot.");
}
