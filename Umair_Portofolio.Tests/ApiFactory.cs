using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Umair_Portofolio.Data;

namespace Umair_Portofolio.Tests;
public class ApiFactory : WebApplicationFactory<Program>
{
    private const string TestConnectionString =
        "Server=localhost\\SQLEXPRESS;Database=PortfolioDb_Test;Trusted_Connection=True;TrustServerCertificate=True";
    public ApiFactory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>() // testdatabase maken voor de API start//
            .UseSqlServer(TestConnectionString)
            .Options;
        using var context = new AppDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.Migrate();
    }
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {      
      builder.ConfigureAppConfiguration((_, config) => //API gebruikt tijdens de tests de testdatabase
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = TestConnectionString
            });
        });
    }
        // rol = null geeft een anonieme client; anders een ingelogde gebruiker met die rol
    public async Task<HttpClient> CreateClientAsync(string? rol)
    {
        var client = CreateClient();
        if (rol is null) return client;

        var email = $"{Guid.NewGuid()}@test.nl";
        var password = $"Aa1!{Guid.NewGuid():N}";   // willekeurig, dus geen vast wachtwoord in de repository

        using (var scope = Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = new IdentityUser { UserName = email, Email = email };

            var created = await userManager.CreateAsync(user, password);
            Assert.True(created.Succeeded);

            var roleResult = await userManager.AddToRoleAsync(user, rol);
            Assert.True(roleResult.Succeeded);
        }

        var login = await client.PostAsJsonAsync("/api/auth/login", new { email, password });
        login.EnsureSuccessStatusCode();

        return client;
    }
}