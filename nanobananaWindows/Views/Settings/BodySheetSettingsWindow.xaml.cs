// rule.mdを読むこと
using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using nanobananaWindows.Models;
using nanobananaWindows.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;

namespace nanobananaWindows.Views.Settings
{
    /// <summary>
    /// 素体三面図 詳細設定ウィンドウ（移動・リサイズ可能）
    /// </summary>
    public sealed partial class BodySheetSettingsWindow : Window
    {
        private readonly BodySheetSettingsViewModel _viewModel;
        private TaskCompletionSource<bool>? _taskCompletionSource;
        private bool _isInitialized = false;

        /// <summary>
        /// 適用された設定を取得
        /// </summary>
        public BodySheetSettingsViewModel? ResultSettings { get; private set; }

        public BodySheetSettingsWindow(BodySheetSettingsViewModel? initialSettings = null)
        {
            InitializeComponent();

            // 既存設定がある場合はコピーして使用
            if (initialSettings != null)
            {
                _viewModel = initialSettings.Clone();
            }
            else
            {
                _viewModel = new BodySheetSettingsViewModel();
            }

            // ウィンドウサイズ設定
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32(600, 700));

            // タイトル設定
            Title = "素体三面図 詳細設定";

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
            // 体型プリセット
            BodyTypePresetComboBox.ItemsSource = Enum.GetValues<BodyTypePreset>()
                .Select(p => new ComboBoxItem { Content = p.GetDisplayName(), Tag = p })
                .ToList();

            // バスト特徴
            BustFeatureComboBox.ItemsSource = Enum.GetValues<BustFeature>()
                .Select(f => new ComboBoxItem { Content = f.GetDisplayName(), Tag = f })
                .ToList();

            // 素体表現タイプ
            BodyRenderTypeComboBox.ItemsSource = Enum.GetValues<BodyRenderType>()
                .Select(t => new ComboBoxItem { Content = t.GetDisplayName(), Tag = t })
                .ToList();
        }

        /// <summary>
        /// ViewModelの設定をUIに反映
        /// </summary>
        private void LoadSettingsToUI()
        {
            FaceSheetImagePathTextBox.Text = _viewModel.FaceSheetImagePath;
            AdditionalDescriptionTextBox.Text = _viewModel.AdditionalDescription;

            // コンボボックスの選択状態を設定
            SelectComboBoxItem(BodyTypePresetComboBox, _viewModel.BodyTypePreset);
            SelectComboBoxItem(BustFeatureComboBox, _viewModel.BustFeature);
            SelectComboBoxItem(BodyRenderTypeComboBox, _viewModel.BodyRenderType);
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

        private void FaceSheetImagePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.FaceSheetImagePath = FaceSheetImagePathTextBox.Text;
        }

        private async void BrowseFaceSheetButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();

            // WinUI 3ではウィンドウハンドルを設定する必要がある
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
                _viewModel.FaceSheetImagePath = file.Path;
                FaceSheetImagePathTextBox.Text = file.Path;
            }
        }

        private void BodyTypePresetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (BodyTypePresetComboBox.SelectedItem is ComboBoxItem item && item.Tag is BodyTypePreset preset)
            {
                _viewModel.BodyTypePreset = preset;
            }
        }

        private void BustFeatureComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (BustFeatureComboBox.SelectedItem is ComboBoxItem item && item.Tag is BustFeature feature)
            {
                _viewModel.BustFeature = feature;
            }
        }

        private void BodyRenderTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (BodyRenderTypeComboBox.SelectedItem is ComboBoxItem item && item.Tag is BodyRenderType renderType)
            {
                _viewModel.BodyRenderType = renderType;
            }
        }

        private void AdditionalDescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.AdditionalDescription = AdditionalDescriptionTextBox.Text;
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
        /// 顔三面図画像のドロップ処理
        /// </summary>
        private async void FaceSheetImagePathTextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    var file = items[0] as Windows.Storage.StorageFile;
                    if (file != null && IsImageFile(file.Name))
                    {
                        _viewModel.FaceSheetImagePath = file.Path;
                        FaceSheetImagePathTextBox.Text = file.Path;
                    }
                }
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
            // 必須項目チェック（顔三面図の画像パスのみ必須）
            if (string.IsNullOrWhiteSpace(_viewModel.FaceSheetImagePath))
            {
                var dialog = new ContentDialog
                {
                    Title = "入力エラー",
                    Content = "顔三面図の画像パスを入力してください。",
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
