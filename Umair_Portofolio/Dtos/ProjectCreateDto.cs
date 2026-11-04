using System.ComponentModel.DataAnnotations;
namespace Umair_Portofolio.Dtos;
public class ProjectCreateDto
{ [Required] public string Titel{get;set;}=string.Empty;
    [Required] public string Beschrijving{get;set;}=string.Empty;
    [Required] public string Categorie{get;set;}=string.Empty;
    public string GitHubUrl{get;set;}=string.Empty;
    public DateTime Datum{get;set;}
    };
