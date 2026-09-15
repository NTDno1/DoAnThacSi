# Báo cáo đồ án thạc sĩ — Nguyễn Tiến Đạt

Nguồn Markdown, đầu ra và script hỗ trợ đã được tách riêng để tránh chỉnh nhầm file sinh tự động.

Thu muc chua bao cao do an thac si **"Nghien cuu va xay dung nen tang AI doanh nghiep huong tich hop da he thong (Enterprise AI Platform for Cross-System Integration)"** cua hoc vien **Nguyen Tien Dat**, Hoc vien Cong nghe Buu chinh Vien thong, 2026.

## Cau truc thu muc

- `chapters/`: các phần Markdown nguồn.
- `outputs/`: bản Markdown tổng hợp và DOCX.
- `tools/`: script ghép và chuyển đổi do công cụ/AI hỗ trợ tạo.

## Hai dinh dang san pham

### 1. Markdown (`.md`)

- **File**: `outputs/BaoCaoDeAn_ThacSi_NguyenTienDat_Full.md`
- **Kich thuoc**: ~146 KB (~125K ky tu)
- **Su dung**: doc tot trong GitHub, GitLab, cac trinh editor Markdown (VS Code, Typora, Obsidian...)
- **Cau truc**: gom 13 file thanh phan noi bo + 1 file Full

### 2. Microsoft Word (`.docx`)

- **File**: `outputs/BaoCaoDeAn_ThacSi_NguyenTienDat_Full.docx`
- **Kich thuoc**: ~104 KB
- **So doan van**: 1067 paragraphs
- **So bang**: 26 tables
- **Dinh dang**: Times New Roman, 13pt, le 2.5cm, justify, line spacing 1.5

## Cach su dung

### Xem truc tiep

Mở file trong `outputs/` bằng trình đọc Markdown hoặc Microsoft Word.

### Tao lai cac file tu dau

```bash
# Buoc 1: Chay merge (neu sua cac file thanh phan)
python reports/01.ThesisMaster_DeAn/tools/merge_report.py

# Buoc 2: Convert sang docx
python reports/01.ThesisMaster_DeAn/tools/convert_to_docx.py
```

Yeu cau Python 3.7+ va goi `python-docx`:

```bash
pip install python-docx
```

## Thong tin do an

| Thong tin | Chi tiet |
|---|---|
| De tai | Nghien cuu va xay dung nen tang AI doanh nghiep huong tich hop da he thong |
| Hoc vien | Nguyen Tien Dat |
| Chuyen nganh | He thong Thong tin |
| Ma so | 8.48.01.04 |
| Nguoi huong dan | PGS.TS. Tran Dinh Que |
| Co so dao tao | Hoc vien Cong nghe Buu chinh Vien thong |
| Nam | 2026 |

## Tom tat noi dung bao cao

Bao cao trinh bay mot kien truc tham chieu toan dien cho nen tang AI doanh nghiep (Enterprise AI Platform) voi cac diem noi bat:

- **Kien truc 7 tang**: Presentation, AI Gateway, Agent Orchestration, AI Engine Core, Provider Abstraction, Integration, Platform Core.
- **Provider Abstraction**: ho tro 4+ LLM provider (OpenAI, Claude, Gemini, Ollama local) qua interface chuan.
- **RAG Engine**: Hybrid Search (BM25 + Vector, RRF) + Cross-Encoder Reranker + Citation.
- **AI Agent**: Multi-Agent Orchestration voi cac pattern (Sequential, Supervisor, Debate).
- **MCP (Model Context Protocol)**: first-class citizen cho tich hop ERP, CRM, DMS, HRM.
- **Multi-tenant**: tenant isolation qua Row-Level Security + RBAC + ABAC.
- **5 Phase trien khai**: Walking Skeleton, moi Phase co san pham chay duoc.

### Ket qua thuc nghiem

- RAGAS diem trung binh: **0.89/1.0** (Gemini 2.5 Flash)
- Ty le tich hop thanh cong: **90%**
- Latency P95: **2.8s** (RAG), **8.5s** (multi-agent)
- Chi phi: **~$0.15 / 1000 query**
- Cai thien **30 lan** thoi gian tich hop so voi cach truyen thong.
