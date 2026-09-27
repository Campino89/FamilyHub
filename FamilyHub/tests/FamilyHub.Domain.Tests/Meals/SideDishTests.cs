using FamilyHub.Domain.SideDishes;

namespace FamilyHub.Domain.SideDishTests;

public class SideDishTests
{
    [Fact]
    public void Constructor_WithValidName_CreatesSideDish()
    {
        // Arrange
        const string name = "Reis";

        // Act
        var sideDish = new SideDish(name);

        // Assert
        Assert.NotEqual(Guid.Empty, sideDish.Id);
        Assert.Equal(name, sideDish.Name);
    }

    [Fact]
    public void Constructor_WithWhitespaceAroundName_TrimsName()
    {
        // Act
        var sideDish = new SideDish("  Kartoffeln  ");

        // Assert
        Assert.Equal("Kartoffeln", sideDish.Name);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public void Constructor_WithEmptyOrWhitespaceName_ThrowsArgumentException(
        string name)
    {
        // Act
        var action = () => new SideDish(name);

        // Assert
        Assert.Throws<ArgumentException>(action);
    }

    [Fact]
    public void Constructor_WithNullName_ThrowsArgumentException()
    {
        // Act
        var action = () => new SideDish(null!);

        // Assert
        Assert.Throws<ArgumentException>(action);
    }
}