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
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider; // UIAutomationProvider
using BitDesk.ViewModels;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace BitDesk
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        // システムトレイ表示用
        //private string _appTitle;

        // ミニマム　フラグ
        private bool isMinimum;

        // 元のサイズを覚える為　
        private double ttt;
        private double lll;
        private double www;
        private double hhh;

        public MainWindow()
        {
            InitializeComponent();

            // システムトレイ表示用
            //_appTitle = (this.DataContext as MainViewModel).AppTitle;

            Loaded += (this.DataContext as MainViewModel).OnWindowLoaded;

            Closing += (this.DataContext as MainViewModel).OnWindowClosing;

            // デフォルトのシステムメニューアイコン状態
            RestoreButton.Visibility = Visibility.Collapsed;
            MaxButton.Visibility = Visibility.Visible;
            TopMenuUnPinButton.Visibility = Visibility.Collapsed;

        }

        // 二重起動からの復帰
        public void BringToForeground()
        {
            if (this.WindowState == WindowState.Minimized || this.Visibility == Visibility.Hidden)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            }

            this.Activate();
            //this.Topmost = true;
            //this.Topmost = false;
            this.Focus();

        }

        // フルスクリーンモードの時にマウスカーソルを隠す
        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //this.Cursor = null;
            //Mouse.OverrideCursor = null;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            (sender as PasswordBox).Tag = "";
        }

        // リンクをクリックして、ブラウザ起動して表示
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        #region == UI変更のViewコードビハインド ==

        private void DepthListBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Task.Run(() => SetDepthListboxScrollPosition());
        }

        private void DepthListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SetDepthListboxScrollPosition();
        }

        private void Depth2ListBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Task.Run(() => SetDepth2ListboxScrollPosition());
        }

        private void Depth2ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SetDepth2ListboxScrollPosition();
        }

        private async void SetDepthListboxScrollPosition()
        {
            try
            {

                await Task.Delay(1000);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (this.DepthListBox.Items.Count > 0)
                    {
                        try
                        {
                            // ListBoxからAutomationPeerを取得
                            var peer = ItemsControlAutomationPeer.CreatePeerForElement(this.DepthListBox);
                            // GetPatternでIScrollProviderを取得
                            var scrollProvider = peer.GetPattern(System.Windows.Automation.Peers.PatternInterface.Scroll) as IScrollProvider;

                            if (scrollProvider != null)
                            {
                                if (scrollProvider.VerticallyScrollable)
                                {
                                    try
                                    {
                                        // パーセントで位置を指定してスクロール
                                        scrollProvider.SetScrollPercent(
                                            // 水平スクロールは今の位置
                                            scrollProvider.HorizontalScrollPercent,
                                            // 垂直方向50%
                                            50.0);
                                    }
                                    catch
                                    {
                                        System.Diagnostics.Debug.WriteLine("■■■■■ SetDepthListboxScrollPosition scrollProvider null Error");
                                    }
                                }
                            }
                        }
                        catch
                        {
                            System.Diagnostics.Debug.WriteLine("■■■■■ SetDepthListboxScrollPosition SetScrollPercent Error");
                        }
                    }
                });

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ SetDepthListboxScrollPosition Exception: " + e);
            }


        }

        private async void SetDepth2ListboxScrollPosition()
        {
            try
            {

                await Task.Delay(1000);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (this.Depth2ListBox.Items.Count > 0)
                    {
                        try
                        {
                            // ListBoxからAutomationPeerを取得
                            var peer = ItemsControlAutomationPeer.CreatePeerForElement(this.Depth2ListBox);
                            // GetPatternでIScrollProviderを取得
                            var scrollProvider = peer.GetPattern(System.Windows.Automation.Peers.PatternInterface.Scroll) as IScrollProvider;

                            if (scrollProvider != null)
                            {
                                if (scrollProvider.VerticallyScrollable)
                                {
                                    try
                                    {
                                        // パーセントで位置を指定してスクロール
                                        scrollProvider.SetScrollPercent(
                                            // 水平スクロールは今の位置
                                            scrollProvider.HorizontalScrollPercent,
                                            // 垂直方向はどまんなか！50%
                                            50.0);
                                    }
                                    catch
                                    {
                                        System.Diagnostics.Debug.WriteLine("■■■■■ SetDepth2ListboxScrollPosition scrollProvider null Error");
                                    }
                                }
                            }
                        }
                        catch
                        {
                            System.Diagnostics.Debug.WriteLine("■■■■■ SetDepth2ListboxScrollPosition SetScrollPercent Error");
                        }
                    }
                });

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ SetDepth2ListboxScrollPosition Exception: " + e);
            }


        }

        private void Depth2AyumiTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Depth2AyumiTab.SelectedIndex == 0)
            {
                DepthAyumiTab.SelectedIndex = 1;
            }
            else if (Depth2AyumiTab.SelectedIndex == 1)
            {
                DepthAyumiTab.SelectedIndex = 0;
            }
        }

        private void DepthAyumiTab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DepthAyumiTab.SelectedIndex == 0)
            {
                Depth2AyumiTab.SelectedIndex = 1;
            }
            else if (DepthAyumiTab.SelectedIndex == 1)
            {
                Depth2AyumiTab.SelectedIndex = 0;
            }
        }

        private void DepthListBoxCenter_Click(object sender, RoutedEventArgs e)
        {
            SetDepthListboxScrollPosition();
        }

        private void Depth2ListBoxCenter_Click(object sender, RoutedEventArgs e)
        {
            SetDepth2ListboxScrollPosition();
        }

        #endregion

        #region == レイアウト変更 ==

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LayoutChange();
        }

        private void LayoutChange()
        {

            /*
            // 幅
            double appWidth = this.ActualWidth;
            double boxWidth = (appWidth - 40) / 3;

            // WrapPanes's width
            Coins_wrap.Width = appWidth;

            // WrapPanes's Margin
            Coins_wrap.Margin = new Thickness(0, 0, 0, 0); ;

            btc_box.Width = boxWidth;
            ltc_box.Width = boxWidth;
            xrp_box.Width = boxWidth;
            eth_box.Width = boxWidth;
            mona_box.Width = boxWidth;
            bch_box.Width = boxWidth;

            // 高さ
            double appHeight = this.ActualHeight;
            double boxHeight = ((appHeight - 40) / 2);

            btc_box.Height = boxHeight;
            ltc_box.Height = boxHeight;
            xrp_box.Height = boxHeight;
            eth_box.Height = boxHeight;
            mona_box.Height = boxHeight;
            bch_box.Height = boxHeight;
            */


            if (this.ActualWidth >= 480)
            {
                //this.Title = "xsmall 480dp 以上 (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                if (this.ActualWidth >= 600)
                {
                    //this.Title = "small 600dp 以上 (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                    if (this.ActualWidth >= 800)
                    {
                        //this.Title = "small 840dp 以上 (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                        if (this.ActualWidth >= 960)
                        {
                            //this.Title = "medium 960dp 以上 (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                            if (this.ActualWidth >= 1024)
                            {
                                //this.Title = "medium 1024dp 以上 (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                                if (this.ActualWidth >= 1135)
                                {
                                    //this.Title = "large 1280dp 以上 (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                                    if (this.ActualWidth >= 1400)
                                    {
                                        //this.Title = "large 1440dp 以上 (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                                        if (this.ActualWidth >= 1635)
                                        {
                                            //this.Title = "large 1600dp 以上 (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                                            //if (this.ActualWidth >= 1920)
                                            //{
                                            //this.Title = "xlarge 1920dp 以上 (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                                            ResizeToXXXLarge();

                                            //System.Diagnostics.Debug.WriteLine(" LayoutChange (ResizeToXXXLarge) - width:" + this.ActualWidth.ToString() + " - height:" + this.ActualHeight.ToString());
                                            //}
                                        }
                                        else
                                        {

                                            ResizeToLarge();

                                            //System.Diagnostics.Debug.WriteLine(" LayoutChange (ResizeToLarge) - width:" + this.ActualWidth.ToString() + " - height:" + this.ActualHeight.ToString());

                                        }
                                    }
                                    else
                                    {

                                        ResizeToDefault();

                                        //System.Diagnostics.Debug.WriteLine(" LayoutChange (ResizeToDefault) - width:" + this.ActualWidth.ToString() + " - height:" + this.ActualHeight.ToString());

                                    }
                                }
                                else
                                {

                                    ResizeToSmall();

                                    //System.Diagnostics.Debug.WriteLine(" LayoutChange (ResizeToSmall) - width:" + this.ActualWidth.ToString() + " - height:" + this.ActualHeight.ToString());

                                }
                            }
                            else
                            {
                                ResizeToSmall();

                                //System.Diagnostics.Debug.WriteLine(" LayoutChange (ResizeToSmall) - width:" + this.ActualWidth.ToString() + " - height:" + this.ActualHeight.ToString());

                            }
                        }
                        else
                        {
                            ResizeToSmall();

                            //System.Diagnostics.Debug.WriteLine(" LayoutChange (ResizeToSmall) - width:" + this.ActualWidth.ToString() + " - height:" + this.ActualHeight.ToString());

                        }
                    }
                    else
                    {
                        ResizeToXXXSmall();

                        //System.Diagnostics.Debug.WriteLine(" LayoutChange (ResizeToXXXSmall) - width:" + this.ActualWidth.ToString() + " - height:" + this.ActualHeight.ToString());

                    }
                }
                else
                {
                    ResizeToXXXSmall();

                    //System.Diagnostics.Debug.WriteLine(" LayoutChange (ResizeToXXXSmall) - width:" + this.ActualWidth.ToString() + " - height:" + this.ActualHeight.ToString());

                }
            }
            else
            {
                //// 480dp 未満
                //this.Title = "480dp 未満 xsmall (" + this.ActualWidth.ToString() + ")";

                ResizeToXXXSmall();
            }

        }

        private void ResizeToXXXSmall()
        {

            // ボトムを消す
            BottomContents.Visibility = Visibility.Collapsed;
            // ボトムのコラムスパンを１に指定
            BottomContents.SetValue(Grid.ColumnSpanProperty, 1);

            //　スプリッタを消す
            Split.Visibility = Visibility.Collapsed;
            // スプリッタのコラムスパンを１に指定
            Split.SetValue(Grid.ColumnSpanProperty, 1);

            // 歩み値を消す
            Transaction.Visibility = Visibility.Visible;
            Transaction.SetValue(Grid.ColumnProperty, 0);
            Transaction.SetValue(Grid.RowProperty, 1);

            // メインスクロールVのコラムスパンを１に指定
            MainScroller.SetValue(Grid.ColumnSpanProperty, 1);
            //MainScroller.SetValue(Grid.ColumnProperty, 0);
            // メインスクロールVのRowスパンを3に指定
            MainScroller.SetValue(Grid.RowSpanProperty, 3);

            // メインコンテンツグリッドのコラムスパンを１に指定
            MainContentsGrid.SetValue(Grid.ColumnSpanProperty, 1);

            // メイン１コラムのコラムスパンを１に指定
            Main1Grid.SetValue(Grid.ColumnSpanProperty, 1);

            // メイン2コラムのコラムスパンを１に指定
            Main2Grid.SetValue(Grid.ColumnSpanProperty, 1);
            Main2Grid.SetValue(Grid.RowProperty, 0);
            Main2Grid.SetValue(Grid.ColumnProperty, 0);

            // メイン3コラムのコラムスパンを１に指定
            Main3Grid.SetValue(Grid.ColumnSpanProperty, 1);
            // メイン3コラムのRowを１に指定
            Main3Grid.SetValue(Grid.RowProperty, 1);
            // メイン3コラムを１に指定
            Main3Grid.SetValue(Grid.ColumnProperty, 0);

            Middle.SetValue(Grid.ColumnSpanProperty, 1);

            Depth.Visibility = Visibility.Collapsed;

            // メインコンテンツグリッドの高さ指定。＊スクロールが出るかでないかここのサイズ。
            MainContentsGrid.Height = 1260;//1554;//1036;

            // メイン１の高さを指定　＊重要
            Main1Grid.Height = 840;//1036;

            Main2Grid.Height = 420;

            Main3Grid.Height = 420;

            Middle.Width = 380;
            Middle.Margin = new Thickness(3, 0, 3, 0);

            Main2Colum.Width = new GridLength(0, GridUnitType.Pixel);
            Main3Colum.Width = new GridLength(0, GridUnitType.Pixel);

            Transaction.Width = 380;
            Transaction.Margin = new Thickness(3, 0, 10, 0);

            BottomContents.Margin = new Thickness(3, 0, 0, 0);

            ChartSpanOneYearRadioButton.Visibility = Visibility.Collapsed;
            ChartSpanOneWeekRadioButton.Visibility = Visibility.Collapsed;
            ChartSpanThreeHourRadioButton.Visibility = Visibility.Collapsed;

        }

        private void ResizeToSmall()
        {
            BottomContents.SetValue(Grid.ColumnSpanProperty, 2);
            Split.SetValue(Grid.ColumnSpanProperty, 2);

            MainScroller.SetValue(Grid.ColumnSpanProperty, 2);
            // メインスクロールVのRowスパンを3に指定
            MainScroller.SetValue(Grid.RowSpanProperty, 3);

            Main1Grid.SetValue(Grid.ColumnSpanProperty, 2);
            Main2Grid.SetValue(Grid.ColumnSpanProperty, 2);

            Main3Grid.SetValue(Grid.ColumnProperty, 1);
            Main3Grid.SetValue(Grid.RowProperty, 0);
            Main3Grid.SetValue(Grid.ColumnSpanProperty, 1);

            Middle.SetValue(Grid.ColumnSpanProperty, 1);

            // ボトムを消す
            BottomContents.Visibility = Visibility.Visible;
            Split.Visibility = Visibility.Visible;

            Transaction.Visibility = Visibility.Collapsed;
            Transaction.SetValue(Grid.ColumnProperty, 0);
            Transaction.SetValue(Grid.RowProperty, 1);

            Depth.Visibility = Visibility.Collapsed;

            // メインコンテンツグリッドの高さ指定。＊スクロールが出るかでないかここのサイズ。
            MainContentsGrid.Height = 420;//1036;

            // メイン１の高さを指定　＊重要
            Main1Grid.Height = 420;
            Main2Grid.Height = 420;
            Main3Grid.Height = 420;

            Middle.Width = 357;//357
            Middle.Height = 416;
            Middle.Margin = new Thickness(3, 1, 3, 0);

            Main2Colum.Width = new GridLength(360, GridUnitType.Pixel);
            Main3Colum.Width = new GridLength(0, GridUnitType.Pixel);

            BottomContents.Margin = new Thickness(3, 0, 3, 0);

            //Transaction.Margin = new Thickness(0, 0, 10, 3);

            ChartSpanOneYearRadioButton.Visibility = Visibility.Collapsed;
            ChartSpanOneWeekRadioButton.Visibility = Visibility.Collapsed;
            ChartSpanThreeHourRadioButton.Visibility = Visibility.Collapsed;

        }

        private void ResizeToDefault()
        {
            // ボトムを
            BottomContents.Visibility = Visibility.Visible;
            // ボトムのコラムスパンを１に指定
            BottomContents.SetValue(Grid.ColumnSpanProperty, 3);

            //　スプリッタを
            Split.Visibility = Visibility.Visible;
            // スプリッタのコラムスパンを１に指定
            Split.SetValue(Grid.ColumnSpanProperty, 3);

            Transaction.Visibility = Visibility.Visible;
            Transaction.SetValue(Grid.ColumnProperty, 3);
            Transaction.SetValue(Grid.RowProperty, 0);

            // メインスクロールVのコラムスパンを3に指定
            MainScroller.SetValue(Grid.ColumnSpanProperty, 3);
            //MainScroller.SetValue(Grid.ColumnProperty, 2);
            // メインスクロールVのRowスパンを1に指定
            MainScroller.SetValue(Grid.RowSpanProperty, 1);

            // メインコンテンツグリッドのコラムスパンを１に指定
            MainContentsGrid.SetValue(Grid.ColumnSpanProperty, 1);
            // メインコンテンツグリッドの高さを指定。
            //MainContentsGrid.Height = 420;

            // メイン１コラムのコラムスパンを3に指定
            Main1Grid.SetValue(Grid.ColumnSpanProperty, 3);

            // メイン2コラムのコラムスパンを１に指定
            Main2Grid.SetValue(Grid.ColumnSpanProperty, 1);
            Main2Grid.SetValue(Grid.RowProperty, 0);
            Main2Grid.SetValue(Grid.ColumnProperty, 0);

            // メイン3コラムのコラムスパンを１に指定
            Main3Grid.SetValue(Grid.ColumnSpanProperty, 1);
            // メイン3コラムのRowを0に指定
            Main3Grid.SetValue(Grid.RowProperty, 0);
            // メイン3コラムを１に指定
            Main3Grid.SetValue(Grid.ColumnProperty, 1);

            Middle.SetValue(Grid.ColumnSpanProperty, 2);

            Depth.Visibility = Visibility.Collapsed;

            // メインコンテンツグリッドの高さ指定。＊スクロールが出るかでないかここのサイズ。
            MainContentsGrid.Height = 420;//1036;

            // メイン１の高さを指定　＊重要
            Main1Grid.Height = 420;
            Main2Grid.Height = 420;
            Main3Grid.Height = 420;

            // 中央のサイズ
            //Middle.Width = 487;
            Middle.Width = this.ActualWidth - 665; //645

            Middle.Height = 416;
            Middle.Margin = new Thickness(3, 1, 0, 3);

            //Main3Grid.Width = 504;
            Main3Grid.Width = this.ActualWidth - 648;//628

            var test = this.ActualWidth - 412;//412
            Main2Colum.Width = new GridLength(test, GridUnitType.Pixel);
            Main3Colum.Width = new GridLength(0, GridUnitType.Pixel);


            BottomContents.Margin = new Thickness(3, 0, 24, 0);

            Transaction.Height = 416;
            Transaction.Width = 213;
            Transaction.Margin = new Thickness(3, 1, 3, 3);

            RightSide1Colum.Width = new GridLength(0, GridUnitType.Pixel);
            RightSide2Colum.Width = new GridLength(0, GridUnitType.Pixel);


            ChartSpanOneYearRadioButton.Visibility = Visibility.Visible;
            ChartSpanOneWeekRadioButton.Visibility = Visibility.Visible;
            ChartSpanThreeHourRadioButton.Visibility = Visibility.Visible;

        }

        private void ResizeToLarge()
        {
            // ボトムを
            BottomContents.Visibility = Visibility.Visible;
            // ボトムのコラムスパンを指定
            BottomContents.SetValue(Grid.ColumnSpanProperty, 4);

            //　スプリッタを
            Split.Visibility = Visibility.Visible;
            // スプリッタのコラムスパンを指定
            Split.SetValue(Grid.ColumnSpanProperty, 4);

            // 歩み値を
            Transaction.Visibility = Visibility.Visible;
            Transaction.SetValue(Grid.ColumnProperty, 3);
            Transaction.SetValue(Grid.RowProperty, 0);

            // メインスクロールVのコラムスパンを１に指定
            MainScroller.SetValue(Grid.ColumnSpanProperty, 4);
            // メインスクロールVのRowスパンを3に指定
            MainScroller.SetValue(Grid.RowSpanProperty, 1);

            // メインコンテンツグリッドのコラムスパンを１に指定
            MainContentsGrid.SetValue(Grid.ColumnSpanProperty, 1);
            // メインコンテンツグリッドの高さを指定。
            //MainContentsGrid.Height = 518;

            // メイン１コラムのコラムスパンを１に指定
            Main1Grid.SetValue(Grid.ColumnSpanProperty, 3);

            // メイン2コラムのコラムスパンを１に指定
            Main2Grid.SetValue(Grid.ColumnSpanProperty, 1);
            Main2Grid.SetValue(Grid.RowProperty, 0);
            Main2Grid.SetValue(Grid.ColumnProperty, 0);

            // メイン3コラムのコラムスパンを2に指定
            Main3Grid.SetValue(Grid.ColumnSpanProperty, 2);
            // メイン3コラムのRowを0に指定
            Main3Grid.SetValue(Grid.RowProperty, 0);
            // メイン3コラムを１に指定
            Main3Grid.SetValue(Grid.ColumnProperty, 1);

            Middle.SetValue(Grid.ColumnSpanProperty, 2);

            Depth.Visibility = Visibility.Collapsed;

            // メインコンテンツグリッドの高さ指定。＊スクロールが出るかでないかここのサイズ。
            MainContentsGrid.Height = 420;//1036;

            // メイン１の高さを指定　＊重要
            Main1Grid.Height = 420;
            Main2Grid.Height = 420;
            Main3Grid.Height = 420;

            Middle.Width = 740; //740
            Middle.Height = 416;
            Middle.Margin = new Thickness(3, 1, 0, 3);

            Main3Grid.Width = 757;//757

            Transaction.Height = 416;
            Transaction.Width = 213;
            Transaction.Margin = new Thickness(3, 1, 3, 3);

            Main2Colum.Width = new GridLength(380, GridUnitType.Pixel);//380
            Main3Colum.Width = new GridLength(380, GridUnitType.Pixel);//380

            BottomContents.Margin = new Thickness(3, 0, 3, 0);


            RightSide1Colum.Width = new GridLength(213, GridUnitType.Pixel);
            RightSide2Colum.Width = new GridLength(0, GridUnitType.Pixel);

            ChartSpanOneYearRadioButton.Visibility = Visibility.Visible;
            ChartSpanOneWeekRadioButton.Visibility = Visibility.Visible;
            ChartSpanThreeHourRadioButton.Visibility = Visibility.Visible;

        }

        private void ResizeToXXXLarge()
        {

            BottomContents.SetValue(Grid.ColumnSpanProperty, 4);
            Split.SetValue(Grid.ColumnSpanProperty, 4);

            Split.Visibility = Visibility.Visible;
            BottomContents.Visibility = Visibility.Visible;

            MainScroller.SetValue(Grid.ColumnSpanProperty, 4);
            // メインスクロールVのRowスパンを1に指定
            MainScroller.SetValue(Grid.RowSpanProperty, 1);

            Main1Grid.SetValue(Grid.ColumnSpanProperty, 3);
            Main2Grid.SetValue(Grid.ColumnSpanProperty, 1);

            Main3Grid.SetValue(Grid.ColumnProperty, 1);
            Main3Grid.SetValue(Grid.RowProperty, 0);
            Main3Grid.SetValue(Grid.ColumnSpanProperty, 2);

            // 歩み値を
            Transaction.Visibility = Visibility.Visible;
            Transaction.SetValue(Grid.ColumnProperty, 3);
            Transaction.SetValue(Grid.RowProperty, 0);

            Middle.SetValue(Grid.ColumnSpanProperty, 2);

            // ボトムを表示
            BottomContents.Visibility = Visibility.Visible;
            Split.Visibility = Visibility.Visible;

            Transaction.Visibility = Visibility.Visible;

            Depth.Visibility = Visibility.Visible;

            // メインコンテンツグリッドの高さ指定。＊スクロールが出るかでないかここのサイズ。
            MainContentsGrid.Height = 420;//1036;

            // メイン１の高さを指定　＊重要
            Main1Grid.Height = 420;

            Main2Grid.Height = 420;

            Main3Grid.Height = 420;

            Middle.Width = 743;
            Middle.Height = 416;
            Middle.Margin = new Thickness(3, 1, 0, 3);

            Main3Grid.Width = 760;//760

            Transaction.Height = 416;
            Transaction.Width = 210;
            Transaction.Margin = new Thickness(3, 1, 0, 3);

            Main2Colum.Width = new GridLength(380, GridUnitType.Pixel);
            Main3Colum.Width = new GridLength(380, GridUnitType.Pixel);

            BottomContents.Margin = new Thickness(3, 0, 3, 0);

            RightSide1Colum.Width = new GridLength(213, GridUnitType.Pixel);
            RightSide2Colum.Width = new GridLength(229, GridUnitType.Pixel);

            ChartSpanOneYearRadioButton.Visibility = Visibility.Visible;
            ChartSpanOneWeekRadioButton.Visibility = Visibility.Visible;
            ChartSpanThreeHourRadioButton.Visibility = Visibility.Visible;

        }

        #endregion

        #region == システムメニュー ==

        // ウィンドウ状態
        private void Window_StateChanged(object sender, EventArgs e)
        {

            if (this.WindowState == WindowState.Normal)
            {
                RestoreButton.Visibility = Visibility.Collapsed;
                MaxButton.Visibility = Visibility.Visible;

                //_dispatcherMouseTimer.Stop();

                // ミニマム表示の時はリストアボタンを隠す
                if (isMinimum)
                {
                    RestoreButton.Visibility = Visibility.Collapsed;
                    MaxButton.Visibility = Visibility.Collapsed;
                }

                //BackgroundGrid.Margin = new Thickness(0, 0, 0, 0);
            }
            else if (this.WindowState == WindowState.Maximized)
            {
                RestoreButton.Visibility = Visibility.Visible;
                MaxButton.Visibility = Visibility.Collapsed;

                //_dispatcherMouseTimer.Start();

                //BackgroundGrid.Margin = new Thickness(3, 3, 3, 3);
            }

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MaxButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void TopMenuPinButton_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;

            TopMenuPinButton.Visibility = Visibility.Collapsed;
            TopMenuUnPinButton.Visibility = Visibility.Visible;
        }

        private void TopMenuUnPinButton_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;

            TopMenuPinButton.Visibility = Visibility.Visible;
            TopMenuUnPinButton.Visibility = Visibility.Collapsed;
        }

        // ミニマム表示
        private void TopMenuMinimumButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }

            ttt = this.Top;
            lll = this.Left;
            www = this.Width;
            hhh = this.Height;

            this.Height = this.MinHeight;//478
            this.Width = this.MinWidth;

            ResizeMode = ResizeMode.NoResize;

            isMinimum = true;

            // 更新されないコントロールを非表示にする
            MinimumVisibility();

        }

        // ミニマム表示から復帰
        private void TopMenuRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            isMinimum = false;

            ResizeMode = ResizeMode.CanResize;

            this.Top = ttt;
            this.Left = lll;
            this.Width = www;
            this.Height = hhh;

            // 非表示にされているコントロールを表示する
            MinimumVisibility();

        }

        private void MinimumVisibility()
        {

            if (isMinimum)
            {
                Transaction.Visibility = Visibility.Collapsed;
                Middle.Visibility = Visibility.Collapsed;

                MainContentsGrid.Height = 420;


                RestoreButton.Visibility = Visibility.Collapsed;
                MaxButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                Transaction.Visibility = Visibility.Visible;
                Middle.Visibility = Visibility.Visible;

                if (this.WindowState == WindowState.Normal)
                {
                    RestoreButton.Visibility = Visibility.Collapsed;
                    MaxButton.Visibility = Visibility.Visible;
                }
                else if (this.WindowState == WindowState.Maximized)
                {
                    RestoreButton.Visibility = Visibility.Visible;
                    MaxButton.Visibility = Visibility.Collapsed;
                }


            }
        }


        #endregion

        #region == フォーカスコントロール系 ==

        private void Textbox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void PasswordBox_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (PasswordBox.Focusable)
                PasswordBox.Focus();
        }

        private void PasswordBoxNew_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (PasswordBoxNew.Focusable)
                PasswordBoxNew.Focus();
        }

        private void PasswordBoxChangeOld_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (PasswordBoxChangeOld.Focusable)
                PasswordBoxChangeOld.Focus();
        }

        #endregion

        #region == MAXIMIZE時のタスクバー被りのFix ==
        // https://engy.us/blog/2020/01/01/implementing-a-custom-window-title-bar-in-wpf/

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            ((HwndSource)PresentationSource.FromVisual(this)).AddHook(HookProc);
        }

        public static IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_GETMINMAXINFO)
            {
                // We need to tell the system what our size should be when maximized. Otherwise it will cover the whole screen,
                // including the task bar.
                MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

                // Adjust the maximized size and position to fit the work area of the correct monitor
                IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

                if (monitor != IntPtr.Zero)
                {
                    MONITORINFO monitorInfo = new MONITORINFO();
                    monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));
                    GetMonitorInfo(monitor, ref monitorInfo);
                    RECT rcWorkArea = monitorInfo.rcWork;
                    RECT rcMonitorArea = monitorInfo.rcMonitor;
                    mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.Left - rcMonitorArea.Left);
                    mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.Top - rcMonitorArea.Top) - 4; // -4を付加した。てっぺんをクリックしても反応がなかったから。
                    mmi.ptMaxSize.X = Math.Abs(rcWorkArea.Right - rcWorkArea.Left);
                    mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.Bottom - rcWorkArea.Top) + 4; // 付加した分の補正。;
                }

                Marshal.StructureToPtr(mmi, lParam, true);
            }

            return IntPtr.Zero;
        }

        private const int WM_GETMINMAXINFO = 0x0024;

        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr handle, uint flags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.Left = left;
                this.Top = top;
                this.Right = right;
                this.Bottom = bottom;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

#endregion
    
    }
}
