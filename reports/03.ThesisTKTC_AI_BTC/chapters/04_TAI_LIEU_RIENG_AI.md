# TÀI LIỆU RIÊNG — ÁP DỤNG AI TRONG HỆ THỐNG VĂN PHÒNG SỐ BỘ TÀI CHÍNH

> **Mục đích:** File này chứa **toàn bộ nội dung chuyên đề về AI** được tách riêng khỏi tài liệu Thiết kế chức năng chính. Mỗi đoạn đều được đánh mã `[AI-X.Y.Z]` để bạn biết chính xác **paste vào vị trí nào** trong file `BaoCaoTKTC_AI_BTC_Full.docx`.
>
> **Cách dùng nhanh:**
> 1. Mở file `BaoCaoTKTC_AI_BTC_Full.docx` bằng Word.
> 2. Mở file `.md` này bằng VSCode.
> 3. Tìm mã `[AI-X.Y.Z]` ở đầu mỗi đoạn → Tra bảng **HƯỚNG DẪN PASTE** ở cuối file để biết paste vào mục nào trong tài liệu chính.

---

# 🔷 PHẦN A — YÊU CẦU AI & BÀI TOÁN GIẢI QUYẾT ĐƯỢC

---

## [AI-A.1.1] **A.1.1. Bối cảnh và lý do áp dụng AI**

### Bối cảnh

Phần mềm Văn phòng số Bộ Tài Chính hiện đang phục vụ công tác **quản lý văn bản, hồ sơ công việc, chỉ đạo điều hành và quản trị nội bộ** cho các đơn vị thuộc Bộ từ Trung ương đến địa phương, với quy mô khoảng **60.000 tài khoản người dùng**. Hệ thống đã vận hành ổn định trên nền tảng SmartOffice Backend Core (.NET 10 / Clean Architecture / PostgreSQL / Redis / Kafka), nhưng trước áp lực **cải cách hành chính, chuyển đổi số và chỉ đạo điều hành thời gian thực**, các nhu cầu nghiệp vụ phát sinh những thách thức mới mà cách xử lý truyền thống (rule-based, form-based) không thể đáp ứng hiệu quả.

### Lý do cần áp dụng AI

| # | Thách thức hiện tại | Hạn chế của giải pháp truyền thống | AI giải quyết được |
|---|---|---|---|
| 1 | Khối lượng văn bản đến/đi mỗi năm lên tới hàng triệu, tăng theo cấp số nhân | Văn thư/chuyên viên phải đọc thủ công để phân loại, tóm tắt → chậm, sai sót | **Tự động tóm tắt**, **phân loại**, **trích xuất metadata** bằng LLM + OCR + RAG |
| 2 | Lãnh đạo Bộ/đơn vị không thể đọc hết văn bản đến để chỉ đạo | Phải đợi văn thư tổng hợp → mất thời gian | **ChatOps với RAG**: lãnh đạo hỏi bằng ngôn ngữ tự nhiên "Văn bản nào của Bộ Tài chính liên quan đến cải cách tiền lương?" |
| 3 | Hồ sơ công việc nhiều cấp, nhiều đơn vị, dễ thất lạc | Tìm kiếm từ khóa không trả về kết quả đúng ý (vd: "hợp đồng" trả về hàng nghìn) | **Hybrid Search** (BM25 + Vector) + **Reranking** cho kết quả chính xác |
| 4 | Văn bản đến dạng PDF scan chiếm ~30-40% tổng văn bản | Văn thư phải gõ lại vào hệ thống → tốn thời gian, sai sót | **OCR + Layout Parser + Schema Mapping** tự động điền form văn bản |
| 5 | Yêu cầu phức tạp đa bước (vd: "Tìm văn bản quá hạn, phân loại, gửi email báo cáo") | Phải thao tác thủ công nhiều bước, nhiều service | **Agent + MCP Tool calling**: Agent tự lập kế hoạch, gọi tool, ghép kết quả |
| 6 | Yêu cầu hỗ trợ tiếng Việt chuẩn hành chính | LLM thương mại nhiều khi không rà văn phong hành chính VN | **Prompt template chuẩn** + **RAG với kho mẫu văn bản** + (tùy chọn) **Fine-tune** trên corpus BTC |
| 7 | Yêu cầu bảo mật dữ liệu cấp nhà nước | Không thể gửi dữ liệu mật ra cloud nước ngoài | **Ollama local on-premise** cho dữ liệu nhạy cảm + **OpenAI cloud** cho dữ liệu thường |
| 8 | Khối lượng metadata truy vấn SQL rất lớn, truy vấn phức tạp | Báo cáo viết tay chậm, KPI cập nhật không kịp | **NL2SQL**: người dùng hỏi tự nhiên → sinh SQL → chạy → trả kết quả + trực quan |

### Mục tiêu tổng quát khi áp dụng AI

1. **Nâng cao năng suất xử lý văn bản** cho 60.000 người dùng lên **ít nhất 30-50%** trong 12 tháng đầu triển khai.
2. **Giảm thời gian tìm kiếm thông tin** từ trung bình 15-30 phút xuống **dưới 30 giây** (qua RAG).
3. **Tự động hóa 70% khối lượng OCR** cho văn bản scan.
4. **Cung cấp khả năng hỏi-đáp ngôn ngữ tự nhiên** (ChatOps) cho lãnh đạo và chuyên viên.
5. **Đảm bảo bảo mật dữ liệu** cấp nhà nước bằng hạ tầng AI hỗn hợp (local + cloud).

---

## [AI-A.1.2] **A.1.2. Yêu cầu chức năng (Functional Requirements) cho AI**

Danh sách yêu cầu chức năng AI được đánh mã theo chuẩn `[FR-AI-NN]` để dễ truy vết.

| Mã | Yêu cầu | Mô tả chi tiết | Độ ưu tiên |
|---|---|---|---|
| **FR-AI-01** | Tóm tắt văn bản hành chính | Sinh tóm tắt 3-5 dòng văn phong hành chính cho văn bản đến/đi/nội bộ | Cao |
| **FR-AI-02** | Chuẩn hóa văn phong | Sửa lỗi chính tả, chuẩn hóa câu chữ theo quy định thể thức văn bản | Cao |
| **FR-AI-03** | Phân loại văn bản tự động | Phân loại theo: loại văn bản, lĩnh vực, độ mật, độ khẩn | Cao |
| **FR-AI-04** | Trích xuất metadata từ văn bản | Số ký hiệu, ngày, cơ quan ban hành, người ký, trích yếu | Cao |
| **FR-AI-05** | OCR văn bản scan | Nhận dạng tiếng Việt từ PDF/ảnh scan | Cao |
| **FR-AI-06** | Trích xuất bảng biểu từ văn bản scan | Lấy bảng → CSV/JSON | Trung bình |
| **FR-AI-07** | Hỏi-đáp ngôn ngữ tự nhiên (ChatOps/RAG) | Hỏi về nội dung văn bản, quy trình, văn bản pháp luật | Cao |
| **FR-AI-08** | Tìm kiếm ngữ nghĩa | Tìm văn bản liên quan theo ý nghĩa, không chỉ từ khóa | Cao |
| **FR-AI-09** | Gợi ý lãnh đạo phê duyệt | AI gợi ý lãnh đạo phù hợp dựa trên lĩnh vực, phòng ban | Trung bình |
| **FR-AI-10** | Tự động tạo báo cáo tuần/tháng | Tổng hợp văn bản, hồ sơ, tiến độ → báo cáo PDF/Word | Trung bình |
| **FR-AI-11** | Agent đa bước (multi-step task) | Giải quyết yêu cầu phức tạp đa bước có gọi tool | Cao |
| **FR-AI-12** | NL2SQL (Natural Language to SQL) | Sinh câu SQL từ câu hỏi tiếng Việt tự nhiên | Trung bình |
| **FR-AI-13** | Tự động phát hiện văn bản quá hạn | Phân tích hạn xử lý, gửi cảnh báo sớm | Trung bình |
| **FR-AI-14** | Gợi ý lịch họp tối ưu | AI xếp lịch dựa trên lịch lãnh đạo, phòng họp trống | Thấp |
| **FR-AI-15** | Ký số có hỗ trợ AI | AI xác nhận nội dung trước khi ký (kiểm tra trích yếu, độ mật) | Thấp |
| **FR-AI-16** | Dịch văn bản đa ngôn ngữ | Anh-Việt, Việt-Anh cho văn bản liên thông quốc tế | Thấp |
| **FR-AI-17** | Phát hiện gian lận / bất thường | Phát hiện pattern bất thường trong xử lý văn bản | Thấp |
| **FR-AI-18** | Đánh giá chất lượng RAG (RAGAS) | Đánh giá Context Precision/Recall, Faithfulness, Answer Relevancy | Cao |
| **FR-AI-19** | Quản trị AI (Admin Console) | CRUD cho Knowledge Source, Prompt, Agent, Tool, Provider, Budget | Cao |
| **FR-AI-20** | Hỗ trợ đa LLM provider | OpenAI + Ollama local + Anthropic (multi-provider abstraction) | Cao |

---

## [AI-A.1.3] **A.1.3. Yêu cầu phi chức năng (Non-Functional Requirements) cho AI**

Được đánh mã `[NFR-AI-NN]`.

