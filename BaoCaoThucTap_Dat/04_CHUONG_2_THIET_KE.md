# Chương 2: PHÂN TÍCH BÀI TOÁN VÀ THIẾT KẾ NỀN TẢNG

## 2.1. Phân tích bài toán

### 2.1.1. Mô tả bài toán và người dùng

Mục tiêu của bài toán là xây dựng một nền tảng AI doanh nghiệp (Enterprise AI Platform) có khả năng cung cấp các khả năng trí tuệ nhân tạo cho nhiều hệ thống khác nhau trong doanh nghiệp. Nền tảng cần đáp ứng các yêu cầu về tính module hóa, khả năng tích hợp đa hệ thống, hỗ trợ nhiều LLM provider, có khả năng chạy on-premise và đảm bảo bảo mật cấp doanh nghiệp.

Các đối tượng người dùng chính của hệ thống bao gồm:
- **Nhân viên doanh nghiệp**: sử dụng các khả năng AI (tìm kiếm, hỏi đáp, tóm tắt) thông qua giao diện web hoặc tích hợp với hệ thống nội bộ (ERP, CRM, DMS).
- **Lập trình viên**: xây dựng các ứng dụng tích hợp với nền tảng thông qua REST API, gRPC hoặc MCP server.
- **Quản trị viên hệ thống**: quản lý tenants, users, plugins, AI providers và giám sát hoạt động của hệ thống.
- **Quản trị viên doanh nghiệp**: cấu hình chính sách bảo mật, theo dõi audit log và sử dụng hệ thống.

Thông tin đầu vào của hệ thống bao gồm:
- Các truy vấn từ người dùng dưới dạng ngôn ngữ tự nhiên hoặc cấu trúc.
- Tài liệu nội bộ của doanh nghiệp (PDF, DOCX, TXT, MD) được upload để xây dựng kho tri thức.
- Cấu hình tenant, plugin, AI provider và các tham số hệ thống.
- Yêu cầu thực thi tác vụ từ các hệ thống bên ngoài thông qua API.

Kết quả đầu ra của hệ thống bao gồm:
- Câu trả lời dạng văn bản có trích dẫn nguồn từ kho tri thức.
- Kết quả tìm kiếm ngữ nghĩa với điểm liên quan.
- Kết quả thực thi tác vụ Agent (ví dụ: tạo báo cáo, truy vấn DB, gửi email).
- Phản hồi streaming theo thời gian thực thông qua Server-Sent Events (SSE) hoặc WebSocket.
- Log kiểm toán (audit log) cho mọi hoạt động của người dùng và Agent.

Phạm vi nghiên cứu tập trung vào việc xây dựng nền tảng AI doanh nghiệp tích hợp đa hệ thống, cung cấp các khả năng AI cốt lõi (RAG, Search, Agent) cho nhiều ứng dụng khác nhau thông qua API chuẩn hóa. Hệ thống không tập trung vào việc xây dựng các ứng dụng nghiệp vụ cụ thể (ERP, CRM) mà tập trung vào lớp nền tảng cho phép tích hợp AI vào các ứng dụng này.

### 2.1.2. Yêu cầu chức năng

Từ bài toán đã xác định, hệ thống cần đáp ứng các yêu cầu chức năng chính được trình bày trong Bảng 2.1.

**Bảng 2.1. Yêu cầu chức năng của hệ thống**

| STT | Yêu cầu chức năng | Mô tả |
|-----|--------------------|-------|
| 1 | Quản lý tài khoản và tenant | Hỗ trợ đăng ký, đăng nhập, quản lý thông tin cá nhân, phân quyền RBAC/ABAC, quản lý multi-tenant với sự cô lập dữ liệu. |
| 2 | Tương tác hội thoại thông minh | Cho phép người dùng trao đổi với hệ thống bằng ngôn ngữ tự nhiên tiếng Việt để đặt câu hỏi, yêu cầu thực hiện tác vụ hoặc tìm kiếm thông tin. |
| 3 | Tìm kiếm ngữ nghĩa | Cung cấp khả năng tìm kiếm tài liệu theo ngữ nghĩa (semantic search) kết hợp với tìm kiếm từ khóa (hybrid search) và reranking. |
| 4 | Hỏi đáp dựa trên RAG | Trả lời câu hỏi dựa trên kho tri thức, có trích dẫn nguồn tài liệu gốc, hỗ trợ streaming response. |
| 5 | AI Agent thực thi tác vụ | Cho phép Agent phân tích yêu cầu phức tạp, lập kế hoạch và thực thi đa bước với khả năng gọi tool từ Tool Registry. |
| 6 | Tích hợp Plugin | Cho phép mở rộng chức năng thông qua plugin mà không cần thay đổi lõi hệ thống, hỗ trợ plugin cho tool, document parser, connector. |
| 7 | Multi-Provider AI | Hỗ trợ nhiều LLM provider (Ollama local, OpenAI cloud) với cơ chế failover, cost tracking, latency tracking. |
| 8 | Text-to-SQL | Chuyển đổi câu hỏi tự nhiên thành SQL queries an toàn (chỉ SELECT), cho phép người dùng không chuyên kỹ thuật truy vấn cơ sở dữ liệu. |
| 9 | Voice AI (STT/TTS) | Hỗ trợ Speech-to-Text sử dụng Whisper và Text-to-Speech cho phép tương tác bằng giọng nói. |
| 10 | MCP Server & Client | Cung cấp MCP server để các AI client bên ngoài (Claude Desktop, Cursor) có thể sử dụng tools của platform; đồng thời hỗ trợ gọi external MCP servers. |
| 11 | Quản lý kho tri thức | Hỗ trợ upload, parse, chunk, embed và quản lý tài liệu từ nhiều nguồn (PDF, DOCX, TXT, MD, Web). |
| 12 | Giám sát và Audit Log | Ghi nhận đầy đủ log cho mọi hoạt động, hỗ trợ tracing, metrics, audit log cho compliance. |
| 13 | Workflow Engine | Hỗ trợ thực thi quy trình đa bước với approval flow, compensation, retry. |
| 14 | Tích hợp hệ thống doanh nghiệp | Cung cấp connector cho ERP, CRM, DMS, HRM với khả năng đồng bộ dữ liệu hai chiều. |
| 15 | Human-in-the-Loop | Cho phép can thiệp thủ công vào quá trình thực thi Agent khi cần thiết, đặc biệt với các hành động nhạy cảm. |

