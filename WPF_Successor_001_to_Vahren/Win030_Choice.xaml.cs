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
選択肢ダイアログの使い方

まずは、初期化します。
var dialog = new Win030_Choice();

選択肢のリストを指定します。
項目数や長さによってウインドウは自動的に大きくなります。
dialog.SetList(List<string> ChoiceList);

タイトルを指定することもできます。
dialog.SetTitle("どれにしますか？");

ダイアログを表示してる間、親ウインドウへ入力できなくなります。
dialog.ShowDialog();

int型で選択した番号を取得します。
int result = dialog.ChoiceNumber;

キー入力で選択することも可能です。
0 ~ 9 までの数字キーに対応してます。
（選択肢が10個を超えるとキー入力では選択できません。）

*/
namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// Win030_Choice.xaml の相互作用ロジック
    /// </summary>
    public partial class Win030_Choice : Window
    {
        public Win030_Choice()
        {
            InitializeComponent();

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 親ウインドウをセットする
            this.Owner = mainWindow;

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

            // 縦横両方とも中央位置にする
            double newLeft = mainLeft + mainWidth / 2 - this.ActualWidth / 2;
            double newTop = mainTop + mainHeight / 2 - this.ActualHeight / 2;

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
            int input_num = -1;

            // 数字キー
            if ((e.Key == Key.D0) || (e.Key == Key.NumPad0))
            {
                // キーを押しっぱなしにしても無視する
                if (e.IsRepeat == false)
                {
                    // 選択肢が無い状態でも 0キーだけは常に受け付ける
                    this.ChoiceNumber = 0;

                    // 戻り値をセットすると自動的に閉じる
                    DialogResult = true;
                }
            }
            else if ((e.Key == Key.D1) || (e.Key == Key.NumPad1))
            {
                input_num = 1;
            }
            else if ((e.Key == Key.D2) || (e.Key == Key.NumPad2))
            {
                input_num = 2;
            }
            else if ((e.Key == Key.D3) || (e.Key == Key.NumPad3))
            {
                input_num = 3;
            }
            else if ((e.Key == Key.D4) || (e.Key == Key.NumPad4))
            {
                input_num = 4;
            }
            else if ((e.Key == Key.D5) || (e.Key == Key.NumPad5))
            {
                input_num = 5;
            }
            else if ((e.Key == Key.D6) || (e.Key == Key.NumPad6))
            {
                input_num = 6;
            }
            else if ((e.Key == Key.D7) || (e.Key == Key.NumPad7))
            {
                input_num = 7;
            }
            else if ((e.Key == Key.D8) || (e.Key == Key.NumPad8))
            {
                input_num = 8;
            }
            else if ((e.Key == Key.D9) || (e.Key == Key.NumPad9))
            {
                input_num = 9;
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

            // 選択肢が存在する番号だけ返す
            if ((input_num > 0) && (input_num < this.panelList.Children.Count))
            {
                // キーを押しっぱなしにしても無視する
                if (e.IsRepeat == false)
                {
                    this.ChoiceNumber = input_num;

                    // 戻り値をセットすると自動的に閉じる
                    DialogResult = true;
                }
            }
        }
        #endregion

        // 文章を指定する
        public void SetTitle(string strInput)
        {
            // タイトル文章
            this.txtTitle.Text = strInput;
        }

        // 選択されたボタンの番号 0,1,2,~
        public int ChoiceNumber { get; set; }

        // 選択肢を設定する
        public void SetList(List<string> ChoiceList)
        {
            const int button_height = 45;

            // 既に存在する選択肢を消去する
            this.panelList.Children.Clear();
            if (ChoiceList.Count == 0)
            {
                // 選択肢が無くてもエラーにしない
                Button btnItem = new Button();
                btnItem.Content = "選択肢を設定してください";
                btnItem.FontSize = 20;
                btnItem.Height = button_height;
                btnItem.Width = (btnItem.FontSize) * 12 + 30 + 4;
                btnItem.Focusable = false;
                btnItem.Tag = 0;
                btnItem.Click += btnItem_Click;
                this.panelList.Children.Add(btnItem);
                this.scrollList.Height = button_height;
                return;
            }

            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // ボタンの幅を文字数によって変える
            int max_length = 1;
            foreach (var strLine in ChoiceList)
            {
                if (max_length < strLine.Length)
                {
                    max_length = strLine.Length;
                }
            }

            // ボタンを追加していく
            int i;
            for (i = 0; i < ChoiceList.Count; i++)
            {
                Button btnItem = new Button();
                btnItem.Content = ChoiceList[i];
                btnItem.FontSize = 20;
                btnItem.Height = button_height;
                // 文字列の左右に 15 pixel ずつ隙間を空ける。枠が 2 pixel ずつ。
                btnItem.Width = (btnItem.FontSize) * max_length + 30 + 4;
                btnItem.Margin = new Thickness(10,10,10,10);
                btnItem.Focusable = false;
                btnItem.Tag = i;
                btnItem.Click += btnItem_Click;
                this.panelList.Children.Add(btnItem);

                // ボタンの背景
                mainWindow.SetButtonImage(btnItem, "wnd5.png");
            }

            // 一度に表示するのは 6個まで
            if (i > 6)
            {
                i = 6;
            }
            this.scrollList.Height = (button_height + 20) * i;
            // ウインドウが縦長にならないようにする
            this.gridMain.MinWidth = (button_height + 20) * i + 40;
        }

        private void btnItem_Click(object sender, RoutedEventArgs e)
        {
            this.ChoiceNumber = Convert.ToInt32(((Button)sender).Tag);

            // 戻り値をセットすると自動的に閉じる
            DialogResult = true;
        }

    }
}
