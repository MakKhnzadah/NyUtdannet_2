using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using nyUtdannet2.Data;
using nyUtdannet2.Models;

namespace nyUtdannet2;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));

        builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        var resumeUploadDir = Path.Combine(builder.Environment.WebRootPath, "uploads", "resumes");
        var coverLetterUploadDir = Path.Combine(builder.Environment.WebRootPath, "uploads", "coverletters");
        Directory.CreateDirectory(resumeUploadDir);
        Directory.CreateDirectory(coverLetterUploadDir);

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var dbContext = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                Console.WriteLine("🔄 Running migrations...");
                await dbContext.Database.MigrateAsync();  
                await ApplicationDbInitializer.Initialize(dbContext, userManager, roleManager);

                Console.WriteLine("✅ Database initialized successfully");
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while initializing the database.");
                Console.WriteLine("❌ Error initializing database: " + ex.Message);
            }
        }

        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapRazorPages();

        app.Run();
    }
}
