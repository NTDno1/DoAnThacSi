# THIẾT KẾ CHI TIẾT CHỨC NĂNG

> Phần này mô tả chi tiết từng phân hệ AI đã liệt kê trong mục **II.2 – Mô hình phân rã chức năng**. Mỗi phân hệ được trình bày theo cấu trúc thống nhất: Mô tả → Luồng xử lý → Module liên quan → Tài liệu liên quan → Thiết kế giao diện (console) → Thành phần giao diện → Thiết kế hàm/thủ tục.

---

## II.5. Hệ thống AI

Hệ thống AI bao gồm **11 phân hệ chức năng** cấu thành AI Layer, hoạt động đồng bộ với các phân hệ nghiệp vụ của Văn phòng số. Mỗi phân hệ dưới đây được thiết kế theo cùng một template, đảm bảo tính thống nhất và dễ bảo trì.

---

## II.5.1. Phân hệ AI Gateway

### II.5.1.1. Mô tả chức năng

AI Gateway là **cổng tiếp nhận duy nhất** cho mọi yêu cầu gọi AI từ:

- Frontend (Web Portal, Mobile App, ChatOps)
- Backend service khác thông qua BFF / service-to-service
- Tác vụ bất đồng bộ (worker job)

**Chức năng chính:**

1. Xác thực (JWT verify, OAuth2/OIDC) và phân quyền (RBAC + ABAC).
2. Rate-limit theo user, tenant, IP, model.
3. Chuẩn hóa request: parse, validate schema, gắn metadata (tenant, user, trace).
4. **Prompt sanitization**: lọc các pattern prompt injection, PII, nội dung không phù hợp.
5. Chọn LLM provider dựa trên: chính sách tenant, chi phí, latency yêu cầu, độ nhạy dữ liệu.
6. Caching phản hồi (semantic cache) cho các câu hỏi lặp lại.
7. Streaming response qua SSE/WebSocket.
8. Ghi audit log và metrics.
9. Failover khi provider lỗi (OpenAI → Ollama, ...).

### II.5.1.2. Luồng xử lý chức năng

```
Client → Kong → AI Gateway → [AuthN] → [Rate-Limit] → [Sanitize]
       → [Cache Check] → [Provider Select] → Call LLM
       → [Stream/Response] → [Audit] → Client
```

**Các bước xử lý chi tiết:**

| Bước | Xử lý |
|---|---|
| 1 | Nhận HTTP request `/v1/ai/chat`, `/v1/ai/summarize`, ... |
| 2 | Verify JWT (OpenIddict public key + JWKS rotation). Lấy `tenant_id`, `user_id`, `roles`. |
| 3 | Áp dụng Rate-limit Kong + service-level token-bucket. |
| 4 | Validate schema theo OpenAPI spec. |
| 5 | Sanitize prompt: lọc pattern injection, redact PII. |
| 6 | Kiểm tra semantic cache (Redis) bằng embedding similarity (cosine ≥ 0.95). Nếu trúng → trả về ngay. |
| 7 | Chọn provider theo policy (cost, sensitivity, availability). |
| 8 | Gọi LLM provider (có streaming). |
| 9 | Validate output (JSON schema nếu function-calling, length, profanity). |
| 10 | Ghi audit log, metrics, charge token usage cho tenant. |
| 11 | Trả về response cho client (SSE nếu streaming). |

### II.5.1.3. Tài liệu liên quan

- II.1.6 (Tích hợp), II.1.9 (Bảo mật), II.5.6 (Provider Abstraction).
- `docs/openapi/ai-platform.yaml` (sẽ sinh ở giai đoạn triển khai).

### II.5.1.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `SmartOffice.AIPlatform.Api.Gateway` | Controller, middleware |
| 2 | `SmartOffice.AIPlatform.Application.Gateway` | CQRS handler cho `SendPromptCommand` |
| 3 | `AIPlatform.Application.Validation` | FluentValidation schema |
| 4 | `SmartOffice.AIPlatform.Infrastructure.Providers` | Provider selector, failover |
| 5 | `SmartOffice.AIPlatform.Infrastructure.Cache` | Semantic cache |

### II.5.1.5. Thiết kế giao diện (AI Admin Console – mục này mô tả phần quản trị Gateway)

![Mô tả] Màn hình cấu hình Gateway: danh sách route, rate-limit, default model, fallback chain, semantic cache TTL.

### II.5.1.6. Thành phần giao diện (AI Admin Console)

| Tên control | Bắt buộc | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Route path | Có | Text | Đường dẫn API AI | `/v1/ai/{action}` |
| HTTP method | Có | Select | Phương thức | POST, GET |
| Default provider | Có | Select | Provider mặc định | openai, ollama, anthropic |
| Fallback chain | Không | Multi-select | Danh sách provider dự phòng | Ưu tiên từ trên xuống |
| Rate limit / user | Có | Number | Số req mỗi phút | 100 mặc định |
| Rate limit / tenant | Có | Number | Số req mỗi phút theo tenant | 10.000 |
| Semantic cache TTL | Không | Number (giây) | Thời gian sống cache | 3600 |
| PII redaction | Không | Boolean | Bật/tắt che PII | Bật mặc định |
| Audit retention | Có | Number (ngày) | Lưu audit log | 365 |

### II.5.1.7. Thiết kế hàm / thủ tục (pseudo)

