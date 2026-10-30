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

        InitializeComponent();

        if (pairViewModel != null)
        {
            this.DataContext = pairViewModel;
        }
        else
        {
            Debug.WriteLine("XlmJpyPage: PairViewModel for xlm_jpy not found.");
        }

        AddHandler(FAFrame.NavigatedToEvent, OnNavigatedTo);
        AddHandler(FAFrame.NavigatedFromEvent, OnNavigatedFrom);
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