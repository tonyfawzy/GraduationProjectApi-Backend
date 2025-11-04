
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
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string UserId { get; set; }
    public User User { get; set; }

    [Required]
    public string TradeId { get; set; }
    public Trade Trade { get; set; }

    [Required]
    public string LocationId { get; set; }
    public Location Location { get; set; }

    public ICollection<ServiceImage> ServiceImages { get; set; }
}