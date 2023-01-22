using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPF_Successor_001_to_Vahren._005_Class;
using WPF_Successor_001_to_Vahren._010_Enum;
using static WPF_Successor_001_to_Vahren.CommonWindow;
using Application = System.Windows.Application;
using Image = System.Windows.Controls.Image;

namespace WPF_Successor_001_to_Vahren._006_ClassStatic
{
    public static class ClassStaticBattle
    {
        public static int TakasaMapTip { get; set; } = 32;
        public static int yokoMapTip { get; set; } = 64;
        public static int TakasaUnit { get; set; } = 32;
        public static int yokoUnit { get; set; } = 32;

        public static void AddBuilding(ClassGameStatus? classGameStatus)
        {
            if (classGameStatus == null) return;
            if (classGameStatus.ClassBattle.ClassMapBattle == null) return;

            //建築物設定

            List<(string, int, int)> bui = new List<(string, int, int)>();
            foreach (var battle in classGameStatus.ClassBattle.ClassMapBattle.MapData.Select((value, index) => (value, index)))
            {
                foreach (var item in battle.value.Select((value, index) => (value, index)))
                {
                    if (item.value.Building.Count != 0)
                    {
                        foreach (var building in item.value.Building)
                        {
                            bui.Add(new(building, battle.index, item.index));
                        }
                    }
                }
            }

            List<ClassUnit> uni = new List<ClassUnit>();
            ClassHorizontalUnit classHorizontalUnit = new ClassHorizontalUnit();
            classHorizontalUnit.FlagBuilding = true;
            foreach (var item in bui)
            {
                var re = classGameStatus.ListObject.Where(x => x.NameTag == item.Item1 && x.Type == _010_Enum.MapTipObjectType.GATE).FirstOrDefault();
                if (re == null) continue;
                long id = classGameStatus.IDCount;
                classGameStatus.SetIDCount();

                uni.Add(new ClassUnitBuilding()
                {
                    ID = id
                    ,
                    Hp = re.Castle
                    ,
                    X = item.Item2
                    ,
                    Y = item.Item3
                    ,
                    Defense = re.CastleDefense
                    ,
                    MagDef = re.CastleMagdef
                    ,
                    Type = MapTipObjectType.GATE
                });
            }
            classHorizontalUnit.ListClassUnit = uni;
            classGameStatus.ClassBattle.DefUnitGroup.Add(classHorizontalUnit);
        }

        public static bool CheckRecObj(bool ch, IEnumerable<Rectangle> targetTip, ClassGameStatus classGameStatus)
        {
            foreach (Rectangle item in targetTip)
            {
                var ob = classGameStatus.ListObject.Where(x => x.NameTag == ((ClassMapTipRectangle)item.Tag).TipName).FirstOrDefault();
                if (ob != null)
                {
                    //デバッグの為、一つにまとめない
                    switch (ob.Type)
                    {
                        case MapTipObjectType.WALL2:
                            ch = false;
                            break;
                        case MapTipObjectType.GATE:
                            ch = false;
                            break;
                        default:
                            break;
                    }
                }
            }

            return ch;
        }

        public static (double, double) ConvertVec90(double x, double y, double vecX, double vecY)
        {
            //(cos90*(x-vecX))+(-sin90*(y-vecY)) = x
            //(sin90*(x-vecX))+(cos90*(y-vecY)) = y
            double resultX = (0 * (x - vecX)) + (-1 * (y - vecY));
            double resultY = (1 * (x - vecX)) + (0 * (y - vecY));

            return (resultX + x, resultY + y);
        }
        public static (double, double) ConvertVecX(double rad, double x, double y, double charX, double charY)
        {
            //キャラクタを基に回転させないと、バグる

            double x2 = 0;
            double y2 = 0;
            x2 = ((x - charX) * Math.Cos(rad)) - ((y - charY) * Math.Sin(rad));
            y2 = ((x - charX) * Math.Sin(rad)) + ((y - charY) * Math.Cos(rad));
            return (x2 + charX, y2 + charY);
        }

        #region GetRecObj
        /// <summary>
        /// 現在の位置に建築物があるなら、その建築物を返却する
        /// </summary>
        /// <param name="getMap"></param>
        /// <param name="nowPosiX"></param>
        /// <param name="nowPosiY"></param>
        /// <returns></returns>
        public static IEnumerable<Rectangle> GetRecObj(List<Rectangle> getMap, double nowPosiX, double nowPosiY)
        {
            return getMap
                                .Where(x => ((ClassMapTipRectangle)x.Tag).LogicalXY.Left <= nowPosiX
                                        && (((ClassMapTipRectangle)x.Tag).LogicalXY.Left + 64) >= nowPosiX)
                                .Where(y => ((ClassMapTipRectangle)y.Tag).LogicalXY.Top <= nowPosiY
                                        && (((ClassMapTipRectangle)y.Tag).LogicalXY.Top + 32) >= nowPosiY);
        }
        #endregion
        #region GetPathTipImage
        /// <summary>
        /// TipImageのパスを取得する
        /// </summary>
        /// <param name="itemListClassUnit"></param>
        /// <param name="directoryGameTitleFullName"></param>
        /// <returns></returns>
        public static string GetPathTipImage((ClassUnit value, int index) itemListClassUnit, string directoryGameTitleFullName)
        {
            List<string> strings = new List<string>();
            strings.Add(directoryGameTitleFullName);
            strings.Add("040_ChipImage");
            strings.Add(itemListClassUnit.value.Image);
            string path = System.IO.Path.Combine(strings.ToArray());
            return path;
        }
        #endregion
        #region GetFiles015_BattleMapCellImage
        /// <summary>
        /// GetFiles015_BattleMapCellImage
        /// </summary>
        /// <param name="directoryGameTitleFullName"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetFiles015_BattleMapCellImage(string directoryGameTitleFullName)
        {
            List<string> strings = new List<string>();
            strings.Add(directoryGameTitleFullName);
            strings.Add("015_BattleMapCellImage");
            string cellImagePath = System.IO.Path.Combine(strings.ToArray());
            // get file.
            var files = System.IO.Directory.EnumerateFiles(
                cellImagePath,
                "*.png",
                System.IO.SearchOption.AllDirectories
                );
            return files;
        }
        #endregion

        #region SetBattleMap関係

