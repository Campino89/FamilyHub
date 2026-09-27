using FamilyHub.Domain.Meals;
using FamilyHub.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Api.Controllers;

[ApiController]
[Route("api/mealcomponents")]
public class MealComponentsController : ControllerBase
{
    private readonly FamilyHubDbContext _dbContext;

    public MealComponentsController(FamilyHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var components = await _dbContext.MealComponents
            .OrderBy(x => x.Name)
            .Select(x => new
            {
                x.Id,
                x.Name
            })
            .ToListAsync();

        return Ok(components);
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateMealComponentRequest request)
    {
        var name = request.Name.Trim();

        var exists = await _dbContext.MealComponents
            .AnyAsync(x => x.Name.ToLower() == name.ToLower());

        if (exists)
            return Conflict($"Die Komponente '{name}' existiert bereits.");

        var component = new MealComponent(name);

        _dbContext.MealComponents.Add(component);
        await _dbContext.SaveChangesAsync();

        return Created(
            $"/api/mealcomponents/{component.Id}",
            new
            {
                component.Id,
                component.Name
            });
    }
}

public record CreateMealComponentRequest(string Name);