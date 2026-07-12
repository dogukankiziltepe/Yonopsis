using Microsoft.EntityFrameworkCore;
using SiteYonetimi.Infrastructure.Entities.Shared;

namespace SiteYonetimi.Infrastructure.Data;

/// <summary>
/// DbMode = Shared olan sitelerin operasyonel verisi burada tutulur.
/// Tüm tablolarda SiteId kolonu ile tenant ayrımı yapılır.
/// </summary>
public class SharedTenantDbContext : DbContext
{
    public SharedTenantDbContext(DbContextOptions<SharedTenantDbContext> options) : base(options) { }

    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<UnitType> UnitTypes => Set<UnitType>();
    public DbSet<SupportRequest> SupportRequests => Set<SupportRequest>();
    public DbSet<SupportRequestComment> SupportRequestComments => Set<SupportRequestComment>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<AccessCard> AccessCards => Set<AccessCard>();
    public DbSet<PersonUnitHistory> PersonUnitHistories => Set<PersonUnitHistory>();
    public DbSet<EmailLog> EmailLogs => Set<EmailLog>();
    public DbSet<SmsLog> SmsLogs => Set<SmsLog>();
    public DbSet<WhatsappLog> WhatsappLogs => Set<WhatsappLog>();
    public DbSet<MobilBildirimLog> MobilBildirimLogs => Set<MobilBildirimLog>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<AidatKalemi> AidatKalemleri => Set<AidatKalemi>();
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<UploadedFile> UploadedFiles => Set<UploadedFile>();

    // Tanımlar modülü
    public DbSet<GelirGrubu> GelirGruplari => Set<GelirGrubu>();
    public DbSet<GiderGrubu> GiderGruplari => Set<GiderGrubu>();
    public DbSet<GelirTanimi> GelirTanimlari => Set<GelirTanimi>();
    public DbSet<GiderTanimi> GiderTanimlari => Set<GiderTanimi>();
    public DbSet<KasaBanka> KasaBanka => Set<KasaBanka>();
    public DbSet<Tesis> Tesisler => Set<Tesis>();

    // Finans modülü
    public DbSet<BorcMakbuzu> BorcMakbuzlari => Set<BorcMakbuzu>();
    public DbSet<TahsilatMakbuzu> TahsilatMakbuzlari => Set<TahsilatMakbuzu>();
    public DbSet<Fatura> Faturalar => Set<Fatura>();
    public DbSet<BankaHareketi> BankaHareketleri => Set<BankaHareketi>();

    // Güvenlik modülü
    public DbSet<ZiyaretciGirisCikis> ZiyaretciGirisCikislar => Set<ZiyaretciGirisCikis>();
    public DbSet<AracGirisCikis> AracGirisCikislar => Set<AracGirisCikis>();
    public DbSet<Olay> Olaylar => Set<Olay>();
    public DbSet<KayipEsya> KayipEsyalar => Set<KayipEsya>();

    // Teknik modülü
    public DbSet<Departman> Departmanlar => Set<Departman>();
    public DbSet<OrtakAlan> OrtakAlanlar => Set<OrtakAlan>();
    public DbSet<TalepTipi> TalepTipleri => Set<TalepTipi>();
    public DbSet<IsEmri> IsEmirleri => Set<IsEmri>();

    // Sayaç modülü
    public DbSet<AnaSayac> AnaSayaclar => Set<AnaSayac>();
    public DbSet<DaireSayac> DaireSayaclar => Set<DaireSayac>();
    public DbSet<SayacOkuma> SayacOkumalar => Set<SayacOkuma>();
    public DbSet<BirimFiyat> BirimFiyatlar => Set<BirimFiyat>();

    // İletişim Kanalları modülü
    public DbSet<EpostaSablonu> EpostaSablonlari => Set<EpostaSablonu>();
    public DbSet<SmsSablonu> SmsSablonlari => Set<SmsSablonu>();
    public DbSet<MobilBildirimSablonu> MobilBildirimSablonlari => Set<MobilBildirimSablonu>();
    public DbSet<OtomatikBildirim> OtomatikBildirimler => Set<OtomatikBildirim>();
    public DbSet<TelefonRehberi> TelefonRehberi => Set<TelefonRehberi>();

