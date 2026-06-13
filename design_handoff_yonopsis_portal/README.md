# Handoff: Yonopsis — Site/Konut Yönetim Portalı

## Genel Bakış
Yonopsis; bir konut sitesinin dijital yönetim arayüzüdür. Üç rol için tek tema/token sistemi üzerinde çalışır:
- **Kiracı (tenant)** ve **Daire Sahibi (owner)** → ferah, kart/özet odaklı bir konut sakini portalı.
- **Yönetim (management)** → yoğun, tablo/form ağırlıklı operasyon paneli.

Akış: **Login → rol seçimi → ilgili portal**. Rol arayüzü değiştirmez; sadece içerik yoğunluğu ve düzen rolü hissettirir.

---

## Bu Paketteki Dosyalar Hakkında
Bu klasördeki dosyalar **HTML ile üretilmiş tasarım referanslarıdır** — amaçlanan görünüm ve davranışı gösteren prototiplerdir, **birebir kopyalanacak production kodu değildir**. Görev: bu tasarımları hedef kod tabanının kendi ortamında **yeniden oluşturmaktır**.

Hedef kod tabanı (spec'e göre): **Next.js App Router + shadcn/ui + Tailwind**, route grupları `(owner)` / `(tenant)` / `management`. Prototip bilinçli olarak bu yapıya eşlenecek şekilde, shadcn token mantığıyla kuruldu. HTML'deki elle yazılmış sınıfları (`.btn`, `.badge`, `.card`, `Dropdown`, `.tbl` …) **mevcut shadcn/ui bileşenlerine** çevir; renkleri token map üzerinden ver, hiçbir yerde hardcoded renk bırakma.

> Prototip React'i (Babel ile, tarayıcıda) kullanır ve mock veriyle çalışır. TypeScript + server components + gerçek veri katmanı hedefte yeniden kurulmalıdır.

---

## Fidelity: **High-fidelity (hifi)**
Renkler, tipografi, boşluklar ve etkileşimler nihaidir. UI'ı, kod tabanının mevcut kütüphane ve desenleriyle **piksel düzeyinde** yeniden oluştur. Aşağıdaki token ve ölçüler bağlayıcıdır.

---

## Design Tokens (bağlayıcı)

Tüm değerler `styles.css` `:root` ve `.dark` bloklarından. shadcn formatına (Tailwind v3 → HSL, v4 → oklch/`@theme`) çevirirken bu hex'leri kaynak al.

### Tipografi
- Sans: **Plus Jakarta Sans** (400/500/600/700/800) — Google Fonts
- Mono: **JetBrains Mono** (400–700) — tüm parasal/sayısal değerlerde, `font-variant-numeric: tabular-nums` ile
- Gövde: `letter-spacing: -0.006em`; başlıklar `-0.02em`–`-0.03em`
- Para birimi formatı: `tr-TR`, 2 ondalık, simge `₺` (örn. `2.450,00 ₺`)

### Renkler — Light (`:root`)
| Token | Değer |
|---|---|
| `--background` | `#f4f6fb` |
| `--surface` | `#ffffff` |
| `--surface-2` | `#eef1f7` |
| `--surface-3` | `#e7ebf4` |
| `--foreground` | `#15203a` |
| `--muted-foreground` | `#5a6580` |
| `--faint-foreground` | `#8a93a8` |
| `--border` | `#e2e7f0` |
| `--border-strong` | `#d3dae6` |
| `--input` | `#d7dde9` |
| `--primary` | `#0f2746` (blueprint laciverti) |
| `--primary-fg` | `#ffffff` |
| `--primary-grad-1` → `--primary-grad-2` | `#0c2140` → `#1f3f6e` (header gradient) |
| `--primary-soft` / `--primary-soft-fg` | `#e7edf6` / `#14315b` |
| `--accent` | `#e08a3c` (sıcak amber — birincil aksiyonlar) |
| `--accent-fg` | `#ffffff` |
| `--accent-600` | `#c9742a` (hover) |
| `--accent-soft` / `--accent-soft-fg` | `#fbedda` / `#93551c` |
| `--success` / `-bg` / `-fg` | `#1f8a5b` / `#e1f3ea` / `#156844` |
| `--warning` / `-bg` / `-fg` | `#b07d12` / `#fbf0d6` / `#8a6210` |
| `--destructive` / `-bg` / `-fg` | `#c4374a` / `#fce2e5` / `#9c2a3a` |
| `--info` / `-bg` / `-fg` | `#2f6fb0` / `#e3eef9` / `#245a92` |

### Renkler — Dark (`.dark`)
| Token | Değer |
|---|---|
| `--background` | `#091222` |
| `--surface` | `#101c33` |
| `--surface-2` | `#16243f` |
| `--surface-3` | `#1d2f4d` |
| `--foreground` | `#e8edf6` |
| `--muted-foreground` | `#97a3ba` |
| `--faint-foreground` | `#6b7790` |
| `--border` / `--border-strong` | `#213251` / `#2c4068` |
| `--input` | `#2a3c5c` |
| `--primary` | `#1f3f6e` |
| `--primary-grad-1` → `-2` | `#0b1a33` → `#244a80` |
| `--primary-soft` / `-fg` | `#18294a` / `#aac3e8` |
| `--accent` / `-fg` / `-600` | `#eb9748` / `#1a1206` / `#d9863a` |
| `--accent-soft` / `-fg` | `#3a2a14` / `#f0b878` |
| `--success` / `-bg` / `-fg` | `#36b07a` / `#122e25` / `#6fd6a6` |
| `--warning` / `-bg` / `-fg` | `#d4a23a` / `#322713` / `#ecc878` |
| `--destructive` / `-bg` / `-fg` | `#e0566a` / `#341820` / `#f29aa8` |
| `--info` / `-bg` / `-fg` | `#5a9bd8` / `#14283f` / `#9cc4ec` |

### Radius
`--radius-sm/md/lg/xl` = `7 / 11 / 16 / 22 px` (Tweaks ile keskin `4/6/9/12` veya yuvarlak `10/15/21/28` varyantı).

### Gölgeler
- `--shadow-xs`: `0 1px 2px rgba(15,39,70,.05)`
- `--shadow-card`: `0 1px 2px rgba(15,39,70,.04), 0 6px 18px rgba(15,39,70,.06)`
- `--shadow-card-hover`: `0 2px 4px …, 0 14px 32px rgba(15,39,70,.10)`
- `--shadow-popover`: `0 10px 34px rgba(12,28,54,.18), 0 2px 8px rgba(12,28,54,.10)`
- Dark için karşılıkları `.dark` bloğunda (siyah tabanlı, daha derin).

### Spacing
4'lük skala: `4 / 8 / 12 / 16 / 20 / 24 / 32 / 40 / 48 px`. Owner/tenant tarafı `lg–xl` (24–32) boşluk ritmi; management tarafı kompakt (10–16).

---

## Bileşen Eşleme (HTML sınıfı → shadcn/ui)

| Prototip | shadcn/ui | Not |
|---|---|---|
| `.btn .btn-accent` | `<Button>` (default = accent) | h:40, radius-md, accent zemin |
| `.btn-primary / -outline / -ghost` | `<Button variant="secondary/outline/ghost">` | |
| `.btn-sm`, `.btn-icon` | `size="sm" / "icon"` | |
| `.badge-{success,warning,destructive,info,accent,neutral,primary}` | `<Badge variant>` | renk + nokta + işaret birlikte |
| `.card`, `.dues-card`, `.panel`, `.stat-chip`, `.kpi` | `<Card>` | gölge `--shadow-card` |
| `Dropdown` + `MenuItem` (üst nav) | `<NavigationMenu>` veya `<DropdownMenu>` | hover+click açılır |
| site seçici / avatar / dil | `<DropdownMenu>` | |
| `.tbl` (hesap dökümü, daireler, giderler) | `<Table>` | sağa hizalı sayılar, `tabular-nums` |
| `HesapDokumuModal` | `<Dialog>` | |
| `.seg` (dönem/durum filtresi) | `<Tabs>` / `<ToggleGroup>` | |
| `.input`, `select`, `textarea`, `.check` | `<Input>/<Select>/<Textarea>/<Checkbox>` | focus ring = `--ring` (accent %42) |
| `.chip`, `.flat-tag` | `<Badge variant="outline">` / mono etiket | |
| `.bar` | `<Progress>` | doluluk/katılım |
| ikonlar | **lucide-react** | aşağıdaki eşleme |

### İkon eşleme (özel SVG → lucide-react)
`home→Home, poll→BarChart3, report→FileText, mail→Mail, key→KeyRound, user→User, wrench→Wrench, info→Info, building→Building2, calendar→Calendar, bell→Bell, car→Car, doc→FileText, shield→Shield, gauge→Gauge, search→Search, plus→Plus, download→Download, filter→Filter, send→Send, phone→Phone, sun→Sun, moon→Moon, chevron→ChevronDown, chevronR→ChevronRight, check→Check, arrowUp→ArrowUp, arrowDn→ArrowDown, grid→LayoutGrid, list→List, clock→Clock, pin→MapPin, edit→Pencil, x→X, logout→LogOut`. Logo = ev silüeti + amber node; basit özgün marka, lucide değil (bkz. `ui.jsx` `Logo`).

---

## Ekranlar / Görünümler

### 1. Login (`app/(auth)/login/page.tsx`)
- **Düzen:** 2 sütun grid `1.05fr / 0.95fr`. Sol: `--primary-grad` gradient panel + blueprint network SVG overlay (opacity ~0.5), marka + başlık + 3 istatistik (96 Daire / %87,4 Tahsilat / 4 Blok). Sağ: ortalanmış form kartı (max-width 400).
- **İçerik:** "Tekrar hoş geldiniz" / "Devam etmek için hesabınıza giriş yapın." E-posta + Şifre (ikonlu inputlar), "Beni hatırla" + "Şifremi unuttum", **Giriş yap** (accent, h:46), ayraç "veya", **e-Devlet ile giriş** (outline).
- **Davranış:** Giriş yap / e-Devlet → rol seçimine git. `< 1080px` sol panel gizlenir, tek sütun.

### 2. Rol Seçimi (`app/(auth)/role-select/page.tsx`)
- **Düzen:** Dikey ortalanmış; üstte radial primary-soft glow. Başlık "Nasıl devam etmek istersiniz?" + alt metin. 3 kart `repeat(3, 280px)` (mobilde tek sütun).
- **Kartlar:** Kiracı (info renk, key ikon), Daire Sahibi (accent, home), Yönetim (primary, building). Her kart: 54px ikon kutusu, başlık, açıklama, "Görünüme geç →". Hover'da `translateY(-5px)` + güçlü gölge; köşede blueprint overlay.
- **Davranış:** Kiracı/Sahip → owner-tenant portal; Yönetim → management. Altta "Çıkış yap".

### 3. Owner/Tenant Portal Header (owner-tenant `layout.tsx`)
- **Zemin:** `--primary-grad` yatay gradient + blueprint network overlay (opacity **0.12**, Tweaks ile 0.04–0.30). Sticky, h:64, iç genişlik max 1320.
- **Sol:** ev logosu + "Yonopsis".
- **Orta nav (her biri dropdown):** Anketler · Raporlar · İletişim · **Kira (yalnız owner)** · Size Özel · Teknik Bilgiler · Genel Bilgiler. Alt menü öğeleri `data.jsx`/`portal-header.jsx` `NAV` dizisinde. Aktif öğe açık zeminli "pill" (beyaz zemin, primary metin). Dropdown: `--surface` zemin, `--shadow-popover`, radius-md, hover **ve** click ile açılır, dışarı tıkda kapanır.
- **Sağ:** site/parsel seçici dropdown · dark mode toggle (sun/moon) · dil (TR bayrağı) · avatar (baş harfler, accent) → menüde Profilim/Bildirim/Rol değiştir/Çıkış.

### 4. Owner/Tenant Landing (owner-tenant `page.tsx`)
Sıra:
1. **Karşılama:** Zamana göre selam ("İyi akşamlar/Günaydın, {Ad} 👋") + daire çipleri (site, B Blok · Daire 12, şehir). Sağda 3 statü çipi: **Güncel Bakiye** (negatifse destructive, mono), **Sonraki Vade**, **Açık Talep** sayısı.
2. **İçerik gridi** `1.55fr / 1fr`:
   - Sol **Duyurular** feed'i: her kart kategori `Badge`'i (semantik renk + nokta), başlık, tarih (mono), kısa özet. En yeni üstte. Boşsa boş durum ("Henüz duyuru yok").
   - Sağ **Son Aidat Bilgileri** kartı: başlıkta ay + durum badge'i (Ödendi=success / Bekliyor=warning / Gecikmiş=destructive), büyük aidat tutarı (mono 38px), son ödeme tarihi, güncel bakiye, owner ise "Kira geliri" (success). Birincil aksiyon **Hesap Dökümü** (accent, tam genişlik). Altında **Hızlı İşlemler** listesi.

### 5. Hesap Dökümü (`<Dialog>` veya `/hesap-dokumu`)
- **Üst:** dönem filtresi (`Tabs`: 2026 / Son 6 Ay / Tümü) + **PDF indir** (outline).
- **Tablo:** sütunlar **Tarih · Açıklama · Borç · Alacak · Bakiye**. Tüm tutarlar mono + tabular-nums, **sağa hizalı**. Yürüyen bakiye (running balance) her satırda. Borç `−` işareti + destructive ton; alacak `+` + success ton (renk **ve** işaret birlikte). Footer: toplam borç / toplam alacak / güncel bakiye. Boşsa "Bu dönemde hareket yok."
- Hesaplama: `bakiye = öncekiBakiye − borç + alacak` (bkz. `data.jsx` `buildStatement`).

### 6. Diğer portal sayfaları
- **Kira Ödemeleri** (`KiraPage`): 4 özet çip (aylık kira, sonraki vade, depozito, sözleşme bitiş) + ödeme geçmişi tablosu (durum badge'leri).
- **Talepler** (`TalepPage`): Tabs "Taleplerim / Yeni Talep". Liste = tablo (No, Konu, Kategori, Tarih, Öncelik, Durum). Yeni = form (Konu, Kategori, Öncelik, Açıklama) → gönderince başarı durumu (TLP-XXXX).
- **Anketler** (`AnketPage`): kart grid; her kartta durum badge'i, başlık, katılım `Progress`, "Oy kullan"/"Oyunuz alındı".
- **Genel placeholder** (`GenericPage`): henüz tasarlanmamış nav alt sayfaları için başlık + boş durum; aynı ritim.

### 7. Management Paneli (`management/*`)
- **Sol sidebar** (232px, sticky): marka + "Yönetim" badge'i, site seçici, gruplu nav (Genel: Panel/Daireler/Tahsilat · Finans: Aidat/Giderler/Raporlar · Operasyon: Talepler/Bakım/Duyurular · Site: Anketler/Belgeler/Ayarlar), bazı öğelerde sayaç rozeti. Alt: kullanıcı menüsü (rol değiştir, tema, çıkış). Aktif öğe primary zemin.
- **Topbar** (60px, sticky): sayfa başlığı + arama inputu + bildirim/tema/“Yeni kayıt” (accent).
- **Panel (dashboard):** 4 KPI kartı (Tahsilat oranı, Geciken alacak, Kasa+banka, Açık talep) — değer mono, delta yön + renk (up=success/down=destructive). Altında `1.7fr/1fr` grid: solda **Daire Tahakkuk Durumu** kompakt tablosu; sağda **Blok Doluluk** (`Progress`) + **Açık İş Emirleri** listesi.
- **Daireler:** DataTable — durum sekmeleri (Tümü/Ödenen/Bekleyen/Gecikmiş, sayaçlı) + arama + filtre/dışa aktar. Sütunlar: Daire (mono etiket), Sakin, Tip (Sahip/Kiracı badge), Telefon, Aidat, Ödenen (renkli), Durum. Footer toplam tahakkuk.
- **Giderler / Talepler:** benzer kompakt tablolar.

---

## Etkileşim & Davranış
- **Navigasyon:** Header dropdown'ları hover veya click ile açılır; öğeye tıkda sayfa değişir + scroll top. Aktif üst nav, alt sayfa anahtarının ön ekine göre hesaplanır (`topKeyFor`).
- **Dark mode:** `document.documentElement` üzerinde `.dark` sınıfı; `localStorage["yon_theme"]`'de saklanır. Hedefte `next-themes` ile uygula.
- **Modal:** overlay blur + `--shadow-popover`; dışarı tıkla / X ile kapanır; `scaleIn` girişi.
- **Hover:** kartlar `--shadow-card-hover` + hafif `translateY`; butonlar renk geçişi (.15s).
- **Animasyonlar:** girişler `cubic-bezier(.22,.68,0,1)`, 0.16–0.5s. **Önemli:** girişleri yalnız transform ile yap (opacity'yi 0'dan kalıcı tutma) — `prefers-reduced-motion` / SSR / ilk boyada içerik görünür kalsın.
- **Responsive:** `≤1080px` portal-grid/kpi/mgrid tek sütun, login tek sütun; `≤720px` üst nav gizlenir (mobil menüye taşınmalı), KPI tek sütun.
- **Form validasyonu:** Talep formu boş alan kontrolü; hedefte `react-hook-form + zod` önerilir.

## State Yönetimi
- **Global:** `screen` (login/roles/portal/mgmt) → hedefte **routing** ile değişir. `role` (owner/tenant/management). `theme` (light/dark, persist). `site` (aktif site/parsel).
- **Portal:** `page` (aktif alt sayfa), `stmtOpen` (hesap dökümü modal).
- **Sayfa içi:** dönem filtresi, durum filtresi + arama (Daireler), Tabs (Talepler), oy verme (Anketler), talep gönderildi durumu.
- **Veri:** Tümü şu an `data.jsx` mock'u. Hedefte server component + gerçek API: aidat/bakiye, hesap hareketleri, duyurular, talepler, anketler, daire tahakkuk listesi, gider/iş emri.

## Assets
- **Fontlar:** Plus Jakarta Sans + JetBrains Mono (Google Fonts → `next/font` ile self-host önerilir).
- **İkonlar:** Tümü inline SVG; hedefte **lucide-react** (yukarıdaki eşleme). Logo özgün, korunmalı.
- **Blueprint network overlay:** `ui.jsx` içindeki `BlueprintBg` (grid pattern + bina silüetleri + node ağı) — bir React bileşeni olarak taşınabilir; gerçek görsel gerekmez.
- **Bayrak:** TR bayrağı inline SVG (`login.jsx` `TRFlag`).
- Stok fotoğraf/illüstrasyon **yok**.

## Files (bu pakette)
- `Yonopsis.html` — giriş; script yükleme sırası burada.
- `styles.css` — **token sistemi + atomlar** (ilk port edilecek dosya).
- `app.css` — yapısal bileşen stilleri (login, header, landing, tablolar, modal, management).
- `data.jsx` — mock veri + format yardımcıları (`fmtTRY`).
- `ui.jsx` — ikon seti, `BlueprintBg`, `Logo`, `Badge`, `Dropdown`, `MenuItem`.
- `login.jsx` — Login + RoleSelect + TRFlag.
- `portal-header.jsx` — `NAV` dizisi + header.
- `portal-pages.jsx` — Landing + StatementTable + HesapDokumuModal.
- `portal-pages2.jsx` — Kira / Talep / Anket / Generic.
- `portal.jsx` — portal kabuğu + footer.
- `management.jsx` — yönetim paneli (sidebar, dashboard, tablolar).
- `app.jsx` — routing + tema + Tweaks (CSS değişkeni override mantığı).

## Önerilen uygulama sırası
1. `styles.css` token'larını `globals.css` shadcn token map'ine port et (light + dark).
2. Atomları shadcn bileşenlerine bağla (Button/Badge/Card/Table/Dialog/Tabs/DropdownMenu/Input).
3. Owner/tenant `layout.tsx` (header) → Landing → Hesap Dökümü.
4. Management `layout.tsx` → Panel → Daireler DataTable.
5. Login + rol seçimi + yönlendirme.
6. `next-themes` ile dark mode; mock veriyi gerçek veri katmanıyla değiştir.
