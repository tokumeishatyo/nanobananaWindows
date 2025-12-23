// rule.mdを読むこと
using System;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using nanobananaWindows.Models;
using nanobananaWindows.ViewModels;
using Windows.Storage.Pickers;

namespace nanobananaWindows.Views.Settings
{
    /// <summary>
    /// 衣装着用 詳細設定ダイアログ
    /// </summary>
    public sealed partial class OutfitSettingsDialog : ContentDialog
    {
        private readonly OutfitSettingsViewModel _viewModel;
        private readonly Window _parentWindow;
        private bool _isInitialized = false;

        /// <summary>
        /// 適用された設定を取得
        /// </summary>
        public OutfitSettingsViewModel? ResultSettings { get; private set; }

        public OutfitSettingsDialog(Window parentWindow, OutfitSettingsViewModel? initialSettings = null)
        {
            InitializeComponent();
            _parentWindow = parentWindow;

            // 既存設定がある場合はコピーして使用
            if (initialSettings != null)
            {
                _viewModel = initialSettings.Clone();
            }
            else
            {
                _viewModel = new OutfitSettingsViewModel();
            }

            // コンボボックスを初期化
            InitializeComboBoxes();

            // UIに反映
            LoadSettingsToUI();

            _isInitialized = true;
        }

        /// <summary>
        /// コンボボックスの初期化
        /// </summary>
        private void InitializeComboBoxes()
        {
            // カテゴリ
            OutfitCategoryComboBox.ItemsSource = Enum.GetValues<OutfitCategory>()
                .Select(c => new ComboBoxItem { Content = c.GetDisplayName(), Tag = c })
                .ToList();

            // 色
            OutfitColorComboBox.ItemsSource = Enum.GetValues<OutfitColor>()
                .Select(c => new ComboBoxItem { Content = c.GetDisplayName(), Tag = c })
                .ToList();

            // 柄
            OutfitPatternComboBox.ItemsSource = Enum.GetValues<OutfitPattern>()
                .Select(p => new ComboBoxItem { Content = p.GetDisplayName(), Tag = p })
                .ToList();

            // スタイル（印象）
            OutfitStyleComboBox.ItemsSource = Enum.GetValues<OutfitFashionStyle>()
                .Select(s => new ComboBoxItem { Content = s.GetDisplayName(), Tag = s })
                .ToList();
        }

        /// <summary>
        /// 形状コンボボックスを更新
        /// </summary>
        private void UpdateShapeComboBox(OutfitCategory category)
        {
            var shapes = category.GetShapes();
            OutfitShapeComboBox.ItemsSource = shapes
                .Select(s => new ComboBoxItem { Content = s, Tag = s })
                .ToList();

            // 現在の形状が新しいリストに存在する場合はそれを選択、なければ最初の項目
            var currentShape = _viewModel.OutfitShape;
            var index = Array.IndexOf(shapes, currentShape);
            OutfitShapeComboBox.SelectedIndex = index >= 0 ? index : 0;
        }

        /// <summary>
        /// ViewModelの設定をUIに反映
        /// </summary>
        private void LoadSettingsToUI()
        {
            // 共通
            BodySheetImagePathTextBox.Text = _viewModel.BodySheetImagePath;
            AdditionalDescriptionTextBox.Text = _viewModel.AdditionalDescription;

            // モード選択
            PresetModeRadio.IsChecked = _viewModel.UseOutfitBuilder;
            ReferenceModeRadio.IsChecked = !_viewModel.UseOutfitBuilder;
            UpdateModeVisibility(_viewModel.UseOutfitBuilder);

            // プリセットモード
            SelectComboBoxItem(OutfitCategoryComboBox, _viewModel.OutfitCategory);
            UpdateShapeComboBox(_viewModel.OutfitCategory);
            SelectComboBoxItemByTag(OutfitShapeComboBox, _viewModel.OutfitShape);
            SelectComboBoxItem(OutfitColorComboBox, _viewModel.OutfitColor);
            SelectComboBoxItem(OutfitPatternComboBox, _viewModel.OutfitPattern);
            SelectComboBoxItem(OutfitStyleComboBox, _viewModel.OutfitStyle);

            // 参考画像モード
            ReferenceOutfitImagePathTextBox.Text = _viewModel.ReferenceOutfitImagePath;
            ReferenceDescriptionTextBox.Text = _viewModel.ReferenceDescription;
            BodyPriorityRadio.IsChecked = _viewModel.FitMode == FitMode.BodyPriority;
            OutfitPriorityRadio.IsChecked = _viewModel.FitMode == FitMode.OutfitPriority;
            HybridRadio.IsChecked = _viewModel.FitMode == FitMode.Hybrid;
            IncludeHeadwearCheckBox.IsChecked = _viewModel.IncludeHeadwear;
            UpdateHeadwearCheckBoxState();
        }

