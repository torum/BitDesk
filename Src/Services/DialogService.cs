using BitDesk.Models;
using BitDesk.Services.Contracts;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;

namespace BitDesk.Services;

internal sealed class DialogService : IDialogService
{
    public DialogService()
    {

    }

    public async Task<string> ShowSetUpDialog()
    {
        var dialog = new FluentAvalonia.UI.Controls.FAContentDialog
        {
            Title = "Set up",
            IsPrimaryButtonEnabled = true,
            PrimaryButtonText = "OK",
            DefaultButton = FluentAvalonia.UI.Controls.FAContentDialogButton.Primary,
            IsSecondaryButtonEnabled = false,
            Content = new Views.SetUpDialog()
            {
                //DataContext = new DialogViewModel()
            }
        };

        var result = await dialog.ShowAsync();

        if (result == FluentAvalonia.UI.Controls.FAContentDialogResult.Primary && dialog.Content is Views.SetUpDialog dlg)
        {
            var str = dlg.UserKeyTextBox.Text ?? string.Empty;

            if (!string.IsNullOrEmpty(str.Trim()))
            {
                return str.Trim();
            }
        }

        return string.Empty;
    }

    public async Task<string> ShowLogOnDialog()
    {
        var dialog = new FluentAvalonia.UI.Controls.FAContentDialog
        {
            Title = "Log on",
            IsPrimaryButtonEnabled = true,
            PrimaryButtonText = "OK",
            DefaultButton = FluentAvalonia.UI.Controls.FAContentDialogButton.Primary,
            IsSecondaryButtonEnabled = false,

            Content = new Views.LogOnDialog()
            {
                //DataContext = new DialogViewModel()
            }
        };

        var result = await dialog.ShowAsync();

        if (result == FluentAvalonia.UI.Controls.FAContentDialogResult.Primary && dialog.Content is Views.LogOnDialog dlg)
        {
            var str = dlg.UserKeyTextBox.Text ?? string.Empty;

            if (!string.IsNullOrEmpty(str.Trim()))
            {
                return str.Trim();
            }
        }

        return string.Empty;
    }
}
