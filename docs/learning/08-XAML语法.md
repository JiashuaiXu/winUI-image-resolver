# 08. XAML 语法

XAML (eXtensible Application Markup Language) 是用于定义用户界面的标记语言。

## 🎯 什么是 XAML？

XAML 是：
- **声明式语言**: 描述"是什么"而不是"怎么做"
- **XML 基础**: 基于 XML 语法
- **UI 定义**: 用于定义 Windows 应用的界面

## 📝 基本语法

### 1. 元素和属性

```xml
<Button Content="Click Me" />
```

**说明**:
- `<Button>`: 元素（控件）
- `Content`: 属性
- `"Click Me"`: 属性值

### 2. 嵌套元素

```xml
<Grid>
    <Button Content="OK" />
    <TextBlock Text="Hello" />
</Grid>
```

**说明**:
- `Grid` 包含 `Button` 和 `TextBlock`
- 形成层次结构

### 3. 属性语法

```xml
<!-- 方式 1: 属性语法 -->
<Button Content="Click Me" />

<!-- 方式 2: 属性元素语法 -->
<Button>
    <Button.Content>Click Me</Button.Content>
</Button>

<!-- 方式 3: 内容属性（Content 是默认属性） -->
<Button>Click Me</Button>
```

## 🏗️ 布局控件

### Grid（网格）

**最常用的布局控件**

```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto"/>
        <RowDefinition Height="*"/>
        <RowDefinition Height="Auto"/>
    </Grid.RowDefinitions>
    
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*"/>
        <ColumnDefinition Width="*"/>
    </Grid.ColumnDefinitions>
    
    <TextBlock Grid.Row="0" Grid.Column="0" Text="Header"/>
    <TextBlock Grid.Row="1" Grid.Column="1" Text="Content"/>
</Grid>
```

**说明**:
- `RowDefinitions`: 定义行
- `ColumnDefinitions`: 定义列
- `Height="*"`: 自动填充
- `Grid.Row`: 指定行位置

### StackPanel（堆叠）

**垂直或水平排列**

```xml
<StackPanel Orientation="Vertical">
    <Button Content="Button 1"/>
    <Button Content="Button 2"/>
    <Button Content="Button 3"/>
</StackPanel>
```

### ScrollViewer（滚动）

**可滚动内容**

```xml
<ScrollViewer>
    <StackPanel>
        <!-- 很多内容 -->
    </StackPanel>
</ScrollViewer>
```

## 🎨 样式和资源

### 主题资源

```xml
<TextBlock 
    Foreground="{ThemeResource SystemControlForegroundBaseHighBrush}"
    Style="{ThemeResource BodyTextBlockStyle}" />
```

**说明**:
- `{ThemeResource ...}`: 使用系统主题资源
- 自动适配深色/浅色主题

### 本地资源

```xml
<Page.Resources>
    <SolidColorBrush x:Key="MyColor" Color="Blue"/>
</Page.Resources>

<TextBlock Foreground="{StaticResource MyColor}"/>
```

## 🔗 数据绑定

### x:Bind（编译时绑定）

```xml
<TextBlock Text="{x:Bind FileName}" />
```

**特点**:
- 编译时检查
- 性能更好
- 需要代码隐藏类

### Binding（运行时绑定）

```xml
<TextBlock Text="{Binding FileName}" />
```

**特点**:
- 运行时解析
- 更灵活
- 需要 DataContext

### 绑定模式

```xml
<!-- 单向绑定（默认） -->
<TextBlock Text="{x:Bind FileName, Mode=OneWay}" />

<!-- 双向绑定 -->
<TextBox Text="{x:Bind FileName, Mode=TwoWay}" />

<!-- 一次性绑定 -->
<TextBlock Text="{x:Bind FileName, Mode=OneTime}" />
```

## 🎯 事件处理

### 事件语法

```xml
<Button Click="Button_Click" />
```

```csharp
// MainWindow.xaml.cs
private void Button_Click(object sender, RoutedEventArgs e)
{
    // 处理点击事件
}
```

### 事件参数

```csharp
private void Button_Click(object sender, RoutedEventArgs e)
{
    // sender: 触发事件的控件
    Button button = sender as Button;
    
    // e: 事件参数
    // 包含事件相关信息
}
```

## 📦 项目中的 XAML 示例

### MainWindow.xaml 结构

```xml
<Window x:Class="ImageResolver.MainWindow">
    <Grid>
        <!-- 三行布局 -->
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>  <!-- Header -->
            <RowDefinition Height="*"/>     <!-- Content -->
            <RowDefinition Height="Auto"/>   <!-- Footer -->
        </Grid.RowDefinitions>
        
        <!-- Header -->
        <Border Grid.Row="0">
            <Button x:Name="SelectFolderButton" 
                    Click="SelectFolderButton_Click"/>
        </Border>
        
        <!-- Content -->
        <Grid Grid.Row="1">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="2*"/>
                <ColumnDefinition Width="3*"/>
            </Grid.ColumnDefinitions>
            
            <!-- 图片列表 -->
            <ItemsControl x:Name="ImageListControl" 
                          Grid.Column="0">
                <ItemsControl.ItemTemplate>
                    <DataTemplate>
                        <Border PointerPressed="ImageItem_PointerPressed">
                            <TextBlock Text="{Binding FileName}"/>
                        </Border>
                    </DataTemplate>
                </ItemsControl.ItemTemplate>
            </ItemsControl>
            
            <!-- 详情面板 -->
            <ScrollViewer Grid.Column="1">
                <StackPanel x:Name="DetailsPanel"/>
            </ScrollViewer>
        </Grid>
        
        <!-- Footer -->
        <Border Grid.Row="2">
            <TextBlock x:Name="StatusText" Text="Ready"/>
            <Button x:Name="ExportButton" 
                    Click="ExportButton_Click"/>
        </Border>
    </Grid>
</Window>
```

## 🎨 常用属性

### 布局属性

```xml
<Button 
    Margin="10"           <!-- 外边距 -->
    Padding="20,10"       <!-- 内边距 -->
    Width="200"          <!-- 宽度 -->
    Height="50"          <!-- 高度 -->
    HorizontalAlignment="Center"  <!-- 水平对齐 -->
    VerticalAlignment="Center"    <!-- 垂直对齐 -->
/>
```

### 文本属性

```xml
<TextBlock 
    Text="Hello"
    FontSize="16"
    FontWeight="Bold"
    TextWrapping="Wrap"
    TextTrimming="CharacterEllipsis"
/>
```

### 颜色和背景

```xml
<Border 
    Background="LightBlue"
    BorderBrush="Gray"
    BorderThickness="1"
    CornerRadius="8"
/>
```

## 🔧 命名和引用

### x:Name

```xml
<Button x:Name="MyButton" Content="Click" />
```

```csharp
// 在代码中访问
MyButton.Content = "New Text";
```

### x:Class

```xml
<Window x:Class="ImageResolver.MainWindow">
```

**说明**:
- 指定代码隐藏类
- 连接 XAML 和 C# 代码

## 📚 最佳实践

1. **使用 Grid 进行复杂布局**
2. **使用主题资源保持一致性**
3. **合理使用 x:Bind 提高性能**
4. **保持 XAML 结构清晰**
5. **使用命名空间组织代码**

## 🎓 下一步

- 学习数据绑定 → [10-数据绑定](./10-数据绑定.md)
- 查看 UI 实现 → [11-UI实现详解](./11-UI实现详解.md)
- 学习 C# 异步 → [09-CSharp异步编程](./09-CSharp异步编程.md)

---

**继续学习**: [09-CSharp异步编程](./09-CSharp异步编程.md) →

