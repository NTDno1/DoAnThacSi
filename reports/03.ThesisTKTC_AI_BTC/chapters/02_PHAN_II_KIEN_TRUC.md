# II. MÔ HÌNH KIẾN TRÚC

## II.1. Mô hình kiến trúc hệ thống

### II.1.0. Tổng quan kiến trúc End-to-End

Hệ thống AI phục vụ Văn phòng số Bộ Tài Chính được xây dựng theo **kiến trúc Microservices kết hợp AI Layer** trên nền tảng SmartOffice Backend Core (.NET 10), tuân thủ Quyết định số 258/QĐ-BTC và định hướng Phần mềm Văn phòng số toàn ngành năm 2026. Kiến trúc quản lý văn bản và điều hành hiện hữu (Chương trình VBĐH) được giữ nguyên làm nền tảng nghiệp vụ, bổ sung **một tầng AI độc lập (AI Layer)** đóng vai trò "bộ não mở rộng" cho toàn hệ thống, có khả năng tương tác với mọi phân hệ nghiệp vụ thông qua API Gateway, Event Bus và MCP Server.

```
┌───────────────────────────────────────────────────────────────────────────┐
│                       LỚP NGƯỜI DÙNG (Presentation)                       │
│  Web Portal · Mobile App · Email/SMS · Microsoft Teams / Zalo Connect    │
└───────────────────────────────┬───────────────────────────────────────────┘
                                │
                                ▼
┌───────────────────────────────────────────────────────────────────────────┐
│                BFF GATEWAY (.NET) + Kong API Gateway                      │
│         AuthN/AuthZ · Rate-limit · Routing · Aggregation · Tracing        │
└───────────────────────────────┬───────────────────────────────────────────┘
                                │
                ┌───────────────┼───────────────┐
                ▼               ▼               ▼
        ┌──────────────┐ ┌──────────────┐ ┌──────────────────┐
        │  Identity    │ │ Organization │ │  Office Manage   │
        │  Service     │ │  Service     │ │  Service (DMS)   │
        └──────────────┘ └──────────────┘ └──────────────────┘
                │               │               │
                ▼               ▼               ▼
        ┌──────────────────────────────────────────────────┐
        │                EVENT BUS (Kafka)                  │
        │     + Outbox Pattern + MassTransit + Dead-Letter  │
        └──────────────────────────────────────────────────┘
                                │
                                ▼
┌───────────────────────────────────────────────────────────────────────────┐
│                         AI LAYER (MỚI)                                   │
│  AI Gateway · AI Engine Core · Agent Orchestrator · RAG · OCR · LLM     │
│  Provider Abstraction · MCP Server · Audit & Observability · AI Jobs    │
└──────┬────────────────┬────────────────┬────────────────┬─────────────────┘
       │                │                │                │
       ▼                ▼                ▼                ▼
┌─────────────┐  ┌──────────────┐ ┌──────────────┐ ┌─────────────────┐
│ PostgreSQL  │  │  pgvector /  │ │    Redis     │ │  MinIO / S3     │
│ (Multi-     │  │   Qdrant     │ │ (Cache/Sess) │ │  File Storage   │
│  schema)    │  │ (Embeddings) │ │              │ │                 │
└─────────────┘  └──────────────┘ └──────────────┘ └─────────────────┘
       │                              │                │
       └──────────────────┬───────────┴────────────────┘
                          │
                          ▼
┌───────────────────────────────────────────────────────────────────────────┐
│        LLM PROVIDERS · OpenTelemetry · ELK · Prometheus · Grafana        │
│   OpenAI · Ollama · Anthropic · Local GPU · Azure OpenAI (optional)       │
└───────────────────────────────────────────────────────────────────────────┘
```

### II.1.1. Mục tiêu kiến trúc

Mục tiêu của kiến trúc Hệ thống AI là **xây dựng một nền tảng kỹ thuật thống nhất, có khả năng phục vụ toàn bộ vòng đời nghiệp vụ Văn phòng số**, từ tiếp nhận thông tin, xử lý nghiệp vụ, phê duyệt, ký số, phát hành, giao việc, theo dõi thực hiện đến lưu trữ, tra cứu, khai thác dữ liệu và hỗ trợ quyết định bằng AI.

Kiến trúc phải giải quyết đồng thời **hai yêu cầu cốt lõi**:

1. **Đặc thù nhà nước**: Hệ thống phải bảo đảm các quy trình nghiệp vụ đặc thù của cơ quan nhà nước, trong đó văn bản và hồ sơ được xử lý theo cơ cấu tổ chức, chức danh, thẩm quyền và luồng phê duyệt; tuân thủ pháp luật về văn thư, lưu trữ, an toàn thông tin, chữ ký số.
2. **Quy mô lớn**: Phục vụ số lượng lớn đơn vị và người sử dụng (khoảng 60.000 user) mà không tạo ra các hệ thống kỹ thuật riêng biệt khó quản lý cho từng đơn vị.

Theo phạm vi mới, Hệ thống AI hướng tới:

- Quản lý **thống nhất, tập trung xuyên suốt toàn ngành Tài Chính**, triển khai cho **các đơn vị từ Trung ương đến địa phương**.
- Quy mô tổ chức có sự khác biệt đáng kể giữa các khối: cơ quan Bộ, Thuế, Kho bạc Nhà nước, Hải quan, Ủy ban Chứng khoán Nhà nước, Dự trữ, Thống kê, BHXH Việt Nam.
- Có đơn vị tổ chức hai cấp, ba cấp với số lượng người sử dụng rất khác nhau.
- Vì vậy, kiến trúc **không được phụ thuộc cứng vào một mô hình tổ chức cụ thể**.

**Các mục tiêu trọng tâm của kiến trúc AI gồm:**

