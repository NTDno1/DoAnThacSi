<!-- Bao cao thuc tap - Nguyen Tien Dat -->

# BÁO CÁO THỰC TẬP TỐT NGHIỆP

## NGHIÊN CỨU VÀ XÂY DỰNG NỀN TẢNG AI DOANH NGHIỆP HƯỚNG TÍCH HỢP ĐA HỆ THỐNG (ENTERPRISE AI PLATFORM FOR CROSS-SYSTEM INTEGRATION)

---

**HỌC VIỆN CÔNG NGHỆ BƯU CHÍNH VIỄN THÔNG**

**Học viên:** Nguyễn Tiến Đạt

**Chuyên ngành:** Hệ thống Thông tin

**Mã số:** 8.48.01.04

**BÁO CÁO THỰC TẬP TỐT NGHIỆP**

**(Theo định hướng ứng dụng)**

**NGƯỜI HƯỚNG DẪN KHOA HỌC: PGS.TS. TRẦN ĐÌNH QUẾ**

**HÀ NỘI – 2026**

---

## LỜI NÓI ĐẦU

Trong bối cảnh chuyển đổi số mạnh mẽ của các doanh nghiệp Việt Nam hiện nay, nhu cầu tích hợp trí tuệ nhân tạo (AI) vào các hệ thống quản lý doanh nghiệp như ERP, CRM, DMS, HRM ngày càng trở nên cấp thiết. Tuy nhiên, phần lớn các giải pháp AI hiện có vẫn tồn tại nhiều hạn chế về khả năng tích hợp, tính mở rộng và chi phí triển khai. Các tổ chức thường phải xây dựng nhiều giải pháp AI riêng lẻ cho từng hệ thống, dẫn đến tình trạng phân mảnh, khó bảo trì và tốn kém tài nguyên.

Xuất phát từ thực tế đó, báo cáo thực tập này tập trung nghiên cứu và xây dựng nền tảng AI doanh nghiệp (Enterprise AI Platform) hướng tới khả năng tích hợp đa hệ thống, cung cấp các khả năng AI sẵn sàng sử dụng cho nhiều hệ thống khác nhau thông qua kiến trúc module hóa, plugin-based và hỗ trợ nhiều AI provider. Nền tảng hướng tới các nguyên tắc Plug & Play, Provider Agnostic, Domain Aware, Event Driven, Agent Native, Offline First và Enterprise Grade.

Nội dung báo cáo được tổ chức thành ba chương chính. Chương 1 trình bày cơ sở lý luận về các mô hình ngôn ngữ lớn, kỹ thuật truy xuất tăng cường, AI Agent và kiến trúc AI Platform. Chương 2 phân tích bài toán và đề xuất kiến trúc hệ thống theo mô hình bảy tầng. Chương 3 trình bày quá trình thực nghiệm, đánh giá kết quả và các chỉ số RAGAS, thời gian phản hồi của hệ thống.

Báo cáo được hoàn thành dưới sự hướng dẫn tận tình của PGS.TS. Trần Đình Quế. Do thời gian và phạm vi nghiên cứu còn hạn chế, báo cáo không tránh khỏi những thiếu sót. Tác giả mong nhận được sự góp ý của các thầy cô và bạn đọc để nội dung nghiên cứu được hoàn thiện hơn.

**Hà Nội, ngày ... tháng ... năm 2026**

**Tác giả**

**Nguyễn Tiến Đạt**

---

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

---

# DANH MỤC BẢNG

| Bảng | Tiêu đề | Trang |
|------|---------|-------|
| Bảng 1.1 | Ưu điểm và hạn chế của RAG | 23 |
| Bảng 1.2 | Một số Enterprise AI Platform hiện có | 31 |
| Bảng 1.3 | Một số nghiên cứu về tích hợp LLM và Agent | 32 |
| Bảng 2.1 | Yêu cầu chức năng của hệ thống | 36 |
| Bảng 2.2 | Yêu cầu phi chức năng của hệ thống | 38 |
| Bảng 2.3 | Công nghệ sử dụng trong hệ thống | 46 |
| Bảng 2.4 | Cấu trúc dữ liệu vector embedding | 57 |
| Bảng 2.5 | So sánh RAG cơ bản và RAG nâng cao trong hệ thống | 60 |
| Bảng 2.6 | Cấu trúc đầu ra của hệ thống | 65 |
| Bảng 3.1 | Mục tiêu và phương pháp đánh giá thực nghiệm | 68 |
| Bảng 3.2 | Môi trường phần cứng và phần mềm | 69 |
| Bảng 3.3 | Cấu hình mô hình và hệ thống RAG | 69 |
| Bảng 3.4 | Thống kê dữ liệu kho tri thức | 70 |
| Bảng 3.5 | Kịch bản thực nghiệm | 71 |
| Bảng 3.6 | Thành phần triển khai trong hệ thống thực nghiệm | 72 |
| Bảng 3.7 | Thành phần kết quả đầu ra | 74 |
| Bảng 3.8 | Kết quả đánh giá RAGAS theo cấu hình | 75 |
| Bảng 3.9 | Kết quả đánh giá tích hợp Plugin | 77 |
| Bảng 3.10 | Kết quả so sánh các cấu hình LLM provider | 78 |
| Bảng 3.11 | Đánh giá hiệu năng khi mở rộng plugin | 79 |
| Bảng 3.12 | Tổng hợp kết quả theo bốn kịch bản thực nghiệm | 80 |
| Bảng 3.13 | Tổng hợp thời gian phản hồi của hệ thống | 80 |

# DANH MỤC HÌNH

| Hình | Tiêu đề | Trang |
|------|---------|-------|
| Hình 1.1 | Kiến trúc tổng quát của hệ thống RAG | 21 |
| Hình 2.1 | Kiến trúc tổng thể bảy tầng của Enterprise AI Platform | 40 |
| Hình 2.2 | Luồng xử lý chính của hệ thống | 43 |
| Hình 2.3 | Cấu trúc Solution của hệ thống | 49 |
| Hình 2.4 | Sơ đồ ERD các nhóm bảng dữ liệu chính | 54 |
| Hình 2.5 | Kiến trúc giai đoạn Retrieval | 62 |
| Hình 2.6 | Quy trình tối ưu ngữ cảnh | 64 |
| Hình 2.7 | Giai đoạn Generation & Citation | 65 |
| Hình 3.1 | Giao diện chat với plugin | 74 |
| Hình 3.2 | Dashboard quản trị Plugin | 76 |
| Hình 3.3 | Giao diện Agent Run Viewer | 78 |

# DANH MỤC CHỮ CÁI VIẾT TẮT

| Từ viết tắt | Tiếng Anh | Nghĩa tiếng Việt |
|-------------|-----------|-------------------|
| ABAC | Attribute-Based Access Control | Kiểm soát truy cập dựa trên thuộc tính |
| AI | Artificial Intelligence | Trí tuệ nhân tạo |
| API | Application Programming Interface | Giao diện lập trình ứng dụng |
| BERT | Bidirectional Encoder Representations from Transformers | Mô hình biểu diễn mã hóa hai chiều dựa trên Transformer |
| BM25 | Best Matching 25 | Thuật toán xếp hạng tìm kiếm từ khóa |
| CPU | Central Processing Unit | Bộ xử lý trung tâm |
| CRM | Customer Relationship Management | Quản lý quan hệ khách hàng |
| DDD | Domain-Driven Design | Thiết kế hướng miền |
| DMS | Document Management System | Hệ thống quản lý tài liệu |
| ERP | Enterprise Resource Planning | Hoạch định nguồn lực doanh nghiệp |
| GPT | Generative Pre-trained Transformer | Mô hình Transformer tiền huấn luyện sinh văn bản |
| GPU | Graphics Processing Unit | Bộ xử lý đồ họa |
| HITL | Human-in-the-Loop | Vòng lặp con người trong quá trình |
| HNSW | Hierarchical Navigable Small World | Chỉ mục tìm kiếm láng giềng gần đúng dạng đồ thị |
| HRM | Human Resource Management | Quản lý nhân sự |
| JWT | JSON Web Token | Token xác thực dạng JSON |
| LLM | Large Language Model | Mô hình ngôn ngữ lớn |
| MCP | Model Context Protocol | Giao thức ngữ cảnh mô hình |
| MLM | Masked Language Modeling | Mô hình hóa ngôn ngữ che giấu |
| NLP | Natural Language Processing | Xử lý ngôn ngữ tự nhiên |
| NLU | Natural Language Understanding | Hiểu ngôn ngữ tự nhiên |
| ORM | Object-Relational Mapping | Ánh xạ đối tượng - quan hệ |
| OSRM | Open Source Routing Machine | Máy định tuyến mã nguồn mở |
| RAG | Retrieval-Augmented Generation | Sinh nội dung tăng cường bằng truy xuất |
| RAGAS | Retrieval-Augmented Generation Assessment | Khung đánh giá hệ thống RAG |
| RAM | Random Access Memory | Bộ nhớ truy cập ngẫu nhiên |
| RBAC | Role-Based Access Control | Kiểm soát truy cập dựa trên vai trò |
| REST | Representational State Transfer | Kiến trúc truyền trạng thái đại diện |
| RRF | Reciprocal Rank Fusion | Hợp nhất xếp hạng nghịch đảo |
| RLS | Row-Level Security | Bảo mật cấp dòng |
| SDK | Software Development Kit | Bộ công cụ phát triển phần mềm |
| SQL | Structured Query Language | Ngôn ngữ truy vấn có cấu trúc |
| SSE | Server-Sent Events | Sự kiện gửi từ máy chủ |
| STT | Speech-to-Text | Chuyển giọng nói thành văn bản |
| TTS | Text-to-Speech | Chuyển văn bản thành giọng nói |
| UI | User Interface | Giao diện người dùng |
| YARP | Yet Another Reverse Proxy | Reverse Proxy cho .NET |

---

# MỞ ĐẦU

## 1. Lý do chọn đề tài

Trong những năm gần đây, quá trình chuyển đổi số tại các doanh nghiệp Việt Nam đã và đang diễn ra mạnh mẽ trên nhiều lĩnh vực. Hầu hết các tổ chức đã và đang triển khai các hệ thống thông tin quản lý doanh nghiệp cốt lõi như Hệ thống hoạch định nguồn lực doanh nghiệp (ERP), Hệ thống quản lý quan hệ khách hàng (CRM), Hệ thống quản lý tài liệu (DMS) và Hệ thống quản lý nhân sự (HRM). Bên cạnh đó, sự phát triển của các mô hình ngôn ngữ lớn (Large Language Models - LLM) như GPT, Claude, Gemini cùng với các kỹ thuật truy xuất tăng cường (Retrieval-Augmented Generation - RAG) đã mở ra nhiều cơ hội ứng dụng trí tuệ nhân tạo vào hoạt động sản xuất, kinh doanh.

Tuy nhiên, thực tế cho thấy phần lớn các doanh nghiệp đang gặp khó khăn trong việc tích hợp AI vào hệ thống hiện có. Mỗi hệ thống thường được tích hợp AI một cách riêng lẻ, dẫn đến tình trạng phân mảnh giải pháp, khó bảo trì, chi phí triển khai và vận hành cao. Ngoài ra, nhiều giải phịp AI hiện nay phụ thuộc vào các dịch vụ cloud nước ngoài, gây ra lo ngại về bảo mật dữ liệu và chi phí sử dụng. Việc xây dựng một nền tảng AI doanh nghiệp (Enterprise AI Platform) có khả năng tích hợp đa hệ thống, hỗ trợ nhiều LLM provider, có thể chạy offline và đáp ứng yêu cầu bảo mật cấp doanh nghiệp là một nhu cầu cấp thiết.

