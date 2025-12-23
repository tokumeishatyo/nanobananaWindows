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
    /// ポーズ 詳細設定ウィンドウ（移動・リサイズ可能）
    /// </summary>
    public sealed partial class PoseSettingsWindow : Window
    {
        private readonly PoseSettingsViewModel _viewModel;
        private TaskCompletionSource<bool>? _taskCompletionSource;
        private bool _isInitialized = false;

        /// <summary>
        /// 適用された設定を取得
        /// </summary>
        public PoseSettingsViewModel? ResultSettings { get; private set; }

        public PoseSettingsWindow(PoseSettingsViewModel? initialSettings = null)
        {
            InitializeComponent();

            // 既存設定がある場合はコピーして使用
            if (initialSettings != null)
            {
                _viewModel = initialSettings.Clone();
            }
            else
            {
                _viewModel = new PoseSettingsViewModel();
            }

            // ウィンドウサイズ設定
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32(750, 900));

            // タイトル設定
            Title = "ポーズ 詳細設定";

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
            // ポーズプリセット
            PosePresetComboBox.ItemsSource = Enum.GetValues<PosePreset>()
                .Select(p => new ComboBoxItem { Content = p.GetDisplayName(), Tag = p })
                .ToList();

            // 目線
            EyeLineComboBox.ItemsSource = Enum.GetValues<EyeLine>()
                .Select(e => new ComboBoxItem { Content = e.GetDisplayName(), Tag = e })
                .ToList();

            // 表情
            ExpressionComboBox.ItemsSource = Enum.GetValues<PoseExpression>()
                .Select(e => new ComboBoxItem { Content = e.GetDisplayName(), Tag = e })
                .ToList();

            // 風の効果
            WindEffectComboBox.ItemsSource = Enum.GetValues<WindEffect>()
                .Select(w => new ComboBoxItem { Content = w.GetDisplayName(), Tag = w })
                .ToList();
        }

        /// <summary>
        /// ViewModelの設定をUIに反映
        /// </summary>
        private void LoadSettingsToUI()
        {
            // ポーズプリセット
            SelectComboBoxItem(PosePresetComboBox, _viewModel.SelectedPreset);
            UsePoseCaptureCheckBox.IsChecked = _viewModel.UsePoseCapture;
            PoseReferenceImagePathTextBox.Text = _viewModel.PoseReferenceImagePath;
            UpdatePoseCaptureControlsState();

            // 入力画像
            OutfitSheetImagePathTextBox.Text = _viewModel.OutfitSheetImagePath;

            // 向き・表情
            SelectComboBoxItem(EyeLineComboBox, _viewModel.EyeLine);
            SelectComboBoxItem(ExpressionComboBox, _viewModel.Expression);
            ExpressionDetailTextBox.Text = _viewModel.ExpressionDetail;

            // 動作説明
            ActionDescriptionTextBox.Text = _viewModel.ActionDescription;

            // ビジュアル効果
            IncludeEffectsCheckBox.IsChecked = _viewModel.IncludeEffects;
            SelectComboBoxItem(WindEffectComboBox, _viewModel.WindEffect);
            TransparentBackgroundCheckBox.IsChecked = _viewModel.TransparentBackground;
        }

        /// <summary>
        /// ポーズキャプチャ関連コントロールの有効/無効を更新
        /// </summary>
        private void UpdatePoseCaptureControlsState()
        {
            bool usePoseCapture = _viewModel.UsePoseCapture;
            PoseReferenceImagePathTextBox.IsEnabled = usePoseCapture;
            BrowsePoseReferenceButton.IsEnabled = usePoseCapture;
            PosePresetComboBox.IsEnabled = !usePoseCapture;
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
        // イベントハンドラ - ポーズプリセット
        // ============================================================

        private void PosePresetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (PosePresetComboBox.SelectedItem is ComboBoxItem item && item.Tag is PosePreset preset)
            {
                _viewModel.SelectedPreset = preset;
                // プリセット変更時にActionDescriptionとWindEffectを自動更新
                if (preset != PosePreset.None && !_viewModel.UsePoseCapture)
                {
                    ActionDescriptionTextBox.Text = _viewModel.ActionDescription;
                    SelectComboBoxItem(WindEffectComboBox, _viewModel.WindEffect);
                }
            }
        }

        private void UsePoseCaptureCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.UsePoseCapture = UsePoseCaptureCheckBox.IsChecked ?? false;
            UpdatePoseCaptureControlsState();
        }

        private void PoseReferenceImagePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.PoseReferenceImagePath = PoseReferenceImagePathTextBox.Text;
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
        /// ポーズ参考画像のドロップ処理
        /// </summary>
        private async void PoseReferenceImagePathTextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    var file = items[0] as Windows.Storage.StorageFile;
                    if (file != null && IsImageFile(file.Name))
                    {
                        _viewModel.PoseReferenceImagePath = file.Path;
                        PoseReferenceImagePathTextBox.Text = file.Path;
                    }
                }
            }
        }

        // ============================================================
        // イベントハンドラ - 入力画像
        // ============================================================

        private void OutfitSheetImagePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.OutfitSheetImagePath = OutfitSheetImagePathTextBox.Text;
        }

        /// <summary>
        /// 衣装着用三面図のドロップ処理
        /// </summary>
        private async void OutfitSheetImagePathTextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    var file = items[0] as Windows.Storage.StorageFile;
                    if (file != null && IsImageFile(file.Name))
                    {
                        _viewModel.OutfitSheetImagePath = file.Path;
                        OutfitSheetImagePathTextBox.Text = file.Path;
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
        // イベントハンドラ - 向き・表情
        // ============================================================

        private void EyeLineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (EyeLineComboBox.SelectedItem is ComboBoxItem item && item.Tag is EyeLine eyeLine)
            {
                _viewModel.EyeLine = eyeLine;
            }
        }

        private void ExpressionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (ExpressionComboBox.SelectedItem is ComboBoxItem item && item.Tag is PoseExpression expression)
            {
                _viewModel.Expression = expression;
            }
        }

        private void ExpressionDetailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.ExpressionDetail = ExpressionDetailTextBox.Text;
        }

        // ============================================================
        // イベントハンドラ - 動作説明
        // ============================================================

        private void ActionDescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.ActionDescription = ActionDescriptionTextBox.Text;
        }

        // ============================================================
        // イベントハンドラ - ビジュアル効果
        // ============================================================

        private void IncludeEffectsCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.IncludeEffects = IncludeEffectsCheckBox.IsChecked ?? true;
        }

        private void WindEffectComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (WindEffectComboBox.SelectedItem is ComboBoxItem item && item.Tag is WindEffect effect)
            {
                _viewModel.WindEffect = effect;
            }
        }

        private void TransparentBackgroundCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.TransparentBackground = TransparentBackgroundCheckBox.IsChecked ?? false;
        }

        // ============================================================
        // 参照ボタン
        // ============================================================

        private async void BrowsePoseReferenceButton_Click(object sender, RoutedEventArgs e)
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
                _viewModel.PoseReferenceImagePath = file.Path;
                PoseReferenceImagePathTextBox.Text = file.Path;
            }
        }

        private async void BrowseOutfitSheetButton_Click(object sender, RoutedEventArgs e)
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
                _viewModel.OutfitSheetImagePath = file.Path;
                OutfitSheetImagePathTextBox.Text = file.Path;
            }
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