### 2.1.3. Yêu cầu phi chức năng

Bên cạnh các chức năng chính, hệ thống cần đáp ứng một số yêu cầu phi chức năng nhằm đảm bảo khả năng vận hành ổn định, an toàn và phù hợp với môi trường doanh nghiệp. Các yêu cầu này được trình bày trong Bảng 2.2.

**Bảng 2.2. Yêu cầu phi chức năng của hệ thống**

| STT | Yêu cầu | Mô tả |
|-----|----------|-------|
| 1 | Hiệu năng xử lý | Hệ thống cần phản hồi trong thời gian hợp lý đối với các truy vấn RAG, semantic search và Agent, đảm bảo trải nghiệm người dùng không bị gián đoạn. Mục tiêu: thời gian phản hồi trung bình dưới 5 giây cho câu hỏi đơn giản, dưới 30 giây cho RAG query phức tạp. |
| 2 | Khả năng mở rộng | Kiến trúc hệ thống cần hỗ trợ mở rộng theo chiều ngang (horizontal scaling) khi số lượng người dùng, dữ liệu và khối lượng truy vấn tăng lên. |
| 3 | Độ tin cậy | Kết quả sinh ra cần bám sát dữ liệu được truy xuất từ kho tri thức, hạn chế thông tin sai lệch và đảm bảo tính nhất quán. |
| 4 | Tính sẵn sàng | Hệ thống cần duy trì khả năng hoạt động ổn định 99.5% uptime, hỗ trợ cơ chế failover cho LLM provider. |
| 5 | Bảo mật dữ liệu | Thông tin tài khoản, dữ liệu tenant, lịch sử hội thoại và tài liệu nội bộ cần được mã hóa (encryption at-rest và in-transit), hỗ trợ RBAC/ABAC và audit log. |
| 6 | Khả năng tương thích | Giao diện hệ thống cần hiển thị phù hợp trên nhiều thiết bị, đặc biệt là trình duyệt web trên máy tính và thiết bị di động. Hỗ trợ đa ngôn ngữ UI. |
| 7 | Khả năng bảo trì | Hệ thống cần được thiết kế theo hướng module hóa cao, bounded context rõ ràng, tách biệt các thành phần để thuận tiện cho nâng cấp và mở rộng. |
| 8 | Tính nhất quán dữ liệu | Dữ liệu giữa các module (Identity, Tenant, AI Engine, Knowledge) cần được đồng bộ hợp lý nhằm tránh sai lệch trong quá trình truy xuất và xử lý. |
| 9 | Plug & Play | Cho phép thêm/xóa plugin, AI provider, connector mà không cần khởi động lại hệ thống (hot-reload). |
| 10 | Multi-tenant isolation | Đảm bảo sự cô lập hoàn toàn về dữ liệu giữa các tenant, hỗ trợ Row-Level Security (RLS) ở cấp database. |
| 11 | Offline First | Có khả năng chạy hoàn toàn offline với LLM local (Ollama), không phụ thuộc kết nối Internet hay cloud provider. |

## 2.2. Kiến trúc hệ thống

### 2.2.1. Kiến trúc tổng thể bảy tầng

Hệ thống được thiết kế theo kiến trúc bảy tầng (7-Layer Architecture), trong đó các tầng được phân chia theo chức năng nghiệp vụ rõ ràng, đảm bảo tính module hóa, khả năng mở rộng và bảo trì. Bảy tầng chính bao gồm:

1. **Layer 7 - Presentation Layer**: tầng giao diện người dùng (Web, Mobile, CLI, SDK).
2. **Layer 6 - AI Gateway Layer**: cổng API, xác thực, phân quyền, rate limiting, routing.
3. **Layer 5 - Agent Orchestration Layer**: điều phối Agent, Task Planner, Intent Router.
4. **Layer 4 - AI Engine Core**: RAG Engine, Search Engine, SQL Engine, Voice Engine, Tool Engine.
5. **Layer 3 - Provider Abstraction Layer**: trừu tượng hóa LLM, Embedding, Vector Store providers.
6. **Layer 2 - Integration Layer**: MCP Server/Client, Plugin SDK, REST/gRPC Gateway, Event Bus, Message Queue.
7. **Layer 1 - Platform Core**: Multi-Tenancy, Configuration, Audit, Secrets Management.

**Layer 7 - Presentation Layer**: Được xây dựng trên nền tảng Next.js 14 với React 18 và TailwindCSS. Tầng này cung cấp giao diện chat, dashboard quản trị, document manager, agent run viewer và API explorer. Xác thực phía client sử dụng NextAuth với JWT. Tầng này giao tiếp với các tầng dưới thông qua REST API, gRPC và SSE Streaming.

**Layer 6 - AI Gateway Layer**: Đóng vai trò cổng vào duy nhất cho mọi request. Thực hiện xác thực (JWT/OAuth2/API Key), phân quyền (RBAC/ABAC), tenant resolution, rate limiting, request routing, load balancing, circuit breaker và request/response transformation. Sử dụng YARP (Yet Another Reverse Proxy) cho API Gateway trong .NET 8 [1].

**Layer 5 - Agent Orchestration Layer**: Quản lý vòng đời của Agent, bao gồm Intent Router (phân loại ý định), Task Planner (lập kế hoạch thực thi), Agent Executor (thực thi plan), Human-in-the-Loop (phê duyệt thủ công) và Safety Guardrails (kiểm tra an toàn). Hỗ trợ multi-agent thông qua shared memory và message passing.

**Layer 4 - AI Engine Core**: Trái tim của hệ thống, bao gồm:
- **RAG Engine**: pipeline từ query → embed → retrieve → rerank → prompt → LLM → citation.
- **Search Engine**: hybrid search (vector + keyword) với reranking.
- **SQL Engine**: text-to-SQL an toàn với schema awareness.
- **Voice Engine**: STT (Whisper) và TTS.
- **Tool Engine & Registry**: quản lý tools cho Agent.
- **Workflow Engine**: thực thi quy trình đa bước.
- **Memory Manager**: bộ nhớ ngắn hạn và dài hạn cho Agent.

