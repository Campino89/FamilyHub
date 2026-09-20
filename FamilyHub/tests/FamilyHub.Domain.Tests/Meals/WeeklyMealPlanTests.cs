using FamilyHub.Domain.Meals;

namespace FamilyHub.Domain.Tests.Meals;

public class WeeklyMealPlanTests
{
    [Fact]
    public void AddMeal_ShouldAddEntryToPlan()
    {
        // Arrange
        var plan = new WeeklyMealPlan(2026, 39);

        var date = new DateOnly(2026, 9, 21);
        var meal = new Meal("Hähnchen Curry mit Reis");

        // Act
        plan.AddMeal(date, meal);

        // Assert
        Assert.Single(plan.Entries);
        Assert.Equal(date, plan.Entries[0].Datum);
        Assert.Equal(meal, plan.Entries[0].Essen);
    }

    [Fact]
    public void AddMeal_WhenDateAlreadyExists_ShouldThrowException()
    {
        // Arrange
        var plan = new WeeklyMealPlan(2026, 39);
        var date = new DateOnly(2026, 9, 21);

        plan.AddMeal(date, new Meal("Lasagne"));

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            plan.AddMeal(date, new Meal("Pizza")));
    }

    [Fact]
    public void AddMeal_WhenDateIsOutsideCalendarWeek_ShouldThrowException()
    {
        // Arrange
        var plan = new WeeklyMealPlan(2026, 39);

        var date = new DateOnly(2026, 12, 24);
        var meal = new Meal("Weihnachtsessen");

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            plan.AddMeal(date, meal));
    }

    [Fact]
    public void Constructor_WithInvalidCalendarWeek_ShouldThrowException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new WeeklyMealPlan(2026, 0));
    }
}