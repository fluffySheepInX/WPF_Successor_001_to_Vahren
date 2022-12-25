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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_Successor_001_to_Vahren._005_Class;

/*
キー入力でクリックを代用することも可能です。
Enter, Space, Z キーを押すとクリックします。
Ctrl キーを押し続けると連続的にクリックします。


文章中の特殊記号やタグの説明

文章に @ や # が含まれてると、そこまで表示して待機します。
@	クリック待ちして改行する
#	クリック待ちして改行しない

ルビ文字は HTML と同じタグで指定します。
<ruby>難しい漢字<rt>よみがな</rt></ruby>

*/
namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// UserControl050_Msg.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl050_Msg : UserControl
    {
        public UserControl050_Msg()
        {
            InitializeComponent();

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 最前面に配置する（エフェクト用のfadeよりも前）
            Canvas.SetZIndex(this, 101);

            // 次アイコン
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("006_WindowImage");
                strings.Add("nextmsg.png");
                string path = System.IO.Path.Combine(strings.ToArray());
                if (System.IO.File.Exists(path))
                {
                    // 画像が存在する時だけアイコンを表示する
                    BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                    this.imgNext.Width = bitimg1.PixelWidth;
                    this.imgNext.Height = bitimg1.PixelHeight;
                    this.imgNext.Source = bitimg1;

                    // 上下に動くアニメーション
                    var animeMargin = new ThicknessAnimation();
                    animeMargin.From = new Thickness()
                    {
                        Bottom = this.imgNext.Height / 2
                    };
                    animeMargin.Duration = new Duration(TimeSpan.FromSeconds(0.5));
                    animeMargin.AutoReverse = true;
                    animeMargin.RepeatBehavior = RepeatBehavior.Forever;
                    this.imgNext.BeginAnimation(Image.MarginProperty, animeMargin);
                }
            }

            // ウインドウ枠
            SetWindowFrame2(mainWindow);

            // 後ろのコントロールに入力できないよう、画面全体に広げる
            this.Width = mainWindow.canvasTop.ActualWidth;
            this.Height = mainWindow.canvasTop.ActualHeight;

            /*
            // テキストウィンドウを画面の下中央に配置する
            double offsetTop = mainWindow.canvasUI.Margin.Top;
            if (offsetTop < 0)
            {
                offsetTop = 0;
            }
            this.gridMain.Margin = new Thickness()
            {
                Left = Math.Truncate(this.Width / 2 - this.gridMain.Width / 2),
                Top = this.Height - this.gridMain.Height - offsetTop
            };
            */
        }

        // ウインドウ枠を作る
        private void SetWindowFrame2(MainWindow mainWindow)
        {
            // ウインドウスキンを読み込む
            List<string> strings = new List<string>();
            strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
            strings.Add("006_WindowImage");
            strings.Add("wnd4.png");
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
            this.rectShadowRight2.Visibility = Visibility.Hidden;
            this.rectShadowBottom.Visibility = Visibility.Hidden;

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

        // マウスの左クリック
        private void win_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // 次の文章を表示した場合はまだ終わらない
            if (this.NextText() == true)
            {
                return;
            }

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // カウンターが 0よりも多い時だけ減らす
            if (mainWindow.condition.IsSet == false)
            {
                mainWindow.condition.Signal();
            }
        }

        // ウィンドウ位置を変更できるようにする？
        // ヴァーレンの msg2 / talk2 / chat2 は上端に表示される
        public void PositionBottom()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // テキストウィンドウを画面の下中央に配置する
            double offsetTop = mainWindow.canvasUI.Margin.Top;
            if (offsetTop < 0)
            {
                offsetTop = 0;
            }
            double newLeft = Math.Truncate(this.Width / 2 - this.gridMain.Width / 2);
            double newTop = this.Height - this.gridMain.Height - offsetTop;
            // 違う場所にあった場合だけ、アニメーションしながら出現させる
            if ((this.gridMain.Margin.Left != newLeft) || (this.gridMain.Margin.Top != newTop))
            {
                this.gridMain.Margin = new Thickness()
                {
                    Left = Math.Truncate(this.Width / 2 - this.gridMain.Width / 2),
                    Top = this.Height - this.gridMain.Height - offsetTop
                };

                // 画面下から出てくるアニメーションを付ける
                // 文章が表示されたまま動くと変なので、アニメーションが終わってから表示する
                this.txtMain.Visibility = Visibility.Hidden;

                // かなり速く動く（1/8秒で終わる）
                var animeOpacity = new DoubleAnimation();
                animeOpacity.From = 0.1;
                animeOpacity.Duration = new Duration(TimeSpan.FromSeconds(0.11));
                this.gridMain.BeginAnimation(Grid.OpacityProperty, animeOpacity);

                var animeMargin = new ThicknessAnimation();
                animeMargin.From = new Thickness()
                {
                    Left = this.gridMain.Margin.Left,
                    Top = this.gridMain.Margin.Top + Math.Truncate(this.gridMain.Height / 2)
                };
                animeMargin.Duration = new Duration(TimeSpan.FromSeconds(0.125));
                animeMargin.Completed += anime_PositionBottom_Completed;
                this.gridMain.BeginAnimation(Grid.MarginProperty, animeMargin);
            }
        }

        private void anime_PositionBottom_Completed(object? sender, EventArgs e)
        {
            // ウィンドウを動かすアニメーションを消して、文章を表示する
            this.gridMain.BeginAnimation(Grid.OpacityProperty, null);
            this.gridMain.BeginAnimation(Grid.MarginProperty, null);
            this.txtMain.Visibility = Visibility.Visible;
        }

        // 文章の表示を待ってる位置 (マイナスなら待ってない)
        private int _indexWait = -1;
        // 表示待ちの残り文章
        private string _strRemain = string.Empty;

        // 文章を指定する
        public void SetText(string strInput)
        {
            // 特殊記号を探す
            char[] charsToWait = { '@', '#' };
            _indexWait = strInput.IndexOfAny(charsToWait);

            // ルビ文字のタグを探す
            string strOutput, strRubyBottom, strRubyTop;
            int indexRuby = strInput.IndexOf("<ruby>");
            int indexRuby2 = strInput.IndexOf("</ruby>");
            int indexRT = strInput.IndexOf("<rt>");
            int indexRT2 = strInput.IndexOf("</rt>");

            // まずは Text を空にする
            this.txtMain.Text = string.Empty;
            this.txtMain.Inlines.Clear();

            // ルビ文字のタグが存在するか調べる
            if ((indexRuby >= 0) && (indexRT > indexRuby) && (indexRT2 > indexRT) && (indexRuby2 > indexRT2))
            {
                // ルビ文字のタグが存在する限り続行する
                _strRemain = strInput;
                while ((indexRuby >= 0) && (indexRT > indexRuby) && (indexRT2 > indexRT) && (indexRuby2 > indexRT2))
                {
                    // 特殊記号がルビ文字までに存在する場合は、そこまで表示して待機する
                    if ((_indexWait >= 0) && (_indexWait < indexRuby))
                    {
                        // 待機文字まで表示する
                        strOutput = _strRemain.Substring(0, _indexWait);
                        // 「@」で待つ場合は改行を挿入する
                        if (_strRemain[_indexWait] == '@')
                        {
                            strOutput += System.Environment.NewLine;
                        }
                        this.txtMain.Inlines.Add(strOutput);

                        // 残りの文章を記録しておく
                        _strRemain = _strRemain.Substring(_indexWait + 1);
                        return;
                    }

                    // ルビ文字まで表示する
                    strOutput = _strRemain.Substring(0, indexRuby);
                    this.txtMain.Inlines.Add(strOutput);

                    // ルビを付ける文字
                    strRubyBottom = _strRemain.Substring(indexRuby + 6, indexRT - indexRuby - 6);
                    // ルビの文字
                    strRubyTop = _strRemain.Substring(indexRT + 4, indexRT2 - indexRT - 4);
                    // 残りの文章
                    _strRemain = _strRemain.Substring(indexRuby2 + 7);

                    // ルビの部分を２行にする
                    Run runRuby = new Run();
                    runRuby.Text = strRubyTop + System.Environment.NewLine;
                    // ルビ文字のフォントサイズを本文の半分にする
                    runRuby.FontSize = this.txtMain.FontSize / 2;
                    runRuby.Typography.Variants = FontVariants.Ruby;
                    TextBlock txtBody = new TextBlock();
                    // 本文の少し上にルビ文字を置く
                    txtBody.LineHeight = this.txtMain.FontSize + 2;
                    txtBody.TextAlignment = TextAlignment.Center;
                    txtBody.Inlines.Add(runRuby);
                    txtBody.Inlines.Add(strRubyBottom);
                    this.txtMain.Inlines.Add(txtBody);

                    // 次の特殊記号を探す
                    _indexWait = _strRemain.IndexOfAny(charsToWait);

                    // 次のルビ文字のタグの位置
                    indexRuby = _strRemain.IndexOf("<ruby>");
                    indexRuby2 = _strRemain.IndexOf("</ruby>");
                    indexRT = _strRemain.IndexOf("<rt>");
                    indexRT2 = _strRemain.IndexOf("</rt>");
                }

                // 文章が残ってれば出力する
                if (_strRemain != string.Empty)
                {
                    // 残りの文章に特殊記号が含まれてるなら、更にクリック待ちする
                    if (_indexWait >= 0)
                    {
                        // 待機文字まで表示する
                        strOutput = _strRemain.Substring(0, _indexWait);
                        // 「@」で待つ場合は改行を挿入する
                        if (_strRemain[_indexWait] == '@')
                        {
                            strOutput += System.Environment.NewLine;
                        }
                        this.txtMain.Inlines.Add(strOutput);

                        // 残りの文章を記録しておく
                        _strRemain = _strRemain.Substring(_indexWait + 1);
                    }
                    else
                    {
                        // 残りの文章を全て表示する
                        this.txtMain.Inlines.Add(_strRemain);
                        _strRemain = string.Empty;
                    }
                }
            }
            // クリック待ちするか調べる
            else if (_indexWait >= 0)
            {
                // 待機文字まで表示する
                strOutput = strInput.Substring(0, _indexWait);
                // 「@」で待つ場合は改行を挿入する
                if (strInput[_indexWait] == '@')
                {
                    strOutput += System.Environment.NewLine;
                }
                this.txtMain.Inlines.Add(strOutput);

                // 残りの文章を記録しておく
                _strRemain = strInput.Substring(_indexWait + 1);
            }
            else
            {
                // そのまま表示する
                this.txtMain.Text = strInput;
                _strRemain = string.Empty;
            }
        }

        // 待機中に、残りの文章を表示する
        public bool NextText()
        {
            // 待機中でなければ false を返して終わる
            if (_indexWait < 0)
            {
                return false;
            }

            // 特殊記号を探す
            char[] charsToWait = { '@', '#' };
            _indexWait = _strRemain.IndexOfAny(charsToWait);

            // ルビ文字のタグを探す
            string strOutput, strRubyBottom, strRubyTop;
            int indexRuby = _strRemain.IndexOf("<ruby>");
            int indexRuby2 = _strRemain.IndexOf("</ruby>");
            int indexRT = _strRemain.IndexOf("<rt>");
            int indexRT2 = _strRemain.IndexOf("</rt>");

            // まだルビ文字のタグが存在するか調べる
            while ((indexRuby >= 0) && (indexRT > indexRuby) && (indexRT2 > indexRT) && (indexRuby2 > indexRT2))
            {
                // 特殊記号がルビ文字までに存在する場合は、そこまで表示して待機する
                if ((_indexWait >= 0) && (_indexWait < indexRuby))
                {
                    // 待機文字まで表示する
                    strOutput = _strRemain.Substring(0, _indexWait);
                    // 「@」で待つ場合は改行を挿入する
                    if (_strRemain[_indexWait] == '@')
                    {
                        strOutput += System.Environment.NewLine;
                    }
                    this.txtMain.Inlines.Add(strOutput);

                    // 残りの文章を記録しておく
                    _strRemain = _strRemain.Substring(_indexWait + 1);
                    return true;
                }

                // ルビ文字まで表示する
                strOutput = _strRemain.Substring(0, indexRuby);
                this.txtMain.Inlines.Add(strOutput);

                // ルビを付ける文字
                strRubyBottom = _strRemain.Substring(indexRuby + 6, indexRT - indexRuby - 6);
                // ルビの文字
                strRubyTop = _strRemain.Substring(indexRT + 4, indexRT2 - indexRT - 4);
                // 残りの文章
                _strRemain = _strRemain.Substring(indexRuby2 + 7);

                // ルビの部分を２行にする
                Run runRuby = new Run();
                runRuby.Text = strRubyTop + System.Environment.NewLine;
                // ルビ文字のフォントサイズを本文の半分にする
                runRuby.FontSize = this.txtMain.FontSize / 2;
                runRuby.Typography.Variants = FontVariants.Ruby;
                TextBlock txtBody = new TextBlock();
                // 本文の少し上にルビ文字を置く
                txtBody.LineHeight = this.txtMain.FontSize + 2;
                txtBody.TextAlignment = TextAlignment.Center;
                txtBody.Inlines.Add(runRuby);
                txtBody.Inlines.Add(strRubyBottom);
                this.txtMain.Inlines.Add(txtBody);

                // 次の特殊記号を探す
                _indexWait = _strRemain.IndexOfAny(charsToWait);

                // 次のルビ文字のタグの位置
                indexRuby = _strRemain.IndexOf("<ruby>");
                indexRuby2 = _strRemain.IndexOf("</ruby>");
                indexRT = _strRemain.IndexOf("<rt>");
                indexRT2 = _strRemain.IndexOf("</rt>");
            }

            // 文章が残ってれば出力する
            if (_strRemain != string.Empty)
            {
                // 残りの文章に特殊記号が含まれてるなら、更にクリック待ちする
                if (_indexWait >= 0)
                {
                    // 待機文字まで表示する
                    strOutput = _strRemain.Substring(0, _indexWait);
                    // 「@」で待つ場合は改行を挿入する
                    if (_strRemain[_indexWait] == '@')
                    {
                        strOutput += System.Environment.NewLine;
                    }
                    this.txtMain.Inlines.Add(strOutput);

                    // 残りの文章を記録しておく
                    _strRemain = _strRemain.Substring(_indexWait + 1);
                }
                else
                {
                    // 残りの文章を全て表示する
                    this.txtMain.Inlines.Add(_strRemain);
                    _strRemain = string.Empty;
                }
            }

            // 次の文章を表示したら true を返す
            return true;
        }

        // 名前を指定する
        public void AddName(string strInput)
        {
            if (strInput == string.Empty)
            {
                this.RemoveName();
                return;
            }

            // 名前
            this.txtName.Text = strInput;
            this.txtName.Visibility = Visibility.Visible;

            // タイトル全体を表示する
            this.gridSub.Visibility = Visibility.Visible;
        }
        public void RemoveName()
        {
            // 名前を消す
            this.txtName.Text = string.Empty;
            this.txtName.Visibility = Visibility.Collapsed;

            // 名前と肩書の両方が空なら、タイトル全体を隠す
            if (this.txtHelp.Text == string.Empty)
            {
                this.gridSub.Visibility = Visibility.Hidden;
            }
        }

        // 肩書を指定する
        public void AddHelp(string strInput)
        {
            if (strInput == string.Empty)
            {
                this.RemoveHelp();
                return;
            }

            // 肩書
            this.txtHelp.Text = strInput;
            this.txtHelp.Visibility = Visibility.Visible;

            // タイトル全体を表示する
            this.gridSub.Visibility = Visibility.Visible;
        }
        public void RemoveHelp()
        {
            // 肩書を消す
            this.txtHelp.Text = string.Empty;
            this.txtHelp.Visibility = Visibility.Collapsed;

            // 名前と肩書の両方が空なら、タイトル全体を隠す
            if (this.txtName.Text == string.Empty)
            {
                this.gridSub.Visibility = Visibility.Hidden;
            }
        }

        // 顔絵を指定する
        public void AddFace(string strFilename)
        {
            if (strFilename == string.Empty)
            {
                this.RemoveFace();
                return;
            }

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 顔絵のファイルを読み込む
            List<string> strings = new List<string>();
            strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
            strings.Add("010_FaceImage");
            strings.Add(strFilename);
            string path = System.IO.Path.Combine(strings.ToArray());
            if (System.IO.File.Exists(path))
            {
                BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                this.imgFace.Source = bitimg1;
                this.imgFace.Visibility = Visibility.Visible;
                this.borderFace.Visibility = Visibility.Visible;

                // 小さな画像はそのままのサイズで表示する
                const int max_size = 192;
                if ((bitimg1.PixelWidth < max_size) && (bitimg1.PixelHeight < max_size))
                {
                    this.imgFace.Width = bitimg1.PixelWidth;
                    this.imgFace.Height = bitimg1.PixelHeight;
                }
                else
                {
                    this.imgFace.Width = max_size;
                }
                this.txtMain.Margin = new Thickness(20, 15, this.imgFace.Width + 4 + 40, 0);
            }
        }
        public void RemoveFace()
        {
            // 顔絵を消す
            this.imgFace.Source = null;
            this.imgFace.Visibility = Visibility.Collapsed;
            this.borderFace.Visibility = Visibility.Collapsed;
            this.txtMain.Margin = new Thickness(20, 15, 20, 0);
        }

    }
}
