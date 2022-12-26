using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
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
using System.Xml.Linq;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;
using Image = System.Windows.Controls.Image;

namespace MapEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<List<ClassMap>>? MapData { get; set; } = null;
        public string NameSelectionMapTip { get; set; } = string.Empty;
        public bool NameSelectionMapTipObj { get; set; } = false;
        public int TipSize { get; set; } = 64;
        public int BorSize { get; set; } = 4;
        public List<string> fileTips { get; set; } = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void win_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtMapHeight.Text = "50";
            this.txtMapWidth.Text = "50";
            this.TipSize = (int)(slTipSize.Value * 12.8);
            int size = TipSize;

            ////スクロールバーの幅はパブリックとして公開されていない？
            //double a = (this.grdCanvas.ActualWidth - 17) / double.Parse(this.txtMapWidth.Text);
            this.slTipSize.Value = 5;

            wrapCanvas.Rows = int.Parse(this.txtMapHeight.Text);
            wrapCanvas.Columns = int.Parse(this.txtMapWidth.Text);
            gridMaptip.Height = (size * wrapCanvas.Rows);
            gridMaptip.Width = (size * wrapCanvas.Columns);

            MapData = new List<List<ClassMap>>();

            for (int i = 0; i < wrapCanvas.Columns; i++)
            {
                MapData.Add(new List<ClassMap>());
                for (int j = 0; j < wrapCanvas.Rows; j++)
                {
                    MapData[i].Add(new ClassMap());
                    DisplayGrid(size, i, j);
                }
            }
        }

        private void DisplayGrid(int size, int col, int hei)
        {
            Border border = new Border();
            border.Width = size;
            border.Height = size;
            border.BorderBrush = Brushes.Black;
            border.BorderThickness = new Thickness() { Left = 1, Top = 1, Right = 1, Bottom = 1 };
            var cm = new ContextMenu();
            {
                var i1 = new MenuItem() { Header = "退却位置とする" };
                cm.Items.Add(i1);
                i1.Click += CmMenu1_Click;
                i1.Tag = "A" + col + "," + hei;
            }
            {
                var i1 = new MenuItem() { Header = "出撃位置とする" };
                cm.Items.Add(i1);
                i1.Click += CmMenu2_Click;
                i1.Tag = "A" + col + "," + hei;
            }
            {
                var i1 = new MenuItem() { Header = "防衛位置とする" };
                cm.Items.Add(i1);
                i1.Click += CmMenu3_Click;
                i1.Tag = "A" + col + "," + hei;
            }
            {
                var i1 = new MenuItem() { Header = "オブジェクトを配置する（複数可能" };
                cm.Items.Add(i1);
                i1.Click += CmMenu4_Click;
                i1.Tag = "A" + col + "," + hei;
            }
            {
                var i1 = new MenuItem() { Header = "オブジェクトを削除する" };
                cm.Items.Add(i1);
                i1.Click += CmMenu6_Click;
                i1.Tag = "A" + col + "," + hei;
            }
            {
                var i1 = new MenuItem() { Header = "ユニット、陣形、方角を指定する(イベント戦で有効)" };
                cm.Items.Add(i1);
                i1.Click += CmMenu5_Click;
                i1.Tag = "A" + col + "," + hei;
            }
            {
                var i1 = new MenuItem() { Header = "ユニット、陣形、方角を削除する" };
                cm.Items.Add(i1);
                i1.Click += CmMenu7_Click;
                i1.Tag = "A" + col + "," + hei;
            }
            border.ContextMenu = cm;

            Canvas canvas = new Canvas();
            canvas.Width = size;
            canvas.Height = size;

            if (MapData != null)
            {
                if (MapData[col][hei].field == String.Empty)
                {
                    canvas.Background = Brushes.AliceBlue;
                }
                else
                {
                    var bi = new BitmapImage(new Uri(MapData[col][hei].field));
                    Image image = new Image();
                    image.Width = slTipSize.Value * 12.8;
                    image.Height = slTipSize.Value * 12.8;
                    image.Stretch = Stretch.Fill;
                    image.Source = bi;
                    image.Margin = new Thickness(0, 0, 0, 0);
                    canvas.Children.Add(image);
                }
            }

            canvas.MouseEnter += ButtonMap_MouseEnter;
            canvas.MouseLeftButtonDown += ButtonMap_Click;
            canvas.Tag = col + "," + hei;
            border.Child = canvas;

            this.wrapCanvas.Children.Add(border);
        }

        #region 退却位置とする
        /// <summary>
        /// 退却位置とする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmMenu1_Click(object sender, RoutedEventArgs e)
        {
            if (MapData is null) return;

            var aaa = (MenuItem)sender;
            var bbb = Convert.ToString(aaa.Tag);
            if (bbb is null) return;
            bbb = string.Join("", bbb.Skip(1).ToList());
            var ccc = bbb.Split(',');
            int col = int.Parse(ccc[0]);
            int hei = int.Parse(ccc[1]);
            MapData[col][hei].unit = "@ESC@";
            MessageBox.Show(col + "," + hei + "に" + MapData[col][hei].unit + "を入れました。");
        }
        #endregion
        #region 出撃位置とする
        /// <summary>
        /// 出撃位置とする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmMenu2_Click(object sender, RoutedEventArgs e)
        {
            if (MapData is null) return;

            var aaa = (MenuItem)sender;
            var bbb = Convert.ToString(aaa.Tag);
            if (bbb is null) return;
            bbb = string.Join("", bbb.Skip(1).ToList());
            var ccc = bbb.Split(',');
            int col = int.Parse(ccc[0]);
            int hei = int.Parse(ccc[1]);
            MapData[col][hei].unit = "@@";
            MessageBox.Show(col + "," + hei + "に" + MapData[col][hei].unit + "を入れました。");
        }
        #endregion
        #region 防衛位置とする
        /// <summary>
        /// 防衛位置とする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmMenu3_Click(object sender, RoutedEventArgs e)
        {
            if (MapData is null) return;

            var aaa = (MenuItem)sender;
            var bbb = Convert.ToString(aaa.Tag);
            if (bbb is null) return;
            bbb = string.Join("", bbb.Skip(1).ToList());
            var ccc = bbb.Split(',');
            int col = int.Parse(ccc[0]);
            int hei = int.Parse(ccc[1]);
            MapData[col][hei].unit = "@";
            MessageBox.Show(col + "," + hei + "に" + MapData[col][hei].unit + "を入れました。");
        }
        #endregion
        #region オブジェクトを配置する（複数可能
        /// <summary>
        /// オブジェクトを配置する（複数可能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmMenu4_Click(object sender, RoutedEventArgs e)
        {
            if (MapData is null) return;

            var dialog = new WinObj();
            dialog.ShowDialog();
            var aaa = (MenuItem)sender;
            var bbb = Convert.ToString(aaa.Tag);
            if (bbb is null) return;
            bbb = string.Join("", bbb.Skip(1).ToList());
            var ccc = bbb.Split(',');
            int col = int.Parse(ccc[0]);
            int hei = int.Parse(ccc[1]);
            MapData[col][hei].build = dialog.Obj;

            int abc = (hei * MapData[col].Count) + col;
            var re = (Canvas)((Border)this.wrapCanvas.Children[abc]).Child;
            re.Children.Add(new TextBlock() { Name = "txtObj", Text = "obj" + System.Environment.NewLine, FontSize = FontSize + 10 });
        }
        #endregion
        #region ユニット、陣形、方角を指定する(イベント戦で有効)
        /// <summary>
        /// ユニット、陣形、方角を指定する(イベント戦で有効)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmMenu5_Click(object sender, RoutedEventArgs e)
        {
            if (MapData is null) return;

            var dialog = new WinUnit();
            dialog.ShowDialog();
            var aaa = (MenuItem)sender;
            var bbb = Convert.ToString(aaa.Tag);
            if (bbb is null) return;
            bbb = string.Join("", bbb.Skip(1).ToList());
            var ccc = bbb.Split(',');
            int col = int.Parse(ccc[0]);
            int hei = int.Parse(ccc[1]);
            MapData[col][hei].direction = dialog.selectCmbHoukou.ToString();
            MapData[col][hei].formation = dialog.selectCmbZinkei.ToString();
            MapData[col][hei].unit = dialog.setUnitName;
            MessageBox.Show(col + "," + hei + "に"
                            + MapData[col][hei].direction
                            + "&"
                            + MapData[col][hei].formation
                            + "&"
                            + MapData[col][hei].unit
                            + "を入れました。");
            int abc = (hei * MapData[col].Count) + col;
            var re = (Canvas)((Border)this.wrapCanvas.Children[abc]).Child;
            re.Children.Add(new TextBlock() { Name = "txtUnit", Text = "unit" + System.Environment.NewLine, FontSize = FontSize + 10 });

        }
        #endregion

        private void CmMenu6_Click(object sender, RoutedEventArgs e)
        {
            if (MapData is null) return;

            var aaa = (MenuItem)sender;
            var bbb = Convert.ToString(aaa.Tag);
            if (bbb is null) return;
            bbb = string.Join("", bbb.Skip(1).ToList());
            var ccc = bbb.Split(',');
            int col = int.Parse(ccc[0]);
            int hei = int.Parse(ccc[1]);
            MapData[col][hei].build = new List<string>();

            int abc = (hei * MapData[col].Count) + col;
            var re = (Canvas)((Border)this.wrapCanvas.Children[abc]).Child;
            {
                var ri = (TextBlock)LogicalTreeHelper.FindLogicalNode(re, "txtObj");
                if (ri != null)
                {
                    re.Children.Remove(ri);
                }
            }
        }
        private void CmMenu7_Click(object sender, RoutedEventArgs e)
        {
            if (MapData is null) return;

            var aaa = (MenuItem)sender;
            var bbb = Convert.ToString(aaa.Tag);
            if (bbb is null) return;
            bbb = string.Join("", bbb.Skip(1).ToList());
            var ccc = bbb.Split(',');
            int col = int.Parse(ccc[0]);
            int hei = int.Parse(ccc[1]);
            MapData[col][hei].unit = string.Empty;
            MapData[col][hei].direction = string.Empty;
            MapData[col][hei].formation = string.Empty;

            int abc = (hei * MapData[col].Count) + col;
            var re = (Canvas)((Border)this.wrapCanvas.Children[abc]).Child;
            {
                var ri = (TextBlock)LogicalTreeHelper.FindLogicalNode(re, "txtUnit");
                if (ri != null)
                {
                    re.Children.Remove(ri);
                }
            }
        }

        #region 素材のあるフォルダを開く
        /// <summary>
        /// 素材のあるフォルダを開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        fileTips.Add(name);
                    }
                }
            }
        }
        #endregion

        private void ButtonTip_Click(object sender, RoutedEventArgs e)
        {
            {
                foreach (var item in wrapMaptip.Children)
                {
                    var a = item as Border;
                    if (a == null) continue;
                    a.BorderBrush = Brushes.Black;
                    //a.BorderThickness = new Thickness() { Left = 1, Top = 1, Right = 1, Bottom = 1 };
                    //var reCan = ((Canvas)sender);
                    //reCan.Height = TipSize -2;
                    //reCan.Width = TipSize - 2;
                }
            }

            {
                var re = ((Image)((Canvas)sender).Children[0]);
                var bi = (BitmapImage)re.Source;
                this.NameSelectionMapTip = (bi).UriSource.LocalPath;
                //MessageBox.Show(NameSelectionMapTip);
                var reB = ((Border)((Canvas)sender).Parent);
                reB.BorderBrush = Brushes.Red;
                //reB.BorderThickness = new Thickness() { Left = 4, Top = 4, Right = 4, Bottom = 4 };
                //var reCan = ((Canvas)sender);
                //reCan.Height = TipSize - 8;
                //reCan.Width = TipSize - 8;
                if (bi.Height > 32)
                {
                    NameSelectionMapTipObj = true;
                }
                else
                {
                    NameSelectionMapTipObj = false;
                }
            }
        }
        private void ButtonMap_MouseEnter(object sender, RoutedEventArgs e)
        {
            DisplayTip(sender);
        }

        private void DisplayTip(object sender)
        {
            if (this.NameSelectionMapTip == String.Empty) return;
            if (Mouse.LeftButton != MouseButtonState.Pressed) return;
            if (MapData == null) return;

            var target = (Canvas)sender;
            if (target == null) return;
            if (target.Tag == null) return;

            string? abc = Convert.ToString(target.Tag);
            if (abc == null) return;

            var strings = abc.Split(",");
            int first = int.Parse(strings[0]);
            int second = int.Parse(strings[1]);
            if (this.NameSelectionMapTipObj == true)
            {
                var li = MapData[first][second].build.ToList();
                li.Add(this.NameSelectionMapTip);
                MapData[first][second]
                    = new ClassMap()
                    {
                        field = MapData[first][second].field,
                        build = li,
                        flag = MapData[first][second].flag,
                        unit = MapData[first][second].unit,
                        direction = MapData[first][second].direction,
                        formation = MapData[first][second].formation
                    };
            }
            else
            {
                MapData[first][second]
                    = new ClassMap()
                    {
                        field = this.NameSelectionMapTip,
                        build = MapData[first][second].build,
                        flag = MapData[first][second].flag,
                        unit = MapData[first][second].unit,
                        direction = MapData[first][second].direction,
                        formation = MapData[first][second].formation
                    };
            }

            target.Children.Clear();
            var bi = new BitmapImage(new Uri(this.NameSelectionMapTip));
            Image image = new Image();
            image.Width = this.TipSize;
            image.Height = this.TipSize;
            image.Stretch = Stretch.Fill;
            image.Source = bi;
            image.Margin = new Thickness(0, 0, 0, 0);
            target.Children.Add(image);
        }

        private void ButtonMap_Click(object sender, RoutedEventArgs e)
        {
            DisplayTip(sender);
        }

        private void btnExecuteGridSizeChange_Click(object sender, RoutedEventArgs e)
        {
            int size = TipSize;
            wrapCanvas.Children.Clear();
            wrapCanvas.Rows = int.Parse(this.txtMapHeight.Text);
            wrapCanvas.Columns = int.Parse(this.txtMapWidth.Text);
            gridMaptip.Height = (size * wrapCanvas.Rows);
            gridMaptip.Width = (size * wrapCanvas.Columns);

            MapData = new List<List<ClassMap>>();

            for (int i = 0; i < wrapCanvas.Columns; i++)
            {
                MapData.Add(new List<ClassMap>());
                for (int j = 0; j < wrapCanvas.Rows; j++)
                {
                    MapData[i].Add(new ClassMap());
                    DisplayGrid(size, i, j);
                }
            }
        }

        private void heiUp_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtMapHeight.Text == String.Empty) return;
            this.txtMapHeight.Text = (int.Parse(this.txtMapHeight.Text) + 1).ToString();
        }

        private void heiDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtMapHeight.Text == String.Empty) return;
            this.txtMapHeight.Text = (int.Parse(this.txtMapHeight.Text) - 1).ToString();
        }

        private void widUp_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtMapWidth.Text == String.Empty) return;
            this.txtMapWidth.Text = (int.Parse(this.txtMapWidth.Text) + 1).ToString();
        }

        private void widDown_Click(object sender, RoutedEventArgs e)
        {
            if (this.txtMapWidth.Text == String.Empty) return;
            this.txtMapWidth.Text = (int.Parse(this.txtMapWidth.Text) - 1).ToString();
        }

        private void slTipSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int size = (int)(e.NewValue * 12.8);

            SizeChangeTip(size);
        }

        private void SizeChangeTip(int size)
        {
            wrapCanvas.Children.Clear();
            wrapCanvas.Rows = int.Parse(this.txtMapHeight.Text);
            wrapCanvas.Columns = int.Parse(this.txtMapWidth.Text);
            gridMaptip.Height = (size * wrapCanvas.Rows);
            gridMaptip.Width = (size * wrapCanvas.Columns);

            for (int i = 0; i < wrapCanvas.Columns; i++)
            {
                for (int j = 0; j < wrapCanvas.Rows; j++)
                {
                    DisplayGrid(size, i, j);
                }
            }
        }

        private void btnBaketu_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("本当に塗りつぶしますか？", "", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No)
            {
                return;
            }

            MapData = new List<List<ClassMap>>();

            for (int i = 0; i < wrapCanvas.Columns; i++)
            {
                MapData.Add(new List<ClassMap>());
                for (int j = 0; j < wrapCanvas.Rows; j++)
                {
                    MapData[i].Add(new ClassMap(this.NameSelectionMapTip, new List<string>(), 0, "", "", ""));
                }
            }

            for (int i = 0; i < wrapCanvas.Columns * wrapCanvas.Rows; i++)
            {
                var re = (Canvas)((Border)this.wrapCanvas.Children[i]).Child;
                re.Children.Clear();
                var bi = new BitmapImage(new Uri(this.NameSelectionMapTip));
                Image image = new Image();
                image.Width = this.TipSize;
                image.Height = this.TipSize;
                image.Stretch = Stretch.Fill;
                image.Source = bi;
                image.Margin = new Thickness(0, 0, 0, 0);
                re.Children.Add(image);
            }
        }

        private void btnSaveNew_Click(object sender, RoutedEventArgs e)
        {
            string fileName = string.Empty;
            string folderName = string.Empty;
            var a = new CommonSaveFileDialog();
            a.Filters.Add(new CommonFileDialogFilter("TEXT", "*.txt"));
            a.Title = "場所を選択してください";
            using (var dia = a)
            {
                if (dia.ShowDialog() != CommonFileDialogResult.Ok)
                {
                    return;
                }

                fileName = dia.FileAsShellObject.ParsingName;
            }

            if (MapData == null)
            {
                return;
            }

            List<string> groupString = new List<string>();
            List<string> groupString2 = new List<string>();
            {
                for (int i = 0; i < MapData.Count; i++)
                {
                    var b = MapData[i].GroupBy(x => x).GroupBy(x => x.Key).Select(x => x.First()).ToList();
                    foreach (var item in b)
                    {
                        groupString.Add(item.Key.field);
                    }
                }
                for (int i = 0; i < MapData.Count; i++)
                {
                    var b = MapData[i].GroupBy(x => x).GroupBy(x => x.Key).Select(x => x.First()).ToList();
                    foreach (var item in b)
                    {
                        foreach (var itemBuild in item.Key.build)
                        {
                            groupString2.Add(itemBuild);
                        }
                    }
                }
            }
            var reGroupString = groupString.GroupBy(x => x).GroupBy(x => x.Key).Select(x => x.First()).ToList();
            var reGroupString2 = groupString2.GroupBy(x => x).GroupBy(x => x.Key).Select(x => x.First()).ToList();
            List<string> target = new List<string>();
            {
                foreach (var item in reGroupString)
                {
                    target.Add(item.Key);
                }
                foreach (var item in reGroupString2)
                {
                    target.Add(item.Key);
                }
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("map " + System.IO.Path.GetFileName(fileName));
            stringBuilder.AppendLine("{");
            Dictionary<string, string> targetString = new Dictionary<string, string>();
            for (int i = 0; i < target.Count; i++)
            {
                string ele = "ele" + i;
                targetString.Add(target[i], ele);
                stringBuilder.AppendLine(ele + " = \"" + System.IO.Path.GetFileNameWithoutExtension(target[i]) + "\";");
            }

            stringBuilder.AppendLine("data = \"");
            for (int i = 0; i < MapData.Count; i++)
            {
                for (int k = 0; k < MapData[i].Count; k++)
                {
                    targetString.TryGetValue(MapData[i][k].field, out string? valueField);
                    if (valueField == null)
                    {
                        continue;
                    }
                    List<string> liGetValueBuild = new List<string>();
                    foreach (var item in MapData[i][k].build)
                    {
                        targetString.TryGetValue(item, out string? valueBuild);
                        if (valueBuild == null)
                        {
                            continue;
                        }
                        else
                        {
                            liGetValueBuild.Add(valueBuild);
                        }
                    }
                    string getValueBuild = string.Empty;
                    if (liGetValueBuild.Count == 0)
                    {
                        getValueBuild = "null";
                    }
                    else
                    {
                        foreach (var item in liGetValueBuild)
                        {
                            getValueBuild = getValueBuild + "$" + System.IO.Path.GetFileNameWithoutExtension(item);
                        }
                    }
                    string unit;
                    if (MapData[i][k].unit == String.Empty)
                    {
                        unit = "null";
                    }
                    else
                    {
                        unit = MapData[i][k].unit;
                    }
                    string houkou;
                    if (MapData[i][k].direction == String.Empty)
                    {
                        houkou = "null";
                    }
                    else
                    {
                        houkou = MapData[i][k].direction;
                    }
                    string zinkei;
                    if (MapData[i][k].formation == String.Empty)
                    {
                        zinkei = "null";
                    }
                    else
                    {
                        zinkei = MapData[i][k].formation;
                    }

                    stringBuilder.Append(System.IO.Path.GetFileNameWithoutExtension(valueField) +
                                    "*" + getValueBuild +
                                    "*" + Convert.ToString(MapData[i][k].flag) +
                                    "*" + unit +
                                    "*" + houkou +
                                    "*" + zinkei +
                                    ",");
                }
                //改行
                stringBuilder.AppendLine("@,");
            }
            stringBuilder.AppendLine("\";");

            stringBuilder.AppendLine("}");
            File.AppendAllText(fileName + ".txt", stringBuilder.ToString());
        }

        private void grdCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.GetKeyStates(Key.LeftCtrl) == KeyStates.Down
                || Keyboard.GetKeyStates(Key.RightCtrl) == KeyStates.Down)
            {
                slTipSize.Value = slTipSize.Value + ((e.Delta > 0) ? 1 : -1);
                SizeChangeTip((int)(slTipSize.Value * 12.8));
            }
        }

        private void btnPreMap_Click(object sender, RoutedEventArgs e)
        {
            var aaa = new WinPreBattle(MapData, fileTips);
            aaa.ShowDialog();
        }
    }
}
