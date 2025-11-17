using System.ComponentModel.DataAnnotations;
namespace GraduationProjectApi.DTOs.Auth;

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

