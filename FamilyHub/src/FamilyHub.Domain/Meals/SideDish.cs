namespace FamilyHub.Domain.Meals;

public class SideDish
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    private SideDish()
    {
    }

    public SideDish(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(
                "Der Name der Beilage darf nicht leer sein.",
                nameof(name));
        }

        Id = Guid.NewGuid();
        Name = name.Trim();
    }
}