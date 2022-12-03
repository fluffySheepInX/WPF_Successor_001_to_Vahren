using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
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

            // レイアウトが終わるのを待ってから、大きさと位置を調節する
            Dispatcher.BeginInvoke(new Action(() =>
            {
                // ウインドウの高さを調節する
                this.Height = Canvas.GetTop(this.txtDetail) + this.txtDetail.ActualHeight + 20;

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
                        Left = mainWindow.canvasUI.Width - offsetLeft - this.Width,
                        Top = mainWindow.canvasUI.Height - offsetTop - this.Height
                    };
                }
                else
                {
                    // 画面の左下隅に配置する
                    this.Margin = new Thickness()
                    {
                        Left = offsetLeft,
                        Top = mainWindow.canvasUI.Height - offsetTop - this.Height
                    };
                }

            }),
            DispatcherPriority.Loaded);

        }
    }
}
