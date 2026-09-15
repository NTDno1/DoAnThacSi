# Hướng dẫn đọc và làm việc với dự án

## Điểm vào bắt buộc

Đọc theo thứ tự: `README.md` → `docs/00.INDEX.md` → tài liệu theo phạm vi công việc. Nếu thay đổi runtime, đối chiếu mã thật trong `src/` và cấu hình trong `docker/`; nhiều tài liệu kiến trúc mô tả trạng thái đích chứ không đảm bảo đã được cài đặt.

## Nguồn chuẩn theo phạm vi

- Backend: `src/backend/src/AIBaseFramework.API/`.
- Frontend: `src/frontend/`.
- Hạ tầng local: `docker/`.
- Tài liệu kỹ thuật: `docs/`.
- Kế hoạch triển khai: `planning/`.
- Báo cáo đang dùng: `reports/`.
- Nội dung lịch sử/thử nghiệm: `archive/`; không lấy làm implementation hiện hành.

Không tạo lại các thư mục cũ `PlanTrienKhaiVaCode/`, `BaoCaoDeAn/`, `BaoCaoThucTap_Dat/` hoặc `BaoCaoTKTC_AI_BTC/`.

## Quy ước báo cáo

- Chỉnh nội dung tại `reports/<report>/chapters/`.
- File trong `outputs/` là sản phẩm được sinh; không sửa tay nếu thay đổi có thể thực hiện từ nguồn.
- Script hỗ trợ tài liệu nằm trong `tools/`; script kiểm tra nằm trong `verify_scripts/`.
- Mẫu hoặc tài liệu đầu vào đặt tại `references/`.
- Bản cũ và script thử nghiệm chỉ được giữ trong `archive/`.

## Kiểm tra theo phạm vi

```bash
dotnet build src/backend/AIBaseFramework.sln
npm --prefix src/frontend run build
docker compose -f docker/docker-compose.yml config
python reports/01.ThesisMaster_DeAn/tools/merge_report.py
python reports/02.ThesisInternship_ThucTap/tools/merge_report.py
python reports/03.ThesisTKTC_AI_BTC/tools/merge_report.py
python reports/03.ThesisTKTC_AI_BTC/verify_scripts/verify_drawio.py
```

Chỉ chạy kiểm tra liên quan đến phần đã sửa. Trước khi kết luận lỗi mới, đọc `docs/12.AUDIT/DIEM_BAT_HOP_LY.md` vì backend và frontend đã có các lỗi prototype được ghi nhận từ trước.
