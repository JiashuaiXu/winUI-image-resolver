# 13. EXIF 读取

EXIF (Exchangeable Image File Format) 是嵌入在图片中的元数据。

## 🎯 什么是 EXIF？

EXIF 包含图片的元信息：
- 相机信息（品牌、型号）
- 拍摄参数（ISO、光圈、快门）
- GPS 位置（如果有）
- 拍摄时间
- 其他元数据

## 📦 ExifLib 库

### 安装

项目已通过 NuGet 安装：
```xml
<PackageReference Include="ExifLib" Version="1.7.0" />
```

### 使用

```csharp
using ExifLib;

using (var reader = new ExifReader(filePath))
{
    // 读取 EXIF 数据
}
```

## 🔍 读取 EXIF 数据

### 基本用法

```csharp
using (var reader = new ExifReader(filePath))
{
    // 读取相机品牌
    if (reader.GetTagValue(ExifTags.Make, out string make))
    {
        imageInfo.CameraMake = make;
    }
    
    // 读取相机型号
    if (reader.GetTagValue(ExifTags.Model, out string model))
    {
        imageInfo.CameraModel = model;
    }
}
```

### 读取不同类型的数据

```csharp
// 字符串
if (reader.GetTagValue(ExifTags.Make, out string make))
    imageInfo.CameraMake = make;

// 日期时间
if (reader.GetTagValue(ExifTags.DateTime, out DateTime dateTime))
    imageInfo.DateTaken = dateTime;

// 数值
if (reader.GetTagValue(ExifTags.ISOSpeedRatings, out ushort iso))
    imageInfo.ISO = iso;

// 浮点数
if (reader.GetTagValue(ExifTags.FNumber, out double fNumber))
    imageInfo.FNumber = fNumber;
```

## 📋 项目中的实现

### 完整示例

```csharp
private void ReadExifData(string filePath, ImageInfo imageInfo)
{
    try
    {
        using (var reader = new ExifReader(filePath))
        {
            // 相机信息
            reader.GetTagValue(ExifTags.Make, out string make);
            reader.GetTagValue(ExifTags.Model, out string model);
            imageInfo.CameraMake = make;
            imageInfo.CameraModel = model;
            
            // 拍摄时间
            if (reader.GetTagValue(ExifTags.DateTime, out DateTime dateTime))
                imageInfo.DateTaken = dateTime;
            
            // GPS 信息
            if (reader.GetTagValue(ExifTags.GPSLatitude, out double latitude) &&
                reader.GetTagValue(ExifTags.GPSLongitude, out double longitude))
            {
                imageInfo.Latitude = latitude;
                imageInfo.Longitude = longitude;
            }
            
            // 相机设置
            if (reader.GetTagValue(ExifTags.ISOSpeedRatings, out ushort iso))
                imageInfo.ISO = iso;
            
            if (reader.GetTagValue(ExifTags.FNumber, out double fNumber))
                imageInfo.FNumber = fNumber;
        }
    }
    catch
    {
        // 图片可能没有 EXIF 数据，静默处理
    }
}
```

## 🗺️ GPS 坐标处理

### 读取 GPS 数据

```csharp
if (reader.GetTagValue(ExifTags.GPSLatitude, out double latitude) &&
    reader.GetTagValue(ExifTags.GPSLongitude, out double longitude))
{
    imageInfo.Latitude = latitude;
    imageInfo.Longitude = longitude;
}
```

**说明**:
- 不是所有图片都有 GPS 数据
- 需要相机支持 GPS 功能

## ⚠️ 注意事项

### 1. 不是所有图片都有 EXIF

```csharp
try
{
    using (var reader = new ExifReader(filePath))
    {
        // 读取数据
    }
}
catch
{
    // 图片可能没有 EXIF，这是正常的
}
```

### 2. 数据类型转换

```csharp
// ExifTags.ISOSpeedRatings 返回 ushort
if (reader.GetTagValue(ExifTags.ISOSpeedRatings, out ushort iso))
    imageInfo.ISO = (int)iso; // 转换为 int
```

### 3. 过时的标签

```csharp
// ISOSpeedRatings 已过时，建议使用 PhotographicSensitivity
// 但为了兼容性，仍可使用
if (reader.GetTagValue(ExifTags.ISOSpeedRatings, out ushort iso))
    imageInfo.ISO = iso;
```

## 📚 常用 EXIF 标签

| 标签 | 类型 | 说明 |
|------|------|------|
| Make | string | 相机品牌 |
| Model | string | 相机型号 |
| DateTime | DateTime | 拍摄时间 |
| GPSLatitude | double | 纬度 |
| GPSLongitude | double | 经度 |
| ISOSpeedRatings | ushort | ISO 感光度 |
| FNumber | double | 光圈值 |
| ExposureTime | double | 曝光时间 |
| FocalLength | double | 焦距 |

## 📚 下一步

- 查看 CSV 导出 → [14-CSV导出](./14-CSV导出.md)
- 学习核心类 → [15-核心类解析](./15-核心类解析.md)

---

**继续学习**: [14-CSV导出](./14-CSV导出.md) →

