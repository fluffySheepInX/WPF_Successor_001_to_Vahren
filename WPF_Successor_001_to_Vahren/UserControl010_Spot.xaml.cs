﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_Successor_001_to_Vahren._005_Class;

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// UserControl010_Spot.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl010_Spot : UserControl
    {
        public UserControl010_Spot()
        {
            InitializeComponent();
        }

        // 定数
        // ユニットのタイルサイズをここで調節できます
        private const int tile_width = 48, tile_height = 66, header_width = 56;
        // ドロップ先の強調枠の太さを一括指定します
        // 太さの違いで判定してるから、必ず drop_select_width > drop_target_width にすること。
        private const int drop_target_width = 2, drop_select_width = 4;

        // 最初に呼び出した時
        private bool _isControl = false; // 操作可能かどうかの設定
        public void SetData()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 最前面に配置する
            var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
            if ((listWindow != null) && (listWindow.Any()))
            {
                int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                Canvas.SetZIndex(this, maxZ + 1);
            }

            // プレイヤーが操作可能かどうか
            if (((ClassPowerAndCity)this.Tag).ClassPower.NameTag == mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.NameTag)
            {
                // 同じ勢力なら、操作できる
                _isControl = true;
            }
            else
            {
                _isControl = false;
            }

            // 領地ウィンドウを開いてる間、ワールドマップ上で強調する
            var worldMap = mainWindow.ClassGameStatus.WorldMap;
            if (worldMap != null)
            {
                string strFilename;
                // プレイヤー勢力なら色を変える
                if (_isControl)
                {
                    strFilename = "circle_Aqua.png";
                }
                // 中立領地
                else if (((ClassPowerAndCity)this.Tag).ClassPower.NameTag == string.Empty)
                {
                    strFilename = "circle_Yellow.png";
                }
                else
                {
                    strFilename =  "circle_Lime.png";
                }
                worldMap.SpotMarkAnime(strFilename, ((ClassPowerAndCity)this.Tag).ClassSpot.NameTag);
            }

            // 領地の情報を表示する
            DisplaySpotStatus(mainWindow);

            // ボタンの背景
            if (_isControl)
            {
                mainWindow.SetButtonImage(this.btnSelectAll, "wnd5.png");
                mainWindow.SetButtonImage(this.btnMercenary, "wnd5.png");
                mainWindow.SetButtonImage(this.btnPolitics, "wnd5.png");
            }

            // ウインドウ枠
            SetWindowFrame(mainWindow);

            // 透明から不透明になる
            var animeOpacity = new DoubleAnimation();
            animeOpacity.From = 0.1;
            animeOpacity.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            this.BeginAnimation(Grid.OpacityProperty, animeOpacity);
        }

        // ウインドウ枠を作る
        private void SetWindowFrame(MainWindow mainWindow)
        {
            // ウインドウスキンを読み込む
            List<string> strings = new List<string>();
            strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
            strings.Add("006_WindowImage");
            // 自勢力かどうかで枠を変える
            if (_isControl)
            {
                strings.Add("wnd0.png");
            }
            else
            {
                strings.Add("wnd1.png");
            }
            string path = System.IO.Path.Combine(strings.ToArray());
            if (System.IO.File.Exists(path) == false)
            {
                // 画像が存在しない場合は、デザイン時のまま（色や透明度は xaml で指定する）
                return;
            }
            var skin_bitmap = new BitmapImage(new Uri(path));
            Int32Rect rect;
            ImageBrush myImageBrush;

            // RPGツクールXP (192x128) と VX (128x128) のスキンに対応する
            if ((skin_bitmap.PixelHeight != 128) || ((skin_bitmap.PixelWidth != 128) && (skin_bitmap.PixelWidth != 192)))
            {
                // その他の画像は、そのまま引き延ばして表示する
                // ブラシ設定によって、タイルしたり、アスペクト比を保ったりすることも可能
                myImageBrush = new ImageBrush(skin_bitmap);
                myImageBrush.Stretch = Stretch.Fill;
                this.rectWindowPlane.Fill = myImageBrush;
                return;
            }

            // 不要な背景を表示しない
            this.rectShadowRight.Visibility = Visibility.Hidden;
            this.rectShadowBottom.Visibility = Visibility.Hidden;

            // 中央
            rect = new Int32Rect(0, 0, skin_bitmap.PixelWidth - 64, skin_bitmap.PixelWidth - 64);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Stretch = Stretch.Fill;
            this.rectWindowPlane.Margin = new Thickness(4, 4, 4, 4);
            this.rectWindowPlane.Fill = myImageBrush;

            // 左上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 64, 0, 16, 16);
            this.imgWindowLeftTop.Source = new CroppedBitmap(skin_bitmap, rect);

            // 右上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 16, 0, 16, 16);
            this.imgWindowRightTop.Source = new CroppedBitmap(skin_bitmap, rect);

            // 左下
            rect = new Int32Rect(skin_bitmap.PixelWidth - 64, 48, 16, 16);
            this.imgWindowLeftBottom.Source = new CroppedBitmap(skin_bitmap, rect);

            // 右上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 16, 48, 16, 16);
            this.imgWindowRightBottom.Source = new CroppedBitmap(skin_bitmap, rect);

            // 上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 48, 0, 32, 16);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowTop.Fill = myImageBrush;

            // 下
            rect = new Int32Rect(skin_bitmap.PixelWidth - 48, 48, 32, 16);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowBottom.Fill = myImageBrush;

            // 左
            rect = new Int32Rect(skin_bitmap.PixelWidth - 64, 16, 16, 32);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowLeft.Fill = myImageBrush;

            // 右
            rect = new Int32Rect(skin_bitmap.PixelWidth - 16, 16, 16, 32);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowRight.Fill = myImageBrush;
        }

        // 領地のユニットを変更した際に、ユニット表示だけを更新する
        public void UpdateSpotUnit(MainWindow mainWindow)
        {
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;

            // 画像のディレクトリ
            List<string> strings = new List<string>();
            strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
            strings.Add("040_ChipImage");
            string pathDirectory = System.IO.Path.Combine(strings.ToArray()) + System.IO.Path.DirectorySeparatorChar;

            // 最初に全て消去する
            this.canvasSpotUnit.Children.Clear();

            // 全ての部隊を表示する（駐留数の制限を超えていても許容する）
            var listTroop = classPowerAndCity.ClassSpot.UnitGroup;
            int i = 0, j, j_max = 1;
            foreach (var itemTroop in listTroop)
            {
                j = 0;
                foreach (var itemUnit in itemTroop.ListClassUnit)
                {
                    // 部隊の陣形はリーダー（先頭のユニット）を参照する
                    if (j == 0)
                    {
                        if (_isControl)
                        {
                            // 陣形コンボボックス
                            ComboBox cmbFormation = new ComboBox();
                            cmbFormation.Name = "cmbFormation" + i.ToString();
                            cmbFormation.Height = header_width - 22;
                            cmbFormation.Width = header_width - 4;
                            cmbFormation.Margin = new Thickness(2, (tile_height - cmbFormation.Height) / 2, 0, 0);
                            cmbFormation.Focusable = false;

                            // _010_Enum.Formation の順番に表示するので、Enum数値と Index は同じになる
                            // 項目ごとに背景色を変える
                            Label lblItem0 = new Label();
                            lblItem0.Content = _010_Enum.Formation.F.ToString();
                            lblItem0.FontSize = 20;
                            lblItem0.HorizontalContentAlignment = HorizontalAlignment.Center;
                            lblItem0.Padding = new Thickness(-5);
                            lblItem0.Width = header_width - 32;
                            lblItem0.Height = header_width - 28;
                            lblItem0.Background = Brushes.Maroon; // RGB=128,0,0
                            lblItem0.Foreground = Brushes.White;
                            ComboBoxItem cbItem0 = new ComboBoxItem();
                            cbItem0.Content = lblItem0;
                            cmbFormation.Items.Add(cbItem0);

                            Label lblItem1 = new Label();
                            lblItem1.Content = _010_Enum.Formation.M.ToString();
                            lblItem1.FontSize = 20;
                            lblItem1.HorizontalContentAlignment = HorizontalAlignment.Center;
                            lblItem1.Padding = new Thickness(-5);
                            lblItem1.Width = header_width - 32;
                            lblItem1.Height = header_width - 28;
                            lblItem1.Background = Brushes.DarkGreen; // RGB=0,100,0
                            lblItem1.Foreground = Brushes.White;
                            ComboBoxItem cbItem1 = new ComboBoxItem();
                            cbItem1.Content = lblItem1;
                            cmbFormation.Items.Add(cbItem1);

                            Label lblItem2 = new Label();
                            lblItem2.Content = _010_Enum.Formation.B.ToString();
                            lblItem2.FontSize = 20;
                            lblItem2.HorizontalContentAlignment = HorizontalAlignment.Center;
                            lblItem2.Padding = new Thickness(-5);
                            lblItem2.Width = header_width - 32;
                            lblItem2.Height = header_width - 28;
                            lblItem2.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 160));
                            lblItem2.Foreground = Brushes.White;
                            ComboBoxItem cbItem2 = new ComboBoxItem();
                            cbItem2.Content = lblItem2;
                            cmbFormation.Items.Add(cbItem2);

                            cmbFormation.SelectedIndex = (int)itemUnit.Formation.Formation;
                            cmbFormation.SelectionChanged += cmbFormation_SelectionChanged;
                            this.canvasSpotUnit.Children.Add(cmbFormation);
                            Canvas.SetTop(cmbFormation, tile_height * i);
                        }
                        else
                        {
                            // 操作できない場合は、陣形だけ表示する
                            Label lblFormation = new Label();
                            lblFormation.Foreground = Brushes.White;
                            // 項目ごとに背景色を変える
                            if ((int)itemUnit.Formation.Formation == 0)
                            {
                                lblFormation.Background = Brushes.Maroon;
                            }
                            else if ((int)itemUnit.Formation.Formation == 1)
                            {
                                lblFormation.Background = Brushes.DarkGreen;
                            }
                            else
                            {
                                lblFormation.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 160));
                            }
                            lblFormation.Width = header_width - 22;
                            lblFormation.Height = lblFormation.Width;
                            lblFormation.Margin = new Thickness((header_width - lblFormation.Width) / 2, (tile_height - lblFormation.Height) / 2, 0, 0);
                            lblFormation.FontSize = 20;
                            lblFormation.Padding = new Thickness(-5);
                            lblFormation.HorizontalContentAlignment = HorizontalAlignment.Center;
                            lblFormation.VerticalContentAlignment = VerticalAlignment.Center;
                            lblFormation.Content = itemUnit.Formation.Formation.ToString();
                            this.canvasSpotUnit.Children.Add(lblFormation);
                            Canvas.SetTop(lblFormation, tile_height * i);
                        }
                    }

                    // ユニットのパネル
                    StackPanel panelUnit = new StackPanel();
                    panelUnit.Name = "panelUnit" + i.ToString() + "_" + j.ToString();
                    panelUnit.Tag = itemUnit;
                    panelUnit.Height = tile_height;
                    panelUnit.Width = tile_width;
                    // 色を変えるためのイベントを追加する
                    panelUnit.MouseEnter += panel_MouseEnter;
                    panelUnit.MouseLeave += panel_MouseLeave;
                    // ユニット情報を表示するためのイベント
                    panelUnit.MouseLeftButtonDown += unit_MouseLeftButtonDown;
                    if (_isControl)
                    {
                        // 操作可能な時だけドラッグ移動の準備をしておく
                        if (j > 0)
                        {
                            // 部下なら、そのユニットだけ
                            panelUnit.MouseRightButtonDown += unit_MouseRightButtonDown;
                        }
                        else
                        {
                            // 隊長なら、部隊のユニット全て
                            panelUnit.MouseRightButtonDown += troop_MouseRightButtonDown;
                        }
                    }

                    // ユニットの画像
                    BitmapImage bitimg1 = new BitmapImage(new Uri(pathDirectory + itemUnit.Image));
                    Image imgUnit = new Image();
                    imgUnit.Name = "imgUnit" + i.ToString() + "_" + j.ToString();
                    imgUnit.Source = bitimg1;
                    imgUnit.Width = tile_width;
                    imgUnit.Height = tile_width;
                    // 画像本来のピクセルサイズで表示する場合は、PixelWidth と PixelHeight を指定する
                    //imgUnit.Width = bitimg1.PixelWidth;
                    //imgUnit.Height = bitimg1.PixelHeight;
                    panelUnit.Children.Add(imgUnit);

                    // ユニットのレベル
                    TextBlock txtLevel = new TextBlock();
                    txtLevel.Name = "txtLevel" + i.ToString() + "_" + j.ToString();
                    txtLevel.Height = tile_height - tile_width;
                    txtLevel.FontSize = 16;
                    txtLevel.TextAlignment = TextAlignment.Center;
                    if (itemUnit.IsDone == false)
                    {
                        txtLevel.Text = "lv" + itemUnit.Level.ToString();
                        txtLevel.Foreground = Brushes.White;
                    }
                    else
                    {
                        // 行動済みなら、レベルの横に「E」が付いて、文字色が黄色になる
                        txtLevel.Text = "lv" + itemUnit.Level.ToString() + "E";
                        txtLevel.Foreground = Brushes.Yellow;
                    }
                    panelUnit.Children.Add(txtLevel);

                    this.canvasSpotUnit.Children.Add(panelUnit);
                    Canvas.SetLeft(panelUnit, header_width + tile_width * j);
                    Canvas.SetTop(panelUnit, tile_height * i);

                    j++;
                    if (j_max < j)
                    {
                        j_max = j;
                    }
                }

                /*
                // 部隊の所属領地が正しくセットされてるか調べるデバッグ用
                if (itemTroop.Spot != classPowerAndCity.ClassSpot)
                {
                    MessageBox.Show(i + "番の領地 " + itemTroop.Spot.NameTag);
                }
                */

                i++;
            }

            // 将来的には、戦力値もここで更新すればよさそう。
            // ユニットを表示するついでに、戦力値を取得して合計しておけばいい。

            // 部隊数も更新する
            int spot_capacity = classPowerAndCity.ClassSpot.Capacity;
            this.txtTroopCount.Text = "部隊 " + i.ToString() + "/" + spot_capacity.ToString();

            // ユニット配置場所の大きさ
            if (_isControl)
            {
                // 操作可能な時だけドロップ先の枠の分も確保する
                int member_capacity = mainWindow.ClassGameStatus.ListClassScenarioInfo[mainWindow.ClassGameStatus.NumberScenarioSelection].MemberCapacity;
                if (j_max < member_capacity)
                {
                    j_max++;
                }
                if (i < spot_capacity)
                {
                    i++;
                }
            }
            this.canvasSpotUnit.Width = header_width + tile_width * j_max;
            this.canvasSpotUnit.Height = tile_height * i;
        }

        // 既に表示されていて、表示を更新する際
        public void DisplaySpotStatus(MainWindow mainWindow)
        {
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;

            // プレイヤーが操作可能かどうか
            if (_isControl == false)
            {
                // 異なる勢力なら、操作ボタンを隠す
                btnSelectAll.Visibility = Visibility.Hidden;
                btnMercenary.Visibility = Visibility.Hidden;
                btnPolitics.Visibility = Visibility.Hidden;
            }

            // 旗は存在する時だけ
            if (classPowerAndCity.ClassPower.FlagPath != string.Empty)
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("030_FlagImage");
                strings.Add(classPowerAndCity.ClassPower.FlagPath);
                string path = System.IO.Path.Combine(strings.ToArray());
                BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                Int32Rect rect = new Int32Rect(0, 0, 32, 32);
                var destimg = new CroppedBitmap(bitimg1, rect);
                this.imgFlag.Source = destimg;
            }
            // 領地名
            {
                this.txtNameSpot.Text = classPowerAndCity.ClassSpot.Name;
            }
            // 経済値
            {
                this.txtGain.Text = "経済 " + classPowerAndCity.ClassSpot.Gain.ToString();
            }
            // 城壁値
            {
                this.txtCastle.Text = "城壁 " + classPowerAndCity.ClassSpot.Castle.ToString();
            }
            // 戦力値
            {
                this.txtForce.Text = "戦力 ?";
            }
            // 部隊駐留数とユニット
            {
                UpdateSpotUnit(mainWindow);
            }

        }

        // ユニットをドロップする処理
        private bool DropTarget_Unit(MainWindow mainWindow, int troop_id, int member_id, string strTarget)
        {
            string[] strPart = strTarget.Split('_');
            ClassSpot srcSpot = ((ClassPowerAndCity)this.Tag).ClassSpot;
            ClassSpot? dstSpot = null;
            UserControl010_Spot? windowSpot = null;

            if (strPart[0] == this.Name)
            {
                // 同じ領地ウインドウの上
                windowSpot = this;
                dstSpot = srcSpot;
            }
            else if (strPart[0] == "Spot")
            {
                // 領地リストのインデックスから ClassSpot を取得する
                int spot_id = Int32.Parse(strPart[1]);
                dstSpot = mainWindow.ClassGameStatus.NowListSpot[spot_id];

                // 領地ウインドウが開いてるかどうか調べる
                foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
                {
                    string strTitle = itemWindow.Name;
                    if (strTitle.StartsWith(StringName.windowSpot))
                    {
                        if (dstSpot.NameTag == ((ClassPowerAndCity)itemWindow.Tag).ClassSpot.NameTag)
                        {
                            windowSpot = itemWindow;
                            break;
                        }
                    }
                }
            }
            else
            {
                // 指定番号の領地ウインドウを探す
                windowSpot = (UserControl010_Spot)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, strPart[0]);
                if (windowSpot == null)
                {
                    return false;
                }
                dstSpot = ((ClassPowerAndCity)windowSpot.Tag).ClassSpot;
            }
            if (dstSpot == null)
            {
                return false;
            }

            // 部隊メンバー入れ替え
            if ((strPart[1] == "Unit") && (strPart.Length >= 4))
            {
                // 入れ替え元の部隊とユニット
                ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[troop_id];
                ClassUnit srcUnit = srcTroop.ListClassUnit[member_id];

                // 入れ替え先の部隊とユニット
                int dst_troop_id = Int32.Parse(strPart[2]);
                int dst_member_id = Int32.Parse(strPart[3]);
                ClassHorizontalUnit dstTroop = dstSpot.UnitGroup[dst_troop_id];
                ClassUnit dstUnit = dstTroop.ListClassUnit[dst_member_id];

                // 移動先を取り除いてから、移動元を挿入すれば、位置がずれない
                dstTroop.ListClassUnit.RemoveAt(dst_member_id);
                dstTroop.ListClassUnit.Insert(dst_member_id, srcUnit);
                srcTroop.ListClassUnit.RemoveAt(member_id);
                srcTroop.ListClassUnit.Insert(member_id, dstUnit);

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if ((windowSpot != null) && (windowSpot != this))
                {
                    // 異なる領地に移動したら行動済みにする
                    srcUnit.IsDone = true;

                    // ウインドウが異なる場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            // 部隊メンバー追加
            if ((strPart[1] == "Right") && (strPart.Length >= 3))
            {
                // 移動元の部隊とユニット
                ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[troop_id];
                ClassUnit srcUnit = srcTroop.ListClassUnit[member_id];

                // 移動先の部隊にユニットを追加する
                int dst_troop_id = Int32.Parse(strPart[2]);
                ClassHorizontalUnit dstTroop = dstSpot.UnitGroup[dst_troop_id];
                dstTroop.ListClassUnit.Add(srcUnit);

                // 元の部隊からユニットを取り除く
                srcTroop.ListClassUnit.RemoveAt(member_id);

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if ((windowSpot != null) && (windowSpot != this))
                {
                    // 異なる領地に移動したら行動済みにする
                    srcUnit.IsDone = true;

                    // ウインドウが異なる場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            // 新規部隊を作成して間に追加
            if ((strPart[1] == "Top") && (strPart.Length >= 3))
            {
                // 移動元の部隊とユニット
                ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[troop_id];
                ClassUnit srcUnit = srcTroop.ListClassUnit[member_id];

                // 新規部隊を指定位置に挿入してユニットを追加する
                var listUnit = new List<ClassUnit>();
                listUnit.Add(srcUnit);
                int dst_troop_id = Int32.Parse(strPart[2]);
                dstSpot.UnitGroup.Insert(dst_troop_id, new ClassHorizontalUnit()
                {
                    Spot = dstSpot,
                    FlagDisplay = true,
                    ListClassUnit = listUnit
                });

                // 元の部隊からユニットを取り除く
                srcTroop.ListClassUnit.RemoveAt(member_id);

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if ((windowSpot != null) && (windowSpot != this))
                {
                    // 異なる領地に移動したら行動済みにする
                    srcUnit.IsDone = true;

                    // ウインドウが異なる場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            // 新規部隊を作成して末尾に追加
            if (strPart[1] == "Bottom")
            {
                // 移動元の部隊とユニット
                ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[troop_id];
                ClassUnit srcUnit = srcTroop.ListClassUnit[member_id];

                // 新規部隊を末尾に追加してユニットを追加する
                var listUnit = new List<ClassUnit>();
                listUnit.Add(srcUnit);
                dstSpot.UnitGroup.Add(new ClassHorizontalUnit()
                {
                    Spot = dstSpot,
                    FlagDisplay = true,
                    ListClassUnit = listUnit
                });

                // 元の部隊からユニットを取り除く
                srcTroop.ListClassUnit.RemoveAt(member_id);

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if ((windowSpot != null) && (windowSpot != this))
                {
                    // 異なる領地に移動したら行動済みにする
                    srcUnit.IsDone = true;

                    // ウインドウが異なる場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            // 別領地に新規部隊を作成して末尾に追加
            if ((strPart[0] == "Spot") && (strPart.Length >= 2))
            {
                // 移動元の部隊とユニット
                ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[troop_id];
                ClassUnit srcUnit = srcTroop.ListClassUnit[member_id];

                // 新規部隊を末尾に追加してユニットを追加する
                var listUnit = new List<ClassUnit>();
                listUnit.Add(srcUnit);
                srcUnit.IsDone = true; // 行動済みにする
                dstSpot.UnitGroup.Add(new ClassHorizontalUnit()
                {
                    Spot = dstSpot,
                    FlagDisplay = true,
                    ListClassUnit = listUnit
                });

                // 元の部隊からユニットを取り除く
                srcTroop.ListClassUnit.RemoveAt(member_id);

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if (windowSpot != null)
                {
                    // ウインドウが存在する場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            return false;
        }

        // 部隊をドロップする処理
        private bool DropTarget_Troop(MainWindow mainWindow, int troop_id, string strTarget)
        {
            string[] strPart = strTarget.Split('_');
            ClassSpot srcSpot = ((ClassPowerAndCity)this.Tag).ClassSpot;
            ClassSpot? dstSpot = null;
            UserControl010_Spot? windowSpot = null;

            if (strPart[0] == this.Name)
            {
                // 同じ領地ウインドウの上
                windowSpot = this;
                dstSpot = srcSpot;
            }
            else if (strPart[0] == "Spot")
            {
                // 領地リストのインデックスから ClassSpot を取得する
                int spot_id = Int32.Parse(strPart[1]);
                dstSpot = mainWindow.ClassGameStatus.NowListSpot[spot_id];

                // 領地ウインドウが開いてるかどうか調べる
                foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
                {
                    string strTitle = itemWindow.Name;
                    if (strTitle.StartsWith(StringName.windowSpot))
                    {
                        if (dstSpot.NameTag == ((ClassPowerAndCity)itemWindow.Tag).ClassSpot.NameTag)
                        {
                            windowSpot = itemWindow;
                            break;
                        }
                    }
                }
            }
            else
            {
                // 指定番号の領地ウインドウを探す
                windowSpot = (UserControl010_Spot)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, strPart[0]);
                if (windowSpot == null)
                {
                    return false;
                }
                dstSpot = ((ClassPowerAndCity)windowSpot.Tag).ClassSpot;
            }
            if (dstSpot == null)
            {
                return false;
            }

            // 部隊メンバー追加
            if ((strPart[1] == "Right") && (strPart.Length >= 3))
            {
                // 移動元の部隊
                ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[troop_id];

                // 移動先の部隊に全てのユニットを追加する
                int dst_troop_id = Int32.Parse(strPart[2]);
                ClassHorizontalUnit dstTroop = dstSpot.UnitGroup[dst_troop_id];
                foreach (ClassUnit srcUnit in srcTroop.ListClassUnit)
                {
                    dstTroop.ListClassUnit.Add(srcUnit);

                    // 異なる領地に移動したら行動済みにする
                    if ((windowSpot != null) && (windowSpot != this))
                    {
                        srcUnit.IsDone = true;
                    }
                }

                // 元の部隊から全てのユニットを取り除く
                srcTroop.ListClassUnit.Clear();

                // 移動元領地から部隊を取り除く
                srcSpot.UnitGroup.RemoveAt(troop_id);

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if ((windowSpot != null) && (windowSpot != this))
                {
                    // ウインドウが異なる場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            // 部隊を間に追加
            if ((strPart[1] == "Top") && (strPart.Length >= 3))
            {
                // 移動元の部隊
                ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[troop_id];

                // 移動先の指定位置
                int dst_troop_id = Int32.Parse(strPart[2]);

                // 後に挿入する場合、先に取り除くと順番が変わるので、先に挿入する
                if (dst_troop_id > troop_id)
                {
                    // 移動先領地の指定位置に部隊を挿入する
                    dstSpot.UnitGroup.Insert(dst_troop_id, srcTroop);
                    srcTroop.Spot = dstSpot;

                    // 移動元領地から部隊を取り除く
                    srcSpot.UnitGroup.RemoveAt(troop_id);
                }
                // 前に挿入する場合、先に挿入すると順番が変わるので、先に取り除く
                else
                {
                    // 移動元領地から部隊を取り除く
                    srcSpot.UnitGroup.RemoveAt(troop_id);

                    // 移動先領地の指定位置に部隊を挿入する
                    dstSpot.UnitGroup.Insert(dst_troop_id, srcTroop);
                    srcTroop.Spot = dstSpot;
                }

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if ((windowSpot != null) && (windowSpot != this))
                {
                    // 異なる領地に移動したら行動済みにする
                    foreach (ClassUnit srcUnit in srcTroop.ListClassUnit)
                    {
                        srcUnit.IsDone = true;
                    }

                    // ウインドウが異なる場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            // 部隊を末尾に移動
            if (strPart[1] == "Bottom")
            {
                // 移動元の部隊
                ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[troop_id];

                // 移動先領地の末尾に部隊を追加する
                dstSpot.UnitGroup.Add(srcTroop);
                srcTroop.Spot = dstSpot;

                // 移動元領地から部隊を取り除く
                srcSpot.UnitGroup.RemoveAt(troop_id);

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if ((windowSpot != null) && (windowSpot != this))
                {
                    // 異なる領地に移動したら行動済みにする
                    foreach (ClassUnit srcUnit in srcTroop.ListClassUnit)
                    {
                        srcUnit.IsDone = true;
                    }

                    // ウインドウが異なる場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            // 部隊を別領地の末尾に移動
            if ((strPart[0] == "Spot") && (strPart.Length >= 2))
            {
                // 移動元の部隊
                ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[troop_id];

                // 移動先領地の末尾に部隊を追加する
                dstSpot.UnitGroup.Add(srcTroop);
                srcTroop.Spot = dstSpot;

                // 行動済みにする
                foreach (ClassUnit srcUnit in srcTroop.ListClassUnit)
                {
                    srcUnit.IsDone = true;
                }

                // 移動元領地から部隊を取り除く
                srcSpot.UnitGroup.RemoveAt(troop_id);

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if (windowSpot != null)
                {
                    // ウインドウが存在する場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            return false;
        }

        // 複数の部隊をドロップする処理
        // 戻り値 : 0 = 移動せず, マイナス = 全て移動, 1~ 限られた数だけ移動
        private int DropTarget_Whole(MainWindow mainWindow, string strTarget)
        {
            string[] strPart = strTarget.Split('_');
            ClassSpot srcSpot = ((ClassPowerAndCity)this.Tag).ClassSpot;
            ClassSpot? dstSpot = null;
            UserControl010_Spot? windowSpot = null;
            int moved_result = 0;

            if (strPart[0] == this.Name)
            {
                // 同じ領地ウインドウの上にはドロップできないはず
                return 0;
            }
            else if (strPart[0] == "Spot")
            {
                // 領地リストのインデックスから ClassSpot を取得する
                int spot_id = Int32.Parse(strPart[1]);
                dstSpot = mainWindow.ClassGameStatus.NowListSpot[spot_id];

                // 領地ウインドウが開いてるかどうか調べる
                foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
                {
                    string strTitle = itemWindow.Name;
                    if (strTitle.StartsWith(StringName.windowSpot))
                    {
                        if (dstSpot.NameTag == ((ClassPowerAndCity)itemWindow.Tag).ClassSpot.NameTag)
                        {
                            windowSpot = itemWindow;
                            break;
                        }
                    }
                }
            }
            else
            {
                // 指定番号の領地ウインドウを探す
                windowSpot = (UserControl010_Spot)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, strPart[0]);
                if (windowSpot == null)
                {
                    return 0;
                }
                dstSpot = ((ClassPowerAndCity)windowSpot.Tag).ClassSpot;
            }
            if (dstSpot == null)
            {
                return 0;
            }

            // 移動先の空きが部隊数よりも少ない場合は、入るだけ移動させる
            int src_troop_count = srcSpot.UnitGroup.Count;
            int dst_troop_count = dstSpot.UnitGroup.Count;
            int spot_capacity = dstSpot.Capacity;
            int move_count = src_troop_count;
            moved_result = move_count * -1;
            if (move_count > spot_capacity - dst_troop_count)
            {
                move_count = spot_capacity - dst_troop_count;
                moved_result = move_count;
            }

            // 部隊を間に追加
            if ((strPart[1] == "Top") && (strPart.Length >= 3))
            {
                // 移動先の指定位置
                int dst_troop_id = Int32.Parse(strPart[2]);

                for (int i = 0; i < move_count; i++)
                {
                    // 先頭の部隊から移動元にする
                    ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[0];

                    // 移動先領地の指定位置に部隊を挿入する
                    dstSpot.UnitGroup.Insert(dst_troop_id + i, srcTroop);
                    srcTroop.Spot = dstSpot;

                    // 行動済みにする
                    foreach (ClassUnit srcUnit in srcTroop.ListClassUnit)
                    {
                        srcUnit.IsDone = true;
                    }

                    // 移動元領地から先頭の部隊を取り除く
                    srcSpot.UnitGroup.RemoveAt(0);
                }

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if (windowSpot != null)
                {
                    // ウインドウが存在する場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return moved_result;
            }

            // 部隊を別領地の末尾に移動（領地ウインドウと領地アイコンで同じ処理）
            if ((strPart[1] == "Bottom") ||
                 ((strPart[0] == "Spot") && (strPart.Length >= 2)))
            {
                for (int i = 0; i < move_count; i++)
                {
                    // 先頭の部隊から移動元にする
                    ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[0];

                    // 移動先領地の末尾に部隊を追加する
                    dstSpot.UnitGroup.Add(srcTroop);
                    srcTroop.Spot = dstSpot;

                    // 行動済みにする
                    foreach (ClassUnit srcUnit in srcTroop.ListClassUnit)
                    {
                        srcUnit.IsDone = true;
                    }

                    // 移動元領地から部隊を取り除く
                    srcSpot.UnitGroup.RemoveAt(0);
                }

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if (windowSpot != null)
                {
                    // ウインドウが存在する場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return moved_result;
            }

            return 0;
        }

        // ユニットをドラッグ移動する際に、移動先を作る
        private void MakeDropTarget_Unit(MainWindow mainWindow, int troop_id, int member_id)
        {
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;
            int member_capacity = mainWindow.ClassGameStatus.ListClassScenarioInfo[mainWindow.ClassGameStatus.NumberScenarioSelection].MemberCapacity;
            int spot_capacity = classPowerAndCity.ClassSpot.Capacity;

            var listTroop = classPowerAndCity.ClassSpot.UnitGroup;
            int i = 0, j, troop_count, member_count;
            troop_count = listTroop.Count;
            foreach (var itemTroop in listTroop)
            {
                // 他のユニットの位置なら「部隊メンバー入れ替え」動作になる
                member_count = itemTroop.ListClassUnit.Count;
                if (i == troop_id)
                {
                    // 選択ユニットと同じ部隊だけ、リーダーとの入れ替えが可能
                    j = 0;
                }
                else
                {
                    j = 1;
                }
                for (; j < member_count; j++)
                {
                    // ドラッグ中のユニットの位置を暗くする
                    if ((i == troop_id) && (j == member_id))
                    {
                        Border border = new Border();
                        border.Name = "DropTarget";
                        // 半透明のブラシを作成する (黒の50%)
                        SolidColorBrush mySolidColorBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
                        border.Background = mySolidColorBrush;
                        border.BorderThickness = new Thickness(0);
                        border.Width = tile_width;
                        border.Height = tile_height;
                        this.canvasSpotUnit.Children.Add(border);
                        Canvas.SetLeft(border, header_width + tile_width * j);
                        Canvas.SetTop(border, tile_height * i);
                    }
                    else
                    {
                        Border border = new Border();
                        border.Name = "DropTarget" + this.Name + "_Unit_" + i.ToString() + "_" + j.ToString();
                        border.Background = Brushes.Transparent;
                        border.BorderThickness = new Thickness(drop_target_width);
                        border.BorderBrush = Brushes.Magenta;
                        border.Width = tile_width - 1;
                        border.Height = tile_height - 1;
                        this.canvasSpotUnit.Children.Add(border);
                        Canvas.SetLeft(border, header_width + tile_width * j + 1);
                        Canvas.SetTop(border, tile_height * i);
                    }
                }

                // 右の空きスペースなら「部隊メンバー追加」動作になる
                if ((j < member_capacity) && ((i != troop_id) || (member_id < member_count - 1)))
                {
                    Border border = new Border();
                    border.Name = "DropTarget" + this.Name + "_Right_" + i.ToString();
                    border.Background = Brushes.Transparent;
                    border.BorderThickness = new Thickness(drop_target_width);
                    border.BorderBrush = Brushes.Magenta;
                    border.Width = tile_width * (member_capacity - j) - 1;
                    border.Height = tile_height - 1;
                    this.canvasSpotUnit.Children.Add(border);
                    Canvas.SetLeft(border, header_width + tile_width * j + 1);
                    Canvas.SetTop(border, tile_height * i);
                }

                // 先頭ユニットの上端なら「新規部隊を作成して間に追加」動作になる
                if ((troop_count < spot_capacity) && (i != troop_id))
                {
                    Border border = new Border();
                    border.Name = "DropTarget" + this.Name + "_Top_" + i.ToString();
                    border.Background = Brushes.Transparent;
                    border.BorderThickness = new Thickness(drop_target_width);
                    border.BorderBrush = Brushes.Magenta;
                    border.Width = header_width + tile_width;
                    border.Height = tile_height / 3;
                    this.canvasSpotUnit.Children.Add(border);
                    Canvas.SetTop(border, tile_height * i);
                }

                i++;
            }

            // 下の空きスペースなら「新規部隊を作成して末尾に追加」動作になる
            if (i < spot_capacity)
            {
                Border border = new Border();
                border.Name = "DropTarget" + this.Name + "_Bottom";
                border.Background = Brushes.Transparent;
                border.BorderThickness = new Thickness(drop_target_width);
                border.BorderBrush = Brushes.Magenta;
                border.Width = header_width + tile_width * member_capacity;
                border.Height = tile_height * (spot_capacity - i);
                this.canvasSpotUnit.Children.Add(border);
                Canvas.SetTop(border, tile_height * i);
            }

            // 他の領地ウインドウを探す
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
            {
                string strTitle = itemWindow.Name;
                if ((strTitle.StartsWith(StringName.windowSpot)) && (strTitle != this.Name))
                {
                    var targetPowerAndCity = (ClassPowerAndCity)itemWindow.Tag;
                    // 同じ勢力の領地ウインドウだけ対象にする
                    if (targetPowerAndCity.ClassPower.NameTag == classPowerAndCity.ClassPower.NameTag)
                    {
                        // 領地の部隊情報を調べる
                        listTroop = targetPowerAndCity.ClassSpot.UnitGroup;
                        spot_capacity = targetPowerAndCity.ClassSpot.Capacity;
                        i = 0;
                        troop_count = listTroop.Count;
                        foreach (var itemTroop in listTroop)
                        {
                            member_count = itemTroop.ListClassUnit.Count;

                            // 右の空きスペースなら「部隊メンバー追加」動作になる
                            if (member_count < member_capacity)
                            {
                                Border border = new Border();
                                border.Name = "DropTarget" + strTitle + "_Right_" + i.ToString();
                                border.Background = Brushes.Transparent;
                                border.BorderThickness = new Thickness(drop_target_width);
                                border.BorderBrush = Brushes.Magenta;
                                border.Width = tile_width * (member_capacity - member_count) - 1;
                                border.Height = tile_height - 1;
                                itemWindow.canvasSpotUnit.Children.Add(border);
                                Canvas.SetLeft(border, header_width + tile_width * member_count + 1);
                                Canvas.SetTop(border, tile_height * i);
                            }

                            // 先頭ユニットの上端なら「新規部隊を作成して間に追加」動作になる
                            if (troop_count < spot_capacity)
                            {
                                Border border = new Border();
                                border.Name = "DropTarget" + strTitle + "_Top_" + i.ToString();
                                border.Background = Brushes.Transparent;
                                border.BorderThickness = new Thickness(drop_target_width);
                                border.BorderBrush = Brushes.Magenta;
                                border.Width = header_width + tile_width;
                                border.Height = tile_height / 3;
                                itemWindow.canvasSpotUnit.Children.Add(border);
                                Canvas.SetTop(border, tile_height * i);
                            }

                            i++;
                        }

                        // 下の空きスペースなら「新規部隊を作成して末尾に追加」動作になる
                        if (i < spot_capacity)
                        {
                            Border border = new Border();
                            border.Name = "DropTarget" + strTitle + "_Bottom";
                            border.Background = Brushes.Transparent;
                            border.BorderThickness = new Thickness(drop_target_width);
                            border.BorderBrush = Brushes.Magenta;
                            border.Width = header_width + tile_width * member_capacity;
                            border.Height = tile_height * (spot_capacity - i);
                            itemWindow.canvasSpotUnit.Children.Add(border);
                            Canvas.SetTop(border, tile_height * i);
                        }
                    }
                }
            }

            // 戦略マップ上の領地アイコンも
            if (mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.ListMember.Count > 1)
            {
                var worldMap = mainWindow.ClassGameStatus.WorldMap;
                if (worldMap != null)
                {
                    var listSpot = mainWindow.ClassGameStatus.NowListSpot;
                    int spot_count = listSpot.Count;
                    for (int spot_id = 0; spot_id < spot_count; spot_id++)
                    {
                        // 同じ勢力の領地だけ
                        ClassSpot itemSpot = listSpot[spot_id];
                        if (itemSpot.PowerNameTag == classPowerAndCity.ClassPower.NameTag)
                        {
                            // 領地の部隊数に空きがあるなら
                            spot_capacity = itemSpot.Capacity;
                            troop_count = itemSpot.UnitGroup.Count;
                            if ((troop_count < spot_capacity) && (itemSpot.NameTag != classPowerAndCity.ClassSpot.NameTag))
                            {
                                Border border = new Border();
                                // 領地リストのインデックスで識別する
                                border.Name = "DropTargetSpot_" + spot_id.ToString();
                                // ワールドマップ上で識別しやすいよう、背景を少し暗くする
                                SolidColorBrush mySolidColorBrush = new SolidColorBrush(Color.FromArgb(64, 0, 0, 0));
                                border.Background = mySolidColorBrush;
                                border.BorderThickness = new Thickness(drop_target_width);
                                border.BorderBrush = Brushes.Magenta;
                                // 領地アイコンの大きさが不明なので、大きめの枠にする
                                border.Width = 64;
                                border.Height = 64;
                                border.Margin = new Thickness()
                                {
                                    Left = itemSpot.X - 32,
                                    Top = itemSpot.Y - 32
                                };
                                worldMap.canvasMap.Children.Add(border);
                                Panel.SetZIndex(border, 2);
                            }
                        }
                    }
                }
            }
        }

        // 部隊をドラッグ移動する際に、移動先を作る
        private void MakeDropTarget_Troop(MainWindow mainWindow, int troop_id)
        {
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;
            int member_capacity = mainWindow.ClassGameStatus.ListClassScenarioInfo[mainWindow.ClassGameStatus.NumberScenarioSelection].MemberCapacity;
            int spot_capacity = classPowerAndCity.ClassSpot.Capacity;

            var listTroop = classPowerAndCity.ClassSpot.UnitGroup;
            int i = 0, troop_count, member_count, src_member_count;
            troop_count = listTroop.Count;
            src_member_count = listTroop[troop_id].ListClassUnit.Count; // ドラッグ移動中の部隊に所属するユニット数
            foreach (var itemTroop in listTroop)
            {
                member_count = itemTroop.ListClassUnit.Count;
                if (i == troop_id)
                {
                    // ドラッグ中の部隊の位置を暗くする
                    Border border = new Border();
                    border.Name = "DropTarget";
                    // 半透明のブラシを作成する (黒の50%)
                    SolidColorBrush mySolidColorBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
                    border.Background = mySolidColorBrush;
                    border.BorderThickness = new Thickness(0);
                    border.Width = tile_width * member_count;
                    border.Height = tile_height;
                    this.canvasSpotUnit.Children.Add(border);
                    Canvas.SetLeft(border, header_width);
                    Canvas.SetTop(border, tile_height * i);
                }
                else if (member_count + src_member_count <= member_capacity)
                {
                    // 右の空きスペースなら「部隊を統合」動作になる
                    Border border = new Border();
                    border.Name = "DropTarget" + this.Name + "_Right_" + i.ToString();
                    border.Background = Brushes.Transparent;
                    border.BorderThickness = new Thickness(drop_target_width);
                    border.BorderBrush = Brushes.Magenta;
                    border.Width = tile_width * (member_capacity - member_count) - 1;
                    border.Height = tile_height - 1;
                    this.canvasSpotUnit.Children.Add(border);
                    Canvas.SetLeft(border, header_width + tile_width * member_count + 1);
                    Canvas.SetTop(border, tile_height * i);
                }

                // 先頭ユニットの上端なら「部隊間に部隊を移動」動作になる
                if ((troop_count < spot_capacity) && (i != troop_id) && (i != troop_id + 1))
                {
                    Border border = new Border();
                    border.Name = "DropTarget" + this.Name + "_Top_" + i.ToString();
                    border.Background = Brushes.Transparent;
                    border.BorderThickness = new Thickness(drop_target_width);
                    border.BorderBrush = Brushes.Magenta;
                    border.Width = header_width + tile_width;
                    border.Height = tile_height / 3;
                    this.canvasSpotUnit.Children.Add(border);
                    Canvas.SetTop(border, tile_height * i);
                }

                i++;
            }

            // 下の空きスペースなら「部隊を末尾に移動」動作になる
            if ((i < spot_capacity) && (troop_id < i - 1))
            {
                Border border = new Border();
                border.Name = "DropTarget" + this.Name + "_Bottom";
                border.Background = Brushes.Transparent;
                border.BorderThickness = new Thickness(drop_target_width);
                border.BorderBrush = Brushes.Magenta;
                border.Width = header_width + tile_width * member_capacity;
                border.Height = tile_height * (spot_capacity - i);
                this.canvasSpotUnit.Children.Add(border);
                Canvas.SetTop(border, tile_height * i);
            }

            // 他の領地ウインドウを探す
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
            {
                string strTitle = itemWindow.Name;
                if ((strTitle.StartsWith(StringName.windowSpot)) && (strTitle != this.Name))
                {
                    var targetPowerAndCity = (ClassPowerAndCity)itemWindow.Tag;
                    // 同じ勢力の領地ウインドウだけ対象にする
                    if (targetPowerAndCity.ClassPower.NameTag == classPowerAndCity.ClassPower.NameTag)
                    {
                        // 領地の部隊情報を調べる
                        listTroop = targetPowerAndCity.ClassSpot.UnitGroup;
                        spot_capacity = targetPowerAndCity.ClassSpot.Capacity;
                        i = 0;
                        troop_count = listTroop.Count;
                        foreach (var itemTroop in listTroop)
                        {
                            member_count = itemTroop.ListClassUnit.Count;
                            if (member_count + src_member_count <= member_capacity)
                            {
                                // 右の空きスペースなら「部隊を統合」動作になる
                                Border border = new Border();
                                border.Name = "DropTarget" + strTitle + "_Right_" + i.ToString();
                                border.Background = Brushes.Transparent;
                                border.BorderThickness = new Thickness(drop_target_width);
                                border.BorderBrush = Brushes.Magenta;
                                border.Width = tile_width * (member_capacity - member_count) - 1;
                                border.Height = tile_height - 1;
                                itemWindow.canvasSpotUnit.Children.Add(border);
                                Canvas.SetLeft(border, header_width + tile_width * member_count + 1);
                                Canvas.SetTop(border, tile_height * i);
                            }

                            // 先頭ユニットの上端なら「部隊間に部隊を移動」動作になる
                            if (troop_count < spot_capacity)
                            {
                                Border border = new Border();
                                border.Name = "DropTarget" + strTitle + "_Top_" + i.ToString();
                                border.Background = Brushes.Transparent;
                                border.BorderThickness = new Thickness(drop_target_width);
                                border.BorderBrush = Brushes.Magenta;
                                border.Width = header_width + tile_width;
                                border.Height = tile_height / 3;
                                itemWindow.canvasSpotUnit.Children.Add(border);
                                Canvas.SetTop(border, tile_height * i);
                            }

                            i++;
                        }

                        // 下の空きスペースなら「部隊を末尾に移動」動作になる
                        if (i < spot_capacity)
                        {
                            Border border = new Border();
                            border.Name = "DropTarget" + strTitle + "_Bottom";
                            border.Background = Brushes.Transparent;
                            border.BorderThickness = new Thickness(drop_target_width);
                            border.BorderBrush = Brushes.Magenta;
                            border.Width = header_width + tile_width * member_capacity;
                            border.Height = tile_height * (spot_capacity - i);
                            itemWindow.canvasSpotUnit.Children.Add(border);
                            Canvas.SetTop(border, tile_height * i);
                        }
                    }
                }
            }

            // 戦略マップ上の領地アイコンも
            if (mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.ListMember.Count > 1)
            {
                var worldMap = mainWindow.ClassGameStatus.WorldMap;
                if (worldMap != null)
                {
                    var listSpot = mainWindow.ClassGameStatus.NowListSpot;
                    int spot_count = listSpot.Count;
                    for (int spot_id = 0; spot_id < spot_count; spot_id++)
                    {
                        // 同じ勢力の領地だけ
                        ClassSpot itemSpot = listSpot[spot_id];
                        if (itemSpot.PowerNameTag == classPowerAndCity.ClassPower.NameTag)
                        {
                            // 領地の部隊数に空きがあるなら
                            spot_capacity = itemSpot.Capacity;
                            troop_count = itemSpot.UnitGroup.Count;
                            if ((troop_count < spot_capacity) && (itemSpot.NameTag != classPowerAndCity.ClassSpot.NameTag))
                            {
                                Border border = new Border();
                                // 領地リストのインデックスで識別する
                                border.Name = "DropTargetSpot_" + spot_id.ToString();
                                // ワールドマップ上で識別しやすいよう、背景を少し暗くする
                                SolidColorBrush mySolidColorBrush = new SolidColorBrush(Color.FromArgb(64, 0, 0, 0));
                                border.Background = mySolidColorBrush;
                                border.BorderThickness = new Thickness(drop_target_width);
                                border.BorderBrush = Brushes.Magenta;
                                // 親コントロールが Grid なので、標準だと中央配置になる。左上を原点に変えておく。
                                border.HorizontalAlignment = HorizontalAlignment.Left;
                                border.VerticalAlignment = VerticalAlignment.Top;
                                // 領地アイコンの大きさが不明なので、大きめの枠にする
                                border.Width = 64;
                                border.Height = 64;
                                border.Margin = new Thickness()
                                {
                                    Left = itemSpot.X - 32,
                                    Top = itemSpot.Y - 32
                                };
                                worldMap.canvasMap.Children.Add(border);
                                Panel.SetZIndex(border, 2);
                            }
                        }
                    }
                }
            }
        }

        // 全ての部隊をドラッグ移動する際に、移動先を作る
        private bool MakeDropTarget_Whole(MainWindow mainWindow)
        {
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;
            int member_capacity = mainWindow.ClassGameStatus.ListClassScenarioInfo[mainWindow.ClassGameStatus.NumberScenarioSelection].MemberCapacity;

            var listTroop = classPowerAndCity.ClassSpot.UnitGroup;
            int i, troop_count, spot_capacity;
            troop_count = listTroop.Count;

            // ドラッグ中の部隊の位置を暗くする
            if (troop_count > 0)
            {
                Border border = new Border();
                border.Name = "DropTarget";
                // 半透明のブラシを作成する (黒の50%)
                SolidColorBrush mySolidColorBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 0));
                border.Background = mySolidColorBrush;
                border.BorderThickness = new Thickness(0);
                border.Width = tile_width * member_capacity;
                border.Height = tile_height * troop_count;
                this.canvasSpotUnit.Children.Add(border);
                Canvas.SetLeft(border, header_width);
            }
            else
            {
                // ドラッグする部隊が存在しない
                return false;
            }

            // 他の領地ウインドウを探す
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
            {
                string strTitle = itemWindow.Name;
                if ((strTitle.StartsWith(StringName.windowSpot)) && (strTitle != this.Name))
                {
                    var targetPowerAndCity = (ClassPowerAndCity)itemWindow.Tag;
                    // 同じ勢力の領地ウインドウだけ対象にする
                    if (targetPowerAndCity.ClassPower.NameTag == classPowerAndCity.ClassPower.NameTag)
                    {
                        // 領地の部隊情報を調べる
                        listTroop = targetPowerAndCity.ClassSpot.UnitGroup;
                        spot_capacity = targetPowerAndCity.ClassSpot.Capacity;
                        i = 0;
                        troop_count = listTroop.Count;
                        foreach (var itemTroop in listTroop)
                        {
                            // 先頭ユニットの上端なら「部隊間に部隊を移動」動作になる
                            if (troop_count < spot_capacity)
                            {
                                Border border = new Border();
                                border.Name = "DropTarget" + strTitle + "_Top_" + i.ToString();
                                border.Background = Brushes.Transparent;
                                border.BorderThickness = new Thickness(drop_target_width);
                                border.BorderBrush = Brushes.Magenta;
                                border.Width = header_width + tile_width;
                                border.Height = tile_height / 3;
                                itemWindow.canvasSpotUnit.Children.Add(border);
                                Canvas.SetTop(border, tile_height * i);
                            }

                            i++;
                        }

                        // 下の空きスペースなら「部隊を末尾に移動」動作になる
                        if (i < spot_capacity)
                        {
                            Border border = new Border();
                            border.Name = "DropTarget" + strTitle + "_Bottom";
                            border.Background = Brushes.Transparent;
                            border.BorderThickness = new Thickness(drop_target_width);
                            border.BorderBrush = Brushes.Magenta;
                            border.Width = header_width + tile_width * member_capacity;
                            border.Height = tile_height * (spot_capacity - i);
                            itemWindow.canvasSpotUnit.Children.Add(border);
                            Canvas.SetTop(border, tile_height * i);
                        }
                    }
                }
            }

            // 戦略マップ上の領地アイコンも
            if (mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.ListMember.Count > 1)
            {
                var worldMap = mainWindow.ClassGameStatus.WorldMap;
                if (worldMap != null)
                {
                    var listSpot = mainWindow.ClassGameStatus.NowListSpot;
                    int spot_count = listSpot.Count;
                    for (int spot_id = 0; spot_id < spot_count; spot_id++)
                    {
                        // 同じ勢力の領地だけ
                        ClassSpot itemSpot = listSpot[spot_id];
                        if (itemSpot.PowerNameTag == classPowerAndCity.ClassPower.NameTag)
                        {
                            // 領地の部隊数に空きがあるなら
                            spot_capacity = itemSpot.Capacity;
                            troop_count = itemSpot.UnitGroup.Count;
                            if ((troop_count < spot_capacity) && (itemSpot.NameTag != classPowerAndCity.ClassSpot.NameTag))
                            {
                                Border border = new Border();
                                // 領地リストのインデックスで識別する
                                border.Name = "DropTargetSpot_" + spot_id.ToString();
                                // ワールドマップ上で識別しやすいよう、背景を少し暗くする
                                SolidColorBrush mySolidColorBrush = new SolidColorBrush(Color.FromArgb(64, 0, 0, 0));
                                border.Background = mySolidColorBrush;
                                border.BorderThickness = new Thickness(drop_target_width);
                                border.BorderBrush = Brushes.Magenta;
                                // 親コントロールが Grid なので、標準だと中央配置になる。左上を原点に変えておく。
                                border.HorizontalAlignment = HorizontalAlignment.Left;
                                border.VerticalAlignment = VerticalAlignment.Top;
                                // 領地アイコンの大きさが不明なので、大きめの枠にする
                                border.Width = 64;
                                border.Height = 64;
                                border.Margin = new Thickness()
                                {
                                    Left = itemSpot.X - 32,
                                    Top = itemSpot.Y - 32
                                };
                                worldMap.canvasMap.Children.Add(border);
                                Panel.SetZIndex(border, 2);
                            }
                        }
                    }
                }
            }

            return true;
        }

        // ユニットをドラッグ移動した後に、移動先を取り除く
        // 部隊（隊長）をドラッグする場合は、member_id を 0 にすること
        // 全部隊（全部ボタン）をドラッグする場合は、troop_id を -1 にすること
        private int RemoveDropTarget(MainWindow mainWindow, int troop_id, int member_id)
        {
            int moved_result = 0;

            // 領地ウインドウのユニット欄に追加した枠を消去する
            for (int i = this.canvasSpotUnit.Children.Count - 1; i >= 0; i += -1)
            {
                UIElement Child = this.canvasSpotUnit.Children[i];
                if (Child is Border)
                {
                    var border = (Border)Child;
                    string strTarget = border.Name;
                    if (strTarget.StartsWith("DropTarget"))
                    {
                        // 枠が太くなっていれば、選択中の印
                        if (border.BorderThickness.Left > drop_target_width)
                        {
                            // 同じ領地にドロップできるのはユニットか部隊だけなので、全部隊は判定しない
                            strTarget = strTarget.Replace("DropTarget", String.Empty);
                            if (member_id >= 1)
                            {
                                // 部下なら必ず member_id が 1以上になる
                                if (DropTarget_Unit(mainWindow, troop_id, member_id, strTarget))
                                {
                                    // 領地を更新した場合は枠も消えるので、ループから抜ける
                                    break;
                                }
                            }
                            else
                            {
                                // 隊長なら member_id は常に 0 なので、省略する
                                if (DropTarget_Troop(mainWindow, troop_id, strTarget))
                                {
                                    // 領地を更新した場合は枠も消えるので、ループから抜ける
                                    break;
                                }
                            }
                        }
                        // 枠を取り除く
                        this.canvasSpotUnit.Children.Remove(border);
                    }
                }
            }

            // 他の領地の枠も
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
            {
                string strTitle = itemWindow.Name;
                if ((strTitle.StartsWith(StringName.windowSpot)) && (strTitle != this.Name))
                {
                    var classPowerAndCity = (ClassPowerAndCity)this.Tag;
                    var targetPowerAndCity = (ClassPowerAndCity)itemWindow.Tag;
                    // 同じ勢力の領地ウインドウだけ対象にする
                    if (targetPowerAndCity.ClassPower.NameTag == classPowerAndCity.ClassPower.NameTag)
                    {
                        for (int i = itemWindow.canvasSpotUnit.Children.Count - 1; i >= 0; i += -1)
                        {
                            UIElement Child = itemWindow.canvasSpotUnit.Children[i];
                            if (Child is Border)
                            {
                                var border = (Border)Child;
                                string strTarget = border.Name;
                                if (strTarget.StartsWith("DropTarget"))
                                {
                                    // 枠が太くなっていれば、選択中の印
                                    if (border.BorderThickness.Left > drop_target_width)
                                    {
                                        strTarget = strTarget.Replace("DropTarget", String.Empty);
                                        if (troop_id >= 0)
                                        {
                                            if (member_id >= 1)
                                            {
                                                // 部下なら必ず member_id が 1以上になる
                                                if (DropTarget_Unit(mainWindow, troop_id, member_id, strTarget))
                                                {
                                                    // 領地を更新した場合は枠も消えるので、ループから抜ける
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                // 隊長なら member_id は常に 0 なので、省略する
                                                if (DropTarget_Troop(mainWindow, troop_id, strTarget))
                                                {
                                                    // 領地を更新した場合は枠も消えるので、ループから抜ける
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // 全部隊
                                            moved_result = DropTarget_Whole(mainWindow, strTarget);
                                            if (moved_result != 0)
                                            {
                                                // 領地を更新した場合は枠も消えるので、ループから抜ける
                                                break;
                                            }
                                        }
                                    }
                                    // 枠を取り除く
                                    itemWindow.canvasSpotUnit.Children.Remove(border);
                                }
                            }
                        }
                    }
                }
            }

            // 戦略マップ上の枠も
            if (mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.ListMember.Count > 1)
            {
                var worldMap = mainWindow.ClassGameStatus.WorldMap;
                if (worldMap != null)
                {
                    for (int i = worldMap.canvasMap.Children.Count - 1; i >= 0; i += -1)
                    {
                        UIElement Child = worldMap.canvasMap.Children[i];
                        if (Child is Border)
                        {
                            var border = (Border)Child;
                            string strTarget = border.Name;
                            if (strTarget.StartsWith("DropTarget"))
                            {
                                // 枠が太くなっていれば、選択中の印
                                if (border.BorderThickness.Left > drop_target_width)
                                {
                                    strTarget = strTarget.Replace("DropTarget", String.Empty);
                                    if (troop_id >= 0)
                                    {
                                        if (member_id >= 1)
                                        {
                                            // 部下なら必ず member_id が 1以上になる
                                            DropTarget_Unit(mainWindow, troop_id, member_id, strTarget);
                                        }
                                        else
                                        {
                                            // 隊長なら member_id は常に 0 なので、省略する
                                            DropTarget_Troop(mainWindow, troop_id, strTarget);
                                        }
                                    }
                                    else
                                    {
                                        // 全部隊
                                        moved_result = DropTarget_Whole(mainWindow, strTarget);
                                    }
                                }
                                // 枠を取り除く
                                worldMap.canvasMap.Children.Remove(border);
                            }
                        }
                    }
                }
            }

            return moved_result;
        }

        /*
        ドラッグ画像を IsHitTestVisible = false にすれば
        InputHitTest でその下の物を検出できるので、そちらを使うことにしました。
        他の用途で使うかもしれないので、実装例としてコードは残しておきます。

        // ドラッグ中にドロップ先を判定するための HitTest 用
        private DependencyObject? _hitResults = null;
        private int _hitCount = 0;
        private HitTestResultBehavior OnHitTestResultCallback(HitTestResult result)
        {
            _hitCount++;

            // Borderを見つけたら終了する
            if (result.VisualHit.GetType() == typeof(Border))
            {
                _hitResults = result.VisualHit;
                return HitTestResultBehavior.Stop;
            }

            // 上位２個まで調べる
            // ドラッグ画像のImage、ドロップ先のBorder
            if (_hitCount >= 2)
            {
                return HitTestResultBehavior.Stop;
            }

            return HitTestResultBehavior.Continue;
        }
        */

        // ユニットをドラッグ移動中に、移動先をホバー表示する
        private void HoverDropTarget(MainWindow mainWindow, Point posMouse)
        {
            // マウスポインタ―の下にあるなら、最初の枠だけ太くする
            Border? borderHit = null;
/*
            _hitResults = null;
            _hitCount = 0;

            VisualTreeHelper.HitTest(mainWindow
                    , null
                    , new HitTestResultCallback(OnHitTestResultCallback)
                    , new PointHitTestParameters(posMouse));

            if (_hitResults != null)
            {
                borderHit = (Border)_hitResults;
*/
            IInputElement? hitResult = mainWindow.InputHitTest(posMouse);
            if ((hitResult != null) && (hitResult is Border))
            {
                borderHit = (Border)hitResult;
                if (borderHit.Name.StartsWith("DropTarget"))
                {
                    if (borderHit.BorderThickness.Left == drop_target_width)
                    {
                        borderHit.BorderThickness = new Thickness(drop_select_width);
                        borderHit.BorderBrush = Brushes.Aqua;
                    }
                }
            }

            // それ以外の枠が太ければ普通に戻す
            foreach (var border in this.canvasSpotUnit.Children.OfType<Border>())
            {
                if ((border != borderHit) && (border.Name.StartsWith("DropTarget")))
                {
                    if (border.BorderThickness.Left > drop_target_width)
                    {
                        border.BorderThickness = new Thickness(drop_target_width);
                        border.BorderBrush = Brushes.Magenta;
                    }
                }
            }

            // 他の領地の枠も
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
            {
                string strTitle = itemWindow.Name;
                if ((strTitle.StartsWith(StringName.windowSpot)) && (strTitle != this.Name))
                {
                    var classPowerAndCity = (ClassPowerAndCity)this.Tag;
                    var targetPowerAndCity = (ClassPowerAndCity)itemWindow.Tag;
                    // 同じ勢力の領地ウインドウだけ対象にする
                    if (targetPowerAndCity.ClassPower.NameTag == classPowerAndCity.ClassPower.NameTag)
                    {
                        foreach (var border in itemWindow.canvasSpotUnit.Children.OfType<Border>())
                        {
                            if ((border != borderHit) && (border.Name.StartsWith("DropTarget")))
                            {
                                if (border.BorderThickness.Left > drop_target_width)
                                {
                                    border.BorderThickness = new Thickness(drop_target_width);
                                    border.BorderBrush = Brushes.Magenta;
                                }
                            }
                        }
                    }
                }
            }

            // 戦略マップ上の枠も
            if (mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.ListMember.Count > 1)
            {
                var worldMap = mainWindow.ClassGameStatus.WorldMap;
                if (worldMap != null)
                {
                    foreach (var border in worldMap.canvasMap.Children.OfType<Border>())
                    {
                        if ((border != borderHit) && (border.Name.StartsWith("DropTarget")))
                        {
                            if (border.BorderThickness.Left > drop_target_width)
                            {
                                border.BorderThickness = new Thickness(drop_target_width);
                                border.BorderBrush = Brushes.Magenta;
                            }
                        }
                    }
                }
            }
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 雇用ウインドウを開いてた場合は閉じる
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl020_Mercenary>())
            {
                if (itemWindow.Name == this.Name + "Mercenary")
                {
                    mainWindow.canvasUI.Children.Remove(itemWindow);
                    break;
                }
            }

            // ワールドマップ上での強調を解除する
            var worldMap = mainWindow.ClassGameStatus.WorldMap;
            if (worldMap != null)
            {
                worldMap.RemoveSpotMark(((ClassPowerAndCity)this.Tag).ClassSpot.NameTag);
            }

            // キャンバスから自身を取り除く
            mainWindow.canvasUI.Children.Remove(this);

            // ルーティングを処理済みとしてマークする（親コントロールのイベントが発生しなくなる）
            e.Handled = true;
        }

        #region ウインドウ移動
        private bool _isDrag = false; // 外部に公開する必要なし
        private Point _startPoint;

        private void win_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow != null)
            {
                // 最前面に移動させる
                var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
                if ((listWindow != null) && (listWindow.Any()))
                {
                    int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                    Canvas.SetZIndex(this, maxZ + 1);
                }
            }

            // ドラッグを開始する
            UIElement el = (UIElement)sender;
            if (el != null)
            {
                _isDrag = true;
                _startPoint = e.GetPosition(el);
                el.CaptureMouse();
                el.MouseLeftButtonUp += win_MouseLeftButtonUp;
                el.MouseMove += win_MouseMove;
            }
        }
        private void win_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // ドラック中なら終了する
            if (_isDrag == true)
            {
                UIElement el = (UIElement)sender;
                el.ReleaseMouseCapture();
                el.MouseLeftButtonUp -= win_MouseLeftButtonUp;
                el.MouseMove -= win_MouseMove;
                _isDrag = false;
            }
        }
        private void win_MouseMove(object sender, MouseEventArgs e)
        {
            // ドラック中
            if (_isDrag == true)
            {
                UIElement el = (UIElement)sender;
                Point pt = e.GetPosition(el);

                var thickness = new Thickness();
                thickness.Left = this.Margin.Left + (pt.X - _startPoint.X);
                thickness.Top = this.Margin.Top + (pt.Y - _startPoint.Y);
                this.Margin = thickness;
            }
        }
        #endregion

        // ボタン等を右クリックした際に、親コントロールが反応しないようにする
        private void Disable_MouseEvent(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow != null)
            {
                // 最前面に移動させる
                var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
                if ((listWindow != null) && (listWindow.Any()))
                {
                    int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                    Canvas.SetZIndex(this, maxZ + 1);
                }
            }

            // ルーティングを処理済みとしてマークする（親コントロールのイベントが発生しなくなる）
            e.Handled = true;
        }

        // ボタン等をクリックした際に、UserControlを最前面に移動させる
        private void Raise_ZOrder(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow != null)
            {
                // 最前面に移動させる
                var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
                if ((listWindow != null) && (listWindow.Any()))
                {
                    int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                    Canvas.SetZIndex(this, maxZ + 1);
                }
            }
        }

        // マウスカーソルが上に来たらユニットのパネルの色を変える
        private void panel_MouseEnter(object sender, MouseEventArgs e)
        {
            var panelUnit = (StackPanel)sender;
            // WPFの標準色をどうやって取得するのか知らないので、暗い色にする
            panelUnit.Background = Brushes.Gray;

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow != null)
            {
                // 場所が重なるのでヘルプを全て隠す
                foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
                {
                    if ((itemHelp.Visibility == Visibility.Visible) && (itemHelp.Name.StartsWith("Help_") == true))
                    {
                        itemHelp.Visibility = Visibility.Hidden;
                    }
                }

                // メンバーにできるユニットを表示する
                var helpMember = new UserControl031_HelpMember();
                helpMember.Name = StringName.windowMember;
                helpMember.Tag = panelUnit.Tag;
                helpMember.SetData();
                mainWindow.canvasUI.Children.Add(helpMember);

                // ユニットが人材なら
                if (((ClassUnit)panelUnit.Tag).Talent == "on")
                {
                    ClassCityAndUnit classCityAndUnit = new ClassCityAndUnit();
                    classCityAndUnit.ClassPowerAndCity = (ClassPowerAndCity)this.Tag;
                    classCityAndUnit.ClassUnit = (ClassUnit)panelUnit.Tag;

                    // ユニット情報のヒントを表示する
                    var hintUnit = new UserControl016_UnitHint();
                    hintUnit.Tag = classCityAndUnit;
                    hintUnit.Name = StringName.windowUnitHint;
                    hintUnit.SetData();
                    mainWindow.canvasUI.Children.Add(hintUnit);
                }
            }
        }
        private void panel_MouseLeave(object sender, MouseEventArgs e)
        {
            var panelUnit = (StackPanel)sender;
            // 背景の設定自体を消去する
            panelUnit.Background = null;

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow != null)
            {
                // メンバーのヘルプを閉じる
                foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl031_HelpMember>())
                {
                    if (itemWindow.Name == StringName.windowMember)
                    {
                        mainWindow.canvasUI.Children.Remove(itemWindow);
                        break;
                    }
                }

                // ユニットが人材なら
                if (((ClassUnit)panelUnit.Tag).Talent == "on")
                {
                    // ユニット情報のヒントを閉じる
                    foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl016_UnitHint>())
                    {
                        if (itemWindow.Name == StringName.windowUnitHint)
                        {
                            mainWindow.canvasUI.Children.Remove(itemWindow);
                            break;
                        }
                    }
                }

                // ヘルプを隠してた場合は、最前面のヘルプだけ表示する
                int maxZ = -1, thisZ;
                foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
                {
                    if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                    {
                        thisZ = Canvas.GetZIndex(itemHelp);
                        if (maxZ < thisZ)
                        {
                            maxZ = thisZ;
                        }
                    }
                }
                if (maxZ >= 0)
                {
                    foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
                    {
                        if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                        {
                            if (Canvas.GetZIndex(itemHelp) == maxZ)
                            {
                                itemHelp.Visibility = Visibility.Visible;
                                break;
                            }
                        }
                    }
                }
            }
        }

        #region ユニットのドラッグ移動
        private bool _isDragUnit = false;

        private void unit_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // ルーティングを処理済みとしてマークする（親コントロールのイベントが発生しなくなる）
            e.Handled = true;

            // 最前面に移動させる
            var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
            if ((listWindow != null) && (listWindow.Any()))
            {
                int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                Canvas.SetZIndex(this, maxZ + 1);
            }

            // ドラッグを開始する
            UIElement el = (UIElement)sender;
            if (el != null)
            {
                // ドロップ先を作る
                string unit_name = ((StackPanel)sender).Name;
                string unit_id = unit_name.Replace("panelUnit", String.Empty);
                string[] strPart = unit_id.Split('_');
                int troop_id = Int32.Parse(strPart[0]);
                int member_id = Int32.Parse(strPart[1]);
                MakeDropTarget_Unit(mainWindow, troop_id, member_id);

                // ユニット画像をそのまま流用する
                BitmapImage? bitimg1 = null;
                var ri = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasSpotUnit, "imgUnit" + unit_id);
                if (ri != null)
                {
                    bitimg1 = (BitmapImage)ri.Source;
                }
                Image imgDrag = new Image();
                imgDrag.Source = bitimg1;
                imgDrag.Width = tile_width;
                imgDrag.Height = tile_width;
                imgDrag.Name = "DragImage" + troop_id.ToString() + "_" + member_id.ToString();
                imgDrag.IsHitTestVisible = false;
                mainWindow.canvasUI.Children.Add(imgDrag);

                // ドラッグ画像をウインドウよりも前面に移動させる
                Canvas.SetZIndex(imgDrag, Canvas.GetZIndex(this) + 1);

                // ボタンの位置に置く
                Point pos = el.TranslatePoint(new Point(0, 0), mainWindow.canvasUI);
                Canvas.SetLeft(imgDrag, pos.X);
                Canvas.SetTop(imgDrag, pos.Y);

                _startPoint = e.GetPosition(el);
                _isDragUnit = true;

                // ドラッグ画像に対して、イベントを追加する
                imgDrag.CaptureMouse();
                imgDrag.MouseRightButtonUp += unit_MouseRightButtonUp;
                imgDrag.MouseMove += unit_MouseMove;
            }
        }
        private void unit_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // ドラック中なら終了する
            if (_isDragUnit == true)
            {
                UIElement el = (UIElement)sender;
                el.ReleaseMouseCapture();
                el.MouseRightButtonUp -= unit_MouseRightButtonUp;
                el.MouseMove -= unit_MouseMove;
                _isDragUnit = false;

                var mainWindow = (MainWindow)Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    // ドロップ先を取り除くと同時に、ドロップ判定を行う
                    string unit_name = ((Image)sender).Name;
                    string unit_id = unit_name.Replace("DragImage", String.Empty);
                    string[] strPart = unit_id.Split('_');
                    int troop_id = Int32.Parse(strPart[0]);
                    int member_id = Int32.Parse(strPart[1]);
                    RemoveDropTarget(mainWindow, troop_id, member_id);

                    // ドラッグ画像を取り除く
                    mainWindow.canvasUI.Children.Remove(el);
                }
            }
        }
        private void unit_MouseMove(object sender, MouseEventArgs e)
        {
            // ドラック中
            if (_isDragUnit == true)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    // ドロップ先をホバー表示する
                    HoverDropTarget(mainWindow, e.GetPosition(mainWindow));
                }

                UIElement el = (UIElement)sender;
                Point pt = e.GetPosition(el);

                // ドラッグ量に応じて子コントロールを移動する
                Canvas.SetLeft(el, Canvas.GetLeft(el) + (pt.X - _startPoint.X));
                Canvas.SetTop(el, Canvas.GetTop(el) + (pt.Y - _startPoint.Y));
            }
        }

        // 部隊の隊長を選択した場合は、部隊メンバー全てが移動する
        private void troop_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // ルーティングを処理済みとしてマークする（親コントロールのイベントが発生しなくなる）
            e.Handled = true;

            // 最前面に移動させる
            var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
            if ((listWindow != null) && (listWindow.Any()))
            {
                int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                Canvas.SetZIndex(this, maxZ + 1);
            }

            // ドラッグを開始する
            UIElement el = (UIElement)sender;
            if (el != null)
            {
                // ドロップ先を作る
                string unit_name = ((StackPanel)sender).Name;
                string unit_id = unit_name.Replace("panelUnit", String.Empty);
                string[] strPart = unit_id.Split('_');
                int troop_id = Int32.Parse(strPart[0]);
                MakeDropTarget_Troop(mainWindow, troop_id);

                // ドラッグ移動中の部隊に所属するユニット数
                var classPowerAndCity = (ClassPowerAndCity)this.Tag;
                var listTroop = classPowerAndCity.ClassSpot.UnitGroup;
                int member_count = listTroop[troop_id].ListClassUnit.Count;

                // 隊長のユニット画像
                BitmapImage? bitimg1 = null;
                var ri = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasSpotUnit, "imgUnit" + unit_id);
                if (ri != null)
                {
                    bitimg1 = (BitmapImage)ri.Source;
                }
                Image imgDrag = new Image();
                imgDrag.Source = bitimg1;
                imgDrag.Width = tile_width;
                imgDrag.Height = tile_width;
                imgDrag.Name = "DragImage" + troop_id.ToString() + "_" + member_count.ToString();
                imgDrag.IsHitTestVisible = false;
                mainWindow.canvasUI.Children.Add(imgDrag);

                // ドラッグ画像をウインドウよりも前面に移動させる
                Canvas.SetZIndex(imgDrag, Canvas.GetZIndex(this) + 1);

                // ボタンの位置に置く
                Point pos = el.TranslatePoint(new Point(0, 0), mainWindow.canvasUI);
                Canvas.SetLeft(imgDrag, pos.X);
                Canvas.SetTop(imgDrag, pos.Y);

                // 部下のユニット画像
                for (int j = 1; j < member_count; j++)
                {
                    var ri2 = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasSpotUnit, "imgUnit" + troop_id.ToString() + "_" + j.ToString());
                    if (ri2 != null)
                    {
                        bitimg1 = (BitmapImage)ri2.Source;
                    }
                    else
                    {
                        break;
                    }
                    var imgDrag2 = new Image();
                    imgDrag2.Source = bitimg1;
                    imgDrag2.Width = tile_width;
                    imgDrag2.Height = tile_width;
                    imgDrag2.Name = "DragImageExtra" + j.ToString();
                    mainWindow.canvasUI.Children.Add(imgDrag2);

                    // 隊長の画像の右横に並べる
                    Canvas.SetZIndex(imgDrag2, Canvas.GetZIndex(this) + 1);
                    Canvas.SetLeft(imgDrag2, pos.X + tile_width * j);
                    Canvas.SetTop(imgDrag2, pos.Y);
                }

                _startPoint = e.GetPosition(el);
                _isDragUnit = true;

                // ドラッグ画像に対して、イベントを追加する
                imgDrag.CaptureMouse();
                imgDrag.MouseRightButtonUp += troop_MouseRightButtonUp;
                imgDrag.MouseMove += troop_MouseMove;
            }
        }
        private void troop_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // ドラック中なら終了する
            if (_isDragUnit == true)
            {
                UIElement el = (UIElement)sender;
                el.ReleaseMouseCapture();
                el.MouseRightButtonUp -= unit_MouseRightButtonUp;
                el.MouseMove -= unit_MouseMove;
                _isDragUnit = false;

                var mainWindow = (MainWindow)Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    // ドロップ先を取り除くと同時に、ドロップ判定を行う
                    string unit_name = ((Image)sender).Name;
                    string unit_id = unit_name.Replace("DragImage", String.Empty);
                    string[] strPart = unit_id.Split('_');
                    int troop_id = Int32.Parse(strPart[0]);
                    int member_count = Int32.Parse(strPart[1]);
                    RemoveDropTarget(mainWindow, troop_id, 0);

                    // ドラッグ画像を取り除く
                    mainWindow.canvasUI.Children.Remove(el);
                    for (int j = 1; j < member_count; j++)
                    {
                        var ri = (Image)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, "DragImageExtra" + j.ToString());
                        if (ri != null)
                        {
                            mainWindow.canvasUI.Children.Remove(ri);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
        private void troop_MouseMove(object sender, MouseEventArgs e)
        {
            // ドラック中
            if (_isDragUnit == true)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    // ドロップ先をホバー表示する
                    HoverDropTarget(mainWindow, e.GetPosition(mainWindow));
                }

                UIElement el = (UIElement)sender;
                Point pt = e.GetPosition(el);

                // ドラッグ量に応じて子コントロールを移動する
                Canvas.SetLeft(el, Canvas.GetLeft(el) + (pt.X - _startPoint.X));
                Canvas.SetTop(el, Canvas.GetTop(el) + (pt.Y - _startPoint.Y));

                // 右横の子コントロールも同時に移動させる
                string unit_name = ((Image)sender).Name;
                string unit_id = unit_name.Replace("DragImage", String.Empty);
                string[] strPart = unit_id.Split('_');
                int member_count = Int32.Parse(strPart[1]);
                if (mainWindow == null)
                {
                    return;
                }
                for (int j = 1; j < member_count; j++)
                {
                    var ri = (Image)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, "DragImageExtra" + j.ToString());
                    if (ri != null)
                    {
                        Canvas.SetLeft(ri, Canvas.GetLeft(ri) + (pt.X - _startPoint.X));
                        Canvas.SetTop(ri, Canvas.GetTop(ri) + (pt.Y - _startPoint.Y));
                    }
                    else
                    {
                        break;
                    }
                }

            }
        }

        // 全部ボタンを押した場合は、全ての部隊が移動する
        private void whole_MouseDown(object sender, MouseEventArgs e)
        {
            // ルーティングを処理済みとしてマークする（親コントロールのイベントが発生しなくなる）
            e.Handled = true;

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 最前面に移動させる
            var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
            if ((listWindow != null) && (listWindow.Any()))
            {
                int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                Canvas.SetZIndex(this, maxZ + 1);
            }

            // ドラッグを開始する
            UIElement el = (UIElement)sender;
            if (el != null)
            {
                // ドロップ先を作る
                if (MakeDropTarget_Whole(mainWindow) == false)
                {
                    // ドラッグする部隊が存在しない場合は、何もせずに終わる
                    return;
                }

                // ドラッグ移動中の部隊に所属するユニット数
                var classPowerAndCity = (ClassPowerAndCity)this.Tag;
                var listTroop = classPowerAndCity.ClassSpot.UnitGroup;
                int troop_count = listTroop.Count;

                // 一番上の隊長のユニット画像
                BitmapImage? bitimg1 = null;
                var ri = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasSpotUnit, "imgUnit0_0");
                if (ri != null)
                {
                    bitimg1 = (BitmapImage)ri.Source;
                }
                Image imgDrag = new Image();
                imgDrag.Source = bitimg1;
                imgDrag.Width = tile_width;
                imgDrag.Height = tile_width;
                imgDrag.Name = "DragImage" + troop_count.ToString();
                imgDrag.IsHitTestVisible = false;
                mainWindow.canvasUI.Children.Add(imgDrag);

                // ドラッグ画像をウインドウよりも前面に移動させる
                Canvas.SetZIndex(imgDrag, Canvas.GetZIndex(this) + 1);

                // 画像をマウス・カーソルの下に置く
                Point posMouse = e.GetPosition(mainWindow.canvasUI);
                Point pos = new Point(posMouse.X - tile_width / 2, posMouse.Y - tile_width / 2);
                Canvas.SetLeft(imgDrag, pos.X);
                Canvas.SetTop(imgDrag, pos.Y);

                // 残りの隊長のユニット画像
                for (int i = 1; i < troop_count; i++)
                {
                    var ri2 = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasSpotUnit, "imgUnit" + i.ToString() + "_0");
                    if (ri2 != null)
                    {
                        bitimg1 = (BitmapImage)ri2.Source;
                    }
                    else
                    {
                        break;
                    }
                    var imgDrag2 = new Image();
                    imgDrag2.Source = bitimg1;
                    imgDrag2.Width = tile_width;
                    imgDrag2.Height = tile_width;
                    imgDrag2.Name = "DragImageExtra" + i.ToString();
                    mainWindow.canvasUI.Children.Add(imgDrag2);

                    // 隊長の画像の下に並べる
                    Canvas.SetZIndex(imgDrag2, Canvas.GetZIndex(this) + 1);
                    Canvas.SetLeft(imgDrag2, pos.X);
                    Canvas.SetTop(imgDrag2, pos.Y + tile_width * i);
                }

                // マウス・カーソルからの相対位置にする
                _startPoint = new Point(tile_width / 2, tile_width / 2);
                _isDragUnit = true;

                // ドラッグ画像に対して、イベントを追加する
                imgDrag.CaptureMouse();
                imgDrag.MouseUp += whole_MouseUp;
                imgDrag.MouseMove += whole_MouseMove;
            }
        }
        private void whole_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // ドラック中なら終了する
            if (_isDragUnit == true)
            {
                UIElement el = (UIElement)sender;
                el.ReleaseMouseCapture();
                el.MouseUp -= whole_MouseUp;
                el.MouseMove -= whole_MouseMove;
                _isDragUnit = false;

                var mainWindow = (MainWindow)Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    // ドロップ先を取り除くと同時に、ドロップ判定を行う
                    string unit_name = ((Image)sender).Name;
                    string unit_id = unit_name.Replace("DragImage", String.Empty);
                    int troop_count = Int32.Parse(unit_id);
                    int moved_result = RemoveDropTarget(mainWindow, -1, 0);

                    // ドラッグ画像を取り除く
                    mainWindow.canvasUI.Children.Remove(el);
                    for (int i = 1; i < troop_count; i++)
                    {
                        var ri = (Image)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, "DragImageExtra" + i.ToString());
                        if (ri != null)
                        {
                            mainWindow.canvasUI.Children.Remove(ri);
                        }
                        else
                        {
                            break;
                        }
                    }

                    // 全部隊を移動できなかった場合は、何部隊が移動したかを通知する
                    if (moved_result > 0)
                    {
                        var dialog = new Win020_Dialog();
                        dialog.SetText(moved_result.ToString() + "部隊を移動しました。");
                        dialog.SetTime(1.2); // 待ち時間を1.2秒に短縮する
                        dialog.ShowDialog();
                    }
                }
            }
        }
        private void whole_MouseMove(object sender, MouseEventArgs e)
        {
            // ドラック中
            if (_isDragUnit == true)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    // ドロップ先をホバー表示する
                    HoverDropTarget(mainWindow, e.GetPosition(mainWindow));
                }

                UIElement el = (UIElement)sender;
                Point pt = e.GetPosition(el);

                // ドラッグ量に応じて子コントロールを移動する
                Canvas.SetLeft(el, Canvas.GetLeft(el) + (pt.X - _startPoint.X));
                Canvas.SetTop(el, Canvas.GetTop(el) + (pt.Y - _startPoint.Y));

                // 下の子コントロールも同時に移動させる
                string unit_name = ((Image)sender).Name;
                string unit_id = unit_name.Replace("DragImage", String.Empty);
                int troop_count = Int32.Parse(unit_id);
                if (mainWindow == null)
                {
                    return;
                }
                for (int i = 1; i < troop_count; i++)
                {
                    var ri = (Image)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, "DragImageExtra" + i.ToString());
                    if (ri != null)
                    {
                        Canvas.SetLeft(ri, Canvas.GetLeft(ri) + (pt.X - _startPoint.X));
                        Canvas.SetTop(ri, Canvas.GetTop(ri) + (pt.Y - _startPoint.Y));
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        // 陣形が変更された時
        private void cmbFormation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // どの部隊（先頭ユニット）の陣形か
            ComboBox cmbFormation = (ComboBox)sender;
            string unit_name = cmbFormation.Name;
            string unit_id = unit_name.Replace("cmbFormation", String.Empty);
            int troop_id = Int32.Parse(unit_id);

            var classPowerAndCity = (ClassPowerAndCity)this.Tag;
            var listTroop = classPowerAndCity.ClassSpot.UnitGroup;
            ClassHorizontalUnit itemTroop = listTroop[troop_id];
            ClassUnit itemUnit = itemTroop.ListClassUnit[0];
            itemUnit.Formation.Formation = (_010_Enum.Formation)cmbFormation.SelectedIndex;
        }

        // ユニット情報ウインドウを開く
        private void unit_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // ユニット情報のヒントが表示されてる場合は閉じる
            bool bCloseHint = false;
            if (((ClassUnit)((StackPanel)sender).Tag).Talent == "on")
            {
                foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl016_UnitHint>())
                {
                    if (itemWindow.Name == StringName.windowUnitHint)
                    {
                        mainWindow.canvasUI.Children.Remove(itemWindow);
                        bCloseHint = true;
                        break;
                    }
                }
            }

            // 最前面に移動させる
            var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
            if ((listWindow != null) && (listWindow.Any()))
            {
                int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                Canvas.SetZIndex(this, maxZ + 1);
            }

            // どのユニットをクリックしたのか
            ClassCityAndUnit classCityAndUnit = new ClassCityAndUnit();
            classCityAndUnit.ClassPowerAndCity = (ClassPowerAndCity)this.Tag;
            classCityAndUnit.ClassUnit = (ClassUnit)((StackPanel)sender).Tag;

            // ウインドウ番号によって表示位置を変える
            double offsetLeft = 0, offsetTop = 0;
            if (mainWindow.canvasUI.Margin.Left < 0)
            {
                offsetLeft = mainWindow.canvasUI.Margin.Left * -1;
            }
            if (mainWindow.canvasUI.Margin.Top < 0)
            {
                offsetTop = mainWindow.canvasUI.Margin.Top * -1;
            }

            // 既に表示されてるユニット・ウインドウをチェックする
            const int dY = 60, dX = 40, dX2 = 120, dZ = 8;
            int window_id, max_id = 0;
            var id_list = new List<int>();
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl015_Unit>())
            {
                string strTitle = itemWindow.Name;
                if (strTitle.StartsWith(StringName.windowUnit))
                {
                    window_id = Int32.Parse(strTitle.Substring(StringName.windowUnit.Length));
                    id_list.Add(window_id);
                    if (max_id < window_id)
                    {
                        max_id = window_id;
                    }
                    ClassUnit? testUnit = ((ClassCityAndUnit)itemWindow.Tag).ClassUnit;
                    if (testUnit == classCityAndUnit.ClassUnit)
                    {
                        // ユニット・ウインドウを既に開いてる場合は、新規に作らない
                        max_id = -1;
                        itemWindow.Margin = new Thickness()
                        {
                            Left = mainWindow.canvasUI.Width - offsetLeft - itemWindow.MinWidth - ((window_id - 1) % dZ) * dX - ((window_id - 1) / dZ) * dX2,
                            Top = offsetTop + ((window_id - 1) % dZ) * dY
                        };

                        // ユニット・ウインドウをこのウインドウよりも前面に移動させる
                        Canvas.SetZIndex(itemWindow, Canvas.GetZIndex(this) + 1);

                        // ヒントを閉じた場合は右上から移動させる
                        if (bCloseHint)
                        {
                            var animeMargin = new ThicknessAnimation();
                            animeMargin.From = new Thickness()
                            {
                                Left = mainWindow.canvasUI.Width - offsetLeft - itemWindow.MinWidth,
                                Top = offsetTop
                            };
                            animeMargin.Duration = new Duration(TimeSpan.FromSeconds(0.2));
                            itemWindow.BeginAnimation(Grid.MarginProperty, animeMargin);
                        }

                        break;
                    }
                }
            }
            if (max_id >= 0)
            {
                if (max_id > id_list.Count)
                {
                    // ウインドウ個数よりも最大値が大きいなら、未使用の番号を使って作成する
                    for (window_id = 1; window_id < max_id; window_id++)
                    {
                        if (id_list.Contains(window_id) == false)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    // 使用中のウインドウ番号の最大値 + 1 にして、新規に作成する
                    window_id = max_id + 1;
                }
                var itemWindow = new UserControl015_Unit();
                itemWindow.Tag = classCityAndUnit;
                itemWindow.Name = StringName.windowUnit + window_id.ToString();
                itemWindow.Margin = new Thickness()
                {
                    Left = mainWindow.canvasUI.Width - offsetLeft - itemWindow.MinWidth - ((window_id - 1) % dZ) * dX - ((window_id - 1) / dZ) * dX2,
                    Top = offsetTop + ((window_id - 1) % dZ) * dY
                };
                // プレイヤーがボタンを操作可能かどうか
                bool bControl = false;
                if (classCityAndUnit.ClassPowerAndCity.ClassPower.NameTag == mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.NameTag)
                {
                    bControl = true;
                }
                itemWindow.SetData(bControl);
                mainWindow.canvasUI.Children.Add(itemWindow);

                // ヒントを閉じた場合は右上から移動させる
                if (bCloseHint)
                {
                    var animeMargin = new ThicknessAnimation();
                    animeMargin.From = new Thickness()
                    {
                        Left = mainWindow.canvasUI.Width - offsetLeft - itemWindow.MinWidth,
                        Top = offsetTop
                    };
                    animeMargin.Duration = new Duration(TimeSpan.FromSeconds(0.2));
                    itemWindow.BeginAnimation(Grid.MarginProperty, animeMargin);
                }
                else
                {
                    var animeOpacity = new DoubleAnimation();
                    animeOpacity.From = 0.1;
                    animeOpacity.Duration = new Duration(TimeSpan.FromSeconds(0.2));
                    itemWindow.BeginAnimation(Grid.OpacityProperty, animeOpacity);
                }
            }
            id_list.Clear();

            // ルーティングを処理済みとしてマークする（親コントロールのイベントが発生しなくなる）
            e.Handled = true;
        }

        // 領地の雇用ウインドウを開く
        private void btnMercenary_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 領地から雇用なら、ClassUnit 部分を null にする
            ClassCityAndUnit classCityAndUnit = new ClassCityAndUnit();
            classCityAndUnit.ClassPowerAndCity = (ClassPowerAndCity)this.Tag;
            classCityAndUnit.ClassUnit = null;

            // 領地ウインドウの右横に雇用ウインドウを表示する
            double offsetLeft = this.Margin.Left + this.ActualWidth;

            // 既に雇用ウインドウが表示されてる場合は再利用する
            bool isFound = false;
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl020_Mercenary>())
            {
                string strTitle = itemWindow.Name;
                if ((strTitle.StartsWith(StringName.windowSpot)) || (strTitle.StartsWith(StringName.windowUnit)))
                {
                    // 新規に作らない
                    itemWindow.Tag = classCityAndUnit;
                    itemWindow.Name = this.Name + "Mercenary";
                    if (this.Margin.Left + this.ActualWidth / 2 > mainWindow.CanvasMainWidth / 2)
                    {
                        // 画面の右側なら、左横に表示する
                        offsetLeft = this.Margin.Left - itemWindow.MinWidth;
                    }
                    itemWindow.Margin = new Thickness()
                    {
                        Left = offsetLeft,
                        Top = this.Margin.Top
                    };
                    itemWindow.DisplayMercenary(mainWindow);

                    // 雇用ウインドウをこのウインドウと同じ順位にする
                    Canvas.SetZIndex(itemWindow, Canvas.GetZIndex(this));

                    isFound = true;
                    break;
                }
            }
            if (isFound == false)
            {
                // 新規に作成する
                var itemWindow = new UserControl020_Mercenary();
                itemWindow.Tag = classCityAndUnit;
                itemWindow.Name = this.Name + "Mercenary";
                if (this.Margin.Left + this.ActualWidth / 2 > mainWindow.CanvasMainWidth / 2)
                {
                    // 画面の右側なら、左横に表示する
                    offsetLeft = this.Margin.Left - itemWindow.MinWidth;
                }
                itemWindow.Margin = new Thickness()
                {
                    Left = offsetLeft,
                    Top = this.Margin.Top
                };
                itemWindow.SetData();
                mainWindow.canvasUI.Children.Add(itemWindow);

                // 雇用ウインドウをこのウインドウの後面にする
                Canvas.SetZIndex(itemWindow, Canvas.GetZIndex(this) - 1);

                // 親ウインドウから出てくるように見せる
                double offsetFrom = this.Margin.Left;
                if (offsetLeft > offsetFrom)
                {
                    offsetFrom = offsetLeft - itemWindow.MinWidth;
                }
                var animeMargin = new ThicknessAnimation();
                animeMargin.From = new Thickness()
                {
                    Left = offsetFrom,
                    Top = this.Margin.Top
                };
                animeMargin.Duration = new Duration(TimeSpan.FromSeconds(0.25));
                itemWindow.BeginAnimation(Grid.MarginProperty, animeMargin);
                var animeOpacity = new DoubleAnimation();
                animeOpacity.From = 0.1;
                animeOpacity.Duration = new Duration(TimeSpan.FromSeconds(0.2));
                itemWindow.BeginAnimation(Grid.OpacityProperty, animeOpacity);
            }
        }


        // 領地ウインドウにカーソルを乗せた時
        private void win_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // カーソルを離した時のイベントを追加する
            var cast = (UIElement)sender;
            cast.MouseLeave += win_MouseLeave;

            // 他のヘルプを全て隠す
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Visible) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    itemHelp.Visibility = Visibility.Hidden;
                }
            }

            // 操作できるかどうかでヘルプ文章を変える
            string strHelp;
            if (_isControl)
            {
                strHelp = "キャラを左クリックするとステータスが表示されます。\nキャラを右ドラッグすると配置位置を変更できます。\n右ドラッグ中のキャラは他のウィンドウにも移動できます。\nワールドマップの領地に直接ドロップする事も出来ます。\n「F」はその部隊を前衛にします。\n「M」は中衛にして、「B」は後衛にします。";
            }
            else
            {
                strHelp = "領地名のトップバーを右クリックするとウィンドウを閉じます。";
            }

            // ヘルプを作成する
            var helpWindow = new UserControl030_Help();
            helpWindow.Name = "Help_" + this.Name;
            helpWindow.SetText(strHelp);
            mainWindow.canvasUI.Children.Add(helpWindow);

            // メンバーにできるユニットが表示されてる時はヘルプを隠す
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl031_HelpMember>())
            {
                if (itemWindow.Name == StringName.windowMember)
                {
                    helpWindow.Visibility = Visibility.Hidden;
                    break;
                }
            }
        }
        private void win_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // イベントを取り除く
            var cast = (UIElement)sender;
            cast.MouseLeave -= win_MouseLeave;

            // 表示中のヘルプを取り除く
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if (itemHelp.Name == "Help_" + this.Name)
                {
                    mainWindow.canvasUI.Children.Remove(itemHelp);
                    break;
                }
            }

            // 他のヘルプを隠してた場合は、最前面のヘルプだけ表示する
            int maxZ = -1, thisZ;
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    thisZ = Canvas.GetZIndex(itemHelp);
                    if (maxZ < thisZ)
                    {
                        maxZ = thisZ;
                    }
                }
            }
            if (maxZ >= 0)
            {
                foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
                {
                    if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                    {
                        if (Canvas.GetZIndex(itemHelp) == maxZ)
                        {
                            itemHelp.Visibility = Visibility.Visible;
                            break;
                        }
                    }
                }
            }
        }

        // 全部ボタンにカーソルを乗せた時
        private void btnSelectAll_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // カーソルを離した時のイベントを追加する
            var cast = (UIElement)sender;
            cast.MouseLeave += btnSelectAll_MouseLeave;

            // 他のヘルプを全て隠す
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Visible) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    itemHelp.Visibility = Visibility.Hidden;
                }
            }

            // ヘルプを作成する
            var helpWindow = new UserControl030_Help();
            helpWindow.Name = "Help_" + this.Name + "_btnSelectAll";
            helpWindow.SetText("全てのキャラをドラッグ＆ドロップできるボタンです。\n部隊単位で目的地に入る分だけ移動します。");
            mainWindow.canvasUI.Children.Add(helpWindow);
        }

        private void btnPolitics_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var itemWindow = new UserControl080_InternalAffairs();
            //itemWindow.Tag = classPowerAndCity;
            //itemWindow.Name = StringName.windowSpot + window_id.ToString();
            //itemWindow.Margin = posWindow;
            //itemWindow.SetData();
            mainWindow.canvasUI.Children.Add(itemWindow);
        }

        private void btnSelectAll_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // イベントを取り除く
            var cast = (UIElement)sender;
            cast.MouseLeave -= btnSelectAll_MouseLeave;

            // 表示中のヘルプを取り除く
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if (itemHelp.Name == "Help_" + this.Name + "_btnSelectAll")
                {
                    mainWindow.canvasUI.Children.Remove(itemHelp);
                    break;
                }
            }

            // 他のヘルプを隠してた場合は、最前面のヘルプだけ表示する
            int maxZ = -1, thisZ;
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    thisZ = Canvas.GetZIndex(itemHelp);
                    if (maxZ < thisZ)
                    {
                        maxZ = thisZ;
                    }
                }
            }
            if (maxZ >= 0)
            {
                foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
                {
                    if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                    {
                        if (Canvas.GetZIndex(itemHelp) == maxZ)
                        {
                            itemHelp.Visibility = Visibility.Visible;
                            break;
                        }
                    }
                }
            }
        }

        // 雇用ボタンにカーソルを乗せた時
        private void btnMercenary_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // カーソルを離した時のイベントを追加する
            var cast = (UIElement)sender;
            cast.MouseLeave += btnMercenary_MouseLeave;

            // 他のヘルプを全て隠す
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Visible) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    itemHelp.Visibility = Visibility.Hidden;
                }
            }

            // ヘルプを作成する
            var helpWindow = new UserControl030_Help();
            helpWindow.Name = "Help_" + this.Name + "_btnMercenary";
            helpWindow.SetText("領地の雇用ウィンドウを表示します。");
            mainWindow.canvasUI.Children.Add(helpWindow);
        }
        private void btnMercenary_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // イベントを取り除く
            var cast = (UIElement)sender;
            cast.MouseLeave -= btnMercenary_MouseLeave;

            // 表示中のヘルプを取り除く
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if (itemHelp.Name == "Help_" + this.Name + "_btnMercenary")
                {
                    mainWindow.canvasUI.Children.Remove(itemHelp);
                    break;
                }
            }

            // 他のヘルプを隠してた場合は、最前面のヘルプだけ表示する
            int maxZ = -1, thisZ;
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    thisZ = Canvas.GetZIndex(itemHelp);
                    if (maxZ < thisZ)
                    {
                        maxZ = thisZ;
                    }
                }
            }
            if (maxZ >= 0)
            {
                foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
                {
                    if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                    {
                        if (Canvas.GetZIndex(itemHelp) == maxZ)
                        {
                            itemHelp.Visibility = Visibility.Visible;
                            break;
                        }
                    }
                }
            }
        }


    }
}
