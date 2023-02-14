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
    /// UserControl042_PowerHint.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl042_PowerHint : UserControl
    {
        public UserControl042_PowerHint()
        {
            InitializeComponent();

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // ウインドウ枠
            SetWindowFrame(mainWindow);
        }

        // 画面の左下に置く
        public void SetPos()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
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

            // 面倒なので表示する際のアニメーションは省略する
            // 将来的には追加してもいい、引数でアニメの on/off 指定とか
            double offsetLeft = 0, offsetTop = 0;
            if (mainWindow.canvasUI.Margin.Left < 0)
            {
                offsetLeft = mainWindow.canvasUI.Margin.Left * -1;
            }
            if (mainWindow.canvasUI.Margin.Top < 0)
            {
                offsetTop = mainWindow.canvasUI.Margin.Top * -1;
            }
            // 画面の左下隅に配置する
            this.Margin = new Thickness()
            {
                Left = offsetLeft,
                Top = mainWindow.canvasUI.Height - offsetTop - this.Height
            };
        }

        // 勢力を指定する（後から更新・変更できる）
        public void SetPower(ClassPower? targetPower, bool boolDetail)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 勢力が空の場合は、全ての表示を消す。
            this.txtStatus.Text = string.Empty;
            if (targetPower == null)
            {
                // 上の項目
                this.borderFace.Visibility = Visibility.Collapsed;
                this.imgFlag.Source = null;
                this.txtNamePower.Text = string.Empty;
                this.txtMoney.Text = string.Empty;
                this.txtTotalGain.Text = string.Empty;
                this.txtNumberSpot.Text = string.Empty;
                this.txtNumberUnit.Text = string.Empty;
                // 下の項目
                this.txtTotalCost.Text = string.Empty;
                this.txtTotalFinance.Text = string.Empty;
                this.txtTrainingAverage.Text = string.Empty;
                this.txtTrainingUp.Text = string.Empty;
                this.txtBaseLevel.Text = string.Empty;
                return;
            }

            // マスターの顔絵と名前
            bool boolFound = false;
            var unitMaster = mainWindow.ClassGameStatus.NowListUnit.Where(x => x.NameTag == targetPower.MasterTag).FirstOrDefault();
            if (unitMaster != null)
            {
                // 顔絵
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("010_FaceImage");
                strings.Add(unitMaster.Face);
                string path = System.IO.Path.Combine(strings.ToArray());
                if (System.IO.File.Exists(path))
                {
                    boolFound = true;
                    this.borderFace.Visibility = Visibility.Visible;
                    BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                    this.imgFace.Source = bitimg1;
                }
            }
            if (boolFound == false)
            {
                // 顔絵を表示しない場合は、枠自体を消す
                this.borderFace.Visibility = Visibility.Collapsed;
            }

            //旗
            if (targetPower.FlagPath != string.Empty){
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("030_FlagImage");
                strings.Add(targetPower.FlagPath);
                string path = System.IO.Path.Combine(strings.ToArray());
                BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                Int32Rect rect = new Int32Rect(0, 0, 32, 32);
                var destimg = new CroppedBitmap(bitimg1, rect);
                this.imgFlag.Source = destimg;
            }
            //勢力名
            {
                this.txtNamePower.Text = targetPower.Name;
            }

            //軍資金
            {
                this.txtMoney.Text = targetPower.Money.ToString();
            }
            //領地数
            {
                this.txtNumberSpot.Text = targetPower.ListMember.Count.ToString();
            }

            // 各領地のデータを集計する
            int total_gain = 0;
            int unit_count = 0;
            int total_cost = 0, total_finance = 0;
            int talent_count = 0;
            int total_level = 0;
            string powerNameTag = targetPower.NameTag;
            var listSpot = mainWindow.ClassGameStatus.NowListSpot.Where(x => x.PowerNameTag == powerNameTag);
            foreach (var itemSpot in listSpot)
            {
                total_gain += itemSpot.Gain;
                foreach (var itemTroop in itemSpot.UnitGroup)
                {
                    unit_count += itemTroop.ListClassUnit.Count;
                    foreach (var itemUnit in itemTroop.ListClassUnit)
                    {
                        // 維持費と財政値
                        if (itemUnit.Cost > 0)
                        {
                            total_cost += itemUnit.Cost;
                        }
                        else if (itemUnit.Cost < 0)
                        {
                            // 維持費がマイナスなら財政値になる
                            total_finance -= itemUnit.Cost;
                        }
                        if (itemUnit.Finance > 0)
                        {
                            total_finance += itemUnit.Finance;
                        }

                        // 人材のレベルを合計する
                        if (itemUnit.Talent == "on")
                        {
                            total_level += itemUnit.Level;
                            talent_count++;
                        }
                    }
                }
            }

            //総収入
            {
                total_gain *= (int)(mainWindow.ClassGameStatus.ClassContext.GainPer * 0.01);
                this.txtTotalGain.Text = "+" + total_gain.ToString();
            }
            //ユニット数
            {
                this.txtNumberUnit.Text = unit_count.ToString();
            }

            // 詳細データを表示する場合
            if (boolDetail)
            {
                // 維持費
                this.txtTotalCost.Text = "維持費 " + total_cost.ToString();

                // 財政値
                this.txtTotalFinance.Text = "財政値 " + total_finance.ToString();

                // 訓練限界
                {
                    int average_level = 0;
                    if (talent_count > 0)
                    {
                        // 勢力の訓練限界値の人材平均レベルパーセンテージが設定されてる場合は、平均値に掛けること。
                        // 今はまだ設定が実装されてないので、平均値のままにしておく。
                        average_level = total_level / talent_count;
                    }
                    this.txtTrainingAverage.Text = "訓練限界 " + average_level.ToString();
                }

                // 訓練上昇
                {
                    // 勢力の訓練上昇値（１ターンの訓練でレベルアップする数量）が設定されてる場合は、標準値と違う。
                    // 今はまだ設定が実装されてないので、標準値のままにしておく。
                    this.txtTrainingUp.Text = "兵レベル 2";
                }

                // 兵レベル
                {
                    this.txtBaseLevel.Text = "兵レベル +?";
                }
            }
            else
            {
                // 全ての項目を空にする（項目名も消える）
                this.txtTotalCost.Text = string.Empty;
                this.txtTotalFinance.Text = string.Empty;
                this.txtTrainingAverage.Text = string.Empty;
                this.txtTrainingUp.Text = string.Empty;
                this.txtBaseLevel.Text = string.Empty;
            }
        }

        // 勢力を指定する（後から更新・変更できる）
        public void SetText(string strText)
        {
            this.txtStatus.Text = strText;
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

        // 画面の左下にアニメーション付きで置く
        public void SetPosAnime()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 同じウィンドウ（あるいはダミー画像）が既に存在する場合は消す
            {
                foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl042_PowerHint>())
                {
                    if (itemWindow.Name == StringName.windowPowerHint)
                    {
                        mainWindow.canvasUI.Children.Remove(itemWindow);
                        break;
                    }
                }
                var imgDummy = (Image)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, "DummyHintPower");
                if (imgDummy != null)
                {
                    mainWindow.canvasUI.Children.Remove(imgDummy);
                }
            }

            // 最前面に配置する
            var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
            if ( (listWindow != null) && (listWindow.Any()) )
            {
                int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                Canvas.SetZIndex(this, maxZ + 1);
            }

            // 画面の左下隅に配置する
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
                Top = mainWindow.canvasUI.Height - offsetTop - this.Height
            };

            // 配置が終わったら、しゅっと表示されるようにする（少し上から落ちる）
            var animeOpacity = new DoubleAnimation();
            animeOpacity.From = 0.1;
            animeOpacity.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            this.BeginAnimation(Canvas.OpacityProperty, animeOpacity);
            var animeMargin = new ThicknessAnimation();
            animeMargin.From = new Thickness()
            {
                Left = this.Margin.Left,
                Top = this.Margin.Top - 100
            };
            animeMargin.Duration = new Duration(TimeSpan.FromSeconds(0.25));
            this.BeginAnimation(Canvas.MarginProperty, animeMargin);
        }

        // アニメーション付きでウィンドウを取り除く
        public void Remove()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 消えるエフェクト用にダミー画像を用意する
            Image imgDummy = new Image();
            imgDummy.Name = "DummyHintPower";
            imgDummy.Width = this.Width;
            imgDummy.Height = this.Height;
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
            this.BeginAnimation(Canvas.OpacityProperty, null);
            this.BeginAnimation(Canvas.MarginProperty, null);
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
            imgDummy.BeginAnimation(Image.OpacityProperty, animeOpacity);

            var animeMargin = new ThicknessAnimation();
            animeMargin.To = new Thickness()
            {
                Left = imgDummy.Margin.Left,
                Top = mainWindow.canvasUI.Height - offsetTop - imgDummy.Height - 100
            };
            animeMargin.Duration = new Duration(TimeSpan.FromSeconds(time_span));
            animeMargin.Completed += animeRemoveHint_Completed;
            imgDummy.BeginAnimation(Image.MarginProperty, animeMargin);
        }
        private void animeRemoveHint_Completed(object? sender, EventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var imgDummy = (Image)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, "DummyHintPower");
            if (imgDummy == null)
            {
                return;
            }

            // ダミー画像を消す
            imgDummy.BeginAnimation(Image.OpacityProperty, null);
            imgDummy.BeginAnimation(Image.MarginProperty, null);
            mainWindow.canvasUI.Children.Remove(imgDummy);
        }

    }
}