| # | Mục tiêu | Cách hiện thực |
|---|---|---|
| 1 | Quản trị tập trung nhưng có khả năng phân cấp | Triển khai theo cụm (cluster) region Trung ương – địa phương, multi-tenant isolation theo tenant_id |
| 2 | Tách biệt các miền nghiệp vụ | Microservices theo bounded-context (Identity, Organization, OfficeManage, AI Platform, Notification, Schedule, ...) |
| 3 | Chuẩn hóa cơ chế tích hợp | API Gateway (Kong), Event Bus (Kafka), MCP Server, OpenAPI/Swagger, Outbox pattern |
| 4 | An toàn và kiểm soát truy cập xuyên suốt | OpenIddict + JWT, RBAC/ABAC, Row-Level Security trên PostgreSQL, audit log mọi request AI |
| 5 | HA và scale-out | Kubernetes-ready, stateless API, Redis cache, PostgreSQL read-replica, Vector DB cluster |
| 6 | Multi-tenant linh hoạt | Schema-per-tenant hoặc Row-Level Security, Tenant-id định danh mọi request |
| 7 | Mở rộng AI thông minh | Plugin SDK cho MCP tool, Multi-Provider (OpenAI, Ollama, Anthropic, ...), RAG pipeline có cấu hình |
| 8 | Báo cáo và trực quan hóa | AI Admin Console, Dashboard KPI AI (độ chính xác, latency, throughput, chi phí token) |

### II.1.2. Nguyên tắc thiết kế kiến trúc

| # | Nguyên tắc | Mô tả |
|---|---|---|
| 1 | **Single Responsibility** | Mỗi microservice chỉ chịu trách nhiệm một bounded context rõ ràng. |
| 2 | **Loose Coupling** | Service giao tiếp qua API + Event Bus; không gọi trực tiếp DB của nhau. |
| 3 | **High Cohesion** | Code liên quan một nghiệp vụ đặt cùng module/folder theo Vertical Slice. |
| 4 | **Clean Architecture** | Phân lớp rõ: Domain – Application – Infrastructure – API. |
| 5 | **Outbox-first** | Mọi sự kiện xuất bản qua Outbox để đảm bảo reliable delivery. |
| 6 | **CQRS** | Tách riêng luồng Command (ghi) và Query (đọc) để tối ưu hiệu năng và bảo mật. |
| 7 | **API-First** | Mọi chức năng công khai đều có OpenAPI spec chuẩn, là cơ sở cho SDK/Portal. |
| 8 | **Stateless API** | Stateless để scale-out dễ dàng, state lưu trong Redis/PostgreSQL. |
| 9 | **Domain-Driven Design** | Bounded context, Aggregate Root, Domain Event. |
| 10 | **Plugin-based** | Mở rộng tính năng AI qua plugin mà không cần sửa lõi. |
| 11 | **Provider Agnostic** | Không phụ thuộc vào một LLM provider, có lớp abstraction. |
| 12 | **Offline First** | Ưu tiên các giải pháp chạy on-premise (Ollama local) khi cần bảo mật/suverain dữ liệu. |
| 13 | **Observability by Default** | OpenTelemetry + ELK + Distributed Tracing cho mọi service. |
| 14 | **Security by Design** | Zero-trust, mTLS nội bộ, JWT rotation, RBAC + ABAC, audit log mọi action. |
| 15 | **Configuration as Code** | Mọi config Kubernetes + Helm + .env được version hóa trên Git. |

### II.1.3. Kiến trúc tổng thể

Hệ thống AI được tổ chức theo **kiến trúc phân lớp 7 tầng** mở rộng từ kiến trúc tổng thể Văn phòng số, đảm bảo tính module hóa, khả năng mở rộng và bảo mật:

```
┌─────────────────────────────────────────────────────────────────────┐
│  Tầng 7 · Presentation Layer (Web Portal / Mobile / ChatOps)        │
│  Next.js 14 · React 18 · Ant Design · TailwindCSS · Chat UI · PWA   │
└─────────────────────────────────┬───────────────────────────────────┘
                                  │
┌─────────────────────────────────▼───────────────────────────────────┐
│  Tầng 6 · API Composition Layer (BFF Gateway + Kong)               │
│  BFF (.NET) · Kong · AuthN/AuthZ · Rate-limit · Aggregation        │
└─────────────────────────────────┬───────────────────────────────────┘
                                  │
┌─────────────────────────────────▼───────────────────────────────────┐
│  Tầng 5 · AI Gateway Layer ← MỚI                                   │
│  Request Router · Prompt Builder · Provider Selector · Cache        │
└─────────────────────────────────┬───────────────────────────────────┘
                                  │
┌─────────────────────────────────▼───────────────────────────────────┐
│  Tầng 4 · Agent Orchestration Layer ← MỚI                          │
│  Intent Router · Task Planner · Multi-Agent Coordinator · HITL      │
└─────────────────────────────────┬───────────────────────────────────┘
                                  │
┌─────────────────────────────────▼───────────────────────────────────┐
│  Tầng 3 · AI Engine Core ← MỚI                                     │
│  RAG Pipeline · LLM Inference · OCR · Embedding · Re-ranking       │
│  Context Aggregation · Faithfulness Check · Citation Extraction     │
└─────────────────────────────────┬───────────────────────────────────┘
                                  │
┌─────────────────────────────────▼───────────────────────────────────┐
│  Tầng 2 · Provider Abstraction Layer ← MỚI                         │
│  OpenAI · Ollama · Anthropic · Azure OpenAI · Cohere · Bedrock     │
└─────────────────────────────────┬───────────────────────────────────┘
                                  │
┌─────────────────────────────────▼───────────────────────────────────┐
│  Tầng 1 · Platform Core (Backend SmartOffice .NET)                  │
│  Identity · Organization · OfficeManage · Files · MasterData ·      │
│  Notification · Schedule · Permission · Document · Workflow         │
│  PostgreSQL · Redis · Kafka · MinIO · MCP Server                    │
└─────────────────────────────────────────────────────────────────────┘
```

