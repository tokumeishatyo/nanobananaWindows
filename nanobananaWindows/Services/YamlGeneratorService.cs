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
                // 他の出力タイプは順次実装
                OutputType.BodySheet => GeneratePlaceholderYaml("素体三面図", "02_body_sheet.yaml"),
                OutputType.Outfit => GeneratePlaceholderYaml("衣装着用", "03_outfit.yaml"),
                OutputType.Pose => GeneratePlaceholderYaml("ポーズ", "04_pose.yaml"),
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
