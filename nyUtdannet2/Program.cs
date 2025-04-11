using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using nyUtdannet2.Data;
using nyUtdannet2.Models;

namespace nyUtdannet2;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // --- Configuration: Database ---
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        // --- Configuration: Identity & Roles ---
        builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false; // Email confirmation disabled for testing
            // Optional: Password settings for development
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        // --- MVC and Razor Pages ---
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        var app = builder.Build();

        // --- Database Initialization (Roles, Users) ---
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var dbContext = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            ApplicationDbInitializer.Initialize(dbContext, userManager, roleManager);
        }

        // --- Middleware Pipeline ---
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts(); // Enables HTTP Strict Transport Security
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication(); // Needed if login is in use
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        
        app.MapRazorPages();

        app.Run();
    }
}
