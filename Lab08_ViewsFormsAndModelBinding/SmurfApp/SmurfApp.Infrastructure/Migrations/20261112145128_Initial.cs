using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmurfApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Smurfs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Smurfs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Smurfs",
                columns: new[] { "Id", "Age", "Category", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { new Guid("01234567-89ab-4cde-8fab-0123456789ab"), 160, 2, "https://charactersdb.com/wp-content/uploads/jokey-smurf-cartoon.jpg", "Jokey Smurf" },
                    { new Guid("11223344-5566-4777-8899-aabbccddeeff"), 100, 1, "https://charactersdb.com/wp-content/uploads/baker-smurf-cartoon.jpg", "Chef Smurf" },
                    { new Guid("123e4567-e89b-42d3-a456-426614174000"), 100, 1, "https://charactersdb.com/wp-content/uploads/tracker-smurf-cartoon.jpg", "Tracker Smurf" },
                    { new Guid("7bd2c98e-8e90-4f50-9f7e-2fd9b5ce36f0"), 546, 0, "https://charactersdb.com/wp-content/uploads/papa-smurf-cartoon.jpg", "Papa Smurf" },
                    { new Guid("9f8e7d6c-5b4a-4321-8e9f-0a1b2c3d4e5f"), 99, 3, "https://charactersdb.com/wp-content/uploads/vanity-smurf-cartoon.jpg", "Vanity Smurf" },
                    { new Guid("a2b3c4d5-e6f7-4812-9a3b-0c1d2e3f4b5c"), 80, 3, "https://charactersdb.com/wp-content/uploads/smurfette-cartoon-80s.jpg", "Smurfette" },
                    { new Guid("c3b2a1f0-e9d8-4765-8c7b-6a5d4e3f2b1c"), 100, 2, "https://charactersdb.com/wp-content/uploads/clumsy-smurf-cartoon.jpg", "Clumsy Smurf" },
                    { new Guid("d1a1f4b2-3c6b-4f7e-9d1e-3b2c4a5f6e7a"), 101, 0, "https://charactersdb.com/wp-content/uploads/brainy-smurf-cartoon.jpg", "Brainy Smurf" },
                    { new Guid("f4e3d2c1-b0a9-4876-8f6e-5d4c3b2a1f0e"), 100, 1, "https://charactersdb.com/wp-content/uploads/hefty-smurf-cartoon.jpg", "Hefty Smurf" },
                    { new Guid("ffeeddcc-bbaa-4111-9988-776655443322"), 102, 1, "https://charactersdb.com/wp-content/uploads/poet-smurf-cartoon.jpg", "Poet Smurf" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Smurfs");
        }
    }
}
