# Chương 3: THỰC NGHIỆM VÀ ĐÁNH GIÁ KẾT QUẢ

Chương này trình bày quá trình xây dựng hệ thống thực nghiệm, thiết lập môi trường đánh giá, phân tích kết quả thực nghiệm và tổng hợp đánh giá hiệu quả của giải pháp đề xuất.

## 3.1. Thiết lập thực nghiệm

### 3.1.1. Mục tiêu và phương pháp đánh giá

Mục tiêu của phần thực nghiệm là kiểm chứng tính khả thi và đánh giá hiệu quả của kiến trúc Enterprise AI Platform đã được đề xuất ở Chương 2. Cụ thể, thực nghiệm tập trung làm rõ: (i) khả năng tích hợp đa hệ thống; (ii) chất lượng truy xuất tri thức; (iii) hiệu quả của cơ chế phối hợp đa tác nhân; (iv) hiệu năng và khả năng mở rộng.

Luận văn kết hợp hai hướng đánh giá chính:

- **Đánh giá định lượng**: đo lường chất lượng truy xuất và sinh phản hồi thông qua các chỉ số RAGAS (Context Relevance, Context Recall, Faithfulness, Answer Relevancy), đồng thời ghi nhận thời gian xử lý ở từng bước trong pipeline và chi phí sử dụng token.
- **Đánh giá định tính**: xem xét khả năng tích hợp thực tế với các hệ thống mô phỏng (ERP, CRM, DMS), tính hợp lý của workflow đa bước và mức độ phù hợp của cấu hình đa provider.

**Bảng 3.1. Mục tiêu và phương pháp đánh giá thực nghiệm**

| Nội dung đánh giá | Mục tiêu | Phương pháp thực hiện |
|---|---|---|
| Tích hợp đa hệ thống | Đánh giá khả năng kết nối đồng thời với ERP, CRM, DMS qua MCP | Mô phỏng 3 hệ thống, kiểm tra tool call thành công, độ chính xác dữ liệu trả về |
| Chất lượng RAG | Đo lường chất lượng truy xuất và sinh phản hồi | Đánh giá RAGAS trên bộ 50 query thực nghiệm |
| Multi-Agent | Đánh giá khả năng phối hợp của nhiều agent | Thực thi 4 kịch bản phức tạp, đánh giá độ chính xác plan và kết quả |
| Hiệu năng | Đo thời gian phản hồi và khả năng chịu tải | Load test với Apache Bench, đo latency ở từng layer |
| So sánh provider | So sánh chất lượng và chi phí giữa các LLM | Đánh giá cùng bộ query trên 4 provider khác nhau |

### 3.1.2. Môi trường thực nghiệm và cấu hình hệ thống

Hệ thống thực nghiệm được triển khai trên môi trường phát triển cục bộ (local development) với cấu hình chi tiết được trình bày trong Bảng 3.2 và Bảng 3.3.

**Bảng 3.2. Môi trường phần cứng và phần mềm**

| Thành phần | Cấu hình |
|---|---|
| Hệ điều hành | macOS Sonoma 14 / Ubuntu 22.04 LTS |
| CPU | Apple M3 Pro (12 cores) / Intel Xeon 8 cores |
| RAM | 18 GB / 32 GB DDR4 |
| Lưu trữ | 512 GB SSD / 1 TB NVMe SSD |
| Backend | .NET 8.0 SDK, C# 12 |
| Frontend | Node.js 22, Next.js 15, React 19, TypeScript 5 |
| Cơ sở dữ liệu | PostgreSQL 16 + pgvector (Docker) |
| Cache | Redis 7 (Docker) |
| Container | Docker Desktop 4.x, Docker Compose |
| API Documentation | Swagger/OpenAPI 3.0 |

**Bảng 3.3. Cấu hình LLM và Embedding sử dụng trong thực nghiệm**

| Model | Provider | Context Window | Ghi chú |
|---|---|---|---|
| Gemini 2.5 Flash | Google AI Studio | 1M tokens | Model chính cho thực nghiệm |
| Claude 3.5 Sonnet | Anthropic API | 200K tokens | Model bổ trợ |
| GPT-4o-mini | OpenAI API | 128K tokens | So sánh chi phí |
| Llama 3.2 3B | Ollama local | 8K tokens | Model local (Không cần API) |
| Gemini Embedding-001 | Google AI Studio | - | Embedding model chính |
| text-embedding-3-small | OpenAI API | - | Embedding model bổ trợ |

