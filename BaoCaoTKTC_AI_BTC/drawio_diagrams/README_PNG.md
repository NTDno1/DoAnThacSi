# 7 Sơ đồ Module AI - Module AI Bộ Tài Chính

> 7 file PNG chất lượng cao, dùng được ngay (kéo thả vào Word, slide, README, ...)

---

## 1. Kiến trúc tổng thể 7 tầng

![01 - Kiến trúc tổng thể 7 tầng](png_diagrams/01_architecture_tong_the_7_tang.png)

> **Mô tả**: Toàn bộ Phần mềm Văn phòng số Bộ Tài Chính (bao gồm cả Module AI) được tổ chức thành 7 tầng, từ Trình bày (User) → API Gateway → BFF → Microservice nghiệp vụ → **Microservice AI (TẦNG 5 - TÂM ĐIỂM)** → Sự kiện & Bộ đệm → Dữ liệu.

## 2. Quan hệ 11 phân hệ AI

![02 - Quan hệ 11 phân hệ](png_diagrams/02_quan_he_11_phan_he.png)

> **Mô tả**: 11 phân hệ AI và cách chúng phối hợp. Phân hệ ① AI Gateway là cổng tiếp nhận duy nhất; ② Engine Core xử lý yêu cầu đơn giản; ③ Agent xử lý đa bước; lớp nền tảng (Auth, Audit, Admin) chạy ngầm.

## 3. Luồng 1 yêu cầu AI (sequence)

![03 - Luồng 1 yêu cầu AI](png_diagrams/03_luong_1_yeu_cau_ai.png)

> **Mô tả**: 11 bước từ lúc User gửi câu hỏi → qua 5 microservice → nhận câu trả lời streaming.

## 4. Hạ tầng 7 vùng mạng

![04 - Hạ tầng 7 vùng mạng](png_diagrams/04_ha_tang_7_vung_mang.png)

> **Mô tả**: Internet → Firewall → Load Balancer → Kong Gateway → Firewall Core → K8s Cluster (CPU) + GPU Cluster → Kafka/Redis → PostgreSQL/Vector DB/MinIO.

## 5. Pipeline RAG chi tiết

![05 - Pipeline RAG](png_diagrams/05_pipeline_rag_chi_tiet.png)

> **Mô tả**: 5 bước xử lý 1 câu hỏi AI: Embed → Hybrid Search (BM25 + Cosine) → Rerank (BGE) → LLM (GPT-4o-mini hoặc Ollama) → Cite + Quality.

## 6. Clean Architecture 4 lớp

![06 - Clean Architecture](png_diagrams/06_clean_architecture.png)

> **Mô tả**: Mỗi phân hệ AI được tổ chức theo 4 lớp: API → Infrastructure → Application → Domain (lõi nghiệp vụ).

## 7. Bảo mật 7 lớp cho AI

![07 - Bảo mật 7 lớp](png_diagrams/07_bao_mat_7_lop_ai.png)

> **Mô tả**: 7 lớp bảo mật từ ngoài vào trong: Firewall → API Gateway → JWT → Encryption → Data Classification → IDS/IPS → Audit Log.

---

## Cách sử dụng

| Mục đích | Cách dùng |
|---|---|
| **Trong tài liệu Word** | Insert → Picture → chọn file PNG |
| **Trong slide PowerPoint** | Insert → Picture → chọn file PNG |
| **Trong file Markdown** | Dùng cú pháp `![alt](path)` |
| **Trong trang web** | Thẻ `<img src="path">` |
| **In ra giấy A3** | Chất lượng đủ rõ (300 DPI equivalent) |

## Thông số kỹ thuật

- Độ phân giải: 150 DPI
- Tỷ lệ: trang ngang (landscape), từ 20×11 đến 24×14 inches
- Định dạng: PNG với nền trắng
- Font: Segoe UI (Windows native) — hiển thị đầy đủ tiếng Việt
- Tổng dung lượng: ~1.3 MB cho cả 7 file

## Có thể chỉnh sửa?

Có. Nếu bạn muốn điều chỉnh (thêm/bớt thành phần, đổi màu, dịch sang tiếng Anh, ...), tôi có thể:
1. Chỉnh sửa file `generate_png.py` rồi chạy lại
2. Hoặc mở file PNG và dùng Paint/PowerPoint để chỉnh tay
3. Hoặc dùng file `.drawio` cũ (trong cùng folder) — mở bằng draw.io desktop, import thành công 100%