# 09. C# 异步编程

异步编程是现代应用开发的重要技能，用于处理耗时操作而不阻塞 UI。

## 🎯 为什么需要异步？

### 同步 vs 异步

**同步（阻塞）**:
```csharp
// 阻塞 UI 线程，界面卡顿
var file = File.ReadAllText("largefile.txt");
```

**异步（非阻塞）**:
```csharp
// 不阻塞 UI，界面保持响应
var file = await File.ReadAllTextAsync("largefile.txt");
```

## 🔑 核心关键字

### async / await

```csharp
// async: 标记异步方法
private async Task LoadDataAsync()
{
    // await: 等待异步操作完成
    var data = await GetDataAsync();
    // 继续执行后续代码
}
```

**规则**:
- `async` 方法必须返回 `Task` 或 `Task<T>`
- `await` 只能在 `async` 方法中使用
- `await` 不会阻塞线程

## 📦 Task 类型

### Task（无返回值）

```csharp
private async Task DoSomethingAsync()
{
    await Task.Delay(1000); // 等待 1 秒
    // 完成
}
```

### Task<T>（有返回值）

```csharp
private async Task<string> GetStringAsync()
{
    await Task.Delay(1000);
    return "Hello";
}

// 使用
var result = await GetStringAsync();
```

## 🎯 项目中的异步使用

### 1. 文件夹选择

```csharp
private async void SelectFolderButton_Click(object sender, RoutedEventArgs e)
{
    var folderPicker = new FolderPicker();
    var folder = await folderPicker.PickSingleFolderAsync();
    
    if (folder != null)
    {
        await LoadImagesAsync(folder.Path);
    }
}
```

**说明**:
- `PickSingleFolderAsync()`: 异步显示文件夹选择对话框
- `await`: 等待用户选择完成
- UI 线程不被阻塞

### 2. 批量处理图片

```csharp
private async Task LoadImagesAsync(string folderPath)
{
    // 显示加载状态
    StatusText.Text = "Loading images...";
    ProgressRing.IsActive = true;
    
    // 异步处理
    var imageInfos = await _imageInfoService.ProcessImagesAsync(folderPath);
    
    // 更新 UI
    foreach (var info in imageInfos)
    {
        _imageInfos.Add(new ImageInfoViewModel(info));
    }
    
    // 隐藏加载状态
    ProgressRing.IsActive = false;
}
```

### 3. 图片处理

```csharp
public async Task<List<ImageInfo>> ProcessImagesAsync(string folderPath)
{
    var imageInfos = new List<ImageInfo>();
    var folder = await StorageFolder.GetFolderFromPathAsync(folderPath);
    var files = await folder.GetFilesAsync();
    
    foreach (var file in files)
    {
        var imageInfo = await ProcessImageAsync(file);
        imageInfos.Add(imageInfo);
    }
    
    return imageInfos;
}
```

## ⚠️ 常见错误

### 1. 死锁（Deadlock）

```csharp
// ❌ 错误：可能导致死锁
var result = SomeAsyncMethod().Result;

// ✅ 正确：使用 await
var result = await SomeAsyncMethod();
```

### 2. async void

```csharp
// ❌ 错误：async void 只用于事件处理
private async void BadMethod() { }

// ✅ 正确：使用 async Task
private async Task GoodMethod() { }

// ✅ 事件处理可以使用 async void
private async void Button_Click(object sender, RoutedEventArgs e) { }
```

### 3. 忘记 await

```csharp
// ❌ 错误：忘记 await，不会等待完成
ProcessImageAsync(file);

// ✅ 正确：使用 await
await ProcessImageAsync(file);
```

## 🎯 最佳实践

### 1. 异常处理

```csharp
try
{
    var result = await SomeAsyncMethod();
}
catch (Exception ex)
{
    // 处理异常
    System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
}
```

### 2. 并发处理

```csharp
// 顺序处理（慢）
foreach (var file in files)
{
    await ProcessFileAsync(file);
}

// 并发处理（快）
var tasks = files.Select(file => ProcessFileAsync(file));
await Task.WhenAll(tasks);
```

### 3. 取消操作

```csharp
private CancellationTokenSource _cancellationTokenSource;

private async Task ProcessAsync()
{
    _cancellationTokenSource = new CancellationTokenSource();
    
    try
    {
        await LongRunningTaskAsync(_cancellationTokenSource.Token);
    }
    catch (OperationCanceledException)
    {
        // 操作被取消
    }
}
```

## 📚 下一步

- 学习数据绑定 → [10-数据绑定](./10-数据绑定.md)
- 查看图片处理 → [12-图片处理](./12-图片处理.md)

---

**继续学习**: [10-数据绑定](./10-数据绑定.md) →

