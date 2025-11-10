# Bug 跟踪文档

本目录包含项目中发现的所有 bug 记录。

## Bug 列表

- [BUG-001: XAML Compilation Error with .NET 10](./001-xaml-compilation-error-net10.md) - XAML 编译器无法解析 WinRT.Runtime
- [BUG-002: ExifLib Compatibility Warning](./002-exiflib-compatibility-warning.md) - ExifLib 包兼容性警告
- [BUG-003: RuntimeIdentifier Not Recognized](./003-runtimeidentifier-not-recognized.md) - RuntimeIdentifier 不被识别（已修复）
- [BUG-004: Windows SDK .NET Ref Required](./004-windows-sdk-net-ref-required.md) - 需要 WindowsSdkPackageVersion（已修复）
- [BUG-005: File Lock Error](./005-file-lock-error.md) - 文件锁定错误（临时问题）
- [BUG-006: XAML Compilation Error with .NET 9](./006-xaml-compilation-error-net9.md) - .NET 9 和 WindowsAppSDK 1.8 的 XAML 编译错误
- [BUG-007: XAML Compilation Fixes](./007-xaml-compilation-fixes.md) - XAML 编译修复方案
- [BUG-008: dotnet Command Wrong Directory](./008-dotnet-command-wrong-directory.md) - 在错误目录下执行 dotnet 命令失败（已解决）
- [兼容性说明](./COMPATIBILITY.md) - 关于 WindowsAppRuntime 版本兼容性的说明

## 命名规范

Bug 文件命名格式：`{编号}-{简短描述}.md`

例如：
- `001-xaml-compilation-error-net10.md`
- `002-exiflib-compatibility-warning.md`

## 状态说明

- **待修复**: Bug 已确认，尚未解决
- **已修复**: Bug 已解决
- **部分修复**: Bug 部分解决，仍有相关问题
- **低优先级**: 警告级别，不影响功能
- **临时问题**: 偶发性问题，已通过清理等方式解决

