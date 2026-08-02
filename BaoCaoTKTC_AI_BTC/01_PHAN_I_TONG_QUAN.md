# I. TỔNG QUAN

## I.1. Mục đích

Tài liệu **"Thiết kế chức năng Hệ thống Trí tuệ nhân tạo (AI) phục vụ Văn phòng số Bộ Tài Chính"** được xây dựng nhằm mô tả chi tiết yêu cầu nghiệp vụ, kiến trúc kỹ thuật, giải pháp công nghệ và thiết kế chức năng của phân hệ AI — một thành phần cốt lõi của Phần mềm Văn phòng số Bộ Tài Chính. Tài liệu là cơ sở thống nhất giữa **Chủ đầu tư (Bộ Tài Chính), đơn vị quản lý, đơn vị sử dụng, đơn vị tư vấn thiết kế, đơn vị tư vấn giám sát, đơn vị kiểm thử và đơn vị thi công (Mobifone Solutions)** trong toàn bộ vòng đời phát triển và vận hành hệ thống.

Tài liệu hướng tới các mục tiêu chính sau:

1. Chuẩn hóa yêu cầu nghiệp vụ và chức năng AI của Hệ thống trên cơ sở các quy định hiện hành về công tác văn thư, quản lý văn bản điện tử, hồ sơ công việc, lưu trữ điện tử, ký số, bảo mật thông tin và các quy trình nghiệp vụ đang áp dụng tại Bộ Tài Chính.
2. Xác định đầy đủ phạm vi các phân hệ AI, chức năng AI, đối tượng sử dụng, vai trò tham gia và mối quan hệ giữa AI với các phân hệ nghiệp vụ khác của Văn phòng số.
3. Chuẩn hóa quy trình xử lý AI trên môi trường điện tử, bảo đảm tính thống nhất trong toàn ngành Tài chính nhưng vẫn đáp ứng yêu cầu phân cấp, phân quyền, đặc thù tổ chức của các đơn vị thuộc Bộ Từ Trung ương đến địa phương.
4. Làm cơ sở cho việc phân tích và thiết kế kiến trúc AI Gateway, AI Engine Core, Agent Orchestration, Knowledge Base (RAG), OCR, Provider Abstraction, MCP Server; kiến trúc dữ liệu; cơ chế phân quyền, tích hợp và liên thông với các hệ thống có liên quan (ERP, DMS, HRM, Kho bạc, Thuế, Hải quan, ...).
5. Làm căn cứ để xây dựng tài liệu thiết kế chi tiết (TKT), tài liệu hướng dẫn sử dụng (HDSD), kịch bản kiểm thử (QC), kiểm thử chấp nhận người dùng (UAT), tài liệu đào tạo, chuyển đổi dữ liệu, triển khai và nghiệm thu Hệ thống.
6. Bảo đảm Hệ thống AI được xây dựng theo hướng **tập trung – thống nhất – có khả năng mở rộng – tích hợp – liên thông** với các hệ thống hiện có của Bộ Tài Chính và các hệ thống liên thông vùng/ngành.
7. Đáp ứng yêu cầu **cải cách hành chính, chuyển đổi số, chỉ đạo điều hành** và hình thành môi trường làm việc số trong toàn Bộ Tài Chính.
8. Tạo nền tảng để từng bước ứng dụng trí tuệ nhân tạo (AI), tự động hóa, tìm kiếm thông minh, phân tích ngữ nghĩa và khai thác dữ liệu nhằm hỗ trợ người sử dụng, nâng cao năng suất xử lý công việc và chất lượng công tác quản lý, chỉ đạo, điều hành tại Bộ Tài Chính.

## I.2. Phạm vi hệ thống

Hệ thống AI phục vụ Văn phòng số Bộ Tài Chính (viết tắt: **Hệ thống AI**) là một thành phần trong Phần mềm Văn phòng số Bộ Tài Chính. Hệ thống được xây dựng theo định hướng nền tảng số thống nhất, phục vụ công tác quản lý văn bản, hồ sơ công việc, chỉ đạo điều hành và quản trị nội bộ của các đơn vị thuộc Bộ Tài Chính.

### I.2.1. Phạm vi nghiệp vụ

Hệ thống AI bao trùm các phân hệ chức năng chính của Văn phòng số Bộ Tài Chính, bao gồm:

