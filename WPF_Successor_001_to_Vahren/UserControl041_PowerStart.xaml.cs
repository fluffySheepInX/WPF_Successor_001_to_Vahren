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
    /// UserControl041_PowerStart.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl041_PowerStart : UserControl
    {
        public UserControl041_PowerStart()
        {
            InitializeComponent();
        }

        // 定数
        // 項目サイズをここで調節できます
        private const int item_height = 60, space_height = 5, btn_width = 54, btn_height = 54;

        public void SetData()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 人材選択欄を表示する
            DisplayTalentList(mainWindow);

            // 勢力情報を表示する
            DisplayPowerInfo(mainWindow);

            // ウインドウ枠
            SetWindowFrame(mainWindow);

            // 画面の中央に配置する
            this.Margin = new Thickness()
            {
                Left = mainWindow.canvasUI.Width / 2 - this.Width / 2,
                Top = mainWindow.canvasUI.Height / 2 - this.Height / 2
            };

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
                this.rectWindowPlane2.Fill = myImageBrush;
                return;
            }

            // 不要な背景を表示しない
            this.rectShadowRight.Visibility = Visibility.Hidden;
            this.rectShadowBottom.Visibility = Visibility.Hidden;
            this.rectShadowRight2.Visibility = Visibility.Hidden;
            this.rectShadowBottom2.Visibility = Visibility.Hidden;

            // 中央
            rect = new Int32Rect(0, 0, skin_bitmap.PixelWidth - 64, skin_bitmap.PixelWidth - 64);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Stretch = Stretch.Fill;
            this.rectWindowPlane.Margin = new Thickness(4, 4, 4, 4);
            this.rectWindowPlane.Fill = myImageBrush;
            this.rectWindowPlane2.Margin = new Thickness(4, 4, 4, 4);
            this.rectWindowPlane2.Fill = myImageBrush;

            // 左上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 64, 0, 16, 16);
            this.imgWindowLeftTop.Source = new CroppedBitmap(skin_bitmap, rect);
            this.imgWindowLeftTop2.Source = new CroppedBitmap(skin_bitmap, rect);

            // 右上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 16, 0, 16, 16);
            this.imgWindowRightTop.Source = new CroppedBitmap(skin_bitmap, rect);
            this.imgWindowRightTop2.Source = new CroppedBitmap(skin_bitmap, rect);

            // 左下
            rect = new Int32Rect(skin_bitmap.PixelWidth - 64, 48, 16, 16);
            this.imgWindowLeftBottom.Source = new CroppedBitmap(skin_bitmap, rect);
            this.imgWindowLeftBottom2.Source = new CroppedBitmap(skin_bitmap, rect);

            // 右上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 16, 48, 16, 16);
            this.imgWindowRightBottom.Source = new CroppedBitmap(skin_bitmap, rect);
            this.imgWindowRightBottom2.Source = new CroppedBitmap(skin_bitmap, rect);

            // 上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 48, 0, 32, 16);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowTop.Fill = myImageBrush;
            this.rectWindowTop2.Fill = myImageBrush;

            // 下
            rect = new Int32Rect(skin_bitmap.PixelWidth - 48, 48, 32, 16);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowBottom.Fill = myImageBrush;
            this.rectWindowBottom2.Fill = myImageBrush;

            // 左
            rect = new Int32Rect(skin_bitmap.PixelWidth - 64, 16, 16, 32);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowLeft.Fill = myImageBrush;
            this.rectWindowLeft2.Fill = myImageBrush;

            // 右
            rect = new Int32Rect(skin_bitmap.PixelWidth - 16, 16, 16, 32);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowRight.Fill = myImageBrush;
            this.rectWindowRight2.Fill = myImageBrush;
        }

        private void DisplayTalentList(MainWindow mainWindow)
        {
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;
            if (classPowerAndCity == null)
            {
                return;
            }

            // タイトル
            this.txtTitleTalent.Text = classPowerAndCity.ClassPower.Name + "の人材";

            // ボタンの背景
            BitmapImage? myBackImage = null;
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("006_WindowImage");
                strings.Add("wnd5.png");
                string path = System.IO.Path.Combine(strings.ToArray());
                if (System.IO.File.Exists(path))
                {
                    // 画像が存在する時だけ、ボタンの背景にする
                    myBackImage = new BitmapImage(new Uri(path));
                }
            }

