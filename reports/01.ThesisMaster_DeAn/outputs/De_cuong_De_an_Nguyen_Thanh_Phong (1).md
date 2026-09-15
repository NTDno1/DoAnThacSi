

<!-- Start of picture text -->
HỌC VIỆN CÔNG NGHỆ BƯU CHÍNH VIỄN THÔNG<br><!-- End of picture text -->

**------------------------------------Nguyễn Thanh Phong ĐỀ CƯƠNG ĐỀ ÁN TỐT NGHIỆP THẠC SĨ KỸ THUẬT (Theo định hướng ứng dụng)** 

**HÀ NỘI - 2026** 

### **HỌC VIỆN CÔNG NGHỆ BƯU CHÍNH VIỄN THÔNG** 

**------------------------------------Nguyễn Thanh Phong** 

**_Chủ đề: Nghiên cứu và xây dựng hệ thống phát hiện và theo dõi vật thể nhỏ chuyển động nhanh trong video thể thao đơn camera_** 

CHUYÊN NGÀNH : KHOA HỌC MÁY TÍNH MÃ SỐ : 8.48.01.01 

**ĐỀ CƯƠNG ĐỀ ÁN TỐT NGHIỆP THẠC SĨ KỸ THUẬT** 

**(Theo định hướng ứng dụng)** 

**NGƯỜI HƯỚNG DẪN KHOA HỌC: TS. Đào Thị Thúy Quỳnh** 

**HÀ NỘI - 2026** 

# **MỤC LỤC** 

|**I. MỞ ĐẦU**|**5**|
|---|---|
|1. Lý do chọn đề tài|5|
|2. Tổng quan về vấn đề nghiên cứu|5|
|3. Mục đích nghiên cứu|6|
|4. Đối tượng và phạm vi nghiên cứu|7|
|5. Phương pháp nghiên cứu|8|
|6. Ý nghĩa khoa học và thực tiễn|8|
|**II. NỘI DUNG**|**9**|
|CHƯƠNG 1: CƠ SỞ LÝ LUẬN|9|
|CHƯƠNG 2: TỔNG QUAN VỀ VẤN ĐỀ NGHIÊN CỨU|12|
|CHƯƠNG 3: PHƯƠNG PHÁP THỰC HIỆN|15|
|CHƯƠNG 4: THỰC NGHIỆM VÀ ĐÁNH GIÁ|18|
|**III. KẾT LUẬN**|**21**|
|**IV. DANH MỤC CÁC TÀI LIỆU THAM KHẢO**|**22**|
|**V. DỰ KIẾN KẾ HOẠCH THỰC HIỆN**|22|
|PHỤ LỤC A: CÔNG CỤ VÀ THƯ VIỆN DỰ KIẾN SỬ DỤNG|24|
|PHỤ LỤC B: BẢNG THUẬT NGỮ VIẾT TẮT|**24**|



## **I. MỞ ĐẦU** 

#### **1. Lý do chọn đề tài** 

Trong tennis và cầu lông, các thông số về tốc độ bóng/cầu, tần suất đánh bóng, vị trí di chuyển và khoảng cách bao phủ sân có giá trị lớn trong huấn luyện và phân tích chiến thuật. Các hệ thống thương mại như Hawk-Eye hay các nền tảng phân tích của các liên đoàn thể thao chuyên nghiệp thường dựa trên hệ thống nhiều camera tốc độ cao đồng bộ hoá cùng cảm biến chuyên dụng, chi phí đầu tư và vận hành rất lớn, chỉ phù hợp với các giải đấu chuyên nghiệp. Trong khi đó, phần lớn huấn luyện viên, câu lạc bộ nghiệp dư và người chơi phong trào chỉ có thể ghi hình bằng một camera đơn thông thường (điện thoại, camera an ninh, máy quay cầm tay). 

Sự phát triển mạnh mẽ của các mô hình thị giác máy tính (computer vision) dựa trên học sâu trong khoảng một thập kỷ gần đây — từ các họ mô hình phát hiện đối tượng hai giai đoạn (Faster R-CNN), một giai đoạn (họ YOLO), cho đến các kiến trúc Transformer cho phát hiện đối tượng (DETR, RT-DETR) — đã mở ra khả năng xây dựng các hệ thống phân tích video thể thao chỉ dựa trên video thông thường mà không cần phần cứng chuyên dụng. Đề tài này kế thừa hướng đi đó, đề xuất một hệ thống phân tích dựa trên video đơn camera, sử dụng các mô hình thị giác máy tính để rút trích thông tin từ hình ảnh, giúp hạ thấp chi phí và mở rộng khả năng tiếp cận công nghệ phân tích trận đấu. 

Dự án hiện tại (Match Analysis) đã có xử lý video tennis, bao gồm: phát hiện người chơi bằng YOLO, phát hiện bóng tennis bằng mô hình YOLO fine-tuned, phát hiện keypoints của sân tennis bằng CNN/ResNet, chuyển toạ độ từ video sang mini-court, ước lượng tốc độ bóng và tốc độ người chơi, và xuất video kết quả có bounding boxes, mini-court và bảng thống kê. Đây là điểm khởi đầu thực tế, cho thấy tính khả thi của hướng tiếp cận, đồng thời cũng bộc lộ hạn chế cốt lõi cần giải quyết: độ chính xác phát hiện bóng/cầu — vật thể có kích thước rất nhỏ trong khung hình và di chuyển với tốc độ cao — vẫn còn là nút thắt của toàn bộ pipeline. 

#### **2. Tổng quan về vấn đề nghiên cứu** 

Vấn đề đặt ra là: với dữ liệu đầu vào chỉ là video thông thường (single-camera, không cảm biến chuyên dụng), làm thế nào để phát hiện chính xác một vật thể rất nhỏ và di 

