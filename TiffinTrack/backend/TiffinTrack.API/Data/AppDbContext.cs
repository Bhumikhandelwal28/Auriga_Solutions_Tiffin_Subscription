using Microsoft.EntityFrameworkCore;
using TiffinTrack.API.Models;

namespace TiffinTrack.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
public DbSet<Plan> Plans { get; set; }
public DbSet<Subscription> Subscriptions { get; set; }
public DbSet<PausePeriod> PausePeriods { get; set; }
public DbSet<Notification> Notifications { get; set; }
}