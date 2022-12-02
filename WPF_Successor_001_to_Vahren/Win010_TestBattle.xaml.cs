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
    public partial class Win010_TestBattle : CommonWindow
    {
        public ClassTestBattle classTestBattle = new ClassTestBattle();

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
                this.ClassGameStatus.ClassBattle.ClassMapBattle = extractMap;
                ClassStaticBattle.AddBuilding(this.ClassGameStatus);
            }

            foreach (var item in this.classTestBattle.ListMember)
            {
                for (int i = 0; i < item.Item2; i++)
                {
                    var info = this.ClassGameStatus.ListUnit.Where(x => x.NameTag == item.Item1).FirstOrDefault();
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
                    this.ClassGameStatus.ClassBattle.SortieUnitGroup.Add(aaa);
                }
            }
            foreach (var item in this.classTestBattle.ListMemberBouei)
            {
                for (int i = 0; i < item.Item2; i++)
                {
                    var info = this.ClassGameStatus.ListUnit.Where(x => x.NameTag == item.Item1).FirstOrDefault();
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
                    this.ClassGameStatus.ClassBattle.DefUnitGroup.Add(aaa);
                }
            }

            SetBattleMap();
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
                        classUnitBuilding.NowPosi = new Point(path.Margin.Left, path.Margin.Top);
                        canvas.Children.Add(path);
                    }
                }

                this.canvasMain.Children.Add(
                    GetCanvasBattleBack(canvas,
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
                decimal countMeHalf = Math.Floor((decimal)ClassGameStatus.ClassBattle.DefUnitGroup.Count / 2);
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

        #region BattleEvent
        private void CanvasMapBattle_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            // ドラッグを開始する
            UIElement? el = sender as UIElement;
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
                UIElement? el = sender as UIElement;
                if (el == null)
                {
                    return;
                }
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
                    UIElement? el = sender as UIElement;
                    if (el == null)
                    {
                        return;
                    }
                    Point pt = e.GetPosition(el);

                    var thickness = new Thickness();
                    thickness.Left = ri.Margin.Left + (pt.X - this.ClassGameStatus.StartPoint.X);
                    thickness.Top = ri.Margin.Top + (pt.Y - this.ClassGameStatus.StartPoint.Y);
                    ri.Margin = thickness;
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
            List<ClassHorizontalUnit> lisClassHorizontalUnit = new List<ClassHorizontalUnit>();
            switch (ClassGameStatus.ClassBattle.BattleWhichIsThePlayer)
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
            List<ClassHorizontalUnit> lisClassHorizontalUnit = new List<ClassHorizontalUnit>();
            switch (ClassGameStatus.ClassBattle.BattleWhichIsThePlayer)
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

            ////スキルスレッド開始
            //出撃ユニット
            {
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                (Task, CancellationTokenSource) a = new(Task.Run(() => TaskBattleSkill(token)), tokenSource);
                this.ClassGameStatus.TaskBattleSkill = a;
            }
            //防衛ユニット
            {
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                (Task, CancellationTokenSource) a = new(Task.Run(() => TaskBattleSkill(token)), tokenSource);
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
                        (Task, CancellationTokenSource) a = new(Task.Run(() => TaskBattleMoveAsync(token)), tokenSource);
                        this.ClassGameStatus.TaskBattleMoveAsync = a;
                    }
                    //防衛(AI)ユニット
                    {
                        var tokenSource = new CancellationTokenSource();
                        var token = tokenSource.Token;
                        (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveAIAsync(token, this.ClassGameStatus, this)), tokenSource);
                        this.ClassGameStatus.TaskBattleMoveDefAsync = a;
                    }
                    break;
                case BattleWhichIsThePlayer.Def:
                    //出撃(AI)ユニット
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
                        (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveAIAsync(token, this.ClassGameStatus, this)), tokenSource);
                        this.ClassGameStatus.TaskBattleMoveDefAsync = a;
                    }
                    break;
                case BattleWhichIsThePlayer.None:
                    //出撃(AI)ユニット
                    {
                        var tokenSource = new CancellationTokenSource();
                        var token = tokenSource.Token;
                        (Task, CancellationTokenSource) a = new(Task.Run(() => TaskBattleMoveAsync(token)), tokenSource);
                        this.ClassGameStatus.TaskBattleMoveAsync = a;
                    }
                    //防衛(AI)ユニット
                    {
                        var tokenSource = new CancellationTokenSource();
                        var token = tokenSource.Token;
                        (Task, CancellationTokenSource) a = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveAIAsync(token, this.ClassGameStatus, this)), tokenSource);
                        this.ClassGameStatus.TaskBattleMoveDefAsync = a;
                    }
                    break;
                default:
                    break;
            }

        }

        private void TimerAction60FPSBattle()
        {
            //攻撃側勝利
            {
                bool flgaDefHp = false;
                foreach (var itemDefUnitGroup in this.ClassGameStatus.ClassBattle.DefUnitGroup)
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
                    this.ClassGameStatus.ClassBattle.SortieUnitGroup.Clear();
                    this.ClassGameStatus.ClassBattle.DefUnitGroup.Clear();
                    this.ClassGameStatus.ClassBattle.NeutralUnitGroup.Clear();

                    return;
                }
            }
            //防衛側勝利
            {
                bool flgaAttackHp = false;
                foreach (var itemDefUnitGroup in this.ClassGameStatus.ClassBattle.SortieUnitGroup)
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
                    this.ClassGameStatus.ClassBattle.SortieUnitGroup.Clear();
                    this.ClassGameStatus.ClassBattle.DefUnitGroup.Clear();
                    this.ClassGameStatus.ClassBattle.NeutralUnitGroup.Clear();

                    return;
                }
            }
        }

        private void TaskBattleSkill(CancellationToken token)
        {
            List<ClassHorizontalUnit> aaa = new List<ClassHorizontalUnit>();
            List<ClassHorizontalUnit> bbb = new List<ClassHorizontalUnit>();
            switch (this.ClassGameStatus.ClassBattle.BattleWhichIsThePlayer)
            {
                case BattleWhichIsThePlayer.Sortie:
                    aaa = this.ClassGameStatus.ClassBattle.SortieUnitGroup;
                    bbb = this.ClassGameStatus.ClassBattle.DefUnitGroup;
                    break;
                case BattleWhichIsThePlayer.Def:
                    aaa = this.ClassGameStatus.ClassBattle.DefUnitGroup;
                    bbb = this.ClassGameStatus.ClassBattle.SortieUnitGroup;
                    break;
                case BattleWhichIsThePlayer.None:
                    break;
                default:
                    break;
            }

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
                            var xA = itemGroupBy.NowPosi;
                            foreach (var itemDefUnitGroup in bbb)
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
                .ClassBattle.SortieUnitGroup)
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
                            (Task, CancellationTokenSource) aaa = new(Task.Run(() => ClassStaticBattle.TaskBattleMoveExecuteAsync(itemGroupBy, token, ClassGameStatus, this)), tokenSource);
                            t.Add(itemGroupBy.ID, aaa);
                        }
                    }
                }
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
                        foreach (var item in this.ClassGameStatus.ClassBattle.DefUnitGroup)
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
                                                        ClassGameStatus.ClassBattle.ListBuildingAlive.Remove(re2);
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
