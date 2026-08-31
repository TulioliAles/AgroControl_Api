using AgroControl.Application.Abstractions.Data;
using AgroControl.Application.Catalog;
using AgroControl.Application.Catalog.CreateReferenceData;
using AgroControl.Application.Catalog.Repositories;
using AgroControl.Domain.Catalog;

namespace AgroControl.Application.UnitTests.Catalog;

public sealed class CreateCatalogReferenceDataHandlerTests
{
    [Fact]
    public async Task CreateInputCategory_WithValidData_ShouldPersistAndReturnId()
    {
        var repository = new FakeInputCategoryRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateInputCategoryHandler(repository, unitOfWork);

        var result = await handler.HandleAsync(
            new CreateInputCategoryCommand(" Defensivos ", " Controle de pragas "));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.NotNull(repository.AddedEntity);
        Assert.Equal("Defensivos", repository.AddedEntity.Name);
        Assert.Equal(result.Value.Id, repository.AddedEntity.Id);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreateInputCategory_WhenNameExists_ShouldReturnConflictAndNotPersist()
    {
        var repository = new FakeInputCategoryRepository { Exists = true };
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateInputCategoryHandler(repository, unitOfWork);

        var result = await handler.HandleAsync(
            new CreateInputCategoryCommand("Defensivos", null));

        Assert.True(result.IsFailure);
        Assert.Equal(CatalogErrors.InputCategoryNameAlreadyExists, result.Error);
        Assert.Null(repository.AddedEntity);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreateManufacturer_WithValidData_ShouldPersistAndReturnId()
    {
        var repository = new FakeManufacturerRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateManufacturerHandler(repository, unitOfWork);

        var result = await handler.HandleAsync(
            new CreateManufacturerCommand(" Fabricante A ", " 12.345.678/0001-90 "));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.NotNull(repository.AddedEntity);
        Assert.Equal("Fabricante A", repository.AddedEntity.Name);
        Assert.Equal("12.345.678/0001-90", repository.AddedEntity.RegistrationNumber);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreateManufacturer_WhenNameExists_ShouldReturnConflictAndNotPersist()
    {
        var repository = new FakeManufacturerRepository { Exists = true };
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateManufacturerHandler(repository, unitOfWork);

        var result = await handler.HandleAsync(
            new CreateManufacturerCommand("Fabricante A", null));

        Assert.True(result.IsFailure);
        Assert.Equal(CatalogErrors.ManufacturerNameAlreadyExists, result.Error);
        Assert.Null(repository.AddedEntity);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreateMeasurementUnit_WithValidData_ShouldPersistAndReturnId()
    {
        var repository = new FakeMeasurementUnitRepository();
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateMeasurementUnitHandler(repository, unitOfWork);

        var result = await handler.HandleAsync(
            new CreateMeasurementUnitCommand(" Litro ", " L ", 1m));

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.NotNull(repository.AddedEntity);
        Assert.Equal("Litro", repository.AddedEntity.Name);
        Assert.Equal("L", repository.AddedEntity.Symbol);
        Assert.Equal(1m, repository.AddedEntity.ConversionFactor);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task CreateMeasurementUnit_WhenSymbolExists_ShouldReturnConflictAndNotPersist()
    {
        var repository = new FakeMeasurementUnitRepository { Exists = true };
        var unitOfWork = new FakeUnitOfWork();
        var handler = new CreateMeasurementUnitHandler(repository, unitOfWork);

        var result = await handler.HandleAsync(
            new CreateMeasurementUnitCommand("Litro", "L", 1m));

        Assert.True(result.IsFailure);
        Assert.Equal(CatalogErrors.MeasurementUnitSymbolAlreadyExists, result.Error);
        Assert.Null(repository.AddedEntity);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    private sealed class FakeInputCategoryRepository : IInputCategoryRepository
    {
        public bool Exists { get; set; }
        public InputCategory? AddedEntity { get; private set; }

        public Task<InputCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<InputCategory?>(null);

        public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) =>
            Task.FromResult(Exists);

        public void Add(InputCategory category) => AddedEntity = category;
    }

    private sealed class FakeManufacturerRepository : IManufacturerRepository
    {
        public bool Exists { get; set; }
        public Manufacturer? AddedEntity { get; private set; }

        public Task<Manufacturer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<Manufacturer?>(null);

        public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default) =>
            Task.FromResult(Exists);

        public void Add(Manufacturer manufacturer) => AddedEntity = manufacturer;
    }

    private sealed class FakeMeasurementUnitRepository : IMeasurementUnitRepository
    {
        public bool Exists { get; set; }
        public MeasurementUnit? AddedEntity { get; private set; }

        public Task<MeasurementUnit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult<MeasurementUnit?>(null);

        public Task<bool> ExistsBySymbolAsync(string symbol, CancellationToken cancellationToken = default) =>
            Task.FromResult(Exists);

        public void Add(MeasurementUnit measurementUnit) => AddedEntity = measurementUnit;
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
