# GuardVN Face Recognition System

## 📋 Mô tả Project

GuardVN Face Recognition là hệ thống nhận diện khuôn mặt được phát triển bằng WPF (.NET 8) với OpenCV, hỗ trợ camera webcam và RTSP streaming để giám sát an ninh thông minh.

## ✨ Tính năng chính

### 🎥 Camera Streaming
- **Webcam Support**: Hỗ trợ multiple webcam devices
- **RTSP Streaming**: Kết nối với IP camera qua RTSP protocol
- **Maximum Resolution**: Tự động detect và set độ phân giải cao nhất
- **Real-time Display**: Hiển thị video real-time với 30 FPS

### 🖥️ Giao diện người dùng
- **Modern UI**: Thiết kế Material Design hiện đại
- **MVVM Pattern**: Kiến trúc MVVM chuẩn với WPF
- **Responsive Layout**: Giao diện responsive, hỗ trợ full-screen
- **Multi-tab Navigation**: Điều hướng giữa các chức năng

### 📷 Quản lý Camera
- **Add/Remove Camera**: Thêm/xóa camera động
- **Auto Detection**: Tự động phát hiện webcam có sẵn
- **Camera Validation**: Kiểm tra tính khả dụng của camera
- **Status Monitoring**: Theo dõi trạng thái camera real-time

### 🎯 Tính năng nâng cao
- **Frame Capture**: Chụp và lưu ảnh chất lượng cao
- **Error Handling**: Xử lý lỗi thông minh và recovery
- **Threading**: Multi-threading an toàn cho UI
- **Resource Management**: Quản lý tài nguyên tối ưu

## 🛠️ Công nghệ sử dụng

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

## 🚀 Installation & Setup

### System Requirements
- **OS**: Windows 10/11
- **.NET**: .NET 8 Runtime
- **Camera**: Webcam or RTSP-compatible IP Camera
- **Visual Studio**: 2022 or newer (for development)

### Getting Started
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

## 📖 User Guide

### 1. Application Startup
- Application automatically detects available webcams
- Select camera from ComboBox in the header

### 2. Start Camera Streaming
- Click **"▶ Start"** button to begin streaming
- Video will display in the main camera area
- Status bar shows streaming information

### 3. Capture Images
- Click **"📷 Capture"** button while camera is running
- Images are saved to Desktop with timestamp format

### 4. Add New Camera
- Click **"➕ Add Camera"**
- Enter camera name and connection details:
  - **Webcam**: Enter device ID (0, 1, 2...)
  - **RTSP**: Enter RTSP URL (rtsp://...)

### 5. Camera Management
- **Edit**: Modify camera settings (coming soon)
- **Remove**: Delete camera from list

## 🏗️ Project Architecture

### 📁 Directory Structure
```
GuardVNFaceRecoginition/
├── Commands/           # ICommand implementations
├── Converters/         # Value converters for XAML
├── Models/            # Data models
├── Services/          # Business logic services
├── ViewModels/        # MVVM ViewModels
├── Views/             # XAML Views
└── Resources/         # Assets and resources
```

### 🔧 Key Components

#### Services
- **CameraService**: Manages camera streaming with OpenCV
- **CameraDetector**: Auto-detect available cameras
- **MessageService**: Display dialogs and notifications

#### ViewModels
- **MainWindowViewModel**: Main UI ViewModel
- **AddCameraWindowViewModel**: Add camera dialog ViewModel

#### Models
- **Camera**: Camera configuration model
- **CameraStatus**: Status model with color coding
- **NavigationPage**: Navigation tab enumeration

## 🐛 Troubleshooting

### Camera Won't Start
- Check if camera is being used by another application
- Verify RTSP URL and network connection
- Check Windows camera permissions in Settings

### Performance Issues
- Lower resolution if system is slow
- Close other camera applications
- Update camera drivers

## 🤝 Contributing

All contributions are welcome! Please:

1. Fork the project
2. Create feature branch: `git checkout -b feature/AmazingFeature`
3. Commit changes: `git commit -m 'Add some AmazingFeature'`
4. Push to branch: `git push origin feature/AmazingFeature`
5. Create Pull Request

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.

## 👨‍💻 Author

**Thang Nguyen** - [@thangcntt0290](https://github.com/thangcntt0290)

Project Link: [https://github.com/thangcntt0290/GuardVNFaceRecoginition](https://github.com/thangcntt0290/GuardVNFaceRecoginition)

## 🙏 Acknowledgments

- [OpenCV](https://opencv.org/) - Computer Vision library
- [Microsoft WPF](https://github.com/dotnet/wpf) - UI Framework
- [Material Design](https://material.io/design) - Design principles

---
