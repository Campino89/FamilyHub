using FamilyHub.Domain.Meals;
using FamilyHub.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Api.Controllers;

[ApiController]
[Route("api/sidedishes")]
public class SideDishesController : ControllerBase
{
    private readonly FamilyHubDbContext _dbContext;

    public SideDishesController(FamilyHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var sideDishes = await _dbContext.SideDishes
            .OrderBy(x => x.Name)
            .Select(x => new
            {
                x.Id,
                x.Name
            })
            .ToListAsync();

        return Ok(sideDishes);
    }

    [HttpPost]
    public async Task<ActionResult> Create(
        CreateSideDishRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Der Name der Beilage darf nicht leer sein.");
        }

        var name = request.Name.Trim();

        var exists = await _dbContext.SideDishes
            .AnyAsync(x => x.Name.ToLower() == name.ToLower());

        if (exists)
        {
            return Conflict(
                $"Die Beilage '{name}' existiert bereits.");
        }

        var sideDish = new SideDish(name);

        _dbContext.SideDishes.Add(sideDish);

        await _dbContext.SaveChangesAsync();

        return Created(
            $"/api/sidedishes/{sideDish.Id}",
            new
            {
                sideDish.Id,
                sideDish.Name
            });
    }
}

public record CreateSideDishRequest(string Name);