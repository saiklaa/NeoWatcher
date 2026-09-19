using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NeoWatcher.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeCloseApproaches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NearEarthObjects");

            migrationBuilder.CreateTable(
                name: "Asteroids",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    EstimatedDiameterMin = table.Column<double>(type: "double precision", nullable: false),
                    EstimatedDiameterMax = table.Column<double>(type: "double precision", nullable: false),
                    IsPotentiallyHazardous = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asteroids", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CloseApproaches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AsteroidId = table.Column<string>(type: "text", nullable: false),
                    CloseApproachDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RelativeVelocityKmh = table.Column<double>(type: "double precision", nullable: false),
                    MissDistanceKm = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CloseApproaches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CloseApproaches_Asteroids_AsteroidId",
                        column: x => x.AsteroidId,
                        principalTable: "Asteroids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CloseApproaches_AsteroidId_CloseApproachDate",
                table: "CloseApproaches",
                columns: new[] { "AsteroidId", "CloseApproachDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CloseApproaches");

            migrationBuilder.DropTable(
                name: "Asteroids");

            migrationBuilder.CreateTable(
                name: "NearEarthObjects",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CloseApproachDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EstimatedDiameterMax = table.Column<double>(type: "double precision", nullable: false),
                    EstimatedDiameterMin = table.Column<double>(type: "double precision", nullable: false),
                    IsPotentiallyHazardous = table.Column<bool>(type: "boolean", nullable: false),
                    MissDistanceKm = table.Column<double>(type: "double precision", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    RelativeVelocityKmh = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NearEarthObjects", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NearEarthObjects_CloseApproachDate",
                table: "NearEarthObjects",
                column: "CloseApproachDate");
        }
    }
}
