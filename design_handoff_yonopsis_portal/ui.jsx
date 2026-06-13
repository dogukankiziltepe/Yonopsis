/* ============================================================
   Yonopsis — Paylaşılan UI (ikonlar, badge, dropdown, blueprint bg)
   ============================================================ */

/* ---------- Icons (stroke line set) ---------- */
const ICON_PATHS = {
  home:    "M3 10.5 12 3l9 7.5M5 9.5V20a1 1 0 0 0 1 1h3v-6h6v6h3a1 1 0 0 0 1-1V9.5",
  poll:    "M8 20V10M12 20V4M16 20v-7M4 20h16",
  report:  "M7 3h7l5 5v13a1 1 0 0 1-1 1H7a1 1 0 0 1-1-1V4a1 1 0 0 1 1-1ZM14 3v5h5M9 13h6M9 17h6",
  mail:    "M3 6.5A1.5 1.5 0 0 1 4.5 5h15A1.5 1.5 0 0 1 21 6.5v11A1.5 1.5 0 0 1 19.5 19h-15A1.5 1.5 0 0 1 3 17.5v-11ZM4 7l8 5.5L20 7",
  key:     "M14.5 4a5.5 5.5 0 1 0 4.9 8H22l-1.5-1.5L19 12l-1.5-1.5-1 1A5.5 5.5 0 0 0 14.5 4ZM11 11l-8 8M3 19v2.5M5.5 19H3",
  user:    "M12 12a4 4 0 1 0 0-8 4 4 0 0 0 0 8ZM5 20a7 7 0 0 1 14 0",
  wrench:  "M14.7 6.3a4 4 0 0 0-5.2 5l-6.2 6.2a1.5 1.5 0 0 0 2.1 2.1l6.2-6.2a4 4 0 0 0 5-5.2l-2.6 2.6-2.1-.5-.5-2.1 2.6-2.6Z",
  info:    "M12 21a9 9 0 1 0 0-18 9 9 0 0 0 0 18ZM12 11v5M12 8h.01",
  chevron: "M6 9l6 6 6-6",
  chevronR:"M9 6l6 6-6 6",
  sun:     "M12 17a5 5 0 1 0 0-10 5 5 0 0 0 0 10ZM12 2v2M12 20v2M4.9 4.9l1.4 1.4M17.7 17.7l1.4 1.4M2 12h2M20 12h2M4.9 19.1l1.4-1.4M17.7 6.3l1.4-1.4",
  moon:    "M21 12.8A9 9 0 1 1 11.2 3a7 7 0 0 0 9.8 9.8Z",
  search:  "M11 19a8 8 0 1 0 0-16 8 8 0 0 0 0 16ZM21 21l-4.3-4.3",
  plus:    "M12 5v14M5 12h14",
  download:"M12 3v12M7 11l5 4 5-4M5 21h14",
  filter:  "M3 5h18l-7 8v6l-4 2v-8L3 5Z",
  building: "M4 21V5a1 1 0 0 1 1-1h9a1 1 0 0 1 1 1v16M15 21V9h4a1 1 0 0 1 1 1v11M3 21h18M7 8h2M7 12h2M7 16h2M11 8h0M11 12h0M11 16h0",
  calendar:"M7 3v3M17 3v3M4 8h16M5 5h14a1 1 0 0 1 1 1v13a1 1 0 0 1-1 1H5a1 1 0 0 1-1-1V6a1 1 0 0 1 1-1Z",
  bell:    "M18 8a6 6 0 1 0-12 0c0 7-3 9-3 9h18s-3-2-3-9M13.7 21a2 2 0 0 1-3.4 0",
  car:     "M5 13l1.5-4.5A2 2 0 0 1 8.4 7h7.2a2 2 0 0 1 1.9 1.5L19 13M5 13h14v4a1 1 0 0 1-1 1h-1a1 1 0 0 1-1-1v-1H8v1a1 1 0 0 1-1 1H6a1 1 0 0 1-1-1v-4ZM7.5 15.5h.01M16.5 15.5h.01",
  doc:     "M7 3h7l5 5v13a1 1 0 0 1-1 1H7a1 1 0 0 1-1-1V4a1 1 0 0 1 1-1ZM14 3v5h5",
  logout:  "M9 21H5a1 1 0 0 1-1-1V4a1 1 0 0 1 1-1h4M16 17l5-5-5-5M21 12H9",
  check:   "M5 12.5l4.5 4.5L19 7",
  arrowUp: "M12 19V5M6 11l6-6 6 6",
  arrowDn: "M12 5v14M6 13l6 6 6-6",
  shield:  "M12 3l8 3v6c0 5-3.5 8-8 9-4.5-1-8-4-8-9V6l8-3Z",
  gauge:   "M12 14a2 2 0 1 0 0-4 2 2 0 0 0 0 4ZM12 12l3.5-3.5M5.2 17.5a8 8 0 1 1 13.6 0",
  list:    "M8 6h13M8 12h13M8 18h13M3 6h.01M3 12h.01M3 18h.01",
  grid:    "M4 4h7v7H4zM13 4h7v7h-7zM4 13h7v7H4zM13 13h7v7h-7z",
  phone:   "M5 4h4l2 5-2.5 1.5a11 11 0 0 0 5 5L20 13l5 2v4a2 2 0 0 1-2 2A16 16 0 0 1 3 6a2 2 0 0 1 2-2Z",
  pin:     "M12 21s7-6.4 7-12a7 7 0 1 0-14 0c0 5.6 7 12 7 12ZM12 11a2 2 0 1 0 0-4 2 2 0 0 0 0 4Z",
  edit:    "M4 20h4L18.5 9.5a2 2 0 0 0-2.8-2.8L5 17v3ZM14.5 6.5l3 3",
  send:    "M21 3 3 10.5l6.5 2.5M21 3l-7 18-3.5-8.5L21 3Z",
  x:       "M6 6l12 12M18 6 6 18",
  menu:    "M4 7h16M4 12h16M4 17h16",
  clock:   "M12 21a9 9 0 1 0 0-18 9 9 0 0 0 0 18ZM12 7v5l3 2",
  drop:    "M12 3s6 6.5 6 11a6 6 0 0 1-12 0c0-4.5 6-11 6-11Z",
  bolt:    "M13 2 4 14h6l-1 8 9-12h-6l1-8Z",
};

