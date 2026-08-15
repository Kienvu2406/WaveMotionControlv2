# Test 16 PR: dòng riêng và lấy gốc cơ khí

## Dòng riêng cho chế độ 16 PR

Trong MANUAL > TEST CON TRƯỢT GẦN ĐỀU — 16 PR có ô:

- `Dòng Peak test 16 PR (A)`
- Giới hạn: 0,5–4,0 A
- Khi START, phần mềm ghi giá trị này vào 0x0191 và đọc lại để xác nhận.
- Giá trị chỉ áp dụng khi chạy test, không ghi EEPROM.

## Điểm gốc cơ khí chuẩn

Với R=50 mm, L=100 mm, e=15 mm (tâm tay quay thấp hơn tâm con trượt):

- Đầu ngoài: A-B-C thẳng hàng, B nằm giữa A và C.
  - x = 149,248 mm
  - góc tay quay = 5,739°
- Đầu trong: B-A-C thẳng hàng, A nằm giữa B và C.
  - x = 47,697 mm
  - góc tay quay = 197,458°

Không lấy gốc ở 0° hoặc 180° vì cơ cấu lệch tâm 15 mm.

## Trình tự hiệu chuẩn

1. Chọn đúng `Home đang ở`: đầu ngoài hoặc đầu trong.
2. HOME bằng DI5 để tìm vị trí lặp lại gần điểm gốc.
3. JOG thật chậm đến khi A-B-C thẳng hàng đúng như mô tả trên.
4. Dừng motor hoàn toàn.
5. Bấm `ĐẶT GỐC TẠI ĐÂY` và xác nhận.
6. Kiểm tra trạng thái chuyển thành HOMED và log có `[UNIFORM ZERO] ... = 0 pulse`.
7. Đặt dòng Peak test, tốc độ con trượt, Acc/Dec rồi bấm `GHI 16 PR + START LOOP`.

Nút đặt gốc ghi 0x0021 vào 0x6002, đặt tọa độ hiện tại của driver thành 0. Sau khi mất nguồn, thay đổi cơ khí hoặc Quick Stop giữa chu kỳ, cần lấy gốc lại trước khi chạy.
