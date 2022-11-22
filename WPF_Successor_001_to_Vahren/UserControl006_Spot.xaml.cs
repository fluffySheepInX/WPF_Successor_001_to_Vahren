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
using WPF_Successor_001_to_Vahren._005_Class;

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// UserControl006_Spot.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl006_Spot : UserControl
    {
        public UserControl006_Spot()
        {
            InitializeComponent();
        }

        // 最初に呼び出した時
        public void SetData()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
            	return;
            }

            DisplaySpotStatus(mainWindow);

            // 最前面に配置する
            try
            {
                int maxZ = mainWindow.canvasUI.Children.OfType<UIElement>()
                   .Where(x => x != this)
                   .Select(x => Canvas.GetZIndex(x))
                   .Max();
                Canvas.SetZIndex(this, maxZ + 1);
            }
            catch (InvalidOperationException)
            {
                // 比較する子ウインドウがなければそのまま
            }
        }

        // 領地のユニットを変更した際に、ユニット表示だけを更新する
        public void UpdateSpotUnit(MainWindow mainWindow)
        {
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;

            // プレイヤーが操作可能かどうか
            bool isControl = false;
            if (classPowerAndCity.ClassPower.NameTag == mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.NameTag)
            {
                // 同じ勢力なら、操作できる
                isControl = true;
            }

            // 陣形リスト
            ObservableCollection<ClassFormation> formation = new ObservableCollection<ClassFormation>();
            formation.Add(new ClassFormation() { Id = 0, Formation = _010_Enum.Formation.F });
            formation.Add(new ClassFormation() { Id = 1, Formation = _010_Enum.Formation.M });
            formation.Add(new ClassFormation() { Id = 2, Formation = _010_Enum.Formation.B });

            // 画像のディレクトリ
            List<string> strings = new List<string>();
            strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
            strings.Add("040_ChipImage");
            string pathDirectory = System.IO.Path.Combine(strings.ToArray()) + System.IO.Path.DirectorySeparatorChar;

            // ユニットのタイルサイズ
            int tile_width = 48, tile_height = 68, header_width = 50;

            // 最初に全て消去する
            this.canvasSpotUnit.Children.Clear();

            var listTroop = mainWindow.ClassGameStatus.AllListSpot
                .Where(x => x.NameTag == classPowerAndCity.ClassSpot.NameTag)
                .First()
                .UnitGroup;
            // 全ての部隊を表示する（駐留数の制限を超えていても許容する）
            int i = 0, j, j_max = 1;
            foreach (var itemTroop in listTroop)
            {
                j = 0;
                foreach (var itemUnit in itemTroop.ListClassUnit)
                {
                    // 部隊の陣形はリーダー（先頭のユニット）を参照する
                    if (j == 0)
                    {
                        if (isControl)
                        {
                            // 出撃ボタン
                            Button btnSelect = new Button();
                            btnSelect.Name = "btnSelect" + i.ToString();
                            btnSelect.Height = 30;
                            btnSelect.Width = header_width - 4;
                            btnSelect.Margin = new Thickness(2);
                            btnSelect.FontSize = 15;
                            btnSelect.Content = "出撃";
                            this.canvasSpotUnit.Children.Add(btnSelect);
                            Canvas.SetTop(btnSelect, tile_height * i);

                            // 陣形コンボボックス
                            ComboBox cmbFormation = new ComboBox();
                            cmbFormation.Name = "cmbFormation" + i.ToString();
                            cmbFormation.Height = 30;
                            cmbFormation.Width = header_width - 4;
                            cmbFormation.Margin = new Thickness(2);
                            cmbFormation.SelectedValuePath = "Id";
                            cmbFormation.DisplayMemberPath = "Formation";
                            cmbFormation.ItemsSource = formation;
                            cmbFormation.FontSize = 15;
                            cmbFormation.SelectedIndex = itemUnit.Formation.Id;
                            this.canvasSpotUnit.Children.Add(cmbFormation);
                            Canvas.SetTop(cmbFormation, tile_height * i + tile_height / 2);
                        }
                        else
                        {
                            // 操作できない場合は、陣形だけ表示する
                            Label label = new Label();
                            label.Background = SystemColors.WindowBrush;
                            label.Width = 30;
                            label.Height = 30;
                            label.Margin = new Thickness(10, 20, 0, 0);
                            label.FontSize = 15;
                            label.Content = itemUnit.Formation.Formation;
                            this.canvasSpotUnit.Children.Add(label);
                            Canvas.SetTop(label, tile_height * i);
                        }
                    }

                    // ユニットのボタン
                    Button btnUnit = new Button();
                    btnUnit.Name = "btnUnit" + i.ToString() + "_" + j.ToString();
                    btnUnit.Height = tile_height;
                    btnUnit.Width = tile_width;
                    btnUnit.Background = Brushes.Transparent;
                    btnUnit.BorderThickness = new Thickness(0);
                    if (isControl)
                    {
                        // 操作可能な時だけドラッグ移動の準備をしておく
                        if (j > 0)
                        {
                            // 部隊メンバーの場合
                            btnUnit.MouseRightButtonDown += unit_MouseRightButtonDown;
                            //btnUnit.MouseMove += unit_MouseMove;
                        }
                    }

                    // ユニットのパネル
                    StackPanel stkUnit = new StackPanel();

                    // ユニットの画像
                    BitmapImage bitimg1 = new BitmapImage(new Uri(pathDirectory + itemUnit.Image));
                    Image imgUnit = new Image();
                    imgUnit.Name = "imgUnit" + i.ToString() + "_" + j.ToString();
                    imgUnit.Source = bitimg1;
                    imgUnit.Width = tile_width;
                    imgUnit.Height = tile_width;
                    // 画像本来のピクセルサイズで表示する場合は、PixelWidth と PixelHeight を指定する
                    //imgUnit.Width = bitimg1.PixelWidth;
                    //imgUnit.Height = bitimg1.PixelHeight;
                    stkUnit.Children.Add(imgUnit);

                    // ユニットのレベル
                    Label lblLevel = new Label();
                    lblLevel.Name = "lblLevel" + i.ToString() + "_" + j.ToString();
                    lblLevel.Height = tile_height - tile_width;
                    lblLevel.FontSize = 15;
                    lblLevel.Padding = new Thickness(-5);
                    lblLevel.Foreground = Brushes.White;
                    lblLevel.HorizontalAlignment = HorizontalAlignment.Center;
                    lblLevel.Content = "lv" + itemUnit.Level;
                    stkUnit.Children.Add(lblLevel);
                    btnUnit.Content = stkUnit;

                    this.canvasSpotUnit.Children.Add(btnUnit);
                    Canvas.SetLeft(btnUnit, header_width + tile_width * j);
                    Canvas.SetTop(btnUnit, tile_height * i);

                    j++;
                    if (j_max < j)
                    {
                        j_max = j;
                    }
                }

                i++;
            }

            // ユニット配置場所の大きさ
            if (isControl)
            {
                // 操作可能な時だけドロップ先の枠の分も確保する
                int member_capacity = mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].MemberCapacity;
                if (j_max < member_capacity)
                {
                    j_max++;
                }
                int spot_capacity = classPowerAndCity.ClassSpot.Capacity;
                if (i < spot_capacity)
                {
                    i++;
                }
            }
            this.canvasSpotUnit.Width = header_width + tile_width * j_max;
            this.canvasSpotUnit.Height = tile_height * i;
        }

        // 既に表示されていて、表示を更新する際
        public void DisplaySpotStatus(MainWindow mainWindow)
        {
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;

            //旗は存在する時だけ
            if (classPowerAndCity.ClassPower.FlagPath != string.Empty)
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("030_FlagImage");
                strings.Add(classPowerAndCity.ClassPower.FlagPath);
                string path = System.IO.Path.Combine(strings.ToArray());
                BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                Int32Rect rect = new Int32Rect(0, 0, 32, 32);
                var destimg = new CroppedBitmap(bitimg1, rect);
                this.imgFlag.Source = destimg;
            }
            //領地名
            {
                //this.lblNameSpot.Content = this.Name; // ウインドウ番号を表示する実験用
                this.lblNameSpot.Content = classPowerAndCity.ClassSpot.Name;
            }
            //経済値
            {
                this.lblGain.Content = classPowerAndCity.ClassSpot.Gain;
            }
            //城壁値
            {
                this.lblCastle.Content = classPowerAndCity.ClassSpot.Castle;
            }
            //部隊駐留数
            {
                int spot_capacity = classPowerAndCity.ClassSpot.Capacity;
                int count = mainWindow.ClassGameStatus.AllListSpot
                    .Where(x => x.NameTag == classPowerAndCity.ClassSpot.NameTag)
                    .First()
                    .UnitGroup
                    .Where(x => x.Spot.NameTag == classPowerAndCity.ClassSpot.NameTag)
                    .Count();
                this.lblMemberCount.Content = count.ToString() + "/" + spot_capacity.ToString();
            }
            //ユニット
            {
                UpdateSpotUnit(mainWindow);
            }


        }

        // ユニットをドラッグ移動する際の、移動先を作る
        private void MakeDropTarget_Unit(MainWindow mainWindow, int troop_id, int member_id)
        {
            // とりあえず、同じ領地上だけ考える
            // 将来的には、他の領地にもドラッグ移動できるようにする

            var classPowerAndCity = (ClassPowerAndCity)this.Tag;

            // ユニットのタイルサイズ
            int tile_width = 48, tile_height = 68, header_width = 50;
            int member_capacity = mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].MemberCapacity;
            int spot_capacity = classPowerAndCity.ClassSpot.Capacity;

            var listTroop = mainWindow.ClassGameStatus.AllListSpot
                .Where(x => x.NameTag == classPowerAndCity.ClassSpot.NameTag)
                .First()
                .UnitGroup;
            int i = 0, j, troop_count, member_count;
            troop_count = listTroop.Count;
            foreach (var itemTroop in listTroop)
            {
                // 他のユニットの位置なら「部隊メンバー入れ替え」動作になる
                member_count = itemTroop.ListClassUnit.Count;
                for (j = 1; j < member_count; j++)
                {
                    Border border = new Border();
                    border.Name = "DropTarget_" + this.Name + "_Unit" + i.ToString() + "_" + j.ToString();
                    border.BorderThickness = new Thickness(2);
                    // ドラッグ中のユニットの位置を暗くする
                    if ( (i == troop_id) && (j == member_id) )
                    {
                        // 半透明のブラシを作成する (黒の33%)
                        SolidColorBrush mySolidColorBrush = new SolidColorBrush(Colors.Black);
                        mySolidColorBrush.Opacity = 0.33;
                        border.Background = mySolidColorBrush;
                        border.BorderBrush = Brushes.Red;
                    }
                    else
                    {
                        border.BorderBrush = Brushes.Aqua;
                    }
                    border.Width = tile_width;
                    border.Height = tile_height;
                    this.canvasSpotUnit.Children.Add(border);
                    Canvas.SetLeft(border, header_width + tile_width * j);
                    Canvas.SetTop(border, tile_height * i);
                }

                // 右の空きスペースなら「部隊メンバー追加」動作になる
                if (j < member_capacity)
                {
                    Border border = new Border();
                    border.Name = "DropTarget_" + this.Name + "_Right" + i.ToString();
                    border.BorderThickness = new Thickness(2);
                    border.BorderBrush = Brushes.Aqua;
                    border.Width = tile_width * (member_capacity - j);
                    border.Height = tile_height;
                    this.canvasSpotUnit.Children.Add(border);
                    Canvas.SetLeft(border, header_width + tile_width * j);
                    Canvas.SetTop(border, tile_height * i);
                }

                // 先頭ユニットの上なら「新規部隊を作成して間に追加」動作になる
                if (troop_count < spot_capacity)
                {
                    Border border = new Border();
                    border.Name = "DropTarget_" + this.Name + "_Top" + i.ToString();
                    border.BorderThickness = new Thickness(2);
                    border.BorderBrush = Brushes.Aqua;
                    border.Width = tile_width;
                    border.Height = tile_height / 4;
                    this.canvasSpotUnit.Children.Add(border);
                    Canvas.SetLeft(border, header_width);
                    Canvas.SetTop(border, tile_height * i);
                }

                i++;
            }

            // 下の空きスペースなら「新規部隊を作成して末尾に追加」動作になる
            if (i < spot_capacity)
            {
                Border border = new Border();
                border.Name = "DropTarget_" + this.Name + "_Bottom" + i.ToString();
                border.BorderThickness = new Thickness(2);
                border.BorderBrush = Brushes.Aqua;
                border.Width = tile_width * member_capacity;
                border.Height = tile_height * (spot_capacity - i);
                this.canvasSpotUnit.Children.Add(border);
                Canvas.SetLeft(border, header_width);
                Canvas.SetTop(border, tile_height * i);
            }

        }

        // ユニットをドラッグ移動する際に、移動先を作る
        private void RemoveDropTarget_Unit(MainWindow mainWindow)
        {
            // とりあえず、同じ領地上だけ考える
            // 将来的には、他の領地も対象にする

            // 全ての枠を消去する
            for (int i = this.canvasSpotUnit.Children.Count - 1; i >= 0; i += -1) {
                UIElement Child = this.canvasSpotUnit.Children[i];
                if (Child is Border)
                {
                    var border = (Border)Child;
                    string str = border.Name;
                    if (str.StartsWith("DropTarget_"))
                    {
                        this.canvasSpotUnit.Children.Remove(border);
                    }
                }
            }

            /*
            foreach (var border in this.canvasSpotUnit.Children.OfType<Border>())
            {
                string str = border.Name;
                if (str.StartsWith("DropTarget_"))
                {
                    this.canvasSpotUnit.Children.Remove(border);
                }
            }
            */
            
        }

        private static BitmapSource FrameworkElementToBitmapSource(FrameworkElement element)
        {
            element.UpdateLayout();
            var width = element.ActualWidth;
            var height = element.ActualHeight;
            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                dc.DrawRectangle(new BitmapCacheBrush(element), null, new Rect(0, 0, width, height));
            }
            var rtb = new RenderTargetBitmap((int)width, (int)height, 96d, 96d, PixelFormats.Pbgra32);
            rtb.Render(dv);
            return rtb;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 使用していたウインドウ番号の登録を抹消する
            string window_name = this.Name.Replace("WindowSpot", String.Empty);
            int window_id = Int32.Parse(window_name);
            mainWindow.ClassGameStatus.ListWindowSpot.Remove(window_id);

            // キャンバスから自身を取り除く
            // ガベージ・コレクタが掃除してくれるか不明
            mainWindow.canvasUI.Children.Remove(this);
        }

        #region ウインドウ移動
        private bool _isDrag = false; // 外部に公開する必要なし
        private Point _startPoint;

        private void win_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow != null)
            {
                // 最前面に移動させる
                try
                {
                    int maxZ = mainWindow.canvasUI.Children.OfType<UIElement>()
                       .Where(x => x != this)
                       .Select(x => Canvas.GetZIndex(x))
                       .Max();
                    Canvas.SetZIndex(this, maxZ + 1);
                }
                catch (InvalidOperationException)
                {
                    // 比較する子ウインドウがなければそのまま
                }
            }

            // ドラッグを開始する
            UIElement el = (UIElement)sender;
            if (el != null)
            {
                _isDrag = true;
                _startPoint = e.GetPosition(el);
                el.CaptureMouse();
                el.MouseLeftButtonUp += win_MouseLeftButtonUp;
                el.MouseMove += win_MouseMove;
            }
        }
        private void win_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // ドラック中なら終了する
            if (_isDrag == true)
            {
                UIElement el = (UIElement)sender;
                el.ReleaseMouseCapture();
                el.MouseLeftButtonUp -= win_MouseLeftButtonUp;
                el.MouseMove -= win_MouseMove;
                _isDrag = false;
            }
        }
        private void win_MouseMove(object sender, MouseEventArgs e)
        {
            // ドラック中
            if (_isDrag == true)
            {
                UIElement el = (UIElement)sender;
                Point pt = e.GetPosition(el);

                var thickness = new Thickness();
                thickness.Left = this.Margin.Left + (pt.X - _startPoint.X);
                thickness.Top = this.Margin.Top + (pt.Y - _startPoint.Y);
                this.Margin = thickness;
            }
        }
        #endregion

        // ボタン等を右クリックした際に、親コントロールが反応しないようにする
        private void Disable_MouseEvent(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }

        // ボタン等をクリックした際に、UserControlを最前面に移動させる
        private void Raise_ZOrder(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow != null)
            {
                // 最前面に移動させる
                try
                {
                    int maxZ = mainWindow.canvasUI.Children.OfType<UIElement>()
                       .Where(x => x != this)
                       .Select(x => Canvas.GetZIndex(x))
                       .Max();
                    Canvas.SetZIndex(this, maxZ + 1);
                }
                catch (InvalidOperationException)
                {
                    // 比較する子ウインドウがなければそのまま
                }
            }
        }

        #region ユニットのドラッグ移動
        private bool _isDragUnit = false;

        private void unit_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 最前面に移動させる
            try
            {
                int maxZ = mainWindow.canvasUI.Children.OfType<UIElement>()
                   .Where(x => x != this)
                   .Select(x => Canvas.GetZIndex(x))
                   .Max();
                Canvas.SetZIndex(this, maxZ + 1);
            }
            catch (InvalidOperationException)
            {
                // 比較する子ウインドウがなければそのまま
            }

            // ドラッグを開始する
            UIElement el = (UIElement)sender;
            if (el != null)
            {
                // ドロップ先を作る
                string unit_name = ((Button)sender).Name;
                string unit_id = unit_name.Replace("btnUnit", String.Empty);
                string[] array =  unit_id.Split('_');
                int troop_id = Int32.Parse(array[0]);
                int member_id = Int32.Parse(array[1]);
                MakeDropTarget_Unit(mainWindow, troop_id, member_id);


                // 画像をキャプチャして、ドラッグさせる Imageコントロールを作成する
                BitmapSource bitimg1 = FrameworkElementToBitmapSource((FrameworkElement)el);
                Image imgDrag = new Image();
                imgDrag.Name = "DragImage";
                imgDrag.Source = bitimg1;
                imgDrag.Width = bitimg1.PixelWidth;
                imgDrag.Height = bitimg1.PixelHeight;
                imgDrag.Stretch = Stretch.None;
                mainWindow.canvasUI.Children.Add(imgDrag);


/*
                // ボタンのユニット画像を使う場合はこちら
                BitmapImage bitimg1 = null;
                var ri = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasSpotUnit, "imgUnit" + unit_id);
                if (ri != null)
                {
                    bitimg1 = (BitmapImage)ri.Source;
                }
                Image imgDrag = new Image();
                imgDrag.Name = "DragImage";
                imgDrag.Source = bitimg1;
                imgDrag.Width = 48;
                imgDrag.Height = 48;
                mainWindow.canvasUI.Children.Add(imgDrag);
*/


                // ドラッグ画像をウインドウよりも前面に移動させる
                Canvas.SetZIndex(imgDrag, Canvas.GetZIndex(this) + 1);

                // ボタンの位置に置く
                Point pos = el.TranslatePoint(new Point(0, 0), mainWindow.canvasUI);
                Canvas.SetLeft(imgDrag, pos.X);
                Canvas.SetTop(imgDrag, pos.Y);

                _startPoint = e.GetPosition(el);
                _isDragUnit = true;

                // ドラッグ画像に対して、イベントを追加する
                imgDrag.CaptureMouse();
                imgDrag.MouseRightButtonUp += unit_MouseRightButtonUp;
                imgDrag.MouseMove += unit_MouseMove;
            }
        }
        private void unit_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

            // ドラック中なら終了する
            if (_isDragUnit == true)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    // ドロップ先を取り除く
                    RemoveDropTarget_Unit(mainWindow);

                    // ドラッグ画像を取り除く
                    var ri = (Image)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, "DragImage");
                    if (ri != null)
                    {
                        mainWindow.canvasUI.Children.Remove(ri);
                    }
                }

                UIElement el = (UIElement)sender;
                el.ReleaseMouseCapture();
                el.MouseRightButtonUp -= unit_MouseRightButtonUp;
                el.MouseMove -= unit_MouseMove;
                _isDragUnit = false;
            }
        }
        private void unit_MouseMove(object sender, MouseEventArgs e)
        {
            // ドラック中
            if (_isDragUnit == true)
            {
                UIElement el = (UIElement)sender;
                Point pt = e.GetPosition(el);

                // ドラッグ量に応じて子コントロールを移動する
                Canvas.SetLeft(el, Canvas.GetLeft(el) + (pt.X - _startPoint.X));
                Canvas.SetTop(el, Canvas.GetTop(el) + (pt.Y - _startPoint.Y));
            }
        }
        #endregion


    }
}