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

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// Page030_GameResult.xaml の相互作用ロジック
    /// </summary>
    public partial class Page030_GameResult : UserControl
    {
        public MainWindow? MyProperty { get; set; } = null;

        public Page030_GameResult()
        {
            InitializeComponent();

            var window = Application.Current.Properties["window"];
            if (window is null) return;
            if (window is not MainWindow) return;
            MyProperty = window as MainWindow;
        }

        private void btnBackTitle_Click(object sender, RoutedEventArgs e)
        {
            if (MyProperty == null) return;

            MyProperty.FadeOut = true;

            MyProperty.delegateMainWindowContentRendered = MyProperty.MainWindowContentRendered;

            MyProperty.FadeIn = true;
        }
    }
}
