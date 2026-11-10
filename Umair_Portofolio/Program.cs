using Microsoft.EntityFrameworkCore;
using Umair_Portofolio.Data;
using Umair_Portofolio.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// dependancy injection registreren //
builder.Services.AddDbContext<AppDbContext>(options =>  
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); //(config ef core om sql te gebruiken)
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IBlogpostService, BlogpostService>(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();