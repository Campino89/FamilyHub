using FamilyHub.Domain.Meals;
using FamilyHub.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MealsController : ControllerBase
{
    private readonly FamilyHubDbContext _dbContext;

    public MealsController(FamilyHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<Meal>>> GetAll()
    {
        var meals = await _dbContext.Meals
            .AsNoTracking()
            .ToListAsync();

        return Ok(meals);
    }

    [HttpPost]
    public async Task<ActionResult<Meal>> Create(CreateMealRequest request)
    {
        var name = request.Name.Trim();

        var exists = await _dbContext.Meals
            .AnyAsync(x => x.Name.ToLower() == name.ToLower());

        if (exists)
        {
            return Conflict(new
            {
                message = $"Das Gericht '{name}' existiert bereits."
            });
        }

        var meal = new Meal(name);

        _dbContext.Meals.Add(meal);
        await _dbContext.SaveChangesAsync();

        return Ok(meal);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var meal = await _dbContext.Meals.FindAsync(id);

        if (meal is null)
        {
            return NotFound(new
            {
                message = "Das Gericht wurde nicht gefunden."
            });
        }

        var isUsedInMealPlan = await _dbContext.MealPlanEntries
            .AnyAsync(x => x.MealId == id);

        if (isUsedInMealPlan)
        {
            return Conflict(new
            {
                message = "Das Gericht wird noch in einem Wochenplan verwendet und kann deshalb nicht gelöscht werden."
            });
        }

        _dbContext.Meals.Remove(meal);
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
                message = $"Für {year} / KW {calendarWeek} wurde kein Wochenplan gefunden."
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
                .SingleOrDefault(x => x.Date == requestedEntry.Date);

            // Tag soll leer sein
            if (requestedEntry.MealId is null)
            {
                if (existingEntry is not null)
                {
                    _dbContext.MealPlanEntries.Remove(existingEntry);
                }

                continue;
            }

            // Prüfen, ob Gericht existiert
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

            // Neuer Tag
            if (existingEntry is null)
            {
                plan.AddMeal(
                    requestedEntry.Date,
                    meal);

                var newEntry = plan.Entries
                    .Single(x => x.Date == requestedEntry.Date);

                _dbContext.MealPlanEntries.Add(newEntry);
            }
            else
            {
                // Vorhandenen Tag ändern
                existingEntry.ChangeMeal(meal);
            }
        }

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{mealId:guid}/components")]
    public async Task<ActionResult> SetComponents(
    Guid mealId,
    SetMealComponentsRequest request)
    {
        var meal = await _dbContext.Meals
            .Include(x => x.Components)
            .FirstOrDefaultAsync(x => x.Id == mealId);

        if (meal is null)
            return NotFound("Gericht wurde nicht gefunden.");

        var componentIds = request.ComponentIds
            .Distinct()
            .ToList();

        var components = await _dbContext.MealComponents
            .Where(x => componentIds.Contains(x.Id))
            .ToListAsync();

        if (components.Count != componentIds.Count)
            return BadRequest(
                "Mindestens eine angegebene Komponente wurde nicht gefunden.");

        var existingComponents = meal.Components.ToList();

        foreach (var component in existingComponents)
            meal.RemoveComponent(component);

        foreach (var component in components)
            meal.AddComponent(component);

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{mealId:guid}/components")]
    public async Task<ActionResult> GetComponents(Guid mealId)
    {
        var meal = await _dbContext.Meals
            .Include(x => x.Components)
            .FirstOrDefaultAsync(x => x.Id == mealId);

        if (meal is null)
            return NotFound();

        var components = meal.Components
            .OrderBy(x => x.Name)
            .Select(x => new
            {
                x.Id,
                x.Name
            });

        return Ok(components);
    }
}

public record CreateMealRequest(string Name);

public record SetMealComponentsRequest(
    List<Guid> ComponentIds);