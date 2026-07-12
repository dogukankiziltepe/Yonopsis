using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations.MasterDb
{
    /// <inheritdoc />
    public partial class AddPersonDetailFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "UserSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "UserSites",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BirthPlace",
                table: "UserSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "UserSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EducationStatus",
                table: "UserSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FamilySiraNo",
                table: "UserSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FatherName",
                table: "UserSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasPrivateInsurance",
                table: "UserSites",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "IdentitySeriNo",
                table: "UserSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentitySiraNo",
                table: "UserSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMartyrOrVeteranRelative",
                table: "UserSites",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "KayitSiraNo",
                table: "UserSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaritalStatus",
                table: "UserSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotherName",
                table: "UserSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Nationality",
                table: "UserSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportNo",
                table: "UserSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PetDetail",
                table: "UserSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PetType",
                table: "UserSites",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Profession",
                table: "UserSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisteredCity",
                table: "UserSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisteredDistrict",
                table: "UserSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegisteredNeighborhood",
                table: "UserSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SchoolOrInstitution",
                table: "UserSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecondaryEmail",
                table: "UserSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxOffice",
                table: "UserSites",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PersonPhones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserSiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonPhones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonPhones_UserSites_UserSiteId",
                        column: x => x.UserSiteId,
                        principalTable: "UserSites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonPhones_UserSiteId",
                table: "PersonPhones",
                column: "UserSiteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonPhones");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "BirthPlace",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "EducationStatus",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "FamilySiraNo",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "FatherName",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "HasPrivateInsurance",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "IdentitySeriNo",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "IdentitySiraNo",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "IsMartyrOrVeteranRelative",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "KayitSiraNo",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "MotherName",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "PassportNo",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "PetDetail",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "PetType",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "Profession",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "RegisteredCity",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "RegisteredDistrict",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "RegisteredNeighborhood",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "SchoolOrInstitution",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "SecondaryEmail",
                table: "UserSites");

            migrationBuilder.DropColumn(
                name: "TaxOffice",
                table: "UserSites");
        }
    }
}
