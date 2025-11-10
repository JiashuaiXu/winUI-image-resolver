# Image Resolver 打包发布说明

本文档详细说明如何打包和发布 Image Resolver 应用程序。

## 📋 目录

- [发布方式概览](#发布方式概览)
- [快速开始](#快速开始)
- [详细发布步骤](#详细发布步骤)
- [发布文件说明](#发布文件说明)
- [分发指南](#分发指南)
- [常见问题](#常见问题)

---

## 发布方式概览

Image Resolver 支持以下三种发布方式：

| 发布方式 | 优点 | 缺点 | 适用场景 |
|---------|------|------|---------|
| **自包含发布** | 文件完整，易于分发 | 文件较大（~100MB） | 个人分发、企业内部分发 |
| **单文件发布** | 单个exe，最便携 | 首次启动较慢 | 个人使用、快速分享 |
| **MSIX 打包** | 现代化安装体验 | 需要签名证书 | Microsoft Store、企业分发 |

---

## 快速开始

### 使用发布脚本（推荐）

```powershell
# 发布所有版本
powershell -ExecutionPolicy Bypass -File publish.ps1 -PublishType All

# 仅发布自包含版本
powershell -ExecutionPolicy Bypass -File publish.ps1 -PublishType SelfContained

# 仅发布单文件版本
powershell -ExecutionPolicy Bypass -File publish.ps1 -PublishType SingleFile
```

### 手动发布

```powershell
# 自包含发布
dotnet publish ImageResolver.csproj -c Release -r win-x64 --self-contained true -p:WindowsAppSDKSelfContained=true -o ./publish/SelfContained

# 单文件发布
dotnet publish ImageResolver.csproj -c Release -r win-x64 --self-contained true -p:WindowsAppSDKSelfContained=true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish/SingleFile
```

---

## 详细发布步骤

### 方式 1: 自包含发布（Self-Contained）

**特点：**
- 包含所有运行时依赖
- 无需安装 .NET Runtime
- 文件较大但运行稳定

**步骤：**

1. **执行发布命令**
   ```powershell
   dotnet publish ImageResolver.csproj `
     -c Release `
     -r win-x64 `
     --self-contained true `
     -p:WindowsAppSDKSelfContained=true `
     -p:PublishSingleFile=false `
     -o ./publish/SelfContained/win-x64
   ```

2. **发布文件位置**
   ```
   publish/
   └── SelfContained/
       └── win-x64/
           ├── ImageResolver.exe
           ├── ImageResolver.dll
           ├── README.txt
           └── [其他运行时文件]
   ```

3. **分发方式**
   - 将整个 `win-x64` 文件夹压缩为 ZIP
   - 用户解压后直接运行 `ImageResolver.exe`

**系统要求：**
- Windows 10 版本 1809 (17763) 或更高版本
- Windows 11
- 无需安装 .NET Runtime

---

### 方式 2: 单文件发布（Single File）

**特点：**
- 单个 exe 文件，最便携
- 首次启动需要解压，可能稍慢
- 适合快速分享

**步骤：**

1. **执行发布命令**
   ```powershell
   dotnet publish ImageResolver.csproj `
     -c Release `
     -r win-x64 `
     --self-contained true `
     -p:WindowsAppSDKSelfContained=true `
     -p:PublishSingleFile=true `
     -p:IncludeNativeLibrariesForSelfExtract=true `
     -p:EnableCompressionInSingleFile=true `
     -o ./publish/SingleFile/win-x64
   ```

2. **发布文件位置**
   ```
   publish/
   └── SingleFile/
       └── win-x64/
           ├── ImageResolver.exe  (单个文件，约 80-100MB)
           └── README.txt
   ```

3. **分发方式**
   - 直接分发 `ImageResolver.exe`
   - 用户双击运行即可

**注意事项：**
- 首次启动可能需要 3-5 秒解压时间
- 文件会被解压到临时目录
- 杀毒软件可能误报（因为是自解压文件）

---

### 方式 3: MSIX 打包

**特点：**
- 现代化安装体验
- 自动更新支持
- 适合 Microsoft Store 发布

**前置条件：**
- ✅ 已创建 `Package.appxmanifest` 文件
- ✅ 已准备应用图标（在 `Assets/` 目录）
- ✅ 需要代码签名证书（用于发布到 Store）

**步骤：**

#### 3.1 使用 Visual Studio（推荐）

1. 打开项目文件 `ImageResolver.csproj`
2. 右键项目 → **发布** → **创建应用包**
3. 选择 **Microsoft Store** 或 **旁加载**
4. 按照向导完成打包

#### 3.2 使用命令行

1. **修改项目文件**
   在 `ImageResolver.csproj` 中设置：
   ```xml
   <WindowsPackageType>Msix</WindowsPackageType>
   ```

2. **发布应用**
   ```powershell
   dotnet publish ImageResolver.csproj `
     -c Release `
     -r win-x64 `
     --self-contained true `
     -p:WindowsAppSDKSelfContained=true `
     -p:WindowsPackageType=Msix `
     -o ./publish/MSIX/win-x64
   ```

3. **使用 MakeAppx.exe 打包**
   ```powershell
   # 需要安装 Windows SDK
   MakeAppx.exe pack /d publish\MSIX\win-x64 /p ImageResolver.msix
   ```

4. **签名（可选，但推荐）**
   ```powershell
   # 使用证书签名
   SignTool.exe sign /fd SHA256 /a /f YourCertificate.pfx /p YourPassword ImageResolver.msix
   ```

**发布到 Microsoft Store：**

1. 登录 [Partner Center](https://partner.microsoft.com/dashboard)
2. 创建新应用或选择现有应用
3. 上传 MSIX 包
4. 填写应用信息、截图、描述等
5. 提交审核

---

## 发布文件说明

### 文件结构

```
publish/
├── SelfContained/
│   └── win-x64/
│       ├── ImageResolver.exe          # 主程序
│       ├── ImageResolver.dll          # 程序集
│       ├── ImageResolver.runtimeconfig.json
│       ├── Microsoft.WindowsAppSDK.dll
│       ├── Microsoft.WinUI.dll
│       ├── [其他运行时 DLL]
│       └── README.txt                 # 使用说明
│
├── SingleFile/
│   └── win-x64/
│       ├── ImageResolver.exe          # 单文件（包含所有依赖）
│       └── README.txt
│
└── MSIX/
    └── win-x64/
        ├── ImageResolver.exe
        ├── Package.appxmanifest
        └── Assets/
            ├── StoreLogo.png
            ├── Square44x44Logo.png
            ├── Square150x150Logo.png
            └── Wide310x150Logo.png
```

### 文件大小参考

| 发布方式 | 文件大小 | 说明 |
|---------|---------|------|
| 自包含 | ~100-150 MB | 包含所有运行时 |
| 单文件 | ~80-120 MB | 压缩后的单文件 |
| MSIX | ~100-150 MB | 包含运行时和资源 |

---

## 分发指南

### 个人分发

**推荐方式：** 自包含发布

1. 发布应用（使用脚本或手动）
2. 压缩 `publish/SelfContained/win-x64` 文件夹为 ZIP
3. 上传到网盘或 GitHub Releases
4. 提供下载链接和简要说明

**示例说明：**
```
Image Resolver v1.0.0

使用方法：
1. 解压 ZIP 文件
2. 运行 ImageResolver.exe

系统要求：
- Windows 10 (1809+) 或 Windows 11
- 无需安装 .NET Runtime
```

### 企业分发

**推荐方式：** MSIX 或自包含发布

- **MSIX**: 适合通过企业应用商店分发
- **自包含**: 适合通过文件服务器或内网分发

### Microsoft Store 发布

**推荐方式：** MSIX

**步骤：**
1. 完成 MSIX 打包
2. 使用代码签名证书签名
3. 在 Partner Center 创建应用
4. 上传 MSIX 包
5. 填写应用信息
6. 提交审核

**审核时间：** 通常 1-3 个工作日

---

## 常见问题

### Q1: 发布后文件太大怎么办？

**A:** 这是正常的，因为包含了：
- .NET 8.0 运行时
- Windows App Runtime
- WinUI 3 框架
- 所有依赖库

**优化建议：**
- 使用单文件发布（有压缩）
- 考虑使用框架依赖发布（需要用户安装 .NET Runtime）

### Q2: 单文件发布后首次启动很慢？

**A:** 这是正常的，因为需要解压文件到临时目录。后续启动会快很多。

### Q3: 杀毒软件误报怎么办？

**A:** 单文件发布可能被误报，因为它是自解压文件。

**解决方案：**
1. 使用代码签名证书签名
2. 提交到杀毒软件厂商白名单
3. 使用自包含发布（多文件）替代

### Q4: 如何更新版本号？

**A:** 修改以下文件中的版本号：

1. **app.manifest**
   ```xml
   <assemblyIdentity version="1.0.0.0" name="ImageResolver.app"/>
   ```

2. **Package.appxmanifest** (MSIX)
   ```xml
   <Identity Version="1.0.0.0" ... />
   ```

3. **发布脚本中的版本变量**

### Q5: 支持哪些 Windows 版本？

**A:** 
- Windows 10 版本 1809 (17763) 或更高版本
- Windows 11

### Q6: 如何发布到其他架构（x86, ARM64）？

**A:** 修改 `-r` 参数：

```powershell
# x86
dotnet publish ... -r win-x86

# ARM64
dotnet publish ... -r win-arm64
```

---

## 发布检查清单

发布前请确认：

- [ ] 所有功能已测试
- [ ] 版本号已更新
- [ ] 应用图标已准备
- [ ] README 文件已更新
- [ ] 在不同 Windows 版本上测试
- [ ] 检查文件大小是否合理
- [ ] 准备应用描述和截图（Store 发布）

---

## 技术支持

如有问题，请：
1. 查看项目 [README.md](README.md)
2. 查看 [GitHub Issues](https://github.com/JiashuaiXu/winUI-image-resolver/issues)
3. 提交新的 Issue

---

## 更新日志

### v1.0.0 (2024-11-07)
- ✅ 初始发布
- ✅ 支持自包含发布
- ✅ 支持单文件发布
- ✅ 支持 MSIX 打包

---

**最后更新：** 2024-11-07

