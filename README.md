# Hệ thống Tìm kiếm và Hỏi đáp Tài liệu Nội bộ sử dụng Semantic Search và RAG

**Đề tài luận văn thạc sĩ**

**Phiên bản:** 1.0  
**Ngày cập nhật:** 2026-05-03  
**Trạng thái:** Đang phát triển

---

## Mục lục

1. [Tổng quan dự án](#1-tổng-quan-dự-án)
2. [Kiến trúc hệ thống](#2-kiến-trúc-hệ-thống)
3. [Cấu trúc tài liệu](#3-cấu-trúc-tài-liệu)
4. [Bắt đầu nhanh](#4-bắt-đầu-nhanh)
5. [Tổng quan các module](#5-tổng-quan-các-module)
6. [Công nghệ chính](#6-công-nghệ-chính)
7. [Quy trình phát triển](#7-quy-trình-phát-triển)
8. [Triển khai](#8-triển-khai)
9. [Giám sát và quan sát](#9-giám-sát-và-quan-sát)
10. [Lộ trình phát triển](#10-lộ-trình-phát-triển)
11. [Tài liệu tham khảo](#11-tài-liệu-tham-khảo)
12. [Liên hệ](#12-liên-hệ)

---

## 1. Tổng quan dự án

### 1.1. Tên dự án

**Hệ thống Tìm kiếm và Hỏi đáp Tài liệu Nội bộ sử dụng Semantic Search và RAG**

*(Internal Document Search and Q&A System using Semantic Search and RAG)*

### 1.2. Mô tả ngắn

Hệ thống AI thông minh cho phép người dùng tìm kiếm và hỏi đáp về tài liệu nội bộ bằng ngôn ngữ tự nhiên, sử dụng công nghệ Semantic Search và RAG (Retrieval-Augmented Generation) để đảm bảo câu trả lời có trích dẫn nguồn chính xác.

### 1.3. Đặc điểm nổi bật - 100% Offline Local Docker

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                    🎯 KIẾN TRÚC ĐẶC BIỆT: 100% OFFLINE                         │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ✅ Tất cả services chạy trên Docker (máy local của bạn)                │
│  ✅ Sử dụng Ollama để chạy LLM local (KHÔNG cần OpenAI API)              │
│  ✅ Embedding model chạy local (nomic-embed-text)                         │
│  ✅ Không gửi dữ liệu ra Internet                                         │
│  ✅ Không tốn chi phí API                                                │
│  ✅ Bảo mật tuyệt đối - dữ liệu không rời máy bạn                       │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### 1.3. Key Features

| Tính năng | Mô tả |
|------------|--------|
| **Chatbot hỏi đáp RAG** | Trả lời câu hỏi bằng ngôn ngữ tự nhiên với trích dẫn nguồn |
| **Semantic Search** | Tìm kiếm theo ý nghĩa, không cần từ khóa chính xác |
| **Hybrid Search** | Kết hợp semantic + keyword search để tối ưu kết quả |
| **Quản lý tài liệu** | Upload, phân loại, và index tài liệu tự động |
| **Dashboard Admin** | Quản lý người dùng, tài liệu, và theo dõi usage |
| **Đa ngôn ngữ** | Hỗ trợ tiếng Việt và tiếng Anh |
| **Phân quyền RBAC** | Kiểm soát truy cập theo vai trò và phòng ban |

### 1.4. Tech Stack tổng quan

```
┌─────────────────────────────────────────────────────────────────┐
│                         FRONTEND                                 │
│              Next.js 14 + TypeScript + TailwindCSS               │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                          API GATEWAY                             │
│                    FastAPI + Python 3.11+                       │
├─────────────────────────────────────────────────────────────────┤
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐  │
│  │  Document    │  │    Search    │  │       Chat          │  │
│  │  Service     │  │   Service    │  │     Service         │  │
│  └──────────────┘  └──────────────┘  └──────────────────────┘  │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────────────┐  │
│  │    Auth      │  │  Embedding   │  │       LLM           │  │
│  │   Service    │  │   Service    │  │    Service          │  │
│  │              │  │  (Ollama)   │  │   (Ollama)        │  │
│  └──────────────┘  └──────────────┘  └──────────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        ▼                     ▼                     ▼
┌──────────────┐    ┌──────────────┐    ┌──────────────────────────┐
│  PostgreSQL  │    │     Redis    │    │        MinIO             │
│  + pgvector  │    │   (Cache)    │    │   (Object Storage)      │
└──────────────┘    └──────────────┘    └──────────────────────────┘
```

```
┌────────────────────────────────────────────────────────────────┐
│              OLLAMA CONTAINER (Local LLM)                        │
│  • Chat Models: llama3.2, mistral, phi3, qwen2.5            │
│  • Embedding: nomic-embed-text, mxbai-embed-large          │
│  • GPU Support: NVIDIA CUDA (hoặc CPU only)                  │
└────────────────────────────────────────────────────────────────┘
```
| Layer | Công nghệ | Ghi chú |
|-------|-----------|---------|
| **Frontend** | Next.js 14, React, TailwindCSS | App Router, Server Components |
| **API Gateway** | FastAPI, Python 3.11 | Async, high performance |
| **Vector DB** | PostgreSQL 16 + pgvector | Native vector storage |
| **Cache** | Redis 7 | Session, rate limiting |
| **Object Storage** | MinIO (S3-compatible) | Document files |
| **LLM** | Ollama (llama3.2, mistral, phi3) | **Local, offline, không cần API** |
| **Embedding** | Ollama (nomic-embed-text) | **Local, offline, không cần API** |

---

## 2. Kiến trúc hệ thống

### 2.1. Sơ đồ kiến trúc tổng thể

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                              SYSTEM ARCHITECTURE                                      │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│    ┌─────────────────────────────────────────────────────────────────────────┐     │
│    │                           CLIENT LAYER                                    │     │
│    │  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐        │     │
│    │  │   Web Client    │  │   Mobile App    │  │   Admin Portal  │        │     │
│    │  │   (Next.js)     │  │   (PWA)        │  │   (Dashboard)   │        │     │
│    │  └─────────────────┘  └─────────────────┘  └─────────────────┘        │     │
│    └─────────────────────────────────────────────────────────────────────────┘     │
│                                          │                                          │
│                                          ▼                                          │
│    ┌─────────────────────────────────────────────────────────────────────────┐     │
│    │                         API GATEWAY LAYER                                │     │
│    │                                                                               │     │
│    │  ┌─────────────────────────────────────────────────────────────────────┐ │     │
│    │  │                         FastAPI Application                          │ │     │
│    │  │  ┌─────────┐  ┌─────────┐  ┌─────────┐  ┌─────────┐  ┌─────────┐   │ │     │
│    │  │  │  Auth   │  │Document │  │ Search  │  │  Chat   │  │  Admin  │   │ │     │
│    │  │  │Router   │  │ Router  │  │ Router  │  │ Router  │  │ Router  │   │ │     │
│    │  │  └────┬────┘  └────┬────┘  └────┬────┘  └────┬────┘  └────┬────┘   │ │     │
│    │  └───────┼────────────┼────────────┼────────────┼────────────┼────────┘ │     │
│    │          │            │            │            │            │          │     │
│    │          ▼            ▼            ▼            ▼            ▼          │     │
│    │  ┌───────────┐  ┌───────────┐  ┌───────────┐  ┌───────────┐          │     │
│    │  │  Auth     │  │ Document  │  │  Search   │  │   Chat    │          │     │
│    │  │  Service  │  │  Service  │  │  Service  │  │  Service  │          │     │
│    │  └─────┬─────┘  └─────┬─────┘  └─────┬─────┘  └─────┬─────┘          │     │
│    │        │              │              │              │                  │     │
│    └────────┼──────────────┼──────────────┼──────────────┼────────────────┘     │
│             │              │              │              │                        │
│             ▼              ▼              ▼              ▼                        │
│    ┌─────────────────────────────────────────────────────────────────────────┐     │
│    │                       SERVICE LAYER                                       │     │
│    │                                                                               │     │
│    │  ┌───────────────┐  ┌───────────────┐  ┌───────────────────────┐       │     │
│    │  │  Embedding    │  │      LLM      │  │    Document          │       │     │
│    │  │  Service      │  │   Service     │  │    Processor         │       │     │
│    │  │  (OpenAI)    │  │ (GPT-4/Claude)│  │  (Parser, Chunker)   │       │     │
│    │  └───────────────┘  └───────────────┘  └───────────────────────┘       │     │
│    │                                                                               │     │
│    └─────────────────────────────────────────────────────────────────────────┘     │
│                                          │                                          │
│    ┌─────────────────────────────────────┼─────────────────────────────────────┐     │
│    │                              DATA LAYER                                    │     │
│    │                                                                               │     │
│    │  ┌──────────────────┐  ┌──────────────────┐  ┌────────────────────────┐  │     │
│    │  │  PostgreSQL 16   │  │       Redis      │  │        MinIO           │  │     │
│    │  │                  │  │                  │  │                        │  │     │
│    │  │  • users         │  │  • Sessions     │  │  • Document files     │  │     │
│    │  │  • documents     │  │  • Cache       │  │  • PDF, DOCX, XLSX    │  │     │
│    │  │  • document_     │  │  • Rate Limit  │  │  • Images             │  │     │
│    │  │    chunks        │  │                │  │  • Thumbnails         │  │     │
│    │  │  • chat_sessions │  │                │  │                        │  │     │
│    │  │  • chat_messages │  │                │  │                        │  │     │
│    │  │  • search_logs   │  │                │  │                        │  │     │
│    │  │                  │  │                │  │                        │  │     │
│    │  │  + pgvector     │  │                │  │                        │  │     │
│    │  │    (embeddings) │  │                │  │                        │  │     │
│    │  └──────────────────┘  └──────────────────┘  └────────────────────────┘  │     │
│    │                                                                               │     │
│    └─────────────────────────────────────────────────────────────────────────────┘     │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

### 2.2. Các layer chính

| Layer | Mô tả | Các thành phần |
|-------|-------|----------------|
| **Client Layer** | Giao diện người dùng | Web (Next.js), PWA, Admin Dashboard |
| **API Gateway** | Điều hướng request | FastAPI, Authentication, Rate Limiting |
| **Service Layer** | Business logic | Auth, Document, Search, Chat, Embedding, LLM |
| **Data Layer** | Lưu trữ dữ liệu | PostgreSQL, Redis, MinIO |

### 2.3. Data Flow cho RAG

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                              RAG DATA FLOW                                          │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│  ┌─────────┐     ┌─────────┐     ┌─────────┐     ┌─────────┐     ┌─────────┐     │
│  │ Upload  │────>│ Extract │────>│ Chunk   │────>│ Embed   │────>│ Store   │     │
│  │ Document│     │  Text   │     │ & Split │     │ & Index │     │ in DB   │     │
│  └─────────┘     └─────────┘     └─────────┘     └─────────┘     └─────────┘     │
│       │                                                  │                         │
│       │                                                  ▼                         │
│       │                                         ┌─────────────────┐                │
│       │                                         │  Vector Store   │                │
│       │                                         │  (pgvector)     │                │
│       │                                         └─────────────────┘                │
│       │                                                  │                         │
│       │                                                  ▼                         │
│       │  ┌────────────────────────────────────────────────────────────────────┐   │
│       │  │                     QUERY FLOW                                      │   │
│       │  │                                                                     │   │
│       │  │  ┌─────────┐     ┌─────────┐     ┌─────────┐     ┌─────────┐     │   │
│       │  │  │ User    │────>│ Embed   │────>│ Vector  │────>│ Rerank  │     │   │
│       │  │  │ Query   │     │ Query   │     │ Search  │     │ Results │     │   │
│       │  │  └─────────┘     └─────────┘     └─────────┘     └────┬────┘     │   │
│       │  │                                                        │          │   │
│       │  │                                                        ▼          │   │
│       │  │  ┌─────────┐     ┌─────────┐     ┌─────────┐     ┌─────────┐     │   │
│       │  │  │  LLM    │<────│ Augment │<────│  Build │<────│ Context │     │   │
│       │  │  │Response │     │ Prompt  │     │ Context │     │ Retrieval│    │   │
│       │  │  └────┬────┘     └─────────┘     └─────────┘     └─────────┘     │   │
│       │  │       │                                                               │   │
│       │  └───────┼───────────────────────────────────────────────────────────┘   │
│       │          ▼                                                                   │
│       │  ┌────────────────────────────────────────────────────────────────────┐   │
│       │  │                     USER RESPONSE                                   │   │
│       │  │         Answer + Source Citations + References                      │   │
│       │  └────────────────────────────────────────────────────────────────────┘   │
│       │                                                                               │
│       └───────────────────────────────────────────────────────────────────────────┘   │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 3. Cấu trúc tài liệu

### 3.1. Tree structure

```
DoAnThacSI/
├── README.md                        # 📋 File tổng hợp chính (this file)
│
├── docs/
│   ├── 01.PROBLEM_ANALYSIS.md       # 📌 Phân tích bài toán và xác định phạm vi
│   │
│   ├── architecture/
│   │   └── 02.SYSTEM_ARCHITECTURE.md # 🏗️ Thiết kế kiến trúc hệ thống chi tiết
│   │
│   ├── modules/
│   │   └── 03.MODULES.md            # 📦 Tài liệu 9 modules và dependencies
│   │
│   ├── database/
│   │   └── 04.DATABASE_DESIGN.md    # 🗄️ Thiết kế database (PostgreSQL + pgvector)
│   │
│   ├── api/
│   │   └── 05.API_DESIGN.md         # 🔌 Thiết kế REST API endpoints
│   │
│   ├── flows/
│   │   ├── 06a.DOCUMENT_PIPELINE_FLOW.md  # 📄 Document processing pipeline
│   │   └── 06b.RAG_CHATBOT_FLOW.md         # 💬 RAG chatbot flow
│   │
│   ├── roadmap/
│   │   └── 07.ROADMAP_AND_TASKS.md  # 🗺️ Lộ trình phát triển 5 phases
│   │
│   ├── risks/
│   │   └── 08.TECHNICAL_RISKS.md    # ⚠️ Phân tích rủi ro kỹ thuật
│   │
│   ├── mvp/
│   │   └── 09.MVP_30_DAYS.md        # 🚀 MVP implementation plan (30 ngày)
│   │
│   └── standards/
│       └── 10.MASTERS_THESIS_STANDARDS.md  # 📝 Tiêu chuẩn luận văn thạc sĩ
│
├── src/
│   ├── backend/                     # FastAPI backend
│   │   ├── services/
│   │   ├── routers/
│   │   ├── models/
│   │   └── core/
│   │
│   └── frontend/                    # Next.js frontend
│       ├── app/
│       ├── components/
│       └── lib/
│
├── docker/
│   ├── docker-compose.yml           # Docker Compose for local dev
│   └── Dockerfile                   # Dockerfile for production
│
├── tests/
│   ├── unit/
│   ├── integration/
│   └── e2e/
│
└── scripts/
    ├── setup.sh                     # Setup script
    ├── migrate.sh                   # Database migration
    └── seed.sh                      # Seed data
```

### 3.2. Mục đích từng file

| File | Mô tả | Đối tượng |
|------|-------|-----------|
| `01.PROBLEM_ANALYSIS.md` | Phân tích bài toán, bối cảnh, mục tiêu, yêu cầu | Supervisor, Committee |
| `02.SYSTEM_ARCHITECTURE.md` | Kiến trúc chi tiết, sơ đồ, công nghệ | Developer, Architect |
| `03.MODULES.md` | 9 modules với specifications và dependencies | Developer |
| `04.DATABASE_DESIGN.md` | Schema, ERD, pgvector strategy, indexing | DBA, Developer |
| `05.API_DESIGN.md` | REST API endpoints, request/response models | Developer |
| `06a.DOCUMENT_PIPELINE_FLOW.md` | Document processing pipeline | Developer |
| `06b.RAG_CHATBOT_FLOW.md` | RAG chatbot implementation | Developer |
| `07.ROADMAP_AND_TASKS.md` | Lộ trình 5 phases, milestones | PM, Supervisor |
| `08.TECHNICAL_RISKS.md` | Risk assessment, mitigation | Architect, PM |
| `09.MVP_30_DAYS.md` | MVP implementation plan 30 ngày | Developer |
| `10.MASTERS_THESIS_STANDARDS.md` | Tiêu chuẩn trình bày luận văn | Author |

---

## 4. Bắt đầu nhanh

### 4.1. Yêu cầu hệ thống

| Requirement | Phiên bản tối thiểu | Ghi chú |
|-------------|---------------------|---------|
| **Docker** | 20.10+ | Container runtime |
| **Docker Compose** | 2.0+ | Multi-container orchestration |
| **Python** | 3.11+ | Backend development |
| **Node.js** | 18+ | Frontend development |
| **RAM** | 8GB+ | Khuyến nghị 16GB |
| **Disk** | 20GB+ | Cho database và documents |
| **CPU** | 4 cores+ | Khuyến nghị 8 cores |

### 4.2. Docker Compose Setup

```bash
# Clone repository
git clone <repository-url>
cd DoAnThacSI

# Copy environment file
cp .env.example .env

# Start all services
docker-compose up -d

# Xem logs
docker-compose logs -f

# Stop services
docker-compose down
```

### 4.3. Running Instructions

#### Development Mode

```bash
# Backend
cd src/backend
python -m uvicorn app.main:app --reload --port 8000

# Frontend
cd src/frontend
npm run dev

# Hoặc sử dụng Docker
docker-compose --profile dev up
```

#### Production Mode

```bash
# Build và start production
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d --build

# Kiểm tra health
curl http://localhost:8000/health
```

### 4.4. Environment Variables

```bash
# Database
DATABASE_URL=postgresql://user:password@localhost:5432/document_rag

# Redis
REDIS_URL=redis://localhost:6379

# MinIO
MINIO_ENDPOINT=localhost:9000
MINIO_ACCESS_KEY=minioadmin
MINIO_SECRET_KEY=minioadmin

# OpenAI API (cho embeddings và LLM)
OPENAI_API_KEY=sk-...

# JWT Secret
JWT_SECRET_KEY=your-secret-key-here
```

---

## 5. Tổng quan các module

### 5.1. Danh sách 9 modules

| ID | Module | Mô tả | Priority |
|----|--------|--------|----------|
| **M01** | Authentication Module | JWT auth, OAuth2, RBAC | P0 - Core |
| **M02** | Document Management Module | Upload, storage, metadata | P0 - Core |
| **M03** | Document Processing Pipeline | Parse, chunk, OCR | P0 - Core |
| **M04** | Embedding & Vectorization | Text embedding, indexing | P0 - Core |
| **M05** | Semantic Search Module | Vector search, hybrid search | P0 - Core |
| **M06** | RAG Chatbot Module | LLM generation, citations | P0 - Core |
| **M07** | Chat History Module | Session management | P1 - Important |
| **M08** | Admin Dashboard Module | User, document, analytics | P1 - Important |
| **M09** | Search Analytics Module | Logging, metrics, feedback | P2 - Nice to have |

### 5.2. Module Dependencies

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                           MODULE DEPENDENCIES                                        │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│                          ┌─────────────────┐                                        │
│                          │    M01 Auth     │                                        │
│                          │  (Foundation)   │                                        │
│                          └────────┬────────┘                                        │
│                                   │                                                  │
│           ┌───────────────────────┼───────────────────────┐                          │
│           │                       │                       │                          │
│           ▼                       ▼                       ▼                          │
│  ┌─────────────────┐   ┌─────────────────┐   ┌─────────────────┐                    │
│  │   M02 Document   │   │   M07 Chat     │   │   M08 Admin    │                    │
│  │   Management     │   │   History      │   │   Dashboard    │                    │
│  └────────┬────────┘   └────────┬────────┘   └─────────────────┘                    │
│           │                       │                                                  │
│           ▼                       │                                                  │
│  ┌─────────────────┐              │                                                  │
│  │   M03 Document  │              │                                                  │
│  │   Processing    │              │                                                  │
│  └────────┬────────┘              │                                                  │
│           │                       │                                                  │
│           ▼                       ▼                                                  │
│  ┌─────────────────┐   ┌─────────────────┐                                         │
│  │   M04 Embedding  │◄──│   M06 RAG      │                                         │
│  │   & Vectorize    │   │   Chatbot      │                                         │
│  └────────┬────────┘   └────────┬────────┘                                         │
│           │                     │                                                    │
│           │                     │                                                    │
│           ▼                     ▼                                                    │
│  ┌─────────────────────────────────────────┐                                        │
│  │           M05 Semantic Search           │                                        │
│  └────────────────────┬────────────────────┘                                        │
│                        │                                                              │
│                        ▼                                                              │
│  ┌─────────────────────────────────────────┐                                        │
│  │         M09 Search Analytics            │                                        │
│  └─────────────────────────────────────────┘                                        │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

### 5.3. Chi tiết từng module

#### M01 - Authentication Module
- **Chức năng**: JWT authentication, OAuth2 login, Role-based access control
- **Entities**: Users, Roles, Permissions, Refresh Tokens
- **Endpoints**: `/api/auth/*`
- **Dependencies**: None (foundation module)

#### M02 - Document Management Module
- **Chức năng**: Upload, download, metadata management, versioning
- **Entities**: Documents, Document Categories
- **Endpoints**: `/api/documents/*`
- **Dependencies**: M01 (Auth)

#### M03 - Document Processing Pipeline
- **Chức năng**: Extract text, OCR, chunking, preprocessing
- **Supported formats**: PDF, DOCX, XLSX, PPTX, TXT, MD, images
- **Endpoints**: Internal service
- **Dependencies**: M02 (Documents)

#### M04 - Embedding & Vectorization
- **Chức năng**: Generate embeddings, store in pgvector, manage index
- **Models**: text-embedding-3-small (1536 dimensions)
- **Endpoints**: Internal service
- **Dependencies**: M03 (Processing)

#### M05 - Semantic Search Module
- **Chức năng**: Vector search, hybrid search, reranking, filtering
- **Endpoints**: `/api/search/*`
- **Dependencies**: M04 (Embeddings)

#### M06 - RAG Chatbot Module
- **Chức năng**: LLM generation, context assembly, citation generation
- **LLMs**: GPT-4o-mini, Claude 3.5 Haiku
- **Endpoints**: `/api/chat/*`
- **Dependencies**: M05 (Search)

#### M07 - Chat History Module
- **Chức năng**: Session management, message history, archiving
- **Entities**: Chat Sessions, Chat Messages
- **Endpoints**: `/api/sessions/*`
- **Dependencies**: M01 (Auth), M06 (Chatbot)

#### M08 - Admin Dashboard Module
- **Chức năng**: User management, document management, analytics dashboard
- **Endpoints**: `/api/admin/*`
- **Dependencies**: M01 (Auth), M02 (Documents)

#### M09 - Search Analytics Module
- **Chức năng**: Search logging, user feedback, metrics collection
- **Entities**: Search Logs
- **Endpoints**: `/api/analytics/*`
- **Dependencies**: M05 (Search)

---

## 6. Công nghệ chính

### 6.1. Bảng tổng hợp công nghệ

| Category | Technology | Version | Purpose |
|----------|-----------|---------|---------|
| **Frontend Framework** | Next.js | 14+ | App Router, SSR, SSG |
| **UI Library** | React | 18+ | Component library |
| **Styling** | TailwindCSS | 3.4+ | Utility-first CSS |
| **State Management** | Zustand | 4+ | Client state |
| **API Client** | Axios | 1.6+ | HTTP requests |
| **Backend Framework** | FastAPI | 0.104+ | Async REST API |
| **ORM** | SQLAlchemy | 2.0+ | Database ORM |
| **Database** | PostgreSQL | 16+ | Primary database |
| **Vector Extension** | pgvector | 0.5+ | Vector storage |
| **Cache** | Redis | 7+ | Session, cache |
| **Object Storage** | MinIO | latest | Document storage |
| **LLM** | GPT-4o-mini | - | Chat generation |
| **Embeddings** | text-embedding-3-small | - | Vectorization |
| **Container** | Docker | 24+ | Containerization |
| **Orchestration** | Docker Compose | 2+ | Local dev |
| **Monitoring** | Prometheus | 2+ | Metrics |
| **Logging** | Serilog | 3+ | Structured logging |

### 6.2. Links đến tài liệu công nghệ

| Technology | Documentation Link |
|------------|-------------------|
| Next.js | https://nextjs.org/docs |
| FastAPI | https://fastapi.tiangolo.com |
| PostgreSQL | https://www.postgresql.org/docs |
| pgvector | https://github.com/pgvector/pgvector |
| Redis | https://redis.io/docs |
| MinIO | https://min.io/docs |
| Docker | https://docs.docker.com |
| GPT-4 | https://platform.openai.com/docs/models/gpt-4 |
| TailwindCSS | https://tailwindcss.com/docs |

---

## 7. Quy trình phát triển

### 7.1. Git Workflow

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                              GIT WORKFLOW                                             │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│   main (production) ──────────────────────────────────────────────────────────►     │
│        ▲                                                                      │     │
│        │                                                                      │     │
│   develop (integration) ────────────────────────────────────────────────►    │     │
│        ▲                                                                      │     │
│        │        ┌──────────────────────────────────────────────────────┐     │     │
│        │        │                                                       │     │     │
│        │        ▼                                                       │     │     │
│        │   feature/M01-auth ──────► feature/M02-documents ───► ...    │     │     │
│        │   (module 1)              (module 2)                                    │     │
│        │                                                                     │     │
│        └─────────────────────────────────────────────────────────────────┘     │     │
│                                                                                     │
│   Branch naming convention:                                                          │
│   - feature/M##-<description>  (vd: feature/M01-auth-jwt)                          │
│   - bugfix/<description>                                                            │
│   - hotfix/<description>                                                           │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

### 7.2. Coding Standards

#### Python (Backend)
- Sử dụng type hints cho tất cả functions
- Docstrings theo Google style
- Maximum line length: 120 characters
- Import theo PEP 8

```python
def get_document_by_id(document_id: UUID) -> Document:
    """
    Lấy document theo ID.
    
    Args:
        document_id: UUID của document
        
    Returns:
        Document object hoặc None nếu không tìm thấy
        
    Raises:
        DocumentNotFoundError: Khi document không tồn tại
    """
    document = db.query(Document).filter(Document.id == document_id).first()
    if not document:
        raise DocumentNotFoundError(f"Document {document_id} not found")
    return document
```

#### TypeScript (Frontend)
- Strict mode enabled
- Interface cho props và state
- Component naming: PascalCase
- File naming: kebab-case

```typescript
interface DocumentCardProps {
  document: Document;
  onDelete?: (id: string) => void;
  onDownload?: (id: string) => void;
}

export const DocumentCard: React.FC<DocumentCardProps> = ({ 
  document, 
  onDelete, 
  onDownload 
}) => {
  // Component implementation
};
```

### 7.3. Pull Request Process

1. **Tạo feature branch** từ `develop`
2. **Implement** tính năng với unit tests
3. **Commit** với conventional commit messages
4. **Push** và tạo Pull Request
5. **Review** bởi ít nhất 1 reviewer
6. **Merge** vào `develop` sau khi approved
7. **Delete** feature branch sau khi merge

---

## 8. Triển khai

### 8.1. Docker Compose (Local Development)

```yaml
# docker-compose.yml
version: '3.8'

services:
  api:
    build: ./src/backend
    ports:
      - "8000:8000"
    environment:
      - DATABASE_URL=postgresql://postgres:postgres@db:5432/document_rag
      - REDIS_URL=redis://redis:6379
    depends_on:
      - db
      - redis
    volumes:
      - ./src/backend:/app

  frontend:
    build: ./src/frontend
    ports:
      - "3000:3000"
    environment:
      - NEXT_PUBLIC_API_URL=http://localhost:8000
    depends_on:
      - api

  db:
    image: postgres:16
    environment:
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=postgres
      - POSTGRES_DB=document_rag
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - ./docker/init.sql:/docker-entrypoint-initdb.d/init.sql
    ports:
      - "5432:5432"

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"

  minio:
    image: minio/minio
    ports:
      - "9000:9000"
      - "9001:9001"
    environment:
      - MINIO_ROOT_USER=minioadmin
      - MINIO_ROOT_PASSWORD=minioadmin
    volumes:
      - minio_data:/data

volumes:
  postgres_data:
  minio_data:
```

### 8.2. Kubernetes-Ready Structure

```
kubernetes/
├── base/
│   ├── deployment.yaml           # Base deployment template
│   ├── service.yaml             # Service definitions
│   ├── configmap.yaml            # ConfigMaps
│   └── secrets.yaml             # Secrets (external)
│
├── overlays/
│   ├── development/
│   │   ├── kustomization.yaml
│   │   └── replica-patch.yaml
│   ├── staging/
│   │   ├── kustomization.yaml
│   │   └── resources.yaml
│   └── production/
│       ├── kustomization.yaml
│       └── resources.yaml
│
├── ingress/
│   └── ingress.yaml             # Ingress controller
│
└── helm/
    └── chart/                   # Helm chart for deployment
```

---

## 9. Giám sát và quan sát

### 9.1. Logging (Serilog)

```python
# Structured logging với Serilog
import serilog
from serilog import Logger

logger = Logger()
logger.info(
    "Document indexed successfully",
    document_id=str(document.id),
    user_id=str(user.id),
    chunks_count=len(chunks),
    processing_time_ms=processing_time
)
```

**Log Levels:**
- `DEBUG`: Detailed information for debugging
- `INFO`: General operational information
- `WARNING`: Potential issues
- `ERROR`: Errors that need attention
- `CRITICAL`: System failures

### 9.2. Metrics (Prometheus)

```python
# Prometheus metrics
from prometheus_client import Counter, Histogram, Gauge

# Request metrics
request_duration = Histogram(
    'http_request_duration_seconds',
    'HTTP request duration',
    ['method', 'endpoint', 'status']
)

# Business metrics
documents_indexed = Counter(
    'documents_indexed_total',
    'Total documents indexed',
    ['status']
)

# System metrics
active_connections = Gauge(
    'active_database_connections',
    'Active database connections'
)
```

### 9.3. Health Checks

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                              HEALTH CHECK ENDPOINTS                                │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│  GET /health                      → Liveness probe (basic check)                   │
│  GET /health/ready                → Readiness probe (all dependencies)              │
│  GET /health/live                 → Liveness probe (process alive)                 │
│                                                                                     │
│  Components checked:                                                                  │
│  ├── Database (PostgreSQL + pgvector)                                             │
│  ├── Redis (Cache + Session)                                                       │
│  ├── MinIO (Object Storage)                                                       │
│  └── External APIs (OpenAI)                                                       │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 10. Lộ trình phát triển

### 10.1. Tổng quan 5 Phases

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                              PROJECT TIMELINE                                         │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│  Phase 1: Foundation (Month 1-2)                                                    │
│  ├── Architecture design                                                            │
│  ├── Database setup                                                                 │
│  ├── Core infrastructure                                                            │
│  └── Authentication module (M01)                                                    │
│                                                                                     │
│  Phase 2: Document Management (Month 2-3)                                           │
│  ├── Document upload & storage                                                      │
│  ├── Document processing pipeline (M03)                                             │
│  └── Embedding & vectorization (M04)                                                │
│                                                                                     │
│  Phase 3: Search & RAG (Month 3-4)                                                 │
│  ├── Semantic search (M05)                                                         │
│  ├── RAG chatbot (M06)                                                             │
│  └── Chat history (M07)                                                             │
│                                                                                     │
│  Phase 4: Admin & Analytics (Month 4-5)                                             │
│  ├── Admin dashboard (M08)                                                         │
│  └── Search analytics (M09)                                                         │
│                                                                                     │
│  Phase 5: Production (Month 5-6)                                                   │
│  ├── Testing & QA                                                                  │
│  ├── Performance optimization                                                      │
│  └── Deployment & monitoring                                                        │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

### 10.2. MVP 30 Days Highlight

| Day | Milestone | Deliverables |
|-----|-----------|--------------|
| 1-5 | Project Setup | Repository, Docker, CI/CD pipeline |
| 6-10 | Database | Schema, migrations, seed data |
| 11-15 | Auth Module | JWT, login, RBAC |
| 16-20 | Document Module | Upload, storage, processing |
| 21-25 | Search Module | Vector index, search API |
| 26-30 | Chat Module | RAG chatbot, citations |

---

## 11. Tài liệu tham khảo

### 11.1. Key Papers

1. **"Retrieval-Augmented Generation for Knowledge-Intensive NLP Tasks"**
   - Lewis et al., 2020
   - Facebook AI Research

2. **"Efficient and robust approximate nearest neighbor search using Hierarchical Navigable Small World graphs"**
   - Malkov & Yashunin, 2020
   - HNSW Algorithm

3. **"Dense Passage Retrieval for Open-Domain Question Answering"**
   - Karpukhin et al., 2020
   - DPR for RAG

4. **"RAG vs Fine-tuning: Which is Right for Your LLM Application?"**
   - Anyscale, 2024

### 11.2. External Resources

| Resource | Link |
|----------|------|
| pgvector GitHub | https://github.com/pgvector/pgvector |
| RAG Best Practices | https://docs.anyscale.com/tutorials/rag |
| OpenAI Embeddings | https://platform.openai.com/docs/guides/embeddings |
| Semantic Search Guide | https://www.pinecone.io/learn/semantic-search |

---

## 12. Liên hệ

### 12.1. Thông tin tác giả

| Field | Value |
|-------|-------|
| **Họ tên** | [Tên tác giả] |
| **Email** | [email@example.com] |
| **Mã số sinh viên** | [MSSV] |
| **Khoa** | Công nghệ Thông tin |
| **Trường** | Đại học [Tên trường] |

### 12.2. Thông tin giảng viên hướng dẫn

| Field | Value |
|-------|-------|
| **Họ tên** | [Tên giảng viên] |
| **Học hàm/Học vị** | [GS.TS / PGS.TS / TS] |
| **Khoa** | Công nghệ Thông tin |
| **Trường** | Đại học [Tên trường] |

---

## Appendix

### A. Quick Reference Commands

```bash
# Start development
docker-compose up -d

# Run migrations
cd src/backend && alembic upgrade head

# Seed data
cd src/backend && python -m scripts.seed

# Run tests
cd src/backend && pytest tests/

# Build frontend
cd src/frontend && npm run build

# View logs
docker-compose logs -f api

# Stop all services
docker-compose down
```

### B. Common Issues

| Issue | Solution |
|-------|----------|
| pgvector not found | Enable extension: `CREATE EXTENSION vector;` |
| Memory error on embedding | Reduce batch size in config |
| Slow search | Check HNSW index status |
| MinIO connection failed | Verify MINIO credentials in .env |

---

**Document Version**: 1.0  
**Last Updated**: 2026-05-03  
**Status**: Draft

---

*Đề tài được thực hiện cho mục đích bảo vệ luận văn thạc sĩ*
