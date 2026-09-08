using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations.SharedTenantDb
{
    /// <inheritdoc />
    public partial class Finans_Faz2_YeniEntityler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DevirBakiyeleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvrakNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BorcluUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BorcluRol = table.Column<int>(type: "int", nullable: true),
                    BorcluAdiSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevirBakiyeleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DevirBakiyeleri_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GelirTahsilatMakbuzlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvrakNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CariHesapId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KasaBankaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GelirTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DagitimYapilacak = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GelirTahsilatMakbuzlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GelirTahsilatMakbuzlari_GelirTanimlari_GelirTanimiId",
                        column: x => x.GelirTanimiId,
                        principalTable: "GelirTanimlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GelirTahsilatMakbuzlari_KasaBanka_KasaBankaId",
                        column: x => x.KasaBankaId,
                        principalTable: "KasaBanka",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IadeMakbuzlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvrakNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BorcluUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BorcluRol = table.Column<int>(type: "int", nullable: true),
                    BorcluAdiSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    KasaBankaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IadeMakbuzlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IadeMakbuzlari_KasaBanka_KasaBankaId",
                        column: x => x.KasaBankaId,
                        principalTable: "KasaBanka",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OdemeMakbuzlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvrakNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CariHesapId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KasaBankaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GiderTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DagitimYapilacak = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OdemeMakbuzlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OdemeMakbuzlari_GiderTanimlari_GiderTanimiId",
                        column: x => x.GiderTanimiId,
                        principalTable: "GiderTanimlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OdemeMakbuzlari_KasaBanka_KasaBankaId",
                        column: x => x.KasaBankaId,
                        principalTable: "KasaBanka",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DevirBakiyeleri_SiteId_BorcluUserId",
                table: "DevirBakiyeleri",
                columns: new[] { "SiteId", "BorcluUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_DevirBakiyeleri_SiteId_EvrakNo",
                table: "DevirBakiyeleri",
                columns: new[] { "SiteId", "EvrakNo" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_DevirBakiyeleri_SiteId_UnitId",
                table: "DevirBakiyeleri",
                columns: new[] { "SiteId", "UnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_DevirBakiyeleri_UnitId",
                table: "DevirBakiyeleri",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_GelirTahsilatMakbuzlari_GelirTanimiId",
                table: "GelirTahsilatMakbuzlari",
                column: "GelirTanimiId");

            migrationBuilder.CreateIndex(
                name: "IX_GelirTahsilatMakbuzlari_KasaBankaId",
                table: "GelirTahsilatMakbuzlari",
                column: "KasaBankaId");

            migrationBuilder.CreateIndex(
                name: "IX_GelirTahsilatMakbuzlari_SiteId_CariHesapId",
                table: "GelirTahsilatMakbuzlari",
                columns: new[] { "SiteId", "CariHesapId" });

            migrationBuilder.CreateIndex(
                name: "IX_GelirTahsilatMakbuzlari_SiteId_EvrakNo",
                table: "GelirTahsilatMakbuzlari",
                columns: new[] { "SiteId", "EvrakNo" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_IadeMakbuzlari_KasaBankaId",
                table: "IadeMakbuzlari",
                column: "KasaBankaId");

            migrationBuilder.CreateIndex(
                name: "IX_IadeMakbuzlari_SiteId_BorcluUserId",
                table: "IadeMakbuzlari",
                columns: new[] { "SiteId", "BorcluUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_IadeMakbuzlari_SiteId_EvrakNo",
                table: "IadeMakbuzlari",
                columns: new[] { "SiteId", "EvrakNo" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_OdemeMakbuzlari_GiderTanimiId",
                table: "OdemeMakbuzlari",
                column: "GiderTanimiId");

            migrationBuilder.CreateIndex(
                name: "IX_OdemeMakbuzlari_KasaBankaId",
                table: "OdemeMakbuzlari",
                column: "KasaBankaId");

            migrationBuilder.CreateIndex(
                name: "IX_OdemeMakbuzlari_SiteId_CariHesapId",
                table: "OdemeMakbuzlari",
                columns: new[] { "SiteId", "CariHesapId" });

            migrationBuilder.CreateIndex(
                name: "IX_OdemeMakbuzlari_SiteId_EvrakNo",
                table: "OdemeMakbuzlari",
                columns: new[] { "SiteId", "EvrakNo" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DevirBakiyeleri");

            migrationBuilder.DropTable(
                name: "GelirTahsilatMakbuzlari");

            migrationBuilder.DropTable(
                name: "IadeMakbuzlari");

            migrationBuilder.DropTable(
                name: "OdemeMakbuzlari");
        }
    }
}
