# KẾT LUẬN

## 1. Các kết quả đạt được

- Hệ thống hóa cơ sở lý luận về LLM, embedding, RAG, hybrid search, AI Agent, MCP và quản trị AI.
- Đề xuất kiến trúc phân lớp cho Enterprise AI Platform với provider abstraction, RAG engine, agent orchestration và integration layer.
- Xây dựng mã nguồn prototype trên .NET 8, Next.js, PostgreSQL/pgvector và Docker Compose.
- Thực hiện đánh giá tĩnh trên hiện vật mã nguồn, nhận diện hạn chế và khoảng trống kỹ thuật.
- Thiết kế quy trình thực nghiệm tái lập cho giai đoạn tiếp theo.

## 2. Nhận xét, đề xuất, khuyến nghị

Prototype có phạm vi rộng nhưng chưa đạt build/test ổn định. Cần ưu tiên sửa build, bổ sung test tự động, thống nhất schema và cấu hình trước khi mở rộng tính năng. Đề xuất tuân thủ lộ trình: sửa build → kiểm thử → thực nghiệm → hardening. Các kết luận chỉ nên dựa trên bằng chứng kiểm chứng được.

## 3. Hướng nghiên cứu tiếp theo

- Sửa build và thống nhất nền tảng (DbContext, schema, config, vector dimension).
- Thiết lập quality gate bằng unit test và integration test.
- Thực hiện đánh giá định lượng: retrieval (Recall@k, MRR, nDCG), RAG (Faithfulness, Citation Accuracy), Agent/MCP.
- Enterprise hardening: RBAC/ABAC, RLS, audit, secret management, observability, CI/CD.
