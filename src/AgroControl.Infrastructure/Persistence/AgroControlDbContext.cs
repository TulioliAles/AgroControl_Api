using AgroControl.Application.Abstractions.Data;
using AgroControl.Domain.Catalog;
using AgroControl.Domain.Inventory;
using Microsoft.EntityFrameworkCore;

namespace AgroControl.Infrastructure.Persistence;

public sealed class AgroControlDbContext(DbContextOptions<AgroControlDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<AgriculturalInput> AgriculturalInputs => Set<AgriculturalInput>();
    public DbSet<InputCategory> InputCategories => Set<InputCategory>();
    public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
    public DbSet<MeasurementUnit> MeasurementUnits => Set<MeasurementUnit>();
    public DbSet<StockLot> StockLots => Set<StockLot>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgroControlDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
