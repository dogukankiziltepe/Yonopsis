import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import { UserClaims } from '@/types/auth'
import { PageDto } from '@/types/page'

interface AuthState {
  loginToken: string | null
  siteToken: string | null
  user: UserClaims | null
  pages: PageDto[]

  setLoginToken: (token: string, user: UserClaims) => void
  setSiteToken: (token: string, siteData: Pick<UserClaims, 'siteId' | 'siteName' | 'userType' | 'roleTypeId' | 'roleName'>) => void
  setPages: (pages: PageDto[]) => void
  clearPages: () => void
  clearTokens: () => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      loginToken: null,
      siteToken: null,
      user: null,
      pages: [],

      setLoginToken: (token, user) => {
        set({ loginToken: token, siteToken: null, user })
        document.cookie = 'session-stage=login; path=/; SameSite=Lax'
        if (user.mustChangePassword) {
          document.cookie = 'must-change-password=true; path=/; SameSite=Lax'
        } else {
          document.cookie = 'must-change-password=; path=/; SameSite=Lax; max-age=0'
        }
      },

      setSiteToken: (token, siteData) => {
        set((state) => ({
          siteToken: token,
          loginToken: null,
          user: state.user ? { ...state.user, ...siteData } : null,
        }))
        document.cookie = 'session-stage=site; path=/; SameSite=Lax'
        document.cookie = 'must-change-password=; path=/; SameSite=Lax; max-age=0'
      },

      setPages: (pages) => set({ pages }),

      clearPages: () => set({ pages: [] }),

      clearTokens: () => {
        set({ loginToken: null, siteToken: null, user: null, pages: [] })
        document.cookie = 'session-stage=none; path=/; SameSite=Lax; max-age=0'
        document.cookie = 'must-change-password=; path=/; SameSite=Lax; max-age=0'
      },
    }),
    {
      name: 'auth-storage',
    }
  )
)
