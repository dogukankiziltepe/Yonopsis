using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations.SharedTenantDb
{
    /// <inheritdoc />
    public partial class AddFinansEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BorcMakbuzlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvrakNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Donem = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SonOdemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BorcluAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GelirTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GecikmeTutari = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    OdenenTutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorcMakbuzlari", x => x.Id);
                    table.ForeignKey(name: "FK_BorcMakbuzlari_Units_UnitId", column: x => x.UnitId, principalTable: "Units", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(name: "FK_BorcMakbuzlari_GelirTanimlari_GelirTanimiId", column: x => x.GelirTanimiId, principalTable: "GelirTanimlari", principalColumn: "Id", onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(name: "IX_BorcMakbuzlari_SiteId_EvrakNo", table: "BorcMakbuzlari", columns: new[] { "SiteId", "EvrakNo" }, unique: true, filter: "[IsDeleted] = 0");
            migrationBuilder.CreateIndex(name: "IX_BorcMakbuzlari_UnitId", table: "BorcMakbuzlari", column: "UnitId");
            migrationBuilder.CreateIndex(name: "IX_BorcMakbuzlari_GelirTanimiId", table: "BorcMakbuzlari", column: "GelirTanimiId");

            migrationBuilder.CreateTable(
                name: "TahsilatMakbuzlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvrakNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BorcluAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    KasaBankaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BorcMakbuzuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OdemeTutari = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OdemeTipi = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TahsilatMakbuzlari", x => x.Id);
                    table.ForeignKey(name: "FK_TahsilatMakbuzlari_KasaBanka_KasaBankaId", column: x => x.KasaBankaId, principalTable: "KasaBanka", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(name: "FK_TahsilatMakbuzlari_BorcMakbuzlari_BorcMakbuzuId", column: x => x.BorcMakbuzuId, principalTable: "BorcMakbuzlari", principalColumn: "Id", onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(name: "IX_TahsilatMakbuzlari_SiteId_EvrakNo", table: "TahsilatMakbuzlari", columns: new[] { "SiteId", "EvrakNo" }, unique: true, filter: "[IsDeleted] = 0");
            migrationBuilder.CreateIndex(name: "IX_TahsilatMakbuzlari_KasaBankaId", table: "TahsilatMakbuzlari", column: "KasaBankaId");
            migrationBuilder.CreateIndex(name: "IX_TahsilatMakbuzlari_BorcMakbuzuId", table: "TahsilatMakbuzlari", column: "BorcMakbuzuId");

            migrationBuilder.CreateTable(
                name: "Faturalar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    EvrakNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FaturaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CariAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GelirTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GiderTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToplamTutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faturalar", x => x.Id);
                    table.ForeignKey(name: "FK_Faturalar_GelirTanimlari_GelirTanimiId", column: x => x.GelirTanimiId, principalTable: "GelirTanimlari", principalColumn: "Id", onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(name: "FK_Faturalar_GiderTanimlari_GiderTanimiId", column: x => x.GiderTanimiId, principalTable: "GiderTanimlari", principalColumn: "Id", onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(name: "IX_Faturalar_SiteId_Tip_EvrakNo", table: "Faturalar", columns: new[] { "SiteId", "Tip", "EvrakNo" }, unique: true, filter: "[IsDeleted] = 0");
            migrationBuilder.CreateIndex(name: "IX_Faturalar_GelirTanimiId", table: "Faturalar", column: "GelirTanimiId");
            migrationBuilder.CreateIndex(name: "IX_Faturalar_GiderTanimiId", table: "Faturalar", column: "GiderTanimiId");

            migrationBuilder.CreateTable(
                name: "BankaHareketleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KasaBankaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ReferansNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    EslestirmeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankaHareketleri", x => x.Id);
                    table.ForeignKey(name: "FK_BankaHareketleri_KasaBanka_KasaBankaId", column: x => x.KasaBankaId, principalTable: "KasaBanka", principalColumn: "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(name: "IX_BankaHareketleri_SiteId_KasaBankaId_Tarih", table: "BankaHareketleri", columns: new[] { "SiteId", "KasaBankaId", "Tarih" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "BankaHareketleri");
            migrationBuilder.DropTable(name: "Faturalar");
            migrationBuilder.DropTable(name: "TahsilatMakbuzlari");
            migrationBuilder.DropTable(name: "BorcMakbuzlari");
        }
    }
}
