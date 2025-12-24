// rule.mdを読むこと
using System.ComponentModel;
using System.Runtime.CompilerServices;
using nanobananaWindows.Models;

namespace nanobananaWindows.ViewModels
{
    /// <summary>
    /// ポーズ設定ViewModel
    /// ※スタイルはメイン画面で一元管理（各ステップでは設定しない）
    /// </summary>
    public class PoseSettingsViewModel : INotifyPropertyChanged
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
        // ポーズプリセット関連
        // ============================================================

        private PosePreset _selectedPreset = PosePreset.None;
        /// <summary>
        /// 選択されたポーズプリセット
        /// </summary>
        public PosePreset SelectedPreset
        {
            get => _selectedPreset;
            set
            {
                if (SetProperty(ref _selectedPreset, value))
                {
                    // プリセット変更時に動作説明と風効果を自動設定
                    if (value != PosePreset.None && !UsePoseCapture)
                    {
                        ActionDescription = value.GetDescription();
                        WindEffect = value.GetDefaultWindEffect();
                    }
                }
            }
        }

        private bool _usePoseCapture = false;
        /// <summary>
        /// 参考画像のポーズをキャプチャするか
        /// </summary>
        public bool UsePoseCapture
        {
            get => _usePoseCapture;
            set => SetProperty(ref _usePoseCapture, value);
        }

        private string _poseReferenceImagePath = "";
        /// <summary>
        /// ポーズ参考画像のパス
        /// </summary>
        public string PoseReferenceImagePath
        {
            get => _poseReferenceImagePath;
            set => SetProperty(ref _poseReferenceImagePath, value);
        }

        // ============================================================
        // 入力画像
        // ============================================================

        private string _outfitSheetImagePath = "";
        /// <summary>
        /// 衣装着用三面図の画像パス
        /// </summary>
        public string OutfitSheetImagePath
        {
            get => _outfitSheetImagePath;
            set => SetProperty(ref _outfitSheetImagePath, value);
        }

        // ============================================================
        // 向き・表情
        // ============================================================

        private EyeLine _eyeLine = EyeLine.Front;
        /// <summary>
        /// 目線方向
        /// </summary>
        public EyeLine EyeLine
        {
            get => _eyeLine;
            set => SetProperty(ref _eyeLine, value);
        }

        private PoseExpression _expression = PoseExpression.Neutral;
        /// <summary>
        /// 表情
        /// </summary>
        public PoseExpression Expression
        {
            get => _expression;
            set => SetProperty(ref _expression, value);
        }

        private string _expressionDetail = "";
        /// <summary>
        /// 表情補足
        /// </summary>
        public string ExpressionDetail
        {
            get => _expressionDetail;
            set => SetProperty(ref _expressionDetail, value);
        }

        // ============================================================
        // 動作説明
        // ============================================================

        private string _actionDescription = "";
        /// <summary>
        /// 動作説明
        /// </summary>
        public string ActionDescription
        {
            get => _actionDescription;
            set => SetProperty(ref _actionDescription, value);
        }

        // ============================================================
        // ビジュアル効果
        // ============================================================

        private bool _includeEffects = false;
        /// <summary>
        /// エフェクトを描画するか（デフォルト: オフ）
        /// </summary>
        public bool IncludeEffects
        {
            get => _includeEffects;
            set => SetProperty(ref _includeEffects, value);
        }

        private WindEffect _windEffect = WindEffect.None;
        /// <summary>
        /// 風の効果
        /// </summary>
        public WindEffect WindEffect
        {
            get => _windEffect;
            set => SetProperty(ref _windEffect, value);
        }

        private bool _transparentBackground = true;
        /// <summary>
        /// 背景を透過にするか（デフォルト: オン）
        /// </summary>
        public bool TransparentBackground
        {
            get => _transparentBackground;
            set => SetProperty(ref _transparentBackground, value);
        }

        // ============================================================
        // メソッド
        // ============================================================

        /// <summary>
        /// 設定が有効かどうか
        /// </summary>
        public bool HasSettings => !string.IsNullOrWhiteSpace(OutfitSheetImagePath);

        /// <summary>
        /// 設定をコピーする
        /// </summary>
        public PoseSettingsViewModel Clone()
        {
            return new PoseSettingsViewModel
            {
                SelectedPreset = this.SelectedPreset,
                UsePoseCapture = this.UsePoseCapture,
                PoseReferenceImagePath = this.PoseReferenceImagePath,
                OutfitSheetImagePath = this.OutfitSheetImagePath,
                EyeLine = this.EyeLine,
                Expression = this.Expression,
                ExpressionDetail = this.ExpressionDetail,
                ActionDescription = this.ActionDescription,
                IncludeEffects = this.IncludeEffects,
                WindEffect = this.WindEffect,
                TransparentBackground = this.TransparentBackground
            };
        }

        /// <summary>
        /// 設定をリセットする
        /// </summary>
        public void Reset()
        {
            SelectedPreset = PosePreset.None;
            UsePoseCapture = false;
            PoseReferenceImagePath = "";
            OutfitSheetImagePath = "";
            EyeLine = EyeLine.Front;
            Expression = PoseExpression.Neutral;
            ExpressionDetail = "";
            ActionDescription = "";
            IncludeEffects = false;
            WindEffect = WindEffect.None;
            TransparentBackground = true;
        }
    }
}
