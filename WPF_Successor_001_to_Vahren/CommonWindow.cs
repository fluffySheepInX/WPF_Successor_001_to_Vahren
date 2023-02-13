using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using WPF_Successor_001_to_Vahren._005_Class;
using WPF_Successor_001_to_Vahren._006_ClassStatic;
using WPF_Successor_001_to_Vahren._010_Enum;

namespace WPF_Successor_001_to_Vahren
{
    public class CommonWindow : Window
    {
        #region NowNumberGameTitle
        private int _nowNumberGameTitle = 0;
        public int NowNumberGameTitle
        {
            get
            {
                return _nowNumberGameTitle;
            }
            set { _nowNumberGameTitle = value; }
        }
        #endregion
        public int _sizeClientWinWidth = 0;
        public int _sizeClientWinHeight = 0;
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
        #region ClassGameStatus
        private ClassGameStatus _classGameStatus = new ClassGameStatus();
        public ClassGameStatus ClassGameStatus
        {
            get { return _classGameStatus; }
            set { _classGameStatus = value; }
        }
        #endregion
        #region ClassConfigGameTitle
        private ClassConfigGameTitle _classConfigGameTitle = new ClassConfigGameTitle();
        public ClassConfigGameTitle ClassConfigGameTitle
        {
            get
            {
                return _classConfigGameTitle;
            }
            set { _classConfigGameTitle = value; }
        }
        #endregion
        #region Dir001_Warehouse
        /// <summary>
        /// ゲームデータを格納するフォルダ名
        /// </summary>
        public string Dir001_Warehouse
        {
            get
            {
                return System.IO.Path.Combine(Environment.CurrentDirectory, "001_Warehouse");
            }
        }
        #endregion
        #region FileOrderDocument
        /// <summary>
        /// 基礎的なゲームデータを設定するテキストファイル名
        /// </summary>
        public string FileOrderDocument
        {
            get
            {
                return System.IO.Path.Combine(Dir001_Warehouse, "OrderDocument.txt");
            }
        }
        #endregion
        #region IsEng
        /// <summary>
        /// 英語か否か
        /// </summary>
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
        #region IsBattle
        public bool IsBattle { get; set; } = false;
        #endregion
        #region フェード関係
        private bool _fadeOut = false;
        private bool _fadeOutExecution = false;
        private bool _fadeIn = false;
        private bool _fadeInExecution = false;
        private bool _afterFadeIn = false;

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

        public DispatcherTimer timerAfterFadeIn = new DispatcherTimer(DispatcherPriority.Background);

        public delegate void DelegateMapRenderedFromBattle();
        public DelegateMapRenderedFromBattle? delegateMapRenderedFromBattle = null;

