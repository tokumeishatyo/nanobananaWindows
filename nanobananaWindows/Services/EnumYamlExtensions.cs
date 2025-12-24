// rule.mdを読むこと
using nanobananaWindows.Models;
using nanobananaWindows.ViewModels;

namespace nanobananaWindows.Services
{
    /// <summary>
    /// enum型のYAML値変換拡張メソッド
    /// </summary>
    public static class EnumYamlExtensions
    {
        // ============================================================
        // メイン画面関連
        // ============================================================

        /// <summary>
        /// ColorModeのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this ColorMode colorMode)
        {
            return colorMode switch
            {
                ColorMode.FullColor => "fullcolor",
                ColorMode.Monochrome => "monochrome",
                ColorMode.Sepia => "sepia",
                ColorMode.Duotone => "duotone",
                _ => "fullcolor"
            };
        }

        /// <summary>
        /// OutputStyleのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this OutputStyle outputStyle)
        {
            return outputStyle switch
            {
                OutputStyle.Anime => "anime",
                OutputStyle.PixelArt => "pixel_art",
                OutputStyle.Chibi => "chibi",
                OutputStyle.Realistic => "realistic",
                OutputStyle.Watercolor => "watercolor",
                OutputStyle.OilPainting => "oil_painting",
                _ => "anime"
            };
        }

