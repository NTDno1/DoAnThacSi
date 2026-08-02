# KẾT LUẬN VÀ HƯỚNG PHÁT TRIỂN

## 1. Tổng kết kết quả nghiên cứu

Trong quá trình thực hiện đề tài, đồ án đã đạt được các kết quả nghiên cứu sau:

**Về nghiên cứu lý thuyết**, đồ án đã tổng hợp và phân tích một cách hệ thống các công nghệ nền tảng cho bài toán tích hợp AI vào doanh nghiệp, bao gồm: mô hình ngôn ngữ lớn (LLM) với kiến trúc Transformer và các mô hình tiêu biểu; kỹ thuật Retrieval-Augmented Generation (RAG) ở cả mức cơ bản và nâng cao với Hybrid Search, Cross-Encoder Reranking, Context Compression; AI Agent và Multi-Agent Orchestration với các pattern điều phối; giao thức Model Context Protocol (MCP); và các phương pháp tích hợp hệ thống doanh nghiệp truyền thống. Qua khảo sát toàn diện, đồ án đã xác định được bốn khoảng trống nghiên cứu chính: thiếu kiến trúc tổng thể tích hợp nhiều công nghệ AI; thiếu hỗ trợ MCP ở mức production-grade; thiếu hướng dẫn triển khai từng bước (Phase); và thiếu giải pháp phù hợp cho doanh nghiệp nhỏ và vừa.

**Về kiến trúc hệ thống**, đồ án đã đề xuất một kiến trúc tham chiếu toàn diện cho nền tảng Enterprise AI Platform với 7 tầng rõ ràng: Presentation, AI Gateway, Agent Orchestration, AI Engine Core, Provider Abstraction, Integration và Platform Core. Kiến trúc tuân thủ 8 nguyên tắc thiết kế cốt lõi: Plug & Play, Provider Agnostic, Domain Aware, Event Driven, Agent Native, Offline First, Enterprise Grade và Multi-Tenant. Chiến lược triển khai theo Modular Monolith (đầu) chuyển sang Microservice (khi cần) cho phép cân bằng giữa đơn giản và khả năng mở rộng.

**Về thiết kế module**, đồ án đã chi tiết thiết kế các module cốt lõi bao gồm: ILLMProvider và IEmbeddingProvider interface với 5 implementation (Ollama, OpenAI, Anthropic, Azure OpenAI, Gemini); Provider Router với chiến lược chọn provider tối ưu; RAG Engine với Hybrid Search (BM25 + Vector, RRF Fusion); Agent Orchestration với Intent Router, Task Planner, Executor, Safety Guardrails và HITL; AI Gateway với AuthN/AuthZ, Rate Limiting, Observability; Integration Layer với MCP Server/Client và Plugin SDK.

**Về thiết kế dữ liệu**, đồ án đã đề xuất lược đồ PostgreSQL với 4 nhóm bảng chính, tích hợp pgvector cho dữ liệu vector, cơ chế Multi-tenant với Row-Level Security và Enterprise Knowledge Base cho phép tổ chức tri thức doanh nghiệp một cách có hệ thống.

**Về triển khai thực nghiệm**, đồ án đã triển khai thành công hệ thống Walking Skeleton chạy end-to-end qua tất cả các layer, bao gồm Phase 1 (Foundation), Phase 2 (AI Engine & RAG) và Phase 3 (Agent & Integration). Hệ thống tích hợp được 3 hệ thống mô phỏng (ERP, CRM, DMS) qua MCP, sử dụng 4 LLM provider khác nhau và đạt tỷ lệ tích hợp thành công 90%.

**Về đánh giá**, đồ án đã thực hiện đánh giá toàn diện trên 4 kịch bản với 50 query thực nghiệm. Kết quả cho thấy: chất lượng RAG đạt trung bình 0.87/1.0 theo thang RAGAS với Gemini 2.5 Flash; khả năng tích hợp đa hệ thống đạt 90%; latency trung bình 1.8s cho RAG đơn và 6.5s cho multi-agent workflow; chi phí vận hành rất thấp với Gemini Flash ($0.000075/1K tokens input).

