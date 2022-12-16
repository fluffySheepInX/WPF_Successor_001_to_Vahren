using System;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WPF_Successor_001_to_Vahren._005_Class;

/*
ダイアログの使い方

まずは、初期化します。
var dialog = new Win020_Dialog();

文章を指定します。改行することもできます。
文章が３行以下なら、顔絵がある場合と同じ高さで表示されます。
４行以上だと、ウインドウが高くなっていきます。
dialog.SetData("なんたら国\nと友好が深まりました。");

顔絵を追加する際は、ユニットの識別名（NameTag）かファイル名を指定します。
通常は文章の左側に顔絵を表示するけど、右側に追加することもできます。
dialog.AddFace("AbelIrijhorn", "");
dialog.AddFaceRight("", "face002.png");

設定されてる顔絵を消すことも可能（左右個別に指定）
dialog.RemoveFace();
dialog.RemoveFaceRight();

ダイアログを放置すると 5秒後に自動的に閉じます。
マウス左ドラッグで動かすと、閉じなくなります。
他のウインドウの後ろにある時も、閉じません。
最前面に戻すと、自動的に閉じるようになります。

自動的に閉じるまでの待ち時間を設定可能（秒単位）
dialog.SetTime(2.5);

自動的に閉じないようにする際はマイナスを指定
dialog.SetTime(-1);

ダイアログを表示してる間、親ウインドウへ入力できなくなります。
dialog.ShowDialog();

戻り値で、手動で閉じたか、自動で閉じたかを判別可能です。
bool? result = dialog.ShowDialog();

ダイアログが表示されてる状態で、
Enter, Space, Z キーを押すと、閉じます。

*/
namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// Win020_Dialog.xaml の相互作用ロジック
    /// </summary>
    public partial class Win020_Dialog : Window
    {
        public Win020_Dialog()
        {
            InitializeComponent();

            // 親ウインドウをセットする
            this.Owner = Application.Current.MainWindow;

            // タイマーの初期化
            _timerClose.Tick += new EventHandler(Window_AutoClose);
            _timerClose.Interval = TimeSpan.FromSeconds(5); // 5秒間隔に設定
        }

        // 文章を指定する
        public void SetData(string txtInput)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 文章
            this.txtMain.Text = txtInput;

            // ボタンの背景
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("006_WindowImage");
                strings.Add("wnd5.png");
                string path = System.IO.Path.Combine(strings.ToArray());
                if (System.IO.File.Exists(path))
                {
                    // 画像が存在する時だけ、ボタンの枠と文字色を背景に合わせる
                    BitmapImage theImage = new BitmapImage(new Uri(path));
                    ImageBrush myImageBrush = new ImageBrush(theImage);
                    myImageBrush.Stretch = Stretch.Fill;
                    this.btnOK.Background = myImageBrush;
                    this.btnOK.Foreground = Brushes.White;
                    this.btnOK.BorderBrush = Brushes.Silver;
                }
            }

            // ウインドウ枠
            SetWindowFrame(mainWindow);
        }

        // ウインドウ枠を作る
        private void SetWindowFrame(MainWindow mainWindow)
        {
            // ウインドウスキンを読み込む
            List<string> strings = new List<string>();
            strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
            strings.Add("006_WindowImage");
            strings.Add("wnd3.png");
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
                return;
            }

            // 不要な背景を表示しない
            this.rectShadowRight.Visibility = Visibility.Hidden;
            this.rectShadowBottom.Visibility = Visibility.Hidden;

            // 中央
            rect = new Int32Rect(0, 0, skin_bitmap.PixelWidth - 64, skin_bitmap.PixelWidth - 64);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Stretch = Stretch.Fill;
            this.rectWindowPlane.Margin = new Thickness(4, 4, 4, 4);
            this.rectWindowPlane.Fill = myImageBrush;

            // 左上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 64, 0, 16, 16);
            this.imgWindowLeftTop.Source = new CroppedBitmap(skin_bitmap, rect);

            // 右上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 16, 0, 16, 16);
            this.imgWindowRightTop.Source = new CroppedBitmap(skin_bitmap, rect);

            // 左下
            rect = new Int32Rect(skin_bitmap.PixelWidth - 64, 48, 16, 16);
            this.imgWindowLeftBottom.Source = new CroppedBitmap(skin_bitmap, rect);

            // 右上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 16, 48, 16, 16);
            this.imgWindowRightBottom.Source = new CroppedBitmap(skin_bitmap, rect);

            // 上
            rect = new Int32Rect(skin_bitmap.PixelWidth - 48, 0, 32, 16);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowTop.Fill = myImageBrush;

            // 下
            rect = new Int32Rect(skin_bitmap.PixelWidth - 48, 48, 32, 16);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowBottom.Fill = myImageBrush;

            // 左
            rect = new Int32Rect(skin_bitmap.PixelWidth - 64, 16, 16, 32);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowLeft.Fill = myImageBrush;

            // 右
            rect = new Int32Rect(skin_bitmap.PixelWidth - 16, 16, 16, 32);
            myImageBrush = new ImageBrush(new CroppedBitmap(skin_bitmap, rect));
            myImageBrush.Viewport = new Rect(0, 0, rect.Width, rect.Height);
            myImageBrush.ViewportUnits = BrushMappingMode.Absolute;
            myImageBrush.TileMode = TileMode.Tile;
            this.rectWindowRight.Fill = myImageBrush;
        }

        #region ウインドウ移動
        private void Window_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            // 移動中に閉じないよう、タイマーを止める
            _timerClose.Stop();

            // Windowにはドラッグ移動が標準で実装されてる
            this.DragMove();
        }
        #endregion

        #region キー入力
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Enter, Space, Z キー = OK
            if ((e.Key == Key.Return) || (e.Key == Key.Space) || (e.Key == Key.Z))
            {
                // 戻り値をセットすると自動的に閉じる
                DialogResult = true;
            }
        }
        #endregion

        // 確認ボタン = OK
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            // 戻り値をセットすると自動的に閉じる
            DialogResult = true;
        }

        #region タイマー処理

        // ウインドウが開いて一定時間経過すると、自動的に閉じる
        private DispatcherTimer _timerClose = new DispatcherTimer();
        private void Window_AutoClose(object sender, EventArgs e)
        {
            // プレイヤーが入力してないので、false を返す
            DialogResult = false;
        }

        // ウインドウが最前面になったらタイマーを開始する
        private void Window_Activated(object sender, EventArgs e)
        {
            _timerClose.Start();
        }

        // 他のウインドウの後ろになったらタイマーを止める
        private void Window_Deactivated(object sender, EventArgs e)
        {
            _timerClose.Stop();
        }

        // 自動的に閉じるまでの待ち時間を設定する（初期設定は5秒）
        public void SetTime(double timeWait)
        {
            // 不正な値は最大値にする（ほぼ無限に待つ）
            if ((timeWait < 0) || (timeWait > 2147483))
            {
                // 2147483647 / 1000 = 2147483
                timeWait = 2147483;
            }

            _timerClose.Interval = TimeSpan.FromSeconds(timeWait);
        }

        #endregion

        #region 顔絵表示

        // 文章の左側に顔絵を追加する
        public void AddFace(string strNameTag, string strFilename)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            string strFaceFile = string.Empty;

            // ユニットの識別名を指定した場合
            if (strNameTag != string.Empty)
            {
                var classUnit = mainWindow.ClassGameStatus.ListUnit.Where(x => x.NameTag == strNameTag).FirstOrDefault();
                if (classUnit != null)
                {
                    strFaceFile = classUnit.Face;
                }
            }

            // ユニットに顔絵が無い場合でも、ファイル名を直接指定できる
            if (strFaceFile == string.Empty)
            {
                strFaceFile = strFilename;
            }

            // 顔絵のファイルを読み込む
            if (strFaceFile != string.Empty)
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("010_FaceImage");
                strings.Add(strFaceFile);
                string path = System.IO.Path.Combine(strings.ToArray());
                if (System.IO.File.Exists(path))
                {
                    BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                    imgFaceLeft.Source = bitimg1;

                    // 枠を表示して間隔を空ける
                    borderLeft.Visibility = Visibility.Visible;
                    txtMain.Margin = new Thickness(130, 10, txtMain.Margin.Right, 10);
                }
            }
        }

        // 文章の左側の顔絵を取り除く
        public void RemoveFace()
        {
            // 顔絵のファイルを読み込む
            if (imgFaceLeft.Source != null)
            {
                imgFaceLeft.Source = null;

                // 枠を隠す
                borderLeft.Visibility = Visibility.Hidden;
                txtMain.Margin = new Thickness(15, 10, txtMain.Margin.Right, 10);
            }
        }

        // 文章の右側に顔絵を追加する
        public void AddFaceRight(string strNameTag, string strFilename)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            string strFaceFile = string.Empty;

            // ユニットの識別名を指定した場合
            if (strNameTag != string.Empty)
            {
                var classUnit = mainWindow.ClassGameStatus.ListUnit.Where(x => x.NameTag == strNameTag).FirstOrDefault();
                if (classUnit != null)
                {
                    strFaceFile = classUnit.Face;
                }
            }

            // ユニットに顔絵が無い場合でも、ファイル名を直接指定できる
            if (strFaceFile == string.Empty)
            {
                strFaceFile = strFilename;
            }

            // 顔絵のファイルを読み込む
            if (strFaceFile != string.Empty)
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("010_FaceImage");
                strings.Add(strFaceFile);
                string path = System.IO.Path.Combine(strings.ToArray());
                if (System.IO.File.Exists(path))
                {
                    BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                    imgFaceRight.Source = bitimg1;

                    // 枠を表示して間隔を空ける
                    borderRight.Visibility = Visibility.Visible;
                    txtMain.Margin = new Thickness(txtMain.Margin.Left, 10, 130, 10);
                }
            }
        }

        // 文章の右側の顔絵を取り除く
        public void RemoveFaceRight()
        {
            // 顔絵のファイルを読み込む
            if (imgFaceRight.Source != null)
            {
                imgFaceRight.Source = null;

                // 枠を隠す
                borderRight.Visibility = Visibility.Hidden;
                txtMain.Margin = new Thickness(txtMain.Margin.Left, 10, 15, 10);
            }
        }

        #endregion

    }
}
