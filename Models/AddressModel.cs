using System.ComponentModel.DataAnnotations;

namespace GraduationProjectApi.Models;

public class Address
{
    [Required]
    public string AddressId { get; set; } = Guid.NewGuid().ToString();
    [Required, MaxLength(150)]
    public string Governorate { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
}