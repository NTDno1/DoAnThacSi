# Chương 2: PHÂN TÍCH BÀI TOÁN VÀ THIẾT KẾ NỀN TẢNG

Chương 1 của báo cáo đã trình bày nền tảng lý thuyết và tổng quan nghiên cứu liên quan đến đề tài, bao gồm: (i) trí tuệ nhân tạo và mô hình ngôn ngữ lớn; (ii) kỹ thuật Retrieval-Augmented Generation; (iii) AI Agent và cơ chế phối hợp đa tác nhân; (iv) giao thức Model Context Protocol; (v) tích hợp hệ thống doanh nghiệp; (vi) khảo sát các nghiên cứu và hệ thống liên quan. Trên cơ sở những nền tảng lý thuyết đó, chương này chuyển trọng tâm sang phân tích bài toán cụ thể của đề tài và đề xuất kiến trúc tổng thể cho nền tảng Enterprise AI Platform.

Nội dung chương được tổ chức theo trình tự từ trừu tượng đến cụ thể: bắt đầu bằng phân tích bài toán và yêu cầu hệ thống (mục 2.1), tiếp theo là thiết kế kiến trúc tổng thể và các nguyên tắc nền tảng (mục 2.2), sau đó đi sâu vào thiết kế chi tiết các module cốt lõi (mục 2.3), thiết kế dữ liệu và cơ sở tri thức (mục 2.4), thiết kế giao thức tích hợp và bảo mật (mục 2.5), và cuối cùng là quy trình triển khai theo giai đoạn (mục 2.6). Mỗi phần đều được phân tích với mức chi tiết phù hợp cho một thiết kế kiến trúc học thuật: có mô tả bài toán, phân tích các phương án, so sánh đa tiêu chí, lựa chọn có cơ sở, và minh họa bằng sơ đồ, bảng hoặc mã nguồn. Mục tiêu cuối cùng của chương là cung cấp một bản thiết kế có tính khả thi, có khả năng định hướng chi tiết cho quá trình cài đặt và thực nghiệm ở Chương 3.

## 2.1. Phân tích bài toán

### 2.1.1. Mô tả bài toán và bối cảnh doanh nghiệp

Trong bối cảnh chuyển đổi số hiện nay, các doanh nghiệp vừa và lớn tại Việt Nam nói riêng và khu vực Đông Nam Á nói chung đang vận hành đồng thời nhiều hệ thống thông tin chuyên ngành: Hệ thống hoạch định nguồn lực doanh nghiệp (Enterprise Resource Planning - ERP), Hệ thống quản trị quan hệ khách hàng (Customer Relationship Management - CRM), Hệ thống quản lý tài liệu (Document Management System - DMS), Hệ thống quản trị nhân sự (Human Resource Management - HRM), các hệ thống quản lý quy trình nghiệp vụ (Workflow Management System), và các hệ thống trực quan hóa dữ liệu (Business Intelligence - BI). Mỗi hệ thống đảm nhận một phạm vi chức năng riêng, sử dụng các mô hình dữ liệu, nghiệp vụ và giao thức tích hợp khác nhau, thường được triển khai tại nhiều thời điểm bởi các nhà cung cấp khác nhau, tạo nên một bức tranh hạ tầng CNTT phức tạp và ít nhiều phân mảnh.

Sự phân mảnh này tạo ra một khoảng cách đáng kể giữa khối lượng tri thức mà doanh nghiệp đang nắm giữ và khả năng khai thác thực tế của đội ngũ nhân sự. Theo khảo sát của Gartner năm 2024, trung bình một doanh nghiệp lớn vận hành từ 4 đến 7 hệ thống CNTT chính, nhưng chỉ khoảng 23% nhân viên có thể tìm kiếm thông tin xuyên suốt giữa các hệ thống này một cách hiệu quả. Khoảng cách này không chỉ ảnh hưởng đến năng suất lao động mà còn hạn chế khả năng ra quyết định dựa trên dữ liệu tổng hợp.

Khi các mô hình ngôn ngữ lớn (Large Language Models - LLM) và kỹ thuật truy xuất tăng cường (Retrieval-Augmented Generation - RAG) trở nên phổ biến trong những năm 2023-2024, nhiều doanh nghiệp nhận thấy tiềm năng to lớn trong việc ứng dụng AI để nâng cao hiệu quả vận hành, hỗ trợ ra quyết định và tự động hóa một phần nghiệp vụ. Tuy nhiên, các cách tiếp cận hiện tại thường gặp phải bốn nghịch lý có tính cấu trúc, mà đề tài xác định như sau:

**Nghịch lý thứ nhất - Phân mảnh giải pháp**: Mỗi hệ thống ERP, CRM, DMS... thường được tích hợp AI theo cách riêng biệt, dẫn đến hiện tượng nhân bản công sức triển khai, khó đồng bộ tri thức xuyên suốt giữa các hệ thống, và tăng chi phí bảo trì khi cần nâng cấp. Một tổ chức có thể có nhiều "hệ thống AI" khác nhau cho từng hệ thống nội bộ, thay vì một nền tảng AI chung duy nhất.

**Nghịch lý thứ hai - Khóa cứng nhà cung cấp**: Nhiều giải pháp AI doanh nghiệp hiện tại được thiết kế gắn chặt với một nhà cung cấp mô hình cụ thể (ví dụ: chỉ dùng OpenAI GPT-4 hoặc chỉ dùng Google Gemini). Điều này gây ra ba hệ quả tiêu cực: khó khăn khi muốn chuyển đổi sang mô hình có chi phí hiệu quả hơn; khó khăn khi muốn kết hợp nhiều mô hình cho các tác vụ khác nhau; và khó khăn khi doanh nghiệp muốn triển khai mô hình cục bộ (on-premise) để phục vụ yêu cầu tuân thủ dữ liệu.

**Nghịch lý thứ ba - Tri thức rời rạc**: Tri thức doanh nghiệp tồn tại trong nhiều hệ thống, nhiều định dạng khác nhau (văn bản, bảng biểu, hình ảnh, cơ sở dữ liệu quan hệ, email, chat log), trong khi nhu cầu tra cứu và tổng hợp của người dùng lại mang tính xuyên suốt. Người dùng không quan tâm rằng thông tin "đơn hàng" nằm ở CRM và thông tin "tồn kho" nằm ở ERP - họ muốn hỏi một câu duy nhất và nhận được câu trả lời tổng hợp.

**Nghịch lý thứ tư - An toàn dữ liệu xung đột với tiện ích AI**: Dữ liệu doanh nghiệp thường chứa thông tin nhạy cảm (tài chính, khách hàng, nhân sự, bí mật thương mại). Nhiều giải pháp AI thương mại đòi hỏi gửi dữ liệu ra bên ngoài để xử lý, tạo xung đột trực tiếp với chính sách an toàn thông tin, yêu cầu GDPR (đối với doanh nghiệp có khách hàng châu Âu), hoặc quy định về lưu trữ dữ liệu ngành tài chính, y tế.

Từ thực tiễn trên, đề tài đặt ra bài toán nghiên cứu: *"Xây dựng một nền tảng AI doanh nghiệp (Enterprise AI Platform) có khả năng tích hợp đa hệ thống, cho phép người dùng chỉ cần cắm (plug) nền tảng vào hệ thống hiện có là có thể khai thác được sức mạnh của mô hình ngôn ngữ lớn, mà không cần đại tu hạ tầng, không cần cam kết với một nhà cung cấp AI cụ thể, và không cần đánh đổi tính an toàn dữ liệu."* Sứ mệnh này đặt ra yêu cầu tổng hợp: nền tảng phải trở thành một lớp trung gian trí tuệ (intelligence middleware layer) giữa hệ thống doanh nghiệp và người dùng cuối, đồng thời phải tôn trọng tính đa dạng về hạ tầng, mô hình AI và chính sách dữ liệu của từng doanh nghiệp.

Bối cảnh doanh nghiệp mục tiêu của đề tài là các doanh nghiệp có quy mô từ 100 đến 5.000 nhân viên, đã có hạ tầng CNTT ổn định nhưng chưa có một lớp AI thống nhất. Các đặc thù nghiệp vụ phổ biến được tập trung gồm: (i) tra cứu thông tin nội bộ qua hội thoại tự nhiên; (ii) hỏi đáp trên tài liệu doanh nghiệp (hợp đồng, quy chế, tài liệu kỹ thuật, sổ tay nhân viên); (iii) sinh báo cáo tự động từ dữ liệu quan hệ; (iv) hỗ trợ nghiệp vụ qua giọng nói tiếng Việt; (v) tự động hóa quy trình xuyên suốt nhiều hệ thống. Năm đặc thù này được sử dụng làm kịch bản thực nghiệm trong Chương 3.

### 2.1.2. Yêu cầu chức năng

Trên cơ sở mô tả bài toán ở mục 2.1.1 và bối cảnh doanh nghiệp mục tiêu, đề tài xác định 12 yêu cầu chức năng (Functional Requirement - FR), được mã hóa theo định danh FR-x và mô tả với mức chi tiết phục vụ thiết kế kiến trúc. Danh sách đầy đủ được tổng hợp trong Bảng 2.1.

**Bảng 2.1. Danh sách yêu cầu chức năng của hệ thống**

| Mã | Tên yêu cầu | Mô tả chi tiết | Module chính phụ trách |
|---|---|---|---|
| FR-01 | Hội thoại đa lượt với LLM | Hỗ trợ hội thoại tự nhiên đa lượt, có lưu ngữ cảnh theo phiên, streaming response token theo thời gian thực. Có thể tạo nhiều phiên hội thoại song song. | AI Engine Core (Conversation Manager) |
| FR-02 | Truy xuất tri thức tăng cường (RAG) | Upload tài liệu đa định dạng (PDF, DOCX, Markdown, HTML, TXT), tạo embedding, truy xuất theo ngữ cảnh, sinh câu trả lời kèm trích dẫn nguồn. Hỗ trợ incremental indexing. | RAG Engine |
| FR-03 | Tìm kiếm ngữ nghĩa lai (Hybrid Search) | Kết hợp dense retrieval (vector similarity) và sparse retrieval (BM25), hợp nhất kết quả bằng RRF (Reciprocal Rank Fusion), có rerank bằng Cross-Encoder. | Search Engine (Hybrid Module) |
| FR-04 | Định tuyến ý định (Intent Routing) | Phân tích câu hỏi người dùng, phân loại ý định (tra cứu, dữ liệu, tự động hóa), chọn handler phù hợp. Có thể cấu hình routing rule. | Agent Orchestration (Intent Router) |
| FR-05 | Tác nhân AI đa bước (Multi-step Agent) | Lập kế hoạch gồm nhiều bước, gọi tool tuần tự hoặc phân nhánh, tự đánh giá kết quả và điều chỉnh, có giới hạn bước và token. | Agent Orchestration (Executor, Planner) |
| FR-06 | Tích hợp đa hệ thống qua MCP | Kết nối với ERP, CRM, DMS, HRM qua giao thức MCP chuẩn. Cho phép thêm/bớt hệ thống kết nối mà không cần dừng nền tảng. | Integration Layer (MCP Client) |
| FR-07 | Plugin có versioning và hot-reload | Mở rộng chức năng qua plugin, có thể nạp/gỡ khi hệ thống đang chạy. Plugin có manifest, phiên bản, và sandbox an toàn. | Integration Layer (Plugin SDK) |
| FR-08 | Truy vấn dữ liệu quan hệ bằng ngôn ngữ tự nhiên (Text-to-SQL) | Sinh câu SQL an toàn từ câu hỏi tiếng Việt, qua lớp kiểm duyệt SQLGuard. Chỉ hỗ trợ SELECT, có LIMIT, cấm truy cập bảng nhạy cảm. | AI Engine Core (SQL Engine) |
| FR-09 | Tương tác bằng giọng nói (Voice I/O) | Nhận diện giọng nói tiếng Việt (STT), sinh giọng nói tiếng Việt (TTS), tích hợp với engine hội thoại. Ưu tiên offline (Whisper.cpp). | AI Engine Core (Voice Engine) |
| FR-10 | Quản lý bộ nhớ hội thoại và người dùng | Lưu trữ ngắn hạn (trong phiên, Redis), dài hạn (thông tin người dùng, preferences, lịch sử tương tác), phân biệt theo người dùng/tenant. | AI Engine Core (Memory Service) |
| FR-11 | Đa người thuê với cô lập dữ liệu (Multi-tenant) | Từng tenant có cấu hình riêng (model, plugin, policy), dữ liệu cô lập hoàn toàn qua Row-Level Security. | Tenant Service, Database Layer |
| FR-12 | Giám sát, kiểm toán và phân tích | Ghi nhật ký toàn bộ truy vấn, phản hồi, token tiêu thụ. Ghi audit log bất biến. Dashboard phân tích sử dụng. | Analytics & Audit Service |

Các yêu cầu chức năng có mối quan hệ chặt chẽ và phụ thuộc lẫn nhau. FR-01 (hội thoại đa lượt) đóng vai trò nền tảng và được hiện thực ngay từ Phase 1. FR-02, FR-03, FR-08, FR-09 tạo thành nhóm các khả năng xử lý tri thức và dữ liệu (đặc trưng Phase 2 và Phase 4). FR-04, FR-05, FR-06, FR-07 tạo thành nhóm tích hợp và tự động hóa (đặc trưng Phase 3). FR-10, FR-11, FR-12 tạo thành nhóm nền tảng vận hành (đặc trưng Phase 5). Cách phân nhóm này phản ánh triết lý triển khai theo phase được trình bày ở mục 2.6.

Bên cạnh mô tả tóm tắt trong bảng, mỗi yêu cầu chức năng đều có các ràng buộc nghiệp vụ chi tiết hơn. Đối với FR-02, hệ thống phải hỗ trợ nhiều định dạng tài liệu phổ biến (PDF, DOCX, Markdown, HTML, TXT), phải có cơ chế tách đoạn (chunking) với kích thước cấu hình được (mặc định 512 token với độ trùng 64 token), và phải hỗ trợ cập nhật tài liệu gốc (incremental re-indexing) khi tài liệu thay đổi. Đối với FR-06, hệ thống phải cho phép thêm/bớt hệ thống tích hợp mà không cần dừng service, và phải có cơ chế xác thực riêng cho từng MCP server. Đối với FR-08, mọi câu SQL sinh ra phải đi qua SQLGuard trước khi thực thi, đảm bảo chỉ thực hiện SELECT, tự động thêm LIMIT 100 nếu thiếu, và cấm truy cập các bảng audit_log, credentials, api_keys. Đối với FR-11, cô lập dữ liệu phải được đảm bảo ở mức hàng dữ liệu (row-level) và phải tự động áp dụng vào mọi truy vấn của ứng dụng thông qua RLS.

### 2.1.3. Yêu cầu phi chức năng

Song song với các yêu cầu chức năng, đề tài xác định 8 nhóm yêu cầu phi chức năng (Non-Functional Requirement - NFR) đặc trưng cho hệ thống doanh nghiệp quy mô vừa. Các yêu cầu này chi phối trực tiếp đến quyết định kiến trúc (phong cách monolith, modular monolith hay microservice), lựa chọn công nghệ, và chiến lược triển khai. Danh sách đầy đủ được tổng hợp trong Bảng 2.2.

**Bảng 2.2. Danh sách yêu cầu phi chức năng của hệ thống**

| Mã | Tên yêu cầu | Mục tiêu đo lường cụ thể | Phạm vi áp dụng |
|---|---|---|---|
| NFR-01 | Hiệu năng (Performance) | Thời gian phản hồi trung bình dưới 2 giây cho truy vấn RAG cơ bản (top-5 chunks, không streaming); streaming token đầu tiên dưới 800ms; throughput tối thiểu 50 concurrent sessions | Toàn hệ thống |
| NFR-02 | Khả năng mở rộng (Scalability) | Hỗ trợ mở rộng theo chiều ngang qua container và orchestration, scale từ 1 tenant đến 500+ tenant trên cùng triển khai | Lớp AI Engine, Gateway |
| NFR-03 | Tính sẵn sàng (Availability) | Mục tiêu uptime 99,5% (tương đương tối đa 3,65 giờ downtime/tháng); graceful degradation khi LLM provider gặp sự cố (tự động fallback) | Toàn hệ thống |
| NFR-04 | Bảo mật (Security) | Tuân thủ OWASP Top 10; mã hóa AES-256 cho dữ liệu at rest; TLS 1.3 cho dữ liệu in transit; kiểm soát truy cập RBAC chi tiết; dependency scanning tự động | Toàn hệ thống |
| NFR-05 | Tuân thủ và kiểm toán (Compliance & Audit) | Ghi nhật ký toàn bộ truy vấn và phản hồi; log kiểm toán bất biến, có thể truy vết lịch sử phiên; hỗ trợ export log định dạng chuẩn | Toàn hệ thống |
| NFR-06 | Khả năng bảo trì (Maintainability) | Mã nguồn tổ chức theo module rõ ràng; có tài liệu API tự động (OpenAPI/Swagger); bao phủ test tối thiểu 80% ở mức unit, 60% ở mức integration; convention文档 đầy đủ | Lớp trình bày, business logic |
| NFR-07 | Khả năng quan sát (Observability) | Log theo chuẩn OpenTelemetry (trace, metric, log); có distributed tracing; dashboard Prometheus/Grafana; alerting khi threshold bị vi phạm | Toàn hệ thống |
| NFR-08 | Tính thuần nhất nhà cung cấp AI (Provider Agnosticism) | Cho phép thay đổi nhà cung cấp AI mà không cần sửa business logic; tối thiểu 3 provider được hỗ trợ ngay từ đầu | Lớp AI Provider Abstraction |

Các yêu cầu phi chức năng có mối quan hệ qua lại và đôi khi xung đột nhẹ với nhau. Ví dụ, NFR-01 (hiệu năng) có thể xung đột với NFR-02 (mở rộng) nếu chọn kiến trúc phân tán quá sớm vì chi phí mạng giữa các service sẽ làm tăng độ trễ. NFR-03 (sẵn sàng) và NFR-08 (provider agnosticism) cùng thúc đẩy thiết kế một lớp trừu tượng hóa mạnh mẽ ngay từ đầu để khi provider gặp sự cố có thể fallback không cần thay đổi code. NFR-04 (bảo mật) và NFR-01 (hiệu năng) cũng có căng thẳng nhẹ vì mã hóa và kiểm tra quyền tốn thêm thời gian xử lý. Do đó, đề tài sử dụng bảng yêu cầu phi chức năng như một tập ràng buộc đa mục tiêu, làm cơ sở cho các quyết định kiến trúc ở các mục tiếp theo.

