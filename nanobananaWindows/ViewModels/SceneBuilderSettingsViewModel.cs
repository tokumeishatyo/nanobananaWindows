// rule.mdを読むこと
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using nanobananaWindows.Models;

namespace nanobananaWindows.ViewModels
{
    /// <summary>
    /// ストーリーシーン用キャラクターデータ
    /// </summary>
    public class StoryCharacter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _imagePath = "";
        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }

        private string _expression = "";
        public string Expression
        {
            get => _expression;
            set { _expression = value; OnPropertyChanged(); }
        }

        private string _traits = "";
        public string Traits
        {
            get => _traits;
            set { _traits = value; OnPropertyChanged(); }
        }

        public StoryCharacter Clone()
        {
            return new StoryCharacter
            {
                ImagePath = ImagePath,
                Expression = Expression,
                Traits = Traits
            };
        }
    }

    /// <summary>
    /// 装飾テキストオーバーレイアイテム（最大10個）
    /// </summary>
    public class TextOverlayItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _imagePath = "";
        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }

        private string _position = "Center";
        public string Position
        {
            get => _position;
            set { _position = value; OnPropertyChanged(); }
        }

        private string _size = "100%";
        public string Size
        {
            get => _size;
            set { _size = value; OnPropertyChanged(); }
        }

        private TextOverlayLayer _layer = TextOverlayLayer.Frontmost;
        public TextOverlayLayer Layer
        {
            get => _layer;
            set { _layer = value; OnPropertyChanged(); }
        }

        public TextOverlayItem Clone()
        {
            return new TextOverlayItem
            {
                ImagePath = ImagePath,
                Position = Position,
                Size = Size,
                Layer = Layer
            };
        }
    }

    /// <summary>
    /// シーンビルダー設定ViewModel（Python版準拠）
    /// ※現在はストーリーシーンのみ対応。バトルシーン・ボスレイドは後日実装予定
    /// </summary>
    public class SceneBuilderSettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        // ============================================================
        // シーンタイプ（デフォルト: ストーリーシーン）
        // ============================================================

        private SceneType _sceneType = SceneType.Story;
        public SceneType SceneType
        {
            get => _sceneType;
            set => SetProperty(ref _sceneType, value);
        }

        // ============================================================
        // 共通: 背景設定
        // ============================================================

        private BackgroundSourceType _backgroundSourceType = BackgroundSourceType.File;
        public BackgroundSourceType BackgroundSourceType
        {
            get => _backgroundSourceType;
            set => SetProperty(ref _backgroundSourceType, value);
        }

        private string _backgroundImagePath = "";
        public string BackgroundImagePath
        {
            get => _backgroundImagePath;
            set => SetProperty(ref _backgroundImagePath, value);
        }

        private string _backgroundDescription = "";
        public string BackgroundDescription
        {
            get => _backgroundDescription;
            set => SetProperty(ref _backgroundDescription, value);
        }

        // ============================================================
        // ストーリーシーン用: 背景
        // ============================================================

        private double _storyBlurAmount = 10;
        public double StoryBlurAmount
        {
            get => _storyBlurAmount;
            set => SetProperty(ref _storyBlurAmount, value);
        }

        private LightingMood _storyLightingMood = LightingMood.Morning;
        public LightingMood StoryLightingMood
        {
            get => _storyLightingMood;
            set => SetProperty(ref _storyLightingMood, value);
        }

        private string _storyCustomMood = "";
        public string StoryCustomMood
        {
            get => _storyCustomMood;
            set => SetProperty(ref _storyCustomMood, value);
        }

        // ============================================================
        // ストーリーシーン用: 配置設定
        // ============================================================

        private StoryLayout _storyLayout = StoryLayout.SideBySide;
        public StoryLayout StoryLayout
        {
            get => _storyLayout;
            set => SetProperty(ref _storyLayout, value);
        }

        private string _storyCustomLayout = "";
        public string StoryCustomLayout
        {
            get => _storyCustomLayout;
            set => SetProperty(ref _storyCustomLayout, value);
        }

        private StoryDistance _storyDistance = StoryDistance.Close;
        public StoryDistance StoryDistance
        {
            get => _storyDistance;
            set => SetProperty(ref _storyDistance, value);
        }

        // ============================================================
        // ストーリーシーン用: キャラクター配置（動的、最大5人）
        // ============================================================

        private CharacterCount _storyCharacterCount = CharacterCount.Two;
        public CharacterCount StoryCharacterCount
        {
            get => _storyCharacterCount;
            set => SetProperty(ref _storyCharacterCount, value);
        }

        // 5人分のキャラクター枠を事前確保
        public ObservableCollection<StoryCharacter> StoryCharacters { get; } = new()
        {
            new StoryCharacter(),
            new StoryCharacter(),
            new StoryCharacter(),
            new StoryCharacter(),
            new StoryCharacter()
        };

        // ============================================================
        // ストーリーシーン用: ダイアログ設定
        // ============================================================

        private string _storyNarration = "";
        public string StoryNarration
        {
            get => _storyNarration;
            set => SetProperty(ref _storyNarration, value);
        }

        private NarrationPosition _storyNarrationPosition = NarrationPosition.Auto;
        public NarrationPosition StoryNarrationPosition
        {
            get => _storyNarrationPosition;
            set => SetProperty(ref _storyNarrationPosition, value);
        }

        // 5人分のセリフ枠を事前確保
        public ObservableCollection<string> StoryDialogues { get; } = new()
        {
            "", "", "", "", ""
        };

        // ============================================================
        // 共通: 装飾テキストオーバーレイ（最大10個）
        // ============================================================

        public ObservableCollection<TextOverlayItem> TextOverlayItems { get; } = new();

        // ============================================================
        // 設定判定
        // ============================================================

        /// <summary>
        /// 設定済みかどうか
        /// </summary>
        public bool HasSettings
        {
            get
            {
                // 背景が設定されているか、キャラクターが設定されているか
                if (!string.IsNullOrEmpty(BackgroundImagePath)) return true;
                if (!string.IsNullOrEmpty(BackgroundDescription)) return true;
                for (int i = 0; i < StoryCharacterCount.GetIntValue(); i++)
                {
                    if (!string.IsNullOrEmpty(StoryCharacters[i].ImagePath)) return true;
                }
                return false;
            }
        }

        // ============================================================
        // Clone / Reset
        // ============================================================

        /// <summary>
        /// ディープコピーを作成
        /// </summary>
        public SceneBuilderSettingsViewModel Clone()
        {
            var clone = new SceneBuilderSettingsViewModel
            {
                SceneType = SceneType,
                BackgroundSourceType = BackgroundSourceType,
                BackgroundImagePath = BackgroundImagePath,
                BackgroundDescription = BackgroundDescription,
                StoryBlurAmount = StoryBlurAmount,
                StoryLightingMood = StoryLightingMood,
                StoryCustomMood = StoryCustomMood,
                StoryLayout = StoryLayout,
                StoryCustomLayout = StoryCustomLayout,
                StoryDistance = StoryDistance,
                StoryCharacterCount = StoryCharacterCount,
                StoryNarration = StoryNarration,
                StoryNarrationPosition = StoryNarrationPosition
            };

            // キャラクターをコピー
            for (int i = 0; i < 5; i++)
            {
                clone.StoryCharacters[i] = StoryCharacters[i].Clone();
            }

            // セリフをコピー
            for (int i = 0; i < 5; i++)
            {
                clone.StoryDialogues[i] = StoryDialogues[i];
            }

            // 装飾テキストをコピー
            foreach (var item in TextOverlayItems)
            {
                clone.TextOverlayItems.Add(item.Clone());
            }

            return clone;
        }

        /// <summary>
        /// 設定をリセット
        /// </summary>
        public void Reset()
        {
            SceneType = SceneType.Story;
            BackgroundSourceType = BackgroundSourceType.File;
            BackgroundImagePath = "";
            BackgroundDescription = "";
            StoryBlurAmount = 10;
            StoryLightingMood = LightingMood.Morning;
            StoryCustomMood = "";
            StoryLayout = StoryLayout.SideBySide;
            StoryCustomLayout = "";
            StoryDistance = StoryDistance.Close;
            StoryCharacterCount = CharacterCount.Two;
            StoryNarration = "";
            StoryNarrationPosition = NarrationPosition.Auto;

            for (int i = 0; i < 5; i++)
            {
                StoryCharacters[i] = new StoryCharacter();
                StoryDialogues[i] = "";
            }

            TextOverlayItems.Clear();
        }
    }
}
