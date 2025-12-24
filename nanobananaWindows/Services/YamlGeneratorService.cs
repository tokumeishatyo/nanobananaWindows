// rule.mdを読むこと
using System.Collections.Generic;
using nanobananaWindows.Models;
using nanobananaWindows.ViewModels;

namespace nanobananaWindows.Services
{
    /// <summary>
    /// YAML生成サービス
    /// テンプレートファイルを読み込み、変数を置換してYAMLを生成
    /// </summary>
    public class YamlGeneratorService
    {
        private readonly TemplateEngine _templateEngine = new();

        /// <summary>
        /// YAML生成メソッド
        /// </summary>
        /// <param name="outputType">出力タイプ</param>
        /// <param name="mainViewModel">メインビューモデル</param>
        /// <returns>生成されたYAML文字列</returns>
        public string GenerateYaml(OutputType outputType, MainViewModel mainViewModel)
        {
            return outputType switch
            {
                OutputType.FaceSheet => GenerateFaceSheetYaml(mainViewModel),
                OutputType.BodySheet => GenerateBodySheetYaml(mainViewModel),
                // 他の出力タイプは順次実装
                OutputType.Outfit => GenerateOutfitYaml(mainViewModel),
                OutputType.Pose => GeneratePoseYaml(mainViewModel),
                OutputType.SceneBuilder => GeneratePlaceholderYaml("シーンビルダー", "05_scene.yaml"),
                OutputType.Background => GeneratePlaceholderYaml("背景生成", "06_background.yaml"),
                OutputType.DecorativeText => GeneratePlaceholderYaml("装飾テキスト", "07_decorative_text.yaml"),
                OutputType.FourPanelManga => GeneratePlaceholderYaml("4コマ漫画", "08_four_panel.yaml"),
                OutputType.StyleTransform => GeneratePlaceholderYaml("スタイル変換", "09_style_transform.yaml"),
                OutputType.Infographic => GeneratePlaceholderYaml("インフォグラフィック", "10_infographic.yaml"),
                _ => "# 未実装の出力タイプです"
            };
        }

        /// <summary>
        /// 顔三面図YAML生成
        /// </summary>
        private string GenerateFaceSheetYaml(MainViewModel mainViewModel)
        {
            var settings = mainViewModel.FaceSheetSettings;
            if (settings == null || !settings.HasSettings)
            {
                return "# Error: 顔三面図の設定がありません\n# 詳細設定ボタンから設定を入力してください";
            }

            // 変数辞書を構築
            var variables = BuildFaceSheetVariables(mainViewModel, settings);

            // テンプレートをレンダリング
            return _templateEngine.Render("01_face_sheet.yaml", variables);
        }

        /// <summary>
        /// 顔三面図用の変数辞書を構築
        /// </summary>
        private Dictionary<string, string> BuildFaceSheetVariables(
            MainViewModel mainViewModel,
            FaceSheetSettingsViewModel settings)
        {
            // 作者名の処理
            var authorName = mainViewModel.AuthorName?.Trim() ?? "";

            // title_overlay設定
            var titleOverlayEnabled = mainViewModel.IncludeTitleInImage;
            var (titlePosition, titleSize, authorPosition, authorSize) =
                GetTitleOverlayPositions(titleOverlayEnabled, !string.IsNullOrEmpty(authorName));

            return new Dictionary<string, string>
            {
                // ヘッダーパーシャル用
                ["header_comment"] = "Face Character Reference Sheet",
                ["type"] = "character_design",
                ["title"] = mainViewModel.Title ?? "",
                ["author"] = authorName,
                ["color_mode"] = mainViewModel.SelectedColorMode.ToYamlValue(),
                ["output_style"] = mainViewModel.SelectedOutputStyle.ToYamlValue(),
                ["aspect_ratio"] = mainViewModel.SelectedAspectRatio.ToYamlValue(),
                ["title_overlay_enabled"] = titleOverlayEnabled ? "true" : "false",
                ["title_position"] = titlePosition,
                ["title_size"] = titleSize,
                ["author_position"] = authorPosition,
                ["author_size"] = authorSize,

                // 顔三面図固有
                ["name"] = settings.CharacterName ?? "",
                ["reference_sheet"] = YamlUtilities.GetFileName(settings.ReferenceImagePath),
                ["description"] = YamlUtilities.ConvertNewlinesToComma(settings.AppearanceDescription)
            };
        }

        /// <summary>
        /// 素体三面図YAML生成
        /// </summary>
        private string GenerateBodySheetYaml(MainViewModel mainViewModel)
        {
            var settings = mainViewModel.BodySheetSettings;
            if (settings == null || !settings.HasSettings)
            {
                return "# Error: 素体三面図の設定がありません\n# 詳細設定ボタンから設定を入力してください";
            }

            var variables = BuildBodySheetVariables(mainViewModel, settings);
            return _templateEngine.Render("02_body_sheet.yaml", variables);
        }

