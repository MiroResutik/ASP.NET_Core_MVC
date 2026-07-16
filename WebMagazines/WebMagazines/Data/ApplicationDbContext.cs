using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using WebMagazines.Models;

namespace WebMagazines.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Define DbSet properties for your entities
        // Accepting dbContextOptions to configure the database connection and other options
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define a DbSet for the Category entity
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            // Configure the Category entity
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Adventure" },
                new Category { Id = 2, Name = "Supersport" },
                new Category { Id = 3, Name = "News" },
                new Category { Id = 4, Name = "Classic" },
                new Category { Id = 5, Name = "Off-Road" },
                new Category { Id = 6, Name = "Customs" },
                new Category { Id = 7, Name = "Electric" },
                new Category { Id = 8, Name = "Commuter" }

                );
        }
    }
}