### II.1.4. Kiến trúc ứng dụng

Mỗi microservice được tổ chức theo **Clean Architecture + Vertical Slice** trong `qlvb-backend-core`:

```
+---------------------- SmartOffice.<Module>.Api ----------------------+
| Controllers · Middleware · OpenAPI/Scalar · HealthChecks · WAF       |
+---------------------------------+------------------------------------+
                                  |
                                  ▼
+--------------------- SmartOffice.<Module>.Application ---------------+
| Commands · Queries · Validators (FluentValidation)                   |
| Pipeline Behaviors · Mapping (Mediator Source Generator)             |
| ICacheService · IAppDbContext · IEventBus                            |
+---------------------------------+------------------------------------+
                                  |
                                  ▼
+-------------------- SmartOffice.<Module>.Infrastructure ------------+
| ApplicationDbContext (EF Core + Npgsql)                              |
| CacheService (Redis) · OutboxWriter · EventBus (MassTransit)         |
| External API Adapters · OpenTelemetry · JWT validation               |
+---------------------------------+------------------------------------+
                                  |
                                  ▼
+-------------------- SmartOffice.<Module>.Domain ---------------------+
| Entities · Aggregate Roots · Value Objects · Domain Events           |
| Result<T> · Errors · Exceptions · Enums                              |
+----------------------------------------------------------------------+
```

**Các đặc điểm nổi bật:**

- **Vertical Slice**: Mỗi feature/Use case là một slice độc lập, gồm Command + Handler + Validator + Test.
- **Mediator Source Generator**: Không dùng reflection, sinh mã tại compile-time → hiệu năng cao.
- **Outbox Pattern**: Toàn bộ sự kiện nghiệp vụ (DocumentCreated, WorkflowAdvanced, AIJobCompleted, ...) được ghi vào outbox table trong cùng transaction với business state change → đảm bảo exactly-once delivery.
- **BFF cho frontend**: Tổng hợp query từ nhiều service, giảm round-trip.

**Module phục vụ AI (mở rộng mới):**

| Module | Mô tả |
|---|---|
| `SmartOffice.AIPlatform.Api` | API cho AI Gateway và AI Engine Core |
| `SmartOffice.AIPlatform.Application` | CQRS handlers, validators cho AI use case |
| `SmartOffice.AIPlatform.Infrastructure` | OpenAI/Ollama client, pgvector, MCP tools |
| `SmartOffice.AIPlatform.Domain` | Entity: `AIJob`, `AIJobStep`, `KnowledgeChunk`, `PromptTemplate`, `Tool`, `Agent`, `Conversation`, `Feedback` |
| `SmartOffice.AIPlatform.UnitTests` | xUnit v3 + FluentAssertions + NSubstitute |
| `SmartOffice.AIPlatform.IntegrationTests` | Testcontainers (PostgreSQL + Ollama local) |

### II.1.5. Kiến trúc dịch vụ

**Bảng microservice hiện hữu và mở rộng:**

| Service | Schema DB | Trách nhiệm |
|---|---|---|
| `Identity API` | `identity` | OpenIddict, User, Role, Permission, Refresh Token, MFA |
| `Organization API` | `organization` | Đơn vị, phòng ban, chức danh, cơ cấu tổ chức, tenant management |
| `Office Manage API` (DMS) | `office_manage` | Văn bản đến/đi/nội bộ, hồ sơ công việc, workflow phê duyệt |
| `Files Management API` | `files_management` | Upload/download, ký số, anti-virus, OCR pipeline input |
| `Master Data API` | `master_data` | Danh mục dùng chung: loại văn bản, độ mật, lĩnh vực, ... |
| `Permission API` | (embedded) | RBAC/ABAC phân quyền |
| `Notification API` | `notification` | Gửi email, SMS, push notification, NotificationHub |
| `BFF Gateway` | – | Tổng hợp API cho FE |
| `Kong Gateway` | – | Routing, auth, rate-limit, plugin |
| **`AI Platform API` (mới)** | `ai_platform` | AI Gateway, AI Engine Core, Agent, Knowledge Base |
| **`MCP Server` (mới)** | – | Triển khai Model Context Protocol để expose tool tới Agent |
| **Frontend Services** | – | Next.js Web Portal, Admin Console, Mobile App |

**Giao tiếp giữa service:**

- **Synchronous**: REST (OpenAPI), gRPC (cho luồng AI Engine ↔ Internal Service yêu cầu latency thấp).
- **Asynchronous**: Event Bus Kafka, đảm bảo qua Outbox Pattern.
- **AI-specific**: MCP (JSON-RPC) cho Agent ↔ Tool/Resource.
- **File channel**: WebSocket / Server-Sent Events (SSE) cho streaming response từ LLM.

### II.1.6. Kiến trúc tích hợp

```
              ┌─────────────────────────┐
              │      AI Platform        │
              │  ┌───────────────────┐  │
              │  │   MCP Server       │  │ ← expose tools: get_document, search_doc,
              │  └───────────────────┘  │   summarize, ocr_extract, list_user_tasks
              └────────────┬────────────┘
                           │ MCP protocol
   ┌───────────────────┐   │   ┌─────────────────────┐
   │   Agent Core      │◄──┘   │  External AI Tool   │
   └─────────┬─────────┘       └─────────────────────┘
             │
             ▼
   ┌──────────────────────────────────────────────────────┐
   │              INTEGRATION HUB                         │
   │  ERP · DMS · HRM · Kho bạc · Thuế · Hải quan · BHXH   │
   │  MCP · REST · gRPC · Webhook · Kafka · Email/SMS     │
   └──────────────────────────────────────────────────────┘
```

