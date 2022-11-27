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
    /// UserControl010_Spot.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl010_Spot : UserControl
    {
        public UserControl010_Spot()
        {
            InitializeComponent();
        }

        // 定数
        // ユニットのタイルサイズをここで調節できます
        private const int tile_width = 48, tile_height = 66, header_width = 48;

        // 最初に呼び出した時
        private bool _isControl = false; // 操作可能かどうかの設定
        public void SetData()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
            	return;
            }

            // プレイヤーが操作可能かどうか
            if (((ClassPowerAndCity)this.Tag).ClassPower.NameTag == mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.NameTag)
            {
                // 同じ勢力なら、操作できる
                _isControl = true;
            }
            else
            {
                _isControl = false;
            }

            // 領地の情報を表示する
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

            // 最初に全て消去する
            this.canvasSpotUnit.Children.Clear();

            // 全ての部隊を表示する（駐留数の制限を超えていても許容する）
            var listTroop = classPowerAndCity.ClassSpot.UnitGroup;
            int i = 0, j, j_max = 1;
            foreach (var itemTroop in listTroop)
            {
                j = 0;
                foreach (var itemUnit in itemTroop.ListClassUnit)
                {
                    // 部隊の陣形はリーダー（先頭のユニット）を参照する
                    if (j == 0)
                    {
                        if (_isControl)
                        {
                            // 出撃ボタン
                            Button btnSelect = new Button();
                            btnSelect.Name = "btnSelect" + i.ToString();
                            btnSelect.Height = tile_height / 2 - 4;
                            btnSelect.Width = header_width - 4;
                            btnSelect.Margin = new Thickness(2);
                            btnSelect.FontSize = 15;
                            btnSelect.Content = "出撃";
                            this.canvasSpotUnit.Children.Add(btnSelect);
                            Canvas.SetTop(btnSelect, tile_height * i);

                            // 陣形コンボボックス
                            ComboBox cmbFormation = new ComboBox();
                            cmbFormation.Name = "cmbFormation" + i.ToString();
                            cmbFormation.Height = tile_height / 2 - 4;
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

                    // ユニットのパネル
                    StackPanel panelUnit = new StackPanel();
                    panelUnit.Name = "panelUnit" + i.ToString() + "_" + j.ToString();
                    panelUnit.Tag = itemUnit;
                    panelUnit.Height = tile_height;
                    panelUnit.Width = tile_width;
                    // 色を変えるためのイベントを追加する
                    panelUnit.MouseEnter += panel_MouseEnter;
                    panelUnit.MouseLeave += panel_MouseLeave;
                    // ユニット情報を表示するためのイベント
                    panelUnit.MouseLeftButtonDown += unit_MouseLeftButtonDown;
                    if (_isControl)
                    {
                        // 操作可能な時だけドラッグ移動の準備をしておく
                        if (j > 0)
                        {
                            // 部下なら、そのユニットだけ
                            panelUnit.MouseRightButtonDown += unit_MouseRightButtonDown;
                        }
                        else
                        {
                            // 隊長なら、部隊のユニット全て
                            panelUnit.MouseRightButtonDown += troop_MouseRightButtonDown;
                        }
                    }

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
                    panelUnit.Children.Add(imgUnit);

                    // ユニットのレベル
                    Label lblLevel = new Label();
                    lblLevel.Name = "lblLevel" + i.ToString() + "_" + j.ToString();
                    lblLevel.Height = tile_height - tile_width;
                    lblLevel.FontSize = 15;
                    lblLevel.Padding = new Thickness(-5);
                    lblLevel.Foreground = Brushes.White;
                    lblLevel.HorizontalAlignment = HorizontalAlignment.Center;
                    lblLevel.Content = "lv" + itemUnit.Level;
                    panelUnit.Children.Add(lblLevel);

                    this.canvasSpotUnit.Children.Add(panelUnit);
                    Canvas.SetLeft(panelUnit, header_width + tile_width * j);
                    Canvas.SetTop(panelUnit, tile_height * i);

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
            if (_isControl)
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

            // プレイヤーが操作可能かどうか
            if (_isControl == false)
            {
                // 異なる勢力なら、操作ボタンを無効にする
                btnSelectAll.IsEnabled = false;
                btnMercenary.IsEnabled = false;
                btnPolitics.IsEnabled = false;
            }

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
            //戦力値
            {
                this.lblForce.Content = this.Name.Replace("dowSpot", String.Empty); // ウインドウ番号を表示する実験用
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
            ClassSpot srcSpot = ((ClassPowerAndCity)this.Tag).ClassSpot;
            ClassSpot dstSpot = null;
            UserControl010_Spot windowSpot = null;

            if (strPart[0] == this.Name)
            {
                // 同じ領地ウインドウの上
                windowSpot = this;
                dstSpot = srcSpot;
            }
            else if (strPart[0] == "Spot")
            {
                // 領地リストのインデックスから ClassSpot を取得する
                int spot_id = Int32.Parse(strPart[1]);
                dstSpot = mainWindow.ClassGameStatus.AllListSpot[spot_id];

                // 領地ウインドウが開いてるかどうか調べる
                foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
                {
                    string strTitle = itemWindow.Name;
                    if (strTitle.StartsWith("WindowSpot"))
                    {
                        if (dstSpot.NameTag == ((ClassPowerAndCity)itemWindow.Tag).ClassSpot.NameTag)
                        {
                            windowSpot = itemWindow;
                            break;
                        }
                    }
                }
            }
            else
            {
                // 指定番号の領地ウインドウを探す
                windowSpot = (UserControl010_Spot)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, strPart[0]);
                if (windowSpot == null)
                {
                    return false;
                }
                dstSpot = ((ClassPowerAndCity)windowSpot.Tag).ClassSpot;
            }

            // 部隊メンバー入れ替え
            if ( (strPart[1] == "Unit") && (strPart.Length >= 4) )
            {
                // 入れ替え元の部隊とユニット
                ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[troop_id];
                ClassUnit srcUnit = srcTroop.ListClassUnit[member_id];

                // 入れ替え先の部隊とユニット
                int dst_troop_id = Int32.Parse(strPart[2]);
                int dst_member_id = Int32.Parse(strPart[3]);
                ClassHorizontalUnit dstTroop = dstSpot.UnitGroup[dst_troop_id];
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
                ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[troop_id];
                ClassUnit srcUnit = srcTroop.ListClassUnit[member_id];

                // 移動先の部隊にユニットを追加する
                int dst_troop_id = Int32.Parse(strPart[2]);
                ClassHorizontalUnit dstTroop = dstSpot.UnitGroup[dst_troop_id];
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
                ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[troop_id];
                ClassUnit srcUnit = srcTroop.ListClassUnit[member_id];

                // 新規部隊を指定位置に挿入してユニットを追加する
                var listUnit = new List<ClassUnit>();
                listUnit.Add(srcUnit);
                int dst_troop_id = Int32.Parse(strPart[2]);
                dstSpot.UnitGroup.Insert(dst_troop_id, new ClassHorizontalUnit()
                    {
                        Spot = dstSpot,
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
                ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[troop_id];
                ClassUnit srcUnit = srcTroop.ListClassUnit[member_id];

                // 新規部隊を末尾に追加してユニットを追加する
                var listUnit = new List<ClassUnit>();
                listUnit.Add(srcUnit);
                dstSpot.UnitGroup.Add(new ClassHorizontalUnit()
                    {
                        Spot = dstSpot,
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

            // 別領地に新規部隊を作成して末尾に追加
            if ( (strPart[0] == "Spot") && (strPart.Length >= 2) )
            {
                // 移動元の部隊とユニット
                ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[troop_id];
                ClassUnit srcUnit = srcTroop.ListClassUnit[member_id];

                // 新規部隊を末尾に追加してユニットを追加する
                var listUnit = new List<ClassUnit>();
                listUnit.Add(srcUnit);
                dstSpot.UnitGroup.Add(new ClassHorizontalUnit()
                    {
                        Spot = dstSpot,
                        FlagDisplay = true,
                        ListClassUnit = listUnit
                    });

                // 元の部隊からユニットを取り除く
                srcTroop.ListClassUnit.RemoveAt(member_id);

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if (windowSpot != null)
                {
                    // ウインドウが存在する場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            return false;
        }

        // 部隊をドロップする処理
        private bool DropTarget_Troop(MainWindow mainWindow, int troop_id, string strTarget)
        {
            string[] strPart =  strTarget.Split('_');
            ClassSpot srcSpot = ((ClassPowerAndCity)this.Tag).ClassSpot;
            ClassSpot dstSpot = null;
            UserControl010_Spot windowSpot = null;

            if (strPart[0] == this.Name)
            {
                // 同じ領地ウインドウの上
                windowSpot = this;
                dstSpot = srcSpot;
            }
            else if (strPart[0] == "Spot")
            {
                // 領地リストのインデックスから ClassSpot を取得する
                int spot_id = Int32.Parse(strPart[1]);
                dstSpot = mainWindow.ClassGameStatus.AllListSpot[spot_id];

                // 領地ウインドウが開いてるかどうか調べる
                foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
                {
                    string strTitle = itemWindow.Name;
                    if (strTitle.StartsWith("WindowSpot"))
                    {
                        if (dstSpot.NameTag == ((ClassPowerAndCity)itemWindow.Tag).ClassSpot.NameTag)
                        {
                            windowSpot = itemWindow;
                            break;
                        }
                    }
                }
            }
            else
            {
                // 指定番号の領地ウインドウを探す
                windowSpot = (UserControl010_Spot)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, strPart[0]);
                if (windowSpot == null)
                {
                    return false;
                }
                dstSpot = ((ClassPowerAndCity)windowSpot.Tag).ClassSpot;
            }

            // 部隊メンバー追加
            if ( (strPart[1] == "Right") && (strPart.Length >= 3) )
            {
                // 移動元の部隊
                ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[troop_id];

                // 移動先の部隊に全てのユニットを追加する
                int dst_troop_id = Int32.Parse(strPart[2]);
                ClassHorizontalUnit dstTroop = dstSpot.UnitGroup[dst_troop_id];
                foreach (ClassUnit srcUnit in srcTroop.ListClassUnit)
                {
                    dstTroop.ListClassUnit.Add(srcUnit);
                }

                // 元の部隊から全てのユニットを取り除く
                srcTroop.ListClassUnit.Clear();

                // 移動元領地から部隊を取り除く
                srcSpot.UnitGroup.RemoveAt(troop_id);

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
                // 移動元の部隊
                ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[troop_id];

                // 移動先の指定位置
                int dst_troop_id = Int32.Parse(strPart[2]);

                // 後に挿入する場合、先に取り除くと順番が変わるので、先に挿入する
                if (dst_troop_id > troop_id)
                {
                    // 移動先領地の指定位置に部隊を挿入する
                    dstSpot.UnitGroup.Insert(dst_troop_id, srcTroop);

                    // 移動元領地から部隊を取り除く
                    srcSpot.UnitGroup.RemoveAt(troop_id);
                }
                // 前に挿入する場合、先に挿入すると順番が変わるので、先に取り除く
                else
                {
                    // 移動元領地から部隊を取り除く
                    srcSpot.UnitGroup.RemoveAt(troop_id);

                    // 移動先領地の指定位置に部隊を挿入する
                    dstSpot.UnitGroup.Insert(dst_troop_id, srcTroop);
                }

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if (windowSpot != this)
                {
                    // ウインドウが異なる場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            // 部隊を末尾に移動
            if (strPart[1] == "Bottom")
            {
                // 移動元の部隊
                ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[troop_id];

                // 移動先領地の末尾に部隊を追加する
                dstSpot.UnitGroup.Add(srcTroop);

                // 移動元領地から部隊を取り除く
                srcSpot.UnitGroup.RemoveAt(troop_id);

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if (windowSpot != this)
                {
                    // ウインドウが異なる場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            // 部隊を別領地の末尾に移動
            if ( (strPart[0] == "Spot") && (strPart.Length >= 2) )
            {
                // 移動元の部隊
                ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[troop_id];

                // 移動先領地の末尾に部隊を追加する
                dstSpot.UnitGroup.Add(srcTroop);

                // 移動元領地から部隊を取り除く
                srcSpot.UnitGroup.RemoveAt(troop_id);

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if (windowSpot != null)
                {
                    // ウインドウが存在する場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            return false;
        }

        // 複数の部隊をドロップする処理
        private bool DropTarget_Whole(MainWindow mainWindow, string strTarget)
        {
            string[] strPart =  strTarget.Split('_');
            ClassSpot srcSpot = ((ClassPowerAndCity)this.Tag).ClassSpot;
            ClassSpot dstSpot = null;
            UserControl010_Spot windowSpot = null;

            if (strPart[0] == this.Name)
            {
                // 同じ領地ウインドウの上にはドロップできないはず
                return false;
            }
            else if (strPart[0] == "Spot")
            {
                // 領地リストのインデックスから ClassSpot を取得する
                int spot_id = Int32.Parse(strPart[1]);
                dstSpot = mainWindow.ClassGameStatus.AllListSpot[spot_id];

                // 領地ウインドウが開いてるかどうか調べる
                foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
                {
                    string strTitle = itemWindow.Name;
                    if (strTitle.StartsWith("WindowSpot"))
                    {
                        if (dstSpot.NameTag == ((ClassPowerAndCity)itemWindow.Tag).ClassSpot.NameTag)
                        {
                            windowSpot = itemWindow;
                            break;
                        }
                    }
                }
            }
            else
            {
                // 指定番号の領地ウインドウを探す
                windowSpot = (UserControl010_Spot)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, strPart[0]);
                if (windowSpot == null)
                {
                    return false;
                }
                dstSpot = ((ClassPowerAndCity)windowSpot.Tag).ClassSpot;
            }

            // 移動先の空きが部隊数よりも少ない場合は、入るだけ移動させる
            int src_troop_count = srcSpot.UnitGroup.Count;
            int dst_troop_count = dstSpot.UnitGroup.Count;
            int spot_capacity = dstSpot.Capacity;
            int move_count = src_troop_count;
            if (move_count > spot_capacity - dst_troop_count)
            {
                move_count = spot_capacity - dst_troop_count;
            }

            // 新規部隊を作成して間に追加
            if ( (strPart[1] == "Top") && (strPart.Length >= 3) )
            {
                // 移動先の指定位置
                int dst_troop_id = Int32.Parse(strPart[2]);

                for (int i = 0; i < move_count; i++)
                {
                    // 先頭の部隊から移動元にする
                    ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[0];

                    // 移動先領地の指定位置に部隊を挿入する
                    dstSpot.UnitGroup.Insert(dst_troop_id + i, srcTroop);

                    // 移動元領地から先頭の部隊を取り除く
                    srcSpot.UnitGroup.RemoveAt(0);
                }

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if (windowSpot != null)
                {
                    // ウインドウが存在する場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            // 部隊を別領地の末尾に移動（領地ウインドウと領地アイコンで同じ処理）
            if ( (strPart[1] == "Bottom") ||
                 ( (strPart[0] == "Spot") && (strPart.Length >= 2) ) )
            {
                for (int i = 0; i < move_count; i++)
                {
                    // 先頭の部隊から移動元にする
                    ClassHorizontalUnit srcTroop = srcSpot.UnitGroup[0];

                    // 移動先領地の末尾に部隊を追加する
                    dstSpot.UnitGroup.Add(srcTroop);

                    // 移動元領地から部隊を取り除く
                    srcSpot.UnitGroup.RemoveAt(0);
                }

                // 表示を更新する
                this.UpdateSpotUnit(mainWindow);
                if (windowSpot != null)
                {
                    // ウインドウが存在する場合は、移動先も更新する
                    windowSpot.UpdateSpotUnit(mainWindow);
                }
                return true;
            }

            return false;
        }

        // ユニットをドラッグ移動する際に、移動先を作る
        private void MakeDropTarget_Unit(MainWindow mainWindow, int troop_id, int member_id)
        {
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;
            int member_capacity = mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].MemberCapacity;
            int spot_capacity = classPowerAndCity.ClassSpot.Capacity;

            var listTroop = classPowerAndCity.ClassSpot.UnitGroup;
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
                    // ドラッグ中のユニットの位置を暗くする
                    if ( (i == troop_id) && (j == member_id) )
                    {
                        Border border = new Border();
                        border.Name = "DropTarget";
                        // 半透明のブラシを作成する (黒の50%)
                        SolidColorBrush mySolidColorBrush = new SolidColorBrush(Colors.Black);
                        mySolidColorBrush.Opacity = 0.5;
                        border.Background = mySolidColorBrush;
                        border.BorderThickness = new Thickness(0);
                        border.Width = tile_width;
                        border.Height = tile_height;
                        this.canvasSpotUnit.Children.Add(border);
                        Canvas.SetLeft(border, header_width + tile_width * j);
                        Canvas.SetTop(border, tile_height * i);
                    }
                    else
                    {
                        Border border = new Border();
                        border.Name = "DropTarget" + this.Name + "_Unit_" + i.ToString() + "_" + j.ToString();
                        border.Background = Brushes.Transparent;
                        border.BorderThickness = new Thickness(2);
                        border.BorderBrush = Brushes.Aqua;
                        border.Width = tile_width - 1;
                        border.Height = tile_height - 1;
                        this.canvasSpotUnit.Children.Add(border);
                        Canvas.SetLeft(border, header_width + tile_width * j + 1);
                        Canvas.SetTop(border, tile_height * i);
                    }
                }

                // 右の空きスペースなら「部隊メンバー追加」動作になる
                if ( (j < member_capacity) && ( (i != troop_id) || (member_id < member_count - 1) ) )
                {
                    Border border = new Border();
                    border.Name = "DropTarget" + this.Name + "_Right_" + i.ToString();
                    border.Background = Brushes.Transparent;
                    border.BorderThickness = new Thickness(2);
                    border.BorderBrush = Brushes.Aqua;
                    border.Width = tile_width * (member_capacity - j) - 1;
                    border.Height = tile_height - 1;
                    this.canvasSpotUnit.Children.Add(border);
                    Canvas.SetLeft(border, header_width + tile_width * j + 1);
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
                    border.Width = header_width + tile_width;
                    border.Height = tile_height / 3;
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
                border.Width = header_width + tile_width * member_capacity;
                border.Height = tile_height * (spot_capacity - i);
                this.canvasSpotUnit.Children.Add(border);
                Canvas.SetTop(border, tile_height * i);
            }

            // 他の領地ウインドウを探す
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
            {
                string strTitle = itemWindow.Name;
                if ( (strTitle.StartsWith("WindowSpot")) && (strTitle != this.Name) )
                {
                    var targetPowerAndCity = (ClassPowerAndCity)itemWindow.Tag;
                    // 同じ勢力の領地ウインドウだけ対象にする
                    if (targetPowerAndCity.ClassPower.NameTag == classPowerAndCity.ClassPower.NameTag)
                    {
                        // 領地の部隊情報を調べる
                        listTroop = targetPowerAndCity.ClassSpot.UnitGroup;
                        spot_capacity = targetPowerAndCity.ClassSpot.Capacity;
                        i = 0;
                        troop_count = listTroop.Count;
                        foreach (var itemTroop in listTroop)
                        {
                            member_count = itemTroop.ListClassUnit.Count;

                            // 右の空きスペースなら「部隊メンバー追加」動作になる
                            if (member_count < member_capacity)
                            {
                                Border border = new Border();
                                border.Name = "DropTarget" + strTitle + "_Right_" + i.ToString();
                                border.Background = Brushes.Transparent;
                                border.BorderThickness = new Thickness(2);
                                border.BorderBrush = Brushes.Aqua;
                                border.Width = tile_width * (member_capacity - member_count) - 1;
                                border.Height = tile_height - 1;
                                itemWindow.canvasSpotUnit.Children.Add(border);
                                Canvas.SetLeft(border, header_width + tile_width * member_count + 1);
                                Canvas.SetTop(border, tile_height * i);
                            }

                            // 先頭ユニットの上端なら「新規部隊を作成して間に追加」動作になる
                            if (troop_count < spot_capacity)
                            {
                                Border border = new Border();
                                border.Name = "DropTarget" + strTitle + "_Top_" + i.ToString();
                                border.Background = Brushes.Transparent;
                                border.BorderThickness = new Thickness(2);
                                border.BorderBrush = Brushes.Aqua;
                                border.Width = header_width + tile_width;
                                border.Height = tile_height / 3;
                                itemWindow.canvasSpotUnit.Children.Add(border);
                                Canvas.SetTop(border, tile_height * i);
                            }

                            i++;
                        }

                        // 下の空きスペースなら「新規部隊を作成して末尾に追加」動作になる
                        if (i < spot_capacity)
                        {
                            Border border = new Border();
                            border.Name = "DropTarget" + strTitle + "_Bottom";
                            border.Background = Brushes.Transparent;
                            border.BorderThickness = new Thickness(2);
                            border.BorderBrush = Brushes.Aqua;
                            border.Width = header_width + tile_width * member_capacity;
                            border.Height = tile_height * (spot_capacity - i);
                            itemWindow.canvasSpotUnit.Children.Add(border);
                            Canvas.SetTop(border, tile_height * i);
                        }
                    }
                }
            }

            // 戦略マップ上の領地アイコンも
            if (mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.ListMember.Count > 1)
            {
                var gridMapStrategy = (Grid)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasMain, StringName.gridMapStrategy);
                if (gridMapStrategy != null)
                {
                    var listSpot = mainWindow.ClassGameStatus.AllListSpot;
                    int spot_count = listSpot.Count;
                    for (int spot_id = 0; spot_id < spot_count; spot_id++)
                    {
                        // 同じ勢力の領地だけ
                        ClassSpot itemSpot = listSpot[spot_id];
                        if (itemSpot.PowerNameTag == classPowerAndCity.ClassPower.NameTag)
                        {
                            // 領地の部隊数に空きがあるなら
                            spot_capacity = itemSpot.Capacity;
                            troop_count = itemSpot.UnitGroup.Count;
                            if ( (troop_count < spot_capacity) && (itemSpot.NameTag != classPowerAndCity.ClassSpot.NameTag) )
                            {
                                Border border = new Border();
                                // 領地リストのインデックスで識別する
                                border.Name = "DropTargetSpot_" + spot_id.ToString();
                                border.Background = Brushes.Transparent;
                                border.BorderThickness = new Thickness(2);
                                border.BorderBrush = Brushes.Aqua;
                                // 親コントロールが Grid なので、標準だと中央配置になる。左上を原点に変えておく。
                                border.HorizontalAlignment = HorizontalAlignment.Left;
                                border.VerticalAlignment = VerticalAlignment.Top;
                                // 領地アイコンの大きさが不明なので、大きめの枠にする
                                border.Width = 64;
                                border.Height = 64;
                                border.Margin = new Thickness()
                                {
                                    Left = itemSpot.X - 32,
                                    Top = itemSpot.Y - 32
                                };
                                gridMapStrategy.Children.Add(border);
                            }
                        }
                    }
                }
            }
        }

        // 部隊をドラッグ移動する際に、移動先を作る
        private void MakeDropTarget_Troop(MainWindow mainWindow, int troop_id)
        {
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;
            int member_capacity = mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].MemberCapacity;
            int spot_capacity = classPowerAndCity.ClassSpot.Capacity;

            var listTroop = classPowerAndCity.ClassSpot.UnitGroup;
            int i = 0, troop_count, member_count, src_member_count;
            troop_count = listTroop.Count;
            src_member_count = listTroop[troop_id].ListClassUnit.Count; // ドラッグ移動中の部隊に所属するユニット数
            foreach (var itemTroop in listTroop)
            {
                member_count = itemTroop.ListClassUnit.Count;
                if (i == troop_id)
                {
                    // ドラッグ中の部隊の位置を暗くする
                    Border border = new Border();
                    border.Name = "DropTarget";
                    // 半透明のブラシを作成する (黒の50%)
                    SolidColorBrush mySolidColorBrush = new SolidColorBrush(Colors.Black);
                    mySolidColorBrush.Opacity = 0.5;
                    border.Background = mySolidColorBrush;
                    border.BorderThickness = new Thickness(0);
                    border.Width = tile_width * member_count;
                    border.Height = tile_height;
                    this.canvasSpotUnit.Children.Add(border);
                    Canvas.SetLeft(border, header_width);
                    Canvas.SetTop(border, tile_height * i);
                }
                else if (member_count + src_member_count <= member_capacity)
                {
                    // 右の空きスペースなら「部隊を統合」動作になる
                    Border border = new Border();
                    border.Name = "DropTarget" + this.Name + "_Right_" + i.ToString();
                    border.Background = Brushes.Transparent;
                    border.BorderThickness = new Thickness(2);
                    border.BorderBrush = Brushes.Aqua;
                    border.Width = tile_width * (member_capacity - member_count) - 1;
                    border.Height = tile_height - 1;
                    this.canvasSpotUnit.Children.Add(border);
                    Canvas.SetLeft(border, header_width + tile_width * member_count + 1);
                    Canvas.SetTop(border, tile_height * i);
                }

                // 先頭ユニットの上端なら「部隊間に部隊を移動」動作になる
                if ( (troop_count < spot_capacity) && (i != troop_id) && (i != troop_id + 1) )
                {
                    Border border = new Border();
                    border.Name = "DropTarget" + this.Name + "_Top_" + i.ToString();
                    border.Background = Brushes.Transparent;
                    border.BorderThickness = new Thickness(2);
                    border.BorderBrush = Brushes.Aqua;
                    border.Width = header_width + tile_width;
                    border.Height = tile_height / 3;
                    this.canvasSpotUnit.Children.Add(border);
                    Canvas.SetTop(border, tile_height * i);
                }

                i++;
            }

            // 下の空きスペースなら「部隊を末尾に移動」動作になる
            if ( (i < spot_capacity) && (troop_id < i - 1) )
            {
                Border border = new Border();
                border.Name = "DropTarget" + this.Name + "_Bottom";
                border.Background = Brushes.Transparent;
                border.BorderThickness = new Thickness(2);
                border.BorderBrush = Brushes.Aqua;
                border.Width = header_width + tile_width * member_capacity;
                border.Height = tile_height * (spot_capacity - i);
                this.canvasSpotUnit.Children.Add(border);
                Canvas.SetTop(border, tile_height * i);
            }

            // 他の領地ウインドウを探す
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
            {
                string strTitle = itemWindow.Name;
                if ( (strTitle.StartsWith("WindowSpot")) && (strTitle != this.Name) )
                {
                    var targetPowerAndCity = (ClassPowerAndCity)itemWindow.Tag;
                    // 同じ勢力の領地ウインドウだけ対象にする
                    if (targetPowerAndCity.ClassPower.NameTag == classPowerAndCity.ClassPower.NameTag)
                    {
                        // 領地の部隊情報を調べる
                        listTroop = targetPowerAndCity.ClassSpot.UnitGroup;
                        spot_capacity = targetPowerAndCity.ClassSpot.Capacity;
                        i = 0;
                        troop_count = listTroop.Count;
                        foreach (var itemTroop in listTroop)
                        {
                            member_count = itemTroop.ListClassUnit.Count;
                            if (member_count + src_member_count <= member_capacity)
                            {
                                // 右の空きスペースなら「部隊を統合」動作になる
                                Border border = new Border();
                                border.Name = "DropTarget" + strTitle + "_Right_" + i.ToString();
                                border.Background = Brushes.Transparent;
                                border.BorderThickness = new Thickness(2);
                                border.BorderBrush = Brushes.Aqua;
                                border.Width = tile_width * (member_capacity - member_count) - 1;
                                border.Height = tile_height - 1;
                                itemWindow.canvasSpotUnit.Children.Add(border);
                                Canvas.SetLeft(border, header_width + tile_width * member_count + 1);
                                Canvas.SetTop(border, tile_height * i);
                            }

                            // 先頭ユニットの上端なら「部隊間に部隊を移動」動作になる
                            if (troop_count < spot_capacity)
                            {
                                Border border = new Border();
                                border.Name = "DropTarget" + strTitle + "_Top_" + i.ToString();
                                border.Background = Brushes.Transparent;
                                border.BorderThickness = new Thickness(2);
                                border.BorderBrush = Brushes.Aqua;
                                border.Width = header_width + tile_width;
                                border.Height = tile_height / 3;
                                itemWindow.canvasSpotUnit.Children.Add(border);
                                Canvas.SetTop(border, tile_height * i);
                            }

                            i++;
                        }

                        // 下の空きスペースなら「部隊を末尾に移動」動作になる
                        if (i < spot_capacity)
                        {
                            Border border = new Border();
                            border.Name = "DropTarget" + strTitle + "_Bottom";
                            border.Background = Brushes.Transparent;
                            border.BorderThickness = new Thickness(2);
                            border.BorderBrush = Brushes.Aqua;
                            border.Width = header_width + tile_width * member_capacity;
                            border.Height = tile_height * (spot_capacity - i);
                            itemWindow.canvasSpotUnit.Children.Add(border);
                            Canvas.SetTop(border, tile_height * i);
                        }
                    }
                }
            }

            // 戦略マップ上の領地アイコンも
            if (mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.ListMember.Count > 1)
            {
                var gridMapStrategy = (Grid)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasMain, StringName.gridMapStrategy);
                if (gridMapStrategy != null)
                {
                    var listSpot = mainWindow.ClassGameStatus.AllListSpot;
                    int spot_count = listSpot.Count;
                    for (int spot_id = 0; spot_id < spot_count; spot_id++)
                    {
                        // 同じ勢力の領地だけ
                        ClassSpot itemSpot = listSpot[spot_id];
                        if (itemSpot.PowerNameTag == classPowerAndCity.ClassPower.NameTag)
                        {
                            // 領地の部隊数に空きがあるなら
                            spot_capacity = itemSpot.Capacity;
                            troop_count = itemSpot.UnitGroup.Count;
                            if ( (troop_count < spot_capacity) && (itemSpot.NameTag != classPowerAndCity.ClassSpot.NameTag) )
                            {
                                Border border = new Border();
                                // 領地リストのインデックスで識別する
                                border.Name = "DropTargetSpot_" + spot_id.ToString();
                                border.Background = Brushes.Transparent;
                                border.BorderThickness = new Thickness(2);
                                border.BorderBrush = Brushes.Aqua;
                                // 親コントロールが Grid なので、標準だと中央配置になる。左上を原点に変えておく。
                                border.HorizontalAlignment = HorizontalAlignment.Left;
                                border.VerticalAlignment = VerticalAlignment.Top;
                                // 領地アイコンの大きさが不明なので、大きめの枠にする
                                border.Width = 64;
                                border.Height = 64;
                                border.Margin = new Thickness()
                                {
                                    Left = itemSpot.X - 32,
                                    Top = itemSpot.Y - 32
                                };
                                gridMapStrategy.Children.Add(border);
                            }
                        }
                    }
                }
            }
        }

        // 全ての部隊をドラッグ移動する際に、移動先を作る
        private bool MakeDropTarget_Whole(MainWindow mainWindow)
        {
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;
            int member_capacity = mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].MemberCapacity;

            var listTroop = classPowerAndCity.ClassSpot.UnitGroup;
            int i, troop_count, spot_capacity;
            troop_count = listTroop.Count;

            // ドラッグ中の部隊の位置を暗くする
            if (troop_count > 0)
            {
                Border border = new Border();
                border.Name = "DropTarget";
                // 半透明のブラシを作成する (黒の50%)
                SolidColorBrush mySolidColorBrush = new SolidColorBrush(Colors.Black);
                mySolidColorBrush.Opacity = 0.5;
                border.Background = mySolidColorBrush;
                border.BorderThickness = new Thickness(0);
                border.Width = tile_width * member_capacity;
                border.Height = tile_height * troop_count;
                this.canvasSpotUnit.Children.Add(border);
                Canvas.SetLeft(border, header_width);
            }
            else
            {
                // ドラッグする部隊が存在しない
                return false;
            }

            // 他の領地ウインドウを探す
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
            {
                string strTitle = itemWindow.Name;
                if ( (strTitle.StartsWith("WindowSpot")) && (strTitle != this.Name) )
                {
                    var targetPowerAndCity = (ClassPowerAndCity)itemWindow.Tag;
                    // 同じ勢力の領地ウインドウだけ対象にする
                    if (targetPowerAndCity.ClassPower.NameTag == classPowerAndCity.ClassPower.NameTag)
                    {
                        // 領地の部隊情報を調べる
                        listTroop = targetPowerAndCity.ClassSpot.UnitGroup;
                        spot_capacity = targetPowerAndCity.ClassSpot.Capacity;
                        i = 0;
                        troop_count = listTroop.Count;
                        foreach (var itemTroop in listTroop)
                        {
                            // 先頭ユニットの上端なら「部隊間に部隊を移動」動作になる
                            if (troop_count < spot_capacity)
                            {
                                Border border = new Border();
                                border.Name = "DropTarget" + strTitle + "_Top_" + i.ToString();
                                border.Background = Brushes.Transparent;
                                border.BorderThickness = new Thickness(2);
                                border.BorderBrush = Brushes.Aqua;
                                border.Width = header_width + tile_width;
                                border.Height = tile_height / 3;
                                itemWindow.canvasSpotUnit.Children.Add(border);
                                Canvas.SetTop(border, tile_height * i);
                            }

                            i++;
                        }

                        // 下の空きスペースなら「部隊を末尾に移動」動作になる
                        if (i < spot_capacity)
                        {
                            Border border = new Border();
                            border.Name = "DropTarget" + strTitle + "_Bottom";
                            border.Background = Brushes.Transparent;
                            border.BorderThickness = new Thickness(2);
                            border.BorderBrush = Brushes.Aqua;
                            border.Width = header_width + tile_width * member_capacity;
                            border.Height = tile_height * (spot_capacity - i);
                            itemWindow.canvasSpotUnit.Children.Add(border);
                            Canvas.SetTop(border, tile_height * i);
                        }
                    }
                }
            }

            // 戦略マップ上の領地アイコンも
            if (mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.ListMember.Count > 1)
            {
                var gridMapStrategy = (Grid)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasMain, StringName.gridMapStrategy);
                if (gridMapStrategy != null)
                {
                    var listSpot = mainWindow.ClassGameStatus.AllListSpot;
                    int spot_count = listSpot.Count;
                    for (int spot_id = 0; spot_id < spot_count; spot_id++)
                    {
                        // 同じ勢力の領地だけ
                        ClassSpot itemSpot = listSpot[spot_id];
                        if (itemSpot.PowerNameTag == classPowerAndCity.ClassPower.NameTag)
                        {
                            // 領地の部隊数に空きがあるなら
                            spot_capacity = itemSpot.Capacity;
                            troop_count = itemSpot.UnitGroup.Count;
                            if ( (troop_count < spot_capacity) && (itemSpot.NameTag != classPowerAndCity.ClassSpot.NameTag) )
                            {
                                Border border = new Border();
                                // 領地リストのインデックスで識別する
                                border.Name = "DropTargetSpot_" + spot_id.ToString();
                                border.Background = Brushes.Transparent;
                                border.BorderThickness = new Thickness(2);
                                border.BorderBrush = Brushes.Aqua;
                                // 親コントロールが Grid なので、標準だと中央配置になる。左上を原点に変えておく。
                                border.HorizontalAlignment = HorizontalAlignment.Left;
                                border.VerticalAlignment = VerticalAlignment.Top;
                                // 領地アイコンの大きさが不明なので、大きめの枠にする
                                border.Width = 64;
                                border.Height = 64;
                                border.Margin = new Thickness()
                                {
                                    Left = itemSpot.X - 32,
                                    Top = itemSpot.Y - 32
                                };
                                gridMapStrategy.Children.Add(border);
                            }
                        }
                    }
                }
            }

            return true;
        }

        // ユニットをドラッグ移動した後に、移動先を取り除く
        // 部隊（隊長）をドラッグする場合は、member_id を 0 にすること
        // 全部隊（全部ボタン）をドラッグする場合は、troop_id を -1 にすること
        private void RemoveDropTarget(MainWindow mainWindow, int troop_id, int member_id)
        {
            // 領地ウインドウのユニット欄に追加した枠を消去する
            for (int i = this.canvasSpotUnit.Children.Count - 1; i >= 0; i += -1) {
                UIElement Child = this.canvasSpotUnit.Children[i];
                if (Child is Border)
                {
                    var border = (Border)Child;
                    string strTarget = border.Name;
                    if (strTarget.StartsWith("DropTarget"))
                    {
                        // 枠が太くなっていれば、選択中の印
                        if (border.BorderThickness.Left > 2)
                        {
                            // 同じ領地にドロップできるのはユニットか部隊だけなので、全部隊は判定しない
                            strTarget = strTarget.Replace("DropTarget", String.Empty);
                            if (member_id >= 1)
                            {
                                // 部下なら必ず member_id が 1以上になる
                                if (DropTarget_Unit(mainWindow, troop_id, member_id, strTarget))
                                {
                                    // 領地を更新した場合は枠も消えるので、ループから抜ける
                                    break;
                                }
                            }
                            else
                            {
                                // 隊長なら member_id は常に 0 なので、省略する
                                if (DropTarget_Troop(mainWindow, troop_id, strTarget))
                                {
                                    // 領地を更新した場合は枠も消えるので、ループから抜ける
                                    break;
                                }
                            }
                        }
                        // 枠を取り除く
                        this.canvasSpotUnit.Children.Remove(border);
                    }
                }
            }

            // 他の領地の枠も
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
            {
                string strTitle = itemWindow.Name;
                if ( (strTitle.StartsWith("WindowSpot")) && (strTitle != this.Name) )
                {
                    var classPowerAndCity = (ClassPowerAndCity)this.Tag;
                    var targetPowerAndCity = (ClassPowerAndCity)itemWindow.Tag;
                    // 同じ勢力の領地ウインドウだけ対象にする
                    if (targetPowerAndCity.ClassPower.NameTag == classPowerAndCity.ClassPower.NameTag)
                    {
                        for (int i = itemWindow.canvasSpotUnit.Children.Count - 1; i >= 0; i += -1) {
                            UIElement Child = itemWindow.canvasSpotUnit.Children[i];
                            if (Child is Border)
                            {
                                var border = (Border)Child;
                                string strTarget = border.Name;
                                if (strTarget.StartsWith("DropTarget"))
                                {
                                    // 枠が太くなっていれば、選択中の印
                                    if (border.BorderThickness.Left > 2)
                                    {
                                        strTarget = strTarget.Replace("DropTarget", String.Empty);
                                        if (troop_id >= 0)
                                        {
                                            if (member_id >= 1)
                                            {
                                                // 部下なら必ず member_id が 1以上になる
                                                if (DropTarget_Unit(mainWindow, troop_id, member_id, strTarget))
                                                {
                                                    // 領地を更新した場合は枠も消えるので、ループから抜ける
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                // 隊長なら member_id は常に 0 なので、省略する
                                                if (DropTarget_Troop(mainWindow, troop_id, strTarget))
                                                {
                                                    // 領地を更新した場合は枠も消えるので、ループから抜ける
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            // 全部隊
                                            if (DropTarget_Whole(mainWindow, strTarget))
                                            {
                                                // 領地を更新した場合は枠も消えるので、ループから抜ける
                                                break;
                                            }
                                        }
                                    }
                                    // 枠を取り除く
                                    itemWindow.canvasSpotUnit.Children.Remove(border);
                                }
                            }
                        }
                    }
                }
            }

            // 戦略マップ上の枠も
            if (mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.ListMember.Count > 1)
            {
                var gridMapStrategy = (Grid)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasMain, StringName.gridMapStrategy);
                if (gridMapStrategy != null)
                {
                    for (int i = gridMapStrategy.Children.Count - 1; i >= 0; i += -1) {
                        UIElement Child = gridMapStrategy.Children[i];
                        if (Child is Border)
                        {
                            var border = (Border)Child;
                            string strTarget = border.Name;
                            if (strTarget.StartsWith("DropTarget"))
                            {
                                // 枠が太くなっていれば、選択中の印
                                if (border.BorderThickness.Left > 2)
                                {
                                    strTarget = strTarget.Replace("DropTarget", String.Empty);
                                    if (troop_id >= 0)
                                    {
                                        if (member_id >= 1)
                                        {
                                            // 部下なら必ず member_id が 1以上になる
                                            DropTarget_Unit(mainWindow, troop_id, member_id, strTarget);
                                        }
                                        else
                                        {
                                            // 隊長なら member_id は常に 0 なので、省略する
                                            DropTarget_Troop(mainWindow, troop_id, strTarget);
                                        }
                                    }
                                    else
                                    {
                                        // 全部隊
                                        DropTarget_Whole(mainWindow, strTarget);
                                    }
                                }
                                // 枠を取り除く
                                gridMapStrategy.Children.Remove(border);
                            }
                        }
                    }
                }
            }
        }

        // ドラッグ中にドロップ先を判定するための HitTest 用
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
        private void HoverDropTarget(MainWindow mainWindow, Point posMouse)
        {
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
                if ( (border != borderHit) && (border.Name.StartsWith("DropTarget")) )
                {
                    if (border.BorderThickness.Left > 2)
                    {
                        border.BorderThickness = new Thickness(2);
                    }
                }
            }

            // 他の領地の枠も
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
            {
                string strTitle = itemWindow.Name;
                if ( (strTitle.StartsWith("WindowSpot")) && (strTitle != this.Name) )
                {
                    var classPowerAndCity = (ClassPowerAndCity)this.Tag;
                    var targetPowerAndCity = (ClassPowerAndCity)itemWindow.Tag;
                    // 同じ勢力の領地ウインドウだけ対象にする
                    if (targetPowerAndCity.ClassPower.NameTag == classPowerAndCity.ClassPower.NameTag)
                    {
                        foreach (var border in itemWindow.canvasSpotUnit.Children.OfType<Border>())
                        {
                            if ( (border != borderHit) && (border.Name.StartsWith("DropTarget")) )
                            {
                                if (border.BorderThickness.Left > 2)
                                {
                                    border.BorderThickness = new Thickness(2);
                                }
                            }
                        }
                    }
                }
            }

            // 戦略マップ上の枠も
            if (mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.ListMember.Count > 1)
            {
                var gridMapStrategy = (Grid)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasMain, StringName.gridMapStrategy);
                if (gridMapStrategy != null)
                {
                    foreach (var border in gridMapStrategy.Children.OfType<Border>())
                    {
                        if ( (border != borderHit) && (border.Name.StartsWith("DropTarget")) )
                        {
                            if (border.BorderThickness.Left > 2)
                            {
                                border.BorderThickness = new Thickness(2);
                            }
                        }
                    }
                }
            }
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 雇用ウインドウを開いてた場合は閉じる
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl020_Mercenary>())
            {
                if (itemWindow.Name == this.Name + "Mercenary")
                {
                    mainWindow.canvasUI.Children.Remove(itemWindow);
                    break;
                }
            }

            // キャンバスから自身を取り除く
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

        // マウスカーソルが上に来たらユニットのパネルの色を変える
        private void panel_MouseEnter(object sender, MouseEventArgs e)
        {
            var panelUnit = (StackPanel)sender;
            // WPFの標準色をどうやって取得するのか知らないので、暗い色にする
            panelUnit.Background = Brushes.Gray;
        }
        private void panel_MouseLeave(object sender, MouseEventArgs e)
        {
            var panelUnit = (StackPanel)sender;
            // 背景の設定自体を消去する
            panelUnit.Background = null;
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
                string unit_name = ((StackPanel)sender).Name;
                string unit_id = unit_name.Replace("panelUnit", String.Empty);
                string[] strPart =  unit_id.Split('_');
                int troop_id = Int32.Parse(strPart[0]);
                int member_id = Int32.Parse(strPart[1]);
                MakeDropTarget_Unit(mainWindow, troop_id, member_id);

                // ユニット画像をそのまま流用する
                BitmapImage bitimg1 = null;
                var ri = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasSpotUnit, "imgUnit" + unit_id);
                if (ri != null)
                {
                    bitimg1 = (BitmapImage)ri.Source;
                }
                Image imgDrag = new Image();
                imgDrag.Source = bitimg1;
                imgDrag.Width = tile_width;
                imgDrag.Height = tile_width;
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
                    RemoveDropTarget(mainWindow, troop_id, member_id);

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
                    HoverDropTarget(mainWindow, e.GetPosition(mainWindow));
                }

                UIElement el = (UIElement)sender;
                Point pt = e.GetPosition(el);

                // ドラッグ量に応じて子コントロールを移動する
                Canvas.SetLeft(el, Canvas.GetLeft(el) + (pt.X - _startPoint.X));
                Canvas.SetTop(el, Canvas.GetTop(el) + (pt.Y - _startPoint.Y));
            }
        }

        // 部隊の隊長を選択した場合は、部隊メンバー全てが移動する
        private void troop_MouseRightButtonDown(object sender, MouseEventArgs e)
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
                string unit_name = ((StackPanel)sender).Name;
                string unit_id = unit_name.Replace("panelUnit", String.Empty);
                string[] strPart =  unit_id.Split('_');
                int troop_id = Int32.Parse(strPart[0]);
                MakeDropTarget_Troop(mainWindow, troop_id);

                // ドラッグ移動中の部隊に所属するユニット数
                var classPowerAndCity = (ClassPowerAndCity)this.Tag;
                var listTroop = classPowerAndCity.ClassSpot.UnitGroup;
                int member_count = listTroop[troop_id].ListClassUnit.Count;

                // 隊長のユニット画像
                BitmapImage bitimg1 = null;
                var ri = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasSpotUnit, "imgUnit" + unit_id);
                if (ri != null)
                {
                    bitimg1 = (BitmapImage)ri.Source;
                }
                Image imgDrag = new Image();
                imgDrag.Source = bitimg1;
                imgDrag.Width = tile_width;
                imgDrag.Height = tile_width;
                imgDrag.Name = "DragImage" + troop_id.ToString() + "_" + member_count.ToString();
                mainWindow.canvasUI.Children.Add(imgDrag);

                // ドラッグ画像をウインドウよりも前面に移動させる
                Canvas.SetZIndex(imgDrag, Canvas.GetZIndex(this) + 1);

                // ボタンの位置に置く
                Point pos = el.TranslatePoint(new Point(0, 0), mainWindow.canvasUI);
                Canvas.SetLeft(imgDrag, pos.X);
                Canvas.SetTop(imgDrag, pos.Y);

                // 部下のユニット画像
                for (int j = 1; j < member_count; j++)
                {
                    var ri2 = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasSpotUnit, "imgUnit" + troop_id.ToString() + "_" + j.ToString());
                    if (ri2 != null)
                    {
                        bitimg1 = (BitmapImage)ri2.Source;
                    }
                    else
                    {
                        break;
                    }
                    var imgDrag2 = new Image();
                    imgDrag2.Source = bitimg1;
                    imgDrag2.Width = tile_width;
                    imgDrag2.Height = tile_width;
                    imgDrag2.Name = "DragImageExtra" + j.ToString();
                    mainWindow.canvasUI.Children.Add(imgDrag2);

                    // 隊長の画像の右横に並べる
                    Canvas.SetZIndex(imgDrag2, Canvas.GetZIndex(this) + 1);
                    Canvas.SetLeft(imgDrag2, pos.X + tile_width * j);
                    Canvas.SetTop(imgDrag2, pos.Y);
                }

                _startPoint = e.GetPosition(el);
                _isDragUnit = true;

                // ドラッグ画像に対して、イベントを追加する
                imgDrag.CaptureMouse();
                imgDrag.MouseRightButtonUp += troop_MouseRightButtonUp;
                imgDrag.MouseMove += troop_MouseMove;
            }
        }
        private void troop_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
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
                    int member_count = Int32.Parse(strPart[1]);
                    RemoveDropTarget(mainWindow, troop_id, 0);

                    // ドラッグ画像を取り除く
                    mainWindow.canvasUI.Children.Remove(el);
                    for (int j = 1; j < member_count; j++)
                    {
                        var ri = (Image)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, "DragImageExtra" + j.ToString());
                        if (ri != null)
                        {
                            mainWindow.canvasUI.Children.Remove(ri);
                        }
                        else
                        {
                            break;
                        }
                    }
                 }
            }
        }
        private void troop_MouseMove(object sender, MouseEventArgs e)
        {
            // ドラック中
            if (_isDragUnit == true)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    // ドロップ先をホバー表示する
                    HoverDropTarget(mainWindow, e.GetPosition(mainWindow));
                }

                UIElement el = (UIElement)sender;
                Point pt = e.GetPosition(el);

                // ドラッグ量に応じて子コントロールを移動する
                Canvas.SetLeft(el, Canvas.GetLeft(el) + (pt.X - _startPoint.X));
                Canvas.SetTop(el, Canvas.GetTop(el) + (pt.Y - _startPoint.Y));

                // 右横の子コントロールも同時に移動させる
                string unit_name = ((Image)sender).Name;
                string unit_id = unit_name.Replace("DragImage", String.Empty);
                string[] strPart =  unit_id.Split('_');
                int member_count = Int32.Parse(strPart[1]);
                for (int j = 1; j < member_count; j++)
                {
                    var ri = (Image)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, "DragImageExtra" + j.ToString());
                    if (ri != null)
                    {
                        Canvas.SetLeft(ri, Canvas.GetLeft(ri) + (pt.X - _startPoint.X));
                        Canvas.SetTop(ri, Canvas.GetTop(ri) + (pt.Y - _startPoint.Y));
                    }
                    else
                    {
                        break;
                    }
                }

            }
        }

        // 全部ボタンを押した場合は、全ての部隊が移動する
        private void whole_MouseDown(object sender, MouseEventArgs e)
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
                if (MakeDropTarget_Whole(mainWindow) == false)
                {
                    // ドラッグする部隊が存在しない場合は、何もせずに終わる
                    return;
                }

                // ドラッグ移動中の部隊に所属するユニット数
                var classPowerAndCity = (ClassPowerAndCity)this.Tag;
                var listTroop = classPowerAndCity.ClassSpot.UnitGroup;
                int troop_count = listTroop.Count;

                // 一番上の隊長のユニット画像
                BitmapImage bitimg1 = null;
                var ri = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasSpotUnit, "imgUnit0_0");
                if (ri != null)
                {
                    bitimg1 = (BitmapImage)ri.Source;
                }
                Image imgDrag = new Image();
                imgDrag.Source = bitimg1;
                imgDrag.Width = tile_width;
                imgDrag.Height = tile_width;
                imgDrag.Name = "DragImage" + troop_count.ToString();
                mainWindow.canvasUI.Children.Add(imgDrag);

                // ドラッグ画像をウインドウよりも前面に移動させる
                Canvas.SetZIndex(imgDrag, Canvas.GetZIndex(this) + 1);

                // 画像をマウス・カーソルの下に置く
                Point posMouse = e.GetPosition(mainWindow.canvasUI);
                Point pos = new Point(posMouse.X - tile_width / 2, posMouse.Y - tile_width / 2);
                Canvas.SetLeft(imgDrag, pos.X);
                Canvas.SetTop(imgDrag, pos.Y);

                // 残りの隊長のユニット画像
                for (int i = 1; i < troop_count; i++)
                {
                    var ri2 = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasSpotUnit, "imgUnit" + i.ToString() + "_0");
                    if (ri2 != null)
                    {
                        bitimg1 = (BitmapImage)ri2.Source;
                    }
                    else
                    {
                        break;
                    }
                    var imgDrag2 = new Image();
                    imgDrag2.Source = bitimg1;
                    imgDrag2.Width = tile_width;
                    imgDrag2.Height = tile_width;
                    imgDrag2.Name = "DragImageExtra" + i.ToString();
                    mainWindow.canvasUI.Children.Add(imgDrag2);

                    // 隊長の画像の下に並べる
                    Canvas.SetZIndex(imgDrag2, Canvas.GetZIndex(this) + 1);
                    Canvas.SetLeft(imgDrag2, pos.X);
                    Canvas.SetTop(imgDrag2, pos.Y + tile_width * i);
                }

                // マウス・カーソルからの相対位置にする
                _startPoint = new Point(tile_width / 2, tile_width / 2);
                _isDragUnit = true;

                // ドラッグ画像に対して、イベントを追加する
                imgDrag.CaptureMouse();
                imgDrag.MouseUp += whole_MouseUp;
                imgDrag.MouseMove += whole_MouseMove;
            }
        }
        private void whole_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // ドラック中なら終了する
            if (_isDragUnit == true)
            {
                UIElement el = (UIElement)sender;
                el.ReleaseMouseCapture();
                el.MouseUp -= whole_MouseUp;
                el.MouseMove -= whole_MouseMove;
                _isDragUnit = false;

                var mainWindow = (MainWindow)Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    // ドロップ先を取り除くと同時に、ドロップ判定を行う
                    string unit_name = ((Image)sender).Name;
                    string unit_id = unit_name.Replace("DragImage", String.Empty);
                    int troop_count = Int32.Parse(unit_id);
                    RemoveDropTarget(mainWindow, -1, 0);

                    // ドラッグ画像を取り除く
                    mainWindow.canvasUI.Children.Remove(el);
                    for (int i = 1; i < troop_count; i++)
                    {
                        var ri = (Image)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, "DragImageExtra" + i.ToString());
                        if (ri != null)
                        {
                            mainWindow.canvasUI.Children.Remove(ri);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
        private void whole_MouseMove(object sender, MouseEventArgs e)
        {
            // ドラック中
            if (_isDragUnit == true)
            {
                var mainWindow = (MainWindow)Application.Current.MainWindow;
                if (mainWindow != null)
                {
                    // ドロップ先をホバー表示する
                    HoverDropTarget(mainWindow, e.GetPosition(mainWindow));
                }

                UIElement el = (UIElement)sender;
                Point pt = e.GetPosition(el);

                // ドラッグ量に応じて子コントロールを移動する
                Canvas.SetLeft(el, Canvas.GetLeft(el) + (pt.X - _startPoint.X));
                Canvas.SetTop(el, Canvas.GetTop(el) + (pt.Y - _startPoint.Y));

                // 下の子コントロールも同時に移動させる
                string unit_name = ((Image)sender).Name;
                string unit_id = unit_name.Replace("DragImage", String.Empty);
                int troop_count = Int32.Parse(unit_id);
                for (int i = 1; i < troop_count; i++)
                {
                    var ri = (Image)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUI, "DragImageExtra" + i.ToString());
                    if (ri != null)
                    {
                        Canvas.SetLeft(ri, Canvas.GetLeft(ri) + (pt.X - _startPoint.X));
                        Canvas.SetTop(ri, Canvas.GetTop(ri) + (pt.Y - _startPoint.Y));
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        // ユニット情報ウインドウを開く
        private void unit_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // どのユニットをクリックしたのか
            ClassCityAndUnit classCityAndUnit = new ClassCityAndUnit();
            classCityAndUnit.ClassPowerAndCity = (ClassPowerAndCity)this.Tag;
            classCityAndUnit.ClassUnit = (ClassUnit)((StackPanel)sender).Tag;

            // ウインドウ番号によって表示位置を変える
            double offsetLeft = 0, offsetTop = 0;
            if (mainWindow.canvasUI.Margin.Left < 0)
            {
                offsetLeft = mainWindow.canvasUI.Margin.Left * -1;
            }
            if (mainWindow.canvasUI.Margin.Top < 0)
            {
                offsetTop = mainWindow.canvasUI.Margin.Top * -1;
            }

            // 既に表示されてるユニット・ウインドウをチェックする
            int window_id, max_id = 0;
            var id_list = new List<int>();
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl015_Unit>())
            {
                string strTitle = itemWindow.Name;
                if (strTitle.StartsWith("WindowUnit"))
                {
                    window_id = Int32.Parse(strTitle.Replace("WindowUnit", String.Empty));
                    id_list.Add(window_id);
                    if (max_id < window_id)
                    {
                        max_id = window_id;
                    }
                    ClassUnit testUnit = ((ClassCityAndUnit)itemWindow.Tag).ClassUnit;
                    if (testUnit == classCityAndUnit.ClassUnit)
                    {
                        // ユニット・ウインドウを既に開いてる場合は、新規に作らない
                        max_id = -1;
                        itemWindow.Margin = new Thickness()
                        {
                            Left = offsetLeft + ((window_id - 1) % 10) * 50 + ((window_id - 1) / 10) * 50,
                            Top = offsetTop + ((window_id - 1) % 10) * 50
                        };

                        // ユニット・ウインドウをこのウインドウよりも前面に移動させる
                        Canvas.SetZIndex(itemWindow, Canvas.GetZIndex(this) + 1);

                        break;
                    }
                }
            }
            if (max_id >= 0)
            {
                if (max_id > id_list.Count)
                {
                    // ウインドウ個数よりも最大値が大きいなら、未使用の番号を使って作成する
                    for (window_id = 1; window_id < max_id; window_id++)
                    {
                        if (id_list.Contains(window_id) == false)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    // 使用中のウインドウ番号の最大値 + 1 にして、新規に作成する
                    window_id = max_id + 1;
                }
                var windowUnit = new UserControl015_Unit();
                windowUnit.Tag = classCityAndUnit;
                windowUnit.Name = "WindowUnit" + window_id.ToString();
                windowUnit.Margin = new Thickness()
                {
                    Left = offsetLeft + ((window_id - 1) % 10) * 50 + ((window_id - 1) / 10) * 50,
                    Top = offsetTop + ((window_id - 1) % 10) * 50
                };
                windowUnit.SetData();
                mainWindow.canvasUI.Children.Add(windowUnit);
            }
            id_list.Clear();
        }

        // 領地の雇用ウインドウを開く
        private void btnMercenary_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 領地から雇用なら、ClassUnit 部分を null にする
            ClassCityAndUnit classCityAndUnit = new ClassCityAndUnit();
            classCityAndUnit.ClassPowerAndCity = (ClassPowerAndCity)this.Tag;
            classCityAndUnit.ClassUnit = null;

            // 領地ウインドウの右横に雇用ウインドウを表示する
            double offsetLeft = this.Margin.Left + this.Width;

            // 既に雇用ウインドウが表示されてる場合は再利用する
            bool isFound = false;
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl020_Mercenary>())
            {
                string strTitle = itemWindow.Name;
                if ( (strTitle.StartsWith("WindowSpot")) || (strTitle.StartsWith("WindowUnit")) )
                {
                    // 新規に作らない
                    itemWindow.Tag = classCityAndUnit;
                    itemWindow.Name = this.Name + "Mercenary";
                    if (this.Margin.Left + this.Width / 2  > mainWindow.CanvasMainWidth / 2)
                    {
                        // 画面の右側なら、左横に表示する
                        offsetLeft = this.Margin.Left - itemWindow.Width;
                    }
                    itemWindow.Margin = new Thickness()
                    {
                        Left = offsetLeft,
                        Top = this.Margin.Top
                    };
                    itemWindow.SetData();

                    // 雇用ウインドウをこのウインドウよりも前面に移動させる
                    Canvas.SetZIndex(itemWindow, Canvas.GetZIndex(this) + 1);

                    isFound = true;
                    break;
                }
            }
            if (isFound == false)
            {
                // 新規に作成する
                var windowMercenary = new UserControl020_Mercenary();
                windowMercenary.Tag = classCityAndUnit;
                windowMercenary.Name = this.Name + "Mercenary";
                if (this.Margin.Left + this.Width / 2 > mainWindow.CanvasMainWidth / 2)
                {
                    // 画面の右側なら、左横に表示する
                    offsetLeft = this.Margin.Left - windowMercenary.Width;
                }
                windowMercenary.Margin = new Thickness()
                {
                    Left = offsetLeft,
                    Top = this.Margin.Top
                };
                windowMercenary.SetData();
                mainWindow.canvasUI.Children.Add(windowMercenary);
            }
        }

    }
}
