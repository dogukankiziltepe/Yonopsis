/* ============================================================
   Yonopsis — Portal Header (owner/tenant)
   ============================================================ */

const NAV = [
  { key: "anketler", label: "Anketler", icon: "poll", items: [
    { key: "anketler-aktif", label: "Aktif Anketler", icon: "poll" },
    { key: "anketler-gecmis", label: "Geçmiş Anketler", icon: "clock" },
    { key: "anketler-sonuc", label: "Sonuçlar", icon: "report" },
  ]},
  { key: "raporlar", label: "Raporlar", icon: "report", items: [
    { key: "rapor-aidat", label: "Aidat Raporu", icon: "report" },
    { key: "rapor-gider", label: "Gider Raporu", icon: "list" },
    { key: "rapor-yillik", label: "Yıllık Özet", icon: "gauge" },
  ]},
  { key: "iletisim", label: "İletişim", icon: "mail", items: [
    { key: "iletisim-mesaj", label: "Yönetime Mesaj", icon: "send" },
    { key: "talep-olustur", label: "Talep Oluştur", icon: "plus" },
    { key: "talep-gecmis", label: "Talep Geçmişi", icon: "list" },
  ]},
  { key: "kira", label: "Kira", icon: "key", owner: true, items: [
    { key: "kira-odemeleri", label: "Kira Ödemeleri", icon: "key" },
    { key: "kira-sozlesme", label: "Sözleşme", icon: "doc" },
    { key: "kira-gecmis", label: "Ödeme Geçmişi", icon: "clock" },
  ]},
  { key: "ozel", label: "Size Özel", icon: "user", items: [
    { key: "ozel-profil", label: "Profilim", icon: "user" },
    { key: "ozel-araclar", label: "Araçlarım", icon: "car" },
    { key: "ozel-kart", label: "Giriş Kartlarım", icon: "shield" },
    { key: "ozel-bildirim", label: "Bildirim Ayarları", icon: "bell" },
  ]},
  { key: "teknik", label: "Teknik Bilgiler", icon: "wrench", items: [
    { key: "teknik-ariza", label: "Arıza Bildir", icon: "wrench" },
    { key: "teknik-bakim", label: "Bakım Takvimi", icon: "calendar" },
    { key: "teknik-sayac", label: "Sayaç / Asansör", icon: "gauge" },
  ]},
  { key: "genel", label: "Genel Bilgiler", icon: "info", items: [
    { key: "genel-kurallar", label: "Site Kuralları", icon: "shield" },
    { key: "genel-plan", label: "Yönetim Planı", icon: "doc" },
    { key: "genel-rehber", label: "İletişim Rehberi", icon: "phone" },
    { key: "genel-belge", label: "Belgeler", icon: "report" },
  ]},
];

function topKeyFor(page) {
  for (const n of NAV) { if (n.items.some((i) => i.key === page) || n.key === page) return n.key; }
  return page === "home" ? "home" : null;
}

function PortalHeader({ role, page, onNav, theme, onTheme, site, onSite, onSwitchRole, onLogout, bpOpacity = 0.12 }) {
  const user = CURRENT_USER[role] || CURRENT_USER.owner;
  const activeTop = topKeyFor(page);
  const navItems = NAV.filter((n) => !n.owner || role === "owner");

  return (
    <header className="phead">
      <BlueprintBg opacity={bpOpacity} />
      <div className="phead-inner">
        <button className="phead-brand" onClick={() => onNav("home")} style={{ background: "none", border: 0, cursor: "pointer", color: "#fff" }}>
          <Logo size={28} light />
          <span className="nm">Yonopsis</span>
        </button>

        <nav className="pnav">
          {navItems.map((n) => (
            <Dropdown key={n.key} width={232}
              trigger={(open) => (
                <button className={"pnav-item" + (activeTop === n.key ? " active" : "") + (open ? " open" : "")}>
                  {n.label}
                  <Icon name="chevron" size={14} className="chev" />
                </button>
              )}>
              <div className="dd-menu-label">{n.label}</div>
              {n.items.map((it) => (
                <MenuItem key={it.key} icon={it.icon} active={page === it.key}
                  onClick={() => onNav(it.key)}>{it.label}</MenuItem>
              ))}
            </Dropdown>
          ))}
        </nav>

        <div className="phead-right">
          {/* site selector */}
          <Dropdown align="right" width={240}
            trigger={() => (
              <button className="site-pick">
                <Icon name="building" size={17} />
                <span style={{ textAlign: "left" }}>
                  <span className="nm" style={{ display: "block" }}>{site.name}</span>
                  <span className="pc">{site.parcel}</span>
                </span>
                <Icon name="chevron" size={14} style={{ opacity: 0.7 }} />
              </button>
            )}>
            <div className="dd-menu-label">Site / Parsel</div>
            {SITES.map((s) => (
              <MenuItem key={s.id} icon="pin" active={s.id === site.id} onClick={() => onSite(s)}>
                <span style={{ display: "block", fontWeight: 600 }}>{s.name}</span>
                <span style={{ fontSize: 11, color: "var(--faint-foreground)" }}>{s.parcel}</span>
              </MenuItem>
            ))}
          </Dropdown>

          <button className="hicon" onClick={onTheme} title="Tema değiştir">
            <Icon name={theme === "dark" ? "sun" : "moon"} size={18} />
          </button>

          <Dropdown align="right" width={140}
            trigger={() => (
              <button className="hicon" title="Dil"><TRFlag /></button>
            )}>
            <MenuItem icon="check">Türkçe</MenuItem>
            <MenuItem>English</MenuItem>
          </Dropdown>

          <Dropdown align="right" width={224}
            trigger={() => (
              <button className="avatar">{user.initials}</button>
            )}>
            <div style={{ padding: "8px 10px 10px" }}>
              <div style={{ fontWeight: 700, fontSize: 14 }}>{user.name}</div>
              <div style={{ fontSize: 12, color: "var(--muted-foreground)" }}>
                {role === "owner" ? "Daire Sahibi" : "Kiracı"} · {user.block} {user.flat}
              </div>
            </div>
            <hr className="divider" style={{ margin: "4px 0" }} />
            <MenuItem icon="user" onClick={() => onNav("ozel-profil")}>Profilim</MenuItem>
            <MenuItem icon="bell" onClick={() => onNav("ozel-bildirim")}>Bildirim Ayarları</MenuItem>
            <MenuItem icon="grid" onClick={onSwitchRole}>Rol değiştir</MenuItem>
            <hr className="divider" style={{ margin: "4px 0" }} />
            <MenuItem icon="logout" onClick={onLogout}>Çıkış yap</MenuItem>
          </Dropdown>
        </div>
      </div>
    </header>
  );
}

Object.assign(window, { PortalHeader, NAV, topKeyFor });
