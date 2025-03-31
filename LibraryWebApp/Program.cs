using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using LibraryWebApp.Data;
using LibraryWebApp.Models;
using Microsoft.AspNetCore.Builder;
using System;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<LibraryWebAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryWebAppContext") ?? throw new InvalidOperationException("Connection string 'LibraryWebAppContext' not found.")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<LibraryWebAppContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;


await InitializeAsync(services);

app.Run();

async Task InitializeAsync(IServiceProvider services)
{
    try
    {
        SeedData.Initialize(services);
        await RoleInitializer.CreateRoles(services);
    }
    catch (Exception ex)
    {
        // Consider logging the exception
        throw; // or handle the exception as needed
    }
}

class RoleInitializer
{
    public static async Task CreateRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        
        var adminUser = new IdentityUser
        {
            UserName = "admin@example.com",
            Email = "admin@example.com"
        };

        string adminUserPassword = "SecurePassword123!";
        var user = await userManager.FindByEmailAsync(adminUser.Email);
        if (user == null)
        {
            
            var createAdminUser = await userManager.CreateAsync(adminUser, adminUserPassword);
            if (createAdminUser.Succeeded)
            {
                
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}

