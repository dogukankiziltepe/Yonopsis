using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations.SharedTenantDb
{
    /// <inheritdoc />
    public partial class Finans_KasaTransfer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KasaTransferleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvrakNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    BelgeNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CikisKasaBankaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GirisKasaBankaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KasaTransferleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KasaTransferleri_KasaBanka_CikisKasaBankaId",
                        column: x => x.CikisKasaBankaId,
                        principalTable: "KasaBanka",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KasaTransferleri_KasaBanka_GirisKasaBankaId",
                        column: x => x.GirisKasaBankaId,
                        principalTable: "KasaBanka",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KasaTransferleri_CikisKasaBankaId",
                table: "KasaTransferleri",
                column: "CikisKasaBankaId");

            migrationBuilder.CreateIndex(
                name: "IX_KasaTransferleri_GirisKasaBankaId",
                table: "KasaTransferleri",
                column: "GirisKasaBankaId");

            migrationBuilder.CreateIndex(
                name: "IX_KasaTransferleri_SiteId_EvrakNo",
                table: "KasaTransferleri",
                columns: new[] { "SiteId", "EvrakNo" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_KasaTransferleri_SiteId_Tarih",
                table: "KasaTransferleri",
                columns: new[] { "SiteId", "Tarih" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KasaTransferleri");
        }
    }
}
