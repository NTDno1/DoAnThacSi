<!-- Bao cao do an thac si - Nguyen Tien Dat -->

<!-- COVER -->

HỌC VIỆN CÔNG NGHỆ BƯU CHÍNH VIỄN THÔNG

![Biểu trưng Học viện Công nghệ Bưu chính Viễn thông](../references/logo_ptit.png)

NGUYỄN TIẾN ĐẠT

NGHIÊN CỨU VÀ XÂY DỰNG NỀN TẢNG AI DOANH NGHIỆP HƯỚNG TÍCH HỢP ĐA HỆ THỐNG

ENTERPRISE AI PLATFORM FOR CROSS-SYSTEM INTEGRATION

CHUYÊN NGÀNH: HỆ THỐNG THÔNG TIN

MÃ SỐ: 8.48.01.04

BÁO CÁO ĐỒ ÁN TỐT NGHIỆP

(Theo định hướng ứng dụng)

NGƯỜI HƯỚNG DẪN KHOA HỌC: PGS.TS. TRẦN ĐÌNH QUẾ

HÀ NỘI - 2026

<!-- END_COVER -->

---

# LỜI CẢM ƠN

Tác giả xin bày tỏ lòng biết ơn sâu sắc tới **PGS.TS. Trần Đình Quế**, người đã định hướng đề tài, góp ý về phương pháp nghiên cứu và tận tình hướng dẫn trong quá trình thực hiện đồ án.

Tác giả trân trọng cảm ơn các thầy, cô của Học viện Công nghệ Bưu chính Viễn thông đã trang bị nền tảng kiến thức về hệ thống thông tin, kiến trúc phần mềm và trí tuệ nhân tạo. Những kiến thức đó là cơ sở quan trọng để tác giả tiếp cận bài toán xây dựng nền tảng AI doanh nghiệp theo hướng có cấu trúc và có khả năng kiểm chứng.

Tác giả cũng xin cảm ơn đồng nghiệp, bạn bè và gia đình đã chia sẻ kinh nghiệm, động viên và tạo điều kiện trong suốt thời gian học tập, nghiên cứu và hoàn thiện báo cáo.

Do giới hạn về thời gian và mức độ hoàn thiện của prototype, báo cáo vẫn còn những nội dung cần tiếp tục kiểm thử và bổ sung bằng dữ liệu thực nghiệm. Tác giả chân thành đón nhận các ý kiến đóng góp để công trình được hoàn thiện hơn.

<!-- SIGNATURE -->
Hà Nội, năm 2026

**Nguyễn Tiến Đạt**
<!-- END_SIGNATURE -->

---

# MỤC LỤC

[[TOC]]

---

# DANH MỤC CHỮ CÁI VIẾT TẮT

*Bảng viết tắt được sắp xếp theo thứ tự chữ cái; chỉ bao gồm các thuật ngữ được sử dụng trong báo cáo.*

| Từ viết tắt | Tiếng Anh | Nghĩa tiếng Việt |
|---|---|---|
| ABAC | Attribute-Based Access Control | Kiểm soát truy cập dựa trên thuộc tính |
| AI | Artificial Intelligence | Trí tuệ nhân tạo |
| API | Application Programming Interface | Giao diện lập trình ứng dụng |
| BERT | Bidirectional Encoder Representations from Transformers | Mô hình biểu diễn ngôn ngữ hai chiều dựa trên Transformer |
| BM25 | Best Matching 25 | Thuật toán xếp hạng tìm kiếm từ khóa |
| CI/CD | Continuous Integration / Continuous Delivery | Tích hợp liên tục / chuyển giao liên tục |
| CRM | Customer Relationship Management | Hệ thống quản lý quan hệ khách hàng |
| DMS | Document Management System | Hệ thống quản lý tài liệu |
| DTO | Data Transfer Object | Đối tượng truyền dữ liệu |
| ERP | Enterprise Resource Planning | Hệ thống hoạch định nguồn lực doanh nghiệp |
| HNSW | Hierarchical Navigable Small World | Cấu trúc đồ thị tìm kiếm láng giềng gần đúng |
| HRM | Human Resource Management | Hệ thống quản lý nguồn nhân lực |
| HTTP | Hypertext Transfer Protocol | Giao thức truyền siêu văn bản |
| JWT | JSON Web Token | Mã thông báo xác thực dạng JSON |
| LLM | Large Language Model | Mô hình ngôn ngữ lớn |
| MCP | Model Context Protocol | Giao thức ngữ cảnh mô hình |
| NLP | Natural Language Processing | Xử lý ngôn ngữ tự nhiên |
| NFR | Non-Functional Requirement | Yêu cầu phi chức năng |
| OCR | Optical Character Recognition | Nhận dạng ký tự quang học |
| RAG | Retrieval-Augmented Generation | Sinh nội dung tăng cường bằng truy xuất |
| RAGAS | Retrieval-Augmented Generation Assessment | Khung đánh giá hệ thống RAG |
| RBAC | Role-Based Access Control | Kiểm soát truy cập dựa trên vai trò |
| ReAct | Reasoning and Acting | Mẫu kết hợp suy luận và hành động |
| REST | Representational State Transfer | Phong cách kiến trúc truyền trạng thái đại diện |
| RLS | Row-Level Security | Bảo mật ở cấp hàng dữ liệu |
| RRF | Reciprocal Rank Fusion | Hợp nhất thứ hạng nghịch đảo |
| SDK | Software Development Kit | Bộ công cụ phát triển phần mềm |
| SQL | Structured Query Language | Ngôn ngữ truy vấn có cấu trúc |
| SSE | Server-Sent Events | Cơ chế máy chủ đẩy sự kiện tới trình duyệt |
| UI | User Interface | Giao diện người dùng |

