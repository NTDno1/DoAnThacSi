# MỞ ĐẦU

## 1. Lý do chọn đề tài

Trong những năm gần đây, sự bùng nổ của trí tuệ nhân tạo (Artificial Intelligence - AI), đặc biệt là các mô hình ngôn ngữ lớn (Large Language Models - LLM), đã tạo ra một cuộc cách mạng trong cách thức con người tương tác với hệ thống thông tin. Các mô hình như GPT-4, Claude, Gemini hay các mô hình mã nguồn mở như Llama, Mistral, DeepSeek không chỉ dừng lại ở khả năng sinh văn bản mà còn có thể lập luận, phân tích, lập kế hoạch và gọi các công cụ (function calling) để thực hiện tác vụ phức tạp. Điều này mở ra cơ hội ứng dụng AI vào hầu hết các ngành nghề, từ y tế, giáo dục, tài chính đến đặc biệt là trong lĩnh vực doanh nghiệp.

Tuy nhiên, việc ứng dụng AI vào các hệ thống doanh nghiệp hiện hữu (ERP, CRM, DMS, HRM, Workflow…) vẫn còn gặp nhiều rào cản. Theo khảo sát của các tổ chức nghiên cứu, phần lớn các doanh nghiệp khi muốn tích hợp AI vào hệ thống thường phải đối mặt với bốn vấn đề lớn: **chi phí tích hợp cao**, **thời gian triển khai lâu**, **yêu cầu chuyên môn sâu về cả AI lẫn hệ thống** và **khó mở rộng khi muốn áp dụng cho nhiều hệ thống khác nhau**. Một doanh nghiệp có hệ thống ERP muốn bổ sung chatbot hỗ trợ tra cứu thông tin phải xây dựng từ đầu; khi muốn tích hợp thêm với CRM lại phải làm lại; các giải pháp AI cho từng hệ thống thường không liên thông với nhau, dẫn đến dữ liệu và tri thức bị phân mảnh.

Bên cạnh đó, một thách thức cốt lõi khác là sự **phụ thuộc vào AI provider**. Mỗi nhà cung cấp (OpenAI, Anthropic, Google, các mô hình mã nguồn mở…) có API riêng, cách tính phí riêng, giới hạn ngữ cảnh khác nhau và hành vi khác nhau. Việc chuyển đổi giữa các provider, hoặc sử dụng đồng thời nhiều provider, đòi hỏi phải thay đổi mã nguồn tích hợp. Điều này khiến doanh nghiệp bị khóa vào một nhà cung cấp cụ thể (vendor lock-in), đặc biệt nguy hiểm khi giá cả hoặc chính sách thay đổi.

Đồng thời, sự ra đời của **giao thức Model Context Protocol (MCP)** vào năm 2024 bởi Anthropic đã chuẩn hóa cách thức AI Agent tương tác với các hệ thống và công cụ bên ngoài, mở ra một hướng tiếp cận mới: AI có thể trở thành "cầu nối thông minh" giữa người dùng và hệ thống mà không cần phải sửa đổi hệ thống gốc.

Xuất phát từ những vấn đề trên, việc **nghiên cứu và xây dựng một nền tảng AI doanh nghiệp (Enterprise AI Platform)** hướng tới tích hợp đa hệ thống là cần thiết và có ý nghĩa cả về mặt khoa học lẫn thực tiễn. Mục tiêu là tạo ra một **sứ mệnh rõ ràng**: *"Chỉ cần cắm AI vào hệ thống khác là có thể sử dụng được"* — nghĩa là doanh nghiệp có thể tích hợp AI vào ERP, CRM, DMS, HRM chỉ trong thời gian ngắn nhất, chi phí thấp nhất, độc lập với AI provider cụ thể và có khả năng mở rộng cao.

Đề tài không chỉ đóng góp vào việc ứng dụng các công nghệ AI hiện đại (LLM, RAG, AI Agent, MCP) vào thực tế doanh nghiệp mà còn xây dựng một mô hình kiến trúc có thể tham chiếu cho các dự án AI doanh nghiệp tại Việt Nam và khu vực.

## 2. Tổng quan về vấn đề nghiên cứu

### 2.1. Giới thiệu lĩnh vực nghiên cứu

