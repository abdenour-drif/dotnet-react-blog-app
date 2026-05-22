import { Field } from '../components/Field'
import { QuickPrefillButton } from '../components/QuickPrefillButton'
import { initials } from '../lib/ui'
import type { DemoAccount } from '../constants/demoAccounts'
import type { AuthMe, LoginForm, RegisterForm } from '../types'

type AuthPageProps = {
  currentUser: AuthMe | null
  authMode: 'login' | 'register'
  busy: string | null
  loginForm: LoginForm
  registerForm: RegisterForm
  demoAccounts: readonly DemoAccount[]
  onAuthModeChange: (mode: 'login' | 'register') => void
  onLoginFormChange: React.Dispatch<React.SetStateAction<LoginForm>>
  onRegisterFormChange: React.Dispatch<React.SetStateAction<RegisterForm>>
  onLogin: (event: React.FormEvent<HTMLFormElement>) => void
  onRegister: (event: React.FormEvent<HTMLFormElement>) => void
  onLogout: () => void
  onBackHome: () => void
}

export function AuthPage({
  currentUser,
  authMode,
  busy,
  loginForm,
  registerForm,
  demoAccounts,
  onAuthModeChange,
  onLoginFormChange,
  onRegisterFormChange,
  onLogin,
  onRegister,
  onLogout,
  onBackHome,
}: AuthPageProps) {
  return (
    <section className="grid gap-6 lg:grid-cols-[1.15fr_0.85fr]">
      <div className="shell-card">
        <div className="flex items-center justify-between gap-3">
          <div>
            <p className="text-[0.68rem] font-semibold uppercase tracking-[0.3em] text-stone-500">
              Access
            </p>
            <h2 className="mt-2 font-['Cormorant_Garamond',serif] text-3xl leading-none text-stone-950">
              {currentUser ? 'Session active' : authMode === 'login' ? 'Login' : 'Register'}
            </h2>
          </div>
          {!currentUser && (
            <div className="rounded-full border border-stone-200 bg-stone-100 p-1">
              <button
                type="button"
                onClick={() => onAuthModeChange('login')}
                className={`rounded-full px-3 py-1 text-xs font-semibold uppercase tracking-[0.2em] ${
                  authMode === 'login' ? 'bg-stone-950 text-stone-50' : 'text-stone-600'
                }`}
              >
                Login
              </button>
              <button
                type="button"
                onClick={() => onAuthModeChange('register')}
                className={`rounded-full px-3 py-1 text-xs font-semibold uppercase tracking-[0.2em] ${
                  authMode === 'register' ? 'bg-stone-950 text-stone-50' : 'text-stone-600'
                }`}
              >
                Register
              </button>
            </div>
          )}
        </div>

        {currentUser ? (
          <div className="mt-6 rounded-[1.5rem] border border-amber-200 bg-amber-50/90 p-5">
            <div className="flex items-center gap-4">
              <div className="flex h-16 w-16 items-center justify-center rounded-full bg-stone-950 font-['Cormorant_Garamond',serif] text-3xl text-amber-100">
                {initials(currentUser.displayName)}
              </div>
              <div>
                <p className="text-lg font-semibold text-stone-950">{currentUser.displayName}</p>
                <p className="text-sm text-stone-600">{currentUser.email}</p>
                <p className="mt-1 text-[0.68rem] uppercase tracking-[0.24em] text-stone-500">
                  Contributor #{currentUser.contributorId}
                </p>
              </div>
            </div>
            <div className="mt-5 flex flex-wrap gap-3">
              <button type="button" className="button-shell" onClick={onBackHome}>
                Back to blogs
              </button>
              <button
                type="button"
                className="rounded-full border border-stone-300 bg-white px-4 py-3 text-xs font-semibold uppercase tracking-[0.2em] text-stone-700"
                onClick={onLogout}
              >
                Logout
              </button>
            </div>
          </div>
        ) : authMode === 'login' ? (
          <form className="mt-6 grid gap-4" onSubmit={onLogin}>
            <div className="flex flex-wrap gap-2">
              {demoAccounts.map((account) => (
                <QuickPrefillButton
                  key={account.label}
                  label={`Login ${account.label}`}
                  onClick={() => onLoginFormChange(account.login)}
                />
              ))}
            </div>

            <Field label="Email">
              <input
                className="input-shell"
                type="email"
                value={loginForm.email}
                onChange={(event) => onLoginFormChange((current) => ({ ...current, email: event.target.value }))}
                placeholder="mara.vale@example.com"
                required
              />
            </Field>

            <Field label="Password">
              <input
                className="input-shell"
                type="password"
                value={loginForm.password}
                onChange={(event) =>
                  onLoginFormChange((current) => ({ ...current, password: event.target.value }))
                }
                placeholder="Use one of the preset passwords"
                required
              />
            </Field>

            <button className="button-shell mt-2" disabled={busy === 'login'} type="submit">
              {busy === 'login' ? 'Signing in' : 'Sign in'}
            </button>
          </form>
        ) : (
          <form className="mt-6 grid gap-4" onSubmit={onRegister}>
            <div className="flex flex-wrap gap-2">
              {demoAccounts.map((account) => (
                <QuickPrefillButton
                  key={account.label}
                  label={`Fill ${account.label}`}
                  onClick={() => onRegisterFormChange(account.register)}
                />
              ))}
            </div>

            <div className="grid gap-4 sm:grid-cols-2">
              <Field label="Display name">
                <input
                  className="input-shell"
                  value={registerForm.displayName}
                  onChange={(event) =>
                    onRegisterFormChange((current) => ({ ...current, displayName: event.target.value }))
                  }
                  placeholder="Mara Vale"
                  required
                />
              </Field>
              <Field label="Email">
                <input
                  className="input-shell"
                  type="email"
                  value={registerForm.email}
                  onChange={(event) =>
                    onRegisterFormChange((current) => ({ ...current, email: event.target.value }))
                  }
                  placeholder="mara.vale@example.com"
                  required
                />
              </Field>
            </div>

            <Field label="Password">
              <input
                className="input-shell"
                type="password"
                value={registerForm.password}
                onChange={(event) =>
                  onRegisterFormChange((current) => ({ ...current, password: event.target.value }))
                }
                placeholder="Preset password or your own"
                required
              />
            </Field>

            <div className="grid gap-4 sm:grid-cols-3">
              <Field label="First name">
                <input
                  className="input-shell"
                  value={registerForm.firstName}
                  onChange={(event) =>
                    onRegisterFormChange((current) => ({ ...current, firstName: event.target.value }))
                  }
                  placeholder="Mara"
                  required
                />
              </Field>
              <Field label="Middle names">
                <input
                  className="input-shell"
                  value={registerForm.middleNames}
                  onChange={(event) =>
                    onRegisterFormChange((current) => ({ ...current, middleNames: event.target.value }))
                  }
                  placeholder="Elise"
                />
              </Field>
              <Field label="Last name">
                <input
                  className="input-shell"
                  value={registerForm.lastName}
                  onChange={(event) =>
                    onRegisterFormChange((current) => ({ ...current, lastName: event.target.value }))
                  }
                  placeholder="Vale"
                  required
                />
              </Field>
            </div>

            <Field label="Avatar URL">
              <input
                className="input-shell"
                value={registerForm.avatarUrl}
                onChange={(event) =>
                  onRegisterFormChange((current) => ({ ...current, avatarUrl: event.target.value }))
                }
                placeholder="https://example.com/avatar.jpg"
              />
            </Field>

            <Field label="Bio">
              <textarea
                className="input-shell min-h-28 resize-none"
                value={registerForm.bio}
                onChange={(event) =>
                  onRegisterFormChange((current) => ({ ...current, bio: event.target.value }))
                }
                placeholder="Short public contributor bio"
              />
            </Field>

            <button className="button-shell mt-2" disabled={busy === 'register'} type="submit">
              {busy === 'register' ? 'Creating account' : 'Create account'}
            </button>
          </form>
        )}
      </div>

      <aside className="shell-card">
        <p className="text-[0.68rem] font-semibold uppercase tracking-[0.3em] text-stone-500">
          Quick test flow
        </p>
        <h3 className="mt-2 font-['Cormorant_Garamond',serif] text-3xl leading-none text-stone-950">
          Use the preset buttons
        </h3>
        <ol className="mt-5 grid gap-4 text-sm leading-7 text-stone-700">
          <li>1. Open register and fill one of the two demo authors.</li>
          <li>2. Submit once to create the backend account and contributor profile.</li>
          <li>3. Logout, switch to login, then use the matching login preset button.</li>
          <li>4. Return to Blogs and create or edit posts with the signed-in cookie session.</li>
        </ol>
      </aside>
    </section>
  )
}
