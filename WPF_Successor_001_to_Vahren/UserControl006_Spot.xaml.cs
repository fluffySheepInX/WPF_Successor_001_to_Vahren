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
                   .Select(x => Panel.GetZIndex(x))
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
            if (classPowerAndCity.ClassPower.NameTag ==             mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.NameTag)
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

            // 最初に全て消去する
            this.stkUnitList.Children.Clear();

            var listTroop = mainWindow.ClassGameStatus.AllListSpot
                .Where(x => x.NameTag == classPowerAndCity.ClassSpot.NameTag)
                .First()
                .UnitGroup;
            // 全ての部隊を表示する（駐留数の制限を超えていても許容する）
            int i = 0, j;
            foreach (var itemTroop in listTroop)
            {
                // 部隊のパネル
                StackPanel stkTroop = new StackPanel();
                stkTroop.Name = "stkTroop" + i.ToString();
                stkTroop.Orientation = Orientation.Horizontal;
                stkTroop.Height = 68;

                // 部隊メンバー
                j = 0;
                foreach (var itemUnit in itemTroop.ListClassUnit)
                {
                    // 部隊の陣形はリーダー（先頭のユニット）を参照する
                    if (j == 0)
                    {
                        if (isControl)
                        {
                            // 操作パネル
                            StackPanel stkControl = new StackPanel();
                            stkControl.Name = "stkControl" + i.ToString();

                            // 出撃ボタン
                            Button btnSelect = new Button();
                            btnSelect.Name = "btnSelect" + i.ToString();
                            btnSelect.Height = 30;
                            btnSelect.Margin = new Thickness(2);
                            btnSelect.FontSize = 15;
                            btnSelect.Content = "出撃";
                            stkControl.Children.Add(btnSelect);

                            // 陣形コンボボックス
                            ComboBox cmbFormation = new ComboBox();
                            cmbFormation.Name = "cmbFormation" + i.ToString();
                            cmbFormation.Height = 30;
                            cmbFormation.Width = 44;
                            cmbFormation.Margin = new Thickness(2);
                            cmbFormation.SelectedValuePath = "Id";
                            cmbFormation.DisplayMemberPath = "Formation";
                            cmbFormation.ItemsSource = formation;
                            cmbFormation.FontSize = 15;
                            cmbFormation.SelectedIndex = itemUnit.Formation.Id;
                            stkControl.Children.Add(cmbFormation);
                            stkTroop.Children.Add(stkControl);
                        }
                        else
                        {
                            // 操作できない場合は、陣形だけ表示する
                            Grid grid = new Grid();
                            grid.Width = 48;
                            Label label = new Label();
                            label.Background = SystemColors.WindowBrush;
                            label.Width = 30;
                            label.Height = 30;
                            label.FontSize = 15;
                            label.Content = itemUnit.Formation.Formation;
                            grid.Children.Add(label);
                            stkTroop.Children.Add(grid);
                        }
                    }

                    // ユニットのボタン
                    Button btnUnit = new Button();
                    btnUnit.Name = "btnUnit" + i.ToString() + "_" + j.ToString();
                    btnUnit.Height = 68;
                    btnUnit.Width = 48;
                    btnUnit.Background = Brushes.Transparent;
                    btnUnit.BorderThickness = new Thickness(0);

                    // ユニットのパネル
                    StackPanel stkUnit = new StackPanel();
                    stkUnit.Name = "stkUnit" + i.ToString() + "_" + j.ToString();

                    // ユニットの画像
                    BitmapImage bitimg1 = new BitmapImage(new Uri(pathDirectory + itemUnit.Image));
                    Image imgUnit = new Image();
                    imgUnit.Name = "imgUnit" + i.ToString() + "_" + j.ToString();
                    imgUnit.Source = bitimg1;
                    imgUnit.Height = 48;
                    imgUnit.Width = 48;
                    // 画像本来のピクセルサイズで表示する場合は、PixelWidth と PixelHeight を指定する
                    //imgUnit.Height = bitimg1.PixelHeight;
                    //imgUnit.Width = bitimg1.PixelWidth;
                    stkUnit.Children.Add(imgUnit);

                    // ユニットのレベル
                    Label lblLevel = new Label();
                    lblLevel.Name = "lblLevel" + i.ToString() + "_" + j.ToString();
                    lblLevel.Height = 20;
                    lblLevel.FontSize = 15;
                    lblLevel.Padding = new Thickness(-5);
                    lblLevel.Foreground = Brushes.White;
                    lblLevel.HorizontalAlignment = HorizontalAlignment.Center;
                    lblLevel.Content = "lv" + itemUnit.Level;
                    stkUnit.Children.Add(lblLevel);
                    btnUnit.Content = stkUnit;

                    stkTroop.Children.Add(btnUnit);
                    j++;
                }

                this.stkUnitList.Children.Add(stkTroop);
                i++;
            }
            
            
            
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
                       .Select(x => Panel.GetZIndex(x))
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
            }
        }
        private void win_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // ドラック中なら終了する
            if (_isDrag == true)
            {
                UIElement el = (UIElement)sender;
                el.ReleaseMouseCapture();
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

    }
}