chuyển nhanh như bóng tennis hoặc cầu lông, đồng thời duy trì việc theo dõi ổn định người chơi và ước lượng đúng các thông số chuyển động qua thời gian? Đây là bài toán khó vì bóng/cầu dễ bị motion blur, che khuất hoặc mất dấu qua nhiều frame liên tiếp, trong khi độ chính xác của các phép đo tốc độ lại phụ thuộc trực tiếp vào chất lượng phát hiện đối tượng và phát hiện keypoints sân. 

Bài toán phát hiện vật thể nhỏ, tốc độ cao trong thể thao đã được nghiên cứu từ trước, tiêu biểu là mạng TrackNet — một mạng học sâu dạng heatmap được thiết kế riêng để theo dõi bóng tennis từ video broadcast, huấn luyện để nhận diện vị trí bóng không chỉ từ một khung hình đơn lẻ mà còn từ chuỗi khung hình liên tiếp nhằm học được quy luật chuyển động của bóng, qua đó vẫn định vị được ngay cả khi ảnh mờ hoặc bóng bị che khuất một phần. Gần đây, một số công trình đã đề xuất kết hợp YOLO để phát hiện người chơi, mô hình chuyên biệt để phát hiện bóng, cùng mạng phát hiện keypoints sân nhằm xây dựng hệ thống kiểu "Hawk-Eye thu nhỏ" từ video đơn camera; các công trình này cũng ghi nhận rằng bài toán phát hiện bóng vẫn là thành phần khó và kém ổn định nhất trong toàn bộ pipeline do đặc thù kích thước nhỏ, tốc độ cao và hiện tượng nhoè ảnh. Điều này cho thấy hướng đi của đề tài là phù hợp với xu thế nghiên cứu hiện tại, đồng thời gợi ý rằng việc so sánh có hệ thống nhiều họ mô hình object detection khác nhau (thay vì chỉ dùng một mô hình cố định) là cần thiết để tìm ra cấu hình tối ưu cho từng thành phần của pipeline. 

Do đó, đề tài tập trung so sánh nhiều họ mô hình object detection và các kỹ thuật temporal modeling khác nhau, nhằm tìm ra cấu hình phù hợp nhất cho bài toán phát hiện bóng/cầu kích thước nhỏ trong pipeline phân tích video hiện có. 

#### **3. Mục đích nghiên cứu** 

Mục tiêu tổng quát: xây dựng một pipeline phân tích video tennis/cầu lông có khả năng nhận diện đối tượng, theo dõi chuyển động và tính toán thông số trận đấu một cách tự động, trên nền tảng video quay bằng một camera thông thường, không đòi hỏi phần cứng chuyên dụng. Pipeline này không chỉ dừng lại ở việc tái lập các chức năng đã có của dự án Match Analysis, mà hướng đến việc xác định một cách có căn cứ khoa học cấu hình mô hình object detection và tracking nào là phù hợp nhất cho từng thành phần của hệ thống, thông qua thực nghiệm so sánh có kiểm soát. 

Mục tiêu cụ thể của đề án bao gồm: 

- Phát hiện và theo dõi hai người chơi trong video. 

- Phát hiện bóng tennis hoặc cầu lông trong từng frame. 

- Nội suy vị trí bóng khi bị mất detection trong một số frame. 

- Phát hiện keypoints của sân tennis hoặc cầu lông để tạo mặt phẳng quy chiếu. 

- Ánh xạ vị trí người chơi và bóng lên mini-court. 

- Xác định các thời điểm đánh bóng. 

- Tính tốc độ bóng, tốc độ di chuyển của người chơi và các thống kê liên quan. 

- So sánh nhiều mô hình object detection khác nhau cho bài toán phát hiện bóng tennis, cầu lông. 

#### **4. Đối tượng và phạm vi nghiên cứu** 

Đối tượng nghiên cứu: các họ mô hình object detection (YOLO, Detection Transformer, two-stage detector), các mô hình segmentation/keypoint cho sân thi đấu, các thuật toán tracking và temporal modeling (Kalman Filter, optical flow, ByteTrack/BoT-SORT/DeepSORT), áp dụng trên video tennis và cầu lông quay từ một camera đơn. 

Phạm vi đề tài: 

- Đầu vào: video tennis, cầu lông quay từ một góc nhìn tương đối ổn định; dataset bóng tennis, cầu lông định dạng YOLO (train/valid/test); mô hình keypoint sân tennis đã huấn luyện hoặc có thể fine-tune tiếp. 

- Đầu ra: video đã annotate (bounding box người chơi, bounding box bóng, keypoints sân, mini-court, bảng thống kê); file weight của mô hình phát hiện bóng tốt nhất; bảng đánh giá mô hình theo mAP, precision, recall và tốc độ inference. 

- Giới hạn: hệ thống chưa xử lý tốt mọi góc quay phức tạp; bóng tennis, cầu lông kích thước rất nhỏ, dễ bị motion blur hoặc che khuất; tốc độ ước lượng phụ thuộc vào chất lượng keypoints và góc camera; ban đầu chỉ tập trung single-camera, chưa dùng multi-camera reconstruction. 

#### **5. Phương pháp nghiên cứu** 

Đề án sử dụng phương pháp thực nghiệm so sánh (comparative experimentation): triển khai pipeline baseline hiện có (YOLOv8/YOLOv8x cho người chơi, YOLO fine-tuned cho bóng, ResNet/CNN cho keypoints sân, heuristic kết hợp interpolation cho quỹ đạo bóng), sau đó lần lượt thử nghiệm thay thế/bổ sung bằng các họ mô hình khác nhau (YOLO các phiên bản, Detection Transformer, two-stage detector, segmentation/keypoint, các thuật toán tracking) trên cùng một bộ dữ liệu, đánh giá theo các chỉ số định lượng (precision, recall, mAP50, mAP50-95, FPS) để lựa chọn cấu hình phù hợp nhất. 

#### **6. Ý nghĩa khoa học và thực tiễn** 

