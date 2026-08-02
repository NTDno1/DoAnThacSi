# TÀI LIỆU THIẾT KẾ CHỨC NĂNG — MODULE AI
## Phục vụ Phần mềm Văn phòng số Bộ Tài Chính

*(Phiên bản 1.0 — Tháng 7 năm 2026)*

---

<!-- TRANG BÌA -->

# Bộ Tài Chính

# TÀI LIỆU
# THIẾT KẾ CHỨC NĂNG

**Hợp đồng số:** ..................

**Gói thầu:** ..................

**Thuộc dự án:** Văn phòng số Bộ Tài Chính

**Tên phân hệ/module:** Module Trí tuệ nhân tạo (AI)

**Chủ đầu tư:** Bộ Tài Chính

**Đơn vị thi công:** Mobifone Solutions

**Phiên bản tài liệu:** 1.0

Hà Nội – 2026

---

<!-- TRANG KÝ -->

# TRANG KÝ

| ĐẠI DIỆN CHỦ ĐẦU TƯ | ĐẠI DIỆN ĐƠN VỊ TƯ VẤN THIẾT KẾ |
|---|---|
| **Bộ Tài Chính** | **Mobifone Solutions** |
| <CHỨC DANH> | <CHỨC DANH> |
| | |
| <Họ và tên> | <Họ và tên> |

| ĐẠI DIỆN ĐƠN VỊ TƯ VẤN GIÁM SÁT | ĐẠI DIỆN ĐƠN VỊ KIỂM THỬ |
|---|---|
| <CHỨC DANH> | <CHỨC DANH> |
| | |
| <Họ và tên> | <Họ và tên> |

| ĐẠI DIỆN NHÀ THẦU |
|---|
| <CHỨC DANH> |
| |
| <Họ và tên> |

---

<!-- BẢNG GHI NHẬN THAY ĐỔI -->

# BẢNG GHI NHẬN THAY ĐỔI TÀI LIỆU

| Ngày thay đổi | Vị trí thay đổi | Lý do | Nguồn gốc | Phiên bản cũ | Mô tả thay đổi | Phiên bản mới |
|---|---|---|---|---|---|---|
| | | | | | | |
| | | | | | | |
| | | | | | | |
| | | | | | | |
| | | | | | | |

---

<!-- LỜI NÓI ĐẦU -->

# LỜI NÓI ĐẦU

Tài liệu này mô tả **module Trí tuệ nhân tạo (AI)** — một phân hệ mở rộng của Phần mềm Văn phòng số Bộ Tài Chính. Module AI có nhiệm vụ bổ sung khả năng "suy nghĩ, hiểu và phản hồi" cho hệ thống văn phòng số hiện tại, giúp cho công tác văn thư, chỉ đạo điều hành và quản trị nội bộ của Bộ trở nên nhanh hơn, chính xác hơn và đỡ tốn công sức hơn.

Tài liệu tuân thủ **cấu trúc mẫu Tài liệu Thiết kế chức năng** do Bộ Tài Chính ban hành, gồm:

- **Phần I — TỔNG QUAN**: mục đích, phạm vi, tài liệu liên quan, thuật ngữ.
- **Phần II — MÔ HÌNH KIẾN TRÚC**: bức tranh tổng thể về cách AI được xây dựng, tích hợp và triển khai.
- **THIẾT KẾ CHI TIẾT CHỨC NĂNG**: trình bày 11 phân hệ thành phần của module AI, mỗi phân hệ có 7 mục mô tả đồng nhất.

Đối tượng đọc chính của tài liệu là **Lãnh đạo Bộ, lãnh đạo các đơn vị, chuyên viên quản trị và các bên tham gia dự án**. Nội dung được viết bằng ngôn ngữ đời thường, đi kèm hình ảnh minh họa, tránh các thuật ngữ kỹ thuật quá chuyên sâu.

**Đơn vị thi công**

**Mobifone Solutions**

---

<!-- MỤC LỤC -->

# MỤC LỤC

| Mục | Tiêu đề | Trang |
|---|---|---|
| | **LỜI NÓI ĐẦU** | |
| | **I. TỔNG QUAN** | |
| I.1 | Mục đích của tài liệu | |
| I.2 | Phạm vi của module AI | |
| I.3 | Tài liệu liên quan | |
| I.4 | Thuật ngữ quan trọng | |
| | **II. MÔ HÌNH KIẾN TRÚC** | |
| II.1 | Bức tranh tổng thể | |
| II.2 | Các chức năng AI | |
| II.3 | Tích hợp với hệ thống hiện có | |
| II.4 | Triển khai và hạ tầng | |
| | **THIẾT KẾ CHI TIẾT CHỨC NĂNG** | |
| II.5 | 11 phân hệ của module AI | |
| | II.5.1 AI Gateway — Cổng tiếp nhận | |
| | II.5.2 AI Engine — Bộ máy xử lý trung tâm | |
| | II.5.3 Agent — Trợ lý AI đa bước | |
| | II.5.4 Knowledge Base — Kho tri thức | |
| | II.5.5 OCR — Nhận dạng văn bản scan | |
| | II.5.6 Provider — Kết nối các mô hình AI | |
| | II.5.7 Job Queue — Hàng đợi tác vụ | |
| | II.5.8 MCP Server — Kho công cụ | |
| | II.5.9 Audit — Giám sát và truy vết | |
| | II.5.10 Admin Console — Bảng điều khiển | |
| | II.5.11 Auth — Phân quyền và bảo mật | |

---

# I. TỔNG QUAN

## I.1. Mục đích của tài liệu

Tài liệu này được viết ra với 4 mục đích chính:

**Một là**, giúp Lãnh đạo Bộ, lãnh đạo các đơn vị và các bên liên quan **hiểu được module AI là gì, hoạt động ra sao, mang lại lợi ích gì** cho công việc hàng ngày.

**Hai là**, làm cơ sở để **đơn vị thi công (Mobifone Solutions) phát triển** module AI đúng theo yêu cầu, đúng tiến độ và đảm bảo chất lượng.

**Ba là**, giúp **đơn vị giám sát và kiểm thử** có đủ thông tin để đánh giá sản phẩm trước khi nghiệm thu.

**Bốn là**, làm căn cứ để **đào tạo người dùng** cuối (cán bộ, chuyên viên, văn thư) sử dụng hiệu quả các tính năng AI khi hệ thống đi vào vận hành.

Về bản chất, module AI được xây dựng dựa trên 3 nguyên tắc lớn:

- **Kế thừa**: AI không phải hệ thống riêng biệt mà là phần mở rộng của Phần mềm Văn phòng số hiện hữu, tận dụng hạ tầng, cơ sở dữ liệu và quy trình nghiệp vụ đang vận hành.
- **Tuân thủ**: AI hoạt động theo đúng các quy định về bảo mật dữ liệu cấp nhà nước, đặc biệt với văn bản có độ mật cao.
- **Mở và linh hoạt**: AI không bị "trói" vào một nhà cung cấp công nghệ duy nhất; có thể chuyển đổi giữa các mô hình AI khác nhau tùy nhu cầu.

## I.2. Phạm vi của module AI

### I.2.1. AI giúp được gì cho công việc?

Module AI phục vụ **8 nhóm nghiệp vụ chính** trong Bộ:

| # | Nhóm nghiệp vụ | Công việc cụ thể AI hỗ trợ |
|---|---|---|
| 1 | **Xử lý văn bản thông minh** | Tóm tắt văn bản dài thành 3-5 dòng; chuẩn hóa văn phong; tự động điền các thông tin mẫu (số văn bản, ngày tháng, cơ quan ban hành, người ký) |
| 2 | **Nhận dạng văn bản scan** | Chuyển PDF scan, ảnh chụp văn bản thành dạng số, có thể chỉnh sửa; trích xuất bảng biểu từ văn bản scan |
| 3 | **Hỏi-đáp ngôn ngữ tự nhiên** | Lãnh đạo, chuyên viên có thể hỏi bằng tiếng Việt tự nhiên (vd: "Cho tôi biết các văn bản về cải cách tiền lương quý 3/2026"), AI sẽ trả lời kèm trích dẫn văn bản gốc |
| 4 | **Tìm kiếm thông minh** | Tìm văn bản theo **ý nghĩa**, không chỉ từ khóa (vd: tìm "chính sách thuế VAT" sẽ ra cả văn bản không chứa từ "VAT" nhưng nói về cùng chủ đề) |
| 5 | **Phân loại tự động** | AI gợi ý loại văn bản, lĩnh vực, độ mật, độ khẩn; gợi ý lãnh đạo phù hợp để phê duyệt |
| 6 | **Trợ lý đa bước (Agent)** | Với yêu cầu phức tạp (vd: "Tìm văn bản quá hạn tuần này, phân loại theo đơn vị, soạn báo cáo, gửi email cho lãnh đạo Bộ"), AI tự lên kế hoạch và thực hiện nhiều bước, có người duyệt ở bước nhạy cảm |
| 7 | **Sinh báo cáo tự động** | AI tổng hợp dữ liệu từ văn bản, hồ sơ → báo cáo Excel/Word/PDF theo mẫu |
| 8 | **Gợi ý thông minh** | Gợi ý văn bản tương tự khi đang xem 1 văn bản; cảnh báo văn bản quá hạn; gợi ý xếp lịch họp |

### I.2.2. AI phục vụ ai?

| Đối tượng | Số lượng (dự kiến) | Cách dùng AI |
|---|---|---|
| Lãnh đạo Bộ và Lãnh đạo các đơn vị | Khoảng 2.000 người | Hỏi-đáp nhanh bằng tiếng Việt; xem tóm tắt văn bản; nhận gợi ý phê duyệt |
| Chuyên viên, văn thư, thư ký | Khoảng 8.000 người | Tóm tắt văn bản, trích xuất thông tin mẫu, nhận dạng văn bản scan, tìm văn bản tương tự |
| Cán bộ quản trị hệ thống | Khoảng 200 người | Cấu hình kho tri thức, quản lý prompt, theo dõi chi phí và audit |
| Lãnh đạo cấp Bộ (quản trị cao cấp) | Khoảng 50 người | Cấu hình chính sách AI toàn hệ thống, duyệt các thay đổi lớn |

**Tổng số người dùng dự kiến: khoảng 60.000 tài khoản** (toàn ngành Tài chính), với **khoảng 100.000 yêu cầu AI mỗi ngày**.

### I.2.3. AI quản lý những loại dữ liệu nào?

Module AI không tạo ra hệ thống dữ liệu riêng biệt. AI **kế thừa và sử dụng dữ liệu có sẵn** của Phần mềm Văn phòng số, đồng thời bổ sung một số dữ liệu mới phục vụ riêng cho hoạt động AI:

- **Kho tri thức (Knowledge Source)**: danh sách các nguồn dữ liệu mà AI được phép đọc (văn bản DMS, file upload, trang web nội bộ, cơ sở dữ liệu HRM, ...).
- **Đoạn tri thức đã vector hóa (Knowledge Chunk)**: văn bản đã được AI "học" và lưu trữ dưới dạng đặc trưng số để tìm kiếm nhanh.
- **Mẫu câu hỏi (Prompt Template)**: các kịch bản hỏi AI đã được soạn sẵn, có phiên bản rõ ràng để dễ điều chỉnh.
- **Định nghĩa trợ lý (Agent Definition)**: các trợ lý AI đặc thù (trợ lý tìm văn bản, trợ lý soạn báo cáo, trợ lý đặt lịch họp, ...).
- **Kho công cụ (Tool Registry)**: các công cụ mà AI được phép sử dụng (tìm văn bản, soạn email, ký số, ...).
- **Cấu hình nhà cung cấp AI (LLM Provider)**: thông tin kết nối tới các mô hình AI (OpenAI, Anthropic, mô hình nội bộ).
- **Lệnh xử lý AI (AI Job)**: các tác vụ AI đang chạy nền (tóm tắt văn bản lớn, OCR văn bản nhiều trang, ...).
- **Nhật ký hoạt động (Audit Log)**: ghi lại mọi yêu cầu AI để phục vụ kiểm tra, truy vết.
- **Theo dõi chi phí (Usage & Cost Log)**: ghi nhận lượng sử dụng và chi phí theo từng đơn vị.

**Yêu cầu bảo mật**: dữ liệu AI được **cô lập theo đơn vị** (Tổng cục Thuế không thấy dữ liệu của Kho bạc), và có **cơ chế phân loại độ mật** để áp dụng chính sách xử lý phù hợp (văn bản "Mật"/"Tối mật" chỉ dùng AI chạy nội bộ, không gửi lên cloud).

### I.2.4. Nguyên tắc thiết kế và vận hành

**Về kỹ thuật:**
- AI được xây dựng trên nền tảng hạ tầng hiện có của Bộ (Kubernetes, cơ sở dữ liệu PostgreSQL, hệ thống sự kiện Kafka).
- AI hỗ trợ nhiều nhà cung cấp mô hình AI khác nhau (gọi là "đa nhà cung cấp"), không bị phụ thuộc vào một hãng duy nhất.
- AI có thể mở rộng theo nhu cầu (thêm máy chủ khi nhiều người dùng, thêm GPU khi cần xử lý nặng).

**Về vận hành:**
- Hệ thống hoạt động ổn định ≥ 99,9% thời gian trong giờ hành chính.
- Khi một mô hình AI gặp sự cố, hệ thống tự động chuyển sang mô hình dự phòng.
- Mọi hoạt động AI đều được ghi log, đo lường và truy vết để phục vụ kiểm tra.

**Về ngân sách và chi phí:**
- Hệ thống theo dõi chi phí sử dụng AI theo từng đơn vị, từng người dùng.
- Có cảnh báo khi chi phí vượt ngưỡng cho phép.
- Hỗ trợ cả mô hình AI miễn phí (chạy nội bộ) lẫn mô hình trả phí (chạy trên cloud) để tối ưu chi phí.

---

## I.3. Tài liệu liên quan

| STT | Tên tài liệu | Nguồn / Mã |
|---|---|---|
| 1 | Quyết định số 258/QĐ-BTC về Chương trình Quản lý văn bản và điều hành | Quyết định 258/QĐ-BTC |
| 2 | Tài liệu chuẩn hóa nghiệp vụ văn bản và điều hành | Tài liệu nghiệp vụ VBĐH |
| 3 | Tài liệu thiết kế tổng thể Phần mềm Văn phòng số Bộ Tài Chính | BTC-VPS-TKTT |
| 4 | Tài liệu phân tích yêu cầu người dùng (URD) | URD v1.0 |
| 5 | Tài liệu thiết kế cơ sở dữ liệu | BTC-VPS-DB |
| 6 | Tài liệu thiết kế API | BTC-VPS-API |
| 7 | Tài liệu SmartOffice Backend Core | Backend Core |
| 8 | Luật An toàn thông tin mạng | Luật 2015 |
| 9 | Luật An ninh mạng | Luật 2018 |
| 10 | Nghị định 13/2023/NĐ-CP về bảo vệ dữ liệu cá nhân | NĐ 13/2023 |
| 11 | Quy chế quản lý và sử dụng chữ ký số chuyên dùng | Quy chế ký số |
| 12 | Tiêu chuẩn kỹ thuật quốc gia về tài liệu số | TCVN |
| 13 | Tài liệu hướng dẫn tích hợp Cổng dịch vụ công quốc gia | Cổng DVCQG |
| 14 | Tài liệu kỹ thuật về các mô hình AI (OpenAI, Anthropic, mô hình mã nguồn mở) | OpenAI / Anthropic / Ollama |
| 15 | Tài liệu về Vector Database | pgvector / Qdrant |
| 16 | Tài liệu về MCP (Model Context Protocol) | Anthropic MCP |
| 17 | Tài liệu về đánh giá chất lượng RAG | RAGAS |

---

## I.4. Thuật ngữ quan trọng

Phần này giải thích các thuật ngữ kỹ thuật được sử dụng trong tài liệu, theo cách dễ hiểu nhất.

| STT | Thuật ngữ | Giải thích đơn giản |
|---|---|---|
| 1 | **AI (Trí tuệ nhân tạo)** | Khả năng máy tính "suy nghĩ", "hiểu" và "phản hồi" giống con người |
| 2 | **LLM (Mô hình ngôn ngữ lớn)** | "Bộ não" của AI — chương trình đã được dạy rất nhiều văn bản để có thể hiểu và sinh câu trả lời (vd: GPT-4o, Claude, Qwen2.5) |
| 3 | **RAG (Truy xuất tăng cường)** | Kỹ thuật cho AI tìm kiếm thông tin trong kho tri thức trước khi trả lời, giúp câu trả lời chính xác và có trích dẫn |
| 4 | **Embedding (Vector hóa)** | Biến văn bản thành dãy số đặc trưng để máy tính so sánh được "ý nghĩa" giữa các câu |
| 5 | **Vector Database (Cơ sở dữ liệu vector)** | Nơi lưu trữ các dãy số đặc trưng của văn bản để tìm kiếm nhanh theo ý nghĩa |
| 6 | **Chunk (Đoạn tri thức)** | Một đoạn văn bản ngắn (~1-2 trang A4) sau khi chia nhỏ từ văn bản dài để AI xử lý hiệu quả |
| 7 | **Ingestion Pipeline (Quy trình nhập tri thức)** | Quy trình tự động: lấy văn bản → làm sạch → chia nhỏ → vector hóa → lưu vào kho |
| 8 | **Prompt (Câu hỏi gửi AI)** | Câu hỏi hoặc chỉ dẫn gửi cho AI |
| 9 | **Prompt Template (Mẫu câu hỏi)** | Mẫu câu hỏi soạn sẵn, có phiên bản rõ ràng |
| 10 | **Agent (Trợ lý AI)** | AI có khả năng tự lập kế hoạch và thực hiện nhiều bước để hoàn thành yêu cầu phức tạp |
| 11 | **MCP (Giao thức ngữ cảnh mô hình)** | Chuẩn giao tiếp để trợ lý AI gọi các công cụ bên ngoài |
| 12 | **HITL (Người duyệt trong vòng lặp)** | Cơ chế yêu cầu con người duyệt trước khi AI thực hiện thao tác nhạy cảm (vd: gửi email, ký số) |
| 13 | **OCR (Nhận dạng ký tự quang học)** | Chuyển ảnh chụp hoặc PDF scan thành văn bản có thể chỉnh sửa |
| 14 | **Hallucination (Ảo giác)** | Hiện tượng AI "bịa" ra thông tin không có thật — lý do cần dùng RAG |
| 15 | **Multi-tenant (Đa đơn vị)** | Kiến trúc cho phép nhiều đơn vị (Tổng cục Thuế, Kho bạc, ...) dùng chung một hệ thống nhưng dữ liệu tách biệt hoàn toàn |
| 16 | **Ollama** | Phần mềm chạy mô hình AI mã nguồn mở ngay trong máy chủ của Bộ, không cần gửi dữ liệu lên cloud |
| 17 | **OpenAI / Anthropic / Azure OpenAI** | Các nhà cung cấp mô hình AI thương mại, chạy trên cloud (cần trả phí theo lượng sử dụng) |
| 18 | **pgvector** | Tiện ích mở rộng của cơ sở dữ liệu PostgreSQL để lưu trữ và tìm kiếm vector |
| 19 | **GPU (Bộ xử lý đồ họa)** | Loại chip chuyên dụng giúp tăng tốc các tác vụ AI lên gấp nhiều lần |
| 20 | **Token** | Đơn vị nhỏ nhất AI xử lý văn bản (khoảng 0,5-0,75 từ tiếng Việt) |
| 21 | **PII (Thông tin định danh cá nhân)** | Thông tin nhạy cảm như số CCCD, số điện thoại, email — AI tự động ẩn trước khi xử lý |
| 22 | **DMS** | Hệ thống quản lý văn bản hiện có của Bộ |
| 23 | **Audit Log (Nhật ký kiểm tra)** | Ghi lại mọi hoạt động AI để phục vụ kiểm tra, truy vết |
| 24 | **RBAC (Phân quyền theo vai trò)** | Mỗi người dùng có vai trò (Admin, Operator, User, Auditor) và chỉ được làm những việc thuộc vai trò của mình |
| 25 | **RLS (Bảo mật cấp dòng)** | Cơ chế PostgreSQL đảm bảo mỗi người chỉ thấy dữ liệu của đơn vị mình |
| 26 | **RAGAS** | Bộ công cụ đánh giá chất lượng hệ thống hỏi-đáp AI |
| 27 | **Zero-Trust (Không tin tưởng mặc định)** | Nguyên tắc bảo mật: mọi yêu cầu đều phải xác thực và kiểm tra phân quyền, không có ngoại lệ |

