// rule.mdを読むこと
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace nanobananaWindows.ViewModels
{
    /// <summary>
    /// 背景生成の詳細設定ViewModel
    /// </summary>
    public class BackgroundSettingsViewModel : INotifyPropertyChanged
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

        private bool _useReferenceImage = false;
        /// <summary>
        /// 参考画像を使用するか（true=参考画像モード、false=説明文モード）
        /// </summary>
        public bool UseReferenceImage
        {
            get => _useReferenceImage;
            set => SetProperty(ref _useReferenceImage, value);
        }

        private string _referenceImagePath = "";
        /// <summary>
        /// 参考画像パス
        /// </summary>
        public string ReferenceImagePath
        {
            get => _referenceImagePath;
            set => SetProperty(ref _referenceImagePath, value);
        }

        private bool _removeCharacters = true;
        /// <summary>
        /// 人物を除去するか（参考画像モード用）
        /// </summary>
        public bool RemoveCharacters
        {
            get => _removeCharacters;
            set => SetProperty(ref _removeCharacters, value);
        }

        private string _description = "";
        /// <summary>
        /// 背景の説明文（説明文モード）または変換指示（参考画像モード）
        /// </summary>
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        // ============================================================
        // メソッド
        // ============================================================

        /// <summary>
        /// 設定済みかどうか
        /// </summary>
        public bool HasSettings =>
            (UseReferenceImage && !string.IsNullOrWhiteSpace(ReferenceImagePath)) ||
            (!UseReferenceImage && !string.IsNullOrWhiteSpace(Description));

        /// <summary>
        /// ディープコピーを作成
        /// </summary>
        public BackgroundSettingsViewModel Clone()
        {
            return new BackgroundSettingsViewModel
            {
                UseReferenceImage = this.UseReferenceImage,
                ReferenceImagePath = this.ReferenceImagePath,
                RemoveCharacters = this.RemoveCharacters,
                Description = this.Description
            };
        }

        /// <summary>
        /// 初期値にリセット
        /// </summary>
        public void Reset()
        {
            UseReferenceImage = false;
            ReferenceImagePath = "";
            RemoveCharacters = true;
            Description = "";
        }
    }
}
