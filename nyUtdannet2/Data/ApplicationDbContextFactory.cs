// File: Data/ApplicationDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using nyUtdannet2.Data;

namespace nyUtdannet2.Data
{
    public class ApplicationDbContextFactory
        : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // هنا نستخدم نفس سلسلة الاتصال التي في appsettings.json
            optionsBuilder.UseSqlite("Data Source=nyUtdannet2.db");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}