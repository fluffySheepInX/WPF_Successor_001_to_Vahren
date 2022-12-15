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

namespace WPF_Successor_001_to_Vahren
{
    /// <summary>
    /// UserControl015_Unit.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl015_Unit : UserControl
    {
        public UserControl015_Unit()
        {
            InitializeComponent();
        }

        // 最初に呼び出した時
        private bool _isControl = false; // 操作可能かどうかの設定
        public void SetData()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            ClassCityAndUnit classCityAndUnit = (ClassCityAndUnit)this.Tag;

            // 最前面に配置する
            var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
            if ( (listWindow != null) && (listWindow.Any()) )
            {
                int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                Canvas.SetZIndex(this, maxZ + 1);
            }

            // プレイヤーが操作可能かどうか
            if (classCityAndUnit.ClassPowerAndCity.ClassPower.NameTag == mainWindow.ClassGameStatus.SelectionPowerAndCity.ClassPower.NameTag)
            {
                // 同じ勢力なら、操作できる
                _isControl = true;
            }
            else
            {
                _isControl = false;
            }

            // ユニットの情報を表示する
            DisplayUnitStatus(mainWindow);

            // ボタンの背景
            if (_isControl)
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
                    this.btnDismiss.Background = myImageBrush;
                    this.btnDismiss.Foreground = Brushes.White;
                    this.btnDismiss.BorderBrush = Brushes.Silver;
                    this.btnMercenary.Background = myImageBrush;
                    this.btnMercenary.Foreground = Brushes.White;
                    this.btnMercenary.BorderBrush = Brushes.Silver;
                    this.btnItem.Background = myImageBrush;
                    this.btnItem.Foreground = Brushes.White;
                    this.btnItem.BorderBrush = Brushes.Silver;
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

        // 既に表示されていて、表示を更新する際
        public void DisplayUnitStatus(MainWindow mainWindow)
        {
            ClassCityAndUnit classCityAndUnit = (ClassCityAndUnit)this.Tag;
            ClassPower targetPower = classCityAndUnit.ClassPowerAndCity.ClassPower;
            if (classCityAndUnit.ClassUnit == null)
            {
                throw new Exception();
            }
            ClassUnit targetUnit = classCityAndUnit.ClassUnit;

            // 部隊内でのインデックスを調べる
            // 放浪人材は陪臣の人数を表示する
            int member_id = 0, member_count = 0;
            var listTroop = classCityAndUnit.ClassPowerAndCity.ClassSpot.UnitGroup;
            foreach (var itemTroop in listTroop)
            {
                member_id = 0;
                foreach (var itemUnit in itemTroop.ListClassUnit)
                {
                    if (itemUnit == targetUnit)
                    {
                        member_count = itemTroop.ListClassUnit.Count;
                        break;
                    }
                    member_id++;
                }
                if (member_count > 0)
                {
                    break;
                }
            }

            // プレイヤーが操作可能かどうか
            if (_isControl == false)
            {
                // 異なる勢力なら、操作ボタンを隠す
                btnDismiss.Visibility = Visibility.Hidden;
                btnMercenary.Visibility = Visibility.Hidden;
                btnItem.Visibility = Visibility.Hidden;
            }

            // ユニット画像は戦場での大きさで表示したい、けど、サイズのデータがClassUnitにない
            if (targetUnit.Image != string.Empty)
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("040_ChipImage");
                strings.Add(targetUnit.Image);
                string path = System.IO.Path.Combine(strings.ToArray());
                BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                this.imgUnit.Width = bitimg1.PixelWidth;
                this.imgUnit.Height = bitimg1.PixelHeight;
                this.imgUnit.Source = bitimg1;
            }
            // ユニット顔絵は大きくてもいい
            if (targetUnit.Face != string.Empty)
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("010_FaceImage");
                strings.Add(targetUnit.Face);
                string path = System.IO.Path.Combine(strings.ToArray());
                if (System.IO.File.Exists(path))
                {
                    BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                    // 大きな顔絵ならウインドウの半分程度に制限する
                    if (bitimg1.PixelWidth > 96)
                    {
                        this.imgFace.Height = bitimg1.PixelHeight;
                        if (this.imgFace.Height > 256)
                        {
                            this.imgFace.Height = 256;
                        }
                        this.imgFace.Width = bitimg1.PixelWidth;
                        if (this.imgFace.Width > 256)
                        {
                            this.imgFace.Width = 256;
                        }
                    }
                    this.imgFace.Source = bitimg1;
                }
            }
            // 旗は存在する時だけ
            if (targetPower.FlagPath != string.Empty)
            {
                List<string> strings = new List<string>();
                strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                strings.Add("030_FlagImage");
                strings.Add(targetPower.FlagPath);
                string path = System.IO.Path.Combine(strings.ToArray());
                BitmapImage bitimg1 = new BitmapImage(new Uri(path));
                Int32Rect rect = new Int32Rect(0, 0, 32, 32);
                var destimg = new CroppedBitmap(bitimg1, rect);
                this.imgFlag.Source = destimg;
            }

