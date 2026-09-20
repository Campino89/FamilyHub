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

}