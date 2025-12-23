// rule.mdを読むこと
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using nanobananaWindows.Models;
using nanobananaWindows.ViewModels;
using Windows.ApplicationModel.DataTransfer;

namespace nanobananaWindows.Views.Settings
{
    /// <summary>
    /// 装飾テキスト配置ダイアログ（サブダイアログ）
    /// シーンビルダーから呼び出される、最大10個の装飾テキストを配置
    /// </summary>
    public sealed partial class TextOverlayPlacementDialog : ContentDialog
    {
        private const int MaxItems = 10;
        private readonly ObservableCollection<TextOverlayItem> _items;
        private readonly List<Border> _itemRows = new();

        /// <summary>
        /// 適用された結果を取得
        /// </summary>
        public ObservableCollection<TextOverlayItem>? ResultItems { get; private set; }

        public TextOverlayPlacementDialog(ObservableCollection<TextOverlayItem>? initialItems = null)
        {
            InitializeComponent();

            // 既存アイテムがある場合はコピーして使用
            _items = new ObservableCollection<TextOverlayItem>();
            if (initialItems != null)
            {
                foreach (var item in initialItems)
                {
                    _items.Add(item.Clone());
                }
            }

            // UIを更新
            RefreshUI();
        }

        /// <summary>
        /// UI全体を更新
        /// </summary>
        private void RefreshUI()
        {
            // カウント更新
            CountText.Text = $"{_items.Count} / {MaxItems}";

            // ボタン状態更新
            AddButton.IsEnabled = _items.Count < MaxItems;
            RemoveButton.IsEnabled = _items.Count > 0;

            // プレースホルダー表示/非表示
            PlaceholderText.Visibility = _items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

            // 行を再構築
            RebuildItemRows();
        }

        /// <summary>
        /// アイテム行を再構築
        /// </summary>
        private void RebuildItemRows()
        {
            // 既存の行をクリア（プレースホルダー以外）
            foreach (var row in _itemRows)
            {
                ItemsPanel.Children.Remove(row);
            }
            _itemRows.Clear();

            // 各アイテムの行を作成
            for (int i = 0; i < _items.Count; i++)
            {
                var row = CreateItemRow(i);
                _itemRows.Add(row);
                ItemsPanel.Children.Add(row);
            }
        }

        /// <summary>
        /// アイテム行を作成
        /// </summary>
        private Border CreateItemRow(int index)
        {
            var item = _items[index];

            var border = new Border
            {
                Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"],
                CornerRadius = new CornerRadius(6),
                Padding = new Thickness(8)
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30) });  // 番号
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) }); // 画像パス
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) }); // 位置
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(70) });  // サイズ
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) }); // レイヤー

            // 番号
            var numberText = new TextBlock
            {
                Text = $"{index + 1}.",
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
            };
            Grid.SetColumn(numberText, 0);
            grid.Children.Add(numberText);

            // 画像パス
            var imagePathBox = new TextBox
            {
                Text = item.ImagePath,
                PlaceholderText = "画像パス（ドロップ可）",
                AllowDrop = true,
                Margin = new Thickness(4, 0, 4, 0)
            };
            imagePathBox.TextChanged += (s, e) => item.ImagePath = imagePathBox.Text;
            imagePathBox.DragOver += ImagePathTextBox_DragOver;
            imagePathBox.Drop += async (s, e) =>
            {
                if (e.DataView.Contains(StandardDataFormats.StorageItems))
                {
                    var items = await e.DataView.GetStorageItemsAsync();
                    if (items.Count > 0)
                    {
                        var file = items[0] as Windows.Storage.StorageFile;
                        if (file != null && IsImageFile(file.Name))
                        {
                            item.ImagePath = file.Path;
                            imagePathBox.Text = file.Path;
                        }
                    }
                }
            };
            Grid.SetColumn(imagePathBox, 1);
            grid.Children.Add(imagePathBox);

            // 位置
            var positionBox = new TextBox
            {
                Text = item.Position,
                PlaceholderText = "Center",
                Margin = new Thickness(4, 0, 4, 0)
            };
            positionBox.TextChanged += (s, e) => item.Position = positionBox.Text;
            Grid.SetColumn(positionBox, 2);
            grid.Children.Add(positionBox);

            // サイズ
            var sizeBox = new TextBox
            {
                Text = item.Size,
                PlaceholderText = "100%",
                Margin = new Thickness(4, 0, 4, 0)
            };
            sizeBox.TextChanged += (s, e) => item.Size = sizeBox.Text;
            Grid.SetColumn(sizeBox, 3);
            grid.Children.Add(sizeBox);

            // レイヤー
            var layerCombo = new ComboBox
            {
                Margin = new Thickness(4, 0, 0, 0)
            };
            foreach (var layer in Enum.GetValues<TextOverlayLayer>())
            {
                layerCombo.Items.Add(new ComboBoxItem { Content = layer.GetDisplayName(), Tag = layer });
            }
            // 現在の値を選択
            for (int i = 0; i < layerCombo.Items.Count; i++)
            {
                if (layerCombo.Items[i] is ComboBoxItem comboItem && comboItem.Tag is TextOverlayLayer l && l == item.Layer)
                {
                    layerCombo.SelectedIndex = i;
                    break;
                }
            }
            layerCombo.SelectionChanged += (s, e) =>
            {
                if (layerCombo.SelectedItem is ComboBoxItem comboItem && comboItem.Tag is TextOverlayLayer l)
                {
                    item.Layer = l;
                }
            };
            Grid.SetColumn(layerCombo, 4);
            grid.Children.Add(layerCombo);

            border.Child = grid;
            return border;
        }

        /// <summary>
        /// 画像ファイルのドラッグオーバー処理
        /// </summary>
        private void ImagePathTextBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                e.AcceptedOperation = DataPackageOperation.Copy;
            }
        }

        /// <summary>
        /// 画像ファイルかどうかを判定
        /// </summary>
        private static bool IsImageFile(string fileName)
        {
            var ext = System.IO.Path.GetExtension(fileName).ToLowerInvariant();
            return ext is ".png" or ".jpg" or ".jpeg" or ".gif" or ".webp";
        }

        // ============================================================
        // イベントハンドラ
        // ============================================================

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_items.Count < MaxItems)
            {
                _items.Add(new TextOverlayItem());
                RefreshUI();
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_items.Count > 0)
            {
                _items.RemoveAt(_items.Count - 1);
                RefreshUI();
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // 適用：結果を返す
            ResultItems = _items;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // キャンセル：nullを返す
            ResultItems = null;
        }
    }
}
