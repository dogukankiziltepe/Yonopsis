using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations.SharedTenantDb
{
    /// <inheritdoc />
    public partial class RedesignFinansModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AidatKalemleri");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.RenameColumn(
                name: "BorcluAdi",
                table: "TahsilatMakbuzlari",
                newName: "BorcluAdiSnapshot");

            migrationBuilder.RenameColumn(
                name: "BorcluAdi",
                table: "BorcMakbuzlari",
                newName: "BorcluAdiSnapshot");

            migrationBuilder.AddColumn<Guid>(
                name: "BorcluUserId",
                table: "TahsilatMakbuzlari",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BorcluRol",
                table: "BorcMakbuzlari",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BorcluUserId",
                table: "BorcMakbuzlari",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TopluBorclandirmaBatchId",
                table: "BorcMakbuzlari",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TahsilatMakbuzlari_SiteId_BorcluUserId",
                table: "TahsilatMakbuzlari",
                columns: new[] { "SiteId", "BorcluUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_BorcMakbuzlari_SiteId_BorcluUserId",
                table: "BorcMakbuzlari",
                columns: new[] { "SiteId", "BorcluUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_BorcMakbuzlari_SiteId_UnitId",
                table: "BorcMakbuzlari",
                columns: new[] { "SiteId", "UnitId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TahsilatMakbuzlari_SiteId_BorcluUserId",
                table: "TahsilatMakbuzlari");

            migrationBuilder.DropIndex(
                name: "IX_BorcMakbuzlari_SiteId_BorcluUserId",
                table: "BorcMakbuzlari");

            migrationBuilder.DropIndex(
                name: "IX_BorcMakbuzlari_SiteId_UnitId",
                table: "BorcMakbuzlari");

            migrationBuilder.DropColumn(
                name: "BorcluUserId",
                table: "TahsilatMakbuzlari");

            migrationBuilder.DropColumn(
                name: "BorcluRol",
                table: "BorcMakbuzlari");

            migrationBuilder.DropColumn(
                name: "BorcluUserId",
                table: "BorcMakbuzlari");

            migrationBuilder.DropColumn(
                name: "TopluBorclandirmaBatchId",
                table: "BorcMakbuzlari");

            migrationBuilder.RenameColumn(
                name: "BorcluAdiSnapshot",
                table: "TahsilatMakbuzlari",
                newName: "BorcluAdi");

            migrationBuilder.RenameColumn(
                name: "BorcluAdiSnapshot",
                table: "BorcMakbuzlari",
                newName: "BorcluAdi");

            migrationBuilder.CreateTable(
                name: "AidatKalemleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AidatKalemleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AidatKalemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    PaidDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UnitId",
                table: "Payments",
                column: "UnitId");
        }
    }
}
