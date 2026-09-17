using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiffinTrack.API.Data;
using TiffinTrack.API.DTOs;
using TiffinTrack.API.Models;

namespace TiffinTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly AppDbContext _db;

    public SubscriptionsController(AppDbContext db)
    {
        _db = db;
    }

    // POST: api/subscriptions
    [HttpPost]
    public async Task<IActionResult> Create(CreateSubscriptionRequest request)
    {
        var user = await _db.Users.FindAsync(request.UserId);

        if (user == null)
        {
            return BadRequest("Customer not found.");
        }

        var plan = await _db.Plans.FindAsync(request.PlanId);

        if (plan == null || !plan.IsActive)
        {
            return BadRequest("Valid active plan is required.");
        }

        // Customer can have only one active/paused subscription
        var existingSubscription = await _db.Subscriptions
            .AnyAsync(x =>
                x.UserId == request.UserId &&
                (x.Status == SubscriptionStatus.Active ||
                 x.Status == SubscriptionStatus.Paused));

        if (existingSubscription)
        {
            return Conflict(
                "Customer already has an active or paused subscription."
            );
        }

        var subscription = new Subscription
        {
            UserId = request.UserId,
            PlanId = request.PlanId,
            StartDate = request.StartDate.Date,
            Status = SubscriptionStatus.Active
        };

        _db.Subscriptions.Add(subscription);

        await _db.SaveChangesAsync();

        return Ok(await Load(subscription.Id));
    }

    // GET: api/subscriptions/1
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await Load(id);

        if (result == null)
        {
            return NotFound("Subscription not found.");
        }

        return Ok(result);
    }

    // POST: api/subscriptions/1/pause
    [HttpPost("{id}/pause")]
    public async Task<IActionResult> Pause(
        int id,
        PauseRequest request)
    {
        var start = request.StartDate.Date;
        var end = request.EndDate.Date;

        if (end < start)
        {
            return BadRequest(
                "End date must be on or after start date."
            );
        }

        var subscription = await _db.Subscriptions
            .Include(x => x.PausePeriods)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (subscription == null)
        {
            return NotFound("Subscription not found.");
        }

        if (subscription.Status == SubscriptionStatus.Cancelled)
        {
            return BadRequest(
                "Cancelled subscription cannot be paused."
            );
        }

        if (start < subscription.StartDate.Date)
        {
            return BadRequest(
                "Pause cannot start before subscription start date."
            );
        }

        // Reject overlapping pause periods
        var overlap = subscription.PausePeriods.Any(p =>
            start <= p.EndDate.Date &&
            end >= p.StartDate.Date);

        if (overlap)
        {
            return Conflict(
                "Pause period overlaps an existing pause."
            );
        }

        subscription.PausePeriods.Add(new PausePeriod
        {
            StartDate = start,
            EndDate = end,
            Reason = request.Reason?.Trim() ?? ""
        });

        subscription.Status = SubscriptionStatus.Paused;

        await _db.SaveChangesAsync();

        return Ok(await Load(id));
    }

    // POST: api/subscriptions/1/resume
    [HttpPost("{id}/resume")]
    public async Task<IActionResult> Resume(int id)
    {
        var subscription = await _db.Subscriptions.FindAsync(id);

        if (subscription == null)
        {
            return NotFound("Subscription not found.");
        }

        if (subscription.Status == SubscriptionStatus.Cancelled)
        {
            return BadRequest(
                "Cancelled subscription cannot be resumed."
            );
        }

        subscription.Status = SubscriptionStatus.Active;

        await _db.SaveChangesAsync();

        return Ok(await Load(id));
    }

    // POST: api/subscriptions/1/cancel
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(int id)
    {
        var subscription = await _db.Subscriptions.FindAsync(id);

        if (subscription == null)
        {
            return NotFound("Subscription not found.");
        }

        if (subscription.Status == SubscriptionStatus.Cancelled)
        {
            return BadRequest(
                "Subscription is already cancelled."
            );
        }

        subscription.Status = SubscriptionStatus.Cancelled;

        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Subscription cancelled successfully.",
            subscriptionId = id
        });
    }

    // GET: api/subscriptions/1/billing
    [HttpGet("{id}/billing")]
    public async Task<IActionResult> Billing(int id)
    {
        var subscription = await _db.Subscriptions
            .Include(x => x.Plan)
            .Include(x => x.PausePeriods)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (subscription == null)
        {
            return NotFound("Subscription not found.");
        }

        var today = DateTime.Today;

        var monthStart = new DateTime(
            today.Year,
            today.Month,
            1
        );

        var monthEnd = monthStart
            .AddMonths(1)
            .AddDays(-1);

        // Billing should not start before subscription starts
        var billingStart = subscription.StartDate.Date > monthStart
            ? subscription.StartDate.Date
            : monthStart;

        if (billingStart > monthEnd)
        {
            return Ok(new
            {
                subscriptionId = subscription.Id,
                month = monthStart.ToString("yyyy-MM"),
                totalWeekdays = 0,
                pausedWeekdays = 0,
                deliveredWeekdays = 0,
                monthlyPrice = subscription.Plan.MonthlyPrice,
                amount = 0
            });
        }

        // Count weekdays in billing period
        int totalWeekdays = 0;

        for (
            var date = billingStart;
            date <= monthEnd;
            date = date.AddDays(1))
        {
            if (IsWeekday(date))
            {
                totalWeekdays++;
            }
        }

        // Count paused weekdays
        int pausedWeekdays = 0;

        foreach (var pause in subscription.PausePeriods)
        {
            var pauseStart = pause.StartDate.Date > billingStart
                ? pause.StartDate.Date
                : billingStart;

            var pauseEnd = pause.EndDate.Date < monthEnd
                ? pause.EndDate.Date
                : monthEnd;

            if (pauseStart > pauseEnd)
            {
                continue;
            }

            for (
                var date = pauseStart;
                date <= pauseEnd;
                date = date.AddDays(1))
            {
                if (IsWeekday(date))
                {
                    pausedWeekdays++;
                }
            }
        }

        var deliveredWeekdays =
            Math.Max(totalWeekdays - pausedWeekdays, 0);

        decimal amount = 0;

        if (totalWeekdays > 0)
        {
            amount = Math.Round(
                subscription.Plan.MonthlyPrice *
                deliveredWeekdays /
                totalWeekdays,
                2
            );
        }

        return Ok(new
        {
            subscriptionId = subscription.Id,
            month = monthStart.ToString("yyyy-MM"),
            billingStart,
            billingEnd = monthEnd,

            totalWeekdays,
            pausedWeekdays,
            deliveredWeekdays,

            monthlyPrice = subscription.Plan.MonthlyPrice,
            amount
        });
    }

    private static bool IsWeekday(DateTime date)
    {
        return date.DayOfWeek != DayOfWeek.Saturday &&
               date.DayOfWeek != DayOfWeek.Sunday;
    }

    // Common subscription loader
    private async Task<object?> Load(int id)
    {
        return await _db.Subscriptions
            .Include(x => x.User)
            .Include(x => x.Plan)
            .Include(x => x.PausePeriods)
            .Where(x => x.Id == id)
            .Select(x => new
            {
                x.Id,

                x.UserId,
                CustomerName = x.User.Name,
                CustomerPhone = x.User.Phone,

                x.PlanId,
                PlanName = x.Plan.Name,
                x.Plan.MonthlyPrice,

                x.StartDate,

                Status = x.Status.ToString(),

                PausePeriods = x.PausePeriods
                    .Select(p => new
                    {
                        p.Id,
                        p.StartDate,
                        p.EndDate,
                        p.Reason
                    })
                    .ToList()
            })
            .SingleOrDefaultAsync();
    }
}