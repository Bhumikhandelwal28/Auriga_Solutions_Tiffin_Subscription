namespace TiffinTrack.API.Models;

public enum SubscriptionStatus
{
    Active,
    Paused,
    Cancelled
}

public class Subscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PlanId { get; set; }
    public DateTime StartDate { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Plan Plan { get; set; } = null!;
    public ICollection<PausePeriod> PausePeriods { get; set; } = new List<PausePeriod>();
}
