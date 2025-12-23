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
    /// 顔三面図 詳細設定ウィンドウ（移動・リサイズ可能）
    /// </summary>
    public sealed partial class FaceSheetSettingsWindow : Window
    {
        private readonly FaceSheetSettingsViewModel _viewModel;
        private TaskCompletionSource<bool>? _taskCompletionSource;

        /// <summary>
        /// 適用された設定を取得
        /// </summary>
        public FaceSheetSettingsViewModel? ResultSettings { get; private set; }

        public FaceSheetSettingsWindow(FaceSheetSettingsViewModel? initialSettings = null)
        {
            InitializeComponent();

            // 既存設定がある場合はコピーして使用
            if (initialSettings != null)
            {
                _viewModel = initialSettings.Clone();
            }
            else
            {
                _viewModel = new FaceSheetSettingsViewModel();
            }

            // ウィンドウサイズ設定
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32(650, 500));

            // タイトル設定
            Title = "顔三面図 - 詳細設定";

            // 閉じるイベント
            this.Closed += OnWindowClosed;

            // UIに反映
            LoadSettingsToUI();
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
            CharacterNameTextBox.Text = _viewModel.CharacterName;
            ReferenceImagePathTextBox.Text = _viewModel.ReferenceImagePath;
            AppearanceDescriptionTextBox.Text = _viewModel.AppearanceDescription;
        }

        // ============================================================
        // イベントハンドラ
        // ============================================================

        private void CharacterNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.CharacterName = CharacterNameTextBox.Text;
        }

        private void ReferenceImagePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.ReferenceImagePath = ReferenceImagePathTextBox.Text;
        }

        private void AppearanceDescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _viewModel.AppearanceDescription = AppearanceDescriptionTextBox.Text;
        }

        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
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
                _viewModel.ReferenceImagePath = file.Path;
                ReferenceImagePathTextBox.Text = file.Path;
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
        /// 参照画像のドロップ処理
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

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
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
