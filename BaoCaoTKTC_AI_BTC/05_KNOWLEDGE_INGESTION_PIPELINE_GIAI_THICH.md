# KNOWLEDGE INGESTION PIPELINE — Giải thích chi tiết

## 📚 Nó là gì?

**Knowledge Ingestion Pipeline** (Quy trình Nhập Tri Thức) là **một chuỗi các bước tự động** để:

1. **Lấy dữ liệu** từ các nguồn khác nhau (văn bản DMS, file PDF, web, database, ...)
2. **Làm sạch** dữ liệu (loại bỏ HTML, header/footer, lỗi chính tả, ...)
3. **Chia nhỏ** văn bản dài thành các đoạn ngắn (chunk)
4. **Biến đổi** mỗi đoạn thành **vector embedding** (dãy số đặc trưng)
5. **Lưu trữ** vector + metadata vào **Vector Database** (pgvector / Qdrant)
6. **Đánh index** để có thể tìm kiếm nhanh theo ngữ nghĩa

> 💡 **Ví dụ đời thường:** Tưởng tượng bạn có 1 triệu văn bản của Bộ Tài Chính. Bạn muốn khi lãnh đạo hỏi *"Cho tôi biết các văn bản về cải cách tiền lương quý 3/2026"*, hệ thống tìm được ngay trong < 1 giây. Để làm được vậy, trước đó toàn bộ văn bản phải được **"nhập" vào hệ thống AI** theo một quy trình chuẩn — đó chính là **Knowledge Ingestion Pipeline**.

---

## 🎯 Dùng để làm gì trong dự án Bộ Tài Chính?

### Mục đích chính

| # | Mục đích | Giải thích | Use case cụ thể |
|---|---|---|---|
| 1 | **Tìm kiếm ngữ nghĩa** | Tìm văn bản theo **ý nghĩa**, không chỉ từ khóa | Lãnh đạo hỏi *"văn bản về chính sách thuế VAT"* → trả về tất cả văn bản liên quan, kể cả văn bản không chứa chính xác từ "VAT" |
| 2 | **Hỏi-đáp RAG** | Cung cấp context cho LLM sinh câu trả lời có trích dẫn | User hỏi *"Quy trình phê duyệt văn bản mật thế nào?"* → LLM lấy context từ tri thức → trả lời + citation |
| 3 | **ChatOps cho lãnh đạo** | Lãnh đạo hỏi trực tiếp bằng tiếng Việt tự nhiên | *"Văn bản nào của Tổng cục Thuế ban hành trong tháng 7/2026?"* |
| 4 | **Phân loại & gợi ý** | AI gợi ý lĩnh vực, độ mật, người xử lý | Khi upload văn bản mới, AI gợi ý metadata đã có trong kho |
| 5 | **Tìm văn bản tương tự** | Tìm văn bản tương tự văn bản đang xem | Khi mở 1 văn bản, gợi ý 10 văn bản tương tự để tham khảo |
| 6 | **Khử trùng lặp** | Phát hiện văn bản trùng nội dung | Tránh lưu trữ văn bản trùng nhau |
| 7 | **Hỗ trợ Agent** | Agent tra cứu tri thức để trả lời câu hỏi phức tạp | Agent multi-step tự động tìm văn bản → phân tích → sinh báo cáo |

### Đối tượng sử dụng

| Nhóm | Cách dùng |
|---|---|
| **Lãnh đạo Bộ / Đơn vị** | Hỏi trực tiếp qua ChatOps, nhận câu trả lời tổng hợp |
| **Chuyên viên, văn thư** | Tìm kiếm nhanh văn bản, gợi ý văn bản tương tự |
| **Quản trị hệ thống** | Cấu hình nguồn tri thức, theo dõi trạng thái ingestion |
| **AI Agent** | Tự động truy xuất tri thức phục vụ tác vụ đa bước |

---

