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
    /// Page001_Conscription.xaml の相互作用ロジック
    /// </summary>
    public partial class Page001_Conscription : Page
    {
        public Page001_Conscription()
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

            var classPowerAndCity = Application.Current.Properties["ClassPowerAndCity"];
            if (classPowerAndCity == null)
            {
                return;
            }
            var targetPowerAndCity = classPowerAndCity as ClassPowerAndCity;
            if (targetPowerAndCity == null)
            {
                return;
            }

            DisplayMember(mainWindow, targetPowerAndCity);

            this.lblCommonConscription.Content = "(基本)雇用";

            // 基本雇用
            {
                StackPanel sPCC = new StackPanel();
                sPCC.Orientation = Orientation.Vertical;
                foreach (var item in mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.ListCommonConscription)
                {
                    var ext = mainWindow.ClassGameStatus
                        .ListUnit
                        .Where(x => x.NameTag ==item)
                        .FirstOrDefault();

                    if (ext == null)
                    {
                        continue;
                    }

                    Canvas c = new Canvas();
                    c.HorizontalAlignment = HorizontalAlignment.Left;
                    c.VerticalAlignment = VerticalAlignment.Top;
                    c.Height = 32;
                    c.Width = 390;

                    //ボタン
                    {
                        Button button = new Button();
                        button.Width = 32;
                        button.Height = 32;
                        Image img = new Image();
                        img.Stretch = Stretch.Fill;

                        List<string> strings = new List<string>();
                        strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                        strings.Add("040_ChipImage");
                        strings.Add(ext.Image);
                        string path = System.IO.Path.Combine(strings.ToArray());
                        BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                        img.Source = bitimg1;

                        button.HorizontalAlignment = HorizontalAlignment.Left;
                        button.VerticalAlignment = VerticalAlignment.Top;
                        button.Content = img;
                        ClassCityAndUnit classCityAndUnit = new ClassCityAndUnit();
                        classCityAndUnit.ClassUnit = ext;
                        classCityAndUnit.ClassPowerAndCity = targetPowerAndCity;
                        button.Click += btnConscriptionUnit_Click;
                        button.PreviewMouseRightButtonUp += btnConscriptionUnit_RightButtonUp;
                        button.Tag = classCityAndUnit;
                        c.Children.Add(button);
                    }
                    //名前
                    {
                        Label lblName = new Label();
                        lblName.Width = 200;
                        lblName.Height = 35;
                        lblName.Content = ext.Name;
                        lblName.Foreground = Brushes.White;
                        lblName.FontSize = 20;
                        lblName.Margin = new Thickness()
                        {
                            Left = 32 + 5,
                            Top = -3
                        };
                        c.Children.Add(lblName);
                    }
                    //雇用費
                    {
                        Label lblPrice = new Label();
                        lblPrice.Width = 100;
                        lblPrice.Height = 35;
                        lblPrice.Content = ext.Price;
                        lblPrice.Foreground = Brushes.White;
                        lblPrice.FontSize = 20;
                        lblPrice.Margin = new Thickness()
                        {
                            Left = 32 + 5 + 200 + 5,
                            Top = -3
                        };
                        c.Children.Add(lblPrice);
                    }

                    sPCC.Children.Add(c);
                }
                ScrollViewer scrollViewer = new ScrollViewer();
                scrollViewer.Content = sPCC;
                scrollViewer.Width = 390;
                scrollViewer.Height = 340;
                this.canvasCommonConscription.Children.Add(scrollViewer);
            }

            this.borLeftInfo.Visibility = Visibility.Hidden;
        }

        private void DisplayMember(MainWindow mainWindow, ClassPowerAndCity targetPowerAndCity)
        {
            // 駐在部隊トップバー
            {
                var count = mainWindow.ClassGameStatus.AllListSpot
                    .Where(x => x.NameTag == targetPowerAndCity.ClassSpot.NameTag)
                    .First()
                    .UnitGroup
                    .Where(x => x.Spot.NameTag == targetPowerAndCity.ClassSpot.NameTag)
                    .Count();
                this.lblMemberCount.Content =
                    count.ToString() +
                    "/" +
                    mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].SpotCapacity;
            }
            // 駐在部隊
            {
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Vertical;
                stackPanel.Name = StringName.stackPanelResidentVertical;
                var tar = mainWindow.ClassGameStatus.AllListSpot
                    .Where(x => x.NameTag == targetPowerAndCity.ClassSpot.NameTag)
                    .First();
                foreach (var item in tar.UnitGroup.Where(x => x.Spot.NameTag == targetPowerAndCity.ClassSpot.NameTag))
                {
                    StackPanel stackPanelUnit = new StackPanel();
                    stackPanelUnit.Orientation = Orientation.Horizontal;
                    stackPanelUnit.Name = StringName.stackPanelResidentHorizontal;

                    //横方向
                    foreach (var itemUnit in item.ListClassUnit.Select((value, index) => (value, index)))
                    {
                        //陣形コンボボックス
                        if (itemUnit.index == 0)
                        {
                            ComboBox comboBox = new ComboBox();
                            comboBox.Height = 48;
                            comboBox.Width = 48;
                            comboBox.VerticalAlignment = VerticalAlignment.Center;
                            comboBox.SelectedValuePath = "Id";
                            comboBox.DisplayMemberPath = "Formation";
                            ObservableCollection<ClassFormation> formation = new ObservableCollection<ClassFormation>();
                            formation.Add(new ClassFormation() { Id = 0, Formation = _010_Enum.Formation.F });
                            formation.Add(new ClassFormation() { Id = 1, Formation = _010_Enum.Formation.M });
                            formation.Add(new ClassFormation() { Id = 2, Formation = _010_Enum.Formation.B });
                            comboBox.ItemsSource = formation;
                            comboBox.FontSize = comboBox.FontSize + 5;
                            comboBox.SelectedIndex = itemUnit.value.Formation.Id;
                            stackPanelUnit.Children.Add(comboBox);
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
                        button.Click += btnUnit_Click;
                        ClassCityAndUnit classCityAndUnit = new ClassCityAndUnit();
                        classCityAndUnit.ClassUnit = itemUnit.value;
                        classCityAndUnit.ClassPowerAndCity = targetPowerAndCity;
                        button.Tag = classCityAndUnit;
                        button.MouseEnter += unit_MouseEnter;

                        if (itemUnit.value.IsSelect == true)
                        {
                            DisplayButtonSelect(button);
                        }

                        StackPanel stackPanelLabel = new StackPanel();
                        stackPanelLabel.Width = 48;
                        stackPanelLabel.Height = 72;
                        stackPanelLabel.Orientation = Orientation.Vertical;

                        Label label = new Label();
                        label.Content = "Lv:" + itemUnit.value.Level;
                        label.Foreground = Brushes.White;
                        label.FontSize = 15;
                        label.HorizontalAlignment = HorizontalAlignment.Center;

                        stackPanelLabel.Children.Add(button);
                        stackPanelLabel.Children.Add(label);

                        stackPanelUnit.Children.Add(stackPanelLabel);
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

            var classPowerAndCity = Application.Current.Properties["ClassPowerAndCity"];
            if (classPowerAndCity == null)
            {
                return;
            }
            var targetPowerAndCity = classPowerAndCity as ClassPowerAndCity;
            if (targetPowerAndCity == null)
            {
                return;
            }

            var ri = (Frame)LogicalTreeHelper.FindLogicalNode(mainWindow, StringName.windowConscription);
            if (ri == null)
            {
                return;
            }

            DisposeSelectUnit(targetPowerAndCity);

            mainWindow.canvasMain.Children.Remove(ri);
        }

        private void btnUnit_Click(object sender, RoutedEventArgs e)
        {
            var convButton = sender as Button;
            if (convButton == null)
            {
                return;
            }
            var convTag = convButton.Tag as ClassCityAndUnit;
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

            //見た目を元に戻す
            {
                var ri = (ScrollViewer)LogicalTreeHelper.FindLogicalNode(this.canvasListMember, StringName.windowConscriptionMember);
                if (ri == null)
                {
                    return;
                }
                var verticalStack = ri.Content as StackPanel;
                if (verticalStack == null)
                {
                    return;
                }
                foreach (StackPanel itemH in verticalStack.Children)
                {
                    foreach (var itemStackPa in itemH.Children)
                    {
                        var sta = itemStackPa as StackPanel;
                        if (sta is not StackPanel) continue;
                        foreach (var item in sta.Children)
                        {
                            var conv = item as Button;
                            if (conv != null)
                            {
                                DisplayButtonNormal(conv);
                            }
                        }
                    }
                }
            }

            if (convTag.ClassUnit == null)
            {
                throw new Exception();
            }
            if (convTag.ClassUnit.IsSelect == true)
            {
                convTag.ClassUnit.IsSelect = false;
                return;
            }

            DisposeSelectUnit(convTag.ClassPowerAndCity);

            convTag.ClassUnit.IsSelect = true;

            DisplayButtonSelect(convButton);
        }

        private void btnConscriptionUnit_RightButtonUp(object sender, RoutedEventArgs e)
        {
            var convButton = sender as Button;
            if (convButton == null)
            {
                return;
            }
            var convTag = convButton.Tag as ClassCityAndUnit;
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

            //選択されているユニットがあるか？
            int selectUnitNumber = -1;
            selectUnitNumber = SearchgSelectUnit(mainWindow, selectUnitNumber);
            int count = 0;

            if (convTag.ClassUnit == null)
            {
                throw new Exception();
            }
            ClassUnit? extName = mainWindow.ClassGameStatus
                                    .ListUnit
                                    .Where(x => x.NameTag ==convTag.ClassUnit.NameTag)
                                    .FirstOrDefault();

            if (extName == null)
            {
                return;
            }

            if (selectUnitNumber == -1)
            {
                count = mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].MemberCapacity;

                var cloneExt = extName.DeepCopy();
                cloneExt.IsSelect = false;

                var lis = new List<ClassUnit>();
                lis.Add(cloneExt);
                convTag.ClassPowerAndCity.ClassSpot.UnitGroup.Add(new ClassHorizontalUnit() { FlagDisplay = true, ListClassUnit = lis });

                for (int i = 0; i < count - 1; i++)
                {
                    var cop = extName.DeepCopy();
                    cop.IsSelect = false;
                    mainWindow.ClassGameStatus.SelectionPowerAndCity
                            .ClassSpot
                            .UnitGroup[convTag.ClassPowerAndCity.ClassSpot.UnitGroup.Count - 1]
                            .ListClassUnit
                            .Add(cop);
                }
            }
            else
            {
                //その部隊での残り雇用数算出
                count = mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].MemberCapacity
                    - mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassSpot.UnitGroup[selectUnitNumber].ListClassUnit.Count;

                if (count <= 0)
                {
                    MessageBox.Show("選択されている部隊においては、もうこれ以上雇用出来ません");
                    return;
                }

                for (int i = 0; i < count; i++)
                {
                    var cop = extName.DeepCopy();
                    cop.IsSelect = false;
                    mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassSpot.UnitGroup[selectUnitNumber].ListClassUnit.Add(cop);
                }
            }

            DisplayMember(mainWindow, mainWindow.ClassGameStatus.SelectionPowerAndCity);
        }

        private static void DisposeSelectUnit(ClassPowerAndCity convTag)
        {
            //他の選択されているユニットを解除する
            foreach (var itemUnits in convTag.ClassSpot.UnitGroup)
            {
                foreach (var item in itemUnits.ListClassUnit)
                {
                    item.IsSelect = false;
                }
            }
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

        private static void DisplayButtonSelect(Button convButton)
        {
            convButton.BorderBrush = Brushes.Red;
            convButton.BorderThickness = new Thickness()
            {
                Left = 3,
                Top = 3,
                Right = 3,
                Bottom = 3
            };
        }

        private void btnConscriptionUnit_Click(object sender, RoutedEventArgs e)
        {
            var convButton = sender as Button;
            if (convButton == null)
            {
                return;
            }
            var convTag = convButton.Tag as ClassCityAndUnit;
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

            //金が足りなかったらダメ
            if (convTag.ClassUnit == null)
            {
                throw new Exception();
            }
            if (mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.Money - convTag.ClassUnit.Price < 0)
            {
                return;
            }

            //駐在数が多すぎたらダメ
            if (mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassSpot.UnitGroup.Count
                >= mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].SpotCapacity)
            {
                return;
            }

            mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.Money -= convTag.ClassUnit.Price;
            mainWindow.ClassGameStatus.WindowStrategyMenu.DisplayPowerStatus(mainWindow);

            ClassUnit? extName = mainWindow.ClassGameStatus
                        .ListUnit
                        .Where(x => x.NameTag == convTag.ClassUnit.NameTag)
                        .FirstOrDefault();

            if (extName == null)
            {
                return;
            }
            var cloneExt = extName.DeepCopy();
            cloneExt.ID = mainWindow.ClassGameStatus.IDCount;
            cloneExt.IsSelect = false;

            //選択されているユニットがあるか？
            int selectUnitNumber = -1;
            selectUnitNumber = SearchgSelectUnit(mainWindow, selectUnitNumber);

            if (selectUnitNumber != -1)
            {
                if (mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassSpot.UnitGroup[selectUnitNumber].ListClassUnit.Count
                    >= mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].MemberCapacity)
                {
                    MessageBox.Show("定員オーバーです");
                    return;
                }
                mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassSpot
                    .UnitGroup[selectUnitNumber]
                    .ListClassUnit
                    .Add(cloneExt);
            }
            else
            {
                //選択されているユニットが無ければ、新部隊長誕生
                var lis = new List<ClassUnit>();
                lis.Add(cloneExt);
                convTag.ClassPowerAndCity.ClassSpot.UnitGroup
                    .Add(
                    new ClassHorizontalUnit()
                    {
                        FlagDisplay = true,
                        ListClassUnit = lis
                    });
            }

            mainWindow.ClassGameStatus.SetIDCount();
            DisplayMember(mainWindow, mainWindow.ClassGameStatus.SelectionPowerAndCity);
        }

        private static int SearchgSelectUnit(MainWindow mainWindow, int selectUnitNumber)
        {
            bool flagSelect = false;
            foreach (var unitGroup in mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassSpot.UnitGroup.Select((value, index) => (value, index)))
            {
                foreach (var item in unitGroup.value.ListClassUnit)
                {
                    if (item.IsSelect == true)
                    {
                        selectUnitNumber = unitGroup.index;
                        flagSelect = true;
                        break;
                    }
                }
                if (flagSelect == true)
                {
                    break;
                }
            }

            return selectUnitNumber;
        }

        private void unit_MouseEnter(object sender, MouseEventArgs e)
        {
            var convButton = sender as Button;
            if (convButton == null)
            {
                return;
            }
            var convTag = convButton.Tag as ClassCityAndUnit;
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

            this.borLeftInfo.Visibility = Visibility.Visible;

            //名前
            {
                if (convTag.ClassUnit == null)
                {
                    throw new Exception();
                }

                this.lblNameTarget.Content = convTag.ClassUnit.Name;
            }

            //画像
            {
                if ((convTag.ClassUnit.Face == string.Empty || convTag.ClassUnit.Face == null) == false)
                {
                    List<string> strings = new List<string>();
                    strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                    strings.Add("010_FaceImage");
                    strings.Add(convTag.ClassUnit.Face);
                    string path = System.IO.Path.Combine(strings.ToArray());
                    BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                    this.imgFace.Source = bitimg1;
                }
                else
                {
                    this.imgFace.Source = null;
                }
            }

            //skill
            {
                this.spSkill.Children.Clear();

                foreach (var item in convTag.ClassUnit.Skill)
                {
                    var result = mainWindow.ClassGameStatus.ListSkill.Where(x => x.NameTag == item.NameTag).FirstOrDefault();
                    if (result == null)
                    {
                        continue;
                    }
                    Canvas canvas = new Canvas();
                    canvas.HorizontalAlignment = HorizontalAlignment.Left;
                    canvas.VerticalAlignment = VerticalAlignment.Top;
                    canvas.Margin = new Thickness(0, 0, 0, 0);
                    foreach (var itemIcon in Enumerable.Reverse(result.Icon).ToList())
                    {
                        List<string> strings = new List<string>();
                        strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                        strings.Add("041_ChipImageSkill");
                        strings.Add(itemIcon);
                        string path = System.IO.Path.Combine(strings.ToArray());

                        var bi = new BitmapImage(new Uri(path));
                        Image image = new Image();
                        image.Stretch = Stretch.Fill;
                        image.Source = bi;
                        image.Margin = new Thickness(0, 0, 0, 0);
                        image.Height = 32;
                        image.Width = 32;
                        image.HorizontalAlignment = HorizontalAlignment.Left;
                        image.VerticalAlignment = VerticalAlignment.Top;
                        canvas.Children.Add(image);
                    }

                    Button button = new Button();
                    button.Content = canvas;
                    button.HorizontalAlignment = HorizontalAlignment.Left;
                    button.VerticalAlignment = VerticalAlignment.Top;
                    button.Width = 32;
                    button.Height = 32;
                    button.HorizontalContentAlignment = HorizontalAlignment.Left;
                    button.VerticalContentAlignment = VerticalAlignment.Top;
                    this.spSkill.Children.Add(button);
                }
            }

            //status
            {
                this.lblMoveType.Content = convTag.ClassUnit.MoveType;
                this.lblHP.Content = convTag.ClassUnit.Hp;
                this.lblMP.Content = convTag.ClassUnit.Mp;
                this.lblAttack.Content = convTag.ClassUnit.Attack;
                this.lblDef.Content = convTag.ClassUnit.Defense;
                this.lblMagic.Content = convTag.ClassUnit.Magic;
                this.lblMagicDef.Content = convTag.ClassUnit.MagDef;
                this.lblSpeed.Content = convTag.ClassUnit.Speed;
                //Dext = 技術 ?
                this.lblTech.Content = convTag.ClassUnit.Dext;
                //hprecではないか？
                this.lblHealHP.Content = convTag.ClassUnit.Heal_max;
                //mprecではないか？
                this.lblHealMP.Content = "";
                this.lblMove.Content = convTag.ClassUnit.Move;
                this.lblSummon.Content = convTag.ClassUnit.Summon_max;
                this.lblFinance.Content = convTag.ClassUnit.Finance;
            }
        }
    }
}
