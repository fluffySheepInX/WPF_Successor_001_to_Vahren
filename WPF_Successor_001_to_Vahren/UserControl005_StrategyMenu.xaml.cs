using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WPF_Successor_001_to_Vahren._005_Class;
using WPF_Successor_001_to_Vahren._006_ClassStatic;

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

            // タイマーの初期化
            _timerTurn.Interval = TimeSpan.FromSeconds(1); // 実験用に待機時間を長くする。本来は短くする
        }

        // 最初に呼び出した時
        public void SetData()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // ウインドウ枠
            SetWindowFrame(mainWindow);

            DisplayPowerStatus(mainWindow);
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

        // 表示を更新する時
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
                var ima = mainWindow.ClassGameStatus.NowListUnit
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
            //領地数
            {
                this.txtNumberSpot.Text = mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.ListMember.Count.ToString();
            }

            // 各領地のデータを集計する
            int total_gain = 0;
            int unit_count = 0;
            int total_cost = 0, total_finance = 0;
            int talent_count = 0;
            int total_level = 0;
            string powerNameTag = mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.NameTag;
            var listSpot = mainWindow.ClassGameStatus.NowListSpot.Where(x => x.PowerNameTag == powerNameTag);
            foreach (var itemSpot in listSpot)
            {
                total_gain += itemSpot.Gain;
                foreach (var itemTroop in itemSpot.UnitGroup)
                {
                    unit_count += itemTroop.ListClassUnit.Count;
                    foreach (var itemUnit in itemTroop.ListClassUnit)
                    {
                        // 維持費と財政値
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

                        // 人材のレベルを合計する
                        if (itemUnit.Talent == "on")
                        {
                            total_level += itemUnit.Level;
                            talent_count++;
                        }
                    }
                }
            }

            //総収入
            {
                total_gain *= (int)(mainWindow.ClassGameStatus.ClassContext.GainPer * 0.01);
                this.txtTotalGain.Text = "+" + total_gain.ToString();
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
            //訓練限界
            if (talent_count > 0)
            {
                // 勢力の訓練限界値の人材平均レベルパーセンテージが設定されてる場合は、平均値に掛けること。
                // 今はまだ設定が実装されてないので、平均値のままにしておく。
                int average_level = total_level / talent_count;
                this.txtTrainingAverage.Text = average_level.ToString();
            }
            else
            {
                this.txtTrainingAverage.Text = "0";
            }
            //訓練上昇
            {
                // 勢力の訓練上昇値（１ターンの訓練でレベルアップする数量）が設定されてる場合は、標準値と違う。
                // 今はまだ設定が実装されてないので、標準値のままにしておく。
                this.txtTrainingUp.Text = "2";
            }
            //兵レベル
            {
                this.txtBaseLevel.Text = "+?";
            }

            // 人材プレイだと項目が変わる。
            // 将来的には、内政用の項目を追加きるようにする。下は例
            //収入補正
            {
                this.txtGainAdjust.Text = "?";
            }
            //影響力
            {
                this.txtInfluence.Text = "?";
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


        // ターン経過処理用（アクセスしやすいよう MainWindow の public にしてもいいかも？）
        private DispatcherTimer _timerTurn = new DispatcherTimer();
        private ClassPower? _nowPower = null; // 現在思考中の勢力

        // 経過表示用
        private void ShowProgress(MainWindow mainWindow, string strStatus)
        {
            // 勢力のヒントを表示する（既に表示されてる場合は更新する）
            bool boolFound = false;
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl042_PowerHint>())
            {
                if (itemWindow.Name == StringName.windowPowerHint)
                {
                    if (_nowPower != null)
                    {
                        itemWindow.SetPower(_nowPower, false);
                    }
                    itemWindow.SetPos();
                    itemWindow.SetText(strStatus);
                    boolFound = true;
                    break;
                }
            }
            if (boolFound == false)
            {
                var itemWindow = new UserControl042_PowerHint();
                itemWindow.Name = StringName.windowPowerHint;
                if (_nowPower != null)
                {
                    itemWindow.SetPower(_nowPower, false);
                }
                itemWindow.SetPos();
                itemWindow.SetText(strStatus);
                mainWindow.canvasUI.Children.Add(itemWindow);
            }
        }
        private void HideProgress(MainWindow mainWindow)
        {
            // 勢力のヒントを閉じる
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl042_PowerHint>())
            {
                if (itemWindow.Name == StringName.windowPowerHint)
                {
                    mainWindow.canvasUI.Children.Remove(itemWindow);
                    break;
                }
            }
        }

        // AI思考を開始する勢力を選ぶ
        private void turn_SelectAI(object? sender, EventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }
            // ゲーム画面が最前面かどうか調べて、背面になってれば何もせずに終わる
            // バックグラウンドでも動くようにしたい場合は、続行すればいい。
            if (mainWindow.IsActive == false)
            {
                return;
            }

            // 連続実行されないよう、自身を取り除いて、タイマーを止める
            _timerTurn.Tick -= new EventHandler(turn_SelectAI);
            _timerTurn.Stop();

            // AI思考を開始する勢力を選ぶ
            foreach (var itemPower in mainWindow.ClassGameStatus.NowListPower)
            {
                // 既に思考した勢力が来たら、その次を選ぶ
                if (_nowPower == itemPower)
                {
                    _nowPower = null; // 空にしておけば、次のループで選ばれるはず
                    continue;
                }

                // プレイヤー操作勢力は除外する
                if (itemPower == mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower)
                {
                    continue;
                }
                // 将来的には、滅亡したCOM勢力も除外すること

                // 思考中の勢力が空なら、最初に見つけた勢力にする
                if (_nowPower == null)
                {
                    _nowPower = itemPower;
                    break;
                }
            }

            // 未行動の勢力が見つかれば
            if (_nowPower != null)
            {
                //MessageBox.Show(_nowPower.Name + "のAI思考中です。");
                ShowProgress(mainWindow, "思考中・・・");

                // AI思考を開始する
                _timerTurn.Tick += new EventHandler(turn_StartAI);
                _timerTurn.Start();
            }
            // 全ての勢力が行動終了したら
            else
            {
                //MessageBox.Show("全ての勢力の手順が終了しました。");
                HideProgress(mainWindow);

                // 次はターン開始処理にする
                _timerTurn.Tick += new EventHandler(turn_StartTurn);
                _timerTurn.Start();
            }
        }

        // AI思考を開始する
        private void turn_StartAI(object? sender, EventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }
            // ゲーム画面が最前面かどうか調べて、背面になってれば何もせずに終わる
            // バックグラウンドでも動くようにしたい場合は、続行すればいい。
            if (mainWindow.IsActive == false)
            {
                return;
            }

            // 連続実行されないよう、自身を取り除いて、タイマーを止める
            _timerTurn.Tick -= new EventHandler(turn_StartAI);
            _timerTurn.Stop();

            if (_nowPower == null)
            {
                ShowProgress(mainWindow, "手順の終了処理・・・");
                _timerTurn.Tick += new EventHandler(turn_FinishAI);
                _timerTurn.Start();
                return;
            }

            // AI思考を実行する
            bool boolBattle = ClassStaticStraregyAI.ThinkingEasy(mainWindow.ClassGameStatus, _nowPower, mainWindow);
            if (boolBattle)
            {
                ShowProgress(mainWindow, "戦闘開始・・・");
                _timerTurn.Tick += new EventHandler(turn_BattleAI);
                _timerTurn.Start();
            }
            else
            {
                // 戦闘しないなら、終了処理を行う
                ShowProgress(mainWindow, "手順の終了処理・・・");
                _timerTurn.Tick += new EventHandler(turn_FinishAI);
                _timerTurn.Start();
            }
        }

        // AI戦闘を開始する
        private void turn_BattleAI(object? sender, EventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }
            // ゲーム画面が最前面かどうか調べて、背面になってれば何もせずに終わる
            // バックグラウンドでも動くようにしたい場合は、続行すればいい。
            if (mainWindow.IsActive == false)
            {
                return;
            }

            // 連続実行されないよう、自身を取り除く
            _timerTurn.Tick -= new EventHandler(turn_BattleAI);
            // 戦闘は非同期処理なので、タイマーを動かしたまま、経過表示を消す
            HideProgress(mainWindow);

            // COM勢力の戦闘開始
            ClassStaticStraregyAI.StartBattle(mainWindow);

            // 戦闘が終了するのを待つ
            _timerTurn.Tick += new EventHandler(turn_WaitAI);
        }

        // 戦闘が終わるのを待つ
        private void turn_WaitAI(object? sender, EventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }
            // ゲーム画面が最前面かどうか調べて、背面になってれば何もせずに終わる
            // バックグラウンドでも動くようにしたい場合は、続行すればいい。
            if (mainWindow.IsActive == false)
            {
                return;
            }

            // 戦闘中なら何もしない
            if (mainWindow.IsBattle)
            {
                return;
            }

            // 戦闘してなければ、終了処理を行う
            _timerTurn.Tick -= new EventHandler(turn_WaitAI);

            // 戦闘終了後に、徴兵など次ターンの準備する？
            // その場合は、もう一つステップを増やすこと
            ShowProgress(mainWindow, "手順の終了処理・・・");
            _timerTurn.Tick += new EventHandler(turn_FinishAI);
            _timerTurn.Start();
        }

        // AI思考を終える
        private void turn_FinishAI(object? sender, EventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }
            // ゲーム画面が最前面かどうか調べて、背面になってれば何もせずに終わる
            // バックグラウンドでも動くようにしたい場合は、続行すればいい。
            if (mainWindow.IsActive == false)
            {
                return;
            }

            // 連続実行されないよう、自身を取り除いて、タイマーを止める
            _timerTurn.Tick -= new EventHandler(turn_FinishAI);
            _timerTurn.Stop();

            if (_nowPower == null)
            {
                _timerTurn.Tick += new EventHandler(turn_SelectAI);
                _timerTurn.Start();
                return;
            }

            // COM勢力のターン終了処理（資金の増減や終了時イベントなど）
            ClassStaticStraregyAI.ThinkingEnd(mainWindow.ClassGameStatus, _nowPower, mainWindow);

            // 次の勢力へ移る
            _timerTurn.Tick += new EventHandler(turn_SelectAI);
            _timerTurn.Start();
        }

        // 全体のターン開始時の処理（訓練して、未行動にして、イベントを実行する）
        private void turn_StartTurn(object? sender, EventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }
            // ゲーム画面が最前面かどうか調べて、背面になってれば何もせずに終わる
            // バックグラウンドでも動くようにしたい場合は、続行すればいい。
            if (mainWindow.IsActive == false)
            {
                return;
            }

            // 連続実行されないよう、自身を取り除いて、タイマーを止める
            _timerTurn.Tick -= new EventHandler(turn_StartTurn);
            _timerTurn.Stop();

            // ターン開始時に全ての勢力を同時に訓練する方がいいかも？
            // ヴァーレントゥーガは勢力ごとの手順が終わる際に訓練上昇するので、
            // 後手が訓練前に攻め込む際に、先手は訓練が終わってるから、先手が有利になる。
            foreach (var itemPower in mainWindow.ClassGameStatus.NowListPower)
            {
                // まずは勢力の人材の平均レベルを計算する
                int total_level = 0, average_level = 0;
                int talent_count = 0;
                string powerNameTag = itemPower.NameTag;
                var listSpot = mainWindow.ClassGameStatus.NowListSpot.Where(x => x.PowerNameTag == powerNameTag);
                foreach (var itemSpot in listSpot)
                {
                    // その領地の部隊
                    foreach (var itemTroop in itemSpot.UnitGroup)
                    {
                        // その部隊のユニット
                        foreach (var itemUnit in itemTroop.ListClassUnit)
                        {
                            // 人材のレベルを合計する
                            if (itemUnit.Talent == "on")
                            {
                                talent_count++;
                                total_level += itemUnit.Level;
                            }
                        }
                    }
                }
                if (talent_count > 0)
                {
                    average_level = total_level / talent_count;
                }

                // 行動済みでない一般兵のレベルを上昇させる
                foreach (var itemSpot in listSpot)
                {
                    // その領地の部隊
                    foreach (var itemTroop in itemSpot.UnitGroup)
                    {
                        // リーダースキルで上昇量が多いことを考慮して、隊長のスキルを調べる？
                        // 今はまだ訓練スキルが無いので、そのままにする。

                        // その部隊のユニット
                        foreach (var itemUnit in itemTroop.ListClassUnit)
                        {
                            if (itemUnit.IsDone == false)
                            {
                                // 未行動の一般兵で、レベルが低いなら
                                if ((itemUnit.Talent != "on") && (itemUnit.Level < average_level))
                                {
                                    itemUnit.Level += 2; // 標準で 2レベル上昇させる
                                }
                            }
                            else
                            {
                                itemUnit.IsDone = false; // 未行動に戻す
                            }
                        }
                    }
                }
            }

            /*
            訓練でレベルを上昇させる時に未行動にすればいいかも？

            // ターン開始時のイベント前に残存勢力の全てのユニットを未行動に戻す
            // 中立領地を除外するのに、勢力が設定されてない領地を抜き出せばいい？
            var listSpot = mainWindow.ClassGameStatus.NowListSpot.Where(x => x.PowerNameTag != string.Empty);
            foreach (var itemSpot in listSpot)
            {
                // その領地の部隊
                foreach (var itemTroop in itemSpot.UnitGroup)
                {
                    // その部隊のユニット
                    foreach (var itemUnit in itemTroop.ListClassUnit)
                    {
                        if (itemUnit.IsDone)
                        {
                            itemUnit.IsDone = false;
                        }
                    }
                }
            }
            */

            ////ターン開始時処理
            //ターン加算
            // 本来は、全ての勢力行動が終わってからターン数を増やさないといけない。
            // 現在は AI勢力の思考部分ができてないので、プレイヤー操作が終わった時点で加算する。
            mainWindow.ClassGameStatus.NowTurn += 1;
            DisplayTurn(mainWindow);
            //イベント実行
            mainWindow.ExecuteEvent();
            //メニュー表示
            this.Visibility = Visibility.Visible;

            // プレイヤーがワールドマップを操作できるようにする
            var worldMap = mainWindow.ClassGameStatus.WorldMap;
            if (worldMap != null)
            {
                worldMap.canvasMap.IsHitTestVisible = true;
            }
        }


        // ターン終了ボタンをクリックした時
        private void btnTurnEnd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Win025_Select();
            dialog.SetText("プレイヤーのターンを終了します。");
            bool? result = dialog.ShowDialog();
            if (result == false)
            {
                return;
            }

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 開いてる子ウインドウを全て閉じる
            mainWindow.canvasUI.Children.Clear();

            ////ターン終了時処理
            //メニュー隠す
            this.Visibility = Visibility.Hidden;

            // プレイヤーがワールドマップを操作できないようにする
            // 他のウインドウは操作できるので、イベントには応答できる。ゲーム終了もできる。
            var worldMap = mainWindow.ClassGameStatus.WorldMap;
            if (worldMap != null)
            {
                worldMap.canvasMap.IsHitTestVisible = false;
            }

            //お金増やす
            {
                var listSpotMoney = mainWindow.ClassGameStatus.NowListSpot
                                .Where(x => x.PowerNameTag == mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.NameTag);
                int countMoney = 0;
                int addMoney = 0; // 維持費と財政値による増減
                foreach (var itemSpot in listSpotMoney)
                {
                    countMoney += itemSpot.Gain;
                    foreach (var itemTroop in itemSpot.UnitGroup)
                    {
                        foreach (var itemUnit in itemTroop.ListClassUnit)
                        {
                            // 維持費の分だけ減らして、財政値の分だけ増やす
                            addMoney -= itemUnit.Cost;
                            addMoney += itemUnit.Finance;
                        }
                    }
                }
                countMoney *= (int)(mainWindow.ClassGameStatus.ClassContext.GainPer * 0.01);
                countMoney += addMoney;

                mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.Money += countMoney;
                this.txtMoney.Text = mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.Money.ToString();
            }

            // AI呼び出し
            if (mainWindow.ClassGameStatus.ClassContext.enemyTurnSkip == false)
            {
                _nowPower = null; // 思考中の勢力を消す
                _timerTurn.Tick += new EventHandler(turn_SelectAI);
            }
            // AI思考をスキップするなら、ターン開始処理に繋ぐ
            else
            {
                _timerTurn.Tick += new EventHandler(turn_StartTurn);
            }
            _timerTurn.Start();
        }

        // ターン終了ボタンにカーソルを乗せた時
        private void btnTurnEnd_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // カーソルを離した時のイベントを追加する
            var cast = (UIElement)sender;
            cast.MouseLeave += btnTurnEnd_MouseLeave;

            // 他のヘルプを全て隠す
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Visible) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    itemHelp.Visibility = Visibility.Hidden;
                }
            }

            // ヘルプを作成する
            var helpWindow = new UserControl030_Help();
            helpWindow.Name = "Help_StrategyMenu_btnTurnEnd";
            helpWindow.SetText("ターンを終了します。");
            mainWindow.canvasUI.Children.Add(helpWindow);
        }
        private void btnTurnEnd_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // イベントを取り除く
            var cast = (UIElement)sender;
            cast.MouseLeave -= btnTurnEnd_MouseLeave;

            // 表示中のヘルプを取り除く
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if (itemHelp.Name == "Help_StrategyMenu_btnTurnEnd")
                {
                    mainWindow.canvasUI.Children.Remove(itemHelp);
                    break;
                }
            }

            // 他のヘルプを隠してた場合は、最前面のヘルプだけ表示する
            int maxZ = -1, thisZ;
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    thisZ = Canvas.GetZIndex(itemHelp);
                    if (maxZ < thisZ)
                    {
                        maxZ = thisZ;
                    }
                }
            }
            if (maxZ >= 0)
            {
                foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
                {
                    if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                    {
                        if (Canvas.GetZIndex(itemHelp) == maxZ)
                        {
                            itemHelp.Visibility = Visibility.Visible;
                            break;
                        }
                    }
                }
            }
        }


        // 戦略メニューにカーソルを乗せた時
        private void win_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // カーソルを離した時のイベントを追加する
            var cast = (UIElement)sender;
            cast.MouseLeave += win_MouseLeave;

            // 他のヘルプを全て隠す
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Visible) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    itemHelp.Visibility = Visibility.Hidden;
                }
            }

            // ヘルプを作成する
            var helpWindow = new UserControl030_Help();
            helpWindow.Name = "Help_StrategyMenu";
            helpWindow.SetText("※敵領地を右クリックすると出撃ウィンドウが出ます。\n（戦争する時は敵領地を右クリックしてください）");
            mainWindow.canvasUI.Children.Add(helpWindow);
        }
        private void win_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // イベントを取り除く
            var cast = (UIElement)sender;
            cast.MouseLeave -= win_MouseLeave;

            // 表示中のヘルプを取り除く
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if (itemHelp.Name == "Help_StrategyMenu")
                {
                    mainWindow.canvasUI.Children.Remove(itemHelp);
                    break;
                }
            }

            // 他のヘルプを隠してた場合は、最前面のヘルプだけ表示する
            int maxZ = -1, thisZ;
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    thisZ = Canvas.GetZIndex(itemHelp);
                    if (maxZ < thisZ)
                    {
                        maxZ = thisZ;
                    }
                }
            }
            if (maxZ >= 0)
            {
                foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
                {
                    if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                    {
                        if (Canvas.GetZIndex(itemHelp) == maxZ)
                        {
                            itemHelp.Visibility = Visibility.Visible;
                            break;
                        }
                    }
                }
            }
        }

        // 維持費にカーソルを乗せた時
        private void TotalCost_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // カーソルを離した時のイベントを追加する
            var cast = (UIElement)sender;
            cast.MouseLeave += TotalCost_MouseLeave;

            // 他のヘルプを全て隠す
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Visible) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    itemHelp.Visibility = Visibility.Hidden;
                }
            }

            // ヘルプを作成する
            var helpWindow = new UserControl030_Help();
            helpWindow.Name = "Help_StrategyMenu_TotalCost";
            helpWindow.SetText("全ユニットの維持費の合計値です。");
            mainWindow.canvasUI.Children.Add(helpWindow);
        }
        private void TotalCost_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // イベントを取り除く
            var cast = (UIElement)sender;
            cast.MouseLeave -= TotalCost_MouseLeave;

            // 表示中のヘルプを取り除く
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if (itemHelp.Name == "Help_StrategyMenu_TotalCost")
                {
                    mainWindow.canvasUI.Children.Remove(itemHelp);
                    break;
                }
            }

            // 他のヘルプを隠してた場合は、最前面のヘルプだけ表示する
            int maxZ = -1, thisZ;
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    thisZ = Canvas.GetZIndex(itemHelp);
                    if (maxZ < thisZ)
                    {
                        maxZ = thisZ;
                    }
                }
            }
            if (maxZ >= 0)
            {
                foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
                {
                    if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                    {
                        if (Canvas.GetZIndex(itemHelp) == maxZ)
                        {
                            itemHelp.Visibility = Visibility.Visible;
                            break;
                        }
                    }
                }
            }
        }

        // 財政値にカーソルを乗せた時
        private void TotalFinance_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // カーソルを離した時のイベントを追加する
            var cast = (UIElement)sender;
            cast.MouseLeave += TotalFinance_MouseLeave;

            // 他のヘルプを全て隠す
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Visible) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    itemHelp.Visibility = Visibility.Hidden;
                }
            }

            // ヘルプを作成する
            var helpWindow = new UserControl030_Help();
            helpWindow.Name = "Help_StrategyMenu_TotalFinance";
            helpWindow.SetText("全ユニットの財政力の合計値です。");
            mainWindow.canvasUI.Children.Add(helpWindow);
        }
        private void TotalFinance_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // イベントを取り除く
            var cast = (UIElement)sender;
            cast.MouseLeave -= TotalFinance_MouseLeave;

            // 表示中のヘルプを取り除く
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if (itemHelp.Name == "Help_StrategyMenu_TotalFinance")
                {
                    mainWindow.canvasUI.Children.Remove(itemHelp);
                    break;
                }
            }

            // 他のヘルプを隠してた場合は、最前面のヘルプだけ表示する
            int maxZ = -1, thisZ;
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    thisZ = Canvas.GetZIndex(itemHelp);
                    if (maxZ < thisZ)
                    {
                        maxZ = thisZ;
                    }
                }
            }
            if (maxZ >= 0)
            {
                foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
                {
                    if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                    {
                        if (Canvas.GetZIndex(itemHelp) == maxZ)
                        {
                            itemHelp.Visibility = Visibility.Visible;
                            break;
                        }
                    }
                }
            }
        }

        // 訓練限界にカーソルを乗せた時
        private void TrainingAverage_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // カーソルを離した時のイベントを追加する
            var cast = (UIElement)sender;
            cast.MouseLeave += TrainingAverage_MouseLeave;

            // 他のヘルプを全て隠す
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Visible) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    itemHelp.Visibility = Visibility.Hidden;
                }
            }

            // ヘルプを作成する
            var helpWindow = new UserControl030_Help();
            helpWindow.Name = "Help_StrategyMenu_TrainingAverage";
            helpWindow.SetText("訓練限界値は勢力の全人材ユニットの平均レベルです。\nこれより低い一般兵のレベルが訓練で上昇します。");
            mainWindow.canvasUI.Children.Add(helpWindow);
        }
        private void TrainingAverage_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // イベントを取り除く
            var cast = (UIElement)sender;
            cast.MouseLeave -= TrainingAverage_MouseLeave;

            // 表示中のヘルプを取り除く
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if (itemHelp.Name == "Help_StrategyMenu_TrainingAverage")
                {
                    mainWindow.canvasUI.Children.Remove(itemHelp);
                    break;
                }
            }

            // 他のヘルプを隠してた場合は、最前面のヘルプだけ表示する
            int maxZ = -1, thisZ;
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    thisZ = Canvas.GetZIndex(itemHelp);
                    if (maxZ < thisZ)
                    {
                        maxZ = thisZ;
                    }
                }
            }
            if (maxZ >= 0)
            {
                foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
                {
                    if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                    {
                        if (Canvas.GetZIndex(itemHelp) == maxZ)
                        {
                            itemHelp.Visibility = Visibility.Visible;
                            break;
                        }
                    }
                }
            }
        }

        // 訓練上昇にカーソルを乗せた時
        private void TrainingUp_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // カーソルを離した時のイベントを追加する
            var cast = (UIElement)sender;
            cast.MouseLeave += TrainingUp_MouseLeave;

            // 他のヘルプを全て隠す
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Visible) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    itemHelp.Visibility = Visibility.Hidden;
                }
            }

            // ヘルプを作成する
            var helpWindow = new UserControl030_Help();
            helpWindow.Name = "Help_StrategyMenu_TrainingUp";
            helpWindow.SetText("訓練によるターン毎のレベル上昇値です。");
            mainWindow.canvasUI.Children.Add(helpWindow);
        }
        private void TrainingUp_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // イベントを取り除く
            var cast = (UIElement)sender;
            cast.MouseLeave -= TrainingUp_MouseLeave;

            // 表示中のヘルプを取り除く
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if (itemHelp.Name == "Help_StrategyMenu_TrainingUp")
                {
                    mainWindow.canvasUI.Children.Remove(itemHelp);
                    break;
                }
            }

            // 他のヘルプを隠してた場合は、最前面のヘルプだけ表示する
            int maxZ = -1, thisZ;
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    thisZ = Canvas.GetZIndex(itemHelp);
                    if (maxZ < thisZ)
                    {
                        maxZ = thisZ;
                    }
                }
            }
            if (maxZ >= 0)
            {
                foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
                {
                    if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                    {
                        if (Canvas.GetZIndex(itemHelp) == maxZ)
                        {
                            itemHelp.Visibility = Visibility.Visible;
                            break;
                        }
                    }
                }
            }
        }

        // 兵レベルにカーソルを乗せた時
        private void BaseLevel_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // カーソルを離した時のイベントを追加する
            var cast = (UIElement)sender;
            cast.MouseLeave += BaseLevel_MouseLeave;

            // 他のヘルプを全て隠す
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Visible) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    itemHelp.Visibility = Visibility.Hidden;
                }
            }

            // ヘルプを作成する
            var helpWindow = new UserControl030_Help();
            helpWindow.Name = "Help_StrategyMenu_BaseLevel";
            helpWindow.SetText("雇用兵士の底上げレベルです。");
            mainWindow.canvasUI.Children.Add(helpWindow);
        }
        private void BaseLevel_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // イベントを取り除く
            var cast = (UIElement)sender;
            cast.MouseLeave -= BaseLevel_MouseLeave;

            // 表示中のヘルプを取り除く
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if (itemHelp.Name == "Help_StrategyMenu_BaseLevel")
                {
                    mainWindow.canvasUI.Children.Remove(itemHelp);
                    break;
                }
            }

            // 他のヘルプを隠してた場合は、最前面のヘルプだけ表示する
            int maxZ = -1, thisZ;
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    thisZ = Canvas.GetZIndex(itemHelp);
                    if (maxZ < thisZ)
                    {
                        maxZ = thisZ;
                    }
                }
            }
            if (maxZ >= 0)
            {
                foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
                {
                    if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                    {
                        if (Canvas.GetZIndex(itemHelp) == maxZ)
                        {
                            itemHelp.Visibility = Visibility.Visible;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 軍資金にカーソルを乗せた時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtMilitaryFunds_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // カーソルを離した時のイベントを追加する
            var cast = (UIElement)sender;
            cast.MouseLeave += txtMilitaryFunds_MouseLeave;

            // 他のヘルプを全て隠す
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Visible) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    itemHelp.Visibility = Visibility.Hidden;
                }
            }

            // ヘルプを作成する
            var helpWindow = new UserControl030_Help();
            helpWindow.Name = "Help_StrategyMenu_MilitaryFunds";
            helpWindow.SetText("軍資金の増減 = 総収入 - 維持費 + 財政値");
            mainWindow.canvasUI.Children.Add(helpWindow);
        }
        private void txtMilitaryFunds_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // イベントを取り除く
            var cast = (UIElement)sender;
            cast.MouseLeave -= TotalCost_MouseLeave;

            // 表示中のヘルプを取り除く
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if (itemHelp.Name == "Help_StrategyMenu_MilitaryFunds")
                {
                    mainWindow.canvasUI.Children.Remove(itemHelp);
                    break;
                }
            }

            // 他のヘルプを隠してた場合は、最前面のヘルプだけ表示する
            int maxZ = -1, thisZ;
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    thisZ = Canvas.GetZIndex(itemHelp);
                    if (maxZ < thisZ)
                    {
                        maxZ = thisZ;
                    }
                }
            }
            if (maxZ >= 0)
            {
                foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
                {
                    if ((itemHelp.Visibility == Visibility.Hidden) && (itemHelp.Name.StartsWith("Help_") == true))
                    {
                        if (Canvas.GetZIndex(itemHelp) == maxZ)
                        {
                            itemHelp.Visibility = Visibility.Visible;
                            break;
                        }
                    }
                }
            }
        }

    }
}
