# AI Base Framework - Enterprise AI Platform

## Overview

AI Base Framework là một nền tảng AI doanh nghiệp được thiết kế để tích hợp các khả năng AI vào mọi hệ thống (ERP, DMS, CRM, HRM). Nền tảng này cung cấp các tính năng AI sẵn sàng sử dụng như Semantic Search, RAG Chatbot, AI Agent, Voice AI, và nhiều hơn nữa.

## Key Features

### 🔍 Semantic Search
- **Vector Search**: Tìm kiếm ngữ nghĩa với độ chính xác cao
- **Hybrid Search**: Kết hợp tìm kiếm vector và keyword
- **RAG Pipeline**: Retrieval Augmented Generation cho câu trả lời chính xác

### 💬 RAG Chatbot
- Trả lời câu hỏi dựa trên tài liệu được upload
- Trích dẫn nguồn tài liệu gốc
- Hỗ trợ đa ngôn ngữ (Vietnamese, English)

### 🤖 AI Agent
- Orchestration engine cho multi-step tasks
- Tool calling và function execution
- Safety guardrails cho enterprise security
- Human-in-the-loop approval

### 🗄️ Natural Language to SQL
- Chuyển đổi câu hỏi tự nhiên thành SQL queries
- An toàn với SQL injection protection
- Chỉ cho phép SELECT queries

### 🎤 Voice AI
- Speech-to-Text với Whisper
- Text-to-Speech
- Voice command recognition

### 🔌 Integration
- **MCP (Model Context Protocol)**: Giao tiếp AI-native với external systems
- REST API & gRPC
- WebSocket cho real-time communication

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        Frontend (Next.js)                         │
│  Dashboard │ Search │ Chat │ Documents │ Agent │ Admin          │
└────────────────────────────┬────────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────────┐
│                    API Gateway Layer                              │
│  Auth │ Documents │ Chat │ Search │ Agent │ Voice │ SQL         │
└────────────────────────────┬────────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────────┐
│                  AI Orchestration Layer                           │
│  Agent Orchestrator │ Tool Registry │ Safety Guard              │
└────────────────────────────┬────────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────────┐
│                    AI Engine Core                                │
│  RAG Pipeline │ Search Engine │ SQL Engine │ Voice Service      │
└────────────────────────────┬────────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────────┐
│                Provider Abstraction Layer                         │
│         Ollama (Local) │ OpenAI (Cloud) │ Custom                │
└────────────────────────────┬────────────────────────────────────┘
                             │