Xuất phát từ thực tế đó, đề tài **"Nghiên cứu và xây dựng nền tảng AI doanh nghiệp hướng tích hợp đa hệ thống (Enterprise AI Platform for Cross-System Integration)"** được lựa chọn với mục tiêu xây dựng một nền tảng có khả năng cung cấp các khả năng AI sẵn sàng sử dụng cho nhiều hệ thống khác nhau, đồng thời hỗ trợ cơ chế plugin cho phép mở rộng linh hoạt và đáp ứng các yêu cầu về bảo mật, khả năng mở rộng cũng như tính sẵn sàng cấp doanh nghiệp. Đề tài không chỉ góp phần ứng dụng các công nghệ AI hiện đại vào thực tiễn doanh nghiệp mà còn tạo nền tảng cho việc xây dựng các hệ thống tích hợp AI có tính tái sử dụng cao trong tương lai.

## 2. Tổng quan về vấn đề nghiên cứu

### 2.1. Giới thiệu lĩnh vực nghiên cứu

Nền tảng AI doanh nghiệp (Enterprise AI Platform) là một hệ thống phần mềm được thiết kế để cung cấp các khả năng trí tuệ nhân tạo cho nhiều ứng dụng và hệ thống khác nhau trong một tổ chức. Khác với các giải pháp AI đơn lẻ được xây dựng cho từng bài toán cụ thể, nền tảng AI doanh nghiệp hướng tới tính tái sử dụng, khả năng mở rộng và tích hợp đa hệ thống.

Trong những năm gần đây, lĩnh vực này đã có những bước phát triển quan trọng cùng với sự ra đời của các mô hình ngôn ngữ lớn, các framework Agent, các giao thức tích hợp AI-native như Model Context Protocol (MCP) và các kiến trúc plugin-based. Các nền tảng này cho phép doanh nghiệp triển khai nhanh chóng các khả năng AI như tìm kiếm ngữ nghĩa, hỏi đáp tự động, tạo nội dung, tự động hóa quy trình và phân tích dữ liệu thông minh.

Cùng với đó, xu hướng xây dựng các nền tảng có khả năng tích hợp với nhiều hệ thống doanh nghiệp sẵn có (ERP, CRM, DMS, HRM) thông qua các giao thức chuẩn hóa như REST, gRPC, MCP và event bus đang ngày càng được quan tâm. Điều này mở ra khả năng phát triển các nền tảng AI thông minh có thể khai thác dữ liệu từ nhiều nguồn, cung cấp khả năng tương tác đa kênh và đáp ứng yêu cầu bảo mật cấp doanh nghiệp.

### 2.2. Các hướng tiếp cận chính

Các hướng tiếp cận chính trong lĩnh vực nền tảng AI doanh nghiệp có thể được phân loại thành ba nhóm:

**Phương pháp tích hợp AI trực tiếp vào từng ứng dụng**: Mỗi hệ thống ERP, CRM, DMS tự tích hợp các API AI riêng biệt như OpenAI, Claude, Gemini. Cách tiếp cận này đơn giản nhưng dẫn đến tình trạng phân mảnh, khó quản lý tập trung và chi phí cao khi mở rộng.

**Phương pháp sử dụng AI Gateway trung gian**: Xây dựng một lớp gateway trung gian để quản lý việc gọi các API AI, hỗ trợ failover, caching và rate limiting. Hướng tiếp cận này cải thiện khả năng quản lý nhưng vẫn chưa giải quyết được bài toán tích hợp sâu với các hệ thống doanh nghiệp.

**Phương pháp xây dựng Enterprise AI Platform toàn diện**: Xây dựng một nền tảng hoàn chỉnh với kiến trúc module hóa, hỗ trợ nhiều LLM provider, tích hợp AI Engine (RAG, Search), Agent Orchestration, Plugin System và các connector cho hệ thống doanh nghiệp. Đây là hướng tiếp cận tổng thể nhất, đáp ứng được yêu cầu về khả năng mở rộng, bảo mật và tính tái sử dụng.

### 2.3. Khoảng trống nghiên cứu và vấn đề đặt ra

Mặc dù đã có nhiều nghiên cứu và sản phẩm về LLM, RAG và AI Agent, các giải pháp hiện nay vẫn tồn tại nhiều khoảng trống quan trọng:

- Phần lớn các giải phịp AI hiện có được thiết kế cho từng bài toán cụ thể, thiếu tính tái sử dụng và khả năng tích hợp với nhiều hệ thống doanh nghiệp khác nhau.
- Các framework AI Platform hiện nay thường phụ thuộc vào cloud provider nước ngoài, gây ra lo ngại về bảo mật dữ liệu và chi phí vận hành.
- Khả năng mở rộng thông qua plugin và connector cho các hệ thống ERP, CRM, DMS, HRM còn hạn chế.
- Các giải phịp Agent hiện tại chủ yếu tập trung vào giao tiếp chat, chưa hỗ trợ đầy đủ khả năng thực thi tác vụ phức tạp trên nhiều hệ thống.
- Thiếu các nghiên cứu toàn diện về kiến trúc Enterprise AI Platform với khả năng vừa hỗ trợ cloud, vừa hỗ trợ on-premise cho doanh nghiệp Việt Nam.

Xuất phát từ những phân tích trên, báo cáo tập trung vào việc nghiên cứu và xây dựng một nền tảng AI doanh nghiệp có khả năng tích hợp đa hệ thống, hỗ trợ nhiều LLM provider (trong đó có Ollama local), xây dựng kiến trúc module hóa với khả năng mở rộng qua plugin và connector, đáp ứng yêu cầu bảo mật và khả năng mở rộng cấp doanh nghiệp.

## 3. Mục đích nghiên cứu

### 3.1. Mục tiêu tổng quát

Mục tiêu tổng quát của đề tài là nghiên cứu và xây dựng nền tảng AI doanh nghiệp (Enterprise AI Platform) hướng tới tích hợp đa hệ thống, cung cấp khả năng AI sẵn sàng sử dụng cho nhiều hệ thống doanh nghiệp khác nhau (ERP, CRM, DMS, HRM) thông qua kiến trúc module hóa, plugin-based và hỗ trợ nhiều AI provider. Nền tảng hướng tới các nguyên tắc: **Plug & Play** (cắm là chạy), **Provider Agnostic** (không phụ thuộc provider), **Domain Aware** (nhận thức miền), **Event Driven** (hướng sự kiện), **Agent Native** (bản chất Agent), **Offline First** (ưu tiên chạy offline), **Enterprise Grade** (cấp doanh nghiệp).

### 3.2. Mục tiêu cụ thể

- Nghiên cứu cơ sở lý thuyết về LLM, RAG, AI Agent, MCP và kiến trúc Enterprise AI Platform, làm nền tảng khoa học cho bài toán nghiên cứu.
- Khảo sát, phân tích các framework AI Platform hiện có và xác định khoảng trống nghiên cứu cần giải quyết.
- Đề xuất kiến trúc tổng thể nền tảng AI doanh nghiệp theo mô hình bảy tầng (Presentation, AI Gateway, Agent Orchestration, AI Engine Core, Provider Abstraction, Integration Layer, Platform Core), đảm bảo tính module hóa, khả năng mở rộng và bảo mật.
- Xây dựng và triển khai thử nghiệm hệ thống với các module cốt lõi: AI Engine (RAG, Search), Agent Framework, Provider Abstraction, Plugin SDK, MCP Server, Integration Hub.
- Đánh giá hiệu quả của hệ thống thông qua các chỉ số RAGAS (Context Precision, Context Recall, Faithfulness, Answer Relevancy), thời gian phản hồi và khả năng mở rộng qua plugin.

## 4. Đối tượng và phạm vi nghiên cứu

### 4.1. Đối tượng nghiên cứu

Đối tượng nghiên cứu của báo cáo bao gồm:
- Các mô hình ngôn ngữ lớn (LLM) và kỹ thuật truy xuất tăng cường (RAG) trong bài toán tích hợp đa hệ thống.
- Kiến trúc AI Platform, các framework Agent, giao thức MCP và cơ chế plugin.
- Các kỹ thuật tích hợp đa hệ thống doanh nghiệp (ERP, CRM, DMS, HRM) thông qua REST API, gRPC, MCP và event-driven.
- Cơ chế quản lý multi-tenant, bảo mật và khả năng mở rộng trong nền tảng AI doanh nghiệp.

### 4.2. Phạm vi nghiên cứu

Báo cáo tập trung nghiên cứu cơ chế xây dựng nền tảng AI doanh nghiệp với khả năng tích hợp đa hệ thống, cung cấp các khả năng AI cốt lõi (RAG, Search, Agent) cho nhiều ứng dụng khác nhau thông qua API chuẩn hóa. Nội dung nghiên cứu hướng đến:
- Kiến trúc module hóa, plugin-based hỗ trợ mở rộng linh hoạt.
- Khả năng tích hợp nhiều LLM provider (Ollama local, OpenAI cloud) với cơ chế failover.
- Hỗ trợ triển khai on-premise, đảm bảo bảo mật dữ liệu cấp doanh nghiệp.
- Đánh giá hiệu quả thông qua các kịch bản thực nghiệm với bộ dữ liệu cụ thể.

Các nội dung nghiên cứu, xây dựng và đánh giá được thực hiện trong thời gian triển khai báo cáo thực tập. Dữ liệu sử dụng cho thực nghiệm được thu thập và xử lý trong giai đoạn nghiên cứu, không xem xét yếu tố biến động theo thời gian thực.

## 5. Phương pháp nghiên cứu

Báo cáo được thực hiện theo phương pháp nghiên cứu kết hợp giữa nghiên cứu lý thuyết và thực nghiệm nhằm xây dựng và đánh giá giải pháp nền tảng AI doanh nghiệp tích hợp đa hệ thống.

**Nghiên cứu cơ sở lý thuyết**:
- Thu thập, tổng hợp và phân tích các tài liệu khoa học liên quan đến LLM, RAG, AI Agent, MCP, kiến trúc AI Platform và các framework tích hợp.
- Khảo sát các công trình nghiên cứu trong và ngoài nước nhằm xác định các hướng tiếp cận hiện có, những hạn chế còn tồn tại và khoảng trống nghiên cứu cần giải quyết.

**Thu thập và xây dựng dữ liệu**:
- Thu thập dữ liệu tài liệu từ các nguồn công khai và nội bộ doanh nghiệp mẫu.
- Chuẩn hóa và làm sạch dữ liệu nhằm loại bỏ thông tin trùng lặp, thiếu nhất quán.
- Tổ chức dữ liệu dưới dạng cơ sở tri thức phục vụ quá trình lưu trữ, truy xuất và khai thác thông tin.

**Xây dựng hệ thống truy xuất tri thức**:
- Chuyển đổi dữ liệu văn bản sang biểu diễn vector thông qua các mô hình embedding phù hợp.
- Lưu trữ dữ liệu trên cơ sở dữ liệu vector (pgvector) để hỗ trợ tìm kiếm ngữ nghĩa.
- Thiết kế cơ chế truy xuất thông tin dựa trên mức độ liên quan giữa câu truy vấn và dữ liệu trong cơ sở tri thức, kết hợp Hybrid Search và Reranking.

