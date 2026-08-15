# Bản sửa MANUAL – con trượt gần đều bằng 16 PR

## Nguyên nhân project cũ không có thay đổi

Project gốc không chứa các thành phần sau:

- `Models/UniformSliderMotion.cs`
- `Services/IUniformSliderMotionService.cs`
- `Em2RsModbusService` không triển khai `IUniformSliderMotionService`
- `ManualPage` không có panel `GHI 16 PR + START LOOP`

Vì vậy chương trình chỉ có JOG và MOVE số vòng; không có lệnh ghi PR0..PR15.

## Thay đổi trong bản này

- Thêm bộ tính quỹ đạo tay quay–con trượt với R=50 mm, L=100 mm, e=15 mm.
- Chia một vòng thành 16 PR, mỗi chiều con trượt có 8 đoạn bằng nhau theo quãng đường.
- Tính tốc độ RPM riêng cho từng PR để tốc độ trung bình của con trượt gần bằng giá trị mm/s đã đặt.
- Ghi PR0..PR15 bằng FC10, đọc lại PR0 và PR15 để xác nhận.
- PR0 -> PR1 -> ... -> PR15 -> PR0 tự Jump trong driver.
- Bật bit `OVLP` để driver chuyển tiếp tốc độ giữa các PR mà không giảm về 0 ở từng ranh giới.
- Máy tính chỉ gửi một lệnh START PR0; sau đó driver tự lặp nội bộ.
- Dùng dòng MANUAL và Pulse/vòng đã lưu trong SETTING.
- Giữ giới hạn dòng cứng tối đa 4,0 A.
- Bản service đi kèm có cơ chế tự mở lại COM đã lưu khi USB–RS485 bị reset tạm thời.

## Cách nhận biết đang chạy đúng bản

Trong MANUAL phải thấy card:

`TEST CON TRƯỢT GẦN ĐỀU — 16 PR`

và nút:

`GHI 16 PR + START LOOP`

Sau khi bấm START, log phải có các dòng:

- `[UNIFORM PR] ... START 16 PR tự Jump ... OVLP=ON`
- `[UNIFORM TABLE] ... PR0=... PR7=...`
- `[UNIFORM TABLE] ... PR8=... PR15=...`

Nếu không có card hoặc log trên thì đang chạy file EXE/build cũ.

## Trình tự test an toàn

1. Kết nối đúng một driver có gắn motor.
2. HOME thành công.
3. Trong SETTING kiểm tra Pulse/vòng đúng với driver, ví dụ 10.000.
4. Đặt dòng MANUAL phù hợp, không vượt 4,0 A.
5. Vào MANUAL, kéo xuống card 16 PR.
6. Dùng ban đầu: tốc độ con trượt 20 mm/s, PR Acc=1000, PR Dec=1000.
7. Chọn đúng vị trí cảm biến Home và chiều quay motor.
8. Bấm `GHI 16 PR + START LOOP`.
9. Dùng `QUICK STOP TEST` để dừng. Sau Quick Stop phải HOME lại.

## Giới hạn kỹ thuật

Chuyển động bằng 16 PR chỉ là gần đều. Tại đúng hai điểm chết, vận tốc tức thời của con trượt vẫn phải bằng 0. 16 đoạn và OVLP làm giảm rõ rệt biến thiên tốc độ, nhưng không thể tạo vận tốc toán học hoàn toàn không đổi trên toàn hành trình.