## 🏗️ Kiến trúc Knowledge Ingestion Pipeline

### Sơ đồ tổng quan

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                  KNOWLEDGE INGESTION PIPELINE                               │
│                                                                             │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │  STEP 1: SOURCE CONNECTORS (Kết nối nguồn)                          │  │
│  │  ──────────────────────────────────────────────                        │  │
│  │  • DMS API ← văn bản Văn phòng số                                    │  │
│  │  • File Upload ← PDF, DOCX, XLSX người dùng upload                   │  │
│  │  • Web Scraper ← website nội bộ, cổng thông tin                       │  │
│  │  • Database ← HRM, ERP, Kho bạc                                       │  │
│  │  • SharePoint ← tài liệu dự án                                       │  │
│  │  • Email ← Công văn đến qua email                                     │  │
│  └────────────────┬─────────────────────────────────────────────────────┘  │
│                   ▼                                                          │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │  STEP 2: EXTRACT (Trích xuất text)                                   │  │
│  │  ──────────────────────────────────────                                │  │
│  │  • PDF text → parser (PyPDF2, pdfplumber)                              │  │
│  │  • PDF scan → OCR (Tesseract/PaddleOCR)                                │  │
│  │  • DOCX → python-docx                                                 │  │
│  │  • HTML → BeautifulSoup (loại bỏ tag, giữ lại text)                  │  │
│  │  • XLSX → openpyxl (từng sheet)                                       │  │
│  │  • Image → OCR (nếu có text)                                          │  │
│  │  Output: { text thuần, metadata (tác giả, ngày, ...), trang }       │  │
│  └────────────────┬─────────────────────────────────────────────────────┘  │
│                   ▼                                                          │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │  STEP 3: CLEAN & NORMALIZE (Làm sạch)                                │  │
│  │  ────────────────────────────────────────                              │  │
│  │  • Loại bỏ HTML tags, CSS, JS                                        │  │
│  │  • Loại bỏ header/footer lặp lại (số trang, ngày in, ...)             │  │
│  │  • Chuẩn hóa khoảng trắng, dấu tiếng Việt                           │  │
│  │  • Sửa lỗi OCR (nếu có)                                              │  │
│  │  • Loại bỏ dữ liệu nhạy cảm (PII detection)                          │  │
│  │  • Chuẩn hóa ngày tháng, số văn bản                                   │  │
│  │  Output: text sạch, chuẩn hóa                                         │  │
│  └────────────────┬─────────────────────────────────────────────────────┘  │
│                   ▼                                                          │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │  STEP 4: CHUNK (Chia nhỏ)                                            │  │
│  │  ────────────────────────────                                          │  │
│  │  • Recursive Chunking (mặc định)                                      │  │
│  │    - Chia theo \n\n → \n → câu → từ                                 │  │
│  │    - chunk_size: 512 tokens, overlap: 50 tokens                       │  │
│  │  • Semantic Chunking                                                   │  │
│  │    - Chia theo đoạn văn có cùng chủ đề                                │  │
│  │  • Layout-aware Chunking                                               │  │
│  │    - Chia theo header/section của văn bản                             │  │
│  │  • Sliding Window Chunking                                            │  │
│  │    - Trượt cửa sổ với overlap cố định                                │  │
│  │  Output: danh sách chunks (mỗi chunk ~200-500 tokens)                │  │
│  └────────────────┬─────────────────────────────────────────────────────┘  │
│                   ▼                                                          │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │  STEP 5: METADATA EXTRACTION (Trích metadata)                        │  │
│  │  ──────────────────────────────────────────────                        │  │
│  │  • Tác giả / Cơ quan ban hành                                         │  │
│  │  • Ngày ban hành / Ngày hiệu lực                                     │  │
│  │  • Số ký hiệu văn bản                                                 │  │
│  │  • Lĩnh vực, loại văn bản, độ mật, độ khẩn                          │  │
│  │  • Người ký, chức vụ                                                  │  │
│  │  • Trích yếu (1-2 dòng)                                               │  │
│  │  Output: { content, metadata }                                        │  │
│  └────────────────┬─────────────────────────────────────────────────────┘  │
│                   ▼                                                          │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │  STEP 6: EMBEDDING (Vector hóa)                                      │  │
│  │  ──────────────────────────────                                        │  │
│  │  • Mỗi chunk → gọi Embedding Model                                    │  │
│  │    - text-embedding-3-small (OpenAI) → 1536 dimensions                │  │
│  │    - nomic-embed-text (Ollama local) → 768 dimensions                │  │
│  │    - bge-m3 (Ollama) → 1024 dimensions                               │  │
│  │  • Batch processing (100 chunks / lần) → tiết kiệm chi phí          │  │
│  │  • Caching (Redis) cho nội dung đã embedding rồi                      │  │
│  │  Output: vector float[1536] cho mỗi chunk                            │  │
│  └────────────────┬─────────────────────────────────────────────────────┘  │
│                   ▼                                                          │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │  STEP 7: INDEXING (Lập chỉ mục)                                      │  │
│  │  ────────────────────────────────                                      │  │
│  │  • Lưu vào PostgreSQL + pgvector                                    │  │
│  │    - Table: ai_platform.knowledge_chunk                              │  │
│  │    - Index: ivfflat (cosine similarity)                              │  │
│  │    - Index: GIN (full-text search tiếng Việt)                        │  │
│  │  • Hoặc Qdrant cluster (khi scale lớn)                                │  │
│  │  • Row-Level Security theo tenant_id                                  │  │
│  │  Output: chunks đã được index, sẵn sàng truy vấn                     │  │
│  └────────────────┬─────────────────────────────────────────────────────┘  │
│                   ▼                                                          │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │  STEP 8: INGESTION LOG (Ghi log)                                     │  │
│  │  ──────────────────────────────                                        │  │
│  │  • Ghi vào bảng ai_platform.knowledge_ingestion_log                   │  │
│  │  • Trạng thái: pending → running → done / failed                     │  │
│  │  • Số chunk, thời gian, lỗi (nếu có)                                │  │
│  │  • Trigger notification cho admin                                      │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 🔄 Luồng xử lý chi tiết từng bước

