
using Microsoft.EntityFrameworkCore;
using WebMagazines.Models;

namespace WebMagazines.DataAccess.Data
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
        public DbSet<Product> Products { get; set; }

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
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Adventure Bike Rider",
                    Author = "Moto Explorer",
                    Description = "The ultimate guide to adventure motorcycles, covering long-distance touring, off-road riding techniques, essential gear, and unforgettable routes around the world.",
                    ISBN = "ADV1000000001",
                    ListPrice = 39.99,
                    Price = 34.99,
                    Price50 = 31.99,
                    Price100 = 28.99,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "Supersport Bike",
                    Author = "James Rider",
                    Description = "Discover the world of high-performance supersport motorcycles, from track riding and cornering techniques to choosing the perfect sport bike.",
                    ISBN = "SPR1000000002",
                    ListPrice = 44.99,
                    Price = 39.99,
                    Price50 = 36.99,
                    Price100 = 33.99,
                    CategoryId = 2
                },
                new Product
                {
                    Id = 3,
                    Name = "MCN",
                    Author = "Motorcycle News",
                    Description = "Weekly motorcycle magazine featuring the latest bike reviews, industry news, riding tips, product tests, and upcoming motorcycle events.",
                    ISBN = "MCN1000000003",
                    ListPrice = 7.99,
                    Price = 6.99,
                    Price50 = 5.99,
                    Price100 = 4.99,
                    CategoryId = 3
                },
                new Product
                {
                    Id = 4,
                    Name = "Classic Bike",
                    Author = "David Cooper",
                    Description = "A celebration of iconic classic motorcycles, exploring their history, restoration projects, famous manufacturers, and timeless engineering.",
                    ISBN = "CLS1000000004",
                    ListPrice = 34.99,
                    Price = 30.99,
                    Price50 = 27.99,
                    Price100 = 24.99,
                    CategoryId = 4
                },
                new Product
                {
                    Id = 5,
                    Name = "Off-Road Explorer",
                    Author = "Chris Walker",
                    Description = "Learn everything about trail riding, motocross, and enduro adventures, including bike setup, riding skills, and essential off-road equipment.",
                    ISBN = "OFF1000000005",
                    ListPrice = 37.99,
                    Price = 33.99,
                    Price50 = 29.99,
                    Price100 = 26.99,
                    CategoryId = 5
                },
                new Product
                {
                    Id = 6,
                    Name = "Customs Bike",
                    Author = "Rick Johnson",
                    Description = "An inspiring look at custom motorcycle culture, featuring café racers, bobbers, choppers, custom builds, fabrication techniques, and unique designs.",
                    ISBN = "CUS1000000006",
                    ListPrice = 42.99,
                    Price = 38.99,
                    Price50 = 34.99,
                    Price100 = 30.99,
                    CategoryId = 6
                },
                new Product
                {
                    Id = 7,
                    Name = "Electric Bike",
                    Author = "Sarah Green",
                    Description = "Explore the future of motorcycling with electric motorcycles, battery technology, charging infrastructure, and the latest electric bike reviews.",
                    ISBN = "ELE1000000007",
                    ListPrice = 36.99,
                    Price = 32.99,
                    Price50 = 28.99,
                    Price100 = 25.99,
                    CategoryId = 7
                },
                new Product
                {
                    Id = 8,
                    Name = "Commuter Bike",
                    Author = "Urban Rider",
                    Description = "Practical advice for everyday riders, covering fuel-efficient motorcycles, commuting tips, maintenance, riding in traffic, and essential accessories.",
                    ISBN = "COM1000000008",
                    ListPrice = 29.99,
                    Price = 25.99,
                    Price50 = 22.99,
                    Price100 = 19.99,
                    CategoryId = 8
                }
            );
        }
    }
}