Về mặt khoa học, đề án góp phần hệ thống hoá và cung cấp bằng chứng thực nghiệm về mức độ phù hợp của các họ mô hình object detection và tracking hiện đại đối với một lớp bài toán cụ thể — phát hiện vật thể rất nhỏ, tốc độ cao trong video thể thao đơn camera — vốn có ít công trình so sánh trực tiếp, đầy đủ giữa nhiều họ kiến trúc trên cùng một pipeline và cùng bộ tiêu chí. 

Về mặt thực tiễn, kết quả của đề án có thể được sử dụng để: (i) nâng cấp trực tiếp hệ thống Match Analysis hiện có của nhóm nghiên cứu; (ii) làm tài liệu tham khảo cho các cá nhân, câu lạc bộ thể thao phong trào muốn xây dựng công cụ phân tích trận đấu chi phí thấp từ video quay bằng điện thoại hoặc camera thông thường, không cần đầu tư hệ thống multi-camera đắt tiền; (iii) làm nền tảng kỹ thuật để mở rộng sang các môn thể thao khác có đặc điểm bóng/cầu nhỏ tương tự như bóng bàn, cầu lông chuyên nghiệp. 

## **II. NỘI DUNG** 

#### **CHƯƠNG 1: CƠ SỞ LÝ LUẬN** 

Chương này trình bày các khái niệm nền tảng về phát hiện đối tượng (object detection) và theo dõi đối tượng (tracking) trong thị giác máy tính, làm cơ sở lý thuyết cho việc lựa chọn và so sánh mô hình ở các chương sau, đồng thời trình bày tổng quan pipeline phân tích video và baseline hiện tại của hệ thống. 

##### **1.1. Khái niệm phát hiện đối tượng (Object Detection)** 

Phát hiện đối tượng là bài toán xác định vị trí (bounding box) và nhãn lớp của các đối tượng xuất hiện trong một ảnh hoặc một frame video. Khác với bài toán phân loại ảnh (image classification) chỉ trả về một nhãn cho toàn bộ ảnh, object detection phải đồng thời giải quyết hai bài toán con: định vị (localization) và phân loại (classification) cho từng đối tượng riêng lẻ, kể cả khi trong ảnh có nhiều đối tượng cùng lúc. 

Các phương pháp object detection dựa trên học sâu thường được chia thành hai nhóm kiến trúc chính: 

- Two-stage detector (phát hiện hai giai đoạn): giai đoạn 1 sinh ra tập các vùng đề xuất (region proposals) có khả năng chứa đối tượng; giai đoạn 2 phân loại và tinh chỉnh bounding box cho từng vùng đề xuất đó. Đại diện tiêu biểu là họ R-CNN (R-CNN, Fast R-CNN, Faster R-CNN, Mask R-CNN). Nhóm này thường cho độ chính xác cao nhưng tốc độ suy luận chậm hơn. 

- One-stage detector (phát hiện một giai đoạn): dự đoán trực tiếp vị trí và nhãn lớp trên toàn bộ ảnh trong một lượt xử lý duy nhất, không qua bước sinh region proposal riêng biệt. Đại diện tiêu biểu là họ YOLO (You Only Look Once) và SSD. Nhóm này có tốc độ suy luận nhanh hơn nhiều, phù hợp ứng dụng thời gian thực, đánh đổi bằng độ chính xác có thể thấp hơn với vật thể nhỏ. 

Gần đây, kiến trúc Transformer được đưa vào bài toán object detection (DETR — DEtection TRansformer), coi bài toán phát hiện là bài toán dự đoán tập hợp (set prediction) trực tiếp, loại bỏ bước hậu xử lý Non-Maximum Suppression (NMS) vốn cần thiết ở các kiến trúc CNN truyền thống. RT-DETR là một cải tiến của hướng này nhằm đạt tốc độ suy luận thời gian thực trong khi vẫn giữ được ưu điểm end-to-end của Transformer. 

##### **1.2. Các chỉ số đánh giá trong object detection** 

Intersection over Union (IoU) là chỉ số đo mức độ trùng khớp giữa bounding box dự đoán và bounding box thực tế (ground truth), tính bằng tỉ lệ giữa diện tích phần giao và diện tích phần hợp của hai bounding box. Một dự đoán được xem là đúng (True Positive) nếu IoU giữa nó và ground truth vượt một ngưỡng cho trước (thường là 0.5). 

Dựa trên IoU, các chỉ số Precision và Recall được định nghĩa như sau: 

- Precision = TP / (TP + FP): tỉ lệ dự đoán đúng trên tổng số dự đoán được đưa ra. 

- Recall = TP / (TP + FN): tỉ lệ đối tượng thực tế được phát hiện đúng trên tổng số đối tượng thực tế. 

mAP (mean Average Precision) là chỉ số tổng hợp, tính bằng trung bình của Average Precision (diện tích dưới đường cong Precision-Recall) trên tất cả các lớp đối tượng. mAP50 được tính tại ngưỡng IoU = 0.5, trong khi mAP50-95 là trung bình của AP tại nhiều ngưỡng IoU từ 0.5 đến 0.95 (bước 0.05), phản ánh độ chính xác định vị nghiêm ngặt hơn. Đây là hai chỉ số phổ biến nhất để so sánh các mô hình object detection, đặc biệt trên bộ dữ liệu chuẩn COCO. 

##### **1.3. Theo dõi đối tượng (Object Tracking) và bộ lọc Kalman** 

Object tracking là bài toán duy trì định danh (ID) nhất quán cho từng đối tượng qua các frame liên tiếp của video, dựa trên kết quả phát hiện đối tượng ở từng frame riêng lẻ. Cách tiếp cận phổ biến hiện nay là tracking-by-detection: chạy một detector ở mỗi frame, sau đó liên kết (association) các bounding box giữa các frame liên tiếp dựa trên vị trí, hình dạng và đặc trưng ngoại hình (appearance feature). 

