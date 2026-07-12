using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations.SharedTenantDb
{
    /// <inheritdoc />
    public partial class Personel_Faz1_Genisletme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Personeller",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Aciklama",
                table: "Personeller",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Adres",
                table: "Personeller",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankaHesapNo",
                table: "Personeller",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankaIBAN",
                table: "Personeller",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BankaSubesiId",
                table: "Personeller",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "CikisTarihi",
                table: "Personeller",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cinsiyet",
                table: "Personeller",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Firma",
                table: "Personeller",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KanGrubu",
                table: "Personeller",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "KidemTazminatiBaslamaTarihi",
                table: "Personeller",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MuhasebeHesapKoduId",
                table: "Personeller",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OgrenimDurumu",
                table: "Personeller",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OkulKurum",
                table: "Personeller",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonelKodu",
                table: "Personeller",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "YemekKarti",
                table: "Personeller",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YillikIzinHakkiGun",
                table: "Personeller",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VergiDairesi",
                table: "HesapPlani",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VergiNumarasi",
                table: "HesapPlani",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BorclandirilacakKisi",
                table: "GiderTanimlari",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BosDairelereDagit",
                table: "GiderTanimlari",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DagitimSekli",
                table: "GiderTanimlari",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GiderKodu",
                table: "GiderTanimlari",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Kdv",
                table: "GiderTanimlari",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MuhasebeKodu",
                table: "GiderTanimlari",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PersonelAcilDurumKisileri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdSoyad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Yakinlik = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonelAcilDurumKisileri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonelAcilDurumKisileri_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonelEgitimleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EgitiminKonusu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Egitmen = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EgitimYeri = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BaslamaTarihi = table.Column<DateOnly>(type: "date", nullable: true),
                    BitisTarihi = table.Column<DateOnly>(type: "date", nullable: true),
                    ToplamSaat = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonelEgitimleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonelEgitimleri_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonelIzinleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaslangicTarihi = table.Column<DateOnly>(type: "date", nullable: false),
                    BitisTarihi = table.Column<DateOnly>(type: "date", nullable: false),
                    IzinTuru = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonelIzinleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonelIzinleri_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonelKimlikBilgileri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TcKimlikNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Seri = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Sira = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    BabaAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    AnaAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OncekiSoyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DogumYeri = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DogumTarihi = table.Column<DateOnly>(type: "date", nullable: true),
                    MedeniHali = table.Column<int>(type: "int", nullable: true),
                    Il = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Ilce = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MahalleKoy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    CiltNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AileSiraNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SiraNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    VerildigiYer = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    VerilisNedeni = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    KayitNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VerilisTarihi = table.Column<DateOnly>(type: "date", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonelKimlikBilgileri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonelKimlikBilgileri_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonelMuhasebeEntegrasyonlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrutUcretlerGiderTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HuzurHakkiBrutUcretlerGiderTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SgkIsverenPayiGiderTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IssizlikSigortasiIsverenPayiGiderTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PrimVeIkramiyelerGiderTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FazlaMesaiGiderTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    KidemTazminatlariGiderTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IhbarTazminatlariGiderTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    YolYardimiGiderTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    YemekYardimiGiderTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PersonelGelirVergisiHesapId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PersonelDamgaVergisiHesapId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OdenecekSgkHesapId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AsgariGecimIndirimiHesapId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IcraKesintisiHesapId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DigerKesintilerHesapId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BesHesapId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonelMuhasebeEntegrasyonlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonelMuhasebeEntegrasyonlari_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonelTelefonlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Label = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonelTelefonlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonelTelefonlari_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            // Mevcut kayıtlarda PersonelKodu/GiderKodu boş (varsayılan '') olduğundan,
            // yeni unique index'ler oluşturulmadan önce benzersiz geçici kodlar atanır.
            migrationBuilder.Sql(@"
                UPDATE p SET p.PersonelKodu = CONCAT('GECICI-', CAST(rn.RowNum AS varchar(10)))
                FROM Personeller p
                JOIN (SELECT Id, ROW_NUMBER() OVER (PARTITION BY SiteId ORDER BY CreatedAt) AS RowNum FROM Personeller WHERE PersonelKodu = '') rn
                    ON rn.Id = p.Id;
            ");
            migrationBuilder.Sql(@"
                UPDATE g SET g.GiderKodu = CONCAT('GECICI-', CAST(rn.RowNum AS varchar(10)))
                FROM GiderTanimlari g
                JOIN (SELECT Id, ROW_NUMBER() OVER (PARTITION BY SiteId ORDER BY CreatedAt) AS RowNum FROM GiderTanimlari WHERE GiderKodu = '') rn
                    ON rn.Id = g.Id;
            ");

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_MuhasebeHesapKoduId",
                table: "Personeller",
                column: "MuhasebeHesapKoduId");

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_SiteId_PersonelKodu",
                table: "Personeller",
                columns: new[] { "SiteId", "PersonelKodu" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_GiderTanimlari_SiteId_GiderKodu",
                table: "GiderTanimlari",
                columns: new[] { "SiteId", "GiderKodu" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelAcilDurumKisileri_PersonelId",
                table: "PersonelAcilDurumKisileri",
                column: "PersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelAcilDurumKisileri_SiteId_PersonelId",
                table: "PersonelAcilDurumKisileri",
                columns: new[] { "SiteId", "PersonelId" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonelEgitimleri_PersonelId",
                table: "PersonelEgitimleri",
                column: "PersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelEgitimleri_SiteId_PersonelId",
                table: "PersonelEgitimleri",
                columns: new[] { "SiteId", "PersonelId" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonelIzinleri_PersonelId",
                table: "PersonelIzinleri",
                column: "PersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelIzinleri_SiteId_PersonelId",
                table: "PersonelIzinleri",
                columns: new[] { "SiteId", "PersonelId" });

            migrationBuilder.CreateIndex(
                name: "IX_PersonelKimlikBilgileri_PersonelId",
                table: "PersonelKimlikBilgileri",
                column: "PersonelId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelMuhasebeEntegrasyonlari_PersonelId",
                table: "PersonelMuhasebeEntegrasyonlari",
                column: "PersonelId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelTelefonlari_PersonelId",
                table: "PersonelTelefonlari",
                column: "PersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelTelefonlari_SiteId_PersonelId",
                table: "PersonelTelefonlari",
                columns: new[] { "SiteId", "PersonelId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Personeller_HesapPlani_MuhasebeHesapKoduId",
                table: "Personeller",
                column: "MuhasebeHesapKoduId",
                principalTable: "HesapPlani",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personeller_HesapPlani_MuhasebeHesapKoduId",
                table: "Personeller");

            migrationBuilder.DropTable(
                name: "PersonelAcilDurumKisileri");

            migrationBuilder.DropTable(
                name: "PersonelEgitimleri");

            migrationBuilder.DropTable(
                name: "PersonelIzinleri");

            migrationBuilder.DropTable(
                name: "PersonelKimlikBilgileri");

            migrationBuilder.DropTable(
                name: "PersonelMuhasebeEntegrasyonlari");

            migrationBuilder.DropTable(
                name: "PersonelTelefonlari");

            migrationBuilder.DropIndex(
                name: "IX_Personeller_MuhasebeHesapKoduId",
                table: "Personeller");

            migrationBuilder.DropIndex(
                name: "IX_Personeller_SiteId_PersonelKodu",
                table: "Personeller");

            migrationBuilder.DropIndex(
                name: "IX_GiderTanimlari_SiteId_GiderKodu",
                table: "GiderTanimlari");

            migrationBuilder.DropColumn(
                name: "Aciklama",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "Adres",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "BankaHesapNo",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "BankaIBAN",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "BankaSubesiId",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "CikisTarihi",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "Cinsiyet",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "Firma",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "KanGrubu",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "KidemTazminatiBaslamaTarihi",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "MuhasebeHesapKoduId",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "OgrenimDurumu",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "OkulKurum",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "PersonelKodu",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "YemekKarti",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "YillikIzinHakkiGun",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "VergiDairesi",
                table: "HesapPlani");

            migrationBuilder.DropColumn(
                name: "VergiNumarasi",
                table: "HesapPlani");

            migrationBuilder.DropColumn(
                name: "BorclandirilacakKisi",
                table: "GiderTanimlari");

            migrationBuilder.DropColumn(
                name: "BosDairelereDagit",
                table: "GiderTanimlari");

            migrationBuilder.DropColumn(
                name: "DagitimSekli",
                table: "GiderTanimlari");

            migrationBuilder.DropColumn(
                name: "GiderKodu",
                table: "GiderTanimlari");

            migrationBuilder.DropColumn(
                name: "Kdv",
                table: "GiderTanimlari");

            migrationBuilder.DropColumn(
                name: "MuhasebeKodu",
                table: "GiderTanimlari");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Personeller",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);
        }
    }
}