**Các hệ thống liên thông:**

| Hệ thống liên thông | Giao thức | Chuẩn dữ liệu | Loại dữ liệu |
|---|---|---|---|
| Cổng DVCQG (Cổng dịch vụ công quốc gia) | REST + OAuth2 | JSON theo Cổng DVCQG | Hồ sơ, kết quả |
| Hệ thống Một cửa điện tử cấp Bộ | REST | JSON + PDF/A | Văn bản, hồ sơ |
| Hệ thống Quản lý văn bản chuyên ngành (Thuế, Hải quan, KBNN, ...) | REST, Kafka | XML/JSON theo QĐ 258 | Văn bản, metadata |
| Hệ thống Cơ sở dữ liệu quốc gia về công dân / doanh nghiệp | gRPC / REST | JSON | Định danh |
| Hệ thống VNPT-CA / Viettel-CA / FPT-CA (chữ ký số) | REST + XML-Sig | XML-Signature | Ký số |
| Hệ thống Email Bộ (Exchange/365) | SMTP/IMAP + Graph API | MIME | Công văn gửi/nhận |
| Microsoft Teams / Zalo OA | Webhook + Bot Framework | JSON | Thông báo nội bộ |
| Hệ thống DMS của đơn vị trực thuộc (Cục/Vụ/Sở) | REST + Kafka | JSON | Đồng bộ văn bản |
| Cổng CSDL của Kho bạc | gRPC + Protobuf | Protobuf | Ngân sách, thanh toán |
| Hệ thống ERP/HRM của Bộ | OData + REST | JSON | Nhân sự, chấm công |
| OpenAI API | HTTPS REST | OpenAI JSON | LLM, embedding |
| Ollama (local) | HTTP REST | Ollama API JSON | LLM local, embedding |

**Tiêu chí tích hợp:**
- **Chuẩn hóa contract**: OpenAPI 3.0 / AsyncAPI 2.0 → sinh SDK.
- **Idempotency**: Mọi request đều có `Idempotency-Key` để retry an toàn.
- **Retry + Dead Letter**: Đảm bảo message không mất.
- **Schema registry**: Cho Kafka topic để versioning.
- **Observability**: Trace mọi request AI xuyên qua nhiều service.

### II.1.7. Kiến trúc dữ liệu

**Sơ đồ ER tổng thể (rút gọn – đầy đủ ở file `docs/database/`):**

```
┌────────────────┐ 1    * ┌───────────────────────┐ 1    * ┌──────────────────────┐
│   tenant       │────────│   organization_unit   │────────│       user           │
└────────────────┘        └───────────────────────┘        └──────────────────────┘
        │                              │                              │
        │                              │ *                          │
        │                              ▼                              ▼
        │                    ┌───────────────────────┐   ┌──────────────────────┐
        │                    │   position / title    │   │      user_role       │
        │                    └───────────────────────┘   └──────────────────────┘
        │
        │ 1
        ▼ *
┌───────────────────────┐ 1    * ┌────────────────────────┐ 1    * ┌─────────────────────┐
│      document         │────────│   document_chunk       │────────│ document_embedding  │
│ (text + metadata)     │        │  (chunked text)        │        │  (pgvector / qd)    │
└───────────────────────┘        └────────────────────────┘        └─────────────────────┘
        │                                                                  │
        │ *                                                                │ *
        ▼ 1                                                                ▼ 1
┌───────────────────────┐                                       ┌──────────────────────┐
│   document_version    │                                       │   embedding_model    │
└───────────────────────┘                                       └──────────────────────┘

                ┌──────────────────────────────────────────────────┐
                │                 ai_platform                      │
                ├──────────────────────────────────────────────────┤
                │ ai_job, ai_job_step, ai_job_artifact             │
                │ conversation, conversation_message, feedback     │
                │ prompt_template, prompt_version                  │
                │ agent, agent_tool, tool_invocation               │
                │ tool, tool_version, tool_argument_schema         │
                │ knowledge_source, knowledge_ingestion_log        │
                │ audit_log (ai_action, prompt, response)           │
                │ budget, token_usage, model_benchmark             │
                └──────────────────────────────────────────────────┘
```

**Bảng dữ liệu chính của AI Platform (lược đồ rút gọn):**

| Bảng | Mục đích |
|---|---|
| `ai_job` | Mỗi yêu cầu AI: `summarize`, `classify`, `extract`, `rag_chat`, `ocr`. Có `status`, `payload`, `result`, `created_at`. |
| `ai_job_step` | Các bước xử lý của một job (embedding → retrieve → rerank → generate → validate). |
| `ai_job_artifact` | Kết quả trung gian (trích dẫn, embedding cached, file OCR). |
| `prompt_template` | Template prompt có versioning, đa ngôn ngữ, đa tenant. |
| `prompt_version` | Lịch sử thay đổi prompt → A/B test. |
| `conversation` | Phiên hội thoại người dùng–AI. |
| `conversation_message` | Tin nhắn user / assistant / tool. |
| `feedback` | Đánh giá của user (like/dislike, comment) → dùng để fine-tune. |
| `agent` | Định nghĩa agent (role, instruction, model, tools, max_steps). |
| `tool` | Định nghĩa tool (MCP tool): name, schema, endpoint, auth. |
| `agent_tool` | Quan hệ agent ↔ tool. |
| `tool_invocation` | Lịch sử gọi tool: arguments, response, latency, error. |
| `knowledge_source` | Nguồn tri thức: loại (DMS, HRM, ERP, web, file), connection string. |
| `knowledge_ingestion_log` | Log nhập tri thức: số chunk, thời gian, lỗi. |
| `audit_log` | Mọi action AI: ai_action, prompt_input, response_output, model, tenant_id, user_id. |
| `token_usage` | Theo dõi số token/chi phí theo tenant, model, ngày. |
| `model_benchmark` | Kết quả đánh giá model (RAGAS): precision, recall, faithfulness, latency. |

