type StatusBannerProps = {
  statusMessage: string
  errorMessage: string
}

export function StatusBanner({ statusMessage, errorMessage }: StatusBannerProps) {
  if (!statusMessage && !errorMessage) {
    return null
  }

  return (
    <section
      className={`rounded-[1.5rem] border px-4 py-3 text-sm shadow-sm ${
        errorMessage
          ? 'border-rose-200 bg-rose-50 text-rose-700'
          : 'border-emerald-200 bg-emerald-50 text-emerald-700'
      }`}
    >
      {errorMessage || statusMessage}
    </section>
  )
}
