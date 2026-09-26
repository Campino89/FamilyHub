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
        var meal = new Meal(request.Name);

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
            return NotFound();
        }

        _dbContext.Meals.Remove(meal);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}

public record CreateMealRequest(string Name);