---

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

---

# CHƯƠNG 1. CƠ SỞ LÝ LUẬN

## Giới thiệu chương

Chương này trình bày nền tảng lý thuyết cho việc xây dựng Enterprise AI Platform, bao gồm: mô hình ngôn ngữ lớn, embedding và vector search, Retrieval-Augmented Generation, tìm kiếm lai, AI Agent, Model Context Protocol, tích hợp hệ thống doanh nghiệp và quản trị AI.

## Nội dung

Khái quát và hệ thống hóa cơ sở lý luận về các vấn đề nghiên cứu chính:

**Mô hình ngôn ngữ lớn (LLM).** Trình bày kiến trúc Transformer [19], cơ chế self-attention, mô hình nền tảng [2] và các hạn chế của LLM trong bối cảnh doanh nghiệp: hallucination, thiếu nguồn gốc, tính không xác định, giới hạn ngữ cảnh. Từ đó xác lập yêu cầu về provider abstraction và kiểm soát ngữ cảnh.

**Embedding và tìm kiếm vector.** Biểu diễn ngữ nghĩa bằng vector, Sentence-BERT [15], Dense Passage Retrieval [6]. Chia đoạn (chunking) và metadata. Tìm kiếm chính xác và tìm kiếm gần đúng với HNSW [8]. pgvector cho PostgreSQL [14].

**Retrieval-Augmented Generation (RAG).** Khái niệm RAG [7]: kết hợp bộ nhớ tham số với bộ nhớ phi tham số. Pipeline nhập liệu (ingestion): tiếp nhận, trích xuất, chia đoạn, embedding, lập chỉ mục. Pipeline truy vấn: query processing, retrieval, context assembly, generation, citation.

**Tìm kiếm lai, RRF và reranking.** BM25 cho tìm kiếm từ khóa [16], dense retrieval cho ngữ nghĩa [6], Reciprocal Rank Fusion (RRF) hợp nhất thứ hạng [4], reranking với BERT [12].

**AI Agent và sử dụng công cụ.** Khái niệm tác nhân, mẫu ReAct [20], tool use và Toolformer [17], vòng lặp planner-executor-memory. Kiểm soát thực thi và human-in-the-loop.

**Model Context Protocol (MCP).** Kiến trúc host-client-server, các primitive (resources, prompts, tools), giao tiếp JSON-RPC/SSE, bảo mật MCP [10].

**Microservices và tích hợp doanh nghiệp.** Đặc trưng tích hợp doanh nghiệp, microservices vs modular monolith [11], provider abstraction, multi-tenancy.

**Quản trị, bảo mật và đánh giá AI.** NIST AI RMF [18], OWASP Top 10 for LLM [13], guardrail, audit, observability. Khung đánh giá RAG với RAGAS [5].

**Nghiên cứu liên quan và khoảng trống.** Tổng hợp các hướng nghiên cứu hiện có và xác định năm khoảng trống mà đề tài tập trung giải quyết.

## Kết luận chương

