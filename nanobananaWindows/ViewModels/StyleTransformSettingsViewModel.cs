// rule.mdを読むこと
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace nanobananaWindows.ViewModels
{
    /// <summary>
    /// スタイル変換タイプ
    /// </summary>
    public enum StyleTransformType
    {
        Chibi, // ちびキャラ化
        Pixel  // ドットキャラ化
    }

    public static class StyleTransformTypeExtensions
    {
        public static string GetDisplayName(this StyleTransformType type)
        {
            return type switch
            {
                StyleTransformType.Chibi => "ちびキャラ化",
                StyleTransformType.Pixel => "ドットキャラ化",
                _ => type.ToString()
            };
        }
    }

    /// <summary>
    /// ちびキャラスタイル
    /// </summary>
    public enum ChibiStyle
    {
        TwoHead,   // 2頭身
        ThreeHead, // 3頭身
        Nendoroid, // ねんどろいど風
        Mascot     // マスコット風
    }

    public static class ChibiStyleExtensions
    {
        public static string GetDisplayName(this ChibiStyle style)
        {
            return style switch
            {
                ChibiStyle.TwoHead => "2頭身",
                ChibiStyle.ThreeHead => "3頭身",
                ChibiStyle.Nendoroid => "ねんどろいど風",
                ChibiStyle.Mascot => "マスコット風",
                _ => style.ToString()
            };
        }

        public static string GetPrompt(this ChibiStyle style)
        {
            return style switch
            {
                ChibiStyle.TwoHead => "2-head-tall chibi style, super deformed, cute big head, small body",
                ChibiStyle.ThreeHead => "3-head-tall chibi style, slightly deformed, cute proportions",
                ChibiStyle.Nendoroid => "Nendoroid figure style, glossy plastic look, articulated joints visible",
                ChibiStyle.Mascot => "Mascot character style, extremely simplified, round shapes, cute expression",
                _ => ""
            };
        }
    }

    /// <summary>
    /// ピクセルアートスタイル
    /// </summary>
    public enum PixelStyle
    {
        Bit8,   // 8bit風
        Bit16,  // 16bit風
        Modern, // モダンピクセル
        Retro,  // レトロゲーム風
        Isometric // アイソメトリック
    }

    public static class PixelStyleExtensions
    {
        public static string GetDisplayName(this PixelStyle style)
        {
            return style switch
            {
                PixelStyle.Bit8 => "8bit風",
                PixelStyle.Bit16 => "16bit風",
                PixelStyle.Modern => "モダンピクセル",
                PixelStyle.Retro => "レトロゲーム風",
                PixelStyle.Isometric => "アイソメトリック",
                _ => style.ToString()
            };
        }

        public static string GetPrompt(this PixelStyle style)
        {
            return style switch
            {
                PixelStyle.Bit8 => "8-bit pixel art style, NES/Famicom era, limited color palette",
                PixelStyle.Bit16 => "16-bit pixel art style, SNES/Super Famicom era, vibrant colors, detailed sprites",
                PixelStyle.Modern => "Modern pixel art style, high resolution pixels, smooth gradients, detailed shading",
                PixelStyle.Retro => "Retro game style, arcade game aesthetic, chunky pixels",
                PixelStyle.Isometric => "Isometric pixel art, 3D perspective, detailed tiles",
                _ => ""
            };
        }
    }

    /// <summary>
    /// スプライトサイズ
    /// </summary>
    public enum SpriteSize
    {
        Size32,  // 32x32
        Size64,  // 64x64
        Size128, // 128x128
        Size256  // 256x256
    }

    public static class SpriteSizeExtensions
    {
        public static string GetDisplayName(this SpriteSize size)
        {
            return size switch
            {
                SpriteSize.Size32 => "32x32",
                SpriteSize.Size64 => "64x64",
                SpriteSize.Size128 => "128x128",
                SpriteSize.Size256 => "256x256",
                _ => size.ToString()
            };
        }

        public static string GetPrompt(this SpriteSize size)
        {
            return size switch
            {
                SpriteSize.Size32 => "32x32 pixel sprite, minimal detail, iconic design",
                SpriteSize.Size64 => "64x64 pixel sprite, medium detail",
                SpriteSize.Size128 => "128x128 pixel sprite, high detail",
                SpriteSize.Size256 => "256x256 pixel sprite, very detailed",
                _ => ""
            };
        }
    }

    /// <summary>
    /// スタイル変換の詳細設定ViewModel
    /// </summary>
    public class StyleTransformSettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        // ============================================================
        // 共通プロパティ
        // ============================================================

        private string _sourceImagePath = "";
        /// <summary>
        /// 元画像のパス
        /// </summary>
        public string SourceImagePath
        {
            get => _sourceImagePath;
            set => SetProperty(ref _sourceImagePath, value);
        }

        private StyleTransformType _transformType = StyleTransformType.Chibi;
        /// <summary>
        /// 変換タイプ
        /// </summary>
        public StyleTransformType TransformType
        {
            get => _transformType;
            set => SetProperty(ref _transformType, value);
        }

        private bool _transparentBackground = true;
        /// <summary>
        /// 背景を透過にするか
        /// </summary>
        public bool TransparentBackground
        {
            get => _transparentBackground;
            set => SetProperty(ref _transparentBackground, value);
        }

        // ============================================================
        // ちびキャラ化用プロパティ
        // ============================================================

        private ChibiStyle _chibiStyle = ChibiStyle.TwoHead;
        /// <summary>
        /// ちびキャラスタイル
        /// </summary>
        public ChibiStyle ChibiStyle
        {
            get => _chibiStyle;
            set => SetProperty(ref _chibiStyle, value);
        }

        // ============================================================
        // ドットキャラ化用プロパティ
        // ============================================================

        private PixelStyle _pixelStyle = PixelStyle.Bit16;
        /// <summary>
        /// ピクセルアートスタイル
        /// </summary>
        public PixelStyle PixelStyle
        {
            get => _pixelStyle;
            set => SetProperty(ref _pixelStyle, value);
        }

        private SpriteSize _spriteSize = SpriteSize.Size64;
        /// <summary>
        /// スプライトサイズ
        /// </summary>
        public SpriteSize SpriteSize
        {
            get => _spriteSize;
            set => SetProperty(ref _spriteSize, value);
        }

        // ============================================================
        // メソッド
        // ============================================================

        /// <summary>
        /// 設定済みかどうか
        /// </summary>
        public bool HasSettings => !string.IsNullOrWhiteSpace(SourceImagePath);

        /// <summary>
        /// ディープコピーを作成
        /// </summary>
        public StyleTransformSettingsViewModel Clone()
        {
            return new StyleTransformSettingsViewModel
            {
                SourceImagePath = this.SourceImagePath,
                TransformType = this.TransformType,
                TransparentBackground = this.TransparentBackground,
                ChibiStyle = this.ChibiStyle,
                PixelStyle = this.PixelStyle,
                SpriteSize = this.SpriteSize
            };
        }

        /// <summary>
        /// 初期値にリセット
        /// </summary>
        public void Reset()
        {
            SourceImagePath = "";
            TransformType = StyleTransformType.Chibi;
            TransparentBackground = true;
            ChibiStyle = ChibiStyle.TwoHead;
            PixelStyle = PixelStyle.Bit16;
            SpriteSize = SpriteSize.Size64;
        }
    }
}
