using AgroControl.Application.Abstractions.Data;
using AgroControl.Application.Catalog;
using AgroControl.Application.Catalog.Repositories;
using AgroControl.Application.Catalog.UpdateAgriculturalInput;
using AgroControl.Domain.Catalog;

namespace AgroControl.Application.UnitTests.Catalog;

public sealed class UpdateAgriculturalInputHandlerTests
{
    [Fact]
    public async Task Update_WithValidData_ShouldChangeEntityAndPersist()
    {
        var fixture = CreateFixture();

        var result = await fixture.UpdateHandler.HandleAsync(new UpdateAgriculturalInputCommand(
            fixture.Input.Id,
            "Herbicida Atualizado",
            "Nome Comercial",
            AgriculturalInputType.Pesticide,
            fixture.Category.Id,
            fixture.Manufacturer.Id,
            fixture.MeasurementUnit.Id));

        Assert.True(result.IsSuccess);
        Assert.Equal("Herbicida Atualizado", fixture.Input.Name);
        Assert.Equal(1, fixture.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Update_WhenInputDoesNotExist_ShouldReturnNotFound()
    {
        var fixture = CreateFixture();
        fixture.InputRepository.Entity = null;

        var result = await fixture.UpdateHandler.HandleAsync(new UpdateAgriculturalInputCommand(
            Guid.NewGuid(),
            "Herbicida",
            null,
            AgriculturalInputType.Pesticide,
            fixture.Category.Id,
            fixture.Manufacturer.Id,
            fixture.MeasurementUnit.Id));

        Assert.True(result.IsFailure);
        Assert.Equal(CatalogErrors.AgriculturalInputNotFound, result.Error);
        Assert.Equal(0, fixture.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Update_WhenNewNameAlreadyExists_ShouldReturnConflict()
    {
        var fixture = CreateFixture();
        fixture.InputRepository.NameExists = true;

        var result = await fixture.UpdateHandler.HandleAsync(new UpdateAgriculturalInputCommand(
            fixture.Input.Id,
            "Outro Nome",
            null,
            AgriculturalInputType.Pesticide,
            fixture.Category.Id,
            fixture.Manufacturer.Id,
            fixture.MeasurementUnit.Id));

        Assert.True(result.IsFailure);
        Assert.Equal(CatalogErrors.AgriculturalInputNameAlreadyExists, result.Error);
        Assert.Equal(0, fixture.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Deactivate_ShouldChangeStatusAndPersist()
    {
        var fixture = CreateFixture();

        var result = await fixture.StatusHandler.HandleAsync(fixture.Input.Id, false);

        Assert.True(result.IsSuccess);
        Assert.False(fixture.Input.IsActive);
        Assert.Equal(1, fixture.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Activate_ShouldChangeStatusAndPersist()
    {
        var fixture = CreateFixture();
        fixture.Input.Deactivate();

        var result = await fixture.StatusHandler.HandleAsync(fixture.Input.Id, true);

        Assert.True(result.IsSuccess);
        Assert.True(fixture.Input.IsActive);
        Assert.Equal(1, fixture.UnitOfWork.SaveChangesCallCount);
    }

    private static Fixture CreateFixture()
    {
        var category = InputCategory.Create("Defensivos");
        var manufacturer = Manufacturer.Create("Fabricante");
        var unit = MeasurementUnit.Create("Litro", "L");
        var input = AgriculturalInput.Create(
            "Herbicida",
            null,
            AgriculturalInputType.Pesticide,
            category.Id,
            manufacturer.Id,
            unit.Id);
        var inputRepository = new FakeAgriculturalInputRepository(input);
        var unitOfWork = new FakeUnitOfWork();

        return new Fixture(
            input,
            category,
            manufacturer,
            unit,
            inputRepository,
            unitOfWork,
            new UpdateAgriculturalInputHandler(
                inputRepository,
                new FakeInputCategoryRepository(category),
                new FakeManufacturerRepository(manufacturer),
                new FakeMeasurementUnitRepository(unit),
                unitOfWork),
            new ChangeAgriculturalInputStatusHandler(inputRepository, unitOfWork));
    }

    private sealed record Fixture(
        AgriculturalInput Input,
        InputCategory Category,
        Manufacturer Manufacturer,
        MeasurementUnit MeasurementUnit,
        FakeAgriculturalInputRepository InputRepository,
        FakeUnitOfWork UnitOfWork,
        UpdateAgriculturalInputHandler UpdateHandler,
        ChangeAgriculturalInputStatusHandler StatusHandler);

    private sealed class FakeAgriculturalInputRepository(AgriculturalInput? entity)
        : IAgriculturalInputRepository
    {
        public AgriculturalInput? Entity { get; set; } = entity;
        public bool NameExists { get; set; }

        public Task<AgriculturalInput?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Entity);

        public Task<AgriculturalInput?> GetForUpdateByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Entity);

        public Task<(IReadOnlyList<AgriculturalInput> Items, int TotalCount)> ListAsync(
            int page,
            int pageSize,
            string? search,
            bool? isActive,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(((IReadOnlyList<AgriculturalInput>)[], 0));

        public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) =>
            Task.FromResult(NameExists);

        public void Add(AgriculturalInput agriculturalInput) => throw new NotSupportedException();
    }

    private sealed class FakeInputCategoryRepository(InputCategory entity) : IInputCategoryRepository
    {
        public Task<InputCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<InputCategory?>(entity);

        public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public void Add(InputCategory category) => throw new NotSupportedException();
    }

    private sealed class FakeManufacturerRepository(Manufacturer entity) : IManufacturerRepository
    {
        public Task<Manufacturer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<Manufacturer?>(entity);

        public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public void Add(Manufacturer manufacturer) => throw new NotSupportedException();
    }

    private sealed class FakeMeasurementUnitRepository(MeasurementUnit entity) : IMeasurementUnitRepository
    {
        public Task<MeasurementUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<MeasurementUnit?>(entity);

        public Task<bool> ExistsBySymbolAsync(string symbol, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public void Add(MeasurementUnit measurementUnit) => throw new NotSupportedException();
    }

    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int SaveChangesCallCount { get; private set; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveChangesCallCount++;
            return Task.FromResult(1);
        }
    }
}