Ngoài 8 nhóm yêu cầu phi chức năng trên, một số ràng buộc ngầm định cũng cần được nhắc đến. Thứ nhất, chi phí vận hành phải nằm trong ngân sách hợp lý đối với doanh nghiệp vừa (ước tính tối đa 50-100 triệu đồng/tháng cho hạ tầng cloud). Thứ hai, sản phẩm phải có khả năng triển khai offline (không phụ thuộc hoàn toàn vào dịch vụ bên ngoài, đặc biệt đối với các doanh nghiệp có chính sách dữ liệu nghiêm ngặt). Thứ ba, giao diện người dùng phải hỗ trợ tiếng Việt hoàn toàn từ giao diện, tài liệu đến thông báo lỗi. Thứ tư, hệ thống phải có khả năng tự phục hồi (self-healing) khi gặp sự cố nhỏ mà không cần can thiệp thủ công.

## 2.2. Kiến trúc tổng thể

### 2.2.1. Nguyên tắc thiết kế và triết lý kiến trúc

Trên cơ sở phân tích bài toán ở mục 2.1 và tổng hợp yêu cầu chức năng - phi chức năng ở Bảng 2.1 và Bảng 2.2, đề tài đề xuất 8 nguyên tắc thiết kế được xem là "kim chỉ nam" xuyên suốt quá trình phát triển. Mỗi nguyên tắc được gắn với một số yêu cầu chức năng/phi chức năng cụ thể và được hiện thực bằng các quyết định kiến trúc ở các mục 2.2.2, 2.3 và 2.5. Tám nguyên tắc này được thiết kế để giải quyết trực tiếp bốn nghịch lý đã được xác định ở mục 2.1.1.

**(1) PLUG & PLAY - Cắm vào là chạy**

Nguyên tắc PLUG & PLAY khẳng định rằng một doanh nghiệp có thể đưa nền tảng Enterprise AI Platform vào vận hành mà không phải sửa đổi hệ thống ERP/CRM/DMS hiện có, không phải thay đổi cấu trúc dữ liệu hiện tại, và không cần đội ngũ kỹ thuật chuyên sâu để tích hợp. Nguyên tắc này trực tiếp giải quyết Nghịch lý thứ nhất (phân mảnh giải pháp).

Để hiện thực nguyên tắc này, đề tài xây dựng ba cơ chế cốt lõi. Thứ nhất, Plugin SDK cho phép đóng gói mã tích hợp với một hệ thống cụ thể thành một plugin có thể cài đặt qua giao diện quản trị, không cần deploy lại toàn bộ nền tảng. Thứ hai, MCP Client cho phép nền tảng kết nối ngược ra bên ngoài thông qua giao thức chuẩn MCP, thay vì yêu cầu hệ thống bên ngoài phải thay đổi để tương thích. Thứ ba, cơ chế cấu hình kết nối không cần viết mã (no-code/low-code configuration) cho phép quản trị viên thiết lập kết nối mới chỉ bằng thao tác trên giao diện web. Nguyên tắc này trực tiếp phục vụ FR-06 (tích hợp MCP) và FR-07 (plugin).

Tuy nhiên, cần lưu ý rằng PLUG & PLAY không có nghĩa là "không cần cấu hình gì cả". Mức độ plug & play thực tế phụ thuộc vào mức độ chuẩn hóa của hệ thống đích. Một hệ thống có API REST chuẩn và tài liệu mô tả rõ ràng sẽ dễ tích hợp hơn so với một hệ thống legacy sử dụng giao thức độc quyền. Do đó, thiết kế tập trung vào việc giảm thiểu công sức tích hợp thay vì tuyên bố tích hợp 100% tự động.

**(2) PROVIDER AGNOSTIC - Không phụ thuộc nhà cung cấp AI**

Nguyên tắc PROVIDER AGNOSTIC đặt ra rằng hệ thống phải cho phép sử dụng kết hợp nhiều nhà cung cấp AI (OpenAI GPT-4o, Anthropic Claude 3.5, Google Gemini 2.0, Meta Llama 3.3, Mistral Large, Ollama local...) và cho phép chuyển đổi hoặc kết hợp tùy theo bài toán cụ thể, chi phí vận hành và chính sách dữ liệu của từng doanh nghiệp. Nguyên tắc này trực tiếp giải quyết Nghịch lý thứ hai (khóa cứng nhà cung cấp).

Hiện thực của nguyên tắc này nằm ở Layer 3 - Provider Abstraction (mục 2.3.1), với các interface chuẩn ILLMProvider, IEmbeddingProvider, Multi-Provider Router, và Cache. Cách tổ chức này đảm bảo rằng business logic ở Layer 4 và Layer 5 không bao giờ gọi trực tiếp API của nhà cung cấp cụ thể; mọi tương tác đều qua interface trừu tượng. Hệ quả trực tiếp là khi OpenAI thay đổi API, khi Anthropic ra phiên bản mới, hoặc khi doanh nghiệp muốn chuyển từ GPT-4o sang Claude 3.5, chỉ cần thay đổi một lớp hiện thực (implementation class) ở Layer 3 mà không ảnh hưởng đến phần còn lại của hệ thống. Nguyên tắc này đáp ứng NFR-08 (Provider Agnosticism).

Một hệ quả quan trọng của nguyên tắc PROVIDER AGNOSTIC là tính linh hoạt trong việc quản lý chi phí. Khi prompt đơn giản và có thể xử lý bằng mô hình rẻ hơn, hệ thống có thể tự động chuyển sang model nhẹ hơn; khi cần chất lượng cao, có thể chuyển sang model mạnh hơn. Điều này mang lại lợi ích kinh tế đáng kể cho doanh nghiệp vừa và nhỏ.

**(3) DOMAIN AWARE - Hiểu miền nghiệp vụ**

Nguyên tắc DOMAIN AWARE yêu cầu hệ thống phải có khả năng ánh xạ câu hỏi người dùng về đúng tài nguyên trong đúng miền nghiệp vụ (ERP, CRM, HRM, DMS...), thay vì truy xuất tổng quát một cách mơ hồ và thiếu chính xác. Nguyên tắc này gián tiếp giải quyết Nghịch lý thứ ba (tri thức rời rạc) bằng cách giúp hệ thống hiểu rằng câu hỏi "doanh thu tháng này" thuộc miền ERP trong khi câu hỏi "khách hàng VIP" thuộc miền CRM.

Hiện thực của nguyên tắc này gồm: Module Domain trong plugin (mỗi plugin gắn với một miền nghiệp vụ và có metadata mô tả); Intent Router ở Layer 5 sử dụng Domain Context để chọn handler phù hợp; RAG Engine ở Layer 4 hỗ trợ phân tách tri thức theo miền (domain-specific knowledge bases). Cách tổ chức này cho phép khi người dùng hỏi "tình trạng đơn hàng của khách A", hệ thống biết rằng câu hỏi thuộc miền CRM, sử dụng MCP server tương ứng, và truy vấn vào đúng hệ thống CRM.

**(4) EVENT DRIVEN - Hướng sự kiện, ghép nối lỏng**

Nguyên tắc EVENT DRIVEN đặt ra rằng các hành động giữa các module không nên gọi trực tiếp qua API đồng bộ khi có thể, mà nên giao tiếp qua event bus theo cơ chế phát hành/đăng ký (publish-subscribe). Điều này cho phép module hóa theo chiều ngang (tách biệt các domain logic), dễ mở rộng (thêm subscriber mới không cần sửa publisher), và chịu tải tốt hơn (event được xử lý bất đồng bộ).

Nguyên tắc này ảnh hưởng trực tiếp đến thiết kế Message Queue trong Layer 2 và Workflow Engine trong Layer 4. Ví dụ: khi một tài liệu được upload, thay vì module RAG Engine gọi trực tiếp module Embedding để xử lý (gắn kết chặt), RAG Engine chỉ phát sự kiện `DocumentUploaded` lên event bus. Embedding Service, Audit Service, Notification Service đều có thể đăng ký nhận sự kiện này và xử lý độc lập. Nếu Embedding Service tạm thời không khả dụng, sự kiện vẫn được lưu trong queue và sẽ được xử lý khi dịch vụ phục hồi - đây cũng là cơ chế hỗ trợ NFR-03 (Availability).

Tuy nhiên, nguyên tắc EVENT DRIVEN không phải là giải pháp cho mọi tình huống. Trong một số trường hợp cần đồng bộ (ví dụ: xác thực token), gọi trực tiếp vẫn phù hợp hơn. Đề tài áp dụng nguyên tắc "đồng bộ khi cần consistency, bất đồng bộ khi cần scalability" làm heuristic cho quyết định thiết kế.

**(5) AGENT NATIVE - Hỗ trợ AI Agent đa nhiệm từ gốc**

Nguyên tắc AGENT NATIVE khẳng định rằng kiến trúc không chỉ "hỗ trợ" AI Agent như một thành phần phụ thêm, mà Agent phải là đơn vị xử lý chính (first-class citizen) được thiết kế ngay từ đầu. Khác với các hệ thống truyền thống trong đó có một module AI riêng biệt, nguyên tắc này yêu cầu rằng mọi khả năng của nền tảng (RAG, SQL Engine, Voice, MCP, Plugin) đều phải có thể được gọi từ một Agent thông qua Tool Registry.

Hiện thực của nguyên tắc này dẫn đến Layer 5 (Agent Orchestration) là một tầng độc lập với giao diện rõ ràng với Layer 4 (LLM, tool, memory, human-in-the-loop). Intent Router, Task Planner, Executor, Memory Management, và HITL được thiết kế như các thành phần ngang hàng, có thể cấu hình và thay thế. Đặc biệt, Tool Registry ở Layer 4 được thiết kế để mọi tool (RAG, SQL, MCP call, workflow) đều có thể được LLM gọi theo cùng một cách thức chuẩn, thông qua schema định nghĩa tool theo chuẩn Anthropic Claude hoặc OpenAI function calling. Nguyên tắc này phục vụ FR-04 (Intent Routing) và FR-05 (Multi-step Agent).

**(6) OFFLINE FIRST - Ưu tiên xử lý cục bộ**

Nguyên tắc OFFLINE FIRST đặt ra rằng mặc định hệ thống ưu tiên mô hình cục bộ (on-premise) qua Ollama, llama.cpp hoặc vLLM khi điều kiện phần cứng cho phép. Chỉ khi tác vụ vượt quá khả năng xử lý của mô hình cục bộ (ví dụ: yêu cầu mô hình rất lớn, hoặc yêu cầu chất lượng cao vượt quá mô hình local) hoặc khi người dùng chủ động chọn, hệ thống mới gọi API thương mại. Nguyên tắc này trực tiếp giải quyết Nghịch lý thứ tư (an toàn dữ liệu) bằng cách đảm bảo dữ liệu nhạy cảm có thể được xử lý hoàn toàn trong hạ tầng của doanh nghiệp.

Nguyên tắc OFFLINE FIRST có hai hệ quả quan trọng. Thứ nhất, nó giảm chi phí vận hành đáng kể vì không phải trả phí API cho mọi truy vấn; chi phí chuyển thành chi phí mua thiết bị một lần (GPU) và điện năng. Thứ hai, nó hỗ trợ tuân thủ dữ liệu vì dữ liệu doanh nghiệp không bao giờ rời khỏi hạ tầng nội bộ - phù hợp với các ngành tài chính, y tế, và chính phủ. Provider Abstraction Layer ở Layer 3 được thiết kế để Ollama là một provider bình đẳng với OpenAI hay Anthropic, không phải một "lựa chọn dự phòng" mà là provider mặc định.

Trong thực tế, hệ thống cần cấu hình được mức độ ưu tiên offline/online. Một doanh nghiệp có GPU mạnh có thể đặt Ollama làm provider mặc định và chỉ fallback khi Ollama không khả dụng. Một doanh nghiệp khác có thể đặt Claude làm mặc định vì yêu cầu chất lượng cao và sẵn sàng chấp nhận chi phí API. Cấu hình này là per-tenant.

**(7) ENTERPRISE GRADE - Bảo mật, kiểm toán, tuân thủ**

Nguyên tắc ENTERPRISE GRADE đặt ra rằng hệ thống phải được thiết kế để đáp ứng các tiêu chuẩn doanh nghiệp: bảo mật đa tầng (multi-layer security), ghi log kiểm toán đầy đủ, phân quyền chi tiết theo vai trò và tài nguyên, khả năng truy vết sự cố, và tuân thủ các quy định pháp lý liên quan. Đây là nguyên tắc có mức ưu tiên cao nhất trong hệ thống 8 nguyên tắc.

Hiện thực của nguyên tắc này thể hiện trên nhiều tầng. Ở tầng dữ liệu (Layer 1), RLS đảm bảo cô lập dữ liệu ở mức hàng. Ở tầng API (Layer 6), AuthN/AuthZ kiểm soát ai có thể gọi API nào. Ở tầng nghiệp vụ (Layer 4, Layer 5), SQLGuard đảm bảo câu SQL sinh ra không truy cập bảng nhạy cảm. Ở tầng tích hợp (Layer 2), sandbox của plugin ngăn plugin truy cập tài nguyên không được phép. Audit log bất biến ghi lại mọi hành động quan trọng. Nguyên tắc này phục vụ NFR-04 (Security) và NFR-05 (Compliance & Audit).

**(8) MULTI-TENANT - Nhiều tenant cấu hình độc lập**

Nguyên tắc MULTI-TENANT yêu cầu một triển khai của nền tảng phải có khả năng phục vụ nhiều tenant đồng thời, mỗi tenant có cấu hình riêng về nhà cung cấp AI, plugin được cài, chính sách bảo mật, giới hạn sử dụng, và dữ liệu hoàn toàn cô lập. Nguyên tắc này phục vụ FR-11 và đồng thời là cơ sở cho mô hình kinh doanh SaaS của nền tảng.

Hiện thực của nguyên tắc này gồm: Tenant Context được truyền qua mọi tầng thông qua middleware; RLS ở tầng dữ liệu đảm bảo cô lập ở mức hàng; namespace cho plugin đảm bảo plugin của tenant A không nhìn thấy dữ liệu của tenant B; cơ chế rate limiting theo tenant ngăn một tenant chiếm toàn bộ tài nguyên; cơ chế quota tracking theo tenant để kiểm soát chi phí.

Tám nguyên tắc trên có đặc điểm chung là mang tính ràng buộc kiến trúc tổng thể, không phải là hướng dẫn chi tiết cho từng lớp. Trong quá trình thiết kế chi tiết, có những tình huống các nguyên tắc có thể xung đột nhẹ với nhau. Ví dụ, OFFLINE FIRST có thể xung đột với yêu cầu chất lượng khi mô hình cục bộ chưa đủ mạnh cho một số tác vụ phức tạp. Khi đó, các quyết định được đưa ra dựa trên thứ tự ưu tiên: an toàn dữ liệu và tuân thủ luôn được đặt lên hàng đầu (ENTERPRISE GRADE), tiếp đến là tính thuần nhất nhà cung cấp và khả năng mở rộng. Trình tự ưu tiên này được phản ánh trong Bảng 2.3 và được tuân thủ xuyên suốt báo cáo.

### 2.2.2. Kiến trúc 7 tầng (Layered Architecture)

Trên cơ sở 8 nguyên tắc thiết kế, đề tài đề xuất kiến trúc 7 tầng cho Enterprise AI Platform. Mỗi tầng có một tập trách nhiệm rõ ràng, một tập giao diện (interface) chuẩn để giao tiếp với tầng liền kề, và có khả năng được triển khai, kiểm thử, thay thế độc lập. Mô hình 7 tầng được chọn thay vì mô hình 3 tầng truyền thống (presentation, business logic, data) vì hai lý do: thứ nhất, độ phức tạp của một hệ thống AI doanh nghiệp đòi hỏi sự phân tách tinh tế hơn; thứ hai, cách phân tách 7 tầng đã được chứng minh qua thực tiễn nhiều nền tảng AI lớn (LangChain, Semantic Kernel, AutoGen). Sơ đồ tổng quát của kiến trúc được trình bày trong Hình 2.1.

**Hình 2.1. Sơ đồ tổng quát kiến trúc 7 tầng của Enterprise AI Platform**

```
┌──────────────────────────────────────────────────────────────────────┐
│ LAYER 7 - PRESENTATION                                              │
│   Web UI (Next.js) | Voice UI | Dashboard | API Explorer             │
├──────────────────────────────────────────────────────────────────────┤
│ LAYER 6 - API GATEWAY                                               │
│   AuthN/AuthZ | Rate Limiting | Routing | Load Balancing | Analytics │
├──────────────────────────────────────────────────────────────────────┤
│ LAYER 5 - AGENT ORCHESTRATION                                       │
│   Intent Router | Task Planner | Executor | Memory Mgmt | HITL      │
├──────────────────────────────────────────────────────────────────────┤
│ LAYER 4 - AI ENGINE CORE                                            │
│   RAG Engine | Hybrid Search | Tool Registry | Workflow |           │
│   SQL Engine | Voice Engine | Memory | Context Builder               │
├──────────────────────────────────────────────────────────────────────┤
│ LAYER 3 - PROVIDER ABSTRACTION                                      │
│   LLM Abstraction | Embedding Abstraction | Multi-Provider Router | │
│   Cache | Model Registry | Quota Tracker                             │
├──────────────────────────────────────────────────────────────────────┤
│ LAYER 2 - INTEGRATION                                               │
│   MCP Server/Client | Plugin SDK | REST/gRPC Gateway |              │
│   Message Queue | Event Bus                                          │
├──────────────────────────────────────────────────────────────────────┤
│ LAYER 1 - INFRASTRUCTURE                                            │
│   PostgreSQL+pgvector | Redis | MinIO/S3 | Ollama | Docker/K8s       │
└──────────────────────────────────────────────────────────────────────┘
```

**Layer 1 - Infrastructure**: Tầng hạ tầng vật lý và dịch vụ nền tảng. Trong phạm vi đề tài, các thành phần cốt lõi gồm: (i) PostgreSQL kết hợp extension **pgvector** cho lưu trữ quan hệ và vector - đây là lựa chọn then chốt vì cho phép truy vấn vector trong cùng một transaction với dữ liệu quan hệ, giảm đáng kể độ phức tạp vận hành so với việc tách thành hai hệ CSDL riêng biệt như Milvus hay Qdrant; (ii) Redis cho cache đa tầng (prompt cache, semantic cache, embedding cache), hàng đợi nhẹ (lightweight job queue), và session store; (iii) MinIO hoặc S3 cho lưu trữ đối tượng (file tài liệu gốc, audio, log, backup); (iv) Ollama, vLLM hoặc llama.cpp để chạy mô hình cục bộ, phục vụ nguyên tắc OFFLINE FIRST; (v) Docker và Kubernetes cho đóng gói và orchestration, hỗ trợ mở rộng theo chiều ngang. Một điểm đáng lưu ý là Layer 1 được thiết kế theo nguyên tắc "bạn có thể thay thế từng thành phần": nếu doanh nghiệp đã có PostgreSQL, họ có thể dùng PostgreSQL hiện có; nếu họ dùng AWS, họ có thể dùng S3 thay vì MinIO. Layer 1 chỉ định nghĩa interface (cần gì), không bắt buộc cài đặt cụ thể nào.

