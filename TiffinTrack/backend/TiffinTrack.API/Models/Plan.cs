namespace TiffinTrack.API.Models;

public class Plan

{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal MonthlyPrice { get; set; }
    public string Description { get; set; } = "";
    public bool IsActive { get; set; } = true;

    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