Bộ lọc Kalman (Kalman Filter) là một thuật toán ước lượng trạng thái tuyến tính được sử dụng phổ biến để dự đoán vị trí tiếp theo của đối tượng dựa trên chuyển động trước đó, giúp làm mượt quỹ đạo và nội suy vị trí khi mất detection trong một số frame. Các thuật toán tracking hiện đại như DeepSORT, ByteTrack, BoT-SORT đều sử dụng Kalman Filter kết hợp với các cơ chế liên kết dữ liệu (data association) như thuật toán Hungarian, có hoặc không kết hợp đặc trưng ngoại hình học sâu (deep appearance feature), để nâng cao độ ổn định ID khi đối tượng bị che khuất tạm thời hoặc chuyển động nhanh. 

##### **1.4. Phát hiện keypoint và phân vùng ảnh (Segmentation)** 

Ngoài bounding box, một số bài toán con của hệ thống cần thông tin chi tiết hơn về hình học của đối tượng hoặc sân đấu. Phát hiện keypoint (keypoint detection) là bài toán xác định toạ độ của các điểm đặc trưng cố định trên một đối tượng — ví dụ các góc và giao điểm vạch kẻ của sân tennis/cầu lông, hoặc các khớp cơ thể của người chơi. Kết quả keypoint là đầu vào cần thiết để tính phép biến đổi phối cảnh (homography) giữa toạ độ trên khung hình camera và toạ độ thực trên sân, từ đó mới có thể ánh xạ vị trí người chơi/bóng sang hệ toạ độ mini-court và tính được các đại lượng vật lý như tốc độ, quãng đường di chuyển. 

Instance segmentation (phân vùng theo từng thực thể) là bài toán xác định chính xác từng pixel thuộc về đối tượng nào, thay vì chỉ vẽ một bounding box bao quanh. Mask R-CNN là kiến trúc tiêu biểu, mở rộng Faster R-CNN bằng cách thêm một nhánh dự đoán mặt nạ (mask) song song với nhánh phân loại và định vị. Trong đề tài này, segmentation được xem là một hướng thử nghiệm bổ sung để đánh giá xem việc có được vùng pixel chính xác của người chơi/sân có cải thiện đáng kể chất lượng ánh xạ toạ độ so với chỉ dùng bounding box hay không, đổi lại chi phí gán nhãn dữ liệu cao hơn đáng kể. 

##### **1.5. Tổng quan pipeline** 

- Đọc video và tách thành frame. 

- Phát hiện người chơi. 

- Phát hiện bóng tennis, cầu lông. 

- Nội suy vị trí bóng bị thiếu. 

- Phát hiện keypoints sân tennis, cầu lông. 

- Lọc hai người chơi chính dựa trên vị trí sân. 

- Chuyển toạ độ người chơi và bóng sang mini-court. 

- Phát hiện các frame có cú đánh bóng. 

- Tính toán tốc độ bóng, tốc độ di chuyển và thống kê. 

- Vẽ kết quả lên video đầu ra. 

##### **1.6. Baseline hiện tại** 

Baseline của dự án sử dụng YOLOv8/YOLOv8x cho phát hiện người chơi, YOLO fine-tuned cho phát hiện bóng tennis, cầu lông, ResNet/CNN cho phát hiện keypoints sân tennis, cầu lông, và heuristic kết hợp interpolation cho xử lý quỹ đạo bóng cũng như xác định frame đánh bóng. Đây là baseline phù hợp vì YOLO có tốc độ inference nhanh, dễ fine-tune và có sẵn tooling tốt với dataset định dạng YOLO. 

#### **CHƯƠNG 2: TỔNG QUAN VỀ VẤN ĐỀ NGHIÊN CỨU** 

Chương này phân loại và phân tích các mô hình sẽ được thử nghiệm nhằm giải quyết bài toán phát hiện bóng nhỏ, di chuyển nhanh. Việc lựa chọn các họ mô hình dựa trên tổng quan các hướng tiếp cận object detection hiện đại — từ các mô hình một giai đoạn tối ưu tốc độ, các mô hình hai giai đoạn tối ưu độ chính xác, đến các kiến trúc Transformer mới — nhằm đảm bảo phép so sánh bao quát được sự đánh đổi giữa tốc độ suy luận và độ chính xác, vốn là yếu tố quyết định khi lựa chọn mô hình triển khai thực tế cho một hệ thống chạy gần thời gian thực. 

##### **2.1. Nhóm YOLO** 

Họ YOLO (You Only Look Once) là dòng mô hình one-stage detector được phát triển liên tục từ năm 2016 đến nay, hiện do Ultralytics duy trì và phát triển các phiên bản YOLOv5 trở về sau. Kiến trúc YOLO gồm ba thành phần chính: backbone (trích xuất đặc trưng), neck (kết hợp đặc trưng đa tỉ lệ) và head (dự đoán bounding box, lớp và độ tin cậy). Từ YOLOv8, kiến trúc chuyển sang anchor-free (không cần định nghĩa trước các anchor box), sử dụng decoupled head (tách nhánh phân loại và nhánh định vị), giúp cải thiện độ chính xác so với các phiên bản anchor-based trước đó. Đây là lý do YOLO được lựa chọn làm baseline chính của đề tài: cân bằng tốt giữa tốc độ và độ chính xác, đồng thời có hệ sinh thái công cụ (Ultralytics) hỗ trợ huấn luyện, đánh giá và triển khai rất thuận tiện. 

##### **YOLOv8n / YOLOv8s / YOLOv8m** 

- Mục đích: baseline nhẹ, dễ train nhanh trên Colab. 

- Ưu điểm: tốc độ nhanh, dễ fine-tune, hỗ trợ tốt với Ultralytics. 

- Nhược điểm: với vật thể rất nhỏ như bóng tennis, bản nhẹ có thể bỏ sót nếu frame bị mờ. 

- Cách dùng: train nhiều kích thước model và so sánh mAP, recall, FPS. 