```python
async def handle_ai_request(req: AiRequest) -> AiResponse:
    user = auth.verify_jwt(req.token)            # Bước 2
    rate_limiter.check(user, req.tenant)          # Bước 3
    schema.validate(req)                          # Bước 4
    clean_prompt = pii_redactor.sanitize(req)     # Bước 5
    cache_key = embedding.hash(clean_prompt)
    cached = await cache.get(cache_key)           # Bước 6
    if cached:
        return cached                            # Bước 6.1 (early-return)
    provider = provider_selector.pick(req.tenant, clean_prompt) # Bước 7
    response = await llm_client.stream(provider, clean_prompt) # Bước 8
    validator.check(response)                     # Bước 9
    audit.write(user, req, response)              # Bước 10
    await cache.set(cache_key, response)          # Bước 10.1
    return response                               # Bước 11
```

---

## II.5.2. Phân hệ AI Engine Core

### II.5.2.1. Mô tả chức năng

AI Engine Core là **lõi xử lý AI**, nơi thực thi logic nghiệp vụ AI:

1. Nhận request (đã được sanitize từ Gateway).
2. **Prompt builder**: ghép system prompt, context, user prompt theo template versioning.
3. Điều phối các bước phụ thuộc use case:
   - **Embedding**: tạo vector embedding cho câu truy vấn.
   - **Retrieval**: tìm kiếm context từ Knowledge Base (hybrid search).
   - **Reranking**: sắp xếp lại kết quả bằng cross-encoder.
   - **Context aggregation**: tổng hợp context, đếm token, cắt gọn.
   - **Generation**: gọi LLM sinh câu trả lời.
   - **Validation**: kiểm tra output có "đúng cấu trúc", "trung thực" (faithfulness).
   - **Citation extraction**: tách trích dẫn từ `source_url/page/number`.
4. Trả về response có cấu trúc + citations cho Gateway.

### II.5.2.2. Luồng xử lý chức năng (RAG Use Case)

```
Request → [Build Prompt] → [Embed query] → [Retrieve top-K]
        → [Rerank] → [Aggregate context] → [Generate]
        → [Validate] → [Extract citations] → Response
```

| Bước | Xử lý |
|---|---|
| 1 | Nhận `RunPipelineCommand` từ Gateway/JobQueue. |
| 2 | Load `PromptTemplate` theo `tenant_id` + `prompt_version` |
| 3 | Render template, gắn system message, user message |
| 4 | Gọi Embedding service (pgvector / OpenAI ada) → `query_embedding` |
| 5 | Gọi Retrieval service (hybrid BM25 + vector) → `top_k_documents` |
| 6 | Rerank top_k → top_n (5-10) bằng cross-encoder |
| 7 | Aggregate context: cắt theo token budget, gom metadata |
| 8 | Call LLM generate → response |
| 9 | Validate JSON schema, hallucination check (cosine similarity context ↔ response) |
| 10 | Trích citations |
| 11 | Trả response |

### II.5.2.3. Tài liệu liên quan

- II.1.7 (Kiến trúc dữ liệu – bảng `ai_job_step`).
- `docs/database/smart_office.ai_platform.md` (sẽ sinh ở giai đoạn triển khai).

### II.5.2.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `SmartOffice.AIPlatform.Application.Engine` | CQRS handlers cho `RunPipelineCommand`, `BuildPromptCommand` |
| 2 | `SmartOffice.AIPlatform.Domain.Pipeline` | `Pipeline`, `PipelineStep`, `PipelineContext` |
| 3 | `SmartOffice.AIPlatform.Infrastructure.LLM` | EmbeddingAdapter, GenerationAdapter |

### II.5.2.5. Thiết kế giao diện (AI Admin Console - Pipeline Builder)

Màn hình cho phép Admin cấu hình pipeline bằng cách kéo thả các step: Embedding → Retrieve → Rerank → Generate → Validate.

### II.5.2.6. Thành phần giao diện

| Tên control | Bắt buộc | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Pipeline name | Có | Text | Tên pipeline | `rag_default` |
| Step list | Có | Drag-drop | Chuỗi bước | Thứ tự: Embed → Retrieve → Rerank → Gen → Validate |
| Top-K | Có | Number | Số kết quả retrieve | 50 |
| Top-N | Có | Number | Số kết quả rerank | 8 |
| Token budget | Có | Number | Ngân sách token output | 1024 |
| Temperature | Có | Number (0-1) | Độ sáng tạo LLM | 0.2 |
| Provider | Có | Select | LLM provider | openai, ollama |
| Model | Có | Select | Model cụ thể | gpt-4o-mini, qwen2.5:7b |
| Citation enabled | Không | Boolean | Bật/tắt citation | Bật |
| Faithfulness check | Không | Boolean | Bật kiểm tra hallucination | Bật |

### II.5.2.7. Thiết kế hàm / thủ tục

```python
async def run_pipeline(req: RunPipelineCommand) -> PipelineResult:
    template = prompt_repo.get_active(req.tenant, req.use_case) # bước 2
    prompt   = template.render(req.vars)                          # bước 3
    q_vec    = embedding.encode(prompt.user)                      # bước 4
    docs     = retrieval.hybrid_search(q_vec, top_k=req.top_k,    # bước 5
                                     tenant=req.tenant)
    docs     = reranker.rerank(prompt.user, docs, top_n=req.top_n)# bước 6
    context  = context_aggregator.fit(docs, req.token_budget)    # bước 7
    answer   = llm.generate(prompt.system, context, prompt.user, # bước 8
                            temperature=req.temperature)
    validator.check(answer, schema=OUTPUT_SCHEMA)                # bước 9
    citations = citation_extractor.extract(answer, docs)         # bước 10
    return PipelineResult(answer=answer, citations=citations)     # bước 11
```