---

# II. MÔ HÌNH KIẾN TRÚC

## II.1. Bức tranh tổng thể

Phần này giúp người đọc hình dung AI được xây dựng, tích hợp và triển khai như thế nào trong toàn bộ hệ thống Văn phòng số của Bộ Tài Chính.

### II.1.0. Mô hình kiến trúc tổng thể (Sơ đồ tổng quan)

Để người đọc có cái nhìn toàn cảnh ngay từ đầu, dưới đây là **sơ đồ kiến trúc tổng thể** của module AI trong toàn hệ thống Văn phòng số Bộ Tài Chính:

```
╔══════════════════════════════════════════════════════════════════════════════════════════════╗
║                                                                                              ║
║                         PHẦN MỀM VĂN PHÒNG SỐ BỘ TÀI CHÍNH                                ║
║                                                                                              ║
║  ┌────────────────────────────────────────────────────────────────────────────────────────┐  ║
║  │  TẦNG 1 — TRÌNH BÀY (Người dùng nhìn thấy)                                             │  ║
║  │  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐    │  ║
║  │  │  Web Portal     │  │  Mobile App     │  │  ChatOps UI     │  │  Admin Console  │    │  ║
║  │  │  (Lãnh đạo,     │  │  (Chuyên viên,  │  │  (Hỏi-đáp AI    │  │  (Quản trị AI)  │    │  ║
║  │  │   văn thư)      │  │   văn thư)      │  │   bằng tiếng    │  │                 │    │  ║
║  │  │                 │  │                 │  │   Việt tự nhiên) │  │                 │    │  ║
║  │  └────────┬────────┘  └────────┬────────┘  └────────┬────────┘  └────────┬────────┘    │  ║
║  └───────────┼────────────────────┼────────────────────┼────────────────────┼────────────┘  ║
║              │                    │                    │                    │                ║
║  ┌───────────▼────────────────────▼────────────────────▼────────────────────▼────────────┐  ║
║  │  TẦNG 2 — API GATEWAY (Kong Gateway)                                                  │  ║
║  │  Xác thực JWT, giới hạn tốc độ, chống DDoS, định tuyến API                            │  ║
║  └───────────┬────────────────────────────────────────────────────────────────────────────┘  ║
║              │                                                                               ║
║  ┌───────────▼────────────────────────────────────────────────────────────────────────────┐  ║
║  │  TẦNG 3 — BFF (Backend For Frontend) — .NET 10                                          │  ║
║  │  Điều phối yêu cầu, gom dữ liệu từ nhiều microservice                                │  ║
║  └───────────┬────────────────────────────────────────────────────────────────────────────┘  ║
║              │                                                                               ║
║  ┌───────────▼────────────────────────────────────────────────────────────────────────────┐  ║
║  │  TẦNG 4 — MICROSERVICE NGHIỆP VỤ (Hiện hữu)                                          │  ║
║  │  ┌────────┐  ┌────────┐  ┌────────┐  ┌────────┐  ┌────────┐  ┌────────┐  ┌────────┐    │  ║
║  │  │DMS văn │  │Workflow│  │ Files  │  │Identity│  │  HRM   │  │Notifica│  │  ERP   │    │  ║
║  │  │  bản   │  │công việc│  │đính kèm│  │ user   │  │nhân sự │  │  -tion │  │        │    │  ║
║  │  └───┬────┘  └───┬────┘  └───┬────┘  └───┬────┘  └───┬────┘  └───┬────┘  └───┬────┘    │  ║
║  └──────┼───────────┼───────────┼───────────┼───────────┼───────────┼───────────┼────────┘  ║
║         │           │           │           │           │           │           │            ║
║  ┌──────▼───────────▼───────────▼───────────▼───────────▼───────────▼───────────▼────────┐  ║
║  │                                                                                      │  ║
║  │        ★ TẦNG 5 — MICROSERVICE AI (MỚI — MODULE AI CỦA ĐỀ ÁN) ★                    │  ║
║  │                                                                                      │  ║
║  │  ┌──────────────────────────────────────────────────────────────────────────────┐    │  ║
║  │  │                    ① AI Gateway (Cổng tiếp nhận duy nhất)                    │    │  ║
║  │  │   Xác thực · Giới hạn tốc độ · Chống injection · Che PII · Chọn provider    │    │  ║
║  │  └────────┬─────────────────────────────────┬───────────────────────────────────┘    │  ║
║  │           │                                 │                                        │  ║
║  │           │ đơn giản                       │ phức tạp đa bước                      │  ║
║  │           ▼                                 ▼                                        │  ║
║  │  ┌──────────────────────┐         ┌──────────────────────────┐                       │  ║
║  │  │ ② AI Engine Core     │         │ ③ Agent Orchestration    │                       │  ║
║  │  │ (Tóm tắt, hỏi-đáp,  │         │ (Trợ lý đa bước,         │                       │  ║
║  │  │  phân loại, trích    │         │  Human-in-the-Loop)       │                       │  ║
║  │  │  metadata)           │         │                          │                       │  ║
║  │  └─────────┬────────────┘         └──────────┬───────────────┘                       │  ║
║  │            │                                 │                                        │  ║
║  │            └────────────┬────────────────────┘                                        │  ║
║  │                         ▼                                                              │  ║
║  │            ┌────────────────────────────┐                                              │  ║
║  │            │ ⑥ Provider Abstraction     │                                              │  ║
║  │            │ (Đa nhà cung cấp LLM)     │                                              │  ║
║  │            └──────────┬─────────────────┘                                              │  ║
║  │                       ▼                                                                │  ║
║  │  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐                        │  ║
║  │  │  OpenAI Cloud   │  │  Anthropic      │  │  Ollama Local   │  ← Văn bản mật chỉ     │  ║
║  │  │  (gpt-4o-mini)  │  │  (Claude)       │  │  (qwen2.5:7b)   │    dùng Ollama nội bộ  │  ║
║  │  └─────────────────┘  └─────────────────┘  └─────────────────┘                        │  ║
║  │                                                                                      │  ║
║  │  ┌────────────────────────────┐  ┌────────────────────────────┐                      │  ║
║  │  │ ④ Knowledge Base & RAG     │  │ ⑤ OCR & Document Extract.  │                      │  ║
║  │  │ (Kho tri thức, tìm kiếm   │  │ (OCR tiếng Việt,           │                      │  ║
║  │  │  ngữ nghĩa, ingestion)    │  │  trích bảng biểu)          │                      │  ║
║  │  └────────┬───────────────────┘  └──────────┬─────────────────┘                      │  ║
║  │           │                                 │                                        │  ║
║  │           ▼                                 ▼                                        │  ║
║  │  ┌─────────────────────────────────────────────────────────────────┐                  │  ║
║  │  │  ⑦ AI Job & Task Queue (Kafka)                                  │                  │  ║
║  │  │  Hàng đợi tác vụ AI nặng (tóm tắt hàng loạt, OCR quy mô lớn) │                  │  ║
║  │  └─────────────────────────────────────────────────────────────────┘                  │  ║
║  │           │                                 │                                        │  ║
║  │           ▼                                 ▼                                        │  ║
║  │  ┌────────────────────────────┐  ┌────────────────────────────┐                      │  ║
║  │  │ ⑧ MCP Server               │  │ ⑨ Audit & Observability    │                      │  ║
║  │  │ (Kho công cụ cho Agent)    │  │ (Log, metric, trace)       │                      │  ║
║  │  └────────────────────────────┘  └────────────────────────────┘                      │  ║
║  │                                                                                      │  ║
║  │  ┌────────────────────────────┐  ┌────────────────────────────┐                      │  ║
║  │  │ ⑩ AI Admin Console         │  │ ⑪ Auth & Tenant            │                      │  ║
║  │  │ (Bảng điều khiển cho admin)│  │ (Phân quyền, RLS, JWT)     │                      │  ║
║  │  └────────────────────────────┘  └────────────────────────────┘                      │  ║
║  │                                                                                      │  ║
║  └──────────────────────────────────────────────────────────────────────────────────────┘  ║
║                                                                                              ║
║  ┌────────────────────────────────────────────────────────────────────────────────────────┐  ║
║  │  TẦNG 6 — SỰ KIỆN & BỘ ĐỆM                                                            │  ║
║  │  ┌─────────────────────────────────┐  ┌─────────────────────────────────────────┐    │  ║
║  │  │  Kafka Cluster                  │  │  Redis Cluster                         │    │  ║
║  │  │  (Sự kiện AI, Job Queue)        │  │  (Cache, semantic cache, rate-limit)   │    │  ║
║  │  └─────────────────────────────────┘  └─────────────────────────────────────────┘    │  ║
║  └────────────────────────────────────────────────────────────────────────────────────────┘  ║
║                                                                                              ║
║  ┌────────────────────────────────────────────────────────────────────────────────────────┐  ║
║  │  TẦNG 7 — DỮ LIỆU (được bảo vệ bởi DB Firewall)                                       │  ║
║  │  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────┐   │  ║
║  │  │ PostgreSQL       │  │ pgvector /       │  │  File Server     │  │ Knowledge    │   │  ║
║  │  │ (Nghiệp vụ +    │  │ Qdrant           │  │  (MinIO/S3)      │  │ Chunk store  │   │  ║
║  │  │  ai_platform)    │  │ (Vector tri thức)│  │  (PDF/DOCX scan)  │  │ (Redis)      │   │  ║
║  │  └──────────────────┘  └──────────────────┘  └──────────────────┘  └──────────────┘   │  ║
║  └────────────────────────────────────────────────────────────────────────────────────────┘  ║
║                                                                                              ║
║  ┌────────────────────────────────────────────────────────────────────────────────────────┐  ║
║  │  HẠ TẦNG VẬT LÝ (đặt dưới tầng 7)                                                     │  ║
║  │  ┌────────────────────────────────┐  ┌──────────────────────────────────────────┐    │  ║
║  │  │  K8s Cluster (CPU)            │  │  GPU Cluster (2-3 node A10 24GB)          │    │  ║
║  │  │  Chạy 11 microservice AI       │  │  Chạy Ollama LLM + OCR Worker nặng       │    │  ║
║  │  └────────────────────────────────┘  └──────────────────────────────────────────┘    │  ║
║  └────────────────────────────────────────────────────────────────────────────────────────┘  ║
║                                                                                              ║
╚══════════════════════════════════════════════════════════════════════════════════════════════╝
```

**Cách đọc sơ đồ**:
- **Từ trên xuống dưới**: luồng xử lý từ người dùng → AI → trả kết quả.
- **Tầng 1-2**: nơi người dùng giao tiếp với hệ thống.
- **Tầng 3-4**: các phòng ban nghiệp vụ hiện có.
- **Tầng 5** (khung viền đậm): **module AI** — tâm điểm của tài liệu này, gồm 11 phân hệ.
- **Tầng 6-7**: hạ tầng kỹ thuật.

### II.1.1. Mục tiêu của kiến trúc

Module AI hướng tới 7 mục tiêu lớn:

1. **Quản trị tập trung nhưng phân cấp cho từng đơn vị**: AI do Bộ quản lý tập trung, nhưng mỗi đơn vị (Tổng cục Thuế, Kho bạc, Hải quan, ...) có thể tự cấu hình kho tri thức và cách AI hoạt động trong phạm vi đơn vị mình.
2. **Tách biệt rõ ràng**: AI được chia thành các "phân hệ" nhỏ, mỗi phân hệ đảm nhận một nhóm chức năng, có thể phát triển và nâng cấp độc lập.
3. **Tích hợp mượt mà**: AI tích hợp chặt chẽ với các hệ thống hiện có (quản lý văn bản, chữ ký số, quản lý công việc, ...) chứ không hoạt động tách rời.
4. **Bảo mật cấp nhà nước**: tuân thủ nghiêm ngặt các quy định về bảo mật, đặc biệt với văn bản có độ mật cao (chỉ xử lý bằng AI chạy nội bộ).
5. **Sẵn sàng cao, chịu tải lớn**: hệ thống hoạt động ổn định ≥ 99,9% và phục vụ được khoảng 60.000 người dùng.
6. **Hỗ trợ nhiều nhà cung cấp AI**: không phụ thuộc vào một hãng duy nhất; có thể dùng đồng thời OpenAI, Anthropic, mô hình nội bộ.
7. **Mở rộng dễ dàng**: khi Bộ cần thêm tính năng AI mới hoặc thêm đơn vị sử dụng, hệ thống có thể mở rộng mà không cần "phá" lại từ đầu.

### II.1.2. Nguyên tắc thiết kế

Module AI được xây dựng theo **15 nguyên tắc** sau (diễn giải đơn giản):

| # | Nguyên tắc | Ý nghĩa dễ hiểu |
|---|---|---|
| 1 | **Kế thừa** | Tận dụng hạ tầng, dữ liệu, quy trình của Phần mềm Văn phòng số hiện có — không xây dựng song song |
| 2 | **Đơn trách nhiệm** | Mỗi phân hệ AI chỉ làm một nhóm việc rõ ràng, không "ôm đồm" |
| 3 | **Tách biệt** | Tách rời các thành phần (RAG, Agent, OCR, ...) để dễ nâng cấp từng phần |
| 4 | **Không tin tưởng mặc định** | Mọi yêu cầu đều phải xác thực và kiểm tra quyền, không có ngoại lệ |
| 5 | **Đa nhà cung cấp** | Không bị trói vào một hãng AI; có thể thay đổi nhà cung cấp linh hoạt |
| 6 | **Bảo mật dữ liệu cấp nhà nước** | Văn bản mật chỉ xử lý bằng AI nội bộ; không gửi lên cloud |
| 7 | **Quan sát được** | Mọi hoạt động AI đều có log, số liệu, truy vết để kiểm tra |
| 8 | **Khả năng phục hồi** | Khi một thành phần lỗi, hệ thống tự chuyển sang dự phòng, không đứng hẳn |
| 9 | **Mở rộng theo nhu cầu** | Thêm người dùng, thêm tính năng, thêm đơn vị đều có thể xử lý |
| 10 | **Tái sử dụng** | Các thành phần AI là "viên gạch" chung, dùng cho nhiều nghiệp vụ |
| 11 | **Đa đơn vị** | Nhiều đơn vị dùng chung hệ thống nhưng dữ liệu tách biệt hoàn toàn |
| 12 | **Cấu hình hóa** | Thay đổi prompt, thay đổi công cụ AI qua giao diện, không cần lập trình lại |
| 13 | **Phiên bản hóa** | Mọi thứ đều có phiên bản rõ ràng để dễ quay lại khi có sự cố |
| 14 | **Kiểm thử được** | Mỗi phân hệ đều có bộ kiểm thử riêng để đánh giá chất lượng |
| 15 | **Tiết kiệm chi phí** | Theo dõi chi phí sử dụng; cảnh báo khi vượt ngưỡng |

### II.1.3. Bức tranh 7 tầng của hệ thống

Toàn bộ Phần mềm Văn phòng số Bộ Tài Chính (bao gồm cả module AI) được tổ chức thành **7 tầng**, giống như 7 tầng của một tòa nhà:

| Tầng | Tên | Mô tả đời thường | Module AI đóng vai trò gì? |
|---|---|---|---|
| 1 | **Tầng trình bày** | Màn hình web, app di động mà người dùng nhìn thấy | Giao diện ChatOps, giao diện quản trị AI |
| 2 | **Tầng API Gateway** | "Cổng bảo vệ" kiểm tra mọi yêu cầu trước khi vào hệ thống | Tiếp nhận mọi yêu cầu AI, kiểm tra quyền |
| 3 | **Tầng ứng dụng BFF** | Tầng "phiên dịch" giữa giao diện và các dịch vụ bên dưới | Điều phối yêu cầu AI đến đúng phân hệ |
| 4 | **Tầng dịch vụ nghiệp vụ** | Các "phòng ban" xử lý văn bản, chữ ký số, quản lý công việc, ... | Cung cấp dữ liệu và nhận kết quả từ AI |
| 5 | **Tầng dịch vụ AI** | **Các "phòng ban AI"** gồm 11 phân hệ | **Đây chính là module AI** |
| 6 | **Tầng sự kiện & bộ đệm** | "Bưu điện" và "bộ nhớ đệm" của hệ thống | AI gửi/nhận sự kiện, cache kết quả |
| 7 | **Tầng dữ liệu** | "Kho lưu trữ" văn bản, hồ sơ, kho tri thức AI | Lưu trữ văn bản gốc, kho tri thức đã vector hóa |

**Điểm mấu chốt**: Module AI nằm ở **tầng 5**, đóng vai trò là "bộ não bổ sung" cho các phòng ban nghiệp vụ ở tầng 4. AI không thay thế các hệ thống hiện có, mà **làm cho chúng thông minh hơn**.

### II.1.4. Cấu trúc bên trong một phân hệ AI

Mỗi phân hệ AI được tổ chức theo mô hình **4 lớp** (Clean Architecture), giống như cấu trúc của một công ty:

| Lớp | Vai trò | Ví dụ trong phân hệ AI Gateway |
|---|---|---|
| **Domain (Lõi)** | Các "quy tắc nghiệp vụ" cốt lõi | Định nghĩa: AiRequest, RateLimitCounter, SemanticCacheEntry |
| **Application (Xử lý)** | Các "quy trình" xử lý yêu cầu | Handler: kiểm tra quyền → chống injection → chọn nhà cung cấp |
| **Infrastructure (Hạ tầng)** | Kết nối với thế giới bên ngoài | Kết nối Redis, PostgreSQL, OpenAI API |
| **API (Giao tiếp)** | Cửa ngõ tiếp nhận yêu cầu | REST API endpoint, JWT middleware |

Lợi ích: khi cần thay đổi cách lưu trữ (vd: từ Redis sang Memcached), chỉ cần sửa lớp Infrastructure, không ảnh hưởng đến quy tắc nghiệp vụ.

### II.1.5. Cách các phân hệ AI giao tiếp với nhau

Các phân hệ AI trao đổi với nhau qua **3 kênh**:

1. **REST API / gRPC** (giao tiếp trực tiếp): khi cần gọi và chờ kết quả ngay (vd: AI Gateway gọi AI Engine để lấy câu trả lời).
2. **Kafka** (giao tiếp bất đồng bộ): khi gửi tác vụ nặng cần xử lý nền (vd: gửi yêu cầu OCR cho 100 văn bản).
3. **PostgreSQL** (chia sẻ dữ liệu): khi cần trao đổi dữ liệu đã lưu (vd: AI Engine đọc kho tri thức do Knowledge Worker ghi).

