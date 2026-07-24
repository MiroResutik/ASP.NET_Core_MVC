using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebMagazines.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50" },
                values: new object[] { "Moto Explorer", "The ultimate guide to adventure motorcycles, covering long-distance touring, off-road riding techniques, essential gear, and unforgettable routes around the world.", "ADV1000000001", 39.990000000000002, 34.990000000000002, 28.989999999999998, 31.989999999999998 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50" },
                values: new object[] { "James Rider", "Discover the world of high-performance supersport motorcycles, from track riding and cornering techniques to choosing the perfect sport bike.", "SPR1000000002", 44.990000000000002, 39.990000000000002, 33.990000000000002, 36.990000000000002 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50" },
                values: new object[] { "Motorcycle News", "Weekly motorcycle magazine featuring the latest bike reviews, industry news, riding tips, product tests, and upcoming motorcycle events.", "MCN1000000003", 7.9900000000000002, 6.9900000000000002, 4.9900000000000002, 5.9900000000000002 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50" },
                values: new object[] { "David Cooper", "A celebration of iconic classic motorcycles, exploring their history, restoration projects, famous manufacturers, and timeless engineering.", "CLS1000000004", 34.990000000000002, 30.989999999999998, 24.989999999999998, 27.989999999999998 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50" },
                values: new object[] { "Chris Walker", "Learn everything about trail riding, motocross, and enduro adventures, including bike setup, riding skills, and essential off-road equipment.", "OFF1000000005", 37.990000000000002, 33.990000000000002, 26.989999999999998, 29.989999999999998 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50" },
                values: new object[] { "Rick Johnson", "An inspiring look at custom motorcycle culture, featuring café racers, bobbers, choppers, custom builds, fabrication techniques, and unique designs.", "CUS1000000006", 42.990000000000002, 38.990000000000002, 30.989999999999998, 34.990000000000002 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50" },
                values: new object[] { "Sarah Green", "Explore the future of motorcycling with electric motorcycles, battery technology, charging infrastructure, and the latest electric bike reviews.", "ELE1000000007", 36.990000000000002, 32.990000000000002, 25.989999999999998, 28.989999999999998 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50" },
                values: new object[] { "Urban Rider", "Practical advice for everyday riders, covering fuel-efficient motorcycles, commuting tips, maintenance, riding in traffic, and essential accessories.", "COM1000000008", 29.989999999999998, 25.989999999999998, 19.989999999999998, 22.989999999999998 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50" },
                values: new object[] { "Alba Solver", "A thrilling adventure deep in the heart of an uncharted jungle, where ancient secrets and hidden treasures await those brave enough to seek them.", "JNG7777770001", 45.0, 40.0, 30.0, 35.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50" },
                values: new object[] { "Bob Roarman", "An insightful exploration of wealth, ambition, and the relentless passage of time. A story that challenges what it truly means to be rich.", "MNT8888880001", 55.0, 50.0, 40.0, 45.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50" },
                values: new object[] { "Jessica Alban", "A mysterious tale set around a remote lake where strange occurrences lead a young detective to uncover a decades-old secret buried beneath the water.", "LKE9999990001", 35.0, 30.0, 25.0, 28.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50" },
                values: new object[] { "Kristen Lober", "A captivating journey through the cosmos, blending science and imagination as humanity reaches beyond the stars for the very first time.", "SPC1010100001", 65.0, 60.0, 50.0, 55.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50" },
                values: new object[] { "Laura Goldberg", "At the edge of an ancient forest, dawn reveals more than just light. A poetic novel about renewal, loss, and the quiet strength found in nature.", "FRD1111110001", 30.0, 27.0, 20.0, 24.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50" },
                values: new object[] { "Elara Vance", "Deep within a forgotten grove, the trees hold memories of those who came before. A hauntingly beautiful story of legacy and belonging.", "WGR1212120001", 42.0, 38.0, 30.0, 34.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50" },
                values: new object[] { "Anya Ravenwood", "A brilliant cryptographer stumbles upon an ancient code that could rewrite history. But cracking it means confronting a conspiracy centuries in the making.", "CIP1313130001", 50.0, 45.0, 35.0, 40.0 });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50" },
                values: new object[] { "Elara Vance", "When silence falls over a once-thriving orchard, one woman returns to her childhood home to uncover the truth behind its abandonment and her family's past.", "ORC1414140001", 38.0, 34.0, 26.0, 30.0 });
        }
    }
}