Tích hợp AI vào hệ thống doanh nghiệp là một lĩnh vực nghiên cứu đang phát triển rất nhanh, nằm ở giao điểm của nhiều ngành: trí tuệ nhân tạo, hệ thống phân tán, tích hợp hệ thống, kiến trúc phần mềm và quản trị doanh nghiệp. Lĩnh vực này có thể được phân thành các hướng nghiên cứu chính:

- **Hệ thống gợi ý và trợ lý thông minh (Intelligent Assistant)**: sử dụng LLM kết hợp RAG để hỗ trợ tra cứu thông tin, hỏi đáp và đưa ra gợi ý dựa trên dữ liệu nội bộ doanh nghiệp.
- **AI Agent tự động hóa quy trình (Process Automation)**: sử dụng AI Agent có khả năng lập kế hoạch và gọi tool để tự động hóa các quy trình nghiệp vụ như phê duyệt đơn, tạo báo cáo, đồng bộ dữ liệu giữa các hệ thống.
- **Nền tảng AI đa thuê (Multi-tenant AI Platform)**: kiến trúc cho phép nhiều doanh nghiệp cùng sử dụng chung một nền tảng với cấu hình và bảo mật riêng biệt.
- **Chuẩn giao tiếp AI – Hệ thống**: như MCP, Agent-to-Agent Protocol, các chuẩn mở nhằm chuẩn hóa cách AI tương tác với hệ thống ngoài.

### 2.2. Các hướng tiếp cận chính

Trong nghiên cứu và triển khai thực tế hiện nay, có ba hướng tiếp cận chính cho bài toán tích hợp AI vào doanh nghiệp:

**Hướng 1: Tích hợp trực tiếp vào hệ thống hiện hữu (Embedded AI)**
Mỗi hệ thống (ERP, CRM…) tự tích hợp AI bằng cách gọi trực tiếp API của LLM provider. Ưu điểm là đơn giản cho từng hệ thống riêng lẻ, nhưng nhược điểm là chi phí nhân bản, khó chia sẻ tri thức giữa các hệ thống và dễ bị khóa vào provider.

**Hướng 2: Xây dựng nền tảng trung gian (Middleware / Integration Platform)**
Một nền tảng trung gian đặt giữa các hệ thống doanh nghiệp và các AI provider, cung cấp các khả năng như: chuẩn hóa giao tiếp (provider abstraction), truy xuất tri thức chung (RAG), điều phối nhiều agent (multi-agent orchestration). Đây là hướng tiếp cận của đề tài.

**Hướng 3: Sử dụng nền tảng AI thương mại SaaS**
Sử dụng các dịch vụ như Microsoft Copilot, Google Workspace AI, Salesforce Einstein. Ưu điểm là triển khai nhanh nhưng chi phí cao, khó tùy biến, phụ thuộc hoàn toàn vào nhà cung cấp và không phù hợp với các hệ thống tự xây dựng.

### 2.3. Khoảng trống nghiên cứu và vấn đề đặt ra

Mặc dù cả ba hướng trên đều có nghiên cứu và sản phẩm thương mại, vẫn còn những khoảng trống quan trọng:

- **Thiếu một kiến trúc tổng thể** kết hợp đồng thời LLM, RAG, AI Agent, MCP trong cùng một nền tảng có khả năng tích hợp với nhiều hệ thống doanh nghiệp khác nhau mà không phải viết lại từ đầu cho mỗi hệ thống.
- **Thiếu cơ chế "Plug and Play" thực sự**: hầu hết các giải pháp hiện nay vẫn yêu cầu can thiệp sâu vào hệ thống đích hoặc cấu hình phức tạp. Chưa có giải pháp nào đạt được mức "cắm vào là chạy" (zero config) ở mức tổng quát.
- **Vendor lock-in chưa được giải quyết triệt để**: các giải pháp thường gắn liền với một provider cụ thể hoặc một bộ công nghệ cụ thể. Khi muốn chuyển đổi, doanh nghiệp thường phải làm lại.
- **Thiếu chiến lược triển khai tiết kiệm tài nguyên AI trong khi vẫn phải đáp ứng yêu cầu doanh nghiệp**: nhiều công trình xây dựng kiến trúc lý thuyết đầy đủ nhưng thiếu hướng dẫn triển khai từng bước (Phase) để có thể áp dụng vào đội ngũ từ 20-100 lập trình viên.
- **Ứng dụng cho bối cảnh doanh nghiệp Việt Nam**: các nghiên cứu hiện có chủ yếu ở thị trường quốc tế. Cần một kiến trúc tham chiếu phù hợp với đặc thù hệ thống doanh nghiệp Việt Nam (quy mô vừa và nhỏ, hạ tầng tự xây, yêu cầu chạy cả offline lẫn hybrid).

