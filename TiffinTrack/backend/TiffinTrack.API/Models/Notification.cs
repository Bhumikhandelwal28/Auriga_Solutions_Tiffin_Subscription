namespace TiffinTrack.API.Models;

public class Notification
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string Phone { get; set; } = "";

    public string Message { get; set; } = "";

    public DateTime CreatedAt { get; set; }

    public bool Sent { get; set; }
}