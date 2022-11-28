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
            int j, member_id = -1;
            var listTroop = classCityAndUnit.ClassPowerAndCity.ClassSpot.UnitGroup;
            foreach (var itemTroop in listTroop)
            {
                j = 0;
                foreach (var itemUnit in itemTroop.ListClassUnit)
                {
                    if (itemUnit == targetUnit)
                    {
                        member_id = j;
                        break;
                    }
                    j++;
                }
                if (member_id >= 0)
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
                    if (this.imgFace.Height > 200)
                    {
                        this.imgFace.Height = 200;
                    }
                    this.imgFace.Width = bitimg1.PixelWidth;
                    if (this.imgFace.Width > 200)
                    {
                        this.imgFace.Width = 200;
                    }
                }
                this.imgFace.Margin = new Thickness()
                {
                    Left = this.Width - this.imgFace.Width - 10,
                    Top = this.Height - this.imgFace.Height - 10
                };
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
                this.lblNameUnit.Content = targetUnit.Name;
            }
            // クラス
            {
                this.lblClass.Content = this.Name; // ウインドウ番号を表示する実験用
            }
            // 経験値
            {
                this.lblExp.Content = "ID = " + targetUnit.ID; // ユニット番号を表示する実験用
                //this.lblExp.Content = "Member ID = " + member_id; // メンバー番号を表示する実験用
            }
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
