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
                double actual_width = this.ActualWidth;
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
                        Left = mainWindow.canvasUI.Width - offsetLeft - actual_width,
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

                // 配置が終わったら、しゅっと表示されるようにする（少し上から落ちる）
                var animeOpacity = new DoubleAnimation();
                animeOpacity.From = 0.1;
                animeOpacity.Duration = new Duration(TimeSpan.FromSeconds(0.2));
                this.BeginAnimation(Grid.OpacityProperty, animeOpacity);
                var animeMargin = new ThicknessAnimation();
                animeMargin.From = new Thickness()
                {
                    Left = this.Margin.Left,
                    Top = this.Margin.Top - 100
                };
                animeMargin.Duration = new Duration(TimeSpan.FromSeconds(0.25));
                this.BeginAnimation(Grid.MarginProperty, animeMargin);

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

        // 詳細説明ウィンドウを取り除く
        public void Remove()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 消えるエフェクト用にダミー画像を用意する
            Image imgDummy = new Image();
            imgDummy.Name = "DummyDetailSpot";
            imgDummy.Width = this.ActualWidth;
            imgDummy.Height = this.ActualHeight;
            imgDummy.Source = mainWindow.FrameworkElementToBitmapSource(this);
            imgDummy.Stretch = Stretch.None;
            Canvas.SetZIndex(imgDummy, Canvas.GetZIndex(this));
            // 現在位置と透明度からアニメーションを開始する
            imgDummy.Opacity = this.Opacity;
            imgDummy.Margin = new Thickness()
            {
                Left = this.Margin.Left,
                Top = this.Margin.Top
            };
            mainWindow.canvasUI.Children.Add(imgDummy);

            // 本体を取り除く
            this.BeginAnimation(Grid.MarginProperty, null);
            mainWindow.canvasUI.Children.Remove(this);

            // ダミー画像をアニメーションさせる
            double offsetTop = 0;
            if (mainWindow.canvasUI.Margin.Top < 0)
            {
                offsetTop = mainWindow.canvasUI.Margin.Top * -1;
            }

            // 移動距離に応じてアニメーション時間を変える
            double move_length = this.Margin.Top - (mainWindow.canvasUI.Height - offsetTop - imgDummy.Height - 100);
            double time_span = 0.25 * move_length / 100;

            var animeOpacity = new DoubleAnimation();
            animeOpacity.To = 0.1;
            animeOpacity.Duration = new Duration(TimeSpan.FromSeconds(time_span));
            imgDummy.BeginAnimation(Grid.OpacityProperty, animeOpacity);

            var animeMargin = new ThicknessAnimation();
            animeMargin.To = new Thickness()
            {
                Left = imgDummy.Margin.Left,
                Top = mainWindow.canvasUI.Height - offsetTop - imgDummy.Height - 100
            };
            animeMargin.Duration = new Duration(TimeSpan.FromSeconds(time_span));
            animeMargin.Completed += animeRemoveDetail_Completed;
            imgDummy.BeginAnimation(Grid.MarginProperty, animeMargin);
        }
        private void animeRemoveDetail_Completed(object? sender, EventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var imgDummy = (Image)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, "DummyDetailSpot");
            if (imgDummy == null)
            {
                return;
            }

            // ダミー画像を消す
            imgDummy.BeginAnimation(Grid.OpacityProperty, null);
            imgDummy.BeginAnimation(Grid.MarginProperty, null);
            mainWindow.canvasUI.Children.Remove(imgDummy);
        }

    }
}
