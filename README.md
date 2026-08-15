# Wave Motion Control — WinForms C#

HMI cho tối đa 64 driver Leadshine EM2RS: 4 line RS485 × 16 driver.

## Môi trường

- Visual Studio 2022 có workload **.NET Desktop Development**
- .NET 9 SDK
- Target framework: `net9.0-windows`

## Chạy dự án

1. Mở `WaveMotionControl.sln`.
2. Chọn project `WaveMotionControl` làm Startup Project.
3. Xóa `bin` và `obj` nếu vừa thay code.
4. Chọn `Debug | Any CPU` rồi nhấn `F5`.

`Program.cs` đang khởi tạo `Em2RsModbusService`, tức là dùng cổng COM và Modbus RTU thật.

## Các màn hình

- **MAIN**: Connect/Disconnect, HOME và trạng thái driver.
- **MANUAL**: JOG, MOVE số vòng và test con trượt gần đều bằng 16 PR tự Jump.
- **AUTO**: điều khiển hiệu ứng tự động.
- **STATUS**: trạng thái hệ thống.
- **SETTING**: dòng HOME/MANUAL/AUTO, Pulse/vòng, HOME, DI và EEPROM.

Đọc `README_UNIFORM_PR_FIX.md` trước khi thử chức năng 16 PR.