            // ユニット名
            {
                this.txtNameUnit.Text = targetUnit.Name;
            }
            // 種族
            if (targetUnit.Race != string.Empty){
                this.txtRace.Text = "（" + targetUnit.Race + "）";
            }
            // レベルとクラス
            {
                //this.txtClass.Text = this.Name; // ウインドウ番号を表示する実験用
                this.txtLevelClass.Text = "Lv" + targetUnit.Level.ToString() + " " + targetUnit.Class;
            }

            // 経験値
            {
                int required_exp = targetUnit.Exp;   // レベルアップに必要な経験値
                int level_up = targetUnit.Level - 1; // レベルアップ回数
                while (level_up > 0)
                {
                    required_exp = targetUnit.Exp + required_exp * targetUnit.Exp_mul / 100;
                    level_up--;
                }
                this.txtExp.Text = "EXP 0/" + required_exp.ToString();
            }
            // 戦功値
            if ( (targetPower.NameTag == string.Empty) || (targetUnit.NameTag == targetPower.MasterTag) )
            {
                // 中立ユニットやマスターは戦功値を表示しない
                this.txtMerits.Text = string.Empty;
            }
            else
            {
                this.txtMerits.Text = "Member = " + member_id.ToString() + "/" + member_count; // メンバー番号を表示する実験用
            }

            // 所持金
            {
                this.txtMoney.Text = "ID = " + targetUnit.ID.ToString(); // ユニット番号を表示する実験用
            }
            // 維持費
            if (targetUnit.NameTag == targetPower.MasterTag)
            {
                // マスターは維持費がかからない
                this.txtCost.Text = string.Empty;
            }
            else
            {
                int actual_cost = targetUnit.Cost;
                if (actual_cost < 0)
                {
                    // 維持費がマイナスの場合はゼロと表示する
                    actual_cost = 0;
                }
                // 身分によって維持費が変動することに注意
                this.txtCost.Text = "維持費 " + actual_cost.ToString();
            }

            // 人材の時だけ項目を表示する
            if (targetUnit.Talent == "on")
            {
                // マスターなら忠誠ではなく信用度を表示する
                if (targetUnit.NameTag == targetPower.MasterTag)
                {
                    this.txtRank.Text = "マスター";
                    this.txtLoyal.Text = "信用度 ?";
                }
                else
                {
                    this.txtRank.Text = "一般";
                    this.txtLoyal.Text = "忠誠 " + targetUnit.Loyal.ToString();
                }
            }
            else
            {
                // 一般兵は表示しない
                this.txtRank.Text = string.Empty;
                this.txtLoyal.Text = string.Empty;
            }

            // 能力値
            {
                this.txtMoveType.Text = targetUnit.MoveType;

                // スキルで増減した補正値と、本来の値の２種類を表示する
                this.txtHP.Text = targetUnit.Hp.ToString() + "/" + targetUnit.Hp.ToString();
                this.txtMP.Text = targetUnit.Mp.ToString() + "/" + targetUnit.Mp.ToString();
                this.txtAttack.Text = targetUnit.Attack.ToString() + "/" + targetUnit.Attack.ToString();
                this.txtDefense.Text = targetUnit.Defense.ToString() + "/" + targetUnit.Defense.ToString();
                this.txtMagic.Text = targetUnit.Magic.ToString() + "/" + targetUnit.Magic.ToString();
                this.txtMagDef.Text = targetUnit.MagDef.ToString() + "/" + targetUnit.MagDef.ToString();
                this.txtSpeed.Text = targetUnit.Speed.ToString() + "/" + targetUnit.Speed.ToString();
                this.txtDext.Text = targetUnit.Dext.ToString() + "/" + targetUnit.Dext.ToString();
                this.txtHPRec.Text = targetUnit.Hprec.ToString() + "/" + targetUnit.Hprec.ToString();
                this.txtMPRec.Text = targetUnit.Mprec.ToString() + "/" + targetUnit.Mprec.ToString();
                this.txtMove.Text = targetUnit.Move.ToString() + "/" + targetUnit.Move.ToString();
                this.txtSummon.Text = targetUnit.Summon_max.ToString() + "/" + targetUnit.Summon_max.ToString();

                // 維持費がマイナスなら財政値になる
                int actual_finance = targetUnit.Finance;
                if (targetUnit.Cost < 0)
                {
                    actual_finance -= targetUnit.Cost;
                }
                this.txtFinance.Text = actual_finance.ToString();
            }