1. **Phân hệ Quản lý Văn bản & Hồ sơ công việc** — Tự động tóm tắt, chuẩn hóa văn bản, phân loại văn bản, gợi ý lãnh đạo phê duyệt, trích xuất thông tin từ văn bản.
2. **Phân hệ Chỉ đạo điều hành** — Gợi ý lịch họp, tối ưu lịch tuần, ưu tiên công việc, tự động tạo báo cáo tiến độ.
3. **Phân hệ AI & tự động hóa** — Cung cấp các năng lực AI cốt lõi: **AI Gateway, AI Engine Core, Agent Orchestration, Knowledge Base (RAG), OCR & Document Extraction, Provider Abstraction (LLM), AI Job & Task Queue, MCP Server, Audit & Observability, AI Admin Console, Auth & Tenant**.
4. **Phân hệ Quản trị nội bộ** — Hỗ trợ AI trong quản lý nhân sự, tài sản, kho văn bản, lưu trữ.
5. **Phân hệ Tích hợp & Liên thông** — Tích hợp với **DMS, ERP, HRM, Kho bạc, Thuế, Hải quan, Ủy ban Chứng khoán, BHXH Việt Nam**, các hệ thống nghiệp vụ toàn ngành và các cổng dịch vụ công quốc gia.

### I.2.2. Phạm vi tổ chức

Hệ thống phải đáp ứng đồng thời nhiều mô hình tổ chức trong Bộ Tài Chính, bao gồm:

- **Cơ quan Bộ** — Văn phòng Bộ, các Cục chuyên ngành, các đơn vị tham mưu.
- **Các đơn vị trực thuộc lớn** — Tổng cục Thuế, Kho bạc Nhà nước, Tổng cục Hải quan, Ủy ban Chứng khoán Nhà nước, Tổng cục Dự trữ Nhà nước, Tổng cục Thống kê, Bảo hiểm xã hội Việt Nam.
- **Các đơn vị có tổ chức nhiều cấp** từ Trung ương đến địa phương và cơ sở.

Hệ thống AI không phụ thuộc cứng vào một mô hình tổ chức cụ thể mà phải cho phép cấu hình linh hoạt cơ cấu tổ chức, cấp quản lý, chức danh, thẩm quyền, quy trình nghiệp vụ và **multi-tenant** cho từng khối/đơn vị.

### I.2.3. Phạm vi người sử dụng

Hệ thống phục vụ các nhóm người dùng sau:

| STT | Nhóm người dùng | Quy mô dự kiến |
|---|---|---|
| 1 | Lãnh đạo Bộ | ~50 tài khoản |
| 2 | Lãnh đạo các đơn vị (cấp Cục, Vụ, Sở, Chi cục, ...) | ~3.000 tài khoản |
| 3 | Văn thư, thư ký | ~5.000 tài khoản |
| 4 | Chuyên viên, cán bộ xử lý nghiệp vụ | ~45.000 tài khoản |
| 5 | Cán bộ quản trị hệ thống | ~500 tài khoản |
| 6 | Nhóm tích hợp hệ thống, API consumer | Không giới hạn |
| | **Tổng cộng** | **~60.000 tài khoản đồng thời (active users)** |

Quy mô thiết kế hướng tới **phục vụ ổn định 60.000 người dùng** với xu hướng tăng trưởng theo mở rộng cơ cấu tổ chức và nhu cầu sử dụng. Hệ thống phải đảm bảo khả năng **scale-out theo chiều ngang** (Horizontal Scaling) thông qua Cloud-native / Kubernetes / Docker mà không phải thay đổi kiến trúc ứng dụng.

### I.2.4. Phạm vi dữ liệu

Hệ thống AI xử lý dữ liệu xuyên suốt vòng đời của văn bản, hồ sơ, nhiệm vụ và các đối tượng nghiệp vụ liên quan trong Văn phòng số, gồm:

- **Dữ liệu văn bản**: nội dung văn bản, tóm tắt, trích yếu, metadata, lịch sử xử lý.
- **Dữ liệu hồ sơ công việc**: tiêu đề, mô tả, giao việc, tiến độ, báo cáo.
- **Dữ liệu tri thức ngữ nghĩa**: vector embedding của văn bản, hồ sơ, quy trình, hướng dẫn.
- **Dữ liệu OCR**: văn bản trích xuất từ ảnh/PDF scan.
- **Dữ liệu người dùng và quyền hạn**: thông tin cá nhân, tenant, vai trò, phân quyền.
- **Dữ liệu audit/log**: lịch sử truy vấn AI, prompt, response, model version, kết quả.

