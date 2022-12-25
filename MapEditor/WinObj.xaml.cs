using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace MapEditor
{
    /// <summary>
    /// WinObj.xaml の相互作用ロジック
    /// </summary>
    public partial class WinObj : Window
    {
        public WinObj()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public int TipSize { get; set; } = 64;
        public int BorSize { get; set; } = 4;
        public string NameSelectionMapTip { get; set; } = string.Empty;
        /// <summary>
        /// 城壁や矢倉など
        /// </summary>
        public List<string> Obj { get; set; } = new List<string>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var a = new CommonOpenFileDialog();
            a.Title = "フォルダを選択してください";
            a.IsFolderPicker = true;
            using (var cofd = a)
            {
                if (cofd.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                string[] names = Directory.GetFiles(cofd.FileName, "*");
                int size = TipSize;
                foreach (string name in names)
                {
                    if (System.IO.Path.GetExtension(name) == ".png")
                    {
                        var bi = new BitmapImage(new Uri(name));
                        Image image = new Image();
                        image.Width = size;
                        image.Height = size;
                        image.Stretch = Stretch.Fill;
                        image.Source = bi;
                        image.Margin = new Thickness(0, 0, 0, 0);
                        Canvas canvas = new Canvas();
                        canvas.Width = size;
                        canvas.Height = size;
                        canvas.Children.Add(image);
                        canvas.MouseLeftButtonUp += ButtonTip_Click;
                        Border border = new Border();
                        border.Width = size + BorSize + BorSize;
                        border.Height = size + BorSize + BorSize;
                        border.BorderBrush = Brushes.Black;
                        border.BorderThickness = new Thickness() { Left = BorSize, Top = BorSize, Right = BorSize, Bottom = BorSize };
                        border.Child = canvas;
                        wrapMaptip.Children.Add(border);
                    }
                }
            }
            this.Topmost = true;
        }
        private void ButtonTip_Click(object sender, RoutedEventArgs e)
        {
            {
                foreach (var item in wrapMaptip.Children)
                {
                    var a = item as Border;
                    if (a == null) continue;
                    a.BorderBrush = Brushes.Black;
                }
            }

            {
                var re = ((Image)((Canvas)sender).Children[0]);
                var bi = (BitmapImage)re.Source;

                this.listObj.Items.Add((bi).UriSource.LocalPath);
                var reB = ((Border)((Canvas)sender).Parent);
                reB.BorderBrush = Brushes.Red;
            }
        }

        private void btnDecide_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.listObj.Items.OfType<string>())
            {
                Obj.Add(item.ToString());
            }
            
            this.Close();
        }
    }
}
