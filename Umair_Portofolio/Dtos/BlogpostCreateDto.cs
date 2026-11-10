using System.ComponentModel.DataAnnotations;
namespace Umair_Portofolio.Dtos;
public class BlogpostCreateDto
{
    [Required] public string Titel { get; set; } = string.Empty; //verplicht anders geeft api foutmelding//
    [Required] public string Inhoud { get; set; } = string.Empty;
    public DateTime Publicatiedatum { get; set; }
}
//klassen die alleen velden bevatten die een api mag ontvangen en teruggeven//
// create is de invoer voor POST. (geen Id  db kiest dat)//