---

## II.5.3. Phân hệ Agent Orchestration

### II.5.3.1. Mô tả chứnăng

Agent Orchestration cho phép giải quyết **các yêu cầu phức tạp đa bước** mà một lần gọi LLM không xử lý được. Ví dụ:

- *"Tìm tất cả văn bản đến quá hạn phê duyệt trong 7 ngày qua, phân loại theo đơn vị, sinh báo cáo Excel đính kèm email gửi lãnh đạo."*

Thành phần gồm:

1. **Intent Router** (LLM-based classifier): phân loại yêu cầu → use case (`rag_chat`, `task`, `report`, `code`).
2. **Task Planner**: sinh chuỗi bước (chain-of-thought) cho yêu cầu phức tạp.
3. **Multi-Agent Coordinator**: giao bước cho các agent chuyên trách (`RetrieverAgent`, `AnalyzerAgent`, `WriterAgent`).
4. **Tool calling (MCP)**: Agent gọi tool thông qua MCP Server để thao tác với hệ thống ngoài (DMS, HRM, Email).
5. **Human-in-the-Loop** (HITL): Khi agent thực hiện thao tác "ghi" (gửi mail, xóa văn bản), hệ thống **dừng chờ duyệt** từ user.
6. **Safety Guardrails**: Lọc action nguy hiểm (vd: cố gắng xóa DB, gửi tới domain lạ).
7. **Max-steps / Loop protection**: Tránh agent loop vô hạn.

### II.5.3.2. Luồng xử lý chức năng

```
User query → [Intent Router] → Use Case
       ├─ simple: → AI Gateway (RAG)
       └─ complex:
           → [Task Planner] → plan = [(step1, agent1, tool1), ...]
           → FOR step in plan:
               - [Safety check]
               - [Pick agent] → [Agent executes] → [Tool call MCP]
               - [HITL pause if write action]
               - [Aggregate intermediate result]
           → [Final answer]
```

| Bước | Xử lý |
|---|---|
| 1 | Nhận yêu cầu từ user qua Gateway. |
| 2 | Intent Router (LLM) phân loại: simple rag hay complex task. |
| 3 | Nếu simple → trả về II.5.2 (RAG pipeline). |
| 4 | Nếu complex → Task Planner sinh plan JSON. |
| 5 | Multi-Agent Coordinator lặp qua plan. |
| 6 | Với mỗi step: Safety Guardrail kiểm tra trước khi thực thi. |
| 7 | Agent gọi MCP tool tương ứng. |
| 8 | Nếu tool là write (create/send/update) → HITL: chờ duyệt. |
| 9 | Ghi intermediate result vào `ai_job_step`. |
| 10 | Sau khi hết plan → Final Agent ghép nội dung trả lời. |
| 11 | Audit + token tracking toàn bộ plan. |

### II.5.3.3. Tài liệu liên quan

- II.5.1, II.5.8 (MCP Server).
- Tài liệu "AI Agent Workflow": `qlvb-backend-core/README_AI_AGENT_WORKFLOW.md`.

### II.5.3.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `SmartOffice.AIPlatform.Application.Agent` | `IntentRouter`, `TaskPlanner`, `Coordinator` |
| 2 | `SmartOffice.AIPlatform.Domain.Agent` | `Agent`, `Tool`, `ToolInvocation`, `Plan` |
| 3 | `SmartOffice.AIPlatform.Infrastructure.MCP` | MCP client SDK |

### II.5.3.5. Thiết kế giao diện (AI Admin Console – Agent Designer)

Giao diện trực quan để thiết kế agent: đặt tên, instruction (system prompt), chọn tools, max-steps, model mặc định, allowed tenants.

### II.5.3.6. Thành phần giao diện

| Tên control | Bắt buộc | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Agent name | Có | Text | Tên agent | `DocReportAgent` |
| Instruction | Có | Textarea (markdown) | System prompt agent | Hỗ trợ template |
| Model | Có | Select | Model mặc định | gpt-4o-mini |
| Tools | Có | Multi-select | Danh sách tool MCP | search_doc, list_tasks |
| Max steps | Có | Number | Số bước tối đa | 10 |
| HITL required | Không | Boolean | Bắt buộc duyệt write | true cho write actions |
| Allowed tenants | Không | Multi-select | Tenant được phép | – |
| Safety keywords | Không | List text | Lọc action cấm | "delete", "drop", "truncate" |

### II.5.3.7. Thiết kế hàm / thủ tục

```python
async def orchestrate(req: OrchestrateCommand) -> OrchestrateResult:
    intent = intent_router.classify(req.prompt)               # bước 2
    if intent.kind == "rag":
        return await engine.run_pipeline(rag_for(req))        # bước 3
    plan = task_planner.plan(req.prompt, req.tenant, intents=intent) # bước 4
    intermediate = []
    for step in plan.steps:
        if not safety_guardrail.allow(step, req.user):        # bước 6
            raise SafetyError("blocked")
        result = await agent_executor.run(step.agent, step, intermediate) # bước 7
        if step.requires_hitl:
            await hitl.wait_for_approval(req.user, step, result) # bước 8
        intermediate.append(result)                          # bước 9
        audit.append_step(req.job_id, step, result)
    final = writer_agent.compose(req.prompt, intermediate)     # bước 10
    return OrchestrateResult(answer=final, plan=plan)         # bước 11
```

---

