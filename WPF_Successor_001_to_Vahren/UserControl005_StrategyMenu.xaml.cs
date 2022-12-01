using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// UserControl005_StrategyMenu.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl005_StrategyMenu : UserControl
    {
        public UserControl005_StrategyMenu()
        {
            InitializeComponent();
        }

        public void SetData()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            DisplayPowerStatus(mainWindow);
        }

        public void DisplayPowerStatus(MainWindow mainWindow)
        {
            //旗
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("030_FlagImage");
                strings.Add(mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.FlagPath);
                string path = System.IO.Path.Combine(strings.ToArray());
                BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                Int32Rect rect = new Int32Rect(0, 0, 32, 32);
                var destimg = new CroppedBitmap(bitimg1, rect);
                this.imgFlag.Source = destimg;
            }
            //勢力名
            {
                this.txtNamePower.Text = mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.Name;
            }
            //顔グラ
            {
                var ima = mainWindow.ClassGameStatus.ListUnit
                    .Where(x => x.NameTag == mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.MasterTag)
                    .FirstOrDefault();
                if (ima != null)
                {
                    List<string> strings = new List<string>();
                    strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                    strings.Add("010_FaceImage");
                    strings.Add(ima.Face);
                    string path = System.IO.Path.Combine(strings.ToArray());
                    BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                    this.imgFace.Source = bitimg1;
                }
            }

            //軍資金
            {
                this.txtMoney.Text = mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.Money.ToString();
            }
            //収入補正
            {
                this.txtGainCorrection.Text = "";
            }
            //領地数
            {
                this.txtNumberSpot.Text = mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.ListMember.Count.ToString();
            }

            // 各領地のデータを集計する
            int total_gain = 0;
            int unit_count = 0;
            int total_cost = 0, total_finance = 0;
            string powerNameTag = mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.NameTag;
            var listSpot = mainWindow.ClassGameStatus.AllListSpot.Where(x => x.PowerNameTag == powerNameTag);
            foreach (var itemSpot in listSpot)
            {
                total_gain += itemSpot.Gain;
                foreach (var itemTroop in itemSpot.UnitGroup)
                {
                    unit_count += itemTroop.ListClassUnit.Count;
                    foreach (var itemUnit in itemTroop.ListClassUnit)
                    {
                        if (itemUnit.Cost > 0)
                        {
                            total_cost += itemUnit.Cost;
                        }
                        else if (itemUnit.Cost < 0)
                        {
                            // 維持費がマイナスなら財政値になる
                            total_finance -= itemUnit.Cost;
                        }
                        if (itemUnit.Finance > 0)
                        {
                            total_finance += itemUnit.Finance;
                        }
                    }
                }
            }

            //総収入
            {
                this.txtTotalGain.Text = total_gain.ToString();
            }
            //ユニット数
            {
                this.txtNumberUnit.Text = unit_count.ToString();
            }
            //維持費
            {
                this.txtTotalCost.Text = total_cost.ToString();
            }
            //財政値
            {
                this.txtTotalFinance.Text = total_finance.ToString();
            }
            //訓練値
            {
                this.txtNumberTraining.Text = "";
            }
            //影響力
            {
                this.txtInfluence.Text = "";
            }
        }

        // ターン数を表示する
        public void DisplayTurn(MainWindow mainWindow)
        {
            // 既に表示されてるか調べる
            bool isFound = false;
            foreach (var itemText in mainWindow.canvasUIRightTop.Children.OfType<TextBlock>())
            {
                if (itemText.Name == "Turn")
                {
                    // 既に表示されてる場合は、値を更新する
                    itemText.Text = "turn " + mainWindow.ClassGameStatus.NowTurn.ToString();
                    isFound = true;
                    break;
                }
            }

            // まだ表示されてなければ、新たに表示する
            if (isFound == false)
            {
                TextBlock textTurn = new TextBlock();
                textTurn.Name = "Turn";
                textTurn.FontSize = 30;
                textTurn.Foreground = Brushes.White;
                textTurn.Width = 150;
                textTurn.TextAlignment = TextAlignment.Center;
                textTurn.Text = "turn " + mainWindow.ClassGameStatus.NowTurn.ToString();
                // 影を付ける (右下45度方向に3ピクセル)
                textTurn.Effect =
                    new DropShadowEffect
                    {
                        Direction = 315,
                        ShadowDepth = 3,
                        Opacity = 1,
                        BlurRadius = 0
                    };
                mainWindow.canvasUIRightTop.Children.Add(textTurn);
                Canvas.SetLeft(textTurn, mainWindow.canvasUIRightTop.Width - 150);
            }
        }

        private void btnTurnEnd_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 開いてる子ウインドウを全て閉じる
            mainWindow.canvasUI.Children.Clear();

            //ターン終了時処理
            this.Visibility = Visibility.Hidden;

            //ターン加算
            // 本来は、全ての勢力行動が終わってからターン数を増やさないといけない。
            // 現在は AI勢力の思考部分ができてないので、プレイヤー操作が終わった時点で加算する。
            mainWindow.ClassGameStatus.NowTurn += 1;
            DisplayTurn(mainWindow);

            // AI呼び出し

            // AI呼び出し後に下を行う
            //ターン開始時処理
            mainWindow.ExecuteEvent();
            this.Visibility = Visibility.Visible;
        }
    }
}