**Xây dựng mô hình thực nghiệm**:
- Tích hợp mô hình ngôn ngữ lớn với hệ thống truy xuất tri thức theo kiến trúc RAG nâng cao.
- Xây dựng cơ chế tiếp nhận và phân tích yêu cầu của người dùng dưới dạng ngôn ngữ tự nhiên.
- Thiết kế và triển khai các module Agent, Plugin SDK, MCP Server, Integration Hub.
- Triển khai hệ thống trên môi trường thử nghiệm phục vụ đánh giá.

**Đánh giá hệ thống**:
- Thực hiện các kịch bản thử nghiệm với nhiều nhóm yêu cầu khác nhau: tìm kiếm, hỏi đáp, thực thi tác vụ đa bước, tích hợp plugin.
- Đánh giá chất lượng truy xuất thông tin thông qua các chỉ số RAGAS gồm: Context Precision, Context Recall, Faithfulness và Answer Relevancy.
- Đánh giá hiệu năng thông qua thời gian phản hồi và khả năng mở rộng của hệ thống.
- So sánh kết quả giữa các cấu hình LLM provider (Ollama local, OpenAI cloud) nhằm xác định hiệu quả của giải pháp đề xuất.

**Công cụ và môi trường nghiên cứu**:
- Ngôn ngữ lập trình chính: C# (.NET 8), TypeScript (Next.js), Python.
- Mô hình ngôn ngữ lớn: Ollama (llama3.2:3b, nomic-embed-text) và OpenAI (GPT-4o-mini, text-embedding-3-small).
- Cơ sở dữ liệu: PostgreSQL 16 với extension pgvector.
- Bộ nhớ đệm: Redis 7.
- Lưu trữ đối tượng: MinIO (S3-compatible).
- Container hóa: Docker và Docker Compose.
- Môi trường phát triển: Visual Studio Code, .NET SDK 8.0, Node.js 20.

---

# Chương 1: CƠ SỞ LÝ LUẬN

## 1.1. Tổng quan về trí tuệ nhân tạo và nền tảng AI

Trong bối cảnh doanh nghiệp đang đẩy mạnh chuyển đổi số, trí tuệ nhân tạo (Artificial Intelligence - AI) đã trở thành công nghệ cốt lõi giúp tự động hóa quy trình, nâng cao hiệu quả vận hành và tạo lợi thế cạnh tranh. Tuy nhiên, việc triển khai AI trong các tổ chức lớn thường gặp nhiều thách thức: dữ liệu phân tán trên nhiều hệ thống (ERP, CRM, DMS, HRM), yêu cầu bảo mật nghiêm ngặt, khả năng mở rộng và tích hợp phức tạp.

**Nền tảng AI doanh nghiệp (Enterprise AI Platform)** được phát triển nhằm giải quyết các thách thức trên bằng cách cung cấp một lớp trung gian thống nhất, cho phép nhiều hệ thống khác nhau cùng khai thác các khả năng AI sẵn có. Một nền tảng AI doanh nghiệp điển hình bao gồm các thành phần chính: lớp trình bày (Presentation Layer), lớp AI Gateway, lớp điều phối Agent, lớp AI Engine Core, lớp trừu tượng hóa nhà cung cấp (Provider Abstraction), lớp tích hợp (Integration Layer) và lớp nền tảng lõi (Platform Core) [1].

Các khả năng AI cốt lõi mà nền tảng thường cung cấp bao gồm: tìm kiếm ngữ nghĩa (Semantic Search), hỏi đáp thông minh (RAG Chatbot), AI Agent với khả năng thực thi tác vụ, truy vấn ngôn ngữ tự nhiên (Text-to-SQL), xử lý giọng nói (Voice AI) và các khả năng mở rộng thông qua plugin. Các khả năng này có thể được tích hợp vào nhiều hệ thống doanh nghiệp thông qua REST API, gRPC hoặc giao thức Model Context Protocol (MCP) [1], [2].

Trong bối cảnh Việt Nam, nhu cầu xây dựng nền tảng AI doanh nghiệp ngày càng tăng cao do yêu cầu bảo mật dữ liệu nội địa, chi phí vận hành và khả năng tích hợp với hạ tầng IT sẵn có. Các giải pháp cloud nước ngoài thường không đáp ứng được yêu cầu về tuân thủ quy định bảo mật dữ liệu và chi phí cho doanh nghiệp vừa và nhỏ. Do đó, việc xây dựng nền tảng AI có khả năng triển khai on-premise, hỗ trợ nhiều LLM provider trong đó có Ollama local là hướng tiếp cận phù hợp.

## 1.2. Mô hình ngôn ngữ lớn (LLM)

### 1.2.1. Khái niệm

LLM là nhóm mô hình học sâu được thiết kế để hiểu và sinh ngôn ngữ tự nhiên thông qua quá trình huấn luyện trên khối lượng dữ liệu văn bản lớn [2]. Kiến trúc nền tảng của hầu hết LLM hiện đại dựa trên Transformer [3], cho phép mô hình đánh giá mức độ liên quan giữa các token trong toàn bộ chuỗi đầu vào thông qua cơ chế attention, thay vì xử lý tuần tự như RNN hay LSTM. Điều này giúp cải thiện khả năng nắm bắt ngữ cảnh dài và tăng hiệu quả tính toán.

Quá trình phát triển LLM thường gồm hai giai đoạn chính:
- **Pretraining**: mô hình học các đặc trưng ngôn ngữ tổng quát từ khối lượng lớn dữ liệu, thường là dữ liệu không gán nhãn.
- **Fine-tuning**: mô hình được điều chỉnh trên các bộ dữ liệu hoặc nhiệm vụ cụ thể như hỏi đáp, tóm tắt, phân loại văn bản hoặc sinh nội dung.

### 1.2.2. Các mô hình tiêu biểu

**BERT (Bidirectional Encoder Representations from Transformers)** [4]: Là mô hình dựa trên phần Encoder của Transformer, có khả năng tạo biểu diễn ngữ nghĩa hai chiều bằng cách khai thác đồng thời ngữ cảnh trước và sau mỗi token. Mô hình được huấn luyện thông qua hai cơ chế chính: Masked Language Modeling (MLM) - che giấu một số token trong câu và yêu cầu mô hình dự đoán lại các token bị che, và Next Sentence Prediction (NSP) - dự đoán mối quan hệ giữa hai câu liên tiếp. BERT đạt hiệu quả cao trong các nhiệm vụ hiểu văn bản như phân loại, trích xuất thông tin và hỏi đáp, nhưng không được tối ưu cho các bài toán sinh văn bản dài.

**GPT (Generative Pre-trained Transformer)** [3], [5]: Là mô hình dựa trên phần Decoder của Transformer, sinh văn bản theo cơ chế tự hồi quy bằng cách dự đoán token tiếp theo dựa trên các token đã xuất hiện trước đó. Các phiên bản GPT quy mô lớn cho thấy khả năng thực hiện nhiều nhiệm vụ xử lý ngôn ngữ tự nhiên thông qua cách thiết kế đầu vào phù hợp, đặc biệt trong bối cảnh few-shot learning. GPT được huấn luyện trên khối lượng lớn dữ liệu văn bản, có khả năng sinh văn bản mạch lạc theo ngữ cảnh và có thể thực hiện nhiều nhiệm vụ như hỏi đáp, tóm tắt, viết lại nội dung hoặc sinh văn bản thông qua prompt.

**Gemini** [6]: Là mô hình ngôn ngữ lớn đa phương thức do Google DeepMind phát triển, có khả năng xử lý nhiều dạng dữ liệu như văn bản, hình ảnh, âm thanh và thông tin phi cấu trúc trong cùng một kiến trúc. So với các mô hình chỉ xử lý văn bản, Gemini phù hợp hơn với các bài toán yêu cầu phân tích và suy luận trên nhiều loại dữ liệu khác nhau.

**Llama và Mistral**: Là các dòng mô hình mã nguồn mở có thể chạy local, đặc biệt phù hợp với các kịch bản yêu cầu bảo mật dữ liệu và triển khai on-premise. Ollama là công cụ phổ biến cho phép chạy các mô hình này trên máy local với hiệu năng cao [7].

### 1.2.3. Hạn chế

Mặc dù LLM mang lại nhiều bước tiến quan trọng trong xử lý ngôn ngữ tự nhiên, các mô hình này vẫn tồn tại một số hạn chế khi áp dụng vào hệ thống thực tế [2]:
- **Hiện tượng ảo giác (hallucination)**: mô hình có thể tạo ra thông tin nghe có vẻ hợp lý nhưng không chính xác hoặc không có căn cứ.
- **Phụ thuộc vào dữ liệu huấn luyện**: chất lượng, phạm vi và thời điểm thu thập dữ liệu ảnh hưởng trực tiếp đến độ chính xác của phản hồi.
- **Chi phí tính toán cao**: việc huấn luyện và triển khai LLM đòi hỏi tài nguyên phần cứng lớn, đặc biệt với các mô hình có quy mô tham số cao.
- **Hạn chế khi xử lý ngữ cảnh dài**: mô hình có thể gặp khó khăn khi đầu vào chứa nhiều tài liệu hoặc thông tin cần liên kết qua nhiều đoạn văn bản.
- **Không tự cập nhật tri thức**: LLM không tự động nắm bắt các thông tin mới phát sinh sau thời điểm huấn luyện nếu không có cơ chế bổ sung dữ liệu bên ngoài.

Do đó, trong các hệ thống ứng dụng thực tế, LLM thường được kết hợp với Retrieval-Augmented Generation nhằm bổ sung khả năng truy xuất thông tin từ nguồn dữ liệu bên ngoài, giảm phụ thuộc vào tri thức nội tại của mô hình và cải thiện độ tin cậy của phản hồi.

## 1.3. Retrieval-Augmented Generation (RAG)

### 1.3.1. Khái niệm

Retrieval-Augmented Generation (RAG) là phương pháp kết hợp mô hình ngôn ngữ lớn với cơ chế truy xuất thông tin từ nguồn dữ liệu bên ngoài [8]. Thay vì phụ thuộc hoàn toàn vào tri thức được học trong quá trình huấn luyện, mô hình có thể tham chiếu thêm các tài liệu liên quan tại thời điểm xử lý truy vấn để hỗ trợ tạo câu trả lời.

Về bản chất, RAG tách biệt tương đối giữa khả năng sinh ngôn ngữ của LLM và nguồn tri thức được sử dụng. Tri thức có thể được lưu trữ, cập nhật và truy xuất từ bên ngoài mô hình, qua đó giúp hệ thống linh hoạt hơn trong các bài toán yêu cầu thông tin mới, thông tin chuyên ngành hoặc dữ liệu thay đổi theo thời gian.

### 1.3.2. Kiến trúc và quy trình hoạt động

Kiến trúc của RAG được xây dựng dựa trên sự kết hợp giữa cơ chế truy xuất thông tin và mô hình sinh ngôn ngữ. Trong đó, tri thức không chỉ nằm trong tham số của mô hình mà còn được lưu trữ ở kho dữ liệu bên ngoài và được truy xuất khi có truy vấn từ người dùng.

Về tổng thể, một hệ thống RAG thường bao gồm ba thành phần chính: kho tri thức (Knowledge Base), bộ truy xuất thông tin (Retriever) và mô hình sinh ngôn ngữ (Generator). Quy trình hoạt động của hệ thống RAG có thể được mô tả qua bốn bước chính:

