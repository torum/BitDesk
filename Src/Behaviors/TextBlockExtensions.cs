using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Text;

namespace BitDesk.Behaviors;

public static class TextBlockExtensions
{
    // 1. Register the attached property as an IBrush
    public static readonly AttachedProperty<IBrush?> FlashColorProperty =
        AvaloniaProperty.RegisterAttached<string, TextBlock, IBrush?>("FlashColor", null);

    // 2. Update Get/Set accessors to use IBrush
    public static IBrush? GetFlashColor(TextBlock element)
        => element.GetValue(FlashColorProperty);

    public static void SetFlashColor(TextBlock element, IBrush? value)
        => element.SetValue(FlashColorProperty, value);

    // 3. Keep the static class handler
    static TextBlockExtensions()
    {
        TextBlock.TextProperty.Changed.AddClassHandler<TextBlock>((sender, e) =>
            OnTextChanged(sender, e));
    }

    private static void OnTextChanged(TextBlock textBlock, AvaloniaPropertyChangedEventArgs e)
    {
        var originalBrush = textBlock.Foreground;
        var flashBrush = GetFlashColor(textBlock);
        if (flashBrush == null) return;

        // 1. Initialize the transition exactly once per TextBlock
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

        // 2. Detach transitions to snap instantly to the flash color
        textBlock.Transitions = null;
        textBlock.Foreground = flashBrush;

        // 3. Re-attach transitions to animate going forward
        textBlock.Transitions = activeTransitions;

        if (originalBrush != null)
        {
            textBlock.Foreground = originalBrush;
        }
        /*
        // 4. Determine the exact ThemeVariant (e.g., ThemeVariant.Dark) active on this element
        var currentTheme = textBlock.ActualThemeVariant ?? ThemeVariant.Default;

        // 5. Look up the resource passing the explicit theme context
        if (textBlock.TryFindResource("TextFillColorPrimary", currentTheme, out var defaultResource) &&
            defaultResource is Color defaultColor)
        {
            // This will successfully generate the white/near-white brush for Dark theme!
            textBlock.Foreground = new SolidColorBrush(defaultColor);
        }
        else
        {
            // Fail-safe fallbacks if the resource lookup completely goes sideways
            textBlock.Foreground = currentTheme == ThemeVariant.Dark ? Brushes.White : Brushes.Black;
        }
        */

        /*
        var flashBrush = GetFlashColor(textBlock);

        if (flashBrush != null)
        {
            var originalBrush = textBlock.Foreground;

            var animation = new Animation
            {
                Duration = TimeSpan.FromMilliseconds(400),
                FillMode = FillMode.Forward,
                PlaybackDirection = PlaybackDirection.Normal,
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0.0),
                        Setters = { new Setter(TextBlock.ForegroundProperty, flashBrush) }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1.0),
                        Setters = { new Setter(TextBlock.ForegroundProperty, originalBrush) } // Revert to original
                    }
                }
            };

            animation.RunAsync(textBlock);
        }
        */
    }


    /*
    public static readonly AttachedProperty<bool> FlashOnUpdateProperty =
        AvaloniaProperty.RegisterAttached<TextBlock, bool>("FlashOnUpdate", typeof(TextBlockExtensions), false);

    public static bool GetFlashOnUpdate(TextBlock element)
        => element.GetValue(FlashOnUpdateProperty);

    public static void SetFlashOnUpdate(TextBlock element, bool value)
        => element.SetValue(FlashOnUpdateProperty, value); // Pass both the property and the value

    static TextBlockExtensions()
    {
        TextBlock.TextProperty.Changed.AddClassHandler<TextBlock>((sender, e) =>
            OnTextChanged(sender, e));
    }

    private static void OnTextChanged(TextBlock textBlock, AvaloniaPropertyChangedEventArgs e)
    {
        // Check if our attached property "FlashOnUpdate" is set to true on this TextBlock
        if (GetFlashOnUpdate(textBlock))
        {
            TriggerFlashAnimation(textBlock);
        }
    }

    private static void TriggerFlashAnimation(TextBlock textBlock)
    {
        var originalForeground = textBlock.Foreground;

        var animation = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(400),
            FillMode = FillMode.Forward,
            PlaybackDirection = PlaybackDirection.Normal,
            Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0.0),
                        Setters = { new Setter(TextBlock.ForegroundProperty, Brushes.Gainsboro) }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1.0),
                        Setters = { new Setter(TextBlock.ForegroundProperty, originalForeground) } // Revert to original
                    }
                }
        };

        // Run the animation on the TextBlock
        animation.RunAsync(textBlock);
    }
    */
}
