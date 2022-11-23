﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
                this.lblNamePower.Content = mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.Name;
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
                this.lblNameMoney.Content = mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.Money;
            }
            //総収入
            {
                string select_NameTag = mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.NameTag;
                int gain_sum = 0;
                var list_spot = mainWindow.ClassGameStatus.AllListSpot.Where(x => x.PowerNameTag == select_NameTag);
                foreach (var item_spot in list_spot)
                {
                    gain_sum += item_spot.Gain;
                }
                this.lblNameTotalGain.Content = gain_sum;
            }
            //収入補正
            {
                this.lblNameGainCorrection.Content = "";
            }
            //領地数
            {
                this.lblNameNumberSpot.Content = mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.ListMember.Count;
            }
            //ユニット数
            {
                string select_NameTag = mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.NameTag;
                int unit_count = 0;
                var list_spot = mainWindow.ClassGameStatus.AllListSpot.Where(x => x.PowerNameTag == select_NameTag);
                foreach (var item_spot in list_spot)
                {
                    foreach (var item_group in item_spot.UnitGroup)
                    {
                        unit_count += item_group.ListClassUnit.Count;
                    }
                }
                this.lblNameNumberUnit.Content = unit_count;
            }
            //維持費
            {
                this.lblNameMaintenanceCosts.Content = "";
            }
            //財政値
            {
                this.lblNameNumberFinance.Content = "";
            }
            //訓練値
            {
                this.lblNameNumberTraining.Content = "";
            }
            //影響力
            {
                this.lblNameInfluence.Content = "";
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

            // AI呼び出し

            // AI呼び出し後に下を行う
            //ターン開始時処理
            mainWindow.ExecuteEvent();
            this.Visibility = Visibility.Visible;
        }
    }
}
