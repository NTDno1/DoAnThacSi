# Kho lưu trữ

Thư mục này giữ nội dung không còn là nguồn hiện hành nhưng cần bảo toàn để đối chiếu:

- `old_reports/`: biến thể báo cáo cũ hoặc trùng lặp.
- `old_docx_versions/`: phiên bản Word đã được thay thế.
- `reference_pdfs/`: tài liệu bên ngoài dùng làm mẫu/tham khảo.
- `tools/`: script thử nghiệm, script đã thay thế và dữ liệu trung gian do công cụ/AI tạo.

Không dùng đường dẫn hoặc script trong `archive/` cho luồng build hiện hành. Một số script cũ vẫn giữ đường dẫn tuyệt đối lịch sử để bảo toàn ngữ cảnh; muốn dùng lại cần chủ động hiện đại hóa và chuyển sang `reports/<report>/tools/`.
