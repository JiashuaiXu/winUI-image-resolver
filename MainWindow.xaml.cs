using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using ImageResolver.Models;
using ImageResolver.Services;

namespace ImageResolver
{
    public sealed partial class MainWindow : Window
    {
        private readonly ImageInfoService _imageInfoService;
        private readonly CsvExportService _csvExportService;
        private ObservableCollection<ImageInfoViewModel> _imageInfos;
        private ImageInfoViewModel? _selectedImage;
        private string? _currentFolderPath;

        public MainWindow()
        {
            this.InitializeComponent();
            _imageInfoService = new ImageInfoService();
            _csvExportService = new CsvExportService();
            _imageInfos = new ObservableCollection<ImageInfoViewModel>();
            ImageListControl.ItemsSource = _imageInfos;
        }

        private async void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");
            
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                _currentFolderPath = folder.Path;
                await LoadImagesAsync(folder.Path);
            }
        }

        private async Task LoadImagesAsync(string folderPath)
        {
            try
            {
                StatusText.Text = "Loading images...";
                ProgressRing.IsActive = true;
                ProgressRing.Visibility = Visibility.Visible;
                SelectFolderButton.IsEnabled = false;
                ExportButton.IsEnabled = false;

                _imageInfos.Clear();
                DetailsPanel.Children.Clear();
                _previousSelectedBorder = null;
                _selectedImage = null;

                var imageInfos = await _imageInfoService.ProcessImagesAsync(folderPath);

                foreach (var info in imageInfos)
                {
                    var viewModel = new ImageInfoViewModel(info);
                    _imageInfos.Add(viewModel);
                }

                // Load thumbnails asynchronously
                _ = LoadThumbnailsAsync();

                StatusText.Text = $"Loaded {_imageInfos.Count} image(s)";
                ExportButton.IsEnabled = _imageInfos.Count > 0;
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
            }
            finally
            {
                ProgressRing.IsActive = false;
                ProgressRing.Visibility = Visibility.Collapsed;
                SelectFolderButton.IsEnabled = true;
            }
        }

        private Border? _previousSelectedBorder;

        private void ImageItem_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Border border && border.DataContext is ImageInfoViewModel viewModel)
            {
                // 恢复之前选中项的样式
                if (_previousSelectedBorder != null && _previousSelectedBorder != border)
                {
                    ResetBorderStyle(_previousSelectedBorder);
                }
                
                // 清除之前选中项的状态
                if (_selectedImage != null && _selectedImage != viewModel)
                {
                    _selectedImage.IsSelected = false;
                }
                
                // 设置新的选中项
                _selectedImage = viewModel;
                viewModel.IsSelected = true;
                _previousSelectedBorder = border;
                
                // 更新当前项的样式为选中状态
                ApplySelectedStyle(border);
                
                UpdateDetailsPanel(viewModel);
            }
        }

        private void ApplySelectedStyle(Border border)
        {
            // 使用系统高亮色，自动适配深色/浅色主题
            var accentBrush = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["SystemControlHighlightAccentBrush"];
            var accentLowBrush = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["SystemControlHighlightListAccentLowBrush"];
            
            border.Background = accentLowBrush ?? new Microsoft.UI.Xaml.Media.SolidColorBrush(
                Microsoft.UI.ColorHelper.FromArgb(255, 0, 120, 215));
            border.BorderBrush = accentBrush ?? new Microsoft.UI.Xaml.Media.SolidColorBrush(
                Microsoft.UI.ColorHelper.FromArgb(255, 0, 99, 177));
            border.BorderThickness = new Thickness(2);
        }

        private void ResetBorderStyle(Border border)
        {
            // 恢复默认样式
            border.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"];
            border.BorderBrush = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"];
            border.BorderThickness = new Thickness(1);
        }

        private void UpdateDetailsPanel(ImageInfoViewModel viewModel)
        {
            DetailsPanel.Children.Clear();

            var info = viewModel.ImageInfo;

            // Basic Information Card
            AddDetailCard("Basic Information", new Dictionary<string, string>
            {
                { "File Name", info.FileName },
                { "File Path", info.FilePath },
                { "Resolution", info.Resolution },
                { "Width", $"{info.Width} px" },
                { "Height", $"{info.Height} px" },
                { "File Size", info.FileSizeFormatted }
            });

            // Camera Information Card
            if (!string.IsNullOrEmpty(info.CameraMake) || !string.IsNullOrEmpty(info.CameraModel))
            {
                AddDetailCard("Camera Information", new Dictionary<string, string>
                {
                    { "Make", info.CameraMake ?? "N/A" },
                    { "Model", info.CameraModel ?? "N/A" },
                    { "Date Taken", info.DateTaken?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A" },
                    { "Orientation", info.Orientation ?? "N/A" }
                });
            }

            // Camera Settings Card
            if (info.ISO.HasValue || info.FNumber.HasValue || info.ExposureTime.HasValue || info.FocalLength.HasValue)
            {
                AddDetailCard("Camera Settings", new Dictionary<string, string>
                {
                    { "ISO", info.ISO?.ToString() ?? "N/A" },
                    { "F-Number", info.FNumber?.ToString("F2") ?? "N/A" },
                    { "Exposure Time", info.ExposureTime.HasValue ? $"{info.ExposureTime.Value:F4} s" : "N/A" },
                    { "Focal Length", info.FocalLength?.ToString("F1") + " mm" ?? "N/A" }
                });
            }

            // Location Information Card
            if (info.Latitude.HasValue && info.Longitude.HasValue)
            {
                AddDetailCard("Location Information", new Dictionary<string, string>
                {
                    { "Latitude", info.Latitude.Value.ToString("F6") },
                    { "Longitude", info.Longitude.Value.ToString("F6") }
                });
            }

            // Metadata Card
            if (!string.IsNullOrEmpty(info.Software) || !string.IsNullOrEmpty(info.Artist) || !string.IsNullOrEmpty(info.Copyright))
            {
                AddDetailCard("Metadata", new Dictionary<string, string>
                {
                    { "Software", info.Software ?? "N/A" },
                    { "Artist", info.Artist ?? "N/A" },
                    { "Copyright", info.Copyright ?? "N/A" }
                });
            }
        }

        private void AddDetailCard(string title, Dictionary<string, string> properties)
        {
            var card = new Border
            {
                Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.Colors.Transparent),
                BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.ColorHelper.FromArgb(255, 200, 200, 200)),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(16),
                Margin = new Thickness(0, 0, 0, 16)
            };

            var stackPanel = new StackPanel();

            var titleBlock = new TextBlock
            {
                Text = title,
                Style = (Style)Application.Current.Resources["SubtitleTextBlockStyle"],
                FontSize = 16,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 8)
            };
            stackPanel.Children.Add(titleBlock);

            foreach (var prop in properties)
            {
                var grid = new Grid
                {
                    Margin = new Thickness(0, 0, 0, 12)
                };
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });

                var keyBlock = new TextBlock
                {
                    Text = prop.Key + ":",
                    Style = (Style)Application.Current.Resources["CaptionTextBlockStyle"],
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                        Microsoft.UI.ColorHelper.FromArgb(255, 120, 120, 120))
                };
                Grid.SetColumn(keyBlock, 0);
                grid.Children.Add(keyBlock);

                var valueBlock = new TextBlock
                {
                    Text = prop.Value,
                    Style = (Style)Application.Current.Resources["BodyTextBlockStyle"],
                    TextWrapping = TextWrapping.Wrap
                };
                Grid.SetColumn(valueBlock, 1);
                grid.Children.Add(valueBlock);

                stackPanel.Children.Add(grid);
            }

            card.Child = stackPanel;
            DetailsPanel.Children.Add(card);
        }

        private async void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            if (_imageInfos.Count == 0 || string.IsNullOrEmpty(_currentFolderPath))
                return;

            var filePicker = new FileSavePicker();
            filePicker.SuggestedFileName = "image_info_export";
            filePicker.FileTypeChoices.Add("CSV Files", new List<string> { ".csv" });

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);

            var file = await filePicker.PickSaveFileAsync();
            if (file != null)
            {
                try
                {
                    StatusText.Text = "Exporting to CSV...";
                    ProgressRing.IsActive = true;
                    ProgressRing.Visibility = Visibility.Visible;
                    ExportButton.IsEnabled = false;

                    var imageInfos = _imageInfos.Select(vm => vm.ImageInfo).ToList();
                    var outputPath = await _csvExportService.ExportToCsvAsync(imageInfos, file.Path);

                    StatusText.Text = $"Exported successfully to {file.Name}";
                }
                catch (Exception ex)
                {
                    StatusText.Text = $"Export error: {ex.Message}";
                }
                finally
                {
                    ProgressRing.IsActive = false;
                    ProgressRing.Visibility = Visibility.Collapsed;
                    ExportButton.IsEnabled = true;
                }
            }
        }

        private async Task LoadThumbnailsAsync()
        {
            foreach (var viewModel in _imageInfos)
            {
                try
                {
                    var file = await StorageFile.GetFileFromPathAsync(viewModel.ImageInfo.FilePath);
                    var bitmap = new BitmapImage();
                    await bitmap.SetSourceAsync(await file.OpenReadAsync());
                    viewModel.Thumbnail = bitmap;
                }
                catch
                {
                    // Ignore thumbnail loading errors
                }
            }
        }
    }

    public class ImageInfoViewModel : System.ComponentModel.INotifyPropertyChanged
    {
        private BitmapImage? _thumbnail;
        private bool _isSelected;

        public ImageInfo ImageInfo { get; }
        public string FileName => ImageInfo.FileName;
        public string Resolution => ImageInfo.Resolution;
        public string FileSizeFormatted => ImageInfo.FileSizeFormatted;

        public BitmapImage? Thumbnail
        {
            get => _thumbnail;
            set
            {
                _thumbnail = value;
                OnPropertyChanged(nameof(Thumbnail));
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public ImageInfoViewModel(ImageInfo imageInfo)
        {
            ImageInfo = imageInfo;
        }

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}

