/* ============================================================
   Yonopsis — Portal Sayfaları (landing, hesap dökümü, kira, vb.)
   ============================================================ */

function greeting() {
  const h = new Date().getHours();
  if (h < 6) return "İyi geceler";
  if (h < 12) return "Günaydın";
  if (h < 18) return "İyi günler";
  return "İyi akşamlar";
}

/* ---------- Landing ---------- */
function Landing({ role, onNav, onStatement }) {
  const user = CURRENT_USER[role] || CURRENT_USER.owner;
  const d = DUES_SUMMARY;
  const openReq = REQUESTS.filter((r) => r.status !== "success").length;

  return (
    <div className="pcontainer fade-in">
      {/* welcome */}
      <div className="welcome">
        <div>
          <h1>{greeting()}, <span className="accent">{user.name.split(" ")[0]}</span> 👋</h1>
          <div className="welcome-chips">
            <span className="chip"><Icon name="building" size={14} /> {SITE.name}</span>
            <span className="chip"><Icon name="home" size={14} /> {user.block} · {user.flat}</span>
            <span className="chip"><Icon name="pin" size={14} /> {SITE.city}</span>
          </div>
        </div>
        <div className="stat-chips">
          <div className="stat-chip">
            <div className="l"><Icon name="gauge" size={14} /> Güncel Bakiye</div>
            <div className="v neg num">{fmtTRY(d.balance)}</div>
          </div>
          <div className="stat-chip">
            <div className="l"><Icon name="calendar" size={14} /> Sonraki Vade</div>
            <div className="v" style={{ fontSize: 16 }}>{d.dueDate}</div>
          </div>
          <div className="stat-chip">
            <div className="l"><Icon name="wrench" size={14} /> Açık Talep</div>
            <div className="v num">{openReq}</div>
          </div>
        </div>
      </div>

      <div className="portal-grid">
        {/* announcements */}
        <section>
          <div className="section-title">
            <h2>Duyurular</h2>
            <button className="btn btn-ghost btn-sm" onClick={() => onNav("genel-belge")}>Tümü <Icon name="chevronR" size={14} /></button>
          </div>
          {ANNOUNCEMENTS.length === 0 ? (
            <div className="card"><div className="empty">
              <div className="ic"><Icon name="bell" size={22} /></div>
              <h3>Henüz duyuru yok</h3>
              <p>Yönetim bir duyuru yayınladığında burada görünecek.</p>
            </div></div>
          ) : (
            <div className="feed">
              {ANNOUNCEMENTS.map((a, i) => (
                <article key={a.id} className="ann fade-up" style={{ animationDelay: i * 50 + "ms" }}>
                  <div className="ann-top">
                    <Badge tone={a.tone} dot>{a.cat}</Badge>
                    <span className="ann-date">{a.date}</span>
                  </div>
                  <h3>{a.title}</h3>
                  <p>{a.body}</p>
                </article>
              ))}
            </div>
          )}
        </section>

        {/* sidebar: dues + quick actions */}
        <aside style={{ display: "flex", flexDirection: "column", gap: 24 }}>
          <div>
            <div className="section-title"><h2>Son Aidat Bilgileri</h2></div>
            <div className="dues-card">
              <div className="dues-head">
                <span className="ttl"><Icon name="gauge" size={16} /> {d.monthLabel}</span>
                <Badge tone={d.status} dot>{d.statusLabel}</Badge>
              </div>
              <div className="dues-amount">
                <div className="lbl">Bu ayki aidat tutarı</div>
                <div className="big num">{fmtTRY(d.amount)}</div>
              </div>
              <div className="dues-rows">
                <div className="dues-r">
                  <span className="k"><Icon name="calendar" size={15} /> Son ödeme tarihi</span>
                  <span className="vv">{d.dueDate}</span>
                </div>
                <div className="dues-r">
                  <span className="k"><Icon name="gauge" size={15} /> Güncel bakiye</span>
                  <span className="vv neg num">{fmtTRY(d.balance)}</span>
                </div>
                {role === "owner" && (
                  <div className="dues-r">
                    <span className="k"><Icon name="key" size={15} /> Kira geliri (Haz)</span>
                    <span className="vv pos num" style={{ color: "var(--success-fg)" }}>{fmtTRY(d.rentIncome)}</span>
                  </div>
                )}
              </div>
              <div className="dues-foot">
                <button className="btn btn-accent" onClick={onStatement}>
                  <Icon name="report" size={17} /> Hesap Dökümü
                </button>
              </div>
            </div>
          </div>

          {/* quick actions */}
          <div>
            <div className="section-title"><h2>Hızlı İşlemler</h2></div>
            <div className="card" style={{ padding: 8 }}>
              {[
                { k: "talep-olustur", i: "plus", t: "Talep oluştur" },
                { k: "teknik-ariza", i: "wrench", t: "Arıza bildir" },
                role === "owner"
                  ? { k: "anketler-aktif", i: "poll", t: "Aktif anketler" }
                  : { k: "kira-odemeleri", i: "key", t: "Kira ödemeleri" },
                { k: "iletisim-mesaj", i: "send", t: "Yönetime mesaj" },
              ].map((q) => (
                <button key={q.k} className="dd-item" onClick={() => onNav(q.k)} style={{ padding: "11px 12px" }}>
                  <Icon name={q.i} size={17} />
                  <span className="grow" style={{ textAlign: "left", fontWeight: 600 }}>{q.t}</span>
                  <Icon name="chevronR" size={15} />
                </button>
              ))}
            </div>
          </div>
        </aside>
      </div>
    </div>
  );
}

