# Docker Infrastructure - Hệ thống Tìm kiếm và Hỏi đáp Tài liệu Nội bộ

## Mục lục

- [Tổng quan](#tổng-quan)
- [Cấu trúc thư mục](#cấu-trúc-thư-mục)
- [Kiến trúc 100% Offline Local Docker](#kiến-trúc-100-offline-local-docker)
- [Yêu cầu hệ thống](#yêu-cầu-hệ-thống)
- [Hướng dẫn cài đặt](#hướng-dẫn-cài-đặt)
  - [Development](#development)
  - [Production](#production)
- [Ollama - LLM Local](#ollama---llm-local)
- [Services](#services)
- [Environment Variables](#environment-variables)
- [Các lệnh Docker thường dùng](#các-lệnh-docker-thường-dùng)
- [Kiểm tra và Troubleshooting](#kiểm-tra-và-troubleshooting)
- [Security](#security)
- [Backup và Restore](#backup-và-restore)

---

## Tổng quan

Đây là bộ infrastructure Docker cho đề tài **"Hệ thống Tìm kiếm và Hỏi đáp Tài liệu Nội bộ sử dụng Semantic Search và RAG"** - Luận văn Thạc sĩ.

### Đặc điểm kiến trúc: 100% Offline Local Docker

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                    🎯 KIẾN TRÚC 100% OFFLINE LOCAL                            │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ✅ Tất cả services chạy trên Docker (máy local của bạn)                │
│  ✅ Sử dụng Ollama thay vì OpenAI API (KHÔNG cần Internet)              │
│  ✅ Embedding model chạy local (nomic-embed-text)                         │
│  ✅ Không gửi dữ liệu ra Internet                                         │
│  ✅ Bảo mật tuyệt đối - dữ liệu không rời máy bạn                       │
│                                                                              │
└─────────────────────────────────────────────────────────────────────────────┘
```

### Công nghệ sử dụng

| Service | Image | Mục đích |
|---------|-------|-----------|
| **Ollama** | ollama/ollama:latest | LLM local (chat + embeddings) |
| PostgreSQL + pgvector | pgvector/pgvector:pg16 | Vector database cho semantic search |
| Redis | redis:7-alpine | Cache và session store |
| MinIO | minio/minio | Object storage cho tài liệu |
| Backend API | Custom (.NET 8) | REST API xử lý nghiệp vụ |
| Frontend | Custom (React + Vite) | Giao diện người dùng |
| Nginx | nginx:alpine | Reverse proxy (Production) |

> **Lưu ý**: Hệ thống **KHÔNG sử dụng OpenAI API**. Tất cả LLM và embedding models chạy local bằng Ollama.

---

## Cấu trúc thư mục

```
DoAnThacSi/docker/
├── docker-compose.yml          # Development compose (100% offline)
├── docker-compose.prod.yml    # Production compose
├── init.sql                   # Database initialization
├── .env.example               # Environment variables template (Ollama config)
├── Dockerfile.backend         # Backend multi-stage build
├── Dockerfile.frontend       # Frontend multi-stage build
├── Dockerfile.ollama          # Ollama Dockerfile (optional, for pre-downloading)
├── nginx.conf                 # Nginx configuration
├── scripts/
│   └── download-models.sh     # Script để download Ollama models
├── README.md                  # This file
└── ssl/                       # SSL certificates (production)
```

---

## Kiến trúc 100% Offline Local Docker

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                        KIẾN TRÚC 100% OFFLINE LOCAL DOCKER                             │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│  Tất cả services chạy trên Docker (máy local của bạn)                                 │
│                                                                                     │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐    ┌──────────────┐      │
│  │   Frontend   │    │   Backend    │    │   Ollama    │    │   MinIO     │      │
│  │  (Next.js)  │    │  (.NET API)  │    │  (LLM Local) │    │  (Storage)  │      │
│  │   :3000      │    │    :5000      │    │   :11434      │    │    :9000     │      │
│  └──────────────┘    └──────────────┘    └──────────────┘    └──────────────┘      │
│          │                  │                  │                  │                    │
│          └──────────────────┴──────────────────┴──────────────────┘                    │
│                                              │                                          │
│  ┌───────────────────────────────────────────┼───────────────────────────────────┐      │
│  │                           DATA LAYER                                          │      │
│  │  ┌──────────────┐    ┌──────────────┐    ┌──────────────────────────┐  │      │
│  │  │  PostgreSQL   │    │     Redis    │    │        Ollama Models      │  │      │
│  │  │  + pgvector  │    │   (Cache)    │    │                          │  │      │
│  │  │    :5432       │    │    :6379     │    │  • llama3.2:3b (chat) │  │      │
│  │  │                │    │              │    │  • nomic-embed-text  │  │      │
│  │  │                │    │              │    │    (embedding)        │  │      │
│  │  └──────────────┘    └──────────────┘    └──────────────────────────┘  │      │
│  └───────────────────────────────────────────────────────────────────────────────┘      │
│                                                                                     │
│  ══════════════════════════════════════════════════════════════════════════════════  │
│  🔒 KHÔNG CÓ KẾT NỐI INTERNET - 100% LOCAL                                      │
│  ══════════════════════════════════════════════════════════════════════════════════  │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

---

## Yêu cầu hệ thống

### Development
- Docker Desktop 4.0+ (với WSL2 backend khuyến nghị)
- **8GB RAM tối thiểu** (16GB khuyến nghị)
- 20GB disk space cho Docker images + models
- **NVIDIA GPU (khuyến nghị)** cho inference nhanh
  - CUDA 11.8+ cho GPU support
  - Nếu không có GPU, vẫn chạy được ở CPU mode (chậm hơn)

### Production
- Docker Engine 24.0+
- Docker Compose v2
- 16GB RAM tối thiểu
- 100GB SSD disk space
- NVIDIA GPU cho production (khuyến nghị)

---

## Hướng dẫn cài đặt

### Development

**Bước 1: Copy file .env**

```powershell
cd DoAnThacSi\docker
copy .env.example .env
```

**Bước 2: Chỉnh sửa .env (Cấu hình Ollama)**

Mở file `.env` và cập nhật các giá trị Ollama:
- `OLLAMA_CHAT_MODEL`: Model cho chatbot (mặc định: llama3.2:3b)
- `OLLAMA_EMBEDDING_MODEL`: Model cho embedding (mặc định: nomic-embed-text)
- `CUDA_VISIBLE_DEVICES`: Set `0` cho GPU đầu tiên, hoặc `""` cho CPU-only

**Lưu ý**: KHÔNG cần OpenAI API key. Hệ thống sử dụng Ollama để chạy LLM local.

**Bước 3: Khởi động services**

```powershell
# Khởi động tất cả services
docker-compose up -d

# Xem logs
docker-compose logs -f

# Xem logs của một service cụ thể
docker-compose logs -f backend
```

**Bước 4: Download Ollama Models (Lần đầu tiên)**

```powershell
# Scripts để download models (chạy sau khi Ollama container đã start)
# Linux/Mac:
./scripts/download-models.sh

# Windows (PowerShell):
bash ./scripts/download-models.sh

# Hoặc download manual từng model:
docker exec aibf_ollama ollama pull llama3.2:3b
docker exec aibf_ollama ollama pull nomic-embed-text
```

**Bước 5: Khởi động services**

```powershell
# Khởi động tất cả services (đợi Ollama start trước)
docker-compose up -d

# Đợi Ollama health check pass
docker-compose ps

# Xem logs
docker-compose logs -f

# Xem logs của một service cụ thể
docker-compose logs -f backend
docker-compose logs -f ollama
```

**Bước 6: Truy cập ứng dụng**

| Service | URL |
|---------|-----|
| Frontend | http://localhost:3000 |
| Backend API | http://localhost:5000 |
| API Health | http://localhost:5000/api/health |
| Ollama API | http://localhost:11434 |
| Ollama WebUI | http://localhost:11434 (API only - no web UI) |
| MinIO Console | http://localhost:9001 |
| PostgreSQL | localhost:5432 |

### Production

**Bước 1: Chuẩn bị Server**

```bash
# Cài đặt Docker Engine
curl -fsSL https://get.docker.com | sh

# Cài đặt Docker Compose
apt-get install docker-compose
```

**Bước 2: Copy files lên server**

```bash
scp -r DoAnThacSi/docker user@server:/opt/aibaseframework/
```

**Bước 3: Configure SSL**

```bash
# Tạo thư mục ssl
mkdir -p /opt/aibaseframework/docker/ssl

# Copy certificates (hoặc sử dụng Let's Encrypt)
cp your-cert.crt /opt/aibaseframework/docker/ssl/certificate.crt
cp your-key.key /opt/aibaseframework/docker/ssl/private.key
```

**Bước 4: Khởi động Production**

```bash
cd /opt/aibaseframework/docker

# Chỉnh sửa .env với production values
nano .env

# Build và start
docker-compose -f docker-compose.prod.yml up -d --build

# Kiểm tra trạng thái
docker-compose -f docker-compose.prod.yml ps
```

---

## Services

### PostgreSQL với pgvector

```yaml
Ports: 5432
Volumes: postgres_data
Extensions: vector, uuid-ossp, pg_trgm
Healthcheck: pg_isready
```

**Kết nối từ Backend:**
```
Host: postgresql
Port: 5432
Database: aibaseframework
Username: aibfuser
Password: (từ .env)
```

### Redis

```yaml
Ports: 6379
Persistence: AOF enabled
Auth: Required
Healthcheck: redis-cli ping
```

**Kết nối từ Backend:**
```
Host: redis
Port: 6379
Password: (từ .env)
```

### MinIO Object Storage

```yaml
API Port: 9000
Console Port: 9001
Default Buckets: documents, uploads, embeddings
Healthcheck: MinIO health endpoint
```

**Truy cập Console:**
```
URL: http://localhost:9001
Access Key: minioadmin
Secret Key: (từ .env)
```

### Backend API (.NET 8)

```yaml
Ports: 5000 (HTTP), 5001 (HTTPS)
Healthcheck: /api/health endpoint
Non-root user: appuser (UID 1001)
Environment: Ollama endpoint configured for local LLM
```

**API Endpoints:**
- `GET /api/health` - Health check
- `POST /api/auth/login` - Authentication
- `GET /api/documents` - List documents
- `POST /api/documents/upload` - Upload document
- `GET /api/search` - Semantic search
- `POST /api/chat/ask` - RAG chatbot

### Ollama (LLM Local - THAY THẾ OpenAI)

```yaml
Ports: 11434 (API), 11436 (Metrics)
Volumes: ollama_data:/root/.ollama
GPU Support: NVIDIA CUDA (tự động detect)
Healthcheck: /api/tags endpoint
```

**Kết nối từ Backend:**
```
Host: ollama
Port: 11434
```

**Available Models:**
| Model | Type | Dimensions | VRAM | Use Case |
|-------|------|------------|------|----------|
| `llama3.2:3b` | Chat | - | 2GB | Fast, recommended |
| `mistral:7b` | Chat | - | 5GB | Better quality |
| `phi3:latest` | Chat | - | 2.5GB | Very fast |
| `qwen2.5:7b` | Chat | - | 5GB | Good Vietnamese |
| `nomic-embed-text` | Embedding | 768 | 1GB | Fast, recommended |
| `mxbai-embed-large` | Embedding | 1024 | 1.5GB | High quality |

**Test Ollama API:**
```bash
# List models
curl http://localhost:11434/api/tags

# Test embedding
curl -X POST http://localhost:11434/api/embeddings \
  -H "Content-Type: application/json" \
  -d '{"model": "nomic-embed-text", "prompt": "test"}'

# Test chat
curl -X POST http://localhost:11434/api/chat \
  -H "Content-Type: application/json" \
  -d '{"model": "llama3.2:3b", "messages": [{"role": "user", "content": "Hello"}]}'
```

### Frontend (React + Vite)

```yaml
Port: 3000 (dev) / 80 (prod)
Build: Multi-stage với Node 20
Runtime: Nginx alpine
```

### Seq Logging

```yaml
Ingestion Port: 5341
Web UI Port: 5340
Volume: seq_data
```

**Truy cập Web UI:**
```
URL: http://localhost:5340
```

---

## Environment Variables

### Database
```env
POSTGRES_USER=aibfuser
POSTGRES_PASSWORD=secure_password_here
POSTGRES_DB=aibaseframework
```

### Redis
```env
REDIS_PASSWORD=secure_password_here
```

### MinIO
```env
MINIO_ROOT_USER=minioadmin
MINIO_ROOT_PASSWORD=secure_password_here
MINIO_BUCKETS=documents,uploads,embeddings
```

### Ollama (LLM Local - THAY THẾ OpenAI)

```env
# Ollama Chat Model (LLM cho chatbot)
OLLAMA_CHAT_MODEL=llama3.2:3b

# Ollama Embedding Model
OLLAMA_EMBEDDING_MODEL=nomic-embed-text

# Ollama Configuration
OLLAMA_NUM_PARALLEL=4
OLLAMA_MAX_LOADED_MODELS=2
OLLAMA_TEMPERATURE=0.3
OLLAMA_MAX_TOKENS=2000

# GPU Configuration (set "" for CPU-only mode)
CUDA_VISIBLE_DEVICES=0

# OPENAI - KHÔNG SỬ DỤNG (Offline Mode)
# Để trống hoặc comment out để sử dụng Ollama
# OPENAI_API_KEY=
```

### JWT
```env
JWT_SECRET=your-256-bit-secret-minimum-32-characters
JWT_ISSUER=AIBaseFrameworkAPI
JWT_AUDIENCE=AIBaseFrameworkClient
JWT_EXPIRY_MINUTES=60
```

---

## Các lệnh Docker thường dùng

### Khởi động và Dừng

```powershell
# Development
docker-compose up -d              # Start all services
docker-compose up -d backend      # Start specific service
docker-compose down               # Stop and remove containers
docker-compose down -v            # Stop and remove containers + volumes

# Production
docker-compose -f docker-compose.prod.yml up -d
docker-compose -f docker-compose.prod.yml down
```

### Logs và Monitoring

```powershell
# View all logs
docker-compose logs -f

# View specific service
docker-compose logs -f backend
docker-compose logs --tail=100 postgresql

# Follow logs of multiple services
docker-compose logs -f backend frontend
```

### Rebuild

```powershell
# Rebuild without cache
docker-compose build --no-cache

# Rebuild specific service
docker-compose build backend
docker-compose up -d --build backend
```

### Debugging

```powershell
# Shell vào container
docker-compose exec backend sh
docker-compose exec postgresql psql -U aibfuser -d aibaseframework
docker-compose exec redis redis-cli -a your_password

# Xem resource usage
docker stats

# Xem container details
docker-compose ps
```

### Database Operations

```powershell
# Backup database
docker-compose exec postgresql pg_dump -U aibfuser aibaseframework > backup.sql

# Restore database
docker-compose exec -T postgresql psql -U aibfuser -d aibaseframework < backup.sql

# Connect to PostgreSQL
docker-compose exec postgresql psql -U aibfuser -d aibaseframework
```

### Clean up

```powershell
# Remove unused images
docker image prune -a

# Remove unused volumes
docker volume prune

# Remove all stopped containers
docker container prune

# Full cleanup
docker system prune -a --volumes
```

---

## Kiểm tra và Troubleshooting

### Health Checks

```powershell
# Kiểm tra tất cả services
docker-compose ps

# Kiểm tra health status
docker inspect --format='{{.State.Health.Status}}' aibf_backend
docker inspect --format='{{.State.Health.Status}}' aibf_postgresql
docker inspect --format='{{.State.Health.Status}}' aibf_ollama
```

### Ollama Issues

**1. Ollama container không start (GPU not detected)**
```powershell
# Kiểm tra NVIDIA GPU
nvidia-smi

# Kiểm tra CUDA
nvcc --version

# Chạy ở CPU-only mode (nếu không có GPU)
# Chỉnh sửa docker-compose.yml:
# Đổi CUDA_VISIBLE_DEVICES="" (CPU only)
# Hoặc xóa phần GPU configuration
```

**2. Model chưa được download**
```powershell
# Download model
docker exec aibf_ollama ollama pull llama3.2:3b
docker exec aibf_ollama ollama pull nomic-embed-text

# List downloaded models
docker exec aibf_ollama ollama list
```

**3. Slow inference (CPU mode)**
```powershell
# Đây là bình thường khi chạy CPU-only
# Khuyến nghị: Sử dụng model nhỏ hơn
docker exec aibf_ollama ollama pull llama3.2:1b
# Sau đó cập nhật OLLAMA_CHAT_MODEL=llama3.2:1b trong .env
```

**4. Out of Memory (VRAM)**
```powershell
# Sử dụng model nhỏ hơn
docker exec aibf_ollama ollama pull llama3.2:1b
docker exec aibf_ollama ollama rm llama3.2:3b

# Cập nhật .env:
# OLLAMA_CHAT_MODEL=llama3.2:1b
# Restart backend
docker-compose restart backend
```

### Common Issues

**1. Port đã được sử dụng**
```powershell
# Tìm process sử dụng port
netstat -ano | findstr :5000

# Kill process hoặc đổi port trong docker-compose.yml
```

**2. Volume permissions**
```powershell
# Reset volumes
docker-compose down -v
docker-compose up -d
```

**3. Database connection failed**
```powershell
# Kiểm tra logs
docker-compose logs postgresql

# Reset database
docker-compose down -v
docker volume rm aibf_postgres_data
docker-compose up -d
```

**4. Out of memory**
```powershell
# Tăng memory cho Docker Desktop
# Settings > Resources > Memory: 8GB+
```

### Testing Endpoints

```bash
# Health check
curl http://localhost:5000/api/health

# API docs (Swagger)
curl http://localhost:5000/swagger

# Test search (requires auth)
curl -H "Authorization: Bearer <token>" \
     http://localhost:5000/api/search?q=performance
```

---

## Security

### Production Checklist

- [ ] Đổi tất cả default passwords
- [ ] Cấu hình SSL/TLS certificates
- [ ] Enable firewall (chỉ mở port 80, 443)
- [ ] Không expose database ports ra ngoài
- [ ] Sử dụng secrets management
- [ ] Regular security updates
- [ ] Backup automation

### Non-root Users

Tất cả containers đều chạy với non-root users:
- Backend: `appuser` (UID 1001)
- Frontend: `nginx` (UID 101)

### Network Isolation

Production network sử dụng custom subnet:
```
172.28.0.0/16
```

Chỉ Nginx được expose ra external network.

---

## Backup và Restore

### Automated Backup Script

```bash
#!/bin/bash
# backup.sh - Chạy hàng ngày qua cron

BACKUP_DIR="/backups"
DATE=$(date +%Y%m%d_%H%M%S)

# Backup PostgreSQL
docker-compose exec -T postgresql pg_dump -U aibfuser aibaseframework > $BACKUP_DIR/db_$DATE.sql

# Backup MinIO data
docker-compose exec minio mc mirror local/ $BACKUP_DIR/minio_$DATE/

# Gzip backups
gzip $BACKUP_DIR/db_$DATE.sql

# Keep only last 7 days
find $BACKUP_DIR -mtime +7 -delete
```

### Restore

```bash
# Stop services
docker-compose down

# Restore PostgreSQL
gunzip < backup.sql.gz | docker-compose exec -T postgresql psql -U aibfuser -d aibaseframework

# Restore MinIO
docker-compose exec minio mc mirror backup/ local/

# Start services
docker-compose up -d
```

---

## License

Đề tài Thạc sĩ - Trường Đại học Hồng Liên Viên
