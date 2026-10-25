using CommunityToolkit.Mvvm.ComponentModel;

namespace BitDesk.Models;

public partial class Asset: ObservableObject
{
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            if (_name == value)
            {
                return;
            }

            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }

    private decimal _amount;
    public decimal Amount
    {
        get => _amount;
        set
        {
            if (_amount == value)
            {
                return;
            }

            _amount = value;
            OnPropertyChanged(nameof(Amount));
            OnPropertyChanged(nameof(AmountText));
        }
    }

    public decimal FreeAmount
    {
        get; set;
    }

    public string AmountText =>Amount.ToString();
    /*
get
{
    // TODO: 各通貨に併せて、小数点以下を調整する？

    return Amount.ToString();
}
*/
}