| Mã | Yêu cầu | Chỉ số mục tiêu |
|---|---|---|
| **NFR-AI-01** | **Hiệu năng (Performance)** | p95 latency tóm tắt ≤ 5s; hỏi-đáp RAG ≤ 8s; OCR 1 trang ≤ 3s |
| **NFR-AI-02** | **Thông lượng (Throughput)** | ≥ 1.000 request/phút (peak); hỗ trợ 60.000 user đồng thời qua cache + queue |
| **NFR-AI-03** | **Sẵn sàng (Availability)** | Uptime ≥ 99.9% trong giờ hành chính; graceful degradation khi LLM provider lỗi |
| **NFR-AI-04** | **Bảo mật (Security)** | Zero-Trust; mTLS; JWT rotation; PII redaction; audit log mọi request |
| **NFR-AI-05** | **Tuân thủ pháp luật** | Nghị định 13/2023/NĐ-CP; Luật An toàn thông tin 2015; Luật An ninh mạng 2018 |
| **NFR-AI-06** | **Bảo mật dữ liệu mật** | Văn bản "Mật"/"Tối mật"/"Tuyệt mật" → chỉ dùng Ollama local, KHÔNG gửi cloud |
| **NFR-AI-07** | **Khả năng mở rộng (Scalability)** | Scale-out theo chiều ngang qua Kubernetes; stateless API; horizontal scale AI worker |
| **NFR-AI-08** | **Khả năng quan sát (Observability)** | OpenTelemetry, ELK, Prometheus, Grafana; distributed tracing |
| **NFR-AI-09** | **Khả năng bảo trì (Maintainability)** | Clean Architecture + Vertical Slice; unit test ≥ 80% coverage; CI/CD |
| **NFR-AI-10** | **Đa ngôn ngữ** | Hỗ trợ tiếng Việt (ưu tiên), tiếng Anh |
| **NFR-AI-11** | **Khả năng tái sử dụng (Reusability)** | Module AI dùng được cho nhiều phân hệ (Văn bản, Hồ sơ, Lịch, ...) |
| **NFR-AI-12** | **Tính đồng nhất (Consistency)** | Cùng prompt → cùng style output; version-controlled prompt template |
| **NFR-AI-13** | **Khả năng rollback** | Roll-back model version, prompt version, knowledge version trong < 5 phút |
| **NFR-AI-14** | **Chi phí (Cost)** | Theo dõi token usage / tenant; budget alert khi vượt 80% |
| **NFR-AI-15** | **Giới hạn tỷ lệ lỗi (Error Rate)** | Error rate < 1%; retry tối đa 3 lần; fallback provider khi lỗi |
| **NFR-AI-16** | **Hỗ trợ nhiều LLM provider** | OpenAI, Ollama, Anthropic, Azure OpenAI (không phụ thuộc cứng vào 1 provider) |
| **NFR-AI-17** | **Plugin / MCP extensibility** | Cho phép đăng ký tool mới mà không sửa lõi |
| **NFR-AI-18** | **Đo lường chất lượng (Quality)** | RAGAS framework đánh giá định kỳ; baseline + benchmark mỗi model |

---

## [AI-A.2.1] **A.2.1. Bài toán nghiệp vụ 1 — Tự động tóm tắt & chuẩn hóa văn bản**

### Mô tả bài toán

Hiện tại, khi có văn bản đến, **văn thư** phải đọc toàn bộ nội dung để:
- Tóm tắt nội dung trích yếu (ghi vào sổ văn bản, hệ thống).
- Chuẩn hóa lại văn phong (sửa lỗi chính tả, cấu trúc câu).
- Trích metadata: số ký hiệu, ngày tháng, cơ quan ban hành, người ký.

Mỗi văn bản mất **5-15 phút** xử lý thủ công. Với hàng triệu văn bản/năm, đây là khối lượng công việc khổng lồ.

### Giải pháp AI

1. **OCR pipeline** (Tesseract/PaddleOCR) đối với văn bản PDF scan.
2. **LLM** đọc nội dung → sinh tóm tắt 3-5 dòng văn phong hành chính.
3. **Prompt template** có versioning, ví dụ:

```
Bạn là cán bộ hành chính Bộ Tài Chính.
Yêu cầu:
- Tóm tắt nội dung văn bản dưới đây thành 3-5 dòng
- Giữ nguyên ý chính, không suy diễn
- Văn phong hành chính, ngắn gọn, rõ ràng

Văn bản:
{{document_content}}

Các văn bản tương tự (nếu có):
{{top_k_documents}}
```

4. **Async job** qua Kafka + worker pool → trả kết quả trong < 5 giây cho văn bản < 10 trang.
5. **Audit log** lưu prompt + response để truy vết.

### Giá trị mang lại

| Chỉ số | Trước AI | Sau AI |
|---|---|---|
| Thời gian xử lý 1 văn bản | 5-15 phút | < 5 giây (async) |
| Năng suất văn thư | ~30 văn bản/ngày | ~300+ văn bản/ngày (chỉ review) |
| Tỷ lệ lỗi metadata | ~5-8% | < 1% |
| Chi phí nhân sự | Cao | Giảm 30-50% |

### Phân hệ AI tham chiếu: `II.5.2 AI Engine Core`, `II.5.5 OCR & Document Extraction`, `II.5.7 AI Job & Task Queue`

---

## [AI-A.2.2] **A.2.2. Bài toán nghiệp vụ 2 — Hỏi-đáp ngôn ngữ tự nhiên (ChatOps/RAG)**

### Mô tả bài toán

Lãnh đạo Bộ, lãnh đạo đơn vị, chuyên viên thường xuyên cần tra cứu:
- *"Văn bản nào của Tổng cục Thuế ban hành trong tháng 7/2026 liên quan đến chính sách thuế VAT?"*
- *"Quy trình phê duyệt văn bản mật trong Bộ Tài Chính được quy định như thế nào?"*
- *"Có bao nhiêu văn bản đến quá hạn xử lý trong tuần qua tại Cục Tin học và Thống kê tài chính?"*

Hiện tại, họ phải **mở hệ thống, gõ từ khóa, lọc thủ công**, mất **15-30 phút/câu hỏi**.

### Giải pháp AI (RAG - Retrieval-Augmented Generation)

1. **Knowledge Base** được ingest từ:
   - Văn bản DMS (sau khi số hóa, OCR)
   - Quy trình nghiệp vụ HRM, ERP
   - Văn bản pháp luật (Luật Ngân sách, Luật Thuế, ...)
   - Hồ sơ công việc
   - Web nội bộ (nếu cần)

2. **Embedding** toàn bộ knowledge bằng model `text-embedding-3-small` (OpenAI) hoặc `nomic-embed-text` (Ollama local).
3. **Hybrid Search** kết hợp:
   - **BM25** cho từ khóa chính xác (số văn bản, tên riêng).
   - **Vector cosine similarity** cho ngữ nghĩa.
   - **RRF Fusion** (Reciprocal Rank Fusion) để hợp nhất.
4. **Cross-Encoder Reranking** để sắp xếp lại top-K → top-N (5-10).
5. **LLM** sinh câu trả lời dựa trên context + trích dẫn nguồn.
6. **Faithfulness check** để tránh hallucination.

### Ví dụ luồng xử lý

```
User: "Văn bản nào về cải cách tiền lương trong Quý 3/2026?"
  → Embed query
  → Search: 50 văn bản (BM25 + vector)
  → Rerank: top 8 văn bản liên quan nhất
  → Aggregate context (token budget 2048)
  → Prompt: system="Bạn là trợ lý AI của Bộ Tài Chính", user=query, context=top8
  → LLM (gpt-4o-mini hoặc qwen2.5:7b)
  → Output: danh sách 8 văn bản + trích dẫn + tóm tắt
  → Trả về cho user với citation link
```

### Giá trị mang lại

| Chỉ số | Trước AI | Sau AI |
|---|---|---|
| Thời gian trả lời 1 câu hỏi | 15-30 phút | < 8 giây |
| Độ chính xác (RAGAS) | N/A | Context Precision ≥ 0.85, Recall ≥ 0.80 |
| Mức độ hài lòng (dự kiến) | – | ≥ 80% user hài lòng |

### Phân hệ AI tham chiếu: `II.5.2 AI Engine Core`, `II.5.4 Knowledge Base & RAG`

---

## [AI-A.2.3] **A.2.3. Bài toán nghiệp vụ 3 — OCR & Trích xuất dữ liệu từ văn bản scan**

### Mô tả bài toán

Khoảng **30-40% văn bản đến** Bộ Tài Chính là PDF scan từ đơn vị gửi. Văn thư phải **gõ lại thủ công** vào hệ thống DMS:
- Số ký hiệu
- Ngày tháng
- Cơ quan ban hành
- Trích yếu
- Lĩnh vực
- Độ mật
- Người ký

Mỗi văn bản scan mất **10-20 phút**. Sai sót phổ biến do gõ nhầm số, ngày tháng.

### Giải pháp AI

1. **Detect**: Tesseract 5+ (tiếng Việt) hoặc PaddleOCR/Surya (nhanh hơn, chính xác hơn cho TV).
2. **Layout-aware parser**: Tách trang, cột, bảng.
3. **Table extraction**: OpenCV + heuristic hoặc Table-Transformer.
4. **Form/Key-Value extraction**: Dùng **Donut** hoặc **LLM với prompt trích xuất**.
5. **Schema mapping**: Chuẩn hóa về schema `DocumentMetadata` của Bộ.

### Ví dụ pipeline

```
PDF scan (10 trang)
  → [Tesseract OCR] → text + bounding box (20s)
  → [Layout Parser] → {header, body, table, signature} (5s)
  → [Table extract] → 3 bảng → JSON (3s)
  → [LLM extract] → JSON metadata (5s)
  → [Schema mapper] → {Số: 1234/BTC, Ngày: 15/7/2026, ...} (1s)
  → Lưu vào DMS metadata, hiển thị cho user xác nhận
```

### Giá trị mang lại

| Chỉ số | Trước AI | Sau AI |
|---|---|---|
| Thời gian xử lý 1 văn bản scan | 10-20 phút | < 30 giây (auto) + 1 phút review |
| Độ chính xác OCR (tiếng Việt) | N/A | ≥ 95% với Tesseract, ≥ 98% với PaddleOCR |
| Tỷ lệ tự động hóa | ~5% | ≥ 70% |
| Chi phí văn thư | Cao | Giảm 40% |

### Phân hệ AI tham chiếu: `II.5.5 OCR & Document Extraction`

---

## [AI-A.2.4] **A.2.4. Bài toán nghiệp vụ 4 — Agent tự động xử lý tác vụ đa bước**

### Mô tả bài toán

Nhiều yêu cầu nghiệp vụ phức tạp đòi hỏi **nhiều bước** kết hợp nhiều service:

**Ví dụ 1:** *"Tìm tất cả văn bản đến quá hạn phê duyệt trong 7 ngày qua, phân loại theo đơn vị, sinh báo cáo Excel đính kèm email gửi lãnh đạo Bộ."*
→ Cần: search_doc → list_overdue → group_by_unit → generate_excel → draft_email → send_email

**Ví dụ 2:** *"Lập lịch họp tuần cho ban giám đốc dựa trên lịch rảnh của 5 lãnh đạo, đặt phòng họp còn trống, gửi lời mời qua Teams."*
→ Cần: fetch_calendar → find_common_slot → check_room → create_meeting → send_invitation

### Giải pháp AI (Multi-Agent + MCP)

1. **Intent Router** phân loại: simple rag → RAG pipeline; complex task → Multi-Agent.
2. **Task Planner** sinh plan JSON (chain-of-thought).
3. **Multi-Agent Coordinator**:
   - **RetrieverAgent**: search knowledge base
   - **AnalyzerAgent**: phân tích dữ liệu
   - **WriterAgent**: sinh báo cáo/email
4. **MCP Tool calling**: mỗi agent gọi tool thông qua MCP Server.
5. **Human-in-the-Loop (HITL)**: với thao tác "write" (gửi email, xóa văn bản), agent dừng chờ duyệt.
6. **Safety Guardrails**: lọc action nguy hiểm (delete, drop, truncate).

