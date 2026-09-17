using Microsoft.EntityFrameworkCore;
using TiffinTrack.API.Models;

namespace TiffinTrack.API.Data;

public static class SeedData
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Plans.AnyAsync())
        {
            db.Plans.AddRange(
                new Plan
                {
                    Name = "Standard Lunch",
                    MonthlyPrice = 2500,
                    Description = "Homestyle weekday lunch."
                },
                new Plan
                {
                    Name = "Premium Lunch",
                    MonthlyPrice = 3500,
                    Description = "Premium weekday lunch with extra variety."
                }
            );
        }

        if (!await db.Users.AnyAsync())
        {
            db.Users.Add(new User
            {
                Name = "Demo Customer",
                Phone = "9999999999",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password@123"),
                CreatedAt = DateTime.UtcNow
            });
        }

        await db.SaveChangesAsync();
    }
}