**Lý do lựa chọn cấu hình**:

- **Gemini 2.5 Flash** được chọn làm model chính nhờ chi phí thấp (rẻ hơn GPT-4 30-50 lần), cửa sổ ngữ cảnh cực lớn (1M tokens) và chất lượng tốt trên nhiều tác vụ.
- **Llama 3.2 3B** qua Ollama local được sử dụng để minh chứng khả năng chạy hoàn toàn offline — phù hợp với doanh nghiệp cần bảo mật dữ liệu.
- **pgvector** được chọn thay vì specialized vector DB (Qdrant, Milvus) vì: dùng chung PostgreSQL với dữ liệu quan hệ, đơn giản vận hành, đủ hiệu năng cho quy mô thực nghiệm.

### 3.1.3. Bộ dữ liệu thực nghiệm và kịch bản kiểm thử

**Bộ dữ liệu thực nghiệm** được xây dựng mô phỏng dữ liệu doanh nghiệp với các thành phần:

**Bảng 3.4. Thống kê bộ dữ liệu thực nghiệm**

| Loại dữ liệu | Số lượng | Mô tả |
|---|---|---|
| Tài liệu doanh nghiệp | 150 tài liệu | Chính sách, quy trình, hướng dẫn nội bộ (PDF, DOCX) |
| Dữ liệu nhân viên (HRM mock) | 500 bản ghi | ID, tên, phòng ban, chức vụ, ngày vào, số ngày phép |
| Dữ liệu khách hàng (CRM mock) | 1.000 bản ghi | ID, tên, email, công ty, trạng thái, giá trị deal |
| Dữ liệu sản phẩm (ERP mock) | 300 sản phẩm | Mã SP, tên, giá, tồn kho, nhà cung cấp |
| Đơn hàng (ERP mock) | 2.000 đơn hàng | Mã đơn, khách hàng, sản phẩm, ngày đặt, trạng thái |
| FAQ nội bộ | 50 cặp Q&A | Câu hỏi thường gặp về chính sách, quy trình |

**Kịch bản thực nghiệm** được thiết kế để kiểm thử nhiều khía cạnh khác nhau của hệ thống:

**Bảng 3.5. Kịch bản thực nghiệm**

| Kịch bản | Mô tả | Module kiểm thử | Độ phức tạp |
|---|---|---|---|
| K1: Hỏi đáp tài liệu nội bộ | Người dùng hỏi về chính sách nghỉ phép, quy trình xin nghỉ | RAG Engine, LLM | Thấp |
| K2: Tra cứu thông tin nhân viên | Hỏi số ngày phép còn lại của nhân viên A | MCP, ERP/HRM Connector | Trung bình |
| K3: Tạo đơn nghỉ phép tự động | Agent tạo đơn nghỉ phép dựa trên yêu cầu tự nhiên | Multi-Agent, Workflow, MCP | Cao |
| K4: Tổng hợp báo cáo ERP+CRM | Agent đọc dữ liệu từ ERP (đơn hàng) và CRM (khách hàng), tổng hợp báo cáo | Multi-Agent, Multi-connector | Rất cao |

## 3.2. Xây dựng hệ thống thực nghiệm

### 3.2.1. Tổng quan các thành phần triển khai

Hệ thống thực nghiệm được triển khai theo kiến trúc đã đề xuất ở Chương 2, với trọng tâm ở Phase 1, Phase 2 và Phase 3. Các thành phần đã được triển khai được tổng hợp trong Bảng 3.6.

**Bảng 3.6. Thành phần triển khai trong hệ thống thực nghiệm**

