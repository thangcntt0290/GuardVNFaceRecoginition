# GuardVN Face Recognition System

## ?? Mô t? Project

GuardVN Face Recognition là m?t h? th?ng nh?n di?n khuôn m?t ???c phát tri?n b?ng WPF (.NET 8) v?i OpenCV, h? tr? camera webcam và RTSP streaming ?? giám sát an ninh thông minh.

## ? Tính n?ng chính

### ?? Camera Streaming
- **Webcam Support**: H? tr? multiple webcam devices
- **RTSP Streaming**: K?t n?i v?i IP camera qua RTSP protocol
- **Maximum Resolution**: T? ??ng detect và set ?? phân gi?i cao nh?t
- **Real-time Display**: Hi?n th? video real-time v?i 30 FPS

### ??? Giao di?n ng??i dùng
- **Modern UI**: Thi?t k? Material Design hi?n ??i
- **MVVM Pattern**: Ki?n trúc MVVM chu?n v?i WPF
- **Responsive Layout**: Giao di?n responsive, h? tr? full-screen
- **Multi-tab Navigation**: ?i?u h??ng gi?a các ch?c n?ng

### ?? Qu?n lý Camera
- **Add/Remove Camera**: Thêm/xóa camera ??ng
- **Auto Detection**: T? ??ng phát hi?n webcam có s?n
- **Camera Validation**: Ki?m tra tính kh? d?ng c?a camera
- **Status Monitoring**: Theo dõi tr?ng thái camera real-time

### ?? Tính n?ng nâng cao
- **Frame Capture**: Ch?p và l?u ?nh ch?t l??ng cao
- **Error Handling**: X? lý l?i thông minh và recovery
- **Threading**: Multi-threading an toàn cho UI
- **Resource Management**: Qu?n lý tài nguyên t?i ?u

## ??? Công ngh? s? d?ng

### Framework & Libraries
- **.NET 8**: Platform chính
- **WPF**: Windows Presentation Foundation
- **OpenCV 4.11**: Computer Vision library
- **MVVM Pattern**: Model-View-ViewModel architecture

### NuGet Packages
```xml
<PackageReference Include="OpenCvSharp4.Windows" Version="4.11.0.20250507" />
<PackageReference Include="FFMpegCore" Version="5.1.0" />
<PackageReference Include="System.Drawing.Common" Version="8.0.0" />
```

## ?? Cài ??t và ch?y

### Yêu c?u h? th?ng
- **OS**: Windows 10/11
- **.NET**: .NET 8 Runtime
- **Camera**: Webcam ho?c IP Camera h? tr? RTSP
- **Visual Studio**: 2022 ho?c m?i h?n (?? development)

### Cách ch?y
1. Clone repository:
```bash
git clone https://github.com/thangcntt0290/GuardVNFaceRecoginition.git
cd GuardVNFaceRecoginition
```

2. Restore NuGet packages:
```bash
dotnet restore
```

3. Build project:
```bash
dotnet build
```

4. Run application:
```bash
dotnet run
```

## ?? H??ng d?n s? d?ng

### 1. Kh?i ??ng ?ng d?ng
- ?ng d?ng s? t? ??ng detect webcam có s?n
- Ch?n camera t? ComboBox ? góc trên

### 2. Start Camera Streaming
- Nh?n nút **"? Start"** ?? b?t ??u streaming
- Video s? hi?n th? trong khung camera chính
- Status bar s? hi?n th? thông tin streaming

### 3. Ch?p ?nh
- Nh?n nút **"?? Capture"** khi camera ?ang ch?y
- ?nh s? ???c l?u vào Desktop v?i format timestamp

### 4. Thêm camera m?i
- Nh?n **"? Add Camera"**
- Nh?p tên camera và thông tin k?t n?i:
  - **Webcam**: Nh?p ID (0, 1, 2...)
  - **RTSP**: Nh?p URL (rtsp://...)

### 5. Qu?n lý camera
- **Edit**: Ch?nh s?a thông tin camera
- **Remove**: Xóa camera kh?i danh sách

## ??? Ki?n trúc Project

### ?? C?u trúc th? m?c
```
GuardVNFaceRecoginition/
??? Commands/           # ICommand implementations
??? Converters/         # Value converters for XAML
??? Models/            # Data models
??? Services/          # Business logic services
??? ViewModels/        # MVVM ViewModels
??? Views/             # XAML Views
??? Resources/         # Assets và resources
```

### ?? Components chính

#### Services
- **CameraService**: Qu?n lý camera streaming v?i OpenCV
- **CameraDetector**: Auto-detect available cameras
- **MessageService**: Hi?n th? dialogs và notifications

#### ViewModels
- **MainWindowViewModel**: ViewModel chính cho giao di?n
- **AddCameraWindowViewModel**: ViewModel cho dialog thêm camera

#### Models
- **Camera**: Model cho camera configuration
- **CameraStatus**: Status model v?i color coding
- **NavigationPage**: Enum cho tab navigation

## ?? X? lý l?i ph? bi?n

### Camera không kh?i ??ng ???c
- Ki?m tra camera có b? ?ng d?ng khác s? d?ng
- Verify RTSP URL và network connection
- Check Windows camera permissions

### Performance issues
- Gi?m resolution n?u máy y?u
- ?óng các ?ng d?ng camera khác
- Update driver camera

## ?? ?óng góp

M?i ?óng góp ??u ???c hoan nghênh! Vui lòng:

1. Fork project
2. T?o feature branch: `git checkout -b feature/AmazingFeature`
3. Commit changes: `git commit -m 'Add some AmazingFeature'`
4. Push to branch: `git push origin feature/AmazingFeature`
5. T?o Pull Request

## ?? License

Distributed under the MIT License. See `LICENSE` for more information.

## ????? Tác gi?

**Th?ng Cao** - [@thangcntt0290](https://github.com/thangcntt0290)

Project Link: [https://github.com/thangcntt0290/GuardVNFaceRecoginition](https://github.com/thangcntt0290/GuardVNFaceRecoginition)

## ?? Acknowledgments

- [OpenCV](https://opencv.org/) - Computer Vision library
- [Microsoft WPF](https://github.com/dotnet/wpf) - UI Framework
- [Material Design](https://material.io/design) - Design principles

---

? **N?u project h?u ích, ??ng quên star repository!** ?