using Umair_Portofolio.Dtos;
namespace Umair_Portofolio.Services;
public interface IBlogpostService
{
    Task<IEnumerable<BlogpostDto>> GetAllAsync();
    Task<BlogpostDto?> GetByIdAsync(int id);
    Task<BlogpostDto> CreateAsync(BlogpostCreateDto dto);
    Task<bool> UpdateAsync(int id, BlogpostUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}