### STEP 1: Source Connectors

**Mục đích:** Kết nối và lấy dữ liệu từ nguồn.

| Nguồn | Cơ chế kết nối | Use case |
|---|---|---|
| **DMS (Văn phòng số)** | DMS API + Kafka | Văn bản đến/đi/nội bộ hiện có (2020-2026) |
| **File Upload** | Multipart upload → MinIO/S3 | User upload PDF/DOCX |
| **Web Scraper** | Crawler định kỳ | Cổng thông tin điện tử Bộ, web nội bộ |
| **Database** | ETL job | HRM, ERP, Kho bạc |
| **SharePoint** | Graph API | Tài liệu dự án, slide, báo cáo |
| **Email** | IMAP/Graph API | Công văn đến qua email |

### STEP 2: Extract (Trích xuất text)

**Mục đích:** Chuyển mọi định dạng về text thuần.

```python
# Ví dụ: trích text từ PDF scan
def extract(file_path: str) -> ExtractedDocument:
    if file_path.endswith('.pdf'):
        # Thử text layer trước
        text = pdfplumber.extract_text(file_path)
        if not text or len(text.strip()) < 100:
            # PDF scan → OCR
            text = paddle_ocr.extract(file_path)
        return ExtractedDocument(text=text, source='pdf')
    
    elif file_path.endswith('.docx'):
        return ExtractedDocument(
            text=docx2txt.extract(file_path),
            source='docx'
        )
    
    elif file_path.endswith('.html'):
        return ExtractedDocument(
            text=bs4(html_content, 'html.parser').get_text(),
            source='html'
        )
```

