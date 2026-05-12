# BÁO CÁO CÁC ĐIỂM BẤT HỢP LÝ TRONG DỰ ÁN

**Dự án:** Hệ thống Tìm kiếm và Hỏi đáp Tài liệu Nội bộ (Semantic Search + RAG)
**Ngày rà soát:** 2026-05-11
**Phạm vi:** Toàn bộ workspace `DoAnThacSi/` (docs, infrastructure, backend .NET, frontend Next.js)

> Mức độ: 🔴 Nghiêm trọng (khiến build/run fail) · 🟠 Cao (mâu thuẫn thiết kế) · 🟡 Trung bình (gây nhầm lẫn/debt) · 🔵 Thấp (cosmetic)

---

## 1. Khủng hoảng nhận dạng dự án — 🟠 Cao

Cùng một dự án nhưng được đặt **ít nhất 4 tên khác nhau**, rải rác ở nhiều nơi:

| Tên dùng | Xuất hiện ở |
|----------|-------------|
| `DoAnThacSi` | Tên repo gốc |
| `HoaLienVien` (prefix `hlv_`) | `/docker/docker-compose.yml`, `/docker/Dockerfile.backend` (ENTRYPOINT `HoaLienVien.API.dll`), `.sln` (project test `HoaLienVien.Tests`), `docker-compose.prod.yml` container names |
| `AIBaseFramework` (prefix `aibf_`) | `/PlanTrienKhaiVaCode/02.INFRASTRUCTURE/docker/docker-compose.yml`, toàn bộ C# namespace (`AIBaseFramework.API.*`), `.csproj`, `appsettings.json` |
| `DocQA` | `/docs/modules/03.MODULES.md` dùng namespace `DocQA.Domain.Entities` để minh họa |

**Hệ quả:** Không biết tên chính thức là gì, container name trong prod khác dev, docs mô tả namespace khác code thật.

**Đề xuất:** Chọn một tên (khuyến nghị `AIBaseFramework` vì code thực tế đã dùng) và đổi đồng bộ tất cả.

---

## 2. Hai bộ Docker Compose trùng lặp và mâu thuẫn — 🔴 Nghiêm trọng

Repo có **2 thư mục docker** với nội dung khác nhau:

| Điểm | `/docker/` (gốc) | `/PlanTrienKhaiVaCode/02.INFRASTRUCTURE/docker/` |
|------|------------------|--------------------------------------------------|
| Container prefix | `hlv_` | `aibf_` |
| Database name | `hoalienvien` | `aibaseframework` |
| DB user | `hlvuser` / `hlvpass123` | `aibfuser` / `aibfpass123` |
| Dockerfile COPY | `src/Be/HoaLienVien.API/...` | `src/AIBaseFramework.API/...` |
| ENTRYPOINT | `HoaLienVien.API.dll` | `AIBaseFramework.API.dll` |
| Volume name | `hlv_postgres_data` | `hlv_postgres_data` ❗ (của file aibf nhưng ghi `hlv_` — có volume bị ghi nhầm prefix) |

Ngoài ra trong `/PlanTrienKhaiVaCode/02.INFRASTRUCTURE/docker/docker-compose.yml`:

```yaml
dockerfile: ../../DoAnThacSI/PlanTrienKhaiVaCode/02.INFRASTRUCTURE/docker/Dockerfile.backend
```

- Viết sai tên thư mục: `DoAnThacSI` (I hoa) — thực tế là `DoAnThacSi`.
- Kết hợp với `context: ../03.BACKEND` thì đường dẫn Dockerfile trỏ ra ngoài workspace, không tồn tại.

**Đề xuất:** Xóa một trong hai bộ. Nếu code thực ở `/PlanTrienKhaiVaCode/03.BACKEND/` thì xóa hẳn `/docker/` gốc.

---

## 3. Backend không biên dịch được — 🔴 Nghiêm trọng

