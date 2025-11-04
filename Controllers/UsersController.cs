using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GraduationProjectApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using GraduationProjectApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using YamlDotNet.Core.Tokens;
using System.Net.Http.Headers;

namespace GraduationProjectApi.Controllers;

[ApiController]
[Route("user")]
public class UsersController(JwtOptions jwtOptions, ApplicationDbContext dbContext) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto RegDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var isPhoneNumberExists = await _dbContext.Users.AnyAsync(u =>
            u.PhoneNumber == RegDto.PhoneNumber || u.SecondPhoneNumber == RegDto.PhoneNumber);

        if (isPhoneNumberExists)
            return BadRequest(new
            {
                code = 400,
                error = "Phone number already in use."
            });

        var user = new User
        {
            Fullname = RegDto.Fullname,
            PhoneNumber = RegDto.PhoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(RegDto.Password),
            Gender = string.IsNullOrWhiteSpace(RegDto.Gender) ? null : RegDto.Gender,
            DateOfBirth = RegDto.DateOfBirth,
            Bio = string.IsNullOrWhiteSpace(RegDto.Bio) ? null : RegDto.Bio,
            Email = string.IsNullOrWhiteSpace(RegDto.Email) ? null : RegDto.Email,
            NormalizedEmail = string.IsNullOrWhiteSpace(RegDto.Email) ? null : RegDto.Email.ToUpperInvariant(),
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            code = 200,
            msg = "User registered successfully."
        });
    }



    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto LogDto)
    {

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == LogDto.PhoneNumber);
        if (user == null || !BCrypt.Net.BCrypt.Verify(LogDto.Password, user.PasswordHash))
        {
            return Unauthorized(new { error = "Invalid phone number or password." });
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtOptions.Key)), SecurityAlgorithms.HmacSha256Signature),
            Subject = new ClaimsIdentity(new Claim[]
            {
                new (ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new (ClaimTypes.Name, user.Fullname),
                new (ClaimTypes.Role, user.Permission == 0 ? "User" : "Admin")
            })
        };
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(securityToken);

        return Ok(new
        {
            code = 200,
            msg = "Login successful",
            userId = user.UserId,
            token = accessToken
        });
    }

    [HttpGet("getAllUsers")]
    public async Task<IActionResult> GetUsersAsync()
    {
        var users = await _dbContext.Users
            .Include(u => u.Services)
                .ThenInclude(s => s.Location)
            .Include(u => u.Services)
                .ThenInclude(s => s.Trade)
            .ToListAsync();

        var result = users.Select(user => new
        {
            user.UserId,
            user.Fullname,
            user.PhoneNumber,
            user.Email,
            user.Bio,
            user.ProfileImageUrl,
            Services = user.Services.Select(service => new
            {
                service.ServiceId,
                service.ServiceName,
                service.Description,
                TradeName = service.Trade.TradeName,
                LocationName = service.Location.City 
            })
        });

        return Ok(result);
    }

    [HttpGet("getInfo")]
    [Authorize]
    public async Task<IActionResult> GetMyInfoAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized(new { error = "Invalid token." });
        }

        var userId = userIdClaim.Value;

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null)
        {
            return NotFound(new
            {
                code = 404,
                error = "User not found."
            });
        }

        var result = new
        {
            userId = user.UserId,
            fullName = user.Fullname,
            phoneNumber = user.PhoneNumber,
            secondPhoneNumber = user.SecondPhoneNumber,
            permission = (user.Permission == 0) ? "User" : "Admin",
            gender = user.Gender,
            dateOfBirth = user.DateOfBirth,
            profileImage = user.ProfileImageUrl,
            bio = user.Bio,
            email = user.Email,
            isSuspended = user.IsSuspended,
            createdAt = user.CreatedAt
        };

        return Ok(new
        {
            code = 200,
            msg = "Success",
            data = result 
        });
    }

    [HttpPatch("update-profile")]
    [Authorize]
    public async Task<IActionResult> UpdateProfile([FromForm] UpdateUserInfoDto updateDto, [FromForm] IFormFile? ProfileImage)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized(new { error = "Invalid token." });
        }

        var userId = userIdClaim.Value;

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null)
        {
            return NotFound(new { error = "User not found." });
        }

        if (!string.IsNullOrWhiteSpace(updateDto.Fullname))
        {
            if (updateDto.Fullname.Length < 3 || updateDto.Fullname.Length > 70)
            {
                return BadRequest(new { error = "Fullname must be between 3 and 70 characters." });
            }
            user.Fullname = updateDto.Fullname;
        }

        if (!string.IsNullOrWhiteSpace(updateDto.Bio))
        {
            if (updateDto.Bio.Length > 250)
            {
                return BadRequest(new { error = "Bio cannot exceed 250 characters." });
            }
            user.Bio = updateDto.Bio;
        }

        if (!string.IsNullOrWhiteSpace(updateDto.SecondPhoneNumber))
        {
            if (updateDto.SecondPhoneNumber.Length > 15)
            {
                return BadRequest(new { error = "Second phone number cannot exceed 15 characters." });
            }
            user.SecondPhoneNumber = updateDto.SecondPhoneNumber;
        }

        if (ProfileImage != null)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "pfps");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = $"{Guid.NewGuid()}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await ProfileImage.CopyToAsync(stream);
            }

            user.ProfileImageUrl = $"/uploads/pfps/{fileName}";
        }

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Profile updated successfully.",
            Fullname = user.Fullname,
            Bio = user.Bio,
            SecondPhoneNumber = user.SecondPhoneNumber,
            ProfileImageUrl = user.ProfileImageUrl
        });
    }

    [HttpDelete("delete-my-account")]
    [Authorize]
    public async Task<IActionResult> DeleteUserAsync()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized(new { error = "Invalid token." });
        }

        var userId = userIdClaim.Value;

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null)
        {
            return NotFound(new
            {
                code = 404,
                msg = "User not found." 
            });
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            code = 200,
            msg = "Your account has been deleted successfully."
        });
    }

}