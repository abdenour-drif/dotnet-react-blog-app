import type { Blog, UpsertBlogForm } from '../types'

export function splitList(value: string) {
  return value
    .split(',')
    .map((item) => item.trim())
    .filter(Boolean)
}

export function blogPayload(form: UpsertBlogForm) {
  return {
    title: form.title.trim(),
    subtitle: form.subtitle.trim(),
    intro: form.intro.trim(),
    content: form.content.trim(),
    summary: form.summary.trim(),
    coverImageUrl: form.coverImageUrl.trim() || null,
    tags: splitList(form.tags),
  }
}

export function summarize(value: string) {
  return value.length > 180 ? `${value.slice(0, 177)}...` : value
}

export function authorNames(blog: Blog) {
  const names = blog.contributors.map((contributor) => contributor.displayName)
  return names.length > 0 ? names.join(', ') : 'No contributors listed'
}
