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

            e.Handled = true;
        }

        #region ウインドウ移動
        private bool _isMouse;
        public bool IsMouse
        {
            get { return _isMouse; }
            set { _isMouse = value; }
        }
        private Point _startPoint;
        public Point StartPoint
        {
            get { return _startPoint; }
            set { _startPoint = value; }
        }

        private void win_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

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

            this.IsMouse = true;
            this.StartPoint = e.GetPosition(this);
            e.Handled = true;
        }
        private void win_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.IsMouse = false;
            e.Handled = true;
        }
        private void win_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.IsMouse == false)
            {
                return;
            }

            Point CurrentPoint = e.GetPosition(this);

            var thickness = new Thickness();
            thickness.Left = this.Margin.Left + (CurrentPoint.X - this.StartPoint.X);
            thickness.Top = this.Margin.Top + (CurrentPoint.Y - this.StartPoint.Y);
            this.Margin = thickness;

            e.Handled = true;
        }
        private void win_MouseLeave(object sender, MouseEventArgs e)
        {
            this.IsMouse = false;
            e.Handled = true;
        }
        #endregion

    }
}