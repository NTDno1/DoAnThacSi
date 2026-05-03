# KẾ HOẠCH TRIỂN KHAI CHI TIẾT - 30 NGÀY MVP

**Ngày tạo**: 2026-05-03
**Phiên bản**: 1.0
**Trạng thái**: Planning

---

## MỤC LỤC

1. [Tổng quan](#1-tổng-quan)
2. [Tuần 1: Foundation](#2-tuần-1-foundation-days-1-7)
3. [Tuần 2: Core Processing](#3-tuần-2-core-processing-days-8-14)
4. [Tuần 3: Search + Chat](#4-tuần-3-search--chat-days-15-21)
5. [Tuần 4: Polish + Admin](#5-tuần-4-polish--admin-days-22-30)
6. [Milestones](#6-milestones)
7. [Definition of Done](#7-definition-of-done)

---

## 1. TỔNG QUAN

### 1.1 Mục tiêu

Xây dựng **Minimum Viable Product (MVP)** cho hệ thống Semantic Search và RAG Chatbot trong **30 ngày**, đáp ứng các yêu cầu của đề tài Thạc sĩ.

### 1.2 Triết lý MVP

```
┌─────────────────────────────────────────────────────────────────┐
│                    TRIẾT LÝ MVP                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  1. SHIP EARLY, SHIP OFTEN                                      │
│     • Phát hành incremental, không đợi hoàn hảo                   │
│     • Mỗi tuần có working feature                                │
│                                                                  │
│  2. CORE VALUE FIRST                                            │
│     • Tập trung: Tìm kiếm + Chat với tài liệu                   │
│     • Không implement features không cần thiết                    │
│                                                                  │
│  3. MINIMUM VIABLE ≠ MINIMUM QUALITY                           │
│     • Code sạch, có unit tests                                   │
│     • UX tốt, không có dead buttons                              │
│     • Error handling đầy đủ                                      │
│                                                                  │
│  4. FOCUS ON USER PAIN POINTS                                   │
│     • Giải quyết triệt để vấn đề: "Tìm thông tin trong tài     │
│       liệu mất thời gian"                                       │
│                                                                  │
│  5. ITERATE BASED ON FEEDBACK                                   │
│     • Thu thập feedback từ users                                 │
│     • Điều chỉnh features dựa trên usage                        │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### 1.3 Features Priority

| Priority | Feature | Mô tả | Weeks |
|----------|---------|--------|-------|
| **P0** | Authentication | Login, Register, JWT | Week 1 |
| **P0** | Document Upload | MinIO, file validation | Week 1 |
| **P0** | Text Extraction | PDF, DOCX, TXT | Week 2 |
| **P0** | Chunking + Embedding | Ollama, Redis cache | Week 2 |
| **P0** | Semantic Search | Vector similarity, pgvector | Week 3 |
| **P0** | RAG Chatbot | RAG pipeline, citations | Week 3 |
| **P1** | Admin Dashboard | Stats, management | Week 4 |
| **P2** | Advanced Search | Hybrid, filters | Week 4 |
| **P2** | User Feedback | Thumbs up/down | Week 4 |

---

## 2. TUẦN 1: FOUNDATION (Days 1-7)

### 2.1 Day 1-2: Infrastructure Setup

#### Ngày 1: Docker + Basic Services

**Mục tiêu cuối ngày**: Tất cả Docker services chạy thành công

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 1 TASKS                                                    │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  [ ] Setup Docker Desktop / Docker Engine                        │
│  [ ] Clone repository và checkout develop branch                 │
│  [ ] Copy .env.example → .env và chỉnh sửa passwords           │
│  [ ] Pull images: postgres, redis, minio, ollama                │
│  [ ] Setup Docker Compose for all services                       │
│  [ ] Verify PostgreSQL connection với pgvector extension         │
│  [ ] Verify Redis connection                                    │
│  [ ] Verify MinIO bucket creation                               │
│  [ ] Verify Ollama API accessible                               │
│                                                                  │
│  DELIVERABLE: docker-compose ps shows all services healthy      │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

**Commands**:
```bash
# Pull base images
docker pull postgres:16
docker pull redis:7-alpine
docker pull minio/minio:latest
docker pull ollama/ollama:latest

# Start services
docker-compose up -d

# Verify
docker-compose ps
curl http://localhost:11434/api/tags  # Ollama
```

#### Ngày 2: .NET 8 Backend Project Structure

**Mục tiêu cuối ngày**: Backend project structure hoàn chỉnh, build thành công

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 2 TASKS                                                    │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  [ ] Create .NET 8 solution và API project                      │
│  [ ] Setup Clean Architecture folder structure                  │
│  [ ] Add NuGet packages:                                        │
│      • Microsoft.EntityFrameworkCore                            │
│      • Npgsql.EntityFrameworkCore.PostgreSQL                    │
│      • Npgsql.EntityFrameworkCore.Progresql                  │
│      • Microsoft.AspNetCore.Authentication.JwtBearer             │
│      • MediatR                                                │
│      • FluentValidation.AspNetCore                             │
│      • Serilog.AspNetCore                                     │
│      • Hangfire.AspNetCore                                     │
│      • StackExchange.Redis                                     │
│      • AWSSDK.S3 (for MinIO compatibility)                    │
│                                                                  │
│  [ ] Setup DbContext với PostgreSQL                            │
│  [ ] Configure Serilog logging → Seq                            │
│  [ ] Configure JWT authentication                               │
│  [ ] Add MediatR pipeline behaviors (logging, validation)       │
│  [ ] Build và verify: dotnet build                            │
│                                                                  │
│  DELIVERABLE: Backend project builds without errors             │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

**Commands**:
```bash
# Create solution
dotnet new sln -n AIBaseFramework
dotnet new webapi -n AIBaseFramework.API -o src/AIBaseFramework.API
dotnet sln add src/AIBaseFramework.API

# Add packages
cd src/AIBaseFramework.API
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package MediatR
dotnet add package FluentValidation.AspNetCore
dotnet add package Serilog.AspNetCore
dotnet add package Hangfire.AspNetCore
dotnet add package StackExchange.Redis

# Build
dotnet build
```

**Backend Project Structure**:
```
src/AIBaseFramework.API/
├── Controllers/           # API Controllers
├── Services/             # Business Logic
│   ├── Interfaces/
│   └── Implementations/
├── Domain/               # Domain Layer
│   ├── Entities/
│   ├── Enums/
│   └── Interfaces/
├── Infrastructure/       # Infrastructure Layer
│   ├── Data/
│   ├── AI/
│   ├── Storage/
│   └── Cache/
├── Application/          # Application Layer
│   ├── DTOs/
│   ├── Commands/
│   └── Queries/
├── Common/              # Shared
│   ├── Constants/
│   ├── Extensions/
│   └── Middleware/
└── Program.cs
```

### 2.2 Day 3-4: Authentication

#### Ngày 3: Auth API Implementation

**Mục tiêu cuối ngày**: Login/Register API hoạt động, JWT tokens được tạo

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 3 TASKS                                                    │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  [ ] Create User entity và database migration                    │
│  [ ] Implement IUserRepository                                  │
│  [ ] Implement AuthService:                                     │
│      • Register(email, password, name)                          │
│      • Login(email, password) → JWT tokens                      │
│      • RefreshToken(token) → new JWT                           │
│      • Logout(userId)                                          │
│                                                                  │
│  [ ] Create AuthController với endpoints:                       │
│      • POST /api/v1/auth/register                              │
│      • POST /api/v1/auth/login                                 │
│      • POST /api/v1/auth/refresh                              │
│      • POST /api/v1/auth/logout                               │
│      • GET /api/v1/auth/me                                    │
│                                                                  │
│  [ ] Add FluentValidation for RegisterRequest, LoginRequest     │
│  [ ] Hash password với BCrypt                                  │
│  [ ] Configure JWT Bearer authentication                        │
│  [ ] Run migration: dotnet ef migrations add InitAuth          │
│                                                                  │
│  DELIVERABLE: Auth API returns JWT tokens on successful login  │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

**Auth Flow**:
```
┌─────────────────────────────────────────────────────────────────┐
│                    AUTHENTICATION FLOW                            │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  REGISTER:                                                      │
│  User ──► [email, password] ──► Hash Password ──► Save User    │
│           ◄── 201 Created ──────────────────────────────────►  │
│                                                                  │
│  LOGIN:                                                         │
│  User ──► [email, password] ──► Verify Password ──► JWT Tokens │
│           ◄── {accessToken, refreshToken} ──────────────────►  │
│                                                                  │
│  ACCESS TOKEN (1 giờ):                                          │
│  • Contains: userId, email, role                               │
│  • Used for: API authorization                                 │
│                                                                  │
│  REFRESH TOKEN (7 ngày):                                        │
│  • Stored in database (refresh_tokens table)                    │
│  • Can be revoked                                              │
│  • Used to get new access token                                │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Ngày 4: Auth Frontend + Seed Data

**Mục tiêu cuối ngày**: Login/Register UI hoạt động, có thể login thành công

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 4 TASKS                                                    │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  FRONTEND:                                                      │
│  [ ] Setup Next.js project với TypeScript                       │
│  [ ] Install dependencies: axios, react-hook-form, zustand    │
│  [ ] Create AuthProvider wrapper                                │
│  [ ] Create Zustand authStore:                                 │
│      • user state                                               │
│      • accessToken                                               │
│      • login(), logout(), register()                           │
│      • persist to localStorage                                  │
│                                                                  │
│  [ ] Create Login page:                                         │
│      • Email + Password inputs                                  │
│      • Form validation                                          │
│      • Show/hide password                                       │
│      • Error messages                                           │
│      • Redirect to dashboard on success                          │
│                                                                  │
│  [ ] Create Register page:                                     │
│      • Name, Email, Password, Confirm Password                   │
│      • Form validation                                          │
│      • Redirect to login on success                            │
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Add seed data script với default users:                   │
│      • admin@yourcompany.com / admin123 (Admin)               │
│      • user@yourcompany.com / user123 (User)                   │
│      • guest@yourcompany.com / guest123 (Guest)              │
│                                                                  │
│  [ ] Run seed data                                              │
│                                                                  │
│  DELIVERABLE: Có thể login với admin user qua UI              │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### 2.3 Day 5-7: Document Upload Basic

#### Ngày 5: MinIO Integration + Document Entity

**Mục tiêu cuối ngày**: Có thể upload file lên MinIO

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 5 TASKS                                                    │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Add MinIO SDK (Minio .NET SDK)                           │
│  [ ] Create IMinIOService interface:                            │
│      • UploadFile(stream, bucket, objectName)                   │
│      • DownloadFile(bucket, objectName) → stream               │
│      • DeleteFile(bucket, objectName)                          │
│      • GetPresignedUrl(bucket, objectName)                     │
│                                                                  │
│  [ ] Create Document entity:                                    │
│      • Id, Title, OriginalFilename                             │
│      • StoragePath, FileSize, MimeType                         │
│      • FileHash (SHA-256 for deduplication)                    │
│      • CategoryId, UploaderId                                  │
│      • Status (Pending, Processing, Indexed, Failed)            │
│      • CreatedAt, UpdatedAt                                    │
│                                                                  │
│  [ ] Add DocumentCategory entity:                              │
│      • Id, Name, Slug, Description                             │
│      • ParentId (self-reference), Color                        │
│                                                                  │
│  [ ] Run migration: dotnet ef migrations add InitDocuments      │
│                                                                  │
│  DELIVERABLE: Có thể save document metadata vào DB            │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Ngày 6: Upload API + Progress Tracking

**Mục tiêu cuối ngày**: Upload API nhận file và lưu lên MinIO

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 6 TASKS                                                    │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Create DocumentService:                                   │
│      • UploadDocument(file, userId)                             │
│      • GetDocuments(userId, page, pageSize)                     │
│      • GetDocument(id)                                         │
│      • DeleteDocument(id)                                      │
│                                                                  │
│  [ ] Create DocumentsController:                                │
│      • POST /api/v1/documents/upload (multipart/form-data)      │
│      • GET /api/v1/documents (paginated list)                  │
│      • GET /api/v1/documents/{id}                             │
│      • DELETE /api/v1/documents/{id}                          │
│                                                                  │
│  [ ] Add file validation:                                      │
│      • Allowed types: PDF, DOCX, TXT                           │
│      • Max size: 50MB                                          │
│      • Virus scan (optional for MVP)                          │
│                                                                  │
│  [ ] Add file hash check for duplicate detection               │
│                                                                  │
│  DELIVERABLE: Upload file → MinIO + metadata saved             │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Ngày 7: Upload UI + Document List

**Mục tiêu cuối ngày**: Upload UI hoạt động, có thể xem danh sách document

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 7 TASKS                                                    │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  FRONTEND:                                                      │
│  [ ] Create documentStore (Zustand):                           │
│      • documents state                                          │
│      • fetchDocuments(), uploadDocument()                       │
│                                                                  │
│  [ ] Create Upload page:                                       │
│      • Drag & drop file zone                                    │
│      • File type validation (visual feedback)                   │
│      • Progress bar during upload                              │
│      • Success/error messages                                  │
│                                                                  │
│  [ ] Create Documents list page:                              │
│      • Grid/List view toggle                                   │
│      • File info: name, size, date, status                     │
│      • Delete button                                           │
│      • Pagination                                              │
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Add download endpoint:                                    │
│      • GET /api/v1/documents/{id}/download                    │
│                                                                  │
│  [ ] Add preview endpoint:                                     │
│      • GET /api/v1/documents/{id}/preview (extracted text)    │
│                                                                  │
│  DELIVERABLE: Upload + list documents working end-to-end      │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

## 3. TUẦN 2: CORE PROCESSING (Days 8-14)

### 3.1 Day 8-9: Text Extraction

#### Ngày 8: PDF Extraction

**Mục tiêu cuối ngày**: Extract text từ PDF file

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 8 TASKS                                                    │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Add PDF extraction library (iText7 or PdfPig)              │
│  [ ] Create ITextExtractionService:                            │
│      • ExtractTextFromPdf(stream) → string                      │
│      • ExtractTextWithMetadata(stream) → (text, pageCount)     │
│                                                                  │
│  [ ] Create page-level extraction for citations:                │
│      • Extract text per page                                   │
│      • Track page numbers                                     │
│                                                                  │
│  [ ] Handle OCR fallback for scanned PDFs:                     │
│      • Detect if PDF is scanned (no text layer)                │
│      • Use PaddleOCR if scanned                               │
│                                                                  │
│  [ ] Add DOCX extraction:                                      │
│      • ExtractTextFromDocx(stream) → string                    │
│                                                                  │
│  DELIVERABLE: Extract text from PDF/DOCX files                 │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Ngày 9: Text Cleaning + Organization

**Mục tiêu cuối ngày**: Text được clean và organize thành chunks

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 9 TASKS                                                    │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Create ITextCleaningService:                               │
│      • NormalizeUnicode(text) - NFC/NFD handling               │
│      • RemoveExtraWhitespace(text)                             │
│      • FixEncodingIssues(text)                                 │
│      • PreserveParagraphStructure(text)                         │
│                                                                  │
│  [ ] Create IChunkingService:                                  │
│      • ChunkText(text, chunkSize, overlap)                      │
│      • Support different strategies:                           │
│          - RecursiveCharacter (default)                         │
│          - Sentence                                            │
│          - Paragraph                                           │
│                                                                  │
│  [ ] Create Chunking strategy:                                 │
│      • Target: 512 tokens (~2000 chars)                        │
│      • Overlap: 20% (~100 tokens)                             │
│      • Split by: \n\n > \n > . > ! > ? > , > space           │
│                                                                  │
│  [ ] Add token counting utility                                │
│      • Estimate tokens với char/4 approximation               │
│                                                                  │
│  DELIVERABLE: Text cleaned, chunked into 512-token segments    │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### 3.2 Day 10-11: Embedding + Caching

#### Ngày 10: Ollama Embedding Service

**Mục tiêu cuối ngày**: Generate embeddings với Ollama

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 10 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Create Ollama service:                                     │
│      • IEmbeddingService: GenerateEmbedding(text) → float[]     │
│      • ILLMService: Chat(prompt, context) → response          │
│                                                                  │
│  [ ] Ollama API integration:                                    │
│      • Base URL: http://ollama:11434                           │
│      • Embeddings endpoint: /api/embeddings                     │
│      • Chat endpoint: /api/chat                               │
│                                                                  │
│  [ ] Model configuration:                                       │
│      • Chat model: llama3.2:3b                                │
│      • Embedding model: nomic-embed-text                       │
│      • Temperature: 0.3 (factual responses)                   │
│      • Max tokens: 2000                                       │
│                                                                  │
│  [ ] Error handling:                                           │
│      • Connection retry (3 attempts)                          │
│      • Timeout handling (60s for embedding)                   │
│      • Fallback response if Ollama unavailable                │
│                                                                  │
│  DELIVERABLE: Generate embeddings với Ollama API               │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Ngày 11: Redis Cache + Batch Embedding

**Mục tiêu cuối ngày**: Embeddings được cache, batch processing hoạt động

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 11 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Add Redis service:                                        │
│      • ICacheService: Get, Set, Delete, Exists                │
│      • Cache embedding results với TTL 30 days                 │
│      • Cache key: hash of text                                 │
│                                                                  │
│  [ ] Create IEmbeddingBatchService:                           │
│      • BatchGenerateEmbeddings(texts) → embeddings[]          │
│      • Batch size: 100 (Ollama API limit)                     │
│      • Parallel processing                                    │
│                                                                  │
│  [ ] Add background job for embedding:                         │
│      • Hangfire job: ProcessDocumentEmbedding(documentId)      │
│      • Queue-based processing                                 │
│      • Update document status after completion                │
│                                                                  │
│  [ ] Add progress tracking:                                    │
│      • DocumentProcessingJob table                            │
│      • Track: total chunks, processed, failed                 │
│                                                                  │
│  DELIVERABLE: Batch embedding với Redis caching               │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### 3.3 Day 12-14: Vector Indexing

#### Ngày 12: pgvector Setup + Chunk Entity

**Mục tiêu cuối ngày**: Document chunks được lưu với vector embeddings

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 12 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Create DocumentChunk entity:                              │
│      • Id, DocumentId, ChunkIndex                             │
│      • Content, ContentHash                                    │
│      • Embedding (vector(1536))                                │
│      • PageNumber, SectionTitle                                │
│      • TokenCount, Metadata (JSONB)                            │
│      • CreatedAt                                             │
│                                                                  │
│  [ ] Configure pgvector in DbContext:                         │
│      • Enable vector extension                                │
│      • Configure vector type mapping                          │
│                                                                  │
│  [ ] Add vector index:                                        │
│      • HNSW index on embedding column                         │
│      • m=16, ef_construction=64                              │
│                                                                  │
│  [ ] Run migration: dotnet ef migrations add InitChunks        │
│                                                                  │
│  DELIVERABLE: Chunks với embeddings stored in PostgreSQL      │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Ngày 13: Document Processing Pipeline

**Mục tiêu cuối ngày**: Full pipeline từ upload → indexed hoạt động

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 13 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Create IDocumentProcessingService:                        │
│      • ProcessDocument(documentId)                              │
│          1. Extract text                                      │
│          2. Clean text                                        │
│          3. Chunk text                                         │
│          4. Generate embeddings (batch)                        │
│          5. Save chunks to database                           │
│          6. Update document status                            │
│                                                                  │
│  [ ] Integrate with Hangfire:                                 │
│      • Enqueue job on document upload                         │
│      • Retry on failure (3 times)                             │
│      • Log processing time                                    │
│                                                                  │
│  [ ] Add status update endpoint:                               │
│      • GET /api/v1/documents/{id}/status                     │
│      • Returns: pending, processing, indexed, failed           │
│      • Progress: 0%, 25%, 50%, 75%, 100%                     │
│                                                                  │
│  DELIVERABLE: Upload → Extract → Chunk → Embed → Indexed       │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Ngày 14: Integration Testing + Bug Fixes

**Mục tiêu cuối ngày**: Document processing hoạt động end-to-end

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 14 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  TESTING:                                                       │
│  [ ] Integration test: Upload PDF → Check indexed              │
│  [ ] Test text extraction với various PDF formats              │
│  [ ] Test chunking: verify chunk sizes                         │
│  [ ] Test embedding generation: verify dimensions               │
│  [ ] Test Redis cache: verify cache hits                       │
│                                                                  │
│  BUG FIXES:                                                    │
│  [ ] Fix any extraction issues                                 │
│  [ ] Fix any chunking edge cases                              │
│  [ ] Fix any embedding failures                               │
│  [ ] Add proper error handling                                │
│                                                                  │
│  DOCUMENTATION:                                                 │
│  [ ] Document processing flow                                  │
│  [ ] Add processing status to UI                               │
│                                                                  │
│  DELIVERABLE: Full document processing pipeline works           │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

## 4. TUẦN 3: SEARCH + CHAT (Days 15-21)

### 4.1 Day 15-17: Semantic Search

#### Ngày 15: Vector Similarity Search

**Mục tiêu cuối ngày**: Search endpoint trả về kết quả dựa trên similarity

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 15 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Create ISearchService:                                    │
│      • Search(query, topK, filters) → results                   │
│                                                                  │
│  [ ] Implement vector search:                                  │
│      • Generate query embedding (Ollama)                       │
│      • Cosine similarity với pgvector                         │
│      • Return top-K results                                   │
│      • Include relevance scores                               │
│                                                                  │
│  [ ] Create SearchController:                                  │
│      • GET /api/v1/search?q={query}&topK=10                  │
│                                                                  │
│  [ ] Add metadata filtering:                                  │
│      • Filter by category                                     │
│      • Filter by date range                                   │
│      • Filter by uploader                                    │
│                                                                  │
│  DELIVERABLE: Basic semantic search working                    │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Ngày 16: Hybrid Search (Semantic + Keyword)

**Mục tiêu cuối ngày**: Kết hợp semantic và keyword search

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 16 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Add PostgreSQL full-text search:                         │
│      • Create GIN index on content                             │
│      • Use to_tsquery for Vietnamese                           │
│      • BM25 scoring                                           │
│                                                                  │
│  [ ] Implement RRF (Reciprocal Rank Fusion):                  │
│      • Combine semantic (70%) + keyword (30%)                 │
│      • Weighted scoring                                       │
│                                                                  │
│  [ ] Add search logs:                                         │
│      • Log all searches with results                         │
│      • Track latency, result count                           │
│      • User feedback (optional)                              │
│                                                                  │
│  DELIVERABLE: Hybrid search combining vector + keyword        │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Ngày 17: Re-ranking + Search UI

**Mục tiêu cuối ngày**: Search với re-ranking, UI hiển thị kết quả

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 17 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Add re-ranking:                                          │
│      • Cross-encoder reranking (optional for MVP)              │
│      • MMR for diversity (optional for MVP)                    │
│                                                                  │
│  FRONTEND:                                                      │
│  [ ] Create searchStore (Zustand):                            │
│      • searchResults state                                     │
│      • performSearch(query)                                    │
│                                                                  │
│  [ ] Create Search page:                                       │
│      • Search input với debounce                             │
│      • Loading state                                          │
│      • Results list with:                                    │
│          - Document title                                    │
│          - Chunk content (highlighted)                        │
│          - Relevance score                                    │
│          - Source link                                        │
│                                                                  │
│  DELIVERABLE: Search UI với highlighted results               │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### 4.2 Day 18-21: RAG Chatbot

#### Ngày 18: Session Management

**Mục tiêu cuối ngày**: Chat sessions được lưu trong database

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 18 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Create ChatSession entity:                                │
│      • Id, UserId, Title                                      │
│      • MessageCount, LastMessagePreview                        │
│      • CreatedAt, UpdatedAt                                   │
│                                                                  │
│  [ ] Create ChatMessage entity:                               │
│      • Id, SessionId, Role (System/User/Assistant)           │
│      • Content, Citations (JSONB)                             │
│      • TokenCount, ModelUsed, LatencyMs                       │
│      • CreatedAt                                             │
│                                                                  │
│  [ ] Create IChatService:                                     │
│      • CreateSession(userId, title)                           │
│      • GetSessions(userId)                                    │
│      • GetSession(id)                                          │
│      • DeleteSession(id)                                      │
│                                                                  │
│  [ ] Create ChatController:                                    │
│      • GET /api/v1/chat/sessions                              │
│      • POST /api/v1/chat/sessions                            │
│      • GET /api/v1/chat/sessions/{id}                        │
│      • DELETE /api/v1/chat/sessions/{id}                     │
│                                                                  │
│  DELIVERABLE: Chat session CRUD working                       │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Ngày 19: RAG Pipeline Implementation

**Mục tiêu cuối ngày**: RAG pipeline hoàn chỉnh hoạt động

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 19 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Implement IRAGPipeline:                                  │
│      • GenerateResponse(userMessage, sessionId) → response     │
│                                                                  │
│  [ ] RAG Pipeline steps:                                       │
│      1. Get conversation history                               │
│      2. Build context from retrieved chunks                   │
│      3. Generate RAG prompt with:                            │
│          - System prompt (instructions)                        │
│          - Retrieved context                                  │
│          - Conversation history                               │
│          - User message                                        │
│                                                                  │
│  [ ] Ollama chat integration:                                 │
│      • Send prompt to Ollama                                  │
│      • Stream response (optional for MVP)                      │
│      • Parse response + citations                            │
│                                                                  │
│  [ ] Save chat message:                                       │
│      • Store user message                                     │
│      • Store assistant response + citations                   │
│                                                                  │
│  DELIVERABLE: RAG chatbot returns responses                   │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Ngày 20: Citations + System Prompt

**Mục tiêu cuối ngày**: Chatbot trả lời với citations

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 20 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Add citations to response:                               │
│      • Extract source documents from retrieved chunks           │
│      • Include [1], [2], etc. in response                    │
│      • Return sources array: [{docId, title, page, excerpt}]  │
│                                                                  │
│  [ ] Optimize system prompt:                                   │
│      • Clear instructions to cite sources                     │
│      • Fallback behavior when no relevant context            │
│      • Vietnamese language support                            │
│                                                                  │
│  [ ] Add confidence scoring:                                  │
│      • Calculate based on retrieval similarity               │
│      • Include in response metadata                           │
│                                                                  │
│  DELIVERABLE: Chatbot responses include citations              │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Ngày 21: Chat UI + Streaming

**Mục tiêu cuối ngày**: Chat UI hoạt động với streaming responses

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 21 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  FRONTEND:                                                      │
│  [ ] Create chatStore (Zustand):                               │
│      • messages state                                          │
│      • sessions state                                          │
│      • sendMessage(content)                                    │
│      • createSession()                                         │
│                                                                  │
│  [ ] Create Chat page:                                         │
│      • Session list sidebar                                    │
│      • Chat message area                                      │
│      • Message input (textarea)                               │
│      • Send button                                            │
│                                                                  │
│  [ ] Message display:                                          │
│      • User messages (right-aligned)                          │
│      • Assistant messages (left-aligned)                     │
│      • Citations inline                                       │
│      • Sources expandable section                             │
│                                                                  │
│  [ ] Add loading indicator:                                   │
│      • Show "typing..." while waiting                        │
│      • (Optional) Streaming text display                     │
│                                                                  │
│  DELIVERABLE: Full chat UI working with RAG responses         │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

## 5. TUẦN 4: POLISH + ADMIN (Days 22-30)

### 5.1 Day 22-24: Admin Dashboard

#### Ngày 22: Stats API

**Mục tiêu cuối ngày**: API trả về statistics cho dashboard

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 22 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Create IAdminService:                                     │
│      • GetDashboardStats() → stats                           │
│      • GetUserStats() → user statistics                       │
│      • GetDocumentStats() → document statistics              │
│      • GetSearchAnalytics() → search data                     │
│                                                                  │
│  [ ] Stats to include:                                         │
│      • Total users, documents, chats                         │
│      • Documents by status                                    │
│      • Searches per day                                       │
│      • Popular search terms                                   │
│      • Average response time                                  │
│                                                                  │
│  [ ] Create AdminController:                                  │
│      • GET /api/v1/admin/stats                                │
│      • GET /api/v1/admin/analytics                           │
│                                                                  │
│  DELIVERABLE: Admin stats API returns data                    │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Ngày 23: User Management

**Mục tiêu cuối ngày**: Admin có thể quản lý users

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 23 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Add admin endpoints:                                      │
│      • GET /api/v1/admin/users                                │
│      • POST /api/v1/admin/users                               │
│      • PUT /api/v1/admin/users/{id}                           │
│      • DELETE /api/v1/admin/users/{id}                       │
│                                                                  │
│  [ ] Add role management:                                     │
│      • Update user role (Admin, Teacher, Student)            │
│      • Activate/deactivate users                             │
│                                                                  │
│  FRONTEND:                                                      │
│  [ ] Create Admin Users page:                                │
│      • Users table with pagination                            │
│      • Add/Edit user modal                                   │
│      • Role assignment                                       │
│      • Delete confirmation                                   │
│                                                                  │
│  DELIVERABLE: Admin user management UI working                │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Ngày 24: Document Management + Moderation

**Mục tiêu cuối ngày**: Admin có thể quản lý documents

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 24 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Add admin document endpoints:                            │
│      • GET /api/v1/admin/documents                            │
│      • PUT /api/v1/admin/documents/{id}/status               │
│      • DELETE /api/v1/admin/documents/{id}                   │
│                                                                  │
│  [ ] Add moderation features:                                  │
│      • Approve/reject documents                               │
│      • View processing logs                                   │
│      • Reprocess failed documents                            │
│                                                                  │
│  FRONTEND:                                                      │
│  [ ] Create Admin Documents page:                             │
│      • All documents table                                    │
│      • Filter by status                                      │
│      • Bulk actions (approve, delete)                        │
│      • View document details                                 │
│                                                                  │
│  DELIVERABLE: Admin document management UI                     │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### 5.2 Day 25-27: Polish + Testing

#### Ngày 25: UI/UX Polish

**Mục tiêu cuối ngày**: UI nhất quán và professional

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 25 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  FRONTEND:                                                      │
│  [ ] Consistent styling với Tailwind                         │
│  [ ] Loading states for all async operations                   │
│  [ ] Error handling UI:                                       │
│      • Toast notifications                                    │
│      • Error boundaries                                       │
│      • Retry buttons                                         │
│                                                                  │
│  [ ] Responsive design:                                       │
│      • Desktop (> 1024px)                                   │
│      • Tablet (768-1024px)                                   │
│      • Mobile (< 768px)                                     │
│                                                                  │
│  [ ] Empty states:                                            │
│      • No documents uploaded                                 │
│      • No search results                                     │
│      • No chat sessions                                      │
│                                                                  │
│  DELIVERABLE: Professional, consistent UI                     │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Ngày 26: Unit Tests

**Mục tiêu cuối ngày**: Core functionality có unit tests

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 26 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  BACKEND:                                                       │
│  [ ] Setup xUnit test project                                 │
│  [ ] Add test packages:                                       │
│      • Moq                                                   │
│      • FluentAssertions                                       │
│      • Microsoft.AspNetCore.Mvc.Testing                      │
│                                                                  │
│  [ ] Write tests for:                                         │
│      • AuthService (login, register, JWT)                    │
│      • DocumentService (upload, CRUD)                        │
│      • SearchService (similarity, filtering)                  │
│      • ChunkingService (size, overlap)                       │
│      • RAGPipeline (context building)                        │
│                                                                  │
│  TARGET: 70% coverage on core services                       │
│                                                                  │
│  DELIVERABLE: Unit tests passing                             │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Ngày 27: Integration Tests + Bug Fixes

**Mục tiêu cuối ngày**: Integration tests pass, bugs fixed

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 27 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  TESTING:                                                       │
│  [ ] Write integration tests:                                  │
│      • Full document upload → search workflow                  │
│      • Chat → RAG response workflow                           │
│      • Auth flow                                              │
│                                                                  │
│  [ ] Run all tests, fix failures                              │
│  [ ] Performance testing:                                       │
│      • Search latency < 1s                                   │
│      • Chat response < 5s                                    │
│      • Document processing < 5 min                           │
│                                                                  │
│  BUG FIXES:                                                    │
│  [ ] Address any remaining bugs                               │
│  [ ] Edge cases handling                                      │
│  [ ] Error messages improvements                              │
│                                                                  │
│  DELIVERABLE: All tests passing, performance acceptable       │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

### 5.3 Day 28-30: Documentation + Deploy

#### Ngày 28: Documentation

**Mục tiêu cuối ngày**: Documentation hoàn chỉnh

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 28 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  [ ] Update README files:                                      │
│      • Project overview                                       │
│      • Setup instructions                                     │
│      • Architecture diagram                                   │
│      • API documentation                                      │
│      • Deployment guide                                       │
│                                                                  │
│  [ ] Create API documentation:                                │
│      • Swagger/OpenAPI spec                                  │
│      • Request/response examples                             │
│                                                                  │
│  [ ] Document troubleshooting:                                │
│      • Common issues                                         │
│      • Solutions                                             │
│                                                                  │
│  DELIVERABLE: Complete documentation                          │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Ngày 29: Deployment Scripts

**Mục tiêu cuối ngày**: Deployment scripts hoạt động

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 29 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  INFRASTRUCTURE:                                               │
│  [ ] Create setup.sh:                                         │
│      • Initialize Docker volumes                              │
│      • Run database migrations                               │
│      • Seed initial data                                     │
│      • Download Ollama models                                │
│                                                                  │
│  [ ] Create start.sh:                                         │
│      • Start all services                                    │
│      • Wait for health checks                                │
│      • Display access URLs                                   │
│                                                                  │
│  [ ] Create stop.sh:                                         │
│      • Graceful shutdown                                     │
│      • Backup option                                        │
│                                                                  │
│  [ ] Create health-check.sh:                                  │
│      • Check all services                                   │
│      • Report status                                        │
│                                                                  │
│  DELIVERABLE: One-command deployment                         │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

#### Ngày 30: Final Testing + MVP Complete

**Mục tiêu cuối ngày**: MVP hoàn chỉnh và deploy được

**Tasks**:

```
┌─────────────────────────────────────────────────────────────────┐
│  DAY 30 TASKS                                                   │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  FINAL TESTING:                                                 │
│  [ ] End-to-end testing all features                         │
│  [ ] Test on clean environment                               │
│  [ ] Verify all services healthy                             │
│                                                                  │
│  DEMO PREPARATION:                                             │
│  [ ] Prepare demo scenarios:                                 │
│      1. Upload PDF → process → indexed                        │
│      2. Semantic search                                       │
│      3. RAG chatbot with citations                          │
│      4. Admin dashboard                                       │
│                                                                  │
│  [ ] Create demo script                                       │
│  [ ] Prepare backup solutions                                 │
│                                                                  │
│  MVP DELIVERABLE CHECKLIST:                                   │
│  [x] Authentication (Login, Register, JWT)                  │
│  [x] Document Upload (MinIO, CRUD)                          │
│  [x] Text Extraction (PDF, DOCX, TXT)                       │
│  [x] Chunking + Embedding (Ollama)                         │
│  [x] Vector Indexing (pgvector, HNSW)                       │
│  [x] Semantic Search (hybrid)                               │
│  [x] RAG Chatbot (citations)                               │
│  [x] Admin Dashboard (stats, management)                    │
│  [x] Unit Tests (70%+ coverage)                            │
│  [x] Documentation (README, API docs)                        │
│                                                                  │
│  🎉 MVP COMPLETE! 🎉                                          │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

## 6. MILESTONES

| Milestone | Ngày | Deliverables | Status |
|-----------|------|--------------|--------|
| **M1** | Day 2 | Infrastructure ready, Docker working | ⏳ |
| **M2** | Day 4 | Authentication complete | ⏳ |
| **M3** | Day 7 | Basic upload working | ⏳ |
| **M4** | Day 9 | Text extraction working | ⏳ |
| **M5** | Day 14 | Embedding & indexing complete | ⏳ |
| **M6** | Day 17 | Search working | ⏳ |
| **M7** | Day 21 | Chatbot working with RAG | ⏳ |
| **M8** | Day 24 | Admin dashboard complete | ⏳ |
| **M9** | Day 28 | Unit tests passing | ⏳ |
| **M10** | Day 30 | MVP complete, deployable | ⏳ |

---

## 7. DEFINITION OF DONE

Mỗi feature được coi là **DONE** khi:

```
┌─────────────────────────────────────────────────────────────────┐
│                    DEFINITION OF DONE                             │
├─────────────────────────────────────────────────────────────────┤
│                                                                  │
│  CODE:                                                          │
│  ✓ Code follows project conventions                            │
│  ✓ No TODO comments left                                        │
│  ✓ Error handling implemented                                   │
│                                                                  │
│  TESTS:                                                         │
│  ✓ Unit tests written and passing                              │
│  ✓ Manual testing completed                                    │
│                                                                  │
│  UI:                                                            │
│  ✓ Responsive on all breakpoints                              │
│  ✓ Loading states implemented                                 │
│  ✓ Error messages user-friendly                               │
│                                                                  │
│  DOCUMENTATION:                                                │
│  ✓ API endpoints documented                                   │
│  ✓ README updated (if needed)                                 │
│                                                                  │
│  DEPLOYMENT:                                                   │
│  ✓ Works on clean environment                                 │
│  ✓ No console errors in browser                               │
│                                                                  │
└─────────────────────────────────────────────────────────────────┘
```

---

**Mục đích**: Framework base tích hợp AI cho mọi hệ thống
**Framework**: AI Integration Base (AIBaseFramework)
**Ngày tạo**: 2026-05-03
