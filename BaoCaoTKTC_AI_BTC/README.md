# Báo cáo Thiết kế chức năng — Hệ thống AI phục vụ Văn phòng số Bộ Tài Chính

## 📋 Mục đích

Đây là bản `.md` của **Tài liệu Thiết kế chức năng** phân hệ AI thuộc **Dự án Văn phòng số Bộ Tài Chính**, viết theo **đúng template** mẫu của Bộ (xem mục [Mẫu tham chiếu](#-mẫu-tham-chiếu) bên dưới). Tài liệu được soạn để **báo cáo lên Bộ Tài Chính**.

## 🗂️ Cấu trúc thư mục

```
BaoCaoTKTC_AI_BTC/
├── 00_BIA_VA_LOI_NOI_DAU.md            ← Trang bìa + lời nói đầu + mục lục
├── 01_PHAN_I_TONG_QUAN.md              ← Phần I. TỔNG QUAN (Mục đích, phạm vi, tài liệu, thuật ngữ)
├── 02_PHAN_II_KIEN_TRUC.md             ← Phần II. MÔ HÌNH KIẾN TRÚC (mục II.1 - II.4)
├── 03_PHAN_II_5_THIET_KE_CHI_TIET.md   ← THIẾT KẾ CHI TIẾT 11 phân hệ AI (II.5.1 → II.5.11)
├── 04_TAI_LIEU_RIENG_AI.md             ⭐ TÀI LIỆU RIÊNG VỀ AI (đánh mã [AI-X.Y.Z])
├── 04_TAI_LIEU_RIENG_AI.docx           ⭐ FILE .DOCX RIÊNG VỀ AI (paste vào tài liệu chính)
├── BaoCaoTKTC_AI_BTC_Full.md           ← FILE MD ĐẦY ĐỦ (tài liệu chính)
├── BaoCaoTKTC_AI_BTC_Full.docx         ← FILE DOCX ĐẦY ĐỦ (tài liệu chính)
├── merge_report.py                     ← Script gộp 4 file thành phần
├── convert_md_to_docx.py               ← Script tạo file .docx chính
└── convert_ai_only_to_docx.py          ← Script tạo file .docx riêng AI
```

## 🚀 Cách dùng

### File chính (BaoCaoTKTC_AI_BTC_Full.docx)
- Tài liệu TKTC đầy đủ theo mẫu Bộ Tài Chính, đã bao gồm tất cả nội dung về AI.
- Dùng để báo cáo chính thức.

### File riêng (04_TAI_LIEU_RIENG_AI.docx) — dùng khi cần BỔ SUNG CHI TIẾT về AI
- Tài liệu **chuyên đề AI** riêng biệt, gồm 3 phần lớn:
  - **PHẦN A** — Yêu cầu AI & Bài toán giải quyết được (FR-AI-01 → FR-AI-20, NFR-AI-01 → NFR-AI-18, BT1 → BT6)
  - **PHẦN B** — Thiết kế hệ thống AI (LLM, RAG pipeline, Agent, MCP, OCR, Provider)
  - **PHẦN C** — Hạ tầng cần thiết (GPU, vector DB, semantic cache, worker pool, K8s, security, monitoring, cost, roadmap)
- Mỗi đoạn được đánh mã `[AI-X.Y.Z]` để bạn biết chính xác **paste vào vị trí nào** trong file chính.
- Cuối file có **BẢNG HƯỚNG DẪN PASTE** chi tiết từng đoạn.

### Hướng dẫn paste nhanh

1. Mở 2 file cạnh nhau:
   - `BaoCaoTKTC_AI_BTC_Full.docx` (trong Word)
   - `04_TAI_LIEU_RIENG_AI.docx` (trong Word)

2. Trong file riêng AI, tìm mã `[AI-X.Y.Z]` ở đầu mỗi đoạn (Ctrl+F).

3. Tra **BẢNG HƯỚNG DẪN PASTE** ở cuối file AI → biết paste vào mục nào trong file chính.

4. Trong file chính, di chuyển đến đúng vị trí → Ctrl+V.

### Ví dụ một số vị trí paste quan trọng:

| Mã | Nội dung | Paste vào mục |
|---|---|---|
| `[AI-A.2.1]` | BT1 - Tóm tắt văn bản | **II.5.2 AI Engine Core** |
| `[AI-B.2.2]` | RAG Pipeline chi tiết | **II.5.2 + II.5.4** |
| `[AI-B.2.3]` | AI Agent multi-step | **II.5.3 Agent Orchestration** |
| `[AI-B.2.4]` | MCP Server chi tiết | **II.5.8 MCP Server** |
| `[AI-C.2.1]` | GPU Node | **II.1.8 Hạ tầng** |
| `[AI-C.3.2]` | Chi phí & ROI | **II.4 Triển khai** |
| `[AI-C.3.3]` | Lộ trình | **II.4 Triển khai** |

### 1. Đã có sẵn file Full.md

Sau khi chạy `merge_report.py`, file `BaoCaoTKTC_AI_BTC_Full.md` được sinh ra với **~97.000 ký tự** chứa toàn bộ nội dung báo cáo.

### 2. Copy nội dung sang file Word

Làm theo 1 trong 2 cách:

**Cách A – Paste trực tiếp (khuyến nghị):**
1. Mở file `BaoCaoTKTC_AI_BTC_Full.md` bằng VSCode / Notepad++.
2. `Ctrl + A` → `Ctrl + C` để copy toàn bộ.
3. Mở file `.doc` mới trong Word (đặt font: **Times New Roman 13pt, A4, lề 2.5cm, line-spacing 1.5**).
4. `Ctrl + V` để paste.
5. Word sẽ tự render phần lớn heading, bảng và code block.

**Cách B – Dùng công cụ chuyển đổi tự động:**

Dùng **Pandoc** (mã nguồn mở) để chuyển trực tiếp `.md → .docx`:

```bash
# Cài đặt pandoc một lần:
# https://pandoc.org/installing.html

pandoc BaoCaoTKTC_AI_BTC_Full.md -o BaoCaoTKTC_AI_BTC_Full.docx \
  --reference-doc=template.docx      # (tùy chọn) dùng template Word có sẵn
```

Hoặc dùng extension VSCode `Markdown Preview Enhanced` để export.

### 3. Cập nhật thông tin nhà thầu (BẮT BUỘC trước khi gửi)

Mở file `00_BIA_VA_LOI_NOI_DAU.md`, thay các placeholder:

| Placeholder | Cần thay bằng |
|---|---|
| `Hợp đồng số:` `................` | Số hợp đồng thực tế |
| `Gói thầu` `.................` | Mã gói thầu |
| `<CHỨC DANH>`, `<Họ và tên>` | Điền tên người ký từng bên |

## 📝 Nội dung chính theo mẫu Bộ

Tài liệu được viết đúng theo template gốc của Bộ Tài Chính, gồm:

### Phần I — TỔNG QUAN
- **I.1** Mục đích
- **I.2** Phạm vi hệ thống (chia nhỏ: phạm vi nghiệp vụ, tổ chức, người dùng ~60.000 user, dữ liệu, kỹ thuật, nguyên tắc kế thừa)
- **I.3** Tài liệu liên quan (18 nguồn)
- **I.4** Thuật ngữ và các từ viết tắt (54 thuật ngữ)

### Phần II — MÔ HÌNH KIẾN TRÚC
- **II.1** Mô hình kiến trúc hệ thống (mục 1.0 đến 1.11)
  - 1.1 Mục tiêu kiến trúc
  - 1.2 Nguyên tắc thiết kế (15 nguyên tắc)
  - 1.3 Kiến trúc tổng thể (7 tầng)
  - 1.4 Kiến trúc ứng dụng (Clean Architecture + Vertical Slice)
  - 1.5 Kiến trúc dịch vụ (microservices)
  - 1.6 Kiến trúc tích hợp
  - 1.7 Kiến trúc dữ liệu (multi-schema PostgreSQL)
  - 1.8 Kiến trúc hạ tầng và triển khai
  - 1.9 Kiến trúc an toàn bảo mật (Zero-Trust, 4 lớp)
  - 1.10 Giám sát và vận hành
  - 1.11 Khả năng mở rộng
- **II.2** Mô hình phân rã chức năng (11 phân hệ AI)
- **II.3** Mô hình tích hợp hệ thống
- **II.4** Mô hình triển khai hệ thống

### Phần THIẾT KẾ CHI TIẾT CHỨC NĂNG
11 phân hệ AI, mỗi phân hệ gồm **đầy đủ 7 mục**:

| # | Phân hệ | Nội dung |
|---|---|---|
| II.5.1 | AI Gateway | Rate-limit, sanitization, provider routing, cache |
| II.5.2 | AI Engine Core | RAG pipeline (embed→retrieve→rerank→generate→validate) |
| II.5.3 | Agent Orchestration | Multi-agent, MCP, HITL, Safety |
| II.5.4 | Knowledge Base & RAG | Ingestion + Hybrid Search + Reranker |
| II.5.5 | OCR & Document Extraction | Tesseract/PaddleOCR/Donut, schema mapping |
| II.5.6 | Provider Abstraction (LLM) | OpenAI / Ollama / Anthropic / Azure |
| II.5.7 | AI Job & Task Queue | Kafka + Outbox + Retry/DLQ |
| II.5.8 | MCP Server | Model Context Protocol cho agent |
| II.5.9 | Audit & Observability | Log, Metrics, Traces |
| II.5.10 | AI Admin Console | UI cho admin Bộ |
| II.5.11 | Auth & Tenant cho AI | RBAC + ABAC + Row-Level Security |

Mỗi phân hệ đều có:
- ✅ Mô tả chức năng
- ✅ Sơ đồ luồng xử lý + bảng các bước
- ✅ Tài liệu liên quan
- ✅ Tên module (mapping code `qlvb-backend-core` thật)
- ✅ Thiết kế giao diện (Admin Console)
- ✅ Bảng thành phần giao diện (control)
- ✅ Pseudo-code hàm/thủ tục

## 📂 Nguồn tham chiếu được sử dụng

| Tài liệu | Vai trò |
|---|---|
| `C:\Users\datt\Documents\GitProject\qlvb-documents` (toàn bộ) | Tài liệu dự án Bộ Tài Chính |
| `C:\Users\datt\Documents\GitProject\qlvb-backend-core\README.md` | Kiến trúc tổng thể backend |
| `qlvb-backend-core/docs/database/*` | Schema các module hiện hữu |
| `qlvb-backend-core/docs/modules/Identity/*` | Luồng auth, login, MFA |
| `qlvb-backend-core/docs/modules/Files/*` | Upload file, ký số |
| `qlvb-backend-core/docs/Entity-Domain-And-OutboxEvent.md` | Outbox pattern |
| `qlvb-backend-core/docs/Apply Vertical Slice in Clean Architecture .NET API.md` | Vertical Slice |
| `qlvb-documents/Core/.../1. Nghiên cứu/AI_README.md` | AI Worker Design (DMS) |
| `qlvb-documents/Core/.../2. Kiến trúc/*.png` | Sơ đồ Microservice BTC, Hạ tầng |
| `qlvb-documents/Core/.../3. Tiêu chuẩn/...URD_v1.0.docx` | URD – Yêu cầu người dùng |
| Mẫu TÀI LIỆU THIẾT KẾ CHỨC NĂNG (Bộ Tài Chính / Mobifone Solutions) | Template biên soạn |

## 🔄 Cập nhật nội dung

Sau khi chỉnh sửa 1 trong 4 file thành phần, chạy lại:

```powershell
python merge_report.py
```

để cập nhật `BaoCaoTKTC_AI_BTC_Full.md`.

## 📌 Mẫu tham chiếu

Mẫu báo cáo gốc (Bộ Tài Chính / Mobifone Solutions):

- Tên file gốc trong tài liệu do user cung cấp (file Word trong `qlvb-documents`).
- Phong cách: trang bìa có 5 ô ký (Chủ đầu tư, Tư vấn thiết kế, Tư vấn giám sát, Kiểm thử, Nhà thầu) + bảng ghi nhận thay đổi tài liệu.
- Cấu trúc 2 phần: TỔNG QUAN + MÔ HÌNH KIẾN TRÚC (gồm 11 mục con) + THIẾT KẾ CHI TIẾT.

---

**Tác giả/Đơn vị biên soạn:** Mobifone Solutions  
**Mục đích sử dụng:** Báo cáo chính thức dự án Văn phòng số Bộ Tài Chính  
**Phiên bản:** 1.0  
**Ngày:** 07/2026
