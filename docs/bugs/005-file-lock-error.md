# BUG-005: File Lock Error During XAML Compilation

## 描述
构建时出现文件锁定错误，XAML 编译器无法访问 input.json 文件。

## 错误信息
```
error : The process cannot access the file 'C:\Users\Administrator\Desktop\winUI-image-resolver\obj\Debug\net10.0-windows10.0.19041.0\input.json' because it is being used by another process.
```

## 原因
可能是之前的构建进程仍在运行，或者文件被其他进程锁定（如 IDE、防病毒软件等）。

## 解决方案
1. 运行 `dotnet clean` 清理构建输出
2. 关闭可能锁定文件的进程（IDE、文件资源管理器等）
3. 等待几秒后重试构建

## 状态
临时问题 - 已通过清理解决

## 创建日期
2024-11-07

