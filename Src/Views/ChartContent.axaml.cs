using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BitDesk.ViewModels;
using System.Threading.Tasks;

namespace BitDesk.Views;

public partial class ChartContent : UserControl
{
    public ChartContent()
    {
        InitializeComponent();
    }

    // 1. Define the StyledProperty (equivalent to DependencyProperty)
    // In Avalonia, we use StyledProperty<T> or DirectProperty<T>
    public static readonly StyledProperty<PairViewModel?> PairVMProperty = AvaloniaProperty.Register<ChartContent, PairViewModel?>(name: nameof(PairVM), defaultValue: null);

    // 2. Wrap it with a standard property getter/setter
    public PairViewModel? PairVM
    {
        get => GetValue(PairVMProperty);
        set => SetValue(PairVMProperty, value);
    }

    // 3. Register the property changed callback using Avalonia's static constructor paradigm
    static ChartContent()
    {
        // Affects renders/measures or just tracks logic changes cleanly
        PairVMProperty.Changed.AddClassHandler<ChartContent, PairViewModel?>((x, e) => x.OnViewModelChanged(e));
    }

    // 4. Handle instance changes cleanly to prevent memory leaks!
    private void OnViewModelChanged(AvaloniaPropertyChangedEventArgs<PairViewModel?> args)
    {
        // Unsubscribe from the OLD view model if it exists
        if (args.OldValue.Value is PairViewModel oldVm)
        {
            // Assuming it's an action or event, unhook it to prevent memory leaks
            oldVm.DepthScrollCenter -= OnDepthScrollCenter;
        }

        // Subscribe to the NEW view model
        if (args.NewValue.Value is PairViewModel newVm)
        {
            // Avalonia idiomatic way to bind your local handler
            newVm.DepthScrollCenter += OnDepthScrollCenter;
            this.DataContext = PairVM;
        }
    }

    private void OnDepthScrollCenter(object? sender, bool e)
    {
        SetDepthListboxScrollPosition();
    }

    private async void SetDepthListboxScrollPosition()
    {
        await Task.Delay(300);
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (DepthScrollViewer != null)
            {
                // Calculate the centered coordinates
                double centerX = (DepthScrollViewer.Extent.Width - DepthScrollViewer.Viewport.Width) / 2;
                double centerY = (DepthScrollViewer.Extent.Height - DepthScrollViewer.Viewport.Height) / 2;

                // Apply the new position via a Vector (clamped automatically by Avalonia)
                DepthScrollViewer.Offset = new Vector(centerX, centerY);
            }
        });
    }

    private void DepthScrollViewer_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        SetDepthListboxScrollPosition();
    }

    private void DepthListBox_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        SetDepthListboxScrollPosition();
    }

    private void SplitViewToggleButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ChartSplitView.IsPaneOpen = !ChartSplitView.IsPaneOpen;
    }
}