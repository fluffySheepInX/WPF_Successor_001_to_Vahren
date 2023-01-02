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
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_Successor_001_to_Vahren._005_Class;

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// UserControl060_WorldMap.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl060_WorldMap : UserControl
    {
        public UserControl060_WorldMap()
        {
            InitializeComponent();
        }

        // 全てのデータを初期化する
        public void SetData()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // サイズを設定する（画像のサイズではない）
            this.Width = mainWindow.CanvasMainWidth * 2;
            this.Height = mainWindow.CanvasMainHeight * 2;
            // 最初はマップの中央を画面の中央にする
            this.Margin = new Thickness()
            {
                Left = -(mainWindow.CanvasMainWidth / 2),
                Top = -(mainWindow.CanvasMainHeight / 2)
            };
            mainWindow.ClassGameStatus.Camera = new Point(this.Margin.Left, this.Margin.Top);

            this.canvasMap.Children.Clear();

            // mapImage読み込み
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("005_BackgroundImage");
                strings.Add("015_MapImage");
                strings.Add(mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].NameMapImageFile);
                string path = System.IO.Path.Combine(strings.ToArray());
                if (System.IO.File.Exists(path))
                {
                    BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                    Image imgMap = new Image();
                    imgMap.Width = this.Width;
                    imgMap.Height = this.Height;
                    imgMap.Stretch = Stretch.Fill;
                    imgMap.Source = bitimg1;
                    this.canvasMap.Children.Add(imgMap);
                }
            }

            // spot読み込み
            {
                //現シナリオで使用するスポットを抽出する
                List<ClassSpot> spotList = new List<ClassSpot>();
                foreach (var item in mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].DisplayListSpot)
                {
                    foreach (var item2 in mainWindow.ClassGameStatus.AllListSpot)
                    {
                        if (item == item2.NameTag)
                        {
                            spotList.Add(item2);
                            break;
                        }
                    }
                }

                //spotをlineで繋ぐ
                foreach (var item in mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].ListLinkSpot)
                {
                    var ext1 = spotList.Where(x => x.NameTag == item.Item1).FirstOrDefault();
                    if (ext1 == null)
                    {
                        continue;
                    }
                    var ext2 = spotList.Where(x => x.NameTag == item.Item2).FirstOrDefault();
                    if (ext2 == null)
                    {
                        continue;
                    }
                    Line line = new Line();
                    line.X1 = ext1.X;
                    line.Y1 = ext1.Y;
                    line.X2 = ext2.X;
                    line.Y2 = ext2.Y;
                    line.Stroke = Brushes.Black;
                    line.StrokeThickness = 3;
                    canvasMap.Children.Add(line);
                }

                //spotを出す
                foreach (var item in spotList.Select((value, index) => (value, index)))
                {
                    Grid gridButton = new Grid();
                    gridButton.Name = "SpotGrid" + item.value.NameTag;
                    gridButton.Height = mainWindow.ClassGameStatus.GridCityWidthAndHeight.Y;
                    gridButton.Width = mainWindow.ClassGameStatus.GridCityWidthAndHeight.X;
                    gridButton.Margin = new Thickness()
                    {
                        Left = item.value.X - gridButton.Width / 2,
                        Top = item.value.Y - gridButton.Height / 2
                    };
                    gridButton.MouseLeftButtonDown += mapSpot_MouseLeftButtonDown;
                    gridButton.MouseRightButtonDown += mapSpot_MouseRightButtonDown;
                    gridButton.MouseEnter += mapSpot_MouseEnter;

                    // 将来的には、領地アイコンのサイズを spot 構造体で指定する。
                    //// 標準は 32、とりあえず（32, 40, 48）で実験する。
                    //int spot_size = 32 + (item.index % 3) * 8;
                    int spot_size = 32;

                    BitmapImage bitimg1 = new BitmapImage(new Uri(item.value.ImagePath));
                    Image imgSpot = new Image();
                    imgSpot.Name = "SpotIcon" + item.value.NameTag;
                    imgSpot.Source = bitimg1;
                    imgSpot.HorizontalAlignment = HorizontalAlignment.Center;
                    imgSpot.VerticalAlignment = VerticalAlignment.Center;
                    imgSpot.Height = spot_size;
                    imgSpot.Width = spot_size;
                    gridButton.Children.Add(imgSpot);

                    TextBlock txtNameSpot = new TextBlock();
                    txtNameSpot.Name = "SpotName" + item.value.NameTag;
                    txtNameSpot.HorizontalAlignment = HorizontalAlignment.Center;
                    txtNameSpot.VerticalAlignment = VerticalAlignment.Top;
                    txtNameSpot.FontSize = 17;
                    txtNameSpot.Text = item.value.Name;
                    txtNameSpot.Foreground = Brushes.White;
                    // 文字に影を付ける (右下45度方向に1.5ピクセル)
                    txtNameSpot.Effect =
                        new DropShadowEffect
                        {
                            Direction = 315,
                            ShadowDepth = 1.5,
                            Opacity = 1,
                            BlurRadius = 0
                        };
                    // 領地アイコンと領地名の間隔は GridCityWidthAndHeight.Y によって決まる。
                    txtNameSpot.Margin = new Thickness()
                    {
                        Top = (gridButton.Height + spot_size) / 2
                    };
                    gridButton.Children.Add(txtNameSpot);

                    // その都市固有の情報を見る為に、勢力の持つスポットと、シナリオで登場するスポットを比較
                    string flag_path = string.Empty;
                    bool ch = false;
                    for (int i = 0; i < mainWindow.ClassGameStatus.ListPower.Count; i++)
                    {
                        foreach (var item3 in mainWindow.ClassGameStatus.ListPower[i].ListMember)
                        {
                            if (item3 == item.value.NameTag)
                            {
                                // その都市固有の情報を見る為にも、勢力情報と都市情報を入れる
                                var classPowerAndCity = new ClassPowerAndCity(mainWindow.ClassGameStatus.ListPower[i], item.value);
                                gridButton.Tag = classPowerAndCity;
                                //ついでに、スポットの属する勢力名を設定
                                item.value.PowerNameTag = mainWindow.ClassGameStatus.ListPower[i].NameTag;
                                // 旗画像のパスを取得する
                                flag_path = mainWindow.ClassGameStatus.ListPower[i].FlagPath;
                                ch = true;
                                break;
                            }
                        }

                        if (ch == true)
                        {
                            break;
                        }
                    }

                    //このタイミングで、そのボタンタグに何も設定されていない場合、無所属である
                    if (gridButton.Tag is not ClassPowerAndCity)
                    {
                        gridButton.Tag = new ClassPowerAndCity(new ClassPower(), item.value);
                    }
                    this.canvasMap.Children.Add(gridButton);
                    // 後から連結線を変更しても、領地が前面に来るようにする
                    Panel.SetZIndex(gridButton, 1);

                    // 旗を表示する
                    if (flag_path != String.Empty)
                    {
                        Image imgFlag = mainWindow.DisplayFlag(flag_path);
                        imgFlag.Name = "SpotFlag" + item.value.NameTag;
                        imgFlag.Margin = new Thickness()
                        {
                            Left = item.value.X - spot_size / 4,
                            Top = item.value.Y - spot_size / 2 - imgFlag.Height
                        };
                        this.canvasMap.Children.Add(imgFlag);
                        Panel.SetZIndex(imgFlag, 1);
                    }
                }
            }
        }

        // 領地の旗を変更する（旗のファイル名が空なら消す）
        public void ChangeFlag(string spotNameTag, string strFilename)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 旗が既に存在する場合は、先にその旗を消す
            if (spotNameTag == string.Empty)
            {
                return;
            }
            var flag = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasMap, "SpotFlag" + spotNameTag);
            if (flag != null)
            {
                this.canvasMap.Children.Remove(flag);
            }

            // 旗を表示する
            if (strFilename == string.Empty)
            {
                return;
            }
            var classSpot = mainWindow.ClassGameStatus.AllListSpot.Where(x => x.NameTag == spotNameTag).FirstOrDefault();
            if (classSpot == null)
            {
                return;
            }

            int spot_size = 32;
            Image imgFlag = mainWindow.DisplayFlag(strFilename);
            imgFlag.Name = "SpotFlag" + spotNameTag;
            imgFlag.Margin = new Thickness()
            {
                Left = classSpot.X - spot_size / 4,
                Top = classSpot.Y - spot_size / 2 - imgFlag.Height
            };
            this.canvasMap.Children.Add(imgFlag);
            Panel.SetZIndex(imgFlag, 1);
        }

        // 領地の所属勢力を変更する
        public void ChangeSpotPower(string spotNameTag, string powerNameTag)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 指定された領地を探す
            if (spotNameTag == string.Empty)
            {
                return;
            }
            var gridButton = (Grid)LogicalTreeHelper.FindLogicalNode(this.canvasMap, "SpotGrid" + spotNameTag);
            if (gridButton == null)
            {
                return;
            }
            var classPowerAndCity = (ClassPowerAndCity)gridButton.Tag;
            if (classPowerAndCity == null)
            {
                return;
            }
            if (classPowerAndCity is not ClassPowerAndCity)
            {
                return;
            }
            if (classPowerAndCity.ClassSpot.NameTag != spotNameTag)
            {
                return;
            }

            // 指定された勢力が空なら中立領地にする
            if (powerNameTag == string.Empty)
            {
                // 他の勢力に所属してた場合は、取り除く
                if (classPowerAndCity.ClassSpot.PowerNameTag != string.Empty)
                {
                    classPowerAndCity.ClassPower.ListMember.Remove(spotNameTag);
                }
                classPowerAndCity.ClassPower = new ClassPower();

                // 旗を消す
                this.ChangeFlag(spotNameTag, "");
                return;
            }

            // 指定された勢力を探す
            var newPower = mainWindow.ClassGameStatus.ListPower
                        .Where(x => x.NameTag == powerNameTag)
                        .FirstOrDefault();
            if (newPower == null)
            {
                // 存在しない勢力なら何もしない
                return;
            }

            // 他の勢力に所属してた場合は、取り除く
            if (classPowerAndCity.ClassSpot.PowerNameTag != string.Empty)
            {
                classPowerAndCity.ClassPower.ListMember.Remove(spotNameTag);
            }

            // 領地の所属情報を書き換える
            classPowerAndCity.ClassSpot.PowerNameTag = powerNameTag;
            newPower.ListMember.Add(spotNameTag);
            classPowerAndCity.ClassPower = newPower;

            // 領地に古い旗アイコンがあれば消して、新しい旗アイコンを置く
            this.ChangeFlag(spotNameTag, newPower.FlagPath);
        }


        // 勢力の全領地を強調する
        public void PowerMark(string strFilename, string powerNameTag)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // エフェクトの画像を読み込む
            List<string> strings = new List<string>();
            strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
            strings.Add("005_BackgroundImage");
            strings.Add(strFilename);
            string path = System.IO.Path.Combine(strings.ToArray());
            if (System.IO.File.Exists(path) == false)
            {
                // 画像が存在しない場合はエフェクトも無い
                return;
            }

            BitmapImage bitimg1 = new BitmapImage(new Uri(path));
            const int ring_size = 128;

            var listSpot = mainWindow.ClassGameStatus.AllListSpot.Where(x => x.PowerNameTag == powerNameTag);
            foreach (var itemSpot in listSpot)
            {
                // 領地が既に強調されてる場合はとばす
                var itemImage = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasMap, "SpotMark" + itemSpot.NameTag);
                if (itemImage != null)
                {
                    continue;
                }

                Image imgRing = new Image();
                imgRing.Name = "PowerMark" + itemSpot.NameTag;
                imgRing.Source = bitimg1;
                // アスペクト比を保つので、横幅だけ指定する
                imgRing.Width = ring_size;
                imgRing.Margin = new Thickness()
                {
                    Left = itemSpot.X - ring_size / 2,
                    Top = itemSpot.Y - ring_size / 2
                };
                this.canvasMap.Children.Add(imgRing);
            }
        }

        // 勢力の全領地をアニメーション付きで強調する
        public void PowerMarkAnime(string strFilename, string powerNameTag)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // エフェクトの画像を読み込む
            List<string> strings = new List<string>();
            strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
            strings.Add("005_BackgroundImage");
            strings.Add(strFilename);
            string path = System.IO.Path.Combine(strings.ToArray());
            if (System.IO.File.Exists(path) == false)
            {
                // 画像が存在しない場合はエフェクトも無い
                return;
            }

            BitmapImage bitimg1 = new BitmapImage(new Uri(path));
            const int ring_size = 96, ring_size2 = 152;

            // 輪の大きさを時間経過で変化させる（1.5秒間隔でループする）
            var animeRingSize = new DoubleAnimation();
            animeRingSize.To = ring_size2;
            animeRingSize.Duration = new Duration(TimeSpan.FromSeconds(0.75));
            animeRingSize.AutoReverse = true;
            animeRingSize.RepeatBehavior = RepeatBehavior.Forever;

            var listSpot = mainWindow.ClassGameStatus.AllListSpot.Where(x => x.PowerNameTag == powerNameTag);
            foreach (var itemSpot in listSpot)
            {
                // 領地が既に強調されてる場合はとばす
                var itemImage = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasMap, "SpotMark" + itemSpot.NameTag);
                if (itemImage != null)
                {
                    continue;
                }

                Image imgRing = new Image();
                imgRing.Name = "PowerMark" + itemSpot.NameTag;
                imgRing.Source = bitimg1;
                // アスペクト比を保って拡大縮小するので、横幅だけ指定する
                imgRing.Width = ring_size;
                imgRing.Margin = new Thickness()
                {
                    Left = itemSpot.X - ring_size / 2,
                    Top = itemSpot.Y - ring_size / 2
                };
                this.canvasMap.Children.Add(imgRing);

                // 円の大きさは共通アニメーションにする
                imgRing.BeginAnimation(Image.WidthProperty, animeRingSize);

                // 円の位置も変えないと中心がずれる
                var animeRingPos = new ThicknessAnimation();
                animeRingPos.To = new Thickness()
                {
                    Left = itemSpot.X - ring_size2 / 2,
                    Top = itemSpot.Y - ring_size2 / 2
                };
                animeRingPos.Duration = new Duration(TimeSpan.FromSeconds(0.75));
                animeRingPos.AutoReverse = true;
                animeRingPos.RepeatBehavior = RepeatBehavior.Forever;
                imgRing.BeginAnimation(Image.MarginProperty, animeRingPos);
            }
        }

        // 勢力領の強調を解除する
        public void RemovePowerMark(string powerNameTag)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var listSpot = mainWindow.ClassGameStatus.AllListSpot.Where(x => x.PowerNameTag == powerNameTag);
            foreach (var itemSpot in listSpot)
            {
                var itemImage = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasMap, "PowerMark" + itemSpot.NameTag);
                if (itemImage != null)
                {
                    // 円を取り除く
                    this.canvasMap.Children.Remove(itemImage);
                }
            }
        }

        // 全ての勢力領の強調を解除する
        public void RemovePowerMarkAll()
        {
            for (int i = this.canvasMap.Children.Count - 1; i >= 0; i += -1)
            {
                UIElement Child = this.canvasMap.Children[i];
                if (Child is Image)
                {
                    var itemImage = (Image)Child;
                    if (itemImage.Name.StartsWith("PowerMark"))
                    {
                        // 円を取り除く
                        this.canvasMap.Children.Remove(itemImage);
                    }
                }
            }
        }


        // 領地を強調する
        public void SpotMark(string strFilename, string spotNameTag)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var classSpot = mainWindow.ClassGameStatus.AllListSpot.Where(x => x.NameTag == spotNameTag).FirstOrDefault();
            if (classSpot == null)
            {
                return;
            }

            // 領地が既に強調されてる場合は古い方を消す
            var itemImage = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasMap, "SpotMark" + classSpot.NameTag);
            if (itemImage != null)
            {
                this.canvasMap.Children.Remove(itemImage);
            }

            // エフェクトの画像を読み込む
            List<string> strings = new List<string>();
            strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
            strings.Add("005_BackgroundImage");
            strings.Add(strFilename);
            string path = System.IO.Path.Combine(strings.ToArray());
            if (System.IO.File.Exists(path) == false)
            {
                // 画像が存在しない場合はエフェクトも無い
                return;
            }

            BitmapImage bitimg1 = new BitmapImage(new Uri(path));
            const int ring_size = 128;

            Image imgRing = new Image();
            imgRing.Name = "SpotMark" + classSpot.NameTag;
            imgRing.Source = bitimg1;
                // アスペクト比を保つので、横幅だけ指定する
            imgRing.Width = ring_size;
            imgRing.Margin = new Thickness()
            {
                Left = classSpot.X - ring_size / 2,
                Top = classSpot.Y - ring_size / 2
            };
            this.canvasMap.Children.Add(imgRing);
        }

        // 領地をアニメーション付きで強調する
        public void SpotMarkAnime(string strFilename, string spotNameTag)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var classSpot = mainWindow.ClassGameStatus.AllListSpot.Where(x => x.NameTag == spotNameTag).FirstOrDefault();
            if (classSpot == null)
            {
                return;
            }

            // 領地が既に強調されてる場合は古い方を消す
            var itemImage = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasMap, "SpotMark" + classSpot.NameTag);
            if (itemImage != null)
            {
                this.canvasMap.Children.Remove(itemImage);
            }

            // エフェクトの画像を読み込む
            List<string> strings = new List<string>();
            strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
            strings.Add("005_BackgroundImage");
            strings.Add(strFilename);
            string path = System.IO.Path.Combine(strings.ToArray());
            if (System.IO.File.Exists(path) == false)
            {
                // 画像が存在しない場合はエフェクトも無い
                return;
            }

            BitmapImage bitimg1 = new BitmapImage(new Uri(path));
            const int ring_size = 96, ring_size2 = 152;

            Image imgRing = new Image();
            imgRing.Name = "SpotMark" + classSpot.NameTag;
            imgRing.Source = bitimg1;
            // アスペクト比を保って拡大縮小するので、横幅だけ指定する
            imgRing.Width = ring_size;
            imgRing.Margin = new Thickness()
            {
                Left = classSpot.X - ring_size / 2,
                Top = classSpot.Y - ring_size / 2
            };
            this.canvasMap.Children.Add(imgRing);

            // 輪の大きさを時間経過で変化させる（1.5秒間隔でループする）
            var animeRingSize = new DoubleAnimation();
            animeRingSize.To = ring_size2;
            animeRingSize.Duration = new Duration(TimeSpan.FromSeconds(0.75));
            animeRingSize.AutoReverse = true;
            animeRingSize.RepeatBehavior = RepeatBehavior.Forever;
            imgRing.BeginAnimation(Image.WidthProperty, animeRingSize);

            // 円の位置も変えないと中心がずれる
            var animeRingPos = new ThicknessAnimation();
            animeRingPos.To = new Thickness()
            {
                Left = classSpot.X - ring_size2 / 2,
                Top = classSpot.Y - ring_size2 / 2
            };
            animeRingPos.Duration = new Duration(TimeSpan.FromSeconds(0.75));
            animeRingPos.AutoReverse = true;
            animeRingPos.RepeatBehavior = RepeatBehavior.Forever;
            imgRing.BeginAnimation(Image.MarginProperty, animeRingPos);
        }

        // 領地の強調を解除する
        public void RemoveSpotMark(string spotNameTag)
        {
            var itemImage = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasMap, "SpotMark" + spotNameTag);
            if (itemImage != null)
            {
                // 円を取り除く
                this.canvasMap.Children.Remove(itemImage);
            }
        }

        // 全ての領地の強調を解除する
        public void RemoveSpotMarkAll()
        {
            for (int i = this.canvasMap.Children.Count - 1; i >= 0; i += -1)
            {
                UIElement Child = this.canvasMap.Children[i];
                if (Child is Image)
                {
                    var itemImage = (Image)Child;
                    if (itemImage.Name.StartsWith("SpotMark"))
                    {
                        // 円を取り除く
                        this.canvasMap.Children.Remove(itemImage);
                    }
                }
            }
        }


        // 勢力選択中なら、シナリオ選択画面に戻る
        private void map_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            switch (mainWindow.ClassGameStatus.NowSituation)
            {
                case _010_Enum.Situation.Title:
                    break;
                case _010_Enum.Situation.MainMenu:
                    break;
                case _010_Enum.Situation.SelectGroup: // 勢力選択画面

                    mainWindow.FadeOut = true;

                    mainWindow.delegateMainWindowContentRendered = mainWindow.SetWindowMainMenu;

                    mainWindow.FadeIn = true;

                    break;
                case _010_Enum.Situation.TextWindow_Conversation:
                    break;
                case _010_Enum.Situation.PlayerTurn:
                    break;
                case _010_Enum.Situation.EnemyTurn:
                    break;
                case _010_Enum.Situation.InfoWindowMini:
                    break;
                case _010_Enum.Situation.DebugGame:
                    break;
                case _010_Enum.Situation.PlayerTurnEnemyCityLeftClick:
                    break;
                case _010_Enum.Situation.PlayerTurnPlayerCityLeftClick:
                    break;
                case _010_Enum.Situation.Battle_InfoWindowMini:
                    break;
                case _010_Enum.Situation.Battle:
                    break;
                case _010_Enum.Situation.BattleStop:
                    break;
                case _010_Enum.Situation.Game:
                    break;
                case _010_Enum.Situation.GenusList:
                    break;
                case _010_Enum.Situation.ToolList:
                    break;
                case _010_Enum.Situation.GameStop:
                    break;
                case _010_Enum.Situation.PreparationBattle:
                    break;
                case _010_Enum.Situation.PreparationBattle_UnitList:
                    break;
                case _010_Enum.Situation.PreparationBattle_MiniWindow:
                    break;
                default:
                    break;
            }
        }

        // 勢力選択中のヘルプ
        private void map_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 勢力選択画面
            if (mainWindow.ClassGameStatus.NowSituation == _010_Enum.Situation.SelectGroup)
            {
                // カーソルを離した時のイベントを追加する
                var cast = (UIElement)sender;
                cast.MouseLeave += map_MouseLeave;

                // ヘルプを作成する
                var helpWindow = new UserControl030_Help();
                helpWindow.Name = "Help_SelectPower";
                helpWindow.SetText("旗のある領地を左クリックするとプレイ勢力を選択します。\n領地以外を左ドラッグするとワールドマップを動かせます。\n右クリックするとシナリオ選択画面に戻ります。");
                mainWindow.canvasUI.Children.Add(helpWindow);

                // 領地のヒントが表示されてる時はヘルプを隠す
                foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl011_SpotHint>())
                {
                    if (itemWindow.Name == "HintSpot")
                    {
                        helpWindow.Visibility = Visibility.Hidden;
                        break;
                    }
                }
            }
        }
        private void map_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 勢力選択画面
            if (mainWindow.ClassGameStatus.NowSituation == _010_Enum.Situation.SelectGroup)
            {
                // イベントを取り除く
                var cast = (UIElement)sender;
                cast.MouseLeave -= map_MouseLeave;

                // 表示中のヘルプを閉じる
                foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
                {
                    if (itemWindow.Name == "Help_SelectPower")
                    {
                        mainWindow.canvasUI.Children.Remove(itemWindow);
                        break;
                    }
                }
            }
        }

        #region マップ移動
        private bool _isDrag = false; // 外部に公開する必要なし
        private Point _startPoint;

        private void map_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            // ドラッグを開始する
            UIElement? el = sender as UIElement;
            if (el == null)
            {
                return;
            }
            _isDrag = true;
            _startPoint = e.GetPosition(el);
            el.CaptureMouse();
            el.MouseLeftButtonUp += map_MouseLeftButtonUp;
            el.MouseMove += map_MouseMove;
        }
        private void map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // ドラック中なら終了する
            if (_isDrag == true)
            {
                UIElement? el = sender as UIElement;
                if (el == null)
                {
                    return;
                }
                el.ReleaseMouseCapture();
                el.MouseLeftButtonUp -= map_MouseLeftButtonUp;
                el.MouseMove -= map_MouseMove;
                _isDrag = false;
            }
        }
        private void map_MouseMove(object sender, MouseEventArgs e)
        {
            // ドラック中
            if (_isDrag == true)
            {
                UIElement? el = sender as UIElement;
                if (el == null)
                {
                    return;
                }
                Point pt = e.GetPosition(el);

                var thickness = new Thickness();
                thickness.Left = this.Margin.Left + (pt.X - _startPoint.X);
                if (thickness.Left > this.Width / 4)
                {
                    thickness.Left = this.Width / 4;
                }
                if (thickness.Left < this.Width / 4 - this.Width)
                {
                    thickness.Left = this.Width / 4 - this.Width;
                }
                thickness.Top = this.Margin.Top + (pt.Y - _startPoint.Y);
                if (thickness.Top > this.Height / 4)
                {
                    thickness.Top = this.Height / 4;
                }
                if (thickness.Top < this.Height / 4 - this.Height)
                {
                    thickness.Top = this.Height / 4 - this.Height;
                }
                this.Margin = thickness;
            }
        }
        #endregion


        // 戦略マップの領地にマウスを乗せた時
        private void mapSpot_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var cast = (FrameworkElement)sender;
            if (cast.Tag is not ClassPowerAndCity)
            {
                return;
            }

            // マウスを離した時のイベントを追加する
            cast.MouseLeave += mapSpot_MouseLeave;

            // 選択した領地を強調する
            ClassPowerAndCity classPowerAndCity = (ClassPowerAndCity)cast.Tag;
            var txtNameSpot = (TextBlock)LogicalTreeHelper.FindLogicalNode(this.canvasMap, "SpotName" + classPowerAndCity.ClassSpot.NameTag);
            if (txtNameSpot != null)
            {
                // 領地名の色を変える（少し暗くする）
                txtNameSpot.Foreground = Brushes.Gainsboro;
            }
            var imgSpot = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasMap, "SpotIcon" + classPowerAndCity.ClassSpot.NameTag);
            if (imgSpot != null)
            {
                // 同じ箇所で既にダミー画像を表示してる場合は、新たに表示しない
                var oldDummy = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasMap, "DummySpotIcon" + classPowerAndCity.ClassSpot.NameTag);
                if (oldDummy == null)
                {
                    // 本体を透明にして、ダミー画像でアニメーション表示する
                    // 余計なイベントが発生しないはず
                    imgSpot.Opacity = 0;

                    Image imgDummy = new Image();
                    imgDummy.Name = "DummySpotIcon" + classPowerAndCity.ClassSpot.NameTag;
                    imgDummy.Source = imgSpot.Source;
                    imgDummy.HorizontalAlignment = HorizontalAlignment.Left;
                    imgDummy.VerticalAlignment = VerticalAlignment.Top;
                    imgDummy.Width = imgSpot.Width;
                    imgDummy.Height = imgSpot.Height;
                    imgDummy.Margin = new Thickness()
                    {
                        Left = classPowerAndCity.ClassSpot.X - imgDummy.Width / 2,
                        Top = classPowerAndCity.ClassSpot.Y - imgDummy.Height / 2
                    };
                    this.canvasMap.Children.Add(imgDummy);

                    // 少し上に上がって、元の位置に戻るアニメーション
                    var animeIconPos = new ThicknessAnimation();
                    animeIconPos.To = new Thickness()
                    {
                        Left = classPowerAndCity.ClassSpot.X - imgDummy.Width / 2,
                        Top = classPowerAndCity.ClassSpot.Y - imgDummy.Height / 2 - 12
                    };
                    animeIconPos.Duration = new Duration(TimeSpan.FromSeconds(0.2));
                    animeIconPos.AutoReverse = true;
                    // アニメーションが終わったら自動的にダミー画像を消す
                    animeIconPos.Completed += animeSpotMouseEnter_Completed;
                    imgDummy.BeginAnimation(Image.MarginProperty, animeIconPos);
                }
            }

            // 同じ勢力の全ての領地を強調する
            if (classPowerAndCity.ClassPower.ListMember.Count > 0)
            {
                string powerNameTag = classPowerAndCity.ClassPower.NameTag;
                string strFilename;
                // プレイヤー勢力なら色を変える
                if (mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.NameTag == powerNameTag)
                {
                    strFilename = "circle_Aqua.png";
                }
                else
                {
                    strFilename =  "circle_Lime.png";
                }
                PowerMark(strFilename, powerNameTag);
            }

            // 場所が重なるのでヘルプを全て隠す
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Visible) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    itemHelp.Visibility = Visibility.Hidden;
                }
            }

            // 領地のヒントを作成する
            var hintSpot = new UserControl011_SpotHint();
            hintSpot.Name = "HintSpot";
            hintSpot.Tag = classPowerAndCity;
            hintSpot.SetData();
            mainWindow.canvasUI.Children.Add(hintSpot);

            // 領地の説明文を表示する
            if (classPowerAndCity.ClassSpot.Text != string.Empty)
            {
                var detailSpot = new UserControl025_DetailSpot();
                detailSpot.Name = "DetailSpot";
                detailSpot.Tag = classPowerAndCity.ClassSpot;
                detailSpot.SetData();
                mainWindow.canvasUI.Children.Add(detailSpot);
            }
        }
        private void mapSpot_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // イベントを取り除く
            var cast = (FrameworkElement)sender;
            cast.MouseLeave -= mapSpot_MouseLeave;

            // 選択した領地の強調を解除する
            ClassPowerAndCity classPowerAndCity = (ClassPowerAndCity)cast.Tag;
            var txtNameSpot = (TextBlock)LogicalTreeHelper.FindLogicalNode(this.canvasMap, "SpotName" + classPowerAndCity.ClassSpot.NameTag);
            if (txtNameSpot != null)
            {
                // 領地名の色を戻す
                txtNameSpot.Foreground = Brushes.White;
            }
            /*
            自動的に消す手法は、タイミング次第では別の場所のを消す可能性がある。
            問題があるようならこちらに戻すので、元のコードを残しておく。
            var imgSpot = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasMap, "SpotIcon" + classPowerAndCity.ClassSpot.NameTag);
            if (imgSpot != null)
            {
                // 透明度を元に戻して、ダミー画像を消す
                imgSpot.Opacity = 1;
                var imgDummy = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasMap, "DummySpotIcon" + classPowerAndCity.ClassSpot.NameTag);
                if (imgDummy != null)
                {
                    this.canvasMap.Children.Remove(imgDummy);
                }
            }
            */

            // 勢力領の強調を解除する
            if (classPowerAndCity.ClassPower.ListMember.Count > 0)
            {
                RemovePowerMark(classPowerAndCity.ClassPower.NameTag);
            }

            // 領地のヒントを閉じる
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl011_SpotHint>())
            {
                if (itemWindow.Name == "HintSpot")
                {
                    mainWindow.canvasUI.Children.Remove(itemWindow);
                    break;
                }
            }

            // ヘルプを隠してた場合は、最前面のヘルプだけ表示する
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

            // 領地の説明文を取り除く
            if (classPowerAndCity.ClassSpot.Text != string.Empty)
            {
                foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl025_DetailSpot>())
                {
                    if (itemWindow.Name == "DetailSpot")
                    {
                        itemWindow.Remove();
                        break;
                    }
                }
            }
        }
        private void animeSpotMouseEnter_Completed(object? sender, EventArgs e)
        {
            // アニメーションが終わったダミー画像の名前を取得できない欠陥があることに注意
            foreach (var imgDummy in this.canvasMap.Children.OfType<Image>())
            {
                if (imgDummy.Name.StartsWith("DummySpotIcon") == true)
                {
                    var imgSpot = (Image)LogicalTreeHelper.FindLogicalNode(this.canvasMap, imgDummy.Name.Substring(5));
                    if (imgSpot != null)
                    {
                        // 透明度を元に戻す
                        imgSpot.Opacity = 1;
                    }

                    // ダミー画像を消す
                    imgDummy.BeginAnimation(Grid.MarginProperty, null);
                    this.canvasMap.Children.Remove(imgDummy);

                    break;
                }
            }
        }

        private void mapSpot_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            // ルーティングを処理済みとしてマークする（親コントロールのイベントが発生しなくなる）
            e.Handled = true;

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // プレイヤーのターン中なら、領地ウィンドウを表示する
            if (mainWindow.ClassGameStatus.NowSituation == _010_Enum.Situation.PlayerTurn)
            {
                var cast = (FrameworkElement)sender;
                if (cast.Tag is not ClassPowerAndCity)
                {
                    return;
                }
                var classPowerAndCity = (ClassPowerAndCity)cast.Tag;

                // ウインドウの左上が領地の場所になるように配置する
                // （領地のクリック範囲が広いので、マウスカーソルを基準にする）
                Point posMouse = Mouse.GetPosition(mainWindow.canvasUI);
                Thickness posWindow = new Thickness()
                {
                    Left = posMouse.X - 40,
                    Top = posMouse.Y - 40
                };

                // 出撃ウィンドウが開いてる場合は出撃選択用領地ウィンドウを表示する
                bool isFound = false;
                foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl065_Sortie>())
                {
                    if (itemWindow.Name.StartsWith(StringName.windowSortie))
                    {
                        // 出撃可能な領地かどうかを調べる
                        List<ClassSpot>? listSpot = (List<ClassSpot>)itemWindow.Tag;
                        if (listSpot == null)
                        {
                            return;
                        }
                        var spot = listSpot.Where(x => x.NameTag == classPowerAndCity.ClassSpot.NameTag).FirstOrDefault();
                        if (spot == null)
                        {
                            var dialog = new Win020_Dialog();
                            dialog.SetText("この領地は出撃不可です。\n青枠の領地を選択してください。");
                            dialog.SetTime(1.2); // 待ち時間を1.2秒に短縮する
                            dialog.ShowDialog();
                            return;
                        }

                        isFound = true;
                        break;
                    }
                }
                if (isFound)
                {
                    // 既に表示されてる領地ウィンドウをチェックする
                    isFound = false;
                    string strTitle = StringName.windowSpotSortie + classPowerAndCity.ClassSpot.NameTag;
                    foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl012_SpotSortie>())
                    {
                        if (itemWindow.Name == strTitle)
                        {
                            // 領地ウインドウを既に開いてる場合は、新規に作らない
                            itemWindow.Margin = posWindow;

                            // 最前面に移動する
                            var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != itemWindow);
                            if ((listWindow != null) && (listWindow.Any()))
                            {
                                int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                                Canvas.SetZIndex(itemWindow, maxZ + 1);
                            }

                            isFound = true;
                            break;
                        }
                    }
                    if (isFound == false)
                    {
                        // 新規に作成する
                        var windowSpot = new UserControl012_SpotSortie();
                        windowSpot.Tag = classPowerAndCity;
                        windowSpot.Name = strTitle;
                        windowSpot.Margin = posWindow;
                        windowSpot.SetData();
                        mainWindow.canvasUI.Children.Add(windowSpot);
                    }
                    return;
                }

                // 既に表示されてる領地ウィンドウをチェックする
                int window_id, max_id = 0;
                var id_list = new List<int>();
                foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl010_Spot>())
                {
                    string strTitle = itemWindow.Name;
                    if (strTitle.StartsWith(StringName.windowSpot))
                    {
                        window_id = Int32.Parse(strTitle.Substring(StringName.windowSpot.Length));
                        id_list.Add(window_id);
                        if (max_id < window_id)
                        {
                            max_id = window_id;
                        }
                        var ri = (ClassPowerAndCity)itemWindow.Tag;
                        if (ri.ClassSpot.NameTag == classPowerAndCity.ClassSpot.NameTag)
                        {
                            // 領地ウィンドウを既に開いてる場合は、新規に作らない
                            max_id = -1;
                            itemWindow.Margin = posWindow;

                            // 最前面に移動する
                            var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != itemWindow);
                            if ((listWindow != null) && (listWindow.Any()))
                            {
                                int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                                Canvas.SetZIndex(itemWindow, maxZ + 1);
                            }

                            break;
                        }
                    }
                }
                if (max_id >= 0)
                {
                    if (max_id > id_list.Count)
                    {
                        // ウィンドウ個数よりも最大値が大きいなら、未使用の番号を使って作成する
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
                        // 使用中のウィンドウ番号の最大値 + 1 にして、新規に作成する
                        window_id = max_id + 1;
                    }
                    var windowSpot = new UserControl010_Spot();
                    windowSpot.Tag = classPowerAndCity;
                    windowSpot.Name = StringName.windowSpot + window_id.ToString();
                    windowSpot.Margin = posWindow;
                    windowSpot.SetData();
                    mainWindow.canvasUI.Children.Add(windowSpot);
                }
                id_list.Clear();

            }
            // 勢力を選択中なら、勢力詳細ウィンドウを表示する
            else if (mainWindow.ClassGameStatus.NowSituation == _010_Enum.Situation.SelectGroup)
            {
                mainWindow.DisplayPowerSelection(sender);
            }
        }

        private void mapSpot_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            // ルーティングを処理済みとしてマークする（親コントロールのイベントが発生しなくなる）
            e.Handled = true;

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            if (mainWindow.ClassGameStatus.NowSituation == _010_Enum.Situation.SelectGroup)
            {
                return; //勢力選択中は出撃しない。
            }

            var cast = (FrameworkElement)sender;
            if (cast.Tag is not ClassPowerAndCity)
            {
                return;
            }
            var classPowerAndCity = (ClassPowerAndCity)cast.Tag;

            //自ターンチェック
            //CPUタイムに押されても平気なように

            // 既に出撃ウィンドウが開いてる場合は、右クリックを無視する
            // 自動的に閉じてもいいけど、他の場所をクリックした際の警告メッセージがややこしい
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl065_Sortie>())
            {
                // 出撃先が同じ場合は、ウィンドウ位置を中央に戻す
                if (itemWindow.Name == StringName.windowSortie + classPowerAndCity.ClassSpot.NameTag)
                {
                    // 最前面に配置する
                    var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != itemWindow);
                    if ((listWindow != null) && (listWindow.Any()))
                    {
                        int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                        Canvas.SetZIndex(itemWindow, maxZ + 1);
                    }

                    // 画面の中央に配置する
                    itemWindow.Margin = new Thickness()
                    {
                        Left = mainWindow.canvasUI.Width / 2 - 300,
                        Top = mainWindow.canvasUI.Height / 2 - 400
                    };
                    return;
                }
                else if (itemWindow.Name.StartsWith(StringName.windowSortie))
                {
                    return;
                }
            }

            //所属チェック
            if (classPowerAndCity.ClassPower.NameTag == mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.NameTag)
            {
                var dialog2 = new Win020_Dialog();
                dialog2.SetText("自勢力の領地には攻め込めません。");
                dialog2.SetTime(1.2); // 待ち時間を1.2秒に短縮する
                dialog2.ShowDialog();
                return; //自国には攻め込まない。
            }

            ////隣接チェック
            //国に関係なく隣接都市名を抽出
            List<string> NameRinsetuSpot = new List<string>();
            foreach (var item in mainWindow.ListClassScenarioInfo[mainWindow.NumberScenarioSelection].ListLinkSpot)
            {
                if (classPowerAndCity.ClassSpot.NameTag == item.Item1)
                {
                    NameRinsetuSpot.Add(item.Item2);
                    continue;
                }
                if (classPowerAndCity.ClassSpot.NameTag == item.Item2)
                {
                    NameRinsetuSpot.Add(item.Item1);
                }
            }
            //国で隣接都市名を抽出
            List<ClassSpot> classSpots = new List<ClassSpot>();
            foreach (var item in NameRinsetuSpot)
            {
                var ge = mainWindow.ClassGameStatus.AllListSpot.Where(x => x.NameTag == item).FirstOrDefault();
                if (ge == null)
                {
                    continue;
                }
                if (ge.PowerNameTag != mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.NameTag)
                {
                    continue;
                }
                // 領地に部隊が存在するか確かめる（簡易チェックなので、行動済みかどうかまでは調べない）
                if (ge.UnitGroup.Count > 0)
                {
                    classSpots.Add(ge);
                }
            }
            if (classSpots.Count == 0)
            {
                var dialog2 = new Win020_Dialog();
                dialog2.SetText("味方領が隣接してない領地には攻め込めません。");
                dialog2.SetTime(1.2); // 待ち時間を1.2秒に短縮する
                dialog2.ShowDialog();
                return; //自国と隣接してないので出撃できない。
            }

            // ダイアログを表示する
            //MessageBox.Show("出撃します");
            var dialog = new Win020_Dialog();
            dialog.SetText(classPowerAndCity.ClassSpot.Name + "へ出撃します。\n青枠の領地から編成してください。");
            dialog.SetTime(1.2); // 待ち時間を1.2秒に短縮する
            dialog.ShowDialog();

            // 現在のマップ表示位置を記録しておく
            mainWindow.ClassGameStatus.Camera = new Point(this.Margin.Left, this.Margin.Top);

            // 出撃ウィンドウを表示する
            var windowSortie = new UserControl065_Sortie();
            windowSortie.Tag = classSpots; // 出撃可能な隣接領のリスト
            windowSortie.Name = StringName.windowSortie + classPowerAndCity.ClassSpot.NameTag; // 出撃先の識別名
            windowSortie.SetData();
            mainWindow.canvasUI.Children.Add(windowSortie);

            // 戦闘後に防衛側の情報を参照できるよう記録しておく
            Application.Current.Properties["defensePowerAndCity"] = classPowerAndCity;

/*
            Uri uri = new Uri("/Page010_SortieMenu.xaml", UriKind.Relative);
            Frame frame = new Frame();
            frame.Source = uri;
            frame.Margin = new Thickness(0, 0, 0, 0);
            frame.Name = StringName.windowSortieMenu;
            mainWindow.canvasMain.Children.Add(frame);
            Application.Current.Properties["window"] = mainWindow;
            Application.Current.Properties["spots"] = classSpots;
*/

        }

    }
}