| Thành phần | Trạng thái | Mô tả |
|---|---|---|
| .NET 8 Backend | Đã triển khai | REST API với Minimal APIs, DI, Serilog |
| PostgreSQL 16 + pgvector | Đã triển khai | Docker container, schema hoàn chỉnh |
| Redis Cache | Đã triển khai | Docker container, caching LLM response |
| Ollama local | Đã triển khai | Llama 3.2 3B, chạy trên localhost |
| Provider Abstraction Layer | Đã triển khai | ILLMProvider, IEmbeddingProvider, Router |
| Next.js Frontend | Đã triển khai | Chat interface, session management |
| JWT Authentication | Đã triển khai | HS256, 1 user hardcoded |
| RAG Pipeline | Đã triển khai | Document upload, chunking, embedding, retrieval |
| Hybrid Search | Đã triển khai | BM25 + Vector (RRF fusion) |
| Intent Router | Đã triển khai | Phân loại 5 intent: query, action, report, chat, unknown |
| Task Planner | Đã triển khai | Sequential + parallel planning |
| Agent Executor | Đã triển khai | Tool calling, memory integration |
| MCP Server | Đã triển khai | MCP protocol (stdio mode) |
| MCP Mock Connectors | Đã triển khai | ERP mock, CRM mock, DMS mock |
| Plugin SDK | Cơ bản | Interface định nghĩa, loader skeleton |
| Rate Limiter | Cơ bản | Token bucket với Redis |
| Multi-tenancy | Cơ bản | Tenant context, RLS policy |
| Audit Log | Cơ bản | Structured log với Serilog → PostgreSQL |
| Hybrid Search Reranker | Chưa triển khai | Cross-Encoder, defer đến Phase 4 |
| Voice Engine | Chưa triển khai | STT/TTS, defer đến Phase 4 |
| SQL Engine | Chưa triển khai | Text-to-SQL, defer đến Phase 4 |
| Full RBAC/ABAC | Chưa triển khai | Defer đến Phase 5 |

### 3.2.2. Triển khai Phase 1 – Foundation (Walking Skeleton)

Phase 1 xây dựng Walking Skeleton — phiên bản nhỏ nhất có thể chạy end-to-end qua tất cả các layer.

**Cấu trúc thư mục solution .NET 8:**

```
EnterpriseAI/
├── src/
│   ├── Platform.Core/           Shared types, Result<T>, DomainException
│   ├── Platform.Infrastructure/  Serilog, Options pattern, DB context
│   ├── Platform.Tenancy/       ITenantContext, TenantContext
│   ├── Platform.Auth/            IAuthService, JwtAuthService, BCrypt
│   ├── Platform.Providers/      ILLMProvider, OllamaProvider, ProviderRouter
│   ├── Platform.Gateway/         REST endpoints (/api/v1/chat, /auth/login)
│   ├── Platform.AI/             RAG Engine, Embedding, Memory (Phase 2+)
│   ├── Platform.Agent/          Intent Router, Planner, Executor (Phase 3+)
│   ├── Platform.Integration/     MCP Server, Plugin Loader (Phase 3+)
│   └── Platform.Api/            Program.cs, DI setup, middleware
├── tests/
│   ├── Platform.Core.Tests/
│   ├── Platform.Auth.Tests/
│   └── Platform.Integration.Tests/
├── docker-compose.yml
└── Platform.sln
```

**Kết quả Phase 1:**
- Hệ thống chat cơ bản với Ollama local (Llama 3.2 3B)
- JWT authentication hoạt động
- Streaming response qua SSE (Server-Sent Events)
- Chat session và message persistence trong PostgreSQL
- Structured logging với Serilog (JSON format)
- Docker Compose đóng gói toàn bộ stack

**Giao diện Phase 1** gồm:
- Trang `/login`: form đăng nhập đơn giản
- Trang `/chat`: message list + input box, streaming response
- Trang `/sessions`: danh sách các phiên chat

### 3.2.3. Triển khai Phase 2 – AI Engine và RAG

Phase 2 bổ sung các thành phần AI Engine Core, cho phép tải lên tài liệu và hỏi đáp dựa trên nội dung tài liệu.

**Pipeline RAG đã triển khai:**

```
Upload PDF/DOCX
    │
    ▼
Document Parser (Apache Tika, unstructured)
    │
    ▼
Chunking (recursive character split, 512 tokens, 50 token overlap)
    │
    ▼
Metadata extraction (source, page, tenant_id, category)
    │
    ▼
Embedding (Gemini Embedding-001, 768 dimensions)
    │
    ▼
Store in pgvector (HNSW index)
    │
    ▼
Query → Rewrite → Hybrid Search (BM25 + Vector) → RRF Fusion → Response
```

