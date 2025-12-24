// rule.mdを読むこと
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using nanobananaWindows.Models;
using nanobananaWindows.Services;

namespace nanobananaWindows.ViewModels
{
    /// <summary>
    /// メイン画面のViewModel
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly YamlGeneratorService _yamlGeneratorService = new();
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

        private BackgroundSettingsViewModel _backgroundSettings = new();
        /// <summary>
        /// 背景生成の詳細設定
        /// </summary>
        public BackgroundSettingsViewModel BackgroundSettings
        {
            get => _backgroundSettings;
            set
            {
                if (SetProperty(ref _backgroundSettings, value))
                {
                    OnPropertyChanged(nameof(SettingsStatusText));
                }
            }
        }

        private DecorativeTextSettingsViewModel _decorativeTextSettings = new();
        /// <summary>
        /// 装飾テキストの詳細設定
        /// </summary>
        public DecorativeTextSettingsViewModel DecorativeTextSettings
        {
            get => _decorativeTextSettings;
            set
            {
                if (SetProperty(ref _decorativeTextSettings, value))
                {
                    OnPropertyChanged(nameof(SettingsStatusText));
                }
            }
        }

        private FourPanelMangaSettingsViewModel _fourPanelMangaSettings = new();
        /// <summary>
        /// 4コマ漫画の詳細設定
        /// </summary>
        public FourPanelMangaSettingsViewModel FourPanelMangaSettings
        {
            get => _fourPanelMangaSettings;
            set
            {
                if (SetProperty(ref _fourPanelMangaSettings, value))
                {
                    OnPropertyChanged(nameof(SettingsStatusText));
                }
            }
        }

        private StyleTransformSettingsViewModel _styleTransformSettings = new();
        /// <summary>
        /// スタイル変換の詳細設定
        /// </summary>
        public StyleTransformSettingsViewModel StyleTransformSettings
        {
            get => _styleTransformSettings;
            set
            {
                if (SetProperty(ref _styleTransformSettings, value))
                {
                    OnPropertyChanged(nameof(SettingsStatusText));
                }
            }
        }

        private InfographicSettingsViewModel _infographicSettings = new();
        /// <summary>
        /// インフォグラフィックの詳細設定
        /// </summary>
        public InfographicSettingsViewModel InfographicSettings
        {
            get => _infographicSettings;
            set
            {
                if (SetProperty(ref _infographicSettings, value))
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
                OutputType.Background => BackgroundSettings.HasSettings ? "設定済み" : "未設定",
                OutputType.DecorativeText => DecorativeTextSettings.HasSettings ? "設定済み" : "未設定",
                OutputType.FourPanelManga => FourPanelMangaSettings.HasSettings ? "設定済み" : "未設定",
                OutputType.StyleTransform => StyleTransformSettings.HasSettings ? "設定済み" : "未設定",
                OutputType.Infographic => InfographicSettings.HasSettings ? "設定済み" : "未設定",
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

        /// <summary>
        /// YAML生成（バリデーションエラーがあればエラーメッセージを返す）
        /// </summary>
        /// <returns>エラーメッセージ（成功時はnull）</returns>
        public string? GenerateYaml()
        {
            // タイトル必須チェック
            if (string.IsNullOrWhiteSpace(Title))
            {
                return "タイトルを入力してください。";
            }

            // 各出力タイプ固有のチェック
            var validationError = ValidateCurrentOutputType();
            if (validationError != null)
            {
                return validationError;
            }

            // YAML生成
            YamlPreviewText = _yamlGeneratorService.GenerateYaml(SelectedOutputType, this);
            return null;
        }

        /// <summary>
        /// 現在の出力タイプのバリデーション
        /// </summary>
        /// <returns>エラーメッセージ（問題なければnull）</returns>
        private string? ValidateCurrentOutputType()
        {
            return SelectedOutputType switch
            {
                OutputType.FaceSheet => ValidateFaceSheetSettings(),
                OutputType.BodySheet => ValidateBodySheetSettings(),
                OutputType.Outfit => ValidateOutfitSettings(),
                OutputType.Pose => ValidatePoseSettings(),
                OutputType.SceneBuilder => ValidateSceneBuilderSettings(),
                OutputType.Background => ValidateBackgroundSettings(),
                OutputType.DecorativeText => ValidateDecorativeTextSettings(),
                OutputType.FourPanelManga => ValidateFourPanelMangaSettings(),
                // 他の出力タイプは順次実装
                _ => null
            };
        }

        /// <summary>
        /// 顔三面図設定のバリデーション
        /// </summary>
        private string? ValidateFaceSheetSettings()
        {
            var settings = FaceSheetSettings;
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(settings.CharacterName))
            {
                errors.Add("名前");
            }
            if (string.IsNullOrWhiteSpace(settings.AppearanceDescription))
            {
                errors.Add("外見説明");
            }

            if (errors.Count > 0)
            {
                return $"顔三面図の必須項目が未入力です。\n\n以下の項目を入力してください：\n・{string.Join("\n・", errors)}";
            }

            return null;
        }

        /// <summary>
        /// 素体三面図設定のバリデーション
        /// </summary>
        private string? ValidateBodySheetSettings()
        {
            var settings = BodySheetSettings;

            if (string.IsNullOrWhiteSpace(settings.FaceSheetImagePath))
            {
                return "素体三面図の必須項目が未入力です。\n\n顔三面図の画像パスを入力してください。";
            }

            return null;
        }

        /// <summary>
        /// 衣装着用設定のバリデーション
        /// </summary>
        private string? ValidateOutfitSettings()
        {
            var settings = OutfitSettings;
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(settings.BodySheetImagePath))
            {
                errors.Add("素体三面図の画像パス");
            }

            // 参考画像モードの場合
            if (!settings.UseOutfitBuilder && string.IsNullOrWhiteSpace(settings.ReferenceOutfitImagePath))
            {
                errors.Add("衣装参考画像");
            }

            if (errors.Count > 0)
            {
                return $"衣装着用の必須項目が未入力です。\n\n以下の項目を入力してください：\n・{string.Join("\n・", errors)}";
            }

            return null;
        }