Do đó, đề tài đặt ra vấn đề nghiên cứu: **Làm thế nào để xây dựng một nền tảng AI doanh nghiệp có khả năng tích hợp nhanh chóng vào nhiều hệ thống khác nhau, độc lập với AI provider, có khả năng mở rộng và đạt chuẩn doanh nghiệp về bảo mật, kiểm toán?**

## 3. Mục đích nghiên cứu

### 3.1. Mục tiêu tổng quát

Mục tiêu tổng quát của đề tài là nghiên cứu và xây dựng một nền tảng AI doanh nghiệp (Enterprise AI Platform) hướng tới tích hợp đa hệ thống (multi-system integration), cho phép doanh nghiệp "cắm AI vào" các hệ thống ERP, CRM, DMS, HRM, Workflow… một cách nhanh chóng, chi phí thấp, độc lập với AI provider cụ thể và có khả năng mở rộng cao. Nền tảng được thiết kế theo triết lý **"Plug & Play — Provider Agnostic — Domain Aware — Enterprise Grade"**, kết hợp đồng thời các công nghệ hiện đại gồm LLM, RAG, AI Agent, Multi-Agent Orchestration và giao thức MCP.

### 3.2. Mục tiêu cụ thể

Để đạt được mục tiêu tổng quát, đề tài đặt ra các mục tiêu cụ thể sau:

1. **Nghiên cứu cơ sở lý thuyết**: tổng hợp và phân tích các công nghệ nền tảng gồm LLM, RAG, Hybrid Search, Reranking, AI Agent, Multi-Agent Orchestration, giao thức MCP và các phương pháp tích hợp hệ thống doanh nghiệp, từ đó xác định khoảng trống nghiên cứu và xu hướng phát triển.

2. **Khảo sát thực tiễn**: nghiên cứu các nền tảng AI doanh nghiệp hiện có (cả thương mại và nghiên cứu), phân tích ưu nhược điểm và xác định yêu cầu thực tế khi áp dụng tại Việt Nam.

3. **Đề xuất kiến trúc nền tảng**: xây dựng một kiến trúc phân lớp (7-layer) gồm Presentation, AI Gateway, Agent Orchestration, AI Engine Core, Provider Abstraction, Integration và Infrastructure; trong đó tích hợp các công nghệ AI mới nhất vào một thiết kế thống nhất, có khả năng triển khai theo từng giai đoạn (phase).

4. **Phát triển hệ thống thực nghiệm**: triển khai nền tảng ở mức chạy được (working software) với các module cốt lõi: AI Provider Abstraction (hỗ trợ nhiều LLM), RAG Engine với Hybrid Search và Reranking, Agent Orchestration với Multi-Agent, MCP Server để tích hợp với hệ thống ngoài, cơ chế Plugin và bảo mật đa tenant.

5. **Đánh giá hiệu quả**: thực hiện các kịch bản đánh giá về (i) khả năng tích hợp đa hệ thống; (ii) chất lượng truy xuất tri thức (RAGAS: Context Relevance, Faithfulness, Answer Relevancy); (iii) khả năng phối hợp đa tác nhân; (iv) hiệu năng và khả năng mở rộng; (v) so sánh với các phương pháp truyền thống.

## 4. Đối tượng và phạm vi nghiên cứu

### 4.1. Đối tượng nghiên cứu

Đối tượng nghiên cứu của luận văn bao gồm:

