using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations.SharedTenantDb
{
    /// <inheritdoc />
    public partial class Finans_HesaplarArasiVirman : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HesaplarArasiVirmanlar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvrakNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BelgeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BelgeNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HesaplarArasiVirmanlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VirmanSatirlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VirmanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiraNo = table.Column<int>(type: "int", nullable: false),
                    HesapTuru = table.Column<int>(type: "int", nullable: false),
                    KisiUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KisiAdiSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GelirTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BorcTutari = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AlacakTutari = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirmanSatirlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VirmanSatirlari_GelirTanimlari_GelirTanimiId",
                        column: x => x.GelirTanimiId,
                        principalTable: "GelirTanimlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_VirmanSatirlari_HesaplarArasiVirmanlar_VirmanId",
                        column: x => x.VirmanId,
                        principalTable: "HesaplarArasiVirmanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VirmanSatirlari_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HesaplarArasiVirmanlar_SiteId_EvrakNo",
                table: "HesaplarArasiVirmanlar",
                columns: new[] { "SiteId", "EvrakNo" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_HesaplarArasiVirmanlar_SiteId_Tarih",
                table: "HesaplarArasiVirmanlar",
                columns: new[] { "SiteId", "Tarih" });

            migrationBuilder.CreateIndex(
                name: "IX_VirmanSatirlari_GelirTanimiId",
                table: "VirmanSatirlari",
                column: "GelirTanimiId");

            migrationBuilder.CreateIndex(
                name: "IX_VirmanSatirlari_SiteId_KisiUserId",
                table: "VirmanSatirlari",
                columns: new[] { "SiteId", "KisiUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_VirmanSatirlari_UnitId",
                table: "VirmanSatirlari",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_VirmanSatirlari_VirmanId",
                table: "VirmanSatirlari",
                column: "VirmanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VirmanSatirlari");

            migrationBuilder.DropTable(
                name: "HesaplarArasiVirmanlar");
        }
    }
}