        public static void DisplayBuilding(Canvas canvas, int takasaMapTip, int yokoMapTip, List<(BitmapImage, int, int)> listTakaiObj, List<Rectangle> getMap)
        {
            foreach (var item in listTakaiObj.OrderBy(x => x.Item2).ThenByDescending(y => y.Item3))
            {
                ImageBrush image = new ImageBrush();
                image.Stretch = Stretch.Fill;
                image.ImageSource = item.Item1;

                System.Windows.Shapes.Rectangle rectangle = new Rectangle();
                rectangle.Name = "Bui" + item.Item2 + "a" + item.Item3;
                ClassMapTipRectangle classMapTipRectangle = new ClassMapTipRectangle();
                classMapTipRectangle.TipName = System.IO.Path.GetFileNameWithoutExtension(item.Item1.UriSource.AbsolutePath);
                classMapTipRectangle.LogicalXY = new Thickness()
                {
                    Left = (item.Item2 * (yokoMapTip / 2)) + (item.Item3 * (yokoMapTip / 2)),
                    Top = ((canvas.Height / 2) + (item.Item2 * (takasaMapTip / 2)) + (item.Item3 * (-(takasaMapTip / 2)))) - takasaMapTip / 2
                };
                classMapTipRectangle.TipXY = new Point(item.Item2, item.Item3);

                rectangle.Tag = classMapTipRectangle;
                rectangle.Fill = image;
                rectangle.Stretch = Stretch.Fill;
                rectangle.StrokeThickness = 0;
                rectangle.Width = yokoMapTip;
                rectangle.Height = item.Item1.PixelHeight;
                rectangle.Margin = new Thickness()
                {
                    Left = (item.Item2 * (yokoMapTip / 2)) + (item.Item3 * (yokoMapTip / 2)),
                    Top = ((canvas.Height / 2) + (item.Item2 * (takasaMapTip / 2)) + (item.Item3 * (-(takasaMapTip / 2)))) - (item.Item1.PixelHeight - takasaMapTip / 2)
                };
                canvas.Children.Add(rectangle);
                getMap.Add(rectangle);
            }
        }
        public static void CreatePathIntoCanvas(Canvas canvas, int takasaMapTip, int yokoMapTip, Dictionary<string, string> map,
                                                            ClassGameStatus classGameStatus,
                                                           out List<(BitmapImage, int, int)> listTakaiObj)
        {
            //out 初期代入
            listTakaiObj = new List<(BitmapImage, int, int)>();

            if (classGameStatus.ClassBattle.ClassMapBattle is null) return;

            foreach (var itemCol in classGameStatus.ClassBattle.ClassMapBattle.MapData
                                    .Select((value, index) => (value, index)))
            {
                foreach (var itemRow in itemCol.value.Select((value, index) => (value, index)))
                {
                    map.TryGetValue(itemRow.value.Tip, out string? value);
                    if (value == null) continue;

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
        }
        #region CreatePageBattle
        /// <summary>
        /// 戦闘画面で情報を表示する窓を作り出す処理
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="_sizeClientWinHeight"></param>
        /// <param name="window"></param>
        public static void CreatePageBattle(Canvas canvas, int _sizeClientWinHeight, Window window)
        {
            Application.Current.Properties["window"] = window;
            {
                Uri uri = new Uri("/Page025_Battle_Command.xaml", UriKind.Relative);
                Frame frame = new Frame();
                frame.Source = uri;
                frame.Margin = new Thickness(0, _sizeClientWinHeight - 310 - 60, 0, 0);
                frame.Name = StringName.windowBattleCommand;
                Canvas.SetZIndex(frame, 99);
                canvas.Children.Add(frame);
            }
            {
                Uri uri = new Uri("/Page026_Battle_SelectUnit.xaml", UriKind.Relative);
                Frame frame = new Frame();
                frame.Source = uri;
                frame.Margin = new Thickness(0, _sizeClientWinHeight - 120, 0, 0);
                frame.Name = StringName.windowBattleCommand;
                Canvas.SetZIndex(frame, 99);
                canvas.Children.Add(frame);
            }
        }
        #endregion

        #region CreateCanvasBattle
        /// <summary>
        /// バトル用のキャンバス作成
        /// </summary>
        /// <param name="classMapBattle"></param>
        /// <param name="takasaMapTip"></param>
        /// <param name="yokoMapTip"></param>
        /// <param name="_sizeClientWinHeight"></param>
        /// <param name="canvasMainWidth"></param>
        /// <param name="_sizeClientWinWidth"></param>
        /// <param name="canvasMapBattle_MouseLeftButtonDown"></param>
        /// <param name="windowMapBattle_MouseRightButtonDown"></param>
        /// <returns></returns>
        public static Canvas CreateCanvasBattle(ClassMapBattle? classMapBattle, int takasaMapTip, int yokoMapTip,
                                            int _sizeClientWinHeight, int canvasMainWidth, int _sizeClientWinWidth,
                                            MouseButtonEventHandler canvasMapBattle_MouseLeftButtonDown,
                                            MouseButtonEventHandler windowMapBattle_MouseRightButtonDown
                                            )
        {
            Canvas canvas = new Canvas();
            canvas.Name = StringName.windowMapBattle;
            canvas.Background = Brushes.Black;
            canvas.MouseLeftButtonDown += canvasMapBattle_MouseLeftButtonDown;
            canvas.MouseRightButtonDown += windowMapBattle_MouseRightButtonDown;

            if (classMapBattle is null)
            {
                int BaseNum = 600;
                canvas.Width = BaseNum
                                + (BaseNum / 2);
                canvas.Height = BaseNum;
                canvas.Margin = new Thickness()
                {
                    Left = BaseNum / 2,
                    Top = (_sizeClientWinHeight / 2) - (canvas.Height / 2)
                };
            }
            else
            {
                canvas.Width = classMapBattle.MapData[0].Count * yokoMapTip;
                canvas.Height = classMapBattle.MapData.Count * takasaMapTip;
                canvas.Margin = new Thickness()
                {
                    Left = ((
                            (canvasMainWidth / 2) - (_sizeClientWinWidth / 2)
                            ))
                                +
                            (_sizeClientWinWidth / 2) - ((classMapBattle.MapData[0].Count * 32) / 2),
                    Top = (_sizeClientWinHeight / 2) - (canvas.Height / 2)
                };
            }

            return canvas;
        }
        #endregion

        #endregion

        #region 移動関係

        public static void TaskBattleMoveExecuteAsync(ClassUnit classUnit,
                                                            CancellationToken token,
                                                            ClassGameStatus classGameStatus,
                                                            Window window)
        {
            try
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 1000)));

                    ClassVec classVec = new ClassVec();
                    classVec.Target = new Point(classUnit.OrderPosiLeft.X, classUnit.OrderPosiLeft.Y);
                    classVec.Vec = new Point(classUnit.VecMove.X, classUnit.VecMove.Y);
                    classVec.Speed = classUnit.Speed;

                    if (classVec.Hit(new Point(classUnit.NowPosiLeft.X, classUnit.NowPosiLeft.Y)))
                    {
                        classUnit.OrderPosiLeft = new Point()
                        {
                            X = classUnit.NowPosiLeft.X,
                            Y = classUnit.NowPosiLeft.Y
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

                        //移動後の位置計算
                        double afterNowPosiX = classUnit.NowPosiLeft.X + (classUnit.VecMove.X * classUnit.Speed);
                        double afterNowPosiY = classUnit.NowPosiLeft.Y + (classUnit.VecMove.Y * classUnit.Speed);

                        if (Application.Current == null)
                        {
                            Environment.Exit(1);
                        }

                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            try
                            {
                                bool ch = true;
                                var targetTip = ClassStaticBattle.GetRecObj(classGameStatus.ClassBattle.ListBuildingAlive, afterNowPosiX, afterNowPosiY);
                                ch = ClassStaticBattle.CheckRecObj(ch, targetTip, classGameStatus);

                                if (ch == true)
                                {
                                    //移動後に建築物無し
                                    classUnit.NowPosiLeft = new Point()
                                    {
                                        X = afterNowPosiX,
                                        Y = afterNowPosiY
                                    };

                                    var ca1 = (Canvas)window.Content;
                                    var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(ca1, StringName.windowMapBattle);
                                    if (re1 != null)
                                    {
                                        var re2 = (Border)LogicalTreeHelper.FindLogicalNode(re1, "border" + classUnit.ID.ToString());
                                        if (re2 != null)
                                        {
                                            re2.Margin = new Thickness(classUnit.NowPosiLeft.X, classUnit.NowPosiLeft.Y, 0, 0);
                                        }
                                    }
                                }
                                else
                                {
                                    ////移動後に建築物有り
                                    //移動後の位置を再計算（一度バックする）
                                    afterNowPosiX = classUnit.NowPosiLeft.X + (classUnit.VecMove.X * -(classUnit.Speed * 5));
                                    afterNowPosiY = classUnit.NowPosiLeft.Y + (classUnit.VecMove.Y * -(classUnit.Speed * 5));
                                    //afterNowPosiX = classUnit.NowPosi.X + (classUnit.VecMove.X * -(32));
                                    //afterNowPosiY = classUnit.NowPosi.Y + (classUnit.VecMove.Y * -(16));
                                    //行列変換
                                    var resultConv = ClassStaticBattle.ConvertVec90(afterNowPosiX, afterNowPosiY, classUnit.NowPosiLeft.X, classUnit.NowPosiLeft.Y);

                                    bool ch2 = true;
                                    var targetTip2 = ClassStaticBattle.GetRecObj(classGameStatus.ClassBattle.ListBuildingAlive, resultConv.Item1, resultConv.Item2);
                                    ch2 = ClassStaticBattle.CheckRecObj(ch2, targetTip2, classGameStatus);

                                    if (ch2 == true)
                                    {
                                        //移動後に建築物無し
                                        classUnit.NowPosiLeft = new Point()
                                        {
                                            X = resultConv.Item1,
                                            Y = resultConv.Item2
                                        };

                                        var ca1 = (Canvas)window.Content;
                                        var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(ca1, StringName.windowMapBattle);
                                        if (re1 != null)
                                        {
                                            var re2 = (Border)LogicalTreeHelper.FindLogicalNode(re1, "border" + classUnit.ID.ToString());
                                            if (re2 != null)
                                            {
                                                re2.Margin = new Thickness(classUnit.NowPosiLeft.X, classUnit.NowPosiLeft.Y, 0, 0);
                                            }
                                        }

                                        //再計算する
                                        var calc0 = ClassCalcVec.ReturnVecDistance(
                                            from: new Point(classUnit.NowPosiLeft.X, classUnit.NowPosiLeft.Y),
                                            to: classUnit.OrderPosiLeft
                                            );
                                        classUnit.VecMove = ClassCalcVec.ReturnNormalize(calc0);
                                    }
                                    else
                                    {
                                        ////バックして90度変換した後に建築物有り
                                        //止まる
                                        var ca1 = (Canvas)window.Content;
                                        var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(ca1, StringName.windowMapBattle);
                                        if (re1 != null)
                                        {
                                            var re2 = (Border)LogicalTreeHelper.FindLogicalNode(re1, "border" + classUnit.ID.ToString());
                                            if (re2 != null)
                                            {
                                                re2.Margin = new Thickness(classUnit.NowPosiLeft.X, classUnit.NowPosiLeft.Y, 0, 0);
                                            }
                                        }
                                        classUnit.OrderPosiLeft = classUnit.NowPosiLeft;
                                        classUnit.FlagMoving = false;
                                        return;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                //移動中にゲームを落とすとエラーになるので暫定的に
                                throw;
                            }
                        }));
                        //Application.Current.Dispatcher.Invoke終了
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void TaskBattleMoveAsync(CancellationToken cancelToken, ClassGameStatus classGameStatus, Window window)
        {
            Dictionary<long, (Task, CancellationTokenSource)> t = new Dictionary<long, (Task, CancellationTokenSource)>();

            List<ClassHorizontalUnit> listClassHorizontalUnits = new List<ClassHorizontalUnit>();
            switch (classGameStatus.ClassBattle.BattleWhichIsThePlayer)
            {
                case BattleWhichIsThePlayer.Sortie:
                    listClassHorizontalUnits = classGameStatus.ClassBattle.SortieUnitGroup;
                    break;
                case BattleWhichIsThePlayer.Def:
                    listClassHorizontalUnits = classGameStatus.ClassBattle.DefUnitGroup;
                    break;
                case BattleWhichIsThePlayer.None:
                    break;
                default:
                    break;
            }

            while (true)
            {
                if (cancelToken.IsCancellationRequested) return;

                Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 1000)));

