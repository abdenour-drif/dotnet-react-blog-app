import { BlogFields } from '../components/BlogFields'
import { authorNames, summarize } from '../lib/blog'
import { formatDate, initials } from '../lib/ui'
import type { AuthMe, Blog, UpsertBlogForm } from '../types'

type HomePageProps = {
  blogs: Blog[]
  currentUser: AuthMe | null
  createBlogForm: UpsertBlogForm
  busy: string | null
  onRefresh: () => Promise<void>
  onOpenBlog: (slug: string) => void
  onAuthClick: () => void
  onLogout: () => void
  onCreateBlogFormChange: React.Dispatch<React.SetStateAction<UpsertBlogForm>>
  onCreateBlog: (event: React.FormEvent<HTMLFormElement>) => void
}

export function HomePage({
  blogs,
  currentUser,
  createBlogForm,
  busy,
  onRefresh,
  onOpenBlog,
  onAuthClick,
  onLogout,
  onCreateBlogFormChange,
  onCreateBlog,
}: HomePageProps) {
  return (
    <div className="grid gap-6 lg:grid-cols-[1.35fr_0.9fr]">
      <section className="shell-card">
        <div className="flex items-end justify-between gap-3">
          <div>
            <p className="text-[0.68rem] font-semibold uppercase tracking-[0.3em] text-stone-500">
              Feed
            </p>
            <h2 className="mt-2 font-['Cormorant_Garamond',serif] text-3xl leading-none text-stone-950">
              Latest blog previews
            </h2>
          </div>
          <button
            type="button"
            onClick={() => void onRefresh()}
            className="rounded-full border border-stone-300 px-3 py-2 text-[0.68rem] font-semibold uppercase tracking-[0.2em] text-stone-600"
          >
            Refresh
          </button>
        </div>

        <div className="mt-6 grid gap-4">
          {blogs.map((blog) => (
            <article
              key={blog.slug}
              className="rounded-[1.5rem] border border-stone-200 bg-stone-50/70 p-5 transition hover:-translate-y-0.5 hover:border-stone-400 hover:shadow-[0_18px_34px_rgba(91,70,41,0.08)]"
            >
              <div className="flex items-start justify-between gap-4">
                <div>
                  <p className="text-[0.68rem] font-semibold uppercase tracking-[0.24em] text-stone-500">
                    {blog.slug}
                  </p>
                  <h3 className="mt-3 text-xl font-semibold text-stone-950">{blog.title}</h3>
                  <p className="mt-2 text-sm leading-7 text-stone-700">{blog.subtitle}</p>
                </div>
                <span className="rounded-full border border-stone-300 px-3 py-1 text-[0.68rem] uppercase tracking-[0.2em] text-stone-700">
                  {formatDate(blog.updatedAt)}
                </span>
              </div>

              <p className="mt-4 text-sm leading-7 text-stone-600">{summarize(blog.summary || blog.intro)}</p>

              <div className="mt-5 flex flex-wrap items-center gap-2 text-[0.68rem] font-semibold uppercase tracking-[0.18em] text-stone-500">
                {blog.tags.slice(0, 4).map((tag) => (
                  <span key={tag} className="rounded-full bg-white px-3 py-1 text-stone-700">
                    {tag}
                  </span>
                ))}
              </div>

              <div className="mt-5 flex items-center justify-between gap-3">
                <p className="text-xs uppercase tracking-[0.18em] text-stone-500">{authorNames(blog)}</p>
                <button type="button" className="button-shell" onClick={() => onOpenBlog(blog.slug)}>
                  Open story
                </button>
              </div>
            </article>
          ))}
        </div>
      </section>

      <aside className="grid gap-6">
        <section className="shell-card">
          <p className="text-[0.68rem] font-semibold uppercase tracking-[0.3em] text-stone-500">
            Session
          </p>
          {currentUser ? (
            <>
              <div className="mt-4 flex items-center gap-4 rounded-[1.4rem] border border-amber-200 bg-amber-50/90 p-4">
                <div className="flex h-14 w-14 items-center justify-center rounded-full bg-stone-950 font-['Cormorant_Garamond',serif] text-2xl text-amber-100">
                  {initials(currentUser.displayName)}
                </div>
                <div>
                  <p className="font-semibold text-stone-950">{currentUser.displayName}</p>
                  <p className="text-sm text-stone-600">{currentUser.email}</p>
                </div>
              </div>
              <button
                type="button"
                onClick={onLogout}
                disabled={busy === 'logout'}
                className="button-shell mt-4 w-full"
              >
                {busy === 'logout' ? 'Signing out' : 'Logout'}
              </button>
            </>
          ) : (
            <>
              <p className="mt-4 text-sm leading-7 text-stone-700">
                You are browsing publicly. Use login to create posts or edit stories where your
                contributor role is Author or Editor.
              </p>
              <button type="button" onClick={onAuthClick} className="button-shell mt-5 w-full">
                Login or register
              </button>
            </>
          )}
        </section>

        {currentUser && (
          <section className="shell-card">
            <p className="text-[0.68rem] font-semibold uppercase tracking-[0.3em] text-stone-500">
              Publish
            </p>
            <h3 className="mt-2 font-['Cormorant_Garamond',serif] text-3xl leading-none text-stone-950">
              New blog post
            </h3>

            <form className="mt-5 grid gap-3" onSubmit={onCreateBlog}>
              <BlogFields form={createBlogForm} onChange={onCreateBlogFormChange} />
              <button className="button-shell mt-2" disabled={busy === 'create-blog'} type="submit">
                {busy === 'create-blog' ? 'Publishing' : 'Create blog'}
              </button>
            </form>
          </section>
        )}
      </aside>
    </div>
  )
}
