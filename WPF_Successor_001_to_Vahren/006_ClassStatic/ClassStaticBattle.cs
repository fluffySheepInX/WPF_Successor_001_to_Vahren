using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using WPF_Successor_001_to_Vahren._005_Class;
using WPF_Successor_001_to_Vahren._010_Enum;
using System.Threading;
using System.Security.Cryptography.Xml;
using System.Diagnostics;
using System.Windows.Documents;
using System.Runtime.CompilerServices;
using static WPF_Successor_001_to_Vahren.CommonWindow;
using static System.Net.Mime.MediaTypeNames;
using Image = System.Windows.Controls.Image;
using Application = System.Windows.Application;

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

        #region 移動関係

        public static async Task TaskBattleMoveExecuteAsync(ClassUnit classUnit, CancellationToken token, ClassGameStatus classGameStatus, Window window)
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

                        await Task.Run(() =>
                        {
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
                        });
                        //await Task.Run終了
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

        #region アスターアルゴリズムで移動
        /// <summary>
        /// アスターアルゴリズムで移動
        /// </summary>
        /// <param name="cancelToken"></param>
        /// <param name="classGameStatus"></param>
        /// <param name="window"></param>
        public static async void TaskBattleMoveAIAsync(CancellationToken cancelToken, ClassGameStatus classGameStatus, Window window, Canvas canvasMain)
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
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(canvasMain, StringName.windowMapBattle);
                    if (re1 == null) return;

                    listPath = re1.Children.OfType<Path>().ToList();
                }));
            });
            int counter = 1;
            while (true)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                //Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 100000)));
                Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 1000)));

                try
                {
                    foreach (var item in listTarget)
                    {
                        try
                        {
                            foreach (var itemGroupBy in item.ListClassUnit.Where(x => x.FlagMoving == false))
                            {
                                var listRoot = new List<Point>();
                                if (counter >= 180)
                                {
                                    counter = 1;
                                    ////アスターアルゴリズムで移動経路取得
                                    //まず現在のマップチップを取得
                                    int rowT = -1;
                                    int colT = -1;
                                    await Task.Run(() =>
                                    {
                                        Application.Current.Dispatcher.Invoke((Action)(() =>
                                        {
                                            var initMapTip = listPath.Where(x => (x.Margin.Left + (ClassStaticBattle.yokoUnit / 2)) < itemGroupBy.NowPosiCenter.X
                                                                            && (x.Margin.Left + (ClassStaticBattle.yokoUnit / 2)) > (itemGroupBy.NowPosiCenter.X - yokoMapTip))
                                                                        .Where(y => y.Margin.Top < (itemGroupBy.NowPosiCenter.Y)
                                                                                && y.Margin.Top > ((itemGroupBy.NowPosiCenter.Y - TakasaMapTip)))
                                                                        .FirstOrDefault();

                                            if (initMapTip == null) return;

                                            foreach (var itemR in classGameStatus.ClassBattle.ClassMapBattle.MapData
                                                                    .Select((value, index) => (value, index)))
                                            {
                                                foreach (var itemC in itemR.value
                                                                    .Select((value, index) => (value, index)))
                                                {
                                                    if (itemC.value.MapPath == null) continue;
                                                    if (itemC.value.MapPath.Name == initMapTip.Name)
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
                                    });
                                    if (rowT == -1) continue;
                                    if (colT == -1) continue;

                                    //最寄りの敵のマップチップを取得
                                    Point xy1 = itemGroupBy.NowPosiCenter;
                                    xy1.X = xy1.X * xy1.X;
                                    xy1.Y = xy1.Y * xy1.Y;
                                    double disA = xy1.X + xy1.Y;
                                    Dictionary<ClassUnit, double> dicDis = new Dictionary<ClassUnit, double>();
                                    foreach (var itemEnemy in listEnemy)
                                    {
                                        foreach (var itemListClassUnit in itemEnemy.ListClassUnit)
                                        {
                                            Point xy2 = itemListClassUnit.NowPosiCenter;
                                            xy2.X = xy2.X * xy2.X;
                                            xy2.Y = xy2.Y * xy2.Y;
                                            double disB = xy2.X + xy2.Y;
                                            dicDis.Add(itemListClassUnit, disA - disB);
                                        }
                                    }
                                    var minValue = dicDis.Values.Min();
                                    var minElem = dicDis.FirstOrDefault(x => x.Value == minValue);

                                    //最寄りの敵のマップチップを取得
                                    int rowE = -1;
                                    int colE = -1;
                                    await Task.Run(() =>
                                    {
                                        Application.Current.Dispatcher.Invoke((Action)(() =>
                                        {
                                            var initMapTip = listPath.Where(x => (x.Margin.Left + (ClassStaticBattle.yokoUnit / 2)) < minElem.Key.NowPosiCenter.X
                                                                            && (x.Margin.Left + (ClassStaticBattle.yokoUnit / 2)) > (minElem.Key.NowPosiCenter.X - yokoMapTip))
                                                                        .Where(y => y.Margin.Top < (minElem.Key.NowPosiCenter.Y)
                                                                                && y.Margin.Top > ((minElem.Key.NowPosiCenter.Y - TakasaMapTip)))
                                                                        .FirstOrDefault();

                                            if (initMapTip == null) throw new Exception();

                                            foreach (var itemR in classGameStatus.ClassBattle.ClassMapBattle.MapData
                                                                    .Select((value, index) => (value, index)))
                                            {
                                                foreach (var itemC in itemR.value
                                                                    .Select((value, index) => (value, index)))
                                                {
                                                    if (itemC.value.MapPath == null) continue;
                                                    if (itemC.value.MapPath.Name == initMapTip.Name)
                                                    {
                                                        rowE = itemR.index;
                                                        colE = itemC.index;
                                                        break;
                                                    }
                                                }
                                            }
                                        }));
                                    });
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
                                            continue;
                                        }
                                        classAStarManager.RemoveClassAStar(startAstar);
                                        classAStarManager.OpenAround(startAstar, classGameStatus.ClassBattle.ClassMapBattle.MapData, classGameStatus);

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
                                }

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
                                (Task, CancellationTokenSource) aaa =
                                    new(Task.Run(() => ClassStaticBattle.TaskBattleMoveExecuteAsync(itemGroupBy, token, classGameStatus, window)), tokenSource);
                                t.Add(itemGroupBy.ID, aaa);
                            }
                        }
                        catch (Exception)
                        {
                            //コレクションに変更があった時
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
        #endregion

        #region スキル関係

        public static async Task TaskBattleSkillExecuteAsync(ClassUnit classUnit,
                                                            ClassUnit classUnitDef,
                                                            ClassSkill classSkill,
                                                            ClassGameStatus classGameStatus,
                                                            Canvas canvasMain)
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
                classVec.Target = classUnit.OrderPosiSkill;
                classVec.Vec = classUnit.VecMoveSkill;
                classVec.Speed = classSkill.Speed;

                if (classVec.Hit(classUnit.NowPosiSkill))
                {
                    classUnit.FlagMovingSkill = false;
                    await Task.Run(() =>
                    {
                        Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(canvasMain, StringName.windowMapBattle);
                            if (re1 == null) return;
                            var re2 = (Canvas)LogicalTreeHelper.FindLogicalNode(re1, "skillEffect" + classUnit.ID.ToString());
                            if (re2 == null) return;
                            re1.Children.Remove(re2);
                            var re3 = (Line)LogicalTreeHelper.FindLogicalNode(re1, "skillEffectRay" + classUnit.ID.ToString());
                            if (re3 == null) return;
                            re1.Children.Remove(re3);
                        }));
                    });

                    //体力計算処理
                    foreach (var item in listTarget)
                    {
                        var re = item.ListClassUnit.Where(x => x.NowPosiCenter.X <= classVec.Target.X + classSkill.RandomSpace
                                                    && x.NowPosiCenter.X >= classVec.Target.X - classSkill.RandomSpace
                                                    && x.NowPosiCenter.Y <= classVec.Target.Y + classSkill.RandomSpace
                                                    && x.NowPosiCenter.Y >= classVec.Target.Y - classSkill.RandomSpace);

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
                                        //通常ユニット破壊
                                        {
                                            var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(canvasMain, StringName.windowMapBattle);
                                            if (re1 != null)
                                            {
                                                var re2 = (Border)LogicalTreeHelper.FindLogicalNode(re1, "border" + itemRe.ID.ToString());
                                                if (re2 != null)
                                                {
                                                    re1.Children.Remove(re2);
                                                }
                                            }
                                        }
                                        //建築物破壊
                                        if (itemRe is ClassUnitBuilding building)
                                        {
                                            var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(canvasMain, StringName.windowMapBattle);
                                            if (re1 != null)
                                            {
                                                var re2 = (Rectangle)LogicalTreeHelper.FindLogicalNode(re1, "Bui" + building.X + "a" + building.Y);
                                                if (re2 != null)
                                                {
                                                    re1.Children.Remove(re2);
                                                    classGameStatus.ClassBattle.ListBuildingAlive.Remove(re2);
                                                }
                                            }
                                        }
                                    }));
                                });
                            }
                        }
                    }
                    //体力計算処理終了

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
                        X = classUnit.NowPosiSkill.X + (classUnit.VecMoveSkill.X * (classSkill.Speed / 100)),
                        Y = classUnit.NowPosiSkill.Y + (classUnit.VecMoveSkill.Y * (classSkill.Speed / 100))
                    };
                    await Task.Run(() =>
                    {
                        try
                        {
                            Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(canvasMain, StringName.windowMapBattle);
                                if (re1 == null) return;
                                var re2 = (Canvas)LogicalTreeHelper.FindLogicalNode(re1, "skillEffect" + classUnit.ID.ToString());
                                if (re2 == null) return;
                                re2.Margin = new Thickness(classUnit.NowPosiSkill.X, classUnit.NowPosiSkill.Y, 0, 0);
                                var re3 = (Line)LogicalTreeHelper.FindLogicalNode(re1, "skillEffectRay" + classUnit.ID.ToString());
                                if (re3 == null) return;
                                re3.X2 = classUnit.NowPosiSkill.X + (classSkill.W / 2);
                                re3.Y2 = classUnit.NowPosiSkill.Y + (classSkill.H / 2);
                            }));
                        }
                        catch (Exception)
                        {
                            //攻撃中にゲームを落とすとエラーになるので暫定的に
                            //throw;
                        }
                    });
                }
            }
        }

        public static void TaskBattleSkill(CancellationToken token, Canvas canvasMain, ClassGameStatus classGameStatus)
        {
            List<ClassHorizontalUnit> aaa = new List<ClassHorizontalUnit>();
            List<ClassHorizontalUnit> bbb = new List<ClassHorizontalUnit>();
            switch (classGameStatus.ClassBattle.BattleWhichIsThePlayer)
            {
                case BattleWhichIsThePlayer.Sortie:
                    aaa = classGameStatus.ClassBattle.SortieUnitGroup;
                    bbb = classGameStatus.ClassBattle.DefUnitGroup;
                    break;
                case BattleWhichIsThePlayer.Def:
                    aaa = classGameStatus.ClassBattle.DefUnitGroup;
                    bbb = classGameStatus.ClassBattle.SortieUnitGroup;
                    break;
                case BattleWhichIsThePlayer.None:
                    break;
                default:
                    break;
            }
            if (classGameStatus.CommonWindow == null)
            {
                return;
            }
            string fP = classGameStatus.CommonWindow.GetPathDirectoryGameTitleFullName();

            while (true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 1000)));
                bool flagAttack = false;

                foreach (var item in aaa)
                {
                    foreach (var itemGroupBy in item.ListClassUnit.Where(x => x.FlagMovingSkill == false))
                    {
                        //スキル優先順位確認
                        foreach (var itemSkill in itemGroupBy.Skill.OrderBy(x => x.SortKey))
                        {
                            //スキル射程範囲確認
                            var xA = itemGroupBy.NowPosiLeft;
                            foreach (var itemDefUnitGroup in bbb)
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

                                    itemGroupBy.NowPosiSkill = itemGroupBy.GetNowPosiCenter();
                                    itemGroupBy.OrderPosiSkill = itemDefUnitList.GetNowPosiCenter();
                                    var calc0 = ClassCalcVec.ReturnVecDistance(
                                                    from: itemGroupBy.NowPosiSkill,
                                                    to: itemGroupBy.OrderPosiSkill
                                                    );
                                    itemGroupBy.VecMoveSkill = ClassCalcVec.ReturnNormalize(calc0);
                                    itemGroupBy.FlagMovingSkill = true;

                                    //Image出す
                                    {
                                        Application.Current.Dispatcher.Invoke((Action)(() =>
                                        {
                                            var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(canvasMain, StringName.windowMapBattle);
                                            var re2 = (Canvas)LogicalTreeHelper.FindLogicalNode(re1, "skillEffect" + itemGroupBy.ID);
                                            if (re2 != null)
                                            {
                                                re1.Children.Remove(re2);
                                            }
                                            var re3 = (Line)LogicalTreeHelper.FindLogicalNode(re1, "skillEffectRay" + itemGroupBy.ID);
                                            if (re3 != null)
                                            {
                                                re1.Children.Remove(re3);
                                            }

                                            //スキル画像
                                            {
                                                //二点間の角度を求める
                                                var radian = MathF.Atan2((float)(itemDefUnitList.NowPosiLeft.Y - itemGroupBy.NowPosiSkill.Y),
                                                                            (float)(itemDefUnitList.NowPosiLeft.X - itemGroupBy.NowPosiSkill.X));
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
                                                    Left = itemGroupBy.NowPosiSkill.X,
                                                    Top = itemGroupBy.NowPosiSkill.Y
                                                };
                                                canvas.Name = "skillEffect" + itemGroupBy.ID;

                                                RotateTransform rotateTransform2 =
                                                    new RotateTransform(degree + 90);
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
                                                line.Name = "skillEffectRay" + itemGroupBy.ID;
                                                line.X1 = itemGroupBy.NowPosiSkill.X;
                                                line.X2 = itemGroupBy.NowPosiSkill.X + (itemSkill.W / 2);
                                                line.Y1 = itemGroupBy.NowPosiSkill.Y;
                                                line.Y2 = itemGroupBy.NowPosiSkill.Y + (itemSkill.H / 2);
                                                line.HorizontalAlignment = HorizontalAlignment.Left;
                                                line.VerticalAlignment = VerticalAlignment.Top;
                                                re1.Children.Add(line);
                                            }
                                        }));
                                    }

                                    //スキル発動スレッド開始
                                    var t = Task.Run(() => ClassStaticBattle.TaskBattleSkillExecuteAsync(itemGroupBy, itemDefUnitList, itemSkill, classGameStatus, canvasMain));
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

        #region Timer
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
                    if (classGameStatus.TaskBattleMoveAsync.Item1 != null)
                    {
                        classGameStatus.TaskBattleMoveAsync.Item2.Cancel();
                    }
                    if (classGameStatus.TaskBattleMoveDefAsync.Item1 != null)
                    {
                        classGameStatus.TaskBattleMoveDefAsync.Item2.Cancel();
                    }

                    //画面戻る
                    commonWindow.FadeOut = true;

                    commonWindow.delegateMapRenderedFromBattle = action;

                    commonWindow.FadeIn = true;

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
                        var targetSpot = classGameStatus.AllListSpot
                                .Where(x => x.NameTag == convSpots.ClassSpot.NameTag)
                                .First();

                        // 他の勢力に所属してた場合は、取り除く
                        if (convSpots.ClassSpot.PowerNameTag != string.Empty)
                        {
                            convSpots.ClassPower.ListMember.Remove(convSpots.ClassSpot.NameTag);
                        }

                        //spotの所属情報を書き換え
                        convSpots.ClassSpot.PowerNameTag = selectedItemClassSpot.PowerNameTag;
                        targetSpot.PowerNameTag = convSpots.ClassSpot.PowerNameTag;
                        var newPower = classGameStatus.ListPower
                                .Where(x => x.NameTag == convSpots.ClassSpot.PowerNameTag)
                                .First();
                        newPower.ListMember.Add(convSpots.ClassSpot.NameTag);
                        convSpots.ClassPower = newPower;

                        // 領地に古い旗アイコンがあれば消して、新しい旗アイコンを置く
                        var worldMap = classGameStatus.WorldMap;
                        if (worldMap != null)
                        {
                            worldMap.ChangeFlag(newPower.FlagPath, convSpots.ClassSpot.NameTag);
                        }

                        ////unitの所属情報を書き換え
                        //防衛部隊を削除、又は他都市へ移動。隣接都市が無ければ放浪する
                        targetSpot.UnitGroup.Clear();
                        foreach (var item in classGameStatus.AllListSpot.Where(x => x.NameTag == selectedItemClassSpot.NameTag))
                        {
                            foreach (var itemUnitGroup in item.UnitGroup)
                            {
                                itemUnitGroup.Spot = convSpots.ClassSpot;
                                itemUnitGroup.FlagDisplay = true;
                                //unit移動
                                targetSpot.UnitGroup.Add(itemUnitGroup);
                            }
                            //これでは出撃してないユニットも全部消えてしまうので、後で対応、対応したらこのコメント消す
                            item.UnitGroup.Clear();
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
                    if (classGameStatus.TaskBattleMoveAsync.Item1 != null)
                    {
                        classGameStatus.TaskBattleMoveAsync.Item2.Cancel();
                    }
                    if (classGameStatus.TaskBattleMoveDefAsync.Item1 != null)
                    {
                        classGameStatus.TaskBattleMoveDefAsync.Item2.Cancel();
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
    }
}
