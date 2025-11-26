namespace GraduationProjectApi.Models;

public class ServiceRequestImage
{
    public string ServReqImageId { get; set; } = Guid.NewGuid().ToString();

    public required string ServReqImageUrl { get; set; }

    public required string ServReqId { get; set; }
    public ServiceRequest ServiceRequest { get; set; }
}