**Layer 3 - Provider Abstraction Layer**: Lớp trừu tượng hóa cho phép chuyển đổi linh hoạt giữa các AI provider mà không ảnh hưởng đến logic nghiệp vụ. Bao gồm:
- **LLM Provider**: Ollama (local), OpenAI, Anthropic Claude, Google Gemini, Azure OpenAI, Custom providers.
- **Embedding Provider**: nomic-embed-text (Ollama), OpenAI text-embedding-3-small, BGE, Custom.
- **Vector Store**: pgvector (PostgreSQL), Qdrant, Milvus.
- **Provider Router**: chọn provider tối ưu dựa trên cost, latency, health check.
- **Model Registry**: catalog các model và capabilities.
- **Provider Cache**: cache response LLM/embedding để giảm chi phí.

**Layer 2 - Integration Layer**: Cung cấp khả năng tích hợp với các hệ thống bên ngoài:
- **MCP Server & Client**: hỗ trợ Model Context Protocol.
- **Plugin SDK & Loader**: load plugin động từ folder.
- **REST/gRPC Gateway**: API cho client bên ngoài.
- **Message Queue Adapter**: RabbitMQ, Kafka.
- **Event Bus**: in-process (MediatR) và cross-process (RabbitMQ).
- **External Connectors**: ERP, DMS, CRM, HRM, Workflow, BI.

**Layer 1 - Platform Core**: Nền tảng nền tảng với các cross-cutting concerns:
- **Multi-Tenancy Manager**: quản lý tenant, isolation.
- **Configuration Manager**: quản lý cấu hình tập trung.
- **Audit & Compliance**: ghi nhận mọi hoạt động.
- **Secrets & Key Management**: quản lý khóa an toàn.

Hệ thống sử dụng PostgreSQL 16 tích hợp pgvector làm cơ sở dữ liệu chính, lưu trữ cả dữ liệu quan hệ (users, tenants, plugins, agents) và dữ liệu vector (embeddings cho RAG). Redis 7 được sử dụng cho caching và session store. MinIO (S3-compatible) lưu trữ tài liệu gốc.

### 2.2.2. Luồng xử lý chính

Hệ thống xử lý yêu cầu của người dùng thông qua một chuỗi các bước liên tiếp, bắt đầu từ việc tiếp nhận yêu cầu, xác thực, phân tích ngữ cảnh, truy xuất tri thức và kết thúc bằng việc sinh phản hồi hoàn chỉnh. Luồng xử lý tổng quát của hệ thống được mô tả trong các bước sau:

**Bước 1: Tiếp nhận và xác thực request**
Khi người dùng gửi yêu cầu từ giao diện hoặc API client, request được gửi đến AI Gateway. Tại đây, hệ thống thực hiện:
- Xác thực người dùng thông qua JWT token hoặc API key.
- Xác định tenant từ token và thiết lập tenant context.
- Kiểm tra rate limit và quota của tenant.
- Áp dụng các chính sách bảo mật (RBAC/ABAC).

**Bước 2: Quản lý ngữ cảnh hội thoại**
Khi người dùng gửi yêu cầu trong một phiên hội thoại, hệ thống trước tiên xử lý ngữ cảnh hội thoại hiện tại. Các thông tin từ những lượt trao đổi trước được tổng hợp nhằm duy trì tính liên tục của cuộc hội thoại và hỗ trợ việc hiểu đúng nội dung yêu cầu. Trong bước này, hệ thống thực hiện:
- Tổng hợp các thông tin liên quan từ lịch sử hội thoại.
- Phân giải các tham chiếu ngữ cảnh như địa điểm hoặc đối tượng đã được đề cập trước đó.
- Xây dựng ngữ cảnh đầu vào cho các bước xử lý tiếp theo.

**Bước 3: Phân tích ý định (Intent Classification)**
Sau khi xác định được ngữ cảnh, hệ thống tiến hành phân tích nội dung yêu cầu nhằm nhận diện ý định của người dùng thông qua Intent Router. Intent Router phân loại yêu cầu vào một trong các nhóm chính:
- **Question Answering**: câu hỏi cần trả lời từ kho tri thức.
- **Task Execution**: yêu cầu thực thi tác vụ phức tạp (Agent).
- **Search**: tìm kiếm tài liệu.
- **Chat**: hội thoại thông thường.
- **System Command**: lệnh hệ thống (quản trị).

**Bước 4: Kiểm tra tính đầy đủ và an toàn**
Hệ thống đánh giá mức độ đầy đủ của dữ liệu đã thu thập được và kiểm tra an toàn:
- Nếu còn thiếu các thông tin quan trọng ảnh hưởng đến việc xử lý, hệ thống sẽ chủ động yêu cầu người dùng bổ sung.
- Safety Guardrails kiểm tra prompt injection, sensitive content và các mối đe dọa bảo mật.
- Nếu phát hiện hành động nhạy cảm, hệ thống chuyển sang Human-in-the-Loop để chờ phê duyệt.

**Bước 5: Định tuyến yêu cầu**
Tùy thuộc vào loại yêu cầu được nhận diện, hệ thống lựa chọn quy trình xử lý phù hợp:
- Câu hỏi đơn giản: sử dụng pipeline RAG trực tiếp.
- Tác vụ phức tạp: chuyển sang Agent Executor với Task Planner.
- Câu hỏi về cơ sở dữ liệu: chuyển sang SQL Engine.
- Lệnh thoại: chuyển sang Voice Engine.
- Cơ chế fallback tự động chuyển đổi khi có lỗi từ provider.

**Bước 6: Truy xuất tri thức và thực thi Agent**
Đây là giai đoạn trung tâm của hệ thống. Đối với yêu cầu RAG:
- RAG Engine thực hiện Hybrid Search (BM25 + vector) trên kho tri thức.
- Cross-Encoder Reranker sắp xếp lại kết quả.
- Citation Engine trích dẫn nguồn.
- LLM Provider được chọn thông qua Provider Router (Ollama hoặc OpenAI).
- Response được sinh và streaming về client.

Đối với Agent task:
- Task Planner lập kế hoạch đa bước.
- Agent Executor thực thi từng bước, gọi Tool từ Registry.
- MCP Client/Server trao đổi với external systems.
- Memory Manager lưu trữ ngữ cảnh.

