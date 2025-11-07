using GraduationProjectApi.DTOs;
using GraduationProjectApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationProjectApi.Controllers;
[ApiController]
[Route("mod")]
public class ModsController(ApplicationDbContext dbContext) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    [HttpPost("services/locations")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddNewLocation([FromBody] LocationDTO newLocationDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var isLocationExists = await _dbContext.Locations.AnyAsync(l => l.City == newLocationDto.City);
        if (isLocationExists)
        {
            return BadRequest(new
            {
                error = "The city already exists."
            });
        }

        var location = new Location
        {
            City = newLocationDto.City
        };

        _dbContext.Locations.Add(location);
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Location added successfully.",
            locationId = location.LocationId
        });
    }

    [HttpDelete("services/locations/{locationId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteLocation([FromRoute] string locationId)
    {
        var location = await _dbContext.Locations.FirstOrDefaultAsync(l => l.LocationId == locationId);
        if (location == null)
        {
            return NotFound(new
            {
                message = "Location not found."
            });
        }

        _dbContext.Locations.Remove(location);
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "The location has been deleted successfully."
        });
    }

    [HttpPut("services/locations/{locationId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateLocation([FromRoute] string locationId, [FromBody] LocationDTO updateLocationDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var location = await _dbContext.Locations.FirstOrDefaultAsync(l => l.LocationId == locationId);
        if (location == null)
        {
            return NotFound(new
            {
                message = "Location not found."
            });
        }

        location.City = updateLocationDto.City;

        _dbContext.Locations.Update(location);
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "The location updated successfully."
        });
    }

}
