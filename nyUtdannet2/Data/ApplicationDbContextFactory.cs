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
            
            optionsBuilder.UseSqlite("Data Source=nyUtdannet2.db");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}