// rule.mdを読むこと
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using nanobananaWindows.Models;

namespace nanobananaWindows.ViewModels
{
    /// <summary>
    /// メイン画面のViewModel
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        // ============================================================
        // 出力タイプ選択
        // ============================================================

        private OutputType _selectedOutputType = OutputType.FaceSheet;
        public OutputType SelectedOutputType
        {
            get => _selectedOutputType;
            set
            {
                if (SetProperty(ref _selectedOutputType, value))
                {
                    OnPropertyChanged(nameof(SettingsStatusText));
                    OnPropertyChanged(nameof(IsSettingsButtonEnabled));
                }
            }
        }

        public string SettingsStatusText => $"現在の設定: {SelectedOutputType.GetDisplayName()}";

        public bool IsSettingsButtonEnabled => true; // 常に有効

        // ============================================================
        // 各出力タイプの詳細設定
        // ============================================================

        private FaceSheetSettingsViewModel _faceSheetSettings = new();
        /// <summary>
        /// 顔三面図の詳細設定
        /// </summary>
        public FaceSheetSettingsViewModel FaceSheetSettings
        {
            get => _faceSheetSettings;
            set
            {
                if (SetProperty(ref _faceSheetSettings, value))
                {
                    OnPropertyChanged(nameof(SettingsStatusText));
                }
            }
        }

        private BodySheetSettingsViewModel _bodySheetSettings = new();
        /// <summary>
        /// 素体三面図の詳細設定
        /// </summary>
        public BodySheetSettingsViewModel BodySheetSettings
        {
            get => _bodySheetSettings;
            set
            {
                if (SetProperty(ref _bodySheetSettings, value))
                {
                    OnPropertyChanged(nameof(SettingsStatusText));
                }
            }
        }

        private OutfitSettingsViewModel _outfitSettings = new();
        /// <summary>
        /// 衣装着用の詳細設定
        /// </summary>
        public OutfitSettingsViewModel OutfitSettings
        {
            get => _outfitSettings;
            set
            {
                if (SetProperty(ref _outfitSettings, value))
                {
                    OnPropertyChanged(nameof(SettingsStatusText));
                }
            }
        }

        private PoseSettingsViewModel _poseSettings = new();
        /// <summary>
        /// ポーズの詳細設定
        /// </summary>
        public PoseSettingsViewModel PoseSettings
        {
            get => _poseSettings;
            set
            {
                if (SetProperty(ref _poseSettings, value))
                {
                    OnPropertyChanged(nameof(SettingsStatusText));
                }
            }
        }

        private SceneBuilderSettingsViewModel _sceneBuilderSettings = new();
        /// <summary>
        /// シーンビルダーの詳細設定
        /// </summary>
        public SceneBuilderSettingsViewModel SceneBuilderSettings
        {
            get => _sceneBuilderSettings;
            set
            {
                if (SetProperty(ref _sceneBuilderSettings, value))
                {
                    OnPropertyChanged(nameof(SettingsStatusText));
                }
            }
        }

        /// <summary>
        /// 現在の出力タイプの設定状態テキストを取得
        /// </summary>
        public string GetCurrentSettingsStatus()
        {
            return SelectedOutputType switch
            {
                OutputType.FaceSheet => FaceSheetSettings.HasSettings ? "設定済み" : "未設定",
                OutputType.BodySheet => BodySheetSettings.HasSettings ? "設定済み" : "未設定",
                OutputType.Outfit => OutfitSettings.HasSettings ? "設定済み" : "未設定",
                OutputType.Pose => PoseSettings.HasSettings ? "設定済み" : "未設定",
                OutputType.SceneBuilder => SceneBuilderSettings.HasSettings ? "設定済み" : "未設定",
                // TODO: 他の出力タイプの設定状態を追加
                _ => "未設定"
            };
        }

        // ============================================================
        // スタイル設定
        // ============================================================

        private ColorMode _selectedColorMode = ColorMode.FullColor;
        public ColorMode SelectedColorMode
        {
            get => _selectedColorMode;
            set => SetProperty(ref _selectedColorMode, value);
        }

        private OutputStyle _selectedOutputStyle = OutputStyle.Anime;
        public OutputStyle SelectedOutputStyle
        {
            get => _selectedOutputStyle;
            set => SetProperty(ref _selectedOutputStyle, value);
        }

