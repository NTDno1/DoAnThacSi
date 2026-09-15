**HỌC VIỆN CÔNG NGHỆ BƯU CHÍNH VIỄN THÔNG** 

**ĐỀ CƯƠNG ĐỀ ÁN TỐT NGHIỆP THẠC SĨ KỸ THUẬT ể ỏ Phát hiện và theo dõi vật th nh , ể ể ơ chuy n động nhanh trong video th thao đ n camera** 

Chuyên ngành: Khoa học Máy tính  ·  Mã số: 8.48.01.01 

**Học viên:** Nguyễn Thanh Phong 

**Người hướng dẫn khoa học:** TS. Đào Thị Thúy Quỳnh 

Hà Nội · 2026 

##### **I . 1 M Ở ĐẦU** 

# **Lý do chọn đề� tài** 

Tốc độ bóng/cầu, tần suất đánh bóng và vị trí di chuyển là dữ liệu quan trọng cho huấn luyện và phân tích chiến thuật trong tennis, cầu lông. 

Các hệ thống chuyên nghiệp (Hawk-Eye…) dùng đa camera tốc độ cao + cảm biến chuyên dụng — chi phí rất lớn, chỉ phù hợp giải đấu đỉnh cao. 

**DỰ ÁN NỀN: MATCH ANALYSIS** 

Đã có: phát hiện người chơi (YOLO), phát hiện bóng (YOLO fine-tuned), keypoints sân (CNN/ResNet), quy đổi minicourt, ước lượng tốc độ, xuất video kết quả. 

###### **NÚT THẮT CỐT LÕI** 

Học sâu trong computer vision (YOLO, Faster R-CNN, DETR/RT-DETR) mở ra khả năng phân tích video chỉ với một camera thông thường, không cần phần cứng đặc thù. 

Độ chính xác phát hiện bóng/cầu — vật thể rất nhỏ, di chuyển tốc độ cao — vẫn là điểm yếu, giới hạn toàn bộ pipeline phân tích. 

Đề cương Đề án Tốt nghiệp Thạc sĩ Kỹ thuật · PTIT 

2 

##### **I . 2 M Ở ĐẦU** 

# **Tổng quan về� vấ�n đề� nghiền cứu** 

Với đầu vào chỉ là video một camera thông thường, làm sao phát hiện chính xác vật thể rất nhỏ, di chuyển nhanh (bóng/cầu), đồng thời giữ ổn định theo dõi người chơi và ước lượng đúng thông số chuyển động? 

**1** 

#### **Kích thước cực nhỏ** 

Bóng/cầu chiếm rất ít pixel trong khung hình, dễ bị bỏ sót. 

**2** 

#### **Motion blur** 

Tốc độ di chuyển cao gây nhoè ảnh, khó nhận diện đặc trưng. 

**3** 

#### **Che khuấ�t / mấ�t dấ�u** 

Vật thể có thể biến mất qua nhiều frame liên tiếp. 

**4** 

#### **Sai số� lan truyề�n** 

Độ chính xác tốc độ phụ thuộc chất lượng detection + keypoints sân. 

_TrackNet (heatmap học sâu, dùng chuỗi frame) đã chứng minh hướng đi này khả thi; nghiên cứu gần đây kết hợp YOLO + mô hình bóng chuyên biệt + keypoints sân — nhưng phát hiện bóng vẫn kém ổn định nhất. Đề tài so sánh có hệ thống nhiều họ object detection và temporal modeling để tìm cấu hình tối ưu._ 

Đề cương Đề án Tốt nghiệp Thạc sĩ Kỹ thuật · PTIT 

3 

**I . 3 M Ở ĐẦU** 

# **ứ Mục đích nghiền c u** 

Xây dựng pipeline phân tích video tennis/cầu lông tự động (nhận diện, theo dõi, tính thông số trận đấu) trên video quay bằng một camera thông thường — đồng thời xác định có căn cứ khoa học cấu hình mô hình detection/tracking phù hợp nhất cho từng thành phần. 

Phát hiện & theo dõi hai người chơi Phát hiện bóng tennis / cầu lông theo frame Nội suy vị trí bóng khi mất detection Phát hiện keypoints sân (mặt phẳng quy chiếu) Ánh xạ người chơi & bóng lên mini-court Xác định thời điểm đánh bóng Tính tốc độ bóng, tốc độ người chơi So sánh nhiều mô hình object detection 

