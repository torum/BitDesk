using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace BitDesk.Models;

public class Configuration
{
    public class WindowSettings
    {
        public double Width { get; set; } = 780;
        public double Height { get; set; } = 600;
        public int Left { get; set; } = 0;
        public int Top { get; set; } = 0;
        public WindowState WindowState { get; set; } = WindowState.Normal;

        public double RestoreWidth { get; set; } = 780;
        public double RestoreHeight { get; set; } = 600;
        public int RestoreLeft { get; set; } = 0;
        public int RestoreTop { get; set; } = 0;
    }

    public WindowSettings MainWindow { get; set; } = new WindowSettings();

    public bool IsSaveLog { get; set; } = false;

    public bool IsNavigationViewMenuOpen { get; set; } = true;

    public bool IsOnBtcJpy { get; set; } = false;
    public bool IsOnXrpJpy { get; set; } = false;
    public bool IsOnEthJpy { get; set; } = false;

}
