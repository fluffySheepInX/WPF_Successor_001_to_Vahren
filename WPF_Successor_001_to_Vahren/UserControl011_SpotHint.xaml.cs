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
            int spot_capacity = classPowerAndCity.ClassSpot.Capacity;
            int tile_height = 32, tile_width = 32;
            var listTroop = classPowerAndCity.ClassSpot.UnitGroup;
            int troop_count = listTroop.Count();
            int member_max = 0;
            if (troop_count > 0)
            {
                // 画像のディレクトリ
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("040_ChipImage");
                string pathDirectory = System.IO.Path.Combine(strings.ToArray()) + System.IO.Path.DirectorySeparatorChar;

                // 部隊数によって画像サイズを変える
                // ８部隊、８人、までなら32ドット。それを超えると画像が途切れる。
                tile_height = 256 / troop_count;
                if (tile_height > 32)
                {
                    tile_height = 32;
                }
                else if (tile_height < 16)
                {
                    tile_height = 16;
                }
                int member_capacity = mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].MemberCapacity;
                tile_width = 384 / member_capacity;
                if (tile_width > 32)
                {
                    tile_width = 32;
                }
                else if (tile_width < 16)
                {
                    tile_width = 16;
                }

                // ユニットを並べる
                int j;
                foreach (var itemTroop in listTroop)
                {
                    // 部隊のパネル
                    StackPanel panelTroop = new StackPanel();
                    panelTroop.Orientation = Orientation.Horizontal;
                    panelTroop.Height = tile_height;

                    j = 0;
                    foreach (var itemUnit in itemTroop.ListClassUnit)
                    {
                        // ユニットの画像
                        BitmapImage bitimg1 = new BitmapImage(new Uri(pathDirectory + itemUnit.Image));
                        Int32Rect rect = new Int32Rect(0, 0, tile_width, tile_height);
                        var destimg = new CroppedBitmap(bitimg1, rect);

                        Image imgUnit = new Image();
                        imgUnit.Source = destimg;
                        imgUnit.Width = tile_width;
                        imgUnit.Height = tile_height;
                        panelTroop.Children.Add(imgUnit);

                        j++;
                    }
                    if (member_max < j)
                    {
                        member_max = j;
                    }

                    this.panelSpotUnit.Children.Add(panelTroop);
                }
            }

            // 経済、城壁、部隊、戦力
            this.txtStatus.Text = "経済" + classPowerAndCity.ClassSpot.Gain.ToString()
                                + " 城壁" + classPowerAndCity.ClassSpot.Castle.ToString()
                                + " 部隊" + troop_count.ToString() + "/" + spot_capacity.ToString()
                                + " 戦力 ?";

            // ウインドウの大きさを調節する
            if (troop_count > 0)
            {
                this.borderWindow.Height = 85 + tile_height * troop_count + 10;
            }
            if (10 + tile_width * member_max + 10 > 400)
            {
                this.borderWindow.Height = 10 + tile_width * member_max + 10;
            }

        }

    }
}
