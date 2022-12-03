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
            int tile_height = 32, max_width = 0;
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

                    int panel_width = 0;
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
                        panel_width += image_width;
                    }
                    if (max_width < panel_width)
                    {
                        max_width = panel_width;
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

            // ウインドウの大きさを調節する
            if (troop_count > 0)
            {
                this.Height = 85 + tile_height * troop_count + 10;
            }
            if (10 + max_width + 10 > 400)
            {
                this.Width = 10 + max_width + 10;
            }

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
    }
}
