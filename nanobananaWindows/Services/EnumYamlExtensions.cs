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

        /// <summary>
        /// PoseExpressionのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this PoseExpression expression)
        {
            return expression switch
            {
                PoseExpression.Neutral => "neutral",
                PoseExpression.Smile => "smile",
                PoseExpression.Angry => "angry",
                PoseExpression.Crying => "crying",
                PoseExpression.Shy => "shy",
                _ => "neutral"
            };
        }

        /// <summary>
        /// WindEffectのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this WindEffect effect)
        {
            return effect switch
            {
                WindEffect.None => "none",
                WindEffect.FromFront => "from_front",
                WindEffect.FromBehind => "from_behind",
                WindEffect.FromSide => "from_side",
                _ => "none"
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

        /// <summary>
        /// LightingMoodのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this LightingMood mood)
        {
            return mood switch
            {
                LightingMood.Morning => "Morning Sunlight",
                LightingMood.Sunset => "Sunset",
                LightingMood.SummerNoon => "Summer Noon",
                LightingMood.Night => "Night",
                LightingMood.Custom => "custom",
                _ => "Morning Sunlight"
            };
        }

        /// <summary>
        /// StoryLayoutのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this StoryLayout layout)
        {
            return layout switch
            {
                StoryLayout.SideBySide => "Side by Side (Walking)",
                StoryLayout.FaceToFace => "Face to Face (Table)",
                StoryLayout.CenterListener => "Center & Listener",
                StoryLayout.Custom => "custom",
                _ => "Side by Side (Walking)"
            };
        }

        /// <summary>
        /// StoryDistanceのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this StoryDistance distance)
        {
            return distance switch
            {
                StoryDistance.Close => "Close Friends",
                StoryDistance.Normal => "Normal",
                StoryDistance.Distant => "Distant",
                _ => "Close Friends"
            };
        }

        /// <summary>
        /// NarrationPositionのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this NarrationPosition position)
        {
            return position switch
            {
                NarrationPosition.TopHorizontal => "Top Horizontal",
                NarrationPosition.BottomHorizontal => "Bottom Horizontal",
                NarrationPosition.LeftVertical => "Left Vertical",
                NarrationPosition.RightVertical => "Right Vertical",
                NarrationPosition.Auto => "Auto",
                _ => "Auto"
            };
        }

        /// <summary>
        /// TextOverlayLayerのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this TextOverlayLayer layer)
        {
            return layer switch
            {
                TextOverlayLayer.Frontmost => "Frontmost (Above Characters)",
                TextOverlayLayer.BehindCharacters => "Behind Characters",
                TextOverlayLayer.AboveBackground => "Above Background Only",
                _ => "Frontmost (Above Characters)"
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

        /// <summary>
        /// TitleFontのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this TitleFont font)
        {
            return font switch
            {
                TitleFont.HeavyMincho => "Heavy Mincho",
                TitleFont.Brush => "Brush Script",
                TitleFont.Gothic => "Gothic Bold",
                _ => "Heavy Mincho"
            };
        }

        /// <summary>
        /// TitleSizeのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this TitleSize size)
        {
            return size switch
            {
                TitleSize.VeryLarge => "Extra Large",
                TitleSize.Large => "Large",
                TitleSize.Medium => "Medium",
                _ => "Extra Large"
            };
        }

        /// <summary>
        /// GradientColorのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this GradientColor color)
        {
            return color switch
            {
                GradientColor.WhiteToBlue => "White to Blue Gradient",
                GradientColor.WhiteToRed => "White to Red Gradient",
                GradientColor.GoldToOrange => "Gold to Orange Gradient",
                GradientColor.WhiteToPurple => "White to Purple Gradient",
                GradientColor.SolidWhite => "Solid White",
                GradientColor.SolidGold => "Solid Gold",
                _ => "White to Blue Gradient"
            };
        }

        /// <summary>
        /// OutlineColorのYAML値を取得（装飾テキスト用）
        /// </summary>
        public static string ToYamlValue(this OutlineColor color)
        {
            return color switch
            {
                OutlineColor.Gold => "Gold",
                OutlineColor.Black => "Black",
                OutlineColor.Red => "Red",
                OutlineColor.Blue => "Blue",
                OutlineColor.None => "None",
                _ => "Gold"
            };
        }

        /// <summary>
        /// GlowEffectのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this GlowEffect effect)
        {
            return effect switch
            {
                GlowEffect.None => "None",
                GlowEffect.BlueLightning => "Blue Lightning",
                GlowEffect.Fire => "Fire",
                GlowEffect.Electric => "Electric Spark",
                GlowEffect.Aura => "Aura Glow",
                _ => "None"
            };
        }

        /// <summary>
        /// CalloutTypeのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this CalloutType type)
        {
            return type switch
            {
                CalloutType.Comic => "Comic Style",
                CalloutType.VerticalShout => "Vertical Shout",
                CalloutType.Pop => "Pop Style",
                _ => "Comic Style"
            };
        }

        /// <summary>
        /// CalloutColorのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this CalloutColor color)
        {
            return color switch
            {
                CalloutColor.RedYellow => "Red with Yellow Outline",
                CalloutColor.WhiteBlack => "White with Black Outline",
                CalloutColor.BlueWhite => "Blue with White Outline",
                CalloutColor.YellowRed => "Yellow with Red Outline",
                _ => "Red with Yellow Outline"
            };
        }

        /// <summary>
        /// TextRotationのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this TextRotation rotation)
        {
            return rotation switch
            {
                TextRotation.None => "0",
                TextRotation.SlightLeft => "-5",
                TextRotation.Left => "-15",
                TextRotation.SlightRight => "5",
                TextRotation.Right => "15",
                _ => "0"
            };
        }

        /// <summary>
        /// TextDistortionのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this TextDistortion distortion)
        {
            return distortion switch
            {
                TextDistortion.None => "None",
                TextDistortion.ZoomIn => "Zoom In",
                TextDistortion.ZoomOut => "Zoom Out",
                TextDistortion.Wave => "Wave",
                _ => "None"
            };
        }

        /// <summary>
        /// NameTagDesignのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this NameTagDesign design)
        {
            return design switch
            {
                NameTagDesign.Jagged => "Jagged Sticker",
                NameTagDesign.Simple => "Simple Frame",
                NameTagDesign.Ribbon => "Ribbon Banner",
                _ => "Jagged Sticker"
            };
        }

        /// <summary>
        /// MessageWindowModeのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this MessageWindowMode mode)
        {
            return mode switch
            {
                MessageWindowMode.Full => "full",
                MessageWindowMode.FaceOnly => "face_only",
                MessageWindowMode.TextOnly => "text_only",
                _ => "full"
            };
        }

        /// <summary>
        /// MessageWindowStyleのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this MessageWindowStyle style)
        {
            return style switch
            {
                MessageWindowStyle.SciFi => "SciFi Robot",
                MessageWindowStyle.RetroRPG => "Retro RPG",
                MessageWindowStyle.VisualNovel => "Visual Novel",
                _ => "SciFi Robot"
            };
        }

        /// <summary>
        /// MessageFrameTypeのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this MessageFrameType frame)
        {
            return frame switch
            {
                MessageFrameType.CyberneticBlue => "Cybernetic Blue",
                MessageFrameType.ClassicBlack => "Classic Black",
                MessageFrameType.TranslucentWhite => "Translucent White",
                MessageFrameType.GoldOrnate => "Gold Ornate",
                _ => "Cybernetic Blue"
            };
        }

        /// <summary>
        /// FaceIconPositionのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this FaceIconPosition position)
        {
            return position switch
            {
                FaceIconPosition.LeftInside => "Left Inside",
                FaceIconPosition.RightInside => "Right Inside",
                FaceIconPosition.LeftOutside => "Left Outside",
                FaceIconPosition.None => "None",
                _ => "Left Inside"
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

        // ============================================================
        // 4コマ漫画関連
        // ============================================================

        /// <summary>
        /// SpeechCharacterをキャラクター名に変換
        /// </summary>
        public static string ToCharacterName(this SpeechCharacter character, string char1Name, string char2Name)
        {
            return character switch
            {
                SpeechCharacter.Character1 => char1Name,
                SpeechCharacter.Character2 => char2Name,
                SpeechCharacter.None => "",
                _ => ""
            };
        }

        /// <summary>
        /// SpeechPositionのYAML値を取得
        /// </summary>
        public static string ToYamlValue(this SpeechPosition position)
        {
            return position switch
            {
                SpeechPosition.Left => "left",
                SpeechPosition.Right => "right",
                _ => "left"
            };
        }
    }
}
