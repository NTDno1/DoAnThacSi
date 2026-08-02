# Chương 1: CƠ SỞ LÝ LUẬN VÀ TỔNG QUAN NGHIÊN CỨU

Chương này trình bày nền tảng lý thuyết và tổng quan nghiên cứu liên quan đến đề tài, bao gồm: (i) trí tuệ nhân tạo và mô hình ngôn ngữ lớn; (ii) kỹ thuật Retrieval-Augmented Generation; (iii) AI Agent và cơ chế phối hợp đa tác nhân; (iv) giao thức Model Context Protocol; (v) tích hợp hệ thống doanh nghiệp; (vi) khảo sát các nghiên cứu và hệ thống liên quan.

## 1.1. Tổng quan về trí tuệ nhân tạo và mô hình ngôn ngữ lớn

### 1.1.1. Khái quát về trí tuệ nhân tạo

Trí tuệ nhân tạo (Artificial Intelligence - AI) là lĩnh vực nghiên cứu và phát triển các hệ thống máy tính có khả năng thực hiện những tác vụ mà khi con người thực hiện đòi hỏi trí thông minh [1]. Lịch sử phát triển của AI trải qua nhiều giai đoạn với các trường phái khác nhau: AI biểu tượng (symbolic AI) trong những năm 1950-1980, AI kết nối (connectionism) với mạng nơ-ron nhân tạo, AI thống kê (statistical AI) từ những năm 1990, và gần đây là AI học sâu (deep learning) kết hợp với dữ liệu lớn.

Trong thập niên 2010-2020, AI đạt được những bước tiến đột phá nhờ sự kết hợp của ba yếu tố: (i) sức mạnh tính toán tăng theo cấp số nhân, đặc biệt với GPU và TPU chuyên dụng; (ii) khối lượng dữ liệu khổng lồ được tạo ra từ Internet, mạng xã hội và các thiết bị cảm biến; (iii) các thuật toán học sâu ngày càng tinh vi, đặc biệt là kiến trúc Transformer [2]. Sự kết hợp này đã tạo ra những mô hình có khả năng vượt trội so với con người trong nhiều tác vụ cụ thể: nhận diện hình ảnh, dịch máy, chơi cờ vây, chẩn đoán y tế…

Từ năm 2020, một hướng phát triển mới nổi lên mang tính cách mạng là các **mô hình ngôn ngữ lớn** (Large Language Models - LLM). Các mô hình này thể hiện khả năng tổng quát hóa đáng kinh ngạc trên nhiều tác vụ ngôn ngữ, từ viết văn, lập trình, phân tích dữ liệu đến lý luận logic, với chất lượng gần với con người. Sự ra đời của ChatGPT vào cuối năm 2022 đã đánh dấu một bước ngoặt lớn trong việc đưa AI đến với người dùng phổ thông và doanh nghiệp.

Tuy nhiên, các mô hình LLM cũng đặt ra nhiều thách thức mới về chi phí tính toán, an toàn dữ liệu, kiểm soát đầu ra và vấn đề "ảo giác" (hallucination). Do đó, việc nghiên cứu các kiến trúc cho phép khai thác sức mạnh của LLM một cách an toàn, hiệu quả và phù hợp với từng ngữ cảnh ứng dụng là một trong những hướng nghiên cứu quan trọng nhất hiện nay.

### 1.1.2. Mô hình ngôn ngữ lớn (LLM)

#### a. Khái niệm

Mô hình ngôn ngữ lớn (Large Language Model - LLM) là nhóm mô hình học sâu được huấn luyện trên khối lượng rất lớn dữ liệu văn bản, có khả năng hiểu và sinh ngôn ngữ tự nhiên ở mức độ gần với con người [3]. Hầu hết các LLM hiện đại đều dựa trên kiến trúc Transformer [2], cho phép mô hình đánh giá mức độ liên quan giữa các token trong toàn bộ chuỗi đầu vào thông qua cơ chế attention, thay vì xử lý tuần tự như các kiến trúc RNN hay LSTM trước đó. Điều này giúp cải thiện khả năng nắm bắt ngữ cảnh dài và tăng hiệu quả tính toán.

Một LLM thường có số lượng tham số (parameters) rất lớn, từ vài tỷ (VinaLlama 7B, Mistral 7B) đến hàng trăm tỷ tham số (GPT-4 ước tính khoảng 1.8 nghìn tỷ, Claude 3 Opus khoảng 2 nghìn tỷ, Llama 3.1 405B). Quy mô tham số này cho phép mô hình "lưu trữ" một lượng lớn tri thức về ngôn ngữ, sự kiện và kỹ năng lý luận.

Quá trình phát triển một LLM thường qua hai giai đoạn chính:
- **Pretraining**: mô hình học các đặc trưng ngôn ngữ tổng quát từ khối lượng lớn dữ liệu (thường là dữ liệu không gán nhãn), sử dụng các tác vụ như dự đoán token tiếp theo (next token prediction) hoặc masked language modeling. Giai đoạn này đòi hỏi tài nguyên tính toán rất lớn (hàng nghìn GPU trong nhiều tuần).
- **Fine-tuning**: sau khi pretraining, mô hình có thể được tinh chỉnh cho các tác vụ cụ thể (hỏi đáp, tóm tắt, phân loại, sinh nội dung) hoặc được căn chỉnh theo ý muốn con người thông qua RLHF (Reinforcement Learning from Human Feedback).

