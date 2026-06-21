/* ============================================================
   Yonopsis — Örnek Veri (Türkçe)
   ============================================================ */

const SITE = {
  name: "Maviyaka Konutları",
  parcel: "Ada 2847 · Parsel 14",
  city: "İstanbul, Ataşehir",
};

const SITES = [
  { id: "maviyaka", name: "Maviyaka Konutları", parcel: "Ada 2847 · Parsel 14" },
  { id: "lavanta", name: "Lavanta Bahçeleri", parcel: "Ada 1190 · Parsel 6" },
  { id: "panorama", name: "Panorama Rezidans", parcel: "Ada 3320 · Parsel 2" },
];

const CURRENT_USER = {
  owner:  { name: "Elif Kaya",   initials: "EK", block: "B Blok", flat: "Daire 12", role: "Daire Sahibi" },
  tenant: { name: "Mert Demir",  initials: "MD", block: "C Blok", flat: "Daire 7",  role: "Kiracı" },
  management: { name: "Selin Arıkan", initials: "SA", role: "Site Yöneticisi" },
};

/* ---- Duyurular (announcements feed) ---- */
const ANNOUNCEMENTS = [
  {
    id: 1, cat: "Önemli", tone: "destructive", date: "12 Haziran 2026",
    title: "Su deposu bakımı — 15 Haziran 09:00–13:00",
    body: "Ana su deposunun yıllık dezenfeksiyonu nedeniyle belirtilen saatlerde tüm bloklarda su kesintisi yaşanacaktır. Mağduriyet yaşanmaması için önceden su biriktirmenizi rica ederiz.",
  },
  {
    id: 2, cat: "Etkinlik", tone: "accent", date: "10 Haziran 2026",
    title: "Yaz şenliği ve komşuluk kahvaltısı",
    body: "21 Haziran Pazar sabahı sosyal tesis bahçesinde geleneksel kahvaltımızda buluşuyoruz. Katılım için yönetim ofisine kayıt yaptırabilirsiniz.",
  },
  {
    id: 3, cat: "Bakım", tone: "info", date: "8 Haziran 2026",
    title: "A Blok asansör revizyonu tamamlandı",
    body: "A Blok 2 numaralı asansörün yıllık revizyon ve fren testi tamamlanmış, TSE muayenesi olumlu sonuçlanmıştır.",
  },
  {
    id: 4, cat: "Yönetim", tone: "primary", date: "3 Haziran 2026",
    title: "Haziran ayı aidat tutarı güncellendi",
    body: "Artan enerji ve personel giderleri nedeniyle Haziran 2026 itibarıyla aylık aidat 2.450 ₺ olarak belirlenmiştir. Detaylar gider raporunda mevcuttur.",
  },
  {
    id: 5, cat: "Güvenlik", tone: "warning", date: "29 Mayıs 2026",
    title: "Otopark turnike kartları yenileniyor",
    body: "Yeni nesil RFID kartlara geçiş yapılmaktadır. Eski kartınızı 30 Haziran'a kadar güvenlik noktasından yenileyebilirsiniz.",
  },
];

/* ---- Aidat / Bakiye ---- */
const DUES_SUMMARY = {
  monthLabel: "Haziran 2026",
  amount: 2450.0,
  status: "warning",         // success | warning | destructive
  statusLabel: "Bekliyor",
  dueDate: "30 Haziran 2026",
  balance: -2450.0,          // negatif = borç
  rentIncome: 18500.0,       // owner için kira geliri
};

/* ---- Hesap Dökümü (running balance) ---- */
// type: debit (borç) | credit (alacak)
const STATEMENT_RAW = [
  { date: "01.01.2026", desc: "Devreden bakiye", debit: 0, credit: 0, opening: 0 },
  { date: "05.01.2026", desc: "Ocak aidatı tahakkuku", debit: 2300, credit: 0 },
  { date: "11.01.2026", desc: "Aidat ödemesi — Havale", debit: 0, credit: 2300 },
  { date: "05.02.2026", desc: "Şubat aidatı tahakkuku", debit: 2300, credit: 0 },
  { date: "09.02.2026", desc: "Aidat ödemesi — Kredi kartı", debit: 0, credit: 2300 },
  { date: "18.02.2026", desc: "Otopark ek kullanım bedeli", debit: 150, credit: 0 },
  { date: "20.02.2026", desc: "Otopark bedeli ödemesi", debit: 0, credit: 150 },
  { date: "05.03.2026", desc: "Mart aidatı tahakkuku", debit: 2300, credit: 0 },
  { date: "14.03.2026", desc: "Aidat ödemesi — Havale", debit: 0, credit: 2300 },
  { date: "05.04.2026", desc: "Nisan aidatı tahakkuku", debit: 2300, credit: 0 },
  { date: "10.04.2026", desc: "Aidat ödemesi — Otomatik ödeme", debit: 0, credit: 2300 },
  { date: "22.04.2026", desc: "Su sayacı fark bedeli", debit: 87.5, credit: 0 },
  { date: "05.05.2026", desc: "Mayıs aidatı tahakkuku", debit: 2300, credit: 0 },
  { date: "12.05.2026", desc: "Aidat ödemesi — Havale", debit: 0, credit: 2300 },
  { date: "22.05.2026", desc: "Su sayacı fark ödemesi", debit: 0, credit: 87.5 },
  { date: "05.06.2026", desc: "Haziran aidatı tahakkuku", debit: 2450, credit: 0 },
  { date: "08.06.2026", desc: "Demirbaş katılım payı (peyzaj)", debit: 320, credit: 0 },
];

