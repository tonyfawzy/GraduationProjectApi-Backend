using GraduationProjectApi.DTOs;
using GraduationProjectApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.ColorSpaces.Companding;

namespace GraduationProjectApi.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/jobs")]
public class ServiceRequestsController(ApplicationDbContext dbContext) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateServiceRequestAsync([FromForm] CreateServReqDto servReqDto)
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

        if (servReqDto.Budget < 0)
            return BadRequest(new { error = "Budget cannot be negative." });
/*
        if (servReqDto.ServiceRequestImages != null && servReqDto.ServiceRequestImages.Count > 5)
            return BadRequest(new { error = "Maximum 5 images allowed." });
*/

         var servReq = new ServiceRequest
        {
            UserId = user.UserId,
            ServReqName = servReqDto.Title,
            Description = servReqDto.Description,
            Budget = servReqDto.Budget,
        };

        _dbContext.ServiceRequests.Add(servReq);
        await _dbContext.SaveChangesAsync();

/*
        if (servReqDto.ServiceRequestImages != null && servReqDto.ServiceRequestImages.Any())
        {
             var folderPath = Path.Combine("wwwroot", "job", servReq.ServReqId, "images");   
             if (!Directory.Exists(folderPath))
             {
                 Directory.CreateDirectory(folderPath);
             }

             foreach (var image in servReqDto.ServiceRequestImages)
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

                     var serviceRequestImage = new ServiceRequestImage
                     {
                         ServReqId = servReq.ServReqId,
                         ServReqImageUrl = $"/service/{servReq.ServReqId}/images/{uniqueFileName}"
                     };
                     _dbContext.ServiceRequestImages.Add(serviceRequestImage);
                 }
             }
        }
*/

        return Ok(new
        {
            message = "Job added successfully.",
            ServReqId = servReq.ServReqId
        });

    }



    [HttpGet("{servReqId}")]
    public async Task<IActionResult> GetServiceRequestByIdAsync([FromRoute] string servReqId)
    {
        var serviceRequest = await _dbContext.ServiceRequests
            .Include(sr => sr.User)
            .FirstOrDefaultAsync(sr => sr.ServReqId == servReqId);

        if (serviceRequest == null)
            return NotFound(new { error = "Service request not found." });

        return Ok(new
        {
            message = "Job retrieved successfully.",
            data = new
            {
                serviceRequest.ServReqId,
                serviceRequest.ServReqName,
                serviceRequest.Description,
                serviceRequest.Budget,
                User = new
                {
                    serviceRequest.User.UserId,
                    serviceRequest.User.Fullname,
                    serviceRequest.User.ProfileImage,
                    serviceRequest.User.PhoneNumber,
                    serviceRequest.User.SecondPhoneNumber,
                    serviceRequest.User.Bio,
                    address = serviceRequest.User.Address != null ? serviceRequest.User.Address.Governorate : null
                },
                serviceRequest.CreatedAt
            }
        });
    }


    [HttpGet]
    public async Task<IActionResult> GetAllServiceRequestsAsync()
    {
        var serviceRequests = await _dbContext.ServiceRequests
            .Include(sr => sr.User)
            .ToListAsync();

        var data = serviceRequests.Select(sr => new
        {
            sr.ServReqId,
            sr.ServReqName,
            sr.Description,
            sr.Budget,
            User = new
            {
                sr.User.UserId,
                sr.User.Fullname,
                sr.User.ProfileImage,
                address = sr.User.Address != null ? sr.User.Address.Governorate : null
            },
            sr.CreatedAt
        });

        return Ok(new
        {   
            message = "Jobs retrieved successfully.",
            data
        });
    }


    [HttpDelete("{servReqId}")]
    [Authorize]
    public async Task<IActionResult> DeleteServiceRequestAsync([FromRoute] string servReqId)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized(new { error = "Invalid token: missing user identifier claim." });

        var userId = userIdClaim.Value;

        var servReq = await _dbContext.ServiceRequests.FirstOrDefaultAsync(sr => sr.ServReqId == servReqId);
        if (servReq == null)
            return NotFound(new { error = "Service request not found." });

        if (servReq.UserId != userId)
            return Forbid();

        _dbContext.ServiceRequests.Remove(servReq);
        await _dbContext.SaveChangesAsync();

        return Ok(new { message = "Service request deleted successfully." });
    }


}
