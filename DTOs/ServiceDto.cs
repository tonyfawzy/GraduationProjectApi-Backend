using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GraduationProjectApi.DTOs;

public class CreateServiceRequestDto
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    //public List<string>? TagIds { get; set; }
    public List<IFormFile>? ServiceImages { get; set; }
}