### STEP 3: Clean & Normalize

**Mục đích:** Loại bỏ nhiễu, chuẩn hóa.

```python
def clean(text: str) -> str:
    # Loại bỏ URL, email quảng cáo
    text = re.sub(r'http\S+|www\.\S+', '', text)
    
    # Chuẩn hóa khoảng trắng
    text = re.sub(r'\s+', ' ', text).strip()
    
    # Loại bỏ header/footer lặp lại
    text = remove_repeated_lines(text)
    
    # Chuẩn hóa dấu tiếng Việt
    text = unicodedata.normalize('NFC', text)
    
    # Loại bỏ PII (căn cước, số điện thoại, email)
    text = pii_redactor.redact(text)
    
    return text
```

### STEP 4: Chunking (Chia nhỏ)

**Mục đích:** Văn bản dài → nhiều đoạn ngắn (chunk) để embedding hiệu quả.

**Tại sao phải chunk?**
- LLM có giới hạn **context window** (vd: gpt-4o-mini = 128k tokens, nhưng embedding model chỉ hiểu tốt ≤ 512 tokens).
- Vector search chính xác hơn khi chunk ngắn, tập trung vào 1 chủ đề.
- Giảm chi phí embedding (embed cả văn bản 100 trang tốn hơn 200 chunks 500 tokens).

```python
# Recursive Chunking (mặc định)
def chunk_recursive(text: str, chunk_size=512, overlap=50) -> List[Chunk]:
    """Chia văn bản theo recursive separator strategy."""
    separators = ["\n\n", "\n", ". ", "? ", "! ", " "]  # ưu tiên
    chunks = []
    
    # Thử tách theo separator lớn nhất trước
    for separator in separators:
        if can_split(text, separator, chunk_size):
            parts = text.split(separator)
            # Ghép các parts lại sao cho mỗi chunk ≤ chunk_size
            current_chunk = ""
            for part in parts:
                if len(current_chunk) + len(part) <= chunk_size:
                    current_chunk += separator + part
                else:
                    if current_chunk:
                        chunks.append(current_chunk)
                    current_chunk = part
            if current_chunk:
                chunks.append(current_chunk)
            break
    
    # Thêm overlap giữa các chunks
    chunks_with_overlap = add_overlap(chunks, overlap=overlap)
    return chunks_with_overlap
```

**So sánh các chiến lược chunking:**

| Chiến lược | Ưu điểm | Nhược điểm | Dùng khi |
|---|---|---|---|
| **Recursive** | Đơn giản, nhanh | Có thể cắt giữa câu | Mặc định cho hầu hết văn bản |
| **Semantic** | Chia theo chủ đề → chunk chất lượng | Chậm hơn (cần embed mỗi câu) | Văn bản có cấu trúc chủ đề rõ |
| **Layout-aware** | Giữ được cấu trúc văn bản | Cần parser phức tạp | Văn bản có header/section rõ ràng |
| **Sliding Window** | Không mất context giữa chunks | Trùng lặp nhiều, tốn storage | Văn bản cần context dài |

### STEP 5: Metadata Extraction

**Mục đích:** Trích thông tin có cấu trúc từ văn bản.

```python
# Metadata cho văn bản hành chính Vn
def extract_metadata(text: str) -> Metadata:
    # Dùng LLM với prompt trích xuất
    prompt = f"""
    Trích xuất metadata từ văn bản sau, trả về JSON:
    {{
      "so_ky_hieu": "...",
      "ngay_ban_hanh": "dd/mm/yyyy",
      "co_quan_ban_hanh": "...",
      "nguoi_ky": "...",
      "chuc_vu": "...",
      "trich_yeu": "...",
      "linh_vuc": "...",
      "do_mat": "...",
      "do_khan": "..."
    }}
    
    Văn bản: {text[:2000]}
    """
    return json.loads(llm.generate(prompt))
```

