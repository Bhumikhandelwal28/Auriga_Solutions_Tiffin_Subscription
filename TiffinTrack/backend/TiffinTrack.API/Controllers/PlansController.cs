using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiffinTrack.API.Data;
using TiffinTrack.API.Models;

namespace TiffinTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlansController : ControllerBase
{
    private readonly AppDbContext _db;

    public PlansController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/plans
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var plans = await _db.Plans
            .Where(x => x.IsActive)
            .OrderBy(x => x.MonthlyPrice)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Description,
                x.MonthlyPrice,
                x.IsActive
            })
            .ToListAsync();

        return Ok(plans);
    }

    // GET: api/plans/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOne(int id)
    {
        var plan = await _db.Plans
            .Where(x => x.Id == id)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Description,
                x.MonthlyPrice,
                x.IsActive
            })
            .FirstOrDefaultAsync();

        if (plan == null)
        {
            return NotFound("Plan not found.");
        }

        return Ok(plan);
    }

    // POST: api/plans
    [HttpPost]
    public async Task<IActionResult> Create(Plan plan)
    {
        if (string.IsNullOrWhiteSpace(plan.Name))
        {
            return BadRequest("Plan name is required.");
        }

        if (plan.MonthlyPrice <= 0)
        {
            return BadRequest("Monthly price must be greater than 0.");
        }

        plan.Id = 0;
        plan.Name = plan.Name.Trim();
        plan.IsActive = true;

        _db.Plans.Add(plan);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetOne),
            new { id = plan.Id },
            plan
        );
    }

    // PUT: api/plans/1
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Plan input)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
        {
            return BadRequest("Plan name is required.");
        }

        if (input.MonthlyPrice <= 0)
        {
            return BadRequest("Monthly price must be greater than 0.");
        }

        var plan = await _db.Plans.FindAsync(id);

        if (plan == null)
        {
            return NotFound("Plan not found.");
        }

        plan.Name = input.Name.Trim();
        plan.MonthlyPrice = input.MonthlyPrice;
        plan.Description = input.Description;

        await _db.SaveChangesAsync();

        return Ok(plan);
    }

    // DELETE: api/plans/1
    // Soft delete
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var plan = await _db.Plans.FindAsync(id);

        if (plan == null)
        {
            return NotFound("Plan not found.");
        }

        plan.IsActive = false;

        await _db.SaveChangesAsync();

        return NoContent();
    }
}