// rule.mdを読むこと
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using nanobananaWindows.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;

namespace nanobananaWindows.Views.Settings
{
    /// <summary>
    /// スタイル変換 詳細設定ウィンドウ（移動・リサイズ可能）
    /// </summary>
    public sealed partial class StyleTransformSettingsWindow : Window
    {
        private readonly StyleTransformSettingsViewModel _viewModel;
        private TaskCompletionSource<bool>? _taskCompletionSource;
        private bool _isInitialized = false;

        /// <summary>
        /// 適用された設定を取得
        /// </summary>
        public StyleTransformSettingsViewModel? ResultSettings { get; private set; }

        public StyleTransformSettingsWindow(StyleTransformSettingsViewModel? initialSettings = null)
        {
            InitializeComponent();

            // 既存設定がある場合はコピーして使用
            if (initialSettings != null)
            {
                _viewModel = initialSettings.Clone();
            }
            else
            {
                _viewModel = new StyleTransformSettingsViewModel();
            }

            // ウィンドウサイズ設定
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32(600, 700));

            // タイトル設定
            Title = "スタイル変換 詳細設定";

            // 閉じるイベント
            this.Closed += OnWindowClosed;

            // コンボボックスを初期化
            InitializeComboBoxes();

            // UIに反映
            LoadSettingsToUI();

            _isInitialized = true;
        }

        /// <summary>
        /// ダイアログ的に表示して結果を待機
        /// </summary>
        public Task<bool> ShowDialogAsync()
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();
            this.Activate();
            return _taskCompletionSource.Task;
        }

        /// <summary>
        /// コンボボックスの初期化
        /// </summary>
        private void InitializeComboBoxes()
        {
            // ちびキャラスタイル
            ChibiStyleComboBox.ItemsSource = Enum.GetValues<ChibiStyle>()
                .Select(s => new ComboBoxItem { Content = s.GetDisplayName(), Tag = s })
                .ToList();

            // ピクセルスタイル
            PixelStyleComboBox.ItemsSource = Enum.GetValues<PixelStyle>()
                .Select(s => new ComboBoxItem { Content = s.GetDisplayName(), Tag = s })
                .ToList();

            // スプライトサイズ
            SpriteSizeComboBox.ItemsSource = Enum.GetValues<SpriteSize>()
                .Select(s => new ComboBoxItem { Content = s.GetDisplayName(), Tag = s })
                .ToList();
        }

        /// <summary>
        /// ViewModelの設定をUIに反映
        /// </summary>
        private void LoadSettingsToUI()
        {
            SourceImagePathTextBox.Text = _viewModel.SourceImagePath;

            // 変換タイプ
            ChibiModeRadio.IsChecked = _viewModel.TransformType == StyleTransformType.Chibi;
            PixelModeRadio.IsChecked = _viewModel.TransformType == StyleTransformType.Pixel;
            UpdateSectionVisibility(_viewModel.TransformType);

            // ちびキャラ
            SelectComboBoxItem(ChibiStyleComboBox, _viewModel.ChibiStyle);

            // ドットキャラ
            SelectComboBoxItem(PixelStyleComboBox, _viewModel.PixelStyle);
            SelectComboBoxItem(SpriteSizeComboBox, _viewModel.SpriteSize);

            // 共通
            TransparentBackgroundCheckBox.IsChecked = _viewModel.TransparentBackground;
        }

        /// <summary>
        /// 変換タイプに応じてセクションの表示/非表示を切り替え
        /// </summary>
        private void UpdateSectionVisibility(StyleTransformType type)
        {
            ChibiSection.Visibility = type == StyleTransformType.Chibi ? Visibility.Visible : Visibility.Collapsed;
            PixelSection.Visibility = type == StyleTransformType.Pixel ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// コンボボックスの選択状態を設定
        /// </summary>
        private void SelectComboBoxItem<T>(ComboBox comboBox, T value) where T : Enum
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (comboBox.Items[i] is ComboBoxItem item && item.Tag is T tagValue)
                {
                    if (tagValue.Equals(value))
                    {
                        comboBox.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        // ============================================================
        // イベントハンドラ
        // ============================================================

        private void SourceImagePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.SourceImagePath = SourceImagePathTextBox.Text;
        }

        private void TransformTypeRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;
            var type = ChibiModeRadio.IsChecked == true ? StyleTransformType.Chibi : StyleTransformType.Pixel;
            _viewModel.TransformType = type;
            UpdateSectionVisibility(type);
        }

        private void ChibiStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (ChibiStyleComboBox.SelectedItem is ComboBoxItem item && item.Tag is ChibiStyle style)
                _viewModel.ChibiStyle = style;
        }

        private void PixelStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (PixelStyleComboBox.SelectedItem is ComboBoxItem item && item.Tag is PixelStyle style)
                _viewModel.PixelStyle = style;
        }

        private void SpriteSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (SpriteSizeComboBox.SelectedItem is ComboBoxItem item && item.Tag is SpriteSize size)
                _viewModel.SpriteSize = size;
        }

        private void TransparentBackgroundCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.TransparentBackground = TransparentBackgroundCheckBox.IsChecked ?? true;
        }

        /// <summary>
        /// 画像ファイルのドラッグオーバー処理
        /// </summary>
        private void ImagePathTextBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
        }

        /// <summary>
        /// 元画像のドロップ処理
        /// </summary>
        private async void SourceImagePathTextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    var file = items[0] as Windows.Storage.StorageFile;
                    if (file != null && IsImageFile(file.Name))
                    {
                        _viewModel.SourceImagePath = file.Path;
                        SourceImagePathTextBox.Text = file.Path;
                    }
                }
            }
        }

        private async void BrowseSourceButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);

            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".gif");
            picker.FileTypeFilter.Add(".webp");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                _viewModel.SourceImagePath = file.Path;
                SourceImagePathTextBox.Text = file.Path;
            }
        }

        /// <summary>
        /// 画像ファイルかどうかを判定
        /// </summary>
        private static bool IsImageFile(string fileName)
        {
            var ext = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
            return ext is ".png" or ".jpg" or ".jpeg" or ".gif" or ".webp";
        }

        // ============================================================
        // ウィンドウボタン
        // ============================================================

        private async void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            // 元画像は必須
            if (string.IsNullOrWhiteSpace(_viewModel.SourceImagePath))
            {
                var dialog = new ContentDialog
                {
                    Title = "入力エラー",
                    Content = "元画像を入力してください。",
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
                return;
            }

            ResultSettings = _viewModel;
            _taskCompletionSource?.SetResult(true);
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ResultSettings = null;
            _taskCompletionSource?.SetResult(false);
            this.Close();
        }

        private void OnWindowClosed(object sender, WindowEventArgs args)
        {
            _taskCompletionSource?.TrySetResult(false);
        }
    }
}
