using System;
using System.Globalization;
using System.Resources;
using Avalonia.Markup.Xaml;

namespace BitDesk.Localization;

internal static class Strings
{
    private static readonly ResourceManager ResourceManager = new("BitDesk.Localization.Strings", typeof(Strings).Assembly);

    public static string Get(string key) =>
        ResourceManager.GetString(key, CultureInfo.CurrentUICulture)
        ?? throw new InvalidOperationException($"Missing localization resource '{key}'.");

    public static string Format(string key, params object?[] arguments) =>
        string.Format(CultureInfo.CurrentUICulture, Get(key), arguments);
}

public sealed class LocExtension : MarkupExtension
{
    public string Key { get; set; } = string.Empty;

    public override object ProvideValue(IServiceProvider serviceProvider) => Strings.Get(Key);
}
