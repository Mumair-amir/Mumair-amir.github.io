using Microsoft.AspNetCore.Identity;
using Umair_Portofolio.Dtos;
namespace Umair_Portofolio.Services;
public interface IAuthService
{   Task<IdentityResult> RegisterAsync(RegisterDto dto);
    Task<bool> LoginAsync(LoginDto dto);
}