### II.1.6. Tích hợp với các hệ thống hiện có của Bộ

Module AI kết nối với **các hệ thống đang vận hành** của Bộ theo bảng dưới:

| Hệ thống hiện có | AI kết nối để làm gì? |
|---|---|
| Hệ thống quản lý văn bản (DMS) | Đọc nội dung văn bản, trả về tóm tắt, gợi ý xử lý |
| Hệ thống quản lý công việc (Workflow) | Nhận thông báo khi có văn bản mới, gợi ý xử lý |
| Hệ thống quản lý tệp tin (Files) | Lấy file scan để OCR |
| Hệ thống chữ ký số | Hỗ trợ ký số thông minh (kiểm tra nội dung trước khi ký) |
| Hệ thống danh tính (Identity) | Xác thực người dùng, lấy thông tin vai trò |
| Hệ thống thông báo (Notification) | Gửi kết quả AI cho người dùng qua email/app |
| Hệ thống quản lý nhân sự (HRM) | Cung cấp thông tin về quy trình HRM cho ChatOps |
| Cổng dịch vụ công quốc gia | Liên thông văn bản với các cơ quan nhà nước khác |

AI cũng kết nối tới **các nhà cung cấp mô hình AI**:
- **OpenAI** (mô hình trên cloud, chất lượng cao, trả phí).
- **Anthropic Claude** (mô hình trên cloud, chất lượng cao, trả phí).
- **Azure OpenAI** (mô hình trên cloud của Microsoft, phù hợp cho doanh nghiệp lớn).
- **Ollama + mô hình mã nguồn mở** (chạy ngay trong máy chủ của Bộ, miễn phí, dùng cho văn bản mật).

### II.1.7. Dữ liệu AI được tổ chức ra sao?

Module AI sử dụng **cùng cơ sở dữ liệu PostgreSQL** của Phần mềm Văn phòng số hiện hữu, nhưng dành riêng một **"khu vực"** (gọi là schema `ai_platform`) để lưu các bảng dữ liệu AI. Cách tổ chức này giống như trong một tòa nhà lớn có nhiều phòng ban, ta dành riêng một tầng cho AI.

**Các bảng dữ liệu chính của AI**:

| Bảng | Lưu trữ cái gì? | Số dòng ước tính |
|---|---|---|
| `knowledge_source` | Danh sách nguồn tri thức (văn bản, file, web) | Vài trăm |
| `knowledge_chunk` | Đoạn văn bản đã "học" (dạng số đặc trưng) | Hàng chục triệu |
| `prompt_template` | Mẫu câu hỏi cho AI | Vài trăm |
| `agent_definition` | Định nghĩa các trợ lý AI | Vài chục |
| `mcp_tool` | Danh sách công cụ AI được phép dùng | Vài chục |
| `llm_provider` | Cấu hình nhà cung cấp mô hình AI | Vài chục |
| `ai_job` | Các tác vụ AI đang xử lý | Hàng triệu/năm |
| `ai_audit_log` | Nhật ký mọi hoạt động AI | Hàng triệu/năm |
| `ai_usage_log` | Theo dõi chi phí sử dụng AI | Hàng triệu/năm |

**Bảo mật dữ liệu AI**: mỗi bảng đều có cơ chế **Row-Level Security (RLS)** — nghĩa là người dùng của đơn vị nào chỉ thấy dữ liệu của đơn vị đó, không thể "nhìn trộm" sang đơn vị khác.

### II.1.8. Hạ tầng triển khai

Module AI chạy trên **hạ tầng Kubernetes hiện có** của Bộ, kết hợp bổ sung **cụm máy chủ GPU** chuyên dụng cho các tác vụ AI nặng.

**Hình dung đơn giản**: cũng như một bếp ăn lớn có nhiều khu vực (khu nấu chính, khu nấu phụ, khu chứa nguyên liệu), hạ tầng AI gồm:

| Khu vực | Chức năng |
|---|---|
| **Cụm máy chủ chính** (Kubernetes) | Chạy các phân hệ AI (Gateway, Engine, Knowledge Worker, Admin Console) |
| **Cụm máy chủ GPU** (2-3 node có card đồ họa chuyên dụng) | Chạy mô hình AI nội bộ (Ollama) và xử lý OCR văn bản scan |
| **Cụm lưu trữ** (PostgreSQL) | Lưu dữ liệu nghiệp vụ và dữ liệu AI |
| **Cụm sự kiện** (Kafka) | Xử lý các yêu cầu AI bất đồng bộ (tóm tắt văn bản lớn, OCR nhiều trang) |

**Ước tính chi phí hạ tầng cho module AI** (chưa tính chi phí phát triển): khoảng **700 triệu VNĐ** cho hạ tầng vật lý, bao gồm các máy chủ AI Gateway, AI Engine, Worker Pool và cụm GPU.

### II.1.9. Bảo mật

Module AI áp dụng mô hình **bảo mật nhiều lớp**, giống như một tòa nhà có nhiều lớp cửa bảo vệ:

| Lớp | Biện pháp | Giải thích đời thường |
|---|---|---|
| 1 | **Tường lửa biên** | "Hàng rào" ngoài cùng, chặn tin tặc |
| 2 | **API Gateway** | "Lễ tân" kiểm tra giấy tờ mọi người vào |
| 3 | **JWT + phân quyền** | "Thẻ nhân viên" gắn với từng vai trò (Admin, Operator, User) |
| 4 | **Mã hóa dữ liệu** | "Hộp kín" bảo vệ dữ liệu khi truyền và lưu trữ |
| 5 | **Phân loại độ mật** | Văn bản mật chỉ dùng AI nội bộ, không gửi cloud |
| 6 | **Phát hiện tấn công** | "Camera an ninh" tự động phát hiện hành vi bất thường |
| 7 | **Nhật ký kiểm tra** | "Sổ ghi chép" mọi hoạt động để truy vết khi cần |

**Các biện pháp bảo mật đặc thù cho AI**:

- **Chống prompt injection**: tự động phát hiện và chặn các câu hỏi cố gắng "lừa" AI làm việc sai.
- **Che giấu thông tin cá nhân**: tự động ẩn số CCCD, số điện thoại, email trước khi gửi cho AI.
- **Giới hạn sử dụng**: mỗi người dùng chỉ được hỏi AI tối đa 100 câu/phút, 5.000 câu/giờ.
- **Vòng xoay khóa API**: khóa kết nối tới OpenAI/Anthropic được thay đổi tự động mỗi tháng.
- **Hạn chế công cụ**: trợ lý AI chỉ được gọi những công cụ được phép, các thao tác nguy hiểm (xóa dữ liệu, ...) đều phải có người duyệt.

### II.1.10. Giám sát và vận hành

Module AI được giám sát liên tục qua **3 trụ cột**:

1. **Logs (Nhật ký)**: ghi lại mọi hoạt động, có thể tìm kiếm khi cần.
2. **Metrics (Chỉ số)**: thống kê thời gian thực (tốc độ, lỗi, chi phí).
3. **Traces (Truy vết)**: theo dõi đường đi của một yêu cầu qua nhiều phân hệ để dễ debug.

**Các chỉ số quan trọng cần theo dõi**:

| Chỉ số | Mục tiêu | Ý nghĩa |
|---|---|---|
| Thời gian phản hồi trung bình | ≤ 8 giây cho hỏi-đáp AI | Người dùng không phải chờ lâu |
| Tỷ lệ lỗi | < 1% | Hệ thống hoạt động ổn định |
| Lượng yêu cầu/ngày | ≥ 100.000 | Phục vụ được toàn Bộ |
| Chi phí/ngày | Theo ngân sách | Không vượt quá khả năng chi trả |
| Tỷ lệ cache hit | ≥ 30% | Tiết kiệm chi phí tái sử dụng kết quả |
| Điểm chất lượng RAG | ≥ 0.85 | Câu trả lời AI chính xác |

### II.1.11. Khả năng mở rộng

Module AI được thiết kế để **dễ dàng mở rộng** theo nhiều chiều:

| Chiều mở rộng | Cách thức |
|---|---|
| **Thêm người dùng** | Tăng số lượng máy chủ AI Gateway, AI Engine tự động theo tải |
| **Thêm tri thức** | Tăng dung lượng lưu trữ kho tri thức; có thể nén vector để tiết kiệm |
| **Thêm nhà cung cấp AI** | Chỉ cần cấu hình thêm trong Admin Console, không cần lập trình |
| **Thêm tính năng AI mới** | Thêm phân hệ AI mới (vd: chuyển giọng nói thành văn bản) dưới dạng viên gạch chung |
| **Thêm đơn vị sử dụng** | Bật/tắt theo đơn vị qua "cờ tính năng" (feature flag); thử nghiệm ở 1-2 đơn vị trước khi triển khai đại trà |

---

## II.2. Các chức năng AI

Phần này tổng hợp **20 chức năng AI** được phân rã vào **11 phân hệ** thành phần. Mỗi phân hệ đảm nhận một nhóm chức năng rõ ràng, được mô tả chi tiết trong phần II.5.

| STT | Chức năng AI | Phân hệ phụ trách | Mô tả ngắn |
|---|---|---|---|
| 1 | Tóm tắt văn bản | AI Engine Core | Tóm tắt văn bản dài thành 3-5 dòng |
| 2 | Chuẩn hóa văn phong | AI Engine Core | Sửa lỗi chính tả, chuẩn hóa câu chữ |
| 3 | Phân loại văn bản | AI Engine Core | Phân loại theo loại văn bản, lĩnh vực, độ mật |
| 4 | Trích xuất thông tin mẫu | AI Engine Core | Trích số văn bản, ngày tháng, người ký |
| 5 | Hỏi-đáp ngôn ngữ tự nhiên (ChatOps) | AI Gateway + AI Engine | Hỏi về văn bản bằng tiếng Việt tự nhiên |
| 6 | Tìm kiếm ngữ nghĩa | Knowledge Base | Tìm văn bản theo ý nghĩa |
| 7 | OCR văn bản scan | OCR Worker | Chuyển PDF scan, ảnh chụp thành text |
| 8 | Trích xuất bảng biểu | OCR Worker | Lấy bảng từ văn bản scan |
| 9 | Trợ lý AI đa bước (Agent) | Agent Orchestration | Giải quyết yêu cầu phức tạp, có người duyệt |
| 10 | Tạo báo cáo tự động | Agent + AI Engine | Tổng hợp dữ liệu thành báo cáo Excel/Word/PDF |
| 11 | NL2SQL (Hỏi SQL bằng tiếng Việt) | AI Engine | Sinh truy vấn SQL từ câu hỏi tiếng Việt |
| 12 | Gợi ý lãnh đạo phê duyệt | AI Engine | AI gợi ý lãnh đạo phù hợp |
| 13 | Gợi ý lịch họp | Agent + Tools | AI xếp lịch dựa trên lịch lãnh đạo |
| 14 | Phát hiện văn bản quá hạn | Agent + Tools | Phân tích hạn xử lý, cảnh báo sớm |
| 15 | Ký số có hỗ trợ AI | MCP Tool | AI xác nhận nội dung trước khi ký |
| 16 | Dịch đa ngôn ngữ | AI Engine | Anh ↔ Việt |
| 17 | Kết nối đa nhà cung cấp AI | Provider Abstraction | OpenAI / Ollama / Anthropic / Azure |
| 18 | Hàng đợi tác vụ | AI Job & Task Queue | Xử lý tác vụ AI nặng nề |
| 19 | Cung cấp công cụ cho Agent | MCP Server | Kho công cụ (12+ tools) |
| 20 | Quản trị hệ thống AI | Admin Console | Giao diện quản trị cho admin |

---

## II.3. Tích hợp với hệ thống hiện có

Module AI không hoạt động độc lập mà **liên kết chặt chẽ** với các hệ thống sẵn có của Bộ Tài Chính. Bảng dưới đây cho thấy AI "nói chuyện" với từng hệ thống như thế nào:

| Hệ thống Bộ | Cách AI kết nối | Dữ liệu trao đổi |
|---|---|---|
| **Hệ thống quản lý văn bản (DMS)** | Gọi REST API + nhận sự kiện qua Kafka | Văn bản, thông tin mẫu, file đính kèm |
| **Hệ thống quản lý công việc (Workflow)** | Nhận sự kiện qua Kafka | Sự kiện công việc (tạo mới, hoàn thành, quá hạn) |
| **Hệ thống quản lý tệp tin (Files)** | Gọi REST API + S3 | File PDF/DOCX/PNG scan |
| **Hệ thống danh tính (Identity)** | Gọi REST API + JWT | Thông tin người dùng, vai trò, đơn vị |
| **Hệ thống thông báo (Notification)** | Gọi REST API + Kafka | Gửi email, push notification |
| **Hệ thống quản lý nhân sự (HRM)** | Gọi REST API | Quy trình HRM, chức danh |
| **OpenAI Cloud** | Gọi HTTPS REST | Mô hình AI (trả phí) |
| **Anthropic Claude** | Gọi HTTPS REST | Mô hình AI (trả phí) |
| **Azure OpenAI** | Gọi HTTPS REST | Mô hình AI cho doanh nghiệp |
| **Ollama nội bộ** | Gọi HTTP REST | Mô hình AI chạy trong máy chủ Bộ |
| **Cổng dịch vụ công quốc gia** | Gọi HTTPS REST | Liên thông văn bản với cơ quan khác |

**Yêu cầu tích hợp**:
- **Không trùng lặp yêu cầu**: nếu gửi yêu cầu 2 lần, hệ thống chỉ xử lý 1 lần.
- **Chịu lỗi tạm thời**: nếu một hệ thống đang chậm, AI không bị "đứng hình".
- **Tự thử lại thông minh**: kết nối bị lỗi, AI tự thử lại sau vài giây.
- **Có phiên bản rõ ràng**: mỗi lần thay đổi giao tiếp đều có phiên bản để dễ quản lý.

---

## II.4. Triển khai và hạ tầng

Module AI được triển khai trên hạ tầng của Phần mềm Văn phòng số hiện hữu, kết hợp bổ sung cụm máy chủ GPU chuyên biệt. Hệ thống tuân theo kiến trúc **phân vùng mạng nhiều lớp** để bảo đảm an toàn thông tin.

### II.4.1. Vùng biên (Lớp bảo vệ ngoài cùng)

Là lớp bảo vệ đầu tiên trước các yêu cầu truy cập từ Internet/Intranet.

- **Cấu phần**: cụm tường lửa biên (firewall), có tính sẵn sàng cao (HA).
- **Chức năng chính**: chặn tấn công từ bên ngoài, chống DDoS, chỉ cho phép các giao thức hợp lệ (thường là HTTPS 443).

### II.4.2. Vùng Load Balancer (Cân bằng tải)

Sau tường lửa biên là các thiết bị cân bằng tải, xử lý yêu cầu đến AI Gateway.

- **Chức năng**: phân tải lưu lượng đến nhiều máy chủ AI Gateway, đảm bảo không có máy chủ nào bị quá tải.

### II.4.3. Vùng Proxy / API Gateway

- **Cấu phần**: NGINX (reverse proxy, mã hóa SSL) và Kong Gateway (định tuyến API, giới hạn tốc độ).
- **Chức năng**: AI Gateway là điểm vào duy nhất cho mọi yêu cầu AI.

### II.4.4. Vùng Firewall Core (Lớp bảo vệ trong)

Kiểm soát kết nối trước khi vào vùng ứng dụng AI nội bộ.

- **Chức năng**: phân tách rõ ràng giữa vùng Proxy và vùng ứng dụng; chỉ cho phép các luồng cần thiết; kết hợp IDS/IPS phát hiện hành vi bất thường.

### II.4.5. Vùng Ứng dụng & Xử lý (Application / Compute Zone)

Đây là **vùng lõi xử lý AI** của hệ thống, gồm 2 cụm chính:

#### Cụm Kubernetes (chạy các microservice nghiệp vụ và AI)
- Chạy các microservice của Phần mềm Văn phòng số hiện hữu (Identity, DMS, Workflow, Files, Notification, HRM).
- Chạy các microservice AI mới (AI Gateway, AI Engine Core, Agent Orchestration, AI Admin API, Audit Service).
- Triển khai theo mô hình đa node, có thể mở rộng theo chiều ngang.

#### Cụm GPU (chuyên biệt cho AI nặng)
- **Máy chủ Ollama LLM** (2-3 node có card GPU A10): chạy các mô hình AI mã nguồn mở (qwen2.5, llama3.2, ...).
- **Máy chủ OCR Worker**: chạy phần mềm nhận dạng văn bản scan tiếng Việt.
- **Máy chủ Knowledge Worker**: chạy quy trình nhập tri thức tự động.
- **Máy chủ Embedding Worker**: xử lý hàng loạt vector hóa văn bản.

### II.4.6. Vùng Sự kiện & Bộ đệm (Event & Caching)

- **Kafka Cluster**: hệ thống xử lý sự kiện cho các tác vụ AI bất đồng bộ.
- **Redis Cluster**: bộ nhớ đệm cho session, semantic cache, bộ đếm giới hạn tốc độ.

### II.4.7. Vùng Dữ liệu (Data Zone)

Được bảo vệ bởi **Database Firewall** (DBFW).

- **PostgreSQL Cluster (Patroni)**: lưu trữ toàn bộ dữ liệu nghiệp vụ và dữ liệu AI (schema `ai_platform`).
- **Vector Database (pgvector/Qdrant)**: lưu trữ vector đặc trưng của kho tri thức.
- **File Server (MinIO/S3)**: lưu trữ file PDF/DOCX scan gốc.

---

### II.4.8. Định lượng tài nguyên hạ tầng (Capacity Planning)

Phần này trả lời câu hỏi: **"Cần bao nhiêu máy chủ, bao nhiêu RAM, bao nhiêu GPU để đáp ứng 60.000 người dùng và 100.000 yêu cầu AI mỗi ngày?"** — đây là phần dành cho cán bộ IT, Quản lý hạ tầng và Lãnh đạo phê duyệt ngân sách.

#### II.4.8.1. Cơ sở tính toán (đầu vào định lượng)

| Tham số | Giá trị | Nguồn |
|---|---|---|
| Tổng số tài khoản | 60.000 user | Toàn ngành Tài chính |
| User hoạt động hàng ngày (DAU) | 15.000 user (25%) | Phổ biến trong dịch vụ công |
| Giờ cao điểm | 8h-11h và 14h-16h (5 giờ/ngày) | Giờ hành chính |
| Tổng yêu cầu AI/ngày | 100.000 yêu cầu | Mục tiêu nghiệp vụ |
| Yêu cầu cao điểm/giây (peak) | **~12 yêu cầu/giây** | Tính: 100.000 ÷ 8h ÷ 3.600s × hệ số cao điểm ×2 = ~7; có Agent/OCR nặng → 12/s |
| Thời gian phản hồi mục tiêu | ≤ 8 giây cho hỏi-đáp | SLA |
| Thời gian hoạt động (uptime) | ≥ 99,9% | SLA |
| Tỷ lệ cache hit | 30% | Mục tiêu |
| Tỷ lệ OCR / tác vụ nặng | 10% yêu cầu | Nghiệp vụ |
| Tỷ lệ yêu cầu chạy nội bộ (Ollama) | 40% (văn bản mật + ngân sách tiết kiệm) | Chính sách bảo mật |
| Tỷ lệ yêu cầu chạy cloud (OpenAI) | 60% | Còn lại |
| Kích thước kho tri thức | ~10 triệu đoạn (~2,5 triệu văn bản × 4 đoạn) | Quy mô Bộ |
| Dung lượng mỗi vector | 1536 chiều × 4 byte = **6 KB** | BGE m3 / OpenAI |
| Tổng dung lượng vector | 10 triệu × 6KB = **~60 GB** (chưa nén); nén PQ = ~15 GB | Tính toán |
| Tốc độ tăng trưởng kho tri thức | +5%/tháng | Quy mô Bộ |

