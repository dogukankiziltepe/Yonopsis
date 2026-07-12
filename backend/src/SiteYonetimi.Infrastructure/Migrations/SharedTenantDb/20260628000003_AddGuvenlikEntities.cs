using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations.SharedTenantDb
{
    /// <inheritdoc />
    public partial class AddGuvenlikEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ZiyaretciGirisCikislar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GelensAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GeldigiKisi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ZiyaretAmaci = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GirisSaati = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CikisSaati = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Plaka = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZiyaretciGirisCikislar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZiyaretciGirisCikislar_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(name: "IX_ZiyaretciGirisCikislar_SiteId_GirisSaati", table: "ZiyaretciGirisCikislar", columns: new[] { "SiteId", "GirisSaati" });
            migrationBuilder.CreateIndex(name: "IX_ZiyaretciGirisCikislar_UnitId", table: "ZiyaretciGirisCikislar", column: "UnitId");

            migrationBuilder.CreateTable(
                name: "AracGirisCikislar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Plaka = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SuruculAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AracTipi = table.Column<int>(type: "int", nullable: true),
                    GirisSaati = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CikisSaati = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AracGirisCikislar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AracGirisCikislar_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(name: "IX_AracGirisCikislar_SiteId_GirisSaati", table: "AracGirisCikislar", columns: new[] { "SiteId", "GirisSaati" });
            migrationBuilder.CreateIndex(name: "IX_AracGirisCikislar_Plaka", table: "AracGirisCikislar", column: "Plaka");
            migrationBuilder.CreateIndex(name: "IX_AracGirisCikislar_UnitId", table: "AracGirisCikislar", column: "UnitId");

            migrationBuilder.CreateTable(
                name: "Olaylar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    OlayTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    Konum = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Olaylar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Olaylar_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(name: "IX_Olaylar_SiteId_OlayTarihi", table: "Olaylar", columns: new[] { "SiteId", "OlayTarihi" });
            migrationBuilder.CreateIndex(name: "IX_Olaylar_SiteId_Durum", table: "Olaylar", columns: new[] { "SiteId", "Durum" });
            migrationBuilder.CreateIndex(name: "IX_Olaylar_UnitId", table: "Olaylar", column: "UnitId");

            migrationBuilder.CreateTable(
                name: "KayipEsyalar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EsyaAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BulunanYer = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    BulunanTarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SahipAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SahipIletisim = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KayipEsyalar", x => x.Id);
                });

            migrationBuilder.CreateIndex(name: "IX_KayipEsyalar_SiteId_BulunanTarih", table: "KayipEsyalar", columns: new[] { "SiteId", "BulunanTarih" });
            migrationBuilder.CreateIndex(name: "IX_KayipEsyalar_SiteId_Durum", table: "KayipEsyalar", columns: new[] { "SiteId", "Durum" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ZiyaretciGirisCikislar");
            migrationBuilder.DropTable(name: "AracGirisCikislar");
            migrationBuilder.DropTable(name: "Olaylar");
            migrationBuilder.DropTable(name: "KayipEsyalar");
        }
    }
}