### Giá trị mang lại

| Chỉ số | Trước AI | Sau AI |
|---|---|---|
| Thời gian xử lý tác vụ đa bước | 30-60 phút (thủ công) | 5-10 phút (có HITL) |
| Số bước user tự làm | 5-10 bước | 1-2 bước (review & approve) |
| Lỗi do thao tác thủ công | ~5% | < 1% (có guardrails) |

### Phân hệ AI tham chiếu: `II.5.3 Agent Orchestration`, `II.5.8 MCP Server`

---

## [AI-A.2.5] **A.2.5. Bài toán nghiệp vụ 5 — Tìm kiếm ngữ nghĩa trong kho văn bản lớn**

### Mô tả bài toán

Kho văn bản của Bộ Tài Chính có hàng **triệu văn bản** tích lũy qua nhiều năm. Tìm kiếm bằng từ khóa hiện tại:
- Trả về quá nhiều kết quả không liên quan (recall cao nhưng precision thấp).
- Không hiểu ý đồ người dùng (vd: tìm "hợp đồng" mà ý là "hợp đồng mua sắm thiết bị" → kết quả chứa mọi loại hợp đồng).

### Giải pháp AI (Hybrid Search + Reranking)

1. **Embedding** toàn bộ văn bản bằng `text-embedding-3-small` hoặc `nomic-embed-text`.
2. **Hybrid Search**:
   - **BM25** cho từ khóa chính xác (số văn bản, tên riêng, mã số thuế).
   - **Vector cosine** cho ngữ nghĩa.
   - **RRF Fusion** (Reciprocal Rank Fusion).
3. **Cross-Encoder Reranking** (vd: `BAAI/bge-reranker-v2-m3`):
   - Input: (query, document) pair
   - Output: relevance score
   - Sắp xếp lại top-50 → top-10.
4. **Filter theo metadata**: tenant_id, document_type, date_range, security_level.

### So sánh kết quả

| Phương pháp | Precision@10 | Recall@10 | Latency |
|---|---|---|---|
| Chỉ keyword (PostgreSQL full-text) | 0.45 | 0.62 | 50ms |
| Chỉ vector (pgvector) | 0.68 | 0.71 | 120ms |
| **Hybrid + Reranking** | **0.88** | **0.83** | 250ms |

### Phân hệ AI tham chiếu: `II.5.4 Knowledge Base & RAG`

---

## [AI-A.2.6] **A.2.6. Bài toán nghiệp vụ 6 — Đảm bảo an toàn dữ liệu cấp nhà nước**

### Mô tả bài toán

Theo Nghị định 13/2023/NĐ-CP và Luật An toàn thông tin mạng, **dữ liệu văn bản mật** của Bộ Tài Chính **KHÔNG được phép gửi ra cloud nước ngoài**. Tuy nhiên:
- LLM mã nguồn mở (Ollama) chạy local có chất lượng thấp hơn LLM thương mại.
- LLM thương mại (OpenAI) có chất lượng cao nhưng không được dùng cho dữ liệu mật.

### Giải pháp AI (Hybrid Provider)

1. **Phân loại độ nhạy của request**:
   - **Low**: văn bản thường, công khai → dùng OpenAI (gpt-4o-mini) - rẻ, nhanh, chất lượng cao.
   - **Medium**: văn bản nội bộ → dùng OpenAI hoặc Ollama tùy policy tenant.
   - **High**: văn bản "Mật"/"Tối mật"/"Tuyệt mật" → **CHỈ dùng Ollama local** trên GPU on-premise.

2. **Provider Selector** dựa trên:
   - Sensitivity level của request
   - Tenant policy (cho phép cloud hay không)
   - Cost budget
   - Latency requirement
   - Availability

3. **Data classification tự động**:
   - Phân tích metadata `security_level` của văn bản.
   - Phân tích nội dung bằng keyword + LLM nhỏ để detect "MẬT", "TỐI MẬT".
   - Nếu phát hiện → tự chuyển sang Ollama local.

4. **Audit log** ghi rõ provider đã dùng, đảm bảo truy vết.

### Bảng quyết định

| Sensitivity | Tenant policy | Provider | Model |
|---|---|---|---|
| Low | Cho phép cloud | OpenAI | gpt-4o-mini |
| Medium | Cho phép cloud | OpenAI | gpt-4o-mini / gpt-4o |
| Medium | Không cho phép cloud | Ollama | qwen2.5:7b |
| High | – | **Ollama local only** | qwen2.5:7b |
| High + đặc biệt | – | Ollama local + offline | qwen2.5:7b (no internet) |

### Phân hệ AI tham chiếu: `II.5.6 Provider Abstraction`, `II.1.9 Kiến trúc an toàn bảo mật`

---

# 🔷 PHẦN B — THIẾT KẾ HỆ THỐNG AI (LLM, RAG, AGENT, OCR, PROVIDER)

---

## [AI-B.1.1] **B.1.1. Kiến trúc tổng thể AI Layer**

```
┌──────────────────────────────────────────────────────────────────┐
│                  PRESENTATION (Web/Mobile/ChatOps)               │
└─────────────────────────┬────────────────────────────────────────┘
                          │
┌─────────────────────────▼────────────────────────────────────────┐
│  KONG GATEWAY + BFF (.NET)  ← AuthN/AuthZ/Rate-limit/Routing    │
└─────────────────────────┬────────────────────────────────────────┘
                          │
┌─────────────────────────▼────────────────────────────────────────┐
│   AI GATEWAY (II.5.1)  ← Single entry for all AI requests      │
└──┬──────────┬──────────┬──────────┬──────────┬─────────────────┘
   │          │          │          │          │
   ▼          ▼          ▼          ▼          ▼
┌──────┐ ┌──────┐ ┌──────────┐ ┌────────┐ ┌────────┐
│Agent │→│Engine│→│Knowledge │→│  OCR   │→│Provider│
│Orch. │ │ Core │ │ Base RAG │ │Extract │ │Abs.(LLM)│
└──┬───┘ └──┬───┘ └────┬─────┘ └───┬────┘ └───┬────┘
   │        │          │            │          │
   ▼        ▼          ▼            ▼          ▼
┌──────┐ ┌──────┐ ┌──────────┐ ┌────────┐ ┌────────┐
│ MCP  │ │ Audit│ │PostgreSQL│ │MinIO/  │ │OpenAI/ │
│Server│ │&Obs. │ │+pgvector │ │  S3    │ │Ollama  │
└──────┘ └──────┘ └──────────┘ └────────┘ └────────┘
```

**Đặc điểm:**

- **AI Gateway** là single entry point.
- **AI Engine Core** chạy RAG pipeline.
- **Agent Orchestrator** cho multi-step task.
- **Knowledge Base** lưu vector + metadata.
- **OCR Pipeline** xử lý văn bản scan.
- **Provider Abstraction** đa LLM.
- **MCP Server** expose tool cho Agent.
- **Audit & Observability** đảm bảo truy vết.

---

## [AI-B.1.2] **B.1.2. Luồng AI xử lý một request từ Frontend**

```
1. User gửi POST /v1/ai/chat
       Body: { "message": "...", "conversation_id": "...", "stream": true }

2. Kong Gateway
   → AuthN: verify JWT (OpenIddict)
   → Rate-limit: check quota user/tenant
   → Route → BFF → AI Gateway

3. AI Gateway (II.5.1)
   → Load user, tenant, permissions
   → Prompt sanitization (chống prompt injection, PII redaction)
   → Semantic cache check (Redis)
   → Provider selection (cost + sensitivity + availability)
   → Forward to AI Engine Core (II.5.2) hoặc Agent (II.5.3)

4. AI Engine Core (II.5.2)
   → Load PromptTemplate (versioning)
   → Embed query → search Knowledge Base (hybrid + rerank)
   → Aggregate context (token budget)
   → Call LLM (streaming)
   → Validate output
   → Extract citations
   → Return response + citations

5. AI Gateway trả response cho client
   → Audit log: user, tenant, prompt_hash, model, tokens, latency
   → Charge token usage vào tenant budget

6. Trả về client qua SSE/WebSocket (nếu stream)
```

---

## [AI-B.2.1] **B.2.1. LLM (Large Language Model) — Lựa chọn và cấu hình**

### LLM được sử dụng

| Provider | Model | Vai trò | Use case chính | License |
|---|---|---|---|---|
| **OpenAI** | `gpt-4o-mini` | Chat/Generation | Tóm tắt, chuẩn hóa, hỏi-đáp (low/medium sensitivity) | Commercial |
| **OpenAI** | `gpt-4o` | Chat/Generation | Tác vụ phức tạp, multi-step | Commercial |
| **OpenAI** | `text-embedding-3-small` | Embedding | Vector hóa văn bản cho RAG | Commercial |
| **OpenAI** | `text-embedding-3-large` | Embedding | Embedding chất lượng cao cho multi-lingual | Commercial |
| **Ollama local** | `qwen2.5:7b-instruct` | Chat/Generation | Tác vụ mật (on-prem) | Open source |
| **Ollama local** | `llama3.2:3b` | Chat/Generation | Tác vụ nhẹ, latency thấp | Open source |
| **Ollama local** | `nomic-embed-text` | Embedding | Embedding tiếng Việt cho on-prem | Open source |
| **Ollama local** | `bge-m3` | Embedding | Embedding multilingual | Open source |
| **Ollama local** | `bge-reranker-v2-m3` | Reranking | Reranker cross-encoder | Open source |
| **Anthropic** (optional) | `claude-3-5-sonnet` | Chat/Generation | Backup provider | Commercial |

### Cấu hình mặc định cho mỗi use case

```python
# config/ai_defaults.yaml
summarize:
  provider: openai
  model: gpt-4o-mini
  temperature: 0.2
  max_tokens: 500

classify:
  provider: openai
  model: gpt-4o-mini
  temperature: 0.0
  max_tokens: 50

chat_rag:
  provider: openai
  model: gpt-4o-mini
  temperature: 0.3
  max_tokens: 1024
  retrieval:
    top_k: 50
    top_n: 8
    reranker: enabled

chat_rag_high_sensitivity:
  provider: ollama
  model: qwen2.5:7b-instruct
  temperature: 0.3
  max_tokens: 1024
  embedding_model: nomic-embed-text
  retrieval:
    top_k: 50
    top_n: 8
    reranker: bge-reranker-v2-m3

agent_multi_step:
  provider: openai
  model: gpt-4o
  temperature: 0.1
  max_tokens: 2048
  max_steps: 10
  hitl: true
```

### Token counting và Cost estimation