#### II.4.8.2. Sơ đồ dung lượng hạ tầng

```
┌──────────────────────────────────────────────────────────────────────────────┐
│                     TỔNG QUAN HẠ TẦNG MODULE AI                              │
│                                                                              │
│   60.000 users ─┐                                                           │
│                 ▼                                                            │
│        ┌───────────────────┐         ┌───────────────────────────────┐       │
│        │ API Gateway Pool  │  ───►   │  Microservice Pool (CPU)      │       │
│        │ 3 node × 16 vCPU  │         │  12 node × 32 vCPU × 64 GB    │       │
│        │ 3 node × 32 GB    │         │  AI Gateway + Engine + Agent  │       │
│        └───────────────────┘         │  + Auth + Audit + Admin       │       │
│                                       └────────┬──────────────────────┘       │
│                                                │                              │
│         ┌──────────────────────────────────────┼──────────────────────┐       │
│         │                                      │                      │       │
│         ▼                                      ▼                      ▼       │
│  ┌─────────────────┐              ┌──────────────────────┐    ┌────────────┐  │
│  │ Vector DB       │              │  GPU Cluster         │    │ Embed/OCR  │  │
│  │ 3 node pgvector │              │  3 node x A10 24GB   │    │ 3 node CPU │  │
│  │ 1 TB SSD / node │              │  chạy qwen2.5:14b    │    │ 16 vCPU    │  │
│  └─────────────────┘              └──────────────────────┘    └────────────┘  │
│                                                                              │
│  ┌─────────────────┐    ┌─────────────────┐    ┌────────────────────────┐   │
│  │ PostgreSQL HA   │    │ Kafka Cluster   │    │ Redis Cluster          │   │
│  │ Patroni 1+1+1   │    │ 3 broker × 1 TB │    │ 3 node × 32 GB RAM     │   │
│  │ chứa ai_platform│    │ events + jobs   │    │ cache + rate-limit     │   │
│  └─────────────────┘    └─────────────────┘    └────────────────────────┘   │
│                                                                              │
│  ┌─────────────────┐    ┌─────────────────┐    ┌────────────────────────┐   │
│  │ MinIO Object    │    │ Load Balancer   │    │ Firewall pair (HA)     │   │
│  │ 4 node × 4 TB   │    │ F5 / HAProxy    │    │ Biên + Core (2 cặp)    │   │
│  │ PDF/DOCX scan   │    │ SSL termination │    │ IPS/IDS + WAF           │   │
│  └─────────────────┘    └─────────────────┘    └────────────────────────┘   │
│                                                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```

#### II.4.8.3. Định lượng từng thành phần

##### A. Cụm Kubernetes (CPU) — xử lý microservice nghiệp vụ và AI

| Thành phần | Số node | Cấu hình / node | Tổng | Lý do |
|---|---|---|---|---|
| **Control Plane (Master)** | 3 node | 8 vCPU, 16 GB RAM, 100 GB SSD | 24 vCPU, 48 GB | HA cho K8s |
| **AI Gateway** | 3 node | 8 vCPU, 16 GB | 24 vCPU, 48 GB | Stateless, chịu tải gateway |
| **AI Engine Core** | 4 node | 16 vCPU, 32 GB | 64 vCPU, 128 GB | Xử lý RAG + LLM streaming |
| **Agent Orchestration** | 3 node | 16 vCPU, 32 GB | 48 vCPU, 96 GB | Multi-step Agent |
| **Knowledge Worker / Embedding Worker** | 3 node | 16 vCPU, 32 GB | 48 vCPU, 96 GB | Ingestion + vector hóa |
| **OCR Worker** | 2 node | 16 vCPU, 32 GB + GPU T4 | 32 vCPU, 64 GB | OCR văn bản scan |
| **Audit Service** | 2 node | 8 vCPU, 16 GB | 16 vCPU, 32 GB | Log + metric |
| **Admin API** | 2 node | 8 vCPU, 16 GB | 16 vCPU, 32 GB | UI Backend |
| **AI Admin Console (Web)** | 2 node | 4 vCPU, 8 GB | 8 vCPU, 16 GB | SPA React |
| **Buffer (dự phòng + mở rộng)** | 3 node | 16 vCPU, 32 GB | 48 vCPU, 96 GB | Scale-out theo tải |

**Tổng cụm K8s CPU**: **~26 node, ~340 vCPU, ~656 GB RAM**, dung lượng lưu trữ khoảng **~10 TB SSD** (cho container images, logs ngắn hạn).

##### B. Cụm GPU — xử lý mô hình AI nội bộ

| Thành phần | Số node GPU | Cấu hình / node | Mô hình AI chạy | Lý do |
|---|---|---|---|---|
| **Ollama LLM Server** | 3 node | 1 × GPU NVIDIA A10 24 GB, 16 vCPU, 64 GB RAM | qwen2.5:14b-instruct-q4_K_M | Chạy mô hình cho văn bản mật |
| **OCR Engine** | 2 node | 1 × GPU NVIDIA T4 16 GB, 8 vCPU, 32 GB | PaddleOCR-VL + VietOCR | Nhận dạng tiếng Việt |
| **Embedding Server** | 2 node | 1 × GPU NVIDIA T4 16 GB, 8 vCPU, 32 GB | BGE-M3 (multilingual) | Vector hóa hàng loạt |

**Tổng cụm GPU**: **~7 node GPU**, trong đó **3 GPU A10 24 GB** + **4 GPU T4 16 GB**.

**Tại sao cần GPU A10?** Mô hình `qwen2.5:14b-instruct-q4_K_M` (~9 GB VRAM) yêu cầu GPU riêng để có thời gian phản hồi < 8 giây. Chạy trên CPU sẽ mất 30-60 giây/câu, không đáp ứng SLA.

**Công suất xử lý của cluster Ollama**:
- Mỗi node A10 chạy được **~10-15 yêu cầu/giây** với qwen2.5:14b.
- 3 node = **~30-45 yêu cầu/giây**, vượt yêu cầu 12 yêu cầu/giây (40% dành cho nội bộ = ~5 yêu cầu/giây).

##### C. Cụm cơ sở dữ liệu

| Thành phần | Số node | Cấu hình / node | Tổng | Lưu trữ | Lý do |
|---|---|---|---|---|---|
| **PostgreSQL (HA)** | 3 node (Patroni 1 Primary + 2 Replica) | 16 vCPU, 64 GB RAM | 48 vCPU, 192 GB | 2 TB NVMe SSD / node | Lưu dữ liệu nghiệp vụ + schema `ai_platform` |
| **Vector DB (pgvector)** | 3 node | 16 vCPU, 64 GB RAM | 48 vCPU, 192 GB | 1 TB NVMe SSD / node | Lưu ~60 GB vector (10 triệu đoạn); index HNSW |
| **Redis Cluster** | 3 node (cluster mode) | 8 vCPU, 32 GB RAM | 24 vCPU, 96 GB | 256 GB SSD | Semantic cache + rate-limit + session |
| **Kafka Cluster** | 3 broker | 8 vCPU, 32 GB RAM | 24 vCPU, 96 GB | 1 TB SSD / broker | Sự kiện + Job Queue |
| **MinIO (Object Storage)** | 4 node | 8 vCPU, 16 GB RAM | 32 vCPU, 64 GB | 4 TB HDD / node (16 TB tổng) | Lưu file PDF/DOCX scan gốc |

**Tổng DB/Storage**: **~16 node, ~176 vCPU, ~640 GB RAM, ~22 TB lưu trữ**.

##### D. Thiết bị mạng & bảo mật

| Thiết bị | Số lượng | Cấu hình | Lý do |
|---|---|---|---|
| **Tường lửa biên (Firewall)** | 2 (HA pair) | Thiết bị chuyên dụng Fortinet/FortiGate 100F hoặc Palo Alto PA-3220 | Chặn tấn công, chống DDoS |
| **Load Balancer** | 2 (HA pair) | F5 BIG-IP / HAProxy / NGINX Plus | Phân tải SSL, định tuyến |
| **WAF (Web App Firewall)** | 1 cụm | ModSecurity / Cloudflare | Chống SQL injection, XSS |
| **Database Firewall (DBFW)** | 1 cụm | Imperva / FortiDB | Bảo vệ PostgreSQL |
| **Switch lớp 2/3 (Core Switch)** | 2 (stack) | 48 port 10 Gbps | Kết nối giữa các vùng |
| **Switch lớp 2 (Access Switch)** | 4-6 switch | 48 port 1 Gbps | Kết nối rack |

##### E. Tổng hợp toàn bộ hạ tầng

| Loại | Số node/thiết bị | Tổng vCPU | Tổng RAM | Tổng lưu trữ |
|---|---|---|---|---|
| K8s Control Plane | 3 node | 24 | 48 GB | 300 GB |
| K8s Worker (microservice AI) | ~26 node | ~340 | ~656 GB | ~10 TB |
| GPU Cluster | 7 node | ~80 | ~336 GB | ~7 TB |
| PostgreSQL + Vector DB | 6 node | ~96 | ~384 GB | ~12 TB |
| Redis + Kafka + MinIO | 10 node | ~80 | ~256 GB | ~18 TB |
| Network/Security | 8 thiết bị | – | – | – |
| **TỔNG** | **~60 server/thiết bị** | **~620 vCPU** | **~1,7 TB RAM** | **~47 TB lưu trữ** |

#### II.4.8.4. Định lượng băng thông mạng

| Loại traffic | Lưu lượng ước tính | Giải thích |
|---|---|---|
| **Traffic API AI Gateway** (đỉnh) | ~50 Mbps | 12 req/s × ~500 KB/request (streaming response) |
| **Traffic embedding ingestion** | ~200 Mbps (off-peak) | Vector hóa 100 văn bản/giờ đêm |
| **Traffic tới OpenAI Cloud** | ~100 Mbps | 60% yêu cầu chạy cloud |
| **Backup dữ liệu** | ~500 Mbps | Tối đa 2 giờ/ngày |
| **Yêu cầu mạng** | ≥ 10 Gbps uplink giữa các vùng; ≥ 1 Gbps mỗi rack | Đảm bảo không nghẽn |

#### II.4.8.5. Chi phí hạ tầng ước tính

Phần này giúp Lãnh đạo phê duyệt ngân sách:

| Hạng mục | Số lượng | Đơn giá (VNĐ) | Thành tiền (VNĐ) |
|---|---|---|---|
| Server CPU (Dell PowerEdge R750) | ~40 | 250.000.000 | 10.000.000.000 |
| Server GPU A10 (Dell R750xa + GPU) | 3 | 800.000.000 | 2.400.000.000 |
| Server GPU T4 (Dell R750 + GPU) | 4 | 400.000.000 | 1.600.000.000 |
| SSD NVMe (7,68 TB) | ~30 | 50.000.000 | 1.500.000.000 |
| Firewall HA pair | 1 | 600.000.000 | 600.000.000 |
| Load Balancer HA pair | 1 | 500.000.000 | 500.000.000 |
| Switch Core 10G | 2 | 200.000.000 | 400.000.000 |
| Switch Access | 6 | 50.000.000 | 300.000.000 |
| UPS + tủ rack + cáp quang | 1 lô | – | 800.000.000 |
| **Tổng hạ tầng vật lý (CAPEX)** | | | **~18,1 tỷ VNĐ** |
| **Chi phí vận hành / năm (OPEX)** | | | **~3,5 tỷ VNĐ** |
| Điện năng (1 MW liên tục × 2.500đ/kWh × 8.760h) | – | – | ~22 tỷ/năm (đã gộp ở trên) |
| Bảo trì, nhân sự vận hành | – | – | ~1,5 tỷ/năm |

> **Ghi chú**: Số liệu trên là **ước tính định lượng**, cần khảo sát giá thị trường Việt Nam và chào giá từ 3-5 nhà cung cấp trước khi phê duyệt ngân sách chính thức.

#### II.4.8.6. Chi phí sử dụng AI hàng năm (OPEX)

Chi phí sử dụng mô hình AI trên cloud (OpenAI/Anthropic):

| Tham số | Giá trị |
|---|---|
| Trung bình token / yêu cầu (input) | 2.000 token |
| Trung bình token / yêu cầu (output) | 500 token |
| Tỷ lệ dùng cloud | 60% yêu cầu |
| Yêu cầu / ngày | 100.000 |
| Token input / ngày (cho cloud) | 100.000 × 60% × 2.000 = **120 triệu token input/ngày** |
| Token output / ngày (cho cloud) | 100.000 × 60% × 500 = **30 triệu token output/ngày** |
| Token input / năm (cho cloud) | ~36 tỷ token |
| Token output / năm (cho cloud) | ~9 tỷ token |

| Nhà cung cấp | Input (USD/1M token) | Output (USD/1M token) | Chi phí / năm (USD) | Chi phí / năm (VNĐ) |
|---|---|---|---|---|
| GPT-4o-mini | 0,15 | 0,60 | 5.400 + 5.400 = **~10.800** | ~270 triệu |
| GPT-4o | 2,50 | 10,00 | 90.000 + 90.000 = **~180.000** | ~4,5 tỷ |
| Claude Sonnet 3.5 | 3,00 | 15,00 | 108.000 + 135.000 = **~243.000** | ~6 tỷ |
| Ollama nội bộ (miễn phí, chỉ tốn điện) | 0 | 0 | 0 | ~300 triệu (điện cho GPU) |

> **Khuyến nghị**: Dùng **GPT-4o-mini** làm mặc định cho cloud (chất lượng tốt, chi phí thấp), dùng **Ollama nội bộ** cho văn bản mật.
> **Tổng OPEX dự kiến cho AI cloud / năm**: **~300 - 500 triệu VNĐ**.

#### II.4.8.7. Cách tính công suất hệ thống (cuốn chiếu cho lãnh đạo)

Để Lãnh đạo hiểu được khả năng đáp ứng, sau đây là cách tính ngược:

| Mục tiêu | Giá trị |
|---|---|
| **Số user hoạt động đồng thời (peak)** | 15.000 × 25% = ~3.750 user |
| **Yêu cầu trung bình / user / giờ** | 100.000 yêu cầu / 15.000 DAU / 8 giờ = 0,8 yêu cầu |
| **Yêu cầu đồng thời (peak)** | 3.750 × 0,8 = **~3.000 yêu cầu đồng thời** |
| **Yêu cầu / giây (peak)** | ~12 yêu cầu/giây |
| **Yêu cầu cache hit (30%)** | Còn ~8 yêu cầu thực sự đi tới LLM/giây |
| **Yêu cầu LLM cloud (60%)** | ~5 yêu cầu/giây đi tới OpenAI |
| **Yêu cầu LLM nội bộ (40%)** | ~3 yêu cầu/giây đi tới Ollama |

| Khả năng phục vụ của cụm Ollama (3 node A10) | ~30-45 yêu cầu/giây (gấp 10 lần peak) |
| **Khả năng phục vụ của OpenAI cloud** | Hầu như không giới hạn (khả năng mở rộng tự động) |

**Kết luận**: Hạ tầng được đề xuất có **khả năng dư thừa gấp 10 lần** so với yêu cầu peak, đảm bảo:
- ✅ Phục vụ 60.000 user và 100.000 yêu cầu/ngày.
- ✅ Thời gian phản hồi trung bình ≤ 8 giây với cache hit 30%.
- ✅ SLA 99,9% uptime.
- ✅ Có dư thừa để tăng trưởng 5-10 lần trong tương lai.

#### II.4.8.8. Kế hoạch mở rộng (Scale-out)

Khi hệ thống tăng trưởng, có thể mở rộng từng thành phần mà **không cần thay đổi kiến trúc**:

| Tình huống | Giải pháp mở rộng |
|---|---|
| **DAU tăng gấp đôi** → 30.000 user | Tăng số node Worker (K8s) từ 26 → 50; thêm 2 node Redis |
| **Yêu cầu tăng gấp 3** → 300.000 yêu cầu/ngày | Tăng node AI Gateway, AI Engine; thêm 2 node GPU A10 |
| **Kho tri thức tăng gấp đôi** → 20 triệu đoạn | Tăng node Vector DB từ 3 → 6; tăng dung lượng SSD |
| **Thêm đơn vị dùng** | Bật tenant mới trên hạ tầng hiện có (multi-tenant) — không cần thêm server |
| **Thêm nhà cung cấp AI** | Cấu hình trong Admin Console — không cần thêm server |
| **Nâng cấp mô hình AI** (vd: từ qwen2.5:14b lên qwen2.5:32b) | Cần GPU A100 40GB hoặc H100 thay cho A10; thêm 1-2 node |

#### II.4.8.9. Yêu cầu môi trường vật lý (phòng máy chủ)

| Yêu cầu | Giá trị |
|---|---|
| Diện tích phòng máy | ~40-50 m² (cho 6-8 rack 42U) |
| Công suất điện | ≥ 30 kW (IT load) + 30 kW (làm mát) = **~60 kW tổng** |
| Làm mát | In-row cooling hoặc hot/cold aisle, duy trì 22-24°C |
| UPS | ≥ 100 kVA, dự phòng 30 phút |
| Máy phát điện dự phòng | ≥ 100 kVA, tự động chuyển mạch ≤ 30 giây |
| Phát hiện/chữa cháy | FM-200 hoặc Novec 1230 |
| Giám sát môi trường | Cảm biến nhiệt độ, độ ẩm, rò rỉ nước |
| Kết nối Internet | ≥ 1 Gbps leased line, dự phòng ≥ 200 Mbps 4G/5G |

#### II.4.8.10. Tổng ngân sách ước tính

| Hạng mục | Số tiền (VNĐ) |
|---|---|
| **CAPEX — Hạ tầng vật lý (mua server, switch, firewall)** | **~18,1 tỷ** |
| CAPEX — Phần mềm (license Windows Server, SQL Server nếu dùng, ...) | ~1,5 tỷ |
| **OPEX / năm — Điện, mát, bảo trì, nhân sự** | **~3,5 tỷ** |
| **OPEX / năm — AI cloud (GPT-4o-mini + buffer)** | **~300-500 triệu** |
| OPEX / năm — Internet leased line | ~200 triệu |
| **TỔNG 5 NĂM (CAPEX + 5×OPEX)** | **~38 - 40 tỷ VNĐ** |

> **Ghi chú quan trọng**: Đây là **ước tính định lượng cho module AI**. Chi phí của toàn bộ Phần mềm Văn phòng số Bộ Tài Chính sẽ lớn hơn nhiều, bao gồm cả chi phí phát triển (CAPEX phần mềm), đào tạo, và các hạng mục khác.

---

# THIẾT KẾ CHI TIẾT CHỨC NĂNG

## II.5. Hệ thống AI

Module AI gồm **11 phân hệ**, mỗi phân hệ đảm nhận một nhóm chức năng rõ ràng và được mô tả theo cấu trúc **7 mục đồng nhất**:

