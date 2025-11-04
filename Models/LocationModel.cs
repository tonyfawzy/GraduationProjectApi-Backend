namespace GraduationProjectApi.Models;

public class Location
{
    public string LocationId { get; set; } = Guid.NewGuid().ToString();

    public required string City { get; set; }
}