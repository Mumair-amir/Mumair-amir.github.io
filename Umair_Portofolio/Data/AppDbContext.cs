using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Umair_Portofolio.Models;

namespace Umair_Portofolio.Data;

public class AppDbContext : IdentityDbContext<IdentityUser> // is een dbcontext van micro met de usertabellen ingebouwd, aspuser,roles etc
{                                             //standaard klasse voor gebruiker met email en w8woord
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Blogpost> Blogposts => Set<Blogpost>();
}