- Mục 1: Mô tả chức năng — phân hệ này làm gì
- Mục 2: Luồng xử lý — quy trình xử lý ra sao
- Mục 3: Tài liệu liên quan — tham khảo thêm ở đâu
- Mục 4: Tên module — gồm những thành phần nào
- Mục 5: Thiết kế giao diện — màn hình quản trị thế nào
- Mục 6: Thành phần giao diện — các nút bấm, ô nhập liệu
- Mục 7: Thiết kế hàm/thủ tục — mô tả hàm xử lý (bằng ngôn ngữ tự nhiên)

**11 phân hệ của module AI:**

| STT | Tên phân hệ | Vai trò dễ hiểu |
|---|---|---|
| 1 | **AI Gateway** | Cổng tiếp nhận, kiểm tra quyền, chống lạm dụng |
| 2 | **AI Engine Core** | Bộ máy xử lý AI trung tâm (tóm tắt, hỏi-đáp, phân loại) |
| 3 | **Agent Orchestration** | Trợ lý AI đa bước (lập kế hoạch, gọi công cụ) |
| 4 | **Knowledge Base & RAG** | Kho tri thức (lưu trữ, tìm kiếm ngữ nghĩa) |
| 5 | **OCR & Document Extraction** | Nhận dạng văn bản scan, trích bảng biểu |
| 6 | **Provider Abstraction** | Kết nối tới nhiều nhà cung cấp AI khác nhau |
| 7 | **AI Job & Task Queue** | Hàng đợi xử lý tác vụ AI nặng |
| 8 | **MCP Server** | Kho công cụ cho trợ lý AI |
| 9 | **Audit & Observability** | Giám sát, truy vết, theo dõi chi phí |
| 10 | **AI Admin Console** | Bảng điều khiển cho admin |
| 11 | **Auth & Tenant** | Phân quyền và bảo mật dữ liệu |

### Sơ đồ quan hệ giữa 11 phân hệ AI

```
                          ┌──────────────────────────────────┐
                          │  ① AI GATEWAY (cổng duy nhất)   │
                          │  Xác thực · Giới hạn · Cache     │
                          └──────┬───────────────┬───────────┘
                                 │               │
                          ┌──────▼──────┐  ┌─────▼────────────┐
                          │ Yêu cầu     │  │ Yêu cầu          │
                          │ đơn giản    │  │ phức tạp đa bước │
                          └──────┬──────┘  └─────┬────────────┘
                                 │               │
                          ┌──────▼───────────────▼───────────┐
                          │  ② AI ENGINE CORE                  │
                          │  (Tóm tắt · Hỏi-đáp · Phân loại) │
                          └──────┬────────────────────────────┘
                                 │ gọi LLM
                          ┌──────▼────────────────────────────┐
                          │  ⑥ PROVIDER ABSTRACTION (LLM)     │
                          │  OpenAI · Anthropic · Ollama       │
                          └───────────────────────────────────┘

       ┌───────────────────────────────────────────────────────────┐
       │  ③ AGENT ORCHESTRATION (Trợ lý đa bước)                 │
       │  Lập kế hoạch → Gọi công cụ → Nhờ người duyệt          │
       └───┬─────────────────┬─────────────────────────┬─────────┘
           │                 │                         │
           ▼                 ▼                         ▼
   ┌──────────────┐  ┌────────────────┐  ┌──────────────────────┐
   │ ② AI Engine  │  │ ⑧ MCP SERVER   │  │ ④ KNOWLEDGE BASE     │
   │   Core       │  │ (Kho công cụ)  │  │ (Tìm ngữ nghĩa)     │
   └──────────────┘  └────────┬───────┘  └──────────┬───────────┘
                              │                      │
                              │ search · get ·       │ retrieve ·
                              │ summarize · sign     │ embed · RAG
                              │                      │
                              ▼                      ▼
   ┌──────────────────────────────────────────────────────────────┐
   │   ④ KNOWLEDGE BASE & RAG  ←→  ⑤ OCR & DOC EXTRACTION         │
   │   (Kho tri thức, vector DB)    (Nhận dạng văn bản scan)      │
   └──────────────────────────────────────────────────────────────┘
                              │
                              ▼ nhập tri thức + OCR nặng
   ┌──────────────────────────────────────────────────────────────┐
   │  ⑦ AI JOB & TASK QUEUE (Kafka)                              │
   │  Hàng đợi xử lý tác vụ nặng song song                       │
   └──────────────────────────────────────────────────────────────┘

   ╔══════════════════════════════════════════════════════════════╗
   ║  LỚP NỀN TẢNG (chạy ngầm, phục vụ cho 11 phân hệ trên)    ║
   ║                                                              ║
   ║  ┌────────────────────────┐  ┌─────────────────────────────┐ ║
   ║  │ ⑪ AUTH & TENANT        │  │  ⑨ AUDIT & OBSERVABILITY    │ ║
   ║  │ Phân quyền · RLS · JWT │  │  Log · Metric · Trace       │ ║
   ║  └────────────────────────┘  └─────────────────────────────┘ ║
   ║                                                              ║
   ║  ┌─────────────────────────────────────────────────────────┐ ║
   ║  │  ⑩ AI ADMIN CONSOLE (UI cho admin)                      │ ║
   ║  │  Quản lý 11 phân hệ phía trên qua giao diện web        │ ║
   ║  └─────────────────────────────────────────────────────────┘ ║
   ╚══════════════════════════════════════════════════════════════╝
```

**Cách đọc sơ đồ 11 phân hệ**:
- **Phía trên**: 3 phân hệ trực tiếp xử lý yêu cầu AI (Gateway → Engine → Provider).
- **Ở giữa**: 3 phân hệ bổ trợ cho yêu cầu phức tạp (Agent, MCP, Knowledge).
- **Bên dưới**: OCR và Job Queue xử lý tác vụ nặng.
- **Lớp nền tảng**: 3 phân hệ chạy ngầm (Auth, Audit, Admin Console) phục vụ cho tất cả.

### Sơ đồ luồng 1 yêu cầu AI đi qua hệ thống

```
 Người dùng                Tầng 1-2              Tầng 5 (Module AI)                    Tầng 6-7
 (Web/Mobile)          (Kong Gateway)                                                   (Lưu trữ)
      │                      │                          │                                  │
      │  1. Đặt câu hỏi      │                          │                                  │
      ├─────────────────────►│                          │                                  │
      │                      │  2. Xác thực JWT,        │                                  │
      │                      │     kiểm tra quyền       │                                  │
      │                      ├─────────────────────────►│                                  │
      │                      │                          │  3. AI Gateway: chống injection,  │
      │                      │                          │     che PII, kiểm tra cache       │
      │                      │                          │                                  │
      │                      │                          │  4. Gọi AI Engine (đơn giản)     │
      │                      │                          │     HOẶC Agent (phức tạp)        │
      │                      │                          │                                  │
      │                      │                          │  5. AI Engine: tìm kiếm TỪ KHÓA  │
      │                      │                          │     + NGỮ NGHĨA qua Knowledge    │
      │                      │                          ├─────────────────────────────────►│
      │                      │                          │  ← 6. Trả về 8 văn bản liên quan │
      │                      │                          │◄─────────────────────────────────┤
      │                      │                          │                                  │
      │                      │                          │  7. Xếp hạng lại, tổng hợp       │
      │                      │                          │     ngữ cảnh, gọi LLM             │
      │                      │                          │                                  │
      │                      │                          │  8. Nhận câu trả lời streaming   │
      │                      │                          │                                  │
      │                      │                          │  9. Kiểm tra chất lượng, trích dẫn│
      │                      │                          │                                  │
      │                      │                          │  10. Ghi log audit + cập nhật    │
      │                      │                          │      chi phí + cache kết quả      │
      │                      │                          ├─────────────────────────────────►│
      │                      │                          │                                  │
      │  11. Trả câu trả lời  │                          │                                  │
      │   kèm trích dẫn      │                          │                                  │
      │◄─────────────────────┼──────────────────────────┤                                  │
      │                      │                          │                                  │
```

**Cách đọc sơ đồ luồng**:
- **11 bước** từ lúc user nhấn "Gửi" đến lúc nhận câu trả lời.
- Mỗi bước làm rõ **phân hệ nào chịu trách nhiệm** và **dữ liệu đi đâu**.

### Sơ đồ hạ tầng triển khai (vùng mạng)

```
                  ┌─────────────────────────────────────────────┐
                  │         INTERNET / INTRANET Bộ TC            │
                  └────────────────────┬────────────────────────┘
                                       │
            ╔══════════════════════════▼══════════════════════════╗
            ║  VÙNG BIÊN                                         ║
            ║  ┌─────────────┐  ┌─────────────┐                  ║
            ║  │ Firewall #1 │  │    WAF      │  Chống DDoS,     ║
            ║  │   (HA)      │  │   (HA)      │  lọc tấn công    ║
            ║  └──────┬──────┘  └──────┬──────┘                  ║
            ╚─────────┼────────────────┼─────────────────────────╝
                      │                │
            ╔═════════▼════════════════▼═════════════════════════╗
            ║  VÙNG LOAD BALANCER                                ║
            ║  ┌─────────────┐  ┌─────────────┐                  ║
            ║  │   LB #1     │  │   LB #2     │  Phân tải,        ║
            ║  │             │  │             │  SSL/TLS          ║
            ║  └──────┬──────┘  └──────┬──────┘                  ║
            ╚─────────┼────────────────┼─────────────────────────╝
                      │                │
            ╔═════════▼════════════════▼═════════════════════════╗
            ║  VÙNG PROXY / API GATEWAY                          ║
            ║  ┌─────────────┐  ┌─────────────┐                  ║
            ║  │   NGINX     │  │  Kong GW    │  Routing,         ║
            ║  │  (SSL term) │  │ (rate-limit)│  JWT verify       ║
            ║  └─────────────┘  └──────┬──────┘                  ║
            ╚════════════════════════ │ ═════════════════════════╝
                                      │
            ╔═════════════════════════▼══════════════════════════╗
            ║  VÙNG FIREWALL CORE (IDS/IPS)                       ║
            ║  Kiểm tra chi tiết trước khi vào microservices      ║
            ╚═════════════════════════╤══════════════════════════╝
                                      │
            ╔═════════════════════════▼══════════════════════════╗
            ║  VÙNG ỨNG DỤNG & XỬ LÝ                            ║
            ║                                                      ║
            ║  ┌── K8s Cluster (CPU) ──────────────────────────┐  ║
            ║  │                                                │  ║
            ║  │  ┌─AI GW─┐ ┌─AI Engine─┐ ┌─Agent─┐ ┌─Admin─┐ │  ║
            ║  │  │       │ │           │ │Orch.  │ │API    │ │  ║
            ║  │  └───────┘ └───────────┘ └───────┘ └───────┘ │  ║
            ║  │  ┌─MCP─┐ ┌─Knowledge─┐ ┌─Auth─┐ ┌─Audit─┐  │  ║
            ║  │  │Srv  │ │  Worker   │ │      │ │       │  │  ║
            ║  │  └─────┘ └───────────┘ └──────┘ └───────┘  │  ║
            ║  │  ┌─Job Workers (tóm tắt, ingestion, OCR)─┐  │  ║
            ║  │  └────────────────────────────────────────┘  │  ║
            ║  └────────────────────────────────────────────────┘  ║
            ║                                                      ║
            ║  ┌── GPU Cluster (2-3 node A10 24GB) ─────────────┐  ║
            ║  │                                                │  ║
            ║  │  ┌─Ollama LLM─┐ ┌─OCR Worker─┐ ┌─Embedding─┐ │  ║
            ║  │  │  qwen2.5   │ │  PaddleOCR │ │  worker   │ │  ║
            ║  │  │  llama3.2  │ │  Tesseract │ │  bge-m3   │ │  ║
            ║  │  └────────────┘ └────────────┘ └────────────┘ │  ║
            ║  └────────────────────────────────────────────────┘  ║
            ╚════════════════════════════════════════════════════════╝
                                      │
            ╔═════════════════════════▼══════════════════════════╗
            ║  VÙNG SỰ KIỆN & BỘ ĐỆM                            ║
            ║  ┌──────────────┐  ┌──────────────────────────────┐ ║
            ║  │ Kafka Cluster│  │  Redis Cluster               │ ║
            ║  │ (8 broker)   │  │  (semantic cache, rate-limit)│ ║
            ║  └──────────────┘  └──────────────────────────────┘ ║
            ╚════════════════════════════════════════════════════════╝
                                      │
            ╔═════════════════════════▼══════════════════════════╗
            ║  VÙNG DỮ LIỆU (bảo vệ bởi DB Firewall)             ║
            ║  ┌──────────────┐  ┌──────────────┐ ┌────────────┐  ║
            ║  │ PostgreSQL   │  │ pgvector /   │ │  File      │  ║
            ║  │ (Patroni HA) │  │ Qdrant       │ │  Server    │  ║
            ║  │ + ai_platform│  │ (vector DB)  │ │  (MinIO)   │  ║
            ║  └──────────────┘  └──────────────┘ └────────────┘  ║
            ╚════════════════════════════════════════════════════════╝
```

**Cách đọc sơ đồ hạ tầng**:
- **7 vùng mạng** xếp chồng từ ngoài vào trong.
- **Văn bản mật** chỉ được xử lý trong **GPU Cluster** (Ollama), không bao giờ rời khỏi vùng nội bộ.
- **DB Firewall** bảo vệ vùng dữ liệu cuối cùng.

### Sơ đồ luồng dữ liệu RAG (chi tiết)

```
┌────────────────────────────────────────────────────────────────────────────┐
│                  PIPELINE RAG (Truy xuất tăng cường)                       │
│                                                                            │
│  Câu hỏi user ──► Embed ──► Hybrid Search ──► Rerank ──► LLM ──► Trả lời │
│                       │            │              │         │         │      │
│                       ▼            ▼              ▼         ▼         ▼      │
│                  ┌─────────┐  ┌──────────┐  ┌─────────┐ ┌──────┐ ┌──────┐  │
│                  │Vector   │  │BM25 +    │  │ BGE-    │ │GPT-4o│ │Trích │  │
│                  │(số đặc  │  │Cosine    │  │reranker │ │hoặc  │ │dẫn + │  │
│                  │trưng)   │  │similarity│  │(xếp lại)│ │qwen  │ │đánh  │  │
│                  │         │  │          │  │         │ │2.5   │ │giá   │  │
│                  └─────────┘  └──────────┘  └─────────┘ └──────┘ └──────┘  │
│                                                                            │
│  Dữ liệu vào:  1 câu hỏi                       Dữ liệu ra: 1 câu trả lời │
│                                            + danh sách trích dẫn (3-5 nguồn) │
│                                            + điểm tin cậy (0.0-1.0)          │
└────────────────────────────────────────────────────────────────────────────┘
```

**Giải thích pipeline RAG**:
1. **Embed** (Vector hóa): Biến câu hỏi thành dãy số 1536 chiều.
2. **Hybrid Search** (Tìm kiếm lai): Kết hợp tìm theo từ khóa (BM25) + tìm theo ngữ nghĩa (cosine similarity) → 50 văn bản ứng viên.
3. **Rerank** (Xếp hạng lại): Mô hình BGE-reranker chọn 8 văn bản tốt nhất.
4. **LLM** (Gọi mô hình AI): Gửi 8 văn bản + câu hỏi cho GPT-4o (cloud) hoặc qwen2.5 (local); nhận câu trả lời streaming.
5. **Trích dẫn + đánh giá**: Trích xuất nguồn (văn bản nào, trang nào) + đánh giá độ trung thực.

---

## II.5.1. Phân hệ AI Gateway

### II.5.1.1. Mô tả chức năng

**AI Gateway** là cổng tiếp nhận duy nhất (single entry point) cho mọi yêu cầu AI trong toàn hệ thống. Tất cả yêu cầu AI, dù đến từ người dùng trên web/app hay từ các hệ thống nghiệp vụ khác, đều phải đi qua AI Gateway trước.

**Hình dung đời thường**: AI Gateway giống như "lễ tân" của một khách sạn 5 sao. Mọi khách đều phải đến quầy lễ tân trước, được xác minh danh tính, được hướng dẫn đến đúng phòng, được ghi nhận giờ vào/ra, và được thông báo nếu có dịch vụ phù hợp.

**Các chức năng chính:**

| Chức năng | Mô tả dễ hiểu |
|---|---|
| Xác thực người dùng | Kiểm tra "thẻ nhân viên" (JWT) để biết ai đang gọi AI |
| Phân quyền | Kiểm tra người dùng có quyền sử dụng AI không, thuộc đơn vị nào |
| Giới hạn tốc độ | Mỗi người chỉ được hỏi tối đa 100 câu/phút, 5.000 câu/giờ |
| Lọc câu hỏi nguy hiểm | Tự động phát hiện và chặn các câu hỏi cố ý "lừa" AI |
| Che giấu thông tin cá nhân | Tự động ẩn số CCCD, SĐT, email trước khi gửi AI |
| Chọn mô hình AI phù hợp | Văn bản mật → dùng AI nội bộ; văn bản thường → dùng AI cloud chất lượng cao |
| Bộ nhớ đệm thông minh | Nếu 10 người hỏi cùng 1 câu, chỉ trả lời 1 lần, 9 người còn lại nhận kết quả có sẵn |
| Điều phối yêu cầu | Hỏi đơn giản → AI Engine; yêu cầu phức tạp → Agent đa bước |
| Ghi nhật ký | Mọi yêu cầu đều được ghi lại để phục vụ kiểm tra |
| Theo dõi chi phí | Tính tiền theo từng người dùng, từng đơn vị |

### II.5.1.2. Luồng xử lý chức năng

**Quy trình xử lý 1 yêu cầu AI** (từ lúc người dùng nhấn "Gửi" đến lúc nhận câu trả lời):

| Bước | Xử lý | Mô tả đời thường |
|---|---|---|
| 1 | Nhận yêu cầu | Lễ tân tiếp nhận khách |
| 2 | Xác minh "thẻ nhân viên" | Kiểm tra giấy tờ tùy thân |
| 3 | Kiểm tra giới hạn | Không cho khách thứ 101 vào nếu quá giới hạn |
| 4 | Lọc câu hỏi nguy hiểm | Loại bỏ khách có hành vi đáng ngờ |
| 5 | Che giấu thông tin cá nhân | Che số CCCD, SĐT trên giấy tờ |
| 6 | Kiểm tra bộ nhớ đệm | Đã có câu trả lời tương tự chưa? |
| 7 | Phân loại độ nhạy | Văn bản này có mật không? |
| 8 | Chọn mô hình AI | Văn bản mật → AI nội bộ; thường → AI cloud |
| 9 | Gửi tới AI Engine hoặc Agent | Hướng dẫn khách đến đúng phòng ban |
| 10 | Nhận câu trả lời | Lấy kết quả từ phòng ban |
| 11 | Ghi nhật ký | Ghi sổ vào/ra |
| 12 | Cập nhật chi phí | Cộng dồn vào hóa đơn đơn vị |
| 13 | Trả lời người dùng | Đưa kết quả cho khách |

### II.5.1.3. Tài liệu liên quan

| STT | Tên tài liệu | Mã tài liệu |
|---|---|---|
| 1 | Tài liệu thiết kế API của module AI | BTC-AI-API-001 |
| 2 | Tài liệu về OpenIddict JWT | OpenIddict Docs |
| 3 | Tài liệu về Kong Gateway | Kong Rate Limiting |