**YOLOv5l6u / YOLOv8x** 

- Mục đích: baseline mạnh hơn cho phát hiện bóng nhỏ. 

- Ưu điểm: khả năng học đặc trưng tốt hơn. 

- Nhược điểm: chậm, tốn GPU, không tối ưu cho máy yếu. 

- Cách dùng: train sau khi đã xác nhận pipeline đúng bằng model nhỏ. 

##### **YOLOv10 / YOLOv11** 

- Mục đích: thử các phiên bản YOLO mới hơn nếu môi trường và thư viện hỗ trợ ổn định. 

- Ưu điểm: có thể cải thiện tốc độ hoặc độ chính xác. 

- Nhược điểm: cần kiểm tra khả năng tương thích với code hiện tại. 

##### **2.2. Nhóm Detection Transformer** 

DETR (DEtection TRansformer) là kiến trúc đầu tiên áp dụng Transformer cho object detection theo hướng end-to-end, coi bài toán phát hiện là bài toán dự đoán một tập hợp cố định các đối tượng (set prediction), sử dụng cơ chế bipartite matching giữa dự đoán và ground truth trong quá trình huấn luyện. Ưu điểm lớn nhất là loại bỏ hoàn toàn bước hậu xử lý NMS, vốn cần tinh chỉnh ngưỡng thủ công và có thể gây lỗi khi nhiều đối tượng nhỏ nằm gần nhau — đúng là tình huống thường gặp khi bóng tennis/cầu lông nằm gần vạch sân hoặc gần chân người chơi. Tuy nhiên, DETR nguyên bản có tốc độ hội tụ chậm và độ trễ suy luận cao, chưa phù hợp ứng dụng thời gian thực. 

##### **RT-DETR** 

- Mục đích: so sánh với YOLO trong bài toán real-time detection. 

- Ưu điểm: hướng end-to-end, giảm phụ thuộc vào NMS, có khả năng cân bằng tốc độ và độ chính xác. 

- Nhược điểm: có thể cần cấu hình train kỹ hơn YOLO, tốn tài nguyên hơn với dataset nhỏ. 

- Cách dùng: fine-tune trên dataset bóng tennis và so sánh với YOLO theo mAP50, mAP50-95, recall và inference time. 

**RF-DETR (đề xuất cho phiên bản sau)** 

- Mục đích: nghiên cứu hướng detection transformer hiện đại hơn, đặc biệt khi cần tối ưu accuracy-latency. 

- Lý do để ở bản sau: cần thêm thời gian tích hợp framework, chuẩn hoá dataset, benchmark và kiểm tra khả năng deploy. Kỳ vọng: nếu hoạt động tốt với object nhỏ, RF-DETR có thể trở thành hướng nâng cấp cho ball detector. 

##### **2.3. Nhóm two-stage detector** 

Khác với các mô hình one-stage/Transformer nêu trên, two-stage detector tách rõ hai bước: sinh region proposal (thường bằng Region Proposal Network — RPN) và phân loại/tinh chỉnh bounding box cho từng proposal. Việc tách bước này cho phép mô hình tập trung tính toán kỹ hơn vào các vùng có khả năng chứa đối tượng, thường mang lại độ chính xác cao, đặc biệt hữu ích để làm chuẩn đối chiếu (benchmark) khi đánh giá xem các mô hình nhanh hơn (YOLO, RT-DETR) có đánh đổi độ chính xác ở mức chấp nhận được hay không. 

**Faster R-CNN** 

- Mục đích: làm baseline chính xác cao hơn nhưng chậm hơn. 

- Ưu điểm: có thể bắt vật thể nhỏ tốt nếu cấu hình anchor và input size phù hợp. 

- Nhược điểm: inference chậm, khó real-time. 

- Vai trò trong đề tài: benchmark offline để biết YOLO/DETR có bỏ sót quá nhiều bóng hay không. 

##### **2.4. Nhóm segmentation/keypoint** 

##### **Mask R-CNN / YOLO segmentation** 

- Mục đích: thử nghiệm phân vùng người chơi hoặc sân. 

- Ưu điểm: có thể lấy vị trí chính xác hơn bounding box. 

- Nhược điểm: cần label segmentation, chi phí dữ liệu cao. 

**HRNet / ViTPose / Keypoint R-CNN cho court keypoints** 

- Mục đích: nâng cấp phần phát hiện keypoints sân tennis. 

- Ưu điểm: keypoints tốt hơn giúp quy đổi toạ độ và tính tốc độ ổn định hơn. 

- Nhược điểm: cần dataset keypoint đủ chuẩn. 

##### **2.5. Tracking và temporal modeling** 

**ByteTrack / BoT-SORT / DeepSORT** 

- Mục đích: theo dõi người chơi và bóng qua frame. 

- Ưu điểm: giảm mất ID, giúp thống kê liên tục hơn. 

- Nhược điểm: bóng tennis di chuyển nhanh, nhỏ, dễ bị mất track. 

##### **Kalman Filter / optical flow / interpolation** 

- Mục đích: làm mượt quỹ đạo bóng và nội suy frame mất detection. 

- Ưu điểm: nhẹ, dễ tích hợp vào pipeline hiện tại. 

- Nhược điểm: dễ sai khi bóng đổi hướng nhanh sau cú đánh. 

#### **CHƯƠNG 3: PHƯƠNG PHÁP THỰC HIỆN** 

##### **3.1. Môi trường và công cụ thực nghiệm** 

- Ngôn ngữ lập trình: Python. 

- Framework huấn luyện mô hình: PyTorch, thư viện Ultralytics (cho họ YOLO), các repository chính thức của RT-DETR/Faster R-CNN khi cần đối chiếu. 

- Công cụ quản lý và gán nhãn dữ liệu: Roboflow (quản lý phiên bản dataset, export định dạng YOLO). 

