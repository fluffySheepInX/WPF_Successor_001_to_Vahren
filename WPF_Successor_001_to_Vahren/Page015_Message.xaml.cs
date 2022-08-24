using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
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
    /// Page015_Message.xaml の相互作用ロジック
    /// </summary>
    public partial class Page015_Message : Page
    {
        public Page015_Message()
        {
            InitializeComponent();
        }

        private void canvasMessage_KeyUp(object sender, KeyEventArgs e)
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
            mainWindow.condition.Signal();
            Thread.Sleep(1);
            mainWindow.condition.Reset();
        }
    }
}