**Layer 2 - Integration**: Tầng tích hợp cung cấp các "cổng giao tiếp" với hệ thống bên ngoài. Các thành phần chính gồm: (i) MCP Server cho phép hệ thống khác (ứng dụng di động, phần mềm bên thứ ba) gọi tới nền tảng; (ii) MCP Client cho phép nền tảng gọi tới hệ thống khác (ERP, CRM, DMS, HRM); (iii) Plugin SDK để xây dựng và nạp plugin mở rộng; (iv) REST/gRPC Gateway cho tích hợp điểm-điểm truyền thống khi MCP không phù hợp; (v) Message Queue (Redis Streams hoặc Kafka) và Event Bus cho tích hợp theo cơ chế phát hành/đăng ký. Đây là tầng hiện thực trực tiếp nguyên tắc PLUG & PLAY và EVENT DRIVEN.

**Layer 3 - Provider Abstraction**: Tầng trừu tượng hóa các dịch vụ AI bên ngoài, là lõi kiến trúc của nguyên tắc PROVIDER AGNOSTIC và OFFLINE FIRST. Các thành phần gồm: (i) ILLMProvider - interface chuẩn cho mọi LLM; (ii) IEmbeddingProvider - interface chuẩn cho mô hình embedding; (iii) Multi-Provider Router để định tuyến dựa trên tiêu chí (chi phí, chất lượng, vị trí dữ liệu); (iv) Cache ở nhiều mức (prompt cache, semantic cache, embedding cache) giúp giảm số lần gọi API và cải thiện độ trễ; (v) Model Registry để quản lý phiên bản mô hình; (vi) Quota Tracker để theo dõi giới hạn sử dụng theo tenant/provider.

**Layer 4 - AI Engine Core**: Trái tim xử lý AI của nền tảng. Các thành phần gồm: (i) RAG Engine quản lý vòng đời tri thức (indexing, retrieval, generation); (ii) Search Engine hỗ trợ semantic search, sparse search và hybrid search; (iii) Tool Registry lưu danh sách công cụ có thể gọi; (iv) Workflow Engine điều phối tác vụ dài hạn; (v) SQL Engine chuyển ngôn ngữ tự nhiên thành SQL an toàn; (vi) Voice Engine cho STT/TTS; (vii) Memory cho hội thoại ngắn hạn và bộ nhớ người dùng dài hạn; (viii) Context Builder tổng hợp ngữ cảnh trước khi gọi LLM. Tầng này đáp ứng FR-01, FR-02, FR-03, FR-08, FR-09, FR-10.

**Layer 5 - Agent Orchestration**: Tầng điều phối tác nhân AI, hiện thực nguyên tắc AGENT NATIVE. Các thành phần gồm: (i) Intent Router phân tích và phân loại ý định; (ii) Task Planner lập kế hoạch tác vụ; (iii) Executor thực thi kế hoạch; (iv) Memory Management kết nối với Memory ở Layer 4; (v) Human-in-the-Loop cho phép chuyển sang người vận hành khi cần. Tầng này phục vụ FR-04, FR-05.

**Layer 6 - API Gateway**: Tầng cổng API, điểm vào duy nhất cho mọi request từ bên ngoài. Các thành phần gồm: (i) AuthN/AuthZ xác thực và phân quyền; (ii) Rate Limiting theo tenant và người dùng; (iii) Routing thông minh; (iv) Load Balancing; (v) Analytics thu thập metric. Tầng này đảm bảo NFR-02, NFR-03, NFR-04, NFR-07.

**Layer 7 - Presentation**: Tầng giao diện người dùng. Gồm: (i) Web UI (Next.js) với hai module chính: Chat Console cho hội thoại/RAG và Admin Dashboard cho quản trị; (ii) Voice UI cho tương tác giọng nói; (iii) API Explorer cho nhà phát triển tự kiểm thử API.

Ba đặc điểm quan trọng cần nhấn mạnh trong kiến trúc 7 tầng.

Đặc điểm thứ nhất là **một chiều phụ thuộc (downward dependency)**: tầng trên phụ thuộc tầng dưới nhưng chiều ngược lại không bắt buộc. Cụ thể, Layer 4 có thể gọi Layer 3, nhưng Layer 3 không nên gọi Layer 4. Layer 5 có thể gọi Layer 4, nhưng Layer 4 không nên gọi Layer 5. Quy tắc này đảm bảo khả năng thay thế từng tầng mà không phá vỡ các tầng khác, góp phần đáp ứng NFR-06 (Maintainability).

