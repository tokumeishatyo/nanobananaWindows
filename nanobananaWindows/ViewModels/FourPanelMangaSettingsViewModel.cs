// rule.mdを読むこと
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using nanobananaWindows.Models;

namespace nanobananaWindows.ViewModels
{
    /// <summary>
    /// 4コマ漫画パネルのデータ
    /// </summary>
    public class MangaPanel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _scene = "";
        /// <summary>
        /// シーン説明
        /// </summary>
        public string Scene
        {
            get => _scene;
            set { _scene = value; OnPropertyChanged(); }
        }

        private SpeechCharacter _speech1Char = SpeechCharacter.Character1;
        /// <summary>
        /// セリフ1の話者
        /// </summary>
        public SpeechCharacter Speech1Char
        {
            get => _speech1Char;
            set { _speech1Char = value; OnPropertyChanged(); }
        }

        private string _speech1Text = "";
        /// <summary>
        /// セリフ1のテキスト
        /// </summary>
        public string Speech1Text
        {
            get => _speech1Text;
            set { _speech1Text = value; OnPropertyChanged(); }
        }

        private SpeechPosition _speech1Position = SpeechPosition.Left;
        /// <summary>
        /// セリフ1の位置
        /// </summary>
        public SpeechPosition Speech1Position
        {
            get => _speech1Position;
            set { _speech1Position = value; OnPropertyChanged(); }
        }

        private SpeechCharacter _speech2Char = SpeechCharacter.None;
        /// <summary>
        /// セリフ2の話者
        /// </summary>
        public SpeechCharacter Speech2Char
        {
            get => _speech2Char;
            set { _speech2Char = value; OnPropertyChanged(); }
        }

        private string _speech2Text = "";
        /// <summary>
        /// セリフ2のテキスト
        /// </summary>
        public string Speech2Text
        {
            get => _speech2Text;
            set { _speech2Text = value; OnPropertyChanged(); }
        }

        private SpeechPosition _speech2Position = SpeechPosition.Right;
        /// <summary>
        /// セリフ2の位置
        /// </summary>
        public SpeechPosition Speech2Position
        {
            get => _speech2Position;
            set { _speech2Position = value; OnPropertyChanged(); }
        }

        private string _narration = "";
        /// <summary>
        /// ナレーション
        /// </summary>
        public string Narration
        {
            get => _narration;
            set { _narration = value; OnPropertyChanged(); }
        }

        public MangaPanel Clone()
        {
            return new MangaPanel
            {
                Scene = this.Scene,
                Speech1Char = this.Speech1Char,
                Speech1Text = this.Speech1Text,
                Speech1Position = this.Speech1Position,
                Speech2Char = this.Speech2Char,
                Speech2Text = this.Speech2Text,
                Speech2Position = this.Speech2Position,
                Narration = this.Narration
            };
        }
    }

    /// <summary>
    /// セリフの話者
    /// </summary>
    public enum SpeechCharacter
    {
        None,       // なし
        Character1, // キャラクター1
        Character2  // キャラクター2
    }

    public static class SpeechCharacterExtensions
    {
        public static string GetDisplayName(this SpeechCharacter character)
        {
            return character switch
            {
                SpeechCharacter.None => "（なし）",
                SpeechCharacter.Character1 => "キャラ1",
                SpeechCharacter.Character2 => "キャラ2",
                _ => character.ToString()
            };
        }
    }

    /// <summary>
    /// セリフの位置
    /// </summary>
    public enum SpeechPosition
    {
        Left,  // 左
        Right  // 右
    }

    public static class SpeechPositionExtensions
    {
        public static string GetDisplayName(this SpeechPosition position)
        {
            return position switch
            {
                SpeechPosition.Left => "左",
                SpeechPosition.Right => "右",
                _ => position.ToString()
            };
        }
    }

    /// <summary>
    /// 4コマ漫画の詳細設定ViewModel
    /// </summary>
    public class FourPanelMangaSettingsViewModel : INotifyPropertyChanged
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
        // キャラクター1
        // ============================================================

        private string _character1Name = "";
        public string Character1Name
        {
            get => _character1Name;
            set => SetProperty(ref _character1Name, value);
        }

        private string _character1ImagePath = "";
        public string Character1ImagePath
        {
            get => _character1ImagePath;
            set => SetProperty(ref _character1ImagePath, value);
        }

        private string _character1Description = "";
        public string Character1Description
        {
            get => _character1Description;
            set => SetProperty(ref _character1Description, value);
        }

        // ============================================================
        // キャラクター2（任意）
        // ============================================================

        private string _character2Name = "";
        public string Character2Name
        {
            get => _character2Name;
            set => SetProperty(ref _character2Name, value);
        }

        private string _character2ImagePath = "";
        public string Character2ImagePath
        {
            get => _character2ImagePath;
            set => SetProperty(ref _character2ImagePath, value);
        }

        private string _character2Description = "";
        public string Character2Description
        {
            get => _character2Description;
            set => SetProperty(ref _character2Description, value);
        }

        // ============================================================
        // パネル（4コマ）
        // ============================================================

        private ObservableCollection<MangaPanel> _panels;
        public ObservableCollection<MangaPanel> Panels
        {
            get => _panels;
            set => SetProperty(ref _panels, value);
        }

        public FourPanelMangaSettingsViewModel()
        {
            _panels = new ObservableCollection<MangaPanel>
            {
                new MangaPanel(), // 1コマ目（起）
                new MangaPanel(), // 2コマ目（承）
                new MangaPanel(), // 3コマ目（転）
                new MangaPanel()  // 4コマ目（結）
            };
        }

        // ============================================================
        // メソッド
        // ============================================================

        /// <summary>
        /// 設定済みかどうか
        /// </summary>
        public bool HasSettings =>
            !string.IsNullOrWhiteSpace(Character1Name) ||
            !string.IsNullOrWhiteSpace(Character1ImagePath);

        /// <summary>
        /// ディープコピーを作成
        /// </summary>
        public FourPanelMangaSettingsViewModel Clone()
        {
            var clone = new FourPanelMangaSettingsViewModel
            {
                Character1Name = this.Character1Name,
                Character1ImagePath = this.Character1ImagePath,
                Character1Description = this.Character1Description,
                Character2Name = this.Character2Name,
                Character2ImagePath = this.Character2ImagePath,
                Character2Description = this.Character2Description
            };

            clone.Panels.Clear();
            foreach (var panel in this.Panels)
            {
                clone.Panels.Add(panel.Clone());
            }

            return clone;
        }

        /// <summary>
        /// 初期値にリセット
        /// </summary>
        public void Reset()
        {
            Character1Name = "";
            Character1ImagePath = "";
            Character1Description = "";
            Character2Name = "";
            Character2ImagePath = "";
            Character2Description = "";

            Panels.Clear();
            for (int i = 0; i < 4; i++)
            {
                Panels.Add(new MangaPanel());
            }
        }
    }
}
