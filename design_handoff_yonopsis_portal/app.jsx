/* ============================================================
   Yonopsis — App (routing + tema + tweaks)
   ============================================================ */

const TWEAK_DEFAULTS = /*EDITMODE-BEGIN*/{
  "accent": "#e08a3c",
  "primaryTone": "#0f2746",
  "blueprintOpacity": 0.12,
  "landingStyle": "ferah",
  "radius": "yumusak",
  "headerStyle": "gradient"
}/*EDITMODE-END*/;

const RADII = {
  keskin:  ["4px", "6px", "9px", "12px"],
  yumusak: ["7px", "11px", "16px", "22px"],
  yuvarlak:["10px", "15px", "21px", "28px"],
};

function App() {
  const [t, setTweak] = useTweaks(TWEAK_DEFAULTS);
  const [screen, setScreen] = React.useState("login"); // login | roles | portal | mgmt
  const [role, setRole] = React.useState("owner");
  const [theme, setTheme] = React.useState(() => localStorage.getItem("yon_theme") || "light");
  const [site, setSite] = React.useState(SITES[0]);

  // theme class
  React.useEffect(() => {
    document.documentElement.classList.toggle("dark", theme === "dark");
    localStorage.setItem("yon_theme", theme);
  }, [theme]);

  // tweak -> CSS vars
  React.useEffect(() => {
    const r = document.documentElement;
    r.style.setProperty("--accent", t.accent);
    r.style.setProperty("--accent-600", `color-mix(in srgb, ${t.accent}, #000 14%)`);
    r.style.setProperty("--accent-soft", `color-mix(in srgb, ${t.accent} 15%, var(--surface))`);
    r.style.setProperty("--accent-soft-fg", `color-mix(in srgb, ${t.accent}, #000 32%)`);
    r.style.setProperty("--ring", `color-mix(in srgb, ${t.accent} 42%, transparent)`);

    r.style.setProperty("--primary", t.primaryTone);
    r.style.setProperty("--primary-grad-1", `color-mix(in srgb, ${t.primaryTone}, #000 14%)`);
    r.style.setProperty("--primary-grad-2", `color-mix(in srgb, ${t.primaryTone}, #fff 20%)`);
    r.style.setProperty("--primary-soft", `color-mix(in srgb, ${t.primaryTone} 9%, var(--surface))`);
    r.style.setProperty("--primary-soft-fg", `color-mix(in srgb, ${t.primaryTone}, #fff 8%)`);

    const rad = RADII[t.radius] || RADII.yumusak;
    r.style.setProperty("--radius-sm", rad[0]);
    r.style.setProperty("--radius-md", rad[1]);
    r.style.setProperty("--radius-lg", rad[2]);
    r.style.setProperty("--radius-xl", rad[3]);
  }, [t.accent, t.primaryTone, t.radius]);

  const toggleTheme = () => setTheme((x) => (x === "dark" ? "light" : "dark"));

  let view;
  if (screen === "login") {
    view = <LoginScreen onLogin={() => setScreen("roles")} />;
  } else if (screen === "roles") {
    view = <RoleSelect
      onPick={(rk) => { setRole(rk); setScreen(rk === "management" ? "mgmt" : "portal"); }}
      onBack={() => setScreen("login")} />;
  } else if (screen === "mgmt") {
    view = <Management role={role} theme={theme} onTheme={toggleTheme}
      site={site} setSite={setSite}
      onSwitchRole={() => setScreen("roles")} onLogout={() => setScreen("login")} />;
  } else {
    view = <Portal role={role} theme={theme} onTheme={toggleTheme}
      tweaks={t}
      bpOpacity={t.blueprintOpacity} headerStyle={t.headerStyle}
      onSwitchRole={() => setScreen("roles")} onLogout={() => setScreen("login")} />;
  }

  return (
    <React.Fragment>
      {view}
      <TweaksPanel>
        <TweakSection label="Marka renkleri" />
        <TweakColor label="Accent" value={t.accent}
          options={["#e08a3c", "#2f9e7a", "#3b82c4", "#c2475a", "#7c6cd6"]}
          onChange={(v) => setTweak("accent", v)} />
        <TweakColor label="Primary lacivert" value={t.primaryTone}
          options={["#0f2746", "#16335c", "#22357a", "#1b3b6f", "#102a3c"]}
          onChange={(v) => setTweak("primaryTone", v)} />

        <TweakSection label="Header" />
        <TweakSlider label="Blueprint dokusu" value={t.blueprintOpacity} min={0.04} max={0.30} step={0.02}
          onChange={(v) => setTweak("blueprintOpacity", v)} />

        <TweakSection label="Düzen" />
        <TweakRadio label="Landing ritmi" value={t.landingStyle} options={["ferah", "kompakt"]}
          onChange={(v) => setTweak("landingStyle", v)} />
        <TweakRadio label="Köşe yuvarlaklığı" value={t.radius} options={["keskin", "yumusak", "yuvarlak"]}
          onChange={(v) => setTweak("radius", v)} />
      </TweaksPanel>
    </React.Fragment>
  );
}

ReactDOM.createRoot(document.getElementById("root")).render(<App />);
