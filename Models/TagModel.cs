using System.ComponentModel.DataAnnotations;

namespace GraduationProjectApi.Models;

public class Tag
{
    [Required]
    public string TagId { get; set; } = Guid.NewGuid().ToString();

    [Required, MaxLength(100)]
    public required string TagName { get; set; }
    [Required]
    public long CreatedAt { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();

    public ICollection<ServiceTag> ServiceTags { get; set; } = new List<ServiceTag>();
}