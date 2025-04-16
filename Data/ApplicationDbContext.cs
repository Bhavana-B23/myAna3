using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using DataVizNavigator1.Models;

namespace DataVizNavigator1.Data
{
  


    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Mapping> Mappings { get; set; }
        public DbSet<Chart> Charts { get; set; }
    }
}