## 2. Đóng góp của đề tài

Đề tài mang lại các đóng góp mới sau:

**Đóng góp về kiến trúc hệ thống**: đồ án đề xuất kiến trúc 7 tầng toàn diện, tích hợp đồng thời LLM, RAG, AI Agent, MCP trong một framework thống nhất. Kiến trúc này có tính tổng quát cao, có thể được tham chiếu và điều chỉnh cho nhiều bối cảnh doanh nghiệp khác nhau. Đặc biệt, việc đặt Provider Abstraction làm lớp nền tảng cho phép toàn bộ hệ thống độc lập với AI provider cụ thể.

**Đóng góp về phương pháp tích hợp**: đồ án sử dụng giao thức MCP như cơ chế chuẩn hóa cho việc kết nối AI với hệ thống doanh nghiệp. Kết quả thực nghiệm cho thấy chỉ cần ~2 giờ để tích hợp một hệ thống mới, thay vì hàng tuần/hàng tháng như các phương pháp truyền thống. Đây là một cải tiến đáng kể về mặt thời gian và chi phí tích hợp.

**Đóng góp về chiến lược triển khai**: đồ án đề xuất chiến lược 5 Phase với nguyên tắc Walking Skeleton và Vertical Slice, cho phép có sản phẩm chạy được ở mỗi giai đoạn và có thể đánh giá sớm với stakeholder. Chiến lược này phù hợp với thực tế phát triển phần mềm tại Việt Nam.

**Đóng góp về đánh giá**: đồ án xây dựng bộ tiêu chí đánh giá toàn diện cho nền tảng AI doanh nghiệp, kết hợp cả chỉ số định lượng (RAGAS, latency, throughput) và đánh giá định tính (khả năng tích hợp, tính khả thi kiến trúc). Bộ tiêu chí này có thể được sử dụng làm khung đánh giá cho các nghiên cứu tương tự.

**Đóng góp về bối cảnh Việt Nam**: đề tài là một trong những công trình đầu tiên nghiên cứu và xây dựng kiến trúc tham chiếu Enterprise AI Platform phù hợp với bối cảnh doanh nghiệp Việt Nam, nơi các hệ thống thường có quy mô vừa và nhỏ, hạ tầng tự xây dựng và yêu cầu chạy cả online lẫn offline.

## 3. Hạn chế của đề tài

Bên cạnh các kết quả đạt được, đồ án còn một số hạn chế cần được ghi nhận:

**Về phạm vi triển khai**: mới triển khai và đánh giá ở Phase 1-3, chưa triển khai đầy đủ Phase 4 (Hybrid Search Reranker, Voice Engine, SQL Engine, Memory nâng cao) và Phase 5 (Full multi-tenancy, RBAC/ABAC hoàn chỉnh, Audit nâng cao, Kubernetes deployment). Việc đánh giá toàn diện chưa thể thực hiện do thời gian nghiên cứu có hạn.

**Về dữ liệu thực nghiệm**: bộ dữ liệu sử dụng trong thực nghiệm chủ yếu là dữ liệu mô phỏng (mock data), chưa có dữ liệu thực từ doanh nghiệp. Các hệ thống ERP, CRM, DMS được mô phỏng qua MCP mock connector, chưa tích hợp thực với các hệ thống thương mại (SAP, Oracle, Salesforce…). Điều này ảnh hưởng đến tính đại diện của kết quả đánh giá.

**Về đánh giá người dùng**: chưa có đánh giá từ người dùng thực tế (end-user evaluation). Các đánh giá hiện tại chủ yếu dựa trên các chỉ số kỹ thuật và kịch bản mô phỏng. Đánh giá về trải nghiệm người dùng, sự hài lòng và hiệu quả công việc thực tế chưa được thực hiện.

