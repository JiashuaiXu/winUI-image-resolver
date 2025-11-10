# Image Resolver 快速发布指南

## 🚀 一键发布

```powershell
# 发布所有版本（自包含 + 单文件）
powershell -ExecutionPolicy Bypass -File publish.ps1 -PublishType All
```

## 📦 发布方式对比

| 方式 | 命令 | 文件大小 | 适用场景 |
|------|------|---------|---------|
| **自包含** | `-PublishType SelfContained` | ~100MB | 个人分发、企业分发 |
| **单文件** | `-PublishType SingleFile` | ~80MB | 快速分享、便携使用 |
| **全部** | `-PublishType All` | - | 完整发布 |

## 📁 发布文件位置

```
publish/
├── SelfContained/win-x64/    # 自包含版本（推荐分发）
└── SingleFile/win-x64/        # 单文件版本（便携）
```

## ✅ 发布后检查

1. ✅ 检查 `publish/` 目录是否生成
2. ✅ 测试运行 `ImageResolver.exe`
3. ✅ 检查文件大小是否合理
4. ✅ 准备分发说明

## 📤 分发建议

**个人分发：**
- 压缩 `SelfContained/win-x64` 为 ZIP
- 上传到网盘或 GitHub Releases

**快速分享：**
- 直接分享 `SingleFile/win-x64/ImageResolver.exe`

## 📖 详细文档

查看 [PUBLISH.md](PUBLISH.md) 获取完整发布说明。

---

**版本：** 1.0.0  
**最后更新：** 2024-11-07

