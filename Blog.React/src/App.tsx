import { useEffect, useMemo, useState } from 'react'
import { Navbar } from './components/Navbar'
import { StatusBanner } from './components/StatusBanner'
import { demoAccounts } from './constants/demoAccounts'
import { api, ApiError } from './lib/api'
import { blogPayload, splitList } from './lib/blog'
import { navigateTo, parseHashRoute, type HashRoute } from './lib/routing'
import { AuthPage } from './pages/AuthPage'
import { BlogPage } from './pages/BlogPage'
import { HomePage } from './pages/HomePage'
import type {
  AuthMe,
  Blog,
  BlogRole,
  Contributor,
  LoginForm,
  RegisterForm,
  UpsertBlogForm,
} from './types'

const emptyBlogForm: UpsertBlogForm = {
  title: '',
  subtitle: '',
  intro: '',
  content: '',
  summary: '',
  coverImageUrl: '',
  tags: '',
}

const emptyRegisterForm: RegisterForm = {
  email: '',
  password: '',
  displayName: '',
  firstName: '',
  middleNames: '',
  lastName: '',
  avatarUrl: '',
  bio: '',
}

const emptyLoginForm: LoginForm = {
  email: '',
  password: '',
}

function App() {
  const [route, setRoute] = useState<HashRoute>(() => parseHashRoute())
  const [currentUser, setCurrentUser] = useState<AuthMe | null>(null)
  const [blogs, setBlogs] = useState<Blog[]>([])
  const [contributors, setContributors] = useState<Contributor[]>([])
  const [selectedBlog, setSelectedBlog] = useState<Blog | null>(null)
  const [authMode, setAuthMode] = useState<'login' | 'register'>('login')
  const [loginForm, setLoginForm] = useState<LoginForm>(emptyLoginForm)
  const [registerForm, setRegisterForm] = useState<RegisterForm>(emptyRegisterForm)
  const [createBlogForm, setCreateBlogForm] = useState<UpsertBlogForm>(emptyBlogForm)
  const [editBlogForm, setEditBlogForm] = useState<UpsertBlogForm>(emptyBlogForm)
  const [contributorAssignment, setContributorAssignment] = useState({ contributorId: '', role: 'Editor' })
  const [roleDrafts, setRoleDrafts] = useState<Record<number, BlogRole>>({})
  const [statusMessage, setStatusMessage] = useState('Ready')
  const [errorMessage, setErrorMessage] = useState('')
  const [busy, setBusy] = useState<string | null>(null)

  const currentBlogRole = useMemo(() => {
    if (!currentUser || !selectedBlog) return null

    return (
      selectedBlog.contributors.find(
        (contributor) => contributor.contributorId === Number(currentUser.contributorId),
      )?.role ?? null
    )
  }, [currentUser, selectedBlog])

  const canEditSelectedBlog =
    currentBlogRole === 'Author' || currentBlogRole === 'CoAuthor' || currentBlogRole === 'Editor'
  const canManageContributors = currentBlogRole === 'Author'
  const canDeleteSelectedBlog = currentBlogRole === 'Author'

  useEffect(() => {
    void bootstrap()
  }, [])

  useEffect(() => {
    const onHashChange = () => setRoute(parseHashRoute())
    window.addEventListener('hashchange', onHashChange)
    return () => window.removeEventListener('hashchange', onHashChange)
  }, [])

  useEffect(() => {
    if (route.kind === 'blog') {
      void fetchBlog(route.slug)
      return
    }

    setSelectedBlog(null)
  }, [route])

  async function bootstrap() {
    setBusy('bootstrap')
    try {
      await Promise.all([refreshSession(), refreshBlogs(), refreshContributors()])
      setStatusMessage('Backend connected')
    } catch (error) {
      handleError(error, 'Failed to load initial data.')
    } finally {
      setBusy(null)
    }
  }

  async function refreshSession() {
    try {
      const user = await api<AuthMe>('/auth/me')
      setCurrentUser(user)
    } catch (error) {
      if (error instanceof ApiError && error.status === 401) {
        setCurrentUser(null)
        return
      }

      throw error
    }
  }

  async function refreshBlogs() {
    const data = await api<Blog[]>('/blogs')
    setBlogs(data)

    if (route.kind === 'blog' && !data.some((blog) => blog.slug === route.slug)) {
      navigateTo({ kind: 'home' })
    }
  }

  async function refreshContributors() {
    const data = await api<Contributor[]>('/contributors')
    setContributors(data)
  }

  async function fetchBlog(slug: string) {
    try {
      const data = await api<Blog>(`/blogs/${slug}`)
      setSelectedBlog(data)
      setEditBlogForm({
        title: data.title,
        subtitle: data.subtitle,
        intro: data.intro,
        content: data.content,
        summary: data.summary,
        coverImageUrl: data.coverImageUrl ?? '',
        tags: data.tags.join(', '),
      })
      setRoleDrafts(
        Object.fromEntries(
          data.contributors.map((contributor) => [contributor.contributorId, contributor.role]),
        ) as Record<number, BlogRole>,
      )
    } catch (error) {
      handleError(error, 'Failed to load blog detail.')
    }
  }

  async function handleRegister(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setBusy('register')
    clearMessages()

    try {
      const payload = {
        ...registerForm,
        middleNames: splitList(registerForm.middleNames),
      }

      const user = await api<AuthMe>('/auth/register', {
        method: 'POST',
        body: JSON.stringify(payload),
      })

      setCurrentUser(user)
      setRegisterForm(emptyRegisterForm)
      await Promise.all([refreshContributors(), refreshBlogs()])
      navigateTo({ kind: 'home' })
      setStatusMessage('Account created and signed in.')
    } catch (error) {
      handleError(error, 'Register failed.')
    } finally {
      setBusy(null)
    }
  }

  async function handleLogin(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setBusy('login')
    clearMessages()

    try {
      const user = await api<AuthMe>('/auth/login', {
        method: 'POST',
        body: JSON.stringify(loginForm),
      })

      setCurrentUser(user)
      setLoginForm(emptyLoginForm)
      await Promise.all([refreshBlogs(), refreshContributors()])
      navigateTo({ kind: 'home' })
      setStatusMessage('Signed in.')
    } catch (error) {
      handleError(error, 'Login failed.')
    } finally {
      setBusy(null)
    }
  }

  async function handleLogout() {
    setBusy('logout')
    clearMessages()

    try {
      await api<{ message: string }>('/auth/logout', { method: 'POST' })
      setCurrentUser(null)
      navigateTo({ kind: 'home' })
      setStatusMessage('Signed out.')
    } catch (error) {
      handleError(error, 'Logout failed.')
    } finally {
      setBusy(null)
    }
  }

  async function handleCreateBlog(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setBusy('create-blog')
    clearMessages()

    try {
      const created = await api<Blog>('/blogs', {
        method: 'POST',
        body: JSON.stringify(blogPayload(createBlogForm)),
      })

      setCreateBlogForm(emptyBlogForm)
      await refreshBlogs()
      navigateTo({ kind: 'blog', slug: created.slug })
      setStatusMessage('Blog created.')
    } catch (error) {
      handleError(error, 'Create blog failed.')
    } finally {
      setBusy(null)
    }
  }

  async function handleUpdateBlog(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!selectedBlog) return

    setBusy('update-blog')
    clearMessages()

    try {
      const updated = await api<Blog>(`/blogs/${selectedBlog.slug}`, {
        method: 'PATCH',
        body: JSON.stringify(blogPayload(editBlogForm)),
      })

      setSelectedBlog(updated)
      await refreshBlogs()
      setStatusMessage('Blog updated.')
    } catch (error) {
      handleError(error, 'Update blog failed.')
    } finally {
      setBusy(null)
    }
  }

  async function handleDeleteBlog() {
    if (!selectedBlog) return

    setBusy('delete-blog')
    clearMessages()

    try {
      await api<void>(`/blogs/${selectedBlog.slug}`, { method: 'DELETE' })
      setSelectedBlog(null)
      await refreshBlogs()
      navigateTo({ kind: 'home' })
      setStatusMessage('Blog deleted.')
    } catch (error) {
      handleError(error, 'Delete blog failed.')
    } finally {
      setBusy(null)
    }
  }

  async function handleAddContributor(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    if (!selectedBlog || !contributorAssignment.contributorId) return

    setBusy('add-contributor')
    clearMessages()

    try {
      await api(`/blogs/${selectedBlog.slug}/contributors`, {
        method: 'POST',
        body: JSON.stringify({
          contributorId: Number(contributorAssignment.contributorId),
          role: contributorAssignment.role,
        }),
      })

      setContributorAssignment({ contributorId: '', role: 'Editor' })
      await Promise.all([fetchBlog(selectedBlog.slug), refreshBlogs()])
      setStatusMessage('Contributor added to blog.')
    } catch (error) {
      handleError(error, 'Add contributor failed.')
    } finally {
      setBusy(null)
    }
  }

  async function handleUpdateContributorRole(contributorId: number) {
    if (!selectedBlog) return

    const role = roleDrafts[contributorId]
    if (!role) return

    setBusy(`role-${contributorId}`)
    clearMessages()

    try {
      await api(`/blogs/${selectedBlog.slug}/contributors/${contributorId}?role=${encodeURIComponent(role)}`, {
        method: 'PATCH',
      })

      await Promise.all([fetchBlog(selectedBlog.slug), refreshBlogs()])
      setStatusMessage('Contributor role updated.')
    } catch (error) {
      handleError(error, 'Role update failed.')
    } finally {
      setBusy(null)
    }
  }

  async function handleRemoveContributor(contributorId: number) {
    if (!selectedBlog) return

    setBusy(`remove-${contributorId}`)
    clearMessages()

    try {
      await api(`/blogs/${selectedBlog.slug}/contributors/${contributorId}`, {
        method: 'DELETE',
      })

      await Promise.all([fetchBlog(selectedBlog.slug), refreshBlogs()])
      setStatusMessage('Contributor removed from blog.')
    } catch (error) {
      handleError(error, 'Remove contributor failed.')
    } finally {
      setBusy(null)
    }
  }

  function clearMessages() {
    setStatusMessage('')
    setErrorMessage('')
  }

  function handleError(error: unknown, fallback: string) {
    if (error instanceof ApiError) {
      setErrorMessage(error.message || fallback)
      return
    }

    if (error instanceof Error) {
      setErrorMessage(error.message || fallback)
      return
    }

    setErrorMessage(fallback)
  }

  return (
    <div className="min-h-screen bg-[radial-gradient(circle_at_top,_rgba(255,203,118,0.32),_transparent_28%),linear-gradient(180deg,_#fcfaf5_0%,_#f7f1e8_48%,_#efe5d8_100%)] text-stone-900">
      <div className="mx-auto flex min-h-screen max-w-[60rem] flex-col gap-6 px-4 py-4 sm:px-5 lg:px-6">
        <Navbar
          blogCount={blogs.length}
          contributorCount={contributors.length}
          route={route}
          currentUser={currentUser}
          busy={busy}
          onHomeClick={() => navigateTo({ kind: 'home' })}
          onAuthClick={() => navigateTo({ kind: 'auth' })}
          onLogout={handleLogout}
        />

        <StatusBanner statusMessage={statusMessage} errorMessage={errorMessage} />

        {route.kind === 'auth' ? (
          <AuthPage
            currentUser={currentUser}
            authMode={authMode}
            busy={busy}
            loginForm={loginForm}
            registerForm={registerForm}
            demoAccounts={demoAccounts}
            onAuthModeChange={setAuthMode}
            onLoginFormChange={setLoginForm}
            onRegisterFormChange={setRegisterForm}
            onLogin={handleLogin}
            onRegister={handleRegister}
            onLogout={handleLogout}
            onBackHome={() => navigateTo({ kind: 'home' })}
          />
        ) : route.kind === 'blog' ? (
          <BlogPage
            blog={selectedBlog}
            contributors={contributors}
            currentBlogRole={currentBlogRole}
            canEditSelectedBlog={canEditSelectedBlog}
            canManageContributors={canManageContributors}
            canDeleteSelectedBlog={canDeleteSelectedBlog}
            editBlogForm={editBlogForm}
            contributorAssignment={contributorAssignment}
            roleDrafts={roleDrafts}
            busy={busy}
            onBack={() => navigateTo({ kind: 'home' })}
            onEditBlogFormChange={setEditBlogForm}
            onContributorAssignmentChange={setContributorAssignment}
            onRoleDraftsChange={setRoleDrafts}
            onUpdateBlog={handleUpdateBlog}
            onDeleteBlog={handleDeleteBlog}
            onAddContributor={handleAddContributor}
            onUpdateContributorRole={handleUpdateContributorRole}
            onRemoveContributor={handleRemoveContributor}
          />
        ) : (
          <HomePage
            blogs={blogs}
            currentUser={currentUser}
            createBlogForm={createBlogForm}
            busy={busy}
            onRefresh={refreshBlogs}
            onOpenBlog={(slug) => navigateTo({ kind: 'blog', slug })}
            onAuthClick={() => navigateTo({ kind: 'auth' })}
            onLogout={handleLogout}
            onCreateBlogFormChange={setCreateBlogForm}
            onCreateBlog={handleCreateBlog}
          />
        )}
      </div>
    </div>
  )
}

export default App
