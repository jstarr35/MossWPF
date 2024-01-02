using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MossWPF.Data
{
    public class DesignTimeDbContext : IDesignTimeDbContextFactory<MossDbContext>
    {
        public MossDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<MossDbContext>();
            builder.UseSqlite("Data Source=moss.db");
            return new MossDbContext(builder.Options);
        }
    }
}
