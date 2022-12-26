using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MapEditor
{
    /// <summary>
    /// WinPreBattle.xaml の相互作用ロジック
    /// </summary>
    public partial class WinPreBattle : Window
    {
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
        public Point StartPoint { get; set; } = new Point();
        public bool IsDrag { get; set; } = false;

        public List<List<ClassMap>> MyProperty { get; set; }
        public List<string> fileTips { get; set; } = new List<string>();

        public WinPreBattle(List<List<ClassMap>> classMaps, List<string> fileTips)
        {
            InitializeComponent();
            MyProperty = classMaps;
            this.fileTips = fileTips;
        }

        public static int TakasaMapTip { get; set; } = 32;
        public static int YokoMapTip { get; set; } = 64;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //マップそのもの
            Canvas canvas = new Canvas();
            int takasaMapTip = TakasaMapTip;
            int yokoMapTip = YokoMapTip;
            canvas.Name = "windowMapBattle";
            canvas.Background = Brushes.Black;
            canvas.MouseLeftButtonDown += CanvasMapBattle_MouseLeftButtonDown;
            //canvas.MouseRightButtonDown += windowMapBattle_MouseRightButtonDown;
            {
                if (false)
                {
                }
                else
                {
                    //キャンバス設定
                    {
                        canvas.Width = MyProperty[0].Count * yokoMapTip;
                        canvas.Height = MyProperty.Count * takasaMapTip;
                        canvas.Margin = new Thickness()
                        {
                            Left = ((
                                    (this.CanvasMainWidth / 2) - (this._sizeClientWinWidth / 2)
                                    ))
                                        +
                                    (this._sizeClientWinWidth / 2) - ((MyProperty[0].Count * 32) / 2),
                            Top = (this._sizeClientWinHeight / 2) - (canvas.Height / 2)
                        };
                    }

                    //// get files.
                    //IEnumerable<string> files = fileTips;
                    //Dictionary<string, string> map = new Dictionary<string, string>();
                    //foreach (var item in files)
                    //{
                    //    map.Add(System.IO.Path.GetFileNameWithoutExtension(item), item);
                    //}

                    //Path描写
                    List<(BitmapImage, int, int)> listTakaiObj = new List<(BitmapImage, int, int)>();
                    foreach (var itemCol in MyProperty
                                            .Select((value, index) => (value, index)))
                    {
                        foreach (var itemRow in itemCol.value.Select((value, index) => (value, index)))
                        {
                            //map.TryGetValue(itemRow.value.field, out string? value);
                            //if (value == null) continue;

                            //複数オブジェクトを将来、出すつもり
                            if (itemRow.value.build.Count != 0)
                            {
                                var build = new BitmapImage(new Uri(itemRow.value.build[0]));
                                listTakaiObj.Add(new(build, itemCol.index, itemRow.index));
                            }

                            var bi = new BitmapImage(new Uri(itemRow.value.field));
                            ImageBrush image = new ImageBrush();
                            image.Stretch = Stretch.Fill;
                            image.ImageSource = bi;
                            System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
                            path.Fill = image;

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
                            //itemRow.value.MapPath = path;
                        }
                    }

                    ////建築物描写
                    //ClassStaticBattle.DisplayBuilding(canvas, takasaMapTip, yokoMapTip, listTakaiObj, ClassGameStatus.ClassBattle.ListBuildingAlive);
                }

                this.canBase.Children.Add(
                    SetAndGetCanvasBattleBack(canvas,
                                        this._sizeClientWinWidth,
                                        this._sizeClientWinHeight,
                                        this.CanvasMainWidth,
                                        this.CanvasMainHeight)
                    );
            }
        }

        private void canBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // クライアント領域を知る方法
            var si = e.NewSize;
            this._sizeClientWinWidth = (int)si.Width;
            this._sizeClientWinHeight = (int)si.Height;

            // canvasMain を常にウインドウの中央に置く。
            this.canBase.Margin = new Thickness()
            {
                Top = (this._sizeClientWinHeight / 2) - (this.CanvasMainHeight / 2),
                Left = (this._sizeClientWinWidth / 2) - (this.CanvasMainWidth / 2)
            };
        }

        public static Canvas SetAndGetCanvasBattleBack(Canvas canvas,
                                                int _sizeClientWinWidth,
                                                int _sizeClientWinHeight,
                                                int CanvasMainWidth,
                                                int CanvasMainHeight
                                                )
        {
            Canvas backCanvas = new Canvas();
            backCanvas.Name = "gridMapBattle";
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
        /// <summary>
        /// ドラッグを開始する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CanvasMapBattle_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            UIElement? el = sender as UIElement;
            if (el == null) return;

            IsDrag = true;
            StartPoint = e.GetPosition(el);
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
            if (IsDrag == false) return;
            UIElement? el = sender as UIElement;
            if (el == null) return;

            el.ReleaseMouseCapture();
            el.MouseLeftButtonUp -= CanvasMapBattle_MouseLeftButtonUp;
            el.MouseMove -= CanvasMapBattle_MouseMove;
            IsDrag = false;
        }
        /// <summary>
        /// ドラック中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CanvasMapBattle_MouseMove(object sender, MouseEventArgs e)
        {
            //var cw = ClassStaticCommonMethod.FindAncestors((Canvas)sender).OfType<CommonWindow>().FirstOrDefault();
            //if (cw == null) return;
            if (IsDrag == false) return;

            UIElement? el = sender as UIElement;
            if (el == null) return;

            var ri = (Canvas)LogicalTreeHelper.FindLogicalNode(this.canBase, "windowMapBattle");

            Point pt = e.GetPosition(el);
            var thickness = new Thickness();
            thickness.Left = ri.Margin.Left + (pt.X - StartPoint.X);
            thickness.Top = ri.Margin.Top + (pt.Y - StartPoint.Y);
            ri.Margin = thickness;
        }
    }
}
