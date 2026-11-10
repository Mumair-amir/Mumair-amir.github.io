# Portfolio Backend API

Backend voor mijn portfoliosite (WPFW opdracht 3). De API biedt CRUD voor projecten en blogposts.

## Technische gegevens
- .NET-versie: .NET 10 (net10.0)
- API-stijl: controllers (ASP.NET Core Web API) met lagen
- Database: SQL Server Express (`localhost\SQLEXPRESS`), toegang via EF Core
- Swagger UI: Swashbuckle.AspNetCore

## Codestructuur
Het API-project staat in de map `Umair_Portofolio/Umair_Portofolio/`.

| Map | Inhoud |
|---|---|
| `Controllers/` | HTTP-afhandeling (`ProjectsController`, `BlogpostsController`) |
| `Services/` | applicatielogica en mapping entiteit ↔ DTO |
| `Data/` | `AppDbContext` (EF Core) |
| `Models/` | entiteiten `Project` en `Blogpost` |
| `Dtos/` | invoer-DTO's (Create, Update) en uitvoer-DTO (Dto) |
| `Migrations/` | EF Core-migraties |

## Endpoints
Per type (`/api/projects` en `/api/blogposts`):
GET alle, GET op id, POST, PUT op id, DELETE op id.

## SQL Server configureren
1. Installeer SQL Server Express en zorg dat de service `MSSQL$SQLEXPRESS` draait.
2. De connectionstring staat in `appsettings.json` onder `ConnectionStrings:DefaultConnection`:
   `Server=localhost\SQLEXPRESS;Database=PortfolioDb;Trusted_Connection=True;TrustServerCertificate=True`
3. Gebruik je een andere SQL Server, pas dan alleen deze string aan.

## Database aanmaken
Aanpak: **EF Core-migraties** (geen EnsureCreated).

```powershell
dotnet tool install --global dotnet-ef
cd Umair_Portofolio/Umair_Portofolio
dotnet ef database update
```

De migratie `InitialCreate` maakt de database `PortfolioDb` met de tabellen `Projects` en `Blogposts`.

## Applicatie starten
```powershell
cd Umair_Portofolio/Umair_Portofolio
dotnet run
```
Open daarna Swagger UI op `http://localhost:5220/swagger` (de poort staat in de terminal en in `Properties/launchSettings.json`).