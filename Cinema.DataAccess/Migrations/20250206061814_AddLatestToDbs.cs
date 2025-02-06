using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Cinema.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddLatestToDbs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    movieID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    trailerLink = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    releaseDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ageLimit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    movieImage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.movieID);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    productID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nameProduct = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    productType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    price = table.Column<double>(type: "float", nullable: false),
                    productImage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.productID);
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "movieID", "ageLimit", "createdAt", "description", "duration", "genre", "movieImage", "releaseDate", "title", "trailerLink", "updatedAt" },
                values: new object[,]
                {
                    { 1, "10+", null, "A thief who enters the dreams of others to steal secrets.", "148 min", "Sci-Fi", "", "2010-07-16", "Inception", "https://example.com/inception", null },
                    { 2, "18+", null, "Batman faces the Joker, a criminal mastermind.", "152 min", "Action", "", "2008-07-18", "The Dark Knight", "https://example.com/darkknight", null }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "productID", "description", "nameProduct", "price", "productImage", "productType" },
                values: new object[,]
                {
                    { 1, "A large bucket of buttered popcorn.", "Popcorn", 5.9900000000000002, "", "Snack" },
                    { 2, "Refreshing cold soda, 500ml.", "Soda", 2.9900000000000002, "", "Drink" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
