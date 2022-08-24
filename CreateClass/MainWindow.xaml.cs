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

namespace CreateClass
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            TextRange propTextRange = new TextRange(
                                        this.riTarget.Document.ContentStart,
                                        this.riTarget.Document.ContentEnd
                                    );
            TextRange kindTextRange = new TextRange(
                                        this.riTag.Document.ContentStart,
                                        this.riTag.Document.ContentEnd
                                    );
            TextRange getTextRange = new TextRange(
                                        this.riTarget_Copy.Document.ContentStart,
                                        this.riTarget_Copy.Document.ContentEnd
                                    );
            var a = propTextRange.Text.Split(Environment.NewLine);
            var b = kindTextRange.Text.Split(Environment.NewLine);
            propTextRange.Text = string.Empty;
            getTextRange.Text = string.Empty;
            foreach (var item in a.Select((value, index) => (value, index)))
            {
                if (item.value == string.Empty)
                {
                    continue;
                }
                var c = string.Join("", item.value.Skip(1).ToArray());
                var d = item.value[0].ToString().ToUpper() + c;
                {
                    StringBuilder ab = new StringBuilder();
                    ab.AppendLine("        #region " + d);
                    try
                    {
                        if (b[0] == "i")
                        {
                            ab.AppendLine("        private int _" + item.value + ";");
                            ab.AppendLine("        public int " + d);
                        }
                        else
                        {
                            ab.AppendLine("        private string _" + item.value + " = string.Empty;");
                            ab.AppendLine("        public string " + d);
                        }
                    }
                    catch (Exception)
                    {
                        ab.AppendLine("        private string _" + item.value + " = string.Empty;");
                        ab.AppendLine("        public string " + d);
                    }
                    ab.AppendLine("        {");
                    ab.AppendLine("            get { return _" + item.value + "; }");
                    ab.AppendLine("            set { _" + item.value + " = value; }");
                    ab.AppendLine("        }");
                    ab.AppendLine("        #endregion");
                    propTextRange.Text = propTextRange.Text + ab.ToString();
                }

                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("            {");
                    stringBuilder.AppendLine("                var " + item.value + " =");
                    stringBuilder.AppendLine("                    new Regex(GetPat(\"" + item.value + "\"), RegexOptions.IgnoreCase)");
                    stringBuilder.AppendLine("                    .Matches(value);");
                    stringBuilder.AppendLine("                var first = CheckMatchElement(" + item.value + ");");
                    stringBuilder.AppendLine("                if (first == null)");
                    stringBuilder.AppendLine("                {");
                    if (b[0] == "i")
                    {
                        stringBuilder.AppendLine("                    classUnit." + d + " = 0;");
                    }
                    else
                    {
                        stringBuilder.AppendLine("                    classUnit." + d + " = String.Empty;");
                    }
                    stringBuilder.AppendLine("                }");
                    stringBuilder.AppendLine("                else");
                    stringBuilder.AppendLine("                {");
                    if (b[0] == "i")
                    {
                        stringBuilder.AppendLine("                    classUnit." + d + " = Convert.ToInt32(first.Value);");
                    }
                    else
                    {
                        stringBuilder.AppendLine("                    classUnit." + d + " = first.Value.Replace(Environment.NewLine, \"\");");
                    }
                    stringBuilder.AppendLine("                }");
                    stringBuilder.AppendLine("            }");
                    getTextRange.Text = getTextRange.Text + stringBuilder.ToString();
                }
            }
        }
    }
}