| Model | Input cost | Output cost |
|---|---|---|
| gpt-4o-mini | $0.15/1M tokens | $0.60/1M tokens |
| gpt-4o | $2.50/1M tokens | $10.00/1M tokens |
| text-embedding-3-small | $0.02/1M tokens | – |
| Ollama (self-hosted) | $0 (chỉ GPU cost) | $0 |

**Ước lượng chi phí hàng tháng (cho 60.000 user):**

| Use case | Số request/ngày | Token/request | Chi phí/tháng (OpenAI) | Chi phí/tháng (Ollama local) |
|---|---|---|---|---|
| Tóm tắt văn bản | ~5.000 | 1.500 | $135 | $0 (chỉ GPU) |
| Hỏi-đáp RAG | ~20.000 | 2.000 | $900 | $0 (chỉ GPU) |
| OCR + Extract | ~2.000 | 3.000 | $90 | $0 (chỉ GPU) |
| Agent multi-step | ~500 | 5.000 | $75 | $0 (chỉ GPU) |
| Embedding (RAG ingest) | ~10.000 | 1.000 | $6 | $0 (chỉ GPU) |
| **Tổng** | | | **~$1.200/tháng** | **~$0 token + GPU** |

---

## [AI-B.2.2] **B.2.2. RAG (Retrieval-Augmented Generation) — Pipeline chi tiết**

### Pipeline tổng quan

```
INPUT (câu truy vấn của user)
       │
       ▼
[1] Embed query  ─────────────► embedding model
       │
       ▼
[2] Hybrid Search ─────┬──► BM25 search (PostgreSQL full-text)
                       └──► Vector search (pgvector cosine)
       │
       ▼
[3] RRF Fusion (Reciprocal Rank Fusion) → top 50 candidates
       │
       ▼
[4] Cross-Encoder Reranking  ─────► bge-reranker-v2-m3 → top 10
       │
       ▼
[5] Context Aggregation (token budget, dedup, ordering)
       │
       ▼
[6] Prompt Assembly
       │
       ├── system prompt (role, instructions)
       ├── context (top 10 docs)
       └── user query
       │
       ▼
[7] LLM Generation (streaming)
       │
       ▼
[8] Validation (JSON schema, faithfulness check)
       │
       ▼
[9] Citation Extraction
       │
       ▼
OUTPUT (answer + citations + confidence)
```

### Cấu hình RAG chi tiết

```yaml
rag_pipeline:
  ingestion:
    chunk_size: 512          # tokens per chunk
    chunk_overlap: 50        # tokens overlap
    splitter: recursive      # recursive character splitter
    min_chunk_length: 50     # skip quá ngắn
    clean_html: true         # loại bỏ HTML tag
    extract_metadata: true   # trích tác giả, ngày, ...

  embedding:
    model: text-embedding-3-small   # OpenAI
    # backup: nomic-embed-text (Ollama)
    batch_size: 100
    normalize: true          # cosine similarity
    dimensions: 1536

  retrieval:
    bm25:
      k1: 1.5
      b: 0.75
      language: vietnamese
    vector:
      metric: cosine
      ef_search: 100
      probes: 10
    hybrid:
      fusion: rrf
      k_constant: 60
      weights:
        bm25: 0.4
        vector: 0.6

  reranking:
    enabled: true
    model: bge-reranker-v2-m3   # hoặc cross-encoder/ms-marco-MiniLM
    top_k_from_retrieval: 50
    top_n_after_rerank: 8

  context_aggregation:
    token_budget: 2048         # max token cho context
    deduplicate: true
    order_by: relevance
    include_metadata: true

  generation:
    model: gpt-4o-mini
    temperature: 0.3
    max_tokens: 1024
    streaming: true

  validation:
    faithfulness_check: true   # detect hallucination
    citation_extraction: true
    json_schema_validation: false   # chỉ bật cho function-calling
```

### Chunking strategies

```python
class ChunkStrategy:
    # 1. Recursive character splitter (mặc định)
    recursive = {
        "separators": ["\n\n", "\n", ". ", "? ", "! ", " "],
        "chunk_size": 512,
        "chunk_overlap": 50,
    }

    # 2. Semantic chunking (chia theo đoạn văn có chủ đề)
    semantic = {
        "model": "text-embedding-3-small",
        "similarity_threshold": 0.7,
        "min_chunk": 100,
        "max_chunk": 1000,
    }

    # 3. Layout-aware (theo header/section của văn bản)
    layout_aware = {
        "preserve_sections": True,
        "include_headers_in_chunk": True,
    }
```

### RAGAS Evaluation

Đánh giá chất lượng hệ thống RAG theo framework **RAGAS** (Retrieval-Augmented Generation Assessment):

| Metric | Mô tả | Mục tiêu |
|---|---|---|
| **Context Precision** | Tỷ lệ context thực sự liên quan / tổng context retrieved | ≥ 0.85 |
| **Context Recall** | Tỷ lệ context cần thiết được retrieved | ≥ 0.80 |
| **Faithfulness** | Câu trả lời có trung thực với context không (không bịa) | ≥ 0.90 |
| **Answer Relevancy** | Câu trả lời có liên quan đến câu hỏi không | ≥ 0.85 |

---

## [AI-B.2.3] **B.2.3. AI Agent — Multi-step Task với MCP Tool**

### Khái niệm AI Agent

**Agent** là LLM có khả năng:
- **Lập kế hoạch** (planning): phân rã yêu cầu thành các bước.
- **Gọi công cụ** (tool calling): thực thi hành động qua MCP.
- **Tự ra quyết định** (decision making): chọn bước tiếp theo dựa trên kết quả.
- **Học từ phản hồi** (reflection): đánh giá kết quả và điều chỉnh.

### Các Agent trong hệ thống

| Agent | Vai trò | Tools sử dụng |
|---|---|---|
| **DocRetrieverAgent** | Tìm văn bản trong DMS | search_doc, get_document, ocr_extract |
| **AnalyzerAgent** | Phân tích dữ liệu | nl2sql, summarize, classify |
| **WriterAgent** | Sinh nội dung | generate_report, draft_email, generate_excel |
| **SchedulerAgent** | Lập lịch | fetch_calendar, find_slot, create_meeting |
| **NotifierAgent** | Gửi thông báo | send_email, send_teams, send_sms |
| **AuditorAgent** | Kiểm tra tuân thủ | check_policy, check_security_level |

### Luồng Agent (multi-step)

```
User: "Tìm văn bản quá hạn trong tuần, phân loại theo đơn vị, gửi báo cáo cho lãnh đạo"

[1] Intent Router phân loại → "complex multi-step task"
[2] Task Planner sinh plan:
    {
      "steps": [
        {"agent": "DocRetrieverAgent", "tool": "search_doc", "params": {"query": "quá hạn", "filter": "status=overdue AND date>=last_week"}},
        {"agent": "AnalyzerAgent", "tool": "classify", "params": {"by": "organization_unit"}},
        {"agent": "WriterAgent", "tool": "generate_excel", "params": {"data": "step2.result", "format": "xlsx"}},
        {"agent": "WriterAgent", "tool": "draft_email", "params": {"to": "leadership", "attachment": "step3.file"}},
        {"agent": "NotifierAgent", "tool": "send_email", "params": {"hitl": true}}
      ]
    }

[3] Coordinator lặp qua từng step:
    Step 1: DocRetrieverAgent → 23 văn bản quá hạn
    Step 2: AnalyzerAgent → group theo 5 đơn vị
    Step 3: WriterAgent → Excel file (24 KB)
    Step 4: WriterAgent → Email draft (subject + body)
    Step 5: HITL → chờ user duyệt email → gửi

[4] Final Answer từ Coordinator: "Đã chuẩn bị báo cáo, email đang chờ duyệt."
```

### Human-in-the-Loop (HITL)

Với thao tác **write** (gửi email, xóa, cập nhật), Agent **KHÔNG tự ý thực hiện** mà dừng chờ user duyệt qua giao diện:

```
┌─────────────────────────────────────────────────────┐
│  Agent muốn thực hiện: send_email                   │
│                                                     │
│  TO: lanhdaobo@btch.gov.vn                          │
│  SUBJECT: Báo cáo văn bản quá hạn tuần 27          │
│  BODY: Kính gửi Lãnh đạo Bộ,...                    │
│  ATTACHMENT: baocao_quahan_2026W27.xlsx             │
│                                                     │
│  [✅ Duyệt và gửi]  [✏️ Sửa]  [❌ Hủy]             │
└─────────────────────────────────────────────────────┘
```

### Safety Guardrails

Agent có các **rào chắn an toàn**:

```python
SAFETY_RULES = [
    {"action": "delete_*", "block": True, "reason": "Không được xóa dữ liệu"},
    {"action": "drop_*", "block": True, "reason": "Không được drop table"},
    {"action": "truncate_*", "block": True, "reason": "Không được truncate"},
    {"action": "send_email", "block": False, "require_hitl": True},
    {"action": "update_*", "block": False, "require_hitl": True, "audit": True},
    {"action": "external_api", "block": False, "require_hitl": True, "audit": True},
]
```

---

## [AI-B.2.4] **B.2.4. MCP (Model Context Protocol) Server**

### Khái niệm MCP

**MCP** là giao thức chuẩn (do Anthropic đề xuất) cho phép Agent gọi **tools** và truy cập **resources** một cách thống nhất. MCP sử dụng **JSON-RPC** qua HTTP hoặc stdio.

### Cấu trúc MCP Tool

```json
{
  "name": "search_doc",
  "description": "Tìm kiếm văn bản trong hệ thống DMS",
  "inputSchema": {
    "type": "object",
    "properties": {
      "query": {"type": "string", "description": "Câu truy vấn"},
      "tenant_id": {"type": "string", "description": "Tenant ID (auto-fill)"},
      "top_k": {"type": "integer", "default": 10, "minimum": 1, "maximum": 50},
      "filters": {
        "type": "object",
        "properties": {
          "document_type": {"type": "string"},
          "date_from": {"type": "string", "format": "date"},
          "date_to": {"type": "string", "format": "date"},
          "security_level": {"type": "string"}
        }
      }
    },
    "required": ["query"]
  },
  "auth": {
    "type": "user",
    "scopes": ["doc:read"]
  },
  "hitl": false,
  "audit": true
}
```

### Danh sách 12 MCP Tools chính

