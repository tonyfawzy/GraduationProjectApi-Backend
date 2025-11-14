
using System.ComponentModel.DataAnnotations;

namespace GraduationProjectApi.DTOs;

public class AddServiceDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public List<string>? Tags { get; set; }
}