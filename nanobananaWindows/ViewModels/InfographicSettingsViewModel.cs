// rule.mdを読むこと
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace nanobananaWindows.ViewModels
{
    /// <summary>
    /// インフォグラフィックスタイル
    /// </summary>
    public enum InfographicStyle
    {
        GraphicRecording, // グラフィックレコーディング
        Magazine,         // 雑誌風
        Newspaper,        // 新聞風
        Poster,           // ポスター風
        Presentation      // プレゼン資料風
    }

    public static class InfographicStyleExtensions
    {
        public static string GetDisplayName(this InfographicStyle style)
        {
            return style switch
            {
                InfographicStyle.GraphicRecording => "グラフィックレコーディング",
                InfographicStyle.Magazine => "雑誌風",
                InfographicStyle.Newspaper => "新聞風",
                InfographicStyle.Poster => "ポスター風",
                InfographicStyle.Presentation => "プレゼン資料風",
                _ => style.ToString()
            };
        }

        public static string GetKey(this InfographicStyle style)
        {
            return style switch
            {
                InfographicStyle.GraphicRecording => "graphic_recording",
                InfographicStyle.Magazine => "magazine",
                InfographicStyle.Newspaper => "newspaper",
                InfographicStyle.Poster => "poster",
                InfographicStyle.Presentation => "presentation",
                _ => "graphic_recording"
            };
        }

        public static string GetPrompt(this InfographicStyle style)
        {
            return style switch
            {
                InfographicStyle.GraphicRecording => "graphic recording style, colorful hand-drawn infographic, visual note-taking, sketchy icons, speech bubbles, arrows connecting concepts",
                InfographicStyle.Magazine => "magazine layout style, glossy editorial design, bold headlines, photo-realistic elements mixed with graphics",
                InfographicStyle.Newspaper => "newspaper style, black and white with accent colors, column layout, classic typography, journalistic feel",
                InfographicStyle.Poster => "poster design style, bold graphics, minimal text, high impact visuals, eye-catching layout",
                InfographicStyle.Presentation => "presentation slide style, clean corporate design, bullet points, charts and graphs, professional look",
                _ => ""
            };
        }
    }

    /// <summary>
    /// 出力言語
    /// </summary>
    public enum InfographicLanguage
    {
        Japanese, // 日本語
        English,  // 英語
        Korean,   // 韓国語
        Chinese,  // 中国語
        Other     // その他
    }

    public static class InfographicLanguageExtensions
    {
        public static string GetDisplayName(this InfographicLanguage lang)
        {
            return lang switch
            {
                InfographicLanguage.Japanese => "日本語",
                InfographicLanguage.English => "英語",
                InfographicLanguage.Korean => "韓国語",
                InfographicLanguage.Chinese => "中国語",
                InfographicLanguage.Other => "その他",
                _ => lang.ToString()
            };
        }

        public static string GetLanguageValue(this InfographicLanguage lang)
        {
            return lang switch
            {
                InfographicLanguage.Japanese => "Japanese",
                InfographicLanguage.English => "English",
                InfographicLanguage.Korean => "Korean",
                InfographicLanguage.Chinese => "Chinese",
                _ => ""
            };
        }
    }

    /// <summary>
    /// インフォグラフィックのセクションデータ
    /// </summary>
    public class InfographicSection : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _title = "";
        /// <summary>
        /// セクションタイトル
        /// </summary>
        public string Title
        {
            get => _title;
            set { _title = value; OnPropertyChanged(); }
        }

        private string _content = "";
        /// <summary>
        /// セクション内容
        /// </summary>
        public string Content
        {
            get => _content;
            set { _content = value; OnPropertyChanged(); }
        }

        public InfographicSection Clone()
        {
            return new InfographicSection
            {
                Title = this.Title,
                Content = this.Content
            };
        }
    }

    /// <summary>
    /// インフォグラフィックの詳細設定ViewModel
    /// </summary>
    public class InfographicSettingsViewModel : INotifyPropertyChanged
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
        // スタイル設定
        // ============================================================

        private InfographicStyle _infographicStyle = InfographicStyle.GraphicRecording;
        /// <summary>
        /// インフォグラフィックスタイル
        /// </summary>
        public InfographicStyle InfographicStyle
        {
            get => _infographicStyle;
            set => SetProperty(ref _infographicStyle, value);
        }

        private InfographicLanguage _outputLanguage = InfographicLanguage.Japanese;
        /// <summary>
        /// 出力言語
        /// </summary>
        public InfographicLanguage OutputLanguage
        {
            get => _outputLanguage;
            set => SetProperty(ref _outputLanguage, value);
        }

        private string _customLanguage = "";
        /// <summary>
        /// カスタム言語（OutputLanguage=Otherの場合）
        /// </summary>
        public string CustomLanguage
        {
            get => _customLanguage;
            set => SetProperty(ref _customLanguage, value);
        }

        // ============================================================
        // タイトル設定
        // ============================================================

        private string _mainTitle = "";
        /// <summary>
        /// メインタイトル
        /// </summary>
        public string MainTitle
        {
            get => _mainTitle;
            set => SetProperty(ref _mainTitle, value);
        }

        private string _subtitle = "";
        /// <summary>
        /// サブタイトル
        /// </summary>
        public string Subtitle
        {
            get => _subtitle;
            set => SetProperty(ref _subtitle, value);
        }

        // ============================================================
        // キャラクター設定
        // ============================================================

        private string _mainCharacterImagePath = "";
        /// <summary>
        /// メインキャラクター画像パス
        /// </summary>
        public string MainCharacterImagePath
        {
            get => _mainCharacterImagePath;
            set => SetProperty(ref _mainCharacterImagePath, value);
        }

        private string _subCharacterImagePath = "";
        /// <summary>
        /// サブキャラクター（ボーナスキャラ）画像パス
        /// </summary>
        public string SubCharacterImagePath
        {
            get => _subCharacterImagePath;
            set => SetProperty(ref _subCharacterImagePath, value);
        }

        // ============================================================
        // セクション（8個）
        // ============================================================

        private ObservableCollection<InfographicSection> _sections;
        /// <summary>
        /// セクションリスト（8個）
        /// </summary>
        public ObservableCollection<InfographicSection> Sections
        {
            get => _sections;
            set => SetProperty(ref _sections, value);
        }

        public InfographicSettingsViewModel()
        {
            _sections = new ObservableCollection<InfographicSection>();
            // デフォルトのセクションタイトル
            string[] defaultTitles = { "基本プロフィール", "性格", "好きなもの", "苦手なもの",
                                       "特技", "趣味", "口癖", "秘密" };
            for (int i = 0; i < 8; i++)
            {
                _sections.Add(new InfographicSection { Title = defaultTitles[i] });
            }
        }

        // ============================================================
        // メソッド
        // ============================================================

        /// <summary>
        /// 設定済みかどうか
        /// </summary>
        public bool HasSettings =>
            !string.IsNullOrWhiteSpace(MainCharacterImagePath) ||
            !string.IsNullOrWhiteSpace(MainTitle);

        /// <summary>
        /// ディープコピーを作成
        /// </summary>
        public InfographicSettingsViewModel Clone()
        {
            var clone = new InfographicSettingsViewModel
            {
                InfographicStyle = this.InfographicStyle,
                OutputLanguage = this.OutputLanguage,
                CustomLanguage = this.CustomLanguage,
                MainTitle = this.MainTitle,
                Subtitle = this.Subtitle,
                MainCharacterImagePath = this.MainCharacterImagePath,
                SubCharacterImagePath = this.SubCharacterImagePath
            };

            clone.Sections.Clear();
            foreach (var section in this.Sections)
            {
                clone.Sections.Add(section.Clone());
            }

            return clone;
        }

        /// <summary>
        /// 初期値にリセット
        /// </summary>
        public void Reset()
        {
            InfographicStyle = InfographicStyle.GraphicRecording;
            OutputLanguage = InfographicLanguage.Japanese;
            CustomLanguage = "";
            MainTitle = "";
            Subtitle = "";
            MainCharacterImagePath = "";
            SubCharacterImagePath = "";

            Sections.Clear();
            string[] defaultTitles = { "基本プロフィール", "性格", "好きなもの", "苦手なもの",
                                       "特技", "趣味", "口癖", "秘密" };
            for (int i = 0; i < 8; i++)
            {
                Sections.Add(new InfographicSection { Title = defaultTitles[i] });
            }
        }
    }
}