**PostgreSQL Multi-Schema:**

| Schema | Phạm vi | Sở hữu |
|---|---|---|
| `identity` | User, Role, Claim, Token | Identity service |
| `organization` | Tenant, Department, Position | Organization service |
| `office_manage` | Document, Version, Workflow, Routing | OfficeManage/DMS service |
| `master_data` | Loại văn bản, độ mật, lĩnh vực | MasterData service |
| `files_management` | File metadata, attachment | FilesManagement service |
| `notification` | Template, log | Notification service |
| `schedule` | Lịch họp, phòng họp, xe | Schedule service |
| `permission` | (RBAC) | Permission service |
| **`ai_platform`** | **AI Job, Knowledge, Conversation, Audit** | **AI Platform service** |
| `public` | `__EFMigrationsHistory`, extension `vector`, ... | Platform-wide |

### II.1.8. Kiến trúc hạ tầng và triển khai

**Sơ đồ triển khai (Dev/Stage/Prod):**

```
                 ┌──────────────── INTERNET ─────────────────┐
                          │
                          ▼
                ┌─────────────────────┐
                │  WAF / CDN          │
                │  Cloudflare/        │
                │  F5 BIG-IP          │
                └──────────┬──────────┘
                           │
                ┌──────────▼──────────┐
                │  Kong API Gateway   │
                │  + BFF (.NET)       │
                └──────────┬──────────┘
                           │
        ┌──────────────────┼─────────────────────┐
        │                  │                      │
        ▼                  ▼                      ▼
┌───────────────┐  ┌───────────────────┐  ┌────────────────┐
│ K8s cluster:  │  │ K8s cluster:      │  │ K8s cluster:   │
│ Core services │  │ AI Platform       │  │ Observability  │
│ - Identity    │  │ - AI Gateway      │  │ - Prometheus   │
│ - Org         │  │ - AI Engine Core  │  │ - Grafana      │
│ - Office Mgmt │  │ - Agent Orchestr. │  │ - ELK Stack    │
│ - Files       │  │ - RAG Pipeline    │  │ - Jaeger       │
│ - Schedule    │  │ - MCP Server      │  │ - Alertmanager │
│ - Notif       │  │ - OCR Workers     │  └────────────────┘
└──────┬────────┘  └─────────┬─────────┘
       │                      │
       └──────────┬───────────┘
                  │
       ┌──────────▼──────────┐
       │ Data Plane          │
       │ - PostgreSQL HA     │
       │   + pgvector        │
       │ - Redis Sentinel    │
       │ - Kafka cluster     │
       │ - MinIO cluster     │
       │ - GPU nodes (LLM)   │
       └─────────────────────┘
```

**Thông số hạ tầng tham chiếu (cho 60.000 user):**

| Thành phần | Cấu hình đề xuất | Ghi chú |
|---|---|---|
| **Kong API Gateway** | 3 instance (HA, 4 vCPU/8 GB RAM mỗi node) | Reverse proxy + WAF + rate-limit |
| **BFF Gateway** | 3 instance (.NET 10, 4 vCPU/8 GB) | Stateless, scale-out |
| **Identity API** | 2 instance (2 vCPU/4 GB) | Có thể scale lên 5 nếu login nhiều |
| **Organization API** | 3 instance (2 vCPU/4 GB) | |
| **Office Manage API** | 6 instance (4 vCPU/8 GB) | Service chính xử lý văn bản |
| **Files Management API** | 3 instance (4 vCPU/8 GB) | Upload/download |
| **AI Gateway** | 4 instance (4 vCPU/8 GB) | Stateless, high-throughput |
| **AI Engine Core** | 4 instance + 2 GPU node (NVIDIA A10/L4) | Chạy Ollama local + inference CPU-bound |
| **Agent Orchestrator** | 3 instance (4 vCPU/8 GB) | Stateful in Redis |
| **MCP Server** | 2 instance (2 vCPU/4 GB) | Stateless, đáp ứng JSON-RPC |
| **OCR Workers** | 2-4 instance CPU; có thể scale GPU khi xử lý PDF scan nặng | Apache Tika + Tesseract hoặc PaddleOCR |
| **PostgreSQL Primary** | 16 vCPU/64 GB RAM / 2 TB NVMe | Multi-schema, Row-Level Security |
| **PostgreSQL Replica** | 2 node read-replica (16 vCPU/64 GB) | Read scale-out |
| **Redis Cluster** | 6 node (3 master + 3 replica) | Session, cache, rate-limit |
| **Kafka Cluster** | 3 broker (4 vCPU/16 GB) | Event bus, outbox |
| **MinIO** | 4 node (4 vCPU/16 GB / 8 TB) | S3-compatible, lưu file OCR, snapshot |
| **GPU node (LLM local)** | 1-2 node A10/L4 (24 GB VRAM) | Ollama + Qwen2.5-7B-Instruct, nomic-embed-text |
| **ELK Stack** | Elasticsearch 3-node, Kibana 1-node, APM 1-node | Theo README |
| **Prometheus + Grafana** | 1 node Prometheus + 1 node Grafana | Metrics + dashboard |

**Cấu hình mạng:**

- **Internal**: mTLS giữa các service (Istio service mesh).
- **External**: HTTPS only, HSTS, TLS 1.3.
- **DNS**: Internal cluster DNS, external qua Cloudflare/DNS quốc gia.
- **Rate limit**: Kong áp dụng rate-limit (vd: 100 req/phút/user, 10.000 req/phút/tenant).

