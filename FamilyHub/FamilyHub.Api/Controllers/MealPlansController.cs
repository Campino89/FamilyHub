using FamilyHub.Domain.Meals;
using FamilyHub.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Api.Controllers;

[ApiController]
[Route("api/mealplans")]
public class MealPlansController : ControllerBase
{
    private readonly FamilyHubDbContext _dbContext;

    public MealPlansController(FamilyHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<ActionResult<WeeklyMealPlan>> Create(
        CreateMealPlanRequest request)
    {
        var existingPlan = await _dbContext.WeeklyMealPlans
            .AnyAsync(x =>
                x.Year == request.Year &&
                x.CalendarWeek == request.CalendarWeek);

        if (existingPlan)
        {
            return Conflict(
                $"Für {request.Year} / KW {request.CalendarWeek} existiert bereits ein Wochenplan.");
        }

        var plan = new WeeklyMealPlan(
            request.Year,
            request.CalendarWeek);

        _dbContext.WeeklyMealPlans.Add(plan);

        await _dbContext.SaveChangesAsync();

        return Ok(plan);
    }

    [HttpGet("{year:int}/{calendarWeek:int}")]
    public async Task<ActionResult<WeeklyMealPlan>> Get(
        int year,
        int calendarWeek)
    {
        var plan = await _dbContext.WeeklyMealPlans
            .AsNoTracking()
            .Include(x => x.Entries)
                .ThenInclude(x => x.Meal)
            .Include(x => x.Entries)
                .ThenInclude(x => x.SideDishes)
            .SingleOrDefaultAsync(x =>
                x.Year == year &&
                x.CalendarWeek == calendarWeek);

        if (plan is null)
        {
            return NotFound(
                $"Für {year} / KW {calendarWeek} wurde kein Wochenplan gefunden.");
        }

        return Ok(plan);
    }

    [HttpPost("{year:int}/{calendarWeek:int}/entries")]
    public async Task<ActionResult<MealPlanEntry>> AddEntry(
        int year,
        int calendarWeek,
        AddMealPlanEntryRequest request)
    {
        var plan = await _dbContext.WeeklyMealPlans
            .Include(x => x.Entries)
            .SingleOrDefaultAsync(x =>
                x.Year == year &&
                x.CalendarWeek == calendarWeek);

        if (plan is null)
        {
            return NotFound(
                $"Für {year} / KW {calendarWeek} wurde kein Wochenplan gefunden.");
        }

        var meal = await _dbContext.Meals
            .SingleOrDefaultAsync(x =>
                x.Id == request.MealId);

        if (meal is null)
        {
            return NotFound(
                $"Das Gericht mit der ID {request.MealId} wurde nicht gefunden.");
        }

        plan.AddMeal(request.Date, meal);

        var entry = plan.Entries
            .Single(x => x.Date == request.Date);

        _dbContext.MealPlanEntries.Add(entry);

        await _dbContext.SaveChangesAsync();

        return Ok(entry);
    }

    [HttpPut("{year:int}/{calendarWeek:int}/entries/{date}")]
    public async Task<ActionResult<MealPlanEntry>> UpdateEntry(
        int year,
        int calendarWeek,
        DateOnly date,
        UpdateMealPlanEntryRequest request)
    {
        var plan = await _dbContext.WeeklyMealPlans
            .Include(x => x.Entries)
                .ThenInclude(x => x.Meal)
            .SingleOrDefaultAsync(x =>
                x.Year == year &&
                x.CalendarWeek == calendarWeek);

        if (plan is null)
        {
            return NotFound();
        }

        var entry = plan.Entries
            .SingleOrDefault(x => x.Date == date);

        if (entry is null)
        {
            return NotFound();
        }

        var meal = await _dbContext.Meals
            .SingleOrDefaultAsync(x =>
                x.Id == request.MealId);

        if (meal is null)
        {
            return NotFound();
        }

        entry.ChangeMeal(meal);

        await _dbContext.SaveChangesAsync();

        return Ok(entry);
    }

    [HttpDelete("{year:int}/{calendarWeek:int}/entries/{date}")]
    public async Task<IActionResult> DeleteEntry(
        int year,
        int calendarWeek,
        DateOnly date)
    {
        var plan = await _dbContext.WeeklyMealPlans
            .Include(x => x.Entries)
            .SingleOrDefaultAsync(x =>
                x.Year == year &&
                x.CalendarWeek == calendarWeek);

        if (plan is null)
        {
            return NotFound();
        }

        var entry = plan.Entries
            .SingleOrDefault(x => x.Date == date);

        if (entry is null)
        {
            return NotFound();
        }

        _dbContext.MealPlanEntries.Remove(entry);

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{year:int}/{calendarWeek:int}")]
    public async Task<IActionResult> SaveWeek(
        int year,
        int calendarWeek,
        SaveWeekRequest request)
    {
        var plan = await _dbContext.WeeklyMealPlans
            .Include(x => x.Entries)
                .ThenInclude(x => x.Meal)
            .SingleOrDefaultAsync(x =>
                x.Year == year &&
                x.CalendarWeek == calendarWeek);

        if (plan is null)
        {
            return NotFound(new
            {
                message =
                    $"Für {year} / KW {calendarWeek} wurde kein Wochenplan gefunden."
            });
        }

        var requestedMealIds = request.Entries
            .Where(x => x.MealId.HasValue)
            .Select(x => x.MealId!.Value)
            .Distinct()
            .ToList();

        var meals = await _dbContext.Meals
            .Where(x => requestedMealIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        foreach (var requestedEntry in request.Entries)
        {
            var existingEntry = plan.Entries
                .SingleOrDefault(x =>
                    x.Date == requestedEntry.Date);

            if (requestedEntry.MealId is null)
            {
                if (existingEntry is not null)
                {
                    _dbContext.MealPlanEntries
                        .Remove(existingEntry);
                }

                continue;
            }

            if (!meals.TryGetValue(
                    requestedEntry.MealId.Value,
                    out var meal))
            {
                return BadRequest(new
                {
                    message =
                        $"Das Gericht mit der ID {requestedEntry.MealId} wurde nicht gefunden."
                });
            }

            if (existingEntry is null)
            {
                plan.AddMeal(
                    requestedEntry.Date,
                    meal);

                var newEntry = plan.Entries
                    .Single(x =>
                        x.Date == requestedEntry.Date);

                _dbContext.MealPlanEntries
                    .Add(newEntry);
            }
            else
            {
                existingEntry.ChangeMeal(meal);
            }
        }

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    // ---------------------------------------------------------
    // Side Dishes
    // ---------------------------------------------------------

    [HttpGet(
        "{year:int}/{calendarWeek:int}/entries/{date}/sidedishes")]
    public async Task<ActionResult> GetSideDishes(
        int year,
        int calendarWeek,
        DateOnly date)
    {
        var plan = await _dbContext.WeeklyMealPlans
            .AsNoTracking()
            .Include(x => x.Entries)
                .ThenInclude(x => x.SideDishes)
            .SingleOrDefaultAsync(x =>
                x.Year == year &&
                x.CalendarWeek == calendarWeek);

        if (plan is null)
        {
            return NotFound(
                $"Für {year} / KW {calendarWeek} wurde kein Wochenplan gefunden.");
        }

        var entry = plan.Entries
            .SingleOrDefault(x => x.Date == date);

        if (entry is null)
        {
            return NotFound(
                $"Für den {date} wurde kein Eintrag gefunden.");
        }

        var sideDishes = entry.SideDishes
            .OrderBy(x => x.Name)
            .Select(x => new
            {
                x.Id,
                x.Name
            });

        return Ok(sideDishes);
    }

    [HttpPut(
        "{year:int}/{calendarWeek:int}/entries/{date}/sidedishes")]
    public async Task<IActionResult> SetSideDishes(
        int year,
        int calendarWeek,
        DateOnly date,
        SetSideDishesRequest request)
    {
        var plan = await _dbContext.WeeklyMealPlans
            .Include(x => x.Entries)
                .ThenInclude(x => x.SideDishes)
            .SingleOrDefaultAsync(x =>
                x.Year == year &&
                x.CalendarWeek == calendarWeek);

        if (plan is null)
        {
            return NotFound(
                $"Für {year} / KW {calendarWeek} wurde kein Wochenplan gefunden.");
        }

        var entry = plan.Entries
            .SingleOrDefault(x => x.Date == date);

        if (entry is null)
        {
            return NotFound(
                $"Für den {date} wurde kein Eintrag gefunden.");
        }

        var sideDishIds = request.SideDishIds
            .Distinct()
            .ToList();

        var sideDishes = await _dbContext.SideDishes
            .Where(x => sideDishIds.Contains(x.Id))
            .ToListAsync();

        if (sideDishes.Count != sideDishIds.Count)
        {
            return BadRequest(
                "Mindestens eine angegebene Beilage wurde nicht gefunden.");
        }

        var existingSideDishes =
            entry.SideDishes.ToList();

        foreach (var sideDish in existingSideDishes)
        {
            entry.RemoveSideDish(sideDish);
        }

        foreach (var sideDish in sideDishes)
        {
            entry.AddSideDish(sideDish);
        }

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}


// ---------------------------------------------------------
// Requests
// ---------------------------------------------------------

public record CreateMealPlanRequest(
    int Year,
    int CalendarWeek);

public record AddMealPlanEntryRequest(
    DateOnly Date,
    Guid MealId);

public record UpdateMealPlanEntryRequest(
    Guid MealId);

public record SaveWeekRequest(
    List<SaveWeekEntryRequest> Entries);

public record SaveWeekEntryRequest(
    DateOnly Date,
    Guid? MealId);

public record SetSideDishesRequest(
    List<Guid> SideDishIds);