function Icon({ name, size = 18, stroke = 1.7, className = "", style = {}, fill = "none" }) {
  const d = ICON_PATHS[name];
  return (
    <svg width={size} height={size} viewBox="0 0 24 24" fill={fill}
      stroke="currentColor" strokeWidth={stroke} strokeLinecap="round" strokeLinejoin="round"
      className={className} style={{ flex: "0 0 auto", display: "block", ...style }} aria-hidden="true">
      {d.split("M").filter(Boolean).map((seg, i) => <path key={i} d={"M" + seg} />)}
    </svg>
  );
}

/* ---------- Blueprint network background (header) ---------- */
function BlueprintBg({ opacity = 0.12 }) {
  return (
    <svg className="blueprint-bg" viewBox="0 0 1200 200" preserveAspectRatio="xMidYMid slice"
      style={{ position: "absolute", inset: 0, width: "100%", height: "100%", opacity, pointerEvents: "none" }}
      aria-hidden="true">
      <defs>
        <pattern id="bpgrid" width="40" height="40" patternUnits="userSpaceOnUse">
          <path d="M40 0H0V40" fill="none" stroke="var(--blueprint-line)" strokeWidth="1" />
        </pattern>
      </defs>
      <rect width="1200" height="200" fill="url(#bpgrid)" />
      {/* building silhouettes */}
      <g fill="none" stroke="var(--blueprint-line)" strokeWidth="1.4">
        <rect x="120" y="70" width="90" height="130" />
        <rect x="140" y="92" width="16" height="16" /><rect x="174" y="92" width="16" height="16" />
        <rect x="140" y="124" width="16" height="16" /><rect x="174" y="124" width="16" height="16" />
        <rect x="140" y="156" width="16" height="16" /><rect x="174" y="156" width="16" height="16" />
        <rect x="930" y="48" width="74" height="152" />
        <rect x="948" y="70" width="13" height="13" /><rect x="975" y="70" width="13" height="13" />
        <rect x="948" y="98" width="13" height="13" /><rect x="975" y="98" width="13" height="13" />
        <rect x="948" y="126" width="13" height="13" /><rect x="975" y="126" width="13" height="13" />
        <rect x="1040" y="90" width="60" height="110" />
      </g>
      {/* network nodes + connecting lines */}
      <g stroke="var(--blueprint-line)" strokeWidth="1.1">
        <line x1="300" y1="60" x2="420" y2="120" /><line x1="420" y1="120" x2="560" y2="80" />
        <line x1="560" y1="80" x2="700" y2="140" /><line x1="700" y1="140" x2="840" y2="90" />
        <line x1="420" y1="120" x2="470" y2="40" /><line x1="700" y1="140" x2="640" y2="40" />
      </g>
      <g fill="var(--blueprint-node)">
        {[[300,60],[420,120],[560,80],[700,140],[840,90],[470,40],[640,40]].map(([x,y],i)=>(
          <circle key={i} cx={x} cy={y} r="3.2" />
        ))}
      </g>
    </svg>
  );
}