Ngoài ra, kỹ thuật **prompt engineering** cho phép sử dụng một LLM đã huấn luyện sẵn để thực hiện nhiều tác vụ khác nhau chỉ bằng cách thay đổi câu lệnh đầu vào, không cần fine-tuning. Đây là cách tiếp cận được đề tài sử dụng để giữ chi phí tích hợp thấp.

#### b. Các mô hình LLM tiêu biểu

##### (1) GPT (Generative Pre-trained Transformer)

GPT là họ mô hình của OpenAI dựa trên phần Decoder của Transformer, sinh văn bản theo cơ chế tự hồi quy bằng cách dự đoán token tiếp theo dựa trên các token đã xuất hiện trước đó [2], [4]. Các phiên bản GPT quy mô lớn (GPT-3.5, GPT-4, GPT-4o, GPT-4o mini) cho thấy khả năng thực hiện nhiều nhiệm vụ xử lý ngôn ngữ tự nhiên thông qua cách thiết kế đầu vào (prompt) phù hợp, đặc biệt trong bối cảnh few-shot learning và zero-shot learning.

Một số đặc điểm chính của GPT:
- Được huấn luyện trên khối lượng lớn dữ liệu văn bản đa ngôn ngữ.
- Có khả năng sinh văn bản mạch lạc theo ngữ cảnh dài.
- Hỗ trợ tool calling (function calling) cho phép kết nối với hệ thống ngoài.
- Cửa sổ ngữ cảnh (context window) ngày càng được mở rộng: từ 4K (GPT-3.5), 8K, 16K, 32K, 128K (GPT-4 Turbo) đến 1M token (GPT-4.1).

##### (2) Claude (Anthropic)

Claude là họ mô hình do Anthropic phát triển, nổi bật với khả năng xử lý ngữ cảnh dài (200K token trở lên), cửa sổ ngữ cảnh rất lớn (Claude 3.5 Sonnet hỗ trợ 200K, Claude 4 hỗ trợ đến 1M), và có năng lực lý luận (reasoning) tốt. Đặc biệt, Anthropic là đơn vị khởi xướng giao thức Model Context Protocol (MCP), đây là chuẩn mở cho phép kết nối AI với các công cụ và nguồn dữ liệu. Claude đặc biệt phù hợp cho các tác vụ đòi hỏi phân tích sâu và tuân thủ an toàn.

##### (3) Gemini (Google)

Gemini là mô hình ngôn ngữ lớn đa phương thức do Google DeepMind phát triển, có khả năng xử lý nhiều dạng dữ liệu (văn bản, hình ảnh, âm thanh, video) trong cùng một kiến trúc [5]. Các phiên bản Gemini 1.5 và 2.5 có cửa sổ ngữ cảnh rất lớn (1M đến 2M token), phù hợp cho các tác vụ cần phân tích nhiều tài liệu. Gemini cũng cung cấp mô hình embedding chất lượng cao (Gemini Embedding) tích hợp tốt với hệ thống truy xuất.

##### (4) Llama (Meta) và họ mô hình mã nguồn mở

Llama là họ mô hình mã nguồn mở do Meta phát triển. Các phiên bản gần đây (Llama 3, Llama 3.1, Llama 3.2, Llama 3.3) đạt chất lượng gần các mô hình thương mại. Ưu điểm của Llama và các mô hình mã nguồn mở là có thể tự triển khai trên hạ tầng riêng (on-premise), giảm chi phí và bảo vệ dữ liệu. Thông qua Ollama, vLLM, llama.cpp, các công cụ này có thể chạy trên một máy chủ có GPU phù hợp.

Các mô hình mã nguồn mở khác đáng chú ý:
- **Mistral** (Mistral AI): các phiên bản 7B, 8x7B (MoE), Mistral Large.
- **DeepSeek** (Trung Quốc): DeepSeek-V2, DeepSeek-V3, có chất lượng cao với chi phí thấp.
- **Qwen** (Alibaba): Qwen 2, Qwen 2.5 với nhiều kích thước.
- **VinaLlama** và **PhoGPT** (Việt Nam): các mô hình được tối ưu cho tiếng Việt.

#### c. Đặc điểm kỹ thuật phục vụ doanh nghiệp

Khi triển khai LLM trong môi trường doanh nghiệp, cần quan tâm đến các đặc điểm:

- **Chi phí sử dụng**: mỗi provider có chính sách giá khác nhau, tính theo token đầu vào/đầu ra. Cần cơ chế cache, lựa chọn model phù hợp cho từng tác vụ.
- **Latency**: thời gian phản hồi ảnh hưởng đến trải nghiệm người dùng, đặc biệt trong các ứng dụng thời gian thực.
- **Quota và Rate limit**: mỗi provider giới hạn số request mỗi phút. Hệ thống cần cơ chế retry, queue, fallback.
- **Privacy và Data residency**: doanh nghiệp có thể yêu cầu dữ liệu không rời khỏi hạ tầng (châu Âu GDPR, y tế, tài chính).
- **Reproducibility**: kết quả có thể thay đổi giữa các lần gọi do temperature > 0. Cần chiến lược đảm bảo tính ổn định.

