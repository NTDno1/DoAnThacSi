# Hệ thống Tìm kiếm và Hỏi đáp Tài liệu Nội bộ sử dụng Semantic Search và RAG

**Đề tài luận văn thạc sĩ**

**Phiên bản:** 1.1
**Ngày cập nhật:** 2026-05-03
**Trạng thái:** Đang phát triển (đã bổ sung M10 RAG-SQL Engine)

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
| **Truy cập kết hợp Public/Private** | Không cần đăng nhập vẫn truy vấn được tài liệu, LLM và dữ liệu công khai; đăng nhập để mở rộng truy vấn database theo quyền RBAC |
| **Auto-DB Query (RAG-SQL)** | Người dùng đã xác thực có thể yêu cầu chatbot tự động truy vấn database thông qua SQL theo phạm vi quyền được cấp phát |

### 1.4. Mô hình truy cập kép (Public + Authenticated)

Hệ thống hỗ trợ hai chế độ truy cập linh hoạt, cho phép người dùng sử dụng ngay mà không cần đăng nhập, đồng thời mở rộng khả năng khi đã xác thực.

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                    MÔ HÌNH TRUY CẬP KẾT HỢP: PUBLIC + AUTHENTICATED               │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│  ┌──────────────────────────────────┐   ┌──────────────────────────────────┐        │
│  │      REQUEST KHÔNG TOKEN         │   │        REQUEST CÓ TOKEN          │        │
│  │         (Anonymous/Guest)         │   │       (Authenticated User)        │        │
│  └───────────────┬──────────────────┘   └───────────────┬──────────────────┘        │
│                  │                                      │                             │
│                  ▼                                      ▼                             │
│  ┌──────────────────────────────────┐   ┌──────────────────────────────────┐        │
│  │  ✅ Tài liệu public (doc)       │   │  ✅ Tất cả quyền của Anonymous    │        │
│  │  ✅ LLM (chatbot RAG)           │   │  ✅ Tài liệu theo phân quyền     │        │
│  │  ✅ Dữ liệu public trong DB     │   │  ✅ Database query theo RBAC      │        │
│  │  ✅ Semantic/Hybrid Search       │   │  ✅ Auto-DB Query (RAG-SQL)       │        │
│  │  ❌ Database RBAC private        │   │  ✅ Chat history cá nhân          │        │
│  │  ❌ Chat history riêng tư        │   │  ✅ Upload/quản lý tài liệu      │        │
│  │  ❌ Upload tài liệu cá nhân      │   │                                  │        │
│  └──────────────────────────────────┘   └──────────────────────────────────┘        │
│                                                                                     │
│  ┌───────────────────────────────────────────────────────────────────────────────┐   │
│  │                          CHI TIẾT QUYỀN RBAC                                   │   │
│  ├───────────────────────────────────────────────────────────────────────────────┤   │
│  │                                                                               │   │
│  │  Role         │ Tài liệu    │ Database Tables  │ Auto-DB Query │ Admin       │   │
│  │  ─────────────┼─────────────┼──────────────────┼───────────────┼─────────────│   │
│  │  Anonymous    │ Public only │ Public data only │ ❌            │ ❌           │   │
│  │  Viewer       │ Assigned    │ Read-only (granted│ Limited       │ ❌           │   │
│  │               │ dept+public │ tables)          │               │             │   │
│  │  Editor       │ Assigned    │ Read+Write        │ Yes           │ ❌           │   │
│  │               │ dept+public │ (granted tables)  │               │             │   │
│  │  Manager      │ All dept    │ Full access       │ Yes           │ Read-only   │   │
│  │               │ + public    │ (dept scope)      │               │             │   │
│  │  Admin        │ All         │ All tables        │ Yes           │ Full        │   │
│  │               │             │                  │               │             │   │
│  └───────────────────────────────────────────────────────────────────────────────┘   │
│                                                                                     │
│  ┌───────────────────────────────────────────────────────────────────────────────┐   │
│  │                    AUTO-DB QUERY (RAG-SQL) FLOW                                  │   │
│  ├───────────────────────────────────────────────────────────────────────────────┤   │
│  │                                                                               │   │
│  │  User Query ──► LLM Parse Intent ──► Detect DB Query Need ──►                 │   │
│  │       │                                              │                         │   │
│  │       │                                              ▼                         │   │
│  │       │                                     ┌─────────────────┐               │   │
│  │       │                                     │ RBAC Permission │               │   │
│  │       │                                     │    Check        │               │   │
│  │       │                                     └────────┬────────┘               │   │
│  │       │                                              │                         │   │
│  │       │                        ┌─────────────────────┼─────────────────────┐   │   │
│  │       │                        │                     │                     │   │   │
│  │       │                        ▼                     ▼                     ▼   │   │
│  │       │                  ┌──────────┐       ┌──────────┐          ┌──────────┐│   │
│  │       │                  │  ✅ Allow │       │ ❌ Deny   │          │ ⚠️ Limit │   │   │
│  │       │                  │  Execute  │       │ No Access│          │  Scope   │   │   │
│  │       │                  │  + Return │       │  Return  │          │ Return   │  │   │
│  │       │                  │  Results  │       │  Error   │          │ Partial  │  │   │
│  │       │                  └────┬──────┘       └──────────┘          └────┬─────┘   │
│  │       │                       │                                          │        │
│  │       ▼                       ▼                                          ▼        │
│  │  LLM Generate Response ─────────────────────────────────────────────────────────►  │
│  │       │                                                                               │
│  │       ▼                                                                               │
│  │  User receives: Answer + DB Results + Citations                                        │
│  │                                                                               │   │
│  └─────────────────────────────────────────────────────────────────────────────────────┘   │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