        /// <summary>
        /// 素体三面図用の変数辞書を構築
        /// </summary>
        private Dictionary<string, string> BuildBodySheetVariables(
            MainViewModel mainViewModel,
            BodySheetSettingsViewModel settings)
        {
            var authorName = mainViewModel.AuthorName?.Trim() ?? "";
            var titleOverlayEnabled = mainViewModel.IncludeTitleInImage;
            var (titlePosition, titleSize, authorPosition, authorSize) =
                GetTitleOverlayPositions(titleOverlayEnabled, !string.IsNullOrEmpty(authorName));

            return new Dictionary<string, string>
            {
                // ヘッダーパーシャル用
                ["header_comment"] = "Body Reference Sheet (素体三面図)",
                ["type"] = "character_design",
                ["title"] = mainViewModel.Title ?? "",
                ["author"] = authorName,
                ["color_mode"] = mainViewModel.SelectedColorMode.ToYamlValue(),
                ["output_style"] = mainViewModel.SelectedOutputStyle.ToYamlValue(),
                ["aspect_ratio"] = mainViewModel.SelectedAspectRatio.ToYamlValue(),
                ["title_overlay_enabled"] = titleOverlayEnabled ? "true" : "false",
                ["title_position"] = titlePosition,
                ["title_size"] = titleSize,
                ["author_position"] = authorPosition,
                ["author_size"] = authorSize,

                // 素体三面図固有
                ["face_sheet"] = YamlUtilities.GetFileName(settings.FaceSheetImagePath),
                ["body_type"] = settings.BodyTypePreset.ToYamlValue(),
                ["bust"] = settings.BustFeature.ToYamlValue(),
                ["render_type"] = settings.BodyRenderType.ToYamlValue(),
                ["additional_notes"] = YamlUtilities.ConvertNewlinesToComma(settings.AdditionalDescription)
            };
        }

        /// <summary>
        /// 衣装着用YAML生成（モードに応じてテンプレートを切り替え）
        /// </summary>
        private string GenerateOutfitYaml(MainViewModel mainViewModel)
        {
            var settings = mainViewModel.OutfitSettings;
            if (settings == null || !settings.HasSettings)
            {
                return "# Error: 衣装着用の設定がありません\n# 詳細設定ボタンから設定を入力してください";
            }

            if (settings.UseOutfitBuilder)
            {
                // プリセットモード
                var variables = BuildOutfitPresetVariables(mainViewModel, settings);
                return _templateEngine.Render("03_outfit_preset.yaml", variables);
            }
            else
            {
                // 参考画像モード
                var variables = BuildOutfitReferenceVariables(mainViewModel, settings);
                return _templateEngine.Render("03_outfit_reference.yaml", variables);
            }
        }

        /// <summary>
        /// プリセットモード用の変数辞書を構築
        /// </summary>
        private Dictionary<string, string> BuildOutfitPresetVariables(
            MainViewModel mainViewModel,
            OutfitSettingsViewModel settings)
        {
            var authorName = mainViewModel.AuthorName?.Trim() ?? "";
            var titleOverlayEnabled = mainViewModel.IncludeTitleInImage;
            var (titlePosition, titleSize, authorPosition, authorSize) =
                GetTitleOverlayPositions(titleOverlayEnabled, !string.IsNullOrEmpty(authorName));

            // プロンプト生成
            var prompt = BuildOutfitPrompt(settings);

            return new Dictionary<string, string>
            {
                // ヘッダーパーシャル用
                ["header_comment"] = "Outfit Reference Sheet (衣装着用 - プリセット)",
                ["type"] = "character_design",
                ["title"] = mainViewModel.Title ?? "",
                ["author"] = authorName,
                ["color_mode"] = mainViewModel.SelectedColorMode.ToYamlValue(),
                ["output_style"] = mainViewModel.SelectedOutputStyle.ToYamlValue(),
                ["aspect_ratio"] = mainViewModel.SelectedAspectRatio.ToYamlValue(),
                ["title_overlay_enabled"] = titleOverlayEnabled ? "true" : "false",
                ["title_position"] = titlePosition,
                ["title_size"] = titleSize,
                ["author_position"] = authorPosition,
                ["author_size"] = authorSize,

                // 衣装着用（プリセット）固有
                ["body_sheet"] = YamlUtilities.GetFileName(settings.BodySheetImagePath),
                ["category"] = settings.OutfitCategory.ToYamlValue(),
                ["shape"] = settings.OutfitShape,
                ["color"] = settings.OutfitColor.ToYamlValue(),
                ["pattern"] = settings.OutfitPattern.ToYamlValue(),
                ["style_impression"] = settings.OutfitStyle.ToYamlValue(),
                ["prompt"] = prompt,
                ["additional_notes"] = YamlUtilities.ConvertNewlinesToComma(settings.AdditionalDescription)
            };
        }