| Tool | Mô tả | HITL | Audit |
|---|---|---|---|
| `search_doc` | Tìm văn bản theo từ khóa/ngữ nghĩa | Không | Có |
| `get_document` | Lấy nội dung đầy đủ 1 văn bản | Không | Có |
| `summarize` | Tóm tắt văn bản | Không | Có |
| `classify` | Phân loại văn bản | Không | Có |
| `ocr_extract` | Trích xuất từ văn bản scan | Không | Có |
| `list_user_tasks` | Danh sách công việc của user hiện tại | Không | Có |
| `create_worklog` | Tạo nhật ký công việc | **Có** | Có |
| `send_email` | Gửi email | **Có** | Có |
| `sign_document` | Ký số văn bản | **Có** | Có |
| `fetch_calendar` | Lấy lịch | Không | Có |
| `create_meeting` | Đặt lịch họp | **Có** | Có |
| `nl2sql` | Sinh SQL từ câu hỏi tự nhiên | Không | Có |

### Giao tiếp MCP

```
Agent Core (MCP client)
    │
    │ JSON-RPC over HTTP
    │ {"jsonrpc": "2.0", "method": "tools/call",
    │  "params": {"name": "search_doc", "arguments": {...}},
    │  "id": 1}
    ▼
MCP Server (this system)
    │
    ├─► Auth check (JWT, scope, tenant)
    ├─► Tool execution (delegate to handler)
    ├─► Audit log
    └─► Response: {"jsonrpc": "2.0", "result": {...}, "id": 1}
```

---

## [AI-B.2.5] **B.2.5. OCR & Document Extraction Pipeline**

### Kiến trúc OCR

```
┌────────────────────────────────────────────────────────────┐
│                  OCR PIPELINE                              │
│                                                            │
│  File input (PDF/PNG/JPG/TIFF)                             │
│       │                                                    │
│       ▼                                                    │
│  [1] File detect & preprocess                              │
│      • DPI normalization (300 DPI)                         │
│      • Deskew                                             │
│      • Denoise                                            │
│       │                                                    │
│       ▼                                                    │
│  [2] OCR Engine                                            │
│      • Tesseract 5+ (mặc định, free)                       │
│      • PaddleOCR (chất lượng cao hơn cho TV)               │
│      • Surya (latest, SOTA cho Latin)                      │
│      Output: text + bounding box + confidence              │
│       │                                                    │
│       ▼                                                    │
│  [3] Layout Parser                                         │
│      • Tách cột, đoạn văn, bảng, header/footer             │
│      • Output: structured blocks                          │
│       │                                                    │
│       ▼                                                    │
│  [4] Table Extraction                                      │
│      • OpenCV line detection                              │
│      • Hoặc Table-Transformer model                       │
│      • Output: CSV / JSON table                           │
│       │                                                    │
│       ▼                                                    │
│  [5] Key-Value Extraction                                  │
│      • Donut / LayoutLM                                   │
│      • Hoặc LLM với prompt trích xuất                    │
│      • Output: JSON {Số ký hiệu, Ngày, ...}               │
│       │                                                    │
│       ▼                                                    │
│  [6] Schema Mapping                                        │
│      • Map về DocumentMetadata schema                     │
│      • Validation (required fields)                       │
│       │                                                    │
│       ▼                                                    │
│  OUTPUT → lưu vào DMS, hiển thị cho user xác nhận        │
└────────────────────────────────────────────────────────────┘
```

### OCR Engines so sánh

| Engine | Ưu điểm | Nhược điểm | Độ chính xác (TV) | Tốc độ |
|---|---|---|---|---|
| **Tesseract 5+** | Miễn phí, dễ tích hợp | TV chưa tốt lắm | 88-92% | Nhanh |
| **PaddleOCR** | Rất tốt cho TV, hỗ trợ layout | Cần Python env | 95-98% | Trung bình |
| **Surya** | SOTA cho tiếng Anh, layout tốt | TV chưa tốt | 90-93% | Trung bình |
| **Donut** | End-to-end, không cần detect | Cần GPU | 92-96% | Chậm |
| **Azure Document Intelligence** | Cloud, rất tốt | Cần cloud, cost | 97-99% | Nhanh |

**Lựa chọn đề xuất:** **PaddleOCR** (chất lượng cao cho tiếng Việt + free + on-prem).

### Ví dụ prompt Key-Value Extraction

```text
Bạn là chuyên gia trích xuất dữ liệu từ văn bản hành chính Việt Nam.

Trích xuất các thông tin sau từ văn bản bên dưới, trả về dưới dạng JSON:

{
  "so_ky_hieu": "...",        // Số ký hiệu văn bản (vd: "123/BTC-TCT")
  "ngay_ban_hanh": "...",     // Ngày ban hành (dd/mm/yyyy)
  "co_quan_ban_hanh": "...",  // Cơ quan ban hành
  "nguoi_ky": "...",          // Người ký
  "chuc_vu_nguoi_ky": "...",  // Chức vụ người ký
  "trich_yeu": "...",         // Trích yếu (1-2 dòng)
  "linh_vuc": "...",          // Lĩnh vực
  "do_mat": "...",            // Độ mật (Công khai/Mật/Tối mật/Tuyệt mật)
  "do_khan": "...",           // Độ khẩn (Bình thường/Khẩn/Hoả tốc)
  "so_trang": 0              // Số trang
}

Nếu trường nào không tìm thấy, để null.
Chỉ trả về JSON, KHÔNG giải thích thêm.

Văn bản:
{{document_text}}
```

---

## [AI-B.2.6] **B.2.6. Provider Abstraction Layer (Multi-LLM)**

### Kiến trúc

```python
class LLMProvider(ABC):
    @abstractmethod
    async def generate(self, prompt: Prompt, **kwargs) -> GenerationResult: ...
    
    @abstractmethod
    async def stream(self, prompt: Prompt, **kwargs) -> AsyncIterator[Token]: ...
    
    @abstractmethod
    async def embed(self, texts: List[str]) -> List[List[float]]: ...
    
    @abstractmethod
    def count_tokens(self, text: str) -> int: ...

class OpenAIProvider(LLMProvider):
    """Wraps openai>=1.0 SDK."""

class OllamaProvider(LLMProvider):
    """Calls local Ollama HTTP API."""

class AnthropicProvider(LLMProvider):
    """Wraps anthropic SDK."""
```

### Provider Selector (logic chọn provider)

```python
def select_provider(request: AiRequest) -> LLMProvider:
    # 1. Check tenant policy
    tenant = tenant_repo.get(request.tenant_id)
    
    # 2. Classify sensitivity
    sensitivity = classify_sensitivity(request.prompt, request.context)
    
    # 3. Force local nếu high sensitivity
    if sensitivity == "high":
        return ollama_provider  # BẮT BUỘC
    
    # 4. Tenant policy cho cloud
    if not tenant.allow_cloud:
        return ollama_provider
    
    # 5. Cost-based routing
    if request.task == "summarize" and tenant.cost_budget_remaining < 0.2:
        return ollama_provider  # Tiết kiệm chi phí
    
    # 6. Latency-based
    if request.max_latency_ms < 3000:
        # Ollama local nhanh hơn cloud (không cần internet)
        return ollama_provider
    
    # 7. Default: OpenAI
    return openai_provider
```

### Failover chain

```python
FAILOVER_CHAIN = {
    "openai": ["openai", "ollama"],
    "ollama": ["ollama", "openai"],
    "anthropic": ["anthropic", "openai", "ollama"],
}

async def call_with_failover(provider: LLMProvider, request):
    chain = FAILOVER_CHAIN[provider.name]
    for provider_name in chain:
        try:
            return await get_provider(provider_name).generate(request)
        except (ProviderError, RateLimitError) as e:
            logger.warn(f"provider {provider_name} failed: {e}, trying next")
    raise AllProvidersDownError()
```

---

## [AI-B.2.7] **B.2.7. Prompt Engineering & Template Management**

### Cấu trúc Prompt Template

```yaml
id: summarize_v1
version: 1.2.0
use_case: summarize
tenant: "*"  # all tenant
language: vi
system: |
  Bạn là cán bộ hành chính Bộ Tài Chính.
  Nhiệm vụ của bạn là tóm tắt văn bản hành chính một cách chính xác, ngắn gọn.

  Nguyên tắc:
  - Tóm tắt 3-5 dòng
  - Giữ nguyên ý chính, không suy diễn
  - Văn phong hành chính, trang trọng
  - Không thêm thông tin mới

user: |
  Văn bản cần tóm tắt:
  {{ document_content }}

  {% if context %}
  Các văn bản tương tự (tham khảo):
  {{ context }}
  {% endif %}

  Trích yếu:

input_variables:
  - name: document_content
    type: string
    required: true
    max_length: 50000
  - name: context
    type: string
    required: false

output_format: text
max_tokens: 500
temperature: 0.2
stop_sequences: ["\n\n\n"]
```

### Prompt versioning & A/B testing

- Mỗi prompt template có nhiều version.
- Có thể chạy A/B test: 50% traffic → version A, 50% → version B.
- Theo dõi chỉ số (chất lượng, latency) để quyết định rollout.

---

## [AI-B.2.8] **B.2.8. Knowledge Base & Vector Database**

### Cấu trúc Knowledge Source

```sql
-- PostgreSQL metadata
CREATE TABLE ai_platform.knowledge_source (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL,
    name VARCHAR(200),
    source_type VARCHAR(50),         -- dms / file / web / hr / ckan
    connection_config JSONB,         -- thông tin kết nối
    chunking_config JSONB,           -- chunk_size, overlap, strategy
    embedding_model VARCHAR(100),    -- text-embedding-3-small
    schedule_cron VARCHAR(50),       -- lịch ingestion định kỳ
    status VARCHAR(20),              -- pending/running/done/failed
    last_ingest_at TIMESTAMPTZ,
    total_chunks INTEGER,
    error_message TEXT,
    created_by UUID,
    created_at TIMESTAMPTZ,
    updated_at TIMESTAMPTZ
);

-- pgvector table (trong cùng schema)
CREATE TABLE ai_platform.knowledge_chunk (
    id UUID PRIMARY KEY,
    source_id UUID NOT NULL,
    tenant_id UUID NOT NULL,
    doc_id VARCHAR(200),             -- ID gốc trong DMS
    chunk_index INTEGER,
    content TEXT,
    content_hash VARCHAR(64),        -- SHA-256
    embedding vector(1536),          -- pgvector
    metadata JSONB,                  -- {author, date, security_level, ...}
    created_at TIMESTAMPTZ
);

CREATE INDEX ON ai_platform.knowledge_chunk 
    USING ivfflat (embedding vector_cosine_ops) 
    WITH (lists = 100);

CREATE INDEX ON ai_platform.knowledge_chunk (tenant_id);
CREATE INDEX ON ai_platform.knowledge_chunk (source_id);
CREATE INDEX ON ai_platform.knowledge_chunk USING gin (to_tsvector('vietnamese', content));
```

### Ingestion flow

