using System.ComponentModel.DataAnnotations;

namespace GraduationProjectApi.Models;

public class ServiceRequestTag
{
    [Required]
    public string ServReqId { get; set; }
    public ServiceRequest ServiceRequest { get; set; }

    [Required]
    public string TagId { get; set; }
    public Tag Tag { get; set; }
}