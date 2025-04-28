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
        public DbSet<ChartPage> ChartPages { get; set; }
        public DbSet<PageChartMapping> PageChartMappings { get; set; }
        // Add the User DbSet
        public DbSet<User> Users { get; set; }

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

            // Add seed data for Users
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Bhavana",
                    LastName = "Burra",
                    Username = "Bhavana",
                    Password = "Myana@23"
                },
                new User
                {
                    Id = 2,
                    FirstName = "Ishika",
                    LastName = "Varma",
                    Username = "Ishika",
                    Password = "DataViz@23"
                }
            );
        }
    }
}