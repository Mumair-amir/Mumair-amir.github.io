using Microsoft.AspNetCore.Mvc;
using Umair_Portofolio.Dtos;
using Umair_Portofolio.Services;

namespace Umair_Portofolio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogpostsController : ControllerBase
{
    private readonly IBlogpostService _service;

    public BlogpostsController(IBlogpostService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BlogpostDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BlogpostDto>> GetById(int id)
    {
        var blogpost = await _service.GetByIdAsync(id);
        if (blogpost is null) return NotFound();
        return Ok(blogpost);
    }

    [HttpPost]
    public async Task<ActionResult<BlogpostDto>> Create(BlogpostCreateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, BlogpostUpdateDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (!updated) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}