### STEP 6: Embedding (Vector hóa)

**Mục đích:** Biến text thành vector số để so sánh ngữ nghĩa.

```python
# Embedding batch (tiết kiệm chi phí)
def embed_chunks(chunks: List[Chunk]) -> List[Vector]:
    vectors = []
    for batch in batched(chunks, batch_size=100):
        texts = [c.content for c in batch]
        # Cache check: skip chunks đã embed
        uncached = [t for t in texts if hash(t) not in embedding_cache]
        cached = {t: embedding_cache[hash(t)] for t in texts if hash(t) in embedding_cache}
        
        # Embed chỉ phần chưa có
        if uncached:
            new_vecs = embedding_model.encode(uncached)  # API call
            for t, v in zip(uncached, new_vecs):
                embedding_cache[hash(t)] = v
                cached[t] = v
        
        vectors.extend([cached[t] for t in texts])
    return vectors
```

**Các embedding model được sử dụng:**

| Model | Provider | Dimensions | Chi phí | Chất lượng (TV) |
|---|---|---|---|---|
| `text-embedding-3-small` | OpenAI | 1536 | $0.02/1M tokens | Tốt (8/10) |
| `text-embedding-3-large` | OpenAI | 3072 | $0.13/1M tokens | Rất tốt (9/10) |
| `nomic-embed-text` | Ollama local | 768 | $0 (chỉ GPU) | Tốt (8/10) |
| `bge-m3` | Ollama local | 1024 | $0 (chỉ GPU) | Tốt (8.5/10) |

### STEP 7: Indexing (Lập chỉ mục)

**Mục đích:** Lưu vào Vector DB với index để tìm kiếm nhanh.

```sql
-- pgvector schema
CREATE TABLE ai_platform.knowledge_chunk (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    source_id UUID NOT NULL,
    doc_id VARCHAR(200),
    
    chunk_index INTEGER,
    content TEXT NOT NULL,
    content_hash CHAR(64) NOT NULL,      -- SHA-256 để dedup
    embedding vector(1536),              -- vector embedding
    metadata JSONB DEFAULT '{}',
    
    created_at TIMESTAMPTZ DEFAULT now()
);

-- Index cho vector search (cosine similarity)
CREATE INDEX knowledge_chunk_embedding_idx 
    ON ai_platform.knowledge_chunk 
    USING ivfflat (embedding vector_cosine_ops) 
    WITH (lists = 100);

-- Index cho full-text search (BM25)
CREATE INDEX knowledge_chunk_content_fts_idx 
    ON ai_platform.knowledge_chunk 
    USING gin (to_tsvector('simple', content));

-- Index cho filter theo metadata
CREATE INDEX knowledge_chunk_metadata_idx 
    ON ai_platform.knowledge_chunk 
    USING gin (metadata);

-- Row-Level Security (cô lập theo tenant)
ALTER TABLE ai_platform.knowledge_chunk ENABLE ROW LEVEL SECURITY;
CREATE POLICY tenant_isolation ON ai_platform.knowledge_chunk
    USING (tenant_id = current_setting('app.tenant_id')::uuid);
```

### STEP 8: Ingestion Log

**Mục đích:** Theo dõi trạng thái, lỗi, thống kê.

```sql
CREATE TABLE ai_platform.knowledge_ingestion_log (
    id UUID PRIMARY KEY,
    source_id UUID NOT NULL,
    tenant_id UUID NOT NULL,
    status VARCHAR(20),              -- pending / running / done / failed
    total_documents INTEGER,
    total_chunks INTEGER,
    total_tokens INTEGER,
    started_at TIMESTAMPTZ,
    finished_at TIMESTAMPTZ,
    duration_seconds INTEGER,
    error_message TEXT,
    created_at TIMESTAMPTZ DEFAULT now()
);
```

---

## 🔄 Khi nào Pipeline chạy?

### Các trigger (chế độ chạy)