```python
async def ingest_source(source_id: UUID):
    source = await knowledge_repo.get(source_id)
    
    # 1. Extract documents
    docs = await extractor.extract(source.connection_config)
    # Có thể là: DMS API, file upload, web scraper
    
    # 2. Chunk
    chunks = chunker.split(
        docs,
        size=source.chunking_config['chunk_size'],
        overlap=source.chunking_config['chunk_overlap'],
        strategy=source.chunking_config.get('strategy', 'recursive'),
    )
    
    # 3. Embed (batch)
    embeddings = []
    for batch in batched(chunks, 100):
        vecs = await embedding_provider.embed([c.content for c in batch])
        embeddings.extend(vecs)
    
    # 4. Store
    for chunk, vec in zip(chunks, embeddings):
        await pgvector.upsert({
            "source_id": source.id,
            "tenant_id": source.tenant_id,
            "content": chunk.content,
            "content_hash": hash(chunk.content),
            "embedding": vec,
            "metadata": chunk.metadata,
        })
    
    # 5. Update source status
    source.status = "done"
    source.total_chunks = len(chunks)
    source.last_ingest_at = now()
    await knowledge_repo.save(source)
```

---

# 🔷 PHẦN C — HẠ TẦNG CẦN THIẾT CHO AI MODULE

---

## [AI-C.1.1] **C.1.1. Tổng quan hạ tầng**

Hạ tầng cho AI Module cần **mở rộng** từ hạ tầng SmartOffice Backend hiện hữu (xem `II.1.8`), tập trung vào:

| Thành phần | Mới | Mục đích |
|---|---|---|
| **GPU Node** | ✅ Mới | Chạy Ollama LLM local |
| **Vector DB** | ✅ Mới | pgvector (có sẵn) hoặc Qdrant cluster |
| **AI Worker Pool** | ✅ Mới | Chạy OCR, ingestion, agent |
| **MCP Server** | ✅ Mới | Expose tool cho agent |
| **LLM API Gateway** | ✅ Mới | AI Gateway |
| **Cache semantic** | ✅ Mới | Redis với vector similarity |
| **Knowledge Ingestion Pipeline** | ✅ Mới | ETL cho tri thức |

---

## [AI-C.2.1] **C.2.1. GPU Node — Hạ tầng chạy LLM local**

### Cấu hình GPU đề xuất

**Cho 60.000 user, tải trung bình 100.000 LLM request/ngày:**

| Hạ tầng | Cấu hình | Công dụng | Số lượng | Đơn giá (VNĐ) |
|---|---|---|---|---|
| **NVIDIA A10** | 24 GB VRAM, 150W | Chạy `qwen2.5:7b-instruct`, `nomic-embed-text` | 2 | ~150 triệu/node |
| **NVIDIA L4** | 24 GB VRAM, 72W | Chạy `qwen2.5:7b-instruct`, tiết kiệm điện | 2 (backup) | ~120 triệu/node |
| **NVIDIA A100** | 40/80 GB VRAM, 300W | Chạy model lớn `qwen2.5:32b`, `llama3.1:70b` (optional) | 0-1 | ~400 triệu/node |

**Khuyến nghị: 2 × A10** (đủ cho hầu hết use case, dự phòng 1).

### So sánh GPU models

| Model | VRAM cần | Throughput | Latency (avg) | Chất lượng (TV) |
|---|---|---|---|---|
| `qwen2.5:7b-instruct` | ~5 GB | ~40 tokens/s | 2-3s/response | Tốt (9/10) |
| `llama3.2:3b` | ~3 GB | ~80 tokens/s | 1s/response | Khá (7/10) |
| `qwen2.5:14b-instruct` | ~10 GB | ~25 tokens/s | 4-5s/response | Rất tốt (9.5/10) |
| `qwen2.5:32b-instruct` | ~20 GB | ~12 tokens/s | 8-10s/response | Xuất sắc (9.8/10) |
| `llama3.1:70b-instruct` | ~40 GB | ~5 tokens/s | 20s/response | Xuất sắc (9.8/10) |

### Ollama configuration

```yaml
# /etc/ollama/config.yaml (hoặc env)
OLLAMA_HOST: 0.0.0.0:11434
OLLAMA_KEEP_ALIVE: 30m         # giữ model trong VRAM 30 phút
OLLAMA_NUM_PARALLEL: 4          # song song 4 request
OLLAMA_MAX_LOADED_MODELS: 3     # tối đa 3 model trong VRAM
OLLAMA_GPU_LAYERS: 99           # dùng GPU hoàn toàn

# Models preload
PRELOAD:
  - qwen2.5:7b-instruct-q5_K_M     # quantized ~5GB
  - nomic-embed-text-v1.5-q
  - bge-reranker-v2-m3
```

### Load balancing Ollama cluster

```
                ┌──────────────────┐
                │  Ollama Load     │
                │  Balancer        │
                │  (nginx/native)  │
                └────────┬─────────┘
                         │
        ┌────────────────┼────────────────┐
        │                │                │
        ▼                ▼                ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│  Ollama #1   │  │  Ollama #2   │  │  Ollama #3   │
│  A10 GPU     │  │  A10 GPU     │  │  A10 GPU     │
│  qwen2.5:7b  │  │  qwen2.5:14b │  │  embedding   │
└──────────────┘  └──────────────┘  └──────────────┘
```

---

## [AI-C.2.2] **C.2.2. Vector Database — pgvector & Qdrant**

### Lựa chọn Vector DB

| Tính năng | pgvector (PostgreSQL ext) | Qdrant |
|---|---|---|
| Triển khai | Đơn giản (cùng PostgreSQL) | Riêng (cluster) |
| Hiệu năng | Tốt (< 10M vector) | Rất tốt (> 10M vector) |
| Hỗ trợ filter | Native SQL | Native |
| Backup | Cùng PostgreSQL backup | Riêng |
| Scale | Hạn chế | Cluster dễ |
| Chi phí | $0 (open source) | $0 (open source) |

**Khuyến nghị cho Bộ Tài Chính:**
- **Giai đoạn 1 (< 5M chunks)**: dùng **pgvector** (đơn giản, cùng DB hiện hữu).
- **Giai đoạn 2 (> 5M chunks)**: dùng **Qdrant cluster** riêng.

### Cấu hình pgvector

```sql
-- Extension
CREATE EXTENSION IF NOT EXISTS vector;

-- Schema riêng
CREATE SCHEMA ai_platform;

-- Bảng vector
CREATE TABLE ai_platform.knowledge_chunk (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    source_id UUID NOT NULL,
    doc_id VARCHAR(200),
    chunk_index INTEGER,
    content TEXT NOT NULL,
    content_hash CHAR(64) NOT NULL,
    embedding vector(1536),     -- text-embedding-3-small dimension
    metadata JSONB DEFAULT '{}',
    created_at TIMESTAMPTZ DEFAULT now()
);

-- Index cho cosine similarity
CREATE INDEX knowledge_chunk_embedding_idx 
    ON ai_platform.knowledge_chunk 
    USING ivfflat (embedding vector_cosine_ops) 
    WITH (lists = 100);

-- Index cho BM25 full-text (tiếng Việt)
CREATE INDEX knowledge_chunk_content_fts_idx 
    ON ai_platform.knowledge_chunk 
    USING gin (to_tsvector('simple', content));

-- Row-Level Security
ALTER TABLE ai_platform.knowledge_chunk ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation ON ai_platform.knowledge_chunk
    USING (tenant_id = current_setting('app.tenant_id')::uuid);

-- Query example
SET app.tenant_id = 'xxx';
SELECT id, content, 1 - (embedding <=> $1::vector) AS similarity
FROM ai_platform.knowledge_chunk
WHERE tenant_id = current_setting('app.tenant_id')::uuid
ORDER BY embedding <=> $1::vector
LIMIT 50;
```

### Cấu hình Qdrant cluster (khi scale lên)

```yaml
# docker-compose qdrant
services:
  qdrant:
    image: qdrant/qdrant:v1.10.0
    deploy:
      replicas: 3
      resources:
        limits:
          memory: 8G
    environment:
      QDRANT__SERVICE__GRPC_PORT: 6334
      QDRANT__STORAGE__PERFORMANCE__OPTIMIZERS__INDEXING_THRESHOLD: 20000
    volumes:
      - qdrant_data:/qdrant/storage

  # Cho production với sharding
  qdrant-coordinator:
    image: qdrant/qdrant:v1.10.0
    command: ./qdrant --uri http://qdrant:6333
```

---

## [AI-C.2.3] **C.2.3. Semantic Cache (Redis)**

### Mục đích

Cache lại response cho những câu hỏi có **ngữ nghĩa tương tự**, tiết kiệm chi phí LLM và giảm latency.

### Cấu trúc

```python
class SemanticCache:
    """Cache lại response theo embedding similarity."""
    
    def __init__(self, redis, embed_provider):
        self.redis = redis
        self.embed = embed_provider
    
    async def get(self, query: str, threshold: float = 0.95) -> Optional[CachedResponse]:
        q_vec = await self.embed.encode(query)
        # Search for similar cached query
        results = await self.redis.search_vector(
            "semantic_cache",
            q_vec,
            top_k=1,
            threshold=threshold,
        )
        if results:
            cached = await self.redis.get(f"semantic_cache:{results[0].id}")
            return json.loads(cached)
        return None
    
    async def set(self, query: str, response: dict, ttl: int = 3600):
        q_vec = await self.embed.encode(query)
        cache_id = hashlib.md5(query.encode()).hexdigest()
        await self.redis.set(
            f"semantic_cache:{cache_id}",
            json.dumps(response),
            ex=ttl,
        )
        await self.redis.add_vector("semantic_cache", cache_id, q_vec)
```

### Cache hit rate (kỳ vọng)

| Use case | Cache hit rate |
|---|---|
| Tóm tắt văn bản | ~30% (nhiều văn bản unique) |
| Hỏi-đáp RAG | ~40% (câu hỏi lặp lại nhiều) |
| OCR | ~10% (file khác nhau) |

---

## [AI-C.2.4] **C.2.4. AI Worker Pool — xử lý async job**

### Mục đích

Worker pool chạy các tác vụ AI **không đồng bộ**:
- Tóm tắt văn bản dài (1-3 phút)
- OCR sách dày 100 trang (5-10 phút)
- Ingestion 10.000 file vào Knowledge Base
- Agent multi-step task (timeout 10 phút)

### Kiến trúc

