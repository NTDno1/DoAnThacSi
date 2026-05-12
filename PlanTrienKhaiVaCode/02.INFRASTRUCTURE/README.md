# INFRASTRUCTURE - Cơ sở hạ tầng Docker

## Cấu trúc

```
02.INFRASTRUCTURE/
├── scripts/                    # Shell scripts tự động hóa
│   ├── setup.sh               # Script setup ban đầu
│   ├── download-models.sh       # Download Ollama models
│   └── health-check.sh         # Kiểm tra health của services
│
├── docker/                    # Docker configurations
│   ├── docker-compose.yml      # Development compose
│   ├── docker-compose.prod.yml # Production compose
│   ├── Dockerfile.backend      # Backend .NET 8
│   ├── Dockerfile.frontend     # Frontend Next.js
│   ├── Dockerfile.ollama       # Ollama LLM
│   ├── nginx.conf             # Nginx configuration
│   ├── init.sql               # Database initialization
│   └── .env.example           # Environment template
│
└── README.md                 # This file
```

## Quick Start

### 1. Setup môi trường

```bash
cd 02.INFRASTRUCTURE
chmod +x scripts/*.sh
./scripts/setup.sh
```

### 2. Download Ollama models

```bash
./scripts/download-models.sh
# Chọn option 1: Download recommended models
```

### 3. Kiểm tra health

```bash
./scripts/health-check.sh
```

## Services

| Service | Port | Mô tả |
|---------|------|--------|
| PostgreSQL | 5432 | Database + pgvector |
| Redis | 6379 | Cache & Session |
| MinIO | 9000, 9001 | Object Storage |
| Ollama | 11434 | Local LLM |
| Backend | 5000 | .NET 8 API |
| Frontend | 3000 | Next.js App |
| Seq | 5340 | Log Aggregation |

## Commands

```bash
# Start all services
docker-compose -f docker/docker-compose.yml up -d

# Stop all services
docker-compose -f docker/docker-compose.yml down

# View logs
docker-compose -f docker/docker-compose.yml logs -f

# Restart a service
docker-compose -f docker/docker-compose.yml restart backend

# Shell into a container
docker-compose -f docker/docker-compose.yml exec backend sh
```

## Environment Variables

Xem `.env.example` để biết các biến môi trường cần thiết.

**Lưu ý quan trọng**:
- Thay đổi tất cả passwords trước khi deploy production
- Ollama models cần được download sau khi container start

## Troubleshooting

### Ollama không start
- Kiểm tra GPU: `nvidia-smi`
- Chạy CPU-only: đặt `CUDA_VISIBLE_DEVICES=""` trong .env

### PostgreSQL không ready
- Chờ 30 giây sau khi start
- Kiểm tra logs: `docker-compose logs postgresql`

### Models chưa có
```bash
docker exec aibf_ollama ollama pull llama3.2:3b
docker exec aibf_ollama ollama pull nomic-embed-text
```