Chương 1 xác lập cơ sở lý luận cho Enterprise AI Platform. LLM cần được bao quanh bởi tri thức ngoài mô hình và cơ chế quản trị. Embedding, BM25, vector search, RRF và reranking tạo pipeline retrieval; RAG nối retrieval với sinh câu trả lời có nguồn. Agent mở rộng từ trả lời sang hành động nên cần kiểm soát tool. MCP chuẩn hóa giao tiếp nhưng không thay thế authorization. Các khái niệm này là căn cứ để Chương 2 phân tích yêu cầu và đề xuất kiến trúc.

---

# Chương 2: PHÂN TÍCH BÀI TOÁN VÀ THIẾT KẾ NỀN TẢNG

## Giới thiệu chương

Chương này phân tích yêu cầu và thiết kế Enterprise AI Platform. Nội dung phân biệt kiến trúc đề xuất (trạng thái đích) với prototype hiện có trong mã nguồn.

## Nội dung

Khái quát thực trạng, phân tích và thiết kế giải pháp trên cơ sở lý thuyết đã trình bày ở Chương 1:

**Phân tích bài toán và yêu cầu.** Phát biểu bài toán: thiết kế nền tảng AI mở, hỗ trợ triển khai cục bộ, chuẩn hóa truy cập LLM/embedding, cung cấp RAG và Agent qua API, tạo nền tảng cho quản trị và tích hợp. Xác định bốn nhóm người dùng (nhân viên khai thác tri thức, quản lý nội dung, quản trị viên, lập trình viên tích hợp). Phạm vi chức năng, các ca sử dụng chính và yêu cầu phi chức năng.

**Nguyên tắc thiết kế và kiến trúc tổng thể.** Đề xuất kiến trúc bảy tầng: Presentation, API Gateway, Application Services, AI Engine (RAG, Agent, Provider), Integration (MCP, Connector), Data & Knowledge, Infrastructure. Ranh giới phân hệ, luồng xử lý một yêu cầu AI, Clean Architecture.

**Thiết kế dữ liệu và kho tri thức.** Phân lớp dữ liệu (quan hệ, vector, file, cache). Mô hình logic: Users, Tenants, Documents, Chunks, Embeddings, Conversations, AuditLogs. Thiết kế chunk, vector dimension, metadata, nhất quán và cách ly tenant.

**Thiết kế quy trình tiếp nhận tài liệu.** Pipeline ingestion: upload → validate → extract text → chunk → embed → index. Chiến lược chunking và kiểm soát chất lượng.

**Thiết kế Semantic Search và RAG.** Truy hồi hybrid (BM25 + dense), RRF, reranking, context assembly, prompt construction, streaming response, caching. Kế hoạch đánh giá retrieval và generation.

**Provider abstraction, Agent, Tool và MCP.** Trừu tượng hóa nhà cung cấp AI (Ollama, OpenAI). Thiết kế Agent với ReAct loop. Tool registry và kiểm soát thực thi. MCP client/server và tích hợp hệ thống.

**API, bảo mật, multi-tenant và governance.** Thiết kế REST API, mô hình bảo mật nhiều lớp (JWT, RBAC), multi-tenant, governance và audit trail.

**Kiến trúc triển khai.** Lựa chọn công nghệ (.NET 8 [9], Next.js, PostgreSQL/pgvector [14], Docker). Topology phát triển với Docker Compose. Kiến trúc triển khai mục tiêu và lộ trình hiện thực hóa.

## Kết luận chương

Chương 2 đã phân tích bài toán, xác định yêu cầu và đề xuất kiến trúc phân lớp cho Enterprise AI Platform. Thiết kế bao phủ dữ liệu, ingestion, RAG, agent/MCP, API, bảo mật và triển khai. Các quyết định thiết kế sẽ được kiểm chứng qua prototype ở Chương 3.

---

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

---

# KẾT LUẬN

## 1. Các kết quả đạt được

- Hệ thống hóa cơ sở lý luận về LLM, embedding, RAG, hybrid search, AI Agent, MCP và quản trị AI.
- Đề xuất kiến trúc phân lớp cho Enterprise AI Platform với provider abstraction, RAG engine, agent orchestration và integration layer.
- Xây dựng mã nguồn prototype trên .NET 8, Next.js, PostgreSQL/pgvector và Docker Compose.
- Thực hiện đánh giá tĩnh trên hiện vật mã nguồn, nhận diện hạn chế và khoảng trống kỹ thuật.
- Thiết kế quy trình thực nghiệm tái lập cho giai đoạn tiếp theo.

