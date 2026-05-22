import type { LoginForm, RegisterForm } from '../types'

export type DemoAccount = {
  label: string
  register: RegisterForm
  login: LoginForm
}

export const demoAccounts = [
  {
    label: 'Mara Vale',
    register: {
      email: 'mara.vale@example.com',
      password: 'MaraPass123!',
      displayName: 'Mara Vale',
      firstName: 'Mara',
      middleNames: 'Elise',
      lastName: 'Vale',
      avatarUrl:
        'https://images.unsplash.com/photo-1494790108377-be9c29b29330?auto=format&fit=crop&w=600&q=80',
      bio: 'Editorial lead focused on essays, culture pieces and clean long-form writing.',
    },
    login: {
      email: 'mara.vale@example.com',
      password: 'MaraPass123!',
    },
  },
  {
    label: 'Noah Reed',
    register: {
      email: 'noah.reed@example.com',
      password: 'NoahPass123!',
      displayName: 'Noah Reed',
      firstName: 'Noah',
      middleNames: 'J.',
      lastName: 'Reed',
      avatarUrl:
        'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=600&q=80',
      bio: 'Contributor covering product notes, reviews and quick editorial updates.',
    },
    login: {
      email: 'noah.reed@example.com',
      password: 'NoahPass123!',
    },
  },
] as const satisfies readonly DemoAccount[]
