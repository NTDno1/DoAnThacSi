# KẾ HOẠCH TRIỂN KHAI VÀ CODE - FRAMEWORK TÍCH HỢP AI (BASE PROJECT)

**Dự án Base/Common**: Framework tích hợp AI vào bất kỳ hệ thống nào
**Ngày tạo**: 2026-05-03
**Phiên bản**: 1.0
**Trạng thái**: Planning

---

## MỤC LỤC

1. [Tổng quan Dự án](#1-tổng-quan-dự-án)
2. [Kiến trúc Hệ thống](#2-kiến-trúc-hệ-thống)
3. [Lộ trình Triển khai](#3-lộ-trình-triển-khai)
4. [Cấu trúc Thư mục](#4-cấu-trúc-thư-mục)
5. [Yêu cầu Hệ thống](#5-yêu-cầu-hệ-thống)
6. [Quick Start](#6-quick-start)
7. [Công nghệ Sử dụng](#7-công-nghệ-sử-dụng)

---

## 1. TỔNG QUAN DỰ ÁN

### 1.1 Mục đích của Framework

Đây là **dự án base/common** được thiết kế để **áp dụng cho tất cả các hệ thống** nếu muốn chuyển đổi thêm tính năng AI vào trong hệ thống. Framework này cung cấp nền tảng tích hợp AI 100% offline, có thể nhúng vào bất kỳ ứng dụng web, mobile, hoặc hệ thống doanh nghiệp nào.

### 1.2 Mô tả Dự án

Framework **AI Integration Base** giúp các nhà phát triển:

- **Tích hợp AI vào hệ thống** hiện có một cách dễ dàng
- **Upload tài liệu** (PDF, DOCX, TXT) để AI có thể truy vấn
- **Tìm kiếm ngữ nghĩa** bằng câu hỏi tự nhiên (Tiếng Việt)
- **Chat với tài liệu** bằng RAG (Retrieval-Augmented Generation)
- **Quản lý tài liệu** và phân quyền truy cập
- Hoạt động **100% offline** - không phụ thuộc vào dịch vụ AI bên ngoài

### 1.3 Mục tiêu Chính

```
┌─────────────────────────────────────────────────────────────────┐
│                    MỤC TIÊU DỰ ÁN                               │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  1. Xây dựng MVP trong 30 ngày                                 │
│     ├── Week 1: Foundation (Auth, Upload)                      │
│     ├── Week 2: Processing (Extract, Chunk, Embed)              │
│     ├── Week 3: Search + Chat (RAG Pipeline)                    │
│     └── Week 4: Polish + Admin Dashboard                        │
│                                                                 │
│  2. Đạt tiêu chuẩn Luận văn Thạc sĩ                            │
│     ├── Code quality: Clean Architecture, SOLID                  │
│     ├── Documentation: 6 chapters, 30+ references               │
│     └── Technical depth: RAG, Vector Search, pgvector          │
│                                                                 │
│  3. Production-ready system                                     │
│     ├── 99.9% uptime                                            │
│     ├── Search P95 < 1s, Chat P95 < 5s                         │
│     └── Security: JWT, RLS, Rate Limiting                       │
│                                                                 │
│  4. FRAMEWORK BASE - Áp dụng cho mọi hệ thống                  │
│     ├── Tách biệt AI layer khỏi business logic                 │
│     ├── Có thể nhúng vào web/mobile/enterprise app            │
│     └── Không phục vụ cho một web cụ thể nào                   │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

### 1.4 Đặc điểm Nổi bật

| Đặc điểm | Mô tả |
|-----------|--------|
| **100% Offline** | Sử dụng Ollama (LLM local) thay vì OpenAI API |
| **Base Framework** | Có thể tích hợp vào bất kỳ hệ thống nào |
| **Tiếng Việt** | Hỗ trợ đầy đủ Tiếng Việt trong search và chat |
| **RAG Pipeline** | Retrieval-Augmented Generation với citations |
| **Vector Search** | Semantic search với pgvector + HNSW index |
| **Hybrid Search** | Kết hợp semantic (70%) + keyword (30%) |

---

## 2. KIẾN TRÚC HỆ THỐNG

### 2.1 Sơ đồ Kiến trúc Tổng thể

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│           KIẾN TRÚC HỆ THỐNG TỔNG THỂ - AI INTEGRATION BASE FRAMEWORK         │
│                   100% Offline • Multi-System Compatible • RAG Ready            │
└─────────────────────────────────────────────────────────────────────────────────┘

    ┌─────────────────────────────────────────────────────────────────────────┐
    │                      PRESENTATION TIER (Client Layer)                     │
    │  ┌─────────────────────────────────────────────────────────────────────┐ │
    │  │                        Next.js 14 + TypeScript                        │ │
    │  │   Zustand (State)  •  TanStack Query (Server State)  •  TailwindCSS │ │
    │  └─────────────────────────────────────────────────────────────────────┘ │
    └─────────────────────────────────────────────────────────────────────────┘
                                      │
                                      │ HTTPS/WSS
                                      ▼
    ┌─────────────────────────────────────────────────────────────────────────┐
    │                      API GATEWAY / NGINX                                 │
    │   Rate Limiting  •  SSL Termination  •  Load Balancing  •  Caching      │
    └─────────────────────────────────────────────────────────────────────────┘
                                      │
                                      │ Internal HTTP
                                      ▼
    ┌─────────────────────────────────────────────────────────────────────────┐
    │                      BUSINESS LOGIC TIER (Backend .NET 8)                │
    │  ┌─────────────────────────────────────────────────────────────────────┐ │
    │  │                    MEDIATR PIPELINE                                  │ │
    │  │  Logging → Validation → Cache → Auth → CORS → Exception Handler    │ │
    │  └─────────────────────────────────────────────────────────────────────┘ │
    │  ┌─────────────────────────────────────────────────────────────────────┐ │
    │  │                    CQRS HANDLERS                                    │ │
    │  │  Auth  •  Documents  •  Search  •  Chat  •  Admin                  │ │
    │  └─────────────────────────────────────────────────────────────────────┘ │
    └─────────────────────────────────────────────────────────────────────────┘
                    │
                    ├───┬─────────────────────┬──────┐
                    ▼   ▼                     ▼      ▼
    ┌───────────────────────┐  ┌───────────────────┐  ┌───────────────────────┐
    │   BACKGROUND WORKER    │  │   AI SERVICES     │  │   MESSAGE QUEUE       │
    │      (Hangfire)        │  │   (Ollama)        │  │   (Redis Streams)     │
    │                        │  │                   │  │                       │
    │  • Document Processing │  │  • Embedding      │  │  • Document Queue    │
    │  • OCR Processing     │  │  • LLM Chat       │  │  • Search Queue       │
    │  • Vector Indexing    │  │  • RAG Pipeline   │  │  • Notification       │
    │  • Cleanup Jobs       │  │                   │  │                       │
    └───────────────────────┘  └───────────────────┘  └───────────────────────┘
                                        │
                                        ▼
    ┌─────────────────────────────────────────────────────────────────────────┐
    │                          DATA TIER (Persistence)                        │
    │  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌────────────┐ │
    │  │ PostgreSQL 16 │  │ pgvector    │  │   Redis 7.x  │  │   MinIO    │ │
    │  │              │  │  (Vectors)  │  │   (Cache)    │  │ (Storage)  │ │
    │  │  • Users     │  │             │  │  • Sessions  │  │  • PDFs    │ │
    │  │  • Documents │  │  • Chunks   │  │  • Cache     │  │  • DOCX    │ │
    │  │  • Chats     │  │  • Embeds   │  │  • Queue     │  │  • Images  │ │
    │  │ 1536 dim     │  │  HNSW idx   │  │              │  │            │ │
    │  └──────────────┘  └──────────────┘  └──────────────┘  └────────────┘ │
    └─────────────────────────────────────────────────────────────────────────┘

    ┌─────────────────────────────────────────────────────────────────────────┐
    │                     BASE FRAMEWORK LAYER (AI Integration)                │
    │                                                                              │
    │  Đây là LỚP TÍCH HỢP AI - có thể nhúng vào bất kỳ hệ thống nào:        │
    │                                                                              │
    │    ┌─────────────────┐   ┌─────────────────┐   ┌─────────────────┐        │
    │    │   Web App       │   │   Mobile App    │   │  Enterprise    │        │
    │    │  (Next.js)      │   │   (React Native)│   │  System        │        │
    │    └────────┬────────┘   └────────┬────────┘   └────────┬────────┘        │
    │             │                       │                       │                │
    │             └───────────────────────┼───────────────────────┘                │
    │                                     │                                        │
    │                              ┌──────▼──────┐                                  │
    │                              │ AI Gateway  │                                  │
    │                              │ (This API) │                                  │
    │                              └─────────────┘                                  │
    │                                                                              │
    └─────────────────────────────────────────────────────────────────────────┘
```

### 2.2 Luồng Dữ liệu Chính

```
╔════════════════════════════════════════════════════════════════════════════════╗
║  LUỒNG 1: DOCUMENT UPLOAD & PROCESSING (Write Path)                           ║
╠════════════════════════════════════════════════════════════════════════════════╣
║                                                                                ║
║  User ──► Upload ──► Validate ──► MinIO ──► Queue ──► Background Worker        ║
║                                      │           │               │             ║
║                                      │           │               ▼             ║
║                                      │           │     ┌─────────────────┐     ║
║                                      │           │     │ TEXT EXTRACTION │     ║
║                                      │           │     │   (iText7)      │     ║
║                                      │           │     └────────┬────────┘     ║
║                                      │           │              │               ║
║                                      │           │              ▼               ║
║                                      │           │     ┌─────────────────┐     ║
║                                      │           │     │ CHUNKING        │     ║
║                                      │           │     │ (512 tokens)    │     ║
║                                      │           │     └────────┬────────┘     ║
║                                      │           │              │               ║
║                                      │           │              ▼               ║
║                                      │           │     ┌─────────────────┐     ║
║                                      │           │     │ EMBEDDING       │     ║
║                                      │           │     │ (Ollama nomic)  │     ║
║                                      │           │     └────────┬────────┘     ║
║                                      │           │              │               ║
║                                      │           │              ▼               ║
║                                      │           │     ┌─────────────────┐     ║
║                                      │           │     │ VECTOR INDEX    │     ║
║                                      │           │     │ (pgvector HNSW) │     ║
║                                      │           │     └────────┬────────┘     ║
║                                      │           │              │               ║
║                                      │           │              ▼               ║
║                                      │           │     ┌─────────────────┐     ║
║                                      │           └────►│ POSTGRES       │     ║
║                                      │               │   (Indexed)     │     ║
║                                      │               └─────────────────┘     ║
╚════════════════════════════════════════════════════════════════════════════════╝

╔════════════════════════════════════════════════════════════════════════════════╗
║  LUỒNG 2: SEMANTIC SEARCH (Read Path)                                         ║
╠════════════════════════════════════════════════════════════════════════════════╣
║                                                                                ║
║  User ──► Query ──► Embedding ──► Vector Search ──► Top-K Results             ║
║                      │              │               │                         ║
║                      ▼              ▼               ▼                         ║
║              ┌─────────────┐ ┌──────────────┐ ┌───────────────┐               ║
║              │  Redis      │ │  pgvector    │ │  Metadata     │               ║
║              │  (Cache)   │ │  (HNSW)     │ │  + Filters    │               ║
║              └─────────────┘ └──────────────┘ └───────────────┘               ║
║                                     │                                         ║
║                                     ▼                                         ║
║                            ┌─────────────────┐                                ║
║                            │ RERANKING       │                                ║
║                            │ (Cross-Encoder) │                                ║
║                            └────────┬────────┘                                ║
║                                     │                                         ║
║                                     ▼                                         ║
║                              ┌─────────────┐                                 ║
║                              │  Response   │                                 ║
║                              │  + Scores   │                                 ║
╚════════════════════════════════════════════════════════════════════════════════╝

╔════════════════════════════════════════════════════════════════════════════════╗
║  LUỒNG 3: RAG CHATBOT (AI-Enhanced Path)                                      ║
╠════════════════════════════════════════════════════════════════════════════════╣
║                                                                                ║
║  User ──► Message ──► Query Understanding ──► Retrieval ──► Generation         ║
║                     │                    │                │                    ║
║                     ▼                    ▼                ▼                    ║
║              ┌─────────────┐    ┌──────────────┐   ┌───────────────┐          ║
║              │  Session    │    │  Vector +   │   │  Ollama       │          ║
║              │  History    │    │  Keyword    │   │  (llama3.2)   │          ║
║              └─────────────┘    └──────┬───────┘   └───────┬───────┘          ║
║                                        │                   │                   ║
║                                        ▼                   ▼                   ║
║                                ┌──────────────┐    ┌─────────────────┐       ║
║                                │ RRF + MMR    │    │ PROMPT + RAG    │       ║
║                                │ (Hybrid)     │    │ CONTEXT         │       ║
║                                └──────┬───────┘    └────────┬────────┘       ║
║                                       │                  │                  ║
║                                       │                  ▼                  ║
║                                       │         ┌─────────────────┐          ║
║                                       │         │ STREAMING       │          ║
║                                       │         │ RESPONSE        │          ║
║                                       │         └────────┬────────┘          ║
║                                       │                  │                  ║
║                                       │                  ▼                  ║
║                                       │         ┌─────────────────┐          ║
║                                       │         │ CITATIONS +     │          ║
║                                       └────────►│ SOURCES         │          ║
║                                                 └─────────────────┘          ║
╚════════════════════════════════════════════════════════════════════════════════╝
```

---

## 3. LỘ TRÌNH TRIỂN KHAI

### 3.1 Tổng quan 30 ngày

```
┌────────────────────────────────────────────────────────────────────────────────┐
│                           LỘ TRÌNH 30 NGÀY MVP                                 │
├────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  TUẦN 1: FOUNDATION (Days 1-7)                                                │
│  ├── Day 1-2: Setup Infrastructure                                              │
│  │         • Docker + Docker Compose                                           │
│  │         • PostgreSQL + pgvector + Redis + MinIO                             │
│  │         • Ollama (LLM local)                                               │
│  │         • Backend: .NET 8 project structure                                 │
│  │         • Frontend: Next.js + TypeScript + Tailwind                        │
│  │                                                                            │
│  ├── Day 3-4: Authentication                                                   │
│  │         • JWT tokens (access + refresh)                                    │
│  │         • Login/Register UI + API                                          │
│  │         • User roles (Admin, User, Guest)                                  │
│  │                                                                            │
│  └── Day 5-7: Document Upload Basic                                           │
│            • MinIO integration                                                 │
│            • File validation (size, format)                                     │
│            • Document CRUD API + UI                                            │
│                                                                                 │
│  TUẦN 2: CORE PROCESSING (Days 8-14)                                         │
│  ├── Day 8-9: Text Extraction                                                 │
│  │         • PDF extraction (iText7)                                          │
│  │         • DOCX extraction (OpenXML)                                        │
│  │         • TXT extraction                                                   │
│  │                                                                            │
│  ├── Day 10-11: Chunking & Embedding                                          │
│  │         • Recursive character splitting (512 tokens)                       │
│  │         • Ollama embedding service (nomic-embed-text)                      │
│  │         • Redis cache for embeddings                                       │
│  │                                                                            │
│  └── Day 12-14: Vector Indexing                                               │
│            • pgvector setup + HNSW index                                      │
│            • Background job processing                                         │
│            • Progress tracking                                                 │
│                                                                                 │
│  TUẦN 3: SEARCH + CHAT (Days 15-21)                                          │
│  ├── Day 15-17: Semantic Search                                               │
│  │         • Vector similarity search                                         │
│  │         • Hybrid search (semantic + keyword)                              │
│  │         • Re-ranking (cross-encoder)                                      │
│  │                                                                            │
│  └── Day 18-21: RAG Chatbot                                                   │
│            • Session management                                                │
│            • RAG pipeline                                                     │
│            • Citations + sources                                               │
│            • Streaming responses                                               │
│                                                                                 │
│  TUẦN 4: POLISH + ADMIN (Days 22-30)                                         │
│  ├── Day 22-24: Admin Dashboard                                               │
│  │         • Statistics & analytics                                           │
│  │         • User management                                                   │
│  │         • Document moderation                                              │
│  │                                                                            │
│  ├── Day 25-27: Polish & Testing                                             │
│  │         • UI/UX improvements                                               │
│  │         • Unit tests + integration tests                                  │
│  │         • Performance optimization                                         │
│  │                                                                            │
│  └── Day 28-30: Documentation & Deploy                                       │
│            • README + API docs                                                │
│            • Deployment scripts                                               │
│            • Final testing                                                    │
│                                                                                 │
└────────────────────────────────────────────────────────────────────────────────┘
```

### 3.2 Milestones

| Milestone | Ngày | Deliverables |
|-----------|------|--------------|
| **M1** | Day 2 | Infrastructure ready, Docker working |
| **M2** | Day 4 | Authentication complete |
| **M3** | Day 7 | Basic upload working |
| **M4** | Day 11 | Text extraction working |
| **M5** | Day 14 | Embedding & indexing complete |
| **M6** | Day 17 | Search working |
| **M7** | Day 21 | Chatbot working with RAG |
| **M8** | Day 28 | Admin dashboard complete |
| **M9** | Day 30 | MVP complete, deployable |

---

## 4. CẤU TRÚC THƯ MỤC

```
AIBaseFramework/
│
├── 01.DEPLOYMENT_PLAN/              # Kế hoạch triển khai chi tiết
│   ├── README.md                     # README tổng quan (file này)
│   ├── DEPLOYMENT_PLAN.md           # Kế hoạch chi tiết từng ngày
│   ├── TIMELINE.md                  # Timeline và milestones
│   └── CHECKLIST.md                 # Checklist triển khai
│
├── 02.INFRASTRUCTURE/               # Cơ sở hạ tầng
│   ├── scripts/
│   │   ├── setup.sh                 # Script setup ban đầu
│   │   ├── start.sh                 # Script khởi động
│   │   ├── stop.sh                  # Script dừng
│   │   ├── backup.sh                # Script backup database
│   │   ├── restore.sh               # Script restore database
│   │   ├── download-models.sh       # Script download Ollama models
│   │   └── health-check.sh          # Script kiểm tra health
│   │
│   └── docker/
│       ├── docker-compose.yml       # Development compose
│       ├── docker-compose.prod.yml  # Production compose
│       ├── Dockerfile.backend       # Backend .NET 8
│       ├── Dockerfile.frontend      # Frontend Next.js
│       ├── Dockerfile.ollama       # Ollama LLM
│       ├── nginx.conf               # Nginx config
│       ├── .env.example             # Environment template
│       └── init.sql                 # Database init script
│
├── 03.BACKEND/                      # Backend .NET 8
│   ├── AIBaseFramework.sln          # Solution file
│   ├── README.md                    # Backend README
│   │
│   └── src/
│       └── AIBaseFramework.API/     # Main API project
│           ├── Program.cs           # Entry point
│           ├── appsettings.json    # Configuration
│           │
│           ├── Controllers/         # API Controllers
│           │   ├── AuthController.cs
│           │   ├── DocumentsController.cs
│           │   ├── SearchController.cs
│           │   ├── ChatController.cs
│           │   └── AdminController.cs
│           │
│           ├── Services/             # Business Logic
│           │   ├── Interfaces/
│           │   │   ├── IAuthService.cs
│           │   │   ├── IDocumentService.cs
│           │   │   ├── ISearchService.cs
│           │   │   └── IChatService.cs
│           │   │
│           │   └── Implementations/
│           │       ├── AuthService.cs
│           │       ├── DocumentService.cs
│           │       ├── SearchService.cs
│           │       └── ChatService.cs
│           │
│           ├── Domain/               # Domain Layer
│           │   ├── Entities/
│           │   │   ├── User.cs
│           │   │   ├── Document.cs
│           │   │   ├── DocumentChunk.cs
│           │   │   ├── ChatSession.cs
│           │   │   └── ChatMessage.cs
│           │   │
│           │   ├── Enums/
│           │   │   ├── UserRole.cs
│           │   │   ├── DocumentStatus.cs
│           │   │   └── MessageRole.cs
│           │   │
│           │   └── Interfaces/
│           │       ├── IUserRepository.cs
│           │       └── IDocumentRepository.cs
│           │
│           ├── Infrastructure/       # Infrastructure Layer
│           │   ├── Data/
│           │   │   ├── AppDbContext.cs
│           │   │   └── Configurations/
│           │   │
│           │   ├── AI/
│           │   │   ├── OllamaService.cs
│           │   │   └── EmbeddingService.cs
│           │   │
│           │   ├── Storage/
│           │   │   └── MinIOService.cs
│           │   │
│           │   └── Cache/
│           │       └── RedisService.cs
│           │
│           ├── Application/          # Application Layer
│           │   ├── DTOs/
│           │   │   ├── Auth/
│           │   │   ├── Document/
│           │   │   ├── Search/
│           │   │   └── Chat/
│           │   │
│           │   ├── Commands/
│           │   │   ├── Auth/
│           │   │   ├── Document/
│           │   │   └── Chat/
│           │   │
│           │   ├── Queries/
│           │   │   ├── Document/
│           │   │   └── Search/
│           │   │
│           │   └── Behaviors/
│           │       ├── LoggingBehavior.cs
│           │       ├── ValidationBehavior.cs
│           │       └── ExceptionBehavior.cs
│           │
│           ├── Common/               # Shared utilities
│           │   ├── Constants/
│           │   ├── Extensions/
│           │   ├── Middleware/
│           │   └── Exceptions/
│           │
│           └── Tests/               # Unit tests
│               ├── Services.Tests/
│               └── Controllers.Tests/
│
├── 04.FRONTEND/                    # Frontend Next.js 14
│   ├── package.json
│   ├── README.md
│   │
│   └── src/
│       ├── app/                     # App Router
│       │   ├── layout.tsx
│       │   ├── page.tsx            # Trang chủ
│       │   │
│       │   ├── (auth)/             # Auth routes
│       │   │   ├── login/page.tsx
│       │   │   └── register/page.tsx
│       │   │
│       │   ├── (dashboard)/         # Protected routes
│       │   │   ├── layout.tsx
│       │   │   ├── documents/
│       │   │   ├── search/
│       │   │   ├── chat/
│       │   │   └── admin/
│       │   │
│       │   └── api/                # API routes
│       │
│       ├── components/              # React components
│       │   ├── ui/                 # Base UI components
│       │   ├── layout/             # Layout components
│       │   ├── auth/              # Auth components
│       │   ├── documents/          # Document components
│       │   ├── search/            # Search components
│       │   └── chat/              # Chat components
│       │
│       ├── hooks/                  # Custom hooks
│       │   ├── useAuth.ts
│       │   ├── useDocuments.ts
│       │   ├── useSearch.ts
│       │   └── useChat.ts
│       │
│       ├── lib/                    # Utilities
│       │   ├── api.ts             # API client
│       │   ├── auth.ts            # Auth helpers
│       │   └── utils.ts
│       │
│       ├── stores/                 # Zustand stores
│       │   ├── authStore.ts
│       │   ├── documentStore.ts
│       │   └── chatStore.ts
│       │
│       └── types/                  # TypeScript types
│           ├── auth.ts
│           ├── document.ts
│           └── chat.ts
│
└── 05.DOCUMENTATION/              # Tài liệu kỹ thuật
    ├── ARCHITECTURE.md            # Kiến trúc chi tiết
    ├── API_SPEC.md                # API specification
    ├── DATABASE_SCHEMA.md          # Database schema
    ├── RAG_PIPELINE.md           # RAG pipeline details
    └── DEPLOYMENT.md              # Hướng dẫn triển khai
```

---

## 5. YÊU CẦU HỆ THỐNG

### 5.1 Development Environment

| Component | Yêu cầu tối thiểu | Khuyến nghị |
|-----------|-------------------|--------------|
| **CPU** | 4 cores | 8 cores |
| **RAM** | 8 GB | 16 GB |
| **GPU** | Không bắt buộc | NVIDIA GPU 6GB+ VRAM |
| **Disk** | 20 GB free | 50 GB SSD |
| **OS** | Windows 10+, Ubuntu 20.04+ | Windows 11 / Ubuntu 22.04 |

### 5.2 Software Prerequisites

| Software | Phiên bản | Ghi chú |
|----------|-----------|---------|
| Docker Desktop | 4.0+ | Với Docker Compose |
| .NET SDK | 8.0+ | Backend development |
| Node.js | 20 LTS | Frontend development |
| Git | 2.0+ | Version control |

### 5.3 Ollama Models

```
┌─────────────────────────────────────────────────────────────────┐
│                    OLLAMA MODELS REQUIREMENTS                     │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  Model                  │ VRAM    │ Disk   │ Use Case           │
│  ───────────────────────┼─────────┼────────┼─────────────────── │
│  llama3.2:3b           │ 2 GB    │ 2 GB   │ Primary chat      │
│  nomic-embed-text       │ 1 GB    │ 274 MB │ Embeddings        │
│  ─────────────────────────────────────────────────────────────  │
│  OPTIONAL MODELS:                                              │
│  mistral:7b            │ 5 GB    │ 4 GB   │ Better quality    │
│  phi3:latest           │ 2 GB    │ 2 GB   │ Fast inference    │
│  qwen2.5:7b           │ 5 GB    │ 4 GB   │ Good Vietnamese   │
│  mxbai-embed-large     │ 1.5 GB  │ 670 MB │ Better embeddings │
│                                                                  │
│  Total Required: ~3.5 GB (with llama3.2:3b + nomic-embed-text)  │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

## 6. QUICK START

### 6.1 Clone và Setup

```bash
# Clone repository
git clone https://github.com/your-org/AIBaseFramework.git
cd AIBaseFramework/PlanTrienKhaiVaCode

# Copy environment file
cp 02.INFRASTRUCTURE/docker/.env.example .env
# Chỉnh sửa .env với password của bạn

# Khởi động tất cả services
docker-compose -f 02.INFRASTRUCTURE/docker/docker-compose.yml up -d

# Download Ollama models (lần đầu)
./02.INFRASTRUCTURE/scripts/download-models.sh

# Kiểm tra health
./02.INFRASTRUCTURE/scripts/health-check.sh
```

### 6.2 Truy cập Ứng dụng

| Service | URL | Credentials |
|---------|-----|-------------|
| Frontend | http://localhost:3000 | - |
| Backend API | http://localhost:5000 | - |
| API Swagger | http://localhost:5000/swagger | - |
| MinIO Console | http://localhost:9001 | minioadmin / [password] |
| Seq Logs | http://localhost:5340 | - |
| Ollama API | http://localhost:11434 | - |

### 6.3 Default Users

| Email | Password | Role |
|-------|----------|------|
| admin@yourcompany.com | admin123 | Admin |
| user@yourcompany.com | user123 | User |
| guest@yourcompany.com | guest123 | Guest |

---

## 7. CÔNG NGHỆ SỬ DỤNG

### 7.1 Backend Stack

| Layer | Technology | Purpose |
|-------|------------|---------|
| Runtime | .NET 8 | Application framework |
| API | ASP.NET Core 8 | REST API |
| ORM | Entity Framework Core 8 | Database access |
| CQRS | MediatR | Command/Query separation |
| Auth | JWT Bearer | Authentication |
| Validation | FluentValidation | Input validation |
| Logging | Serilog | Structured logging |
| Background Jobs | Hangfire | Async processing |
| Vector DB | pgvector | Semantic search |
| Cache | Redis 7 | Caching |
| Storage | MinIO | Object storage |
| LLM | Ollama | Local LLM inference |

### 7.2 Frontend Stack

| Layer | Technology | Purpose |
|-------|------------|---------|
| Framework | Next.js 14 | React framework |
| Language | TypeScript 5 | Type safety |
| UI | Tailwind CSS + shadcn/ui | Styling |
| State | Zustand | Client state |
| Server State | TanStack Query | Data fetching |
| Forms | React Hook Form | Form handling |
| Icons | Lucide React | Icon set |

### 7.3 Infrastructure Stack

| Component | Technology | Purpose |
|-----------|------------|---------|
| Container | Docker | Containerization |
| Orchestration | Docker Compose | Multi-container |
| Database | PostgreSQL 16 | Primary database |
| Vector Store | pgvector | Vector embeddings |
| Cache | Redis | Session & cache |
| Storage | MinIO | S3-compatible storage |
| Reverse Proxy | Nginx | Load balancing |
| Logging | Seq | Log aggregation |
| LLM | Ollama | Local AI inference |

---

## TIẾP THEO

Xem chi tiết từng phần:

- [DEPLOYMENT_PLAN.md](./01.DEPLOYMENT_PLAN/DEPLOYMENT_PLAN.md) - Kế hoạch chi tiết từng ngày
- [INFRASTRUCTURE README.md](../02.INFRASTRUCTURE/README.md) - Hướng dẫn Docker
- [BACKEND README.md](../03.BACKEND/README.md) - Backend structure
- [FRONTEND README.md](../04.FRONTEND/README.md) - Frontend structure

---

**Mục đích**: Framework base tích hợp AI - áp dụng cho mọi hệ thống
**Framework**: AI Integration Base (AIBaseFramework)
**Ngày cập nhật**: 2026-05-03
