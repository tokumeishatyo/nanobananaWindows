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

    // ============================================================
    // 衣装着用（Step3）関連
    // ============================================================

    /// <summary>
    /// 衣装カテゴリ
    /// </summary>
    public enum OutfitCategory
    {
        Auto,       // おまかせ
        Model,      // モデル用
        Suit,       // スーツ
        Swimsuit,   // 水着
        Casual,     // カジュアル
        Uniform,    // 制服
        Formal,     // ドレス/フォーマル
        Sports,     // スポーツ
        Japanese,   // 和服
        Workwear    // 作業着/職業服
    }

    public static class OutfitCategoryExtensions
    {
        public static string GetDisplayName(this OutfitCategory category)
        {
            return category switch
            {
                OutfitCategory.Auto => "おまかせ",
                OutfitCategory.Model => "モデル用",
                OutfitCategory.Suit => "スーツ",
                OutfitCategory.Swimsuit => "水着",
                OutfitCategory.Casual => "カジュアル",
                OutfitCategory.Uniform => "制服",
                OutfitCategory.Formal => "ドレス/フォーマル",
                OutfitCategory.Sports => "スポーツ",
                OutfitCategory.Japanese => "和服",
                OutfitCategory.Workwear => "作業着/職業服",
                _ => category.ToString()
            };
        }

        /// <summary>
        /// カテゴリに対応する形状リストを取得
        /// </summary>
        public static string[] GetShapes(this OutfitCategory category)
        {
            return category switch
            {
                OutfitCategory.Auto => new[] { "おまかせ" },
                OutfitCategory.Model => new[] { "おまかせ", "白レオタード", "グレーレオタード", "黒レオタード",
                    "白下着", "Tシャツ+短パン", "タンクトップ+短パン" },
                OutfitCategory.Suit => new[] { "おまかせ", "パンツスタイル", "タイトスカート", "プリーツスカート",
                    "ミニスカート", "スリーピース", "ダブルスーツ", "タキシード" },
                OutfitCategory.Swimsuit => new[] { "おまかせ", "三角ビキニ", "ホルターネック", "バンドゥ",
                    "ワンピース", "ハイレグ", "パレオ付き", "サーフパンツ", "競泳パンツ" },
                OutfitCategory.Casual => new[] { "おまかせ", "Tシャツ+デニム", "ワンピース", "ブラウス+スカート",
                    "パーカー", "カーディガン", "シャツ+チノパン", "ポロシャツ", "レザージャケット" },
                OutfitCategory.Uniform => new[] { "おまかせ", "セーラー服", "ブレザー", "メイド服", "ナース服",
                    "OL制服", "学ラン", "詰襟", "警察官", "軍服" },
                OutfitCategory.Formal => new[] { "おまかせ", "イブニングドレス", "カクテルドレス", "ウェディングドレス",
                    "チャイナドレス", "サマードレス", "タキシード", "モーニング", "燕尾服" },
                OutfitCategory.Sports => new[] { "おまかせ", "テニスウェア", "体操服", "レオタード", "ヨガウェア",
                    "競泳水着", "サッカーユニフォーム", "野球ユニフォーム", "バスケユニフォーム", "柔道着" },
                OutfitCategory.Japanese => new[] { "おまかせ", "着物", "浴衣", "振袖", "巫女服",
                    "袴", "紋付袴", "羽織", "甚平" },
                OutfitCategory.Workwear => new[] { "おまかせ", "白衣", "作業着", "シェフコート", "消防服", "建設作業員" },
                _ => new[] { "おまかせ" }
            };
        }
    }

    /// <summary>
    /// 衣装カラー
    /// </summary>
    public enum OutfitColor
    {
        Auto,       // おまかせ
        Black,      // 黒
        White,      // 白
        Navy,       // 紺
        Red,        // 赤
        Pink,       // ピンク
        Blue,       // 青
        LightBlue,  // 水色
        Green,      // 緑
        Yellow,     // 黄
        Orange,     // オレンジ
        Purple,     // 紫
        Beige,      // ベージュ
        Gray,       // グレー
        Gold,       // ゴールド
        Silver      // シルバー
    }

    public static class OutfitColorExtensions
    {
        public static string GetDisplayName(this OutfitColor color)
        {
            return color switch
            {
                OutfitColor.Auto => "おまかせ",
                OutfitColor.Black => "黒",
                OutfitColor.White => "白",
                OutfitColor.Navy => "紺",
                OutfitColor.Red => "赤",
                OutfitColor.Pink => "ピンク",
                OutfitColor.Blue => "青",
                OutfitColor.LightBlue => "水色",
                OutfitColor.Green => "緑",
                OutfitColor.Yellow => "黄",
                OutfitColor.Orange => "オレンジ",
                OutfitColor.Purple => "紫",
                OutfitColor.Beige => "ベージュ",
                OutfitColor.Gray => "グレー",
                OutfitColor.Gold => "ゴールド",
                OutfitColor.Silver => "シルバー",
                _ => color.ToString()
            };
        }
    }

    /// <summary>
    /// 衣装柄
    /// </summary>
    public enum OutfitPattern
    {
        Auto,       // おまかせ
        Solid,      // 無地
        Stripe,     // ストライプ
        Check,      // チェック
        Floral,     // 花柄
        Dot,        // ドット
        Border,     // ボーダー
        Tropical,   // トロピカル
        Lace,       // レース
        Camouflage, // 迷彩
        Animal      // アニマル柄
    }

    public static class OutfitPatternExtensions
    {
        public static string GetDisplayName(this OutfitPattern pattern)
        {
            return pattern switch
            {
                OutfitPattern.Auto => "おまかせ",
                OutfitPattern.Solid => "無地",
                OutfitPattern.Stripe => "ストライプ",
                OutfitPattern.Check => "チェック",
                OutfitPattern.Floral => "花柄",
                OutfitPattern.Dot => "ドット",
                OutfitPattern.Border => "ボーダー",
                OutfitPattern.Tropical => "トロピカル",
                OutfitPattern.Lace => "レース",
                OutfitPattern.Camouflage => "迷彩",
                OutfitPattern.Animal => "アニマル柄",
                _ => pattern.ToString()
            };
        }
    }

    /// <summary>
    /// 衣装スタイル（印象）
    /// </summary>
    public enum OutfitFashionStyle
    {
        Auto,         // おまかせ
        Mature,       // 大人っぽい
        Cute,         // 可愛い
        Sexy,         // セクシー
        Cool,         // クール
        Modest,       // 清楚
        Sporty,       // スポーティ
        Gorgeous,     // ゴージャス
        Wild,         // ワイルド
        Intellectual, // 知的
        Dandy,        // ダンディ
        Casual        // カジュアル
    }

    public static class OutfitFashionStyleExtensions
    {
        public static string GetDisplayName(this OutfitFashionStyle style)
        {
            return style switch
            {
                OutfitFashionStyle.Auto => "おまかせ",
                OutfitFashionStyle.Mature => "大人っぽい",
                OutfitFashionStyle.Cute => "可愛い",
                OutfitFashionStyle.Sexy => "セクシー",
                OutfitFashionStyle.Cool => "クール",
                OutfitFashionStyle.Modest => "清楚",
                OutfitFashionStyle.Sporty => "スポーティ",
                OutfitFashionStyle.Gorgeous => "ゴージャス",
                OutfitFashionStyle.Wild => "ワイルド",
                OutfitFashionStyle.Intellectual => "知的",
                OutfitFashionStyle.Dandy => "ダンディ",
                OutfitFashionStyle.Casual => "カジュアル",
                _ => style.ToString()
            };
        }
    }

    /// <summary>
    /// フィットモード（参考画像モード用）
    /// </summary>
    public enum FitMode
    {
        BodyPriority,   // 素体優先
        OutfitPriority, // 衣装優先
        Hybrid          // ハイブリッド
    }

    public static class FitModeExtensions
    {
        public static string GetDisplayName(this FitMode mode)
        {
            return mode switch
            {
                FitMode.BodyPriority => "素体優先",
                FitMode.OutfitPriority => "衣装優先",
                FitMode.Hybrid => "ハイブリッド",
                _ => mode.ToString()
            };
        }
    }

    // ============================================================
    // ポーズ（Step4）関連
    // ============================================================

    /// <summary>
    /// ポーズプリセット
    /// </summary>
    public enum PosePreset
    {
        None,        // （プリセットなし）
        Hadouken,    // 波動拳（かめはめ波）
        SpeciumRay,  // スペシウム光線
        RiderKick,   // ライダーキック
        FingerBeam,  // 指先ビーム
        Meditation   // 坐禅（瞑想）
    }

    public static class PosePresetExtensions
    {
        public static string GetDisplayName(this PosePreset preset)
        {
            return preset switch
            {
                PosePreset.None => "（プリセットなし）",
                PosePreset.Hadouken => "波動拳（かめはめ波）",
                PosePreset.SpeciumRay => "スペシウム光線",
                PosePreset.RiderKick => "ライダーキック",
                PosePreset.FingerBeam => "指先ビーム",
                PosePreset.Meditation => "坐禅（瞑想）",
                _ => preset.ToString()
            };
        }

        /// <summary>
        /// プリセットに対応する動作説明（英語）
        /// </summary>
        public static string GetDescription(this PosePreset preset)
        {
            return preset switch
            {
                PosePreset.None => "",
                PosePreset.Hadouken => "Thrusting both palms forward at waist level, knees slightly bent, focusing energy between hands",
                PosePreset.SpeciumRay => "Crossing arms in a plus sign shape (+) in front of chest, right hand vertical, left hand horizontal",
                PosePreset.RiderKick => "Mid-air dynamic flying kick, one leg extended forward, body angled downward, floating in the air",
                PosePreset.FingerBeam => "Pointing index finger forward, arm fully extended, other fingers closed, cool and composed expression",
                PosePreset.Meditation => "Sitting cross-legged in lotus position, hands resting on knees, eyes closed, meditative posture",
                _ => ""
            };
        }

        /// <summary>
        /// プリセットに対応するデフォルト風効果
        /// </summary>
        public static WindEffect GetDefaultWindEffect(this PosePreset preset)
        {
            return preset switch
            {
                PosePreset.Hadouken or PosePreset.SpeciumRay or PosePreset.RiderKick => WindEffect.FromFront,
                _ => WindEffect.None
            };
        }
    }

    /// <summary>
    /// 目線方向
    /// </summary>
    public enum EyeLine
    {
        Front, // 前を見る
        Up,    // 上を見る
        Down   // 下を見る
    }

    public static class EyeLineExtensions
    {
        public static string GetDisplayName(this EyeLine eyeLine)
        {
            return eyeLine switch
            {
                EyeLine.Front => "前を見る",
                EyeLine.Up => "上を見る",
                EyeLine.Down => "下を見る",
                _ => eyeLine.ToString()
            };
        }
    }

    /// <summary>
    /// ポーズ表情
    /// </summary>
    public enum PoseExpression
    {
        Neutral, // 無表情
        Smile,   // 笑顔
        Angry,   // 怒り
        Crying,  // 泣き
        Shy      // 恥じらい
    }

    public static class PoseExpressionExtensions
    {
        public static string GetDisplayName(this PoseExpression expression)
        {
            return expression switch
            {
                PoseExpression.Neutral => "無表情",
                PoseExpression.Smile => "笑顔",
                PoseExpression.Angry => "怒り",
                PoseExpression.Crying => "泣き",
                PoseExpression.Shy => "恥じらい",
                _ => expression.ToString()
            };
        }

        public static string GetPrompt(this PoseExpression expression)
        {
            return expression switch
            {
                PoseExpression.Neutral => "neutral expression, calm face, no emotion",
                PoseExpression.Smile => "smiling, happy expression, cheerful face",
                PoseExpression.Angry => "angry expression, furious face, frowning",
                PoseExpression.Crying => "crying, tearful expression, sad face with tears",
                PoseExpression.Shy => "shy expression, blushing, embarrassed face",
                _ => ""
            };
        }
    }

    /// <summary>
    /// 風の効果
    /// </summary>
    public enum WindEffect
    {
        None,       // なし
        FromFront,  // 前からの風
        FromBehind, // 後ろからの風
        FromSide    // 横からの風
    }

    public static class WindEffectExtensions
    {
        public static string GetDisplayName(this WindEffect effect)
        {
            return effect switch
            {
                WindEffect.None => "なし",
                WindEffect.FromFront => "前からの風",
                WindEffect.FromBehind => "後ろからの風",
                WindEffect.FromSide => "横からの風",
                _ => effect.ToString()
            };
        }

        public static string GetPrompt(this WindEffect effect)
        {
            return effect switch
            {
                WindEffect.None => "",
                WindEffect.FromFront => "Strong Wind from Front",
                WindEffect.FromBehind => "Wind from Behind",
                WindEffect.FromSide => "Side Wind",
                _ => ""
            };
        }
    }
}
