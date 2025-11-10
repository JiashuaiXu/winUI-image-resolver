# BUG-003: RuntimeIdentifier Not Recognized in .NET 10

## 描述
在 .NET 10 中，`win10-x86`、`win10-x64`、`win10-arm64` 等 RuntimeIdentifier 不被识别。

## 错误信息
```
error NETSDK1083: The specified RuntimeIdentifier 'win10-x86' is not recognized.
error NETSDK1083: The specified RuntimeIdentifier 'win10-x64' is not recognized.
error NETSDK1083: The specified RuntimeIdentifier 'win10-arm64' is not recognized.
```

## 原因
.NET 10 可能更改了 RuntimeIdentifier 的命名规范，或者这些 RID 已被弃用。

## 解决方案
移除项目文件中的 `<RuntimeIdentifiers>` 配置。对于 WinUI 3 桌面应用，通常不需要显式指定 RID，除非进行发布打包。

## 状态
已修复

## 创建日期
2024-11-07

