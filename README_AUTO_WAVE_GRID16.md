# AUTO Wave Grid 16x16 — revision: constant motor speed

Phần AUTO dùng bản đồ cơ cấu 16x16 và cho phép nhiều cụm độc lập.

## Quy tắc đã chốt
- Mỗi cụm có kích thước Dài x Rộng và đặt tại một vị trí bất kỳ trên Grid 16x16.
- Mỗi ô trong cụm **bắt buộc** phải có Driver ID dạng `1.1` ... `4.16` trước khi START.
- Một Driver ID không được gán trùng trong nhiều ô/cụm.
- Khi START, **100% driver của từng cụm phải Online**.
- Tất cả driver phải ở pha 0: đã HOME hoặc vừa dùng chức năng **LẤY VỊ TRÍ HIỆN TẠI = GỐC**.
- HOME được hiểu là pha 0 với con trượt ở phía trong.
- Hiệu ứng hiện có: **Sóng từ tâm — vòng chữ nhật đồng tâm**.
- Cụm chẵn có nhiều ô trung tâm cùng Layer 0.
- Mỗi cụm có tốc độ motor riêng, nhập theo `vòng/s`; driver chạy theo RPM tương ứng.
- Lớp kế tiếp bắt đầu trễ một lượng `LayerOffsetRevolutions / MotorSpeedRps`.
- PAUSE dừng motor và đóng băng tiến trình thời gian của hiệu ứng; RESUME tiếp tục các layer đã chạy và tiếp tục lịch các layer chưa khởi động.
- STOP không chạy HOME.

## Giai đoạn hiện tại: motor quay đều
AUTO **chưa bù hình học slider-crank**. Motor quay đều một chiều.

Để tránh stream target vị trí liên tục từ PC qua RS485, service nạp cho mỗi EM2RS một vòng PR tự Jump gồm 16 đoạn góc bằng nhau, tất cả cùng RPM. Tổng pulse của 16 đoạn bằng đúng PPR của driver. Đây chỉ là cơ chế tạo vòng quay đều cục bộ trong driver, không phải bảng 16 PR bù tốc độ con trượt.

Sau khi nạp bảng, PC chỉ phát lệnh START theo lịch Layer. Vì vậy tải bus trong lúc chạy giảm rất nhiều so với kiến trúc stream vị trí cũ.

## Layer vòng chữ nhật
Ví dụ 5x5:

```text
2 2 2 2 2
2 1 1 1 2
2 1 0 1 2
2 1 1 1 2
2 2 2 2 2
```

Ví dụ 4x4:

```text
1 1 1 1
1 0 0 1
1 0 0 1
1 1 1 1
```

## Lấy vị trí hiện tại làm gốc
Nút AUTO ORIGIN áp dụng cho **cụm đang chọn**:
1. Cụm phải gán đủ Driver ID.
2. Tất cả driver trong cụm phải Online.
3. Service Quick Stop từng driver trước khi đặt gốc.
4. Ghi `0x6002 = 0x0021` để vị trí hiện tại trở thành 0.
5. Đọc lại Actual Position và chỉ xác nhận thành công khi sai số <= 2 pulse.

Sau thao tác này cụm có thể START mà không cần chạy HOME sensor.

## UI
- Sidebar trái và phải có vùng cuộn riêng để không còn control/panel chồng lên nhau khi chiều cao cửa sổ hoặc DPI thay đổi.
- Tốc độ motor nằm trong phần HIỆU ỨNG CỤM và được lưu riêng theo cụm đang chọn.
- Hiển thị đồng thời vòng/s và RPM.
- Grid dùng màu chữ để báo nhanh: đỏ = Offline, vàng = chưa ở pha 0, trắng = sẵn sàng.
- AUTO START bị khóa bằng validation nếu còn ô thiếu ID, driver Offline hoặc driver chưa ở pha 0.

## Giới hạn của revision này
- Chưa chạy build/hardware test trong môi trường tạo bản sửa vì máy này không có .NET SDK và không có EM2RS thật.
- Các lệnh START cho nhiều slave trên cùng một RS485 line vẫn phải phát tuần tự theo frame Modbus, nên “cùng layer” là đồng thời ở mức điều khiển HMI, không phải đồng bộ phần cứng tuyệt đối microsecond.
- Ramp tăng/giảm chưa được đưa vào vòng AUTO revision này; ưu tiên đầu tiên là làm motor quay đều, START/PAUSE/STOP và hiệu ứng layer hoạt động ổn định.
