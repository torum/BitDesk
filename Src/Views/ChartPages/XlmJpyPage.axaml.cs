using Avalonia.Controls;
using BitDesk.ViewModels;
using FluentAvalonia.UI.Controls;
using FluentAvalonia.UI.Navigation;
using System.Diagnostics;
using System.Linq;

namespace BitDesk.Views.ChartPages;

public partial class XlmJpyPage : UserControl
{   
    public XlmJpyPage()
    {
        var mainViewModel = App.GetService<MainViewModel>();
        var pairViewModel = mainViewModel.Pairs.FirstOrDefault(p => p.PairCode == Models.PairCodes.xlm_jpy);
        if (pairViewModel != null)
        {
            this.DataContext = pairViewModel;
        }
        else
        {
            Debug.WriteLine("XlmJpyPage: PairViewModel for xlm_jpy not found.");
        }

        InitializeComponent();

        this.Loaded += XlmJpyPage_Loaded;
        AddHandler(FAFrame.NavigatedToEvent, OnNavigatedTo);
        AddHandler(FAFrame.NavigatedFromEvent, OnNavigatedFrom);
    }

    private void XlmJpyPage_Loaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //Debug.WriteLine("XlmJpyPage loaded. DataContext: " + (this.DataContext?.ToString() ?? "null"));
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