    // Web Sitesi modülü
    public DbSet<FotografGalerisi> FotografGalerisi => Set<FotografGalerisi>();
    public DbSet<Anket> Anketler => Set<Anket>();
    public DbSet<AnaSayfaAyar> AnaSayfaAyarlari => Set<AnaSayfaAyar>();
    public DbSet<SiteTemasi> SiteTemalari => Set<SiteTemasi>();

    // Rezervasyon & Personel
    public DbSet<Rezervasyon> Rezervasyonlar => Set<Rezervasyon>();
    public DbSet<Personel> Personeller => Set<Personel>();

    // Site yönetim modülü
    public DbSet<AjandaEtkinlik> AjandaEtkinlikleri => Set<AjandaEtkinlik>();
    public DbSet<Toplanti> Toplantilar => Set<Toplanti>();
    public DbSet<Teklif> Teklifler => Set<Teklif>();
    public DbSet<YapilacakIs> YapilacakIsler => Set<YapilacakIs>();

    // Muhasebe modülü
    public DbSet<HesapPlani> HesapPlani => Set<HesapPlani>();
    public DbSet<MuhasebeDonem> MuhasebeDonemler => Set<MuhasebeDonem>();
    public DbSet<MuhasebeFisi> MuhasebeFisleri => Set<MuhasebeFisi>();
    public DbSet<MuhasebeFisiDetay> MuhasebeFisiDetaylari => Set<MuhasebeFisiDetay>();
    public DbSet<MuhasebeParametre> MuhasebeParametreler => Set<MuhasebeParametre>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Building>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasMany(x => x.Units).WithOne(x => x.Building).HasForeignKey(x => x.BuildingId);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Unit>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.BuildingId).IsRequired();
            e.Property(x => x.DoorNumber).HasMaxLength(20).IsRequired();
            e.Property(x => x.Code).HasMaxLength(20);
            e.Property(x => x.Floor).HasMaxLength(50);
            e.Property(x => x.GrossArea).HasPrecision(18, 2);
            e.Property(x => x.NetArea).HasPrecision(18, 2);
            e.Property(x => x.LandShare).HasPrecision(18, 2);
            e.Property(x => x.MonthlyFee).HasPrecision(18, 2);
            e.Property(x => x.Internet).HasMaxLength(100);
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.Code2).HasMaxLength(20);
            e.Property(x => x.Code3).HasMaxLength(20);
            e.HasOne(x => x.UnitType).WithMany().HasForeignKey(x => x.UnitTypeId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<UnitType>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<SupportRequest>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.UserId).IsRequired();
            e.Property(x => x.Subject).HasMaxLength(200).IsRequired();
            e.Property(x => x.Description).HasMaxLength(2000);
            e.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(x => x.Comments).WithOne(x => x.Request).HasForeignKey(x => x.RequestId).OnDelete(DeleteBehavior.Cascade);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<SupportRequestComment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Content).HasMaxLength(2000).IsRequired();
            e.Property(x => x.AuthorName).HasMaxLength(100);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Vehicle>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Plate).HasMaxLength(20).IsRequired();
            e.Property(x => x.Brand).HasMaxLength(50);
            e.Property(x => x.Model).HasMaxLength(50);
            e.Property(x => x.Color).HasMaxLength(30);
            e.Property(x => x.HgsNo).HasMaxLength(30);
            e.HasOne(x => x.Unit)
                .WithMany(u => u.Vehicles)
                .HasForeignKey(x => x.UnitId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.SiteId, x.Plate })
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
            e.HasIndex(x => x.UnitId);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<AccessCard>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.UserId).IsRequired();
            e.Property(x => x.CardNumber).HasMaxLength(50).IsRequired();
            e.Property(x => x.Notes).HasMaxLength(500);
            e.HasOne(x => x.Unit)
                .WithMany()
                .HasForeignKey(x => x.UnitId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => x.UnitId);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<PersonUnitHistory>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.PersonUserId).IsRequired();
            e.Property(x => x.Role).HasConversion<int>();
            e.Property(x => x.ContactPerson).HasMaxLength(200);
            e.Property(x => x.Notes).HasMaxLength(500);
            e.Property(x => x.BankPaymentCode).HasMaxLength(50);
            e.Property(x => x.SharePercentage).HasPrecision(5, 2);
            e.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.SiteId, x.UnitId });
            e.HasIndex(x => new { x.SiteId, x.PersonUserId });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<EmailLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.UserId).IsRequired();
            e.Property(x => x.RecipientEmail).HasMaxLength(256).IsRequired();
            e.Property(x => x.Subject).HasMaxLength(300).IsRequired();
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.HasIndex(x => new { x.SiteId, x.UserId });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<SmsLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.UserId).IsRequired();
            e.Property(x => x.PhoneNumber).HasMaxLength(50).IsRequired();
            e.Property(x => x.Message).HasMaxLength(500).IsRequired();
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.HasIndex(x => new { x.SiteId, x.UserId });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<WhatsappLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.UserId).IsRequired();
            e.Property(x => x.PhoneNumber).HasMaxLength(50).IsRequired();
            e.Property(x => x.Message).HasMaxLength(500).IsRequired();
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.HasIndex(x => new { x.SiteId, x.UserId });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<MobilBildirimLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.UserId).IsRequired();
            e.Property(x => x.Message).HasMaxLength(500).IsRequired();
            e.Property(x => x.Status).HasMaxLength(50).IsRequired();
            e.HasIndex(x => new { x.SiteId, x.UserId });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Payment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Amount).HasPrecision(18, 2).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<AidatKalemi>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Announcement>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Title).HasMaxLength(200).IsRequired();
            e.Property(x => x.Content).HasMaxLength(5000).IsRequired();
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<UploadedFile>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.UploadedByUserId).IsRequired();
            e.Property(x => x.OriginalFileName).HasMaxLength(260).IsRequired();
            e.Property(x => x.StoredFileName).HasMaxLength(260).IsRequired();
            e.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
            e.Property(x => x.FileSize).IsRequired();
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<HesapPlani>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.HesapKodu).HasMaxLength(50).IsRequired();
            e.Property(x => x.HesapAdi).HasMaxLength(200).IsRequired();
            e.Property(x => x.HesapTipi).HasConversion<int>();
            e.Property(x => x.HesapKategorisi).HasConversion<int>();
            e.Property(x => x.NormalBakiye).HasConversion<int>();
            e.Property(x => x.CariTuru).HasConversion<int>();
            e.Property(x => x.Aciklama).HasMaxLength(500);
            // Hesap kodu tenant içinde unique (silinmemiş kayıtlar için)
            e.HasIndex(x => new { x.SiteId, x.HesapKodu }).IsUnique().HasFilter("[IsDeleted] = 0");
            e.HasIndex(x => new { x.SiteId, x.ParentId });
            e.HasIndex(x => new { x.SiteId, x.PersonId });
            e.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<MuhasebeDonem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Ad).HasMaxLength(100).IsRequired();
            e.Property(x => x.Durum).HasConversion<int>();
            e.HasIndex(x => new { x.SiteId, x.Yil }).IsUnique().HasFilter("[IsDeleted] = 0");
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<MuhasebeFisi>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.DonemId).IsRequired();
            e.Property(x => x.FisNo).HasMaxLength(50).IsRequired();
            e.Property(x => x.Aciklama).HasMaxLength(500);
            e.Property(x => x.FisTuru).HasConversion<int>();
            e.Property(x => x.Durum).HasConversion<int>();
            e.Property(x => x.ToplamBorc).HasPrecision(18, 2);
            e.Property(x => x.ToplamAlacak).HasPrecision(18, 2);
            e.HasIndex(x => new { x.SiteId, x.FisNo }).IsUnique().HasFilter("[IsDeleted] = 0");
            e.HasIndex(x => new { x.SiteId, x.DonemId, x.YevmiyeNo });
            e.HasIndex(x => new { x.SiteId, x.FisTarihi });
            e.HasOne(x => x.Donem).WithMany().HasForeignKey(x => x.DonemId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(x => x.Detaylar).WithOne(x => x.Fis).HasForeignKey(x => x.FisId).OnDelete(DeleteBehavior.Cascade);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<MuhasebeFisiDetay>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.FisId).IsRequired();
            e.Property(x => x.HesapId).IsRequired();
            e.Property(x => x.HesapKodu).HasMaxLength(50).IsRequired();
            e.Property(x => x.BorcTutar).HasPrecision(18, 2);
            e.Property(x => x.AlacakTutar).HasPrecision(18, 2);
            e.Property(x => x.Aciklama).HasMaxLength(500);
            e.Property(x => x.BelgeNo).HasMaxLength(100);
            e.HasIndex(x => new { x.SiteId, x.HesapId });
            e.HasIndex(x => x.FisId);
        });

        modelBuilder.Entity<MuhasebeParametre>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.AlicilarAnaHesapKodu).HasMaxLength(50);
            e.Property(x => x.SaticilarAnaHesapKodu).HasMaxLength(50);
            e.Property(x => x.GiderAnaHesapKodu).HasMaxLength(50);
            e.Property(x => x.CariKodSablonu).HasMaxLength(100);
            e.Property(x => x.FisNoSablonu).HasMaxLength(100);
            e.Property(x => x.ParaBirimi).HasMaxLength(10);
            e.Property(x => x.KdvOrani).HasPrecision(5, 2);
            e.HasIndex(x => x.SiteId).IsUnique().HasFilter("[IsDeleted] = 0");
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<GelirGrubu>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasMany(x => x.GelirTanimlari).WithOne(x => x.GelirGrubu).HasForeignKey(x => x.GelirGrubuId).OnDelete(DeleteBehavior.SetNull);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<GiderGrubu>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasMany(x => x.GiderTanimlari).WithOne(x => x.GiderGrubu).HasForeignKey(x => x.GiderGrubuId).OnDelete(DeleteBehavior.SetNull);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<GelirTanimi>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<GiderTanimi>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<KasaBanka>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Tip).HasConversion<int>();
            e.Property(x => x.BankaAdi).HasMaxLength(100);
            e.Property(x => x.SubeAdi).HasMaxLength(100);
            e.Property(x => x.HesapNo).HasMaxLength(50);
            e.Property(x => x.IBAN).HasMaxLength(50);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Tesis>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Name).HasMaxLength(100).IsRequired();
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<BorcMakbuzu>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.EvrakNo).HasMaxLength(30).IsRequired();
            e.Property(x => x.Donem).HasMaxLength(10);
            e.Property(x => x.BorcluAdi).HasMaxLength(200);
            e.Property(x => x.Tutar).HasPrecision(18, 2).IsRequired();
            e.Property(x => x.GecikmeTutari).HasPrecision(18, 2).HasDefaultValue(0m);
            e.Property(x => x.OdenenTutar).HasPrecision(18, 2).HasDefaultValue(0m);
            e.Property(x => x.Aciklama).HasMaxLength(500);
            e.Ignore(x => x.KalanTutar);
            e.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.GelirTanimi).WithMany().HasForeignKey(x => x.GelirTanimiId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => new { x.SiteId, x.EvrakNo }).IsUnique().HasFilter("[IsDeleted] = 0");
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<TahsilatMakbuzu>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.EvrakNo).HasMaxLength(30).IsRequired();
            e.Property(x => x.BorcluAdi).HasMaxLength(200);
            e.Property(x => x.OdemeTutari).HasPrecision(18, 2).IsRequired();
            e.Property(x => x.OdemeTipi).HasConversion<int>();
            e.Property(x => x.Aciklama).HasMaxLength(500);
            e.HasOne(x => x.KasaBanka).WithMany().HasForeignKey(x => x.KasaBankaId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.BorcMakbuzu).WithMany().HasForeignKey(x => x.BorcMakbuzuId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => new { x.SiteId, x.EvrakNo }).IsUnique().HasFilter("[IsDeleted] = 0");
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Fatura>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Tip).HasConversion<int>();
            e.Property(x => x.EvrakNo).HasMaxLength(30).IsRequired();
            e.Property(x => x.CariAdi).HasMaxLength(200).IsRequired();
            e.Property(x => x.ToplamTutar).HasPrecision(18, 2).IsRequired();
            e.Property(x => x.Aciklama).HasMaxLength(500);
            e.Property(x => x.OdemeDurumu).HasConversion<int>();
            e.HasOne(x => x.GelirTanimi).WithMany().HasForeignKey(x => x.GelirTanimiId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.GiderTanimi).WithMany().HasForeignKey(x => x.GiderTanimiId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => new { x.SiteId, x.Tip, x.EvrakNo }).IsUnique().HasFilter("[IsDeleted] = 0");
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<BankaHareketi>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.KasaBankaId).IsRequired();
            e.Property(x => x.Aciklama).HasMaxLength(500).IsRequired();
            e.Property(x => x.ReferansNo).HasMaxLength(100);
            e.Property(x => x.Tutar).HasPrecision(18, 2).IsRequired();
            e.Property(x => x.Durum).HasConversion<int>();
            e.HasOne(x => x.KasaBanka).WithMany().HasForeignKey(x => x.KasaBankaId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.SiteId, x.KasaBankaId, x.Tarih });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<ZiyaretciGirisCikis>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.GelensAdi).HasMaxLength(200).IsRequired();
            e.Property(x => x.GeldigiKisi).HasMaxLength(200);
            e.Property(x => x.ZiyaretAmaci).HasMaxLength(500);
            e.Property(x => x.Plaka).HasMaxLength(20);
            e.Property(x => x.Aciklama).HasMaxLength(500);
            e.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => new { x.SiteId, x.GirisSaati });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<AracGirisCikis>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Plaka).HasMaxLength(20).IsRequired();
            e.Property(x => x.SuruculAdi).HasMaxLength(200);
            e.Property(x => x.AracTipi).HasConversion<int>();
            e.Property(x => x.Aciklama).HasMaxLength(500);
            e.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => new { x.SiteId, x.GirisSaati });
            e.HasIndex(x => x.Plaka);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Olay>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Baslik).HasMaxLength(300).IsRequired();
            e.Property(x => x.Aciklama).HasMaxLength(3000).IsRequired();
            e.Property(x => x.Tip).HasConversion<int>();
            e.Property(x => x.Durum).HasConversion<int>();
            e.Property(x => x.Konum).HasMaxLength(200);
            e.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => new { x.SiteId, x.OlayTarihi });
            e.HasIndex(x => new { x.SiteId, x.Durum });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<KayipEsya>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.EsyaAdi).HasMaxLength(200).IsRequired();
            e.Property(x => x.Aciklama).HasMaxLength(1000);
            e.Property(x => x.BulunanYer).HasMaxLength(300);
            e.Property(x => x.SahipAdi).HasMaxLength(200);
            e.Property(x => x.SahipIletisim).HasMaxLength(200);
            e.Property(x => x.Durum).HasConversion<int>();
            e.HasIndex(x => new { x.SiteId, x.BulunanTarih });
            e.HasIndex(x => new { x.SiteId, x.Durum });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        // ── Teknik ───────────────────────────────────────────────────────
        modelBuilder.Entity<Departman>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Ad).HasMaxLength(100).IsRequired();
            e.Property(x => x.Aciklama).HasMaxLength(500);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasIndex(x => new { x.SiteId, x.Ad }).IsUnique().HasFilter("[IsDeleted] = 0");
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<OrtakAlan>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Ad).HasMaxLength(100).IsRequired();
            e.Property(x => x.Aciklama).HasMaxLength(500);
            e.Property(x => x.Konum).HasMaxLength(200);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<TalepTipi>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Ad).HasMaxLength(100).IsRequired();
            e.Property(x => x.Aciklama).HasMaxLength(500);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<IsEmri>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Baslik).HasMaxLength(300).IsRequired();
            e.Property(x => x.Aciklama).HasMaxLength(3000);
            e.Property(x => x.Notlar).HasMaxLength(2000);
            e.Property(x => x.AtananKisiAdi).HasMaxLength(200);
            e.Property(x => x.Oncelik).HasConversion<int>();
            e.Property(x => x.Durum).HasConversion<int>();
            e.HasOne(x => x.TalepTipi).WithMany().HasForeignKey(x => x.TalepTipiId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.Departman).WithMany(d => d.IsEmirleri).HasForeignKey(x => x.DepartmanId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.OrtakAlan).WithMany().HasForeignKey(x => x.OrtakAlanId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => new { x.SiteId, x.Durum });
            e.HasIndex(x => new { x.SiteId, x.CreatedAt });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        // ── Sayaç ────────────────────────────────────────────────────────
        modelBuilder.Entity<AnaSayac>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Ad).HasMaxLength(100).IsRequired();
            e.Property(x => x.Tip).HasConversion<int>();
            e.Property(x => x.SeriNo).HasMaxLength(100);
            e.Property(x => x.Marka).HasMaxLength(100);
            e.Property(x => x.Aciklama).HasMaxLength(500);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasIndex(x => new { x.SiteId, x.Tip });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<DaireSayac>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.UnitId).IsRequired();
            e.Property(x => x.AnaSayacId).IsRequired();
            e.Property(x => x.Tip).HasConversion<int>();
            e.Property(x => x.SeriNo).HasMaxLength(100);
            e.Property(x => x.Marka).HasMaxLength(100);
            e.Property(x => x.Aciklama).HasMaxLength(500);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasOne(x => x.Unit).WithMany().HasForeignKey(x => x.UnitId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.AnaSayac).WithMany(a => a.DaireSayaclari).HasForeignKey(x => x.AnaSayacId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(x => new { x.SiteId, x.UnitId, x.Tip });
            e.HasQueryFilter(x => !x.IsDeleted);
        });
        modelBuilder.Entity<SayacOkuma>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.OncekiEndeks).HasPrecision(18, 4);
            e.Property(x => x.SonEndeks).HasPrecision(18, 4);
            e.Property(x => x.Aciklama).HasMaxLength(500);
            e.Ignore(x => x.Tuketim);
            e.HasOne(x => x.AnaSayac).WithMany(a => a.Okumalar).HasForeignKey(x => x.AnaSayacId).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.DaireSayac).WithMany(d => d.Okumalar).HasForeignKey(x => x.DaireSayacId).IsRequired(false).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => new { x.SiteId, x.OkumaTarihi });
            e.HasIndex(x => x.AnaSayacId);
            e.HasIndex(x => x.DaireSayacId);
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<BirimFiyat>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Tip).HasConversion<int>();
            e.Property(x => x.Fiyat).HasPrecision(18, 4).IsRequired();
            e.Property(x => x.Birim).HasMaxLength(20);
            e.Property(x => x.Aciklama).HasMaxLength(500);
            e.HasIndex(x => new { x.SiteId, x.Tip, x.BaslangicTarihi });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        // ── İletişim Kanalları ───────────────────────────────────────────
        modelBuilder.Entity<EpostaSablonu>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Ad).HasMaxLength(200).IsRequired();
            e.Property(x => x.Konu).HasMaxLength(300).IsRequired();
            e.Property(x => x.IcerikHtml).IsRequired();
            e.Property(x => x.Kategori).HasMaxLength(100);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasIndex(x => new { x.SiteId, x.Ad });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<SmsSablonu>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Ad).HasMaxLength(200).IsRequired();
            e.Property(x => x.Icerik).HasMaxLength(500).IsRequired();
            e.Property(x => x.Kategori).HasMaxLength(100);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasIndex(x => new { x.SiteId, x.Ad });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<MobilBildirimSablonu>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Ad).HasMaxLength(200).IsRequired();
            e.Property(x => x.Baslik).HasMaxLength(200).IsRequired();
            e.Property(x => x.Icerik).HasMaxLength(500).IsRequired();
            e.Property(x => x.Kategori).HasMaxLength(100);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasIndex(x => new { x.SiteId, x.Ad });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<OtomatikBildirim>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.OlayTipi).HasConversion<int>();
            e.HasOne(x => x.EpostaSablonu).WithMany().HasForeignKey(x => x.EpostaSablonuId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.SmsSablonu).WithMany().HasForeignKey(x => x.SmsSablonuId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.MobilSablonu).WithMany().HasForeignKey(x => x.MobilSablonuId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => new { x.SiteId, x.OlayTipi }).IsUnique().HasFilter("[IsDeleted] = 0");
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<TelefonRehberi>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Ad).HasMaxLength(200).IsRequired();
            e.Property(x => x.Unvan).HasMaxLength(100);
            e.Property(x => x.Telefon).HasMaxLength(50).IsRequired();
            e.Property(x => x.Dahili).HasMaxLength(20);
            e.Property(x => x.Email).HasMaxLength(200);
            e.Property(x => x.Departman).HasMaxLength(100);
            e.Property(x => x.Aciklama).HasMaxLength(500);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasIndex(x => new { x.SiteId, x.Ad });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        // ── Web Sitesi ───────────────────────────────────────────────────
        modelBuilder.Entity<FotografGalerisi>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Baslik).HasMaxLength(200).IsRequired();
            e.Property(x => x.Aciklama).HasMaxLength(500);
            e.Property(x => x.ImageUrl).HasMaxLength(500).IsRequired();
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasIndex(x => new { x.SiteId, x.Sira });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Anket>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Baslik).HasMaxLength(300).IsRequired();
            e.Property(x => x.Aciklama).HasMaxLength(1000);
            e.Property(x => x.Durum).HasConversion<int>();
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasIndex(x => new { x.SiteId, x.Durum });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<AnaSayfaAyar>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.SiteAdi).HasMaxLength(200);
            e.Property(x => x.Slogan).HasMaxLength(300);
            e.Property(x => x.KisaAciklama).HasMaxLength(500);
            e.Property(x => x.IletisimTelefon).HasMaxLength(50);
            e.Property(x => x.IletisimEmail).HasMaxLength(200);
            e.Property(x => x.Adres).HasMaxLength(500);
            e.Property(x => x.LogoUrl).HasMaxLength(500);
            e.Property(x => x.KapakFotoUrl).HasMaxLength(500);
            e.HasIndex(x => x.SiteId).IsUnique().HasFilter("[IsDeleted] = 0");
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<SiteTemasi>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.PrimaryColor).HasMaxLength(20);
            e.Property(x => x.SecondaryColor).HasMaxLength(20);
            e.Property(x => x.AccentColor).HasMaxLength(20);
            e.Property(x => x.LogoUrl).HasMaxLength(500);
            e.Property(x => x.FaviconUrl).HasMaxLength(500);
            e.Property(x => x.FontFamily).HasMaxLength(100);
            e.HasIndex(x => x.SiteId).IsUnique().HasFilter("[IsDeleted] = 0");
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        // ── Personel ─────────────────────────────────────────────────────
        modelBuilder.Entity<Personel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Title).HasMaxLength(100);
            e.Property(x => x.Phone).HasMaxLength(50);
            e.Property(x => x.Email).HasMaxLength(200);
            e.Property(x => x.Department).HasMaxLength(100);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasIndex(x => new { x.SiteId, x.Name });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        // ── Rezervasyon ─────────────────────────────────────────────────
        modelBuilder.Entity<Rezervasyon>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Notes).HasMaxLength(1000);
            e.Property(x => x.Durum).HasConversion<int>();
            e.HasOne(x => x.Tesis).WithMany().HasForeignKey(x => x.TesisId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => new { x.SiteId, x.StartDate });
            e.HasIndex(x => new { x.SiteId, x.TesisId, x.StartDate });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        // ── Site Yönetim ─────────────────────────────────────────────────
        modelBuilder.Entity<AjandaEtkinlik>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Baslik).HasMaxLength(300).IsRequired();
            e.Property(x => x.Aciklama).HasMaxLength(2000);
            e.Property(x => x.Konum).HasMaxLength(200);
            e.Property(x => x.Renk).HasMaxLength(20);
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasIndex(x => new { x.SiteId, x.BaslangicTarihi });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Toplanti>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Baslik).HasMaxLength(300).IsRequired();
            e.Property(x => x.Aciklama).HasMaxLength(2000);
            e.Property(x => x.Gundem).HasMaxLength(3000);
            e.Property(x => x.Konum).HasMaxLength(200);
            e.Property(x => x.Katilimcilar).HasMaxLength(1000);
            e.Property(x => x.Kararlar).HasMaxLength(3000);
            e.Property(x => x.Durum).HasConversion<int>();
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasIndex(x => new { x.SiteId, x.ToplamtiTarihi });
            e.HasIndex(x => new { x.SiteId, x.Durum });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<Teklif>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Baslik).HasMaxLength(300).IsRequired();
            e.Property(x => x.Aciklama).HasMaxLength(2000);
            e.Property(x => x.TedarikciAdi).HasMaxLength(200);
            e.Property(x => x.Tutar).HasPrecision(18, 2);
            e.Property(x => x.Notlar).HasMaxLength(1000);
            e.Property(x => x.Durum).HasConversion<int>();
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasIndex(x => new { x.SiteId, x.Durum });
            e.HasIndex(x => new { x.SiteId, x.TeklifTarihi });
            e.HasQueryFilter(x => !x.IsDeleted);
        });

        modelBuilder.Entity<YapilacakIs>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.SiteId).IsRequired();
            e.Property(x => x.Baslik).HasMaxLength(300).IsRequired();
            e.Property(x => x.Aciklama).HasMaxLength(2000);
            e.Property(x => x.AtananKisi).HasMaxLength(200);
            e.Property(x => x.Durum).HasConversion<int>();
            e.Property(x => x.Oncelik).HasConversion<int>();
            e.Property(x => x.IsActive).HasDefaultValue(true);
            e.HasIndex(x => new { x.SiteId, x.Durum });
            e.HasQueryFilter(x => !x.IsDeleted);
        });
    }
}