### 3.1 `Program.cs` — sử dụng `builder` trước khi khai báo

```csharp
// Dòng 9
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)   // ❌ builder chưa tồn tại
    ...

try
{
    ...
    var builder = WebApplication.CreateBuilder(args);  // Khai báo ở dòng 15
```

→ Lỗi CS0103: The name 'builder' does not exist in the current context.

### 3.2 Tham chiếu class không tồn tại

Trong `Program.cs`:

- `app.UseMiddleware<ExceptionHandlingMiddleware>();` — **không có file nào** định nghĩa class này.
- `new HangfireAuthorizationFilter()` — **không có file nào** định nghĩa class này.
- `using AIBaseFramework.API.Common.Middleware;` và `AIBaseFramework.API.Common.Extensions;` — namespace trống.

### 3.3 Đăng ký DI cho service chưa có implementation

```csharp
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IChatService, ChatService>();
```

→ Chỉ có `AuthService.cs`. `DocumentService`, `SearchService`, `ChatService` **chưa được viết**, nhưng controller đã inject → runtime sẽ ném `InvalidOperationException` ngay khi start.

### 3.4 `AppDbContext.cs` thiếu namespace

```csharp
// AppDbContext.cs
using AIBaseFramework.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

public class AppDbContext : DbContext   // ❌ Không có namespace → nằm ở global
```

Nhưng `Program.cs` lại `using AIBaseFramework.API.Infrastructure.Data;` → không tìm thấy `AppDbContext`.

Đồng thời, file này còn định nghĩa `RefreshToken`, `DocumentCategory`, `SearchLog`, `AuditLog`, `SystemSetting` **ở global namespace**, gây xung đột với các entity khác dùng namespace `AIBaseFramework.API.Domain.Entities`.

### 3.5 `RAGPipeline.cs` định nghĩa lại entity

File này **redefine** cả `Document` và `DocumentStatus`:

```csharp
// Cuối RAGPipeline.cs
public class Document
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DocumentStatus Status { get; set; }
}

public enum DocumentStatus { Pending, Processing, Indexed, Failed, Deleted }
```

Trong khi đã có `Domain/Entities/Document.cs` với đầy đủ fields. → Duplicate type definition, compile error hoặc ambiguous reference.

### 3.6 `MinIOService.cs` — kiểu trả về sai cú pháp

```csharp
public interface IMinIOService
{
    Task<void> DeleteFileAsync(string objectName);   // ❌ Task<void> không hợp lệ
```

Phải là `Task DeleteFileAsync(...)`. Lỗi compile.

### 3.7 `RAGPipeline.cs` — logic similarity **ngược chiều**

```csharp
.OrderByDescending(c => c.Embedding.CosineDistance(embeddingStr))
.Take(topK)
```

`CosineDistance` trả về: 0 = giống hệt, 2 = đối nghịch. `OrderByDescending` sẽ lấy **những chunk xa nhất** đứng đầu. → RAG trả về kết quả **không liên quan nhất**.

Phải là `OrderBy(c => c.Embedding.CosineDistance(...))` hoặc chuyển sang `CosineSimilarity` + `OrderByDescending`.

### 3.8 Thiếu package `Pgvector.EntityFrameworkCore`

Code gọi `c.Embedding.CosineDistance(embeddingStr)` — đây là extension method của package `Pgvector.EntityFrameworkCore`. `.csproj` chỉ có `Npgsql.EntityFrameworkCore.PostgreSQL` và `NetTopologySuite`, **không** có Pgvector → không compile.

### 3.9 `Ollama` — endpoint và payload sai

```csharp
// OllamaService.cs
var request = new OllamaEmbedRequest { Model = ..., Input = text };
await _httpClient.PostAsJsonAsync($"{_baseUrl}/api/embeddings", request);
```

Ollama API có 2 endpoint:
- `/api/embeddings` (cũ): nhận field `prompt`.
- `/api/embed` (mới): nhận field `input`.

