#!/bin/bash
# ==============================================================================
# SCRIPT DOWNLOAD OLLAMA MODELS
# Đề tài Thạc sĩ: Hệ thống Tìm kiếm và Hỏi đáp Tài liệu Nội bộ
# ==============================================================================
#
# Script này download các LLM và embedding models cho Ollama
# Chạy script này sau khi docker-compose đã start Ollama container
#
# Cách sử dụng:
#   ./download-models.sh          # Download recommended models
#   ./download-models.sh --all    # Download all models
#   ./download-models.sh --cpu   # Download models for CPU-only mode
#
# ==============================================================================

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
OLLAMA_HOST="${OLLAMA_HOST:-http://localhost:11434}"
OLLAMA_CONTAINER="${OLLAMA_CONTAINER:-hlv_ollama}"

echo "=========================================="
echo "  OLLAMA MODELS DOWNLOADER"
echo "  Host: $OLLAMA_HOST"
echo "=========================================="
echo ""

# Check if Ollama is running
check_ollama() {
    echo -e "${BLUE}[1/5] Checking Ollama service...${NC}"
    
    if docker ps --format '{{.Names}}' | grep -q "^${OLLAMA_CONTAINER}$"; then
        echo -e "${GREEN}✓${NC} Ollama container is running"
        return 0
    else
        echo -e "${YELLOW}⚠${NC} Ollama container not found. Starting inline check..."
        
        # Try direct connection
        if curl -s --connect-timeout 5 "$OLLAMA_HOST/api/tags" > /dev/null 2>&1; then
            echo -e "${GREEN}✓${NC} Ollama service is accessible at $OLLAMA_HOST"
            USE_CONTAINER=false
            return 0
        fi
        
        echo -e "${RED}✗${NC} Ollama service is not running!"
        echo "Please start Ollama first:"
        echo "  docker-compose up -d ollama"
        return 1
    fi
}

# Function to pull a model
pull_model() {
    local model=$1
    local description=$2
    local size=$3
    
    echo ""
    echo -e "${BLUE}Downloading ${model}...${NC}"
    echo "  Description: $description"
    echo "  Estimated size: $size"
    
    if [ "$USE_CONTAINER" = true ]; then
        docker exec "$OLLAMA_CONTAINER" ollama pull "$model"
    else
        curl -X POST "$OLLAMA_HOST/api/pull" -d "{\"name\": \"$model\"}"
    fi
    
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✓${NC} $model downloaded successfully"
    else
        echo -e "${RED}✗${NC} Failed to download $model"
    fi
}

# Download recommended models (default)
download_recommended() {
    echo ""
    echo -e "${GREEN}[2/5] Downloading Recommended Models${NC}"
    echo "=========================================="
    
    # Chat Models
    echo ""
    echo "--- Chat Models ---"
    
    pull_model "llama3.2:3b" \
        "3B params, fast, balanced quality, 2GB VRAM" \
        "~2GB"
    
    pull_model "phi3:latest" \
        "3.8B params, very fast, good quality, 2.5GB VRAM" \
        "~2.5GB"
    
    # Embedding Models
    echo ""
    echo "--- Embedding Models ---"
    
    pull_model "nomic-embed-text" \
        "768 dimensions, fast embedding, 1GB VRAM" \
        "~1GB"
}

# Download all models
download_all() {
    echo ""
    echo -e "${GREEN}[2/5] Downloading All Models (Full Set)${NC}"
    echo "=========================================="
    
    # Chat Models
    echo ""
    echo "--- Chat Models ---"
    
    pull_model "llama3.2:3b" \
        "3B params, fast, balanced quality" \
        "~2GB"
    
    pull_model "llama3.2:1b" \
        "1B params, very fast, low memory" \
        "~1.3GB"
    
    pull_model "mistral:7b" \
        "7B params, better quality, 5GB VRAM" \
        "~4.1GB"
    
    pull_model "phi3:latest" \
        "3.8B params, fast, good quality" \
        "~2.5GB"
    
    pull_model "qwen2.5:7b" \
        "7B params, excellent Vietnamese support, 5GB VRAM" \
        "~4.4GB"
    
    # Embedding Models
    echo ""
    echo "--- Embedding Models ---"
    
    pull_model "nomic-embed-text" \
        "768 dimensions, fast, recommended" \
        "~1GB"
    
    pull_model "mxbai-embed-large" \
        "1024 dimensions, high quality embedding" \
        "~1.3GB"
}

