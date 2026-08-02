# MỤC LỤC

| Nội dung | Trang |
|----------|-------|
| LỜI NÓI ĐẦU | 1 |
| MỤC LỤC | 2 |
| DANH MỤC BẢNG | 4 |
| DANH MỤC HÌNH | 5 |
| DANH MỤC CHỮ CÁI VIẾT TẮT | 6 |
| | |
| **MỞ ĐẦU** | **8** |
| 1. Lý do chọn đề tài | 8 |
| 2. Tổng quan về vấn đề nghiên cứu | 9 |
| 2.1. Giới thiệu lĩnh vực nghiên cứu | 9 |
| 2.2. Các hướng tiếp cận chính | 10 |
| 2.3. Khoảng trống nghiên cứu và vấn đề đặt ra | 10 |
| 3. Mục đích nghiên cứu | 11 |
| 3.1. Mục tiêu tổng quát | 11 |
| 3.2. Mục tiêu cụ thể | 12 |
| 4. Đối tượng và phạm vi nghiên cứu | 13 |
| 4.1. Đối tượng nghiên cứu | 13 |
| 4.2. Phạm vi nghiên cứu | 13 |
| 5. Phương pháp nghiên cứu | 14 |
| | |
| **NỘI DUNG** | **16** |
| **Chương 1: CƠ SỞ LÝ LUẬN** | **16** |
| 1.1. Tổng quan về trí tuệ nhân tạo và nền tảng AI | 16 |
| 1.2. Mô hình ngôn ngữ lớn (LLM) | 17 |
| 1.2.1. Khái niệm | 17 |
| 1.2.2. Các mô hình tiêu biểu | 18 |
| 1.2.3. Hạn chế | 20 |
| 1.3. Retrieval-Augmented Generation (RAG) | 20 |
| 1.3.1. Khái niệm | 20 |
| 1.3.2. Kiến trúc và quy trình hoạt động | 21 |
| 1.3.3. Vai trò của RAG trong hệ thống ứng dụng LLM | 22 |
| 1.3.4. Ưu điểm và hạn chế | 23 |
| 1.4. AI Agent và Multi-Agent Orchestration | 24 |
| 1.4.1. Khái niệm AI Agent | 24 |
| 1.4.2. Kiến trúc Agent | 25 |
| 1.4.3. Model Context Protocol (MCP) | 26 |
| 1.5. Hybrid Search và Reranking | 26 |
| 1.5.1. Hybrid Search | 26 |
| 1.5.2. Reranking | 27 |
| 1.6. Kiến trúc Enterprise AI Platform | 28 |
| 1.6.1. Đặc trưng của nền tảng AI doanh nghiệp | 28 |
| 1.6.2. Kiến trúc Modular Monolith | 29 |
| 1.6.3. Kiến trúc Plugin-based | 30 |
| 1.7. Các nghiên cứu và hệ thống liên quan | 31 |
| 1.7.1. Các framework AI Platform hiện có | 31 |
| 1.7.2. Các hệ thống tích hợp LLM và Agent | 32 |
| 1.7.3. Khoảng trống nghiên cứu | 33 |
| 1.8. Kết luận chương 1 | 34 |
| | |
| **Chương 2: PHÂN TÍCH BÀI TOÁN VÀ THIẾT KẾ NỀN TẢNG** | **35** |
| 2.1. Phân tích bài toán | 35 |
| 2.1.1. Mô tả bài toán và người dùng | 35 |
| 2.1.2. Yêu cầu chức năng | 36 |
| 2.1.3. Yêu cầu phi chức năng | 38 |
| 2.2. Kiến trúc hệ thống | 40 |
| 2.2.1. Kiến trúc tổng thể bảy tầng | 40 |
| 2.2.2. Luồng xử lý chính | 43 |
| 2.2.3. Công nghệ sử dụng | 46 |
| 2.3. Thiết kế module và plugin | 49 |
| 2.3.1. Cấu trúc Solution | 49 |
| 2.3.2. Module Identity và Tenant | 50 |
| 2.3.3. Module AI Engine Core | 51 |
| 2.3.4. Module Agent và MCP | 52 |
| 2.3.5. Plugin SDK | 53 |
| 2.4. Thiết kế dữ liệu | 54 |
| 2.4.1. Cơ sở dữ liệu quan hệ | 54 |
| 2.4.2. Cơ sở dữ liệu vector (pgvector) | 57 |
| 2.4.3. Kho tri thức | 58 |
| 2.5. Thiết kế hệ thống RAG nâng cao | 60 |
| 2.5.1. Pipeline RAG nâng cao | 60 |
| 2.5.2. Hybrid Search và Reranking | 62 |
| 2.5.3. Tối ưu ngữ cảnh | 64 |
| 2.5.4. Sinh phản hồi và trích dẫn nguồn | 65 |
| 2.6. Kết luận chương 2 | 66 |
| | |
| **Chương 3: THỰC NGHIỆM VÀ ĐÁNH GIÁ KẾT QUẢ** | **68** |
| 3.1. Thiết lập thực nghiệm | 68 |
| 3.1.1. Mục tiêu và phương pháp | 68 |
| 3.1.2. Môi trường thực nghiệm và cấu hình | 69 |
| 3.1.3. Bộ dữ liệu thực nghiệm | 70 |
| 3.1.4. Kịch bản thực nghiệm | 71 |
| 3.2. Xây dựng hệ thống thực nghiệm | 72 |
| 3.2.1. Tổng quan các thành phần triển khai | 72 |
| 3.2.2. Một số điểm triển khai đáng chú ý | 73 |
| 3.2.3. Kết quả đầu ra của hệ thống | 74 |
| 3.3. Kết quả và đánh giá | 75 |
| 3.3.1. Đánh giá chất lượng RAG | 75 |
| 3.3.2. Đánh giá khả năng tích hợp đa hệ thống | 77 |
| 3.3.3. So sánh các cấu hình LLM provider | 78 |
| 3.3.4. Đánh giá khả năng mở rộng plugin | 79 |
| 3.3.5. Tổng hợp kết quả đánh giá | 80 |
| 3.4. Kết luận chương 3 | 82 |
| | |
| **KẾT LUẬN** | **83** |
| **DANH MỤC CÁC TÀI LIỆU THAM KHẢO** | **85** |