## II.5.4. Phân hệ Knowledge Base & RAG

### II.5.4.1. Mô tả chức năng

Quản lý kho tri thức ngữ nghĩa cho toàn hệ thống:

1. **Knowledge Source management**: khai báo nguồn tri thức (Văn bản DMS, Quy trình HRM, File PDF, Web nội bộ).
2. **Ingestion pipeline**:
   - Trích text (cho file: OCR + parser; cho HTML: scraping).
   - Chunking (recursive, semantic, sliding window) cấu hình được.
   - Embedding bằng model đã đăng ký.
   - Lưu vào `pgvector` / Qdrant cluster.
3. **Retrieval pipeline**:
   - Hybrid search (BM25 + vector cosine) → RRF fusion.
   - Cross-encoder reranker.
   - Context aggregation với token budget.
   - Citation extraction.
   - Faithfulness check.
4. **Quality evaluation**: Tích hợp framework RAGAS đánh giá offline.

### II.5.4.2. Luồng xử lý

**(a) Ingestion:**

```
Source → [Extract text] → [Clean/normalize] → [Chunk] → [Embedding]
       → [Store vector] → Index trong pgvector + metadata trong PostgreSQL
```

**(b) Retrieval:**

```
Query → [Embed query] → [BM25 + Vector search + RRF] → [Rerank]
     → [Aggregate context] → Citations
```

| Bước | Xử lý |
|---|---|
| 1 | Trigger ingestion (manual / schedule / event from DMS). |
| 2 | Extract text (HTML/PDF/DOCX). |
| 3 | Clean: loại bỏ HTML tag, header/footer, khoảng trắng thừa. |
| 4 | Chunk theo cấu hình (chunk_size, overlap, semantic delimiter). |
| 5 | Embedding mỗi chunk. |
| 6 | Lưu vector vào pgvector + metadata vào PostgreSQL. |
| 7 | Ghi `knowledge_ingestion_log`. |
| 8 | Khi truy vấn: embed query. |
| 9 | Hybrid search kết hợp BM25 + vector. |
| 10 | RRF fusion. |
| 11 | Rerank với cross-encoder. |
| 12 | Aggregate context, trích citations. |

### II.5.4.3. Tài liệu liên quan

- II.1.7 (Dữ liệu), II.5.6 (Provider), tài liệu `AI_README.md` trong `qlvb-documents`.

### II.5.4.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `SmartOffice.AIPlatform.Application.Knowledge` | `IngestSourceCommand`, `SearchQuery` |
| 2 | `SmartOffice.AIPlatform.Infrastructure.Vector` | pgvector / Qdrant adapter |
| 3 | `SmartOffice.AIPlatform.Infrastructure.Embedding` | Embedding model adapter |

### II.5.4.5. Thiết kế giao diện (AI Admin Console – Knowledge Source)

Màn hình quản lý nguồn tri thức: danh sách, trạng thái ingestion, số chunks, lỗi.

### II.5.4.6. Thành phần giao diện

| Tên control | Bắt buộc | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Source name | Có | Text | Tên nguồn | `Văn bản Bộ` |
| Source type | Có | Select | Loại: dms/file/web | dms, file, web |
| Connection | Có | Text | Connection string DMS / URL | – |
| Chunk size | Có | Number | Kích thước chunk | 512 |
| Overlap | Có | Number | Overlap giữa chunk | 50 |
| Embedding model | Có | Select | Model embedding | text-embedding-3-small |
| Schedule | Không | Cron | Lịch ingestion định kỳ | `0 2 * * *` |
| Status | Không | Select | Hiển thị trạng thái | pending/running/done/failed |
| Last ingest | Không | Datetime | Lần ingest cuối | – |

### II.5.4.7. Thiết kế hàm / thủ tục

```python
async def ingest(source: KnowledgeSource):
    docs = extract(source)                            # bước 2-3
    chunks = chunker.split(docs, size=512, overlap=50) # bước 4
    vectors = [embedding.encode(c.text) for c in chunks] # bước 5
    pgvector.upsert(source.tenant, chunks, vectors)   # bước 6
    log_ingestion(source, len(chunks))                # bước 7

async def retrieve(query: str, tenant: str, top_k=50, top_n=8):
    q_vec = embedding.encode(query)                   # bước 8
    bm25_hits = bm25.search(query, top_k, tenant)     # bước 9a
    vec_hits  = vector.search(q_vec, top_k, tenant)   # bước 9b
    fused = rrf_fuse(bm25_hits, vec_hits)             # bước 10
    reranked = cross_encoder.rerank(query, fused, top_n) # bước 11
    return aggregator.fit(reranked, token_budget=2048) # bước 12
```

---

## II.5.5. Phân hệ OCR & Document Extraction

### II.5.5.1. Mô tả chức năng

Trích xuất dữ liệu có cấu trúc từ văn bản scan/ảnh/PDF của Bộ Tài Chính.

**Chức năng:**

1. Nhận file scan từ `Files Management API`.
2. OCR Engine (Tesseract / PaddleOCR / Surya) lấy text + bounding box.
3. Layout-aware parser: tách cột, bảng biểu, header/footer.
4. Table extraction: trích xuất bảng thành CSV/JSON.
5. Form extraction: dùng LLM/Donut trích xuất key-value.
6. Mapping: chuyển key-value sang schema định sẵn của văn bản hành chính (`Số ký hiệu`, `Ngày`, `Trích yếu`, `Đơn vị ban hành`, `Độ mật`, `Lĩnh vực`, ...).
7. Lưu vào `Files Management` metadata + link AI result.

