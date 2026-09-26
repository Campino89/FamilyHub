using FamilyHub.Domain.Meals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyHub.Infrastructure.Configurations;

public class WeeklyMealPlanConfiguration
    : IEntityTypeConfiguration<WeeklyMealPlan>
{
    public void Configure(EntityTypeBuilder<WeeklyMealPlan> builder)
    {
        builder.ToTable("WeeklyMealPlans");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Year)
            .IsRequired();

        builder.Property(x => x.CalendarWeek)
            .IsRequired();

        builder.HasMany(x => x.Entries)
            .WithOne()
            .HasForeignKey(x => x.WeeklyMealPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
        {
            x.Year,
            x.CalendarWeek
        })
        .IsUnique();
    }
}