            // スキル
            this.panelSkill.Children.Clear(); // 最初に全て消去する
            foreach (var itemSkill in targetUnit.Skill)
            {
                // スキルのアイコンを重ねて表示する
                Grid gridSkill = new Grid();
                foreach (var itemIcon in Enumerable.Reverse(itemSkill.Icon).ToList())
                {
                    List<string> strings = new List<string>();
                    strings.Add(mainWindow.ClassConfigGameTitle.DirectoryGameTitle[mainWindow.NowNumberGameTitle].FullName);
                    strings.Add("041_ChipImageSkill");
                    strings.Add(itemIcon);
                    string path = System.IO.Path.Combine(strings.ToArray());
                    if (System.IO.File.Exists(path) == false)
                    {
                        // スキル画像が存在しない場合はスキップする
                        continue;
                    }
                    var bitmap = new BitmapImage(new Uri(path));
                    Image imageSkill = new Image();
                    imageSkill.Source = bitmap;
                    // 小さな画像はそのままのサイズで表示する
                    if ((bitmap.PixelWidth < 32) && (bitmap.PixelHeight < 32))
                    {
                        imageSkill.Width = bitmap.PixelWidth;
                        imageSkill.Height = bitmap.PixelHeight;
                    }
                    else
                    {
                        imageSkill.Width = 32;
                        imageSkill.Height = 32;
                    }
                    gridSkill.Children.Add(imageSkill);
                }

                // ボタンの内枠の分だけサイズを画像よりも大きくする
                Button buttonSkill = new Button();
                buttonSkill.Content = gridSkill;
                buttonSkill.Tag = itemSkill;
                buttonSkill.Width = 34;
                buttonSkill.Height = 34;
                buttonSkill.Background = Brushes.Black;
                buttonSkill.BorderThickness = new Thickness(0,0,0,0);
                buttonSkill.Focusable = false;
                buttonSkill.Click += btnSkill_Click;
                buttonSkill.MouseEnter += btnSkill_MouseEnter;
                buttonSkill.PreviewMouseLeftButtonDown += Raise_ZOrder;
                this.panelSkill.Children.Add(buttonSkill);
            }

