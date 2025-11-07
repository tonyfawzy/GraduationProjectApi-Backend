using System.ComponentModel.DataAnnotations;

namespace GraduationProjectApi.DTOs;

public class LocationDTO
{
    [Required(ErrorMessage = "City is required")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "City must be between 2 and 30 characters")]
    [RegularExpression(@"^([a-zA-Z\u0080-\u024F]+(?:. |-| |'))*[a-zA-Z\u0080-\u024F]*$", ErrorMessage = "\"^([a-zA-Z\u0080-\u024F]+(?:. |-| |'))*[a-zA-Z\u0080-\u024F]*$\"")]
    public string City { get; set; }
}
