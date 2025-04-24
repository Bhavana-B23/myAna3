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
        // Add these lines to your ApplicationDbContext.cs file
        public DbSet<ChartPage> ChartPages { get; set; }
        public DbSet<PageChartMapping> PageChartMappings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the many-to-many relationship
            modelBuilder.Entity<PageChartMapping>()
                .HasKey(pcm => pcm.Id);

            modelBuilder.Entity<PageChartMapping>()
                .HasOne(pcm => pcm.ChartPage)
                .WithMany(cp => cp.PageChartMappings)
                .HasForeignKey(pcm => pcm.PageId);

            modelBuilder.Entity<PageChartMapping>()
                .HasOne(pcm => pcm.Chart)
                .WithMany()
                .HasForeignKey(pcm => pcm.ChartId);
        }
    }
}
