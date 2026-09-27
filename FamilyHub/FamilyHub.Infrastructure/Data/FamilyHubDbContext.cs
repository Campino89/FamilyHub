using FamilyHub.Domain.MealComponents;
using FamilyHub.Domain.Meals;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Infrastructure.Data;

public class FamilyHubDbContext : DbContext
{
    // Wird von EF Core Design-Time verwendet
    public FamilyHubDbContext()
    {
    }

    // Wird später von unserer API verwendet
    public FamilyHubDbContext(
        DbContextOptions<FamilyHubDbContext> options)
        : base(options)
    {
    }

    public DbSet<Meal> Meals => Set<Meal>();

    public DbSet<MealPlanEntry> MealPlanEntries => Set<MealPlanEntry>();

    public DbSet<WeeklyMealPlan> WeeklyMealPlans => Set<WeeklyMealPlan>();

    public DbSet<MealComponent> MealComponents => Set<MealComponent>();

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=familyhub;Username=familyhub;Password=familyhub");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(FamilyHubDbContext).Assembly);
    }
}