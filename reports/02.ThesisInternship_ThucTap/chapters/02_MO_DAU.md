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
