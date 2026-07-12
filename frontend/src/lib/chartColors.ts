import { useEffect, useState } from 'react'

export const CATEGORICAL_LIGHT = ['#2a78d6', '#1baf7a', '#eda100', '#008300', '#4a3aa7', '#e34948', '#e87ba4', '#eb6834']
export const CATEGORICAL_DARK = ['#3987e5', '#199e70', '#c98500', '#008300', '#9085e9', '#e66767', '#d55181', '#d95926']

export const CHART_INK = {
  light: { primary: '#0b0b0b', secondary: '#52514e', muted: '#898781', grid: '#e1e0d9', axis: '#c3c2b7', surface: '#fcfcfb' },
  dark: { primary: '#ffffff', secondary: '#c3c2b7', muted: '#898781', grid: '#2c2c2a', axis: '#383835', surface: '#1a1a19' },
}

// tahsil edilen / tahsil edilemeyen — categorical slot 1 (blue) & slot 6 (red)
export const TAHSIL_EDILEN_COLOR = { light: '#2a78d6', dark: '#3987e5' }
export const TAHSIL_EDILEMEYEN_COLOR = { light: '#e34948', dark: '#e66767' }

export function useIsDarkMode() {
  const [isDark, setIsDark] = useState(false)
  useEffect(() => {
    const mq = window.matchMedia('(prefers-color-scheme: dark)')
    setIsDark(mq.matches)
    const handler = (e: MediaQueryListEvent) => setIsDark(e.matches)
    mq.addEventListener('change', handler)
    return () => mq.removeEventListener('change', handler)
  }, [])
  return isDark
}

export function useChartTheme() {
  const isDark = useIsDarkMode()
  return {
    isDark,
    categorical: isDark ? CATEGORICAL_DARK : CATEGORICAL_LIGHT,
    ink: isDark ? CHART_INK.dark : CHART_INK.light,
    tahsilEdilen: isDark ? TAHSIL_EDILEN_COLOR.dark : TAHSIL_EDILEN_COLOR.light,
    tahsilEdilemeyen: isDark ? TAHSIL_EDILEMEYEN_COLOR.dark : TAHSIL_EDILEMEYEN_COLOR.light,
  }
}