Đặc điểm thứ hai là **tách giao diện và hiện thực (interface-implementation separation)**: mọi giao tiếp giữa hai tầng đều đi qua một interface được định nghĩa tĩnh (C# interface hoặc giao thức truyền thông) và có thể có nhiều hiện thực khác nhau. ILLMProvider có hiện thực cho OpenAI, Anthropic, Gemini, Ollama; IEmbeddingProvider có hiện thực cho OpenAI embedding, Gemini embedding, Ollama embedding, BGE embedding. Cách tổ chức này cho phép thay thế nhà cung cấp mà không ảnh hưởng business logic, đồng thời hỗ trợ kiểm thử với mock implementation.

Đặc điểm thứ ba là **tích hợp chéo giữa các tầng (cross-cutting concerns)**: một số quan tâm xuyên suốt như Logging, Distributed Tracing, Security, Tenant Context, Error Handling, Retry Policy không được gắn vào một tầng cụ thể mà được xử lý ở mức middleware hoặc cross-cutting library, áp dụng tự động tại mọi entry point. Cách tổ chức này giúp tránh hiện tượng "code bị trùng lặp ở mọi nơi" và đảm bảo NFR-04, NFR-05, NFR-07 được nhất quán.

### 2.2.3. So sánh các phong cách kiến trúc

Một quyết định kiến trúc có ý nghĩa quyết định cho cả đề tài là lựa chọn phong cách kiến trúc tổng thể giữa ba ứng cử viên: Monolith, Modular Monolith và Microservice. Đây là quyết định ảnh hưởng đến mọi khía cạnh từ tổ chức mã nguồn, quy trình triển khai, đến chi phí vận hành và khả năng mở rộng. Trong phần này, đề tài thực hiện phân tích so sánh đa tiêu chí và đề xuất hướng đi phù hợp với bối cảnh nghiên cứu.

**Monolith (nguyên khối)** là kiểu kiến trúc trong đó toàn bộ hệ thống được đóng gói trong một đơn vị triển khai duy nhất. Tất cả các module, tầng, và chức năng nằm trong cùng một codebase và deploy như một khối thống nhất. Đây là kiểu kiến trúc truyền thống nhất, có ưu điểm là đơn giản trong giai đoạn đầu, dễ debug vì mọi thứ trong cùng một process, và chi phí vận hành thấp (một tiến trình duy nhất). Tuy nhiên, monolith truyền thống gặp hạn chế nghiêm trọng khi hệ thống phát triển lớn: một thay đổi nhỏ ở module RAG vẫn phải deploy lại toàn bộ hệ thống; toàn bộ hệ thống sập nếu một module bị lỗi nghiêm trọng; và không thể mở rộng riêng từng module theo nhu cầu thực tế.

**Modular Monolith (đơn thể phân module)** là kiểu kiến trúc trong đó hệ thống vẫn là một đơn vị triển khai duy nhất, nhưng được tổ chức rõ ràng thành các module với ranh giới chặt chẽ (bounded context), giao tiếp qua interface được công bố, và có thể phát triển, kiểm thử độc lập. Điểm khác biệt then chốt so với monolith truyền thống là tính kỷ luật về ranh giới module: không có gọi trực tiếp vào code bên trong của module khác, không dùng biến toàn cục chia sẻ, mọi giao tiếp đều qua interface hoặc event. Modular Monolith giữ được sự đơn giản vận hành của monolith (một process, một database, dễ debug) trong khi có tính module hóa cao, chuẩn bị sẵn sàng cho việc tách thành microservice khi cần.

**Microservice (đa dịch vụ)** là kiểu kiến trúc trong đó hệ thống được chia thành nhiều service nhỏ, độc lập, mỗi service có database riêng, có thể được phát triển, triển khai và mở rộng độc lập. Microservice mang lại khả năng mở rộng và cô lập lỗi tốt nhất, nhưng đòi hỏi chi phí vận hành cao hơn nhiều: cần infrastructure cho service orchestration (Kubernetes), service mesh (Istio, Linkerd), distributed tracing, API gateway chuyên dụng, và đội ngũ có kinh nghiệm về hệ thống phân tán.

**Bảng 2.3. So sánh ba phong cách kiến trúc theo các tiêu chí của đề tài**

| Tiêu chí | Monolith (nguyên khối) | Modular Monolith (đơn thể phân module) | Microservice (đa dịch vụ) |
|---|---|---|---|
| Độ phức tạp triển khai ban đầu | Thấp nhất (1 process, 1 DB) | Trung bình (1 process, boundary rõ) | Cao (nhiều service, cần infra) |
| Tốc độ phát triển ở giai đoạn đầu | Nhanh nhất (không cần infra) | Nhanh (infra đơn giản, tập trung logic) | Chậm (phải xây dựng infra trước) |
| Khả năng mở rộng theo module | Thấp (scale toàn bộ) | Trung bình (vẫn trong 1 process) | Cao (scale từng service riêng) |
| Khả năng cô lập lỗi (Fault Isolation) | Yếu (lỗi module A có thể sập cả hệ thống) | Khá (lỗi trong module được cô lập qua boundary, exception handler) | Tốt nhất (service chết không ảnh hưởng service khác) |
| Chi phí vận hành hạ tầng | Thấp nhất | Thấp đến trung bình | Cao (K8s, service mesh, monitoring) |
| Rào cản debug và distributed tracing | Thấp (trong cùng process, call stack đơn giản) | Trung bình (module rõ nhưng vẫn cùng process) | Cao (cần distributed tracing như OpenTelemetry, Jaeger) |
| Độ phức tạp của CI/CD | Thấp (1 pipeline) | Trung bình (1 pipeline, nhưng test per module) | Cao (n pipeline, cần release coordination) |
| Khả năng đáp ứng EVENT DRIVEN, PLUG & PLAY | Yếu (module ghép chặt, khó plug mới) | Tốt (module có boundary, internal event bus khả thi) | Rất tốt (event bus là bản chất của microservice) |
| Khả năng tiến hóa (Evolution) | Khó (refactor lớn nếu muốn tách) | Dễ (module có thể tách thành service khi cần) | Đã ở trạng thái phân tán, không cần tiến hóa |
| Yêu cầu về đội ngũ | Nhỏ (2-3 người) | Trung bình (3-5 người) | Lớn (5+ người, cần DevOps/SRE) |
| Phù hợp với ngữ cảnh nghiên cứu đề tài | Không - không đáp ứng yêu cầu module hóa và mở rộng | **Phù hợp nhất** - đáp ứng đủ module hóa với chi phí vừa phải | Phù hợp về kỹ thuật nhưng chi phí triển khai cao, vượt quy mô đồ án |

Trên cơ sở phân tích đa tiêu chí ở Bảng 2.3, đề tài đề xuất chiến lược **"Modular Monolith trước, Microservice sau"** (Incremental Decomposition). Cụ thể: giai đoạn đầu (Phase 1 đến Phase 4), toàn bộ hệ thống được triển khai như một khối Modular Monolith đơn nhất. Mỗi module được tổ chức theo bounded context rõ ràng, giao tiếp qua interface và internal event bus. Khi hệ thống ổn định và có dấu hiệu nghẽn cục bộ (ví dụ: RAG Engine có lưu lượng truy vấn cao hơn các module khác), tiến hành tách module đó thành service độc lập chạy trên container riêng. Chiến lược này vừa đảm bảo tốc độ phát triển ở giai đoạn đầu (ưu điểm của monolith), vừa không đóng cửa khả năng mở rộng theo chiều ngang (ưu điểm của microservice), vừa phù hợp với quy mô đồ án thạc sĩ (không đòi hỏi infrastructure quá phức tạp).

Để hiện thực chiến lược trên một cách nhất quán, đề tài áp dụng bốn nguyên tắc bổ sung. Thứ nhất, mỗi module được đóng gói trong một **Bounded Context** riêng theo Domain-Driven Design (DDD), giao tiếp với module khác chỉ qua các interface được công bố (published interface), không dùng internal class của module khác. Thứ hai, truyền thông liên module **không** dùng biến toàn cục hoặc gọi hàm trực tiếp vào code bên trong; thay vào đó dùng Application Service hoặc Event Bus nội bộ. Thứ ba, các module được đặt trong các namespace riêng, quy ước tên file theo module để dễ dàng tách sau này mà không cần refactor lớn. Thứ tư, mỗi module có bộ test riêng (unit test + integration test), tỷ lệ bao phủ test mức unit phải đạt tối thiểu 80%.

### 2.2.4. Communication Patterns giữa các module

Song song với quyết định về phong cách kiến trúc tổng thể, đề tài cũng xác định các communication pattern (mẫu giao tiếp) giữa các module trong nền tảng. Bốn pattern chính được sử dụng: REST, gRPC, GraphQL, và MCP. Việc lựa chọn pattern phụ thuộc vào bối cảnh sử dụng cụ thể, như trình bày trong Bảng 2.4.

**Bảng 2.4. So sánh các communication patterns**

| Tiêu chí | REST | gRPC | GraphQL | MCP |
|---|---|---|---|---|
| Định dạng dữ liệu | JSON | Protocol Buffers (binary) | JSON | JSON-RPC 2.0 |
| Streaming | Server-Sent Events (SSE) | Bidirectional streaming (Bidi-stream) | Subscriptions | Server-Sent Notifications |
| Trường hợp dùng chính | API public, tích hợp bên ngoài | Giao tiếp nội bộ high-performance | Truy vấn linh hoạt từ client | Tích hợp AI tool/function calling |
| Khám phá API | OpenAPI/Swagger | Protobuf + grpc-ui | Introspection built-in | Dynamic discovery protocol |
| Phù hợp trong đề tài | API Gateway, Plugin SDK | Inter-service (Module-to-Module) | Admin Dashboard | Agent-to-Tool |

Trong thiết kế chi tiết, đề tài sử dụng REST cho các endpoint public của API Gateway (để tương thích với mọi client), gRPC cho giao tiếp nội bộ giữa các module trong cùng một process hoặc khi cần tách service (hiệu suất cao, schema ràng buộc), GraphQL cho Admin Dashboard (truy vấn linh hoạt), và MCP cho giao tiếp Agent-to-Tool và tích hợp hệ thống bên ngoài (dynamic discovery, chuẩn hóa tool schema).

## 2.3. Thiết kế các module cốt lõi

Sau khi kiến trúc tổng thể được xác định ở mục 2.2, mục 2.3 đi sâu vào thiết kế chi tiết các module cốt lõi. Trong mỗi mục con, đề tài trình bày trách nhiệm, giao diện chính (interface), một số quyết định thiết kế quan trọng kèm code minh họa C#. Do toàn bộ hệ thống có khoảng 40 module con, chương này tập trung vào nhóm module thuộc Layer 3, Layer 4, Layer 5 và tích hợp Layer 2; các module phụ trợ khác (Notification Service, Feature Flag Service, Config Service) được mô tả ngắn gọn và tham chiếu đến phụ lục thiết kế chi tiết.

### 2.3.1. AI Provider Abstraction Layer

Layer 3 (Provider Abstraction) là tầng quyết định khả năng đáp ứng nguyên tắc PROVIDER AGNOSTIC và OFFLINE FIRST của toàn hệ thống. Nếu không có tầng này, business logic ở Layer 4 và Layer 5 sẽ bị gắn chặt với nhà cung cấp cụ thể, gây khó khăn cho việc chuyển đổi và mở rộng. Ở tầng này, mọi tương tác với mô hình AI bên ngoài đều phải đi qua một bộ interface chuẩn.

**ILLMProvider - Interface tổng quát cho mọi LLM**: Đây là interface cốt lõi nhất của toàn hệ thống. Mọi hiện thực cụ thể (OpenAI, Anthropic, Gemini, Ollama...) đều phải implement interface này. Tầng business chỉ phụ thuộc vào ILLMProvider, không phụ thuộc vào bất kỳ provider cụ thể nào.

```csharp
/// <summary>
/// Interface chuẩn cho mọi LLM provider.
/// Mọi hiện thực cụ thể (OpenAI, Anthropic, Gemini, Ollama...)
/// phải implement interface này.
/// Tầng business chỉ phụ thuộc vào ILLMProvider.
/// </summary>
public interface ILLMProvider
{
    /// <summary>
    /// Tên định danh provider (vd: "openai", "anthropic", "ollama-llama3").
    /// </summary>
    string Name { get; }
    
    /// <summary>
    /// Loại provider: Cloud, OnPremise, Local.
    /// Dùng để routing theo OFFLINE FIRST.
    /// </summary>
    ProviderLocation Location { get; }
    
    /// <summary>
    /// Thực hiện truy vấn hoàn chỉnh (completion) - bất đồng bộ.
    /// </summary>
    Task<LLMResponse> CompleteAsync(LLMRequest request, CancellationToken ct);
    
    /// <summary>
    /// Streaming token đầu ra theo thời gian thực.
    /// Dùng cho real-time chat interface.
    /// </summary>
    IAsyncEnumerable<LLMStreamToken> StreamAsync(LLMRequest request, CancellationToken ct);
    
    /// <summary>
    /// Liệt kê các model mà provider hỗ trợ.
    /// </summary>
    Task<IReadOnlyList<ModelInfo>> ListModelsAsync(CancellationToken ct);
    
    /// <summary>
    /// Kiểm tra provider có khả dụng không.
    /// </summary>
    Task<bool> HealthCheckAsync(CancellationToken ct);
}
```

**IEmbeddingProvider - Interface cho mô hình embedding**: Được tách riêng khỏi ILLMProvider vì nhiều hệ thống dùng embedding provider khác LLM provider. Ví dụ: dùng OpenAI embedding cho indexing nhưng dùng Anthropic Claude cho generation. Cách tách này mang lại sự linh hoạt tối đa.

```csharp
/// <summary>
/// Interface cho mô hình embedding.
/// Tách biệt khỏi ILLMProvider vì nhiều hệ thống dùng
/// embedding provider khác LLM provider.
(vd: OpenAI embedding + Anthropic LLM).
/// </summary>
public interface IEmbeddingProvider
{
    string Name { get; }
    ProviderLocation Location { get; }
    
    /// <summary>
    /// Số chiều vector của embedding (vd: 1536 cho text-embedding-3-small,
    /// 3072 cho text-embedding-3-large).
    /// </summary>
    int Dimensions { get; }
    
    /// <summary>
    /// Sinh embedding cho một batch văn bản.
    /// </summary>
    Task<float[][]> EmbedAsync(string[] texts, CancellationToken ct);
    
    Task<bool> HealthCheckAsync(CancellationToken ct);
}
```

Hai interface trên có vai trò "ổ cắm" (socket) để gắn vào nhiều loại nhà cung cấp khác nhau. Provider cụ thể (OpenAIProvider, AnthropicProvider, OllamaProvider, GeminiProvider, MistralProvider, VinaLlamaProvider...) sẽ implement các interface này. Điều quan trọng là khi thêm provider mới (ví dụ: một dịch vụ AI mới của nhà cung cấp nội địa), chỉ cần thêm một lớp hiện thực mới mà không phải sửa đổi bất kỳ module nào ở tầng business.

**Multi-Provider Router**: Trong thực tế, một truy vấn có thể được gửi đến provider khác nhau tùy theo tiêu chí. Multi-Provider Router chịu trách nhiệm quyết định "gửi truy vấn này đến đâu". Bốn chiến lược định tuyến được hỗ trợ:

- *Cost-based routing*: ưu tiên provider có giá rẻ nhất cho tác vụ thường (VD: gọi llama3 8B local cho hỏi đáp đơn giản, chỉ gọi GPT-4o khi cần chất lượng cao).
- *Quality-based routing*: ưu tiên provider có chất lượng cao nhất cho tác vụ phức tạp (VD: phân tích tài liệu pháp lý dùng Claude 3.5 Opus).
- *Location-based routing*: ưu tiên mô hình cục bộ (OFFLINE FIRST) khi bật; chỉ chuyển sang cloud khi local không khả dụng.
- *Fallback routing*: provider chính lỗi hoặc quota hết sẽ tự động chuyển sang provider dự phòng đã cấu hình, không ảnh hưởng trải nghiệm người dùng.

```csharp
public interface ILLMRouter
{
    /// <summary>
    /// Chọn provider phù hợp dựa trên routing strategy
    /// và routing context (tác vụ, độ phức tạp, ưu tiên offline...).
    /// </summary>
    Task<ILLMProvider> ResolveProviderAsync(RoutingContext ctx, CancellationToken ct);
    
    /// <summary>
    /// Gọi provider đã chọn, với cơ chế fallback tự động.
    /// </summary>
    Task<LLMResponse> RouteAndExecuteAsync(
        LLMRequest request, 
        RoutingStrategy strategy, 
        CancellationToken ct);
}
```

**Cache Layer**: Cache được đặt ngay sau Multi-Provider Router và trước khi gọi provider thực sự, hoạt động như một lớp trung gian giảm tải. Ba loại cache được hỗ trợ:

- *Prompt Cache*: cache dựa trên hash SHA-256 của toàn bộ prompt. Cùng prompt đầu vào tương đương sẽ trả về cùng response đã cache. Phù hợp cho các câu hỏi lặp lại.
- *Semantic Cache*: cache dựa trên độ tương đồng ngữ nghĩa của câu hỏi. Câu hỏi gần giống nhau (cosine similarity > threshold) sẽ dùng chung response đã cache. Phù hợp cho các biến thể của cùng một câu hỏi.
- *Embedding Cache*: cache kết quả embedding để tránh gọi lại API embedding cho cùng đoạn văn bản. Đặc biệt hữu ích khi cùng một tài liệu được embed nhiều lần (ví dụ: khi cập nhật metadata).

Cache sử dụng Redis làm backend với các key có TTL cấu hình được. Prompt cache có TTL mặc định 1 giờ; semantic cache có TTL mặc định 30 phút; embedding cache có TTL mặc định 7 ngày.

**Model Registry**: Là dịch vụ lưu trữ và cung cấp thông tin về các model đang được sử dụng trong hệ thống. Mỗi model record chứa: tên model, phiên bản, provider tương ứng, chi phí ước lượng (token/đơn vị tiền), giới hạn token đầu vào/đầu ra, các tham số mặc định (temperature, top_p, top_k). Model Registry có giao diện CRUD cho quản trị viên (qua Admin Dashboard) và giao diện truy vấn cho các module khác. Đây là nơi quản lý cấu hình "nên dùng model nào cho tác vụ nào" ở cấp toàn hệ thống.

```csharp
public class ModelRecord
{
    public string Id { get; set; }
    public string ProviderName { get; set; }
    public string ModelName { get; set; }        // vd: "gpt-4o"
    public string Description { get; set; }
    public int MaxInputTokens { get; set; }
    public int MaxOutputTokens { get; set; }
    public decimal CostPer1KInputTokens { get; set; }
    public decimal CostPer1KOutputTokens { get; set; }
    public List<string> Capabilities { get; set; } // "chat", "function_calling", "vision"
    public Dictionary<string, JsonElement> DefaultParams { get; set; }
    public bool IsEnabled { get; set; }
    public int Priority { get; set; }  // ưu tiên trong router
}
```

**Quota Tracker**: Chịu trách nhiệm theo dõi số lượng token đã tiêu thụ của từng tenant/người dùng/provider, áp dụng giới hạn (ngày/giờ/tháng) và cảnh báo khi gần đến ngưỡng. Mỗi khi một request được gửi qua Layer 3, Quota Tracker được gọi để kiểm tra và ghi nhận. Nếu quota của tenant đã hết, request bị từ chối với mã lỗi 429 (Too Many Requests) thay vì gọi provider. Quota Tracker cũng ghi metric về chi phí thực tế (dựa trên Model Registry) để hỗ trợ billing.

```csharp
public interface IQuotaTracker
{
    /// <summary>
    /// Kiểm tra xem tenant có quota còn lại không.
    /// </summary>
    Task<QuotaCheckResult> CheckAsync(string tenantId, int estimatedTokens, CancellationToken ct);
    
    /// <summary>
    /// Ghi nhận việc sử dụng quota sau khi request hoàn tất.
    /// </summary>
    Task RecordUsageAsync(string tenantId, string modelId, 
        int tokensIn, int tokensOut, CancellationToken ct);
    
    /// <summary>
    /// Lấy thống kê sử dụng quota của tenant.
    /// </summary>
    Task<QuotaSummary> GetSummaryAsync(string tenantId, Period period, CancellationToken ct);
}
```

Tổng hợp lại, thiết kế Layer 3 phản ánh một nguyên tắc xuyên suốt: **hệ thống không bao giờ gọi trực tiếp API của nhà cung cấp AI**. Mọi tương tác đều đi qua interface, router, cache, registry. Cách tổ chức này cho phép đề tài thử nghiệm nhiều chiến lược provider, đồng thời chuyển đổi provider không tốn công sức.

### 2.3.2. AI Engine Core – RAG Engine và Hybrid Search

Layer 4 (AI Engine Core) là nơi diễn ra phần lớn logic AI của hệ thống. Trong phạm vi chương này, mục 2.3.2 tập trung vào hai thành phần cốt lõi nhất: RAG Engine và Search Engine (Hybrid Search).

**RAG Engine**: Có trách nhiệm tổng quát về quản lý vòng đời tri thức doanh nghiệp gồm ba giai đoạn chính: indexing (upload, parse, chunk, embed, lưu trữ), retrieval (truy xuất theo truy vấn), và generation (sinh câu trả lời kết hợp ngữ cảnh được truy xuất với LLM). Quy trình indexing được minh họa trong Hình 2.2.

**Hình 2.2. Quy trình Indexing của RAG Engine**

```
┌──────────┐     ┌────────────┐     ┌────────────┐     ┌─────────────┐
│ Document │ --> │ Parser     │ --> │ Chunker    │ --> │ Embedder    │
│ Upload   │     │ (PDF/DOCX/│     │ (Recursive │     │ (text-      │
│          │     │  MD/HTML)  │     │  + sliding)│     │  embedding-3)│
└──────────┘     └────────────┘     └────────────┘     └──────┬──────┘
                                                                │
                                                                ▼
                                                       ┌─────────────┐
                                                       │ PostgreSQL  │
                                                       │ + pgvector  │
                                                       │ (chunks)    │
                                                       └─────────────┘
                                                                │
                                                       ┌─────────────┐
                                                       │ MinIO/S3    │
                                                       │ (raw files) │
                                                       └─────────────┘
```

Trong Hình 2.2, các giai đoạn chính gồm: (i) **Parser** trích xuất văn bản và metadata (tên file, tác giả, ngày tạo, phân quyền, tiêu đề, mục lục) từ định dạng gốc. Đề tài sử dụng thư viện mã nguồn mở để parse: PdfPig cho PDF, DocumentFormat.OpenXml cho DOCX, Markdig cho Markdown, HtmlAgilityPack cho HTML. (ii) **Chunker** chia văn bản thành các đoạn (chunk) với kích thước và độ trùng lặp có thể cấu hình. Đề tài sử dụng phương pháp Recursive Character Text Splitting với fallback theo cấu trúc (đoạn văn, câu) - đây là phương pháp phổ biến nhất trong các hệ thống RAG sản phẩm, được đánh giá hiệu quả hơn so với chunking cố định theo ký tự. Mặc định: chunk_size = 512 tokens, chunk_overlap = 64 tokens. (iii) **Embedder** chuyển mỗi đoạn thành vector thông qua IEmbeddingProvider (Layer 3). (iv) **Lưu trữ**: PostgreSQL + pgvector lưu vector kèm metadata trong bảng `chunks`; MinIO lưu file gốc để tái sử dụng khi cần truy vết nguồn.

Một đặc điểm thiết kế quan trọng của RAG Engine là hỗ trợ **incremental indexing**: khi một tài liệu được cập nhật (ví dụ: phiên bản mới của hợp đồng), hệ thống không cần xóa toàn bộ và index lại từ đầu mà chỉ cập nhật các chunk đã thay đổi. Cơ chế này sử dụng hash của nội dung chunk để phát hiện chunk nào cần cập nhật.

Quy trình truy vấn RAG (Retrieval-Augmented Generation) được minh họa trong Hình 2.3.

**Hình 2.3. Quy trình truy vấn RAG**

```
┌──────────┐     ┌───────────────┐     ┌──────────────┐     ┌────────────┐
│ Query    │ --> │ Query Rewrite │ --> │ Hybrid Search│ --> │ Reranking  │
│ (User)   │     │ (optional)    │     │ (Dense+Sparse│     │ (Cross-Enc)│
└──────────┘     └───────────────┘     │  + RRF)      │     └─────┬──────┘
                                          └──────────────┘           │
                                                                       ▼
┌─────────────┐    ┌──────────────┐    ┌─────────────────┐    ┌──────────────┐
│ Answer      │ <- │ LLM Generate │ <- │ Context         │ <- │ Top-K Chunks │
│ (Streaming) │    │ (with cit.)  │    │ Compression     │    │  (with meta) │
└─────────────┘    └──────────────┘    └─────────────────┘    └──────────────┘
```

Trong Hình 2.3, sau khi truy vấn người dùng được viết lại (Query Rewrite - tùy chọn, dùng để cải thiện chất lượng truy vấn bằng cách mở rộng từ đồng nghĩa, sửa lỗi chính tả), hệ thống thực hiện Hybrid Search kết hợp giữa dense retrieval (tìm kiếm vector, đo similarity) và sparse retrieval (tìm kiếm BM25, đánh giá theo tần suất từ), rồi hợp nhất kết quả theo thuật toán **Reciprocal Rank Fusion (RRF)**. Sau đó, Top K chunks được xếp lại bằng Cross-Encoder Reranker để cải thiện độ chính xác. Context Compression giảm kích thước ngữ cảnh nhưng vẫn giữ thông tin liên quan nhất (sử dụng LLM để tóm tắt hoặc lọc chunk ít liên quan). Cuối cùng, câu trả lời được sinh bởi LLM (Layer 3), kèm theo trích dẫn (citation) đến tài liệu nguồn để tăng độ tin cậy.

Đoạn mã C# dưới đây minh họa interface của RAG Engine ở mức trừu tượng, cho thấy cách tổ chức module với các phương thức rõ ràng:

```csharp
/// <summary>
/// Interface của RAG Engine.
/// Quản lý vòng đời tri thức: indexing, retrieval, generation.
/// </summary>
public interface IRagEngine
{
    /// <summary>
    /// Index một tài liệu mới hoặc cập nhật tài liệu đã tồn tại.
    /// </summary>
    Task<IndexResult> IndexDocumentAsync(IndexRequest request, CancellationToken ct);
    
    /// <summary>
    /// Truy vấn kết hợp retrieval + generation (non-streaming).
    /// </summary>
    Task<RagResponse> QueryAsync(RagQuery query, CancellationToken ct);
    
    /// <summary>
    /// Phiên bản streaming: trả về từng phần câu trả lời
    /// kèm citation khi có. Dùng cho real-time chat.
    /// </summary>
    IAsyncEnumerable<RagStreamChunk> QueryStreamAsync(RagQuery query, CancellationToken ct);
    
    /// <summary>
    /// Xóa tài liệu khỏi index (soft delete trước, sau đó garbage collect).
    /// </summary>
    Task DeleteDocumentAsync(string tenantId, string documentId, CancellationToken ct);
    
    /// <summary>
    /// Lấy trạng thái index của một tài liệu.
    /// </summary>
    Task<DocumentIndexStatus> GetIndexStatusAsync(string tenantId, string documentId, CancellationToken ct);
}
```

**Hybrid Search Engine**: Search Engine trong Layer 4 cung cấp cả ba chế độ tìm kiếm: dense (chỉ vector similarity), sparse (chỉ BM25), và hybrid (kết hợp cả hai). Trong cấu hình mặc định, chế độ hybrid được bật để tận dụng ưu điểm bổ trợ của cả hai phương pháp: dense search giỏi tìm theo ngữ nghĩa (cùng nghĩa, từ khác), trong khi sparse search giỏi tìm theo từ khóa chính xác (mã sản phẩm, số hợp đồng, tên riêng). Giao diện chính:

```csharp
public interface ISearchEngine
{
    /// <summary>
    /// Tìm kiếm với chế độ Dense, Sparse hoặc Hybrid.
    /// </summary>
    Task<SearchResult> SearchAsync(SearchQuery query, CancellationToken ct);
}

/// <summary>
/// Query tìm kiếm.
/// </summary>
public class SearchQuery
{
    public string TenantId { get; set; }
    public string Text { get; set; }                  // truy vấn văn bản
    public SearchMode Mode { get; set; }             // Dense | Sparse | Hybrid
    public int TopK { get; set; } = 10;              // số kết quả trả về
    public double RrfK { get; set; } = 60;          // tham số k cho RRF
    public Dictionary<string, object> Filters { get; set; }  // lọc theo metadata
    public string[] EmbeddingModel { get; set; }      // model embed (nếu cần override)
}

public class SearchResult
{
    public IReadOnlyList<SearchHit> Hits { get; init; }
    public string TraceId { get; init; }             // cho debug và audit
    public long QueryTimeMs { get; init; }
    public SearchMode ModeUsed { get; init; }
}

public class SearchHit
{
    public string ChunkId { get; init; }
    public string DocumentId { get; init; }
    public string Content { get; init; }              // nội dung chunk
    public double Score { get; init; }                // điểm RRF tổng hợp
    public double? VectorScore { get; init; }        // điểm vector (dense)
    public double? Bm25Score { get; init; }           // điểm BM25 (sparse)
    public Dictionary<string, object> Metadata { get; init; }
    public string DocumentName { get; init; }
    public string PageReference { get; init; }        // tham chiếu trang (nếu có)
}
```

Thuật toán **Reciprocal Rank Fusion (RRF)** được sử dụng để hợp nhất kết quả từ dense và sparse retrieval. Công thức RRF:

$$RRF_{score}(d) = \sum_{i=1}^{k} \frac{1}{k + rank_i(d)}$$

Trong đó $rank_i(d)$ là thứ hạng của document $d$ trong danh sách kết quả thứ $i$. Tham số $k$ (mặc định 60) kiểm soát mức độ ưu tiên của thứ hạng cao so với thứ hạng thấp. Giá trị $k$ càng lớn, hai danh sách kết quả có trọng số càng gần nhau; giá trị $k$ nhỏ thì ưu tiên kết quả xuất hiện ở thứ hạng cao trong cả hai danh sách.

**SQL Engine**: Có khả năng chuyển câu hỏi ngôn ngữ tự nhiên thành câu truy vấn SQL trên cơ sở dữ liệu quan hệ của doanh nghiệp. Đây là thành phần quan trọng để tích hợp với ERP, CRM, HRM. Tuy nhiên, do yêu cầu an toàn cao (Nghịch lý thứ tư), mọi câu SQL sinh ra đều phải đi qua lớp **SQLGuard** có các ràng buộc: (i) chỉ cho phép SELECT, không INSERT/UPDATE/DELETE/DROP/TRUNCATE; (ii) tự động thêm LIMIT nếu thiếu (mặc định 100 dòng); (iii) cấm truy cập vào bảng nhạy cảm (audit_log, credentials, api_keys, internal_config); (iv) kiểm tra cú pháp qua PostgreSQL parser (Npgsql) trước khi thực thi; (v) tất cả tên bảng/cột phải được escape để tránh SQL injection.

**Tool Registry**: Danh sách các "công cụ" (tools) mà Agent có thể gọi. Mỗi tool có: tên định danh, mô tả (được đưa vào system prompt để LLM hiểu khi nào nên gọi), JSON Schema mô tả tham số, và implementation handler. Tool Registry được nạp động từ các Plugin và MCP Server - khi plugin mới được cài, tool của plugin tự động xuất hiện trong Tool Registry mà không cần restart hệ thống.

```csharp
public interface IToolRegistry
{
    void Register(ToolDefinition definition);
    void Unregister(string toolName);
    IReadOnlyList<ToolDefinition> GetAll();
    ToolDefinition? Get(string toolName);
    bool Exists(string toolName);
}
```

**Workflow Engine**: Phục vụ các tác vụ dài hạn, có thể cần duyệt nhiều bước qua nhiều ngày. Workflow Engine có các đặc điểm: (i) định nghĩa quy trình theo BPMN-lite (đủ cho các quy trình phổ biến, không cần full BPMN engine); (ii) trạng thái workflow được lưu vào PostgreSQL, đảm bảo durability; (iii) hỗ trợ callback timer cho các bước chờ theo thời gian; (iv) hỗ trợ Human-in-the-Loop (HITL) cho phép tạm dừng và chờ phê duyệt. Workflow Engine sử dụng Event Bus ở Layer 2 để giao tiếp với các module khác.

**Voice Engine**: Hỗ trợ Speech-to-Text (STT) và Text-to-Speech (TTS), đặc biệt ưu tiên giọng tiếng Việt. Voice Engine có khả năng chuyển đổi linh hoạt giữa: (i) Whisper.cpp cho STT offline - mô hình mã nguồn mở, chạy cục bộ; (ii) Vosk cho STT offline tiếng Việt; (iii) Azure Speech, Google Speech cho STT cloud khi cần độ chính x�ng cao hơn. Tương tự với TTS: (i) Coqui TTS cho TTS offline tiếng Việt; (ii) Azure TTS, Google TTS cho TTS cloud.

**Memory Service**: Lưu trữ theo hai mức rõ ràng: (i) **Short-term Memory** (bộ nhớ ngắn hạn) trong phiên hội thoại, được lưu trong Redis với TTL theo session (mặc định 30 phút sau lần tương tác cuối); (ii) **Long-term Memory** (bộ nhớ dài hạn) cho thông tin người dùng, preferences, và lịch sử tương tác quan trọng, được lưu trong PostgreSQL. Memory Service có giao diện đồng nhất cho cả hai tầng:

```csharp
public interface IMemoryService
{
    // Short-term: đọc/ghi trong phiên
    Task<List<MemoryEntry>> GetConversationMemoryAsync(string sessionId, int lastN, CancellationToken ct);
    Task AppendAsync(string sessionId, MemoryEntry entry, CancellationToken ct);
    
    // Long-term: đọc/ghi thông tin người dùng
    Task<Dictionary<string, object>> GetUserMemoryAsync(string tenantId, string userId, CancellationToken ct);
    Task SetUserFactAsync(string tenantId, string userId, string key, object value, CancellationToken ct);
    
    // Summary: tổng hợp long-term memory thành context cho LLM
    Task<string> SummarizeAsync(string tenantId, string userId, CancellationToken ct);
}
```

**Context Builder**: Thành phần có nhiệm vụ tổng hợp tất cả ngữ cảnh cần thiết (hội thoại gần đây, memory, retrieved chunks, domain context, system prompt) thành một prompt hoàn chỉnh trước khi gọi LLM. Context Builder có cơ chế nén ngữ cảnh (context compression) khi tổng kích thước vượt quá giới hạn token của model, đảm bảo phần quan trọng nhất (retrieved chunks có điểm cao nhất, tin nhắn gần nhất) được giữ lại.

### 2.3.3. Agent Orchestration Layer

Layer 5 (Agent Orchestration) là nơi các yêu cầu FR-04 (Intent Routing), FR-05 (Multi-step Agent) được hiện thực, và là hiện thực cốt lõi của nguyên tắc AGENT NATIVE. Đặc trưng thiết kế của tầng này là mọi thành phần đều có interface chuẩn, có thể cấu hình động, và có thể thay thế bằng hiện thực khác.

**Intent Router**: Tiếp nhận câu hỏi người dùng (kèm ngữ cảnh phiên) và phân loại ý định. Đây là bước đầu tiên trong pipeline xử lý agent. Có ba loại ý định chính được xác định:

- *Tra cứu (Lookup)*: câu hỏi tìm kiếm thông tin đơn giản, được chuyển sang RAG Engine ở Layer 4.
- *Truy vấn dữ liệu (DataQuery)*: câu hỏi yêu cầu dữ liệu cụ thể từ hệ thống, được chuyển sang SQL Engine.
- *Tác vụ đa bước (Automation)*: câu hỏi yêu cầu thực hiện một quy trình phức tạp, được chuyển sang Task Planner.

Phân loại ý định được thực hiện bằng prompt-based classification với few-shot examples, hoặc bằng một mô hình phân loại nhỏ gọn (distilled classifier). Thiết kế cho phép cấu hình rule-based routing (nếu câu hỏi chứa từ khóa X thì route đến handler Y) song song với ML-based routing, cho phép ưu tiên rule-based cho các use case đã biết rõ và ML-based cho các trường hợp mới.

```csharp
public interface IIntentRouter
{
    /// <summary>
    /// Phân loại ý định của input người dùng.
    /// </summary>
    Task<IntentClassification> ClassifyAsync(IntentInput input, CancellationToken ct);
}

public class IntentClassification
{
    public IntentType Type { get; init; }        // Lookup | DataQuery | Automation
    public double Confidence { get; init; }      // độ tin cậy (0-1)
    public string? SuggestedHandler { get; init; } // handler cụ thể gợi ý
    public Dictionary<string, object> Metadata { get; init; }
    public string Reasoning { get; init; }       // giải thích tại sao chọn loại này
}
```

**Task Planner**: Với các tác vụ đa bước, Task Planner sinh ra một kế hoạch thực thi dưới dạng chuỗi bước (plan steps). Mỗi bước mô tả: tool cần gọi, tham số đầu vào, điều kiện để bước tiếp theo chạy, và rollback action nếu bước thất bại. Đề tài hỗ trợ ba chiến lược lập kế hoạch:

- *ReAct (Reason + Act)*: lặp lại Reason-Act cho đến khi đạt mục tiêu hoặc đạt số bước tối đa. Phù hợp cho tác vụ có lộ trình không xác định trước.
- *Plan-and-Execute*: lên kế hoạch toàn bộ một lần, sau đó thực thi tuần tự (plan-for-execution) hoặc song song (plan-for-parallel-execution). Phù hợp cho tác vụ có nhiều bước độc lập.
- *Tree-of-Thought* (mở rộng): khám phá nhiều nhánh kế hoạch cùng lúc, đánh giá mỗi nhánh bằng LLM, chọn nhánh có đánh giá tốt nhất. Phù hợp cho tác vụ phức tạp đòi hỏi lựa chọn chiến lược.

```csharp
public interface ITaskPlanner
{
    Task<ExecutionPlan> CreatePlanAsync(
        string goal, 
        IReadOnlyList<ToolDefinition> availableTools, 
        PlanningStrategy strategy, 
        PlanConstraints constraints,
        CancellationToken ct);
}

public class ExecutionPlan
{
    public string PlanId { get; init; }
    public string Goal { get; init; }
    public PlanningStrategy StrategyUsed { get; init; }
    public IReadOnlyList<PlanStep> Steps { get; init; }
    public string Reasoning { get; init; }  // giải thích tại sao chọn kế hoạch này
}

public class PlanStep
{
    public int StepId { get; init; }
    public string ToolName { get; init; }
    public Dictionary<string, object> Arguments { get; init; }
    public string ExpectedOutcome { get; init; }
    public PlanStep[]? ConditionalNextSteps { get; init; } // nhánh có điều kiện
    public string? RollbackAction { get; init; }
}
```

**Executor**: Chịu trách nhiệm thực thi các bước trong kế hoạch. Executor giao tiếp với Tool Registry, gọi LLM khi cần ra quyết định, xử lý lỗi, áp dụng retry/backoff khi cần. Một Executor có thể được cấu hình với các ràng buộc an toàn: giới hạn số bước tối đa (mặc định 20), ngân sách token tối đa (mặc định 8000 tokens), kích thước context tối đa, timeout cho mỗi bước. Các ràng buộc này giúp tránh hiện tượng agent lặp vô hạn (infinite loop).

```csharp
public interface IAgentExecutor
{
    Task<ExecutionResult> ExecutePlanAsync(
        ExecutionPlan plan,
        ExecutorConfig config,
        CancellationToken ct);
}

public class ExecutionResult
{
    public string FinalAnswer { get; init; }
    public IReadOnlyList<ExecutedStep> ExecutedSteps { get; init; }
    public string TraceId { get; init; }
    public bool RequiresHumanApproval { get; init; }
    public string? ApprovalRequest { get; init; }
    public ExecutionStatus Status { get; init; } // Success | Failed | NeedsApproval | MaxStepsReached
}
```

**Memory Management**: Kết nối Executor với Memory Service ở Layer 4. Mỗi agent có thể đọc/ghi vào short-term memory trong suốt phiên, và tổng hợp các sự kiện quan trọng thành long-term memory sau khi phiên kết thúc. Memory Management cũng chịu trách nhiệm quản lý context window: khi tổng ngữ cảnh gần đạt giới hạn của model, nó tự động tóm tắt các bước cũ thành một đoạn summary.

**Human-in-the-Loop (HITL)**: Cho phép một số bước của kế hoạch yêu cầu người vận hành xác nhận trước khi tiếp tục. Đây là cơ chế quan trọng để đảm bảo an toàn trong các tác vụ có rủi ro cao. Ví dụ: "Tôi đã chuẩn bị lệnh xóa 1.500 bản ghi trong hệ thống ERP. Bạn có chắc chắn muốn tiếp tục không?". HITL sử dụng kết hợp Notification Service (gửi thông báo đến người phê duyệt) và một hàng đợi phê duyệt (approval queue) trong database.

Hình 2.4 dưới đây minh họa luồng xử lý chính trong Agent Orchestration, từ khi nhận input cho đến khi trả về kết quả.

**Hình 2.4. Luồng xử lý trong Agent Orchestration**

```
┌──────────────┐
│ User Input   │
└──────┬───────┘
       ▼
┌──────────────┐    ┌─────────────────┐
│ Intent       │ -> │ Phân loại ý định│
│ Router       │    │ (classifier)    │
└──────┬───────┘    └─────────────────┘
       │
       ├─ Lookup ──────────────────> RAG Engine
       │
       ├─ DataQuery ────────────────> SQL Engine
       │
       └─ Automation:
              │
              ▼
        ┌──────────────┐    ┌─────────────────┐
        │ Task Planner │ -> │ Sinh ExecutionPlan │
        └──────┬───────┘    └─────────────────┘
               ▼
        ┌──────────────┐    ┌─────────────────┐
        │ Executor     │ -> │ Gọi tool, LLM,  │
        │ + Memory     │    │ hợp nhất kết quả│
        └──────┬───────┘    └─────────────────┘
               │
        ┌──────┴──────┐
        │             │
    Thành công     Cần duyệt?
        │             │
        ▼             ▼
┌────────────┐  ┌─────────────────┐
│ Kết quả    │  │ Human-in-the-   │
│ FinalAnswer│  │ Loop approval   │
└────────────┘  └─────────────────┘
```

Một điểm đáng chú ý trong thiết kế là khả năng kết nối giữa Intent Router và Task Planner có thể được cấu hình động. Ví dụ: với một số use case đã được rule-based rõ ràng, hệ thống có thể bỏ qua bước phân loại ý định và chuyển thẳng sang Plan-and-Execute. Với use case khác, có thể ép luôn sử dụng ReAct thay vì Tree-of-Thought để giảm chi phí. Cơ chế cấu hình này giúp tối ưu độ trễ và chi phí cho từng loại tác vụ cụ thể.

### 2.3.4. AI Gateway Layer

Layer 6 (API Gateway) là cổng giao tiếp giữa mọi client bên ngoài (ứng dụng web, mobile, tích hợp từ hệ thống khác) và các dịch vụ nội bộ. Gateway đóng vai trò như "lễ tân" của hệ thống, tiếp nhận mọi request, thực hiện các kiểm tra bảo mật và định tuyến đến service phù hợp. Đây là tầng duy nhất mà client bên ngoài được phép giao tiếp trực tiếp.

Gateway có năm trách nhiệm chính. **Thứ nhất, Authentication (AuthN)**: xác thực người gọi thông qua JWT/OAuth2, kiểm tra token còn hạn và chữ ký hợp lệ. Đối với service-to-service, sử dụng mTLS hoặc API key ký HMAC. **Thứ hai, Authorization (AuthZ)**: kiểm tra quyền truy cập dựa trên vai trò (RBAC) và ngữ cảnh (resource-based policy). Một request có thể được phép hoặc từ chối tùy theo tenant, vai trò, và resource cụ thể. **Thứ ba, Rate Limiting**: giới hạn số request trong khoảng thời gian, theo tenant và theo người dùng. Đề tài sử dụng thuật toán Token Bucket cho rate limiting. **Thứ tư, Routing**: định tuyến request đến instance service phù hợp, hỗ trợ weighted routing cho A/B testing và canary deployment. **Thứ năm, Analytics**: thu thập metric (latency, status code, số token, số lỗi, provider distribution) và đẩy về hệ thống giám sát (Prometheus/Grafana).

Đoạn mã C# dưới đây minh họa khung middleware cho Gateway trong ASP.NET Core:

```csharp
/// <summary>
/// Middleware xử lý các bước bảo mật chung cho mọi request.
/// Được đăng ký ở đầu pipeline ASP.NET Core.
/// </summary>
public class GatewayMiddleware
{
    public async Task InvokeAsync(
        HttpContext ctx, 
        ITenantResolver tenantResolver,
        ITokenBucketLimiter limiter,
        IPolicyEnforcer policy,
        IMetricsCollector metrics)
    {
        using var timer = metrics.StartRequestTimer(ctx.Request.Path);
        
        // 1. Xác thực: kiểm tra JWT hoặc API key
        var claims = await AuthenticateAsync(ctx);
        if (claims is null) 
        { 
            ctx.Response.StatusCode = 401; 
            return; 
        }
        
        // 2. Xác định tenant: gắn tenant ID vào context
        var tenantId = await tenantResolver.ResolveAsync(claims, ctx);
        if (tenantId is null) 
        { 
            ctx.Response.StatusCode = 403; 
            return; 
        }
        
        // 3. Rate limiting: kiểm tra quota
        var rateLimitKey = $"{tenantId}:{claims.UserId}";
        if (!await limiter.TryAcquireAsync(rateLimitKey))
        {
            ctx.Response.StatusCode = 429;
            await ctx.Response.WriteAsJsonAsync(new { error = "Rate limit exceeded" });
            return;
        }
        
        // 4. Phân quyền: kiểm tra có quyền gọi endpoint này không
        if (!await policy.IsAllowedAsync(ctx.Request.Path, ctx.Request.Method, claims))
        {
            ctx.Response.StatusCode = 403;
            return;
        }
        
        // 5. Gắn context: đưa tenant, claims vào HttpContext.Items
        // để các tầng dưới có thể truy cập
        ctx.Items["TenantId"] = tenantId;
        ctx.Items["Claims"] = claims;
        ctx.Items["TraceId"] = ctx.TraceIdentifier;
        
        await _next(ctx);
    }
}
```

Gateway không chỉ xử lý HTTP REST mà còn phối hợp với Adapter cho: (i) WebSocket cho streaming response thời gian thực; (ii) gRPC cho inter-service communication; (iii) MCP-internal gateway cho tích hợp MCP. Các endpoint nội bộ (ví dụ: `/internal/v1/agents/...`) được tách riêng khỏi endpoint public bằng separate middleware pipeline với policy khác nhau. Điều này đảm bảo rằng chỉ authenticated request mới được chuyển tiếp vào nội bộ.

Một đặc điểm thiết kế quan trọng của Gateway là **caching ở tầng này**: các response có thể cache được (ví dụ: kết quả truy vấn metadata của tenant, cấu hình model) được cache tại đây thay vì để Layer 4 xử lý. Điều này giúp giảm tải cho các tầng phía sau và cải thiện latency cho các request phổ biến.

### 2.3.5. Integration Layer – MCP và Plugin SDK

Layer 2 chứa hai thành phần quan trọng nhất để hiện thực nguyên tắc PLUG & PLAY: MCP (Model Context Protocol) và Plugin SDK. Phần này trình bày chi tiết hơn về cách hai thành phần này được thiết kế và tích hợp.

**MCP (Model Context Protocol)**: Giao thức chuẩn mở do Anthropic khởi xướng, cho phép AI Agent kết nối với các công cụ và nguồn dữ liệu một cách thống nhất thông qua một giao thức chung. Đề tài xem MCP là giao thức tích hợp chính vì ba lý do thực tiễn: (i) MCP đã trở thành chuẩn de-facto trong cộng đồng AI (nhiều nhà cung cấp đã hỗ trợ); (ii) MCP cho phép chia sẻ công cụ giữa nhiều nền tảng AI mà không cần viết adapter riêng cho từng nền tảng; (iii) MCP hỗ trợ khám phá động (dynamic discovery) các tool - agent có thể liệt kê tất cả tool khả dụng mà không cần biết trước schema. Trong kiến trúc, nền tảng vừa đóng vai trò MCP Client (gọi tới MCP server bên ngoài để truy xuất tool từ ERP, CRM...), vừa đóng vai trò MCP Server (cho phép hệ thống bên ngoài gọi tới nền tảng).

```csharp
/// <summary>
/// Interface MCP Client - cho phép nền tảng gọi tới MCP server bên ngoài.
/// </summary>
public interface IMcpClient
{
    /// <summary>
    /// Khám phá server và lấy thông tin server.
    /// </summary>
    Task<McpServerInfo> DiscoverAsync(string endpoint, CancellationToken ct);
    
    /// <summary>
    /// Liệt kê tất cả tool mà server cung cấp.
    /// </summary>
    Task<McpToolInfo[]> ListToolsAsync(string endpoint, CancellationToken ct);
    
    /// <summary>
    /// Gọi một tool cụ thể.
    /// </summary>
    Task<McpToolResult> CallToolAsync(
        string endpoint, 
        string toolName, 
        JsonElement arguments, 
        CancellationToken ct);
    
    /// <summary>
    /// Streaming response từ tool (cho tool chạy lâu).
    /// </summary>
    IAsyncEnumerable<McpToolStreamEvent> StreamToolAsync(
        string endpoint, 
        string toolName, 
        JsonElement arguments, 
        CancellationToken ct);
}

/// <summary>
/// Interface MCP Server - cho phép hệ thống bên ngoài gọi tới nền tảng.
/// </summary>
public interface IMcpServer
{
    /// <summary>
    /// Đăng ký một tool mới.
    /// </summary>
    void RegisterTool(McpToolDefinition tool);
    
    /// <summary>
    /// Đăng ký một resource.
    /// </summary>
    void RegisterResource(McpResourceDefinition resource);
    
    /// <summary>
    /// Đăng ký một prompt mẫu.
    /// </summary>
    void RegisterPrompt(McpPromptDefinition prompt);
}
```

**Thiết kế kết nối MCP**: Hệ thống lưu trữ danh sách MCP server trong bảng `mcp_servers`. Mỗi server có: tên định danh, endpoint URL, mode kết nối (stdio cho tích hợp cục bộ, HTTP+SSE cho tích hợp qua mạng), thông tin xác thực (API key hoặc Bearer token), whitelist các tool/resource được phép sử dụng (chính sách an ninh), và trạng thái (enabled/disabled). Trong quá trình thực thi Agent, MCP Client sẽ hợp nhất tool từ tất cả server đã bật, kiểm tra whitelist, và đăng ký vào Tool Registry ở Layer 4. Cách làm này đảm bảo rằng khi một MCP server mới được thêm (ví dụ: MCP server cho hệ thống HRM mới), agent tự động "thấy" các tool của nó mà không cần thay đổi code.

**Giao thức truyền thông**: Hỗ trợ cả ba phương thức được MCP định nghĩa: (i) stdio cho tích hợp cục bộ (MCP server chạy cùng máy với nền tảng, giao tiếp qua standard input/output - phù hợp cho các tool cần tốc độ cao và bảo mật); (ii) HTTP+SSE cho tích hợp qua mạng (phần lớn tình huống tích hợp ERP, CRM); (iii) Streamable HTTP cho các use case yêu cầu streaming hiệu năng cao. Đề tài sử dụng HTTP+SSE làm mặc định và Streamable HTTP cho các trường hợp đặc biệt.

**Plugin SDK**: Plugin SDK cho phép mở rộng hệ thống mà không cần sửa mã nguồn core. Một plugin có thể đăng ký: tool mới (mở rộng khả năng của Agent), intent handler mới (thêm loại ý định mới), hook vào các sự kiện nội bộ (theo dõi document upload, session start/end...), hoặc thậm chí provider AI mới. Plugin được đóng gói dưới dạng assembly .NET (file .dll) với manifest JSON mô tả metadata.

```csharp
/// <summary>
/// Interface gốc mà mọi plugin phải implement.
/// </summary>
public interface IEnterpriseAiPlugin
{
    string PluginId { get; }
    string Version { get; }
    string Description { get; }
    
    /// <summary>
    /// Được gọi khi plugin được nạp.
    /// Đăng ký services, tools, event handlers.
    /// </summary>
    void Register(IServiceCollection services, IPluginContext context);
    
    /// <summary>
    /// Lifecycle hooks.
    /// </summary>
    Task OnEnableAsync(CancellationToken ct);
    Task OnDisableAsync(CancellationToken ct);
}

/// <summary>
/// Ví dụ plugin kết nối ERP Acme.
/// </summary>
[EnterpriseAiPlugin("acme.erp.connector", Version = "1.2.0", 
    MinCoreVersion = "1.0", MaxCoreVersion = "2.0")]
public class AcmeErpConnector : IEnterpriseAiPlugin
{
    public void Register(IServiceCollection services, IPluginContext ctx)
    {
        // 1. Đăng ký HTTP client cho ERP
        services.AddHttpClient<IAcmeErpClient, AcmeErpClient>(client =>
        {
            client.BaseAddress = new Uri(ctx.Config.Get("erp.acme.baseUrl"));
            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", 
                    ctx.Config.Get("erp.acme.apiKey"));
        });
        
        // 2. Đăng ký tool cho Agent
        ctx.Tools.Register(new ToolDefinition
        {
            Name = "erp.get_invoice",
            Description = "Tra cứu hóa đơn theo mã số trong hệ thống ERP Acme.",
            ParametersSchema = new
            {
                type = "object",
                properties = new
                {
                    invoiceNumber = new { type = "string", description = "Mã hóa đơn" }
                },
                required = new[] { "invoiceNumber" }
            },
            Handler = async (args, ct) => 
            {
                var client = ctx.Services.GetRequiredService<IAcmeErpClient>();
                return await client.GetInvoiceAsync(args.invoiceNumber, ct);
            }
        });
        
        // 3. Đăng ký event handler
        ctx.Events.Subscribe<DocumentUploaded>(async e =>
        {
            // Đồng bộ tài liệu mới vào ERP
            if (e.Domain == "finance")
            {
                var client = ctx.Services.GetRequiredService<IAcmeErpClient>();
                await client.SyncDocumentAsync(e.DocumentId, ct);
            }
        });
    }
    
    public async Task OnEnableAsync(CancellationToken ct) { ... }
    public async Task OnDisableAsync(CancellationToken ct) { ... }
}
```

**Cơ chế Hot-reload**: Đây là đặc điểm quan trọng của Plugin SDK, cho phép nạp/gỡ plugin khi hệ thống đang chạy mà không cần restart. Cơ chế được đảm bảo bởi bốn thành phần:

- **AssemblyLoadContext riêng cho mỗi plugin**: mỗi plugin được nạp vào một ALC (Assembly Load Context) riêng biệt, tránh xung đột assembly giữa các plugin và giữa plugin với core. Khi unload plugin, ALC được giải phóng hoàn toàn.
- **File System Watcher**: hệ thống giám sát thư mục `/plugins` và tự động phát hiện file mới hoặc file thay đổi. Khi có thay đổi, hệ thống unload plugin cũ, nạp phiên bản mới.
- **Lifecycle hooks**: plugin có các phương thức `OnEnable` và `OnDisable` được gọi đúng thời điểm trong vòng đời, cho phép plugin cleanup tài nguyên, đóng kết nối, lưu trạng thái trước khi unload.
- **Sandbox giới hạn**: plugin chỉ được phép gọi các API do nền tảng công khai qua `IPluginContext`. Plugin không có quyền truy cập trực tiếp vào database, không có quyền gọi mạng ngoài whitelist, không có quyền truy cập file system ngoài thư mục plugin. Điều này đảm bảo rằng một plugin lỗi hoặc độc hại không thể ảnh hưởng đến toàn bộ hệ thống.

**Versioning Contract**: Đề tài định nghĩa một hệ thống versioning cho plugin. Mỗi plugin khai báo `minCoreVersion` và `maxCoreVersion` - khoảng phiên bản core mà plugin tương thích. Khi core được nâng cấp, nếu plugin không còn tương thích, quản trị viên nhận cảnh báo. Ngược lại, core có thể khai báo `contractVersion` - tập hợp các interface và API mà core hỗ trợ. Plugin yêu cầu `contractVersion: 2` nghĩa là cần core hỗ trợ ít nhất contract version 2. Nhờ đó, việc nâng cấp core có thể được kiểm soát có hệ thống và backward compatibility được đảm bảo.

Hình 2.5 dưới đây minh họa vòng đời của một plugin từ khi được phát hiện đến khi được unload.

**Hình 2.5. Vòng đời của plugin**

```
┌────────────┐
│ Discovered │  (manifest đọc thành công, kiểm tra signature)
└─────┬──────┘
      ▼
┌────────────┐
│ Validated  │  (kiểm tra contract version, dependency, sandbox)
└─────┬──────┘
      ▼
┌────────────┐
│ Loaded     │  (AssemblyLoadContext.Isolated, dependency resolve)
└─────┬──────┘
      ▼
┌────────────┐
│ OnEnable   │  (Register called, đăng ký tools, handlers)
└─────┬──────┘
      ▼
┌────────────┐
│ Enabled    │  (plugin đang chạy)
└─────┬──────┘
      │
      │  Hot-reload: file changed ────────────────────┐
      │  Admin disable ─────────────────────────────┐  │
      ▼                                              │  │
┌────────────┐                                       │  │
│ OnDisable  │  (cleanup, lưu state)               │  │
└─────┬──────┘                                       │  │
      ▼                                              │  │
┌────────────┐                                       │  │
│ Unloaded   │  (AssemblyLoadContext.Unload) <─────┘  │
└────────────┘
```

## 2.4. Thiết kế dữ liệu và cơ sở tri thức

### 2.4.1. Mô hình dữ liệu quan hệ (PostgreSQL)

Cơ sở dữ liệu quan hệ của hệ thống được tổ chức theo nhóm chức năng: quản lý tenant và người dùng; quản lý phiên và lịch sử hội thoại; quản lý tri thức (tài liệu, chunks, embedding); quản lý công cụ và plugin; quản lý audit log và monitoring. Việc chọn PostgreSQL làm cơ sở dữ liệu chính dựa trên ba lý do: (i) PostgreSQL là cơ sở dữ liệu quan hệ mã nguồn mở phổ biến nhất với hệ sinh thái phong phú (extension, driver, ORM); (ii) pgvector cho phép lưu trữ và truy vấn vector trong cùng một database, giảm đáng kể độ phức tạp vận hành; (iii) PostgreSQL có Row-Level Security (RLS) mạnh mẽ cho multi-tenant.

Một số bảng chính và quan hệ giữa chúng được minh họa trong Hình 2.6.

**Hình 2.6. Sơ đồ ER rút gọn của cơ sở dữ liệu quan hệ**

```
┌─────────────┐         ┌──────────────┐         ┌─────────────┐
│ tenants     │ 1──────* │ users        │ 1──────*│ sessions    │
└─────────────┘         └──────────────┘         └──────┬──────┘
                                                        │
                                                        │ 1
                                                        ▼ *
                                               ┌────────────────┐
                                               │ messages       │
                                               │ (role, tokens) │
                                               └────────────────┘

┌─────────────┐         ┌──────────────┐         ┌─────────────┐
│ tenants     │ 1──────* │ documents    │ 1──────*│ chunks      │
└─────────────┘         └──────────────┘         └──────┬──────┘
                                                        │
                                                        │ 1
                                                        ▼ *
                                               ┌────────────────┐
                                               │ embeddings     │
                                               │ (vector(1536)) │
                                               └────────────────┘

┌─────────────┐         ┌──────────────┐         ┌─────────────┐
│ tenants     │ 1──────* │ api_keys     │         │ audit_logs  │
└─────────────┘         └──────────────┘         └─────────────┘
        │                                              │
        └──────────* plugins ─────────────────────────┘
```

**Bảng 2.5. Một số bảng chính trong cơ sở dữ liệu**

| Bảng | Một số cột quan trọng | Vai trò | Ghi chú |
|---|---|---|---|
| tenants | id (uuid PK), code (unique), name, plan, config (jsonb), created_at | Lưu thông tin tenant | config chứa cấu hình AI provider, plugin, quota per tenant |
| users | id (uuid PK), tenant_id (FK), email, role, hashed_password, preferences (jsonb) | Người dùng nội bộ của mỗi tenant | |
| api_keys | id, tenant_id (FK), name, hashed_key, scopes (text[]), expires_at, last_used_at | Khóa API cho service-to-service | hashed_key: SHA-256 hash |
| sessions | id (uuid PK), tenant_id (FK), user_id (FK), started_at, ended_at, metadata (jsonb) | Phiên hội thoại | |
| messages | id, session_id (FK), role (enum: user/assistant/system/tool), content (text), tokens_in, tokens_out, metadata (jsonb), created_at | Tin nhắn trong phiên | role = 'tool' cho kết quả từ tool call |
| documents | id (uuid PK), tenant_id (FK), name, source_path, mime_type, size_bytes, uploaded_by (FK), ingested_at, status (enum), metadata (jsonb) | Siêu dữ liệu tài liệu | status: queued/parsing/chunking/embedding/done/error |
| chunks | id (bigserial PK), tenant_id (FK), document_id (FK), ordinal (int), content (text), tokens_count (int), metadata (jsonb), created_at | Đoạn văn sau khi chunk | |
| embeddings | chunk_id (PK, FK), embedding (vector(1536)), model (text), dimensions (int), created_at | Vector embedding | Có thể gộp cột embedding vào bảng chunks |
| workflows | id (uuid PK), tenant_id (FK), name, definition (jsonb), status (enum), current_step, variables (jsonb) | Định nghĩa và trạng thái workflow | |
| plugins | id (uuid PK), name, version, manifest (jsonb), enabled, installed_at | Plugin đã cài | |
| mcp_servers | id (uuid PK), tenant_id (FK), name, endpoint, mode, auth_config (jsonb encrypted), enabled, tools_whitelist (text[]) | MCP server đã đăng ký | |
| audit_logs | id (bigserial PK), tenant_id (FK), actor (text), action (text), target (text), payload (jsonb), at (timestamptz) | Log kiểm toán bất biến | |
| quota_usage | id, tenant_id (FK), period_start (date), tokens_used (bigint), cost_estimate (decimal) | Thống kê quota | |

Các bảng trên đều có cột `tenant_id` để hỗ trợ đa tenant. Khóa ngoại luôn bao gồm cả `tenant_id` trong composite foreign key để đảm bảo tính nhất quán referential integrity giữa các bảng thuộc cùng tenant.

### 2.4.2. Cơ sở dữ liệu vector (pgvector)

pgvector là extension của PostgreSQL, cho phép lưu trữ vector với số chiều tùy ý và thực hiện tìm kiếm lân cận gần (Approximate Nearest Neighbor - ANN). Việc hợp nhất vector store trong cùng cơ sở dữ liệu quan hệ mang lại ba lợi ích quan trọng:

- **Giảm độ phức tạp vận hành**: không cần vận hành thêm một hệ quản trị CSDL riêng cho vector (như Milvus, Qdrant, Weaviate, Pinecone). Một database duy nhất cho cả dữ liệu quan hệ và vector đồng nghĩa với một backup strategy, một monitoring setup, một team có thể vận hành.
- **Tận dụng transaction, backup, replication có sẵn**: toàn bộ dữ liệu (quan hệ + vector) nằm trong cùng transaction, đảm bảo tính nhất quán. Backup bằng pg_dump/pg_restore bao gồm cả vector.
- **Kết hợp truy vấn quan hệ và vector trong một câu SQL**: cho phép lọc theo metadata (tenant, document, ngày tạo...) trước khi tính khoảng cách vector - đây là truy vấn rất phổ biến trong bối cảnh RAG đa tenant.

Đoạn mã SQL dưới đây minh họa cấu trúc bảng chunks với cột embedding kiểu vector và các chỉ mục:

```sql
-- Bật extension pgvector
CREATE EXTENSION IF NOT EXISTS vector;

-- Bảng chunks: lưu cả nội dung văn bản và vector embedding
CREATE TABLE chunks (
    id              BIGSERIAL PRIMARY KEY,
    tenant_id       UUID NOT NULL,
    document_id     UUID NOT NULL REFERENCES documents(id) ON DELETE CASCADE,
    ordinal         INT NOT NULL,           -- thứ tự chunk trong tài liệu
    content         TEXT NOT NULL,           -- nội dung văn bản của chunk
    tokens_count    INT,                     -- số token ước lượng
    metadata        JSONB NOT NULL DEFAULT '{}',  -- siêu dữ liệu (trang, tiêu đề...)
    embedding       VECTOR(1536),            -- vector 1536 chiều
    created_at      TIMESTAMPTZ NOT NULL DEFAULT now(),
    
    -- Ràng buộc: mỗi chunk chỉ có một vector
    CONSTRAINT fk_document FOREIGN KEY (document_id, tenant_id) 
        REFERENCES documents(id, tenant_id) ON DELETE CASCADE
);

-- Chỉ mục vector: IVF-Flat cho tìm kiếm ANN
-- lists = 100: chia không gian thành 100 clusters
-- Phù hợp cho <1M vectors. Khi scale, tăng lists hoặc chuyển HNSW.
CREATE INDEX idx_chunks_embedding ON chunks 
USING ivfflat (embedding vector_cosine_ops) WITH (lists = 100);

-- Chỉ mục cho tenant: tăng tốc lọc theo tenant
CREATE INDEX idx_chunks_tenant ON chunks(tenant_id);

-- Chỉ mục cho document: tăng tốc truy vấn theo tài liệu
CREATE INDEX idx_chunks_document ON chunks(document_id, tenant_id);

-- Chỉ mục JSONB cho metadata filter
CREATE INDEX idx_chunks_metadata ON chunks USING gin (metadata);
```

Trong đoạn trên, chỉ số `ivfflat` (Inverted File Index with Flat) được dùng cho tìm kiếm ANN với khoảng cách cosine - đây là lựa chọn phù hợp cho đa số trường hợp với dưới 1 triệu vector. Ưu điểm của IVF-Flat là tốc độ xây dựng chỉ mục nhanh và bộ nhớ yêu cầu thấp. Khi số lượng vector tăng đáng kể (> 1 triệu), có thể chuyển sang chỉ mục HNSW (Hierarchical Navigable Small World), vốn có tốc độ truy vấn nhanh hơn (recall cao hơn) với chi phí xây dựng chỉ mục lớn hơn và bộ nhớ cao hơn. Đề tài thiết kế để dễ dàng chuyển đổi giữa hai loại chỉ mục mà không cần thay đổi application code.

Để tận dụng tối đa khả năng của pgvector trong bối cảnh đa tenant, các truy vấn được thiết kế theo mẫu:

```sql
-- Truy vấn hybrid search: vector similarity + metadata filter + tenant isolation
-- $1 = query embedding (float array)
-- $2 = metadata filter (jsonb, vd: {"category": "contract"})
-- $3 = top K
SELECT 
    id, 
    content, 
    metadata,
    1 - (embedding <=> $1) AS similarity,  -- cosine distance -> similarity
    (metadata @> $2 OR $2 IS NULL) AS filter_match
FROM chunks
WHERE tenant_id = current_setting('app.tenant_id')::uuid
  AND (metadata @> $2 OR $2 IS NULL)
ORDER BY embedding <=> $1
LIMIT $3;
```

Biến `current_setting('app.tenant_id')` được set tự động bởi middleware ở tầng kết nối (ví dụ: Npgsql connection interceptor) mỗi khi có request. Kết hợp với RLS, điều kiện này đảm bảo rằng mọi truy vấn vector đều tự động có điều kiện `tenant_id`, ngay cả khi developer quên thêm.

### 2.4.3. Multi-tenant với RLS (Row-Level Security)

Multi-tenant là yêu cầu cốt lõi (FR-11) và là một trong 8 nguyên tắc thiết kế. Cách tiếp cận của đề tài là dùng chung cơ sở dữ liệu, chia sẻ schema (shared schema, shared database) nhưng phân tách dữ liệu ở mức hàng (row) bằng Row-Level Security của PostgreSQL. Đây là cách tiếp cận phổ biến và được khuyến nghị cho multi-tenant vì: (i) hiệu quả về chi phí (không cần nhiều database); (ii) dễ quản lý (một database duy nhất); (iii) RLS đảm bảo cô lập ở mức database.

Cơ chế hoạt động của RLS trong đề tài gồm ba bước:

**Bước 1 - Thiết lập RLS**: Mỗi bảng có dữ liệu tenant-sensitive được bật RLS và có policy giới hạn quyền đọc theo tenant_id.

**Bước 2 - Thiết lập biến session**: Mỗi request HTTP đi qua middleware xác định tenant dựa trên JWT token hoặc API key. Middleware gọi câu lệnh `SET LOCAL app.tenant_id = '<uuid>'` trên transaction của request đó. Lệnh `SET LOCAL` chỉ có hiệu lực trong transaction hiện tại, tự động hủy khi transaction kết thúc - đảm bảo không có leak giữa các request.

**Bước 3 - Áp dụng policy**: Mọi SELECT, UPDATE, DELETE trên bảng có RLS sẽ tự động kiểm tra policy. PostgreSQL kiểm tra biến `current_setting('app.tenant_id')` và chỉ trả về hàng có tenant_id khớp.

Đoạn mã SQL dưới đây minh họa policy trên bảng chunks:

```sql
-- Bật RLS trên bảng chunks
ALTER TABLE chunks ENABLE ROW LEVEL SECURITY;
-- Vô hiệu hóa SELECT cho owner (chỉ qua policy)
ALTER TABLE chunks FORCE ROW LEVEL SECURITY;

-- Policy cô lập tenant: chỉ đọc hàng thuộc tenant hiện tại
CREATE POLICY tenant_isolation_chunks ON chunks
FOR ALL
USING (tenant_id = current_setting('app.tenant_id')::uuid)
WITH CHECK (tenant_id = current_setting('app.tenant_id')::uuid);

-- Tương tự cho các bảng khác
ALTER TABLE documents ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_documents ON documents
FOR ALL USING (tenant_id = current_setting('app.tenant_id')::uuid)
WITH CHECK (tenant_id = current_setting('app.tenant_id')::uuid);

ALTER TABLE sessions ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_sessions ON sessions
FOR ALL USING (tenant_id = current_setting('app.tenant_id')::uuid)
WITH CHECK (tenant_id = current_setting('app.tenant_id')::uuid);

ALTER TABLE messages ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation_messages ON messages
FOR ALL USING (tenant_id = current_setting('app.tenant_id')::uuid)
WITH CHECK (tenant_id = current_setting('app.tenant_id')::uuid);
```

Để giảm rủi ro khi quên set biến tenant, hệ thống sử dụng một database role riêng (`app_tenant_user`) thay vì superuser cho mọi kết nối từ ứng dụng. Role này không có quyền bypass RLS, buộc mọi truy vấn phải qua policy. Điều này đảm bảo rằng ngay cả khi code có bug và quên set tenant, PostgreSQL sẽ trả về 0 hàng thay vì tất cả hàng (fail-safe).

Ngoài RLS, đề tài còn áp dụng các biện pháp bổ trợ: (i) mọi bảng đều có cột `tenant_id` được denormalized (lưu trùng lắp) để tăng tốc truy vấn và đảm bảo referential integrity; (ii) khóa ngoại composite `(id, tenant_id)` được sử dụng cho các bảng có quan hệ cha-con trong cùng tenant, đảm bảo không thể có orphan record thuộc tenant khác; (iii) trigger `prevent_cross_tenant_insert` trên mỗi bảng để đảm bảo rằng khi insert, giá trị `tenant_id` được so sánh với session variable, ngăn chặn insert nhầm tenant.

### 2.4.4. Enterprise Knowledge Base

Enterprise Knowledge Base (EKB) là tổng thể các cấu trúc dữ liệu và dịch vụ phục vụ tri thức doanh nghiệp. EKB không chỉ đơn thuần là các tài liệu PDF/DOCX được upload lên, mà là một hệ thống tri thức phân tầng bao gồm bốn loại tri thức:

**Structured Knowledge (Tri thức có cấu trúc)**: các bảng dữ liệu trong ERP, CRM, HRM, BI mà AI có thể truy vấn qua SQL Engine hoặc qua MCP server tương ứng. Đây là tri thức có độ chính xác cao nhất vì được quản lý bởi hệ thống nghiệp vụ. Tuy nhiên, truy cập cần được kiểm soát chặt chẽ qua SQLGuard.

**Unstructured Knowledge (Tri thức không cấu trúc)**: tài liệu văn bản (PDF, DOCX, Markdown, HTML, TXT) được upload và index trong RAG Engine. Đây là tri thức phổ biến nhất trong giai đoạn đầu. Chất lượng phụ thuộc vào chất lượng chunking và embedding.

**Conversational Knowledge (Tri thức hội thoại)**: lịch sử hội thoại và dữ liệu do Agent ghi nhận trong Memory. Đây là tri thức "sống", được tạo ra trong quá trình sử dụng, bao gồm thông tin về sở thích người dùng, cách diễn giải một khái niệm, các câu hỏi thường gặp trong tổ chức.

**Procedural Knowledge (Tri thức quy trình)**: workflow, runbook, playbook mà Workflow Engine thực thi. Đây là tri thức về "cách làm" thay vì "cái gì".

EKB có hai đặc tính quan trọng: (i) được gắn với một tenant cụ thể và cô lập theo tenant (qua RLS); (ii) có khả năng cập nhật liên tục. Khi tài liệu mới được upload, nó được ingest và index gần như thời gian thực (qua hàng đợi background). Khi tài liệu cũ được cập nhật, hệ thống phát hiện thay đổi qua hash và tự động re-index các chunk liên quan.

Để đảm bảo khả năng theo vết (traceability) và tái tạo (reproducibility) của quá trình ingest, đề tài sử dụng bảng `ingestion_jobs`:

```sql
CREATE TABLE ingestion_jobs (
    id              BIGSERIAL PRIMARY KEY,
    tenant_id       UUID NOT NULL,
    document_id     UUID NOT NULL REFERENCES documents(id) ON DELETE CASCADE,
    status          TEXT NOT NULL, 
        -- queued | parsing | chunking | embedding | indexing | done | error
    started_at      TIMESTAMPTZ,
    finished_at     TIMESTAMPTZ,
    chunks_created  INT,
    error           TEXT,
    retry_count     INT DEFAULT 0,
    created_at      TIMESTAMPTZ NOT NULL DEFAULT now()
);
```

Qua bảng `ingestion_jobs`, mọi quá trình nạp tri thức đều được ghi nhận đầy đủ. Nếu một tài liệu không được index đúng, quản trị viên có thể truy vết: tài liệu đã được upload lúc nào, đang ở bước nào, có lỗi gì không. Điều này phục vụ cả hai mục tiêu NFR-05 (Audit) và NFR-07 (Observability).

EKB cũng được thiết kế để hỗ trợ **cross-domain knowledge synthesis**: khi người dùng hỏi một câu hỏi liên quan đến nhiều miền (ví dụ: "tổng doanh thu quý này và so sánh với hợp đồng lớn nhất"), Context Builder ở Layer 4 có khả năng tổng hợp kết quả từ nhiều knowledge source (SQL Engine cho doanh thu, MCP call cho hợp đồng) thành một ngữ cảnh duy nhất trước khi gọi LLM. Đây là cách nguyên tắc DOMAIN AWARE được hiện thực ở tầng dữ liệu.

## 2.5. Thiết kế giao thức tích hợp và bảo mật

### 2.5.1. MCP Server và Client

Phần này trình bày chi tiết hơn về cách MCP được sử dụng trong đề tài, kế thừa phần giới thiệu ở mục 2.3.5. MCP được thiết kế với hai vai trò đối xứng trong kiến trúc.

**Vai trò của nền tảng với tư cách MCP Client**: Nền tảng gọi tới các MCP server bên ngoài để truy xuất tool và resource. Ví dụ: khi người dùng yêu cầu "kiểm tra đơn hàng ORD-2025-001 trong ERP", Agent sẽ sử dụng MCP client để gọi tới MCP server `erp-acme` được cấu hình sẵn, với tool `get_order`. Cơ chế "khám phá động" (dynamic discovery) cho phép Agent liệt kê tất cả tool mà MCP server cung cấp mà không cần biết trước schema - đây là ưu điểm then chốt của MCP so với việc hard-code function calling.

Quy trình tích hợp một hệ thống mới qua MCP gồm: (i) quản trị viên đăng ký MCP server trong Admin Dashboard (tên, endpoint, auth); (ii) hệ thống gọi `DiscoverAsync` để lấy thông tin server; (iii) hệ thống gọi `ListToolsAsync` để lấy danh sách tool; (iv) hệ thống kiểm tra whitelist, đăng ký tool vào Tool Registry; (v) từ thời điểm này, Agent có thể gọi tool như bất kỳ tool nội bộ nào. Toàn bộ quy trình mất khoảng 1-2 phút và không cần restart hệ thống.

**Vai trò của nền tảng với tư cách MCP Server**: Ngược lại, nền tảng cũng cung cấp MCP server để các hệ thống bên ngoài có thể truy vấn tới. Ví dụ: một ứng dụng di động hoặc một chatbot bên thứ ba có thể gọi tới MCP server của nền tảng với tool `ai.chat_completion` để sử dụng năng lực hội thoại, hoặc với tool `ai.search_knowledge` để truy vấn knowledge base. Cách tổ chức này phù hợp triết lý "cắm vào là chạy": một đối tác có MCP client tương thích sẽ tự động "thấy" các khả năng của nền tảng mà không cần tài liệu API chi tiết.

Đoạn mã C# dưới đây minh họa cách nền tảng đăng ký các tool và resource của mình như một MCP Server:

```csharp
/// <summary>
/// Plugin đăng ký các tool nội bộ của nền tảng như MCP Server.
/// </summary>
public class PlatformMcpServerPlugin : IPluginModule
{
    public void Register(IPluginContext ctx)
    {
        // Tool hội thoại AI
        ctx.Mcp.RegisterTool(new McpToolDefinition
        {
            Name = "ai.chat_completion",
            Description = "Sinh câu trả lời dựa trên tri thức nội bộ của tenant. " +
                "Tự động sử dụng RAG, SQL Engine, hoặc Agent tùy theo câu hỏi.",
            ParametersSchema = new
            {
                type = "object",
                properties = new
                {
                    query = new { type = "string", description = "Câu hỏi của người dùng" },
                    domain = new { type = "string", description = "Miền nghiệp vụ (tùy chọn)" },
                    sessionId = new { type = "string", description = "ID phiên hội thoại (tùy chọn)" }
                },
                required = new[] { "query" }
            },
            Handler = async (args, ct) => 
            {
                var orchestrator = ctx.Services.GetRequiredService<IAgentOrchestrator>();
                var result = await orchestrator.ExecuteAsync(new AgentTask
                {
                    Input = args.query,
                    DomainContext = args.domain,
                    SessionId = args.sessionId
                }, ct);
                return new { answer = result.FinalAnswer, traceId = result.TraceId };
            }
        });

        // Tool tra cứu knowledge base
        ctx.Mcp.RegisterTool(new McpToolDefinition
        {
            Name = "ai.search_knowledge",
            Description = "Tìm kiếm trong knowledge base của tenant.",
            ParametersSchema = new
            {
                type = "object",
                properties = new
                {
                    query = new { type = "string" },
                    topK = new { type = "integer", default = 5 }
                },
                required = new[] { "query" }
            },
            Handler = async (args, ct) => 
            {
                var ragEngine = ctx.Services.GetRequiredService<IRagEngine>();
                var result = await ragEngine.QueryAsync(new RagQuery
                {
                    Text = args.query,
                    TopK = args.topK ?? 5
                }, ct);
                return result.Chunks.Select(c => new 
                { 
                    content = c.Content, 
                    source = c.DocumentName, 
                    score = c.Score 
                });
            }
        });

        // Resource: danh sách tài liệu đã index
        ctx.Mcp.RegisterResource(new McpResourceDefinition
        {
            Uri = "platform://tenants/{tenantId}/documents",
            Description = "Liệt kê tài liệu đã được index của tenant.",
            MimeType = "application/json",
            Handler = async (uriVars, ct) => 
            {
                var tenantId = uriVars["tenantId"];
                var docs = await ctx.Services
                    .GetRequiredService<IDocumentRepository>()
                    .ListAsync(tenantId, ct);
                return docs.Select(d => new { id = d.Id, name = d.Name, status = d.Status });
            }
        });
    }
}
```

**Giao thức truyền thông**: Như đã đề cập ở mục 2.2.4, đề tài hỗ trợ ba phương thức: (i) stdio cho tích hợp cục bộ (MCP server chạy cùng máy, không qua mạng - phù hợp cho các tool nhạy cảm cần độ trễ thấp); (ii) HTTP+SSE cho phần lớn tình huống (reliable, firewall-friendly); (iii) Streamable HTTP cho các use case yêu cầu streaming hiệu năng cao. Việc hỗ trợ cả ba phương thức đảm bảo tính linh hoạt tối đa trong mọi bối cảnh triển khai.

### 2.5.2. Plugin và Hot-reload

Plugin ở đề tài được đóng gói thành file `.dll` (cho ngôn ngữ C#/.NET) kèm manifest JSON. Một plugin hoàn chỉnh gồm: (i) manifest mô tả metadata (tên, phiên bản, mô tả, tác giả, contract version, dependencies); (ii) assembly chứa code thực thi (một hoặc nhiều .dll); (iii) tài nguyên kèm theo (icon, schema, template). Hệ thống lưu plugin trong thư mục `/plugins` trên filesystem và quản lý qua API hoặc Admin Dashboard.

**Cơ chế Hot-reload** cho phép nạp/gỡ plugin khi hệ thống đang chạy, không cần restart toàn bộ ứng dụng. Đây là tính năng quan trọng trong môi trường production vì cho phép cập nhật plugin mà không gây downtime. Cơ chế được đảm bảo bởi bốn thành phần đã được mô tả ở mục 2.3.5: AssemblyLoadContext riêng, FileSystemWatcher, lifecycle hooks, và sandbox giới hạn.

Ngoài hot-reload, Plugin SDK còn cung cấp một số tiện ích phát triển: (i) `IPluginContext` - giao diện chuẩn để plugin tương tác với nền tảng (đăng ký tool, đăng ký event handler, truy cập config, truy cập services); (ii) `IPluginLogger` - logging chuẩn cho plugin, log được aggregate cùng log của core; (iii) `IPluginConfig` - cấu hình riêng của plugin, được lưu trong bảng plugins và có thể chỉnh sửa qua Admin Dashboard; (iv) `IPluginDiagnostics` - cung cấp thông tin về trạng thái plugin (memory usage, số tool đã đăng ký, thời gian chạy) cho mục đích giám sát.

**Versioning Contract** đảm bảo tính tương thích giữa plugin và core qua một hệ thống version đơn giản nhưng hiệu quả. Mỗi core release có một `ContractVersion` (số nguyên tăng dần). Plugin khai báo `ContractVersion` tối thiểu mà nó cần. Khi install plugin, hệ thống kiểm tra: nếu core ContractVersion < plugin yêu cầu, install bị từ chối với thông báo "Plugin requires contract version X but current version is Y". Ngược lại, nếu core ContractVersion > plugin max, install cũng bị từ chối với thông báo "Plugin is not compatible with this version of core". Cách làm này ngăn chặn plugin không tương thích được install và gây lỗi runtime.

### 2.5.3. Xác thực, phân quyền và Audit log

Bảo mật là một trong 8 nguyên tắc thiết kế và là NFR-04, NFR-05. Phần này trình bày chi tiết ba cơ chế chính: Authentication (AuthN), Authorization (AuthZ), và Audit Logging.

**Authentication (AuthN)**: Hệ thống hỗ trợ bốn phương thức xác thực, cho phép linh hoạt theo nhu cầu của từng doanh nghiệp:

- **Username/Password**: với hashing chuẩn Argon2id (ưu tiên) hoặc BCrypt. Hệ thống không bao giờ lưu mật khẩu dạng plain text hoặc hash yếu (MD5, SHA1 bị cấm). Mật khẩu phải đáp ứng policy tối thiểu: độ dài tối thiểu 8 ký tự, có chữ hoa, chữ thường, số.
- **JWT (JSON Web Token)**: có chữ ký HS256 hoặc RS256, thời gian sống ngắn. Đề tài sử dụng access token 15 phút và refresh token 7 ngày. Refresh token được lưu hashed trong database và có thể thu hồi.
- **OAuth2/OIDC**: tích hợp với các nhà cung cấp bên ngoài như Google Workspace, Microsoft Entra ID (Azure AD). Phù hợp với doanh nghiệp đã có SSO doanh nghiệp.
- **API Key**: cho service-to-service integration. Khóa có dạng `eai_sk_xxx` (prefix để nhận diện), được hash bằng SHA-256 trước khi lưu vào database. Mỗi API key có: tên, scopes (danh sách quyền), ngày hết hạn, tenant gắn kèm.

Sơ đồ dưới đây mô tả cách token được cấp và sử dụng trong hệ thống:

```
Client ──[1. Login (credentials/API key)]──> Auth Service ──[2. Validate]──> DB
                                                        │
                                                        ▼
                                                  [3. Issue JWT]
                                                        │
                                                        ▼
                                                  [4. Return JWT]
                                                        │
Client ──[5. Bearer JWT]──> API Gateway ──[6. Verify & Extract]──> Claims
                                                        │
                                                        ├──[7a. Tenant mapping]──> ctx.Items["TenantId"]
                                                        │
                                                        └──[7b. Role extraction]──> ctx.Items["Roles"]
                                                              │
                                                              ▼
                                                         [8. Internal Service]
```

**Authorization (AuthZ)**: Sử dụng mô hình kết hợp RBAC (Role-Based Access Control) và ABAC (Attribute-Based Access Control). Hệ thống có sẵn bốn vai trò mặc định:

- `super_admin`: quản trị nền tảng (quản lý multi-tenant, cài plugin toàn cục).
- `tenant_admin`: quản trị tenant (quản lý user trong tenant, cấu hình AI, cài plugin tenant).
- `developer`: phát triển plugin, truy cập API explorer.
- `end_user`: người dùng cuối (sử dụng chat, truy vấn knowledge base).

Mỗi vai trò có một tập quyền (permissions) cố định, có thể được mở rộng bởi tenant thông qua custom roles. Ngoài RBAC, một số resource cụ thể yêu cầu kiểm tra policy bổ sung (ABAC). Ví dụ: action `document.delete` yêu cầu kiểm tra xem user có phải là chủ sở hữu document hay không (`ownerId == currentUserId` hoặc `role == tenant_admin`).

```csharp
/// <summary>
/// Policy Enforcer - kiểm tra quyền truy cập dựa trên RBAC + ABAC.
/// </summary>
public class PolicyEnforcer : IPolicyEnforcer
{
    public async Task<bool> IsAllowedAsync(
        ClaimsPrincipal user, 
        string action, 
        string resource, 
        object? context)
    {
        // 1. Lấy roles và tenant từ claims
        var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        var tenantId = user.FindFirst("tenant_id")?.Value;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // 2. Lấy permissions của roles
        var permissions = await _roleRepository.GetPermissionsAsync(roles);
        
        // 3. Kiểm tra permission cơ bản
        if (!permissions.Contains(action))
            return false;
        
        // 4. Kiểm tra ABAC policy nếu có (ví dụ: ownership check)
        if (_policyStore.HasAbacPolicy(action))
        {
            var abacResult = await _policyStore.EvaluateAbacAsync(new PolicyRequest
            {
                TenantId = tenantId,
                UserId = userId,
                Roles = roles,
                Action = action,
                Resource = resource,
                Context = context  // ví dụ: { ownerId = "u-45" }
            });
            return abacResult;
        }
        
        return true;
    }
}
```

**Audit log**: Mọi hành động quan trọng đều được ghi vào bảng `audit_logs` với các trường: thời gian chính xác, actor (người thực hiện), tenant, hành động, tài nguyên bị tác động, payload ngắn gọn. Audit log có ba đặc điểm then chốt:

- **Bất biến (Immutability)**: không cho UPDATE/DELETE ở cấp database policy. Chỉ có role `audit_admin` (tách biệt khỏi super_admin) mới có quyền SELECT. Không có bất kỳ operation nào cho phép sửa hoặc xóa log.
- **Dấu thời gian chuẩn**: cột `at` kiểu `TIMESTAMPTZ` (timestamp with timezone), có chỉ mục để truy vấn theo thời gian và theo actor.
- **Phục vụ truy vết**: cho phép truy ngược "ai đã gọi tool X với tham số Y vào thời điểm nào, kết quả ra sao".

Ngoài audit log, hệ thống còn có **application log** theo chuẩn OpenTelemetry (log, metric, trace). Mỗi request HTTP có một `trace_id` liên kết tất cả log liên quan từ Gateway qua Agent Orchestrator đến RAG Engine và LLM Provider. Điều này phục vụ NFR-07 (Observability) và là cơ sở cho dashboard giám sát Prometheus/Grafana.

## 2.6. Quy trình triển khai theo Phase

### 2.6.1. Tổng quan 5 Phase

Quá trình triển khai được chia thành 5 phase, mỗi phase kéo dài từ 2 đến 5 tuần. Nguyên tắc chung là *mỗi phase phải cho ra một phiên bản có thể chạy được (runnable), có thể demo được (demonstrable), có thể đo lường được (measurable)*. Không có phase nào kết thúc mà không có sản phẩm có thể trình diễn. Tổng quan 5 phase được tổng hợp trong Bảng 2.6.

**Bảng 2.6. Tổng quan 5 phase triển khai**

| Phase | Tên gọi | Mục tiêu chính | Thời lượng | Kết quả đầu ra kiểm chứng được |
|---|---|---|---|---|
| 1 | Foundation - Walking Skeleton | Có hệ thống minimal chạy được với Ollama local, 1 tenant, hội thoại cơ bản, CI/CD | 2-3 tuần | Demo: "Hỏi đáp với AI local". CI/CD chạy. Docker Compose triển khai. |
| 2 | AI Engine & RAG | Upload tài liệu, hỏi đáp trên tài liệu, multi-provider LLM, hybrid search | 3-4 tuần | Demo: "Upload PDF, hỏi đáp có trích dẫn". Bộ test RAG cơ bản. |
| 3 | Agent & Integration | Intent Router, Task Planner, Agent Executor, MCP Client, Plugin SDK | 4-5 tuần | Demo: "Agent đa bước gọi tool ERP". Plugin mẫu (ERP connector). |
| 4 | Advanced Features | Hybrid search nâng cao, Workflow Engine, Memory, Voice, SQL Engine | 3-4 tuần | Demo: "Quy trình duyệt tự động có HITL". Hỗ trợ giọng nói tiếng Việt. |
| 5 | Enterprise Ready | Multi-tenancy đầy đủ, RBAC, Audit, K8s deployment | 4-5 tuần | Demo: "Nhiều tenant, RBAC, audit đầy đủ". Production-ready. |

Tổng thời gian ước tính: 16-21 tuần (4-5 tháng). Mốc thời gian này phù hợp với quy mô và khung thời gian của đồ án thạc sĩ, đồng thời vừa đủ để chứng minh tính khả thi kỹ thuật của kiến trúc đề xuất.

**Chi tiết từng phase:**

**Phase 1 - Foundation (Walking Skeleton)**: Phase này xây dựng một hệ thống cực kỳ tối giản nhưng đi xuyên suốt tất cả 7 tầng kiến trúc. Mục tiêu không phải là có một hệ thống hoàn chỉnh mà là có một "đường ống" (pipeline) từ đầu đến cuối chạy được. Layer 1 (Infrastructure) gồm: PostgreSQL đơn giản, Redis, MinIO, Ollama. Layer 2 (Integration) gồm: Plugin SDK skeleton, Event Bus đơn giản. Layer 3 (Provider Abstraction) gồm: ILLMProvider với hiện thực Ollama, không có router. Layer 4 (AI Engine Core) gồm: Conversation Manager đơn giản (lưu tin nhắn, gọi LLM). Layer 5 (Agent Orchestration) gồm: pass-through (gọi thẳng LLM). Layer 6 (API Gateway) gồm: AuthN đơn giản. Layer 7 (Presentation) gồm: Web UI đơn giản (form nhập + hiển thị response). Kết quả demo: người dùng nhập câu hỏi, nhận câu trả lời từ Ollama local.

**Phase 2 - AI Engine & RAG**: Phase này biến hệ thống từ "chat với AI" thành "chat với tri thức doanh nghiệp". Các module mới: RAG Engine hoàn chỉnh (parser, chunker, embedder, indexer, retrieval, generation với citation); Hybrid Search Engine (dense + sparse + RRF + reranking); Multi-Provider Router (OpenAI, Anthropic, Ollama); Cache Layer (prompt cache, embedding cache); Model Registry UI. Kết quả demo: upload file PDF (hợp đồng 50 trang), hỏi "điều khoản phạt vi phạm là gì?", nhận câu trả lời kèm citation đến trang cụ thể.

**Phase 3 - Agent & Integration**: Phase này biến hệ thống từ "hỏi đáp" thành "tự động hóa nghiệp vụ". Các module mới: Intent Router (phân loại ý định); Task Planner (ReAct + Plan-and-Execute); Executor với giới hạn bước/token; MCP Client (kết nối với MCP server mẫu); Plugin SDK hoàn chỉnh; Workflow Engine cơ bản. Kết quả demo: hỏi "tổng doanh thu quý 3 của phòng kinh doanh", Agent phân tích ý định, gọi SQL Engine truy vấn database, trả về kết quả có biểu đồ.

**Phase 4 - Advanced Features**: Phase này bổ sung các tính năng nâng cao. Các module mới: Hybrid Search nâng cao (query expansion, multi-vector); Workflow Engine hoàn chỉnh (BPMN-lite, HITL); Memory Service hoàn chỉnh (short-term + long-term); Voice Engine (STT + TTS tiếng Việt, Whisper.cpp + Coqui TTS); SQL Engine hoàn chỉnh (SQLGuard, schema introspection); Context Builder với compression. Kết quả demo: người dùng nói "cho tôi xem báo cáo tình hình nhân sự tháng này", hệ thống nhận diện giọng nói, truy vấn HRM, tổng hợp, sinh báo cáo, đọc kết quả bằng TTS.

**Phase 5 - Enterprise Ready**: Phase cuối cùng biến hệ thống thành production-ready. Các module mới: RLS hoàn chỉnh cho multi-tenant; RBAC + ABAC hoàn chỉnh; Audit log bất biến; Quota Tracker; Kubernetes deployment manifests; Auto-scaling configuration; Monitoring và alerting. Kết quả demo: 3 tenant đồng thời sử dụng hệ thống với dữ liệu cô lập, mỗi tenant có cấu hình AI provider riêng, quản trị viên tenant có dashboard audit xem ai đã truy vấn gì.

### 2.6.2. Walking Skeleton và Vertical Slice

Hai khái niệm "Walking Skeleton" và "Vertical Slice" chi phối cách xây dựng từng phase và là hai chiến lược phát triển phần mềm có tính hệ thống.

**Walking Skeleton** là cách tiếp cận trong đó phase đầu tiên (Phase 1) xây dựng một phiên bản cực kỳ tối giản nhưng đi xuyên suốt tất cả các tầng kiến trúc, có khả năng chạy thật (không phải mock), và có CI/CD pipeline hoàn chỉnh. Khác với prototype (chỉ chạy trong lab), Walking Skeleton là một hệ thống thật nhưng đơn giản. Điều này đảm bảo hai điều quan trọng: (i) đường ống (pipeline) từ đầu đến cuối đã chạy được - nếu không có Walking Skeleton, team có thể mất nhiều tuần để phát hiện rằng tầng Integration không tương thích với tầng Provider Abstraction; (ii) mọi rủi ro tích hợp đã được phát hiện sớm - những vấn đề khó nhất thường nằm ở ranh giới giữa các tầng, không phải bên trong một tầng.

**Vertical Slice** là cách cắt theo tính năng: mỗi tính năng được cài đặt xuyên suốt tất cả các tầng trong một chu kỳ ngắn (1-2 sprint), thay vì triển khai nguyên một tầng rồi mới sang tầng khác. Trong cách tiếp cận ngược lại (Layered Build), team sẽ dành 3 tuần để hoàn thành Layer 3 trước khi bắt đầu Layer 4 - điều này có nghĩa là trong 3 tuần không có gì để demo. Trong cách tiếp cận Vertical Slice, tuần đầu tiên đã có thể có một tính năng đơn giản chạy end-to-end.

Ví dụ cụ thể về Vertical Slice trong Phase 2: để triển khai tính năng "Upload tài liệu và hỏi đáp có trích dẫn", team cần làm đồng thời: Web UI (form upload, hiển thị câu trả lời), API endpoint cho upload, RAG Engine (parser, chunker, embedder, retrieval), Hybrid Search (dense + sparse), Context Builder (gắn citation), ILLMProvider call. Tất cả được làm trong 2 sprint (2 tuần) và cho ra một demo hoàn chỉnh. Sau đó, team tiếp tục slice tiếp theo: "Streaming response" hoặc "Multi-document search".

Hình 2.7 dưới đây minh họa sự khác biệt giữa Layered Build và Vertical Slice.

**Hình 2.7. So sánh Layered Build (theo tầng) và Vertical Slice (theo tính năng)**

```
Layered Build (TRÁNH - không có gì để demo trong 3 tháng đầu):
Sprint 1-2: Layer 7 (UI)
Sprint 3-4: Layer 6 (Gateway)
Sprint 5-6: Layer 5 (Agent)
Sprint 7-8: Layer 4 (AI Engine)
Sprint 9-10: Layer 3 (Provider)
Sprint 11-12: Layer 2 (Integration)
Sprint 13: Layer 1 (Infra)
── không chạy thử được cho đến khi tất cả xong! ──

Vertical Slice (DÙNG - luôn có demo sau mỗi sprint):
Sprint 1:  UI(RAG) → API(RAG) → RAG Engine → LLM Provider → Ollama  ✓ (chạy được)
Sprint 2:  UI(Search) → API(Search) → Hybrid Search → LLM Provider  ✓ (chạy được)
Sprint 3:  UI(MultiDoc) → API → Hybrid → Multi-doc retrieval           ✓ (chạy được)
Sprint 4:  UI(Citation) → API → Context Builder → Citation format     ✓ (chạy được)
```

Áp dụng kết hợp: **Walking Skeleton cho Phase đầu tiên** (đảm bảo đường ống tổng thể chạy), **Vertical Slice cho các Phase tiếp theo** (đảm bảo mỗi phase có nhiều sản phẩm demo).

### 2.6.3. Quản lý Interface giữa các Phase

Một thách thức lớn của triển khai theo phase là đảm bảo **Interface giữa các module ổn định**. Nếu Phase 2 thay đổi interface của ILLMProvider, Phase 3 phải sửa lại - đây là hiện tượng "breaking change propagation" rất tốn kém. Để tránh tình trạng này, đề tài áp dụng ba nguyên tắc quản lý interface.

**Nguyên tắc Stable Interface**: Mọi interface được khai báo ở đầu Phase 2 sẽ được "đóng băng" (frozen) trong suốt phần còn lại của dự án. Đóng băng không có nghĩa là không bao giờ thay đổi, mà là: (i) mọi thay đổi phải được lên kế hoạch như một breaking change; (ii) phải tạo version mới của interface (ví dụ: `ILLMProviderV2`); (iii) phải có adapter chuyển tiếp (adapter pattern) để code cũ vẫn chạy được với interface mới; (iv) phải có migration guide cho developer. Cách làm này giống với nguyên tắc "Open-Closed Principle" nhưng áp dụng ở cấp hệ thống, không chỉ cấp module.

```csharp
// V1: được định nghĩa ở Phase 1, frozen từ Phase 2
public interface ILLMProvider
{
    string Name { get; }
    Task<LLMResponse> CompleteAsync(LLMRequest request, CancellationToken ct);
}

// V2: được thêm ở Phase 3 khi cần hỗ trợ streaming
// V1 vẫn được giữ nguyên và có adapter V1ToV2Adapter
public interface ILLMProviderV2 : ILLMProvider
{
    IAsyncEnumerable<LLMStreamToken> StreamAsync(LLMRequest request, CancellationToken ct);
}
```

**Nguyên tắc 200K Token Budget**: Trong nghiên cứu này, tổng số token sử dụng cho việc gọi LLM (cả để sinh code, refactor, debug, viết tài liệu) được kiểm soát qua dashboard. Mục tiêu là dưới 200.000 token cho toàn bộ quá trình phát triển. Nguyên tắc này có hai tác dụng: (i) ràng buộc chi phí - mỗi lần gọi LLM đều có chi phí, buộc team phải suy nghĩ kỹ trước khi dùng; (ii) buộc người thiết kế phải tư duy trước (design-first) thay vì "thử-sai" với LLM. Trong thực tế, việc tư duy thiết kế kỹ lưỡng trước khi viết code tiết kiệm nhiều token hơn so với việc code rồi nhờ LLM sửa. Nguyên tắc này cũng khuyến khích tái sử dụng interface và pattern, giảm số lần phải viết code mới.

**Nguyên tắc Checkpoint & Tag**: Mỗi phase kết thúc phải có một git tag rõ ràng (ví dụ: `v0.1-foundation`, `v0.2-ai-engine`, `v0.3-agent`, `v0.4-advanced`, `v0.5-enterprise`). Tại mỗi checkpoint, hệ thống phải: build thành công, toàn bộ test pass, có thể demo được (có endpoint để trình diễn), và có changelog mô tả đã làm gì. Một checkpoint có thể được "phát hành" (publish) dưới dạng pre-release để người dùng nội bộ thử nghiệm và phản hồi.

Đề tài cũng định nghĩa rõ **tiêu chí vào/ra (entry/exit criteria)** cho từng phase. Mỗi phase chỉ được bắt đầu khi phase trước đạt exit criteria, và chỉ được kết thúc khi đạt exit criteria riêng. Ví dụ:

- *Phase 1 exit*: demo "Hỏi đáp với Ollama local" thành công, CI pipeline xanh, Docker Compose chạy đầy đủ services, tài liệu API Swagger tự động generate.
- *Phase 2 entry*: Phase 1 exit phải đạt.
- *Phase 2 exit*: demo "Upload PDF, hỏi đáp có trích dẫn" thành công, bộ test RAG đạt precision@5 > 0.7 trên benchmark dataset, Multi-Provider Router chuyển đổi được giữa Ollama và OpenAI.
- *(tiếp tục tương tự cho các phase tiếp theo)*.

Cách tổ chức này giúp dự án luôn có một "sản phẩm khả dụng" tại mọi thời điểm, giảm rủi ro tích lũy (accumulated risk) và tăng khả năng phản hồi khi có thay đổi yêu cầu. Ngay cả khi đề tài phải kết thúc sớm vì lý do nào đó, vẫn có một hệ thống có thể trình diễn ở phase gần nhất.

## 2.7. Kết luận chương 2

Chương 2 đã trình bày một cách có hệ thống quá trình phân tích bài toán và đề xuất thiết kế cho nền tảng Enterprise AI Platform. Từ bối cảnh doanh nghiệp và bốn nghịch lý trong tích hợp AI, đề tài đã xác định 12 yêu cầu chức năng (Bảng 2.1) và 8 nhóm yêu cầu phi chức năng (Bảng 2.2), từ đó hình thành 8 nguyên tắc thiết kế làm kim chỉ nam cho toàn bộ kiến trúc. Tám nguyên tắc này - PLUG & PLAY, PROVIDER AGNOSTIC, DOMAIN AWARE, EVENT DRIVEN, AGENT NATIVE, OFFLINE FIRST, ENTERPRISE GRADE, MULTI-TENANT - mỗi nguyên tắc được gắn với một số yêu cầu cụ thể và được hiện thực bằng các quyết định kiến trúc có cơ sở.

Kiến trúc 7 tầng (Hình 2.1) đã được trình bày chi tiết với trách nhiệm của từng tầng và cơ chế giao tiếp giữa các tầng. Ba đặc điểm then chốt của kiến trúc - một chiều phụ thuộc, tách giao diện và hiện thực, tích hợp chéo - đảm bảo tính module hóa và khả năng bảo trì. Phân tích so sánh ba phong cách kiến trúc (Bảng 2.3) đã dẫn đến quyết định sử dụng chiến lược "Modular Monolith trước, Microservice sau", phù hợp với bối cảnh nghiên cứu và quy mô đồ án thạc sĩ.

Trên cơ sở kiến trúc tổng thể, chương đã đi vào thiết kế chi tiết các module cốt lõi: Provider Abstraction Layer với các interface ILLMProvider, IEmbeddingProvider, Multi-Provider Router, Cache, Model Registry và Quota Tracker; AI Engine Core với RAG Engine (Hình 2.2, 2.3), Hybrid Search Engine với RRF, SQL Engine với SQLGuard, Tool Registry, Workflow Engine, Voice Engine và Memory Service; Agent Orchestration với Intent Router, Task Planner, Executor và HITL (Hình 2.4); AI Gateway với các middleware AuthN, AuthZ, Rate Limiting; Integration Layer với MCP Client/Server và Plugin SDK có hot-reload (Hình 2.5). Mỗi module đều được trình bày kèm interface C# minh họa và giải thích lý do thiết kế.

Thiết kế dữ liệu đã làm rõ mô hình quan hệ với 13 bảng chính (Hình 2.6, Bảng 2.5), cách sử dụng pgvector cho truy vấn vector với chỉ mục IVF-Flat/HNSW, cơ chế RLS cho multi-tenant ở mức hàng dữ liệu, và khái niệm Enterprise Knowledge Base với bốn loại tri thức. Thiết kế giao thức tích hợp và bảo mật đã trình bày chi tiết cách MCP được sử dụng trong cả hai vai trò (client và server), cách Plugin SDK với hot-reload và versioning contract, và ba tầng bảo mật (AuthN, AuthZ, Audit). Cuối cùng, quy trình triển khai 5 phase đã được mô tả với hai chiến lược Walking Skeleton và Vertical Slice (Hình 2.7), cùng ba nguyên tắc Stable Interface, 200K Token Budget và Checkpoint & Tag đảm bảo chất lượng và tiến độ.

Tổng thể thiết kế ở chương này nhằm đảm bảo ba mục tiêu xuyên suốt. Thứ nhất, **tính khả thi về mặt kỹ thuật** - thông qua các interface rõ ràng, công nghệ mã nguồn mở phổ biến và có cộng đồng hỗ trợ, các bản demo gia tăng qua từng phase, và nguyên tắc Walking Skeleton giúp phát hiện rủi ro sớm. Thứ hai, **tính đúng đắn học thuật** - thông qua 8 nguyên tắc thiết kế có cơ sở lý thuyết vững (interface segregation, separation of concerns, bounded context, fail-safe design), có so sánh đa tiêu chí (Bảng 2.3, 2.4) và có diễn giải rõ ràng về lý do lựa chọn công nghệ. Thứ ba, **phù hợp với bối cảnh nghiên cứu** - thông qua quy mô vừa phải (Modular Monolith), ngân sách token được kiểm soát, khả năng đo lường và tái lập, và kế hoạch 5 phase phù hợp với khung thời gian đồ án thạc sĩ.

Chương 3 sẽ trình bày quá trình triển khai cụ thể theo từng phase, các thực nghiệm và đánh giá kết quả đạt được trên các tiêu chí hiệu năng, khả năng tích hợp, chất lượng truy xuất tri thức, và khả năng phối hợp đa tác nhân.