**Bước 7: Hậu xử lý kết quả**
Kết quả sinh ra từ mô hình được xử lý trước khi trả về:
- Citation Engine bổ sung trích dẫn nguồn.
- SQL Validator kiểm tra tính an toàn của SQL query.
- Output Filter lọc sensitive content.
- Audit Logger ghi nhận hoạt động.

**Bước 8: Trả kết quả và cập nhật**
Sau khi hoàn tất xử lý, hệ thống thực hiện:
- Truyền kết quả về giao diện theo thời gian thực (SSE Streaming).
- Lưu kết quả vào bộ nhớ đệm (Redis Cache) nhằm tối ưu hiệu suất.
- Ghi nhận metrics, traces và audit log.
- Cập nhật memory và hồ sơ người dùng phục vụ cá nhân hóa.

### 2.2.3. Công nghệ sử dụng

Trong quá trình xây dựng hệ thống, các công nghệ được lựa chọn nhằm đáp ứng yêu cầu về hiệu năng, khả năng mở rộng, mức độ phù hợp với bài toán và khả năng tích hợp với các thành phần AI. Bảng 2.3 trình bày tổng hợp các công nghệ chính được sử dụng.

**Bảng 2.3. Công nghệ sử dụng trong hệ thống**

| Vai trò | Công nghệ | Ghi chú |
|---------|-----------|---------|
| Ứng dụng web | Next.js 14 + React 18 + TypeScript + TailwindCSS | Xây dựng giao diện người dùng, dashboard quản trị, chat UI |
| Backend API | .NET 8 (ASP.NET Core) + Minimal APIs | Xử lý nghiệp vụ và cung cấp REST API, gRPC |
| ORM | Entity Framework Core 8 | Quản lý truy cập cơ sở dữ liệu |
| Cơ sở dữ liệu | PostgreSQL 16 + pgvector | Lưu trữ dữ liệu quan hệ và dữ liệu vector |
| Cache & Session | Redis 7 | Caching LLM response, session store, distributed lock |
| Object Storage | MinIO (S3-compatible) | Lưu trữ tài liệu gốc, embeddings |
| LLM Local | Ollama (llama3.2:3b, qwen2.5, mistral) | Chạy local, offline, bảo mật dữ liệu |
| LLM Cloud | OpenAI (GPT-4o-mini, GPT-4o) | Cloud provider với chất lượng cao |
| Embedding | nomic-embed-text (Ollama), text-embedding-3-small (OpenAI) | Sinh vector embedding cho semantic search |
| Framework RAG | LangChain.NET / Custom | Điều phối pipeline RAG, truy xuất và sinh phản hồi |
| MCP | Model Context Protocol (Anthropic) | Giao thức tích hợp AI-native |
| Logging | Serilog + Seq | Structured logging, log aggregation |
| Container | Docker + Docker Compose | Container hóa toàn bộ hệ thống |
| Reverse Proxy | Nginx | Reverse proxy, load balancing |
| API Gateway | YARP (.NET) | API Gateway với rate limiting, auth |
| Message Queue | RabbitMQ | Async task, event-driven |
| CI/CD | GitHub Actions | Build, test, deploy tự động |

**Lý do lựa chọn công nghệ:**

- **.NET 8**: framework mạnh mẽ, hiệu năng cao, hỗ trợ tốt cho backend API và microservices, đặc biệt phù hợp với môi trường doanh nghiệp.
- **PostgreSQL 16 + pgvector**: cho phép quản lý đồng thời dữ liệu quan hệ và dữ liệu vector trên cùng một nền tảng, hỗ trợ Row-Level Security cho multi-tenant, đơn giản hóa kiến trúc lưu trữ.
- **Ollama local**: cho phép chạy LLM hoàn toàn offline, đảm bảo bảo mật dữ liệu cho doanh nghiệp, không phụ thuộc vào cloud provider nước ngoài.
- **OpenAI Cloud**: được sử dụng làm provider bổ sung cho các tác vụ yêu cầu chất lượng cao, có cơ chế failover khi Ollama không khả dụng.
- **MinIO**: object storage S3-compatible cho phép lưu trữ tài liệu gốc với chi phí thấp, dễ tích hợp với các hệ thống khác.
- **Next.js + React**: framework frontend phổ biến, hỗ trợ SSR/SSG, dễ phát triển và bảo trì, cộng đồng lớn.
- **MCP**: giao thức chuẩn hóa cho phép tích hợp AI với nhiều hệ thống khác nhau, đặc biệt hữu ích cho tích hợp ERP, CRM, DMS.
- **Redis**: hỗ trợ caching, session store, distributed lock, message broker - đáp ứng nhiều nhu cầu của hệ thống phân tán.

## 2.3. Thiết kế module và plugin

### 2.3.1. Cấu trúc Solution

Hệ thống được tổ chức theo kiến trúc Modular Monolith với cấu trúc solution rõ ràng, phân chia theo bounded context. Source generator và central package management được sử dụng để đảm bảo tính nhất quán trong quản lý dependencies.

**Cấu trúc Solution tổng thể:**

```
AIBaseFramework/
├── src/
│   ├── Core/                                    # SHARED KERNEL
│   │   ├── AIBaseFramework.SharedKernel/        # Base classes, interfaces, events
│   │   ├── AIBaseFramework.Abstractions/        # Public contracts/interfaces
│   │   └── AIBaseFramework.SDK/                 # Client SDK cho external systems
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
│   │   ├── AIBaseFramework.Messaging/           # Event bus (RabbitMQ)
│   │   └── AIBaseFramework.AIProviders/         # Ollama, OpenAI adapters
│   │
│   ├── Hosts/                                   # DEPLOYMENT HOSTS
│   │   ├── AIBaseFramework.API/                 # Main API host
│   │   ├── AIBaseFramework.Worker/              # Background job host
│   │   ├── AIBaseFramework.Gateway/             # API Gateway (YARP)
│   │   └── AIBaseFramework.MCPServer/           # MCP Server host
│   │
│   └── Plugins/                                 # PLUGIN SYSTEM
│       ├── AIBaseFramework.Plugin.DocumentQA/
│       ├── AIBaseFramework.Plugin.RAGSQL/
│       ├── AIBaseFramework.Plugin.Voice/
│       └── AIBaseFramework.Plugin.Template/     # Template cho plugin mới
├── tests/
│   ├── AIBaseFramework.UnitTests/
│   ├── AIBaseFramework.IntegrationTests/
│   └── AIBaseFramework.ArchTests/               # Architecture fitness tests
└── docker/
```

