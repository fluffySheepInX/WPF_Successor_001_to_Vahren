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
    /// UserControl016_UnitHint.xaml の相互作用ロジック
    /// </summary>
    public partial class UserControl016_UnitHint : UserControl
    {
        public UserControl016_UnitHint()
        {
            InitializeComponent();
        }

        // 最初に呼び出した時
        public void SetData()
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null)
            {
                return;
            }

            // 最前面に配置する
            var listWindow = mainWindow.canvasUI.Children.OfType<UIElement>().Where(x => x != this);
            if ( (listWindow != null) && (listWindow.Any()) )
            {
                int maxZ = listWindow.Select(x => Canvas.GetZIndex(x)).Max();
                Canvas.SetZIndex(this, maxZ + 1);
            }

            // ユニットの情報を表示する
            DisplayUnitStatus(mainWindow);

            // ウインドウ枠
            SetWindowFrame(mainWindow);

            // 画面の右上隅に配置する
            double offsetLeft = 0, offsetTop = 0;
            if (mainWindow.canvasUI.Margin.Left < 0)
            {
                offsetLeft = mainWindow.canvasUI.Margin.Left * -1;
            }
            if (mainWindow.canvasUI.Margin.Top < 0)
            {
                offsetTop = mainWindow.canvasUI.Margin.Top * -1;
            }
            this.Margin = new Thickness()
            {
                Left = mainWindow.canvasUI.Width - offsetLeft - this.MinWidth,
                Top = offsetTop
            };

            // 透明度を変化させる（移動が終わる前に不透明になる）
            var animeOpacity = new DoubleAnimation();
            animeOpacity.From = 0.1;
            animeOpacity.Duration = new Duration(TimeSpan.FromSeconds(0.2));
            this.BeginAnimation(Grid.OpacityProperty, animeOpacity);

            // 画面の右端から出現する（最初から半分は表示されてる）
            var animeMargin = new ThicknessAnimation();
            animeMargin.From = new Thickness()
            {
                Left = mainWindow.canvasUI.Width - offsetLeft - this.MinWidth / 2,
                Top = offsetTop
            };
            animeMargin.Duration = new Duration(TimeSpan.FromSeconds(0.25));
            this.BeginAnimation(Grid.MarginProperty, animeMargin);
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
                gridSkill.Width = 34;
                gridSkill.Height = 34;
                gridSkill.Background = Brushes.Black;
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

                this.panelSkill.Children.Add(gridSkill);
            }

            // 耐性
            this.panelResist.Children.Clear(); // 最初に全て消去する


        }

    }
}