- Các mô hình ngôn ngữ lớn (LLM) và đặc tính kỹ thuật phục vụ cho việc triển khai trong môi trường doanh nghiệp (multi-provider, latency, cost, safety).
- Kỹ thuật Retrieval-Augmented Generation (RAG), Hybrid Search, Reranking, Vector Database và các phương pháp nâng cao chất lượng truy xuất.
- Kiến trúc AI Agent, Multi-Agent Orchestration và các cơ chế lập kế hoạch – thực thi tác vụ (task planning, tool calling, workflow).
- Giao thức Model Context Protocol (MCP) và các chuẩn giao tiếp AI – hệ thống khác.
- Kiến trúc tích hợp hệ thống doanh nghiệp (Enterprise Integration Architecture), bao gồm: ERP, CRM, DMS, HRM, Workflow, BI, Database.
- Các yêu cầu doanh nghiệp về bảo mật (multi-tenancy, RBAC, audit log, secrets management) và khả năng mở rộng.

### 4.2. Phạm vi nghiên cứu

Luận văn tập trung nghiên cứu và xây dựng một **nền tảng kiến trúc tham chiếu (reference architecture)** cho AI doanh nghiệp, có khả năng tích hợp với đa dạng hệ thống hiện hữu. Phạm vi cụ thể:

- **Về công nghệ**: tập trung vào tích hợp LLM (đa provider), RAG, AI Agent/Multi-Agent, MCP, plugin SDK và các pattern kiến trúc phần mềm kèm theo. Không đi sâu vào huấn luyện mô hình từ đầu (pre-training, fine-tuning ở quy mô lớn).
- **Về lĩnh vực**: hệ thống doanh nghiệp (enterprise systems), bao gồm nhưng không giới hạn ở ERP, CRM, DMS, HRM, Workflow, BI. Có thể mở rộng sang các hệ thống chuyên ngành trong tương lai.
- **Về quy mô**: nền tảng được thiết kế cho quy mô từ doanh nghiệp nhỏ và vừa đến doanh nghiệp lớn; chạy được cả môi trường on-premise, private cloud lẫn hybrid cloud. Triển khai đa tenant từ 20 đến hàng ngàn người dùng trong một tenant.
- **Về mặt ngôn ngữ và bối cảnh**: hỗ trợ tiếng Việt là ngôn ngữ chính và tiếng Anh là ngôn ngữ bổ trợ, phù hợp với bối cảnh doanh nghiệp Việt Nam và khả năng mở rộng khu vực.
- **Về dữ liệu**: sử dụng dữ liệu mô phỏng và dữ liệu công khai phục vụ thực nghiệm, đánh giá. Các dữ liệu nội bộ riêng của từng doanh nghiệp chỉ được tích hợp khi có sự đồng ý và tuân thủ chính sách bảo mật.
- **Về thời gian**: nghiên cứu, xây dựng và đánh giá được thực hiện trong thời gian triển khai luận văn. Các công nghệ AI có tốc độ phát triển nhanh, do đó một số chi tiết kỹ thuật có thể thay đổi theo thời gian nhưng kiến trúc tổng thể vẫn giữ tính tham chiếu.

Ngoài phạm vi: nghiên cứu không đi sâu vào các bài toán ngoài doanh nghiệp (ví dụ: chatbot giải trí, sáng tạo nội dung thuần túy, game), cũng không xây dựng một LLM mới từ đầu. Nền tảng giả định sử dụng các LLM có sẵn (cả thương mại và mã nguồn mở).

## 5. Phương pháp nghiên cứu

Luận văn được thực hiện theo phương pháp nghiên cứu kết hợp giữa nghiên cứu lý thuyết và nghiên cứu thực nghiệm, nhằm đảm bảo vừa có cơ sở khoa học chặt chẽ vừa có tính ứng dụng thực tiễn cao.

### 5.1 Nghiên cứu cơ sở lý thuyết

- Thu thập, tổng hợp và phân tích các tài liệu khoa học, bài báo, sách, tài liệu kỹ thuật về LLM, RAG, AI Agent, MCP, kiến trúc tích hợp hệ thống doanh nghiệp và các pattern kiến trúc phần mềm hiện đại.
- Khảo sát các công trình nghiên cứu trong và ngoài nước nhằm xác định các hướng tiếp cận hiện có, những hạn chế và khoảng trống nghiên cứu cần giải quyết.
- Phân tích các tiêu chuẩn đánh giá hệ thống RAG (RAGAS), tiêu chuẩn đánh giá agent và các khung chất lượng phần mềm phù hợp.

### 5.2 Thu thập và xây dựng dữ liệu

