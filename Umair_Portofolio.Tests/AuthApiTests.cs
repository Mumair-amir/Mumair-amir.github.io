using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Umair_Portofolio.Dtos;

namespace Umair_Portofolio.Tests;

[Collection("Api")]
public class AuthApiTests
{
    private readonly ApiFactory _factory;

    public AuthApiTests(ApiFactory factory)
    {
        _factory = factory;
    }

    // ---------- hulpmethoden ----------

    // Willekeurige gegevens, dus geen vast wachtwoord in de repository
    private static RegisterDto NieuweGebruiker() => new()
    {
        Email = $"{Guid.NewGuid()}@test.nl",
        Password = $"Aa1!{Guid.NewGuid():N}"
    };

    // Haalt de rollen van een gebruiker rechtstreeks uit Identity
    private async Task<IList<string>> HaalRollenOpAsync(string email)
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var user = await userManager.FindByEmailAsync(email);
        Assert.NotNull(user);
        return await userManager.GetRolesAsync(user);
    }

    // ---------- registratie ----------

    [Fact]
    public async Task Register_MetGeldigeGegevens_GeeftOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        var gebruiker = NieuweGebruiker();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", gebruiker);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_ZonderEmail_GeeftBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var gebruiker = NieuweGebruiker();
        gebruiker.Email = "";

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", gebruiker);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_BestaandEmailadres_GeeftBadRequest()
    {
        // Arrange: de gebruiker is al geregistreerd
        var client = _factory.CreateClient();
        var gebruiker = NieuweGebruiker();
        var eerste = await client.PostAsJsonAsync("/api/auth/register", gebruiker);
        Assert.Equal(HttpStatusCode.OK, eerste.StatusCode);

        // Act
        var tweede = await client.PostAsJsonAsync("/api/auth/register", gebruiker);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, tweede.StatusCode);
    }

    [Fact]
    public async Task Register_ZwakWachtwoord_GeeftBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        var gebruiker = NieuweGebruiker();
        gebruiker.Password = "abc";

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", gebruiker);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_NieuweGebruiker_KrijgtRolGast()
    {
        // Arrange
        var client = _factory.CreateClient();
        var gebruiker = NieuweGebruiker();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", gebruiker);

        // Assert: de gebruiker is aangemaakt en heeft precies één rol, Gast
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var rollen = await HaalRollenOpAsync(gebruiker.Email);
        Assert.Equal("Gast", Assert.Single(rollen));
    }

    [Fact]
    public async Task Register_MetExtraRolVeld_KentGeenEditorOfAdminToe()
    {
        // Arrange: een client probeert zichzelf Admin te maken door een extra veld mee te sturen
        var client = _factory.CreateClient();
        var gebruiker = NieuweGebruiker();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", new
        {
            email = gebruiker.Email,
            password = gebruiker.Password,
            rol = "Admin"
        });

        // Assert: registratie lukt, maar de gebruiker is alleen Gast
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var rollen = await HaalRollenOpAsync(gebruiker.Email);
        Assert.Contains("Gast", rollen);
        Assert.DoesNotContain("Editor", rollen);
        Assert.DoesNotContain("Admin", rollen);
    }

    // ---------- login ----------

    [Fact]
    public async Task Login_MetGeldigeGegevens_GeeftOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        var gebruiker = NieuweGebruiker();
        var registratie = await client.PostAsJsonAsync("/api/auth/register", gebruiker);
        Assert.Equal(HttpStatusCode.OK, registratie.StatusCode);

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginDto
        {
            Email = gebruiker.Email,
            Password = gebruiker.Password
        });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_MetVerkeerdWachtwoord_GeeftUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var gebruiker = NieuweGebruiker();
        var registratie = await client.PostAsJsonAsync("/api/auth/register", gebruiker);
        Assert.Equal(HttpStatusCode.OK, registratie.StatusCode);

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginDto
        {
            Email = gebruiker.Email,
            Password = $"Bb2!{Guid.NewGuid():N}"
        });

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_ZonderEmail_GeeftBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginDto
        {
            Email = "",
            Password = $"Aa1!{Guid.NewGuid():N}"
        });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task NaLogin_KanGebruikerBeveiligdeRouteGebruiken()
    {
        // Arrange: registreren, rol Editor toekennen en controleren dat de route voor de login geweigerd wordt
        var client = _factory.CreateClient();
        var gebruiker = NieuweGebruiker();
        var registratie = await client.PostAsJsonAsync("/api/auth/register", gebruiker);
        Assert.Equal(HttpStatusCode.OK, registratie.StatusCode);

        using (var scope = _factory.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = await userManager.FindByEmailAsync(gebruiker.Email);
            var rolResultaat = await userManager.AddToRoleAsync(user!, "Editor");
            Assert.True(rolResultaat.Succeeded);
        }

        var project = new ProjectCreateDto
        {
            Titel = "Project na login",
            Beschrijving = "Beschrijving",
            Categorie = "Software",
            GitHubUrl = "https://github.com/test/test",
            Datum = new DateTime(2026, 10, 1)
        };

        var voorLogin = await client.PostAsJsonAsync("/api/projects", project);
        Assert.Equal(HttpStatusCode.Unauthorized, voorLogin.StatusCode);

        // Act: inloggen met dezelfde client (de cookie wordt bewaard) en opnieuw proberen
        var login = await client.PostAsJsonAsync("/api/auth/login", new LoginDto
        {
            Email = gebruiker.Email,
            Password = gebruiker.Password
        });
        var naLogin = await client.PostAsJsonAsync("/api/projects", project);

        // Assert
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        Assert.Equal(HttpStatusCode.Created, naLogin.StatusCode);
    }
}