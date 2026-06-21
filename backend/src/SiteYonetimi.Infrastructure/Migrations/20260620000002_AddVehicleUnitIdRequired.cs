using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations
{
    /// <summary>
    /// Phase 2: makes Vehicles.UnitId NOT NULL.
    /// Apply only after confirming all non-deleted vehicles have a UnitId:
    ///   SELECT COUNT(*) FROM Vehicles WHERE UnitId IS NULL AND IsDeleted = 0;
    /// Must return 0 before running this migration.
    /// </summary>
    public partial class AddVehicleUnitIdRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Units_UnitId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_UnitId",
                table: "Vehicles");

            migrationBuilder.AlterColumn<Guid>(
                name: "UnitId",
                table: "Vehicles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_UnitId",
                table: "Vehicles",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Units_UnitId",
                table: "Vehicles",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Units_UnitId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_UnitId",
                table: "Vehicles");

            migrationBuilder.AlterColumn<Guid>(
                name: "UnitId",
                table: "Vehicles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_UnitId",
                table: "Vehicles",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Units_UnitId",
                table: "Vehicles",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
