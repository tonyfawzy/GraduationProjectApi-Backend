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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;


namespace GraduationProjectApi.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController(JwtOptions jwtOptions, ApplicationDbContext dbContext) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterUserDto RegDto)
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
                error = "Phone number already in use."
            });

        var user = new User
        {
            Fullname = RegDto.Fullname,
            PhoneNumber = RegDto.PhoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(RegDto.Password),
            Gender = string.IsNullOrWhiteSpace(RegDto.Gender) ? null : RegDto.Gender,
            DateOfBirth = RegDto.DateOfBirth,
            Email = string.IsNullOrWhiteSpace(RegDto.Email) ? null : RegDto.Email,
            NormalizedEmail = string.IsNullOrWhiteSpace(RegDto.Email) ? null : RegDto.Email.ToUpperInvariant(),
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "User registered successfully."
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginUserDto LogDto)
    {
        try {

        
        var user = await _dbContext.Users
        .Include(a => a.Address)
        .FirstOrDefaultAsync(u => u.PhoneNumber == LogDto.PhoneNumber);

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

        var userDto = new
        {
            userId = user.UserId,
            fullname = user.Fullname,
            phoneNumber = user.PhoneNumber,
            secondPhoneNumber = user.SecondPhoneNumber,
            email = user.Email,
            role = (user.Permission == 0) ? "User" : "Admin",
            gender = user.Gender,
            dateOfBirth = user.DateOfBirth,
            address = user.Address.Governorate,
            profileImage = user.ProfileImage,
            bio = user.Bio,
            balance = user.Balance,
            isSuspended = user.IsSuspended,
            //services = user.Services

            
            createdAt = user.CreatedAt,
        };

        return Ok(new
        {
            message = "Login successful",
            user = userDto,
            token = accessToken
        });
    } catch  (DbUpdateException ex) {
        return StatusCode(500, new { error = "An error occurred while accessing the database.", details = ex.Message });
    } catch (Exception ex) {
        return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
    }
    }
}

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UsersController(JwtOptions jwtOptions, ApplicationDbContext dbContext) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    [HttpGet]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsersAsync()
    {
        var users = await _dbContext.Users
            .Include(u => u.Services)
                .ThenInclude(s => s.ServiceTags)
                    .ThenInclude(st => st.Tag)
            .Include(a => a.Address)
            .ToListAsync();

        var result = users.Select(user => new
        {
            user.UserId,
            user.Fullname,
            user.PhoneNumber,
            user.Email,
            user.DateOfBirth,
            user.Bio,
            user.ProfileImage,
            user.Address.Governorate,
            user.Balance,
            Services = user.Services.Select(service => new
            {
                service.ServiceId,
                service.ServiceName,
                service.Description,
                ServiceTags = (service.ServiceTags ?? Enumerable.Empty<ServiceTag>())
                    .Select(st => new
                    {
                        st.TagId,
                        tagName = st.Tag != null ? st.Tag.TagName : null
                    })
            })
        });

        return Ok(result);
    }

    [HttpGet("{userId}")]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserByIdAsync([FromRoute] string userId)
    {
        var user = await _dbContext.Users
            .Include(u => u.Services)
                .ThenInclude(s => s.ServiceTags)
                    .ThenInclude(st => st.Tag)
            .Include(a => a.Address)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user == null)
        {
            return NotFound(new { error = "User not found." });
        }

        var result = new
        {
            user.UserId,
            user.Fullname,
            user.PhoneNumber,
            user.Email,
            user.DateOfBirth,
            user.Bio,
            user.ProfileImage,
            address = user.Address.Governorate,
            Services = user.Services.Select(service => new
            {
                service.ServiceId,
                service.ServiceName,
                service.Description,
                ServiceTags = (service.ServiceTags ?? Enumerable.Empty<ServiceTag>())
                    .Select(st => new
                    {
                        st.TagId,
                        tagName = st.Tag != null ? st.Tag.TagName : null
                    })
            })
        };

        return Ok(
            new
            {
                message = "Success",
                data = result
            });
    }

/*
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
            profileImage = user.ProfileImage,
            bio = user.Bio,
            email = user.Email,
            isSuspended = user.IsSuspended,
            createdAt = user.CreatedAt
        };

        return Ok(new
        {
            message = "Success",
            data = result
        });
    }
*/
    [HttpPatch]
    [Authorize]
    public async Task<IActionResult> UpdateProfileAsync([FromForm] UpdateUserInfoDto updateDto, [FromForm] IFormFile? ProfileImage)
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
            /*
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
            */

/*
        using (var ms = new MemoryStream())
        {
            await ProfileImage.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            string base64String = Convert.ToBase64String(fileBytes);

            user.ProfileImageUrl = "data:image/jpeg;base64," + base64String;
        }
*/

            using var image = await SixLabors.ImageSharp.Image.LoadAsync(ProfileImage.OpenReadStream());

            image.Mutate(x => x.Resize(new SixLabors.ImageSharp.Processing.ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(128, 128),
                Mode = SixLabors.ImageSharp.Processing.ResizeMode.Max,
            }));

            image.Mutate(x => x.Pad(128, 128, SixLabors.ImageSharp.Color.White));

            using var ms = new MemoryStream();
            await image.SaveAsJpegAsync(ms);

            string base64String = Convert.ToBase64String(ms.ToArray());
            user.ProfileImage = "data:image/jpeg;base64," + base64String;

        }

        _dbContext.Users.Update(user);
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Profile updated successfully.",
            Fullname = user.Fullname,
            Bio = user.Bio,
            SecondPhoneNumber = user.SecondPhoneNumber,
            ProfileImage = user.ProfileImage
        });
    }

    [HttpDelete]
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
                message = "User not found."
            });
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Your account has been deleted successfully."
        });
    }

}
