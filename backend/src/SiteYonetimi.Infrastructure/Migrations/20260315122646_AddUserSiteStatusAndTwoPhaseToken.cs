using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSiteStatusAndTwoPhaseToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSites_UserId_SiteId_UserType",
                table: "UserSites");

            migrationBuilder.AlterColumn<int>(
                name: "UserType",
                table: "UserSites",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UserSites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserSites_UserId_SiteId_UserType",
                table: "UserSites",
                columns: new[] { "UserId", "SiteId", "UserType" },
                unique: true,
                filter: "[UserType] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserSites_UserId_SiteId_UserType",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserSites");

            migrationBuilder.AlterColumn<int>(
                name: "UserType",
                table: "UserSites",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSites_UserId_SiteId_UserType",
                table: "UserSites",
                columns: new[] { "UserId", "SiteId", "UserType" },
                unique: true);
        }
    }
}