function buildStatement(rows) {
  let bal = 0;
  return rows.map((r) => {
    bal = bal - r.debit + r.credit; // borç bakiyeyi düşürür
    return { ...r, balance: bal };
  });
}
const STATEMENT = buildStatement(STATEMENT_RAW);

/* ---- Kira ödemeleri (tenant) ---- */
const RENT = {
  monthly: 18500,
  status: "success",
  statusLabel: "Ödendi",
  nextDue: "05 Temmuz 2026",
  deposit: 37000,
  contractEnd: "31 Ağustos 2026",
  payments: [
    { month: "Haziran 2026",  amount: 18500, date: "03.06.2026", method: "Otomatik ödeme", status: "success", label: "Ödendi" },
    { month: "Mayıs 2026",    amount: 18500, date: "04.05.2026", method: "Havale",          status: "success", label: "Ödendi" },
    { month: "Nisan 2026",    amount: 18500, date: "05.04.2026", method: "Havale",          status: "success", label: "Ödendi" },
    { month: "Mart 2026",     amount: 18000, date: "06.03.2026", method: "Kredi kartı",     status: "success", label: "Ödendi" },
    { month: "Şubat 2026",    amount: 18000, date: "03.02.2026", method: "Havale",          status: "success", label: "Ödendi" },
  ],
};

/* ---- Talepler (requests) ---- */
const REQUESTS = [
  { id: "TLP-2041", subject: "Mutfak lavabo sifonu damlatıyor", cat: "Tesisat", date: "11.06.2026", status: "warning", label: "İşlemde", priority: "Orta" },
  { id: "TLP-2034", subject: "Otopark aydınlatması yanmıyor (B2)", cat: "Elektrik", date: "06.06.2026", status: "info",    label: "Atandı",  priority: "Düşük" },
  { id: "TLP-2019", subject: "Daire kapı zili çalışmıyor", cat: "Elektrik", date: "28.05.2026", status: "success", label: "Çözüldü", priority: "Orta" },
  { id: "TLP-1998", subject: "Asansör kapısı geç kapanıyor", cat: "Asansör", date: "19.05.2026", status: "success", label: "Çözüldü", priority: "Yüksek" },
];

/* ---- Anketler (surveys) ---- */
const SURVEYS = [
  { id: 1, title: "Çocuk oyun alanı yenileme tercihi", status: "active",  label: "Aktif",  ends: "20 Haziran 2026", votes: 64, total: 120, you: false },
  { id: 2, title: "Site giriş güvenlik sistemi (yüz tanıma)", status: "active", label: "Aktif", ends: "25 Haziran 2026", votes: 38, total: 120, you: true },
  { id: 3, title: "Ortak alan elektrik tedarikçisi seçimi", status: "closed", label: "Sonuçlandı", ends: "30 Mayıs 2026", votes: 109, total: 120, you: true },
];

/* ============================================================
   MANAGEMENT verileri
   ============================================================ */

const MGMT_KPIS = [
  { id: "collect", label: "Tahsilat oranı", value: "%87,4", delta: "+3,1", trend: "up", sub: "Haziran 2026" },
  { id: "overdue", label: "Geciken alacak", value: "142.300 ₺", delta: "+12.400", trend: "down", sub: "31 daire" },
  { id: "balance", label: "Kasa + banka", value: "486.920 ₺", delta: "+58.700", trend: "up", sub: "Toplam likidite" },
  { id: "open",    label: "Açık talep",    value: "14", delta: "-3", trend: "up", sub: "4 yüksek öncelik" },
];

const MGMT_BLOCKS = [
  { id: "A", flats: 24, occupied: 23 },
  { id: "B", flats: 24, occupied: 24 },
  { id: "C", flats: 24, occupied: 22 },
  { id: "D", flats: 24, occupied: 21 },
];

