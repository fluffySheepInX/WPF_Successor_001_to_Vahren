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
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;
using Image = System.Windows.Controls.Image;

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// Page025_Battle_Command.xaml の相互作用ロジック
    /// </summary>
    public partial class Page025_Battle_Command : Page
    {
        public Page025_Battle_Command()
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

            List<string> list = new List<string>();
            foreach (var item in mainWindow.ClassGameStatus
            .ClassBattle.SortieUnitGroup)
            {
                foreach (var itemGroupBy in item.ListClassUnit.GroupBy(x => x.SkillName))
                {
                    list.AddRange(itemGroupBy.Key.ToList());
                }
            }
            list = list.Distinct().ToList();

            foreach (var item in list)
            {
                var result = mainWindow.ClassGameStatus.ListSkill.Where(x => x.NameTag == item).FirstOrDefault();
                if (result == null)
                {
                    continue;
                }
                result.Icon.Reverse();
                Canvas canvas = new Canvas();
                canvas.HorizontalAlignment = HorizontalAlignment.Left;
                canvas.VerticalAlignment = VerticalAlignment.Top;
                canvas.Margin = new Thickness(0, 0, 0, 0);
                foreach (var itemIcon in result.Icon)
                {
                    List<string> strings = new List<string>();
                    strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                    strings.Add("041_ChipImageSkill");
                    strings.Add(itemIcon);
                    string path = System.IO.Path.Combine(strings.ToArray());

                    var bi = new BitmapImage(new Uri(path));
                    Image image = new Image();
                    image.Stretch = Stretch.Fill;
                    image.Source = bi;
                    image.Margin = new Thickness(0, 0, 0, 0);
                    image.Height = 32;
                    image.Width = 32;
                    image.HorizontalAlignment = HorizontalAlignment.Left;
                    image.VerticalAlignment = VerticalAlignment.Top;
                    canvas.Children.Add(image);
                }

                Button button = new Button();
                button.Content = canvas;
                button.HorizontalAlignment = HorizontalAlignment.Left;
                button.VerticalAlignment = VerticalAlignment.Top;
                button.Width = 32;
                button.Height = 32;
                button.HorizontalContentAlignment = HorizontalAlignment.Left;
                button.VerticalContentAlignment = VerticalAlignment.Top;
                this.stkSkill.Children.Add(button);
            }
        }
    }
}
