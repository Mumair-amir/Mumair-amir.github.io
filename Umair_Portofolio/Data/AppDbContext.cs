using Microsoft.EntityFrameworkCore; 
using Umair_Portofolio.Models;
//de EF core communiceert met de database
namespace Umair_Portofolio.Data;
//praat met SQL Server via EF Core
// datatoegang hoort niet bij de HTTP laag 
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Blogpost> Blogposts => Set<Blogpost>();
}