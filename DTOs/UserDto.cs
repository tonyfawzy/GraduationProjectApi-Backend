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
    [RegularExpression(@"^[a-zA-Z0-9]{6,23}$", ErrorMessage = "Password must be 6-23 characters long and contain only letters and numbers")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required")]
    [MaxLength(6)]
    [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be 'Male', or 'Female'.")]
    public string Gender { get; set; }

    [Required(ErrorMessage = "Date of birth is required")]
    public DateTime DateOfBirth { get; set; }

    //public string? ProfileImageUrl { get; set; }

    public string? Email { get; set; }
}


public class LoginUserDto
{
    [Required(ErrorMessage = "Phone number is required")]
    [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
    [RegularExpression(@"^\+\d{1,14}$", ErrorMessage = "Phone number must start with '+' and contain only digits")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [RegularExpression(@"^[a-zA-Z0-9]{6,23}$", ErrorMessage = "Password must be 6-23 characters long and contain only letters and numbers")]
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

    [MaxLength(250)]
    public string? Bio { get; set; }

    public string? Email { get; set; }
}

/*
public class UpdateProfileImageDto
{
    public IFormFile ProfileImage { get; set; }
}
*/