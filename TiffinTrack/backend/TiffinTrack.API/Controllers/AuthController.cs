using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiffinTrack.API.Data;
using TiffinTrack.API.DTOs;
using TiffinTrack.API.Models;

namespace TiffinTrack.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;

    public AuthController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.Phone) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Name, phone and password are required.");
        }

        var phone = request.Phone.Trim();

        if (await _db.Users.AnyAsync(x => x.Phone == phone))
        {
            return Conflict("Phone already registered.");
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Phone = phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            message = "Registration successful",
            userId = user.Id,
            user.Name,
            user.Phone
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Phone) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Phone and password are required.");
        }

        var phone = request.Phone.Trim();

        var user = await _db.Users
            .FirstOrDefaultAsync(x => x.Phone == phone);

        if (user == null)
        {
            return Unauthorized("Invalid credentials.");
        }

        var valid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash
        );

        if (!valid)
        {
            return Unauthorized("Invalid credentials.");
        }

        return Ok(new
        {
            message = "Login successful",
            user.Id,
            user.Name,
            user.Phone
        });
    }
}