Đề cương Đề án Tốt nghiệp Thạc sĩ Kỹ thuật · PTIT 

4 

##### **I . 4 M Ở ĐẦU** 

# **Đố�i t ứ ượng và phạm vi nghiền c u** 

**Đối tượng nghiên cứu:** các họ object detection (YOLO, Detection Transformer, two-stage), mô hình segmentation/keypoint sân thi đấu, thuật toán tracking & temporal modeling (Kalman Filter, optical flow, ByteTrack/BoT-SORT/DeepSORT) — áp dụng trên video tennis, cầu lông quay từ một camera đơn. 

#### **Đầ�u vào** 

#### **Đầ�u ra** 

#### **Giới hạn** 

- Video tennis, cầu lông (góc quay tương đối ổn định) 

- Dataset bóng/cầu định dạng YOLO (train/valid/test) 

- Mô hình keypoint sân đã huấn luyện / có thể fine-tune 

- Video đã annotate: bbox người chơi, bóng, keypoints, mini-court, bảng thống kê 

- Weight mô hình phát hiện bóng tốt nhất 

- Bảng đánh giá mAP, precision, recall, tốc độ inference 

- Chưa xử lý tốt mọi góc quay phức tạp 

- Bóng/cầu rất nhỏ, dễ motion blur / che khuất 

- Chỉ single-camera — chưa multi-camera reconstruction 

Đề cương Đề án Tốt nghiệp Thạc sĩ Kỹ thuật · PTIT 

5 

**I . 5 – I . 6 M Ở ĐẦU** 

# **Phươ ứ ng pháp nghiền c u & Ý nghĩa** 

**PHƯƠNG PHÁP: THỰC NGHIỆM SO SÁNH** 

**Triển khai Thay thế / bổ sung pipeline baseline nhiều họ mô hình** 

**Đánh giá trên cùng Lựa chọn cấu hình dataset & chỉ số phù hợp nhất** 

_Chỉ số đánh giá: Precision · Recall · mAP50 · mAP50-95 · FPS (inference time)_ 

##### **Ý NGHĨA KHOA HỌC & THỰC TIỄN** 

##### **Khoa học** 

##### **Thực tiễ�n** 

Hệ thống hoá bằng chứng thực nghiệm về mức độ phù hợp của các họ object detection/tracking hiện đại cho bài toán vật thể rất nhỏ, tốc độ cao — vốn ít có so sánh trực tiếp, đầy đủ. 

Nâng cấp trực tiếp Match Analysis; tài liệu tham khảo cho CLB phong trào chi phí thấp; nền tảng mở rộng sang bóng bàn, cầu lông chuyên nghiệp. 

Đề cương Đề án Tốt nghiệp Thạc sĩ Kỹ thuật · PTIT 

6 

##### **CH ƯƠ N G 1 · CƠ S Ở LÝ LUẬN** 

# **Pipeline tổng quan & Baseline hiện tại** 

**Đọc video Phát hiện Phát hiện Nội suy vị trí Phát hiện Quy đổi Xác định Tính tốc độ & tách frame người chơi bóng/cầu bóng thiếu keypoints sân mini-court cú đánh & thống kê** 

_Khối tô đậm = trọng tâm cải tiến của đề tài (phát hiện bóng/cầu nhỏ, tốc độ cao)_ 

##### **BASELINE HIỆN TẠI** 

**Người chơi Bóng / cầu Keypoints sân YOLOv8 / YOLOv8x YOLO fine-tuned ResNet / CNN** 

**Quỹ đạo bóng Heuristic + Interpolation** 

_Lý do chọn YOLO làm baseline: inference nhanh, dễ fine-tune, tooling (Ultralytics) tốt với dataset định dạng YOLO._ 

Đề cương Đề án Tốt nghiệp Thạc sĩ Kỹ thuật · PTIT 

7 

##### **CH ƯƠ N G 2 · TỔN G Q UA N VẤN Đ Ề N G H I Ê N C ỨU** 

# **Các h mố hình Object Detection so sánhọ** 

### **YOLO** 

### **Detection Transformer** 

_One-stage · anchor-free_ 

_End-to-end · set prediction_ 

YOLOv8n/s/m (baseline nhẹ), YOLOv5l6u/YOLOv8x (mạnh hơn cho vật thể nhỏ), YOLOv10/v11 (phiên bản mới). 

RT-DETR (real-time, giảm phụ thuộc NMS); RF-DETR đề xuất cho phiên bản sau. 

