# Chương 2: PHÂN TÍCH BÀI TOÁN VÀ THIẾT KẾ NỀN TẢNG

## Giới thiệu chương

Chương này phân tích yêu cầu và thiết kế Enterprise AI Platform. Nội dung phân biệt kiến trúc đề xuất (trạng thái đích) với prototype hiện có trong mã nguồn.

## Nội dung

Khái quát thực trạng, phân tích và thiết kế giải pháp trên cơ sở lý thuyết đã trình bày ở Chương 1:

**Phân tích bài toán và yêu cầu.** Phát biểu bài toán: thiết kế nền tảng AI mở, hỗ trợ triển khai cục bộ, chuẩn hóa truy cập LLM/embedding, cung cấp RAG và Agent qua API, tạo nền tảng cho quản trị và tích hợp. Xác định bốn nhóm người dùng (nhân viên khai thác tri thức, quản lý nội dung, quản trị viên, lập trình viên tích hợp). Phạm vi chức năng, các ca sử dụng chính và yêu cầu phi chức năng.

**Nguyên tắc thiết kế và kiến trúc tổng thể.** Đề xuất kiến trúc bảy tầng: Presentation, API Gateway, Application Services, AI Engine (RAG, Agent, Provider), Integration (MCP, Connector), Data & Knowledge, Infrastructure. Ranh giới phân hệ, luồng xử lý một yêu cầu AI, Clean Architecture.

**Thiết kế dữ liệu và kho tri thức.** Phân lớp dữ liệu (quan hệ, vector, file, cache). Mô hình logic: Users, Tenants, Documents, Chunks, Embeddings, Conversations, AuditLogs. Thiết kế chunk, vector dimension, metadata, nhất quán và cách ly tenant.

**Thiết kế quy trình tiếp nhận tài liệu.** Pipeline ingestion: upload → validate → extract text → chunk → embed → index. Chiến lược chunking và kiểm soát chất lượng.

**Thiết kế Semantic Search và RAG.** Truy hồi hybrid (BM25 + dense), RRF, reranking, context assembly, prompt construction, streaming response, caching. Kế hoạch đánh giá retrieval và generation.

**Provider abstraction, Agent, Tool và MCP.** Trừu tượng hóa nhà cung cấp AI (Ollama, OpenAI). Thiết kế Agent với ReAct loop. Tool registry và kiểm soát thực thi. MCP client/server và tích hợp hệ thống.

**API, bảo mật, multi-tenant và governance.** Thiết kế REST API, mô hình bảo mật nhiều lớp (JWT, RBAC), multi-tenant, governance và audit trail.

**Kiến trúc triển khai.** Lựa chọn công nghệ (.NET 8 [9], Next.js, PostgreSQL/pgvector [14], Docker). Topology phát triển với Docker Compose. Kiến trúc triển khai mục tiêu và lộ trình hiện thực hóa.

## Kết luận chương

Chương 2 đã phân tích bài toán, xác định yêu cầu và đề xuất kiến trúc phân lớp cho Enterprise AI Platform. Thiết kế bao phủ dữ liệu, ingestion, RAG, agent/MCP, API, bảo mật và triển khai. Các quyết định thiết kế sẽ được kiểm chứng qua prototype ở Chương 3.