| Trigger | Mô tả | Use case |
|---|---|---|
| **Manual** | Admin bấm nút "Ingest ngay" trong Admin Console | Test, ingest 1 file mới |
| **Schedule** | Cron job theo lịch | Ingestion định kỳ (vd: 0h mỗi đêm, mỗi tuần) |
| **Event-driven** | Khi có văn bản mới đến (Kafka event) | Real-time ingestion |
| **Webhook** | Service khác gọi API ingestion | Tích hợp HRM, ERP |

### Ví dụ cấu hình schedule

```yaml
# Knowledge Source config
sources:
  - id: dms_van_ban_den
    name: "Văn bản đến"
    type: dms
    connection:
      api_url: "https://dms.btach.gov.vn/api"
      auth: oauth2
    schedule:
      mode: cron
      cron: "0 2 * * *"           # 2h sáng mỗi ngày
    chunking:
      strategy: recursive
      chunk_size: 512
      overlap: 50
    embedding:
      model: text-embedding-3-small
    filter:
      ngay_tu: "2020-01-01"
      loai_van_ban: ["cong_van", "quyet_dinh", "thong_bao"]
```

---

## 📊 Ví dụ thực tế cho Bộ Tài Chính

### Case 1: Ingestion văn bản 10 năm của Bộ

**Input:**
- 2.5 triệu văn bản từ 2020-2025
- Tổng dung lượng: ~500 GB
- Mixed: PDF text + PDF scan + DOCX

**Pipeline:**

```
1. Connect: DMS API list_all_documents
2. Extract: 800.000 PDF text + 1.500.000 PDF scan + 200.000 DOCX
3. Clean: bỏ noise, chuẩn hóa
4. Chunk: 2.500.000 văn bản → 12.500.000 chunks (avg 5 chunks/văn bản)
5. Metadata: LLM trích metadata → 12.500.000 records
6. Embedding: 12.500.000 chunks × 1536 dims × 4 bytes = ~76 GB vector data
7. Index: pgvector với 100 lists (ivfflat)
8. Log: done - 8 giờ ingestion

Kết quả:
- 12.5M chunks indexed
- Sẵn sàng cho hybrid search + RAG
```

### Case 2: Ingestion văn bản mới real-time

**Input:** Văn thư upload văn bản mới vào DMS

```
1. Event: file.uploaded → Kafka
2. Trigger: AI Worker pool nhận event
3. Extract: PDF text → text
4. Clean: remove noise
5. Chunk: 5 chunks
6. Embed: 5 vectors
7. Index: upsert vào pgvector
8. Time: < 30s cho văn bản 10 trang
9. Notify: User nhận thông báo "văn bản đã sẵn sàng cho tìm kiếm"
```

### Case 3: Ingestion HRM cho ChatOps

**Input:**
- Quy trình HRM (15 quy trình chính)
- Quy chế chi tiêu nội bộ
- 1.000 FAQ nhân sự

**Pipeline:**
```
1. Source: HRM Database + SharePoint
2. Extract: SQL + Graph API
3. Clean: bỏ template noise
4. Chunk: mỗi quy trình → 3-5 chunks
5. Embed: 5.000 chunks
6. Index: tag với source_type="hr"
7. Done: 5 phút
```

---

## 🎯 Các chỉ số đánh giá (KPIs)

| Chỉ số | Mục tiêu | Đo lường |
|---|---|---|
| **Ingestion throughput** | ≥ 1.000 chunks/phút | Count chunks ingest/giờ |
| **Ingestion latency** | 95%ile ≤ 60s cho văn bản 10 trang | Stopwatch |
| **Failure rate** | < 1% | Count failed jobs / total |
| **Storage efficiency** | ≤ 1 KB/vector (sau quantization) | Storage / num_vectors |
| **Retrieval Precision@10** | ≥ 0.85 | RAGAS framework |
| **Retrieval Recall@10** | ≥ 0.80 | RAGAS framework |
| **Cost per chunk** | ≤ $0.0001 (OpenAI) | Total cost / num_chunks |
| **Index freshness** | Real-time ingestion ≤ 5 phút | Time from upload to indexed |

