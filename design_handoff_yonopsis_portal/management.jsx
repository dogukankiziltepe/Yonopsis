/* ============================================================
   Yonopsis — Management (Site Yöneticisi) Paneli
   ============================================================ */

const MGMT_NAV = [
  { grp: "Genel", items: [
    { key: "panel", label: "Panel", icon: "grid" },
    { key: "daireler", label: "Daireler", icon: "building", count: "96" },
    { key: "tahsilat", label: "Tahsilat", icon: "gauge" },
  ]},
  { grp: "Finans", items: [
    { key: "aidat", label: "Aidat", icon: "report" },
    { key: "giderler", label: "Giderler", icon: "list" },
    { key: "raporlar", label: "Raporlar", icon: "gauge" },
  ]},
  { grp: "Operasyon", items: [
    { key: "talepler", label: "Talepler", icon: "wrench", count: "14" },
    { key: "bakim", label: "Bakım Takvimi", icon: "calendar" },
    { key: "duyurular", label: "Duyurular", icon: "bell" },
  ]},
  { grp: "Site", items: [
    { key: "anketler", label: "Anketler", icon: "poll" },
    { key: "belgeler", label: "Belgeler", icon: "doc" },
    { key: "ayarlar", label: "Ayarlar", icon: "user" },
  ]},
];

const MGMT_TITLES = {
  panel: "Yönetim Paneli", daireler: "Daireler", tahsilat: "Tahsilat Takibi",
  aidat: "Aidat Yönetimi", giderler: "Giderler", raporlar: "Raporlar",
  talepler: "Talepler & İş Emirleri", bakim: "Bakım Takvimi", duyurular: "Duyurular",
  anketler: "Anketler", belgeler: "Belgeler", ayarlar: "Ayarlar",
};

function Management({ role, theme, onTheme, onSwitchRole, onLogout, site, setSite }) {
  const [page, setPage] = React.useState("panel");
  const user = CURRENT_USER.management;

  return (
    <div className="mgmt">
      {/* Sidebar */}
      <aside className="msidebar">
        <div className="msb-brand">
          <Logo size={26} />
          <span className="nm">Yonopsis</span>
          <span className="badge badge-primary" style={{ marginLeft: "auto", fontSize: 10 }}>Yönetim</span>
        </div>
        <div className="msb-site">
          <Dropdown width={208}
            trigger={() => (
              <button className="btn btn-outline btn-sm" style={{ width: "100%", justifyContent: "space-between" }}>
                <span style={{ display: "flex", alignItems: "center", gap: 8, overflow: "hidden" }}>
                  <Icon name="building" size={15} />
                  <span style={{ whiteSpace: "nowrap", overflow: "hidden", textOverflow: "ellipsis" }}>{site.name}</span>
                </span>
                <Icon name="chevron" size={14} />
              </button>
            )}>
            <div className="dd-menu-label">Site / Parsel</div>
            {SITES.map((s) => (
              <MenuItem key={s.id} icon="pin" active={s.id === site.id} onClick={() => setSite(s)}>{s.name}</MenuItem>
            ))}
          </Dropdown>
        </div>
        <nav className="msb-nav">
          {MGMT_NAV.map((g) => (
            <React.Fragment key={g.grp}>
              <div className="grp">{g.grp}</div>
              {g.items.map((it) => (
                <button key={it.key} className={"msb-item" + (page === it.key ? " active" : "")} onClick={() => setPage(it.key)}>
                  <Icon name={it.icon} size={17} />
                  <span>{it.label}</span>
                  {it.count && <span className="count num">{it.count}</span>}
                </button>
              ))}
            </React.Fragment>
          ))}
        </nav>
        <div className="msb-foot">
          <Dropdown width={208}
            trigger={() => (
              <button className="msb-item" style={{ background: "var(--surface-2)" }}>
                <span className="avatar" style={{ width: 28, height: 28, fontSize: 11, border: 0 }}>{user.initials}</span>
                <span style={{ textAlign: "left", lineHeight: 1.2 }}>
                  <span style={{ display: "block", fontWeight: 700, fontSize: 12.5, color: "var(--foreground)" }}>{user.name}</span>
                  <span style={{ fontSize: 11, color: "var(--muted-foreground)" }}>{user.role}</span>
                </span>
                <Icon name="chevron" size={14} style={{ marginLeft: "auto" }} />
              </button>
            )}>
            <MenuItem icon="grid" onClick={onSwitchRole}>Rol değiştir</MenuItem>
            <MenuItem icon={theme === "dark" ? "sun" : "moon"} onClick={onTheme}>Tema: {theme === "dark" ? "Açık" : "Koyu"}</MenuItem>
            <hr className="divider" style={{ margin: "4px 0" }} />
            <MenuItem icon="logout" onClick={onLogout}>Çıkış yap</MenuItem>
          </Dropdown>
        </div>
      </aside>

      {/* Main */}
      <div className="mmain">
        <div className="mtopbar">
          <h1>{MGMT_TITLES[page] || "Panel"}</h1>
          <div className="search" style={{ marginLeft: 18 }}>
            <Icon name="search" size={16} />
            <input className="input" placeholder="Daire, sakin veya talep ara..." />
          </div>
          <div style={{ marginLeft: "auto", display: "flex", gap: 8 }}>
            <button className="btn btn-outline btn-sm btn-icon"><Icon name="bell" size={16} /></button>
            <button className="btn btn-outline btn-sm btn-icon" onClick={onTheme}><Icon name={theme === "dark" ? "sun" : "moon"} size={16} /></button>
            <button className="btn btn-accent btn-sm"><Icon name="plus" size={15} /> Yeni kayıt</button>
          </div>
        </div>
        <div className="mcontent">
          {page === "panel" && <MgmtDashboard setPage={setPage} />}
          {page === "daireler" && <MgmtResidents />}
          {page === "giderler" && <MgmtExpenses />}
          {page === "talepler" && <MgmtTasks />}
          {!["panel", "daireler", "giderler", "talepler"].includes(page) && <MgmtGeneric page={page} />}
        </div>
      </div>
    </div>
  );
}

