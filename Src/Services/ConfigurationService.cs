using Avalonia.Controls;
using Avalonia.Threading;
using BitDesk.Models;
using BitDesk.Services.Contracts;
using BitDesk.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace BitDesk.Services;

/*
 * WinUI3/AvaloniaUI does not provide a way to get the restore bounds of a window like WPF does when it is maximized or minimized.
 * So, we need to manually track the restore bounds of the window when it is in the restored state.
 * AppWindow_Changed(WinUI3)/Window_PositionChanged(AvaloniaUI) and Window_SizeChanged event handlers are called when the window state changes, such as when the window is maximized, minimized, or restored.
 * Make sure to update the restore bounds when the window is in the restored state.
 * Also, use Window_Activated event handler to update the restore bounds when the window is activated, because sometimes the window state change events are not fired when the window is activated.
 * This way, even if the window is closed while maximized or minimized, we can still save the restore bounds to the configuration file.
        WinRestoreWidth = (int)mw.RestoreWidth;
        WinRestoreHeight = (int)mw.RestoreHeight;
        WinRestoreLeft = mw.RestoreLeft;
        WinRestoreTop = mw.RestoreTop;
 */

internal sealed class ConfigurationService : IConfigurationService
{
    private bool _isInitialized;
    private bool _isConfigLoaded;
    private string? _configFilePath = string.Empty;
    private XDocument? _xDoc;
    private XElement? _xPairs;

    private string _userKey = string.Empty;
    private string _userSalt = string.Empty;

    public ConfigurationService()
    {
        _isConfigLoaded = false;
        CurrentConfiguration = new Models.Configuration();
    }

    public Models.Configuration CurrentConfiguration { get; private set; }

