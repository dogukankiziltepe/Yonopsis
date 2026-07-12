using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations.SharedTenantDb
{
    /// <inheritdoc />
    public partial class AddUnitDetailAndSharePercentage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code2",
                table: "Units",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code3",
                table: "Units",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "Units",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SharePercentage",
                table: "PersonUnitHistories",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code2",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Code3",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "SharePercentage",
                table: "PersonUnitHistories");
        }
    }
}
