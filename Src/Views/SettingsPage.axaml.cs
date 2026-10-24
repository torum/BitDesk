using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace BitDesk.Views;

public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        this.DataContext = App.GetService<BitDesk.ViewModels.MainViewModel>();

        InitializeComponent();
    }
}