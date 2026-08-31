using AgroControl.Domain.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroControl.Infrastructure.Persistence.Configurations;

internal sealed class StockLotConfiguration : IEntityTypeConfiguration<StockLot>
{
    public void Configure(EntityTypeBuilder<StockLot> builder)
    {
        builder.ToTable("StockLots");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.LotNumber).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ExpirationDate).HasColumnType("date");
        builder.Property(x => x.CurrentQuantity).HasPrecision(18, 6);
        builder.Property(x => x.IsActive).IsRequired();
        builder.Ignore(x => x.DomainEvents);

        builder.HasIndex(x => new { x.AgriculturalInputId, x.LotNumber }).IsUnique();
        builder.HasIndex(x => x.ExpirationDate);

        builder.HasOne<AgroControl.Domain.Catalog.AgriculturalInput>()
            .WithMany()
            .HasForeignKey(x => x.AgriculturalInputId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Movements)
            .WithOne()
            .HasForeignKey(x => x.StockLotId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(x => x.Movements)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
