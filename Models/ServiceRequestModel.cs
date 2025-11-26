using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace GraduationProjectApi.Models;

public class ServiceRequest
{
    [Required]
    public string ServReqId { get; set; } = Guid.NewGuid().ToString();
    [Required, MaxLength(150)]
    public required string ServReqName { get; set; }
    public string? Description { get; set; }
    public double Budget { get; set; } = 0.0;
    public bool IsPromoted { get; set; } = false;
    public int Status { get; set; } = 1;
    public long CreatedAt { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();

    [Required]
    public string UserId { get; set; }
    public User User { get; set; }

    public ICollection<ServiceRequestTag> ServiceRequestTags { get; set; }
    public ICollection<ServiceRequestImage> ServiceRequestImages { get; set; }
}