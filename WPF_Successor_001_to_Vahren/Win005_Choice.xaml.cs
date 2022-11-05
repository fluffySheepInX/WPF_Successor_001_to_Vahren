using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// Win005_Choice.xaml の相互作用ロジック
    /// </summary>
    public partial class Win005_Choice : Window
    {
        #region 閉じるボタン
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        #endregion

        public int ChoiceNumber { get; set; }

        public Win005_Choice(List<string> ChoiceList)
        {
            InitializeComponent();

            int buHeight = 50;

            Canvas canvas = new Canvas();
            canvas.Height = (30 + buHeight) * ChoiceList.Count + 30 + 30;
            this.borChoice.Height = canvas.Height;
            canvas.Width = this.borChoice.Width;
            canvas.Margin = new Thickness()
            {
                Left = 0,
                Top = 0
            };

            if (ChoiceList.Count == 0)
            {
                throw new Exception();
            }

            for (int i = 0; i < ChoiceList.Count; i++)
            {
                Button button = new Button();
                button.Content = ChoiceList[i];
                button.HorizontalContentAlignment = HorizontalAlignment.Center;
                button.Width = canvas.Width - 50;
                button.Height = buHeight;
                button.Margin = new Thickness()
                {
                    Left = (canvas.Width / 2) - (button.Width / 2),
                    Top = (30 + button.Height) * i + 30
                };
                button.Tag = i;
                button.Click += Button_Click;
                canvas.Children.Add(button);
            }

            this.borChoice.Child = canvas;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //閉じるボタン用
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.ChoiceNumber = Convert.ToInt32(((Button)sender).Tag);
            this.Close();
        }
    }
}