Code gửi field `input` vào endpoint cũ → Ollama trả về embedding rỗng.

Ngoài ra, response JSON của Ollama dùng tên field **snake_case/lowercase** (`model`, `embedding`, `message`), nhưng DTO C# không có `[JsonPropertyName]`, chế độ mặc định của `System.Text.Json` là case-insensitive nhưng **không** tự lowercase camelCase → cần cấu hình `JsonSerializerOptions`. Hiện `PostAsJsonAsync` dùng options mặc định, serialize property như `Model` thành `"Model"` (Pascal) chứ không `"model"`. Ollama sẽ không nhận ra.

### 3.10 `.sln` trỏ sai đường dẫn

```
Project(...) = "AIBaseFramework", "src\AIBaseFramework\AIBaseFramework.API.csproj", "..."
```

Thực tế file nằm ở `src\AIBaseFramework.API\AIBaseFramework.API.csproj`. Ngoài ra solution tham chiếu `tests\HoaLienVien.Tests\HoaLienVien.Tests.csproj` — **thư mục và file này không tồn tại**.

### 3.11 `.csproj` có dependency thừa

```xml
<PackageReference Include="AWSSDK.S3" Version="3.7.305.9" />
<PackageReference Include="Minio" Version="6.0.1" />
```

Code chỉ dùng `Minio.*`. AWSSDK.S3 là dead weight (~10MB).

---

## 4. Frontend không biên dịch / render được — 🔴 Nghiêm trọng

### 4.1 `layout.tsx` — xung đột Client/Server Component

```tsx
'use client';

export const metadata: Metadata = { ... };   // ❌ Client Component không được export metadata
```

Next.js App Router: chỉ **Server Component** mới export được `metadata`. Khi đánh dấu `'use client'` thì `metadata` bị ignore + cảnh báo/ lỗi build tuỳ phiên bản.

### 4.2 `page.tsx` — dùng hook trong Server Component

```tsx
// Không có 'use client'
export default function HomePage() {
  const { user, isAuthenticated } = useAuthStore();   // ❌ Hook chỉ dùng được ở client
```

→ Runtime error: "useState is not available on the server".

### 4.3 `authStore.ts` — thiếu dependencies được cài đặt

Diagnostics báo:
- Cannot find module `'zustand'`
- Cannot find module `'zustand/middleware'`
- Parameter `set` / `state` implicitly has `'any'` type.

Mặc dù `package.json` có khai báo `zustand`, nhưng `node_modules` chưa được cài, và project chưa có `tsconfig.json` để TypeScript nhận biết types.

### 4.4 Lưu JWT trong `localStorage`

```ts
localStorage.setItem('accessToken', accessToken);
localStorage.setItem('refreshToken', refreshToken);
```

Dễ bị XSS đọc token. Trong khi docs `02.SYSTEM_ARCHITECTURE.md` và `03.MODULES.md` viết rõ: dùng **httpOnly cookie**. → Code đi ngược design.

### 4.5 Next.js `rewrites` trùng với backend path

```js
source: '/api/:path*',
destination: `${API_URL}/api/:path*`
```

Nhưng `apiClient` (axios) lại `baseURL: 'http://localhost:5000'` + gọi `/api/v1/auth/...` trực tiếp → rewrites này **không được dùng** (axios gọi thẳng tới backend), cấu hình thừa và có thể gây CORS preflight khác giữa dev/prod.

### 4.6 Thiếu file cấu hình

Dựa vào `list_directory`, frontend chỉ có `package.json`, `next.config.js`, `tailwind.config.js`. **Thiếu**: `tsconfig.json`, `postcss.config.js`, `globals.css` (bị import ở `layout.tsx`), `components/providers.tsx` (được import nhưng chưa verify nội dung).

---

## 5. SQL khởi tạo DB bị lỗi cú pháp — 🔴 Nghiêm trọng

