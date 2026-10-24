using Avalonia.Controls;
using Avalonia.Threading;
using BitDesk.Services.Contracts;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
    private XDocument? _xdoc;

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
        _xdoc = null;
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
            _xdoc = XDocument.Load(_configFilePath);
            if (_xdoc.Root is null)
            {
                return config;
            }

            #region == Window setting ==
            
            // Main Window element
            var mainWindow = _xdoc.Root.Element("MainWindow");
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
            
            var opts = _xdoc.Root.Element("Options");
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


            }
            
            #endregion

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

    public async Task SaveConfigurationAsync()
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

        /// 
        root.AppendChild(opts);

        #endregion

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
