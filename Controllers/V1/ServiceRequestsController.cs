using GraduationProjectApi.DTOs;
using GraduationProjectApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        if (servReqDto.ServiceRequestImages != null && servReqDto.ServiceRequestImages.Count > 5)
            return BadRequest(new { error = "Maximum 5 images allowed." });


         var servReq = new ServiceRequest
        {
            ServReqName = servReqDto.Title,
            Description = servReqDto.Description,
            UserId = user.UserId,
        };

        _dbContext.ServiceRequests.Add(servReq);
        await _dbContext.SaveChangesAsync();


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

        await _dbContext.SaveChangesAsync();
        return Ok(new
        {
            message = "Job added successfully.",
            ServReqId = servReq.ServReqId
        });


    }



}
