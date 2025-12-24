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
    /// 4コマ漫画 詳細設定ウィンドウ（移動・リサイズ可能）
    /// </summary>
    public sealed partial class FourPanelMangaSettingsWindow : Window
    {
        private readonly FourPanelMangaSettingsViewModel _viewModel;
        private TaskCompletionSource<bool>? _taskCompletionSource;
        private bool _isInitialized = false;

        // パネルUI要素への参照
        private TextBox[] _sceneTextBoxes = new TextBox[4];
        private ComboBox[] _speech1CharComboBoxes = new ComboBox[4];
        private TextBox[] _speech1TextBoxes = new TextBox[4];
        private ComboBox[] _speech1PositionComboBoxes = new ComboBox[4];
        private ComboBox[] _speech2CharComboBoxes = new ComboBox[4];
        private TextBox[] _speech2TextBoxes = new TextBox[4];
        private ComboBox[] _speech2PositionComboBoxes = new ComboBox[4];
        private TextBox[] _narrationTextBoxes = new TextBox[4];

        /// <summary>
        /// 適用された設定を取得
        /// </summary>
        public FourPanelMangaSettingsViewModel? ResultSettings { get; private set; }

        public FourPanelMangaSettingsWindow(FourPanelMangaSettingsViewModel? initialSettings = null)
        {
            InitializeComponent();

            // 既存設定がある場合はコピーして使用
            if (initialSettings != null)
            {
                _viewModel = initialSettings.Clone();
            }
            else
            {
                _viewModel = new FourPanelMangaSettingsViewModel();
            }

            // ウィンドウサイズ設定
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32(800, 900));

            // タイトル設定
            Title = "4コマ漫画 詳細設定";

            // 閉じるイベント
            this.Closed += OnWindowClosed;

            // パネルUIを動的に生成
            CreatePanelUI();

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
        /// パネルUIを動的に生成
        /// </summary>
        private void CreatePanelUI()
        {
            string[] panelTitles = { "1コマ目（起）", "2コマ目（承）", "3コマ目（転）", "4コマ目（結）" };

            for (int i = 0; i < 4; i++)
            {
                int panelIndex = i; // クロージャ用

                var border = new Border
                {
                    Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"],
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(12)
                };

                var mainStack = new StackPanel { Spacing = 8 };

                // タイトル
                mainStack.Children.Add(new TextBlock
                {
                    Text = panelTitles[i],
                    FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
                });

                // シーン説明
                mainStack.Children.Add(new TextBlock { Text = "シーン説明:", FontSize = 12 });
                _sceneTextBoxes[i] = new TextBox
                {
                    PlaceholderText = "例：教室で翔子が由衣に話しかける",
                    TextWrapping = TextWrapping.Wrap,
                    AcceptsReturn = true,
                    Height = 50
                };
                _sceneTextBoxes[i].TextChanged += (s, e) =>
                {
                    if (_isInitialized && panelIndex < _viewModel.Panels.Count)
                        _viewModel.Panels[panelIndex].Scene = ((TextBox)s).Text;
                };
                mainStack.Children.Add(_sceneTextBoxes[i]);

                // セリフ1
                var speech1Grid = new Grid();
                speech1Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
                speech1Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                speech1Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
                speech1Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });

                var speech1Label = new TextBlock { Text = "セリフ1:", VerticalAlignment = VerticalAlignment.Center };
                Grid.SetColumn(speech1Label, 0);
                speech1Grid.Children.Add(speech1Label);

                _speech1CharComboBoxes[i] = CreateSpeechCharacterComboBox();
                _speech1CharComboBoxes[i].SelectionChanged += (s, e) =>
                {
                    if (_isInitialized && panelIndex < _viewModel.Panels.Count)
                    {
                        var combo = (ComboBox)s;
                        if (combo.SelectedItem is ComboBoxItem item && item.Tag is SpeechCharacter character)
                            _viewModel.Panels[panelIndex].Speech1Char = character;
                    }
                };
                Grid.SetColumn(_speech1CharComboBoxes[i], 1);
                speech1Grid.Children.Add(_speech1CharComboBoxes[i]);

                var pos1Label = new TextBlock { Text = "位置:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
                Grid.SetColumn(pos1Label, 2);
                speech1Grid.Children.Add(pos1Label);

                _speech1PositionComboBoxes[i] = CreateSpeechPositionComboBox();
                _speech1PositionComboBoxes[i].SelectionChanged += (s, e) =>
                {
                    if (_isInitialized && panelIndex < _viewModel.Panels.Count)
                    {
                        var combo = (ComboBox)s;
                        if (combo.SelectedItem is ComboBoxItem item && item.Tag is SpeechPosition position)
                            _viewModel.Panels[panelIndex].Speech1Position = position;
                    }
                };
                Grid.SetColumn(_speech1PositionComboBoxes[i], 3);
                speech1Grid.Children.Add(_speech1PositionComboBoxes[i]);

                mainStack.Children.Add(speech1Grid);

                _speech1TextBoxes[i] = new TextBox { PlaceholderText = "セリフ内容..." };
                _speech1TextBoxes[i].TextChanged += (s, e) =>
                {
                    if (_isInitialized && panelIndex < _viewModel.Panels.Count)
                        _viewModel.Panels[panelIndex].Speech1Text = ((TextBox)s).Text;
                };
                mainStack.Children.Add(_speech1TextBoxes[i]);

                // セリフ2
                var speech2Grid = new Grid();
                speech2Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
                speech2Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                speech2Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
                speech2Grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });

                var speech2Label = new TextBlock { Text = "セリフ2:", VerticalAlignment = VerticalAlignment.Center };
                Grid.SetColumn(speech2Label, 0);
                speech2Grid.Children.Add(speech2Label);

                _speech2CharComboBoxes[i] = CreateSpeechCharacterComboBox();
                _speech2CharComboBoxes[i].SelectionChanged += (s, e) =>
                {
                    if (_isInitialized && panelIndex < _viewModel.Panels.Count)
                    {
                        var combo = (ComboBox)s;
                        if (combo.SelectedItem is ComboBoxItem item && item.Tag is SpeechCharacter character)
                            _viewModel.Panels[panelIndex].Speech2Char = character;
                    }
                };
                Grid.SetColumn(_speech2CharComboBoxes[i], 1);
                speech2Grid.Children.Add(_speech2CharComboBoxes[i]);

                var pos2Label = new TextBlock { Text = "位置:", VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(8, 0, 0, 0) };
                Grid.SetColumn(pos2Label, 2);
                speech2Grid.Children.Add(pos2Label);

                _speech2PositionComboBoxes[i] = CreateSpeechPositionComboBox();
                _speech2PositionComboBoxes[i].SelectionChanged += (s, e) =>
                {
                    if (_isInitialized && panelIndex < _viewModel.Panels.Count)
                    {
                        var combo = (ComboBox)s;
                        if (combo.SelectedItem is ComboBoxItem item && item.Tag is SpeechPosition position)
                            _viewModel.Panels[panelIndex].Speech2Position = position;
                    }
                };
                Grid.SetColumn(_speech2PositionComboBoxes[i], 3);
                speech2Grid.Children.Add(_speech2PositionComboBoxes[i]);

                mainStack.Children.Add(speech2Grid);

                _speech2TextBoxes[i] = new TextBox { PlaceholderText = "セリフ内容（任意）..." };
                _speech2TextBoxes[i].TextChanged += (s, e) =>
                {
                    if (_isInitialized && panelIndex < _viewModel.Panels.Count)
                        _viewModel.Panels[panelIndex].Speech2Text = ((TextBox)s).Text;
                };
                mainStack.Children.Add(_speech2TextBoxes[i]);

                // ナレーション
                mainStack.Children.Add(new TextBlock { Text = "ナレーション（任意）:", FontSize = 12 });
                _narrationTextBoxes[i] = new TextBox { PlaceholderText = "例：こうして放課後の予定が決まった" };
                _narrationTextBoxes[i].TextChanged += (s, e) =>
                {
                    if (_isInitialized && panelIndex < _viewModel.Panels.Count)
                        _viewModel.Panels[panelIndex].Narration = ((TextBox)s).Text;
                };
                mainStack.Children.Add(_narrationTextBoxes[i]);

                border.Child = mainStack;
                PanelsContainer.Children.Add(border);
            }
        }

        /// <summary>
        /// 話者選択用コンボボックスを作成
        /// </summary>
        private ComboBox CreateSpeechCharacterComboBox()
        {
            var comboBox = new ComboBox { HorizontalAlignment = HorizontalAlignment.Stretch };
            comboBox.ItemsSource = Enum.GetValues<SpeechCharacter>()
                .Select(c => new ComboBoxItem { Content = c.GetDisplayName(), Tag = c })
                .ToList();
            return comboBox;
        }

        /// <summary>
        /// 位置選択用コンボボックスを作成
        /// </summary>
        private ComboBox CreateSpeechPositionComboBox()
        {
            var comboBox = new ComboBox { HorizontalAlignment = HorizontalAlignment.Stretch };
            comboBox.ItemsSource = Enum.GetValues<SpeechPosition>()
                .Select(p => new ComboBoxItem { Content = p.GetDisplayName(), Tag = p })
                .ToList();
            return comboBox;
        }

        /// <summary>
        /// ViewModelの設定をUIに反映
        /// </summary>
        private void LoadSettingsToUI()
        {
            // キャラクター1
            Character1NameTextBox.Text = _viewModel.Character1Name;
            Character1ImagePathTextBox.Text = _viewModel.Character1ImagePath;
            Character1DescriptionTextBox.Text = _viewModel.Character1Description;

            // キャラクター2
            Character2NameTextBox.Text = _viewModel.Character2Name;
            Character2ImagePathTextBox.Text = _viewModel.Character2ImagePath;
            Character2DescriptionTextBox.Text = _viewModel.Character2Description;

            // パネル
            for (int i = 0; i < 4 && i < _viewModel.Panels.Count; i++)
            {
                var panel = _viewModel.Panels[i];
                _sceneTextBoxes[i].Text = panel.Scene;
                SelectComboBoxItem(_speech1CharComboBoxes[i], panel.Speech1Char);
                _speech1TextBoxes[i].Text = panel.Speech1Text;
                SelectComboBoxItem(_speech1PositionComboBoxes[i], panel.Speech1Position);
                SelectComboBoxItem(_speech2CharComboBoxes[i], panel.Speech2Char);
                _speech2TextBoxes[i].Text = panel.Speech2Text;
                SelectComboBoxItem(_speech2PositionComboBoxes[i], panel.Speech2Position);
                _narrationTextBoxes[i].Text = panel.Narration;
            }
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
        // イベントハンドラ - キャラクター
        // ============================================================

        private void Character1NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.Character1Name = Character1NameTextBox.Text;
        }

        private void Character1ImagePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.Character1ImagePath = Character1ImagePathTextBox.Text;
        }

        private void Character1DescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.Character1Description = Character1DescriptionTextBox.Text;
        }

        private void Character2NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.Character2Name = Character2NameTextBox.Text;
        }

        private void Character2ImagePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.Character2ImagePath = Character2ImagePathTextBox.Text;
        }

        private void Character2DescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.Character2Description = Character2DescriptionTextBox.Text;
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

        private async void Character1ImagePathTextBox_Drop(object sender, DragEventArgs e)
        {
            await HandleImageDrop(e, path =>
            {
                _viewModel.Character1ImagePath = path;
                Character1ImagePathTextBox.Text = path;
            });
        }

        private async void Character2ImagePathTextBox_Drop(object sender, DragEventArgs e)
        {
            await HandleImageDrop(e, path =>
            {
                _viewModel.Character2ImagePath = path;
                Character2ImagePathTextBox.Text = path;
            });
        }

        private async Task HandleImageDrop(DragEventArgs e, Action<string> setPath)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    var file = items[0] as Windows.Storage.StorageFile;
                    if (file != null && IsImageFile(file.Name))
                    {
                        setPath(file.Path);
                    }
                }
            }
        }

        private async void BrowseCharacter1Button_Click(object sender, RoutedEventArgs e)
        {
            var path = await BrowseImageFile();
            if (path != null)
            {
                _viewModel.Character1ImagePath = path;
                Character1ImagePathTextBox.Text = path;
            }
        }

        private async void BrowseCharacter2Button_Click(object sender, RoutedEventArgs e)
        {
            var path = await BrowseImageFile();
            if (path != null)
            {
                _viewModel.Character2ImagePath = path;
                Character2ImagePathTextBox.Text = path;
            }
        }

        private async Task<string?> BrowseImageFile()
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
            return file?.Path;
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

            // キャラクター1は必須
            if (string.IsNullOrWhiteSpace(_viewModel.Character1Name))
            {
                errors.Add("キャラクター1の名前");
            }
            if (string.IsNullOrWhiteSpace(_viewModel.Character1ImagePath))
            {
                errors.Add("キャラクター1の画像");
            }
            if (string.IsNullOrWhiteSpace(_viewModel.Character1Description))
            {
                errors.Add("キャラクター1の説明");
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
