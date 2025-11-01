using System.ComponentModel.DataAnnotations;

namespace GraduationProjectApi.DTOs;

public class RegisterUserDto
{
    [Required(ErrorMessage = "Fullname is required")]
    [StringLength(70, MinimumLength = 3, ErrorMessage = "Fullname must be between 3 and 70 characters")]
    [RegularExpression(@"^[^\p{So}]+$", ErrorMessage = "Fullname cannot contain emojis")]
    public string Fullname { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
    [RegularExpression(@"^\+\d{1,14}$", ErrorMessage = "Phone number must start with '+' and contain only digits")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;

    [MaxLength(6)]
    public string? Gender { get; set; }

    public DateTime? DateOfBirth { get; set; }

    //public string? ProfileImageUrl { get; set; }

    [MaxLength(250)]
    public string? Bio { get; set; }
    public string? Email { get; set; }
}


public class LoginUserDto
{
    [Required(ErrorMessage = "Phone number is required")]
    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
    [RegularExpression(@"^\+\d{1,14}$", ErrorMessage = "Phone number must start with '+' and contain only digits")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}


public class UpdateUserInfoDto
{
    [StringLength(70, MinimumLength = 3, ErrorMessage = "Fullname must be between 3 and 70 characters")]
    [RegularExpression(@"^[^\p{So}]+$", ErrorMessage = "Fullname cannot contain emojis")]
    public string? Fullname { get; set; }

    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
    [RegularExpression(@"^\+\d{1,14}$", ErrorMessage = "Phone number must start with '+' and contain only digits")]
    public string? PhoneNumber { get; set; }

    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
    [RegularExpression(@"^\+\d{1,14}$", ErrorMessage = "Phone number must start with '+' and contain only digits")]
    public string? SecondPhoneNumber { get; set; }

    public string? Gender { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? Bio { get; set; }

    public string? Email { get; set; }
}

/*
public class UpdateProfileImageDto
{
    public IFormFile ProfileImage { get; set; }
}
*/