        #region Event
        public void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // ESCキーを押すと終了する。
            if (e.Key == Key.Escape)
            {
                if (MessageBox.Show("ゲームを終了しますか？", "ローガントゥーガ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    this.Close();
                }
            }
            // F11キーを押すとフルスクリーン状態を切り替える。
            else if ((e.Key == Key.F11) && (e.IsRepeat == false))
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
            // Enter, Space, Z キー = OK
            else if ((e.Key == Key.Return) || (e.Key == Key.Space) || (e.Key == Key.Z))
            {
                // キーを押しっぱなしにしても無視する
                if (e.IsRepeat == false)
                {
                    // テキストウィンドウが存在する時
                    if (this.ClassGameStatus.TextWindow != null)
                    {
                        if (this.ClassGameStatus.TextWindow is UserControl050_Msg)
                        {
                            // 次の文章を表示待ちなら
                            var textWindow = (UserControl050_Msg)(this.ClassGameStatus.TextWindow);
                            if (textWindow.NextText() == true)
                            {
                                return;
                            }
                        }

                        var mainWindow = (MainWindow)Application.Current.MainWindow;
                        if (mainWindow != null)
                        {
                            // カウンターが 0よりも多い時だけ減らす
                            if (mainWindow.condition.IsSet == false)
                            {
                                mainWindow.condition.Signal();
                            }
                        }
                    }
                }
            }
            // Control キー = Repeating OK
            else if ((e.Key == Key.RightCtrl) || (e.Key == Key.LeftCtrl))
            {
                // テキストウィンドウが存在する時
                if (this.ClassGameStatus.TextWindow != null)
                {
                    if (this.ClassGameStatus.TextWindow is UserControl050_Msg)
                    {
                        // 次の文章を表示待ちなら
                        var textWindow = (UserControl050_Msg)(this.ClassGameStatus.TextWindow);
                        if (textWindow.NextText() == true)
                        {
                            return;
                        }
                    }

                    var mainWindow = (MainWindow)Application.Current.MainWindow;
                    if (mainWindow != null)
                    {
                        // カウンターが 0よりも多い時だけ減らす
                        if (mainWindow.condition.IsSet == false)
                        {
                            mainWindow.condition.Signal();
                        }
                    }
                }
            }
        }
        public void canvasTop_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var beseCanvas = ((Canvas)((Window)((Canvas)sender).Parent).Content);
            var re = (Canvas)LogicalTreeHelper.FindLogicalNode(beseCanvas, "canvasMain");
            if (re == null) return;
            var re2 = (Canvas)LogicalTreeHelper.FindLogicalNode(beseCanvas, "canvasUIRightTop");
            if (re2 == null) return;
            var re3 = (Canvas)LogicalTreeHelper.FindLogicalNode(beseCanvas, "canvasUIRightBottom");
            if (re3 == null) return;
            var re4 = (Canvas)LogicalTreeHelper.FindLogicalNode(beseCanvas, "canvasUI");
            if (re4 == null) return;

            // クライアント領域を知る方法
            var si = e.NewSize;
            this._sizeClientWinWidth = (int)si.Width;
            this._sizeClientWinHeight = (int)si.Height;

            // canvasMain を常にウインドウの中央に置く。
            re.Margin = new Thickness()
            {
                Top = (this._sizeClientWinHeight / 2) - (this.CanvasMainHeight / 2),
                Left = (this._sizeClientWinWidth / 2) - (this.CanvasMainWidth / 2)
            };
            // canvasUI も canvasMain と同じく中央に置く。
            re4.Margin = re.Margin;

            // canvasUIRightTop をウインドウの右上隅に置く。
            {
                double newTop, newLeft;
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
                re2.Margin = new Thickness()
                {
                    Top = newTop,
                    Left = newLeft
                };
            }
            // canvasUIRightBottom をウインドウの右下隅に置く。
            {
                double newTop, newLeft;
                if (this._sizeClientWinHeight > this.CanvasMainHeight)
                {
                    newTop = (this._sizeClientWinHeight / 2) - (this.CanvasMainHeight / 2);
                }
                else
                {
                    // ウインドウの高さが低い場合は下端に合わせる。
                    newTop = this._sizeClientWinHeight - this.CanvasMainHeight;
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
                re3.Margin = new Thickness()
                {
                    Top = newTop,
                    Left = newLeft
                };
            }
        }
        public void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.ClassGameStatus.NowSituation = _010_Enum.Situation.GameStop;
            foreach (var item in this.ClassGameStatus.TaskBattleMoveAsync)
            {
                if (item.Item1 != null)
                {
                    item.Item2.Cancel();
                }
            }
            foreach (var item in this.ClassGameStatus.TaskBattleMoveDefAsync)
            {
                if (item.Item1 != null)
                {
                    item.Item2.Cancel();
                }
            }

            if (this.ClassGameStatus.TaskBattleSkill.Item1 != null)
            {
                this.ClassGameStatus.TaskBattleSkill.Item2.Cancel();
            }
            if (this.ClassGameStatus.TaskBattleSkillDef.Item1 != null)
            {
                this.ClassGameStatus.TaskBattleSkillDef.Item2.Cancel();
            }
        }

        #region BattleEvent
        /// <summary>
        /// ドラッグを開始する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CanvasMapBattle_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            var cw = ClassStaticCommonMethod.FindAncestors((Canvas)sender).OfType<CommonWindow>().FirstOrDefault();
            if (cw == null) return;
            UIElement? el = sender as UIElement;
            if (el == null) return;

