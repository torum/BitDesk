using BitDesk.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace BitDesk.Models;

// 取引履歴クラス
public partial class Trade : ObservableObject
{
    public ulong TradeID
    {
        get; set;
    }

    public string Pair
    {
        get; set;
    } = string.Empty;

    public ulong OrderID
    {
        get; set;
    }

    private string _side = string.Empty;// buy または sell
    public string Side
    {
        get => _side;
        set
        {
            if (_side == value)
            {
                return;
            }

            _side = value;
            OnPropertyChanged(nameof(Side));
            OnPropertyChanged(nameof(SideText));
        }
    }

    public string SideText
    {
        get
        {
            if (Side == "buy")
            {
                return "買";
            }
            else if (Side == "sell")
            {
                return "売";
            }
            else
            {
                return string.Empty;
            }
        }
    }

    private string _type = string.Empty;// limit または market, 指値注文の場合はlimit、成行注文の場合はmarket
    public string Type
    {
        get => _type;
        set
        {
            if (_type == value)
            {
                return;
            }

            _type = value;
            OnPropertyChanged(nameof(Type));
            OnPropertyChanged(nameof(TypeText));
        }
    }

    public string TypeText
    {
        get
        {
            if (Type == "market")
            {
                return "成行";
            }
            else if (Type == "limit")
            {
                return "指値";
            }
            else
            {
                return string.Empty;
            }
        }
    }

    public decimal Amount
    {
        get; set;
    }

    private decimal _price;// 注文価格
    public decimal Price
    {
        get => _price;
        set
        {
            if (_price == value)
            {
                return;
            }

            _price = value;
            OnPropertyChanged(nameof(Price));
        }
    }

    public string? MakerTaker
    {
        get; set;
    }

    /*
    public decimal FeeAmountBase
    {
        get; set;
    }
    */

    public decimal FeeAmountQuote
    {
        get; set;
    }
    public decimal FeeOccurredAmountQuote
    {
        get; set;
    }

    private DateTime _executedAt;// 注文日時(UnixTimeのミリ秒)
    public DateTime ExecutedAt
    {
        get => _executedAt;
        set
        {
            if (_executedAt == value)
            {
                return;
            }

            _executedAt = value;
            OnPropertyChanged(nameof(ExecutedAt));
            OnPropertyChanged(nameof(ExecutedAtText));
        }
    }

    public string ExecutedAtText => _executedAt.ToString("yyyy/MM/dd HH:mm:ss");
}

public class TradeHistory
{
    public List<Trade> TradeList
    {
        get; set;
    }

    public TradeHistory()
    {
        TradeList = [];
    }
}




