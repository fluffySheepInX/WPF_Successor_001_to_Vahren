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
using WPF_Successor_001_to_Vahren._005_Class;

/*
選択ダイアログの使い方

まずは、初期化します。
var dialog = new Win025_Select();

文章を指定します。改行することもできます。
文章の長さによってウインドウは自動的に大きくなります。
なお、ボタンは常に同じ位置になります。（親ウインドウの中央付近）
dialog.SetText("なんたら国でよろしいですか？");

顔絵を追加する際は、ユニットの識別名（NameTag）かファイル名を指定します。
dialog.AddFace("AbelIrijhorn", "");

設定されてる顔絵を消すことも可能
dialog.RemoveFace();

ダイアログを表示してる間、親ウインドウへ入力できなくなります。
bool? result = dialog.ShowDialog();

戻り値で、どちらのボタンを押したかを判別します。
true  = 決定ボタン
false = 取消ボタン

キー入力で選択することも可能です。
Enter, Space, Z キーを押すと、決定します。
X, Insert, NumPad0 キーを押すと、取り消します。

行の先頭に特殊記号が存在すると、その行の文字色を変えます。
#	黄色
##	淡い水色
#{	橙色（行を跨いだ一括指定が可能）
#}	橙色から通常色に戻る

*/
namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// Win025_Select.xaml の相互作用ロジック
    /// </summary>
    public partial class Win025_Select : Window
    {
        public Win025_Select()
        {
            InitializeComponent();

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 親ウインドウをセットする
            this.Owner = mainWindow;

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
                    this.btnCancel.Background = myImageBrush;
                    this.btnCancel.Foreground = Brushes.White;
                    this.btnCancel.BorderBrush = Brushes.Silver;
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

        #region ウインドウ配置
        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 親ウインドウの中央付近に配置する
            double mainLeft = mainWindow.Left;
            double mainTop = mainWindow.Top;
            double mainWidth = mainWindow.ActualWidth;
            double mainHeight = mainWindow.ActualHeight;

            // 横方向は中央だけど、ウインドウの下端を同じ位置にする
            double newLeft = mainLeft + mainWidth / 2 - this.ActualWidth / 2;
            double newTop = mainTop + mainHeight / 2 - this.ActualHeight + 100;

            // 画面の外に出ないようにする
            double maxLeft = System.Windows.SystemParameters.WorkArea.Width - this.ActualWidth;
            double maxTop = System.Windows.SystemParameters.WorkArea.Height - this.ActualHeight;
            if (newLeft < 0)
            {
                newLeft = 0;
            }
            if (newLeft > maxLeft)
            {
                newLeft = maxLeft;
            }
            if (newTop < 0)
            {
                newTop = 0;
            }
            if (newTop > maxTop)
            {
                newTop = maxTop;
            }

            this.Left = newLeft;
            this.Top = newTop;
        }
        #endregion

        #region ウインドウ移動
        private void Window_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
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
                // キーを押しっぱなしにしても無視する
                if (e.IsRepeat == false)
                {
                    // 戻り値をセットすると自動的に閉じる
                    DialogResult = true;
                }
            }
            // X, Insert, NumPad0 キー = Cancel
            else if ((e.Key == Key.X) || (e.Key == Key.Insert) || (e.Key == Key.NumPad0))
            {
                // キーを押しっぱなしにしても無視する
                if (e.IsRepeat == false)
                {
                    // 戻り値をセットすると自動的に閉じる
                    DialogResult = false;
                }
            }
            // Escape キー = ゲーム終了
            else if (e.Key == Key.Escape)
            {
                if (MessageBox.Show("ゲームを終了しますか？", "ローガントゥーガ", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    // ゲーム画面とダイアログの両方を閉じる
                    var mainWindow = (MainWindow)Application.Current.MainWindow;
                    if (mainWindow != null)
                    {
                        mainWindow.Close();
                    }
                    DialogResult = false;
                }
            }
        }
        #endregion

        // 決定ボタン = OK
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            // 戻り値をセットすると自動的に閉じる
            DialogResult = true;
        }

        // 取消ボタン = Cancel
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // 戻り値をセットすると自動的に閉じる
            DialogResult = false;
        }

        // 文章を指定する
        public void SetText(string strInput)
        {
            // まずは Text を空にする
            this.txtMain.Text = string.Empty;
            this.txtMain.Inlines.Clear();

            // 文字色を変更するかどうか調べる
            if (strInput.Contains("#"))
            {
                // 行単位で色を変えるので、行ごとに分割する
                string[] everyLines = strInput.ReplaceLineEndings().Split(System.Environment.NewLine);

                // 行ごとに先頭と末尾をチェックする
                bool IsColorChange = false;
                string strOutput = string.Empty;
                int countLines = everyLines.Length;
                for (int i = 0; i < countLines; i++)
                {
                    string eachLine = everyLines[i];

                    // 行頭の「#}」は通常色に戻す
                    if (eachLine.StartsWith("#}"))
                    {
                        IsColorChange = false;
                        strOutput += eachLine.Substring(2);
                        if (i + 1 < countLines)
                        {
                            strOutput += System.Environment.NewLine;
                        }
                    }
                    // 行頭の「#{」は橙色
                    else if ((eachLine.StartsWith("#{") == true) || (IsColorChange == true))
                    {
                        // 以前の文章が存在するなら出力する
                        if (strOutput != string.Empty)
                        {
                            this.txtMain.Inlines.Add(strOutput);
                        }

                        // この行以降の色を変えて出力する
                        IsColorChange = true;
                        if (eachLine.StartsWith("#{"))
                        {
                            strOutput = eachLine.Substring(2);
                        }
                        else
                        {
                            strOutput = eachLine;
                        }
                        if (i + 1 < countLines)
                        {
                            strOutput += System.Environment.NewLine;
                        }
                        Run runColor = new Run();
                        runColor.Text = strOutput;
                        runColor.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 200, 0));
                        this.txtMain.Inlines.Add(runColor);

                        strOutput = string.Empty;
                    }
                    // 行頭の「##」は淡い水色
                    else if (eachLine.StartsWith("##"))
                    {
                        // 以前の文章が存在するなら出力する
                        if (strOutput != string.Empty)
                        {
                            this.txtMain.Inlines.Add(strOutput);
                        }

                        // この行だけ色を変えて出力する
                        strOutput = eachLine.Substring(2);
                        if (i + 1 < countLines)
                        {
                            strOutput += System.Environment.NewLine;
                        }
                        Run runColor = new Run();
                        runColor.Text = strOutput;
                        runColor.Foreground = new SolidColorBrush(Color.FromArgb(255, 200, 255, 255));
                        this.txtMain.Inlines.Add(runColor);

                        strOutput = string.Empty;
                    }
                    // 行頭の「#」は黄色
                    else if (eachLine.StartsWith("#"))
                    {
                        // 以前の文章が存在するなら出力する
                        if (strOutput != string.Empty)
                        {
                            this.txtMain.Inlines.Add(strOutput);
                        }

                        // この行だけ色を変えて出力する
                        strOutput = eachLine.Substring(1);
                        if (i + 1 < countLines)
                        {
                            strOutput += System.Environment.NewLine;
                        }
                        Run runColor = new Run();
                        runColor.Text = strOutput;
                        runColor.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 0));
                        this.txtMain.Inlines.Add(runColor);

                        strOutput = string.Empty;
                    }
                    // 通常色
                    else
                    {
                        strOutput += eachLine;
                        if (i + 1 < countLines)
                        {
                            strOutput += System.Environment.NewLine;
                        }
                    }
                }

                // 文章が残ってれば出力する
                if (strOutput != string.Empty)
                {
                    this.txtMain.Inlines.Add(strOutput);
                }
            }
            else
            {
                // そのまま表示する
                this.txtMain.Text = strInput;
            }
        }

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
                    this.imgFaceLeft.Source = bitimg1;
                    this.imgFaceLeft.Visibility = Visibility.Visible;

                    // 枠を表示して間隔を空ける
                    borderLeft.Visibility = Visibility.Visible;
                    this.txtMain.Margin = new Thickness(130, 10, 15, 15);
                }
            }
        }

        // 文章の左側の顔絵を取り除く
        public void RemoveFace()
        {
            // 顔絵のファイルを読み込む
            if (this.imgFaceLeft.Source != null)
            {
                this.imgFaceLeft.Source = null;
                this.imgFaceLeft.Visibility = Visibility.Collapsed;

                // 枠を隠す
                borderLeft.Visibility = Visibility.Collapsed;
                this.txtMain.Margin = new Thickness(15, 10, 15, 15);
            }
        }

        #endregion

    }
}