### II.5.1.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `AiGateway.Api` | Cửa tiếp nhận yêu cầu AI |
| 2 | `AiGateway.Application` | Xử lý nghiệp vụ (xác thực, kiểm tra quyền, ...) |
| 3 | `AiGateway.Domain` | Định nghĩa các đối tượng (yêu cầu, bộ đếm, cache) |
| 4 | `AiGateway.Infrastructure` | Kết nối tới Redis, OpenAI, các dịch vụ khác |

### II.5.1.5. Thiết kế giao diện

<Insert màn hình giao diện AI Admin Console – Tab "AI Gateway Overview">

Giao diện quản trị hiển thị các chỉ số thời gian thực của AI Gateway: số yêu cầu mỗi phút, thời gian phản hồi trung bình, tỷ lệ cache hit, tỷ lệ lỗi, chi phí theo đơn vị.

### II.5.1.6. Thành phần giao diện

| Tên control | Bắt buộc nhập | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Chọn nhà cung cấp AI | Không | Dropdown (OpenAI/Ollama/Tự động) | Chọn mô hình AI cho yêu cầu | Mặc định Tự động |
| Ghi đè độ nhạy | Không | Dropdown (Thấp/TB/Cao) | Admin test với độ nhạy cụ thể | Dành cho admin |
| Giới hạn output | Không | Số nguyên | Số token tối đa cho câu trả lời | 100-8000 |
| Độ sáng tạo | Không | Số thập phân | Mức độ "sáng tạo" của AI | 0.0-1.0 (khuyến nghị 0.0-0.5) |
| Streaming | Không | Checkbox | Bật trả lời theo từng phần | Mặc định bật |
| Dùng cache | Không | Checkbox | Có dùng bộ nhớ đệm không | Mặc định bật |
| Chế độ thử nghiệm | Không | Checkbox | Không gọi AI, chỉ kiểm tra | Dành cho test |

### II.5.1.7. Thiết kế hàm / thủ tục

**Hàm chính `ProcessRequest`**: nhận một yêu cầu AI, thực hiện đầy đủ 13 bước nêu trên, trả về câu trả lời cho người dùng.

Các bước xử lý được mô tả bằng ngôn ngữ tự nhiên:

1. **Xác minh thẻ nhân viên** từ JWT token, lấy thông tin user_id, tenant_id, roles.
2. **Kiểm tra giới hạn tốc độ** trong Redis; trả về lỗi 429 nếu vượt.
3. **Lọc câu hỏi nguy hiểm** bằng regex và mô hình AI bảo vệ.
4. **Che giấu thông tin cá nhân** (CCCD, SĐT, email) trong câu hỏi.
5. **Kiểm tra bộ nhớ đệm** bằng vector similarity; trả kết quả cache nếu ≥ 0.95 độ tương đồng.
6. **Phân loại độ nhạy** văn bản (Thấp/TB/Cao) bằng mô hình AI nhỏ chạy nội bộ.
7. **Chọn nhà cung cấp AI**: mật → Ollama nội bộ; thường → OpenAI.
8. **Chuyển tiếp yêu cầu** tới AI Engine (cho hỏi đơn giản) hoặc Agent (cho yêu cầu phức tạp).
9. **Nhận câu trả lời** streaming từ AI Engine/Agent.
10. **Lưu vào bộ nhớ đệm** với thời hạn 1 giờ.
11. **Ghi nhật ký** (user, tenant, hash câu hỏi, model, token, thời gian, chi phí).
12. **Cập nhật chi phí** của đơn vị vào Redis.
13. **Trả kết quả** cho người dùng.

---

## II.5.2. Phân hệ AI Engine Core

### II.5.2.1. Mô tả chức năng

**AI Engine Core** là "bộ não" trung tâm của module AI, nơi thực hiện các tác vụ AI quan trọng nhất: tóm tắt văn bản, trích xuất thông tin, phân loại, hỏi-đáp có trích dẫn (RAG).

**Hình dung đời thường**: AI Engine Core giống như một "thư ký AI" thông minh, có thể đọc hiểu văn bản, tóm tắt nội dung, trả lời câu hỏi dựa trên văn bản đã đọc, và luôn trích dẫn nguồn để người dùng kiểm chứng.

**Các chức năng chính:**

| Chức năng | Mô tả dễ hiểu |
|---|---|
| Tải mẫu câu hỏi | Lấy mẫu câu hỏi soạn sẵn, có phiên bản rõ ràng |
| Vector hóa câu hỏi | Biến câu hỏi thành dãy số đặc trưng để tìm kiếm |
| Tìm kiếm lai | Kết hợp tìm theo từ khóa + tìm theo ý nghĩa |
| Xếp hạng lại kết quả | Sắp xếp lại top kết quả cho chính xác nhất |
| Tổng hợp ngữ cảnh | Chọn thông tin liên quan nhất, tránh vượt quá giới hạn |
| Gọi AI sinh câu trả lời | Gửi câu hỏi + ngữ cảnh cho AI, nhận câu trả lời streaming |
| Kiểm tra chất lượng | Đánh giá câu trả lời có trung thực với văn bản gốc không |
| Trích xuất trích dẫn | Ghi rõ câu trả lời dựa trên văn bản nào, trang nào |
| Đánh giá chất lượng định kỳ | Đo lường chất lượng hệ thống theo thời gian |
| Hỗ trợ hội thoại nhiều lượt | Nhớ ngữ cảnh các câu hỏi trước trong cùng cuộc hội thoại |

### II.5.2.2. Luồng xử lý chức năng

**Quy trình hỏi-đáp có trích dẫn (RAG)** — use case quan trọng nhất:

| Bước | Xử lý | Mô tả đời thường |
|---|---|---|
| 1 | Tải mẫu câu hỏi | Lấy kịch bản hỏi đáp soạn sẵn |
| 2 | Vector hóa câu hỏi | Biến câu hỏi thành dãy số |
| 3 | Tìm kiếm lai | Tìm 50 văn bản liên quan nhất (kết hợp từ khóa + ý nghĩa) |
| 4 | Xếp hạng lại | Chọn 8 văn bản tốt nhất bằng mô hình AI xếp hạng |
| 5 | Tổng hợp ngữ cảnh | Đảm bảo tổng văn bản không quá 2048 token |
| 6 | Soạn câu hỏi đầy đủ | Gồm: hướng dẫn cho AI + 8 văn bản + câu hỏi user |
| 7 | Gọi AI sinh câu trả lời | Gửi cho AI và nhận câu trả lời theo từng phần |
| 8 | Kiểm tra trung thực | Câu trả lời có khớp với văn bản gốc không |
| 9 | Trích xuất trích dẫn | Ghi rõ câu trả lời dựa trên văn bản nào |
| 10 | Trả kết quả | Kèm câu trả lời, trích dẫn, điểm tin cậy, thông tin token |

### II.5.2.3. Tài liệu liên quan

| STT | Tên tài liệu | Mã tài liệu |
|---|---|---|
| 1 | LangChain RAG patterns | LangChain RAG |
| 2 | RAGAS Framework | RAGAS Docs |
| 3 | BGE Reranker | BAAI BGE |

### II.5.2.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `AiEngine.Api` | Dịch vụ gRPC tốc độ cao |
| 2 | `AiEngine.Application` | Xử lý pipeline RAG |
| 3 | `AiEngine.Domain` | Định nghĩa mẫu câu hỏi, hội thoại |
| 4 | `AiEngine.Infrastructure` | Kết nối tới embedding, retrieval, LLM |

### II.5.2.5. Thiết kế giao diện

<Insert màn hình giao diện AI Admin Console – Pipeline Builder>

Giao diện cho phép admin cấu hình pipeline RAG: chọn chiến lược tìm kiếm (từ khóa / ngữ nghĩa / kết hợp), chọn mô hình vector hóa, cấu hình xếp hạng lại, điều chỉnh số lượng kết quả, giới hạn token.

### II.5.2.6. Thành phần giao diện

| Tên control | Bắt buộc nhập | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Chiến lược tìm kiếm | Có | Dropdown | Cách tìm văn bản liên quan | Mặc định Kết hợp |
| Mô hình vector hóa | Có | Dropdown | Mô hình biến văn bản thành số | |
| Số kết quả lấy về | Có | Số nguyên (1-100) | Số văn bản ứng viên | Mặc định 50 |
| Số kết quả cuối | Có | Số nguyên (1-20) | Số văn bản đưa vào AI | Mặc định 8 |
| Bật xếp hạng lại | Không | Checkbox | Dùng mô hình AI xếp hạng lại | |
| Giới hạn token ngữ cảnh | Có | Số nguyên | Token tối đa cho phần "văn bản liên quan" | |
| Độ sáng tạo | Có | Số thập phân | Mức độ sáng tạo của AI | |
| Số token tối đa output | Có | Số nguyên | Độ dài tối đa câu trả lời | |
| Mô hình AI sinh câu trả lời | Có | Dropdown | Chọn mô hình AI | |

### II.5.2.7. Thiết kế hàm / thủ tục

**Hàm chính `Run`** (xử lý một câu hỏi và trả về câu trả lời có trích dẫn):

Các bước xử lý được mô tả bằng ngôn ngữ tự nhiên:

1. **Tải mẫu câu hỏi** phiên bản đang hoạt động từ DB.
2. **Vector hóa câu hỏi** bằng mô hình embedding.
3. **Tìm kiếm lai**: kết hợp tìm theo từ khóa (BM25) + tìm theo vector (cosine), hợp nhất bằng thuật toán RRF.
4. **Xếp hạng lại** top 50 → top 8 bằng mô hình BGE-reranker (nếu bật).
5. **Tổng hợp ngữ cảnh**: gộp 8 văn bản, loại bỏ trùng lặp, sắp xếp theo độ liên quan, đảm bảo tổng ≤ giới hạn token.
6. **Soạn câu hỏi đầy đủ** cho AI: gồm hướng dẫn (system prompt), ngữ cảnh, câu hỏi người dùng, lịch sử hội thoại.
7. **Gọi AI** qua Provider Abstraction, nhận câu trả lời theo từng phần (streaming).
8. **Trích xuất trích dẫn**: xác định câu trả lời dựa trên văn bản nào, trang nào, văn bản gốc nào.
9. **Kiểm tra trung thực**: dùng AI đánh giá xem câu trả lời có khớp với ngữ cảnh không (chống "bịa").
10. **Trả kết quả** gồm: câu trả lời, trích dẫn, điểm tin cậy, số token sử dụng.

---

## II.5.3. Phân hệ Agent Orchestration

### II.5.3.1. Mô tả chức năng

**Agent Orchestration** là "trợ lý AI đa bước" — thành phần giúp AI có thể xử lý các yêu cầu **phức tạp, đa bước** mà không thể giải quyết chỉ với một lần hỏi-đáp.

**Hình dung đời thường**: Bạn yêu cầu "Tìm văn bản quá hạn trong tuần, phân loại theo đơn vị, soạn báo cáo Excel, gửi email cho lãnh đạo Bộ". Con người sẽ phải làm 5 việc liên tiếp. Agent AI cũng vậy — nó sẽ tự lập kế hoạch 5 bước, thực hiện từng bước, và dừng lại hỏi bạn trước khi gửi email.

**Các chức năng chính:**

| Chức năng | Mô tả dễ hiểu |
|---|---|
| Phân loại yêu cầu | Xác định yêu cầu đơn giản hay phức tạp đa bước |
| Lập kế hoạch | Sinh chuỗi bước cần làm để hoàn thành yêu cầu |
| Điều phối trợ lý | Phối hợp 6 loại trợ lý chuyên biệt (tìm văn bản, phân tích, soạn thảo, xếp lịch, gửi thông báo, kiểm tra) |
| Gọi công cụ | Sử dụng 12+ công cụ (tìm văn bản, soạn email, đặt lịch họp, ...) |
| Lưu trạng thái | Nếu lỗi giữa chừng, có thể tiếp tục từ bước đang dở |
| Người duyệt trong vòng lặp | Với thao tác nhạy cảm (gửi email, ký số), dừng lại chờ người duyệt |
| Rào chắn an toàn | Chặn các thao tác nguy hiểm (xóa dữ liệu, thay đổi cấu hình quan trọng) |

### II.5.3.2. Luồng xử lý chức năng

**Quy trình xử lý yêu cầu đa bước**:

| Bước | Xử lý | Mô tả đời thường |
|---|---|---|
| 1 | Phân loại yêu cầu | Yêu cầu này đơn giản hay phức tạp? |
| 2 | Lập kế hoạch | Liệt kê 5 bước cần làm |
| 3 | Kiểm tra an toàn từng bước | Bước nào nguy hiểm → chặn hoặc yêu cầu duyệt |
| 4 | Thực hiện bước 1 | Trợ lý "Tìm văn bản" → 23 văn bản |
| 5 | Thực hiện bước 2 | Trợ lý "Phân tích" → phân nhóm 5 đơn vị |
| 6 | Thực hiện bước 3 | Trợ lý "Soạn thảo" → file Excel 24 KB |
| 7 | Thực hiện bước 4 | Trợ lý "Soạn thảo" → soạn email |
| 8 | Dừng chờ duyệt | Trợ lý "Gửi thông báo" → chờ user duyệt email |
| 9 | Trả kết quả | Báo cáo Excel đã sẵn sàng, email đang chờ duyệt |

### II.5.3.3. Tài liệu liên quan

| STT | Tên tài liệu | Mã tài liệu |
|---|---|---|
| 1 | Anthropic MCP Specification | MCP Spec |
| 2 | LangChain Agent Documentation | LangChain Agents |
| 3 | AutoGen Multi-Agent Framework | AutoGen |

### II.5.3.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `AgentOrchestrator.Api` | Dịch vụ gRPC |
| 2 | `AgentOrchestrator.Application` | Lập kế hoạch + điều phối |
| 3 | `AgentOrchestrator.Domain` | Định nghĩa trợ lý, kế hoạch |
| 4 | `AgentOrchestrator.Infrastructure` | Kết nối LLM, MCP, lưu trạng thái |

### II.5.3.5. Thiết kế giao diện

<Insert màn hình AI Admin Console – Agent Designer>

Giao diện cho phép admin tạo/sửa trợ lý: chọn mô hình AI, viết hướng dẫn cho trợ lý, kéo thả các công cụ được phép dùng, bật/tắt chế độ duyệt tay.

### II.5.3.6. Thành phần giao diện

| Tên control | Bắt buộc nhập | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Tên trợ lý | Có | Chuỗi | Tên trợ lý | Duy nhất |
| Mô hình AI | Có | Dropdown | Chọn mô hình AI | |
| Hướng dẫn cho AI | Có | Textarea | Vai trò & nguyên tắc của trợ lý | |
| Công cụ được phép | Không | Multi-select | Các công cụ trợ lý được dùng | |
| Bắt buộc duyệt tay | Không | Checkbox | Mọi thao tác đều cần user duyệt | Mặc định tắt |
| Số bước tối đa | Có | Số nguyên (1-20) | Giới hạn vòng lặp | Mặc định 10 |
| Quy tắc an toàn | Không | Multi-line | Chặn / duyệt theo từng thao tác | |

### II.5.3.7. Thiết kế hàm / thủ tục

**Hàm chính `Execute`** (xử lý một yêu cầu đa bước):

Các bước xử lý được mô tả bằng ngôn ngữ tự nhiên:

1. **Phân loại yêu cầu**: yêu cầu này là đơn giản (dùng AI Engine) hay đa bước (dùng Agent)?
2. **Nếu đơn giản**: chuyển thẳng cho AI Engine xử lý.
3. **Nếu phức tạp**: gọi AI lập kế hoạch (vd: 5 bước), sinh ra chuỗi JSON mô tả từng bước.
4. **Khởi tạo phiên thực thi** với trạng thái rỗng.
5. **Lặp qua từng bước**:
   - Kiểm tra rào chắn an toàn (vd: "xóa dữ liệu" → chặn).
   - Nếu thao tác yêu cầu duyệt (vd: "gửi email") → dừng, chờ user duyệt.
   - Chọn trợ lý phù hợp với bước (tìm văn bản / phân tích / soạn thảo / ...).
   - Gọi trợ lý thực hiện, lưu kết quả vào trạng thái.
6. **Tổng hợp kết quả** cuối cùng.
7. **Trả về** kết quả tổng hợp + các kết quả trung gian + trạng thái duyệt.

---

## II.5.4. Phân hệ Knowledge Base & RAG

### II.5.4.1. Mô tả chức năng

**Knowledge Base & RAG** quản lý **kho tri thức** của AI — tất cả văn bản mà AI được phép "đọc" và "trả lời dựa trên". Phân hệ này cũng thực hiện việc **nhập tri thức tự động** (ingestion pipeline) và **tìm kiếm ngữ nghĩa**.

**Hình dung đời thường**: Kho tri thức giống như một thư viện khổng lồ. AI có thể đọc tất cả sách trong thư viện, và khi bạn hỏi, AI sẽ tìm đúng trang sách liên quan để trả lời. "Ingestion pipeline" là quy trình thủ thư dùng để nhập sách mới vào thư viện (phân loại, đánh mục lục, ...).

**Các chức năng chính:**

| Chức năng | Mô tả dễ hiểu |
|---|---|
| Quản lý nguồn tri thức | Thêm/sửa/xóa nguồn (văn bản Bộ, file upload, web, HRM, ...) |
| Quy trình nhập tri thức tự động | Lấy văn bản → trích text → làm sạch → chia đoạn ~1 trang → vector hóa → lưu kho |
| Lập lịch nhập tự động | Mỗi đêm tự động đồng bộ văn bản mới từ DMS |
| Nhập thời gian thực | Khi có văn bản mới, ngay lập tức đưa vào kho |
| Tìm kiếm lai | Kết hợp tìm từ khóa + tìm ngữ nghĩa |
| Xếp hạng lại kết quả | Mô hình AI sắp xếp lại top kết quả |
| Lọc theo đơn vị và độ mật | Tổng cục Thuế chỉ thấy văn bản của Tổng cục Thuế |

### II.5.4.2. Luồng xử lý chức năng

**Quy trình nhập tri thức (Ingestion)**:

| Bước | Xử lý | Mô tả đời thường |
|---|---|---|
| 1 | Lấy văn bản từ nguồn | Vào DMS, lấy 2.500.000 văn bản |
| 2 | Trích text | Mở PDF/DOCX, lấy phần chữ; PDF scan → dùng OCR |
| 3 | Làm sạch | Bỏ header/footer lặp lại, sửa lỗi chính tả |
| 4 | Chia đoạn | Mỗi văn bản dài → 5 đoạn ~1 trang |
| 5 | Trích thông tin mẫu | Số văn bản, ngày, người ký → lưu metadata |
| 6 | Vector hóa | Mỗi đoạn → dãy số 1536 chiều |
| 7 | Lưu kho | Lưu vào PostgreSQL + Vector Database |
| 8 | Ghi log | Ghi lại trạng thái: bao nhiêu văn bản, bao nhiêu đoạn, lỗi gì |

**Quy trình tìm kiếm (Retrieval)**:

| Bước | Xử lý | Mô tả đời thường |
|---|---|---|
| 1 | Vector hóa câu hỏi | Biến câu hỏi thành dãy số |
| 2 | Tìm theo từ khóa | Tìm 50 văn bản có chứa từ khóa |
| 3 | Tìm theo ngữ nghĩa | Tìm 50 văn bản có ý nghĩa tương tự |
| 4 | Hợp nhất | Kết hợp 2 danh sách bằng thuật toán RRF |
| 5 | Xếp hạng lại | Mô hình AI chọn 8 văn bản tốt nhất |
| 6 | Lọc theo đơn vị | Chỉ giữ văn bản của đơn vị người hỏi |
| 7 | Trả kết quả | 8 văn bản liên quan nhất |

