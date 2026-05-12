# PHÂN TÍCH TOÀN DIỆN: ENTERPRISE AI PLATFORM ARCHITECTURE

**Dự án:** Hệ thống Tìm kiếm và Hỏi đáp Tài liệu Nội bộ → Enterprise AI Platform
**Ngày phân tích:** 2026-05-12
**Phạm vi:** Enterprise Architecture, AI Platform Engineering, Agentic AI, Multi-tenant, Security

---

## MỤC LỤC

1. [Đánh giá kiến trúc hiện tại](#1-đánh-giá-kiến-trúc-hiện-tại)
2. [Kiến trúc tổng thể đề xuất](#2-kiến-trúc-tổng-thể-đề-xuất)
3. [Module Boundaries & Dependency Flow](#3-module-boundaries--dependency-flow)
4. [Plugin Architecture & Extensibility](#4-plugin-architecture--extensibility)
5. [AI Agent Architecture](#5-ai-agent-architecture)
6. [AI Provider Abstraction & Gateway](#6-ai-provider-abstraction--gateway)
7. [Integration Strategy](#7-integration-strategy-cho-hệ-thống-bên-ngoài)
8. [Metadata & Domain Understanding](#8-metadata--domain-understanding-strategy)
9. [Voice & Multimodal AI](#9-voice--multimodal-ai)
10. [Security, Governance & Multi-tenant](#10-security-governance--multi-tenant)
11. [Deployment Architecture](#11-deployment-architecture)
12. [Recommended Tech Stack](#12-recommended-tech-stack)
13. [Roadmap phát triển](#13-roadmap-phát-triển-theo-giai-đoạn)

---

## 1. ĐÁNH GIÁ KIẾN TRÚC HIỆN TẠI

### 1.1 Thống kê kiến trúc hiện tại

```
CURRENT STATE:
├── Frontend: Next.js 14 + TypeScript + TailwindCSS
├── Backend: .NET 8 ASP.NET Core (single project)
│   ├── Controllers/ (Auth, Chat, Documents, Search)
│   ├── Services/ (AuthService only implemented)
│   ├── Infrastructure/
│   │   ├── AI/ (OllamaService, EmbeddingService, RAGPipeline)
│   │   ├── Data/ (AppDbContext - EF Core + pgvector)
│   │   ├── Cache/ (RedisCacheService)
│   │   └── Storage/ (MinIOService)
│   └── Domain/Entities/ (User, Document, DocumentChunk, ChatSession, ChatMessage)
├── Database: PostgreSQL 16 + pgvector
├── Cache: Redis 7
├── Object Storage: MinIO (S3-compatible)
├── LLM: Ollama (local, offline)
├── Deployment: Docker Compose
└── Docs: 11+ tài liệu thiết kế chi tiết
```

### 1.2 Đánh giá khả năng mở rộng thành AI Platform

| Tiêu chí | Hiện tại | Yêu cầu Platform | Gap |
|-----------|----------|-------------------|-----|
| **Modularity** | Single project, folder-based | Multi-module, bounded contexts | 🔴 Lớn |
| **Reusability** | Tightly coupled to Document Q&A | Pluggable cho mọi hệ thống | 🔴 Lớn |
| **Multi-tenant** | Single tenant | Per-tenant isolation | 🔴 Lớn |
| **AI Provider** | Ollama only (hardcoded) | Multi-provider abstraction | 🟠 Trung bình |
| **Integration** | Không có integration layer | REST/gRPC/MCP/Event-driven | 🔴 Lớn |
| **Agent/Tool** | RAG pipeline cố định | Dynamic tool calling, agents | 🔴 Lớn |
| **Security** | Basic JWT | Zero Trust, RBAC/ABAC, audit | 🟠 Trung bình |
| **Extensibility** | Không có plugin system | Plugin-based architecture | 🔴 Lớn |
| **Event-driven** | Không có | Event bus, async workflows | 🔴 Lớn |

### 1.3 Kết luận

**Kiến trúc hiện tại KHÔNG phù hợp** để phát triển trực tiếp thành reusable AI Platform vì:
- Monolithic single-project, không có module boundaries
- Business logic (Document Q&A) bị trộn lẫn với AI engine
- Không có abstraction layer cho AI providers
- Không có plugin/extension mechanism
- Không có event-driven communication
- Không có multi-tenant support

**Tuy nhiên**, các thành phần cốt lõi (Ollama integration, RAG pipeline, pgvector, Redis cache) có thể được **tách ra và tái cấu trúc** thành reusable modules.

### 1.4 Hướng đề xuất: Hybrid Architecture (Modular Monolith + Plugin + Agent)

Không nên chọn thuần microservice (quá phức tạp cho giai đoạn đầu), cũng không nên giữ monolith hiện tại. Hướng tối ưu:

```
RECOMMENDED: Modular Monolith → Gradual Microservice Extraction
+ Plugin-based Architecture (cho extensibility)
+ Agent-based Architecture (cho AI capabilities)
+ Event-driven Integration (cho communication)
```

---

## 2. KIẾN TRÚC TỔNG THỂ ĐỀ XUẤT

### 2.1 High-Level Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                              ENTERPRISE AI PLATFORM                                      │
├─────────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                         │
│  ┌───────────────────────────────────────────────────────────────────────────────────┐  │
│  │                         CLIENT LAYER (Multi-channel)                               │  │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐  │  │
│  │  │ Web App  │ │Mobile App│ │ Voice UI │ │ CLI/SDK  │ │ Embedded │ │3rd Party │  │  │
│  │  │(Next.js) │ │  (PWA)   │ │(Assistant)│ │(Dev Tool)│ │ Widget   │ │  Apps    │  │  │
│  │  └────┬─────┘ └────┬─────┘ └────┬─────┘ └────┬─────┘ └────┬─────┘ └────┬─────┘  │  │
│  └───────┼─────────────┼─────────────┼─────────────┼─────────────┼─────────────┼─────┘  │
│          └─────────────┴─────────────┴──────┬──────┴─────────────┴─────────────┘         │
│                                             │                                            │
│  ┌──────────────────────────────────────────┼────────────────────────────────────────┐   │
│  │                          API GATEWAY LAYER                                         │   │
│  │  ┌─────────────────────────────────────────────────────────────────────────────┐  │   │
│  │  │  • Authentication (JWT/OAuth2/API Key)    • Rate Limiting                   │  │   │
│  │  │  • Tenant Resolution                      • Request Routing                 │  │   │
│  │  │  • API Versioning                         • Load Balancing                  │  │   │
│  │  │  • Request/Response Transformation        • Circuit Breaker                 │  │   │
│  │  └─────────────────────────────────────────────────────────────────────────────┘  │   │
│  └───────────────────────────────────────────┬───────────────────────────────────────┘   │
│                                              │                                           │
│  ┌───────────────────────────────────────────┼───────────────────────────────────────┐   │
│  │                    PLATFORM CORE (Modular Monolith)                                │   │
│  │                                                                                    │   │
│  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌────────────┐ │   │
│  │  │   Tenant    │ │   Auth &    │ │  AI Agent   │ │  AI Engine  │ │  Plugin    │ │   │
│  │  │  Management │ │  Security   │ │   Runtime   │ │    Core     │ │  Runtime   │ │   │
│  │  │   Module    │ │   Module    │ │   Module    │ │   Module    │ │   Module   │ │   │
│  │  └──────┬──────┘ └──────┬──────┘ └──────┬──────┘ └──────┬──────┘ └──────┬─────┘ │   │
│  │         │               │               │               │               │        │   │
│  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌────────────┐ │   │
│  │  │  Workflow   │ │  Document   │ │   Search    │ │    Chat     │ │Integration │ │   │
│  │  │   Engine    │ │  Pipeline   │ │   Engine    │ │   Engine    │ │   Hub      │ │   │
│  │  │   Module    │ │   Module    │ │   Module    │ │   Module    │ │   Module   │ │   │
│  │  └──────┬──────┘ └──────┬──────┘ └──────┬──────┘ └──────┬──────┘ └──────┬─────┘ │   │
│  │         │               │               │               │               │        │   │
│  │  ┌──────┴───────────────┴───────────────┴───────────────┴───────────────┴──────┐ │   │
│  │  │                        SHARED KERNEL (Cross-cutting)                         │ │   │
│  │  │  • Event Bus  • Logging  • Caching  • Configuration  • Audit  • Metrics    │ │   │
│  │  └─────────────────────────────────────────────────────────────────────────────┘ │   │
│  └───────────────────────────────────────────┬───────────────────────────────────────┘   │
│                                              │                                           │
│  ┌───────────────────────────────────────────┼───────────────────────────────────────┐   │
│  │                       AI INFRASTRUCTURE LAYER                                      │   │
│  │                                                                                    │   │
│  │  ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌─────────────┐ ┌────────────┐ │   │
│  │  │ AI Gateway  │ │  Embedding  │ │   Vector    │ │   Model     │ │   Tool     │ │   │
│  │  │  (Router)   │ │   Engine    │ │    Store    │ │  Registry   │ │  Registry  │ │   │
│  │  └──────┬──────┘ └──────┬──────┘ └──────┬──────┘ └──────┬──────┘ └──────┬─────┘ │   │
│  │         │               │               │               │               │        │   │
│  │  ┌──────┴───────────────┴───────────────┴───────────────┴───────────────┴──────┐ │   │
│  │  │                         AI PROVIDERS                                         │ │   │
│  │  │  ┌────────┐ ┌────────┐ ┌────────┐ ┌────────┐ ┌────────┐ ┌────────────────┐ │ │   │
│  │  │  │Ollama  │ │OpenAI  │ │Claude  │ │Gemini  │ │Azure   │ │Custom/Self-host│ │ │   │
│  │  │  │(Local) │ │  GPT   │ │Anthropic│ │Google  │ │OpenAI  │ │   Providers    │ │ │   │
│  │  │  └────────┘ └────────┘ └────────┘ └────────┘ └────────┘ └────────────────┘ │ │   │
│  │  └─────────────────────────────────────────────────────────────────────────────┘ │   │
│  └───────────────────────────────────────────┬───────────────────────────────────────┘   │
│                                              │                                           │
│  ┌───────────────────────────────────────────┼───────────────────────────────────────┐   │
│  │                          DATA LAYER                                                │   │
│  │  ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌──────────┐ ┌───────────┐ │   │
│  │  │PostgreSQL│ │  Redis   │ │  MinIO   │ │  Vector  │ │  Event   │ │  Config   │ │   │
│  │  │(Relational)│ │ (Cache)  │ │(Objects) │ │  Store   │ │  Store   │ │  Store    │ │   │
│  │  └──────────┘ └──────────┘ └──────────┘ └──────────┘ └──────────┘ └───────────┘ │   │
│  └───────────────────────────────────────────────────────────────────────────────────┘   │
│                                                                                         │
│  ┌───────────────────────────────────────────────────────────────────────────────────┐   │
│  │                    EXTERNAL SYSTEM INTEGRATION                                     │   │
│  │  ┌────────┐ ┌────────┐ ┌────────┐ ┌────────┐ ┌────────┐ ┌────────┐ ┌─────────┐ │   │
│  │  │  ERP   │ │  CRM   │ │  DMS   │ │  HRM   │ │Workflow│ │   BI   │ │ Custom  │ │   │
│  │  │Systems │ │Systems │ │Systems │ │Systems │ │Systems │ │Systems │ │ Systems │ │   │
│  │  └────────┘ └────────┘ └────────┘ └────────┘ └────────┘ └────────┘ └─────────┘ │   │
│  └───────────────────────────────────────────────────────────────────────────────────┘   │
│                                                                                         │
└─────────────────────────────────────────────────────────────────────────────────────────┘
```

### 2.2 Lý do chọn Hybrid Architecture

| Approach | Ưu điểm | Nhược điểm | Phù hợp? |
|----------|---------|------------|-----------|
| **Pure Microservice** | Scale độc lập, team autonomy | Quá phức tạp ban đầu, operational overhead | ❌ Giai đoạn đầu |
| **Pure Monolith** | Đơn giản, deploy dễ | Không extensible, không reusable | ❌ Không đạt mục tiêu |
| **Modular Monolith** | Module boundaries rõ, deploy đơn giản, dễ tách sau | Cần discipline | ✅ Foundation |
| **Plugin-based** | Extensible, 3rd party integration | Plugin management phức tạp | ✅ Cho extensibility |
| **Agent-based** | AI-native, autonomous actions | Cần guardrails mạnh | ✅ Cho AI capabilities |
| **Hybrid (đề xuất)** | Kết hợp ưu điểm tất cả | Cần thiết kế cẩn thận | ✅✅ Tối ưu |

---

## 3. MODULE BOUNDARIES & DEPENDENCY FLOW

### 3.1 Bounded Contexts (DDD)

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                         BOUNDED CONTEXTS MAP                                     │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  ┌─────────────────────┐         ┌─────────────────────┐                       │
│  │  IDENTITY CONTEXT   │◄───────►│  TENANT CONTEXT     │                       │
│  │  • User             │         │  • Tenant            │                       │
│  │  • Role             │         │  • Subscription      │                       │
│  │  • Permission       │         │  • Configuration     │                       │
│  │  • Token            │         │  • Quota             │                       │
│  └──────────┬──────────┘         └──────────┬──────────┘                       │
│             │                               │                                   │
│             ▼                               ▼                                   │
│  ┌─────────────────────┐         ┌─────────────────────┐                       │
│  │  AI ENGINE CONTEXT  │         │  AGENT CONTEXT      │                       │
│  │  • LLM Provider     │◄───────►│  • Agent Definition │                       │
│  │  • Embedding Model  │         │  • Tool Registry    │                       │
│  │  • RAG Pipeline     │         │  • Action Execution │                       │
│  │  • Prompt Template  │         │  • Memory Store     │                       │
│  │  • Model Config     │         │  • Task Planning    │                       │
│  └──────────┬──────────┘         └──────────┬──────────┘                       │
│             │                               │                                   │
│             ▼                               ▼                                   │
│  ┌─────────────────────┐         ┌─────────────────────┐                       │
│  │  KNOWLEDGE CONTEXT  │         │  INTEGRATION CONTEXT│                       │
│  │  • Document         │         │  • Connector        │                       │
│  │  • Chunk            │         │  • API Registry     │                       │
│  │  • Vector Index     │         │  • Event Contract   │                       │
│  │  • Citation         │         │  • Webhook          │                       │
│  │  • Search Index     │         │  • MCP Server       │                       │
│  └──────────┬──────────┘         └──────────┬──────────┘                       │
│             │                               │                                   │
│             ▼                               ▼                                   │
│  ┌─────────────────────┐         ┌─────────────────────┐                       │
│  │  WORKFLOW CONTEXT   │         │  OBSERVABILITY CTX  │                       │
│  │  • Workflow Def     │         │  • Audit Log        │                       │
│  │  • Step Execution   │         │  • Metrics          │                       │
│  │  • Approval Flow   │         │  • Trace            │                       │
│  │  • Compensation     │         │  • Alert            │                       │
│  └─────────────────────┘         └─────────────────────┘                       │
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 3.2 Module Structure (Solution Layout)

```
AIBaseFramework/
├── src/
│   ├── Core/                                    # SHARED KERNEL
│   │   ├── AIBaseFramework.SharedKernel/        # Base classes, interfaces, events
│   │   ├── AIBaseFramework.Abstractions/        # Public contracts/interfaces
│   │   └── AIBaseFramework.SDK/                 # Client SDK for external systems
│   │
│   ├── Modules/                                 # BOUNDED CONTEXT MODULES
│   │   ├── AIBaseFramework.Identity/            # Auth, Users, RBAC/ABAC
│   │   ├── AIBaseFramework.Tenant/              # Multi-tenant management
│   │   ├── AIBaseFramework.AIEngine/            # LLM, Embedding, RAG core
│   │   ├── AIBaseFramework.Agent/               # Agent runtime, tool calling
│   │   ├── AIBaseFramework.Knowledge/           # Document, Search, Vector
│   │   ├── AIBaseFramework.Workflow/            # Workflow engine, approvals
│   │   ├── AIBaseFramework.Integration/         # Connectors, MCP, webhooks
│   │   └── AIBaseFramework.Observability/       # Audit, metrics, tracing
│   │
│   ├── Infrastructure/                          # INFRASTRUCTURE CONCERNS
│   │   ├── AIBaseFramework.Persistence/         # EF Core, PostgreSQL
│   │   ├── AIBaseFramework.Cache/               # Redis implementation
│   │   ├── AIBaseFramework.Storage/             # MinIO/S3 implementation
│   │   ├── AIBaseFramework.Messaging/           # Event bus (Redis Streams/RabbitMQ)
│   │   └── AIBaseFramework.AIProviders/         # Ollama, OpenAI, Claude adapters
│   │
│   ├── Hosts/                                   # DEPLOYMENT HOSTS
│   │   ├── AIBaseFramework.API/                 # Main API host (ASP.NET Core)
│   │   ├── AIBaseFramework.Worker/              # Background job host
│   │   ├── AIBaseFramework.Gateway/             # API Gateway (YARP)
│   │   └── AIBaseFramework.MCPServer/           # MCP Server host
│   │
│   └── Plugins/                                 # PLUGIN SYSTEM
│       ├── AIBaseFramework.Plugin.DocumentQA/   # Document Q&A plugin
│       ├── AIBaseFramework.Plugin.RAGSQL/       # RAG-SQL plugin
│       ├── AIBaseFramework.Plugin.Voice/        # Voice processing plugin
│       └── AIBaseFramework.Plugin.Template/     # Template for new plugins
│
├── tests/
│   ├── AIBaseFramework.UnitTests/
│   ├── AIBaseFramework.IntegrationTests/
│   └── AIBaseFramework.ArchTests/               # Architecture fitness tests
│
├── tools/
│   ├── AIBaseFramework.CLI/                     # CLI tool for management
│   └── AIBaseFramework.DevKit/                  # Developer toolkit
│
└── docker/
    ├── docker-compose.yml                       # Development
    ├── docker-compose.prod.yml                  # Production
    └── Dockerfile.*                             # Per-service Dockerfiles
```

### 3.3 Dependency Flow (Dependency Rule)

```
                    ┌─────────────────────────┐
                    │      Hosts (API,        │  ← Composition Root
                    │      Worker, Gateway)   │     (DI registration)
                    └────────────┬────────────┘
                                 │ depends on
                    ┌────────────┼────────────┐
                    │            │            │
                    ▼            ▼            ▼
            ┌────────────┐ ┌─────────┐ ┌──────────┐
            │  Modules   │ │Infra-   │ │ Plugins  │
            │(Business   │ │structure│ │(Optional │
            │ Logic)     │ │(Impl)   │ │ Features)│
            └──────┬─────┘ └────┬────┘ └────┬─────┘
                   │            │            │
                   │ depends on │ implements │ depends on
                   ▼            ▼            ▼
            ┌─────────────────────────────────────┐
            │           Core Layer                 │
            │  (SharedKernel + Abstractions)       │
            │                                     │
            │  • Domain Events                    │
            │  • Base Entities                    │
            │  • Interfaces (IRepository, etc.)   │
            │  • Value Objects                    │
            │  • Cross-cutting contracts          │
            └─────────────────────────────────────┘

RULES:
✅ Modules → Core (allowed)
✅ Infrastructure → Core (implements interfaces)
✅ Plugins → Core + Modules (consumes contracts)
✅ Hosts → Everything (composition root)
❌ Core → Modules (forbidden)
❌ Core → Infrastructure (forbidden)
❌ Modules → Infrastructure (forbidden, use interfaces)
❌ Module A → Module B directly (forbidden, use events/interfaces)
```

### 3.4 Inter-Module Communication

```
┌──────────────────────────────────────────────────────────────────┐
│              COMMUNICATION PATTERNS BETWEEN MODULES               │
├──────────────────────────────────────────────────────────────────┤
│                                                                  │
│  PATTERN 1: In-Process Event Bus (Default - Modular Monolith)    │
│  ┌──────────┐    Event    ┌──────────┐                          │
│  │ Module A │───────────►│ Module B │                          │
│  │(Publisher)│  (MediatR  │(Handler) │                          │
│  └──────────┘  Notification)└──────────┘                          │
│                                                                  │
│  PATTERN 2: Async Message Queue (For heavy/cross-boundary)       │
│  ┌──────────┐    Message    ┌──────────┐    Message    ┌────────┐│
│  │ Module A │──────────────►│  Redis   │─────────────►│Module B││
│  │(Producer)│               │ Streams  │              │(Consumer)│
│  └──────────┘               └──────────┘              └────────┘│
│                                                                  │
│  PATTERN 3: Shared Interface (For synchronous queries)           │
│  ┌──────────┐    IXxxQuery    ┌──────────┐                      │
│  │ Module A │────────────────►│ Module B │                      │
│  │(Consumer)│  (via DI, no   │(Provider)│                      │
│  └──────────┘   direct ref)   └──────────┘                      │
│                                                                  │
│  PATTERN 4: Integration Events (For external systems)            │
│  ┌──────────┐  IntegrationEvent  ┌──────────┐  Webhook/API      │
│  │ Platform │───────────────────►│Integration│──────────────►External│
│  │  Module  │                    │   Hub     │              System│
│  └──────────┘                    └──────────┘                    │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

---

## 4. PLUGIN ARCHITECTURE & EXTENSIBILITY

### 4.1 Plugin System Design

```
┌─────────────────────────────────────────────────────────────────────────┐
│                        PLUGIN ARCHITECTURE                               │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ┌───────────────────────────────────────────────────────────────────┐  │
│  │                      PLUGIN RUNTIME                                │  │
│  │                                                                   │  │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐              │  │
│  │  │   Plugin    │  │   Plugin    │  │   Plugin    │              │  │
│  │  │  Loader     │  │  Registry   │  │  Lifecycle  │              │  │
│  │  │(Assembly/DLL)│  │(Metadata)   │  │ (Start/Stop)│              │  │
│  │  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘              │  │
│  │         │                │                │                      │  │
│  │  ┌──────┴────────────────┴────────────────┴──────────────────┐   │  │
│  │  │                    PLUGIN HOST                              │   │  │
│  │  │  • Dependency Injection Scope (per-plugin)                 │   │  │
│  │  │  • Configuration Binding                                   │   │  │
│  │  │  • Permission Sandbox                                      │   │  │
│  │  │  • Resource Quota                                          │   │  │
│  │  └───────────────────────────────────────────────────────────┘   │  │
│  └───────────────────────────────────────────────────────────────────┘  │
│                              │                                          │
│                              ▼                                          │
│  ┌───────────────────────────────────────────────────────────────────┐  │
│  │                    PLUGIN CONTRACTS (Interfaces)                   │  │
│  │                                                                   │  │
│  │  interface IAIPlugin {                                            │  │
│  │    string Id { get; }                                             │  │
│  │    string Name { get; }                                           │  │
│  │    string Version { get; }                                        │  │
│  │    PluginCapabilities Capabilities { get; }                       │  │
│  │    Task InitializeAsync(IPluginContext context);                   │  │
│  │    Task ShutdownAsync();                                          │  │
│  │  }                                                                │  │
│  │                                                                   │  │
│  │  interface IToolProvider {                                        │  │
│  │    IReadOnlyList<ToolDefinition> GetTools();                      │  │
│  │    Task<ToolResult> ExecuteToolAsync(ToolCall call, CancellationToken ct);│
│  │  }                                                                │  │
│  │                                                                   │  │
│  │  interface IPromptProvider {                                      │  │
│  │    IReadOnlyList<PromptTemplate> GetPrompts();                    │  │
│  │    Task<string> RenderPromptAsync(string templateId, object context);│
│  │  }                                                                │  │
│  │                                                                   │  │
│  │  interface IMetadataProvider {                                    │  │
│  │    Task<SystemMetadata> GetMetadataAsync();                       │  │
│  │    Task<IReadOnlyList<ApiDefinition>> GetApisAsync();             │  │
│  │    Task<DatabaseSchema> GetSchemaAsync();                         │  │
│  │  }                                                                │  │
│  │                                                                   │  │
│  │  interface IEventHandler<TEvent> {                                │  │
│  │    Task HandleAsync(TEvent @event, CancellationToken ct);         │  │
│  │  }                                                                │  │
│  └───────────────────────────────────────────────────────────────────┘  │
│                                                                         │
│  ┌───────────────────────────────────────────────────────────────────┐  │
│  │                    EXAMPLE PLUGINS                                 │  │
│  │                                                                   │  │
│  │  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │  │
│  │  │ Document Q&A    │  │   RAG-SQL       │  │   ERP Connector │  │  │
│  │  │ Plugin          │  │   Plugin        │  │   Plugin        │  │  │
│  │  │                 │  │                 │  │                 │  │  │
│  │  │ • Upload docs   │  │ • Schema ingest │  │ • SAP/Oracle    │  │  │
│  │  │ • RAG search    │  │ • SQL generation│  │ • CRUD proxy    │  │  │
│  │  │ • Citations     │  │ • RBAC filter   │  │ • Event sync    │  │  │
│  │  └─────────────────┘  └─────────────────┘  └─────────────────┘  │  │
│  │                                                                   │  │
│  │  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │  │
│  │  │ Voice Assistant │  │  HRM Connector  │  │  Custom Plugin  │  │  │
│  │  │ Plugin          │  │  Plugin         │  │  (User-built)   │  │  │
│  │  │                 │  │                 │  │                 │  │  │
│  │  │ • STT/TTS       │  │ • Leave mgmt   │  │ • Any business  │  │  │
│  │  │ • Voice commands│  │ • Attendance    │  │   logic         │  │  │
│  │  │ • Intent detect │  │ • Payroll query │  │ • Custom tools  │  │  │
│  │  └─────────────────┘  └─────────────────┘  └─────────────────┘  │  │
│  └───────────────────────────────────────────────────────────────────┘  │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

### 4.2 Khi nào dùng gì?

| Cần gì? | Dùng | Lý do |
|---------|------|-------|
| Reuse logic trong cùng process | **Shared Library/Package** | Không overhead network, type-safe |
| Expose AI capabilities cho external dev | **SDK (NuGet/npm)** | Developer experience, versioned |
| Route requests tới nhiều AI providers | **AI Gateway** | Centralized control, routing logic |
| Scale độc lập một component nặng | **Microservice** | Embedding service cần GPU riêng |
| AI đọc hiểu hệ thống external | **MCP (Model Context Protocol)** | Standardized context protocol |
| AI thực hiện action trên hệ thống | **Tool Calling** | LLM-native, structured output |
| Thêm tính năng mới không sửa core | **Plugin** | Extensibility, isolation |
| Orchestrate nhiều bước phức tạp | **Workflow Engine** | State management, compensation |

### 4.3 Phân loại Component → Packaging Strategy

| Component | Package Type | Lý do |
|-----------|-------------|-------|
| **RAG Engine** | Module (in-process) | Core capability, cần low latency |
| **Semantic Search** | Module (in-process) | Tight coupling với Vector Store |
| **Hybrid Search** | Module (in-process) | Kết hợp semantic + keyword |
| **Embedding Engine** | Service (có thể tách) | GPU-intensive, có thể scale riêng |
| **Ollama Integration** | Infrastructure adapter | Swappable provider |
| **LLM Provider Abstraction** | Shared Library (NuGet) | Reuse across projects |
| **Vector Search** | Module (in-process) | Coupled với pgvector |
| **Citation Engine** | Module (in-process) | Part of RAG pipeline |
| **Prompt Engine** | Shared Library (NuGet) | Reusable, template-based |
| **AI Memory** | Module + Redis | Stateful, per-session |
| **RAG-SQL Engine** | Plugin | Optional feature, isolated |
| **AI Workflow Engine** | Module (core) | Orchestration backbone |
| **AI Orchestrator** | Module (core) | Agent coordination |
| **Tool Calling Engine** | Module (core) | Agent runtime essential |
| **Function Calling Engine** | Part of AI Gateway | Provider-specific |
| **AI Action Execution** | Module (core) | Security-critical |
| **Voice Processing** | Plugin + Service | Heavy, optional, GPU |
| **Speech-to-Text** | Service (tách) | Resource-intensive |
| **Text-to-Speech** | Service (tách) | Resource-intensive |
| **AI Agent Runtime** | Module (core) | Central orchestration |

---

## 5. AI AGENT ARCHITECTURE

### 5.1 Agent Runtime Design

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                          AI AGENT RUNTIME ARCHITECTURE                            │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  USER INPUT                                                                     │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  "Tạo công văn mới cho phòng kế toán"                                   │   │
│  │  OR Voice: [audio stream] → STT → text                                  │   │
│  └──────────────────────────────────┬──────────────────────────────────────┘   │
│                                     │                                           │
│                                     ▼                                           │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  STEP 1: INTENT RECOGNITION & CONTEXT ANALYSIS                          │   │
│  │  ┌───────────────┐  ┌───────────────┐  ┌───────────────────────────┐   │   │
│  │  │ Intent Parser │  │Context Builder│  │ Permission Resolver       │   │   │
│  │  │ (LLM-based)   │  │(User, Tenant, │  │ (RBAC check before plan) │   │   │
│  │  │               │  │ History, Scope)│  │                           │   │   │
│  │  └───────┬───────┘  └───────┬───────┘  └───────────┬───────────────┘   │   │
│  │          │                  │                       │                    │   │
│  │          ▼                  ▼                       ▼                    │   │
│  │  Intent: CREATE_DOCUMENT   Context: {tenant,       Permissions: {       │   │
│  │  Entity: "công văn"        user, dept: "kế toán",  canCreate: true,     │   │
│  │  Target: "phòng kế toán"   systems: [DMS, ERP]}    scope: "dept"}       │   │
│  └──────────────────────────────────┬──────────────────────────────────────┘   │
│                                     │                                           │
│                                     ▼                                           │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  STEP 2: TASK PLANNING (ReAct / Plan-and-Execute)                       │   │
│  │                                                                         │   │
│  │  LLM generates execution plan:                                          │   │
│  │  ┌─────────────────────────────────────────────────────────────────┐   │   │
│  │  │  Plan:                                                           │   │   │
│  │  │  1. Resolve "phòng kế toán" → department_id (Tool: lookup_dept) │   │   │
│  │  │  2. Get document template for "công văn" (Tool: get_template)    │   │   │
│  │  │  3. Create document with template (Tool: create_document)        │   │   │
│  │  │  4. Assign to department (Tool: assign_document)                 │   │   │
│  │  │  5. Notify relevant users (Tool: send_notification)              │   │   │
│  │  │                                                                  │   │   │
│  │  │  Approval Required: Step 3 (CREATE action)                       │   │   │
│  │  │  Estimated Steps: 5                                              │   │   │
│  │  │  Risk Level: LOW                                                 │   │   │
│  │  └─────────────────────────────────────────────────────────────────┘   │   │
│  └──────────────────────────────────┬──────────────────────────────────────┘   │
│                                     │                                           │
│                                     ▼                                           │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  STEP 3: ACTION EXECUTION PIPELINE                                      │   │
│  │                                                                         │   │
│  │  ┌─────────┐   ┌─────────┐   ┌─────────┐   ┌─────────┐   ┌─────────┐ │   │
│  │  │Validate │──►│ Guard-  │──►│ Execute │──►│ Verify  │──►│  Audit  │ │   │
│  │  │ Action  │   │  rails  │   │  Tool   │   │ Result  │   │   Log   │ │   │
│  │  └─────────┘   └─────────┘   └─────────┘   └─────────┘   └─────────┘ │   │
│  │       │              │              │              │              │      │   │
│  │       ▼              ▼              ▼              ▼              ▼      │   │
│  │  • Schema valid  • No dangerous • Call API    • Response OK  • Who     │   │
│  │  • Params OK       SQL           • CRUD op    • Data valid   • What    │   │
│  │  • Permission    • No PII leak  • Workflow    • Side effects • When    │   │
│  │    confirmed     • Rate limit     trigger       checked      • Result  │   │
│  │                  • Scope check                                          │   │
│  └──────────────────────────────────┬──────────────────────────────────────┘   │
│                                     │                                           │
│                                     ▼                                           │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  STEP 4: HUMAN-IN-THE-LOOP (Conditional)                               │   │
│  │                                                                         │   │
│  │  Risk Assessment:                                                       │   │
│  │  ┌────────────────────────────────────────────────────────────────┐    │   │
│  │  │  LOW RISK (auto-approve):  Read operations, search, query      │    │   │
│  │  │  MEDIUM RISK (notify):     Create, minor updates               │    │   │
│  │  │  HIGH RISK (require approval): Delete, bulk update, financial  │    │   │
│  │  │  CRITICAL (block + escalate): Schema change, permission change │    │   │
│  │  └────────────────────────────────────────────────────────────────┘    │   │
│  │                                                                         │   │
│  │  If approval needed:                                                    │   │
│  │  → Pause execution → Notify approver → Wait → Resume/Reject            │   │
│  └──────────────────────────────────┬──────────────────────────────────────┘   │
│                                     │                                           │
│                                     ▼                                           │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │  STEP 5: RESPONSE GENERATION                                            │   │
│  │                                                                         │   │
│  │  LLM synthesizes results → Natural language response                    │   │
│  │  + Action summary + Citations + Next suggested actions                  │   │
│  │                                                                         │   │
│  │  Output: "Đã tạo công văn CV-2026-0512 cho phòng kế toán.              │   │
│  │           Đã gán cho trưởng phòng Nguyễn Văn B để duyệt.               │   │
│  │           Bạn có muốn thêm nội dung cho công văn không?"                │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 5.2 Tool Registry Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         TOOL REGISTRY                                     │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ┌───────────────────────────────────────────────────────────────────┐  │
│  │  TOOL DEFINITION (JSON Schema-based)                              │  │
│  │                                                                   │  │
│  │  {                                                                │  │
│  │    "id": "dms.create_document",                                   │  │
│  │    "name": "Create Document",                                     │  │
│  │    "description": "Tạo tài liệu mới trong hệ thống DMS",         │  │
│  │    "system": "DMS",                                               │  │
│  │    "category": "CRUD",                                            │  │
│  │    "riskLevel": "MEDIUM",                                         │  │
│  │    "requiresApproval": false,                                     │  │
│  │    "parameters": {                                                │  │
│  │      "type": "object",                                            │  │
│  │      "properties": {                                              │  │
│  │        "title": { "type": "string", "required": true },           │  │
│  │        "department_id": { "type": "string", "required": true },   │  │
│  │        "template_id": { "type": "string" },                       │  │
│  │        "content": { "type": "string" }                            │  │
│  │      }                                                            │  │
│  │    },                                                             │  │
│  │    "permissions": ["document:create"],                            │  │
│  │    "endpoint": "POST /api/v1/documents",                          │  │
│  │    "compensationTool": "dms.delete_document"                      │  │
│  │  }                                                                │  │
│  └───────────────────────────────────────────────────────────────────┘  │
│                                                                         │
│  TOOL SOURCES:                                                          │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  ┌────────────┐ │
│  │  Built-in    │  │   Plugin     │  │    MCP       │  │  Dynamic   │ │
│  │  Tools       │  │   Tools      │  │   Tools      │  │  (OpenAPI) │ │
│  │              │  │              │  │              │  │            │ │
│  │ • search     │  │ • per-plugin │  │ • external   │  │ • auto-gen │ │
│  │ • query_db   │  │   registered │  │   MCP server │  │   from API │ │
│  │ • send_email │  │   tools      │  │   tools      │  │   spec     │ │
│  └──────────────┘  └──────────────┘  └──────────────┘  └────────────┘ │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

### 5.3 Multi-Agent Architecture

```
┌─────────────────────────────────────────────────────────────────────────┐
│                      MULTI-AGENT ORCHESTRATION                            │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  ┌───────────────────────────────────────────────────────────────────┐  │
│  │                    ORCHESTRATOR AGENT (Supervisor)                 │  │
│  │                                                                   │  │
│  │  Responsibilities:                                                │  │
│  │  • Decompose complex tasks into sub-tasks                         │  │
│  │  • Route sub-tasks to specialist agents                           │  │
│  │  • Aggregate results                                              │  │
│  │  • Handle failures and compensation                               │  │
│  └───────────────────────────┬───────────────────────────────────────┘  │
│                              │                                          │
│          ┌───────────────────┼───────────────────┐                     │
│          │                   │                   │                      │
│          ▼                   ▼                   ▼                      │
│  ┌───────────────┐  ┌───────────────┐  ┌───────────────┐              │
│  │  Knowledge    │  │   Action      │  │   Workflow    │              │
│  │  Agent        │  │   Agent       │  │   Agent       │              │
│  │               │  │               │  │               │              │
│  │ • RAG search  │  │ • CRUD ops    │  │ • Multi-step  │              │
│  │ • Doc Q&A     │  │ • API calls   │  │ • Approvals   │              │
│  │ • SQL query   │  │ • Tool exec   │  │ • Long-running│              │
│  │ • Citations   │  │ • Validation  │  │ • Compensation│              │
│  └───────────────┘  └───────────────┘  └───────────────┘              │
│          │                   │                   │                      │
│          ▼                   ▼                   ▼                      │
│  ┌───────────────────────────────────────────────────────────────────┐  │
│  │                    SHARED SERVICES                                 │  │
│  │  • AI Memory (conversation + long-term)                           │  │
│  │  • Tool Registry (available actions)                              │  │
│  │  • Permission Engine (what agent can do)                          │  │
│  │  • Audit Trail (what agent did)                                   │  │
│  └───────────────────────────────────────────────────────────────────┘  │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

### 5.4 AI Memory Architecture

```
MEMORY LAYERS:
┌─────────────────────────────────────────────────────────────────┐
│ Layer 1: Working Memory (Context Window)                         │
│ • Current conversation messages                                  │
│ • Active tool results                                            │
│ • Immediate context (user, tenant, permissions)                  │
│ • Storage: In-memory (per request)                               │
│ • TTL: Request lifetime                                          │
├─────────────────────────────────────────────────────────────────┤
│ Layer 2: Short-term Memory (Session)                             │
│ • Conversation history (last N turns)                            │
│ • Recent tool executions                                         │
│ • User preferences for session                                   │
│ • Storage: Redis                                                 │
│ • TTL: Session duration (30 min - 24h)                           │
├─────────────────────────────────────────────────────────────────┤
│ Layer 3: Long-term Memory (Persistent)                           │
│ • User interaction patterns                                      │
│ • Learned preferences                                            │
│ • Domain knowledge (per tenant)                                  │
│ • Successful action patterns                                     │
│ • Storage: PostgreSQL + Vector (semantic retrieval)               │
│ • TTL: Permanent (with decay/summarization)                      │
├─────────────────────────────────────────────────────────────────┤
│ Layer 4: Shared Knowledge (Platform-wide)                        │
│ • Domain ontologies                                              │
│ • System metadata cache                                          │
│ • Common prompt templates                                        │
│ • Tool documentation                                             │
│ • Storage: PostgreSQL + Redis                                    │
│ • TTL: Until invalidated                                         │
└─────────────────────────────────────────────────────────────────┘
```

### 5.5 Safety Guardrails & Rollback

```
AI ACTION SAFETY PIPELINE:

Input → [Prompt Injection Detection]
     → [Intent Classification]
     → [Permission Check (RBAC/ABAC)]
     → [Parameter Validation]
     → [Dangerous Pattern Detection]
         • SQL: No DROP/ALTER/TRUNCATE
         • API: No admin endpoints without explicit permission
         • File: No system paths
         • Data: No PII in logs
     → [Rate Limiting (per user, per tenant)]
     → [Scope Limitation]
         • Only access granted systems
         • Only granted tables/columns
         • Only within tenant boundary
     → [Human Approval Gate] (if risk > threshold)
     → [Execute in Sandbox]
     → [Verify Result]
     → [Audit Log]
     → [Compensation Registration] (for rollback)

ROLLBACK STRATEGY (Saga Pattern):
┌────────┐    ┌────────┐    ┌────────┐    ┌────────┐
│ Step 1 │───►│ Step 2 │───►│ Step 3 │───►│ Step 4 │
│ (done) │    │ (done) │    │ (FAIL) │    │(cancel)│
└────┬───┘    └────┬───┘    └────────┘    └────────┘
     │              │              │
     ▼              ▼              │
┌────────┐    ┌────────┐          │
│Compensate│◄──│Compensate│◄────────┘
│ Step 1  │    │ Step 2  │
└─────────┘    └─────────┘
```

---

## 6. AI PROVIDER ABSTRACTION & GATEWAY

### 6.1 AI Gateway Architecture

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           AI GATEWAY ARCHITECTURE                                 │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                        AI GATEWAY (Central Router)                       │   │
│  │                                                                         │   │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  │   │
│  │  │  Request    │  │   Model     │  │   Cost      │  │  Fallback   │  │   │
│  │  │  Router     │  │  Selector   │  │  Optimizer  │  │  Manager    │  │   │
│  │  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘  │   │
│  │         │                │                │                │          │   │
│  │  ┌──────┴────────────────┴────────────────┴────────────────┴──────┐   │   │
│  │  │                    ROUTING LOGIC                                 │   │   │
│  │  │                                                                 │   │   │
│  │  │  Routing Strategies:                                            │   │   │
│  │  │  • Tenant-based: Each tenant can configure preferred provider   │   │   │
│  │  │  • Task-based: Code gen → Claude, Chat → GPT, Embed → Local    │   │   │
│  │  │  • Cost-based: Route to cheapest provider meeting quality bar   │   │   │
│  │  │  • Latency-based: Route to fastest available provider           │   │   │
│  │  │  • Fallback: Primary → Secondary → Tertiary → Local            │   │   │
│  │  │  • Load-balance: Round-robin across equivalent providers        │   │   │
│  │  │  • Capability-based: Tool calling → providers that support it   │   │   │
│  │  └────────────────────────────────────────────────────────────────┘   │   │
│  │                              │                                         │   │
│  │  ┌───────────────────────────┼───────────────────────────────────┐    │   │
│  │  │              PROVIDER ADAPTERS (Uniform Interface)              │    │   │
│  │  │                                                                │    │   │
│  │  │  interface ILLMProvider {                                       │    │   │
│  │  │    string ProviderId { get; }                                   │    │   │
│  │  │    ModelCapabilities Capabilities { get; }                      │    │   │
│  │  │    Task<ChatResponse> ChatAsync(ChatRequest req);               │    │   │
│  │  │    Task<EmbeddingResponse> EmbedAsync(EmbedRequest req);        │    │   │
│  │  │    IAsyncEnumerable<StreamChunk> StreamChatAsync(ChatRequest);  │    │   │
│  │  │    Task<bool> IsAvailableAsync();                               │    │   │
│  │  │    ProviderMetrics GetMetrics();                                │    │   │
│  │  │  }                                                              │    │   │
│  │  └────────────────────────────────────────────────────────────────┘    │   │
│  └─────────────────────────────────────────────────────────────────────────┘   │
│                                     │                                           │
│         ┌───────────────────────────┼───────────────────────────┐              │
│         │              │            │            │               │              │
│         ▼              ▼            ▼            ▼               ▼              │
│  ┌────────────┐ ┌────────────┐ ┌────────────┐ ┌────────────┐ ┌────────────┐  │
│  │  Ollama    │ │  OpenAI    │ │  Claude    │ │  Gemini    │ │  Azure     │  │
│  │  (Local)   │ │  (Cloud)   │ │  (Cloud)   │ │  (Cloud)   │ │  OpenAI    │  │
│  │            │ │            │ │            │ │            │ │  (Cloud)   │  │
│  │ • llama3.2│ │ • gpt-4o   │ │ • opus     │ │ • pro      │ │ • gpt-4o   │  │
│  │ • mistral │ │ • gpt-4o-  │ │ • sonnet   │ │ • flash    │ │ • gpt-4    │  │
│  │ • phi3    │ │   mini     │ │ • haiku    │ │ • nano     │ │            │  │
│  │ • nomic   │ │ • embed-3  │ │            │ │ • embed    │ │            │  │
│  │   embed   │ │            │ │            │ │            │ │            │  │
│  └────────────┘ └────────────┘ └────────────┘ └────────────┘ └────────────┘  │
│         │              │            │            │               │              │
│         ▼              ▼            ▼            ▼               ▼              │
│  ┌────────────┐ ┌────────────┐ ┌────────────┐ ┌────────────┐ ┌────────────┐  │
│  │  DeepSeek  │ │  Cohere    │ │   Groq     │ │  Mistral   │ │ OpenRouter │  │
│  │  (Cloud)   │ │  (Cloud)   │ │  (Cloud)   │ │  (Cloud)   │ │  (Proxy)   │  │
│  └────────────┘ └────────────┘ └────────────┘ └────────────┘ └────────────┘  │
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 6.2 Provider Configuration (Per-Tenant)

```json
{
  "tenantId": "tenant-001",
  "aiConfig": {
    "defaultProvider": "ollama",
    "fallbackChain": ["ollama", "openai", "claude"],
    "providers": {
      "ollama": {
        "enabled": true,
        "baseUrl": "http://ollama:11434",
        "models": {
          "chat": "llama3.2:8b",
          "embedding": "nomic-embed-text"
        },
        "priority": 1,
        "maxConcurrent": 4
      },
      "openai": {
        "enabled": true,
        "apiKey": "${OPENAI_API_KEY}",
        "models": {
          "chat": "gpt-4o-mini",
          "embedding": "text-embedding-3-small"
        },
        "priority": 2,
        "costLimit": { "daily": 10.00, "monthly": 200.00 },
        "maxTokensPerRequest": 4096
      },
      "claude": {
        "enabled": false,
        "apiKey": "${CLAUDE_API_KEY}",
        "models": { "chat": "claude-sonnet-4-20250514" },
        "priority": 3
      }
    },
    "routing": {
      "strategy": "cost-aware-with-fallback",
      "rules": [
        { "taskType": "embedding", "provider": "ollama", "reason": "local, free" },
        { "taskType": "simple_chat", "provider": "ollama", "reason": "fast, free" },
        { "taskType": "complex_reasoning", "provider": "openai", "reason": "quality" },
        { "taskType": "code_generation", "provider": "claude", "reason": "best for code" }
      ]
    }
  }
}
```

### 6.3 Hybrid Local/Cloud Strategy

```
DECISION FLOW: Local vs Cloud

User Request
     │
     ▼
┌─────────────────┐     YES     ┌─────────────────┐
│ Is local model  │────────────►│  Use Ollama     │
│ sufficient?     │             │  (Free, Fast,   │
│                 │             │   Private)       │
└────────┬────────┘             └─────────────────┘
         │ NO
         ▼
┌─────────────────┐     YES     ┌─────────────────┐
│ Is cloud allowed│────────────►│  Route to Cloud │
│ by tenant policy│             │  Provider       │
│ + data policy?  │             │  (Best quality) │
└────────┬────────┘             └─────────────────┘
         │ NO
         ▼
┌─────────────────┐
│ Degrade gracefully│
│ Use local with   │
│ quality warning  │
└─────────────────┘

SCENARIOS:
• Offline/Air-gapped: 100% Ollama (no internet required)
• Hybrid (default): Local for simple tasks, cloud for complex
• Cloud-first: Cloud providers primary, local as fallback
• Cost-optimized: Local until quality threshold not met
```

---

## 7. INTEGRATION STRATEGY CHO HỆ THỐNG BÊN NGOÀI

### 7.1 So sánh Communication Patterns

| Pattern | Latency | Coupling | Scalability | Offline | Security | Use Case |
|---------|---------|----------|-------------|---------|----------|----------|
| **REST API** | Medium | Low | High | ✅ | Good | CRUD, standard integration |
| **gRPC** | Low | Medium | Very High | ✅ | Excellent | Internal services, streaming |
| **GraphQL** | Medium | Low | Medium | ✅ | Good | Flexible queries, frontend |
| **DB Direct** | Very Low | Very High | Low | ✅ | Poor | ❌ Anti-pattern (avoid) |
| **RabbitMQ** | Medium | Very Low | High | ✅ | Good | Async tasks, reliable delivery |
| **Kafka** | Low | Very Low | Very High | ✅ | Good | Event streaming, high volume |
| **Webhook** | Variable | Low | Medium | ❌ | Medium | Event notification |
| **MCP** | Low | Medium | Medium | ✅ | Good | AI context provision |
| **Plugin SDK** | Very Low | Medium | Medium | ✅ | Excellent | In-process extension |
| **Agent SDK** | Low | Low | High | ✅ | Good | AI-powered integration |
| **Event Bus** | Low | Very Low | High | ✅ | Good | Internal async communication |
| **API Gateway** | Medium | Very Low | Very High | ✅ | Excellent | Centralized routing |
| **Service Mesh** | Low | Very Low | Very High | ✅ | Excellent | Complex microservice |

### 7.2 Recommended Integration Architecture

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                    INTEGRATION HUB ARCHITECTURE                                   │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  EXTERNAL SYSTEMS                    AI PLATFORM                                │
│                                                                                 │
│  ┌──────────┐                  ┌─────────────────────────────────────────────┐ │
│  │   ERP    │──REST/gRPC──────►│                                             │ │
│  └──────────┘                  │         INTEGRATION HUB                      │ │
│  ┌──────────┐                  │                                             │ │
│  │   CRM    │──REST/Webhook───►│  ┌─────────────────────────────────────┐   │ │
│  └──────────┘                  │  │        CONNECTOR LAYER               │   │ │
│  ┌──────────┐                  │  │                                     │   │ │
│  │   DMS    │──MCP Server─────►│  │  ┌──────────┐  ┌──────────┐       │   │ │
│  └──────────┘                  │  │  │  REST    │  │  gRPC    │       │   │ │
│  ┌──────────┐                  │  │  │ Connector│  │ Connector│       │   │ │
│  │   HRM    │──Event Bus──────►│  │  └──────────┘  └──────────┘       │   │ │
│  └──────────┘                  │  │  ┌──────────┐  ┌──────────┐       │   │ │
│  ┌──────────┐                  │  │  │  MCP     │  │  Event   │       │   │ │
│  │ Workflow │──Plugin SDK─────►│  │  │ Connector│  │ Connector│       │   │ │
│  └──────────┘                  │  │  └──────────┘  └──────────┘       │   │ │
│  ┌──────────┐                  │  │  ┌──────────┐  ┌──────────┐       │   │ │
│  │   BI     │──REST───────────►│  │  │  DB      │  │  Plugin  │       │   │ │
│  └──────────┘                  │  │  │ Connector│  │ Connector│       │   │ │
│                                │  │  └──────────┘  └──────────┘       │   │ │
│                                │  └─────────────────────────────────────┘   │ │
│                                │                    │                        │ │
│                                │                    ▼                        │ │
│                                │  ┌─────────────────────────────────────┐   │ │
│                                │  │      ADAPTER & TRANSFORM LAYER       │   │ │
│                                │  │                                     │   │ │
│                                │  │  • Protocol translation             │   │ │
│                                │  │  • Data mapping                     │   │ │
│                                │  │  • Schema validation                │   │ │
│                                │  │  • Error normalization              │   │ │
│                                │  │  • Retry/Circuit breaker            │   │ │
│                                │  └─────────────────────────────────────┘   │ │
│                                │                    │                        │ │
│                                │                    ▼                        │ │
│                                │  ┌─────────────────────────────────────┐   │ │
│                                │  │      UNIFIED INTERNAL API            │   │ │
│                                │  │                                     │   │ │
│                                │  │  AI Platform consumes all external  │   │ │
│                                │  │  systems through ONE interface      │   │ │
│                                │  └─────────────────────────────────────┘   │ │
│                                └─────────────────────────────────────────────┘ │
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 7.3 Kiến trúc phù hợp theo yêu cầu

| Yêu cầu | Pattern đề xuất | Lý do |
|----------|----------------|-------|
| **Enterprise** | REST API + Event Bus + API Gateway | Standard, well-understood |
| **Offline local** | gRPC + In-process Event Bus | No external dependency |
| **Bảo mật cao** | mTLS + API Gateway + Zero Trust | End-to-end encryption |
| **Scalable** | Kafka/RabbitMQ + gRPC + Service Mesh | Async, high throughput |
| **Multi-system** | Integration Hub + MCP + Plugin SDK | Unified abstraction |
| **Hybrid cloud/local** | API Gateway + Fallback routing | Transparent switching |
| **Low latency** | gRPC + In-memory Event Bus + Redis | Minimal overhead |
| **Maintainability** | REST + Clean contracts + SDK | Simple, documented |

### 7.4 Minimal Integration Contract

Khi hệ thống mới muốn "cắm" vào AI Platform, chỉ cần implement:

```csharp
// MINIMAL: Chỉ cần 1 interface này
public interface ISystemConnector
{
    // Metadata: AI đọc hiểu hệ thống
    Task<SystemManifest> GetManifestAsync();
    
    // Tools: AI có thể thực hiện actions
    Task<IReadOnlyList<ToolDefinition>> GetToolsAsync();
    
    // Execute: AI gọi action
    Task<ToolResult> ExecuteAsync(ToolCall call, ExecutionContext ctx);
}

// SystemManifest chứa:
public record SystemManifest
{
    public string SystemId { get; init; }
    public string Name { get; init; }
    public string Description { get; init; }
    public IReadOnlyList<ApiEndpoint> Apis { get; init; }
    public DatabaseSchema? Schema { get; init; }
    public IReadOnlyList<WorkflowDefinition> Workflows { get; init; }
    public DomainGlossary Glossary { get; init; }
    public PermissionModel Permissions { get; init; }
}
```

---

## 8. METADATA & DOMAIN UNDERSTANDING STRATEGY

### 8.1 AI cần tối thiểu những gì để hiểu hệ thống mới?

```
MINIMUM VIABLE METADATA (MVM):
┌─────────────────────────────────────────────────────────────────────────┐
│                                                                         │
│  TIER 1: BẮT BUỘC (AI có thể hoạt động cơ bản)                        │
│  ┌───────────────────────────────────────────────────────────────────┐  │
│  │  1. OpenAPI/Swagger Spec (API endpoints + schemas)                │  │
│  │  2. Database Schema (tables, columns, types, relationships)       │  │
│  │  3. Domain Glossary (thuật ngữ nghiệp vụ → giải thích)           │  │
│  │  4. Permission Model (ai được làm gì, không được làm gì)         │  │
│  └───────────────────────────────────────────────────────────────────┘  │
│                                                                         │
│  TIER 2: NÊN CÓ (AI hoạt động thông minh hơn)                         │
│  ┌───────────────────────────────────────────────────────────────────┐  │
│  │  5. Business Rules (validation, constraints, logic)               │  │
│  │  6. Workflow Definitions (state machines, approval flows)         │  │
│  │  7. Entity Relationships (ERD, foreign keys, business meaning)    │  │
│  │  8. Example Queries/Actions (few-shot examples cho AI)            │  │
│  └───────────────────────────────────────────────────────────────────┘  │
│                                                                         │
│  TIER 3: TỐI ƯU (AI hoạt động như expert)                             │
│  ┌───────────────────────────────────────────────────────────────────┐  │
│  │  9. UI Actions & Form Metadata (what users can do on UI)          │  │
│  │  10. Event Contracts (what events system emits/consumes)          │  │
│  │  11. Data Dictionary (field meanings, valid values, formats)      │  │
│  │  12. Document Templates (output formats, report structures)       │  │
│  │  13. Ontology/Taxonomy (domain hierarchy, categories)             │  │
│  │  14. Historical Patterns (common queries, frequent actions)       │  │
│  └───────────────────────────────────────────────────────────────────┘  │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

### 8.2 Metadata Structure

```json
{
  "system": {
    "id": "hrm-system",
    "name": "Human Resource Management",
    "version": "2.1.0",
    "description": "Quản lý nhân sự, chấm công, nghỉ phép"
  },
  "apis": {
    "openApiSpec": "https://hrm.internal/swagger/v1/swagger.json",
    "baseUrl": "https://hrm.internal/api/v1",
    "authentication": { "type": "bearer", "tokenEndpoint": "/auth/token" }
  },
  "database": {
    "schema": [
      {
        "table": "employees",
        "description": "Bảng nhân viên",
        "columns": [
          { "name": "id", "type": "uuid", "description": "Mã nhân viên" },
          { "name": "full_name", "type": "varchar", "description": "Họ tên đầy đủ" },
          { "name": "department_id", "type": "uuid", "description": "Phòng ban", "fk": "departments.id" }
        ],
        "aiAccess": "read-only",
        "sensitiveColumns": ["salary", "ssn"]
      }
    ]
  },
  "glossary": {
    "nghỉ phép": "Leave request - đơn xin nghỉ phép của nhân viên",
    "chấm công": "Attendance - ghi nhận giờ vào/ra của nhân viên",
    "KPI": "Key Performance Indicator - chỉ số đánh giá hiệu suất"
  },
  "workflows": [
    {
      "id": "leave-approval",
      "name": "Quy trình duyệt nghỉ phép",
      "states": ["draft", "submitted", "manager_review", "approved", "rejected"],
      "transitions": [
        { "from": "draft", "to": "submitted", "action": "submit", "actor": "employee" },
        { "from": "submitted", "to": "manager_review", "action": "auto", "actor": "system" },
        { "from": "manager_review", "to": "approved", "action": "approve", "actor": "manager" },
        { "from": "manager_review", "to": "rejected", "action": "reject", "actor": "manager" }
      ]
    }
  ],
  "permissions": {
    "roles": ["employee", "manager", "hr_admin"],
    "matrix": {
      "employee": { "employees": ["read:self"], "leave_requests": ["read:self", "create:self"] },
      "manager": { "employees": ["read:dept"], "leave_requests": ["read:dept", "approve:dept"] },
      "hr_admin": { "employees": ["read:all", "write:all"], "leave_requests": ["read:all", "manage:all"] }
    }
  }
}
```

### 8.3 AI Ingest Pipeline

```
METADATA INGESTION FLOW:

┌──────────────┐     ┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│  1. Collect  │────►│  2. Parse    │────►│  3. Enrich   │────►│  4. Index    │
│  Metadata    │     │  & Validate  │     │  & Link      │     │  & Store     │
└──────────────┘     └──────────────┘     └──────────────┘     └──────────────┘
      │                    │                    │                    │
      ▼                    ▼                    ▼                    ▼
• OpenAPI spec       • Schema valid?     • Generate         • Vector embed
• DB schema dump     • Types correct?      descriptions       glossary terms
• Manual glossary    • Relations valid?  • Link entities    • Store in
• Workflow YAML      • Permissions        to glossary         knowledge base
• Business docs        complete?         • Infer implicit   • Build tool
                                           relationships       definitions
                                         • Generate
                                           few-shot examples
```

### 8.4 Context Engineering Strategy

```
CONTEXT WINDOW OPTIMIZATION:

┌─────────────────────────────────────────────────────────────────┐
│  DYNAMIC CONTEXT ASSEMBLY (per request)                          │
│                                                                  │
│  Budget: ~128K tokens (model dependent)                          │
│                                                                  │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │  ALWAYS INCLUDE (System Prompt):           ~2K tokens    │    │
│  │  • Platform identity & capabilities                      │    │
│  │  • Safety rules & guardrails                             │    │
│  │  • Output format instructions                            │    │
│  └─────────────────────────────────────────────────────────┘    │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │  TENANT CONTEXT:                           ~3K tokens    │    │
│  │  • Tenant config & preferences                           │    │
│  │  • User role & permissions                               │    │
│  │  • Available systems & tools                             │    │
│  └─────────────────────────────────────────────────────────┘    │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │  RELEVANT METADATA (dynamically selected): ~5-10K tokens │    │
│  │  • Only schemas of relevant tables (based on intent)     │    │
│  │  • Only relevant API endpoints                           │    │
│  │  • Only relevant glossary terms                          │    │
│  │  • Only relevant workflow definitions                    │    │
│  └─────────────────────────────────────────────────────────┘    │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │  TOOL DEFINITIONS (relevant subset):       ~3-5K tokens  │    │
│  │  • Only tools for detected intent                        │    │
│  │  • Prioritized by relevance score                        │    │
│  └─────────────────────────────────────────────────────────┘    │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │  CONVERSATION HISTORY:                     ~5-20K tokens │    │
│  │  • Recent messages (sliding window)                      │    │
│  │  • Summarized older context                              │    │
│  └─────────────────────────────────────────────────────────┘    │
│  ┌─────────────────────────────────────────────────────────┐    │
│  │  RAG RESULTS (if knowledge query):         ~5-15K tokens │    │
│  │  • Retrieved document chunks                             │    │
│  │  • Database query results                                │    │
│  └─────────────────────────────────────────────────────────┘    │
│                                                                  │
│  TOTAL: Dynamically balanced to fit context window               │
└─────────────────────────────────────────────────────────────────┘
```

---

## 9. VOICE & MULTIMODAL AI

### 9.1 Voice Pipeline Architecture

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                        VOICE-TO-ACTION PIPELINE                                   │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  ┌─────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────────────┐ │
│  │  User   │    │   Audio     │    │   Speech    │    │    Text Output      │ │
│  │  Voice  │───►│  Capture    │───►│   to Text   │───►│  (Transcription)    │ │
│  │  Input  │    │  (WebRTC/   │    │  (Whisper/  │    │                     │ │
│  │         │    │   WebSocket)│    │   Local STT)│    │  "Duyệt đơn nghỉ   │ │
│  └─────────┘    └─────────────┘    └─────────────┘    │   phép của Nguyễn   │ │
│                                                        │   Văn A"            │ │
│                                                        └──────────┬──────────┘ │
│                                                                   │             │
│                                                                   ▼             │
│  ┌─────────────────────────────────────────────────────────────────────────┐   │
│  │                    STANDARD AI AGENT PIPELINE                            │   │
│  │  (Same as text input - Intent → Plan → Execute → Respond)               │   │
│  └──────────────────────────────────────────────────────────────┬──────────┘   │
│                                                                  │              │
│                                                                  ▼              │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌──────────┐       │
│  │   Text      │    │   Text to   │    │   Audio     │    │  User    │       │
│  │  Response   │───►│   Speech    │───►│  Streaming  │───►│  Hears   │       │
│  │  from Agent │    │  (TTS/Local)│    │  (WebSocket)│    │ Response │       │
│  └─────────────┘    └─────────────┘    └─────────────┘    └──────────┘       │
│                                                                                 │
│  OFFLINE STT OPTIONS:                    OFFLINE TTS OPTIONS:                   │
│  • Whisper.cpp (C++, fast)               • Piper TTS (fast, multilingual)      │
│  • Vosk (lightweight)                    • Coqui TTS (high quality)            │
│  • faster-whisper (Python)               • VITS (Vietnamese support)           │
│                                                                                 │
│  CLOUD STT OPTIONS:                      CLOUD TTS OPTIONS:                    │
│  • OpenAI Whisper API                    • OpenAI TTS                          │
│  • Google Speech-to-Text                 • Google Cloud TTS                    │
│  • Azure Speech Services                 • Azure Speech Services               │
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 9.2 Voice Security

```
VOICE AUTHENTICATION FLOW:
1. Voice input received
2. Speaker verification (optional - voiceprint matching)
3. Session token validation (user must be authenticated)
4. STT transcription
5. Intent + action goes through SAME permission pipeline as text
6. High-risk voice commands require additional confirmation:
   "Bạn vừa yêu cầu XÓA tất cả hợp đồng. Xác nhận bằng cách nói 'Xác nhận xóa'"
```

---

## 10. SECURITY, GOVERNANCE & MULTI-TENANT

### 10.1 Zero Trust AI Security Model

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                      AI SECURITY ARCHITECTURE                                     │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  LAYER 1: PERIMETER SECURITY                                                   │
│  ┌───────────────────────────────────────────────────────────────────────────┐ │
│  │  • API Gateway with rate limiting                                         │ │
│  │  • mTLS for service-to-service                                            │ │
│  │  • DDoS protection                                                        │ │
│  │  • IP allowlisting (for enterprise)                                       │ │
│  └───────────────────────────────────────────────────────────────────────────┘ │
│                                                                                 │
│  LAYER 2: IDENTITY & ACCESS                                                    │
│  ┌───────────────────────────────────────────────────────────────────────────┐ │
│  │  • JWT + OAuth2 authentication                                            │ │
│  │  • RBAC (Role-Based) + ABAC (Attribute-Based) hybrid                      │ │
│  │  • Tenant isolation (data + compute)                                      │ │
│  │  • API key management (for SDK/external)                                  │ │
│  │  • Session management with rotation                                       │ │
│  └───────────────────────────────────────────────────────────────────────────┘ │
│                                                                                 │
│  LAYER 3: AI-SPECIFIC SECURITY                                                 │
│  ┌───────────────────────────────────────────────────────────────────────────┐ │
│  │  • Prompt injection detection (input sanitization)                        │ │
│  │  • Output filtering (PII, sensitive data)                                 │ │
│  │  • Hallucination detection for actions (verify before execute)            │ │
│  │  • SQL injection prevention (parameterized, allowlist tables)             │ │
│  │  • Tool execution sandboxing                                              │ │
│  │  • Token budget limits (prevent cost attacks)                             │ │
│  └───────────────────────────────────────────────────────────────────────────┘ │
│                                                                                 │
│  LAYER 4: DATA SECURITY                                                        │
│  ┌───────────────────────────────────────────────────────────────────────────┐ │
│  │  • Encryption at rest (AES-256)                                           │ │
│  │  • Encryption in transit (TLS 1.3)                                        │ │
│  │  • Data classification (public/internal/confidential/restricted)          │ │
│  │  • PII masking in AI context                                              │ │
│  │  • Data residency compliance (per tenant)                                 │ │
│  │  • Right to be forgotten (GDPR)                                           │ │
│  └───────────────────────────────────────────────────────────────────────────┘ │
│                                                                                 │
│  LAYER 5: AUDIT & COMPLIANCE                                                   │
│  ┌───────────────────────────────────────────────────────────────────────────┐ │
│  │  • Full audit trail (who, what, when, result)                             │ │
│  │  • AI decision logging (prompt, response, actions taken)                  │ │
│  │  • Compliance reporting (SOC2, ISO27001, GDPR)                            │ │
│  │  • Anomaly detection (unusual AI behavior)                                │ │
│  │  • Retention policies (per regulation)                                    │ │
│  └───────────────────────────────────────────────────────────────────────────┘ │
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 10.2 Multi-Tenant Architecture

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                      MULTI-TENANT ISOLATION MODEL                                 │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  ISOLATION STRATEGY: Shared Infrastructure + Logical Isolation                  │
│                                                                                 │
│  ┌───────────────────────────────────────────────────────────────────────────┐ │
│  │                         SHARED COMPUTE                                     │ │
│  │                                                                           │ │
│  │  ┌─────────────────────────────────────────────────────────────────────┐ │ │
│  │  │  API Gateway (tenant resolution via subdomain/header/token)         │ │ │
│  │  └─────────────────────────────────────────────────────────────────────┘ │ │
│  │                              │                                            │ │
│  │          ┌───────────────────┼───────────────────┐                       │ │
│  │          ▼                   ▼                   ▼                        │ │
│  │  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐                  │ │
│  │  │  Tenant A   │    │  Tenant B   │    │  Tenant C   │                  │ │
│  │  │  Context    │    │  Context    │    │  Context    │                  │ │
│  │  │             │    │             │    │             │                  │ │
│  │  │ • Config    │    │ • Config    │    │ • Config    │                  │ │
│  │  │ • AI Model  │    │ • AI Model  │    │ • AI Model  │                  │ │
│  │  │ • Plugins   │    │ • Plugins   │    │ • Plugins   │                  │ │
│  │  │ • Prompts   │    │ • Prompts   │    │ • Prompts   │                  │ │
│  │  │ • Tools     │    │ • Tools     │    │ • Tools     │                  │ │
│  │  │ • Memory    │    │ • Memory    │    │ • Memory    │                  │ │
│  │  └──────┬──────┘    └──────┬──────┘    └──────┬──────┘                  │ │
│  │         │                  │                  │                           │ │
│  └─────────┼──────────────────┼──────────────────┼───────────────────────────┘ │
│            │                  │                  │                              │
│  ┌─────────┼──────────────────┼──────────────────┼───────────────────────────┐ │
│  │         ▼                  ▼                  ▼         DATA ISOLATION     │ │
│  │                                                                           │ │
│  │  Option A: Schema-per-tenant (PostgreSQL schemas)                         │ │
│  │  ┌──────────────────────────────────────────────────────────────────┐    │ │
│  │  │  PostgreSQL                                                       │    │ │
│  │  │  ├── schema: tenant_a  (all tables for tenant A)                  │    │ │
│  │  │  ├── schema: tenant_b  (all tables for tenant B)                  │    │ │
│  │  │  └── schema: shared    (platform-wide config)                     │    │ │
│  │  └──────────────────────────────────────────────────────────────────┘    │ │
│  │                                                                           │ │
│  │  Option B: Row-level security (for smaller tenants)                       │ │
│  │  ┌──────────────────────────────────────────────────────────────────┐    │ │
│  │  │  Every table has tenant_id column + RLS policy                    │    │ │
│  │  │  SET app.current_tenant = 'tenant_a';                             │    │ │
│  │  │  -- All queries automatically filtered                            │    │ │
│  │  └──────────────────────────────────────────────────────────────────┘    │ │
│  │                                                                           │ │
│  │  Option C: Database-per-tenant (for enterprise/compliance)                │ │
│  │  ┌──────────────────────────────────────────────────────────────────┐    │ │
│  │  │  Separate PostgreSQL instance per tenant                          │    │ │
│  │  │  (highest isolation, highest cost)                                │    │ │
│  │  └──────────────────────────────────────────────────────────────────┘    │ │
│  │                                                                           │ │
│  └───────────────────────────────────────────────────────────────────────────┘ │
│                                                                                 │
│  RECOMMENDED: Start with Row-Level Security → Migrate to Schema-per-tenant     │
│  as tenant count grows. Database-per-tenant only for regulated industries.     │
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 10.3 Per-Tenant Configuration

```
CONFIGURATION HIERARCHY:

Platform Defaults (base)
    └── Tenant Override (per tenant)
        └── Project Override (per project within tenant)
            └── User Preferences (per user)

WHAT CAN BE CONFIGURED PER TENANT:
• AI provider selection (Ollama/OpenAI/Claude/etc.)
• Model selection (which model for which task)
• Prompt templates (custom system prompts)
• Tool availability (which tools are enabled)
• Plugin activation (which plugins are loaded)
• Security policies (approval thresholds, allowed actions)
• Data retention (how long to keep AI logs)
• Cost limits (daily/monthly token budgets)
• Integration endpoints (which external systems)
• Language preferences (UI + AI response language)
```

---

## 11. DEPLOYMENT ARCHITECTURE

### 11.1 Deployment Models

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                       DEPLOYMENT OPTIONS                                          │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│  MODEL 1: SINGLE-NODE DOCKER (Development / Small Enterprise)                   │
│  ┌───────────────────────────────────────────────────────────────────────────┐ │
│  │  docker-compose.yml                                                       │ │
│  │  ├── ai-platform-api (ASP.NET Core)                                       │ │
│  │  ├── ai-platform-worker (Background jobs)                                 │ │
│  │  ├── ai-gateway (YARP reverse proxy)                                      │ │
│  │  ├── postgres (+ pgvector)                                                │ │
│  │  ├── redis                                                                │ │
│  │  ├── minio                                                                │ │
│  │  ├── ollama (GPU passthrough)                                             │ │
│  │  └── nginx (reverse proxy + SSL)                                          │ │
│  │                                                                           │ │
│  │  Requirements: 16GB RAM, 4 CPU cores, GPU optional                        │ │
│  │  Best for: Offline, air-gapped, single-team                               │ │
│  └───────────────────────────────────────────────────────────────────────────┘ │
│                                                                                 │
│  MODEL 2: KUBERNETES (Production / Multi-tenant)                                │
│  ┌───────────────────────────────────────────────────────────────────────────┐ │
│  │  Kubernetes Cluster                                                       │ │
│  │  ├── Namespace: ai-platform                                               │ │
│  │  │   ├── Deployment: api (3 replicas, HPA)                                │ │
│  │  │   ├── Deployment: worker (2 replicas)                                  │ │
│  │  │   ├── Deployment: gateway (2 replicas)                                 │ │
│  │  │   ├── StatefulSet: postgres (primary + replica)                        │ │
│  │  │   ├── StatefulSet: redis (sentinel mode)                               │ │
│  │  │   └── DaemonSet: ollama (GPU nodes only)                               │ │
│  │  ├── Namespace: monitoring                                                │ │
│  │  │   ├── Prometheus + Grafana                                             │ │
│  │  │   ├── Jaeger (distributed tracing)                                     │ │
│  │  │   └── ELK Stack (logging)                                              │ │
│  │  └── Ingress: nginx-ingress + cert-manager                                │ │
│  │                                                                           │ │
│  │  Requirements: 3+ nodes, GPU node pool for AI                             │ │
│  │  Best for: Multi-tenant SaaS, high availability                           │ │
│  └───────────────────────────────────────────────────────────────────────────┘ │
│                                                                                 │
│  MODEL 3: HYBRID (Enterprise with compliance requirements)                      │
│  ┌───────────────────────────────────────────────────────────────────────────┐ │
│  │  On-Premise:                          Cloud:                              │ │
│  │  ├── AI Platform Core                 ├── Cloud AI Providers              │ │
│  │  ├── PostgreSQL (sensitive data)      │   (OpenAI, Claude, etc.)          │ │
│  │  ├── Ollama (local inference)         ├── CDN (static assets)             │ │
│  │  └── MinIO (document storage)         └── Monitoring (optional)           │ │
│  │                                                                           │ │
│  │  Connection: VPN / Private Link / API Gateway                             │ │
│  │  Data policy: Sensitive data never leaves on-premise                      │ │
│  │  AI policy: Local model for sensitive, cloud for non-sensitive            │ │
│  └───────────────────────────────────────────────────────────────────────────┘ │
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 11.2 Scaling Strategy

```
COMPONENT SCALING:

┌────────────────────┬──────────────┬────────────────────────────────────┐
│ Component          │ Scale Type   │ Strategy                           │
├────────────────────┼──────────────┼────────────────────────────────────┤
│ API Server         │ Horizontal   │ HPA based on CPU/request count     │
│ Worker             │ Horizontal   │ Based on queue depth               │
│ AI Gateway         │ Horizontal   │ Based on request rate              │
│ Ollama (LLM)      │ Vertical+H   │ GPU nodes, model sharding          │
│ Embedding Service  │ Horizontal   │ Batch processing, GPU pool         │
│ PostgreSQL         │ Vertical     │ Read replicas for queries          │
│ Redis              │ Horizontal   │ Cluster mode for cache             │
│ MinIO              │ Horizontal   │ Distributed mode                   │
└────────────────────┴──────────────┴────────────────────────────────────┘
```

---

## 12. RECOMMENDED TECH STACK

### 12.1 Core Platform

| Layer | Technology | Version | Justification |
|-------|-----------|---------|---------------|
| **Runtime** | .NET 8 (LTS) | 8.0 | Performance, cross-platform, enterprise-grade |
| **API Framework** | ASP.NET Core | 8.0 | Mature, high-performance, middleware pipeline |
| **API Gateway** | YARP | 2.1 | .NET native, lightweight, configurable |
| **ORM** | EF Core | 8.0 | Migrations, LINQ, pgvector support |
| **CQRS/Mediator** | MediatR | 12.x | Pipeline behaviors, decoupling |
| **Validation** | FluentValidation | 11.x | Expressive, testable |
| **Background Jobs** | Hangfire / Quartz.NET | Latest | Reliable, dashboard, cron |
| **Logging** | Serilog | 3.x | Structured, multiple sinks |
| **Metrics** | OpenTelemetry | 1.x | Vendor-neutral observability |

### 12.2 AI & ML

| Component | Technology | Justification |
|-----------|-----------|---------------|
| **Local LLM** | Ollama | Simple API, model management, GPU support |
| **Cloud LLM** | OpenAI/Claude/Gemini | Quality, tool calling, large context |
| **Embedding** | nomic-embed-text (local) / text-embedding-3 (cloud) | Good quality/cost ratio |
| **Vector Store** | PostgreSQL + pgvector | Single DB, no extra infra |
| **STT (offline)** | Whisper.cpp / faster-whisper | Best offline accuracy |
| **TTS (offline)** | Piper TTS | Fast, multilingual |
| **Agent Framework** | Custom (built on platform) | Full control, no vendor lock |

### 12.3 Infrastructure

| Component | Technology | Justification |
|-----------|-----------|---------------|
| **Database** | PostgreSQL 16 | pgvector, RLS, schemas, mature |
| **Cache** | Redis 7 | Pub/sub, streams, caching |
| **Object Storage** | MinIO | S3-compatible, self-hosted |
| **Message Bus** | Redis Streams → RabbitMQ | Start simple, upgrade when needed |
| **Container** | Docker + Docker Compose | Development + small deploy |
| **Orchestration** | Kubernetes (production) | Scale, HA, rolling updates |
| **Reverse Proxy** | Nginx / Traefik | SSL, load balancing |
| **Monitoring** | Prometheus + Grafana | Industry standard |
| **Tracing** | Jaeger / Zipkin | Distributed tracing |

### 12.4 Frontend

| Component | Technology | Justification |
|-----------|-----------|---------------|
| **Framework** | Next.js 14 | SSR, App Router, React ecosystem |
| **Language** | TypeScript | Type safety, DX |
| **Styling** | TailwindCSS | Utility-first, fast |
| **State** | Zustand | Lightweight, simple |
| **Server State** | TanStack Query | Caching, sync, mutations |
| **UI Components** | shadcn/ui | Accessible, customizable |
| **Real-time** | SignalR / WebSocket | Streaming AI responses |

### 12.5 Recommended Patterns

| Pattern | Where | Why |
|---------|-------|-----|
| **Clean Architecture** | Module internals | Testability, dependency inversion |
| **CQRS** | Complex modules | Separate read/write optimization |
| **Event Sourcing** | Audit-critical flows | Full history, replay |
| **Saga Pattern** | Multi-step AI actions | Compensation, rollback |
| **Circuit Breaker** | External integrations | Resilience |
| **Outbox Pattern** | Event publishing | Reliable messaging |
| **Strangler Fig** | Migration from current | Gradual refactoring |
| **Feature Flags** | Plugin/tenant features | Safe rollout |
| **Repository Pattern** | Data access | Abstraction, testability |
| **Specification Pattern** | Complex queries | Composable filters |

---

## 13. ROADMAP PHÁT TRIỂN THEO GIAI ĐOẠN

### Phase 0: Foundation Refactoring (1-2 tháng)

**Mục tiêu:** Sửa lỗi hiện tại, tái cấu trúc thành modular monolith

```
Tasks:
├── Sửa tất cả lỗi compile (xem DIEM_BAT_HOP_LY.md)
├── Chọn 1 tên dự án (AIBaseFramework), đồng bộ toàn bộ
├── Tách solution thành multi-project:
│   ├── AIBaseFramework.SharedKernel
│   ├── AIBaseFramework.Identity
│   ├── AIBaseFramework.AIEngine
│   ├── AIBaseFramework.Knowledge
│   ├── AIBaseFramework.Persistence
│   └── AIBaseFramework.API (host)
├── Implement proper DI, interfaces, abstractions
├── Xóa duplicate Docker Compose, giữ 1 bộ
├── Setup CI/CD (GitHub Actions)
├── Viết Architecture Decision Records (ADRs)
└── Deliverable: Clean, buildable, testable codebase
```

### Phase 1: AI Engine Core (2-3 tháng)

**Mục tiêu:** Tách AI capabilities thành reusable engine

```
Tasks:
├── AI Provider Abstraction Layer (ILLMProvider interface)
├── Implement Ollama adapter (fix current bugs)
├── Implement OpenAI adapter
├── AI Gateway with routing logic
├── Embedding Engine (multi-provider)
├── RAG Pipeline refactoring (proper cosine similarity)
├── Prompt Engine (template-based, per-tenant)
├── Vector Search optimization (HNSW index)
├── Citation Engine
├── AI Memory (Redis-based session memory)
├── Event Bus (MediatR notifications + Redis Streams)
└── Deliverable: Working AI Engine usable by any module
```

### Phase 2: Agent Runtime & Tool Calling (2-3 tháng)

**Mục tiêu:** AI có thể thực hiện actions trên hệ thống

```
Tasks:
├── Tool Registry (JSON Schema-based tool definitions)
├── Tool Calling Engine (LLM → structured output → execution)
├── Agent Runtime (ReAct loop: Think → Act → Observe)
├── Action Execution Pipeline (validate → guard → execute → audit)
├── Permission-aware execution (RBAC check before action)
├── Safety Guardrails (dangerous action detection)
├── Human-in-the-loop approval workflow
├── Audit Trail (full logging of AI decisions)
├── Rollback/Compensation (Saga pattern)
├── RAG-SQL Engine (safe SQL generation + execution)
├── Multi-Agent orchestration (supervisor pattern)
└── Deliverable: AI Agent that can perform CRUD via natural language
```

### Phase 3: Plugin System & Integration Hub (2-3 tháng)

**Mục tiêu:** Extensible platform, external system integration

```
Tasks:
├── Plugin Runtime (load/unload, lifecycle, isolation)
├── Plugin SDK (NuGet package for plugin developers)
├── Plugin contracts (IAIPlugin, IToolProvider, IMetadataProvider)
├── Integration Hub (connector framework)
├── REST Connector (generic, config-driven)
├── MCP Server implementation (expose platform as MCP)
├── MCP Client (consume external MCP servers)
├── Webhook support (inbound + outbound)
├── OpenAPI auto-discovery (generate tools from Swagger)
├── Metadata ingestion pipeline
├── Domain understanding engine
├── First plugins: Document Q&A, RAG-SQL
└── Deliverable: Platform that external systems can plug into
```

### Phase 4: Multi-Tenant & Enterprise Features (2-3 tháng)

**Mục tiêu:** Production-ready multi-tenant platform

```
Tasks:
├── Tenant management (CRUD, configuration)
├── Row-Level Security implementation
├── Per-tenant AI configuration
├── Per-tenant plugin activation
├── Per-tenant prompt strategies
├── API Key management (for SDK consumers)
├── Usage metering & billing hooks
├── Cost tracking per tenant
├── Admin dashboard (platform-wide)
├── Tenant onboarding wizard
├── Configuration-driven architecture
├── Feature flags (per tenant)
└── Deliverable: Multi-tenant SaaS-ready platform
```

### Phase 5: Voice & Advanced AI (2-3 tháng)

**Mục tiêu:** Voice interface, advanced AI capabilities

```
Tasks:
├── Speech-to-Text integration (Whisper.cpp for offline)
├── Text-to-Speech integration (Piper TTS for offline)
├── Voice command pipeline
├── Real-time audio streaming (WebSocket/WebRTC)
├── Intent recognition from voice
├── Conversational AI (multi-turn voice)
├── Cloud STT/TTS fallback (OpenAI Whisper, Google)
├── AI Workflow Engine (multi-step orchestration)
├── Long-running task management
├── AI learning from interactions
├── Advanced context engineering
└── Deliverable: Voice-enabled AI copilot
```

### Phase 6: Scale & Production Hardening (2-3 tháng)

**Mục tiêu:** Enterprise-grade production deployment

```
Tasks:
├── Kubernetes deployment manifests
├── Horizontal Pod Autoscaling
├── GPU node pool for AI inference
├── Distributed caching (Redis Cluster)
├── Database read replicas
├── Comprehensive monitoring (Prometheus + Grafana)
├── Distributed tracing (OpenTelemetry + Jaeger)
├── Disaster recovery plan
├── Security audit & penetration testing
├── Performance benchmarking
├── Documentation (API docs, SDK docs, admin guide)
├── Compliance (SOC2, ISO27001 preparation)
└── Deliverable: Production-ready, scalable platform
```

---

## TỔNG KẾT

### Kiến trúc đề xuất tóm tắt

```
┌─────────────────────────────────────────────────────────────────┐
│                    AIBaseFramework Platform                       │
│                                                                  │
│  Architecture: Hybrid Modular Monolith + Plugin + Agent          │
│  Communication: Event-driven (internal) + REST/gRPC (external)   │
│  AI Strategy: Provider-agnostic, offline-first, cloud-optional   │
│  Tenant Model: Shared infra + logical isolation (RLS/Schema)     │
│  Extension: Plugin-based (NuGet packages, hot-loadable)          │
│  Integration: MCP + REST + Event Bus + Plugin SDK                │
│  Security: Zero Trust + RBAC/ABAC + AI Guardrails                │
│  Deployment: Docker (dev) → Kubernetes (prod)                    │
│                                                                  │
│  KEY PRINCIPLES:                                                 │
│  1. AI Engine là reusable core, không biết business domain       │
│  2. Business domain đến từ metadata + plugins                    │
│  3. Mọi hệ thống mới chỉ cần implement ISystemConnector         │
│  4. AI tự hiểu domain qua metadata ingestion                    │
│  5. Actions luôn qua permission + audit pipeline                 │
│  6. Offline-first: mọi thứ chạy được không cần internet         │
│  7. Cloud-optional: thêm cloud providers khi cần quality/scale   │
│  8. Plugin isolation: plugin lỗi không crash platform            │
│  9. Tenant isolation: data + config + AI context riêng biệt      │
│  10. Gradual evolution: monolith → extract services khi cần      │
└─────────────────────────────────────────────────────────────────┘
```

### Ưu tiên hành động ngay

1. **Sửa lỗi compile** (Phase 0) - không thể làm gì khác nếu code không build
2. **Tách multi-project** - tạo module boundaries ngay từ đầu
3. **AI Provider Abstraction** - interface ILLMProvider + Ollama adapter
4. **Event Bus** - MediatR notifications cho inter-module communication
5. **Tool Registry** - foundation cho Agent capabilities

---

*Tài liệu này là living document, sẽ được cập nhật theo tiến độ phát triển.*
