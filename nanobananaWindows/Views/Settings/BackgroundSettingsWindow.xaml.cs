// rule.mdを読むこと
using System;
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
    /// 背景生成 詳細設定ウィンドウ（移動・リサイズ可能）
    /// </summary>
    public sealed partial class BackgroundSettingsWindow : Window
    {
        private readonly BackgroundSettingsViewModel _viewModel;
        private TaskCompletionSource<bool>? _taskCompletionSource;
        private bool _isInitialized = false;

        /// <summary>
        /// 適用された設定を取得
        /// </summary>
        public BackgroundSettingsViewModel? ResultSettings { get; private set; }

        public BackgroundSettingsWindow(BackgroundSettingsViewModel? initialSettings = null)
        {
            InitializeComponent();

            // 既存設定がある場合はコピーして使用
            if (initialSettings != null)
            {
                _viewModel = initialSettings.Clone();
            }
            else
            {
                _viewModel = new BackgroundSettingsViewModel();
            }

            // ウィンドウサイズ設定
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32(600, 600));

            // タイトル設定
            Title = "背景生成 詳細設定";

            // 閉じるイベント
            this.Closed += OnWindowClosed;

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
        /// ViewModelの設定をUIに反映
        /// </summary>
        private void LoadSettingsToUI()
        {
            // モード選択
            ReferenceModeRadio.IsChecked = _viewModel.UseReferenceImage;
            DescriptionModeRadio.IsChecked = !_viewModel.UseReferenceImage;
            UpdateModeVisibility(_viewModel.UseReferenceImage);

            // 説明文モード
            DescriptionTextBox.Text = _viewModel.UseReferenceImage ? "" : _viewModel.Description;

            // 参考画像モード
            ReferenceImagePathTextBox.Text = _viewModel.ReferenceImagePath;
            RemoveCharactersCheckBox.IsChecked = _viewModel.RemoveCharacters;
            TransformDescriptionTextBox.Text = _viewModel.UseReferenceImage ? _viewModel.Description : "";
        }

        /// <summary>
        /// モードに応じてセクションの表示/非表示を切り替え
        /// </summary>
        private void UpdateModeVisibility(bool useReference)
        {
            DescriptionSection.Visibility = useReference ? Visibility.Collapsed : Visibility.Visible;
            ReferenceSection.Visibility = useReference ? Visibility.Visible : Visibility.Collapsed;
        }

        // ============================================================
        // イベントハンドラ
        // ============================================================

        private void ModeRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;
            bool useReference = ReferenceModeRadio.IsChecked == true;
            _viewModel.UseReferenceImage = useReference;
            UpdateModeVisibility(useReference);
        }

        private void DescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (!_viewModel.UseReferenceImage)
            {
                _viewModel.Description = DescriptionTextBox.Text;
            }
        }

        private void ReferenceImagePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.ReferenceImagePath = ReferenceImagePathTextBox.Text;
        }

        private void RemoveCharactersCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.RemoveCharacters = RemoveCharactersCheckBox.IsChecked ?? true;
        }

        private void TransformDescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (_viewModel.UseReferenceImage)
            {
                _viewModel.Description = TransformDescriptionTextBox.Text;
            }
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
        /// 参考画像のドロップ処理
        /// </summary>
        private async void ReferenceImagePathTextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    var file = items[0] as Windows.Storage.StorageFile;
                    if (file != null && IsImageFile(file.Name))
                    {
                        _viewModel.ReferenceImagePath = file.Path;
                        ReferenceImagePathTextBox.Text = file.Path;
                    }
                }
            }
        }

        private async void BrowseReferenceButton_Click(object sender, RoutedEventArgs e)
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
                _viewModel.ReferenceImagePath = file.Path;
                ReferenceImagePathTextBox.Text = file.Path;
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
            var errors = new System.Collections.Generic.List<string>();

            if (_viewModel.UseReferenceImage)
            {
                // 参考画像モード
                if (string.IsNullOrWhiteSpace(_viewModel.ReferenceImagePath))
                {
                    errors.Add("参考画像");
                }
            }
            else
            {
                // 説明文モード
                if (string.IsNullOrWhiteSpace(_viewModel.Description))
                {
                    errors.Add("背景の情景説明");
                }
            }

            if (errors.Count > 0)
            {
                var dialog = new ContentDialog
                {
                    Title = "入力エラー",
                    Content = $"以下の必須項目が未入力です：\n\n・{string.Join("\n・", errors)}",
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