                foreach (var item in listClassHorizontalUnits)
                {
                    foreach (var itemGroupBy in item.ListClassUnit.Where(x => x.FlagMoving == false))
                    {
                        if (itemGroupBy.NowPosiLeft != itemGroupBy.OrderPosiLeft)
                        {
                            //移動スレッド開始
                            var calc0 = ClassCalcVec.ReturnVecDistance(
                                from: new Point(itemGroupBy.NowPosiLeft.X, itemGroupBy.NowPosiLeft.Y),
                                to: itemGroupBy.OrderPosiLeft
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
                            (Task, CancellationTokenSource) aaa = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveExecuteAsync(itemGroupBy, token, classGameStatus, window)), tokenSource);
                            t.Add(itemGroupBy.ID, aaa);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 格納されたルート情報を基に移動する
        /// </summary>
        /// <param name="cancelToken"></param>
        /// <param name="classGameStatus"></param>
        /// <param name="window"></param>
        public static void TaskBattleMoveAIAsync(CancellationToken cancelToken,
                                                    ClassGameStatus classGameStatus,
                                                    Window window)
        {
            // チェック
            if (classGameStatus.ClassBattle == null) return;
            if (classGameStatus.ClassBattle.ClassMapBattle == null) return;

            Dictionary<long, (Task, CancellationTokenSource)> t = new Dictionary<long, (Task, CancellationTokenSource)>();

            // 移動する陣営決定
            List<ClassHorizontalUnit> listTarget = new List<ClassHorizontalUnit>();
            switch (classGameStatus.ClassBattle.BattleWhichIsThePlayer)
            {
                case BattleWhichIsThePlayer.Sortie:
                    listTarget = classGameStatus.ClassBattle.DefUnitGroup;
                    break;
                case BattleWhichIsThePlayer.Def:
                    listTarget = classGameStatus.ClassBattle.SortieUnitGroup;
                    break;
                case BattleWhichIsThePlayer.None:
                    break;
                default:
                    break;
            }

            List<ClassUnit> list = new List<ClassUnit>();
            foreach (var itemH in listTarget)
            {
                list.AddRange(itemH.ListClassUnit);
            }

            while (true)
            {
                if (cancelToken.IsCancellationRequested) return;

                Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 1000)));

                try
                {
                    foreach (var item in classGameStatus.AiRoot)
                    {
                        try
                        {
                            foreach (var moveUnit in list.Where(x => x.ID == item.Key && x.FlagMoving == false))
                            {
                                foreach (var itemNext in item.Value)
                                {
                                    while (true)
                                    {
                                        if (moveUnit.FlagMoving == true)
                                        {
                                            if (cancelToken.IsCancellationRequested) return;

                                            Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 1000)));
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    //移動スレッド開始
                                    var aaaaa = classGameStatus.ClassBattle.ClassMapBattle.MapData[(int)itemNext.X][(int)itemNext.Y].MapPath;
                                    if (aaaaa == null) return;
                                    if (Application.Current == null)
                                    {
                                        Environment.Exit(1);
                                    }
                                    Application.Current.Dispatcher.Invoke((Action)(() =>
                                    {
                                        moveUnit.OrderPosiLeft = new Point(aaaaa.Margin.Left, aaaaa.Margin.Top);
                                    }));
                                    var calc0 = ClassCalcVec.ReturnVecDistance(
                                        from: new Point(moveUnit.NowPosiLeft.X, moveUnit.NowPosiLeft.Y),
                                        to: moveUnit.OrderPosiLeft
                                    );
                                    moveUnit.VecMove = ClassCalcVec.ReturnNormalize(calc0);
                                    moveUnit.FlagMoving = true;
                                    if (t.TryGetValue(moveUnit.ID, out (Task, CancellationTokenSource) value))
                                    {
                                        if (value.Item1 != null)
                                        {
                                            value.Item2.Cancel();
                                            t.Remove(moveUnit.ID);
                                        }
                                    }
                                    var tokenSource = new CancellationTokenSource();
                                    var token = tokenSource.Token;
                                    (Task, CancellationTokenSource) aaa =
                                        new(Task.Run(() => ClassStaticBattle.TaskBattleMoveExecuteAsync(moveUnit, token, classGameStatus, window)), tokenSource);
                                    t.Add(moveUnit.ID, aaa);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        #region アスターアルゴリズム関係

        #region TaskBattleMoveAIAsync
        /// <summary>
        /// アスターアルゴリズムでルート情報を格納
        /// </summary>
        /// <param name="cancelToken"></param>
        /// <param name="classGameStatus"></param>
        /// <param name="window"></param>
        public static void TaskBattleMoveAStar(CancellationToken cancelToken,
                                                        ClassGameStatus classGameStatus,
                                                        Window window,
                                                        Canvas canvasMain)
        {
            // チェック
            if (classGameStatus.ClassBattle == null) return;
            if (classGameStatus.ClassBattle.ClassMapBattle == null) return;

            Dictionary<long, (Task, CancellationTokenSource)> t = new Dictionary<long, (Task, CancellationTokenSource)>();

            // 移動する陣営決定
            List<ClassHorizontalUnit> listTarget = new List<ClassHorizontalUnit>();
            List<ClassHorizontalUnit> listEnemy = new List<ClassHorizontalUnit>();
            switch (classGameStatus.ClassBattle.BattleWhichIsThePlayer)
            {
                case BattleWhichIsThePlayer.Sortie:
                    listTarget = classGameStatus.ClassBattle.DefUnitGroup;
                    listEnemy = classGameStatus.ClassBattle.SortieUnitGroup;
                    break;
                case BattleWhichIsThePlayer.Def:
                    listTarget = classGameStatus.ClassBattle.SortieUnitGroup;
                    listEnemy = classGameStatus.ClassBattle.DefUnitGroup;
                    break;
                case BattleWhichIsThePlayer.None:
                    break;
                default:
                    break;
            }

            //アスターアルゴリズムの為
            List<Path> listPath = new List<Path>(); ;
            if (Application.Current == null)
            {
                Environment.Exit(1);
            }

            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(canvasMain, StringName.windowMapBattle);
                if (re1 == null) return;

                listPath = re1.Children.OfType<Path>().ToList();
            }));

            int counter = 180;
            while (true)
            {
                if (cancelToken.IsCancellationRequested) return;

                Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 1000)));

                try
                {
                    foreach (var itemTarget in listTarget.Where(x => x.FlagBuilding == false))
                    {
                        try
                        {
                            foreach (var itemListClassUnit in itemTarget.ListClassUnit.Where(x => x.FlagMoving == false))
                            {
                                if (counter >= 180)
                                {
                                    var listRoot = new List<Point>();

                                    counter = 1;
                                    ////アスターアルゴリズムで移動経路取得
                                    //まず現在のマップチップを取得
                                    int rowT = -1;
                                    int colT = -1;

                                    if (Application.Current == null) Environment.Exit(1);
                                    Application.Current.Dispatcher.Invoke((Action)(() =>
                                    {
                                        var initMapTip = listPath.Where(x => (x.Margin.Left + (ClassStaticBattle.yokoMapTip)) >= itemListClassUnit.NowPosiCenter.X
                                                                        && (x.Margin.Left) <= (itemListClassUnit.NowPosiCenter.X));

                                        if (initMapTip.Count() <= 0) return;

                                        var initMapTipA = initMapTip.Where(y => (y.Margin.Top + (ClassStaticBattle.TakasaMapTip)) >= (itemListClassUnit.NowPosiCenter.Y)
                                                                            && (y.Margin.Top) <= ((itemListClassUnit.NowPosiCenter.Y)))
                                                                    .FirstOrDefault();

                                        if (initMapTipA == null) return;

                                        foreach (var itemR in classGameStatus.ClassBattle.ClassMapBattle.MapData
                                                                .Select((value, index) => (value, index)))
                                        {
                                            foreach (var itemC in itemR.value
                                                                .Select((value, index) => (value, index)))
                                            {
                                                if (itemC.value.MapPath == null) continue;
                                                if (itemC.value.MapPath.Name == initMapTipA.Name)
                                                {
                                                    rowT = itemR.index;
                                                    colT = itemC.index;
                                                    break;
                                                }
                                            }
                                        }
                                        //initMapTip.Stroke = Brushes.Blue;
                                        //initMapTip.StrokeThickness = 10;
                                        //classGameStatus.ClassBattle.ClassMapBattle.MapData[row][col].MapPath.Stroke = Brushes.Yellow;
                                        //classGameStatus.ClassBattle.ClassMapBattle.MapData[row][col].MapPath.StrokeThickness = 10;
                                    }));

                                    if (rowT == -1) continue;
                                    if (colT == -1) continue;

                                    //最寄りの敵のマップチップを取得
                                    Point xy1 = itemListClassUnit.NowPosiCenter;
                                    xy1.X = xy1.X * xy1.X;
                                    xy1.Y = xy1.Y * xy1.Y;
                                    double disA = xy1.X + xy1.Y;
                                    Dictionary<ClassUnit, double> dicDis = new Dictionary<ClassUnit, double>();
                                    foreach (var itemEnemy in listEnemy)
                                    {
                                        foreach (var itemEnemyListClassUnit in itemEnemy.ListClassUnit)
                                        {
                                            Point xy2 = itemEnemyListClassUnit.NowPosiCenter;
                                            xy2.X = xy2.X * xy2.X;
                                            xy2.Y = xy2.Y * xy2.Y;
                                            double disB = xy2.X + xy2.Y;
                                            dicDis.Add(itemEnemyListClassUnit, disA - disB);
                                        }
                                    }
                                    var minValue = dicDis.Values.Min();
                                    var minElem = dicDis.FirstOrDefault(x => x.Value == minValue);

                                    //最寄りの敵のマップチップを取得
                                    int rowE = -1;
                                    int colE = -1;
                                    if (Application.Current == null) Environment.Exit(1);
                                    Application.Current.Dispatcher.Invoke((Action)(() =>
                                    {
                                        var initMapTip = listPath.Where(x => (x.Margin.Left + (ClassStaticBattle.yokoMapTip)) >= minElem.Key.NowPosiCenter.X
                                                                        && (x.Margin.Left) <= (minElem.Key.NowPosiCenter.X));

                                        if (initMapTip.Count() <= 0) return;

                                        var initMapTipA = initMapTip.Where(y => (y.Margin.Top + (ClassStaticBattle.TakasaMapTip)) >= (minElem.Key.NowPosiCenter.Y)
                                                                            && (y.Margin.Top) <= ((minElem.Key.NowPosiCenter.Y)))
                                                                    .FirstOrDefault();

                                        if (initMapTipA == null) return;

                                        foreach (var itemR in classGameStatus.ClassBattle.ClassMapBattle.MapData
                                                                .Select((value, index) => (value, index)))
                                        {
                                            foreach (var itemC in itemR.value
                                                                .Select((value, index) => (value, index)))
                                            {
                                                if (itemC.value.MapPath == null) continue;
                                                if (itemC.value.MapPath.Name == initMapTipA.Name)
                                                {
                                                    rowE = itemR.index;
                                                    colE = itemC.index;
                                                    break;
                                                }
                                            }
                                        }
                                    }));
                                    if (rowE == -1) continue;
                                    if (colE == -1) continue;

                                    List<List<ClassAStar>> MapO = new List<List<ClassAStar>>();
                                    foreach (var itemMapData in classGameStatus.ClassBattle.ClassMapBattle.MapData.Select((value, index) => (value, index)))
                                    {
                                        MapO.Add(new List<ClassAStar>());
                                        foreach (var itemDetail in itemMapData.value.Select((value, index) => (value, index)))
                                        {
                                            MapO[MapO.Count - 1].Add(new ClassAStar(itemMapData.index, itemDetail.index));
                                        }
                                    }

                                    ////現在地を開く
                                    ClassAStarManager classAStarManager = new ClassAStarManager(rowE, colE);
                                    ClassAStar? startAstar = classAStarManager.OpenOne(rowT, colT, 0, null);
                                    if (classAStarManager.ListClassAStar != null)
                                    {
                                        if (startAstar != null)
                                        {
                                            classAStarManager.ListClassAStar.Add(startAstar);
                                        }
                                    }

                                    ////移動経路取得
                                    while (true)
                                    {
                                        if (startAstar == null)
                                        {
                                            listRoot.Clear();
                                            break;
                                        }
                                        classAStarManager.RemoveClassAStar(startAstar);
                                        classAStarManager.OpenAround(startAstar, classGameStatus.ClassBattle.ClassMapBattle.MapData, classGameStatus, canvasMain);

                                        if (classAStarManager.ListClassAStar != null)
                                        {
                                            startAstar = SearchMinScore(classAStarManager.ListClassAStar);
                                            #region 開封範囲を見たい時用に
                                            //if (startAstar != null)
                                            //{
                                            //    Application.Current.Dispatcher.Invoke((Action)(() =>
                                            //    {
                                            //        var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(canvasMain, StringName.windowMapBattle);

                                            //        Canvas canvas = new Canvas();
                                            //        canvas.Background = Brushes.Blue;
                                            //        canvas.Height = TakasaMapTip;
                                            //        canvas.Width = yokoMapTip;
                                            //        var aaaaa = classGameStatus.ClassBattle.ClassMapBattle.MapData[startAstar.Row][startAstar.Col].MapPath;
                                            //        canvas.Margin = new Thickness()
                                            //        {
                                            //            Left = aaaaa.Margin.Left,
                                            //            Top = aaaaa.Margin.Top
                                            //        };
                                            //        re1.Children.Add(canvas);
                                            //    }));
                                            //}
                                            #endregion
                                        }

                                        if (startAstar == null)
                                        {
                                            continue;
                                        }

                                        if (startAstar.Row == classAStarManager.EndX && startAstar.Col == classAStarManager.EndY)
                                        {
                                            startAstar.GetRoot(listRoot);
                                            listRoot.Reverse();
                                            break;
                                        }
                                    }

                                    #region 移動経路を見たい時用に
                                    if (listRoot.Count != 0)
                                    {
                                        foreach (var itemResultAStarRev in listRoot)
                                        {
                                            Application.Current.Dispatcher.Invoke((Action)(() =>
                                            {
                                                var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(canvasMain, StringName.windowMapBattle);

                                                Canvas canvas = new Canvas();
                                                canvas.Background = Brushes.Yellow;
                                                canvas.Height = TakasaMapTip;
                                                canvas.Width = yokoMapTip;
                                                var aaaaa = classGameStatus.ClassBattle.ClassMapBattle.MapData[(int)itemResultAStarRev.X][(int)itemResultAStarRev.Y].MapPath;
                                                if (aaaaa == null) return;
                                                canvas.Margin = new Thickness()
                                                {
                                                    Left = aaaaa.Margin.Left,
                                                    Top = aaaaa.Margin.Top
                                                };
                                                re1.Children.Add(canvas);
                                            }));
                                        }
                                    }
                                    #endregion

                                    if (listRoot.Count != 0)
                                    {
                                        if (classGameStatus.AiRoot.TryAdd(itemListClassUnit.ID, listRoot) == false)
                                        {
                                            classGameStatus.AiRoot[itemListClassUnit.ID] = listRoot;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //コレクションに変更があった時
                            MessageBox.Show(ex.Message);
                            throw;
                        }
                    }
                }
                catch (Exception)
                {
                    //コレクションに変更があった時
                }

                counter++;
            }
        }
        #endregion

        private static ClassAStar? SearchMinScore(List<ClassAStar> ls)
        {
            // 最小スコア
            int min = 99999;
            // 最小コスト
            int minCost = 99999;
            ClassAStar? targetClassAStar = null;
            foreach (ClassAStar itemClassAStar in ls)
            {
                int score = itemClassAStar.Cost + itemClassAStar.HCost;
                if (score > min)
                {
                    continue;
                }

                // スコアが同じ
                if (score == min)
                {
                    //コストが最小コストより大きい
                    if (itemClassAStar.Cost >= minCost)
                    {
                        continue;
                    }
                }

                // 最小値更新.
                min = score;
                minCost = itemClassAStar.Cost;
                targetClassAStar = itemClassAStar;
            }
            return targetClassAStar;
        }

        #region HeuristicMethod
        /// <summary>
        /// エースターアルゴリズムで使用
        /// </summary>
        /// <param name="nowX"></param>
        /// <param name="nowY"></param>
        /// <param name="targetX"></param>
        /// <param name="targetY"></param>
        /// <returns></returns>
        public static int HeuristicMethod(int nowX, int nowY, int targetX, int targetY)
        {
            var x = Math.Abs(nowX - targetX);
            var y = Math.Abs(nowX - targetX);
            if (x > y)
            {
                return x;
            }
            else
            {
                return y;
            }
        }
        #endregion

        #endregion

        #endregion

        #region スキル関係

        public static void TaskBattleSkillExecuteAsync(ClassUnit classUnit,
                                                            ClassUnit classUnitDef,
                                                            ClassSkill classSkill,
                                                            ClassGameStatus classGameStatus,
                                                            Canvas canvasMain,
                                                            int dicKey)
        {
            List<ClassHorizontalUnit> listTarget = new List<ClassHorizontalUnit>();
            switch (classGameStatus.ClassBattle.BattleWhichIsThePlayer)
            {
                case BattleWhichIsThePlayer.Sortie:
                    listTarget = classGameStatus.ClassBattle.DefUnitGroup;
                    break;
                case BattleWhichIsThePlayer.Def:
                    listTarget = classGameStatus.ClassBattle.SortieUnitGroup;
                    break;
                case BattleWhichIsThePlayer.None:
                    break;
                default:
                    break;
            }

            while (true)
            {
                Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 1000)));

                ClassVec classVec = new ClassVec();
                classUnit.OrderPosiSkill.TryGetValue(dicKey, out Point resultTryOrd);
                classVec.Target = resultTryOrd;
                classUnit.VecMoveSkill.TryGetValue(dicKey, out Point resultTryVec);
                classVec.Vec = resultTryVec;
                classVec.Speed = classSkill.Speed;
                if (classUnit.NowPosiSkill.TryGetValue(dicKey, out Point point) == false)
                {
                    return;
                }
                if (classVec.Hit(point) == true)
                {
                    classUnit.FlagMovingSkill = false;

                    if (Application.Current == null)
                    {
                        Environment.Exit(1);
                    }

                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(canvasMain, StringName.windowMapBattle);
                        if (re1 == null) return;
                        var re2 = (Canvas)LogicalTreeHelper.FindLogicalNode(re1, "skillEffect" + classUnit.ID.ToString() + dicKey);
                        if (re2 != null) re1.Children.Remove(re2);
                        var re3 = (Line)LogicalTreeHelper.FindLogicalNode(re1, "skillEffectRay" + classUnit.ID.ToString() + dicKey);
                        if (re3 != null) re1.Children.Remove(re3);
                    }));