    public void Initialize(string configFilePath)
    {
        _configFilePath = configFilePath;
        _isConfigLoaded = false;
        _xDoc = null;
        _xPairs = null;
        _isInitialized = true;

        CurrentConfiguration = new Models.Configuration();

        var folderPath = Path.GetDirectoryName(configFilePath);
        if (folderPath is not null)
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }
        else
        {
            throw new InvalidOperationException("Invalid configuration file path.");
        }
    }

    public async Task<Models.Configuration> LoadConfigurationAsync()
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("ConfigurationService is not initialized.");
        }

        var config = new Models.Configuration();

        if (_isConfigLoaded)
        {
            Debug.WriteLine("LoadConfigurationAsync() - Already loaded, returning current configuration.");
            return CurrentConfiguration ?? config;
        }
        if (string.IsNullOrEmpty(_configFilePath))
        {
            throw new InvalidOperationException("Configuration file path is not set.");
        }
        if (!System.IO.File.Exists(_configFilePath))
        {
            Debug.WriteLine($"Configuration file not found: {_configFilePath}");
            // Set _isConfigLoaded to true so that SaveConfigurationAsync() can create the file with default settings.
            _isConfigLoaded = true;
            return config;
        }

        Debug.WriteLine("LoadConfigurationAsync()");

        try
        {
            _xDoc = XDocument.Load(_configFilePath);
            if (_xDoc.Root is null)
            {
                return config;
            }

            #region == Window setting ==
            
            // Main Window element
            var mainWindow = _xDoc.Root.Element("MainWindow");
            if (mainWindow is not null)
            {
                int wY = 24;
                int wX = 24;

                var hoge = mainWindow.Attribute("top");
                if (hoge is not null)
                {
                    //w.Top = double.Parse(hoge.Value);
                    //wY = int.Parse(hoge.Value);
                    if (Int32.TryParse(hoge.Value, out wY))
                    {
                        config.MainWindow.Top = wY;
                    }
                }

                hoge = mainWindow.Attribute("left");
                if (hoge is not null)
                {
                    //w.Left = double.Parse(hoge.Value);
                    //wX = int.Parse(hoge.Value);
                    if (Int32.TryParse(hoge.Value, out wX))
                    {
                        config.MainWindow.Left = wX;
                    }
                }
                //w.Position = new PixelPoint(wX, wY);

                hoge = mainWindow.Attribute("height");
                if (hoge is not null)
                {
                    if (!string.IsNullOrEmpty(hoge.Value))
                    {
                        //w.Height = double.Parse(hoge.Value);
                        config.MainWindow.Height = double.Parse(hoge.Value, CultureInfo.InvariantCulture);
                    }
                }

                hoge = mainWindow.Attribute("width");
                if (hoge is not null)
                {
                    if (!string.IsNullOrEmpty(hoge.Value))
                    {
                        //w.Width = double.Parse(hoge.Value);
                        config.MainWindow.Width = double.Parse(hoge.Value, CultureInfo.InvariantCulture);
                    }
                }

                hoge = mainWindow.Attribute("state");
                if (hoge is not null)
                {
                    if (hoge.Value == "Maximized")
                    {
                        // Since there is no restorebounds in AvaloniaUI.....
                        config.MainWindow.WindowState = WindowState.Maximized;
                    }
                    else if (hoge.Value == "Normal")
                    {
                        config.MainWindow.WindowState = WindowState.Normal;
                    }
                    else if (hoge.Value == "Minimized")
                    {
                        config.MainWindow.WindowState = WindowState.Normal;
                    }
                    else if (hoge.Value == "FullScreen")
                    {
                        config.MainWindow.WindowState = WindowState.FullScreen;
                    }
                }
            }

            #endregion

            #region == Layout ==
            /*
            var lay = _xdoc.Root.Element("Layout");
            if (lay is not null)
            {
                var leftpain = lay.Element("LeftPain");
                if (leftpain is not null)
                {
                    var hoge = leftpain.Attribute("NavigationViewMenuOpen");
                    if (hoge is not null)
                    {
                        if (hoge.Value == "True")
                        {
                            config.IsNavigationViewMenuOpen = true;
                        }
                        else
                        {
                            config.IsNavigationViewMenuOpen = false;
                        }
                    }
                }
            }
*/
            #endregion

            #region == Options ==
            
            var opts = _xDoc.Root.Element("Options");
            if (opts is not null)
            {
                var attrs = opts.Attribute("IsSaveLog");
                if (attrs is not null)
                {
                    if (!string.IsNullOrEmpty(attrs.Value))
                    {
                        if (attrs.Value == "True")
                        {
                            config.IsSaveLog = true;
                        }
                        else
                        {
                            config.IsSaveLog = false;
                        }
                    }
                }

                attrs = opts.Attribute("IsNavigationViewMenuOpen");
                if (attrs is not null)
                {
                    if (!string.IsNullOrEmpty(attrs.Value))
                    {
                        if (attrs.Value == "True")
                        {
                            config.IsNavigationViewMenuOpen = true;
                        }
                        else
                        {
                            config.IsNavigationViewMenuOpen = false;
                        }
                    }
                }

                attrs = opts.Attribute("IsOnBtcJpy");
                if (attrs is not null)
                {
                    if (!string.IsNullOrEmpty(attrs.Value))
                    {
                        if (attrs.Value == "True")
                        {
                            config.IsOnBtcJpy = true;
                        }
                        else
                        {
                            config.IsOnBtcJpy = false;
                        }
                    }
                }

                attrs = opts.Attribute("IsOnXrpJpy");
                if (attrs is not null)
                {
                    if (!string.IsNullOrEmpty(attrs.Value))
                    {
                        if (attrs.Value == "True")
                        {
                            config.IsOnXrpJpy = true;
                        }
                        else
                        {
                            config.IsOnXrpJpy = false;
                        }
                    }
                }

                attrs = opts.Attribute("IsOnEthJpy");
                if (attrs is not null)
                {
                    if (!string.IsNullOrEmpty(attrs.Value))
                    {
                        if (attrs.Value == "True")
                        {
                            config.IsOnEthJpy = true;
                        }
                        else
                        {
                            config.IsOnEthJpy = false;
                        }
                    }
                }
                
                attrs = opts.Attribute("IsOnXlmJpy");
                if (attrs is not null)
                {
                    if (!string.IsNullOrEmpty(attrs.Value))
                    {
                        if (attrs.Value == "True")
                        {
                            config.IsOnXlmJpy = true;
                        }
                        else
                        {
                            config.IsOnXlmJpy = false;
                        }
                    }
                }
            }

            #endregion

            #region == User Settings ==

            var user = _xDoc.Root.Element("User");
            if (user is not null)
            {
                var hoge = user.Attribute("UserHashedKey");
                if (hoge is not null)
                {
                    config.User.UserHashedKey = hoge.Value;
                }

                hoge = user.Attribute("UserSalt");
                if (hoge is not null)
                {
                    config.User.UserSalt = hoge.Value;
                }
            }

            #endregion

            #region == API Keys and Secrets ==

            var keys = _xDoc.Root.Element("APIKeys");
            if (keys != null)
            {
                var key = keys.Element("Assets");

                if (key != null)
                {
                    var xvalue = key.Attribute("Key");
                    if (xvalue != null)
                    {
                        if (!string.IsNullOrEmpty(xvalue.Value))
                        {
                            //config.AssetsApiKey = Helper.EncryptDecrypt.Decrypt(xvalue.Value);
                            config.AssetsApiKey = xvalue.Value;
                        }
                    }

                    xvalue = key.Attribute("Secret");
                    if (xvalue != null)
                    {
                        if (!string.IsNullOrEmpty(xvalue.Value))
                        {
                            //config.AssetsSecret = Helper.EncryptDecrypt.Decrypt(xvalue.Value);
                            config.AssetsSecret = xvalue.Value;
                        }
                    }
                }

                key = keys.Element("Orders");

                if (key != null)
                {
                    var xvalue = key.Attribute("Key");
                    if (xvalue != null)
                    {
                        if (!string.IsNullOrEmpty(xvalue.Value))
                        {
                            //config.OrdersApiKey = Helper.EncryptDecrypt.Decrypt(xvalue.Value);
                            config.OrdersApiKey = xvalue.Value;
                        }
                    }

                    xvalue = key.Attribute("Secret");
                    if (xvalue != null)
                    {
                        if (!string.IsNullOrEmpty(xvalue.Value))
                        {
                            //config.OrdersSecret = Helper.EncryptDecrypt.Decrypt(xvalue.Value);
                            config.OrdersSecret = xvalue.Value;
                        }
                    }
                }

                key = keys.Element("TradeHistory");

                if (key != null)
                {
                    var xvalue = key.Attribute("Key");
                    if (xvalue != null)
                    {
                        if (!string.IsNullOrEmpty(xvalue.Value))
                        {
                            //config.TradeHistoryApiKey = Helper.EncryptDecrypt.Decrypt(xvalue.Value);
                            config.TradeHistoryApiKey = xvalue.Value;
                        }
                    }

                    xvalue = key.Attribute("Secret");
                    if (xvalue != null)
                    {
                        if (!string.IsNullOrEmpty(xvalue.Value))
                        {
                            //config.TradeHistorySecret = Helper.EncryptDecrypt.Decrypt(xvalue.Value);
                            config.TradeHistorySecret = xvalue.Value;
                        }
                    }
                }

                key = keys.Element("MakeOrder");

                if (key != null)
                {
                    var xvalue = key.Attribute("Key");
                    if (xvalue != null)
                    {
                        if (!string.IsNullOrEmpty(xvalue.Value))
                        {
                            //config.MakeOrderApiKey = Helper.EncryptDecrypt.Decrypt(xvalue.Value);
                            config.MakeOrderApiKey = xvalue.Value;
                        }
                    }

                    xvalue = key.Attribute("Secret");
                    if (xvalue != null)
                    {
                        if (!string.IsNullOrEmpty(xvalue.Value))
                        {
                            //config.MakeOrderSecret = Helper.EncryptDecrypt.Decrypt(xvalue.Value);
                            config.MakeOrderSecret = xvalue.Value;
                        }
                    }
                }
            }

            #endregion

            // Save lator to be retrieved by GetPairs() method.
            _xPairs = _xDoc.Root.Element("Pairs");

            _isConfigLoaded = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error loading configuration: {ex}"); 
            Dispatcher.UIThread.Post(() => { App.AppendErrorLog("Error loading configuration", ex.Message); });
        }

        CurrentConfiguration = config;

        return CurrentConfiguration;
    }

    public void GetPairs(ObservableCollection<PairViewModel> pairs, string userKey, string userSalt)
    {
        // Each Pairs
        if (_xPairs == null)
        {
            return;
        }

        var pairList = _xPairs.Elements("Pair");

        foreach (var hoge in pairList)
        {
            if (hoge is not XElement element)
                continue;

            var xvalue = element.Attribute("PairCode");
            if (xvalue == null)
            {
                continue;
            }
            if (string.IsNullOrEmpty(xvalue.Value))
            {
                continue;
            }

            try
            {
                var pair = pairs.FirstOrDefault(x => x.PairCode.ToString() == xvalue.Value);
                if (pair is not null)
                {
                    // TODO:
                    //pair.IsChartTooltipVisible = MainVM.IsChartTooltipVisible;

                    var attrv = hoge.Attribute("IsEnabled");
                    if (attrv != null)
                    {
                        if (!string.IsNullOrEmpty(attrv.Value))
                        {
                            pair.IsEnabled = attrv.Value == "True";
                        }
                    }

                    attrv = hoge.Attribute("IsPaneOpen");
                    if (attrv != null)
                    {
                        if (!string.IsNullOrEmpty(attrv.Value))
                        {
                            pair.IsPaneOpen = attrv.Value == "True";
                        }
                    }

                    attrv = hoge.Attribute("CandleType");
                    if (attrv != null)
                    {
                        if (!string.IsNullOrEmpty(attrv.Value))
                        {
                            var candleTypeString = attrv.Value;

                            if (candleTypeString == "OneMin")
                            {
                                pair.SelectedCandleType = CandleTypes.OneMin;
                            }
                            else if (candleTypeString == "FiveMin")
                            {
                                pair.SelectedCandleType = CandleTypes.FiveMin;
                            }
                            else if (candleTypeString == "FifteenMin")
                            {
                                pair.SelectedCandleType = CandleTypes.FifteenMin;
                            }
                            else if (candleTypeString == "ThirtyMin")
                            {
                                pair.SelectedCandleType = CandleTypes.ThirtyMin;
                            }
                            else if (candleTypeString == "OneHour")
                            {
                                pair.SelectedCandleType = CandleTypes.OneHour;
                            }
                            else if (candleTypeString == "FourHour")
                            {
                                pair.SelectedCandleType = CandleTypes.FourHour;
                            }
                            else if (candleTypeString == "EightHour")
                            {
                                pair.SelectedCandleType = CandleTypes.EightHour;
                            }
                            else if (candleTypeString == "TwelveHour")
                            {
                                pair.SelectedCandleType = CandleTypes.TwelveHour;
                            }
                            else if (candleTypeString == "OneDay")
                            {
                                pair.SelectedCandleType = CandleTypes.OneDay;
                            }
                            else if (candleTypeString == "OneWeek")
                            {
                                pair.SelectedCandleType = CandleTypes.OneWeek;
                            }
                            else if (candleTypeString == "OneMonth")
                            {
                                pair.SelectedCandleType = CandleTypes.OneMonth;
                            }
                        }
                    }



                    attrv = hoge.Attribute("AutoTradeApiKey");
                    if (attrv != null)
                    {
                        if (!string.IsNullOrEmpty(attrv.Value))
                        {
                            pair.AutoTradeApiKey = Helper.EncryptDecrypt.Decrypt(attrv.Value, userKey, userSalt);
                        }
                    }

                    attrv = hoge.Attribute("AutoTradeSecret");
                    if (attrv != null)
                    {
                        if (!string.IsNullOrEmpty(attrv.Value))
                        {
                            pair.AutoTradeSecret = Helper.EncryptDecrypt.Decrypt(attrv.Value, userKey, userSalt);
                        }
                    }

                    attrv = hoge.Attribute("AutoTradeTama");
                    if (attrv != null)
                    {
                        if (!string.IsNullOrEmpty(attrv.Value))
                        {
                            pair.AutoTradeTama = Decimal.Parse(attrv.Value);
                        }
                    }

                    attrv = hoge.Attribute("AutoTradeDefaultHaba");
                    if (attrv != null)
                    {
                        if (!string.IsNullOrEmpty(attrv.Value))
                        {
                            pair.AutoTradeDefaultHaba = Decimal.Parse(attrv.Value);
                        }
                    }

                    attrv = hoge.Attribute("AutoTradeDefaultRikakuHaba");
                    if (attrv != null)
                    {
                        if (!string.IsNullOrEmpty(attrv.Value))
                        {
                            pair.AutoTradeDefaultRikakuHaba = Decimal.Parse(attrv.Value);
                        }
                    }

                    attrv = hoge.Attribute("AutoTradeSlots");
                    if (attrv != null)
                    {
                        if (!string.IsNullOrEmpty(attrv.Value))
                        {
                            pair.AutoTradeSlots = Decimal.Parse(attrv.Value);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception@GetPairs: {ex}");

                Dispatcher.UIThread.Post(() =>
                {
                    App.AppendErrorLog("Exception@GetPairs", ex.ToString());
                });
            }
        }
    }

    public void SetUserKey(string userKey, string userSalt)
    {
        _userKey = userKey;
        _userSalt = userSalt;
    }

    private static XmlElement SetPairs(ObservableCollection<PairViewModel> pairs, XmlDocument doc, string userKey, string userSalt)
    {
        // Each pairs
        var xPairs = doc.CreateElement(string.Empty, "Pairs", string.Empty);
        foreach (var hoge in pairs)
        {
            var xPair = doc.CreateElement(string.Empty, "Pair", string.Empty);

            var xAttrs = doc.CreateAttribute("PairCode");
            xAttrs.Value = hoge.PairCode.ToString();
            xPair.SetAttributeNode(xAttrs);

            xAttrs = doc.CreateAttribute("IsEnabled");
            xAttrs.Value = hoge.IsEnabled.ToString();
            xPair.SetAttributeNode(xAttrs);

            xAttrs = doc.CreateAttribute("IsPaneOpen");
            xAttrs.Value = hoge.IsPaneOpen.ToString();
            xPair.SetAttributeNode(xAttrs);

            xAttrs = doc.CreateAttribute("CandleType");
            xAttrs.Value = hoge.SelectedCandleType.ToString();
            xPair.SetAttributeNode(xAttrs);

            xAttrs = doc.CreateAttribute("IsChartTooltipVisible");
            xAttrs.Value = "";// TODO:
            xPair.SetAttributeNode(xAttrs);

            xAttrs = doc.CreateAttribute("AutoTradeApiKey");
            xAttrs.Value = Helper.EncryptDecrypt.Encrypt(hoge.AutoTradeApiKey, userKey, userSalt);
            xPair.SetAttributeNode(xAttrs);

            xAttrs = doc.CreateAttribute("AutoTradeSecret");
            xAttrs.Value = Helper.EncryptDecrypt.Encrypt(hoge.AutoTradeSecret, userKey, userSalt);
            xPair.SetAttributeNode(xAttrs);

            xAttrs = doc.CreateAttribute("AutoTradeTama");
            xAttrs.Value = hoge.AutoTradeTama.ToString();
            xPair.SetAttributeNode(xAttrs);

            xAttrs = doc.CreateAttribute("AutoTradeDefaultHaba");
            xAttrs.Value = hoge.AutoTradeDefaultHaba.ToString();
            xPair.SetAttributeNode(xAttrs);

            xAttrs = doc.CreateAttribute("AutoTradeDefaultRikakuHaba");
            xAttrs.Value = hoge.AutoTradeDefaultRikakuHaba.ToString();
            xPair.SetAttributeNode(xAttrs);

            xAttrs = doc.CreateAttribute("AutoTradeSlots");
            xAttrs.Value = hoge.AutoTradeSlots.ToString();
            xPair.SetAttributeNode(xAttrs);


            xPairs.AppendChild(xPair);

        }

        return xPairs;
    }

    public async Task SaveConfigurationAsync(ObservableCollection<PairViewModel> pairs)
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("ConfigurationService is not initialized.");
        }
        // Make sure Window and settings have been fully loaded and not overriding with empty data.
        if (!_isConfigLoaded)
        {
            // Should be fine now -> Don't return, because we want to save the configuration even if it hasn't been loaded yet, like the first time.
            //return;
        }
        if (string.IsNullOrEmpty(_configFilePath))
        {
            throw new InvalidOperationException("Configuration file path is not set.");
        }

        Debug.WriteLine("SaveConfigurationAsync");

        // TODO: Load xdoc from global variable if exists, and apply changes to it, then save it back to file.
        //_xdoc

        var config = CurrentConfiguration;

        XmlDocument doc = new();
        XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        doc.InsertBefore(xmlDeclaration, doc.DocumentElement);

        // Root Document Element
        XmlElement root = doc.CreateElement(string.Empty, "App", string.Empty);
        doc.AppendChild(root);

        XmlAttribute attrs;

        #region == Window settings ==

        var w = config.MainWindow;
        //double windowWidth = 780;

        // Main Window element
        XmlElement mainWindow = doc.CreateElement(string.Empty, "MainWindow", string.Empty);

        // Main Window attributes
        attrs = doc.CreateAttribute("height");
        attrs.Value = w.WindowState == WindowState.Normal ? w.Height.ToString(CultureInfo.InvariantCulture) : w.RestoreHeight.ToString(CultureInfo.InvariantCulture);
        mainWindow.SetAttributeNode(attrs);

        attrs = doc.CreateAttribute("width");
        if (w.WindowState == WindowState.Normal)
        {
            attrs.Value = w.Width.ToString(CultureInfo.InvariantCulture);
            //windowWidth = w.Width;
        }
        else
        {
            attrs.Value = w.RestoreWidth.ToString(CultureInfo.InvariantCulture);
        }
        mainWindow.SetAttributeNode(attrs);

        attrs = doc.CreateAttribute("top");
        attrs.Value = w.WindowState == WindowState.Normal ? w.Top.ToString(CultureInfo.InvariantCulture) : w.RestoreTop.ToString(CultureInfo.InvariantCulture);
        mainWindow.SetAttributeNode(attrs);

        attrs = doc.CreateAttribute("left");
        attrs.Value = w.WindowState == WindowState.Normal ? w.Left.ToString(CultureInfo.InvariantCulture) : w.RestoreLeft.ToString(CultureInfo.InvariantCulture);
        mainWindow.SetAttributeNode(attrs);

        attrs = doc.CreateAttribute("state");
        attrs.Value = w.WindowState switch
        {
            WindowState.Maximized => "Maximized",
            WindowState.Normal => "Normal",
            WindowState.Minimized => "Minimized",
            WindowState.FullScreen => "FullScreen",
            _ => attrs.Value
        };
        mainWindow.SetAttributeNode(attrs);

        // set MainWindow element to root.
        root.AppendChild(mainWindow);

        #endregion

        #region == Layout ==
        /*
        var lay = doc.CreateElement(string.Empty, "Layout", string.Empty);

        // LeftPain
        var leftpain = doc.CreateElement(string.Empty, "LeftPain", string.Empty);

        attrs = doc.CreateAttribute("NavigationViewMenuOpen");
        attrs.Value = config.IsNavigationViewMenuOpen ? "True" : "False";
        leftpain.SetAttributeNode(attrs);

        //
        lay.AppendChild(leftpain);

        #region == Header columns ==

        var headers = doc.CreateElement(string.Empty, "Headers", string.Empty);

        //
        lay.AppendChild(headers);

        #endregion

        ////
        root.AppendChild(lay);
        */
        #endregion

        #region == Options ==

        var opts = doc.CreateElement(string.Empty, "Options", string.Empty);

        /*
        // LastOpenedDirectory
        attrs = doc.CreateAttribute("lastOpenedDirectory");
        attrs.Value = !string.IsNullOrEmpty(config.LastOpenedDirectory) ? config.LastOpenedDirectory : string.Empty;
        opts.SetAttributeNode(attrs);
        */

        attrs = doc.CreateAttribute("IsSaveLog");
        attrs.Value = config.IsSaveLog ? "True" : "False";
        opts.SetAttributeNode(attrs);

        attrs = doc.CreateAttribute("IsNavigationViewMenuOpen");
        attrs.Value = config.IsNavigationViewMenuOpen ? "True" : "False";
        opts.SetAttributeNode(attrs);

        attrs = doc.CreateAttribute("IsOnBtcJpy");
        attrs.Value = config.IsOnBtcJpy ? "True" : "False";
        opts.SetAttributeNode(attrs);

        attrs = doc.CreateAttribute("IsOnXrpJpy");
        attrs.Value = config.IsOnXrpJpy ? "True" : "False";
        opts.SetAttributeNode(attrs);

        attrs = doc.CreateAttribute("IsOnEthJpy");
        attrs.Value = config.IsOnEthJpy ? "True" : "False";
        opts.SetAttributeNode(attrs);

        attrs = doc.CreateAttribute("IsOnXlmJpy");
        attrs.Value = config.IsOnXlmJpy ? "True" : "False";
        opts.SetAttributeNode(attrs);

        /// 
        root.AppendChild(opts);

        #endregion

        #region == User Settings ==

        XmlElement user = doc.CreateElement(string.Empty, "User", string.Empty);

        attrs = doc.CreateAttribute("UserHashedKey");
        attrs.Value = config.User.UserHashedKey;
        user.SetAttributeNode(attrs);

        attrs = doc.CreateAttribute("UserSalt");
        attrs.Value = config.User.UserSalt;
        user.SetAttributeNode(attrs);

        root.AppendChild(user);

        #endregion

        #region == API Keys and Secrets ==

        // API Keys
        var xAPIKeys = doc.CreateElement(string.Empty, "APIKeys", string.Empty);

        var xAsset = doc.CreateElement(string.Empty, "Assets", string.Empty);

        attrs = doc.CreateAttribute("Key");
        //attrs.Value = Helper.EncryptDecrypt.Encrypt(config.AssetsApiKey);
        attrs.Value = config.AssetsApiKey;
        xAsset.SetAttributeNode(attrs);

        attrs = doc.CreateAttribute("Secret");
        //attrs.Value = Helper.EncryptDecrypt.Encrypt(config.AssetsSecret);
        attrs.Value = config.AssetsSecret;
        xAsset.SetAttributeNode(attrs);

        xAPIKeys.AppendChild(xAsset);

        var xOrder = doc.CreateElement(string.Empty, "Orders", string.Empty);

        attrs = doc.CreateAttribute("Key");
        //attrs.Value = Helper.EncryptDecrypt.Encrypt(config.OrdersApiKey);
        attrs.Value = config.OrdersApiKey;
        xOrder.SetAttributeNode(attrs);

        attrs = doc.CreateAttribute("Secret");
        //attrs.Value = Helper.EncryptDecrypt.Encrypt(config.OrdersSecret);
        attrs.Value = config.OrdersSecret;
        xOrder.SetAttributeNode(attrs);

        xAPIKeys.AppendChild(xOrder);

        var xTradeHistory = doc.CreateElement(string.Empty, "TradeHistory", string.Empty);

        attrs = doc.CreateAttribute("Key");
        //attrs.Value = Helper.EncryptDecrypt.Encrypt(config.TradeHistoryApiKey);
        attrs.Value = config.TradeHistoryApiKey;
        xTradeHistory.SetAttributeNode(attrs);

        attrs = doc.CreateAttribute("Secret");
        //attrs.Value = Helper.EncryptDecrypt.Encrypt(config.TradeHistorySecret);
        attrs.Value = config.TradeHistorySecret;
        xTradeHistory.SetAttributeNode(attrs);

        xAPIKeys.AppendChild(xTradeHistory);

        var xMakeOrder = doc.CreateElement(string.Empty, "MakeOrder", string.Empty);

        attrs = doc.CreateAttribute("Key");
        //attrs.Value = Helper.EncryptDecrypt.Encrypt(config.MakeOrderApiKey);
        attrs.Value = config.MakeOrderApiKey;
        xMakeOrder.SetAttributeNode(attrs);

        attrs = doc.CreateAttribute("Secret");
        //attrs.Value = Helper.EncryptDecrypt.Encrypt(config.MakeOrderSecret);
        attrs.Value = config.MakeOrderSecret;
        xMakeOrder.SetAttributeNode(attrs);

        xAPIKeys.AppendChild(xMakeOrder);


        root.AppendChild(xAPIKeys);

        #endregion

        // Each pairs
        var xPairs = SetPairs(pairs, doc, _userKey, _userSalt);
        root.AppendChild(xPairs);

        try
        {
            if (!Directory.Exists(App.AppDataFolder))
            {
                Directory.CreateDirectory(App.AppDataFolder);
            }

            //doc.Save(App.AppConfigFilePath);
            var temporaryPath = _configFilePath + ".tmp";
            doc.Save(temporaryPath);
            File.Move(temporaryPath, _configFilePath, overwrite: true);
        }
        //catch (System.IO.FileNotFoundException) { }
        catch (Exception ex)
        {
            if (config.IsSaveLog)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    App.AppendErrorLog("Exception@SaveConfigurationAsync", ex.ToString());
                });
            }
        }

        if (config.IsSaveLog)
        {
            // Save error logs.
            Dispatcher.UIThread.Post(App.SaveErrorLog);
        }

    }

}
