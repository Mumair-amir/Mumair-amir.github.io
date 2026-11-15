using Microsoft.EntityFrameworkCore;
using Umair_Portofolio.Data;
using Umair_Portofolio.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// dependancy injection registreren //
builder.Services.AddDbContext<AppDbContext>(options =>  
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); //(config ef core om sql te gebruiken)
    builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();//regis Identity//
builder.Services.ConfigureApplicationCookie(options =>  //past de instellingen aan van de cookie die AddIdentity heeft aangemaakt.
{options.Events.OnRedirectToLogin = context =>
{ context.Response.StatusCode = StatusCodes.Status401Unauthorized;
return Task.CompletedTask; };
options.Events.OnRedirectToAccessDenied=context=>
{context.Response.StatusCode=StatusCodes.Status403Forbidden;
return Task.CompletedTask;};
});
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IBlogpostService, BlogpostService>(); // DI instellen//
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build(); //bouwt de app op basis van de configuratie in builder  // rollseeder aanroepen

await RoleSeeder.SeedAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();   //leest de cookie
app.UseAuthorization();    //controleert de rol
app.MapControllers();

app.Run();
public partial class Program { } //patial voegt lege uitbreiding toe aan klasse//