using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace BitDesk.Models;

public class Configuration
{
    public class WindowSettings
    {
        public double Width { get; set; } = 1024;
        public double Height { get; set; } = 768;
        public int Left { get; set; } = 0;
        public int Top { get; set; } = 0;
        public WindowState WindowState { get; set; } = WindowState.Normal;

        public double RestoreWidth { get; set; } = 1024;
        public double RestoreHeight { get; set; } = 768;
        public int RestoreLeft { get; set; } = 0;
        public int RestoreTop { get; set; } = 0;
    }

    public class UserSettings
    {
        public string UserHashedKey { get; set; } = string.Empty;
        public string UserSalt { get; set; } = string.Empty;
    }

    public WindowSettings MainWindow { get; set; } = new WindowSettings();

    public UserSettings User { get; set; } = new UserSettings();

    public bool IsSaveLog { get; set; } = false;

    public bool IsNavigationViewMenuOpen { get; set; } = true;
    public bool IsPaneOpen { get; set; } = true;

    public bool IsOnBtcJpy { get; set; } = true;
    public bool IsOnXrpJpy { get; set; } = false;
    public bool IsOnEthJpy { get; set; } = false;
    public bool IsOnXlmJpy { get; set; } = false;

    public string AssetsApiKey { get; set; } = string.Empty;
    public string AssetsSecret { get; set; } = string.Empty;
    public string OrdersApiKey { get; set; } = string.Empty;
    public string OrdersSecret { get; set; } = string.Empty;
    public string TradeHistoryApiKey { get; set; } = string.Empty;
    public string TradeHistorySecret { get; set; } = string.Empty;
    public string MakeOrderApiKey { get; set; } = string.Empty;
    public string MakeOrderSecret { get; set; } = string.Empty;

}
