using System.Globalization;

namespace FamilyHub.Domain.Meals;

public class WeeklyMealPlan
{
    private readonly List<MealPlanEntry> _entries = new();

    private WeeklyMealPlan()
    {
    }

    public WeeklyMealPlan(int year, int calendarWeek)
    {
        if (calendarWeek < 1 ||
            calendarWeek > ISOWeek.GetWeeksInYear(year))
        {
            throw new ArgumentOutOfRangeException(nameof(calendarWeek));
        }

        Id = Guid.NewGuid();
        Year = year;
        CalendarWeek = calendarWeek;
    }

    public Guid Id { get; private set; }

    public int Year { get; private set; }

    public int CalendarWeek { get; private set; }

    public IReadOnlyList<MealPlanEntry> Entries => _entries;

    public void AddMeal(DateOnly date, Meal meal)
    {
        var dateTime = date.ToDateTime(TimeOnly.MinValue);

        if (ISOWeek.GetYear(dateTime) != Year ||
            ISOWeek.GetWeekOfYear(dateTime) != CalendarWeek)
        {
            throw new ArgumentException(
                "Das Datum gehört nicht zu diesem Wochenplan.",
                nameof(date));
        }

        if (_entries.Any(x => x.Date == date))
        {
            throw new InvalidOperationException(
                "Für diesen Tag ist bereits ein Gericht eingetragen.");
        }

        _entries.Add(new MealPlanEntry(Id, date, meal));
    }
}