using System.ComponentModel.DataAnnotations;

namespace GraduationProjectApi.DTOs;

public class RegisterUser
{
    [Required(ErrorMessage = "Fullname is required")]
    [StringLength(70, MinimumLength = 3, ErrorMessage = "Fullname must be between 3 and 70 characters")]
    [RegularExpression(@"^[^\p{So}]+$", ErrorMessage = "Fullname cannot contain emojis")]
    public string Fullname { get; set; }
}