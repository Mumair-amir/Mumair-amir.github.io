using System.ComponentModel.DataAnnotations;
namespace Umair_Portofolio.Dtos;
public class BlogpostUpdateDto
{
    [Required] public string Titel { get; set; } = string.Empty;
    [Required] public string Inhoud { get; set; } = string.Empty;
    public DateTime Publicatiedatum { get; set; }
}
//uitvoer PUT //