        /// <summary>
        /// 参考画像モード用の変数辞書を構築
        /// </summary>
        private Dictionary<string, string> BuildOutfitReferenceVariables(
            MainViewModel mainViewModel,
            OutfitSettingsViewModel settings)
        {
            var authorName = mainViewModel.AuthorName?.Trim() ?? "";
            var titleOverlayEnabled = mainViewModel.IncludeTitleInImage;
            var (titlePosition, titleSize, authorPosition, authorSize) =
                GetTitleOverlayPositions(titleOverlayEnabled, !string.IsNullOrEmpty(authorName));

            return new Dictionary<string, string>
            {
                // ヘッダーパーシャル用
                ["header_comment"] = "Outfit Reference Sheet (衣装着用 - 参考画像)",
                ["type"] = "character_design",
                ["title"] = mainViewModel.Title ?? "",
                ["author"] = authorName,
                ["color_mode"] = mainViewModel.SelectedColorMode.ToYamlValue(),
                ["output_style"] = mainViewModel.SelectedOutputStyle.ToYamlValue(),
                ["aspect_ratio"] = mainViewModel.SelectedAspectRatio.ToYamlValue(),
                ["title_overlay_enabled"] = titleOverlayEnabled ? "true" : "false",
                ["title_position"] = titlePosition,
                ["title_size"] = titleSize,
                ["author_position"] = authorPosition,
                ["author_size"] = authorSize,

                // 衣装着用（参考画像）固有
                ["body_sheet"] = YamlUtilities.GetFileName(settings.BodySheetImagePath),
                ["outfit_reference"] = YamlUtilities.GetFileName(settings.ReferenceOutfitImagePath),
                ["description"] = YamlUtilities.ConvertNewlinesToComma(settings.ReferenceDescription),
                ["fit_mode"] = settings.FitMode.ToYamlValue(),
                ["include_headwear"] = settings.IncludeHeadwear ? "true" : "false",
                ["additional_notes"] = YamlUtilities.ConvertNewlinesToComma(settings.AdditionalDescription)
            };
        }

        /// <summary>
        /// プリセット衣装のプロンプトを生成
        /// </summary>
        private static string BuildOutfitPrompt(OutfitSettingsViewModel settings)
        {
            var parts = new List<string>();

            if (settings.OutfitCategory != OutfitCategory.Auto)
                parts.Add(settings.OutfitCategory.ToYamlValue());
            if (settings.OutfitShape != "おまかせ")
                parts.Add(settings.OutfitShape);
            if (settings.OutfitColor != OutfitColor.Auto)
                parts.Add(settings.OutfitColor.ToYamlValue());
            if (settings.OutfitPattern != OutfitPattern.Auto)
                parts.Add(settings.OutfitPattern.ToYamlValue());
            if (settings.OutfitStyle != OutfitFashionStyle.Auto)
                parts.Add(settings.OutfitStyle.ToYamlValue());

            return parts.Count > 0 ? string.Join(", ", parts) : "auto";
        }

        /// <summary>
        /// ポーズYAML生成（モードに応じてテンプレートを切り替え）
        /// </summary>
        private string GeneratePoseYaml(MainViewModel mainViewModel)
        {
            var settings = mainViewModel.PoseSettings;
            if (settings == null || !settings.HasSettings)
            {
                return "# Error: ポーズの設定がありません\n# 詳細設定ボタンから設定を入力してください";
            }

            if (settings.UsePoseCapture)
            {
                // キャプチャモード
                var variables = BuildPoseCaptureVariables(mainViewModel, settings);
                return _templateEngine.Render("04_pose_capture.yaml", variables);
            }
            else
            {
                // プリセットモード
                var variables = BuildPosePresetVariables(mainViewModel, settings);
                return _templateEngine.Render("04_pose_preset.yaml", variables);
            }
        }