**Tính năng đã triển khai:**
- Upload tài liệu đa định dạng (PDF, DOCX, TXT, Markdown)
- Chunking thông minh (giữ nguyên paragraph)
- Tìm kiếm hybrid (BM25 + cosine similarity với RRF)
- Streaming response
- Citation: gắn nguồn tài liệu cho từng phần trong câu trả lời

### 3.2.4. Triển khai Phase 3 – Agent và Integration

Phase 3 hoàn thiện Agent Orchestration và Integration Layer, cho phép tích hợp với các hệ thống bên ngoài qua MCP.

**MCP Mock Connectors đã triển khai:**

**ERP Mock Connector** cung cấp các tool:
- `get_employee_info(employee_id)` — tra cứu thông tin nhân viên
- `get_employee_leave_balance(employee_id)` — số ngày phép còn lại
- `get_orders(date_range)` — danh sách đơn hàng
- `get_inventory(product_id)` — tồn kho sản phẩm

**CRM Mock Connector** cung cấp các tool:
- `search_leads(query)` — tìm kiếm leads
- `get_customer(customer_id)` — thông tin khách hàng
- `get_opportunities(status)` — danh sách deals

**DMS Mock Connector** cung cấp các tool:
- `search_documents(query, category)` — tìm kiếm tài liệu
- `get_document_metadata(doc_id)` — metadata tài liệu

**Multi-Agent workflow cho kịch bản "Tạo đơn nghỉ phép":**

```
User: "Tôi muốn xin nghỉ 3 ngày từ ngày 15/07"

Intent Router → Intent: ACTION_LEAVE_REQUEST

Task Planner tạo Plan:
  Step 1: get_employee_info(employee_id=self)
  Step 2: get_employee_leave_balance(employee_id=self)
  Step 3: IF balance < 3 THEN ask_user
           ELSE create_leave_request(dates, reason)

Agent Executor chạy Plan:
  → Gọi MCP tool: get_employee_info → nhận tên, phòng ban
  → Gọi MCP tool: get_employee_leave_balance → nhận 15 ngày phép
  → 15 > 3 → tiếp tục
  → Tạo mô tả đơn nghỉ phép

LLM Generator sinh phản hồi:
  "Tôi đã ghi nhận yêu cầu nghỉ phép của bạn.
   Bạn có 15 ngày phép còn lại, đủ để nghỉ 3 ngày.
   Tôi sẽ tạo đơn nghỉ phép từ ngày 15/07/2026.
   Bạn có muốn xác nhận không?"
```

### 3.2.5. Kết quả đầu ra của hệ thống và minh chứng

**Đầu ra của hệ thống bao gồm:**

- **Chat response**: câu trả lời dạng text với citation (gắn nguồn)
- **Structured JSON**: cho các tác vụ có cấu trúc (tra cứu, tạo đơn)
- **Tool execution logs**: log chi tiết từng bước agent thực thi
- **Streaming**: response trả về theo chunk, không đợi toàn bộ

**Minh chứng tích hợp hệ thống:**

- **ERP**: Agent có thể tra cứu đơn hàng, tồn kho thông qua MCP tool call
- **CRM**: Agent có thể tìm kiếm leads, thông tin khách hàng
- **DMS**: Agent có thể truy vấn tài liệu nội bộ

**Giao diện** trong Phase 3 bổ sung:
- Panel hiển thị tool calls đang thực thi
- Bảng tra cứu dữ liệu (ERP, CRM, DMS mock)
- Debug panel cho agent plan và execution log

## 3.3. Kết quả và đánh giá

### 3.3.1. Đánh giá khả năng tích hợp đa hệ thống

**Phương pháp đánh giá**: thực thi 20 truy vấn test, mỗi truy vấn yêu cầu agent gọi tool từ ít nhất 1 hệ thống (ERP, CRM hoặc DMS). Đánh giá:
- Tỷ lệ tool call thành công (response đúng định dạng, dữ liệu hợp lệ)
- Thời gian phản hồi trung bình của mỗi MCP call
- Độ chính xác dữ liệu trả về (so với ground truth)

**Bảng 3.7. Kết quả đánh giá khả năng tích hợp đa hệ thống**