- Môi trường tính toán: Google Colab (GPU T4) cho các thử nghiệm quy mô nhỏ và vừa; máy trạm GPU cá nhân (nếu có) cho các thử nghiệm cần thời gian train dài hơn. 

- Công cụ theo dõi thực nghiệm: ghi log kết quả train/validate theo từng epoch (loss, mAP, precision, recall) để so sánh giữa các lần chạy. 

##### **3.2. Dataset** 

- Sử dụng dataset tennis-ball-detection-6 định dạng YOLO. 

- Chia train/valid/test theo cấu trúc có sẵn. 

- Kiểm tra lại nhãn, ảnh lỗi, ảnh không có bóng và các trường hợp motion blur. 

- Bổ sung thêm frame từ video thực tế nếu dataset chưa đủ đa dạng. 

##### **3.3. Cấu hình train ban đầu** 

- Model baseline: YOLOv8n. 

- Epoch: 50–100. 

- Image size: 640, sau đó thử 960 hoặc 1280 nếu bóng quá nhỏ. 

- Batch size: 8 hoặc 16 tuỳ GPU. 

- GPU: Colab T4 hoặc local GPU nếu có. 

##### **3.4. Kỹ thuật tăng cường dữ liệu (Data Augmentation)** 

Do bóng tennis/cầu lông chiếm diện tích rất nhỏ trong khung hình, các kỹ thuật tăng cường dữ liệu được áp dụng nhằm giúp mô hình học tốt hơn với vật thể nhỏ và tăng khả năng tổng quát hoá: 

- Mosaic augmentation: ghép 4 ảnh thành 1 ảnh huấn luyện, giúp mô hình học được nhiều ngữ cảnh và tỉ lệ vật thể khác nhau trong cùng một batch. 

- Random scale/crop: thay đổi tỉ lệ và cắt ngẫu nhiên vùng ảnh, mô phỏng các khoảng cách camera khác nhau. 

- Color jitter (thay đổi độ sáng, tương phản, bão hoà màu): tăng độ bền vững của mô hình trước điều kiện ánh sáng sân đấu khác nhau. 

- Motion blur augmentation (nếu cần bổ sung thủ công): mô phỏng hiện tượng nhoè do bóng di chuyển tốc độ cao, giúp mô hình học được đặc trưng của bóng ngay cả khi bị mờ. 

##### **3.5. Các vòng thử nghiệm** 

**Vòng 1 — Baseline nhanh** 

- Nội dung: train YOLOv8n trong 100 epoch; xác nhận pipeline train, validate và export weight đúng. 

- Mục đích: đảm bảo toàn bộ pipeline kỹ thuật (đọc dataset, train, export, load lại vào hệ thống chính) chạy đúng trước khi đầu tư thời gian train các mô hình lớn hơn. 

- Rủi ro có thể gặp: lỗi định dạng dataset, mismatch giữa phiên bản Ultralytics và weight export; biện pháp giảm thiểu là cố định phiên bản thư viện ngay từ đầu. 

**Vòng 2 — Tăng độ chính xác** 

- Nội dung: train YOLOv8s, YOLOv8m hoặc YOLOv5l6u; thử tăng image size; so sánh mAP và recall trên validation/test set. 

- Mục đích: xác định xem việc tăng kích thước mô hình/ảnh đầu vào có cải thiện đáng kể khả năng phát hiện bóng nhỏ hay không, và đánh đổi về tốc độ suy luận tương ứng là bao nhiêu. 

- Rủi ro: thời gian train tăng đáng kể với ảnh kích thước lớn (960/1280); cần theo dõi bộ nhớ GPU và điều chỉnh batch size phù hợp. 

**Vòng 3 — Cải thiện temporal** 

   - Nội dung: thêm interpolation, Kalman Filter hoặc optical flow cho bóng; đánh giá số frame bị mất bóng và độ mượt quỹ đạo. 

   - Mục đích: bù đắp cho các frame mà detector không phát hiện được bóng (do motion blur/che khuất), vốn là hạn chế cố hữu của mọi detector dù mạnh đến đâu. 

   - Rủi ro: các phương pháp nội suy có thể cho kết quả sai khi bóng đổi hướng đột ngột (ví dụ ngay sau cú đánh); cần đánh giá riêng độ chính xác nội suy ở các đoạn có đổi hướng. 

- **Vòng 4 — Mô hình thay thế** 

   - Nội dung: thử RT-DETR; nếu có thời gian, thiết kế bản sau với RF-DETR và/hoặc Faster R-CNN làm benchmark offline. 

   - Mục đích: kiểm chứng liệu kiến trúc Transformer/two-stage có mang lại lợi ích rõ rệt so với YOLO cho bài toán vật thể nhỏ hay không, đủ để đánh đổi chi phí tích hợp và tốc độ suy luận chậm hơn. 

   - Rủi ro: thời gian tích hợp framework mới (khác hệ sinh thái Ultralytics) có thể vượt kế hoạch; cần ưu tiên thử nghiệm RT-DETR trước, RF-DETR chuyển sang giai đoạn sau nếu không đủ thời gian. 

##### **Vòng 5 — Tích hợp pipeline cuối** 

- Nội dung: đưa weight tốt nhất vào models/, chạy main.py, tạo video output và bảng kết quả. 

- Mục đích: xác nhận mô hình được chọn hoạt động ổn định trong toàn bộ pipeline thực tế (không chỉ trên tập test tách biệt), tạo ra sản phẩm bàn giao hoàn chỉnh. 

#### **CHƯƠNG 4: THỰC NGHIỆM VÀ ĐÁNH GIÁ** 

##### **4.1. Giao thức đánh giá thực nghiệm** 

Để đảm bảo tính khách quan khi so sánh các mô hình, thực nghiệm được thiết kế theo giao thức sau: 

- Tất cả các mô hình được huấn luyện và đánh giá trên cùng một cách chia train/valid/test, cùng một cấu hình phần cứng (GPU), để loại bỏ sai lệch do khác biệt dữ liệu hoặc môi trường. 

