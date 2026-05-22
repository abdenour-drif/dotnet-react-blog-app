import type { Dispatch, SetStateAction } from 'react'
import type { UpsertBlogForm } from '../types'
import { Field } from './Field'

type BlogFieldsProps = {
  form: UpsertBlogForm
  onChange: Dispatch<SetStateAction<UpsertBlogForm>>
}

export function BlogFields({ form, onChange }: BlogFieldsProps) {
  return (
    <>
      <Field label="Title">
        <input
          className="input-shell"
          value={form.title}
          onChange={(event) => onChange((current) => ({ ...current, title: event.target.value }))}
          placeholder="Quiet product launch notes"
          required
        />
      </Field>

      <Field label="Subtitle">
        <input
          className="input-shell"
          value={form.subtitle}
          onChange={(event) => onChange((current) => ({ ...current, subtitle: event.target.value }))}
          placeholder="One short line to frame the article"
          required
        />
      </Field>

      <Field label="Intro">
        <textarea
          className="input-shell min-h-24 resize-none"
          value={form.intro}
          onChange={(event) => onChange((current) => ({ ...current, intro: event.target.value }))}
          placeholder="Opening paragraph"
          required
        />
      </Field>

      <Field label="Summary">
        <textarea
          className="input-shell min-h-24 resize-none"
          value={form.summary}
          onChange={(event) => onChange((current) => ({ ...current, summary: event.target.value }))}
          placeholder="Short card summary"
          required
        />
      </Field>

      <Field label="Content">
        <textarea
          className="input-shell min-h-44 resize-y"
          value={form.content}
          onChange={(event) => onChange((current) => ({ ...current, content: event.target.value }))}
          placeholder="Main article body"
          required
        />
      </Field>

      <Field label="Cover image URL">
        <input
          className="input-shell"
          value={form.coverImageUrl}
          onChange={(event) =>
            onChange((current) => ({ ...current, coverImageUrl: event.target.value }))
          }
          placeholder="Optional image URL"
        />
      </Field>

      <Field label="Tags">
        <input
          className="input-shell"
          value={form.tags}
          onChange={(event) => onChange((current) => ({ ...current, tags: event.target.value }))}
          placeholder="culture, notes, review"
        />
      </Field>
    </>
  )
}
