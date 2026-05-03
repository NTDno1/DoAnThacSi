# AIBaseFramework - Backend .NET 8 API

## Mục lục

1. [Giới thiệu](#giới-thiệu)
2. [Cấu trúc Project](#cấu-trúc-project)
3. [Yêu cầu](#yêu-cầu)
4. [Setup](#setup)
5. [Chạy Development](#chạy-development)
6. [API Endpoints](#api-endpoints)
7. [Kiến trúc](#kiến-trúc)

---

## Giới thiệu

Backend API cho hệ thống **Semantic Search + RAG Chatbot** sử dụng:

- **.NET 8** - ASP.NET Core Web API
- **Clean Architecture** - Phân tách layer rõ ràng
- **CQRS** - Command Query Responsibility Segregation với MediatR
- **Entity Framework Core** - Database access
- **pgvector** - Vector similarity search
- **Ollama** - Local LLM (thay thế OpenAI)

---

## Cấu trúc Project

```
src/AIBaseFramework.API/
├── Program.cs                      # Entry point, DI configuration
├── appsettings.json                # Configuration
│
├── Controllers/                   # API Controllers
│   ├── AuthController.cs          # Authentication endpoints
│   ├── DocumentsController.cs      # Document CRUD
│   ├── SearchController.cs        # Semantic search
│   ├── ChatController.cs          # RAG chatbot
│   ├── AdminController.cs         # Admin endpoints
│   └── HealthController.cs         # Health check
│
├── Services/                      # Business Logic Interfaces
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IDocumentService.cs
│   │   ├── ISearchService.cs
│   │   ├── IChatService.cs
│   │   ├── IEmbeddingService.cs
│   │   └── IOllamaService.cs
│   │
│   └── Implementations/
│       ├── AuthService.cs
│       ├── DocumentService.cs
│       ├── SearchService.cs
│       ├── ChatService.cs
│       └── OllamaService.cs
│
├── Domain/                        # Domain Layer (Core Business Logic)
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Document.cs
│   │   ├── DocumentChunk.cs
│   │   ├── ChatSession.cs
│   │   └── ChatMessage.cs
│   │
│   ├── Enums/
│   │   ├── UserRole.cs
│   │   ├── DocumentStatus.cs
│   │   └── MessageRole.cs
│   │
│   └── Interfaces/
│       ├── IUserRepository.cs
│       ├── IDocumentRepository.cs
│       └── IChatRepository.cs
│
├── Infrastructure/                # Infrastructure Layer
│   ├── Data/
│   │   ├── AppDbContext.cs      # EF Core DbContext
│   │   └── Configurations/       # Entity configurations
│   │
│   ├── AI/                       # AI Services
│   │   ├── OllamaService.cs
│   │   ├── EmbeddingService.cs
│   │   └── RAGPipeline.cs
│   │
│   ├── Storage/                  # MinIO Storage
│   │   └── MinIOService.cs
│   │
│   └── Cache/                    # Redis Cache
│       └── RedisService.cs
│
├── Application/                  # Application Layer (Use Cases)
│   ├── DTOs/                     # Data Transfer Objects
│   │   ├── Auth/
│   │   ├── Document/
│   │   ├── Search/
│   │   └── Chat/
│   │
│   ├── Commands/                  # CQRS Commands
│   │   ├── Auth/
│   │   ├── Document/
│   │   └── Chat/
│   │
│   ├── Queries/                  # CQRS Queries
│   │   ├── Auth/
│   │   ├── Document/
│   │   └── Search/
│   │
│   └── Behaviors/                # MediatR Pipeline Behaviors
│       ├── LoggingBehavior.cs
│       ├── ValidationBehavior.cs
│       └── ExceptionBehavior.cs
│
└── Common/                       # Shared Utilities
    ├── Constants/
    ├── Extensions/
    ├── Middleware/
    └── Exceptions/
```

---

## Yêu cầu

- .NET 8 SDK
- Docker Desktop (cho PostgreSQL, Redis, MinIO, Ollama)
- Visual Studio 2022+ hoặc VS Code

---

## Setup

### 1. Restore packages

```bash
cd src/AIBaseFramework.API
dotnet restore
```

### 2. Database Migration

```bash
# Tạo migration
dotnet ef migrations add InitialCreate

# Apply migration
dotnet ef database update
```

### 3. Chạy Development

```bash
dotnet run
```

API sẽ chạy tại `http://localhost:5000`

---

## Chạy Development

### Với Docker (Khuyến nghị)

```bash
cd 02.INFRASTRUCTURE
docker-compose -f docker/docker-compose.yml up -d
```

### Development không Docker

1. Start PostgreSQL, Redis, MinIO, Ollama riêng
2. Update `appsettings.Development.json` với connection strings
3. Run `dotnet run`

---

## API Endpoints

### Authentication

| Method | Endpoint | Mô tả |
|--------|----------|--------|
| POST | `/api/v1/auth/register` | Đăng ký |
| POST | `/api/v1/auth/login` | Đăng nhập |
| POST | `/api/v1/auth/refresh` | Refresh token |
| POST | `/api/v1/auth/logout` | Đăng xuất |
| GET | `/api/v1/auth/me` | Profile hiện tại |

### Documents

| Method | Endpoint | Mô tả |
|--------|----------|--------|
| POST | `/api/v1/documents/upload` | Upload document |
| GET | `/api/v1/documents` | Danh sách documents |
| GET | `/api/v1/documents/{id}` | Chi tiết document |
| DELETE | `/api/v1/documents/{id}` | Xóa document |
| GET | `/api/v1/documents/{id}/status` | Trạng thái xử lý |

### Search

| Method | Endpoint | Mô tả |
|--------|----------|--------|
| GET | `/api/v1/search?q={query}` | Semantic search |
| POST | `/api/v1/search/hybrid` | Hybrid search |

### Chat

| Method | Endpoint | Mô tả |
|--------|----------|--------|
| GET | `/api/v1/chat/sessions` | Danh sách sessions |
| POST | `/api/v1/chat/sessions` | Tạo session mới |
| POST | `/api/v1/chat/sessions/{id}/messages` | Gửi message |
| GET | `/api/v1/chat/sessions/{id}/messages` | Lịch sử messages |

### Admin

| Method | Endpoint | Mô tả |
|--------|----------|--------|
| GET | `/api/v1/admin/stats` | Dashboard statistics |
| GET | `/api/v1/admin/users` | Quản lý users |
| GET | `/api/v1/admin/documents` | Quản lý documents |

---

## Kiến trúc

### Clean Architecture Layers

```
┌─────────────────────────────────────────────────────┐
│                 PRESENTATION LAYER                    │
│  (Controllers, DTOs, API Responses)               │
└─────────────────────┬─────────────────────────────┘
                      │
┌─────────────────────▼─────────────────────────────┐
│                 APPLICATION LAYER                   │
│  (Commands, Queries, MediatR Handlers)            │
└─────────────────────┬─────────────────────────────┘
                      │
┌─────────────────────▼─────────────────────────────┐
│                   DOMAIN LAYER                      │
│  (Entities, Enums, Domain Services)               │
└─────────────────────┬─────────────────────────────┘
                      │
┌─────────────────────▼─────────────────────────────┐
│                INFRASTRUCTURE LAYER                 │
│  (DbContext, AI Services, Storage, Cache)         │
└─────────────────────────────────────────────────────┘
```

### Dependency Flow

```
Controllers → Services → Repositories → Infrastructure
     ↑            ↑          ↑            ↑
     └────────────┴──────────┴────────────┘
              Dependency Injection
```

---

## Testing

```bash
dotnet test
```

---

## License

Đề tài Thạc sĩ - AI Base Framework
