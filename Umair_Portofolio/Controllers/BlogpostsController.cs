using Microsoft.AspNetCore.Mvc;
using Umair_Portofolio.Dtos;
using Umair_Portofolio.Services;
using Microsoft.AspNetCore.Authorization;

namespace Umair_Portofolio.Controllers;
//controller van blogposts
// (controller laag)HTTP ontvangen en teruggeven en resultaat kiezen
[ApiController] //markeert de klasse als API-controller en zet validatie aan//
[Route("api/[controller]")]
public class BlogpostsController : ControllerBase
{
    private readonly IBlogpostService _service;

    public BlogpostsController(IBlogpostService service) //DI .net levert service//
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpGet] //met async en await hoeft de server niet te wachten op de database//
    public async Task<ActionResult<IEnumerable<BlogpostDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [AllowAnonymous]  //route is voor iedereen, ook zonder login//
    [HttpGet("{id}")] //reageert op een GET req URL met ID//
    public async Task<ActionResult<BlogpostDto>> GetById(int id)
    {
        var blogpost = await _service.GetByIdAsync(id);
        if (blogpost is null) return NotFound();
        return Ok(blogpost);
    }

    [Authorize(Roles = "Editor,Admin")]
    [HttpPost]
    public async Task<ActionResult<BlogpostDto>> Create(BlogpostCreateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created); 
        //201 terugegeven en vertelt waar de item optehalen is//
    }

    [Authorize(Roles = "Editor,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, BlogpostUpdateDto dto) 
                 // werkt asychroon en geeft HTTP antwoord terug//
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (!updated) return NotFound();
        return NoContent(); //204 geen inhoud  
        //Geeft HTTP code terug 
    }

    [Authorize(Roles = "Editor,Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}