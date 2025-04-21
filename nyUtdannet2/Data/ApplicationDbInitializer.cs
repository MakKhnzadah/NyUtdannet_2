using Microsoft.AspNetCore.Identity;
using nyUtdannet2.Models;

namespace nyUtdannet2.Data;

public static class ApplicationDbInitializer
{
    public static async Task Initialize(ApplicationDbContext db, UserManager<ApplicationUser> um, RoleManager<IdentityRole> rm)
    {
        Console.WriteLine("🔄 Initializing database...");

        
        string[] roleNames = { "Employee", "Employer" };
        foreach (var roleName in roleNames)
        {
            if (!await rm.RoleExistsAsync(roleName))
            {
                await rm.CreateAsync(new IdentityRole(roleName));
                Console.WriteLine($"✅ Created role: {roleName}");
            }
        }

        
        var users = new List<(ApplicationUser user, string password, string role)>
        {
            (new ApplicationUser
            {
                UserName = "normaluser1@example.com",
                Email = "normaluser1@example.com",
                FirstName = "Jonas",
                LastName = "Olsen",
                PostalCode = "12345",
                StreetName = "Hovedgata",
                StreetNumber = "1A",
                City = "Oslo",
                Country = "Norge",
                DateOfBirth = new DateTime(1990, 1, 1)
            }, "Password123!", "Employee"),

            (new ApplicationUser
            {
                UserName = "normaluser2@example.com",
                Email = "normaluser2@example.com",
                FirstName = "Anna",
                LastName = "Hansen",
                PostalCode = "54321",
                StreetName = "Andregata",
                StreetNumber = "2B",
                City = "Bergen",
                Country = "Norge",
                DateOfBirth = new DateTime(1992, 5, 15)
            }, "Password123!", "Employee"),

            (new EmployerUser
            {
                UserName = "employer1@example.com",
                Email = "employer1@example.com",
                FirstName = "Alice",
                LastName = "Johansen",
                PostalCode = "98765",
                StreetName = "Tredjeavenue",
                StreetNumber = "3C",
                City = "Stavanger",
                Country = "Norge",
                DateOfBirth = new DateTime(1985, 3, 10),
                CompanyName = "TechCorp",
                CompanyDescription = "Innovative teknologiløsninger.",
                CompanyWebsite = "https://techcorp.no"
            }, "Password123!", "Employer"),

            (new EmployerUser
            {
                UserName = "employer2@example.com",
                Email = "employer2@example.com",
                FirstName = "Bob",
                LastName = "Brun",
                PostalCode = "67890",
                StreetName = "Fjerdeblvd",
                StreetNumber = "4D",
                City = "Trondheim",
                Country = "Norge",
                DateOfBirth = new DateTime(1980, 7, 20),
                CompanyName = "ByggCo",
                CompanyDescription = "Bygging og utvikling.",
                CompanyWebsite = "https://byggco.no"
            }, "Password123!", "Employer"),
        };

        foreach (var (user, password, role) in users)
        {
            if (await um.FindByEmailAsync(user.Email) == null)
            {
                var result = await um.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await um.AddToRoleAsync(user, role);
                    Console.WriteLine($"👤 Created user: {user.Email} with role {role}");
                }
            }
        }

        
        var employer1 = await um.FindByEmailAsync("employer1@example.com") as EmployerUser;
        var employer2 = await um.FindByEmailAsync("employer2@example.com") as EmployerUser;

        if (employer1 != null && employer2 != null && !db.JobListings.Any())
        {
            var job1 = new JobListing
            {
                Headline = "Bli med på vårt team som programvareingeniør!",
                Title = "Programvareingeniør",
                Description = "Utvikle banebrytende programvareløsninger for ulike industrier.",
                Requirements = "Sterke programmeringsferdigheter, erfaring med .NET Core og kunnskap om skybaserte systemer.",
                EmployerUserId = employer1.Id,
                EmployerUser = employer1,
                Deadline = DateTime.Now.AddDays(30)
            };

            var job2 = new JobListing
            {
                Headline = "Spennende mulighet som prosjektleder",
                Title = "Prosjektleder",
                Description = "Led team på tvers av funksjoner og sørg for prosjektets suksess.",
                Requirements = "Dokumentert erfaring som prosjektleder, fremragende kommunikasjonsevner.",
                EmployerUserId = employer2.Id,
                EmployerUser = employer2,
                Deadline = DateTime.Now.AddDays(20)
            };

            db.JobListings.AddRange(job1, job2);
            await db.SaveChangesAsync();

            // --- طلبات التوظيف ---
            var user1 = await um.FindByEmailAsync("normaluser1@example.com");
            var user2 = await um.FindByEmailAsync("normaluser2@example.com");

            if (user1 != null && user2 != null && !db.JobApps.Any())
            {
                var app1 = new JobApp
                {
                    Title = "Søknad om programvareingeniørstilling",
                    Summary = "Erfaren programvareutvikler klar til å bidra til innovative prosjekter.",
                    Content = "Jeg har over 5 års erfaring med .NET utvikling og skybaserte systemer.",
                    Status = ApplicationStatus.Pending,
                    UserId = user1.Id,
                    JobListingId = job1.Id
                };

                var app2 = new JobApp
                {
                    Title = "Søknad om prosjektlederstilling",
                    Summary = "Erfaren prosjektleder klar til å lede teamet til suksess.",
                    Content = "Over 8 års erfaring med prosjektledelse på tvers av ulike industrier.",
                    Status = ApplicationStatus.Pending,
                    UserId = user2.Id,
                    JobListingId = job2.Id
                };

                db.JobApps.AddRange(app1, app2);
                await db.SaveChangesAsync();
            }
        }

        Console.WriteLine("✅ Database initialized successfully.");
    }
}
