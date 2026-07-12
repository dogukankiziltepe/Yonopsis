using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations.SharedTenantDb
{
    /// <inheritdoc />
    public partial class AddRezervasyonPersonelEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── Rezervasyon ────────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "Rezervasyonlar",
                columns: table => new
                {
                    Id        = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId    = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TesisId   = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PersonId  = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate   = table.Column<DateOnly>(type: "date", nullable: false),
                    Durum     = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Notes     = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervasyonlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rezervasyonlar_Tesisler_TesisId",
                        column: x => x.TesisId,
                        principalTable: "Tesisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rezervasyonlar_SiteId_StartDate",
                table: "Rezervasyonlar",
                columns: new[] { "SiteId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Rezervasyonlar_SiteId_TesisId_StartDate",
                table: "Rezervasyonlar",
                columns: new[] { "SiteId", "TesisId", "StartDate" });

            // ── Personel ──────────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "Personeller",
                columns: table => new
                {
                    Id           = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId       = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name         = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title        = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone        = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email        = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Department   = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StartDate    = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActive     = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted    = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt    = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt    = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personeller", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_SiteId_Name",
                table: "Personeller",
                columns: new[] { "SiteId", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Rezervasyonlar");
            migrationBuilder.DropTable(name: "Personeller");
        }
    }
}
