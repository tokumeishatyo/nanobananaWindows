// rule.mdを読むこと
using System;

namespace nanobananaWindows.Models
{
    // ============================================================
    // アプリ基本定数
    // 出力タイプ、出力モード、解像度、アプリ設定など
    // プルダウン選択肢は DropdownOptions.cs を参照
    // ============================================================

    /// <summary>
    /// 出力タイプ定義
    /// </summary>
    public enum OutputType
    {
        FaceSheet,      // 顔三面図
        BodySheet,      // 素体三面図
        Outfit,         // 衣装着用
        Pose,           // ポーズ
        SceneBuilder,   // シーンビルダー
        Background,     // 背景生成
        DecorativeText, // 装飾テキスト
        FourPanelManga, // 4コマ漫画
        StyleTransform, // スタイル変換
        Infographic     // インフォグラフィック
    }

    /// <summary>
    /// OutputType の拡張メソッド
    /// </summary>
    public static class OutputTypeExtensions
    {
        public static string GetDisplayName(this OutputType type)
        {
            return type switch
            {
                OutputType.FaceSheet => "顔三面図",
                OutputType.BodySheet => "素体三面図",
                OutputType.Outfit => "衣装着用",
                OutputType.Pose => "ポーズ",
                OutputType.SceneBuilder => "シーンビルダー",
                OutputType.Background => "背景生成",
                OutputType.DecorativeText => "装飾テキスト",
                OutputType.FourPanelManga => "4コマ漫画",
                OutputType.StyleTransform => "スタイル変換",
                OutputType.Infographic => "インフォグラフィック",
                _ => type.ToString()
            };
        }

        public static string GetInternalKey(this OutputType type)
        {
            return type switch
            {
                OutputType.FaceSheet => "step1_face",
                OutputType.BodySheet => "step2_body",
                OutputType.Outfit => "step3_outfit",
                OutputType.Pose => "step4_pose",
                OutputType.SceneBuilder => "scene_builder",
                OutputType.Background => "background",
                OutputType.DecorativeText => "decorative_text",
                OutputType.FourPanelManga => "four_panel_manga",
                OutputType.StyleTransform => "style_transform",
                OutputType.Infographic => "infographic",
                _ => type.ToString().ToLower()
            };
        }
    }

    /// <summary>
    /// 出力モード
    /// </summary>
    public enum OutputMode
    {
        Yaml,   // YAML出力
        Api     // 画像出力(API)
    }

    public static class OutputModeExtensions
    {
        public static string GetDisplayName(this OutputMode mode)
        {
            return mode switch
            {
                OutputMode.Yaml => "YAML出力",
                OutputMode.Api => "画像出力(API)",
                _ => mode.ToString()
            };
        }
    }

    /// <summary>
    /// APIサブモード
    /// </summary>
    public enum ApiSubMode
    {
        Normal, // 通常
        Redraw, // 参考画像清書
        Simple  // シンプル
    }

    public static class ApiSubModeExtensions
    {
        public static string GetDisplayName(this ApiSubMode mode)
        {
            return mode switch
            {
                ApiSubMode.Normal => "通常",
                ApiSubMode.Redraw => "参考画像清書",
                ApiSubMode.Simple => "シンプル",
                _ => mode.ToString()
            };
        }
    }

    /// <summary>
    /// 解像度
    /// </summary>
    public enum Resolution
    {
        OneK,   // 1K
        TwoK,   // 2K
        FourK   // 4K
    }

    public static class ResolutionExtensions
    {
        public static string GetDisplayName(this Resolution res)
        {
            return res switch
            {
                Resolution.OneK => "1K",
                Resolution.TwoK => "2K",
                Resolution.FourK => "4K",
                _ => res.ToString()
            };
        }
    }

    /// <summary>
    /// アプリ定数
    /// </summary>
    public static class AppConstants
    {
        public const int MaxSpeechLength = 30;
        public const int MaxRecentFiles = 5;
        public const int MaxCharacters = 5;

        public const double WindowMinWidth = 1720;
        public const double WindowMinHeight = 700;

        public const double LeftColumnWidth = 500;
        public const double MiddleColumnWidth = 500;
        public const double RightColumnWidth = 700;
    }
}
