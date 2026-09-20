using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace FamilyHub.Domain.Meals;

public class WeeklyMealPlan
{
    private readonly List<MealPlanEntry> _entries = new List<MealPlanEntry>();

    public int Year { get; private set; }

    public int CalendarWeek { get; private set; }

    public IReadOnlyList<MealPlanEntry> Entries => _entries;

    public WeeklyMealPlan(int year, int calendarWeek)
    {
        if (calendarWeek < 1 || calendarWeek > ISOWeek.GetWeeksInYear(year))
        {
            throw new ArgumentOutOfRangeException(
                nameof(calendarWeek),
                $"Die Kalenderwoche muss zwischen 1 und {ISOWeek.GetWeeksInYear(year)} liegen.");
        }

        Year = year;
        CalendarWeek = calendarWeek;
    }

    public void AddMeal(DateOnly date, Meal meal)
    {
        var dateTime = date.ToDateTime(TimeOnly.MinValue);

        var calendarWeek = ISOWeek.GetWeekOfYear(dateTime);
        var year = ISOWeek.GetYear(dateTime);

        if (year != Year || calendarWeek != CalendarWeek)
        {
            throw new ArgumentException(
                $"Das Datum {date:dd.MM.yyyy} gehört nicht zur KW {CalendarWeek}/{Year}.",
                nameof(date));
        }

        if (_entries.Any(entry => entry.Datum == date))
        {
            throw new InvalidOperationException(
                $"Für den {date:dd.MM.yyyy} ist bereits ein Gericht eingetragen.");
        }

        var entry = new MealPlanEntry(date, meal);

        _entries.Add(entry);
    }
}