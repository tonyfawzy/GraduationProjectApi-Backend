using System.ComponentModel.DataAnnotations;

namespace GraduationProjectApi.DTOs;


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