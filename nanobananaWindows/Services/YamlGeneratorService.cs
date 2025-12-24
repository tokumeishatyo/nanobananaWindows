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
                OutputType.SceneBuilder => GenerateSceneBuilderYaml(mainViewModel),
                OutputType.Background => GenerateBackgroundYaml(mainViewModel),
                OutputType.DecorativeText => GenerateDecorativeTextYaml(mainViewModel),
                OutputType.FourPanelManga => GenerateFourPanelMangaYaml(mainViewModel),
                OutputType.StyleTransform => GenerateStyleTransformYaml(mainViewModel),
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
        /// シーンビルダーYAML生成（ストーリーシーン）
        /// </summary>
        private string GenerateSceneBuilderYaml(MainViewModel mainViewModel)
        {
            var settings = mainViewModel.SceneBuilderSettings;
            if (settings == null || !settings.HasSettings)
            {
                return "# Error: シーンビルダーの設定がありません\n# 詳細設定ボタンから設定を入力してください";
            }

            var variables = BuildSceneBuilderVariables(mainViewModel, settings);
            return _templateEngine.Render("05_scene_story.yaml", variables);
        }

        /// <summary>
        /// シーンビルダー用の変数辞書を構築
        /// </summary>
        private Dictionary<string, string> BuildSceneBuilderVariables(
            MainViewModel mainViewModel,
            SceneBuilderSettingsViewModel settings)
        {
            var authorName = mainViewModel.AuthorName?.Trim() ?? "";
            var titleOverlayEnabled = mainViewModel.IncludeTitleInImage;
            var (titlePosition, titleSize, authorPosition, authorSize) =
                GetTitleOverlayPositions(titleOverlayEnabled, !string.IsNullOrEmpty(authorName));

            var variables = new Dictionary<string, string>
            {
                // ヘッダーパーシャル用
                ["header_comment"] = "Story Scene Composition (シーンビルダー - ストーリー)",
                ["type"] = "scene_composition",
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

                // 背景設定
                ["background_source_type"] = settings.BackgroundSourceType.ToYamlValue(),
                ["background_image"] = YamlUtilities.GetFileName(settings.BackgroundImagePath),
                ["background_description"] = YamlUtilities.ConvertNewlinesToComma(settings.BackgroundDescription),
                ["blur_amount"] = ((int)settings.StoryBlurAmount).ToString(),
                ["lighting_mood"] = GetLightingMoodValue(settings),

                // 配置設定
                ["layout_type"] = GetLayoutTypeValue(settings),
                ["distance"] = settings.StoryDistance.ToYamlValue(),

                // キャラクター人数
                ["character_count"] = settings.StoryCharacterCount.GetIntValue().ToString()
            };

            // キャラクター1～5
            int charCount = settings.StoryCharacterCount.GetIntValue();
            for (int i = 0; i < 5; i++)
            {
                string prefix = $"character_{i + 1}";
                if (i < charCount)
                {
                    var character = settings.StoryCharacters[i];
                    variables[$"{prefix}_image"] = YamlUtilities.GetFileName(character.ImagePath);
                    variables[$"{prefix}_expression"] = character.Expression ?? "";
                    variables[$"{prefix}_traits"] = character.Traits ?? "";
                    variables[$"{prefix}_dialogue"] = settings.StoryDialogues[i] ?? "";
                }
                else
                {
                    // 使用しないキャラクター枠は空文字
                    variables[$"{prefix}_image"] = "";
                    variables[$"{prefix}_expression"] = "";
                    variables[$"{prefix}_traits"] = "";
                    variables[$"{prefix}_dialogue"] = "";
                }
            }

            // ナレーション
            variables["narration"] = YamlUtilities.ConvertNewlinesToComma(settings.StoryNarration);
            variables["narration_position"] = settings.StoryNarrationPosition.ToYamlValue();

            // 装飾テキストセクション
            variables["text_overlay_section"] = BuildTextOverlaySection(settings);

            return variables;
        }

        /// <summary>
        /// 雰囲気の値を取得（カスタム時はカスタム値を使用）
        /// </summary>
        private static string GetLightingMoodValue(SceneBuilderSettingsViewModel settings)
        {
            if (settings.StoryLightingMood == LightingMood.Custom &&
                !string.IsNullOrWhiteSpace(settings.StoryCustomMood))
            {
                return settings.StoryCustomMood;
            }
            return settings.StoryLightingMood.ToYamlValue();
        }

        /// <summary>
        /// 配置タイプの値を取得（カスタム時はカスタム値を使用）
        /// </summary>
        private static string GetLayoutTypeValue(SceneBuilderSettingsViewModel settings)
        {
            if (settings.StoryLayout == StoryLayout.Custom &&
                !string.IsNullOrWhiteSpace(settings.StoryCustomLayout))
            {
                return settings.StoryCustomLayout;
            }
            return settings.StoryLayout.ToYamlValue();
        }

        /// <summary>
        /// 装飾テキストセクションを構築
        /// </summary>
        private static string BuildTextOverlaySection(SceneBuilderSettingsViewModel settings)
        {
            if (settings.TextOverlayItems.Count == 0)
            {
                return "";
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("  # 装飾テキストオーバーレイ");
            sb.AppendLine("  text_overlays:");

            for (int i = 0; i < settings.TextOverlayItems.Count; i++)
            {
                var item = settings.TextOverlayItems[i];
                sb.AppendLine($"    - image: \"{YamlUtilities.GetFileName(item.ImagePath)}\"");
                sb.AppendLine($"      position: \"{item.Position}\"");
                sb.AppendLine($"      size: \"{item.Size}\"");
                sb.AppendLine($"      layer: \"{item.Layer.ToYamlValue()}\"");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 背景生成YAML生成
        /// </summary>
        private string GenerateBackgroundYaml(MainViewModel mainViewModel)
        {
            var settings = mainViewModel.BackgroundSettings;
            if (settings == null || !settings.HasSettings)
            {
                return "# Error: 背景生成の設定がありません\n# 詳細設定ボタンから設定を入力してください";
            }

            var variables = BuildBackgroundVariables(mainViewModel, settings);
            return _templateEngine.Render("06_background.yaml", variables);
        }

        /// <summary>
        /// 背景生成用の変数辞書を構築
        /// </summary>
        private Dictionary<string, string> BuildBackgroundVariables(
            MainViewModel mainViewModel,
            BackgroundSettingsViewModel settings)
        {
            var authorName = mainViewModel.AuthorName?.Trim() ?? "";
            var titleOverlayEnabled = mainViewModel.IncludeTitleInImage;
            var (titlePosition, titleSize, authorPosition, authorSize) =
                GetTitleOverlayPositions(titleOverlayEnabled, !string.IsNullOrEmpty(authorName));

            // 参考画像モードで変換指示が空の場合はデフォルト値を使用
            var description = settings.Description;
            if (settings.UseReferenceImage && string.IsNullOrWhiteSpace(description))
            {
                description = "アニメ調に変換";
            }

            return new Dictionary<string, string>
            {
                // ヘッダーパーシャル用
                ["header_comment"] = "Background Generation (背景生成)",
                ["type"] = "background",
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

                // 背景生成固有
                ["use_reference_image"] = settings.UseReferenceImage ? "true" : "false",
                ["reference_image"] = YamlUtilities.GetFileName(settings.ReferenceImagePath),
                ["remove_people"] = settings.RemoveCharacters ? "true" : "false",
                ["description"] = YamlUtilities.ConvertNewlinesToComma(description)
            };
        }

        /// <summary>
        /// 装飾テキストYAML生成
        /// </summary>
        private string GenerateDecorativeTextYaml(MainViewModel mainViewModel)
        {
            var settings = mainViewModel.DecorativeTextSettings;
            if (settings == null || !settings.HasSettings)
            {
                return "# Error: 装飾テキストの設定がありません\n# 詳細設定ボタンから設定を入力してください";
            }

            var variables = BuildDecorativeTextVariables(mainViewModel, settings);
            return _templateEngine.Render("07_decorative_text.yaml", variables);
        }

        /// <summary>
        /// 装飾テキスト用の変数辞書を構築
        /// </summary>
        private Dictionary<string, string> BuildDecorativeTextVariables(
            MainViewModel mainViewModel,
            DecorativeTextSettingsViewModel settings)
        {
            var authorName = mainViewModel.AuthorName?.Trim() ?? "";
            var titleOverlayEnabled = mainViewModel.IncludeTitleInImage;
            var (titlePosition, titleSize, authorPosition, authorSize) =
                GetTitleOverlayPositions(titleOverlayEnabled, !string.IsNullOrEmpty(authorName));

            var variables = new Dictionary<string, string>
            {
                // ヘッダーパーシャル用
                ["header_comment"] = "Decorative Text (装飾テキスト)",
                ["type"] = "decorative_text",
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

                // 共通
                ["decorative_type"] = settings.TextType.ToYamlValue(),
                ["transparent_background"] = settings.TransparentBackground ? "true" : "false",
                ["text"] = settings.Text ?? "",
                ["background"] = settings.TransparentBackground ? "Transparent" : "White",
                ["ui_preset"] = settings.TextType.GetUiPreset()
            };

            // テキストタイプに応じたセクションを動的に構築
            variables["type_specific_section"] = BuildTypeSpecificSection(settings);
            variables["type_specific_constraints"] = BuildTypeSpecificConstraints(settings);

            return variables;
        }

        /// <summary>
        /// テキストタイプに応じた入力セクションを構築
        /// </summary>
        private static string BuildTypeSpecificSection(DecorativeTextSettingsViewModel settings)
        {
            var sb = new System.Text.StringBuilder();

            switch (settings.TextType)
            {
                case DecorativeTextType.SkillName:
                    sb.AppendLine("# ====================================================");
                    sb.AppendLine("# Input - Skill (技名テロップ)");
                    sb.AppendLine("# ====================================================");
                    sb.AppendLine("input_skill:");
                    sb.AppendLine($"  font_type: \"{settings.TitleFont.ToYamlValue()}\"");
                    sb.AppendLine($"  size: \"{settings.TitleSize.ToYamlValue()}\"");
                    sb.AppendLine($"  fill_color: \"{settings.TitleColor.ToYamlValue()}\"");
                    sb.AppendLine($"  outline_enabled: {(settings.TitleOutline != OutlineColor.None ? "true" : "false")}");
                    sb.AppendLine($"  outline_color: \"{settings.TitleOutline.ToYamlValue()}\"");
                    sb.AppendLine($"  outline_thickness: \"thick\"");
                    sb.AppendLine($"  glow_effect: \"{settings.TitleGlow.ToYamlValue()}\"");
                    break;

                case DecorativeTextType.Catchphrase:
                    sb.AppendLine("# ====================================================");
                    sb.AppendLine("# Input - Catchphrase (決め台詞)");
                    sb.AppendLine("# ====================================================");
                    sb.AppendLine("input_catchphrase:");
                    sb.AppendLine($"  type: \"{settings.CalloutType.ToYamlValue()}\"");
                    sb.AppendLine($"  color: \"{settings.CalloutColor.ToYamlValue()}\"");
                    sb.AppendLine($"  rotation: \"{settings.CalloutRotation.ToYamlValue()}\"");
                    sb.AppendLine($"  distortion: \"{settings.CalloutDistortion.ToYamlValue()}\"");
                    break;

                case DecorativeTextType.NamePlate:
                    sb.AppendLine("# ====================================================");
                    sb.AppendLine("# Input - Nameplate (キャラ名プレート)");
                    sb.AppendLine("# ====================================================");
                    sb.AppendLine("input_nameplate:");
                    sb.AppendLine($"  design_type: \"{settings.NameTagDesign.ToYamlValue()}\"");
                    sb.AppendLine($"  rotation: \"{settings.NameTagRotation.ToYamlValue()}\"");
                    break;

                case DecorativeTextType.MessageWindow:
                    sb.AppendLine("# ====================================================");
                    sb.AppendLine("# Input - Message (メッセージウィンドウ)");
                    sb.AppendLine("# ====================================================");
                    sb.AppendLine("input_message:");
                    sb.AppendLine($"  mode: \"{settings.MessageMode.ToYamlValue()}\"");
                    sb.AppendLine($"  speaker_name: \"{settings.SpeakerName ?? ""}\"");
                    sb.AppendLine($"  style_preset: \"{settings.MessageStyle.ToYamlValue()}\"");
                    sb.AppendLine($"  position: \"bottom\"");
                    sb.AppendLine($"  width: \"full\"");
                    sb.AppendLine($"  frame_type: \"{settings.MessageFrameType.ToYamlValue()}\"");
                    sb.AppendLine($"  background_opacity: {settings.MessageOpacity.ToString("F1", System.Globalization.CultureInfo.InvariantCulture)}");
                    sb.AppendLine($"  face_icon_enabled: {(settings.FaceIconPosition != FaceIconPosition.None ? "true" : "false")}");
                    sb.AppendLine($"  face_icon_source: \"{YamlUtilities.GetFileName(settings.FaceIconImagePath)}\"");
                    sb.AppendLine($"  face_icon_position: \"{settings.FaceIconPosition.ToYamlValue()}\"");
                    break;
            }

            sb.AppendLine();
            return sb.ToString();
        }

        /// <summary>
        /// テキストタイプに応じた制約セクションを構築
        /// </summary>
        private static string BuildTypeSpecificConstraints(DecorativeTextSettingsViewModel settings)
        {
            var sb = new System.Text.StringBuilder();

            switch (settings.TextType)
            {
                case DecorativeTextType.NamePlate:
                    sb.AppendLine("  nameplate:");
                    sb.AppendLine("    - \"Generate ONLY the name plate/tag element\"");
                    sb.AppendLine("    - \"Do NOT add any game UI elements (health bars, meters, VS logos)\"");
                    sb.AppendLine("    - \"Do NOT add any fighting game or battle interface elements\"");
                    break;

                case DecorativeTextType.MessageWindow:
                    sb.AppendLine("  message:");
                    sb.AppendLine("    - \"Generate ONLY the message window UI element\"");
                    sb.AppendLine("    - \"Do NOT draw any full-body character in the scene\"");
                    sb.AppendLine("    - \"Do NOT include any character outside the message window\"");
                    sb.AppendLine("    - \"The reference image is ONLY for the face icon, not for adding a character to the scene\"");
                    break;
            }

            return sb.ToString();
        }

        /// <summary>
        /// 4コマ漫画YAML生成
        /// </summary>
        private string GenerateFourPanelMangaYaml(MainViewModel mainViewModel)
        {
            var settings = mainViewModel.FourPanelMangaSettings;
            if (settings == null || !settings.HasSettings)
            {
                return "# Error: 4コマ漫画の設定がありません\n# 詳細設定ボタンから設定を入力してください";
            }

            var variables = BuildFourPanelMangaVariables(mainViewModel, settings);
            return _templateEngine.Render("08_four_panel.yaml", variables);
        }

        /// <summary>
        /// 4コマ漫画用の変数辞書を構築
        /// </summary>
        private Dictionary<string, string> BuildFourPanelMangaVariables(
            MainViewModel mainViewModel,
            FourPanelMangaSettingsViewModel settings)
        {
            var authorName = mainViewModel.AuthorName?.Trim() ?? "";
            var titleOverlayEnabled = mainViewModel.IncludeTitleInImage;
            var (titlePosition, titleSize, authorPosition, authorSize) =
                GetTitleOverlayPositions(titleOverlayEnabled, !string.IsNullOrEmpty(authorName));

            var variables = new Dictionary<string, string>
            {
                // ヘッダーパーシャル用
                ["header_comment"] = "Four Panel Manga (4コマ漫画)",
                ["type"] = "four_panel_manga",
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

                // キャラクター
                ["character_1_name"] = settings.Character1Name ?? "",
                ["character_1_reference"] = YamlUtilities.GetFileName(settings.Character1ImagePath),
                ["character_1_description"] = settings.Character1Description ?? "",
                ["character_2_name"] = settings.Character2Name ?? "",
                ["character_2_reference"] = YamlUtilities.GetFileName(settings.Character2ImagePath),
                ["character_2_description"] = settings.Character2Description ?? ""
            };

            // パネル変数を追加
            AddPanelVariables(variables, settings);

            return variables;
        }

        /// <summary>
        /// パネル変数を追加
        /// </summary>
        private static void AddPanelVariables(
            Dictionary<string, string> variables,
            FourPanelMangaSettingsViewModel settings)
        {
            var char1Name = settings.Character1Name ?? "";
            var char2Name = settings.Character2Name ?? "";

            for (int i = 0; i < 4; i++)
            {
                var panel = settings.Panels[i];
                var panelNum = i + 1;
                var prefix = $"panel_{panelNum}";

                variables[$"{prefix}_prompt"] = panel.Scene ?? "";
                variables[$"{prefix}_narration"] = panel.Narration ?? "";

                // セリフ1
                variables[$"{prefix}_speech1_character"] = panel.Speech1Char.ToCharacterName(char1Name, char2Name);
                variables[$"{prefix}_speech1_content"] = panel.Speech1Text ?? "";
                variables[$"{prefix}_speech1_position"] = panel.Speech1Position.ToYamlValue();

                // セリフ2
                variables[$"{prefix}_speech2_character"] = panel.Speech2Char.ToCharacterName(char1Name, char2Name);
                variables[$"{prefix}_speech2_content"] = panel.Speech2Text ?? "";
                variables[$"{prefix}_speech2_position"] = panel.Speech2Position.ToYamlValue();
            }
        }

        /// <summary>
        /// スタイル変換YAML生成
        /// </summary>
        private string GenerateStyleTransformYaml(MainViewModel mainViewModel)
        {
            var settings = mainViewModel.StyleTransformSettings;
            if (settings == null || !settings.HasSettings)
            {
                return "# Error: スタイル変換の設定がありません\n# 詳細設定ボタンから設定を入力してください";
            }

            var variables = BuildStyleTransformVariables(mainViewModel, settings);
            return _templateEngine.Render("09_style_transform.yaml", variables);
        }

        /// <summary>
        /// スタイル変換用の変数辞書を構築
        /// </summary>
        private Dictionary<string, string> BuildStyleTransformVariables(
            MainViewModel mainViewModel,
            StyleTransformSettingsViewModel settings)
        {
            var authorName = mainViewModel.AuthorName?.Trim() ?? "";
            var titleOverlayEnabled = mainViewModel.IncludeTitleInImage;
            var (titlePosition, titleSize, authorPosition, authorSize) =
                GetTitleOverlayPositions(titleOverlayEnabled, !string.IsNullOrEmpty(authorName));

            return new Dictionary<string, string>
            {
                // ヘッダーパーシャル用
                ["header_comment"] = "Style Transform (スタイル変換)",
                ["type"] = "style_transform",
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

                // スタイル変換固有
                ["source_image"] = YamlUtilities.GetFileName(settings.SourceImagePath),
                ["style_type"] = settings.TransformType.ToYamlValue(),
                ["transparent_background"] = settings.TransparentBackground ? "true" : "false",
                ["background"] = settings.TransparentBackground ? "transparent" : "white",

                // タイプ別セクション（動的生成）
                ["type_specific_input"] = BuildStyleTransformInputSection(settings),
                ["type_specific_constraints"] = BuildStyleTransformConstraintsSection(settings),
                ["type_specific_anti_hallucination"] = BuildStyleTransformAntiHallucinationSection(settings)
            };
        }

        /// <summary>
        /// スタイル変換のタイプ別入力セクションを構築
        /// </summary>
        private static string BuildStyleTransformInputSection(StyleTransformSettingsViewModel settings)
        {
            var sb = new System.Text.StringBuilder();

            if (settings.TransformType == StyleTransformType.Chibi)
            {
                sb.AppendLine("# ====================================================");
                sb.AppendLine("# Input - Chibi");
                sb.AppendLine("# ====================================================");
                sb.AppendLine("input_chibi:");
                sb.AppendLine($"  style: \"{settings.ChibiStyle.GetPrompt()}\"");
            }
            else
            {
                sb.AppendLine("# ====================================================");
                sb.AppendLine("# Input - Pixel Art");
                sb.AppendLine("# ====================================================");
                sb.AppendLine("input_pixel:");
                sb.AppendLine($"  style: \"{settings.PixelStyle.GetPrompt()}\"");
                sb.AppendLine($"  sprite_size: \"{settings.SpriteSize.GetPrompt()}\"");
            }

            sb.AppendLine();
            return sb.ToString();
        }

        /// <summary>
        /// スタイル変換のタイプ別制約セクションを構築
        /// </summary>
        private static string BuildStyleTransformConstraintsSection(StyleTransformSettingsViewModel settings)
        {
            var sb = new System.Text.StringBuilder();

            if (settings.TransformType == StyleTransformType.Chibi)
            {
                sb.AppendLine("# ====================================================");
                sb.AppendLine("# Constraints - Chibi Only (CRITICAL)");
                sb.AppendLine("# ====================================================");
                sb.AppendLine("constraints_chibi:");
                sb.AppendLine("  - \"Transform to chibi style\"");
                sb.AppendLine("  - \"Large head, small body, simplified features\"");
                sb.AppendLine("  - \"Keep the cuteness and appeal of chibi style\"");
                sb.AppendLine("  - \"Use consistent chibi proportions throughout\"");
                sb.AppendLine("  - \"Clean, cute linework suitable for chibi style\"");
            }
            else
            {
                sb.AppendLine("# ====================================================");
                sb.AppendLine("# Constraints - Pixel Art Only (CRITICAL)");
                sb.AppendLine("# ====================================================");
                sb.AppendLine("constraints_pixel:");
                sb.AppendLine("  - \"Convert to pixel art style\"");
                sb.AppendLine("  - \"Clean, sharp pixels with no anti-aliasing blur\"");
                sb.AppendLine("  - \"Limited color palette appropriate for pixel art\"");
                sb.AppendLine("  - \"Consistent pixel size throughout the sprite\"");
                sb.AppendLine("  - \"Game sprite aesthetic, suitable for game use\"");
            }

            sb.AppendLine();
            return sb.ToString();
        }

        /// <summary>
        /// スタイル変換のタイプ別Anti-Hallucinationセクションを構築
        /// </summary>
        private static string BuildStyleTransformAntiHallucinationSection(StyleTransformSettingsViewModel settings)
        {
            var sb = new System.Text.StringBuilder();

            if (settings.TransformType == StyleTransformType.Chibi)
            {
                sb.AppendLine("# ====================================================");
                sb.AppendLine("# Anti-Hallucination - Chibi Only (MUST FOLLOW)");
                sb.AppendLine("# ====================================================");
                sb.AppendLine("anti_hallucination_chibi:");
                sb.AppendLine("  - \"MAINTAIN chibi proportions consistently\"");
            }
            else
            {
                sb.AppendLine("# ====================================================");
                sb.AppendLine("# Anti-Hallucination - Pixel Art Only (MUST FOLLOW)");
                sb.AppendLine("# ====================================================");
                sb.AppendLine("anti_hallucination_pixel:");
                sb.AppendLine("  - \"Do NOT add pixel art artifacts or noise\"");
                sb.AppendLine("  - \"Do NOT blur or anti-alias the pixels\"");
                sb.AppendLine("  - \"MAINTAIN consistent pixel grid\"");
            }

            sb.AppendLine();
            return sb.ToString();
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
