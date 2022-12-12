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
    /// UserControl045_Skill.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl045_Skill : UserControl
    {
        public UserControl045_Skill()
        {
            InitializeComponent();
        }

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
            if ( (listWindow != null) && (listWindow.Any()) )
            {
                int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                Canvas.SetZIndex(this, maxZ + 1);
            }

            // スキルの情報を表示する
            DisplaySkillInfo(mainWindow);

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

        // 既に表示されていて、表示を更新する際
        public void DisplaySkillInfo(MainWindow mainWindow)
        {
            var classSkill = (ClassSkill)this.Tag;
            if (classSkill == null)
            {
                return;
            }

            // スキルセット(Yellow)やリーダースキル(White)・アシストスキルなど
            // 文字の色が用途によって違う。
            // まだデータが存在しないのでセットしない。

            // スキルのアイコンを重ねて表示する
            foreach (var itemIcon in Enumerable.Reverse(classSkill.Icon).ToList())
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("041_ChipImageSkill");
                strings.Add(itemIcon);
                string path = System.IO.Path.Combine(strings.ToArray());
                if (System.IO.File.Exists(path) == false)
                {
                    // スキル画像が存在しない場合はスキップする
                    continue;
                }
                var bitmap = new BitmapImage(new Uri(path));
                Image imageSkill = new Image();
                imageSkill.Source = bitmap;
                // 小さな画像はそのままのサイズで表示する
                if ((bitmap.PixelWidth < 32) && (bitmap.PixelHeight < 32))
                {
                    imageSkill.Width = bitmap.PixelWidth;
                    imageSkill.Height = bitmap.PixelHeight;
                }
                else
                {
                    imageSkill.Width = 32;
                    imageSkill.Height = 32;
                }
                gridSkillIcon.Children.Add(imageSkill);
            }

            // スキル名
            this.txtNameSkill.Text = classSkill.Name;

            // Func種類
            switch (classSkill.Func)
            {
                case _010_Enum.SkillFunc.sword:
                    if (classSkill.Mp > 0)
                    {
                        this.txtFunc.Text = "（接近攻撃）消費MP" + classSkill.Mp.ToString();
                    }
                    else
                    {
                        this.txtFunc.Text = "（接近攻撃）";
                    }
                    break;
                default: // 設定が無い場合は遠距離攻撃・攻撃魔法にする
                    if (classSkill.Mp > 0)
                    {
                        this.txtFunc.Text = "（攻撃魔法）消費MP" + classSkill.Mp.ToString();
                    }
                    else
                    {
                        this.txtFunc.Text = "（遠距離攻撃）";
                    }
                    break;
            }

            // ヘルプ
            this.txtHelp.Text = classSkill.Help;

            // 実験用
            this.txtDefault.Text = "ウインドウ名 = " + this.Name
                    + "\nNameTag = " + classSkill.NameTag
                    + "\nSlowPer = " + classSkill.SlowPer.ToString()
                    + "\nSlowTime = " + classSkill.SlowTime.ToString()
                    + "\nAttr = " + classSkill.Attr
                    + "\nStr.Item1 = " + classSkill.Str.Item1
                    + "\nStr.Item2 = " + classSkill.Str.Item2.ToString()
                    + "\nRange = " + classSkill.Range.ToString()
                    + "\nDamageRangeAdjust = " + classSkill.DamageRangeAdjust.ToString()
                    + "\nRangeMin = " + classSkill.RangeMin.ToString()
                    + "\nSpeed = " + classSkill.Speed.ToString()
                    + "\nGunDelay.Item1 = " + classSkill.GunDelay.Item1
                    + "\nGunDelay.Item2 = " + classSkill.GunDelay.Item2.ToString();


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
                if ( (listWindow != null) && (listWindow.Any()) )
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
                thickness.Left = Math.Truncate(this.Margin.Left + (pt.X - _startPoint.X));
                thickness.Top = Math.Truncate(this.Margin.Top + (pt.Y - _startPoint.Y));
                this.Margin = thickness;
            }
        }
        #endregion

    }
}
