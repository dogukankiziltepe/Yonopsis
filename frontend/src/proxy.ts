import { NextResponse } from 'next/server'
import type { NextRequest } from 'next/server'

export function proxy(request: NextRequest) {
  const stage = request.cookies.get('session-stage')?.value ?? 'none'
  const { pathname } = request.nextUrl

  const mustChangePassword = request.cookies.get('must-change-password')?.value === 'true'

  // Auth pages
  if (pathname.startsWith('/login')) {
    if (stage === 'site') return NextResponse.redirect(new URL('/', request.url))
    if (stage === 'login') {
      if (mustChangePassword) return NextResponse.redirect(new URL('/change-password', request.url))
      return NextResponse.redirect(new URL('/site-selection', request.url))
    }
    return NextResponse.next()
  }

  if (pathname.startsWith('/site-selection')) {
    if (stage === 'site') return NextResponse.redirect(new URL('/', request.url))
    if (stage !== 'login') return NextResponse.redirect(new URL('/login', request.url))
    if (mustChangePassword) return NextResponse.redirect(new URL('/change-password', request.url))
    return NextResponse.next()
  }

  if (pathname.startsWith('/change-password')) {
    if (stage === 'none') return NextResponse.redirect(new URL('/login', request.url))
    return NextResponse.next()
  }

  // Dashboard and admin routes require site token
  if (pathname === '/' || pathname.startsWith('/buildings') || pathname.startsWith('/units')) {
    if (stage !== 'site') {
      if (stage === 'login') return NextResponse.redirect(new URL('/site-selection', request.url))
      return NextResponse.redirect(new URL('/login', request.url))
    }
    return NextResponse.next()
  }

  if (pathname.startsWith('/admin')) {
    if (stage === 'none') {
      return NextResponse.redirect(new URL('/login', request.url))
    }
    if (mustChangePassword) return NextResponse.redirect(new URL('/change-password', request.url))
    // isSuperAdmin check happens inside the layout
    return NextResponse.next()
  }

  return NextResponse.next()
}

export const config = {
  matcher: ['/((?!api|_next/static|_next/image|favicon.ico).*)'],
}
