// rule.mdを読むこと
using System;
using System.Collections.Generic;
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
    /// シーンビルダー 詳細設定ウィンドウ（移動・リサイズ可能）
    /// </summary>
    public sealed partial class SceneBuilderSettingsWindow : Window
    {
        private const int MaxTextOverlayItems = 10;
        private readonly SceneBuilderSettingsViewModel _viewModel;
        private TaskCompletionSource<bool>? _taskCompletionSource;
        private bool _isInitialized = false;

        // 動的生成したUI要素の参照
        private readonly List<Border> _characterRows = new();
        private readonly List<TextBox> _dialogueBoxes = new();
        private readonly List<Border> _textOverlayRows = new();

        /// <summary>
        /// 適用された設定を取得
        /// </summary>
        public SceneBuilderSettingsViewModel? ResultSettings { get; private set; }

        public SceneBuilderSettingsWindow(SceneBuilderSettingsViewModel? initialSettings = null)
        {
            InitializeComponent();

            // 既存設定がある場合はコピーして使用
            if (initialSettings != null)
            {
                _viewModel = initialSettings.Clone();
            }
            else
            {
                _viewModel = new SceneBuilderSettingsViewModel();
            }

            // ウィンドウサイズ設定
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32(950, 800));

            // タイトル設定
            Title = "シーンビルダー 詳細設定";

            // 閉じるイベント
            this.Closed += OnWindowClosed;

            // コンボボックスを初期化
            InitializeComboBoxes();

            // UIに反映
            LoadSettingsToUI();

            // キャラクター・ダイアログUIを構築
            RebuildCharacterUI();
            RebuildDialogueUI();
            RebuildTextOverlayUI();

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
            // 雰囲気
            LightingMoodComboBox.ItemsSource = Enum.GetValues<LightingMood>()
                .Select(m => new ComboBoxItem { Content = m.GetDisplayName(), Tag = m })
                .ToList();

            // 配置パターン
            StoryLayoutComboBox.ItemsSource = Enum.GetValues<StoryLayout>()
                .Select(l => new ComboBoxItem { Content = l.GetDisplayName(), Tag = l })
                .ToList();

            // 距離感
            StoryDistanceComboBox.ItemsSource = Enum.GetValues<StoryDistance>()
                .Select(d => new ComboBoxItem { Content = d.GetDisplayName(), Tag = d })
                .ToList();

            // キャラクター人数
            CharacterCountComboBox.ItemsSource = Enum.GetValues<CharacterCount>()
                .Select(c => new ComboBoxItem { Content = c.GetDisplayName(), Tag = c })
                .ToList();

            // ナレーション位置
            NarrationPositionComboBox.ItemsSource = Enum.GetValues<NarrationPosition>()
                .Select(p => new ComboBoxItem { Content = p.GetDisplayName(), Tag = p })
                .ToList();
        }

        /// <summary>
        /// ViewModelの設定をUIに反映
        /// </summary>
        private void LoadSettingsToUI()
        {
            // シーンタイプ
            switch (_viewModel.SceneType)
            {
                case SceneType.Story:
                    StorySceneRadio.IsChecked = true;
                    break;
                case SceneType.Battle:
                    BattleSceneRadio.IsChecked = true;
                    break;
                case SceneType.BossRaid:
                    BossRaidRadio.IsChecked = true;
                    break;
            }

            // 背景ソースタイプ
            if (_viewModel.BackgroundSourceType == BackgroundSourceType.File)
            {
                BackgroundFileRadio.IsChecked = true;
                BackgroundFilePanel.Visibility = Visibility.Visible;
                BackgroundPromptPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                BackgroundPromptRadio.IsChecked = true;
                BackgroundFilePanel.Visibility = Visibility.Collapsed;
                BackgroundPromptPanel.Visibility = Visibility.Visible;
            }
            BackgroundImagePathTextBox.Text = _viewModel.BackgroundImagePath;
            BackgroundDescriptionTextBox.Text = _viewModel.BackgroundDescription;

            // ストーリー背景オプション
            BlurAmountSlider.Value = _viewModel.StoryBlurAmount;
            BlurAmountText.Text = ((int)_viewModel.StoryBlurAmount).ToString();
            SelectComboBoxItem(LightingMoodComboBox, _viewModel.StoryLightingMood);
            CustomMoodTextBox.Text = _viewModel.StoryCustomMood;
            UpdateCustomMoodVisibility();

            // 配置設定
            SelectComboBoxItem(StoryLayoutComboBox, _viewModel.StoryLayout);
            SelectComboBoxItem(StoryDistanceComboBox, _viewModel.StoryDistance);
            CustomLayoutTextBox.Text = _viewModel.StoryCustomLayout;
            UpdateCustomLayoutVisibility();

            // キャラクター人数
            SelectComboBoxItem(CharacterCountComboBox, _viewModel.StoryCharacterCount);

            // ダイアログ設定
            SelectComboBoxItem(NarrationPositionComboBox, _viewModel.StoryNarrationPosition);
            NarrationTextBox.Text = _viewModel.StoryNarration;
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

        /// <summary>
        /// キャラクターUIを再構築
        /// </summary>
        private void RebuildCharacterUI()
        {
            // 既存の行をクリア
            foreach (var row in _characterRows)
            {
                CharactersPanel.Children.Remove(row);
            }
            _characterRows.Clear();

            // 人数分の行を作成
            int count = _viewModel.StoryCharacterCount.GetIntValue();
            for (int i = 0; i < count; i++)
            {
                var row = CreateCharacterRow(i);
                _characterRows.Add(row);
                CharactersPanel.Children.Add(row);
            }
        }

        /// <summary>
        /// キャラクター行を作成
        /// </summary>
        private Border CreateCharacterRow(int index)
        {
            var character = _viewModel.StoryCharacters[index];

            var border = new Border
            {
                Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["SubtleFillColorSecondaryBrush"],
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8),
                Margin = new Thickness(0, 4, 0, 0)
            };

            var stack = new StackPanel { Spacing = 4 };

            // ヘッダー
            stack.Children.Add(new TextBlock
            {
                Text = $"キャラクター {index + 1}",
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            });

            // 画像パス
            var imageStack = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
            imageStack.Children.Add(new TextBlock { Text = "画像:", VerticalAlignment = VerticalAlignment.Center, Width = 50 });
            var imagePathBox = new TextBox
            {
                Text = character.ImagePath,
                PlaceholderText = "画像をドロップ、またはパスを入力...",
                Width = 350,
                AllowDrop = true
            };
            int capturedIndex = index;
            imagePathBox.TextChanged += (s, e) => _viewModel.StoryCharacters[capturedIndex].ImagePath = imagePathBox.Text;
            imagePathBox.DragOver += ImagePathTextBox_DragOver;
            imagePathBox.Drop += async (s, e) =>
            {
                if (e.DataView.Contains(StandardDataFormats.StorageItems))
                {
                    var items = await e.DataView.GetStorageItemsAsync();
                    if (items.Count > 0)
                    {
                        var file = items[0] as Windows.Storage.StorageFile;
                        if (file != null && IsImageFile(file.Name))
                        {
                            _viewModel.StoryCharacters[capturedIndex].ImagePath = file.Path;
                            imagePathBox.Text = file.Path;
                        }
                    }
                }
            };
            imageStack.Children.Add(imagePathBox);
            var browseButton = new Button { Content = "参照...", Margin = new Thickness(4, 0, 0, 0) };
            browseButton.Click += async (s, e) =>
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
                    _viewModel.StoryCharacters[capturedIndex].ImagePath = file.Path;
                    imagePathBox.Text = file.Path;
                }
            };
            imageStack.Children.Add(browseButton);
            stack.Children.Add(imageStack);

            // 表情・特徴
            var detailStack = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 16 };

            var exprStack = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
            exprStack.Children.Add(new TextBlock { Text = "表情:", VerticalAlignment = VerticalAlignment.Center, Width = 50 });
            var exprBox = new TextBox
            {
                Text = character.Expression,
                PlaceholderText = "笑顔、怒り...",
                Width = 150
            };
            exprBox.TextChanged += (s, e) => _viewModel.StoryCharacters[capturedIndex].Expression = exprBox.Text;
            exprStack.Children.Add(exprBox);
            detailStack.Children.Add(exprStack);

            var traitsStack = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8 };
            traitsStack.Children.Add(new TextBlock { Text = "特徴:", VerticalAlignment = VerticalAlignment.Center });
            var traitsBox = new TextBox
            {
                Text = character.Traits,
                PlaceholderText = "髪色、服装など...",
                Width = 200
            };
            traitsBox.TextChanged += (s, e) => _viewModel.StoryCharacters[capturedIndex].Traits = traitsBox.Text;
            traitsStack.Children.Add(traitsBox);
            detailStack.Children.Add(traitsStack);

            stack.Children.Add(detailStack);

            border.Child = stack;
            return border;
        }

        /// <summary>
        /// ダイアログUIを再構築
        /// </summary>
        private void RebuildDialogueUI()
        {
            // 既存のセリフボックスをクリア
            foreach (var box in _dialogueBoxes)
            {
                DialoguesPanel.Children.Remove(box);
            }
            _dialogueBoxes.Clear();

            // 人数分のセリフ入力を作成
            int count = _viewModel.StoryCharacterCount.GetIntValue();
            for (int i = 0; i < count; i++)
            {
                var box = CreateDialogueBox(i);
                _dialogueBoxes.Add(box);
                DialoguesPanel.Children.Add(box);
            }
        }

        /// <summary>
        /// セリフ入力ボックスを作成
        /// </summary>
        private TextBox CreateDialogueBox(int index)
        {
            var box = new TextBox
            {
                Text = _viewModel.StoryDialogues[index],
                PlaceholderText = $"キャラクター {index + 1} のセリフ",
                Margin = new Thickness(0, 2, 0, 2)
            };
            int capturedIndex = index;
            box.TextChanged += (s, e) => _viewModel.StoryDialogues[capturedIndex] = box.Text;
            return box;
        }

        /// <summary>
        /// 装飾テキストUIを再構築
        /// </summary>
        private void RebuildTextOverlayUI()
        {
            // カウント更新
            TextOverlayCountText.Text = $"{_viewModel.TextOverlayItems.Count} / {MaxTextOverlayItems}";

            // ボタン状態更新
            TextOverlayAddButton.IsEnabled = _viewModel.TextOverlayItems.Count < MaxTextOverlayItems;
            TextOverlayRemoveButton.IsEnabled = _viewModel.TextOverlayItems.Count > 0;

            // プレースホルダー表示/非表示
            TextOverlayPlaceholderText.Visibility = _viewModel.TextOverlayItems.Count == 0
                ? Visibility.Visible : Visibility.Collapsed;

            // 既存の行をクリア
            foreach (var row in _textOverlayRows)
            {
                TextOverlayItemsPanel.Children.Remove(row);
            }
            _textOverlayRows.Clear();

            // 各アイテムの行を作成
            for (int i = 0; i < _viewModel.TextOverlayItems.Count; i++)
            {
                var row = CreateTextOverlayRow(i);
                _textOverlayRows.Add(row);
                TextOverlayItemsPanel.Children.Add(row);
            }
        }

        /// <summary>
        /// 装飾テキスト行を作成
        /// </summary>
        private Border CreateTextOverlayRow(int index)
        {
            var item = _viewModel.TextOverlayItems[index];

            var border = new Border
            {
                Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["SubtleFillColorSecondaryBrush"],
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });  // 番号
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }); // 画像パス
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50) });  // 参照ボタン
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) }); // 位置
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(70) });  // サイズ
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) }); // レイヤー

            // 番号
            var numberText = new TextBlock
            {
                Text = $"{index + 1}.",
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
            };
            Grid.SetColumn(numberText, 0);
            grid.Children.Add(numberText);

            // 画像パス
            int capturedIndex = index;
            var imagePathBox = new TextBox
            {
                Text = item.ImagePath,
                PlaceholderText = "画像パス（ドロップ可）",
                AllowDrop = true,
                Margin = new Thickness(4, 0, 4, 0)
            };
            imagePathBox.TextChanged += (s, e) => _viewModel.TextOverlayItems[capturedIndex].ImagePath = imagePathBox.Text;
            imagePathBox.DragOver += ImagePathTextBox_DragOver;
            imagePathBox.Drop += async (s, e) =>
            {
                if (e.DataView.Contains(StandardDataFormats.StorageItems))
                {
                    var items = await e.DataView.GetStorageItemsAsync();
                    if (items.Count > 0)
                    {
                        var file = items[0] as Windows.Storage.StorageFile;
                        if (file != null && IsImageFile(file.Name))
                        {
                            _viewModel.TextOverlayItems[capturedIndex].ImagePath = file.Path;
                            imagePathBox.Text = file.Path;
                        }
                    }
                }
            };
            Grid.SetColumn(imagePathBox, 1);
            grid.Children.Add(imagePathBox);

            // 参照ボタン
            var browseButton = new Button
            {
                Content = "...",
                Margin = new Thickness(2, 0, 2, 0),
                Padding = new Thickness(8, 4, 8, 4)
            };
            browseButton.Click += async (s, e) =>
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
                    _viewModel.TextOverlayItems[capturedIndex].ImagePath = file.Path;
                    imagePathBox.Text = file.Path;
                }
            };
            Grid.SetColumn(browseButton, 2);
            grid.Children.Add(browseButton);

            // 位置
            var positionBox = new TextBox
            {
                Text = item.Position,
                PlaceholderText = "Center",
                Margin = new Thickness(4, 0, 4, 0)
            };
            positionBox.TextChanged += (s, e) => _viewModel.TextOverlayItems[capturedIndex].Position = positionBox.Text;
            Grid.SetColumn(positionBox, 3);
            grid.Children.Add(positionBox);

            // サイズ
            var sizeBox = new TextBox
            {
                Text = item.Size,
                PlaceholderText = "100%",
                Margin = new Thickness(4, 0, 4, 0)
            };
            sizeBox.TextChanged += (s, e) => _viewModel.TextOverlayItems[capturedIndex].Size = sizeBox.Text;
            Grid.SetColumn(sizeBox, 4);
            grid.Children.Add(sizeBox);

            // レイヤー
            var layerCombo = new ComboBox
            {
                Margin = new Thickness(4, 0, 0, 0)
            };
            foreach (var layer in Enum.GetValues<TextOverlayLayer>())
            {
                layerCombo.Items.Add(new ComboBoxItem { Content = layer.GetDisplayName(), Tag = layer });
            }
            // 現在の値を選択
            for (int i = 0; i < layerCombo.Items.Count; i++)
            {
                if (layerCombo.Items[i] is ComboBoxItem comboItem && comboItem.Tag is TextOverlayLayer l && l == item.Layer)
                {
                    layerCombo.SelectedIndex = i;
                    break;
                }
            }
            int layerCapturedIndex = capturedIndex;
            layerCombo.SelectionChanged += (s, e) =>
            {
                if (layerCombo.SelectedItem is ComboBoxItem comboItem && comboItem.Tag is TextOverlayLayer l)
                {
                    _viewModel.TextOverlayItems[layerCapturedIndex].Layer = l;
                }
            };
            Grid.SetColumn(layerCombo, 5);
            grid.Children.Add(layerCombo);

            border.Child = grid;
            return border;
        }

        /// <summary>
        /// カスタム雰囲気の表示/非表示を更新
        /// </summary>
        private void UpdateCustomMoodVisibility()
        {
            CustomMoodPanel.Visibility = _viewModel.StoryLightingMood == LightingMood.Custom
                ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// カスタム配置の表示/非表示を更新
        /// </summary>
        private void UpdateCustomLayoutVisibility()
        {
            CustomLayoutPanel.Visibility = _viewModel.StoryLayout == StoryLayout.Custom
                ? Visibility.Visible : Visibility.Collapsed;
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
        // イベントハンドラ - ドラッグアンドドロップ共通
        // ============================================================

        private void ImagePathTextBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
        }

        // ============================================================
        // イベントハンドラ - シーンタイプ
        // ============================================================

        private void SceneTypeRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;
            if (StorySceneRadio.IsChecked == true)
                _viewModel.SceneType = SceneType.Story;
            else if (BattleSceneRadio.IsChecked == true)
                _viewModel.SceneType = SceneType.Battle;
            else if (BossRaidRadio.IsChecked == true)
                _viewModel.SceneType = SceneType.BossRaid;
        }

        // ============================================================
        // イベントハンドラ - 背景設定
        // ============================================================

        private void BackgroundSourceRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;
            if (BackgroundFileRadio.IsChecked == true)
            {
                _viewModel.BackgroundSourceType = BackgroundSourceType.File;
                BackgroundFilePanel.Visibility = Visibility.Visible;
                BackgroundPromptPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                _viewModel.BackgroundSourceType = BackgroundSourceType.Prompt;
                BackgroundFilePanel.Visibility = Visibility.Collapsed;
                BackgroundPromptPanel.Visibility = Visibility.Visible;
            }
        }

        private void BackgroundImagePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.BackgroundImagePath = BackgroundImagePathTextBox.Text;
        }

        private async void BackgroundImagePathTextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    var file = items[0] as Windows.Storage.StorageFile;
                    if (file != null && IsImageFile(file.Name))
                    {
                        _viewModel.BackgroundImagePath = file.Path;
                        BackgroundImagePathTextBox.Text = file.Path;
                    }
                }
            }
        }

        private async void BrowseBackgroundButton_Click(object sender, RoutedEventArgs e)
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
                _viewModel.BackgroundImagePath = file.Path;
                BackgroundImagePathTextBox.Text = file.Path;
            }
        }

        private void BackgroundDescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.BackgroundDescription = BackgroundDescriptionTextBox.Text;
        }

        private void BlurAmountSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.StoryBlurAmount = BlurAmountSlider.Value;
            BlurAmountText.Text = ((int)BlurAmountSlider.Value).ToString();
        }

        private void LightingMoodComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (LightingMoodComboBox.SelectedItem is ComboBoxItem item && item.Tag is LightingMood mood)
            {
                _viewModel.StoryLightingMood = mood;
                UpdateCustomMoodVisibility();
            }
        }

        private void CustomMoodTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.StoryCustomMood = CustomMoodTextBox.Text;
        }

        // ============================================================
        // イベントハンドラ - 配置設定
        // ============================================================

        private void StoryLayoutComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (StoryLayoutComboBox.SelectedItem is ComboBoxItem item && item.Tag is StoryLayout layout)
            {
                _viewModel.StoryLayout = layout;
                UpdateCustomLayoutVisibility();
            }
        }

        private void StoryDistanceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (StoryDistanceComboBox.SelectedItem is ComboBoxItem item && item.Tag is StoryDistance distance)
            {
                _viewModel.StoryDistance = distance;
            }
        }

        private void CustomLayoutTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.StoryCustomLayout = CustomLayoutTextBox.Text;
        }

        // ============================================================
        // イベントハンドラ - キャラクター配置
        // ============================================================

        private void CharacterCountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (CharacterCountComboBox.SelectedItem is ComboBoxItem item && item.Tag is CharacterCount count)
            {
                _viewModel.StoryCharacterCount = count;
                RebuildCharacterUI();
                RebuildDialogueUI();
            }
        }

        // ============================================================
        // イベントハンドラ - ダイアログ設定
        // ============================================================

        private void NarrationPositionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (NarrationPositionComboBox.SelectedItem is ComboBoxItem item && item.Tag is NarrationPosition position)
            {
                _viewModel.StoryNarrationPosition = position;
            }
        }

        private void NarrationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.StoryNarration = NarrationTextBox.Text;
        }

        // ============================================================
        // イベントハンドラ - 装飾テキスト
        // ============================================================

        private void TextOverlayAddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.TextOverlayItems.Count < MaxTextOverlayItems)
            {
                _viewModel.TextOverlayItems.Add(new TextOverlayItem());
                RebuildTextOverlayUI();
            }
        }

        private void TextOverlayRemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.TextOverlayItems.Count > 0)
            {
                _viewModel.TextOverlayItems.RemoveAt(_viewModel.TextOverlayItems.Count - 1);
                RebuildTextOverlayUI();
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
