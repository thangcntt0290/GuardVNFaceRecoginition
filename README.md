# GuardVN Face Recognition System

## ?? M� t? Project

GuardVN Face Recognition l� m?t h? th?ng nh?n di?n khu�n m?t ???c ph�t tri?n b?ng WPF (.NET 8) v?i OpenCV, h? tr? camera webcam v� RTSP streaming ?? gi�m s�t an ninh th�ng minh.

## ? T�nh n?ng ch�nh

### ?? Camera Streaming
- **Webcam Support**: H? tr? multiple webcam devices
- **RTSP Streaming**: K?t n?i v?i IP camera qua RTSP protocol
- **Maximum Resolution**: T? ??ng detect v� set ?? ph�n gi?i cao nh?t
- **Real-time Display**: Hi?n th? video real-time v?i 30 FPS

### ??? Giao di?n ng??i d�ng
- **Modern UI**: Thi?t k? Material Design hi?n ??i
- **MVVM Pattern**: Ki?n tr�c MVVM chu?n v?i WPF
- **Responsive Layout**: Giao di?n responsive, h? tr? full-screen
- **Multi-tab Navigation**: ?i?u h??ng gi?a c�c ch?c n?ng

### ?? Qu?n l� Camera
- **Add/Remove Camera**: Th�m/x�a camera ??ng
- **Auto Detection**: T? ??ng ph�t hi?n webcam c� s?n
- **Camera Validation**: Ki?m tra t�nh kh? d?ng c?a camera
- **Status Monitoring**: Theo d�i tr?ng th�i camera real-time

### ?? T�nh n?ng n�ng cao
- **Frame Capture**: Ch?p v� l?u ?nh ch?t l??ng cao
- **Error Handling**: X? l� l?i th�ng minh v� recovery
- **Threading**: Multi-threading an to�n cho UI
- **Resource Management**: Qu?n l� t�i nguy�n t?i ?u

## ??? C�ng ngh? s? d?ng

### Framework & Libraries
- **.NET 8**: Platform ch�nh
- **WPF**: Windows Presentation Foundation
- **OpenCV 4.11**: Computer Vision library
- **MVVM Pattern**: Model-View-ViewModel architecture

### NuGet Packages
```xml
<PackageReference Include="OpenCvSharp4.Windows" Version="4.11.0.20250507" />
<PackageReference Include="FFMpegCore" Version="5.1.0" />
<PackageReference Include="System.Drawing.Common" Version="8.0.0" />
```

## ?? C�i ??t v� ch?y

### Y�u c?u h? th?ng
- **OS**: Windows 10/11
- **.NET**: .NET 8 Runtime
- **Camera**: Webcam ho?c IP Camera h? tr? RTSP
- **Visual Studio**: 2022 ho?c m?i h?n (?? development)

### C�ch ch?y
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
- ?ng d?ng s? t? ??ng detect webcam c� s?n
- Ch?n camera t? ComboBox ? g�c tr�n

### 2. Start Camera Streaming
- Nh?n n�t **"? Start"** ?? b?t ??u streaming
- Video s? hi?n th? trong khung camera ch�nh
- Status bar s? hi?n th? th�ng tin streaming

### 3. Ch?p ?nh
- Nh?n n�t **"?? Capture"** khi camera ?ang ch?y
- ?nh s? ???c l?u v�o Desktop v?i format timestamp

### 4. Th�m camera m?i
- Nh?n **"? Add Camera"**
- Nh?p t�n camera v� th�ng tin k?t n?i:
  - **Webcam**: Nh?p ID (0, 1, 2...)
  - **RTSP**: Nh?p URL (rtsp://...)

### 5. Qu?n l� camera
- **Edit**: Ch?nh s?a th�ng tin camera
- **Remove**: X�a camera kh?i danh s�ch

## ??? Ki?n tr�c Project

### ?? C?u tr�c th? m?c
```
GuardVNFaceRecoginition/
??? Commands/           # ICommand implementations
??? Converters/         # Value converters for XAML
??? Models/            # Data models
??? Services/          # Business logic services
??? ViewModels/        # MVVM ViewModels
??? Views/             # XAML Views
??? Resources/         # Assets v� resources
```

### ?? Components ch�nh

#### Services
- **CameraService**: Qu?n l� camera streaming v?i OpenCV
- **CameraDetector**: Auto-detect available cameras
- **MessageService**: Hi?n th? dialogs v� notifications

#### ViewModels
- **MainWindowViewModel**: ViewModel ch�nh cho giao di?n
- **AddCameraWindowViewModel**: ViewModel cho dialog th�m camera

#### Models
- **Camera**: Model cho camera configuration
- **CameraStatus**: Status model v?i color coding
- **NavigationPage**: Enum cho tab navigation

## ?? X? l� l?i ph? bi?n

### Camera kh�ng kh?i ??ng ???c
- Ki?m tra camera c� b? ?ng d?ng kh�c s? d?ng
- Verify RTSP URL v� network connection
- Check Windows camera permissions

### Performance issues
- Gi?m resolution n?u m�y y?u
- ?�ng c�c ?ng d?ng camera kh�c
- Update driver camera

## ?? ?�ng g�p

M?i ?�ng g�p ??u ???c hoan ngh�nh! Vui l�ng:

1. Fork project
2. T?o feature branch: `git checkout -b feature/AmazingFeature`
3. Commit changes: `git commit -m 'Add some AmazingFeature'`
4. Push to branch: `git push origin feature/AmazingFeature`
5. T?o Pull Request

## ?? License

Distributed under the MIT License. See `LICENSE` for more information.

## ????? T�c gi?

**Th?ng Cao** - [@thangcntt0290](https://github.com/thangcntt0290)

Project Link: [https://github.com/thangcntt0290/GuardVNFaceRecoginition](https://github.com/thangcntt0290/GuardVNFaceRecoginition)

## ?? Acknowledgments

- [OpenCV](https://opencv.org/) - Computer Vision library
- [Microsoft WPF](https://github.com/dotnet/wpf) - UI Framework
- [Material Design](https://material.io/design) - Design principles

---

? **N?u project h?u �ch, ??ng qu�n star repository!** ?