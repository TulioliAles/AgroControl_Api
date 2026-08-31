using AgroControl.Domain.Catalog;
using AgroControl.Domain.Catalog.Events;

namespace AgroControl.Domain.UnitTests.Catalog;

public sealed class AgriculturalInputTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateActiveInputAndRaiseCreatedEvent()
    {
        var categoryId = Guid.NewGuid();
        var manufacturerId = Guid.NewGuid();
        var measurementUnitId = Guid.NewGuid();

        var input = AgriculturalInput.Create(
            "  Herbicida A  ",
            "  Campo Limpo  ",
            AgriculturalInputType.Pesticide,
            categoryId,
            manufacturerId,
            measurementUnitId);

        Assert.NotEqual(Guid.Empty, input.Id);
        Assert.Equal("Herbicida A", input.Name);
        Assert.Equal("Campo Limpo", input.CommercialName);
        Assert.Equal(AgriculturalInputType.Pesticide, input.Type);
        Assert.Equal(categoryId, input.CategoryId);
        Assert.Equal(manufacturerId, input.ManufacturerId);
        Assert.Equal(measurementUnitId, input.MeasurementUnitId);
        Assert.True(input.IsActive);

        var domainEvent = Assert.Single(input.DomainEvents);
        var createdEvent = Assert.IsType<AgriculturalInputCreatedDomainEvent>(domainEvent);
        Assert.Equal(input.Id, createdEvent.AgriculturalInputId);
        Assert.Equal(input.Name, createdEvent.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldThrowArgumentException(string? name)
    {
        var action = () => AgriculturalInput.Create(
            name!,
            null,
            AgriculturalInputType.Fertilizer,
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Create_WithEmptyCategoryId_ShouldThrowArgumentException()
    {
        var action = () => AgriculturalInput.Create(
            "Fertilizante A",
            null,
            AgriculturalInputType.Fertilizer,
            Guid.Empty,
            Guid.NewGuid(),
            Guid.NewGuid());

        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Update_WithValidData_ShouldChangePropertiesAndRaiseUpdatedEvent()
    {
        var input = CreateValidInput();
        input.ClearDomainEvents();

        var categoryId = Guid.NewGuid();
        var manufacturerId = Guid.NewGuid();
        var measurementUnitId = Guid.NewGuid();

        input.Update(
            "  Semente B  ",
            "  Cultivar B  ",
            AgriculturalInputType.Seed,
            categoryId,
            manufacturerId,
            measurementUnitId);

        Assert.Equal("Semente B", input.Name);
        Assert.Equal("Cultivar B", input.CommercialName);
        Assert.Equal(AgriculturalInputType.Seed, input.Type);
        Assert.Equal(categoryId, input.CategoryId);
        Assert.Equal(manufacturerId, input.ManufacturerId);
        Assert.Equal(measurementUnitId, input.MeasurementUnitId);

        var domainEvent = Assert.Single(input.DomainEvents);
        var updatedEvent = Assert.IsType<AgriculturalInputUpdatedDomainEvent>(domainEvent);
        Assert.Equal(input.Id, updatedEvent.AgriculturalInputId);
        Assert.Equal(input.Name, updatedEvent.Name);
    }

    [Fact]
    public void ActivateAndDeactivate_ShouldChangeActiveStatus()
    {
        var input = CreateValidInput();

        input.Deactivate();
        Assert.False(input.IsActive);

        input.Activate();
        Assert.True(input.IsActive);
    }

    private static AgriculturalInput CreateValidInput() => AgriculturalInput.Create(
        "Fertilizante A",
        "Comercial A",
        AgriculturalInputType.Fertilizer,
        Guid.NewGuid(),
        Guid.NewGuid(),
        Guid.NewGuid());
}
