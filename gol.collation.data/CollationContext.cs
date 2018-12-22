//using Microsoft.EntityFrameworkCore;
using gol.collation.core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gol.collation.data
{
    public class CollationContext : DbContext
    {
        public CollationContext(DbContextOptions<CollationContext> options) : base(options)
        {

        }

        public DbSet<ApiUser> ApiUsers { get; set; }
    }
}
