#!/bin/bash
# ============================================
# Download Ollama Models Script
# Hệ thống Semantic Search + RAG Chatbot
# ============================================

set -e

echo "============================================"
echo "  DOWNLOAD OLLAMA MODELS"
echo "============================================"
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Functions
info() { echo -e "${BLUE}[INFO]${NC} $1"; }
success() { echo -e "${GREEN}[SUCCESS]${NC} $1"; }
warning() { echo -e "${YELLOW}[WARNING]${NC} $1"; }
error() { echo -e "${RED}[ERROR]${NC} $1"; }

# Check if Ollama container is running
check_ollama() {
    if docker ps | grep -q hlv_ollama; then
        info "Ollama container đang chạy"
        return 0
    else
        error "Ollama container không chạy. Vui lòng chạy: docker-compose up -d ollama"
        return 1
    fi
}

# Check Ollama API
check_api() {
    info "Kiểm tra Ollama API..."
    if curl -s http://localhost:11434/api/tags &> /dev/null; then
        success "Ollama API đang hoạt động"
        return 0
    else
        error "Ollama API không phản hồi. Đang chờ..."
        return 1
    fi
}

# Pull a model
pull_model() {
    local model=$1
    local size=$2

    info "Downloading $model ($size)..."
    if docker exec hlv_ollama ollama pull "$model" 2>&1 | tee /dev/stderr | grep -q "success"; then
        success "Đã download $model"
    elif docker exec hlv_ollama ollama pull "$model" &> /dev/null; then
        success "Đã download $model"
    else
        warning "Không thể download $model. Có thể cần GPU hoặc kết nối internet."
        return 1
    fi
}

# List installed models
list_models() {
    info "Các models đã được cài đặt:"
    echo ""
    docker exec hlv_ollama ollama list
    echo ""
}

# Download recommended models
download_recommended() {
    echo ""
    echo "Downloading RECOMMENDED models..."
    echo ""

    # Chat model (recommended: llama3.2:3b)
    pull_model "llama3.2:3b" "~2GB"

    # Embedding model
    pull_model "nomic-embed-text" "~274MB"
}

# Download all models
download_all() {
    echo ""
    echo "Downloading ALL models (sẽ tốn nhiều thời gian và dung lượng)..."
    echo ""

    # Chat models
    pull_model "llama3.2:1b" "~1.3GB"
    pull_model "llama3.2:3b" "~2GB"
    pull_model "mistral:7b" "~4GB"
    pull_model "phi3:latest" "~2.2GB"
    pull_model "qwen2.5:7b" "~4.4GB"

    # Embedding models
    pull_model "nomic-embed-text" "~274MB"
    pull_model "mxbai-embed-large" "~670MB"
}

# Download CPU-optimized models
download_cpu_only() {
    echo ""
    echo "Downloading CPU-OPTIMIZED models (không cần GPU)..."
    echo ""

    # Small chat model
    pull_model "llama3.2:1b" "~1.3GB"

    # Embedding model
    pull_model "nomic-embed-text" "~274MB"
}

# Main menu
show_menu() {
    echo ""
    echo "============================================"
    echo "  OLLAMA MODEL DOWNLOADER"
    echo "============================================"
    echo ""
    echo "Chọn option:"
    echo "  1) Download RECOMMENDED models (llama3.2:3b + nomic-embed)"
    echo "  2) Download ALL models (tất cả các models)"
    echo "  3) Download CPU-ONLY models (cho máy không có GPU)"
    echo "  4) List installed models"
    echo "  5) Verify installation"
    echo "  0) Exit"
    echo ""
    read -p "Nhập lựa chọn: " choice
}

# Verify installation
verify_installation() {
    echo ""
    info "Verifying Ollama installation..."

    # Check API
    echo ""
    echo "=== Ollama API Status ==="
    curl -s http://localhost:11434/api/tags | head -20 || echo "API không phản hồi"

    # List models
    echo ""
    echo "=== Installed Models ==="
    docker exec hlv_ollama ollama list

    # Check disk usage
    echo ""
    echo "=== Disk Usage ==="
    docker exec hlv_ollama du -sh /root/.ollama/models 2>/dev/null || echo "Không thể đọc disk usage"
}

# Print summary
print_summary() {
    echo ""
    echo "============================================"
    echo "  SUMMARY"
    echo "============================================"
    echo ""
    echo "Models đã download:"
    docker exec hlv_ollama ollama list
    echo ""
    info "Để sử dụng models trong backend, cập nhật .env:"
    echo "  OLLAMA_CHAT_MODEL=llama3.2:3b"
    echo "  OLLAMA_EMBEDDING_MODEL=nomic-embed-text"
    echo ""
    info "Sau đó restart backend: docker-compose restart backend"
    echo ""
}

# Main
main() {
    # Check prerequisites
    check_ollama || exit 1
    check_api || exit 1

    # Parse arguments
    if [ "$1" == "recommended" ]; then
        download_recommended
        print_summary
        exit 0
    elif [ "$1" == "all" ]; then
        download_all
        print_summary
        exit 0
    elif [ "$1" == "cpu" ]; then
        download_cpu_only
        print_summary
        exit 0
    elif [ "$1" == "list" ]; then
        list_models
        exit 0
    elif [ "$1" == "verify" ]; then
        verify_installation
        exit 0
    fi

    # Interactive mode
    while true; do
        show_menu

        case $choice in
            1) download_recommended; print_summary; break ;;
            2) download_all; print_summary; break ;;
            3) download_cpu_only; print_summary; break ;;
            4) list_models ;;
            5) verify_installation ;;
            0) echo "Exit."; exit 0 ;;
            *) warning "Lựa chọn không hợp lệ" ;;
        esac
    done
}

main "$@"
