namespace GraduationProjectApi.Models;

public class ServiceImage
{
    public string ServiceImageId { get; set; } = Guid.NewGuid().ToString();

    public required string ServiceImageUrl { get; set; }

    public required string ServiceId { get; set; }
    public Service Service { get; set; }
}