# Chương 3: HIỆN THỰC HÓA PROTOTYPE VÀ ĐÁNH GIÁ

## Giới thiệu chương

Chương này trình bày kết quả hiện thực hóa prototype và đánh giá mức độ đáp ứng kiến trúc đề xuất. Phương pháp đánh giá dựa trên truy nguyên bằng chứng từ mã nguồn, cấu hình và hiện vật trong repository.

## Nội dung

Trình bày phần hiện thực và kết quả thực nghiệm để minh chứng cho các thiết kế trong Chương 2:

**Mục tiêu và phương pháp đánh giá.** Bốn mục tiêu: xác định hiện vật mã nguồn tương ứng kiến trúc; kiểm tra tính nhất quán; nhận diện khoảng trống; thiết kế quy trình thực nghiệm tái lập. Sử dụng thang năm mức bằng chứng (E0-Thiết kế → E4-Runtime đã kiểm chứng).

**Hiện vật prototype.** Backend ASP.NET Core: controller/service cho auth, document, search, chat, agent, admin; provider Ollama/OpenAI; RAG, MCP, SQL. Frontend Next.js: các trang login, document, search, chat, agent, admin. Hạ tầng Docker Compose: PostgreSQL/pgvector, Redis, MinIO, Ollama, Seq.

**Đánh giá tĩnh và ma trận hiện thực.** Ma trận chức năng theo mức bằng chứng. Tính nhất quán xuyên tầng (backend ↔ frontend ↔ database ↔ Docker). Kết quả có thể khẳng định và kết quả cần thêm bằng chứng.

**Bất nhất và rủi ro kỹ thuật.** Các lỗi ngăn build, rủi ro dữ liệu, rủi ro tích hợp AI, rủi ro frontend và rủi ro bảo mật.

**Thiết kế thực nghiệm tái lập.** Điều kiện tiên quyết, bộ dữ liệu đánh giá, đánh giá retrieval/RAG (Recall@k, MRR, nDCG, Faithfulness, Citation Accuracy), đánh giá Agent/MCP, đánh giá tải và bảo mật.

**Thảo luận.** Đối chiếu kết quả với mục tiêu, phân tích đe dọa tính hợp lệ.

## Kết luận chương

Prototype có độ phủ mã rộng nhưng chưa đạt build/test/runtime ổn định. Đánh giá tĩnh cho phép xác định trạng thái hiện thực và khoảng trống. Thiết kế thực nghiệm tái lập tạo cơ sở cho đánh giá định lượng ở giai đoạn tiếp theo.