            cw.ClassGameStatus.IsDrag = true;
            cw.ClassGameStatus.StartPoint = e.GetPosition(el);
            el.CaptureMouse();
            el.MouseLeftButtonUp += CanvasMapBattle_MouseLeftButtonUp;
            el.MouseMove += CanvasMapBattle_MouseMove;
        }
        /// <summary>
        /// ドラック中なら終了する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CanvasMapBattle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var cw = ClassStaticCommonMethod.FindAncestors((Canvas)sender).OfType<CommonWindow>().FirstOrDefault();
            if (cw == null) return;
            if (cw.ClassGameStatus.IsDrag == false) return;
            UIElement? el = sender as UIElement;
            if (el == null) return;

            el.ReleaseMouseCapture();
            el.MouseLeftButtonUp -= CanvasMapBattle_MouseLeftButtonUp;
            el.MouseMove -= CanvasMapBattle_MouseMove;
            this.ClassGameStatus.IsDrag = false;
        }
        /// <summary>
        /// ドラック中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CanvasMapBattle_MouseMove(object sender, MouseEventArgs e)
        {
            var cw = ClassStaticCommonMethod.FindAncestors((Canvas)sender).OfType<CommonWindow>().FirstOrDefault();
            if (cw == null) return;
            if (cw.ClassGameStatus.IsDrag == false) return;

            UIElement? el = sender as UIElement;
            if (el == null) return;

            var ri2 = ClassStaticCommonMethod.FindAncestors((Canvas)sender).OfType<Canvas>().Where(x => x.Name == StringName.canvasMain).FirstOrDefault();
            if (ri2 == null) return;
            var ri = (Canvas)LogicalTreeHelper.FindLogicalNode(ri2, StringName.windowMapBattle);

            Point pt = e.GetPosition(el);
            var thickness = new Thickness();
            thickness.Left = ri.Margin.Left + (pt.X - cw.ClassGameStatus.StartPoint.X);
            thickness.Top = ri.Margin.Top + (pt.Y - cw.ClassGameStatus.StartPoint.Y);
            ri.Margin = thickness;
        }
        /// <summary>
        /// 移動フラグを立てたり、枠の色を消したり
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void windowMapBattle_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            var ri2 = ClassStaticCommonMethod.FindAncestors((Canvas)sender).OfType<Canvas>().Where(x => x.Name == StringName.canvasMain).FirstOrDefault();
            if (ri2 == null) return;
            var ri = (Canvas)LogicalTreeHelper.FindLogicalNode(ri2, StringName.windowMapBattle);

            //SortieUnitGroupではなくプレイヤー側でないとダメ
            var cw = ClassStaticCommonMethod.FindAncestors((Canvas)sender).OfType<CommonWindow>().FirstOrDefault();
            if (cw == null) return;

            List<ClassHorizontalUnit> lisClassHorizontalUnit = new List<ClassHorizontalUnit>();
            switch (cw.ClassGameStatus.ClassBattle.BattleWhichIsThePlayer)
            {
                case BattleWhichIsThePlayer.Sortie:
                    lisClassHorizontalUnit = ClassGameStatus.ClassBattle.SortieUnitGroup;
                    break;
                case BattleWhichIsThePlayer.Def:
                    lisClassHorizontalUnit = ClassGameStatus.ClassBattle.DefUnitGroup;
                    break;
                case BattleWhichIsThePlayer.None:
                    break;
                default:
                    break;
            }

            foreach (var item in lisClassHorizontalUnit)
            {
                var re = item.ListClassUnit.Where(x => x.FlagMove == true).FirstOrDefault();
                if (re == null) continue;

                var nowOrderPosi = e.GetPosition(ri);
                if (re.FlagMoving = true && re.OrderPosiLeft != nowOrderPosi)
                {
                    re.FlagMoveDispose = true;
                }
                re.OrderPosiLeft = nowOrderPosi;
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
        /// <summary>
        /// 移動フラグを立てたり、枠に色を付けたり
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void WindowMapBattleUnit_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            var can = (Canvas)sender;
            var bor = (Border)can.Parent;
            long name = long.Parse((string)(can).Tag);

            var cw = ClassStaticCommonMethod.FindAncestors((Canvas)sender).OfType<CommonWindow>().FirstOrDefault();
            if (cw == null) return;

            List<ClassHorizontalUnit> lisClassHorizontalUnit = new List<ClassHorizontalUnit>();
            switch (cw.ClassGameStatus.ClassBattle.BattleWhichIsThePlayer)
            {
                case BattleWhichIsThePlayer.Sortie:
                    lisClassHorizontalUnit = ClassGameStatus.ClassBattle.SortieUnitGroup;
                    break;
                case BattleWhichIsThePlayer.Def:
                    lisClassHorizontalUnit = ClassGameStatus.ClassBattle.DefUnitGroup;
                    break;
                case BattleWhichIsThePlayer.None:
                    break;
                default:
                    break;
            }

            foreach (var item in lisClassHorizontalUnit)
            {
                var re = item.ListClassUnit
                            .Where(x => x.ID == name)
                            .FirstOrDefault();
                if (re == null) continue;

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
        public void btnDebugWin_Click(object sender, RoutedEventArgs e)
        {
            var cw = ClassStaticCommonMethod.FindAncestors((Button)sender).OfType<CommonWindow>().FirstOrDefault();
            if (cw == null) return;

            List<ClassHorizontalUnit> lisClassHorizontalUnit = new List<ClassHorizontalUnit>();
            switch (cw.ClassGameStatus.ClassBattle.BattleWhichIsThePlayer)
            {
                case BattleWhichIsThePlayer.Sortie:
                    lisClassHorizontalUnit = ClassGameStatus.ClassBattle.DefUnitGroup;
                    break;
                case BattleWhichIsThePlayer.Def:
                    lisClassHorizontalUnit = ClassGameStatus.ClassBattle.SortieUnitGroup;
                    break;
                case BattleWhichIsThePlayer.None:
                    break;
                default:
                    break;
            }

            lisClassHorizontalUnit.Clear();
        }

        #endregion

        #endregion

        public void ReadFileOrderDocument()
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
                //  空行
                if (item.Length < 1) continue;
                //  コメント行
                if (item[0] == '#') continue;

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
                            this.ClassGameStatus.CommonWindow = this;
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
                    case "BattleThread":
                        {
                            int num = 1;
                            int.TryParse(resultSplit[1], out num);
                            this.ClassGameStatus.BattleThread = num;
                        }
                        break;
                    case "DebugBattle":
                        {
                            bool aaa = false;
                            bool.TryParse(resultSplit[1], out aaa);
                            this.ClassGameStatus.IsDebugBattle = aaa;
                        }
                        break;
                    default:
                        break;
                }
                //識別子処理の終わり
            }
            //一行毎の読み込み終わり
        }

        public string GetPathDirectoryGameTitleFullName()
        {
            return ClassConfigGameTitle.DirectoryGameTitle[NowNumberGameTitle].FullName;
        }

        public static Canvas SetAndGetCanvasBattleBack(Canvas canvas,
                                                int _sizeClientWinWidth,
                                                int _sizeClientWinHeight,
                                                int CanvasMainWidth,
                                                int CanvasMainHeight
                                                )
        {
            Canvas backCanvas = new Canvas();
            backCanvas.Name = StringName.gridMapBattle;
            backCanvas.Background = Brushes.Brown;
            backCanvas.Width = _sizeClientWinWidth;
            backCanvas.Height = _sizeClientWinHeight;
            backCanvas.Margin = new Thickness()
            {
                Left = (CanvasMainWidth / 2) - (_sizeClientWinWidth / 2),
                Top = (CanvasMainHeight / 2) - (_sizeClientWinHeight / 2)
            };
            backCanvas.Children.Add(canvas);

            Canvas.SetZIndex(backCanvas, 98);
            return backCanvas;
        }

    }
}
