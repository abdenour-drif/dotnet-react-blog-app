export type HashRoute = { kind: 'home' } | { kind: 'auth' } | { kind: 'blog'; slug: string }

export function parseHashRoute(): HashRoute {
  const hash = window.location.hash.replace(/^#/, '')

  if (hash === '/auth') {
    return { kind: 'auth' }
  }

  if (hash.startsWith('/blog/')) {
    const slug = decodeURIComponent(hash.slice('/blog/'.length))
    if (slug) {
      return { kind: 'blog', slug }
    }
  }

  return { kind: 'home' }
}

export function navigateTo(route: HashRoute) {
  if (route.kind === 'home') {
    window.location.hash = '/'
    return
  }

  if (route.kind === 'auth') {
    window.location.hash = '/auth'
    return
  }

  window.location.hash = `/blog/${encodeURIComponent(route.slug)}`
}
