#!/bin/bash
# ============================================
# Health Check Script
# AI Base Framework - Framework tich hop AI cho moi he thong
# ============================================

set -e

echo "============================================"
echo "  HEALTH CHECK - AI BASE FRAMEWORK"
echo "============================================"
echo ""

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

# Functions
ok() { echo -e "${GREEN}[OK]${NC} $1"; }
fail() { echo -e "${RED}[FAIL]${NC} $1"; }
warn() { echo -e "${YELLOW}[WARN]${NC} $1"; }
info() { echo -e "${BLUE}[INFO]${NC} $1"; }

# Counters
PASS=0
FAIL=0
WARN_COUNT=0

# Check a service
check_service() {
    local name=$1
    local url=$2
    local expected=${3:-200}

    info "Checking $name..."

    response=$(curl -s -o /dev/null -w "%{http_code}" --max-time 10 "$url" 2>/dev/null || echo "000")

    if [ "$response" == "$expected" ]; then
        ok "$name is healthy (HTTP $response)"
        ((PASS++))
        return 0
    elif [ "$response" == "000" ]; then
        fail "$name is not reachable"
        ((FAIL++))
        return 1
    else
        warn "$name returned HTTP $response (expected $expected)"
        ((WARN_COUNT++))
        return 1
    fi
}

# Check Docker container
check_container() {
    local name=$1

    info "Checking container: $name..."

    if docker ps --format '{{.Names}}' | grep -q "^${name}$"; then
        local status=$(docker inspect --format='{{.State.Status}}' "$name" 2>/dev/null)
        local health=$(docker inspect --format='{{.State.Health.Status}}' "$name" 2>/dev/null)

        if [ "$health" == "healthy" ]; then
            ok "Container $name is running and healthy"
            ((PASS++))
        elif [ "$status" == "running" ]; then
            warn "Container $name is running but not healthy"
            ((WARN_COUNT++))
        else
            fail "Container $name is not running (status: $status)"
            ((FAIL++))
        fi
    else
        fail "Container $name not found"
        ((FAIL++))
    fi
}

# Check PostgreSQL
check_postgres() {
    info "Checking PostgreSQL..."

    if docker-compose -f docker/docker-compose.yml exec -T postgresql pg_isready -U aibfuser -d aibaseframework &> /dev/null; then
        ok "PostgreSQL is ready"
        ((PASS++))
    else
        fail "PostgreSQL is not ready"
        ((FAIL++))
    fi
}

# Check Redis
check_redis() {
    info "Checking Redis..."

    if docker exec aibf_redis redis-cli -a redis123 ping &> /dev/null; then
        ok "Redis is responding"
        ((PASS++))
    else
        # Try without password
        if docker exec aibf_redis redis-cli ping &> /dev/null; then
            ok "Redis is responding"
            ((PASS++))
        else
            fail "Redis is not responding"
            ((FAIL++))
        fi
    fi
}

# Check Ollama models
check_ollama_models() {
    info "Checking Ollama models..."

    models=$(curl -s http://localhost:11434/api/tags 2>/dev/null | grep -o '"name":"[^"]*"' | wc -l)

    if [ "$models" -gt 0 ]; then
        ok "Ollama has $models model(s) installed"
        ((PASS++))
        echo ""
        info "Installed models:"
        curl -s http://localhost:11434/api/tags 2>/dev/null | grep -o '"name":"[^"]*"' | sed 's/"name":"//g' | sed 's/"//g' | sed 's/^/  - /g'
    else
        warn "Ollama has no models installed"
        ((WARN_COUNT++))
    fi
}

# Main
echo ""
echo "=== Container Status ==="
check_container "aibf_postgresql"
check_container "aibf_redis"
check_container "aibf_minio"
check_container "aibf_ollama"
check_container "aibf_backend"
check_container "aibf_frontend"
check_container "aibf_seq"

echo ""
echo "=== Service Health Checks ==="
check_service "Backend API" "http://localhost:5000/health"
check_service "Frontend" "http://localhost:3000"
check_service "Ollama API" "http://localhost:11434/api/tags"
check_service "MinIO Console" "http://localhost:9001/minio/health/live"
check_service "Seq UI" "http://localhost:5340"

echo ""
echo "=== Database Checks ==="
check_postgres
check_redis

echo ""
echo "=== AI Services ==="
check_ollama_models

echo ""
echo "============================================"
echo "  SUMMARY"
echo "============================================"
echo ""
echo -e "  ${GREEN}PASSED${NC}: $PASS"
echo -e "  ${YELLOW}WARNINGS${NC}: $WARN_COUNT"
echo -e "  ${RED}FAILED${NC}: $FAIL"
echo ""

if [ $FAIL -eq 0 ]; then
    echo -e "${GREEN}All critical checks passed!${NC}"
    exit 0
else
    echo -e "${RED}Some checks failed. Please review the output above.${NC}"
    exit 1
fi
