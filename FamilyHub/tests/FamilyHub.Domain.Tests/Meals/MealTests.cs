using FamilyHub.Domain.MealComponents;
using FamilyHub.Domain.Meals;

namespace FamilyHub.Domain.Tests.Meals;

public class MealTests
{
    [Fact]
    public void Constructor_WithName_CreatesMeal()
    {
        // Arrange
        const string name = "Hähnchen Curry mit Reis";

        // Act
        var meal = new Meal(name);

        // Assert
        Assert.Equal(name, meal.Name);
        Assert.NotEqual(Guid.Empty, meal.Id);
    }

    [Fact]
    public void Constructor_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        const string name = "";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Meal(name));
    }

    [Fact]
    public void AddComponent_AddsComponentToMeal()
    {
        // Arrange
        var meal = new Meal("Frikadellen mit Reis");
        var component = new MealComponent("Reis");

        // Act
        meal.AddComponent(component);

        // Assert
        Assert.Single(meal.Components);
        Assert.Contains(component, meal.Components);
    }

    [Fact]
    public void AddComponent_SameComponentTwice_DoesNotAddDuplicate()
    {
        // Arrange
        var meal = new Meal("Frikadellen mit Reis");
        var component = new MealComponent("Reis");

        // Act
        meal.AddComponent(component);
        meal.AddComponent(component);

        // Assert
        Assert.Single(meal.Components);
    }

    [Fact]
    public void RemoveComponent_RemovesComponentFromMeal()
    {
        // Arrange
        var meal = new Meal("Frikadellen mit Reis");
        var component = new MealComponent("Reis");

        meal.AddComponent(component);

        // Act
        meal.RemoveComponent(component);

        // Assert
        Assert.Empty(meal.Components);
    }

    [Fact]
    public void AddComponent_WithNull_ThrowsArgumentNullException()
    {
        // Arrange
        var meal = new Meal("Frikadellen mit Reis");

        // Act & Assert
        Assert.Throws<ArgumentNullException>(
            () => meal.AddComponent(null!));
    }

}