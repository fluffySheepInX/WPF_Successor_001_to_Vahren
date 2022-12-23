using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
    public partial class Page015_Message : UserControl
    {
        public MainWindow? MyProperty { get; set; } = null;

        public Page015_Message()
        {
            InitializeComponent();

            var window = Application.Current.Properties["window"];
            if (window is null) return;
            if (window is not MainWindow) return;
            MyProperty = window as MainWindow;

            var con = Convert.ToString(Application.Current.Properties["message"]);
            if (con is null) return;
            Label1.Content = con;
        }

        private void canvasMessage_KeyUp(object sender, KeyEventArgs e)
        {
            if (MyProperty == null) return;
            MyProperty.condition.Signal();
        }

        private void canvasMessage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MyProperty == null) return;
            MyProperty.condition.Signal();
        }
    }
}
