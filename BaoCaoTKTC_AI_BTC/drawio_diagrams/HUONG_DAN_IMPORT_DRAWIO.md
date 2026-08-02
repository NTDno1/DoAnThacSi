# Hướng dẫn Import vào draw.io Web

## Vấn đề

Nếu file `.drawio` không mở được trên **draw.io web** (https://app.diagrams.net), có thể do:

1. **Trình duyệt chặn load file local** (bảo mật CORS)
2. **draw.io web chỉ đọc `.drawio` chứ không phải `.xml`**
3. **Extension `.drawio` bị browser coi là text** (một số browser)

## ✅ Cách 1 — Kéo thả (đơn giản nhất)

1. Mở **File Explorer**
2. **Kéo file `.drawio`** từ folder `BaoCaoTKTC_AI_BTC\drawio_diagrams\` vào **cửa sổ draw.io web**
3. Sơ đồ sẽ hiển thị ngay

## ✅ Cách 2 — File menu

1. Vào **draw.io web** (https://app.diagrams.net)
2. Chọn **File → Open from Device** (Ctrl+O)
3. Browse tới folder `BaoCaoTKTC_AI_BTC\drawio_diagrams\`
4. **Quan trọng**: chọn **All Files** thay vì chỉ `.xml` (vì `.drawio` không phải extension chuẩn của browser)
5. Chọn file muốn mở

## ✅ Cách 3 — Paste nội dung XML (chắc chắn nhất)

Nếu 2 cách trên không được:

1. Mở file `.drawio` bằng **Notepad** (hoặc VS Code)
2. **Ctrl+A** → **Ctrl+C** (copy toàn bộ nội dung XML)
3. Vào **draw.io web**
4. Chọn **Extras → Edit Diagram** (hoặc **File → New...**)
5. Click vào **"Drawio" tab** (không phải "Shape" tab)
6. **Ctrl+V** (paste nội dung XML)
7. Sơ đồ sẽ render ngay

## ✅ Cách 4 — Đổi extension sang `.xml` (nếu browser từ chối)

1. Đổi tên `01_architecture_tong_the_7_tang.drawio` → `01_architecture_tong_the_7_tang.xml`
2. Mở bằng **drag-and-drop** vào draw.io web
3. draw.io sẽ tự nhận dạng là mxGraphModel XML

## ✅ Cách 5 — Dùng draw.io Desktop (chắc chắn 100%)

Nếu các cách trên không được:

1. Tải **draw.io desktop**: https://github.com/jgraph/drawio-desktop/releases
2. Cài đặt
3. Mở bằng menu **File → Open** → chọn file `.drawio`
4. Sơ đồ sẽ mở được **100% không lỗi**

## Cách kiểm tra file đúng chuẩn

Mở file `.drawio` bằng Notepad, phải thấy:

```xml
<?xml version="1.0" encoding="UTF-8"?>
<mxfile host="app.diagrams.net" ...>
  <diagram id="..." name="...">
    <mxGraphModel ...>
      <root>
        <mxCell id="1" parent="0"/>   ← QUAN TRỌNG: dòng này phải có
        <mxCell id="..." value="..." style="..." vertex="1" parent="1">   ← tất cả shape phải parent="1"
        ...
      </root>
    </mxGraphModel>
  </diagram>
</mxfile>
```

## File PNG thay thế (nếu vẫn không import được `.drawio`)

Folder `png_diagrams/` chứa **7 file PNG** chất lượng cao, dùng được luôn trong Word/PowerPoint. Nếu bạn cần chỉnh sửa sơ đồ, hãy dùng PNG làm reference và vẽ lại trong draw.io.

## Liên hệ

Nếu vẫn gặp lỗi, báo lại **screenshot lỗi** và tôi sẽ fix chính xác.