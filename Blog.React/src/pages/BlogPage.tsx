import { BlogFields } from '../components/BlogFields'
import { formatDate } from '../lib/ui'
import type { Blog, BlogRole, Contributor, UpsertBlogForm } from '../types'

type BlogPageProps = {
  blog: Blog | null
  contributors: Contributor[]
  currentBlogRole: BlogRole | null
  canEditSelectedBlog: boolean
  canManageContributors: boolean
  canDeleteSelectedBlog: boolean
  editBlogForm: UpsertBlogForm
  contributorAssignment: { contributorId: string; role: string }
  roleDrafts: Record<number, BlogRole>
  busy: string | null
  onBack: () => void
  onEditBlogFormChange: React.Dispatch<React.SetStateAction<UpsertBlogForm>>
  onContributorAssignmentChange: React.Dispatch<
    React.SetStateAction<{ contributorId: string; role: string }>
  >
  onRoleDraftsChange: React.Dispatch<React.SetStateAction<Record<number, BlogRole>>>
  onUpdateBlog: (event: React.FormEvent<HTMLFormElement>) => void
  onDeleteBlog: () => void
  onAddContributor: (event: React.FormEvent<HTMLFormElement>) => void
  onUpdateContributorRole: (contributorId: number) => void
  onRemoveContributor: (contributorId: number) => void
}

