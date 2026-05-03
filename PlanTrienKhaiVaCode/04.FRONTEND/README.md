# Frontend - Next.js 14 Application

## Cấu trúc

```
04.FRONTEND/
├── src/
│   ├── app/                    # Next.js App Router
│   │   ├── layout.tsx          # Root layout
│   │   ├── page.tsx           # Trang chủ
│   │   ├── (auth)/
│   │   │   ├── login/page.tsx
│   │   │   └── register/page.tsx
│   │   └── (dashboard)/
│   │       ├── layout.tsx
│   │       ├── page.tsx       # Dashboard
│   │       ├── documents/
│   │       │   ├── page.tsx   # Document list
│   │       │   └── upload/page.tsx
│   │       ├── search/page.tsx
│   │       ├── chat/
│   │       │   ├── page.tsx
│   │       │   └── [id]/page.tsx
│   │       └── admin/
│   │           └── page.tsx
│   │
│   ├── components/             # React components
│   │   ├── ui/               # shadcn/ui components
│   │   ├── layout/           # Layout components
│   │   ├── auth/
│   │   ├── documents/
│   │   ├── search/
│   │   └── chat/
│   │
│   ├── hooks/                 # Custom hooks
│   ├── lib/                  # Utilities
│   ├── stores/               # Zustand stores
│   └── types/               # TypeScript types
│
├── package.json
└── README.md
```

## Tech Stack

- **Framework**: Next.js 14 (App Router)
- **Language**: TypeScript 5
- **UI**: Tailwind CSS + shadcn/ui
- **State**: Zustand (client), TanStack Query (server)
- **Forms**: React Hook Form + Zod
- **HTTP**: Axios
- **Icons**: Lucide React

## Setup

```bash
npm install
npm run dev
```

## Environment Variables

```env
NEXT_PUBLIC_API_URL=http://localhost:5000
NEXT_PUBLIC_APP_ENV=development
```