        /// <summary>
        /// AspectRatioのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this AspectRatio aspectRatio)
        {
            return aspectRatio switch
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

        // ============================================================
        // 素体三面図（Step2）関連
        // ============================================================

        /// <summary>
        /// BodyTypePresetのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this BodyTypePreset preset)
        {
            return preset switch
            {
                BodyTypePreset.FemaleStandard => "female_standard",
                BodyTypePreset.MaleStandard => "male_standard",
                BodyTypePreset.Slim => "slim",
                BodyTypePreset.Muscular => "muscular",
                BodyTypePreset.Chubby => "chubby",
                BodyTypePreset.Petite => "petite",
                BodyTypePreset.Tall => "tall",
                BodyTypePreset.Short => "short",
                _ => "female_standard"
            };
        }

        /// <summary>
        /// BustFeatureのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this BustFeature feature)
        {
            return feature switch
            {
                BustFeature.Auto => "auto",
                BustFeature.Small => "small",
                BustFeature.Normal => "normal",
                BustFeature.Large => "large",
                _ => "auto"
            };
        }

        /// <summary>
        /// BodyRenderTypeのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this BodyRenderType type)
        {
            return type switch
            {
                BodyRenderType.Silhouette => "silhouette",
                BodyRenderType.WhiteLeotard => "white_leotard",
                BodyRenderType.WhiteUnderwear => "white_underwear",
                BodyRenderType.Anatomical => "anatomical",
                _ => "silhouette"
            };
        }

        // ============================================================
        // 衣装着用（Step3）関連
        // ============================================================

        /// <summary>
        /// OutfitCategoryのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this OutfitCategory category)
        {
            return category switch
            {
                OutfitCategory.Auto => "auto",
                OutfitCategory.Model => "model",
                OutfitCategory.Suit => "suit",
                OutfitCategory.Swimsuit => "swimsuit",
                OutfitCategory.Casual => "casual",
                OutfitCategory.Uniform => "uniform",
                OutfitCategory.Formal => "formal",
                OutfitCategory.Sports => "sports",
                OutfitCategory.Japanese => "japanese",
                OutfitCategory.Workwear => "workwear",
                _ => "auto"
            };
        }

        /// <summary>
        /// OutfitColorのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this OutfitColor color)
        {
            return color switch
            {
                OutfitColor.Auto => "auto",
                OutfitColor.Black => "black",
                OutfitColor.White => "white",
                OutfitColor.Navy => "navy",
                OutfitColor.Red => "red",
                OutfitColor.Pink => "pink",
                OutfitColor.Blue => "blue",
                OutfitColor.LightBlue => "light_blue",
                OutfitColor.Green => "green",
                OutfitColor.Yellow => "yellow",
                OutfitColor.Orange => "orange",
                OutfitColor.Purple => "purple",
                OutfitColor.Beige => "beige",
                OutfitColor.Gray => "gray",
                OutfitColor.Gold => "gold",
                OutfitColor.Silver => "silver",
                _ => "auto"
            };
        }

        /// <summary>
        /// OutfitPatternのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this OutfitPattern pattern)
        {
            return pattern switch
            {
                OutfitPattern.Auto => "auto",
                OutfitPattern.Solid => "solid",
                OutfitPattern.Stripe => "stripe",
                OutfitPattern.Check => "check",
                OutfitPattern.Floral => "floral",
                OutfitPattern.Dot => "dot",
                OutfitPattern.Border => "border",
                OutfitPattern.Tropical => "tropical",
                OutfitPattern.Lace => "lace",
                OutfitPattern.Camouflage => "camouflage",
                OutfitPattern.Animal => "animal",
                _ => "auto"
            };
        }

        /// <summary>
        /// OutfitFashionStyleのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this OutfitFashionStyle style)
        {
            return style switch
            {
                OutfitFashionStyle.Auto => "auto",
                OutfitFashionStyle.Mature => "mature",
                OutfitFashionStyle.Cute => "cute",
                OutfitFashionStyle.Sexy => "sexy",
                OutfitFashionStyle.Cool => "cool",
                OutfitFashionStyle.Modest => "modest",
                OutfitFashionStyle.Sporty => "sporty",
                OutfitFashionStyle.Gorgeous => "gorgeous",
                OutfitFashionStyle.Wild => "wild",
                OutfitFashionStyle.Intellectual => "intellectual",
                OutfitFashionStyle.Dandy => "dandy",
                OutfitFashionStyle.Casual => "casual",
                _ => "auto"
            };
        }

        /// <summary>
        /// FitModeのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this FitMode mode)
        {
            return mode switch
            {
                FitMode.BodyPriority => "body_priority",
                FitMode.OutfitPriority => "outfit_priority",
                FitMode.Hybrid => "hybrid",
                _ => "body_priority"
            };
        }

        // ============================================================
        // ポーズ（Step4）関連
        // ============================================================

        /// <summary>
        /// EyeLineのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this EyeLine eyeLine)
        {
            return eyeLine switch
            {
                EyeLine.Front => "looking straight ahead",
                EyeLine.Up => "looking up",
                EyeLine.Down => "looking down",
                _ => "looking straight ahead"
            };
        }

        // ============================================================
        // シーンビルダー関連
        // ============================================================

        /// <summary>
        /// BackgroundSourceTypeのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this BackgroundSourceType type)
        {
            return type switch
            {
                BackgroundSourceType.File => "file",
                BackgroundSourceType.Prompt => "prompt",
                _ => "file"
            };
        }

        // ============================================================
        // 装飾テキスト関連
        // ============================================================

        /// <summary>
        /// DecorativeTextTypeのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this DecorativeTextType type)
        {
            return type switch
            {
                DecorativeTextType.SkillName => "skill",
                DecorativeTextType.Catchphrase => "catchphrase",
                DecorativeTextType.NamePlate => "nameplate",
                DecorativeTextType.MessageWindow => "message",
                _ => "skill"
            };
        }

        /// <summary>
        /// DecorativeTextTypeのUIプリセット値を取得
        /// </summary>
        public static string GetUiPreset(this DecorativeTextType type)
        {
            return type switch
            {
                DecorativeTextType.SkillName => "Anime Battle",
                DecorativeTextType.Catchphrase => "Anime Battle",
                DecorativeTextType.NamePlate => "Character Name Plate",
                DecorativeTextType.MessageWindow => "Message Window",
                _ => "Anime Battle"
            };
        }

        // ============================================================
        // スタイル変換関連
        // ============================================================

        /// <summary>
        /// StyleTransformTypeのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this StyleTransformType type)
        {
            return type switch
            {
                StyleTransformType.Chibi => "chibi",
                StyleTransformType.Pixel => "pixel",
                _ => "chibi"
            };
        }
    }
}
