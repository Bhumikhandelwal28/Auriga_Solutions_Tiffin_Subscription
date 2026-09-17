namespace TiffinTrack.API.DTOs;

public record CreateSubscriptionRequest(int UserId, int PlanId, DateTime StartDate);
public record PauseRequest(DateTime StartDate, DateTime EndDate, string? Reason);
