# AI BASE FRAMEWORK - Deployment Plan & Code

**Dự án Base/Common**: Framework tích hợp AI cho mọi hệ thống
**Phiên bản**: 1.0
**Trạng thái**: Planning

---

## MỤC ĐÍCH

Đây là **dự án base/common** được thiết kế để **áp dụng cho tất cả các hệ thống** nếu muốn chuyển đổi thêm tính năng AI vào trong hệ thống. Framework này cung cấp nền tảng tích hợp AI 100% offline, có thể nhúng vào bất kỳ ứng dụng web, mobile, hoặc hệ thống doanh nghiệp nào.

**Lưu ý quan trọng**: Dự án này **không phục vụ cho web AIBaseFramework** đang có trong các tài liệu khác. Đây là một framework độc lập, có thể tái sử dụng cho bất kỳ hệ thống nào muốn tích hợp AI.

---

## ĐẶC ĐIỂM NỔI BẬT

| Đặc điểm | Mô tả |
|-----------|--------|
| **100% Offline** | Sử dụng Ollama (LLM local) - không phụ thuộc OpenAI/API bên ngoài |
| **Base Framework** | Có thể tích hợp vào bất kỳ hệ thống nào |
| **Tiếng Việt** | Hỗ trợ đầy đủ Tiếng Việt trong search và chat |
| **RAG Pipeline** | Retrieval-Augmented Generation với citations |
| **Vector Search** | Semantic search với pgvector + HNSW index |
| **Hybrid Search** | Kết hợp semantic (70%) + keyword (30%) |

---

## CẤU TRÚC THƯ MỤC

```
PlanTrienKhaiVaCode/
├── 01.DEPLOYMENT_PLAN/              # Kế hoạch triển khai chi tiết 30 ngày
├── 02.INFRASTRUCTURE/               # Docker, Scripts, Database
├── 03.BACKEND/                     # .NET 8 API (AIBaseFramework.API)
├── 04.FRONTEND/                    # Next.js 14 Frontend
└── 05.DOCUMENTATION/              # Tài liệu kiến trúc chi tiết
```

---

## CÔNG NGHỆ STACK

### Backend
- **.NET 8** + ASP.NET Core
- **Entity Framework Core 8** + **pgvector** (vector search)
- **MediatR** (CQRS pattern)
- **JWT** Authentication
- **Hangfire** (background jobs)
- **Redis** (cache) + **MinIO** (storage)
- **Ollama** (Local LLM - 100% offline)

### Frontend
- **Next.js 14** (App Router)
- **TypeScript 5**
- **Tailwind CSS** + shadcn/ui
- **Zustand** (state) + **TanStack Query** (server state)

### Infrastructure
- **Docker** + **Docker Compose**
- **PostgreSQL 16** + **pgvector**
- **Redis 7**
- **MinIO** (S3-compatible storage)
- **Nginx** (reverse proxy)
- **Ollama** (Local LLM)

---

## QUICK START

```bash
# 1. Copy environment file
cp 02.INFRASTRUCTURE/docker/.env.example .env

# 2. Start all services
docker-compose -f 02.INFRASTRUCTURE/docker/docker-compose.yml up -d

# 3. Download Ollama models
./02.INFRASTRUCTURE/scripts/download-models.sh

# 4. Access services
# - Frontend: http://localhost:3000
# - Backend API: http://localhost:5000
# - API Swagger: http://localhost:5000/swagger
# - MinIO Console: http://localhost:9001
# - Ollama API: http://localhost:11434
```

### Default Credentials
- **Email**: admin@yourcompany.com
- **Password**: admin123

---

## TÀI LIỆU CHI TIẾT

| Tài liệu | Mô tả |
|----------|--------|
| [01.DEPLOYMENT_PLAN/README.md](./01.DEPLOYMENT_PLAN/README.md) | Tổng quan kế hoạch triển khai |
| [01.DEPLOYMENT_PLAN/DEPLOYMENT_PLAN.md](./01.DEPLOYMENT_PLAN/DEPLOYMENT_PLAN.md) | Kế hoạch chi tiết 30 ngày |
| [02.INFRASTRUCTURE/README.md](./02.INFRASTRUCTURE/README.md) | Hướng dẫn Docker & Infrastructure |
| [03.BACKEND/README.md](./03.BACKEND/README.md) | Backend structure & API documentation |
| [04.FRONTEND/README.md](./04.FRONTEND/README.md) | Frontend structure & setup |
| [05.DOCUMENTATION/ARCHITECTURE.md](./05.DOCUMENTATION/ARCHITECTURE.md) | Kiến trúc hệ thống chi tiết |

---

## ỨNG DỤNG CỦA FRAMEWORK

Framework này có thể được tích hợp vào:

1. **Hệ thống E-commerce** - Tìm kiếm sản phẩm bằng câu hỏi tự nhiên
2. **Hệ thống LMS/E-Learning** - Chatbot hỏi đáp về bài giảng
3. **Hệ thống Quản lý tài liệu** - Semantic search trong kho tài liệu
4. **Hệ thống HR/Helpdesk** - Chatbot trả lời câu hỏi nhân sự
5. **Hệ thống Thư viện** - Tìm kiếm sách/bài báo nghiên cứu
6. **Bất kỳ hệ thống nào** - Cần tích hợp AI một cách dễ dàng

---

**Framework**: AI Base Framework (AIBaseFramework)
**Ngày cập nhật**: 2026-05-03