**Chiến lược triển khai:**

| Môi trường | Mục đích | Cách triển khai |
|---|---|---|
| **Local-dev** | Dev cá nhân | Docker Compose (`docker/local-env/docker-compose.yml`) |
| **Dev** | Chia sẻ giữa team | Docker Compose trên server dev (`docker/dev-env/docker-compose.yml`) |
| **Staging** | Test hệ thống | Kubernetes (single cluster, low resource) |
| **Prod (Trung ương)** | Chính thức Bộ | Kubernetes multi-region, HA |
| **Prod (địa phương)** | Các đơn vị trực thuộc | Kubernetes cluster theo đơn vị (tuỳ) hoặc site-to-site VPN về Trung ương |

**CI/CD pipeline:**

```
Git push → GitHub Actions / GitLab CI
   → Build → Unit tests → SonarQube → Container build (Docker)
   → Push image → Dev env auto-deploy
   → Manual approval → Staging deploy
   → UAT run → Manual approval → Prod deploy (blue/green hoặc canary)
```

### II.1.9. Kiến trúc an toàn, bảo mật

**Mô hình bảo mật Zero-Trust:**

```
┌──────────────────────────────────────────────────────────────┐
│              EXTERNAL (Internet / User)                      │
└────────────────────────┬─────────────────────────────────────┘
                         │ HTTPS + WAF + DDoS
┌────────────────────────▼─────────────────────────────────────┐
│            LỚP 1: EDGE SECURITY                              │
│  WAF (OWASP rules) · DDoS · Bot protection · Geo-allowlist  │
└────────────────────────┬─────────────────────────────────────┘
                         │ mTLS
┌────────────────────────▼─────────────────────────────────────┐
│            LỚP 2: API SECURITY                               │
│  Kong · JWT verify · Rate-limit · Scope check · Audit log   │
└────────────────────────┬─────────────────────────────────────┘
                         │ mTLS (service-to-service)
┌────────────────────────▼─────────────────────────────────────┐
│            LỚP 3: SERVICE SECURITY                           │
│  OpenIddict validation · FluentValidation                    │
│  Authorization handler (RBAC + ABAC) · Tenant isolation     │
└────────────────────────┬─────────────────────────────────────┘
                         │ Encrypted channel
┌────────────────────────▼─────────────────────────────────────┐
│            LỚP 4: DATA SECURITY                              │
│  PostgreSQL RLS (tenant_id, role) · pgcrypto for PII        │
│  pgvector namespace · MinIO encryption at-rest + in-transit │
│  Backup encrypted · Token encryption (Fernet/AES-GCM)        │
└──────────────────────────────────────────────────────────────┘
```

**Các biện pháp cụ thể:**

| Lớp | Biện pháp |
|---|---|
| **Xác thực (AuthN)** | OpenIddict + OAuth2/OIDC, JWT (RS256), MFA bắt buộc cho admin, hỗ trợ SSO với Cổng DVCQG, refresh-token rotation, JWKS public key rotation |
| **Phân quyền (AuthZ)** | RBAC (vai trò: Lãnh đạo Bộ, Lãnh đạo đơn vị, Văn thư, Chuyên viên, Admin, Auditor) + ABAC (theo thuộc tính: đơn vị, lĩnh vực, độ mật, thời gian, IP) |
| **Tenant Isolation** | Row-Level Security trên PostgreSQL: `USING (tenant_id = current_setting('app.tenant_id')::uuid)`. Mọi query PHẢI kèm `SET app.tenant_id`. |
| **API Security** | HTTPS only, HSTS, CORS whitelist, CSP header, API key + IP allowlist cho inter-service, Idempotency-Key chống replay |
| **Audit & Logging** | Mọi action AI (query, response, model, latency, token) ghi vào `audit_log`. Không log PII hay nội dung văn bản mật. |
| **Encryption** | TLS 1.3 in-transit; AES-256 at-rest (PostgreSQL TDE, MinIO SSE-S3); Fernet/AES-GCM cho secret trong env |
| **PII / Pháp luật** | Tuân thủ Nghị định 13/2023/NĐ-CP (bảo vệ dữ liệu cá nhân), Luật An toàn thông tin mạng 2015, Luật An ninh mạng 2018 |
| **Secret management** | HashiCorp Vault / Kubernetes Secrets (encrypted) — KHÔNG hardcode, KHÔNG commit |
| **Backup & Recovery** | PostgreSQL: pg_basebackup daily + WAL streaming; MinIO: cross-region replication; backup retention 30 ngày, quarterly DR drill |
| **Anti-virus / EDR** | Files API scan với ClamAV trước khi nhập vào kho tri thức |
| **Rate limiting** | Kong: 100 req/phút/user, 10.000 req/phút/tenant, 50 req/giây/global |

**Bảo mật AI đặc thù:**

- **Prompt injection defense**: Lọc, validate input → chặn lệnh prompt injection.
- **Output validation**: Validate cấu trúc JSON (function calling), length, không chứa PII.
- **Sandbox**: Môi trường chạy LLM/Agent cách ly (container, không có quyền truy cập prod data khi test).
- **Model versioning**: Theo dõi version LLM, roll-back khi có vấn đề.
- **Audit và reproducible**: Lưu toàn bộ (prompt, response, model, params) phục vụ điều tra.
- **Chống rò rỉ dữ liệu**: Không gửi dữ liệu mật (văn bản "Mật", "Tối mật", "Tuyệt mật") sang OpenAI public cloud. Phải dùng Ollama local hoặc Azure OpenAI (private endpoint).

### II.1.10. Giám sát và vận hành

**Ba trụ cột observability:**

