using AgroControl.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroControl.Infrastructure.Persistence.Configurations;

internal sealed class AgriculturalInputConfiguration : IEntityTypeConfiguration<AgriculturalInput>
{
    public void Configure(EntityTypeBuilder<AgriculturalInput> builder)
    {
        builder.ToTable("AgriculturalInputs");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(150).IsRequired();
        builder.Property(x => x.CommercialName).HasMaxLength(150);
        builder.Property(x => x.Type).HasConversion<int>().IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
        builder.HasOne<InputCategory>().WithMany().HasForeignKey(x => x.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Manufacturer>().WithMany().HasForeignKey(x => x.ManufacturerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<MeasurementUnit>().WithMany().HasForeignKey(x => x.MeasurementUnitId).OnDelete(DeleteBehavior.Restrict);
        builder.Ignore(x => x.DomainEvents);
    }
}