                    //体力計算処理
                    try
                    {
                        foreach (var item in listTarget)
                        {
                            var re = item.ListClassUnit.Where(x => x.NowPosiCenter.X <= classVec.Target.X + classSkill.RandomSpace
                                                        && x.NowPosiCenter.X >= classVec.Target.X - classSkill.RandomSpace
                                                        && x.NowPosiCenter.Y <= classVec.Target.Y + classSkill.RandomSpace
                                                        && x.NowPosiCenter.Y >= classVec.Target.Y - classSkill.RandomSpace);
                            try
                            {
                                foreach (var itemRe in re)
                                {
                                    itemRe.Hp = (int)(itemRe.Hp - (Math.Floor((classSkill.Str.Item2 * 0.1) * classUnit.Attack)));
                                    if (itemRe.Hp <= 0)
                                    {
                                        if (Application.Current == null) Environment.Exit(1);

                                        Application.Current.Dispatcher.Invoke((Action)(() =>
                                        {
                                            if (itemRe is ClassUnitBuilding building)
                                            {
                                                //建築物破壊
                                                item.ListClassUnit.Remove(building);

                                                var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(canvasMain, StringName.windowMapBattle);
                                                if (re1 == null) Environment.Exit(1);
                                                var re2 = (Rectangle)LogicalTreeHelper.FindLogicalNode(re1, "Bui" + building.X + "a" + building.Y);
                                                if (re2 == null) throw new Exception();

                                                re1.Children.Remove(re2);
                                                classGameStatus.ClassBattle.ListBuildingAlive.Remove(re2);
                                            }
                                            else
                                            {
                                                //通常ユニット破壊
                                                item.ListClassUnit.Remove(itemRe);

                                                var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(canvasMain, StringName.windowMapBattle);
                                                if (re1 == null) Environment.Exit(1);
                                                var re2 = (Border)LogicalTreeHelper.FindLogicalNode(re1, "border" + itemRe.ID.ToString());
                                                if (re2 != null) re1.Children.Remove(re2);

                                            }
                                        }));
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                throw;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    //体力計算処理終了

                    classUnit.OrderPosiSkill.Remove(dicKey);
                    classUnit.VecMoveSkill.Remove(dicKey);
                    classUnit.NowPosiSkill.Remove(dicKey);

                    return;
                }
                else
                {
                    //ベクトルが何故か0になったらちょっと増やす
                    if (classUnit.VecMoveSkill[dicKey].X == 0 && classUnit.VecMoveSkill[dicKey].Y == 0)
                    {
                        classUnit.VecMoveSkill[dicKey] = new Point() { X = 0.5, Y = 0.5 };
                    }
                    //移動量増やす
                    if (classUnit.NowPosiSkill.TryGetValue(dicKey, out point) == false)
                    {
                        return;
                    }

                    classUnit.NowPosiSkill[dicKey] = new Point()
                    {
                        X = classUnit.NowPosiSkill[dicKey].X + (classUnit.VecMoveSkill[dicKey].X * (classSkill.Speed / 100)),
                        Y = classUnit.NowPosiSkill[dicKey].Y + (classUnit.VecMoveSkill[dicKey].Y * (classSkill.Speed / 100))
                    };

                    if (Application.Current == null)
                    {
                        Environment.Exit(1);
                    }

                    Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(canvasMain, StringName.windowMapBattle);
                        if (re1 == null) return;
                        var re2 = (Canvas)LogicalTreeHelper.FindLogicalNode(re1, "skillEffect" + classUnit.ID.ToString() + dicKey);
                        if (re2 == null) return;
                        re2.Margin = new Thickness(classUnit.NowPosiSkill[dicKey].X, classUnit.NowPosiSkill[dicKey].Y, 0, 0);
                        var re3 = (Line)LogicalTreeHelper.FindLogicalNode(re1, "skillEffectRay" + classUnit.ID.ToString() + dicKey);
                        if (re3 == null) return;
                        re3.X2 = classUnit.NowPosiSkill[dicKey].X + (classSkill.W / 2);
                        re3.Y2 = classUnit.NowPosiSkill[dicKey].Y + (classSkill.H / 2);
                    }));
                }
            }
        }

