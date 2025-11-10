# Image Resolver

一个基于 WinUI 3 的 Windows 原生桌面应用程序，用于批量提取图片的分辨率信息和 EXIF 元数据，并支持导出为 CSV 格式。

## 功能特性

- 📸 批量提取图片分辨率信息
- 📋 读取完整的 EXIF 元数据（相机信息、GPS、拍摄参数等）
- 📊 美观的现代化 UI 界面
- 💾 导出为 CSV 格式，便于数据分析
- 🎨 支持 Windows 深色/浅色主题

## 系统要求

- Windows 10 版本 1809 (17763) 或更高版本
- Windows 11
- .NET 6.0 Runtime
- Windows App Runtime 1.5 或更高版本

## 构建和运行

### 前置条件

确保已安装：
- .NET 6.0 SDK 或更高版本
- Visual Studio 2022 或 Visual Studio Code
- Windows App SDK

### 构建步骤

1. 克隆或下载项目
2. 在项目目录打开终端
3. 还原 NuGet 包：
   ```bash
   dotnet restore
   ```
4. 构建项目：
   ```bash
   dotnet build
   ```
5. 运行应用：
   ```bash
   dotnet run
   ```

### Visual Studio

1. 打开 `ImageResolver.csproj`
2. 按 F5 运行

## 使用说明

1. **选择文件夹**: 点击 "Select Folder" 按钮，选择包含图片的文件夹
2. **查看列表**: 左侧显示所有检测到的图片，包含缩略图和基本信息
3. **查看详情**: 点击任意图片，右侧面板显示完整的详细信息
4. **导出 CSV**: 点击 "Export to CSV" 按钮，选择保存位置，导出所有图片信息

## 支持的图片格式

- JPEG/JPG
- PNG
- BMP
- TIFF/TIF
- GIF

## CSV 导出字段

导出的 CSV 文件包含以下字段：
- 文件名和路径
- 分辨率信息（宽度、高度）
- 文件大小
- 相机信息（品牌、型号）
- 拍摄日期和方向
- GPS 坐标（如果可用）
- 相机设置（ISO、光圈、曝光时间、焦距）
- 元数据（软件、作者、版权）

## 技术栈

- **WinUI 3** - 现代化 Windows UI 框架
- **.NET 6.0** - 运行时和 SDK
- **ExifLib** - EXIF 数据读取
- **CsvHelper** - CSV 文件生成

## 项目结构

```
ImageResolver/
├── Models/              # 数据模型
├── Services/            # 业务逻辑服务
├── App.xaml            # 应用程序入口
├── MainWindow.xaml     # 主窗口 UI
└── MainWindow.xaml.cs  # 主窗口逻辑
```

## 文档

- [设计文档](DESIGN.md) - 详细的设计文档和规范
- [快速开始](QUICKSTART.md) - 快速上手指南
- [发布文档](PUBLISH.md) - 打包发布说明
- [学习指南](docs/learning/README.md) - 新手学习教程（推荐！）

### 📚 学习指南

专为新手设计的学习文档，从零开始学习整个项目：

- 🚀 [入门篇](docs/learning/README.md#-入门篇) - 项目简介、环境搭建、快速开始
- 🏗️ [架构篇](docs/learning/README.md#-架构篇) - 项目架构、结构、设计模式
- 💻 [技术栈篇](docs/learning/README.md#-技术栈篇) - WinUI 3、XAML、C# 异步、数据绑定
- 🔧 [实现篇](docs/learning/README.md#-实现篇) - UI 实现、图片处理、EXIF 读取、CSV 导出

**开始学习**: [docs/learning/README.md](docs/learning/README.md)

## 许可证

MIT License

