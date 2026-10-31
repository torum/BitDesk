using Avalonia.Threading;
using BitDesk.Models;
using BitDesk.Services.Contracts;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BitDesk.ViewModels;

internal class MainViewModel : ObservableObject, IDisposable
{
    private readonly bool _isWebSocketOn = true;
    private readonly bool _isLiveChartsOn = true;
    private readonly bool _isAssetsOn = true;

    private string _userKey = string.Empty;

    private readonly IBitbankStreamService _bitbankStreamService;
    private readonly IBitbankRestService _bitbankRestService;
    private readonly IConfigurationService _configurationService;

    public event EventHandler<IsEnabledState>? IsEnabledChanged;

    public MainViewModel()
    {
        // This constructor is for design-time data context in XAML preview. It should not be used in production code.

        // Sutisfiy the non-null for design-time purposes.
        _bitbankStreamService = App.GetService<IBitbankStreamService>();
        _configurationService = App.GetService<IConfigurationService>();
        _bitbankRestService = App.GetService<IBitbankRestService>();
    }

    public MainViewModel(IBitbankStreamService bitbankStreamService, IConfigurationService configurationService, IBitbankRestService bitbankRestService)
    {
        _bitbankStreamService = bitbankStreamService;
        _configurationService = configurationService;
        _bitbankRestService = bitbankRestService;

        var config = _configurationService.CurrentConfiguration;
        IsSaveLog = config.IsSaveLog;
        IsNavigationViewMenuOpen = config.IsNavigationViewMenuOpen;

        IsOnBtcJpy = config.IsOnBtcJpy;
        IsOnEthJpy = config.IsOnEthJpy;
        IsOnXrpJpy = config.IsOnXrpJpy;
        IsOnXlmJpy = config.IsOnXlmJpy;

        IsEnabledChanged += OnIsEnabledChanged;

        _bitbankStreamService.TickerUpdated += OnBitbankStreamService_TickerUpdated;
        _bitbankStreamService.DepthUpdated += OnBitbankStreamService_DepthUpdated;
        _bitbankStreamService.ErrorOccured += OnErrorOccured;

        // For developing purposes, we can set this to true to avoid starting the ticker service and charts loading.
        _isWebSocketOn = true;
        _isLiveChartsOn = true;
        _isAssetsOn = true;

        // TODO:
#if DEBUG
        IsSaveLog = true;
#endif
    }

    #region == Properties ==

    // TODO:
    public bool IsSaveLog { get; }

    public string VersionText
    {
        get
        {
            return GetVersionText();
        }
    }

    private static string GetVersionText()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version!;

