using Microsoft.AspNetCore.Identity;
using SchoolManagement.BLL;
using SchoolManagement.DAL;
using SchoolManagement.DAL.Data;
using SchoolManagement.Domain.Entities;

var builder = WebApplication.CreateBuilder(args);

// 1. Layers registration
builder.Services.AddDataAccess(builder.Configuration);
builder.Services.AddBusinessLogic();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
