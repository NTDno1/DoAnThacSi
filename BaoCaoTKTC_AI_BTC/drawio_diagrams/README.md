# HƯỚNG DẪN IMPORT VÀO DRAW.IO

Folder này chứa **7 file drawio** sinh từ `AI_Platform_TKTC.md` (do @279-391 của file MD).

## Cách import

### Cách 1 — Import vào draw.io web (đơn giản nhất)
1. Truy cập https://app.diagrams.net (hoặc https://draw.io)
2. Chọn **File → Open from Device** → chọn file `.drawio`
3. Sơ đồ sẽ hiển thị với shapes, edges, và text đầy đủ
4. Bạn có thể tùy chỉnh thêm (màu sắc, font, kích thước) rồi **Export as PNG/SVG/PDF**

### Cách 2 — Kéo thả
- Kéo thả file `.drawio` từ File Explorer vào cửa sổ draw.io

### Cách 3 — VS Code extension
- Cài extension **Draw.io Integration** → click vào file `.drawio` để mở

---

## Danh sách 7 sơ đồ

| File | Mô tả | Tương ứng mục trong MD |
|---|---|---|
| `01_architecture_tong_the_7_tang.drawio` | Kiến trúc tổng thể 7 tầng của hệ thống Văn phòng số | §II.1.0 |
| `02_quan_he_11_phan_he.drawio` | Quan hệ giữa 11 phân hệ module AI | §II.5 |
| `03_luong_1_yeu_cau_ai.drawio` | Sequence diagram 11 bước xử lý 1 yêu cầu AI | §II.5 |
| `04_ha_tang_7_vung_mang.drawio` | Hạ tầng 7 vùng mạng (Internet → DB) | §II.4 |
| `05_pipeline_rag_chi_tiet.drawio` | Pipeline RAG 5 bước (Embed → Hybrid → Rerank → LLM → Cite) | §II.5.2 |
| `06_clean_architecture.drawio` | Clean Architecture 4 lớp trong mỗi phân hệ AI | §II.1.4 |
| `07_bao_mat_7_lop_ai.drawio` | Bảo mật 7 lớp cho AI | §II.1.9 |

---

## Đặc điểm kỹ thuật của mỗi file

- **Định dạng**: XML chuẩn draw.io (mxfile/mxGraphModel)
- **Host**: `app.diagrams.net` (draw.io official)
- **Encoding**: UTF-8 — hỗ trợ đầy đủ tiếng Việt có dấu
- **Số shapes**: từ 20-60 shapes / file
- **Số edges (mũi tên)**: 10-30 edges / file
- **Page size**: A4 landscape (1654×1169 hoặc 2339×1169)

---

## Gợi ý sử dụng

1. **Cho báo cáo**: Export thành PNG/PDF, embed vào tài liệu Word.
2. **Cho slide thuyết trình**: Export PNG nền trong suốt.
3. **Cho tài liệu thiết kế**: Giữ nguyên drawio, share link edit.
4. **Để chỉnh sửa tiếp**: Mở file, chỉnh sửa trực tiếp.

Nếu muốn **đổi tên**, **dịch sang tiếng Anh**, **thêm công thức tính toán**, hoặc **thêm sơ đồ mới**, cứ yêu cầu tiếp.