// Daire bazlı tahakkuk tablosu
const MGMT_RESIDENTS = [
  { flat: "A-3",  name: "Ahmet Yıldız",     type: "Sahip",  phone: "0532 •• 41", dues: 2450, paid: 2450, status: "success", label: "Ödendi" },
  { flat: "A-8",  name: "Zeynep Aksoy",     type: "Kiracı", phone: "0541 •• 09", dues: 2450, paid: 0,    status: "destructive", label: "Gecikmiş" },
  { flat: "A-15", name: "Burak Şahin",      type: "Sahip",  phone: "0535 •• 77", dues: 2450, paid: 2450, status: "success", label: "Ödendi" },
  { flat: "B-2",  name: "Elif Kaya",        type: "Sahip",  phone: "0533 •• 18", dues: 2450, paid: 0,    status: "warning", label: "Bekliyor" },
  { flat: "B-12", name: "Deniz Çelik",      type: "Sahip",  phone: "0538 •• 52", dues: 2450, paid: 2450, status: "success", label: "Ödendi" },
  { flat: "B-19", name: "Ayşe Korkmaz",     type: "Kiracı", phone: "0530 •• 63", dues: 2450, paid: 1225, status: "warning", label: "Kısmi" },
  { flat: "C-7",  name: "Mert Demir",       type: "Kiracı", phone: "0542 •• 30", dues: 2450, paid: 2450, status: "success", label: "Ödendi" },
  { flat: "C-11", name: "Gökhan Arslan",    type: "Sahip",  phone: "0537 •• 84", dues: 2450, paid: 0,    status: "destructive", label: "Gecikmiş" },
  { flat: "C-20", name: "Selin Yılmaz",     type: "Sahip",  phone: "0539 •• 11", dues: 2450, paid: 2450, status: "success", label: "Ödendi" },
  { flat: "D-4",  name: "Kerem Öztürk",     type: "Kiracı", phone: "0531 •• 95", dues: 2450, paid: 2450, status: "success", label: "Ödendi" },
  { flat: "D-9",  name: "Nazlı Doğan",      type: "Sahip",  phone: "0536 •• 27", dues: 2450, paid: 0,    status: "warning", label: "Bekliyor" },
  { flat: "D-16", name: "Emre Polat",       type: "Sahip",  phone: "0534 •• 48", dues: 2450, paid: 2450, status: "success", label: "Ödendi" },
];

const MGMT_EXPENSES = [
  { date: "10.06.2026", desc: "Güvenlik personeli (3 kişi)", cat: "Personel", amount: 84000 },
  { date: "08.06.2026", desc: "Ortak alan elektrik", cat: "Enerji", amount: 31250 },
  { date: "05.06.2026", desc: "Peyzaj & bahçe bakımı", cat: "Bakım", amount: 14800 },
  { date: "03.06.2026", desc: "Asansör bakım sözleşmesi", cat: "Bakım", amount: 9600 },
  { date: "01.06.2026", desc: "Temizlik malzemeleri", cat: "Malzeme", amount: 6420 },
];

const MGMT_TASKS = [
  { id: "TLP-2041", subject: "Mutfak lavabo sifonu — B-12", assignee: "Tesisat ekibi", status: "warning", label: "İşlemde", due: "Bugün" },
  { id: "TLP-2038", subject: "Çatı yalıtım kontrolü — A Blok", assignee: "Dış firma", status: "info", label: "Planlandı", due: "16 Haz" },
  { id: "TLP-2034", subject: "Otopark aydınlatması — B2", assignee: "Elektrik ekibi", status: "info", label: "Atandı", due: "14 Haz" },
  { id: "TLP-2030", subject: "Yangın tüpü dolum kontrolü", assignee: "Dış firma", status: "destructive", label: "Gecikti", due: "10 Haz" },
];

/* Helpers */
function fmtTRY(n, withSymbol = true) {
  const s = Math.abs(n).toLocaleString("tr-TR", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
  return (n < 0 ? "-" : "") + s + (withSymbol ? " ₺" : "");
}
function fmtTRY0(n) {
  return n.toLocaleString("tr-TR", { maximumFractionDigits: 0 }) + " ₺";
}

Object.assign(window, {
  SITE, SITES, CURRENT_USER, ANNOUNCEMENTS, DUES_SUMMARY, STATEMENT,
  RENT, REQUESTS, SURVEYS,
  MGMT_KPIS, MGMT_BLOCKS, MGMT_RESIDENTS, MGMT_EXPENSES, MGMT_TASKS,
  fmtTRY, fmtTRY0,
});