        return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }

    #region == View related ==

    public bool IsNavigationViewMenuOpen { get; set; } = true;

    public int UniformGridColumns
    {
        get
        {
            int count = EnabledPairs.Count;//Pairs.Count(p => p.IsEnabled);
            if (count <= 0) return 1;
            if (count == 1) return 1;
            if (count == 2) return 2;
            if (count <= 4) return 2;
            if (count <= 6) return 3;
            if (count <= 9) return 3;
            if (count <= 12) return 4;
            if (count <= 16) return 4;
            if (count <= 20) return 5;
            return 5; // Default to a maximum of 5 columns for more than 20 pairs.
        }
    }

    #endregion

    #region == Pairs ==

    public ObservableCollection<PairViewModel> Pairs { get; } = [
        new PairViewModel(PairCodes.btc_jpy, 24, "{0:#,0}", "C", 100M, 1000M),
        new PairViewModel(PairCodes.xrp_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.eth_jpy, 24, "{0:#,0}", "C", 100M, 1000M),
        new PairViewModel(PairCodes.xlm_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        /*
        new PairViewModel(PairCodes.ltc_jpy, 24, "{0:#,0.0}", "C1", 100M, 1000M),
        new PairViewModel(PairCodes.mona_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 1M),
        new PairViewModel(PairCodes.bcc_jpy, 24, "{0:#,0}", "C", 10M, 100M),
        new PairViewModel(PairCodes.qtum_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.bat_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.omg_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.xym_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.link_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.mkr_jpy, 24, "{0:#,0}", "C", 100M, 1000M),// to sky_jpy SKY/JPY
        new PairViewModel(PairCodes.boba_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.enj_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),// no longer supported
        new PairViewModel(PairCodes.matic_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M), // to pol_jpy POL/JPY
        new PairViewModel(PairCodes.dot_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.doge_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.astr_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.ada_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.avax_jpy, 24, "{0:#,0.000}", "C3", 100M, 1000M),
        new PairViewModel(PairCodes.axs_jpy, 24, "{0:#,0.000}", "C3", 100M, 1000M),
        new PairViewModel(PairCodes.flr_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        new PairViewModel(PairCodes.sand_jpy, 24, "{0:#,0.000}", "C3", 0.1M, 0.01M),
        */
    ];

    public ObservableCollection<PairViewModel> EnabledPairs
    {
        get
        {
            return field ?? new ObservableCollection<PairViewModel>(Pairs.Where(x => x.IsEnabled));
        }
        private set
        {
            field = value;
            OnPropertyChanged(nameof(EnabledPairs));
        }
    }

    private PairViewModel? _selectedPair;
    public PairViewModel? SelectedPair
    {
        get => _selectedPair;
        set
        {
            if (_selectedPair == value)
            {
                return;
            }

            _selectedPair = value;

            if (_selectedPair != null)
            {
                foreach (var hoge in Pairs)
                {
                    if (hoge.PairCode == _selectedPair.PairCode)
                    {
                        hoge.IsSelectedActive = true;
                    }
                    else
                    {
                        hoge.IsSelectedActive = false;
                    }
                }
            }

            OnPropertyChanged();
        }
    }

    #endregion

    #region == IsOn ==

    private bool _isOnBtcJpy = true;
    public bool IsOnBtcJpy
    {
        get => _isOnBtcJpy;
        set
        {
            if (_isOnBtcJpy != value)
            {
                _isOnBtcJpy = value;
                OnPropertyChanged(nameof(IsOnBtcJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.btc_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnXrpJpy = true;
    public bool IsOnXrpJpy
    {
        get => _isOnXrpJpy;
        set
        {
            if (_isOnXrpJpy != value)
            {
                _isOnXrpJpy = value;
                OnPropertyChanged(nameof(IsOnXrpJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.xrp_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnEthJpy = true;
    public bool IsOnEthJpy
    {
        get => _isOnEthJpy;
        set
        {
            if (_isOnEthJpy != value)
            {
                _isOnEthJpy = value;
                OnPropertyChanged(nameof(IsOnEthJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.eth_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnLtcJpy = false;
    public bool IsOnLtcJpy
    {
        get => _isOnLtcJpy;
        set
        {
            if (_isOnLtcJpy != value)
            {
                _isOnLtcJpy = value;
                OnPropertyChanged(nameof(IsOnLtcJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.ltc_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnBccJpy = false;
    public bool IsOnBccJpy
    {
        get => _isOnBccJpy;
        set
        {
            if (_isOnBccJpy != value)
            {
                _isOnBccJpy = value;
                OnPropertyChanged(nameof(IsOnBccJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.bcc_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnMonaJpy = false;
    public bool IsOnMonaJpy
    {
        get => _isOnMonaJpy;
        set
        {
            if (_isOnMonaJpy != value)
            {
                _isOnMonaJpy = value;
                OnPropertyChanged(nameof(IsOnMonaJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.mona_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnXlmJpy = false;
    public bool IsOnXlmJpy
    {
        get => _isOnXlmJpy;
        set
        {
            if (_isOnXlmJpy != value)
            {
                _isOnXlmJpy = value;
                OnPropertyChanged(nameof(IsOnXlmJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.xlm_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnQtumJpy = false;
    public bool IsOnQtumJpy
    {
        get => _isOnQtumJpy;
        set
        {
            if (_isOnQtumJpy != value)
            {
                _isOnQtumJpy = value;
                OnPropertyChanged(nameof(IsOnQtumJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.qtum_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnBatJpy = false;
    public bool IsOnBatJpy
    {
        get => _isOnBatJpy;
        set
        {
            if (_isOnBatJpy != value)
            {
                _isOnBatJpy = value;
                OnPropertyChanged(nameof(IsOnBatJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.bat_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnOmgJpy = false;
    public bool IsOnOmgJpy
    {
        get => _isOnOmgJpy;
        set
        {
            if (_isOnOmgJpy != value)
            {
                _isOnOmgJpy = value;
                OnPropertyChanged(nameof(IsOnOmgJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.omg_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnXymJpy = false;
    public bool IsOnXymJpy
    {
        get => _isOnXymJpy;
        set
        {
            if (_isOnXymJpy != value)
            {
                _isOnXymJpy = value;
                OnPropertyChanged(nameof(IsOnXymJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.xym_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnLinkJpy = false;
    public bool IsOnLinkJpy
    {
        get => _isOnLinkJpy;
        set
        {
            if (_isOnLinkJpy != value)
            {
                _isOnLinkJpy = value;
                OnPropertyChanged(nameof(IsOnLinkJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.link_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnMkrJpy = false;
    public bool IsOnMkrJpy
    {
        get => _isOnMkrJpy;
        set
        {
            if (_isOnMkrJpy != value)
            {
                _isOnMkrJpy = value;
                OnPropertyChanged(nameof(IsOnMkrJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.mkr_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnBobaJpy = false;
    public bool IsOnBobaJpy
    {
        get => _isOnBobaJpy;
        set
        {
            if (_isOnBobaJpy != value)
            {
                _isOnBobaJpy = value;
                OnPropertyChanged(nameof(IsOnBobaJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.boba_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnEnjJpy = false;
    public bool IsOnEnjJpy
    {
        get => _isOnEnjJpy;
        set
        {
            if (_isOnEnjJpy != value)
            {
                _isOnEnjJpy = value;
                OnPropertyChanged(nameof(IsOnEnjJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.enj_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnMaticJpy = false;
    public bool IsOnMaticJpy
    {
        get => _isOnMaticJpy;
        set
        {
            if (_isOnMaticJpy != value)
            {
                _isOnMaticJpy = value;
                OnPropertyChanged(nameof(IsOnMaticJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.matic_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnDotJpy = false;
    public bool IsOnDotJpy
    {
        get => _isOnDotJpy;
        set
        {
            if (_isOnDotJpy != value)
            {
                _isOnDotJpy = value;
                OnPropertyChanged(nameof(IsOnDotJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.dot_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnDogeJpy = false;
    public bool IsOnDogeJpy
    {
        get => _isOnDogeJpy;
        set
        {
            if (_isOnDogeJpy != value)
            {
                _isOnDogeJpy = value;
                OnPropertyChanged(nameof(IsOnDogeJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.doge_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnAstrJpy = false;
    public bool IsOnAstrJpy
    {
        get => _isOnAstrJpy;
        set
        {
            if (_isOnAstrJpy != value)
            {
                _isOnAstrJpy = value;
                OnPropertyChanged(nameof(IsOnAstrJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.astr_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnAdaJpy = false;
    public bool IsOnAdaJpy
    {
        get => _isOnAdaJpy;
        set
        {
            if (_isOnAdaJpy != value)
            {
                _isOnAdaJpy = value;
                OnPropertyChanged(nameof(IsOnAdaJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.ada_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnAvaxJpy = false;
    public bool IsOnAvaxJpy
    {
        get => _isOnAvaxJpy;
        set
        {
            if (_isOnAvaxJpy != value)
            {
                _isOnAvaxJpy = value;
                OnPropertyChanged(nameof(IsOnAvaxJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.avax_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnAxsJpy = false;
    public bool IsOnAxsJpy
    {
        get => _isOnAxsJpy;
        set
        {
            if (_isOnAxsJpy != value)
            {
                _isOnAxsJpy = value;
                OnPropertyChanged(nameof(IsOnAxsJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.axs_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnFlrJpy = false;
    public bool IsOnFlrJpy
    {
        get => _isOnFlrJpy;
        set
        {
            if (_isOnFlrJpy != value)
            {
                _isOnFlrJpy = value;
                OnPropertyChanged(nameof(IsOnFlrJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.flr_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    private bool _isOnSandJpy = false;
    public bool IsOnSandJpy
    {
        get => _isOnSandJpy;
        set
        {
            if (_isOnSandJpy != value)
            {
                _isOnSandJpy = value;
                OnPropertyChanged(nameof(IsOnSandJpy));
            }

            var pair = Pairs.FirstOrDefault(x => x.PairCode == PairCodes.sand_jpy);
            if (pair is not null)
            {
                if (pair.IsEnabled != value)
                {
                    pair.IsEnabled = value;
                    IsEnabledChanged?.Invoke(this, new IsEnabledState { IsEnabled = value, PairCode = pair.PairCode });
                    OnPropertyChanged(nameof(EnabledPairs));
                    OnPropertyChanged(nameof(UniformGridColumns));
                }
            }
        }
    }

    #endregion

    #region == LTP PercentChange ==

    private string _ltpBtcJpyPercentChange = "";
    public string LtpBtcJpyPercentChange
    {
        get => _ltpBtcJpyPercentChange;
        set
        {
            if (_ltpBtcJpyPercentChange == value)
            {
                return;
            }

            _ltpBtcJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpBtcJpyPercentChange));
        }
    }

    private string _ltpXrpJpyPercentChange = "";
    public string LtpXrpJpyPercentChange
    {
        get => _ltpXrpJpyPercentChange;
        set
        {
            if (_ltpXrpJpyPercentChange == value)
            {
                return;
            }

            _ltpXrpJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpXrpJpyPercentChange));
        }
    }

    private string _ltpEthJpyPercentChange = "";
    public string LtpEthJpyPercentChange
    {
        get => _ltpEthJpyPercentChange;
        set
        {
            if (_ltpEthJpyPercentChange == value)
            {
                return;
            }

            _ltpEthJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpEthJpyPercentChange));
        }
    }

    private string _ltpLtcJpyPercentChange = "";
    public string LtpLtcJpyPercentChange
    {
        get => _ltpLtcJpyPercentChange;
        set
        {
            if (_ltpLtcJpyPercentChange == value)
            {
                return;
            }

            _ltpLtcJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpLtcJpyPercentChange));
        }
    }

    private string _ltpBccJpyPercentChange = "";
    public string LtpBccJpyPercentChange
    {
        get => _ltpBccJpyPercentChange;
        set
        {
            if (_ltpBccJpyPercentChange == value)
            {
                return;
            }

            _ltpBccJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpBccJpyPercentChange));
        }
    }


    private string _ltpMonaJpyPercentChange = "";
    public string LtpMonaJpyPercentChange
    {
        get => _ltpMonaJpyPercentChange;
        set
        {
            if (_ltpMonaJpyPercentChange == value)
            {
                return;
            }

            _ltpMonaJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpMonaJpyPercentChange));
        }
    }

    private string _ltpXlmJpyPercentChange = "";
    public string LtpXlmJpyPercentChange
    {
        get => _ltpXlmJpyPercentChange;
        set
        {
            if (_ltpXlmJpyPercentChange == value)
            {
                return;
            }

            _ltpXlmJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpXlmJpyPercentChange));
        }
    }

    private string _ltpQtumJpyPercentChange = "";
    public string LtpQtumJpyPercentChange
    {
        get => _ltpQtumJpyPercentChange;
        set
        {
            if (_ltpQtumJpyPercentChange == value)
            {
                return;
            }

            _ltpQtumJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpQtumJpyPercentChange));
        }
    }

    private string _ltpBatJpyPercentChange = "";
    public string LtpBatJpyPercentChange
    {
        get => _ltpBatJpyPercentChange;
        set
        {
            if (_ltpBatJpyPercentChange == value)
            {
                return;
            }

            _ltpBatJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpBatJpyPercentChange));
        }
    }

    private string _ltpOmgJpyPercentChange = "";
    public string LtpOmgJpyPercentChange
    {
        get => _ltpOmgJpyPercentChange;
        set
        {
            if (_ltpOmgJpyPercentChange == value)
            {
                return;
            }

            _ltpOmgJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpOmgJpyPercentChange));
        }
    }

    private string _ltpXymJpyPercentChange = "";
    public string LtpXymJpyPercentChange
    {
        get => _ltpXymJpyPercentChange;
        set
        {
            if (_ltpXymJpyPercentChange == value)
            {
                return;
            }

            _ltpXymJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpXymJpyPercentChange));
        }
    }

    private string _ltpLinkJpyPercentChange = "";
    public string LtpLinkJpyPercentChange
    {
        get => _ltpLinkJpyPercentChange;
        set
        {
            if (_ltpLinkJpyPercentChange == value)
            {
                return;
            }

            _ltpLinkJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpLinkJpyPercentChange));
        }
    }

    private string _ltpMkrJpyPercentChange = "";
    public string LtpMkrJpyPercentChange
    {
        get => _ltpMkrJpyPercentChange;
        set
        {
            if (_ltpMkrJpyPercentChange == value)
            {
                return;
            }

            _ltpMkrJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpMkrJpyPercentChange));
        }
    }


    private string _ltpBobaJpyPercentChange = "";
    public string LtpBobaJpyPercentChange
    {
        get => _ltpBobaJpyPercentChange;
        set
        {
            if (_ltpBobaJpyPercentChange == value)
            {
                return;
            }

            _ltpBobaJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpBobaJpyPercentChange));
        }
    }

    private string _ltpEnjJpyPercentChange = "";
    public string LtpEnjJpyPercentChange
    {
        get => _ltpEnjJpyPercentChange;
        set
        {
            if (_ltpEnjJpyPercentChange == value)
            {
                return;
            }

            _ltpEnjJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpEnjJpyPercentChange));
        }
    }


    private string _ltpMaticJpyPercentChange = "";
    public string LtpMaticJpyPercentChange
    {
        get => _ltpMaticJpyPercentChange;
        set
        {
            if (_ltpMaticJpyPercentChange == value)
            {
                return;
            }

            _ltpMaticJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpMaticJpyPercentChange));
        }
    }

    private string _ltpDotJpyPercentChange = "";
    public string LtpDotJpyPercentChange
    {
        get => _ltpDotJpyPercentChange;
        set
        {
            if (_ltpDotJpyPercentChange == value)
            {
                return;
            }

            _ltpDotJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpDotJpyPercentChange));
        }
    }

    private string _ltpDogeJpyPercentChange = "";
    public string LtpDogeJpyPercentChange
    {
        get => _ltpDogeJpyPercentChange;
        set
        {
            if (_ltpDogeJpyPercentChange == value)
            {
                return;
            }

            _ltpDogeJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpDogeJpyPercentChange));
        }
    }

    private string _ltpAstrJpyPercentChange = "";
    public string LtpAstrJpyPercentChange
    {
        get => _ltpAstrJpyPercentChange;
        set
        {
            if (_ltpAstrJpyPercentChange == value)
            {
                return;
            }

            _ltpAstrJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpAstrJpyPercentChange));
        }
    }

    private string _ltpAdaJpyPercentChange = "";
    public string LtpAdaJpyPercentChange
    {
        get => _ltpAdaJpyPercentChange;
        set
        {
            if (_ltpAdaJpyPercentChange == value)
            {
                return;
            }

            _ltpAdaJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpAdaJpyPercentChange));
        }
    }


    private string _ltpAvaxJpyPercentChange = "";
    public string LtpAvaxJpyPercentChange
    {
        get => _ltpAvaxJpyPercentChange;
        set
        {
            if (_ltpAvaxJpyPercentChange == value)
            {
                return;
            }

            _ltpAvaxJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpAvaxJpyPercentChange));
        }
    }


    private string _ltpAxsJpyPercentChange = "";
    public string LtpAxsJpyPercentChange
    {
        get => _ltpAxsJpyPercentChange;
        set
        {
            if (_ltpAxsJpyPercentChange == value)
            {
                return;
            }

            _ltpAxsJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpAxsJpyPercentChange));
        }
    }

    private string _ltpFlrJpyPercentChange = "";
    public string LtpFlrJpyPercentChange
    {
        get => _ltpFlrJpyPercentChange;
        set
        {
            if (_ltpFlrJpyPercentChange == value)
            {
                return;
            }

            _ltpFlrJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpFlrJpyPercentChange));
        }
    }

    private string _ltpSandJpyPercentChange = "";
    public string LtpSandJpyPercentChange
    {
        get => _ltpSandJpyPercentChange;
        set
        {
            if (_ltpSandJpyPercentChange == value)
            {
                return;
            }

            _ltpSandJpyPercentChange = value;
            OnPropertyChanged(nameof(LtpSandJpyPercentChange));
        }
    }

    #endregion

    #region == LTP ==

    private string _ltpBtcJpy = "...";
    public string LtpBtcJpy
    {
        get => _ltpBtcJpy;
        set
        {
            if (_ltpBtcJpy == value)
            {
                return;
            }

            _ltpBtcJpy = value;
            OnPropertyChanged(nameof(LtpBtcJpy));
        }
    }

    private string _ltpXrpJpy = "...";
    public string LtpXrpJpy
    {
        get => _ltpXrpJpy;
        set
        {
            if (_ltpXrpJpy == value)
            {
                return;
            }

            _ltpXrpJpy = value;
            OnPropertyChanged(nameof(LtpXrpJpy));
        }
    }

    private string _ltpEthJpy = "...";
    public string LtpEthJpy
    {
        get => _ltpEthJpy;
        set
        {
            if (_ltpEthJpy == value)
            {
                return;
            }

            _ltpEthJpy = value;
            OnPropertyChanged(nameof(LtpEthJpy));
        }
    }

    private string _ltpLtcJpy = "...";
    public string LtpLtcJpy
    {
        get => _ltpLtcJpy;
        set
        {
            if (_ltpLtcJpy == value)
            {
                return;
            }

            _ltpLtcJpy = value;
            OnPropertyChanged(nameof(LtpLtcJpy));
        }
    }

    private string _ltpMonaJpy = "...";
    public string LtpMonaJpy
    {
        get => _ltpMonaJpy;
        set
        {
            if (_ltpMonaJpy == value)
            {
                return;
            }

            _ltpMonaJpy = value;
            OnPropertyChanged(nameof(LtpMonaJpy));
        }
    }

    private string _ltpBccJpy = "...";
    public string LtpBccJpy
    {
        get => _ltpBccJpy;
        set
        {
            if (_ltpBccJpy == value)
            {
                return;
            }

            _ltpBccJpy = value;
            OnPropertyChanged(nameof(LtpBccJpy));
        }
    }

    private string _ltpXlmJpy = "...";
    public string LtpXlmJpy
    {
        get => _ltpXlmJpy;
        set
        {
            if (_ltpXlmJpy == value)
            {
                return;
            }

            _ltpXlmJpy = value;
            OnPropertyChanged(nameof(LtpXlmJpy));
        }
    }

    private string _ltpQtumJpy = "...";
    public string LtpQtumJpy
    {
        get => _ltpQtumJpy;
        set
        {
            if (_ltpQtumJpy == value)
            {
                return;
            }

            _ltpQtumJpy = value;
            OnPropertyChanged(nameof(LtpQtumJpy));
        }
    }

    private string _ltpBatJpy = "...";
    public string LtpBatJpy
    {
        get => _ltpBatJpy;
        set
        {
            if (_ltpBatJpy == value)
            {
                return;
            }

            _ltpBatJpy = value;
            OnPropertyChanged(nameof(LtpBatJpy));
        }
    }

    private string _ltpOmgJpy = "...";
    public string LtpOmgJpy
    {
        get => _ltpOmgJpy;
        set
        {
            if (_ltpOmgJpy == value)
            {
                return;
            }

            _ltpOmgJpy = value;
            OnPropertyChanged(nameof(LtpOmgJpy));
        }
    }

    private string _ltpXymJpy = "...";
    public string LtpXymJpy
    {
        get => _ltpXymJpy;
        set
        {
            if (_ltpXymJpy == value)
            {
                return;
            }

            _ltpXymJpy = value;
            OnPropertyChanged(nameof(LtpXymJpy));
        }
    }

    private string _ltpLinkJpy = "...";
    public string LtpLinkJpy
    {
        get => _ltpLinkJpy;
        set
        {
            if (_ltpLinkJpy == value)
            {
                return;
            }

            _ltpLinkJpy = value;
            OnPropertyChanged(nameof(LtpLinkJpy));
        }
    }

    private string _ltpMkrJpy = "...";
    public string LtpMkrJpy
    {
        get => _ltpMkrJpy;
        set
        {
            if (_ltpMkrJpy == value)
            {
                return;
            }

            _ltpMkrJpy = value;
            OnPropertyChanged(nameof(LtpMkrJpy));
        }
    }

    private string _ltpBobaJpy = "...";
    public string LtpBobaJpy
    {
        get => _ltpBobaJpy;
        set
        {
            if (_ltpBobaJpy == value)
            {
                return;
            }

            _ltpBobaJpy = value;
            OnPropertyChanged(nameof(LtpBobaJpy));
        }
    }

    private string _ltpEnjJpy = "...";
    public string LtpEnjJpy
    {
        get => _ltpEnjJpy;
        set
        {
            if (_ltpEnjJpy == value)
            {
                return;
            }

            _ltpEnjJpy = value;
            OnPropertyChanged(nameof(LtpEnjJpy));
        }
    }

    private string _ltpMaticJpy = "...";
    public string LtpMaticJpy
    {
        get => _ltpMaticJpy;
        set
        {
            if (_ltpMaticJpy == value)
            {
                return;
            }

            _ltpMaticJpy = value;
            OnPropertyChanged(nameof(LtpMaticJpy));
        }
    }

    private string _ltpDotJpy = "...";
    public string LtpDotJpy
    {
        get => _ltpDotJpy;
        set
        {
            if (_ltpDotJpy == value)
            {
                return;
            }

            _ltpDotJpy = value;
            OnPropertyChanged(nameof(LtpDotJpy));
        }
    }

    private string _ltpDogeJpy = "...";
    public string LtpDogeJpy
    {
        get => _ltpDogeJpy;
        set
        {
            if (_ltpDogeJpy == value)
            {
                return;
            }

            _ltpDogeJpy = value;
            OnPropertyChanged(nameof(LtpDogeJpy));
        }
    }

    private string _ltpAstrJpy = "...";
    public string LtpAstrJpy
    {
        get => _ltpAstrJpy;
        set
        {
            if (_ltpAstrJpy == value)
            {
                return;
            }

            _ltpAstrJpy = value;
            OnPropertyChanged(nameof(LtpAstrJpy));
        }
    }

    private string _ltpAdaJpy = "...";
    public string LtpAdaJpy
    {
        get => _ltpAdaJpy;
        set
        {
            if (_ltpAdaJpy == value)
            {
                return;
            }

            _ltpAdaJpy = value;
            OnPropertyChanged(nameof(LtpAdaJpy));
        }
    }

    private string _ltpAvaxJpy = "...";
    public string LtpAvaxJpy
    {
        get => _ltpAvaxJpy;
        set
        {
            if (_ltpAvaxJpy == value)
            {
                return;
            }

            _ltpAvaxJpy = value;
            OnPropertyChanged(nameof(LtpAvaxJpy));
        }
    }

    private string _ltpAxsJpy = "...";
    public string LtpAxsJpy
    {
        get => _ltpAxsJpy;
        set
        {
            if (_ltpAxsJpy == value)
            {
                return;
            }

            _ltpAxsJpy = value;
            OnPropertyChanged(nameof(LtpAxsJpy));
        }
    }

    private string _ltpFlrJpy = "...";
    public string LtpFlrJpy
    {
        get => _ltpFlrJpy;
        set
        {
            if (_ltpFlrJpy == value)
            {
                return;
            }

            _ltpFlrJpy = value;
            OnPropertyChanged(nameof(LtpFlrJpy));
        }
    }

    private string _ltpSandJpy = "...";
    public string LtpSandJpy
    {
        get => _ltpSandJpy;
        set
        {
            if (_ltpSandJpy == value)
            {
                return;
            }

            _ltpSandJpy = value;
            OnPropertyChanged(nameof(LtpSandJpy));
        }
    }


    #endregion

    #region == Error ==

    public FAInfoBarSeverity InfoBarSeverity
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    } = FAInfoBarSeverity.Warning;

    public string InfoBarMessage
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    } = string.Empty;

    public string InfoBarTitle
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    } = string.Empty;

    public bool IsInfoBarOpen
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnPropertyChanged();
        }
    }

    #endregion

    #region == Assets ==

    private ObservableCollection<Models.Asset> _assets = [];
    public ObservableCollection<Models.Asset> Assets
    {
        get => _assets;
        set
        {
            if (_assets == value)
            {
                return;
            }

            _assets = value;
            OnPropertyChanged(nameof(Assets));
        }
    }

    #endregion

    #region == API keys and secrets ==

    // Assets

    private string _assetsApiKey = string.Empty;
    public string AssetsApiKey
    {
        get => _assetsApiKey;
        set
        {
            if (_assetsApiKey == value.Trim())
            {
                return;
            }

            _assetsApiKey = value.Trim();
            OnPropertyChanged(nameof(AssetsApiKey));

            if (!string.IsNullOrEmpty(_assetsApiKey) && !string.IsNullOrEmpty(_assetsSecret))
            {
                AssetsApiKeyIsSet = true;
            }
        }
    }

    // TODO: encrypt
    private string _assetsSecret = string.Empty;
    public string AssetsSecret
    {
        get => _assetsSecret;
        set
        {
            if (_assetsSecret == value.Trim())
            {
                return;
            }

            _assetsSecret = value.Trim();
            OnPropertyChanged(nameof(AssetsSecret));

            if (!string.IsNullOrEmpty(_assetsApiKey) && !string.IsNullOrEmpty(_assetsSecret))
            {
                AssetsApiKeyIsSet = true;
            }
        }
    }

    private bool _assetsApiKeyIsSet;
    public bool AssetsApiKeyIsSet
    {
        get => _assetsApiKeyIsSet;
        set
        {
            if (_assetsApiKeyIsSet == value)
            {
                return;
            }

            _assetsApiKeyIsSet = value;
            OnPropertyChanged(nameof(AssetsApiKeyIsSet));

            //this.NotifyPropertyChanged("ShowAsset");
            //this.NotifyPropertyChanged("ShowAssetApiSet");
        }
    }

    // Orders

    private string _ordersApiKey = string.Empty;
    public string OrdersApiKey
    {
        get => _ordersApiKey;
        set
        {
            if (_ordersApiKey == value.Trim())
            {
                return;
            }

            _ordersApiKey = value.Trim();
            OnPropertyChanged(nameof(OrdersApiKey));

            if (!string.IsNullOrEmpty(_ordersApiKey) && !string.IsNullOrEmpty(_ordersSecret))
            {
                AssetsApiKeyIsSet = true;
            }
        }
    }

    // TODO: encrypt
    private string _ordersSecret = string.Empty;
    public string OrdersSecret
    {
        get => _ordersSecret;
        set
        {
            if (_ordersSecret == value.Trim())
            {
                return;
            }

            _ordersSecret = value.Trim();
            OnPropertyChanged(nameof(OrdersSecret));

            if (!string.IsNullOrEmpty(_ordersApiKey) && !string.IsNullOrEmpty(_ordersSecret))
            {
                OrdersApiKeyIsSet = true;
            }
        }
    }

    private bool _ordersApiKeyIsSet;
    public bool OrdersApiKeyIsSet
    {
        get => _ordersApiKeyIsSet;
        set
        {
            if (_ordersApiKeyIsSet == value)
            {
                return;
            }

            _ordersApiKeyIsSet = value;
            OnPropertyChanged(nameof(OrdersApiKeyIsSet));
        }
    }

    // TradeHistory 

    private string _tradeHistoryApiKey = string.Empty;
    public string TradeHistoryApiKey
    {
        get => _tradeHistoryApiKey;
        set
        {
            if (_tradeHistoryApiKey == value.Trim())
            {
                return;
            }

            _tradeHistoryApiKey = value.Trim();
            OnPropertyChanged(nameof(TradeHistoryApiKey));

            if (!string.IsNullOrEmpty(_tradeHistoryApiKey) && !string.IsNullOrEmpty(_tradeHistorySecret))
            {
                TradeHistoryApiKeyIsSet = true;
            }
        }
    }

    // TODO: encrypt
    private string _tradeHistorySecret = string.Empty;
    public string TradeHistorySecret
    {
        get => _tradeHistorySecret;
        set
        {
            if (_tradeHistorySecret == value.Trim())
            {
                return;
            }

            _tradeHistorySecret = value.Trim();
            OnPropertyChanged(nameof(TradeHistorySecret));

            if (!string.IsNullOrEmpty(_tradeHistoryApiKey) && !string.IsNullOrEmpty(_tradeHistorySecret))
            {
                TradeHistoryApiKeyIsSet = true;
            }
        }
    }

    private bool _tradeHistoryApiKeyIsSet;
    public bool TradeHistoryApiKeyIsSet
    {
        get => _tradeHistoryApiKeyIsSet;
        set
        {
            if (_tradeHistoryApiKeyIsSet == value)
            {
                return;
            }

            _tradeHistoryApiKeyIsSet = value;
            OnPropertyChanged(nameof(TradeHistoryApiKeyIsSet));
        }
    }


    // Make order

    private string _makeOrderApiKey = string.Empty;
    public string MakeOrderApiKey
    {
        get => _makeOrderApiKey;
        set
        {
            if (_makeOrderApiKey == value.Trim())
            {
                return;
            }

            _makeOrderApiKey = value.Trim();
            OnPropertyChanged(nameof(MakeOrderApiKey));

            if (!string.IsNullOrEmpty(_makeOrderApiKey) && !string.IsNullOrEmpty(_makeOrderSecret))
            {
                MakeOrderApiKeyIsSet = true;
            }
        }
    }

    // TODO: encrypt
    private string _makeOrderSecret = string.Empty;
    public string MakeOrderSecret
    {
        get => _makeOrderSecret;
        set
        {
            if (_makeOrderSecret == value.Trim())
            {
                return;
            }

            _makeOrderSecret = value.Trim();
            OnPropertyChanged(nameof(MakeOrderSecret));

            if (!string.IsNullOrEmpty(_makeOrderApiKey) && !string.IsNullOrEmpty(_makeOrderSecret))
            {
                MakeOrderApiKeyIsSet = true;
            }
        }
    }

    private bool _makeOrderApiKeyIsSet;
    public bool MakeOrderApiKeyIsSet
    {
        get => _makeOrderApiKeyIsSet;
        set
        {
            if (_makeOrderApiKeyIsSet == value)
            {
                return;
            }

            _makeOrderApiKeyIsSet = value;
            OnPropertyChanged(nameof(MakeOrderApiKeyIsSet));

            //this.NotifyPropertyChanged("ShowAsset");
            //this.NotifyPropertyChanged("ShowAssetApiSet");
        }
    }

    #endregion

    #endregion

    #region == Public Methods ==

    public async Task InitializeAndStartAsync(string userKey)
    {
        _userKey = userKey;

        try
        {
            var config = _configurationService.CurrentConfiguration;

            AssetsApiKey = Helper.EncryptDecrypt.Decrypt(config.AssetsApiKey, _userKey, config.User.UserSalt);
            AssetsSecret = Helper.EncryptDecrypt.Decrypt(config.AssetsSecret, _userKey, config.User.UserSalt);
            OrdersApiKey = Helper.EncryptDecrypt.Decrypt(config.OrdersApiKey, _userKey, config.User.UserSalt);
            OrdersSecret = Helper.EncryptDecrypt.Decrypt(config.OrdersSecret, _userKey, config.User.UserSalt);
            TradeHistoryApiKey = Helper.EncryptDecrypt.Decrypt(config.TradeHistoryApiKey, _userKey, config.User.UserSalt);
            TradeHistorySecret = Helper.EncryptDecrypt.Decrypt(config.TradeHistorySecret, _userKey, config.User.UserSalt);
            MakeOrderApiKey = Helper.EncryptDecrypt.Decrypt(config.MakeOrderApiKey, _userKey, config.User.UserSalt);
            MakeOrderSecret = Helper.EncryptDecrypt.Decrypt(config.MakeOrderSecret, _userKey, config.User.UserSalt);

            // Load the pairs from the configuration service into the Pairs collection
            _configurationService.GetPairs(Pairs, _userKey, config.User.UserSalt);

            foreach (var pair in Pairs)
            {
                pair.ErrorOccured += OnErrorOccured;

                pair.TradeHistoryApiKey = TradeHistoryApiKey;
                pair.TradeHistorySecret = TradeHistorySecret;
                pair.OrdersApiKey = OrdersApiKey;
                pair.OrdersSecret = OrdersSecret;
                pair.MakeOrderApiKey = MakeOrderApiKey;
                pair.MakeOrderSecret = MakeOrderSecret;
            }

            _bitbankStreamService.Initialize(Pairs.Where(x => x.IsEnabled).Select(x => x.PairCode).ToList());

            if (_isWebSocketOn)
                await _bitbankStreamService.StartAsync();

            if (_isAssetsOn)
                await GetAssetsAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in InitializeAndStartAsync: {ex}");
        }
    }

    public async Task SetSelectedPairFromCodeAsync(PairCodes pairCode)
    {
        if (_selectedPair?.PairCode == pairCode)
        {
            return;
        }
        else
        {
            //Debug.WriteLine($"SetSelectedPairFromCodeAsync() called for {pairCode} at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        }

        try
        {
            SelectedPair = EnabledPairs.FirstOrDefault(x => x.PairCode == pairCode);

            if (SelectedPair is not null)
            {
                if (_isLiveChartsOn)
                    await SelectedPair.InitializeAndLoadAsync();
            }
            else
            {
                Debug.WriteLine($"No PairViewModel found for {pairCode} at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in SetSelectedPairFromCodeAsync: {ex}");
        }

    }

    public async Task SetSelectedAllPairsFromCodeAsync()
    {
        try
        {
            foreach (var pairVM in EnabledPairs)
            {
                pairVM.IsSelectedActive = true;

                if (_isLiveChartsOn)
                    await pairVM.InitializeAndLoadAsync();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in SetSelectedAllPairsFromCodeAsync: {ex}");
        }
    }

    public void UpdateCurrentSettings()
    {
        var config = _configurationService.CurrentConfiguration;

        config.IsSaveLog = IsSaveLog;

        config.IsNavigationViewMenuOpen = IsNavigationViewMenuOpen;

        config.IsOnBtcJpy = IsOnBtcJpy;
        config.IsOnEthJpy = IsOnEthJpy;
        config.IsOnXrpJpy = IsOnXrpJpy;
        config.IsOnXlmJpy = IsOnXlmJpy;

        config.AssetsApiKey = Helper.EncryptDecrypt.Encrypt(AssetsApiKey, _userKey, config.User.UserSalt);
        config.AssetsSecret = Helper.EncryptDecrypt.Encrypt(AssetsSecret, _userKey, config.User.UserSalt);
        config.OrdersApiKey = Helper.EncryptDecrypt.Encrypt(OrdersApiKey, _userKey, config.User.UserSalt);
        config.OrdersSecret = Helper.EncryptDecrypt.Encrypt(OrdersSecret, _userKey, config.User.UserSalt);
        config.TradeHistoryApiKey = Helper.EncryptDecrypt.Encrypt(TradeHistoryApiKey, _userKey, config.User.UserSalt);
        config.TradeHistorySecret = Helper.EncryptDecrypt.Encrypt(TradeHistorySecret, _userKey, config.User.UserSalt);
        config.MakeOrderApiKey = Helper.EncryptDecrypt.Encrypt(MakeOrderApiKey, _userKey, config.User.UserSalt);
        config.MakeOrderSecret = Helper.EncryptDecrypt.Encrypt(MakeOrderSecret, _userKey, config.User.UserSalt);

        _configurationService.SetUserKey(_userKey, config.User.UserSalt);
    }

    public void Dispose()
    {
        _bitbankStreamService.TickerUpdated -= OnBitbankStreamService_TickerUpdated;
        _bitbankStreamService.DepthUpdated -= OnBitbankStreamService_DepthUpdated;
        _bitbankStreamService.ErrorOccured -= OnErrorOccured;

        IsEnabledChanged -= OnIsEnabledChanged;

        try
        {
            foreach (var pair in Pairs)
            {
                pair.ErrorOccured -= OnErrorOccured;
                pair.Dispose();
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error disposing PairViewModels: {ex}");
        }

        GC.SuppressFinalize(this);

        Debug.WriteLine($"MainViewModel disposed at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    }

    #endregion

    #region == Private Event Handlers ==

    private async void OnIsEnabledChanged(object? sender, IsEnabledState e)
    {
        // Update the EnabledPairs collection whenever a pair's IsEnabled state changes
        EnabledPairs = new ObservableCollection<PairViewModel>(Pairs.Where(x => x.IsEnabled));

        try
        {
            if (e.IsEnabled)
            {
                await _bitbankStreamService.JoinTickerRoomAsync(e.PairCode);
            }
            else
            {
                await _bitbankStreamService.LeaveTickerRoomAsync(e.PairCode);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error in OnIsEnabledChanged: {ex}");
        }
    }

    private async void OnBitbankStreamService_TickerUpdated(object? sender, BitbankTickerData data)
    {
        /*
        PairCodes pair = data.TickerPair;
        DateTime timeStamp = DateTimeOffset.FromUnixTimeMilliseconds(data.Timestamp).LocalDateTime;

        // Calculate a quick math metric (e.g., daily percentage change)
        decimal priceChange = data.Last - data.Open;
        decimal percentChange = data.Open != 0 ? (priceChange / data.Open) * 100 : 0;

        // Print out your typed metrics using standard number formatting
        Debug.WriteLine($"=== {pair} NUMERIC DATA [{timeStamp:yyyy-MM-dd HH:mm:ss}] ===");
        Debug.WriteLine($"  Last Price : {data.Last:N0} JPY ({percentChange:+0.00;-0.00;0.00}%)");
        Debug.WriteLine($"  Bid (Buy)  : {data.Buy:N0}   | Ask (Sell) : {data.Sell:N0}");
        Debug.WriteLine($"  24h High   : {data.High:N0}  | 24h Low    : {data.Low:N0}");
        Debug.WriteLine($"  24h Volume : {data.Vol:N4}");
        Debug.WriteLine(new string('=', 45));
        */

        var pairViewModel = Pairs.FirstOrDefault(p => p.PairCode == data.TickerPair);
        if (pairViewModel is not null)
        {
            if (pairViewModel.IsEnabled == false)
            {
                return;
            }

            await pairViewModel.UpdateTickerDataAsync(data);

            decimal priceChange = data.Last - data.Open;
            decimal percentChange = data.Open != 0 ? (priceChange / data.Open) * 100 : 0;

            Dispatcher.UIThread.Post(() =>
            {
                switch (data.TickerPair)
                {
                    case PairCodes.btc_jpy:
                        LtpBtcJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpBtcJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.xrp_jpy:
                        LtpXrpJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpXrpJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.eth_jpy:
                        LtpEthJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpEthJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.ltc_jpy:
                        LtpLtcJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpLtcJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.mona_jpy:
                        LtpMonaJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpMonaJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.bcc_jpy:
                        LtpBccJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpBccJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.xlm_jpy:
                        LtpXlmJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpXlmJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.qtum_jpy:
                        LtpQtumJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpQtumJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.bat_jpy:
                        LtpBatJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpBatJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.omg_jpy:
                        LtpOmgJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpOmgJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.xym_jpy:
                        LtpXymJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpXymJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.link_jpy:
                        LtpLinkJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpLinkJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.mkr_jpy:
                        LtpMkrJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpMkrJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.boba_jpy:
                        LtpBobaJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpBobaJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.enj_jpy:
                        LtpEnjJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpEnjJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.matic_jpy:
                        LtpMaticJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpMaticJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.dot_jpy:
                        LtpDotJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpDotJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.doge_jpy:
                        LtpDogeJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpDogeJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.astr_jpy:
                        LtpAstrJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpAstrJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.ada_jpy:
                        LtpAdaJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpAdaJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.avax_jpy:
                        LtpAvaxJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpAvaxJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.axs_jpy:
                        LtpAxsJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpAxsJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.flr_jpy:
                        LtpFlrJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpFlrJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    case PairCodes.sand_jpy:
                        LtpSandJpy = string.Format(pairViewModel.LtpFormstString, data.Last);
                        LtpSandJpyPercentChange = string.Format("{0:+0.00;-0.00;0.00}%", percentChange);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            });
        }
        else
        {
            Debug.WriteLine($"No PairViewModel found for {data.TickerPair} at {DateTimeOffset.FromUnixTimeMilliseconds(data.Timestamp).LocalDateTime:yyyy-MM-dd HH:mm:ss}");
        }
    }

    private async void OnBitbankStreamService_DepthUpdated(object? sender, BitbankDepthData data)
    {
        var pairViewModel = Pairs.FirstOrDefault(p => p.PairCode == data.DepthPair);
        if (pairViewModel is not null)
        {
            if (pairViewModel.IsEnabled == false)
            {
                return;
            }

            await pairViewModel.UpdateDepthDataAsync(data);
        }
        else
        {
            Debug.WriteLine($"No PairViewModel found for {data.DepthPair} at {DateTimeOffset.FromUnixTimeMilliseconds(data.Timestamp).LocalDateTime:yyyy-MM-dd HH:mm:ss}");
        }
    }

    private void OnErrorOccured(object? sender, ErrorInfo errorInfo)
    {
        Dispatcher.UIThread.Post(() =>
        {
            InfoBarSeverity = errorInfo.ErrorSeverity;
            InfoBarMessage = errorInfo.ErrorMessage;
            InfoBarTitle = errorInfo.ErrorTitle;
            IsInfoBarOpen = true;
        });
    }


    #endregion

    #region == Private methods ==

    private async Task GetAssetsAsync()
    {
        if (AssetsApiKeyIsSet == false)
        {
            // TODO: show message?
            System.Diagnostics.Debug.WriteLine("■■■■■ GetAssets: (AssetsApiKeyIsSet == false)");

            var err = new ErrorInfo(ErrType.Other)
            {
                ErrorSeverity = FAInfoBarSeverity.Informational,
                ErrorMessage = "資産情報の取得には、APIキーとシークレットが必要です。設定のページでAPIキーとシークレットを設定してください。",
            };

            OnErrorOccured(this, err);

            return;
        }

        try
        {
            // TODO: Use AssetsResult
            var asts = await _bitbankRestService.GetAssetListAsync(AssetsApiKey, AssetsSecret);

            if (asts != null)
            {
                try
                {
                    var newAssets = new ObservableCollection<Models.Asset>();

                    foreach (var ast in asts)
                    {
                        if (ast.Name == "jpy")
                        {
                            foreach (var ps in EnabledPairs)
                            {
                                ps.AssetJPYName = ast.Name.ToUpper();
                                ps.AssetJPYAmount = ast.Amount;
                                ps.AssetJPYFreeAmount = ast.FreeAmount;
                            }
                        }
                        else
                        {
                            if (ast.Amount > 0)
                            {
                                ast.Name = ast.Name.ToUpper();
                                newAssets.Add(ast);
                            }

                            var pr = EnabledPairs.FirstOrDefault(x => x.CurrencyUnitString.Equals(ast.Name, StringComparison.OrdinalIgnoreCase));
                            if (pr is not null)
                            {
                                pr.AssetCurrentName = ast.Name.ToUpper();
                                pr.AssetCurrentAmount = ast.Amount;
                                pr.AssetCurrentFreeAmount = ast.FreeAmount;
                            }
                        }
                    }

                    //Assets = newAssets;
                    foreach (var ors in newAssets)
                    {
                        var found = Assets.FirstOrDefault(x => x.Name == ors.Name);
                        if (found != null)
                        {
                            found = ors;
                        }
                        else
                        {
                            Assets.Add(ors);
                        }
                    }
                    var lst = new List<Asset>();
                    foreach (var ors in Assets)
                    {
                        var found = newAssets.FirstOrDefault(x => x.Name == ors.Name);
                        if (found == null)
                        {
                            lst.Add(ors);
                        }
                    }

                    foreach (var ff in lst)
                    {
                        var found = Assets.FirstOrDefault(x => x.Name == ff.Name);
                        if (found != null)
                        {
                            Assets.Remove(ff);
                        }
                    }

                    //await Task.Delay(1000);
                    return;
                }
                catch (Exception ex)
                {
                    //System.Diagnostics.Debug.WriteLine("■■■■■ GetAssets: Exception - " + ex.Message);
                    if (ex.InnerException != null)
                    {
                        System.Diagnostics.Debug.WriteLine("■■■■■ GetAssets InnerException1: " + ex.InnerException.Message);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("■■■■■ GetAssets Exception1: " + ex.Message);
                    }

                    //await Task.Delay(1000);
                    return;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetAssets: 取得失敗");

                //APIResultAssets = "<<取得失敗>>";

                await Task.Delay(1000);
                return;
            }

        }
        catch (Exception e)
        {
            if (e.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetAssets InnerException2: " + e.InnerException.Message);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetAssets Exception2: " + e.Message);
            }

            await Task.Delay(1000);
            return;
        }

    }

    #endregion

}