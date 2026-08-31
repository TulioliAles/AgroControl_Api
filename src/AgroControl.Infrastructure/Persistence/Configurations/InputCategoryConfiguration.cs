using AgroControl.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgroControl.Infrastructure.Persistence.Configurations;

internal sealed class InputCategoryConfiguration : IEntityTypeConfiguration<InputCategory>
{
    public void Configure(EntityTypeBuilder<InputCategory> builder)
    {
        builder.ToTable("InputCategories");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Ignore(x => x.DomainEvents);
    }
}