## 2. Nhận xét, đề xuất, khuyến nghị

Prototype có phạm vi rộng nhưng chưa đạt build/test ổn định. Cần ưu tiên sửa build, bổ sung test tự động, thống nhất schema và cấu hình trước khi mở rộng tính năng. Đề xuất tuân thủ lộ trình: sửa build → kiểm thử → thực nghiệm → hardening. Các kết luận chỉ nên dựa trên bằng chứng kiểm chứng được.

## 3. Hướng nghiên cứu tiếp theo

- Sửa build và thống nhất nền tảng (DbContext, schema, config, vector dimension).
- Thiết lập quality gate bằng unit test và integration test.
- Thực hiện đánh giá định lượng: retrieval (Recall@k, MRR, nDCG), RAG (Faithfulness, Citation Accuracy), Agent/MCP.
- Enterprise hardening: RBAC/ABAC, RLS, audit, secret management, observability, CI/CD.

---

# DANH MỤC CÁC TÀI LIỆU THAM KHẢO

## Tiếng Việt

[1] Thủ tướng Chính phủ, "Quyết định số 127/QĐ-TTg: Ban hành Chiến lược quốc gia về nghiên cứu, phát triển và ứng dụng Trí tuệ nhân tạo đến năm 2030," ngày 26 tháng 01 năm 2021. [Trực tuyến]. Địa chỉ: https://vanban.chinhphu.vn/default.aspx?docid=202565&pageid=27160

## Tiếng Anh

[2] R. Bommasani *và cộng sự*, "On the Opportunities and Risks of Foundation Models," Stanford Center for Research on Foundation Models, arXiv:2108.07258, 2021. [Trực tuyến]. Địa chỉ: https://crfm.stanford.edu/report.html

[3] T. B. Brown *và cộng sự*, "Language Models are Few-Shot Learners," trong *Advances in Neural Information Processing Systems 33*, tr. 1877–1901, 2020. [Trực tuyến]. Địa chỉ: https://proceedings.neurips.cc/paper/2020/hash/1457c0d6bfcb4967418bfb8ac142f64a-Abstract.html

[4] G. V. Cormack, C. L. A. Clarke và S. Büttcher, "Reciprocal Rank Fusion Outperforms Condorcet and Individual Rank Learning Methods," trong *Proceedings of the 32nd International ACM SIGIR Conference on Research and Development in Information Retrieval*, tr. 758–759, 2009. DOI: https://doi.org/10.1145/1571941.1572114

[5] S. Es, J. James, L. Espinosa Anke và S. Schockaert, "RAGAs: Automated Evaluation of Retrieval Augmented Generation," trong *Proceedings of the 18th Conference of the European Chapter of the Association for Computational Linguistics: System Demonstrations*, tr. 150–158, 2024. DOI: https://doi.org/10.18653/v1/2024.eacl-demo.16

[6] V. Karpukhin, B. Oguz, S. Min, P. Lewis, L. Wu, S. Edunov, D. Chen và W.-t. Yih, "Dense Passage Retrieval for Open-Domain Question Answering," trong *Proceedings of EMNLP*, tr. 6769–6781, 2020. DOI: https://doi.org/10.18653/v1/2020.emnlp-main.550

[7] P. Lewis *và cộng sự*, "Retrieval-Augmented Generation for Knowledge-Intensive NLP Tasks," trong *Advances in Neural Information Processing Systems 33*, tr. 9459–9474, 2020. [Trực tuyến]. Địa chỉ: https://proceedings.neurips.cc/paper/2020/hash/6b493230205f780e1bc26945df7481e5-Abstract.html

[8] Y. A. Malkov và D. A. Yashunin, "Efficient and Robust Approximate Nearest Neighbor Search Using Hierarchical Navigable Small World Graphs," *IEEE Transactions on Pattern Analysis and Machine Intelligence*, tập 42, số 4, tr. 824–836, 2020. DOI: https://doi.org/10.1109/TPAMI.2018.2889473

[9] Microsoft, "What's new in .NET 8," Microsoft Learn, 2023. [Trực tuyến]. Địa chỉ: https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8/overview

[10] Model Context Protocol Contributors, "Model Context Protocol Specification," phiên bản giao thức 2025-11-25. [Trực tuyến]. Địa chỉ: https://modelcontextprotocol.io/specification/2025-11-25

