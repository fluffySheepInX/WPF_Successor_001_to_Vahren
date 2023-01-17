﻿using System;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_Successor_001_to_Vahren._005_Class;

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// UserControl041_PowerStart.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl041_PowerStart : UserControl
    {
        public UserControl041_PowerStart()
        {
            InitializeComponent();
        }

        // 定数
        // 項目サイズをここで調節できます
        private const int item_height = 60, space_height = 10, btn_width = 54, btn_height = 54;

        public void SetData()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 人材選択欄を表示する
            DisplayTalentList(mainWindow);

            // 勢力情報を表示する
            DisplayPowerInfo(mainWindow);

            // ウインドウ枠
            SetWindowFrame(mainWindow);

            // 画面の中央に配置する
            this.Margin = new Thickness()
            {
                Left = mainWindow.canvasUI.Width / 2 - this.Width / 2,
                Top = mainWindow.canvasUI.Height / 2 - this.Height / 2
            };

        }

        // ウインドウ枠を作る
        private void SetWindowFrame(MainWindow mainWindow)
        {
            // ウインドウスキンを読み込む
            List<string> strings = new List<string>();
            strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
            strings.Add("006_WindowImage");
            strings.Add("wnd0.png");
            string path = System.IO.Path.Combine(strings.ToArray());
            if (System.IO.File.Exists(path) == false)
            {
                // 画像が存在しない場合は、デザイン時のまま（色や透明度は xaml で指定する）
                return;
            }
            var skin_bitmap = new BitmapImage(new Uri(path));
            Int32Rect rect;
            ImageBrush myImageBrush;

            // RPGツクールXP (192x128) と VX (128x128) のスキンに対応する
            if ((skin_bitmap.PixelHeight != 128) || ((skin_bitmap.PixelWidth != 128) && (skin_bitmap.PixelWidth != 192)))
            {
                // その他の画像は、そのまま引き延ばして表示する
                // ブラシ設定によって、タイルしたり、アスペクト比を保ったりすることも可能
                myImageBrush = new ImageBrush(skin_bitmap);
                myImageBrush.Stretch = Stretch.Fill;
                this.rectWindowPlane.Fill = myImageBrush;
                this.rectWindowPlane2.Fill = myImageBrush;
                return;
            }

            // 不要な背景を表示しない
            this.rectShadowRight.Visibility = Visibility.Hidden;
            this.rectShadowBottom.Visibility = Visibility.Hidden;
            this.rectShadowRight2.Visibility = Visibility.Hidden;
            this.rectShadowBottom2.Visibility = Visibility.Hidden;

            // 中央
            rect = new Int32Rect(0, 0, skin_bitmap.PixelWidth - 64, skin_bitmap.PixelWidth - 64);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Stretch = Stretch.Fill;
            this.rectWindowPlane.Margin = new Thickness(4, 4, 4, 4);
            this.rectWindowPlane.Fill = myImageBrush;
            this.rectWindowPlane2.Margin = new Thickness(4, 4, 4, 4);
            this.rectWindowPlane2.Fill = myImageBrush;

            // 左上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 64, 0, 16, 16);
            this.imgWindowLeftTop.Source = new CroppedBitmap(skin_bitmap, rect);
            this.imgWindowLeftTop2.Source = new CroppedBitmap(skin_bitmap, rect);

            // 右上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 16, 0, 16, 16);
            this.imgWindowRightTop.Source = new CroppedBitmap(skin_bitmap, rect);
            this.imgWindowRightTop2.Source = new CroppedBitmap(skin_bitmap, rect);

            // 左下
            rect = new Int32Rect(skin_bitmap.PixelWidth - 64, 48, 16, 16);
            this.imgWindowLeftBottom.Source = new CroppedBitmap(skin_bitmap, rect);
            this.imgWindowLeftBottom2.Source = new CroppedBitmap(skin_bitmap, rect);

            // 右上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 16, 48, 16, 16);
            this.imgWindowRightBottom.Source = new CroppedBitmap(skin_bitmap, rect);
            this.imgWindowRightBottom2.Source = new CroppedBitmap(skin_bitmap, rect);

            // 上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 48, 0, 32, 16);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowTop.Fill = myImageBrush;
            this.rectWindowTop2.Fill = myImageBrush;

            // 下
            rect = new Int32Rect(skin_bitmap.PixelWidth - 48, 48, 32, 16);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowBottom.Fill = myImageBrush;
            this.rectWindowBottom2.Fill = myImageBrush;

            // 左
            rect = new Int32Rect(skin_bitmap.PixelWidth - 64, 16, 16, 32);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowLeft.Fill = myImageBrush;
            this.rectWindowLeft2.Fill = myImageBrush;

            // 右
            rect = new Int32Rect(skin_bitmap.PixelWidth - 16, 16, 16, 32);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowRight.Fill = myImageBrush;
            this.rectWindowRight2.Fill = myImageBrush;
        }

        private void DisplayTalentList(MainWindow mainWindow)
        {
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;
            if (classPowerAndCity == null)
            {
                return;
            }

            // タイトル
            this.txtTitle.Text = classPowerAndCity.ClassPower.Name + "の人材";

            // ボタンの背景
            BitmapImage? myBackImage = null;
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("006_WindowImage");
                strings.Add("wnd5.png");
                string path = System.IO.Path.Combine(strings.ToArray());
                if (System.IO.File.Exists(path))
                {
                    // 画像が存在する時だけ、ボタンの背景にする
                    myBackImage = new BitmapImage(new Uri(path));
                }
            }

            // ユニット画像のディレクトリ
            string pathChipImage;
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("040_ChipImage");
                pathChipImage = System.IO.Path.Combine(strings.ToArray()) + System.IO.Path.DirectorySeparatorChar;
            }

            // 顔絵画像のディレクトリ
            string pathFaceImage;
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("010_FaceImage");
                pathFaceImage = System.IO.Path.Combine(strings.ToArray()) + System.IO.Path.DirectorySeparatorChar;
            }

            // 雇用可能なユニットのリストを初期化する
            this.panelList.Children.Clear();
            int item_count = 0;

            // 勢力に所属する全ての領地
            foreach (var itemSpotTag in classPowerAndCity.ClassPower.ListMember)
            {
                // 識別名で ClassSpot を抜き出す
                var itemSpot = mainWindow.ClassGameStatus.NowListSpot.Where(x => x.NameTag == itemSpotTag).FirstOrDefault();
                if (itemSpot == null)
                {
                    continue; // 指定された領地が見つからなければ次へ
                }
                // 領地の初期配置ユニットが人材なら
                foreach (var itemMember in itemSpot.ListMember)
                {
                    var itemUnit = mainWindow.ClassGameStatus.ListUnit.Where(x => x.NameTag == itemMember.Item1 && x.Talent == "on").FirstOrDefault();
                    if (itemUnit == null)
                    {
                        continue;
                    }

                    // 共通設定で Grid を作る
                    Grid gridItem = new Grid();
                    ColumnDefinition colDef1 = new ColumnDefinition();
                    ColumnDefinition colDef2 = new ColumnDefinition();
                    ColumnDefinition colDef3 = new ColumnDefinition();
                    colDef1.Width = new GridLength(btn_width);
                    colDef2.Width = new GridLength(1.0, GridUnitType.Star);
                    colDef3.Width = new GridLength(0, GridUnitType.Auto);
                    gridItem.ColumnDefinitions.Add(colDef1);
                    gridItem.ColumnDefinitions.Add(colDef2);
                    gridItem.ColumnDefinitions.Add(colDef3);
                    RowDefinition rowDef1 = new RowDefinition();
                    RowDefinition rowDef2 = new RowDefinition();
                    rowDef1.Height = new GridLength(1.0, GridUnitType.Star);
                    rowDef2.Height = new GridLength(1.0, GridUnitType.Star);
                    gridItem.RowDefinitions.Add(rowDef1);
                    gridItem.RowDefinitions.Add(rowDef2);
                    gridItem.Height = item_height;
                    if (item_count == 0)
                    {
                        gridItem.Margin = new Thickness(5, 0, 5, 0);
                    }
                    else
                    {
                        gridItem.Margin = new Thickness(5, space_height, 5, 0);
                    }
                    //gridItem.Background = Brushes.Gray; // 実験用

                    // ユニット画像
                    BitmapImage bitimg1 = new BitmapImage(new Uri(pathChipImage + itemUnit.Image));

                    // 画像は本来のピクセルサイズで表示する
                    Image imgUnit = new Image();
                    imgUnit.Width = bitimg1.PixelWidth;
                    imgUnit.Height = bitimg1.PixelHeight;
                    imgUnit.Source = bitimg1;

                    // ボタンの枠は画像よりも大きくする
                    Button btnUnit = new Button();
                    btnUnit.Name = "btnUnit" + item_count.ToString();
                    btnUnit.Tag = itemUnit;
                    btnUnit.Width = btn_width;
                    btnUnit.Height = btn_height;
                    btnUnit.Focusable = false;

                    if (myBackImage != null)
                    {
                        // 背景画像をボタンに合わせて拡大縮小する
                        Image imgBack = new Image();
                        imgBack.Source = myBackImage;
                        imgBack.Stretch = Stretch.Fill;

                        Grid gridButton = new Grid();
                        gridButton.Children.Add(imgBack);

                        // 背景画像の上にユニット画像を重ねる
                        gridButton.Children.Add(imgUnit);

                        Border borderButton = new Border();
                        borderButton.Margin = new Thickness(-2);
                        borderButton.BorderThickness = new Thickness(2);
                        borderButton.BorderBrush = Brushes.Transparent;
                        // マウスカーソルがボタンの上に来ると強調する
                        borderButton.Background = Brushes.Transparent;
                        borderButton.MouseEnter += borderButtonImage_MouseEnter;
                        gridButton.Children.Add(borderButton);

                        btnUnit.Content = gridButton;
                    }
                    else
                    {
                        // 背景画像が無い場合は普通のボタンで表示する
                        btnUnit.Content = imgUnit;
                    }
                    // btnUnit.Click += btnUnit_Click;
                    // btnUnit.MouseEnter += btnUnit_MouseEnter;
                    Grid.SetRowSpan(btnUnit, 2);
                    gridItem.Children.Add(btnUnit);

                    // 名前
                    TextBlock txtName = new TextBlock();
                    txtName.Tag = itemUnit;
                    txtName.FontSize = 20;
                    txtName.Foreground = Brushes.White;
                    txtName.HorizontalAlignment = HorizontalAlignment.Center;
                    txtName.Text = itemUnit.Name + "（" + itemUnit.Race + "）";
                    Grid.SetColumn(txtName, 1);
                    gridItem.Children.Add(txtName);

                    // クラス
                    TextBlock txtClass = new TextBlock();
                    txtClass.Tag = itemUnit;
                    txtClass.FontSize = 20;
                    txtClass.Foreground = Brushes.Yellow;
                    txtClass.HorizontalAlignment = HorizontalAlignment.Center;
                    var originalClass = mainWindow.ClassGameStatus.ListUnit.Where(x => x.NameTag == itemUnit.Class).FirstOrDefault();
                    if (originalClass == null)
                    {
                        // クラス識別子からクラス情報を取得できなかった場合はそのまま表示する
                        txtClass.Text = "Lv" + itemUnit.Level.ToString() + " " + itemUnit.Class;
                    }
                    else
                    {
                        txtClass.Text = "Lv" + itemUnit.Level.ToString() + " " + originalClass.Name;
                    }
                    Grid.SetColumn(txtClass, 1);
                    Grid.SetRow(txtClass, 1);
                    gridItem.Children.Add(txtClass);

                    // 顔絵が存在する時だけ
                    if ((itemUnit.Face != string.Empty) && (System.IO.File.Exists(pathFaceImage + itemUnit.Face)))
                    {
                        BitmapImage bitimg2 = new BitmapImage(new Uri(pathFaceImage + itemUnit.Face));
                        Image imgFace = new Image();
                        imgFace.Width = item_height;
                        imgFace.Height = item_height;
                        imgFace.Source = bitimg2;

                        // 顔絵のマスク画像が存在する場合
                        string pathMask = pathFaceImage + "face_mask1.png";
                        if (System.IO.File.Exists(pathMask))
                        {
                            BitmapImage bitimg3 = new BitmapImage(new Uri(pathMask));
                            ImageBrush imgMask = new ImageBrush(bitimg3);
                            imgFace.OpacityMask = imgMask;
                        }

                        Grid.SetColumn(imgFace, 2);
                        Grid.SetRowSpan(imgFace, 2);
                        gridItem.Children.Add(imgFace);

                        // 顔絵の枠画像が存在する場合
                        string pathFrame = pathFaceImage + "face_frame1.png";
                        if (System.IO.File.Exists(pathFrame))
                        {
                            BitmapImage bitimg4 = new BitmapImage(new Uri(pathFrame));
                            Image imgFrame = new Image();
                            imgFrame.Width = item_height;
                            imgFrame.Height = item_height;
                            imgFrame.Source = bitimg4;

                            Grid.SetColumn(imgFrame, 2);
                            Grid.SetRowSpan(imgFrame, 2);
                            gridItem.Children.Add(imgFrame);
                        }

                    }

                    this.panelList.Children.Add(gridItem);
                    item_count++;
                }
            }


        }

        private void DisplayPowerInfo(MainWindow mainWindow)
        {
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;
            if (classPowerAndCity == null)
            {
                return;
            }

            //旗
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("030_FlagImage");
                strings.Add(classPowerAndCity.ClassPower.FlagPath);
                string path = System.IO.Path.Combine(strings.ToArray());
                BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                Int32Rect rect = new Int32Rect(0, 0, 32, 32);
                var destimg = new CroppedBitmap(bitimg1, rect);
                this.imgFlag.Source = destimg;
            }

            // 勢力名
            this.txtNamePower.Text = classPowerAndCity.ClassPower.Name;

        
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // キャンバスから自身を取り除く
            mainWindow.canvasUI.Children.Remove(this);

            // ルーティングを処理済みとしてマークする（親コントロールのイベントが発生しなくなる）
            e.Handled = true;
        }

        #region ウインドウ移動
        private bool _isDrag = false; // 外部に公開する必要なし
        private Point _startPoint;

        private void win_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow != null)
            {
                // 最前面に移動させる
                var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
                if ((listWindow != null) && (listWindow.Any()))
                {
                    int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                    Canvas.SetZIndex(this, maxZ + 1);
                }
            }

            // ドラッグを開始する
            UIElement el = (UIElement)sender;
            if (el != null)
            {
                _isDrag = true;
                _startPoint = e.GetPosition(el);
                el.CaptureMouse();
                el.MouseLeftButtonUp += win_MouseLeftButtonUp;
                el.MouseMove += win_MouseMove;
            }
        }
        private void win_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // ドラック中なら終了する
            if (_isDrag == true)
            {
                UIElement el = (UIElement)sender;
                el.ReleaseMouseCapture();
                el.MouseLeftButtonUp -= win_MouseLeftButtonUp;
                el.MouseMove -= win_MouseMove;
                _isDrag = false;
            }
        }
        private void win_MouseMove(object sender, MouseEventArgs e)
        {
            // ドラック中
            if (_isDrag == true)
            {
                UIElement el = (UIElement)sender;
                Point pt = e.GetPosition(el);

                var thickness = new Thickness();
                thickness.Left = this.Margin.Left + (pt.X - _startPoint.X);
                thickness.Top = this.Margin.Top + (pt.Y - _startPoint.Y);
                this.Margin = thickness;
            }
        }
        #endregion


        // 勢力開始ウインドウにカーソルを乗せた時
        private void winR_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // カーソルを離した時のイベントを追加する
            var cast = (UIElement)sender;
            cast.MouseLeave += win_MouseLeave;

            // 他のヘルプを全て隠す
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Visible) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    itemHelp.Visibility = Visibility.Hidden;
                }
            }

            // ヘルプを作成する
            var helpWindow = new UserControl030_Help();
            helpWindow.Name = "Help_" + this.Name;
            helpWindow.SetText("勢力を選択してプレイします。");
            mainWindow.canvasUI.Children.Add(helpWindow);
        }
        private void winL_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // カーソルを離した時のイベントを追加する
            var cast = (UIElement)sender;
            cast.MouseLeave += win_MouseLeave;

            // 他のヘルプを全て隠す
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Visible) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    itemHelp.Visibility = Visibility.Hidden;
                }
            }

            // ヘルプを作成する
            var helpWindow = new UserControl030_Help();
            helpWindow.Name = "Help_" + this.Name;
            helpWindow.SetText("勢力ではなく一人の人材を選択してプレイします。");
            mainWindow.canvasUI.Children.Add(helpWindow);
        }
        private void win_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // イベントを取り除く
            var cast = (UIElement)sender;
            cast.MouseLeave -= win_MouseLeave;

            // 表示中のヘルプを取り除く
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if (itemHelp.Name == "Help_" + this.Name)
                {
                    mainWindow.canvasUI.Children.Remove(itemHelp);
                    break;
                }
            }

            // 他のヘルプを隠してた場合は、最前面のヘルプだけ表示する
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
        }

        // 勢力選択画面で決定押した時の処理
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 勢力一覧ウィンドウを消す
            var ri2 = (UserControl040_PowerSelect)LogicalTreeHelper.FindLogicalNode(mainWindow.canvasUIRightTop, StringName.windowSelectionPowerMini);
            if (ri2 != null)
            {
                mainWindow.canvasUIRightTop.Children.Remove(ri2);
            }

            // キャンバスから自身を取り除く
            mainWindow.canvasUI.Children.Remove(this);