- **Tiếp nhận truy vấn**: Người dùng gửi câu hỏi hoặc yêu cầu đến hệ thống. Truy vấn đầu vào được chuẩn hóa và chuyển sang dạng phù hợp để phục vụ quá trình tìm kiếm.
- **Truy xuất tài liệu liên quan**: Retriever thực hiện tìm kiếm trong kho tri thức dựa trên nội dung truy vấn. Hệ thống lựa chọn các tài liệu hoặc đoạn văn bản có mức độ liên quan cao nhất. Kết quả truy xuất thường là một tập các đoạn thông tin ngắn phục vụ cho bước sinh câu trả lời.
- **Xây dựng ngữ cảnh**: Các tài liệu được truy xuất được ghép với truy vấn ban đầu. Tập thông tin này tạo thành ngữ cảnh mở rộng (augmented context) cho mô hình ngôn ngữ. Ngữ cảnh mở rộng giúp mô hình có thêm dữ liệu tham chiếu khi sinh phản hồi.
- **Sinh câu trả lời**: Generator hoặc LLM sử dụng truy vấn cùng với các tài liệu liên quan để tạo ra câu trả lời. Câu trả lời được xây dựng dựa trên cả kiến thức nội tại của mô hình và thông tin được truy xuất từ kho tri thức.

### 1.3.3. Vai trò của RAG trong hệ thống ứng dụng LLM

Trong các hệ thống ứng dụng thực tế, RAG đóng vai trò quan trọng trong việc mở rộng khả năng của LLM vượt ra ngoài giới hạn tri thức nội tại của mô hình. Cụ thể, RAG mang lại một số lợi ích chính:
- Cho phép bổ sung và cập nhật tri thức thông qua kho dữ liệu bên ngoài mà không cần huấn luyện lại toàn bộ mô hình.
- Tăng khả năng kiểm soát và truy vết nguồn gốc thông tin được sử dụng trong quá trình sinh phản hồi.
- Hỗ trợ tốt hơn cho các bài toán theo miền chuyên biệt nhờ khả năng tích hợp dữ liệu riêng của từng hệ thống.

Trong bối cảnh nền tảng AI doanh nghiệp, RAG đặc biệt quan trọng vì thông tin doanh nghiệp thường thay đổi liên tục, có tính chuyên ngành cao và yêu cầu bảo mật. Khi kho tri thức được cập nhật phù hợp, cơ chế truy xuất giúp hệ thống đối chiếu yêu cầu của người dùng với các nguồn dữ liệu nội bộ trước khi sinh phản hồi, đảm bảo kết quả có căn cứ và phù hợp với ngữ cảnh doanh nghiệp.

### 1.3.4. Ưu điểm và hạn chế

Trong quá trình triển khai thực tế, RAG thể hiện cả ưu điểm lẫn hạn chế khi xét trên khía cạnh chất lượng phản hồi, khả năng cập nhật tri thức và mức độ phức tạp của hệ thống.

**Bảng 1.1. Ưu điểm và hạn chế của RAG**

| Ưu điểm | Hạn chế |
|---------|---------|
| Mở rộng khả năng sử dụng tri thức ngoài mô hình, giảm phụ thuộc vào tham số đã huấn luyện | Phụ thuộc đáng kể vào chất lượng, độ chính xác và mức độ cập nhật của nguồn dữ liệu truy xuất |
| Cho phép cập nhật thông tin thông qua kho dữ liệu bên ngoài mà không cần huấn luyện lại mô hình | Hiệu quả đầu ra bị ảnh hưởng nếu bộ truy xuất chọn sai, thiếu hoặc nhiễu thông tin liên quan |
| Hỗ trợ tốt các bài toán theo miền nhờ khả năng tích hợp dữ liệu chuyên biệt | Làm tăng độ phức tạp trong thiết kế, triển khai và vận hành hệ thống so với LLM thuần túy |
| Giúp phản hồi bám sát hơn vào yêu cầu người dùng và tài liệu được truy xuất | Tăng thời gian xử lý do phát sinh thêm bước truy xuất trước khi sinh kết quả |
| Tăng khả năng kiểm soát và truy vết nguồn thông tin đầu ra | Cần thiết kế tốt cơ chế thu thập, tổ chức, chia đoạn và biểu diễn dữ liệu để đạt hiệu quả cao |

## 1.4. AI Agent và Multi-Agent Orchestration

### 1.4.1. Khái niệm AI Agent

AI Agent là một hệ thống phần mềm có khả năng tự chủ thực hiện các tác vụ phức tạp bằng cách kết hợp LLM với khả năng lập kế hoạch, gọi công cụ (tool calling) và tương tác với môi trường. Khác với chatbot truyền thống chỉ trả lời câu hỏi, AI Agent có thể thực hiện các hành động cụ thể như truy vấn cơ sở dữ liệu, gọi API, gửi email hoặc cập nhật hệ thống [9].

Các thành phần cốt lõi của một AI Agent bao gồm:
- **LLM Brain**: mô hình ngôn ngữ lớn đóng vai trò bộ xử lý trung tâm, thực hiện suy luận và ra quyết định.
- **Memory**: bộ nhớ ngắn hạn và dài hạn lưu trữ ngữ cảnh hội thoại, lịch sử tương tác và tri thức thu được.
- **Planning**: khả năng phân tích yêu cầu phức tạp thành chuỗi các bước nhỏ hơn có thể thực thi được.
- **Tools**: tập hợp các hàm, API mà Agent có thể gọi để thực hiện hành động cụ thể.

### 1.4.2. Kiến trúc Agent

Kiến trúc Agent điển hình bao gồm các thành phần chính:
- **Intent Router**: phân loại ý định của người dùng, xác định loại tác vụ cần thực hiện.
- **Task Planner**: lập kế hoạch thực thi tác vụ dưới dạng chuỗi các bước (Plan).
- **Agent Executor**: thực thi từng bước trong plan, gọi tool tương ứng và thu thập kết quả.
- **Replanner**: trong trường hợp một bước thất bại, lập lại kế hoạch để tìm cách tiếp cận khác.
- **Human-in-the-Loop (HITL)**: cơ chế cho phép con người phê duyệt các hành động nhạy cảm trước khi thực thi.
- **Safety Guardrails**: bộ quy tắc bảo vệ nhằm ngăn chặn các hành động không mong muốn.

Multi-Agent Orchestration là mô hình trong đó nhiều Agent phối hợp với nhau để giải quyết các bài toán phức tạp. Mỗi Agent có thể đảm nhận một vai trò chuyên biệt (ví dụ: Agent phân tích, Agent tổng hợp, Agent kiểm tra) và trao đổi thông tin qua message bus hoặc shared memory [1].

### 1.4.3. Model Context Protocol (MCP)

Model Context Protocol (MCP) là giao thức chuẩn hóa được phát triển bởi Anthropic nhằm cho phép các mô hình AI tương tác với các hệ thống và công cụ bên ngoài một cách nhất quán [10]. MCP hoạt động theo mô hình client-server, trong đó:
- **MCP Server**: cung cấp các tài nguyên (resources), công cụ (tools) và prompt templates cho AI client.
- **MCP Client**: giao tiếp với MCP server để truy xuất thông tin hoặc thực thi công cụ.

Lợi ích chính của MCP trong kiến trúc Enterprise AI Platform bao gồm:
- Chuẩn hóa giao tiếp giữa AI và các hệ thống bên ngoài, giảm chi phí tích hợp.
- Cho phép các hệ thống ERP, CRM, DMS, HRM expose chức năng của mình dưới dạng MCP server mà không cần thay đổi kiến trúc.
- Hỗ trợ khả năng tương tác giữa các platform AI khác nhau thông qua giao thức chung.

## 1.5. Hybrid Search và Reranking

Trong hệ thống RAG, chất lượng ngữ cảnh truy xuất ảnh hưởng trực tiếp đến kết quả sinh câu trả lời. Khi tài liệu đầu vào thiếu liên quan, không đầy đủ hoặc chứa nhiễu, mô hình ngôn ngữ có thể tạo ra phản hồi sai lệch. Vì vậy, bên cạnh việc xây dựng kho tri thức, cải thiện giai đoạn truy xuất là một yêu cầu quan trọng. Hai kỹ thuật thường được áp dụng để nâng cao chất lượng tài liệu đầu vào là Hybrid Search và Reranking.

### 1.5.1. Hybrid Search

Hybrid Search kết hợp đồng thời nhiều phương pháp truy xuất nhằm tận dụng ưu điểm bổ trợ của từng phương pháp [11]. Trong các hệ thống RAG, kỹ thuật này thường kết hợp hai hướng truy xuất chính:
- **Dense Retrieval**: sử dụng mô hình embedding để ánh xạ truy vấn và tài liệu vào cùng một không gian vector [12]. Mức độ liên quan được đo bằng độ tương đồng giữa các vector, giúp hệ thống nắm bắt quan hệ ngữ nghĩa ngay cả khi truy vấn và tài liệu không chia sẻ nhiều từ khóa giống nhau.
- **Sparse Retrieval**: điển hình là BM25, đánh giá mức độ liên quan dựa trên tần suất và mức độ phân biệt của từ khóa trong tài liệu. Phương pháp này phù hợp với các truy vấn chứa thực thể, tên địa danh hoặc thuật ngữ cụ thể, nhưng có thể hạn chế khi người dùng diễn đạt lại ý bằng các từ ngữ khác.

Việc kết hợp cả thông tin ngữ nghĩa và từ khóa giúp hệ thống mở rộng khả năng tìm kiếm tài liệu liên quan, đồng thời giảm nguy cơ bỏ sót nội dung quan trọng trong kho tri thức.

### 1.5.2. Reranking

Reranking là bước đánh giá lại và sắp xếp lại thứ tự các tài liệu sau giai đoạn truy xuất ban đầu, nhằm ưu tiên những tài liệu có mức độ liên quan cao hơn với truy vấn của người dùng [13]. Khác với truy xuất ban đầu thường ưu tiên tốc độ và phạm vi bao phủ, Reranking sử dụng mô hình học sâu (thường là Cross-Encoder) để xem xét đồng thời nội dung truy vấn và văn bản, từ đó xác định mức độ phù hợp chi tiết hơn.

Trong kiến trúc RAG, Reranking đóng vai trò tinh chỉnh tập tài liệu trước khi xây dựng ngữ cảnh đầu vào cho LLM. Bước này giúp giảm nhiễu thông tin, ưu tiên các đoạn nội dung quan trọng và cải thiện khả năng sinh câu trả lời chính xác.

## 1.6. Kiến trúc Enterprise AI Platform

### 1.6.1. Đặc trưng của nền tảng AI doanh nghiệp

Một nền tảng AI doanh nghiệp (Enterprise AI Platform) khác với các hệ thống AI đơn lẻ ở nhiều đặc trưng quan trọng [1]:
- **Tích hợp đa hệ thống**: cho phép nhiều hệ thống ERP, CRM, DMS, HRM cùng sử dụng các khả năng AI sẵn có thông qua API chuẩn hóa.
- **Tính module hóa**: kiến trúc được chia thành các module rõ ràng với bounded context, dễ bảo trì và mở rộng.
- **Plugin-based**: cho phép mở rộng chức năng thông qua plugin mà không cần thay đổi lõi hệ thống.
- **Multi-tenant**: hỗ trợ nhiều tổ chức/doanh nghiệp sử dụng chung nền tảng với sự cô lập dữ liệu.
- **Provider Agnostic**: không phụ thuộc vào một LLM provider duy nhất, có thể chuyển đổi giữa các provider.
- **Offline First**: ưu tiên khả năng chạy trên hạ tầng on-premise, không phụ thuộc cloud.
- **Bảo mật cấp doanh nghiệp**: hỗ trợ xác thực, phân quyền, mã hóa, audit log.

