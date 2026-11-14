using System.ComponentModel.DataAnnotations;
namespace Umair_Portofolio.Dtos;
public class RegisterDto
{[Required, EmailAddress] public string Email { get; set; } = string.Empty;
[Required] public string Password { get; set; } = string.Empty;}