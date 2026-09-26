using FamilyHub.Domain.Meals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHub.Infrastructure.Configurations;

public class MealPlanEntryConfiguration
    : IEntityTypeConfiguration<MealPlanEntry>
{
    public void Configure(EntityTypeBuilder<MealPlanEntry> builder)
    {
        builder.ToTable("MealPlanEntries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Date)
            .IsRequired();

        builder.Property(x => x.MealId)
            .IsRequired();

        builder.HasOne(x => x.Meal)
            .WithMany()
            .HasForeignKey(x => x.MealId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}