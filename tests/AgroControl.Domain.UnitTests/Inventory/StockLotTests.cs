using AgroControl.Domain.Inventory;

namespace AgroControl.Domain.UnitTests.Inventory;

public sealed class StockLotTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateActiveEmptyLot()
    {
        var inputId = Guid.NewGuid();
        var expirationDate = new DateOnly(2027, 6, 30);

        var lot = StockLot.Create(inputId, " LOT-001 ", expirationDate);

        Assert.NotEqual(Guid.Empty, lot.Id);
        Assert.Equal(inputId, lot.AgriculturalInputId);
        Assert.Equal("LOT-001", lot.LotNumber);
        Assert.Equal(expirationDate, lot.ExpirationDate);
        Assert.Equal(0m, lot.CurrentQuantity);
        Assert.True(lot.IsActive);
        Assert.Empty(lot.Movements);
    }

    [Fact]
    public void Receive_ShouldIncreaseBalanceAndRegisterEntry()
    {
        var lot = StockLot.Create(Guid.NewGuid(), "LOT-001");
        var occurredAt = DateTimeOffset.UtcNow;

        lot.Receive(25.5m, occurredAt, "Initial receipt");

        Assert.Equal(25.5m, lot.CurrentQuantity);
        var movement = Assert.Single(lot.Movements);
        Assert.Equal(StockMovementType.Entry, movement.Type);
        Assert.Equal(25.5m, movement.Quantity);
        Assert.Equal(occurredAt, movement.OccurredAt);
    }

    [Fact]
    public void Issue_WithAvailableStock_ShouldDecreaseBalanceAndRegisterExit()
    {
        var lot = StockLot.Create(Guid.NewGuid(), "LOT-001");
        lot.Receive(20m, DateTimeOffset.UtcNow);

        lot.Issue(7.5m, DateTimeOffset.UtcNow, "Field application");

        Assert.Equal(12.5m, lot.CurrentQuantity);
        Assert.Equal(2, lot.Movements.Count);
        Assert.Equal(StockMovementType.Exit, lot.Movements.Last().Type);
    }

    [Fact]
    public void Issue_AboveAvailableStock_ShouldRejectMovement()
    {
        var lot = StockLot.Create(Guid.NewGuid(), "LOT-001");
        lot.Receive(5m, DateTimeOffset.UtcNow);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            lot.Issue(6m, DateTimeOffset.UtcNow));

        Assert.Equal("Insufficient stock for this movement.", exception.Message);
        Assert.Equal(5m, lot.CurrentQuantity);
        Assert.Single(lot.Movements);
    }

    [Fact]
    public void Movement_OnInactiveLot_ShouldBeRejected()
    {
        var lot = StockLot.Create(Guid.NewGuid(), "LOT-001");
        lot.Deactivate();

        Assert.Throws<InvalidOperationException>(() =>
            lot.Receive(1m, DateTimeOffset.UtcNow));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Receive_WithInvalidQuantity_ShouldReject(decimal quantity)
    {
        var lot = StockLot.Create(Guid.NewGuid(), "LOT-001");

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            lot.Receive(quantity, DateTimeOffset.UtcNow));
    }
}
