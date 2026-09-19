using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NeoWatcher.Migrations
{
    /// <inheritdoc />
    public partial class AddCloseApproachDateIndexBack : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CloseApproaches_CloseApproachDate",
                table: "CloseApproaches",
                column: "CloseApproachDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CloseApproaches_CloseApproachDate",
                table: "CloseApproaches");
        }
    }
}