# Download CPU-optimized models
download_cpu_only() {
    echo ""
    echo -e "${GREEN}[2/5] Downloading CPU-Optimized Models${NC}"
    echo "=========================================="
    echo "Models selected for CPU inference (slower but uses less memory)"
    
    # Chat Models (smaller, faster)
    echo ""
    echo "--- Chat Models ---"
    
    pull_model "llama3.2:1b" \
        "1B params, very fast on CPU" \
        "~1.3GB"
    
    pull_model "phi3:latest" \
        "3.8B params, optimized for CPU inference" \
        "~2.5GB"
    
    # Embedding Models
    echo ""
    echo "--- Embedding Models ---"
    
    pull_model "nomic-embed-text" \
        "768 dimensions, fast embedding, CPU-friendly" \
        "~1GB"
}

# List downloaded models
list_models() {
    echo ""
    echo -e "${GREEN}[3/5] Listing Downloaded Models${NC}"
    echo "=========================================="
    
    echo ""
    if [ "$USE_CONTAINER" = true ]; then
        docker exec "$OLLAMA_CONTAINER" ollama list
    else
        curl -s "$OLLAMA_HOST/api/tags" | python3 -m json.tool 2>/dev/null || curl -s "$OLLAMA_HOST/api/tags"
    fi
}

# Verify installation
verify_installation() {
    echo ""
    echo -e "${GREEN}[4/5] Verifying Installation${NC}"
    echo "=========================================="
    
    echo ""
    echo "Testing embedding generation..."
    if [ "$USE_CONTAINER" = true ]; then
        docker exec "$OLLAMA_CONTAINER" ollama embed -m nomic-embed-text "test"
    else
        curl -s -X POST "$OLLAMA_HOST/api/embeddings" \
            -H "Content-Type: application/json" \
            -d '{"model":"nomic-embed-text","prompt":"test"}' | head -c 100
    fi
    
    echo ""
    echo ""
    echo -e "${GREEN}✓${NC} Embedding test completed"
    
    echo ""
    echo "Testing chat generation (fast test)..."
    if [ "$USE_CONTAINER" = true ]; then
        docker exec "$OLLAMA_CONTAINER" ollama run llama3.2:3b "Hello" --verbose 2>&1 | head -5
    else
        curl -s -X POST "$OLLAMA_HOST/api/chat" \
            -H "Content-Type: application/json" \
            -d '{"model":"llama3.2:3b","messages":[{"role":"user","content":"Hi"}],"stream":false}' | head -c 200
    fi
    
    echo ""
    echo ""
    echo -e "${GREEN}✓${NC} Chat test completed"
}

# Print summary
print_summary() {
    echo ""
    echo "=========================================="
    echo -e "${GREEN}[5/5] Summary${NC}"
    echo "=========================================="
    echo ""
    echo -e "Download complete!"
    echo ""
    echo "Next steps:"
    echo "  1. Start all services: ${YELLOW}docker-compose up -d${NC}"
    echo "  2. Check status: ${YELLOW}docker-compose ps${NC}"
    echo "  3. View logs: ${YELLOW}docker-compose logs -f backend${NC}"
    echo "  4. Access app: ${YELLOW}http://localhost:3000${NC}"
    echo ""
    echo "Available models:"
    echo "  Chat: llama3.2:3b, phi3:latest, mistral:7b, qwen2.5:7b"
    echo "  Embedding: nomic-embed-text, mxbai-embed-large"
    echo ""
    echo "To switch models, update ${YELLOW}OLLAMA_CHAT_MODEL${NC} and ${YELLOW}OLLAMA_EMBEDDING_MODEL${NC}"
    echo "in your .env file and restart the backend."
    echo ""
    echo "=========================================="
}

# Main script
main() {
    USE_CONTAINER=true
    
    # Parse arguments
    case "${1:-}" in
        --all)
            MODE="all"
            ;;
        --cpu)
            MODE="cpu"
            ;;
        --recommended)
            MODE="recommended"
            ;;
        --help|-h)
            echo "Usage: $0 [--all|--cpu|--recommended|--help]"
            echo ""
            echo "Options:"
            echo "  --all         Download all available models"
            echo "  --cpu         Download CPU-optimized models (smaller)"
            echo "  --recommended Download recommended default models"
            echo "  --help        Show this help message"
            echo ""
            echo "Default: Download recommended models"
            exit 0
            ;;
        *)
            MODE="recommended"
            ;;
    esac
    
    # Check Ollama
    if ! check_ollama; then
        exit 1
    fi
    
    # Download based on mode
    case "$MODE" in
        all)
            download_all
            ;;
        cpu)
            download_cpu_only
            ;;
        recommended)
            download_recommended
            ;;
    esac
    
    # List and verify
    list_models
    verify_installation
    print_summary
}

# Run main
main "$@"