**Nhanh, dễ fine-tune, hệ sinh thái Ultralytics** 

**Bỏ NMS — tốt cho vật thể nhỏ nằm gần nhau** 

### **Two-stage Detector** 

### **Segmentation / Keypoint** 

_Region proposal + refine_ 

_Pixel-level & landmark_ 

Faster R-CNN làm benchmark offline, đối chiếu độ chính xác với YOLO/RT-DETR. 

Mask R-CNN / YOLO-seg cho người chơi & sân; HRNet/ViTPose/Keypoint R-CNN cho keypoints sân. 

**Chính xác cao, tốc độ chậm hơn** 

**Hỗ trợ homography chính xác hơn** 

Đề cương Đề án Tốt nghiệp Thạc sĩ Kỹ thuật · PTIT 

8 

**CH ƯƠ N G 2 · TỔN G Q UA N VẤN Đ Ề N G H I Ê N C ỨU** 

# **Tracking & Temporal Modeling** 

Tracking-by-detection: chạy detector mỗi frame, sau đó liên kết (associate) các bounding box giữa các frame liên tiếp dựa trên vị trí, hình dạng và đặc trưng ngoại hình. 

**Data-association Trackers** 

**Quỹ đạo & Nội suy** 

**ByteTrack, BoT-SORT, DeepSORT** 

**Kalman Filter, Optical Flow, Interpolation** 

Kết hợp Kalman Filter + thuật toán Hungarian, có/không đặc trưng ngoại hình học sâu. 

Làm mượt quỹ đạo bóng, nội suy vị trí khi mất detection ở một số frame. 

**Ưu điểm:** giảm mất ID, thống kê liên tục hơn. 

**Ưu điểm:** nhẹ, dễ tích hợp vào pipeline hiện tại. 

**Nhược điểm:** bóng di chuyển nhanh, nhỏ → dễ mất track. 

**Nhược điểm:** dễ sai khi bóng đổi hướng nhanh sau cú đánh. 

Đề cương Đề án Tốt nghiệp Thạc sĩ Kỹ thuật · PTIT 

9 

**CH ƯƠ N G 3 · P H ƯƠ N G P H Á P T H ỰC H I ỆN** 

# **ử Thiề�t kề� thực nghiệm — 5 vòng th nghiệm** 



<!-- Start of picture text -->
1 2 3 4 5<br>Baseline nhanh Tăng đ chính xácộ Cải thiện temporal Mô hình thay thễ� Tích hợp pipeline<br>Train YOLOv8n (100 epoch);  YOLOv8s/m, YOLOv5l6u; tăng  Thêm interpolation / Kalman  Thử RT-DETR; dự phòng RF- Đưa weight tốt nhất vào hệ<br>xác nhận pipeline  image size; so sánh mAP &  Filter / optical flow; đánh giá  DETR, Faster R-CNN làm  thống chính; xuất video &<br>train/validate/export đúng. recall. độ mượt quỹ đạo. benchmark offline. bảng kết quả cuối.<br><!-- End of picture text -->

_Môi trường & công cụ: Python · PyTorch · Ultralytics  ·  Roboflow (quản lý & export dataset YOLO)  ·  Google Colab (GPU T4) / máy trạm GPU cá nhân_ 

_Tăng cường dữ liệu: Mosaic · Random scale/crop · Color jitter · Motion-blur augmentation (mô phỏng nhoè do tốc độ cao)_ 

Đề cương Đề án Tốt nghiệp Thạc sĩ Kỹ thuật · PTIT 

10 

# **CH ƯƠ N G 4 · T H ỰC N G H I ỆM VÀ ĐÁ N H G I Á Giao thức & Tiều chí đánh giá** 

**Giao thức thực nghiệm** 

- Cùng cách chia train/valid/test, cùng cấu hình phần cứng cho mọi mô hình 

- Lặp huấn luyện 2–3 lần với seed khác nhau để giảm nhiễu ngẫu nhiên 

**Tiêu chí — Object Detection** 

Precision · Recall · mAP50 · mAP50-95 Số frame phát hiện được / mất bóng Inference time (FPS) 

- Chọn checkpoint bằng tập validation; báo cáo kết quả cuối trên tập test độc lập 

- Kiểm tra định tính (visual inspection) video kết quả bên cạnh chỉ số định lượng 

**Tiêu chí — Tracking & Phân tích** 

Độ ổn định quỹ đạo bóng Độ hợp lý frame đánh bóng & tốc độ ước lượng Độ ổn định ID hai người chơi · Chất lượng video output 