### II.5.5.2. Luồng xử lý

```
File scan → [Detect] → [OCR] → [Layout-aware parse]
        → [Table extract] → [Form extract] → [Map to schema]
        → Output JSON → Save + audit
```

| Bước | Xử lý |
|---|---|
| 1 | Nhận file từ FileManagement API (qua Kafka event `file.uploaded`). |
| 2 | Detect file: PDF, image (JPEG, PNG, TIFF). |
| 3 | OCR (Tesseract 5+ cho tiếng Việt hoặc PaddleOCR). |
| 4 | Layout-aware: phân vùng trang, tách cột, đoạn, bảng. |
| 5 | Table extraction: OpenCV + heuristic hoặc Table-Transformer model. |
| 6 | Form/key-value extraction: dùng Donut hoặc LLM với prompt trích xuất. |
| 7 | Mapping: chuẩn hóa output về schema `DocumentMetadata`. |
| 8 | Validate: các trường bắt buộc (số ký hiệu, ngày). |
| 9 | Lưu kết quả vào `document_metadata` + push notification cho user. |

### II.5.5.3. Tài liệu liên quan

- Tài liệu `luong-ky-so-qlvb-old.md`, `strategy-pattern-ky-so.md`, `upload-large-file-flow.md` (trong `qlvb-backend-core/docs/modules/Files/`).

### II.5.5.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `SmartOffice.AIPlatform.Application.OCR` | `ExtractDocumentCommand` |
| 2 | `SmartOffice.AIPlatform.Infrastructure.OCR` | Tesseract / PaddleOCR / Donut adapter |
| 3 | `SmartOffice.AIPlatform.Infrastructure.Layout` | Layout parser |

### II.5.5.5. Thiết kế giao diện (AI Admin Console – OCR Jobs)

Theo dõi job OCR: pending / processing / done / failed + retry.

### II.5.5.6. Thành phần giao diện

| Tên control | Bắt buộc | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| File ID | Có | Text | UUID file | – |
| Engine | Có | Select | OCR engine | tesseract, paddleocr, donut |
| Language | Có | Select | Ngôn ngữ | vie, vie+eng |
| Extract table | Không | Boolean | Trích bảng | true |
| Extract form | Không | Boolean | Trích key-value | true |
| Schema mapping | Có | Select | Schema mapping | văn bản đến, văn bản đi, hợp đồng |
| Status | Không | Select | Hiển thị trạng thái | pending/done/failed |

### II.5.5.7. Thiết kế hàm / thủ tục

```python
async def extract(file_id: str, schema: str):
    file = files_repo.get(file_id)                  # bước 1
    pages = ocr_engine.detect_pages(file)           # bước 2-3
    layout = layout_parser.parse(pages)             # bước 4
    tables = table_extractor.extract(layout)        # bước 5
    kvs    = form_extractor.extract(layout, schema) # bước 6
    mapped = mapper.map(schema, kvs, tables)        # bước 7
    mapped = validator.required_check(mapped)       # bước 8
    files_repo.save_metadata(file_id, mapped)       # bước 9
    notify_user(file.owner, mapped)
```

---

## II.5.6. Phân hệ Provider Abstraction (LLM)

### II.5.6.1. Mô tả chức năng

Lớp trừu tượng cho phép hệ thống sử dụng **nhiều LLM provider** với cùng một giao diện.

**Chức năng:**

1. Implement interface `ILLMProvider` (`Generate`, `Stream`, `Embed`, `CountToken`).
2. Hỗ trợ: OpenAI, Ollama, Anthropic, Azure OpenAI, Cohere, Bedrock.
3. Cost-based routing: prompt được phân loại sensitivity (Low/Medium/High) → chọn provider tương ứng (Ollama local cho High, OpenAI cho Medium).
4. Failover chain: nếu provider chính lỗi → tự động fallback provider kế tiếp.
5. Token counting + budget tracking theo tenant.
6. Rate-limit & quota riêng cho từng provider.

### II.5.6.2. Luồng xử lý

```
Request → [Sensitivity classifier] → [Pick provider]
       → [Rate limit check] → [Token budget check]
       → [Call provider] → (on error) → [Failover next provider]
       → [Token counted] → Response
```

### II.5.6.3. Tài liệu liên quan

- II.5.1 (Gateway), `qlvb-documents` tài liệu "Thống kê nghiên cứu sử dụng các mô hình AI.xlsx".

### II.5.6.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `SmartOffice.AIPlatform.Domain.Providers` | interface `ILLMProvider`, `ProviderKind` enum |
| 2 | `SmartOffice.AIPlatform.Infrastructure.Providers.OpenAI` | OpenAI adapter |
| 3 | `SmartOffice.AIPlatform.Infrastructure.Providers.Ollama` | Ollama adapter |
| 4 | `SmartOffice.AIPlatform.Infrastructure.Providers.Anthropic` | Anthropic adapter |
| 5 | `SmartOffice.AIPlatform.Infrastructure.Providers.Azure` | Azure OpenAI adapter |

### II.5.6.5. Thiết kế giao diện (AI Admin Console – Provider Management)

Quản lý API key, base URL, model whitelist, default provider, fallback chain.

### II.5.6.6. Thành phần giao diện

