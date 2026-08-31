using AgroControl.Domain.Catalog;

namespace AgroControl.Domain.UnitTests.Catalog;

public sealed class MeasurementUnitTests
{
    [Fact]
    public void Create_WithValidData_ShouldNormalizeValuesAndCreateActiveUnit()
    {
        var unit = MeasurementUnit.Create("  Quilograma  ", "  kg  ", 1m);

        Assert.NotEqual(Guid.Empty, unit.Id);
        Assert.Equal("Quilograma", unit.Name);
        Assert.Equal("kg", unit.Symbol);
        Assert.Equal(1m, unit.ConversionFactor);
        Assert.True(unit.IsActive);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidConversionFactor_ShouldThrowArgumentOutOfRangeException(
        decimal conversionFactor)
    {
        var action = () => MeasurementUnit.Create("Quilograma", "kg", conversionFactor);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }

    [Fact]
    public void Update_WithValidData_ShouldChangeProperties()
    {
        var unit = MeasurementUnit.Create("Quilograma", "kg");

        unit.Update("  Tonelada  ", "  t  ", 1000m);

        Assert.Equal("Tonelada", unit.Name);
        Assert.Equal("t", unit.Symbol);
        Assert.Equal(1000m, unit.ConversionFactor);
    }

    [Fact]
    public void ActivateAndDeactivate_ShouldChangeActiveStatus()
    {
        var unit = MeasurementUnit.Create("Litro", "L");

        unit.Deactivate();
        Assert.False(unit.IsActive);

        unit.Activate();
        Assert.True(unit.IsActive);
    }
}