| Trụ cột | Công cụ | Mục đích |
|---|---|---|
| **Logs** | ELK Stack (Elasticsearch, Logstash, Kibana) + OpenTelemetry Collector | Thu thập log structured (JSON) từ tất cả service. |
| **Metrics** | Prometheus + Grafana + Alertmanager | Thu thập metric hệ thống (CPU, RAM, latency p95/p99, throughput, error rate, token usage, cost). |
| **Traces** | Jaeger / Elastic APM | Distributed tracing toàn bộ request → vẽ call graph xuyên service. |

**Dashboard & Alert quan trọng (ví dụ):**

| Dashboard | Chỉ số chính | Alert ngưỡng |
|---|---|---|
| **Core Services Health** | Request rate, p95 latency, error rate per service | Error > 1% trong 5 phút |
| **AI Pipeline** | Job pending, Job processing, Job success/failure rate, p95 latency, token/s | Queue length > 10.000 trong 5 phút |
| **RAG Quality (RAGAS)** | Context Precision, Context Recall, Faithfulness, Answer Relevancy | Recall giảm > 10% week-over-week |
| **Cost Dashboard** | Chi phí token theo tenant, theo model, theo ngày | Chi phí tenant > budget |
| **Security Dashboard** | Failed login rate, suspicious prompt, anomalous access | > 50 failed login/phút/user |
| **SLA Compliance** | Uptime %, MTTR, MTTD | Uptime < 99.9% tháng |

**Vận hành:**

- **On-call**: 24/7 theo ca, sử dụng PagerDuty / OpsGenie.
- **Runbook**: Cho mỗi alert nghiêm trọng có runbook + quy trình khắc phục.
- **Change management**: Mọi thay đổi phải qua CAB (Change Advisory Board), có rollback plan.
- **Capacity planning**: Dự báo tăng trưởng user/đơn vị 6 tháng tới → scale hạ tầng trước.
- **DR drill**: Test khôi phục sau sự cố mỗi quý.

### II.1.11. Khả năng mở rộng hệ thống

**Nguyên tắc mở rộng:**

1. **Horizontal scaling (ưu tiên)**: Thêm node thay vì nâng cấp node.
2. **Stateless services**: Mọi API có thể scale-out mà không cần session affinity.
3. **Stateless state lưu ở Redis/PostgreSQL**: Cho scale-out đồng đều.
4. **Database sharding**: Theo tenant_id khi quy mô vượt ngưỡng (Citus trên PostgreSQL).
5. **Vector DB**: pgvector → Qdrant cluster khi dung lượng embedding > 1 tỷ vector.

**Các chiều mở rộng cụ thể:**

| Chiều | Hiện tại | Khi cần mở rộng |
|---|---|---|
| **Số user** | 60.000 | Đến 200.000: scale Postgres read-replica, Redis shard. Đến 1 triệu: sharding theo tenant. |
| **Dung lượng văn bản** | ~10 triệu văn bản | Sử dụng pgvector scale-out hoặc Qdrant cluster. |
| **Embedding storage** | pgvector | Qdrant cluster riêng khi > 100M vector |
| **LLM throughput** | Ollama local + cache | Thêm GPU node, batching, async queue |
| **Số tenant** | Multi-tenant | Schema-per-tenant cho các tenant lớn |
| **Số Agent song song** | 100 worker | Tăng worker pool, có backpressure |
| **Geographical** | 1 vùng | Multi-region Trung ương – địa phương với active-active |

**Cơ chế backpressure & graceful degradation:**

- Khi LLM provider quá tải → chuyển sang provider dự phòng (OpenAI → Ollama).
- Khi Vector DB chậm → fallback tìm kiếm BM25.
- Khi OCR quá tải → queue với priority theo tenant VIP.
- Khi system down → trả lời dạng "AI tạm không khả dụng, vui lòng thử lại".

**CI/CD & Feature Flag:**

- **Feature flags** (Unleash / LaunchDarkly) để rollout từng nhóm tenant.
- **Canary release** cho phiên bản AI mới → 5% traffic → monitor → 100%.

## II.2. Mô hình phân rã chức năng

**(Hệ thống AI phục vụ Văn phòng số Bộ Tài Chính)**

```
                       ┌───────────────────────────────────┐
                       │   HỆ THỐNG AI - VĂN PHÒNG SỐ BTC  │
                       └────────────────┬──────────────────┘
                                        │
        ┌─────────────────┬──────────────┼──────────────┬─────────────────┐
        │                 │              │              │                 │
        ▼                 ▼              ▼              ▼                 ▼
┌───────────────┐ ┌───────────────┐ ┌─────────────┐ ┌─────────────┐ ┌──────────────┐
│  AI GATEWAY   │ │ AI ENGINE CORE│ │  AGENT      │ │ KNOWLEDGE   │ │    OCR &     │
│               │ │               │ │  ORCHESTR.  │ │  BASE (RAG) │ │   DOC EXTR.  │
└───────┬───────┘ └───────┬───────┘ └──────┬──────┘ └──────┬──────┘ └──────┬───────┘
        │                 │              │              │              │
   II.5.1            II.5.2          II.5.3          II.5.4           II.5.5

        ┌─────────────────┬──────────────┼──────────────┬─────────────────┐
        │                 │              │              │                 │
        ▼                 ▼              ▼              ▼                 ▼
┌───────────────┐ ┌───────────────┐ ┌─────────────┐ ┌─────────────┐ ┌──────────────┐
│  PROVIDER     │ │ AI JOB & TASK │ │ MCP SERVER  │ │  AUDIT &    │ │ AI ADMIN     │
│  ABSTRACTION  │ │    QUEUE      │ │             │ │ OBSERV.     │ │  CONSOLE     │
└───────┬───────┘ └───────┬───────┘ └──────┬──────┘ └──────┬──────┘ └──────┬───────┘
        │                 │              │              │              │
   II.5.6            II.5.7          II.5.8          II.5.9           II.5.10
        │
        ▼
   II.5.11 (Auth & Tenant for AI chia sẻ chung ngữ cảnh với các phân hệ trên)
```

