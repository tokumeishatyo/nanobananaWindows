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
    /// インフォグラフィック 詳細設定ウィンドウ（移動・リサイズ可能）
    /// </summary>
    public sealed partial class InfographicSettingsWindow : Window
    {
        private readonly InfographicSettingsViewModel _viewModel;
        private TaskCompletionSource<bool>? _taskCompletionSource;
        private bool _isInitialized = false;

        // セクションUI参照
        private TextBox[] _sectionTitleTextBoxes = new TextBox[8];
        private TextBox[] _sectionContentTextBoxes = new TextBox[8];

        /// <summary>
        /// 適用された設定を取得
        /// </summary>
        public InfographicSettingsViewModel? ResultSettings { get; private set; }

        public InfographicSettingsWindow(InfographicSettingsViewModel? initialSettings = null)
        {
            InitializeComponent();

            // 既存設定がある場合はコピーして使用
            if (initialSettings != null)
            {
                _viewModel = initialSettings.Clone();
            }
            else
            {
                _viewModel = new InfographicSettingsViewModel();
            }

            // ウィンドウサイズ設定
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32(750, 850));

            // タイトル設定
            Title = "インフォグラフィック 詳細設定";

            // 閉じるイベント
            this.Closed += OnWindowClosed;

            // コンボボックスを初期化
            InitializeComboBoxes();

            // セクションUIを動的生成
            CreateSectionUI();

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
            // インフォグラフィックスタイル
            InfographicStyleComboBox.ItemsSource = Enum.GetValues<InfographicStyle>()
                .Select(s => new ComboBoxItem { Content = s.GetDisplayName(), Tag = s })
                .ToList();

            // 出力言語
            OutputLanguageComboBox.ItemsSource = Enum.GetValues<InfographicLanguage>()
                .Select(l => new ComboBoxItem { Content = l.GetDisplayName(), Tag = l })
                .ToList();
        }

        /// <summary>
        /// セクションUIを動的に生成（8セクション）
        /// </summary>
        private void CreateSectionUI()
        {
            SectionsContainer.Children.Clear();

            for (int i = 0; i < 8; i++)
            {
                var sectionBorder = new Border
                {
                    Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardBackgroundFillColorSecondaryBrush"],
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(8),
                    Margin = new Thickness(0, 0, 0, 4)
                };

                var sectionGrid = new Grid();
                sectionGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
                sectionGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                sectionGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                sectionGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                // セクション番号
                var numberText = new TextBlock
                {
                    Text = $"#{i + 1}",
                    FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetRow(numberText, 0);
                Grid.SetColumn(numberText, 0);
                sectionGrid.Children.Add(numberText);

                // タイトル入力
                var titleTextBox = new TextBox
                {
                    PlaceholderText = "タイトル（例：基本プロフィール）",
                    Margin = new Thickness(0, 2, 0, 2)
                };
                int sectionIndex = i;
                titleTextBox.TextChanged += (s, e) => OnSectionTitleChanged(sectionIndex, titleTextBox.Text);
                Grid.SetRow(titleTextBox, 0);
                Grid.SetColumn(titleTextBox, 1);
                sectionGrid.Children.Add(titleTextBox);
                _sectionTitleTextBoxes[i] = titleTextBox;

                // 内容ラベル
                var contentLabel = new TextBlock
                {
                    Text = "内容:",
                    FontSize = 12,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 8, 0, 0)
                };
                Grid.SetRow(contentLabel, 1);
                Grid.SetColumn(contentLabel, 0);
                sectionGrid.Children.Add(contentLabel);

                // 内容入力（複数行）
                var contentTextBox = new TextBox
                {
                    PlaceholderText = "セクションの内容を入力...\n（箇条書き推奨）",
                    AcceptsReturn = true,
                    TextWrapping = TextWrapping.Wrap,
                    MinHeight = 60,
                    Margin = new Thickness(0, 2, 0, 2)
                };
                contentTextBox.TextChanged += (s, e) => OnSectionContentChanged(sectionIndex, contentTextBox.Text);
                Grid.SetRow(contentTextBox, 1);
                Grid.SetColumn(contentTextBox, 1);
                sectionGrid.Children.Add(contentTextBox);
                _sectionContentTextBoxes[i] = contentTextBox;

                sectionBorder.Child = sectionGrid;
                SectionsContainer.Children.Add(sectionBorder);
            }
        }

        /// <summary>
        /// ViewModelの設定をUIに反映
        /// </summary>
        private void LoadSettingsToUI()
        {
            // スタイル・言語
            SelectComboBoxItem(InfographicStyleComboBox, _viewModel.InfographicStyle);
            SelectComboBoxItem(OutputLanguageComboBox, _viewModel.OutputLanguage);
            UpdateCustomLanguageVisibility();
            CustomLanguageTextBox.Text = _viewModel.CustomLanguage;

            // タイトル
            MainTitleTextBox.Text = _viewModel.MainTitle;
            SubtitleTextBox.Text = _viewModel.Subtitle;

            // キャラクター画像
            MainCharacterImagePathTextBox.Text = _viewModel.MainCharacterImagePath;
            SubCharacterImagePathTextBox.Text = _viewModel.SubCharacterImagePath;

            // セクション
            for (int i = 0; i < 8 && i < _viewModel.Sections.Count; i++)
            {
                _sectionTitleTextBoxes[i].Text = _viewModel.Sections[i].Title;
                _sectionContentTextBoxes[i].Text = _viewModel.Sections[i].Content;
            }
        }

        /// <summary>
        /// カスタム言語入力欄の表示/非表示を切り替え
        /// </summary>
        private void UpdateCustomLanguageVisibility()
        {
            var isOther = _viewModel.OutputLanguage == InfographicLanguage.Other;
            CustomLanguageLabel.Visibility = isOther ? Visibility.Visible : Visibility.Collapsed;
            CustomLanguageTextBox.Visibility = isOther ? Visibility.Visible : Visibility.Collapsed;
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

        private void InfographicStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (InfographicStyleComboBox.SelectedItem is ComboBoxItem item && item.Tag is InfographicStyle style)
                _viewModel.InfographicStyle = style;
        }

        private void OutputLanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (OutputLanguageComboBox.SelectedItem is ComboBoxItem item && item.Tag is InfographicLanguage lang)
            {
                _viewModel.OutputLanguage = lang;
                UpdateCustomLanguageVisibility();
            }
        }

        private void CustomLanguageTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.CustomLanguage = CustomLanguageTextBox.Text;
        }

        private void MainTitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.MainTitle = MainTitleTextBox.Text;
        }

        private void SubtitleTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.Subtitle = SubtitleTextBox.Text;
        }

        private void MainCharacterImagePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.MainCharacterImagePath = MainCharacterImagePathTextBox.Text;
        }

        private void SubCharacterImagePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.SubCharacterImagePath = SubCharacterImagePathTextBox.Text;
        }

        private void OnSectionTitleChanged(int index, string title)
        {
            if (!_isInitialized) return;
            if (index >= 0 && index < _viewModel.Sections.Count)
                _viewModel.Sections[index].Title = title;
        }

        private void OnSectionContentChanged(int index, string content)
        {
            if (!_isInitialized) return;
            if (index >= 0 && index < _viewModel.Sections.Count)
                _viewModel.Sections[index].Content = content;
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
        /// メインキャラクター画像のドロップ処理
        /// </summary>
        private async void MainCharacterImagePathTextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    var file = items[0] as Windows.Storage.StorageFile;
                    if (file != null && IsImageFile(file.Name))
                    {
                        _viewModel.MainCharacterImagePath = file.Path;
                        MainCharacterImagePathTextBox.Text = file.Path;
                    }
                }
            }
        }

        /// <summary>
        /// サブキャラクター画像のドロップ処理
        /// </summary>
        private async void SubCharacterImagePathTextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    var file = items[0] as Windows.Storage.StorageFile;
                    if (file != null && IsImageFile(file.Name))
                    {
                        _viewModel.SubCharacterImagePath = file.Path;
                        SubCharacterImagePathTextBox.Text = file.Path;
                    }
                }
            }
        }

        private async void BrowseMainCharacterButton_Click(object sender, RoutedEventArgs e)
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
                _viewModel.MainCharacterImagePath = file.Path;
                MainCharacterImagePathTextBox.Text = file.Path;
            }
        }

        private async void BrowseSubCharacterButton_Click(object sender, RoutedEventArgs e)
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
                _viewModel.SubCharacterImagePath = file.Path;
                SubCharacterImagePathTextBox.Text = file.Path;
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
