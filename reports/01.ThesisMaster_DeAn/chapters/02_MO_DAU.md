# MỞ ĐẦU

## 1. Lý do chọn đề tài

Trí tuệ nhân tạo đang chuyển từ công cụ phân tích chuyên biệt sang năng lực dùng chung trong hệ thống thông tin doanh nghiệp. Chiến lược quốc gia về AI đến năm 2030 đặt yêu cầu ứng dụng AI trong hoạt động kinh tế - xã hội [1]. Kiến trúc Transformer [19] và mô hình ngôn ngữ lớn (LLM) [3] cho phép xử lý ngôn ngữ tự nhiên ở quy mô lớn, nhưng cũng mang theo rủi ro khi triển khai rộng [2].

Trong doanh nghiệp, tri thức phân tán giữa tài liệu, cơ sở dữ liệu và nhiều hệ thống nghiệp vụ. Retrieval-Augmented Generation (RAG) bổ sung cơ chế truy xuất tài liệu trước khi sinh câu trả lời [7], nhưng cần giải quyết thêm nhập liệu, chia đoạn, tìm kiếm lai, phân quyền và đánh giá tính trung thực. Model Context Protocol (MCP) chuẩn hóa giao tiếp context/tool giữa ứng dụng LLM và hệ thống bên ngoài [10], song chưa tự giải quyết bảo mật và quản trị.

Từ bối cảnh trên, đề tài **"Nghiên cứu và xây dựng nền tảng Enterprise AI Platform tích hợp LLM, RAG, AI Agent và MCP"** được lựa chọn nhằm tiếp cận AI như một năng lực nền tảng dùng chung, đặt giữa ứng dụng người dùng, nguồn tri thức, mô hình AI và hệ thống nghiệp vụ.

## 2. Tổng quan về vấn đề nghiên cứu

Đề tài nằm ở giao điểm bốn lĩnh vực: (1) xử lý ngôn ngữ tự nhiên với LLM và embedding; (2) truy xuất thông tin gồm tìm kiếm thưa, vector, lai và xếp hạng lại; (3) tích hợp hệ thống doanh nghiệp; (4) quản trị AI gồm bảo mật, kiểm toán và giám sát.

Các công trình hiện có cung cấp từng khối kỹ thuật [19], [7], [10], [20] nhưng chưa tạo thành kiến trúc doanh nghiệp hoàn chỉnh. Bốn khoảng trống chính: vòng đời tài liệu chưa nối xuyên suốt tới prompt; ranh giới trả lời và thực thi chưa rõ [13]; provider abstraction chưa giải quyết hết khác biệt capability; kiến trúc mục tiêu thường rộng hơn bằng chứng hiện thực.

## 3. Mục đích nghiên cứu

Nghiên cứu, đề xuất và hiện thực ở mức nguyên mẫu một kiến trúc Enterprise AI Platform kết hợp LLM, RAG, AI Agent và MCP. Kết quả cần đạt:

- Hệ thống hóa cơ sở lý luận về LLM, embedding, vector search, RAG, hybrid search, AI Agent, MCP và tích hợp doanh nghiệp.
- Phân tích yêu cầu và đề xuất kiến trúc phân lớp với provider abstraction, RAG engine, agent orchestration và integration layer.
- Xây dựng nguyên mẫu trên .NET 8, Next.js, PostgreSQL/pgvector trong Docker.
- Xây dựng bộ tiêu chí đánh giá cho retrieval, RAG, agent và hệ thống.
- Nhận diện hạn chế và đề xuất lộ trình hoàn thiện.

## 4. Đối tượng và phạm vi nghiên cứu

**Đối tượng nghiên cứu:** các mô hình, phương pháp và thành phần phần mềm cần thiết để xây dựng lớp nền tảng AI dùng chung cho hệ thống doanh nghiệp (embedding, vector search, RAG, AI Agent, MCP, provider abstraction, quản trị).

**Phạm vi nghiên cứu:** tập trung vào ba ca sử dụng: quản lý và lập chỉ mục tài liệu; tìm kiếm/hỏi đáp dựa trên RAG; thử nghiệm tác nhân gọi công cụ qua MCP. Nguyên mẫu giới hạn ở backend ASP.NET Core/.NET 8 [9], frontend Next.js, PostgreSQL/pgvector và Docker. Đề tài không huấn luyện LLM mới, không thực hiện fine-tuning quy mô lớn.

## 5. Phương pháp nghiên cứu

- **Nghiên cứu tài liệu:** tổng hợp tài liệu về LLM, RAG, agent, MCP, microservices và quản trị AI [19]–[5].
- **Phân tích và mô hình hóa:** phân tích tác nhân, ca sử dụng, ranh giới tin cậy và thuộc tính chất lượng.
- **Thiết kế và xây dựng nguyên mẫu:** áp dụng design science, đề xuất kiến trúc, hiện thực lát cắt dọc.
- **Thực nghiệm và đánh giá:** tách chất lượng retrieval, generation, agent; tham khảo RAGAS [5].
