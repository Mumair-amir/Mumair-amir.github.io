using System.Net;
using System.Net.Http.Json;
using Umair_Portofolio.Dtos;

namespace Umair_Portofolio.Tests;

[Collection("Api")]
public class ProjectsApiTests
{
    private readonly ApiFactory _factory;

    public ProjectsApiTests(ApiFactory factory)
    {
        _factory = factory;
    }

    //  hulpmethoden 

    private static ProjectCreateDto NieuwProject() => new()
    {
        Titel = "Testproject",
        Beschrijving = "Beschrijving van het testproject",
        Categorie = "Software",
        GitHubUrl = "https://github.com/test/test",
        Datum = new DateTime(2026, 10, 1)
    };

    private static ProjectUpdateDto Wijziging() => new()
    {
        Titel = "Gewijzigde titel",
        Beschrijving = "Gewijzigde beschrijving",
        Categorie = "Hardware",
        GitHubUrl = "https://github.com/test/gewijzigd",
        Datum = new DateTime(2026, 11, 1)
    };

    // Maakt een project aan met de gegeven client en geeft het aangemaakte project terug
    private static async Task<ProjectDto> MaakProjectAanAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/projects", NieuwProject());
        response.EnsureSuccessStatusCode();
        var project = await response.Content.ReadFromJsonAsync<ProjectDto>();
        return project!;
    }

    // GET 

    [Fact]
    public async Task GetProjects_ZonderLogin_GeeftOk()
    {
        // Arrange
        var client = await _factory.CreateClientAsync(null);

        // Act
        var response = await client.GetAsync("/api/projects");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProjects_NaAanmaken_BevatProject()
    {
        // Arrange: een Editor maakt een project aan, een anonieme gebruiker leest de lijst
        var editor = await _factory.CreateClientAsync("Editor");
        var anoniem = await _factory.CreateClientAsync(null);
        var aangemaakt = await MaakProjectAanAsync(editor);

        // Act
        var response = await anoniem.GetAsync("/api/projects");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var lijst = await response.Content.ReadFromJsonAsync<List<ProjectDto>>();
        Assert.NotNull(lijst);
        Assert.Contains(lijst, p => p.Id == aangemaakt.Id);
    }

    [Fact]
    public async Task GetProjectById_BestaandProject_GeeftOkEnJuisteGegevens()
    {
        // Arrange
        var editor = await _factory.CreateClientAsync("Editor");
        var anoniem = await _factory.CreateClientAsync(null);
        var verwacht = NieuwProject();
        var aangemaakt = await MaakProjectAanAsync(editor);

        // Act
        var response = await anoniem.GetAsync($"/api/projects/{aangemaakt.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var project = await response.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.NotNull(project);
        Assert.Equal(aangemaakt.Id, project.Id);
        Assert.Equal(verwacht.Titel, project.Titel);
        Assert.Equal(verwacht.Beschrijving, project.Beschrijving);
        Assert.Equal(verwacht.Categorie, project.Categorie);
        Assert.Equal(verwacht.GitHubUrl, project.GitHubUrl);
        Assert.Equal(verwacht.Datum, project.Datum);
    }

    [Fact]
    public async Task GetProjectById_OnbekendId_GeeftNotFound()
    {
        // Arrange
        var client = await _factory.CreateClientAsync(null);

        // Act
        var response = await client.GetAsync("/api/projects/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    //  POST

    [Fact]
    public async Task PostProject_ZonderLogin_GeeftUnauthorized()
    {
        // Arrange
        var client = await _factory.CreateClientAsync(null);

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", NieuwProject());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostProject_AlsGast_GeeftForbidden()
    {
        // Arrange
        var client = await _factory.CreateClientAsync("Gast");

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", NieuwProject());

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task PostProject_AlsEditorOfAdmin_MaaktProjectAan(string rol)
    {
        // Arrange
        var client = await _factory.CreateClientAsync(rol); //maakt gebruiker
        var nieuw = NieuwProject();

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", nieuw);

        // Assert: statuscode en inhoud van de response
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var aangemaakt = await response.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.NotNull(aangemaakt);
        Assert.True(aangemaakt.Id > 0);
        Assert.Equal(nieuw.Titel, aangemaakt.Titel);

        // Assert: effect, het nieuwe project is ook op te halen
        var opgehaald = await client.GetFromJsonAsync<ProjectDto>($"/api/projects/{aangemaakt.Id}");
        Assert.NotNull(opgehaald);
        Assert.Equal(nieuw.Titel, opgehaald.Titel);
    }

    [Fact]
    public async Task PostProject_ZonderTitel_GeeftBadRequest()
    {
        // Arrange
        var client = await _factory.CreateClientAsync("Editor");
        var ongeldig = NieuwProject();
        ongeldig.Titel = "";

        // Act
        var response = await client.PostAsJsonAsync("/api/projects", ongeldig);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ---------- PUT ----------

    [Theory]
    [InlineData(null, HttpStatusCode.Unauthorized)]
    [InlineData("Gast", HttpStatusCode.Forbidden)]
    public async Task PutProject_ZonderRechten_WordtGeweigerd(string? rol, HttpStatusCode verwacht)
    {
        // Arrange: een Editor maakt een project aan, daarna probeert iemand zonder rechten het te wijzigen
        var editor = await _factory.CreateClientAsync("Editor");
        var aangemaakt = await MaakProjectAanAsync(editor);
        var client = await _factory.CreateClientAsync(rol);

        // Act
        var response = await client.PutAsJsonAsync($"/api/projects/{aangemaakt.Id}", Wijziging());

        // Assert: geweigerd, en het project is niet gewijzigd
        Assert.Equal(verwacht, response.StatusCode);
        var opgehaald = await editor.GetFromJsonAsync<ProjectDto>($"/api/projects/{aangemaakt.Id}");
        Assert.NotNull(opgehaald);
        Assert.Equal(NieuwProject().Titel, opgehaald.Titel);
    }

    [Fact]
    public async Task PutProject_OnbekendId_GeeftNotFound()
    {
        // Arrange
        var client = await _factory.CreateClientAsync("Editor");

        // Act
        var response = await client.PutAsJsonAsync("/api/projects/999999", Wijziging());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task PutProject_AlsEditorOfAdmin_WijzigtProject(string rol)
    {
        // Arrange
        var client = await _factory.CreateClientAsync(rol);
        var aangemaakt = await MaakProjectAanAsync(client);//doet een POST en geeft het aangemaakte project terug
        var wijziging = Wijziging();

        // Act
        var response = await client.PutAsJsonAsync($"/api/projects/{aangemaakt.Id}", wijziging);

        // Assert: statuscode en effect, alle velden zijn gewijzigd
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var opgehaald = await client.GetFromJsonAsync<ProjectDto>($"/api/projects/{aangemaakt.Id}");
        Assert.NotNull(opgehaald);
        Assert.Equal(wijziging.Titel, opgehaald.Titel);
        Assert.Equal(wijziging.Beschrijving, opgehaald.Beschrijving);
        Assert.Equal(wijziging.Categorie, opgehaald.Categorie);
        Assert.Equal(wijziging.GitHubUrl, opgehaald.GitHubUrl);
        Assert.Equal(wijziging.Datum, opgehaald.Datum);
    }

    //  DELETE 

    [Theory]
    [InlineData(null, HttpStatusCode.Unauthorized)]
    [InlineData("Gast", HttpStatusCode.Forbidden)]
    public async Task DeleteProject_ZonderRechten_WordtGeweigerd(string? rol, HttpStatusCode verwacht)
    {
        // Arrange: een Editor maakt een project aan, daarna probeert iemand zonder rechten het te verwijderen
        var editor = await _factory.CreateClientAsync("Editor");
        var aangemaakt = await MaakProjectAanAsync(editor);
        var client = await _factory.CreateClientAsync(rol);

        // Act
        var response = await client.DeleteAsync($"/api/projects/{aangemaakt.Id}");

        // Assert: geweigerd, en het project bestaat nog
        Assert.Equal(verwacht, response.StatusCode);
        var nogAanwezig = await editor.GetAsync($"/api/projects/{aangemaakt.Id}");
        Assert.Equal(HttpStatusCode.OK, nogAanwezig.StatusCode);
    }

    [Fact]
    public async Task DeleteProject_OnbekendId_GeeftNotFound()
    {
        // Arrange
        var client = await _factory.CreateClientAsync("Editor");

        // Act
        var response = await client.DeleteAsync("/api/projects/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("Editor")]
    [InlineData("Admin")]
    public async Task DeleteProject_AlsEditorOfAdmin_VerwijdertProject(string rol)
    {
        // Arrange
        var client = await _factory.CreateClientAsync(rol);
        var aangemaakt = await MaakProjectAanAsync(client); //doet een POST en geeft het aangemaakte project terug

        // Act
        var response = await client.DeleteAsync($"/api/projects/{aangemaakt.Id}");

        // Assert: statuscode en effect, het project bestaat niet meer
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var daarna = await client.GetAsync($"/api/projects/{aangemaakt.Id}");
        Assert.Equal(HttpStatusCode.NotFound, daarna.StatusCode);
    }
}