- Thu thập tài liệu kỹ thuật, hướng dẫn sử dụng, đặc tả API của các LLM provider (OpenAI, Anthropic, Google, Ollama, Azure OpenAI…) phục vụ cho việc xây dựng lớp provider abstraction.
- Xây dựng bộ dữ liệu mô phỏng cho kho tri thức doanh nghiệp, bao gồm: tài liệu nội bộ mẫu, danh sách nhân viên, danh mục sản phẩm, quy trình nghiệp vụ và các tình huống hỏi đáp.
- Chuẩn hóa và làm sạch dữ liệu, tổ chức dưới dạng cơ sở tri thức có cấu trúc và phi cấu trúc phục vụ truy xuất.

### 5.3 Xây dựng hệ thống truy xuất tri thức

- Chuyển đổi dữ liệu văn bản thành vector embedding thông qua các mô hình embedding (Gemini Embedding, OpenAI Embedding, sentence-transformer…).
- Lưu trữ và truy xuất trên cơ sở dữ liệu vector (pgvector, Qdrant, Milvus hoặc các lựa chọn tương đương).
- Thiết kế cơ chế Hybrid Search kết hợp tìm kiếm ngữ nghĩa (dense) và tìm kiếm từ khóa (sparse), bổ sung Cross-Encoder Reranking và nén ngữ cảnh nhằm nâng cao chất lượng truy xuất.

### 5.4 Xây dựng mô hình thực nghiệm

- Phát triển nền tảng theo kiến trúc phân lớp đề xuất, triển khai các module cốt lõi: Provider Abstraction, AI Engine Core, Agent Orchestration, AI Gateway, Integration (MCP, Plugin) và Platform Core.
- Tích hợp nhiều LLM provider thông qua abstraction layer để chứng minh tính độc lập với nhà cung cấp.
- Tích hợp AI Agent với khả năng lập kế hoạch và gọi tool, thử nghiệm với các kịch bản nghiệp vụ thực tế (tạo đơn nghỉ phép, tra cứu khách hàng, tạo báo cáo…).
- Triển khai MCP Server cho phép hệ thống ngoài kết nối và tương tác với nền tảng.
- Triển khai môi trường thực nghiệm với Docker Compose, CI/CD cơ bản, đảm bảo reproducibility.

### 5.5 Đánh giá hệ thống

Đề tài sử dụng phương pháp đánh giá kết hợp định lượng và định tính:

**Đánh giá định lượng**:
- Đo lường chất lượng truy xuất tri thức thông qua bộ chỉ số RAGAS gồm: Context Relevance, Context Recall, Faithfulness, Answer Relevancy.
- Đo thời gian phản hồi (latency), thông lượng (throughput) của từng thành phần trong pipeline.
- Đo chi phí sử dụng token và hiệu quả tài nguyên trên các cấu hình khác nhau.

**Đánh giá định tính**:
- Đánh giá khả năng tích hợp đa hệ thống qua các kịch bản mô phỏng (một agent thao tác đồng thời trên ERP, CRM và DMS).
- Đánh giá khả năng phối hợp đa tác nhân (multi-agent) qua các tình huống phức tạp đòi hỏi nhiều bước và nhiều nguồn tri thức.
- So sánh kết quả giữa cấu hình có cá nhân hóa (dựa trên tenant, role, ngữ cảnh) và cấu hình mặc định.

### 5.6 Công cụ và môi trường nghiên cứu

- **Ngôn ngữ lập trình**: .NET 8 (C#) cho backend, TypeScript/Next.js cho frontend, Python cho các script tiện ích.
- **Mô hình ngôn ngữ lớn**: đa dạng gồm OpenAI GPT-4o, Anthropic Claude 3.5, Google Gemini 2.5, Ollama local (Llama 3.2, Mistral, DeepSeek).
- **Mô hình embedding**: Gemini Embedding, OpenAI text-embedding-3 và một số mô hình mã nguồn mở (BGE, mE5).
- **Cơ sở dữ liệu**: PostgreSQL 16 + pgvector, Redis (cache), Docker cho container hóa.
- **Công cụ khác**: Serilog (logging), OpenTelemetry (observability), Docker Compose, GitHub Actions (CI/CD cơ bản), Swagger/OpenAPI (mô tả API).
- **Môi trường phát triển**: Visual Studio Code, .NET CLI, Node.js, các extension hỗ trợ.
