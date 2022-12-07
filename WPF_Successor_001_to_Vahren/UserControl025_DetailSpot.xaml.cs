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
using System.Windows.Threading;
using WPF_Successor_001_to_Vahren._005_Class;

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// UserControl025_DetailSpot.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl025_DetailSpot : UserControl
    {
        public UserControl025_DetailSpot()
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

            var targetSpot = (ClassSpot)this.Tag;
            if (targetSpot == null)
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

            // 領地名
            this.txtNameSpot.Text = targetSpot.Name;

            // 領地アイコン
            if (targetSpot.ImagePath != string.Empty)
            {
                BitmapImage bitimg1 = new BitmapImage(new Uri(targetSpot.ImagePath));
                this.imgSpot.Width = bitimg1.PixelWidth;
                this.imgSpot.Height = bitimg1.PixelHeight;
                this.imgSpot.Source = bitimg1;
            }

            // 説明文
            this.txtDetail.Text = targetSpot.Text;

            // ウインドウ枠
            SetWindowFrame(mainWindow);

            // 位置を変更する前の状態が見えないようにする
            this.Visibility = Visibility.Hidden;

            // レイアウトが終わるのを待ってから位置を調節する
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // ウインドウの大きさを取得する
                // 自動サイズを整数にするため、UseLayoutRounding="true" にすること！
                // （整数にするのが無理な場合は、ここで小数点以下を切り上げればいい）
                double actual_height = this.ActualHeight;

                // マウスの位置によってウインドウの位置を変える
                Point pos = Mouse.GetPosition(mainWindow.canvasUI);
                double offsetLeft = 0, offsetTop = 0;
                if (mainWindow.canvasUI.Margin.Left < 0)
                {
                    offsetLeft = mainWindow.canvasUI.Margin.Left * -1;
                }
                if (mainWindow.canvasUI.Margin.Top < 0)
                {
                    offsetTop = mainWindow.canvasUI.Margin.Top * -1;
                }
                if (pos.X < mainWindow.canvasUI.Width / 2)
                {
                    // 画面の右下隅に配置する
                    this.Margin = new Thickness()
                    {
                        Left = mainWindow.canvasUI.Width - offsetLeft - this.MinWidth,
                        Top = mainWindow.canvasUI.Height - offsetTop - actual_height
                    };
                }
                else
                {
                    // 画面の左下隅に配置する
                    this.Margin = new Thickness()
                    {
                        Left = offsetLeft,
                        Top = mainWindow.canvasUI.Height - offsetTop - actual_height
                    };
                }
                // 位置が決まったら、見えるようにする
                this.Visibility = Visibility.Visible;

                /*
                表示する際はアニメーションできても、閉じる際は一瞬で消える。
                エフェクトに時間をかけるより、すぐに表示された方がいいかも？
                
                // 配置が終わったら、じわじわ表示されるようにする
                var myDoubleAnimation = new DoubleAnimation();
                myDoubleAnimation.From = 0.2;
                myDoubleAnimation.To = 1.0;
                myDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                this.BeginAnimation(Grid.OpacityProperty, myDoubleAnimation);
                */

            }),
            DispatcherPriority.Loaded);

        }

        // ウインドウ枠を作る
        private void SetWindowFrame(MainWindow mainWindow)
        {
            // ウインドウスキンを読み込む
            List<string> strings = new List<string>();
            strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
            strings.Add("006_WindowImage");
            strings.Add("wnd2.png");
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
            this.rectWindowPlane.Visibility = Visibility.Hidden;

            // 中央
            rect = new Int32Rect(0, 0, skin_bitmap.PixelWidth - 64, skin_bitmap.PixelWidth - 64);
            this.imgWindowCenter.Source = new CroppedBitmap(skin_bitmap, rect);

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
