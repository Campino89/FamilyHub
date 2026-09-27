using FamilyHub.Domain.SideDishes;

namespace FamilyHub.Domain.Meals;

public class MealPlanEntry
{
    private MealPlanEntry()
    {
    }

    public MealPlanEntry(
        Guid weeklyMealPlanId,
        DateOnly date,
        Meal meal)
    {
        Meal = meal ?? throw new ArgumentNullException(nameof(meal));

        Id = Guid.NewGuid();
        WeeklyMealPlanId = weeklyMealPlanId;
        Date = date;
        MealId = meal.Id;
    }

    public void ChangeMeal(Meal meal)
    {
        Meal = meal ?? throw new ArgumentNullException(nameof(meal));
        MealId = meal.Id;
    }
    public Guid Id { get; private set; }

    public Guid WeeklyMealPlanId { get; private set; }

    public DateOnly Date { get; private set; }

    public Guid MealId { get; private set; }

    public Meal Meal { get; private set; } = null!;

    private readonly List<SideDish> _sideDishes = new();

    public IReadOnlyCollection<SideDish> SideDishes => _sideDishes;

    public void AddSideDish(SideDish sideDish)
    {
        ArgumentNullException.ThrowIfNull(sideDish);

        if (_sideDishes.Any(x => x.Id == sideDish.Id))
            return;

        _sideDishes.Add(sideDish);
    }

    public void RemoveSideDish(SideDish sideDish)
    {
        ArgumentNullException.ThrowIfNull(sideDish);

        _sideDishes.Remove(sideDish);
    }
}