using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProdApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedProducs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "Description", "ImageUrl", "Price", "ProductName" },
                values: new object[,]
                {
                    { 11, 1, "Shoe from Nike.", "https://localhost:7189/images/nikerunner.jpg", 2999m, "Nike Runnerr" },
                    { 12, 1, "Shoe from Puma", "https://localhost:7189/images/pumarunner.jpg", 4999m, "Puma Runnerr" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 12);

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "CategoryId", "Description", "ImageUrl", "Price", "ProductName" },
                values: new object[,]
                {
                    { 1, 1, "Shoe from Nike.", "https://localhost:7189/images/nikerunner.jpg", 2999m, "Nike Runnerr" },
                    { 2, 1, "Shoe from Puma", "https://localhost:7189/images/pumarunner.jpg", 4999m, "Puma Runnerr" }
                });
        }
    }
}
