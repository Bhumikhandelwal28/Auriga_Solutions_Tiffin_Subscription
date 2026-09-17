using TiffinTrack.API.Models;

namespace TiffinTrack.API.Services;

public class BillingService
{
    public BillingResult Calculate(Subscription subscription, int year, int month)
    {
        var monthStart = new DateTime(year, month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var eligibleStart = subscription.StartDate.Date > monthStart
            ? subscription.StartDate.Date
            : monthStart;

        if (eligibleStart > monthEnd)
            return new BillingResult(subscription.User.Name, subscription.Plan.Name,
                subscription.Plan.MonthlyPrice, 0, 0, 0);

        var eligibleWeekdays = Dates(eligibleStart, monthEnd)
            .Where(IsWeekday)
            .ToList();

        var pausedWeekdays = eligibleWeekdays.Count(d =>
            subscription.PausePeriods.Any(p =>
                d >= p.StartDate.Date && d <= p.EndDate.Date));

        var delivered = eligibleWeekdays.Count - pausedWeekdays;
        var dailyRate = eligibleWeekdays.Count == 0
            ? 0
            : subscription.Plan.MonthlyPrice / eligibleWeekdays.Count;

        var total = Math.Round(dailyRate * delivered, 2);

        return new BillingResult(
            subscription.User.Name,
            subscription.Plan.Name,
            subscription.Plan.MonthlyPrice,
            eligibleWeekdays.Count,
            delivered,
            total);
    }

    private static IEnumerable<DateTime> Dates(DateTime start, DateTime end)
    {
        for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
            yield return d;
    }

    private static bool IsWeekday(DateTime d) =>
        d.DayOfWeek != DayOfWeek.Saturday &&
        d.DayOfWeek != DayOfWeek.Sunday;
}

public record BillingResult(
    string CustomerName,
    string PlanName,
    decimal MonthlyPrice,
    int EligibleWeekdays,
    int DeliveredDays,
    decimal TotalBill);
