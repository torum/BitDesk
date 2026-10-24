using Avalonia.Controls;
using BitDesk.ViewModels;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Navigation;
using System.Diagnostics;
using System.Linq;

namespace BitDesk.Views.ChartPages;

public partial class XrpJpyPage : UserControl
{   
    public XrpJpyPage()
    {
        var mainViewModel = App.GetService<MainViewModel>();
        var pairViewModel = mainViewModel.Pairs.FirstOrDefault(p => p.PairCode == Models.PairCodes.xrp_jpy);
        if (pairViewModel != null)
        {
            this.DataContext = pairViewModel;
        }
        else
        {
            Debug.WriteLine("XrpJpyPage: PairViewModel for xrp_jpy not found.");
        }

        InitializeComponent();

        this.Loaded += XrpJpyPage_Loaded;
        AddHandler(FAFrame.NavigatedToEvent, OnNavigatedTo);
        AddHandler(FAFrame.NavigatedFromEvent, OnNavigatedFrom);
    }

    private void XrpJpyPage_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //Debug.WriteLine("XrpJpyPage loaded. DataContext: " + (this.DataContext?.ToString() ?? "null"));
    }

    private void OnNavigatedTo(object? sender, FANavigationEventArgs e)
    {
        if (this.DataContext is PairViewModel pairViewModel)
        {
            pairViewModel.IsSelectedActive = true;
        }
    }

    private void OnNavigatedFrom(object? sender, FANavigationEventArgs e)
    {
        if (this.DataContext is PairViewModel pairViewModel)
        {
            pairViewModel.IsSelectedActive = false;
        }
    }


}