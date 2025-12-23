// rule.mdを読むこと
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using nanobananaWindows.Models;
using nanobananaWindows.ViewModels;
using nanobananaWindows.Views.Settings;

namespace nanobananaWindows
{
    /// <summary>
    /// メインウィンドウ
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private bool _isInitialized = false;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            InitializeComboBoxes();

            // ウィンドウサイズを設定（WinUI 3ではコードで設定）
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32(1720, 800));

            _isInitialized = true;
        }

        /// <summary>
        /// コンボボックスの初期化
        /// </summary>
        private void InitializeComboBoxes()
        {
            // 出力タイプ
            OutputTypeComboBox.ItemsSource = Enum.GetValues<OutputType>()
                .Select(t => new ComboBoxItem { Content = t.GetDisplayName(), Tag = t })
                .ToList();
            OutputTypeComboBox.SelectedIndex = 0;

            // カラーモード
            ColorModeComboBox.ItemsSource = Enum.GetValues<ColorMode>()
                .Select(m => new ComboBoxItem { Content = m.GetDisplayName(), Tag = m })
                .ToList();
            ColorModeComboBox.SelectedIndex = 0;

            // 出力スタイル
            OutputStyleComboBox.ItemsSource = Enum.GetValues<OutputStyle>()
                .Select(s => new ComboBoxItem { Content = s.GetDisplayName(), Tag = s })
                .ToList();
            OutputStyleComboBox.SelectedIndex = 0;

            // アスペクト比
            AspectRatioComboBox.ItemsSource = Enum.GetValues<AspectRatio>()
                .Select(r => new ComboBoxItem { Content = r.GetDisplayName(), Tag = r })
                .ToList();
            AspectRatioComboBox.SelectedIndex = 0;
        }

        // ============================================================
        // 左カラム: 出力タイプ・基本設定
        // ============================================================

        private void OutputTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;

            if (OutputTypeComboBox.SelectedItem is ComboBoxItem item && item.Tag is OutputType type)
            {
                _viewModel.SelectedOutputType = type;
                UpdateSettingsStatus();
            }
        }

        private async void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // 出力タイプに応じた詳細設定ダイアログを開く
            switch (_viewModel.SelectedOutputType)
            {
                case OutputType.FaceSheet:
                    await OpenFaceSheetSettingsDialogAsync();
                    break;
                case OutputType.BodySheet:
                    await OpenBodySheetSettingsDialogAsync();
                    break;
                case OutputType.Outfit:
                    await OpenOutfitSettingsDialogAsync();
                    break;
                case OutputType.Pose:
                    await OpenPoseSettingsDialogAsync();
                    break;
                case OutputType.SceneBuilder:
                    await OpenSceneBuilderSettingsDialogAsync();
                    break;
                // TODO: 他の出力タイプのダイアログを追加
                default:
                    // 未実装の出力タイプ
                    var dialog = new ContentDialog
                    {
                        Title = "未実装",
                        Content = $"{_viewModel.SelectedOutputType.GetDisplayName()}の詳細設定は準備中です。",
                        CloseButtonText = "OK",
                        XamlRoot = this.Content.XamlRoot
                    };
                    await dialog.ShowAsync();
                    break;
            }
        }

        /// <summary>
        /// 顔三面図の詳細設定ダイアログを開く
        /// </summary>
        private async System.Threading.Tasks.Task OpenFaceSheetSettingsDialogAsync()
        {
            var dialog = new FaceSheetSettingsDialog(this, _viewModel.FaceSheetSettings);
            dialog.XamlRoot = this.Content.XamlRoot;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && dialog.ResultSettings != null)
            {
                // 適用された設定を保存
                _viewModel.FaceSheetSettings = dialog.ResultSettings;
                UpdateSettingsStatus();
            }
        }

        /// <summary>
        /// 素体三面図の詳細設定ダイアログを開く
        /// </summary>
        private async System.Threading.Tasks.Task OpenBodySheetSettingsDialogAsync()
        {
            var dialog = new BodySheetSettingsDialog(this, _viewModel.BodySheetSettings);
            dialog.XamlRoot = this.Content.XamlRoot;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && dialog.ResultSettings != null)
            {
                // 適用された設定を保存
                _viewModel.BodySheetSettings = dialog.ResultSettings;
                UpdateSettingsStatus();
            }
        }

        /// <summary>
        /// 衣装着用の詳細設定ダイアログを開く
        /// </summary>
        private async System.Threading.Tasks.Task OpenOutfitSettingsDialogAsync()
        {
            var dialog = new OutfitSettingsDialog(this, _viewModel.OutfitSettings);
            dialog.XamlRoot = this.Content.XamlRoot;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && dialog.ResultSettings != null)
            {
                // 適用された設定を保存
                _viewModel.OutfitSettings = dialog.ResultSettings;
                UpdateSettingsStatus();
            }
        }

        /// <summary>
        /// ポーズの詳細設定ダイアログを開く
        /// </summary>
        private async System.Threading.Tasks.Task OpenPoseSettingsDialogAsync()
        {
            var dialog = new PoseSettingsDialog(this, _viewModel.PoseSettings);
            dialog.XamlRoot = this.Content.XamlRoot;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && dialog.ResultSettings != null)
            {
                // 適用された設定を保存
                _viewModel.PoseSettings = dialog.ResultSettings;
                UpdateSettingsStatus();
            }
        }

        /// <summary>
        /// シーンビルダーの詳細設定ダイアログを開く
        /// </summary>
        private async System.Threading.Tasks.Task OpenSceneBuilderSettingsDialogAsync()
        {
            var dialog = new SceneBuilderSettingsDialog(this, _viewModel.SceneBuilderSettings);
            dialog.XamlRoot = this.Content.XamlRoot;

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && dialog.ResultSettings != null)
            {
                // 適用された設定を保存
                _viewModel.SceneBuilderSettings = dialog.ResultSettings;
                UpdateSettingsStatus();
            }
        }

        /// <summary>
        /// 設定状態表示を更新
        /// </summary>
        private void UpdateSettingsStatus()
        {
            var status = _viewModel.GetCurrentSettingsStatus();
            SettingsStatusText.Text = $"現在の設定: {_viewModel.SelectedOutputType.GetDisplayName()} ({status})";
        }

        private void ColorModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;

            if (ColorModeComboBox.SelectedItem is ComboBoxItem item && item.Tag is ColorMode mode)
            {
                _viewModel.SelectedColorMode = mode;
            }
        }

        private void OutputStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;

            if (OutputStyleComboBox.SelectedItem is ComboBoxItem item && item.Tag is OutputStyle style)
            {
                _viewModel.SelectedOutputStyle = style;
            }
        }

        private void AspectRatioComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;

            if (AspectRatioComboBox.SelectedItem is ComboBoxItem item && item.Tag is AspectRatio ratio)
            {
                _viewModel.SelectedAspectRatio = ratio;
            }
        }

        private void TitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.Title = TitleTextBox.Text;
        }

        private void IncludeTitleCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.IncludeTitleInImage = IncludeTitleCheckBox.IsChecked ?? false;
        }

        private void AuthorNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.AuthorName = AuthorNameTextBox.Text;
        }

        private void GenerateYamlButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.GenerateYaml();
            YamlPreviewText.Text = _viewModel.YamlPreviewText;
            YamlPreviewText.Foreground = (Microsoft.UI.Xaml.Media.Brush)
                Application.Current.Resources["TextFillColorPrimaryBrush"];
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ResetAll();
            TitleTextBox.Text = "";
            AuthorNameTextBox.Text = "";
            YamlPreviewText.Text = "YAML生成後に表示されます";
            YamlPreviewText.Foreground = (Microsoft.UI.Xaml.Media.Brush)
                Application.Current.Resources["TextFillColorSecondaryBrush"];
        }

        private void MangaComposerButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 漫画ページコンポーザーを開く
        }

        private void ImageToolButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 画像ツールを開く
        }

        // ============================================================
        // 中央カラム: API設定
        // ============================================================

        private void OutputModeRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;

            if (sender is RadioButton radio)
            {
                bool isApiMode = radio == ApiModeRadio;
                _viewModel.SelectedOutputMode = isApiMode ? OutputMode.Api : OutputMode.Yaml;
                UpdateApiControlsState(isApiMode);
            }
        }

        private void UpdateApiControlsState(bool isApiMode)
        {
            // API関連コントロールの有効/無効を切り替え
            ApiKeyBox.IsEnabled = isApiMode;
            ClearApiKeyButton.IsEnabled = isApiMode;

            // APIサブモード
            NormalModeRadio.IsEnabled = isApiMode;
            RedrawModeRadio.IsEnabled = isApiMode;
            SimpleModeRadio.IsEnabled = isApiMode;

            // 参考画像
            ReferenceImagePathBox.IsEnabled = isApiMode;
            BrowseReferenceButton.IsEnabled = isApiMode;

            // 解像度
            Res1KRadio.IsEnabled = isApiMode;
            Res2KRadio.IsEnabled = isApiMode;
            Res4KRadio.IsEnabled = isApiMode;

            // テキストボックス
            RedrawInstructionBox.IsEnabled = isApiMode;
            SimplePromptBox.IsEnabled = isApiMode;

            // 生成ボタン
            GenerateImageButton.IsEnabled = isApiMode && !string.IsNullOrEmpty(_viewModel.ApiKey);
        }

        private void ApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.ApiKey = ApiKeyBox.Password;
            GenerateImageButton.IsEnabled = _viewModel.IsApiGenerateButtonEnabled;
        }

        private void ClearApiKeyButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearApiKey();
            ApiKeyBox.Password = "";
            GenerateImageButton.IsEnabled = false;
        }

        private void ApiSubModeRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;

            if (sender is RadioButton radio)
            {
                if (radio == NormalModeRadio)
                {
                    _viewModel.SelectedApiSubMode = ApiSubMode.Normal;
                    RedrawInstructionPanel.Visibility = Visibility.Collapsed;
                    SimplePromptPanel.Visibility = Visibility.Collapsed;
                }
                else if (radio == RedrawModeRadio)
                {
                    _viewModel.SelectedApiSubMode = ApiSubMode.Redraw;
                    RedrawInstructionPanel.Visibility = Visibility.Visible;
                    SimplePromptPanel.Visibility = Visibility.Collapsed;
                }
                else if (radio == SimpleModeRadio)
                {
                    _viewModel.SelectedApiSubMode = ApiSubMode.Simple;
                    RedrawInstructionPanel.Visibility = Visibility.Collapsed;
                    SimplePromptPanel.Visibility = Visibility.Visible;
                }
            }
        }

        private void BrowseReferenceButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: ファイル選択ダイアログを開く
        }

        private void GenerateImageButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: API経由で画像生成
        }

        // ============================================================
        // 右カラム: YAMLプレビュー・画像プレビュー
        // ============================================================

        private void CopyYamlButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: YAMLをクリップボードにコピー
            if (!string.IsNullOrEmpty(_viewModel.YamlPreviewText))
            {
                var dataPackage = new Windows.ApplicationModel.DataTransfer.DataPackage();
                dataPackage.SetText(_viewModel.YamlPreviewText);
                Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
            }
        }

        private void SaveYamlButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: YAMLをファイルに保存
        }

        private void LoadYamlButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: YAMLをファイルから読込
        }

        private void SaveImageButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 画像を保存
        }

        private void RefineImageButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 画像を加工
        }
    }
}
