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
    /// UserControl006_Spot.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl006_Spot : UserControl
    {
        public UserControl006_Spot()
        {
            InitializeComponent();
        }

        public void SetData()
        {
            DisplaySpotStatus();
        }

        public void DisplaySpotStatus()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
            	return;
            }

            var classPowerAndCity = (ClassPowerAndCity)this.Tag;
            int spot_capacity = classPowerAndCity.ClassSpot.Capacity;

            //旗は存在する時だけ
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
            //領地名
            {
                //this.lblNameSpot.Content = this.Name; // ウインドウ番号を表示する実験用
                this.lblNameSpot.Content = classPowerAndCity.ClassSpot.Name;
            }
            //経済値
            {
                this.lblGain.Content = classPowerAndCity.ClassSpot.Gain;
            }
            //城壁値
            {
                this.lblCastle.Content = classPowerAndCity.ClassSpot.Castle;
            }
            //部隊駐留数
            {
                int count = mainWindow.ClassGameStatus.AllListSpot
                    .Where(x => x.NameTag == classPowerAndCity.ClassSpot.NameTag)
                    .First()
                    .UnitGroup
                    .Where(x => x.Spot.NameTag == classPowerAndCity.ClassSpot.NameTag)
                    .Count();
                this.lblMemberCount.Content = count.ToString() + "/" + spot_capacity.ToString();
            }


            // 最前面に配置する
            try
            {
                int maxZ = mainWindow.canvasUI.Children.OfType<UIElement>()
                   .Where(x => x != this)
                   .Select(x => Panel.GetZIndex(x))
                   .Max();
                Canvas.SetZIndex(this, maxZ + 1);
            }
            catch (InvalidOperationException)
            {
                // 比較する子ウインドウがなければそのまま
            }

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 使用していたウインドウ番号の登録を抹消する
            string window_name = this.Name.Replace("WindowSpot", String.Empty);
            int window_id = Int32.Parse(window_name);
            mainWindow.ClassGameStatus.ListWindowSpot.Remove(window_id);

            // キャンバスから自身を取り除く
            // ガベージ・コレクタが掃除してくれるか不明
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
                try
                {
                    int maxZ = mainWindow.canvasUI.Children.OfType<UIElement>()
                       .Where(x => x != this)
                       .Select(x => Panel.GetZIndex(x))
                       .Max();
                    Canvas.SetZIndex(this, maxZ + 1);
                }
                catch (InvalidOperationException)
                {
                    // 比較する子ウインドウがなければそのまま
                }
            }

            // ドラッグを開始する
            UIElement el = (UIElement)sender;
            if (el != null)
            {
                _isDrag = true;
                _startPoint = e.GetPosition(el);
                el.CaptureMouse();
            }
        }
        private void win_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // ドラック中なら終了する
            if (_isDrag == true)
            {
                UIElement el = (UIElement)sender;
                el.ReleaseMouseCapture();
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

    }
}