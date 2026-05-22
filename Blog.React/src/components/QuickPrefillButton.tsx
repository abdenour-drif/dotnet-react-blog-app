type QuickPrefillButtonProps = {
  label: string
  onClick: () => void
}

export function QuickPrefillButton({ label, onClick }: QuickPrefillButtonProps) {
  return (
    <button
      type="button"
      onClick={onClick}
      className="rounded-full border border-stone-300 bg-white px-3 py-2 text-[0.68rem] font-semibold uppercase tracking-[0.2em] text-stone-700 transition hover:border-stone-500"
    >
      {label}
    </button>
  )
}
