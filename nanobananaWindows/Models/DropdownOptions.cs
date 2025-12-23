// rule.mdを読むこと
using System;

namespace nanobananaWindows.Models
{
    // ============================================================
    // プルダウン選択肢定義
    // 各設定画面のPickerで使用する選択肢を集約
    // ============================================================

    // MARK: - メイン画面関連

    /// <summary>
    /// カラーモード定義
    /// </summary>
    public enum ColorMode
    {
        FullColor,  // フルカラー
        Monochrome, // モノクロ
        Sepia,      // セピア色
        Duotone     // 二色刷り
    }

    public static class ColorModeExtensions
    {
        public static string GetDisplayName(this ColorMode mode)
        {
            return mode switch
            {
                ColorMode.FullColor => "フルカラー",
                ColorMode.Monochrome => "モノクロ",
                ColorMode.Sepia => "セピア色",
                ColorMode.Duotone => "二色刷り",
                _ => mode.ToString()
            };
        }

        public static string GetPrompt(this ColorMode mode)
        {
            return mode switch
            {
                ColorMode.FullColor => "",
                ColorMode.Monochrome => "monochrome, black and white, grayscale",
                ColorMode.Sepia => "sepia tone, vintage brown tint, old photograph style",
                ColorMode.Duotone => "", // Combined with DuotoneColor
                _ => ""
            };
        }
    }

    /// <summary>
    /// 二色刷りカラー
    /// </summary>
    public enum DuotoneColor
    {
        RedBlack,    // 赤×黒
        BlueBlack,   // 青×黒
        GreenBlack,  // 緑×黒
        PurpleBlack, // 紫×黒
        OrangeBlack  // オレンジ×黒
    }

    public static class DuotoneColorExtensions
    {
        public static string GetDisplayName(this DuotoneColor color)
        {
            return color switch
            {
                DuotoneColor.RedBlack => "赤×黒",
                DuotoneColor.BlueBlack => "青×黒",
                DuotoneColor.GreenBlack => "緑×黒",
                DuotoneColor.PurpleBlack => "紫×黒",
                DuotoneColor.OrangeBlack => "オレンジ×黒",
                _ => color.ToString()
            };
        }

        public static string GetPrompt(this DuotoneColor color)
        {
            return color switch
            {
                DuotoneColor.RedBlack => "red and black duotone, two-color print, manga style",
                DuotoneColor.BlueBlack => "blue and black duotone, two-color print",
                DuotoneColor.GreenBlack => "green and black duotone, two-color print",
                DuotoneColor.PurpleBlack => "purple and black duotone, two-color print",
                DuotoneColor.OrangeBlack => "orange and black duotone, two-color print",
                _ => ""
            };
        }
    }

    /// <summary>
    /// 出力スタイル定義
    /// </summary>
    public enum OutputStyle
    {
        Anime,      // アニメ調
        PixelArt,   // ドット絵
        Chibi,      // ちびキャラ
        Realistic,  // リアル調
        Watercolor, // 水彩画風
        OilPainting // 油絵風
    }

    public static class OutputStyleExtensions
    {
        public static string GetDisplayName(this OutputStyle style)
        {
            return style switch
            {
                OutputStyle.Anime => "アニメ調",
                OutputStyle.PixelArt => "ドット絵",
                OutputStyle.Chibi => "ちびキャラ",
                OutputStyle.Realistic => "リアル調",
                OutputStyle.Watercolor => "水彩画風",
                OutputStyle.OilPainting => "油絵風",
                _ => style.ToString()
            };
        }
    }

    /// <summary>
    /// アスペクト比定義
    /// </summary>
    public enum AspectRatio
    {
        Square,      // 1:1（正方形）
        Wide16_9,    // 16:9
        Tall9_16,    // 9:16
        Standard4_3, // 4:3
        Portrait3_4, // 3:4
        UltraWide3_1 // 3:1
    }

