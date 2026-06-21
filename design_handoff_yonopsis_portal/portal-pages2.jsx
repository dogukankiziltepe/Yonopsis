/* ============================================================
   Yonopsis — Portal Sayfaları (kira, talepler, anketler, genel)
   ============================================================ */

function PageHead({ eyebrow, title, desc, actions }) {
  return (
    <div className="page-head" style={{ display: "flex", justifyContent: "space-between", alignItems: "flex-end", gap: 20, flexWrap: "wrap" }}>
      <div>
        {eyebrow && <div className="eyebrow">{eyebrow}</div>}
        <h1>{title}</h1>
        {desc && <p>{desc}</p>}
      </div>
      {actions && <div style={{ display: "flex", gap: 10 }}>{actions}</div>}
    </div>
  );
}

/* ---------- Kira ödemeleri ---------- */
function KiraPage() {
  const r = RENT;
  return (
    <div className="pcontainer fade-in">
      <PageHead eyebrow="Kira" title="Kira Ödemeleri"
        desc="Aylık kira ödemeleriniz, sözleşme bilgileri ve geçmiş işlemler."
        actions={<button className="btn btn-accent"><Icon name="download" size={16} /> Dekont indir</button>} />

      <div style={{ display: "grid", gridTemplateColumns: "repeat(4, 1fr)", gap: 16, marginBottom: 24 }}>
        {[
          { l: "Aylık kira", v: fmtTRY(r.monthly), ic: "key" },
          { l: "Sonraki vade", v: r.nextDue, ic: "calendar", small: true },
          { l: "Depozito", v: fmtTRY(r.deposit), ic: "shield" },
          { l: "Sözleşme bitiş", v: r.contractEnd, ic: "doc", small: true },
        ].map((s) => (
          <div key={s.l} className="stat-chip" style={{ minWidth: 0 }}>
            <div className="l"><Icon name={s.ic} size={14} /> {s.l}</div>
            <div className="v num" style={{ fontSize: s.small ? 15 : 19 }}>{s.v}</div>
          </div>
        ))}
      </div>

      <div className="section-title"><h2>Ödeme Geçmişi</h2><Badge tone="success" dot>Güncel</Badge></div>
      <div className="tbl-wrap">
        <table className="tbl">
          <thead><tr><th>Dönem</th><th>Tarih</th><th>Yöntem</th><th className="r">Tutar</th><th className="r" style={{ width: 120 }}>Durum</th></tr></thead>
          <tbody>
            {r.payments.map((p, i) => (
              <tr key={i}>
                <td style={{ fontWeight: 600 }}>{p.month}</td>
                <td className="num" style={{ color: "var(--muted-foreground)" }}>{p.date}</td>
                <td>{p.method}</td>
                <td className="r num" style={{ fontWeight: 700 }}>{fmtTRY(p.amount)}</td>
                <td className="r"><Badge tone={p.status} dot>{p.label}</Badge></td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

/* ---------- Talepler ---------- */
function TalepPage({ create }) {
  const [tab, setTab] = React.useState(create ? "yeni" : "liste");
  const [sent, setSent] = React.useState(false);

  return (
    <div className="pcontainer fade-in">
      <PageHead eyebrow="İletişim" title={tab === "yeni" ? "Talep Oluştur" : "Talep Geçmişi"}
        desc="Site yönetimine ilettiğiniz talepleri takip edin veya yeni bir talep oluşturun." />

      <div className="seg" style={{ marginBottom: 20 }}>
        <button className={tab === "liste" ? "on" : ""} onClick={() => setTab("liste")}>Taleplerim</button>
        <button className={tab === "yeni" ? "on" : ""} onClick={() => { setTab("yeni"); setSent(false); }}>Yeni Talep</button>
      </div>

      {tab === "liste" ? (
        <div className="tbl-wrap">
          <table className="tbl">
            <thead><tr><th style={{ width: 110 }}>No</th><th>Konu</th><th>Kategori</th><th>Tarih</th><th>Öncelik</th><th className="r" style={{ width: 120 }}>Durum</th></tr></thead>
            <tbody>
              {REQUESTS.map((rq) => (
                <tr key={rq.id}>
                  <td><span className="flat-tag">{rq.id}</span></td>
                  <td style={{ fontWeight: 600 }}>{rq.subject}</td>
                  <td>{rq.cat}</td>
                  <td className="num" style={{ color: "var(--muted-foreground)" }}>{rq.date}</td>
                  <td>{rq.priority}</td>
                  <td className="r"><Badge tone={rq.status} dot>{rq.label}</Badge></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      ) : sent ? (
        <div className="card"><div className="empty">
          <div className="ic" style={{ background: "var(--success-bg)", color: "var(--success-fg)" }}><Icon name="check" size={24} /></div>
          <h3>Talebiniz alındı</h3>
          <p>Talebiniz <b>TLP-2042</b> numarasıyla kaydedildi. Yönetim en kısa sürede dönüş yapacak.</p>
          <button className="btn btn-outline btn-sm" style={{ marginTop: 16 }} onClick={() => setTab("liste")}>Taleplerime dön</button>
        </div></div>
      ) : (
        <div className="card" style={{ padding: 24, maxWidth: 640 }}>
          <div className="field"><span className="label">Konu</span>
            <input className="input" placeholder="Örn. Mutfak lavabosu damlatıyor" /></div>
          <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: 14 }}>
            <div className="field"><span className="label">Kategori</span>
              <select className="input"><option>Tesisat</option><option>Elektrik</option><option>Asansör</option><option>Genel</option></select></div>
            <div className="field"><span className="label">Öncelik</span>
              <select className="input"><option>Düşük</option><option>Orta</option><option>Yüksek</option></select></div>
          </div>
          <div className="field"><span className="label">Açıklama</span>
            <textarea className="input" style={{ height: 110, padding: 12, resize: "vertical" }} placeholder="Talebinizi detaylı açıklayın..."></textarea></div>
          <div style={{ display: "flex", gap: 10, marginTop: 8 }}>
            <button className="btn btn-accent" onClick={() => setSent(true)}><Icon name="send" size={16} /> Talebi gönder</button>
            <button className="btn btn-ghost" onClick={() => setTab("liste")}>Vazgeç</button>
          </div>
        </div>
      )}
    </div>
  );
}

/* ---------- Anketler ---------- */
function AnketPage() {
  const [voted, setVoted] = React.useState({});
  return (
    <div className="pcontainer fade-in">
      <PageHead eyebrow="Anketler" title="Aktif Anketler"
        desc="Site kararlarına katılın. Oyunuz yönetim planına göre değerlendirilir." />
      <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(330px, 1fr))", gap: 16 }}>
        {SURVEYS.map((s) => {
          const pct = Math.round((s.votes / s.total) * 100);
          const done = s.status === "closed" || voted[s.id] || s.you;
          return (
            <div key={s.id} className="card" style={{ padding: 20 }}>
              <div style={{ display: "flex", justifyContent: "space-between", marginBottom: 12 }}>
                <Badge tone={s.status === "active" ? "accent" : "neutral"} dot>{s.label}</Badge>
                <span className="ann-date">{s.status === "active" ? "Bitiş: " + s.ends : s.ends}</span>
              </div>
              <h3 style={{ fontSize: 16, fontWeight: 700, margin: "0 0 16px", letterSpacing: "-0.01em" }}>{s.title}</h3>
              <div style={{ display: "flex", justifyContent: "space-between", fontSize: 12.5, color: "var(--muted-foreground)", marginBottom: 7 }}>
                <span>Katılım</span><span className="num">{s.votes}/{s.total} · %{pct}</span>
              </div>
              <div className="bar" style={{ marginBottom: 16 }}><span style={{ width: pct + "%" }} /></div>
              {done ? (
                <button className="btn btn-outline btn-sm" style={{ width: "100%" }} disabled>
                  <Icon name="check" size={15} /> {s.status === "closed" ? "Sonuçlandı" : "Oyunuz alındı"}
                </button>
              ) : (
                <button className="btn btn-accent btn-sm" style={{ width: "100%" }} onClick={() => setVoted((v) => ({ ...v, [s.id]: true }))}>
                  Oy kullan
                </button>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}

/* ---------- Generic placeholder page ---------- */
const PAGE_META = {
  "anketler-gecmis": ["Anketler", "Geçmiş Anketler", "Daha önce sonuçlanan tüm anketler ve katılım oranları."],
  "anketler-sonuc": ["Anketler", "Anket Sonuçları", "Sonuçlanan anketlerin detaylı dökümü."],
  "rapor-aidat": ["Raporlar", "Aidat Raporu", "Dönemsel aidat tahakkuk ve tahsilat raporlarınız."],
  "rapor-gider": ["Raporlar", "Gider Raporu", "Sitenin gider kalemleri ve dağılımı."],
  "rapor-yillik": ["Raporlar", "Yıllık Özet", "Yıl bazında gelir-gider ve bakiye özeti."],
  "iletisim-mesaj": ["İletişim", "Yönetime Mesaj", "Site yönetimi ile doğrudan yazışın."],
  "talep-gecmis": ["İletişim", "Talep Geçmişi", "Geçmiş tüm talepleriniz ve durumları."],
  "kira-sozlesme": ["Kira", "Sözleşme", "Kira sözleşmenizin detayları ve belgeleri."],
  "kira-gecmis": ["Kira", "Ödeme Geçmişi", "Tüm kira ödemelerinizin dökümü."],
  "ozel-profil": ["Size Özel", "Profilim", "Kişisel bilgileriniz ve iletişim tercihleri."],
  "ozel-araclar": ["Size Özel", "Araçlarım", "Otoparka kayıtlı araçlarınız ve plaka tanıma."],
  "ozel-kart": ["Size Özel", "Giriş Kartlarım", "Turnike ve kapı giriş kartlarınız."],
  "ozel-bildirim": ["Size Özel", "Bildirim Ayarları", "E-posta, SMS ve uygulama bildirim tercihleri."],
  "teknik-ariza": ["Teknik Bilgiler", "Arıza Bildir", "Daire veya ortak alan arızalarını bildirin."],
  "teknik-bakim": ["Teknik Bilgiler", "Bakım Takvimi", "Planlı bakım ve servis takvimi."],
  "teknik-sayac": ["Teknik Bilgiler", "Sayaç / Asansör Bilgileri", "Sayaç endeksleri ve asansör muayene kayıtları."],
  "genel-kurallar": ["Genel Bilgiler", "Site Kuralları", "Site yönetim ve yaşam kuralları."],
  "genel-plan": ["Genel Bilgiler", "Yönetim Planı", "Resmi yönetim planı belgesi."],
  "genel-rehber": ["Genel Bilgiler", "İletişim Rehberi", "Yönetim, güvenlik ve acil durum iletişim bilgileri."],
  "genel-belge": ["Genel Bilgiler", "Belgeler", "Toplantı tutanakları, raporlar ve diğer belgeler."],
};

function GenericPage({ page, onNav }) {
  const [eyebrow, title, desc] = PAGE_META[page] || ["Yonopsis", "Sayfa", "İçerik hazırlanıyor."];
  return (
    <div className="pcontainer fade-in">
      <PageHead eyebrow={eyebrow} title={title} desc={desc} />
      <div className="card"><div className="empty" style={{ padding: "72px 24px" }}>
        <div className="ic" style={{ width: 60, height: 60 }}><Icon name="doc" size={26} /></div>
        <h3>Bu bölüm prototip kapsamında özet olarak yer alıyor</h3>
        <p style={{ maxWidth: "44ch", margin: "0 auto" }}>
          “{title}” ekranı aynı tema ve token sistemiyle, ferah owner/tenant ritmini koruyarak tasarlanacak.
          İçerik yapısını netleştirmek için yönetimle paylaşabiliriz.
        </p>
        <button className="btn btn-outline btn-sm" style={{ marginTop: 18 }} onClick={() => onNav("home")}>
          <Icon name="home" size={15} /> Ana sayfaya dön
        </button>
      </div></div>
    </div>
  );
}

Object.assign(window, { KiraPage, TalepPage, AnketPage, GenericPage, PageHead });
