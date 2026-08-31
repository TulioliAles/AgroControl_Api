using AgroControl.Application.Abstractions.Data;
using AgroControl.Application.Catalog;
using AgroControl.Application.Catalog.CreateAgriculturalInput;
using AgroControl.Application.Catalog.Repositories;
using AgroControl.Domain.Catalog;

namespace AgroControl.Application.UnitTests.Catalog;

public sealed class CreateAgriculturalInputHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithValidData_ShouldPersistInputAndReturnId()
    {
        var fixture = CreateFixture();

        var result = await fixture.Handler.HandleAsync(fixture.Command);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.NotNull(fixture.AgriculturalInputRepository.AddedInput);
        Assert.Equal(result.Value.Id, fixture.AgriculturalInputRepository.AddedInput.Id);
        Assert.Equal(1, fixture.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task HandleAsync_WhenNameAlreadyExists_ShouldReturnConflictAndNotPersist()
    {
        var fixture = CreateFixture();
        fixture.AgriculturalInputRepository.NameExists = true;

        var result = await fixture.Handler.HandleAsync(fixture.Command);

        Assert.True(result.IsFailure);
        Assert.Equal(CatalogErrors.AgriculturalInputNameAlreadyExists, result.Error);
        Assert.Null(fixture.AgriculturalInputRepository.AddedInput);
        Assert.Equal(0, fixture.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task HandleAsync_WhenCategoryIsInactive_ShouldReturnNotFoundAndNotPersist()
    {
        var fixture = CreateFixture();
        fixture.Category.Deactivate();

        var result = await fixture.Handler.HandleAsync(fixture.Command);

        Assert.True(result.IsFailure);
        Assert.Equal(CatalogErrors.InputCategoryNotFound, result.Error);
        Assert.Null(fixture.AgriculturalInputRepository.AddedInput);
        Assert.Equal(0, fixture.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task HandleAsync_WhenManufacturerDoesNotExist_ShouldReturnNotFoundAndNotPersist()
    {
        var fixture = CreateFixture();
        fixture.ManufacturerRepository.Entity = null;

        var result = await fixture.Handler.HandleAsync(fixture.Command);

        Assert.True(result.IsFailure);
        Assert.Equal(CatalogErrors.ManufacturerNotFound, result.Error);
        Assert.Null(fixture.AgriculturalInputRepository.AddedInput);
        Assert.Equal(0, fixture.UnitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task HandleAsync_WhenMeasurementUnitDoesNotExist_ShouldReturnNotFoundAndNotPersist()
    {
        var fixture = CreateFixture();
        fixture.MeasurementUnitRepository.Entity = null;

        var result = await fixture.Handler.HandleAsync(fixture.Command);

        Assert.True(result.IsFailure);
        Assert.Equal(CatalogErrors.MeasurementUnitNotFound, result.Error);
        Assert.Null(fixture.AgriculturalInputRepository.AddedInput);
        Assert.Equal(0, fixture.UnitOfWork.SaveChangesCallCount);
    }

    private static TestFixture CreateFixture()
    {
        var category = InputCategory.Create("Defensivos");
        var manufacturer = Manufacturer.Create("Fabricante A");
        var measurementUnit = MeasurementUnit.Create("Litro", "L");

        var agriculturalInputRepository = new FakeAgriculturalInputRepository();
        var categoryRepository = new FakeInputCategoryRepository(category);
        var manufacturerRepository = new FakeManufacturerRepository(manufacturer);
        var measurementUnitRepository = new FakeMeasurementUnitRepository(measurementUnit);
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CreateAgriculturalInputHandler(
            agriculturalInputRepository,
            categoryRepository,
            manufacturerRepository,
            measurementUnitRepository,
            unitOfWork);

        var command = new CreateAgriculturalInputCommand(
            "Herbicida A",
            "Campo Limpo",
            AgriculturalInputType.Pesticide,
            category.Id,
            manufacturer.Id,
            measurementUnit.Id);

        return new TestFixture(
            handler,
            command,
            category,
            agriculturalInputRepository,
            categoryRepository,
            manufacturerRepository,
            measurementUnitRepository,
            unitOfWork);
    }

    private sealed record TestFixture(
        CreateAgriculturalInputHandler Handler,
        CreateAgriculturalInputCommand Command,
        InputCategory Category,
        FakeAgriculturalInputRepository AgriculturalInputRepository,
        FakeInputCategoryRepository CategoryRepository,
        FakeManufacturerRepository ManufacturerRepository,
        FakeMeasurementUnitRepository MeasurementUnitRepository,
        FakeUnitOfWork UnitOfWork);

    private sealed class FakeAgriculturalInputRepository : IAgriculturalInputRepository
    {
        public bool NameExists { get; set; }
        public AgriculturalInput? AddedInput { get; private set; }

        public Task<AgriculturalInput?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<AgriculturalInput?>(null);

        public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) =>
            Task.FromResult(NameExists);

        public void Add(AgriculturalInput agriculturalInput) => AddedInput = agriculturalInput;
    }

    private sealed class FakeInputCategoryRepository(InputCategory? entity) : IInputCategoryRepository
    {
        public InputCategory? Entity { get; set; } = entity;

        public Task<InputCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Entity);

        public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public void Add(InputCategory category) => throw new NotSupportedException();
    }

    private sealed class FakeManufacturerRepository(Manufacturer? entity) : IManufacturerRepository
    {
        public Manufacturer? Entity { get; set; } = entity;

        public Task<Manufacturer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Entity);

        public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);

        public void Add(Manufacturer manufacturer) => throw new NotSupportedException();
    }

    private sealed class FakeMeasurementUnitRepository(MeasurementUnit? entity) : IMeasurementUnitRepository
    {
        public MeasurementUnit? Entity { get; set; } = entity;

        public Task<MeasurementUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Entity);

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
