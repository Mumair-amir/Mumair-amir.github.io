using Umair_Portofolio.Dtos;
namespace Umair_Portofolio.Services;
public interface IProjectService
//interface zorgt ervoor dat de controller niet afhankelijk is van de klasse (ProjectService) maar van de contract//
{
    Task<IEnumerable<ProjectDto>> GetAllAsync();
    Task<ProjectDto?> GetByIdAsync(int id);
    Task<ProjectDto> CreateAsync(ProjectCreateDto dto);
    Task<bool> UpdateAsync(int id, ProjectUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}