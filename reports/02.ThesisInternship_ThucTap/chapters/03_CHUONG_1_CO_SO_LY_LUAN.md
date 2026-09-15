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
