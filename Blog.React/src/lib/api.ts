export class ApiError extends Error {
  status: number

  constructor(status: number, message: string) {
    super(message)
    this.status = status
  }
}

async function parseResponse<T>(response: Response): Promise<T> {
  const contentType = response.headers.get('content-type') ?? ''

  if (response.status === 204) {
    return undefined as T
  }

  if (contentType.includes('application/json')) {
    return (await response.json()) as T
  }

  return (await response.text()) as T
}

export async function api<T>(path: string, init?: RequestInit): Promise<T> {
  const response = await fetch(path, {
    credentials: 'include',
    headers: {
      'Content-Type': 'application/json',
      ...(init?.headers ?? {}),
    },
    ...init,
  })

  if (!response.ok) {
    const payload = await parseResponse<unknown>(response)

    if (typeof payload === 'string') {
      throw new ApiError(response.status, payload)
    }

    if (payload && typeof payload === 'object') {
      const possibleMessage = (payload as { message?: string; title?: string }).message
        ?? (payload as { title?: string }).title

      throw new ApiError(response.status, possibleMessage ?? `Request failed with status ${response.status}`)
    }

    throw new ApiError(response.status, `Request failed with status ${response.status}`)
  }

  return parseResponse<T>(response)
}
