using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteYonetimi.Infrastructure.Migrations.SharedTenantDb
{
    /// <inheritdoc />
    public partial class AddFaturaOdemeAlanlari : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AjandaEtkinlikleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Konum = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Renk = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TumGun = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AjandaEtkinlikleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnaSayaclar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    SeriNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Marka = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TakimTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnaSayaclar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnaSayfaAyarlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Slogan = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    KisaAciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IletisimTelefon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IletisimEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Adres = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    KapakFotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnaSayfaAyarlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Anketler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anketler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AracGirisCikislar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Plaka = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SuruculAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AracTipi = table.Column<int>(type: "int", nullable: true),
                    GirisSaati = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CikisSaati = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AracGirisCikislar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AracGirisCikislar_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BirimFiyatlar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    Fiyat = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Birim = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BirimFiyatlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Departmanlar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departmanlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EpostaSablonlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Konu = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IcerikHtml = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IcerikText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kategori = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpostaSablonlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FotografGalerisi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Sira = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotografGalerisi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GelirGruplari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GelirGruplari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GiderGruplari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiderGruplari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KasaBanka",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    BankaAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SubeAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HesapNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IBAN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KasaBanka", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KayipEsyalar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EsyaAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BulunanYer = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    BulunanTarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SahipAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SahipIletisim = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KayipEsyalar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MobilBildirimSablonlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Icerik = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Kategori = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobilBildirimSablonlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Olaylar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    OlayTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    Konum = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Olaylar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Olaylar_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OrtakAlanlar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Konum = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrtakAlanlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personeller",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personeller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteTemalari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrimaryColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SecondaryColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AccentColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FaviconUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FontFamily = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteTemalari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SmsSablonlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Icerik = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Kategori = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmsSablonlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TalepTipleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TalepTipleri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teklifler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TedarikciAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TeklifTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GecerlilikTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    Notlar = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teklifler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TelefonRehberi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Unvan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Dahili = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Departman = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TelefonRehberi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tesisler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Kapasite = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tesisler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Toplantilar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Gundem = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    ToplamtiTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Konum = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    Katilimcilar = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Kararlar = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Toplantilar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YapilacakIsler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AtananKisi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Oncelik = table.Column<int>(type: "int", nullable: false),
                    TamamlanmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YapilacakIsler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZiyaretciGirisCikislar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GelensAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GeldigiKisi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ZiyaretAmaci = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GirisSaati = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CikisSaati = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Plaka = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZiyaretciGirisCikislar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZiyaretciGirisCikislar_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DaireSayaclar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnaSayacId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    SeriNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Marka = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TakimTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DaireSayaclar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DaireSayaclar_AnaSayaclar_AnaSayacId",
                        column: x => x.AnaSayacId,
                        principalTable: "AnaSayaclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DaireSayaclar_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GelirTanimlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GelirGrubuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GelirTanimlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GelirTanimlari_GelirGruplari_GelirGrubuId",
                        column: x => x.GelirGrubuId,
                        principalTable: "GelirGruplari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "GiderTanimlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GiderGrubuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiderTanimlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GiderTanimlari_GiderGruplari_GiderGrubuId",
                        column: x => x.GiderGrubuId,
                        principalTable: "GiderGruplari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BankaHareketleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KasaBankaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ReferansNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    EslestirmeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankaHareketleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankaHareketleri_KasaBanka_KasaBankaId",
                        column: x => x.KasaBankaId,
                        principalTable: "KasaBanka",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OtomatikBildirimler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OlayTipi = table.Column<int>(type: "int", nullable: false),
                    EpostaAktif = table.Column<bool>(type: "bit", nullable: false),
                    SmsAktif = table.Column<bool>(type: "bit", nullable: false),
                    MobilAktif = table.Column<bool>(type: "bit", nullable: false),
                    EpostaSablonuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SmsSablonuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MobilSablonuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtomatikBildirimler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtomatikBildirimler_EpostaSablonlari_EpostaSablonuId",
                        column: x => x.EpostaSablonuId,
                        principalTable: "EpostaSablonlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OtomatikBildirimler_MobilBildirimSablonlari_MobilSablonuId",
                        column: x => x.MobilSablonuId,
                        principalTable: "MobilBildirimSablonlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OtomatikBildirimler_SmsSablonlari_SmsSablonuId",
                        column: x => x.SmsSablonuId,
                        principalTable: "SmsSablonlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "IsEmirleri",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    TalepTipiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrtakAlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Oncelik = table.Column<int>(type: "int", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    AtananKisiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AtananKisiAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IslemBaslangic = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IslemBitis = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notlar = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IsEmirleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IsEmirleri_Departmanlar_DepartmanId",
                        column: x => x.DepartmanId,
                        principalTable: "Departmanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_IsEmirleri_OrtakAlanlar_OrtakAlanId",
                        column: x => x.OrtakAlanId,
                        principalTable: "OrtakAlanlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_IsEmirleri_TalepTipleri_TalepTipiId",
                        column: x => x.TalepTipiId,
                        principalTable: "TalepTipleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_IsEmirleri_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Rezervasyonlar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TesisId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Durum = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rezervasyonlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rezervasyonlar_Tesisler_TesisId",
                        column: x => x.TesisId,
                        principalTable: "Tesisler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SayacOkumalar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnaSayacId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DaireSayacId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OkumaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OncekiEndeks = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    SonEndeks = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SayacOkumalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SayacOkumalar_AnaSayaclar_AnaSayacId",
                        column: x => x.AnaSayacId,
                        principalTable: "AnaSayaclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SayacOkumalar_DaireSayaclar_DaireSayacId",
                        column: x => x.DaireSayacId,
                        principalTable: "DaireSayaclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BorcMakbuzlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvrakNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Donem = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SonOdemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BorcluAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GelirTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    GecikmeTutari = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    OdenenTutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorcMakbuzlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BorcMakbuzlari_GelirTanimlari_GelirTanimiId",
                        column: x => x.GelirTanimiId,
                        principalTable: "GelirTanimlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BorcMakbuzlari_Units_UnitId",
                        column: x => x.UnitId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Faturalar",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tip = table.Column<int>(type: "int", nullable: false),
                    EvrakNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FaturaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CariAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    GelirTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GiderTanimiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ToplamTutar = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SonOdemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OdemeDurumu = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faturalar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Faturalar_GelirTanimlari_GelirTanimiId",
                        column: x => x.GelirTanimiId,
                        principalTable: "GelirTanimlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Faturalar_GiderTanimlari_GiderTanimiId",
                        column: x => x.GiderTanimiId,
                        principalTable: "GiderTanimlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TahsilatMakbuzlari",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvrakNo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BorcluAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    KasaBankaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BorcMakbuzuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OdemeTutari = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    OdemeTipi = table.Column<int>(type: "int", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TahsilatMakbuzlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TahsilatMakbuzlari_BorcMakbuzlari_BorcMakbuzuId",
                        column: x => x.BorcMakbuzuId,
                        principalTable: "BorcMakbuzlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TahsilatMakbuzlari_KasaBanka_KasaBankaId",
                        column: x => x.KasaBankaId,
                        principalTable: "KasaBanka",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AjandaEtkinlikleri_SiteId_BaslangicTarihi",
                table: "AjandaEtkinlikleri",
                columns: new[] { "SiteId", "BaslangicTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_AnaSayaclar_SiteId_Tip",
                table: "AnaSayaclar",
                columns: new[] { "SiteId", "Tip" });

            migrationBuilder.CreateIndex(
                name: "IX_AnaSayfaAyarlari_SiteId",
                table: "AnaSayfaAyarlari",
                column: "SiteId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Anketler_SiteId_Durum",
                table: "Anketler",
                columns: new[] { "SiteId", "Durum" });

            migrationBuilder.CreateIndex(
                name: "IX_AracGirisCikislar_Plaka",
                table: "AracGirisCikislar",
                column: "Plaka");

            migrationBuilder.CreateIndex(
                name: "IX_AracGirisCikislar_SiteId_GirisSaati",
                table: "AracGirisCikislar",
                columns: new[] { "SiteId", "GirisSaati" });

            migrationBuilder.CreateIndex(
                name: "IX_AracGirisCikislar_UnitId",
                table: "AracGirisCikislar",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_BankaHareketleri_KasaBankaId",
                table: "BankaHareketleri",
                column: "KasaBankaId");

            migrationBuilder.CreateIndex(
                name: "IX_BankaHareketleri_SiteId_KasaBankaId_Tarih",
                table: "BankaHareketleri",
                columns: new[] { "SiteId", "KasaBankaId", "Tarih" });

            migrationBuilder.CreateIndex(
                name: "IX_BirimFiyatlar_SiteId_Tip_BaslangicTarihi",
                table: "BirimFiyatlar",
                columns: new[] { "SiteId", "Tip", "BaslangicTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_BorcMakbuzlari_GelirTanimiId",
                table: "BorcMakbuzlari",
                column: "GelirTanimiId");

            migrationBuilder.CreateIndex(
                name: "IX_BorcMakbuzlari_SiteId_EvrakNo",
                table: "BorcMakbuzlari",
                columns: new[] { "SiteId", "EvrakNo" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_BorcMakbuzlari_UnitId",
                table: "BorcMakbuzlari",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_DaireSayaclar_AnaSayacId",
                table: "DaireSayaclar",
                column: "AnaSayacId");

            migrationBuilder.CreateIndex(
                name: "IX_DaireSayaclar_SiteId_UnitId_Tip",
                table: "DaireSayaclar",
                columns: new[] { "SiteId", "UnitId", "Tip" });

            migrationBuilder.CreateIndex(
                name: "IX_DaireSayaclar_UnitId",
                table: "DaireSayaclar",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Departmanlar_SiteId_Ad",
                table: "Departmanlar",
                columns: new[] { "SiteId", "Ad" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_EpostaSablonlari_SiteId_Ad",
                table: "EpostaSablonlari",
                columns: new[] { "SiteId", "Ad" });

            migrationBuilder.CreateIndex(
                name: "IX_Faturalar_GelirTanimiId",
                table: "Faturalar",
                column: "GelirTanimiId");

            migrationBuilder.CreateIndex(
                name: "IX_Faturalar_GiderTanimiId",
                table: "Faturalar",
                column: "GiderTanimiId");

            migrationBuilder.CreateIndex(
                name: "IX_Faturalar_SiteId_Tip_EvrakNo",
                table: "Faturalar",
                columns: new[] { "SiteId", "Tip", "EvrakNo" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_FotografGalerisi_SiteId_Sira",
                table: "FotografGalerisi",
                columns: new[] { "SiteId", "Sira" });

            migrationBuilder.CreateIndex(
                name: "IX_GelirTanimlari_GelirGrubuId",
                table: "GelirTanimlari",
                column: "GelirGrubuId");

            migrationBuilder.CreateIndex(
                name: "IX_GiderTanimlari_GiderGrubuId",
                table: "GiderTanimlari",
                column: "GiderGrubuId");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_DepartmanId",
                table: "IsEmirleri",
                column: "DepartmanId");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_OrtakAlanId",
                table: "IsEmirleri",
                column: "OrtakAlanId");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_SiteId_CreatedAt",
                table: "IsEmirleri",
                columns: new[] { "SiteId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_SiteId_Durum",
                table: "IsEmirleri",
                columns: new[] { "SiteId", "Durum" });

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_TalepTipiId",
                table: "IsEmirleri",
                column: "TalepTipiId");

            migrationBuilder.CreateIndex(
                name: "IX_IsEmirleri_UnitId",
                table: "IsEmirleri",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_KayipEsyalar_SiteId_BulunanTarih",
                table: "KayipEsyalar",
                columns: new[] { "SiteId", "BulunanTarih" });

            migrationBuilder.CreateIndex(
                name: "IX_KayipEsyalar_SiteId_Durum",
                table: "KayipEsyalar",
                columns: new[] { "SiteId", "Durum" });

            migrationBuilder.CreateIndex(
                name: "IX_MobilBildirimSablonlari_SiteId_Ad",
                table: "MobilBildirimSablonlari",
                columns: new[] { "SiteId", "Ad" });

            migrationBuilder.CreateIndex(
                name: "IX_Olaylar_SiteId_Durum",
                table: "Olaylar",
                columns: new[] { "SiteId", "Durum" });

            migrationBuilder.CreateIndex(
                name: "IX_Olaylar_SiteId_OlayTarihi",
                table: "Olaylar",
                columns: new[] { "SiteId", "OlayTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_Olaylar_UnitId",
                table: "Olaylar",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OtomatikBildirimler_EpostaSablonuId",
                table: "OtomatikBildirimler",
                column: "EpostaSablonuId");

            migrationBuilder.CreateIndex(
                name: "IX_OtomatikBildirimler_MobilSablonuId",
                table: "OtomatikBildirimler",
                column: "MobilSablonuId");

            migrationBuilder.CreateIndex(
                name: "IX_OtomatikBildirimler_SiteId_OlayTipi",
                table: "OtomatikBildirimler",
                columns: new[] { "SiteId", "OlayTipi" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_OtomatikBildirimler_SmsSablonuId",
                table: "OtomatikBildirimler",
                column: "SmsSablonuId");

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_SiteId_Name",
                table: "Personeller",
                columns: new[] { "SiteId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Rezervasyonlar_SiteId_StartDate",
                table: "Rezervasyonlar",
                columns: new[] { "SiteId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Rezervasyonlar_SiteId_TesisId_StartDate",
                table: "Rezervasyonlar",
                columns: new[] { "SiteId", "TesisId", "StartDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Rezervasyonlar_TesisId",
                table: "Rezervasyonlar",
                column: "TesisId");

            migrationBuilder.CreateIndex(
                name: "IX_SayacOkumalar_AnaSayacId",
                table: "SayacOkumalar",
                column: "AnaSayacId");

            migrationBuilder.CreateIndex(
                name: "IX_SayacOkumalar_DaireSayacId",
                table: "SayacOkumalar",
                column: "DaireSayacId");

            migrationBuilder.CreateIndex(
                name: "IX_SayacOkumalar_SiteId_OkumaTarihi",
                table: "SayacOkumalar",
                columns: new[] { "SiteId", "OkumaTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_SiteTemalari_SiteId",
                table: "SiteTemalari",
                column: "SiteId",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_SmsSablonlari_SiteId_Ad",
                table: "SmsSablonlari",
                columns: new[] { "SiteId", "Ad" });

            migrationBuilder.CreateIndex(
                name: "IX_TahsilatMakbuzlari_BorcMakbuzuId",
                table: "TahsilatMakbuzlari",
                column: "BorcMakbuzuId");

            migrationBuilder.CreateIndex(
                name: "IX_TahsilatMakbuzlari_KasaBankaId",
                table: "TahsilatMakbuzlari",
                column: "KasaBankaId");

            migrationBuilder.CreateIndex(
                name: "IX_TahsilatMakbuzlari_SiteId_EvrakNo",
                table: "TahsilatMakbuzlari",
                columns: new[] { "SiteId", "EvrakNo" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Teklifler_SiteId_Durum",
                table: "Teklifler",
                columns: new[] { "SiteId", "Durum" });

            migrationBuilder.CreateIndex(
                name: "IX_Teklifler_SiteId_TeklifTarihi",
                table: "Teklifler",
                columns: new[] { "SiteId", "TeklifTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_TelefonRehberi_SiteId_Ad",
                table: "TelefonRehberi",
                columns: new[] { "SiteId", "Ad" });

            migrationBuilder.CreateIndex(
                name: "IX_Toplantilar_SiteId_Durum",
                table: "Toplantilar",
                columns: new[] { "SiteId", "Durum" });

            migrationBuilder.CreateIndex(
                name: "IX_Toplantilar_SiteId_ToplamtiTarihi",
                table: "Toplantilar",
                columns: new[] { "SiteId", "ToplamtiTarihi" });

            migrationBuilder.CreateIndex(
                name: "IX_YapilacakIsler_SiteId_Durum",
                table: "YapilacakIsler",
                columns: new[] { "SiteId", "Durum" });

            migrationBuilder.CreateIndex(
                name: "IX_ZiyaretciGirisCikislar_SiteId_GirisSaati",
                table: "ZiyaretciGirisCikislar",
                columns: new[] { "SiteId", "GirisSaati" });

            migrationBuilder.CreateIndex(
                name: "IX_ZiyaretciGirisCikislar_UnitId",
                table: "ZiyaretciGirisCikislar",
                column: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AjandaEtkinlikleri");

            migrationBuilder.DropTable(
                name: "AnaSayfaAyarlari");

            migrationBuilder.DropTable(
                name: "Anketler");

            migrationBuilder.DropTable(
                name: "AracGirisCikislar");

            migrationBuilder.DropTable(
                name: "BankaHareketleri");

            migrationBuilder.DropTable(
                name: "BirimFiyatlar");

            migrationBuilder.DropTable(
                name: "Faturalar");

            migrationBuilder.DropTable(
                name: "FotografGalerisi");

            migrationBuilder.DropTable(
                name: "IsEmirleri");

            migrationBuilder.DropTable(
                name: "KayipEsyalar");

            migrationBuilder.DropTable(
                name: "Olaylar");

            migrationBuilder.DropTable(
                name: "OtomatikBildirimler");

            migrationBuilder.DropTable(
                name: "Personeller");

            migrationBuilder.DropTable(
                name: "Rezervasyonlar");

            migrationBuilder.DropTable(
                name: "SayacOkumalar");

            migrationBuilder.DropTable(
                name: "SiteTemalari");

            migrationBuilder.DropTable(
                name: "TahsilatMakbuzlari");

            migrationBuilder.DropTable(
                name: "Teklifler");

            migrationBuilder.DropTable(
                name: "TelefonRehberi");

            migrationBuilder.DropTable(
                name: "Toplantilar");

            migrationBuilder.DropTable(
                name: "YapilacakIsler");

            migrationBuilder.DropTable(
                name: "ZiyaretciGirisCikislar");

            migrationBuilder.DropTable(
                name: "GiderTanimlari");

            migrationBuilder.DropTable(
                name: "Departmanlar");

            migrationBuilder.DropTable(
                name: "OrtakAlanlar");

            migrationBuilder.DropTable(
                name: "TalepTipleri");

            migrationBuilder.DropTable(
                name: "EpostaSablonlari");

            migrationBuilder.DropTable(
                name: "MobilBildirimSablonlari");

            migrationBuilder.DropTable(
                name: "SmsSablonlari");

            migrationBuilder.DropTable(
                name: "Tesisler");

            migrationBuilder.DropTable(
                name: "DaireSayaclar");

            migrationBuilder.DropTable(
                name: "BorcMakbuzlari");

            migrationBuilder.DropTable(
                name: "KasaBanka");

            migrationBuilder.DropTable(
                name: "GiderGruplari");

            migrationBuilder.DropTable(
                name: "AnaSayaclar");

            migrationBuilder.DropTable(
                name: "GelirTanimlari");

            migrationBuilder.DropTable(
                name: "GelirGruplari");
        }
    }
}
