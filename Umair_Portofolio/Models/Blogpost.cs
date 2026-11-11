namespace Umair_Portofolio.Models; //entiteit voor db en ef core//
//weergave van databasetabel in code//
public class Blogpost
{
     public int Id { get; set; }
    public string Titel { get; set; } = string.Empty;
    public string Inhoud { get; set; } = string.Empty;
    public DateTime Publicatiedatum { get; set; }
}