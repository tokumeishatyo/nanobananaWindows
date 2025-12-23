// rule.mdを読むこと
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace nanobananaWindows.ViewModels
{
    /// <summary>
    /// 顔三面図設定ViewModel
    /// ※スタイルはメイン画面で一元管理（各ステップでは設定しない）
    /// </summary>
    public class FaceSheetSettingsViewModel : INotifyPropertyChanged
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
        // プロパティ
        // ============================================================

        private string _characterName = "";
        /// <summary>
        /// キャラクター名
        /// </summary>
        public string CharacterName
        {
            get => _characterName;
            set => SetProperty(ref _characterName, value);
        }

        private string _referenceImagePath = "";
        /// <summary>
        /// 参照画像パス（任意）
        /// </summary>
        public string ReferenceImagePath
        {
            get => _referenceImagePath;
            set => SetProperty(ref _referenceImagePath, value);
        }

        private string _appearanceDescription = "";
        /// <summary>
        /// 外見説明
        /// </summary>
        public string AppearanceDescription
        {
            get => _appearanceDescription;
            set => SetProperty(ref _appearanceDescription, value);
        }

        /// <summary>
        /// 外見説明のプレースホルダーテキスト
        /// </summary>
        public string PlaceholderText => @"例：
・ショートボブの茶髪
・大きな青い瞳
・元気な笑顔
・左頬にホクロ";

        // ============================================================
        // メソッド
        // ============================================================

        /// <summary>
        /// 設定が有効かどうか（最低限キャラクター名か説明が必要）
        /// </summary>
        public bool HasSettings => !string.IsNullOrWhiteSpace(CharacterName) ||
                                   !string.IsNullOrWhiteSpace(AppearanceDescription);

        /// <summary>
        /// 設定をコピーする
        /// </summary>
        public FaceSheetSettingsViewModel Clone()
        {
            return new FaceSheetSettingsViewModel
            {
                CharacterName = this.CharacterName,
                ReferenceImagePath = this.ReferenceImagePath,
                AppearanceDescription = this.AppearanceDescription
            };
        }

        /// <summary>
        /// 設定をリセットする
        /// </summary>
        public void Reset()
        {
            CharacterName = "";
            ReferenceImagePath = "";
            AppearanceDescription = "";
        }
    }
}
