# Image Resolver - Design Document

## 项目概述

Image Resolver 是一个基于 WinUI 3 的 Windows 原生桌面应用程序，用于批量提取图片的分辨率信息和 EXIF 元数据，并支持导出为 CSV 格式。

## 技术栈

- **框架**: WinUI 3 (Windows App SDK 1.5)
- **.NET 版本**: .NET 6.0
- **目标平台**: Windows 10/11 (最低版本 10.0.17763.0)
- **依赖库**:
  - ExifLib 1.7.0 - EXIF 数据读取
  - CsvHelper 30.0.1 - CSV 导出

## 架构设计

### 项目结构

```
ImageResolver/
├── Models/
│   └── ImageInfo.cs          # 图片信息数据模型
├── Services/
│   ├── ImageInfoService.cs   # 图片信息提取服务
│   └── CsvExportService.cs   # CSV 导出服务
├── App.xaml                   # 应用程序入口
├── MainWindow.xaml            # 主窗口 UI
└── MainWindow.xaml.cs         # 主窗口逻辑
```

### 核心组件

#### 1. ImageInfo 模型
存储图片的完整信息：
- 基本信息：文件名、路径、分辨率、文件大小
- EXIF 信息：相机信息、拍摄日期、GPS 坐标、相机设置等

#### 2. ImageInfoService
负责图片信息提取：
- 使用 `Windows.Graphics.Imaging` 获取图片尺寸
- 使用 `ExifLib` 读取 EXIF 元数据
- 支持异步批量处理

#### 3. CsvExportService
负责 CSV 导出：
- 使用 `CsvHelper` 生成标准 CSV 文件
- 包含所有图片信息和 EXIF 数据字段

## UI 设计规范

### 设计原则

1. **现代化**: 采用 Fluent Design System 设计语言
2. **响应式**: 支持窗口大小调整，自适应布局
3. **直观性**: 清晰的信息层次和交互反馈
4. **美观性**: 使用 Acrylic 材质、圆角卡片、合理间距

### 布局结构

#### 三栏布局
- **左侧栏 (40%)**: 图片列表
  - 卡片式展示
  - 缩略图 + 文件名 + 基本信息
  - 可滚动列表
  
- **中间**: 8px 分隔线

- **右侧栏 (60%)**: 详细信息面板
  - 分类卡片展示
  - 可滚动内容
  - 动态显示选中图片的详细信息

#### 顶部 Header
- 应用标题和描述
- 文件夹选择按钮（Accent 样式）

#### 底部 Footer
- 状态文本和进度指示器
- CSV 导出按钮（Accent 样式）

### 视觉规范

#### 颜色系统
- **主题色**: 使用系统 Accent 颜色
- **背景**: Acrylic 材质（SystemControlAcrylicWindowBrush）
- **卡片**: 白色/浅色背景，1px 边框
- **文本**: 使用系统文本颜色，支持深色/浅色主题

#### 间距系统
- **页面边距**: 24px
- **组件间距**: 16px
- **卡片内边距**: 16px
- **按钮内边距**: 24px (水平) × 12px (垂直)

#### 字体系统
- **标题**: 32px, SemiBold
- **副标题**: 20px, SemiBold
- **正文**: 14px, Regular
- **说明文字**: 12px, Regular

#### 圆角规范
- **卡片**: 8px
- **按钮**: 系统默认
- **缩略图容器**: 4px

### 交互设计

#### 图片选择
- 点击图片卡片高亮选中
- 右侧面板实时更新详细信息

#### 加载状态
- 显示进度环（ProgressRing）
- 更新状态文本
- 禁用相关按钮防止重复操作

#### 错误处理
- 静默处理单个图片错误，继续处理其他图片
- 在状态栏显示错误信息

## 功能特性

### 支持的功能

1. **文件夹选择**: 使用 Windows 文件选择器
2. **批量处理**: 自动扫描文件夹中的所有图片
3. **信息提取**:
   - 图片分辨率（宽度 × 高度）
   - 文件大小
   - EXIF 元数据（相机信息、GPS、拍摄参数等）
4. **详细信息展示**: 分类展示图片的所有信息
5. **CSV 导出**: 导出所有图片信息到 CSV 文件

### 支持的图片格式

- JPEG/JPG
- PNG
- BMP
- TIFF/TIF
- GIF

### CSV 导出字段

| 字段名 | 说明 |
|--------|------|
| FileName | 文件名 |
| FilePath | 完整路径 |
| Width | 宽度（像素） |
| Height | 高度（像素） |
| Resolution | 分辨率字符串 |
| FileSize (bytes) | 文件大小（字节） |
| FileSize (Formatted) | 格式化文件大小 |
| Camera Make | 相机品牌 |
| Camera Model | 相机型号 |
| Date Taken | 拍摄日期 |
| Orientation | 方向 |
| Latitude | 纬度 |
| Longitude | 经度 |
| Software | 软件信息 |
| Artist | 作者 |
| Copyright | 版权信息 |
| ISO | ISO 感光度 |
| F-Number | 光圈值 |
| Exposure Time | 曝光时间 |
| Focal Length | 焦距 |

## 性能考虑

1. **异步处理**: 所有 I/O 操作使用异步方法
2. **批量处理**: 支持批量处理多张图片
3. **错误隔离**: 单个图片错误不影响整体处理
4. **内存管理**: 及时释放图片流资源

## 扩展性

- 模块化设计，易于添加新的信息提取功能
- 服务层独立，便于单元测试
- UI 与业务逻辑分离

## 部署要求

- Windows 10 版本 1809 或更高版本
- Windows App Runtime 1.5 或更高版本
- .NET 6.0 Runtime

