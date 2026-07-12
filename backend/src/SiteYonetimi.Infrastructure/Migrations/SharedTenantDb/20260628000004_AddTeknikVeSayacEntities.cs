using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations.SharedTenantDb
{
    /// <inheritdoc />
    public partial class AddTeknikVeSayacEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── Teknik ────────────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "Departmanlar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departmanlar", x => x.Id);
                });

            migrationBuilder.CreateIndex(name: "IX_Departmanlar_SiteId_Ad", table: "Departmanlar", columns: new[] { "SiteId", "Ad" }, unique: true, filter: "[IsDeleted] = 0");

            migrationBuilder.CreateTable(
                name: "OrtakAlanlar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Konum = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrtakAlanlar", x => x.Id);
                });

            migrationBuilder.CreateIndex(name: "IX_OrtakAlanlar_SiteId", table: "OrtakAlanlar", column: "SiteId");

            migrationBuilder.CreateTable(
                name: "TalepTipleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TalepTipleri", x => x.Id);
                });

            migrationBuilder.CreateIndex(name: "IX_TalepTipleri_SiteId", table: "TalepTipleri", column: "SiteId");

            migrationBuilder.CreateTable(
                name: "IsEmirleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    TalepTipiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrtakAlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Oncelik = table.Column<int>(type: "int", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    AtananKisiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AtananKisiAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IslemBaslangic = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IslemBitis = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notlar = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IsEmirleri", x => x.Id);
                    table.ForeignKey(name: "FK_IsEmirleri_TalepTipleri_TalepTipiId", column: x => x.TalepTipiId, principalTable: "TalepTipleri", principalColumn: "Id", onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(name: "FK_IsEmirleri_Departmanlar_DepartmanId", column: x => x.DepartmanId, principalTable: "Departmanlar", principalColumn: "Id", onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(name: "FK_IsEmirleri_OrtakAlanlar_OrtakAlanId", column: x => x.OrtakAlanId, principalTable: "OrtakAlanlar", principalColumn: "Id", onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(name: "FK_IsEmirleri_Units_UnitId", column: x => x.UnitId, principalTable: "Units", principalColumn: "Id", onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(name: "IX_IsEmirleri_SiteId_Durum", table: "IsEmirleri", columns: new[] { "SiteId", "Durum" });
            migrationBuilder.CreateIndex(name: "IX_IsEmirleri_SiteId_CreatedAt", table: "IsEmirleri", columns: new[] { "SiteId", "CreatedAt" });
            migrationBuilder.CreateIndex(name: "IX_IsEmirleri_TalepTipiId", table: "IsEmirleri", column: "TalepTipiId");
            migrationBuilder.CreateIndex(name: "IX_IsEmirleri_DepartmanId", table: "IsEmirleri", column: "DepartmanId");
            migrationBuilder.CreateIndex(name: "IX_IsEmirleri_OrtakAlanId", table: "IsEmirleri", column: "OrtakAlanId");
            migrationBuilder.CreateIndex(name: "IX_IsEmirleri_UnitId", table: "IsEmirleri", column: "UnitId");

            // ── Sayaç ─────────────────────────────────────────────────────
            migrationBuilder.CreateTable(
                name: "AnaSayaclar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    SeriNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Marka = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TakimTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnaSayaclar", x => x.Id);
                });

            migrationBuilder.CreateIndex(name: "IX_AnaSayaclar_SiteId_Tip", table: "AnaSayaclar", columns: new[] { "SiteId", "Tip" });

            migrationBuilder.CreateTable(
                name: "DaireSayaclar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnaSayacId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    SeriNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Marka = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TakimTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DaireSayaclar", x => x.Id);
                    table.ForeignKey(name: "FK_DaireSayaclar_Units_UnitId", column: x => x.UnitId, principalTable: "Units", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(name: "FK_DaireSayaclar_AnaSayaclar_AnaSayacId", column: x => x.AnaSayacId, principalTable: "AnaSayaclar", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(name: "IX_DaireSayaclar_SiteId_UnitId_Tip", table: "DaireSayaclar", columns: new[] { "SiteId", "UnitId", "Tip" });
            migrationBuilder.CreateIndex(name: "IX_DaireSayaclar_UnitId", table: "DaireSayaclar", column: "UnitId");
            migrationBuilder.CreateIndex(name: "IX_DaireSayaclar_AnaSayacId", table: "DaireSayaclar", column: "AnaSayacId");

            migrationBuilder.CreateTable(
                name: "SayacOkumalar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnaSayacId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DaireSayacId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OkumaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OncekiEndeks = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    SonEndeks = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SayacOkumalar", x => x.Id);
                    table.ForeignKey(name: "FK_SayacOkumalar_AnaSayaclar_AnaSayacId", column: x => x.AnaSayacId, principalTable: "AnaSayaclar", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(name: "FK_SayacOkumalar_DaireSayaclar_DaireSayacId", column: x => x.DaireSayacId, principalTable: "DaireSayaclar", principalColumn: "Id", onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(name: "IX_SayacOkumalar_SiteId_OkumaTarihi", table: "SayacOkumalar", columns: new[] { "SiteId", "OkumaTarihi" });
            migrationBuilder.CreateIndex(name: "IX_SayacOkumalar_AnaSayacId", table: "SayacOkumalar", column: "AnaSayacId");
            migrationBuilder.CreateIndex(name: "IX_SayacOkumalar_DaireSayacId", table: "SayacOkumalar", column: "DaireSayacId");

            migrationBuilder.CreateTable(
                name: "BirimFiyatlar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    Fiyat = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Birim = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirimFiyatlar", x => x.Id);
                });

            migrationBuilder.CreateIndex(name: "IX_BirimFiyatlar_SiteId_Tip_BaslangicTarihi", table: "BirimFiyatlar", columns: new[] { "SiteId", "Tip", "BaslangicTarihi" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "IsEmirleri");
            migrationBuilder.DropTable(name: "TalepTipleri");
            migrationBuilder.DropTable(name: "Departmanlar");
            migrationBuilder.DropTable(name: "OrtakAlanlar");
            migrationBuilder.DropTable(name: "SayacOkumalar");
            migrationBuilder.DropTable(name: "DaireSayaclar");
            migrationBuilder.DropTable(name: "BirimFiyatlar");
            migrationBuilder.DropTable(name: "AnaSayaclar");
        }
    }
}
