using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations.SharedTenantDb
{
    /// <inheritdoc />
    public partial class AddUnitFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Units",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Units",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Units",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Direction",
                table: "Units",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoorNumber",
                table: "Units",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Floor",
                table: "Units",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GrossArea",
                table: "Units",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasDask",
                table: "Units",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Internet",
                table: "Units",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LandShare",
                table: "Units",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyFee",
                table: "Units",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NetArea",
                table: "Units",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerUserId",
                table: "Units",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParkingCount",
                table: "Units",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Units",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantUserId",
                table: "Units",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Units",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<Guid>(
                name: "UnitTypeId",
                table: "Units",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Units_UnitTypeId",
                table: "Units",
                column: "UnitTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Units_UnitTypes_UnitTypeId",
                table: "Units",
                column: "UnitTypeId",
                principalTable: "UnitTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_UnitTypes_UnitTypeId",
                table: "Units");

            migrationBuilder.DropIndex(
                name: "IX_Units_UnitTypeId",
                table: "Units");

            migrationBuilder.DropColumn(name: "Code", table: "Units");
            migrationBuilder.DropColumn(name: "CreatedAt", table: "Units");
            migrationBuilder.DropColumn(name: "Description", table: "Units");
            migrationBuilder.DropColumn(name: "Direction", table: "Units");
            migrationBuilder.DropColumn(name: "DoorNumber", table: "Units");
            migrationBuilder.DropColumn(name: "Floor", table: "Units");
            migrationBuilder.DropColumn(name: "GrossArea", table: "Units");
            migrationBuilder.DropColumn(name: "HasDask", table: "Units");
            migrationBuilder.DropColumn(name: "Internet", table: "Units");
            migrationBuilder.DropColumn(name: "LandShare", table: "Units");
            migrationBuilder.DropColumn(name: "MonthlyFee", table: "Units");
            migrationBuilder.DropColumn(name: "NetArea", table: "Units");
            migrationBuilder.DropColumn(name: "OwnerUserId", table: "Units");
            migrationBuilder.DropColumn(name: "ParkingCount", table: "Units");
            migrationBuilder.DropColumn(name: "Status", table: "Units");
            migrationBuilder.DropColumn(name: "TenantUserId", table: "Units");
            migrationBuilder.DropColumn(name: "UpdatedAt", table: "Units");
            migrationBuilder.DropColumn(name: "UnitTypeId", table: "Units");
        }
    }
}