**Nguyên tắc Dependency Rule:**
- Hosts phụ thuộc vào Modules, Infrastructure, Core.
- Modules phụ thuộc vào Core và Abstractions.
- Infrastructure implement các abstractions từ Core.
- Plugins phụ thuộc vào Abstractions và SDK.
- Modules KHÔNG phụ thuộc lẫn nhau (bounded context isolation).
- Core KHÔNG phụ thuộc vào bất kỳ thành phần nào khác.

### 2.3.2. Module Identity và Tenant

**Module Identity** chịu trách nhiệm quản lý người dùng, xác thực và phân quyền. Các thành phần chính:
- **User Management**: CRUD users, profile management.
- **Authentication**: JWT issue/validate, OAuth2 integration, API key management.
- **Authorization**: RBAC (Role-Based Access Control) và ABAC (Attribute-Based Access Control).
- **Session Management**: session store trong Redis, refresh token rotation.

**Module Tenant** quản lý multi-tenant với sự cô lập dữ liệu:
- **Tenant CRUD**: tạo, cập nhật, xóa tenant.
- **Tenant Configuration**: cấu hình riêng cho từng tenant (LLM provider, plugin enable/disable, quota).
- **Tenant Isolation**: sử dụng Row-Level Security (RLS) ở PostgreSQL, TenantId discriminator.
- **Subscription Management**: quản lý gói dịch vụ, quota tracking.

Mối quan hệ giữa Identity và Tenant: một user có thể thuộc nhiều tenant (qua Membership), mỗi membership có role riêng trong tenant đó. Khi request đến, Gateway xác thực JWT, trích xuất tenantId và userId, thiết lập context cho toàn bộ pipeline xử lý.

### 2.3.3. Module AI Engine Core

AI Engine Core là trái tim của hệ thống, bao gồm các thành phần:

**RAG Engine** (P0 - Ưu tiên cao nhất):
- Interface `IRAGEngine` với `QueryAsync` và `StreamQueryAsync`.
- Pipeline: Query → Embed → Retrieve (top-k) → Rerank → Context Build → LLM → Citation.
- Hỗ trợ nhiều strategy: Vector, Hybrid (BM25 + Vector), Hybrid + Reranker.
- Citation: chỉ ra document nào, chunk nào, text snippet.
- Streaming response với Server-Sent Events.

**Document Ingestion & Chunking**:
- `IDocumentParser` interface với implementations: PDF (iText7), DOCX (Open XML SDK), TXT, MD (Markdig).
- `IChunker` interface: FixedSizeChunker, RecursiveChunker, MarkdownChunker.
- Auto-detect MIME type, streaming upload.
- Config: chunk_size, chunk_overlap, separator.

**Vector Store & Semantic Search**:
- `IVectorStore` interface với: UpsertAsync, SearchAsync, DeleteAsync, CreateCollectionAsync.
- `PgVectorStore` implementation sử dụng Npgsql + pgvector.
- Hỗ trợ: cosine distance, HNSW index, batch insert, metadata filter.

**Hybrid Search & Reranker**:
- Kết hợp Dense (vector) + Sparse (BM25) thông qua Reciprocal Rank Fusion (RRF).
- Cross-Encoder Reranker sắp xếp lại kết quả.
- Metadata filter cho từng loại collection.

**Citation Engine**:
- Trích dẫn token-level attribution.
- Hiển thị `[1]`, `[2]` trong response.
- Click để mở modal xem source document.

**Tool Engine & Registry**:
- `ITool` interface cho các tool function.
- `IToolRegistry` quản lý tập tool.
- Built-in tools: Calculator, CurrentTime, WebSearch, DatabaseQuery.
- Hỗ trợ async tool, parallel tool execution.

**SQL Engine (Text-to-SQL)**:
- Chuyển đổi câu hỏi tự nhiên thành SQL queries.
- Schema awareness: hiểu cấu trúc database.
- SQL injection protection: chỉ cho phép SELECT queries, validate identifiers.
- Caching kết quả truy vấn.

**Voice Engine (STT/TTS)**:
- STT sử dụng Whisper (chạy local trong Ollama).
- TTS sử dụng model phù hợp.
- Hỗ trợ streaming audio.

**Memory Manager**:
- Short-term memory: session-level context.
- Long-term memory: cross-session user preferences.
- Working memory: trong quá trình thực thi Agent.

### 2.3.4. Module Agent và MCP

**Module Agent** cung cấp framework cho AI Agent:

**Intent Router** (P0):
- `IIntentRouter` interface.
- `RuleBasedIntentRouter`: sử dụng keyword matching và regular expression.
- `LLMBasedIntentRouter`: sử dụng LLM để phân loại intent phức tạp.
- Multi-intent detection: hỗ trợ nhiều intent trong một câu.

**Task Planner** (P0):
- `ITaskPlanner` interface.
- `PlanAndExecutePlanner`: sử dụng LLM sinh plan dạng JSON.
- Plan structure: `Plan` chứa danh sách `PlanStep`, mỗi step có `StepType`, `ToolName`, `Parameters`.
- Replanning: khi step fail, planner sinh lại plan mới.

**Agent Executor** (P0):
- `IAgentExecutor` interface.
- Thực thi plan tuần tự hoặc song song.
- Retry với exponential backoff.
- Tool calling với parameter validation.

**Human-in-the-Loop (HITL)**:
- `IHITLService` interface.
- Pause execution khi gặp action nhạy cảm.
- Notification qua SignalR/WebSocket.
- Approve/Reject từ UI.

**Safety Guardrails**:
- Input validation: phát hiện prompt injection.
- Output filtering: lọc sensitive content.
- Action validation: kiểm tra action có nằm trong whitelist.
- Rate limiting per user/tenant.

**MCP (Model Context Protocol)**:

**MCP Server**:
- Expose platform tools qua MCP protocol.
- Cho phép Claude Desktop, Cursor và các AI client khác sử dụng platform tools.
- Resource exposure: documents, knowledge base.
- Tool exposure: tất cả tools trong registry.
- Prompt template exposure: các prompt mẫu.

**MCP Client**:
- Gọi external MCP servers.
- Tích hợp với các hệ thống hỗ trợ MCP.
- Dynamic tool discovery.

### 2.3.5. Plugin SDK

Plugin SDK cho phép mở rộng chức năng của platform mà không cần build lại lõi:

**Cấu trúc Plugin:**
- **Plugin Manifest** (plugin.json): metadata, version, dependencies, permissions.
- **Plugin Assembly** (.dll): implementation.
- **Extension Points**: các interface mà plugin implement.

**Các loại Extension Point:**

1. **IToolProvider**: cung cấp tool mới cho Agent.
   ```csharp
   public interface IToolProvider
   {
       string Name { get; }
       string Description { get; }
       Task<ToolResult> ExecuteAsync(Dictionary<string, object> parameters, CancellationToken ct);
   }
   ```

2. **IDocumentParser**: hỗ trợ định dạng tài liệu mới (Excel, PowerPoint, HTML, etc.).

3. **IEmbeddingProvider**: tích hợp mô hình embedding mới.

4. **ILLMProvider**: tích hợp LLM provider mới.

5. **IConnector**: kết nối hệ thống bên ngoài (ERP, CRM, HRM, etc.).

6. **IWorkflowStep**: mở rộng workflow engine.

7. **IPromptTemplate**: cung cấp prompt template mới.

**Plugin Lifecycle:**
- Discovery: scan folder `plugins/` để tìm plugin manifests.
- Loading: load assembly và validate dependencies.
- Registration: register extension points với platform.
- Execution: thực thi khi có request.
- Unloading: hot-unload khi cần update.

**Security Sandbox:**
- Plugin chạy trong sandbox với permission giới hạn.
- Plugin không được truy cập trực tiếp database hoặc file system ngoài whitelist.
- Mọi resource access phải qua platform API.

## 2.4. Thiết kế dữ liệu

### 2.4.1. Cơ sở dữ liệu quan hệ

Hệ thống sử dụng PostgreSQL 16 làm cơ sở dữ liệu quan hệ chính. Lược đồ dữ liệu được thiết kế theo các nhóm chức năng, nhằm đảm bảo tính toàn vẹn dữ liệu, hỗ trợ multi-tenant và thuận lợi cho việc mở rộng hệ thống.

**Nhóm dữ liệu Tenant và Identity:**
- `tenants`: lưu thông tin tenant (id, name, slug, status, created_at, configuration).
- `users`: thông tin tài khoản (id, email, password_hash, full_name, is_active).
- `memberships`: quan hệ user-tenant (user_id, tenant_id, role, status).
- `roles`: định nghĩa role (id, tenant_id, name, permissions).
- `permissions`: định nghĩa permission (id, name, resource, action).
- `api_keys`: API key cho programmatic access.
- `refresh_tokens`: refresh token cho JWT.

**Nhóm dữ liệu Knowledge:**
- `document_collections`: nhóm tài liệu (id, tenant_id, name, description).
- `documents`: tài liệu gốc (id, tenant_id, collection_id, title, file_path, mime_type, status, uploaded_by, created_at).
- `document_chunks`: đoạn văn bản sau khi chunk (id, document_id, chunk_index, content, content_hash, embedding vector(768/1536), metadata jsonb).
- `tags`: nhãn cho tài liệu.
- `document_tags`: quan hệ n-n.

**Nhóm dữ liệu Chat và Conversation:**
- `chat_sessions`: phiên chat (id, tenant_id, user_id, title, created_at).
- `chat_messages`: tin nhắn trong phiên (id, session_id, role, content, citations jsonb, created_at).

**Nhóm dữ liệu Agent:**
- `agent_definitions`: định nghĩa agent (id, tenant_id, name, system_prompt, tools, configuration).
- `agent_runs`: lần chạy agent (id, agent_id, session_id, status, plan jsonb, result, started_at, completed_at).
- `agent_plan_steps`: bước trong plan (id, run_id, step_index, tool_name, parameters, status, result, error).
- `agent_events`: sự kiện trong quá trình chạy (id, run_id, event_type, payload, created_at).

**Nhóm dữ liệu Tool và Plugin:**
- `tool_definitions`: định nghĩa tool (id, name, description, schema jsonb, plugin_id, is_builtin).
- `tool_calls`: lịch sử gọi tool (id, agent_run_id, tool_name, parameters, result, duration_ms, status).
- `plugins`: thông tin plugin (id, tenant_id, name, version, manifest jsonb, status, assembly_path, loaded_at).
- `mcp_servers`: MCP server config (id, tenant_id, name, endpoint, transport, configuration).

**Nhóm dữ liệu Provider và Configuration:**
- `llm_providers`: cấu hình LLM provider (id, tenant_id, provider_type, base_url, api_key_encrypted, models jsonb, is_enabled).
- `embedding_providers`: cấu hình embedding provider.
- `vector_stores`: cấu hình vector store.

**Nhóm dữ liệu Audit và Observability:**
- `audit_logs`: log kiểm toán (id, tenant_id, user_id, action, resource_type, resource_id, payload jsonb, ip_address, created_at).
- `metrics`: metrics hệ thống (id, name, value, tags jsonb, timestamp).
- `traces`: distributed tracing (id, trace_id, span_id, parent_span_id, operation, duration_ms, tags jsonb).

### 2.4.2. Cơ sở dữ liệu vector (pgvector)

Bên cạnh dữ liệu quan hệ, hệ thống sử dụng extension pgvector trên PostgreSQL để lưu trữ vector embedding phục vụ truy xuất ngữ nghĩa trong pipeline RAG. Việc sử dụng chung PostgreSQL cho cả dữ liệu quan hệ và dữ liệu vector giúp đơn giản hóa kiến trúc triển khai, đồng thời thỗ trợ Row-Level Security cho multi-tenant.

Các tài liệu trong kho tri thức sau khi được thu thập và chuẩn hóa sẽ được chia thành các đoạn nội dung nhỏ hơn (chunk). Mỗi đoạn văn bản được chuyển đổi thành vector embedding thông qua mô hình nhúng và được lưu trữ cùng với nội dung gốc cũng như các thông tin mô tả liên quan.

