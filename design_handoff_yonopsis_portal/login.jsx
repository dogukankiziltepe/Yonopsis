/* ============================================================
   Yonopsis — Login + Rol Seçimi
   ============================================================ */

function TRFlag() {
  return (
    <svg className="flag" viewBox="0 0 30 20" aria-hidden="true">
      <rect width="30" height="20" fill="#E30A17" />
      <circle cx="12" cy="10" r="5" fill="#fff" />
      <circle cx="13.2" cy="10" r="4" fill="#E30A17" />
      <path fill="#fff" d="M17.5 10l4.6-1.5-2.85 3.9V7.6l2.85 3.9z" />
    </svg>
  );
}

function LoginScreen({ onLogin }) {
  const [email, setEmail] = React.useState("elif.kaya@maviyaka.com.tr");
  const [pw, setPw] = React.useState("••••••••");

  return (
    <div className="login-wrap fade-in">
      <aside className="login-aside">
        <div className="bp"><BlueprintBg opacity={0.55} /></div>
        <div className="login-aside-content">
          <div className="login-brand">
            <Logo size={34} light />
            <span className="login-brand-name">Yonopsis</span>
          </div>
        </div>
        <div className="login-aside-content">
          <h1 className="login-headline">Sitenizin dijital yönetim merkezi.</h1>
          <p className="login-sub">
            Aidatlar, duyurular, talepler ve raporlar — sakin, yönetici ve mülk sahibi
            için tek bir blueprint üzerinde buluşuyor.
          </p>
        </div>
        <div className="login-stats">
          <div>
            <div className="login-stat-v num">96</div>
            <div className="login-stat-l">Daire</div>
          </div>
          <div>
            <div className="login-stat-v num">%87,4</div>
            <div className="login-stat-l">Tahsilat oranı</div>
          </div>
          <div>
            <div className="login-stat-v num">4</div>
            <div className="login-stat-l">Blok</div>
          </div>
        </div>
      </aside>

      <main className="login-main">
        <div className="login-card fade-up">
          <div style={{ display: "flex", alignItems: "center", gap: 10, marginBottom: 26 }}>
            <Logo size={28} />
            <span style={{ fontSize: 17, fontWeight: 800, letterSpacing: "-0.02em" }}>Yonopsis</span>
          </div>
          <h2>Tekrar hoş geldiniz</h2>
          <p className="muted">Devam etmek için hesabınıza giriş yapın.</p>

          <div className="field">
            <span className="label">E-posta</span>
            <div className="input-icon">
              <Icon name="mail" size={17} />
              <input className="input" value={email} onChange={(e) => setEmail(e.target.value)}
                placeholder="ornek@site.com" />
            </div>
          </div>
          <div className="field">
            <span className="label">Şifre</span>
            <div className="input-icon">
              <Icon name="key" size={17} />
              <input className="input" type="password" value={pw} onChange={(e) => setPw(e.target.value)}
                placeholder="••••••••" />
            </div>
          </div>

          <div className="login-row">
            <label className="check">
              <input type="checkbox" defaultChecked /> Beni hatırla
            </label>
            <span className="link">Şifremi unuttum</span>
          </div>

          <button className="btn btn-accent" style={{ width: "100%", height: 46 }} onClick={onLogin}>
            Giriş yap
            <Icon name="chevronR" size={18} />
          </button>

          <div style={{ display: "flex", alignItems: "center", gap: 12, margin: "22px 0" }}>
            <hr className="divider" style={{ flex: 1 }} />
            <span style={{ fontSize: 12, color: "var(--faint-foreground)" }}>veya</span>
            <hr className="divider" style={{ flex: 1 }} />
          </div>

          <button className="btn btn-outline" style={{ width: "100%", height: 46 }} onClick={onLogin}>
            <Icon name="shield" size={18} /> e-Devlet ile giriş
          </button>

          <p style={{ textAlign: "center", fontSize: 13, color: "var(--muted-foreground)", marginTop: 24 }}>
            Hesabınız yok mu? <span className="link">Yönetimle iletişime geçin</span>
          </p>
        </div>
      </main>
    </div>
  );
}

/* ---------- Rol seçimi ---------- */
const ROLE_CARDS = [
  {
    key: "tenant", icon: "key", color: "var(--info-fg)", bg: "var(--info-bg)",
    title: "Kiracı", desc: "Kira ödemeleri, sözleşme bilgileri, talep oluşturma ve site duyurularına erişin.",
  },
  {
    key: "owner", icon: "home", color: "var(--accent-600)", bg: "var(--accent-soft)",
    title: "Daire Sahibi", desc: "Aidat hesabı, kira geliri özeti, anketler ve mülkünüze dair tüm bilgiler.",
  },
  {
    key: "management", icon: "building", color: "var(--primary-soft-fg)", bg: "var(--primary-soft)",
    title: "Yönetim", desc: "Tahsilat, gider, talep ve daire yönetimi için operasyon paneli.",
  },
];

function RoleSelect({ onPick, onBack }) {
  return (
    <div className="role-wrap fade-in">
      <div className="role-head">
        <div style={{ display: "inline-flex", alignItems: "center", gap: 10 }}>
          <Logo size={30} />
          <span style={{ fontSize: 18, fontWeight: 800, letterSpacing: "-0.02em" }}>Yonopsis</span>
        </div>
        <h1>Nasıl devam etmek istersiniz?</h1>
        <p>Hesabınız birden fazla role sahip. Lütfen bir görünüm seçin.</p>
      </div>

      <div className="role-grid">
        {ROLE_CARDS.map((r, i) => (
          <button key={r.key} className="role-card fade-up" style={{ animationDelay: i * 70 + "ms" }}
            onClick={() => onPick(r.key)}>
            <div className="role-corner"><BlueprintBg opacity={0.5} /></div>
            <div className="role-ic" style={{ background: r.bg, color: r.color }}>
              <Icon name={r.icon} size={26} stroke={1.8} />
            </div>
            <h3>{r.title}</h3>
            <p>{r.desc}</p>
            <div className="role-go">Görünüme geç <Icon name="chevronR" size={16} /></div>
          </button>
        ))}
      </div>

      <button className="btn btn-ghost" style={{ marginTop: 34, position: "relative", zIndex: 2 }} onClick={onBack}>
        <Icon name="logout" size={16} style={{ transform: "scaleX(-1)" }} /> Çıkış yap
      </button>
    </div>
  );
}

Object.assign(window, { LoginScreen, RoleSelect, TRFlag });
