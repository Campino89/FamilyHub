using FamilyHub.Domain.Meals;

namespace FamilyHub.Domain.Tests.Meals;

public class MealPlanEntryTests
{
    [Fact]
    public void Constructor_WithDateAndMeal_CreatesEntry()
    {
        // Arrange
        var date = new DateOnly(2026, 9, 21);
        var meal = new Meal("Hähnchen Curry mit Reis");

        // Act
        var entry = new MealPlanEntry(date, meal);

        // Assert
        Assert.Equal(date, entry.Datum);
        Assert.Equal(meal, entry.Essen);
    }

    [Fact]
    public void Constructor_WithoutMeal_ShouldThrowException()
    {
        var date = new DateOnly(2026, 9, 21);

        Assert.Throws<ArgumentNullException>(
            () => new MealPlanEntry(date, null!));
    }
}