**Bảng 2.4. Cấu trúc dữ liệu vector embedding**

| Trường | Kiểu dữ liệu | Mô tả |
|--------|---------------|-------|
| id | uuid | Khóa định danh của bản ghi vector |
| tenant_id | uuid | Tenant ID cho multi-tenant isolation |
| document_id | uuid | Foreign key đến documents |
| content | text | Nội dung văn bản gốc |
| embedding | vector(768) hoặc vector(1536) | Vector biểu diễn ngữ nghĩa của văn bản |
| metadata | jsonb | Thông tin mô tả phục vụ lọc và truy xuất |
| created_at | timestamptz | Thời điểm tạo |

Mô hình nhúng được sử dụng là nomic-embed-text (768 dimensions) cho Ollama local hoặc text-embedding-3-small (1536 dimensions) cho OpenAI. Trong quá trình truy xuất, câu hỏi của người dùng cũng được chuyển đổi thành vector embedding và so sánh với các vector trong kho tri thức bằng độ đo cosine similarity, trong đó pgvector sử dụng chỉ mục HNSW (Hierarchical Navigable Small World) để tăng tốc tìm kiếm láng giềng gần đúng.

### 2.4.3. Kho tri thức

Kho tri thức là nguồn dữ liệu đầu vào của pipeline RAG, cung cấp thông tin để hệ thống truy xuất và sinh phản hồi. Dữ liệu trong kho tri thức được xây dựng từ nhiều nguồn khác nhau nhằm đảm bảo tính đa dạng, độ bao phủ và khả năng bổ sung thông tin cho bài toán doanh nghiệp.

Các nguồn dữ liệu chính được sử dụng trong hệ thống bao gồm:
- **Dữ liệu nội bộ doanh nghiệp**: tài liệu PDF, DOCX, TXT, Markdown được upload bởi người dùng hoặc đồng bộ từ hệ thống ERP, CRM, DMS, HRM qua connector.
- **Dữ liệu biên soạn thủ công**: thông tin tổng quan, quy trình, chính sách nội bộ đã được kiểm duyệt.
- **Dữ liệu từ web**: crawl hoặc scrape từ các nguồn web nội bộ (intranet), tài liệu hướng dẫn sử dụng.
- **Dữ liệu từ Database**: kết quả truy vấn từ SQL Engine có thể được cache thành knowledge.

Sau khi thu thập, dữ liệu được chuẩn hóa và xử lý theo quy trình: thu thập → parse → clean → chunk → embed → index. Quy trình này đảm bảo dữ liệu từ nhiều nguồn khác nhau được chuẩn hóa về cấu trúc, loại bỏ các thông tin không phù hợp và duy trì tính nhất quán trước khi đưa vào sử dụng.

## 2.5. Thiết kế hệ thống RAG nâng cao

### 2.5.1. Pipeline RAG nâng cao

Hệ thống RAG là thành phần trung tâm, chịu trách nhiệm kết nối kho tri thức với mô hình ngôn ngữ lớn để sinh ra các phản hồi có căn cứ dữ liệu. Kiến trúc được xây dựng theo hướng RAG nâng cao (Advanced RAG), trong đó bổ sung nhiều giai đoạn tối ưu nhằm cải thiện chất lượng ngữ cảnh truy xuất và kết quả đầu ra.

**Bảng 2.5. So sánh RAG cơ bản và RAG nâng cao trong hệ thống**

| Giai đoạn | RAG cơ bản | RAG nâng cao |
|-----------|------------|--------------|
| Trước truy xuất | Sử dụng trực tiếp truy vấn gốc | Phân tích yêu cầu, trích xuất thực thể, mở rộng truy vấn |
| Truy xuất | Truy xuất vector đơn lẻ | Hybrid Search (BM25 + Vector) với RRF |
| Sau truy xuất | Ít hoặc không có xử lý bổ sung | Cross-Encoder Reranking, lọc nhiễu |
| Xây dựng ngữ cảnh | Chủ yếu tài liệu truy xuất | Kết hợp tài liệu, structured data, user profile, history |
| Sinh kết quả | Prompt đơn giản với context giới hạn | Prompt có context mở rộng, structured output |
| Hậu xử lý | Không có | Citation extraction, faithfulness check |

Pipeline RAG nâng cao trong hệ thống bao gồm các bước:

1. **Query Analysis**: phân tích ý định, trích xuất thực thể (điểm đến, thời gian, ngân sách), xác định loại câu hỏi.
2. **Query Expansion**: mở rộng truy vấn với synonyms, related terms để tăng độ bao phủ.
3. **Hybrid Retrieval**: kết hợp vector search và BM25 thông qua RRF.
4. **Cross-Encoder Reranking**: sắp xếp lại kết quả bằng mô hình cross-encoder đa ngôn ngữ.
5. **Context Aggregation**: tổng hợp ngữ cảnh từ retrieved documents, structured DB, user profile, conversation history.
6. **Context Compression**: nén ngữ cảnh dài, loại bỏ thông tin trùng lặp.
7. **LLM Generation**: sinh phản hồi sử dụng LLM provider được chọn.
8. **Citation Extraction**: trích dẫn nguồn cho từng phần của phản hồi.
9. **Faithfulness Check**: kiểm tra tính trung thực của phản hồi với kho tri thức.
10. **Streaming Response**: truyền phản hồi về client qua SSE.

### 2.5.2. Hybrid Search và Reranking

Hệ thống sử dụng kết hợp Hybrid Search và Reranking để nâng cao chất lượng truy xuất:

**Hybrid Search:**
- **Dense Retrieval**: sử dụng vector embedding từ Ollama (nomic-embed-text) hoặc OpenAI (text-embedding-3-small). Truy xuất top-k=50 candidates.
- **Sparse Retrieval**: sử dụng BM25 trên PostgreSQL full-text search với pg_trgm extension.
- **Reciprocal Rank Fusion (RRF)**: kết hợp kết quả từ hai phương pháp bằng công thức:

  RRF(d) = Σ 1/(k + rank_r(d))

  Trong đó R là tập các danh sách kết quả truy xuất, rank_r(d) là thứ hạng của tài liệu d trong danh sách r, còn k là hằng số điều hòa (thường k=60).

