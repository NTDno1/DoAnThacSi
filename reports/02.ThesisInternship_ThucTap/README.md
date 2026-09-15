# Báo cáo thực tập — Nguyễn Tiến Đạt

## Cấu trúc

- `chapters/`: 9 phần Markdown nguồn, từ bìa đến tài liệu tham khảo.
- `outputs/`: bản Markdown đã ghép và bản Word.
- `tools/merge_report.py`: ghép các chương theo thứ tự chuẩn.
- `tools/convert_to_docx.py`: chuyển bản Markdown tổng hợp sang DOCX.

## Tạo lại đầu ra

Chạy từ thư mục gốc repository:

```bash
python reports/02.ThesisInternship_ThucTap/tools/merge_report.py
python reports/02.ThesisInternship_ThucTap/tools/convert_to_docx.py
```

Script chuyển Word cần gói `python-docx`. Chỉ sửa nội dung trong `chapters/`; file tại `outputs/` là sản phẩm được sinh.
