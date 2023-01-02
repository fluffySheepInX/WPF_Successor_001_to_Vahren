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

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// UserControl012_SpotSortie.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl012_SpotSortie : UserControl
    {
        public UserControl012_SpotSortie()
        {
            InitializeComponent();
        }

        // 定数
        // ユニットのタイルサイズをここで調節できます
        private const int tile_width = 48, tile_height = 66, header_width = 56;
        // 出撃登録中の強調枠の太さを一括指定します
        private const int sortie_select_width = 2;

        // 最初に呼び出した時
        private bool _isControl = true; // 陣形を変更可能かどうかの設定
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

            // 領地ウィンドウを開いてる間、ワールドマップ上で強調する
            var worldMap = mainWindow.ClassGameStatus.WorldMap;
            if (worldMap != null)
            {
                worldMap.SpotMarkAnime("circle_Blue.png", ((ClassPowerAndCity)this.Tag).ClassSpot.NameTag);
            }

            // 領地の情報を表示する
            DisplaySpotStatus(mainWindow);

            // ボタンの背景
            if (_isControl)
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("006_WindowImage");
                strings.Add("wnd5.png");
                string path = System.IO.Path.Combine(strings.ToArray());
                if (System.IO.File.Exists(path))
                {
                    // 画像が存在する時だけ、ボタンの枠と文字色を背景に合わせる
                    BitmapImage theImage = new BitmapImage(new Uri(path));
                    ImageBrush myImageBrush = new ImageBrush(theImage);
                    myImageBrush.Stretch = Stretch.Fill;
                    this.btnSelectAll.Background = myImageBrush;
                    this.btnSelectAll.Foreground = Brushes.White;
                    this.btnSelectAll.BorderBrush = Brushes.Silver;
                    this.btnCancelAll.Background = myImageBrush;
                    this.btnCancelAll.Foreground = Brushes.White;
                    this.btnCancelAll.BorderBrush = Brushes.Silver;
                }
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
                    panelUnit.Name = "panelUnit" + i.ToString();
                    panelUnit.Tag = itemUnit;
                    panelUnit.Height = tile_height;
                    panelUnit.Width = tile_width;
                    // 色を変えるためのイベントを追加する
                    panelUnit.MouseEnter += panel_MouseEnter;
                    panelUnit.MouseLeave += panel_MouseLeave;
                    // ユニット情報を表示するためのイベント
                    panelUnit.MouseLeftButtonDown += unit_MouseLeftButtonDown;
                    // 出撃登録するためのイベント
                    panelUnit.MouseRightButtonDown += troop_MouseRightButtonDown;

                    // ユニットの画像
                    BitmapImage bitimg1 = new BitmapImage(new Uri(pathDirectory + itemUnit.Image));
                    Image imgUnit = new Image();
                    imgUnit.Source = bitimg1;
                    imgUnit.Width = tile_width;
                    imgUnit.Height = tile_width;
                    // 画像本来のピクセルサイズで表示する場合は、PixelWidth と PixelHeight を指定する
                    //imgUnit.Width = bitimg1.PixelWidth;
                    //imgUnit.Height = bitimg1.PixelHeight;
                    panelUnit.Children.Add(imgUnit);

                    // ユニットのレベル
                    TextBlock txtLevel = new TextBlock();
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

                // 部隊を強調する枠を付ける
                if (itemTroop.FlagDisplay == false)
                {
                    Border border = new Border();
                    border.Name = "SortieSelect" + i.ToString();
                    border.BorderThickness = new Thickness(sortie_select_width);
                    border.BorderBrush = Brushes.Aqua;
                    border.Width = tile_width * j;
                    border.Height = tile_width + sortie_select_width; // ユニット画像の所まで
                    this.canvasSpotUnit.Children.Add(border);
                    Canvas.SetLeft(border, header_width);
                    Canvas.SetTop(border, tile_height * i);
                }

                i++;
            }

            // ユニット配置場所の大きさ
            this.canvasSpotUnit.Width = header_width + tile_width * j_max;
            this.canvasSpotUnit.Height = tile_height * i;
        }

        // 既に表示されていて、表示を更新する際
        public void DisplaySpotStatus(MainWindow mainWindow)
        {
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;

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
            // 部隊駐留数
            {
                int spot_capacity = classPowerAndCity.ClassSpot.Capacity;
                int troop_count = classPowerAndCity.ClassSpot.UnitGroup.Count();
                this.txtTroopCount.Text = "部隊 " + troop_count.ToString() + "/" + spot_capacity.ToString();
            }
            // ユニット
            {
                UpdateSpotUnit(mainWindow);
            }

        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // ワールドマップ上での強調を変更する
            var worldMap = mainWindow.ClassGameStatus.WorldMap;
            if (worldMap != null)
            {
                worldMap.SpotMark("circle_Blue.png", ((ClassPowerAndCity)this.Tag).ClassSpot.NameTag);
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
        }
        private void panel_MouseLeave(object sender, MouseEventArgs e)
        {
            var panelUnit = (StackPanel)sender;
            // 背景の設定自体を消去する
            panelUnit.Background = null;
        }


        // 部隊を領地ごとに出撃登録する
        // 登録された部隊は、追加した順番ではなく、領地内での配置順になる
        private void add_troop(MainWindow mainWindow, List<ClassHorizontalUnit> listTroop, int troop_id)
        {
            // 同じ領地の部隊が既に登録されてるなら、その前後に挿入する
            int found_index = -1;
            for (int id = troop_id - 1; id >= 0; id--)
            {
                found_index = mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.IndexOf(listTroop[id]);
                if (found_index >= 0)
                {
                    // 前の部隊の次の位置に挿入する
                    mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Insert(found_index + 1, listTroop[troop_id]);
                    break;
                }
            }
            if (found_index < 0)
            {
                int troop_id_max = listTroop.Count;
                for (int id = troop_id + 1; id < troop_id_max; id++)
                {
                    found_index = mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.IndexOf(listTroop[id]);
                    if (found_index >= 0)
                    {
                        // 後ろの部隊の前の位置に挿入する
                        mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Insert(found_index, listTroop[troop_id]);
                        break;
                    }
                }
            }
            if (found_index < 0)
            {
                // 同じ領地の部隊がまだ登録されてないなら、末尾に追加する
                mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Add(listTroop[troop_id]);
            }
            listTroop[troop_id].FlagDisplay = false; // 登録済みの印が false なのはややこしい
        }

        // 部隊単位で出撃登録する
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

            // 現在登録されてる出撃部隊数をチェックする
            int war_capacity = mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].WarCapacity;
            if (mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Count >= war_capacity)
            {
                var dialog = new Win020_Dialog();
                dialog.SetText("最大出撃数は" + war_capacity.ToString() + "です。");
                dialog.SetTime(1.2); // 待ち時間を1.2秒に短縮する
                dialog.ShowDialog();
                return; // 最大数に達してたら終わる
            }

            // 部隊を出撃登録する
            string unit_name = ((StackPanel)sender).Name;
            string unit_id = unit_name.Replace("panelUnit", String.Empty);
            int troop_id = Int32.Parse(unit_id);
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;
            var listTroop = classPowerAndCity.ClassSpot.UnitGroup;
            if (listTroop[troop_id].FlagDisplay == false)
            {
                // 既に出撃登録済みなら終わる
                return;
            }
            /*
            //TODO 行動済
            // 動作実験しにくいので行動済みでも出撃できるようにコメントアウトしてます。
            // 最終的にはコメント解除して制限してください。
            foreach (var itemUnit in listTroop[troop_id].ListClassUnit)
            {
                if (itemUnit.IsDone)
                {
                    var dialog = new Win020_Dialog();
                    dialog.SetText("行動済ユニットがいる部隊は出撃できません。");
                    dialog.SetTime(1.2); // 待ち時間を1.2秒に短縮する
                    dialog.ShowDialog();
                    return; // 行動済みのユニットが部隊に含まれてたら終わる
                }
            }
            */
            add_troop(mainWindow, listTroop, troop_id);

            // 部隊を強調する枠を付ける
            int member_count = listTroop[troop_id].ListClassUnit.Count;
            Border border = new Border();
            border.Name = "SortieSelect" + troop_id.ToString();
            border.BorderThickness = new Thickness(sortie_select_width);
            border.BorderBrush = Brushes.Aqua;
            border.Width = tile_width * member_count;
            border.Height = tile_width + sortie_select_width; // ユニット画像の所まで
            this.canvasSpotUnit.Children.Add(border);
            Canvas.SetLeft(border, header_width);
            Canvas.SetTop(border, tile_height * troop_id);

            // 出撃ウィンドウを更新する
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl065_Sortie>())
            {
                if (itemWindow.Name.StartsWith(StringName.windowSortie))
                {
                    itemWindow.UpdateSortieUnit(mainWindow);
                    break;
                }
            }
        }

        // 全て出撃登録する
        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var classPowerAndCity = (ClassPowerAndCity)this.Tag;
            var listTroop = classPowerAndCity.ClassSpot.UnitGroup;

            // 領地の部隊が登録されてるか調べる
            int found_count = 0;
            int troop_count = listTroop.Count;
            foreach (var itemTroop in listTroop)
            {
                // 出撃登録されてるなら
                if (itemTroop.FlagDisplay == false)
                {
                    found_count++;
                }

                /*
                //TODO 行動済
                // 動作実験しにくいので行動済みでも出撃できるようにコメントアウトしてます。
                // 最終的にはコメント解除して制限してください。

                // 行動済みの部隊は除外する
                foreach (var itemUnit in itemTroop.ListClassUnit)
                {
                    if (itemUnit.IsDone)
                    {
                        troop_count--;
                        break;
                    }
                }

                */
            }
            if (found_count >= troop_count)
            {
                // 既に全て出撃登録されてるなら終わる
                return;
            }

            // 現在登録されてる出撃部隊数をチェックする
            int war_capacity = mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].WarCapacity;
            int sortie_troop_count = mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Count;
            if (sortie_troop_count >= war_capacity)
            {
                var dialog = new Win020_Dialog();
                dialog.SetText("最大出撃数は" + war_capacity.ToString() + "です。");
                dialog.SetTime(1.2); // 待ち時間を1.2秒に短縮する
                dialog.ShowDialog();
                return; // 最大数に達してたら終わる
            }

            // 部隊数が多い場合は、入るだけ登録する
            int move_count = troop_count - found_count; // 新たに出撃登録する部隊数
            int moved_result = move_count * -1;
            if (move_count > war_capacity - sortie_troop_count)
            {
                move_count = war_capacity - sortie_troop_count;
                moved_result = move_count;
            }

            // 部隊を出撃登録する
            int troop_id = -1;
            foreach (var itemTroop in listTroop)
            {
                troop_id++;
                if (itemTroop.FlagDisplay == false)
                {
                    // 出撃登録済みなら次へ
                    continue;
                }

                /*
                //TODO 行動済
                // 動作実験しにくいので行動済みでも出撃できるようにコメントアウトしてます。
                // 最終的にはコメント解除して制限してください。

                // 行動済みの部隊は除外する
                bool IsDoneAny = false;
                foreach (var itemUnit in itemTroop.ListClassUnit)
                {
                    if (itemUnit.IsDone)
                    {
                        IsDoneAny = true;
                        break;
                    }
                }
                if (IsDoneAny)
                {
                    continue;
                }

                */

                // 部隊を出撃登録する
                add_troop(mainWindow, listTroop, troop_id);

                // 部隊を強調する枠を付ける
                int member_count = itemTroop.ListClassUnit.Count;
                Border border = new Border();
                border.Name = "SortieSelect" + troop_id.ToString();
                border.BorderThickness = new Thickness(sortie_select_width);
                border.BorderBrush = Brushes.Aqua;
                border.Width = tile_width * member_count;
                border.Height = tile_width + sortie_select_width; // ユニット画像の所まで
                this.canvasSpotUnit.Children.Add(border);
                Canvas.SetLeft(border, header_width);
                Canvas.SetTop(border, tile_height * troop_id);

                // 最大数まで登録したら終わる
                move_count--;
                if (move_count <= 0)
                {
                    break;
                }
            }

            // 出撃ウィンドウを更新する
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl065_Sortie>())
            {
                if (itemWindow.Name.StartsWith(StringName.windowSortie))
                {
                    itemWindow.UpdateSortieUnit(mainWindow);
                    break;
                }
            }

            // 全部隊を出撃登録できなかった場合は、何部隊が出撃するかを通知する
            if (moved_result > 0)
            {
                var dialog = new Win020_Dialog();
                dialog.SetText(moved_result.ToString() + "部隊だけ出撃しました。");
                dialog.SetTime(1.2); // 待ち時間を1.2秒に短縮する
                dialog.ShowDialog();
            }
        }

        // 全てキャンセルする
        private void btnCancelAll_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 領地の部隊が登録されてるか調べる
            int found_count = 0;
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;
            var listTroop = classPowerAndCity.ClassSpot.UnitGroup;
            foreach (var itemTroop in listTroop)
            {
                // 出撃登録されてるなら
                if (itemTroop.FlagDisplay == false)
                {
                    // リストから取り除く
                    mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Remove(itemTroop);
                    itemTroop.FlagDisplay = true;
                    found_count++;
                }
            }

            if (found_count > 0)
            {
                // 部隊の強調枠を全て取り除く
                for (int i = this.canvasSpotUnit.Children.Count - 1; i >= 0; i += -1)
                {
                    UIElement Child = this.canvasSpotUnit.Children[i];
                    if (Child is Border)
                    {
                        var border = (Border)Child;
                        if (border.Name.StartsWith("SortieSelect"))
                        {
                            this.canvasSpotUnit.Children.Remove(border);
                        }
                    }
                }

                // 出撃ウィンドウを更新する
                foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl065_Sortie>())
                {
                    if (itemWindow.Name.StartsWith(StringName.windowSortie))
                    {
                        itemWindow.UpdateSortieUnit(mainWindow);
                        break;
                    }
                }
            }
        }


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

                var animeOpacity = new DoubleAnimation();
                animeOpacity.From = 0.1;
                animeOpacity.Duration = new Duration(TimeSpan.FromSeconds(0.2));
                itemWindow.BeginAnimation(Grid.OpacityProperty, animeOpacity);
            }
            id_list.Clear();

            // ルーティングを処理済みとしてマークする（親コントロールのイベントが発生しなくなる）
            e.Handled = true;
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
            helpWindow.SetText("ユニットを右クリックする事で出撃要員に加えます。\nユニットは部隊単位で出撃登録されます。");
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

        // 全て出撃ボタンにカーソルを乗せた時
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
            helpWindow.SetText("全てのキャラを出撃登録できるボタンです。");
            mainWindow.canvasUI.Children.Add(helpWindow);
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

        // 全てキャンセルボタンにカーソルを乗せた時
        private void btnCancelAll_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // カーソルを離した時のイベントを追加する
            var cast = (UIElement)sender;
            cast.MouseLeave += btnCancelAll_MouseLeave;

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
            helpWindow.SetText("全てのキャラを出撃キャンセルできるボタンです。");
            mainWindow.canvasUI.Children.Add(helpWindow);
        }
        private void btnCancelAll_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // イベントを取り除く
            var cast = (UIElement)sender;
            cast.MouseLeave -= btnCancelAll_MouseLeave;

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

    }
}
