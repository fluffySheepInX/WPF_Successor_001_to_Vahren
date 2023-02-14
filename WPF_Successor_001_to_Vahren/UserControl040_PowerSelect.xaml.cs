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
    /// UserControl040_PowerSelect.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl040_PowerSelect : UserControl
    {
        public UserControl040_PowerSelect()
        {
            InitializeComponent();
        }

        // 定数
        // 項目サイズをここで調節できます
        private const int item_height = 50;

        public void SetData()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 勢力選択リストを表示する
            DisplayPowerList(mainWindow);

            // ボタンの背景
            mainWindow.SetButtonImage(this.btnWatch, "wnd5.png");
            mainWindow.SetButtonImage(this.btnTalent, "wnd5.png");

            // ウインドウ枠
            SetWindowFrame(mainWindow);
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

        public void DisplayPowerList(MainWindow mainWindow)
        {
            SolidColorBrush mySolidColorBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 255));

            // 勢力リストを初期化する
            this.panelList.Children.Clear();
            int item_count = 0;

            // 現シナリオに登場する初期勢力を抽出する
            List<ClassPower> powerList = new List<ClassPower>();
            foreach (var item in mainWindow.ClassGameStatus.ListClassScenarioInfo[mainWindow.ClassGameStatus.NumberScenarioSelection].InitListPower)
            {
                foreach (var item2 in mainWindow.ClassGameStatus.NowListPower)
                {
                    if (item == item2.NameTag)
                    {
                        powerList.Add(item2);
                        break;
                    }
                }
            }
            foreach (var itemPower in powerList)
            {

                StackPanel panelItem = new StackPanel();
                panelItem.Orientation = Orientation.Horizontal;

                BitmapImage bitimg1 = new BitmapImage(new Uri(itemPower.FlagPath));
                //旗を加工する処理を入れたい
                Int32Rect rect = new Int32Rect(0, 0, 32, 32);
                var destimg = new CroppedBitmap(bitimg1, rect);
                Image imgFlag = new Image();
                // 拡大せずに 32 x 32 のままで表示する
                imgFlag.Source = destimg;
                imgFlag.Height = 32;
                imgFlag.Width = 32;
                imgFlag.Margin = new Thickness { Left = 4 };
                panelItem.Children.Add(imgFlag);

                TextBlock txtName = new TextBlock();
                txtName.FontSize = 20;
                txtName.Text = itemPower.Name;
                txtName.Foreground = Brushes.White;
                txtName.Margin = new Thickness { Left = 10 };
                txtName.VerticalAlignment = VerticalAlignment.Center;
                panelItem.Children.Add(txtName);

                // マウスボタンを押した時に反応するので、Button ではなく透明な Border コントロールにする
                Border borderMenu = new Border();
                borderMenu.Height = item_height;
                borderMenu.Tag = itemPower;
                borderMenu.Child = panelItem;
                borderMenu.BorderThickness = new Thickness(1);
                borderMenu.BorderBrush = new SolidColorBrush(Color.FromRgb(200, 200, 255)); // 微妙に白色じゃない
                // マウスカーソルがボタンの上に来ると強調する
                borderMenu.Background = Brushes.Transparent;
                borderMenu.MouseEnter += btnPowerSelect_MouseEnter;
                borderMenu.MouseLeftButtonDown += btnPowerSelect_MouseLeftButtonDown;
                this.panelList.Children.Add(borderMenu);
                item_count += 1;
            }

            // 表示するのは最大で10個までにする
            if (item_count > 10)
            {
                item_count = 10;
            }
            else
            if (item_count < 1)
            {
                item_count = 1;
            }
            this.scrollList.Height = item_count * item_height;
        }

        /// <summary>
        /// 勢力一覧ウィンドウのボタンを押した時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void btnPowerSelect_MouseLeftButtonDown(Object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var cast = (Border)sender;
            if (cast.Tag is not ClassPower)
            {
                return;
            }

            var worldMap = mainWindow.ClassGameStatus.WorldMap;
            if (worldMap == null)
            {
                return;
            }

            double target_X = 0, target_Y = 0;
            int spot_count = 0;
            var classPower = (ClassPower)cast.Tag;
            foreach (var item in classPower.ListMember)
            {
                var spot = mainWindow.ClassGameStatus.AllListSpot.Where(x => x.NameTag == item).FirstOrDefault();
                if (spot != null)
                {
                    target_X += spot.X;
                    target_Y += spot.Y;
                    spot_count += 1;
                }
            }

            if (spot_count > 0)
            {
                target_X = target_X / spot_count;
                target_Y = target_Y / spot_count;

                // ワールドマップの表示倍率
                var tran = worldMap.canvasMap.RenderTransform as ScaleTransform;
                if (tran != null){
                    double scale = tran.ScaleX;
                    target_X *= scale;
                    target_Y *= scale;
                }

                ClassVec classVec = new ClassVec();
                // 現在の値
                classVec.X = Canvas.GetLeft(worldMap);
                classVec.Y = Canvas.GetTop(worldMap);

                // 目標にする領地の座標をウインドウ中央にするための値
                target_X = Math.Floor(mainWindow.CanvasMainWidth / 2 - target_X);
                target_Y = Math.Floor(mainWindow.CanvasMainHeight / 2 - target_Y);
                classVec.Target = new Point(target_X, target_Y);
                // 既に目標に到達してるなら終わる
                if ((classVec.Target.X == classVec.X) && (classVec.Target.Y == classVec.Y))
                {
                    return;
                }
                classVec.Speed = 20;
                classVec.Set();

                while (true)
                {
                    System.Threading.Thread.Sleep(5);

                    if (classVec.Hit(new Point(classVec.X, classVec.Y)))
                    {
                        break;
                    }

                    await Task.Run(() =>
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            var ge = classVec.Get(new Point(classVec.X, classVec.Y));
                            classVec.X = ge.X;
                            classVec.Y = ge.Y;
                            Canvas.SetLeft(worldMap, Math.Floor(ge.X));
                            Canvas.SetTop(worldMap, Math.Floor(ge.Y));
                        }));
                    });
                }

                // 目標にする座標をウインドウ中央にする（移動時に微妙にずれても、最後にここで修正する）
                Canvas.SetLeft(worldMap, target_X);
                Canvas.SetTop(worldMap, target_Y);
            }
        }

        // 勢力一覧ウィンドウのボタンにマウスを乗せた時
        private void btnPowerSelect_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var cast = (Border)sender;
            if (cast.Tag is not ClassPower)
            {
                return;
            }
            // ハイライトで強調する（文字色が白色なので、あまり白くすると読めなくなる）
            cast.Background = new SolidColorBrush(Color.FromArgb(48, 255, 255, 255));

            // マウスを離した時のイベントを追加する
            cast.MouseLeave += btnPowerSelect_MouseLeave;

            // 同じ勢力の全ての領地を強調する
            var classPower = (ClassPower)cast.Tag;
            if (classPower.ListMember.Count > 0)
            {
                var worldMap = mainWindow.ClassGameStatus.WorldMap;
                if (worldMap != null)
                {
                    worldMap.PowerMarkAnime("circle_Yellow.png", classPower.NameTag);
                }
            }

            // 勢力のヒントを表示する
            var itemWindow = new UserControl042_PowerHint();
            itemWindow.Name = StringName.windowPowerHint;
            itemWindow.SetPower(classPower, true);
            itemWindow.SetPosAnime();
            mainWindow.canvasUI.Children.Add(itemWindow);
        }
        private void btnPowerSelect_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var cast = (Border)sender;
            if (cast.Tag is not ClassPower)
            {
                return;
            }
            cast.Background = Brushes.Transparent;

            // イベントを取り除く
            cast.MouseLeave -= btnPowerSelect_MouseLeave;

            // 勢力領の強調を解除する
            var classPower = (ClassPower)cast.Tag;
            if (classPower.ListMember.Count > 0)
            {
                var worldMap = mainWindow.ClassGameStatus.WorldMap;
                if (worldMap != null)
                {
                    worldMap.RemovePowerMark(classPower.NameTag);
                }
            }

            // 勢力のヒントを閉じる
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl042_PowerHint>())
            {
                if (itemWindow.Name == StringName.windowPowerHint)
                {
                    itemWindow.Remove();
                    break;
                }
            }
        }

    }
}