### 1.5. Tech Stack tổng quan

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
| **API Gateway** | Điều hướng request, kiểm tra token, phân quyền | FastAPI, Authentication, Rate Limiting, RBAC Engine |
| **Service Layer** | Business logic | Auth, Document, Search, Chat, Embedding, LLM, RAG-SQL Engine |
| **Data Layer** | Lưu trữ dữ liệu | PostgreSQL, Redis, MinIO |

### 2.3. Mô hình bảo mật hai lớp (Public + Authenticated)

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                         SECURITY ARCHITECTURE: TWO-LAYER ACCESS                      │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│                              ┌─────────────────┐                                    │
│                              │  HTTP Request   │                                    │
│                              └────────┬────────┘                                    │
│                                       │                                              │
│                                       ▼                                              │
│                        ┌─────────────────────────────┐                              │
│                        │    API Gateway Layer         │                              │
│                        │  ┌───────────────────────┐  │                              │
│                        │  │  Token Validation      │  │                              │
│                        │  │  (JWT / Bearer)       │  │                              │
│                        │  └───────────┬───────────┘  │                              │
│                        │              │               │                              │
│                        │    ┌─────────┴─────────┐    │                              │
│                        │    │                    │    │                              │
│                        │    ▼                    ▼    │                              │
│                        │  NO TOKEN           HAS TOKEN│                              │
│                        │    │                    │    │                              │
│                        │    ▼                    ▼    │                              │
│                        │  Anonymous           Authenticated                          │
│                        │  Context             + RBAC Context                        │
│                        │    │                    │    │                              │
│                        └────┼────────────────────┼────┘                              │
│                             │                    │                                   │
│                             ▼                    ▼                                   │
│              ┌──────────────────────┐  ┌──────────────────────┐                    │
│              │  PUBLIC ACCESS       │  │  PRIVATE ACCESS      │                    │
│              │  (No Auth Required)  │  │  (Auth + RBAC)       │                    │
│              ├──────────────────────┤  ├──────────────────────┤                    │
│              │  ✅ /api/search     │  │  ✅ All public APIs  │                    │
│              │  ✅ /api/chat        │  │  ✅ /api/documents/* │                    │
│              │  ✅ /api/documents  │  │  ✅ /api/db/query   │                    │
│              │     /public          │  │  ✅ /api/sessions/* │                    │
│              │  ✅ /api/llm/chat    │  │  ✅ /api/admin/*    │                    │
│              │  ✅ Public metadata  │  │  ✅ RBAC-controlled  │                    │
│              │  ✅ Public DB tables │  │     DB tables       │                    │
│              ├──────────────────────┤  ├──────────────────────┤                    │
│              │  ❌ Private docs    │  │  ✅ Auto-DB Query    │                    │
│              │  ❌ Chat history    │  │     (RAG-SQL)       │                    │
│              │  ❌ RBAC tables     │  │                      │                    │
│              │  ❌ User-specific   │  │                      │                    │
│              │     resources       │  │                      │                    │
│              └──────────────────────┘  └──────────────────────┘                    │
│                                           │                                          │
│                                           ▼                                          │
│                              ┌─────────────────────────┐                            │
│                              │   RBAC Permission Engine │                            │
│                              ├─────────────────────────┤                            │
│                              │  1. Extract role + dept │                            │
│                              │  2. Check document scope │                            │
│                              │  3. Check table access   │                            │
│                              │  4. Check query limits   │                            │
│                              │  5. Return decision      │                            │
│                              └────────────┬────────────┘                            │
│                                           │                                         │
│                                           ▼                                         │
│                              ┌─────────────────────────┐                            │
│                              │   RAG-SQL Engine        │                            │
│                              │  (Auto DB Query)        │                            │
│                              ├─────────────────────────┤                            │
│                              │  • LLM parses intent    │                            │
│                              │  • Generate safe SQL    │                            │
│                              │  • RBAC filter applied  │                            │
│                              │  • Execute + return     │                            │
│                              └─────────────────────────┘                            │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

### 2.4. Auto-DB Query (RAG-SQL) Flow

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                         RAG-SQL (AUTO DATABASE QUERY) FLOW                            │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│  STEP 1: REQUEST NHẬN DIỆN                                                         │
│  ┌─────────────────────────────────────────────────────────────────────────────┐    │
│  │  User Query: "Cho tôi xem doanh thu tháng 3 của phòng ban kỹ thuật"         │    │
│  │                    │                                                            │    │
│  │                    ▼                                                            │    │
│  │  LLM Intent Detection                                                          │    │
│  │  ├── Intent: DB_QUERY (cần truy vấn database)                                │    │
│  │  ├── Detected Tables: revenue, departments                                   │    │
│  │  ├── Required Role: Viewer+                                                   │    │
│  │  └── Query Type: SELECT (read-only)                                          │    │
│  └─────────────────────────────────────────────────────────────────────────────┘    │
│                                          │                                            │
│                                          ▼                                            │
│  STEP 2: RBAC PERMISSION CHECK                                                        │
│  ┌─────────────────────────────────────────────────────────────────────────────┐    │
│  │  Current User: user_001 (Role: Viewer, Dept: Engineering)                   │    │
│  │                                                                               │    │
│  │  Permission Matrix Check:                                                     │    │
│  │  ┌────────────────┬────────────┬───────────┬────────────┐                    │    │
│  │  │ Table          │ Permission │ Dept Scope│ Check Result│                   │    │
│  │  ├────────────────┼────────────┼───────────┼────────────┤                    │    │
│  │  │ revenue        │ SELECT     │ Engineering│ ✅ ALLOW    │                    │    │
│  │  │ departments    │ SELECT     │ Engineering│ ✅ ALLOW    │                    │    │
│  │  │ employees      │ SELECT     │ Engineering│ ✅ ALLOW    │                    │    │
│  │  │ salaries       │ SELECT     │ ❌ Dept only│ ⚠️ LIMITED │                    │    │
│  │  │ hr_records     │ SELECT     │ ❌ HR only  │ ❌ DENY    │                    │    │
│  │  └────────────────┴────────────┴───────────┴────────────┘                    │    │
│  │                                                                               │    │
│  │  Decision: ALLOW (with dept filter applied)                                   │    │
│  └─────────────────────────────────────────────────────────────────────────────┘    │
│                                          │                                            │
│                                          ▼                                            │
│  STEP 3: SQL GENERATION & SAFETY                                                      │
│  ┌─────────────────────────────────────────────────────────────────────────────┐    │
│  │  LLM generates SQL:                                                         │    │
│  │  SELECT r.month, r.amount, d.name as dept                                  │    │
│  │  FROM revenue r                                                             │    │
│  │  JOIN departments d ON r.dept_id = d.id                                     │    │
│  │  WHERE r.month = '2026-03'                                                 │    │
│  │    AND d.name = 'Engineering'  -- RBAC auto-filter                        │    │
│  │                                                                               │    │
│  │  Safety Checks:                                                              │    │
│  │  ├── ❌ DDL blocked (DROP, ALTER, CREATE)                                  │    │
│  │  ├── ❌ DML blocked (INSERT, UPDATE, DELETE)                               │    │
│  │  ├── ❌ PII fields masked (ssn, password_hash)                            │    │
│  │  ├── ✅ SELECT only                                                         │    │
│  │  └── ✅ Row-level security enforced                                         │    │
│  └─────────────────────────────────────────────────────────────────────────────┘    │
│                                          │                                            │
│                                          ▼                                            │
│  STEP 4: EXECUTION & RESULT                                                          │
│  ┌─────────────────────────────────────────────────────────────────────────────┐    │
│  │  Query executed on PostgreSQL:                                              │    │
│  │  ┌─────────────────────────────────────────────────────────────────────────┐ │    │
│  │  │  month    │  amount    │ dept          │                              │ │    │
│  │  ├───────────┼────────────┼───────────────┤                              │ │    │
│  │  │ 2026-03   │ 150,000    │ Engineering   │                              │ │    │
│  │  │ 2026-03   │ 120,000    │ Engineering   │                              │ │    │
│  │  └─────────────────────────────────────────────────────────────────────────┘ │    │
│  └─────────────────────────────────────────────────────────────────────────────┘    │
│                                          │                                            │
│                                          ▼                                            │
│  STEP 5: LLM GENERATES RESPONSE                                                      │
│  ┌─────────────────────────────────────────────────────────────────────────────┐    │
│  │  LLM synthesizes answer:                                                    │    │
│  │                                                                               │    │
│  │  "Doanh thu tháng 3 năm 2026 của phòng ban Kỹ thuật:                        │    │
│  │                                                                               │    │
│  │  • Module A: 150,000 VNĐ                                                    │    │
│  │  • Module B: 120,000 VNĐ                                                    │    │
│  │  • Tổng cộng: 270,000 VNĐ                                                   │    │
│  │                                                                               │    │
│  │  Nguồn: Bảng revenue (đã được lọc theo phòng ban của bạn)"                  │    │
│  │                                                                               │    │
│  │  Citations:                                                                  │    │
│  │  • revenue.month = '2026-03'                                                │    │
│  │  • departments.name = 'Engineering'                                         │    │
│  └─────────────────────────────────────────────────────────────────────────────┘    │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

### 2.5. Data Flow cho RAG

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
│       │  │                            ┌────────────────────────────┼─────┐   │   │
│       │  │                            │                            │     │   │   │
│       │  │                            ▼                            ▼     │   │   │
│       │  │                     ┌────────────┐              ┌─────────────┐ │   │
│       │  │                     │ Document   │              │ DB Query    │ │   │
│       │  │                     │ Retrieval  │              │ (RAG-SQL)  │ │   │
│       │  │                     │ (Vector)   │              │ (if token) │ │   │
│       │  │                     └─────┬──────┘              └──────┬─────┘ │   │
│       │  │                           │                            │       │   │
│       │  │                           └──────────┬─────────────────┘       │   │
│       │  │                                  ▼                           │   │
│       │  │  ┌─────────┐     ┌─────────┐  ┌─────────┐  ┌─────────────┐  │   │
│       │  │  │  LLM    │<────│ Augment │<─│  Build  │<─│   Context   │  │   │
│       │  │  │Response │     │ Prompt  │  │ Context │  │  Retrieval  │  │   │
│       │  │  └────┬────┘     └─────────┘  └─────────┘  └─────────────┘  │   │
│       │  │       │                                                            │   │
│       │  └───────┼────────────────────────────────────────────────────────┘   │
│       │          ▼                                                                   │
│       │  ┌────────────────────────────────────────────────────────────────────┐   │
│       │  │                     USER RESPONSE                                   │   │
│       │  │      Answer + Source Citations + DB Results (if applicable)        │   │
│       │  └────────────────────────────────────────────────────────────────────┘   │
│       │                                                                               │
│       └───────────────────────────────────────────────────────────────────────────┘   │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

### 2.6. Mô hình truy cập Public/Private - Chi tiết từng API

| API Endpoint | Không Token | Có Token (Role) | Ghi chú |
|-------------|-------------|----------------|---------|
| `GET /api/search` | ✅ Public docs only | ✅ Full search + RBAC | Lọc theo dept nếu có token |
| `POST /api/chat` | ✅ RAG public docs | ✅ RAG + RAG-SQL + RBAC | Tự động phát hiện DB query |
| `GET /api/documents/public` | ✅ Public docs | ✅ Public + assigned docs | |
| `GET /api/documents/{id}` | ✅ Nếu public | ✅ Nếu được phép | Kiểm tra ownership/dept |
| `GET /api/llm/chat` | ✅ Không định danh | ✅ Theo user context | |
| `POST /api/db/query` | ❌ Forbidden | ✅ Viewer+ only | RAG-SQL endpoint |
| `GET /api/sessions` | ❌ Forbidden | ✅ Own sessions only | |
| `POST /api/admin/*` | ❌ Forbidden | ✅ Admin only | |

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
│   │   ├── 03.MODULES.md            # 📦 Tài liệu 10 modules và dependencies
│   │   └── 03b.RAG_SQL_MODULE.md   # 🗄️ Chi tiết module RAG-SQL Engine
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
| `03.MODULES.md` | 10 modules với specifications và dependencies | Developer |
| `03b.RAG_SQL_MODULE.md` | Chi tiết module RAG-SQL Engine, RBAC integration | Developer |
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

### 5.1. Danh sách 10 modules

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
| **M10** | RAG-SQL Engine Module | Auto database query, RBAC-filtered SQL generation, secure execution | P0 - Core |

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
│           ┌────────────┴────────────┐                                               │
│           ▼                          ▼                                               │
│  ┌─────────────────────┐   ┌─────────────────────┐                                 │
│  │  M09 Search         │   │  M10 RAG-SQL Engine │                                 │
│  │  Analytics          │   │  (Auto DB Query)    │                                 │
│  └─────────────────────┘   └─────────────────────┘                                 │
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

#### M10 - RAG-SQL Engine Module
- **Chức năng**: Auto database query, RBAC-filtered SQL generation, secure SQL execution
- **Chi tiết chức năng**:
  - Phát hiện intent truy vấn database từ câu hỏi người dùng
  - Sinh SQL an toàn từ LLM (SELECT only)
  - RBAC permission check trước khi thực thi
  - Row-level security filter theo phòng ban/vai trò
  - PII field masking
  - Execute và trả kết quả về cho LLM tổng hợp
- **Entities**: RBAC Permission Matrix, Allowed Tables, Audit Logs
- **Endpoints**: Internal (integrated with M06 Chatbot), `/api/db/query` (direct)
- **Dependencies**: M01 (Auth), M05 (Search), M06 (Chatbot)
- **Cấp quyền theo role**:

  | Role | DB Query | Allowed Tables | Row Filter |
  |------|----------|----------------|------------|
  | Anonymous | ❌ | Public tables only | Dept scope |
  | Viewer | ✅ | Read-only assigned tables | Dept scope |
  | Editor | ✅ | Read/write assigned tables | Dept scope |
  | Manager | ✅ | Full dept scope | Dept scope |
  | Admin | ✅ | All tables | None |

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
| **LLM** | Ollama (llama3.2, mistral) | latest | **Local, offline, không cần API** |
| **Embeddings** | Ollama (nomic-embed-text) | latest | **Local, offline, không cần API** |
| **Container** | Docker | 24+ | Containerization |
| **Orchestration** | Docker Compose | 2+ | Local dev |
| **Monitoring** | Prometheus | 2+ | Metrics |
| **Logging** | Serilog | 3+ | Structured logging |
| **SQL Safety** | SQLGlot / RE2 | latest | SQL parsing, regex validation |
| **RBAC Engine** | Casbin | 3+ | Policy enforcement |

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
| Ollama | https://github.com/ollama/ollama |
| TailwindCSS | https://tailwindcss.com/docs |
| Casbin | https://casbin.org/ |
| SQLGlot | https://github.com/tobymao/sqlglot |

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
│   - feature/M##-<description>  (vd: feature/M01-auth-jwt, feature/M10-rag-sql)       │
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

### 8.2. Danh sách Services và Images khi triển khai hoàn chỉnh

Khi dự án được triển khai thành công, `docker ps` và `docker images` sẽ hiển thị các thành phần sau:

#### 8.2.1. Kết quả `docker ps` - Tất cả Services đang chạy

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                              DOCKER PS - SERVICES ĐANG CHẠY                          │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│  CONTAINER ID   IMAGE                        PORTS                    STATUS        │
│  ─────────────  ───────────────────────────  ───────────────────────  ────────────  │
│  a1b2c3d4e5f6   doanthacsi-api              0.0.0.0:8000->8000/tcp   Up (healthy)  │
│  b2c3d4e5f6a7   doanthacsi-frontend         0.0.0.0:3000->3000/tcp   Up (healthy)  │
│  c3d4e5f6a7b8   postgres:16                 0.0.0.0:5432->5432/tcp   Up (healthy)  │
│  d4e5f6a7b8c9   redis:7-alpine             0.0.0.0:6379->6379/tcp   Up (healthy)  │
│  e5f6a7b8c9d0   minio/minio                 0.0.0.0:9000->9000/tcp   Up (healthy)  │
│                                                  0.0.0.0:9001->9001/tcp              │
│  f6a7b8c9d0e1   minio/mc                    -                         Up           │
│  g7h8i9j0k1l2   ollama/ollama               0.0.0.0:11434->11434/tcp Up (healthy)  │
│  h8i9j0k1l2m3   redis:7-alpine             (minio-cache)             Up           │
│  i9j0k1l2m3n4   postgres:16                (pgvector-init)            Up           │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

#### 8.2.2. Chi tiết từng Service

| # | Container Name | Image | Ports | Mô tả | Health Status |
|---|--------------|-------|-------|--------|--------------|
| 1 | **api** | `doanthacsi-api` | `8000:8000` | FastAPI Backend - Xử lý tất cả API requests | ✅ Healthy |
| 2 | **frontend** | `doanthacsi-frontend` | `3000:3000` | Next.js Frontend - Giao diện người dùng | ✅ Healthy |
| 3 | **db** | `postgres:16` | `5432:5432` | PostgreSQL + pgvector - Lưu trữ dữ liệu & vector embeddings | ✅ Healthy |
| 4 | **redis** | `redis:7-alpine` | `6379:6379` | Redis - Cache & Session management | ✅ Healthy |
| 5 | **minio** | `minio/minio` | `9000:9000`, `9001:9001` | MinIO S3-compatible - Object storage cho tài liệu | ✅ Healthy |
| 6 | **minio-init** | `minio/mc` | - | MinIO Client - Khởi tạo bucket & cấu hình | ⏸️ Exited |
| 7 | **ollama** | `ollama/ollama` | `11434:11434` | Ollama - Local LLM & Embedding models | ✅ Healthy |
| 8 | **ollama-models** | (embedded) | - | Models: llama3.2, mistral, nomic-embed-text | ✅ Running |

#### 8.2.3. Kết quả `docker images` - Tất cả Images

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                              DOCKER IMAGES - TẤT CẢ IMAGES                            │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│  REPOSITORY              TAG         IMAGE ID       SIZE       CREATED              │
│  ──────────────────────  ──────────  ────────────   ────────   ──────────────────  │
│  doanthacsi-api          latest      a1b2c3d4e5f6   1.2GB      ...                  │
│  doanthacsi-frontend     latest      b2c3d4e5f6a7   890MB      ...                  │
│  postgres                16          c3d4e5f6a7b8    750MB      ...                  │
│  redis                   7-alpine    d4e5f6a7b8c9    30MB       ...                  │
│  minio/minio             latest      e5f6a7b8c9d0    230MB      ...                  │
│  minio/mc                latest      f6a7b8c9d0e1    50MB       ...                  │
│  ollama/ollama           latest      g7h8i9j0k1l2    2.1GB      ...                  │
│  ubuntu                  22.04       h8i9j0k1l2m3    80MB       ...                  │
│                                                                                     │
│  OLLAMA MODELS (chạy trong container ollama):                                      │
│  ────────────────────────────────────────────────────────────────────────────────   │
│  llama3.2                latest       -               2.0GB      Pulled              │
│  mistral                 latest       -               4.1GB      Pulled              │
│  nomic-embed-text        latest       -               274MB      Pulled              │
│  mxbai-embed-large       latest       -               1.1GB      Pulled              │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

#### 8.2.4. Chi tiết Images

| # | Repository | Tag | Size | Mô tả |
|---|-----------|-----|------|--------|
| 1 | **doanthacsi-api** | `latest` | ~1.2GB | Custom image - FastAPI backend với tất cả dependencies (Python 3.11, SQLAlchemy, FastAPI, Ollama client, pgvector) |
| 2 | **doanthacsi-frontend** | `latest` | ~890MB | Custom image - Next.js 14 frontend với TypeScript, TailwindCSS, React |
| 3 | **postgres** | `16` | ~750MB | Official PostgreSQL 16 image với pgvector extension |
| 4 | **redis** | `7-alpine` | ~30MB | Official Redis 7 Alpine - Nhẹ, tối ưu cho cache |
| 5 | **minio/minio** | `latest` | ~230MB | MinIO Server - S3-compatible object storage |
| 6 | **minio/mc** | `latest` | ~50MB | MinIO Client - Utility để init buckets |
| 7 | **ollama/ollama** | `latest` | ~2.1GB | Ollama Server - Local LLM inference engine |

#### 8.2.5. Ollama Models (Pulled vào container)

| Model | Size | Type | Mục đích |
|-------|------|------|----------|
| **llama3.2** | ~2.0GB | Chat | Chatbot RAG - Trả lời câu hỏi |
| **mistral** | ~4.1GB | Chat | Chatbot RAG - Chatbot chính (nếu cần) |
| **nomic-embed-text** | ~274MB | Embedding | Tạo vector embeddings cho tài liệu |
| **mxbai-embed-large** | ~1.1GB | Embedding | Embedding model dự phòng (chất lượng cao) |

#### 8.2.6. Networks được tạo tự động

| Network Name | Driver | Mô tả |
|-------------|--------|--------|
| **doanthacsi_default** | bridge | Network mặc định của docker-compose, kết nối tất cả services |
| **doanthacsi_ollama** | bridge | Network riêng cho Ollama (internal) |

#### 8.2.7. Volumes được tạo tự động

| Volume Name | Driver | Mount Point | Mô tả |
|-------------|--------|------------|--------|
| **doanthacsi_postgres_data** | local | `/var/lib/postgresql/data` | Lưu trữ database PostgreSQL |
| **doanthacsi_minio_data** | local | `/data` | Lưu trữ object storage (tài liệu files) |
| **doanthacsi_ollama_models** | local | `/root/.ollama` | Lưu trữ LLM models đã pull |

#### 8.2.8. Ports Mapping tổng hợp

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                              PORTS MAPPING SUMMARY                                    │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│  Service       │ Internal Port │ External Port │ Protocol │ URL                    │
│  ──────────────┼──────────────┼───────────────┼──────────┼─────────────────────   │
│  Frontend      │ 3000         │ 3000          │ HTTP     │ http://localhost:3000   │
│  API Gateway   │ 8000         │ 8000          │ HTTP     │ http://localhost:8000   │
│  API Docs      │ 8000/docs    │ 8000/docs     │ HTTP     │ http://localhost:8000   │
│  PostgreSQL    │ 5432         │ 5432          │ TCP      │ localhost:5432          │
│  Redis         │ 6379         │ 6379          │ TCP      │ localhost:6379          │
│  MinIO API     │ 9000         │ 9000          │ HTTP     │ http://localhost:9000   │
│  MinIO Console │ 9001         │ 9001          │ HTTP     │ http://localhost:9001   │
│  Ollama        │ 11434        │ 11434         │ HTTP     │ http://localhost:11434  │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

#### 8.2.9. Kiểm tra trạng thái sau triển khai

```bash
# Kiểm tra tất cả services đang chạy
docker ps

# Kiểm tra health của từng service
docker ps --format "table {{.Names}}\t{{.Status}}"

# Kiểm tra logs của API
docker logs doanthacsi-api --tail 50

# Kiểm tra logs của Frontend
docker logs doanthacsi-frontend --tail 50

# Kiểm tra kết nối database
docker exec -it doanthacsi-db psql -U postgres -d document_rag -c "\\dt"

# Kiểm tra Ollama models
curl http://localhost:11434/api/tags

# Kiểm tra MinIO buckets
docker exec -it doanthacsi-minio mc ls local/

# Kiểm tra Redis
docker exec -it doanthacsi-redis redis-cli ping

# Kiểm tra API health
curl http://localhost:8000/health

# Kiểm tra API readiness
curl http://localhost:8000/health/ready
```

#### 8.2.10. Lệnh quản lý sau triển khai

```bash
# Stop tất cả services
docker-compose down

# Stop và xóa volumes
docker-compose down -v

# Restart một service cụ thể
docker-compose restart api

# Rebuild một service cụ thể
docker-compose up -d --build api

# Xem resource usage
docker stats

# Clean up unused images
docker image prune -a

# Pull latest Ollama models
docker exec doanthacsi-ollama ollama pull llama3.2
docker exec doanthacsi-ollama ollama pull nomic-embed-text

# Backup database
docker exec doanthacsi-db pg_dump -U postgres document_rag > backup.sql

# Restore database
docker exec -i doanthacsi-db psql -U postgres document_rag < backup.sql
```

### 8.3. Kubernetes-Ready Structure

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
│  ├── Chat history (M07)                                                             │
│  └── RAG-SQL engine (M10)                                                          │
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

5. **"Binder: Binding Language Model to Domain-Specific Language for SQL Question Answering"**
   - Cheng et al., 2024
   - Text-to-SQL with RBAC Integration

6. **"A Survey on Large Language Models for Critical Societal Systems: Finance, Healthcare, and Law"**
   - Anand et al., 2024
   - Safe LLM Deployment in Enterprise

### 11.2. External Resources

| Resource | Link |
|----------|------|
| pgvector GitHub | https://github.com/pgvector/pgvector |
| RAG Best Practices | https://docs.anyscale.com/tutorials/rag |
| Ollama (Local LLM) | https://github.com/ollama/ollama |
| Semantic Search Guide | https://www.pinecone.io/learn/semantic-search |
| RAG-SQL: Text-to-SQL with RAG | https://arxiv.org/abs/2308.14739 |
| Casbin RBAC | https://casbin.org/ |
| SQLGlot SQL Parser | https://github.com/tobymao/sqlglot |

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

**Document Version**: 1.1
**Last Updated**: 2026-05-03
**Status**: Draft
**Changes**: Added M10 RAG-SQL Engine Module with Public/Private Access Model

---

*Đề tài được thực hiện cho mục đích bảo vệ luận văn thạc sĩ*