File `/PlanTrienKhaiVaCode/02.INFRASTRUCTURE/docker/init.sql`:

```sql
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pg_trgm";
CREATE EXTENSION IF NOT EXISTS IF NOT EXISTS "vector";   -- ❌ Duplicate "IF NOT EXISTS"
```

→ PostgreSQL sẽ báo syntax error và **bỏ qua toàn bộ phần sau** → DB không được khởi tạo khi container start.

### 5.1 Password hash seed là giả

```sql
INSERT INTO users (email, password_hash, ...) VALUES (
  'admin@yourcompany.com',
  '$2a$11$rS.h.yQZ6R5eZKJZqXqGeO3VZrGvGvGvGvGvGvGvGvGvGvGvGvGv', -- admin123
  ...
```

Chuỗi này **không phải** BCrypt hash hợp lệ của `"admin123"` — nó chỉ là placeholder pattern. → Không ai login được với default account. Trong khi DEPLOYMENT_PLAN.md hướng dẫn cụ thể "Login với admin@... / admin123".

### 5.2 Embedding dimension không khớp tài liệu

- `init.sql`: `embedding VECTOR(768)`
- `02.SYSTEM_ARCHITECTURE.md`: `1536/3072 dim` (giá trị của OpenAI).
- `DocumentChunk.cs`: 768 (nomic-embed-text).

Mâu thuẫn. Nếu đổi model embedding (ví dụ sang `mxbai-embed-large` = 1024) là bắt buộc migrate schema.

---

## 6. Mô hình "100% offline" không nhất quán — 🟠 Cao

Docs khẳng định **không dùng OpenAI**, nhưng:

| Chỗ nào | Vẫn nói OpenAI / GPT |
|---------|----------------------|
| `README.md` (gốc) Tech stack | Ghi `FastAPI + Python 3.11` làm API Gateway (thực tế code là .NET 8) |
| `docs/modules/03.MODULES.md` | "LLM API: **GPT-4o**", "OpenAI API / Local model" |
| `docs/architecture/02.SYSTEM_ARCHITECTURE.md` | Có cả nhánh Ollama và ghi chú dimensions OpenAI |
| `docs/roadmap/07.ROADMAP_AND_TASKS.md` | Task "OpenAI Embedding API Integration", "GPT-4o Integration" |
| `/docker/docker-compose.yml` (gốc) | Có biến `OpenAiConfig__ApiKey: ""` |
| `appsettings.json` | Không có OpenAI (đúng), nhưng dùng `MinIOConfig` |
| `docker-compose.yml` (aibf) | Bind `MinIOConfig__*` (Pascal chữ IO) |
| `docker-compose.yml` (/docker/ gốc) | Bind `MinioConfig__*` (Pascal chữ io) — **capitalization khác nhau** |

→ Ảnh hưởng cụ thể: key binding của .NET phân biệt hoa/thường ở section name; hai compose file sẽ bind vào hai configuration section khác nhau, 1 trong 2 sẽ **silent-fail** (service lấy default).

---

## 7. Bảo mật / RBAC thiết kế ≠ code — 🟠 Cao

`03.MODULES.md` mô tả chi tiết:
- Account lockout sau 5 lần sai password.
- Rate limiting theo IP.
- Email confirmation bắt buộc mới active.
- Refresh token rotation family (revoke cả họ khi phát hiện reuse).
- Password policy min 8 ký tự, phức tạp.
- Pepper phía server khi hash password.
- RBAC với bảng `Role`, `Permission`, `RolePermission`, `UserRole` (many-to-many).

Code thực tế:
- `AuthService.LoginAsync` không có lockout, không tăng `FailedLoginAttempts`, không rate-limit.
- Không gửi email confirmation.
- `RefreshTokenAsync` revoke token cũ, **không** revoke cả family.
- Không validate password strength.
- Không có bảng Role/Permission; `User.Role` chỉ là enum đơn — không thể dynamic RBAC như design vẽ.

