/*
using GraduationProjectApi.DTOs;
using GraduationProjectApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationProjectApi.Controllers;
[ApiController]
[Route("services")]
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
   

    [HttpPost("add")]
    [Authorize]
    public async Task<IActionResult> AddService([FromBody] AddServiceDto serviceDto)
    {
        
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized(new { error = "Invalid token: missing user identifier claim." });

        var userId = userIdClaim.Value;
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null)
            return Unauthorized(new { error = "User not found for the provided token." });

        if (user.IsSuspended)
            return Unauthorized(new { error = "User account is suspended." });

        if (string.IsNullOrWhiteSpace(serviceDto.Title))
            return BadRequest(new { error = "Title is required." });

        if (serviceDto.Title.Length > 100)
            return BadRequest(new { error = "Title cannot exceed 100 characters." });

        if (!string.IsNullOrWhiteSpace(serviceDto.Description) && serviceDto.Description.Length > 500)
            return BadRequest(new { error = "Description cannot exceed 500 characters." });

        var trade = await _dbContext.Trades.FirstOrDefaultAsync(t => t.TradeId == serviceDto.TradeId);
        if (trade == null)
            return BadRequest(new { error = "Invalid trade ID." });

        var location = await _dbContext.Locations.FirstOrDefaultAsync(l => l.LocationId == serviceDto.LocationId);
        if (location == null)
            return BadRequest(new { error = "Invalid location ID." });

        var service = new Service
        {
            ServiceName = serviceDto.Title, // keep your existing property name
            Description = serviceDto.Description,
            UserId = user.UserId,
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
    
    [HttpGet("tags")]
    [Authorize]
    public async Task<IActionResult> GetTags()
    {
        var tags = await _dbContext.Tags.ToListAsync();
        return Ok( new
        {
            message = "Success",
            data = tags
        });
    }
}

*/