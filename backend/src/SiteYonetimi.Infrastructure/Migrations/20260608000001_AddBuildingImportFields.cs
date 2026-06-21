using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBuildingImportFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalFloors",
                table: "Buildings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Buildings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Buildings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "TotalFloors", table: "Buildings");
            migrationBuilder.DropColumn(name: "Address",     table: "Buildings");
            migrationBuilder.DropColumn(name: "Description", table: "Buildings");
        }
    }
}
