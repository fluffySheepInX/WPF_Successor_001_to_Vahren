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
using System.Windows.Media.Effects;
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
            // 勢力リストを初期化する
            this.panelList.Children.Clear();
            int item_count = 0;

            foreach (var itemPower in mainWindow.ClassGameStatus.ListPower)
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
                panelItem.Children.Add(txtName);

                Button buttonItem = new Button();
                buttonItem.Content = panelItem;
                buttonItem.Height = item_height;
                buttonItem.Tag = itemPower;
                buttonItem.Background = Brushes.Transparent;
                buttonItem.HorizontalContentAlignment = HorizontalAlignment.Left;
                buttonItem.Click += btnPowerSelect_Click;
                buttonItem.MouseEnter += btnPowerSelect_MouseEnter;
                this.panelList.Children.Add(buttonItem);
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
        private async void btnPowerSelect_Click(Object sender, EventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var cast = (Button)sender;
            if (cast.Tag is not ClassPower)
            {
                return;
            }

            var gridMapStrategy = (Grid)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasMain, StringName.gridMapStrategy);
            if (gridMapStrategy == null)
            {
                return;
            }

            double target_X = 0, target_Y = 0;
            int spot_count = 0;
            var classPower = (ClassPower)cast.Tag;
            foreach (var item in classPower.ListInitMember)
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
                target_X = Math.Truncate(target_X / spot_count);
                target_Y = Math.Truncate(target_Y / spot_count);

                // 目標にする座標をウインドウ中央にする
                /*
                gridMapStrategy.Margin = new Thickness()
                {
                    Top = mainWindow.CanvasMainHeight / 2 - target_Y,
                    Left = mainWindow.CanvasMainWidth / 2 - target_X
                };
                */

                ClassVec classVec = new ClassVec();
                // 現在の Margin
                classVec.X = gridMapStrategy.Margin.Left;
                classVec.Y = gridMapStrategy.Margin.Top;

                // 目標にする領地の座標をウインドウ中央にするための Margin
                classVec.Target = new Point(
                    mainWindow.CanvasMainWidth / 2 - target_X,
                    mainWindow.CanvasMainHeight / 2 - target_Y
                );
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
                            gridMapStrategy.Margin = new Thickness()
                            {
                                Left = Math.Truncate(ge.X),
                                Top = Math.Truncate(ge.Y)
                            };
                        }));
                    });
                }
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

            var cast = (Button)sender;
            if (cast.Tag is not ClassPower)
            {
                return;
            }

            // マウスを離した時のイベントを追加する
            cast.MouseLeave += btnPowerSelect_MouseLeave;

            // 同じ勢力の全ての領地を強調する
            var classPower = (ClassPower)cast.Tag;
            var gridMapStrategy = (Grid)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasMain, StringName.gridMapStrategy);
            if (gridMapStrategy == null)
            {
                return;
            }

            if (classPower.ListMember.Count > 0)
            {
                const int ring_size = 80, big_ring_size = 88;
                int diff_size = 16;

                // 円の大きさを時間経過で変化させる（1.5秒間隔でループする）
                var animeBigRingSize = new DoubleAnimation();
                animeBigRingSize.From = big_ring_size - diff_size;
                animeBigRingSize.To = big_ring_size + diff_size;
                animeBigRingSize.Duration = new Duration(TimeSpan.FromSeconds(0.75));
                animeBigRingSize.AutoReverse = true;
                animeBigRingSize.RepeatBehavior = RepeatBehavior.Forever;
                var animeRingSize = new DoubleAnimation();
                animeRingSize.From = ring_size - diff_size;
                animeRingSize.To = ring_size + diff_size;
                animeRingSize.Duration = new Duration(TimeSpan.FromSeconds(0.75));
                animeRingSize.AutoReverse = true;
                animeRingSize.RepeatBehavior = RepeatBehavior.Forever;

                // 円の周りをぼかす（滑らかに見えるよう品質を上げても無駄？）
                BlurEffect blurBig = new BlurEffect();
                blurBig.Radius = 16;
                blurBig.KernelType = KernelType.Gaussian;
                //blurBig.RenderingBias = RenderingBias.Quality;
                BlurEffect blurSmall = new BlurEffect();
                blurSmall.Radius = 2;
                blurSmall.KernelType = KernelType.Gaussian;
                //blurSmall.RenderingBias = RenderingBias.Quality;

                string powerNameTag = classPower.NameTag;
                var listSpot = mainWindow.ClassGameStatus.AllListSpot.Where(x => x.PowerNameTag == powerNameTag);
                foreach (var itemSpot in listSpot)
                {
                    // 太い円の上に細い円を描く
                    Ellipse elliBig = new Ellipse();
                    elliBig.Name = "HintSpot" + itemSpot.NameTag + "Big";
                    elliBig.Stroke = Brushes.Yellow; // #FFFF00
                    elliBig.StrokeThickness = 10;
                    elliBig.Width = big_ring_size;
                    elliBig.Height = big_ring_size;
                    elliBig.Effect = blurBig;
                    elliBig.HorizontalAlignment = HorizontalAlignment.Left;
                    elliBig.VerticalAlignment = VerticalAlignment.Top;
                    elliBig.Margin = new Thickness()
                    {
                        Left = itemSpot.X - big_ring_size / 2,
                        Top = itemSpot.Y - big_ring_size / 2
                    };
                    gridMapStrategy.Children.Add(elliBig);

                    // 円の大きさは共通アニメーションにする
                    elliBig.BeginAnimation(Ellipse.WidthProperty, animeBigRingSize);
                    elliBig.BeginAnimation(Ellipse.HeightProperty, animeBigRingSize);

                    // 円の位置も変えないと中心がずれる
                    var animeBigRingPos = new ThicknessAnimation();
                    animeBigRingPos.From = new Thickness()
                    {
                        Left = itemSpot.X - (big_ring_size - diff_size) / 2,
                        Top = itemSpot.Y - (big_ring_size - diff_size) / 2
                    };
                    animeBigRingPos.To = new Thickness()
                    {
                        Left = itemSpot.X - (big_ring_size + diff_size) / 2,
                        Top = itemSpot.Y - (big_ring_size + diff_size) / 2
                    };
                    animeBigRingPos.Duration = new Duration(TimeSpan.FromSeconds(0.75));
                    animeBigRingPos.AutoReverse = true;
                    animeBigRingPos.RepeatBehavior = RepeatBehavior.Forever;
                    elliBig.BeginAnimation(Ellipse.MarginProperty, animeBigRingPos);

                    Ellipse elli = new Ellipse();
                    elli.Name = "HintSpot" + itemSpot.NameTag;
                    elli.Stroke = Brushes.Yellow; // #FFFF00
                    elli.StrokeThickness = 2;
                    elli.Width = ring_size;
                    elli.Height = ring_size;
                    elli.Effect = blurSmall;
                    elli.HorizontalAlignment = HorizontalAlignment.Left;
                    elli.VerticalAlignment = VerticalAlignment.Top;
                    elli.Margin = new Thickness()
                    {
                        Left = itemSpot.X - ring_size / 2,
                        Top = itemSpot.Y - ring_size / 2
                    };
                    gridMapStrategy.Children.Add(elli);

                    // 円の大きさは共通アニメーションにする
                    elli.BeginAnimation(Ellipse.WidthProperty, animeRingSize);
                    elli.BeginAnimation(Ellipse.HeightProperty, animeRingSize);

                    // 円の位置も変えないと中心がずれる
                    var animeRingPos = new ThicknessAnimation();
                    animeRingPos.From = new Thickness()
                    {
                        Left = itemSpot.X - (ring_size - diff_size) / 2,
                        Top = itemSpot.Y - (ring_size - diff_size) / 2
                    };
                    animeRingPos.To = new Thickness()
                    {
                        Left = itemSpot.X - (ring_size + diff_size) / 2,
                        Top = itemSpot.Y - (ring_size + diff_size) / 2
                    };
                    animeRingPos.Duration = new Duration(TimeSpan.FromSeconds(0.75));
                    animeRingPos.AutoReverse = true;
                    animeRingPos.RepeatBehavior = RepeatBehavior.Forever;
                    elli.BeginAnimation(Ellipse.MarginProperty, animeRingPos);
                }
            }
        }
        private void btnPowerSelect_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var cast = (Button)sender;
            if (cast.Tag is not ClassPower)
            {
                return;
            }

            // イベントを取り除く
            cast.MouseLeave -= btnPowerSelect_MouseLeave;

            // 勢力領の強調を解除する
            var classPower = (ClassPower)cast.Tag;
            var gridMapStrategy = (Grid)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasMain, StringName.gridMapStrategy);
            if (gridMapStrategy == null)
            {
                return;
            }

            if (classPower.ListMember.Count > 0)
            {
                for (int i = gridMapStrategy.Children.Count - 1; i >= 0; i += -1)
                {
                    UIElement Child = gridMapStrategy.Children[i];
                    if (Child is Ellipse)
                    {
                        var itemElli = (Ellipse)Child;
                        if (itemElli.Name.StartsWith("HintSpot"))
                        {
                            // 円を取り除く
                            gridMapStrategy.Children.Remove(itemElli);
                        }
                    }
                }
            }
        }

    }
}
