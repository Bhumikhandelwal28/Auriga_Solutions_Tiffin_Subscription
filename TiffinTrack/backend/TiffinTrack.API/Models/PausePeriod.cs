namespace TiffinTrack.API.Models;

public class PausePeriod
{
    public int Id { get; set; }
    public int SubscriptionId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = "";

    public Subscription Subscription { get; set; } = null!;
}
