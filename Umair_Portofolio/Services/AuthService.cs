using Microsoft.AspNetCore.Identity;
using Umair_Portofolio.Dtos;

namespace Umair_Portofolio.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IdentityResult> RegisterAsync(RegisterDto dto)
    {
        var user = new IdentityUser { UserName = dto.Email, Email = dto.Email };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded) return result;

        return await _userManager.AddToRoleAsync(user, "Gast");
        // Iedere nieuwe gebruiker krijgt altijd de rol Gast
        
    }
     public async Task<bool> LoginAsync(LoginDto dto)
    {
        var result = await _signInManager.PasswordSignInAsync(
            dto.Email, dto.Password, isPersistent: false, lockoutOnFailure: false);

        return result.Succeeded;
    }
    public async Task LogoutAsync()
{
    await _signInManager.SignOutAsync();
}
}