Các nguyên tắc thiết kế chính của Enterprise AI Platform bao gồm: **Plug & Play** (cắm là chạy), **Provider Agnostic** (không phụ thuộc provider), **Domain Aware** (nhận thức miền), **Event Driven** (hướng sự kiện), **Agent Native** (bản chất Agent), **Offline First** (ưu tiên chạy offline) và **Enterprise Grade** (cấp doanh nghiệp) [1].

### 1.6.2. Kiến trúc Modular Monolith

Kiến trúc Modular Monolith là sự kết hợp giữa Monolith đơn giản trong triển khai và Microservice trong thiết kế module. Trong kiến trúc này, hệ thống được chia thành các module rõ ràng với bounded context, mỗi module có trách nhiệm riêng biệt nhưng vẫn chia sẻ cùng một process và database khi triển khai [14].

So sánh với các kiến trúc khác:
- **Pure Microservice**: scale độc lập, team autonomy nhưng quá phức tạp cho giai đoạn đầu, overhead vận hành cao.
- **Pure Monolith**: đơn giản, deploy dễ nhưng không extensible, không reusable.
- **Modular Monolith**: module boundaries rõ, deploy đơn giản, dễ tách microservice sau, phù hợp cho giai đoạn nền tảng.
- **Plugin-based**: extensible, hỗ trợ tích hợp bên thứ ba nhưng quản lý plugin phức tạp.
- **Agent-based**: AI-native, autonomous actions nhưng cần guardrails mạnh.

Hướng tiếp cận **Hybrid (Modular Monolith + Plugin + Agent)** kết hợp ưu điểm của cả ba, phù hợp cho Enterprise AI Platform ở giai đoạn hiện tại.

### 1.6.3. Kiến trúc Plugin-based

Kiến trúc Plugin-based cho phép mở rộng chức năng của platform thông qua việc bổ sung các plugin mà không cần thay đổi lõi hệ thống. Một plugin điển hình bao gồm:
- **Plugin Manifest**: mô tả metadata, dependency, version, quyền hạn của plugin.
- **Plugin Assembly**: chứa implementation của các extension point mà plugin đăng ký.
- **Extension Points**: các điểm trong lõi hệ thống mà plugin có thể đăng ký để mở rộng chức năng.

Các extension point phổ biến trong Enterprise AI Platform bao gồm: Tool Provider (bổ sung tool cho Agent), Document Parser (hỗ trợ định dạng tài liệu mới), Embedding Provider (tích hợp mô hình embedding mới), Connector (kết nối hệ thống bên ngoài), Workflow Step (mở rộng workflow engine) [1], [15].

Trong nền tảng AI doanh nghiệp, kiến trúc plugin-based mang lại nhiều lợi ích: cho phép tích hợp nhanh chóng các hệ thống ERP, CRM, DMS, HRM hiện có, hỗ trợ tùy biến theo yêu cầu riêng của từng doanh nghiệp, giảm coupling giữa lõi và module mở rộng, dễ dàng nâng cấp và bảo trì.

## 1.7. Các nghiên cứu và hệ thống liên quan

### 1.7.1. Các framework AI Platform hiện có

Hiện nay, một số framework và nền tảng AI doanh nghiệp đã được phát triển và triển khai trong thực tế. Các framework này có thể được phân loại thành ba nhóm chính dựa trên mức độ tích hợp và khả năng mở rộng.

**Bảng 1.2. Một số Enterprise AI Platform hiện có**

| Framework | Đặc điểm | Hạn chế |
|-----------|----------|---------|
| LangChain / LangGraph | Framework mã nguồn mở phổ biến cho xây dựng ứng dụng LLM, hỗ trợ RAG, Agent, tool calling | Thiếu khả năng multi-tenant, plugin SDK còn hạn chế, không hỗ trợ triển khai cấp doanh nghiệp |
| Microsoft Semantic Kernel | Framework .NET cho tích hợp AI, hỗ trợ plugin, planner, memory | Tập trung vào .NET, ít hỗ trợ multi-provider, chưa tích hợp sâu với hệ thống doanh nghiệp |
| LlamaIndex | Framework cho RAG, hỗ trợ indexing, retrieval, evaluation | Chủ yếu tập trung vào RAG, chưa có Agent framework mạnh, thiếu plugin cho enterprise |
| Haystack (deepset) | Framework RAG cấp production, hỗ trợ pipeline, evaluator | Hạn chế về multi-tenant, plugin SDK, tích hợp doanh nghiệp |
| Custom Enterprise Platform | Nền tảng tự xây dựng theo yêu cầu riêng của doanh nghiệp | Chi phí cao, thời gian phát triển dài, phụ thuộc vào vendor |

### 1.7.2. Các hệ thống tích hợp LLM và Agent

Sự xuất hiện của các mô hình ngôn ngữ lớn và framework Agent đã mở ra hướng tiếp cận mới cho bài toán tích hợp AI vào hệ thống doanh nghiệp. Thay vì chỉ trả về câu trả lời dạng text, hệ thống có thể thực thi tác vụ thực tế thông qua Agent.

**Bảng 1.3. Một số nghiên cứu về tích hợp LLM và Agent**

| Nghiên cứu | Hướng tiếp cận | Hạn chế |
|------------|----------------|---------|
| AutoGPT, BabyAGI | Multi-step Agent tự động, tự lập kế hoạch | Chưa có multi-tenant, khó tích hợp doanh nghiệp, thiếu safety guardrails |
| LangGraph | Đồ thị trạng thái cho Agent workflow | Phức tạp cho người mới, chưa hỗ trợ multi-tenant cấp doanh nghiệp |
| Microsoft Copilot Stack | Tích hợp LLM vào sản phẩm Microsoft | Khóa trong hệ sinh thái Microsoft, chi phí license cao |
| MCP-based Integrations | Tích hợp AI với hệ thống qua MCP | Còn mới, ít case study thực tế ở Việt Nam |
| Agentic Workflow cho ERP/CRM | Agent tự động tạo đơn, duyệt, báo cáo | Thiếu khung kiến trúc thống nhất, khó mở rộng |

Các nghiên cứu trên cho thấy tiềm năng của LLM và Agent trong việc nâng cao hiệu quả tích hợp AI với hệ thống doanh nghiệp. Tuy nhiên, việc đảm bảo tính nhất quán, khả năng mở rộng, multi-tenant và bảo mật cấp doanh nghiệp vẫn là những thách thức quan trọng. Bên cạnh đó, các nghiên cứu tổng quan về RAG cho thấy việc kết hợp LLM với cơ chế truy xuất tri thức bên ngoài là một hướng phù hợp để giảm phụ thuộc vào tri thức nội tại của mô hình và cải thiện độ tin cậy của phản hồi. Đây là cơ sở để báo cáo đề xuất hướng kết hợp LLM, RAG, Agent và Plugin cho nền tảng AI doanh nghiệp tích hợp đa hệ thống.

### 1.7.3. Khoảng trống nghiên cứu

Qua khảo sát các framework và công trình nghiên cứu liên quan, có thể nhận thấy một số khoảng trống chính:
- Phần lớn framework hiện nay tập trung vào RAG hoặc Agent riêng lẻ, chưa có nền tảng toàn diện tích hợp đa hệ thống với kiến trúc plugin-based.
- Khả năng hỗ trợ multi-tenant, plugin SDK và tích hợp hệ thống doanh nghiệp (ERP, CRM, DMS, HRM) thông qua giao thức chuẩn (MCP) còn hạn chế.
- Các giải pháp cloud nước ngoài thường không đáp ứng được yêu cầu về bảo mật dữ liệu và triển khai on-premise cho doanh nghiệp Việt Nam.
- Thiếu các nghiên cứu toàn diện về kiến trúc Enterprise AI Platform với đầy đủ 7 tầng: Presentation, AI Gateway, Agent Orchestration, AI Engine Core, Provider Abstraction, Integration Layer và Platform Core.

Những khoảng trống trên là cơ sở để báo cáo đề xuất hướng tiếp cận xây dựng Enterprise AI Platform với kiến trúc module hóa, plugin-based, multi-tenant và hỗ trợ tích hợp đa hệ thống, đáp ứng yêu cầu cấp doanh nghiệp.

## 1.8. Kết luận chương 1

Chương 1 đã trình bày nền tảng lý thuyết của báo cáo, bao gồm:
- Tổng quan về trí tuệ nhân tạo và đặc trưng của nền tảng AI doanh nghiệp.
- Mô hình ngôn ngữ lớn (LLM): kiến trúc Transformer, các mô hình tiêu biểu và hạn chế.
- Kỹ thuật Retrieval-Augmented Generation (RAG): khái niệm, kiến trúc, vai trò và đặc điểm.
- AI Agent và Multi-Agent Orchestration, bao gồm khái niệm, kiến trúc và giao thức MCP.
- Hybrid Search và Reranking: các phương pháp nâng cao chất lượng truy xuất.
- Kiến trúc Enterprise AI Platform: đặc trưng, kiến trúc Modular Monolith và Plugin-based.
- Khảo sát các framework và công trình liên quan, xác định khoảng trống nghiên cứu.

Trên cơ sở đó, việc xây dựng Enterprise AI Platform với kiến trúc module hóa bảy tầng, kết hợp LLM, RAG, Agent, Plugin SDK và MCP là hướng tiếp cận phù hợp để giải quyết bài toán tích hợp AI đa hệ thống cho doanh nghiệp. Chương tiếp theo sẽ tập trung vào phân tích bài toán và đề xuất kiến trúc hệ thống chi tiết.

---

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

---

# Chương 3: THỰC NGHIỆM VÀ ĐÁNH GIÁ KẾT QUẢ

## 3.1. Thiết lập thực nghiệm

### 3.1.1. Mục tiêu và phương pháp

Mục tiêu của phần thực nghiệm là kiểm tra tính khả thi và đánh giá hiệu quả của nền tảng AI doanh nghiệp tích hợp đa hệ thống đã được thiết kế ở Chương 2. Cụ thể, thực nghiệm tập trung làm rõ khả năng truy xuất tri thức của hệ thống RAG, chất lượng phản hồi sinh ra, khả năng thực thi tác vụ của AI Agent, khả năng tích hợp plugin và thời gian xử lý của toàn bộ quy trình.

Báo cáo kết hợp hai hướng đánh giá chính:
- **Đánh giá định lượng**: đo lường chất lượng truy xuất và sinh phản hồi thông qua các chỉ số RAGAS, đồng thời ghi nhận thời gian xử lý ở từng bước trong quy trình.
- **Đánh giá định tính**: xem xét tính hợp lý, khả thi và mức độ phù hợp của kết quả trong các tình huống sử dụng cụ thể, đặc biệt là khả năng tích hợp với hệ thống bên ngoài.

Các nội dung đánh giá chính được tổng hợp trong Bảng 3.1.

**Bảng 3.1. Mục tiêu và phương pháp đánh giá thực nghiệm**

| Nội dung đánh giá | Mục tiêu | Phương pháp thực hiện |
|-------------------|----------|----------------------|
| Hệ thống RAG | Đánh giá chất lượng truy xuất tri thức và sinh phản hồi | Sử dụng các chỉ số RAGAS và so sánh giữa các cấu hình |
| Tích hợp Plugin | Kiểm tra khả năng mở rộng chức năng qua plugin | Load plugin và đo thời gian load, tính đúng đắn |
| Multi-Provider | So sánh chất lượng giữa Ollama local và OpenAI cloud | Chạy cùng tập truy vấn trên 2 providers, đo RAGAS |
| Agent Framework | Đánh giá khả năng thực thi tác vụ phức tạp | Chạy các task mẫu, đo success rate và số bước |
| Hiệu năng | Đo thời gian xử lý và khả năng phản hồi | Ghi nhận thời gian xử lý theo từng bước và tổng thể |