### 1.1.3. Hạn chế của LLM trong bài toán doanh nghiệp

Mặc dù LLM mang lại nhiều bước tiến quan trọng, các mô hình này vẫn tồn tại nhiều hạn chế khi áp dụng vào hệ thống doanh nghiệp [6]:

**(1) Hiện tượng ảo giác (hallucination)**: mô hình có thể tạo ra thông tin nghe có vẻ hợp lý nhưng không chính xác hoặc không có căn cứ. Trong bối cảnh doanh nghiệp, điều này đặc biệt nguy hiểm vì có thể dẫn đến quyết định sai lầm, thông tin khách hàng sai, hoặc vi phạm quy định.

**(2) Phụ thuộc vào dữ liệu huấn luyện**: chất lượng, phạm vi và thời điểm thu thập dữ liệu ảnh hưởng trực tiếp đến độ chính xác. Các mô hình có thể không biết về sản phẩm mới, chính sách nội bộ, hoặc thông tin cụ thể của doanh nghiệp.

**(3) Không tự cập nhật tri thức**: LLM không tự động nắm bắt các thông tin mới phát sinh sau thời điểm huấn luyện nếu không có cơ chế bổ sung dữ liệu bên ngoài. Trong doanh nghiệp, thông tin thay đổi liên tục (giá sản phẩm, tồn kho, chính sách nhân sự…).

**(4) Chi phí tính toán cao**: việc huấn luyện và triển khai LLM đòi hỏi tài nguyên phần cứng lớn. Chi phí sử dụng API thương mại cũng tăng theo quy mô.

**(5) Hạn chế khi xử lý ngữ cảnh dài**: mô hình có thể gặp khó khăn khi đầu vào chứa nhiều tài liệu hoặc thông tin cần liên kết qua nhiều đoạn văn bản, mặc dù các phiên bản mới đã cải thiện đáng kể.

**(6) Bảo mật và quyền riêng tư**: khi sử dụng API thương mại, dữ liệu của doanh nghiệp có thể được gửi đến server bên thứ ba, gây lo ngại về bảo mật.

**(7) Chi phí tích hợp cao**: mỗi hệ thống, mỗi use case thường cần code tích hợp riêng, dẫn đến nhân bản công sức.

Do đó, trong các hệ thống doanh nghiệp, LLM thường được kết hợp với các kỹ thuật bổ trợ: **Retrieval-Augmented Generation** (RAG) để cung cấp tri thức cập nhật; **AI Agent** kết hợp **tool calling** để thực hiện tác vụ; **MCP** để chuẩn hóa giao tiếp với hệ thống ngoài. Các kỹ thuật này được trình bày trong các mục tiếp theo.

## 1.2. Retrieval-Augmented Generation (RAG)

### 1.2.1. Khái niệm và nguyên lý hoạt động

Retrieval-Augmented Generation (RAG) là phương pháp kết hợp mô hình ngôn ngữ lớn với cơ chế truy xuất thông tin từ nguồn dữ liệu bên ngoài [7], [8]. Thay vì phụ thuộc hoàn toàn vào tri thức được học trong quá trình huấn luyện, mô hình có thể tham chiếu thêm các tài liệu liên quan tại thời điểm xử lý truy vấn để hỗ trợ tạo câu trả lời.

Về bản chất, RAG tách biệt tương đối giữa khả năng sinh ngôn ngữ của LLM và nguồn tri thức được sử dụng. Tri thức có thể được lưu trữ, cập nhật và truy xuất từ bên ngoài mô hình, qua đó giúp hệ thống linh hoạt hơn trong các bài toán yêu cầu thông tin mới, thông tin chuyên ngành hoặc dữ liệu thay đổi theo thời gian.

Quy trình tổng quát của một hệ thống RAG gồm bốn bước chính:

**Bước 1: Tiếp nhận truy vấn**
Người dùng gửi câu hỏi hoặc yêu cầu. Truy vấn có thể được chuẩn hóa, bổ sung ngữ cảnh và chuyển sang dạng phù hợp để phục vụ quá trình tìm kiếm (ví dụ: tạo embedding, tách thực thể).

**Bước 2: Truy xuất tài liệu liên quan**
Retriever thực hiện tìm kiếm trong kho tri thức dựa trên nội dung truy vấn. Hệ thống lựa chọn các tài liệu hoặc đoạn văn bản có mức độ liên quan cao nhất. Kết quả truy xuất thường là một tập các đoạn thông tin ngắn phục vụ cho bước sinh câu trả lời. Có hai hướng truy xuất chính:
- **Dense Retrieval**: sử dụng mô hình embedding ánh xạ truy vấn và tài liệu vào cùng không gian vector; mức độ liên quan đo bằng cosine similarity.
- **Sparse Retrieval**: sử dụng thuật toán TF-IDF, BM25 [9]; đánh giá mức độ liên quan dựa trên tần suất từ khóa.

