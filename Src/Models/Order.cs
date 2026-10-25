using BitDesk.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace BitDesk.Models;

// 注文　指値・成行
public enum OrderTypes
{
    limit, market
}

public enum OrderSides
{
    buy, sell
}

public class OrderTypeSelectionForComboBox(OrderTypes key, string label)
{
    public string Label { get; set; } = label;
    public OrderTypes Key { get; set; } = key;
};

// 注文情報クラス（JsonOrderClassから）
public partial class Order : ObservableObject
{
    public ulong OrderID
    {
        get; set;
    }

    public string? Pair
    {
        get; set;
    } // btc_jpy, xrp_jpy, ltc_btc, eth_btc, mona_jpy, mona_btc, bcc_jpy, bcc_btc

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
            if (_side == "buy")
            {
                return "買";
            }
            else if (_side == "sell")
            {
                return "売";
            }
            else
            {
                return "";
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
            if (_type == "market")
            {
                return "成行";
            }
            else if (_type == "limit")
            {
                return "指値";
            }
            else
            {
                return "";
            }
        }
    }

    public decimal StartAmount
    {
        get; set;
    } // 注文時の数量

    private decimal _remainingAmount;// 未約定の数量
    public decimal RemainingAmount
    {
        get => _remainingAmount;
        set
        {
            if (_remainingAmount == value)
            {
                return;
            }

            _remainingAmount = value;
            OnPropertyChanged(nameof(RemainingAmount));
        }
    }

    private decimal _executedAmount;// 約定済み数量
    public decimal ExecutedAmount
    {
        get => _executedAmount;
        set
        {
            if (_executedAmount == value)
            {
                return;
            }

            _executedAmount = value;
            OnPropertyChanged(nameof(ExecutedAmount));
        }
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

    private decimal _averagePrice;// 平均約定価格
    public decimal AveragePrice
    {
        get => _averagePrice;
        set
        {
            if (_averagePrice == value)
            {
                return;
            }

            _averagePrice = value;
            OnPropertyChanged(nameof(AveragePrice));
        }
    }

    private DateTime _orderedAt;// 注文日時(UnixTimeのミリ秒)
    public DateTime OrderedAt
    {
        get => _orderedAt;
        set
        {
            if (_orderedAt == value)
            {
                return;
            }

            _orderedAt = value;
            OnPropertyChanged(nameof(OrderedAt));
            OnPropertyChanged(nameof(OrderdAtText));
        }
    }

    public string OrderdAtText => _orderedAt.ToString("yyyy/MM/dd HH:mm:ss");

    private string _status = string.Empty;// 注文ステータス  -  UNFILLED 注文中, PARTIALLY_FILLED 注文中(一部約定), FULLY_FILLED 約定済み, CANCELED_UNFILLED 取消済, CANCELED_PARTIALLY_FILLED 取消済(一部約定)
    public string Status
    {
        get => _status;
        set
        {
            if (_status == value)
            {
                return;
            }

            _status = value;

            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(StatusText));
            OnPropertyChanged(nameof(IsCancelEnabled));
            OnPropertyChanged(nameof(OrderIsDone));
        }
    }
    public string StatusText
    {
        get
        {
            if (_status == "UNFILLED")
            {
                return "注文中";
            }
            else if (_status == "PARTIALLY_FILLED")
            {
                return "注文中(一部約定)";
            }
            else if (_status == "FULLY_FILLED")
            {
                return "約定済み";
            }
            else if (_status == "CANCELED_UNFILLED")
            {
                return "取消済";
            }
            else if (_status == "CANCELED_PARTIALLY_FILLED")
            {
                return "取消済(一部約定)";
            }
            else
            {
                return "";
            }
        }
    }

    // 現在値差額表示
    private decimal _shushi;
    public decimal Shushi
    {
        get => _shushi;
        set
        {
            if (_shushi == value)
            {
                return;
            }

            _shushi = value;
            OnPropertyChanged(nameof(Shushi));
        }
    }

    // 実質価格
    private decimal _actualPrice;
    public decimal ActualPrice
    {
        get => Math.Floor((_actualPrice * 10000M)) / 10000M;//_actualPrice;
        set
        {
            if (_actualPrice == value)
            {
                return;
            }

            _actualPrice = value;
            OnPropertyChanged(nameof(ActualPrice));
        }
    }

    // キャンセルが効くかどうかのフラグ
    public bool IsCancelEnabled
    {
        get
        {
            if ((_status == "UNFILLED") || (_status == "PARTIALLY_FILLED"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private ErrorInfo _err;
    public ErrorInfo Err
    {
        get => _err;
        set
        {
            if (_err == value)
            {
                return;
            }

            _err = value;
            OnPropertyChanged(nameof(Err));
        }
    }

    public bool _hasErrorInfo;
    public bool HasErrorInfo
    {
        get => _hasErrorInfo;
        set
        {
            if (_hasErrorInfo == value)
            {
                return;
            }

            _hasErrorInfo = value;
            OnPropertyChanged(nameof(HasErrorInfo));
        }
    }

    public bool OrderIsDone
    {
        get
        {
            if (_status == "UNFILLED")
            {
                return false;
            }
            else if (_status == "PARTIALLY_FILLED")
            {
                return false;
            }
            else if (_status == "FULLY_FILLED")
            {
                return true;
            }
            else if (_status == "CANCELED_UNFILLED")
            {
                return true;
            }
            else if (_status == "CANCELED_PARTIALLY_FILLED")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public Order()
    {
        _err = new ErrorInfo();
    }
}

public class Orders
{
    public List<Order> OrderList
    {
        get; set;
    }

    public Orders()
    {
        OrderList = [];
    }
}

public class OrderListResult : Orders
{
    public bool IsSuccess
    {
        get; set;
    }
    public int ApiErrorCode
    {
        get; set;
    }

    public ErrorInfo Err
    {
        get; set;
    }

    public OrderListResult()
    {
        Err = new ErrorInfo();
    }
}

public partial class OrderResult : Order
{
    public bool IsSuccess
    {
        get; set;
    }
    public int ApiErrorCode
    {
        get; set;
    }
}