        /// <summary>
        /// プリセットモード用の変数辞書を構築
        /// </summary>
        private Dictionary<string, string> BuildPosePresetVariables(
            MainViewModel mainViewModel,
            PoseSettingsViewModel settings)
        {
            var authorName = mainViewModel.AuthorName?.Trim() ?? "";
            var titleOverlayEnabled = mainViewModel.IncludeTitleInImage;
            var (titlePosition, titleSize, authorPosition, authorSize) =
                GetTitleOverlayPositions(titleOverlayEnabled, !string.IsNullOrEmpty(authorName));

            return new Dictionary<string, string>
            {
                // ヘッダーパーシャル用
                ["header_comment"] = "Pose Image (ポーズ画像 - プリセット)",
                ["type"] = "character_design",
                ["title"] = mainViewModel.Title ?? "",
                ["author"] = authorName,
                ["color_mode"] = mainViewModel.SelectedColorMode.ToYamlValue(),
                ["output_style"] = mainViewModel.SelectedOutputStyle.ToYamlValue(),
                ["aspect_ratio"] = mainViewModel.SelectedAspectRatio.ToYamlValue(),
                ["title_overlay_enabled"] = titleOverlayEnabled ? "true" : "false",
                ["title_position"] = titlePosition,
                ["title_size"] = titleSize,
                ["author_position"] = authorPosition,
                ["author_size"] = authorSize,

                // ポーズ（プリセット）固有
                ["character_sheet"] = YamlUtilities.GetFileName(settings.OutfitSheetImagePath),
                ["eye_line"] = settings.EyeLine.ToYamlValue(),
                ["expression"] = settings.Expression.ToYamlValue(),
                ["expression_detail"] = YamlUtilities.ConvertNewlinesToComma(settings.ExpressionDetail),
                ["action_description"] = YamlUtilities.ConvertNewlinesToComma(settings.ActionDescription),
                ["include_effects"] = settings.IncludeEffects ? "true" : "false",
                ["wind_effect"] = settings.WindEffect.ToYamlValue(),
                ["transparent_background"] = settings.TransparentBackground ? "true" : "false"
            };
        }

        /// <summary>
        /// キャプチャモード用の変数辞書を構築
        /// </summary>
        private Dictionary<string, string> BuildPoseCaptureVariables(
            MainViewModel mainViewModel,
            PoseSettingsViewModel settings)
        {
            var authorName = mainViewModel.AuthorName?.Trim() ?? "";
            var titleOverlayEnabled = mainViewModel.IncludeTitleInImage;
            var (titlePosition, titleSize, authorPosition, authorSize) =
                GetTitleOverlayPositions(titleOverlayEnabled, !string.IsNullOrEmpty(authorName));

            return new Dictionary<string, string>
            {
                // ヘッダーパーシャル用
                ["header_comment"] = "Pose Image (ポーズ画像 - キャプチャ)",
                ["type"] = "character_design",
                ["title"] = mainViewModel.Title ?? "",
                ["author"] = authorName,
                ["color_mode"] = mainViewModel.SelectedColorMode.ToYamlValue(),
                ["output_style"] = mainViewModel.SelectedOutputStyle.ToYamlValue(),
                ["aspect_ratio"] = mainViewModel.SelectedAspectRatio.ToYamlValue(),
                ["title_overlay_enabled"] = titleOverlayEnabled ? "true" : "false",
                ["title_position"] = titlePosition,
                ["title_size"] = titleSize,
                ["author_position"] = authorPosition,
                ["author_size"] = authorSize,

                // ポーズ（キャプチャ）固有
                ["pose_reference"] = YamlUtilities.GetFileName(settings.PoseReferenceImagePath),
                ["character_sheet"] = YamlUtilities.GetFileName(settings.OutfitSheetImagePath),
                ["eye_line"] = settings.EyeLine.ToYamlValue(),
                ["expression"] = settings.Expression.ToYamlValue(),
                ["expression_detail"] = YamlUtilities.ConvertNewlinesToComma(settings.ExpressionDetail),
                ["include_effects"] = settings.IncludeEffects ? "true" : "false",
                ["wind_effect"] = settings.WindEffect.ToYamlValue(),
                ["transparent_background"] = settings.TransparentBackground ? "true" : "false"
            };
        }

        /// <summary>
        /// title_overlayの位置設定を取得
        /// </summary>
        private static (string titlePosition, string titleSize, string authorPosition, string authorSize)
            GetTitleOverlayPositions(bool includeTitleInImage, bool hasAuthor)
        {
            if (!includeTitleInImage)
            {
                return ("", "", "", "");
            }

            if (hasAuthor)
            {
                // 作者名あり: タイトル左(large)、作者名右(small)
                return ("top-left", "large", "top-right", "small");
            }
            else
            {
                // 作者名なし: タイトルのみtop-center
                return ("top-center", "medium", "", "");
            }
        }

        /// <summary>
        /// 未実装の出力タイプ用プレースホルダー
        /// </summary>
        private static string GeneratePlaceholderYaml(string typeName, string templateName)
        {
            return $"""
# ====================================================
# {typeName} - 実装予定
# ====================================================
# テンプレート: {templateName}
#
# この出力タイプは実装予定です。
# yaml_templates/{templateName} を使用します。
# ====================================================
""";
        }
    }
}
