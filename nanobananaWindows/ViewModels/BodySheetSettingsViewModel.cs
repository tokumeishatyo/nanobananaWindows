// rule.mdを読むこと
using System.ComponentModel;
using System.Runtime.CompilerServices;
using nanobananaWindows.Models;

namespace nanobananaWindows.ViewModels
{
    /// <summary>
    /// 素体三面図設定ViewModel
    /// ※スタイルはメイン画面で一元管理（各ステップでは設定しない）
    /// </summary>
    public class BodySheetSettingsViewModel : INotifyPropertyChanged
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

        private string _faceSheetImagePath = "";
        /// <summary>
        /// 顔三面図の画像パス
        /// </summary>
        public string FaceSheetImagePath
        {
            get => _faceSheetImagePath;
            set => SetProperty(ref _faceSheetImagePath, value);
        }

        private BodyTypePreset _bodyTypePreset = BodyTypePreset.FemaleStandard;
        /// <summary>
        /// 体型プリセット
        /// </summary>
        public BodyTypePreset BodyTypePreset
        {
            get => _bodyTypePreset;
            set => SetProperty(ref _bodyTypePreset, value);
        }

        private BustFeature _bustFeature = BustFeature.Auto;
        /// <summary>
        /// バスト特徴
        /// </summary>
        public BustFeature BustFeature
        {
            get => _bustFeature;
            set => SetProperty(ref _bustFeature, value);
        }

        private BodyRenderType _bodyRenderType = BodyRenderType.WhiteLeotard;
        /// <summary>
        /// 素体表現タイプ
        /// </summary>
        public BodyRenderType BodyRenderType
        {
            get => _bodyRenderType;
            set => SetProperty(ref _bodyRenderType, value);
        }

        private string _additionalDescription = "";
        /// <summary>
        /// 追加説明
        /// </summary>
        public string AdditionalDescription
        {
            get => _additionalDescription;
            set => SetProperty(ref _additionalDescription, value);
        }

        // ============================================================
        // メソッド
        // ============================================================

        /// <summary>
        /// 設定が有効かどうか
        /// </summary>
        public bool HasSettings => !string.IsNullOrWhiteSpace(FaceSheetImagePath);

        /// <summary>
        /// 設定をコピーする
        /// </summary>
        public BodySheetSettingsViewModel Clone()
        {
            return new BodySheetSettingsViewModel
            {
                FaceSheetImagePath = this.FaceSheetImagePath,
                BodyTypePreset = this.BodyTypePreset,
                BustFeature = this.BustFeature,
                BodyRenderType = this.BodyRenderType,
                AdditionalDescription = this.AdditionalDescription
            };
        }

        /// <summary>
        /// 設定をリセットする
        /// </summary>
        public void Reset()
        {
            FaceSheetImagePath = "";
            BodyTypePreset = BodyTypePreset.FemaleStandard;
            BustFeature = BustFeature.Auto;
            BodyRenderType = BodyRenderType.WhiteLeotard;
            AdditionalDescription = "";
        }
    }
}