export function BlogPage({
  blog,
  contributors,
  currentBlogRole,
  canEditSelectedBlog,
  canManageContributors,
  canDeleteSelectedBlog,
  editBlogForm,
  contributorAssignment,
  roleDrafts,
  busy,
  onBack,
  onEditBlogFormChange,
  onContributorAssignmentChange,
  onRoleDraftsChange,
  onUpdateBlog,
  onDeleteBlog,
  onAddContributor,
  onUpdateContributorRole,
  onRemoveContributor,
}: BlogPageProps) {
  if (!blog) {
    return (
      <section className="shell-card">
        <p className="text-sm text-stone-600">Loading blog detail...</p>
      </section>
    )
  }

  return (
    <div className="grid gap-6 lg:grid-cols-[1.45fr_0.9fr]">
      <article className="shell-card">
        <button type="button" onClick={onBack} className="text-xs font-semibold uppercase tracking-[0.24em] text-stone-500">
          Back to feed
        </button>

        <p className="mt-5 text-[0.68rem] font-semibold uppercase tracking-[0.28em] text-stone-500">
          {blog.slug}
        </p>
        <h2 className="mt-3 font-['Cormorant_Garamond',serif] text-4xl leading-none text-stone-950 sm:text-5xl">
          {blog.title}
        </h2>
        <p className="mt-4 text-lg leading-8 text-stone-700">{blog.subtitle}</p>

        <div className="mt-6 flex flex-wrap items-center gap-3 text-xs uppercase tracking-[0.22em] text-stone-500">
          <span>{formatDate(blog.updatedAt)}</span>
          <span className="rounded-full border border-stone-300 px-3 py-1">
            {blog.contributors.length} contributors
          </span>
        </div>

        <p className="mt-8 whitespace-pre-line text-[1rem] leading-8 text-stone-800">{blog.content}</p>

        <div className="mt-8 flex flex-wrap gap-2">
          {blog.tags.map((tag) => (
            <span
              key={tag}
              className="rounded-full border border-amber-300 bg-amber-100/70 px-3 py-1 text-[0.68rem] font-semibold uppercase tracking-[0.2em] text-amber-900"
            >
              {tag}
            </span>
          ))}
        </div>

        <div className="mt-10">
          <p className="text-[0.68rem] font-semibold uppercase tracking-[0.28em] text-stone-500">
            Contributors
          </p>
          <div className="mt-4 grid gap-3">
            {blog.contributors.map((contributor) => (
              <div
                key={contributor.contributorId}
                className="rounded-[1.3rem] border border-stone-200 bg-stone-50/70 p-4"
              >
                <div className="flex items-center justify-between gap-3">
                  <div>
                    <p className="font-semibold text-stone-950">{contributor.displayName}</p>
                    <p className="mt-1 text-xs uppercase tracking-[0.18em] text-stone-500">
                      Contributor #{contributor.contributorId}
                    </p>
                  </div>
                  <span className="rounded-full border border-stone-300 px-3 py-1 text-[0.68rem] uppercase tracking-[0.2em] text-stone-700">
                    {contributor.role}
                  </span>
                </div>

                {canManageContributors && (
                  <div className="mt-4 grid gap-3 sm:grid-cols-[1fr_auto_auto]">
                    <select
                      className="input-shell"
                      value={roleDrafts[contributor.contributorId] ?? contributor.role}
                      onChange={(event) =>
                        onRoleDraftsChange((current) => ({
                          ...current,
                          [contributor.contributorId]: event.target.value as BlogRole,
                        }))
                      }
                    >
                      <option value="Author">Author</option>
                      <option value="CoAuthor">CoAuthor</option>
                      <option value="Editor">Editor</option>
                      <option value="Reviewer">Reviewer</option>
                    </select>
                    <button
                      type="button"
                      className="button-shell"
                      disabled={busy === `role-${contributor.contributorId}`}
                      onClick={() => onUpdateContributorRole(contributor.contributorId)}
                    >
                      Save role
                    </button>
                    <button
                      type="button"
                      className="rounded-full border border-rose-300 bg-rose-50 px-4 py-3 text-xs font-semibold uppercase tracking-[0.2em] text-rose-700 transition hover:bg-rose-100"
                      disabled={busy === `remove-${contributor.contributorId}`}
                      onClick={() => onRemoveContributor(contributor.contributorId)}
                    >
                      Remove
                    </button>
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>
      </article>

      <aside className="grid gap-6">
        <section className="shell-card">
          <p className="text-[0.68rem] font-semibold uppercase tracking-[0.28em] text-stone-500">
            Story summary
          </p>
          <p className="mt-4 text-sm leading-7 text-stone-700">{blog.summary}</p>
          <div className="mt-6 rounded-[1.3rem] border border-stone-200 bg-stone-50/70 p-4">
            <p className="text-xs uppercase tracking-[0.2em] text-stone-500">Roles on this blog</p>
            <p className="mt-3 text-sm leading-7 text-stone-700">
              Authors can manage contributors and delete the blog. Editors can update blog content.
            </p>
            {currentBlogRole && (
              <p className="mt-3 text-[0.72rem] font-semibold uppercase tracking-[0.2em] text-stone-600">
                Your role: {currentBlogRole}
              </p>
            )}
          </div>
        </section>

        {canEditSelectedBlog && (
          <section className="shell-card">
            <div className="flex items-end justify-between gap-3">
              <div>
                <p className="text-[0.68rem] font-semibold uppercase tracking-[0.28em] text-stone-500">
                  Edit
                </p>
                <h3 className="mt-2 font-['Cormorant_Garamond',serif] text-3xl leading-none text-stone-950">
                  Update blog
                </h3>
              </div>
              <button
                type="button"
                onClick={() =>
                  onEditBlogFormChange({
                    title: blog.title,
                    subtitle: blog.subtitle,
                    intro: blog.intro,
                    content: blog.content,
                    summary: blog.summary,
                    coverImageUrl: blog.coverImageUrl ?? '',
                    tags: blog.tags.join(', '),
                  })
                }
                className="rounded-full border border-stone-300 px-3 py-2 text-[0.68rem] font-semibold uppercase tracking-[0.2em] text-stone-600"
              >
                Reset
              </button>
            </div>

            <form className="mt-5 grid gap-3" onSubmit={onUpdateBlog}>
              <BlogFields form={editBlogForm} onChange={onEditBlogFormChange} />
              <button className="button-shell mt-2" disabled={busy === 'update-blog'} type="submit">
                {busy === 'update-blog' ? 'Saving' : 'Save changes'}
              </button>
            </form>

            {canDeleteSelectedBlog && (
              <button
                type="button"
                className="mt-4 w-full rounded-full border border-rose-300 bg-rose-50 px-5 py-3 text-xs font-semibold uppercase tracking-[0.2em] text-rose-700 transition hover:bg-rose-100"
                disabled={busy === 'delete-blog'}
                onClick={onDeleteBlog}
              >
                {busy === 'delete-blog' ? 'Deleting' : 'Delete blog'}
              </button>
            )}
          </section>
        )}

        {canManageContributors && (
          <section className="shell-card">
            <p className="text-[0.68rem] font-semibold uppercase tracking-[0.28em] text-stone-500">
              Add contributor
            </p>
            <h3 className="mt-2 font-['Cormorant_Garamond',serif] text-3xl leading-none text-stone-950">
              Manage team
            </h3>

            <form className="mt-5 grid gap-3" onSubmit={onAddContributor}>
              <Field label="Contributor">
                <select
                  className="input-shell"
                  value={contributorAssignment.contributorId}
                  onChange={(event) =>
                    onContributorAssignmentChange((current) => ({
                      ...current,
                      contributorId: event.target.value,
                    }))
                  }
                  required
                >
                  <option value="">Choose contributor</option>
                  {contributors.map((contributor) => (
                    <option key={contributor.id} value={contributor.id}>
                      {contributor.displayName}
                    </option>
                  ))}
                </select>
              </Field>

              <Field label="Role">
                <select
                  className="input-shell"
                  value={contributorAssignment.role}
                  onChange={(event) =>
                    onContributorAssignmentChange((current) => ({
                      ...current,
                      role: event.target.value,
                    }))
                  }
                >
                  <option value="Author">Author</option>
                  <option value="CoAuthor">CoAuthor</option>
                  <option value="Editor">Editor</option>
                  <option value="Reviewer">Reviewer</option>
                </select>
              </Field>

              <button className="button-shell mt-2" disabled={busy === 'add-contributor'} type="submit">
                {busy === 'add-contributor' ? 'Adding' : 'Add contributor'}
              </button>
            </form>
          </section>
        )}
      </aside>
    </div>
  )
}

function Field({
  label,
  children,
}: {
  label: string
  children: React.ReactNode
}) {
  return (
    <label className="grid gap-2">
      <span className="text-[0.68rem] font-semibold uppercase tracking-[0.24em] text-stone-500">
        {label}
      </span>
      {children}
    </label>
  )
}
