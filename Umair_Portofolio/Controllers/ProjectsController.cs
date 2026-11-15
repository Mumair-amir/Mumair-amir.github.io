using Microsoft.AspNetCore.Mvc;
using Umair_Portofolio.Dtos;
using Umair_Portofolio.Services;
using Microsoft.AspNetCore.Authorization;

namespace Umair_Portofolio.Controllers;
//controller van projecten
// (controller laag)HTTP ontvangen en teruggeven en resultaat kiezen
[ApiController] //markeert de klasse als API-controller en zet validatie aan//
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _service;

    public ProjectsController(IProjectService service) //DI .net levert service//
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpGet] //met async en await hoeft de server niet te wachten op de database//
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [AllowAnonymous] //route is voor iedereen, ook zonder login  
    [HttpGet("{id}")] //reageert op een GET req URL met ID//
    public async Task<ActionResult<ProjectDto>> GetById(int id)
    {
        var project = await _service.GetByIdAsync(id);
        if (project is null) return NotFound(); //404 niet gevonden
        return Ok(project);
    }

    [Authorize(Roles = "Editor,Admin")]
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(ProjectCreateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Editor,Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ProjectUpdateDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        if (!updated) return NotFound();
        return NoContent();
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