/* ---------- Yonopsis logo (house + node mark) ---------- */
function Logo({ size = 30, light = false }) {
  const stroke = light ? "#ffffff" : "var(--primary)";
  const accent = "var(--accent)";
  return (
    <svg width={size} height={size} viewBox="0 0 32 32" fill="none" aria-hidden="true" style={{ display: "block" }}>
      <path d="M5 15 16 5l11 10" stroke={stroke} strokeWidth="2.1" strokeLinecap="round" strokeLinejoin="round" />
      <path d="M7.5 13.2V25a1 1 0 0 0 1 1h15a1 1 0 0 0 1-1V13.2" stroke={stroke} strokeWidth="2.1" strokeLinecap="round" strokeLinejoin="round" />
      <circle cx="16" cy="18.5" r="2.6" fill={accent} />
      <path d="M16 21.1V26" stroke={accent} strokeWidth="2.1" strokeLinecap="round" />
    </svg>
  );
}

/* ---------- Badge ---------- */
function Badge({ tone = "neutral", children, dot = false }) {
  return <span className={"badge badge-" + tone}>{dot && <span className="dot" />}{children}</span>;
}

/* ---------- Dropdown (hover + click) ---------- */
function Dropdown({ trigger, children, align = "left", width = 220, onItem }) {
  const [open, setOpen] = React.useState(false);
  const ref = React.useRef(null);
  const timer = React.useRef(null);

  React.useEffect(() => {
    function onDoc(e) { if (ref.current && !ref.current.contains(e.target)) setOpen(false); }
    document.addEventListener("mousedown", onDoc);
    return () => document.removeEventListener("mousedown", onDoc);
  }, []);

  const enter = () => { clearTimeout(timer.current); setOpen(true); };
  const leave = () => { timer.current = setTimeout(() => setOpen(false), 140); };

  return (
    <div ref={ref} className="dd" style={{ position: "relative" }}
      onMouseEnter={enter} onMouseLeave={leave}>
      <div onClick={() => setOpen((o) => !o)}>{trigger(open)}</div>
      {open && (
        <div className="dd-menu pop-in" style={{
          position: "absolute", top: "calc(100% + 8px)",
          [align]: 0, width, zIndex: 80,
        }}
          onClick={() => { setOpen(false); }}>
          {children}
        </div>
      )}
    </div>
  );
}

function MenuItem({ icon, children, onClick, active = false, hint }) {
  return (
    <button className={"dd-item" + (active ? " active" : "")} onClick={onClick}>
      {icon && <Icon name={icon} size={16} />}
      <span className="grow" style={{ textAlign: "left" }}>{children}</span>
      {hint && <span className="dd-hint">{hint}</span>}
    </button>
  );
}

Object.assign(window, { Icon, BlueprintBg, Logo, Badge, Dropdown, MenuItem });
