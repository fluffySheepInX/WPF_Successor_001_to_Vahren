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
            var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
            if ( (listWindow != null) && (listWindow.Any()) )
            {
                int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                Canvas.SetZIndex(this, maxZ + 1);
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

            // 将来的には、戦力値もここで更新すればよさそう。
            // ユニットを表示するついでに、戦力値を取得して合計しておけばいい。

            // 部隊数も更新する
            int spot_capacity = classPowerAndCity.ClassSpot.Capacity;
            this.lblMemberCount.Content = i.ToString() + "/" + spot_capacity.ToString();

            // ユニット配置場所の大きさ
            if (isControl)
            {
                // 操作可能な時だけドロップ先の枠の分も確保する
                int member_capacity = mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].MemberCapacity;
                if (j_max < member_capacity)
                {
                    j_max++;
                }
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

        // ユニットをドロップする処理
        private bool DropTarget_Unit(MainWindow mainWindow, int troop_id, int member_id, string strTarget)
        {
            string[] strPart =  strTarget.Split('_');
            //MessageBox.Show("Drop先: 領地 = " + strPart[0] + " , 対象 = " + strPart[1]);
            UserControl006_Spot windowSpot = null;

            if (strPart[0] == this.Name)
            {
                // 同じ領地ウインドウの上
                windowSpot = this;
            }
            else
            {
                // 指定番号の領地ウインドウを探す
                windowSpot = (UserControl006_Spot)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, strPart[0]);
                if (windowSpot == null)
                {
                    return false;
                }
            }
            var dstPowerAndCity = (ClassPowerAndCity)windowSpot.Tag;
            var srcPowerAndCity = (ClassPowerAndCity)this.Tag;
            //MessageBox.Show("Drop先: 領地 = " + dstPowerAndCity.ClassSpot.Name + " , 対象 = " + strPart[1]);

            // 部隊メンバー入れ替え
            if ( (strPart[1] == "Unit") && (strPart.Length >= 4) )
            {
                // 入れ替え元の部隊とユニット
                ClassHorizontalUnit srcTroop = srcPowerAndCity.ClassSpot.UnitGroup[troop_id];
                ClassUnit srcUnit = srcTroop.ListClassUnit[member_id];

                // 入れ替え先の部隊とユニット
                int dst_troop_id = Int32.Parse(strPart[2]);
                int dst_member_id = Int32.Parse(strPart[3]);
                ClassHorizontalUnit dstTroop = dstPowerAndCity.ClassSpot.UnitGroup[dst_troop_id];
                ClassUnit dstUnit = dstTroop.ListClassUnit[dst_member_id];

                // 移動先を取り除いてから、移動元を挿入すれば、位置がずれない
                dstTroop.ListClassUnit.RemoveAt(dst_member_id);
                dstTroop.ListClassUnit.Insert(dst_member_id, srcUnit);
                srcTroop.ListClassUnit.RemoveAt(member_id);
                srcTroop.ListClassUnit.Insert(member_id, dstUnit);

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if (windowSpot != this)
                {
                    // ウインドウが異なる場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            // 部隊メンバー追加
            if ( (strPart[1] == "Right") && (strPart.Length >= 3) )
            {
                // 移動元の部隊とユニット
                ClassHorizontalUnit srcTroop = srcPowerAndCity.ClassSpot.UnitGroup[troop_id];
                ClassUnit srcUnit = srcTroop.ListClassUnit[member_id];

                // 移動先の部隊にユニットを追加する
                int dst_troop_id = Int32.Parse(strPart[2]);
                ClassHorizontalUnit dstTroop = dstPowerAndCity.ClassSpot.UnitGroup[dst_troop_id];
                dstTroop.ListClassUnit.Add(srcUnit);

                // 元の部隊からユニットを取り除く
                srcTroop.ListClassUnit.RemoveAt(member_id);

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if (windowSpot != this)
                {
                    // ウインドウが異なる場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            // 新規部隊を作成して間に追加
            if ( (strPart[1] == "Top") && (strPart.Length >= 3) )
            {
                // 移動元の部隊とユニット
                ClassHorizontalUnit srcTroop = srcPowerAndCity.ClassSpot.UnitGroup[troop_id];
                ClassUnit srcUnit = srcTroop.ListClassUnit[member_id];

                // 新規部隊を指定位置に挿入してユニットを追加する
                var listUnit = new List<ClassUnit>();
                listUnit.Add(srcUnit);
                int dst_troop_id = Int32.Parse(strPart[2]);
                dstPowerAndCity.ClassSpot.UnitGroup.Insert(dst_troop_id, new ClassHorizontalUnit()
                    {
                        Spot = dstPowerAndCity.ClassSpot,
                        FlagDisplay = true,
                        ListClassUnit = listUnit
                    });

                // 元の部隊からユニットを取り除く
                srcTroop.ListClassUnit.RemoveAt(member_id);

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if (windowSpot != this)
                {
                    // ウインドウが異なる場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            // 新規部隊を作成して末尾に追加
            if (strPart[1] == "Bottom")
            {
                // 移動元の部隊とユニット
                ClassHorizontalUnit srcTroop = srcPowerAndCity.ClassSpot.UnitGroup[troop_id];
                ClassUnit srcUnit = srcTroop.ListClassUnit[member_id];

                // 新規部隊を末尾に追加してユニットを追加する
                var listUnit = new List<ClassUnit>();
                listUnit.Add(srcUnit);
                dstPowerAndCity.ClassSpot.UnitGroup.Add(new ClassHorizontalUnit()
                    {
                        Spot = dstPowerAndCity.ClassSpot,
                        FlagDisplay = true,
                        ListClassUnit = listUnit
                    });

                // 元の部隊からユニットを取り除く
                srcTroop.ListClassUnit.RemoveAt(member_id);

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if (windowSpot != this)
                {
                    // ウインドウが異なる場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            return false;
        }

        // ユニットをドラッグ移動する際に、移動先を作る
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
                if (i == troop_id)
                {
                    // 選択ユニットと同じ部隊だけ、リーダーとの入れ替えが可能
                    j = 0;
                }
                else
                {
                    j = 1;
                }
                for (; j < member_count; j++)
                {
                    Border border = new Border();
                    border.Name = "DropTarget" + this.Name + "_Unit_" + i.ToString() + "_" + j.ToString();
                    // ドラッグ中のユニットの位置を暗くする
                    if ( (i == troop_id) && (j == member_id) )
                    {
                        // 半透明のブラシを作成する (黒の50%)
                        SolidColorBrush mySolidColorBrush = new SolidColorBrush(Colors.Black);
                        mySolidColorBrush.Opacity = 0.5;
                        border.Background = mySolidColorBrush;
                        border.BorderThickness = new Thickness(0);
                        border.Width = tile_width;
                        border.Height = tile_height;
                    }
                    else
                    {
                        border.Background = Brushes.Transparent;
                        border.BorderThickness = new Thickness(2);
                        border.BorderBrush = Brushes.Aqua;
                        border.Width = tile_width - 1;
                        border.Height = tile_height - 1;
                    }
                    this.canvasSpotUnit.Children.Add(border);
                    Canvas.SetLeft(border, header_width + tile_width * j);
                    Canvas.SetTop(border, tile_height * i);
                }

                // 右の空きスペースなら「部隊メンバー追加」動作になる
                if ( (j < member_capacity) && ( (i != troop_id) || (member_id < member_count - 1) ) )
                {
                    Border border = new Border();
                    border.Name = "DropTarget" + this.Name + "_Right_" + i.ToString();
                    border.Background = Brushes.Transparent;
                    border.BorderThickness = new Thickness(2);
                    border.BorderBrush = Brushes.Aqua;
                    border.Width = tile_width * (member_capacity - j) -1;
                    border.Height = tile_height - 1;
                    this.canvasSpotUnit.Children.Add(border);
                    Canvas.SetLeft(border, header_width + tile_width * j);
                    Canvas.SetTop(border, tile_height * i);
                }

                // 先頭ユニットの上端なら「新規部隊を作成して間に追加」動作になる
                if ( (troop_count < spot_capacity) && (i != troop_id) )
                {
                    Border border = new Border();
                    border.Name = "DropTarget" + this.Name + "_Top_" + i.ToString();
                    border.Background = Brushes.Transparent;
                    border.BorderThickness = new Thickness(2);
                    border.BorderBrush = Brushes.Aqua;
                    border.Width = header_width + tile_width - 1;
                    border.Height = tile_height / 4;
                    this.canvasSpotUnit.Children.Add(border);
                    Canvas.SetTop(border, tile_height * i);
                }

                i++;
            }

            // 下の空きスペースなら「新規部隊を作成して末尾に追加」動作になる
            if (i < spot_capacity)
            {
                Border border = new Border();
                border.Name = "DropTarget" + this.Name + "_Bottom";
                border.Background = Brushes.Transparent;
                border.BorderThickness = new Thickness(2);
                border.BorderBrush = Brushes.Aqua;
                border.Width = header_width + tile_width * member_capacity - 1;
                border.Height = tile_height * (spot_capacity - i);
                this.canvasSpotUnit.Children.Add(border);
                Canvas.SetTop(border, tile_height * i);
            }

        }

        // ユニットをドラッグ移動した後に、移動先を取り除く
        private void RemoveDropTarget_Unit(MainWindow mainWindow, int troop_id, int member_id)
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
                    if (str.StartsWith("DropTarget"))
                    {
                        // 枠が太くなっていれば、選択中の印
                        if (border.BorderThickness.Left > 2)
                        {
                            if (DropTarget_Unit(mainWindow, troop_id, member_id, str.Replace("DropTarget", String.Empty)))
                            {
                                // 領地を更新した場合は枠も消えるので、ループから抜ける
                                break;
                            }
                        }
                        // 枠を取り除く
                        this.canvasSpotUnit.Children.Remove(border);
                    }
                }
            }

        }

        private DependencyObject _hitResults = null;
        private int _hitCount = 0;
        private HitTestResultBehavior OnHitTestResultCallback(HitTestResult result)
        {
            _hitCount++;

            // Borderを見つけたら終了する
            if (result.VisualHit.GetType() == typeof(Border))
            {
                _hitResults = result.VisualHit;
                return HitTestResultBehavior.Stop;
            }

            // 上位３個まで調べる
            // Fade用Canvas、ドラッグ画像のImage、ドロップ先のBorder
            if (_hitCount > 3)
            {
                return HitTestResultBehavior.Stop;
            }

            return HitTestResultBehavior.Continue;
        }

        // ユニットをドラッグ移動中に、移動先をホバー表示する
        private void HoverDropTarget_Unit(MainWindow mainWindow, Point posMouse)
        {
            // とりあえず、同じ領地上だけ考える
            // 将来的には、他の領地も対象にする

            // マウスポインタ―の座標をデバッグ表示
            //this.lblGain.Content = posMouse.X;
            //this.lblCastle.Content = posMouse.Y;

            // マウスポインタ―の下にあるなら、最初の枠だけ太くする
            Border borderHit = null;
            _hitResults = null;
            _hitCount = 0;
            VisualTreeHelper.HitTest(mainWindow
                    , null
                    , new HitTestResultCallback(OnHitTestResultCallback)
                    , new PointHitTestParameters(posMouse));

            if (_hitResults != null){
                borderHit = (Border)_hitResults;
                if (borderHit.Name.StartsWith("DropTarget"))
                {
                    if (borderHit.BorderThickness.Left == 2)
                    {
                        borderHit.BorderThickness = new Thickness(5);
                    }
                }
            }

            // それ以外の枠が太ければ普通に戻す
            foreach (var border in this.canvasSpotUnit.Children.OfType<Border>())
            {
                if (border.Name.StartsWith("DropTarget"))
                {
                    if ( (border != borderHit) && (border.BorderThickness.Left > 2) )
                    {
                        border.BorderThickness = new Thickness(2);
                    }
                }
            }
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
                var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
                if ( (listWindow != null) && (listWindow.Any()) )
                {
                    int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                    Canvas.SetZIndex(this, maxZ + 1);
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
                var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
                if ( (listWindow != null) && (listWindow.Any()) )
                {
                    int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                    Canvas.SetZIndex(this, maxZ + 1);
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
            var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
            if ( (listWindow != null) && (listWindow.Any()) )
            {
                int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                Canvas.SetZIndex(this, maxZ + 1);
            }

            // ドラッグを開始する
            UIElement el = (UIElement)sender;
            if (el != null)
            {
                // ドロップ先を作る
                string unit_name = ((Button)sender).Name;
                string unit_id = unit_name.Replace("btnUnit", String.Empty);
                string[] strPart =  unit_id.Split('_');
                int troop_id = Int32.Parse(strPart[0]);
                int member_id = Int32.Parse(strPart[1]);
                MakeDropTarget_Unit(mainWindow, troop_id, member_id);

                // 画像をキャプチャして、ドラッグさせる Imageコントロールを作成する
                BitmapSource bitimg1 = FrameworkElementToBitmapSource((FrameworkElement)el);
                Image imgDrag = new Image();
                imgDrag.Source = bitimg1;
                imgDrag.Width = bitimg1.PixelWidth;
                imgDrag.Height = bitimg1.PixelHeight;
                imgDrag.Stretch = Stretch.None;

/*
                // ボタンのユニット画像を使う場合はこちら
                BitmapImage bitimg1 = null;
                var ri = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasSpotUnit, "imgUnit" + unit_id);
                if (ri != null)
                {
                    bitimg1 = (BitmapImage)ri.Source;
                }
                Image imgDrag = new Image();
                imgDrag.Source = bitimg1;
                imgDrag.Width = 48;
                imgDrag.Height = 48;
*/

                imgDrag.Name = "DragImage" + troop_id.ToString() + "_" + member_id.ToString();
                mainWindow.canvasUI.Children.Add(imgDrag);

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
                UIElement el = (UIElement)sender;
                el.ReleaseMouseCapture();
                el.MouseRightButtonUp -= unit_MouseRightButtonUp;
                el.MouseMove -= unit_MouseMove;
                _isDragUnit = false;

                var mainWindow = (MainWindow)Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    // ドロップ先を取り除くと同時に、ドロップ判定を行う
                    string unit_name = ((Image)sender).Name;
                    string unit_id = unit_name.Replace("DragImage", String.Empty);
                    string[] strPart =  unit_id.Split('_');
                    int troop_id = Int32.Parse(strPart[0]);
                    int member_id = Int32.Parse(strPart[1]);
                    RemoveDropTarget_Unit(mainWindow, troop_id, member_id);

                    // ドラッグ画像を取り除く
                    mainWindow.canvasUI.Children.Remove(el);
                 }
            }
        }
        private void unit_MouseMove(object sender, MouseEventArgs e)
        {
            // ドラック中
            if (_isDragUnit == true)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    // ドロップ先をホバー表示する
                    HoverDropTarget_Unit(mainWindow, e.GetPosition(mainWindow));
                }

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