/* ---------- Dashboard ---------- */
function MgmtDashboard({ setPage }) {
  return (
    <div className="fade-in">
      <div className="kpi-grid">
        {MGMT_KPIS.map((k) => (
          <div key={k.id} className="kpi">
            <div className="l">{k.label}</div>
            <div className="v num">{k.value}</div>
            <div className={"d " + (k.trend === "up" ? "up" : "down")}>
              <Icon name={k.trend === "up" ? "arrowUp" : "arrowDn"} size={14} />
              <span className="num">{k.delta}</span>
              <span className="sub">{k.sub}</span>
            </div>
          </div>
        ))}
      </div>

      <div className="mgrid">
        <div className="panel">
          <div className="panel-head">
            <div><h3>Daire Tahakkuk Durumu</h3><div className="sub">Haziran 2026 · ilk 12 daire</div></div>
            <button className="btn btn-ghost btn-sm" onClick={() => setPage("daireler")}>Tümü <Icon name="chevronR" size={14} /></button>
          </div>
          <table className="tbl compact">
            <thead><tr><th style={{ width: 70 }}>Daire</th><th>Sakin</th><th>Tip</th><th className="r">Aidat</th><th className="r" style={{ width: 110 }}>Durum</th></tr></thead>
            <tbody>
              {MGMT_RESIDENTS.slice(0, 7).map((r) => (
                <tr key={r.flat}>
                  <td><span className="flat-tag">{r.flat}</span></td>
                  <td style={{ fontWeight: 600 }}>{r.name}</td>
                  <td><Badge tone={r.type === "Sahip" ? "primary" : "neutral"}>{r.type}</Badge></td>
                  <td className="r num">{fmtTRY(r.dues)}</td>
                  <td className="r"><Badge tone={r.status} dot>{r.label}</Badge></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        <div style={{ display: "flex", flexDirection: "column", gap: 16 }}>
          <div className="panel" style={{ padding: "6px 18px 14px" }}>
            <div className="panel-head" style={{ padding: "12px 0", borderBottom: "1px solid var(--border)" }}>
              <h3>Blok Doluluk</h3>
            </div>
            {MGMT_BLOCKS.map((b) => (
              <div key={b.id} className="block-stat">
                <div style={{ display: "flex", alignItems: "center", gap: 12 }}>
                  <span className="block-badge">{b.id}</span>
                  <div>
                    <div style={{ fontWeight: 700, fontSize: 13.5 }}>{b.id} Blok</div>
                    <div style={{ fontSize: 12, color: "var(--muted-foreground)" }} className="num">{b.occupied}/{b.flats} dolu</div>
                  </div>
                </div>
                <div style={{ width: 90 }}>
                  <div className="bar"><span style={{ width: (b.occupied / b.flats * 100) + "%" }} /></div>
                </div>
              </div>
            ))}
          </div>

          <div className="panel" style={{ padding: "6px 18px 14px" }}>
            <div className="panel-head" style={{ padding: "12px 0", borderBottom: "1px solid var(--border)" }}>
              <h3>Açık İş Emirleri</h3>
              <button className="btn btn-ghost btn-sm" onClick={() => setPage("talepler")}>Tümü</button>
            </div>
            {MGMT_TASKS.map((t) => (
              <div key={t.id} className="task-item">
                <span className="block-badge" style={{ background: "var(--surface-2)", color: "var(--muted-foreground)" }}>
                  <Icon name="wrench" size={15} />
                </span>
                <div style={{ flex: 1, minWidth: 0 }}>
                  <div className="s" style={{ whiteSpace: "nowrap", overflow: "hidden", textOverflow: "ellipsis" }}>{t.subject}</div>
                  <div className="meta"><span className="id">{t.id}</span> · {t.assignee} · {t.due}</div>
                </div>
                <Badge tone={t.status} dot>{t.label}</Badge>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}

/* ---------- Residents DataTable ---------- */
function MgmtResidents() {
  const [filter, setFilter] = React.useState("all");
  const [q, setQ] = React.useState("");
  let rows = MGMT_RESIDENTS;
  if (filter !== "all") rows = rows.filter((r) => r.status === filter);
  if (q) rows = rows.filter((r) => (r.name + r.flat).toLowerCase().includes(q.toLowerCase()));

  const counts = {
    all: MGMT_RESIDENTS.length,
    success: MGMT_RESIDENTS.filter((r) => r.status === "success").length,
    warning: MGMT_RESIDENTS.filter((r) => r.status === "warning").length,
    destructive: MGMT_RESIDENTS.filter((r) => r.status === "destructive").length,
  };

  return (
    <div className="fade-in">
      <div className="toolbar" style={{ justifyContent: "space-between" }}>
        <div className="seg">
          {[["all", "Tümü"], ["success", "Ödenen"], ["warning", "Bekleyen"], ["destructive", "Gecikmiş"]].map(([k, l]) => (
            <button key={k} className={filter === k ? "on" : ""} onClick={() => setFilter(k)}>{l} <span className="num" style={{ opacity: 0.6 }}>{counts[k]}</span></button>
          ))}
        </div>
        <div style={{ display: "flex", gap: 10 }}>
          <div className="search" style={{ position: "relative", width: 240 }}>
            <Icon name="search" size={16} style={{ position: "absolute", left: 11, top: "50%", transform: "translateY(-50%)", color: "var(--faint-foreground)" }} />
            <input className="input" style={{ paddingLeft: 35, height: 36 }} placeholder="Ara..." value={q} onChange={(e) => setQ(e.target.value)} />
          </div>
          <button className="btn btn-outline btn-sm"><Icon name="filter" size={15} /> Filtre</button>
          <button className="btn btn-outline btn-sm"><Icon name="download" size={15} /> Dışa aktar</button>
        </div>
      </div>

      <div className="tbl-wrap">
        <table className="tbl compact">
          <thead><tr>
            <th style={{ width: 70 }}>Daire</th><th>Sakin</th><th>Tip</th><th>Telefon</th>
            <th className="r">Aidat</th><th className="r">Ödenen</th><th className="r" style={{ width: 110 }}>Durum</th><th style={{ width: 40 }}></th>
          </tr></thead>
          <tbody>
            {rows.map((r) => (
              <tr key={r.flat}>
                <td><span className="flat-tag">{r.flat}</span></td>
                <td style={{ fontWeight: 600 }}>{r.name}</td>
                <td><Badge tone={r.type === "Sahip" ? "primary" : "neutral"}>{r.type}</Badge></td>
                <td className="num" style={{ color: "var(--muted-foreground)" }}>{r.phone}</td>
                <td className="r num">{fmtTRY(r.dues)}</td>
                <td className={"r num " + (r.paid >= r.dues ? "pos" : r.paid === 0 ? "neg" : "")}>{fmtTRY(r.paid)}</td>
                <td className="r"><Badge tone={r.status} dot>{r.label}</Badge></td>
                <td><button className="btn btn-ghost btn-sm btn-icon"><Icon name="chevronR" size={15} /></button></td>
              </tr>
            ))}
          </tbody>
        </table>
        {rows.length === 0 && <div className="empty"><p>Eşleşen daire bulunamadı.</p></div>}
        <div className="tbl-foot">
          <span style={{ color: "var(--muted-foreground)" }}>{rows.length} daire gösteriliyor</span>
          <span className="sum">Toplam tahakkuk <span className="num">{fmtTRY(rows.reduce((s, r) => s + r.dues, 0))}</span></span>
        </div>
      </div>
    </div>
  );
}

/* ---------- Expenses ---------- */
function MgmtExpenses() {
  const total = MGMT_EXPENSES.reduce((s, e) => s + e.amount, 0);
  return (
    <div className="fade-in">
      <div className="toolbar" style={{ justifyContent: "space-between" }}>
        <div className="seg">
          <button className="on">Haziran 2026</button><button>Mayıs 2026</button><button>Çeyrek</button>
        </div>
        <button className="btn btn-accent btn-sm"><Icon name="plus" size={15} /> Gider ekle</button>
      </div>
      <div className="tbl-wrap">
        <table className="tbl compact">
          <thead><tr><th style={{ width: 110 }}>Tarih</th><th>Açıklama</th><th>Kategori</th><th className="r" style={{ width: 160 }}>Tutar</th></tr></thead>
          <tbody>
            {MGMT_EXPENSES.map((e, i) => (
              <tr key={i}>
                <td className="num" style={{ color: "var(--muted-foreground)" }}>{e.date}</td>
                <td style={{ fontWeight: 600 }}>{e.desc}</td>
                <td><Badge tone="neutral">{e.cat}</Badge></td>
                <td className="r num neg" style={{ fontWeight: 700 }}>−{fmtTRY(e.amount, false)}</td>
              </tr>
            ))}
          </tbody>
        </table>
        <div className="tbl-foot">
          <span style={{ color: "var(--muted-foreground)" }}>{MGMT_EXPENSES.length} gider kalemi</span>
          <span className="sum">Toplam gider <span className="num neg">−{fmtTRY(total, false)} ₺</span></span>
        </div>
      </div>
    </div>
  );
}

/* ---------- Tasks ---------- */
function MgmtTasks() {
  return (
    <div className="fade-in">
      <div className="toolbar" style={{ justifyContent: "space-between" }}>
        <div className="seg"><button className="on">Açık</button><button>Bana atanan</button><button>Tümü</button></div>
        <button className="btn btn-accent btn-sm"><Icon name="plus" size={15} /> İş emri</button>
      </div>
      <div className="tbl-wrap">
        <table className="tbl compact">
          <thead><tr><th style={{ width: 100 }}>No</th><th>Konu</th><th>Atanan</th><th>Termin</th><th className="r" style={{ width: 120 }}>Durum</th></tr></thead>
          <tbody>
            {MGMT_TASKS.concat(REQUESTS.filter(r => r.status === "success").map(r => ({ id: r.id, subject: r.subject, assignee: "Tesisat ekibi", status: "success", label: "Çözüldü", due: r.date }))).map((t) => (
              <tr key={t.id}>
                <td><span className="flat-tag">{t.id}</span></td>
                <td style={{ fontWeight: 600 }}>{t.subject}</td>
                <td>{t.assignee}</td>
                <td className="num" style={{ color: "var(--muted-foreground)" }}>{t.due}</td>
                <td className="r"><Badge tone={t.status} dot>{t.label}</Badge></td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

function MgmtGeneric({ page }) {
  return (
    <div className="fade-in">
      <div className="panel"><div className="empty" style={{ padding: "80px 24px" }}>
        <div className="ic" style={{ width: 60, height: 60 }}><Icon name="grid" size={26} /></div>
        <h3>{MGMT_TITLES[page]}</h3>
        <p style={{ maxWidth: "46ch", margin: "0 auto" }}>
          Bu modül yoğun, tablo + form ağırlıklı yönetici düzeniyle aynı token sistemi üzerine kurulacak.
          Öncelikli ekranları (Panel, Daireler, Giderler, Talepler) prototipte çalışır durumda görebilirsiniz.
        </p>
      </div></div>
    </div>
  );
}

Object.assign(window, { Management });