**Bước 3: Xây dựng ngữ cảnh (Augmented Context)**
Các tài liệu được truy xuất được ghép với truy vấn ban đầu và các thông tin bổ sung (hồ sơ người dùng, dữ liệu có cấu trúc, thông tin ngữ cảnh khác) để tạo thành ngữ cảnh mở rộng. Giai đoạn này có thể áp dụng các kỹ thuật nén (context compression) nhằm giảm kích thước ngữ cảnh và giữ lại phần liên quan nhất.

**Bước 4: Sinh câu trả lời (Generation)**
LLM sử dụng truy vấn cùng với ngữ cảnh mở rộng để tạo ra câu trả lời. Câu trả lời được xây dựng dựa trên cả kiến thức nội tại của mô hình và thông tin được truy xuất từ kho tri thức. Nhờ đó, đầu ra thường có độ chính xác và tính cập nhật cao hơn so với cách tiếp cận chỉ sử dụng LLM độc lập.

### 1.2.2. Kiến trúc nâng cao của RAG

Hệ thống RAG trong nghiên cứu và triển khai hiện đại thường được mở rộng với nhiều thành phần:

- **Query Understanding**: phân tích truy vấn, trích xuất thực thể, mở rộng truy vấn (query expansion, query rewriting) để cải thiện chất lượng truy xuất.
- **Hybrid Search**: kết hợp dense và sparse retrieval thông qua các thuật toán như Reciprocal Rank Fusion (RRF) để tận dụng ưu điểm bổ trợ.
- **Reranking**: sử dụng mô hình học sâu (Cross-Encoder) để đánh giá lại mức độ liên quan của từng tài liệu sau bước truy xuất ban đầu, sắp xếp lại thứ tự ưu tiên.
- **Context Compression**: nén ngữ cảnh truy xuất nhằm giảm kích thước đầu vào, loại bỏ thông tin nhiễu, tăng hiệu quả xử lý.
- **Structured Output**: buộc LLM sinh kết quả theo cấu trúc JSON/schema định sẵn để dễ dàng tích hợp với hệ thống.

