# KIẾN TRÚC HỆ THỐNG CHI TIẾT - AI BASE FRAMEWORK

## Mục lục

1. [Tổng quan Kiến trúc](#1-tổng-quan-kiến-trúc)
2. [Frontend Architecture](#2-frontend-architecture)
3. [Backend Architecture](#3-backend-architecture)
4. [Database Schema](#4-database-schema)
5. [API Design](#5-api-design)
6. [AI Integration - Ollama](#6-ai-integration---ollama)
7. [Deployment](#7-deployment)

---

## 1. Tổng quan Kiến trúc

### 1.1 Sơ đồ Tổng thể

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                    AI BASE FRAMEWORK - INTEGRATION ARCHITECTURE              │
│              Framework Base Tích hợp AI - 100% Offline Local                     │
└─────────────────────────────────────────────────────────────────────────────────┘

    ┌─────────────────────────────────────────────────────────────────────────┐
    │                           FRONTEND (Next.js 14)                          │
    │   Zustand (State)  •  TanStack Query (Server State)  •  TailwindCSS   │
    │   Pages: Dashboard  •  Documents  •  Search  •  Chat  •  Admin         │
    └─────────────────────────────────────────────────────────────────────────┘
                                        │
                                        │ HTTP/HTTPS
                                        ▼
    ┌─────────────────────────────────────────────────────────────────────────┐
    │                          API GATEWAY (Nginx)                           │
    │   Rate Limiting  •  SSL Termination  •  Load Balancing  •  Caching   │
    └─────────────────────────────────────────────────────────────────────────┘
                                        │
                                        │ Internal HTTP
                                        ▼
    ┌─────────────────────────────────────────────────────────────────────────┐
    │                        BACKEND (.NET 8 + ASP.NET Core)                     │
    │   MediatR Pipeline  •  CQRS  •  JWT Auth  •  FluentValidation            │
    │   Controllers: Auth  •  Documents  •  Search  •  Chat  •  Admin       │
    └─────────────────────────────────────────────────────────────────────────┘
                                        │
                    ┌─────────────────────┼─────────────────────┐
                    ▼                     ▼                     ▼
    ┌───────────────────────┐  ┌───────────────────┐  ┌───────────────────────┐
    │   AI SERVICES        │  │  DATA TIER        │  │  BACKGROUND JOBS    │
    │   (Ollama)          │  │                   │  │  (Hangfire)          │
    │                      │  │  PostgreSQL 16   │  │                      │
    │  • Chat Model       │  │  pgvector        │  │  • Document Processing│
    │    (llama3.2:3b)   │  │  Redis           │  │  • OCR Processing   │
    │  • Embedding Model  │  │  MinIO           │  │  • Vector Indexing   │
    │    (nomic-embed)   │  │                   │  │  • Cleanup Jobs      │
    └───────────────────────┘  └───────────────────┘  └───────────────────────┘
```

### 1.2 Data Flow

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│  LUỒNG 1: DOCUMENT UPLOAD & PROCESSING                                          │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  User → Upload → Validate → MinIO → Queue → Background Worker                      │
│                                        │               │                         │
│                                        ▼               ▼                         │
│                              ┌─────────────────┐ ┌─────────────────┐            │
│                              │ TEXT EXTRACTION │ │ EMBEDDING        │            │
│                              │ (iText7)       │ │ (Ollama)        │            │
│                              └────────┬────────┘ └────────┬────────┘            │
│                                       │                  │                     │
│                                       ▼                  ▼                     │
│                              ┌─────────────────┐ ┌─────────────────┐            │
│                              │ CHUNKING        │ │ VECTOR INDEX    │            │
│                              │ (512 tokens)   │ │ (pgvector HNSW)│            │
│                              └────────┬────────┘ └────────┬────────┘            │
│                                       │                  │                     │
│                                       └──────────────────┴─────→ PostgreSQL      │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────────┐
│  LUỒNG 2: RAG CHATBOT                                                             │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                  │
│  User → Message → Embedding → Vector Search → RAG Prompt → Ollama → Response    │
│                  │                │               │           │                     │
│                  ▼                ▼               ▼           ▼                     │
│           ┌──────────┐  ┌──────────────┐  ┌──────────┐ ┌──────────┐          │
│           │ Session  │  │ pgvector    │  │ Retrieved│ │ LLM      │          │
│           │ History  │  │ (HNSW)     │  │ Chunks   │ │ Chat    │          │
│           └──────────┘  └──────────────┘  └──────────┘ └──────────┘          │
│                                                                                  │
└─────────────────────────────────────────────────────────────────────────────────┘
```

---

## 2. Frontend Architecture

### 2.1 Tech Stack

| Layer | Technology | Purpose |
|-------|------------|---------|
| Framework | Next.js 14 (App Router) | React framework |
| Language | TypeScript 5 | Type safety |
| Styling | Tailwind CSS + shadcn/ui | UI components |
| State | Zustand | Client state management |
| Server State | TanStack Query | Data fetching & caching |
| Forms | React Hook Form + Zod | Form handling |
| HTTP Client | Axios | API communication |
| Icons | Lucide React | Icon library |

### 2.2 Directory Structure

```
src/
├── app/                    # Next.js App Router
│   ├── layout.tsx          # Root layout
│   ├── page.tsx            # Home/Dashboard
│   ├── (auth)/
│   │   ├── login/         # Login page
│   │   └── register/      # Register page
│   └── (dashboard)/
│       ├── layout.tsx      # Dashboard layout (with sidebar)
│       ├── page.tsx        # Dashboard home
│       ├── documents/      # Document management
│       ├── search/         # Semantic search
│       ├── chat/           # RAG chatbot
│       └── admin/          # Admin panel
│
├── components/             # React components
│   ├── ui/               # shadcn/ui components
│   ├── layout/           # Layout components (Sidebar, Header)
│   ├── auth/             # Auth components
│   ├── documents/         # Document components
│   ├── search/           # Search components
│   └── chat/             # Chat components
│
├── hooks/                  # Custom React hooks
├── lib/                    # Utilities
│   ├── api.ts           # Axios client
│   └── utils.ts         # Helper functions
│
├── stores/                # Zustand stores
│   ├── authStore.ts     # Auth state
│   ├── documentStore.ts # Document state
│   └── chatStore.ts     # Chat state
│
└── types/                # TypeScript types
```

### 2.3 State Management

```
┌─────────────────────────────────────────────────────────────────┐
│                    STATE MANAGEMENT LAYER                           │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌──────────────────┐     ┌──────────────────┐               │
│  │   Zustand        │     │  TanStack Query   │               │
│  │  (Client State)  │     │ (Server State)    │               │
│  │                  │     │                   │               │
│  │  • Auth state   │     │  • Documents      │               │
│  │  • UI state     │     │  • Search results │               │
│  │  • Theme       │     │  • Chat history  │               │
│  │  • Sidebar     │     │  • User list     │               │
│  └────────┬─────────┘     └────────┬─────────┘               │
│           │                           │                          │
│           └───────────────┬─────────┘                          │
│                           ▼                                    │
│                  ┌──────────────────┐                        │
│                  │   React Context   │                        │
│                  │   (Providers)     │                        │
│                  └──────────────────┘                        │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

## 3. Backend Architecture

### 3.1 Clean Architecture Layers

```
┌─────────────────────────────────────────────────────────────────┐
│                    CLEAN ARCHITECTURE                              │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌───────────────────────────────────────────────────────────┐    │
│  │                   PRESENTATION LAYER                       │    │
│  │                                                            │    │
│  │   Controllers          DTOs           Middleware          │    │
│  │   ├── AuthController   ├── AuthDTOs    ├── Exception      │    │
│  │   ├── DocumentCtrl    ├── DocumentDTOs├── Validation     │    │
│  │   ├── SearchCtrl     ├── SearchDTOs   └── CORS           │    │
│  │   └── ChatCtrl       └── ChatDTOs                        │    │
│  └───────────────────────────────────────────────────────────┘    │
│                              │                                  │
│                              ▼                                  │
│  ┌───────────────────────────────────────────────────────────┐    │
│  │                   APPLICATION LAYER                        │    │
│  │                                                            │    │
│  │   Commands/Queries (MediatR)      Behaviors (Pipeline)   │    │
│  │   ├── RegisterCommand             ├── LoggingBehavior     │    │
│  │   ├── LoginQuery                 ├── ValidationBehavior  │    │
│  │   ├── UploadDocumentCommand     └── ExceptionBehavior   │    │
│  │   └── SearchQuery                                     │    │
│  └───────────────────────────────────────────────────────────┘    │
│                              │                                  │
│                              ▼                                  │
│  ┌───────────────────────────────────────────────────────────┐    │
│  │                      DOMAIN LAYER                          │    │
│  │                                                            │    │
│  │   Entities            Enums           Domain Services       │    │
│  │   ├── User           ├── UserRole      ├── IAuthService   │    │
│  │   ├── Document      ├── DocStatus     ├── IDocService    │    │
│  │   ├── DocumentChunk └── MessageRole   └── ISearchService  │    │
│  │   ├── ChatSession                                    │    │
│  │   └── ChatMessage                                    │    │
│  └───────────────────────────────────────────────────────────┘    │
│                              │                                  │
│                              ▼                                  │
│  ┌───────────────────────────────────────────────────────────┐    │
│  │                  INFRASTRUCTURE LAYER                       │    │
│  │                                                            │    │
│  │   Data Access       AI Services       External Services     │    │
│  │   ├── AppDbContext  ├── OllamaService ├── MinIOService    │    │
│  │   ├── UserRepo      ├── EmbeddingService└─ RedisService  │    │
│  │   └── DocRepo       └── RAGPipeline                       │    │
│  └───────────────────────────────────────────────────────────┘    │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### 3.2 CQRS Pattern

```
┌─────────────────────────────────────────────────────────────────┐
│                    CQRS PATTERN                                    │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  COMMAND SIDE                          QUERY SIDE                  │
│  ───────────                          ──────────                  │
│                                                                  │
│  User ──► Command ──► Handler          User ──► Query ──► Handler│
│                    │                                       │        │
│                    ▼                                       ▼        │
│           ┌──────────────┐                     ┌──────────────┐ │
│           │ Validation   │                     │ Read Model   │ │
│           └──────┬───────┘                     └──────┬───────┘ │
│                  │                                   │           │
│                  ▼                                   ▼           │
│           ┌──────────────┐                     ┌──────────────┐ │
│           │ Business    │                     │ Projection   │ │
│           │ Logic      │                     │ (DTO)       │ │
│           └──────┬───────┘                     └──────┬───────┘ │
│                  │                                   │           │
│                  ▼                                   ▼           │
│           ┌──────────────┐                     ┌──────────────┐ │
│           │ Repository  │                     │ Response    │ │
│           │ (Write)   │                     │             │ │
│           └──────────────┘                     └──────────────┘ │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

## 4. Database Schema

### 4.1 Entity Relationship Diagram

```
┌──────────────┐       ┌───────────────────┐       ┌──────────────────┐
│    users    │       │   documents       │       │ document_chunks │
│──────────────│       │───────────────────│       │────────────────│
│ id (PK)    │       │ id (PK)           │       │ id (PK)        │
│ email      │       │ title             │       │ document_id(FK) │
│ password   │       │ storage_path      │       │ chunk_index    │
│ full_name  │       │ file_size        │       │ content        │
│ role       │       │ status           │       │ embedding(768) │
│ department │       │ uploader_id (FK) │       │ page_number    │
│ is_active  │       │ category_id (FK) │       │ token_count    │
└─────┬──────┘       └─────────┬─────────┘       └───────┬────────┘
      │                         │                          │
      │ 1:N                    │ 1:N                     │
      │                         │                          │
      ▼                         ▼                          ▼
┌──────────────┐       ┌───────────────────┐
│chat_sessions│       │ document_categories│
│──────────────│       │───────────────────│
│ id (PK)    │       │ id (PK)           │
│ user_id(FK)│       │ name              │
│ title      │       │ slug              │
│ message_cnt│       │ parent_id (FK)    │
└─────┬──────┘       └───────────────────┘
      │
      │ 1:N
      ▼
┌──────────────────┐
│  chat_messages   │
│──────────────────│
│ id (PK)         │
│ session_id (FK) │
│ role            │
│ content         │
│ citations (JSON)│
│ sources_used    │
│ token_count     │
│ latency_ms      │
└──────────────────┘
```

### 4.2 pgvector Configuration

```sql
-- Vector column configuration
content_embedding VECTOR(768)  -- Ollama nomic-embed-text dimensions

-- HNSW index for fast similarity search
CREATE INDEX idx_chunks_embedding_hnsw
ON document_chunks
USING hnsw (embedding vector_cosine_ops)
WITH (m = 16, ef_construction = 64);

-- Hybrid search: Vector + Full-text
CREATE INDEX idx_chunks_content_fts
ON document_chunks
USING GIN (to_tsvector('vietnamese', content));
```

---

## 5. API Design

### 5.1 RESTful Endpoints

```
┌─────────────────────────────────────────────────────────────────┐
│                    API ENDPOINTS                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  AUTH (/api/v1/auth)                                             │
│  ├── POST   /register          - Đăng ký                       │
│  ├── POST   /login             - Đăng nhập                       │
│  ├── POST   /refresh           - Refresh token                   │
│  ├── POST   /logout            - Đăng xuất                      │
│  └── GET    /me                - Thông tin user hiện tại         │
│                                                                  │
│  DOCUMENTS (/api/v1/documents)                                  │
│  ├── POST   /upload            - Upload tài liệu                 │
│  ├── GET    /                 - Danh sách tài liệu              │
│  ├── GET    /{id}             - Chi tiết tài liệu              │
│  ├── DELETE /{id}             - Xóa tài liệu                   │
│  └── GET    /{id}/status      - Trạng thái xử lý               │
│                                                                  │
│  SEARCH (/api/v1/search)                                        │
│  ├── GET    /?q={query}       - Semantic search                 │
│  └── POST   /hybrid           - Hybrid search (vector + keyword) │
│                                                                  │
│  CHAT (/api/v1/chat)                                            │
│  ├── GET    /sessions        - Danh sách phiên chat            │
│  ├── POST   /sessions         - Tạo phiên chat mới              │
│  ├── GET    /sessions/{id}    - Chi tiết phiên chat            │
│  ├── DELETE /sessions/{id}   - Xóa phiên chat                  │
│  ├── POST   /sessions/{id}/messages  - Gửi tin nhắn (RAG)    │
│  └── GET    /sessions/{id}/messages  - Lịch sử tin nhắn       │
│                                                                  │
│  ADMIN (/api/v1/admin)                                          │
│  ├── GET    /stats           - Thống kê dashboard              │
│  ├── GET    /users           - Quản lý users                   │
│  └── GET    /documents       - Quản lý tài liệu                │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### 5.2 Response Format

```json
// Success Response
{
  "success": true,
  "data": { ... },
  "message": "Operation successful"
}

// Error Response
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Email đã được sử dụng",
    "details": [...]
  }
}
```

---

## 6. AI Integration - Ollama

### 6.1 Ollama Configuration

```yaml
# docker-compose.yml
ollama:
  image: ollama/ollama:latest
  environment:
    CUDA_VISIBLE_DEVICES: "0"
    OLLAMA_NUM_PARALLEL: 4
    OLLAMA_MAX_LOADED_MODELS: 2
  volumes:
    - ollama_data:/root/.ollama
```

### 6.2 Models

| Model | Type | Dimensions | VRAM | Use Case |
|-------|------|------------|------|----------|
| llama3.2:3b | Chat | N/A | 4GB | Primary chatbot |
| nomic-embed-text | Embedding | 768 | 1GB | Semantic search |

### 6.3 RAG Pipeline

```
┌─────────────────────────────────────────────────────────────────┐
│                    RAG PIPELINE                                    │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  1. QUERY EMBEDDING                                             │
│     User Query → Ollama /api/embeddings → Query Vector (768)    │
│                                                                  │
│  2. RETRIEVAL                                                  │
│     Query Vector → pgvector HNSW → Top-K Chunks (cosine sim)    │
│                                                                  │
│  3. CONTEXT ASSEMBLY                                          │
│     Top-K Chunks → Format với metadata → Context String         │
│                                                                  │
│  4. PROMPT ENGINEERING                                         │
│     System Prompt + Context + User Query → Full Prompt          │
│                                                                  │
│  5. GENERATION                                                 │
│     Full Prompt → Ollama /api/chat → Response + Citations       │
│                                                                  │
│  6. POST-PROCESSING                                           │
│     Response → Extract Citations → Format Output                 │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

## 7. Deployment

### 7.1 Docker Services

| Service | Image | Ports | Purpose |
|---------|-------|-------|---------|
| nginx | nginx:alpine | 80, 443 | Reverse proxy |
| postgresql | pgvector/pgvector:pg16 | 5432 | Database |
| redis | redis:7-alpine | 6379 | Cache |
| minio | minio/minio:latest | 9000, 9001 | Object storage |
| ollama | ollama/ollama:latest | 11434 | Local LLM |
| backend | custom (.NET 8) | 5000 | API server |
| frontend | custom (Next.js) | 3000 | Web app |
| seq | datalust/seq:latest | 5340 | Log aggregation |

### 7.2 Production Checklist

```
□ Thay đổi tất cả passwords mặc định
□ Tạo JWT_SECRET ngẫu nhiên (32+ ký tự)
□ Cấu hình SSL certificates
□ Enable firewall (chỉ mở 80, 443)
□ Setup automated backups
□ Configure monitoring & alerting
□ Security hardening (non-root users, etc.)
```

---

**Tài liệu**: AI Base Framework - System Architecture
**Ngày cập nhật**: 2026-05-03
**Phiên bản**: 1.0