### II.5.4.3. Tài liệu liên quan

| STT | Tên tài liệu | Mã tài liệu |
|---|---|---|
| 1 | Tài liệu pgvector | pgvector docs |
| 2 | Tài liệu Qdrant | Qdrant docs |
| 3 | Tài liệu BM25 algorithm | BM25 wiki |
| 4 | Tài liệu BGE Reranker | BAAI |

### II.5.4.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `Knowledge.Api` | REST API cho tìm kiếm và quản trị |
| 2 | `Knowledge.Application` | Xử lý nhập + tìm kiếm |
| 3 | `Knowledge.Domain` | Định nghĩa nguồn tri thức, đoạn tri thức |
| 4 | `Knowledge.Infrastructure` | Kết nối Vector DB, Embedding, BM25 |
| 5 | `Knowledge.Worker` | Worker nền xử lý nhập tri thức |

### II.5.4.5. Thiết kế giao diện

<Insert màn hình AI Admin Console – Knowledge Sources>

### II.5.4.6. Thành phần giao diện

| Tên control | Bắt buộc nhập | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Tên nguồn | Có | Chuỗi | Tên nguồn tri thức | |
| Loại nguồn | Có | Dropdown | DMS / File / Web / HRM / ERP | |
| Cấu hình kết nối | Có | JSON | Thông tin kết nối | |
| Chiến lược chia đoạn | Có | Dropdown | Cách chia văn bản thành đoạn | |
| Kích thước đoạn | Có | Số nguyên (100-2000) | Số token mỗi đoạn | 512 |
| Phần chồng lấn | Có | Số nguyên (0-200) | Phần trùng giữa 2 đoạn liên tiếp | 50 |
| Mô hình vector hóa | Có | Dropdown | Mô hình biến văn bản thành số | |
| Lịch nhập | Không | Cron | Lịch tự động đồng bộ | |
| Bộ lọc văn bản | Không | JSON | Lọc văn bản được nhập | |

### II.5.4.7. Thiết kế hàm / thủ tục

**Hàm `IngestSource`** (nhập một nguồn tri thức vào kho):

Các bước xử lý được mô tả bằng ngôn ngữ tự nhiên:

1. **Lấy thông tin nguồn** từ DB (loại nguồn, cấu hình kết nối, chiến lược chia đoạn, ...).
2. **Trích xuất tài liệu** từ nguồn (gọi API DMS, đọc file, crawl web, ...).
3. **Chia đoạn** mỗi tài liệu thành các đoạn ~512 token với phần chồng lấn 50 token.
4. **Vector hóa hàng loạt** (mỗi lần 100 đoạn) bằng mô hình embedding.
5. **Lưu vào Vector Database** với metadata (nguồn, văn bản gốc, ngày, ...).
6. **Cập nhật trạng thái nguồn**: "đã nhập", số đoạn đã xử lý, thời gian nhập gần nhất.

**Hàm `Search`** (tìm kiếm văn bản liên quan):

Các bước xử lý được mô tả bằng ngôn ngữ tự nhiên:

1. **Vector hóa câu hỏi** bằng mô hình embedding.
2. **Tìm theo từ khóa** (BM25) — lấy top 50.
3. **Tìm theo ngữ nghĩa** (vector cosine) — lấy top 50.
4. **Hợp nhất** 2 danh sách bằng thuật toán RRF.
5. **Xếp hạng lại** bằng mô hình BGE-reranker — chọn top 8.
6. **Trả về** 8 văn bản liên quan nhất kèm metadata.

---

## II.5.5. Phân hệ OCR & Document Extraction

### II.5.5.1. Mô tả chức năng

**OCR & Document Extraction** chuyển đổi văn bản scan (PDF scan, ảnh chụp) sang dạng số có thể chỉnh sửa và tìm kiếm được, đồng thời trích xuất các thông tin có cấu trúc (số văn bản, ngày tháng, người ký, bảng biểu, ...).

**Hình dung đời thường**: OCR giống như một "người đánh máy" cực nhanh, có thể đọc ảnh chụp văn bản và gõ lại thành file Word, đồng thời tự động điền vào các ô thông tin mẫu (số văn bản, ngày, ...).

**Các chức năng chính:**

| Chức năng | Mô tả dời thường |
|---|---|
| Nhận dạng tiếng Việt | Đọc chữ in, chữ viết tay từ ảnh/PDF scan tiếng Việt |
| Tách bố cục | Phân biệt phần đầu trang, nội dung, bảng biểu, chữ ký |
| Trích xuất bảng biểu | Lấy bảng từ văn bản scan → CSV/JSON |
| Trích thông tin mẫu | Tự động điền: số văn bản, ngày, người ký, lĩnh vực, độ mật |
| Hỗ trợ nhiều trang | Xử lý văn bản dài nhiều trang song song |

### II.5.5.2. Luồng xử lý chức năng

| Bước | Xử lý | Mô tả đời thường |
|---|---|---|
| 1 | Tiền xử lý ảnh | Tăng độ phân giải, xoay ảnh đúng chiều, lọc nhiễu |
| 2 | Nhận dạng OCR | "Người đánh máy" đọc từng trang, sinh text |
| 3 | Tách bố cục | Phân biệt tiêu đề, nội dung, bảng, chữ ký |
| 4 | Trích bảng biểu | Bảng → CSV/JSON |
| 5 | Trích thông tin mẫu | Dùng AI đọc và điền các trường thông tin |
| 6 | Chuẩn hóa về schema | Đưa về định dạng chuẩn của Bộ |
| 7 | Trả kết quả | Text + metadata + bảng |

### II.5.5.3. Tài liệu liên quan

| STT | Tên tài liệu | Mã tài liệu |
|---|---|---|
| 1 | Tesseract OCR | Tesseract |
| 2 | PaddleOCR | PaddleOCR |
| 3 | Table Transformer | Table-Transformer |
| 4 | Donut model | Donut |

### II.5.5.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `OcrWorker` | Worker nền xử lý OCR |
| 2 | `Ocr.Api` | REST API cho admin gửi tác vụ |
| 3 | `Ocr.Domain` | Định nghĩa tác vụ OCR, kết quả |
| 4 | `Ocr.Infrastructure` | Kết nối Tesseract/PaddleOCR |

### II.5.5.5. Thiết kế giao diện

<Insert màn hình AI Admin Console – OCR Jobs>

### II.5.5.6. Thành phần giao diện

| Tên control | Bắt buộc nhập | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Upload file | Có | File (PDF/PNG/JPG/TIFF) | Văn bản scan đầu vào | Tối đa 50MB |
| Engine OCR | Có | Dropdown | Phần mềm nhận dạng | Mặc định PaddleOCR |
| Ngôn ngữ | Có | Dropdown (vi/en) | Ngôn ngữ văn bản | Mặc định vi |
| DPI | Có | Số nguyên (150-600) | Độ phân giải quét | 300 |
| Trích bảng | Không | Checkbox | Có trích bảng biểu không | Bật |
| Trích thông tin mẫu | Không | Checkbox | Dùng AI để trích metadata | Bật |
| Lưu vào kho tri thức | Không | Checkbox | Có lưu vào kho tri thức để hỏi-đáp | |

### II.5.5.7. Thiết kế hàm / thủ tục

**Hàm `Process`** (xử lý OCR cho 1 văn bản scan):

Các bước xử lý được mô tả bằng ngôn ngữ tự nhiên:

1. **Tiền xử lý**: tăng DPI, xoay ảnh đúng chiều, lọc nhiễu cho từng trang.
2. **Nhận dạng OCR** song song cho từng trang (tiếng Việt).
3. **Tách bố cục**: phân biệt tiêu đề, nội dung, bảng, chữ ký.
4. **Trích bảng**: nếu có bảng → chuyển thành JSON.
5. **Trích thông tin mẫu** bằng AI từ toàn bộ text.
6. **Chuẩn hóa** metadata về định dạng chuẩn của Bộ.
7. **Trả kết quả** gồm: text đầy đủ, bố cục, bảng, metadata, điểm tin cậy.

---

## II.5.6. Phân hệ Provider Abstraction (LLM)

### II.5.6.1. Mô tả chức năng

**Provider Abstraction** cung cấp một giao diện thống nhất để kết nối tới **nhiều nhà cung cấp mô hình AI** khác nhau, giúp hệ thống không bị phụ thuộc vào một nhà cung cấp duy nhất.

**Hình dung đời thường**: Provider Abstraction giống như ổ cắm điện đa năng. Bạn có thể cắm bất kỳ thiết bị nào (máy tính, tivi, tủ lạnh) vào cùng một ổ cắm, miễn là nó tuân theo chuẩn. Tương tự, AI Engine có thể gọi OpenAI, Anthropic, Ollama, ... qua cùng một giao diện.

**Các chức năng chính:**

| Chức năng | Mô tả dễ hiểu |
|---|---|
| Quản lý nhà cung cấp | Thêm/sửa/xóa thông tin kết nối tới OpenAI, Anthropic, Ollama, Azure |
| Giao diện thống nhất | Một API duy nhất để gọi bất kỳ mô hình AI nào |
| Chọn nhà cung cấp tự động | Dựa trên độ nhạy văn bản, đơn vị, chi phí |
| Dự phòng tự động | Nếu OpenAI lỗi → tự chuyển sang Ollama |
| Theo dõi chi phí | Tính tiền theo từng nhà cung cấp |

### II.5.6.2. Luồng xử lý chức năng

| Bước | Xử lý | Mô tả đời thường |
|---|---|---|
| 1 | Nhận yêu cầu (prompt, use case, độ nhạy, đơn vị) | Lễ tân nhận yêu cầu |
| 2 | Chọn nhà cung cấp theo quy tắc | Văn bản mật → Ollama nội bộ; thường → OpenAI |
| 3 | Gọi mô hình AI | Gửi yêu cầu, nhận câu trả lời streaming |
| 4 | Nếu lỗi → thử nhà cung cấp dự phòng | OpenAI lỗi → thử Ollama |
| 5 | Trả kết quả + thông tin chi phí | Câu trả lời + token đã dùng + chi phí |

### II.5.6.3. Tài liệu liên quan

| STT | Tên tài liệu | Mã tài liệu |
|---|---|---|
| 1 | OpenAI API Specification | OpenAI Docs |
| 2 | Anthropic Claude API | Anthropic Docs |
| 3 | Ollama API | Ollama Docs |
| 4 | Azure OpenAI | Azure OpenAI |

### II.5.6.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `ProviderGateway.Api` | Dịch vụ gRPC |
| 2 | `ProviderGateway.Domain` | Định nghĩa nhà cung cấp, mô hình |
| 3 | `ProviderGateway.Infrastructure` | Kết nối tới OpenAI/Anthropic/Ollama |

### II.5.6.5. Thiết kế giao diện

<Insert màn hình AI Admin Console – Provider Management>

### II.5.6.6. Thành phần giao diện

| Tên control | Bắt buộc nhập | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Tên nhà cung cấp | Có | Chuỗi | Tên nhà cung cấp | Duy nhất |
| Loại | Có | Dropdown | OpenAI / Ollama / Anthropic / Azure | |
| Endpoint | Có | URL | Địa chỉ kết nối | |
| Khóa API | Có | Mật khẩu | Khóa truy cập | Lưu trong Vault |
| Tên mô hình | Có | Chuỗi | Tên mô hình AI | |
| Chi phí đầu vào | Có | Số thập phân | USD/1M token input | |
| Chi phí đầu ra | Có | Số thập phân | USD/1M token output | |
| Mặc định cho đơn vị | Không | Dropdown | Nhà cung cấp mặc định cho đơn vị | |

### II.5.6.7. Thiết kế hàm / thủ tục

**Hàm `Select`** (chọn nhà cung cấp):

Các bước xử lý được mô tả bằng ngôn ngữ tự nhiên:

1. **Nếu độ nhạy "Cao"** → chọn Ollama nội bộ (bắt buộc).
2. **Nếu đơn vị không cho phép cloud** → chọn Ollama.
3. **Nếu ngân sách còn lại < 20%** → chọn Ollama (rẻ hơn).
4. **Mặc định** → chọn OpenAI (chất lượng cao).

**Hàm `Generate`** (gọi mô hình AI với cơ chế dự phòng):

Các bước xử lý được mô tả bằng ngôn ngữ tự nhiên:

1. **Lấy chuỗi dự phòng** (vd: OpenAI → Ollama).
2. **Thử lần lượt** từng nhà cung cấp.
3. **Nếu thành công** → trả kết quả.
4. **Nếu tất cả thất bại** → trả lỗi "Tất cả nhà cung cấp đều không khả dụng".

---

## II.5.7. Phân hệ AI Job & Task Queue

### II.5.7.1. Mô tả chức năng

**AI Job & Task Queue** quản lý các tác vụ AI **nặng** cần xử lý **bất đồng bộ** (không đợi kết quả ngay). Ví dụ: tóm tắt 1.000 văn bản, OCR văn bản 200 trang, nhập tri thức cho 10 năm văn bản, ...

**Hình dung đời thường**: Job Queue giống như hệ thống xếp hàng ở bệnh viện. Bệnh nhân lấy số, chờ tới lượt, vào khám. Khi bác sĩ bận, hệ thống tự sắp xếp lại cho hợp lý. Nếu ca khám thất bại, có bác sĩ khác tiếp quản.

**Các chức năng chính:**

| Chức năng | Mô tả dời thường |
|---|---|
| Gửi tác vụ | Người dùng gửi yêu cầu OCR/tóm tắt → nhận mã số |
| Phân luồng | Tác vụ OCR → worker OCR; tóm tắt → worker tóm tắt |
| Tự động mở rộng | Nhiều tác vụ → tự thêm worker; ít tác vụ → giảm worker |
| Tự thử lại | Tác vụ lỗi → thử lại 3 lần, tăng dần thời gian chờ |
| Hàng đợi chết | Tác vụ lỗi 3 lần → đưa vào "kho lưu" để admin xử lý |
| Theo dõi trạng thái | User xem được: chờ / đang chạy / xong / lỗi |
| Thông báo kết quả | Khi xong → gửi thông báo qua app/email |

### II.5.7.2. Luồng xử lý chức năng

| Bước | Xử lý | Mô tả đời thường |
|---|---|---|
| 1 | Nhận tác vụ từ client | User upload file, bấm "OCR" |
| 2 | Lưu vào DB với trạng thái "chờ" | Bệnh nhân lấy số, chờ tới lượt |
| 3 | Đẩy vào hàng đợi Kafka | Sắp vào hàng |
| 4 | Worker nhận tác vụ | Bác sĩ rảnh → gọi bệnh nhân |
| 5 | Worker xử lý | Bác sĩ khám |
| 6 | Cập nhật trạng thái "xong" | Khám xong, trả kết quả |
| 7 | Thông báo cho user | Gọi tên, đưa kết quả |
| 8 | Nếu lỗi → thử lại 3 lần | Nếu lỗi, thử lại |
| 9 | Sau 3 lần → đưa vào "kho lưu" | Đưa vào phòng chờ đặc biệt để admin xử lý |

### II.5.7.3. Tài liệu liên quan

| STT | Tên tài liệu | Mã tài liệu |
|---|---|---|
| 1 | Apache Kafka | Kafka Docs |
| 2 | MassTransit for .NET | MassTransit |
| 3 | Kubernetes HPA | K8s HPA |

### II.5.7.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `AiJob.Api` | REST API nhận và theo dõi tác vụ |
| 2 | `AiJob.Worker.Summarize` | Worker xử lý tác vụ tóm tắt |
| 3 | `AiJob.Worker.Ocr` | Worker xử lý tác vụ OCR |
| 4 | `AiJob.Worker.Ingest` | Worker xử lý tác vụ nhập tri thức |
| 5 | `AiJob.Worker.Agent` | Worker xử lý tác vụ trợ lý đa bước |
| 6 | `AiJob.Domain` | Định nghĩa tác vụ AI |

### II.5.7.5. Thiết kế giao diện

<Insert màn hình AI Admin Console – Job Monitor>

### II.5.7.6. Thành phần giao diện

| Tên control | Bắt buộc nhập | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Loại tác vụ | Có | Dropdown | Tóm tắt / OCR / Nhập tri thức / Trợ lý | |
| Dữ liệu đầu vào | Có | JSON | Dữ liệu để xử lý | |
| Mức ưu tiên | Không | Dropdown | Thấp / Thường / Cao | |
| Số lần thử tối đa | Không | Số nguyên | Số lần thử lại khi lỗi | 3 |

### II.5.7.7. Thiết kế hàm / thủ tục

**Worker xử lý tác vụ tóm tắt** (một worker trong pool):

Các bước xử lý được mô tả bằng ngôn ngữ tự nhiên:

1. **Nhận tác vụ** từ hàng đợi Kafka.
2. **Cập nhật trạng thái** "đang chạy".
3. **Gọi AI Engine** tóm tắt văn bản.
4. **Cập nhật trạng thái** "xong" và lưu kết quả.
5. **Thông báo** cho user qua WebSocket.
6. **Nếu lỗi**: đợi 5 giây, thử lại; lỗi lần 3 → đưa vào hàng đợi chết.

---

## II.5.8. Phân hệ MCP Server

### II.5.8.1. Mô tả chức năng

**MCP Server** cung cấp "kho công cụ" mà trợ lý AI được phép sử dụng khi thực hiện yêu cầu đa bước. MCP (Model Context Protocol) là chuẩn giao tiếp do Anthropic đề xuất, giúp AI có thể gọi công cụ một cách linh hoạt.

**Hình dung đời thường**: MCP Server giống như "hộp đồ nghề" của một thợ sửa ống nước. Trong hộp có nhiều công cụ (kìm, mỏ lết, cờ lê, ...), mỗi công cụ dùng cho một việc. Trợ lý AI sẽ tự chọn công cụ phù hợp với từng bước công việc.

**Kho công cụ chính (12 công cụ):**

| # | Công cụ | Mô tả | Cần duyệt? |
|---|---|---|---|
| 1 | `tìm_văn_bản` | Tìm văn bản trong DMS | Không |
| 2 | `lấy_văn_bản` | Lấy nội dung đầy đủ 1 văn bản | Không |
| 3 | `tóm_tắt` | Tóm tắt văn bản | Không |
| 4 | `phân_loại` | Phân loại văn bản | Không |
| 5 | `ocr_văn_bản` | Trích xuất từ văn bản scan | Không |
| 6 | `danh_sách_công_việc` | Lấy danh sách công việc của user | Không |
| 7 | `tạo_nhật_ký` | Tạo nhật ký công việc | **Có** |
| 8 | `gửi_email` | Gửi email | **Có** |
| 9 | `ký_số` | Ký số văn bản | **Có** |
| 10 | `xem_lịch` | Xem lịch làm việc | Không |
| 11 | `đặt_lịch_họp` | Đặt lịch họp mới | **Có** |
| 12 | `hỏi_sql` | Sinh SQL từ câu hỏi tiếng Việt | Không |

### II.5.8.2. Luồng xử lý chức năng