            // 耐性
            this.panelResist.Children.Clear(); // 最初に全て消去する


        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 雇用ウインドウを開いてた場合は閉じる
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl020_Mercenary>())
            {
                if (itemWindow.Name == this.Name + "Mercenary")
                {
                    mainWindow.canvasUI.Children.Remove(itemWindow);
                    break;
                }
            }

            // キャンバスから自身を取り除く
            mainWindow.canvasUI.Children.Remove(this);
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
                if ( (listWindow != null) && (listWindow.Any()) )
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
                thickness.Left = Math.Truncate(this.Margin.Left + (pt.X - _startPoint.X));
                thickness.Top = Math.Truncate(this.Margin.Top + (pt.Y - _startPoint.Y));
                this.Margin = thickness;
            }
        }
        #endregion

        // ボタン等を右クリックした際に、親コントロールが反応しないようにする
        private void Disable_MouseEvent(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow != null)
            {
                // 最前面に移動させる
                var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
                if ( (listWindow != null) && (listWindow.Any()) )
                {
                    int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                    Canvas.SetZIndex(this, maxZ + 1);
                }
            }

            // ルーティングを処理済みとしてマークする（親コントロールのイベントが発生しなくなる）
            e.Handled = true;
        }

        // ボタン等をクリックした際に、UserControlを最前面に移動させる
        private void Raise_ZOrder(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow != null)
            {
                // 最前面に移動させる
                var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
                if ( (listWindow != null) && (listWindow.Any()) )
                {
                    int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                    Canvas.SetZIndex(this, maxZ + 1);
                }
            }
        }

        // ユニット情報ウインドウにカーソルを乗せた時
        private void win_MouseEnter(object sender, MouseEventArgs e)
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
            helpWindow.SetData("ウィンドウ内を右クリックするとウィンドウを閉じます。");
            mainWindow.canvasUI.Children.Add(helpWindow);

            // スキルのヒントが表示されてる時はヘルプを隠す
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl046_SkillHint>())
            {
                if (itemWindow.Name == "SkillHint")
                {
                    helpWindow.Visibility = Visibility.Hidden;
                    break;
                }
            }
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


        // ユニットの雇用ウインドウを開く
        private void btnMercenary_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // ユニットから雇用なので、ClassUnit 部分を null にしない
            ClassCityAndUnit classCityAndUnit = (ClassCityAndUnit)this.Tag;

            // ユニット情報ウインドウの右横に雇用ウインドウを表示する
            double offsetLeft = this.Margin.Left + this.ActualWidth;

            // 既に雇用ウインドウが表示されてる場合は再利用する
            bool isFound = false;
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl020_Mercenary>())
            {
                string strTitle = itemWindow.Name;
                if ( (strTitle.StartsWith("WindowSpot")) || (strTitle.StartsWith("WindowUnit")) )
                {
                    // 新規に作らない
                    itemWindow.Tag = classCityAndUnit;
                    itemWindow.Name = this.Name + "Mercenary";
                    if (this.Margin.Left + this.ActualWidth / 2  > mainWindow.CanvasMainWidth / 2)
                    {
                        // 画面の右側なら、左横に表示する
                        offsetLeft = this.Margin.Left - itemWindow.MinWidth;
                    }
                    itemWindow.Margin = new Thickness()
                    {
                        Left = offsetLeft,
                        Top = this.Margin.Top
                    };
                    itemWindow.DisplayMercenary(mainWindow);

                    // 雇用ウインドウをこのウインドウよりも前面に移動させる
                    Canvas.SetZIndex(itemWindow, Canvas.GetZIndex(this) + 1);

                    isFound = true;
                    break;
                }
            }
            if (isFound == false)
            {
                // 新規に作成する
                var windowMercenary = new UserControl020_Mercenary();
                windowMercenary.Tag = classCityAndUnit;
                windowMercenary.Name = this.Name + "Mercenary";
                if (this.Margin.Left + this.ActualWidth / 2 > mainWindow.CanvasMainWidth / 2)
                {
                    // 画面の右側なら、左横に表示する
                    offsetLeft = this.Margin.Left - windowMercenary.MinWidth;
                }
                windowMercenary.Margin = new Thickness()
                {
                    Left = offsetLeft,
                    Top = this.Margin.Top
                };
                windowMercenary.SetData();
                mainWindow.canvasUI.Children.Add(windowMercenary);

                // 親ウインドウから出てくるように見せる
                double offsetFrom = this.Margin.Left;
                if (offsetLeft > offsetFrom)
                {
                    offsetFrom = offsetLeft - windowMercenary.MinWidth;
                }
                var animeMargin = new ThicknessAnimation();
                animeMargin.From = new Thickness()
                {
                    Left = offsetFrom,
                    Top = this.Margin.Top
                };
                animeMargin.Duration = new Duration(TimeSpan.FromSeconds(0.25));
                windowMercenary.BeginAnimation(Grid.MarginProperty, animeMargin);
                var animeOpacity = new DoubleAnimation();
                animeOpacity.From = 0.1;
                animeOpacity.Duration = new Duration(TimeSpan.FromSeconds(0.2));
                windowMercenary.BeginAnimation(Grid.OpacityProperty, animeOpacity);
            }
        }

        // 雇用ボタンにカーソルを乗せた時
        private void btnMercenary_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // カーソルを離した時のイベントを追加する
            var cast = (UIElement)sender;
            cast.MouseLeave += btnMercenary_MouseLeave;

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
            helpWindow.Name = "Help_" + this.Name + "_btnMercenary";
            helpWindow.SetData("ユニットの雇用ウィンドウを表示します。");
            mainWindow.canvasUI.Children.Add(helpWindow);
        }
        private void btnMercenary_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // イベントを取り除く
            var cast = (UIElement)sender;
            cast.MouseLeave -= btnMercenary_MouseLeave;

            // 表示中のヘルプを取り除く
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if (itemHelp.Name == "Help_" + this.Name + "_btnMercenary")
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

        // スキル情報ウインドウを開く
        private void btnSkill_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var cast = (Button)sender;
            if (cast.Tag is not ClassSkill)
            {
                return;
            }

            // スキルのヒントが表示されてる場合は閉じる
            bool bCloseHint = false;
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl046_SkillHint>())
            {
                if (itemWindow.Name == "SkillHint")
                {
                    mainWindow.canvasUI.Children.Remove(itemWindow);
                    bCloseHint = true;
                    break;
                }
            }
            if (bCloseHint)
            {
                // ヘルプを隠してた場合は、最前面のヘルプだけ表示する
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

            // ウインドウ番号によって表示位置を変える
            double offsetLeft = 0, offsetTop = 0;
            if (mainWindow.canvasUI.Margin.Left < 0)
            {
                offsetLeft = mainWindow.canvasUI.Margin.Left * -1;
            }
            if (mainWindow.canvasUI.Margin.Top < 0)
            {
                offsetTop = mainWindow.canvasUI.Margin.Top * -1;
            }

            // 既に表示されてるユニット・ウインドウをチェックする
            const int dY = 60, dX = 40, dX2 = 120, dZ = 8;
            int window_id, max_id = 0;
            var id_list = new List<int>();
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl045_Skill>())
            {
                string strTitle = itemWindow.Name;
                if (strTitle.StartsWith("WindowSkill"))
                {
                    window_id = Int32.Parse(strTitle.Replace("WindowSkill", String.Empty));
                    id_list.Add(window_id);
                    if (max_id < window_id)
                    {
                        max_id = window_id;
                    }
                    var testSkill = (ClassSkill)itemWindow.Tag;
                    if (testSkill == cast.Tag)
                    {
                        // スキル情報ウインドウを既に開いてる場合は、新規に作らない
                        max_id = -1;
                        itemWindow.Margin = new Thickness()
                        {
                            Left = offsetLeft + ((window_id - 1) % dZ) * dX + ((window_id - 1) / dZ) * dX2,
                            Top = offsetTop + ((window_id - 1) % dZ) * dY
                        };

                        // スキル情報ウインドウをこのウインドウよりも前面に移動させる
                        Canvas.SetZIndex(itemWindow, Canvas.GetZIndex(this) + 1);

                        break;
                    }
                }
            }
            if (max_id >= 0)
            {
                if (max_id > id_list.Count)
                {
                    // ウインドウ個数よりも最大値が大きいなら、未使用の番号を使って作成する
                    for (window_id = 1; window_id < max_id; window_id++)
                    {
                        if (id_list.Contains(window_id) == false)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    // 使用中のウインドウ番号の最大値 + 1 にして、新規に作成する
                    window_id = max_id + 1;
                }
                var windowSkill = new UserControl045_Skill();
                windowSkill.Tag = cast.Tag;
                windowSkill.Name = "WindowSkill" + window_id.ToString();
                windowSkill.Margin = new Thickness()
                {
                    Left = offsetLeft + ((window_id - 1) % dZ) * dX + ((window_id - 1) / dZ) * dX2,
                    Top = offsetTop + ((window_id - 1) % dZ) * dY
                };
                windowSkill.SetData();
                mainWindow.canvasUI.Children.Add(windowSkill);
            }
            id_list.Clear();
        }

        // スキルのボタンにマウスを乗せた時
        private void btnSkill_MouseEnter(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var cast = (Button)sender;
            if (cast.Tag is not ClassSkill)
            {
                return;
            }

            // マウスを離した時のイベントを追加する
            cast.MouseLeave += btnSkill_MouseLeave;

            // 場所が重なるのでヘルプを全て隠す
            foreach (var itemHelp in mainWindow.canvasUI.Children.OfType<UserControl030_Help>())
            {
                if ((itemHelp.Visibility == Visibility.Visible) && (itemHelp.Name.StartsWith("Help_") == true))
                {
                    itemHelp.Visibility = Visibility.Hidden;
                }
            }

            // スキルのヒントを表示する
            var hintSkill = new UserControl046_SkillHint();
            hintSkill.Tag = cast.Tag;
            hintSkill.Name = "SkillHint";
            hintSkill.SetData();
            mainWindow.canvasUI.Children.Add(hintSkill);
        }
        private void btnSkill_MouseLeave(object sender, MouseEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            var cast = (Button)sender;
            if (cast.Tag is not ClassSkill)
            {
                return;
            }

            // イベントを取り除く
            cast.MouseLeave -= btnSkill_MouseLeave;

            // スキルのヒントを閉じる
            foreach (var itemWindow in mainWindow.canvasUI.Children.OfType<UserControl046_SkillHint>())
            {
                if (itemWindow.Name == "SkillHint")
                {
                    mainWindow.canvasUI.Children.Remove(itemWindow);
                    break;
                }
            }

            // ヘルプを隠してた場合は、最前面のヘルプだけ表示する
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

    }
}
