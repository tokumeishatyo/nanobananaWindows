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
            // 出力タイプに応じた詳細設定ウィンドウを開く
            switch (_viewModel.SelectedOutputType)
            {
                case OutputType.FaceSheet:
                    await OpenFaceSheetSettingsWindowAsync();
                    break;
                case OutputType.BodySheet:
                    await OpenBodySheetSettingsWindowAsync();
                    break;
                case OutputType.Outfit:
                    await OpenOutfitSettingsWindowAsync();
                    break;
                case OutputType.Pose:
                    await OpenPoseSettingsWindowAsync();
                    break;
                case OutputType.SceneBuilder:
                    await OpenSceneBuilderSettingsWindowAsync();
                    break;
                case OutputType.Background:
                    await OpenBackgroundSettingsWindowAsync();
                    break;
                case OutputType.DecorativeText:
                    await OpenDecorativeTextSettingsWindowAsync();
                    break;
                case OutputType.FourPanelManga:
                    await OpenFourPanelMangaSettingsWindowAsync();
                    break;
                case OutputType.StyleTransform:
                    await OpenStyleTransformSettingsWindowAsync();
                    break;
                case OutputType.Infographic:
                    await OpenInfographicSettingsWindowAsync();
                    break;
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
        /// 顔三面図の詳細設定ウィンドウを開く
        /// </summary>
        private async System.Threading.Tasks.Task OpenFaceSheetSettingsWindowAsync()
        {
            var window = new FaceSheetSettingsWindow(_viewModel.FaceSheetSettings);
            var result = await window.ShowDialogAsync();

            if (result && window.ResultSettings != null)
            {
                // 適用された設定を保存
                _viewModel.FaceSheetSettings = window.ResultSettings;
                UpdateSettingsStatus();
            }
        }

        /// <summary>
        /// 素体三面図の詳細設定ウィンドウを開く
        /// </summary>
        private async System.Threading.Tasks.Task OpenBodySheetSettingsWindowAsync()
        {
            var window = new BodySheetSettingsWindow(_viewModel.BodySheetSettings);
            var result = await window.ShowDialogAsync();

            if (result && window.ResultSettings != null)
            {
                // 適用された設定を保存
                _viewModel.BodySheetSettings = window.ResultSettings;
                UpdateSettingsStatus();
            }
        }

        /// <summary>
        /// 衣装着用の詳細設定ウィンドウを開く
        /// </summary>
        private async System.Threading.Tasks.Task OpenOutfitSettingsWindowAsync()
        {
            var window = new OutfitSettingsWindow(_viewModel.OutfitSettings);
            var result = await window.ShowDialogAsync();

            if (result && window.ResultSettings != null)
            {
                // 適用された設定を保存
                _viewModel.OutfitSettings = window.ResultSettings;
                UpdateSettingsStatus();
            }
        }

        /// <summary>
        /// ポーズの詳細設定ウィンドウを開く
        /// </summary>
        private async System.Threading.Tasks.Task OpenPoseSettingsWindowAsync()
        {
            var window = new PoseSettingsWindow(_viewModel.PoseSettings);
            var result = await window.ShowDialogAsync();

            if (result && window.ResultSettings != null)
            {
                // 適用された設定を保存
                _viewModel.PoseSettings = window.ResultSettings;
                UpdateSettingsStatus();
            }
        }

        /// <summary>
        /// シーンビルダーの詳細設定ウィンドウを開く
        /// </summary>
        private async System.Threading.Tasks.Task OpenSceneBuilderSettingsWindowAsync()
        {
            var window = new SceneBuilderSettingsWindow(_viewModel.SceneBuilderSettings);
            var result = await window.ShowDialogAsync();

            if (result && window.ResultSettings != null)
            {
                // 適用された設定を保存
                _viewModel.SceneBuilderSettings = window.ResultSettings;
                UpdateSettingsStatus();
            }
        }

        /// <summary>
        /// 背景生成の詳細設定ウィンドウを開く
        /// </summary>
        private async System.Threading.Tasks.Task OpenBackgroundSettingsWindowAsync()
        {
            var window = new BackgroundSettingsWindow(_viewModel.BackgroundSettings);
            var result = await window.ShowDialogAsync();

            if (result && window.ResultSettings != null)
            {
                _viewModel.BackgroundSettings = window.ResultSettings;
                UpdateSettingsStatus();
            }
        }

        /// <summary>
        /// 装飾テキストの詳細設定ウィンドウを開く
        /// </summary>
        private async System.Threading.Tasks.Task OpenDecorativeTextSettingsWindowAsync()
        {
            var window = new DecorativeTextSettingsWindow(_viewModel.DecorativeTextSettings);
            var result = await window.ShowDialogAsync();

            if (result && window.ResultSettings != null)
            {
                _viewModel.DecorativeTextSettings = window.ResultSettings;
                UpdateSettingsStatus();
            }
        }

        /// <summary>
        /// 4コマ漫画の詳細設定ウィンドウを開く
        /// </summary>
        private async System.Threading.Tasks.Task OpenFourPanelMangaSettingsWindowAsync()
        {
            var window = new FourPanelMangaSettingsWindow(_viewModel.FourPanelMangaSettings);
            var result = await window.ShowDialogAsync();

            if (result && window.ResultSettings != null)
            {
                _viewModel.FourPanelMangaSettings = window.ResultSettings;
                UpdateSettingsStatus();
            }
        }

        /// <summary>
        /// スタイル変換の詳細設定ウィンドウを開く
        /// </summary>
        private async System.Threading.Tasks.Task OpenStyleTransformSettingsWindowAsync()
        {
            var window = new StyleTransformSettingsWindow(_viewModel.StyleTransformSettings);
            var result = await window.ShowDialogAsync();

            if (result && window.ResultSettings != null)
            {
                _viewModel.StyleTransformSettings = window.ResultSettings;
                UpdateSettingsStatus();
            }
        }

        /// <summary>
        /// インフォグラフィックの詳細設定ウィンドウを開く
        /// </summary>
        private async System.Threading.Tasks.Task OpenInfographicSettingsWindowAsync()
        {
            var window = new InfographicSettingsWindow(_viewModel.InfographicSettings);
            var result = await window.ShowDialogAsync();

            if (result && window.ResultSettings != null)
            {
                _viewModel.InfographicSettings = window.ResultSettings;
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

        private async void GenerateYamlButton_Click(object sender, RoutedEventArgs e)
        {
            var errorMessage = _viewModel.GenerateYaml();

            if (errorMessage != null)
            {
                // バリデーションエラーをダイアログで表示
                var dialog = new ContentDialog
                {
                    Title = "入力エラー",
                    Content = errorMessage,
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await dialog.ShowAsync();
                return;
            }

            // 成功時はYAMLプレビューを更新
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
