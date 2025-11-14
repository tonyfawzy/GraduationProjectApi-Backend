using System.ComponentModel.DataAnnotations;

namespace GraduationProjectApi.DTOs;

public class AddTagDTO
{
    [Required(ErrorMessage = "Tag name is required")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Tag must be between 2 and 30 characters")]
    [RegularExpression(@"^([a-zA-Z\u0080-\u024F]+(?:. |-| |'))*[a-zA-Z\u0080-\u024F]*$", ErrorMessage = "\"^([a-zA-Z\u0080-\u024F]+(?:. |-| |'))*[a-zA-Z\u0080-\u024F]*$\"")]
    public string TagName { get; set; }
}