### I.2.5. Phạm vi kỹ thuật

Hệ thống AI được xây dựng theo kiến trúc hiện đại, dựa trên **nền tảng SmartOffice Backend Core hiện hữu** (`qlvb-backend-core`):

- Backend chính: **.NET 10 / ASP.NET Core**, **Clean Architecture** (Domain – Application – Infrastructure – API), **CQRS** bằng **Mediator Source Generator**, **FluentValidation**, **Entity Framework Core 10 + Npgsql**.
- Cơ sở dữ liệu quan hệ: **PostgreSQL 16** (multi-schema: `identity`, `organization`, `office_manage`, `master_data`, `files_management`, `permission`, `notification`, `schedule`, `ai_platform`, ...).
- Vector Database: **pgvector** (PostgreSQL extension) hoặc **Qdrant** (tùy cấu hình).
- Caching/Session: **Redis 7 / Valkey**.
- Message Broker: **Kafka / RabbitMQ**.
- Object Storage: **MinIO / S3-compatible** (lưu trữ file OCR, embedding snapshot, ...).
- API Gateway: **Kong Gateway** + BFF Gateway (.NET).
- Observability: **OpenTelemetry + ELK Stack (Elasticsearch, Kibana, APM)**.
- Container & Deploy: **Docker / Docker Compose / Kubernetes**.
- LLM Providers hỗ trợ: **OpenAI (gpt-4o, gpt-4o-mini, text-embedding-3-*)**, **Ollama local (llama3.2, qwen2.5, nomic-embed-text, ...)**, có khả năng mở rộng cho Anthropic Claude, Google Gemini.
- Tích hợp chuẩn: **REST, gRPC, MCP (Model Context Protocol), WebSocket, Webhook, Event Bus (MassTransit)**.
- Mã nguồn tổ chức theo nguyên tắc **Outbox-first cho giao tiếp liên service**, đảm bảo **reliable message delivery**.

### I.2.6. Nguyên tắc kế thừa và mở rộng

- Kế thừa toàn bộ quy trình nghiệp vụ đã được chuẩn hóa của Chương trình Quản lý văn bản và điều hành theo **Quyết định số 258/QĐ-BTC**.
- Bám sát các tài liệu chuẩn hóa nghiệp vụ về văn bản đến, văn bản đi, tờ trình, văn bản nội bộ, giao việc, hồ sơ công việc, lịch, phòng họp, xe, liên thông.
- Thiết kế theo hướng **mở, cấu hình linh hoạt**, hạn chế phụ thuộc vào quy trình hoặc cơ cấu tổ chức cố định.
- Tạo nền tảng để tích hợp thêm các năng lực AI mới trong các giai đoạn tiếp theo mà không cần refactor kiến trúc lõi.

## I.3. Tài liệu liên quan

