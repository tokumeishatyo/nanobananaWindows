// rule.mdを読むこと
using System.ComponentModel;
using System.Runtime.CompilerServices;
using nanobananaWindows.Models;

namespace nanobananaWindows.ViewModels
{
    /// <summary>
    /// 衣装着用設定ViewModel
    /// ※スタイルはメイン画面で一元管理（各ステップでは設定しない）
    /// </summary>
    public class OutfitSettingsViewModel : INotifyPropertyChanged
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

        private string _bodySheetImagePath = "";
        /// <summary>
        /// 素体三面図の画像パス
        /// </summary>
        public string BodySheetImagePath
        {
            get => _bodySheetImagePath;
            set => SetProperty(ref _bodySheetImagePath, value);
        }

        private bool _useOutfitBuilder = true;
        /// <summary>
        /// プリセットモードを使用するか（false=参考画像モード）
        /// </summary>
        public bool UseOutfitBuilder
        {
            get => _useOutfitBuilder;
            set => SetProperty(ref _useOutfitBuilder, value);
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
        // プリセットモード用プロパティ
        // ============================================================

        private OutfitCategory _outfitCategory = OutfitCategory.Auto;
        /// <summary>
        /// 衣装カテゴリ
        /// </summary>
        public OutfitCategory OutfitCategory
        {
            get => _outfitCategory;
            set
            {
                if (SetProperty(ref _outfitCategory, value))
                {
                    // カテゴリ変更時に形状をリセット
                    OutfitShape = value.GetShapes()[0];
                }
            }
        }

        private string _outfitShape = "おまかせ";
        /// <summary>
        /// 衣装形状
        /// </summary>
        public string OutfitShape
        {
            get => _outfitShape;
            set => SetProperty(ref _outfitShape, value);
        }

        private OutfitColor _outfitColor = OutfitColor.Auto;
        /// <summary>
        /// 衣装カラー
        /// </summary>
        public OutfitColor OutfitColor
        {
            get => _outfitColor;
            set => SetProperty(ref _outfitColor, value);
        }

        private OutfitPattern _outfitPattern = OutfitPattern.Auto;
        /// <summary>
        /// 衣装柄
        /// </summary>
        public OutfitPattern OutfitPattern
        {
            get => _outfitPattern;
            set => SetProperty(ref _outfitPattern, value);
        }

        private OutfitFashionStyle _outfitStyle = OutfitFashionStyle.Auto;
        /// <summary>
        /// 衣装スタイル（印象）
        /// </summary>
        public OutfitFashionStyle OutfitStyle
        {
            get => _outfitStyle;
            set => SetProperty(ref _outfitStyle, value);
        }

        // ============================================================
        // 参考画像モード用プロパティ
        // ============================================================

        private string _referenceOutfitImagePath = "";
        /// <summary>
        /// 衣装参考画像のパス
        /// </summary>
        public string ReferenceOutfitImagePath
        {
            get => _referenceOutfitImagePath;
            set => SetProperty(ref _referenceOutfitImagePath, value);
        }

        private string _referenceDescription = "";
        /// <summary>
        /// 参考画像の衣装説明
        /// </summary>
        public string ReferenceDescription
        {
            get => _referenceDescription;
            set => SetProperty(ref _referenceDescription, value);
        }

        private FitMode _fitMode = FitMode.BodyPriority;
        /// <summary>
        /// フィットモード
        /// </summary>
        public FitMode FitMode
        {
            get => _fitMode;
            set => SetProperty(ref _fitMode, value);
        }

        private bool _includeHeadwear = false;
        /// <summary>
        /// 頭部装飾（帽子・ヘルメット等）を含めるか
        /// </summary>
        public bool IncludeHeadwear
        {
            get => _includeHeadwear;
            set => SetProperty(ref _includeHeadwear, value);
        }

        // ============================================================
        // メソッド
        // ============================================================

        /// <summary>
        /// 設定が有効かどうか
        /// </summary>
        public bool HasSettings => !string.IsNullOrWhiteSpace(BodySheetImagePath);

        /// <summary>
        /// 設定をコピーする
        /// </summary>
        public OutfitSettingsViewModel Clone()
        {
            return new OutfitSettingsViewModel
            {
                BodySheetImagePath = this.BodySheetImagePath,
                UseOutfitBuilder = this.UseOutfitBuilder,
                AdditionalDescription = this.AdditionalDescription,
                // プリセットモード
                OutfitCategory = this.OutfitCategory,
                OutfitShape = this.OutfitShape,
                OutfitColor = this.OutfitColor,
                OutfitPattern = this.OutfitPattern,
                OutfitStyle = this.OutfitStyle,
                // 参考画像モード
                ReferenceOutfitImagePath = this.ReferenceOutfitImagePath,
                ReferenceDescription = this.ReferenceDescription,
                FitMode = this.FitMode,
                IncludeHeadwear = this.IncludeHeadwear
            };
        }

        /// <summary>
        /// 設定をリセットする
        /// </summary>
        public void Reset()
        {
            BodySheetImagePath = "";
            UseOutfitBuilder = true;
            AdditionalDescription = "";
            // プリセットモード
            OutfitCategory = OutfitCategory.Auto;
            OutfitShape = "おまかせ";
            OutfitColor = OutfitColor.Auto;
            OutfitPattern = OutfitPattern.Auto;
            OutfitStyle = OutfitFashionStyle.Auto;
            // 参考画像モード
            ReferenceOutfitImagePath = "";
            ReferenceDescription = "";
            FitMode = FitMode.BodyPriority;
            IncludeHeadwear = false;
        }
    }
}
