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
using WPF_Successor_001_to_Vahren._006_ClassStatic;
using WPF_Successor_001_to_Vahren._020_AST;
using WPF_Successor_001_to_Vahren._030_Evaluator;

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// UserControl080_InternalAffairs.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl080_InternalAffairs : UserControl
    {
        private List<ClassInternalAffairsDetail> _internalAffairsList = new List<ClassInternalAffairsDetail>();

        public UserControl080_InternalAffairs()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            InitializeComponent();

            //_internalAffairsList.AddRange(CreateInternalAffairsList());

            _internalAffairsList.AddRange(mainWindow.ClassGameStatus.ListClassInternalAffairsDetail);
            AddTreeViewItems(mainWindow.GetPathDirectoryGameTitleFullName());
        }

        private List<ClassInternalAffairsDetail> CreateInternalAffairsList()
        {
            //内政クラスのリストを作成するための処理を実装する
            //ここでは、適当なリストを返すだけに留める
            return new List<ClassInternalAffairsDetail>()
            {
                new ClassInternalAffairsDetail() { NameTag = "1", Title = "内政aaaa", Image = "image1.jpg" },
                new ClassInternalAffairsDetail() { NameTag = "2", Title = "内政2", Image = "image2.jpg" },
                new ClassInternalAffairsDetail() { NameTag = "3", Title = "内政3", Image = "image3.jpg" }
            };
        }

        private void AddTreeViewItems(string pathDirectoryGameTitleFullName)
        {
            foreach (var item in _internalAffairsList)
            {
                var treeViewItem = new TreeViewItem();

                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;

                List<string> strings = new List<string>();
                strings.Add(pathDirectoryGameTitleFullName);
                strings.Add("007_InternalAffairsDetail");
                strings.Add(item.Image);
                string path = System.IO.Path.Combine(strings.ToArray());

                var bi = new BitmapImage(new Uri(path));
                Image image = new Image();
                image.Stretch = Stretch.Fill;
                image.Source = bi;
                image.Margin = new Thickness(0, 0, 0, 0);
                image.Height = 104;
                image.Width = 512;
                image.HorizontalAlignment = HorizontalAlignment.Left;
                image.VerticalAlignment = VerticalAlignment.Top;

                stackPanel.Children.Add(image);
                stackPanel.Children.Add(new TextBlock() { FontSize = 32, Height = 96, Text = item.Title });
                treeViewItem.Header = stackPanel;
                treeViewItem.Tag = item;
                //TreeViewItemに対するクリックイベントを設定する場合
                treeViewItem.MouseLeftButtonUp += TreeViewItem_MouseLeftButtonUp;

                this.tvInternalAffairs.Items.Add(treeViewItem);
            }
        }

        //TreeViewItemに対するクリックイベントを実装する場合
        private void TreeViewItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var tvi = sender as TreeViewItem;
            if (tvi == null)
            {
                return;
            }

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var selectedInternalAffairs = tvi.Tag as ClassInternalAffairsDetail;
            if (selectedInternalAffairs == null)
            {
                return;
            }

            {
                var dialog = new Win025_Select();
                dialog.SetText(ClassStaticCommonMethod.MoldingText(selectedInternalAffairs.Title + "を実行しますか？", "$"));
                bool? result = dialog.ShowDialog();
                if (result == false)
                {
                    return;
                }
            }

            mainWindow.ClassGameStatus.NowListClassInternalAffairsDetail.Add(selectedInternalAffairs);

            {
                var dialog = new Win020_Dialog();
                dialog.SetText(ClassStaticCommonMethod.MoldingText(selectedInternalAffairs.Title + "を実行しました", "$"));
                dialog.ShowDialog();
            }

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

            // ルーティングを処理済みとしてマークする（親コントロールのイベントが発生しなくなる）
            e.Handled = true;
        }
    }
}