┌────────────────────────────▼────────────────────────────────────┐
│                    Infrastructure                                 │
│  PostgreSQL + pgvector │ Redis │ MinIO │ Ollama                 │
└─────────────────────────────────────────────────────────────────┘
```

## Tech Stack

### Backend
- **.NET 8** - ASP.NET Core Web API
- **Entity Framework Core** - Database ORM
- **PostgreSQL 16** with **pgvector** - Vector database
- **Redis** - Caching & Session Store
- **MinIO** - S3-compatible object storage
- **Serilog** - Structured logging

### AI Providers
- **Ollama** - Local LLM (llama3.2:3b)
- **nomic-embed-text** - Embedding model
- **OpenAI** - Cloud LLM (optional)

### Frontend
- **Next.js 14** - React framework
- **React 18** - UI library
- **TailwindCSS 3.4** - Styling
- **Zustand** - State management
- **Axios** - HTTP client

### Infrastructure
- **Docker** - Containerization
- **Nginx** - Reverse proxy
- **Seq** - Log aggregation

## Getting Started

### Prerequisites

- Docker & Docker Compose
- .NET 8 SDK
- Node.js 18+
- Git

### Quick Start

1. **Clone repository**
```bash
git clone <repository-url>
cd DoAnThacSi
```

2. **Setup environment**
```bash
cd PlanTrienKhaiVaCode/02.INFRASTRUCTURE
cp docker/.env.example docker/.env
```

3. **Start services**
```bash
cd docker
docker-compose up -d
```

4. **Access the application**
- Frontend: http://localhost:3000
- Backend API: http://localhost:5000
- Swagger: http://localhost:5000/swagger
- MinIO Console: http://localhost:9001

### Download AI Models

```bash
docker exec -it aibf-ollama ollama pull llama3.2:3b
docker exec -it aibf-ollama ollama pull nomic-embed-text
docker exec -it aibf-ollama ollama pull whisper
```

## Project Structure

```
PlanTrienKhaiVaCode/
├── 01.DEPLOYMENT_PLAN/          # Deployment roadmap
├── 02.INFRASTRUCTURE/           # Docker, scripts, configs
│   ├── docker/                  # Docker Compose files
│   └── scripts/                 # Database & utility scripts
├── 03.BACKEND/                  # .NET Backend
│   └── src/
│       └── AIBaseFramework.API/
│           ├── AI/              # AI components
│           │   ├── Agent/       # Agent orchestration
│           │   ├── Gateway/     # AI gateway
│           │   ├── MCP/        # Model Context Protocol
│           │   ├── Providers/   # LLM providers
│           │   ├── RAG/        # RAG pipeline
│           │   ├── SQL/         # SQL engine
│           │   ├── Search/      # Search engine
│           │   ├── Tools/       # Tool registry
│           │   └── Voice/       # Voice service
│           ├── Controllers/     # API endpoints
│           ├── Domain/         # Domain entities
│           ├── Infrastructure/ # Infrastructure services
│           └── Services/       # Application services
├── 04.FRONTEND/                # Next.js Frontend
│   └── src/
│       ├── app/                # Next.js app router
│       ├── components/         # React components
│       ├── lib/                # Utilities
│       └── stores/             # State management
└── 06.ENTERPRISE_AI_PLATFORM/  # Architecture docs
```

## API Endpoints

### Authentication
- `POST /api/v1/auth/register` - Register new user
- `POST /api/v1/auth/login` - User login
- `POST /api/v1/auth/refresh` - Refresh token
- `GET /api/v1/auth/me` - Get current user

### Documents
- `POST /api/v1/documents/upload` - Upload document
- `GET /api/v1/documents` - List documents
- `GET /api/v1/documents/{id}` - Get document
- `DELETE /api/v1/documents/{id}` - Delete document
- `GET /api/v1/documents/{id}/status` - Get processing status
- `GET /api/v1/documents/{id}/download` - Download document

### Chat
- `GET /api/v1/chat/sessions` - List chat sessions
- `POST /api/v1/chat/sessions` - Create session
- `GET /api/v1/chat/sessions/{id}` - Get session
- `DELETE /api/v1/chat/sessions/{id}` - Delete session
- `POST /api/v1/chat/sessions/{id}/messages` - Send message
- `GET /api/v1/chat/sessions/{id}/messages` - Get messages

### Search
- `GET /api/v1/search` - Basic search
- `POST /api/v1/search` - Advanced search
- `GET /api/v1/search/autocomplete` - Autocomplete

### AI Agent
- `POST /api/v1/agent/process` - Process agent request
- `POST /api/v1/agent/process/stream` - Stream response
- `POST /api/v1/agent/sessions` - Create agent session
- `GET /api/v1/agent/sessions/{id}` - Get session
- `POST /api/v1/agent/sessions/{id}/actions/{actionId}/approve` - Approve action

### Voice
- `POST /api/v1/voice/transcribe` - Speech to text
- `POST /api/v1/voice/synthesize` - Text to speech
- `POST /api/v1/voice/command` - Voice command

## Configuration

### Backend (config.json)
```json
{
  "app": {
    "port": 5000,
    "environment": "Development"
  },
  "database": {
    "connectionString": "Host=localhost;Database=aibaseframework;Username=aibfuser;Password=aibfpass123"
  },
  "redis": {
    "connectionString": "localhost:6379,password=redis123"
  },
  "ollama": {
    "baseUrl": "http://localhost:11434",
    "defaultChatModel": "llama3.2:3b",
    "defaultEmbeddingModel": "nomic-embed-text"
  },
  "minIO": {
    "endpoint": "localhost:9000",
    "accessKey": "minioadmin",
    "secretKey": "minioadmin",
    "bucketName": "documents"
  }
}
```

## Default Credentials

### Admin Account
- **Email**: admin@yourcompany.com
- **Password**: admin123

### Demo Accounts
- **Teacher**: user@yourcompany.com / user123
- **Student**: guest@yourcompany.com / guest123

## Development

### Backend Development
```bash
cd 03.BACKEND/src/AIBaseFramework.API
dotnet run
```

### Frontend Development
```bash
cd 04.FRONTEND
npm install
npm run dev
```

### Run Tests
```bash
cd 03.BACKEND/src/AIBaseFramework.API
dotnet test
```

## Security Features

- **JWT Authentication** with refresh tokens
- **SQL Injection Protection** with query validation
- **Prompt Injection Detection**
- **Rate Limiting** per user/tenant
- **RBAC** (Role-Based Access Control)
- **Audit Logging** for compliance
- **Human-in-the-loop Approval** for sensitive actions

## Multi-Tenancy

- Tenant isolation with Row-Level Security
- Per-tenant AI configuration
- Tenant-specific rate limits
- Configurable LLM models and prompts

## License

MIT License

## Contributing

Contributions are welcome! Please read our contributing guidelines before submitting PRs.

## Support

For issues and questions, please open a GitHub issue.