| STT | Chức năng (Phân hệ AI) | Mô tả |
|---|---|---|
| 1 | **AI Gateway** | Cổng tiếp nhận mọi yêu cầu AI từ các phân hệ (Web, Mobile, BFF, Service khác). Thực hiện xác thực, phân quyền, rate-limit, prompt sanitization, chọn LLM provider, caching phản hồi, streaming response qua SSE/WebSocket. |
| 2 | **AI Engine Core** | Lõi xử lý AI: nhận request, xây prompt có versioning, gọi các module con (Embedding, Retrieve, Rerank, Generate, Validate), tổng hợp kết quả, ghi audit log. |
| 3 | **Agent Orchestration** | Tiếp nhận yêu cầu phức tạp cần nhiều bước (multi-step task). Có: Intent Router phân loại yêu cầu, Task Planner lập kế hoạch, Multi-Agent Coordinator phối hợp agent, Human-in-the-Loop chờ duyệt khi cần, Safety Guardrails chặn thao tác nguy hiểm. |
| 4 | **Knowledge Base & RAG** | Quản lý kho tri thức ngữ nghĩa: ingestion (chunking, embedding, indexing), retrieval (hybrid search BM25 + vector), re-ranking (cross-encoder), context aggregation, citation extraction, faithfulness check. |
| 5 | **OCR & Document Extraction** | Trích xuất văn bản từ ảnh/PDF scan: Detect (Tesseract/PaddleOCR/Surya), Layout-aware, Table extraction, Form extraction. Map dữ liệu về JSON để AI xử lý tiếp. |
| 6 | **Provider Abstraction (LLM)** | Lớp trừu tượng hóa nhà cung cấp LLM: hỗ trợ OpenAI (gpt-4o, gpt-4o-mini, text-embedding-3-*), Ollama (qwen2.5, llama3.2, nomic-embed-text, ...), Anthropic, Azure OpenAI. Có cơ chế failover, cost-based routing, token counting. |
| 7 | **AI Job & Task Queue** | Hàng đợi tác vụ AI (Kafka topic `ai.jobs`): gửi job, worker pool consume, retry, dead-letter, tracking trạng thái (pending → processing → done/failed). |
| 8 | **MCP Server** | Triển khai Model Context Protocol để expose các tool cho agent: get_document, search_doc, summarize, ocr_extract, list_user_tasks, create_worklog, ... Mỗi tool có JSON schema, xác thực riêng, audit riêng. |
| 9 | **Audit & Observability** | Ghi log mọi request AI: prompt input (ẩn PII), response, model, latency, token, user, tenant. Dashboard Grafana + Kibana để theo dõi chất lượng, chi phí, lạm dụng. |
| 10 | **AI Admin Console** | Giao diện quản trị cho Admin Bộ/đơn vị: quản lý Knowledge Source, Prompt Template, Agent, Tool, Model Benchmark, User feedback, Budget. |
| 11 | **Auth & Tenant cho AI** | Mở rộng module Identity sẵn có: cấp phát/quản lý quyền riêng cho AI (ai:summarize, ai:chat, ai:admin). Tenant isolation trong mọi query (Row-Level Security). |

## II.3. Mô hình tích hợp hệ thống

Xem chi tiết tại mục **II.1.6 Kiến trúc tích hợp**. Tóm tắt:

| Hệ thống liên thông | Giao thức liên thông | Chuẩn dữ liệu | Các loại dữ liệu liên thông |
|---|---|---|---|
| Cổng DVCQG | REST + OAuth2 | JSON theo Cổng DVCQG | Hồ sơ, kết quả, định danh |
| DMS chuyên ngành (Thuế, Hải quan, KBNN, ...) | REST + Kafka | QĐ 258, JSON | Văn bản đến/đi, metadata |
| Hệ thống Cơ sở dữ liệu quốc gia về công dân / DN | gRPC / REST | JSON | Định danh |
| CA (VNPT-CA, Viettel-CA, FPT-CA) | REST + XML-Sig | XML-Signature | Ký số |
| Email Bộ (Exchange/365) | SMTP/IMAP + Graph API | MIME | Công văn |
| Teams / Zalo OA | Webhook + Bot Framework | JSON | Thông báo |
| Hệ thống ERP/HRM Bộ | OData + REST | JSON | Nhân sự |
| OpenAI API | HTTPS REST | OpenAI JSON | LLM, embedding |
| Ollama (local GPU) | HTTP REST | Ollama API JSON | LLM, embedding |
| Kho bạc, DVC | gRPC + Protobuf | Protobuf | Ngân sách |
| Cổng thông tin điện tử Bộ | REST | JSON/XML | Công khai văn bản |

## II.4. Mô hình triển khai hệ thống

Xem chi tiết tại mục **II.1.8 Kiến trúc hạ tầng và triển khai**. Các môi trường triển khai:

1. **Local-dev**: Docker Compose cá nhân (postgres, redis, kafka, rabbit, các API backend) — theo `qlvb-backend-core/docker/local-env/docker-compose.yml`.
2. **Dev (chia sẻ)**: Docker Compose trên server dev — `qlvb-backend-core/docker/dev-env/docker-compose.yml`.
3. **Staging**: Kubernetes cluster low-resource để test integration.
4. **Production Trung ương**: Kubernetes multi-AZ, HA, scale theo tải thực tế.
5. **Production đơn vị trực thuộc**: K8s cluster theo đơn vị lớn (Tổng cục) hoặc liên thông VPN/site-to-site về Trung ương (cho đơn vị nhỏ).

**Quy trình triển khai CI/CD:** xem mục II.1.8.

---