        private AspectRatio _selectedAspectRatio = AspectRatio.Square;
        public AspectRatio SelectedAspectRatio
        {
            get => _selectedAspectRatio;
            set => SetProperty(ref _selectedAspectRatio, value);
        }

        // ============================================================
        // 基本情報
        // ============================================================

        private string _title = "";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _includeTitleInImage = false;
        public bool IncludeTitleInImage
        {
            get => _includeTitleInImage;
            set => SetProperty(ref _includeTitleInImage, value);
        }

        private string _authorName = "";
        public string AuthorName
        {
            get => _authorName;
            set => SetProperty(ref _authorName, value);
        }

        // ============================================================
        // API設定
        // ============================================================

        private OutputMode _selectedOutputMode = OutputMode.Yaml;
        public OutputMode SelectedOutputMode
        {
            get => _selectedOutputMode;
            set
            {
                if (SetProperty(ref _selectedOutputMode, value))
                {
                    OnPropertyChanged(nameof(IsApiModeEnabled));
                    OnPropertyChanged(nameof(IsApiGenerateButtonEnabled));
                }
            }
        }

        private string _apiKey = "";
        public string ApiKey
        {
            get => _apiKey;
            set => SetProperty(ref _apiKey, value);
        }

        private ApiSubMode _selectedApiSubMode = ApiSubMode.Normal;
        public ApiSubMode SelectedApiSubMode
        {
            get => _selectedApiSubMode;
            set
            {
                if (SetProperty(ref _selectedApiSubMode, value))
                {
                    OnPropertyChanged(nameof(ShowRedrawInstruction));
                    OnPropertyChanged(nameof(ShowSimplePrompt));
                }
            }
        }

        private string _referenceImagePath = "";
        public string ReferenceImagePath
        {
            get => _referenceImagePath;
            set => SetProperty(ref _referenceImagePath, value);
        }

        private string _redrawInstruction = "";
        public string RedrawInstruction
        {
            get => _redrawInstruction;
            set => SetProperty(ref _redrawInstruction, value);
        }

        private string _simplePrompt = "";
        public string SimplePrompt
        {
            get => _simplePrompt;
            set => SetProperty(ref _simplePrompt, value);
        }

        private Resolution _selectedResolution = Resolution.TwoK;
        public Resolution SelectedResolution
        {
            get => _selectedResolution;
            set => SetProperty(ref _selectedResolution, value);
        }

        public bool IsApiModeEnabled => SelectedOutputMode == OutputMode.Api;
        public bool ShowRedrawInstruction => IsApiModeEnabled && SelectedApiSubMode == ApiSubMode.Redraw;
        public bool ShowSimplePrompt => IsApiModeEnabled && SelectedApiSubMode == ApiSubMode.Simple;
        public bool IsApiGenerateButtonEnabled => IsApiModeEnabled && !string.IsNullOrEmpty(ApiKey);

        // ============================================================
        // YAMLプレビュー
        // ============================================================

        private string _yamlPreviewText = "";
        public string YamlPreviewText
        {
            get => _yamlPreviewText;
            set => SetProperty(ref _yamlPreviewText, value);
        }

        // ============================================================
        // 画像プレビュー
        // ============================================================

        private bool _isGenerating = false;
        public bool IsGenerating
        {
            get => _isGenerating;
            set
            {
                if (SetProperty(ref _isGenerating, value))
                {
                    OnPropertyChanged(nameof(GenerateButtonText));
                }
            }
        }

        public string GenerateButtonText => IsGenerating ? "生成中..." : "画像生成（API）";

        public bool HasGeneratedImage => false; // TODO: 実装時に更新

        public bool IsSaveImageButtonEnabled => HasGeneratedImage;
        public bool IsRefineImageButtonEnabled => HasGeneratedImage;

        // ============================================================
        // 使用状況
        // ============================================================

        public string UsageStatusText => "本日: 0回 / 今月: 0回";

        // ============================================================
        // コマンド（後で実装）
        // ============================================================

        public void GenerateYaml()
        {
            // TODO: YAML生成処理
            YamlPreviewText = "# YAML生成後にここに表示されます";
        }

        public void ResetAll()
        {
            // TODO: リセット処理
            Title = "";
            AuthorName = "";
            YamlPreviewText = "";
        }

        public void OpenSettingsWindow()
        {
            // TODO: 詳細設定ウィンドウを開く
        }

        public void ClearApiKey()
        {
            ApiKey = "";
        }
    }
}
