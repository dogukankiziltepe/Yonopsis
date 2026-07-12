# Yetki Grupları

**Menü Yolu:** Tanımlar > Yetki Grupları
**URL:** https://www.apsiyon.com/manager/securityGroups.aspx

## Genel Bakış
Yönetici paneline erişimde kullanılan yetki gruplarının tanımlandığı sayfadır. 8 kayıt.

## İçerik / Özellikler
- **Filtreler:** Alfabetik harf filtresi (Tümü, #, A-Z)
- **Sekme:** Hepsi
- **Tablo Sütunları:**
  - Adı
  - Açıklama
  - Genel Yetki Tipi (Tam Yetki / Sadece Okuma / Yetkisiz)
  - Aktif
- **Butonlar:**
  - `+` Yeni yetki grubu
  - Excel dosyası olarak kaydet (XLS)

**Mevcut Yetki Grupları:**

| Grup | Açıklama | Yetki Tipi |
|---|---|---|
| Admin | Tüm sayfalara ve işlemlere erişim | Sadece Okuma |
| GKS | Geçiş Kontrol Sistemleri ekranı için | Yetkisiz |
| Güvenlik Mobası | Tüm sayfaları görebilir, sadece okuma | Tam Yetki |
| Site Müdürü | — | Tam Yetki |
| Tegaplus | — | Tam Yetki |
| Yönetim | Parametreler hariç tüm sayfalar | Tam Yetki |
| Yönetim Kurulu | — | Tam Yetki |
| Yönetim Kurulu Üyesi | — | Sadece Okuma |

## Notlar
- "Admin" grubu Sadece Okuma olarak tanımlanmış (dikkat: açıklaması tam erişim diyor)
- GKS grubu Yetkisiz → sadece geçiş kontrol sistemi ara yüzüne erişim için
- Yetkililer (managers.aspx) bu gruplardan birine atanır
