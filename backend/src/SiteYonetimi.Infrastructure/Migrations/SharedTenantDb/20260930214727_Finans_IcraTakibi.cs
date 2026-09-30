using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations.SharedTenantDb
{
    /// <inheritdoc />
    public partial class Finans_IcraTakibi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Avukatlar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdSoyad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BuroAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Eposta = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Adres = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avukatlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IcraTakipleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TakipTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BorcluUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BorcluAdiSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaslangicTutari = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IcraTakipleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IcraTakipleri_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IcraDosyalari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TakipId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DosyaNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IcraTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    BorcluUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BorcluAdiSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AvukatId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DosyaTutari = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IcraDosyalari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IcraDosyalari_Avukatlar_AvukatId",
                        column: x => x.AvukatId,
                        principalTable: "Avukatlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IcraDosyalari_IcraTakipleri_TakipId",
                        column: x => x.TakipId,
                        principalTable: "IcraTakipleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IcraDosyalari_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IcraTakipEvraklari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TakipId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BorcMakbuzuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IcraTakipEvraklari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IcraTakipEvraklari_BorcMakbuzlari_BorcMakbuzuId",
                        column: x => x.BorcMakbuzuId,
                        principalTable: "BorcMakbuzlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IcraTakipEvraklari_IcraTakipleri_TakipId",
                        column: x => x.TakipId,
                        principalTable: "IcraTakipleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IcraDosyasiEvraklari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DosyaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BorcMakbuzuId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IcraDosyasiEvraklari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IcraDosyasiEvraklari_BorcMakbuzlari_BorcMakbuzuId",
                        column: x => x.BorcMakbuzuId,
                        principalTable: "BorcMakbuzlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IcraDosyasiEvraklari_IcraDosyalari_DosyaId",
                        column: x => x.DosyaId,
                        principalTable: "IcraDosyalari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Avukatlar_SiteId",
                table: "Avukatlar",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_IcraDosyalari_AvukatId",
                table: "IcraDosyalari",
                column: "AvukatId");

            migrationBuilder.CreateIndex(
                name: "IX_IcraDosyalari_SiteId_DosyaNo",
                table: "IcraDosyalari",
                columns: new[] { "SiteId", "DosyaNo" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_IcraDosyalari_SiteId_IcraTarihi",
                table: "IcraDosyalari",
                columns: new[] { "SiteId", "IcraTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_IcraDosyalari_TakipId",
                table: "IcraDosyalari",
                column: "TakipId");

            migrationBuilder.CreateIndex(
                name: "IX_IcraDosyalari_UnitId",
                table: "IcraDosyalari",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_IcraDosyasiEvraklari_BorcMakbuzuId",
                table: "IcraDosyasiEvraklari",
                column: "BorcMakbuzuId");

            migrationBuilder.CreateIndex(
                name: "IX_IcraDosyasiEvraklari_DosyaId",
                table: "IcraDosyasiEvraklari",
                column: "DosyaId");

            migrationBuilder.CreateIndex(
                name: "IX_IcraTakipEvraklari_BorcMakbuzuId",
                table: "IcraTakipEvraklari",
                column: "BorcMakbuzuId");

            migrationBuilder.CreateIndex(
                name: "IX_IcraTakipEvraklari_TakipId",
                table: "IcraTakipEvraklari",
                column: "TakipId");

            migrationBuilder.CreateIndex(
                name: "IX_IcraTakipleri_SiteId_BorcluUserId_UnitId",
                table: "IcraTakipleri",
                columns: new[] { "SiteId", "BorcluUserId", "UnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_IcraTakipleri_SiteId_Durum",
                table: "IcraTakipleri",
                columns: new[] { "SiteId", "Durum" });

            migrationBuilder.CreateIndex(
                name: "IX_IcraTakipleri_UnitId",
                table: "IcraTakipleri",
                column: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IcraDosyasiEvraklari");

            migrationBuilder.DropTable(
                name: "IcraTakipEvraklari");

            migrationBuilder.DropTable(
                name: "IcraDosyalari");

            migrationBuilder.DropTable(
                name: "Avukatlar");

            migrationBuilder.DropTable(
                name: "IcraTakipleri");
        }
    }
}