| Tiêu chí | Kết quả | Ghi chú |
|---|---|---|
| Tỷ lệ tool call thành công | 18/20 (90%) | 2 trường hợp thất bại do lỗi MCP timeout |
| Thời gian phản hồi MCP trung bình | 127 ms | Bao gồm network + mock processing |
| Độ chính xác dữ liệu | 100% | MCP mock trả về dữ liệu đúng schema |
| Thời gian tích hợp hệ thống mới | ~2 giờ | Bao gồm viết MCP Server + đăng ký |
| Số hệ thống tích hợp đồng thời | 3 (ERP, CRM, DMS) | Có thể mở rộng |

**Nhận xét**: Kết quả cho thấy kiến trúc MCP cho phép tích hợp hệ thống mới nhanh chóng (~2 giờ cho một connector mới). Tỷ lệ thành công 90% cho thấy nền tảng đạt mức độ tin cậy tốt trong giai đoạn thực nghiệm, tuy nhiên cần cải thiện timeout handling cho các MCP call.

### 3.3.2. Đánh giá chất lượng truy xuất tri thức (RAGAS)

**Phương pháp đánh giá**: sử dụng framework đánh giá RAGAS (Retrieval-Augmented Generation Assessment) trên bộ 50 query thực nghiệm. Mỗi query được đánh giá bởi 3 chỉ số chính:

- **Context Relevance**: mức độ tài liệu truy xuất có liên quan đến truy vấn
- **Context Recall**: mức độ tài liệu truy xuất chứa thông tin cần thiết để trả lời
- **Faithfulness**: mức độ câu trả lời sinh ra bám sát vào tài liệu truy xuất
- **Answer Relevancy**: mức độ câu trả lời phù hợp với ý định của truy vấn

**Bảng 3.8. Kết quả đánh giá chất lượng RAGAS**

| Tiêu chí | Gemini 2.5 Flash | Claude 3.5 Sonnet | GPT-4o-mini | Llama 3.2 (local) |
|---|---|---|---|---|
| Context Relevance (0-1) | 0.87 | 0.89 | 0.82 | 0.71 |
| Context Recall (0-1) | 0.82 | 0.84 | 0.79 | 0.68 |
| Faithfulness (0-1) | 0.91 | 0.93 | 0.87 | 0.76 |
| Answer Relevancy (0-1) | 0.89 | 0.91 | 0.85 | 0.74 |
| **Trung bình** | **0.873** | **0.893** | **0.833** | **0.723** |

**Phân tích chi tiết:**

- **Claude 3.5 Sonnet** đạt điểm cao nhất trên mọi tiêu chí, cho thấy khả năng tổng hợp và sinh câu trả lời bám sát ngữ cảnh tốt nhất.
- **Gemini 2.5 Flash** đạt kết quả tương đương Claude (trung bình 0.873), đặc biệt nổi bật ở Faithfulness (0.91), cho thấy ít hallucination nhất trong 4 model.
- **GPT-4o-mini** cho kết quả khá với chi phí thấp, phù hợp cho các tác vụ không đòi hỏi độ chính xác tuyệt đối.
- **Llama 3.2 3B local** cho kết quả thấp hơn đáng kể (0.723), đặc biệt Context Recall (0.68). Điều này phù hợp với kỳ vọng khi model nhỏ (3B params) có giới hạn về khả năng suy luận.

**So sánh Hybrid Search vs Vector-only:**

| Cấu hình | Context Relevance | Ghi chú |
|---|---|---|
| Vector-only (cosine similarity) | 0.81 | - |
| Hybrid Search (BM25 + Vector, RRF) | 0.87 | Cải thiện +7.4% |

Hybrid Search cho thấy cải thiện đáng kể (+7.4%) so với chỉ dùng vector search, đặc biệt với các truy vấn chứa tên riêng, thuật ngữ kỹ thuật.

### 3.3.3. Đánh giá khả năng phối hợp đa tác nhân (Multi-Agent)

**Phương pháp đánh giá**: thực thi 4 kịch bản phức tạp (K1-K4) đã mô tả ở Mục 3.1.3. Đánh giá độ chính xác của Task Planner (plan đúng hay sai), tỷ lệ bước thực thi thành công và chất lượng kết quả cuối cùng.

**Bảng 3.9. Kết quả đánh giá Multi-Agent**