Đề cương Đề án Tốt nghiệp Thạc sĩ Kỹ thuật · PTIT 

11 

**CH ƯƠ N G 4 · T H ỰC N G H I ỆM VÀ ĐÁ N H G I Á** 

# **ả B ng so sánh d kiề�n các mố hìnhự** 

|**Mô hình**|**Vai trò**|**Ưu điểm**|**Nhược điểm**|**Ưu tiên**|
|---|---|---|---|---|
|**YOLOv8n**|Baseline nhanh|Nhanh, nhẹ|Có thể bỏ sót bóng nhỏ|**Cao**|
|**YOLOv8s/m**|Baseline mạnh hơn|Cân bằng tốt|Chậm hơn YOLOv8n|**Cao**|
|**YOLOv5l6u**|Bóng nhỏ|Mạnh hơn|Nặng, train lâu|**Trung bình**|
|**RT-DETR**|Detector thay thế|End-to-end, mạnh|Cần tích hợp thêm|**Trung bình**|
|**Faster R-CNN**|Benchmark offline|Có thể chính xác|Chậm|**Thấp**|
|**RF-DETR**|Bản sau|Hướng mới, tối ưu accuracy-latency|Cần nghiên cứu tích hợp|**Sau**|



_Bảng chỉ mang tính dự kiến dựa trên đặc điểm kiến trúc đã biết; số liệu Precision/Recall/mAP cụ thể sẽ được xác định sau thực nghiệm Chương 3. Cột "Ưu tiên" phản ánh thứ tự triển khai, không kết luận trước mô hình nào tốt nhất._ 

Đề cương Đề án Tốt nghiệp Thạc sĩ Kỹ thuật · PTIT 

12 

**CH ƯƠ N G 4 – V** 

# **Rủi ro d kiề�n & Kề� ho ch th c hi n ự ạ ự ệ** 

##### **RỦI RO & BIỆN PHÁP GIẢM THIỂU** 

###### **Dataset chưa đa dạng góc quay/ánh sáng** 

Bổ sung thu thập/gán nhãn; augmentation mạnh (mosaic, color jitter) 

###### **Framework mới (RT-DETR/RF-DETR) khó tích hợp** 

Thử nghiệm tách biệt trong notebook riêng trước khi tích hợp 

###### **Thời gian train mô hình lớn vượt GPU miễn phí** 

Ưu tiên mô hình nhỏ trước; giảm epoch/resolution khi cần 

###### **Nội suy sai khi bóng đổi hướng đột ngột** 

Đánh giá riêng đoạn đổi hướng; giới hạn số frame nội suy liên tiếp 

##### **KẾ HOẠCH THỰC HIỆN** 

**Tuần 1–2** 

Khảo sát code, dataset, chạy baseline 

###### **Tuần 3–5** 

Train YOLOv8n/v8s, đánh giá kết quả 

**Tuần 5–6** 

Cải thiện ball tracking & interpolation 

**Tuần 6–7** 

Nâng cấp court keypoints & mini-court 

**Tuần 7–8** 

Thử RT-DETR / Faster R-CNN nếu đủ thời gian 

###### **Tuần 8–9** 

Hoàn thiện demo, báo cáo, slide 

Đề cương Đề án Tốt nghiệp Thạc sĩ Kỹ thuật · PTIT 

13 

## **K ẾT LUẬN Kế�t luận & Hướng phát triển** 

Đóng góp chính: bảng so sánh thực nghiệm có hệ thống giữa các họ object detection (YOLO, DETR, two-stage) và tracking/temporal modeling, trên cùng pipeline & tiêu chí đánh giá cho bài toán vật thể nhỏ, tốc độ cao — có thể mở rộng sang bóng bàn, cầu lông chuyên nghiệp. 

##### **HƯỚNG PHÁT TRIỂN TIẾP THEO** 

Bổ sung RF-DETR để so sánh với YOLO / RT-DETR Nhận diện hành động: serve, forehand, backhand Phân loại điểm rơi bóng và khu vực đánh bóng Cải thiện tính tốc độ bằng homography chính xác hơn 

Xây dựng giao diện upload video & xem kết quả 

Hỗ trợ nhiều góc camera & nhiều loại sân 

_Xin cảm n.ơ_ 

Đề cương Đề án Tốt nghiệp Thạc sĩ Kỹ thuật · PTIT 

14 

