using Microsoft.EntityFrameworkCore;
using Umair_Portofolio.Data;
using Umair_Portofolio.Dtos;
using Umair_Portofolio.Models;
namespace Umair_Portofolio.Services;
public class ProjectService : IProjectService
{
    private readonly AppDbContext _context;
    public ProjectService(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<ProjectDto>> GetAllAsync()
    {
        var projects = await _context.Projects.ToListAsync();
        return projects.Select(ToDto);
    }
    public async Task<ProjectDto?> GetByIdAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        return project is null ? null : ToDto(project);
    }
    public async Task<ProjectDto> CreateAsync(ProjectCreateDto dto)
    {
        var project = new Project
        {
            Titel = dto.Titel,
            Beschrijving = dto.Beschrijving,
            Categorie = dto.Categorie,
            GitHubUrl = dto.GitHubUrl,
            Datum = dto.Datum
        };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync(); //schrijft de wijzigingen naar SQL Server
        return ToDto(project);
    }
    public async Task<bool> UpdateAsync(int id, ProjectUpdateDto dto)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project is null) return false;
        project.Titel = dto.Titel;
        project.Beschrijving = dto.Beschrijving;
        project.Categorie = dto.Categorie;
        project.GitHubUrl = dto.GitHubUrl;
        project.Datum = dto.Datum;
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project is null) return false;
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }
    private static ProjectDto ToDto(Project project) => new()
    {
        Id = project.Id,
        Titel = project.Titel,
        Beschrijving = project.Beschrijving,
        Categorie = project.Categorie,
        GitHubUrl = project.GitHubUrl,
        Datum = project.Datum
    };
}