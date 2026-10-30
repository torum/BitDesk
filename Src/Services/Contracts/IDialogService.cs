using BitDesk.Models;
using BitDesk.ViewModels;
using System.Threading.Tasks;
using static BitDesk.Services.DialogService;

namespace BitDesk.Services.Contracts;

internal interface IDialogService
{
    Task<string> ShowSetUpDialog();
    Task<string> ShowLogOnDialog();
}