[11] S. Newman, *Building Microservices: Designing Fine-Grained Systems*, ấn bản 2. Sebastopol, CA, Hoa Kỳ: O'Reilly Media, 2021, ISBN: 978-1-492-03402-5. [Trực tuyến]. Địa chỉ: https://www.oreilly.com/library/view/building-microservices-2nd/9781492034018/

[12] R. Nogueira và K. Cho, "Passage Re-ranking with BERT," arXiv:1901.04085, 2019. [Trực tuyến]. Địa chỉ: https://arxiv.org/abs/1901.04085

[13] OWASP Foundation, "OWASP Top 10 for LLM Applications 2025," 2024. [Trực tuyến]. Địa chỉ: https://genai.owasp.org/resource/owasp-top-10-for-llm-applications-2025/

[14] pgvector Contributors, "pgvector: Open-source Vector Similarity Search for Postgres," tài liệu dự án. [Trực tuyến]. Địa chỉ: https://github.com/pgvector/pgvector

[15] N. Reimers và I. Gurevych, "Sentence-BERT: Sentence Embeddings using Siamese BERT-Networks," trong *Proceedings of EMNLP-IJCNLP*, tr. 3982–3992, 2019. DOI: https://doi.org/10.18653/v1/D19-1410

[16] S. Robertson và H. Zaragoza, "The Probabilistic Relevance Framework: BM25 and Beyond," *Foundations and Trends in Information Retrieval*, tập 3, số 4, tr. 333–389, 2009. DOI: https://doi.org/10.1561/1500000019

[17] T. Schick, J. Dwivedi-Yu, R. Dessì, R. Raileanu, M. Lomeli, E. Hambro, L. Zettlemoyer, N. Cancedda và T. Scialom, "Toolformer: Language Models Can Teach Themselves to Use Tools," trong *Advances in Neural Information Processing Systems 36*, 2023. DOI: https://doi.org/10.52202/075280-2997

[18] E. Tabassi, *Artificial Intelligence Risk Management Framework (AI RMF 1.0)*, NIST AI 100-1. Gaithersburg, MD, Hoa Kỳ: National Institute of Standards and Technology, 2023. DOI: https://doi.org/10.6028/NIST.AI.100-1

[19] A. Vaswani, N. Shazeer, N. Parmar, J. Uszkoreit, L. Jones, A. N. Gomez, Ł. Kaiser và I. Polosukhin, "Attention Is All You Need," trong *Advances in Neural Information Processing Systems 30*, 2017. [Trực tuyến]. Địa chỉ: https://proceedings.neurips.cc/paper_files/paper/2017/hash/3f5ee243547dee91fbd053c1c4a845aa-Abstract.html

[20] S. Yao, J. Zhao, D. Yu, N. Du, I. Shafran, K. Narasimhan và Y. Cao, "ReAct: Synergizing Reasoning and Acting in Language Models," trong *International Conference on Learning Representations*, 2023. [Trực tuyến]. Địa chỉ: https://openreview.net/forum?id=WE_vluYUL-X

---

# DỰ KIẾN KẾ HOẠCH THỰC HIỆN

| TT | Nội dung | Dự kiến thời gian thực hiện |
|---|---|---|
| 1 | Nghiên cứu, chọn đề tài, xây dựng đề cương đề án tốt nghiệp | Từ 06/07/2026 – 15/07/2026 |
| 2 | Nộp đề cương đề án tốt nghiệp | Đến 20/08/2026 |
| 3 | Báo cáo đề cương, sửa chữa hoàn thiện, nộp đề cương sau báo cáo | Từ 21/08/2026 – 27/08/2026 |
| 4 | Nghiên cứu, viết, hoàn thiện đề án tốt nghiệp | Từ 08/09/2026 đến 08/01/2027 |
|   | Chương 1: Cơ sở lý luận | 08/09/2026 – 30/09/2026 |
|   | Chương 2: Phân tích bài toán và thiết kế nền tảng | 01/10/2026 – 15/11/2026 |
|   | Chương 3: Hiện thực hóa prototype và đánh giá | 16/11/2026 – 15/12/2026 |
|   | Chỉnh sửa, hoàn thiện đồ án | 16/12/2026 – 08/01/2027 |
| 5 | Nộp quyển đề án TN và hồ sơ bảo vệ | Tháng 01-02/2027 |
