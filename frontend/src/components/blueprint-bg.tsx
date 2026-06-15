/*
 * Blueprint network overlay — design_handoff ui.jsx BlueprintBg port'u.
 * Grid pattern + bina silüetleri + node ağı. Renk currentColor üzerinden.
 */
export function BlueprintBg({
  className,
  opacity = 0.5,
}: {
  className?: string
  opacity?: number
}) {
  return (
    <svg
      className={className}
      style={{ opacity }}
      width="100%"
      height="100%"
      viewBox="0 0 800 600"
      preserveAspectRatio="xMidYMid slice"
      fill="none"
      aria-hidden="true"
    >
      <defs>
        <pattern id="bp-grid" width="40" height="40" patternUnits="userSpaceOnUse">
          <path d="M40 0H0V40" stroke="currentColor" strokeWidth="0.5" opacity="0.4" />
        </pattern>
      </defs>
      <rect width="800" height="600" fill="url(#bp-grid)" />

      {/* Bina silüetleri */}
      <g stroke="currentColor" strokeWidth="1.2" opacity="0.7">
        <rect x="90" y="320" width="120" height="200" />
        <rect x="240" y="250" width="140" height="270" />
        <rect x="410" y="300" width="110" height="220" />
        <rect x="560" y="220" width="150" height="300" />
        <line x1="240" y1="290" x2="380" y2="290" />
        <line x1="240" y1="330" x2="380" y2="330" />
        <line x1="560" y1="270" x2="710" y2="270" />
        <line x1="560" y1="320" x2="710" y2="320" />
      </g>

      {/* Node ağı */}
      <g stroke="currentColor" strokeWidth="1" opacity="0.6">
        <line x1="150" y1="140" x2="320" y2="100" />
        <line x1="320" y1="100" x2="500" y2="160" />
        <line x1="500" y1="160" x2="650" y2="110" />
        <line x1="150" y1="140" x2="500" y2="160" />
      </g>
      <g fill="currentColor">
        <circle cx="150" cy="140" r="4" />
        <circle cx="320" cy="100" r="4" />
        <circle cx="500" cy="160" r="4" />
        <circle cx="650" cy="110" r="4" />
      </g>
    </svg>
  )
}
