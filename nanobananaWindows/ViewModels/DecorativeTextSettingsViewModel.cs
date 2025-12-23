// rule.mdを読むこと
using System.ComponentModel;
using System.Runtime.CompilerServices;
using nanobananaWindows.Models;

namespace nanobananaWindows.ViewModels
{
    /// <summary>
    /// 装飾テキストの詳細設定ViewModel
    /// </summary>
    public class DecorativeTextSettingsViewModel : INotifyPropertyChanged
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

        private DecorativeTextType _textType = DecorativeTextType.SkillName;
        /// <summary>
        /// テキストタイプ
        /// </summary>
        public DecorativeTextType TextType
        {
            get => _textType;
            set => SetProperty(ref _textType, value);
        }

        private string _text = "";
        /// <summary>
        /// テキスト内容
        /// </summary>
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
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
        // 技名テロップ用プロパティ
        // ============================================================

        private TitleFont _titleFont = TitleFont.HeavyMincho;
        public TitleFont TitleFont
        {
            get => _titleFont;
            set => SetProperty(ref _titleFont, value);
        }

        private TitleSize _titleSize = TitleSize.VeryLarge;
        public TitleSize TitleSize
        {
            get => _titleSize;
            set => SetProperty(ref _titleSize, value);
        }

        private GradientColor _titleColor = GradientColor.WhiteToBlue;
        public GradientColor TitleColor
        {
            get => _titleColor;
            set => SetProperty(ref _titleColor, value);
        }

        private OutlineColor _titleOutline = OutlineColor.Gold;
        public OutlineColor TitleOutline
        {
            get => _titleOutline;
            set => SetProperty(ref _titleOutline, value);
        }

        private GlowEffect _titleGlow = GlowEffect.BlueLightning;
        public GlowEffect TitleGlow
        {
            get => _titleGlow;
            set => SetProperty(ref _titleGlow, value);
        }

        // ============================================================
        // 決め台詞用プロパティ
        // ============================================================

        private CalloutType _calloutType = CalloutType.Comic;
        public CalloutType CalloutType
        {
            get => _calloutType;
            set => SetProperty(ref _calloutType, value);
        }

        private CalloutColor _calloutColor = CalloutColor.RedYellow;
        public CalloutColor CalloutColor
        {
            get => _calloutColor;
            set => SetProperty(ref _calloutColor, value);
        }

        private TextRotation _calloutRotation = TextRotation.None;
        public TextRotation CalloutRotation
        {
            get => _calloutRotation;
            set => SetProperty(ref _calloutRotation, value);
        }

        private TextDistortion _calloutDistortion = TextDistortion.None;
        public TextDistortion CalloutDistortion
        {
            get => _calloutDistortion;
            set => SetProperty(ref _calloutDistortion, value);
        }

        // ============================================================
        // キャラ名プレート用プロパティ
        // ============================================================

        private NameTagDesign _nameTagDesign = NameTagDesign.Jagged;
        public NameTagDesign NameTagDesign
        {
            get => _nameTagDesign;
            set => SetProperty(ref _nameTagDesign, value);
        }

        private TextRotation _nameTagRotation = TextRotation.None;
        public TextRotation NameTagRotation
        {
            get => _nameTagRotation;
            set => SetProperty(ref _nameTagRotation, value);
        }

        // ============================================================
        // メッセージウィンドウ用プロパティ
        // ============================================================

        private MessageWindowMode _messageMode = MessageWindowMode.Full;
        public MessageWindowMode MessageMode
        {
            get => _messageMode;
            set => SetProperty(ref _messageMode, value);
        }

        private string _speakerName = "";
        public string SpeakerName
        {
            get => _speakerName;
            set => SetProperty(ref _speakerName, value);
        }

        private MessageWindowStyle _messageStyle = MessageWindowStyle.SciFi;
        public MessageWindowStyle MessageStyle
        {
            get => _messageStyle;
            set => SetProperty(ref _messageStyle, value);
        }

        private MessageFrameType _messageFrameType = MessageFrameType.CyberneticBlue;
        public MessageFrameType MessageFrameType
        {
            get => _messageFrameType;
            set => SetProperty(ref _messageFrameType, value);
        }

        private double _messageOpacity = 0.8;
        public double MessageOpacity
        {
            get => _messageOpacity;
            set => SetProperty(ref _messageOpacity, value);
        }

        private FaceIconPosition _faceIconPosition = FaceIconPosition.LeftInside;
        public FaceIconPosition FaceIconPosition
        {
            get => _faceIconPosition;
            set => SetProperty(ref _faceIconPosition, value);
        }

        private string _faceIconImagePath = "";
        public string FaceIconImagePath
        {
            get => _faceIconImagePath;
            set => SetProperty(ref _faceIconImagePath, value);
        }

        // ============================================================
        // メソッド
        // ============================================================

        /// <summary>
        /// 設定済みかどうか
        /// </summary>
        public bool HasSettings => !string.IsNullOrWhiteSpace(Text);

        /// <summary>
        /// ディープコピーを作成
        /// </summary>
        public DecorativeTextSettingsViewModel Clone()
        {
            return new DecorativeTextSettingsViewModel
            {
                // 共通
                TextType = this.TextType,
                Text = this.Text,
                TransparentBackground = this.TransparentBackground,
                // 技名テロップ
                TitleFont = this.TitleFont,
                TitleSize = this.TitleSize,
                TitleColor = this.TitleColor,
                TitleOutline = this.TitleOutline,
                TitleGlow = this.TitleGlow,
                // 決め台詞
                CalloutType = this.CalloutType,
                CalloutColor = this.CalloutColor,
                CalloutRotation = this.CalloutRotation,
                CalloutDistortion = this.CalloutDistortion,
                // キャラ名プレート
                NameTagDesign = this.NameTagDesign,
                NameTagRotation = this.NameTagRotation,
                // メッセージウィンドウ
                MessageMode = this.MessageMode,
                SpeakerName = this.SpeakerName,
                MessageStyle = this.MessageStyle,
                MessageFrameType = this.MessageFrameType,
                MessageOpacity = this.MessageOpacity,
                FaceIconPosition = this.FaceIconPosition,
                FaceIconImagePath = this.FaceIconImagePath
            };
        }

        /// <summary>
        /// 初期値にリセット
        /// </summary>
        public void Reset()
        {
            // 共通
            TextType = DecorativeTextType.SkillName;
            Text = "";
            TransparentBackground = true;
            // 技名テロップ
            TitleFont = TitleFont.HeavyMincho;
            TitleSize = TitleSize.VeryLarge;
            TitleColor = GradientColor.WhiteToBlue;
            TitleOutline = OutlineColor.Gold;
            TitleGlow = GlowEffect.BlueLightning;
            // 決め台詞
            CalloutType = CalloutType.Comic;
            CalloutColor = CalloutColor.RedYellow;
            CalloutRotation = TextRotation.None;
            CalloutDistortion = TextDistortion.None;
            // キャラ名プレート
            NameTagDesign = NameTagDesign.Jagged;
            NameTagRotation = TextRotation.None;
            // メッセージウィンドウ
            MessageMode = MessageWindowMode.Full;
            SpeakerName = "";
            MessageStyle = MessageWindowStyle.SciFi;
            MessageFrameType = MessageFrameType.CyberneticBlue;
            MessageOpacity = 0.8;
            FaceIconPosition = FaceIconPosition.LeftInside;
            FaceIconImagePath = "";
        }
    }
}
