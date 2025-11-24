using GraduationProjectApi.DTOs;
using GraduationProjectApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationProjectApi.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/services")]
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

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateServiceAsync([FromForm] CreateServiceRequestDto serviceDto)
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

        if (serviceDto.ServiceImages != null && serviceDto.ServiceImages.Count > 5)
            return BadRequest(new { error = "Maximum 5 images allowed." });


/*
        var tags = new List<Tag>();
        if (serviceDto.TagIds != null && serviceDto.TagIds.Count > 0)
        {
            var foundTags = await _dbContext.Tags.Where(t => serviceDto.TagIds.Contains(t.TagId)).ToListAsync();
            if (foundTags.Count != serviceDto.TagIds.Count)
                return BadRequest(new { error = "One or more TagIds are invalid." });
            tags.AddRange(foundTags);
        }
        */
        var service = new Service
        {
            ServiceName = serviceDto.Title,
            Description = serviceDto.Description,
            UserId = user.UserId,
        };

        _dbContext.Services.Add(service);
        await _dbContext.SaveChangesAsync();
/*
        if (tags.Count > 0)
        {
            foreach (var tag in tags.Take(10))
            {
                var st = new ServiceTag
                {
                    ServiceId = service.ServiceId,
                    TagId = tag.TagId
                };
                _dbContext.ServiceTags.Add(st);
            }
            await _dbContext.SaveChangesAsync();
        }
*/

        if (serviceDto.ServiceImages != null && serviceDto.ServiceImages.Any())
        {
             var folderPath = Path.Combine("wwwroot", "service", service.ServiceId, "images");   
             if (!Directory.Exists(folderPath))
             {
                 Directory.CreateDirectory(folderPath);
             }

             foreach (var image in serviceDto.ServiceImages)
             {
                 if (image.Length > 0)
                 {
                     var fileExtension = Path.GetExtension(image.FileName);
                     var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                     if (!allowedExtensions.Contains(fileExtension.ToLower()))
                     {
                         return BadRequest(new { error = "Invalid image format. Allowed formats: jpg, jpeg, png." });
                     }

                     var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                     var filePath = Path.Combine(folderPath, uniqueFileName);

                     using (var stream = new FileStream(filePath, FileMode.Create))
                     {
                         await image.CopyToAsync(stream);
                     }

                     var serviceImage = new ServiceImage
                     {
                         ServiceId = service.ServiceId,
                         ServiceImageUrl = $"/service/{service.ServiceId}/images/{uniqueFileName}"
                     };
                     _dbContext.ServiceImages.Add(serviceImage);
                 }
             }
        }

        await _dbContext.SaveChangesAsync();
        return Ok(new
        {
            message = "Service added successfully.",
            ServiceId = service.ServiceId
        });
    }
    

    [HttpGet]
    public async Task<IActionResult> GetAllServicesAsync()
    {
        var services = await _dbContext.Services
            .Include(s => s.ServiceImages)
            .Include(u => u.User)
            .ToListAsync();

        var data = services.Select(service => new
        {
            service.ServiceId,
            service.ServiceName,
            service.Description,
            serviceImages = service.ServiceImages.Select(img => img.ServiceImageUrl).ToList(),
            User = service.User == null ? null : new
            {
                service.User.UserId,
                service.User.Fullname,
                service.User.ProfileImage
            },

            service.CreatedAt
        });

        return Ok(new
        {   
            message = "Services retrieved successfully.",
            data
        });
        
    }

    [HttpGet("{serviceId}")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetServiceByIdAsync([FromRoute] string serviceId)
    {
        var service = await _dbContext.Services
            .Include(s => s.User)
                .ThenInclude(a => a.Address)
            .Include(s => s.ServiceImages)
            .FirstOrDefaultAsync(s => s.ServiceId == serviceId);

        if (service == null)
        {
            return NotFound(new { error = "Service not found." });
        }

        return Ok(new
        {
            message = "Service retrieved successfully.",
            data = new
            {
            service.ServiceId,
            service.ServiceName,
            service.Description,
            service.Rating,
            
            User = new
            {
                service.User.UserId,
                service.User.Fullname,
                service.User.ProfileImage,
                service.User.PhoneNumber,
                service.User.Bio,
                address = service.User.Address.Governorate
            },
            serviceImages = service.ServiceImages.Select(img => img.ServiceImageUrl).ToList(),

            service.CreatedAt
            }


        });
    }
}

