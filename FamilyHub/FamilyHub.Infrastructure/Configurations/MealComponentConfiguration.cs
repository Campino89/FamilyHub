using FamilyHub.Domain.Meals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHub.Infrastructure.Persistence.Configurations;

public class MealComponentConfiguration
    : IEntityTypeConfiguration<MealComponent>
{
    public void Configure(EntityTypeBuilder<MealComponent> builder)
    {
        builder.ToTable("MealComponents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(x => x.Name)
            .IsUnique();
    }
}