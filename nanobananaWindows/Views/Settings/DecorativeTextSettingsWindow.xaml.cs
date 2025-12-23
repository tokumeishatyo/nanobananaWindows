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
    /// 装飾テキスト 詳細設定ウィンドウ（移動・リサイズ可能）
    /// </summary>
    public sealed partial class DecorativeTextSettingsWindow : Window
    {
        private readonly DecorativeTextSettingsViewModel _viewModel;
        private TaskCompletionSource<bool>? _taskCompletionSource;
        private bool _isInitialized = false;

        /// <summary>
        /// 適用された設定を取得
        /// </summary>
        public DecorativeTextSettingsViewModel? ResultSettings { get; private set; }

        public DecorativeTextSettingsWindow(DecorativeTextSettingsViewModel? initialSettings = null)
        {
            InitializeComponent();

            // 既存設定がある場合はコピーして使用
            if (initialSettings != null)
            {
                _viewModel = initialSettings.Clone();
            }
            else
            {
                _viewModel = new DecorativeTextSettingsViewModel();
            }

            // ウィンドウサイズ設定
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32(700, 800));

            // タイトル設定
            Title = "装飾テキスト 詳細設定";

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
            // 技名テロップ
            TitleFontComboBox.ItemsSource = Enum.GetValues<TitleFont>()
                .Select(f => new ComboBoxItem { Content = f.GetDisplayName(), Tag = f })
                .ToList();
            TitleSizeComboBox.ItemsSource = Enum.GetValues<TitleSize>()
                .Select(s => new ComboBoxItem { Content = s.GetDisplayName(), Tag = s })
                .ToList();
            TitleColorComboBox.ItemsSource = Enum.GetValues<GradientColor>()
                .Select(c => new ComboBoxItem { Content = c.GetDisplayName(), Tag = c })
                .ToList();
            TitleOutlineComboBox.ItemsSource = Enum.GetValues<OutlineColor>()
                .Select(c => new ComboBoxItem { Content = c.GetDisplayName(), Tag = c })
                .ToList();
            TitleGlowComboBox.ItemsSource = Enum.GetValues<GlowEffect>()
                .Select(e => new ComboBoxItem { Content = e.GetDisplayName(), Tag = e })
                .ToList();

            // 決め台詞
            CalloutTypeComboBox.ItemsSource = Enum.GetValues<CalloutType>()
                .Select(t => new ComboBoxItem { Content = t.GetDisplayName(), Tag = t })
                .ToList();
            CalloutColorComboBox.ItemsSource = Enum.GetValues<CalloutColor>()
                .Select(c => new ComboBoxItem { Content = c.GetDisplayName(), Tag = c })
                .ToList();
            CalloutRotationComboBox.ItemsSource = Enum.GetValues<TextRotation>()
                .Select(r => new ComboBoxItem { Content = r.GetDisplayName(), Tag = r })
                .ToList();
            CalloutDistortionComboBox.ItemsSource = Enum.GetValues<TextDistortion>()
                .Select(d => new ComboBoxItem { Content = d.GetDisplayName(), Tag = d })
                .ToList();

            // キャラ名プレート
            NameTagDesignComboBox.ItemsSource = Enum.GetValues<NameTagDesign>()
                .Select(d => new ComboBoxItem { Content = d.GetDisplayName(), Tag = d })
                .ToList();
            NameTagRotationComboBox.ItemsSource = Enum.GetValues<TextRotation>()
                .Select(r => new ComboBoxItem { Content = r.GetDisplayName(), Tag = r })
                .ToList();

            // メッセージウィンドウ
            MessageModeComboBox.ItemsSource = Enum.GetValues<MessageWindowMode>()
                .Select(m => new ComboBoxItem { Content = m.GetDisplayName(), Tag = m })
                .ToList();
            MessageStyleComboBox.ItemsSource = Enum.GetValues<MessageWindowStyle>()
                .Select(s => new ComboBoxItem { Content = s.GetDisplayName(), Tag = s })
                .ToList();
            MessageFrameTypeComboBox.ItemsSource = Enum.GetValues<MessageFrameType>()
                .Select(f => new ComboBoxItem { Content = f.GetDisplayName(), Tag = f })
                .ToList();
            FaceIconPositionComboBox.ItemsSource = Enum.GetValues<FaceIconPosition>()
                .Select(p => new ComboBoxItem { Content = p.GetDisplayName(), Tag = p })
                .ToList();
        }

        /// <summary>
        /// ViewModelの設定をUIに反映
        /// </summary>
        private void LoadSettingsToUI()
        {
            // 共通 - テキストタイプラジオボタン
            switch (_viewModel.TextType)
            {
                case DecorativeTextType.SkillName:
                    SkillNameRadio.IsChecked = true;
                    break;
                case DecorativeTextType.Catchphrase:
                    CatchphraseRadio.IsChecked = true;
                    break;
                case DecorativeTextType.NamePlate:
                    NamePlateRadio.IsChecked = true;
                    break;
                case DecorativeTextType.MessageWindow:
                    MessageWindowRadio.IsChecked = true;
                    break;
            }
            TextContentTextBox.Text = _viewModel.Text;
            TransparentBackgroundCheckBox.IsChecked = _viewModel.TransparentBackground;
            UpdateTextPlaceholder(_viewModel.TextType);
            UpdateSectionVisibility(_viewModel.TextType);

            // 技名テロップ
            SelectComboBoxItem(TitleFontComboBox, _viewModel.TitleFont);
            SelectComboBoxItem(TitleSizeComboBox, _viewModel.TitleSize);
            SelectComboBoxItem(TitleColorComboBox, _viewModel.TitleColor);
            SelectComboBoxItem(TitleOutlineComboBox, _viewModel.TitleOutline);
            SelectComboBoxItem(TitleGlowComboBox, _viewModel.TitleGlow);

            // 決め台詞
            SelectComboBoxItem(CalloutTypeComboBox, _viewModel.CalloutType);
            SelectComboBoxItem(CalloutColorComboBox, _viewModel.CalloutColor);
            SelectComboBoxItem(CalloutRotationComboBox, _viewModel.CalloutRotation);
            SelectComboBoxItem(CalloutDistortionComboBox, _viewModel.CalloutDistortion);

            // キャラ名プレート
            SelectComboBoxItem(NameTagDesignComboBox, _viewModel.NameTagDesign);
            SelectComboBoxItem(NameTagRotationComboBox, _viewModel.NameTagRotation);

            // メッセージウィンドウ
            SelectComboBoxItem(MessageModeComboBox, _viewModel.MessageMode);
            SpeakerNameTextBox.Text = _viewModel.SpeakerName;
            SelectComboBoxItem(MessageStyleComboBox, _viewModel.MessageStyle);
            SelectComboBoxItem(MessageFrameTypeComboBox, _viewModel.MessageFrameType);
            MessageOpacitySlider.Value = _viewModel.MessageOpacity;
            SelectComboBoxItem(FaceIconPositionComboBox, _viewModel.FaceIconPosition);
            FaceIconImagePathTextBox.Text = _viewModel.FaceIconImagePath;
            UpdateFaceIconImagePanelVisibility();
        }

        /// <summary>
        /// テキストタイプに応じたプレースホルダを更新
        /// </summary>
        private void UpdateTextPlaceholder(DecorativeTextType type)
        {
            TextPlaceholderHint.Text = type.GetDescription();
            TextContentTextBox.PlaceholderText = type.GetPlaceholder();
        }

        /// <summary>
        /// テキストタイプに応じてセクションの表示/非表示を切り替え
        /// </summary>
        private void UpdateSectionVisibility(DecorativeTextType type)
        {
            SkillNameSection.Visibility = type == DecorativeTextType.SkillName ? Visibility.Visible : Visibility.Collapsed;
            CatchphraseSection.Visibility = type == DecorativeTextType.Catchphrase ? Visibility.Visible : Visibility.Collapsed;
            NamePlateSection.Visibility = type == DecorativeTextType.NamePlate ? Visibility.Visible : Visibility.Collapsed;
            MessageWindowSection.Visibility = type == DecorativeTextType.MessageWindow ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 顔アイコン画像パネルの表示/非表示を更新
        /// </summary>
        private void UpdateFaceIconImagePanelVisibility()
        {
            FaceIconImagePanel.Visibility = _viewModel.FaceIconPosition != FaceIconPosition.None
                ? Visibility.Visible : Visibility.Collapsed;
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
        // イベントハンドラ - 共通
        // ============================================================

        private void TextTypeRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;

            DecorativeTextType type;
            if (SkillNameRadio.IsChecked == true)
                type = DecorativeTextType.SkillName;
            else if (CatchphraseRadio.IsChecked == true)
                type = DecorativeTextType.Catchphrase;
            else if (NamePlateRadio.IsChecked == true)
                type = DecorativeTextType.NamePlate;
            else
                type = DecorativeTextType.MessageWindow;

            _viewModel.TextType = type;
            UpdateTextPlaceholder(type);
            UpdateSectionVisibility(type);
        }

        private void TextContentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.Text = TextContentTextBox.Text;
        }

        private void TransparentBackgroundCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.TransparentBackground = TransparentBackgroundCheckBox.IsChecked ?? true;
        }

        // ============================================================
        // イベントハンドラ - 技名テロップ
        // ============================================================

        private void TitleFontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (TitleFontComboBox.SelectedItem is ComboBoxItem item && item.Tag is TitleFont font)
                _viewModel.TitleFont = font;
        }

        private void TitleSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (TitleSizeComboBox.SelectedItem is ComboBoxItem item && item.Tag is TitleSize size)
                _viewModel.TitleSize = size;
        }

        private void TitleColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (TitleColorComboBox.SelectedItem is ComboBoxItem item && item.Tag is GradientColor color)
                _viewModel.TitleColor = color;
        }

        private void TitleOutlineComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (TitleOutlineComboBox.SelectedItem is ComboBoxItem item && item.Tag is OutlineColor outline)
                _viewModel.TitleOutline = outline;
        }

        private void TitleGlowComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (TitleGlowComboBox.SelectedItem is ComboBoxItem item && item.Tag is GlowEffect glow)
                _viewModel.TitleGlow = glow;
        }

        // ============================================================
        // イベントハンドラ - 決め台詞
        // ============================================================

        private void CalloutTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (CalloutTypeComboBox.SelectedItem is ComboBoxItem item && item.Tag is CalloutType type)
                _viewModel.CalloutType = type;
        }

        private void CalloutColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (CalloutColorComboBox.SelectedItem is ComboBoxItem item && item.Tag is CalloutColor color)
                _viewModel.CalloutColor = color;
        }

        private void CalloutRotationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (CalloutRotationComboBox.SelectedItem is ComboBoxItem item && item.Tag is TextRotation rotation)
                _viewModel.CalloutRotation = rotation;
        }

        private void CalloutDistortionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (CalloutDistortionComboBox.SelectedItem is ComboBoxItem item && item.Tag is TextDistortion distortion)
                _viewModel.CalloutDistortion = distortion;
        }

        // ============================================================
        // イベントハンドラ - キャラ名プレート
        // ============================================================

        private void NameTagDesignComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (NameTagDesignComboBox.SelectedItem is ComboBoxItem item && item.Tag is NameTagDesign design)
                _viewModel.NameTagDesign = design;
        }

        private void NameTagRotationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (NameTagRotationComboBox.SelectedItem is ComboBoxItem item && item.Tag is TextRotation rotation)
                _viewModel.NameTagRotation = rotation;
        }

        // ============================================================
        // イベントハンドラ - メッセージウィンドウ
        // ============================================================

        private void MessageModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (MessageModeComboBox.SelectedItem is ComboBoxItem item && item.Tag is MessageWindowMode mode)
                _viewModel.MessageMode = mode;
        }

        private void SpeakerNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.SpeakerName = SpeakerNameTextBox.Text;
        }

        private void MessageStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (MessageStyleComboBox.SelectedItem is ComboBoxItem item && item.Tag is MessageWindowStyle style)
                _viewModel.MessageStyle = style;
        }

        private void MessageFrameTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (MessageFrameTypeComboBox.SelectedItem is ComboBoxItem item && item.Tag is MessageFrameType frame)
                _viewModel.MessageFrameType = frame;
        }

        private void MessageOpacitySlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.MessageOpacity = MessageOpacitySlider.Value;
        }

        private void FaceIconPositionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (FaceIconPositionComboBox.SelectedItem is ComboBoxItem item && item.Tag is FaceIconPosition position)
            {
                _viewModel.FaceIconPosition = position;
                UpdateFaceIconImagePanelVisibility();
            }
        }

        private void FaceIconImagePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.FaceIconImagePath = FaceIconImagePathTextBox.Text;
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
        /// 顔アイコン画像のドロップ処理
        /// </summary>
        private async void FaceIconImagePathTextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    var file = items[0] as Windows.Storage.StorageFile;
                    if (file != null && IsImageFile(file.Name))
                    {
                        _viewModel.FaceIconImagePath = file.Path;
                        FaceIconImagePathTextBox.Text = file.Path;
                    }
                }
            }
        }

        private async void BrowseFaceIconButton_Click(object sender, RoutedEventArgs e)
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
                _viewModel.FaceIconImagePath = file.Path;
                FaceIconImagePathTextBox.Text = file.Path;
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
