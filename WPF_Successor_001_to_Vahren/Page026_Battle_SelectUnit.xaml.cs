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
using WPF_Successor_001_to_Vahren._010_Enum;

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// Page026_Battle_SelectUnit.xaml の相互作用ロジック
    /// </summary>
    public partial class Page026_Battle_SelectUnit : Page
    {
        public Page026_Battle_SelectUnit()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.Properties["window"];
            if (window == null)
            {
                return;
            }
            var mainWindow = window as MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            foreach (var item in mainWindow.ClassGameStatus
                        .ClassBattle.SortieUnitGroup)
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("040_ChipImage");
                strings.Add(item.ListClassUnit[0].Image);
                string path = System.IO.Path.Combine(strings.ToArray());

                var bi = new BitmapImage(new Uri(path));
                ImageBrush image = new ImageBrush();
                image.Stretch = Stretch.Fill;
                image.ImageSource = bi;
                Button button = new Button();
                button.Background = image;
                button.Width = 32;
                button.Height = 32;
                this.stackUnit.Children.Add(button);
            }

            this.stackUnit.Width = mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Count * 32;
            this.borPage026.Width += mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Count * 32;
        }
    }
}
