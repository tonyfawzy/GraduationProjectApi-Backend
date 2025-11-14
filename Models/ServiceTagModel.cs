using System.ComponentModel.DataAnnotations;

namespace GraduationProjectApi.Models;

public class ServiceTag
{
    [Required]
    public string ServiceId { get; set; }
    public Service Service { get; set; }

    [Required]
    public string TagId { get; set; }
    public Tag Tag { get; set; }
}