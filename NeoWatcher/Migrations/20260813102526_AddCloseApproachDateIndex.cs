using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoWatcher.Migrations
{
    /// <inheritdoc />
    public partial class AddCloseApproachDateIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_NearEarthObjects_CloseApproachDate",
                table: "NearEarthObjects",
                column: "CloseApproachDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_NearEarthObjects_CloseApproachDate",
                table: "NearEarthObjects");
        }
    }
}
