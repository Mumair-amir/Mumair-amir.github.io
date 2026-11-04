using Microsoft.EntityFrameworkCore;
using Umair_Portofolio.Data;
using Umair_Portofolio.Dtos;
using Umair_Portofolio.Models;
namespace Umair_Portofolio.Services;
public class BlogpostService : IBlogpostService
{
    private readonly AppDbContext _context;
    public BlogpostService(AppDbContext context)
    { _context = context;}
    public async Task<IEnumerable<BlogpostDto>> GetAllAsync()
    {
        var blogposts = await _context.Blogposts.ToListAsync();
        return blogposts.Select(ToDto);
    }
    public async Task<BlogpostDto?> GetByIdAsync(int id)
    {
        var blogpost = await _context.Blogposts.FindAsync(id);
        return blogpost is null ? null : ToDto(blogpost);
    }
    public async Task<BlogpostDto> CreateAsync(BlogpostCreateDto dto)
    {
        var blogpost = new Blogpost
        {
            Titel = dto.Titel,
            Inhoud = dto.Inhoud,
            Publicatiedatum = dto.Publicatiedatum
        };
        _context.Blogposts.Add(blogpost);
        await _context.SaveChangesAsync();
        return ToDto(blogpost);
    }
    public async Task<bool> UpdateAsync(int id, BlogpostUpdateDto dto)
    {
        var blogpost = await _context.Blogposts.FindAsync(id);
        if (blogpost is null) return false;
        blogpost.Titel = dto.Titel;
        blogpost.Inhoud = dto.Inhoud;
        blogpost.Publicatiedatum = dto.Publicatiedatum;
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var blogpost = await _context.Blogposts.FindAsync(id);
        if (blogpost is null) return false;
        _context.Blogposts.Remove(blogpost);
        await _context.SaveChangesAsync(); //schrijf de wijzigingen naar SQLserver// 
        return true;
    }
    private static BlogpostDto ToDto(Blogpost blogpost) => new()
    {
        Id = blogpost.Id,
        Titel = blogpost.Titel,
        Inhoud = blogpost.Inhoud,
        Publicatiedatum = blogpost.Publicatiedatum
    };
}