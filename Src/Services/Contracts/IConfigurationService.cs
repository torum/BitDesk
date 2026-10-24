using BitDesk.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace BitDesk.Services.Contracts;

internal interface IConfigurationService
{
    void Initialize(string configFilePath);
    Task<Models.Configuration> LoadConfigurationAsync();
    Task SaveConfigurationAsync();
    Models.Configuration CurrentConfiguration { get; }
}