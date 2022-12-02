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

            // 最前面に配置する
            var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
            if ( (listWindow != null) && (listWindow.Any()) )
            {
                int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                Canvas.SetZIndex(this, maxZ + 1);
            }
        }


        // 既に表示されていて、表示を更新する際
        public void DisplayUnitStatus(MainWindow mainWindow)
        {
            ClassCityAndUnit classCityAndUnit = (ClassCityAndUnit)this.Tag;
            ClassPower targetPower = classCityAndUnit.ClassPowerAndCity.ClassPower;
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
                // 異なる勢力なら、操作ボタンを無効にする
                btnDismiss.IsEnabled = false;
                btnMercenary.IsEnabled = false;
                btnItem.IsEnabled = false;
            }

            // まだ処理を作ってないのでボタンを無効にする
            btnDismiss.IsEnabled = false;
            btnItem.IsEnabled = false;

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
                Canvas.SetLeft(this.imgFace, this.Width - this.imgFace.Width - 10);
                Canvas.SetTop(this.imgFace, this.Height - this.imgFace.Height - 10);
                this.imgFace.Source = bitimg1;
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
                thickness.Left = this.Margin.Left + (pt.X - _startPoint.X);
                thickness.Top = this.Margin.Top + (pt.Y - _startPoint.Y);
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


        // ユニットの雇用ウインドウを開く
        private void btnMercenary_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 領地から雇用なら、ClassUnit 部分を null にする
            ClassCityAndUnit classCityAndUnit = (ClassCityAndUnit)this.Tag;

            // ユニット情報ウインドウの右横に雇用ウインドウを表示する
            double offsetLeft = this.Margin.Left + this.Width;

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
                    if (this.Margin.Left + this.Width / 2  > mainWindow.CanvasMainWidth / 2)
                    {
                        // 画面の右側なら、左横に表示する
                        offsetLeft = this.Margin.Left - itemWindow.Width;
                    }
                    itemWindow.Margin = new Thickness()
                    {
                        Left = offsetLeft,
                        Top = this.Margin.Top
                    };
                    itemWindow.SetData();

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
                if (this.Margin.Left + this.Width / 2 > mainWindow.CanvasMainWidth / 2)
                {
                    // 画面の右側なら、左横に表示する
                    offsetLeft = this.Margin.Left - windowMercenary.Width;
                }
                windowMercenary.Margin = new Thickness()
                {
                    Left = offsetLeft,
                    Top = this.Margin.Top
                };
                windowMercenary.SetData();
                mainWindow.canvasUI.Children.Add(windowMercenary);
            }
        }

    }
}
