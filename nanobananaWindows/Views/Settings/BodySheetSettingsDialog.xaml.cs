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
    /// 素体三面図 詳細設定ダイアログ
    /// </summary>
    public sealed partial class BodySheetSettingsDialog : ContentDialog
    {
        private readonly BodySheetSettingsViewModel _viewModel;
        private readonly Window _parentWindow;
        private bool _isInitialized = false;

        /// <summary>
        /// 適用された設定を取得
        /// </summary>
        public BodySheetSettingsViewModel? ResultSettings { get; private set; }

        public BodySheetSettingsDialog(Window parentWindow, BodySheetSettingsViewModel? initialSettings = null)
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
                _viewModel = new BodySheetSettingsViewModel();
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
            // 体型プリセット
            BodyTypePresetComboBox.ItemsSource = Enum.GetValues<BodyTypePreset>()
                .Select(p => new ComboBoxItem { Content = p.GetDisplayName(), Tag = p })
                .ToList();

            // バスト特徴
            BustFeatureComboBox.ItemsSource = Enum.GetValues<BustFeature>()
                .Select(f => new ComboBoxItem { Content = f.GetDisplayName(), Tag = f })
                .ToList();

            // 素体表現タイプ
            BodyRenderTypeComboBox.ItemsSource = Enum.GetValues<BodyRenderType>()
                .Select(t => new ComboBoxItem { Content = t.GetDisplayName(), Tag = t })
                .ToList();
        }

        /// <summary>
        /// ViewModelの設定をUIに反映
        /// </summary>
        private void LoadSettingsToUI()
        {
            FaceSheetImagePathTextBox.Text = _viewModel.FaceSheetImagePath;
            AdditionalDescriptionTextBox.Text = _viewModel.AdditionalDescription;

            // コンボボックスの選択状態を設定
            SelectComboBoxItem(BodyTypePresetComboBox, _viewModel.BodyTypePreset);
            SelectComboBoxItem(BustFeatureComboBox, _viewModel.BustFeature);
            SelectComboBoxItem(BodyRenderTypeComboBox, _viewModel.BodyRenderType);
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

        private void FaceSheetImagePathTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.FaceSheetImagePath = FaceSheetImagePathTextBox.Text;
        }

        private async void BrowseFaceSheetButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();

            // WinUI 3ではウィンドウハンドルを設定する必要がある
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
                _viewModel.FaceSheetImagePath = file.Path;
                FaceSheetImagePathTextBox.Text = file.Path;
            }
        }

        private void BodyTypePresetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (BodyTypePresetComboBox.SelectedItem is ComboBoxItem item && item.Tag is BodyTypePreset preset)
            {
                _viewModel.BodyTypePreset = preset;
            }
        }

        private void BustFeatureComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (BustFeatureComboBox.SelectedItem is ComboBoxItem item && item.Tag is BustFeature feature)
            {
                _viewModel.BustFeature = feature;
            }
        }

        private void BodyRenderTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isInitialized) return;
            if (BodyRenderTypeComboBox.SelectedItem is ComboBoxItem item && item.Tag is BodyRenderType renderType)
            {
                _viewModel.BodyRenderType = renderType;
            }
        }

        private void AdditionalDescriptionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitialized) return;
            _viewModel.AdditionalDescription = AdditionalDescriptionTextBox.Text;
        }

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
