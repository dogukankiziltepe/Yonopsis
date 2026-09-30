using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations.SharedTenantDb
{
    /// <inheritdoc />
    public partial class Finans_VirmanFisDetaylari : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VirmanSatirlari_SiteId_KisiUserId",
                table: "VirmanSatirlari");

            migrationBuilder.RenameColumn(
                name: "KisiUserId",
                table: "VirmanSatirlari",
                newName: "HesapId");

            migrationBuilder.RenameColumn(
                name: "KisiAdiSnapshot",
                table: "VirmanSatirlari",
                newName: "HesapAdiSnapshot");

            migrationBuilder.AddColumn<decimal>(
                name: "AylikTazminatYuzdesi",
                table: "VirmanSatirlari",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BorcDonemi",
                table: "VirmanSatirlari",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GecikmeTazminatiUygula",
                table: "VirmanSatirlari",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "IcraDosyaNo",
                table: "VirmanSatirlari",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IcraTakibinde",
                table: "VirmanSatirlari",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "IcrayaVerilmeTarihi",
                table: "VirmanSatirlari",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SonOdemeTarihi",
                table: "VirmanSatirlari",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TazminatBaslamaTarihi",
                table: "VirmanSatirlari",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TazminatHesapTarihi",
                table: "VirmanSatirlari",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TazminatUygulamaSekli",
                table: "VirmanSatirlari",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VirmanSatirlari_SiteId_HesapTuru_HesapId",
                table: "VirmanSatirlari",
                columns: new[] { "SiteId", "HesapTuru", "HesapId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VirmanSatirlari_SiteId_HesapTuru_HesapId",
                table: "VirmanSatirlari");

            migrationBuilder.DropColumn(
                name: "AylikTazminatYuzdesi",
                table: "VirmanSatirlari");

            migrationBuilder.DropColumn(
                name: "BorcDonemi",
                table: "VirmanSatirlari");

            migrationBuilder.DropColumn(
                name: "GecikmeTazminatiUygula",
                table: "VirmanSatirlari");

            migrationBuilder.DropColumn(
                name: "IcraDosyaNo",
                table: "VirmanSatirlari");

            migrationBuilder.DropColumn(
                name: "IcraTakibinde",
                table: "VirmanSatirlari");

            migrationBuilder.DropColumn(
                name: "IcrayaVerilmeTarihi",
                table: "VirmanSatirlari");

            migrationBuilder.DropColumn(
                name: "SonOdemeTarihi",
                table: "VirmanSatirlari");

            migrationBuilder.DropColumn(
                name: "TazminatBaslamaTarihi",
                table: "VirmanSatirlari");

            migrationBuilder.DropColumn(
                name: "TazminatHesapTarihi",
                table: "VirmanSatirlari");

            migrationBuilder.DropColumn(
                name: "TazminatUygulamaSekli",
                table: "VirmanSatirlari");

            migrationBuilder.RenameColumn(
                name: "HesapId",
                table: "VirmanSatirlari",
                newName: "KisiUserId");

            migrationBuilder.RenameColumn(
                name: "HesapAdiSnapshot",
                table: "VirmanSatirlari",
                newName: "KisiAdiSnapshot");

            migrationBuilder.CreateIndex(
                name: "IX_VirmanSatirlari_SiteId_KisiUserId",
                table: "VirmanSatirlari",
                columns: new[] { "SiteId", "KisiUserId" });
        }
    }
}