Đồng thời `README.md` còn thêm tính năng **Auto-DB Query (RAG-SQL)** với RBAC chi tiết theo từng bảng/cột — **không có bất kỳ dòng code nào** triển khai điều này.

### 7.1 JWT claim đọc không đồng bộ

- `AuthService` tạo claim với key `"userId"`.
- Controller đọc: `User.FindFirst("userId")`.
- Chuẩn JWT chuyên dụng là `JwtRegisteredClaimNames.Sub` — code không dùng.

Nếu sau này add middleware / policy handler thứ ba chuẩn hoá theo `sub` sẽ không lấy được user id.

### 7.2 `AuthController` bắt `Exception` tổng quát

```csharp
catch (Exception ex) {
    return Unauthorized(new { message = "Email hoặc password không đúng" });
}
```

Một lỗi DB down, connection timeout, NullReferenceException... đều trả 401 "sai mật khẩu" → debug cực khó, sai logic HTTP.

### 7.3 `JwtConfig.Secret` default nhạy cảm

```json
"Secret": "your-256-bit-secret-key-minimum-32-characters-here"
```

Đã commit vào repo. Cần loại bỏ default và bắt buộc set qua env.

---

## 8. CORS cấu hình xung đột — 🟡 Trung bình

`Program.cs`:

```csharp
policy.WithOrigins(allowedOrigins)
  .AllowAnyHeader()
  .AllowAnyMethod()
  .AllowCredentials()   // ❌
  .WithExposedHeaders("Content-Disposition");
```

Kết hợp `AllowCredentials()` + cookies (design yêu cầu) + frontend `localStorage` (code thực) → cấu hình không thống nhất mục đích. Nếu dùng Bearer header thì không cần `AllowCredentials`; nếu dùng cookie thì frontend phải set `withCredentials: true` (không thấy trong `api.ts`).

---

## 9. Nhất quán giữa tài liệu và thực tế — 🟠 Cao

| Docs nói | Thực tế code |
|----------|--------------|
| Backend = FastAPI + Python 3.11 (README gốc) | .NET 8 ASP.NET Core |
| OCR = PaddleOCR | Không có code OCR nào |
| Hangfire dashboard có authorization filter | Filter chưa implement |
| CQRS + MediatR | MediatR được register nhưng **không có** command/query/handler nào được viết |
| Clean Architecture 4 lớp | Thực tế chỉ 1 project API, các "lớp" chỉ là folder trong cùng assembly |
| Tests project | `.sln` tham chiếu `HoaLienVien.Tests` nhưng file không tồn tại |
| GitHub Actions CI/CD | Không có `.github/workflows/` |
| Chunking service, semantic chunking | Chưa có code |
| PDF/DOCX extraction (iText, OpenXml) | Packages được install nhưng không có service sử dụng |
| Background job Hangfire | Register rồi, nhưng không có job nào được enqueue |

→ Phần lớn "tính năng" chỉ tồn tại trên giấy.

---

## 10. Tổ chức thư mục trùng lặp — 🟡 Trung bình

- `/docs/` (doc tổng) **và** `/PlanTrienKhaiVaCode/05.DOCUMENTATION/ARCHITECTURE.md` (doc con).
- `/docker/` (gốc) **và** `/PlanTrienKhaiVaCode/02.INFRASTRUCTURE/docker/`.
- Nhiều README: `/README.md` (1483 lines), `/docker/README.md`, `/PlanTrienKhaiVaCode/README.md`, `/PlanTrienKhaiVaCode/02.INFRASTRUCTURE/README.md`, `/PlanTrienKhaiVaCode/03.BACKEND/README.md`.

Bảo trì song song sẽ sinh drift ngày càng lớn. Phần lớn các mâu thuẫn trong mục 1–9 đến từ đây.

---

## 11. Lỗi nhỏ khác — 🔵 Thấp