---

## 🔒 Bảo mật trong Ingestion Pipeline

| Lớp | Biện pháp |
|---|---|
| **Source authentication** | OAuth2/API key cho DMS, SharePoint, HRM |
| **Transport** | TLS 1.3 cho mọi kết nối |
| **PII redaction** | Tự động redact căn cước, SĐT, email trước khi embed |
| **Tenant isolation** | Mỗi chunk gắn tenant_id; Row-Level Security trong pgvector |
| **Audit log** | Ghi lại mọi ingestion: ai ingest, lúc nào, bao nhiêu chunk |
| **Encryption at rest** | PostgreSQL TDE + MinIO SSE-S3 |
| **Classification** | Văn bản "Mật/Tối mật" → chỉ dùng embedding model on-prem (không gửi OpenAI) |
| **Rate limiting** | Giới hạn số ingestion/ngày/tenant để chống abuse |

---

## ⚙️ Công nghệ sử dụng

| Layer | Công nghệ |
|---|---|
| **Orchestration** | Apache Airflow / Dagster / Prefect (workflow) |
| **Message queue** | Kafka (event-driven) |
| **Source connectors** | Custom Python connectors |
| **Extract** | pdfplumber, python-docx, BeautifulSoup, Tabula |
| **OCR** | Tesseract 5, PaddleOCR |
| **Clean** | Custom Python (regex, NER) |
| **Chunking** | LangChain, LlamaIndex, custom |
| **Metadata** | LLM + NER spaCy |
| **Embedding** | OpenAI API, Ollama local |
| **Vector DB** | PostgreSQL + pgvector |
| **Cache** | Redis |
| **Storage** | MinIO / S3 |
| **Monitoring** | Prometheus + Grafana |

---

## 📁 Triển khai trong code `qlvb-backend-core`

### Backend services sẽ tạo

```
src/Services/AI/
├── SmartOffice.AIPlatform.Domain/
│   ├── Entities/
│   │   ├── KnowledgeSource.cs
│   │   ├── KnowledgeChunk.cs
│   │   └── KnowledgeIngestionLog.cs
│   └── ValueObjects/
│       ├── ChunkingConfig.cs
│       └── EmbeddingConfig.cs
├── SmartOffice.AIPlatform.Application/
│   ├── Knowledge/Commands/
│   │   ├── IngestSourceCommand.cs
│   │   └── IngestSourceHandler.cs
│   └── Knowledge/Queries/
│       ├── GetIngestionStatusQuery.cs
│       └── SearchKnowledgeQuery.cs
└── SmartOffice.AIPlatform.Infrastructure/
    ├── Sources/
    │   ├── DmsSourceConnector.cs
    │   ├── FileSourceConnector.cs
    │   └── WebSourceConnector.cs
    ├── Extractors/
    │   ├── PdfExtractor.cs
    │   ├── DocxExtractor.cs
    │   └── HtmlExtractor.cs
    ├── Chunkers/
    │   ├── RecursiveChunker.cs
    │   └── SemanticChunker.cs
    ├── Embeddings/
    │   ├── OpenAIEmbeddingProvider.cs
    │   └── OllamaEmbeddingProvider.cs
    └── VectorStore/
        ├── PgVectorStore.cs
        └── QdrantVectorStore.cs
```

### Database schema (multi-tenant)