| STT | Tên tài liệu | Mã tài liệu / Nguồn |
|---|---|---|
| 1 | Quyết định số 258/QĐ-BTC về Chương trình Quản lý văn bản và điều hành | Tài liệu pháp lý của Bộ Tài Chính |
| 2 | Tài liệu chuẩn hóa quy trình nghiệp vụ văn bản đến – văn bản đi – văn bản nội bộ | Phòng Văn thư – Bộ Tài Chính |
| 3 | Tài liệu đặc tả yêu cầu người dùng (URD): `2.08bmtkpm.dvc10_TaiLieuPhanTichDacTaYeuCau_URD_v1.0` | `qlvb-documents/Core/5. Tài liệu phát triển/3. Tiêu chuẩn/` |
| 4 | Tài liệu "AI Worker Design – Document Management System (DMS)" | `qlvb-documents/Core/5. Tài liệu phát triển/1. Nghiên cứu/AI_README.md` |
| 5 | Tài liệu "Chuyển đổi AI" | `qlvb-documents/Core/5. Tài liệu phát triển/1. Nghiên cứu/Chuyển đổi AI.docx` |
| 6 | Tài liệu "Nghiên cứu, chọn nền tảng, công cụ phát triển AI cho team BE, FE, BA, QC" | `qlvb-documents/Core/5. Tài liệu phát triển/1. Nghiên cứu/` |
| 7 | Tài liệu "Nghiên cứu cấu trúc và triển khai core BE mới trên nền tảng .NET 8/10" | `qlvb-documents/Core/5. Tài liệu phát triển/1. Nghiên cứu/` |
| 8 | Thống kê nghiên cứu sử dụng các mô hình AI | `qlvb-documents/Core/5. Tài liệu phát triển/1. Nghiên cứu/Thống kê nghiên cứu sử dụng các mô hình AI.xlsx` |
| 9 | SmartOffice Backend Core – README & Solution Structure | `qlvb-backend-core/README.md` |
| 10 | Tài liệu "Entity-Domain-And-OutboxEvent" (Outbox Pattern) | `qlvb-backend-core/docs/Entity-Domain-And-OutboxEvent.md` |
| 11 | Tài liệu "Apply Vertical Slice in Clean Architecture .NET API" | `qlvb-backend-core/docs/Apply Vertical Slice in Clean Architecture .NET API.md` |
| 12 | Tài liệu Cơ sở dữ liệu các schema: `identity`, `organization`, `office_manage`, `master_data`, `notification`, `permission`, `files_management`, `schedule` | `qlvb-backend-core/docs/database/` |
| 13 | Tài liệu luồng xử lý chức năng các module: Identity, Files (Upload large file flow, Luồng ký số, Strategy pattern ký số) | `qlvb-backend-core/docs/modules/` |
| 14 | Tài liệu mô hình Microservice BTC | `qlvb-documents/Core/5. Tài liệu phát triển/2. Kiến trúc/Kiến trúc Microservice BTC.png` |
| 15 | Tài liệu mô hình Hạ tầng | `qlvb-documents/Core/5. Tài liệu phát triển/2. Kiến trúc/Kiến trúc Hạ tầng.png` |
| 16 | Tài liệu Quy trình phát triển | `qlvb-documents/Core/5. Tài liệu phát triển/2. Kiến trúc/Quy trình phát triển.png` |
| 17 | Tài liệu mô tả AI Agent Workflow | `qlvb-backend-core/README_AI_AGENT_WORKFLOW.md` |
| 18 | Cây quyền hệ thống (Permission Tree) | `qlvb-documents/Core/5. Tài liệu phát triển/Cây quyền hệ thống (2).xlsx` |

## I.4. Thuật ngữ và các từ viết tắt

