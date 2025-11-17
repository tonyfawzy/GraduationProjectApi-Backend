using GraduationProjectApi.DTOs;
using GraduationProjectApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationProjectApi.Controllers.V1;
[ApiController]
[Route("tags")]
public class TagsController(ApplicationDbContext dbContext) : ControllerBase
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    [HttpPost]
    //[Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddTag([FromBody] AddTagDTO tagDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var isTagExists = await _dbContext.Tags.AnyAsync(t => t.TagName == tagDto.TagName);
        if (isTagExists)
        {
            return BadRequest(new
            {
                error = "The tag already exists."
            });
        }

        var tag = new Tag
        {
            TagName = tagDto.TagName
        };

        _dbContext.Tags.Add(tag);
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Tag added successfully.",
            tagId = tag.TagId
        });
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllTags()
    {
        
        var tags = await _dbContext.Tags
            .Select(t => new
            {
                t.TagId,
                t.TagName
            })
            .ToListAsync();

        return Ok(new
        {
            message = "Tags retrieved successfully.",
            data = tags
        });
    }


    [HttpPut("{tagId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateTag([FromRoute] string tagId, [FromBody] AddTagDTO tagDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.TagId == tagId);
        if (tag == null)
        {
            return NotFound(new
            {
                error = "Tag not found."
            });
        }

        var isTagExists = await _dbContext.Tags.AnyAsync(t => t.TagName == tagDto.TagName && t.TagId != tagId);
        if (isTagExists)
        {
            return BadRequest(new
            {
                error = "Another tag with the same name already exists."
            });
        }

        tag.TagName = tagDto.TagName;
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Tag updated successfully."
        });
    }

    [HttpDelete("{tagId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteTag([FromRoute] string tagId)
    {
        var tag = await _dbContext.Tags.FirstOrDefaultAsync(t => t.TagId == tagId);
        if (tag == null)
        {
            return NotFound(new
            {
                error = "Tag not found."
            });
        }

        _dbContext.Tags.Remove(tag);
        await _dbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "Tag deleted successfully."
        });
    }

}