```sql
-- Schema riêng cho AI Platform
CREATE SCHEMA ai_platform;

-- Knowledge Source: định nghĩa nguồn
CREATE TABLE ai_platform.knowledge_source (
    id UUID PRIMARY KEY,
    tenant_id UUID NOT NULL,
    name VARCHAR(200),
    source_type VARCHAR(50),                  -- dms/file/web/hr/erp
    connection_config JSONB,
    chunking_config JSONB,
    embedding_config JSONB,
    schedule_cron VARCHAR(50),
    status VARCHAR(20),
    last_ingest_at TIMESTAMPTZ,
    total_chunks INTEGER DEFAULT 0,
    created_at TIMESTAMPTZ DEFAULT now()
);

-- Knowledge Chunk: từng đoạn văn bản đã embed
CREATE TABLE ai_platform.knowledge_chunk (
    id UUID PRIMARY KEY,
    source_id UUID NOT NULL,
    tenant_id UUID NOT NULL,
    doc_id VARCHAR(200),
    chunk_index INTEGER,
    content TEXT NOT NULL,
    content_hash CHAR(64) NOT NULL,
    embedding vector(1536),
    metadata JSONB DEFAULT '{}',
    created_at TIMESTAMPTZ DEFAULT now(),
    UNIQUE(tenant_id, content_hash)          -- dedup
);

-- Ingestion log
CREATE TABLE ai_platform.knowledge_ingestion_log (
    id UUID PRIMARY KEY,
    source_id UUID NOT NULL,
    tenant_id UUID NOT NULL,
    status VARCHAR(20),
    total_documents INTEGER,
    total_chunks INTEGER,
    started_at TIMESTAMPTZ,
    finished_at TIMESTAMPTZ,
    duration_seconds INTEGER,
    error_message TEXT,
    created_at TIMESTAMPTZ DEFAULT now()
);
```

---

## 📊 So sánh: Có và không có Ingestion Pipeline

| Khía cạnh | KHÔNG có Ingestion Pipeline | CÓ Ingestion Pipeline |
|---|---|---|
| **Tìm văn bản** | Gõ từ khóa, lọc thủ công | Hỏi tự nhiên, AI trả về ngay |
| **ChatOps** | Không có | Hỏi LLM có trích dẫn |
| **RAG** | LLM trả lời sai (hallucinate) | LLM trả lời dựa trên văn bản thật |
| **OCR** | Văn thư gõ tay | Tự động trích xuất |
| **Phân loại** | Đọc thủ công | AI gợi ý |
| **Gợi ý văn bản tương tự** | Không có | Tự động |
| **Khử trùng lặp** | Thủ công | Tự động |
| **Thời gian tìm văn bản** | 15-30 phút | < 30 giây |
| **Độ chính xác** | ~60% | > 85% |

---

## 🎯 Tóm tắt 1 câu

> **Knowledge Ingestion Pipeline** = quy trình tự động **lấy → làm sạch → chia nhỏ → vector hóa → lưu trữ → đánh index** văn bản vào Vector Database, để khi LLM cần trả lời câu hỏi có thể **tra cứu ngữ nghĩa trong < 1 giây** thay vì phải đọc toàn bộ 1 triệu văn bản.

---

## 📎 Vị trí trong tài liệu chính

Trong file `BaoCaoTKTC_AI_BTC_Full.docx`, nội dung Knowledge Ingestion Pipeline được đề cập ở:

- **Mục II.5.4 — Phân hệ Knowledge Base & RAG** (chính)
- **Mục II.1.7 — Kiến trúc dữ liệu** (bảng `ai_platform.knowledge_chunk`)
- **Mục II.1.8 — Hạ tầng** (PostgreSQL + pgvector requirement)
- **Phần AI-B.2.2 — RAG Pipeline** (trong file AI riêng)
- **Phần AI-B.2.8 — Knowledge Base & Vector DB** (trong file AI riêng)

Nếu bạn muốn đào sâu thêm vào **bất kỳ step nào** (vd: so sánh chi tiết 5 chiến lược chunking, code mẫu Python đầy đủ, cách tối ưu chi phí embedding, xử lý văn bản 100 trang, ...) tôi sẽ viết tiếp.