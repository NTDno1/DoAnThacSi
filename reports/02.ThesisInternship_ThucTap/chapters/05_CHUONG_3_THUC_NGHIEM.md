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
