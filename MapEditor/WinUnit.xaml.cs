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

        public int selectCmbHoukou { get; set; }
        public int selectCmbZinkei { get; set; }
        public string setUnitName { get; set; } = string.Empty;

        private void Label_Loaded(object sender, RoutedEventArgs e)
        {
            var ri = (ComboBox)LogicalTreeHelper.FindLogicalNode(this, "cmbHoukou");
            if (ri == null) return;
            var cmbZinkei = (ComboBox)LogicalTreeHelper.FindLogicalNode(this, "cmbZinkei");
            if (cmbZinkei == null) return;

            {
                ri.SelectedValuePath = "id";
                ri.DisplayMemberPath = "value";
                ObservableCollection<Houkou> source = new ObservableCollection<Houkou>();
                source.Add(new Houkou() { id = 0, value = "東" });
                source.Add(new Houkou() { id = 1, value = "北東" });
                source.Add(new Houkou() { id = 2, value = "北" });
                source.Add(new Houkou() { id = 3, value = "北西" });
                source.Add(new Houkou() { id = 4, value = "西" });
                source.Add(new Houkou() { id = 5, value = "西南" });
                source.Add(new Houkou() { id = 6, value = "南" });
                source.Add(new Houkou() { id = 7, value = "南東" });
                ri.ItemsSource = source;
                //comboBox.FontSize = comboBox.FontSize + 5;
                ri.SelectedIndex = -1;
            }
            {
                cmbZinkei.SelectedValuePath = "id";
                cmbZinkei.DisplayMemberPath = "value";
                ObservableCollection<Houkou> source = new ObservableCollection<Houkou>();
                source.Add(new Houkou() { id = 0, value = "方陣" });
                source.Add(new Houkou() { id = 1, value = "横列" });
                source.Add(new Houkou() { id = 2, value = "縦列" });
                source.Add(new Houkou() { id = 3, value = "密集" });
                cmbZinkei.ItemsSource = source;
                //comboBox.FontSize = comboBox.FontSize + 5;
                cmbZinkei.SelectedIndex = -1;
            }

        }

        private void btnDecide_Click(object sender, RoutedEventArgs e)
        {
            this.selectCmbHoukou = this.cmbHoukou.SelectedIndex;
            this.selectCmbZinkei = this.cmbZinkei.SelectedIndex;
            this.setUnitName = "@" + this.txtUnitName.Text;
            //MessageBox.Show("方向" + this.selectCmbHoukou + ",陣形" + this.selectCmbZinkei);
            this.Close();
        }
    }
}