| Bước | Xử lý | Mô tả đời thường |
|---|---|---|
| 1 | Trợ lý AI quyết định cần dùng công cụ | Thợ sửa ống nước quyết định cần mỏ lết |
| 2 | Gửi yêu cầu JSON-RPC | "Cho tôi mỏ lết cỡ 15" |
| 3 | MCP Server xác thực và kiểm tra quyền | Kiểm tra thợ có được mượn công cụ không |
| 4 | Kiểm tra có cần duyệt không | Mỏ lết thường không cần duyệt; kìm cắt ống → cần duyệt |
| 5 | Gọi hàm xử lý công cụ | Mở hộp, lấy công cụ |
| 6 | Ghi log và trả kết quả | Trả công cụ + ghi sổ mượn/trả |

### II.5.8.3. Tài liệu liên quan

| STT | Tên tài liệu | Mã tài liệu |
|---|---|---|
| 1 | Model Context Protocol Specification | MCP Spec |
| 2 | JSON-RPC 2.0 | JSON-RPC |

### II.5.8.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `McpServer` | Máy chủ JSON-RPC cung cấp công cụ |
| 2 | `McpServer.Tools` | Triển khai 12 công cụ |
| 3 | `McpServer.Domain` | Định nghĩa công cụ, lời gọi |

### II.5.8.5. Thiết kế giao diện

<Insert màn hình AI Admin Console – Tool Registry>

### II.5.8.6. Thành phần giao diện

| Tên control | Bắt buộc nhập | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Tên công cụ | Có | Chuỗi | Tên công cụ | Duy nhất |
| Mô tả | Có | Textarea | Mô tả cho AI hiểu | |
| Schema đầu vào | Có | JSON | Các tham số cần thiết | |
| Quyền cần thiết | Có | Multi-select | Quyền để dùng công cụ | |
| Cần duyệt tay | Không | Checkbox | Thao tác có cần user duyệt | |
| Ghi log | Không | Checkbox | Có ghi log khi dùng | Mặc định bật |
| Bật/tắt | Không | Checkbox | Cho phép dùng hay không | |

### II.5.8.7. Thiết kế hàm / thủ tục

**Một công cụ mẫu `tìm_văn_bản`** (công cụ tìm kiếm văn bản):

Các bước xử lý được mô tả bằng ngôn ngữ tự nhiên:

1. **Nhận yêu cầu** từ AI (câu truy vấn, số kết quả mong muốn).
2. **Lấy thông tin đơn vị** từ JWT của người dùng.
3. **Gọi Knowledge Service** tìm kiếm theo đơn vị.
4. **Ghi log**: ai tìm, lúc nào, tìm gì, có bao nhiêu kết quả.
5. **Trả kết quả** về cho AI.

---

## II.5.9. Phân hệ Audit & Observability

### II.5.9.1. Mô tả chức năng

**Audit & Observability** đảm bảo mọi hoạt động AI đều được ghi nhận, đo lường và truy vết được, phục vụ 3 mục đích: kiểm tra tuân thủ (audit), giám sát vận hành (monitoring), và truy tìm lỗi (debug).

**Hình dung đời thường**: Giống như "hộp đen" của máy bay. Mọi thứ đều được ghi lại — ai làm gì, lúc nào, trong bao lâu, tốn bao nhiêu. Khi có sự cố, có thể xem lại để tìm nguyên nhân.

**Các chức năng chính:**

| Chức năng | Mô tả dời thường |
|---|---|
| Nhật ký kiểm tra | Ghi lại: ai hỏi gì, AI trả lời gì, tốn bao nhiêu, lúc nào |
| Chỉ số thời gian thực | Tốc độ phản hồi, tỷ lệ lỗi, chi phí theo đơn vị |
| Truy vết phân tán | Theo dõi 1 yêu cầu đi qua bao nhiêu phân hệ, mất bao lâu ở mỗi nơi |
| Tổng hợp log | Gom log từ tất cả phân hệ về 1 nơi để dễ tìm kiếm |
| Bảng điều khiển | Dashboard Grafana hiển thị trực quan |
| Cảnh báo tự động | Gửi tin nhắn cho admin khi có chỉ số vượt ngưỡng |
| Tìm kiếm nhật ký | Tìm theo user, đơn vị, thời gian, model, ... |
| Theo dõi chi phí | Báo cáo chi phí theo đơn vị, theo người dùng |

### II.5.9.2. Luồng xử lý chức năng

**Vòng đời của 1 yêu cầu AI** (từ lúc gửi đến lúc trả lời):

| Bước | Xử lý | Mô tả đời thường |
|---|---|---|
| 1 | AI Gateway: tạo mã truy vết | Cấp số phiếu theo dõi |
| 2 | Ghi nhận: xác thực | Bước 1: xác minh giấy tờ |
| 3 | Ghi nhận: lọc câu hỏi | Bước 2: kiểm tra an toàn |
| 4 | Ghi nhận: truy xuất tri thức | Bước 3: tìm văn bản liên quan |
| 5 | Ghi nhận: xếp hạng lại | Bước 4: chọn văn bản tốt nhất |
| 6 | Ghi nhận: sinh câu trả lời | Bước 5: AI sinh câu trả lời |
| 7 | Ghi nhận kiểm tra | Bước 6: kiểm tra chất lượng |
| 8 | Lưu log tổng hợp | Ghi sổ tổng |
| 9 | Cập nhật chỉ số | Cộng vào bảng thống kê |
| 10 | Gửi log về hệ thống tập trung | Chuyển về kho lưu trữ log |

### II.5.9.3. Tài liệu liên quan

| STT | Tên tài liệu | Mã tài liệu |
|---|---|---|
| 1 | OpenTelemetry | OTel Docs |
| 2 | Prometheus | Prometheus |
| 3 | Grafana | Grafana |
| 4 | ELK Stack | Elastic |

### II.5.9.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `Audit.Api` | REST API tìm kiếm nhật ký |
| 2 | `Audit.Domain` | Định nghĩa nhật ký, bảng chi phí |
| 3 | `Audit.Infrastructure` | Kết nối OpenTelemetry, Prometheus |

### II.5.9.5. Thiết kế giao diện

<Insert màn hình AI Admin Console – Audit Search>

### II.5.9.6. Thành phần giao diện

| Tên control | Bắt buộc nhập | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Lọc theo đơn vị | Không | Dropdown | Chỉ xem log của đơn vị | |
| Lọc theo user | Không | Dropdown | Chỉ xem log của user | |
| Lọc theo thao tác | Không | Dropdown | Loại thao tác (hỏi, tóm tắt, OCR, ...) | |
| Khoảng thời gian | Không | DateRangePicker | Từ ngày - đến ngày | |
| Lọc theo model | Không | Dropdown | Mô hình AI đã dùng | |
| Tìm trong nội dung | Không | Chuỗi | Tìm từ khóa trong câu hỏi/trả lời | |

### II.5.9.7. Thiết kế hàm / thủ tục

**Hàm `Log`** (ghi nhận 1 sự kiện AI):

Các bước xử lý được mô tả bằng ngôn ngữ tự nhiên:

1. **Lưu vào DB** các thông tin: đơn vị, user, thao tác, hash câu hỏi, tóm tắt câu trả lời, model, số token, thời gian xử lý, chi phí, mã truy vết.
2. **Cập nhật chỉ số Prometheus**: tăng đếm số yêu cầu, tổng token, tổng chi phí.
3. **Gửi log** về hệ thống tập trung (ELK).
4. **Gửi trace** về Jaeger.

---

## II.5.10. Phân hệ AI Admin Console

### II.5.10.1. Mô tả chức năng

**AI Admin Console** là giao diện web dành cho admin của Bộ Tài Chính để cấu hình, giám sát và vận hành toàn bộ module AI.

**Hình dung đời thường**: Giống như bảng điều khiển trung tâm của một tòa nhà thông minh — admin có thể bật/tắt đèn, điều chỉnh nhiệt độ, xem camera, kiểm tra năng lượng tiêu thụ, ... tất cả từ một màn hình.

**Các chức năng chính (11 nhóm):**

| # | Tab | Chức năng |
|---|---|---|
| 1 | **Dashboard** | Xem chỉ số tổng quan thời gian thực |
| 2 | **Knowledge Sources** | Quản lý nguồn tri thức |
| 3 | **Prompt Templates** | Quản lý mẫu câu hỏi (có phiên bản) |
| 4 | **Agents** | Thiết kế trợ lý AI |
| 5 | **Tools** | Quản lý kho công cụ |
| 6 | **Providers** | Cấu hình nhà cung cấp AI |
| 7 | **Jobs** | Theo dõi tác vụ AI thời gian thực |
| 8 | **Audit** | Tìm kiếm nhật ký kiểm tra |
| 9 | **Reports** | Xem báo cáo chất lượng RAG định kỳ |
| 10 | **Feedback** | Xem đánh giá chất lượng từ người dùng |
| 11 | **Settings** | Quản lý user, cảnh báo chi phí |

### II.5.10.2. Luồng xử lý chức năng

Admin đăng nhập vào Admin Console → thấy sidebar với 11 tab → chọn tab cần dùng.

**Ví dụ luồng "Thêm nguồn tri thức mới"**:
1. Admin chọn tab "Knowledge Sources".
2. Bấm "Thêm mới".
3. Điền thông tin: tên, loại nguồn, cấu hình kết nối, chiến lược chia đoạn.
4. Bấm "Lưu" → hệ thống kiểm tra kết nối.
5. Nếu OK → kích hoạt nhập tri thức.
6. Admin theo dõi tiến độ nhập qua tab "Jobs".

### II.5.10.3. Tài liệu liên quan

| STT | Tên tài liệu | Mã tài liệu |
|---|---|---|
| 1 | SmartOffice Admin Portal | Admin Portal |
| 2 | Ant Design / Material UI | UI framework |

### II.5.10.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `AiAdmin.Web` | Giao diện web React |
| 2 | `AiAdmin.Api` | REST API cho admin |
| 3 | `AiAdmin.Application` | Xử lý use case |
| 4 | `AiAdmin.Domain` | Định nghĩa admin user |

### II.5.10.5. Thiết kế giao diện

<Insert 11 màn hình cho 11 nhóm chức năng trên>

### II.5.10.6. Thành phần giao diện (tổng quát)

| Tên control | Bắt buộc nhập | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Sidebar | Có | Menu | Điều hướng 11 tab | |
| Tenant Selector | Có | Dropdown | Chọn đơn vị đang quản trị | |
| DataTable | Có | Bảng | Hiển thị danh sách | Có lọc và phân trang |
| Form Modal | Có | Modal | Form thêm/sửa | |
| Confirm Dialog | Có | Modal | Xác nhận thao tác nguy hiểm | |
| WebSocket Indicator | Không | Icon | Trạng thái cập nhật thời gian thực | |

### II.5.10.7. Thiết kế hàm / thủ tục

Mỗi chức năng CRUD (thêm/sửa/xóa) có handler riêng; giao diện gọi API qua JWT. Cụ thể cho 11 tab được mô tả đầy đủ trong tài liệu thiết kế API riêng.

---

## II.5.11. Phân hệ Auth & Tenant cho AI

### II.5.11.1. Mô tả chức năng

**Auth & Tenant** đảm bảo **phân quyền chính xác** cho mọi thao tác AI, đồng thời **cô lập dữ liệu** giữa các đơn vị trong hệ thống multi-tenant.

**Hình dung đời thường**: Giống như hệ thống kiểm soát ra vào trong một khu chung cư. Mỗi cư dân có thẻ từ, mỗi thẻ chỉ mở được cửa căn hộ của mình. Nếu thẻ của A cố mở cửa căn hộ B → hệ thống từ chối. Đồng thời, có 4 loại thẻ: Admin (toàn quyền), Operator (quản lý vận hành), User (dùng thường), Auditor (chỉ xem báo cáo).

**Các chức năng chính:**

| Chức năng | Mô tả dời thường |
|---|---|
| Xác thực JWT | Kiểm tra "thẻ nhân viên" còn hiệu lực không |
| Phân quyền theo vai trò | Mỗi vai trò chỉ được làm những việc thuộc vai trò |
| Phân quyền theo thuộc tính | Lọc theo đơn vị, loại văn bản, độ mật |
| Bảo mật cấp dòng (RLS) | PostgreSQL tự lọc dữ liệu theo đơn vị |
| Cô lập dữ liệu | Tổng cục Thuế không thấy dữ liệu của Kho bạc |
| Phân loại độ nhạy | Tự động phân loại văn bản mật để áp dụng chính sách phù hợp |
| Quản lý khóa API | Lưu trữ an toàn, tự động xoay vòng mỗi tháng |
| Ghi nhật ký truy cập | Mọi truy cập đều được ghi lại |

### II.5.11.2. Luồng xử lý chức năng

**Mỗi khi có yêu cầu AI**, hệ thống thực hiện 7 bước kiểm tra:

| Bước | Xử lý | Mô tả đời thường |
|---|---|---|
| 1 | Lấy JWT từ header | Đọc thẻ nhân viên |
| 2 | Xác minh chữ ký JWT | Kiểm tra thẻ có phải giả không |
| 3 | Trích xuất thông tin | Lấy mã nhân viên, đơn vị, vai trò |
| 4 | Kiểm tra vai trò | Đúng vai trò AI Admin/Operator/User/Auditor |
| 5 | Kiểm tra đơn vị | User có thuộc đơn vị này không |
| 6 | Thiết lập ngữ cảnh RLS | Bật cơ chế lọc dữ liệu theo đơn vị |
| 7 | Chuyển yêu cầu đến phân hệ xử lý | Mở cửa cho user vào hệ thống |

### II.5.11.3. Tài liệu liên quan

| STT | Tên tài liệu | Mã tài liệu |
|---|---|---|
| 1 | OpenIddict | OpenIddict |
| 2 | PostgreSQL RLS | PG RLS |
| 3 | HashiCorp Vault | Vault |

### II.5.11.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `AiAuth.Api` | Middleware xác thực |
| 2 | `AiAuth.Domain` | Định nghĩa User, Role, Tenant, Permission |
| 3 | `AiAuth.Infrastructure` | Xác minh JWT, quản lý RLS |

### II.5.11.5. Thiết kế giao diện

<Insert màn hình AI Admin Console – User Permission>

### II.5.11.6. Thành phần giao diện

| Tên control | Bắt buộc nhập | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Danh sách user | Có | DataTable | Xem tất cả user | |
| Chi tiết user | Có | Form | Thông tin 1 user | |
| Tenant Selector | Có | Dropdown | Đơn vị của user | |
| Roles | Có | Multi-select | AI Admin / Operator / User / Auditor | |
| Permissions | Có | Tree | Phân quyền chi tiết | |
| API Key Management | Có | Bảng | Khóa API LLM, tự xoay vòng | |
| Giới hạn ngân sách | Không | Số thập phân | Token/tháng tối đa | |

### II.5.11.7. Thiết kế hàm / thủ tục

**Middleware xác thực AI**:

Các bước xử lý được mô tả bằng ngôn ngữ tự nhiên:

1. **Lấy JWT** từ header Authorization. Không có → trả lỗi 401.
2. **Xác minh chữ ký** JWT bằng public key của OpenIddict. Sai → trả lỗi 401.
3. **Trích xuất** user_id, tenant_id, roles, scopes từ JWT.
4. **Thiết lập RLS context** trong PostgreSQL: chỉ nhìn thấy dòng dữ liệu có tenant_id khớp.
5. **Gắn user_context** vào request để các middleware sau sử dụng.
6. **Chuyển tiếp yêu cầu** đến phân hệ xử lý.

**Chính sách phân quyền theo độ nhạy**:

1. Nếu độ nhạy "Cao" → **BẮT BUỘC** dùng AI nội bộ, không cho phép cloud.
2. Nếu đơn vị không cho phép cloud → dùng AI nội bộ.
3. Nếu ngân sách còn lại < 20% → dùng AI nội bộ (rẻ hơn).
4. Mặc định → dùng AI cloud chất lượng cao.

---

# Bảng tổng hợp 11 phân hệ AI

| STT | Mã phân hệ | Tên phân hệ | Vai trò đời thường | Cam kết chất lượng |
|---|---|---|---|---|
| 1 | II.5.1 | AI Gateway | Lễ tân tiếp nhận | Phản hồi < 100ms |
| 2 | II.5.2 | AI Engine Core | Thư ký AI trung tâm | Hỏi-đáp < 8s |
| 3 | II.5.3 | Agent Orchestration | Trợ lý đa bước | Hoàn thành < 30s |
| 4 | II.5.4 | Knowledge Base & RAG | Thư viện tri thức | Tìm kiếm < 2s |
| 5 | II.5.5 | OCR & Document Extraction | Người đánh máy scan | < 3s/trang |
| 6 | II.5.6 | Provider Abstraction | Ổ cắm điện đa năng | Chuyển tiếp < 200ms |
| 7 | II.5.7 | AI Job & Task Queue | Hệ thống xếp hàng | Hết hàng đợi < 5 phút |
| 8 | II.5.8 | MCP Server | Hộp đồ nghề cho AI | Phản hồi < 500ms |
| 9 | II.5.9 | Audit & Observability | Hộp đen máy bay | Thời gian thực |
| 10 | II.5.10 | AI Admin Console | Bảng điều khiển trung tâm | – |
| 11 | II.5.11 | Auth & Tenant | Kiểm soát ra vào | < 50ms |

---

# PHỤ LỤC — Bảng thuật ngữ (Glossary)

| Thuật ngữ | Giải thích đời thường |
|---|---|
| **AI Platform** | Hạ tầng phần mềm để phát triển và vận hành các ứng dụng AI |
| **LLM (Mô hình ngôn ngữ lớn)** | "Bộ não" của AI — chương trình đã học rất nhiều văn bản |
| **RAG (Truy xuất tăng cường)** | Kỹ thuật cho AI tìm thông tin trong kho tri thức trước khi trả lời |
| **Embedding (Vector hóa)** | Biến văn bản thành dãy số đặc trưng |
| **Vector Database** | Nơi lưu trữ dãy số đặc trưng để tìm kiếm nhanh |
| **Chunk** | Đoạn văn bản ngắn (~1 trang A4) sau khi chia nhỏ |
| **Ingestion Pipeline** | Quy trình nhập tri thức tự động vào kho |
| **Prompt** | Câu hỏi gửi cho AI |
| **Prompt Template** | Mẫu câu hỏi soạn sẵn, có phiên bản |
| **Agent** | Trợ lý AI có khả năng tự lập kế hoạch và thực hiện nhiều bước |
| **MCP** | Chuẩn giao tiếp cho AI gọi công cụ bên ngoài |
| **HITL** | Cơ chế yêu cầu con người duyệt trước thao tác nhạy cảm |
| **OCR** | Nhận dạng chữ từ ảnh/PDF scan |
| **Hallucination** | Hiện tượng AI "bịa" thông tin |
| **Ollama** | Phần mềm chạy AI mã nguồn mở trong máy chủ Bộ |
| **pgvector** | Tiện ích PostgreSQL lưu trữ và tìm kiếm vector |
| **GPU** | Chip chuyên dụng tăng tốc tính toán AI |
| **PII** | Thông tin cá nhân nhạy cảm (CCCD, SĐT, email) |
| **DMS** | Hệ thống quản lý văn bản hiện có |
| **Audit Log** | Nhật ký kiểm tra |
| **RAGAS** | Bộ công cụ đánh giá chất lượng hỏi-đáp AI |

---

**HẾT TÀI LIỆU THIẾT KẾ CHỨC NĂNG — MODULE AI PHỤC VỤ PHẦN MỀM VĂN PHÒNG SỐ BỘ TÀI CHÍNH**