**Cross-Encoder Reranking:**
- Sử dụng mô hình cross-encoder đa ngôn ngữ (ví dụ: BAAI/bge-reranker-v2-m3) để sắp xếp lại top-50 candidates.
- Cross-encoder xử lý trực tiếp cặp (query, document) để tính điểm liên quan chính xác hơn bi-encoder.
- Output: top-10 documents có điểm liên quan cao nhất.

**Metadata Filtering:**
- Lọc theo tenant_id, collection_id, document_type.
- Lọc theo access permission của user.
- Lọc theo date range, tags.

### 2.5.3. Tối ưu ngữ cảnh

Giai đoạn tối ưu ngữ cảnh có nhiệm vụ xây dựng và tinh gọn ngữ cảnh đầu vào cho mô hình ngôn ngữ từ các dữ liệu đã được truy xuất. Giai đoạn này gồm hai bước chính:

**Context Aggregation (Tổng hợp ngữ cảnh đa nguồn):**
Để xây dựng phản hồi phù hợp, hệ thống không chỉ sử dụng các tài liệu truy xuất từ kho tri thức mà còn kết hợp thêm dữ liệu có cấu trúc và thông tin người dùng. Các nguồn dữ liệu được sử dụng gồm:
- **Retrieved Documents**: các tài liệu đã được truy xuất và xếp hạng.
- **Structured DB**: dữ liệu có cấu trúc về điểm tham quan, nhà hàng, cơ sở lưu trú, tuyến vận chuyển.
- **User Profile**: hồ sơ sở thích và lịch sử tương tác của người dùng.
- **Conversation History**: lịch sử hội thoại gần nhất.
- **External Context**: thông tin từ external APIs (weather, news, etc.).

**Context Compression (Nén ngữ cảnh):**
Sau khi tổng hợp, ngữ cảnh đầu vào có thể chứa thông tin trùng lặp, dư thừa hoặc ít liên quan đến truy vấn. Hệ thống áp dụng kỹ thuật nén ngữ cảnh theo hướng trích xuất, chỉ giữ lại các đoạn thông tin có mức độ liên quan cao với yêu cầu của người dùng. Quá trình này giúp giảm kích thước ngữ cảnh, hạn chế chi phí xử lý và đảm bảo dữ liệu đầu vào phù hợp với giới hạn ngữ cảnh của mô hình ngôn ngữ.

### 2.5.4. Sinh phản hồi và trích dẫn nguồn

Giai đoạn này thực hiện việc sinh phản hồi dựa trên ngữ cảnh đã được xây dựng từ các bước trước. Mô hình LLM (được chọn thông qua Provider Router) nhận ngữ cảnh đã được tổng hợp và tối ưu hóa, sau đó sinh phản hồi phù hợp với từng loại yêu cầu.

**Generation:**
- Prompt template với system prompt, context, user query, structured output schema.
- Temperature thấp (0.3) cho factual responses.
- Streaming với Server-Sent Events.
- Function calling cho Agent mode.

**Structured Output:**
- Sử dụng JSON schema để enforce output structure.
- Output dạng JSON với: answer, citations, follow_up_questions.
- Hỗ trợ multi-language responses.

**Citation Engine:**
- Trích dẫn token-level: chỉ ra đoạn text nào trong document được sử dụng.
- Hiển thị `[1]`, `[2]`, `[3]` trong response.
- Mỗi citation chứa: document_id, chunk_id, text snippet, score, source URL.
- UI cho phép click vào citation để mở modal xem full source.

**Bảng 2.6. Cấu trúc đầu ra của hệ thống**

| Cấp dữ liệu | Nội dung |
|--------------|----------|
| Response | Nội dung câu trả lời dạng Markdown |
| Citations | Danh sách trích dẫn nguồn với score |
| Sources | Document ID, chunk ID, text snippet, URL |
| Metadata | Tenant ID, user ID, session ID, latency, token usage |
| Follow-ups | Gợi ý câu hỏi tiếp theo |

## 2.6. Kết luận chương 2

Chương 2 đã trình bày quá trình phân tích bài toán và thiết kế nền tảng AI doanh nghiệp tích hợp đa hệ thống. Nội dung chính của chương bao gồm:

- Xác định bài toán, đối tượng người dùng, phạm vi nghiên cứu và các yêu cầu chức năng, phi chức năng của hệ thống. Đặc biệt nhấn mạnh 7 nguyên tắc thiết kế: Plug & Play, Provider Agnostic, Domain Aware, Event Driven, Agent Native, Offline First, Enterprise Grade.

- Thiết kế kiến trúc tổng thể theo mô hình bảy tầng gồm: Presentation Layer, AI Gateway Layer, Agent Orchestration Layer, AI Engine Core, Provider Abstraction Layer, Integration Layer và Platform Core. Mỗi tầng có trách nhiệm rõ ràng và sử dụng các công nghệ phù hợp.

- Xây dựng cấu trúc Solution theo kiến trúc Modular Monolith với Core, Modules, Infrastructure, Hosts và Plugins. Phân chia rõ ràng các bounded context: Identity, Tenant, AIEngine, Agent, Knowledge, Workflow, Integration, Observability.

- Thiết kế cơ sở dữ liệu và kho tri thức, bao gồm lược đồ quan hệ cho 9 nhóm dữ liệu chính, cơ sở dữ liệu vector pgvector với HNSW index, và quy trình xử lý dữ liệu đa nguồn.

- Đề xuất hệ thống RAG nâng cao với 10 bước pipeline: Query Analysis, Query Expansion, Hybrid Retrieval, Reranking, Context Aggregation, Context Compression, LLM Generation, Citation Extraction, Faithfulness Check, Streaming Response.

- Tích hợp các kỹ thuật Hybrid Search, Cross-Encoder Reranking, Context Compression, Structured Output và Citation nhằm cải thiện chất lượng truy xuất, độ tin cậy của phản hồi và mức độ phù hợp với yêu cầu doanh nghiệp.

Các nội dung đã trình bày trong Chương 2 là cơ sở để triển khai hệ thống thực nghiệm, tiến hành đánh giá và phân tích kết quả trong Chương 3, qua đó kiểm chứng tính khả thi của phương pháp đề xuất.
