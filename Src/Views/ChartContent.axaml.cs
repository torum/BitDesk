using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using BitDesk.ViewModels;

namespace BitDesk.Views;

public partial class ChartContent : UserControl
{
    // 1. Define the StyledProperty (equivalent to DependencyProperty)
    // In Avalonia, we use StyledProperty<T> or DirectProperty<T>
    public static readonly StyledProperty<PairViewModel?> PairVMProperty =
        AvaloniaProperty.Register<ChartContent, PairViewModel?>(
            name: nameof(PairVM),
            defaultValue: null);

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
            // oldVm.DepthScrollCenter -= OnDepthScrollCenter; 
        }

        // Subscribe to the NEW view model
        if (args.NewValue.Value is PairViewModel newVm)
        {
            // Avalonia idiomatic way to bind your local handler
            //newVm.DepthScrollCenter += OnDepthScrollCenter;
        }
    }

    private void OnDepthScrollCenter()
    {
        // Your scroll centering logic goes here
    }

    public ChartContent()
    {
        InitializeComponent();
    }
}