using GraduationProjectApi.DTOs;
using GraduationProjectApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationProjectApi.Controllers;

[ApiController]
[Route("service")]
public class ServicesController(ApplicationDbContext dbContext) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    /*
        [HttpPost("delete")]
        [Authorize]
        public async Task<IActionResult> DeleteService()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { error = "Invalid token." });
            }

            var userId = userIdClaim.Value;


        }
    */

    [HttpPost("add")]
    [Authorize]
    public async Task<IActionResult> AddService([FromBody] AddServiceDto serviceDto)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !User.Identity.IsAuthenticated)
        {
            return Unauthorized(new { error = "Invalid token." });
        }

        var userId = userIdClaim.Value;

        var trade = await _dbContext.Trades.FirstOrDefaultAsync(t => t.TradeId == serviceDto.TradeId);
        if (trade == null)
        {
            return BadRequest(new
            {
                error = "Invalid trade ID."
            });
        }

        var location = await _dbContext.Locations.FirstOrDefaultAsync(l => l.LocationId == serviceDto.LocationId);
        if (location == null)
        {
            return BadRequest(new
            {
                error = "Invalid location ID."
            });
        }

        var service = new Service
        {
            ServiceName = serviceDto.Title,
            Description = serviceDto.Description,
            UserId = userId,
            TradeId = serviceDto.TradeId,
            LocationId = serviceDto.LocationId
        };

        _dbContext.Services.Add(service);
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Service added successfully.",
            ServiceId = service.ServiceId
        });
    }
}
