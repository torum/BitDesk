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
using System.Windows.Automation.Provider;
using System.Diagnostics;
using Bitbank.ViewModels;


namespace Bitbank.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
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

            // 
            Closing += (this.DataContext as MainViewModel).OnWindowClosing;

            Loaded += (this.DataContext as MainViewModel).OnWindowLoaded;

            // 
            TopMenuUnPinButton.Visibility = Visibility.Collapsed;

            //
            //LogOutButton.Visibility = Visibility.Collapsed;

            if (PasswordBox.Focusable)
                PasswordBox.Focus();

        }

        public void BringToForeground()
        {
            if (this.WindowState == WindowState.Minimized || this.Visibility == Visibility.Hidden)
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            }

            this.Activate();
            this.Topmost = true;
            this.Topmost = false;
            this.Focus();
        }

        private void Textbox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

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

        private void AcrylicWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            LayoutChange();

            Task.Run(() => SetDepthListboxScrollPosition());
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
                                            // 垂直方向はどまんなか！50%
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

        private void LayoutChange()
        {
            ///    <!-- Width="1536" x 848,  Width="1300" x 760 -->
            //System.Diagnostics.Debug.WriteLine(" LayoutChange - width:"+ this.ActualWidth.ToString() + " - height:" + this.ActualHeight.ToString());


            /* For optimal user experience, material design user interfaces should adapt layouts for the following breakpoint widths: 480, 600, 840, 960, 1280, 1440, and 1600dp. */

            if (this.ActualWidth >= 480)
            {
                //this.Title = "xsmall 480dp 以上 (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                if (this.ActualWidth >= 600)
                {
                    //this.Title = "small 600dp 以上 (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                    if (this.ActualWidth >= 840)
                    {
                        //this.Title = "small 840dp 以上 (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                        if (this.ActualWidth >= 960)
                        {
                            //this.Title = "medium 960dp 以上 (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                            if (this.ActualWidth >= 1024)
                            {
                                //this.Title = "medium 1024dp 以上 (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                                if (this.ActualWidth >= 1280)
                                {
                                    //this.Title = "large 1280dp 以上 (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                                    if (this.ActualWidth >= 1440)
                                    {
                                        //this.Title = "large 1440dp 以上 (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                                        if (this.ActualWidth >= 1600)
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
                                            // Width = 1536 x Height = 848

                                            if (this.ActualWidth > 1536)
                                            {
                                                //this.Title = "Big 1536 x 848  (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                                                ResizeToXXXLarge();

                                                //System.Diagnostics.Debug.WriteLine(" LayoutChange (ResizeToXXXLarge) - width:" + this.ActualWidth.ToString() + " - height:" + this.ActualHeight.ToString());
                                            }
                                            else
                                            {
                                                ResizeToLarge();

                                                //System.Diagnostics.Debug.WriteLine(" LayoutChange (ResizeToLarge) - width:" + this.ActualWidth.ToString() + " - height:" + this.ActualHeight.ToString());
                                            }


                                        }
                                    }
                                    else
                                    {

                                        if (this.ActualWidth > 1356)
                                        {
                                            ResizeToLarge();

                                            //System.Diagnostics.Debug.WriteLine(" LayoutChange (ResizeToLarge) - width:" + this.ActualWidth.ToString() + " - height:" + this.ActualHeight.ToString());
                                        }
                                        else
                                        {
                                            ResizeToDefault();

                                            //System.Diagnostics.Debug.WriteLine(" LayoutChange (ResizeToDefault) - width:" + this.ActualWidth.ToString() + " - height:" + this.ActualHeight.ToString());
                                        }

                                        // default
                                        // Width = 1300 x Height = 760
                                        //this.Title = "Default 1300 x 760  (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                                    }
                                }
                                else
                                {

                                    if (this.ActualWidth > 1124)
                                    {
                                        ResizeToDefault();

                                        //System.Diagnostics.Debug.WriteLine(" LayoutChange (ResizeToDefault) - width:" + this.ActualWidth.ToString() + " - height:" + this.ActualHeight.ToString());
                                    }
                                    else
                                    {
                                        ResizeToSmall();

                                        //System.Diagnostics.Debug.WriteLine(" LayoutChange (ResizeToSmall) - width:" + this.ActualWidth.ToString() + " - height:" + this.ActualHeight.ToString());
                                    }

                                }
                            }
                            else
                            {
                                ResizeToSmall();
                            }
                        }
                        else
                        {
                            ResizeToSmall();
                        }
                    }
                    else
                    {
                        // small
                        //this.Title = "Small  (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                        ResizeToSmall();

                    }
                }
                else
                {
                    // smallest
                    //this.Title = "X Small  (" + this.ActualWidth.ToString() + "x" + this.ActualHeight.ToString() + ")";

                    ResizeToXXXSmall();

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
            // ロゴを消す
            Logo.Visibility = Visibility.Collapsed;

            // トップメニュー を消す
            TopMenu.Visibility = Visibility.Visible;
            // トップメニューのコラムスパンを１に指定
            TopMenu.SetValue(Grid.ColumnSpanProperty, 1);

            // 左サイドメニューを表示
            LeftSideMenu.Visibility = Visibility.Visible;
            // 左サイドメニューのサイズを42指定
            //LeftMenuColum.Width = new GridLength(42, GridUnitType.Pixel);


            /*

            MainScroller
            - MainContentsGrid
            - - Main1Grid
            - - - Main2Grid
            - - - Main3Grid
            - - Transaction
            Depth
            Split
            BottomContents

            */

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
            // メインコンテンツグリッドの高さを指定。
            //    MainContentsGrid.Height = 1200;//1036

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


            //Asset.SetValue(Grid.ColumnSpanProperty, 1);
            //Order.SetValue(Grid.ColumnSpanProperty, 1);
            //Ifdoco.SetValue(Grid.ColumnSpanProperty, 1);
            //Chart.SetValue(Grid.ColumnSpanProperty, 1);
            Middle.SetValue(Grid.ColumnSpanProperty, 1);

            Depth.Visibility = Visibility.Collapsed;

            // メインコンテンツグリッドの高さ指定。＊スクロールが出るかでないかここのサイズ。
            MainContentsGrid.Height = 1260;//1554;//1036;

            // メイン１の高さを指定　＊重要
            Main1Grid.Height = 840;//1036;

            Main2Grid.Height = 420;

            Main3Grid.Height = 420;


            ////
            //Asset.Width = 360;
            //Order.Width = 360;
            //Ifdoco.Width = 360;
            //Chart.Width = 360;
            Middle.Width = 360;


            Main2Colum.Width = new GridLength(0, GridUnitType.Pixel);
            Main3Colum.Width = new GridLength(0, GridUnitType.Pixel);

            Transaction.Margin = new Thickness(3, 0, 20, 0);

            BottomContents.Margin = new Thickness(3, 0, 0, 0);

            //Chart.Margin = new Thickness(3, 3, 3, 0);
            Middle.Margin = new Thickness(3, 0, 3, 0);


            ChartSpanOneYearRadioButton.Visibility = Visibility.Collapsed;
            ChartSpanOneWeekRadioButton.Visibility = Visibility.Collapsed;
            ChartSpanThreeHourRadioButton.Visibility = Visibility.Collapsed;


        }

        private void ResizeToSmall()
        {

            Logo.Visibility = Visibility.Collapsed;

            LeftSideMenu.Visibility = Visibility.Visible;

            //LeftMenuColum.Width = new GridLength(42, GridUnitType.Pixel);


            TopMenu.Visibility = Visibility.Visible;
            TopMenu.SetValue(Grid.ColumnSpanProperty, 2);

            BottomContents.SetValue(Grid.ColumnSpanProperty, 2);
            Split.SetValue(Grid.ColumnSpanProperty, 2);


            //Split.Visibility = Visibility.Collapsed;
            //BottomContents.Visibility = Visibility.Collapsed;

            MainScroller.SetValue(Grid.ColumnSpanProperty, 2);
            // メインスクロールVのRowスパンを1に指定
            //MainScroller.SetValue(Grid.RowSpanProperty, 1);
            // メインスクロールVのRowスパンを3に指定
            MainScroller.SetValue(Grid.RowSpanProperty, 3);

            Main1Grid.SetValue(Grid.ColumnSpanProperty, 2);
            Main2Grid.SetValue(Grid.ColumnSpanProperty, 2);

            Main3Grid.SetValue(Grid.ColumnProperty, 1);
            Main3Grid.SetValue(Grid.RowProperty, 0);
            Main3Grid.SetValue(Grid.ColumnSpanProperty, 1);

            //Asset.SetValue(Grid.ColumnSpanProperty, 1);
            //Order.SetValue(Grid.ColumnSpanProperty, 1);
            //Ifdoco.SetValue(Grid.ColumnSpanProperty, 1);
            //Chart.SetValue(Grid.ColumnSpanProperty, 1);
            Middle.SetValue(Grid.ColumnSpanProperty, 1);

            //            Main1Grid.Height = 518;
            //            MainContentsGrid.Height = 518;
            //            Main2Grid.Height = 518;
            //            Main3Grid.Height = 518;


            // ボトムを消す
            BottomContents.Visibility = Visibility.Visible;
            Split.Visibility = Visibility.Visible;

            Transaction.Visibility = Visibility.Collapsed;
            Transaction.SetValue(Grid.ColumnProperty, 0);
            Transaction.SetValue(Grid.RowProperty, 1);

            Depth.Visibility = Visibility.Collapsed;


            //MainScroller.SetValue(Grid.RowSpanProperty, 3);



            // メインコンテンツグリッドの高さ指定。＊スクロールが出るかでないかここのサイズ。
            MainContentsGrid.Height = 420;//1036;

            // メイン１の高さを指定　＊重要
            Main1Grid.Height = 420;
            Main2Grid.Height = 420;
            Main3Grid.Height = 420;

            ////
            //Asset.Width = 360;
            //Order.Width = 360;
            //Ifdoco.Width = 360;
            //Chart.Width = 357;
            Middle.Width = 357;

            Main2Colum.Width = new GridLength(360, GridUnitType.Pixel);
            Main3Colum.Width = new GridLength(0, GridUnitType.Pixel);

            Transaction.Margin = new Thickness(0, 0, 20, 6);

            BottomContents.Margin = new Thickness(3, 0, 3, 0);

            //Chart.Margin = new Thickness(0, 0, 3, 6);
            Middle.Margin = new Thickness(0, 0, 3, 6);

            ChartSpanOneYearRadioButton.Visibility = Visibility.Collapsed;
            ChartSpanOneWeekRadioButton.Visibility = Visibility.Collapsed;
            ChartSpanThreeHourRadioButton.Visibility = Visibility.Collapsed;

        }

        private void ResizeToDefault()
        {
            // ロゴを
            Logo.Visibility = Visibility.Collapsed;

            // トップメニュー を
            TopMenu.Visibility = Visibility.Visible;
            // トップメニューのコラムスパンを１に指定
            TopMenu.SetValue(Grid.ColumnSpanProperty, 3);

            // 左サイドメニューを表示
            LeftSideMenu.Visibility = Visibility.Visible;
            // 左サイドメニューのサイズを42指定
            //LeftMenuColum.Width = new GridLength(42, GridUnitType.Pixel);


            // ボトムを
            BottomContents.Visibility = Visibility.Visible;
            // ボトムのコラムスパンを１に指定
            BottomContents.SetValue(Grid.ColumnSpanProperty, 3);

            //　スプリッタを
            Split.Visibility = Visibility.Visible;
            // スプリッタのコラムスパンを１に指定
            Split.SetValue(Grid.ColumnSpanProperty, 3);

            // 歩み値を
            //Transaction.Visibility = Visibility.Collapsed; //TODO
            //Transaction.SetValue(Grid.ColumnProperty, 2);
            //Transaction.SetValue(Grid.RowProperty, 0);
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


            //Asset.SetValue(Grid.ColumnSpanProperty, 2);
            //Order.SetValue(Grid.ColumnSpanProperty, 2);
            //Ifdoco.SetValue(Grid.ColumnSpanProperty, 2);
            //Chart.SetValue(Grid.ColumnSpanProperty, 2);
            Middle.SetValue(Grid.ColumnSpanProperty, 2);

            Depth.Visibility = Visibility.Collapsed;



            // メインコンテンツグリッドの高さ指定。＊スクロールが出るかでないかここのサイズ。
            MainContentsGrid.Height = 420;//1036;

            // メイン１の高さを指定　＊重要
            Main1Grid.Height = 420;
            Main2Grid.Height = 420;
            Main3Grid.Height = 420;




            /////////////////////////////////////////////////

            //Main2Grid.Width = 360;//
            //Main3Grid.Width = 720;//

            //MainContentsGrid.Width = 1293;
            //Main1Grid.Width = 1080;

            //Asset.Width = 507;
            //Order.Width = 507;
            //Ifdoco.Width = 507;
            //Asset.Width = 487;
           //Order.Width = 487;
            //Ifdoco.Width = 487;
            //Chart.Width = 487;
            Middle.Width = 487;

            Main2Colum.Width = new GridLength(360, GridUnitType.Pixel);
            Main3Colum.Width = new GridLength(360, GridUnitType.Pixel);

            Transaction.Margin = new Thickness(0, 0, 23, 6);

            BottomContents.Margin = new Thickness(3, 0, 3, 0);

            //Chart.Margin = new Thickness(0, 0, 20, 6);
            Middle.Margin = new Thickness(0, 0, 20, 6);
            ///

            RightSide1Colum.Width = new GridLength(0, GridUnitType.Pixel);
            RightSide2Colum.Width = new GridLength(0, GridUnitType.Pixel);


            ChartSpanOneYearRadioButton.Visibility = Visibility.Visible;
            ChartSpanOneWeekRadioButton.Visibility = Visibility.Visible;
            ChartSpanThreeHourRadioButton.Visibility = Visibility.Visible;

        }

        private void ResizeToLarge()
        {

            // ロゴを
            Logo.Visibility = Visibility.Collapsed;

            // トップメニュー を
            TopMenu.Visibility = Visibility.Visible;
            // トップメニューのコラムスパンを１に指定
            TopMenu.SetValue(Grid.ColumnSpanProperty, 4);

            // 左サイドメニューを表示
            LeftSideMenu.Visibility = Visibility.Visible;
            // 左サイドメニューのサイズを42指定
            //LeftMenuColum.Width = new GridLength(42, GridUnitType.Pixel);


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

            // メイン3コラムのコラムスパンを１に指定
            Main3Grid.SetValue(Grid.ColumnSpanProperty, 2);
            // メイン3コラムのRowを１に指定
            Main3Grid.SetValue(Grid.RowProperty, 0);
            // メイン3コラムを１に指定
            Main3Grid.SetValue(Grid.ColumnProperty, 1);


            //Asset.SetValue(Grid.ColumnSpanProperty, 2);
            //Order.SetValue(Grid.ColumnSpanProperty, 2);
            //Ifdoco.SetValue(Grid.ColumnSpanProperty, 2);
            //Chart.SetValue(Grid.ColumnSpanProperty, 2);
            Middle.SetValue(Grid.ColumnSpanProperty, 2);

            Depth.Visibility = Visibility.Collapsed;


            // メインコンテンツグリッドの高さ指定。＊スクロールが出るかでないかここのサイズ。
            MainContentsGrid.Height = 420;//1036;

            // メイン１の高さを指定　＊重要
            Main1Grid.Height = 420;
            Main2Grid.Height = 420;
            Main3Grid.Height = 420;

            ////
            //Asset.Width = 720;
            //Order.Width = 720;
            //Ifdoco.Width = 720;
            //Chart.Width = 720;
            Middle.Width = 720;

            //Main2Colum.Width = new GridLength(360, GridUnitType.Pixel);
            //Main3Colum.Width = new GridLength(360, GridUnitType.Pixel);
            Main2Colum.Width = new GridLength(380, GridUnitType.Pixel);
            Main3Colum.Width = new GridLength(380, GridUnitType.Pixel);

            Transaction.Margin = new Thickness(0, 0, 3, 6);

            BottomContents.Margin = new Thickness(3, 0, 3, 0);

            //Chart.Margin = new Thickness(0, 0, 20, 6);
            Middle.Margin = new Thickness(0, 0, 20, 6);


            RightSide1Colum.Width = new GridLength(213, GridUnitType.Pixel);
            RightSide2Colum.Width = new GridLength(0, GridUnitType.Pixel);


            ChartSpanOneYearRadioButton.Visibility = Visibility.Visible;
            ChartSpanOneWeekRadioButton.Visibility = Visibility.Visible;
            ChartSpanThreeHourRadioButton.Visibility = Visibility.Visible;

        }

        private void ResizeToXXXLarge()
        {

            // 左サイドメニュー
            LeftSideMenu.Visibility = Visibility.Visible;
            //LeftMenuColum.Width = new GridLength(42, GridUnitType.Pixel);

            TopMenu.Visibility = Visibility.Visible;

            TopMenu.SetValue(Grid.ColumnSpanProperty, 5);

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

            //Asset.SetValue(Grid.ColumnSpanProperty, 2);
            //Order.SetValue(Grid.ColumnSpanProperty, 2);
            //Ifdoco.SetValue(Grid.ColumnSpanProperty, 2);
            //Chart.SetValue(Grid.ColumnSpanProperty, 2);
            Middle.SetValue(Grid.ColumnSpanProperty, 2);

            //Main1Grid.Height = 518;
            //MainContentsGrid.Height = 518;



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


            ////
            //Asset.Width = 720;
            //Order.Width = 720;
            //Ifdoco.Width = 720;
            //Asset.Width = 720;
            //Order.Width = 720;
            //Ifdoco.Width = 720;
            //Chart.Width = 720;
            Middle.Width = 720;

            //Main2Colum.Width = new GridLength(360, GridUnitType.Pixel);
            //Main3Colum.Width = new GridLength(360, GridUnitType.Pixel);
            Main2Colum.Width = new GridLength(380, GridUnitType.Pixel);
            Main3Colum.Width = new GridLength(380, GridUnitType.Pixel);


            Transaction.Margin = new Thickness(0, 0, 20, 6);

            BottomContents.Margin = new Thickness(3, 0, 20, 0);

            //Chart.Margin = new Thickness(0, 0, 20, 6);
            Middle.Margin = new Thickness(0, 0, 20, 6);



            RightSide1Colum.Width = new GridLength(213, GridUnitType.Pixel);
            RightSide2Colum.Width = new GridLength(213, GridUnitType.Pixel);


            ChartSpanOneYearRadioButton.Visibility = Visibility.Visible;
            ChartSpanOneWeekRadioButton.Visibility = Visibility.Visible;
            ChartSpanThreeHourRadioButton.Visibility = Visibility.Visible;

        }

        private void TopMenuMinimumButton_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("■■■■■ TopMenuMinimumButton_Click.");

            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }

            ttt = this.Top;
            lll = this.Left;
            www = this.Width;
            hhh = this.Height;

            this.Height = 504;
            //this.Height = 554;
            this.Width = 423;

            ResizeMode = ResizeMode.NoResize;

            isMinimum = true;

            // 更新されないコントロールを非表示にする
            MinimumVisibility();


        }

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
                //Asset.Visibility = Visibility.Collapsed;
                //Order.Visibility = Visibility.Collapsed;
                // Ifdoco.Visibility = Visibility.Collapsed;
                Middle.Visibility = Visibility.Collapsed;

                MainContentsGrid.Height = 420;

            }
            else
            {
                Transaction.Visibility = Visibility.Visible;
                //Asset.Visibility = Visibility.Visible;
                //Order.Visibility = Visibility.Visible;
                //Ifdoco.Visibility = Visibility.Visible;
                Middle.Visibility = Visibility.Visible;
            }
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

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            (sender as PasswordBox).Tag = "";
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

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void PasswordBoxChangeOld_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (PasswordBoxChangeOld.Focusable)
                PasswordBoxChangeOld.Focus();
        }
    }


}