```
┌────────────────────────────────────────────────────────────┐
│                  AI WORKER POOL                            │
│                                                            │
│  Kafka topic: ai.jobs                                      │
│  ├─► summarize.jobs                                       │
│  ├─► ocr.jobs                                             │
│  ├─► ingest.jobs                                          │
│  └─► agent.jobs                                           │
│                                                            │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐        │
│  │ Worker #1   │  │ Worker #2   │  │ Worker #N   │        │
│  │ (CPU/GPU)   │  │ (CPU/GPU)   │  │ (CPU/GPU)   │        │
│  │ Auto-scale  │  │ Auto-scale  │  │ Auto-scale  │        │
│  └─────────────┘  └─────────────┘  └─────────────┘        │
└────────────────────────────────────────────────────────────┘
```

### Cấu hình

```yaml
worker_pool:
  min_workers: 2
  max_workers: 16
  scale_up_threshold: queue_depth > 100
  scale_down_threshold: queue_depth < 10
  job_timeout: 600s          # 10 phút tối đa
  retry:
    max_attempts: 3
    backoff: exponential
    initial_delay: 5s
  dead_letter_queue: ai.jobs.dlq
```

### Triển khai với MassTransit (.NET)

```csharp
public class SummarizeJobConsumer : IConsumer<SummarizeJob>
{
    public async Task Consume(ConsumeContext<SummarizeJob> context)
    {
        var job = context.Message;
        
        // Update ai_job status → processing
        await jobRepo.UpdateStatus(job.Id, "processing");
        
        try
        {
            // Run AI
            var result = await aiEngine.SummarizeAsync(job.Content);
            
            // Update status → done
            await jobRepo.UpdateStatus(job.Id, "done", result);
            
            // Notify user via WebSocket
            await notifier.SendToUser(job.UserId, result);
        }
        catch (Exception ex)
        {
            if (job.RetryCount < 3)
            {
                // Retry
                await context.Publish(job.WithRetry());
            }
            else
            {
                // DLQ
                await jobRepo.UpdateStatus(job.Id, "failed", ex.Message);
                await context.Publish(job.ToDeadLetter());
            }
        }
    }
}
```

---

## [AI-C.2.5] **C.2.5. AI Gateway — Cổng tiếp nhận**

### Mục đích

Là **single entry point** cho mọi AI request, đảm bảo:
- AuthN/AuthZ
- Rate-limit per user/tenant
- Prompt sanitization
- Provider selection
- Caching
- Audit

### Cấu hình Kong Gateway

```yaml
# Kong route cho AI
routes:
  - name: ai-gateway
    paths:
      - /v1/ai
    service: ai-gateway-service
    plugins:
      - name: jwt
        config:
          key_claim_name: kid
          secret_is_base64: false
      
      - name: rate-limiting
        config:
          minute: 100           # 100 req/phút/user
          hour: 5000            # 5000 req/giờ/user
          policy: redis
          redis_host: redis
      
      - name: request-transformer
        config:
          add:
            headers:
              X-Tenant-ID: $(headers.x-tenant-id)
              X-Trace-Id: $(request_id)
```

### AI Gateway service (Docker)

```yaml
# docker-compose
services:
  ai-gateway:
    build:
      context: ./src/Services/AI/SmartOffice.AIPlatform.Api
    environment:
      OPENAI_API_KEY: ${OPENAI_API_KEY}
      OLLAMA_BASE_URL: http://ollama-cluster:11434
      REDIS_URL: redis://redis:6379
      POSTGRES_URL: ${POSTGRES_URL}
      JWT_AUDIENCE: SmartOfficeAPI
      SEMANTIC_CACHE_TTL: 3600
    deploy:
      replicas: 4
      resources:
        limits:
          cpus: '4'
          memory: 8G
    ports:
      - "5007:8080"
```

---

## [AI-C.2.6] **C.2.6. MCP Server — Tool cho Agent**

### Triển khai MCP Server

```python
from mcp.server import Server, Tool
import asyncio

server = Server("smartoffice-ai-mcp")

@server.tool(
    name="search_doc",
    description="Tìm kiếm văn bản trong hệ thống DMS",
    input_schema={
        "type": "object",
        "properties": {
            "query": {"type": "string"},
            "top_k": {"type": "integer", "default": 10}
        },
        "required": ["query"]
    }
)
async def search_doc(query: str, top_k: int = 10, ctx: Context = None):
    # Get tenant from JWT context
    tenant_id = ctx.user.tenant_id
    
    # Call retrieval service
    results = await retrieval_service.search(
        query=query,
        tenant_id=tenant_id,
        top_k=top_k,
    )
    
    # Audit
    await audit_log.record(
        user=ctx.user.id,
        action="tool.search_doc",
        params={"query": query, "top_k": top_k},
        result_count=len(results),
    )
    
    return {"results": results}

if __name__ == "__main__":
    server.run()
```

### MCP Server deployment

```yaml
services:
  mcp-server:
    build:
      context: ./src/Services/AI/SmartOffice.AIPlatform.McpServer
    environment:
      DATABASE_URL: ${POSTGRES_URL}
      REDIS_URL: redis://redis:6379
      JWT_PUBLIC_KEY: ${JWT_PUBLIC_KEY}
    deploy:
      replicas: 2
      resources:
        limits:
          cpus: '2'
          memory: 4G
    ports:
      - "5008:8080"
```

---

## [AI-C.2.7] **C.2.7. Mô hình triển khai Kubernetes**

### K8s manifests

```yaml
# ai-gateway-deployment.yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: ai-gateway
  namespace: smartoffice-ai
spec:
  replicas: 4
  selector:
    matchLabels:
      app: ai-gateway
  template:
    metadata:
      labels:
        app: ai-gateway
    spec:
      containers:
      - name: ai-gateway
        image: mbfs/smartoffice-ai-gateway:1.0.0
        ports:
        - containerPort: 8080
        env:
        - name: OpenAI__ApiKey
          valueFrom:
            secretKeyRef:
              name: ai-secrets
              key: openai-api-key
        - name: Ollama__BaseUrl
          value: "http://ollama-cluster.smartoffice-ai:11434"
        resources:
          requests:
            cpu: "500m"
            memory: "1Gi"
          limits:
            cpu: "4"
            memory: "8Gi"
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
---
apiVersion: v1
kind: Service
metadata:
  name: ai-gateway
  namespace: smartoffice-ai
spec:
  selector:
    app: ai-gateway
  ports:
  - port: 8080
    targetPort: 8080
```

### Ollama GPU deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: ollama
  namespace: smartoffice-ai
spec:
  replicas: 2
  selector:
    matchLabels:
      app: ollama
  template:
    metadata:
      labels:
        app: ollama
    spec:
      nodeSelector:
        gpu: "true"          # chỉ schedule trên node có GPU
      containers:
      - name: ollama
        image: ollama/ollama:latest
        ports:
        - containerPort: 11434
        env:
        - name: OLLAMA_KEEP_ALIVE
          value: "30m"
        - name: OLLAMA_NUM_PARALLEL
          value: "4"
        resources:
          limits:
            nvidia.com/gpu: 1   # 1 GPU A10 mỗi pod
            cpu: "8"
            memory: "16Gi"
        volumeMounts:
        - name: ollama-data
          mountPath: /root/.ollama
      volumes:
      - name: ollama-data
        persistentVolumeClaim:
          claimName: ollama-pvc
```

---

## [AI-C.2.8] **C.2.8. Bảo mật AI — chi tiết**

### Prompt Injection Defense

```python
class PromptInjectionGuard:
    PATTERNS = [
        r"ignore previous instructions",
        r"forget everything",
        r"new persona:",
        r"system:",
        r"\\n\\nHuman:",
        r"</s>|</\|im_end\|>",
        # Tiếng Việt
        r"bỏ qua (các )?hướng dẫn",
        r"quên đi",
        r"thay đổi vai trò",
    ]
    
    def sanitize(self, user_input: str) -> str:
        for pattern in self.PATTERNS:
            if re.search(pattern, user_input, re.IGNORECASE):
                raise PromptInjectionError(f"Suspicious pattern: {pattern}")
        # Giới hạn length
        if len(user_input) > 10000:
            raise ValueError("Input too long")
        return user_input
```

### PII Redaction

```python
PII_PATTERNS = {
    "email": r"\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b",
    "phone": r"\b(0|\+84)[3|5|7|8|9][0-9]{8}\b",
    "id_number": r"\b[0-9]{9,12}\b",  # CMND/CCCD
    "tax_code": r"\b[0-9]{10}[-]?[0-9]{3}\b",
}

def redact_pii(text: str) -> str:
    for label, pattern in PII_PATTERNS.items():
        text = re.sub(pattern, f"[REDACTED_{label.upper()}]", text)
    return text
```

### API Key rotation

```yaml
openai:
  rotation:
    enabled: true
    schedule: monthly
    key_storage: hashicorp-vault
    keys:
      - name: openai-key-2026-q3
        created_at: 2026-07-01
        expires_at: 2026-09-30
      - name: openai-key-2026-q4
        created_at: 2026-10-01
        expires_at: 2026-12-31
```

---

## [AI-C.3.1] **C.3.1. Monitoring & Alerting cho AI**

### Metrics quan trọng

```yaml
ai_metrics:
  # Latency
  - ai_request_duration_seconds (histogram, labels: model, use_case, status)
  - ai_llm_first_token_seconds (gauge)
  
  # Throughput
  - ai_requests_total (counter, labels: use_case, status)
  - ai_tokens_total (counter, labels: model, type=input|output)
  
  # Quality
  - ai_ragas_context_precision (gauge)
  - ai_ragas_context_recall (gauge)
  - ai_ragas_faithfulness (gauge)
  - ai_ragas_answer_relevancy (gauge)
  
  # Cost
  - ai_cost_usd_total (counter, labels: tenant_id, model)
  
  # Errors
  - ai_provider_errors_total (counter, labels: provider, error_type)
  - ai_job_failures_total (counter, labels: job_type)
  
  # Queue
  - ai_jobs_pending (gauge)
  - ai_jobs_processing (gauge)
  - ai_jobs_dlq (gauge)
  
  # Cache
  - ai_semantic_cache_hits_total (counter)
  - ai_semantic_cache_misses_total (counter)
```

### Grafana Dashboard

```yaml
dashboards:
  - name: "AI Overview"
    panels:
      - title: "Request rate by use case"
        type: graph
        query: sum(rate(ai_requests_total[5m])) by (use_case)
      
      - title: "p95 latency by model"
        type: graph
        query: histogram_quantile(0.95, sum(rate(ai_request_duration_seconds_bucket[5m])) by (model, le))
      
      - title: "Cost per tenant (USD/day)"
        type: barchart
        query: sum(increase(ai_cost_usd_total[24h])) by (tenant_id)
      
      - title: "RAGAS scores"
        type: gauge
        query: avg(ai_ragas_faithfulness)
      
      - title: "Queue depth"
        type: graph
        query: ai_jobs_pending