```
┌─────────────────────────────────────────────────────────────┐
│                    RAG nâng cao                             │
│                                                             │
│  ┌───────────┐    ┌───────────────┐    ┌──────────────┐    │
│  │ Query     │ -> │ Hybrid Search │ -> │ Reranking    │    │
│  │ Rewriting │    │ (Dense+Sparse)│    │ (Cross-Enc.) │    │
│  └───────────┘    └───────────────┘    └──────┬───────┘    │
│        │                                       │            │
│        ▼                                       ▼            │
│  ┌────────────┐    ┌───────────────┐    ┌──────────────┐    │
│  │ Structured │ <- │ LLM Generator │ <- │ Context      │    │
│  │ Output     │    │ (Gemini/Claude│    │ Compression  │    │
│  │            │    │ /GPT/Llama)   │    │              │    │
│  └────────────┘    └───────────────┘    └──────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

### 1.2.3. Vai trò của RAG trong hệ thống doanh nghiệp

Trong các hệ thống ứng dụng thực tế, RAG đóng vai trò quan trọng trong việc mở rộng khả năng của LLM vượt ra ngoài giới hạn tri thức nội tại của mô hình [7], [8]. Đối với doanh nghiệp, RAG mang lại một số lợi ích chính:

- **Bổ sung và cập nhật tri thức liên tục**: thông qua kho dữ liệu bên ngoài mà không cần huấn luyện lại toàn bộ mô hình.
- **Tăng khả năng kiểm soát và truy vết nguồn gốc thông tin**: hệ thống có thể chỉ ra tài liệu nào đã được sử dụng, phục vụ tuân thủ và kiểm toán.
- **Hỗ trợ cho các bài toán theo miền chuyên biệt**: nhờ khả năng tích hợp dữ liệu riêng của từng doanh nghiệp.
- **Tiết kiệm chi phí so với fine-tuning**: trong nhiều trường hợp, việc tinh chỉnh một LLM lớn rất tốn kém, RAG cho phép đạt được kết quả tương đương với chi phí thấp hơn nhiều.

### 1.2.4. Ưu điểm và hạn chế của RAG

**Bảng 1.1. Ưu điểm và hạn chế của RAG**

| Ưu điểm | Hạn chế |
|---|---|
| Mở rộng khả năng sử dụng tri thức ngoài mô hình, giảm phụ thuộc vào tham số đã huấn luyện | Phụ thuộc đáng kể vào chất lượng, độ chính xác và mức độ cập nhật của nguồn dữ liệu truy xuất |
| Cho phép cập nhật thông tin thông qua kho dữ liệu bên ngoài mà không cần huấn luyện lại mô hình | Hiệu quả đầu ra bị ảnh hưởng nếu bộ truy xuất chọn sai, thiếu hoặc nhiễu thông tin liên quan |
| Hỗ trợ tốt các bài toán theo miền nhờ khả năng tích hợp dữ liệu chuyên biệt | Làm tăng độ phức tạp trong thiết kế, triển khai và vận hành hệ thống so với LLM thuần túy |
| Giúp phản hồi bám sát hơn vào yêu cầu người dùng và tài liệu được truy xuất | Tăng thời gian xử lý do phát sinh thêm bước truy xuất trước khi sinh kết quả |
| Tăng khả năng kiểm soát và truy vết nguồn thông tin đầu ra | Cần thiết kế tốt cơ chế thu thập, tổ chức, chia đoạn và biểu diễn dữ liệu để đạt hiệu quả cao |

Trong bối cảnh doanh nghiệp, RAG thường được kết hợp với các kỹ thuật tăng cường (Hybrid Search, Reranking, Compression) và được nhúng vào pipeline AI Agent để tạo ra các hệ thống thông minh có khả năng tự động hóa quy trình nghiệp vụ.

## 1.3. AI Agent và cơ chế phối hợp đa tác nhân (Multi-Agent)

### 1.3.1. Khái niệm AI Agent

AI Agent là một hệ thống phần mềm có khả năng **tự chủ cảm nhận môi trường, lập kế hoạch, ra quyết định và thực hiện hành động** để đạt được mục tiêu được giao, thông qua việc kết hợp LLM với các công cụ (tools) và bộ nhớ (memory) [10], [11].

Trong ngữ cảnh LLM, một AI Agent thường gồm các thành phần:

- **LLM làm "bộ não"**: phân tích yêu cầu, lập kế hoạch và quyết định hành động tiếp theo.
- **Tools (công cụ)**: các hàm API mà agent có thể gọi, ví dụ: tra cứu cơ sở dữ liệu, gọi API hệ thống ngoài, tạo báo cáo.
- **Memory (bộ nhớ)**: lưu trữ ngữ cảnh hội thoại, kết quả trung gian, sở thích người dùng.
- **Planning**: khả năng phân tách tác vụ lớn thành các bước nhỏ và xử lý tuần tự hoặc song song.
- **Reflection / Self-critique**: khả năng tự đánh giá kết quả và điều chỉnh.

Các framework AI Agent phổ biến gồm: LangChain/LangGraph (Python, JS), AutoGen (Microsoft), CrewAI, LlamaIndex Agents, Semantic Kernel (.NET). Trong .NET, Semantic Kernel của Microsoft cung cấp abstractions cho AI Agent, function calling, planner và memory.

### 1.3.2. Kiến trúc tác nhân và cơ chế lập kế hoạch

Một kiến trúc agent phổ biến là ReAct (Reason + Act), trong đó agent luân phiên giữa hai bước:
- **Reason**: suy nghĩ về tình huống hiện tại, lên kế hoạch hành động tiếp theo.
- **Act**: thực hiện hành động (gọi tool, truy vấn dữ liệu).

Một số agent nâng cao hơn sử dụng:
- **Plan-and-Execute**: lập kế hoạch toàn bộ trước, sau đó thực thi tuần tự.
- **Multi-step Reasoning**: chain-of-thought, tree-of-thought để giải quyết vấn đề phức tạp.
- **Reflection**: tự đánh giá và sửa chữa kết quả.

### 1.3.3. Multi-Agent Orchestration

Multi-Agent System (MAS) là hệ thống gồm nhiều agent phối hợp để giải quyết vấn đề phức tạp. Mỗi agent có vai trò chuyên biệt (researcher, planner, executor, critic, …) và giao tiếp với nhau thông qua message passing hoặc chia sẻ context chung.

Các mô hình phối hợp phổ biến:

- **Supervisor Pattern**: một agent "supervisor" điều phối, các agent con xử lý tác vụ con.
- **Hierarchical Pattern**: agent cấp cao phân tách, agent cấp thấp thực thi.
- **Collaborative Pattern**: các agent trao đổi ngang hàng để đạt đồng thuận.
- **Pipeline Pattern**: agent A xử lý xong, chuyển output cho agent B tiếp tục.

Trong doanh nghiệp, Multi-Agent rất phù hợp cho các quy trình nghiệp vụ phức tạp: ví dụ một workflow "phê duyệt đơn nghỉ phép" có thể gồm: agent phân tích đơn → agent tra cứu HRM (số ngày phép còn lại) → agent gọi API để tạo ticket → agent gửi thông báo cho quản lý.

### 1.3.4. Ưu điểm và thách thức

**Ưu điểm**:
- Giải quyết được các tác vụ phức tạp đa bước.
- Phân tách trách nhiệm rõ ràng giữa các agent.
- Dễ mở rộng (thêm agent mới khi có nhu cầu mới).
- Tận dụng được tool calling để tương tác với hệ thống ngoài.

**Thách thức**:
- **Chi phí**: nhiều agent có thể dẫn đến chi phí token cao.
- **Latency**: thời gian phản hồi có thể tăng do nhiều bước LLM.
- **Error propagation**: lỗi ở agent sớm có thể lan truyền.
- **Debugging**: khó truy vết khi có nhiều agent phối hợp.
- **Đảm bảo tính an toàn**: cần guardrail, human-in-the-loop cho các quyết định quan trọng.

Trong đề tài, Multi-Agent Orchestration được thiết kế kết hợp với MCP để tạo ra một hệ thống có khả năng phối hợp chặt chẽ với các hệ thống doanh nghiệp hiện hữu.

## 1.4. Giao thức Model Context Protocol (MCP)

### 1.4.1. Bối cảnh ra đời của MCP

Model Context Protocol (MCP) là một giao thức mở được Anthropic giới thiệu vào tháng 11 năm 2024 nhằm chuẩn hóa cách thức các mô hình AI (đặc biệt là LLM Agent) tương tác với các công cụ, dịch vụ và nguồn dữ liệu bên ngoài [12]. Trước khi MCP ra đời, mỗi framework AI Agent thường có cách tích hợp tools riêng, dẫn đến tình trạng phân mảnh: tool của framework A không sử dụng được cho framework B, doanh nghiệp muốn tích hợp tool mới phải code lại.

MCP giải quyết vấn đề này bằng cách định nghĩa một giao thức client-server chuẩn, tương tự như LSP (Language Server Protocol) trong lập trình:

- **MCP Server**: cung cấp các tool, resource và prompt qua giao diện chuẩn.
- **MCP Client**: có thể là AI Agent (Claude Desktop, Cursor IDE, các AI framework khác) kết nối tới MCP Server để sử dụng các khả năng được cung cấp.

Giao thức MCP sử dụng JSON-RPC 2.0 làm định dạng trao đổi và hỗ trợ hai cơ chế giao tiếp chính: stdio (cho local server) và HTTP+SSE (cho remote server).

### 1.4.2. Kiến trúc và cách thức hoạt động

Kiến trúc MCP gồm ba lớp chính:

**Lớp 1: MCP Host** là ứng dụng AI (ví dụ: Claude Desktop, IDE, AI framework) muốn sử dụng các tool.

**Lớp 2: MCP Client** được host nhúng vào, có nhiệm vụ kết nối và giao tiếp với một hoặc nhiều MCP Server.

**Lớp 3: MCP Server** cung cấp ba loại khả năng:
- **Resources**: dữ liệu mà LLM có thể đọc (file, database records, API responses).
- **Tools**: hàm mà LLM có thể gọi để thực hiện hành động (tạo ticket, gửi email…).
- **Prompts**: template prompt được định nghĩa sẵn cho các tác vụ cụ thể.

```
┌────────────────────────────────────────────────────┐
│                  MCP Architecture                   │
│                                                     │
│  ┌────────────────────┐      ┌─────────────────┐  │
│  │   MCP Host         │      │   MCP Server    │  │
│  │   (AI Assistant)   │      │   - Resources   │  │
│  │  ┌──────────────┐  │      │   - Tools       │  │
│  │  │ MCP Client   │◄─┼──────┤   - Prompts     │  │
│  │  │ (JSON-RPC)   │  │ JSON │                 │  │
│  │  └──────────────┘  │ RPC  │   Connect to:   │  │
│  └────────────────────┘      │   - Database    │  │
│                              │   - File system │  │
│                              │   - ERP/CRM API │  │
│                              └─────────────────┘  │
└────────────────────────────────────────────────────┘
```

### 1.4.3. MCP trong bài toán tích hợp hệ thống doanh nghiệp

Đối với doanh nghiệp, MCP mang lại giá trị đặc biệt lớn:

**(1) Chuẩn hóa giao tiếp AI – hệ thống**: thay vì mỗi agent phải tích hợp riêng với từng hệ thống, doanh nghiệp chỉ cần triển khai một MCP Server cho mỗi hệ thống (ERP, CRM, DMS). Sau đó, bất kỳ AI Agent nào hỗ trợ MCP đều có thể kết nối.

**(2) Tái sử dụng**: tool khai thác từ MCP Server có thể được sử dụng bởi nhiều agent khác nhau, nhiều workflow khác nhau, thậm chí nhiều ứng dụng khác nhau.

**(3) Bảo mật**: MCP Server đặt tại hạ tầng doanh nghiệp, có thể áp dụng các chính sách kiểm soát truy cập, logging, audit. Dữ liệu nhạy cảm không cần gửi ra ngoài.

**(4) Khả năng mở rộng**: khi doanh nghiệp muốn tích hợp hệ thống mới, chỉ cần triển khai MCP Server tương ứng. Không cần sửa đổi AI Agent.

Đề tài sử dụng MCP như một cơ chế cốt lõi để nền tảng Enterprise AI có thể "Plug & Play" vào các hệ thống doanh nghiệp.

## 1.5. Tích hợp hệ thống doanh nghiệp (Enterprise System Integration)

### 1.5.1. Tổng quan về ERP, CRM, DMS, HRM

**Enterprise Resource Planning (ERP)**: hệ thống hoạch định nguồn lực doanh nghiệp, tích hợp các quy trình cốt lõi như tài chính, kế toán, mua hàng, sản xuất, kho vận. Các sản phẩm phổ biến: SAP, Oracle ERP, Microsoft Dynamics, Odoo, ERPNext.

**Customer Relationship Management (CRM)**: hệ thống quản lý quan hệ khách hàng, hỗ trợ quản lý leads, opportunities, sales pipeline, marketing automation. Ví dụ: Salesforce, HubSpot, Zoho CRM, Microsoft Dynamics CRM, custom CRM nội bộ.

**Document Management System (DMS)**: hệ thống quản lý tài liệu, lưu trữ, phiên bản hóa, chia sẻ và tìm kiếm. Ví dụ: SharePoint, Alfresco, Nextcloud, hệ thống DMS nội bộ.

**Human Resource Management (HRM)**: hệ thống quản lý nhân sự gồm hồ sơ nhân viên, chấm công, tính lương, đánh giá. Ví dụ: Workday, BambooHR, custom HRM.

**Workflow / BPM**: hệ thống quản lý quy trình nghiệp vụ (Business Process Management), điều phối luồng công việc giữa nhiều bên. Ví dụ: Camunda, jBPM, Activiti, custom BPM.

**Business Intelligence (BI)**: hệ thống phân tích và báo cáo, ví dụ Power BI, Tableau, Metabase.

**Database trực tiếp**: nhiều hệ thống không có API chuẩn mà chỉ có database; tích hợp qua query SQL trực tiếp.

### 1.5.2. Các phương pháp tích hợp truyền thống

Trước khi AI trở nên phổ biến, doanh nghiệp đã có nhiều phương pháp tích hợp hệ thống:

- **Point-to-Point**: hai hệ thống kết nối trực tiếp, đơn giản cho cặp đôi nhưng khó mở rộng.
- **Hub-and-Spoke**: một hub trung gian, các hệ thống kết nối vào hub. Ví dụ: ESB (Enterprise Service Bus).
- **Middleware/ESB**: MuleSoft, IBM Integration Bus, WSO2.
- **API Gateway**: Kong, Apigee, AWS API Gateway.
- **Message Queue**: Kafka, RabbitMQ, ActiveMQ dùng cho tác vụ bất đồng bộ.

Các phương pháp này vẫn được sử dụng rộng rãi, đặc biệt khi tích hợp giữa các hệ thống nghiệp vụ truyền thống. Tuy nhiên, chúng **chưa giải quyết được bài toán tích hợp AI**: cần một lớp khác có khả năng hiểu ngôn ngữ tự nhiên, lập kế hoạch và gọi tool.

### 1.5.3. Thách thức khi tích hợp AI vào hệ thống doanh nghiệp

Khi tích hợp AI vào hệ thống doanh nghiệp, gặp phải các thách thức:

**(1) Đa dạng hệ thống và API**: mỗi hệ thống có API riêng, schema khác nhau, cần abstraction để AI Agent giao tiếp thống nhất.

**(2) Quyền truy cập (Authorization)**: AI không thể có mọi quyền; cần cơ chế phân quyền chi tiết (RBAC/ABAC), giới hạn tool theo role.

**(3) Audit và Compliance**: mọi hành động của AI cần được ghi log để phục vụ kiểm toán và tuân thủ (GDPR, HIPAA, ISO 27001).

**(4) Đảm bảo chất lượng đầu ra**: AI có thể sinh ra câu trả lời sai; cần Human-in-the-Loop cho quyết định quan trọng.

**(5) Chi phí & hiệu năng**: việc gọi LLM nhiều lần tốn kém và chậm; cần cache, batching và chọn model phù hợp.

**(6) Bảo mật & quyền riêng tư**: dữ liệu nội bộ khi qua LLM có thể bị rò rỉ; cần giải pháp on-premise hoặc private cloud.

**(7) Đồng bộ hóa tri thức**: khi doanh nghiệp thay đổi quy trình, sản phẩm, kho tri thức phải được cập nhật.

Đề tài đề xuất một kiến trúc giải quyết đồng thời các thách thức trên, với MCP làm chuẩn giao tiếp, RAG làm cơ chế truy xuất tri thức và AI Gateway làm lớp kiểm soát bảo mật.

## 1.6. Các nghiên cứu và hệ thống liên quan

### 1.6.1. Các nền tảng AI doanh nghiệp hiện có

Hiện nay có một số nền tảng AI doanh nghiệp đáng chú ý:

**Microsoft Copilot Studio**: cho phép doanh nghiệp tạo các "Copilot" tùy chỉnh tích hợp với Microsoft 365, Dynamics 365, Power Platform. Ưu điểm: hệ sinh thái Microsoft rộng, bảo mật tốt. Hạn chế: gắn liền với Microsoft stack, chi phí cao.

**Google Vertex AI Agent Builder**: cho phép xây dựng agent tích hợp với Google Cloud, hỗ trợ nhiều LLM. Ưu điểm: linh hoạt, tích hợp GCP. Hạn chế: cần GCP.

**Amazon Bedrock Agents**: tương tự Vertex AI nhưng trên AWS. Hỗ trợ nhiều foundation model.

**Salesforce Einstein**: AI cho CRM Salesforce. Tích hợp chặt với Salesforce.

**LangChain / LlamaIndex**: framework mã nguồn mở, cung cấp primitives để xây dựng agent.

**Bảng 1.2. So sánh một số nền tảng AI doanh nghiệp hiện có**

| Nền tảng | Đặc điểm | Hạn chế |
|---|---|---|
| Microsoft Copilot Studio | Tích hợp chặt với Microsoft 365, Dynamics; bảo mật cấp doanh nghiệp | Vendor lock-in Microsoft; chi phí license cao; khó tích hợp hệ thống ngoài hệ sinh thái |
| Google Vertex AI Agent Builder | Hỗ trợ đa model, tích hợp GCP, RAG built-in | Cần Google Cloud; giá cao; ít tooling cho môi trường on-premise |
| Amazon Bedrock Agents | Đa model foundation, tích hợp AWS, knowledge base | Cần AWS; phức tạp cho người mới |
| LangChain (mã nguồn mở) | Framework linh hoạt, cộng đồng lớn | Không phải sản phẩm hoàn chỉnh; tự triển khai mọi thứ |
| Salesforce Einstein | AI native cho CRM Salesforce | Gắn liền Salesforce; không tổng quát |

### 1.6.2. Các nghiên cứu về AI Agent và Multi-Agent

Trong lĩnh vực nghiên cứu, các công trình gần đây đáng chú ý:

- **AutoGen (Microsoft Research)**: framework Multi-Agent hỗ trợ hội thoại giữa các agent.
- **CrewAI**: framework tổ chức "crew" các agent theo vai trò.
- **LangGraph**: mở rộng LangChain cho workflow đồ thị.
- **Semantic Kernel (Microsoft)**: SDK cho .NET để xây dựng AI Agent tích hợp với C# / .NET.

**Bảng 1.3. Một số nghiên cứu ứng dụng AI Agent trong doanh nghiệp**

| Nghiên cứu / Framework | Hướng tiếp cận | Hạn chế |
|---|---|---|
| AutoGen [13] | Multi-Agent Conversation; phối hợp qua messages | Chưa có chuẩn MCP chính thức; khó debug khi agent nhiều |
| CrewAI [14] | Role-based Multi-Agent (researcher, writer…) | Giới hạn về workflow phức tạp |
| LangGraph | Workflow đồ thị có trạng thái, hỗ trợ ReAct, Reflection | Phụ thuộc LangChain; learning curve |
| Semantic Kernel (.NET) | Function calling, planner, memory cho .NET | Cộng đồng nhỏ hơn Python ecosystem |

### 1.6.3. Khoảng trống nghiên cứu

Qua khảo sát, có thể nhận thấy các khoảng trống nghiên cứu sau:

**(1) Thiếu kiến trúc tổng thể tích hợp nhiều công nghệ AI**: các nghiên cứu thường tập trung vào một khía cạnh (RAG, agent, tool calling). Chưa có nhiều công trình đề xuất kiến trúc thống nhất, triển khai được thực tế cho doanh nghiệp.

**(2) Thiếu hỗ trợ chuẩn MCP ở mức production-grade**: MCP ra đời 2024, nhiều framework đang trong quá trình tích hợp. Một kiến trúc hoàn chỉnh kết hợp MCP với các thành phần khác còn hạn chế.

**(3) Thiếu hướng dẫn triển khai từng bước (Phase)**: hầu hết tài liệu dừng ở mức kiến trúc, rất ít tài liệu hướng dẫn cụ thể cách triển khai từ prototype đến production với quy mô team 20-100 người.

**(4) Thiếu đánh giá trên bối cảnh doanh nghiệp Việt Nam**: các công trình chủ yếu ở Mỹ, châu Âu, Trung Quốc. Bối cảnh Việt Nam với đặc thù tiếng Việt, văn hóa doanh nghiệp, hạ tầng còn hạn chế chưa được nghiên cứu sâu.

**(5) Thiếu giải pháp "Plug and Play" thực sự cho doanh nghiệp nhỏ và vừa**: các giải pháp thương mại đòi hỏi đầu tư lớn, các framework mã nguồn mở đòi hỏi đội ngũ kỹ thuật cao.

Đề tài nhằm giải quyết các khoảng trống trên bằng cách đề xuất một kiến trúc Enterprise AI Platform hoàn chỉnh, có hướng dẫn triển khai từng phase, tích hợp MCP, RAG và AI Agent, phù hợp với doanh nghiệp Việt Nam.

## 1.7. Kết luận chương 1

Chương 1 đã trình bày nền tảng lý thuyết và tổng quan nghiên cứu của luận văn, bao gồm:

- Tổng quan về trí tuệ nhân tạo và mô hình ngôn ngữ lớn (LLM): khái niệm, các mô hình tiêu biểu (GPT, Claude, Gemini, Llama) và hạn chế khi áp dụng cho doanh nghiệp.
- Kỹ thuật Retrieval-Augmented Generation (RAG): khái niệm, kiến trúc tổng quát, kiến trúc nâng cao với Hybrid Search, Reranking, Context Compression và vai trò trong hệ thống doanh nghiệp.
- AI Agent và Multi-Agent Orchestration: khái niệm AI Agent, kiến trúc ReAct/Plan-and-Execute, các mô hình phối hợp Multi-Agent.
- Giao thức Model Context Protocol (MCP): bối cảnh ra đời, kiến trúc client-server và vai trò trong tích hợp hệ thống doanh nghiệp.
- Tích hợp hệ thống doanh nghiệp: tổng quan về ERP, CRM, DMS, HRM; các phương pháp tích hợp truyền thống; thách thức khi tích hợp AI.
- Khảo sát các nghiên cứu và hệ thống liên quan; xác định khoảng trống nghiên cứu cần giải quyết.

Trên cơ sở đó, chương tiếp theo sẽ tập trung vào phân tích bài toán, đối tượng nghiên cứu cụ thể và đề xuất kiến trúc hệ thống Enterprise AI Platform cho bài toán tích hợp đa hệ thống doanh nghiệp.
