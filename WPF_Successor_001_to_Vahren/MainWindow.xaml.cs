using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;
using WPF_Successor_001_to_Vahren._005_Class;
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
    public partial class MainWindow : Window
    {
        #region Prop

        #region IsEng
        public bool IsEng
        {
            get
            {
                return false;
            }
            set
            {

            }
        }
        #endregion

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

        #region CanvasMainWidth
        public int CanvasMainWidth
        {
            get
            {
                return 1800;
            }
            set
            {

            }
        }
        #endregion

        #region CanvasMainHeight
        public int CanvasMainHeight
        {
            get
            {
                return 1000;
            }
            set
            {

            }
        }
        #endregion

        #region フェード関係
        #region FadeOut
        public bool FadeOut
        {
            get
            {
                return _fadeOut;
            }
            set
            {
                _fadeOut = value;
            }
        }
        #endregion

        #region FadeOutExecution
        public bool FadeOutExecution
        {
            get
            {
                return _fadeOutExecution;
            }
            set
            {
                _fadeOutExecution = value;
            }
        }
        #endregion

        #region FadeIn
        public bool FadeIn
        {
            get
            {
                return _fadeIn;
            }
            set
            {
                _fadeIn = value;
            }
        }
        #endregion

        #region FadeInExecution
        public bool FadeInExecution
        {
            get
            {
                return _fadeInExecution;
            }
            set
            {
                _fadeInExecution = value;
            }
        }
        #endregion

        #region AfterFadeIn
        public bool AfterFadeIn
        {
            get
            {
                return _afterFadeIn;
            }
            set
            {
                _afterFadeIn = value;
            }
        }
        #endregion

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

        #region Dir001_Warehouse
        public string Dir001_Warehouse
        {
            get
            {
                return System.IO.Path.Combine(Environment.CurrentDirectory, "001_Warehouse");
            }
        }
        #endregion

        #region FileOrderDocument
        public string FileOrderDocument
        {
            get
            {
                return System.IO.Path.Combine(Dir001_Warehouse, "OrderDocument.txt");
            }
        }
        #endregion

        #region ClassConfigGameTitle
        public ClassConfigGameTitle ClassConfigGameTitle
        {
            get
            {
                return _classConfigGameTitle;
            }
            set { _classConfigGameTitle = value; }
        }
        #endregion

        #region NowNumberGameTitle
        public int NowNumberGameTitle
        {
            get
            {
                return _nowNumberGameTitle;
            }
            set { _nowNumberGameTitle = value; }
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

        #region ClassGameStatus
        private ClassGameStatus _classGameStatus = new ClassGameStatus();

        public ClassGameStatus ClassGameStatus
        {
            get { return _classGameStatus; }
            set { _classGameStatus = value; }
        }

        #endregion

        #endregion

        #region PrivateField
        private int _numberScenarioSelection;
        private List<ClassScenarioInfo> _listClassScenarioInfo = new List<ClassScenarioInfo>();
        private int _difficultyLevel = 0;
        private int _nowNumberGameTitle = 0;
        private bool _fadeOut = false;
        private bool _fadeOutExecution = false;
        private bool _fadeIn = false;
        private bool _fadeInExecution = false;
        private bool _afterFadeIn = false;

        private int _sizeClientWinWidth = 0;
        private int _sizeClientWinHeight = 0;

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
        public delegate void DelegateMapRenderedFromBattle();
        public DelegateMapRenderedFromBattle? delegateMapRenderedFromBattle = null;

        public DispatcherTimer timerAfterFadeIn = new DispatcherTimer(DispatcherPriority.Background);
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

        private static void KeepInterval(DispatcherTimer timer)
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

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // ESCキーを押すと終了する。
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
            // F11キーを押すとフルスクリーン状態を切り替える。
            else if (e.Key == Key.F11)
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    // 最大化中なら、通常サイズにする。
                    this.WindowStyle = WindowStyle.SingleBorderWindow; // タイトルバーと境界線を表示します。
                    this.WindowState = WindowState.Normal;
                }
                else
                {
                    // 通常サイズか最小化中なら、最大化する。
                    this.WindowStyle = WindowStyle.None; // タイトルバーと境界線を非表示にします。
                    this.WindowState = WindowState.Maximized;
                }
            }
        }

        private void canvasTop_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // クライアント領域を知る方法
            var si = e.NewSize;
            this._sizeClientWinWidth = (int)si.Width;
            this._sizeClientWinHeight = (int)si.Height;

            // canvasMain を常にウインドウの中央に置く。
            this.canvasMain.Margin = new Thickness()
            {
                Top = (this._sizeClientWinHeight / 2) - (this.CanvasMainHeight / 2),
                Left = (this._sizeClientWinWidth / 2) - (this.CanvasMainWidth / 2)
            };

            double newTop, newLeft;
            // canvasUI をウインドウの右上隅に置く。
            if (this._sizeClientWinHeight > this.CanvasMainHeight)
            {
                newTop = (this._sizeClientWinHeight / 2) - (this.CanvasMainHeight / 2);
            }
            else
            {
                // ウインドウの高さが低い場合は上端に合わせる。
                newTop = 0;
            }
            if (this._sizeClientWinWidth > this.CanvasMainWidth)
            {
                newLeft = (this._sizeClientWinWidth / 2) - (this.CanvasMainWidth / 2);
            }
            else
            {
                // ウインドウの横幅が狭い場合は右端に合わせる。
                newLeft = this._sizeClientWinWidth - this.CanvasMainWidth;
            }
            this.canvasUIRightTop.Margin = new Thickness()
            {
                Top = newTop,
                Left = newLeft
            };
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
        private void ScenarioSelection_MouseRightButtonUp(object sender, MouseEventArgs e)
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

            this.ClassGameStatus.Camera = new Point()
            {
                X = ((-this.CanvasMainWidth) + (this.CanvasMainWidth / 2)),
                Y = ((-this.CanvasMainHeight) + (this.CanvasMainHeight / 2)),
            };

            this.FadeOut = true;

            this.NowSituation = Situation.SelectGroup;
            this.delegateMapRendered = SetMapStrategy;

            this.FadeIn = true;
        }

        #region マップ移動
        private void GridMapStrategy_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            var ri = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapStrategy);
            if (ri == null)
            {
                throw new Exception();
            }

            this.ClassGameStatus.IsMouse = true;
            this.ClassGameStatus.StartPoint = e.GetPosition(ri);

            e.Handled = true;
        }
        private void GridMapStrategy_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ClassGameStatus.IsMouse = false;

            e.Handled = true;
        }
        private void GridMapStrategy_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.ClassGameStatus.IsMouse == false)
            {
                return;
            }

            var ri = (Grid)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.gridMapStrategy);
            if (ri == null)
            {
                throw new Exception();
            }
            var ri2 = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapStrategy);
            if (ri == null)
            {
                throw new Exception();
            }

            Point getPoint = e.GetPosition(ri2);
            this.ClassGameStatus.CurrentPoint = getPoint;

            var thickness = new Thickness();
            thickness.Left = ri.Margin.Left + (this.ClassGameStatus.CurrentPoint.X - this.ClassGameStatus.StartPoint.X);
            thickness.Top = ri.Margin.Top + (this.ClassGameStatus.CurrentPoint.Y - this.ClassGameStatus.StartPoint.Y);
            ri.Margin = thickness;

            this.ClassGameStatus.StartPoint = this.ClassGameStatus.CurrentPoint;

            e.Handled = true;
        }
        private void GridMapStrategy_MouseLeave(object sender, MouseEventArgs e)
        {
            this.ClassGameStatus.IsMouse = false;

            e.Handled = true;
        }

        #region Battle
        private void CanvasMapBattle_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            var ri = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapBattle);
            if (ri == null)
            {
                throw new Exception();
            }

            this.ClassGameStatus.IsMouse = true;
            this.ClassGameStatus.StartPointBattle = e.GetPosition(ri);

            e.Handled = true;
        }
        private void CanvasMapBattle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.ClassGameStatus.IsMouse = false;

            e.Handled = true;
        }
        private void CanvasMapBattle_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.ClassGameStatus.IsMouse == false)
            {
                return;
            }

            var ri = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.gridMapBattle);
            if (ri == null)
            {
                throw new Exception();
            }
            var ri2 = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapBattle);
            if (ri == null)
            {
                throw new Exception();
            }

            Point getPoint = e.GetPosition(ri2);
            this.ClassGameStatus.CurrentPointBattle = getPoint;

            var thickness = new Thickness();
            thickness.Left = ri2.Margin.Left + (this.ClassGameStatus.CurrentPointBattle.X - this.ClassGameStatus.StartPointBattle.X);
            thickness.Top = ri2.Margin.Top + (this.ClassGameStatus.CurrentPointBattle.Y - this.ClassGameStatus.StartPointBattle.Y);
            ri2.Margin = thickness;

            //this.ClassGameStatus.StartPointBattle = this.ClassGameStatus.CurrentPointBattle;

            e.Handled = true;
        }
        private void CanvasMapBattle_MouseLeave(object sender, MouseEventArgs e)
        {
            this.ClassGameStatus.IsMouse = false;

            e.Handled = true;
        }

        #endregion

        #endregion

        private void GridMapStrategy_MouseRightButtonUp(object sender, MouseEventArgs e)
        {
            switch (this.NowSituation)
            {
                case Situation.Title:
                    break;
                case Situation.MainMenu:
                    break;
                case Situation.SelectGroup:

                    this.FadeOut = true;

                    this.delegateMainWindowContentRendered = SetWindowMainMenu;

                    this.FadeIn = true;

                    break;
                case Situation.TextWindow_Conversation:
                    break;
                case Situation.PlayerTurn:
                    break;
                case Situation.EnemyTurn:
                    break;
                case Situation.InfoWindowMini:
                    break;
                case Situation.DebugGame:
                    break;
                case Situation.PlayerTurnEnemyCityLeftClick:
                    break;
                case Situation.PlayerTurnPlayerCityLeftClick:
                    break;
                case Situation.Battle_InfoWindowMini:
                    break;
                case Situation.Battle:
                    break;
                case Situation.BattleStop:
                    break;
                case Situation.Game:
                    break;
                case Situation.GenusList:
                    break;
                case Situation.ToolList:
                    break;
                case Situation.GameStop:
                    break;
                case Situation.PreparationBattle:
                    break;
                case Situation.PreparationBattle_UnitList:
                    break;
                case Situation.PreparationBattle_MiniWindow:
                    break;
                default:
                    break;
            }
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
                canvas.MouseRightButtonUp += ScenarioSelection_MouseRightButtonUp;
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

                    tbDate1.Text = String.Join(
                                        Environment.NewLine,
                                        this.ListClassScenarioInfo[tag].ScenarioIntroduce
                                            .Split(Environment.NewLine)
                                            .Select(x => x.TrimStart())
                                            .Select(y => y.TrimEnd())
                                            .ToArray()
                                    )
                                    .Replace("〇", "　");
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
                canvas.MouseRightButtonUp += ScenarioSelection_MouseRightButtonUp;
                {
                    // 枠
                    var rectangleInfo = new Rectangle();
                    rectangleInfo.Fill = ReturnBaseColor();
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
                    image.Stretch = Stretch.Fill;
                    ImageBehavior.SetAnimatedSource(image, bi);
                    canvas.Children.Add(image);
                }
                this.canvasMain.Children.Add(canvas);
            }

        }
        private void ButtonSelectionCity_click(object sender, EventArgs e)
        {
            if (this.NowSituation == Situation.PlayerTurn)
            {
                DisplayCitySelection(sender);
            }
            else if (this.NowSituation == Situation.SelectGroup)
            {
                //await DoWork(new SystemFunctionLiteral());
                //MessageBox.Show("aa");
                DisplayPowerSelection(sender);
            }
        }
        private void ButtonSelectionCity_RightKeyDown(object sender, EventArgs e)
        {
            if (this.NowSituation == Situation.SelectGroup)
            {
                return; //勢力選択中は出撃しない。
            }

            var cast = (Button)sender;
            if (cast.Tag is not ClassPowerAndCity)
            {
                return;
            }
            var classPowerAndCity = (ClassPowerAndCity)cast.Tag;

            //自ターンチェック
            //CPUタイムに押されても平気なように

            //所属チェック
            if (classPowerAndCity.ClassPower.NameTag == this.ClassGameStatus.SelectionPowerAndCity.ClassPower.NameTag)
            {
                return; //自国には攻め込まない。
            }

            ////隣接チェック
            //国に関係なく隣接都市名を抽出
            List<string> NameRinsetuSpot = new List<string>();
            foreach (var item in this.ListClassScenarioInfo[this.NumberScenarioSelection].ListLinkSpot)
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
                var ge = this.ClassGameStatus.AllListSpot.Where(x => x.NameTag == item).FirstOrDefault();
                if (ge == null)
                {
                    continue;
                }
                if (ge.PowerNameTag != this.ClassGameStatus.SelectionPowerAndCity.ClassPower.NameTag)
                {
                    continue;
                }
                classSpots.Add(ge);
            }
            if (classSpots.Count == 0)
            {
                //自国と隣接してないので出撃できない。
                return;
            }
            MessageBox.Show("出撃します");

            Uri uri = new Uri("/Page010_SortieMenu.xaml", UriKind.Relative);
            Frame frame = new Frame();
            frame.Source = uri;
            frame.Margin = new Thickness(0, 0, 0, 0);
            frame.Name = StringName.windowSortieMenu;
            this.canvasMain.Children.Add(frame);
            Application.Current.Properties["window"] = this;
            Application.Current.Properties["spots"] = classSpots;
            Application.Current.Properties["selectSpots"] = classPowerAndCity;
        }

        private void DisplayCitySelection(object sender)
        {
            var cast = (Button)sender;
            if (cast.Tag is not ClassPowerAndCity)
            {
                return;
            }

            var classPowerAndCity = (ClassPowerAndCity)cast.Tag;

            Uri uri = new Uri("/Page001_Conscription.xaml", UriKind.Relative);
            Frame frame = new Frame();
            frame.Source = uri;
            frame.Margin = new Thickness(0, 0, 0, 0);
            frame.Name = StringName.windowConscription;
            this.canvasMain.Children.Add(frame);
            Application.Current.Properties["window"] = this;
            Application.Current.Properties["ClassPowerAndCity"] = classPowerAndCity;
        }
        /// <summary>
        /// 勢力選択画面での勢力情報表示
        /// 決定ボタン押下時「ButtonSelectionPowerDecide_click」
        /// </summary>
        /// <param name="sender"></param>
        private void DisplayPowerSelection(object sender)
        {
            var cast = (Button)sender;
            if (cast.Tag is not ClassPowerAndCity)
            {
                return;
            }

            var classPowerAndCity = (ClassPowerAndCity)cast.Tag;
            if (classPowerAndCity.ClassPower.MasterTag == string.Empty)
            {
                return; //選択領地のマスター名が空なら、勢力情報画面を表示しない。
            }

            var ri = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapStrategy);
            if (ri == null)
            {
                return;
            }

            {
                var windowSelectionPower = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowSelectionPower);
                if (windowSelectionPower != null)
                {
                    this.canvasMain.Children.Remove(windowSelectionPower);
                }
            }

            {
                var ri2 = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapStrategy);
                if (ri2 != null)
                {
                    var ri3 = (Grid)LogicalTreeHelper.FindLogicalNode(ri2, StringName.windowSelectionPowerMini);
                    if (ri3 != null)
                    {
                        ri2.Children.Remove(ri3);
                    }
                }
            }


            var result = this.ClassGameStatus.AllListSpot.Where(x => x.ListMember.Contains(classPowerAndCity.ClassPower.MasterTag)).FirstOrDefault();
            if (result != null)
            {
                this.ClassGameStatus.SelectionCityPoint = new Point
                    (
                    result.X + (this.ClassGameStatus.GridCityWidthAndHeight.X / 2),
                    result.Y + (this.ClassGameStatus.GridCityWidthAndHeight.Y / 2)
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
            canvas.MouseRightButtonUp += SelectionPower_MouseRightButtonUp;
            {
                //LeftTop
                {
                    Grid gridSelectionPower = new Grid();
                    gridSelectionPower.Background = Brushes.DarkGray;
                    gridSelectionPower.Height = this.ClassConfigGameTitle.WindowSelectionPowerImage.Y + spaceMargin * 2;
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
                        tbDate1.Text = classPowerAndCity.ClassPower.Text.Replace(" ", String.Empty).Replace("\t", String.Empty).Replace("　", String.Empty);
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

                    foreach (var item in classPowerAndCity.ClassPower.ListMember)
                    {
                        var ext = this.ClassGameStatus.AllListSpot.Where(x => x.NameTag == item);
                        foreach (var itemSpot in ext)
                        {
                            foreach (var itemMember in itemSpot.ListMember.Select((value, index) => (value, index)))
                            {
                                var unit = this.ClassGameStatus.ListUnit.Where(x => x.NameTag == itemMember.value).FirstOrDefault();
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
                    gridSelectionPower.Height = this.ClassConfigGameTitle.WindowSelectionPowerImage.Y + spaceMargin;
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

            ri.Children.Add(canvas);
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
                itemSpot.UnitGroup = new List<ClassHorizontalUnit>();
                //メンバー配置
                foreach (var itemMember in itemSpot.ListMember)
                {
                    var info = this.ClassGameStatus.ListUnit.Where(x => x.NameTag.Contains(itemMember)).FirstOrDefault();
                    if (info == null)
                    {
                        continue;
                    }
                    var classUnit = new List<ClassUnit>();
                    var deep = info.DeepCopy();
                    deep.ID = this.ClassGameStatus.IDCount;
                    this.ClassGameStatus.SetIDCount();
                    classUnit.Add(deep);

                    //データ構造のミス
                    //ここでも以下のようにすべきだった
                    //var classUnit = new List<ClassUnit>();
                    //for (int i = 0; i < ListWanderingMonster.Item2; i++)
                    //{
                    //    var deep = info.DeepCopy();
                    //    deep.ID = this.ClassGameStatus.IDCount;
                    //    this.ClassGameStatus.SetIDCount();
                    //    classUnit.Add(deep);
                    //}

                    itemSpot.UnitGroup.Add(new ClassHorizontalUnit() { Spot = itemSpot, FlagDisplay = true, ListClassUnit = classUnit });
                }
                //中立配置
                foreach (var ListWanderingMonster in itemSpot.ListWanderingMonster)
                {
                    var info = this.ClassGameStatus.ListUnit.Where(x => x.NameTag.Contains(ListWanderingMonster.Item1)).FirstOrDefault();
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
            var ri = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapStrategy);
            if (ri == null)
            {
                return;
            }

            {
                var windowSelectionPower = (Canvas)LogicalTreeHelper.FindLogicalNode(ri, StringName.windowSelectionPower);
                if (windowSelectionPower != null)
                {
                    ri.Children.Remove(windowSelectionPower);
                }
            }

            var gridMapStrategy = (Grid)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.gridMapStrategy);
            if (gridMapStrategy == null)
            {
                return;
            }

            gridMapStrategy.Margin = new Thickness()
            {
                Left = -(this.CanvasMainWidth / 2),
                Top = -(this.CanvasMainHeight / 2)
            };
        }
        /// <summary>
        /// 勢力選択画面で取り消しした時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonSelectionPowerRemove_click(object sender, EventArgs e)
        {
            var ri = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapStrategy);
            if (ri == null)
            {
                return;
            }

            {
                var windowSelectionPower = (Canvas)LogicalTreeHelper.FindLogicalNode(ri, StringName.windowSelectionPower);
                if (windowSelectionPower != null)
                {
                    ri.Children.Remove(windowSelectionPower);
                }
            }
        }

        /// <summary>
        /// 勢力情報キャンバスを右クリックしたら閉じるようにする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectionPower_MouseRightButtonUp(object sender, MouseEventArgs e)
        {
            ButtonSelectionPowerRemove_click(sender, e);

            e.Handled = true;
        }

        #endregion

        private void WindowMapBattleUnit_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            var can = (Canvas)sender;
            var bor = (Border)can.Parent;
            long name = long.Parse((string)(can).Tag);
            foreach (var item in this.ClassGameStatus.ClassBattleUnits.SortieUnitGroup)
            {
                var re = item.ListClassUnit.Where(x => x.ID == name).FirstOrDefault();
                if (re != null)
                {
                    bor.BorderThickness = new Thickness()
                    {
                        Left = 3,
                        Top = 3,
                        Right = 3,
                        Bottom = 3
                    };
                    bor.BorderBrush = Brushes.DarkRed;
                    re.FlagMove = true;
                    break;
                }
            }
        }
        private void windowMapBattle_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            var ri = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapBattle);
            if (ri == null)
            {
                throw new Exception();
            }

            //SortieUnitGroupではなくプレイヤー側でないとダメ
            foreach (var item in this.ClassGameStatus.ClassBattleUnits.SortieUnitGroup)
            {
                var re = item.ListClassUnit.Where(x => x.FlagMove == true).FirstOrDefault();
                if (re != null)
                {
                    var nowOrderPosi = e.GetPosition(ri);
                    if (re.FlagMoving = true && re.OrderPosi != nowOrderPosi)
                    {
                        re.FlagMoveDispose = true;
                    }
                    re.OrderPosi = nowOrderPosi;
                    re.FlagMove = false;
                    re.FlagMoving = false;

                    var re2 = (Border)LogicalTreeHelper.FindLogicalNode(ri, "border" + re.ID.ToString());
                    re2.BorderThickness = new Thickness()
                    {
                        Left = 0,
                        Top = 0,
                        Right = 0,
                        Bottom = 0
                    };
                    break;
                }
            }

        }


        #region Method
        private void ReadFileOrderDocument()
        {
            if (File.Exists(this.FileOrderDocument) == false)
            {
                //File無し
                throw new NotImplementedException();
            }

            //あったので読み込む
            string[] readAllLines;
            readAllLines = File.ReadAllLines(this.FileOrderDocument);
            readAllLines = readAllLines.Select(line => line.Trim()).ToArray();

            foreach (var item in readAllLines)
            {
                if (item.Length < 1)
                {
                    //  空行
                    continue;
                }

                if (item[0] == '#')
                {
                    //  コメント行
                    continue;
                }

                var resultSplit = item.Split(',');
                switch (resultSplit[0])
                {
                    case "DefaultGameTitle":
                        {
                            if (resultSplit.Length < 2)
                            {
                                //デフォゲーム無し
                                throw new NotImplementedException();
                            }
                            string a = System.IO.Path.Combine(this.Dir001_Warehouse, resultSplit[1]);
                            var b = System.IO.Directory.CreateDirectory(a);
                            ClassConfigGameTitle.DirectoryGameTitle.Add(b);
                        }
                        break;
                    case "Language":
                        if (resultSplit[1] == "japan" ||
                            resultSplit[1] == "Japan")
                        {
                            this.IsEng = false;
                        }
                        else
                        {
                            this.IsEng = true;
                        }
                        break;
                    default:
                        break;
                }
                //識別子処理の終わり
            }
            //一行毎の読み込み終わり
        }

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

            Canvas.SetZIndex(this.canvasMain, 90);
            Canvas.SetZIndex(this.canvasUIRightTop, 99);

            SetWindowTitle(targetNumber: 0);
        }

        /// <summary>
        /// 戦闘画面を作成する処理
        /// </summary>
        public void SetBattleMap()
        {
            //var ri = (Grid)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.gridMapStrategy);
            //if (ri == null)
            //{
            //    throw new Exception();
            //}
            //var ri2 = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapStrategy);
            //if (ri == null)
            //{
            //    throw new Exception();
            //}
            //var ri3 = (Grid)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.canvasWindowStrategy);
            //if (ri3 == null)
            //{
            //    throw new Exception();
            //}
            //var ri4 = (Grid)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowConscription);
            //if (ri4 == null)
            //{
            //    throw new Exception();
            //}
            //var ri5 = (Grid)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowSortieMenu);
            //if (ri5 == null)
            //{
            //    throw new Exception();
            //}

            //this.canvasMain.Children.Remove(ri);
            //this.canvasMain.Children.Remove(ri2);

            //マップそのもの
            Canvas canvas = new Canvas();
            int takasaMapTip = 32;
            int yokoMapTip = 48;
            canvas.Name = StringName.windowMapBattle;
            canvas.Background = Brushes.Black;
            canvas.MouseMove += CanvasMapBattle_MouseMove;
            canvas.MouseLeftButtonUp += CanvasMapBattle_MouseLeftButtonUp;
            canvas.MouseLeftButtonDown += CanvasMapBattle_MouseLeftButtonDown;
            canvas.MouseLeave += CanvasMapBattle_MouseLeave;
            canvas.MouseRightButtonDown += windowMapBattle_MouseRightButtonDown;
            {

                if (this.ClassGameStatus.ClassBattleUnits.ClassMapBattle == null)
                {
                    {
                        canvas.Width = 160
                                        + 80;
                        canvas.Height = 160;
                        canvas.Margin = new Thickness()
                        {
                            Left = 160 / 2,
                            Top = (this._sizeClientWinHeight / 2) - (canvas.Height / 2)
                        };
                    }
                }
                else
                {
                    {
                        canvas.Width = this.ClassGameStatus.ClassBattleUnits.ClassMapBattle.MapData[0].Count * 32
                                            + (this.ClassGameStatus.ClassBattleUnits.ClassMapBattle.MapData.Count * 16);
                        canvas.Height = this.ClassGameStatus.ClassBattleUnits.ClassMapBattle.MapData.Count * 32;
                        canvas.Margin = new Thickness()
                        {
                            Left = ((
                                    (this.CanvasMainWidth / 2) - (this._sizeClientWinWidth / 2)
                                    ))
                                        +
                                    (this._sizeClientWinWidth / 2) - ((this.ClassGameStatus.ClassBattleUnits.ClassMapBattle.MapData[0].Count * 32) / 2),
                            Top = (this._sizeClientWinHeight / 2) - (canvas.Height / 2)
                        };
                        //RotateTransform rotateTransform2 = new RotateTransform(0);
                        ////rotateTransform2.CenterX = 25;
                        ////rotateTransform2.CenterY = 50;
                        //canvas.RenderTransform = rotateTransform2;
                    }

                    // get target path.
                    List<string> strings = new List<string>();
                    strings.Add(this.ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName);
                    strings.Add("015_BattleMapCellImage");
                    string cellImagePath = System.IO.Path.Combine(strings.ToArray());
                    // get file.
                    var files = System.IO.Directory.EnumerateFiles(
                        cellImagePath,
                        "*.png",
                        System.IO.SearchOption.AllDirectories
                        );
                    Dictionary<string, string> map = new Dictionary<string, string>();
                    foreach (var item in files)
                    {
                        map.Add(System.IO.Path.GetFileNameWithoutExtension(item), item);
                    }

                    //double naname = Math.Sqrt((48 / 2) * (48 / 2)) + ((16) * (16));

                    foreach (var itemCol in this.ClassGameStatus.ClassBattleUnits.ClassMapBattle.MapData
                                            .Select((value, index) => (value, index)))
                    {
                        foreach (var itemRow in itemCol.value.Select((value, index) => (value, index)))
                        {
                            map.TryGetValue(itemRow.value.Tip, out string? value);
                            if (value == null) continue;

                            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
                            var bi = new BitmapImage(new Uri(value));
                            ImageBrush image = new ImageBrush();
                            image.Stretch = Stretch.Fill;
                            image.ImageSource = bi;

                            //RotateTransform rotateTransform2 = new RotateTransform(45);
                            //rotateTransform2.CenterX = 25;
                            //rotateTransform2.CenterY = 50;
                            //image.RelativeTransform = rotateTransform2;

                            path.Fill = image;
                            path.Stretch = Stretch.Fill;
                            path.StrokeThickness = 0;
                            path.Data = Geometry.Parse("M 0," + takasaMapTip / 2 + " L " + yokoMapTip / 2 + "," + takasaMapTip + " L " + yokoMapTip + "," + takasaMapTip / 2 + " L " + yokoMapTip / 2 + ",0 Z");
                            path.Margin = new Thickness()
                            {
                                Left = (itemCol.index * (yokoMapTip / 2)) + (itemRow.index * (yokoMapTip / 2)),
                                Top = ((canvas.Height / 2) + (itemCol.index * (takasaMapTip / 2)) + (itemRow.index * (-(takasaMapTip / 2)))) - takasaMapTip / 2
                            };
                            canvas.Children.Add(path);
                        }
                    }
                }

                Canvas backCanvas = new Canvas();
                backCanvas.Name = StringName.gridMapBattle;
                backCanvas.Background = Brushes.AliceBlue;
                backCanvas.Width = this._sizeClientWinWidth;
                backCanvas.Height = this._sizeClientWinHeight;

                backCanvas.Margin = new Thickness()
                {
                    Left = (this.CanvasMainWidth / 2) - (this._sizeClientWinWidth / 2),
                    Top = (this.CanvasMainHeight / 2) - (this._sizeClientWinHeight / 2)
                };
                backCanvas.Children.Add(canvas);

                Canvas.SetZIndex(backCanvas, 98);
                this.canvasMain.Children.Add(backCanvas);
            }

            ////出撃ユニット
            {
                //中点
                decimal countMeHalf = Math.Floor((decimal)this.ClassGameStatus.ClassBattleUnits.SortieUnitGroup.Count / 2);
                //線の端
                Point hidariTakasa = new Point(0, canvas.Height / 2);
                Point migiTakasa = new Point(canvas.Width / 2, canvas.Height);
                //ユニットの端の位置を算出
                if (this.ClassGameStatus.ClassBattleUnits.SortieUnitGroup.Count % 2 == 0)
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
                            .ClassBattleUnits.SortieUnitGroup
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
                        string path = GetPathTipImage(itemListClassUnit);

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
                        canvasChip.PreviewMouseLeftButtonDown += WindowMapBattleUnit_MouseLeftButtonDown;
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
                            Top = top - 192
                        };
                        itemListClassUnit.value.NowPosi = new Point()
                        {
                            X = left,
                            Y = top - 192
                        };
                        itemListClassUnit.value.OrderPosi = new Point()
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
                            .ClassBattleUnits.SortieUnitGroup
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
                        canvasChip.PreviewMouseLeftButtonDown += WindowMapBattleUnit_MouseLeftButtonDown;
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
                        itemListClassUnit.value.NowPosi = new Point()
                        {
                            X = left,
                            Y = top - 86
                        };
                        itemListClassUnit.value.OrderPosi = new Point()
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
                            .ClassBattleUnits.SortieUnitGroup
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
                        canvasChip.PreviewMouseLeftButtonDown += WindowMapBattleUnit_MouseLeftButtonDown;
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
                        itemListClassUnit.value.NowPosi = new Point()
                        {
                            X = left,
                            Y = top
                        };
                        itemListClassUnit.value.OrderPosi = new Point()
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
                decimal countMeHalf = Math.Floor((decimal)this.ClassGameStatus.ClassBattleUnits.DefUnitGroup.Count / 2);
                //線の端
                Point hidariTakasa = new Point(canvas.Width / 2, 0);
                Point migiTakasa = new Point(canvas.Width, canvas.Height / 2);
                //ユニットの端の位置を算出
                if (this.ClassGameStatus.ClassBattleUnits.DefUnitGroup.Count % 2 == 0)
                {
                    ////偶数
                    //これは正しくないが、案が思い浮かばない
                    hidariTakasa.X = (canvas.Width * 0.75) - ((double)countMeHalf * 32);
                    migiTakasa.X = (canvas.Width * 0.75) + ((double)countMeHalf * 32);

                    hidariTakasa.Y = (canvas.Height * 0.25) - ((double)countMeHalf * (takasaMapTip / 2));
                    migiTakasa.Y = (canvas.Height * 0.25) + ((double)countMeHalf * (takasaMapTip / 2));
                }
                else
                {
                    ////奇数
                    //これは正しくないが、案が思い浮かばない
                    hidariTakasa.X = (canvas.Width * 0.75) - (((double)countMeHalf + 1) * 32);
                    migiTakasa.X = (canvas.Width * 0.75) + (((double)countMeHalf + 1) * 32);

                    hidariTakasa.Y = (canvas.Height * 0.25) - (((double)countMeHalf + 1) * (takasaMapTip / 2));
                    migiTakasa.Y = (canvas.Height * 0.25) + (((double)countMeHalf + 1) * (takasaMapTip / 2));
                }

                //防衛前衛
                foreach (var item in this.ClassGameStatus
                            .ClassBattleUnits.DefUnitGroup
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
                        //今は後回し
                        canvasChip.PreviewMouseLeftButtonDown += WindowMapBattleUnit_MouseLeftButtonDown;
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
                        itemListClassUnit.value.NowPosi = new Point()
                        {
                            X = left,
                            Y = top + 192
                        };
                        itemListClassUnit.value.OrderPosi = new Point()
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
                            .ClassBattleUnits.DefUnitGroup
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
                        canvasChip.PreviewMouseLeftButtonDown += WindowMapBattleUnit_MouseLeftButtonDown;
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
                        itemListClassUnit.value.NowPosi = new Point()
                        {
                            X = left,
                            Y = top + 86
                        };
                        itemListClassUnit.value.OrderPosi = new Point()
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
                            .ClassBattleUnits.DefUnitGroup
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
                        canvasChip.PreviewMouseLeftButtonDown += WindowMapBattleUnit_MouseLeftButtonDown;
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
                        itemListClassUnit.value.NowPosi = new Point()
                        {
                            X = left,
                            Y = top
                        };
                        itemListClassUnit.value.OrderPosi = new Point()
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


            this.timerAfterFadeIn = new DispatcherTimer(DispatcherPriority.Background);
            this.timerAfterFadeIn.Interval = TimeSpan.FromSeconds((double)1 / 60);
            this.timerAfterFadeIn.Tick -= (x, s) =>
            {
                TimerAction60FPSAfterFadeInDecidePower();
                KeepInterval(this.timerAfterFadeIn);
            };
            this.timerAfterFadeIn.Tick += (x, s) =>
            {
                TimerAction60FPSAfterFadeInBattleStart();
                KeepInterval(this.timerAfterFadeIn);
            };
            this.timerAfterFadeIn.Start();
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
                    
                    Canvas.SetZIndex(button, 99);

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
        private void SetWindowMainMenu()
        {
            this.canvasMain.Children.Clear();
            this.canvasUIRightTop.Children.Clear();

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
                canvas.MouseRightButtonUp += ScenarioSelection_MouseRightButtonUp;
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
                        button.MouseRightButtonUp += Disable_MouseEvent;

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
                canvas.MouseRightButtonUp += ScenarioSelection_MouseRightButtonUp;
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
                        button.MouseRightButtonUp += Disable_MouseEvent;

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
                            ClassGameStatus.ListClassMapBattle.Add(GetClassMapBattle(getData.Value));
                        }
                    }

                    // Map 終わり
                }

            }
        }

        private ClassMapBattle GetClassMapBattle(string value)
        {
            ClassMapBattle classMapBattle = new ClassMapBattle();

            // コメント行を取り除く
            {
                string[] line = value.Split(Environment.NewLine).ToArray();
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i].Contains("//") == true)
                    {
                        var data = line[i].Split("//");
                        line[i] = String.Concat(data[0], Environment.NewLine);
                    }
                }
                value = String.Join(Environment.NewLine, line);
            }

            int eleNumber = 0;
            Dictionary<string, string> map = new Dictionary<string, string>();
            while (true)
            {
                {
                    var ele =
                        new Regex(GetPat("ele" + eleNumber), RegexOptions.IgnoreCase)
                        .Matches(value);
                    var first = CheckMatchElement(ele);
                    if (first == null)
                    {
                        break;
                    }
                    else
                    {
                        map.Add("ele" + eleNumber, first.Value);
                    }
                }
                eleNumber++;
            }

            //name
            {
                var name =
                    new Regex(GetPat("name"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(name);
                if (first == null)
                {
                    classMapBattle.Name = String.Empty;
                }
                else
                {
                    classMapBattle.Name = first.Value;
                }
            }

            //tag name
            {
                var nameTag = new Regex(GetPatTag("map"), RegexOptions.IgnoreCase)
                                .Matches(value);
                var first = CheckMatchElement(nameTag);
                if (first == null)
                {
                    throw new Exception();
                }
                classMapBattle.TagName = first.Value.Replace(Environment.NewLine, "");
            }

            //data
            {
                var data =
                    new Regex(GetPatComma("data"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(data);
                if (first == null)
                {
                    classMapBattle.MapData = new List<List<MapDetail>>();
                }
                else
                {
                    classMapBattle.MapData.Add(new List<MapDetail>());
                    List<string> re = first.Value.Split(",").ToList();
                    //最後の改行を消す
                    re.RemoveAt(re.Count - 1);
                    for (int i = 0; i < re.Count; i++)
                    {
                        if (re[i] == "@")
                        {
                            classMapBattle.MapData.Add(new List<MapDetail>());
                            continue;
                        }
                        else
                        {
                            MapDetail mapDetail = new MapDetail();
                            map.TryGetValue(re[i].Replace(System.Environment.NewLine, string.Empty), out string? mapValue);
                            if (mapValue != null) mapDetail.Tip = mapValue;
                            classMapBattle.MapData[classMapBattle.MapData.Count - 1].Add(mapDetail);
                        }
                    }
                }
            }

            //最後の空行を消す
            if (classMapBattle.MapData[classMapBattle.MapData.Count - 1].Count == 0)
            {
                classMapBattle.MapData.RemoveAt(classMapBattle.MapData.Count - 1);
            }
            return classMapBattle;
        }
        /// <summary>
        /// シナリオ選択画面から移行する戦略マップ表示画面
        /// 次処理は恐らく「ButtonSelectionCity_click」
        /// </summary>
        private void SetMapStrategy()
        {
            this.canvasMain.Children.Clear();

            this.canvasMain.Background = Brushes.Black;

            {
                Canvas canvas = new Canvas();
                canvas.Height = this.CanvasMainHeight;
                canvas.Width = this.CanvasMainWidth;
                canvas.Margin = new Thickness()
                {
                    Left = 0,
                    Top = 0
                };
                canvas.Background = new SolidColorBrush(Color.FromRgb(39, 51, 54));

                canvas.Name = StringName.windowMapStrategy;
                canvas.MouseMove += GridMapStrategy_MouseMove;
                canvas.MouseLeftButtonDown += GridMapStrategy_MouseLeftButtonDown;
                canvas.MouseLeftButtonUp += GridMapStrategy_MouseLeftButtonUp;
                canvas.MouseRightButtonUp += GridMapStrategy_MouseRightButtonUp;
                canvas.MouseLeave += GridMapStrategy_MouseLeave;

                Point mapPoint = this.ClassGameStatus.Camera;

                Grid grid = new Grid();
                grid.Name = StringName.gridMapStrategy;
                grid.Height = this.CanvasMainHeight * 2;
                grid.Width = this.CanvasMainWidth * 2;
                grid.Margin = new Thickness()
                {
                    Left = -(this.CanvasMainWidth / 2),
                    Top = -(this.CanvasMainHeight / 2)
                };

                // mapImage読み込み
                {
                    List<string> strings = new List<string>();
                    strings.Add(ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName);
                    strings.Add("005_BackgroundImage");
                    strings.Add("015_MapImage");
                    strings.Add(this.ListClassScenarioInfo[this.NumberScenarioSelection].NameMapImageFile);
                    string path = System.IO.Path.Combine(strings.ToArray());

                    this.ClassGameStatus.CurrentPoint =
                            new Point((grid.Width / 2) - ((grid.Width / 2) / 2),
                                (grid.Height / 2) - ((grid.Height / 2) / 2));

                    BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                    Image img = new Image();
                    img.Height = grid.Height;
                    img.Width = grid.Width;
                    img.Stretch = Stretch.Fill;
                    img.Source = bitimg1;
                    img.Margin = new Thickness()
                    {
                        Left = 0,
                        Top = 0
                    };
                    grid.Children.Add(img);
                }

                //spot読み込み
                {
                    //現シナリオで使用するスポットを抽出する
                    List<ClassSpot> result = new List<ClassSpot>();
                    foreach (var item in this.ListClassScenarioInfo[this.NumberScenarioSelection].DisplayListSpot)
                    {
                        foreach (var item2 in this.ClassGameStatus.AllListSpot)
                        {
                            if (item == item2.NameTag)
                            {
                                result.Add(item2);
                            }
                        }
                    }

                    int hei = 32;

                    //spotをlineで繋ぐ
                    foreach (var item in this.ListClassScenarioInfo[this.NumberScenarioSelection].ListLinkSpot)
                    {
                        var ext1 = result.Where(x => x.NameTag == item.Item1).FirstOrDefault();
                        if (ext1 == null)
                        {
                            continue;
                        }
                        var ext2 = result.Where(x => x.NameTag == item.Item2).FirstOrDefault();
                        if (ext2 == null)
                        {
                            continue;
                        }
                        Line line = new Line();
                        line.X1 = ext1.X + (this.ClassGameStatus.GridCityWidthAndHeight.X / 2);
                        line.Y1 = ext1.Y + (hei / 2);
                        line.X2 = ext2.X + (this.ClassGameStatus.GridCityWidthAndHeight.X / 2);
                        line.Y2 = ext2.Y + (hei / 2);
                        line.Stroke = Brushes.Black;
                        line.StrokeThickness = 3;
                        grid.Children.Add(line);
                    }

                    //spotを出す
                    foreach (var item in result.Select((value, index) => (value, index)))
                    {
                        Grid gridButton = new Grid();
                        gridButton.HorizontalAlignment = HorizontalAlignment.Left;
                        gridButton.VerticalAlignment = VerticalAlignment.Top;
                        gridButton.Height = this.ClassGameStatus.GridCityWidthAndHeight.Y;
                        gridButton.Width = this.ClassGameStatus.GridCityWidthAndHeight.X;
                        gridButton.Tag = item.value.Index;
                        gridButton.Margin = new Thickness()
                        {
                            Left = item.value.X,
                            Top = item.value.Y
                        };
                        //grid.AllowDrop = false;

                        BitmapImage bitimg1 = new BitmapImage(new Uri(item.value.ImagePath));
                        Image img = new Image();
                        img.Stretch = Stretch.Fill;
                        img.Source = bitimg1;

                        int fontSizePlus = 5;
                        int widthIcon = 32;
                        TextBlock tbDate1 = new TextBlock();
                        tbDate1.HorizontalAlignment = HorizontalAlignment.Left;
                        tbDate1.VerticalAlignment = VerticalAlignment.Top;
                        tbDate1.FontSize = tbDate1.FontSize + fontSizePlus;
                        tbDate1.Text = item.value.Name;
                        tbDate1.Height = 40;
                        tbDate1.Margin = new Thickness { Left = 15, Top = hei + 10 };
                        gridButton.Children.Add(tbDate1);

                        Button button = new Button();
                        button.Name = StringName.buttonClassPowerAndCity + item.index;
                        button.HorizontalAlignment = HorizontalAlignment.Left;
                        button.VerticalAlignment = VerticalAlignment.Top;
                        button.Content = img;
                        button.Height = hei;
                        button.Width = widthIcon;
                        button.Margin = new Thickness
                        {
                            Left = (gridButton.Width / 2) - (bitimg1.Width / 2),
                            Top = 0
                        };

                        // その都市固有の情報を見る為に、勢力の持つスポットと、シナリオで登場するスポットを比較
                        bool ch = false;
                        for (int i = 0; i < ClassGameStatus.ListPower.Count; i++)
                        {
                            foreach (var item3 in ClassGameStatus.ListPower[i].ListMember)
                            {
                                if (item3 == item.value.NameTag)
                                {
                                    // その都市固有の情報を見る為にも、勢力情報と都市情報を入れる
                                    var classPowerAndCity = new ClassPowerAndCity(ClassGameStatus.ListPower[i], item.value);
                                    button.Tag = classPowerAndCity;
                                    //ついでに、スポットの属する勢力名を設定
                                    var ge = this.ClassGameStatus.AllListSpot.Where(x => x.NameTag == item.value.NameTag).FirstOrDefault();
                                    if (ge != null)
                                    {
                                        ge.PowerNameTag = ClassGameStatus.ListPower[i].NameTag;
                                    }
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
                        if (button.Tag is not ClassPowerAndCity)
                        {
                            button.Tag = new ClassPowerAndCity(new ClassPower(), item.value);
                        }

                        button.Background = Brushes.Transparent;
                        button.Foreground = Brushes.Transparent;
                        button.Background = Brushes.Transparent;
                        button.BorderBrush = Brushes.Transparent;
                        //button.MouseEnter += WindowMainMenuLeftTop_MouseEnter;
                        button.Click += ButtonSelectionCity_click;
                        button.PreviewMouseRightButtonUp += ButtonSelectionCity_RightKeyDown;
                        gridButton.Children.Add(button);

                        grid.Children.Add(gridButton);
                    }

                }

                // 勢力ウィンドウを出す
                {
                    int fontSizePlus = 5;
                    int hei = 50;
                    int widthIcon = 350;

                    Grid gridPower = new Grid();
                    gridPower.HorizontalAlignment = HorizontalAlignment.Left;
                    gridPower.VerticalAlignment = VerticalAlignment.Top;
                    gridPower.Height = (ClassGameStatus.ListPower.Count) * 50;
                    gridPower.Width = widthIcon;
                    gridPower.Name = StringName.windowSelectionPowerMini;
                    //+leftにすれば有効サイズの右端になる
                    gridPower.Margin = new Thickness()
                    {
                        Left = this.canvasMain.Width + this.canvasMain.Margin.Left - widthIcon,
                        Top = 0
                    };
                    //grid.AllowDrop = false;
                    foreach (var item in ClassGameStatus.ListPower.Select((value, index) => (value, index)))
                    {
                        Grid gridPowerButton = new Grid();
                        BitmapImage bitimg1 = new BitmapImage(new Uri(item.value.FlagPath));
                        //旗を加工する処理を入れたい
                        Int32Rect rect = new Int32Rect(0, 0, 32, 32);
                        var destimg = new CroppedBitmap(bitimg1, rect);

                        Image img = new Image();
                        img.Stretch = Stretch.Fill;
                        //img.Source = bitimg1;
                        img.Source = destimg;
                        img.Height = hei;
                        img.Width = hei;
                        img.HorizontalAlignment = HorizontalAlignment.Left;
                        img.Margin = new Thickness { Left = 0, Top = 0 };
                        gridPowerButton.Children.Add(img);

                        TextBlock tbDate1 = new TextBlock();
                        tbDate1.HorizontalAlignment = HorizontalAlignment.Left;
                        tbDate1.VerticalAlignment = VerticalAlignment.Top;
                        tbDate1.FontSize = tbDate1.FontSize + fontSizePlus;
                        tbDate1.Text = item.value.Name;
                        tbDate1.Foreground = Brushes.White;
                        tbDate1.Height = hei;
                        tbDate1.Margin = new Thickness { Left = hei + 15, Top = 10 };

                        gridPowerButton.HorizontalAlignment = HorizontalAlignment.Left;
                        gridPowerButton.VerticalAlignment = VerticalAlignment.Top;
                        gridPowerButton.Height = hei;
                        gridPowerButton.Width = widthIcon;
                        gridPowerButton.Margin = new Thickness()
                        {
                            Left = 0,
                            Top = 0
                        };
                        gridPowerButton.Children.Add(tbDate1);

                        Button button = new Button();
                        button.HorizontalAlignment = HorizontalAlignment.Left;
                        button.VerticalAlignment = VerticalAlignment.Top;
                        button.Content = gridPowerButton;
                        button.Height = hei;
                        button.Width = widthIcon;
                        button.Margin = new Thickness
                        {
                            Left = 0,
                            Top = item.index * hei
                        };
                        button.Tag = item.value;
                        button.Background = Brushes.Black;
                        //button.MouseEnter += WindowMainMenuLeftTop_MouseEnter;
                        //button.Click += ScenarioSelectionButton_click;
                        gridPower.Children.Add(button);

                    }
                    Canvas.SetZIndex(gridPower, 99);
                    canvas.Children.Add(gridPower);
                }


                canvas.Children.Add(grid);
                this.canvasMain.Children.Add(canvas);
            }

        }

        private void SetMapStrategyFromBattle()
        {
            this.canvasMain.Children.Clear();

            this.canvasMain.Background = Brushes.Black;

            {
                Canvas canvas = new Canvas();
                canvas.Height = this.CanvasMainHeight;
                canvas.Width = this.CanvasMainWidth;
                canvas.Margin = new Thickness()
                {
                    Left = 0,
                    Top = 0
                };
                canvas.Background = new SolidColorBrush(Color.FromRgb(39, 51, 54));

                canvas.Name = StringName.windowMapStrategy;
                canvas.MouseMove += GridMapStrategy_MouseMove;
                canvas.MouseLeftButtonDown += GridMapStrategy_MouseLeftButtonDown;
                canvas.MouseLeftButtonUp += GridMapStrategy_MouseLeftButtonUp;
                canvas.MouseRightButtonUp += GridMapStrategy_MouseRightButtonUp;
                canvas.MouseLeave += GridMapStrategy_MouseLeave;

                Point mapPoint = this.ClassGameStatus.Camera;

                Grid grid = new Grid();
                grid.Name = StringName.gridMapStrategy;
                grid.Height = this.CanvasMainHeight * 2;
                grid.Width = this.CanvasMainWidth * 2;
                grid.Margin = new Thickness()
                {
                    Left = -(this.CanvasMainWidth / 2),
                    Top = -(this.CanvasMainHeight / 2)
                };

                // mapImage読み込み
                {
                    List<string> strings = new List<string>();
                    strings.Add(ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName);
                    strings.Add("005_BackgroundImage");
                    strings.Add("015_MapImage");
                    strings.Add(this.ListClassScenarioInfo[this.NumberScenarioSelection].NameMapImageFile);
                    string path = System.IO.Path.Combine(strings.ToArray());

                    this.ClassGameStatus.CurrentPoint =
                            new Point((grid.Width / 2) - ((grid.Width / 2) / 2),
                                (grid.Height / 2) - ((grid.Height / 2) / 2));

                    BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                    Image img = new Image();
                    img.Height = grid.Height;
                    img.Width = grid.Width;
                    img.Stretch = Stretch.Fill;
                    img.Source = bitimg1;
                    img.Margin = new Thickness()
                    {
                        Left = 0,
                        Top = 0
                    };
                    grid.Children.Add(img);
                }

                //spot読み込み
                {
                    //現シナリオで使用するスポットを抽出する
                    List<ClassSpot> result = new List<ClassSpot>();
                    foreach (var item in this.ListClassScenarioInfo[this.NumberScenarioSelection].DisplayListSpot)
                    {
                        foreach (var item2 in this.ClassGameStatus.AllListSpot)
                        {
                            if (item == item2.NameTag)
                            {
                                result.Add(item2);
                            }
                        }
                    }

                    int hei = 32;

                    //spotをlineで繋ぐ
                    foreach (var item in this.ListClassScenarioInfo[this.NumberScenarioSelection].ListLinkSpot)
                    {
                        var ext1 = result.Where(x => x.NameTag == item.Item1).FirstOrDefault();
                        if (ext1 == null)
                        {
                            continue;
                        }
                        var ext2 = result.Where(x => x.NameTag == item.Item2).FirstOrDefault();
                        if (ext2 == null)
                        {
                            continue;
                        }
                        Line line = new Line();
                        line.X1 = ext1.X + (this.ClassGameStatus.GridCityWidthAndHeight.X / 2);
                        line.Y1 = ext1.Y + (hei / 2);
                        line.X2 = ext2.X + (this.ClassGameStatus.GridCityWidthAndHeight.X / 2);
                        line.Y2 = ext2.Y + (hei / 2);
                        line.Stroke = Brushes.Black;
                        line.StrokeThickness = 3;
                        grid.Children.Add(line);
                    }

                    //spotを出す
                    foreach (var item in result.Select((value, index) => (value, index)))
                    {
                        Grid gridButton = new Grid();
                        gridButton.HorizontalAlignment = HorizontalAlignment.Left;
                        gridButton.VerticalAlignment = VerticalAlignment.Top;
                        gridButton.Height = this.ClassGameStatus.GridCityWidthAndHeight.Y;
                        gridButton.Width = this.ClassGameStatus.GridCityWidthAndHeight.X;
                        gridButton.Tag = item.value.Index;
                        gridButton.Margin = new Thickness()
                        {
                            Left = item.value.X,
                            Top = item.value.Y
                        };
                        //grid.AllowDrop = false;

                        BitmapImage bitimg1 = new BitmapImage(new Uri(item.value.ImagePath));
                        Image img = new Image();
                        img.Stretch = Stretch.Fill;
                        img.Source = bitimg1;

                        int fontSizePlus = 5;
                        int widthIcon = 32;
                        TextBlock tbDate1 = new TextBlock();
                        tbDate1.HorizontalAlignment = HorizontalAlignment.Left;
                        tbDate1.VerticalAlignment = VerticalAlignment.Top;
                        tbDate1.FontSize = tbDate1.FontSize + fontSizePlus;
                        tbDate1.Text = item.value.Name;
                        tbDate1.Height = 40;
                        tbDate1.Margin = new Thickness { Left = 15, Top = hei + 10 };
                        gridButton.Children.Add(tbDate1);

                        Button button = new Button();
                        button.Name = StringName.buttonClassPowerAndCity + item.index;
                        button.HorizontalAlignment = HorizontalAlignment.Left;
                        button.VerticalAlignment = VerticalAlignment.Top;
                        button.Content = img;
                        button.Height = hei;
                        button.Width = widthIcon;
                        button.Margin = new Thickness
                        {
                            Left = (gridButton.Width / 2) - (bitimg1.Width / 2),
                            Top = 0
                        };

                        // その都市固有の情報を見る為に、勢力の持つスポットと、シナリオで登場するスポットを比較
                        bool ch = false;
                        for (int i = 0; i < ClassGameStatus.ListPower.Count; i++)
                        {
                            foreach (var item3 in ClassGameStatus.ListPower[i].ListMember)
                            {
                                if (item3 == item.value.NameTag)
                                {
                                    // その都市固有の情報を見る為にも、勢力情報と都市情報を入れる
                                    var classPowerAndCity = new ClassPowerAndCity(ClassGameStatus.ListPower[i], item.value);
                                    button.Tag = classPowerAndCity;
                                    //ついでに、スポットの属する勢力名を設定
                                    var ge = this.ClassGameStatus.AllListSpot.Where(x => x.NameTag == item.value.NameTag).FirstOrDefault();
                                    if (ge != null)
                                    {
                                        ge.PowerNameTag = ClassGameStatus.ListPower[i].NameTag;
                                    }
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
                        if (button.Tag is not ClassPowerAndCity)
                        {
                            button.Tag = new ClassPowerAndCity(new ClassPower(), item.value);
                        }

                        button.Background = Brushes.Transparent;
                        button.Foreground = Brushes.Transparent;
                        button.Background = Brushes.Transparent;
                        button.BorderBrush = Brushes.Transparent;
                        //button.MouseEnter += WindowMainMenuLeftTop_MouseEnter;
                        button.Click += ButtonSelectionCity_click;
                        button.PreviewMouseRightButtonUp += ButtonSelectionCity_RightKeyDown;
                        gridButton.Children.Add(button);

                        grid.Children.Add(gridButton);
                    }

                }

                canvas.Children.Add(grid);
                this.canvasMain.Children.Add(canvas);
            }

            //メッセージ
            MessageBox.Show("戦闘が終了しました。");
        }

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
                                this.ListClassScenarioInfo.Add(GetClassScenarioNewFormat(getData.Value));
                            }
                            else
                            {
                                this.ListClassScenarioInfo.Add(GetClassScenario(getData.Value));
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
                                ClassGameStatus.AllListSpot.Add(GetClassSpotNewFormat(getData.Value));
                            }
                            else
                            {
                                ClassGameStatus.AllListSpot.Add(GetClassSpot(getData.Value));
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
                                ClassGameStatus.ListPower.Add(GetClassPowerNewFormat(getData.Value));
                            }
                            else
                            {
                                ClassGameStatus.ListPower.Add(GetClassPower(getData.Value));
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
                                ClassGameStatus.ListUnit.Add(GetClassUnitNewFormat(getData.Value));
                            }
                            else
                            {
                                ClassGameStatus.ListUnit.Add(GetClassUnit(getData.Value));
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
                                ClassGameStatus.ListSkill.Add(GetClassSkillNewFormat(getData.Value));
                            }
                            else
                            {
                                //ClassGameStatus.ListUnit.Add(GetClassUnit(getData.Value));
                            }
                        }
                    }
                }
                // Skill 終わり

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
                            GetClassEvent(getData.Value);
                        }
                    }

                    // Event 終わり
                }
                // Event 終わり

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
        }

        private ClassSkill GetClassSkillNewFormat(string value)
        {
            ClassSkill classSkill = new ClassSkill();

            // コメント行を取り除く
            {
                string[] line = value.Split(Environment.NewLine).ToArray();
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i].Contains("#") == true)
                    {
                        var data = line[i].Split("#");
                        line[i] = String.Concat(data[0], Environment.NewLine);
                    }
                }
                value = String.Join(Environment.NewLine, line);
            }

            //nameTag
            {
                var nameTag = new Regex(GetPatTag("NewFormatSkill"), RegexOptions.IgnoreCase)
                                .Matches(value);
                var first = CheckMatchElement(nameTag);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.NameTag = first.Value.Replace(Environment.NewLine, "");
            }
            //fkey
            {
                var fkey =
                    new Regex(GetPat("fkey"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(fkey);
                if (first == null)
                {
                    classSkill.FKey = (string.Empty, -1);
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "").Split("*").ToList();
                    classSkill.FKey = (re[0], int.Parse(re[1]));
                }
            }
            //sortkey
            {
                var sortkey =
                    new Regex(GetPat("sortkey"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(sortkey);
                if (first == null)
                {
                    classSkill.SortKey = 0;
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "");
                    classSkill.SortKey = int.Parse(re);
                }
            }
            //func
            {
                var func =
                    new Regex(GetPat("func"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(func);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.Func = (SkillFunc)Enum.Parse(typeof(SkillFunc), first.Value.Replace(Environment.NewLine, ""));
            }
            //icon
            {
                var icon =
                    new Regex(GetPat("icon"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(icon);
                if (first == null)
                {
                    classSkill.Icon = new List<string>();
                }
                else
                {
                    classSkill.Icon = first.Value
                                        .Replace(Environment.NewLine, "")
                                        .Replace(" ", "")
                                        .Split(",").ToList();
                }
            }
            //name
            {
                var name =
                    new Regex(GetPat("name"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(name);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.Name = first.Value.Replace(Environment.NewLine, "");
            }
            //help
            {
                var help =
                    new Regex(GetPat("help"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(help);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.Name = first.Value.Replace(Environment.NewLine, "");
            }
            //center
            {
                var center =
                    new Regex(GetPat("center"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(center);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.Center = first.Value.Replace(Environment.NewLine, "");
            }
            //mp
            {
                var mp =
                    new Regex(GetPat("mp"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(mp);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.Mp = int.Parse(first.Value.Replace(Environment.NewLine, ""));
            }
            //slow_per
            {
                var slow_per =
                    new Regex(GetPat("slow_per"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(slow_per);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.SlowPer = int.Parse(first.Value.Replace(Environment.NewLine, ""));
            }
            //slow_time
            {
                var slow_time =
                    new Regex(GetPat("slow_time"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(slow_time);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.SlowTime = int.Parse(first.Value.Replace(Environment.NewLine, ""));
            }
            //sound
            {
                var sound =
                    new Regex(GetPat("sound"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(sound);
                if (first == null)
                {
                    classSkill.Sound = new List<string>();
                }
                else
                {
                    classSkill.Sound = first.Value
                                        .Replace(Environment.NewLine, "")
                                        .Replace(" ", "")
                                        .Split(",").ToList();
                }
            }
            //image
            {
                var image =
                    new Regex(GetPat("image"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(image);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.Image = first.Value.Replace(Environment.NewLine, "");
            }
            //direct
            {
                var direct =
                    new Regex(GetPat("direct"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(direct);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.Direct = first.Value.Replace(Environment.NewLine, "");
            }
            //w
            {
                var w =
                    new Regex(GetPat("w"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(w);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.W = int.Parse(first.Value.Replace(Environment.NewLine, ""));
            }
            //h
            {
                var h =
                    new Regex(GetPat("h"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(h);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.H = int.Parse(first.Value.Replace(Environment.NewLine, ""));
            }
            //a
            {
                var a =
                    new Regex(GetPat("a"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(a);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.A = int.Parse(first.Value.Replace(Environment.NewLine, ""));
            }
            //force_fire
            {
                var force_fire =
                    new Regex(GetPat("force_fire"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(force_fire);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.ForceFire = first.Value.Replace(Environment.NewLine, "");
            }
            //attr
            {
                var attr =
                    new Regex(GetPat("attr"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(attr);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.Attr = first.Value.Replace(Environment.NewLine, "");
            }
            //str
            {
                var str =
                    new Regex(GetPat("str"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(str);
                if (first == null)
                {
                    classSkill.Str = (string.Empty, -1);
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "").Split("*").ToList();
                    classSkill.Str = (re[0], int.Parse(re[1]));
                }
            }
            //range
            {
                var range =
                    new Regex(GetPat("range"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(range);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.Range = int.Parse(first.Value.Replace(Environment.NewLine, ""));
            }
            //damage_range_adjust
            {
                var damage_range_adjust =
                    new Regex(GetPat("damage_range_adjust"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(damage_range_adjust);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.DamageRangeAdjust = int.Parse(first.Value.Replace(Environment.NewLine, ""));
            }
            //range_min
            {
                var range_min =
                    new Regex(GetPat("range_min"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(range_min);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.RangeMin = int.Parse(first.Value.Replace(Environment.NewLine, ""));
            }
            //speed
            {
                var speed =
                    new Regex(GetPat("speed"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(speed);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.Speed = int.Parse(first.Value.Replace(Environment.NewLine, ""));
            }
            //gun_delay
            {
                var gun_delay =
                    new Regex(GetPat("gun_delay"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(gun_delay);
                if (first == null)
                {
                    classSkill.GunDelay = (string.Empty, -1);
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "").Split("*").ToList();
                    classSkill.GunDelay = (re[0], int.Parse(re[1]));
                }
            }
            //pair_next
            {
                var pair_next =
                    new Regex(GetPat("pair_next"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(pair_next);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.PairNext = first.Value.Replace(Environment.NewLine, "");
            }
            //next
            {
                var next =
                    new Regex(GetPat("next"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(next);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.Next = first.Value.Replace(Environment.NewLine, "");
            }
            //random_space
            {
                var random_space =
                    new Regex(GetPat("random_space"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(random_space);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.RandomSpace = int.Parse(first.Value.Replace(Environment.NewLine, ""));
            }
            //offset
            {
                var offset =
                    new Regex(GetPat("offset"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(offset);
                if (first == null)
                {
                    throw new Exception();
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "").Split(",").ToList();
                    classSkill.Offset = re;
                }
            }
            //ray
            {
                var ray =
                    new Regex(GetPat("ray"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(ray);
                if (first == null)
                {
                    classSkill.Ray = new List<int>();
                }
                else
                {
                    var re = first.Value.Replace(Environment.NewLine, "").Split(',').Select(Int32.Parse)?.ToList();
                    classSkill.Ray = re != null ? re : new List<int>();
                }
            }
            //force_ray
            {
                var force_ray =
                    new Regex(GetPat("force_ray"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(force_ray);
                if (first == null)
                {
                    throw new Exception();
                }
                classSkill.ForceRay = first.Value.Replace(Environment.NewLine, "");
            }


            return classSkill;
        }

        private void GetClassEvent(string value)
        {
            // コメント行を取り除く
            {
                string[] line = value.Split(Environment.NewLine).ToArray();
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i].Contains("//") == true)
                    {
                        var data = line[i].Split("//");
                        line[i] = String.Concat(data[0], Environment.NewLine);
                    }
                }
                value = String.Join(Environment.NewLine, line);
            }

            var nameTag =
                new Regex(GetPatTagEvent("Event"), RegexOptions.IgnoreCase)
                .Matches(value);
            var first = CheckMatchElement(nameTag);
            if (first == null)
            {
                throw new Exception();
            }
            ClassEvent classEvent = new ClassEvent();
            classEvent.Name = first.Value.Replace(Environment.NewLine, "");

            var siki =
                new Regex(GetPatEvent("Event", classEvent.Name), RegexOptions.IgnoreCase)
                .Matches(value);
            var result = CheckMatchElement(siki);
            if (result == null)
            {
                throw new Exception();
            }

            value = result.Value.Replace(Environment.NewLine, "");

            var lexer = new Lexer(value);
            var parser = new Parser(lexer);
            var root = parser.ParseProgram();
            classEvent.Root = root;
            this.ClassGameStatus.ListEvent.Add(classEvent);


        }

        private ClassSpot GetClassSpotNewFormat(string value)
        {
            ClassSpot classSpot = new ClassSpot();

            // コメント行を取り除く
            {
                string[] line = value.Split(Environment.NewLine).ToArray();
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i].Contains("#") == true)
                    {
                        var data = line[i].Split("#");
                        line[i] = String.Concat(data[0], Environment.NewLine);
                    }
                }
                value = String.Join(Environment.NewLine, line);
            }

            //NewFormatSpot
            {
                var nameTag =
                    new Regex(GetPatTag("NewFormatSpot"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(nameTag);
                if (first == null)
                {
                    throw new Exception();
                }
                classSpot.NameTag = first.Value.Replace(Environment.NewLine, "");
            }
            //name
            {
                var name =
                    new Regex(@"(?<=name[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(name);
                if (first == null)
                {
                    throw new Exception();
                }
                classSpot.Name = first.Value.Replace(Environment.NewLine, "");
            }
            //image
            {
                var image =
                    new Regex(@"(?<=image[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(image);
                if (first == null)
                {
                    throw new Exception();
                }

                List<string> strings = new List<string>();
                strings.Add(ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName);
                strings.Add("025_CityImage");
                strings.Add(first.Value.Replace(Environment.NewLine, ""));
                string path = System.IO.Path.Combine(strings.ToArray());
                classSpot.ImagePath = path;
            }
            //x
            {
                var x =
                    new Regex(@"(?<=x[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(x);
                if (first == null)
                {
                    throw new Exception();
                }
                classSpot.X = Convert.ToInt32(first.Value.Replace(Environment.NewLine, ""));
            }
            //y
            {
                var y =
                    new Regex(@"(?<=y[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(y);
                if (first == null)
                {
                    throw new Exception();
                }
                classSpot.Y = Convert.ToInt32(first.Value.Replace(Environment.NewLine, ""));
            }
            //member
            {
                var member =
                    new Regex(GetPatComma("member"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(member);
                if (first == null)
                {
                    classSpot.ListMember = new List<string>();
                }
                else
                {
                    classSpot.ListMember = first.Value.Replace(Environment.NewLine, "").Split(",").ToList();
                }
            }
            //map
            {
                var map =
                    new Regex(@"(?<=map[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(map);
                if (first == null)
                {
                    classSpot.Map = String.Empty;
                }
                else
                {
                    classSpot.Map = first.Value.Replace(Environment.NewLine, "");
                }
            }
            //wanderingMonster
            {
                var wanderingMonster =
                    new Regex(GetPat("wanderingMonster"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(wanderingMonster);
                if (first == null)
                {
                    classSpot.ListWanderingMonster = new List<(string, int)>();
                }
                else
                {
                    List<(string, int)> values = new List<(string, int)>();
                    foreach (var item in first.Value.Replace(Environment.NewLine, "").Split(",").ToList())
                    {
                        var cutResult = item.Split("*").ToList();
                        values.Add(new(cutResult[0], int.Parse(cutResult[1])));
                    }
                    classSpot.ListWanderingMonster = values;
                }
            }
            //monster
            {
                var monster =
                    new Regex(GetPat("monster"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(monster);
                if (first == null)
                {
                    classSpot.ListMonster = new List<(string, int)>();
                }
                else
                {
                    List<(string, int)> values = new List<(string, int)>();
                    foreach (var item in first.Value.Replace(Environment.NewLine, "").Split(",").ToList())
                    {
                        var cutResult = item.Split("*").ToList();
                        values.Add(new(cutResult[0], int.Parse(cutResult[1])));
                    }
                    classSpot.ListMonster = values;
                }
            }

            return classSpot;
        }

        private ClassSpot GetClassSpot(string value)
        {
            ClassSpot classSpot = new ClassSpot();

            {
                var nameTag =
                    new Regex(@"(?<=spot[\s]*)([\S\n]+?)(?=[\s]|{)", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(nameTag);
                if (first == null)
                {
                    throw new Exception();
                }
                classSpot.NameTag = first.Value.Replace(Environment.NewLine, "");
            }
            {
                var name =
                    new Regex(@"(?<=name\s*=\s*)([\S\n]+?.*(?=[\s\n]))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(name);
                if (first == null)
                {
                    throw new Exception();
                }
                classSpot.Name = first.Value.Replace(System.Environment.NewLine, "").Replace("\r", "");
            }
            {
                var image =
                    new Regex(@"(?<=image\s*=\s*)([\S\n]+?.*(?=[\s\n]))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(image);
                if (first == null)
                {
                    throw new Exception();
                }
                var a = first.Value.Replace(System.Environment.NewLine, "").Replace("\r", "");

                List<string> strings = new List<string>();
                strings.Add(ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName);
                strings.Add("025_CityImage");
                //暫定でpng
                strings.Add(a + ".png");
                string path = System.IO.Path.Combine(strings.ToArray());

                classSpot.ImagePath = path;
            }
            {
                var x =
                    new Regex(@"(?<=x\s*=\s*)([\S\n]+?.*(?=[\s\n]))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(x);
                if (first == null)
                {
                    throw new Exception();
                }
                classSpot.X = Convert.ToInt32(first.Value.Replace(System.Environment.NewLine, "").Replace("\r", ""));
            }
            {
                var y =
                    new Regex(@"(?<=y\s*=\s*)([\S\n]+?.*(?=[\s\n]))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(y);
                if (first == null)
                {
                    throw new Exception();
                }
                classSpot.Y = Convert.ToInt32(first.Value.Replace(System.Environment.NewLine, "").Replace("\r", ""));
            }

            return classSpot;
        }

        private ClassScenarioInfo GetClassScenario(string value)
        {
            ClassScenarioInfo classScenario = new ClassScenarioInfo();
            return classScenario;
        }

        private ClassScenarioInfo GetClassScenarioNewFormat(string value)
        {
            ClassScenarioInfo classScenario = new ClassScenarioInfo();

            // コメント行を取り除く
            {
                string[] line = value.Split(Environment.NewLine).ToArray();
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i].Contains("#") == true)
                    {
                        var data = line[i].Split("#");
                        line[i] = String.Concat(data[0], Environment.NewLine);
                    }
                }
                value = String.Join(Environment.NewLine, line);
            }

            //scenarioName
            {
                //先読み、戻り読みの言明、Assertion
                //肯定先読み、肯定戻り読み
                var scenarioName =
                    new Regex(@"(?<=scenario_name[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(scenarioName);
                if (first == null)
                {
                    throw new Exception();
                }
                classScenario.ScenarioName = first.Value.Replace(Environment.NewLine, "");
            }
            //sortkey
            {
                var sortkey =
                    new Regex(@"(?<=sortkey[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(sortkey);
                if (first == null)
                {
                    throw new Exception();
                }
                classScenario.Sortkey = Convert.ToInt32(first.Value);
            }
            //text
            {
                var text =
                    new Regex(@"(?<=text[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(text);
                if (first == null)
                {
                    classScenario.ScenarioIntroduce = string.Empty;
                }
                else
                {
                    classScenario.ScenarioIntroduce = first.Value;
                }
            }
            //scenario_image_bool
            {
                var scenario_image_bool =
                    new Regex(@"(?<=scenario_image_bool[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(scenario_image_bool);
                if (first == null)
                {
                    classScenario.ScenarioImageBool = false;
                }
                else
                {
                    classScenario.ScenarioImageBool = Convert.ToBoolean(first.Value);
                }
            }
            //scenario_image
            {
                var scenario_image =
                    new Regex(@"(?<=scenario_image[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(scenario_image);
                if (first == null)
                {
                    classScenario.ScenarioImage = String.Empty;
                }
                else
                {
                    classScenario.ScenarioImage = Convert.ToString(first.Value);
                }
            }
            //map_image_name_file
            {
                var map_image_name_file =
                    new Regex(@"(?<=map_image_name_file[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(map_image_name_file);
                if (first == null)
                {
                    classScenario.NameMapImageFile = String.Empty;
                }
                else
                {
                    classScenario.NameMapImageFile = Convert.ToString(first.Value);
                }
            }
            //buttonType
            {
                var buttonType =
                    new Regex(@"(?<=ButtonType[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(buttonType);
                if (first == null)
                {
                    classScenario.ButtonType = ButtonType.Scenario;
                }
                else
                {
                    ButtonType buttonType1;
                    Enum.TryParse(first.Value, out buttonType1);
                    classScenario.ButtonType = buttonType1;
                }
            }
            //mail
            {
                var mail =
                    new Regex(@"(?<=mail[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(mail);
                if (first == null)
                {
                    classScenario.Mail = String.Empty;
                }
                else
                {
                    classScenario.Mail = Convert.ToString(first.Value);
                }
            }
            //internet
            {
                var internet =
                    new Regex(@"(?<=internet[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(internet);
                if (first == null)
                {
                    classScenario.Internet = String.Empty;
                }
                else
                {
                    classScenario.Internet = Convert.ToString(first.Value);
                }
            }
            //spot
            {
                var spot =
                    new Regex(@"(?<=spot[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(spot);
                if (first == null)
                {
                    classScenario.DisplayListSpot = new List<string>();
                }
                else
                {
                    classScenario.DisplayListSpot = first.Value.Replace(Environment.NewLine, "").Split(",").ToList();
                }
            }
            //spot_capacity
            {
                var spot_capacity =
                    new Regex(GetPat("spot_capacity"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(spot_capacity);
                if (first == null)
                {
                    classScenario.SpotCapacity = 8;
                }
                else
                {
                    classScenario.SpotCapacity = Convert.ToInt32(first.Value);
                }
            }
            //member_capacity
            {
                var member_capacity =
                    new Regex(GetPat("member_capacity"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(member_capacity);
                if (first == null)
                {
                    classScenario.MemberCapacity = 8;
                }
                else
                {
                    classScenario.MemberCapacity = Convert.ToInt32(first.Value);
                }
            }
            //linkSpot
            {
                var linkSpot =
                    new Regex(GetPatMethod("linkSpot"), RegexOptions.IgnoreCase)
                    .Matches(value);
                if (linkSpot == null)
                {
                    classScenario.DisplayListSpot = new List<string>();
                }
                else
                {
                    classScenario.ListLinkSpot = new List<(string, string)>();
                    foreach (var item in linkSpot)
                    {
                        var conv = Convert.ToString(item);
                        if (conv == null)
                        {
                            continue;
                        }
                        var sp = conv.Split(',');
                        classScenario.ListLinkSpot.Add((sp[0].Trim(), sp[1].Trim()));
                    }
                }
            }
            //world
            {
                var world =
                    new Regex(GetPat("world"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(world);
                if (first == null)
                {
                    classScenario.World = String.Empty;
                }
                else
                {
                    classScenario.World = Convert.ToString(first.Value);
                }
            }

            return classScenario;
        }

        private ClassPower GetClassPowerNewFormat(string value)
        {
            ClassPower classPower = new ClassPower();

            // コメント行を取り除く
            {
                string[] line = value.Split(Environment.NewLine).ToArray();
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i].Contains("#") == true)
                    {
                        var data = line[i].Split("#");
                        line[i] = String.Concat(data[0], Environment.NewLine);
                    }
                }
                value = String.Join(Environment.NewLine, line);
            }

            {
                var name =
                    new Regex(@"(?<=name[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(name);
                if (first == null)
                {
                    throw new Exception();
                }
                classPower.Name = first.Value.Replace(Environment.NewLine, "");
            }
            {
                var nameTag =
                    new Regex(GetPatTag("NewFormatPower"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(nameTag);
                if (first == null)
                {
                    throw new Exception();
                }
                classPower.NameTag = first.Value.Replace(Environment.NewLine, "");
            }
            {
                var money =
                    new Regex(@"(?<=money[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(money);
                if (first == null)
                {
                    classPower.Money = 0;
                }
                else
                {
                    classPower.Money = Convert.ToInt32(first.Value);
                }
            }
            {
                var flag =
                    new Regex(@"(?<=flag[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(flag);
                if (first == null)
                {
                    throw new Exception();
                }

                List<string> strings = new List<string>();
                strings.Add(ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName);
                strings.Add("030_FlagImage");
                strings.Add(first.Value.Replace(Environment.NewLine, ""));
                string path = System.IO.Path.Combine(strings.ToArray());

                if (File.Exists(path + ".png") == true)
                {
                    classPower.FlagPath = path + ".png";
                }
                if (File.Exists(path + ".jpg") == true)
                {
                    classPower.FlagPath = path + ".jpg";
                }
            }
            {
                var master =
                    new Regex(@"(?<=master[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(master);
                if (first == null)
                {
                    throw new Exception();
                }
                classPower.MasterTag = first.Value.Replace(Environment.NewLine, "");
            }
            {
                var home =
                    new Regex(@"(?<=home[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(home);
                if (first == null)
                {
                    classPower.ListHome = new List<string>();
                }
                else
                {
                    classPower.ListHome = first.Value.Replace(Environment.NewLine, "").Split(",").ToList();
                }
            }
            {
                var head =
                    new Regex(@"(?<=head[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(head);
                if (first == null)
                {
                    classPower.Head = String.Empty;
                }
                else
                {
                    classPower.Head = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var diff =
                    new Regex(@"(?<=diff[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))", RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(diff);
                if (first == null)
                {
                    classPower.Diff = String.Empty;
                }
                else
                {
                    classPower.Diff = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var text =
                    new Regex(GetPat("text"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(text);
                if (first == null)
                {
                    classPower.Text = string.Empty;
                }
                else
                {
                    classPower.Text = first.Value;
                }
            }
            {
                var member =
                    new Regex(GetPatComma("member"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(member);
                if (first == null)
                {
                    throw new Exception();
                }
                classPower.ListMember = first.Value.Replace(Environment.NewLine, "").Split(",").ToList();
            }
            {
                var commonConscription =
                    new Regex(GetPatComma("commonConscription"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(commonConscription);
                if (first == null)
                {
                    throw new Exception();
                }
                classPower.ListCommonConscription = first.Value.Replace(Environment.NewLine, "").Split(",").ToList();
            }
            {
                var image =
                    new Regex(GetPat("image"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(image);
                if (first == null)
                {
                    classPower.Image = string.Empty;
                }
                else
                {
                    List<string> strings = new List<string>();
                    strings.Add(ClassConfigGameTitle.DirectoryGameTitle[this.NowNumberGameTitle].FullName);
                    strings.Add("035_PowerImage");
                    strings.Add(first.Value.Replace(Environment.NewLine, ""));
                    string path = System.IO.Path.Combine(strings.ToArray());

                    classPower.Image = path;
                }
            }

            return classPower;
        }

        private ClassPower GetClassPower(string value)
        {
            ClassPower classPower = new ClassPower();
            return classPower;
        }

        private ClassUnit GetClassUnitNewFormat(string value)
        {
            ClassUnit classUnit = new ClassUnit();

            // コメント行を取り除く
            {
                string[] line = value.Split(Environment.NewLine).ToArray();
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i].Contains("#") == true)
                    {
                        var data = line[i].Split("#");
                        line[i] = String.Concat(data[0], Environment.NewLine);
                    }
                }
                value = String.Join(Environment.NewLine, line);
            }

            {
                var nameTag =
                    new Regex(GetPatTag("NewFormatUnit"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(nameTag);
                if (first == null)
                {
                    throw new Exception();
                }
                classUnit.NameTag = first.Value.Replace(Environment.NewLine, "");
            }
            {
                var formation =
                    new Regex(GetPat("formation"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(formation);
                if (first == null)
                {
                    classUnit.Formation = new ClassFormation()
                    {
                        Id = 0,
                        Formation = Formation.F
                    };
                }
                else
                {
                    int conv = Convert.ToInt32(first.Value);
                    switch (conv)
                    {
                        case 0:
                            classUnit.Formation = new ClassFormation()
                            {
                                Id = 0,
                                Formation = Formation.F
                            };
                            break;
                        case 1:
                            classUnit.Formation = new ClassFormation()
                            {
                                Id = 1,
                                Formation = Formation.M
                            };
                            break;
                        case 2:
                            classUnit.Formation = new ClassFormation()
                            {
                                Id = 2,
                                Formation = Formation.B
                            };
                            break;
                        default:
                            classUnit.Formation = new ClassFormation()
                            {
                                Id = 0,
                                Formation = Formation.F
                            };
                            break;
                    }
                }
            }
            {
                var name =
                    new Regex(GetPat("name"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(name);
                if (first == null)
                {
                    throw new Exception();
                }
                classUnit.Name = first.Value.Replace(Environment.NewLine, "");
            }
            {
                var help =
                    new Regex(GetPat("help"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(help);
                if (first == null)
                {
                    classUnit.Help = string.Empty;
                }
                else
                {
                    classUnit.Help = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var race =
                    new Regex(GetPat("race"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(race);
                if (first == null)
                {
                    throw new Exception();
                }
                classUnit.Race = first.Value.Replace(Environment.NewLine, "");
            }
            //{
            //    var @class =
            //        new Regex(GetPat("class"), RegexOptions.IgnoreCase)
            //        .Matches(value);
            //    var first = CheckMatchElement(@class);
            //    if (first == null)
            //    {
            //        throw new Exception();
            //    }
            //    classUnit.Class = first.Value.Replace(Environment.NewLine, "");
            //}
            {
                var image =
                    new Regex(GetPat("image"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(image);
                if (first == null)
                {
                    throw new Exception();
                }
                classUnit.Image = first.Value.Replace(Environment.NewLine, "");
            }
            {
                var dead =
                    new Regex(GetPat("dead"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(dead);
                if (first == null)
                {
                    classUnit.Dead = String.Empty;
                }
                else
                {
                    classUnit.Dead = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var retreat =
                    new Regex(GetPat("retreat"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(retreat);
                if (first == null)
                {
                    classUnit.Retreat = String.Empty;
                }
                else
                {
                    classUnit.Retreat = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var join =
                    new Regex(GetPat("join"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(join);
                if (first == null)
                {
                    classUnit.Join = String.Empty;
                }
                else
                {
                    classUnit.Join = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var face =
                    new Regex(GetPat("face"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(face);
                if (first == null)
                {
                    classUnit.Face = string.Empty;
                }
                else
                {
                    classUnit.Face = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var voice_type =
                    new Regex(GetPat("voice_type"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(voice_type);
                if (first == null)
                {
                    classUnit.Voice_type = string.Empty;
                }
                else
                {
                    classUnit.Voice_type = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var friend =
                    new Regex(GetPat("friend"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(friend);
                if (first == null)
                {
                    classUnit.Friend = string.Empty;
                }
                else
                {
                    classUnit.Friend = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var merce =
                    new Regex(GetPat("merce"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(merce);
                if (first == null)
                {
                    classUnit.Merce = string.Empty;
                }
                else
                {
                    classUnit.Merce = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var staff =
                    new Regex(GetPat("staff"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(staff);
                if (first == null)
                {
                    classUnit.Staff = string.Empty;
                }
                else
                {
                    classUnit.Staff = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var initMember =
                    new Regex(GetPat("initMember"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(initMember);
                if (first == null)
                {
                    classUnit.InitMember = string.Empty;
                }
                else
                {
                    classUnit.InitMember = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var enemy =
                    new Regex(GetPat("enemy"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(enemy);
                if (first == null)
                {
                    classUnit.Enemy = string.Empty;
                }
                else
                {
                    classUnit.Enemy = first.Value.Replace(Environment.NewLine, "");
                }
            }
            {
                var level =
                    new Regex(GetPat("level"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(level);
                if (first == null)
                {
                    classUnit.Level = 0;
                }
                else
                {
                    classUnit.Level = Convert.ToInt32(first.Value);
                }
            }
            {
                var level_max =
                    new Regex(GetPat("level_max"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(level_max);
                if (first == null)
                {
                    classUnit.Level_max = 99999;
                }
                else
                {
                    classUnit.Level_max = Convert.ToInt32(first.Value);
                }
            }
            {
                var price =
                    new Regex(GetPat("price"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(price);
                if (first == null)
                {
                    classUnit.Price = 0;
                }
                else
                {
                    classUnit.Price = Convert.ToInt32(first.Value);
                }
            }
            {
                var cost =
                    new Regex(GetPat("cost"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(cost);
                if (first == null)
                {
                    classUnit.Cost = 0;
                }
                else
                {
                    classUnit.Cost = Convert.ToInt32(first.Value);
                }
            }
            {
                var medical =
                    new Regex(GetPat("medical"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(medical);
                if (first == null)
                {
                    classUnit.Medical = 0;
                }
                else
                {
                    classUnit.Medical = Convert.ToInt32(first.Value);
                }
            }
            {
                var hasExp =
                    new Regex(GetPat("hasExp"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(hasExp);
                if (first == null)
                {
                    classUnit.HasExp = 0;
                }
                else
                {
                    classUnit.HasExp = Convert.ToInt32(first.Value);
                }
            }
            {
                var hp =
                    new Regex(GetPat("hp"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(hp);
                if (first == null)
                {
                    classUnit.Hp = 0;
                }
                else
                {
                    classUnit.Hp = Convert.ToInt32(first.Value);
                }
            }
            {
                var mp =
                    new Regex(GetPat("mp"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(mp);
                if (first == null)
                {
                    classUnit.Mp = 0;
                }
                else
                {
                    classUnit.Mp = Convert.ToInt32(first.Value);
                }
            }
            {
                var attack =
                    new Regex(GetPat("attack"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(attack);
                if (first == null)
                {
                    classUnit.Attack = 0;
                }
                else
                {
                    classUnit.Attack = Convert.ToInt32(first.Value);
                }
            }
            {
                var defense =
                    new Regex(GetPat("defense"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(defense);
                if (first == null)
                {
                    classUnit.Defense = 0;
                }
                else
                {
                    classUnit.Defense = Convert.ToInt32(first.Value);
                }
            }
            {
                var magic =
                    new Regex(GetPat("magic"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(magic);
                if (first == null)
                {
                    classUnit.Magic = 0;
                }
                else
                {
                    classUnit.Magic = Convert.ToInt32(first.Value);
                }
            }
            {
                var magDef =
                    new Regex(GetPat("magDef"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(magDef);
                if (first == null)
                {
                    classUnit.MagDef = 0;
                }
                else
                {
                    classUnit.MagDef = Convert.ToInt32(first.Value);
                }
            }
            {
                var speed =
                    new Regex(GetPat("speed"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(speed);
                if (first == null)
                {
                    classUnit.Speed = 0;
                }
                else
                {
                    classUnit.Speed = Convert.ToDouble(first.Value);
                }
            }
            {
                var dext =
                    new Regex(GetPat("dext"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(dext);
                if (first == null)
                {
                    classUnit.Dext = 0;
                }
                else
                {
                    classUnit.Dext = Convert.ToInt32(first.Value);
                }
            }
            {
                var move =
                    new Regex(GetPat("move"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(move);
                if (first == null)
                {
                    classUnit.Move = 0;
                }
                else
                {
                    classUnit.Move = Convert.ToInt32(first.Value);
                }
            }
            {
                var hprec =
                    new Regex(GetPat("hprec"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(hprec);
                if (first == null)
                {
                    classUnit.Hprec = 0;
                }
                else
                {
                    classUnit.Hprec = Convert.ToInt32(first.Value);
                }
            }
            {
                var mprec =
                    new Regex(GetPat("mprec"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(mprec);
                if (first == null)
                {
                    classUnit.Mprec = 0;
                }
                else
                {
                    classUnit.Mprec = Convert.ToInt32(first.Value);
                }
            }
            {
                var exp =
                    new Regex(GetPat("exp"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(exp);
                if (first == null)
                {
                    classUnit.Exp = 0;
                }
                else
                {
                    classUnit.Exp = Convert.ToInt32(first.Value);
                }
            }
            {
                var exp_mul =
                    new Regex(GetPat("exp_mul"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(exp_mul);
                if (first == null)
                {
                    classUnit.Exp_mul = 0;
                }
                else
                {
                    classUnit.Exp_mul = Convert.ToInt32(first.Value);
                }
            }
            {
                var heal_max =
                    new Regex(GetPat("heal_max"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(heal_max);
                if (first == null)
                {
                    classUnit.Heal_max = 0;
                }
                else
                {
                    classUnit.Heal_max = Convert.ToInt32(first.Value);
                }
            }
            {
                var summon_max =
                    new Regex(GetPat("summon_max"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(summon_max);
                if (first == null)
                {
                    classUnit.Summon_max = 0;
                }
                else
                {
                    classUnit.Summon_max = Convert.ToInt32(first.Value);
                }
            }
            {
                var no_knock =
                    new Regex(GetPat("no_knock"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(no_knock);
                if (first == null)
                {
                    classUnit.No_knock = 0;
                }
                else
                {
                    classUnit.No_knock = Convert.ToInt32(first.Value);
                }
            }
            {
                var loyal =
                    new Regex(GetPat("loyal"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(loyal);
                if (first == null)
                {
                    classUnit.Loyal = 0;
                }
                else
                {
                    classUnit.Loyal = Convert.ToInt32(first.Value);
                }
            }
            {
                var alive_per =
                    new Regex(GetPat("alive_per"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(alive_per);
                if (first == null)
                {
                    classUnit.Alive_per = 0;
                }
                else
                {
                    classUnit.Alive_per = Convert.ToInt32(first.Value);
                }
            }
            {
                var escape_range =
                    new Regex(GetPat("escape_range"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(escape_range);
                if (first == null)
                {
                    classUnit.Escape_range = 0;
                }
                else
                {
                    classUnit.Escape_range = Convert.ToInt32(first.Value);
                }
            }
            {
                var skill =
                    new Regex(GetPat("skill"), RegexOptions.IgnoreCase)
                    .Matches(value);
                var first = CheckMatchElement(skill);
                if (first == null)
                {
                    classUnit.SkillName = new List<string>();
                }
                else
                {
                    classUnit.SkillName = first.Value
                                        .Replace(Environment.NewLine, "")
                                        .Replace(" ", "")
                                        .Split(",").ToList();
                }
            }

            //ここですべきでない
            //classUnit.ID = this.ClassGameStatus.IDCount;
            //this.ClassGameStatus.SetIDCount();
            return classUnit;
        }

        private ClassUnit GetClassUnit(string value)
        {
            ClassUnit classUnit = new ClassUnit();
            return classUnit;
        }


        private Match? CheckMatchElement(MatchCollection scenarioName)
        {
            if (scenarioName == null)
            {
                return null;
            }
            if (scenarioName.Count > 1)
            {
                //タグが複数指定されています
                throw new NotImplementedException();
            }

            return scenarioName.FirstOrDefault();
        }

        /// <summary>
        /// 通常のパターン
        /// name = "test";
        /// など
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetPat(string name)
        {
            string a = @"(?<=[\s\n]+" + name + @"[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\"";))";
            return a;
        }

        /// <summary>
        /// これいる？上で良い気がする
        /// member = "aaa,bbb,ccc"
        /// など
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetPatComma(string name)
        {
            return @"(?<=[\s\n]+" + name + @"[\s]*=[\s]*\"")([\s\S\n]+?.*(?=\""))";
        }

        private string GetPatTag(string name)
        {
            return @"(?<=" + name + @"[\s]*)([\S\n]+?)(?=[\s]|{)";
        }
        private string GetPatMethod(string name)
        {
            return @"(?<=" + name + @"\()([\S\n\s]+?)(?=\);)";
        }
        private string GetPatTagEvent(string name)
        {
            return @"(?<=" + name + @"[\s]*)([\S\n]+?)(?=[\s]+?<-)";
        }
        private string GetPatEvent(string tag, string name)
        {
            return @"(?<=" + tag + @"[\s]*" + name + @"[\S\n\s]*<-)([\S\n\s]+?)(?=->)";
        }


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

                    Canvas.SetZIndex(this.fade, 100);
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

            var gridMapStrategy = (Grid)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.gridMapStrategy);
            if (gridMapStrategy == null)
            {
                return;
            }
            bool flag1 = true;
            double baseDataX = gridMapStrategy.Margin.Left;
            double baseDataY = gridMapStrategy.Margin.Top;

            ClassVec classVec = new ClassVec();
            classVec.X = gridMapStrategy.Margin.Left;
            classVec.Y = gridMapStrategy.Margin.Top;
            classVec.CenterPoint = new Point(gridMapStrategy.Width / 2, gridMapStrategy.Height / 2);
            classVec.Target = this.ClassGameStatus.SelectionCityPoint;
            classVec.Speed = 10;
            classVec.Set();

            //移動し過ぎを防止
            int counter = 500;

            while (flag1 == true)
            {
                Thread.Sleep(5);

                double resultX = 0;
                double resultY = 0;
                //どれだけ移動したかを算出、それを移動量と称す
                resultX = +(gridMapStrategy.Margin.Left - baseDataX);
                resultY = +(gridMapStrategy.Margin.Top - baseDataY);
                //移動量を選択勢力の町座標に足す。その結果が中央に来れば（画面中央付近に来れば）ループから抜け出す
                if ((classVec.CenterPoint.X + 100 > this.ClassGameStatus.SelectionCityPoint.X + resultX
                    && classVec.CenterPoint.X - 100 < this.ClassGameStatus.SelectionCityPoint.X + resultX)
                    &&
                    (classVec.CenterPoint.Y + 100 > this.ClassGameStatus.SelectionCityPoint.Y + resultY
                    && classVec.CenterPoint.Y - 100 < this.ClassGameStatus.SelectionCityPoint.Y + resultY))
                {
                    flag1 = false;
                    break;
                }

                await Task.Run(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        var ge = classVec.Get(new Point(gridMapStrategy.Margin.Left, gridMapStrategy.Margin.Top));
                        gridMapStrategy.Margin = new Thickness()
                        {
                            Left = ge.X,
                            Top = ge.Y
                        };
                    }));
                });

                counter--;

                if (counter <= 0)
                {
                    throw new Exception();
                }
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
                    evaluator.window = this;
                    evaluator.Eval(ev.Root, enviroment);
                    ev.Yet = false;
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
                KeepInterval(this.timerAfterFadeIn);
            };
            this.timerAfterFadeIn.Tick += (x, s) =>
            {
                TimerAction60FPSBattle();
                KeepInterval(this.timerAfterFadeIn);
            };
            this.timerAfterFadeIn.Start();

            //工事中
            ////スキルスレッド開始
            //出撃ユニット
            {
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                (Task, CancellationTokenSource) a = new(Task.Run(() => TaskBattleSkill(token)), tokenSource);
                this.ClassGameStatus.TaskBattleSkill = a;
            }
            ////移動スレッド開始
            //出撃ユニット
            {
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                (Task, CancellationTokenSource) a = new(Task.Run(() => TaskBattleMoveAsync(token)), tokenSource);
                this.ClassGameStatus.TaskBattleMoveAsync = a;
            }
            //防衛ユニット
            {
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                (Task, CancellationTokenSource) a = new(Task.Run(() => TaskBattleMoveDefAsync(token)), tokenSource);
                this.ClassGameStatus.TaskBattleMoveDefAsync = a;
            }

        }

        private void TimerAction60FPSBattle()
        {
            //攻撃側勝利
            {
                bool flgaDefHp = false;
                foreach (var itemDefUnitGroup in this.ClassGameStatus.ClassBattleUnits.DefUnitGroup)
                {
                    if (itemDefUnitGroup.ListClassUnit.Count != 0)
                    {
                        flgaDefHp = true;
                    }
                }

                if (flgaDefHp == false)
                {
                    ////defの負け

                    this.timerAfterFadeIn.Stop();

                    //タスクキル
                    if (this.ClassGameStatus.TaskBattleSkill.Item1 != null)
                    {
                        this.ClassGameStatus.TaskBattleSkill.Item2.Cancel();
                    }
                    if (this.ClassGameStatus.TaskBattleMoveAsync.Item1 != null)
                    {
                        this.ClassGameStatus.TaskBattleMoveAsync.Item2.Cancel();
                    }
                    if (this.ClassGameStatus.TaskBattleMoveDefAsync.Item1 != null)
                    {
                        this.ClassGameStatus.TaskBattleMoveDefAsync.Item2.Cancel();
                    }

                    //画面戻る

                    this.FadeOut = true;

                    this.delegateMapRenderedFromBattle = SetMapStrategyFromBattle;

                    this.FadeIn = true;

                    //部隊所属領地変更
                    {
                        //出撃先領地
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

                        //出撃元領地
                        var selectedItem = Application.Current.Properties["selectedItem"];
                        if (selectedItem == null)
                        {
                            return;
                        }

                        var selectedItemClassSpot = selectedItem as ClassSpot;
                        if (selectedItemClassSpot == null)
                        {
                            return;
                        }

                        //出撃先領地情報
                        var aa = this.ClassGameStatus.AllListSpot
                                .Where(x => x.NameTag == convSpots.ClassSpot.NameTag)
                                .First();

                        //spotの所属情報を書き換え
                        convSpots.ClassSpot.PowerNameTag = selectedItemClassSpot.PowerNameTag;
                        aa.PowerNameTag = convSpots.ClassSpot.PowerNameTag;

                        ////unitの所属情報を書き換え
                        //防衛部隊を削除、又は他都市へ移動。隣接都市が無ければ放浪する
                        aa.UnitGroup.Clear();
                        foreach (var item in this.ClassGameStatus.AllListSpot.Where(x => x.NameTag == selectedItemClassSpot.NameTag))
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
                    }

                    //片付け
                    this.ClassGameStatus.ClassBattleUnits.SortieUnitGroup.Clear();
                    this.ClassGameStatus.ClassBattleUnits.DefUnitGroup.Clear();
                    this.ClassGameStatus.ClassBattleUnits.NeutralUnitGroup.Clear();

                    return;
                }
            }
            //防衛側勝利
            {
                bool flgaAttackHp = false;
                foreach (var itemDefUnitGroup in this.ClassGameStatus.ClassBattleUnits.SortieUnitGroup)
                {
                    if (itemDefUnitGroup.ListClassUnit.Count != 0)
                    {
                        flgaAttackHp = true;
                    }
                }

                if (flgaAttackHp == false)
                {
                    ////defの負け

                    this.timerAfterFadeIn.Stop();

                    //タスクキル
                    if (this.ClassGameStatus.TaskBattleSkill.Item1 != null)
                    {
                        this.ClassGameStatus.TaskBattleSkill.Item2.Cancel();
                    }
                    if (this.ClassGameStatus.TaskBattleMoveAsync.Item1 != null)
                    {
                        this.ClassGameStatus.TaskBattleMoveAsync.Item2.Cancel();
                    }
                    if (this.ClassGameStatus.TaskBattleMoveDefAsync.Item1 != null)
                    {
                        this.ClassGameStatus.TaskBattleMoveDefAsync.Item2.Cancel();
                    }

                    //画面戻る

                    this.FadeOut = true;

                    this.delegateMapRenderedFromBattle = SetMapStrategyFromBattle;

                    this.FadeIn = true;

                    //片付け
                    this.ClassGameStatus.ClassBattleUnits.SortieUnitGroup.Clear();
                    this.ClassGameStatus.ClassBattleUnits.DefUnitGroup.Clear();
                    this.ClassGameStatus.ClassBattleUnits.NeutralUnitGroup.Clear();

                    return;
                }
            }
        }

        private void TaskBattleSkill(CancellationToken token)
        {
            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 1000)));
                bool flagAttack = false;
                //出撃ユニット
                foreach (var item in this.ClassGameStatus
                .ClassBattleUnits.SortieUnitGroup)
                {
                    foreach (var itemGroupBy in item.ListClassUnit.Where(x => x.FlagMovingSkill == false))
                    {
                        //スキル優先順位確認
                        foreach (var itemSkill in itemGroupBy.Skill.OrderBy(x => x.SortKey))
                        {
                            //スキル射程範囲確認
                            var xA = itemGroupBy.NowPosi;
                            foreach (var itemDefUnitGroup in this.ClassGameStatus.ClassBattleUnits.DefUnitGroup)
                            {
                                foreach (var itemDefUnitList in itemDefUnitGroup.ListClassUnit)
                                {
                                    //三平方の定理から射程内か確認
                                    {
                                        var xB = itemDefUnitList.NowPosi;
                                        double teihen = xA.X - xB.X;
                                        double takasa = xA.Y - xB.Y;
                                        double syahen = (teihen * teihen) + (takasa * takasa);
                                        double kyori = Math.Sqrt(syahen);

                                        double xAHankei = (32 / 2) + itemSkill.Range;
                                        double xBHankei = 32 / 2;

                                        bool check = true;
                                        if (kyori > (xAHankei + xBHankei))
                                        {
                                            check = false;
                                        }
                                        //チェック
                                        if (check == false)
                                        {
                                            continue;
                                        }
                                    }

                                    itemGroupBy.NowPosiSkill = new Point() { X = itemGroupBy.NowPosi.X, Y = itemGroupBy.NowPosi.Y };
                                    itemGroupBy.OrderPosiSkill = new Point() { X = itemDefUnitList.NowPosi.X, Y = itemDefUnitList.NowPosi.Y };
                                    var calc0 = ClassCalcVec.ReturnVecDistance(
                                                    from: new Point(itemGroupBy.NowPosiSkill.X, itemGroupBy.NowPosiSkill.Y),
                                                    to: itemDefUnitList.NowPosi
                                                    );
                                    itemGroupBy.VecMoveSkill = ClassCalcVec.ReturnNormalize(calc0);
                                    itemGroupBy.FlagMovingSkill = true;

                                    //Image出す
                                    {
                                        Application.Current.Dispatcher.Invoke((Action)(() =>
                                        {
                                            var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapBattle);
                                            var re2 = (Canvas)LogicalTreeHelper.FindLogicalNode(re1, "skillEffect" + itemGroupBy.ID);
                                            if (re2 != null)
                                            {
                                                re1.Children.Remove(re2);
                                            }

                                            Canvas canvas = new Canvas();
                                            canvas.Background = Brushes.Red;
                                            canvas.Height = itemSkill.H;
                                            canvas.Width = itemSkill.W;
                                            canvas.Margin = new Thickness()
                                            {
                                                Left = itemGroupBy.NowPosiSkill.X,
                                                Top = itemGroupBy.NowPosiSkill.Y
                                            };
                                            canvas.Name = "skillEffect" + itemGroupBy.ID;
                                            re1.Children.Add(canvas);
                                        }));
                                    }

                                    //スキル発動スレッド開始
                                    var t = Task.Run(() => TaskBattleSkillExecuteAsync(itemGroupBy, itemDefUnitList, itemSkill));
                                    flagAttack = true;
                                    break;
                                }

                                if (flagAttack == true)
                                {
                                    break;
                                }
                            }
                            if (flagAttack == true)
                            {
                                break;
                            }
                        }
                        if (flagAttack == true)
                        {
                            flagAttack = false;
                            break;
                        }
                    }
                }
            }
        }
        private void TaskBattleMoveAsync(CancellationToken cancelToken)
        {
            Dictionary<long, (Task, CancellationTokenSource)> t = new Dictionary<long, (Task, CancellationTokenSource)>();
            while (true)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                //Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 100000)));
                Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 1000)));

                foreach (var item in this.ClassGameStatus
                .ClassBattleUnits.SortieUnitGroup)
                {
                    foreach (var itemGroupBy in item.ListClassUnit.Where(x => x.FlagMoving == false))
                    {
                        if (itemGroupBy.NowPosi != itemGroupBy.OrderPosi)
                        {
                            //移動スレッド開始
                            var calc0 = ClassCalcVec.ReturnVecDistance(
                                from: new Point(itemGroupBy.NowPosi.X, itemGroupBy.NowPosi.Y),
                                to: itemGroupBy.OrderPosi
                                );
                            itemGroupBy.VecMove = ClassCalcVec.ReturnNormalize(calc0);
                            itemGroupBy.FlagMoving = true;
                            if (t.TryGetValue(itemGroupBy.ID, out (Task, CancellationTokenSource) value))
                            {
                                if (value.Item1 != null)
                                {
                                    value.Item2.Cancel();
                                    t.Remove(itemGroupBy.ID);
                                }
                            }
                            var tokenSource = new CancellationTokenSource();
                            var token = tokenSource.Token;
                            (Task, CancellationTokenSource) aaa = new(Task.Run(() => TaskBattleMoveExecuteAsync(itemGroupBy, token)), tokenSource);
                            t.Add(itemGroupBy.ID, aaa);
                        }
                    }
                }
            }
        }
        private void TaskBattleMoveDefAsync(CancellationToken cancelToken)
        {
            Dictionary<long, (Task, CancellationTokenSource)> t = new Dictionary<long, (Task, CancellationTokenSource)>();

            while (true)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                //Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 100000)));
                Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 1000)));

                foreach (var item in this.ClassGameStatus
                .ClassBattleUnits.DefUnitGroup)
                {
                    foreach (var itemGroupBy in item.ListClassUnit.Where(x => x.FlagMoving == false))
                    {
                        if (itemGroupBy.NowPosi != itemGroupBy.OrderPosi)
                        {
                            //移動スレッド開始
                            var calc0 = ClassCalcVec.ReturnVecDistance(
                                from: new Point(itemGroupBy.NowPosi.X, itemGroupBy.NowPosi.Y),
                                to: itemGroupBy.OrderPosi
                                );
                            itemGroupBy.VecMove = ClassCalcVec.ReturnNormalize(calc0);
                            itemGroupBy.FlagMoving = true;
                            if (t.TryGetValue(itemGroupBy.ID, out (Task, CancellationTokenSource) value))
                            {
                                if (value.Item1 != null)
                                {
                                    value.Item2.Cancel();
                                    t.Remove(itemGroupBy.ID);
                                }
                            }
                            var tokenSource = new CancellationTokenSource();
                            var token = tokenSource.Token;
                            (Task, CancellationTokenSource) aaa = new(Task.Run(() => TaskBattleMoveExecuteAsync(itemGroupBy, token)), tokenSource);
                            t.Add(itemGroupBy.ID, aaa);
                        }
                    }
                }
            }
        }


        private async Task TaskBattleMoveExecuteAsync(ClassUnit classUnit, CancellationToken token)
        {
            try
            {
                //移動し過ぎを防止
                int counter = 1000;

                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 1000)));
                    if (classUnit.NowPosi.X < classUnit.OrderPosi.X + 5
                        && classUnit.NowPosi.X > classUnit.OrderPosi.X - 5
                        && classUnit.NowPosi.Y < classUnit.OrderPosi.Y + 5
                        && classUnit.NowPosi.Y > classUnit.OrderPosi.Y - 5)
                    {
                        classUnit.OrderPosi = new Point()
                        {
                            X = classUnit.NowPosi.X,
                            Y = classUnit.NowPosi.Y
                        };
                        classUnit.FlagMoving = false;
                        return;
                    }
                    else
                    {
                        if (classUnit.FlagMoving == false)
                        {
                            return;
                        }

                        if (classUnit.VecMove.X == 0 && classUnit.VecMove.Y == 0)
                        {
                            classUnit.VecMove = new Point() { X = 0.5, Y = 0.5 };
                        }
                        classUnit.NowPosi = new Point()
                        {
                            X = classUnit.NowPosi.X + (classUnit.VecMove.X * classUnit.Speed),
                            Y = classUnit.NowPosi.Y + (classUnit.VecMove.Y * classUnit.Speed)
                        };
                        await Task.Run(() =>
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                try
                                {
                                    var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapBattle);
                                    var re2 = (Border)LogicalTreeHelper.FindLogicalNode(re1, "border" + classUnit.ID.ToString());
                                    if (re2 != null)
                                    {
                                        re2.Margin = new Thickness(classUnit.NowPosi.X, classUnit.NowPosi.Y, 0, 0);
                                    }
                                }
                                catch (Exception)
                                {
                                    //移動中にゲームを落とすとエラーになるので暫定的に
                                    throw;
                                }
                            }));
                        });
                    }

                    counter--;

                    if (counter <= 0)
                    {
                        throw new Exception("ErrorNumber:000001");
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task TaskBattleSkillExecuteAsync(ClassUnit classUnit, ClassUnit classUnitDef, ClassSkill classSkill)
        {
            //移動し過ぎを防止
            //長い攻撃だとカウンターが降り切れてしまう。どうしたものか。
            int counter = 1000;

            while (true)
            {
                Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 1000)));

                if (classUnit.NowPosiSkill.X < classUnit.OrderPosiSkill.X + 5
                    && classUnit.NowPosiSkill.X > classUnit.OrderPosiSkill.X - 5
                    && classUnit.NowPosiSkill.Y < classUnit.OrderPosiSkill.Y + 5
                    && classUnit.NowPosiSkill.Y > classUnit.OrderPosiSkill.Y - 5)
                {
                    classUnit.FlagMovingSkill = false;
                    await Task.Run(() =>
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapBattle);
                            if (re1 != null)
                            {
                                var re2 = (Canvas)LogicalTreeHelper.FindLogicalNode(re1, "skillEffect" + classUnit.ID.ToString());
                                if (re2 != null)
                                {
                                    re1.Children.Remove(re2);
                                }
                            }
                        }));
                    });

                    //体力計算処理
                    //どちらがプレイヤー側かで分ける
                    if (true)
                    {
                        foreach (var item in this.ClassGameStatus.ClassBattleUnits.DefUnitGroup)
                        {
                            var re = item.ListClassUnit.Where(x => x.NowPosi.X <= classUnit.NowPosiSkill.X + 5
                                                        && x.NowPosi.X >= classUnit.NowPosiSkill.X - 5
                                                        && x.NowPosi.Y <= classUnit.NowPosiSkill.Y + 5
                                                        && x.NowPosi.Y >= classUnit.NowPosiSkill.Y - 5);

                            foreach (var itemRe in re)
                            {
                                itemRe.Hp = (int)(itemRe.Hp - (Math.Floor((classSkill.Str.Item2 * 0.1) * classUnit.Attack)));
                                if (itemRe.Hp <= 0)
                                {
                                    item.ListClassUnit.Remove(itemRe);
                                    await Task.Run(() =>
                                    {
                                        Application.Current.Dispatcher.Invoke((Action)(() =>
                                        {
                                            var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapBattle);
                                            if (re1 != null)
                                            {
                                                var re2 = (Border)LogicalTreeHelper.FindLogicalNode(re1, "border" + itemRe.ID.ToString());
                                                if (re2 != null)
                                                {
                                                    re1.Children.Remove(re2);
                                                }
                                            }
                                        }));
                                    });
                                }
                            }
                        }
                    }

                    classUnit.OrderPosiSkill = new Point()
                    {
                        X = classUnit.NowPosiSkill.X,
                        Y = classUnit.NowPosiSkill.Y
                    };

                    return;
                }
                else
                {
                    if (classUnit.VecMoveSkill.X == 0 && classUnit.VecMoveSkill.Y == 0)
                    {
                        classUnit.VecMoveSkill = new Point() { X = 0.5, Y = 0.5 };
                    }
                    classUnit.NowPosiSkill = new Point()
                    {
                        X = classUnit.NowPosiSkill.X + (classUnit.VecMoveSkill.X * classSkill.Speed),
                        Y = classUnit.NowPosiSkill.Y + (classUnit.VecMoveSkill.Y * classSkill.Speed)
                    };
                    await Task.Run(() =>
                    {
                        try
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapBattle);
                                if (re1 != null)
                                {
                                    var re2 = (Canvas)LogicalTreeHelper.FindLogicalNode(re1, "skillEffect" + classUnit.ID.ToString());
                                    if (re2 != null)
                                    {
                                        re2.Margin = new Thickness(classUnit.NowPosiSkill.X, classUnit.NowPosiSkill.Y, 0, 0);
                                    }
                                }
                            }));
                        }
                        catch (Exception)
                        {
                            //攻撃中にゲームを落とすとエラーになるので暫定的に
                            //throw;
                        }
                    });
                }

                counter--;

                if (counter <= 0)
                {
                    classUnit.OrderPosiSkill = new Point()
                    {
                        X = classUnit.NowPosiSkill.X,
                        Y = classUnit.NowPosiSkill.Y
                    };
                    classUnit.FlagMovingSkill = false;
                    await Task.Run(() =>
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapBattle);
                            var re2 = (Canvas)LogicalTreeHelper.FindLogicalNode(re1, "skillEffect" + classUnit.ID.ToString());
                            if (re2 != null)
                            {
                                re1.Children.Remove(re2);
                            }
                        }));
                    });

                    // エラーログ出力としたい
                    //throw new Exception("ErrorNumber:000001");
                    return;
                }

            }
        }

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
            while (condition.Wait(100) == false)
            {
                // 待っている間も一定時間ごとに表示を更新する。
                // これによって、ウインドウの操作や入力の処理が動くっぽい。
                DoEvents();
            }
            Thread.Sleep(1);
            condition.Reset();

            // メッセージ枠を取り除く。
            this.canvasMain.Children.Remove(frame);
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
                evaluator.window = this;
                evaluator.Eval(ev.Root, enviroment);
                ev.Yet = false;
            }
        }

        private void SetWindowStrategyMenu()
        {
            int widthCanvas = 350;
            int HeightCanvas = 350;

            //Uri uri = new Uri("/Page005_StrategyMenu.xaml", UriKind.Relative);
            //frame.Source = uri;

            Application.Current.Properties["window"] = this;

            Frame frame = new Frame();
            if (this.ClassGameStatus.WindowStrategyMenu == null)
            {
                this.ClassGameStatus.WindowStrategyMenu = new Page005_StrategyMenu();
            }
            this.ClassGameStatus.WindowStrategyMenu.SetData();
            frame.Navigate(this.ClassGameStatus.WindowStrategyMenu);
            frame.Margin = new Thickness()
            {
                Left = this.canvasMain.Width + this.canvasMain.Margin.Left - widthCanvas,
                Top = this.canvasMain.Height + this.canvasMain.Margin.Top - HeightCanvas
            };
            frame.Name = StringName.canvasWindowStrategy;
            this.canvasMain.Children.Add(frame);

            //TODO https://yudachi-shinko.blogspot.com/2019/09/wpfframepage.html
            while (frame.CanGoBack)
            {
                frame.RemoveBackEntry();
            }
        }

        private SolidColorBrush ReturnBaseColor()
        {
            return new SolidColorBrush(Color.FromRgb(190, 178, 175));
        }

        #endregion

    }
}
