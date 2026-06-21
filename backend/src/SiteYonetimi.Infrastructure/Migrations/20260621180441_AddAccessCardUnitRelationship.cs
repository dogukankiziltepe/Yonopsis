using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccessCardUnitRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UnitId",
                table: "Vehicles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "UnitId",
                table: "AccessCards",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "HesapPlani",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HesapKodu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HesapAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HesapTipi = table.Column<int>(type: "int", nullable: false),
                    HesapKategorisi = table.Column<int>(type: "int", nullable: false),
                    NormalBakiye = table.Column<int>(type: "int", nullable: false),
                    Seviye = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FisKesilebilirMi = table.Column<bool>(type: "bit", nullable: false),
                    CariTuru = table.Column<int>(type: "int", nullable: true),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GiderTuruId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HesapPlani", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HesapPlani_HesapPlani_ParentId",
                        column: x => x.ParentId,
                        principalTable: "HesapPlani",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MuhasebeDonemler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Yil = table.Column<int>(type: "int", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BaslangicTarihi = table.Column<DateOnly>(type: "date", nullable: false),
                    BitisTarihi = table.Column<DateOnly>(type: "date", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    SonYevmiyeNo = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuhasebeDonemler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MuhasebeParametreler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VarsayilanKasaHesapId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VarsayilanBankaHesapId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AidatGelirHesapId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GecikmeFaiziHesapId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AlicilarAnaHesapKodu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SaticilarAnaHesapKodu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GiderAnaHesapKodu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CariKodSablonu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FisNoSablonu = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParaBirimi = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    KdvOrani = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    OtomatikTahsilFisi = table.Column<bool>(type: "bit", nullable: false),
                    OtomatikTediyeFisi = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuhasebeParametreler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MuhasebeFisleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DonemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    YevmiyeNo = table.Column<int>(type: "int", nullable: true),
                    FisNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FisTuru = table.Column<int>(type: "int", nullable: false),
                    FisTarihi = table.Column<DateOnly>(type: "date", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    ToplamBorc = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ToplamAlacak = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SatirSayisi = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuhasebeFisleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MuhasebeFisleri_MuhasebeDonemler_DonemId",
                        column: x => x.DonemId,
                        principalTable: "MuhasebeDonemler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MuhasebeFisiDetaylari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FisId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiraNo = table.Column<int>(type: "int", nullable: false),
                    HesapId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HesapKodu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BorcTutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    AlacakTutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    BelgeNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuhasebeFisiDetaylari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MuhasebeFisiDetaylari_MuhasebeFisleri_FisId",
                        column: x => x.FisId,
                        principalTable: "MuhasebeFisleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessCards_UnitId",
                table: "AccessCards",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_HesapPlani_ParentId",
                table: "HesapPlani",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_HesapPlani_SiteId_HesapKodu",
                table: "HesapPlani",
                columns: new[] { "SiteId", "HesapKodu" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_HesapPlani_SiteId_ParentId",
                table: "HesapPlani",
                columns: new[] { "SiteId", "ParentId" });

            migrationBuilder.CreateIndex(
                name: "IX_HesapPlani_SiteId_PersonId",
                table: "HesapPlani",
                columns: new[] { "SiteId", "PersonId" });

            migrationBuilder.CreateIndex(
                name: "IX_MuhasebeDonemler_SiteId_Yil",
                table: "MuhasebeDonemler",
                columns: new[] { "SiteId", "Yil" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_MuhasebeFisiDetaylari_FisId",
                table: "MuhasebeFisiDetaylari",
                column: "FisId");

            migrationBuilder.CreateIndex(
                name: "IX_MuhasebeFisiDetaylari_SiteId_HesapId",
                table: "MuhasebeFisiDetaylari",
                columns: new[] { "SiteId", "HesapId" });

            migrationBuilder.CreateIndex(
                name: "IX_MuhasebeFisleri_DonemId",
                table: "MuhasebeFisleri",
                column: "DonemId");

            migrationBuilder.CreateIndex(
                name: "IX_MuhasebeFisleri_SiteId_DonemId_YevmiyeNo",
                table: "MuhasebeFisleri",
                columns: new[] { "SiteId", "DonemId", "YevmiyeNo" });

            migrationBuilder.CreateIndex(
                name: "IX_MuhasebeFisleri_SiteId_FisNo",
                table: "MuhasebeFisleri",
                columns: new[] { "SiteId", "FisNo" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_MuhasebeFisleri_SiteId_FisTarihi",
                table: "MuhasebeFisleri",
                columns: new[] { "SiteId", "FisTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_MuhasebeParametreler_SiteId",
                table: "MuhasebeParametreler",
                column: "SiteId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.AddForeignKey(
                name: "FK_AccessCards_Units_UnitId",
                table: "AccessCards",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccessCards_Units_UnitId",
                table: "AccessCards");

            migrationBuilder.DropTable(
                name: "HesapPlani");

            migrationBuilder.DropTable(
                name: "MuhasebeFisiDetaylari");

            migrationBuilder.DropTable(
                name: "MuhasebeParametreler");

            migrationBuilder.DropTable(
                name: "MuhasebeFisleri");

            migrationBuilder.DropTable(
                name: "MuhasebeDonemler");

            migrationBuilder.DropIndex(
                name: "IX_AccessCards_UnitId",
                table: "AccessCards");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "AccessCards");

            migrationBuilder.AlterColumn<Guid>(
                name: "UnitId",
                table: "Vehicles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