| Tên control | Bắt buộc | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Provider | Có | Select | Tên provider | openai, ollama |
| API key | Có | Secret | Khóa truy cập | Không hiển thị plaintext |
| Base URL | Không | Text | URL endpoint | – |
| Models | Có | Multi-select | Model whitelist | – |
| Daily budget | Không | Number | Ngân sách token/ngày | – |
| Allowed tenants | Không | Multi-select | Tenant được dùng | – |
| Sensitivity levels | Không | Multi-select | Độ nhạy được phép | low/med/high |
| Active | Có | Boolean | Bật/tắt | – |

### II.5.6.7. Thiết kế hàm / thủ tục

```python
async def call(req: AiRequest):
    sensitivity = classifier.detect(req.prompt, req.tenant) # bước 1
    chain = provider_selector.chain(sensitivity, req.tenant) # bước 2
    for provider in chain:                                  # bước 3
        try:
            tokens_used = await quota.check(provider, req.tenant)
            response = await provider.generate(req)
            token_counter.charge(provider, req.tenant, response.usage)
            return response
        except (ProviderError, QuotaExceeded):
            logger.warn(f"provider {provider.name} failed, fallback")
            continue
    raise AllProvidersDownError()
```

---

## II.5.7. Phân hệ AI Job & Task Queue

### II.5.7.1. Mô tả chức năng

Quản lý tác vụ AI **không đồng bộ**, đảm bảo tác vụ nặng (summarize full văn bản, OCR scan 100 trang, ingestion 10k file) chạy nền, có retry, có thể quan sát.

**Thành phần:**

1. **Producer**: Service producer publish message vào Kafka topic `ai.jobs` (qua Outbox).
2. **Consumer worker pool**: Pull job, update status, chạy job.
3. **Retry policy**: exponential backoff, max 3 lần.
4. **Dead-letter queue**: `ai.jobs.dlq` cho job fail vĩnh viễn.
5. **Tracking**: bảng `ai_job` (status, payload, result, error).
6. **Priority**: tách theo tenant VIP hoặc độ quan trọng.

### II.5.7.2. Luồng xử lý

```
Service → Outbox → Kafka (ai.jobs)
Worker pool → consume → [SELECT FOR UPDATE ai_job] → mark processing
        → run job → on success: write result, mark done
        → on error: retry → on retry exhausted: send DLQ, mark failed
```

| Bước | Xử lý |
|---|---|
| 1 | Service ghi business change → cùng transaction ghi `outbox_message`. |
| 2 | Outbox relay đẩy sang Kafka `ai.jobs` (đảm bảo exactly-once với idempotency key). |
| 3 | Worker pool consume. |
| 4 | `SELECT FOR UPDATE` job → cập nhật `status=processing, locked_by=worker_id`. |
| 5 | Thực thi job (gọi AI Engine / OCR / Ingestion). |
| 6 | Thành công → ghi `result`, `status=done`. |
| 7 | Thất bại → retry với backoff; nếu hết → `status=failed`, đẩy `ai.jobs.dlq`. |
| 8 | Notify user / dashboard cập nhật. |

### II.5.7.3. Tài liệu liên quan

- `qlvb-backend-core/docs/Entity-Domain-And-OutboxEvent.md`.

### II.5.7.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `SmartOffice.AIPlatform.Application.Jobs` | `EnqueueJobCommand`, `JobDispatcher` |
| 2 | `SmartOffice.AIPlatform.Infrastructure.Jobs` | Worker host, retry, DLQ |
| 3 | `SmartOffice.AIPlatform.Domain.Jobs` | Entity `AIJob`, `AIJobStep` |

### II.5.7.5. Thiết kế giao diện (AI Admin Console – Job Monitor)

Bảng trạng thái job với filter theo: type, status, tenant, time range.

### II.5.7.6. Thành phần giao diện

| Tên control | Bắt buộc | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Job ID | Có | Text | UUID job | – |
| Job type | Có | Select | Loại job | summarize, ocr, ingest, classify |
| Status | Có | Select | Trạng thái | pending/processing/done/failed |
| Tenant | Không | Select | Tenant | – |
| Created at | Không | Datetime | Thời điểm tạo | – |
| Latency | Không | Number | Thời gian xử lý (ms) | – |
| Retry count | Không | Number | Số lần retry | – |
| Error | Không | Text | Lỗi | – |

### II.5.7.7. Thiết kế hàm / thủ tục

```python
async def dispatch(job: AiJob):
    outbox_repo.add_event(job)         # bước 1
    outbox_relay.flush()               # bước 2

async def worker_loop():
    while True:
        msg = kafka.poll("ai.jobs")    # bước 3
        async with job_lock(msg.id):   # bước 4
            try:
                result = await executor.run(msg.payload)
                job_repo.mark_done(msg.id, result) # bước 6
            except RetryableError as e:
                if msg.retry >= 3:
                    kafka.send("ai.jobs.dlq", msg) # bước 7
                    job_repo.mark_failed(msg.id, str(e))
                else:
                    kafka.send("ai.jobs", msg.with_retry())
```

---

## II.5.8. Phân hệ MCP Server

### II.5.8.1. Mô tả chức năng

Triển khai **Model Context Protocol** để expose các tool cho Agent.

**Tool phổ biến:**

| Tool | Mô tả |
|---|---|
| `get_document` | Lấy nội dung văn bản theo ID |
| `search_document` | Tìm văn bản theo từ khóa / ngữ nghĩa |
| `ocr_extract` | Trích dữ liệu từ file scan |
| `summarize` | Tóm tắt văn bản |
| `list_user_tasks` | Danh sách công việc user hiện tại |
| `create_worklog` | Tạo nhật ký công việc |
| `send_email` | Gửi email (HITL) |
| `sign_document` | Ký số văn bản (HITL) |
| `fetch_calendar` | Lấy lịch họp tuần |
| `create_meeting` | Đặt lịch họp (HITL) |

