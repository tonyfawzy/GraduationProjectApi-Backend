using System.ComponentModel.DataAnnotations;

namespace GraduationProjectApi.Models;

public class User
{
    [Required]
    public string UserId { get; set; } = Guid.NewGuid().ToString();

    [Required, MaxLength(70)]
    public required string Fullname { get; set; }

    [Required, MaxLength(15)] // E.164 standard for international phone numbers
    public required string PhoneNumber { get; set; }

    [MaxLength(15)]
    public string? SecondPhoneNumber { get; set; }

    [Required]
    public required string PasswordHash { get; set; }
    public int Permission { get; set; } = 0;

    [MaxLength(6)]
    public string? Gender { get; set; }

    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? DateOfBirth { get; set; }

    public string? ProfileImageUrl { get; set; }

    [MaxLength(250)]
    public string? Bio { get; set; }

    [EmailAddress]
    [MaxLength(100)]
    public string? Email { get; set; }

    [MaxLength(100)]
    public string? NormalizedEmail { get; set; }

    public bool IsSuspended { get; set; } = false;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}