export type BlogRole = 'Author' | 'CoAuthor' | 'Editor' | 'Reviewer'

export interface AuthMe {
  userId: string
  email: string
  contributorId: string
  displayName: string
}

export interface BlogContributorDetails {
  contributorId: number
  role: BlogRole
  displayName: string
  avatarUrl: string | null
}

export interface Blog {
  id: number
  slug: string
  title: string
  subtitle: string
  intro: string
  content: string
  summary: string
  coverImageUrl: string | null
  createdAt: string
  updatedAt: string
  publishedAt: string | null
  contributors: BlogContributorDetails[]
  tags: string[]
}

export interface Contributor {
  id: number
  displayName: string
  email: string
  firstName: string | null
  middleNames: string[]
  lastName: string | null
  avatarUrl: string | null
  bio: string | null
  createdAt: string
  updatedAt: string
}

export interface ContributorBlogSummary {
  slug: string
  title: string
  role: BlogRole
}

export interface RegisterForm {
  email: string
  password: string
  displayName: string
  firstName: string
  middleNames: string
  lastName: string
  avatarUrl: string
  bio: string
}

export interface LoginForm {
  email: string
  password: string
}

export interface UpsertBlogForm {
  title: string
  subtitle: string
  intro: string
  content: string
  summary: string
  coverImageUrl: string
  tags: string
}

export interface UpdateContributorForm {
  displayName: string
  email: string
  firstName: string
  middleNames: string
  lastName: string
  avatarUrl: string
  bio: string
}
