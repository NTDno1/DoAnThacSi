# CHƯƠNG 1. CƠ SỞ LÝ LUẬN

## Giới thiệu chương

Chương này trình bày nền tảng lý thuyết cho việc xây dựng Enterprise AI Platform, bao gồm: mô hình ngôn ngữ lớn, embedding và vector search, Retrieval-Augmented Generation, tìm kiếm lai, AI Agent, Model Context Protocol, tích hợp hệ thống doanh nghiệp và quản trị AI.

## Nội dung

Khái quát và hệ thống hóa cơ sở lý luận về các vấn đề nghiên cứu chính:

**Mô hình ngôn ngữ lớn (LLM).** Trình bày kiến trúc Transformer [19], cơ chế self-attention, mô hình nền tảng [2] và các hạn chế của LLM trong bối cảnh doanh nghiệp: hallucination, thiếu nguồn gốc, tính không xác định, giới hạn ngữ cảnh. Từ đó xác lập yêu cầu về provider abstraction và kiểm soát ngữ cảnh.

**Embedding và tìm kiếm vector.** Biểu diễn ngữ nghĩa bằng vector, Sentence-BERT [15], Dense Passage Retrieval [6]. Chia đoạn (chunking) và metadata. Tìm kiếm chính xác và tìm kiếm gần đúng với HNSW [8]. pgvector cho PostgreSQL [14].

**Retrieval-Augmented Generation (RAG).** Khái niệm RAG [7]: kết hợp bộ nhớ tham số với bộ nhớ phi tham số. Pipeline nhập liệu (ingestion): tiếp nhận, trích xuất, chia đoạn, embedding, lập chỉ mục. Pipeline truy vấn: query processing, retrieval, context assembly, generation, citation.

**Tìm kiếm lai, RRF và reranking.** BM25 cho tìm kiếm từ khóa [16], dense retrieval cho ngữ nghĩa [6], Reciprocal Rank Fusion (RRF) hợp nhất thứ hạng [4], reranking với BERT [12].

**AI Agent và sử dụng công cụ.** Khái niệm tác nhân, mẫu ReAct [20], tool use và Toolformer [17], vòng lặp planner-executor-memory. Kiểm soát thực thi và human-in-the-loop.

**Model Context Protocol (MCP).** Kiến trúc host-client-server, các primitive (resources, prompts, tools), giao tiếp JSON-RPC/SSE, bảo mật MCP [10].

**Microservices và tích hợp doanh nghiệp.** Đặc trưng tích hợp doanh nghiệp, microservices vs modular monolith [11], provider abstraction, multi-tenancy.

**Quản trị, bảo mật và đánh giá AI.** NIST AI RMF [18], OWASP Top 10 for LLM [13], guardrail, audit, observability. Khung đánh giá RAG với RAGAS [5].

**Nghiên cứu liên quan và khoảng trống.** Tổng hợp các hướng nghiên cứu hiện có và xác định năm khoảng trống mà đề tài tập trung giải quyết.

## Kết luận chương

Chương 1 xác lập cơ sở lý luận cho Enterprise AI Platform. LLM cần được bao quanh bởi tri thức ngoài mô hình và cơ chế quản trị. Embedding, BM25, vector search, RRF và reranking tạo pipeline retrieval; RAG nối retrieval với sinh câu trả lời có nguồn. Agent mở rộng từ trả lời sang hành động nên cần kiểm soát tool. MCP chuẩn hóa giao tiếp nhưng không thay thế authorization. Các khái niệm này là căn cứ để Chương 2 phân tích yêu cầu và đề xuất kiến trúc.
