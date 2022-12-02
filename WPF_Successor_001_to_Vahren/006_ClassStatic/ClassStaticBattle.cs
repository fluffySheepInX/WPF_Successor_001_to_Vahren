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

namespace WPF_Successor_001_to_Vahren._006_ClassStatic
{
    public static class ClassStaticBattle
    {
        public static int TakasaMapTip { get; set; } = 32;
        public static int yokoMapTip { get; set; } = 64;

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
                    if (item.value.Building != string.Empty)
                    {
                        bui.Add(new(item.value.Building, battle.index, item.index));
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
                    classVec.Target = new Point(classUnit.OrderPosi.X, classUnit.OrderPosi.Y);
                    classVec.Vec = new Point(classUnit.VecMove.X, classUnit.VecMove.Y);
                    classVec.Speed = classUnit.Speed;

                    if (classVec.Hit(new Point(classUnit.NowPosi.X, classUnit.NowPosi.Y)))
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

                        //移動後の位置計算
                        double afterNowPosiX = classUnit.NowPosi.X + (classUnit.VecMove.X * classUnit.Speed);
                        double afterNowPosiY = classUnit.NowPosi.Y + (classUnit.VecMove.Y * classUnit.Speed);

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
                                        classUnit.NowPosi = new Point()
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
                                                re2.Margin = new Thickness(classUnit.NowPosi.X, classUnit.NowPosi.Y, 0, 0);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ////移動後に建築物有り
                                        //移動後の位置を再計算（一度バックする）
                                        afterNowPosiX = classUnit.NowPosi.X + (classUnit.VecMove.X * -(classUnit.Speed * 5));
                                        afterNowPosiY = classUnit.NowPosi.Y + (classUnit.VecMove.Y * -(classUnit.Speed * 5));
                                        //afterNowPosiX = classUnit.NowPosi.X + (classUnit.VecMove.X * -(32));
                                        //afterNowPosiY = classUnit.NowPosi.Y + (classUnit.VecMove.Y * -(16));
                                        //行列変換
                                        var resultConv = ClassStaticBattle.ConvertVec90(afterNowPosiX, afterNowPosiY, classUnit.NowPosi.X, classUnit.NowPosi.Y);

                                        bool ch2 = true;
                                        var targetTip2 = ClassStaticBattle.GetRecObj(classGameStatus.ClassBattle.ListBuildingAlive, resultConv.Item1, resultConv.Item2);
                                        ch2 = ClassStaticBattle.CheckRecObj(ch2, targetTip2, classGameStatus);

                                        if (ch2 == true)
                                        {
                                            //移動後に建築物無し
                                            classUnit.NowPosi = new Point()
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
                                                    re2.Margin = new Thickness(classUnit.NowPosi.X, classUnit.NowPosi.Y, 0, 0);
                                                }
                                            }

                                            //再計算する
                                            var calc0 = ClassCalcVec.ReturnVecDistance(
                                                from: new Point(classUnit.NowPosi.X, classUnit.NowPosi.Y),
                                                to: classUnit.OrderPosi
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
                                                    re2.Margin = new Thickness(classUnit.NowPosi.X, classUnit.NowPosi.Y, 0, 0);
                                                }
                                            }
                                            classUnit.OrderPosi = classUnit.NowPosi;
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

        public static void TaskBattleMoveAIAsync(CancellationToken cancelToken, ClassGameStatus classGameStatus, Window window)
        {
            Dictionary<long, (Task, CancellationTokenSource)> t = new Dictionary<long, (Task, CancellationTokenSource)>();
            List<ClassHorizontalUnit> listTarget = new List<ClassHorizontalUnit>();
            switch (classGameStatus.ClassBattle.BattleWhichIsThePlayer)
            {
                case BattleWhichIsThePlayer.Sortie:
                    listTarget = classGameStatus.ClassBattle.SortieUnitGroup;
                    break;
                case BattleWhichIsThePlayer.Def:
                    listTarget = classGameStatus.ClassBattle.DefUnitGroup;
                    break;
                case BattleWhichIsThePlayer.None:
                    break;
                default:
                    break;
            }

            while (true)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return;
                }

                //Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 100000)));
                Thread.Sleep((int)(Math.Floor(((double)1 / 60) * 1000)));

                foreach (var item in listTarget)
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
                            (Task, CancellationTokenSource) aaa =
                                new(Task.Run(() => ClassStaticBattle.TaskBattleMoveExecuteAsync(itemGroupBy, token, classGameStatus, window)), tokenSource);
                            t.Add(itemGroupBy.ID, aaa);
                        }
                    }
                }
            }
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
            int result = Math.Abs(nowX - targetX) + Math.Abs(nowY - targetY);
            return result;
        }
        #endregion
    }
}