        /// <summary>
        /// ポーズ設定のバリデーション
        /// </summary>
        private string? ValidatePoseSettings()
        {
            var settings = PoseSettings;
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(settings.OutfitSheetImagePath))
            {
                errors.Add("入力画像（衣装着用三面図）");
            }

            // キャプチャモードの場合
            if (settings.UsePoseCapture && string.IsNullOrWhiteSpace(settings.PoseReferenceImagePath))
            {
                errors.Add("ポーズ参考画像");
            }

            if (errors.Count > 0)
            {
                return $"ポーズの必須項目が未入力です。\n\n以下の項目を入力してください：\n・{string.Join("\n・", errors)}";
            }

            return null;
        }

        /// <summary>
        /// シーンビルダー設定のバリデーション
        /// </summary>
        private string? ValidateSceneBuilderSettings()
        {
            var settings = SceneBuilderSettings;
            var errors = new List<string>();

            // 背景: ファイル指定時は背景画像が必須
            if (settings.BackgroundSourceType == Models.BackgroundSourceType.File &&
                string.IsNullOrWhiteSpace(settings.BackgroundImagePath))
            {
                errors.Add("背景画像（ファイル指定モード）");
            }

            // 背景: 情景説明時は説明が必須
            if (settings.BackgroundSourceType == Models.BackgroundSourceType.Prompt &&
                string.IsNullOrWhiteSpace(settings.BackgroundDescription))
            {
                errors.Add("背景の情景説明");
            }

            // キャラクター画像: 人数分全て必須
            int charCount = settings.StoryCharacterCount.GetIntValue();
            for (int i = 0; i < charCount; i++)
            {
                if (string.IsNullOrWhiteSpace(settings.StoryCharacters[i].ImagePath))
                {
                    errors.Add($"キャラクター {i + 1} の画像パス");
                }
            }

            // 装飾テキスト: 追加した分は全て画像必須
            for (int i = 0; i < settings.TextOverlayItems.Count; i++)
            {
                if (string.IsNullOrWhiteSpace(settings.TextOverlayItems[i].ImagePath))
                {
                    errors.Add($"装飾テキスト {i + 1} の画像パス");
                }
            }

            if (errors.Count > 0)
            {
                return $"シーンビルダーの必須項目が未入力です。\n\n以下の項目を入力してください：\n・{string.Join("\n・", errors)}";
            }

            return null;
        }

        /// <summary>
        /// 背景生成設定のバリデーション
        /// </summary>
        private string? ValidateBackgroundSettings()
        {
            var settings = BackgroundSettings;
            var errors = new List<string>();

            if (settings.UseReferenceImage)
            {
                // 参考画像モード
                if (string.IsNullOrWhiteSpace(settings.ReferenceImagePath))
                {
                    errors.Add("参考画像");
                }
            }
            else
            {
                // 説明文モード
                if (string.IsNullOrWhiteSpace(settings.Description))
                {
                    errors.Add("背景の情景説明");
                }
            }

            if (errors.Count > 0)
            {
                return $"背景生成の必須項目が未入力です。\n\n以下の項目を入力してください：\n・{string.Join("\n・", errors)}";
            }

            return null;
        }

        /// <summary>
        /// 装飾テキスト設定のバリデーション
        /// </summary>
        private string? ValidateDecorativeTextSettings()
        {
            var settings = DecorativeTextSettings;
            var errors = new List<string>();

            // テキスト内容は必須
            if (string.IsNullOrWhiteSpace(settings.Text))
            {
                errors.Add("テキスト内容");
            }

            // メッセージウィンドウで顔アイコン使用時は画像パス必須
            if (settings.TextType == Models.DecorativeTextType.MessageWindow &&
                settings.FaceIconPosition != Models.FaceIconPosition.None &&
                string.IsNullOrWhiteSpace(settings.FaceIconImagePath))
            {
                errors.Add("顔アイコン画像");
            }

            if (errors.Count > 0)
            {
                return $"装飾テキストの必須項目が未入力です。\n\n以下の項目を入力してください：\n・{string.Join("\n・", errors)}";
            }

            return null;
        }

        /// <summary>
        /// 4コマ漫画設定のバリデーション
        /// </summary>
        private string? ValidateFourPanelMangaSettings()
        {
            var settings = FourPanelMangaSettings;
            var errors = new List<string>();

            // キャラクター1は必須
            if (string.IsNullOrWhiteSpace(settings.Character1Name))
            {
                errors.Add("キャラクター1の名前");
            }
            if (string.IsNullOrWhiteSpace(settings.Character1ImagePath))
            {
                errors.Add("キャラクター1の画像");
            }
            if (string.IsNullOrWhiteSpace(settings.Character1Description))
            {
                errors.Add("キャラクター1の説明");
            }

            if (errors.Count > 0)
            {
                return $"4コマ漫画の必須項目が未入力です。\n\n以下の項目を入力してください：\n・{string.Join("\n・", errors)}";
            }

            return null;
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
