using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyApp.Infrastructure.Persistence.Ef.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUniverseHasData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Universes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Universes",
                keyColumn: "Id",
                keyValue: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Universes",
                columns: new[] { "Id", "ApiUrl", "IsEnabled", "Name" },
                values: new object[,]
                {
                    { 1, "https://pokeapi.co/api/v2/", true, "Pokemon" },
                    { 2, "https://swapi.dev/api/", true, "StarWars" }
                });
        }
    }
}
