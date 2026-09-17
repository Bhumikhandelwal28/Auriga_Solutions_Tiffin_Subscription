using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiffinTrack.API.Data;
using TiffinTrack.API.Models;
using TiffinTrack.API.Services;

namespace TiffinTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillingController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly BillingService _billing;

    public BillingController(
        AppDbContext db,
        BillingService billing)
    {
        _db = db;
        _billing = billing;
    }

    // GET: /api/billing/{customerId}?year=2026&month=9
    [HttpGet("{customerId}")]
    public async Task<IActionResult> Get(
        int customerId,
        int year,
        int month)
    {
        if (month < 1 || month > 12)
        {
            return BadRequest("Month must be between 1 and 12.");
        }

        var subscription = await _db.Subscriptions
            .Include(x => x.User)
            .Include(x => x.Plan)
            .Include(x => x.PausePeriods)
            .Where(x =>
                x.UserId == customerId &&
                x.Status != SubscriptionStatus.Cancelled)
            .OrderByDescending(x => x.StartDate)
            .FirstOrDefaultAsync();

        if (subscription is null)
        {
            return NotFound(
                "No subscription found for customer.");
        }

        return Ok(
            _billing.Calculate(
                subscription,
                year,
                month));
    }
}