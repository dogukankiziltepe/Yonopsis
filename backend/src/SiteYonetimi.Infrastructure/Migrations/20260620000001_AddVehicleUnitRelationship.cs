using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleUnitRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Rename UserId → OwnerUserId (cross-context soft ref, now nullable)
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Vehicles",
                newName: "OwnerUserId");

            migrationBuilder.AlterColumn<Guid>(
                name: "OwnerUserId",
                table: "Vehicles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            // 2. Add UnitId as nullable (Phase 1 — safe for existing orphan rows).
            //    Run the data backfill SQL before applying Phase 2 migration.
            //    Suggested backfill (assigns vehicle to a unit where owner/tenant matches):
            //
            //    UPDATE v SET v.UnitId = u.Id
            //    FROM Vehicles v
            //    JOIN Units u ON u.SiteId = v.SiteId AND u.IsDeleted = 0
            //        AND (u.OwnerUserId = v.OwnerUserId OR u.TenantUserId = v.OwnerUserId)
            //    WHERE v.UnitId IS NULL AND v.OwnerUserId IS NOT NULL AND v.IsDeleted = 0;
            //
            //    After confirming all rows have UnitId filled (SELECT COUNT(*) FROM Vehicles WHERE UnitId IS NULL AND IsDeleted = 0),
            //    run the Phase 2 migration: AddVehicleUnitIdRequired.
            migrationBuilder.AddColumn<Guid>(
                name: "UnitId",
                table: "Vehicles",
                type: "uniqueidentifier",
                nullable: true);

            // 3. Index for FK lookups
            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_UnitId",
                table: "Vehicles",
                column: "UnitId");

            // 4. Unique plate per site (excluding soft-deleted rows)
            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_SiteId_Plate",
                table: "Vehicles",
                columns: new[] { "SiteId", "Plate" },
                unique: true,
                filter: "[IsDeleted] = 0");

            // 5. FK constraint (nullable, so existing rows with NULL UnitId are allowed)
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
                name: "IX_Vehicles_SiteId_Plate",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_UnitId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "Vehicles");

            migrationBuilder.RenameColumn(
                name: "OwnerUserId",
                table: "Vehicles",
                newName: "UserId");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
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
