using System.Net;
using System.Net.Http.Json;
using Umair_Portofolio.Dtos;

namespace Umair_Portofolio.Tests;

[Collection("Api")]
public class BlogpostsApiTests
{
    private readonly ApiFactory _factory;

    public BlogpostsApiTests(ApiFactory factory)
    {
        _factory = factory;
    }

    //  hulpmethoden 

    private static BlogpostCreateDto NieuweBlogpost() => new()
    {
        Titel = "Testblogpost",
        Inhoud = "Dit is de inhoud van de testblogpost.",
        Publicatiedatum = new DateTime(2026, 10, 1)
    };

    private static BlogpostUpdateDto Wijziging() => new()
    {
        Titel = "Gewijzigde blogpost",
        Inhoud = "De inhoud is aangepast.",
        Publicatiedatum = new DateTime(2026, 10, 2)
    };

    private static async Task<BlogpostDto> MaakBlogpostAanAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/blogposts", NieuweBlogpost());
        response.EnsureSuccessStatusCode();
        var blogpost = await response.Content.ReadFromJsonAsync<BlogpostDto>();
        return blogpost!;
    }

    //  GET 

    [Fact]
    public async Task GetBlogposts_ZonderLogin_GeeftOk()
    {
        // Arrange
        var client = await _factory.CreateClientAsync(null);

        // Act
        var response = await client.GetAsync("/api/blogposts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetBlogposts_NaAanmaken_BevatBlogpost()
    {
        // Arrange
        var editor = await _factory.CreateClientAsync("Editor");
        var anoniem = await _factory.CreateClientAsync(null);
        var aangemaakt = await MaakBlogpostAanAsync(editor);

        // Act
        var response = await anoniem.GetAsync("/api/blogposts");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var lijst = await response.Content.ReadFromJsonAsync<List<BlogpostDto>>();
        Assert.NotNull(lijst);
        Assert.Contains(lijst, b => b.Id == aangemaakt.Id);
    }

    [Fact]
    public async Task GetBlogpostById_BestaandeBlogpost_GeeftOkEnJuisteGegevens()
    {
        // Arrange
        var editor = await _factory.CreateClientAsync("Editor");
        var anoniem = await _factory.CreateClientAsync(null);
        var verwacht = NieuweBlogpost();
        var aangemaakt = await MaakBlogpostAanAsync(editor);

        // Act
        var response = await anoniem.GetAsync($"/api/blogposts/{aangemaakt.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var blogpost = await response.Content.ReadFromJsonAsync<BlogpostDto>();
        Assert.NotNull(blogpost);
        Assert.Equal(aangemaakt.Id, blogpost.Id);
        Assert.Equal(verwacht.Titel, blogpost.Titel);
        Assert.Equal(verwacht.Inhoud, blogpost.Inhoud);
        Assert.Equal(verwacht.Publicatiedatum, blogpost.Publicatiedatum);
    }

    [Fact]
    public async Task GetBlogpostById_OnbekendId_GeeftNotFound()
    {
        // Arrange
        var client = await _factory.CreateClientAsync(null);

        // Act
        var response = await client.GetAsync("/api/blogposts/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    //  POST

    [Fact]
    public async Task PostBlogpost_ZonderLogin_GeeftUnauthorized()
    {
        // Arrange
        var client = await _factory.CreateClientAsync(null);

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", NieuweBlogpost());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostBlogpost_AlsGast_GeeftForbidden()
    {
        // Arrange
        var client = await _factory.CreateClientAsync("Gast");

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", NieuweBlogpost());

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task PostBlogpost_AlsEditorOfAdmin_MaaktBlogpostAan(string rol)
    {
        // Arrange
        var client = await _factory.CreateClientAsync(rol); //maakt gebruiker
        var nieuw = NieuweBlogpost();

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", nieuw);

        // Assert: statuscode en inhoud van de response
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var aangemaakt = await response.Content.ReadFromJsonAsync<BlogpostDto>();
        Assert.NotNull(aangemaakt);
        Assert.True(aangemaakt.Id > 0);
        Assert.Equal(nieuw.Titel, aangemaakt.Titel);

        // Assert: effect, de nieuwe blogpost is ook op te halen
        var opgehaald = await client.GetFromJsonAsync<BlogpostDto>($"/api/blogposts/{aangemaakt.Id}");
        Assert.NotNull(opgehaald);
        Assert.Equal(nieuw.Titel, opgehaald.Titel);
    }

    [Fact]
    public async Task PostBlogpost_ZonderTitel_GeeftBadRequest()
    {
        // Arrange
        var client = await _factory.CreateClientAsync("Editor");
        var ongeldig = NieuweBlogpost();
        ongeldig.Titel = "";

        // Act
        var response = await client.PostAsJsonAsync("/api/blogposts", ongeldig);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    //  PUT 

    [Theory]
    [InlineData(null, HttpStatusCode.Unauthorized)]
    [InlineData("Gast", HttpStatusCode.Forbidden)]
    public async Task PutBlogpost_ZonderRechten_WordtGeweigerd(string? rol, HttpStatusCode verwacht)
    {
        // Arrange
        var editor = await _factory.CreateClientAsync("Editor");
        var aangemaakt = await MaakBlogpostAanAsync(editor);
        var client = await _factory.CreateClientAsync(rol);

        // Act
        var response = await client.PutAsJsonAsync($"/api/blogposts/{aangemaakt.Id}", Wijziging());

        // Assert: geweigerd, en de blogpost is niet gewijzigd
        Assert.Equal(verwacht, response.StatusCode);
        var opgehaald = await editor.GetFromJsonAsync<BlogpostDto>($"/api/blogposts/{aangemaakt.Id}");
        Assert.NotNull(opgehaald);
        Assert.Equal(NieuweBlogpost().Titel, opgehaald.Titel);
    }

    [Fact]
    public async Task PutBlogpost_OnbekendId_GeeftNotFound()
    {
        // Arrange
        var client = await _factory.CreateClientAsync("Editor");

        // Act
        var response = await client.PutAsJsonAsync("/api/blogposts/999999", Wijziging());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task PutBlogpost_AlsEditorOfAdmin_WijzigtBlogpost(string rol)
    {
        // Arrange
        var client = await _factory.CreateClientAsync(rol);
        var aangemaakt = await MaakBlogpostAanAsync(client);
        var wijziging = Wijziging();

        // Act
        var response = await client.PutAsJsonAsync($"/api/blogposts/{aangemaakt.Id}", wijziging);

        // Assert: statuscode en effect, alle velden zijn gewijzigd
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var opgehaald = await client.GetFromJsonAsync<BlogpostDto>($"/api/blogposts/{aangemaakt.Id}");
        Assert.NotNull(opgehaald);
        Assert.Equal(wijziging.Titel, opgehaald.Titel);
        Assert.Equal(wijziging.Inhoud, opgehaald.Inhoud);
        Assert.Equal(wijziging.Publicatiedatum, opgehaald.Publicatiedatum);
    }

    //  DELETE 

    [Theory]
    [InlineData(null, HttpStatusCode.Unauthorized)]
    [InlineData("Gast", HttpStatusCode.Forbidden)]
    public async Task DeleteBlogpost_ZonderRechten_WordtGeweigerd(string? rol, HttpStatusCode verwacht)
    {
        // Arrange
        var editor = await _factory.CreateClientAsync("Editor");
        var aangemaakt = await MaakBlogpostAanAsync(editor);
        var client = await _factory.CreateClientAsync(rol);

        // Act
        var response = await client.DeleteAsync($"/api/blogposts/{aangemaakt.Id}");

        // Assert: geweigerd, en de blogpost bestaat nog
        Assert.Equal(verwacht, response.StatusCode);
        var nogAanwezig = await editor.GetAsync($"/api/blogposts/{aangemaakt.Id}");
        Assert.Equal(HttpStatusCode.OK, nogAanwezig.StatusCode);
    }

    [Fact]
    public async Task DeleteBlogpost_OnbekendId_GeeftNotFound()
    {
        // Arrange
        var client = await _factory.CreateClientAsync("Editor");

        // Act
        var response = await client.DeleteAsync("/api/blogposts/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task DeleteBlogpost_AlsEditorOfAdmin_VerwijdertBlogpost(string rol)
    {
        // Arrange
        var client = await _factory.CreateClientAsync(rol);
        var aangemaakt = await MaakBlogpostAanAsync(client);

        // Act
        var response = await client.DeleteAsync($"/api/blogposts/{aangemaakt.Id}");

        // Assert: statuscode en effect, de blogpost bestaat niet meer
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var daarna = await client.GetAsync($"/api/blogposts/{aangemaakt.Id}");
        Assert.Equal(HttpStatusCode.NotFound, daarna.StatusCode);
    }
}