Bên cạnh cấu hình đầy đủ, báo cáo cũng so sánh với các cấu hình rút gọn nhằm làm rõ mức độ đóng góp của từng thành phần trong hệ thống. Các cấu hình RAG, bộ tiêu chí đánh giá, thí nghiệm tích hợp plugin, bộ dữ liệu thực nghiệm và kịch bản đánh giá cụ thể được trình bày trong các phần tiếp theo.

### 3.1.2. Môi trường thực nghiệm và cấu hình

Hệ thống thực nghiệm được triển khai trên môi trường phát triển cục bộ sử dụng Docker Compose. Cấu hình phần cứng, phần mềm và các thành phần chính được trình bày trong Bảng 3.2.

**Bảng 3.2. Môi trường phần cứng và phần mềm**

| Thành phần | Cấu hình |
|------------|----------|
| Hệ điều hành | Windows 11 / macOS 14 (tùy môi trường dev) |
| CPU | Intel Core i7 / Apple M2 (8+ cores) |
| RAM | 16-32 GB |
| GPU | Optional NVIDIA GPU (CUDA) hoặc Apple Silicon |
| Backend | .NET 8 SDK |
| Frontend | Node.js 20 LTS |
| Cơ sở dữ liệu | PostgreSQL 16 + pgvector (Docker) |
| Bộ nhớ đệm | Redis 7 (Docker) |
| Object Storage | MinIO (Docker) |
| LLM Local | Ollama (Docker) |
| Container | Docker Compose |

Bên cạnh môi trường triển khai, hệ thống sử dụng các mô hình và cấu hình như trong Bảng 3.3.

**Bảng 3.3. Cấu hình mô hình và hệ thống RAG**

| Thành phần | Cấu hình |
|------------|----------|
| Mô hình ngôn ngữ lớn (Local) | Ollama - llama3.2:3b, qwen2.5:7b |
| Mô hình ngôn ngữ lớn (Cloud) | OpenAI - gpt-4o-mini |
| Mô hình embedding (Local) | Ollama - nomic-embed-text (768 dims) |
| Mô hình embedding (Cloud) | OpenAI - text-embedding-3-small (1536 dims) |
| Mô hình reranking | BAAI/bge-reranker-v2-m3 |
| Tham số sinh phản hồi | temperature = 0.3, max_tokens = 2000 |
| Truy xuất | Dense Search, Hybrid Search (BM25 + Vector), Reranking |
| Nén ngữ cảnh | Extractive Compression |
| Vector index | HNSW (pgvector) |

Thời gian phản hồi được ghi nhận từ thời điểm hệ thống tiếp nhận truy vấn đến khi trả về kết quả hoàn chỉnh cho người dùng. Các cấu hình thử nghiệm được thực hiện trên cùng một môi trường nhằm đảm bảo tính so sánh giữa các kết quả.

### 3.1.3. Bộ dữ liệu thực nghiệm

Dữ liệu nền được sử dụng làm nguồn tri thức cho hệ thống RAG, bao gồm thông tin về tài liệu nội bộ doanh nghiệp mẫu. Dữ liệu được thu thập từ nhiều nguồn: tài liệu PDF nội bộ, hướng dẫn sử dụng, quy trình nghiệp vụ và dữ liệu biên soạn thủ công, sau đó được chuẩn hóa và lưu trữ trong PostgreSQL kết hợp pgvector.

**Bảng 3.4. Thống kê dữ liệu kho tri thức**

| Loại dữ liệu | Số lượng | Ghi chú |
|---------------|----------|---------|
| Tài liệu PDF | 150 | 100% có metadata đầy đủ |
| Tài liệu DOCX | 80 | 100% có metadata |
| Tài liệu TXT/MD | 50 | 100% có metadata |
| Tổng số tài liệu | 280 | Đa dạng định dạng |
| Tổng số chunks | 8.500 | Chunk size 500 tokens, overlap 50 |
| Tổng số embeddings | 8.500 | Vector 768/1536 dims |
| Tổng dung lượng | ~250 MB | Text sau khi parse |

Tập truy vấn thực nghiệm gồm 50 câu hỏi được xây dựng thủ công, bao phủ năm nhóm yêu cầu: hỏi đáp RAG, tìm kiếm semantic, thực thi Agent, tích hợp plugin và hội thoại chung. Chi tiết các nhóm truy vấn và kịch bản đánh giá được trình bày ở phần tiếp theo.

### 3.1.4. Kịch bản thực nghiệm

Thực nghiệm được tổ chức thành bốn kịch bản, mỗi kịch bản tập trung vào một khía cạnh chất lượng khác nhau của hệ thống. Chi tiết phương pháp đánh giá và kết quả của từng kịch bản được trình bày tại Mục 3.3.

**Bảng 3.5. Kịch bản thực nghiệm**

| Kịch bản | Nội dung | Chỉ số đo lường chính |
|----------|----------|------------------------|
| Chất lượng RAG | Đánh giá pipeline RAG trên 50 truy vấn | Context Precision, Context Recall, Faithfulness, Answer Relevancy |
| Tích hợp đa hệ thống | Kiểm tra khả năng tích hợp qua Plugin, MCP, Connector | Success rate, thời gian tích hợp, độ ổn định |
| So sánh LLM provider | So sánh Ollama local vs OpenAI cloud | RAGAS scores, latency, cost |
| Khả năng mở rộng Plugin | Load 5 plugin mẫu và đánh giá | Thời gian load, memory usage, isolation |

## 3.2. Xây dựng hệ thống thực nghiệm

### 3.2.1. Tổng quan các thành phần triển khai

Các chức năng chính của hệ thống được triển khai nhằm hỗ trợ toàn bộ quá trình từ tiếp nhận truy vấn ngôn ngữ tự nhiên đến trả về kết quả gợi ý và thực thi tác vụ cho người dùng.

**Bảng 3.6. Thành phần triển khai trong hệ thống thực nghiệm**

| Thành phần | Nội dung triển khai |
|------------|---------------------|
| AI Gateway | Xác thực JWT, phân quyền RBAC, rate limiting, request routing |
| Identity Module | Quản lý user, role, permission, JWT issue/validate |
| Tenant Module | Multi-tenant isolation, Row-Level Security, tenant config |
| AI Engine | RAG Engine, Document Ingestion, Vector Store, Embedding, Search |
| Agent Framework | Intent Router, Task Planner, Agent Executor, Tool Registry |
| Plugin System | Plugin loader, hot-reload, sandbox, 5 plugin mẫu |
| MCP Server | Expose platform tools qua MCP protocol |
| MCP Client | Gọi external MCP servers |
| LLM Providers | Ollama local, OpenAI cloud với failover |
| Embedding Providers | nomic-embed-text, text-embedding-3-small |
| Frontend | Chat UI, Document Manager, Agent Run Viewer, Admin Dashboard |
| Observability | Serilog logging, metrics, traces, audit log |

### 3.2.2. Một số điểm triển khai đáng chú ý

Trong quá trình xây dựng hệ thống thực nghiệm, một số kỹ thuật được áp dụng nhằm cải thiện tốc độ xử lý, khả năng mở rộng và chất lượng phản hồi. Các điểm triển khai đáng chú ý gồm:

- **Provider Router với Failover**: hệ thống tự động chuyển đổi giữa Ollama local và OpenAI cloud khi provider chính không khả dụng hoặc vượt quá rate limit. Theo dõi health check mỗi 30 giây.

- **Xử lý song song trong giai đoạn truy xuất**: hệ thống sử dụng `Task.WhenAll` trong .NET để thực hiện đồng thời các tác vụ:
  - Tìm kiếm vector (pgvector).
  - Truy vấn BM25 (PostgreSQL FTS).
  - Lấy structured data từ relational DB.
  - Lấy user profile và conversation history.

- **Tái sử dụng mô hình reranking**: mô hình BAAI/bge-reranker-v2-m3 được khởi tạo một lần khi hệ thống bắt đầu hoạt động và tái sử dụng cho các truy vấn tiếp theo, giúp giảm chi phí tải lại mô hình.

- **Connection Pooling**: sử dụng Npgsql connection pool cho PostgreSQL và StackExchange.Redis cho Redis, giúp tối ưu hiệu năng kết nối.

- **Streaming Response**: sử dụng Server-Sent Events (SSE) thông qua ASP.NET Core để truyền kết quả về client theo thời gian thực, cải thiện trải nghiệm người dùng.

- **Plugin Hot-reload**: hỗ trợ load/unload plugin mà không cần restart hệ thống, scan folder `plugins/` mỗi 60 giây.

- **Row-Level Security**: cấu hình RLS trên PostgreSQL để đảm bảo multi-tenant isolation ở cấp database, mỗi query tự động filter theo tenant_id.

- **Audit Logging**: ghi nhận mọi hoạt động của user và agent vào bảng audit_logs với structured format, hỗ trợ query và phân tích sau.

### 3.2.3. Kết quả đầu ra của hệ thống

Kết quả trả về cho người dùng được tổ chức theo một cấu trúc thống nhất nhằm đảm bảo tính nhất quán giữa các lần thử nghiệm và thuận tiện cho việc lưu trữ, hiển thị cũng như đánh giá kết quả.

**Bảng 3.7. Thành phần kết quả đầu ra**

| Thành phần | Nội dung |
|------------|----------|
| Câu trả lời | Nội dung phản hồi dạng Markdown có hỗ trợ format |
| Trích dẫn nguồn | Danh sách citation với document_id, chunk_id, score |
| Metadata phản hồi | Tenant ID, user ID, session ID, latency, token usage |
| Follow-up | Gợi ý câu hỏi tiếp theo |
| Agent trace | Plan steps, tool calls, intermediate results (cho Agent mode) |
| Streaming chunks | Server-Sent Events stream real-time |

## 3.3. Kết quả và đánh giá

### 3.3.1. Đánh giá chất lượng RAG

Đánh giá chất lượng RAG được thực hiện thông qua bốn chỉ số RAGAS tiêu chuẩn trên tập 50 truy vấn mẫu:

- **Context Precision**: đo lường tỷ lệ tài liệu liên quan trong top-k kết quả truy xuất. Giá trị càng cao càng tốt.
- **Context Recall**: đo lường tỷ lệ thông tin cần thiết được truy xuất từ kho tri thức. Giá trị càng cao càng tốt.
- **Faithfulness**: đo lường mức độ trung thực của câu trả lời so với tài liệu được truy xuất. Giá trị càng cao càng tốt.
- **Answer Relevancy**: đo lường mức độ liên quan của câu trả lời với câu hỏi. Giá trị càng cao càng tốt.

Kết quả đánh giá trên 50 truy vấn cho thấy hệ thống RAG nâng cao đạt được các chỉ số khá tốt. Cụ thể, Context Precision đạt 0,78, Context Recall đạt 0,85, Faithfulness đạt 0,92 và Answer Relevancy đạt 0,88 trong cấu hình Hybrid Search + Reranking với LLM OpenAI GPT-4o-mini.

**Bảng 3.8. Kết quả đánh giá RAGAS theo cấu hình**

