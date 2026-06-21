/* ============================================================
   Yonopsis — Portal kabuğu (owner/tenant)
   ============================================================ */

function Portal({ role, theme, onTheme, onSwitchRole, onLogout, tweaks, bpOpacity, headerStyle }) {
  const [page, setPage] = React.useState("home");
  const [site, setSite] = React.useState(SITES[0]);
  const [stmtOpen, setStmtOpen] = React.useState(false);
  const bodyRef = React.useRef(null);

  const nav = (k) => {
    if (k === "hesap-dokumu") { setStmtOpen(true); return; }
    setPage(k);
    if (bodyRef.current) bodyRef.current.scrollTop = 0;
    window.scrollTo({ top: 0 });
  };

  function renderPage() {
    if (page === "home") return <Landing role={role} onNav={nav} onStatement={() => setStmtOpen(true)} />;
    if (page === "kira-odemeleri") return <KiraPage />;
    if (page === "talep-olustur") return <TalepPage create />;
    if (page === "talep-gecmis") return <TalepPage />;
    if (page === "anketler-aktif") return <AnketPage />;
    if (page === "hesap-dokumu") return <StatementPage />;
    return <GenericPage page={page} onNav={nav} />;
  }

  return (
    <div className={"portal" + (tweaks.landingStyle === "kompakt" ? " landing-compact" : "")}>
      <PortalHeader
        role={role} page={page} onNav={nav}
        theme={theme} onTheme={onTheme}
        site={site} onSite={setSite} bpOpacity={bpOpacity}
        onSwitchRole={onSwitchRole} onLogout={onLogout}
      />
      <div className="pbody" ref={bodyRef}>
        {renderPage()}
      </div>
      <PortalFooter site={site} />
      {stmtOpen && <HesapDokumuModal onClose={() => setStmtOpen(false)} />}
    </div>
  );
}

function StatementPage() {
  const [period, setPeriod] = React.useState("2026");
  return (
    <div className="pcontainer fade-in">
      <PageHead eyebrow="Kira" title="Hesap Dökümü"
        desc="Cari hesabınızın borç, alacak ve yürüyen bakiye hareketleri." />
      <StatementTable period={period} setPeriod={setPeriod} />
    </div>
  );
}

function PortalFooter({ site }) {
  return (
    <footer style={{ borderTop: "1px solid var(--border)", background: "var(--surface)" }}>
      <div style={{ maxWidth: 1320, margin: "0 auto", padding: "20px 24px", display: "flex", alignItems: "center", justifyContent: "space-between", flexWrap: "wrap", gap: 14 }}>
        <div style={{ display: "flex", alignItems: "center", gap: 10 }}>
          <Logo size={22} />
          <span style={{ fontSize: 13, color: "var(--muted-foreground)" }}>
            <b style={{ color: "var(--foreground)" }}>Yonopsis</b> · {site.name}
          </span>
        </div>
        <div style={{ display: "flex", gap: 20, fontSize: 12.5, color: "var(--muted-foreground)" }}>
          <span className="link" style={{ color: "var(--muted-foreground)" }}>Gizlilik</span>
          <span className="link" style={{ color: "var(--muted-foreground)" }}>Kullanım Şartları</span>
          <span className="link" style={{ color: "var(--muted-foreground)" }}>Yardım</span>
          <span>© 2026</span>
        </div>
      </div>
    </footer>
  );
}

Object.assign(window, { Portal });