/*
            var classPowerAndCity = (ClassPowerAndCity)this.Tag;
            if (classPowerAndCity == null)
            {
                return;
            }
            // プレイヤー選択不可の勢力なら何もしない
            if (classPowerAndCity.ClassPower.EnableSelect == "off")
            {
                return;
            }
*/

            // 関数をこっちに移してもいいけど、とりあえず MainWindow のメンバ関数として呼び出す
            ((Button)sender).Tag = this.Tag;
            mainWindow.ButtonSelectionPowerDecide_Click(sender, e);
        }


        // ボタンの背景画像を白っぽくする
        private void borderButtonImage_MouseEnter(object sender, MouseEventArgs e)
        {
            var cast = (Border)sender;
            // ハイライトで強調する（文字色が白色なので、あまり白くすると読めなくなる）
            cast.Background = new SolidColorBrush(Color.FromArgb(48, 255, 255, 255));
            // マウスを離した時のイベントを追加する
            cast.MouseLeave += borderButtonImage_MouseLeave;
        }
        private void borderButtonImage_MouseLeave(object sender, MouseEventArgs e)
        {
            var cast = (Border)sender;
            cast.Background = Brushes.Transparent;
            // イベントを取り除く
            cast.MouseLeave -= borderButtonImage_MouseLeave;
        }




    }
}