/*

            // 雇用可能なユニットのリストを初期化する
            this.panelList.Children.Clear();
            int item_count = 0;

            // 勢力に所属する全ての領地
            foreach (var itemSpotTag in classPowerAndCity.ClassPower.ListMember)
            {
                // 識別名で ClassSpot を抜き出す
                var itemSpot = mainWindow.ClassGameStatus.NowListSpot.Where(x => x.NameTag == itemSpotTag).FirstOrDefault();
                if (itemSpot == null)
                {
                    continue; // 指定された領地が見つからなければ次へ
                }
                // 領地の初期配置ユニットが人材なら
                foreach (var itemMember in itemSpot.ListMember)
                {
                    var itemUnit = mainWindow.ClassGameStatus.ListUnit.Where(x => x.NameTag == itemMember.Item1 && x.Talent == "on").FirstOrDefault();
                    if (itemUnit == null)
                    {
                        continue;
                    }

                    // 共通設定で Grid を作る
                    Grid gridItem = new Grid();
                    ColumnDefinition colDef1 = new ColumnDefinition();
                    ColumnDefinition colDef2 = new ColumnDefinition();
                    colDef1.Width = new GridLength(btn_width);
                    colDef2.Width = new GridLength(1.0, GridUnitType.Star);
                    gridItem.ColumnDefinitions.Add(colDef1);
                    gridItem.ColumnDefinitions.Add(colDef2);
                    RowDefinition rowDef1 = new RowDefinition();
                    RowDefinition rowDef2 = new RowDefinition();
                    rowDef1.Height = new GridLength(1.0, GridUnitType.Star);
                    rowDef2.Height = new GridLength(1.0, GridUnitType.Star);
                    gridItem.RowDefinitions.Add(rowDef1);
                    gridItem.RowDefinitions.Add(rowDef2);
                    gridItem.Height = item_height;
                    gridItem.Margin = new Thickness(5, space_height, 0, space_height);

                    // ユニット画像
                    List<string> strings = new List<string>();
                    strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                    strings.Add("040_ChipImage");
                    strings.Add(itemUnit.Image);
                    string path = System.IO.Path.Combine(strings.ToArray());
                    BitmapImage bitimg1 = new BitmapImage(new Uri(path));

                    // 画像は本来のピクセルサイズで表示する
                    Image imgUnit = new Image();
                    imgUnit.Width = bitimg1.PixelWidth;
                    imgUnit.Height = bitimg1.PixelHeight;
                    imgUnit.Source = bitimg1;

                    // ボタンの枠は画像よりも大きくする
                    Button btnUnit = new Button();
                    btnUnit.Name = "btnUnit" + item_count.ToString();
                    btnUnit.Tag = itemUnit;
                    btnUnit.Width = btn_width;
                    btnUnit.Height = btn_height;
                    btnUnit.Focusable = false;


                    
                    
                    
                }

            }
*/

        }

        private void DisplayPowerInfo(MainWindow mainWindow)
        {
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;
            if (classPowerAndCity == null)
            {
                return;
            }

        
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
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


        // 勢力開始ウインドウにカーソルを乗せた時
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
            helpWindow.SetText("勢力を選択してプレイします。");
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

        // 勢力選択画面で決定押した時の処理
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 勢力一覧ウィンドウを消す
            var ri2 = (UserControl040_PowerSelect)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUIRightTop, StringName.windowSelectionPowerMini);
            if (ri2 != null)
            {
                mainWindow.canvasUIRightTop.Children.Remove(ri2);
            }

            // キャンバスから自身を取り除く
            mainWindow.canvasUI.Children.Remove(this);

/*
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;
            if (classPowerAndCity == null)
            {
                return;
            }
            // プレイヤー選択不可の勢力なら何もしない
            if (classPowerAndCity.ClassPower.EnableSelect == "off")
            {
                return;
            }
*/

            // 関数をこっちに移してもいいけど、とりあえず MainWindow のメンバ関数として呼び出す
            ((Button)sender).Tag = this.Tag;
            mainWindow.ButtonSelectionPowerDecide_Click(sender, e);
        }


    }
}