**Cấu trúc MCP:**

- JSON-RPC over HTTP hoặc stdio.
- Mỗi tool có `name`, `description`, `inputSchema` (JSON Schema).
- Có `resource` (file, doc, db row) và `prompt template`.

### II.5.8.2. Luồng xử lý

```
Agent → MCP client → POST /v1/mcp/tools/{name}/invoke
                     → [Auth] → [Audit] → [Execute] → Response
```

### II.5.8.3. Tài liệu liên quan

- `https://modelcontextprotocol.io` (chuẩn MCP).
- II.5.3 (Agent Orchestration).

### II.5.8.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `SmartOffice.AIPlatform.MCPServer` | JSON-RPC server |
| 2 | `SmartOffice.AIPlatform.MCPServer.Tools` | Tool implementations |

### II.5.8.5. Thiết kế giao diện (AI Admin Console – Tool Registry)

Danh sách tool, schema, version, audit log gọi tool.

### II.5.8.6. Thành phần giao diện

| Tên control | Bắt buộc | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Tool name | Có | Text | Tên tool | `search_doc` |
| Description | Có | Textarea | Mô tả | – |
| Input schema | Có | JSON editor | JSON Schema | – |
| Auth required | Có | Select | Cách xác thực | user, service, api-key |
| Allowed agents | Không | Multi-select | Agent nào dùng được | – |
| HITL | Có | Boolean | Yêu cầu duyệt | true cho write |
| Status | Có | Boolean | Active | – |

### II.5.8.7. Thiết kế hàm / thủ tục

```python
@mcp_tool(name="search_doc", description="Tìm văn bản theo từ khóa")
async def search_doc(args: dict, ctx: McpContext):
    auth.verify(ctx.user, ctx.tenant)        # bước 2 (auth)
    result = await retrieval.search(args.query, ctx.tenant, top_k=args.k)
    audit.write("tool.search_doc", ctx, args, result)  # bước 2 (audit)
    return McpResponse(content=result.to_mcp())        # bước 3
```

---

## II.5.9. Phân hệ Audit & Observability

### II.5.9.1. Mô tả chức năng

Đảm bảo **mọi hoạt động AI đều có thể truy vết** phục vụ an toàn thông tin, điều tra sự cố, đánh giá hiệu quả và tối ưu chi phí.

**Chức năng:**

1. **Audit log**: ghi mỗi request AI với `(user, tenant, prompt_hash, response_hash, model, provider, latency, token_usage, status)`.
2. **PII safe**: KHÔNG ghi trực tiếp prompt/response PII; dùng hash + lưu riêng có mã hóa.
3. **Metrics**: token/ngày theo tenant, model; latency p50/p95/p99; error rate.
4. **Distributed tracing**: OpenTelemetry, trace_id xuyên qua service.
5. **Alert**: cấu hình rule ngưỡng trong Grafana.
6. **Dashboard**: prompt hiệu quả, latency, chi phí.

### II.5.9.2. Luồng xử lý

```
Request → interceptor → record (audit, metrics, trace) → process
```

### II.5.9.3. Tài liệu liên quan

- II.1.10, II.5.7.

### II.5.9.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `SmartOffice.AIPlatform.Infrastructure.Audit` | Audit writer |
| 2 | `SmartOffice.AIPlatform.Infrastructure.Observability` | OpenTelemetry + Prometheus |
| 3 | `SmartOffice.AIPlatform.Application.Reports` | Dashboard query |

### II.5.9.5. Thiết kế giao diện (AI Admin Console – Audit Search)

Tìm kiếm audit log với filter: user, tenant, model, time range, action.

### II.5.9.6. Thành phần giao diện

| Tên control | Bắt buộc | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| User | Không | Select | User ID | – |
| Tenant | Không | Select | Tenant | – |
| Model | Không | Select | Model | – |
| Action | Không | Select | Action (chat/summarize/ocr/...) | – |
| From | Không | Datetime | Bắt đầu | – |
| To | Không | Datetime | Kết thúc | – |
| Latency > X ms | Không | Number | Filter latency | – |
| Status | Không | Select | success / error | – |

### II.5.9.7. Thiết kế hàm / thủ tục

```python
def record(event: AuditEvent):
    # Xử lý PII hash
    safe_prompt = hashlib.sha256(event.prompt.encode()).hexdigest()
    audit_log_repo.insert(AuditRow(
        user=event.user, tenant=event.tenant,
        action=event.action, model=event.model,
        prompt_hash=safe_prompt, response_hash=...,
        tokens=event.tokens, latency=event.latency,
        created_at=now()
    ))
    metrics_inc(event.action, event.model, event.tokens)
    span.set_attribute("ai.action", event.action)
```

---

## II.5.10. Phân hệ AI Admin Console

### II.5.10.1. Mô tả chức năng

Giao diện web dành cho **Admin Bộ/đơn vị** để quản trị toàn bộ hệ thống AI:

1. Quản lý Knowledge Source (xem II.5.4).
2. Quản lý Prompt Template (CRUD, version, A/B test).
3. Quản lý Agent (CRUD, tools binding).
4. Quản lý Tool (MCP tool registry).
5. Quản lý Provider (xem II.5.6).
6. Quản lý User & Permission cho AI (mapping role → AI capability).
7. Job monitor (xem II.5.7).
8. Audit search (xem II.5.9).
9. Model benchmark (RAGAS scores).
10. Budget / Cost dashboard.