| STT | Thuật ngữ / Chữ viết tắt | Mô tả |
|---|---|---|
| 1 | **AI** | Artificial Intelligence – Trí tuệ nhân tạo |
| 2 | **LLM** | Large Language Model – Mô hình ngôn ngữ lớn |
| 3 | **RAG** | Retrieval-Augmented Generation – Kỹ thuật sinh văn bản tăng cường truy xuất |
| 4 | **OCR** | Optical Character Recognition – Nhận dạng ký tự quang học |
| 5 | **MCP** | Model Context Protocol – Giao thức ngữ cảnh mô hình, chuẩn giao tiếp giữa Agent và công cụ/dịch vụ bên ngoài |
| 6 | **Embedding** | Biểu diễn vector của văn bản trong không gian nhiều chiều, dùng cho tìm kiếm ngữ nghĩa |
| 7 | **pgvector** | Extension của PostgreSQL cho phép lưu trữ và tìm kiếm vector |
| 8 | **Qdrant** | Hệ quản trị vector database chuyên dụng (tùy chọn) |
| 9 | **Vector DB** | Cơ sở dữ liệu vector |
| 10 | **Agent** | Thực thể AI có khả năng lập kế hoạch, gọi công cụ, tự ra quyết định theo mục tiêu |
| 11 | **Tool / Function calling** | Khả năng của LLM gọi một hàm/công cụ đã định nghĩa sẵn |
| 12 | **Provider Abstraction** | Lớp trừu tượng hóa nhà cung cấp LLM (OpenAI / Ollama / Anthropic / ...) |
| 13 | **AI Gateway** | Cổng trung gian phía trước các LLM provider: rate-limit, cache, failover, audit |
| 14 | **AI Engine Core** | Lõi xử lý AI: prompt builder, context aggregator, response validator |
| 15 | **Knowledge Base** | Kho tri thức ngữ nghĩa phục vụ RAG |
| 16 | **Chunking** | Kỹ thuật chia nhỏ văn bản dài thành các đoạn nhỏ để embedding |
| 17 | **Hybrid Search** | Kết hợp tìm kiếm từ khóa (BM25) và tìm kiếm ngữ nghĩa (vector) |
| 18 | **Reranking** | Sắp xếp lại kết quả tìm kiếm bằng mô hình Cross-Encoder cho chính xác hơn |
| 19 | **Outbox Pattern** | Mẫu thiết kế đảm bảo gửi event/message đáng tin cậy giữa các service |
| 20 | **CQRS** | Command Query Responsibility Segregation – Phân tách lệnh ghi / truy vấn đọc |
| 21 | **BFF** | Backend For Frontend – API Gateway dành riêng cho từng loại frontend |
| 22 | **Kong** | API Gateway mã nguồn mở, dùng để định tuyến, xác thực, rate-limit |
| 23 | **OpenTelemetry** | Chuẩn observability mã nguồn mở (logs, metrics, traces) |
| 24 | **ELK** | Elasticsearch + Logstash + Kibana – Stack thu thập và trực quan hóa log |
| 25 | **Tenant** | Đơn vị tổ chức (cơ quan Bộ, Cục, Sở, Chi cục, ...) trong mô hình multi-tenant |
| 26 | **Multi-tenant** | Kiến trúc cho phép nhiều đơn vị/tổ chức dùng chung một hệ thống mà dữ liệu cô lập theo tenant |
| 27 | **RBAC** | Role-Based Access Control – Phân quyền theo vai trò |
| 28 | **ABAC** | Attribute-Based Access Control – Phân quyền theo thuộc tính |
| 29 | **SmartOffice** | Tên mã nền tảng phần mềm Văn phòng số Bộ Tài Chính |
| 30 | **Backend Core** | Lõi backend .NET 10 Clean Architecture của SmartOffice (`qlvb-backend-core`) |
| 31 | **BFF API** | Backend dành cho Frontend, dùng để gom các query cho UI |
| 32 | **DMS** | Document Management System – Hệ thống quản lý văn bản |
| 33 | **VPS** | Văn phòng số |
| 34 | **UAT** | User Acceptance Testing – Kiểm thử chấp nhận người dùng |
| 35 | **PII** | Personally Identifiable Information – Thông tin định danh cá nhân |
| 36 | **Ollama** | Công cụ chạy LLM, Embedding mã nguồn mở trên local/on-premise |
| 37 | **OpenAI** | Nhà cung cấp LLM thương mại (gpt-4o, text-embedding-3-small, ...) |
| 38 | **Entity / Aggregate Root** | Khái niệm trong Domain-Driven Design, thực thể gốc của một aggregate |
| 39 | **Vertical Slice** | Kiếu cắt dọc tổ chức code theo feature/Use case trong Clean Architecture |
| 40 | **HA** | High Availability – Tính sẵn sàng cao |
| 41 | **DR** | Disaster Recovery – Khôi phục sau sự cố |
| 42 | **SLA** | Service Level Agreement – Cam kết mức dịch vụ |
| 43 | **JWT** | JSON Web Token – Chuẩn token xác thực |
| 44 | **OpenIddict** | Server OAuth2/OpenID Connect cho .NET |
| 45 | **MassTransit** | Thư viện hỗ trợ message bus cho .NET |
| 46 | **DbUp** | Công cụ quản lý schema migration bằng SQL script |
| 47 | **Prompt** | Đoạn văn bản đầu vào gửi cho LLM |
| 48 | **Token** | Đơn vị nhỏ nhất mà LLM xử lý (khoảng 4 ký tự tiếng Anh hoặc 1-2 âm tiết tiếng Việt) |
| 49 | **Temperature** | Tham số điều khiển độ "sáng tạo" của LLM (0 = chính xác, 1 = sáng tạo) |
| 50 | **RAGAS** | Framework đánh giá chất lượng hệ thống RAG (Context Precision, Context Recall, Faithfulness, Answer Relevancy) |
| 51 | **MOBIFONE Solutions** | Đơn vị thi công dự án Văn phòng số Bộ Tài Chính |
| 52 | **PSD / PSDK** | Bộ phát triển phần mềm (Software Development Kit) cho Plugin AI |
| 53 | **Plugin SDK** | Bộ công cụ và giao diện lập trình cho phép mở rộng Hệ thống AI bằng plugin |
| 54 | **Cross-Encoder** | Mô hình reranking đánh giá cặp (query, document) để sắp xếp lại kết quả |