- Với mỗi mô hình, huấn luyện được lặp lại tối thiểu 2–3 lần với các seed khởi tạo khác nhau (nếu thời gian cho phép) để giảm nhiễu ngẫu nhiên khi so sánh kết quả. 

- Kết quả trên tập validation dùng để lựa chọn checkpoint tốt nhất trong quá trình train; kết quả cuối cùng được báo cáo trên tập test độc lập, không được sử dụng trong quá trình lựa chọn siêu tham số. 

- Ngoài các chỉ số định lượng, một số đoạn video kết quả được kiểm tra định tính (visual inspection) để phát hiện các lỗi hệ thống mà chỉ số trung bình có thể không phản ánh hết, ví dụ mất bóng liên tục trong một pha bóng nhanh. 

##### **4.2. Tiêu chí đánh giá cho object detection** 

   - Precision, Recall, mAP50, mAP50-95. 

   - Số frame phát hiện được bóng, số frame mất detection. 

   - Inference time/FPS. 

- **4.3. Tiêu chí đánh giá cho tracking và phân tích trận đấu** 

   - Độ ổn định của quỹ đạo bóng. 

   - Độ hợp lý của frame đánh bóng và tốc độ bóng ước lượng. 

   - Độ ổn định ID của hai người chơi. 

   - Chất lượng video output. 

**4.4. Bảng so sánh dự kiến** 

|**Mô hình**|**Vai trò**|**Ưu điểm**|**Nhược điểm**|**Mức ưu tiên**|
|---|---|---|---|---|
|YOLOv8n|Baseline<br>nhanh|Nhanh, nhẹ|Có thể bỏ sót bóng nhỏ|Cao|
|YOLOv8s/m|Baseline<br>mạnh hơn|Cân bằng tốt|Chậm hơn YOLOv8n|Cao|
|YOLOv5l6u|Bóng nhỏ|Mạnh hơn|Nặng, train lâu|Trung bình|
|RT-DETR|Detector thay<br>thế|End-to-end,<br>mạnh|Cần tích hợp thêm|Trung bình|
|Faster R-CNN|Benchmark<br>offline|Có thể chính xác|Chậm|Thấp|
|RF-DETR|Bản sau|Hướng mới, tối<br>ưu<br>accuracy-latenc<br>y|Cần nghiên cứu tích hợp|Sau|



Bảng trên chỉ mang tính dự kiến dựa trên đặc điểm kiến trúc đã biết của từng họ mô hình; các con số Precision/Recall/mAP cụ thể sẽ chỉ được xác định sau khi hoàn thành thực nghiệm ở Chương 3 và sẽ được cập nhật trong báo cáo đề án tốt nghiệp chính thức. Cách đọc bảng: cột "Mức ưu tiên" phản ánh thứ tự triển khai thử nghiệm trong lộ trình (Mục V), không phản ánh trước kết luận về mô hình nào tốt nhất — kết luận đó chỉ được đưa ra sau khi có số liệu thực nghiệm. 

##### **4.6. Rủi ro dự kiến và biện pháp giảm thiểu** 

|**Rủi ro**|**Ảnh hưởng**|**Biện pháp giảm thiểu**|
|---|---|---|
|Dataset gán nhãn bóng<br>chưa đủ đa dạng góc<br>quay/điều kiện ánh sáng|Mô hình có thể overfit,<br>không tổng quát hoá tốt<br>sang video thực tế khác|Bổ sung thu thập/gán nhãn thêm frame<br>thực tế; áp dụng augmentation mạnh<br>(mosaic, color jitter)|
|Thời gian train các mô<br>hình lớn (YOLOv8x,<br>RT-DETR) vượt tài nguyên<br>GPU miễn phí (Colab T4)|Trễ tiến độ các vòng thử<br>nghiệm 2 và 4|Ưu tiên train mô hình nhỏ trước; giảm<br>epoch/resolution khi cần; cân nhắc<br>GPU trả phí nếu ngân sách cho phép|
|Tích hợp framework mới<br>(RT-DETR/RF-DETR)<br>không tương thích code<br>hiện có|Không hoàn thành được<br>vòng thử nghiệm 4 đúng<br>hạn|Thử nghiệm tách biệt trong notebook<br>riêng trước khi tích hợp vào pipeline<br>chính; có phương án dự phòng chỉ báo<br>cáo kết quả offline|
|Quỹ đạo bóng bị nội suy<br>sai khi bóng đổi hướng đột<br>ngột sau cú đánh|Sai số trong tính tốc độ<br>bóng và xác định frame<br>đánh bóng|Đánh giá riêng độ chính xác nội suy<br>tại các đoạn đổi hướng; giới hạn số<br>frame tối đa được phép nội suy liên<br>tiếp|



- **4.5. Sản phẩm bàn giao** 

   - Source code pipeline phân tích video. 

   - Notebook train ball detector. 

   - Weight mô hình tốt nhất. 

   - Video demo đầu ra. 

   - Sản phẩm bao gồm phần mềm. 

   - Bảng kết quả so sánh mô hình. 

   - Báo cáo tổng kết phương pháp, thực nghiệm và hướng phát triển. 

## **III. KẾT LUẬN** 

Đề tài có tính ứng dụng rõ ràng trong phân tích thể thao và phù hợp để triển khai bằng các kỹ thuật thị giác máy tính hiện đại. Hướng phát triển chính của đề án là: phát hiện người chơi, phát hiện bóng, phát hiện keypoints sân, mini-court và thống kê tốc độ; tăng độ chính xác phát hiện bóng tennis hoặc cầu lông; so sánh nhiều mô hình detection; và nâng cao độ ổn định của tracking theo thời gian. 

Thông qua việc kết hợp nhiều họ mô hình object detection, kỹ thuật tracking và temporal modeling, đề tài kỳ vọng xây dựng được một hệ thống phân tích video có độ chính xác và độ ổn định đủ tốt để hỗ trợ huấn luyện và phân tích chiến thuật. 

