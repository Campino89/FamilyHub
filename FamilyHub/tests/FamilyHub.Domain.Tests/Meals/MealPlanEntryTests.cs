using FamilyHub.Domain.Meals;
using FamilyHub.Domain.SideDishes;

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
        Assert.NotEqual(Guid.Empty, entry.Id);
        Assert.Equal(weeklyMealPlanId, entry.WeeklyMealPlanId);
        Assert.Equal(date, entry.Date);
        Assert.Equal(meal.Id, entry.MealId);
        Assert.Equal(meal, entry.Meal);
        Assert.Empty(entry.SideDishes);
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

    [Fact]
    public void AddSideDish_AddsSideDishToEntry()
    {
        // Arrange
        var entry = CreateMealPlanEntry();
        var sideDish = new SideDish("Reis");

        // Act
        entry.AddSideDish(sideDish);

        // Assert
        Assert.Contains(sideDish, entry.SideDishes);
    }

    [Fact]
    public void AddSideDish_SameSideDishTwice_DoesNotAddDuplicate()
    {
        // Arrange
        var entry = CreateMealPlanEntry();
        var sideDish = new SideDish("Kartoffeln");

        // Act
        entry.AddSideDish(sideDish);
        entry.AddSideDish(sideDish);

        // Assert
        Assert.Single(entry.SideDishes);
    }

    [Fact]
    public void RemoveSideDish_RemovesSideDishFromEntry()
    {
        // Arrange
        var entry = CreateMealPlanEntry();
        var sideDish = new SideDish("Brokkoli");

        // Act
        entry.AddSideDish(sideDish);
        entry.RemoveSideDish(sideDish);

        // Assert
        Assert.DoesNotContain(sideDish, entry.SideDishes);
        Assert.Empty(entry.SideDishes);
    }

    [Fact]
    public void AddSideDish_WithNull_ThrowsArgumentNullException()
    {
        // Arrange
        var entry = CreateMealPlanEntry();

        // Act + Assert
        Assert.Throws<ArgumentNullException>(
            () => entry.AddSideDish(null!));
    }

    [Fact]
    public void RemoveSideDish_WithNull_ThrowsArgumentNullException()
    {
        // Arrange
        var entry = CreateMealPlanEntry();

        // Act + Assert
        Assert.Throws<ArgumentNullException>(
            () => entry.RemoveSideDish(null!));
    }

    private static MealPlanEntry CreateMealPlanEntry()
    {
        var weeklyMealPlanId = Guid.NewGuid();
        var date = new DateOnly(2026, 9, 21);
        var meal = new Meal("Testgericht");

        return new MealPlanEntry(
            weeklyMealPlanId,
            date,
            meal);
    }
}