### II.5.10.2. Luồng xử lý

Console là Next.js Web App, gọi các endpoint `/v1/admin/ai/*` của AI Platform API.

### II.5.10.3. Tài liệu liên quan

- Phần hướng dẫn sử dụng (sẽ viết trong tài liệu HDSD).

### II.5.10.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | Frontend: `admin-console` (Next.js 14, Ant Design) | – |
| 2 | Backend: `SmartOffice.AIPlatform.Api.Admin` | – |

### II.5.10.5. Thiết kế giao diện

Bao gồm **11 màn hình** tương ứng với **11 phân hệ**, mỗi màn hình có CRUD chuẩn + dashboard.

### II.5.10.6. Thành phần giao diện (tổng quát)

| Tên control | Bắt buộc | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| Top Nav | Có | Menu | Điều hướng chính | 11 tab phân hệ |
| Side bar | Có | Tree | Lọc theo tenant/dept | – |
| Data grid | Có | Table | Danh sách | Sort, filter, pagination |
| Form editor | Có | Form | CRUD | Theo schema từng phân hệ |
| Action button | Có | Button | Tác vụ nhanh | Approve/Reject/Retry |

### II.5.10.7. Thiết kế hàm / thủ tục

Console gọi REST API thông qua BFF; ủy quyền theo role `ai:admin`.

---

## II.5.11. Phân hệ Auth & Tenant cho AI

### II.5.11.1. Mô tả chức năng

Mở rộng module **Identity** hiện hữu của Backend SmartOffice, thêm khả năng:

1. Cấp phát **scope/permission riêng cho AI**: `ai:chat`, `ai:summarize`, `ai:admin`, `ai:agent:run`, ...
2. Tích hợp với **Tenant** (đơn vị) để cô lập dữ liệu AI.
3. **Row-Level Security** trên bảng `ai_*`.
4. Audit mọi request AI theo user, tenant.

### II.5.11.2. Luồng xử lý

```
Request → JWT verify → check scope (`ai:summarize`)
        → set session variable `app.tenant_id`
        → query PostgreSQL (RLS tự lọc theo tenant)
```

### II.5.11.3. Tài liệu liên quan

- `qlvb-backend-core/src/Services/Identity/...` (Identity service hiện hữu).
- `qlvb-backend-core/docs/Identity/9.1 Sequence Diagram Identity.md`.
- `qlvb-backend-core/docs/modules/Identity/login-flow.md`.

### II.5.11.4. Tên module

| STT | Tên module | Mô tả |
|---|---|---|
| 1 | `SmartOffice.Identity.Application.AI` | `GrantAiScopeCommand` |
| 2 | `SmartOffice.AIPlatform.Infrastructure.Auth` | JWT middleware, RLS context |
| 3 | `SmartOffice.AIPlatform.Infrastructure.RLS` | Tenant-aware query |

### II.5.11.5. Thiết kế giao diện (AI Admin Console – User Permission)

Màn hình mapping user/role → AI scope.

### II.5.11.6. Thành phần giao diện

| Tên control | Bắt buộc | Định dạng | Vai trò | Ghi chú |
|---|---|---|---|---|
| User / Role | Có | Select | User hoặc Role | – |
| AI scope | Có | Multi-select | Các quyền AI | chat, summarize, agent, admin |
| Tenant | Không | Select | Tenant được phép | – |
| Status | Có | Boolean | Đang active | – |

### II.5.11.7. Thiết kế hàm / thủ tục

```sql
-- Tạo policy cho bảng ai_job
ALTER TABLE ai_platform.ai_job ENABLE ROW LEVEL SECURITY;
CREATE POLICY ai_job_tenant_isolation ON ai_platform.ai_job
  USING (tenant_id = current_setting('app.tenant_id')::uuid);

-- Trước mỗi query, BE set:
SET app.tenant_id = '<tenant-uuid>';
```

```python
# Tại API layer:
async with tenant_context(request):
    return await ai_repo.search(query)  # chỉ thấy data của tenant
```

---

## Bảng tổng hợp 11 phân hệ AI

| Mã | Phân hệ | Mục đích chính | Phụ thuộc |
|---|---|---|---|
| II.5.1 | AI Gateway | Cổng nhận request AI | Identity, Provider, Audit |
| II.5.2 | AI Engine Core | Chạy RAG pipeline | Knowledge, Provider |
| II.5.3 | Agent Orchestration | Multi-step task | MCP, Provider, HITL |
| II.5.4 | Knowledge Base & RAG | Quản lý tri thức | Embedding, pgvector |
| II.5.5 | OCR & Document Extraction | Trích xuất văn bản scan | Files, OCR engines |
| II.5.6 | Provider Abstraction | Multi-LLM | OpenAI/Ollama/... |
| II.5.7 | AI Job & Task Queue | Async job | Kafka, Outbox |
| II.5.8 | MCP Server | Expose tool cho Agent | All services via MCP |
| II.5.9 | Audit & Observability | Truy vết & đo lường | ELK, Prometheus |
| II.5.10 | AI Admin Console | UI quản trị | All phân hệ trên |
| II.5.11 | Auth & Tenant cho AI | Phân quyền, RLS | Identity |

---

**HẾT TÀI LIỆU THIẾT KẾ CHỨC NĂNG — HỆ THỐNG AI PHỤC VỤ VĂN PHÒNG SỐ BỘ TÀI CHÍNH**
