
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
                new Category { Id = 1, Name = "Adventure", DisplayOrder=1 },
                new Category { Id = 2, Name = "Supersport", DisplayOrder=2 },
                new Category { Id = 3, Name = "News", DisplayOrder=3 },
                new Category { Id = 4, Name = "Classic", DisplayOrder=4 },
                new Category { Id = 5, Name = "Off-Road", DisplayOrder=5 },
                new Category { Id = 6, Name = "Customs", DisplayOrder=6 },
                new Category { Id = 7, Name = "Electric", DisplayOrder=7 },
                new Category { Id = 8, Name = "Commuter", DisplayOrder=8 }

                );
        }
    }
}
