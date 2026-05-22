import { initials } from '../lib/ui'
import type { HashRoute } from '../lib/routing'
import type { AuthMe } from '../types'

type NavbarProps = {
  blogCount: number
  contributorCount: number
  route: HashRoute
  currentUser: AuthMe | null
  busy: string | null
  onHomeClick: () => void
  onAuthClick: () => void
  onLogout: () => void
}

export function Navbar({
  blogCount,
  contributorCount,
  route,
  currentUser,
  busy,
  onHomeClick,
  onAuthClick,
  onLogout,
}: NavbarProps) {
  return (
    <header className="shell-card overflow-hidden !p-0">
      <div className="sunrise-mesh border-b border-stone-900/8 px-5 py-4 sm:px-6">
        <nav className="flex items-center justify-between gap-4">
          <button type="button" onClick={onHomeClick} className="text-left">
            <h1 className="mt-2 font-['Cormorant_Garamond',serif] text-3xl leading-none tracking-tight text-stone-950 sm:text-4xl">
              Blog App
            </h1>
          </button>

          <div className="flex items-center gap-2">
            <button
              type="button"
              onClick={onHomeClick}
              className={`rounded-full px-4 py-2 text-xs font-semibold uppercase tracking-[0.2em] transition ${
                route.kind === 'home'
                  ? 'bg-stone-950 text-stone-50'
                  : 'border border-stone-300 bg-white/70 text-stone-700'
              }`}
            >
              Blogs
            </button>
            {currentUser ? (
              <button type="button" onClick={onLogout} disabled={busy === 'logout'} className="button-shell">
                {busy === 'logout' ? 'Signing out' : 'Logout'}
              </button>
            ) : (
              <button type="button" onClick={onAuthClick} className="button-shell">
                Login
              </button>
            )}
          </div>
        </nav>

        <div className="mt-6 grid gap-4 sm:grid-cols-[1.5fr_1fr] sm:items-end">
          <div className="grid grid-cols-2 gap-3 text-center">
            <div className="rounded-[1.4rem] border border-stone-900/10 bg-white/70 px-4 py-3">
              <p className="text-2xl font-semibold text-stone-950">{blogCount}</p>
              <p className="mt-1 text-[0.68rem] uppercase tracking-[0.24em] text-stone-500">Stories</p>
            </div>
            <div className="rounded-[1.4rem] border border-stone-900/10 bg-white/70 px-4 py-3">
              <p className="text-2xl font-semibold text-stone-950">{contributorCount}</p>
              <p className="mt-1 text-[0.68rem] uppercase tracking-[0.24em] text-stone-500">Authors</p>
            </div>
          </div>
        </div>

        {currentUser && (
          <div className="mt-5 flex items-center gap-3 rounded-[1.4rem] border border-stone-900/10 bg-white/65 px-4 py-3">
            <div className="flex h-11 w-11 items-center justify-center rounded-full bg-stone-950 font-['Cormorant_Garamond',serif] text-xl text-amber-100">
              {initials(currentUser.displayName)}
            </div>
            <div>
              <p className="font-semibold text-stone-950">{currentUser.displayName}</p>
              <p className="text-xs uppercase tracking-[0.2em] text-stone-500">
                Contributor #{currentUser.contributorId}
              </p>
            </div>
          </div>
        )}
      </div>
    </header>
  )
}
