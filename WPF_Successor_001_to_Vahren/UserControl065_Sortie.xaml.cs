using System;
using System.Collections.Generic;
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
using WPF_Successor_001_to_Vahren._006_ClassStatic;

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// UserControl065_Sortie.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl065_Sortie : UserControl
    {
        public UserControl065_Sortie()
        {
            InitializeComponent();
        }

        // 定数
        // ユニットのタイルサイズをここで調節できます
        private const int tile_width = 48;

        // 最初に呼び出した時
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

            // 出撃先は Name から取得する
            string spotNameTag = this.Name.Substring(StringName.windowSortie.Length);
            var classSpot = mainWindow.ClassGameStatus.NowListSpot.Where(x => x.NameTag == spotNameTag).FirstOrDefault();
            if (classSpot == null)
            {
                return;
            }

            // 隣接領地は Tag から取得する
            List<ClassSpot>? listSpot = (List<ClassSpot>)this.Tag;
            if (listSpot == null)
            {
                return;
            }

            // 関連するウィンドウを開いてた場合は閉じる（移動や雇用で配置状況が変化するのを防ぐ）
            for (int i = mainWindow.canvasUI.Children.Count - 1; i >= 0; i += -1)
            {
                UIElement Child = mainWindow.canvasUI.Children[i];
                // 領地ウィンドウを閉じる
                if (Child is UserControl010_Spot)
                {
                    var itemWindow = (UserControl010_Spot)Child;
                    if (itemWindow.Name.StartsWith(StringName.windowSpot))
                    {
                        mainWindow.canvasUI.Children.Remove(itemWindow);
                    }
                }
                // ユニット情報ウィンドウを閉じる
                else if (Child is UserControl015_Unit)
                {
                    var itemWindow = (UserControl015_Unit)Child;
                    if (itemWindow.Name.StartsWith(StringName.windowUnit))
                    {
                        mainWindow.canvasUI.Children.Remove(itemWindow);
                    }
                }
                // 雇用ウィンドウを閉じる
                else if (Child is UserControl020_Mercenary)
                {
                    var itemWindow = (UserControl020_Mercenary)Child;
                    if (itemWindow.Name.EndsWith("Mercenary"))
                    {
                        mainWindow.canvasUI.Children.Remove(itemWindow);
                    }
                }
            }

            // 出撃ウィンドウを開いてる間、ワールドマップ上で強調する
            var worldMap = mainWindow.ClassGameStatus.WorldMap;
            if (worldMap != null)
            {
                // ワールドマップ上での強調を全て解除する
                worldMap.RemoveSpotMarkAll();

                // 出撃先は赤色
                worldMap.SpotMarkAnime("circle_Red.png", spotNameTag);

                // 出撃可能な領地は青色
                foreach (var itemSpot in listSpot)
                {
                    worldMap.SpotMark("circle_Blue.png", itemSpot.NameTag);
                }
            }

            // 出撃先の名前
            this.txtNameSpot.Text = classSpot.Name + "へ出撃";

            // 出撃部隊数
            int war_capacity = mainWindow.ClassGameStatus.ListClassScenarioInfo[mainWindow.ClassGameStatus.NumberScenarioSelection].WarCapacity;
            this.txtNumber.Text = "0/" + war_capacity.ToString();

            // ボタンの背景
            mainWindow.SetButtonImage(this.btnSortie, "wnd5.png");

            // ウインドウ枠
            SetWindowFrame(mainWindow);

            // 画面の中央に配置する
            this.Margin = new Thickness()
            {
                Left = mainWindow.canvasUI.Width / 2 - this.MinWidth / 2,
                Top = mainWindow.canvasUI.Height / 2 - this.MinHeight / 2
            };

            // 出現時のアニメーション
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
            strings.Add("wnd0.png");
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

        // 出撃登録されたユニットの表示だけを更新する
        public void UpdateSortieUnit(MainWindow mainWindow)
        {
            // 部隊の最大メンバー数
            int member_capacity = mainWindow.ClassGameStatus.ListClassScenarioInfo[mainWindow.ClassGameStatus.NumberScenarioSelection].MemberCapacity;

            // 画像のディレクトリ
            List<string> strings = new List<string>();
            strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
            strings.Add("040_ChipImage");
            string pathDirectory = System.IO.Path.Combine(strings.ToArray()) + System.IO.Path.DirectorySeparatorChar;

            // 最初に全て消去する
            this.panelBaseSpot.Children.Clear();

            string strPreviousNameTag = "\"";
            foreach (var itemTroop in mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup)
            {
                // 部隊のパネル
                StackPanel panelTroop = new StackPanel();
                panelTroop.Tag = itemTroop;
                panelTroop.Orientation = Orientation.Horizontal;
                panelTroop.Width = tile_width * member_capacity;
                // 背景が無いとマウスに反応しないので透明にする
                panelTroop.Background = Brushes.Transparent;
                // 色を変えるためのイベントを追加する
                panelTroop.MouseEnter += panel_MouseEnter;
                panelTroop.MouseLeave += panel_MouseLeave;
                // 出撃キャンセルするためのイベント
                panelTroop.MouseRightButtonDown += troop_MouseRightButtonDown;

                // ユニットの画像
                foreach (var itemUnit in itemTroop.ListClassUnit)
                {
                    BitmapImage bitimg1 = new BitmapImage(new Uri(pathDirectory + itemUnit.Image));
                    Image imgUnit = new Image();
                    imgUnit.Source = bitimg1;
                    imgUnit.Width = tile_width;
                    imgUnit.Height = tile_width;
                    panelTroop.Children.Add(imgUnit);
                }

                // 出撃元の領地ごとにまとめて表示する
                string strBaseName = string.Empty;
                string strBaseNameTag = string.Empty;
                var baseSpot = (ClassSpot)itemTroop.Spot;
                if (baseSpot != null)
                {
                    strBaseName = baseSpot.Name;
                    strBaseNameTag = baseSpot.NameTag;
                }

                // 同じ領地枠に追加する
                if (strBaseNameTag == strPreviousNameTag)
                {
                    var panelSpot = (StackPanel)LogicalTreeHelper.FindLogicalNode(this.panelBaseSpot, "panelSpot" + strBaseNameTag);
                    if (panelSpot != null)
                    {
                        panelSpot.Children.Add(panelTroop);
                    }
                    else
                    {
                        // 領地枠が見つからなかった
                        strPreviousNameTag = "\"";
                    }
                }
                // 領地枠を新規作成する
                if (strBaseNameTag != strPreviousNameTag)
                {
                    strPreviousNameTag = strBaseNameTag;

                    // 領地の外枠
                    Border borderSpot = new Border();
                    if (this.panelBaseSpot.Children.Count == 0)
                    {
                        // 最初の領地枠だけ上のマージンが無い
                        borderSpot.Margin = new Thickness(0, 0, 5, 0);
                    }
                    else
                    {
                        borderSpot.Margin = new Thickness(0, 5, 5, 0);
                    }
                    borderSpot.Padding = new Thickness(0, 0, 0, 5);
                    borderSpot.BorderThickness = new Thickness(2);
                    borderSpot.BorderBrush = Brushes.White;
                    this.panelBaseSpot.Children.Add(borderSpot);

                    // 領地のパネル
                    StackPanel panelSpot = new StackPanel();
                    panelSpot.Name = "panelSpot" + strBaseNameTag;
                    panelSpot.Tag = baseSpot;
                    borderSpot.Child = panelSpot;

                    // 領地の名前
                    if (strBaseName != string.Empty)
                    {
                        TextBlock txtSpotName = new TextBlock();
                        txtSpotName.Height = 30;
                        txtSpotName.FontSize = 20;
                        txtSpotName.Foreground = Brushes.White;
                        txtSpotName.TextAlignment = TextAlignment.Center;
                        txtSpotName.Text = strBaseName;
                        panelSpot.Children.Add(txtSpotName);
                    }

                    // 名前の下に部隊を追加する
                    panelSpot.Children.Add(panelTroop);
                }
            }

            // 出撃部隊数
            int war_capacity = mainWindow.ClassGameStatus.ListClassScenarioInfo[mainWindow.ClassGameStatus.NumberScenarioSelection].WarCapacity;
            this.txtNumber.Text = mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Count.ToString() + "/" + war_capacity.ToString();
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 出撃登録済みの部隊情報を初期化する
            foreach (var itemTroop in mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup)
            {
                itemTroop.FlagDisplay = true; // 出撃しないと true なのややこしい
            }
            mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Clear();

            // 出撃先は Name から取得する
            string spotNameTag = this.Name.Substring(StringName.windowSortie.Length);
            var classSpot = mainWindow.ClassGameStatus.NowListSpot.Where(x => x.NameTag == spotNameTag).FirstOrDefault();
            if (classSpot == null)
            {
                return;
            }

            // 隣接領地は Tag から取得する
            List<ClassSpot>? listSpot = (List<ClassSpot>)this.Tag;
            if (listSpot == null)
            {
                return;
            }

            // 出撃キャンセルしたら、開いてる出撃選択用領地ウィンドウを全て閉じる
            for (int i = mainWindow.canvasUI.Children.Count - 1; i >= 0; i += -1)
            {
                UIElement Child = mainWindow.canvasUI.Children[i];
                // 出撃選択用領地ウィンドウを閉じる
                if (Child is UserControl012_SpotSortie)
                {
                    var itemWindow = (UserControl012_SpotSortie)Child;
                    if (itemWindow.Name.StartsWith(StringName.windowSpotSortie))
                    {
                        mainWindow.canvasUI.Children.Remove(itemWindow);
                    }
                }
                // ユニット情報ウィンドウを閉じる
                else if (Child is UserControl015_Unit)
                {
                    var itemWindow = (UserControl015_Unit)Child;
                    if (itemWindow.Name.StartsWith(StringName.windowUnit))
                    {
                        mainWindow.canvasUI.Children.Remove(itemWindow);
                    }
                }
            }

            // ワールドマップ上での強調を解除する
            var worldMap = mainWindow.ClassGameStatus.WorldMap;
            if (worldMap != null)
            {
                worldMap.RemoveSpotMark(spotNameTag);
                foreach (var itemSpot in listSpot)
                {
                    worldMap.RemoveSpotMark(itemSpot.NameTag);
                }
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
            var panel = (StackPanel)sender;
            // WPFの標準色をどうやって取得するのか知らないので、暗い色にする
            panel.Background = Brushes.Gray;
        }
        private void panel_MouseLeave(object sender, MouseEventArgs e)
        {
            var panel = (StackPanel)sender;
            // 背景を消去するとマウスに反応しなくなるので透明にする
            panel.Background = Brushes.Transparent;
        }

        // 部隊単位で出撃キャンセルする
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

            // 部隊を出撃キャンセルする
            var itemTroop = (ClassHorizontalUnit)((StackPanel)sender).Tag;
            mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Remove(itemTroop);
            itemTroop.FlagDisplay = true; // 出撃キャンセルすると true なのはややこしい

            // 出撃元の領地ウィンドウが開いてるか探す
            if (itemTroop.Spot.NameTag != string.Empty)
            {
                string strTitle = StringName.windowSpotSortie + itemTroop.Spot.NameTag;
                foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl012_SpotSortie>())
                {
                    if (itemWindow.Name == strTitle)
                    {
                        // 選択部隊の強調枠を取り除く
                        var listTroop = itemTroop.Spot.UnitGroup;
                        int troop_id = listTroop.IndexOf(itemTroop);
                        if (troop_id < 0)
                        {
                            break;
                        }
                        string strBorderName = "SortieSelect" + troop_id.ToString();
                        foreach (var border in itemWindow.canvasSpotUnit.Children.OfType<Border>())
                        {
                            if (border.Name == strBorderName)
                            {
                                itemWindow.canvasSpotUnit.Children.Remove(border);
                                break;
                            }
                        }
                        break;
                    }
                }
            }

            // 出撃ウィンドウを更新する
            this.UpdateSortieUnit(mainWindow);
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

            // ヘルプを作成する
            var helpWindow = new UserControl030_Help();
            helpWindow.Name = "Help_" + this.Name;
            helpWindow.SetText("領地ウィンドウからユニットを右クリックで出撃させます。\n出撃ユニットを右クリックするとキャンセルできます。\nウィンドウを閉じると全てキャンセルされます。");
            mainWindow.canvasUI.Children.Add(helpWindow);
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


        // 出撃する
        private void btnSortie_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 出撃登録されてなければ終わる
            if (mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Count == 0)
            {
                return;
            }

            // 出撃先は Name から取得する
            string spotNameTag = this.Name.Substring(StringName.windowSortie.Length);
            var targetSpot = mainWindow.ClassGameStatus.NowListSpot.Where(x => x.NameTag == spotNameTag).FirstOrDefault();
            if (targetSpot == null)
            {
                return;
            }

            // 隣接領地は Tag から取得する
            List<ClassSpot>? listSpot = (List<ClassSpot>)this.Tag;
            if (listSpot == null)
            {
                return;
            }

            // 確認メッセージを表示する
            var dialog = new Win025_Select();
            dialog.SetText(targetSpot.Name + "に攻め込みます。\nよろしいですか？");
            bool? result = dialog.ShowDialog();
            if (result == false)
            {
                return;
            }

            // 開いてる出撃選択用領地ウィンドウを全て閉じる
            for (int i = mainWindow.canvasUI.Children.Count - 1; i >= 0; i += -1)
            {
                UIElement Child = mainWindow.canvasUI.Children[i];
                // 出撃選択用領地ウィンドウを閉じる
                if (Child is UserControl012_SpotSortie)
                {
                    var itemWindow = (UserControl012_SpotSortie)Child;
                    if (itemWindow.Name.StartsWith(StringName.windowSpotSortie))
                    {
                        mainWindow.canvasUI.Children.Remove(itemWindow);
                    }
                }
                // ユニット情報ウィンドウを閉じる
                else if (Child is UserControl015_Unit)
                {
                    var itemWindow = (UserControl015_Unit)Child;
                    if (itemWindow.Name.StartsWith(StringName.windowUnit))
                    {
                        mainWindow.canvasUI.Children.Remove(itemWindow);
                    }
                }
            }

            // ワールドマップ上での強調を解除する
            var worldMap = mainWindow.ClassGameStatus.WorldMap;
            if (worldMap != null)
            {
                worldMap.RemoveSpotMark(spotNameTag);
                foreach (var itemSpot in listSpot)
                {
                    worldMap.RemoveSpotMark(itemSpot.NameTag);
                }
            }

            // 出撃ウィンドウを閉じる
            mainWindow.canvasUI.Children.Remove(this);

            // 出撃選択を解除して、行動済みにする
            foreach (var itemTroop in mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup)
            {
                itemTroop.FlagDisplay = true;
                foreach (var itemUnit in itemTroop.ListClassUnit)
                {
                    itemUnit.IsDone = true;
                }
            }

            // 空の領地なら戦闘無しに占領する
            if (targetSpot.UnitGroup.Count == 0)
            {
                // メッセージと同時に占領エフェクトを表示する？待ち時間を調節すればいい。
                var dialog2 = new Win020_Dialog();
                dialog2.SetText("戦場の領地に守備兵がいないので戦闘を省略します。");
                dialog2.SetTime(1.2); // 待ち時間を1.2秒に短縮する
                dialog2.ShowDialog();

                // ワールドマップ領地の所属勢力を変更する
                if (worldMap != null)
                {
                    worldMap.ChangeSpotPower(spotNameTag, listSpot[0].PowerNameTag);
                }

                // 出撃先の領地は空なので、守備隊をどうするかは考慮しなくていい。
                // 出撃先に入る数だけ、部隊を移動させる
                int spot_capacity = targetSpot.Capacity;
                foreach (var itemTroop in mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup)
                {
                    if (spot_capacity > 0)
                    {
                        // 出撃元から取り除く
                        var srcSpot = itemTroop.Spot;
                        if (srcSpot != null)
                        {
                            srcSpot.UnitGroup.Remove(itemTroop);
                        }

                        // 出撃先に追加する
                        targetSpot.UnitGroup.Add(itemTroop);
                        itemTroop.Spot = targetSpot;

                        // 空きを減らす
                        spot_capacity--;
                    }
                    else
                    {
                        break;
                    }
                }

                mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Clear();
                mainWindow.ClassGameStatus.ClassBattle.DefUnitGroup.Clear();
                mainWindow.ClassGameStatus.ClassBattle.NeutralUnitGroup.Clear();

                // 勢力メニューを更新する
                if (mainWindow.ClassGameStatus.WindowStrategyMenu != null)
                {
                    mainWindow.ClassGameStatus.WindowStrategyMenu.DisplayPowerStatus(mainWindow);
                }

            }
            // 守備隊が存在する場合は、戦闘になる
            else
            {
                //MessageBox.Show(targetSpot.Name + "には部隊が存在します");

                var extractMap = mainWindow
                                    .ClassGameStatus
                                    .ListClassMapBattle
                                    .Where(x => x.TagName == targetSpot.Map)
                                    .FirstOrDefault();
                if (extractMap != null)
                {
                    mainWindow.ClassGameStatus.ClassBattle.ClassMapBattle = extractMap;

                    ClassStaticBattle.AddBuilding(mainWindow.ClassGameStatus);

                }

                // 防衛ユニット設定
                foreach (var itemTroop in targetSpot.UnitGroup)
                {
                    mainWindow.ClassGameStatus.ClassBattle.DefUnitGroup.Add(itemTroop);
                }

                mainWindow.FadeOut = true;

                mainWindow.delegateBattleMap = mainWindow.SetBattleMap;

                mainWindow.FadeIn = true;
            }
        }

        // 出撃ボタンにカーソルを乗せた時
        private void btnSortie_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // カーソルを離した時のイベントを追加する
            var cast = (UIElement)sender;
            cast.MouseLeave += btnSortie_MouseLeave;

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
            helpWindow.Name = "Help_" + this.Name + "_btnSortie";
            helpWindow.SetText("登録されてるメンバーで出撃します。");
            mainWindow.canvasUI.Children.Add(helpWindow);
        }
        private void btnSortie_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // イベントを取り除く
            var cast = (UIElement)sender;
            cast.MouseLeave -= btnSortie_MouseLeave;

            // 表示中のヘルプを取り除く
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if (itemHelp.Name == "Help_" + this.Name + "_btnSortie")
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
