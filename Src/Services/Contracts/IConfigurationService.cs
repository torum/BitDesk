using BitDesk.Models;
using BitDesk.ViewModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BitDesk.Services.Contracts;

internal interface IConfigurationService
{
    void Initialize(string configFilePath);
    Task<Models.Configuration> LoadConfigurationAsync();
    Task SaveConfigurationAsync(ObservableCollection<PairViewModel> pairs);
    Models.Configuration CurrentConfiguration { get; }
    void GetPairs(ObservableCollection<PairViewModel> pairs);
}