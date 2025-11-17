using System.ComponentModel.DataAnnotations;

namespace GraduationProjectApi.Models;

public class Address
{
    [Required]
    public string AddressId { get; set; } = Guid.NewGuid().ToString();
    [MaxLength(150)]
    public string Governorate { get; set; } = string.Empty;

    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
}