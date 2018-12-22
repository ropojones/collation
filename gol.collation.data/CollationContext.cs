//using Microsoft.EntityFrameworkCore;
using gol.collation.core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gol.collation.data
{
    public class CollationContext : IdentityDbContext<ApiUser>
    {
        public CollationContext(DbContextOptions<CollationContext> options) : base(options)
        {

        }

        public DbSet<ApiUser> ApiUsers { get; set; }
    }

    public class CollationContextFactory : IDesignTimeDbContextFactory<CollationContext>
    {
        CollationContext IDesignTimeDbContextFactory<CollationContext>.CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CollationContext>();
            optionsBuilder.UseSqlServer("Server=LAPTOP-HU4OB30A\\MSSQL14;Database=CollationDB;User Id=sa;Password=password1; MultipleActiveResultSets = true");

            return new CollationContext(optionsBuilder.Options);
        }
    }
}
