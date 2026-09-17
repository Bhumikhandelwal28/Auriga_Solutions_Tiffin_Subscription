using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiffinTrack.API.Data;
using TiffinTrack.API.Models;

namespace TiffinTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly AppDbContext _db;

    public CustomersController(AppDbContext db)
    {
        _db = db;
    }

    // GET: api/customers
    // Example:
    // /api/customers?search=9876&page=1&pageSize=10&sortBy=name&sortOrder=asc
    [HttpGet]
    public async Task<IActionResult> Get(
        string? search = null,
        int page = 1,
        int pageSize = 10,
        string sortBy = "name",
        string sortOrder = "asc")
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 50);

        var query = _db.Users
            .Include(x => x.Subscriptions)
            .ThenInclude(x => x.Plan)
            .AsQueryable();

        // Search by name or phone
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchText = search.Trim();

            query = query.Where(x =>
                x.Name.Contains(searchText) ||
                x.Phone.Contains(searchText));
        }

        // Sorting
        query = sortBy.ToLower() switch
        {
            "phone" => sortOrder.ToLower() == "desc"
                ? query.OrderByDescending(x => x.Phone)
                : query.OrderBy(x => x.Phone),

            _ => sortOrder.ToLower() == "desc"
                ? query.OrderByDescending(x => x.Name)
                : query.OrderBy(x => x.Name)
        };

        var total = await query.CountAsync();

        var customers = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.Phone,

                ActiveSubscription = x.Subscriptions
                    .Where(s => s.Status == SubscriptionStatus.Active)
                    .Select(s => new
                    {
                        s.Id,
                        Plan = s.Plan.Name
                    })
                    .FirstOrDefault(),

                PausedSubscription = x.Subscriptions
                    .Where(s => s.Status == SubscriptionStatus.Paused)
                    .Select(s => new
                    {
                        s.Id,
                        Plan = s.Plan.Name
                    })
                    .FirstOrDefault()
            })
            .ToListAsync();

        return Ok(new
        {
            items = customers,
            total,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(
                total / (double)pageSize
            )
        });
    }

    // GET: api/customers/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOne(int id)
    {
        var user = await _db.Users
            .Include(x => x.Subscriptions)
            .ThenInclude(x => x.Plan)
            .Include(x => x.Subscriptions)
            .ThenInclude(x => x.PausePeriods)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (user == null)
        {
            return NotFound("Customer not found.");
        }

        return Ok(new
        {
            user.Id,
            user.Name,
            user.Phone,

            Subscriptions = user.Subscriptions.Select(s => new
            {
                s.Id,
                s.PlanId,
                PlanName = s.Plan.Name,
                s.Plan.MonthlyPrice,
                s.StartDate,
                Status = s.Status.ToString(),

                PausePeriods = s.PausePeriods.Select(p => new
                {
                    p.Id,
                    p.StartDate,
                    p.EndDate,
                    p.Reason
                })
            })
        });
    }

    // GET: api/customers/1/subscriptions
    [HttpGet("{id}/subscriptions")]
    public async Task<IActionResult> GetSubscriptions(int id)
    {
        var customerExists = await _db.Users
            .AnyAsync(x => x.Id == id);

        if (!customerExists)
        {
            return NotFound("Customer not found.");
        }

        var subscriptions = await _db.Subscriptions
            .Where(x => x.UserId == id)
            .Include(x => x.Plan)
            .Include(x => x.PausePeriods)
            .Select(x => new
            {
                x.Id,
                x.UserId,
                x.PlanId,
                PlanName = x.Plan.Name,
                x.Plan.MonthlyPrice,
                x.StartDate,
                Status = x.Status.ToString(),

                PausePeriods = x.PausePeriods.Select(p => new
                {
                    p.Id,
                    p.StartDate,
                    p.EndDate,
                    p.Reason
                })
            })
            .ToListAsync();

        return Ok(subscriptions);
    }
}