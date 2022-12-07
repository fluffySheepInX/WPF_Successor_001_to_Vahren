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
using WPF_Successor_001_to_Vahren._005_Class;

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// UserControl020_Mercenary.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl020_Mercenary : UserControl
    {
        public UserControl020_Mercenary()
        {
            InitializeComponent();
        }

        // 定数
        // 項目サイズをここで調節できます
        private const int item_height = 56, space_height = 4, btn_width = 52, btn_height = 52;

        // 最初に呼び出した時
        public void SetData()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 最前面に配置する
            var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
            if ((listWindow != null) && (listWindow.Any()))
            {
                int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                Canvas.SetZIndex(this, maxZ + 1);
            }

            // ユニットの情報を表示する
            DisplayMercenary(mainWindow);

            // ウインドウ枠
            SetWindowFrame(mainWindow);
        }

        // ウインドウ枠を作る
        private void SetWindowFrame(MainWindow mainWindow)
        {
            // ウインドウスキンを読み込む
            List<string> strings = new List<string>();
            strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
            strings.Add("006_WindowImage");
            strings.Add("wnd0.png");
            string path = System.IO.Path.Combine(strings.ToArray());
            if (System.IO.File.Exists(path) == false)
            {
                // 画像が存在しない場合は、デザイン時のまま（色や透明度は xaml で指定する）
                return;
            }
            var skin_bitmap = new BitmapImage(new Uri(path));
            Int32Rect rect;
            ImageBrush myImageBrush;

            // RPGツクールXP (192x128) と VX (128x128) のスキンに対応する
            if ((skin_bitmap.PixelHeight != 128) || ((skin_bitmap.PixelWidth != 128) && (skin_bitmap.PixelWidth != 192)))
            {
                // その他の画像は、そのまま引き延ばして表示する
                // ブラシ設定によって、タイルしたり、アスペクト比を保ったりすることも可能
                myImageBrush = new ImageBrush(skin_bitmap);
                myImageBrush.Stretch = Stretch.Fill;
                this.rectWindowPlane.Fill = myImageBrush;
                return;
            }

            // 不要な背景を表示しない
            this.rectShadowRight.Visibility = Visibility.Hidden;
            this.rectShadowBottom.Visibility = Visibility.Hidden;

            // 中央
            rect = new Int32Rect(0, 0, skin_bitmap.PixelWidth - 64, skin_bitmap.PixelWidth - 64);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Stretch = Stretch.Fill;
            this.rectWindowPlane.Margin = new Thickness(4, 4, 4, 4);
            this.rectWindowPlane.Fill = myImageBrush;

            // 左上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 64, 0, 16, 16);
            this.imgWindowLeftTop.Source = new CroppedBitmap(skin_bitmap, rect);

            // 右上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 16, 0, 16, 16);
            this.imgWindowRightTop.Source = new CroppedBitmap(skin_bitmap, rect);

            // 左下
            rect = new Int32Rect(skin_bitmap.PixelWidth - 64, 48, 16, 16);
            this.imgWindowLeftBottom.Source = new CroppedBitmap(skin_bitmap, rect);

            // 右上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 16, 48, 16, 16);
            this.imgWindowRightBottom.Source = new CroppedBitmap(skin_bitmap, rect);

            // 上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 48, 0, 32, 16);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowTop.Fill = myImageBrush;

            // 下
            rect = new Int32Rect(skin_bitmap.PixelWidth - 48, 48, 32, 16);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowBottom.Fill = myImageBrush;

            // 左
            rect = new Int32Rect(skin_bitmap.PixelWidth - 64, 16, 16, 32);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowLeft.Fill = myImageBrush;

            // 右
            rect = new Int32Rect(skin_bitmap.PixelWidth - 16, 16, 16, 32);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowRight.Fill = myImageBrush;
        }

        // 既に表示されていて、表示を更新する際
        public void DisplayMercenary(MainWindow mainWindow)
        {
            ClassCityAndUnit classCityAndUnit = (ClassCityAndUnit)this.Tag;
            ClassPower targetPower = classCityAndUnit.ClassPowerAndCity.ClassPower;
            ClassSpot targetSpot = classCityAndUnit.ClassPowerAndCity.ClassSpot;
            // null かどうかで、呼び出し元（ユニットか領地）を識別する
            ClassUnit? targetUnit = classCityAndUnit.ClassUnit;

            // タイトル
            if (targetSpot == null)
            {
                this.txtTitle.Text = this.Name; // ウインドウ番号を表示する実験用

                // targetSpot は必ず指定しないといけない
                return;
            }
            else if (targetUnit == null)
            {
                // targetUnit が null なら領地の雇用とみなす
                this.txtTitle.Text = targetSpot.Name + "で雇用";
            }
            else
            {
                // targetSpot に居る targetUnit による雇用とみなす
                this.txtTitle.Text = targetUnit.Name + "の雇用";
            }

            // 雇用可能なユニットのリストを初期化する
            this.panelList.Children.Clear();
            int item_count = 0;

            // 勢力の標準雇用クラス
            foreach (var itemNameTag in targetPower.ListCommonConscription)
            {
                // 既に登録済みのクラスは省く
                bool isFound = false;
                foreach (var oldItem in this.panelList.Children.OfType<Grid>())
                {
                    if (oldItem.Name == itemNameTag)
                    {
                        isFound = true;
                        break;
                    }
                }
                if (isFound == true)
                {
                    continue;
                }

                // 元にするクラスのデータを取得する
                ClassUnit? itemBaseUnit = mainWindow.ClassGameStatus
                    .ListUnit
                    .Where(x => x.NameTag == itemNameTag)
                    .FirstOrDefault();
                if (itemBaseUnit == null)
                {
                    continue;
                }

                // 共通設定で Grid を作る
                Grid gridItem = new Grid();
                ColumnDefinition colDef1 = new ColumnDefinition();
                ColumnDefinition colDef2 = new ColumnDefinition();
                colDef1.Width = new GridLength(btn_width);
                colDef2.Width = new GridLength(1.0, GridUnitType.Star);
                gridItem.ColumnDefinitions.Add(colDef1);
                gridItem.ColumnDefinitions.Add(colDef2);
                RowDefinition rowDef1 = new RowDefinition();
                RowDefinition rowDef2 = new RowDefinition();
                rowDef1.Height = new GridLength(1.0, GridUnitType.Star);
                rowDef2.Height = new GridLength(1.0, GridUnitType.Star);
                gridItem.RowDefinitions.Add(rowDef1);
                gridItem.RowDefinitions.Add(rowDef2);
                gridItem.Height = item_height;
                gridItem.Margin = new Thickness(5, space_height, 0, space_height);
                gridItem.Name = itemNameTag;

                // ユニット画像
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("040_ChipImage");
                strings.Add(itemBaseUnit.Image);
                string path = System.IO.Path.Combine(strings.ToArray());
                BitmapImage bitimg1 = new BitmapImage(new Uri(path));

                // 画像は本来のピクセルサイズで表示する
                Image imgUnit = new Image();
                imgUnit.Width = bitimg1.PixelWidth;
                imgUnit.Height = bitimg1.PixelHeight;
                imgUnit.Source = bitimg1;

                // ボタンの枠は画像よりも大きくする
                Button btnUnit = new Button();
                btnUnit.Name = "btnUnit" + item_count.ToString();
                btnUnit.Tag = itemBaseUnit;
                //btnUnit.Background = Brushes.Transparent;
                btnUnit.Width = btn_width;
                btnUnit.Height = btn_height;
                btnUnit.Focusable = false;
                btnUnit.Content = imgUnit;
                btnUnit.Click += btnUnit_Click;
                btnUnit.MouseRightButtonDown += btnUnit_MouseRightButtonDown;
                Grid.SetRowSpan(btnUnit, 2);
                gridItem.Children.Add(btnUnit);

                // 名前
                TextBlock txtName = new TextBlock();
                txtName.Name = "txtName" + item_count.ToString();
                txtName.FontSize = 20;
                txtName.Foreground = Brushes.White;
                txtName.HorizontalAlignment = HorizontalAlignment.Center;
                txtName.Text = itemBaseUnit.Name;
                Grid.SetColumn(txtName, 1);
                gridItem.Children.Add(txtName);

                // 金額
                TextBlock txtPrice = new TextBlock();
                txtPrice.Name = "txtPrice" + item_count.ToString();
                txtPrice.FontSize = 20;
                txtPrice.Foreground = Brushes.White;
                txtPrice.HorizontalAlignment = HorizontalAlignment.Center;
                txtPrice.Text = "金" + itemBaseUnit.Price.ToString();
                Grid.SetColumn(txtPrice, 1);
                Grid.SetRow(txtPrice, 1);
                gridItem.Children.Add(txtPrice);

                this.panelList.Children.Add(gridItem);
                item_count++;
            }

            /*
                        // この領地で雇用できるユニット（中立時に登場するモンスターも含む）
                        // spot構造体の monster や wanderingMonster は違う？
                        if (targetSpot.ListWanderingMonster.Count > 0)
                        {
                            // データ構造「クラス名*数値」の定義がヴァーレントゥーガと違う。
                        }

                        // 指定されたユニットが雇用できるユニット
                        if (targetUnit != null)
                        {
                            // まだデータとして設定されてない
                        }
            */

            // リストの項目数が 7個未満なら、ウインドウの高さを低くする
            if (item_count < 1)
            {
                item_count = 1;
            }
            else if (item_count > 7)
            {
                item_count = 7;
            }
            double new_height = (item_height + space_height * 2) * item_count;
            this.scrollList.Height = new_height;
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
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
                if ((listWindow != null) && (listWindow.Any()))
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
                thickness.Left = Math.Truncate(this.Margin.Left + (pt.X - _startPoint.X));
                thickness.Top = Math.Truncate(this.Margin.Top + (pt.Y - _startPoint.Y));
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
                if ((listWindow != null) && (listWindow.Any()))
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
                if ((listWindow != null) && (listWindow.Any()))
                {
                    int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                    Canvas.SetZIndex(this, maxZ + 1);
                }
            }
        }


        // ボタンを左クリックすると一人雇う
        private void btnUnit_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            ClassCityAndUnit classCityAndUnit = (ClassCityAndUnit)this.Tag;
            ClassPower targetPower = classCityAndUnit.ClassPowerAndCity.ClassPower;
            ClassSpot targetSpot = classCityAndUnit.ClassPowerAndCity.ClassSpot;
            ClassUnit? targetUnit = classCityAndUnit.ClassUnit;

            // 雇用するユニットの元データ
            var btnUnit = (Button)sender;
            ClassUnit baseUnit = (ClassUnit)btnUnit.Tag;
            //this.txtTitle.Text = baseUnit.Name + "を一人雇う"; // 実験用

            // 金が足りなかったらダメ
            if (targetPower.Money < baseUnit.Price)
            {
                return;
            }

            // ユニットの追加先が存在するか調べる
            ClassHorizontalUnit? targetTroop = null;
            int member_capacity = mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].MemberCapacity;
            if (targetUnit != null)
            {
                // ユニット情報ウインドウから雇用する場合は、そのユニットの部隊に優先的に追加する
                int member_count = 0;
                var listTroop = targetSpot.UnitGroup;
                foreach (var itemTroop in listTroop)
                {
                    foreach (var itemUnit in itemTroop.ListClassUnit)
                    {
                        if (itemUnit == targetUnit)
                        {
                            member_count = itemTroop.ListClassUnit.Count;
                            if (member_count < member_capacity)
                            {
                                // 本来は、隊長が部下にできるかも調べないといけない
                                // 指定された targetUnit が隊長とは限らないことに注意
                                // とりあえず、今は空きさえあれば、部下にできるとみなす
                                targetTroop = itemTroop;
                            }
                            break;
                        }
                    }
                    if (member_count > 0)
                    {
                        break;
                    }
                }
            }
            if (targetTroop == null)
            {
                int spot_capacity = targetSpot.Capacity;
                var listTroop = targetSpot.UnitGroup;
                foreach (var itemTroop in listTroop)
                {
                    if (itemTroop.ListClassUnit.Count < member_capacity)
                    {
                        // 本来は、隊長が部下にできるかも調べないといけない
                        // とりあえず、今は空きさえあれば、部下にできるとみなす
                        targetTroop = itemTroop;
                        break;
                    }
                }
                if ((targetTroop == null) && (targetSpot.UnitGroup.Count >= spot_capacity))
                {
                    // 加入可能な部隊が無くて、新規部隊を作る空も無ければ、何もせずに終わる
                    return;
                }
            }

            // 資金を減らす
            targetPower.Money -= baseUnit.Price;

            // 元データから新しくユニットを作る
            ClassUnit newUnit = baseUnit.DeepCopy();
            newUnit.ID = mainWindow.ClassGameStatus.IDCount;
            mainWindow.ClassGameStatus.SetIDCount();
            if (targetTroop == null)
            {
                // 新規に部隊を作って隊長にする
                var listUnit = new List<ClassUnit>();
                listUnit.Add(newUnit);
                targetSpot.UnitGroup.Add(new ClassHorizontalUnit()
                {
                    Spot = targetSpot,
                    FlagDisplay = true,
                    ListClassUnit = listUnit
                });
            }
            else
            {
                // 部隊の末尾に追加する
                targetTroop.ListClassUnit.Add(newUnit);
            }

            // 領地ウインドウが開いてるなら、表示を更新する
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
            {
                string strTitle = itemWindow.Name;
                if (strTitle.StartsWith("WindowSpot"))
                {
                    if (targetSpot.NameTag == ((ClassPowerAndCity)itemWindow.Tag).ClassSpot.NameTag)
                    {
                        itemWindow.UpdateSpotUnit(mainWindow);
                        break;
                    }
                }
            }

            // 勢力メニューを更新する
            mainWindow.ClassGameStatus.WindowStrategyMenu.DisplayPowerStatus(mainWindow);
        }


        // 右ボタンを押して、同じ要素上で離した時だけ反応させる
        private void btnUnit_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            // マウスのキャプチャを開始する
            UIElement el = (UIElement)sender;
            if (el != null)
            {
                el.CaptureMouse();
                el.MouseRightButtonUp += btnUnit_MouseRightButtonUp;
            }
        }
        // ボタンを右クリックすると部隊の空の分まで雇う
        private void btnUnit_MouseRightButtonUp(object sender, MouseEventArgs e)
        {
            // 右ボタンを押した時にイベント・ハンドラーが追加されるので、必ず押してるはず
            UIElement el = (UIElement)sender;
            el.ReleaseMouseCapture();
            el.MouseRightButtonUp -= btnUnit_MouseRightButtonUp;
            if (el.IsMouseOver == false)
            {
                // 右ボタンが他所で離された時は反応しない
                return;
            }

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            ClassCityAndUnit classCityAndUnit = (ClassCityAndUnit)this.Tag;
            ClassPower targetPower = classCityAndUnit.ClassPowerAndCity.ClassPower;
            ClassSpot targetSpot = classCityAndUnit.ClassPowerAndCity.ClassSpot;
            ClassUnit? targetUnit = classCityAndUnit.ClassUnit;

            // 雇用するユニットの元データ
            var btnUnit = (Button)sender;
            ClassUnit baseUnit = (ClassUnit)btnUnit.Tag;

            // 金が足りなかったらダメ
            if (targetPower.Money < baseUnit.Price)
            {
                return;
            }

            // ユニットの追加先が存在するか調べる
            ClassHorizontalUnit? targetTroop = null;
            int member_capacity = mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].MemberCapacity;
            int add_count = member_capacity;
            if (targetUnit != null)
            {
                // ユニット情報ウインドウから雇用する場合は、そのユニットの部隊に優先的に追加する
                int member_count = 0;
                var listTroop = targetSpot.UnitGroup;
                foreach (var itemTroop in listTroop)
                {
                    foreach (var itemUnit in itemTroop.ListClassUnit)
                    {
                        if (itemUnit == targetUnit)
                        {
                            member_count = itemTroop.ListClassUnit.Count;
                            if (member_count < member_capacity)
                            {
                                // 本来は、隊長が部下にできるかも調べないといけない
                                // 指定された targetUnit が隊長とは限らないことに注意
                                // とりあえず、今は空きさえあれば、部下にできるとみなす
                                targetTroop = itemTroop;
                                add_count = member_capacity - member_count;
                            }
                            break;
                        }
                    }
                    if (member_count > 0)
                    {
                        break;
                    }
                }
            }
            if (targetTroop == null)
            {
                int spot_capacity = targetSpot.Capacity;
                var listTroop = targetSpot.UnitGroup;
                foreach (var itemTroop in listTroop)
                {
                    int member_count = itemTroop.ListClassUnit.Count;
                    if (member_count < member_capacity)
                    {
                        // 本来は、隊長が部下にできるかも調べないといけない
                        // とりあえず、今は空きさえあれば、部下にできるとみなす
                        targetTroop = itemTroop;
                        add_count = member_capacity - member_count;
                        break;
                    }
                }
                if ((targetTroop == null) && (targetSpot.UnitGroup.Count >= spot_capacity))
                {
                    // 加入可能な部隊が無くて、新規部隊を作る空も無ければ、何もせずに終わる
                    return;
                }
            }

            // 資金が足りないなら追加する数を減らす
            while (targetPower.Money < baseUnit.Price * add_count)
            {
                add_count--;
            }
            //this.txtTitle.Text = baseUnit.Name + "を" + add_count + "人雇う"; // 実験用

            // 資金を減らす
            targetPower.Money -= baseUnit.Price * add_count;

            // 元データから新しくユニットを作る
            ClassUnit newUnit = baseUnit.DeepCopy();
            newUnit.ID = mainWindow.ClassGameStatus.IDCount;
            mainWindow.ClassGameStatus.SetIDCount();
            if (targetTroop == null)
            {
                // 新規に部隊を作って隊長にする
                var listUnit = new List<ClassUnit>();
                listUnit.Add(newUnit);
                targetTroop = new ClassHorizontalUnit()
                {
                    Spot = targetSpot,
                    FlagDisplay = true,
                    ListClassUnit = listUnit
                };
                targetSpot.UnitGroup.Add(targetTroop);
            }
            else
            {
                // 部隊の末尾に追加する
                targetTroop.ListClassUnit.Add(newUnit);
            }
            add_count--;

            // 追加するのが複数なら
            while (add_count > 0)
            {
                // 元データから新しくユニットを作る
                newUnit = baseUnit.DeepCopy();
                newUnit.ID = mainWindow.ClassGameStatus.IDCount;
                mainWindow.ClassGameStatus.SetIDCount();

                // 部隊の末尾に追加する（新規部隊は既に作成済み）
                targetTroop.ListClassUnit.Add(newUnit);

                add_count--;
            }

            // 領地ウインドウが開いてるなら、表示を更新する
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
            {
                string strTitle = itemWindow.Name;
                if (strTitle.StartsWith("WindowSpot"))
                {
                    if (targetSpot.NameTag == ((ClassPowerAndCity)itemWindow.Tag).ClassSpot.NameTag)
                    {
                        itemWindow.UpdateSpotUnit(mainWindow);
                        break;
                    }
                }
            }

            // 勢力メニューを更新する
            mainWindow.ClassGameStatus.WindowStrategyMenu.DisplayPowerStatus(mainWindow);
        }

    }
}
