using FamilyHub.Domain.Meals;

namespace FamilyHub.Domain.Tests.Meals;

public class MealPlanEntryTests
{
    [Fact]
    public void Constructor_WithDateAndMeal_CreatesEntry()
    {
        // Arrange
        var weeklyMealPlanId = Guid.NewGuid();
        var date = new DateOnly(2026, 9, 21);
        var meal = new Meal("Hähnchen Curry mit Reis");

        // Act
        var entry = new MealPlanEntry(
            weeklyMealPlanId,
            date,
            meal);

        // Assert
        Assert.Equal(weeklyMealPlanId, entry.WeeklyMealPlanId);
        Assert.Equal(date, entry.Date);
        Assert.Equal(meal.Id, entry.MealId);
        Assert.Equal(meal, entry.Meal);
    }

    [Fact]
    public void Constructor_WithoutMeal_ShouldThrowException()
    {
        // Arrange
        var weeklyMealPlanId = Guid.NewGuid();
        var date = new DateOnly(2026, 9, 21);

        // Act + Assert
        Assert.Throws<ArgumentNullException>(
            () => new MealPlanEntry(
                weeklyMealPlanId,
                date,
                null!));
    }
}