using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TiffinTrack.API.Data;

namespace TiffinTrack.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OutboxController : ControllerBase
{
    private readonly AppDbContext _db;

    public OutboxController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var notifications = await _db.Notifications
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new
            {
                x.Id,
                x.CustomerId,
                x.Phone,
                x.Message,
                x.CreatedAt,
                x.Sent
            })
            .ToListAsync();

        return Ok(notifications);
    }
}