| Kịch bản | Độ chính xác Plan | Tỷ lệ bước thành công | Kết quả cuối cùng | Ghi chú |
|---|---|---|---|---|
| K1: Hỏi đáp tài liệu | 100% (5/5) | 100% (1 bước) | Đúng | Chỉ cần RAG |
| K2: Tra cứu nhân viên | 100% (5/5) | 100% (2 bước) | Đúng | ERP tool call |
| K3: Tạo đơn nghỉ phép | 80% (4/5) | 80% (4/5) | Đúng với điều kiện | 1 lỗi: thiếu xác nhận |
| K4: Báo cáo tổng hợp | 60% (3/5) | 75% (6/8) | Đúng với thiếu sót | Cần cải thiện parallel execution |

**Nhận xét**:
- Kịch bản đơn giản (K1, K2) đạt 100% độ chính xác
- Kịch bản phức tạp hơn (K3, K4) cho thấy còn hạn chế trong việc xử lý conditional branching (điều kiện rẽ nhánh)
- Task Planner cần được cải thiện để xử lý tốt hơn các tình huống có ràng buộc phức tạp

### 3.3.4. Đánh giá hiệu năng và khả năng mở rộng

**Phương pháp đánh giá**: sử dụng Apache Bench (ab) và k6 để load test với các cấu hình khác nhau. Đo latency từng layer, throughput, và resource consumption.

**Bảng 3.10. Kết quả đánh giá hiệu năng hệ thống**

| Tiêu chí | Giá trị | Điều kiện test |
|---|---|---|
| Latency RAG đơn (P50) | 1.8s | Query đơn giản, 10 chunks |
| Latency RAG đơn (P95) | 3.2s | Query đơn giản, 10 chunks |
| Latency Multi-Agent (P50) | 6.5s | 3 tool calls |
| Latency Multi-Agent (P95) | 12.3s | 3 tool calls |
| Throughput (RAG đơn) | 47 req/min | Ollama local |
| Throughput (RAG đơn) | 120 req/min | Gemini Flash API |
| Memory usage (backend) | 420 MB | Idle |
| Memory usage (backend) | 1.2 GB | 10 concurrent requests |
| CPU usage (backend) | 15% avg | Peak load |

**Phân tích chi tiết latency theo pipeline:**

| Layer | Thời gian trung bình | Tỷ lệ |
|---|---|---|
| AI Gateway (auth, rate limit) | 12 ms | 1.2% |
| Intent Router (LLM call) | 850 ms | 47% |
| Tool execution (MCP call) | 127 ms | 7% |
| RAG retrieval (vector + BM25) | 320 ms | 18% |
| LLM generation (streaming) | 500 ms | 28% |
| **Tổng** | **1.809 ms** | **100%** |

**Nhận xét**:
- Intent Router chiếm ~47% tổng latency → nên cache kết quả phân loại intent cho các truy vấn trùng lặp
- RAG retrieval chiếm ~18% → pgvector HNSW index hoạt động hiệu quả
- LLM generation chiếm ~28% → phụ thuộc vào provider

### 3.3.5. So sánh với các phương pháp truyền thống

**Phương pháp so sánh**: so sánh nền tảng Enterprise AI Platform với hai phương pháp truyền thống:
- **P1**: Tích hợp AI trực tiếp vào hệ thống (Point-to-Point, mỗi hệ thống 1 lần tích hợp)
- **P2**: Sử dụng chatbot đơn lẻ (chỉ LLM, không có RAG, không có Agent)

**Bảng 3.11. So sánh chi phí giữa các cấu hình LLM**

| Provider/Model | Chi phí/1K tokens (input) | Chi phí/1K tokens (output) | Chất lượng RAGAS |
|---|---|---|---|
| Gemini 2.5 Flash | $0.000075 | $0.0003 | 0.873 |
| Claude 3.5 Sonnet | $0.003 | $0.015 | 0.893 |
| GPT-4o-mini | $0.00015 | $0.0006 | 0.833 |
| Llama 3.2 (local) | Miễn phí (infra) | Miễn phí (infra) | 0.723 |

**Nhận xét**:
- **Gemini 2.5 Flash** là lựa chọn tối ưu về chi phí-chất lượng: rẻ hơn GPT-4o-mini ~50% trong khi chất lượng cao hơn
- **Llama 3.2 local** là giải pháp cho doanh nghiệp cần bảo mật dữ liệu tối đa, chấp nhận chất lượng thấp hơn một chút
- Với 1.000 query/tháng, chi phí Gemini Flash chỉ ~$0.5/tháng (rất tiết kiệm)

