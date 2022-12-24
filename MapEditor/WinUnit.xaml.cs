using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MapEditor
{
    /// <summary>
    /// UserControlUnit.xaml の相互作用ロジック
    /// </summary>
    public partial class WinUnit : Window
    {
        public WinUnit()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        public class Houkou
        {
            public int id { get; set; }
            public string value { get; set; } = string.Empty;
        }

        private void Label_Loaded(object sender, RoutedEventArgs e)
        {
            var ri = (ComboBox)LogicalTreeHelper.FindLogicalNode(this, "cmbHoukou");
            if (ri == null)
            {
                return;
            }

            ri.SelectedValuePath = "id";
            ri.DisplayMemberPath = "value";
            ObservableCollection<Houkou> source = new ObservableCollection<Houkou>();
            source.Add(new Houkou() { id = 0, value = "東" });
            source.Add(new Houkou() { id = 0, value = "西" });
            source.Add(new Houkou() { id = 0, value = "北" });
            source.Add(new Houkou() { id = 0, value = "南" });
            ri.ItemsSource = source;
            //comboBox.FontSize = comboBox.FontSize + 5;
            ri.SelectedIndex = -1;

        }

        private void btnDecide_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