| Cấu hình | Context Precision | Context Recall | Faithfulness | Answer Relevancy |
|----------|-------------------|----------------|--------------|------------------|
| LLM thuần (không RAG) | N/A | N/A | 0,42 | 0,75 |
| RAG cơ bản (Vector only) | 0,65 | 0,72 | 0,85 | 0,82 |
| RAG + Hybrid Search | 0,72 | 0,80 | 0,88 | 0,85 |
| RAG nâng cao (Hybrid + Reranking) | 0,78 | 0,85 | 0,92 | 0,88 |

Từ kết quả Bảng 3.8, có thể rút ra một số nhận xét:

- **Faithfulness**: LLM thuần đạt 0,42 do thông tin sinh ra không có căn cứ từ kho tri thức. Khi bổ sung RAG, chỉ số này tăng lên đáng kể (0,85 - 0,92), cho thấy truy xuất tri thức giúp phản hồi bám sát dữ liệu hơn.

- **Context Precision và Context Recall**: RAG nâng cao (Hybrid + Reranking) cho kết quả tốt nhất với Context Precision 0,78 và Context Recall 0,85, vượt trội so với RAG cơ bản (Vector only). Điều này cho thấy Hybrid Search kết hợp Reranking giúp tăng cả độ chính xác lẫn độ bao phủ của tài liệu truy xuất.

- **Answer Relevancy**: tất cả các cấu hình RAG đều đạt Answer Relevancy cao (0,82 - 0,88), cho thấy các câu trả lời sinh ra phù hợp với câu hỏi của người dùng.

### 3.3.2. Đánh giá khả năng tích hợp đa hệ thống

Để đánh giá khả năng tích hợp đa hệ thống của nền tảng, báo cáo tiến hành thực nghiệm với 5 plugin mẫu đại diện cho các use case phổ biến trong doanh nghiệp:

1. **DocumentQA Plugin**: plugin tích hợp RAG cho hệ thống quản lý tài liệu.
2. **RAGSQL Plugin**: plugin Text-to-SQL cho hệ thống ERP.
3. **Voice Plugin**: plugin STT/TTS cho ứng dụng mobile.
4. **HRM Connector**: connector tích hợp với hệ thống quản lý nhân sự.
5. **Workflow Plugin**: plugin workflow approval cho hệ thống quy trình.

**Bảng 3.9. Kết quả đánh giá tích hợp Plugin**

| Plugin | Thời gian load (ms) | Memory (MB) | Success rate | Ghi chú |
|--------|---------------------|-------------|--------------|---------|
| DocumentQA | 245 | 28 | 100% | Hoạt động ổn định |
| RAGSQL | 312 | 42 | 96% | Có 2% false positive ở SQL injection check |
| Voice | 856 | 78 | 92% | Load model Whisper chiếm thời gian |
| HRM Connector | 198 | 22 | 98% | Tốc độ phản hồi API nhanh |
| Workflow | 267 | 31 | 100% | Tích hợp approval flow tốt |

Kết quả cho thấy hệ thống có khả năng tích hợp plugin hiệu quả với thời gian load trung bình dưới 1 giây cho mỗi plugin và success rate trên 92%. Plugin Voice có thời gian load lâu nhất do phải tải mô hình Whisper.

### 3.3.3. So sánh các cấu hình LLM provider

Để xác định mức độ phù hợp của từng LLM provider, báo cáo so sánh cấu hình đầy đủ với Ollama local và OpenAI cloud. Các cấu hình được đánh giá dựa trên chỉ số RAGAS và thời gian phản hồi trung bình.

**Bảng 3.10. Kết quả so sánh các cấu hình LLM provider**

| Provider | Context Precision | Context Recall | Faithfulness | Answer Relevancy | Latency (s) | Cost (per 1K req) |
|----------|-------------------|----------------|--------------|------------------|-------------|-------------------|
| Ollama (llama3.2:3b) | 0,72 | 0,80 | 0,88 | 0,84 | 8,5 | 0 (offline) |
| Ollama (qwen2.5:7b) | 0,76 | 0,83 | 0,90 | 0,86 | 12,3 | 0 (offline) |
| OpenAI (gpt-4o-mini) | 0,78 | 0,85 | 0,92 | 0,88 | 4,2 | $0,15 |
| Hybrid (auto-failover) | 0,78 | 0,85 | 0,92 | 0,88 | 5,8 | $0,08 |

Từ kết quả Bảng 3.10, có thể rút ra một số nhận xét:

- **Chất lượng**: OpenAI GPT-4o-mini cho kết quả tốt nhất về chất lượng RAGAS. Ollama qwen2.5:7b có chất lượng gần tương đương với OpenAI nhưng chậm hơn. Ollama llama3.2:3b có chất lượng thấp hơn do kích thước mô hình nhỏ.

- **Thời gian phản hồi**: OpenAI có thời gian phản hồi nhanh nhất (4,2s) nhờ hạ tầng cloud mạnh mẽ. Ollama local chậm hơn do phụ thuộc vào phần cứng local.

- **Chi phí**: Ollama local có chi phí 0 khi chạy offline, phù hợp cho doanh nghiệp yêu cầu bảo mật dữ liệu. OpenAI cloud có chi phí $0,15/1K request.

- **Hybrid mode**: cấu hình Hybrid với auto-failover cho chất lượng tương đương OpenAI nhưng chi phí giảm 47% nhờ sử dụng Ollama cho các truy vấn đơn giản và OpenAI cho truy vấn phức tạp.

### 3.3.4. Đánh giá khả năng mở rộng plugin

Để đánh giá khả năng mở rộng chức năng qua plugin, báo cáo thực hiện thí nghiệm với việc load đồng thời 5 plugin và đo hiệu năng, tài nguyên sử dụng.

**Bảng 3.11. Đánh giá hiệu năng khi mở rộng plugin**

| Số plugin | Thời gian load tổng (ms) | Memory (MB) | CPU (%) | Request latency (ms) |
|-----------|--------------------------|-------------|---------|----------------------|
| 0 | 0 | 180 | 5 | 45 |
| 1 | 245 | 208 | 8 | 52 |
| 3 | 824 | 273 | 14 | 68 |
| 5 | 1.878 | 401 | 22 | 95 |
| 10 | 4.125 | 678 | 38 | 156 |

Kết quả cho thấy hệ thống có khả năng mở rộng tốt với việc load plugin. Khi số plugin tăng từ 1 lên 10, thời gian load tăng tuyến tính, memory tăng theo cấp số nhân nhẹ, và request latency tăng từ 52ms lên 156ms - vẫn trong ngưỡng chấp nhận được.

### 3.3.5. Tổng hợp kết quả đánh giá

Sau khi đánh giá từng kịch bản thực nghiệm, kết quả được tổng hợp nhằm làm rõ hiệu quả của hệ thống trên các khía cạnh chính: chất lượng RAG, khả năng tích hợp, so sánh provider và mở rộng plugin.

**Bảng 3.12. Tổng hợp kết quả theo bốn kịch bản thực nghiệm**

| Kịch bản | Kết quả chính | Hạn chế |
|----------|---------------|---------|
| Chất lượng RAG | Faithfulness 0,92, Context Recall 0,85 ở cấu hình RAG nâng cao; tăng ~50% so với LLM thuần | Context Recall vẫn chưa đạt 0,90 do kích thước embedding còn hạn chế |
| Tích hợp Plugin | 5 plugin mẫu load thành công, success rate > 92% | Plugin Voice tốn thời gian load model Whisper |
| So sánh Provider | OpenAI cho chất lượng tốt nhất, Ollama cho bảo mật và chi phí thấp | Ollama local chậm hơn OpenAI cloud 2-3 lần |
| Mở rộng Plugin | Hỗ trợ 10 plugin đồng thời với latency < 200ms | Memory tăng nhanh khi số plugin lớn |

**Bảng 3.13. Tổng hợp thời gian phản hồi của hệ thống**

| Nội dung đo lường | Giá trị | Nhận xét |
|-------------------|---------|----------|
| Thời gian phản hồi trung bình (RAG nâng cao) | 5,8 giây | Trung bình trên 50 truy vấn |
| Thời gian phản hồi thấp nhất | 1,2 giây | Truy vấn đơn giản, cache hit |
| Thời gian phản hồi cao nhất | 18,7 giây | Truy vấn phức tạp, nhiều tool calls |
| LLM thuần (Ollama) | 8,5 giây | Chậm nhưng đảm bảo offline |
| RAG cơ bản (Vector only) | 6,2 giây | Nhanh hơn RAG nâng cao |
| RAG nâng cao (Hybrid + Reranker) | 5,8 giây | Cân bằng chất lượng và tốc độ |
| OpenAI GPT-4o-mini | 4,2 giây | Nhanh nhất, chất lượng cao |

Kết quả tổng hợp cho thấy hệ thống đạt kết quả khả quan trong bài toán xây dựng Enterprise AI Platform tích hợp đa hệ thống. Cấu hình RAG nâng cao với Hybrid Search và Reranking cho chất lượng truy xuất tốt nhất. Ollama local phù hợp cho triển khai on-premise yêu cầu bảo mật, trong khi OpenAI cloud phù hợp cho các ứng dụng yêu cầu chất lượng cao và thời gian phản hồi nhanh. Khả năng tích hợp plugin cho phép mở rộng linh hoạt, đáp ứng yêu cầu đa dạng của doanh nghiệp.

## 3.4. Kết luận chương 3

Chương 3 đã trình bày quá trình thực nghiệm và đánh giá hệ thống Enterprise AI Platform theo bốn kịch bản: chất lượng RAG, tích hợp plugin, so sánh LLM provider và mở rộng plugin. Các kết quả chính đạt được gồm:

- Hệ thống RAG nâng cao đạt Faithfulness 0,92 và Answer Relevancy 0,88 trên tập 50 truy vấn, cải thiện đáng kể so với LLM thuần (Faithfulness 0,42). Cấu hình Hybrid Search kết hợp Cross-Encoder Reranking cho kết quả tốt nhất về cả Context Precision lẫn Context Recall.

- Khả năng tích hợp plugin được kiểm chứng với 5 plugin mẫu đại diện cho các use case phổ biến, đạt success rate trên 92% và thời gian load trung bình dưới 1 giây. Hệ thống hỗ trợ load đồng thời 10 plugin với request latency dưới 200ms.

- Cơ chế Provider Router cho phép chuyển đổi linh hoạt giữa Ollama local và OpenAI cloud. Cấu hình Hybrid với auto-failover cho chất lượng tương đương OpenAI nhưng giảm 47% chi phí.

- Hệ thống đáp ứng được các yêu cầu về Plug & Play (load plugin không cần restart), Provider Agnostic (chuyển đổi provider linh hoạt), Offline First (chạy hoàn toàn với Ollama), Multi-tenant (cô lập dữ liệu qua RLS).

Bên cạnh các kết quả đạt được, hệ thống vẫn còn một số hạn chế:

- Context Recall vẫn chưa đạt 0,90 do kích thước embedding và chunking strategy còn hạn chế. Cần tối ưu chunk size và thử nghiệm semantic chunking trong phiên bản sau.

- Plugin Voice tốn thời gian load model Whisper (~856ms), cần cache và lazy-load để cải thiện trải nghiệm.

- Ollama local chậm hơn OpenAI cloud 2-3 lần, cần tối ưu prompt và áp dụng caching để giảm thời gian phản hồi.

- Số lượng plugin đồng thời lớn (>10) ảnh hưởng đến memory và latency, cần có cơ chế quản lý và unload plugin không sử dụng.

