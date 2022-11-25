using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPF_Successor_001_to_Vahren._005_Class;
using WPF_Successor_001_to_Vahren._006_ClassStatic;
using WPF_Successor_001_to_Vahren._010_Enum;

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// Win010_TestBattle.xaml の相互作用ロジック
    /// </summary>
    public partial class Win010_TestBattle : Window
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

        private int _sizeClientWinWidth = 0;
        private int _sizeClientWinHeight = 0;
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

        public ClassTestBattle classTestBattle = new ClassTestBattle();

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


        public Win010_TestBattle(ClassTestBattle classTestBattle, ClassConfigGameTitle _classConfigGameTitle, ClassGameStatus classGameStatus)
        {
            InitializeComponent();
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
            this.classTestBattle = classTestBattle;
            this.ClassConfigGameTitle = _classConfigGameTitle;
            this.ClassGameStatus = classGameStatus;

            this.DataContext = new
            {
                canvasMainWidth = this.CanvasMainWidth,
                canvasMainHeight = this.CanvasMainHeight
            };

            var extractMap =
                    this.ClassGameStatus
                    .ListClassMapBattle
                    .Where(x => x.TagName == classTestBattle.Map)
                    .FirstOrDefault();
            if (extractMap != null)
            {
                this.ClassGameStatus.ClassBattleUnits.ClassMapBattle = extractMap;
                ClassStaticBattle.AddBuilding(this.ClassGameStatus);
            }

            foreach (var item in this.classTestBattle.ListMember)
            {
                for (int i = 0; i < item.Item2; i++)
                {
                    var info = this.ClassGameStatus.ListUnit.Where(x => x.NameTag.Contains(item.Item1)).FirstOrDefault();
                    if (info == null)
                    {
                        continue;
                    }

                    var classUnit = new List<ClassUnit>();
                    var deep = info.DeepCopy();
                    deep.ID = this.ClassGameStatus.IDCount;
                    this.ClassGameStatus.SetIDCount();
                    classUnit.Add(deep);

                    ClassHorizontalUnit aaa = new ClassHorizontalUnit();
                    aaa.ListClassUnit = classUnit;
                    this.ClassGameStatus.ClassBattleUnits.SortieUnitGroup.Add(aaa);
                }
            }
            foreach (var item in this.classTestBattle.ListMemberBouei)
            {
                for (int i = 0; i < item.Item2; i++)
                {
                    var info = this.ClassGameStatus.ListUnit.Where(x => x.NameTag.Contains(item.Item1)).FirstOrDefault();
                    if (info == null)
                    {
                        continue;
                    }

                    var classUnit = new List<ClassUnit>();
                    var deep = info.DeepCopy();
                    deep.ID = this.ClassGameStatus.IDCount;
                    this.ClassGameStatus.SetIDCount();
                    classUnit.Add(deep);

                    ClassHorizontalUnit aaa = new ClassHorizontalUnit();
                    aaa.ListClassUnit = classUnit;
                    this.ClassGameStatus.ClassBattleUnits.DefUnitGroup.Add(aaa);
                }
            }

            SetBattleMap();
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
            var ri2 = (UserControl005_StrategyMenu)LogicalTreeHelper.FindLogicalNode(this.canvasUIRightBottom, StringName.canvasStrategyMenu);
            if (ri2 != null)
            {
                this.canvasUIRightBottom.Children.Remove(ri2);
            }

            // 開いてる子ウインドウを全て閉じる
            this.canvasUI.Children.Clear();

            //マップそのもの
            Canvas canvas = new Canvas();
            int takasaMapTip = 32;
            int yokoMapTip = 64;
            canvas.Name = StringName.windowMapBattle;
            canvas.Background = Brushes.Black;
            canvas.MouseLeftButtonDown += CanvasMapBattle_MouseLeftButtonDown;
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
                        canvas.Width = this.ClassGameStatus.ClassBattleUnits.ClassMapBattle.MapData[0].Count * yokoMapTip;
                        canvas.Height = this.ClassGameStatus.ClassBattleUnits.ClassMapBattle.MapData.Count * takasaMapTip;
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
                    List<(BitmapImage, int, int)> listTakaiObj = new List<(BitmapImage, int, int)>();
                    foreach (var itemCol in this.ClassGameStatus.ClassBattleUnits.ClassMapBattle.MapData
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

                            if (itemRow.value.Building != string.Empty)
                            {
                                map.TryGetValue(itemRow.value.Building, out string? value2);
                                if (value2 != null)
                                {
                                    var build = new BitmapImage(new Uri(value2));
                                    listTakaiObj.Add(new(build, itemCol.index, itemRow.index));
                                }
                            }

                            var bi = new BitmapImage(new Uri(value));
                            ImageBrush image = new ImageBrush();
                            image.Stretch = Stretch.Fill;
                            image.ImageSource = bi;
                            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
                            path.Fill = image;
                            if (itemRow.value.BoueiButaiNoIti == true)
                            {
                                path.Tag = "Bouei";
                            }
                            if (itemRow.value.KougekiButaiNoIti == true)
                            {
                                path.Tag = "Kougeki";
                            }
                            path.Stretch = Stretch.Fill;
                            path.StrokeThickness = 0;
                            path.Data = Geometry.Parse("M 0," + takasaMapTip / 2
                                                    + " L " + yokoMapTip / 2 + "," + takasaMapTip
                                                    + " L " + yokoMapTip + "," + takasaMapTip / 2
                                                    + " L " + yokoMapTip / 2 + ",0 Z");
                            path.Margin = new Thickness()
                            {
                                Left = (itemCol.index * (yokoMapTip / 2)) + (itemRow.index * (yokoMapTip / 2)),
                                Top = ((canvas.Height / 2) + (itemCol.index * (takasaMapTip / 2)) + (itemRow.index * (-(takasaMapTip / 2)))) - takasaMapTip / 2
                            };
                            canvas.Children.Add(path);
                        }
                    }

                    //建築物描写
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

                    //建築物論理描写
                    //こちらを後でやる。クリックで爆破が出来るように
                    var bui = ClassGameStatus.ClassBattleUnits.DefUnitGroup
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
                        classUnitBuilding.NowPosi = new Point(path.Margin.Left, path.Margin.Top);
                        canvas.Children.Add(path);

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
                for (int i = 0; i < canvas.Children.Count; i++)
                {
                    if (canvas.Children[i] is System.Windows.Shapes.Path ppp)
                    {
                        string? taggg = Convert.ToString(ppp.Tag);
                        if (taggg != null)
                        {
                            if (taggg == "Bouei")
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
                if (this.ClassGameStatus.ClassBattleUnits.DefUnitGroup.Count % 2 == 0)
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
                            .ClassBattleUnits.DefUnitGroup
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
            this.timerAfterFadeIn.Tick += (x, s) =>
            {
                TimerAction60FPSAfterFadeInBattleStart();
                MainWindow.KeepInterval(this.timerAfterFadeIn);
            };
            AfterFadeIn = true;
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

        public DispatcherTimer timerAfterFadeIn = new DispatcherTimer(DispatcherPriority.Background);

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
            // canvasUI も canvasMain と同じく中央に置く。
            this.canvasUI.Margin = this.canvasMain.Margin;

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
                this.canvasUIRightTop.Margin = new Thickness()
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
                this.canvasUIRightBottom.Margin = new Thickness()
                {
                    Top = newTop,
                    Left = newLeft
                };
            }
        }

        #region Battle
        private void CanvasMapBattle_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            // ドラッグを開始する
            UIElement el = sender as UIElement;
            if (el != null)
            {
                this.ClassGameStatus.IsDrag = true;
                this.ClassGameStatus.StartPoint = e.GetPosition(el);
                el.CaptureMouse();
                el.MouseLeftButtonUp += CanvasMapBattle_MouseLeftButtonUp;
                el.MouseMove += CanvasMapBattle_MouseMove;
            }
        }
        private void CanvasMapBattle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // ドラック中なら終了する
            if (this.ClassGameStatus.IsDrag == true)
            {
                UIElement el = sender as UIElement;
                el.ReleaseMouseCapture();
                el.MouseLeftButtonUp -= CanvasMapBattle_MouseLeftButtonUp;
                el.MouseMove -= CanvasMapBattle_MouseMove;
                this.ClassGameStatus.IsDrag = false;
            }
        }
        private void CanvasMapBattle_MouseMove(object sender, MouseEventArgs e)
        {
            // ドラック中
            if (this.ClassGameStatus.IsDrag == true)
            {
                var ri = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapBattle);
                if (ri != null)
                {
                    UIElement el = sender as UIElement;
                    Point pt = e.GetPosition(el);

                    var thickness = new Thickness();
                    thickness.Left = ri.Margin.Left + (pt.X - this.ClassGameStatus.StartPoint.X);
                    thickness.Top = ri.Margin.Top + (pt.Y - this.ClassGameStatus.StartPoint.Y);
                    ri.Margin = thickness;
                }
            }
        }
        #endregion

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
                TimerAction60FPSBattle();
                MainWindow.KeepInterval(this.timerAfterFadeIn);
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
                        var po = this.ClassGameStatus.ListPower
                                .Where(x => x.NameTag == convSpots.ClassSpot.PowerNameTag)
                                .First();
                        po.ListMember.Add(convSpots.ClassSpot.NameTag);

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

        List<Rectangle> getMap = new List<Rectangle>();

        private async Task TaskBattleMoveExecuteAsync(ClassUnit classUnit, CancellationToken token)
        {
            try
            {
                ////移動し過ぎを防止
                //int counter = 1000;

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
                                    var targetTip = GetRecObj(getMap, afterNowPosiX, afterNowPosiY);
                                    ch = ClassStaticBattle.CheckRecObj(ch, targetTip, ClassGameStatus);

                                    if (ch == true)
                                    {
                                        //移動後に建築物無し
                                        classUnit.NowPosi = new Point()
                                        {
                                            X = afterNowPosiX,
                                            Y = afterNowPosiY
                                        };

                                        var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapBattle);
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
                                        //行列変換
                                        var resultConv = ConvertVec90(afterNowPosiX, afterNowPosiY, classUnit.NowPosi.X, classUnit.NowPosi.Y);

                                        bool ch2 = true;
                                        var targetTip2 = GetRecObj(getMap, resultConv.Item1, resultConv.Item2);
                                        ch2 = ClassStaticBattle.CheckRecObj(ch2, targetTip2, ClassGameStatus);

                                        if (ch2 == true)
                                        {
                                            //移動後に建築物無し
                                            classUnit.NowPosi = new Point()
                                            {
                                                X = resultConv.Item1,
                                                Y = resultConv.Item2
                                            };

                                            var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapBattle);
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
                                            var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapBattle);
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
                        });
                    }

                    //counter--;

                    //if (counter <= 0)
                    //{
                    //    throw new Exception("ErrorNumber:000001");
                    //}

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static IEnumerable<Rectangle> GetRecObj(List<Rectangle> getMap, double nowPosiX, double nowPosiY)
        {
            return getMap
                                .Where(x => ((ClassMapTipRectangle)x.Tag).LogicalXY.Left <= nowPosiX
                                        && (((ClassMapTipRectangle)x.Tag).LogicalXY.Left + 64) >= nowPosiX)
                                .Where(y => ((ClassMapTipRectangle)y.Tag).LogicalXY.Top <= nowPosiY
                                        && (((ClassMapTipRectangle)y.Tag).LogicalXY.Top + 32) >= nowPosiY);
        }

        public (double, double) ConvertVec90(double x, double y, double vecX, double vecY)
        {
            //(cos90*(x-vecX))+(-sin90*(y-vecY)) = x
            //(sin90*(x-vecX))+(cos90*(y-vecY)) = y
            double resultX = (0 * (x - vecX)) + (-1 * (y - vecY));
            double resultY = (1 * (x - vecX)) + (0 * (y - vecY));

            return (resultX + x, resultY + y);
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
                                            //通常ユニット破壊
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
                                            }
                                            //建築物破壊
                                            if (itemRe is ClassUnitBuilding building)
                                            {
                                                var re1 = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canvasMain, StringName.windowMapBattle);
                                                if (re1 != null)
                                                {
                                                    var re2 = (Rectangle)LogicalTreeHelper.FindLogicalNode(re1, "Bui" + building.X + "a" + building.Y);
                                                    if (re2 != null)
                                                    {
                                                        re1.Children.Remove(re2);
                                                        getMap.Remove(re2);
                                                    }
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

        #endregion

    }
}
