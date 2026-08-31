using AgroControl.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace AgroControl.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AgroControlDbContext))]
partial class AgroControlDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.11")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("AgroControl.Domain.Catalog.InputCategory", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uniqueidentifier");
            b.Property<string>("Description").HasMaxLength(500).HasColumnType("nvarchar(500)");
            b.Property<bool>("IsActive").HasColumnType("bit");
            b.Property<string>("Name").IsRequired().HasMaxLength(100).HasColumnType("nvarchar(100)");
            b.HasKey("Id");
            b.HasIndex("Name").IsUnique();
            b.ToTable("InputCategories");
        });

        modelBuilder.Entity("AgroControl.Domain.Catalog.Manufacturer", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uniqueidentifier");
            b.Property<bool>("IsActive").HasColumnType("bit");
            b.Property<string>("Name").IsRequired().HasMaxLength(150).HasColumnType("nvarchar(150)");
            b.Property<string>("RegistrationNumber").HasMaxLength(50).HasColumnType("nvarchar(50)");
            b.HasKey("Id");
            b.HasIndex("Name").IsUnique();
            b.ToTable("Manufacturers");
        });

        modelBuilder.Entity("AgroControl.Domain.Catalog.MeasurementUnit", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uniqueidentifier");
            b.Property<decimal>("ConversionFactor").HasPrecision(18, 6).HasColumnType("decimal(18,6)");
            b.Property<bool>("IsActive").HasColumnType("bit");
            b.Property<string>("Name").IsRequired().HasMaxLength(100).HasColumnType("nvarchar(100)");
            b.Property<string>("Symbol").IsRequired().HasMaxLength(20).HasColumnType("nvarchar(20)");
            b.HasKey("Id");
            b.HasIndex("Symbol").IsUnique();
            b.ToTable("MeasurementUnits");
        });

        modelBuilder.Entity("AgroControl.Domain.Catalog.AgriculturalInput", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uniqueidentifier");
            b.Property<Guid>("CategoryId").HasColumnType("uniqueidentifier");
            b.Property<string>("CommercialName").HasMaxLength(150).HasColumnType("nvarchar(150)");
            b.Property<bool>("IsActive").HasColumnType("bit");
            b.Property<Guid>("ManufacturerId").HasColumnType("uniqueidentifier");
            b.Property<Guid>("MeasurementUnitId").HasColumnType("uniqueidentifier");
            b.Property<string>("Name").IsRequired().HasMaxLength(150).HasColumnType("nvarchar(150)");
            b.Property<int>("Type").HasColumnType("int");
            b.HasKey("Id");
            b.HasIndex("CategoryId");
            b.HasIndex("ManufacturerId");
            b.HasIndex("MeasurementUnitId");
            b.HasIndex("Name").IsUnique();
            b.ToTable("AgriculturalInputs");
        });

        modelBuilder.Entity("AgroControl.Domain.Identity.User", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uniqueidentifier");
            b.Property<string>("Email").IsRequired().HasMaxLength(254).HasColumnType("nvarchar(254)");
            b.Property<bool>("IsActive").HasColumnType("bit");
            b.Property<string>("Name").IsRequired().HasMaxLength(150).HasColumnType("nvarchar(150)");
            b.Property<string>("PasswordHash").IsRequired().HasMaxLength(1000).HasColumnType("nvarchar(1000)");
            b.Property<string>("Role").IsRequired().HasMaxLength(50).HasColumnType("nvarchar(50)");
            b.HasKey("Id");
            b.HasIndex("Email").IsUnique();
            b.ToTable("Users");
        });

        modelBuilder.Entity("AgroControl.Domain.Inventory.StockLot", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uniqueidentifier");
            b.Property<Guid>("AgriculturalInputId").HasColumnType("uniqueidentifier");
            b.Property<decimal>("CurrentQuantity").HasPrecision(18, 6).HasColumnType("decimal(18,6)");
            b.Property<DateOnly?>("ExpirationDate").HasColumnType("date");
            b.Property<bool>("IsActive").HasColumnType("bit");
            b.Property<string>("LotNumber").IsRequired().HasMaxLength(100).HasColumnType("nvarchar(100)");
            b.HasKey("Id");
            b.HasIndex("ExpirationDate");
            b.HasIndex("AgriculturalInputId", "LotNumber").IsUnique();
            b.ToTable("StockLots");
        });

        modelBuilder.Entity("AgroControl.Domain.Inventory.StockMovement", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uniqueidentifier");
            b.Property<string>("Notes").HasMaxLength(500).HasColumnType("nvarchar(500)");
            b.Property<DateTimeOffset>("OccurredAt").HasColumnType("datetimeoffset");
            b.Property<decimal>("Quantity").HasPrecision(18, 6).HasColumnType("decimal(18,6)");
            b.Property<Guid>("StockLotId").HasColumnType("uniqueidentifier");
            b.Property<int>("Type").HasColumnType("int");
            b.HasKey("Id");
            b.HasIndex("OccurredAt");
            b.HasIndex("StockLotId");
            b.ToTable("StockMovements");
        });

        modelBuilder.Entity("AgroControl.Domain.Catalog.AgriculturalInput", b =>
        {
            b.HasOne("AgroControl.Domain.Catalog.InputCategory", null)
                .WithMany()
                .HasForeignKey("CategoryId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            b.HasOne("AgroControl.Domain.Catalog.Manufacturer", null)
                .WithMany()
                .HasForeignKey("ManufacturerId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            b.HasOne("AgroControl.Domain.Catalog.MeasurementUnit", null)
                .WithMany()
                .HasForeignKey("MeasurementUnitId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });

        modelBuilder.Entity("AgroControl.Domain.Inventory.StockLot", b =>
        {
            b.HasOne("AgroControl.Domain.Catalog.AgriculturalInput", null)
                .WithMany()
                .HasForeignKey("AgriculturalInputId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });

        modelBuilder.Entity("AgroControl.Domain.Inventory.StockMovement", b =>
        {
            b.HasOne("AgroControl.Domain.Inventory.StockLot", null)
                .WithMany("Movements")
                .HasForeignKey("StockLotId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });
    }
}
