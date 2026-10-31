using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using BitDesk.Models;
using BitDesk.Services;
using BitDesk.Services.Contracts;
using BitDesk.ViewModels;
using BitDesk.Views.ChartPages;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BitDesk.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _mainViewModel;

    private readonly Dictionary<string, Type> _pageMap = new()
    {
        { "BitDesk.Views.ChartPages.BtcJpyPage", typeof(Views.ChartPages.BtcJpyPage) },
        { "BitDesk.Views.ChartPages.XrpJpyPage", typeof(Views.ChartPages.XrpJpyPage) },
        { "BitDesk.Views.ChartPages.EthJpyPage", typeof(Views.ChartPages.EthJpyPage) },
        { "BitDesk.Views.ChartPages.XlmJpyPage", typeof(Views.ChartPages.XlmJpyPage) },
        { "BitDesk.Views.GridViewPage", typeof(Views.GridViewPage) },
        { "BitDesk.Views.SettingsPage", typeof(Views.SettingsPage) },
    };

    private readonly List<(string Tag, Type Page)> _pages =
    [
        (PairCodes.btc_jpy.ToString(), typeof(BtcJpyPage)),
        (PairCodes.xrp_jpy.ToString(), typeof(XrpJpyPage)),
        (PairCodes.eth_jpy.ToString(), typeof(EthJpyPage)),
        (PairCodes.xlm_jpy.ToString(), typeof(XlmJpyPage)),
        ("gridview", typeof(GridViewPage)),
        ("settings", typeof(SettingsPage)),
        /*
        (PairCodes.ltc_jpy.ToString(), typeof(LtcJpyPage)),
        (PairCodes.mona_jpy.ToString(), typeof(MonaJpyPage)),
        (PairCodes.bcc_jpy.ToString(), typeof(BccJpyPage)),
        (PairCodes.qtum_jpy.ToString(), typeof(QtumJpyPage)),
        (PairCodes.bat_jpy.ToString(), typeof(BatJpyPage)),
        (PairCodes.omg_jpy.ToString(), typeof(OmgJpyPage)),
        (PairCodes.xym_jpy.ToString(), typeof(XymJpyPage)),
        (PairCodes.link_jpy.ToString(), typeof(LinkJpyPage)),
        (PairCodes.mkr_jpy.ToString(), typeof(MkrJpyPage)),
        (PairCodes.boba_jpy.ToString(), typeof(BobaJpyPage)),
        (PairCodes.enj_jpy.ToString(), typeof(EnjJpyPage)),
        (PairCodes.matic_jpy.ToString(), typeof(MaticJpyPage)),
        (PairCodes.dot_jpy.ToString(), typeof(DotJpyPage)),
        (PairCodes.doge_jpy.ToString(), typeof(DogeJpyPage)),
        (PairCodes.astr_jpy.ToString(), typeof(AstrJpyPage)),
        (PairCodes.ada_jpy.ToString(), typeof(AdaJpyPage)),
        (PairCodes.avax_jpy.ToString(), typeof(AvaxJpyPage)),
        (PairCodes.axs_jpy.ToString(), typeof(AxsJpyPage)),
        (PairCodes.flr_jpy.ToString(), typeof(FlrJpyPage)),
        (PairCodes.sand_jpy.ToString(), typeof(SandJpyPage)),
        */
    ];

    private int _winRestoreWidth = 1024;
    private int _winRestoreHeight = 768;
    private int _winRestoreTop = 0;
    private int _winRestoreLeft = 0;

    private readonly IBitbankStreamService _bitbankStreamService;
    private readonly IConfigurationService _configurationService;
    private readonly INavigationService _navigationService;
    private readonly IDialogService _dialogService;
    private bool _shutdownStarted;
    private bool _allowClose;

    private bool _isUserKeySet = false;

    // Parameterless constructor for XAML Previewer
    public MainWindow()
    {
        _mainViewModel = App.GetService<MainViewModel>();
        this.DataContext = _mainViewModel;
        _bitbankStreamService = App.GetService<IBitbankStreamService>();
        _configurationService = App.GetService<IConfigurationService>();
        _navigationService = App.GetService<INavigationService>();
        _dialogService = App.GetService<IDialogService>();

        InitializeComponent();

        this.Title = "BitDesk";
        Bitmap bitmap = new(AssetLoader.Open(new Uri("avares://BitDesk/Assets/BitDesk-24.png")));
        ImageAppIcon.Source = bitmap;
        this.WindowDecorations = WindowDecorations.Full;
        this.ExtendClientAreaToDecorationsHint = true;

        _navigationService.Initialize(RootFrame, _pageMap);

        var mw = _configurationService.CurrentConfiguration.MainWindow;

        if (mw.Height >= 200)
        {
            this.Height = mw.Height;
        }

        if (mw.Width >= 200)
        {
            this.Width = mw.Width;
        }

        // TODO: multi-monitor support. If the saved position is outside of the current monitor, move it to the monitor.
        if ((mw.Left >= 0) && (mw.Top >= 0))
        {
            this.Position = new PixelPoint(mw.Left, mw.Top);
        }
        else
        {
            this.Position = new PixelPoint(0, 0);
        }

        _winRestoreWidth = (int)mw.RestoreWidth;
        _winRestoreHeight = (int)mw.RestoreHeight;
        _winRestoreLeft = mw.RestoreLeft;
        _winRestoreTop = mw.RestoreTop;

        if (mw.WindowState == WindowState.Maximized)
        {
            this.WindowState = WindowState.Maximized;
        }
        else
        {
            this.WindowState = WindowState.Normal;
        }

    }

    protected async override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        this.Activated += MainWindow_Activated;
        this.SizeChanged += this.MainWindow_SizeChanged;
        this.PositionChanged += this.MainWindow_PositionChanged;
        this.ActualThemeVariantChanged += OnActualThemeVariantChanged;
        this.PropertyChanged += this.OnWindow_PropertyChanged;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var isMax = this.WindowState == WindowState.Maximized;
            UpdateWindowMargins(isMax);
            UpdateThemeBackground(ActualThemeVariant);
        }

        await Task.Yield();

        var config = _configurationService.CurrentConfiguration;
        var userKey = string.Empty;
        var logOnResult = false;
        if (string.IsNullOrEmpty(config.User.UserHashedKey))
        {
            while (string.IsNullOrEmpty(userKey))
            {
                userKey = await _dialogService.ShowSetUpDialog();
            }

            (string savedHash, string savedSalt) = Helper.Password.HashPassword(userKey);
            config.User.UserHashedKey = savedHash;
            config.User.UserSalt = savedSalt;

            logOnResult = true;
        }
        else
        {
            while (logOnResult == false)
            {
                userKey = await _dialogService.ShowLogOnDialog();

                if (string.IsNullOrEmpty(userKey))
                {
                    continue;
                }

                logOnResult = Helper.Password.VerifyPassword(userKey, config.User.UserHashedKey, config.User.UserSalt);
            }
        }

        // Lator, we can use this flag to determine if we should save configuration or not. If the user has not logged in, we should not save configuration.
        _isUserKeySet = logOnResult;

        await Task.Yield();

        await _mainViewModel.InitializeAndStartAsync(userKey);

        bool IsOnlySettingsPage = false;

        if (_mainViewModel.EnabledPairs.Count <= 0)
        {
            Debug.WriteLine("OnOpened: No enabled pairs found. Please enable at least one pair in the settings.");
            IsOnlySettingsPage = true;
        }

        if (NavigateViewControl.MenuItems.Count <= 0)
        {
            Debug.WriteLine("NavigateViewControl.MenuItems.Count <= 0. No menu items to select.");
            IsOnlySettingsPage = true;
        }

        NavigateViewControl.SelectedItem = NavigateViewControl.MenuItems.OfType<FANavigationViewItem>().Where(x => x.IsVisible == IsVisible).FirstOrDefault();//.First();

        if (NavigateViewControl.SelectedItem is null)
        {
            Debug.WriteLine("NavigateViewControl.SelectedItem is null. No menu items to select.");
            IsOnlySettingsPage = true;
        }

        if (IsOnlySettingsPage)
        {
            var settingsItem = NavigateViewControl.MenuItems.OfType<FANavigationViewItem>().Where(x => x.Tag?.ToString()?.Equals("settings") == true).FirstOrDefault();//.First();
            var SettingsPage = _pages.FirstOrDefault(p => p.Tag == "settings");
            if (_navigationService.NavigateTo(SettingsPage.Page?.FullName, null))
            {
                //Debug.WriteLine($"NavigationView_ItemInvoked: Navigation to pageInfo successful.");
            }
            else
            {
                Debug.WriteLine($"OnOpened: Navigation to SettingsPage failed.");
            }

            return;
        }

        var pairViewModel = _mainViewModel.EnabledPairs.FirstOrDefault();

        if (pairViewModel is not null)
        {
            var item = _pages.FirstOrDefault(p => p.Tag.Equals(pairViewModel.PairCode.ToString()));
            var page = item.Page;

            if (_navigationService.NavigateTo(page.FullName, null))
            {
                await Task.Yield();

                NavigateViewControl.SelectedItem = NavigateViewControl.MenuItems.OfType<FANavigationViewItem>().Where(x => x.Tag?.ToString()?.Equals(pairViewModel.PairCode.ToString()) == true).FirstOrDefault();//.First();

                await _mainViewModel.SetSelectedPairFromCodeAsync(pairViewModel.PairCode);
            }
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        this.Activated -= MainWindow_Activated;
        this.SizeChanged -= this.MainWindow_SizeChanged;
        this.PositionChanged -= this.MainWindow_PositionChanged;
        this.ActualThemeVariantChanged -= OnActualThemeVariantChanged;
        this.PropertyChanged -= this.OnWindow_PropertyChanged;

        base.OnClosed(e);
    }

    protected override async void OnClosing(WindowClosingEventArgs e)
    {
        if (_allowClose)
        {
            return;
        }

        if (_shutdownStarted)
        {
            e.Cancel = true;
            return;
        }

        _shutdownStarted = true;

        // Cancel the closing event to prevent the window from closing immediately
        e.Cancel = true;

        // Window Position must be saved before the window is closed, otherwise the position will be lost.
        #region == Save Configuration ==

        if (_isUserKeySet)
        {
            // The user has logged in, so we can save the configuration. If the user has not logged in, we should not save the configuration.

            _mainViewModel.UpdateCurrentSettings();

            var config = _configurationService.CurrentConfiguration;
            config.MainWindow.Width = this.Width;
            config.MainWindow.Height = this.Height;
            config.MainWindow.Left = this.Position.X;
            config.MainWindow.Top = this.Position.Y;
            config.MainWindow.WindowState = this.WindowState;
            config.MainWindow.RestoreWidth = _winRestoreWidth;
            config.MainWindow.RestoreHeight = _winRestoreHeight;
            config.MainWindow.RestoreLeft = _winRestoreLeft;
            config.MainWindow.RestoreTop = _winRestoreTop;

            config.IsNavigationViewMenuOpen = this.NavigateViewControl.IsPaneOpen;

            await _configurationService.SaveConfigurationAsync(_mainViewModel.Pairs);
        }

        #endregion

        // Hide the window instead of closing it
        this.Hide();

        // Wait for the ticker service to stop gracefully
        await _bitbankStreamService.StopAsync();

        _mainViewModel.Dispose();

        // After the service has stopped, allow the window to close
        _allowClose = true;
        Close();
    }

    private void MainWindow_Activated(object? sender, EventArgs e)
    {
        this.AppTitle.Opacity = 1;

        if (this.WindowState == WindowState.Normal)
        {
            _winRestoreHeight = (int)this.Height;
            _winRestoreWidth = (int)this.Width;
            _winRestoreTop = this.Position.Y;
            _winRestoreLeft = this.Position.X;
        }
    }

    private void Window_Deactivated(object? sender, EventArgs e)
    {
        this.AppTitle.Opacity = 0.5;
    }

    private void OnActualThemeVariantChanged(object? sender, EventArgs e)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            UpdateThemeBackground(ActualThemeVariant);
        }
    }

    private void UpdateWindowMargins(bool isMax)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            if (isMax)
            {
                // It's actually 10px on all sides, but the window border is 2px, so the total is 10px.
                // This covers the window border and makes it look little sophisticated.
                this.ContentGrid.Margin = new Thickness(0, 0, 0, 0);
                this.BackgroundLayerBorder.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                // It's actually 10px on all sides, but the window border is 2px, so the total is 10px.
                // This covers the window border and makes it look little sophisticated.
                this.ContentGrid.Margin = new Thickness(9);
                this.BackgroundLayerBorder.Margin = new Thickness(7);
            }
        }
    }

    private void UpdateThemeBackground(ThemeVariant theme)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            this.BackgroundLayerBorder.IsVisible = true;
            this.BackgroundLayerBorder.Opacity = 1;

            if (theme == ThemeVariant.Dark)
            {
                //this.TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur];
                this.BackgroundLayerBorder.Background = new SolidColorBrush(Color.Parse("#202020"));
            }
            else if (theme == ThemeVariant.Light)
            {
                //this.TransparencyLevelHint = [WindowTransparencyLevel.Blur];
                this.BackgroundLayerBorder.Background = new SolidColorBrush(Color.Parse("#EEEEEE"));
            }
            else if (theme == ThemeVariant.Default)
            {
                //this.TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur];
                this.BackgroundLayerBorder.Background = new SolidColorBrush(Color.Parse("#121212"));
            }
        }
    }


    private void OnWindow_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            if (sender is not Window)
            {
                return;
            }
            if (e.Property.Name != nameof(WindowState))
            {
                return;
            }

            // For linux.
            if (e.NewValue != null && e.OldValue != null)
            {
                if (e.NewValue.Equals(e.OldValue))
                {
                    return;
                }
            }
            else
            {
                return;
            }

            //Debug.WriteLine($"WindowState changed from {e.OldValue} to {e.NewValue}");

            if (e.NewValue is WindowState.FullScreen || e.NewValue is WindowState.Maximized)
            {
                UpdateWindowMargins(true);
            }
            else
            {
                UpdateWindowMargins(false);
            }
        }
    }

    private void MainWindow_SizeChanged(object? sender, Avalonia.Controls.SizeChangedEventArgs e)
    {
        if (this.WindowState == WindowState.Normal)
        {
            _winRestoreHeight = (int)this.Height;
            _winRestoreWidth = (int)this.Width;
            _winRestoreTop = this.Position.Y;
            _winRestoreLeft = this.Position.X;
        }
    }

    private void MainWindow_PositionChanged(object? sender, PixelPointEventArgs e)
    {
        if (this.WindowState == WindowState.Normal)
        {
            _winRestoreHeight = (int)this.Height;
            _winRestoreWidth = (int)this.Width;
            _winRestoreTop = this.Position.Y;
            _winRestoreLeft = this.Position.X;
        }
    }

    private void NavigationView_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not FANavigationView)
        {
            return;
        }

    }

    private void NavigationView_SelectionChanged(object? sender, FluentAvalonia.UI.Controls.FANavigationViewSelectionChangedEventArgs e)
    {
        if (sender is not FluentAvalonia.UI.Controls.FANavigationView)
        {
            return;
        }

    }

    private async void NavigationView_ItemInvoked(object? sender, FluentAvalonia.UI.Controls.FANavigationViewItemInvokedEventArgs e)
    {
        if (sender is not FluentAvalonia.UI.Controls.FANavigationView nv)
        {
            Debug.WriteLine("NavigationView_ItemInvoked: sender is not FANavigationView.");
            return;
        }

        if (this.DataContext is not MainViewModel vm)
        {
            Debug.WriteLine("NavigationView_ItemInvoked: DataContext is not MainViewModel.");
            return;
        }

        if (e.IsSettingsInvoked == true)
        {
            var settingsItem = nv.MenuItems.OfType<FANavigationViewItem>().Where(x => x.Tag?.ToString()?.Equals("settings") == true).FirstOrDefault();//.First();
            var SettingsPage = _pages.FirstOrDefault(p => p.Tag == "settings");
            if (_navigationService.NavigateTo(SettingsPage.Page?.FullName, null))
            {
                //Debug.WriteLine($"NavigationView_ItemInvoked: Navigation to pageInfo successful.");
            }
            else
            {
                Debug.WriteLine($"NavigationView_ItemInvoked: Navigation to SettingsPage failed.");
            }

            return;
        }

        if (e.InvokedItemContainer is null && (e.InvokedItemContainer?.Tag is null))
        {
            Debug.WriteLine("NavigationView_ItemInvoked: InvokedItemContainer or its Tag is null.");
            return;
        }

        if (e.InvokedItemContainer is not FANavigationViewItem invokedItem)
        {
            Debug.WriteLine($"NavigationView_ItemInvoked: InvokedItem is not FANavigationViewItem. {e.InvokedItem}");
            return;
        }

        //Debug.WriteLine($"NavigationView_ItemInvoked: InvokedItem.Tag = {invokedItem.Tag?.ToString()}");

        var pageInfo = _pages.FirstOrDefault(p => p.Tag == invokedItem.Tag?.ToString());

        if (invokedItem.Tag?.ToString() == "gridview")
        {
            // Handle multiple view logic here
            if (_navigationService.NavigateTo(pageInfo.Page?.FullName, null))
            {
                await _mainViewModel.SetSelectedAllPairsFromCodeAsync();
            }

            return;
        }

        /*
        var selectedItem = nv.MenuItems.OfType<FANavigationViewItem>().Where(x => x.Tag?.ToString()?.Equals(invokedItem.Tag?.ToString()) == true).FirstOrDefault();//.First();

        if (selectedItem is null)
        {
            Debug.WriteLine("NavigationView_ItemInvoked: SelectedItem is null.");
            return;
        }
        */

        if (_navigationService.NavigateTo(pageInfo.Page?.FullName, null))
        {
            //Debug.WriteLine($"NavigationView_ItemInvoked: Navigation to pageInfo successful.");
            var _activePair = Enum.TryParse<PairCodes>(invokedItem.Tag?.ToString(), out var parsedPair);
            if (_activePair)
            {
                await _mainViewModel.SetSelectedPairFromCodeAsync(parsedPair);
            }
            else
            {
                Debug.WriteLine($"NavigationView_ItemInvoked: Failed to parse PairCodes from tag '{invokedItem.Tag?.ToString()}'.");
            }
        }
        else
        {
            Debug.WriteLine($"NavigationView_ItemInvoked: Navigation to pageInfo failed.");
        }
    }

    private void RootFrame_Navigated(object sender, FluentAvalonia.UI.Navigation.FANavigationEventArgs e)
    {
        //
    }
}