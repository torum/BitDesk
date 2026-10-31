using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Text;

namespace BitDesk.Behaviors;

public static class TextBlockExtensions
{
    public static readonly AttachedProperty<bool> BlinkOnChangeProperty = AvaloniaProperty.RegisterAttached<TextBlock, bool>("BlinkOnChange", typeof(TextBlockExtensions), false);

    public static bool GetBlinkOnChange(TextBlock element) => element.GetValue(BlinkOnChangeProperty);
    public static void SetBlinkOnChange(TextBlock element, bool value) => element.SetValue(BlinkOnChangeProperty, value);

    static TextBlockExtensions()
    {
        BlinkOnChangeProperty.Changed.AddClassHandler<TextBlock>((textBlock, args) => OnBlinkOnChangePropertyValueChanged(textBlock, args));
    }

    private static void OnBlinkOnChangePropertyValueChanged(TextBlock textBlock, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.NewValue is bool isEnabled)
        {
            if (isEnabled)
            {
                // Subscribe strictly to this single TextBlock instance's Text changes
                // Using Avalonia's built-in .PropertyChanged event instead of IObservable
                textBlock.PropertyChanged += OnTextBlockPropertyChanged;
            }
            else
            {
                // Clean up event handler if it gets disabled or the element is discarded
                textBlock.PropertyChanged -= OnTextBlockPropertyChanged;
            }
        }
    }

    private static void OnTextBlockPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs args)
    {
        // 3. Ensure we only care about the Text property changing on this specific instance
        if (sender is TextBlock textBlock && args.Property == TextBlock.TextProperty)
        {
            // Prevent animation running during initial setup/loading if desired
            if (args.OldValue == null && args.NewValue as string == string.Empty) return;

            TriggerBlinkAnimation(textBlock);
        }
    }

    private static void TriggerBlinkAnimation(TextBlock textBlock)
    {
        // Define a quick fade-out and fade-in animation
        var animation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(400),
            Easing = new LinearEasing(),
            Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0.0),
                        Setters = { new Setter(Visual.OpacityProperty, 1.0) }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(0.5), // Drops to 20% opacity halfway through
                        Setters = { new Setter(Visual.OpacityProperty, 0.2) }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1.0),
                        Setters = { new Setter(Visual.OpacityProperty, 1.0) }
                    }
                }
        };

        // Run the animation seamlessly on the TextBlock
        animation.RunAsync(textBlock);
    }


    /*
    public static readonly AttachedProperty<IBrush?> FlashColorProperty = AvaloniaProperty.RegisterAttached<string, TextBlock, IBrush?>("FlashColor", null);

    public static IBrush? GetFlashColor(TextBlock element) => element.GetValue(FlashColorProperty);

    public static void SetFlashColor(TextBlock element, IBrush? value) => element.SetValue(FlashColorProperty, value);

    static TextBlockExtensions()
    {
        TextBlock.TextProperty.Changed.AddClassHandler<TextBlock>((sender, e) =>  OnTextChanged(sender, e));
    }

    private static void OnTextChanged(TextBlock textBlock, AvaloniaPropertyChangedEventArgs e)
    {
        var originalBrush = textBlock.Foreground;
        var flashBrush = GetFlashColor(textBlock);
        //var originalBrush = GetFlashColor(textBlock);
        //var flashBrush = new SolidColorBrush(Color.FromRgb(0x66, 0x66, 0x66));//Color.Parse("#666666")

        if (flashBrush == null) return;

        if (textBlock.Transitions == null || textBlock.Transitions.Count == 0)
        {
            textBlock.Transitions = new Transitions
            {
                new BrushTransition
                {
                    Property = TextBlock.ForegroundProperty,
                    Duration = TimeSpan.FromMilliseconds(400)
                }
            };
        }

        var activeTransitions = textBlock.Transitions;

        textBlock.Transitions = null;
        textBlock.Foreground = flashBrush;

        textBlock.Transitions = activeTransitions;

        if (originalBrush != null)
        {
            textBlock.Foreground = originalBrush;
        }

    }
    */
}
