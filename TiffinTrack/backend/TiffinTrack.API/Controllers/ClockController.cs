using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiffinTrack.API.Data;
using TiffinTrack.API.Models;
using TiffinTrack.API.Services;

namespace TiffinTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClockController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly NotificationService _notifications;

    public ClockController(
        AppDbContext db,
        NotificationService notifications)
    {
        _db = db;
        _notifications = notifications;
    }

    // POST: /api/clock
    [HttpPost]
    public async Task<IActionResult> Run(DateTime? date = null)
    {
        var today = (date ?? DateTime.Today).Date;

        // No delivery on weekends
        if (today.DayOfWeek == DayOfWeek.Saturday ||
            today.DayOfWeek == DayOfWeek.Sunday)
        {
            return Ok(new
            {
                date = today,
                message = "Weekend - no tiffin deliveries.",
                notified = 0
            });
        }

        var subscriptions = await _db.Subscriptions
            .Include(x => x.User)
            .Include(x => x.PausePeriods)
            .Where(x => x.Status == SubscriptionStatus.Active)
            .ToListAsync();

        var notified = 0;

        foreach (var subscription in subscriptions)
        {
            var paused = subscription.PausePeriods.Any(p =>
                today >= p.StartDate.Date &&
                today <= p.EndDate.Date);

            if (paused)
                continue;

            // Avoid duplicate notification for same customer/day
            var alreadyNotified = await _db.Notifications.AnyAsync(n =>
                n.CustomerId == subscription.UserId &&
                n.CreatedAt.Date == DateTime.UtcNow.Date &&
                n.Message.Contains("scheduled for delivery"));

            if (alreadyNotified)
                continue;

            await _notifications.NotifyDeliveryAsync(
                subscription.UserId,
                subscription.User.Phone,
                subscription.User.Name,
                today);

            notified++;
        }

        return Ok(new
        {
            date = today,
            notified
        });
    }
}