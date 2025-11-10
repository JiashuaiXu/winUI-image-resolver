# BUG-008: dotnet 命令在错误目录下执行失败

## 描述

在 `Assets/Assets` 子目录下执行 `dotnet build` 或 `dotnet restore` 时，出现错误：

```
MSBUILD : error MSB1003: Specify a project or solution file. 
The current working directory does not contain a project or solution file.
```

## 错误信息

```
╭─ 13:38:01 |  in  winUI-image-resolver  Assets  Assets
╰─❯ dotnet build 
MSBUILD : error MSB1003: Specify a project or solution file. The current working directory does not contain a project or solution file.

╭─ 13:38:17 |  in  winUI-image-resolver  Assets  Assets
╰─❯ dotnet restore
MSBUILD : error MSB1003: Specify a project or solution file. The current working directory does not contain a project or solution file.
```

## 原因分析

1. **工作目录错误**: 用户在 `Assets/Assets` 子目录下执行命令
2. **项目文件位置**: `ImageResolver.csproj` 位于项目根目录，不在 `Assets/Assets` 目录中
3. **dotnet 命令行为**: `dotnet build` 和 `dotnet restore` 默认在当前目录查找 `.csproj` 或 `.sln` 文件

## 项目结构

```
winUI-image-resolver/
├── ImageResolver.csproj      ← 项目文件在这里
├── App.xaml
├── MainWindow.xaml
├── Assets/
│   ├── Assets/               ← 用户在这里执行命令（错误）
│   ├── generate_icons.py
│   ├── StoreLogo.png
│   └── ...
└── ...
```

## 解决方案

### 方案 1: 返回到项目根目录（推荐）

```powershell
# 从 Assets/Assets 目录返回到项目根目录
cd C:\Users\Administrator\Desktop\winUI-image-resolver

# 然后执行命令
dotnet build
dotnet restore
```

### 方案 2: 指定项目文件路径

```powershell
# 在任意目录下，指定项目文件路径
dotnet build ..\..\ImageResolver.csproj
dotnet restore ..\..\ImageResolver.csproj
```

### 方案 3: 使用绝对路径

```powershell
dotnet build C:\Users\Administrator\Desktop\winUI-image-resolver\ImageResolver.csproj
dotnet restore C:\Users\Administrator\Desktop\winUI-image-resolver\ImageResolver.csproj
```

## 预防措施

1. **检查当前目录**: 执行命令前确认在项目根目录
   ```powershell
   Get-Location
   Test-Path ImageResolver.csproj
   ```

2. **使用项目根目录**: 始终在包含 `.csproj` 文件的目录下执行命令

3. **使用发布脚本**: 使用提供的 `publish.ps1` 脚本，它会自动处理路径问题

## 相关文件

- `ImageResolver.csproj` - 项目文件
- `publish.ps1` - 发布脚本（自动处理路径）

## 状态

✅ **已解决** - 需要用户在正确的目录下执行命令

## 创建日期

2024-11-10