        /// <summary>
        /// モードに応じてセクションの表示/非表示を切り替え
        /// </summary>
        private void UpdateModeVisibility(bool usePreset)
        {
            PresetSection.Visibility = usePreset ? Visibility.Visible : Visibility.Collapsed;
            ReferenceSection.Visibility = usePreset ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 頭部装飾チェックボックスの有効/無効を更新
        /// </summary>
        private void UpdateHeadwearCheckBoxState()
        {
            // ハイブリッドモードでは無効
            IncludeHeadwearCheckBox.IsEnabled = _viewModel.FitMode != FitMode.Hybrid;
        }

        /// <summary>
        /// コンボボックスの選択状態を設定（Enum用）
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
        /// コンボボックスの選択状態を設定（String Tag用）
        /// </summary>
        private void SelectComboBoxItemByTag(ComboBox comboBox, string value)
        {
            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (comboBox.Items[i] is ComboBoxItem item && item.Tag is string tagValue)
                {
                    if (tagValue == value)
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

        private void BodySheetImagePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.BodySheetImagePath = BodySheetImagePathTextBox.Text;
        }

        private async void BrowseBodySheetButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(_parentWindow);
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
                _viewModel.BodySheetImagePath = file.Path;
                BodySheetImagePathTextBox.Text = file.Path;
            }
        }

        private void OutfitModeRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;
            bool usePreset = PresetModeRadio.IsChecked == true;
            _viewModel.UseOutfitBuilder = usePreset;
            UpdateModeVisibility(usePreset);
        }

        private void AdditionalDescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.AdditionalDescription = AdditionalDescriptionTextBox.Text;
        }

        // ============================================================
        // イベントハンドラ - プリセットモード
        // ============================================================

        private void OutfitCategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (OutfitCategoryComboBox.SelectedItem is ComboBoxItem item && item.Tag is OutfitCategory category)
            {
                _viewModel.OutfitCategory = category;
                UpdateShapeComboBox(category);
            }
        }

        private void OutfitShapeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (OutfitShapeComboBox.SelectedItem is ComboBoxItem item && item.Tag is string shape)
            {
                _viewModel.OutfitShape = shape;
            }
        }

        private void OutfitColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (OutfitColorComboBox.SelectedItem is ComboBoxItem item && item.Tag is OutfitColor color)
            {
                _viewModel.OutfitColor = color;
            }
        }

        private void OutfitPatternComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (OutfitPatternComboBox.SelectedItem is ComboBoxItem item && item.Tag is OutfitPattern pattern)
            {
                _viewModel.OutfitPattern = pattern;
            }
        }

        private void OutfitStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (OutfitStyleComboBox.SelectedItem is ComboBoxItem item && item.Tag is OutfitFashionStyle style)
            {
                _viewModel.OutfitStyle = style;
            }
        }

        // ============================================================
        // イベントハンドラ - 参考画像モード
        // ============================================================

        private void ReferenceOutfitImagePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.ReferenceOutfitImagePath = ReferenceOutfitImagePathTextBox.Text;
        }

        private async void BrowseReferenceOutfitButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(_parentWindow);
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
                _viewModel.ReferenceOutfitImagePath = file.Path;
                ReferenceOutfitImagePathTextBox.Text = file.Path;
            }
        }

        private void ReferenceDescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.ReferenceDescription = ReferenceDescriptionTextBox.Text;
        }

        private void FitModeRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;
            if (BodyPriorityRadio.IsChecked == true)
                _viewModel.FitMode = FitMode.BodyPriority;
            else if (OutfitPriorityRadio.IsChecked == true)
                _viewModel.FitMode = FitMode.OutfitPriority;
            else if (HybridRadio.IsChecked == true)
                _viewModel.FitMode = FitMode.Hybrid;

            UpdateHeadwearCheckBoxState();
        }

        private void IncludeHeadwearCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.IncludeHeadwear = IncludeHeadwearCheckBox.IsChecked ?? false;
        }

        // ============================================================
        // ダイアログボタン
        // ============================================================

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // 適用：設定を返す
            ResultSettings = _viewModel;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // キャンセル：nullを返す
            ResultSettings = null;
        }
    }
}