        public static void TaskBattleSkill(CancellationToken token, Canvas canvasMain, ClassGameStatus classGameStatus)
        {
            List<ClassHorizontalUnit> aaa = new List<ClassHorizontalUnit>();
            List<ClassHorizontalUnit> listTarget = new List<ClassHorizontalUnit>();
            switch (classGameStatus.ClassBattle.BattleWhichIsThePlayer)
            {
                case BattleWhichIsThePlayer.Sortie:
                    aaa = classGameStatus.ClassBattle.SortieUnitGroup;
                    listTarget = classGameStatus.ClassBattle.DefUnitGroup;
                    break;
                case BattleWhichIsThePlayer.Def:
                    aaa = classGameStatus.ClassBattle.DefUnitGroup;
                    listTarget = classGameStatus.ClassBattle.SortieUnitGroup;
                    break;
                case BattleWhichIsThePlayer.None:
                    break;
                default:
                    break;
            }

            if (classGameStatus.CommonWindow == null) return;

            string fP = classGameStatus.CommonWindow.GetPathDirectoryGameTitleFullName();
            Random r1 = new System.Random();

            while (true)
            {
                if (token.IsCancellationRequested) return;

                Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 1000)));
                bool flagAttack = false;

                foreach (var item in aaa)
                {
                    //スキル発動中でないユニットを抽出
                    foreach (var itemGroupBy in item.ListClassUnit.Where(x => x.FlagMovingSkill == false))
                    {
                        //スキル優先順位確認
                        foreach (var itemSkill in itemGroupBy.Skill.OrderBy(x => x.SortKey))
                        {
                            //ターゲットとなるユニットを抽出し、
                            //スキル射程範囲を確認
                            var xA = itemGroupBy.NowPosiLeft;
                            foreach (var itemDefUnitGroup in listTarget)
                            {
                                foreach (var itemDefUnitList in itemDefUnitGroup.ListClassUnit)
                                {
                                    //三平方の定理から射程内か確認
                                    {
                                        var xB = itemDefUnitList.NowPosiLeft;
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

                                    int singleAttackNumber = r1.Next();
                                    itemGroupBy.OrderPosiSkill.Clear();

                                    itemGroupBy.NowPosiSkill.Add(singleAttackNumber, itemGroupBy.GetNowPosiCenter());
                                    itemGroupBy.OrderPosiSkill.Add(singleAttackNumber, itemDefUnitList.GetNowPosiCenter());

                                    var calc0 = ClassCalcVec.ReturnVecDistance(
                                                    from: itemGroupBy.NowPosiSkill[singleAttackNumber],
                                                    to: itemGroupBy.OrderPosiSkill[singleAttackNumber]
                                                    );
                                    itemGroupBy.VecMoveSkill.Add(singleAttackNumber, ClassCalcVec.ReturnNormalize(calc0));
                                    itemGroupBy.FlagMovingSkill = true;

                                    //rush数だけ実行する
                                    int rushBase = 1;
                                    if (itemSkill.Rush > 1) rushBase = itemSkill.Rush;

                                    for (int iii = 0; iii < rushBase; iii++)
                                    {
                                        //rush_random_degree分、ずらす
                                        if (itemSkill.RushRandomDegree > 1)
                                        {
                                            Random rand = new System.Random();
                                            int degree = rand.Next(-itemSkill.RushRandomDegree, itemSkill.RushRandomDegree + 1);
                                            //ラジアン ⇒ 度*π/180 を掛ける
                                            double rad = degree * (Math.PI / 180);

                                            //[0]を基準にするのでOK
                                            var caRe = ConvertVecX(rad,
                                                                    itemGroupBy.OrderPosiSkill[singleAttackNumber].X,
                                                                    itemGroupBy.OrderPosiSkill[singleAttackNumber].Y,
                                                                    itemGroupBy.NowPosiCenter.X,
                                                                    itemGroupBy.NowPosiCenter.Y);

                                            itemGroupBy.OrderPosiSkill[singleAttackNumber] = new Point(caRe.Item1, caRe.Item2);
                                            calc0 = ClassCalcVec.ReturnVecDistance(
                                                            from: itemGroupBy.NowPosiSkill[singleAttackNumber],
                                                            to: itemGroupBy.OrderPosiSkill[singleAttackNumber]
                                                            );
                                            itemGroupBy.VecMoveSkill[singleAttackNumber] = ClassCalcVec.ReturnNormalize(calc0);
                                        }

                                        //Image出す
                                        {
                                            if (Application.Current == null)
                                            {
                                                Environment.Exit(1);
                                            }

                                            Application.Current.Dispatcher.Invoke((Action)(() =>
                                            {
                                                //後始末
                                                var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(canvasMain, StringName.windowMapBattle);
                                                if (re1 != null)
                                                {
                                                    var re2 = (Canvas)LogicalTreeHelper.FindLogicalNode(re1, "skillEffect" + itemGroupBy.ID + singleAttackNumber);
                                                    if (re2 != null) re1.Children.Remove(re2);
                                                    var re3 = (Line)LogicalTreeHelper.FindLogicalNode(re1, "skillEffectRay" + itemGroupBy.ID + singleAttackNumber);
                                                    if (re3 != null) re1.Children.Remove(re3);
                                                }
                                                else if (re1 == null)
                                                {
                                                    return;
                                                }

                                                //スキル画像
                                                {
                                                    //二点間の角度を求める
                                                    var radian = MathF.Atan2((float)(itemDefUnitList.NowPosiCenter.Y - itemGroupBy.NowPosiSkill[singleAttackNumber].Y),
                                                                                (float)(itemDefUnitList.NowPosiCenter.X - itemGroupBy.NowPosiSkill[singleAttackNumber].X));
                                                    var degree = radian * (180 / Math.PI);

                                                    List<string> strings = new List<string>();
                                                    strings.Add(fP);
                                                    strings.Add("042_ChipImageSkillEffect");
                                                    if (degree == 0 || degree == 90 || degree == 180 || degree == 270)
                                                    {
                                                        strings.Add(itemSkill.Image + "N.png");
                                                    }
                                                    else
                                                    {
                                                        strings.Add(itemSkill.Image + "NW.png");
                                                    }
                                                    string path = System.IO.Path.Combine(strings.ToArray());

                                                    var bi = new BitmapImage(new Uri(path));
                                                    Image image = new Image();
                                                    image.Stretch = Stretch.Fill;
                                                    image.Source = bi;
                                                    image.Margin = new Thickness(0, 0, 0, 0);
                                                    image.Height = itemSkill.H;
                                                    image.Width = itemSkill.W;
                                                    image.HorizontalAlignment = HorizontalAlignment.Left;
                                                    image.VerticalAlignment = VerticalAlignment.Top;

                                                    Canvas canvas = new Canvas();
                                                    canvas.Background = Brushes.Transparent;
                                                    canvas.Height = itemSkill.H;
                                                    canvas.Width = itemSkill.W;
                                                    canvas.Opacity = (double)itemSkill.A / 255;
                                                    canvas.Margin = new Thickness()
                                                    {
                                                        Left = itemGroupBy.NowPosiSkill[singleAttackNumber].X,
                                                        Top = itemGroupBy.NowPosiSkill[singleAttackNumber].Y
                                                    };
                                                    canvas.Name = "skillEffect" + itemGroupBy.ID + singleAttackNumber;

                                                    RotateTransform rotateTransform2;
                                                    if (degree == 0 || degree == 90 || degree == 180 || degree == 270)
                                                    {
                                                        rotateTransform2 =
                                                            new RotateTransform(degree - 90);
                                                    }
                                                    else
                                                    {
                                                        rotateTransform2 =
                                                            new RotateTransform(degree - 135);
                                                    }
                                                    canvas.RenderTransform = rotateTransform2;
                                                    canvas.RenderTransformOrigin = new Point(0.5, 0.5);

                                                    canvas.Children.Add(image);
                                                    re1.Children.Add(canvas);
                                                }
                                                //ray表示
                                                {
                                                    var alpha = itemSkill.Ray[0];
                                                    SolidColorBrush solidColorBrush =
                                                        new SolidColorBrush(
                                                            Color.FromRgb((byte)itemSkill.Ray[1], (byte)itemSkill.Ray[2], (byte)itemSkill.Ray[3]));
                                                    Line line = new Line();
                                                    line.Opacity = (double)alpha / 255;
                                                    line.Fill = solidColorBrush;
                                                    line.Stroke = solidColorBrush;
                                                    line.StrokeThickness = 3;
                                                    line.Name = "skillEffectRay" + itemGroupBy.ID + singleAttackNumber;
                                                    line.X1 = itemGroupBy.NowPosiSkill[singleAttackNumber].X;
                                                    line.X2 = itemGroupBy.NowPosiSkill[singleAttackNumber].X + (itemSkill.W / 2);
                                                    line.Y1 = itemGroupBy.NowPosiSkill[singleAttackNumber].Y;
                                                    line.Y2 = itemGroupBy.NowPosiSkill[singleAttackNumber].Y + (itemSkill.H / 2);
                                                    line.HorizontalAlignment = HorizontalAlignment.Left;
                                                    line.VerticalAlignment = VerticalAlignment.Top;
                                                    re1.Children.Add(line);
                                                }
                                            }));
                                        }

                                        //スキル発動スレッド開始
                                        var t = Task.Run(() => ClassStaticBattle.TaskBattleSkillExecuteAsync(itemGroupBy, itemDefUnitList, itemSkill, classGameStatus, canvasMain, singleAttackNumber));

                                        //RushInterval分、間隔を保つ
                                        if (rushBase > 1)
                                        {
                                            Thread.Sleep(itemSkill.RushInterval * 100);

                                            //次ラッシュの準備
                                            singleAttackNumber = r1.Next();
                                            itemGroupBy.NowPosiSkill.Add(singleAttackNumber, itemGroupBy.GetNowPosiCenter());
                                            itemGroupBy.OrderPosiSkill.Add(singleAttackNumber, itemDefUnitList.GetNowPosiCenter());

                                            calc0 = ClassCalcVec.ReturnVecDistance(
                                                            from: itemGroupBy.NowPosiSkill[singleAttackNumber],
                                                            to: itemGroupBy.OrderPosiSkill[singleAttackNumber]
                                                            );
                                            itemGroupBy.VecMoveSkill.Add(singleAttackNumber, ClassCalcVec.ReturnNormalize(calc0));
                                            itemGroupBy.FlagMovingSkill = true;
                                        }
                                    }

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
        #endregion

        #region Timer

        #region TimerAction60FPSBattle
        /// <summary>
        /// 戦闘終了処理などを行う
        /// </summary>
        /// <param name="commonWindow"></param>
        /// <param name="classGameStatus"></param>
        /// <param name="action"></param>
        public static void TimerAction60FPSBattle(CommonWindow commonWindow, ClassGameStatus classGameStatus, DelegateMapRenderedFromBattle? action)
        {
            //攻撃側勝利
            {
                bool flgaDefHp = false;
                foreach (var itemDefUnitGroup in classGameStatus.ClassBattle.DefUnitGroup)
                {
                    if (itemDefUnitGroup.FlagBuilding == true)
                    {
                        continue;
                    }
                    if (itemDefUnitGroup.ListClassUnit.Count != 0)
                    {
                        flgaDefHp = true;
                    }
                }

                if (flgaDefHp == false)
                {
                    ////defの負け

                    commonWindow.timerAfterFadeIn.Stop();

                    //タスクキル
                    if (classGameStatus.TaskBattleSkill.Item1 != null)
                    {
                        classGameStatus.TaskBattleSkill.Item2.Cancel();
                    }
                    foreach (var item in classGameStatus.TaskBattleMoveAsync)
                    {
                        if (item.Item1 != null)
                        {
                            item.Item2.Cancel();
                        }
                    }
                    foreach (var item in classGameStatus.TaskBattleMoveDefAsync)
                    {
                        if (item.Item1 != null)
                        {
                            item.Item2.Cancel();
                        }
                    }

                    //画面戻る
                    commonWindow.FadeOut = true;

                    commonWindow.delegateMapRenderedFromBattle = action;

                    commonWindow.FadeIn = true;

                    //部隊所属領地変更
                    {
                        // 出撃先領地と防衛側勢力
                        var spots = Application.Current.Properties["defensePowerAndCity"];
                        if (spots == null)
                        {
                            return;
                        }

                        var convSpots = spots as ClassPowerAndCity;
                        if (convSpots == null)
                        {
                            return;
                        }
                        var targetSpot = convSpots.ClassSpot;
                        var defensePower = convSpots.ClassPower;

                        // 出撃元領地は部隊ごとに異なるけど、勢力は同じなはず。（共同軍や傭兵はどうなる？）
                        // とりあえず、先頭の部隊の所属を参照する
                        string powerNameTag = classGameStatus.ClassBattle.SortieUnitGroup[0].Spot.PowerNameTag;

                        // ワールドマップ領地の所属勢力を変更する
                        var worldMap = classGameStatus.WorldMap;
                        if (worldMap != null)
                        {
                            worldMap.ChangeSpotPower(targetSpot.NameTag, powerNameTag);
                        }

                        // 中立領地なら退却先が無いので一般兵は全て消える。
                        if (defensePower.NameTag == string.Empty)
                        {
                            // 本来は人材かチェックして放浪させないといけない。
                            targetSpot.UnitGroup.Clear();
                        }
                        // 勢力に所属してる場合がややこしい
                        else
                        {
                            // 防衛部隊を削除、又は他都市へ移動。隣接都市が無ければ放浪する。
                            // 戦闘で生き残った一般兵と人材だけ。死亡した一般兵は消える。
                            // とりあえず、全て消してるけど、後で修正すること！
                            targetSpot.UnitGroup.Clear();
                        }

                        // 出撃先の領地を空にした後で、攻撃側の部隊を入れること！
                        // 出撃先に入る数だけ、部隊を移動させる
                        int spot_capacity = targetSpot.Capacity;
                        foreach (var itemTroop in classGameStatus.ClassBattle.SortieUnitGroup)
                        {
                            if (spot_capacity > 0)
                            {
                                // 出撃元から取り除く
                                var srcSpot = itemTroop.Spot;
                                if (srcSpot != null)
                                {
                                    srcSpot.UnitGroup.Remove(itemTroop);
                                }

                                // 出撃先に追加する
                                targetSpot.UnitGroup.Add(itemTroop);
                                itemTroop.Spot = targetSpot;

                                // 空きを減らす
                                spot_capacity--;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    //片付け
                    classGameStatus.ClassBattle.SortieUnitGroup.Clear();
                    classGameStatus.ClassBattle.DefUnitGroup.Clear();
                    classGameStatus.ClassBattle.NeutralUnitGroup.Clear();

                    return;
                }
            }
            //防衛側勝利
            {
                bool flgaAttackHp = false;
                foreach (var itemDefUnitGroup in classGameStatus.ClassBattle.SortieUnitGroup)
                {
                    if (itemDefUnitGroup.ListClassUnit.Count != 0)
                    {
                        flgaAttackHp = true;
                    }
                }

                if (flgaAttackHp == false)
                {
                    ////defの負け

                    commonWindow.timerAfterFadeIn.Stop();

                    //タスクキル
                    if (classGameStatus.TaskBattleSkill.Item1 != null)
                    {
                        classGameStatus.TaskBattleSkill.Item2.Cancel();
                    }
                    foreach (var item in classGameStatus.TaskBattleMoveAsync)
                    {
                        if (item.Item1 != null)
                        {
                            item.Item2.Cancel();
                        }
                    }
                    foreach (var item in classGameStatus.TaskBattleMoveDefAsync)
                    {
                        if (item.Item1 != null)
                        {
                            item.Item2.Cancel();
                        }
                    }

                    //画面戻る
                    commonWindow.FadeOut = true;

                    commonWindow.delegateMapRenderedFromBattle = action;

                    commonWindow.FadeIn = true;

                    //片付け
                    classGameStatus.ClassBattle.SortieUnitGroup.Clear();
                    classGameStatus.ClassBattle.DefUnitGroup.Clear();
                    classGameStatus.ClassBattle.NeutralUnitGroup.Clear();

                    return;
                }
            }
        }
        #endregion

        #region TimerAction60FPSAfterFadeInBattleStart
        /// <summary>
        /// マップ生成後に実行
        /// </summary>
        /// <param name="commonWindow"></param>
        /// <param name="canvasMain"></param>
        /// <exception cref="Exception"></exception>
        public static async void TimerAction60FPSAfterFadeInBattleStart(CommonWindow commonWindow, Canvas canvasMain, DelegateMapRenderedFromBattle? actionMapRenderedFromBattle)
        {
            if (commonWindow.AfterFadeIn == false)
            {
                return;
            }

            //この位置でなければダメ？
            commonWindow.AfterFadeIn = false;
            commonWindow.timerAfterFadeIn.Stop();

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
            commonWindow.timerAfterFadeIn = new DispatcherTimer(DispatcherPriority.Background);
            commonWindow.timerAfterFadeIn.Interval = TimeSpan.FromSeconds((double)1 / 60);
            commonWindow.timerAfterFadeIn.Tick -= (x, s) =>
            {
                TimerAction60FPSAfterFadeInBattleStart(commonWindow, canvasMain, actionMapRenderedFromBattle);
                ClassStaticCommonMethod.KeepInterval(commonWindow.timerAfterFadeIn);
            };
            commonWindow.timerAfterFadeIn.Tick += (x, s) =>
            {
                ClassStaticBattle.TimerAction60FPSBattle(commonWindow, commonWindow.ClassGameStatus, actionMapRenderedFromBattle);
                ClassStaticCommonMethod.KeepInterval(commonWindow.timerAfterFadeIn);
            };
            commonWindow.timerAfterFadeIn.Start();

            Application.Current.DispatcherUnhandledException += OnDispatcherUnhandledException;

            for (int i = 0; i < commonWindow.ClassGameStatus.BattleThread; i++)
            {
                ////スキルスレッド開始
                //出撃ユニット
                {
                    var tokenSource = new CancellationTokenSource();
                    var token = tokenSource.Token;
                    (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleSkill(token, canvasMain, commonWindow.ClassGameStatus)), tokenSource);
                    commonWindow.ClassGameStatus.TaskBattleSkill = a;
                }
                ////防衛ユニット
                //{
                //    var tokenSource = new CancellationTokenSource();
                //    var token = tokenSource.Token;
                //    (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleSkill(token, canvasMain, commonWindow.ClassGameStatus)), tokenSource);
                //    commonWindow.ClassGameStatus.TaskBattleSkillDef = a;
                //}
            }
            ////移動スレッド開始
            switch (commonWindow.ClassGameStatus.ClassBattle.BattleWhichIsThePlayer)
            {
                case BattleWhichIsThePlayer.Sortie:
                    for (int i = 0; i < commonWindow.ClassGameStatus.BattleThread; i++)
                    {
                        //出撃ユニット
                        {
                            var tokenSource = new CancellationTokenSource();
                            var token = tokenSource.Token;
                            (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveAsync(token, commonWindow.ClassGameStatus, commonWindow)), tokenSource);
                            commonWindow.ClassGameStatus.TaskBattleMoveAsync.Add(a);
                        }
                        //防衛(AI)ユニット
                        {
                            var tokenSource = new CancellationTokenSource();
                            var token = tokenSource.Token;
                            (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveAStar(token, commonWindow.ClassGameStatus, commonWindow, canvasMain)), tokenSource);
                            commonWindow.ClassGameStatus.TaskBattleMoveDefAsync.Add(a);
                        }
                        {
                            var tokenSource = new CancellationTokenSource();
                            var token = tokenSource.Token;
                            (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveAIAsync(token, commonWindow.ClassGameStatus, commonWindow)), tokenSource);
                            commonWindow.ClassGameStatus.TaskBattleMoveDefAsync.Add(a);
                        }
                    }
                    break;
                case BattleWhichIsThePlayer.Def:
                    for (int i = 0; i < commonWindow.ClassGameStatus.BattleThread; i++)
                    {
                        //出撃(AI)ユニット
                        {
                            var tokenSource = new CancellationTokenSource();
                            var token = tokenSource.Token;
                            (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveAStar(token, commonWindow.ClassGameStatus, commonWindow, canvasMain)), tokenSource);
                            commonWindow.ClassGameStatus.TaskBattleMoveAsync.Add(a);
                        }
                        //防衛ユニット
                        {
                            var tokenSource = new CancellationTokenSource();
                            var token = tokenSource.Token;
                            (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveAsync(token, commonWindow.ClassGameStatus, commonWindow)), tokenSource);
                            commonWindow.ClassGameStatus.TaskBattleMoveDefAsync.Add(a);
                        }
                        {
                            var tokenSource = new CancellationTokenSource();
                            var token = tokenSource.Token;
                            (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveAIAsync(token, commonWindow.ClassGameStatus, commonWindow)), tokenSource);
                            commonWindow.ClassGameStatus.TaskBattleMoveDefAsync.Add(a);
                        }
                    }
                    break;
                case BattleWhichIsThePlayer.None:
                    for (int i = 0; i < commonWindow.ClassGameStatus.BattleThread; i++)
                    {
                        //出撃(AI)ユニット
                        {
                            var tokenSource = new CancellationTokenSource();
                            var token = tokenSource.Token;
                            (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveAStar(token, commonWindow.ClassGameStatus, commonWindow, canvasMain)), tokenSource);
                            commonWindow.ClassGameStatus.TaskBattleMoveAsync.Add(a);
                        }
                        //防衛(AI)ユニット
                        {
                            var tokenSource = new CancellationTokenSource();
                            var token = tokenSource.Token;
                            (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveAStar(token, commonWindow.ClassGameStatus, commonWindow, canvasMain)), tokenSource);
                            commonWindow.ClassGameStatus.TaskBattleMoveDefAsync.Add(a);
                        }
                        {
                            var tokenSource = new CancellationTokenSource();
                            var token = tokenSource.Token;
                            (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveAIAsync(token, commonWindow.ClassGameStatus, commonWindow)), tokenSource);
                            commonWindow.ClassGameStatus.TaskBattleMoveDefAsync.Add(a);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region SetTimerBattle
        /// <summary>
        /// バトル起点タイマー
        /// </summary>
        /// <param name="test"></param>
        /// <param name="timerAfterFadeIn"></param>
        /// <param name="commonWindow"></param>
        /// <param name="canvas"></param>
        public static void SetTimerBattle(bool test, DispatcherTimer timerAfterFadeIn, CommonWindow commonWindow, Canvas canvas)
        {
            timerAfterFadeIn = new DispatcherTimer(DispatcherPriority.Background);
            timerAfterFadeIn.Interval = TimeSpan.FromSeconds((double)1 / 60);
            if (test == true)
            {
                timerAfterFadeIn.Tick += (x, s) =>
                {
                    ClassStaticBattle.TimerAction60FPSAfterFadeInBattleStart(commonWindow, canvas, null);
                    ClassStaticCommonMethod.KeepInterval(timerAfterFadeIn);
                };
            }
            else
            {
                //後で直す
                timerAfterFadeIn.Tick += (x, s) =>
                {
                    ClassStaticBattle.TimerAction60FPSAfterFadeInBattleStart(commonWindow, canvas, null);
                    ClassStaticCommonMethod.KeepInterval(timerAfterFadeIn);
                };
            }
            commonWindow.AfterFadeIn = true;
            timerAfterFadeIn.Start();
        }
        #endregion


        #endregion

        /// <summary>
        /// 発動するところを見たことが無い
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Environment.Exit(1);
        }

    }
}
