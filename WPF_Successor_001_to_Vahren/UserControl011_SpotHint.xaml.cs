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
    /// UserControl011_SpotHint.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl011_SpotHint : UserControl
    {
        public UserControl011_SpotHint()
        {
            InitializeComponent();
        }

        public void SetData()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var classPowerAndCity = (ClassPowerAndCity)this.Tag;
            if (classPowerAndCity == null)
            {
                return;
            }
            if (classPowerAndCity.ClassSpot == null)
            {
                return;
            }

            // 最前面に配置する
            var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
            if ( (listWindow != null) && (listWindow.Any()) )
            {
                int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                Canvas.SetZIndex(this, maxZ + 1);
            }

            // 勢力名
            if (classPowerAndCity.ClassPower.NameTag != string.Empty)
            {
                this.txtNamePower.Text = classPowerAndCity.ClassPower.Name;
            }
            else
            {
                this.txtNamePower.Text = "中立勢力";
            }

            // 領地名
            this.txtNameSpot.Text = classPowerAndCity.ClassSpot.Name;

            // 部隊数を数えてユニットを表示する
            int tile_height = 32;
            var listTroop = classPowerAndCity.ClassSpot.UnitGroup;
            int troop_count = listTroop.Count();
            if (troop_count > 0)
            {
                // 画像のディレクトリ
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("040_ChipImage");
                string pathDirectory = System.IO.Path.Combine(strings.ToArray()) + System.IO.Path.DirectorySeparatorChar;

                // 部隊数によって、一部隊ごとの高さを変える
                // ８部隊までなら 32ドット。それを超えると画像が途切れる。最低でも半分の 16ドットは表示する。
                tile_height = 256 / troop_count;
                if (tile_height > 32)
                {
                    tile_height = 32;
                }
                else if (tile_height < 16)
                {
                    tile_height = 16;
                }

                // ユニットを並べる
                foreach (var itemTroop in listTroop)
                {
                    // 部隊のパネル
                    StackPanel panelTroop = new StackPanel();
                    panelTroop.Orientation = Orientation.Horizontal;
                    // 画像のサイズに関係なく、パネルの高さで表示する範囲を決める。
                    panelTroop.Height = tile_height;

                    foreach (var itemUnit in itemTroop.ListClassUnit)
                    {
                        // ユニットの画像
                        BitmapImage bitimg1 = new BitmapImage(new Uri(pathDirectory + itemUnit.Image));
                        Image imgUnit = new Image();
                        imgUnit.Source = bitimg1;
                        // アスペクト比を保つので幅だけ 32までに制限すればいい
                        int image_width = bitimg1.PixelWidth;
                        if (image_width > 32)
                        {
                            image_width = 32;
                        }
                        imgUnit.Width = image_width;
                        imgUnit.Height = bitimg1.PixelHeight;
                        panelTroop.Children.Add(imgUnit);
                    }

                    this.panelSpotUnit.Children.Add(panelTroop);
                }
            }

            // 経済、城壁、部隊、戦力
            int spot_capacity = classPowerAndCity.ClassSpot.Capacity;
            this.txtStatus.Text = "経済" + classPowerAndCity.ClassSpot.Gain.ToString()
                                + " 城壁" + classPowerAndCity.ClassSpot.Castle.ToString()
                                + " 部隊" + troop_count.ToString() + "/" + spot_capacity.ToString()
                                + " 戦力 ?";

            // ウインドウ枠
            SetWindowFrame(mainWindow);

            // 画面の左上隅に配置する
            double offsetLeft = 0, offsetTop = 0;
            if (mainWindow.canvasUI.Margin.Left < 0)
            {
                offsetLeft = mainWindow.canvasUI.Margin.Left * -1;
            }
            if (mainWindow.canvasUI.Margin.Top < 0)
            {
                offsetTop = mainWindow.canvasUI.Margin.Top * -1;
            }
            this.Margin = new Thickness()
            {
                Left = offsetLeft,
                Top = offsetTop
            };
        }

        // ウインドウ枠を作る
        private void SetWindowFrame(MainWindow mainWindow)
        {
            // ウインドウスキンを読み込む
            List<string> strings = new List<string>();
            strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
            strings.Add("006_WindowImage");
            strings.Add("wnd1.png");
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

    }
}