**Về Multi-Agent**: Task Planner hiện tại đạt độ chính xác 60-80% trên các kịch bản phức tạp, còn hạn chế trong xử lý conditional branching và multi-step reasoning dài. Cần cải thiện thuật toán planning để đạt độ chính xác cao hơn.

**Về chi phí và tối ưu**: mặc dù chi phí vận hành thấp với Gemini Flash, hệ thống chưa có cơ chế caching thông minh (smart caching) và model routing tự động dựa trên độ phức tạp của tác vụ.

## 4. Hướng phát triển tiếp theo

Dựa trên các kết quả đạt được và hạn chế đã nhận diện, đồ án đề xuất các hướng phát triển tiếp theo:

**Hoàn thiện Phase 4 và Phase 5**: tiếp tục triển khai các module còn lại bao gồm: Cross-Encoder Reranker để cải thiện chất lượng truy xuất; Voice Engine (STT/TTS) cho phép tương tác bằng giọng nói; SQL Engine (Text-to-SQL) cho phép truy vấn database tự nhiên; Memory Manager nâng cao với long-term memory và preference learning; Workflow Engine với BPMN support; đầy đủ RBAC/ABAC; Kubernetes deployment với auto-scaling.

**Tích hợp hệ thống thực tế**: triển khai thực tế với các hệ thống doanh nghiệp Việt Nam như: Odoo, ERPNext, Base CRM, Haravan, MISA, hoặc các hệ thống tự xây dựng. Thực hiện đánh giá với dữ liệu thực và người dùng thực tế.

**Cải thiện Multi-Agent Planner**: nghiên cứu và tích hợp các thuật toán planning tiên tiến hơn (ReAct, Tree-of-Thought, LLM-based planner) để nâng cao độ chính xác của plan trong các kịch bản phức tạp. Bổ sung cơ chế self-correction và reflection.

**Tối ưu chi phí và hiệu năng**: phát triển cơ chế smart caching sử dụng LLM để dự đoán và cache trước các truy vấn có khả năng lặp lại; triển khai automatic model routing dựa trên độ phức tạp tác vụ (simple query → Llama local, complex query → Claude/Gemini); bổ sung batch processing cho các tác vụ bulk.

**Nghiên cứu AI Governance**: phát triển cơ chế AI Governance toàn diện bao gồm: audit trail chi tiết cho mọi AI decision; explainability để giải thích tại sao AI đưa ra quyết định; bias detection để phát hiện và giảm thiểu bias trong câu trả lời; compliance checker để đảm bảo AI tuân thủ các quy định.

**Ứng dụng cho các lĩnh vực cụ thể**: mở rộng kiến trúc cho các lĩnh vực chuyên biệt như: tài chính – ngân hàng (compliance, risk assessment), y tế (hồ sơ bệnh nhân, quản lý thuốc), giáo dục (hệ thống quản lý đào tạo), sản xuất (quản lý dây chuyền).

**Nghiên cứu continual learning**: phát triển cơ chế cho phép nền tảng tự học từ phản hồi người dùng (human feedback), cập nhật embedding và RAG index một cách tự động khi tri thức mới được xác nhận là đúng.

**Đánh giá người dùng thực tế**: thiết kế và thực hiện user study với người dùng doanh nghiệp thực tế, đánh giá về: task completion rate, time savings, user satisfaction (SUS score), adoption rate.

Tóm lại, đồ án đã hoàn thành mục tiêu nghiên cứu và xây dựng một kiến trúc tham chiếu khả thi cho nền tảng Enterprise AI Platform, với các kết quả thực nghiệm cho thấy tính đúng đắn của cách tiếp cận. Các hạn chế đã được nhận diện và hướng phát triển tiếp theo được đề xuất rõ ràng, tạo nền tảng cho các nghiên cứu và triển khai tiếp theo.

---

**Nguyễn Tiến Đạt**
*Hà Nội, 2026*