    public static class AspectRatioExtensions
    {
        public static string GetDisplayName(this AspectRatio ratio)
        {
            return ratio switch
            {
                AspectRatio.Square => "1:1（正方形）",
                AspectRatio.Wide16_9 => "16:9",
                AspectRatio.Tall9_16 => "9:16",
                AspectRatio.Standard4_3 => "4:3",
                AspectRatio.Portrait3_4 => "3:4",
                AspectRatio.UltraWide3_1 => "3:1",
                _ => ratio.ToString()
            };
        }

        public static string GetYamlValue(this AspectRatio ratio)
        {
            return ratio switch
            {
                AspectRatio.Square => "1:1",
                AspectRatio.Wide16_9 => "16:9",
                AspectRatio.Tall9_16 => "9:16",
                AspectRatio.Standard4_3 => "4:3",
                AspectRatio.Portrait3_4 => "3:4",
                AspectRatio.UltraWide3_1 => "3:1",
                _ => "1:1"
            };
        }

        public static (int Width, int Height) GetRatio(this AspectRatio ratio)
        {
            return ratio switch
            {
                AspectRatio.Square => (1, 1),
                AspectRatio.Wide16_9 => (16, 9),
                AspectRatio.Tall9_16 => (9, 16),
                AspectRatio.Standard4_3 => (4, 3),
                AspectRatio.Portrait3_4 => (3, 4),
                AspectRatio.UltraWide3_1 => (3, 1),
                _ => (1, 1)
            };
        }
    }

    // ============================================================
    // 素体三面図（Step2）関連
    // ============================================================

    /// <summary>
    /// 体型プリセット
    /// </summary>
    public enum BodyTypePreset
    {
        FemaleStandard, // 標準体型（女性）
        MaleStandard,   // 標準体型（男性）
        Slim,           // スリム体型
        Muscular,       // 筋肉質
        Chubby,         // ぽっちゃり
        Petite,         // 幼児体型
        Tall,           // 高身長
        Short           // 低身長
    }

    public static class BodyTypePresetExtensions
    {
        public static string GetDisplayName(this BodyTypePreset preset)
        {
            return preset switch
            {
                BodyTypePreset.FemaleStandard => "標準体型（女性）",
                BodyTypePreset.MaleStandard => "標準体型（男性）",
                BodyTypePreset.Slim => "スリム体型",
                BodyTypePreset.Muscular => "筋肉質",
                BodyTypePreset.Chubby => "ぽっちゃり",
                BodyTypePreset.Petite => "幼児体型",
                BodyTypePreset.Tall => "高身長",
                BodyTypePreset.Short => "低身長",
                _ => preset.ToString()
            };
        }
    }

    /// <summary>
    /// 素体表現タイプ
    /// </summary>
    public enum BodyRenderType
    {
        Silhouette,     // シルエット
        WhiteLeotard,   // 素体（白レオタード）
        WhiteUnderwear, // 素体（白下着）
        Anatomical      // 解剖学的
    }

    public static class BodyRenderTypeExtensions
    {
        public static string GetDisplayName(this BodyRenderType type)
        {
            return type switch
            {
                BodyRenderType.Silhouette => "シルエット",
                BodyRenderType.WhiteLeotard => "素体（白レオタード）",
                BodyRenderType.WhiteUnderwear => "素体（白下着）",
                BodyRenderType.Anatomical => "解剖学的",
                _ => type.ToString()
            };
        }
    }

    /// <summary>
    /// バスト特徴
    /// </summary>
    public enum BustFeature
    {
        Auto,   // おまかせ
        Small,  // 控えめ
        Normal, // 標準
        Large   // 豊か
    }

    public static class BustFeatureExtensions
    {
        public static string GetDisplayName(this BustFeature feature)
        {
            return feature switch
            {
                BustFeature.Auto => "おまかせ",
                BustFeature.Small => "控えめ",
                BustFeature.Normal => "標準",
                BustFeature.Large => "豊か",
                _ => feature.ToString()
            };
        }
    }
}
