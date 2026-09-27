namespace FamilyHub.Domain.Meals;

public class MealComponent
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    public ICollection<Meal> Meals { get; private set; }
        = new List<Meal>();

    private MealComponent()
    {
    }

    public MealComponent(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Der Name der Zutat darf nicht leer sein.",
                nameof(name));

        Id = Guid.NewGuid();
        Name = name.Trim();
    }
}