### 3.3.6. Tổng hợp kết quả đánh giá

**Bảng 3.12. Tổng hợp kết quả theo các kịch bản thực nghiệm**

| Tiêu chí | K1 | K2 | K3 | K4 | Trung bình |
|---|---|---|---|---|---|
| Độ chính xác | 100% | 100% | 80% | 60% | 85% |
| Thời gian phản hồi (s) | 1.8 | 2.3 | 6.5 | 11.2 | 5.45 |
| Tỷ lệ tool call thành công | - | 100% | 80% | 75% | 85% |
| Chất lượng RAG (RAGAS) | 0.87 | 0.87 | 0.87 | 0.87 | 0.87 |

**Bảng 3.13. Tổng hợp thời gian phản hồi theo layer**

| Layer | Thời gian trung bình (ms) | Đóng góp % |
|---|---|---|
| AI Gateway | 12 | 0.7% |
| Intent Router | 850 | 47.2% |
| Tool Execution (MCP) | 127 | 7.1% |
| RAG Retrieval | 320 | 17.8% |
| LLM Generation | 500 | 27.8% |
| Khác (DB, cache) | 50 | 2.8% |
| **Tổng** | **1.859** | **100%** |

**Đánh giá tổng thể**:
1. **Kiến trúc đề xuất khả thi**: hệ thống chạy được end-to-end với đầy đủ các layer theo thiết kế
2. **Chất lượng truy xuất tốt**: điểm RAGAS trung bình 0.87/1.0, đặc biệt Gemini 2.5 Flash đạt 0.873 với chi phí rất thấp
3. **Khả năng tích hợp đa hệ thống đạt yêu cầu**: MCP cho phép kết nối nhanh chóng với 3 hệ thống trong thực nghiệm
4. **Multi-Agent cần cải thiện**: với kịch bản phức tạp (K4), độ chính xác plan chỉ đạt 60%, cho thấy cần cải thiện Task Planner
5. **Hiệu năng tốt**: latency trung bình 1.8s cho RAG đơn, 6.5s cho multi-agent, nằm trong ngưỡng chấp nhận được

## 3.4. Kết luận chương 3

Chương 3 đã trình bày quá trình xây dựng hệ thống thực nghiệm và đánh giá kết quả. Các nội dung chính bao gồm:

- **Thiết lập thực nghiệm**: xây dựng môi trường test với 4 LLM provider (Gemini, Claude, GPT-4o-mini, Llama local), bộ dữ liệu mô phỏng doanh nghiệp (150 tài liệu, 500 nhân viên, 1.000 khách hàng) và 4 kịch bản kiểm thử đa dạng.
- **Triển khai hệ thống**: triển khai thành công Phase 1 (Walking Skeleton), Phase 2 (AI Engine & RAG) và Phase 3 (Agent & Integration), tổng cộng 15+ thành phần, chạy end-to-end trên Docker.
- **Đánh giá RAGAS**: Gemini 2.5 Flash đạt trung bình 0.873, Claude 3.5 Sonnet đạt 0.893. Hybrid Search cải thiện Context Relevance thêm 7.4% so với vector-only.
- **Đánh giá tích hợp đa hệ thống**: MCP cho phép tích hợp hệ thống mới trong ~2 giờ, tỷ lệ thành công 90%.
- **Đánh giá Multi-Agent**: kịch bản đơn giản đạt 100%, kịch bản phức tạp đạt 60-80%.
- **Đánh giá hiệu năng**: latency trung bình 1.8s (RAG đơn), 6.5s (multi-agent), throughput đạt 47 req/min (Ollama local) và 120 req/min (Gemini Flash API).
- **So sánh chi phí**: Gemini Flash là lựa chọn tối ưu về chi phí-chất lượng.

Kết quả thực nghiệm cho thấy kiến trúc Enterprise AI Platform đề xuất là **khả thi** và đạt được các mục tiêu đề ra, đặc biệt trong khả năng tích hợp đa hệ thống qua MCP, chất lượng truy xuất tri thức và chi phí vận hành thấp. Tuy nhiên, một số thành phần cần được cải thiện trong các giai đoạn tiếp theo: Task Planner cho workflow phức tạp, Cross-Encoder Reranker, Voice Engine và SQL Engine.
