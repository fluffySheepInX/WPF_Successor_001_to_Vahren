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
using WPF_Successor_001_to_Vahren._006_ClassStatic;

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// Page010_SortieMenu.xaml の相互作用ロジック
    /// </summary>
    public partial class Page010_SortieMenu : Page
    {
        public Page010_SortieMenu()
        {
            InitializeComponent();
            var window = Application.Current.Properties["window"];
            if (window == null)
            {
                return;
            }
            var mainWindow = window as MainWindow;
            if (mainWindow == null)
            {
                return;
            }
            var spots = Application.Current.Properties["spots"];
            if (spots == null)
            {
                return;
            }
            var listSpots = spots as List<ClassSpot>;
            if (listSpots == null)
            {
                return;
            }

            var ri = (ComboBox)LogicalTreeHelper.FindLogicalNode(this, "comboCity");
            if (ri == null)
            {
                return;
            }

            ri.SelectedValuePath = "Index";
            ri.DisplayMemberPath = "Name";
            ObservableCollection<ClassSpot> source = new ObservableCollection<ClassSpot>();
            foreach (ClassSpot spot in listSpots)
            {
                source.Add(spot);
            }
            ri.ItemsSource = source;
            //comboBox.FontSize = comboBox.FontSize + 5;
            ri.SelectedIndex = -1;


        }

        private void comboCity_SelectionChanged(object? sender, SelectionChangedEventArgs? e)
        {
            var selectedItem = (ClassSpot)this.comboCity.SelectedItem;
            if (selectedItem == null)
            {
                return;
            }
            var window = Application.Current.Properties["window"];
            if (window == null)
            {
                return;
            }
            var mainWindow = window as MainWindow;
            if (mainWindow == null)
            {
                return;
            }


            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.Name = StringName.stackPanelResidentVertical;
            foreach (var item in selectedItem.UnitGroup.Where(x => x.FlagDisplay == true))
            {
                StackPanel stackPanelUnit = new StackPanel();
                stackPanelUnit.Orientation = Orientation.Horizontal;
                stackPanelUnit.Name = StringName.stackPanelResidentHorizontal;
                //横方向
                foreach (var itemUnit in item.ListClassUnit.Select((value, index) => (value, index)))
                {
                    if (itemUnit.index == 0)
                    {
                        Button buttonSelect = new Button();
                        buttonSelect.Width = 48;
                        buttonSelect.Height = 48;
                        buttonSelect.Content = "⇒";
                        buttonSelect.FontSize += 10;
                        buttonSelect.Tag = item;
                        buttonSelect.Click += btnUnitsSelect_Click;
                        stackPanelUnit.Children.Add(buttonSelect);
                    }
                    Button button = new Button();
                    button.Width = 48;
                    button.Height = 48;
                    DisplayButtonNormal(button);

                    Image img = new Image();
                    img.Stretch = Stretch.Fill;

                    List<string> strings = new List<string>();
                    strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                    strings.Add("040_ChipImage");
                    strings.Add(itemUnit.value.Image);
                    string path = System.IO.Path.Combine(strings.ToArray());
                    BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                    img.Source = bitimg1;

                    button.HorizontalAlignment = HorizontalAlignment.Left;
                    button.VerticalAlignment = VerticalAlignment.Top;
                    button.Content = img;
                    stackPanelUnit.Children.Add(button);
                }
                stackPanel.Children.Add(stackPanelUnit);
            }
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Content = stackPanel;
            scrollViewer.Width = 390;
            scrollViewer.Height = 340;
            scrollViewer.Name = StringName.windowConscriptionMember;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            {
                var ri = (ScrollViewer)LogicalTreeHelper.FindLogicalNode(this.canvasListMember, StringName.windowConscriptionMember);
                if (ri != null)
                {
                    this.canvasListMember.Children.Remove(ri);
                }
            }

            this.canvasListMember.Children.Add(scrollViewer);

        }

        private static void DisplayButtonNormal(Button conv)
        {
            conv.BorderBrush = Brushes.Gray;
            conv.BorderThickness = new Thickness()
            {
                Left = 1,
                Top = 1,
                Right = 1,
                Bottom = 1
            };
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.Properties["window"];
            if (window == null)
            {
                return;
            }
            var mainWindow = window as MainWindow;
            if (mainWindow == null)
            {
                return;
            }
            var spots = Application.Current.Properties["spots"];
            if (spots == null)
            {
                return;
            }
            var listSpots = spots as List<ClassSpot>;
            if (listSpots == null)
            {
                return;
            }

            foreach (var item in listSpots)
            {
                foreach (var unit in item.UnitGroup)
                {
                    unit.FlagDisplay = true;
                }
            }

            mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Clear();

            var ri = (Frame)LogicalTreeHelper.FindLogicalNode(mainWindow, StringName.windowSortieMenu);
            if (ri == null)
            {
                return;
            }

            mainWindow.canvasMain.Children.Remove(ri);

        }

        private void btnUnitsSelect_Click(object sender, RoutedEventArgs e)
        {
            var convButton = sender as Button;
            if (convButton == null)
            {
                return;
            }
            var convTag = convButton.Tag as ClassHorizontalUnit;
            if (convTag == null)
            {
                return;
            }
            var window = Application.Current.Properties["window"];
            if (window == null)
            {
                return;
            }
            var mainWindow = window as MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            convTag.FlagDisplay = false;

            //再描写
            comboCity_SelectionChanged(null, null);

            //出撃クラスにunit追加
            mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Add(convTag);

            //右に描写
            DisplaySortieUnit(mainWindow);

        }

        private void DisplaySortieUnit(MainWindow mainWindow)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            foreach (var item in mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup)
            {
                StackPanel stackPanelUnit = new StackPanel();
                stackPanelUnit.Orientation = Orientation.Horizontal;
                //横方向
                foreach (var itemUnit in item.ListClassUnit.Select((value, index) => (value, index)))
                {
                    if (itemUnit.index == 0)
                    {
                        Button buttonSelect = new Button();
                        buttonSelect.Width = 48;
                        buttonSelect.Height = 48;
                        buttonSelect.Content = "←";
                        buttonSelect.FontSize += 10;
                        buttonSelect.Tag = item;
                        //大変ぽいので後回し
                        //buttonSelect.Click += btnUnitsSelect_Click;
                        stackPanelUnit.Children.Add(buttonSelect);
                    }
                    Button button = new Button();
                    button.Width = 48;
                    button.Height = 48;
                    DisplayButtonNormal(button);

                    Image img = new Image();
                    img.Stretch = Stretch.Fill;

                    List<string> strings = new List<string>();
                    strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                    strings.Add("040_ChipImage");
                    strings.Add(itemUnit.value.Image);
                    string path = System.IO.Path.Combine(strings.ToArray());
                    BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                    img.Source = bitimg1;

                    button.HorizontalAlignment = HorizontalAlignment.Left;
                    button.VerticalAlignment = VerticalAlignment.Top;
                    button.Content = img;
                    stackPanelUnit.Children.Add(button);
                }
                stackPanel.Children.Add(stackPanelUnit);
            }
            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Content = stackPanel;
            scrollViewer.Width = 390;
            scrollViewer.Height = 840;
            scrollViewer.Name = StringName.windowSortieMemberDecide;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

            {
                var ri = (ScrollViewer)LogicalTreeHelper.FindLogicalNode(this.canvasListMemberDecide, StringName.windowSortieMemberDecide);
                if (ri != null)
                {
                    this.canvasListMember.Children.Remove(ri);
                }
            }

            this.canvasListMemberDecide.Children.Add(scrollViewer);
        }

        private void btnAll_Click(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.Properties["window"];
            if (window == null)
            {
                return;
            }
            var mainWindow = window as MainWindow;
            if (mainWindow == null)
            {
                return;
            }
            var selectedItem = (ClassSpot)this.comboCity.SelectedItem;
            if (selectedItem == null)
            {
                return;
            }

            foreach (var item in selectedItem.UnitGroup.Where(x => x.FlagDisplay == true))
            {
                //出撃クラスにunit追加
                mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Add(item);

                item.FlagDisplay = false;
            }

            //再描写
            comboCity_SelectionChanged(null, null);

            DisplaySortieUnit(mainWindow);
        }

        private void btnSortie_Click(object sender, RoutedEventArgs e)
        {
            var window = Application.Current.Properties["window"];
            if (window == null)
            {
                return;
            }
            var mainWindow = window as MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var selectedItem = (ClassSpot)this.comboCity.SelectedItem;
            if (selectedItem == null)
            {
                return;
            }

            var spots = Application.Current.Properties["selectSpots"];
            if (spots == null)
            {
                return;
            }

            var convSpots = spots as ClassPowerAndCity;
            if (convSpots == null)
            {
                return;
            }

            //兵が存在する都市かチェック
            if (convSpots.ClassSpot.ListMember.Count == 0 && convSpots.ClassSpot.ListMonster.Count == 0)
            {
                var aa = mainWindow.ClassGameStatus.AllListSpot
                        .Where(x => x.NameTag == convSpots.ClassSpot.NameTag)
                        .First();

                //spotの所属情報を書き換え
                convSpots.ClassSpot.PowerNameTag = selectedItem.PowerNameTag;
                aa.PowerNameTag = convSpots.ClassSpot.PowerNameTag;
                var po = mainWindow.ClassGameStatus.ListPower
                            .Where(x => x.NameTag == selectedItem.PowerNameTag)
                            .First();
                po.ListMember.Add(convSpots.ClassSpot.NameTag);

                convSpots.ClassPower.ListMember.Add(selectedItem.NameTag);

                var ri = (Grid)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasMain, convSpots.ClassSpot.NameTag);
                if (ri == null)
                {
                    throw new Exception();
                }
                var flag = (Image)LogicalTreeHelper.FindLogicalNode(ri, "flag_img" + convSpots.ClassSpot.NameTag);
                if (flag == null)
                {
                    throw new Exception();
                }
                else
                {
                    mainWindow.canvasMain.Children.Remove(flag);

                    // 旗を表示する
                    Image flag_img = new Image();
                    flag_img.Name = convSpots.ClassSpot.NameTag;
                    flag_img = mainWindow.DisplayFlag(ri, 32, po.FlagPath);
                    ri.Children.Add(flag_img);
                }

                //unitの所属情報を書き換え
                foreach (var item in mainWindow.ClassGameStatus.AllListSpot.Where(x => x.NameTag == selectedItem.NameTag))
                {
                    foreach (var itemUnitGroup in item.UnitGroup)
                    {
                        itemUnitGroup.Spot = convSpots.ClassSpot;
                        itemUnitGroup.FlagDisplay = true;
                        //unit移動
                        aa.UnitGroup.Add(itemUnitGroup);
                    }
                    //これでは出撃してないユニットも全部消えてしまうので、後で対応、対応したらこのコメント消す
                    item.UnitGroup.Clear();
                }

                mainWindow.ClassGameStatus.ClassBattle.SortieUnitGroup.Clear();
                mainWindow.ClassGameStatus.ClassBattle.DefUnitGroup.Clear();
                mainWindow.ClassGameStatus.ClassBattle.NeutralUnitGroup.Clear();

                //出撃ウィンドウを消す
                {
                    var frame = (Frame)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasMain, StringName.windowSortieMenu);
                    if (frame != null)
                    {
                        mainWindow.canvasMain.Children.Remove(frame);
                    }
                }
                //message
                MessageBox.Show(convSpots.ClassSpot.Name + "を占領しました！");

                // 勢力メニューを更新する
                var uc5 = (UserControl005_StrategyMenu)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUIRightBottom, StringName.canvasStrategyMenu);
                if (uc5 != null)
                {
                    uc5.DisplayPowerStatus(mainWindow);
                }

                return;
            }

            var extractMap = mainWindow
                                .ClassGameStatus
                                .ListClassMapBattle
                                .Where(x => x.TagName == convSpots.ClassSpot.Map)
                                .FirstOrDefault();
            if (extractMap != null)
            {
                mainWindow.ClassGameStatus.ClassBattle.ClassMapBattle = extractMap;

                ClassStaticBattle.AddBuilding(mainWindow.ClassGameStatus);

            }

            //防衛ユニット設定
            foreach (var item in convSpots.ClassSpot.UnitGroup)
            {
                mainWindow.ClassGameStatus.ClassBattle.DefUnitGroup.Add(item);
            }

            //出兵元情報を格納
            Application.Current.Properties["selectedItem"] = selectedItem;

            mainWindow.FadeOut = true;

            mainWindow.delegateBattleMap = mainWindow.SetBattleMap;

            mainWindow.FadeIn = true;
        }

    }
}
