# 07. WinUI 3 基础

WinUI 3 是微软最新的 Windows UI 框架，用于构建现代化的 Windows 应用。

## 🎯 什么是 WinUI 3？

WinUI 3 (Windows UI Library 3) 是：
- **原生 Windows UI 框架**: 专为 Windows 10/11 设计
- **现代化设计**: 基于 Fluent Design System
- **高性能**: 原生性能，流畅体验
- **跨平台**: 支持 Windows 10/11 桌面应用

## 🏗️ WinUI 3 架构

```
┌─────────────────────────────┐
│      Your Application       │
│   (Image Resolver)          │
└──────────────┬──────────────┘
               │
┌──────────────▼──────────────┐
│         WinUI 3             │
│   (UI Framework)            │
└──────────────┬──────────────┘
               │
┌──────────────▼──────────────┐
│    Windows App SDK          │
│   (Runtime APIs)            │
└──────────────┬──────────────┘
               │
┌──────────────▼──────────────┐
│      Windows OS             │
└─────────────────────────────┘
```

## 📦 核心概念

### 1. Window（窗口）

**作用**: 应用程序的主窗口

**示例**:
```csharp
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
    }
}
```

### 2. XAML（界面标记）

**作用**: 定义用户界面

**特点**:
- 声明式语法
- 数据绑定支持
- 样式和模板

**示例**:
```xml
<Window x:Class="ImageResolver.MainWindow">
    <Grid>
        <Button Content="Click Me" Click="Button_Click"/>
    </Grid>
</Window>
```

### 3. 控件（Controls）

**常用控件**:
- `Button`: 按钮
- `TextBlock`: 文本显示
- `Image`: 图片显示
- `ListView`: 列表
- `Grid`: 网格布局
- `StackPanel`: 堆叠布局

### 4. 数据绑定（Data Binding）

**作用**: 连接数据和 UI

**示例**:
```xml
<TextBlock Text="{Binding FileName}" />
```

```csharp
public string FileName { get; set; } = "image.jpg";
```

## 🎨 Fluent Design System

WinUI 3 基于 Fluent Design，包含：

### 1. 材质（Materials）

- **Acrylic**: 毛玻璃效果
- **Reveal**: 高光效果

**示例**:
```xml
<Border Background="{ThemeResource SystemControlAcrylicWindowBrush}">
    <!-- 内容 -->
</Border>
```

### 2. 动画（Motion）

- 流畅的过渡动画
- 自然的交互反馈

### 3. 深度（Depth）

- 阴影和层次
- 视觉层次感

## 🔧 项目中的 WinUI 3 使用

### 1. 应用程序入口

```csharp
// App.xaml.cs
public partial class App : Application
{
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow();
        m_window.Activate();
    }
}
```

**说明**:
- `Application` 是 WinUI 3 的应用程序基类
- `OnLaunched` 是应用启动时的入口点

### 2. 窗口定义

```csharp
// MainWindow.xaml.cs
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
    }
}
```

**说明**:
- `Window` 是 WinUI 3 的窗口类
- `InitializeComponent()` 加载 XAML

### 3. 控件使用

```xml
<!-- MainWindow.xaml -->
<Button 
    x:Name="SelectFolderButton"
    Content="Select Folder"
    Click="SelectFolderButton_Click" />
```

**说明**:
- `x:Name`: 控件名称，用于代码访问
- `Content`: 控件内容
- `Click`: 点击事件

### 4. 布局系统

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
    </Grid.RowDefinitions>
    
    <TextBlock Grid.Row="0" Text="Header"/>
    <ScrollViewer Grid.Row="1">
        <!-- 内容 -->
    </ScrollViewer>
</Grid>
```

**说明**:
- `Grid`: 网格布局，最灵活
- `RowDefinitions`: 行定义
- `Height="*"`: 自动填充剩余空间

## 🎯 关键特性

### 1. 主题支持

```xml
<TextBlock 
    Foreground="{ThemeResource SystemControlForegroundBaseHighBrush}" />
```

**说明**:
- 自动适配深色/浅色主题
- 使用系统主题资源

### 2. 响应式布局

```xml
<Grid>
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="2*"/>
        <ColumnDefinition Width="3*"/>
    </Grid.ColumnDefinitions>
</Grid>
```

**说明**:
- 使用 `*` 实现比例布局
- 自动适应窗口大小

### 3. 数据绑定

```xml
<ItemsControl ItemsSource="{x:Bind ImageList}">
    <ItemsControl.ItemTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding FileName}"/>
        </DataTemplate>
    </ItemsControl.ItemTemplate>
</ItemsControl>
```

**说明**:
- `x:Bind`: 编译时绑定，性能更好
- `Binding`: 运行时绑定，更灵活

## 📚 学习资源

- [WinUI 3 官方文档](https://learn.microsoft.com/zh-cn/windows/apps/winui/winui3/)
- [控件库](https://learn.microsoft.com/zh-cn/windows/apps/design/controls/)
- [Fluent Design 指南](https://www.microsoft.com/design/fluent/)

## 🎓 下一步

- 学习 XAML 语法 → [08-XAML语法](./08-XAML语法.md)
- 了解数据绑定 → [10-数据绑定](./10-数据绑定.md)
- 查看 UI 实现 → [11-UI实现详解](./11-UI实现详解.md)

---

**继续学习**: [08-XAML语法](./08-XAML语法.md) →

