using TiffinTrack.API.Data;
using TiffinTrack.API.Models;

namespace TiffinTrack.API.Services;

public class NotificationService
{
    private readonly AppDbContext _db;

    public NotificationService(AppDbContext db)
    {
        _db = db;
    }

    public async Task NotifyDeliveryAsync(
        int customerId,
        string phone,
        string customerName,
        DateTime date)
    {
        var notification = new Notification
        {
            CustomerId = customerId,
            Phone = phone,
            Message = $"Hi {customerName}, your tiffin is scheduled for delivery today.",
            CreatedAt = DateTime.UtcNow,
            Sent = true
        };

        _db.Notifications.Add(notification);

        await _db.SaveChangesAsync();
    }
}