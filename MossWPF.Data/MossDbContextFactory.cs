using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MossWPF.Data
{
    public class MossDbContextFactory
    {
        private readonly Action<DbContextOptionsBuilder> _configureDbContext;

        public MossDbContextFactory(Action<DbContextOptionsBuilder> configureDbContext)
        {
            _configureDbContext = configureDbContext;
        }

        public MossDbContext CreateDbContext()
        {
            DbContextOptionsBuilder<MossDbContext> options = new DbContextOptionsBuilder<MossDbContext>();

            _configureDbContext(options);
            return new MossDbContext(options.Options);
        }
    }
}