/* ---------- Hesap Dökümü (tablo) ---------- */
function StatementTable({ period, setPeriod }) {
  const rows = STATEMENT.filter((r) => r.debit || r.credit);
  const totalDebit = rows.reduce((s, r) => s + r.debit, 0);
  const totalCredit = rows.reduce((s, r) => s + r.credit, 0);
  const finalBal = STATEMENT[STATEMENT.length - 1].balance;

  return (
    <div>
      <div className="toolbar" style={{ justifyContent: "space-between" }}>
        <div className="seg">
          {["2026", "Son 6 Ay", "Tümü"].map((p) => (
            <button key={p} className={period === p ? "on" : ""} onClick={() => setPeriod(p)}>{p}</button>
          ))}
        </div>
        <button className="btn btn-outline btn-sm"><Icon name="download" size={16} /> PDF indir</button>
      </div>

      {rows.length === 0 ? (
        <div className="tbl-wrap"><div className="empty">
          <div className="ic"><Icon name="report" size={22} /></div>
          <h3>Bu dönemde hareket yok</h3>
          <p>Seçilen dönem için kayıtlı bir hesap hareketi bulunmuyor.</p>
        </div></div>
      ) : (
        <div className="tbl-wrap">
          <table className="tbl">
            <thead>
              <tr>
                <th style={{ width: 110 }}>Tarih</th>
                <th>Açıklama</th>
                <th className="r" style={{ width: 130 }}>Borç</th>
                <th className="r" style={{ width: 130 }}>Alacak</th>
                <th className="r" style={{ width: 140 }}>Bakiye</th>
              </tr>
            </thead>
            <tbody>
              {rows.map((r, i) => (
                <tr key={i}>
                  <td className="num" style={{ color: "var(--muted-foreground)" }}>{r.date}</td>
                  <td style={{ fontWeight: 500 }}>{r.desc}</td>
                  <td className="r num neg">{r.debit ? "−" + fmtTRY(r.debit, false) : "—"}</td>
                  <td className="r num pos">{r.credit ? "+" + fmtTRY(r.credit, false) : "—"}</td>
                  <td className={"r num bal " + (r.balance < 0 ? "neg" : r.balance > 0 ? "pos" : "")}>
                    {fmtTRY(r.balance, false)}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
          <div className="tbl-foot">
            <span style={{ color: "var(--muted-foreground)" }}>{rows.length} hareket · {period}</span>
            <div style={{ display: "flex", gap: 26 }}>
              <span>Toplam borç <span className="num neg" style={{ fontWeight: 700 }}>−{fmtTRY(totalDebit, false)}</span></span>
              <span>Toplam alacak <span className="num pos" style={{ fontWeight: 700 }}>+{fmtTRY(totalCredit, false)}</span></span>
              <span className="sum">Güncel bakiye <span className={"num " + (finalBal < 0 ? "neg" : "pos")}>{fmtTRY(finalBal, false)} ₺</span></span>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

function HesapDokumuModal({ onClose }) {
  const [period, setPeriod] = React.useState("2026");
  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal" onClick={(e) => e.stopPropagation()} style={{ maxWidth: 880 }}>
        <div className="modal-head">
          <div>
            <h2><Icon name="report" size={20} /> Hesap Dökümü</h2>
            <p>{SITE.name} · B Blok Daire 12 · Cari hesap hareketleri</p>
          </div>
          <button className="hicon" style={{ background: "var(--surface-2)", color: "var(--foreground)" }} onClick={onClose}>
            <Icon name="x" size={18} />
          </button>
        </div>
        <div className="modal-body">
          <StatementTable period={period} setPeriod={setPeriod} />
        </div>
      </div>
    </div>
  );
}

Object.assign(window, { Landing, StatementTable, HesapDokumuModal, greeting });