Về mặt khoa học, đóng góp chính của đề án là cung cấp một bảng so sánh thực nghiệm có hệ thống giữa các họ mô hình object detection (YOLO, Detection Transformer, two-stage detector) và các kỹ thuật tracking/temporal modeling khi áp dụng cho cùng một bài toán vật thể nhỏ, tốc độ cao (bóng tennis/cầu lông), trên cùng một pipeline và cùng bộ tiêu chí đánh giá. Kết quả này có thể dùng làm căn cứ tham khảo cho các hệ thống phân tích thể thao khác có đặc điểm tương tự (ví dụ bóng bàn, cầu lông chuyên nghiệp), không chỉ giới hạn ở tennis. 

Về mặt hạn chế và rủi ro, đề án nhận thức được một số giới hạn cần lưu ý khi triển khai: (i) kết quả so sánh mô hình phụ thuộc vào chất lượng và độ đa dạng của dataset huấn luyện, nếu dataset chưa đủ đa dạng góc quay/điều kiện ánh sáng thì kết quả có 

thể không tổng quát hoá tốt sang video thực tế khác; (ii) việc thử nghiệm đầy đủ tất cả các mô hình (đặc biệt RT-DETR, Faster R-CNN) đòi hỏi tài nguyên GPU và thời gian train đáng kể, có thể ảnh hưởng đến phạm vi thực hiện trong khung thời gian của đề án; (iii) các cải tiến temporal modeling (Kalman Filter, optical flow) vẫn mang tính heuristic, hiệu quả có thể thay đổi tuỳ theo phong cách di chuyển bóng của từng trận đấu cụ thể. 

Hướng phát triển tiềm năng bao gồm: 

- Thêm RF-DETR trong phiên bản sau để so sánh với YOLO và RT-DETR. 

- Bổ sung action recognition để nhận diện serve, forehand, backhand. 

- Phân loại điểm rơi bóng và khu vực đánh bóng. 

- Cải thiện tính tốc độ bằng homography chính xác hơn. 

- Xây dựng giao diện upload video và xem kết quả. 

- Hỗ trợ nhiều góc camera và nhiều loại sân tennis, cầu lông. 

**IV. DANH MỤC CÁC TÀI LIỆU THAM KHẢO** 

[1] Huang, Y.-C., Liao, I-N., Chen, C.-H., Ik, T.-U., Peng, W.-C. (2019). TrackNet: A Deep Learning Network for Tracking High-speed and Tiny Objects in Sports Applications. arXiv:1907.03698. 

[2] Zhao, Y., Lv, W., Xu, S., Wei, J., Wang, G., Dang, Q., Liu, Y., Chen, J. (2023). DETRs Beat YOLOs on Real-time Object Detection (RT-DETR). arXiv:2304.08069. 

[3] Zhang, Y., Sun, P., Jiang, Y., Yu, D., Weng, F., Yuan, Z., Luo, P., Liu, W., Wang, X. (2021). ByteTrack: Multi-Object Tracking by Associating Every Detection Box. arXiv:2110.06864. 

[4] Ren, S., He, K., Girshick, R., Sun, J. (2015). Faster R-CNN: Towards Real-Time Object Detection with Region Proposal Networks. arXiv:1506.01497. 

[5] He, K., Gkioxari, G., Dollár, P., Girshick, R. (2017). Mask R-CNN. arXiv:1703.06870. 

[6] Sapkota, R. và cộng sự (2025). Ultralytics YOLO Evolution: An Overview of YOLO26, YOLO11, YOLOv8 and YOLOv5 Object Detectors for Computer Vision and Pattern Recognition. arXiv:2510.09653. 

[7] Automated Tennis Player and Ball Tracking with Court Keypoints Detection (Hawk Eye System). arXiv:2511.04126. 

[8] https://arxiv.org/pdf/2511.09554 

[9] https://rfdetr.roboflow.com/latest/ 

[10] https://blog.roboflow.com/best-object-detection-models/ 

[11] Wang, C.-K. et al. (2020). TrackNetV2: Efficient Shuttlecock Tracking Network for Badminton Match Analysis. 

[12] Chen, Y.-J., & Wang, Y.-S. (2023). TrackNetV3: Enhancing Shuttlecock Tracking with Augmentations and Trajectory Rectification. ACM Multimedia Asia. 

[13] Liu, P. et al. (2022). MonoTrack: Shuttle Trajectory Reconstruction From Monocular Badminton Video. In Proceedings of the IEEE/CVF Conference on Computer Vision and Pattern Recognition (CVPR) Workshops, pp. 3512–3521 

## **V. DỰ KIẾN KẾ HOẠCH THỰC HIỆN** 

|**TT**|**Nội dung**|**Dự kiến thời gian thực hiện**|
|---|---|---|
|1|Khảo sát code, dataset, chạy baseline|Tuần 1–2|
|2|Train YOLOv8n/v8s, đánh giá kết quả|Tuần 3–5|
|3|Cải thiện ball tracking và interpolation|Tuần 5–6|
|4|Nâng cấp court keypoints và mini-court|Tuần 6–7|
|5|Thử RT-DETR/Faster R-CNN nếu đủ thời<br>gian|Tuần 7–8|
|6|Hoàn thiện demo, báo cáo, slide|Tuần 8–9|



#### **Ý KIẾN CỦA** 

**NGƯỜI LẬP ĐỀ CƯƠNG** 

#### **NGƯỜI HƯỚNG DẪN KHOA HỌC** 

_(Ký ghi rõ họ tên)_ 

_(Ký ghi rõ họ tên)_ 

#### **TS. Đào Thị Thúy Quỳnh Nguyễn Thanh Phong** 

#### **DUYỆT CỦA TRƯỞNG TIỂU BAN ĐÁNH GIÁ ĐỀ CƯƠNG** 

_(Ký ghi rõ họ tên)_ 

