using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
using System.Windows.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;
using WPF_Successor_001_to_Vahren._005_Class;
using WPF_Successor_001_to_Vahren._006_ClassStatic;
using WPF_Successor_001_to_Vahren._010_Enum;
using WPF_Successor_001_to_Vahren._015_Lexer;
using WPF_Successor_001_to_Vahren._020_AST;
using WPF_Successor_001_to_Vahren._025_Parser;
using WPF_Successor_001_to_Vahren._030_Evaluator;
using WpfAnimatedGif;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;
using Image = System.Windows.Controls.Image;

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : CommonWindow
    {
        #region Prop

        #region GameTitle
        public string GameTitle
        {
            get
            {
                if (this.IsEng == true)
                {
                    return "";
                }
                else
                {
                    // TODO
                    return "ローガントゥーガ";
                }
            }
            set
            {

            }
        }
        #endregion

        #region NowSituation
        public _010_Enum.Situation NowSituation
        {
            get
            {
                return _nowSituation;
            }
            set
            {
                _nowSituation = value;
            }
        }
        #endregion

        #region DifficultyLevel
        public int DifficultyLevel
        {
            get
            {
                return _difficultyLevel;
            }
            set
            {
                _difficultyLevel = value;
            }
        }
        #endregion

        #region ListClassScenario
        public List<ClassScenarioInfo> ListClassScenarioInfo
        {
            get { return _listClassScenarioInfo; }
            set { _listClassScenarioInfo = value; }
        }
        #endregion

        #region NumberScenarioSelection
        public int NumberScenarioSelection
        {
            get { return _numberScenarioSelection; }
            set { _numberScenarioSelection = value; }
        }
        #endregion

        #endregion

        #region PrivateField
        private int _numberScenarioSelection;
        private List<ClassScenarioInfo> _listClassScenarioInfo = new List<ClassScenarioInfo>();
        private int _difficultyLevel = 0;
        private _010_Enum.Situation _nowSituation = _010_Enum.Situation.Title;

        private readonly string _pathConfigFile
            = System.IO.Path.Combine(Environment.CurrentDirectory, "configFile.xml");

        private ClassConfigGameTitle _classConfigGameTitle = new ClassConfigGameTitle();

        private const double constantInterval = 16.6;

        #endregion

        #region PublicField
        public delegate void DelegateMainWindowContentRendered();
        public DelegateMainWindowContentRendered? delegateMainWindowContentRendered = null;
        public delegate void DelegateMapRendered();
        public DelegateMapRendered? delegateMapRendered = null;
        public delegate void DelegateNewGame();
        public DelegateNewGame? delegateNewGame = null;
        public delegate void DelegateNewGameAfterFadeIn();
        public DelegateNewGameAfterFadeIn? delegateNewGameAfterFadeIn = null;
        public delegate void DelegateBattleMap();
        public DelegateBattleMap? delegateBattleMap = null;

        public readonly CountdownEvent condition = new CountdownEvent(1);

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            ReadFileOrderDocument();
        }

        #region Event
        private void MainWindow_Initialized(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show("ゲームエンジンとしてこういうことが出来ますよというプレゼンであり、" + System.Environment.NewLine + "デフォシナではないことご了承下さい");
                // 初期状態でフルスクリーンにする。F11キーを押すとウインドウ表示になる。
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                // ウインドウ表示で始めたい場合は、上をコメントアウトして、下のコメントを解除する。
                //this.WindowStyle = WindowStyle.SingleBorderWindow;
                //this.WindowState = WindowState.Normal;
                this.DataContext = new
                {
                    title = this.GameTitle,
                    canvasMainWidth = this.CanvasMainWidth,
                    canvasMainHeight = this.CanvasMainHeight
                };
                // コントロールのプロパティの標準値を指定する
                // ツールチップが表示されるまでの時間を最小にする。
                ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(0));
            }
            catch (Exception err)
            {
                MessageBox.Show("Error.Number is 1:" + Environment.NewLine + err.Message);
                throw;
            }
        }

        private void MainWindow_ContentRendered(object sender, EventArgs e)
        {
            // canvasTop_SizeChangedの後に実行される
            try
            {
                this.Background = System.Windows.Media.Brushes.Black;
                this.canvasMain.Background = System.Windows.Media.Brushes.Black;
                //this.canvasMain.Margin = new Thickness()
                //{
                //    Top = (this._sizeClientWinHeight / 2) - (this.CanvasMainHeight / 2),
                //    Left = (this._sizeClientWinWidth / 2) - (this.CanvasMainWidth / 2),
                //};
                //this.WindowStyle = WindowStyle.SingleBorderWindow;

                // タイマー60FPSで始動
                DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Background);
                timer.Interval = TimeSpan.FromSeconds((double)1 / 60);
                timer.Tick += (x, s) =>
                {
                    TimerAction60FPS();
                    KeepInterval(timer);
                };
                this.Closing += (x, s) => { timer.Stop(); };
                timer.Start();

                this.FadeOut = true;

                this.delegateMainWindowContentRendered = MainWindowContentRendered;

                this.FadeIn = true;

            }
            catch (Exception err)
            {
                MessageBox.Show("Error.Number is 2:" + Environment.NewLine + err.Message);
                throw;
            }
        }

        public static void KeepInterval(DispatcherTimer timer)
        {
            var now = DateTime.Now;
            var nowMilliseconds = now.TimeOfDay.TotalMilliseconds;
            // 16.6から、例えば24.9/16.6の余りである8.3を引くことで、
            // 次の実行が16.6から8.3となり結果的に1秒間60回実行が保たれる
            var timerInterval = constantInterval -
                                nowMilliseconds % constantInterval;
            timer.Interval = TimeSpan.FromMilliseconds(timerInterval);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.NowSituation = _010_Enum.Situation.GameStop;
        }

        /// <summary>
        /// タイトル画面に表示される難易度ボタンがクリックされた時の処理
        /// 押されたボタンから難易度(aに代入されている)を取得、シナリオ選択画面へ移行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void titleButton_click(Object sender, EventArgs e)
        {
            var target = (Button)sender;
            int a = Convert.ToInt32(target.Tag);
            //MessageBox.Show(a.ToString());
            this.DifficultyLevel = a;

            this.FadeOut = true;

            this.delegateMainWindowContentRendered = SetWindowMainMenu;

            this.FadeIn = true;
        }

        /// <summary>
        /// シナリオ選択画面でマウスが右クリックされた時の処理
        /// タイトル画面に戻る
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScenarioSelection_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            this.FadeOut = true;

            this.delegateMainWindowContentRendered = MainWindowContentRendered;

            this.FadeIn = true;
        }

        /// <summary>
        /// ボタン等を右クリックした際に、親コントロールが反応しないようにする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Disable_MouseEvent(object sender, MouseEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// シナリオ選択画面でいずれかのシナリオボタンが押された時の処理
        /// 押されたボタンに対応する番号(aに代入されている)のシナリオを呼び出す
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScenarioSelectionButton_click(Object sender, EventArgs e)
        {
            var target = (Button)sender;
            int a = Convert.ToInt32(target.Tag);
            //MessageBox.Show(a.ToString());
            this.NumberScenarioSelection = a;

            switch (this.ListClassScenarioInfo[a].ButtonType)
            {
                case ButtonType.Scenario:
                    break;
                case ButtonType.Mail:
                    {
                        var startInfo =
                            new System
                            .Diagnostics
                            .ProcessStartInfo("https://mail.google.com/mail/u/0/?tf=cm&fs=1&to=" +
                                                this.ListClassScenarioInfo[a].Mail +
                                                "&su=game%E3%81%AE%E4%BB%B6&body=%E3%81%B5%E3%82%8F%E3%81%B5%E3%82%8F%EF%BD%9E%E3%80%82%E3%82%B2%E3%83%BC%E3%83%A0%E3%81%AE%E4%BB%B6%E3%81%A7%E8%81%9E%E3%81%8D%E3%81%9F%E3%81%84%E3%81%AE%E3%81%A7%E3%81%99%E3%81%8C%E4%BB%A5%E4%B8%8B%E8%A8%98%E8%BF%B0");
                        startInfo.UseShellExecute = true;
                        System.Diagnostics.Process.Start(startInfo);
                        return;
                    }
                case ButtonType.Internet:
                    {
                        var startInfo = new System.Diagnostics.ProcessStartInfo(this.ListClassScenarioInfo[a].Internet);
                        startInfo.UseShellExecute = true;
                        System.Diagnostics.Process.Start(startInfo);
                        return;
                    }
                default:
                    break;
            }

            // シナリオ開始時にデータを初期化する
            InitializeGameData();

            this.FadeOut = true;

            this.NowSituation = Situation.SelectGroup;
            this.delegateMapRendered = SetMapStrategy;

            this.FadeIn = true;
        }

        private void WindowMainMenuLeftTop_MouseEnter(object sender, MouseEventArgs e)
        {
            {
                var ri = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMainMenuRightTop);
                if (ri != null)
                {
                    this.canvasMain.Children.Remove(ri);
                }
            }
            {
                var ri = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMainMenuRightUnder);
                if (ri != null)
                {
                    this.canvasMain.Children.Remove(ri);
                }
            }

            int tag = Convert.ToInt32(((Button)sender).Tag);

            // 右上
            {
                Canvas canvas = new Canvas();
                if (this.ListClassScenarioInfo[tag].ScenarioImageBool == true)
                {
                    canvas.Height = this.CanvasMainHeight / 2;
                }
                else
                {
                    canvas.Height = this.CanvasMainHeight;
                }
                canvas.Width = this.CanvasMainWidth / 2;
                canvas.Margin = new Thickness()
                {
                    Left = canvas.Width,
                    Top = 0
                };
                canvas.Name = StringName.windowMainMenuRightTop;
                canvas.MouseRightButtonDown += ScenarioSelection_MouseRightButtonDown;
                {
                    // 枠
                    var rectangleInfo = new Rectangle();
                    rectangleInfo.Fill = new SolidColorBrush(Color.FromRgb(190, 178, 175));
                    rectangleInfo.Height = canvas.Height;
                    rectangleInfo.Width = this.CanvasMainWidth / 2;
                    rectangleInfo.Stroke = new SolidColorBrush(Colors.Gray);
                    rectangleInfo.StrokeThickness = 5;
                    rectangleInfo.Margin = new Thickness()
                    {
                        Left = 0,
                        Top = 0,
                    };
                    canvas.Children.Add(rectangleInfo);

                    int fontSizePlus = 5;
                    TextBlock tbDate1 = new TextBlock();
                    tbDate1.FontSize = tbDate1.FontSize + fontSizePlus;

                    tbDate1.Text = this.ListClassScenarioInfo[tag].ScenarioIntroduce;
                    tbDate1.Height = canvas.Height;
                    tbDate1.Margin = new Thickness { Left = 15, Top = 15 };
                    canvas.Children.Add(tbDate1);
                }
                this.canvasMain.Children.Add(canvas);
            }

            if (this.ListClassScenarioInfo[tag].ScenarioImageBool == false)
            {
                return;
            }

            // 右下
            {
                Canvas canvas = new Canvas();
                canvas.Height = this.CanvasMainHeight / 2;
                canvas.Width = this.CanvasMainWidth / 2;
                canvas.Margin = new Thickness()
                {
                    Left = canvas.Width,
                    Top = canvas.Height
                };
                canvas.Name = StringName.windowMainMenuRightUnder;
                canvas.MouseRightButtonDown += ScenarioSelection_MouseRightButtonDown;
                canvas.MouseEnter += ImageScenarioSelection_MouseEnter;
                canvas.MouseLeave += ImageScenarioSelection_MouseLeave;
                canvas.Background = Brushes.Transparent;
                {
                    // 最初の画像を一枚だけ表示する

                    //gifをどこかで使うかもしれないので一応残す
                    //// 枠
                    //var rectangleInfo = new Rectangle();
                    //rectangleInfo.Fill = ReturnBaseColor();
                    //rectangleInfo.Height = canvas.Height;
                    //rectangleInfo.Width = this.CanvasMainWidth / 2;
                    //rectangleInfo.Stroke = new SolidColorBrush(Colors.Gray);
                    //rectangleInfo.StrokeThickness = 5;
                    //rectangleInfo.Margin = new Thickness()
                    //{
                    //    Left = 0,
                    //    Top = 0,
                    //};
                    //canvas.Children.Add(rectangleInfo);
                    //image.Stretch = Stretch.Fill;
                    //ImageBehavior.SetAnimatedSource(image, bi);

                    // get target path.
                    List<string> strings = new List<string>();
                    strings.Add(ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName);
                    strings.Add("005_BackgroundImage");
                    strings.Add("005_MenuImage");
                    strings.Add(this.ListClassScenarioInfo[tag].ScenarioImage);
                    string path = System.IO.Path.Combine(strings.ToArray());

                    var bi = new BitmapImage(new Uri(path));
                    Image image = new Image();
                    image.Width = canvas.Width;
                    image.Height = canvas.Height;
                    // アスペクト比を維持する。
                    image.Stretch = Stretch.Uniform;
                    image.Source = bi;
                    image.Name = "ScenarioImage";
                    image.Tag = tag;
                    canvas.Children.Add(image);
                }
                this.canvasMain.Children.Add(canvas);
            }

        }

        // シナリオ選択画面で右下キャンバスにマウス・カーソルを乗せた時
        private void ImageScenarioSelection_MouseEnter(object sender, MouseEventArgs e)
        {
            var image = (Image)LogicalTreeHelper.FindLogicalNode((Canvas)sender, "ScenarioImage");
            if (image != null)
            {
                // get target path.
                List<string> strings = new List<string>();
                strings.Add(ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName);
                strings.Add("005_BackgroundImage");
                strings.Add("005_MenuImage");
                strings.Add(this.ListClassScenarioInfo[Convert.ToInt32(image.Tag)].ScenarioImage);
                string base_path = System.IO.Path.Combine(strings.ToArray());
                base_path = System.IO.Path.ChangeExtension(base_path, string.Empty);
                base_path = base_path.Substring(0, base_path.Length - 1);

                ObjectAnimationUsingKeyFrames animation = new ObjectAnimationUsingKeyFrames();

                // 最初のフレームは指定された画像 (image.Source) を使う
                DiscreteObjectKeyFrame key0 = new DiscreteObjectKeyFrame();
                key0.KeyTime = TimeSpan.Zero;
                key0.Value = image.Source;
                animation.KeyFrames.Add(key0);

                // 次のフレームからは連番で探す（最大99枚まで、多いと重くなる？）
                int frame_span = 1000;
                int num;
                for (num = 1; num < 100; num++)
                {
                    string number_path = base_path + num;

                    // JPG, PNG, GIF の順番に探す
                    string find_path = number_path + ".jpg";
                    if (System.IO.File.Exists(find_path) == false)
                    {
                        find_path = number_path + ".png";
                        if (System.IO.File.Exists(find_path) == false)
                        {
                            find_path = number_path + ".gif";
                            if (System.IO.File.Exists(find_path) == false)
                            {
                                break;
                            }
                        }
                    }
                    BitmapImage bi = new BitmapImage(new Uri(find_path));

                    DiscreteObjectKeyFrame key = new DiscreteObjectKeyFrame();
                    key.KeyTime = new TimeSpan(0, 0, 0, 0, frame_span * num);
                    key.Value = bi;
                    animation.KeyFrames.Add(key);
                }
                // 他の画像を見つけた時だけアニメーションを開始する
                if (num > 1)
                {
                    animation.RepeatBehavior = RepeatBehavior.Forever;
                    animation.Duration = new TimeSpan(0, 0, 0, 0, frame_span * num);
                    image.BeginAnimation(Image.SourceProperty, animation);
                }
            }
        }

        // シナリオ選択画面で右下キャンバスからマウス・カーソルを離した時
        private void ImageScenarioSelection_MouseLeave(object sender, MouseEventArgs e)
        {
            var image = (Image)LogicalTreeHelper.FindLogicalNode((Canvas)sender, "ScenarioImage");
            if (image != null)
            {
                // アニメーションを取り除く
                image.BeginAnimation(Image.SourceProperty, null);

                // 最初に表示してた画像 (image.Source) に戻る
            }
        }

        /// <summary>
        /// 勢力選択画面での勢力情報表示
        /// 決定ボタン押下時「ButtonSelectionPowerDecide_click」
        /// </summary>
        /// <param name="sender"></param>
        public void DisplayPowerSelection(object sender)
        {
            var cast = (FrameworkElement)sender;
            if (cast.Tag is not ClassPowerAndCity)
            {
                return;
            }

            var classPowerAndCity = (ClassPowerAndCity)cast.Tag;
            if (classPowerAndCity.ClassPower.MasterTag == string.Empty)
            {
                return; //選択領地のマスター名が空なら、勢力情報画面を表示しない。
            }

            {
                var windowSelectionPower = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowSelectionPower);
                if (windowSelectionPower != null)
                {
                    this.canvasMain.Children.Remove(windowSelectionPower);
                }
            }


            {
                // 勢力一覧ウィンドウを消す
                var ri2 = (UserControl040_PowerSelect)LogicalTreeHelper.FindLogicalNode(this.canvasUIRightTop, StringName.windowSelectionPowerMini);
                if (ri2 != null)
                {
                    this.canvasUIRightTop.Children.Remove(ri2);
                }
            }

            //データの構造的に、人数も指定しないと駄目だったのかも。
            var result = this.ClassGameStatus.AllListSpot.Where(x => x.ListMember.Contains(new(classPowerAndCity.ClassPower.MasterTag, 1))).FirstOrDefault();
            if (result != null)
            {
                this.ClassGameStatus.SelectionCityPoint = new Point
                    (
                    result.X,
                    result.Y
                    );
            }
            else
            {
                this.ClassGameStatus.SelectionCityPoint = new Point
                    (
                    classPowerAndCity.ClassSpot.X,
                    classPowerAndCity.ClassSpot.Y
                    );
            }

            int spaceMargin = 5;

            Canvas canvas = new Canvas();
            canvas.Background = Brushes.Transparent;
            canvas.Height = this.ClassConfigGameTitle.WindowSelectionPowerLeftTop.Y + this.ClassConfigGameTitle.WindowSelectionPowerUnit.Y + spaceMargin * 5;
            canvas.Width = this.ClassConfigGameTitle.WindowSelectionPowerLeftTop.X + this.ClassConfigGameTitle.WindowSelectionPowerImage.X + spaceMargin * 5;
            canvas.Margin = new Thickness()
            {
                Left = (this.CanvasMainWidth - canvas.Width) / 2,
                Top = (this.CanvasMainHeight - canvas.Height) / 2
            };
            canvas.Name = StringName.windowSelectionPower;
            canvas.MouseRightButtonDown += SelectionPower_MouseRightButtonDown;
            {
                //LeftTop
                {
                    Grid gridSelectionPower = new Grid();
                    gridSelectionPower.Background = Brushes.DarkGray;
                    gridSelectionPower.Height = this.ClassConfigGameTitle.WindowSelectionPowerLeftTop.Y;
                    gridSelectionPower.Width = this.ClassConfigGameTitle.WindowSelectionPowerLeftTop.X;
                    gridSelectionPower.Margin = new Thickness()
                    {
                        Left = 0,
                        Top = 0
                    };
                    gridSelectionPower.VerticalAlignment = VerticalAlignment.Top;
                    gridSelectionPower.HorizontalAlignment = HorizontalAlignment.Left;

                    int fontSizePlus = 5;
                    int hei = 15;
                    int face = 96;
                    int groupHeight = 80;
                    int head = 40;
                    int buttonHeight = 80;
                    int textHeight = 600;

                    //国名テキストボックス
                    {
                        TextBlock tbDate1 = new TextBlock();
                        tbDate1.HorizontalAlignment = HorizontalAlignment.Left;
                        tbDate1.VerticalAlignment = VerticalAlignment.Top;
                        tbDate1.FontSize = tbDate1.FontSize + fontSizePlus + 10;
                        tbDate1.Text = classPowerAndCity.ClassPower.Name;
                        tbDate1.Height = groupHeight;
                        tbDate1.Margin = new Thickness { Left = 15, Top = hei };
                        gridSelectionPower.Children.Add(tbDate1);
                    }

                    //主役ボタン
                    {
                        Button button = new Button();
                        var ima = this.ClassGameStatus.ListUnit.Where(x => x.NameTag == classPowerAndCity.ClassPower.MasterTag).FirstOrDefault();
                        Image img = new Image();
                        img.Stretch = Stretch.Fill;
                        if (ima != null)
                        {
                            List<string> strings = new List<string>();
                            strings.Add(ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName);
                            strings.Add("010_FaceImage");
                            strings.Add(ima.Face);
                            string path = System.IO.Path.Combine(strings.ToArray());
                            BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                            img.Source = bitimg1;
                        }

                        button.HorizontalAlignment = HorizontalAlignment.Left;
                        button.VerticalAlignment = VerticalAlignment.Top;
                        button.Content = img;
                        button.Height = face;
                        button.Width = face;
                        button.Margin = new Thickness
                        {
                            Left = 15,
                            Top = hei + groupHeight
                        };
                        button.BorderBrush = Brushes.AliceBlue;
                        button.BorderThickness = new Thickness() { Left = 3, Top = 3, Right = 3, Bottom = 3 };
                        gridSelectionPower.Children.Add(button);
                    }

                    //称号
                    {
                        TextBlock tbDate1 = new TextBlock();
                        tbDate1.HorizontalAlignment = HorizontalAlignment.Left;
                        tbDate1.VerticalAlignment = VerticalAlignment.Top;
                        tbDate1.FontSize = tbDate1.FontSize + fontSizePlus;
                        tbDate1.Text = classPowerAndCity.ClassPower.Head;
                        tbDate1.Height = head;
                        tbDate1.Margin = new Thickness { Left = 15, Top = hei + groupHeight + face };
                        gridSelectionPower.Children.Add(tbDate1);
                    }

                    //決定ボタン
                    {
                        Button button = new Button();
                        if (classPowerAndCity.ClassPower.EnableSelect == "off")
                        {
                            button.IsEnabled = false;
                        }
                        button.HorizontalAlignment = HorizontalAlignment.Left;
                        button.VerticalAlignment = VerticalAlignment.Top;
                        button.Content = "決定";
                        button.Height = buttonHeight;
                        button.Width = 80;
                        button.Margin = new Thickness
                        {
                            Left = 15,
                            Top = hei + groupHeight + face + head
                        };
                        button.Click += ButtonSelectionPowerDecide_click;
                        button.Tag = classPowerAndCity;
                        gridSelectionPower.Children.Add(button);
                    }

                    //取り消しボタン
                    {
                        Button button = new Button();
                        button.HorizontalAlignment = HorizontalAlignment.Left;
                        button.VerticalAlignment = VerticalAlignment.Top;
                        button.Content = "取り消し";
                        button.Height = buttonHeight;
                        button.Width = 80;
                        button.Margin = new Thickness
                        {
                            Left = 15,
                            Top = hei + groupHeight + face + head + buttonHeight
                        };
                        button.Click += ButtonSelectionPowerRemove_click;
                        gridSelectionPower.Children.Add(button);
                    }

                    //text
                    {
                        TextBlock tbDate1 = new TextBlock();
                        tbDate1.HorizontalAlignment = HorizontalAlignment.Left;
                        tbDate1.VerticalAlignment = VerticalAlignment.Top;
                        tbDate1.FontSize = tbDate1.FontSize + fontSizePlus;
                        tbDate1.Text = classPowerAndCity.ClassPower.Text;
                        tbDate1.TextWrapping = TextWrapping.Wrap;
                        tbDate1.Height = textHeight;
                        tbDate1.Width = 380;
                        tbDate1.Margin = new Thickness { Left = 0, Top = 0 };

                        ScrollViewer scrollViewer = new ScrollViewer();
                        scrollViewer.Content = tbDate1;
                        scrollViewer.Margin = new Thickness { Left = face + 15 + 15, Top = hei + groupHeight };
                        gridSelectionPower.Children.Add(scrollViewer);
                    }

                    var bor = new Border();
                    bor.BorderBrush = Brushes.Black;
                    bor.BorderThickness = new Thickness() { Left = 5, Top = 5, Right = 5, Bottom = 5 };
                    bor.Child = gridSelectionPower;
                    canvas.Children.Add(bor);
                }

                //LeftBottom
                {
                    Grid gridSelectionPower = new Grid();
                    gridSelectionPower.Background = new SolidColorBrush(Color.FromRgb(38, 38, 38));
                    gridSelectionPower.Height = this.ClassConfigGameTitle.WindowSelectionPowerUnit.Y;
                    gridSelectionPower.Width = this.ClassConfigGameTitle.WindowSelectionPowerUnit.X;
                    gridSelectionPower.Margin = new Thickness()
                    {
                        Left = 0,
                        Top = 0
                    };
                    gridSelectionPower.VerticalAlignment = VerticalAlignment.Top;
                    gridSelectionPower.HorizontalAlignment = HorizontalAlignment.Left;

                    //勢力選択画面なのでListInitMemberでOK
                    foreach (var item in classPowerAndCity.ClassPower.ListInitMember)
                    {
                        var ext = this.ClassGameStatus.AllListSpot.Where(x => x.NameTag == item);
                        foreach (var itemSpot in ext)
                        {
                            foreach (var itemMember in itemSpot.ListMember.Select((value, index) => (value, index)))
                            {
                                var unit = this.ClassGameStatus.ListUnit.Where(x => x.NameTag == itemMember.value.Item1 && x.Talent == "on").FirstOrDefault();
                                if (unit == null)
                                {
                                    continue;
                                }

                                Canvas canvasUnit = new Canvas();
                                canvasUnit.Background = Brushes.Black;
                                canvasUnit.Height = 70;
                                canvasUnit.Width = this.ClassConfigGameTitle.WindowSelectionPowerUnit.X - 30;
                                canvasUnit.Margin = new Thickness() { Left = 0, Top = (itemMember.index * 80) + 10 };
                                canvasUnit.HorizontalAlignment = HorizontalAlignment.Left;
                                canvasUnit.VerticalAlignment = VerticalAlignment.Top;

                                //画像
                                {
                                    Polygon polygon001 = new Polygon();
                                    List<Point> points = new List<Point>();
                                    points.Add(new Point { X = 0, Y = 15 });
                                    points.Add(new Point { X = 0, Y = 45 });
                                    //points.Add(new Point { X = 15, Y = 60 });
                                    points.Add(new Point { X = 15, Y = 60 });
                                    points.Add(new Point { X = 60, Y = 45 });
                                    points.Add(new Point { X = 60, Y = 15 });
                                    //points.Add(new Point { X = 45, Y = 0 });
                                    points.Add(new Point { X = 45, Y = 0 });
                                    polygon001.Points = new PointCollection(points);
                                    //polygon001.Stroke = Brushes.Brown;
                                    polygon001.Stroke = Brushes.DarkBlue;
                                    //polygon001.Stroke = Brushes.RosyBrown;
                                    //polygon001.Stroke = Brushes.SaddleBrown;
                                    //polygon001.Stroke = Brushes.SandyBrown;
                                    polygon001.StrokeThickness = 4;
                                    polygon001.Margin = new Thickness { Top = 0, Left = 0 };

                                    List<string> strings = new List<string>();
                                    strings.Add(ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName);
                                    strings.Add("010_FaceImage");
                                    strings.Add(unit.Face);
                                    string path = System.IO.Path.Combine(strings.ToArray());

                                    var bi = new BitmapImage(new Uri(path));
                                    ImageBrush imageBrush = new ImageBrush();
                                    imageBrush.ImageSource = bi;

                                    polygon001.Fill = imageBrush;
                                    polygon001.HorizontalAlignment = HorizontalAlignment.Left;
                                    polygon001.VerticalAlignment = VerticalAlignment.Top;
                                    canvasUnit.Children.Add(polygon001);
                                }
                                //名前
                                {
                                    TextBlock textBlock = new TextBlock();
                                    textBlock.Text = unit.Name + "(" + unit.Race + ")" + ":" + unit.Class;
                                    textBlock.Foreground = Brushes.White;
                                    textBlock.Height = 35;
                                    textBlock.Width = this.ClassConfigGameTitle.WindowSelectionPowerUnit.X;
                                    textBlock.Margin = new Thickness() { Left = 75, Top = 15 };
                                    canvasUnit.Children.Add(textBlock);
                                }
                                //ヘルプ
                                {
                                    TextBlock textBlock = new TextBlock();
                                    textBlock.Text = unit.Help;
                                    textBlock.Foreground = Brushes.White;
                                    textBlock.Height = 35;
                                    textBlock.Width = this.ClassConfigGameTitle.WindowSelectionPowerUnit.X;
                                    textBlock.Margin = new Thickness() { Left = 75, Top = 50 };
                                    canvasUnit.Children.Add(textBlock);
                                }

                                ScrollViewer scrollViewer = new ScrollViewer();
                                scrollViewer.Content = canvasUnit;
                                scrollViewer.Margin = new Thickness { Left = 0, Top = 0 };

                                gridSelectionPower.Children.Add(scrollViewer);
                            }
                        }
                    }

                    var bor = new Border();
                    bor.BorderBrush = Brushes.Black;
                    bor.BorderThickness = new Thickness() { Left = 5, Top = 5, Right = 5, Bottom = 5 };
                    bor.Child = gridSelectionPower;
                    bor.Margin = new Thickness() { Left = 0, Top = this.ClassConfigGameTitle.WindowSelectionPowerLeftTop.Y + spaceMargin * 3 };
                    canvas.Children.Add(bor);
                }

                //Right
                {
                    Grid gridSelectionPower = new Grid();
                    gridSelectionPower.Background = Brushes.DarkRed;
                    gridSelectionPower.Height = this.ClassConfigGameTitle.WindowSelectionPowerImage.Y + spaceMargin * 2;
                    gridSelectionPower.Width = this.ClassConfigGameTitle.WindowSelectionPowerImage.X;
                    gridSelectionPower.Margin = new Thickness()
                    {
                        Left = 0,
                        Top = 0
                    };
                    gridSelectionPower.VerticalAlignment = VerticalAlignment.Top;
                    gridSelectionPower.HorizontalAlignment = HorizontalAlignment.Left;

                    if (classPowerAndCity.ClassPower.Image == string.Empty)
                    {

                    }
                    else
                    {
                        BitmapImage bitimg1 = new BitmapImage(new Uri(classPowerAndCity.ClassPower.Image));
                        Image img = new Image();
                        img.Stretch = Stretch.Fill;
                        img.Source = bitimg1;
                        gridSelectionPower.Children.Add(img);
                    }

                    var bor = new Border();
                    bor.BorderBrush = Brushes.Black;
                    bor.BorderThickness = new Thickness() { Left = 5, Top = 5, Right = 5, Bottom = 5 };
                    bor.Child = gridSelectionPower;
                    bor.Margin = new Thickness()
                    {
                        Left = this.ClassConfigGameTitle.WindowSelectionPowerLeftTop.X + spaceMargin + spaceMargin + spaceMargin,
                        Top = 0
                    };
                    canvas.Children.Add(bor);
                }
            }

            this.canvasMain.Children.Add(canvas);
        }
        /// <summary>
        /// 勢力選択画面で決定押した時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSelectionPowerDecide_click(object sender, EventArgs e)
        {
            this.FadeOut = true;

            this.NowSituation = Situation.PlayerTurn;
            this.delegateNewGame = NewGameWithButtonClick;
            this.ClassGameStatus.SelectionPowerAndCity = ((ClassPowerAndCity)((Button)sender).Tag);
            foreach (var itemSpot in this.ClassGameStatus.AllListSpot)
            {
                // 中立領地にモンスターをランダム配置する（初期メンバーが指定されてる場合は除外する）
                if ((itemSpot.PowerNameTag == string.Empty) && (itemSpot.UnitGroup.Count() == 0))
                {
                    // ListWanderingMonster と ListMonster の仕様が不明なので、元のままにしておく。
                    // ヴァーレンだと、指定された比率と制限に準じて、部隊数とメンバー数をランダムに決める。
                    foreach (var ListWanderingMonster in itemSpot.ListWanderingMonster)
                    {
                        var info = this.ClassGameStatus.ListUnit.Where(x => x.NameTag == ListWanderingMonster.Item1).FirstOrDefault();
                        if (info == null)
                        {
                            continue;
                        }
                        var classUnit = new List<ClassUnit>();
                        for (int i = 0; i < ListWanderingMonster.Item2; i++)
                        {
                            var deep = info.DeepCopy();
                            deep.ID = this.ClassGameStatus.IDCount;
                            this.ClassGameStatus.SetIDCount();
                            classUnit.Add(deep);
                        }

                        itemSpot.UnitGroup.Add(new ClassHorizontalUnit() { Spot = itemSpot, FlagDisplay = true, ListClassUnit = classUnit });
                    }
                }
            }

            //var ri = (Button)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.buttonClassPowerAndCity);
            //if (ri == null)
            //{
            //    throw new Exception();
            //}
            //ri.Tag = new ClassPowerAndCity(this.ClassGameStatus.SelectionPowerAndCity.ClassPower, this.ClassGameStatus.SelectionPowerAndCity.ClassSpot);

            this.timerAfterFadeIn = new DispatcherTimer(DispatcherPriority.Background);
            this.timerAfterFadeIn.Interval = TimeSpan.FromSeconds((double)1 / 60);
            this.timerAfterFadeIn.Tick += (x, s) =>
            {
                TimerAction60FPSAfterFadeInDecidePower();
                KeepInterval(this.timerAfterFadeIn);
            };
            this.timerAfterFadeIn.Start();

            this.FadeIn = true;
        }

        private void NewGameWithButtonClick()
        {
            var windowSelectionPower = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowSelectionPower);
            if (windowSelectionPower != null)
            {
                this.canvasMain.Children.Remove(windowSelectionPower);
            }
        }

        /// <summary>
        /// 勢力選択画面で取り消しした時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSelectionPowerRemove_click(object sender, EventArgs e)
        {
            var windowSelectionPower = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowSelectionPower);
            if (windowSelectionPower != null)
            {
                this.canvasMain.Children.Remove(windowSelectionPower);
            }

            var ri2 = (UserControl040_PowerSelect)LogicalTreeHelper.FindLogicalNode(this.canvasUIRightTop, StringName.windowSelectionPowerMini);
            if (ri2 == null)
            {
                // 勢力一覧ウィンドウを出す
                ListSelectionPowerMini();
            }
        }

        /// <summary>
        /// 勢力情報キャンバスを右クリックしたら閉じるようにする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionPower_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            ButtonSelectionPowerRemove_click(sender, e);

            e.Handled = true;
        }

        #endregion

        #region Method

        /// <summary>
        /// タイトル画面を作成して表示し、BGMを流す
        /// </summary>
        public void MainWindowContentRendered()
        {
            //// xml存在確認
            //string fileName = this._pathConfigFile;
            //if (File.Exists(fileName) == false)
            //{
            //    // 無ければ作る(初期データ作成)
            //    Config config = new Config();
            //    {
            //        var props = typeof(Config).GetProperties().Where(p => p.Name.StartsWith("ClearDay"));
            //        foreach (var p in props)
            //        {
            //            p.SetValue(config, false);
            //        }
            //    }
            //    XmlSerializer serializer = new XmlSerializer(config.GetType());
            //    using (FileStream fs = new FileStream(fileName, FileMode.Create))
            //        serializer.Serialize(fs, config);
            //    using (FileStream fs = new FileStream(fileName, FileMode.Open))
            //        this.config = (Config)serializer.Deserialize(fs);
            //}
            //else
            //{
            //    // 有れば読み込む
            //    this.config = new Config();
            //    XmlSerializer serializer = new XmlSerializer(this.config.GetType());
            //    using (FileStream fs = new FileStream(fileName, FileMode.Open))
            //        this.config = (Config)serializer.Deserialize(fs);
            //}

            SetWindowTitle(targetNumber: 0);
        }

        /// <summary>
        /// 勢力一覧ウィンドウの表示
        /// </summary>
        private void ListSelectionPowerMini()
        {
            var windowSelect = new UserControl040_PowerSelect();
            windowSelect.Name = StringName.windowSelectionPowerMini;
            windowSelect.Margin = new Thickness()
            {
                Left = this.CanvasMainWidth - windowSelect.MinWidth,
                Top = 0
            };
            windowSelect.SetData();
            this.canvasUIRightTop.Children.Add(windowSelect);
        }

        /// <summary>
        /// 戦闘画面を作成する処理
        /// </summary>
        public void SetBattleMap()
        {
            // 開いてる子ウインドウを全て閉じる
            this.canvasUI.Children.Clear();
            this.canvasUIRightBottom.Children.Clear();

            //マップそのもの
            Canvas canvas = new Canvas();
            int takasaMapTip = ClassStaticBattle.TakasaMapTip;
            int yokoMapTip = ClassStaticBattle.yokoMapTip;
            canvas.Name = StringName.windowMapBattle;
            canvas.Background = Brushes.Black;
            canvas.MouseLeftButtonDown += CanvasMapBattle_MouseLeftButtonDown;
            canvas.MouseRightButtonDown += windowMapBattle_MouseRightButtonDown;
            {
                if (ClassGameStatus.ClassBattle.ClassMapBattle == null)
                {
                    //キャンバス設定
                    {
                        int BaseNum = 600;
                        canvas.Width = BaseNum
                                        + (BaseNum / 2);
                        canvas.Height = BaseNum;
                        canvas.Margin = new Thickness()
                        {
                            Left = BaseNum / 2,
                            Top = (this._sizeClientWinHeight / 2) - (canvas.Height / 2)
                        };
                    }
                }
                else
                {
                    //キャンバス設定
                    {
                        canvas.Width = this.ClassGameStatus.ClassBattle.ClassMapBattle.MapData[0].Count * yokoMapTip;
                        canvas.Height = this.ClassGameStatus.ClassBattle.ClassMapBattle.MapData.Count * takasaMapTip;
                        canvas.Margin = new Thickness()
                        {
                            Left = ((
                                    (this.CanvasMainWidth / 2) - (this._sizeClientWinWidth / 2)
                                    ))
                                        +
                                    (this._sizeClientWinWidth / 2) - ((this.ClassGameStatus.ClassBattle.ClassMapBattle.MapData[0].Count * 32) / 2),
                            Top = (this._sizeClientWinHeight / 2) - (canvas.Height / 2)
                        };
                        //RotateTransform rotateTransform2 = new RotateTransform(0);
                        ////rotateTransform2.CenterX = 25;
                        ////rotateTransform2.CenterY = 50;
                        //canvas.RenderTransform = rotateTransform2;
                    }

                    // get files.
                    IEnumerable<string> files = ClassStaticBattle.GetFiles015_BattleMapCellImage(ClassConfigGameTitle.DirectoryGameTitle[NowNumberGameTitle].FullName);
                    Dictionary<string, string> map = new Dictionary<string, string>();
                    foreach (var item in files)
                    {
                        map.Add(System.IO.Path.GetFileNameWithoutExtension(item), item);
                    }

                    //Path描写
                    //double naname = Math.Sqrt((48 / 2) * (48 / 2)) + ((16) * (16));
                    List<(BitmapImage, int, int)> listTakaiObj = new List<(BitmapImage, int, int)>();
                    foreach (var itemCol in ClassGameStatus.ClassBattle.ClassMapBattle.MapData
                                            .Select((value, index) => (value, index)))
                    {
                        foreach (var itemRow in itemCol.value.Select((value, index) => (value, index)))
                        {
                            map.TryGetValue(itemRow.value.Tip, out string? value);
                            if (value == null) continue;

                            //RotateTransform rotateTransform2 = new RotateTransform(45);
                            //rotateTransform2.CenterX = 25;
                            //rotateTransform2.CenterY = 50;
                            //image.RelativeTransform = rotateTransform2;

                            if (itemRow.value.Building.Count != 0)
                            {
                                foreach (var building in itemRow.value.Building)
                                {
                                    map.TryGetValue(building, out string? value2);
                                    if (value2 != null)
                                    {
                                        var build = new BitmapImage(new Uri(value2));
                                        listTakaiObj.Add(new(build, itemCol.index, itemRow.index));
                                    }
                                }
                            }

                            var bi = new BitmapImage(new Uri(value));
                            ImageBrush image = new ImageBrush();
                            image.Stretch = Stretch.Fill;
                            image.ImageSource = bi;
                            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
                            path.Fill = image;
                            ClassBattleMapPath classBattleMapPath = new ClassBattleMapPath();
                            classBattleMapPath.Col = itemCol.index;
                            classBattleMapPath.Row = itemRow.index;
                            if (itemRow.value.BoueiButaiNoIti == true)
                            {
                                classBattleMapPath.KougekiOrBouei = "Bouei";
                                path.Tag = classBattleMapPath;
                            }
                            if (itemRow.value.KougekiButaiNoIti == true)
                            {
                                classBattleMapPath.KougekiOrBouei = "Kougeki";
                                path.Tag = classBattleMapPath;
                            }
                            path.Name = "a" + itemCol.index + "a" + itemRow.index;
                            path.Stretch = Stretch.Fill;
                            path.StrokeThickness = 0;
                            path.Data = Geometry.Parse("M 0," + takasaMapTip / 2
                                                    + " L " + yokoMapTip / 2 + "," + takasaMapTip
                                                    + " L " + yokoMapTip + "," + takasaMapTip / 2
                                                    + " L " + yokoMapTip / 2 + ",0 Z");
                            path.Margin = new Thickness()
                            {
                                Left = (itemCol.index * (yokoMapTip / 2)) + (itemRow.index * (yokoMapTip / 2)),
                                Top =
                                    ((canvas.Height / 2) // マップ半分の高さ
                                    + (itemCol.index * (takasaMapTip / 2))
                                    + (itemRow.index * (-(takasaMapTip / 2)))) // マイナスになる 
                                    - takasaMapTip / 2
                            };
                            canvas.Children.Add(path);
                            itemRow.value.MapPath = path;
                        }
                    }

                    //建築物描写
                    ClassStaticBattle.DisplayBuilding(canvas, takasaMapTip, yokoMapTip, listTakaiObj, ClassGameStatus.ClassBattle.ListBuildingAlive);

                    //建築物論理描写
                    //こちらを後でやる。クリックで爆破が出来るように
                    var bui = ClassGameStatus.ClassBattle.DefUnitGroup
                                .Where(x => x.FlagBuilding == true)
                                .First();
                    foreach (var item in bui.ListClassUnit)
                    {
                        ClassUnitBuilding classUnitBuilding = (ClassUnitBuilding)item;
                        var target = listTakaiObj.Where(x => x.Item2 == classUnitBuilding.X && x.Item3 == classUnitBuilding.Y).FirstOrDefault();
                        if (target == (null, null, null)) continue;

                        System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
                        path.Stretch = Stretch.Fill;
                        path.StrokeThickness = 0;
                        path.Data = Geometry.Parse("M 0," + takasaMapTip / 2
                                                + " L " + yokoMapTip / 2 + "," + takasaMapTip
                                                + " L " + yokoMapTip + "," + takasaMapTip / 2
                                                + " L " + yokoMapTip / 2 + ",0 Z");
                        path.PreviewMouseLeftButtonDown += WindowMapBattleUnit_MouseLeftButtonDown;
                        path.Name = "Chip" + item.ID.ToString();
                        path.Tag = item.ID.ToString();

                        path.Margin = new Thickness()
                        {
                            Left = (target.Item2 * (yokoMapTip / 2)) + (target.Item3 * (yokoMapTip / 2)),
                            Top = ((canvas.Height / 2) + (target.Item2 * (takasaMapTip / 2)) + (target.Item3 * (-(takasaMapTip / 2)))) - takasaMapTip / 2
                        };
                        classUnitBuilding.NowPosiLeft = new Point(path.Margin.Left, path.Margin.Top);
                        canvas.Children.Add(path);
                    }
                }

                this.canvasMain.Children.Add(
                    SetAndGetCanvasBattleBack(canvas,
                                        this._sizeClientWinWidth,
                                        this._sizeClientWinHeight,
                                        this.CanvasMainWidth,
                                        this.CanvasMainHeight)
                    );
            }

            ////出撃ユニット
            {
                //中点
                decimal countMeHalf = Math.Floor((decimal)ClassGameStatus.ClassBattle.SortieUnitGroup.Count / 2);
                //線の端
                Point hidariTakasa = new Point(0, canvas.Height / 2);
                Point migiTakasa = new Point(canvas.Width / 2, canvas.Height);
                for (int i = 0; i < canvas.Children.Count; i++)
                {
                    if (canvas.Children[i] is System.Windows.Shapes.Path ppp)
                    {
                        string? taggg = Convert.ToString(ppp.Tag);
                        if (taggg != null)
                        {
                            if (taggg == "Kougeki")
                            {
                                //線分A の中点 C は、Xc = (X1+X2)÷2, Yc = (Y1+Y2)÷2 で求まる
                                //なので、線分A (X1, Y1)-(X2, Y2) の中点となる(Xc, Yc)と、
                                //目標点P(Xp, Yp) とのズレを算出

                                //中点Cを求めて、点Pから中点Cを引き、結果のXとYを線AのXとYに加算

                                //xxx = ppp.Margin.Left;
                                //xxx = ppp.Margin.Top;
                            }
                        }
                    }
                }
                //ユニットの端の位置を算出
                if (ClassGameStatus.ClassBattle.SortieUnitGroup.Count % 2 == 0)
                {
                    ////偶数
                    //これは正しくないが、案が思い浮かばない
                    hidariTakasa.X = (migiTakasa.X / 2) - ((double)countMeHalf * 32);
                    migiTakasa.X = (migiTakasa.X / 2) + ((double)countMeHalf * 32);

                    hidariTakasa.Y = (migiTakasa.Y * 0.75) - ((double)countMeHalf * (takasaMapTip / 2));
                    migiTakasa.Y = (migiTakasa.Y * 0.75) + ((double)countMeHalf * (takasaMapTip / 2));
                }
                else
                {
                    ////奇数
                    //これは正しくないが、案が思い浮かばない
                    hidariTakasa.X = (migiTakasa.X / 2) - (((double)countMeHalf + 1) * 32);
                    migiTakasa.X = (migiTakasa.X / 2) + (((double)countMeHalf + 1) * 32);

                    hidariTakasa.Y = (migiTakasa.Y * 0.75) - (((double)countMeHalf + 1) * (takasaMapTip / 2));
                    migiTakasa.Y = (migiTakasa.Y * 0.75) + (((double)countMeHalf + 1) * (takasaMapTip / 2));
                }

                //出撃前衛
                foreach (var item in this.ClassGameStatus
                            .ClassBattle.SortieUnitGroup
                            .Where(x => x.ListClassUnit[0].Formation.Formation == Formation.F))
                {
                    //比率
                    Point hiritu = new Point()
                    {
                        X = item.ListClassUnit.Count - 1,
                        Y = 0
                    };

                    foreach (var itemListClassUnit in item.ListClassUnit.Select((value, index) => (value, index)))
                    {
                        string path = ClassStaticBattle.GetPathTipImage(itemListClassUnit, this.ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName);

                        var bi = new BitmapImage(new Uri(path));
                        ImageBrush image = new ImageBrush();
                        image.Stretch = Stretch.Fill;
                        image.ImageSource = bi;
                        Button button = new Button();
                        button.Background = image;
                        button.Width = ClassStaticBattle.yokoUnit;
                        button.Height = ClassStaticBattle.TakasaUnit;
                        Canvas canvasChip = new Canvas();
                        //固有の情報
                        canvasChip.Name = "Chip" + itemListClassUnit.value.ID.ToString();
                        canvasChip.Tag = itemListClassUnit.value.ID.ToString();
                        if (ClassGameStatus.ClassBattle.BattleWhichIsThePlayer == BattleWhichIsThePlayer.Sortie)
                        {
                            canvasChip.PreviewMouseLeftButtonDown += WindowMapBattleUnit_MouseLeftButtonDown;
                        }
                        canvasChip.Children.Add(button);
                        canvasChip.Width = ClassStaticBattle.yokoUnit;
                        canvasChip.Height = ClassStaticBattle.TakasaUnit;
                        //内分点の公式
                        double left = (
                                        ((hiritu.X - itemListClassUnit.index) * hidariTakasa.X) + (itemListClassUnit.index * migiTakasa.X)
                                        )
                                        / (itemListClassUnit.index + (hiritu.X - itemListClassUnit.index));
                        double top = (
                                        ((hiritu.X - itemListClassUnit.index) * hidariTakasa.Y) + (itemListClassUnit.index * migiTakasa.Y)
                                        )
                                        / (itemListClassUnit.index + (hiritu.X - itemListClassUnit.index));
                        if (item.ListClassUnit.Count == 1)
                        {
                            left = (hidariTakasa.X + migiTakasa.X) / 2;
                            top = (hidariTakasa.Y + migiTakasa.Y) / 2;
                        }
                        //Border
                        Border border = new Border();
                        border.Name = "border" + itemListClassUnit.value.ID.ToString();
                        border.BorderThickness = new Thickness();
                        border.Child = canvasChip;
                        border.Margin = new Thickness()
                        {
                            Left = left,
                            Top = top - 192
                        };
                        itemListClassUnit.value.NowPosiLeft = new Point()
                        {
                            X = left,
                            Y = top - 192
                        };
                        itemListClassUnit.value.OrderPosiLeft = new Point()
                        {
                            X = left,
                            Y = top - 192
                        };

                        Canvas.SetZIndex(border, 99);
                        canvas.Children.Add(border);
                    }
                }
                //出撃中衛
                foreach (var item in this.ClassGameStatus
                            .ClassBattle.SortieUnitGroup
                            .Where(x => x.ListClassUnit[0].Formation.Formation == Formation.M))
                {
                    //比率
                    Point hiritu = new Point()
                    {
                        X = item.ListClassUnit.Count - 1,
                        Y = 0
                    };

                    foreach (var itemListClassUnit in item.ListClassUnit.Select((value, index) => (value, index)))
                    {
                        List<string> strings = new List<string>();
                        strings.Add(this.ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName);
                        strings.Add("040_ChipImage");
                        strings.Add(itemListClassUnit.value.Image);
                        string path = System.IO.Path.Combine(strings.ToArray());

                        var bi = new BitmapImage(new Uri(path));
                        ImageBrush image = new ImageBrush();
                        image.Stretch = Stretch.Fill;
                        image.ImageSource = bi;
                        Button button = new Button();
                        button.Background = image;
                        button.Width = 32;
                        button.Height = 32;

                        Canvas canvasChip = new Canvas();
                        //固有の情報
                        canvasChip.Name = "Chip" + itemListClassUnit.value.ID.ToString();
                        canvasChip.Tag = itemListClassUnit.value.ID.ToString();
                        if (ClassGameStatus.ClassBattle.BattleWhichIsThePlayer == BattleWhichIsThePlayer.Sortie)
                        {
                            canvasChip.PreviewMouseLeftButtonDown += WindowMapBattleUnit_MouseLeftButtonDown;
                        }
                        canvasChip.Children.Add(button);
                        canvasChip.Width = 32;
                        canvasChip.Height = 32;
                        //内分点の公式
                        double left = (
                                        ((hiritu.X - itemListClassUnit.index) * hidariTakasa.X) + (itemListClassUnit.index * migiTakasa.X)
                                        )
                                        / (itemListClassUnit.index + (hiritu.X - itemListClassUnit.index));
                        double top = (
                                        ((hiritu.X - itemListClassUnit.index) * hidariTakasa.Y) + (itemListClassUnit.index * migiTakasa.Y)
                                        )
                                        / (itemListClassUnit.index + (hiritu.X - itemListClassUnit.index));
                        if (item.ListClassUnit.Count == 1)
                        {
                            left = (hidariTakasa.X + migiTakasa.X) / 2;
                            top = (hidariTakasa.Y + migiTakasa.Y) / 2;
                        }
                        //Border
                        Border border = new Border();
                        border.Name = "border" + itemListClassUnit.value.ID.ToString();
                        border.BorderThickness = new Thickness();
                        border.Child = canvasChip;
                        border.Margin = new Thickness()
                        {
                            Left = left,
                            Top = top - 86
                        };
                        itemListClassUnit.value.NowPosiLeft = new Point()
                        {
                            X = left,
                            Y = top - 86
                        };
                        itemListClassUnit.value.OrderPosiLeft = new Point()
                        {
                            X = left,
                            Y = top - 86
                        };

                        Canvas.SetZIndex(border, 99);
                        canvas.Children.Add(border);
                    }
                }
                //出撃後衛
                foreach (var item in this.ClassGameStatus
                            .ClassBattle.SortieUnitGroup
                            .Where(x => x.ListClassUnit[0].Formation.Formation == Formation.B))
                {
                    //比率
                    Point hiritu = new Point()
                    {
                        X = item.ListClassUnit.Count - 1,
                        Y = 0
                    };

                    foreach (var itemListClassUnit in item.ListClassUnit.Select((value, index) => (value, index)))
                    {
                        List<string> strings = new List<string>();
                        strings.Add(this.ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName);
                        strings.Add("040_ChipImage");
                        strings.Add(itemListClassUnit.value.Image);
                        string path = System.IO.Path.Combine(strings.ToArray());

                        var bi = new BitmapImage(new Uri(path));
                        ImageBrush image = new ImageBrush();
                        image.Stretch = Stretch.Fill;
                        image.ImageSource = bi;
                        Button button = new Button();
                        button.Background = image;
                        button.Width = 32;
                        button.Height = 32;

                        Canvas canvasChip = new Canvas();
                        //固有の情報
                        canvasChip.Name = "Chip" + itemListClassUnit.value.ID.ToString();
                        canvasChip.Tag = itemListClassUnit.value.ID.ToString();
                        if (ClassGameStatus.ClassBattle.BattleWhichIsThePlayer == BattleWhichIsThePlayer.Sortie)
                        {
                            canvasChip.PreviewMouseLeftButtonDown += WindowMapBattleUnit_MouseLeftButtonDown;
                        }
                        canvasChip.Children.Add(button);
                        canvasChip.Width = 32;
                        canvasChip.Height = 32;
                        //内分点の公式
                        double left = (
                                        ((hiritu.X - itemListClassUnit.index) * hidariTakasa.X) + (itemListClassUnit.index * migiTakasa.X)
                                        )
                                        / (itemListClassUnit.index + (hiritu.X - itemListClassUnit.index));
                        double top = (
                                        ((hiritu.X - itemListClassUnit.index) * hidariTakasa.Y) + (itemListClassUnit.index * migiTakasa.Y)
                                        )
                                        / (itemListClassUnit.index + (hiritu.X - itemListClassUnit.index));
                        if (item.ListClassUnit.Count == 1)
                        {
                            left = (hidariTakasa.X + migiTakasa.X) / 2;
                            top = (hidariTakasa.Y + migiTakasa.Y) / 2;
                        }
                        //Border
                        Border border = new Border();
                        border.Name = "border" + itemListClassUnit.value.ID.ToString();
                        border.BorderThickness = new Thickness();
                        border.Child = canvasChip;
                        border.Margin = new Thickness()
                        {
                            Left = left,
                            Top = top
                        };
                        itemListClassUnit.value.NowPosiLeft = new Point()
                        {
                            X = left,
                            Y = top
                        };
                        itemListClassUnit.value.OrderPosiLeft = new Point()
                        {
                            X = left,
                            Y = top
                        };

                        Canvas.SetZIndex(border, 99);
                        canvas.Children.Add(border);
                    }
                }
            }

            ////防衛ユニット
            {
                //中点
                decimal countMeHalf = Math.Floor((decimal)ClassGameStatus.ClassBattle.DefUnitGroup.Count / 2);
                //線の端
                Point hidariTakasa = new Point(canvas.Width / 2, 0);
                Point migiTakasa = new Point(canvas.Width, canvas.Height / 2);
                var abc = canvas.Children.OfType<System.Windows.Shapes.Path>();
                foreach (var item in abc)
                {
                    ClassBattleMapPath? taggg = (item.Tag) as ClassBattleMapPath;
                    if (taggg == null) continue;
                    if (taggg.KougekiOrBouei == "Bouei")
                    {
                        //とある線(Shape.Line型)を、掲題の通り移動させたい。
                        //例えば、(X1 = 0, Y1 = 50, X2 = 50, Y2 = 100)の線Aと、点P(X = 30, Y = 30)があるとして、
                        //点Pの座標に線Aの中心を置きたい。(傾きはそのまま)

                        //hidariTakasa
                        //migiTakasa
                        //を変えるのが目的

                        //線分A の中点 C は、Xc = (X1+X2)÷2, Yc = (Y1+Y2)÷2 で求まる
                        //なので、線分A (X1, Y1)-(X2, Y2) の中点となる(Xc, Yc)と、
                        //目標点P(Xp, Yp) とのズレを算出

                        //中点Cを求めて、点Pから中点Cを引き、結果のXとYを線AのXとYに加算

                        //xxx = ppp.Margin.Left;
                        //xxx = ppp.Margin.Top;
                    }
                }
                //ユニットの端の位置を算出
                if (ClassGameStatus.ClassBattle.DefUnitGroup.Count % 2 == 0)
                {
                    ////偶数
                    //これは正しくないが、案が思い浮かばない
                    hidariTakasa.X = (canvas.Width * 0.75) - ((double)countMeHalf * 32);
                    hidariTakasa.Y = (canvas.Height * 0.25) - ((double)countMeHalf * (takasaMapTip / 2));

                    migiTakasa.X = (canvas.Width * 0.75) + ((double)countMeHalf * 32);
                    migiTakasa.Y = (canvas.Height * 0.25) + ((double)countMeHalf * (takasaMapTip / 2));
                }
                else
                {
                    ////奇数
                    //これは正しくないが、案が思い浮かばない
                    hidariTakasa.X = (canvas.Width * 0.75) - (((double)countMeHalf + 1) * 32);
                    hidariTakasa.Y = (canvas.Height * 0.25) - (((double)countMeHalf + 1) * (takasaMapTip / 2));

                    migiTakasa.X = (canvas.Width * 0.75) + (((double)countMeHalf + 1) * 32);
                    migiTakasa.Y = (canvas.Height * 0.25) + (((double)countMeHalf + 1) * (takasaMapTip / 2));
                }

                //防衛前衛
                foreach (var item in this.ClassGameStatus
                            .ClassBattle.DefUnitGroup
                            .Where(y => y.FlagBuilding == false)
                            .Where(x => x.ListClassUnit[0].Formation.Formation == Formation.F))
                {
                    //比率
                    Point hiritu = new Point()
                    {
                        X = item.ListClassUnit.Count - 1,
                        Y = 0
                    };

                    foreach (var itemListClassUnit in item.ListClassUnit.Select((value, index) => (value, index)))
                    {
                        List<string> strings = new List<string>();
                        strings.Add(this.ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName);
                        strings.Add("040_ChipImage");
                        strings.Add(itemListClassUnit.value.Image);
                        string path = System.IO.Path.Combine(strings.ToArray());

                        var bi = new BitmapImage(new Uri(path));
                        ImageBrush image = new ImageBrush();
                        image.Stretch = Stretch.Fill;
                        image.ImageSource = bi;
                        Button button = new Button();
                        button.Background = image;
                        button.Width = 32;
                        button.Height = 32;
                        Canvas canvasChip = new Canvas();
                        //固有の情報
                        canvasChip.Name = "Chip" + itemListClassUnit.value.ID.ToString();
                        canvasChip.Tag = itemListClassUnit.value.ID.ToString();
                        //プレイヤー側のみイベントをくっつけるようにする
                        if (ClassGameStatus.ClassBattle.BattleWhichIsThePlayer == BattleWhichIsThePlayer.Def)
                        {
                            canvasChip.PreviewMouseLeftButtonDown += WindowMapBattleUnit_MouseLeftButtonDown;
                        }
                        canvasChip.Children.Add(button);
                        canvasChip.Width = 32;
                        canvasChip.Height = 32;
                        //内分点の公式
                        double left = (
                                        ((hiritu.X - itemListClassUnit.index) * hidariTakasa.X) + (itemListClassUnit.index * migiTakasa.X)
                                        )
                                        / (itemListClassUnit.index + (hiritu.X - itemListClassUnit.index));
                        double top = (
                                        ((hiritu.X - itemListClassUnit.index) * hidariTakasa.Y) + (itemListClassUnit.index * migiTakasa.Y)
                                        )
                                        / (itemListClassUnit.index + (hiritu.X - itemListClassUnit.index));
                        if (item.ListClassUnit.Count == 1)
                        {
                            left = (hidariTakasa.X + migiTakasa.X) / 2;
                            top = (hidariTakasa.Y + migiTakasa.Y) / 2;
                        }
                        //Border
                        Border border = new Border();
                        border.Name = "border" + itemListClassUnit.value.ID.ToString();
                        border.BorderThickness = new Thickness();
                        border.Child = canvasChip;
                        border.Margin = new Thickness()
                        {
                            Left = left,
                            Top = top + 192
                        };
                        itemListClassUnit.value.NowPosiLeft = new Point()
                        {
                            X = left,
                            Y = top + 192
                        };
                        itemListClassUnit.value.OrderPosiLeft = new Point()
                        {
                            X = left,
                            Y = top + 192
                        };

                        Canvas.SetZIndex(border, 99);
                        canvas.Children.Add(border);
                    }
                }
                //防衛中衛
                foreach (var item in this.ClassGameStatus
                            .ClassBattle.DefUnitGroup
                            .Where(y => y.FlagBuilding == false)
                            .Where(x => x.ListClassUnit[0].Formation.Formation == Formation.M))
                {
                    //比率
                    Point hiritu = new Point()
                    {
                        X = item.ListClassUnit.Count - 1,
                        Y = 0
                    };

                    foreach (var itemListClassUnit in item.ListClassUnit.Select((value, index) => (value, index)))
                    {
                        List<string> strings = new List<string>();
                        strings.Add(this.ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName);
                        strings.Add("040_ChipImage");
                        strings.Add(itemListClassUnit.value.Image);
                        string path = System.IO.Path.Combine(strings.ToArray());

                        var bi = new BitmapImage(new Uri(path));
                        ImageBrush image = new ImageBrush();
                        image.Stretch = Stretch.Fill;
                        image.ImageSource = bi;
                        Button button = new Button();
                        button.Background = image;
                        button.Width = 32;
                        button.Height = 32;

                        Canvas canvasChip = new Canvas();
                        //固有の情報
                        canvasChip.Name = "Chip" + itemListClassUnit.value.ID.ToString();
                        canvasChip.Tag = itemListClassUnit.value.ID.ToString();
                        if (ClassGameStatus.ClassBattle.BattleWhichIsThePlayer == BattleWhichIsThePlayer.Def)
                        {
                            canvasChip.PreviewMouseLeftButtonDown += WindowMapBattleUnit_MouseLeftButtonDown;
                        }
                        canvasChip.Children.Add(button);
                        canvasChip.Width = 32;
                        canvasChip.Height = 32;
                        //内分点の公式
                        double left = (
                                        ((hiritu.X - itemListClassUnit.index) * hidariTakasa.X) + (itemListClassUnit.index * migiTakasa.X)
                                        )
                                        / (itemListClassUnit.index + (hiritu.X - itemListClassUnit.index));
                        double top = (
                                        ((hiritu.X - itemListClassUnit.index) * hidariTakasa.Y) + (itemListClassUnit.index * migiTakasa.Y)
                                        )
                                        / (itemListClassUnit.index + (hiritu.X - itemListClassUnit.index));
                        if (item.ListClassUnit.Count == 1)
                        {
                            left = (hidariTakasa.X + migiTakasa.X) / 2;
                            top = (hidariTakasa.Y + migiTakasa.Y) / 2;
                        }
                        //Border
                        Border border = new Border();
                        border.Name = "border" + itemListClassUnit.value.ID.ToString();
                        border.BorderThickness = new Thickness();
                        border.Child = canvasChip;
                        border.Margin = new Thickness()
                        {
                            Left = left,
                            Top = top + 86
                        };
                        itemListClassUnit.value.NowPosiLeft = new Point()
                        {
                            X = left,
                            Y = top + 86
                        };
                        itemListClassUnit.value.OrderPosiLeft = new Point()
                        {
                            X = left,
                            Y = top + 86
                        };

                        Canvas.SetZIndex(border, 99);
                        canvas.Children.Add(border);
                    }
                }
                //防衛後衛
                foreach (var item in this.ClassGameStatus
                            .ClassBattle.DefUnitGroup
                            .Where(y => y.FlagBuilding == false)
                            .Where(x => x.ListClassUnit[0].Formation.Formation == Formation.B))
                {
                    //比率
                    Point hiritu = new Point()
                    {
                        X = item.ListClassUnit.Count - 1,
                        Y = 0
                    };

                    foreach (var itemListClassUnit in item.ListClassUnit.Select((value, index) => (value, index)))
                    {
                        List<string> strings = new List<string>();
                        strings.Add(this.ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName);
                        strings.Add("040_ChipImage");
                        strings.Add(itemListClassUnit.value.Image);
                        string path = System.IO.Path.Combine(strings.ToArray());

                        var bi = new BitmapImage(new Uri(path));
                        ImageBrush image = new ImageBrush();
                        image.Stretch = Stretch.Fill;
                        image.ImageSource = bi;
                        Button button = new Button();
                        button.Background = image;
                        button.Width = 32;
                        button.Height = 32;

                        Canvas canvasChip = new Canvas();
                        //固有の情報
                        canvasChip.Name = "Chip" + itemListClassUnit.value.ID.ToString();
                        canvasChip.Tag = itemListClassUnit.value.ID.ToString();
                        if (ClassGameStatus.ClassBattle.BattleWhichIsThePlayer == BattleWhichIsThePlayer.Def)
                        {
                            canvasChip.PreviewMouseLeftButtonDown += WindowMapBattleUnit_MouseLeftButtonDown;
                        }
                        canvasChip.Children.Add(button);
                        canvasChip.Width = 32;
                        canvasChip.Height = 32;
                        //内分点の公式
                        double left = (
                                        ((hiritu.X - itemListClassUnit.index) * hidariTakasa.X) + (itemListClassUnit.index * migiTakasa.X)
                                        )
                                        / (itemListClassUnit.index + (hiritu.X - itemListClassUnit.index));
                        double top = (
                                        ((hiritu.X - itemListClassUnit.index) * hidariTakasa.Y) + (itemListClassUnit.index * migiTakasa.Y)
                                        )
                                        / (itemListClassUnit.index + (hiritu.X - itemListClassUnit.index));
                        if (item.ListClassUnit.Count == 1)
                        {
                            left = (hidariTakasa.X + migiTakasa.X) / 2;
                            top = (hidariTakasa.Y + migiTakasa.Y) / 2;
                        }
                        //Border
                        Border border = new Border();
                        border.Name = "border" + itemListClassUnit.value.ID.ToString();
                        border.BorderThickness = new Thickness();
                        border.Child = canvasChip;
                        border.Margin = new Thickness()
                        {
                            Left = left,
                            Top = top
                        };
                        itemListClassUnit.value.NowPosiLeft = new Point()
                        {
                            X = left,
                            Y = top
                        };
                        itemListClassUnit.value.OrderPosiLeft = new Point()
                        {
                            X = left,
                            Y = top
                        };

                        Canvas.SetZIndex(border, 99);
                        canvas.Children.Add(border);
                    }
                }
            }

            //ウィンドウ
            {
                Uri uri = new Uri("/Page025_Battle_Command.xaml", UriKind.Relative);
                Frame frame = new Frame();
                frame.Source = uri;
                frame.Margin = new Thickness(0, this._sizeClientWinHeight - 310 - 60, 0, 0);
                frame.Name = StringName.windowBattleCommand;
                Canvas.SetZIndex(frame, 99);
                this.canvasMain.Children.Add(frame);
            }
            {
                Uri uri = new Uri("/Page026_Battle_SelectUnit.xaml", UriKind.Relative);
                Frame frame = new Frame();
                frame.Source = uri;
                frame.Margin = new Thickness(0, this._sizeClientWinHeight - 120, 0, 0);
                frame.Name = StringName.windowBattleCommand;
                Canvas.SetZIndex(frame, 99);
                this.canvasMain.Children.Add(frame);
            }
            Application.Current.Properties["window"] = this;


            timerAfterFadeIn = new DispatcherTimer(DispatcherPriority.Background);
            timerAfterFadeIn.Interval = TimeSpan.FromSeconds((double)1 / 60);
            timerAfterFadeIn.Tick += (x, s) =>
            {
                TimerAction60FPSAfterFadeInBattleStart();
                MainWindow.KeepInterval(timerAfterFadeIn);
            };
            AfterFadeIn = true;
            timerAfterFadeIn.Start();
        }

        private string GetPathTipImage((ClassUnit value, int index) itemListClassUnit)
        {
            List<string> strings = new List<string>();
            strings.Add(this.ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName);
            strings.Add("040_ChipImage");
            strings.Add(itemListClassUnit.value.Image);
            string path = System.IO.Path.Combine(strings.ToArray());
            return path;
        }

        public readonly int startSpaceTop = 30;
        public readonly int startSpaceLeft = 30;

        private void SetWindowTitle(int targetNumber)
        {
            // 既に何か表示されてたら消す (Now Loading... の画像とか)
            this.canvasMain.Children.Clear();
            this.canvasUIRightTop.Children.Clear();
            this.canvasUIRightBottom.Children.Clear();

            // Display Background
            this.canvasMain.Background = GetTitleImage(targetNumber);

            // Display Button
            var displayButton = GetPathTitleButtonImage(targetNumber);
            foreach (var item in displayButton.Select((value, index) => (value, index)))
            {
                {
                    Grid grid = new Grid();
                    BitmapImage bitimg1 = new BitmapImage(new Uri(item.value));
                    Image img = new Image();
                    img.Stretch = Stretch.Fill;
                    img.Source = bitimg1;
                    grid.Children.Add(img);

                    Button button = new Button();
                    button.Width = 160;
                    button.Height = 40;
                    button.Tag = item.index;
                    button.Click += titleButton_click;
                    button.Content = grid;

                    button.Margin = new Thickness()
                    {
                        Top = (50 * item.index) + this.startSpaceTop,
                        Left = this.CanvasMainWidth - button.Width - startSpaceLeft,
                    };

                    this.canvasUIRightTop.Children.Add(button);
                }
            }
            // Play BGM
        }

        private Brush GetTitleImage(int targetNumber)
        {
            ImageBrush imageBrush = new ImageBrush();
            string fullPath = GetPathTitleImage(targetNumber);
            imageBrush.ImageSource =
                new BitmapImage(new Uri(fullPath, UriKind.Relative));
            return imageBrush;
        }

        private string GetPathTitleImage(int targetNumber)
        {
            List<string> strings = new List<string>();
            strings.Add(this.ClassConfigGameTitle.DirectoryGameTitle[targetNumber].FullName);
            strings.Add("001_SystemImage");
            strings.Add("015_TitleMenuImage");
            strings.Add("title.jpg");
            {
                string fullPath = System.IO.Path.Combine(
                        strings.ToArray()
                    );
                if (File.Exists(fullPath) == true)
                {
                    return fullPath;
                }
            }

            strings.RemoveAt(strings.Count - 1);
            strings.Add("title.png");
            {
                string fullPath = System.IO.Path.Combine(
                        strings.ToArray()
                    );
                if (File.Exists(fullPath) == true)
                {
                    return fullPath;
                }
            }

            strings.RemoveAt(strings.Count - 1);
            strings.Add("title.gif");
            {
                string fullPath = System.IO.Path.Combine(
                        strings.ToArray()
                    );
                if (File.Exists(fullPath) == true)
                {
                    return fullPath;
                }
            }

            throw new Exception();
        }
        private List<string> GetPathTitleButtonImage(int targetNumber)
        {
            List<string> strings = new List<string>();
            List<string> result = new List<string>();
            strings.Add(this.ClassConfigGameTitle.DirectoryGameTitle[targetNumber].FullName);
            strings.Add("001_SystemImage");
            strings.Add("015_TitleMenuImage");
            strings.Add("0001_easy.png");
            {
                string fullPath = System.IO.Path.Combine(
                        strings.ToArray()
                    );
                if (File.Exists(fullPath) == true)
                {
                    result.Add(fullPath);
                }
            }

            strings.RemoveAt(strings.Count - 1);
            strings.Add("0002_normal.png");
            {
                string fullPath = System.IO.Path.Combine(
                        strings.ToArray()
                    );
                if (File.Exists(fullPath) == true)
                {
                    result.Add(fullPath);
                }
            }

            strings.RemoveAt(strings.Count - 1);
            strings.Add("0003_hard.png");
            {
                string fullPath = System.IO.Path.Combine(
                        strings.ToArray()
                    );
                if (File.Exists(fullPath) == true)
                {
                    result.Add(fullPath);
                }
            }

            strings.RemoveAt(strings.Count - 1);
            strings.Add("0004_luna.png");
            {
                string fullPath = System.IO.Path.Combine(
                        strings.ToArray()
                    );
                if (File.Exists(fullPath) == true)
                {
                    result.Add(fullPath);
                }
            }

            if (result.Count < 1)
            {
                throw new Exception();
            }

            return result;
        }

        /// <summary>
        /// シナリオ選択ボタンを押す画面
        /// 次処理は恐らく「ScenarioSelectionButton_click」
        /// </summary>
        public void SetWindowMainMenu()
        {
            this.canvasMain.Children.Clear();
            this.canvasUIRightTop.Children.Clear();
            this.canvasUIRightBottom.Children.Clear();

            this.canvasMain.Background = new SolidColorBrush(Color.FromRgb(39, 51, 54));

            // シナリオ情報一括読み込み
            if (this.ListClassScenarioInfo.Count <= 0)
            {
                Set_List_ClassInfo(this.NowNumberGameTitle);
            }

            // Map情報一括読み込み
            if (this.ClassGameStatus.ListClassMapBattle.Count <= 0)
            {
                SetListClassMapBattle(this.NowNumberGameTitle);
            }


            // 左上作る
            {
                Canvas canvas = new Canvas();
                canvas.Height = this.CanvasMainHeight / 2;
                canvas.Width = this.CanvasMainWidth / 2;
                canvas.Margin = new Thickness()
                {
                    Left = 0,
                    Top = 0
                };
                canvas.Name = StringName.windowMainMenuLeftTop;
                canvas.MouseRightButtonDown += ScenarioSelection_MouseRightButtonDown;
                {
                    // 枠
                    var rectangleInfo = new Rectangle();
                    rectangleInfo.Fill = new SolidColorBrush(Color.FromRgb(190, 178, 175));
                    rectangleInfo.Height = this.CanvasMainHeight / 2;
                    rectangleInfo.Width = this.CanvasMainWidth / 2;
                    rectangleInfo.Stroke = new SolidColorBrush(Colors.Gray);
                    rectangleInfo.StrokeThickness = 5;
                    canvas.Children.Add(rectangleInfo);

                    foreach (var item in this.ListClassScenarioInfo
                                            .Where(y => y.Sortkey <= 0)
                                            .OrderBy(x => x.Sortkey)
                                            .Select((value, index) => (value, index)))
                    {
                        // シナリオタイトル
                        int fontSizePlus = 5;
                        TextBlock tbDate1 = new TextBlock();
                        tbDate1.FontSize = tbDate1.FontSize + fontSizePlus;
                        tbDate1.Text = item.value.ScenarioName;
                        tbDate1.Height = 40;
                        tbDate1.Margin = new Thickness { Left = 15, Top = 15 };

                        Button button = new Button();
                        button.Content = tbDate1;
                        int hei = 60;
                        button.Height = hei;
                        button.Width = (this.CanvasMainWidth / 2) - 100;
                        button.Margin = new Thickness { Left = 15, Top = 15 + ((hei + 20) * item.index) };
                        button.Tag = item.index;
                        button.MouseEnter += WindowMainMenuLeftTop_MouseEnter;
                        button.Click += ScenarioSelectionButton_click;
                        button.MouseRightButtonDown += Disable_MouseEvent;

                        canvas.Children.Add(button);
                    }
                }
                this.canvasMain.Children.Add(canvas);
            }

            // 右上作らない

            // 左下作る
            {
                int titleHeight = 60;
                Canvas canvas = new Canvas();
                canvas.Height = this.CanvasMainHeight / 2;
                canvas.Width = this.CanvasMainWidth / 2;
                canvas.Margin = new Thickness()
                {
                    Left = 0,
                    Top = canvas.Height
                };
                canvas.Name = StringName.windowMainMenuLeftUnder;
                canvas.MouseRightButtonDown += ScenarioSelection_MouseRightButtonDown;
                {
                    // 枠下
                    {
                        var rectangleInfo = new Rectangle();
                        rectangleInfo.Fill = new SolidColorBrush(Color.FromRgb(190, 178, 175));
                        rectangleInfo.Height = (this.CanvasMainHeight / 2) - titleHeight;
                        rectangleInfo.Width = this.CanvasMainWidth / 2;
                        rectangleInfo.Stroke = new SolidColorBrush(Colors.Gray);
                        rectangleInfo.StrokeThickness = 5;
                        rectangleInfo.Margin = new Thickness()
                        {
                            Left = 0,
                            Top = titleHeight
                        };
                        canvas.Children.Add(rectangleInfo);
                    }

                    // 枠上
                    {
                        Grid grid = new Grid();
                        var rectangleInfo = new Rectangle();
                        rectangleInfo.Fill = new SolidColorBrush(Colors.Black);
                        rectangleInfo.Height = titleHeight;
                        rectangleInfo.Width = this.CanvasMainWidth / 2;
                        rectangleInfo.Stroke = new SolidColorBrush(Colors.Gray);
                        rectangleInfo.StrokeThickness = 5;
                        grid.Children.Add(rectangleInfo);

                        int fontSizePlus = 5;
                        TextBlock tbDate1 = new TextBlock();
                        tbDate1.FontSize = tbDate1.FontSize + fontSizePlus;
                        tbDate1.Text = "Etc.";
                        tbDate1.Foreground = Brushes.White;
                        tbDate1.Height = 40;
                        tbDate1.Margin = new Thickness { Left = 15, Top = 15 };
                        grid.Children.Add(tbDate1);

                        canvas.Children.Add(grid);
                    }

                    int tag = this.ListClassScenarioInfo
                                            .Where(y => y.Sortkey <= 0)
                                            .Count();

                    foreach (var item in this.ListClassScenarioInfo
                                            .Where(y => y.Sortkey > 0)
                                            .OrderBy(x => x.Sortkey)
                                            .Select((value, index) => (value, index)))
                    {
                        // シナリオタイトル
                        int fontSizePlus = 5;
                        TextBlock tbDate1 = new TextBlock();
                        tbDate1.FontSize = tbDate1.FontSize + fontSizePlus;
                        tbDate1.Text = item.value.ScenarioName;
                        tbDate1.Height = 40;
                        tbDate1.Margin = new Thickness { Left = 15, Top = 15 };

                        Button button = new Button();
                        button.Content = tbDate1;
                        int hei = 60;
                        button.Height = hei;
                        button.Width = (this.CanvasMainWidth / 2) - 100;
                        button.Margin = new Thickness { Left = 15, Top = titleHeight + 15 + ((hei + 20) * item.index) };
                        button.Tag = tag + item.index;
                        button.MouseEnter += WindowMainMenuLeftTop_MouseEnter;
                        button.Click += ScenarioSelectionButton_click;
                        button.MouseRightButtonDown += Disable_MouseEvent;

                        canvas.Children.Add(button);
                    }
                }
                this.canvasMain.Children.Add(canvas);
            }

            // 左下作らない

            // Move window

            //多分意味ない
            Thread.Sleep(10);
        }

        // シナリオごとにデータを初期化する関数
        private void InitializeGameData()
        {
            //シナリオで設定されてる標準の駐留数
            int default_capacity = this.ListClassScenarioInfo[this.NumberScenarioSelection].SpotCapacity;

            // 後から追加するかもしれないので、全ての領地を初期化する
            foreach (var item in this.ClassGameStatus.AllListSpot)
            {
                // とりあえず、初期値をそのまま使う
                // 将来的には、シナリオによって初期値が違う場合も考慮すること
                item.Gain = item.InitGain;
                item.Castle = item.InitCastle;

                // 領地の駐留数が指定されてなければ、シナリオの規定値を使う
                if (item.InitCapacity < 1)
                {
                    item.Capacity = default_capacity;
                }
                else
                {
                    item.Capacity = item.InitCapacity;
                }
            }

        }

        private void SetListClassMapBattle(int gameTitleNumber)
        {
            // get target path.
            List<string> strings = new List<string>();
            strings.Add(this.ClassConfigGameTitle.DirectoryGameTitle[gameTitleNumber].FullName);
            strings.Add("016_BattleMapImage");
            string path = System.IO.Path.Combine(strings.ToArray());

            // get file.
            var files = System.IO.Directory.EnumerateFiles(
                path,
                "*.txt",
                System.IO.SearchOption.AllDirectories
                );

            //check
            {
                if (files.Count() < 1)
                {
                    // ファイルがない！
                    throw new Exception();
                }

                if (this.ClassGameStatus.ListClassMapBattle == null)
                {
                    this.ClassGameStatus.ListClassMapBattle = new List<ClassMapBattle>();
                }
            }

            foreach (var item in files)
            {
                string readAllLines;
                readAllLines = File.ReadAllText(item);

                if (readAllLines.Length == 0)
                {
                    continue;
                }

                // 大文字かっこは許しまへんで
                {
                    var ch = readAllLines.Length - readAllLines.Replace("{", "").Replace("}", "").Length;
                    if (ch % 2 != 0 || readAllLines.Length - ch == 0)
                    {
                        throw new Exception();
                    }
                }

                // Map
                {
                    string targetString = "map";
                    // 大文字かっこも入るが、上でチェックしている←本当か？
                    // \sは空行や改行など
                    var mapMatches = new Regex(targetString + @"[\s]+?.*[\s]+?\{([\s\S\n]+?)\}", RegexOptions.IgnoreCase)
                                        .Matches(readAllLines);

                    var listMatches = mapMatches.Where(x => x != null).ToList();
                    if (listMatches == null)
                    {
                        // データがない！
                        throw new Exception();
                    }
                    if (listMatches.Count < 1)
                    {
                        // データがないので次
                    }
                    else
                    {
                        foreach (var getData in listMatches)
                        {
                            ClassGameStatus.ListClassMapBattle.Add(ClassStaticCommonMethod.GetClassMapBattle(getData.Value));
                        }
                    }

                    // Map 終わり
                }

            }
        }


        /// <summary>
        /// シナリオ選択画面から移行する戦略マップ表示画面
        /// 次処理は勢力の選択
        /// </summary>
        private void SetMapStrategy()
        {
            this.canvasMain.Children.Clear();

            // ワールドマップを構築する
            if (this.ClassGameStatus.WorldMap == null)
            {
                this.ClassGameStatus.WorldMap = new UserControl060_WorldMap();
            }
            this.ClassGameStatus.WorldMap.SetData();
            this.canvasMain.Children.Add(this.ClassGameStatus.WorldMap);

            // 領地に初期メンバーを配置する（中立領地のランダムモンスターは勢力選択後）
            foreach (var itemSpot in this.ClassGameStatus.AllListSpot)
            {
                itemSpot.UnitGroup = new List<ClassHorizontalUnit>();

                // 初期メンバー配置（中立領地でも指定されていれば配置する）
                foreach (var itemMember in itemSpot.ListMember)
                {
                    var info = this.ClassGameStatus.ListUnit.Where(x => x.NameTag == itemMember.Item1).FirstOrDefault();
                    if (info == null)
                    {
                        continue;
                    }

                    var classUnit = new List<ClassUnit>();
                    for (int i = 0; i < itemMember.Item2; i++)
                    {
                        var deep = info.DeepCopy();
                        deep.ID = this.ClassGameStatus.IDCount;
                        this.ClassGameStatus.SetIDCount();
                        classUnit.Add(deep);
                    }

                    itemSpot.UnitGroup.Add(new ClassHorizontalUnit() { Spot = itemSpot, FlagDisplay = true, ListClassUnit = classUnit });
                }
            }

            // 勢力一覧ウィンドウを出す
            ListSelectionPowerMini();
        }

        /// <summary>
        /// 戦闘終了後の処理
        /// </summary>
        private void SetMapStrategyFromBattle()
        {
            this.canvasMain.Children.Clear();

            // ワールドマップを表示する
            var worldMap = this.ClassGameStatus.WorldMap;
            if (worldMap == null)
            {
                worldMap = new UserControl060_WorldMap();
                worldMap.SetData();
            }
            this.canvasMain.Children.Add(worldMap);

            // 戦闘前のマップ位置にする
            worldMap.Margin = new Thickness()
            {
                Left = this.ClassGameStatus.Camera.X,
                Top = this.ClassGameStatus.Camera.Y
            };

            //メッセージ
            MessageBox.Show("戦闘が終了しました。");

            // 勢力メニューウィンドウを表示する
            SetWindowStrategyMenu();
        }

        // 勢力の旗アイコンを用意する
        public Image DisplayFlag(string flag_path)
        {
            BitmapImage flag_bitimg = new BitmapImage(new Uri(flag_path));
            // 旗のアニメーションは 64 * 32 ドットを想定
            CroppedBitmap[] animationImages = new CroppedBitmap[2];
            Int32Rect rect = new Int32Rect(0, 0, 32, 32);
            animationImages[0] = new CroppedBitmap(flag_bitimg, rect);
            rect = new Int32Rect(32, 0, 32, 32);
            animationImages[1] = new CroppedBitmap(flag_bitimg, rect);

            // 500 ms ごとにフレーム切り替え、全体で 1000 ms になる。
            ObjectAnimationUsingKeyFrames animation = new ObjectAnimationUsingKeyFrames();
            for (int i = 0; i < 2; i++)
            {
                DiscreteObjectKeyFrame key = new DiscreteObjectKeyFrame();
                key.KeyTime = new TimeSpan(0, 0, 0, 0, 500 * i);
                key.Value = animationImages[i];
                animation.KeyFrames.Add(key);
            }
            animation.RepeatBehavior = RepeatBehavior.Forever;
            animation.Duration = new TimeSpan(0, 0, 0, 0, 500 * 2);

            Image imgFlag = new Image();
            //imgFlag.Source = flag_bitimg;
            imgFlag.BeginAnimation(Image.SourceProperty, animation);
            imgFlag.Height = 32;
            imgFlag.Width = 32;
            imgFlag.HorizontalAlignment = HorizontalAlignment.Left;
            imgFlag.VerticalAlignment = VerticalAlignment.Top;

            return imgFlag;
        }

        // 以下の記事のコードをそのまま使ってます。
        // 「WPF/C# コントロールの要素をキャプチャする」
        // https://qiita.com/Sakurai-Shinya/items/81a9c413c3265f0e8587
        // キャプチャしたい要素（一番親の要素）を引数に渡すとBitmapSourceで返すメソッド
        public BitmapSource FrameworkElementToBitmapSource(FrameworkElement element)
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

        #region 各種構造体データ読み込みに必要なメソッド群
        /// <summary>
        /// 各種構造体データ読み込み
        /// </summary>
        /// <param name="gameTitleNumber"></param>
        /// <exception cref="Exception"></exception>
        private void Set_List_ClassInfo(int gameTitleNumber)
        {
            // get target path.
            List<string> strings = new List<string>();
            strings.Add(this.ClassConfigGameTitle.DirectoryGameTitle[gameTitleNumber].FullName);
            strings.Add("070_Scenario");
            string path = System.IO.Path.Combine(strings.ToArray());

            // get file.
            var files = System.IO.Directory.EnumerateFiles(
                path,
                "*",
                System.IO.SearchOption.AllDirectories
                );

            //check
            {
                if (files.Count() < 1)
                {
                    // ファイルがない！
                    throw new Exception();
                }

                if (this.ListClassScenarioInfo == null)
                {
                    this.ListClassScenarioInfo = new List<ClassScenarioInfo>();
                }
            }

            //ファイル毎に繰り返し
            foreach (var item in files)
            {
                string readAllLines;
                readAllLines = File.ReadAllText(item);

                if (readAllLines.Length == 0)
                {
                    continue;
                }

                // 大文字かっこは許しまへんで
                {
                    var ch = readAllLines.Length - readAllLines.Replace("{", "").Replace("}", "").Length;
                    if (ch % 2 != 0 || readAllLines.Length - ch == 0)
                    {
                        throw new Exception();
                    }
                }

                // Scenario
                {
                    // 大文字かっこも入るが、上でチェックしている
                    // \sは空行や改行など
                    var newFormatScenarioMatches = new Regex(@"NewFormatScenario[\s]+?.*[\s]+?\{([\s\S\n]+?)\}", RegexOptions.IgnoreCase).Matches(readAllLines);
                    var scenarioMatches = new Regex(@"scenario[\s]+?.*[\s]+?\{([\s\S\n]+?)\}").Matches(readAllLines);

                    var listMatches = newFormatScenarioMatches.Where(x => x != null).ToList();
                    listMatches.AddRange(scenarioMatches.Where(x => x != null).ToList());

                    if (listMatches == null)
                    {
                        // データがない！
                        throw new Exception();
                    }
                    if (listMatches.Count < 1)
                    {
                        // データがないので次
                    }
                    else
                    {
                        foreach (var getData in listMatches)
                        {
                            //enumを使うべき？
                            int kind = 0;
                            {
                                //このコードだとNewFormatScenarioTest等が通るのでよくない
                                string join = string.Join(String.Empty, getData.Value.Take(17));
                                if (String.Compare(join, "NewFormatScenario", true) == 0)
                                {
                                    kind = 0;
                                }
                                else
                                {
                                    kind = 1;
                                }
                            }

                            if (kind == 0)
                            {
                                this.ListClassScenarioInfo.Add(ClassStaticCommonMethod.GetClassScenarioNewFormat(getData.Value));
                            }
                            else
                            {
                                this.ListClassScenarioInfo.Add(ClassStaticCommonMethod.GetClassScenario(getData.Value));
                            }
                        }
                        if (this.ListClassScenarioInfo.Count > 1)
                        {
                            this.ListClassScenarioInfo.Sort((x, y) => x.Sortkey - y.Sortkey);
                        }
                    }
                }
                // Scenario 終わり

                // Spot
                {
                    string targetString = "NewFormatSpot";
                    // 大文字かっこも入るが、上でチェックしている
                    // \sは空行や改行など
                    var newFormatScenarioMatches = new Regex(targetString + @"[\s]+?.*[\s]+?\{([\s\S\n]+?)\}", RegexOptions.IgnoreCase).Matches(readAllLines);
                    var scenarioMatches = new Regex(@"spot[\s]+?.*[\s]+?\{([\s\S\n]+?)\}").Matches(readAllLines);

                    var listMatches = newFormatScenarioMatches.Where(x => x != null).ToList();
                    listMatches.AddRange(scenarioMatches.Where(x => x != null).ToList());

                    if (listMatches == null)
                    {
                        // データがない！
                        throw new Exception();
                    }
                    if (listMatches.Count < 1)
                    {
                        // データがないので次
                    }
                    else
                    {
                        foreach (var getData in listMatches)
                        {
                            //enumを使うべき？
                            int kind = 0;
                            {
                                //このコードだとNewFormatSpotTest等が通るのでよくない
                                string join = string.Join(String.Empty, getData.Value.Take(targetString.Length));
                                if (String.Compare(join, targetString, true) == 0)
                                {
                                    kind = 0;
                                }
                                else
                                {
                                    kind = 1;
                                }
                            }

                            if (kind == 0)
                            {
                                ClassGameStatus.AllListSpot.Add(ClassStaticCommonMethod.GetClassSpotNewFormat(getData.Value, ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName));
                            }
                            else
                            {
                                ClassGameStatus.AllListSpot.Add(ClassStaticCommonMethod.GetClassSpot(getData.Value, ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName));
                            }
                        }
                    }
                }
                // Spot 終わり

                // Power
                {
                    string targetString = "NewFormatPower";
                    // 大文字かっこも入るが、上でチェックしている
                    // \sは空行や改行など
                    var newFormatScenarioMatches = new Regex(targetString + @"[\s]+?.*[\s]+?\{([\s\S\n]+?)\}", RegexOptions.IgnoreCase).Matches(readAllLines);
                    var scenarioMatches = new Regex(@"power[\s]+?.*[\s]+?\{([\s\S\n]+?)\}").Matches(readAllLines);

                    var listMatches = newFormatScenarioMatches.Where(x => x != null).ToList();
                    listMatches.AddRange(scenarioMatches.Where(x => x != null).ToList());

                    if (listMatches == null)
                    {
                        // データがない！
                        throw new Exception();
                    }
                    if (listMatches.Count < 1)
                    {
                        // データがないので次
                    }
                    else
                    {
                        foreach (var getData in listMatches)
                        {
                            //enumを使うべき？
                            int kind = 0;
                            {
                                //このコードだとNewFormatPowerTest等が通るのでよくない
                                string join = string.Join(String.Empty, getData.Value.Take(targetString.Length));
                                if (String.Compare(join, targetString, true) == 0)
                                {
                                    kind = 0;
                                }
                                else
                                {
                                    kind = 1;
                                }
                            }

                            if (kind == 0)
                            {
                                ClassGameStatus.ListPower.Add(ClassStaticCommonMethod.GetClassPowerNewFormat(getData.Value, ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName));
                            }
                            else
                            {
                                ClassGameStatus.ListPower.Add(ClassStaticCommonMethod.GetClassPower(getData.Value));
                            }
                        }
                    }
                }
                // Power 終わり

                // Unit
                {
                    string targetString = "NewFormatUnit";
                    // 大文字かっこも入るが、上でチェックしている
                    // \sは空行や改行など
                    var newFormatScenarioMatches = new Regex(targetString + @"[\s]+?.*[\s]+?\{([\s\S\n]+?)\}", RegexOptions.IgnoreCase).Matches(readAllLines);
                    var scenarioMatches = new Regex(@"Unit[\s]+?.*[\s]+?\{([\s\S\n]+?)\}").Matches(readAllLines);

                    var listMatches = newFormatScenarioMatches.Where(x => x != null).ToList();
                    listMatches.AddRange(scenarioMatches.Where(x => x != null).ToList());

                    if (listMatches == null)
                    {
                        // データがない！
                        throw new Exception();
                    }
                    if (listMatches.Count < 1)
                    {
                        // データがないので次
                    }
                    else
                    {
                        foreach (var getData in listMatches)
                        {
                            //enumを使うべき？
                            int kind = 0;
                            {
                                //このコードだとNewFormatUnitTest等が通るのでよくない
                                string join = string.Join(String.Empty, getData.Value.Take(targetString.Length));
                                if (String.Compare(join, targetString, true) == 0)
                                {
                                    kind = 0;
                                }
                                else
                                {
                                    kind = 1;
                                }
                            }

                            if (kind == 0)
                            {
                                ClassGameStatus.ListUnit.Add(ClassStaticCommonMethod.GetClassUnitNewFormat(getData.Value));
                            }
                            else
                            {
                                //ClassGameStatus.ListUnit.Add(GetClassUnit(getData.Value));
                            }
                        }
                    }
                }
                // Unit 終わり

                // Skill
                {
                    string targetString = "NewFormatSkill";
                    // 大文字かっこも入るが、上でチェックしている
                    // \sは空行や改行など
                    var newFormatScenarioMatches = new Regex(targetString + @"[\s]+?.*[\s]+?\{([\s\S\n]+?)\}", RegexOptions.IgnoreCase).Matches(readAllLines);
                    var scenarioMatches = new Regex(@"Skill[\s]+?.*[\s]+?\{([\s\S\n]+?)\}").Matches(readAllLines);

                    var listMatches = newFormatScenarioMatches.Where(x => x != null).ToList();
                    listMatches.AddRange(scenarioMatches.Where(x => x != null).ToList());

                    if (listMatches == null)
                    {
                        // データがない！
                        throw new Exception();
                    }
                    if (listMatches.Count < 1)
                    {
                        // データがないので次
                    }
                    else
                    {
                        foreach (var getData in listMatches)
                        {
                            //enumを使うべき？
                            int kind = 0;
                            {
                                //このコードだとNewFormatUnitTest等が通るのでよくない
                                string join = string.Join(String.Empty, getData.Value.Take(targetString.Length));
                                if (String.Compare(join, targetString, true) == 0)
                                {
                                    kind = 0;
                                }
                                else
                                {
                                    kind = 1;
                                }
                            }

                            if (kind == 0)
                            {
                                ClassGameStatus.ListSkill.Add(ClassStaticCommonMethod.GetClassSkillNewFormat(getData.Value));
                            }
                            else
                            {
                                //ClassGameStatus.ListUnit.Add(GetClassUnit(getData.Value));
                            }
                        }
                    }
                }
                // Skill 終わり

                // Event
                {
                    string targetString = "event";
                    // 大文字かっこも入るが、上でチェックしている
                    // \sは空行や改行など
                    var eventMatches = new Regex(targetString + @"[\s]+?.*[\s]+?\<-([\s\S\n]+?)\->", RegexOptions.IgnoreCase).Matches(readAllLines);

                    var listMatches = eventMatches.Where(x => x != null).ToList();
                    if (listMatches == null)
                    {
                        // データがない！
                        throw new Exception();
                    }
                    if (listMatches.Count < 1)
                    {
                        // データがないので次
                    }
                    else
                    {
                        foreach (var getData in listMatches)
                        {
                            ClassStaticCommonMethod.GetClassEvent(getData.Value, this.ClassGameStatus);
                        }
                    }

                    // Event 終わり
                }
                // Event 終わり

                //object
                {
                    string targetString = "NewFormatObject";
                    // 大文字かっこも入るが、上でチェックしている
                    // \sは空行や改行など
                    var newFormatScenarioMatches = new Regex(targetString + @"[\s]+?.*[\s]+?\{([\s\S\n]+?)\}", RegexOptions.IgnoreCase).Matches(readAllLines);
                    var scenarioMatches = new Regex(@"Object[\s]+?.*[\s]+?\{([\s\S\n]+?)\}", RegexOptions.IgnoreCase).Matches(readAllLines);

                    var listMatches = newFormatScenarioMatches.Where(x => x != null).ToList();
                    listMatches.AddRange(scenarioMatches.Where(x => x != null).ToList());

                    if (listMatches != null)
                    {
                        if (listMatches.Count < 1)
                        {
                            // データがないので次
                        }
                        else
                        {
                            foreach (var getData in listMatches)
                            {
                                //enumを使うべき？
                                int kind = 0;
                                {
                                    //このコードだとNewFormatObjectTest等が通るのでよくない
                                    string join = string.Join(String.Empty, getData.Value.Take(targetString.Length));
                                    if (String.Compare(join, targetString, true) == 0)
                                    {
                                        kind = 0;
                                    }
                                    else
                                    {
                                        kind = 1;
                                    }
                                }

                                if (kind == 0)
                                {
                                    ClassGameStatus.ListObject.Add(ClassStaticCommonMethod.GetClassObjNewFormat(getData.Value));
                                }
                                else
                                {
                                    //ClassGameStatus.ListObject.Add(GetClassObj(getData.Value));
                                }
                            }
                        }
                    }
                }
                //object 終わり

                //正規表現終わり

                //インデックスを張っておく
                for (int i = 0; i < ClassGameStatus.AllListSpot.Count; i++)
                {
                    ClassGameStatus.AllListSpot[i].Index = i;
                }
                for (int i = 0; i < ClassGameStatus.ListPower.Count; i++)
                {
                    ClassGameStatus.ListPower[i].Index = i;
                }

            }
            //ファイル毎に繰り返し 終了

            // unitのスキル名からスキルクラスを探し、unitに格納
            foreach (var itemUnit in this.ClassGameStatus.ListUnit)
            {
                foreach (var itemSkillName in itemUnit.SkillName)
                {
                    var x = this.ClassGameStatus.ListSkill
                            .Where(x => x.NameTag == itemSkillName)
                            .FirstOrDefault();
                    if (x == null)
                    {
                        continue;
                    }

                    itemUnit.Skill.Add(x);
                }
            }
        }

        #endregion

        private void TimerAction60FPS()
        {
            // 一秒間に60回実行される

            if (this.FadeOut == true
                || this.FadeOutExecution == true)
            {
                this.FadeOutExecution = true;

                if (this.fade.Children.Count == 1)
                {
                    var rec = this.fade.Children[this.fade.Children.Count - 1] as System.Windows.Shapes.Rectangle;
                    if (rec == null)
                    {
                        throw new Exception();
                    }
                    if (rec.Height < this._sizeClientWinHeight)
                    {
                        rec.Height += 50;
                    }

                    if (rec.Height >= this._sizeClientWinHeight)
                    {
                        FadeOut = false;
                        this.FadeOutExecution = false;
                    }
                }
                else
                {
                    this.fade.IsHitTestVisible = true;
                    var rect = new System.Windows.Shapes.Rectangle();
                    rect.Width = 2000;
                    rect.Height = 0;
                    rect.Name = "recFadeOut";
                    rect.Fill = System.Windows.Media.Brushes.Black;

                    // this.fade.Children.Countが1になる
                    this.fade.Children.Add(rect);
                }

                return;
            }

            //裏で実行するもの
            if (delegateMainWindowContentRendered != null)
            {
                delegateMainWindowContentRendered();
                delegateMainWindowContentRendered = null;
            }
            if (delegateMapRendered != null)
            {
                delegateMapRendered();
                delegateMapRendered = null;
            }
            if (delegateNewGame != null)
            {
                delegateNewGame();
                delegateNewGame = null;
            }
            if (delegateBattleMap != null)
            {
                delegateBattleMap();
                delegateBattleMap = null;
            }
            if (delegateMapRenderedFromBattle != null)
            {
                delegateMapRenderedFromBattle();
                delegateMapRenderedFromBattle = null;
            }

            if (this.FadeIn == true
                || this.FadeInExecution == true)
            {
                this.FadeInExecution = true;

                if (this.fade.Children.Count == 1)
                {
                    var rec = this.fade.Children[this.fade.Children.Count - 1] as System.Windows.Shapes.Rectangle;
                    if (rec == null)
                    {
                        throw new Exception();
                    }
                    if (rec.Height > 0
                        && rec.Height < 50)
                    {
                        rec.Height = 0;
                    }
                    else if (rec.Height > 0)
                    {
                        rec.Height -= 50;
                    }

                    if (rec.Height == 0)
                    {
                        this.fade.IsHitTestVisible = false;
                        this.FadeIn = false;
                        this.FadeInExecution = false;

                        this.AfterFadeIn = true;
                    }
                }

                return;
            }
        }

        /// <summary>
        /// 勢力決定後に実行
        /// </summary>
        /// <exception cref="Exception"></exception>
        private async void TimerAction60FPSAfterFadeInDecidePower()
        {
            if (AfterFadeIn == false)
            {
                return;
            }
            var rec = this.fade.Children[this.fade.Children.Count - 1] as System.Windows.Shapes.Rectangle;
            if (rec == null)
            {
                throw new Exception();
            }
            if (rec.Height > 0)
            {
                return;
            }

            //この位置でなければダメ？
            AfterFadeIn = false;
            timerAfterFadeIn.Stop();

            Thread.Sleep(100);

            if (delegateNewGameAfterFadeIn != null)
            {
                delegateNewGameAfterFadeIn();
                delegateNewGameAfterFadeIn = null;
            }

            var worldMap = this.ClassGameStatus.WorldMap;
            if (worldMap == null)
            {
                return;
            }

            ClassVec classVec = new ClassVec();
            // 現在の Margin
            classVec.X = worldMap.Margin.Left;
            classVec.Y = worldMap.Margin.Top;

            // 目標にする領地の座標をウインドウ中央にするための Margin
            classVec.Target = new Point(
                this.CanvasMainWidth / 2 - this.ClassGameStatus.SelectionCityPoint.X,
                this.CanvasMainHeight / 2 - this.ClassGameStatus.SelectionCityPoint.Y
            );
            classVec.Speed = 10;
            classVec.Set();

            while (true)
            {
                Thread.Sleep(5);

                if (classVec.Hit(new Point(classVec.X, classVec.Y)))
                {
                    break;
                }

                await Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        var ge = classVec.Get(new Point(classVec.X, classVec.Y));
                        classVec.X = ge.X;
                        classVec.Y = ge.Y;
                        worldMap.Margin = new Thickness()
                        {
                            Left = Math.Truncate(ge.X),
                            Top = Math.Truncate(ge.Y)
                        };
                    }));
                });
            }

            //イベント実行
            {
                var ev = this.ClassGameStatus.ListEvent
                            .Where(x => x.Name == this.ListClassScenarioInfo[this.NumberScenarioSelection].World)
                            .FirstOrDefault();
                if (ev != null)
                {
                    var enviroment = new Enviroment();
                    var evaluator = new Evaluator();
                    evaluator.ClassGameStatus = this.ClassGameStatus;
                    evaluator.Eval(ev.Root, enviroment);
                    ev.Yet = false;
                }
                // イベント実行中にウインドウを閉じたら、ここで終わる
                if (Application.Current == null)
                {
                    return;
                }

                // テキストウィンドウを閉じる
                if (this.ClassGameStatus.TextWindow != null)
                {
                    this.canvasTop.Children.Remove(this.ClassGameStatus.TextWindow);
                    this.ClassGameStatus.TextWindow = null;
                }
            }

            //ステータス設定
            //※毎ターンチェックする
            this.ClassGameStatus.NowTurn = 1;
            this.ClassGameStatus.NowCountPower = this.ClassGameStatus.ListPower.Count;
            this.ClassGameStatus.NowCountSelectionPowerSpot = this.ClassGameStatus.SelectionPowerAndCity.ClassPower.ListMember.Count;

            //ストラテジーメニュー表示
            SetWindowStrategyMenu();
        }

        #region Battle

        /// <summary>
        /// マップ生成後に実行
        /// </summary>
        /// <exception cref="Exception"></exception>
        private async void TimerAction60FPSAfterFadeInBattleStart()
        {
            if (AfterFadeIn == false)
            {
                return;
            }

            //この位置でなければダメ？
            AfterFadeIn = false;
            timerAfterFadeIn.Stop();

            Thread.Sleep(100);

            //自軍へ視点移動
            bool flag1 = true;

            //移動し過ぎを防止
            int counter = 500;

            //プレイヤー側リーダーへ視点移動
            //まだ未実装
            while (flag1 == true)
            {
                Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 10000)));
                break;
                await Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                    }));
                });
                counter--;

                if (counter <= 0)
                {
                    throw new Exception();
                }
            }

            //イベントチェック


            //開戦ダイアログ
            MessageBox.Show("開戦します");

            //開戦スレッド実行
            this.timerAfterFadeIn = new DispatcherTimer(DispatcherPriority.Background);
            this.timerAfterFadeIn.Interval = TimeSpan.FromSeconds((double)1 / 60);
            this.timerAfterFadeIn.Tick -= (x, s) =>
            {
                TimerAction60FPSAfterFadeInBattleStart();
                MainWindow.KeepInterval(this.timerAfterFadeIn);
            };
            this.timerAfterFadeIn.Tick += (x, s) =>
            {
                ClassStaticBattle.TimerAction60FPSBattle(this, this.ClassGameStatus, SetMapStrategyFromBattle);
                MainWindow.KeepInterval(this.timerAfterFadeIn);
            };
            this.timerAfterFadeIn.Start();

            ////スキルスレッド開始
            //出撃ユニット
            {
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleSkill(token, this.canvasMain, this.ClassGameStatus)), tokenSource);
                this.ClassGameStatus.TaskBattleSkill = a;
            }
            //防衛ユニット
            {
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleSkill(token, this.canvasMain, this.ClassGameStatus)), tokenSource);
                this.ClassGameStatus.TaskBattleSkillDef = a;
            }
            ////移動スレッド開始
            switch (ClassGameStatus.ClassBattle.BattleWhichIsThePlayer)
            {
                case BattleWhichIsThePlayer.Sortie:
                    //出撃ユニット
                    {
                        var tokenSource = new CancellationTokenSource();
                        var token = tokenSource.Token;
                        (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveAsync(token, this.ClassGameStatus, this)), tokenSource);
                        this.ClassGameStatus.TaskBattleMoveAsync.Add(a);
                    }
                    //防衛(AI)ユニット
                    {
                        var tokenSource = new CancellationTokenSource();
                        var token = tokenSource.Token;
                        (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveAIAsync(token, this.ClassGameStatus, this, this.canvasMain)), tokenSource);
                        this.ClassGameStatus.TaskBattleMoveDefAsync = a;
                    }
                    break;
                case BattleWhichIsThePlayer.Def:
                    //出撃(AI)ユニット
                    {
                        var tokenSource = new CancellationTokenSource();
                        var token = tokenSource.Token;
                        (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveAIAsync(token, this.ClassGameStatus, this, this.canvasMain)), tokenSource);
                        this.ClassGameStatus.TaskBattleMoveAsync.Add(a);
                    }
                    //防衛ユニット
                    {
                        var tokenSource = new CancellationTokenSource();
                        var token = tokenSource.Token;
                        (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveAsync(token, this.ClassGameStatus, this)), tokenSource);
                        this.ClassGameStatus.TaskBattleMoveDefAsync = a;
                    }
                    break;
                case BattleWhichIsThePlayer.None:
                    //出撃(AI)ユニット
                    {
                        var tokenSource = new CancellationTokenSource();
                        var token = tokenSource.Token;
                        (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveAIAsync(token, this.ClassGameStatus, this, this.canvasMain)), tokenSource);
                        this.ClassGameStatus.TaskBattleMoveAsync.Add(a);
                    }
                    //防衛(AI)ユニット
                    {
                        var tokenSource = new CancellationTokenSource();
                        var token = tokenSource.Token;
                        (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveAIAsync(token, this.ClassGameStatus, this, this.canvasMain)), tokenSource);
                        this.ClassGameStatus.TaskBattleMoveDefAsync = a;
                    }
                    break;
                default:
                    break;
            }

        }
        #endregion

        /// <summary>
        /// 現在メッセージ待ち行列の中にある全てのUIメッセージを処理します。
        /// </summary>
        private void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            var callback = new DispatcherOperationCallback(obj =>
            {
                ((DispatcherFrame)obj).Continue = false;
                return null;
            });
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, callback, frame);
            Dispatcher.PushFrame(frame);
        }

        public void DoWork(SystemFunctionLiteral systemFunctionLiteral)
        {
            if (Application.Current == null)
            {
                return;
            }

            Frame frame = new Frame();

            // メッセージ枠に表示する文字列を設定する。
            if (systemFunctionLiteral.Token.Type == TokenType.MSG)
            {
                Application.Current.Properties["message"] = systemFunctionLiteral.Parameters[0].Value.Replace("@@", System.Environment.NewLine);
                // キャンバスにメッセージ枠を追加する。
                Uri uri = new Uri("/Page015_Message.xaml", UriKind.Relative);
                frame.Source = uri;
                frame.Margin = new Thickness(15, this._sizeClientWinHeight - 440, 0, 0);
                frame.Name = StringName.windowSortieMenu;
                this.canvasMain.Children.Add(frame);
                Application.Current.Properties["window"] = this;
            }
            else if (systemFunctionLiteral.Token.Type == TokenType.TALK)
            {
                Application.Current.Properties["message"] = systemFunctionLiteral.Parameters[1].Value.Replace("@@", System.Environment.NewLine);
                Application.Current.Properties["face"] = systemFunctionLiteral.Parameters[0].Value;
                // キャンバスにメッセージ枠を追加する。
                Uri uri = new Uri("/Page020_Talk.xaml", UriKind.Relative);
                frame.Source = uri;
                frame.Margin = new Thickness(15, this._sizeClientWinHeight - 440, 0, 0);
                frame.Name = StringName.windowSortieMenu;
                this.canvasMain.Children.Add(frame);
                Application.Current.Properties["window"] = this;
            }

            // キャンバス表示を更新する。これが無いとメッセージ枠が表示されない。
            DoEvents();

            // メッセージ枠への入力を待つ。
            // 実際にはcanvasのどこかに入力ハンドラーを作ればいいっぽい。
            // メインウインドウ全体の入力イベントに連動させた方が、操作しやすそう。
            condition.Reset();
            while (condition.Wait(1) == false)
            {
                // 待っている間も一定時間ごとに表示を更新する。
                // これによって、ウインドウの操作や入力の処理が動くっぽい。
                DoEvents();

                // アプリケーション終了ならループから出る
                if (Application.Current == null)
                {
                    break;
                }
            }
            Thread.Sleep(1);
            condition.Reset();

            // メッセージ枠を取り除く。
            this.canvasMain.Children.Remove(frame);
        }

        public void DoTextWindow(SystemFunctionLiteral systemFunctionLiteral)
        {
            if (Application.Current == null)
            {
                return;
            }

            // テキストウィンドウに表示する文字列を設定する。
            if (systemFunctionLiteral.Token.Type == TokenType.MSG)
            {
                if (this.ClassGameStatus.TextWindow == null)
                {
                    this.ClassGameStatus.TextWindow = new UserControl050_Msg();
                    this.canvasTop.Children.Add(this.ClassGameStatus.TextWindow);
                }
                var textWindow = (UserControl050_Msg)(this.ClassGameStatus.TextWindow);
                textWindow.SetText(ClassStaticCommonMethod.MoldingText(systemFunctionLiteral.Parameters[0].Value, "$"));
                textWindow.PositionBottom();
                textWindow.RemoveName();
                textWindow.RemoveHelp();
                textWindow.RemoveFace();
            }
            else if (systemFunctionLiteral.Token.Type == TokenType.TALK)
            {
                if (this.ClassGameStatus.TextWindow == null)
                {
                    this.ClassGameStatus.TextWindow = new UserControl050_Msg();
                    this.canvasTop.Children.Add(this.ClassGameStatus.TextWindow);
                }
                var textWindow = (UserControl050_Msg)(this.ClassGameStatus.TextWindow);
                textWindow.SetText(ClassStaticCommonMethod.MoldingText(systemFunctionLiteral.Parameters[1].Value, "$"));
                textWindow.PositionBottom();

                // ユニットの識別名から肩書と名前を取得する
                string unitNameTag = systemFunctionLiteral.Parameters[0].Value;
                var classUnit = this.ClassGameStatus.ListUnit
                    .Where(x => x.NameTag == unitNameTag)
                    .FirstOrDefault();
                if (classUnit != null)
                {
                    // データが存在する時だけ表示する
                    textWindow.AddName(classUnit.Name);
                    textWindow.AddHelp(classUnit.Help);
                    textWindow.AddFace(classUnit.Face);
                }
                else
                {
                    textWindow.RemoveName();
                    textWindow.RemoveHelp();
                    textWindow.RemoveFace();
                }
            }

            // キャンバス表示を更新する。これが無いとメッセージ枠が表示されない。
            DoEvents();

            // テキストウィンドウへの入力を待つ。
            condition.Reset();
            while (condition.Wait(1) == false)
            {
                // 待っている間も一定時間ごとに表示を更新する。
                // これによって、ウインドウの操作や入力の処理が動くっぽい。
                DoEvents();

                // アプリケーション終了ならループから出る
                if (Application.Current == null)
                {
                    break;
                }
            }
            Thread.Sleep(1);
            condition.Reset();
        }

        public string MoldingText(string strTarget, string strStatus)
        {
            // 改行ごとに分割 (Split) するため、改行コードを統一する。
            string strTemp = strTarget.ReplaceLineEndings();
            // Split で各行に分割した後、Trim で前後のスペースとタブを取り除く。
            char[] charsToTrim = { ' ', '\t' };
            string[] strLines = strTemp.Split(System.Environment.NewLine).Select(x => x.Trim(charsToTrim)).ToArray();
            // 各行を連結して、一つに戻す。
            strTemp = String.Join("", strLines);

            if (strStatus.Contains("$"))
            {
                // ヴァーレントゥーガのテキスト用特殊記号「$」を改行に置換する。
                strTemp = strTemp.Replace("$", System.Environment.NewLine);
            }

            return strTemp;
        }

        public void ExecuteEvent()
        {
            //イベント実行
            var ev = this.ClassGameStatus.ListEvent
                        .Where(x => x.Name == this.ListClassScenarioInfo[this.NumberScenarioSelection].World)
                        .FirstOrDefault();
            if (ev != null)
            {
                var enviroment = new Enviroment();
                var evaluator = new Evaluator();
                evaluator.ClassGameStatus = this.ClassGameStatus;
                evaluator.Eval(ev.Root, enviroment);
                ev.Yet = false;
            }

            // テキストウィンドウを閉じる
            if (this.ClassGameStatus.TextWindow != null)
            {
                this.canvasTop.Children.Remove(this.ClassGameStatus.TextWindow);
                this.ClassGameStatus.TextWindow = null;
            }
        }

        // 勢力メニューウィンドウ
        private void SetWindowStrategyMenu()
        {
            if (this.ClassGameStatus.WindowStrategyMenu == null)
            {
                this.ClassGameStatus.WindowStrategyMenu = new UserControl005_StrategyMenu();
            }

            // 右下の隅に配置する
            this.ClassGameStatus.WindowStrategyMenu.SetData();
            this.canvasUIRightBottom.Children.Add(this.ClassGameStatus.WindowStrategyMenu);
            Canvas.SetLeft(this.ClassGameStatus.WindowStrategyMenu, this.canvasUIRightBottom.Width - this.ClassGameStatus.WindowStrategyMenu.Width);
            Canvas.SetTop(this.ClassGameStatus.WindowStrategyMenu, this.canvasUIRightBottom.Height - this.ClassGameStatus.WindowStrategyMenu.Height);

            // ターン数も表示する
            this.ClassGameStatus.WindowStrategyMenu.DisplayTurn(this);

        }

        private SolidColorBrush ReturnBaseColor()
        {
            return new SolidColorBrush(Color.FromRgb(190, 178, 175));
        }

        #endregion

    }
}