```

### Alert rules

```yaml
alerts:
  - name: AIHighLatencyP95
    condition: p95(ai_request_duration_seconds) > 10s for 5m
    severity: warning
    action: notify on-call
  
  - name: AIHighErrorRate
    condition: rate(ai_provider_errors_total[5m]) > 0.05
    severity: critical
    action: page on-call
  
  - name: AIBudgetExceeded
    condition: increase(ai_cost_usd_total[24h]) > budget_threshold
    severity: warning
    action: notify admin
  
  - name: AIOllamaGpuDown
    condition: ollama_up == 0
    severity: critical
    action: page on-call
  
  - name: AIQueueBackup
    condition: ai_jobs_pending > 1000 for 10m
    severity: warning
    action: scale workers
```

---

## [AI-C.3.2] **C.3.2. Chi phí triển khai ước tính (CapEx + OpEx)**

### Chi phí đầu tư ban đầu (CapEx)

| Hạng mục | Số lượng | Đơn giá (VNĐ) | Thành tiền (VNĐ) |
|---|---|---|---|
| GPU Server (NVIDIA A10) | 2 | 150 triệu | 300 triệu |
| AI Gateway server | 4 | 30 triệu | 120 triệu |
| AI Worker pool | 8 | 25 triệu | 200 triệu |
| OCR Worker | 2 | 25 triệu | 50 triệu |
| MCP Server | 2 | 20 triệu | 40 triệu |
| **Tổng CapEx (hạ tầng)** | | | **~710 triệu** |
| | | | |
| Phát triển AI module (6 tháng) | 1 team | – | ~2 tỷ |
| Tích hợp & test (2 tháng) | 1 team | – | ~500 triệu |
| **Tổng CapEx (triển khai)** | | | **~3.2 tỷ** |

### Chi phí vận hành hàng năm (OpEx)

| Hạng mục | Chi phí/năm (VNĐ) |
|---|---|
| **OpenAI API** (ước ~$1.200/tháng) | ~360 triệu |
| **Điện GPU** (~2 × 150W × 24h × 365d × 2500đ/kWh) | ~6.5 triệu |
| **Vận hành hạ tầng** (DevOps, monitoring) | ~200 triệu |
| **Bảo trì, nâng cấp model, training** | ~300 triệu |
| **Tổng OpEx/năm** | **~870 triệu** |

### Tổng chi phí 5 năm (TCO)

- **CapEx (1 lần)**: ~3.2 tỷ
- **OpEx (5 năm)**: ~4.35 tỷ
- **Tổng TCO 5 năm**: **~7.5 tỷ VNĐ**

### ROI

- **Tiết kiệm nhân sự**: ~30-50% văn thư → tiết kiệm ~5 tỷ/năm
- **Tăng năng suất chuyên viên**: ~20% → tiết kiệm ~2 tỷ/năm
- **Tổng tiết kiệm/năm**: **~7 tỷ/năm**
- **Payback period**: ~12 tháng

---

## [AI-C.3.3] **C.3.3. Lộ trình triển khai (Roadmap)**

### Giai đoạn 1: Foundation (tháng 1-3)
- [x] Phân tích yêu cầu, lựa chọn LLM
- [ ] Mua sắm hạ tầng GPU
- [ ] Triển khai AI Gateway, AI Engine Core cơ bản
- [ ] Tích hợp OpenAI provider
- [ ] Pilot: tóm tắt văn bản cho 1 đơn vị (Tổng cục Thuế)
- [ ] Đánh giá RAGAS baseline

### Giai đoạn 2: Knowledge & RAG (tháng 4-6)
- [ ] Triển khai Knowledge Base + pgvector
- [ ] Ingestion văn bản DMS lịch sử (2020-2025)
- [ ] Triển khai Hybrid Search + Reranking
- [ ] Pilot: ChatOps hỏi-đáp RAG cho 5 đơn vị
- [ ] Tích hợp Ollama local

### Giai đoạn 3: OCR & Agent (tháng 7-9)
- [ ] Triển khai OCR pipeline (PaddleOCR)
- [ ] Triển khai MCP Server + 12 tools
- [ ] Triển khai Agent Orchestration
- [ ] Pilot: Agent multi-step cho chỉ đạo điều hành
- [ ] HITL workflow

### Giai đoạn 4: Production (tháng 10-12)
- [ ] Triển khai toàn Bộ (60.000 user)
- [ ] Đánh giá hiệu năng, điều chỉnh
- [ ] Đào tạo người dùng
- [ ] Tối ưu chi phí, scaling

### Giai đoạn 5: Mở rộng & Tối ưu (năm 2+)
- [ ] Fine-tune model trên corpus BTC
- [ ] Thêm LLM provider (Anthropic, Cohere)
- [ ] Tích hợp thêm hệ thống (Kho bạc, Hải quan)
- [ ] AI cho phân tích dữ liệu lớn

---

# 📋 BẢNG HƯỚNG DẪN PASTE VÀO TÀI LIỆU CHÍNH

> Mở file `BaoCaoTKTC_AI_BTC_Full.docx` trong Word → tìm vị trí → paste nội dung tương ứng.

| Mã đoạn | Nội dung | Paste vào vị trí nào trong tài liệu chính |
|---|---|---|
| | | |
| **PHẦN A — YÊU CẦU AI & BÀI TOÁN** | | |
| `[AI-A.1.1]` | Bối cảnh và lý do áp dụng AI | **Mục I.1 — Mục đích** (sau đoạn về "chuyển đổi số"), hoặc tạo **Mục I.5 mới** sau I.4 |
| `[AI-A.1.2]` | FR-AI-01 → FR-AI-20 | **Mục I.2.1 — Phạm vi nghiệp vụ** (cuối mục) hoặc **Mục I.5 mới** |
| `[AI-A.1.3]` | NFR-AI-01 → NFR-AI-18 | **Mục I.2.5 — Phạm vi kỹ thuật** (mở rộng thêm yêu cầu AI) |
| `[AI-A.2.1]` | BT1 — Tóm tắt & chuẩn hóa | **Mục II.1.1 — Mục tiêu kiến trúc** (bảng mục tiêu trọng tâm) |
| `[AI-A.2.2]` | BT2 — Hỏi-đáp RAG | **Mục II.5.2 — AI Engine Core** (bổ sung vào mô tả) |
| `[AI-A.2.3]` | BT3 — OCR | **Mục II.5.5 — OCR & Document Extraction** |
| `[AI-A.2.4]` | BT4 — Agent multi-step | **Mục II.5.3 — Agent Orchestration** |
| `[AI-A.2.5]` | BT5 — Tìm kiếm ngữ nghĩa | **Mục II.5.4 — Knowledge Base & RAG** |
| `[AI-A.2.6]` | BT6 — An toàn dữ liệu | **Mục II.1.9 — An toàn bảo mật** (bổ sung) |
| | | |
| **PHẦN B — THIẾT KẾ AI** | | |
| `[AI-B.1.1]` | Kiến trúc tổng thể AI Layer | **Mục II.1.3 — Kiến trúc tổng thể** (sau sơ đồ 7 tầng, thêm AI Layer diagram) |
| `[AI-B.1.2]` | Luồng AI request | **Mục II.1.4 — Kiến trúc ứng dụng** (cuối mục) |
| `[AI-B.2.1]` | LLM — Lựa chọn và cấu hình | **Mục II.5.6 — Provider Abstraction** (bảng provider + token cost) |
| `[AI-B.2.2]` | RAG Pipeline chi tiết | **Mục II.5.2 — AI Engine Core** (chi tiết hơn) + **Mục II.5.4** |
| `[AI-B.2.3]` | AI Agent — multi-step + MCP | **Mục II.5.3 — Agent Orchestration** |
| `[AI-B.2.4]` | MCP Server chi tiết | **Mục II.5.8 — MCP Server** |
| `[AI-B.2.5]` | OCR Pipeline | **Mục II.5.5 — OCR** |
| `[AI-B.2.6]` | Provider Abstraction Layer | **Mục II.5.6 — Provider Abstraction** |
| `[AI-B.2.7]` | Prompt Engineering | **Mục II.5.2** (cuối mục) |
| `[AI-B.2.8]` | Knowledge Base & Vector DB | **Mục II.5.4 — Knowledge Base & RAG** |
| | | |
| **PHẦN C — HẠ TẦNG** | | |
| `[AI-C.1.1]` | Tổng quan hạ tầng AI | **Mục II.1.8 — Kiến trúc hạ tầng và triển khai** (đầu mục) |
| `[AI-C.2.1]` | GPU Node | **Mục II.1.8** (bảng thông số hạ tầng — bổ sung GPU) |
| `[AI-C.2.2]` | Vector DB — pgvector & Qdrant | **Mục II.1.7 — Kiến trúc dữ liệu** (cuối mục) |
| `[AI-C.2.3]` | Semantic Cache | **Mục II.5.1 — AI Gateway** (cuối mục) |
| `[AI-C.2.4]` | AI Worker Pool | **Mục II.5.7 — AI Job & Task Queue** |
| `[AI-C.2.5]` | AI Gateway config | **Mục II.5.1 — AI Gateway** |
| `[AI-C.2.6]` | MCP Server deployment | **Mục II.5.8 — MCP Server** |
| `[AI-C.2.7]` | K8s manifests | **Mục II.4 — Mô hình triển khai** |
| `[AI-C.2.8]` | Bảo mật AI | **Mục II.1.9 — An toàn bảo mật** |
| `[AI-C.3.1]` | Monitoring & Alerting | **Mục II.1.10 — Giám sát và vận hành** |
| `[AI-C.3.2]` | Chi phí & ROI | **Mục II.4 — Mô hình triển khai** (cuối) hoặc tạo **Phụ lục A — Chi phí** |
| `[AI-C.3.3]` | Lộ trình triển khai | **Mục II.4 — Mô hình triển khai** (cuối) |

---

## 💡 GỢI Ý CÁCH PASTE NHANH

1. **Mở 2 file cạnh nhau** trong Word:
   - File 1: `BaoCaoTKTC_AI_BTC_Full.docx`
   - File 2: Mở file `.md` này trong VSCode

2. **Tìm đoạn cần paste**: dùng Ctrl+F gõ mã `[AI-X.Y.Z]` trong VSCode.

3. **Tra bảng hướng dẫn ở trên** → biết paste vào mục nào trong Word.

4. **Trong Word**: di chuyển đến đúng mục → Ctrl+V.

5. **Sau khi paste xong**: dùng Heading style của Word để định dạng (nếu muốn dùng auto-TOC).

---

**HẾT FILE TÀI LIỆU RIÊNG VỀ AI**

Nếu cần tách thành file `.docx` riêng, tôi có thể chạy script `convert_md_to_docx.py` để sinh ra.