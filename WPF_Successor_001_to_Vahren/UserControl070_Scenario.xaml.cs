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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_Successor_001_to_Vahren._005_Class;

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// UserControl070_Scenario.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl070_Scenario : UserControl
    {
        public UserControl070_Scenario()
        {
            InitializeComponent();
        }

        // 定数
        // 項目サイズをここで調節できます
        private const int item_height = 50, space_height = 10;

        public void SetData()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 画面全体に広げる
            this.Width = mainWindow.canvasMain.Width;
            this.Height = mainWindow.canvasMain.Height;

            // シナリオ選択画面でマウスが右クリックされた時の処理
            this.MouseRightButtonDown += mainWindow.ScenarioSelection_MouseRightButtonDown;

            // 背景画像
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("001_SystemImage");
                strings.Add("020_ScenarioBackImage");
                string pathDirectory = System.IO.Path.Combine(strings.ToArray()) + System.IO.Path.DirectorySeparatorChar;

                // 番号 0~9 をランダムに並び替える（シャッフル）
                int[] number_array = new int[]{ 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                int[] number_random = number_array.OrderBy(i => Guid.NewGuid()).ToArray();
                // 画像が存在するかどうか、先頭から順番に試す
                int number_now = 0;
                string path = string.Empty;
                for (int i = 0; i < 10; i++) {
                    number_now = number_random[i];

                    // 画像ファイルが存在するならループから抜ける
                    path = pathDirectory + "pre" + number_now.ToString() + ".png";
                    if (System.IO.File.Exists(path))
                    {
                        number_now = -1;
                        break;
                    }
                    path = pathDirectory + "pre" + number_now.ToString() + ".jpg";
                    if (System.IO.File.Exists(path))
                    {
                        number_now = -2;
                        break;
                    }
                    path = pathDirectory + "pre" + number_now.ToString() + ".gif";
                    if (System.IO.File.Exists(path))
                    {
                        number_now = -3;
                        break;
                    }
                }

                // 背景画像が見つかった場合だけ表示する
                if (number_now < 0)
                {
                    BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                    imgScenarioBack.Source = bitimg1;
                }
            }

            // 左側メニューを２段にするかどうか
            int heightLeftBottom = 0; // 0だと左下メニューを表示しない
            int heightTotal = (int)this.gridWhole.Height;
            // 本来はcontextで指定するんだけど、暫定的に 40% にする。
            heightLeftBottom = heightTotal * 40 / 100;
            if (heightLeftBottom > 0)
            {
                this.gridLeftBottom.Height = heightLeftBottom;
            }
            else
            {
                // 高さが初期値(0) なら完全に表示しない
                this.gridLeftBottom.Visibility = Visibility.Collapsed;
            }

            // シナリオ選択欄を表示する
            DisplayScenarioList(mainWindow, heightLeftBottom);

            // ウインドウ枠
            SetWindowFrame(mainWindow, heightLeftBottom);
        }

        // ウインドウ枠を作る
        private void SetWindowFrame(MainWindow mainWindow, int heightLeftBottom)
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
                if (heightLeftBottom > 0)
                {
                    this.rectWindowPlane3.Fill = myImageBrush;
                }
                return;
            }

            // 不要な背景を表示しない
            this.rectShadowRight.Visibility = Visibility.Hidden;
            this.rectShadowBottom.Visibility = Visibility.Hidden;
            this.rectShadowRight2.Visibility = Visibility.Hidden;
            this.rectShadowBottom2.Visibility = Visibility.Hidden;
            if (heightLeftBottom > 0)
            {
                this.rectShadowRight3.Visibility = Visibility.Hidden;
                this.rectShadowBottom3.Visibility = Visibility.Hidden;
            }

            // 中央
            rect = new Int32Rect(0, 0, skin_bitmap.PixelWidth - 64, skin_bitmap.PixelWidth - 64);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Stretch = Stretch.Fill;
            this.rectWindowPlane.Margin = new Thickness(4, 4, 4, 4);
            this.rectWindowPlane.Fill = myImageBrush;
            this.rectWindowPlane2.Margin = new Thickness(4, 4, 4, 4);
            this.rectWindowPlane2.Fill = myImageBrush;
            if (heightLeftBottom > 0)
            {
                this.rectWindowPlane3.Margin = new Thickness(4, 4, 4, 4);
                this.rectWindowPlane3.Fill = myImageBrush;
            }

            // 左上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 64, 0, 16, 16);
            this.imgWindowLeftTop.Source = new CroppedBitmap(skin_bitmap, rect);
            this.imgWindowLeftTop2.Source = new CroppedBitmap(skin_bitmap, rect);
            if (heightLeftBottom > 0)
            {
                this.imgWindowLeftTop3.Source = new CroppedBitmap(skin_bitmap, rect);
            }

            // 右上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 16, 0, 16, 16);
            this.imgWindowRightTop.Source = new CroppedBitmap(skin_bitmap, rect);
            this.imgWindowRightTop2.Source = new CroppedBitmap(skin_bitmap, rect);
            if (heightLeftBottom > 0)
            {
                this.imgWindowRightTop3.Source = new CroppedBitmap(skin_bitmap, rect);
            }

            // 左下
            rect = new Int32Rect(skin_bitmap.PixelWidth - 64, 48, 16, 16);
            this.imgWindowLeftBottom.Source = new CroppedBitmap(skin_bitmap, rect);
            this.imgWindowLeftBottom2.Source = new CroppedBitmap(skin_bitmap, rect);
            if (heightLeftBottom > 0)
            {
                this.imgWindowLeftBottom3.Source = new CroppedBitmap(skin_bitmap, rect);
            }

            // 右上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 16, 48, 16, 16);
            this.imgWindowRightBottom.Source = new CroppedBitmap(skin_bitmap, rect);
            this.imgWindowRightBottom2.Source = new CroppedBitmap(skin_bitmap, rect);
            if (heightLeftBottom > 0)
            {
                this.imgWindowRightBottom3.Source = new CroppedBitmap(skin_bitmap, rect);
            }

            // 上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 48, 0, 32, 16);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowTop.Fill = myImageBrush;
            this.rectWindowTop2.Fill = myImageBrush;
            if (heightLeftBottom > 0)
            {
                this.rectWindowTop3.Fill = myImageBrush;
            }

            // 下
            rect = new Int32Rect(skin_bitmap.PixelWidth - 48, 48, 32, 16);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowBottom.Fill = myImageBrush;
            this.rectWindowBottom2.Fill = myImageBrush;
            if (heightLeftBottom > 0)
            {
                this.rectWindowBottom3.Fill = myImageBrush;
            }

            // 左
            rect = new Int32Rect(skin_bitmap.PixelWidth - 64, 16, 16, 32);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowLeft.Fill = myImageBrush;
            this.rectWindowLeft2.Fill = myImageBrush;
            if (heightLeftBottom > 0)
            {
                this.rectWindowLeft3.Fill = myImageBrush;
            }

            // 右
            rect = new Int32Rect(skin_bitmap.PixelWidth - 16, 16, 16, 32);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowRight.Fill = myImageBrush;
            this.rectWindowRight2.Fill = myImageBrush;
            if (heightLeftBottom > 0)
            {
                this.rectWindowRight3.Fill = myImageBrush;
            }
        }

        private void DisplayScenarioList(MainWindow mainWindow, int heightLeftBottom)
        {
            // ボタンのリストを初期化する
            this.panelList.Children.Clear();
            this.panelList2.Children.Clear();
            int item_count = 0;

            // シナリオを Sortkey 順に左上のメニューに入れる
            foreach (var item in mainWindow.ClassGameStatus.ListClassScenarioInfo
                                    .Where(y => y.Sortkey <= 0)
                                    .OrderBy(x => x.Sortkey)
                                    .Select((value, index) => (value, index)))
            {
                string strTitle = item.value.ScenarioName;
                if (strTitle == string.Empty)
                {
                    strTitle = "Sortkey = " + item.value.Sortkey.ToString();
                }

                Button btnMenu = new Button();
                btnMenu.Content = strTitle;
                btnMenu.FontSize = 20;
                btnMenu.Focusable = false;
                btnMenu.Height = item_height;
                if (item_count == 0)
                {
                    btnMenu.Margin = new Thickness(5, 0, 5, 0);
                }
                else
                {
                    btnMenu.Margin = new Thickness(5, space_height, 5, 0);
                }
                btnMenu.Tag = item.value;
                btnMenu.MouseEnter += btnMenu_MouseEnter;
                // ボタンの背景
                mainWindow.SetButtonImage(btnMenu, "wnd5.png");

                this.panelList.Children.Add(btnMenu);
                item_count++;
            }

            // 左下メニューが存在するかどうかでボタンの追加先が異なる
            if (heightLeftBottom > 0)
            {
                item_count = 0;
            }
            foreach (var item in mainWindow.ClassGameStatus.ListClassScenarioInfo
                                    .Where(y => y.Sortkey > 0)
                                    .OrderBy(x => x.Sortkey)
                                    .Select((value, index) => (value, index)))
            {
                string strTitle = item.value.ScenarioName;
                if (strTitle == string.Empty)
                {
                    strTitle = "Sortkey = " + item.value.Sortkey.ToString();
                }

                Button btnMenu = new Button();
                btnMenu.Content = strTitle;
                btnMenu.FontSize = 20;
                btnMenu.Focusable = false;
                btnMenu.Height = item_height;
                if (item_count == 0)
                {
                    btnMenu.Margin = new Thickness(5, 0, 5, 0);
                }
                else
                {
                    btnMenu.Margin = new Thickness(5, space_height, 5, 0);
                }
                btnMenu.Tag = item.value;
                btnMenu.MouseEnter += btnMenu_MouseEnter;
                // ボタンの背景
                mainWindow.SetButtonImage(btnMenu, "wnd5.png");

                if (heightLeftBottom > 0)
                {
                    // 左下メニューに入れる
                    this.panelList2.Children.Add(btnMenu);
                }
                else
                {
                    // 左上メニューに入れる
                    this.panelList.Children.Add(btnMenu);
                }
                item_count++;
            }
        }

        private void btnMenu_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var cast = (ClassScenarioInfo)((Button)sender).Tag;
            if (cast == null)
            {
                return;
            }

            // シナリオ名
            this.txtNameScenario.Text = cast.ScenarioName;

            // シナリオ詳細説明文
            switch (cast.ButtonType)
            {
                case _010_Enum.ButtonType.Mail:
                    // 説明文が存在しない時のためにメールアドレスも表示する
                    this.txtDetail.Text = cast.Mail + Environment.NewLine + cast.ScenarioIntroduce;
                    break;
                case _010_Enum.ButtonType.Internet:
                    // 説明文が存在しない時のためにリンク先アドレスも表示する
                    this.txtDetail.Text = cast.Internet + Environment.NewLine + cast.ScenarioIntroduce;
                    break;
                default:
                    this.txtDetail.Text = cast.ScenarioIntroduce;
                    break;
            }

            // シナリオ画像
            this.imgScenario.Source = null;
            this.imgScenario.Visibility = Visibility.Collapsed;
            if (cast.ScenarioImageRate > 0)
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("005_BackgroundImage");
                strings.Add("005_MenuImage");
                strings.Add(cast.ScenarioImage);
                string path = System.IO.Path.Combine(strings.ToArray());
                if (System.IO.File.Exists(path))
                {
                    BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                    // 画像を高さをウィンドウ内寸に対する割合で決める
                    this.imgScenario.Height = Math.Floor((this.gridWhole.Height - 32 - 45) * cast.ScenarioImageRate / 100);
                    this.imgScenario.Source = bitimg1;
                    this.imgScenario.Visibility = Visibility.Visible;
                }
            }
        }

    }
}