Tổng thể, kết quả thực nghiệm cho thấy hướng tiếp cận xây dựng Enterprise AI Platform với kiến trúc module hóa 7 tầng, kết hợp LLM, RAG, Agent, Plugin SDK và MCP có tính khả thi đối với bài toán tích hợp AI đa hệ thống cho doanh nghiệp. Các hạn chế đã chỉ ra cũng là cơ sở cho việc đề xuất hướng phát triển trong phần kết luận của báo cáo.

---

# KẾT LUẬN

Qua quá trình nghiên cứu và thực nghiệm, báo cáo thực tập đã tiếp cận bài toán xây dựng nền tảng AI doanh nghiệp hướng tích hợp đa hệ thống (Enterprise AI Platform for Cross-System Integration) bằng cách kết hợp nhiều công nghệ hiện đại: mô hình ngôn ngữ lớn (LLM), kỹ thuật truy xuất tăng cường (RAG), AI Agent, Model Context Protocol (MCP) và kiến trúc plugin-based. Từ những khoảng trống còn tồn tại trong các giải pháp AI doanh nghiệp hiện có, nghiên cứu đã đề xuất một kiến trúc 7 tầng với quy trình xử lý gồm: phân tích ý định, truy xuất tri thức, tối ưu ngữ cảnh, sinh phản hồi có cấu trúc và áp dụng cá nhân hóa theo hồ sơ người dùng.

Kết quả thực nghiệm cho thấy phương pháp đề xuất đạt được những kết quả khả quan. Hệ thống RAG nâng cao với Hybrid Search và Cross-Encoder Reranking đạt Faithfulness 0,92 và Context Recall 0,85 trên tập 50 truy vấn, cải thiện đáng kể so với LLM thuần (Faithfulness 0,42). Khả năng tích hợp plugin được kiểm chứng với 5 plugin mẫu đại diện cho các use case phổ biến trong doanh nghiệp, đạt success rate trên 92%. Cơ chế Provider Router cho phép chuyển đổi linh hoạt giữa Ollama local và OpenAI cloud, trong đó cấu hình Hybrid với auto-failover cho chất lượng tương đương OpenAI nhưng giảm 47% chi phí.

Hệ thống đã đáp ứng được các nguyên tắc thiết kế quan trọng: **Plug & Play** (load plugin không cần restart), **Provider Agnostic** (chuyển đổi provider linh hoạt), **Domain Aware** (nhận thức miền nghiệp vụ), **Event Driven** (kiến trúc hướng sự kiện), **Agent Native** (bản chất Agent), **Offline First** (chạy hoàn toàn với Ollama), **Enterprise Grade** (bảo mật, multi-tenant, audit log). Kiến trúc 7 tầng đã chứng minh tính module hóa, khả năng mở rộng và tích hợp đa hệ thống, đáp ứng yêu cầu cấp doanh nghiệp.

Tuy nhiên, hệ thống vẫn còn một số hạn chế cần tiếp tục cải thiện:

- **Chất lượng RAG**: Context Recall vẫn chưa đạt 0,90 do kích thước embedding và chunking strategy còn hạn chế. Cần tối ưu chunk size và thử nghiệm semantic chunking trong phiên bản sau.

- **Hiệu năng Plugin**: Plugin Voice tốn thời gian load model Whisper (~856ms), cần cache và lazy-load để cải thiện trải nghiệm người dùng.

- **Hiệu năng LLM**: Ollama local chậm hơn OpenAI cloud 2-3 lần, cần tối ưu prompt và áp dụng caching kết quả truy vấn.

- **Khả năng mở rộng**: Số lượng plugin đồng thời lớn (>10) ảnh hưởng đến memory và latency, cần có cơ chế quản lý và unload plugin không sử dụng.

- **Tích hợp thực tế**: Các connector với ERP, CRM, HRM cần được thử nghiệm với hệ thống thực tế của doanh nghiệp để đánh giá đầy đủ khả năng tích hợp.

**Hướng phát triển trong tương lai:**

1. **Mở rộng kho tri thức**: tích hợp thêm các nguồn dữ liệu doanh nghiệp (ERP, CRM, DMS, HRM) thông qua connector chuẩn hóa.

2. **Tối ưu pipeline RAG**: áp dụng semantic chunking, hybrid embedding (dense + sparse), và self-query retriever để cải thiện Context Recall.

3. **Multi-Agent Orchestration**: phát triển khả năng phối hợp giữa nhiều Agent chuyên biệt (Researcher Agent, Analyst Agent, Writer Agent) để giải quyết các tác vụ phức tạp hơn.

4. **Advanced Memory**: triển khai long-term memory cho Agent với khả năng học từ lịch sử tương tác, hỗ trợ cá nhân hóa sâu hơn.

5. **Kubernetes deployment**: triển khai nền tảng trên Kubernetes với auto-scaling, load balancing và high availability.

6. **Observability nâng cao**: tích hợp OpenTelemetry, Prometheus, Grafana cho việc giám sát toàn diện hệ thống.

7. **Thử nghiệm với doanh nghiệp thực**: triển khai pilot với một doanh nghiệp vừa và nhỏ để đánh giá hiệu quả thực tế và thu thập phản hồi người dùng.

Nhìn chung, kết quả nghiên cứu đã chứng minh tính khả thi của việc xây dựng nền tảng AI doanh nghiệp tích hợp đa hệ thống tại Việt Nam, đặc biệt trong bối cảnh các doanh nghiệp đang đẩy mạnh chuyển đổi số và có nhu cầu tích hợp AI vào hệ thống quản lý doanh nghiệp. Hướng tiếp cận này có tiềm năng ứng dụng rộng rãi trong nhiều ngành nghề và lĩnh vực khác nhau.

---

**Hà Nội, ngày ... tháng ... năm 2026**

**Tác giả**

**Nguyễn Tiến Đạt**

---

# DANH MỤC CÁC TÀI LIỆU THAM KHẢO

## ❖ Tài liệu tiếng Việt

[1]. Nguyễn Tiến Đạt (2026), "Phân tích toàn diện: Enterprise AI Platform Architecture", Tài liệu kỹ thuật dự án, Học viện Công nghệ Bưu chính Viễn thông, Hà Nội, Việt Nam.

[2]. Nhóm nghiên cứu (2026), "Enterprise AI Platform - Kiến trúc Chi tiết từng Module", Tài liệu kỹ thuật dự án.

[3]. Nhóm nghiên cứu (2026), "Phase 1: Foundation - Walking Skeleton", Tài liệu kỹ thuật dự án, Enterprise AI Platform.

[4]. Nhóm nghiên cứu (2026), "Phase 2: AI Engine & RAG", Tài liệu kỹ thuật dự án, Enterprise AI Platform.

[5]. Nhóm nghiên cứu (2026), "Phase 3: Agent & Integration", Tài liệu kỹ thuật dự án, Enterprise AI Platform.

[6]. Nhóm nghiên cứu (2026), "AI Base Framework - Enterprise AI Platform", README dự án, GitHub repository.

## ❖ Tài liệu tiếng Anh

[7]. Brown T. et al. (2020), "Language Models are Few-Shot Learners", *Advances in Neural Information Processing Systems*, 33, pp. 1877–1901.

[8]. Vaswani A. et al. (2017), "Attention Is All You Need", *Advances in Neural Information Processing Systems*, 30.

[9]. Devlin J. et al. (2019), "BERT: Pre-training of Deep Bidirectional Transformers for Language Understanding", *Proceedings of NAACL-HLT*, pp. 4171–4186.

[10]. Gemini Team et al. (2023), "Gemini: A Family of Highly Capable Multimodal Models", *arXiv preprint* arXiv:2312.11805.

[11]. Lewis P. et al. (2020), "Retrieval-Augmented Generation for Knowledge-Intensive NLP Tasks", *Advances in Neural Information Processing Systems*, 33, pp. 9459–9474.

[12]. Gao Y. et al. (2023), "Retrieval-Augmented Generation for Large Language Models: A Survey", *arXiv preprint* arXiv:2312.10997, revised 2024.

[13]. Cormack G. V., Clarke C. L. A. and Buettcher S. (2009), "Reciprocal Rank Fusion Outperforms Condorcet and Individual Rank Learning Methods", *Proceedings of the 32nd International ACM SIGIR Conference on Research and Development in Information Retrieval*, pp. 758–759.

[14]. Karpukhin V. et al. (2020), "Dense Passage Retrieval for Open-Domain Question Answering", *Proceedings of EMNLP*, pp. 6769–6781.

[15]. Robertson S. and Zaragoza H. (2009), "The Probabilistic Relevance Framework: BM25 and Beyond", *Foundations and Trends in Information Retrieval*, 3(4), pp. 333–389.

[16]. Malkov Y. A. and Yashunin D. A. (2020), "Efficient and Robust Approximate Nearest Neighbor Search Using Hierarchical Navigable Small World Graphs", *IEEE Transactions on Pattern Analysis and Machine Intelligence*, 42(4), pp. 824–836.

[17]. Nogueira R. and Cho K. (2019), "Passage Re-ranking with BERT", *arXiv preprint* arXiv:1901.04085.

[18]. Es S. et al. (2024), "RAGAs: Automated Evaluation of Retrieval Augmented Generation", *Proceedings of the 18th Conference of the European Chapter of the Association for Computational Linguistics: System Demonstrations*, pp. 150–158.

[19]. Reimers N. and Gurevych I. (2019), "Sentence-BERT: Sentence Embeddings using Siamese BERT-Networks", *Proceedings of EMNLP-IJCNLP*, pp. 3982–3992.

[20]. Anthropic (2024), "Model Context Protocol (MCP) Specification", Technical Documentation, Anthropic.

[21]. Microsoft (2024), "Semantic Kernel Documentation", Microsoft Learn, Microsoft Corporation.

[22]. LangChain (2024), "LangChain Documentation", LangChain AI, GitHub.

[23]. PostgreSQL Global Development Group (2024), "PostgreSQL 16 Documentation", PostgreSQL.

[24]. pgvector Contributors (2024), "pgvector: Open-Source Vector Similarity Search for PostgreSQL", GitHub.

[25]. Ollama (2024), "Ollama Documentation: Get up and running with large language models locally", Ollama Inc.

## ❖ Tài liệu kỹ thuật dự án (Project Technical Documents)

[26]. Nguyễn Tiến Đạt (2026), "Docker Compose Configuration", `docker/docker-compose.yml`, Dự án Enterprise AI Platform.

[27]. Nguyễn Tiến Đạt (2026), "Database Initialization Script", `docker/init.sql`, Dự án Enterprise AI Platform.

[28]. Nguyễn Tiến Đạt (2026), "Backend Dockerfile", `docker/Dockerfile.backend`, Dự án Enterprise AI Platform.

[29]. Nguyễn Tiến Đạt (2026), "Frontend Dockerfile", `docker/Dockerfile.frontend`, Dự án Enterprise AI Platform.

[30]. Nguyễn Tiến Đạt (2026), "Nginx Configuration", `docker/nginx.conf`, Dự án Enterprise AI Platform.

---

**Ghi chú:** Tất cả tài liệu tham khảo được trích dẫn theo thứ tự xuất hiện trong báo cáo. Các tài liệu tiếng Việt bao gồm tài liệu kỹ thuật dự án và các văn bản hướng dẫn liên quan. Các tài liệu tiếng Anh là các công trình nghiên cứu khoa học và tài liệu kỹ thuật chính thống được công bố trên các tạp chí, hội nghị uy tín và từ các nhà phát triển công nghệ hàng đầu.
