# AI Base Framework — Enterprise AI Platform

Đây là workspace nghiên cứu và prototype cho nền tảng AI doanh nghiệp: tìm kiếm ngữ nghĩa, RAG, AI Agent, MCP, tích hợp hệ thống và triển khai LLM cục bộ.

> Trạng thái hiện tại: dự án đang ở mức prototype/nghiên cứu. Tài liệu kiến trúc mô tả cả trạng thái đích; không phải mọi chức năng trong tài liệu đều đã được hiện thực hóa. Các lỗi build đã biết được ghi tại [báo cáo kiểm toán](docs/12.AUDIT/DIEM_BAT_HOP_LY.md).

## Bắt đầu đọc dự án

1. [Chỉ mục tài liệu](docs/00.INDEX.md) — bản đồ toàn bộ tài liệu.
2. [Tổng quan dài](docs/00.PROJECT_OVERVIEW.md) — bối cảnh, kiến trúc và tầm nhìn chi tiết.
3. [Kiến trúc hệ thống](docs/architecture/02.SYSTEM_ARCHITECTURE.md) — thiết kế kỹ thuật.
4. [Kế hoạch triển khai](planning/README.md) — roadmap và checklist.
5. [Quy ước cho AI](AGENTS.md) — nguồn chuẩn, ranh giới thư mục và cách kiểm tra.

## Bố cục chuẩn

| Thư mục | Nội dung | Quy ước |
|---|---|---|
| `src/backend/` | ASP.NET Core .NET 8, API và các thành phần AI | Mã nguồn backend duy nhất |
| `src/frontend/` | Next.js 14, React và giao diện | Mã nguồn frontend duy nhất |
| `docker/` | Docker Compose, Dockerfile, SQL và script vận hành | Hạ tầng local hiện hành |
| `docs/` | Phân tích, kiến trúc, API, database, flow và tầm nhìn | Tài liệu kỹ thuật chính |
| `planning/` | Kế hoạch 30 ngày, timeline và checklist | Tài liệu quản lý triển khai |
| `reports/` | Báo cáo học thuật/nghiệp vụ, nguồn và đầu ra | Sửa `chapters/`, không sửa tay `outputs/` |
| `archive/` | Bản cũ, PDF tham khảo, script thử nghiệm | Chỉ tham khảo, không dùng làm nguồn hiện hành |
| `tests/` | Điểm đặt kiểm thử | Hiện chưa có test case |
| `deployment/` | Điểm đặt Kubernetes/Helm tương lai | Docker hiện nằm tại `docker/` |

## Kiểm tra nhanh

```bash
# Backend
dotnet build src/backend/AIBaseFramework.sln

# Frontend
npm --prefix src/frontend install
npm --prefix src/frontend run build

# Cấu hình Docker
docker compose -f docker/docker-compose.yml config
```

Các lệnh build có thể phản ánh lỗi prototype đã được ghi trong tài liệu kiểm toán; việc sắp xếp thư mục không tự ý thay đổi logic ứng dụng.

## Báo cáo và file do công cụ sinh

Mỗi báo cáo dùng cùng một quy ước:

- `chapters/`: nội dung nguồn có thể chỉnh sửa.
- `outputs/`: Markdown tổng hợp, DOCX/PDF đã sinh.
- `tools/`: script ghép/chuyển đổi/vẽ do AI hoặc công cụ hỗ trợ tạo.
- `verify_scripts/`: script kiểm tra đầu ra, nếu báo cáo cần.
- `references/`: mẫu và tài liệu tham chiếu riêng của báo cáo.

Xem [chỉ mục báo cáo](reports/README.md) để biết lệnh tạo lại từng tài liệu.
