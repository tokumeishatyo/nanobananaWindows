// rule.mdを読むこと
using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using nanobananaWindows.ViewModels;
using Windows.Storage.Pickers;

namespace nanobananaWindows.Views.Settings
{
    /// <summary>
    /// 顔三面図 詳細設定ダイアログ
    /// </summary>
    public sealed partial class FaceSheetSettingsDialog : ContentDialog
    {
        private readonly FaceSheetSettingsViewModel _viewModel;
        private readonly Window _parentWindow;

        /// <summary>
        /// 適用された設定を取得
        /// </summary>
        public FaceSheetSettingsViewModel? ResultSettings { get; private set; }

        public FaceSheetSettingsDialog(Window parentWindow, FaceSheetSettingsViewModel? initialSettings = null)
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
                _viewModel = new FaceSheetSettingsViewModel();
            }

            // UIに反映
            LoadSettingsToUI();
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
                _viewModel.ReferenceImagePath = file.Path;
                ReferenceImagePathTextBox.Text = file.Path;
            }
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
