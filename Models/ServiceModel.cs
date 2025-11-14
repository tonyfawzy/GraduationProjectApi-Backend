
using System.ComponentModel.DataAnnotations;

namespace GraduationProjectApi.Models;

public class Service
{
    [Required]
    public string ServiceId { get; set; } = Guid.NewGuid().ToString();
    [Required, MaxLength(150)]
    public required string ServiceName { get; set; }
    public string? Description { get; set; }
    public bool IsPromoted { get; set; } = false;
    public int Status { get; set; } = 1;
    public long CreatedAt { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();

    [Required]
    public string UserId { get; set; }
    public User User { get; set; }

    public ICollection<ServiceTag> ServiceTags { get; set; } = new List<ServiceTag>();
    public ICollection<ServiceImage> ServiceImages { get; set; }
}