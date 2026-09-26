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

        // Explizit als neuen Datensatz markieren
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
            .SingleOrDefaultAsync(x => x.Id == request.MealId);

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
}

    public record CreateMealPlanRequest(
    int Year,
    int CalendarWeek);

public record AddMealPlanEntryRequest(
    DateOnly Date,
    Guid MealId);

public record UpdateMealPlanEntryRequest(Guid MealId);