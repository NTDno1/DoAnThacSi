#!/bin/bash
# ============================================
# Setup Script - Khoi tao moi truong phat trien
# AI Base Framework - Framework tich hop AI cho moi he thong
# ============================================

set -e

echo "============================================"
echo "  AI BASE FRAMEWORK - SETUP SCRIPT"
echo "  Khoi tao moi truong phat trien"
echo "============================================"
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Functions
info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check prerequisites
check_prerequisites() {
    echo ""
    echo "Kiểm tra prerequisites..."

    # Check Docker
    if ! command -v docker &> /dev/null; then
        error "Docker chưa được cài đặt. Vui lòng cài đặt Docker Desktop."
        exit 1
    fi
    success "Docker: OK"

    # Check Docker Compose
    if ! command -v docker-compose &> /dev/null && ! docker compose version &> /dev/null; then
        error "Docker Compose chưa được cài đặt."
        exit 1
    fi
    success "Docker Compose: OK"

    # Check .NET SDK
    if ! command -v dotnet &> /dev/null; then
        warning ".NET SDK chưa được cài đặt. Bạn có thể phát triển backend sau."
    else
        dotnet_version=$(dotnet --version)
        success ".NET SDK: $dotnet_version"
    fi

    # Check Node.js
    if ! command -v node &> /dev/null; then
        warning "Node.js chưa được cài đặt. Bạn có thể phát triển frontend sau."
    else
        node_version=$(node --version)
        success "Node.js: $node_version"
    fi
}

# Setup environment file
setup_env() {
    echo ""
    echo "Thiết lập file .env..."

    if [ -f .env ]; then
        warning "File .env đã tồn tại. Bỏ qua."
    else
        if [ -f .env.example ]; then
            cp .env.example .env
            success "Đã tạo file .env từ .env.example"
            info "Vui lòng chỉnh sửa file .env và thay đổi các password mặc định!"
        else
            error "Không tìm thấy file .env.example"
            exit 1
        fi
    fi
}

# Create necessary directories
create_directories() {
    echo ""
    echo "Tạo các thư mục cần thiết..."

    mkdir -p docker/volumes/postgres
    mkdir -p docker/volumes/redis
    mkdir -p docker/volumes/minio
    mkdir -p docker/volumes/ollama
    mkdir -p docker/volumes/seq
    mkdir -p docker/backups

    success "Đã tạo các thư mục volumes"
}

# Pull Docker images
pull_images() {
    echo ""
    echo "Pulling Docker images..."

    docker pull postgres:16 || warning "Không thể pull postgres:16"
    docker pull pgvector/pgvector:pg16 || warning "Không thể pull pgvector"
    docker pull redis:7-alpine || warning "Không thể pull redis"
    docker pull minio/minio:latest || warning "Không thể pull minio"
    docker pull ollama/ollama:latest || warning "Không thể pull ollama"
    docker pull datalust/seq:latest || warning "Không thể pull seq"

    success "Đã pull Docker images"
}

# Start infrastructure services
start_infrastructure() {
    echo ""
    echo "Khởi động infrastructure services..."

    docker-compose -f docker/docker-compose.yml up -d postgresql redis minio

    info "Đang chờ PostgreSQL sẵn sàng..."
    sleep 10

    # Check PostgreSQL
    for i in {1..30}; do
        if docker-compose -f docker/docker-compose.yml exec -T postgresql pg_isready -U aibfuser -d aibaseframework &> /dev/null; then
            success "PostgreSQL đã sẵn sàng"
            break
        fi
        if [ $i -eq 30 ]; then
            error "PostgreSQL không sẵn sàng sau 30 giây"
            exit 1
        fi
        sleep 1
    done

    success "Infrastructure services đã khởi động"
}

# Run database migrations
run_migrations() {
    echo ""
    echo "Chạy database migrations..."

    if command -v dotnet &> /dev/null; then
        cd src/AIBaseFramework.API
        dotnet ef database update || warning "Không thể chạy migrations. Đảm bảo backend đã được build."
        cd ../..
        success "Đã chạy database migrations"
    else
        warning ".NET SDK không được cài đặt. Bỏ qua migrations."
    fi
}

# Download Ollama models
download_models() {
    echo ""
    echo "Downloading Ollama models..."

    # Start Ollama
    docker-compose -f docker/docker-compose.yml up -d ollama

    info "Đang chờ Ollama sẵn sàng..."
    sleep 10

    # Check Ollama
    for i in {1..30}; do
        if curl -s http://localhost:11434/api/tags &> /dev/null; then
            success "Ollama đã sẵn sàng"
            break
        fi
        if [ $i -eq 30 ]; then
            error "Ollama không sẵn sàng sau 30 giây"
            exit 1
        fi
        sleep 1
    done

    # Download models
    echo ""
    echo "Downloading chat model (llama3.2:3b)..."
    docker exec aibf_ollama ollama pull llama3.2:3b || warning "Khong the download llama3.2:3b"

    echo ""
    echo "Downloading embedding model (nomic-embed-text)..."
    docker exec aibf_ollama ollama pull nomic-embed-text || warning "Khong the download nomic-embed-text"

    success "Đã download Ollama models"
}

# Main
main() {
    echo ""
    echo "============================================"
    echo "  AI BASE FRAMEWORK - SETUP"
    echo "============================================"
    echo ""

    # Check if running from correct directory
    if [ ! -f "docker/docker-compose.yml" ]; then
        error "Vui lòng chạy script từ thư mục gốc của project"
        exit 1
    fi

    check_prerequisites
    setup_env
    create_directories
    pull_images
    start_infrastructure
    download_models

    echo ""
    echo "============================================"
    echo "  SETUP HOÀN TẤT!"
    echo "============================================"
    echo ""
    echo "Các services đã được khởi động:"
    echo "  - PostgreSQL: localhost:5432"
    echo "  - Redis: localhost:6379"
    echo "  - MinIO: localhost:9000 (Console: localhost:9001)"
    echo "  - Ollama API: localhost:11434"
    echo ""
    echo "Để xem logs: docker-compose -f docker/docker-compose.yml logs -f"
    echo "Để dừng: docker-compose -f docker/docker-compose.yml down"
    echo ""
}

main "$@"
