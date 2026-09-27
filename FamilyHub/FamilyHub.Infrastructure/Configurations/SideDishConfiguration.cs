using FamilyHub.Domain.SideDishes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHub.Infrastructure.Persistence.Configurations;

public class SideDishConfiguration : IEntityTypeConfiguration<SideDish>
{
    public void Configure(EntityTypeBuilder<SideDish> builder)
    {
        builder.ToTable("SideDishes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}