- `AppDbContext` có wrapper `HasPostgresExtension` vô nghĩa, chỉ gọi lại chính method có sẵn.
- `Document.cs` có `using AIBaseFramework.API.Domain.Entities;` **tự tham chiếu chính namespace của nó** — redundant, không fail nhưng thừa.
- `ChatMessage.Citations` và `SourcesUsed` lưu JSON string trong field string — không dùng JsonDocument / JsonElement → không có validation và IntelliSense.
- `init.sql` dùng `'vietnamese'` làm text search configuration nhưng PostgreSQL **không** có config này mặc định → cần `CREATE TEXT SEARCH CONFIGURATION` trước.
- `docker-compose.yml` (aibf dev) volume tên `postgres_data` nhưng đặt name là `hlv_postgres_data` (dính prefix của tên khác).
- `package.json` không khai báo Node version engines.
- Frontend không có `react-query` provider rõ ràng (đã install `@tanstack/react-query` nhưng chưa thấy QueryClientProvider).
- `Dockerfile.frontend` có lệnh `COPY --from=builder /app/.next /usr/share/nginx/html` — Next.js `output: 'standalone'` cần chạy Node, không serve static qua nginx như vậy. Build artifact sẽ thiếu server bundle → trang không render.

---

## Tóm tắt ưu tiên sửa

### Phải sửa trước khi build/run (🔴)
1. Sửa `Program.cs` (`builder` thứ tự), tạo 2 class thiếu (`ExceptionHandlingMiddleware`, `HangfireAuthorizationFilter`) hoặc xoá tham chiếu.
2. Viết hoặc tạm remove `DocumentService`, `SearchService`, `ChatService` khỏi DI.
3. Gộp/xoá định nghĩa `Document`/`DocumentStatus` trùng ở `RAGPipeline.cs`.
4. Đưa `AppDbContext` vào namespace `AIBaseFramework.API.Infrastructure.Data`.
5. Sửa `Task<void>` → `Task` trong `MinIOService`.
6. Cài `Pgvector.EntityFrameworkCore` và cấu hình type mapping cho cột vector.
7. Sửa `CREATE EXTENSION IF NOT EXISTS IF NOT EXISTS` trong `init.sql`.
8. Sửa `OllamaService` — chọn endpoint đúng (`/api/embed`), thêm `[JsonPropertyName]` lowercase.
9. Đảo `OrderBy` vs `OrderByDescending` cho cosine distance.
10. Sửa `.sln` đường dẫn project, xoá tham chiếu test project không tồn tại.
11. Next.js: tách `'use client'` cho component dùng hook; đặt `metadata` ở Server Component.
12. Cài `node_modules` cho frontend, thêm `tsconfig.json`.

### Phải sửa trước khi demo/nộp (🟠)
13. Chọn một tên dự án và đồng bộ hoá.
14. Giữ **một** bộ Docker Compose, xoá bộ còn lại.
15. Cập nhật tất cả docs bỏ OpenAI/GPT/FastAPI, chỉ còn Ollama + .NET.
16. Hash lại password seed đúng BCrypt để default account login được.
17. Quyết định lưu token bằng httpOnly cookie hay localStorage — sửa đồng bộ code + docs.
18. Giảm quy mô tính năng trong docs (RBAC động, RAG-SQL, OCR, CQRS) xuống đúng những gì thực sự triển khai; hoặc commit code để match docs.

### Nên sửa (🟡 / 🔵)
19. Thống nhất casing `MinIOConfig` / `MinioConfig`.
20. Thêm CI workflow thực sự.
21. Dọn package thừa (`AWSSDK.S3`).
22. Thống nhất một README root duy nhất, các README con chỉ trỏ link.

---

*Lưu ý:* Báo cáo này dựa trên đọc tĩnh source code và docs, không chạy build. Sau khi sửa nhóm 🔴, chạy `dotnet build` và `npm run build` sẽ lộ thêm lỗi phụ thuộc khác.
