using Bitbank.Common;
using Bitbank.Models.Clients;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Security.Cryptography;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;

namespace Bitbank.ViewModels
{

    #region == その他・定数・クラス等 ==

    // 通貨ペア
    public enum Pairs
    {
        btc_jpy, xrp_jpy, ltc_btc, eth_btc, mona_jpy, mona_btc, bcc_jpy, bcc_btc
    }

    // チャート表示期間
    public enum ChartSpans
    {
        OneHour, ThreeHour, OneDay, ThreeDay, OneWeek, OneMonth, TwoMonth, OneYear, FiveYear
    }

    // 注文　指値・成行
    public enum OrderTypes
    {
        limit, market
    }

    #region == チック履歴 TickHistory クラス ==

    // TickHistoryクラス 
    public class TickHistory
    {
        // 価格
        public decimal Price { get; set; } 
        public String PriceString
        {
            get { return String.Format("{0:#,0}", Price); }
        }

        // 日時
        public DateTime TimeAt { get; set; } 
        // タイムスタンプ文字列
        public string TimeStamp
        {
            get { return TimeAt.ToLocalTime().ToString("HH:mm:ss"); }
        }

        public Color TickHistoryPriceColor { get; set; }

        public TickHistory()
        {

        }
    }

    #endregion

    #region == 特殊注文 IFDOCO クラス ==

    public enum IfdocoTypes
    {
        limit, market
    }
    public enum IfdocoSide
    {
        buy, sell
    }
    public enum IfdocoKinds
    {
        ifd, oco, ifdoco
    }

    // IFDOCOクラス
    public class Ifdoco : ViewModelBase
    {
        private int _id;
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (_id == value)
                    return;

                _id = value;
                this.NotifyPropertyChanged("Id");
            }
        }

        // 種別 [ifd, oco, ifdoco]
        private IfdocoKinds _kind; 
        public IfdocoKinds Kind
        {
            get
            {
                return _kind;
            }
            set
            {
                if (_kind == value)
                    return;

                _kind = value;
                this.NotifyPropertyChanged("Kind");
                this.NotifyPropertyChanged("KindString");
            }
        }
        public string KindString
        {
            get
            {
                return _kind.ToString().ToUpper();
            }
        }

        // 注文ID
        private int _ifdoneOrderID;
        public int IfdoneOrderID
        {
            get
            {
                return _ifdoneOrderID;
            }
            set
            {
                if (_ifdoneOrderID == value)
                    return;

                _ifdoneOrderID = value;
                this.NotifyPropertyChanged("IfdoneOrderID");
            }
        }
        private int _ifdDoOrderID;
        public int IfdDoOrderID
        {
            get
            {
                return _ifdDoOrderID;
            }
            set
            {
                if (_ifdDoOrderID == value)
                    return;

                _ifdDoOrderID = value;
                this.NotifyPropertyChanged("IfdDoOrderID");
            }
        }
        private int _ocoOneOrderID;
        public int OcoOneOrderID
        {
            get
            {
                return _ocoOneOrderID;
            }
            set
            {
                if (_ocoOneOrderID == value)
                    return;

                _ocoOneOrderID = value;
                this.NotifyPropertyChanged("OcoOneOrderID");
            }
        }
        private int _ocoOtherOrderID;
        public int OcoOtherOrderID
        {
            get
            {
                return _ocoOtherOrderID;
            }
            set
            {
                if (_ocoOtherOrderID == value)
                    return;

                _ocoOtherOrderID = value;
                this.NotifyPropertyChanged("OcoOtherOrderID");
            }
        }

        // 注文日時
        private DateTime _ifdoneOrderedAt;
        public DateTime IfdoneOrderedAt
        {
            get
            {
                return _ifdoneOrderedAt;
            }
            set
            {
                if (_ifdoneOrderedAt == value)
                    return;

                _ifdoneOrderedAt = value;
                this.NotifyPropertyChanged("IfdoneOrderedAt");
            }
        }
        private DateTime _ifdDoOrderedAt;
        public DateTime IfdDoOrderedAt
        {
            get
            {
                return _ifdDoOrderedAt;
            }
            set
            {
                if (_ifdDoOrderedAt == value)
                    return;

                _ifdDoOrderedAt = value;
                this.NotifyPropertyChanged("IfdDoOrderedAt");
            }
        }
        private DateTime _ocoOneOrderedAt;
        public DateTime OcoOneOrderedAt
        {
            get
            {
                return _ocoOneOrderedAt;
            }
            set
            {
                if (_ocoOneOrderedAt == value)
                    return;

                _ocoOneOrderedAt = value;
                this.NotifyPropertyChanged("OcoOneOrderedAt");
            }
        }
        private DateTime _ocoOtherOrderedAt;
        public DateTime OcoOtherOrderedAt
        {
            get
            {
                return _ocoOtherOrderedAt;
            }
            set
            {
                if (_ocoOtherOrderedAt == value)
                    return;

                _ocoOtherOrderedAt = value;
                this.NotifyPropertyChanged("OcoOtherOrderedAt");
            }
        }

        // 注文タイプ [limit, market]
        private IfdocoTypes _ifdoneType;
        public IfdocoTypes IfdoneType
        {
            get
            {
                return _ifdoneType;
            }
            set
            {
                if (_ifdoneType == value)
                    return;

                _ifdoneType = value;
                this.NotifyPropertyChanged("IfdoneType");
                this.NotifyPropertyChanged("IfdoneTypeString");
                this.NotifyPropertyChanged("IfdoneTypeText");
            }
        }
        public string IfdoneTypeString
        {
            get
            {
                return _ifdoneType.ToString();
            }
        }
        public string IfdoneTypeText
        {
            get
            {
                if (_ifdoneType == IfdocoTypes.market)
                {
                    return "成行";
                }
                else if (_ifdoneType == IfdocoTypes.limit)
                {
                    return "指値";
                }
                else
                {
                    return "";
                }
            }
        }
        private IfdocoTypes _ifdDoType;
        public IfdocoTypes IfdDoType
        {
            get
            {
                return _ifdDoType;
            }
            set
            {
                if (_ifdDoType == value)
                    return;

                _ifdDoType = value;
                this.NotifyPropertyChanged("IfdDoType");
                this.NotifyPropertyChanged("IfdDoTypeString");
                this.NotifyPropertyChanged("IfdDoTypeText");
            }
        }
        public string IfdDoTypeString
        {
            get
            {
                return IfdDoType.ToString();
            }
        }
        public string IfdDoTypeText
        {
            get
            {
                if (_ifdDoType == IfdocoTypes.market)
                {
                    return "成行";
                }
                else if (_ifdDoType == IfdocoTypes.limit)
                {
                    return "指値";
                }
                else
                {
                    return "";
                }
            }
        }
        private IfdocoTypes _ocoOneType;
        public IfdocoTypes OcoOneType
        {
            get
            {
                return _ocoOneType;
            }
            set
            {
                if (_ocoOneType == value)
                    return;

                _ocoOneType = value;
                this.NotifyPropertyChanged("OcoOneType");
                this.NotifyPropertyChanged("OcoOneTypeString");
                this.NotifyPropertyChanged("OcoOneTypeText");
            }
        }
        public string OcoOneTypeString
        {
            get
            {
                return OcoOneType.ToString();
            }
        }
        public string OcoOneTypeText
        {
            get
            {
                if (_ocoOneType == IfdocoTypes.market)
                {
                    return "成行";
                }
                else if (_ocoOneType == IfdocoTypes.limit)
                {
                    return "指値";
                }
                else
                {
                    return "";
                }
            }
        }
        private IfdocoTypes _ocoOtherType;
        public IfdocoTypes OcoOtherType
        {
            get
            {
                return _ocoOtherType;
            }
            set
            {
                if (_ocoOtherType == value)
                    return;

                _ocoOtherType = value;
                this.NotifyPropertyChanged("OcoOtherType");
                this.NotifyPropertyChanged("OcoOtherTypeString");
                this.NotifyPropertyChanged("OcoOtherTypeText");
            }
        }
        public string OcoOtherTypeString
        {
            get
            {
                return OcoOtherType.ToString();
            }
        }
        public string OcoOtherTypeText
        {
            get
            {
                if (_ocoOtherType == IfdocoTypes.market)
                {
                    return "成行";
                }
                else if (_ocoOtherType == IfdocoTypes.limit)
                {
                    return "指値";
                }
                else
                {
                    return "";
                }
            }
        }

        // 売り買いサイド
        private string _ifdoneSide;
        public string IfdoneSide
        {
            get
            {
                return _ifdoneSide;
            }
            set
            {
                if (_ifdoneSide == value)
                    return;

                _ifdoneSide = value;
                this.NotifyPropertyChanged("IfdoneSide");
                this.NotifyPropertyChanged("IfdoneSideText");
            }
        }
        public string IfdoneSideText
        {
            get
            {
                if (_ifdoneSide == "buy")
                {
                    return "買";
                }
                else if (_ifdoneSide == "sell")
                {
                    return "売";
                }
                else
                {
                    return "";
                }
            }

        }
        private string _ifdDoSide;
        public string IfdDoSide
        {
            get
            {
                return _ifdDoSide;
            }
            set
            {
                if (_ifdDoSide == value)
                    return;

                _ifdDoSide = value;
                this.NotifyPropertyChanged("IfdDoSide");
                this.NotifyPropertyChanged("IfdDoSideText");
            }
        }
        public string IfdDoSideText
        {
            get
            {
                if (_ifdDoSide == "buy")
                {
                    return "買";
                }
                else if (_ifdDoSide == "sell")
                {
                    return "売";
                }
                else
                {
                    return "";
                }
            }

        }
        private string _ocoOneSide;
        public string OcoOneSide
        {
            get
            {
                return _ocoOneSide;
            }
            set
            {
                if (_ocoOneSide == value)
                    return;

                _ocoOneSide = value;
                this.NotifyPropertyChanged("OcoOneSide");
                this.NotifyPropertyChanged("OcoOneSideText");
            }
        }
        public string OcoOneSideText
        {
            get
            {
                if (_ocoOneSide == "buy")
                {
                    return "買";
                }
                else if (_ocoOneSide == "sell")
                {
                    return "売";
                }
                else
                {
                    return "";
                }
            }

        }
        private string _ocoOtherSide;
        public string OcoOtherSide
        {
            get
            {
                return _ocoOtherSide;
            }
            set
            {
                if (_ocoOtherSide == value)
                    return;

                _ocoOtherSide = value;
                this.NotifyPropertyChanged("OcoOtherSide");
                this.NotifyPropertyChanged("OcoOtherSideText");
            }
        }
        public string OcoOtherSideText
        {
            get
            {
                if (_ocoOtherSide == "buy")
                {
                    return "買";
                }
                else if (_ocoOtherSide == "sell")
                {
                    return "売";
                }
                else
                {
                    return "";
                }
            }

        }

        // 注文数量
        private decimal _ifdoneStartAmount;
        public decimal IfdoneStartAmount
        {
            get
            {
                return _ifdoneStartAmount;
            }
            set
            {
                if (_ifdoneStartAmount == value)
                    return;

                _ifdoneStartAmount = value;
                this.NotifyPropertyChanged("IfdoneStartAmount");
            }
        }
        private decimal _ifdDoStartAmount;
        public decimal IfdDoStartAmount
        {
            get
            {
                return _ifdDoStartAmount;
            }
            set
            {
                if (_ifdDoStartAmount == value)
                    return;

                _ifdDoStartAmount = value;
                this.NotifyPropertyChanged("IfdDoStartAmount");
            }
        }
        private decimal _ocoOneStartAmount;
        public decimal OcoOneStartAmount
        {
            get
            {
                return _ocoOneStartAmount;
            }
            set
            {
                if (_ocoOneStartAmount == value)
                    return;

                _ocoOneStartAmount = value;
                this.NotifyPropertyChanged("OcoOneStartAmount");
            }
        }
        private decimal _ocoOtherStartAmount;
        public decimal OcoOtherStartAmount
        {
            get
            {
                return _ocoOtherStartAmount;
            }
            set
            {
                if (_ocoOtherStartAmount == value)
                    return;

                _ocoOtherStartAmount = value;
                this.NotifyPropertyChanged("OcoOtherStartAmount");
            }
        }

        // 未約定数量
        private decimal _ifdoneRemainingAmount;
        public decimal IfdoneRemainingAmount
        {
            get
            {
                return _ifdoneRemainingAmount;
            }
            set
            {
                if (_ifdoneRemainingAmount == value)
                    return;

                _ifdoneRemainingAmount = value;
                this.NotifyPropertyChanged("IfdoneRemainingAmount");
            }
        }
        private decimal _ifdDoRemainingAmount;
        public decimal IfdDoRemainingAmount
        {
            get
            {
                return _ifdDoRemainingAmount;
            }
            set
            {
                if (_ifdDoRemainingAmount == value)
                    return;

                _ifdDoRemainingAmount = value;
                this.NotifyPropertyChanged("IfdDoRemainingAmount");
            }
        }
        private decimal _ocoOneRemainingAmount;
        public decimal OcoOneRemainingAmount
        {
            get
            {
                return _ocoOneRemainingAmount;
            }
            set
            {
                if (_ocoOneRemainingAmount == value)
                    return;

                _ocoOneRemainingAmount = value;
                this.NotifyPropertyChanged("OcoOneRemainingAmount");
            }
        }
        private decimal _ocoOtherRemainingAmount;
        public decimal OcoOtherRemainingAmount
        {
            get
            {
                return _ocoOtherRemainingAmount;
            }
            set
            {
                if (_ocoOtherRemainingAmount == value)
                    return;

                _ocoOtherRemainingAmount = value;
                this.NotifyPropertyChanged("OcoOtherRemainingAmount");
            }
        }

        // 約定数量
        private decimal _ifdoneExecutedAmount;
        public decimal IfdoneExecutedAmount
        {
            get
            {
                return _ifdoneExecutedAmount;
            }
            set
            {
                if (_ifdoneExecutedAmount == value)
                    return;

                _ifdoneExecutedAmount = value;
                this.NotifyPropertyChanged("IfdoneExecutedAmount");
            }
        }
        private decimal _ifdDoExecutedAmount;
        public decimal IfdDoExecutedAmount
        {
            get
            {
                return _ifdDoExecutedAmount;
            }
            set
            {
                if (_ifdDoExecutedAmount == value)
                    return;

                _ifdDoExecutedAmount = value;
                this.NotifyPropertyChanged("IfdDoExecutedAmount");
            }
        }
        private decimal _ocoOneExecutedAmount;
        public decimal OcoOneExecutedAmount
        {
            get
            {
                return _ocoOneExecutedAmount;
            }
            set
            {
                if (_ocoOneExecutedAmount == value)
                    return;

                _ocoOneExecutedAmount = value;
                this.NotifyPropertyChanged("OcoOneExecutedAmount");
            }
        }
        private decimal _ocoOtherExecutedAmount;
        public decimal OcoOtherExecutedAmount
        {
            get
            {
                return _ocoOtherExecutedAmount;
            }
            set
            {
                if (_ocoOtherExecutedAmount == value)
                    return;

                _ocoOtherExecutedAmount = value;
                this.NotifyPropertyChanged("OcoOtherExecutedAmount");
            }
        }

        // 注文価格
        private decimal _ifdonePrice;
        public decimal IfdonePrice
        {
            get
            {
                return _ifdonePrice;
            }
            set
            {
                if (_ifdonePrice == value)
                    return;

                _ifdonePrice = value;
                this.NotifyPropertyChanged("IfdonePrice");
            }
        }
        private decimal _ifdDoPrice;
        public decimal IfdDoPrice
        {
            get
            {
                return _ifdDoPrice;
            }
            set
            {
                if (_ifdDoPrice == value)
                    return;

                _ifdDoPrice = value;
                this.NotifyPropertyChanged("IfdDoPrice");
            }
        }
        private decimal _ocoOnePrice;
        public decimal OcoOnePrice
        {
            get
            {
                return _ocoOnePrice;
            }
            set
            {
                if (_ocoOnePrice == value)
                    return;

                _ocoOnePrice = value;
                this.NotifyPropertyChanged("OcoOnePrice");
            }
        }
        private decimal _ocoOtherPrice;
        public decimal OcoOtherPrice
        {
            get
            {
                return _ocoOtherPrice;
            }
            set
            {
                if (_ocoOtherPrice == value)
                    return;

                _ocoOtherPrice = value;
                this.NotifyPropertyChanged("OcoOtherPrice");
            }
        }

        // 平均約定価格
        private decimal _ifdoneAveragePrice;
        public decimal IfdoneAveragePrice
        {
            get
            {
                return _ifdoneAveragePrice;
            }
            set
            {
                if (_ifdoneAveragePrice == value)
                    return;

                _ifdoneAveragePrice = value;
                this.NotifyPropertyChanged("IfdoneAveragePrice");
            }
        }
        private decimal _ifdDoAveragePrice;
        public decimal IfdDoAveragePrice
        {
            get
            {
                return _ifdDoAveragePrice;
            }
            set
            {
                if (_ifdDoAveragePrice == value)
                    return;

                _ifdDoAveragePrice = value;
                this.NotifyPropertyChanged("IfdDoAveragePrice");
            }
        }
        private decimal _ocoOneAveragePrice;
        public decimal OcoOneAveragePrice
        {
            get
            {
                return _ocoOneAveragePrice;
            }
            set
            {
                if (_ocoOneAveragePrice == value)
                    return;

                _ocoOneAveragePrice = value;
                this.NotifyPropertyChanged("OcoOneAveragePrice");
            }
        }
        private decimal _ocoOtherAveragePrice;
        public decimal OcoOtherAveragePrice
        {
            get
            {
                return _ocoOtherAveragePrice;
            }
            set
            {
                if (_ocoOtherAveragePrice == value)
                    return;

                _ocoOtherAveragePrice = value;
                this.NotifyPropertyChanged("OcoOtherAveragePrice");
            }
        }

        // 約定ステータス
        private string _ifdoneStatus;
        public string IfdoneStatus
        {
            get
            {
                return _ifdoneStatus;
            }
            set
            {
                if (_ifdoneStatus == value)
                    return;

                _ifdoneStatus = value;
                this.NotifyPropertyChanged("IfdoneStatus");
                this.NotifyPropertyChanged("IfdoneStatusText");
            }
        }
        public string IfdoneStatusText
        {
            get
            {
                if (_ifdoneStatus == "UNFILLED")
                {
                    return "注文中";
                }
                else if (_ifdoneStatus == "PARTIALLY_FILLED")
                {
                    return "注文中(一部約定)";
                }
                else if (_ifdoneStatus == "FULLY_FILLED")
                {
                    return "約定済み";
                }
                else if (_ifdoneStatus == "CANCELED_UNFILLED")
                {
                    return "取消済";
                }
                else if (_ifdoneStatus == "CANCELED_PARTIALLY_FILLED")
                {
                    return "取消済(一部約定)";
                }
                else
                {
                    if (_ifdoneOrderID == 0)
                    {
                        return "未発注";
                    }
                    else
                    {
                        return ""; // loading...
                    }
                    
                }
            }
        }
        private string _ifdDoStatus;
        public string IfdDoStatus
        {
            get
            {
                return _ifdDoStatus;
            }
            set
            {
                if (_ifdDoStatus == value)
                    return;

                _ifdDoStatus = value;
                this.NotifyPropertyChanged("IfdDoStatus");
                this.NotifyPropertyChanged("IfdDoStatusText");
            }
        }
        public string IfdDoStatusText
        {
            get
            {
                if (_ifdDoStatus == "UNFILLED")
                {
                    return "注文中";
                }
                else if (_ifdDoStatus == "PARTIALLY_FILLED")
                {
                    return "注文中(一部約定)";
                }
                else if (_ifdDoStatus == "FULLY_FILLED")
                {
                    return "約定済み";
                }
                else if (_ifdDoStatus == "CANCELED_UNFILLED")
                {
                    return "取消済";
                }
                else if (_ifdDoStatus == "CANCELED_PARTIALLY_FILLED")
                {
                    return "取消済(一部約定)";
                }
                else
                {
                    if (_ifdDoOrderID == 0)
                    {
                        return "未発注";
                    }
                    else
                    {
                        return ""; // loading...
                    }
                }
            }
        }
        private string _ocoOneStatus;
        public string OcoOneStatus
        {
            get
            {
                return _ocoOneStatus;
            }
            set
            {
                if (_ocoOneStatus == value)
                    return;

                _ocoOneStatus = value;
                this.NotifyPropertyChanged("OcoOneStatus");
                this.NotifyPropertyChanged("OcoOneStatusText");
            }
        }
        public string OcoOneStatusText
        {
            get
            {
                if (_ocoOneStatus == "UNFILLED")
                {
                    return "注文中";
                }
                else if (_ocoOneStatus == "PARTIALLY_FILLED")
                {
                    return "注文中(一部約定)";
                }
                else if (_ocoOneStatus == "FULLY_FILLED")
                {
                    return "約定済み";
                }
                else if (_ocoOneStatus == "CANCELED_UNFILLED")
                {
                    return "取消済";
                }
                else if (_ocoOneStatus == "CANCELED_PARTIALLY_FILLED")
                {
                    return "取消済(一部約定)";
                }
                else
                {
                    if (_ocoOneOrderID == 0)
                    {
                        return "未発注";
                    }
                    else
                    {
                        return ""; // loading...
                    }
                }
            }
        }
        private string _ocoOtherStatus;
        public string OcoOtherStatus
        {
            get
            {
                return _ocoOtherStatus;
            }
            set
            {
                if (_ocoOtherStatus == value)
                    return;

                _ocoOtherStatus = value;
                this.NotifyPropertyChanged("OcoOtherStatus");
                this.NotifyPropertyChanged("OcoOtherStatusText");
            }
        }
        public string OcoOtherStatusText
        {
            get
            {
                if (_ocoOtherStatus == "UNFILLED")
                {
                    return "注文中";
                }
                else if (_ocoOtherStatus == "PARTIALLY_FILLED")
                {
                    return "注文中(一部約定)";
                }
                else if (_ocoOtherStatus == "FULLY_FILLED")
                {
                    return "約定済み";
                }
                else if (_ocoOtherStatus == "CANCELED_UNFILLED")
                {
                    return "取消済";
                }
                else if (_ocoOtherStatus == "CANCELED_PARTIALLY_FILLED")
                {
                    return "取消済(一部約定)";
                }
                else
                {
                    if (_ocoOtherOrderID == 0)
                    {
                        return "未発注";
                    }
                    else
                    {
                        return ""; // loading...
                    }
                }
            }
        }

        // エラー情報保持クラス
        private ErrorInfo _ifdoneErrorInfo;
        public ErrorInfo IfdoneErrorInfo
        {
            get
            {
                return _ifdoneErrorInfo;
            }
            set
            {
                if (_ifdoneErrorInfo == value)
                    return;

                _ifdoneErrorInfo = value;
                this.NotifyPropertyChanged("IfdoneErrorInfo");
            }
        }
        private ErrorInfo _ifdDoErrorInfo;
        public ErrorInfo IfdDoErrorInfo
        {
            get
            {
                return _ifdDoErrorInfo;
            }
            set
            {
                if (_ifdDoErrorInfo == value)
                    return;

                _ifdDoErrorInfo = value;
                this.NotifyPropertyChanged("IfdDoErrorInfo");
            }
        }
        private ErrorInfo _ocoOneErrorInfo;
        public ErrorInfo OcoOneErrorInfo
        {
            get
            {
                return _ocoOneErrorInfo;
            }
            set
            {
                if (_ocoOneErrorInfo == value)
                    return;

                _ocoOneErrorInfo = value;
                this.NotifyPropertyChanged("OcoOneErrorInfo");
            }
        }
        private ErrorInfo _ocoOtherErrorInfo;
        public ErrorInfo OcoOtherErrorInfo
        {
            get
            {
                return _ocoOtherErrorInfo;
            }
            set
            {
                if (_ocoOtherErrorInfo == value)
                    return;

                _ocoOtherErrorInfo = value;
                this.NotifyPropertyChanged("OcoOtherErrorInfo");
            }
        }

        // エラーフラグ
        private bool _ifdoneHasError;
        public bool IfdoneHasError
        {
            get
            {
                return _ifdoneHasError;
            }
            set
            {
                if (_ifdoneHasError == value)
                    return;

                _ifdoneHasError = value;
                this.NotifyPropertyChanged("IfdoneHasError");
            }
        }
        private bool _ifdDoHasError;
        public bool IfdDoHasError
        {
            get
            {
                return _ifdDoHasError;
            }
            set
            {
                if (_ifdDoHasError == value)
                    return;

                _ifdDoHasError = value;
                this.NotifyPropertyChanged("IfdDoHasError");
            }
        }
        private bool _ocoOneHasError;
        public bool OcoOneHasError
        {
            get
            {
                return _ocoOneHasError;
            }
            set
            {
                if (_ocoOneHasError == value)
                    return;

                _ocoOneHasError = value;
                this.NotifyPropertyChanged("OcoOneHasError");
            }
        }
        private bool _ocoOtherHasError;
        public bool OcoOtherHasError
        {
            get
            {
                return _ocoOtherHasError;
            }
            set
            {
                if (_ocoOtherHasError == value)
                    return;

                _ocoOtherHasError = value;
                this.NotifyPropertyChanged("OcoOtherHasError");
            }
        }

        // 終了フラグ
        private bool _ifdoneIsDone;
        public bool IfdoneIsDone
        {
            get
            {
                return _ifdoneIsDone;
            }
            set
            {
                if (_ifdoneIsDone == value)
                    return;

                _ifdoneIsDone = value;
                this.NotifyPropertyChanged("IfdoneIsDone");
            }
        }
        private bool _ifdDoIsDone;
        public bool IfdDoIsDone
        {
            get
            {
                return _ifdDoIsDone;
            }
            set
            {
                if (_ifdDoIsDone == value)
                    return;

                _ifdDoIsDone = value;
                this.NotifyPropertyChanged("IfdDoIsDone");
            }
        }
        private bool _ocoOneIsDone;
        public bool OcoOneIsDone
        {
            get
            {
                return _ocoOneIsDone;
            }
            set
            {
                if (_ocoOneIsDone == value)
                    return;

                _ocoOneIsDone = value;
                this.NotifyPropertyChanged("OcoOneIsDone");
            }
        }
        private bool _ocoOtherIsDone;
        public bool OcoOtherIsDone
        {
            get
            {
                return _ocoOtherIsDone;
            }
            set
            {
                if (_ocoOtherIsDone == value)
                    return;

                _ocoOtherIsDone = value;
                this.NotifyPropertyChanged("OcoOtherIsDone");
            }
        }

        // 完全終了フラグ
        private bool _ifdIsDone;
        public bool IfdIsDone
        {
            get
            {
                return _ifdIsDone;
            }
            set
            {
                if (_ifdIsDone == value)
                    return;

                _ifdIsDone = value;
                this.NotifyPropertyChanged("IfdIsDone");
                this.NotifyPropertyChanged("IsCancelEnabled");
            }
        }
        private bool _ocoIsDone;
        public bool OcoIsDone
        {
            get
            {
                return _ocoIsDone;
            }
            set
            {
                if (_ocoIsDone == value)
                    return;

                _ocoIsDone = value;
                this.NotifyPropertyChanged("OcoIsDone");
                this.NotifyPropertyChanged("IsCancelEnabled");
            }
        }
        private bool _ifdocoIsDone;
        public bool IfdocoIsDone
        {
            get
            {
                return _ifdocoIsDone;
            }
            set
            {
                if (_ifdocoIsDone == value)
                    return;

                _ifdocoIsDone = value;
                this.NotifyPropertyChanged("IfdocoIsDone");
                this.NotifyPropertyChanged("IsCancelEnabled");
            }
        }

        // キャンセル可フラグ
        public bool IsCancelEnabled
        {
            get
            {
                if (_kind == IfdocoKinds.ifd)
                {
                    if ((IfdoneIsDone && IfdDoIsDone) || IfdIsDone)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
                else if (_kind == IfdocoKinds.oco)
                {
                    if (OcoIsDone)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
                else if (_kind == IfdocoKinds.ifdoco)
                {
                    if (IfdocoIsDone)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        // トリガー価格
        private decimal _ifdDoTriggerPrice;
        public decimal IfdDoTriggerPrice
        {
            get
            {
                return _ifdDoTriggerPrice;
            }
            set
            {
                if (_ifdDoTriggerPrice == value)
                    return;

                _ifdDoTriggerPrice = value;
                this.NotifyPropertyChanged("IfdDoTriggerPrice");
            }
        }
        private decimal _ocoOneTriggerPrice;
        public decimal OcoOneTriggerPrice
        {
            get
            {
                return _ocoOneTriggerPrice;
            }
            set
            {
                if (_ocoOneTriggerPrice == value)
                    return;

                _ocoOneTriggerPrice = value;
                this.NotifyPropertyChanged("OcoOneTriggerPrice");
            }
        }
        private decimal _ocoOtherTriggerPrice;
        public decimal OcoOtherTriggerPrice
        {
            get
            {
                return _ocoOtherTriggerPrice;
            }
            set
            {
                if (_ocoOtherTriggerPrice == value)
                    return;

                _ocoOtherTriggerPrice = value;
                this.NotifyPropertyChanged("OcoOtherTriggerPrice");
            }
        }

        // トリガー 以上 Up (0)、以下 Down (1)
        private int _ifdDoTriggerUpDown;
        public int IfdDoTriggerUpDown
        {
            get
            {
                return _ifdDoTriggerUpDown;
            }
            set
            {
                if (_ifdDoTriggerUpDown == value)
                    return;

                _ifdDoTriggerUpDown = value;
                this.NotifyPropertyChanged("IfdDoTriggerUpDown");
            }
        }
        private int _ocoOneTriggerUpDown;
        public int OcoOneTriggerUpDown
        {
            get
            {
                return _ocoOneTriggerUpDown;
            }
            set
            {
                if (_ocoOneTriggerUpDown == value)
                    return;

                _ocoOneTriggerUpDown = value;
                this.NotifyPropertyChanged("OcoOneTriggerUpDown");
            }
        }
        private int _ocoOtherTriggerUpDown;
        public int OcoOtherTriggerUpDown
        {
            get
            {
                return _ocoOtherTriggerUpDown;
            }
            set
            {
                if (_ocoOtherTriggerUpDown == value)
                    return;

                _ocoOtherTriggerUpDown = value;
                this.NotifyPropertyChanged("OcoOtherTriggerUpDown");
            }
        }

        public Ifdoco()
        {

        }

    }

    #endregion

    #region == 自動取引 AutoTrade クラス ==

    public class AutoTrade2 : ViewModelBase
    {
        // 停滞カウンター
        private int _counter;
        public int Counter
        {
            get
            {
                return _counter;
            }
            set
            {
                if (_counter == value)
                    return;

                _counter = value;
                this.NotifyPropertyChanged("Counter");
            }
        }

        // 売り買い
        private string _buySide;
        public string BuySide
        {
            get
            {
                return _buySide;
            }
            set
            {
                if (_buySide == value)
                    return;

                _buySide = value;
                this.NotifyPropertyChanged("BuySide");
                this.NotifyPropertyChanged("BuySideText");
            }
        }
        public string BuySideText
        {
            get
            {
                if (_buySide == "buy")
                {
                    return "買";
                }
                else if (_buySide == "sell")
                {
                    return "売";
                }
                else
                {
                    return "";
                }
            }
        }

        private string _sellSide;
        public string SellSide
        {
            get
            {
                return _sellSide;
            }
            set
            {
                if (_sellSide == value)
                    return;

                _sellSide = value;
                this.NotifyPropertyChanged("SellSide");
                this.NotifyPropertyChanged("SellSideText");
            }
        }
        public string SellSideText
        {
            get
            {
                if (_sellSide == "buy")
                {
                    return "買";
                }
                else if (_sellSide == "sell")
                {
                    return "売";
                }
                else
                {
                    return "";
                }
            }
        }

        // 買い数量
        private decimal _buyAmount;
        public decimal BuyAmount
        {
            get
            {
                return _buyAmount;
            }
            set
            {
                if (_buyAmount == value)
                    return;

                _buyAmount = value;
                this.NotifyPropertyChanged("BuyAmount");
            }
        }

        // 売り数量
        private decimal _sellAmount;
        public decimal SellAmount
        {
            get
            {
                return _sellAmount;
            }
            set
            {
                if (_sellAmount == value)
                    return;

                _sellAmount = value;
                this.NotifyPropertyChanged("SellAmount");
            }
        }

        // 買い価格
        private decimal _buyPrice;
        public decimal BuyPrice
        {
            get
            {
                return _buyPrice;
            }
            set
            {
                if (_buyPrice == value)
                    return;

                _buyPrice = value;
                this.NotifyPropertyChanged("BuyPrice");
            }
        }

        // 売り価格
        private decimal _sellPrice;
        public decimal SellPrice
        {
            get
            {
                return _sellPrice;
            }
            set
            {
                if (_sellPrice == value)
                    return;

                _sellPrice = value;
                this.NotifyPropertyChanged("SellPrice");
            }
        }

        // 買い約定価格
        private decimal _buyFilledPrice;
        public decimal BuyFilledPrice
        {
            get
            {
                return _buyFilledPrice;
            }
            set
            {
                if (_buyFilledPrice == value)
                    return;

                _buyFilledPrice = value;
                this.NotifyPropertyChanged("BuyFilledPrice");
            }
        }

        // 売り約定価格
        private decimal _sellFilledPrice;
        public decimal SellFilledPrice
        {
            get
            {
                return _sellFilledPrice;
            }
            set
            {
                if (_sellFilledPrice == value)
                    return;

                _sellFilledPrice = value;
                this.NotifyPropertyChanged("SellFilledPrice");
            }
        }

        // 買い注文ID
        private int _buyOrderId;
        public int BuyOrderId
        {
            get
            {
                return _buyOrderId;
            }
            set
            {
                if (_buyOrderId == value)
                    return;

                _buyOrderId = value;
                this.NotifyPropertyChanged("BuyOrderId");
            }
        }

        // 売り注文ID
        private int _sellOrderId;
        public int SellOrderId
        {
            get
            {
                return _sellOrderId;
            }
            set
            {
                if (_sellOrderId == value)
                    return;

                _sellOrderId = value;
                this.NotifyPropertyChanged("SellOrderId");
            }
        }

        // 買い注文ステータス
        private string _buyStatus;
        public string BuyStatus
        {
            get
            {
                return _buyStatus;
            }
            set
            {
                if (_buyStatus == value)
                    return;

                _buyStatus = value;
                this.NotifyPropertyChanged("BuyStatus");
                this.NotifyPropertyChanged("BuyStatusText");
            }
        }

        // 売り注文ステータス
        private string _sellStatus;
        public string SellStatus
        {
            get
            {
                return _sellStatus;
            }
            set
            {
                if (_sellStatus == value)
                    return;

                _sellStatus = value;
                this.NotifyPropertyChanged("SellStatus");
                this.NotifyPropertyChanged("SellStatusText");
            }
        }

        // 買い注文ステータス
        public string BuyStatusText
        {
            get
            {
                if (_buyStatus == "UNFILLED")
                {
                    return "注文中";
                }
                else if (_buyStatus == "PARTIALLY_FILLED")
                {
                    return "注文中(一部約定)";
                }
                else if (_buyStatus == "FULLY_FILLED")
                {
                    return "約定済み";
                }
                else if (_buyStatus == "CANCELED_UNFILLED")
                {
                    return "取消済";
                }
                else if (_buyStatus == "CANCELED_PARTIALLY_FILLED")
                {
                    return "取消済(一部約定)";
                }
                else
                {
                    return "";
                }
            }
        }

        // 売り注文ステータス
        public string SellStatusText
        {
            get
            {
                if (_sellStatus == "UNFILLED")
                {
                    return "注文中";
                }
                else if (_sellStatus == "PARTIALLY_FILLED")
                {
                    return "注文中(一部約定)";
                }
                else if (_sellStatus == "FULLY_FILLED")
                {
                    return "約定済み";
                }
                else if (_sellStatus == "CANCELED_UNFILLED")
                {
                    return "取消済";
                }
                else if (_sellStatus == "CANCELED_PARTIALLY_FILLED")
                {
                    return "取消済(一部約定)";
                }
                else
                {
                    return "";
                }
            }
        }



        // 想定利幅
        private decimal _profitAmount;
        public decimal ProfitAmount
        {
            get
            {
                return _profitAmount;
            }
            set
            {
                if (_profitAmount == value)
                    return;

                _profitAmount = value;
                this.NotifyPropertyChanged("ProfitAmount");
            }
        }
        // 損益結果
        private decimal _shushiAmount;
        public decimal ShushiAmount
        {
            get
            {
                return _shushiAmount;
            }
            set
            {
                if (_shushiAmount == value)
                    return;

                _shushiAmount = value;
                this.NotifyPropertyChanged("ShushiAmount");
            }
        }

        // キャンセルフラグ
        private bool isCanceled;
        public bool IsCanceled
        {
            get
            {
                return isCanceled;
            }
            set
            {
                if (isCanceled == value)
                    return;

                isCanceled = value;
                this.NotifyPropertyChanged("IsCanceled");
            }
        }

        // 買い約定済みフラグ
        private bool _buyIsDone;
        public bool BuyIsDone
        {
            get
            {
                return _buyIsDone;
            }
            set
            {
                if (_buyIsDone == value)
                    return;

                _buyIsDone = value;
                this.NotifyPropertyChanged("BuyIsDone");
            }
        }

        // 売り約定済みフラグ
        private bool _sellIsDone;
        public bool SellIsDone
        {
            get
            {
                return _sellIsDone;
            }
            set
            {
                if (_sellIsDone == value)
                    return;

                _sellIsDone = value;
                this.NotifyPropertyChanged("SellIsDone");
            }
        }

        // 終了フラグ
        private bool _isDone;
        public bool IsDone
        {
            get
            {
                return _isDone;
            }
            set
            {
                if (_isDone == value)
                    return;

                _isDone = value;
                this.NotifyPropertyChanged("IsDone");
            }
        }

        // 買いエラー
        private bool _buyHasError;
        public bool BuyHasError
        {
            get
            {
                return _buyHasError;
            }
            set
            {
                if (_buyHasError == value)
                    return;

                _buyHasError = value;
                this.NotifyPropertyChanged("BuyHasError");
            }
        }

        private ErrorInfo _buyErrorInfo;
        public ErrorInfo BuyErrorInfo
        {
            get
            {
                return _buyErrorInfo;
            }
            set
            {
                if (_buyErrorInfo == value)
                    return;

                _buyErrorInfo = value;
                this.NotifyPropertyChanged("BuyErrorInfo");
            }
        }

        // 売りエラー
        private bool _sellHasError;
        public bool SellHasError
        {
            get
            {
                return _sellHasError;
            }
            set
            {
                if (_sellHasError == value)
                    return;

                _sellHasError = value;
                this.NotifyPropertyChanged("SellHasError");
            }
        }

        private ErrorInfo _sellErrorInfo;
        public ErrorInfo SellErrorInfo
        {
            get
            {
                return _sellErrorInfo;
            }
            set
            {
                if (_sellErrorInfo == value)
                    return;

                _sellErrorInfo = value;
                this.NotifyPropertyChanged("SellErrorInfo");
            }
        }





        public AutoTrade2()
        {

        }
    }

    #endregion

    #region == エラークラス ==

    public class ClientError
    {
        public string ErrType { get; set; }
        public int ErrCode { get; set; }
        public string ErrText { get; set; }
        public string ErrPlace { get; set; }
        public string ErrPlaceParent { get; set; }
        public DateTime ErrDatetime { get; set; }
        public string ErrEx { get; set; }
    }

    #endregion

    #endregion

    /// <summary>
    /// メインのビューモデル
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        #region == カラー定義 ==

        // App.xaml 参照のこと 

        private Color _priceUpColor = (Color)ColorConverter.ConvertFromString("#a0d8ef");//Colors.Aqua;
        private Color _priceDownColor = (Color)ColorConverter.ConvertFromString("#e597b2");//Colors.Pink;
        private Color _priceNeutralColor = Colors.Gainsboro;

        private Color _priceWarningColor = (Color)ColorConverter.ConvertFromString("#FFDFD991");

        private Color _accentCoolColor = (Color)ColorConverter.ConvertFromString("#2c4f54");
        private Color _accentWarmColor = (Color)ColorConverter.ConvertFromString("#e0815e");

        //private Color _chartIncreaseColor = (Color)ColorConverter.ConvertFromString("#008db7");
        //private Color _chartDecreaseColor = (Color)ColorConverter.ConvertFromString("#e95464");

        #endregion

        #region == 変数 ==

        // Application version
        private string _appVer = "0.0.0.1";

        // Application name
        private string _appName = "BitDesk";

        // Application config file folder
        private string _appDeveloper = "torum";

        // 当初投資額
        private decimal _initPrice = 81800M;

        #endregion

        #region == プロパティ ==

        // プライベートモード表示切替（自動取引表示）
        public bool PrivateMode
        {
            get
            {
                return true;
            }
        }

        // デモモード表示切替フラグ (ダミー保有資産額表示)
        public bool DemoMode
        {
            get
            {
                return false;
            }
        }

        // ベータモード表示切替フラグ（特殊注文ｖ0.0.2.0）
        public bool BetaMode
        {
            get
            {
                return true;
            }
        }

        // Application Window Title
        public string AppTitle
        {
            get
            {
                return _appName + " " + _appVer;
            }
        }


        #region == 認証・ログイン関連のプロパティ ==

        private bool _isPasswordSet;
        public bool IsPasswordSet
        {
            get
            {
                return _isPasswordSet;
            }
            set
            {
                if (_isPasswordSet == value) return;

                _isPasswordSet = value;
                this.NotifyPropertyChanged("IsPasswordSet");

                if (_isPasswordSet)
                    ShowPasswordSet = false;
                else
                    ShowPasswordSet = true;
            }
        }

        // パスワード設定画面の表示フラグ
        private bool _showPasswordSet = true;
        public bool ShowPasswordSet
        {
            get
            {
                return _showPasswordSet;
            }
            set
            {
                if (_showPasswordSet == value) return;

                _showPasswordSet = value;
                this.NotifyPropertyChanged("ShowPasswordSet");
            }
        }

        // パスワード設定画面のエラー表示
        private string _logInErrorInfo;
        public string LogInErrorInfo
        {
            get
            {
                return _logInErrorInfo;
            }
            set
            {
                if (_logInErrorInfo == value) return;

                _logInErrorInfo = value;
                this.NotifyPropertyChanged("LogInErrorInfo");
            }
        }

        private string _realPassword = "";
        public string Password
        {
            get
            {
                return ""; //DummyPassword(_realPassword);
                //return _password;
            }
            set
            {
                //ClearErrror("Password");
                //_password = value.Trim();
                this.NotifyPropertyChanged("Password");
            }
        }

        // ログインモード
        private bool _loggedInMode = false;
        public bool LoggedInMode
        {
            get
            {
                return _loggedInMode;
            }
            set
            {
                if (_loggedInMode == value) return;

                _loggedInMode = value;
                this.NotifyPropertyChanged("LoggedInMode");
                this.NotifyPropertyChanged("PublicApiOnlyMode");

                if (_loggedInMode == true)
                    ShowLogIn = false;

                this.NotifyPropertyChanged("ShowAsset");
                this.NotifyPropertyChanged("ShowAssetApiSet");

                this.NotifyPropertyChanged("ShowManualTrade");
                this.NotifyPropertyChanged("ShowManualTradeApiSet");

                this.NotifyPropertyChanged("ShowOrders");
                this.NotifyPropertyChanged("ShowOrdersApiSet");

                this.NotifyPropertyChanged("ShowTradeHistory");
                this.NotifyPropertyChanged("ShowTradeHistoryApiSet");

                this.NotifyPropertyChanged("ShowAutoTrade");
                this.NotifyPropertyChanged("ShowAutoTradeApiSet");

                this.NotifyPropertyChanged("ShowIfdocoTrade");
                this.NotifyPropertyChanged("ShowIfdocoTradeApiSet");

            }
        }

        // ログイン画面の表示フラグ
        private bool _showLogIn;
        public bool ShowLogIn
        {
            get
            {
                return _showLogIn;
            }
            set
            {
                if (_showLogIn == value) return;

                _showLogIn = value;
                this.NotifyPropertyChanged("ShowLogIn");
            }
        }

        // パブリックAPIオンリーモード
        public bool PublicApiOnlyMode
        {
            get
            {
                if (_loggedInMode == false)
                {
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
        }

        // 資産情報APIキー設定画面表示フラグ
        public bool ShowAssetApiSet
        {
            get
            {
                if (AssetsApiKeyIsSet)
                {
                    return false;
                }
                else
                {
                    if (LoggedInMode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        // 資産情報画面表示フラグ
        public bool ShowAsset
        {
            get
            {
                if (AssetsApiKeyIsSet && LoggedInMode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // 売買注文APIキー設定画面表示フラグ
        public bool ShowManualTradeApiSet
        {
            get
            {
                if (ManualTradeApiKeyIsSet)
                {
                    return false;
                }
                else
                {
                    if (LoggedInMode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        // 売買注文画面表示フラグ
        public bool ShowManualTrade
        {
            get
            {
                if (ManualTradeApiKeyIsSet && LoggedInMode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // 注文一覧APIキー設定画面表示フラグ
        public bool ShowOrdersApiSet
        {
            get
            {
                if (OrdersApiKeyIsSet)
                {
                    return false;
                }
                else
                {
                    if (LoggedInMode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        // 注文一覧画面表示フラグ
        public bool ShowOrders
        {
            get
            {
                if (OrdersApiKeyIsSet && LoggedInMode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // 取引履歴一覧APIキー設定画面表示フラグ
        public bool ShowTradeHistoryApiSet
        {
            get
            {
                if (TradeHistoryApiKeyIsSet)
                {
                    return false;
                }
                else
                {
                    if (LoggedInMode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        // 取引履歴一覧画面表示フラグ
        public bool ShowTradeHistory
        {
            get
            {
                if (TradeHistoryApiKeyIsSet && LoggedInMode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // 自動取引APIキー設定画面表示フラグ
        public bool ShowAutoTradeApiSet
        {
            get
            {
                if (AutoTradeApiKeyIsSet)
                {
                    return false;
                }
                else
                {
                    if (LoggedInMode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        // 自動取引画面表示フラグ
        public bool ShowAutoTrade
        {
            get
            {
                if (AutoTradeApiKeyIsSet && LoggedInMode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        // 特殊取引APIキー設定画面表示フラグ
        public bool ShowIfdocoTradeApiSet
        {
            get
            {
                if (IfdocoTradeApiKeyIsSet)
                {
                    return false;
                }
                else
                {
                    if (LoggedInMode)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        // 特殊取引画面表示フラグ
        public bool ShowIfdocoTrade
        {
            get
            {
                if (IfdocoTradeApiKeyIsSet && LoggedInMode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #region == 設定画面のプロパティ ==

        private bool _showSettings = false;
        public bool ShowSettings
        {
            get
            {
                return _showSettings;
            }
            set
            {
                if (_showSettings == value) return;

                _showSettings = value;
                this.NotifyPropertyChanged("ShowSettings");

                if (_showSettings)
                    ShowMainContents = false;
                else
                    ShowMainContents = true;

                if (LoggedInMode)
                    ShowApiKeyLock = true;
                else
                    ShowApiKeyLock = false;
            }
        }

        private bool _showMainContents = true;
        public bool ShowMainContents
        {
            get
            {
                return _showMainContents;
            }
            set
            {
                if (_showMainContents == value) return;

                _showMainContents = value;
                this.NotifyPropertyChanged("ShowMainContents");
            }
        }

        private string _changePasswordResultInfo;
        public string ChangePasswordResultInfo
        {
            get
            {
                return _changePasswordResultInfo;
            }
            set
            {
                if (_changePasswordResultInfo == value) return;

                _changePasswordResultInfo = value;
                this.NotifyPropertyChanged("ChangePasswordResultInfo");
            }
        }


        #endregion

        #region == 通貨ペア切り替え用のプロパティ ==

        // 現在の通貨ペア
        private Pairs _currentPair = Pairs.btc_jpy;//"btc_jpy";
        public Pairs CurrentPair
        {
            get
            {
                return _currentPair;
            }
            set
            {
                if (_currentPair == value)
                    return;

                _currentPair = value;

                this.NotifyPropertyChanged("CurrentPair");
                this.NotifyPropertyChanged("CurrentPairString");

                this.NotifyPropertyChanged("CoinString");
                

            }
        }

        // 表示用 通貨ペア名 "BTC/JPY";
        public string CurrentPairString
        {
            get
            {
                return CurrentPairStrings[CurrentPair];
            }
        }

        public Dictionary<Pairs, decimal> Ltps { get; set; } = new Dictionary<Pairs, decimal>()
        {
            {Pairs.btc_jpy, 0},
            {Pairs.xrp_jpy, 0},
            {Pairs.ltc_btc, 0},
            {Pairs.eth_btc, 0},
            {Pairs.mona_jpy, 0},
            {Pairs.mona_btc, 0},
            {Pairs.bcc_jpy, 0},
            {Pairs.bcc_btc, 0},
        };

        public Dictionary<Pairs, string> CurrentPairStrings { get; set; } = new Dictionary<Pairs, string>()
        {
            {Pairs.btc_jpy, "BTC/JPY"},
            {Pairs.xrp_jpy, "XRP/JPY"},
            {Pairs.eth_btc, "ETH/BTC"},
            {Pairs.ltc_btc, "LTC/BTC"},
            {Pairs.mona_jpy, "MONA/JPY"},
            {Pairs.mona_btc, "MONA/BTC"},
            {Pairs.bcc_jpy, "BCH/JPY"},
            {Pairs.bcc_btc, "BCH/BTC"},
        };

        public Dictionary<string, Pairs> GetPairs { get; set; } = new Dictionary<string, Pairs>()
        {
            {"btc_jpy", Pairs.btc_jpy},
            {"xrp_jpy", Pairs.xrp_jpy},
            {"eth_btc", Pairs.eth_btc},
            {"ltc_btc", Pairs.ltc_btc},
            {"mona_jpy", Pairs.mona_jpy},
            {"mona_btc", Pairs.mona_btc},
            {"bcc_jpy", Pairs.bcc_jpy},
            {"bcc_btc", Pairs.bcc_btc},
        };

        // 表示用 通貨 単位
        public string CoinString
        {
            get
            {
                return CurrentPairUnits[CurrentPair].ToUpper();//_coin.ToUpper();
            }
        }

        public Dictionary<Pairs, string> CurrentPairUnits { get; set; } = new Dictionary<Pairs, string>()
        {
            {Pairs.btc_jpy, "btc"},
            {Pairs.xrp_jpy, "xrp"},
            {Pairs.eth_btc, "eth"},
            {Pairs.ltc_btc, "ltc"},
            {Pairs.mona_jpy, "mona"},
            {Pairs.mona_btc, "mona"},
            {Pairs.bcc_jpy, "bch"},
            {Pairs.bcc_btc, "bch"},
        };

        // アルトコインの最新取引価格

        public decimal LtpBtc
        {
            get
            {
                return Ltps[Pairs.btc_jpy];
            }
        }

        public decimal LtpXrp
        {
            get
            {
                return Ltps[Pairs.xrp_jpy];
            }
        }

        public decimal LtpEthBtc
        {
            get
            {
                return Ltps[Pairs.eth_btc];
            }
        }

        public decimal LtpLtcBtc
        {
            get
            {
                return Ltps[Pairs.ltc_btc];
            }
        }

        public decimal LtpMonaJpy
        {
            get
            {
                return Ltps[Pairs.mona_jpy];
            }
        }

        public decimal LtpMonaBtc
        {
            get
            {
                return Ltps[Pairs.mona_btc];
            }
        }

        public decimal LtpBchJpy
        {
            get
            {
                return Ltps[Pairs.bcc_jpy];
            }
        }

        public decimal LtpBchBtc
        {
            get
            {
                return Ltps[Pairs.bcc_btc];
            }
        }

        // 左メニュータブの選択インデックス
        private int _activePairIndex;
        public int ActivePairIndex
        {
            get
            {
                return _activePairIndex;
            }
            set
            {
                if (_activePairIndex == value)
                    return;

                _activePairIndex = value;

                if (_activePairIndex == 0)
                {
                    CurrentPair = Pairs.btc_jpy;
                }
                else if (_activePairIndex == 1)
                {
                    CurrentPair = Pairs.xrp_jpy;
                }
                else if (_activePairIndex == 2)
                {
                    CurrentPair = Pairs.ltc_btc;
                }
                else if (_activePairIndex == 3)
                {
                    CurrentPair = Pairs.eth_btc;
                }
                else if (_activePairIndex == 4)
                {
                    CurrentPair = Pairs.mona_jpy;
                }
                else if (_activePairIndex == 5)
                {
                    CurrentPair = Pairs.bcc_jpy;
                }
                // btc_jpy, xrp_jpy, ltc_btc, eth_btc, mona_jpy, mona_btc, bcc_jpy, bcc_btc

                this.NotifyPropertyChanged("ActivePairIndex");
            }
        }

        #endregion

        #region == APIキーとシークレットのプロパティ） ==

        private bool _showApiKeyLock;
        public bool ShowApiKeyLock
        {
            get
            {
                return _showApiKeyLock;
            }
            set
            {
                if (_showApiKeyLock == value) return;

                _showApiKeyLock = value;
                this.NotifyPropertyChanged("ShowApiKeyLock");

                if (_showApiKeyLock)
                    ShowApiKey = false;
                else
                    ShowApiKey = true;
            }
        }

        private bool _showApiKey;
        public bool ShowApiKey
        {
            get
            {
                return _showApiKey;
            }
            set
            {
                if (_showApiKey == value) return;

                _showApiKey = value;
                this.NotifyPropertyChanged("ShowApiKey");

                if (_showApiKey)
                    ShowApiKeyLock = false;
                else
                    ShowApiKeyLock = true;
            }
        }

        // 注文一覧（参照）
        private string _getOrdersApiKey = "";
        public string OrdersApiKey
        {
            get
            {
                return _getOrdersApiKey;
            }
            set
            {
                if (_getOrdersApiKey == value) return;

                _getOrdersApiKey = value;
                this.NotifyPropertyChanged("OrdersApiKey");
            }
        }
        private string _getOrdersSecret = "";
        public string OrdersSecret
        {
            get
            {
                return _getOrdersSecret;
            }
            set
            {
                if (_getOrdersSecret == value) return;

                _getOrdersSecret = value;
                this.NotifyPropertyChanged("OrdersSecret");
            }
        }
        private bool _ordersApiKeyIsSet;
        public bool OrdersApiKeyIsSet
        {
            get
            {
                return _ordersApiKeyIsSet;
            }
            set
            {
                if (_ordersApiKeyIsSet == value) return;

                _ordersApiKeyIsSet = value;
                this.NotifyPropertyChanged("OrdersApiKeyIsSet");

                this.NotifyPropertyChanged("ShowOrdersApiSet");
                this.NotifyPropertyChanged("ShowOrders");
            }
        }

        // 資産（参照）
        private string _getAssetsApiKey = "";
        public string AssetsApiKey
        {
            get
            {
                return _getAssetsApiKey;
            }
            set
            {
                if (_getAssetsApiKey == value) return;

                _getAssetsApiKey = value;
                this.NotifyPropertyChanged("AssetsApiKey");
            }
        }
        private string _getAssetsSecret = "";
        public string AssetsApiSecret
        {
            get
            {
                return _getAssetsSecret;
            }
            set
            {
                if (_getAssetsSecret == value) return;

                _getAssetsSecret = value;
                this.NotifyPropertyChanged("AssetsApiSecret");
            }
        }
        private bool _assetsApiKeyIsSet;
        public bool AssetsApiKeyIsSet
        {
            get
            {
                return _assetsApiKeyIsSet;
            }
            set
            {
                if (_assetsApiKeyIsSet == value) return;

                _assetsApiKeyIsSet = value;
                this.NotifyPropertyChanged("AssetsApiKeyIsSet");

                this.NotifyPropertyChanged("ShowAsset");
                this.NotifyPropertyChanged("ShowAssetApiSet");
            }
        }

        // 取引履歴（参照）
        private string _getTradeHistoryApiKey = "";
        public string TradeHistoryApiKey
        {
            get
            {
                return _getTradeHistoryApiKey;
            }
            set
            {
                if (_getTradeHistoryApiKey == value) return;

                _getTradeHistoryApiKey = value;
                this.NotifyPropertyChanged("TradeHistoryApiKey");
            }
        }
        private string _getTradeHistorySecret = "";
        public string TradeHistorySecret
        {
            get
            {
                return _getTradeHistorySecret;
            }
            set
            {
                if (_getTradeHistorySecret == value) return;

                _getTradeHistorySecret = value;
                this.NotifyPropertyChanged("TradeHistorySecret");
            }
        }
        private bool _tradeHistoryApiKeyIsSet;
        public bool TradeHistoryApiKeyIsSet
        {
            get
            {
                return _tradeHistoryApiKeyIsSet;
            }
            set
            {
                if (_tradeHistoryApiKeyIsSet == value) return;

                _tradeHistoryApiKeyIsSet = value;
                this.NotifyPropertyChanged("TradeHistoryApiKeyIsSet");

                this.NotifyPropertyChanged("ShowTradeHistory");
                this.NotifyPropertyChanged("ShowTradeHistoryApiSet");
            }
        }

        // 自動取引  (取引権限)
        private string _autoTradeApiKey = "";
        public string AutoTradeApiKey
        {
            get
            {
                return _autoTradeApiKey;
            }
            set
            {
                if (_autoTradeApiKey == value) return;

                _autoTradeApiKey = value;
                this.NotifyPropertyChanged("AutoTradeApiKey");
            }
        }
        private string _autoTradeSecret = "";
        public string AutoTradeSecret
        {
            get
            {
                return _autoTradeSecret;
            }
            set
            {
                if (_autoTradeSecret == value) return;

                _autoTradeSecret = value;
                this.NotifyPropertyChanged("AutoTradeSecret");
            }
        }
        private bool _autoTradeApiKeyIsSet;
        public bool AutoTradeApiKeyIsSet
        {
            get
            {
                return _autoTradeApiKeyIsSet;
            }
            set
            {
                if (_autoTradeApiKeyIsSet == value) return;

                _autoTradeApiKeyIsSet = value;
                this.NotifyPropertyChanged("AutoTradeApiKeyIsSet");

                this.NotifyPropertyChanged("ShowAutoTrade");
                this.NotifyPropertyChanged("ShowAutoTradeApiSet");
            }
        }

        // 手動取引  (取引権限)
        private string _manualTradeApiKey = "";
        public string ManualTradeApiKey
        {
            get
            {
                return _manualTradeApiKey;
            }
            set
            {
                if (_manualTradeApiKey == value) return;

                _manualTradeApiKey = value;
                this.NotifyPropertyChanged("ManualTradeApiKey");
            }

        }
        private string _manualTradeSecret = "";
        public string ManualTradeSecret
        {
            get
            {
                return _manualTradeSecret;
            }
            set
            {
                if (_manualTradeSecret == value) return;

                _manualTradeSecret = value;
                this.NotifyPropertyChanged("ManualTradeSecret");
            }

        }
        private bool _manualTradeApiKeyIsSet = false;
        public bool ManualTradeApiKeyIsSet
        {
            get
            {
                return _manualTradeApiKeyIsSet;
            }
            set
            {
                if (_manualTradeApiKeyIsSet == value) return;

                _manualTradeApiKeyIsSet = value;
                this.NotifyPropertyChanged("ManualTradeApiKeyIsSet");

                this.NotifyPropertyChanged("ShowManualTrade");
                this.NotifyPropertyChanged("ShowManualTradeApiSet");
            }
        }

        // 特殊注文  (取引権限)
        private string _ifdocoTradeApiKey = "";
        public string IfdocoTradeApiKey
        {
            get
            {
                return _ifdocoTradeApiKey;
            }
            set
            {
                if (_ifdocoTradeApiKey == value) return;

                _ifdocoTradeApiKey = value;
                this.NotifyPropertyChanged("IfdocoTradeApiKey");
            }
        }
        private string _ifdocoTradeSecret = "";
        public string IfdocoTradeSecret
        {
            get
            {
                return _ifdocoTradeSecret;
            }
            set
            {
                if (_ifdocoTradeSecret == value) return;

                _ifdocoTradeSecret = value;
                this.NotifyPropertyChanged("IfdocoTradeSecret");
            }
        }
        private bool _ifdocoTradeApiKeyIsSet;
        public bool IfdocoTradeApiKeyIsSet
        {
            get
            {
                return _ifdocoTradeApiKeyIsSet;
            }
            set
            {
                if (_ifdocoTradeApiKeyIsSet == value) return;

                _ifdocoTradeApiKeyIsSet = value;
                this.NotifyPropertyChanged("IfdocoTradeApiKeyIsSet");

                this.NotifyPropertyChanged("ShowIfdocoTrade");
                this.NotifyPropertyChanged("ShowIfdocoTradeApiSet");
            }
        }


        #endregion

        #region == クライアントのプロパティ（API ＆ RSS） ==

        // 公開API Ticker クライアント
        PublicAPIClient _pubTickerApi = new PublicAPIClient();

        // 公開API Depth クライアント
        PublicAPIClient _pubDepthApi = new PublicAPIClient();

        // 公開API Transactions クライアント
        PublicAPIClient _pubTransactionsApi = new PublicAPIClient();

        // 公開API ロウソク足 クライアント
        PublicAPIClient _pubCandlestickApi = new PublicAPIClient();

        // プライベートAPIクライアント 後でコンストラクタでイニシャライズ
        PrivateAPIClient _priApi;

        // RSS フィード取得　クライアント
        RSSClient _rssCli = new RSSClient();
         
        private Rss _selectedFeedItemJa;
        public Rss SelectedFeedItemJa
        {
            get
            {
                return _selectedFeedItemJa;
            }
            set
            {
                if (_selectedFeedItemJa == value) return;

                _selectedFeedItemJa = value;

                this.NotifyPropertyChanged("SelectedFeedItemJa");
            }
        }

        private Rss _selectedFeedItemEn;
        public Rss SelectedFeedItemEn
        {
            get
            {
                return _selectedFeedItemEn;
            }
            set
            {
                if (_selectedFeedItemEn == value) return;

                _selectedFeedItemEn = value;

                this.NotifyPropertyChanged("SelectedFeedItemEn");
            }
        }

        #endregion

        #region == ObservableCollectionのプロパティ ==

        // アクティブな注文一覧
        private ObservableCollection<Order> _orders = new ObservableCollection<Order>();
        public ObservableCollection<Order> ActiveOrders
        {
            get { return this._orders; }
        }

        // TickHistoryクラス list (旧歩み値 )
        private ObservableCollection<TickHistory> _tickHistory = new ObservableCollection<TickHistory>();
        public ObservableCollection<TickHistory> TickHistories
        {
            get { return this._tickHistory; }
        }

        // 板情報
        private ObservableCollection<Depth> _depth = new ObservableCollection<Depth>();
        public ObservableCollection<Depth> Depths
        {
            get { return this._depth; }
        }

        // トランザクション（歩み値）
        private ObservableCollection<Transactions> _transactions = new ObservableCollection<Transactions>();
        public ObservableCollection<Transactions> Transactions
        {
            get { return this._transactions; }
        }

        // 取引履歴 trade list
        private ObservableCollection<Trade> _trades = new ObservableCollection<Trade>();
        public ObservableCollection<Trade> Trades
        {
            get { return this._trades; }
        }

        // 特殊注文IFDOCO注文一覧 Ifdoco order list
        private ObservableCollection<Ifdoco> _ifdocos = new ObservableCollection<Ifdoco>();
        public ObservableCollection<Ifdoco> Ifdocos
        {
            get { return this._ifdocos; }
        }

        // Error list
        private ObservableCollection<ClientError> _errors = new ObservableCollection<ClientError>();
        public ObservableCollection<ClientError> Errors
        {
            get { return this._errors; }
        }

        // RSS _bitcoinNewsJa
        private ObservableCollection<Rss> _bitcoinNewsJa = new ObservableCollection<Rss>();
        public ObservableCollection<Rss> BitcoinNewsJa
        {
            get { return this._bitcoinNewsJa; }
        }

        // RSS _bitcoinNewsEn
        private ObservableCollection<Rss> _bitcoinNewsEn = new ObservableCollection<Rss>();
        public ObservableCollection<Rss> BitcoinNewsEn
        {
            get { return this._bitcoinNewsEn; }
        }

        /*
        // 資産
        private ObservableCollection<Asset> _assets = new ObservableCollection<Asset>();
        public ObservableCollection<Asset> AssetList
        {
            get { return this._assets; }
        }
        */

        // 自動取引2
        private ObservableCollection<AutoTrade2> _autoTrades2 = new ObservableCollection<AutoTrade2>();
        public ObservableCollection<AutoTrade2> AutoTrades2
        {
            get { return this._autoTrades2; }
        }

        #endregion

        #region == リスト・コレクションのカウント数関係のプロパティ ==

        // アクティブな注文数
        private int _activeOrdersCount = 0;
        public int ActiveOrdersCount
        {
            get
            {
                return _activeOrdersCount;
            }
            set
            {
                if (_activeOrdersCount == value)
                    return;

                _activeOrdersCount = value;
                NotifyPropertyChanged("ActiveOrdersCount");
                NotifyPropertyChanged("ActiveOrdersCountTitle");

            }
        }
        // アクティブな注文数　タブ表示
        public string ActiveOrdersCountTitle
        {
            get
            {
                if (ActiveOrdersCount > 0)
                {
                    return "注文一覧(_V) (" + ActiveOrdersCount.ToString() + ")";
                }
                else if (ActiveOrdersCount < 0)
                {
                    return "注文一覧(_V) (!!)";
                }
                else
                {
                    return "注文一覧(_V)";
                }
            }
        }

        // アクティブな特殊注文数
        private int _activeIfdocosCount = 0;
        // アクティブな特殊注文数　タブ表示
        public int ActiveIfdocosCount
        {
            get
            {
                return _activeIfdocosCount;
            }
            set
            {
                if (_activeIfdocosCount == value)
                    return;

                _activeIfdocosCount = value;
                NotifyPropertyChanged("ActiveIfdocosCount");
                NotifyPropertyChanged("ActiveIfdocosCountTitle");

            }
        }
        public string ActiveIfdocosCountTitle
        {
            get
            {
                if (ActiveIfdocosCount > 0)
                {
                    return "特殊注文(_F) (" + ActiveIfdocosCount.ToString() + ")";
                }
                else if (ActiveIfdocosCount < 0)
                {
                    return "特殊注文(_F) (!!)";
                }
                else
                {
                    return "特殊注文(_F)";
                }
            }
        }

        // 取引履歴数
        private int _tradeHistories = 0;
        // 取引履歴　タブ表示
        public string TradeHistoryTitle
        {
            get
            {
                if (_tradeHistories < 0)
                {
                    return "取引履歴(_Y) (!!)";
                }
                else
                {
                    return "取引履歴(_Y)";
                }
            }
        }

        // エラー数
        private int _errorCount = 0;
        public int ErrorsCount
        {
            get
            {
                return _errorCount;
            }
            set
            {
                if (_errorCount == value)
                    return;

                _errorCount = value;
                NotifyPropertyChanged("ErrorsCount");
                NotifyPropertyChanged("ErrorsCountTitle");

            }
        }
        // エラー数　タブ表示
        public string ErrorsCountTitle
        {
            get
            {
                if (_errorCount > 0)
                {
                    return "エラー(_R) (" + _errorCount.ToString() + ")";
                }
                else if (_errorCount < 0)
                {
                    return "エラー(_R) (!!)";
                }
                else
                {
                    return "エラー(_R)";
                }
            }
        }

        // 自動取引オンオフ表示
        public string AutoTradeTitle
        {
            get
            {
                if (AutoTradeStart)
                {
                    return "自動取引(_Z) (On)";
                }
                else
                {
                    return "自動取引(_Z)";
                }
            }
        }

        #endregion

        #region == Ticker系のプロパティ ==

        // Ticker
        // 最終取引価格
        private decimal _ltp;
        public decimal Ltp
        {
            get
            {
                return _ltp;
            }
            set
            {
                if (_ltp == value)
                    return;

                _ltp = value;
                this.NotifyPropertyChanged("Ltp");
                this.NotifyPropertyChanged("LTPString");

            }
        }
        /*
        public string LTPString
        {
            get { return String.Format("{0:#,0}", _ltp); }
        }
        */

        private Color _ltpPriceColor;
        public Color LTPPriceColor
        {
            get
            {
                return _ltpPriceColor;
            }
            set
            {
                if (_ltpPriceColor == value)
                    return;

                _ltpPriceColor = value;
                this.NotifyPropertyChanged("LTPPriceColor");
            }
        }

        private decimal _bid;
        public decimal Bid
        {
            get
            {
                return _bid;
            }
            set
            {
                if (_bid == value)
                    return;

                _bid = value;
                this.NotifyPropertyChanged("Bid");
                this.NotifyPropertyChanged("BidString");

            }
        }
        public string BidString
        {
            get { return String.Format("{0:#,0}", _bid); }
        }

        private decimal _ask;
        public decimal Ask
        {
            get
            {
                return _ask;
            }
            set
            {
                if (_ask == value)
                    return;

                _ask = value;
                this.NotifyPropertyChanged("Ask");
                this.NotifyPropertyChanged("AskString");

            }
        }
        public string AskString
        {
            get { return String.Format("{0:#,0}", _ask); }
        }

        private DateTime _tickTimeStamp;
        public DateTime TickTimeStamp
        {
            get
            {
                return _tickTimeStamp;
            }
            set
            {
                if (_tickTimeStamp == value)
                    return;

                _tickTimeStamp = value;
                this.NotifyPropertyChanged("TickTimeStamp");
                this.NotifyPropertyChanged("TickTimeStampString");

            }
        }
        public string TickTimeStampString
        {
            get { return CurrentPairString + " - " + _tickTimeStamp.ToLocalTime().ToString("yyyy/MM/dd/HH:mm:ss"); }
        }

        // highest & lowest last 24 hours.
        //high 過去24時間の最高値取引価格
        public string High24String
        {
            get { return String.Format("{0:#,0}", _highestIn24Price); }
        }

        //low 過去24時間の最安値取引価格
        public string Low24String
        {
            get { return String.Format("{0:#,0}", _lowestIn24Price); }
        }

        // 24H 最高・最低 値
        private decimal _highestIn24Price;
        public decimal HighestIn24Price
        {
            get { return _highestIn24Price; }
            set
            {
                if (_highestIn24Price == (long)value)
                    return;

                _highestIn24Price = (long)value;
                this.NotifyPropertyChanged("HighestIn24Price");
                this.NotifyPropertyChanged("High24String");
                //this.NotifyPropertyChanged("ChartMaxValue");


                this.NotifyPropertyChanged("MiddleLast24Price");
                this.NotifyPropertyChanged("MiddleLast24PriceIcon");
                this.NotifyPropertyChanged("MiddleLast24PriceIconColor");

                if (MinMode) return;
                // チャートの最高値をセット
                //ChartAxisY[0].MaxValue = (double)_highestIn24Price + 3000;
                // チャートの２４最高値ポイントを更新
                //(ChartSeries[1].Values[0] as ObservableValue).Value = (double)_highestIn24Price;

            }
        }

        private decimal _lowestIn24Price;
        public decimal LowestIn24Price
        {
            get { return _lowestIn24Price; }
            set
            {
                if (_lowestIn24Price == (long)value)
                    return;

                _lowestIn24Price = (long)value;
                this.NotifyPropertyChanged("LowestIn24Price");
                this.NotifyPropertyChanged("Low24String");
                //this.NotifyPropertyChanged("ChartMinValue");

                this.NotifyPropertyChanged("MiddleLast24Price");
                this.NotifyPropertyChanged("MiddleLast24PriceIcon");
                this.NotifyPropertyChanged("MiddleLast24PriceIconColor");

                if (MinMode) return;
                // チャートの最低値をセット
                //ChartAxisY[0].MinValue = (double)_lowestIn24Price - 3000;
                // チャートの２４最低値ポイントを更新
                //(ChartSeries[2].Values[0] as ObservableValue).Value = (double)_lowestIn24Price;
            }
        }

        // 起動後 最高・最低 値
        // highest & lowest since launch.
        private decimal _highestPrice;
        public decimal HighestPrice
        {
            get { return _highestPrice; }
            set
            {
                if (_highestPrice == value)
                    return;

                _highestPrice = value;
                this.NotifyPropertyChanged("HighestPrice");
                this.NotifyPropertyChanged("HighestPriceString");

                this.NotifyPropertyChanged("MiddleInitPrice");
                this.NotifyPropertyChanged("MiddleInitPriceIcon");
                this.NotifyPropertyChanged("MiddleInitPriceIconColor");

                //if (MinMode) return;
                (ChartSeries[1].Values[0] as ObservableValue).Value = (double)_highestPrice;
            }
        }

        private decimal _lowestPrice;
        public decimal LowestPrice
        {
            get { return _lowestPrice; }
            set
            {
                if (_lowestPrice == value)
                    return;

                _lowestPrice = value;
                this.NotifyPropertyChanged("LowestPrice");
                this.NotifyPropertyChanged("LowestPriceString");

                this.NotifyPropertyChanged("MiddleInitPrice");
                this.NotifyPropertyChanged("MiddleInitPriceIcon");
                this.NotifyPropertyChanged("MiddleInitPriceIconColor");

                //if (MinMode) return;
                (ChartSeries[2].Values[0] as ObservableValue).Value = (double)_lowestPrice;
            }
        }

        #endregion

        #region == 資産用のプロパティ ==

        // Assets 資産

        // 円資産名
        private string _assetJPYName;
        public string AssetJPYName
        {
            get
            {
                return _assetJPYName;
            }
            set
            {
                if (_assetJPYName == value)
                    return;

                _assetJPYName = value;
                this.NotifyPropertyChanged("AssetJPYName");
            }
        }
        
        // 円総資産額
        private decimal _assetJPYAmount;
        public decimal AssetJPYAmount
        {
            get
            {
                //return Math.Round(_assetJPYAmount);
                if (DemoMode)
                {
                    return _assetJPYAmount*100;
                }
                else
                {
                    return _assetJPYAmount;
                }
            }
            set
            {
                if (_assetJPYAmount == value)
                    return;

                _assetJPYAmount = value;
                this.NotifyPropertyChanged("AssetJPYAmount");
                this.NotifyPropertyChanged("AssetAllEstimateAmountString");
            }
        }
        
        // 円利用可能資産額
        private decimal _assetJPYFreeAmount;
        public decimal AssetJPYFreeAmount
        {
            get
            {
                return _assetJPYFreeAmount;
            }
            set
            {
                if (_assetJPYFreeAmount == value)
                    return;

                _assetJPYFreeAmount = value;
                this.NotifyPropertyChanged("AssetJPYFreeAmount");
            }
        }

        // ビットコイン資産名
        private string _assetBTCName;
        public string AssetBTCName
        {
            get
            {
                return _assetBTCName;
            }
            set
            {
                if (_assetBTCName == value)
                    return;

                _assetBTCName = value;
                this.NotifyPropertyChanged("AssetBTCName");
            }
        }
        
        // ビットコイン総資産
        private decimal _assetBTCAmount;
        public decimal AssetBTCAmount
        {
            get
            {
                if (DemoMode)
                {
                    return _assetBTCAmount*10000;
                }
                else
                {
                    return _assetBTCAmount;
                }
            }
            set
            {
                if (_assetBTCAmount == value)
                    return;

                _assetBTCAmount = value;
                this.NotifyPropertyChanged("AssetBTCAmount");
            }
        }
        
        // ビットコイン利用可能資産額
        private decimal _assetBTCFreeAmount;
        public decimal AssetBTCFreeAmount
        {
            get
            {
                if (DemoMode)
                {
                    return _assetBTCFreeAmount*100;
                }
                else
                {
                    return _assetBTCFreeAmount;
                }
            }
            set
            {
                if (_assetBTCFreeAmount == value)
                    return;

                _assetBTCFreeAmount = value;
                this.NotifyPropertyChanged("AssetBTCFreeAmount");
            }
        }

        // ビットコイン時価評価額 (ティックから更新される)
        private decimal _assetBTCEstimateAmount;
        public decimal AssetBTCEstimateAmount
        {
            get
            {
                return _assetBTCEstimateAmount;
            }
            set
            {
                if (_assetBTCEstimateAmount == value)
                    return;

                _assetBTCEstimateAmount = value;
                this.NotifyPropertyChanged("AssetBTCEstimateAmount");
                this.NotifyPropertyChanged("AssetBTCEstimateAmountString");
                this.NotifyPropertyChanged("AssetAllEstimateAmountString");
                this.NotifyPropertyChanged("AssetAllShushi");

            }
        }
        
        // ビットコイン時価評価額文字列
        public string AssetBTCEstimateAmountString
        {
            get { return String.Format("{0:#,0}", _assetBTCEstimateAmount); }
        }

        // 円建て総資産時価評価額合計文字列
        public string AssetAllEstimateAmountString
        {
            get { return String.Format("{0:#,0}", _assetBTCEstimateAmount + _assetJPYAmount + AssetXRPEstimateAmount + AssetLtcEstimateAmount + AssetEthEstimateAmount + AssetMonaEstimateAmount + AssetBchEstimateAmount); }
        }

        // 収支概算
        public string AssetAllShushi
        {
            get
            {
                if ((_assetBTCEstimateAmount + _assetJPYAmount) > _initPrice)
                {
                    return "+" + String.Format("{0:#,0}", (_assetBTCEstimateAmount + _assetJPYAmount + AssetXRPEstimateAmount + AssetLtcEstimateAmount + AssetEthEstimateAmount + AssetMonaEstimateAmount + AssetBchEstimateAmount) - _initPrice);
                }
                else if ((_assetBTCEstimateAmount + _assetJPYAmount) < _initPrice)
                {
                    return "-" + String.Format("{0:#,0}", _initPrice - (_assetBTCEstimateAmount + _assetJPYAmount + AssetXRPEstimateAmount + AssetLtcEstimateAmount + AssetEthEstimateAmount + AssetMonaEstimateAmount + AssetBchEstimateAmount));
                }
                else
                {
                    return "+-0";
                }

            }
        }

        // xrp リップル
        private string _assetXRPName;
        public string AssetXRPName
        {
            get
            {
                return _assetXRPName;
            }
            set
            {
                if (_assetXRPName == value)
                    return;

                _assetXRPName = value;
                this.NotifyPropertyChanged("AssetXRPName");
            }
        }
        
        // リップル総資産額
        private decimal _assetXRPAmount;
        public decimal AssetXRPAmount
        {
            get
            {
                return _assetXRPAmount;
            }
            set
            {
                if (_assetXRPAmount == value)
                    return;

                _assetXRPAmount = value;
                this.NotifyPropertyChanged("AssetXRPAmount");
            }
        }
        
        // リップル利用可能額
        private decimal _assetXRPFreeAmount;
        public decimal AssetXRPFreeAmount
        {
            get
            {
                return _assetXRPFreeAmount;
            }
            set
            {
                if (_assetXRPFreeAmount == value)
                    return;

                _assetXRPFreeAmount = value;
                this.NotifyPropertyChanged("AssetXRPFreeAmount");
            }
        }
        
        // リップル時価評価額
        private decimal _assetXRPEstimateAmount;
        public decimal AssetXRPEstimateAmount
        {
            get
            {
                return _assetXRPEstimateAmount;
            }
            set
            {
                if (_assetXRPEstimateAmount == value)
                    return;

                _assetXRPEstimateAmount = value;
                this.NotifyPropertyChanged("AssetXRPEstimateAmount");

                this.NotifyPropertyChanged("AssetAllEstimateAmountString");
                this.NotifyPropertyChanged("AssetAllShushi");
            }
        }

        //ltc ライトコイン
        private string _assetLtcName;
        public string AssetLtcName
        {
            get
            {
                return _assetLtcName;
            }
            set
            {
                if (_assetLtcName == value)
                    return;

                _assetLtcName = value;
                this.NotifyPropertyChanged("AssetLtcName");
            }
        }
        
        // ライトコイン総資産額
        private decimal _assetLtcAmount;
        public decimal AssetLtcAmount
        {
            get
            {
                return _assetLtcAmount;
            }
            set
            {
                if (_assetLtcAmount == value)
                    return;

                _assetLtcAmount = value;
                this.NotifyPropertyChanged("AssetLtcAmount");
            }
        }
        
        // ライトコイン利用可能額
        private decimal _assetLtcFreeAmount;
        public decimal AssetLtcFreeAmount
        {
            get
            {
                return _assetLtcFreeAmount;
            }
            set
            {
                if (_assetLtcFreeAmount == value)
                    return;

                _assetLtcFreeAmount = value;
                this.NotifyPropertyChanged("AssetLtcFreeAmount");
            }
        }
        
        // ライトコイン時価評価額
        private decimal _assetLtcEstimateAmount;
        public decimal AssetLtcEstimateAmount
        {
            get
            {
                return _assetLtcEstimateAmount;
            }
            set
            {
                if (_assetLtcEstimateAmount == value)
                    return;

                _assetLtcEstimateAmount = value;
                this.NotifyPropertyChanged("AssetLtcEstimateAmount");

                this.NotifyPropertyChanged("AssetAllEstimateAmountString");
                this.NotifyPropertyChanged("AssetAllShushi");

            }
        }

        // eth イーサリアム
        private string _assetEthName;
        public string AssetEthName
        {
            get
            {
                return _assetEthName;
            }
            set
            {
                if (_assetEthName == value)
                    return;

                _assetEthName = value;
                this.NotifyPropertyChanged("AssetEthName");
            }
        }
        
        // イーサリアム総資産額
        private decimal _assetEthAmount;
        public decimal AssetEthAmount
        {
            get
            {
                return _assetEthAmount;
            }
            set
            {
                if (_assetEthAmount == value)
                    return;

                _assetEthAmount = value;
                this.NotifyPropertyChanged("AssetEthAmount");
            }
        }
        
        // イーサリアム利用可能額
        private decimal _assetEthFreeAmount;
        public decimal AssetEthFreeAmount
        {
            get
            {
                return _assetEthFreeAmount;
            }
            set
            {
                if (_assetEthFreeAmount == value)
                    return;

                _assetEthFreeAmount = value;
                this.NotifyPropertyChanged("AssetEthFreeAmount");
            }
        }
        
        // イーサリアム時価評価額
        private decimal _assetEthEstimateAmount;
        public decimal AssetEthEstimateAmount
        {
            get
            {
                return _assetEthEstimateAmount;
            }
            set
            {
                if (_assetEthEstimateAmount == value)
                    return;

                _assetEthEstimateAmount = value;
                this.NotifyPropertyChanged("AssetEthEstimateAmount");

                this.NotifyPropertyChanged("AssetAllEstimateAmountString");
                this.NotifyPropertyChanged("AssetAllShushi");

            }
        }

        // mona モナーコイン
        private string _assetMonaName;
        public string AssetMonaName
        {
            get
            {
                return _assetMonaName;
            }
            set
            {
                if (_assetMonaName == value)
                    return;

                _assetMonaName = value;
                this.NotifyPropertyChanged("AssetMonaName");
            }
        }
        
        // モナーコイン総資産額
        private decimal _assetMonaAmount;
        public decimal AssetMonaAmount
        {
            get
            {
                return _assetMonaAmount;
            }
            set
            {
                if (_assetMonaAmount == value)
                    return;

                _assetMonaAmount = value;
                this.NotifyPropertyChanged("AssetMonaAmount");
            }
        }
        
        // モナーコイン利用可能額
        private decimal _assetMonaFreeAmount;
        public decimal AssetMonaFreeAmount
        {
            get
            {
                return _assetMonaFreeAmount;
            }
            set
            {
                if (_assetMonaFreeAmount == value)
                    return;

                _assetMonaFreeAmount = value;
                this.NotifyPropertyChanged("AssetMonaFreeAmount");
            }
        }
        
        // モナーコイン時価評価額
        private decimal _assetMonaEstimateAmount;
        public decimal AssetMonaEstimateAmount
        {
            get
            {
                return _assetMonaEstimateAmount;
            }
            set
            {
                if (_assetMonaEstimateAmount == value)
                    return;

                _assetMonaEstimateAmount = value;
                this.NotifyPropertyChanged("AssetMonaEstimateAmount");

                this.NotifyPropertyChanged("AssetAllEstimateAmountString");
                this.NotifyPropertyChanged("AssetAllShushi");

            }
        }

        // bch ビットコインキャッシュ
        private string _assetBchName;
        public string AssetBchName
        {
            get
            {
                return _assetBchName;
            }
            set
            {
                if (_assetBchName == value)
                    return;

                _assetBchName = value;
                this.NotifyPropertyChanged("AssetBchName");
            }
        }
        
        // ビットコインキャッシュ総資産額
        private decimal _assetBchAmount;
        public decimal AssetBchAmount
        {
            get
            {
                return _assetBchAmount;
            }
            set
            {
                if (_assetBchAmount == value)
                    return;

                _assetBchAmount = value;
                this.NotifyPropertyChanged("AssetBchAmount");
            }
        }
        
        // ビットコインキャッシュ利用可能額
        private decimal _assetBchFreeAmount;
        public decimal AssetBchFreeAmount
        {
            get
            {
                return _assetBchFreeAmount;
            }
            set
            {
                if (_assetBchFreeAmount == value)
                    return;

                _assetBchFreeAmount = value;
                this.NotifyPropertyChanged("AssetBchFreeAmount");
            }
        }
        
        // ビットコインキャッシュ時価評価額
        private decimal _assetBchEstimateAmount;
        public decimal AssetBchEstimateAmount
        {
            get
            {
                return _assetBchEstimateAmount;
            }
            set
            {
                if (_assetBchEstimateAmount == value)
                    return;

                _assetBchEstimateAmount = value;
                this.NotifyPropertyChanged("AssetBchEstimateAmount");

                this.NotifyPropertyChanged("AssetAllEstimateAmountString");
                this.NotifyPropertyChanged("AssetAllShushi");

            }
        }

        #endregion

        #region == 手動取引用のプロパティ ==

        // Order 発注
        // 売り数量
        private decimal _sellAmount = 0.001M; // 通貨別デフォ指定 TODO
        public decimal SellAmount
        {
            get
            {
                return _sellAmount;
            }
            set
            {
                if (_sellAmount == value)
                    return;

                _sellAmount = value;
                this.NotifyPropertyChanged("SellAmount");
                this.NotifyPropertyChanged("SellEstimatePrice");
            }
        }

        // 売り価格
        private decimal _sellPrice; // 通貨別デフォ指定 TODO
        public decimal SellPrice
        {
            get
            {
                return _sellPrice;
            }
            set
            {
                if (_sellPrice == value)
                    return;

                _sellPrice = value;
                this.NotifyPropertyChanged("SellPrice");
                this.NotifyPropertyChanged("SellEstimatePrice");

            }
        }

        // 売りタイプ（指値・成行）
        private OrderTypes _sellType;
        public OrderTypes SellType
        {
            get
            {
                return _sellType;
            }
            set
            {
                if (_sellType == value)
                    return;

                _sellType = value;
                this.NotifyPropertyChanged("SellType");
                this.NotifyPropertyChanged("SellEstimatePrice");
            }
        }
        
        // 売り予想金額
        public decimal SellEstimatePrice
        {
            get
            {
                if (SellType == OrderTypes.market)
                {
                    return SellAmount * _bid;
                }
                else
                {
                    return SellAmount * SellPrice;
                }
            }
        }

        // 買い数量
        private decimal _buyAmount = 0.001M; // 通貨別デフォ指定 TODO
        public decimal BuyAmount
        {
            get
            {
                return _buyAmount;
            }
            set
            {
                if (_buyAmount == value)
                    return;

                _buyAmount = value;
                this.NotifyPropertyChanged("BuyAmount");
                this.NotifyPropertyChanged("BuyEstimatePrice");
            }
        }
       
        // 買い価格
        private decimal _buyPrice;
        public decimal BuyPrice
        {
            get
            {
                return _buyPrice;
            }
            set
            {
                if (_buyPrice == value)
                    return;

                _buyPrice = value;
                this.NotifyPropertyChanged("BuyPrice");
                this.NotifyPropertyChanged("BuyEstimatePrice");
            }
        }
        
        // 買いタイプ（指値・成行）
        private OrderTypes _buyType; 
        public OrderTypes BuyType
        {
            get
            {
                return _buyType;
            }
            set
            {
                if (_buyType == value)
                    return;

                _buyType = value;
                this.NotifyPropertyChanged("BuyType");
                this.NotifyPropertyChanged("BuyEstimatePrice");
            }
        }
        
        // 買い予想金額
        public decimal BuyEstimatePrice
        {
            get
            {
                if (BuyType == OrderTypes.market)
                {
                    return BuyAmount * _ask;
                }
                else
                {
                    return BuyAmount * BuyPrice;
                }
            }
        }

        // API注文コマンド成否結果文字列
        private string _aPIResultBuyCommandResult;
        public string APIResultBuyCommandResult
        {
            get
            {
                if (string.IsNullOrEmpty(APIResultBuyCommandOrderIDString))
                {
                    return _aPIResultBuyCommandResult;
                }
                else
                {
                    return _aPIResultBuyCommandResult + " - 注文ID: " + APIResultBuyCommandOrderIDString;
                }
            }
            set
            {
                // don't
                //if (_aPIResultBuyCommandResult == value)
                //    return;

                _aPIResultBuyCommandResult = value;
                this.NotifyPropertyChanged("APIResultBuyCommandResult");
            }
        }

        private string _aPIResultSellCommandResult;
        public string APIResultSellCommandResult
        {
            get
            {
                if (string.IsNullOrEmpty(APIResultSellCommandOrderIDString))
                {
                    return _aPIResultSellCommandResult;
                }
                else
                {
                    return _aPIResultSellCommandResult + " - 注文ID: " + APIResultSellCommandOrderIDString;
                }
            }
            set
            {
                // don't
                //if (_aPIResultSellCommandResult == value)
                //    return;

                _aPIResultSellCommandResult = value;
                this.NotifyPropertyChanged("APIResultSellCommandResult");
            }
        }

        // API注文コマンドエラー結果文字列
        private string _aPIResultBuyCommandErrorString;
        public string APIResultBuyCommandErrorString
        {
            get
            {
                return _aPIResultBuyCommandErrorString;
            }
            set
            {
                //if (_aPIResultBuyCommandErrorString == value)
                //    return;

                _aPIResultBuyCommandErrorString = value;
                this.NotifyPropertyChanged("APIResultBuyCommandErrorString");
            }
        }

        private string _aPIResultSellCommandErrorString;
        public string APIResultSellCommandErrorString
        {
            get
            {
                return _aPIResultSellCommandErrorString;
            }
            set
            {
                //if (_aPIResultSellCommandErrorString == value)
                //    return;

                _aPIResultSellCommandErrorString = value;
                this.NotifyPropertyChanged("APIResultSellCommandErrorString");
            }
        }

        // API注文コマンド成功時、Order ID 表示
        private string _aPIResultBuyCommandOrderIDString;
        public string APIResultBuyCommandOrderIDString
        {
            get
            {
                return _aPIResultBuyCommandOrderIDString;
            }
            set
            {
                //if (_aPIResultBuyCommandOrderIDString == value)
                //    return;

                _aPIResultBuyCommandOrderIDString = value;
                this.NotifyPropertyChanged("APIResultBuyCommandOrderIDString");
            }
        }

        private string _aPIResultSellCommandOrderIDString;
        public string APIResultSellCommandOrderIDString
        {
            get
            {
                return _aPIResultSellCommandOrderIDString;
            }
            set
            {
                //if (_aPIResultSellCommandOrderIDString == value)
                //    return;

                _aPIResultSellCommandOrderIDString = value;
                this.NotifyPropertyChanged("APIResultSellCommandOrderIDString");
            }
        }

        #endregion

        #region == 自動取引用のプロパティ ==

        // 自動取引開始停止フラグ
        private bool _autoTradeStart = false;
        public bool AutoTradeStart
        {
            get
            {
                return _autoTradeStart;
            }
            set
            {
                if (_autoTradeStart == value)
                    return;

                _autoTradeStart = value;

                this.NotifyPropertyChanged("AutoTradeStart");
                this.NotifyPropertyChanged("StartButtonEnable");
                this.NotifyPropertyChanged("StopButtonEnable");
                this.NotifyPropertyChanged("AutoTradeTitle");

                // 自動取引ループスタート
                //if (_autoTradeStart) UpdateAutoTrade2();

            }
        }

        // 開始ボタン有効
        public bool StartButtonEnable
        {
            get
            {
                if (_autoTradeStart)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        // 停止ボタン有効
        public bool StopButtonEnable
        {
            get
            {
                if (_autoTradeStart)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private decimal _autoTradeProfit;
        public decimal AutoTradeProfit
        {
            get
            {
                return _autoTradeProfit;
            }
            set
            {
                if (_autoTradeProfit == value)
                    return;

                _autoTradeProfit = value;
                this.NotifyPropertyChanged("AutoTradeProfit");
            }
        }

        // 取引単位
        private decimal _autoTradeTama = 0.001M;
        public decimal AutoTradeTama
        {
            get
            {
                return _autoTradeTama;
            }
            set
            {
                if (_autoTradeTama == value)
                    return;

                _autoTradeTama = value;
                this.NotifyPropertyChanged("AutoTradeTama");
            }
        }

        // アッパーリミット
        private decimal _autoTradeUpperLimit;
        public decimal AutoTradeUpperLimit
        {
            get
            {
                return _autoTradeUpperLimit;
            }
            set
            {
                if (_autoTradeUpperLimit == value)
                    return;

                _autoTradeUpperLimit = value;
                this.NotifyPropertyChanged("AutoTradeUpperLimit");
            }
        }

        // ローワーリミット
        private decimal _autoTradeLowerLimit;
        public decimal AutoTradeLowerLimit
        {
            get
            {
                return _autoTradeLowerLimit;
            }
            set
            {
                if (_autoTradeLowerLimit == value)
                    return;

                _autoTradeLowerLimit = value;
                this.NotifyPropertyChanged("AutoTradeLowerLimit");
            }
        }

        // 損切値幅
        private decimal _autoTradeLostCut = 20000M;
        public decimal AutoTradeLostCut
        {
            get
            {
                return _autoTradeLostCut;
            }
            set
            {
                if (_autoTradeLostCut == value)
                    return;

                _autoTradeLostCut = value;
                this.NotifyPropertyChanged("AutoTradeLostCut");
            }
        }

        // アクティブ注文数
        private int _autoTradeActiveOrders;
        public int AutoTradeActiveOrders
        {
            get
            {
                return _autoTradeActiveOrders;
            }
            set
            {
                if (_autoTradeActiveOrders == value)
                    return;

                _autoTradeActiveOrders = value;
                this.NotifyPropertyChanged("AutoTradeActiveOrders");
            }
        }

        // 売り未約定の注文数
        private int _autoTradeSellOrders;
        public int AutoTradeSellOrders
        {
            get
            {
                return _autoTradeSellOrders;
            }
            set
            {
                if (_autoTradeSellOrders == value)
                    return;

                _autoTradeSellOrders = value;
                this.NotifyPropertyChanged("AutoTradeSellOrders");
            }
        }

        // 買い未約定の注文数
        private int _autoTradeBuyOrders;
        public int AutoTradeBuyOrders
        {
            get
            {
                return _autoTradeBuyOrders;
            }
            set
            {
                if (_autoTradeBuyOrders == value)
                    return;

                _autoTradeBuyOrders = value;
                this.NotifyPropertyChanged("AutoTradeBuyOrders");
            }
        }

        // エラーの注文数
        private int _autoTradeErrOrders;
        public int AutoTradeErrOrders
        {
            get
            {
                return _autoTradeErrOrders;
            }
            set
            {
                if (_autoTradeErrOrders == value)
                    return;

                _autoTradeErrOrders = value;
                this.NotifyPropertyChanged("AutoTradeErrOrders");
            }
        }



        #endregion

        #region == アラーム用のプロパティ ==

        // グローバル設定
        private bool _playSound = true;
        public bool PlaySound
        {
            get
            {
                return _playSound;
            }
            set
            {
                if (_playSound == value)
                    return;

                _playSound = value;
                this.NotifyPropertyChanged("PlaySound");
            }
        }

        // アラーム 警告音再生
        private long _alarmPlus;
        public long AlarmPlus
        {
            get
            {
                return _alarmPlus;
            }
            set
            {
                if (_alarmPlus == value)
                    return;

                _alarmPlus = value;
                this.NotifyPropertyChanged("AlarmPlus");
                this.NotifyPropertyChanged(nameof(ChartBlueline));
            }
        }
        private long _alarmMinus;
        public long AlarmMinus
        {
            get
            {
                return _alarmMinus;
            }
            set
            {
                if (_alarmMinus == value)
                    return;

                _alarmMinus = value;
                this.NotifyPropertyChanged("AlarmMinus");
                this.NotifyPropertyChanged(nameof(ChartRedline));
            }
        }

        // 起動後　最安値　最高値　アラーム情報表示
        private string _highLowInfoText;
        public string HighLowInfoText
        {
            get
            {
                return _highLowInfoText;
            }
            set
            {
                if (_highLowInfoText == value)
                    return;

                _highLowInfoText = value;
                this.NotifyPropertyChanged("HighLowInfoText");
            }
        }
        private Color _highLowInfoTextColor;
        public Color HighLowInfoTextColor
        {
            get
            {
                return _highLowInfoTextColor;
            }
            set
            {
                if (_highLowInfoTextColor == value)
                    return;

                _highLowInfoTextColor = value;
                this.NotifyPropertyChanged("HighLowInfoTextColor");
            }
        }

        // 起動後　最安値　最高値
        public string LowestPriceString
        {
            get
            {
                return String.Format("{0:#,0}", _lowestPrice); ;
            }
        }
        public string HighestPriceString
        {
            get
            {
                return String.Format("{0:#,0}", _highestPrice); ;
            }
        }

        // アラーム音
        // 起動後
        private bool _playSoundLowest = false;
        public bool PlaySoundLowest
        {
            get
            {
                return _playSoundLowest;
            }
            set
            {
                if (_playSoundLowest == value)
                    return;

                _playSoundLowest = value;
                this.NotifyPropertyChanged("PlaySoundLowest");
            }
        }
        private bool _playSoundHighest = false;
        public bool PlaySoundHighest
        {
            get
            {
                return _playSoundHighest;
            }
            set
            {
                if (_playSoundHighest == value)
                    return;

                _playSoundHighest = value;
                this.NotifyPropertyChanged("PlaySoundHighest");
            }
        }
        // last 24h
        private bool _playSoundLowest24h = true;
        public bool PlaySoundLowest24h
        {
            get
            {
                return _playSoundLowest24h;
            }
            set
            {
                if (_playSoundLowest24h == value)
                    return;

                _playSoundLowest24h = value;
                this.NotifyPropertyChanged("PlaySoundLowest24h");
            }
        }
        private bool _playSoundHighest24h = true;
        public bool PlaySoundHighest24h
        {
            get
            {
                return _playSoundHighest24h;
            }
            set
            {
                if (_playSoundHighest24h == value)
                    return;

                _playSoundHighest24h = value;
                this.NotifyPropertyChanged("PlaySoundHighest24h");
            }
        }

        #endregion

        #region == 板情報のプロパティ ==

        private int _depthGrouping = 0;
        public int DepthGrouping
        {
            get
            {
                return _depthGrouping;
            }
            set
            {
                if (_depthGrouping == value)
                    return;

                _depthGrouping = value;

                this.NotifyPropertyChanged("DepthGrouping");
            }
        }
        private bool _depthGroupingChanged;
        public bool DepthGroupingChanged
        {
            get
            {
                return _depthGroupingChanged;
            }
            set
            {
                if (_depthGroupingChanged == value)
                    return;

                _depthGroupingChanged = value;

                this.NotifyPropertyChanged("DepthGroupingChanged");
            }
        }


        #endregion

        #region == エラー表示用のプロパティ ==

        // APIエラー結果文字列 
        private string _aPIResultTicker;
        public string APIResultTicker
        {
            get
            {
                return _aPIResultTicker;
            }
            set
            {
                if (_aPIResultTicker == value) return;

                _aPIResultTicker = value;
                this.NotifyPropertyChanged("APIResultTicker");
            }
        }

        private string _aPIResultTradeHistory;
        public string APIResultTradeHistory
        {
            get
            {
                return _aPIResultTradeHistory;
            }
            set
            {
                if (_aPIResultTradeHistory == value) return;

                _aPIResultTradeHistory = value;
                this.NotifyPropertyChanged("APIResultTradeHistory");
            }
        }

        private string _aPIResultActiveOrders;
        public string APIResultActiveOrders
        {
            get
            {
                return _aPIResultActiveOrders;
            }
            set
            {
                if (_aPIResultActiveOrders == value) return;

                _aPIResultActiveOrders = value;
                this.NotifyPropertyChanged("APIResultActiveOrders");
            }
        }

        private string _aPIResultAssets;
        public string APIResultAssets
        {
            get
            {
                return _aPIResultAssets;
            }
            set
            {
                if (_aPIResultAssets == value) return;

                _aPIResultAssets = value;
                this.NotifyPropertyChanged("APIResultAssets");
            }
        }


        #endregion

        #region == 統計情報のプロパティ ==

        // 起動時初期価格
        private decimal _basePrice = 0;
        public decimal BasePrice
        {
            get
            {
                return _basePrice;
            }
            set
            {
                if (_basePrice == value)
                    return;

                _basePrice = value;

                this.NotifyPropertyChanged("BasePrice");
                this.NotifyPropertyChanged("BasePriceIcon");
                this.NotifyPropertyChanged("BasePriceIconColor");

                //if (MinMode) return;
                // チャートの起動時値をセット
                //ChartSeries[1].Values[0] = (new LiveCharts.Defaults.ObservableValue((double)_basePrice));

            }
        }

        // 過去1分間の平均値
        private decimal _averagePrice;
        public decimal AveragePrice
        {
            get { return _averagePrice; }
            set
            {
                if (_averagePrice == value)
                    return;

                _averagePrice = value;
                this.NotifyPropertyChanged("AveragePrice");
                this.NotifyPropertyChanged("AveragePriceIcon");
                this.NotifyPropertyChanged("AveragePriceIconColor");
            }
        }
        public string AveragePriceIcon
        {
            get
            {
                if (_ltp > _averagePrice)
                {
                    return "▲";
                }
                else if (_ltp < _averagePrice)
                {
                    return "▼";
                }
                else
                {
                    return "＝";
                }
            }
        }
        public Color AveragePriceIconColor
        {
            get
            {
                if (_ltp > _averagePrice)
                {
                    return _priceUpColor;//Colors.Aqua;
                }
                else if (_ltp < _averagePrice)
                {
                    return _priceDownColor;//Colors.Pink;
                }
                else
                {
                    return Colors.Gainsboro;
                }
            }
        }

        // 過去２４時間の中央値
        public decimal MiddleLast24Price
        {
            get
            {
                return ((_lowestIn24Price + _highestIn24Price) / 2L);
            }
        }
        public string MiddleLast24PriceIcon
        {
            get
            {
                if (_ltp > MiddleLast24Price)
                {
                    return "▲";
                }
                else if (_ltp < MiddleLast24Price)
                {
                    return "▼";
                }
                else
                {
                    return "＝";
                }
            }
        }
        public Color MiddleLast24PriceIconColor
        {
            get
            {
                if (_ltp > MiddleLast24Price)
                {
                    return _priceUpColor;//Colors.Aqua;
                }
                else if (_ltp < MiddleLast24Price)
                {
                    return _priceDownColor;//Colors.Pink;
                }
                else
                {
                    return Colors.Gainsboro;
                }
            }
        }

        // 起動後の中央値
        public decimal MiddleInitPrice
        {
            get
            {
                return ((_lowestPrice + _highestPrice) / 2L);
            }
        }
        public string MiddleInitPriceIcon
        {
            get
            {
                if (_ltp > MiddleInitPrice)
                {
                    return "▲";
                }
                else if (_ltp < MiddleInitPrice)
                {
                    return "▼";
                }
                else
                {
                    return "＝";
                }
            }
        }
        public Color MiddleInitPriceIconColor
        {
            get
            {
                if (_ltp > MiddleInitPrice)
                {
                    return _priceUpColor;//Colors.Aqua;
                }
                else if (_ltp < MiddleInitPrice)
                {
                    return _priceDownColor;//Colors.Pink;
                }
                else
                {
                    return Colors.Gainsboro;
                }
            }
        }

        // 起動時の値
        public string BasePriceIcon
        {
            get
            {
                if (_ltp > BasePrice)
                {
                    return "▲";
                }
                else if (_ltp < BasePrice)
                {
                    return "▼";
                }
                else
                {
                    return "＝";
                }
            }
        }
        public Color BasePriceIconColor
        {
            get
            {
                if (_ltp > BasePrice)
                {
                    return _priceUpColor;//Colors.Aqua;
                }
                else if (_ltp < BasePrice)
                {
                    return _priceDownColor; //Colors.Pink;
                }
                else
                {
                    return Colors.Gainsboro;
                }
            }
        }

        #endregion

        #region == チャート用のプロパティ ==

        // ロウソク足タイプ
        public enum CandleTypes
        {
            OneMin, FiveMin, FifteenMin, ThirteenMin, OneHour, FourHour, EightHour, TwelveHour, OneDay, OneWeek
        }

        // ロウソク足タイプ　コンボボックス表示用
        public Dictionary<CandleTypes, string> CandleTypesDictionary { get; } = new Dictionary<CandleTypes, string>()
        {
            {CandleTypes.OneMin, "１分"},
            //{CandleTypes.FiveMin, "５分" },
            //{CandleTypes.FifteenMin, "１５分"},
            //{CandleTypes.ThirteenMin, "３０分" },
            {CandleTypes.OneHour, "１時間" },
            //{CandleTypes.FourHour, "４時間"},
            //{CandleTypes.EightHour, "８時間" },
            //{CandleTypes.TwelveHour, "１２時間"},
            {CandleTypes.OneDay, "１日" },
            //{CandleTypes.OneWeek, "１週間"},

        };

        // 選択されたロウソク足タイプ
        public CandleTypes _selectedCandleType = CandleTypes.OneHour;
        public CandleTypes SelectedCandleType
        {
            get
            {
                return _selectedCandleType;
            }
            set
            {
                if (_selectedCandleType == value)
                    return;

                _selectedCandleType = value;
                this.NotifyPropertyChanged("SelectedCandleType");
                //this.NotifyPropertyChanged("SelectedCandleType");

                // 
                if (_selectedCandleType == CandleTypes.OneMin)
                {
                    // 一分毎のロウソク足タイプなら

                    if ((SelectedChartSpan != ChartSpans.OneHour) && (SelectedChartSpan != ChartSpans.ThreeHour))
                    {
                        // デフォルト 一時間の期間で表示
                        SelectedChartSpan = ChartSpans.OneHour;
                        return;
                    }

                    // または、3時間
                    //SelectedChartSpan = ChartSpans.ThreeHour;

                    // 負荷掛かり過ぎなので無し　>または、１日の期間
                    //SelectedChartSpan = ChartSpans.OneDay;

                    // つまり、今日と昨日の1minデータを取得する必要あり。

                }
                else if (_selectedCandleType == CandleTypes.OneHour)
                {
                    // 一時間のロウソク足タイプなら

                    if ((SelectedChartSpan != ChartSpans.ThreeDay) && (SelectedChartSpan != ChartSpans.OneDay) && (SelectedChartSpan != ChartSpans.OneWeek))
                    {
                        // デフォルト 3日の期間で表示
                        SelectedChartSpan = ChartSpans.ThreeDay;
                        return;
                    }


                    // または、１日の期間
                    //SelectedChartSpan = ChartSpans.OneDay;
                    // または、１週間の期間
                    //SelectedChartSpan = ChartSpans.OneWeek;

                    // つまり、今日、昨日、一昨日、その前の1hourデータを取得する必要あり。

                }
                else if (_selectedCandleType == CandleTypes.OneDay)
                {
                    // 1日のロウソク足タイプなら

                    if ((SelectedChartSpan != ChartSpans.OneMonth) && (SelectedChartSpan != ChartSpans.TwoMonth) && (SelectedChartSpan != ChartSpans.OneYear) && (SelectedChartSpan != ChartSpans.FiveYear))
                    {
                        // デフォルト 1ヵ月の期間で表示
                        SelectedChartSpan = ChartSpans.OneMonth;
                        return;
                    }

                    //
                    //SelectedChartSpan = ChartSpans.TwoMonth;

                    //SelectedChartSpan = ChartSpans.OneYear;

                    //SelectedChartSpan = ChartSpans.FiveYear;

                    // つまり、今年、去年、２年前、３年前、４年前、５年前の1hourデータを取得する必要あり。

                }
                else
                {
                    return;

                    // デフォルト 1日の期間で表示
                    //SelectedChartSpan = ChartSpans.OneDay;
                    //Debug.WriteLine("デフォルト Oops");

                }


                // チャート表示
                LoadChart();

            }
        }

        private string _chartLoadingInfo;
        public string ChartLoadingInfo
        {
            get
            {
                return _chartLoadingInfo;
            }
            set
            {
                if (_chartLoadingInfo == value)
                    return;

                _chartLoadingInfo = value;

                this.NotifyPropertyChanged("ChartLoadingInfo");

            }
        }

        // 選択されたチャート表示期間 
        private ChartSpans _chartSpan = ChartSpans.ThreeDay;
        public ChartSpans SelectedChartSpan
        {
            get
            {
                return _chartSpan;
            }
            set
            {
                if (_chartSpan == value)
                    return;

                _chartSpan = value;
                this.NotifyPropertyChanged("SelectedChartSpan");

                // チャート表示アップデート >// TODO ダブルアップデートになってしまう。
                //LoadChart();

                // チャート表示アップデート
                ChangeChartSpan();

            }
        }

        private ZoomingOptions _zoomingMode = ZoomingOptions.X;
        public ZoomingOptions ZoomingMode
        {
            get { return _zoomingMode; }
            set
            {
                _zoomingMode = value;
                this.NotifyPropertyChanged("ZoomingMode");
            }
        }

        // チャートデータ保持
        // 一日単位 今年、去年、２年前、３年前、４年前、５年前の1hourデータを取得する必要あり。
        private List<Ohlcv> _ohlcvsOneDay = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneDay
        {
            get { return _ohlcvsOneDay; }
            set
            {
                _ohlcvsOneDay = value;
                this.NotifyPropertyChanged("Ohlcvs");
            }
        }
        
        // 一時間単位 今日、昨日、一昨日、その前の1hourデータを取得する必要あり。
        private List<Ohlcv> _ohlcvsOneHour = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneHour
        {
            get { return _ohlcvsOneHour; }
            set
            {
                _ohlcvsOneHour = value;
                this.NotifyPropertyChanged("OhlcvsOneHour");
            }
        }
        
        // 一分単位 今日と昨日の1minデータを取得する必要あり。
        private List<Ohlcv> _ohlcvsOneMin = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneMin
        {
            get { return _ohlcvsOneMin; }
            set
            {
                _ohlcvsOneMin = value;
                this.NotifyPropertyChanged("OhlcvsOneMin");
            }
        }

        // 使用していない(TODO 安値アラームのセクションライン)
        public double ChartRedline
        {
            get
            {
                return (double)AlarmMinus;
            }
        }

        // 使用していない(TODO 高値アラームのセクションライン)
        public double ChartBlueline
        {
            get
            {
                return (double)AlarmPlus;
            }
        }

        private SeriesCollection _chartSeries;
        public SeriesCollection ChartSeries
        {
            get
            {
                return _chartSeries;
            }
            set
            {
                if (_chartSeries == value)
                    return;

                _chartSeries = value;
                this.NotifyPropertyChanged("ChartSeries");
            }
        }

        private AxesCollection _chartAxisX = new AxesCollection();
        public AxesCollection ChartAxisX
        {
            get
            {
                return _chartAxisX;
            }
            set
            {
                if (_chartAxisX == value)
                    return;

                _chartAxisX = value;
                this.NotifyPropertyChanged("ChartAxisX");
            }
        }

        private AxesCollection _chartAxisY = new AxesCollection();
        public AxesCollection ChartAxisY
        {
            get
            {
                return _chartAxisY;
            }
            set
            {
                if (_chartAxisY == value)
                    return;

                _chartAxisY = value;
                this.NotifyPropertyChanged("ChartAxisY");
            }
        }

        #endregion

        #region == 省エネ用のプロパティ ==
        // 省エネモード
        private bool _minMode = false;
        public bool MinMode
        {
            get
            {
                return _minMode;
            }
            set
            {
                if (_minMode == value) return;

                _minMode = value;

                // MinMode からの復帰再スタート
                //StartLoop();

                this.NotifyPropertyChanged("MinMode");
                this.NotifyPropertyChanged("NotMinMode");
            }
        }
        // 省エネモードにおける、コントロールの表示・非表示フラグ
        public bool NotMinMode
        {
            get
            {
                return !_minMode;
            }
        }

        #endregion

        #region == 特殊注文(IFDOCO)用のプロパティ ==

        // IFDOCO IDカウンター
        private int _ifdocoIdCount = 0;
        public int IfdocoIdCount
        {
            get { return _ifdocoIdCount; }
            set
            {
                if (_ifdocoIdCount == value)
                    return;

                _ifdocoIdCount = value;
                this.NotifyPropertyChanged("IfdocoIdCount");
            }
        }

        #region == IFD注文 ==

        // 注文結果表示
        private string _iFDOrderCommandResult;
        public string IFDOrderCommandResult
        {
            get { return _iFDOrderCommandResult; }
            set
            {
                if (_iFDOrderCommandResult == value)
                    return;

                _iFDOrderCommandResult = value;
                this.NotifyPropertyChanged("IFDOrderCommandResult");
            }
        }

        // 入力数値エラー表示
        private string _iFDOrderCommandErrorString;
        public string IFDOrderCommandErrorString
        {
            get { return _iFDOrderCommandErrorString; }
            set
            {
                if (_iFDOrderCommandErrorString == value)
                    return;

                _iFDOrderCommandErrorString = value;
                this.NotifyPropertyChanged("IFDOrderCommandErrorString");
            }
        }

        // IFD注文 ifd

        // 数量
        private decimal _iFD_IfdAmount;
        public decimal IFD_IfdAmount
        {
            get
            {
                return _iFD_IfdAmount;
            }
            set
            {
                if (_iFD_IfdAmount == value)
                    return;

                _iFD_IfdAmount = value;
                this.NotifyPropertyChanged("IFD_IfdAmount");
                this.NotifyPropertyChanged("IFD_IfdEstimatePrice");
            }
        }
        
        // 価格
        private decimal _iFD_IfdPrice;
        public decimal IFD_IfdPrice
        {
            get
            {
                return _iFD_IfdPrice;
            }
            set
            {
                if (_iFD_IfdPrice == value)
                    return;

                _iFD_IfdPrice = value;
                this.NotifyPropertyChanged("IFD_IfdPrice");
                this.NotifyPropertyChanged("IFD_IfdEstimatePrice");
            }
        }
        
        // タイプ（指値・成行）
        private IfdocoTypes _iFD_IfdType;
        public IfdocoTypes IFD_IfdType
        {
            get
            {
                return _iFD_IfdType;
            }
            set
            {
                if (_iFD_IfdType == value)
                    return;

                _iFD_IfdType = value;
                this.NotifyPropertyChanged("IFD_IfdType");
                this.NotifyPropertyChanged("IFD_IfdEstimatePrice");
            }
        }
        
        // サイド（売り・買い）
        private IfdocoSide _iFD_IfdSide;
        public IfdocoSide IFD_IfdSide
        {
            get
            {
                return _iFD_IfdSide;
            }
            set
            {
                if (_iFD_IfdSide == value)
                    return;

                _iFD_IfdSide = value;
                this.NotifyPropertyChanged("IFD_IfdSide");
                this.NotifyPropertyChanged("IFD_IfdEstimatePrice");
            }
        }

        public decimal IFD_IfdEstimatePrice
        {
            get
            {
                if (_iFD_IfdType == IfdocoTypes.limit)
                {
                    return _iFD_IfdAmount * _iFD_IfdPrice;
                }
                else if (_iFD_IfdType == IfdocoTypes.market)
                {
                    if (_iFD_IfdSide == IfdocoSide.buy)
                    {
                        return _iFD_IfdAmount * _ask;
                    }
                    else if (_iFD_IfdSide == IfdocoSide.sell)
                    {
                        return _iFD_IfdAmount * _bid;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        // IFD注文 ifd の done
        
        // 数量
        private decimal _iFD_DoAmount;
        public decimal IFD_DoAmount
        {
            get
            {
                return _iFD_DoAmount;
            }
            set
            {
                if (_iFD_DoAmount == value)
                    return;

                _iFD_DoAmount = value;
                this.NotifyPropertyChanged("IFD_DoAmount");
                this.NotifyPropertyChanged("IFD_DoEstimatePrice");
            }
        }
        
        // 価格
        private decimal _iFD_DoPrice;
        public decimal IFD_DoPrice
        {
            get
            {
                return _iFD_DoPrice;
            }
            set
            {
                if (_iFD_DoPrice == value)
                    return;

                _iFD_DoPrice = value;
                this.NotifyPropertyChanged("IFD_DoPrice");
                this.NotifyPropertyChanged("IFD_DoEstimatePrice");
            }
        }
        
        // タイプ（指値・成行）
        private IfdocoTypes _iFD_DoType;
        public IfdocoTypes IFD_DoType
        {
            get
            {
                return _iFD_DoType;
            }
            set
            {
                if (_iFD_DoType == value)
                    return;

                _iFD_DoType = value;
                this.NotifyPropertyChanged("IFD_DoType");
                this.NotifyPropertyChanged("IFD_DoEstimatePrice");
            }
        }
        
        // サイド（売り・買い）
        private IfdocoSide _iFD_DoSide = IfdocoSide.sell;
        public IfdocoSide IFD_DoSide
        {
            get
            {
                return _iFD_DoSide;
            }
            set
            {
                if (_iFD_DoSide == value)
                    return;

                _iFD_DoSide = value;
                this.NotifyPropertyChanged("IFD_DoSide");
                this.NotifyPropertyChanged("IFD_DoEstimatePrice");
            }
        }

        public decimal IFD_DoEstimatePrice
        {
            get
            {
                if (_iFD_DoType == IfdocoTypes.limit)
                {
                    return _iFD_DoAmount * _iFD_DoPrice;
                }
                else if (_iFD_DoType == IfdocoTypes.market)
                {
                    if (_iFD_DoSide == IfdocoSide.buy)
                    {
                        return _iFD_DoAmount * _ask;
                    }
                    else if (_iFD_DoSide == IfdocoSide.sell)
                    {
                        return _iFD_DoAmount * _bid;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        // Doトリガー価格
        private decimal _iFD_DoTriggerPrice;
        public decimal IFD_DoTriggerPrice
        {
            get
            {
                return _iFD_DoTriggerPrice;
            }
            set
            {
                if (_iFD_DoTriggerPrice == value)
                    return;

                _iFD_DoTriggerPrice = value;
                this.NotifyPropertyChanged("IFD_DoTriggerPrice");
            }
        }

        // トリガー[以上(0)以下(1)] // デフォ(1)
        private int _iFD_DoTriggerUpDown = 0;
        public int IFD_DoTriggerUpDown
        {
            get
            {
                return _iFD_DoTriggerUpDown;
            }
            set
            {
                if (_iFD_DoTriggerUpDown == value)
                    return;

                _iFD_DoTriggerUpDown = value;
                this.NotifyPropertyChanged("IFD_DoTriggerUpDown");
            }
        }

        #endregion

        #region == OCO注文 ==

        // 注文結果表示
        private string _ocoOrderCommandResult;
        public string OcoOrderCommandResult
        {
            get { return _ocoOrderCommandResult; }
            set
            {
                if (_ocoOrderCommandResult == value)
                    return;

                _ocoOrderCommandResult = value;
                this.NotifyPropertyChanged("OcoOrderCommandResult");
            }
        }

        // 入力エラー表示
        private string _ocoOrderCommandErrorString;
        public string OcoOrderCommandErrorString
        {
            get { return _ocoOrderCommandErrorString; }
            set
            {
                if (_ocoOrderCommandErrorString == value)
                    return;

                _ocoOrderCommandErrorString = value;
                this.NotifyPropertyChanged("OcoOrderCommandErrorString");
            }
        }

        // OCO注文 One

        // 数量
        private decimal _oCO_OneAmount;
        public decimal OCO_OneAmount
        {
            get
            {
                return _oCO_OneAmount;
            }
            set
            {
                if (_oCO_OneAmount == value)
                    return;

                _oCO_OneAmount = value;
                this.NotifyPropertyChanged("OCO_OneAmount");
                this.NotifyPropertyChanged("OCO_OneEstimatePrice");
            }
        }
        
        // 価格
        private decimal _oCO_OnePrice;
        public decimal OCO_OnePrice
        {
            get
            {
                return _oCO_OnePrice;
            }
            set
            {
                if (_oCO_OnePrice == value)
                    return;

                _oCO_OnePrice = value;
                this.NotifyPropertyChanged("OCO_OnePrice");
                this.NotifyPropertyChanged("OCO_OneEstimatePrice");
            }
        }
        
        // タイプ（指値・成行）
        private IfdocoTypes _oCO_OneType;
        public IfdocoTypes OCO_OneType
        {
            get
            {
                return _oCO_OneType;
            }
            set
            {
                if (_oCO_OneType == value)
                    return;

                _oCO_OneType = value;
                this.NotifyPropertyChanged("OCO_OneType");
                this.NotifyPropertyChanged("OCO_OneEstimatePrice");
            }
        }

        // サイド（売り・買い） // デフォ売り
        private IfdocoSide _oCO_OneSide = IfdocoSide.sell;
        public IfdocoSide OCO_OneSide
        {
            get
            {
                return _oCO_OneSide;
            }
            set
            {
                if (_oCO_OneSide == value)
                    return;

                _oCO_OneSide = value;
                this.NotifyPropertyChanged("OCO_OneSide");
                this.NotifyPropertyChanged("OCO_OneEstimatePrice");
            }
        }

        // 予想金額
        public decimal OCO_OneEstimatePrice
        {
            get
            {
                if (_oCO_OneType == IfdocoTypes.limit)
                {
                    return _oCO_OneAmount * _oCO_OnePrice;
                }
                else if (_oCO_OneType == IfdocoTypes.market)
                {
                    if (_oCO_OneSide == IfdocoSide.buy)
                    {
                        return _oCO_OneAmount * _ask;
                    }
                    else if (_oCO_OneSide == IfdocoSide.sell)
                    {
                        return _oCO_OneAmount * _bid;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        // トリガーOne価格
        private decimal _oCO_OneTriggerPrice;
        public decimal OCO_OneTriggerPrice
        {
            get
            {
                return _oCO_OneTriggerPrice;
            }
            set
            {
                if (_oCO_OneTriggerPrice == value)
                    return;

                _oCO_OneTriggerPrice = value;
                this.NotifyPropertyChanged("OCO_OneTriggerPrice");
            }
        }

        // トリガー[以上(0)以下(1)] // デフォ(0)
        private int _oCO_OneTriggerUpDown = 0;
        public int OCO_OneTriggerUpDown
        {
            get
            {
                return _oCO_OneTriggerUpDown;
            }
            set
            {
                if (_oCO_OneTriggerUpDown == value)
                    return;

                _oCO_OneTriggerUpDown = value;
                this.NotifyPropertyChanged("OCO_OneTriggerUpDown");
            }
        }

        // OCO注文 Other

        // 数量
        private decimal _oCO_OtherAmount;
        public decimal OCO_OtherAmount
        {
            get
            {
                return _oCO_OtherAmount;
            }
            set
            {
                if (_oCO_OtherAmount == value)
                    return;

                _oCO_OtherAmount = value;
                this.NotifyPropertyChanged("OCO_OtherAmount");
                this.NotifyPropertyChanged("OCO_OtherEstimatePrice");
            }
        }
        
        // 価格
        private decimal _oCO_OtherPrice;
        public decimal OCO_OtherPrice
        {
            get
            {
                return _oCO_OtherPrice;
            }
            set
            {
                if (_oCO_OtherPrice == value)
                    return;

                _oCO_OtherPrice = value;
                this.NotifyPropertyChanged("OCO_OtherPrice");
                this.NotifyPropertyChanged("OCO_OtherEstimatePrice");
            }
        }
        
        // タイプ（指値・成行）
        private IfdocoTypes _oCO_OtherType;
        public IfdocoTypes OCO_OtherType
        {
            get
            {
                return _oCO_OtherType;
            }
            set
            {
                if (_oCO_OtherType == value)
                    return;

                _oCO_OtherType = value;
                this.NotifyPropertyChanged("OCO_OtherType");
                this.NotifyPropertyChanged("OCO_OtherEstimatePrice");
            }
        }

        // サイド（売り・買い） // デフォ売り
        private IfdocoSide _oCO_OtherSide = IfdocoSide.sell;
        public IfdocoSide OCO_OtherSide
        {
            get
            {
                return _oCO_OtherSide;
            }
            set
            {
                if (_oCO_OtherSide == value)
                    return;

                _oCO_OtherSide = value;
                this.NotifyPropertyChanged("OCO_OtherSide");
                this.NotifyPropertyChanged("OCO_OtherEstimatePrice");
            }
        }

        // 予想金額
        public decimal OCO_OtherEstimatePrice
        {
            get
            {
                if (_oCO_OtherType == IfdocoTypes.limit)
                {
                    return _oCO_OtherAmount * _oCO_OtherPrice;
                }
                else if (_oCO_OtherType == IfdocoTypes.market)
                {
                    if (_oCO_OtherSide == IfdocoSide.buy)
                    {
                        return _oCO_OtherAmount * _ask;
                    }
                    else if (_oCO_OtherSide == IfdocoSide.sell)
                    {
                        return _oCO_OtherAmount * _bid;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        // トリガーOther価格
        private decimal _oCO_OtherTriggerPrice;
        public decimal OCO_OtherTriggerPrice
        {
            get
            {
                return _oCO_OtherTriggerPrice;
            }
            set
            {
                if (_oCO_OtherTriggerPrice == value)
                    return;

                _oCO_OtherTriggerPrice = value;
                this.NotifyPropertyChanged("OCO_OtherTriggerPrice");
            }
        }

        // トリガー[以上(0)以下(1)] // デフォ(1)
        private int _oCO_OtherTriggerUpDown = 1;
        public int OCO_OtherTriggerUpDown
        {
            get
            {
                return _oCO_OtherTriggerUpDown;
            }
            set
            {
                if (_oCO_OtherTriggerUpDown == value)
                    return;

                _oCO_OtherTriggerUpDown = value;
                this.NotifyPropertyChanged("OCO_OtherTriggerUpDown");
            }
        }

        #endregion

        #region == IFDOCO注文 ==

        // IFDOCO注文 ifd

        private string _ifdocoOrderCommandResult;
        public string IfdocoOrderCommandResult
        {
            get { return _ifdocoOrderCommandResult; }
            set
            {
                if (_ifdocoOrderCommandResult == value)
                    return;

                _ifdocoOrderCommandResult = value;
                this.NotifyPropertyChanged("IfdocoOrderCommandResult");
            }
        }

        private string _ifdocoOrderCommandErrorString;
        public string IfdocoOrderCommandErrorString
        {
            get { return _ifdocoOrderCommandErrorString; }
            set
            {
                if (_ifdocoOrderCommandErrorString == value)
                    return;

                _ifdocoOrderCommandErrorString = value;
                this.NotifyPropertyChanged("IfdocoOrderCommandErrorString");
            }
        }

        // 数量
        private decimal _iFDOCO_IfdAmount;
        public decimal IFDOCO_IfdAmount
        {
            get
            {
                return _iFDOCO_IfdAmount;
            }
            set
            {
                if (_iFDOCO_IfdAmount == value)
                    return;

                _iFDOCO_IfdAmount = value;
                this.NotifyPropertyChanged("IFDOCO_IfdAmount");
                this.NotifyPropertyChanged("IFDOCO_IfdEstimatePrice");
            }
        }
        // 価格
        private decimal _iFDOCO_IfdPrice;
        public decimal IFDOCO_IfdPrice
        {
            get
            {
                return _iFDOCO_IfdPrice;
            }
            set
            {
                if (_iFDOCO_IfdPrice == value)
                    return;

                _iFDOCO_IfdPrice = value;
                this.NotifyPropertyChanged("IFDOCO_IfdPrice");
                this.NotifyPropertyChanged("IFDOCO_IfdEstimatePrice");
            }
        }
        // タイプ（指値・成行）
        private IfdocoTypes _iFDOCO_IfdType;
        public IfdocoTypes IFDOCO_IfdType
        {
            get
            {
                return _iFDOCO_IfdType;
            }
            set
            {
                if (_iFDOCO_IfdType == value)
                    return;

                _iFDOCO_IfdType = value;
                this.NotifyPropertyChanged("IFDOCO_IfdType");
                this.NotifyPropertyChanged("IFDOCO_IfdEstimatePrice");
            }
        }
        // サイド（売り・買い）
        private IfdocoSide _iFDOCO_IfdSide;
        public IfdocoSide IFDOCO_IfdSide
        {
            get
            {
                return _iFDOCO_IfdSide;
            }
            set
            {
                if (_iFDOCO_IfdSide == value)
                    return;

                _iFDOCO_IfdSide = value;
                this.NotifyPropertyChanged("IFDOCO_IfdSide");
                this.NotifyPropertyChanged("IFDOCO_IfdEstimatePrice");
            }
        }
        public decimal IFDOCO_IfdEstimatePrice
        {
            get
            {
                if (_iFDOCO_IfdType == IfdocoTypes.limit)
                {
                    return _iFDOCO_IfdAmount * _iFDOCO_IfdPrice;
                }
                else if (_iFDOCO_IfdType == IfdocoTypes.market)
                {
                    if (_iFDOCO_IfdSide == IfdocoSide.buy)
                    {
                        return _iFDOCO_IfdAmount * _ask;
                    }
                    else if (_iFDOCO_IfdSide == IfdocoSide.sell)
                    {
                        return _iFDOCO_IfdAmount * _bid;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        // IFDOCO注文 One
        // 数量
        private decimal _iFDOCO_OneAmount;
        public decimal IFDOCO_OneAmount
        {
            get
            {
                return _iFDOCO_OneAmount;
            }
            set
            {
                if (_iFDOCO_OneAmount == value)
                    return;

                _iFDOCO_OneAmount = value;
                this.NotifyPropertyChanged("IFDOCO_OneAmount");
                this.NotifyPropertyChanged("IFDOCO_OneEstimatePrice");
            }
        }
        // 価格
        private decimal _iFDOCO_OnePrice;
        public decimal IFDOCO_OnePrice
        {
            get
            {
                return _iFDOCO_OnePrice;
            }
            set
            {
                if (_iFDOCO_OnePrice == value)
                    return;

                _iFDOCO_OnePrice = value;
                this.NotifyPropertyChanged("IFDOCO_OnePrice");
                this.NotifyPropertyChanged("IFDOCO_OneEstimatePrice");
            }
        }
        // タイプ（指値・成行）
        private IfdocoTypes _iFDOCO_OneType;
        public IfdocoTypes IFDOCO_OneType
        {
            get
            {
                return _iFDOCO_OneType;
            }
            set
            {
                if (_iFDOCO_OneType == value)
                    return;

                _iFDOCO_OneType = value;
                this.NotifyPropertyChanged("IFDOCO_OneType");
                this.NotifyPropertyChanged("IFDOCO_OneEstimatePrice");
            }
        }
        // サイド（売り・買い）
        private IfdocoSide _iFDOCO_OneSide = IfdocoSide.sell;
        public IfdocoSide IFDOCO_OneSide
        {
            get
            {
                return _iFDOCO_OneSide;
            }
            set
            {
                if (_iFDOCO_OneSide == value)
                    return;

                _iFDOCO_OneSide = value;
                this.NotifyPropertyChanged("IFDOCO_OneSide");
                this.NotifyPropertyChanged("IFDOCO_OneEstimatePrice");
            }
        }
        public decimal IFDOCO_OneEstimatePrice
        {
            get
            {
                if (_iFDOCO_OneType == IfdocoTypes.limit)
                {
                    return _iFDOCO_OneAmount * _iFDOCO_OnePrice;
                }
                else if (_iFDOCO_OneType == IfdocoTypes.market)
                {
                    if (_iFDOCO_OneSide == IfdocoSide.buy)
                    {
                        return _iFDOCO_OneAmount * _ask;
                    }
                    else if (_iFDOCO_OneSide == IfdocoSide.sell)
                    {
                        return _iFDOCO_OneAmount * _bid;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        // トリガーOne価格
        private decimal _iFDOCO_OneTriggerPrice;
        public decimal IFDOCO_OneTriggerPrice
        {
            get
            {
                return _iFDOCO_OneTriggerPrice;
            }
            set
            {
                if (_iFDOCO_OneTriggerPrice == value)
                    return;

                _iFDOCO_OneTriggerPrice = value;
                this.NotifyPropertyChanged("IFDOCO_OneTriggerPrice");
            }
        }

        // トリガー[以上(0)以下(1)] // デフォ(0)
        private int _iFDOCO_OneTriggerUpDown = 0;
        public int IFDOCO_OneTriggerUpDown
        {
            get
            {
                return _iFDOCO_OneTriggerUpDown;
            }
            set
            {
                if (_iFDOCO_OneTriggerUpDown == value)
                    return;

                _iFDOCO_OneTriggerUpDown = value;
                this.NotifyPropertyChanged("IFDOCO_OneTriggerUpDown");
            }
        }


        // IFDOCO注文 Other
        // 数量
        private decimal _iFDOCO_OtherAmount;
        public decimal IFDOCO_OtherAmount
        {
            get
            {
                return _iFDOCO_OtherAmount;
            }
            set
            {
                if (_iFDOCO_OtherAmount == value)
                    return;

                _iFDOCO_OtherAmount = value;
                this.NotifyPropertyChanged("IFDOCO_OtherAmount");
                this.NotifyPropertyChanged("IFDOCO_OtherEstimatePrice");
            }
        }
        // 価格
        private decimal _iFDOCO_OtherPrice;
        public decimal IFDOCO_OtherPrice
        {
            get
            {
                return _iFDOCO_OtherPrice;
            }
            set
            {
                if (_iFDOCO_OtherPrice == value)
                    return;

                _iFDOCO_OtherPrice = value;
                this.NotifyPropertyChanged("IFDOCO_OtherPrice");
                this.NotifyPropertyChanged("IFDOCO_OtherEstimatePrice");
            }
        }
        // タイプ（指値・成行）
        private IfdocoTypes _iFDOCO_OtherType;
        public IfdocoTypes IFDOCO_OtherType
        {
            get
            {
                return _iFDOCO_OtherType;
            }
            set
            {
                if (_iFDOCO_OtherType == value)
                    return;

                _iFDOCO_OtherType = value;
                this.NotifyPropertyChanged("IFDOCO_OtherType");
                this.NotifyPropertyChanged("IFDOCO_OtherEstimatePrice");
            }
        }
        // サイド（売り・買い）
        private IfdocoSide _iFDOCO_OtherSide = IfdocoSide.sell;
        public IfdocoSide IFDOCO_OtherSide
        {
            get
            {
                return _iFDOCO_OtherSide;
            }
            set
            {
                if (_iFDOCO_OtherSide == value)
                    return;

                _iFDOCO_OtherSide = value;
                this.NotifyPropertyChanged("IFDOCO_OtherSide");
                this.NotifyPropertyChanged("IFDOCO_OtherEstimatePrice");
            }
        }
        public decimal IFDOCO_OtherEstimatePrice
        {
            get
            {
                if (_iFDOCO_OtherType == IfdocoTypes.limit)
                {
                    return _iFDOCO_OtherAmount * _iFDOCO_OtherPrice;
                }
                else if (_iFDOCO_OtherType == IfdocoTypes.market)
                {
                    if (_iFDOCO_OtherSide == IfdocoSide.buy)
                    {
                        return _iFDOCO_OtherAmount * _ask;
                    }
                    else if (_iFDOCO_OtherSide == IfdocoSide.sell)
                    {
                        return _iFDOCO_OtherAmount * _bid;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        // トリガーOther価格
        private decimal _iFDOCO_OtherTriggerPrice;
        public decimal IFDOCO_OtherTriggerPrice
        {
            get
            {
                return _iFDOCO_OtherTriggerPrice;
            }
            set
            {
                if (_iFDOCO_OtherTriggerPrice == value)
                    return;

                _iFDOCO_OtherTriggerPrice = value;
                this.NotifyPropertyChanged("IFDOCO_OtherTriggerPrice");
            }
        }

        // トリガー[以上(0)以下(1)] // デフォ(1)
        private int _iFDOCO_OtherTriggerUpDown = 1;
        public int IFDOCO_OtherTriggerUpDown
        {
            get
            {
                return _iFDOCO_OtherTriggerUpDown;
            }
            set
            {
                if (_iFDOCO_OtherTriggerUpDown == value)
                    return;

                _iFDOCO_OtherTriggerUpDown = value;
                this.NotifyPropertyChanged("IFDOCO_OtherTriggerUpDown");
            }
        }


        #endregion

        #endregion

        #endregion

        #region == コンストラクタ ==

        public MainViewModel()
        {
            #region == コマンド初期化 ==

            BuyOrderCommand = new RelayCommand(BuyOrderCommand_Execute, BuyOrderCommand_CanExecute);
            SellOrderCommand = new RelayCommand(SellOrderCommand_Execute, SellOrderCommand_CanExecute);
            StartAutoTradeCommand = new RelayCommand(StartAutoTradeCommand_Execute, StartAutoTradeCommand_CanExecute);
            StopAutoTradeCommand = new RelayCommand(StopAutoTradeCommand_Execute, StopAutoTradeCommand_CanExecute);

            GetTradeHistoryListCommand = new RelayCommand(GetTradeHistoryListCommand_Execute, GetTradeHistoryListCommand_CanExecute);
            GetAssetsCommand = new RelayCommand(GetAssetsCommand_Execute, GetAssetsCommand_CanExecute);
            GetOrderListCommand = new RelayCommand(GetOrderListCommand_Execute, GetOrderListCommand_CanExecute);

            CancelOrderListviewCommand = new GenericRelayCommand<object>(
                param => CancelOrderListviewCommand_Execute(param),
                param => CancelOrderListviewCommand_CanExecute());
            RemoveDoneOrderListviewCommand = new RelayCommand(RemoveDoneOrderListviewCommand_Execute, RemoveDoneOrderListviewCommand_CanExecute);

            ViewMinimumCommand = new RelayCommand(ViewMinimumCommand_Execute, ViewMinimumCommand_CanExecute);
            ViewRestoreCommand = new RelayCommand(ViewRestoreCommand_Execute, ViewRestoreCommand_CanExecute);

            IfdOrderCommand = new RelayCommand(IfdOrderCommand_Execute, IfdOrderCommand_CanExecute);
            OcoOrderCommand = new RelayCommand(OcoOrderCommand_Execute, OcoOrderCommand_CanExecute);
            IfdocoOrderCommand = new RelayCommand(IfdocoOrderCommand_Execute, IfdocoOrderCommand_CanExecute);

            CancelIfdocoListviewCommand = new GenericRelayCommand<object>(
                param => CancelIfdocoListviewCommand_Execute(param),
                param => CancelIfdocoListviewCommand_CanExecute());
            RemoveDoneIfdocoListviewCommand = new RelayCommand(RemoveDoneIfdocoListviewCommand_Execute, RemoveDoneIfdocoListviewCommand_CanExecute);

            AutoTradeCancelListviewCommand = new GenericRelayCommand<object>(
                param => AutoTradeCancelListviewCommand_Execute(param),
                param => AutoTradeCancelListviewCommand_CanExecute());

            AutoTradeDeleteErrorItemListviewCommand = new GenericRelayCommand<object>(
                param => AutoTradeDeleteErrorItemListviewCommand_Execute(param),
                param => AutoTradeDeleteErrorItemListviewCommand_CanExecute());

            AutoTradeResetErrorListviewCommand = new GenericRelayCommand<object>(
                param => AutoTradeResetErrorListviewCommand_Execute(param),
                param => AutoTradeResetErrorListviewCommand_CanExecute());


            FeedOpenJaCommand = new RelayCommand(FeedOpenJaCommand_Execute, FeedOpenJaCommand_CanExecute);
            FeedOpenEnCommand = new RelayCommand(FeedOpenEnCommand_Execute, FeedOpenEnCommand_CanExecute);

            DepthGroupingCommand = new GenericRelayCommand<object>(
                param => DepthGroupingCommand_Execute(param),
                param => DepthGroupingCommand_CanExecute());

            LogInCommand = new GenericRelayCommand<object>(
                param => LogInCommand_Execute(param),
                param => LogInCommand_CanExecute());

            LogOutCommand = new RelayCommand(LogOutCommand_Execute, LogOutCommand_CanExecute);
            ShowLogInCommand = new RelayCommand(ShowLogInCommand_Execute, ShowLogInCommand_CanExecute);

            NewLogInPasswordCommand = new GenericRelayCommand<object>(
                param => NewLogInPasswordCommand_Execute(param),
                param => LogInCommand_CanExecute());

            LoginCancelCommand = new RelayCommand(LoginCancelCommand_Execute, LoginCancelCommand_CanExecute);

            SettingsCancelCommand = new RelayCommand(SettingsCancelCommand_Execute, SettingsCancelCommand_CanExecute);

            ShowSettingsCommand = new RelayCommand(ShowSettingsCommand_Execute, ShowSettingsCommand_CanExecute);
            SettingsOKCommand = new RelayCommand(SettingsOKCommand_Execute, SettingsOKCommand_CanExecute);

            ChangeLogInPasswordCommand = new GenericRelayCommand<object>(
                param => ChangeLogInPasswordCommand_Execute(param),
                param => ChangeLogInPasswordCommand_CanExecute());


            SetAssetsAPIKeyCommand = new RelayCommand(SetAssetsAPIKeyCommand_Execute, SetAssetsAPIKeyCommand_CanExecute);
            SetManualTradeAPIKeyCommand = new RelayCommand(SetManualTradeAPIKeyCommand_Execute, SetManualTradeAPIKeyCommand_CanExecute);
            SetOrdersAPIKeyCommand = new RelayCommand(SetOrdersAPIKeyCommand_Execute, SetOrdersAPIKeyCommand_CanExecute);
            SetIfdocoTradeAPIKeyCommand = new RelayCommand(SetIfdocoTradeAPIKeyCommand_Execute, SetIfdocoTradeAPIKeyCommand_CanExecute);
            SetAutoTradeAPIKeyCommand = new RelayCommand(SetAutoTradeAPIKeyCommand_Execute, SetAutoTradeAPIKeyCommand_CanExecute);
            SetTradeHistoryAPIKeyCommand = new RelayCommand(SetTradeHistoryAPIKeyCommand_Execute, SetTradeHistoryAPIKeyCommand_CanExecute);

            ShowApiKeyCommand = new GenericRelayCommand<object>(
                param => ShowApiKeyCommand_Execute(param),
                param => ShowApiKeyCommand_CanExecute());

            #endregion

            #region == APIクライアントのイニシャライズ ==

            // プライベートAPIクライアントのイニシャライズ
            _priApi = new PrivateAPIClient();
            // エラーイベントにサブスクライブ
            this._priApi.ErrorOccured += new PrivateAPIClient.ClinetErrorEvent(OnError);

            #endregion

            #region == ObservableCollection collections のスレッド対応 ==

            // スレッド対応 ObservableCollection collection
            BindingOperations.EnableCollectionSynchronization(this._orders, new object());
            BindingOperations.EnableCollectionSynchronization(this._tickHistory, new object());
            BindingOperations.EnableCollectionSynchronization(this._trades, new object());
            BindingOperations.EnableCollectionSynchronization(this._bitcoinNewsJa, new object());
            BindingOperations.EnableCollectionSynchronization(this._bitcoinNewsEn, new object());
            BindingOperations.EnableCollectionSynchronization(this._depth, new object());
            BindingOperations.EnableCollectionSynchronization(this._transactions, new object());
            BindingOperations.EnableCollectionSynchronization(this._ifdocos, new object());
            BindingOperations.EnableCollectionSynchronization(this._errors, new object());
            BindingOperations.EnableCollectionSynchronization(this._autoTrades2, new object());


            #endregion

            #region == チャートのイニシャライズ ==
            
            // Axes

            // 日時 X
            Axis caX = new Axis();
            caX.Name = "AxisX";
            caX.Title = "";
            caX.MaxValue = 60;
            caX.MinValue = 0;
            caX.Labels = new List<string>();
            //caX.LabelFormatter = Formatter;
            caX.Separator.StrokeThickness = 0.1;
            caX.Separator.StrokeDashArray = new DoubleCollection { 4 };
            caX.Separator.IsEnabled = false;
            caX.IsMerged = false;
            //caX.DisableAnimations = true;

            ChartAxisX.Add(caX);

            // 価格 Y
            Axis caY = new Axis();
            caY.Name = "Price";
            caY.Title = "";
            caY.MaxValue = double.NaN;
            caY.MinValue = double.NaN;
            caY.Position = AxisPosition.RightTop;
            caY.Separator.StrokeThickness = 0.1;
            caY.Separator.StrokeDashArray = new DoubleCollection { 4 };
            caY.IsMerged = false;
            caY.Separator.Stroke = System.Windows.Media.Brushes.WhiteSmoke;
            //caY.DisableAnimations = true;

            ChartAxisY.Add(caY);

            // 出来高 Y
            Axis vaY = new Axis();
            vaY.Name = "出来高";
            vaY.Title = "";
            vaY.ShowLabels = false;
            vaY.Labels = null;
            vaY.MaxValue = double.NaN;
            vaY.MinValue = double.NaN;
            vaY.Position = AxisPosition.RightTop;
            vaY.Separator.IsEnabled = false;
            vaY.Separator.StrokeThickness = 0;
            vaY.IsMerged = true;
            //vaY.DisableAnimations = true;
            
            ChartAxisY.Add(vaY);

            // sections

            // 現在値セクション
            AxisSection axs = new AxisSection();
            axs.Value = (double)_ltp;
            axs.Width = 0;
            //axs.SectionWidth = 0;
            axs.StrokeThickness = 0.4;
            axs.StrokeDashArray = new DoubleCollection { 4 };
            axs.Stroke = new SolidColorBrush(Colors.Orange);//new SolidColorBrush(System.Windows.Media.Color.FromRgb(150, 172, 206));
            axs.DataLabel = false;
            //axs.DataLabelForeground = new SolidColorBrush(Colors.Black);
            axs.DisableAnimations = true;
            
            ChartAxisY[0].Sections.Add(axs);

            // 色
            SolidColorBrush yellowBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 0));
            yellowBrush.Opacity = 0.1;

            // Lines
            ChartSeries = new SeriesCollection()
            {
                new CandleSeries()
                //new OhlcSeries()
                {
                    Title = "BTC/JPY",
                    Values = new ChartValues<OhlcPoint>{},
                    Fill = Brushes.Transparent,
                    ScalesYAt = 0,
                    //IncreaseBrush = new SolidColorBrush(_priceUpColor),//System.Windows.Media.Brushes.Aqua,
                    //DecreaseBrush = new SolidColorBrush(_priceDownColor),//System.Windows.Media.Brushes.Pink,
                    //IncreaseBrush = new SolidColorBrush(_chartIncreaseColor)
                    //DecreaseBrush = new SolidColorBrush(_chartDecreaseColor),

                },

                /*
                new LineSeries
                {
                    Title = "起動時価格",
                    PointGeometrySize = 5,
                    PointGeometry = null,
                    Values = new ChartValues<LiveCharts.Defaults.ObservableValue> {new LiveCharts.Defaults.ObservableValue(0)},
                    Fill = Brushes.Transparent,
                },
                */
                new LineSeries
                {
                    Title = "起動後最高値",
                    PointGeometrySize = 10,
                    PointGeometry = DefaultGeometries.Diamond,
                    Stroke = System.Windows.Media.Brushes.Aqua,
                    Values = new ChartValues<LiveCharts.Defaults.ObservableValue> {new LiveCharts.Defaults.ObservableValue(0)},
                    Fill = Brushes.Transparent,
                    ScalesYAt = 0,
                },
                new LineSeries
                {
                    Title = "起動後最低値",
                    PointGeometrySize = 10,
                    PointGeometry = DefaultGeometries.Diamond,
                    Stroke = System.Windows.Media.Brushes.Thistle,
                    Values = new ChartValues<LiveCharts.Defaults.ObservableValue> {new LiveCharts.Defaults.ObservableValue(0)},
                    Fill = Brushes.Transparent,
                    ScalesYAt = 0,
                },
                 new ColumnSeries
                {
                    Title = "出来高",
                    Values = new ChartValues<double> {},
                    ScalesYAt = 1,
                    Fill = yellowBrush,

        }
                /*
                new LineSeries
                {
                    Title = "BTC/JPY",
                    Values = new ChartValues<LiveCharts.Defaults.ObservableValue> {
                        new ObservableValue(0),
                        new ObservableValue(ChartAxisX[0].MaxValue)},
                    PointGeometrySize = 2,
                    PointGeometry = null,
                    LineSmoothness = 0,
                    StrokeThickness = 1,
                    Stroke = System.Windows.Media.Brushes.White,
                    StrokeDashArray = new System.Windows.Media.DoubleCollection {2},
                    Fill = Brushes.Transparent,
                },
                */
                /*
                new LineSeries
                {
                    Title = "過去24時間最高値",
                    PointGeometrySize = 10,
                    PointGeometry = DefaultGeometries.Square,
                    Stroke = System.Windows.Media.Brushes.DarkTurquoise,
                    Values = new ChartValues<LiveCharts.Defaults.ObservableValue> {new LiveCharts.Defaults.ObservableValue(0)},
                    Fill = Brushes.Transparent,
                },
                new LineSeries
                {
                    Title = "過去24時間最低値",
                    PointGeometrySize = 10,
                    PointGeometry = DefaultGeometries.Square,
                    Stroke = System.Windows.Media.Brushes.Pink,
                    Values = new ChartValues<LiveCharts.Defaults.ObservableValue> {new LiveCharts.Defaults.ObservableValue(0)},
                    Fill = Brushes.Transparent,
                },
                */




            };

            // double.NaN
            // new LiveCharts.Defaults.ObservableValue(0)
            #endregion

            #region == タイマー起動 ==

            // Tickerのタイマー起動
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(TickerTimer);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            // Ticker（他の通貨用）のタイマー起動
            System.Windows.Threading.DispatcherTimer dispatcherTimerTickOtherPairs = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimerTickOtherPairs.Tick += new EventHandler(TickerTimerOtherPairs);
            dispatcherTimerTickOtherPairs.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimerTickOtherPairs.Start();

            // Chart表示のタイマー起動
            System.Windows.Threading.DispatcherTimer dispatcherChartTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherChartTimer.Tick += new EventHandler(ChartTimer);
            dispatcherChartTimer.Interval = new TimeSpan(0, 1, 0);
            dispatcherChartTimer.Start();

            // 初回RSS フィードの取得
            Task.Run(() => GetRss());

            // RSSのタイマー起動
            System.Windows.Threading.DispatcherTimer dispatcherTimerRss = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimerRss.Tick += new EventHandler(RssTimer);
            dispatcherTimerRss.Interval = new TimeSpan(0, 15, 0);
            dispatcherTimerRss.Start();

            #endregion

            ShowSettings = false;

            // ループ再生開始　
            StartLoop();

            // チャートの表示
            Task.Run(async () =>
            {
                await Task.Run(() => DisplayChart());

            });

        }

        #endregion

        #region == イベント・タイマー系 ==

        // 現在価格取得 Tickerタイマー
        private async void TickerTimer(object source, EventArgs e)
        {
            try
            {

                Ticker tick = await _pubTickerApi.GetTicker(CurrentPair.ToString());

                if (tick != null)
                {
                    try
                    {
                        // Ticker 取得エラー表示をクリア
                        APIResultTicker = "";

                        // 値をセット
                        if (tick.LTP > _ltp)
                        {
                            LTPPriceColor = _priceUpColor;
                        }
                        else if (tick.LTP < _ltp)
                        {
                            LTPPriceColor = _priceDownColor;
                        }
                        else if (tick.LTP == _ltp)
                        {
                            //LTPPriceColor = Colors.Gainsboro;
                        }

                        Ltp = tick.LTP;
                        Bid = tick.Bid;
                        Ask = tick.Ask;
                        TickTimeStamp = tick.TimeStamp;

                        LowestIn24Price = tick.Low;
                        HighestIn24Price = tick.High;

                        // 起動時価格セット
                        if (_basePrice == 0) BasePrice = _ltp;

                        // 起動後の値と比較して(統計情報)アイコンを変える
                        this.NotifyPropertyChanged("BasePriceIcon");
                        this.NotifyPropertyChanged("BasePriceIconColor");

                        // ビットコイン時価評価額の計算
                        if (AssetBTCAmount != 0)
                        {
                            AssetBTCEstimateAmount = _ltp * AssetBTCAmount;
                        }

                        // 最安値登録
                        if (_lowestPrice == 0)
                        {
                            LowestPrice = _ltp;
                        }
                        if (_ltp < _lowestPrice)
                        {
                            //SystemSounds.Beep.Play();
                            LowestPrice = _ltp;
                        }

                        // 最高値登録
                        if (_highestPrice == 0)
                        {
                            HighestPrice = _ltp;
                        }
                        if (_ltp > _highestPrice)
                        {
                            //SystemSounds.Asterisk.Play();
                            HighestPrice = _ltp;
                        }

                        #region == チック履歴 ==
                         
                        TickHistory aym = new TickHistory();
                        aym.Price = _ltp;
                        aym.TimeAt = _tickTimeStamp;
                        if (_tickHistory.Count > 0)
                        {
                            if (_tickHistory[0].Price > aym.Price)
                            {
                                aym.TickHistoryPriceColor = _priceUpColor;
                                _tickHistory.Insert(0, aym);

                            }
                            else if (_tickHistory[0].Price < aym.Price)
                            {
                                aym.TickHistoryPriceColor = _priceDownColor;
                                _tickHistory.Insert(0, aym);
                            }
                            else
                            {
                                aym.TickHistoryPriceColor = Colors.Gainsboro;
                                _tickHistory.Insert(0, aym);
                            }
                        }
                        else
                        {
                            aym.TickHistoryPriceColor = Colors.Gainsboro;
                            _tickHistory.Insert(0, aym);
                        }

                        // limit the number of the list.
                        if (_tickHistory.Count > 60)
                        {
                            _tickHistory.RemoveAt(60);
                        }

                        // 60(1分)の平均値を求める
                        decimal aSum = 0;
                        int c = 0;
                        if (_tickHistory.Count > 0)
                        {

                            if (_tickHistory.Count > 60)
                            {
                                c = 59;
                            }
                            else
                            {
                                c = _tickHistory.Count - 1;
                            }

                            if (c == 0)
                            {
                                AveragePrice = _tickHistory[0].Price;
                            }
                            else
                            {
                                for (int i = 0; i < c; i++)
                                {
                                    aSum = aSum + _tickHistory[i].Price;
                                }
                                AveragePrice = aSum / c;
                            }

                        }
                        else if (_tickHistory.Count == 1)
                        {
                            AveragePrice = _tickHistory[0].Price;
                        }

                        #endregion

                        // 省エネモードでなかったら
                        if (!MinMode)
                        {
                            // チャートの現在値をセット
                            if (ChartAxisY[0].Sections.Count > 0)
                            {
                                ChartAxisY[0].Sections[0].Value = (double)Ltp;
                            }

                            // 特殊注文 
                            this.NotifyPropertyChanged("IFD_IfdEstimatePrice");
                            this.NotifyPropertyChanged("IFD_DoEstimatePrice");
                            this.NotifyPropertyChanged("OCO_OneEstimatePrice");
                            this.NotifyPropertyChanged("OCO_OtherEstimatePrice");
                            this.NotifyPropertyChanged("IFDOCO_IfdEstimatePrice");
                            this.NotifyPropertyChanged("IFDOCO_OneEstimatePrice");
                            this.NotifyPropertyChanged("IFDOCO_OtherEstimatePrice");

                            // 手動注文の成行予想金額表示の更新
                            if (SellType == OrderTypes.market)
                            {
                                this.NotifyPropertyChanged("SellEstimatePrice");
                            }
                            if (BuyType == OrderTypes.market)
                            {
                                this.NotifyPropertyChanged("BuyEstimatePrice");
                            }

                        } 

                        #region == アラーム ==

                        bool isPlayed = false;

                        // アラーム
                        if (AlarmPlus > 0)
                        {
                            if (_ltp >= AlarmPlus)
                            {
                                HighLowInfoTextColor = Colors.Aqua;
                                HighLowInfoText = "⇑⇑⇑　高値アラーム";

                                if (PlaySound)
                                {
                                    SystemSounds.Hand.Play();
                                    isPlayed = true;
                                }
                            }
                        }
                        else
                        {
                            // 起動後初期値セット
                            AlarmPlus = ((long)(_ltp / 1000) * 1000) + 20000;
                        }

                        if (AlarmMinus > 0)
                        {
                            if (_ltp <= AlarmMinus)
                            {
                                HighLowInfoTextColor = Colors.Pink;
                                HighLowInfoText = "⇓⇓⇓　安値アラーム";
                                if (PlaySound)
                                {
                                    SystemSounds.Beep.Play();
                                    isPlayed = true;
                                }
                            }
                        }
                        else
                        {
                            // 起動後初期値セット
                            AlarmMinus = ((long)(_ltp / 1000) * 1000) - 10000;
                        }

                        // 起動後最高値
                        if (_ltp >= _highestPrice)
                        {
                            if ((_tickHistory.Count > 25) && ((_basePrice + 2000M) < _ltp))
                            {
                                HighLowInfoTextColor = Colors.Aqua;
                                HighLowInfoText = "⇑⇑⇑　起動後最高値更新";

                                if ((isPlayed == false) && (PlaySoundHighest == true))
                                {
                                    if (PlaySound)
                                    {
                                        SystemSounds.Hand.Play();
                                        isPlayed = true;
                                    }
                                }
                            }
                        }
                        // 起動後最安値
                        if (_ltp <= _lowestPrice)
                        {
                            if ((_tickHistory.Count > 25) && ((_basePrice - 2000M) > _ltp))
                            {
                                HighLowInfoTextColor = Colors.Pink;
                                HighLowInfoText = "⇓⇓⇓　起動後最安値更新";

                                if ((isPlayed == false) && (PlaySoundLowest == true))
                                {
                                    if (PlaySound)
                                    {
                                        SystemSounds.Beep.Play();
                                        isPlayed = true;
                                    }
                                }
                            }
                        }

                        // 過去24時間最高値
                        if (_ltp >= _highestIn24Price)
                        {
                            HighLowInfoTextColor = Colors.Aqua;
                            HighLowInfoText = "⇑⇑⇑⇑⇑⇑　過去24時間最高値更新";

                            if ((isPlayed == false) && (PlaySoundHighest24h == true))
                            {
                                if (PlaySound)
                                {
                                    SystemSounds.Hand.Play();
                                    isPlayed = true;
                                }
                            }
                        }
                        // 過去24時間最安値
                        if (_ltp <= _lowestIn24Price)
                        {
                            HighLowInfoTextColor = Colors.Pink;
                            HighLowInfoText = "⇓⇓⇓⇓⇓⇓　過去24時間最安値更新";

                            if ((isPlayed == false) && (PlaySoundLowest24h == true))
                            {
                                if (PlaySound)
                                {
                                    SystemSounds.Hand.Play();
                                    isPlayed = true;
                                }
                            }
                        }

                        #endregion


                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("■■■■■ TickerTimer: Exception1 - " + ex.Message);

                    }
                }
                else
                {
                    APIResultTicker = "<<取得失敗>>";
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ TickerTimer Exception2: " + ex);
            }
        }

        // 現在価格取得（他の通貨） Tickerタイマー
        private async void TickerTimerOtherPairs(object source, EventArgs e)
        {

            // 各通貨ペアをループ
            foreach (string pair in Enum.GetNames(typeof(Pairs)))
            {
                if (pair == CurrentPair.ToString())
                {
                    continue;
                }

                //Debug.WriteLine(pair);

                Ticker tick = await _pubTickerApi.GetTicker(pair);

                if (tick != null)
                {
                    try
                    {
                        // 現在表示中の通貨 (This shouldn't be happening.)
                        if (pair == CurrentPair.ToString())
                        {
                            Ltps[CurrentPair] = tick.LTP;

                        }
                        else
                        {
                            // 他の通貨
                            Ltps[GetPairs[pair]] = tick.LTP;
                            
                            // 各通貨の時価評価額の更新
                            if (pair == "btc_jpy")
                            {
                                AssetBTCEstimateAmount = AssetBTCAmount * tick.LTP;

                                this.NotifyPropertyChanged("LtpBtc");
                            }
                            else if (pair == "xrp_jpy")
                            {
                                AssetXRPEstimateAmount = AssetXRPAmount * tick.LTP;

                                this.NotifyPropertyChanged("LtpXrp");
                            }
                            else if (pair == "eth_btc")
                            {
                                //AssetEthEstimateAmount
                                this.NotifyPropertyChanged("LtpEthBtc");
                            }
                            else if (pair == "mona_jpy")
                            {
                                AssetMonaEstimateAmount = AssetMonaAmount * tick.LTP;

                                this.NotifyPropertyChanged("LtpMonaJpy");
                            }
                            else if (pair == "mona_btc")
                            {
                                //
                                this.NotifyPropertyChanged("LtpMonaBtc");
                            }
                            else if (pair == "ltc_btc")
                            {
                                //AssetLtcEstimateAmount
                                this.NotifyPropertyChanged("LtpLtcBtc");
                            }
                            else if (pair == "bcc_btc")
                            {
                                //
                                this.NotifyPropertyChanged("LtpBccBtc");
                            }
                            else if (pair == "bcc_jpy")
                            {
                                AssetBchEstimateAmount = AssetBchAmount * tick.LTP;

                                this.NotifyPropertyChanged("LtpBchJpy");
                            }


                        }


                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("■■■■■ TickerTimerOtherPairs: Exception1 - " + ex.Message);
                        break;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ TickerTimerOtherPairs: GetTicker returned null");
                    break;
                }
            }
        }

        // 起動時の処理
        public void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            // データ保存フォルダの取得
            var AppDataFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            AppDataFolder = AppDataFolder + System.IO.Path.DirectorySeparatorChar + _appDeveloper + System.IO.Path.DirectorySeparatorChar + _appName;
            // 存在していなかったら作成
            System.IO.Directory.CreateDirectory(AppDataFolder);

            #region == アプリ設定のロード  ==

            // 設定ファイルのパス
            var AppConfigFilePath = AppDataFolder + System.IO.Path.DirectorySeparatorChar + _appName + ".config";

            // アプリ設定情報の読み込み
            if (File.Exists(AppConfigFilePath))
            {
                XDocument xdoc = XDocument.Load(AppConfigFilePath);

                #region == ウィンドウ関連 ==

                if (sender is Window)
                {
                    // Main Window element
                    var mainWindow = xdoc.Root.Element("MainWindow");
                    if (mainWindow != null)
                    {
                        var hoge = mainWindow.Attribute("top");
                        if (hoge != null)
                        {
                            (sender as Window).Top = double.Parse(hoge.Value);
                        }

                        hoge = mainWindow.Attribute("left");
                        if (hoge != null)
                        {
                            (sender as Window).Left = double.Parse(hoge.Value);
                        }

                        hoge = mainWindow.Attribute("height");
                        if (hoge != null)
                        {
                            (sender as Window).Height = double.Parse(hoge.Value);
                        }

                        hoge = mainWindow.Attribute("width");
                        if (hoge != null)
                        {
                            (sender as Window).Width = double.Parse(hoge.Value);
                        }

                        hoge = mainWindow.Attribute("state");
                        if (hoge != null)
                        {
                            if (hoge.Value == "Maximized")
                            {
                                (sender as Window).WindowState = WindowState.Maximized;
                            }
                            else if (hoge.Value == "Normal")
                            {
                                (sender as Window).WindowState = WindowState.Normal;
                            }
                            else if (hoge.Value == "Minimized")
                            {
                                (sender as Window).WindowState = WindowState.Normal;
                            }
                        }

                    }

                }

                #endregion

                #region == 認証関連・APIキー ==

                // 認証

                // User element
                var user = xdoc.Root.Element("User");
                if (user != null)
                { 
                    var pass = user.Attribute("password");
                    if (pass != null)
                    {
                        _realPassword = Decrypt(pass.Value);

                        if (string.IsNullOrEmpty(_realPassword) == false)
                        {
                            IsPasswordSet = true;
                        }
                        else
                        {
                            IsPasswordSet = false;
                        }

                    }

                    // APIキー

                    // Orders
                    var orders = user.Element("Orders");
                    if (orders != null)
                    {
                        var attr = orders.Attribute("key");
                        if (attr != null)
                        {
                            OrdersApiKey = Decrypt(attr.Value);
                        }
                        attr = orders.Attribute("secret");
                        if (attr != null)
                        {
                            OrdersSecret = Decrypt(attr.Value);
                        }

                        if (string.IsNullOrEmpty(_getOrdersApiKey) == false)
                        {
                            OrdersApiKeyIsSet = true;
                        }
                    }

                    // Assets
                    var assets = user.Element("Assets");
                    if (assets != null)
                    {
                        var attr = assets.Attribute("key");
                        if (attr != null)
                        {
                            AssetsApiKey = Decrypt(attr.Value);
                        }
                        attr = assets.Attribute("secret");
                        if (attr != null)
                        {
                            AssetsApiSecret = Decrypt(attr.Value);
                        }
                        //Debug.WriteLine(_getAssetsApiKey);
                        if (string.IsNullOrEmpty(_getAssetsApiKey) == false)
                        {
                            AssetsApiKeyIsSet = true;
                        }
                    }

                    // TradeHistory
                    var history = user.Element("TradeHistory");
                    if (history != null)
                    {
                        var attr = history.Attribute("key");
                        if (attr != null)
                        {
                            TradeHistoryApiKey = Decrypt(attr.Value);
                        }
                        attr = history.Attribute("secret");
                        if (attr != null)
                        {
                            TradeHistorySecret = Decrypt(attr.Value);
                        }

                        if (string.IsNullOrEmpty(_getTradeHistoryApiKey) == false)
                        {
                            TradeHistoryApiKeyIsSet = true;
                        }
                    }

                    // TradeHistory
                    var auto = user.Element("AutoTrade");
                    if (auto != null)
                    {
                        var attr = auto.Attribute("key");
                        if (attr != null)
                        {
                            AutoTradeApiKey = Decrypt(attr.Value);
                        }
                        attr = auto.Attribute("secret");
                        if (attr != null)
                        {
                            AutoTradeSecret = Decrypt(attr.Value);
                        }

                        if (string.IsNullOrEmpty(_autoTradeApiKey) == false)
                        {
                            AutoTradeApiKeyIsSet = true;
                        }
                    }

                    // ManualTrade
                    var manual = user.Element("ManualTrade");
                    if (manual != null)
                    {
                        var attr = manual.Attribute("key");
                        if (attr != null)
                        {
                            ManualTradeApiKey = Decrypt(attr.Value);
                        }
                        attr = manual.Attribute("secret");
                        if (attr != null)
                        {
                            ManualTradeSecret = Decrypt(attr.Value);
                        }

                        if (string.IsNullOrEmpty(_manualTradeApiKey) == false)
                        {
                            ManualTradeApiKeyIsSet = true;
                        }
                    }

                    // IFDOCO
                    var ifdocoOrd = user.Element("IFDOCO");
                    if (ifdocoOrd != null)
                    {
                        var attr = ifdocoOrd.Attribute("key");
                        if (attr != null)
                        {
                            IfdocoTradeApiKey = Decrypt(attr.Value);
                        }
                        attr = ifdocoOrd.Attribute("secret");
                        if (attr != null)
                        {
                            IfdocoTradeSecret = Decrypt(attr.Value);
                        }

                        if (string.IsNullOrEmpty(_ifdocoTradeApiKey) == false)
                        {
                            IfdocoTradeApiKeyIsSet = true;
                        }
                    }



                }

                #endregion

                #region == アラーム関係 ==

                var alarmSetting = xdoc.Root.Element("Alarm");
                if (alarmSetting != null)
                {
                    var hoge = alarmSetting.Attribute("playSoundLowest");
                    if (hoge != null)
                    {
                        if (hoge.Value == "true")
                        {
                            PlaySoundLowest = true;
                        }
                        else
                        {
                            PlaySoundLowest = false;
                        }
                    }

                    hoge = alarmSetting.Attribute("playSoundLowest24h");
                    if (hoge != null)
                    {
                        if (hoge.Value == "true")
                        {
                            PlaySoundLowest24h = true;
                        }
                        else
                        {
                            PlaySoundLowest24h = false;
                        }
                    }

                    hoge = alarmSetting.Attribute("playSoundHighest");
                    if (hoge != null)
                    {
                        if (hoge.Value == "true")
                        {
                            PlaySoundHighest = true;
                        }
                        else
                        {
                            PlaySoundHighest = false;
                        }
                    }

                    hoge = alarmSetting.Attribute("playSoundHighest24h");
                    if (hoge != null)
                    {
                        if (hoge.Value == "true")
                        {
                            PlaySoundHighest24h = true;
                        }
                        else
                        {
                            PlaySoundHighest24h = false;
                        }
                    }

                    hoge = alarmSetting.Attribute("playSound");
                    if (hoge != null)
                    {
                        if (hoge.Value == "true")
                        {
                            PlaySound = true;
                        }
                        else
                        {
                            PlaySound = false;
                        }
                    }
                    
                }

                #endregion

            }

            #endregion

            #region == 特殊注文の保存データのロード ==

            // 特殊注文をロード
            //var BTC_IFDOCOsFilePath = AppDomain.CurrentDomain.BaseDirectory + "BTC_IFDOCOs.csv";

            var BTC_IFDOCOsFilePath = AppDataFolder + System.IO.Path.DirectorySeparatorChar + "BTC_IFDOCOs.csv";

            if (File.Exists(BTC_IFDOCOsFilePath))
            {
                try
                {
                    var contents = File.ReadAllText(BTC_IFDOCOsFilePath).Split('\n');
                    var csv = from line in contents select line.Split(',').ToArray();

                    foreach (var row in csv)
                    {
                        if (string.IsNullOrEmpty(row[0]))
                        {
                            break;
                        }

                        //System.Diagnostics.Debug.WriteLine("■■■■■ "+ row[0]+"-"+ row[1]+"-"+ row[2]);

                        // Kind[0], IFDone_OrderID[1], IFD_DoOrderID[2], IFD_DoSide[3], IFD_DoType[4], IFD_DoAmount[5], IFD_DoPrice[6], OCO_OneOrderID[7], OCO_OneSide[8], OCO_OneType[9], OCO_OneAmount[10], OCO_OnePrice[11], OCO_OtherOrderID[12], OCO_OtherSide[13], OCO_OtherType[14], OCO_OtherAmount[15], OCO_OtherPrice[16]
                        //IFD_DoTriggerPrice, OCO_OneTriggerPrice, OCO_OtherTriggerPrice

                        Ifdoco asdf = new Ifdoco();

                        if (row[0] == "ifd")
                        {
                            asdf.Kind = IfdocoKinds.ifd;

                            asdf.IfdoneOrderID = Int32.Parse(row[1]);

                            asdf.IfdDoOrderID = Int32.Parse(row[2]);
                            asdf.IfdDoSide = row[3];

                            if (row[4] == "limit")
                            {
                                asdf.IfdDoType = IfdocoTypes.limit;
                            }
                            else if (row[4] == "market")
                            {
                                asdf.IfdDoType = IfdocoTypes.market;
                            }

                            asdf.IfdDoStartAmount = Decimal.Parse(row[5]);
                            asdf.IfdDoPrice = Decimal.Parse(row[6]);

                            //System.Diagnostics.Debug.WriteLine("■■■■■ "+ row[0]+"-"+ row[1]+"-"+ row[2]);
                        }
                        else if (row[0] == "oco")
                        {
                            asdf.Kind = IfdocoKinds.oco;

                            asdf.OcoOneOrderID = Int32.Parse(row[7]);
                            asdf.OcoOneSide = row[8];
                            if (row[9] == "limit")
                            {
                                asdf.OcoOneType = IfdocoTypes.limit;
                            }
                            else if (row[9] == "market")
                            {
                                asdf.OcoOneType = IfdocoTypes.market;
                            }

                            asdf.OcoOneStartAmount = Decimal.Parse(row[10]);
                            asdf.OcoOnePrice = Decimal.Parse(row[11]);

                            asdf.OcoOtherOrderID = Int32.Parse(row[12]);
                            asdf.OcoOtherSide = row[13];
                            if (row[14] == "limit")
                            {
                                asdf.OcoOtherType = IfdocoTypes.limit;
                            }
                            else if (row[14] == "market")
                            {
                                asdf.OcoOtherType = IfdocoTypes.market;
                            }

                            asdf.OcoOtherStartAmount = Decimal.Parse(row[15]);
                            asdf.OcoOtherPrice = Decimal.Parse(row[16]);

                        }
                        else if (row[0] == "ifdoco")
                        {
                            asdf.Kind = IfdocoKinds.ifdoco;

                            asdf.IfdoneOrderID = Int32.Parse(row[1]);

                            asdf.OcoOneOrderID = Int32.Parse(row[7]);
                            asdf.OcoOneSide = row[8];
                            if (row[9] == "limit")
                            {
                                asdf.OcoOneType = IfdocoTypes.limit;
                            }
                            else if (row[9] == "market")
                            {
                                asdf.OcoOneType = IfdocoTypes.market;
                            }

                            asdf.OcoOneStartAmount = Decimal.Parse(row[10]);
                            asdf.OcoOnePrice = Decimal.Parse(row[11]);

                            asdf.OcoOtherOrderID = Int32.Parse(row[12]);
                            asdf.OcoOtherSide = row[13];
                            if (row[14] == "limit")
                            {
                                asdf.OcoOtherType = IfdocoTypes.limit;
                            }
                            else if (row[14] == "market")
                            {
                                asdf.OcoOtherType = IfdocoTypes.market;
                            }

                            asdf.OcoOtherStartAmount = Decimal.Parse(row[15]);
                            asdf.OcoOtherPrice = Decimal.Parse(row[16]);

                        }

                        asdf.IfdDoTriggerPrice = Decimal.Parse(row[17]);
                        asdf.OcoOneTriggerPrice = Decimal.Parse(row[18]);
                        asdf.OcoOtherTriggerPrice = Decimal.Parse(row[19]);

                        // リストへ追加
                        _ifdocos.Add(asdf);

                        // IDカウンターをセット
                        if (asdf.Id > IfdocoIdCount)
                        {
                            IfdocoIdCount = asdf.Id;
                        }

                    }
                }
                catch (System.IO.FileNotFoundException) { }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ Error  特殊注文の保存データロード中: " + ex + " while opening : " + BTC_IFDOCOsFilePath);
                }
            }

            #endregion

        }

        // 終了時の処理
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            // データ保存フォルダの取得
            var AppDataFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            AppDataFolder = AppDataFolder + System.IO.Path.DirectorySeparatorChar + _appDeveloper + System.IO.Path.DirectorySeparatorChar + _appName;
            // 存在していなかったら作成
            System.IO.Directory.CreateDirectory(AppDataFolder);

            #region == アプリ設定の保存 ==

            // 設定ファイルのパス
            var AppConfigFilePath = AppDataFolder + System.IO.Path.DirectorySeparatorChar + _appName + ".config";

            // 設定ファイル用のXMLオブジェクト
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.InsertBefore(xmlDeclaration, doc.DocumentElement);

            // Root Document Element
            XmlElement root = doc.CreateElement(string.Empty, "App", string.Empty);
            doc.AppendChild(root);

            XmlAttribute attrs = doc.CreateAttribute("Version");
            attrs.Value = _appVer;
            root.SetAttributeNode(attrs);

            #region == ウィンドウ関連 ==

            if (sender is Window)
            {
                // Main Window element
                XmlElement mainWindow = doc.CreateElement(string.Empty, "MainWindow", string.Empty);

                // Main Window attributes
                attrs = doc.CreateAttribute("height");
                if ((sender as Window).WindowState == WindowState.Maximized)
                {
                    attrs.Value = (sender as Window).RestoreBounds.Height.ToString();
                }
                else
                {
                    attrs.Value = (sender as Window).Height.ToString();
                }
                mainWindow.SetAttributeNode(attrs);

                attrs = doc.CreateAttribute("width");
                if ((sender as Window).WindowState == WindowState.Maximized)
                {
                    attrs.Value = (sender as Window).RestoreBounds.Width.ToString();
                }
                else
                {
                    attrs.Value = (sender as Window).Width.ToString();

                }
                mainWindow.SetAttributeNode(attrs);

                attrs = doc.CreateAttribute("top");
                if ((sender as Window).WindowState == WindowState.Maximized)
                {
                    attrs.Value = (sender as Window).RestoreBounds.Top.ToString();
                }
                else
                {
                    attrs.Value = (sender as Window).Top.ToString();
                }
                mainWindow.SetAttributeNode(attrs);

                attrs = doc.CreateAttribute("left");
                if ((sender as Window).WindowState == WindowState.Maximized)
                {
                    attrs.Value = (sender as Window).RestoreBounds.Left.ToString();
                }
                else
                {
                    attrs.Value = (sender as Window).Left.ToString();
                }
                mainWindow.SetAttributeNode(attrs);

                attrs = doc.CreateAttribute("state");
                if ((sender as Window).WindowState == WindowState.Maximized)
                {
                    attrs.Value = "Maximized";
                }
                else if ((sender as Window).WindowState == WindowState.Normal)
                {
                    attrs.Value = "Normal";

                }
                else if ((sender as Window).WindowState == WindowState.Minimized)
                {
                    attrs.Value = "Minimized";
                }
                mainWindow.SetAttributeNode(attrs);

                // set Main Window element to root.
                root.AppendChild(mainWindow);

            }

            #endregion

            #region == 認証関連 ==

            // User element
            XmlElement user = doc.CreateElement(string.Empty, "User", string.Empty);

            // User attributes
            attrs = doc.CreateAttribute("password");
            attrs.Value = Encrypt(_realPassword);
            user.SetAttributeNode(attrs);

            // set User element to root.
            root.AppendChild(user);

            // APIキー関連
            // Orders
            XmlElement orders = doc.CreateElement(string.Empty, "Orders", string.Empty);

            // Orders attributes
            attrs = doc.CreateAttribute("key");
            attrs.Value = Encrypt(_getOrdersApiKey);
            orders.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("secret");
            attrs.Value = Encrypt(_getOrdersSecret);
            orders.SetAttributeNode(attrs);

            // set Orders element to user.
            user.AppendChild(orders);

            // Assets
            XmlElement assets = doc.CreateElement(string.Empty, "Assets", string.Empty);

            //  attributes
            attrs = doc.CreateAttribute("key");
            attrs.Value = Encrypt(_getAssetsApiKey);
            assets.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("secret");
            attrs.Value = Encrypt(_getAssetsSecret);
            assets.SetAttributeNode(attrs);

            // set Assets element to user.
            user.AppendChild(assets);

            // TradeHistory
            XmlElement history = doc.CreateElement(string.Empty, "TradeHistory", string.Empty);

            //  attributes
            attrs = doc.CreateAttribute("key");
            attrs.Value = Encrypt(_getTradeHistoryApiKey);
            history.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("secret");
            attrs.Value = Encrypt(_getTradeHistorySecret);
            history.SetAttributeNode(attrs);

            // set  element to user.
            user.AppendChild(history);

            // AutoTrade
            XmlElement auto = doc.CreateElement(string.Empty, "AutoTrade", string.Empty);

            //  attributes
            attrs = doc.CreateAttribute("key");
            attrs.Value = Encrypt(_autoTradeApiKey);
            auto.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("secret");
            attrs.Value = Encrypt(_autoTradeSecret);
            auto.SetAttributeNode(attrs);

            // set  element to user.
            user.AppendChild(auto);

            // ManualTrade
            XmlElement manual = doc.CreateElement(string.Empty, "ManualTrade", string.Empty);

            //  attributes
            attrs = doc.CreateAttribute("key");
            attrs.Value = Encrypt(_manualTradeApiKey);
            manual.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("secret");
            attrs.Value = Encrypt(_manualTradeSecret);
            manual.SetAttributeNode(attrs);

            // set  element to user.
            user.AppendChild(manual);

            // IFDOCO
            XmlElement ifdocoOrd = doc.CreateElement(string.Empty, "IFDOCO", string.Empty);

            //  attributes
            attrs = doc.CreateAttribute("key");
            attrs.Value = Encrypt(_ifdocoTradeApiKey);
            ifdocoOrd.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("secret");
            attrs.Value = Encrypt(_ifdocoTradeSecret);
            ifdocoOrd.SetAttributeNode(attrs);

            // set  element to user.
            user.AppendChild(ifdocoOrd);

            #endregion

            #region == アラーム関連 ==

            XmlElement alarmSetting = doc.CreateElement(string.Empty, "Alarm", string.Empty);

            attrs = doc.CreateAttribute("playSoundLowest");
            if (PlaySoundLowest)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            alarmSetting.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundLowest24h");
            if (PlaySoundLowest24h)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            alarmSetting.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundHighest");
            if (PlaySoundHighest)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            alarmSetting.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundHighest24h");
            if (PlaySoundHighest24h)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            alarmSetting.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSound");
            if (PlaySound)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            alarmSetting.SetAttributeNode(attrs);
            

            root.AppendChild(alarmSetting);

            #endregion

            // 設定ファイルの保存
            doc.Save(AppConfigFilePath);

            #endregion

            #region == 特殊注文のデータ保存 ==

            var BTC_IFDOCOsFilePath = AppDataFolder + System.IO.Path.DirectorySeparatorChar + "BTC_IFDOCOs.csv";

            if (_ifdocos.Count > 0)
            {
                var csv = new StringBuilder();

                //var filePath = AppDomain.CurrentDomain.BaseDirectory + "BTC_IFDOCOs.csv";

                var BTC_IFDOCOsFilePath_backup = AppDataFolder + System.IO.Path.DirectorySeparatorChar + "BTC_IFDOCOs.backup.csv";

                foreach (var ifdoco in _ifdocos)
                {

                    // Kind, IFDone_OrderID, IFD_DoOrderID, IFD_DoSide, IFD_DoType, IFD_DoAmount, IFD_DoPrice, OCO_OneOrderID, OCO_OneSide, OCO_OneType, OCO_OneAmount, OCO_OnePrice, OCO_OtherOrderID, OCO_OtherSide, OCO_OtherType, OCO_OtherAmount, OCO_OtherPrice
                    // ifdoco.IfdDoTriggerPrice.ToString(), ifdoco.OcoOneTriggerPrice.ToString(), ifdoco.OcoOtherTriggerPrice.ToString()

                    // 未約定のみ保存する？それとも残す？

                    if (ifdoco.Kind == IfdocoKinds.ifd)
                    {
                        // 未約定のみ保存する
                        if (ifdoco.IfdIsDone == false)
                        {
                            //                                                                                                             Kind,                   IFDone_OrderID,                  IFD_DoOrderID,                  IFD_DoSide,       IFD_DoType,                  IFD_DoAmount,                       IFD_DoPrice,                  OCO_OneOrderID, OCO_OneSide, OCO_OneType, OCO_OneAmount, OCO_OnePrice, OCO_OtherOrderID, OCO_OtherSide, OCO_OtherType, OCO_OtherAmount, OCO_OtherPrice
                            var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}", ifdoco.Kind.ToString(), ifdoco.IfdoneOrderID.ToString(), ifdoco.IfdDoOrderID.ToString(), ifdoco.IfdDoSide, ifdoco.IfdDoType.ToString(), ifdoco.IfdDoStartAmount.ToString(), ifdoco.IfdDoPrice.ToString(), 0, "", "", 0, 0, 0, "", "", 0, 0, ifdoco.IfdDoTriggerPrice.ToString(), ifdoco.OcoOneTriggerPrice.ToString(), ifdoco.OcoOtherTriggerPrice.ToString());
                            csv.AppendLine(newLine);
                        }

                    }
                    else if (ifdoco.Kind == IfdocoKinds.oco)
                    {
                        // 未約定のみ保存する
                        if (ifdoco.OcoIsDone == false)
                        {
                            var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}", ifdoco.Kind.ToString(), 0, 0, "", "", 0, 0, ifdoco.OcoOneOrderID.ToString(), ifdoco.OcoOneSide, ifdoco.OcoOneType.ToString(), ifdoco.OcoOneStartAmount, ifdoco.OcoOnePrice, ifdoco.OcoOtherOrderID.ToString(), ifdoco.OcoOtherSide, ifdoco.OcoOtherType.ToString(), ifdoco.OcoOtherStartAmount.ToString(), ifdoco.OcoOtherPrice.ToString(), ifdoco.IfdDoTriggerPrice.ToString(), ifdoco.OcoOneTriggerPrice.ToString(), ifdoco.OcoOtherTriggerPrice.ToString());
                            csv.AppendLine(newLine);
                        }

                    }
                    else if (ifdoco.Kind == IfdocoKinds.ifdoco)
                    {
                        // 未約定のみ保存する
                        if (ifdoco.IfdocoIsDone == false)
                        {
                            var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19}", ifdoco.Kind.ToString(), ifdoco.IfdoneOrderID.ToString(), ifdoco.IfdDoOrderID.ToString(), ifdoco.IfdDoSide, ifdoco.IfdDoType.ToString(), ifdoco.IfdDoStartAmount.ToString(), ifdoco.IfdDoPrice.ToString(), ifdoco.OcoOneOrderID.ToString(), ifdoco.OcoOneSide, ifdoco.OcoOneType.ToString(), ifdoco.OcoOneStartAmount, ifdoco.OcoOnePrice, ifdoco.OcoOtherOrderID.ToString(), ifdoco.OcoOtherSide, ifdoco.OcoOtherType.ToString(), ifdoco.OcoOtherStartAmount.ToString(), ifdoco.OcoOtherPrice.ToString(), ifdoco.IfdDoTriggerPrice.ToString(), ifdoco.OcoOneTriggerPrice.ToString(), ifdoco.OcoOtherTriggerPrice.ToString());
                            csv.AppendLine(newLine);
                        }
                    }

                }

                File.WriteAllText(BTC_IFDOCOsFilePath, csv.ToString());


                // デバッグ用・異常終了した時の確認用

                File.WriteAllText(BTC_IFDOCOsFilePath_backup, csv.ToString());

            }
            else
            {
                // リストが空なので、ファイルも削除。
                if (File.Exists(BTC_IFDOCOsFilePath))
                {
                    File.Delete(BTC_IFDOCOsFilePath);
                }
            }

            #endregion
        }

        // チャート表示 タイマー
        private void ChartTimer(object source, EventArgs e)
        {
            // TODO MiniModeから復帰したときに、どうなるのか。。。>MiniModeでも更新
            //if (MinMode == false)
            //{
                try
                {
                    UpdateCandlestick();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ ChartTimer Exception: " + ex);
                }
            //}
        }

        // RSS 取得タイマー
        private void RssTimer(object source, EventArgs e)
        {
            // 省エネモードならスルー。
            if (MinMode)
            {
                Task.Run(() => GetRss());
            }

        }

        // エラーイベント
        private void OnError(PrivateAPIClient sender, ClientError err)
        {
            if (err == null) { return; }

            // TODO
            err.ErrPlaceParent = "";

            _errors.Insert(0, err);

            if (Application.Current == null) { return; }
            Application.Current.Dispatcher.Invoke(() =>
            {
                // タブの「エラー（＊）」を更新
                ErrorsCount = _errors.Count;
            });
        }

        #endregion

        #region == メソッド ==

        #region == 認証関連のメソッド ==

        /*
        // 暗号化
        private static string Protect(string str)
        {
            byte[] entropy = Encoding.ASCII.GetBytes(Assembly.GetExecutingAssembly().FullName);
            byte[] data = Encoding.ASCII.GetBytes(str);
            string protectedData = Convert.ToBase64String(ProtectedData.Protect(data, entropy, DataProtectionScope.CurrentUser));
            return protectedData;
        }
        // 復号化
        private static string Unprotect(string str)
        {
            byte[] protectedData = Convert.FromBase64String(str);
            byte[] entropy = Encoding.ASCII.GetBytes(Assembly.GetExecutingAssembly().FullName);
            string data = Encoding.ASCII.GetString(ProtectedData.Unprotect(protectedData, entropy, DataProtectionScope.CurrentUser));
            return data;
        }
        */

        // 暗号化
        private string Encrypt(string s)
        {
            if (String.IsNullOrEmpty(s)) { return ""; }

            byte[] entropy = new byte[] { 0x72, 0xa2, 0x12, 0x04 };

            try
            {
                byte[] userData = System.Text.Encoding.UTF8.GetBytes(s);

                byte[] encryptedData = ProtectedData.Protect(userData, entropy, DataProtectionScope.CurrentUser);

                return System.Convert.ToBase64String(encryptedData);
            }
            catch
            {
                return "";
            }
        }

        // 復号化
        private string Decrypt(string s)
        {
            if (String.IsNullOrEmpty(s)) { return ""; }

            byte[] entropy = new byte[] { 0x72, 0xa2, 0x12, 0x04 };

            try
            {
                byte[] encryptedData = System.Convert.FromBase64String(s);

                byte[] userData = ProtectedData.Unprotect(encryptedData, entropy, DataProtectionScope.CurrentUser);

                return System.Text.Encoding.UTF8.GetString(userData);
            }
            catch
            {
                return "";
            }
        }

        // ダミー＊＊＊生成
        private string DummyPassword(string s)
        {
            if (String.IsNullOrEmpty(s)) { return ""; }
            string e = "";
            for (int i = 1; i <= s.Length; i++)
            {
                e = e + "*";
            }
            return e;
        }

        #endregion

        #region == チャート関係のメソッド ==

        // ロウソク足 Candlestick取得メソッド
        private async Task<List<Ohlcv>> GetCandlestick(CandleTypes ct, DateTime dtTarget)
        {

            string ctString = "";
            string dtString = "";
            if (ct == CandleTypes.OneMin)
            {
                ctString = "1min";
                dtString = dtTarget.ToString("yyyyMMdd");
            }
            else if (ct == CandleTypes.OneHour)
            {
                ctString = "1hour";
                dtString = dtTarget.ToString("yyyyMMdd");
            }
            else if (ct == CandleTypes.OneDay)
            {
                ctString = "1day";
                dtString = dtTarget.ToString("yyyy");
            }
            else
            {
                // デフォ。あってはならない。
                //ctString = "1hour";
                //dtString = dtTarget.ToString("yyyyMMdd");

                throw new System.InvalidOperationException("サポートしてないロウソク足単位");
                //return null;
            }
            // 1min 5min 15min 30min 1hour 4hour 8hour 12hour 1day 1week


            CandlestickResult csr = await _pubCandlestickApi.GetCandlestick(ctString, dtString);
            if (csr != null)
            {
                if (csr.IsSuccess == true)
                {
                    if (csr.Candlesticks.Count > 0)
                    {
                        // ロウソク足タイプが同じかどうか一応確認
                        if (csr.Candlesticks[0].Type.ToString() == ct.ToString())
                        {
                            return csr.Candlesticks[0].Ohlcvs;

                        }
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ GetCandlestick: GetCandlestick returned error");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetCandlestick: GetCandlestick returned null");
            }

            return null;
        }

        // 初回に各種Candlestickをまとめて取得
        private async Task<bool> GetCandlesticks()
        {
            ChartLoadingInfo = "チャートデータを取得中....";

            // 今日の日付セット。UTCで。
            DateTime dtToday = DateTime.Now.ToUniversalTime();

            // データは、ローカルタイムで、朝9:00 から翌8:59分まで。8:59分までしか取れないので、 9:00過ぎていたら 最新のデータとるには日付を１日追加する

            #region == OhlcvsOneHour 1hour毎のデータ ==

            // TODO 取得中フラグセット。

            Debug.WriteLine("1hour取得開始");

            // 一時間のロウソク足タイプなら今日、昨日、一昨日、その前の１週間分の1hourデータを取得する必要あり。
            OhlcvsOneHour = await GetCandlestick(CandleTypes.OneHour, dtToday);
            if (OhlcvsOneHour != null)
            {
                // 逆順にする
                OhlcvsOneHour.Reverse();

                await Task.Delay(300);
                // 昨日
                DateTime dtTarget = dtToday.AddDays(-1);

                List<Ohlcv> res = await GetCandlestick(CandleTypes.OneHour, dtTarget);
                if (res != null)
                {
                    // 逆順にする
                    res.Reverse();

                    foreach (var r in res)
                    {
                        OhlcvsOneHour.Add(r);
                    }

                    await Task.Delay(300);
                    // 一昨日
                    dtTarget = dtTarget.AddDays(-1);
                    List<Ohlcv> last2 = await GetCandlestick(CandleTypes.OneHour, dtTarget);
                    if (last2 != null)
                    {
                        // 逆順にする
                        last2.Reverse();

                        foreach (var l in last2)
                        {
                            OhlcvsOneHour.Add(l);
                        }

                        await Task.Delay(300);
                        // ３日前
                        dtTarget = dtTarget.AddDays(-1);
                        List<Ohlcv> last3 = await GetCandlestick(CandleTypes.OneHour, dtTarget);
                        if (last3 != null)
                        {
                            // 逆順にする
                            last3.Reverse();

                            foreach (var l in last3)
                            {
                                OhlcvsOneHour.Add(l);
                            }

                            await Task.Delay(300);
                            // 4日前
                            dtTarget = dtTarget.AddDays(-1);
                            List<Ohlcv> last4 = await GetCandlestick(CandleTypes.OneHour, dtTarget);
                            if (last4 != null)
                            {
                                // 逆順にする
                                last4.Reverse();

                                foreach (var l in last4)
                                {
                                    OhlcvsOneHour.Add(l);
                                }

                                await Task.Delay(300);
                                // 5日前
                                dtTarget = dtTarget.AddDays(-1);
                                List<Ohlcv> last5 = await GetCandlestick(CandleTypes.OneHour, dtTarget);
                                if (last5 != null)
                                {
                                    // 逆順にする
                                    last5.Reverse();

                                    foreach (var l in last5)
                                    {
                                        OhlcvsOneHour.Add(l);
                                    }

                                    await Task.Delay(300);
                                    // 6日前
                                    dtTarget = dtTarget.AddDays(-1);
                                    List<Ohlcv> last6 = await GetCandlestick(CandleTypes.OneHour, dtTarget);
                                    if (last6 != null)
                                    {
                                        // 逆順にする
                                        last6.Reverse();

                                        foreach (var l in last6)
                                        {
                                            OhlcvsOneHour.Add(l);
                                        }

                                        await Task.Delay(300);
                                        // 7日前
                                        dtTarget = dtTarget.AddDays(-1);
                                        List<Ohlcv> last7 = await GetCandlestick(CandleTypes.OneHour, dtTarget);
                                        if (last7 != null)
                                        {
                                            // 逆順にする
                                            last7.Reverse();

                                            foreach (var l in last7)
                                            {
                                                OhlcvsOneHour.Add(l);
                                            }
                                        }


                                    }
                                }
                            }
                        }
                    }


                }
            }

            // TODO 取得中フラグ解除。

            #endregion

            await Task.Delay(600);

            #region == OhlcvsOneMin 1min毎のデータ ==

            // TODO 取得中フラグセット。

            Debug.WriteLine("今日の1min取得開始");

            // 一分毎のロウソク足タイプなら今日と昨日の1minデータを取得する必要あり。
            OhlcvsOneMin = await GetCandlestick(CandleTypes.OneMin, dtToday);
            if (OhlcvsOneMin != null)
            {
                // 逆順にする
                OhlcvsOneMin.Reverse();

                await Task.Delay(500);

                // 00:00:00から23:59:59分までしか取れないので、 3時間分取るには、00:00:00から3:00までは 最新のデータとるには日付を１日マイナスする
                if (dtToday.Hour <= 3) // < 3
                {
                    Debug.WriteLine("昨日の1min取得開始");
                    // 昨日
                    DateTime dtTarget = dtToday.AddDays(-1);

                    List<Ohlcv> res = await GetCandlestick(CandleTypes.OneMin, dtTarget);
                    if (res != null)
                    {
                        // 逆順にする
                        res.Reverse();

                        foreach (var r in res)
                        {
                            OhlcvsOneMin.Add(r);
                        }
                    }
                }
            }

            // TODO 取得中フラグ解除。

            #endregion

            await Task.Delay(600);

            #region == OhlcvsOneDay 1day毎のデータ ==

            // TODO 取得中フラグセット。

            // 1日のロウソク足タイプなら今年、去年、２年前、３年前、４年前、５年前の1hourデータを取得する必要あり。(５年前は止めた)

            OhlcvsOneDay = await GetCandlestick(CandleTypes.OneDay, dtToday);
            if (OhlcvsOneDay != null)
            {
                // 逆順にする
                OhlcvsOneDay.Reverse();

                await Task.Delay(300);
                // 今年
                DateTime dtTarget = dtToday.AddYears(-1);

                List<Ohlcv> res = await GetCandlestick(CandleTypes.OneDay, dtTarget);
                if (res != null)
                {
                    // 逆順にする
                    res.Reverse();

                    foreach (var r in res)
                    {
                        OhlcvsOneDay.Add(r);
                    }

                    await Task.Delay(300);
                    // 去年
                    dtTarget = dtTarget.AddYears(-1);
                    List<Ohlcv> last = await GetCandlestick(CandleTypes.OneDay, dtTarget);
                    if (last != null)
                    {
                        // 逆順にする
                        last.Reverse();

                        foreach (var l in last)
                        {
                            OhlcvsOneDay.Add(l);
                        }


                        // (５年前は止めた)
                    }

                }
            }

            // TODO 取得中フラグ解除。

            #endregion

            ChartLoadingInfo = "";

            return true;

        }

        // チャートを読み込み表示する。
        private void LoadChart()
        {
            // TODO 個別にロードして、初期表示を早くする。

            ChartLoadingInfo = "チャートをロード中....";

            CandleTypes ct = SelectedCandleType;

            List<Ohlcv> lst;
            int span = 0;

            if (ct == CandleTypes.OneMin)
            {
                // 一分毎のロウソク足タイプなら
                lst = OhlcvsOneMin;

                // 一時間の期間か１日の期間
                if (_chartSpan == ChartSpans.OneHour)
                {
                    span = 60;// +1
                }
                else if (_chartSpan == ChartSpans.ThreeHour)
                {
                    span = 60 * 3;
                }
                else
                {
                    throw new System.InvalidOperationException("一分毎のロウソク足タイプなら、負荷掛かり過ぎなので、１日以上は無し");
                }

                // 負荷掛かり過ぎなので、１日は無し
                /*
                else if (_chartSpan == ChartSpans.OneDay)
                {
                    span = 60*24;
                }
                */

                //Debug.WriteLine("OneMin数" + test.Count.ToString());
            }
            else if (ct == CandleTypes.OneHour)
            {
                // 一時間のロウソク足タイプなら
                lst = OhlcvsOneHour;

                // １日の期間か3日か１週間の期間
                if (_chartSpan == ChartSpans.OneDay)
                {
                    span = 24;
                }
                else if (_chartSpan == ChartSpans.ThreeDay)
                {
                    span = 24 * 3; // +1
                }
                else if (_chartSpan == ChartSpans.OneWeek)
                {
                    span = 24 * 7;
                }
                else
                {
                    throw new System.InvalidOperationException("時間毎のロウソク足タイプなら、負荷掛かり過ぎなので、1週間以上は無し。一日未満もなし");
                }

                // Debug.WriteLine("OneHour数" + test.Count.ToString());
            }
            else if (ct == CandleTypes.OneDay)
            {
                // 1日のロウソク足タイプなら
                lst = OhlcvsOneDay;

                // 1ヵ月、2ヵ月、１年、５年の期間
                if (_chartSpan == ChartSpans.OneMonth)
                {
                    span = 30;//.44
                }
                else if (_chartSpan == ChartSpans.TwoMonth)
                {
                    span = 30 * 2;
                }
                else if (_chartSpan == ChartSpans.OneYear)
                {
                    span = 365;//.2425
                }
                else if (_chartSpan == ChartSpans.FiveYear)
                {
                    span = 365 * 5;
                }
                else
                {
                    throw new System.InvalidOperationException("1日のロウソク足タイプなら、一月以上");
                }

                //Debug.WriteLine("OneDay数" + test.Count.ToString());
            }
            else
            {
                throw new System.InvalidOperationException("Not impl...");
                //return;
            }

            //Debug.WriteLine("スパン：" + span.ToString());

            if (span == 0)
            {
                Debug.WriteLine("スパン 0.");
                return;
            }

            if (lst == null)
            {
                Debug.WriteLine("リスト Null.");
                return;
            }

            if (lst.Count < span - 1)
            {
                Debug.WriteLine("ロード中？");

                // TODO ロード中？
                return;
            }

            try
            {

                if (Application.Current == null) return;
                Application.Current.Dispatcher.Invoke(() =>
                {

                    // チャート OHLCVのロード
                    if (lst.Count > 0)
                    {
                        // Candlestickクリア
                        ChartSeries[0].Values.Clear();

                        // 出来高クリア
                        //ChartSeries[3].Values.Clear();
                        // https://github.com/Live-Charts/Live-Charts/issues/76
                        for (int v = 0; v < ChartSeries[3].Values.Count - 1; v++)
                        {
                            ChartSeries[3].Values[v] = (double)0;
                        }


                        // ラベル表示クリア
                        ChartAxisX[0].Labels.Clear();

                        // 期間設定
                        ChartAxisX[0].MaxValue = span - 1;
                        ChartAxisX[0].MinValue = 0;

                        // Temp を作って、後でまとめて追加する。
                        // https://lvcharts.net/App/examples/v1/wpf/Performance%20Tips

                        //var temporalCv = new OhlcPoint[test.Count];
                        var temporalCv = new OhlcPoint[span - 1];
                        //var temporalOV = new ObservableValue[span - 1];

                        var tempVol = new double[span - 1];

                        // チャート最低値、最高値の設定
                        double HighMax = 0;
                        double LowMax = 999999999;

                        int i = 0;
                        int c = span;
                        foreach (var oh in lst)
                        {
                            // 全てのポイントが同じ場合、スキップする。変なデータ？ 本家もスキップしている。
                            if ((oh.Open == oh.High) && (oh.Open == oh.Low) && (oh.Open == oh.Close) && (oh.Volume == 0))
                            {
                                // スキップすると、何かがおかしくなる。Spanと数が合わなくなる
                                //continue;
                            }

                            // 表示数を限る 直近のspan本
                            //if (i < (span - 1))
                            if (i < (span - 1))
                            {
                                // 最高値と最低値を探る
                                if ((double)oh.High > HighMax)
                                {
                                    HighMax = (double)oh.High;
                                }

                                if ((double)oh.Low < LowMax)
                                {
                                    LowMax = (double)oh.Low;
                                }

                                //Debug.WriteLine(oh.TimeStamp.ToString("dd日 hh時mm分"));

                                // ラベル
                                if (ct == CandleTypes.OneMin)
                                {
                                    ChartAxisX[0].Labels.Insert(0, oh.TimeStamp.ToString("H:mm"));
                                }
                                else if (ct == CandleTypes.OneHour)
                                {
                                    ChartAxisX[0].Labels.Insert(0, oh.TimeStamp.ToString("d日 H:mm"));

                                }
                                else if (ct == CandleTypes.OneDay)
                                {
                                    ChartAxisX[0].Labels.Insert(0, oh.TimeStamp.ToString("M月d日"));
                                }
                                else
                                {
                                    throw new System.InvalidOperationException("LoadChart: 不正な CandleType");
                                }
                                //ChartAxisX[0].Labels.Add(oh.TimeStamp.ToShortTimeString());


                                // ポイント作成
                                OhlcPoint p = new OhlcPoint((double)oh.Open, (double)oh.High, (double)oh.Low, (double)oh.Close);


                                // 直接追加しないで、
                                //ChartSeries[0].Values.Add(p);
                                // 一旦、Tempに追加して、あとでまとめてAddRange
                                temporalCv[c - 2] = p;


                                tempVol[c - 2] = (double)oh.Volume;
                                //ChartSeries[3].Values.Add((double)oh.Volume);

                                c = c - 1;

                            }

                            i = i + 1;
                        }

                        // チャート最低値、最高値のセット
                        ChartAxisY[0].MaxValue = HighMax;// + 1000;
                        ChartAxisY[0].MinValue = LowMax;// - 1000;

                        // まとめて追加

                        // OHLCV
                        ChartSeries[0].Values.AddRange(temporalCv);

                        // volume
                        var cv = new ChartValues<double>();
                        cv.AddRange(tempVol);
                        ChartSeries[3].Values = cv;


                        // TODO what is this? not working.
                        if (ChartAxisY[0].Sections.Count > 0)
                        {
                            //ChartAxisY[0].Sections[0].Width = span;
                            //ChartAxisY[0].Sections[0].SectionWidth = span;
                        }

                    }

                });

            }
            catch (Exception ex)
            {
                ChartLoadingInfo = "チャートのロード中にエラーが発生しました";

                Debug.WriteLine("■■■■■ Chart loading error: " + ex.ToString());
            }

            ChartLoadingInfo = "";

        }

        // 初回、データロードを確認して、チャートをロードする。
        private async void DisplayChart()
        {
            bool bln = await GetCandlesticks();

            if (bln == true)
            {
                LoadChart();
            }
        }

        // チャート表示期間を変えた時にチャートを読み込み直す。
        private void ChangeChartSpan()
        {
            // enum の期間選択からチャートを更新させる。コンボボックスとダブルアップデートにならないようにするため。

            if (_chartSpan == ChartSpans.OneHour)
            {
                if (SelectedCandleType != CandleTypes.OneMin)
                {
                    SelectedCandleType = CandleTypes.OneMin;
                }
                else
                {
                    LoadChart();
                }
            }
            else if (_chartSpan == ChartSpans.ThreeHour)
            {
                if (SelectedCandleType != CandleTypes.OneMin)
                {
                    SelectedCandleType = CandleTypes.OneMin;
                }
                else
                {
                    LoadChart();
                }
            }
            else if (_chartSpan == ChartSpans.OneDay)
            {
                if (SelectedCandleType != CandleTypes.OneHour)
                {
                    SelectedCandleType = CandleTypes.OneHour;
                }
                else
                {
                    LoadChart();
                }
            }
            else if (_chartSpan == ChartSpans.ThreeDay)
            {
                if (SelectedCandleType != CandleTypes.OneHour)
                {
                    SelectedCandleType = CandleTypes.OneHour;
                }
                else
                {
                    LoadChart();
                }
            }
            else if (_chartSpan == ChartSpans.OneWeek)
            {
                if (SelectedCandleType != CandleTypes.OneHour)
                {
                    SelectedCandleType = CandleTypes.OneHour;
                }
                else
                {
                    LoadChart();
                }
            }
            else if (_chartSpan == ChartSpans.OneMonth)
            {
                if (SelectedCandleType != CandleTypes.OneDay)
                {
                    SelectedCandleType = CandleTypes.OneDay;
                }
                else
                {
                    LoadChart();
                }
            }
            else if (_chartSpan == ChartSpans.TwoMonth)
            {
                if (SelectedCandleType != CandleTypes.OneDay)
                {
                    SelectedCandleType = CandleTypes.OneDay;
                }
                else
                {
                    LoadChart();
                }
            }
            else if (_chartSpan == ChartSpans.OneYear)
            {
                if (SelectedCandleType != CandleTypes.OneDay)
                {
                    SelectedCandleType = CandleTypes.OneDay;
                }
                else
                {
                    LoadChart();
                }
            }
            else if (_chartSpan == ChartSpans.FiveYear)
            {
                if (SelectedCandleType != CandleTypes.OneDay)
                {
                    SelectedCandleType = CandleTypes.OneDay;
                }
                else
                {
                    LoadChart();
                }
            }

        }

        // タイマーで、最新のロウソク足データを取得して追加する。
        private async void UpdateCandlestick()
        {
            //ChartLoadingInfo = "チャートデータの更新中....";

            // 今日の日付セット。UTCで。
            DateTime dtToday = DateTime.Now.ToUniversalTime();

            DateTime dtLastUpdate;

            #region == １分毎のデータ ==

            if (OhlcvsOneMin.Count > 0)
            {
                dtLastUpdate = OhlcvsOneMin[0].TimeStamp;

                //Debug.WriteLine(dtLastUpdate.ToString());

                List<Ohlcv> latestOneMin = new List<Ohlcv>();

                latestOneMin = await GetCandlestick(CandleTypes.OneMin, dtToday);

                if (latestOneMin != null)
                {
                    //latestOneMin.Reverse();

                    if (latestOneMin.Count > 0)
                    {
                        foreach(var hoge in latestOneMin)
                        {

                            // 全てのポイントが同じ場合、スキップする。変なデータ？ 本家もスキップしている。
                            if ((hoge.Open == hoge.High) && (hoge.Open == hoge.Low) && (hoge.Open == hoge.Close) && (hoge.Volume == 0))
                            {
                                // スキップすると、何かがおかしくなる。Spanと数が合わなくなる
                                //continue;
                            }

                            //Debug.WriteLine(hoge.TimeStamp.ToString()+" : "+ dtLastUpdate.ToString());

                            if (hoge.TimeStamp >= dtLastUpdate)
                            {

                                if (hoge.TimeStamp == dtLastUpdate)
                                {
                                    // 更新前の最後のポイントを更新する。最終データは中途半端なので。

                                    //Debug.WriteLine("１分毎のチャートデータ更新: " + hoge.TimeStamp.ToString());

                                    OhlcvsOneMin[0].Open = hoge.Open;
                                    OhlcvsOneMin[0].High = hoge.High;
                                    OhlcvsOneMin[0].Low = hoge.Low;
                                    OhlcvsOneMin[0].Close = hoge.Close;
                                    OhlcvsOneMin[0].TimeStamp = hoge.TimeStamp;

                                    UpdateLastCandle(CandleTypes.OneMin, hoge);

                                    //Debug.WriteLine(hoge.TimeStamp.ToString()+" : "+ dtLastUpdate.ToString());
                                }
                                else
                                {
                                    // 新規ポイントを追加する。

                                    //Debug.WriteLine("１分毎のチャートデータ追加: " + hoge.TimeStamp.ToString());

                                    OhlcvsOneMin.Insert(0, hoge);

                                    AddCandle(CandleTypes.OneMin, hoge);

                                    dtLastUpdate = hoge.TimeStamp;
                                }

                            }

                        }

                    }

                }

            }

            #endregion

            #region == １時間毎のデータ ==

            if (OhlcvsOneHour.Count > 0)
            {
                dtLastUpdate = OhlcvsOneHour[0].TimeStamp;

                TimeSpan ts = dtLastUpdate - dtToday;

                if (ts.TotalHours >= 1)
                {                
                    //Debug.WriteLine(dtLastUpdate.ToString());

                    List<Ohlcv> latestOneHour = new List<Ohlcv>();

                    latestOneHour = await GetCandlestick(CandleTypes.OneHour, dtToday);

                    if (latestOneHour != null)
                    {
                        //latestOneMin.Reverse();

                        if (latestOneHour.Count > 0)
                        {
                            foreach (var hoge in latestOneHour)
                            {

                                // 全てのポイントが同じ場合、スキップする。変なデータ？ 本家もスキップしている。
                                if ((hoge.Open == hoge.High) && (hoge.Open == hoge.Low) && (hoge.Open == hoge.Close) && (hoge.Volume == 0))
                                {
                                    // スキップすると、何かがおかしくなる。Spanと数が合わなくなる
                                    //continue;
                                }

                                //Debug.WriteLine(hoge.TimeStamp.ToString()+" : "+ dtLastUpdate.ToString());

                                if (hoge.TimeStamp >= dtLastUpdate)
                                {

                                    if (hoge.TimeStamp == dtLastUpdate)
                                    {
                                        // 更新前の最後のポイントを更新する。最終データは中途半端なので。

                                        Debug.WriteLine("１時間チャートデータ更新: " + hoge.TimeStamp.ToString());

                                        OhlcvsOneHour[0].Open = hoge.Open;
                                        OhlcvsOneHour[0].High = hoge.High;
                                        OhlcvsOneHour[0].Low = hoge.Low;
                                        OhlcvsOneHour[0].Close = hoge.Close;
                                        OhlcvsOneHour[0].TimeStamp = hoge.TimeStamp;

                                        UpdateLastCandle(CandleTypes.OneHour, hoge);

                                        //Debug.WriteLine(hoge.TimeStamp.ToString() + " : " + dtLastUpdate.ToString());
                                    }
                                    else
                                    {
                                        // 新規ポイントを追加する。

                                        Debug.WriteLine("１時間チャートデータ追加: " + hoge.TimeStamp.ToString());

                                        OhlcvsOneHour.Insert(0, hoge);

                                        AddCandle(CandleTypes.OneHour, hoge);

                                        dtLastUpdate = hoge.TimeStamp;
                                    }

                                }

                            }

                        }

                    }


                }

            }

            #endregion

            #region == １日毎のデータ ==

            if (OhlcvsOneDay.Count > 0)
            {
                dtLastUpdate = OhlcvsOneDay[0].TimeStamp;

                TimeSpan ts = dtLastUpdate - dtToday;

                if (ts.TotalDays >= 1)
                {
                    //Debug.WriteLine(dtLastUpdate.ToString());

                    List<Ohlcv> latestOneDay = new List<Ohlcv>();

                    latestOneDay = await GetCandlestick(CandleTypes.OneDay, dtToday);

                    if (latestOneDay != null)
                    {
                        //latestOneMin.Reverse();

                        if (latestOneDay.Count > 0)
                        {
                            foreach (var hoge in latestOneDay)
                            {

                                // 全てのポイントが同じ場合、スキップする。変なデータ？ 本家もスキップしている。
                                if ((hoge.Open == hoge.High) && (hoge.Open == hoge.Low) && (hoge.Open == hoge.Close) && (hoge.Volume == 0))
                                {
                                    // スキップすると、何かがおかしくなる。Spanと数が合わなくなる
                                    //continue;
                                }

                                //Debug.WriteLine(hoge.TimeStamp.ToString()+" : "+ dtLastUpdate.ToString());

                                if (hoge.TimeStamp >= dtLastUpdate)
                                {

                                    if (hoge.TimeStamp == dtLastUpdate)
                                    {
                                        // 更新前の最後のポイントを更新する。最終データは中途半端なので。

                                        Debug.WriteLine("１日チャートデータ更新: " + hoge.TimeStamp.ToString());

                                        OhlcvsOneDay[0].Open = hoge.Open;
                                        OhlcvsOneDay[0].High = hoge.High;
                                        OhlcvsOneDay[0].Low = hoge.Low;
                                        OhlcvsOneDay[0].Close = hoge.Close;
                                        OhlcvsOneDay[0].TimeStamp = hoge.TimeStamp;

                                        UpdateLastCandle(CandleTypes.OneDay, hoge);

                                        //Debug.WriteLine(hoge.TimeStamp.ToString() + " : " + dtLastUpdate.ToString());
                                    }
                                    else
                                    {
                                        // 新規ポイントを追加する。

                                        Debug.WriteLine("１日チャートデータ追加: " + hoge.TimeStamp.ToString());

                                        OhlcvsOneDay.Insert(0, hoge);

                                        AddCandle(CandleTypes.OneDay, hoge);

                                        dtLastUpdate = hoge.TimeStamp;
                                    }

                                }

                            }

                        }

                    }


                }

            }

            #endregion

        }

        // チャートの最後に最新ポイントを追加して更新表示する。
        private void AddCandle(CandleTypes ct, Ohlcv newData)
        {
            // 表示されているのだけ更新。それ以外は不要。
            if (SelectedCandleType != ct) return;

            //Debug.WriteLine("チャートの更新 追加: "+ newData.TimeStamp.ToString());

            if (ChartSeries[0].Values != null)
            {
                if (ChartSeries[0].Values.Count > 0)
                {
                    if (Application.Current == null) return;
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        // ポイント作成
                        OhlcPoint p = new OhlcPoint((double)newData.Open, (double)newData.High, (double)newData.Low, (double)newData.Close);
                        // 追加
                        ChartSeries[0].Values.Add(p);
                        // 一番古いの削除
                        ChartSeries[0].Values.RemoveAt(0);

                        // 出来高
                        ChartSeries[3].Values.Add((double)newData.Volume);
                        ChartSeries[3].Values.RemoveAt(0);

                        // ラベル
                        if (ct == CandleTypes.OneMin)
                        {
                            ChartAxisX[0].Labels.Add(newData.TimeStamp.ToString("HH:mm"));
                        }
                        else if (ct == CandleTypes.OneHour)
                        {
                            ChartAxisX[0].Labels.Add(newData.TimeStamp.ToString("dd日 HH:mm"));

                        }
                        else if (ct == CandleTypes.OneDay)
                        {
                            ChartAxisX[0].Labels.Add(newData.TimeStamp.ToString("MM月dd日"));
                        }
                        else
                        {
                            throw new System.InvalidOperationException("UpdateChart: 不正な CandleTypes");
                        }
                        ChartAxisX[0].Labels.RemoveAt(0);

                        // チャート最低値、最高値のセット
                        if (ChartAxisY[0].MaxValue < (double)newData.High)
                        {
                            ChartAxisY[0].MaxValue = (double)newData.High;
                        }

                        if (ChartAxisY[0].MinValue > (double)newData.Low)
                        {
                            ChartAxisY[0].MinValue = (double)newData.Low;
                        }


                    });

                }
            }

        }

        // チャートの最後のポイントを最新情報に更新表示する。
        private void UpdateLastCandle(CandleTypes ct, Ohlcv newData)
        {
            // 表示されているのだけ更新。それ以外は不要。
            if (SelectedCandleType != ct) return;

            //Debug.WriteLine("チャートの更新 追加: "+ newData.TimeStamp.ToString());

            if (ChartSeries[0].Values != null)
            {
                if (ChartSeries[0].Values.Count > 0)
                {

                    if (Application.Current == null) return;
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        ((OhlcPoint)ChartSeries[0].Values[ChartSeries[0].Values.Count - 1]).Open = (double)newData.Open;
                        ((OhlcPoint)ChartSeries[0].Values[ChartSeries[0].Values.Count - 1]).High = (double)newData.High;
                        ((OhlcPoint)ChartSeries[0].Values[ChartSeries[0].Values.Count - 1]).Low = (double)newData.Low;
                        ((OhlcPoint)ChartSeries[0].Values[ChartSeries[0].Values.Count - 1]).Close = (double)newData.Close;

                    });

                }
            }

        }

        #endregion

        #region == 注文関係のメソッド ==

        // 手動発注から発注。その他は、（priApi）から直に呼び出すこと！
        private async Task<OrderResult> ManualOrder(string pair, decimal amount, decimal price, string side, string type)
        {
            System.Diagnostics.Debug.WriteLine("□ Order - " + pair + ":" + amount + ":" + price.ToString() + ":" + side + ":" + type);

            if (ManualTradeApiKeyIsSet == false)
            {
                // TODO show message?
                Debug.WriteLine("(ManualTradeApiKeyIsSet == false)");

                return null;
            }

            try
            {
                //OrderResult ord = await _priMakeOrderApi.MakeOrder(pair, amount, price, side, type);
                OrderResult ord = await _priApi.MakeOrder(_manualTradeApiKey, _manualTradeSecret, pair, amount, price, side, type);

                if (ord != null)
                {
                    if (ord.IsSuccess)
                    {
                        // TODO
                        // 注文リストに追加する（Insert）
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            try
                            {
                                var found = _orders.FirstOrDefault(x => x.OrderID == ord.OrderID);
                                if (found == null)
                                {
                                    // 現在値のセット
                                    try
                                    {
                                        ord.Shushi = ((_ltp - ord.Price) * ord.StartAmount);
                                        ord.ActualPrice = (ord.Price * ord.StartAmount);
                                    }
                                    catch (Exception e)
                                    {
                                        System.Diagnostics.Debug.WriteLine("■■■■■ Order 現在値のセット2: " + e);
                                    }
                                    // リスト追加
                                    _orders.Insert(0, ord);
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine("■■■■■ Order: Exception - " + ex.Message);

                            }

                        });

                        return ord;
                    }
                    else
                    {
                        return ord;
                    }

                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ Order Exception: " + e);

                return null;
            }
        }

        // 注文キャンセル
        private async Task<OrderResult> CancelOrder(string pair, int orderID)
        {
            if (ManualTradeApiKeyIsSet == false)
            {
                // TODO show message?
                return null;
            }

            try
            {
                // キャンセル注文発注
                //OrderResult ord = await _priMakeOrderApi.CancelOrder(pair, orderID);
                OrderResult ord = await _priApi.CancelOrder(_manualTradeApiKey, _manualTradeSecret, pair, orderID);

                if (ord != null)
                {
                    if (ord.IsSuccess) {

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            try
                            {
                                // 注文リストの中から、同一IDをキャンセル注文結果と入れ替える
                                var found = _orders.FirstOrDefault(x => x.OrderID == orderID);
                                int i = _orders.IndexOf(found);
                                if (i > -1)
                                {
                                    _orders[i] = ord;
                                }
                                //or
                                //var found = theCollection.FirstOrDefault(x => x.Id == myId);
                                //theCollection.Remove(found);
                                //theCollection.Add(newObject);

                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrder: Exception - " + ex.Message);

                            }
                        });

                        return ord;
                    }
                    else
                    {
                        return ord;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrder Exception: " + e);

                return null;
            }

        }

        #endregion

        #region == （資産・板・歩み値・取引履歴）情報取得・更新系のメソッド ==

        // ループ再生開始メソッド
        private void StartLoop()
        {

            // 資産情報の更新ループ
            Task.Run(() => UpdateAssets());

            // 特殊注文リストの更新ループ
            Task.Run(() => UpdateIfdocos());

            // 板情報の更新ループ
            Task.Run(() => UpdateDepth());

            // 歩み値の更新ループ
            Task.Run(() => UpdateTransactions());

            // 取引履歴のGet
            Task.Run(() => UpdateTradeHistory());

            // 注文リストの更新ループ
            Task.Run(() => UpdateOrderList());

        }

        // 自動取引のループ
        private async void UpdateAutoTrade2()
        {
            // 試し買いの単位
            decimal sentinelAmount = 0.0001M;
            // 取引単位
            decimal defaultAmount = 0.001M;

            while (AutoTradeStart)
            {
                // ログインしていなかったらスルー。
                if (LoggedInMode == false)
                {
                    await Task.Delay(2000);
                    continue;
                }

                if (AutoTradeApiKeyIsSet == false)
                {
                    await Task.Delay(2000);
                    continue;
                }

                // 間隔 1/2
                await Task.Delay(1600);

                // 取引単位の変更があった場合に新しい値をセット
                defaultAmount = AutoTradeTama;

                AutoTradeActiveOrders = 0;
                AutoTradeSellOrders = 0;
                AutoTradeBuyOrders = 0;
                AutoTradeErrOrders = 0;

                if (Application.Current == null) break;

                // 未約定注文の情報更新用リスト
                List<int> needUpdateIdsList = new List<int>();

                // 全注文数のセット
                AutoTradeActiveOrders = _autoTrades2.Count;

                // 未約定注文のリストを作る。
                foreach (var pos in _autoTrades2)
                {
                    if (pos.IsCanceled)
                    {
                        continue;
                    }
                    if ((pos.BuyHasError == true) || (pos.SellHasError == true))
                    {
                        // エラー注文数をセット
                        AutoTradeErrOrders = AutoTradeErrOrders + 1;

                        continue;
                    }

                    if ((pos.BuyOrderId != 0) && ((pos.BuyStatus == "UNFILLED") || pos.BuyStatus == "PARTIALLY_FILLED"))
                    {
                        // 買い注文数をセット
                        AutoTradeBuyOrders = AutoTradeBuyOrders + 1;

                        needUpdateIdsList.Add(pos.BuyOrderId);
                    }

                    if ((pos.SellOrderId != 0) && ((pos.SellStatus == "UNFILLED") || pos.SellStatus == "PARTIALLY_FILLED"))
                    {
                        // 売り注文数をセット
                        AutoTradeSellOrders = AutoTradeSellOrders + 1;

                        needUpdateIdsList.Add(pos.SellOrderId);
                    }
                }

                // 未約定注文の最新状態をアップデートする。
                if (needUpdateIdsList.Count > 0)
                {
                    // リストのリスト（小分けにして分割取得用）
                    List<List<int>> ListOfList = new List<List<int>>();

                    // GetOrderListByIDs 40015 数が多いとエラーになるので、小分けにして。
                    List<int> temp = new List<int>();
                    int c = 0;

                    for (int i = 0; i < needUpdateIdsList.Count; i++)
                    {

                        temp.Add(needUpdateIdsList[c]);

                        if (temp.Count == 5)
                        {
                            ListOfList.Add(temp);

                            temp = new List<int>();
                        }

                        if (c == needUpdateIdsList.Count - 1)
                        {
                            if (temp.Count > 0)
                            {
                                ListOfList.Add(temp);
                            }

                            break;
                        }

                        c = c + 1;
                    }

                    foreach (var list in ListOfList)
                    {
                        // 最新の注文情報をゲット
                        Orders ords = await _priApi.GetOrderListByIDs(_autoTradeApiKey, _autoTradeSecret, "btc_jpy", list);//needUpdateIdsList);

                        if (ords != null)
                        {
                            if (Application.Current == null)
                            {
                                break;
                            }
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                // 更新された注文をアップデート
                                foreach (var ord in ords.OrderList)
                                {
                                    try
                                    {
                                        // 買い注文
                                        var found = _autoTrades2.FirstOrDefault((x => x.BuyOrderId == ord.OrderID));
                                        if (found != null)
                                        {

                                            found.BuyStatus = ord.Status;

                                            found.BuyFilledPrice = ord.AveragePrice;
                                        }

                                        // 売り注文
                                        found = _autoTrades2.FirstOrDefault((x => x.SellOrderId == ord.OrderID));
                                        if (found != null)
                                        {
                                            found.SellStatus = ord.Status;

                                            found.SellFilledPrice = ord.AveragePrice;
                                        }

                                    }
                                    catch (Exception e)
                                    {
                                        System.Diagnostics.Debug.WriteLine("■■■■■ AutoTrade2 GetOrderListByIDs Exception: " + e);
                                    }

                                }
                            });
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("■■■■■ AutoTrade2 GetOrderListByIDs ords == null");


                        }
                    }

                }

                // 売り未発注を発注するループ。
                foreach (var pos in _autoTrades2)
                {
                    // キャンセルはスキップ
                    if (pos.IsCanceled) continue;

                    // 済みはスキップ
                    if (pos.IsDone) continue;

                    // 買いキャンセルされた スキップ
                    if ((pos.BuyOrderId != 0) && ((pos.BuyStatus == "CANCELED_UNFILLED") || pos.BuyStatus == "CANCELED_PARTIALLY_FILLED"))
                    {
                        pos.IsCanceled = true;
                        continue;
                    }

                    // 売りキャンセルされた スキップ
                    if ((pos.SellOrderId != 0) && ((pos.SellStatus == "CANCELED_UNFILLED") || pos.SellStatus == "CANCELED_PARTIALLY_FILLED"))
                    {
                        pos.IsCanceled = true;
                        continue;
                    }

                    // TODO HasError スキップ
                    if ((pos.BuyHasError == true) || (pos.SellHasError == true))
                    {
                        continue;
                    }

                    // 買い約定済み
                    if ((pos.BuyOrderId != 0) && (pos.BuyStatus == "FULLY_FILLED"))
                    {
                        pos.BuyIsDone = true;

                        // 売り未発注 (エラーが起きている売りはどうする？)
                        if ((pos.SellOrderId == 0) && (pos.SellHasError == false) && (string.IsNullOrEmpty(pos.SellStatus)))
                        {

                            await Task.Delay(200);

                            // 売り発注
                            OrderResult ord = await _priApi.MakeOrder(_autoTradeApiKey, _autoTradeSecret, "btc_jpy", pos.SellAmount, pos.SellPrice, pos.SellSide, "limit");

                            if (ord != null)
                            {
                                // 売り成功
                                if (ord.IsSuccess)
                                {
                                    pos.SellHasError = false;

                                    pos.SellOrderId = ord.OrderID;
                                    pos.SellStatus = ord.Status;

                                }
                                else
                                {
                                    pos.SellHasError = true;
                                    if (pos.SellErrorInfo == null)
                                    {
                                        pos.SellErrorInfo = new ErrorInfo();
                                    }
                                    pos.SellErrorInfo.ErrorTitle = ord.Err.ErrorTitle;
                                    pos.SellErrorInfo.ErrorDescription = ord.Err.ErrorDescription;
                                    pos.SellErrorInfo.ErrorCode = ord.Err.ErrorCode;

                                    System.Diagnostics.Debug.WriteLine("UpdateAutoTrade2 - 売り　sell MakeOrder API failed");
                                }

                            }
                            else
                            {
                                pos.SellHasError = true;
                                if (pos.SellErrorInfo == null)
                                {
                                    pos.SellErrorInfo = new ErrorInfo();
                                }
                                pos.SellErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                                pos.SellErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                                pos.SellErrorInfo.ErrorCode = -1;

                                System.Diagnostics.Debug.WriteLine("UpdateAutoTrade 2- 売り　sell MakeOrder returened NULL");
                            }

                            await Task.Delay(200);

                        }
                        else
                        {
                            // 売り注文は済みの場合は、フラグ
                            if ((pos.SellOrderId != 0) && (pos.SellStatus == "FULLY_FILLED"))
                            {
                                pos.SellIsDone = true;
                                pos.IsDone = true;
                            }

                        }


                    }

                }

                // 完了済み注文を、新たに注文し直す。 ただし、アッパーリミット以下なら（暴騰時に買うと、リバウンドですぐ下がる）
                if ((_ltp < AutoTradeUpperLimit) && (_ltp > AutoTradeLowerLimit))
                {

                    // 買い価格順でソートしたリスト
                    List<AutoTrade2> SortedList = _autoTrades2.OrderByDescending(o => o.BuyPrice).ToList();

                    // 完了済み注文を、新たに注文し直すループ。
                    for (int i = 0; i < SortedList.Count; i++)
                    {

                        if (SortedList[i].IsCanceled)
                        {
                            SortedList[i].Counter = 0;
                            continue;
                        }
                        if ((SortedList[i].BuyIsDone == true) && (SortedList[i].SellIsDone == false))
                        {
                            SortedList[i].Counter = 0;
                            continue;
                        }

                        // TODO HasError スキップ
                        if ((SortedList[i].BuyHasError == true) || (SortedList[i].SellHasError == true))
                        {
                            SortedList[i].Counter = 0;
                            continue;
                        }

                        // 一番上 未約定
                        if ((i == 0) && ((SortedList[i].BuyIsDone == false) && (SortedList[i].SellIsDone == false)))
                        {

                            // SortedList[i].Counter == 20 とかだったら、チョイ上の+1000価格で、買い注文。ただし、現在値より下の価格で買う。
                            if (SortedList[i].Counter >= 7)
                            {
                                if ((_ltp - SortedList[i].BuyPrice) > 500M)
                                {

                                    decimal basePrice = _ltp - 200M;
                                    //basePrice = ((_ltp / 1000) * 1000);

                                    if ((basePrice < _ltp) && (SortedList[i].BuyPrice < basePrice))
                                    {

                                        AutoTrade2 position = new AutoTrade2();
                                        position.BuySide = "buy";
                                        position.BuyAmount = sentinelAmount;//SortedList[i].BuyAmount;
                                        position.SellSide = "sell";
                                        position.SellAmount = sentinelAmount;//SortedList[i].SellAmount;


                                        position.BuyPrice = basePrice;
                                        position.SellPrice = basePrice + 500M;

                                        position.ShushiAmount = (position.SellPrice * position.SellAmount) - (position.BuyPrice * position.BuyAmount);

                                        // 注文発注
                                        OrderResult res = await _priApi.MakeOrder(_autoTradeApiKey, _autoTradeSecret, "btc_jpy", position.BuyAmount, position.BuyPrice, position.BuySide, "limit");

                                        if (res != null)
                                        {
                                            if (res.IsSuccess)
                                            {

                                                position.BuyHasError = false;

                                                position.BuyOrderId = res.OrderID;
                                                position.BuyFilledPrice = res.AveragePrice;
                                                position.BuyStatus = res.Status;

                                                // 約定
                                                if (res.Status == "FULLY_FILLED")
                                                {
                                                    // フラグをセット
                                                    position.BuyIsDone = true;
                                                }


                                                // 暴露カウンターを0にリセット
                                                SortedList[i].Counter = 0;


                                                // 新規追加
                                                _autoTrades2.Insert(0, position);

                                                await Task.Delay(200);

                                                if (_autoTrades2.Count > 5)
                                                {
                                                    // チョット下あたりの注文を抜く。

                                                    if (_autoTrades2[3].BuyIsDone == false)
                                                    {

                                                        OrderResult ord = await _priApi.CancelOrder(_autoTradeApiKey, _autoTradeSecret, "btc_jpy", _autoTrades2[3].BuyOrderId);
                                                        if (ord != null)
                                                        {
                                                            if (ord.IsSuccess)
                                                            {
                                                                _autoTrades2.RemoveAt(3);

                                                            }
                                                            else
                                                            {
                                                                _autoTrades2[3].BuyHasError = true;
                                                                if (_autoTrades2[3].BuyErrorInfo == null)
                                                                {
                                                                    _autoTrades2[3].BuyErrorInfo = new ErrorInfo();
                                                                }
                                                                _autoTrades2[3].BuyErrorInfo.ErrorTitle = ord.Err.ErrorTitle;
                                                                _autoTrades2[3].BuyErrorInfo.ErrorDescription = ord.Err.ErrorDescription;
                                                                _autoTrades2[3].BuyErrorInfo.ErrorCode = ord.Err.ErrorCode;

                                                                System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade2 一番上 途中を抜くキャンセルで。MakeOrder API returned error code.");
                                                            }

                                                        }
                                                        else
                                                        {
                                                            _autoTrades2[3].BuyHasError = true;
                                                            if (_autoTrades2[3].BuyErrorInfo == null)
                                                            {
                                                                _autoTrades2[3].BuyErrorInfo = new ErrorInfo();
                                                            }
                                                            _autoTrades2[3].BuyErrorInfo.ErrorTitle = ord.Err.ErrorTitle;
                                                            _autoTrades2[3].BuyErrorInfo.ErrorDescription = ord.Err.ErrorDescription;
                                                            _autoTrades2[3].BuyErrorInfo.ErrorCode = ord.Err.ErrorCode;

                                                            System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade2 一番上 途中を抜くキャンセルで。MakeOrder API returned null.");
                                                        }
                                                    }
                                                }

                                                // ループ中にリストを弄ったから、ループから抜ける。
                                                break;


                                            }
                                            else
                                            {
                                                position.BuyHasError = true;
                                                if (position.BuyErrorInfo == null)
                                                {
                                                    position.BuyErrorInfo = new ErrorInfo();
                                                }
                                                position.BuyErrorInfo.ErrorTitle = res.Err.ErrorTitle;
                                                position.BuyErrorInfo.ErrorDescription = res.Err.ErrorDescription;
                                                position.BuyErrorInfo.ErrorCode = res.Err.ErrorCode;

                                                System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade2 完了済み注文を、新たに注文し直すループ。一番上 MakeOrder API returned error code.");
                                            }
                                        }
                                        else
                                        {
                                            position.BuyHasError = true;
                                            if (position.BuyErrorInfo == null)
                                            {
                                                position.BuyErrorInfo = new ErrorInfo();
                                            }
                                            position.BuyErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                                            position.BuyErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                                            position.BuyErrorInfo.ErrorCode = -1;

                                            System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade2 完了済み注文を、新たに注文し直すループ。一番上 MakeOrder API returned null.");

                                        }



                                    }


                                }

                            }

                        }

                        // 売りが約定した
                        if ((SortedList[i].IsDone) && (SortedList[i].BuyHasError == false) && (SortedList[i].SellHasError == false) && (SortedList[i].IsCanceled == false))
                        {

                            // 損益更新
                            //AutoTradeProfit = AutoTradeProfit + ((SortedList[i].SellFilledPrice - SortedList[i].BuyFilledPrice) * SortedList[i].SellAmount);
                            AutoTradeProfit = AutoTradeProfit + ((SortedList[i].SellPrice - SortedList[i].BuyPrice) * SortedList[i].SellAmount);

                            // 新規
                            AutoTrade2 position = new AutoTrade2();
                            position.BuySide = "buy";
                            position.BuyAmount = defaultAmount;//SortedList[i].BuyAmount;
                            position.SellSide = "sell";
                            position.SellAmount = defaultAmount;//SortedList[i].SellAmount;

                            // 上半分
                            if (i <= 7)
                            {
                                // 同じ条件で、再度注文。ただし、現在値より下の価格で買う。

                                position.BuyPrice = SortedList[i].BuyPrice - 200;
                                position.SellPrice = SortedList[i].SellPrice - 200;

                                if (position.BuyPrice > _ltp)
                                {
                                    /*
                                    decimal basePrice = ((_ltp / 1000) * 1000);
                                    position.BuyPrice = basePrice - 500M;
                                    position.SellPrice = basePrice + 500M;
                                    */
                                    position.BuyPrice = _ltp - 500M;
                                    position.SellPrice = _ltp + 500M;
                                }
                            }
                            else
                            {
                                // マイナス買い価格で、再度注文。ただし、現在値より下の価格で買う。

                                position.BuyPrice = SortedList[i].BuyPrice - 1000M;
                                position.SellPrice = SortedList[i].SellPrice - 1000M;

                                if (position.BuyPrice > _ltp)
                                {
                                    /*
                                    decimal basePrice = ((_ltp / 1000) * 1000);
                                    position.BuyPrice = basePrice - 1000M;
                                    position.SellPrice = basePrice;
                                    */
                                    position.BuyPrice = _ltp - 1500M;
                                    position.SellPrice = _ltp + 500M;
                                }

                            }

                            // 予想利益額
                            position.ShushiAmount = (position.SellPrice * position.SellAmount) - (position.BuyPrice * position.BuyAmount);


                            // 注文発注
                            OrderResult res = await _priApi.MakeOrder(_autoTradeApiKey, _autoTradeSecret, "btc_jpy", position.BuyAmount, position.BuyPrice, position.BuySide, "limit");

                            if (res != null)
                            {
                                if (res.IsSuccess)
                                {

                                    position.BuyHasError = false;

                                    position.BuyOrderId = res.OrderID;
                                    position.BuyFilledPrice = res.AveragePrice;
                                    position.BuyStatus = res.Status;

                                    // 約定
                                    if (res.Status == "FULLY_FILLED")
                                    {
                                        // フラグをセット
                                        position.BuyIsDone = true;

                                    }

                                    // 下のカウンターをリセット
                                    if (i < _autoTrades2.Count - 1)
                                    {
                                        _autoTrades2[i + 1].Counter = 0;
                                    }


                                    // IsDone を削除する。>あとで。
                                    //_autoTrades2.RemoveAt(i);

                                    // 新規追加
                                    _autoTrades2.Insert(i, position);

                                    await Task.Delay(300);


                                }
                                else
                                {
                                    position.BuyHasError = true;
                                    if (position.BuyErrorInfo == null)
                                    {
                                        position.BuyErrorInfo = new ErrorInfo();
                                    }
                                    position.BuyErrorInfo.ErrorTitle = res.Err.ErrorTitle;
                                    position.BuyErrorInfo.ErrorDescription = res.Err.ErrorDescription;
                                    position.BuyErrorInfo.ErrorCode = res.Err.ErrorCode;

                                    System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade2 完了済み注文を、新たに注文し直すループ。途中 MakeOrder API returned error code.");
                                }
                            }
                            else
                            {
                                position.BuyHasError = true;
                                if (position.BuyErrorInfo == null)
                                {
                                    position.BuyErrorInfo = new ErrorInfo();
                                }
                                position.BuyErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                                position.BuyErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                                position.BuyErrorInfo.ErrorCode = -1;

                                System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade2 完了済み注文を、新たに注文し直すループ。途中 MakeOrder API returned null.");

                            }


                            // ループ中にリストを弄ったから、ループから抜ける。
                            break;
                        }

                        // SortedList[i].Counter >= 20 とかだったら、停滞中なので、売り待ちの価格より下で、、かつ、現在値よりチョイ下で、買い注文。ただし、全体の数が20を超えない範囲で。
                        if ((SortedList[i].Counter >= 7) && SortedList.Count < 20)
                        {
                            await Task.Delay(200);

                            if ((_ltp - SortedList[i].BuyPrice) > 600M)
                            {

                                // 現在値よりチョイ下の価格で買う。
                                decimal basePrice = _ltp - 400M;

                                if ((basePrice < _ltp) && (basePrice > SortedList[i].BuyPrice))
                                {
                                    AutoTrade2 position = new AutoTrade2();
                                    position.BuySide = "buy";
                                    position.BuyAmount = defaultAmount;//SortedList[i].BuyAmount;
                                    position.SellSide = "sell";
                                    position.SellAmount = defaultAmount;//SortedList[i].SellAmount;

                                    position.BuyPrice = basePrice;
                                    position.SellPrice = basePrice + 600M;

                                    position.ShushiAmount = (position.SellPrice * position.SellAmount) - (position.BuyPrice * position.BuyAmount);


                                    // 注文発注
                                    OrderResult res = await _priApi.MakeOrder(_autoTradeApiKey, _autoTradeSecret, "btc_jpy", position.BuyAmount, position.BuyPrice, position.BuySide, "limit");

                                    if (res != null)
                                    {
                                        if (res.IsSuccess)
                                        {

                                            position.BuyHasError = false;

                                            position.BuyOrderId = res.OrderID;
                                            position.BuyFilledPrice = res.AveragePrice;
                                            position.BuyStatus = res.Status;

                                            // 約定
                                            if (res.Status == "FULLY_FILLED")
                                            {
                                                // フラグをセット
                                                position.BuyIsDone = true;

                                            }


                                            // カウンターをリセット
                                            _autoTrades2[i].Counter = 0;

                                            // 下のカウンターをリセット
                                            if (i < _autoTrades2.Count - 1)
                                            {
                                                _autoTrades2[i + 1].Counter = 0;
                                            }

                                            await Task.Delay(200);

                                            // 新規追加
                                            _autoTrades2.Insert(i, position);



                                            // ループ中にリストを弄ったから、ループから抜ける。
                                            break;

                                        }
                                        else
                                        {
                                            position.BuyHasError = true;
                                            if (position.BuyErrorInfo == null)
                                            {
                                                position.BuyErrorInfo = new ErrorInfo();
                                            }
                                            position.BuyErrorInfo.ErrorTitle = res.Err.ErrorTitle;
                                            position.BuyErrorInfo.ErrorDescription = res.Err.ErrorDescription;
                                            position.BuyErrorInfo.ErrorCode = res.Err.ErrorCode;

                                            System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade2 完了済み注文を、新たに注文し直すループ。途中 MakeOrder API returned error code.");
                                        }
                                    }
                                    else
                                    {
                                        position.BuyHasError = true;
                                        if (position.BuyErrorInfo == null)
                                        {
                                            position.BuyErrorInfo = new ErrorInfo();
                                        }
                                        position.BuyErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                                        position.BuyErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                                        position.BuyErrorInfo.ErrorCode = -1;

                                        System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade2 完了済み注文を、新たに注文し直すループ。途中 MakeOrder API returned null.");

                                    }



                                }
                            }
                        }

                        // 暴露カウンターを＋
                        SortedList[i].Counter = SortedList[i].Counter + 1;

                        // 一番高い買い注文に到達したので、ループから抜ける。
                        break;

                    }

                }

                await Task.Delay(300);

                // 掃除ループ
                int h = -1;
                foreach (var pos in _autoTrades2)
                {
                    h = h + 1;

                    if (pos.IsCanceled == true)
                    {
                        //needDeleteIdsList.Add(pos.BuyOrderId);
                        _autoTrades2.Remove(pos);
                        break;
                    }

                    if (pos.IsDone == true)
                    {
                        //AutoTradeProfit = AutoTradeProfit + ((pos.SellFilledPrice - pos.BuyFilledPrice) * pos.SellAmount);
                        _autoTrades2.Remove(pos);
                        break;
                    }

                    if (pos.BuyHasError == true)
                    {
                        //_autoTrades2.Remove(pos);
                        //break;


                        // 停滞タイムアウトでキャンセルしようとして、50010（指定の注文はキャンセル出来ませんエラー）が返ると、エラーが増え続ける。
                        // ので、スキップ
                        continue;
                    }
                    else if (pos.SellHasError == true)
                    {
                        //_autoTrades2.Remove(pos);
                        //break;

                        continue;
                    }


                    // 停滞中を抜く。
                    // TODO (一部約定している場合は、キャンセルすると端数が出る。それどうするか)
                    if ((pos.BuyStatus == "UNFILLED" || pos.BuyStatus == "PARTIALLY_FILLED") && pos.BuyHasError == false)
                    {
                        // 先頭でもなく、一番下でもない
                        if ((h > 0) && (h < 15))
                        {
                            // 60秒以上停滞している。
                            if (pos.Counter > 60)
                            {
                                // 注文を抜く。
                                OrderResult ord = await _priApi.CancelOrder(_autoTradeApiKey, _autoTradeSecret, "btc_jpy", pos.BuyOrderId);
                                if (ord != null)
                                {
                                    if (ord.IsSuccess)
                                    {
                                        _autoTrades2.Remove(pos);

                                    }
                                    else
                                    {
                                        pos.BuyHasError = true;
                                        if (pos.BuyErrorInfo == null)
                                        {
                                            pos.BuyErrorInfo = new ErrorInfo();
                                        }
                                        pos.BuyErrorInfo.ErrorTitle = ord.Err.ErrorTitle;
                                        pos.BuyErrorInfo.ErrorDescription = ord.Err.ErrorDescription;
                                        pos.BuyErrorInfo.ErrorCode = ord.Err.ErrorCode;

                                        System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade2 停滞中を抜く。キャンセルで。MakeOrder API returned error code.");
                                    }

                                }
                                else
                                {
                                    pos.BuyHasError = true;
                                    if (pos.BuyErrorInfo == null)
                                    {
                                        pos.BuyErrorInfo = new ErrorInfo();
                                    }
                                    pos.BuyErrorInfo.ErrorTitle = ord.Err.ErrorTitle;
                                    pos.BuyErrorInfo.ErrorDescription = ord.Err.ErrorDescription;
                                    pos.BuyErrorInfo.ErrorCode = ord.Err.ErrorCode;

                                    System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade2 停滞中を抜く。キャンセルで。MakeOrder API returned null.");
                                }

                                break;

                            }




                        }


                    }


                    // 指定幅　下がったら、損切。
                    if ((pos.SellOrderId != 0) && pos.BuyIsDone && (pos.SellIsDone == false))
                    {
                        if (pos.SellStatus == "UNFILLED")
                        {
                            if ((pos.SellPrice - Ltp) > AutoTradeLostCut)
                            {


                                OrderResult ord = await _priApi.CancelOrder(_autoTradeApiKey, _autoTradeSecret, "btc_jpy", pos.SellOrderId);
                                if (ord != null)
                                {
                                    if (ord.IsSuccess)
                                    {
                                        pos.SellStatus = ord.Status;

                                        pos.IsCanceled = true;
                                    }
                                    else
                                    {
                                        pos.SellHasError = true;
                                        if (pos.SellErrorInfo == null)
                                        {
                                            pos.SellErrorInfo = new ErrorInfo();
                                        }
                                        pos.SellErrorInfo.ErrorTitle = ord.Err.ErrorTitle;
                                        pos.SellErrorInfo.ErrorDescription = ord.Err.ErrorDescription;
                                        pos.SellErrorInfo.ErrorCode = ord.Err.ErrorCode;

                                        System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade2 損切売りキャンセルで。MakeOrder API returned error code.");
                                    }

                                }
                                else
                                {
                                    pos.SellHasError = true;
                                    if (pos.SellErrorInfo == null)
                                    {
                                        pos.SellErrorInfo = new ErrorInfo();
                                    }
                                    pos.SellErrorInfo.ErrorTitle = ord.Err.ErrorTitle;
                                    pos.SellErrorInfo.ErrorDescription = ord.Err.ErrorDescription;
                                    pos.SellErrorInfo.ErrorCode = ord.Err.ErrorCode;

                                    System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade2 損切売りキャンセルで。MakeOrder API returned null.");
                                }




                            }
                        }
                    }




                }


            }
        }

        // 特殊注文の更新ループ
        private async void UpdateIfdocos()
        {
            // 未約定IDリスト
            List<int> unfilledOrderIDsList = new List<int>();
            // 要発注リスト
            List<Ifdoco> needToOrderList = new List<Ifdoco>();
            // アクティブな注文数カウンター
            int actOrd = 0;

            while (true)
            {
                // 間隔 1/2
                await Task.Delay(1600);

                // ログインしていなかったらスルー。
                if (LoggedInMode == false)
                {
                    await Task.Delay(2000);
                    continue;
                }
                // APIキーがセットされていない場合スルー
                if (IfdocoTradeApiKeyIsSet == false)
                {
                    // TODO show message?
                    await Task.Delay(2000);
                    continue;
                }

                unfilledOrderIDsList.Clear();
                needToOrderList.Clear();
                actOrd = 0;

                // リストをループして、発注が必要なのを 要発注リストに追加。
                // 未約定のを未約定IDリストに追加して、後でアップデート。
                if (Application.Current == null) break;
                Application.Current.Dispatcher.Invoke(() =>
                {

                    foreach (var ifdoco in _ifdocos)
                    {
                        // IFD
                        if (ifdoco.Kind == IfdocoKinds.ifd)
                        {
                            if (ifdoco.IfdIsDone == false)
                            {
                                //if (ifdoco.IfdoneIsDone == false)
                                //{

                                    if (ifdoco.IfdoneStatus == "FULLY_FILLED")
                                    {
                                        // 約定！  Ifd done.

                                        // 済みフラグをセット
                                        ifdoco.IfdoneIsDone = true;


                                    }
                                    else if (ifdoco.IfdoneStatus == "CANCELED_UNFILLED" || ifdoco.IfdoneStatus == "CANCELED_PARTIALLY_FILLED")
                                    {
                                    // キャンセル済み

                                    // ステータス情報等、更新。???
                                    // TODO

                                    // 済みフラグをセット(これいる？)

                                        ifdoco.IfdDoIsDone = true; // ifdDoを発注させないように、キャンセルする。

                                        ifdoco.IfdoneIsDone = true;

                                    }
                                    else
                                    {
                                        // まだ未約定

                                        // 更新リストに追加して後でアップデートする。
                                        // Add to ToBeUpdated List and Update Order info lator.
                                        if (ifdoco.IfdoneOrderID != 0)
                                        {
                                            //Debug.WriteLine("■IFD  unfilledOrderIDsList: " + ifdoco.IfdoneOrderID.ToString());

                                            unfilledOrderIDsList.Add(ifdoco.IfdoneOrderID);
                                        }
                                        else
                                        {
                                            //Debug.WriteLine("■IFD  unfilledOrderIDsList: " + ifdoco.IfdoneOrderID.ToString());

                                        }

                                        if (string.IsNullOrEmpty(ifdoco.IfdoneStatus) == false)
                                        {
                                            // アクティブな注文
                                            actOrd = actOrd + 1;

                                        }

                                    }

                                //}
                                //else
                                //{
                                    if (ifdoco.IfdDoIsDone == false)
                                    {
                                        
                                        if (ifdoco.IfdDoStatus == "FULLY_FILLED")
                                        {
                                            // 約定！  Ifd do.

                                            // IFD 注文全部終了タイミング！(色を変える？)


                                            // 済みフラグをセット(これいる？)
                                            ifdoco.IfdDoIsDone = true;

                                            // IfdIsDone フラグ！(IFD注文完了！)
                                            ifdoco.IfdIsDone = true;

                                        }
                                        else if (ifdoco.IfdDoStatus == "CANCELED_UNFILLED" || ifdoco.IfdDoStatus == "CANCELED_PARTIALLY_FILLED")
                                        {
                                            // キャンセル済み

                                            // ステータス情報等、更新。???
                                            // TODO

                                            // 済みフラグをセット(これいる？)
                                            ifdoco.IfdDoIsDone = true;

                                            // IfdIsDone フラグ！(IFD注文完了！)
                                            ifdoco.IfdIsDone = true;

                                        }
                                        else
                                        {

                                            // 更新リストに追加して後でアップデートする。
                                            // Add to ToBeUpdated List and Update Order info lator.

                                            if (ifdoco.IfdDoOrderID != 0)
                                            {
                                                // まだ未約定

                                                unfilledOrderIDsList.Add(ifdoco.IfdDoOrderID);
                                            }
                                            else
                                            {

                                                // 要発注 IfdoneDo

                                                needToOrderList.Add(ifdoco);

                                            }
                                                
                                            if (string.IsNullOrEmpty(ifdoco.IfdDoStatus) == false)
                                            {
                                                // アクティブな注文
                                                actOrd = actOrd + 1;

                                            }

                                        }

                                    }
                                    else
                                    {
                                        // Ifd は済んでるよ！
                                        ifdoco.IfdIsDone = true;
                                    }
                                //}

                            }
                        }
                        // OCO
                        else if (ifdoco.Kind == IfdocoKinds.oco)
                        {

                            if (ifdoco.OcoIsDone == false)
                            {
                                if (ifdoco.OcoOneIsDone && ifdoco.OcoOtherIsDone)
                                {
                                    // タイミングと価格によっては両方同時に約定で返ってくる可能性。


                                    // OcoIsDone フラグ
                                    ifdoco.OcoIsDone = true;

                                }
                                else if (ifdoco.OcoOneIsDone || ifdoco.OcoOtherIsDone)
                                {
                                    // 片方約定

                                    // どちらかをキャンセル
                                    // 要発注 IfdoneDo

                                    //TODO
                                    needToOrderList.Add(ifdoco);


                                    // OcoIsDone フラグ
                                    //ifdoco.OcoIsDone = true;

                                }
                                else
                                {
                                    // どちらも未約定

                                    // 更新リストに追加して後でアップデートする。
                                    // Add to ToBeUpdated List and Update Order info lator.
                                    if (ifdoco.OcoOneOrderID != 0)
                                    {
                                        unfilledOrderIDsList.Add(ifdoco.OcoOneOrderID);
                                    }
                                    else
                                    {
                                        // 注文IDが０。発注してない。
                                        needToOrderList.Add(ifdoco);

                                    }
                                    if (ifdoco.OcoOtherOrderID != 0)
                                    {
                                        unfilledOrderIDsList.Add(ifdoco.OcoOtherOrderID);
                                    }
                                    else
                                    {
                                        // 注文IDが０。発注してない。
                                        needToOrderList.Add(ifdoco);
                                    }

                                    // アクティブな注文
                                    actOrd = actOrd + 1;


                                }
                            }

                        }
                        // IFDOCO
                        else if (ifdoco.Kind == IfdocoKinds.ifdoco)
                        {
                            
                            if (ifdoco.IfdocoIsDone == false)
                            {

                                // ifd がまだ
                                //if (ifdoco.IfdoneIsDone == false)
                                //{
                                    
                                    if (ifdoco.IfdoneStatus == "FULLY_FILLED")
                                    {
                                        // 約定！  Ifd done.

                                        // 発注 OCO 二つ
                                        //TODO

                                        // 発注済みを更新リストに追加して後でアップデートする。
                                        // Add to ToBeUpdated List and Update Order info lator.


                                        // 済みフラグをセット
                                        ifdoco.IfdoneIsDone = true;

                                        //Debug.WriteLine("■IFDOCO ifdoco : 要発注。");

                                        // 要発注 IfdoneDo
                                        needToOrderList.Add(ifdoco);

                                    }
                                    else if (ifdoco.IfdoneStatus == "CANCELED_UNFILLED" || ifdoco.IfdoneStatus == "CANCELED_PARTIALLY_FILLED")
                                    {
                                        // キャンセル済み

                                        // ステータス情報等、更新。???
                                        // TODO

                                        // 済みフラグをセット(これいる？)
                                        ifdoco.IfdoneIsDone = true;

                                        //TODO
                                        ifdoco.IfdocoIsDone = true;

                                    }
                                    else
                                    {
                                        // まだ未約定

                                        // 更新リストに追加して後でアップデートする。
                                        if (ifdoco.IfdoneOrderID != 0)
                                        {
                                            unfilledOrderIDsList.Add(ifdoco.IfdoneOrderID);
                                        }
                                        else
                                        {
                                            // 注文IDが0。　本来リストビューに登録されるべきでない。
                                            //Debug.WriteLine("■IFDOCO ifd unfilledOrderIDsList: 注文IDが０。" + ifdoco.IfdoneOrderID.ToString());
                                        }


                                        // TODO:
                                        // if HasError!!!

                                        /*
                                        // TEMP
                                        ifdoco.IfdoneHasError = true;
                                        if (ifdoco.IfdoneErrorInfo == null)
                                        {
                                            ifdoco.IfdoneErrorInfo = new ErrorInfo();
                                        }
                                        ifdoco.IfdoneErrorInfo.ErrorTitle = "IfdoneOrderID == 0";
                                        ifdoco.IfdoneErrorInfo.ErrorDescription = "asdfasdf sadf asdf asdf";
                                        */

                                        if (string.IsNullOrEmpty(ifdoco.IfdoneStatus) == false)
                                        {
                                            // アクティブな注文
                                            actOrd = actOrd + 1;

                                        }

                                    }

                                //}
                                //else
                                //{

                                // Ifd done の oco

                                //ifdは済み

                                if (ifdoco.OcoIsDone == false)
                                    {

                                        if (ifdoco.OcoOneIsDone && ifdoco.OcoOtherIsDone)
                                        {
                                            // タイミングと価格によっては両方同時に約定で返ってくる可能性がある。。



                                            // OcoIsDone フラグをセット
                                            ifdoco.OcoIsDone = true;

                                        }
                                        else if (ifdoco.OcoOneIsDone || ifdoco.OcoOtherIsDone)
                                        {
                                            // 片方約定

                                            // どちらかをキャンセル

                                            needToOrderList.Add(ifdoco);


                                            // OcoIsDone フラグをセット
                                            //ifdoco.OcoIsDone = true;


                                            // アクティブな注文
                                            actOrd = actOrd + 1;

                                        }
                                        else
                                        {
                                            // どちらも未約定か、または未発注

                                            // 未発注
                                            if ((ifdoco.OcoOneOrderID == 0) && (ifdoco.OcoOtherOrderID == 0))
                                            //if ((ifdoco.OcoOneOrderID == 0) || (ifdoco.OcoOtherOrderID == 0)) // not good
                                            {

                                                //Debug.WriteLine("□ UpdateIfdocos IFDOCO 要発注");

                                                // 要発注 
                                                needToOrderList.Add(ifdoco);


                                                // アクティブな注文
                                                actOrd = actOrd + 1;

                                            }
                                            else
                                            {
                                                // 更新リストに追加して後でアップデートする。
                                                // Add to ToBeUpdated List and Update Order info lator.
                                                if (ifdoco.OcoOneOrderID != 0)
                                                {
                                                    unfilledOrderIDsList.Add(ifdoco.OcoOneOrderID);
                                                }
                                                else
                                                {
                                                    //Debug.WriteLine("■IFDOCO oco one unfilledOrderIDsList: 注文IDが０。" + ifdoco.OcoOneOrderID.ToString());
                                                }

                                                if (ifdoco.OcoOtherOrderID != 0)
                                                {
                                                    unfilledOrderIDsList.Add(ifdoco.OcoOtherOrderID);
                                                }
                                                else
                                                {
                                                    //Debug.WriteLine("■IFDOCO oco other unfilledOrderIDsList: 注文IDが０。" + ifdoco.OcoOtherOrderID.ToString());
                                                }
                                            }

                                            if ((string.IsNullOrEmpty(ifdoco.OcoOtherStatus) == false) || (string.IsNullOrEmpty(ifdoco.OcoOneStatus) == false))
                                            {
                                                // アクティブな注文
                                                actOrd = actOrd + 1;

                                            }

                                        }

                                    }
                                    else
                                    {
                                        // IFDOCO 済んでるよ！

                                        ifdoco.IfdocoIsDone = true;
                                    }

                                //}

                            }

                        }

                    }

                });

                // タブの「IFDOCO注文（＊）」を更新
                ActiveIfdocosCount= actOrd;

                // リストのリスト（小分けにして分割取得用）
                List<List<int>> ListOfList = new List<List<int>>();

                // 未約定注文の最新状態をアップデートする。
                if (unfilledOrderIDsList.Count > 0)
                {
                    // GetOrderListByIDs 40015 数が多いとエラーになるから小分けにして。
                    List<int> temp = new List<int>();
                    int c = 0;

                    for (int i = 0; i < unfilledOrderIDsList.Count; i++)
                    {

                        temp.Add(unfilledOrderIDsList[c]);

                        if (temp.Count == 5)
                        {
                            // do

                            ListOfList.Add(temp);

                            temp = new List<int>();
                        }

                        if (c == unfilledOrderIDsList.Count - 1)
                        {
                            if (temp.Count > 0)
                            {
                                //do

                                ListOfList.Add(temp);
                            }

                            break;
                        }

                        c = c + 1;
                    }

                    foreach (var list in ListOfList)
                    {
                        // 最新の注文情報をゲット
                        Orders ords = await _priApi.GetOrderListByIDs(_ifdocoTradeApiKey, _ifdocoTradeSecret, "btc_jpy", list);//unfilledOrderIDsList);

                        if (ords != null)
                        {

                            if (Application.Current == null)
                            {
                                //Debug.WriteLine("■IFD Application.Current == null");
                                break;
                            }
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                //Debug.WriteLine("■IFD Invoke " + ords.OrderList.Count.ToString());

                                // 更新された注文をアップデート
                                foreach (var ord in ords.OrderList)
                                {
                                    //Debug.WriteLine("■IFD loop");
                                    try
                                    {
                                        // Ifdone
                                        var found = _ifdocos.FirstOrDefault((x => x.IfdoneOrderID == ord.OrderID));
                                        if (found != null)
                                        {

                                            found.IfdoneSide = ord.Side;
                                            if (ord.Type == "limit")
                                            {
                                                found.IfdoneType = IfdocoTypes.limit;
                                            }
                                            else if (ord.Type == "market")
                                            {
                                                found.IfdoneType = IfdocoTypes.market;
                                            }
                                            found.IfdoneOrderedAt = ord.OrderedAt;
                                            found.IfdoneStartAmount = ord.StartAmount;
                                            found.IfdonePrice = ord.Price;
                                            found.IfdoneExecutedAmount = ord.ExecutedAmount;
                                            found.IfdoneAveragePrice = ord.AveragePrice;
                                            found.IfdoneStatus = ord.Status;

                                            if (found.IfdoneStatus == "FULLY_FILLED" || found.IfdoneStatus == "CANCELED_UNFILLED" || found.IfdoneStatus == "CANCELED_PARTIALLY_FILLED")
                                            {
                                                found.IfdoneIsDone = true;

                                                if (found.IfdoneStatus == "FULLY_FILLED")
                                                {
                                                    //System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos unfilledOrderIDsList Ifdone 約定！");
                                                }

                                                if (found.IfdoneStatus == "CANCELED_UNFILLED" || found.IfdoneStatus == "CANCELED_PARTIALLY_FILLED")
                                                {
                                                    // キャンセルされたので、ifdDoもストップさせる。

                                                    found.IfdDoIsDone = true;
                                                    found.IfdIsDone = true;
                                                }
                                            }

                                        }

                                        // Ifd Do
                                        found = _ifdocos.FirstOrDefault((x => x.IfdDoOrderID == ord.OrderID));
                                        if (found != null)
                                        {

                                            found.IfdDoSide = ord.Side;
                                            if (ord.Type == "limit")
                                            {
                                                found.IfdDoType = IfdocoTypes.limit;
                                            }
                                            else if (ord.Type == "market")
                                            {
                                                found.IfdDoType = IfdocoTypes.market;
                                            }
                                            found.IfdDoOrderedAt = ord.OrderedAt;
                                            found.IfdDoStartAmount = ord.StartAmount;
                                            found.IfdDoPrice = ord.Price;
                                            found.IfdDoExecutedAmount = ord.ExecutedAmount;
                                            found.IfdDoAveragePrice = ord.AveragePrice;
                                            found.IfdDoStatus = ord.Status;

                                            if (found.IfdDoStatus == "FULLY_FILLED" || found.IfdDoStatus == "CANCELED_UNFILLED" || found.IfdDoStatus == "CANCELED_PARTIALLY_FILLED")
                                            {
                                                found.IfdDoIsDone = true;

                                                if (found.IfdDoStatus == "FULLY_FILLED")
                                                {
                                                    //System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos unfilledOrderIDsList IfdDo 約定！");
                                                }

                                                if (found.IfdDoStatus == "CANCELED_UNFILLED" || found.IfdDoStatus == "CANCELED_PARTIALLY_FILLED")
                                                {
                                                    found.IfdDoIsDone = true;
                                                    found.IfdIsDone = true;
                                                }
                                            }

                                        }

                                        // OCO one
                                        found = _ifdocos.FirstOrDefault((x => x.OcoOneOrderID == ord.OrderID));
                                        if (found != null)
                                        {

                                            found.OcoOneSide = ord.Side;
                                            if (ord.Type == "limit")
                                            {
                                                found.OcoOneType = IfdocoTypes.limit;
                                            }
                                            else if (ord.Type == "market")
                                            {
                                                found.OcoOneType = IfdocoTypes.market;
                                            }
                                            found.OcoOneOrderedAt = ord.OrderedAt;
                                            found.OcoOneStartAmount = ord.StartAmount;
                                            found.OcoOnePrice = ord.Price;
                                            found.OcoOneExecutedAmount = ord.ExecutedAmount;
                                            found.OcoOneAveragePrice = ord.AveragePrice;
                                            found.OcoOneStatus = ord.Status;

                                            if (found.OcoOneStatus == "FULLY_FILLED" || found.OcoOneStatus == "CANCELED_UNFILLED" || found.OcoOneStatus == "CANCELED_PARTIALLY_FILLED")
                                            {
                                                found.OcoOneIsDone = true;

                                                if (found.OcoOneStatus == "FULLY_FILLED")
                                                {
                                                    //System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos unfilledOrderIDsList OcoOne 約定！");
                                                }
                                            }

                                        }

                                        // OCO other
                                        found = _ifdocos.FirstOrDefault((x => x.OcoOtherOrderID == ord.OrderID));
                                        if (found != null)
                                        {

                                            found.OcoOtherSide = ord.Side;
                                            if (ord.Type == "limit")
                                            {
                                                found.OcoOtherType = IfdocoTypes.limit;
                                            }
                                            else if (ord.Type == "market")
                                            {
                                                found.OcoOtherType = IfdocoTypes.market;
                                            }
                                            found.OcoOtherOrderedAt = ord.OrderedAt;
                                            found.OcoOtherStartAmount = ord.StartAmount;
                                            found.OcoOtherPrice = ord.Price;
                                            found.OcoOtherExecutedAmount = ord.ExecutedAmount;
                                            found.OcoOtherAveragePrice = ord.AveragePrice;
                                            found.OcoOtherStatus = ord.Status;

                                            if (found.OcoOtherStatus == "FULLY_FILLED" || found.OcoOtherStatus == "CANCELED_UNFILLED" || found.OcoOtherStatus == "CANCELED_PARTIALLY_FILLED")
                                            {
                                                found.OcoOtherIsDone = true;

                                                if (found.OcoOtherStatus == "FULLY_FILLED")
                                                {
                                                    //System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos unfilledOrderIDsList OcoOther 約定！");
                                                }
                                            }

                                        }

                                    }
                                    catch (Exception e)
                                    {
                                        System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdoco Exception: " + e);
                                    }

                                }
                            });

                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdoco ords == null");

                            // エラー表示
                            ActiveIfdocosCount = -1;

                        }
                    }

                }

                // Ifd約定で、Ifd Do / OCO がまだ未発注のがあったら、発注する。
                if (needToOrderList.Count > 0)
                {
                    
                    foreach (var ifdoco in needToOrderList)
                    {
                        
                        if (ifdoco.Kind == IfdocoKinds.ifd)
                        {
                            //System.Diagnostics.Debug.WriteLine("■IFD");

                            // IFD
                            if (ifdoco.IfdoneIsDone)
                            {
                                System.Diagnostics.Debug.WriteLine("■IFD IfdoneIsDone");

                                // 条件が揃ったら、IfdDoを発注
                                if ((ifdoco.IfdDoIsDone == false) && (ifdoco.IfdDoOrderID == 0) && (ifdoco.IfdoneHasError == false))
                                {
                                    // トリガー
                                    System.Diagnostics.Debug.WriteLine("■トリガー:" + ifdoco.IfdDoTriggerPrice.ToString());

                                    if (ifdoco.IfdDoTriggerPrice <= 0) continue;

                                    if (ifdoco.IfdDoTriggerUpDown == 0)
                                    {
                                        System.Diagnostics.Debug.WriteLine("■以上");
                                        // 以上
                                        if (Ltp >= ifdoco.IfdDoTriggerPrice)
                                        {
                                            System.Diagnostics.Debug.WriteLine("■good");
                                            // good
                                        }
                                        else
                                        {
                                            continue;
                                        }

                                    }
                                    else if (ifdoco.IfdDoTriggerUpDown == 1)
                                    {
                                        System.Diagnostics.Debug.WriteLine("■以下");
                                        // 以下
                                        if (Ltp <= ifdoco.IfdDoTriggerPrice)
                                        {
                                            System.Diagnostics.Debug.WriteLine("■good");
                                            // good
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        continue;
                                    }


                                    OrderResult res = await _priApi.MakeOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, "btc_jpy", ifdoco.IfdDoStartAmount, ifdoco.IfdDoPrice, ifdoco.IfdDoSide, ifdoco.IfdDoType.ToString());

                                    if (res != null)
                                    {
                                        if (res.IsSuccess)
                                        {

                                            ifdoco.IfdDoHasError = false;

                                            ifdoco.IfdDoOrderID = res.OrderID;
                                            ifdoco.IfdDoOrderedAt = res.OrderedAt;
                                            ifdoco.IfdDoPrice = res.Price;
                                            ifdoco.IfdDoAveragePrice = res.AveragePrice;
                                            ifdoco.IfdDoStatus = res.Status;

                                            ifdoco.IfdDoRemainingAmount = res.RemainingAmount;
                                            ifdoco.IfdDoExecutedAmount = res.ExecutedAmount;
                                            // TODO

                                            // 約定
                                            if (res.Status == "FULLY_FILLED")
                                            {
                                                // フラグをセット
                                                ifdoco.IfdDoIsDone = true;
                                                ifdoco.IfdIsDone = true;

                                                // 約定！

                                                //System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos needToOrderList IfdDo 約定！");

                                            }

                                        }
                                        else
                                        {
                                            ifdoco.IfdDoHasError = true;
                                            if (ifdoco.IfdDoErrorInfo == null)
                                            {
                                                ifdoco.IfdDoErrorInfo = new ErrorInfo();
                                            }
                                            ifdoco.IfdDoErrorInfo.ErrorTitle = res.Err.ErrorTitle;
                                            ifdoco.IfdDoErrorInfo.ErrorDescription = res.Err.ErrorDescription;
                                            ifdoco.IfdDoErrorInfo.ErrorCode = res.Err.ErrorCode;

                                            System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos needToOrderList IFD MakeOrder API returned error code.");
                                        }
                                    }
                                    else
                                    {
                                        ifdoco.IfdDoHasError = true;
                                        if (ifdoco.IfdDoErrorInfo == null)
                                        {
                                            ifdoco.IfdDoErrorInfo = new ErrorInfo();
                                        }
                                        ifdoco.IfdDoErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                                        ifdoco.IfdDoErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                                        ifdoco.IfdDoErrorInfo.ErrorCode = -1;

                                        System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos needToOrderList IFD MakeOrder API returned null.");

                                    }

                                }
                            }

                        }
                        else if (ifdoco.Kind == IfdocoKinds.oco)
                        {
                            // OCO
                            if ((ifdoco.OcoIsDone == false))
                            {

                                if (ifdoco.OcoOneIsDone && ifdoco.OcoOtherIsDone)
                                {
                                    // do nothing
                                    continue;
                                }
                                else if (ifdoco.OcoOneIsDone || ifdoco.OcoOtherIsDone)
                                {
                                    // 片方約定
                                    // どちらかをキャンセル

                                    if (ifdoco.OcoOneIsDone)
                                    {
                                        // 未発注なら済みにする
                                        if (ifdoco.OcoOtherOrderID == 0)
                                        {
                                            ifdoco.OcoOtherIsDone = true;
                                            ifdoco.OcoIsDone = true;
                                            continue;
                                        }

                                        if (ifdoco.OcoOtherHasError == false)
                                        {

                                            // OCO other をキャンセル
                                            int cancelId = ifdoco.OcoOtherOrderID;

                                            //System.Diagnostics.Debug.WriteLine("■ UpdateIfdocos needToOrderList OCO CancelOrder .....");

                                            //OrderResult res = await _priIfdocoListApi.CancelOrder("btc_jpy", cancelId);
                                            OrderResult res = await _priApi.CancelOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, "btc_jpy", cancelId);

                                            if (res != null)
                                            {
                                                if (res.IsSuccess)
                                                {

                                                    ifdoco.OcoOtherHasError = false;

                                                    ifdoco.OcoOtherOrderID = res.OrderID;
                                                    ifdoco.OcoOtherOrderedAt = res.OrderedAt;
                                                    ifdoco.OcoOtherPrice = res.Price;
                                                    ifdoco.OcoOtherAveragePrice = res.AveragePrice;
                                                    ifdoco.OcoOtherStatus = res.Status;

                                                    ifdoco.OcoOtherRemainingAmount = res.RemainingAmount;
                                                    ifdoco.OcoOtherExecutedAmount = res.ExecutedAmount;
                                                    // TODO

                                                    // 約定
                                                    if (res.Status == "CANCELED_UNFILLED" || res.Status == "CANCELED_PARTIALLY_FILLED")
                                                    {
                                                        // フラグをセット
                                                        ifdoco.OcoOtherIsDone = true;
                                                        ifdoco.OcoIsDone = true;

                                                        // 約定！

                                                        //System.Diagnostics.Debug.WriteLine("■ UpdateIfdocos needToOrderList OCO Other cancel");

                                                    }


                                                }
                                                else
                                                {
                                                    ifdoco.OcoOtherHasError = true;
                                                    if (ifdoco.OcoOtherErrorInfo == null)
                                                    {
                                                        ifdoco.OcoOtherErrorInfo = new ErrorInfo();
                                                    }
                                                    ifdoco.OcoOtherErrorInfo.ErrorTitle = res.Err.ErrorTitle;
                                                    ifdoco.OcoOtherErrorInfo.ErrorDescription = res.Err.ErrorDescription;
                                                    ifdoco.OcoOtherErrorInfo.ErrorCode = res.Err.ErrorCode;

                                                    System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos needToOrderList OCO other CancelOrder API returned error code.");
                                                }
                                            }
                                            else
                                            {
                                                ifdoco.OcoOtherHasError = true;
                                                if (ifdoco.OcoOtherErrorInfo == null)
                                                {
                                                    ifdoco.OcoOtherErrorInfo = new ErrorInfo();
                                                }
                                                ifdoco.OcoOtherErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                                                ifdoco.OcoOtherErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                                                ifdoco.OcoOtherErrorInfo.ErrorCode = -1;

                                                System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos needToOrderList OCO CancelOrder API returned null.");


                                            }

                                        }

                                    }
                                    else if (ifdoco.OcoOtherIsDone)
                                    {
                                        // 未発注なら済みにする
                                        if (ifdoco.OcoOneOrderID == 0)
                                        {
                                            ifdoco.OcoOneIsDone = true;
                                            ifdoco.OcoIsDone = true;
                                            continue;
                                        }

                                        if (ifdoco.OcoOneHasError == false)
                                        {
                                            // OCO one をキャンセル
                                            int cancelId = ifdoco.OcoOneOrderID;

                                            //System.Diagnostics.Debug.WriteLine("■ UpdateIfdocos needToOrderList OCO CancelOrder .....");

                                            OrderResult res = await _priApi.CancelOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, "btc_jpy", cancelId);

                                            if (res != null)
                                            {
                                                if (res.IsSuccess)
                                                {

                                                    ifdoco.OcoOneHasError = false;

                                                    ifdoco.OcoOneOrderID = res.OrderID;
                                                    ifdoco.OcoOneOrderedAt = res.OrderedAt;
                                                    ifdoco.OcoOnePrice = res.Price;
                                                    ifdoco.OcoOneAveragePrice = res.AveragePrice;
                                                    ifdoco.OcoOneStatus = res.Status;

                                                    ifdoco.OcoOneRemainingAmount = res.RemainingAmount;
                                                    ifdoco.OcoOneExecutedAmount = res.ExecutedAmount;
                                                    // TODO

                                                    // 約定
                                                    if (res.Status == "CANCELED_UNFILLED" || res.Status == "CANCELED_PARTIALLY_FILLED")
                                                    {
                                                        // フラグをセット
                                                        ifdoco.OcoOneIsDone = true;
                                                        ifdoco.OcoIsDone = true;

                                                        // 約定！

                                                        //System.Diagnostics.Debug.WriteLine("■ UpdateIfdocos needToOrderList OCO One cancel 約定！");

                                                    }


                                                }
                                                else
                                                {
                                                    ifdoco.OcoOtherHasError = true;
                                                    if (ifdoco.OcoOtherErrorInfo == null)
                                                    {
                                                        ifdoco.OcoOtherErrorInfo = new ErrorInfo();
                                                    }
                                                    ifdoco.OcoOtherErrorInfo.ErrorTitle = res.Err.ErrorTitle;
                                                    ifdoco.OcoOtherErrorInfo.ErrorDescription = res.Err.ErrorDescription;
                                                    ifdoco.OcoOtherErrorInfo.ErrorCode = res.Err.ErrorCode;

                                                    System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos needToOrderList OCO one CancelOrder API returned error code.");
                                                }
                                            }
                                            else
                                            {
                                                ifdoco.OcoOtherHasError = true;
                                                if (ifdoco.OcoOtherErrorInfo == null)
                                                {
                                                    ifdoco.OcoOtherErrorInfo = new ErrorInfo();
                                                }
                                                ifdoco.OcoOtherErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                                                ifdoco.OcoOtherErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                                                ifdoco.OcoOtherErrorInfo.ErrorCode = -1;

                                                System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos needToOrderList OCO CancelOrder API returned null.");


                                            }

                                        }

                                    }

                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("■OCO どちらも未約定");
                                    
                                    // どちらも未約定

                                    // Oco One
                                    if (ifdoco.OcoOneOrderID == 0)
                                    {
                                        if (ifdoco.OcoOneTriggerPrice <= 0) continue;

                                        if (ifdoco.OcoOneTriggerUpDown == 0)
                                        {
                                            //System.Diagnostics.Debug.WriteLine("■以上");
                                            // 以上
                                            if (Ltp >= ifdoco.OcoOneTriggerPrice)
                                            {
                                                System.Diagnostics.Debug.WriteLine("■Oco One 以上: Ltp = " + Ltp.ToString() + " Trigger = " + ifdoco.OcoOneTriggerPrice.ToString());
                                                // good
                                            }
                                            else
                                            {
                                                continue;
                                            }

                                        }
                                        else if (ifdoco.OcoOneTriggerUpDown == 1)
                                        {
                                            //System.Diagnostics.Debug.WriteLine("■以下");
                                            // 以下
                                            if (Ltp >= ifdoco.OcoOneTriggerPrice)
                                            {
                                                System.Diagnostics.Debug.WriteLine("■Oco One 以下: Ltp = " + Ltp.ToString() + " Trigger = " + ifdoco.OcoOneTriggerPrice.ToString());
                                                // good
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            continue;
                                        }

                                        // OcoOne
                                        OrderResult ord = await _priApi.MakeOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, "btc_jpy", ifdoco.OcoOneStartAmount, ifdoco.OcoOnePrice, ifdoco.OcoOneSide, ifdoco.OcoOneType.ToString());

                                        if (ord != null)
                                        {
                                            if (ord.IsSuccess)
                                            {
                                                ifdoco.OcoOneHasError = false;

                                                ifdoco.OcoOneOrderID = ord.OrderID;
                                                ifdoco.OcoOneOrderedAt = ord.OrderedAt;
                                                ifdoco.OcoOnePrice = ord.Price;
                                                ifdoco.OcoOneAveragePrice = ord.AveragePrice;
                                                ifdoco.OcoOneStatus = ord.Status;
                                                // TODO

                                                // 約定
                                                if (ord.Status == "FULLY_FILLED")
                                                {
                                                    // フラグをセット
                                                    ifdoco.OcoOneIsDone = true;

                                                    // 後は、UpdateIFDOCOループにまかせる
                                                }
                                            }
                                            else
                                            {
                                                ifdoco.OcoOneHasError = true;
                                                if (ifdoco.OcoOneErrorInfo == null)
                                                {
                                                    ifdoco.OcoOneErrorInfo = new ErrorInfo();
                                                }
                                                ifdoco.OcoOneErrorInfo.ErrorTitle = ord.Err.ErrorTitle;
                                                ifdoco.OcoOneErrorInfo.ErrorDescription = ord.Err.ErrorDescription;
                                                ifdoco.OcoOneErrorInfo.ErrorCode = ord.Err.ErrorCode;

                                                System.Diagnostics.Debug.WriteLine("UpdateIfdocos - OcoOne MakeOrder API failed");

                                            }

                                        }
                                        else
                                        {
                                            ifdoco.OcoOneHasError = true;
                                            if (ifdoco.OcoOneErrorInfo == null)
                                            {
                                                ifdoco.OcoOneErrorInfo = new ErrorInfo();
                                            }
                                            ifdoco.OcoOneErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                                            ifdoco.OcoOneErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                                            ifdoco.OcoOneErrorInfo.ErrorCode = -1;

                                            System.Diagnostics.Debug.WriteLine("UpdateIfdocos - OcoOne MakeOrder returened NULL");

                                        }


                                    }

                                    // Oco Other
                                    if (ifdoco.OcoOtherOrderID == 0)
                                    {
                                        if (ifdoco.OcoOtherTriggerPrice <= 0) continue;

                                        if (ifdoco.OcoOtherTriggerUpDown == 0)
                                        {
                                            //System.Diagnostics.Debug.WriteLine("■以上");
                                            // 以上
                                            if (Ltp >= ifdoco.OcoOtherTriggerPrice)
                                            {
                                                System.Diagnostics.Debug.WriteLine("■Oco Other 以上: Ltp = " + Ltp.ToString() + " Trigger = " + ifdoco.OcoOtherTriggerPrice.ToString());
                                                // good
                                            }
                                            else
                                            {
                                                continue;
                                            }

                                        }
                                        else if (ifdoco.OcoOtherTriggerUpDown == 1)
                                        {
                                            //System.Diagnostics.Debug.WriteLine("■以下");
                                            // 以下
                                            if (Ltp <= ifdoco.OcoOtherTriggerPrice)
                                            {
                                                System.Diagnostics.Debug.WriteLine("■Oco Other 以下: Ltp = " + Ltp.ToString() + " Trigger = " + ifdoco.OcoOtherTriggerPrice.ToString());
                                                // good
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            continue;
                                        }


                                        OrderResult ord2 = await _priApi.MakeOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, "btc_jpy", ifdoco.OcoOtherStartAmount, ifdoco.OcoOtherPrice, ifdoco.OcoOtherSide, ifdoco.OcoOtherType.ToString());

                                        if (ord2 != null)
                                        {

                                            if (ord2.IsSuccess)
                                            {
                                                ifdoco.OcoOtherHasError = false;

                                                ifdoco.OcoOtherOrderID = ord2.OrderID;
                                                ifdoco.OcoOtherOrderedAt = ord2.OrderedAt;
                                                ifdoco.OcoOtherPrice = ord2.Price;
                                                ifdoco.OcoOtherAveragePrice = ord2.AveragePrice;
                                                ifdoco.OcoOtherStatus = ord2.Status;
                                                // TODO

                                                // 約定
                                                if (ord2.Status == "FULLY_FILLED")
                                                {


                                                    // フラグをセット
                                                    ifdoco.OcoOtherIsDone = true;


                                                    // 後は、UpdateIFDOCOループにまかせる

                                                }
                                            }
                                            else
                                            {
                                                ifdoco.OcoOtherHasError = true;
                                                if (ifdoco.OcoOtherErrorInfo == null)
                                                {
                                                    ifdoco.OcoOtherErrorInfo = new ErrorInfo();
                                                }
                                                ifdoco.OcoOtherErrorInfo.ErrorTitle = ord2.Err.ErrorTitle;
                                                ifdoco.OcoOtherErrorInfo.ErrorDescription = ord2.Err.ErrorDescription;
                                                ifdoco.OcoOtherErrorInfo.ErrorCode = ord2.Err.ErrorCode;

                                                System.Diagnostics.Debug.WriteLine("UpdateIfdocos - OcoOther MakeOrder API failed");
                                            }

                                        }
                                        else
                                        {
                                            ifdoco.OcoOtherHasError = true;
                                            if (ifdoco.OcoOtherErrorInfo == null)
                                            {
                                                ifdoco.OcoOtherErrorInfo = new ErrorInfo();
                                            }
                                            ifdoco.OcoOtherErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                                            ifdoco.OcoOtherErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                                            ifdoco.OcoOtherErrorInfo.ErrorCode = -1;

                                            System.Diagnostics.Debug.WriteLine("UpdateIfdocos - OcoOther MakeOrder returened NULL");
                                        }

                                    }

                                }
                            }
                            
                        }
                        else if (ifdoco.Kind == IfdocoKinds.ifdoco)
                        {
                            // IFDOCO
                            if (ifdoco.IfdocoIsDone == false)
                            {
                                // IFDone が約定済み
                                if (ifdoco.IfdoneIsDone)
                                {
                                    // OCOが両方とも未発注
                                    if (((ifdoco.OcoOneOrderID == 0) || (ifdoco.OcoOtherOrderID == 0)) && (ifdoco.OcoIsDone == false))
                                    {
                                        // OCO One 発注
                                        if ((ifdoco.OcoOneOrderID == 0) && (ifdoco.OcoOneHasError == false))
                                        {
                                            bool isTriggered = false;

                                            if (ifdoco.OcoOneTriggerUpDown == 0)
                                            {
                                                //System.Diagnostics.Debug.WriteLine("■以上");
                                                // 以上
                                                if (Ltp >= ifdoco.OcoOneTriggerPrice)
                                                {
                                                    System.Diagnostics.Debug.WriteLine("■Oco One 以上: Ltp = " + Ltp.ToString() + " Trigger = " + ifdoco.OcoOneTriggerPrice.ToString());
                                                    // good
                                                    isTriggered = true;
                                                }
                                                else
                                                {
                                                    //continue;
                                                }

                                            }
                                            else if (ifdoco.OcoOneTriggerUpDown == 1)
                                            {
                                                //System.Diagnostics.Debug.WriteLine("■以下");
                                                // 以下
                                                if (Ltp >= ifdoco.OcoOneTriggerPrice)
                                                {
                                                    System.Diagnostics.Debug.WriteLine("■Oco One 以下: Ltp = " + Ltp.ToString() + " Trigger = " + ifdoco.OcoOneTriggerPrice.ToString());
                                                    // good
                                                    isTriggered = true;
                                                }
                                                else
                                                {
                                                    //continue;
                                                }
                                            }
                                            else
                                            {
                                                //continue;
                                            }

                                            if (ifdoco.OcoOneTriggerPrice <= 0)
                                            {
                                                isTriggered = false;
                                            }

                                            if (isTriggered)
                                            {
                                                // OcoOne
                                                OrderResult ord = await _priApi.MakeOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, "btc_jpy", ifdoco.OcoOneStartAmount, ifdoco.OcoOnePrice, ifdoco.OcoOneSide, ifdoco.OcoOneType.ToString());

                                                if (ord != null)
                                                {

                                                    if (ord.IsSuccess)
                                                    {
                                                        //ifdoco.IfdoneHasError = false;
                                                        ifdoco.OcoOneHasError = false;

                                                        ifdoco.OcoOneOrderID = ord.OrderID;
                                                        ifdoco.OcoOneOrderedAt = ord.OrderedAt;

                                                        ifdoco.OcoOnePrice = ord.Price;
                                                        ifdoco.OcoOneAveragePrice = ord.AveragePrice;
                                                        ifdoco.OcoOneStatus = ord.Status;
                                                        // TODO

                                                        // 約定
                                                        if (ord.Status == "FULLY_FILLED")
                                                        {
                                                            // フラグをセット
                                                            ifdoco.OcoOneIsDone = true;


                                                        }
                                                    }
                                                    else
                                                    {
                                                        ifdoco.OcoOneHasError = true;
                                                        if (ifdoco.OcoOneErrorInfo == null)
                                                        {
                                                            ifdoco.OcoOneErrorInfo = new ErrorInfo();
                                                        }
                                                        ifdoco.OcoOneErrorInfo.ErrorTitle = ord.Err.ErrorTitle;
                                                        ifdoco.OcoOneErrorInfo.ErrorDescription = ord.Err.ErrorDescription;
                                                        ifdoco.OcoOneErrorInfo.ErrorCode = ord.Err.ErrorCode;

                                                        System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos - Ifdoco - OcoOne MakeOrder API failed:" + ord.Err.ErrorCode.ToString());
                                                    }

                                                }
                                                else
                                                {
                                                    ifdoco.OcoOneHasError = true;
                                                    if (ifdoco.OcoOneErrorInfo == null)
                                                    {
                                                        ifdoco.OcoOneErrorInfo = new ErrorInfo();
                                                    }
                                                    ifdoco.OcoOneErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                                                    ifdoco.OcoOneErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                                                    ifdoco.OcoOneErrorInfo.ErrorCode = -1;

                                                    System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos - Ifdoco - OcoOne MakeOrder returened NULL");
                                                }

                                            }

                                        }
                                        else
                                        {
                                            // TODO
                                            //Debug.WriteLine("□ UpdateIfdocos IFDOCO @OCOが未発注 ELSE OcoOneHasError");
                                        }

                                        // OCO Other 発注
                                        if ((ifdoco.OcoOtherOrderID == 0) && (ifdoco.OcoOtherHasError == false))
                                        {
                                            bool isTriggered = false;

                                            if (ifdoco.OcoOtherTriggerUpDown == 0)
                                            {
                                                //System.Diagnostics.Debug.WriteLine("■以上");
                                                // 以上
                                                if (Ltp >= ifdoco.OcoOtherTriggerPrice)
                                                {
                                                    System.Diagnostics.Debug.WriteLine("■Oco Other 以上: Ltp = " + Ltp.ToString() + " Trigger = " + ifdoco.OcoOtherTriggerPrice.ToString());
                                                    // good
                                                    isTriggered = true;
                                                }
                                                else
                                                {
                                                    //continue;
                                                }

                                            }
                                            else if (ifdoco.OcoOtherTriggerUpDown == 1)
                                            {
                                                //System.Diagnostics.Debug.WriteLine("■以下");
                                                // 以下
                                                if (Ltp <= ifdoco.OcoOtherTriggerPrice)
                                                {
                                                    System.Diagnostics.Debug.WriteLine("■Oco Other 以下: Ltp = " + Ltp.ToString() + " Trigger = " + ifdoco.OcoOtherTriggerPrice.ToString());
                                                    // good
                                                    isTriggered = true;
                                                }
                                                else
                                                {
                                                    //continue;
                                                }
                                            }
                                            else
                                            {
                                                //continue;
                                            }

                                            if (ifdoco.OcoOtherTriggerPrice <= 0)
                                            {
                                                isTriggered = false;
                                            }

                                            if (isTriggered)
                                            {
                                                // OcoOther
                                                OrderResult ord2 = await _priApi.MakeOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, "btc_jpy", ifdoco.OcoOtherStartAmount, ifdoco.OcoOtherPrice, ifdoco.OcoOtherSide, ifdoco.OcoOtherType.ToString());

                                                if (ord2 != null)
                                                {

                                                    if (ord2.IsSuccess)
                                                    {
                                                        ifdoco.OcoOtherHasError = false;

                                                        ifdoco.OcoOtherOrderID = ord2.OrderID;
                                                        ifdoco.OcoOtherOrderedAt = ord2.OrderedAt;
                                                        ifdoco.OcoOtherPrice = ord2.Price;
                                                        ifdoco.OcoOtherAveragePrice = ord2.AveragePrice;
                                                        ifdoco.OcoOtherStatus = ord2.Status;
                                                        // TODO

                                                        // 約定
                                                        if (ord2.Status == "FULLY_FILLED")
                                                        {


                                                            // フラグをセット
                                                            ifdoco.OcoOtherIsDone = true;


                                                        }
                                                    }
                                                    else
                                                    {
                                                        ifdoco.OcoOtherHasError = true;
                                                        if (ifdoco.OcoOtherErrorInfo == null)
                                                        {
                                                            ifdoco.OcoOtherErrorInfo = new ErrorInfo();
                                                        }
                                                        ifdoco.OcoOtherErrorInfo.ErrorTitle = ord2.Err.ErrorTitle;
                                                        ifdoco.OcoOtherErrorInfo.ErrorDescription = ord2.Err.ErrorDescription;
                                                        ifdoco.OcoOtherErrorInfo.ErrorCode = ord2.Err.ErrorCode;

                                                        System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos - Ifdoco OcoOther MakeOrder API failed:" + ord2.Err.ErrorCode.ToString());
                                                    }

                                                }
                                                else
                                                {
                                                    ifdoco.OcoOtherHasError = true;
                                                    if (ifdoco.OcoOtherErrorInfo == null)
                                                    {
                                                        ifdoco.OcoOtherErrorInfo = new ErrorInfo();
                                                    }
                                                    ifdoco.OcoOtherErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                                                    ifdoco.OcoOtherErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                                                    ifdoco.OcoOtherErrorInfo.ErrorCode = -1;

                                                    System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos - Ifdoco OcoOther MakeOrder returened NULL");
                                                }


                                            }

                                        }
                                        else
                                        {
                                            // TODO
                                            //Debug.WriteLine("□ UpdateIfdocos IFDOCO @OCOが未発注 ELSE OcoOtherHasError");
                                        }

                                    }
                                    else if ((ifdoco.OcoOneIsDone || ifdoco.OcoOtherIsDone) && (ifdoco.OcoIsDone == false))
                                    {
                                        // OCOの片方が約定済み ＞どちらかをキャンセル

                                        if (ifdoco.OcoOneIsDone)
                                        {
                                            // OCO other をキャンセル
                                            int cancelId = ifdoco.OcoOtherOrderID;

                                            //System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos needToOrderList OCO CancelOrder .....");

                                            OrderResult res = await _priApi.CancelOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, "btc_jpy", cancelId);

                                            if (res != null)
                                            {
                                                if (res.IsSuccess)
                                                {

                                                    ifdoco.OcoOtherHasError = false;

                                                    ifdoco.OcoOtherOrderID = res.OrderID;
                                                    ifdoco.OcoOtherOrderedAt = res.OrderedAt;
                                                    ifdoco.OcoOtherPrice = res.Price;
                                                    ifdoco.OcoOtherAveragePrice = res.AveragePrice;
                                                    ifdoco.OcoOtherStatus = res.Status;

                                                    ifdoco.OcoOtherRemainingAmount = res.RemainingAmount;
                                                    ifdoco.OcoOtherExecutedAmount = res.ExecutedAmount;
                                                    // TODO

                                                    // 約定
                                                    if (res.Status == "CANCELED_UNFILLED" || res.Status == "CANCELED_PARTIALLY_FILLED")
                                                    {
                                                        // フラグをセット
                                                        ifdoco.OcoOtherIsDone = true;
                                                        ifdoco.OcoIsDone = true;

                                                        // 約定！

                                                        //System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos needToOrderList OCO Other cancel 約定！");

                                                    }


                                                }
                                                else
                                                {
                                                    ifdoco.OcoOtherHasError = true;
                                                    if (ifdoco.OcoOtherErrorInfo == null)
                                                    {
                                                        ifdoco.OcoOtherErrorInfo = new ErrorInfo();
                                                    }
                                                    ifdoco.OcoOtherErrorInfo.ErrorTitle = res.Err.ErrorTitle;
                                                    ifdoco.OcoOtherErrorInfo.ErrorDescription = res.Err.ErrorDescription;
                                                    ifdoco.OcoOtherErrorInfo.ErrorCode = res.Err.ErrorCode;

                                                    System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos needToOrderList OCO OTHER CancelOrder API returned error code.");
                                                }
                                            }
                                            else
                                            {
                                                ifdoco.OcoOtherHasError = true;
                                                if (ifdoco.OcoOtherErrorInfo == null)
                                                {
                                                    ifdoco.OcoOtherErrorInfo = new ErrorInfo();
                                                }
                                                ifdoco.OcoOtherErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                                                ifdoco.OcoOtherErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                                                ifdoco.OcoOtherErrorInfo.ErrorCode = -1;

                                                System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos needToOrderList OCO OTHER CancelOrder API returned null.");


                                            }

                                        }
                                        else if (ifdoco.OcoOtherIsDone)
                                        {
                                            // OCO one をキャンセル
                                            int cancelId = ifdoco.OcoOneOrderID;

                                            //System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos needToOrderList OCO CancelOrder .....");

                                            OrderResult res = await _priApi.CancelOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, "btc_jpy", cancelId);

                                            if (res != null)
                                            {
                                                if (res.IsSuccess)
                                                {

                                                    ifdoco.OcoOneHasError = false;

                                                    ifdoco.OcoOneOrderID = res.OrderID;
                                                    ifdoco.OcoOneOrderedAt = res.OrderedAt;
                                                    ifdoco.OcoOnePrice = res.Price;
                                                    ifdoco.OcoOneAveragePrice = res.AveragePrice;
                                                    ifdoco.OcoOneStatus = res.Status;

                                                    ifdoco.OcoOneRemainingAmount = res.RemainingAmount;
                                                    ifdoco.OcoOneExecutedAmount = res.ExecutedAmount;
                                                    // TODO

                                                    // 約定
                                                    if (res.Status == "CANCELED_UNFILLED" || res.Status == "CANCELED_PARTIALLY_FILLED")
                                                    {
                                                        // フラグをセット
                                                        ifdoco.OcoOneIsDone = true;
                                                        ifdoco.OcoIsDone = true;

                                                        // 約定！

                                                        //System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos needToOrderList OCO One cancel 約定！");

                                                    }


                                                }
                                                else
                                                {
                                                    ifdoco.OcoOtherHasError = true;
                                                    if (ifdoco.OcoOtherErrorInfo == null)
                                                    {
                                                        ifdoco.OcoOtherErrorInfo = new ErrorInfo();
                                                    }
                                                    ifdoco.OcoOtherErrorInfo.ErrorTitle = res.Err.ErrorTitle;
                                                    ifdoco.OcoOtherErrorInfo.ErrorDescription = res.Err.ErrorDescription;
                                                    ifdoco.OcoOtherErrorInfo.ErrorCode = res.Err.ErrorCode;

                                                    System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos needToOrderList OCO ONE CancelOrder API returned error code.");
                                                }
                                            }
                                            else
                                            {
                                                ifdoco.OcoOtherHasError = true;
                                                if (ifdoco.OcoOtherErrorInfo == null)
                                                {
                                                    ifdoco.OcoOtherErrorInfo = new ErrorInfo();
                                                }
                                                ifdoco.OcoOtherErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                                                ifdoco.OcoOtherErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                                                ifdoco.OcoOtherErrorInfo.ErrorCode = -1;

                                                System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos needToOrderList OCO ONE CancelOrder API returned null.");


                                            }

                                        }

                                    }

                                }

                            }

                        }

                    }

                }

                // 間隔 1/2 
                await Task.Delay(1400);

            }

        }

        // 注文リスト取得
        private async Task<bool> GetOrderList()
        {

            if (OrdersApiKeyIsSet == false)
            {
                // TODO show message?
                return false;
            }

            //System.Diagnostics.Debug.WriteLine("GetOrderList......");
            try
            {
                //Orders ords = await _priActiveOrderListApi.GetOrderList();
                Orders ords = await _priApi.GetOrderList(_getOrdersApiKey, _getOrdersSecret);

                if (ords != null)
                {
                    // タブの「注文一覧（＊）」を更新
                    ActiveOrdersCount = ords.OrderList.Count;

                    // 逆順にする
                    ords.OrderList.Reverse();

                    if (Application.Current == null)
                    {
                        return false;
                    }

                    //Application.Current.Dispatcher.Invoke(() =>
                    //{
                        try
                        {
                            foreach (var ord in ords.OrderList)
                            {

                                var found = _orders.FirstOrDefault(x => x.OrderID == ord.OrderID);
                                if (found != null)
                                {
                                    found.AveragePrice = ord.AveragePrice;
                                    found.OrderedAt = ord.OrderedAt;
                                    found.Pair = ord.Pair;
                                    found.Price = ord.Price;
                                    found.RemainingAmount = ord.RemainingAmount;
                                    found.ExecutedAmount = ord.ExecutedAmount;
                                    found.Side = ord.Side;
                                    found.StartAmount = ord.StartAmount;
                                    found.Type = ord.Type;
                                    found.Status = ord.Status;

                                    // 現在値のセット
                                    // 投資金額
                                    if (found.Type == "limit")
                                    {
                                        // 
                                        found.ActualPrice = (ord.Price * ord.StartAmount);
                                        // 一部約定の時は考えない？
                                    }
                                    else
                                    {
                                        found.ActualPrice = (ord.AveragePrice * ord.StartAmount);
                                    }
                                    // 現在値との差額
                                    if ((found.Status == "UNFILLED") || (found.Status == "PARTIALLY_FILLED"))
                                    {
                                        found.Shushi = ((_ltp - ord.Price));
                                    }
                                    else
                                    {
                                        // 約定済みなので
                                        found.Shushi = 0;
                                    }

                                    // TODO
                                    // 未約定が約定になったか比較判定


                                }
                                else
                                {
                                    // 現在値のセット
                                    // 投資金額
                                    if (ord.Type == "limit")
                                    {
                                        ord.ActualPrice = (ord.Price * ord.StartAmount);
                                    }
                                    else
                                    {
                                        ord.ActualPrice = (ord.AveragePrice * ord.StartAmount);
                                    }
                                    // 現在値との差額
                                    if ((ord.Status == "UNFILLED") || (ord.Status == "PARTIALLY_FILLED"))
                                    {
                                        ord.Shushi = ((_ltp - ord.Price));
                                    }
                                    else
                                    {
                                        // 約定済みなので
                                        ord.Shushi = 0;
                                    }


                                    // TODO
                                    // 約定済みで返ってきたかどうか判定


                                    // リスト追加
                                    //_orders.Add(ord);
                                    _orders.Insert(0, ord);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderList: Exception - " + ex.Message);

                        }
                    //});

                    // 返ってきた注文リストに存在しない、注文を抽出
                    List<int> lst = new List<int>();
                    try
                    {
                        foreach (var ors in _orders)
                        {
                            var found = ords.OrderList.FirstOrDefault(x => x.OrderID == ors.OrderID);
                            if (found == null)
                            {
                                if (string.IsNullOrEmpty(ors.Status) || (ors.Status == "UNFILLED") || (ors.Status == "PARTIALLY_FILLED"))
                                {
                                    //if ( (ors.Status != "FULLY_FILLED") || (ors.Status != "CANCELED_UNFILLED") || (ors.Status != "CANCELED_PARTIALLY_FILLED")) { 
                                    lst.Add(ors.OrderID);
                                }
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderList lst: Exception - " + ex.Message);

                    }

                    // 注文リスト更新
                    if (lst.Count > 0)
                    {
                        // 最新の情報をゲット
                        //Orders oup = await _priActiveOrderListApi.GetOrderListByIDs("btc_jpy", lst);
                        Orders oup = await _priApi.GetOrderListByIDs(_getOrdersApiKey, _getOrdersSecret, "btc_jpy", lst);

                        if (oup != null)
                        {
                            // 注文をアップデート
                            foreach (var ord in oup.OrderList)
                            {
                                if (Application.Current == null)
                                {
                                    return false;
                                }
                                //Application.Current.Dispatcher.Invoke(() =>
                                //{
                                    try
                                    {
                                        var found = _orders.FirstOrDefault(x => x.OrderID == ord.OrderID);
                                        if (found != null)
                                        {
                                            int i = _orders.IndexOf(found);
                                            if (i > -1)
                                            {
                                                _orders[i].Status = ord.Status;
                                                _orders[i].AveragePrice = ord.AveragePrice;
                                                _orders[i].OrderedAt = ord.OrderedAt;
                                                _orders[i].Type = ord.Type;
                                                _orders[i].StartAmount = ord.StartAmount;
                                                _orders[i].RemainingAmount = ord.RemainingAmount;
                                                _orders[i].ExecutedAmount = ord.ExecutedAmount;
                                                _orders[i].Price = ord.Price;
                                                _orders[i].AveragePrice = ord.AveragePrice;


                                                // 現在値のセット
                                                // 投資金額
                                                if (_orders[i].Type == "limit")
                                                {
                                                    _orders[i].ActualPrice = (ord.Price * ord.StartAmount);
                                                }
                                                else
                                                {
                                                    _orders[i].ActualPrice = (ord.AveragePrice * ord.StartAmount);
                                                }
                                                // 現在値との差額
                                                if ((_orders[i].Status == "UNFILLED") || (_orders[i].Status == "PARTIALLY_FILLED"))
                                                {
                                                    _orders[i].Shushi = ((_ltp - ord.Price));
                                                }
                                                else
                                                {
                                                    // 約定済みなので
                                                    _orders[i].Shushi = 0;
                                                }


                                                //TODO
                                                //　約定！

                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine("■■■■■ Order oup: Exception - " + ex.Message);

                                    }
                                //});
                            }
                        }
                    }


                    APIResultActiveOrders = "";
                    //NotifyPropertyChanged(nameof(APIResultActiveOrders));

                    await Task.Delay(1000);
                    return true;
                }
                else
                {
                    //
                    ActiveOrdersCount = -1;

                    APIResultActiveOrders = "<<注文一覧　取得失敗>>";
                    //NotifyPropertyChanged(nameof(APIResultActiveOrders));
                    System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderList 注文一覧　取得失敗");

                    await Task.Delay(1000);
                    return false;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderList Exception: " + e);

                await Task.Delay(1000);
                return false;
            }
        }

        // 注文リスト更新ループ
        private async void UpdateOrderList()
        {
            while (true)
            {
                // ログインしていなかったらスルー。
                if (LoggedInMode == false)
                {
                    await Task.Delay(2000);
                    continue;
                }
                // 省エネモードならスルー。
                if (MinMode)
                {
                    await Task.Delay(5000);
                    continue;
                }

                if (OrdersApiKeyIsSet == false)
                {
                    await Task.Delay(6000);
                    continue;
                }

                // 間隔 1/2
                await Task.Delay(1200);

                try
                {
                    await GetOrderList();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ UpdateOrderList Exception: " + e);
                }

                // 間隔 1/2
                await Task.Delay(1900);
            }

        }

        // 資産取得
        private async Task<bool> GetAssets()
        {
            if (AssetsApiKeyIsSet == false)
            {
                // TODO show message?

                System.Diagnostics.Debug.WriteLine("■■■■■ GetAssets: (AssetsApiKeyIsSet == false)");
                return false;
            }

            try
            {
                // TODO AssetsResult
                Assets asts = await _priApi.GetAssetList(_getAssetsApiKey, _getAssetsSecret);

                if (asts != null)
                {
                    try
                    {

                        foreach (var ast in asts.AssetList)
                        {

                            if (ast.Name == "jpy")
                            {
                                ast.NameText = "日本円";

                                AssetJPYName = ast.Name;
                                AssetJPYAmount = ast.Amount;
                                AssetJPYFreeAmount = ast.FreeAmount;

                                //Debug.WriteLine(AssetJPYAmount.ToString());
                            }
                            else if (ast.Name == "btc")
                            {
                                ast.NameText = "ビットコイン";

                                AssetBTCName = ast.Name;
                                AssetBTCAmount = ast.Amount;
                                AssetBTCFreeAmount = ast.FreeAmount;

                                //Debug.WriteLine("AssetBTCAmount :" + AssetBTCAmount.ToString());
                            }
                            else if (ast.Name == "ltc")
                            {
                                ast.NameText = "ライトコイン";

                                AssetLtcName = ast.Name;
                                AssetLtcAmount = ast.Amount;
                                AssetLtcFreeAmount = ast.FreeAmount;
                            }
                            else if (ast.Name == "xrp")
                            {
                                ast.NameText = "リップル";

                                AssetXRPName = ast.Name;
                                AssetXRPAmount = ast.Amount;
                                AssetXRPFreeAmount = ast.FreeAmount;
                            }
                            else if (ast.Name == "eth")
                            {
                                ast.NameText = "イーサリアム";

                                AssetEthName = ast.Name;
                                AssetEthAmount = ast.Amount;
                                AssetEthFreeAmount = ast.FreeAmount;
                            }
                            else if (ast.Name == "mona")
                            {
                                ast.NameText = "モナーコイン";

                                AssetMonaName = ast.Name;
                                AssetMonaAmount = ast.Amount;
                                AssetMonaFreeAmount = ast.FreeAmount;
                            }
                            else if (ast.Name == "bcc")
                            {
                                ast.NameText = "ビットコインキャッシュ";


                                AssetBchName = ast.Name;
                                AssetBchAmount = ast.Amount;
                                AssetBchFreeAmount = ast.FreeAmount;
                            }

                        }

                        // 

                        /*
                        // 円建て資産
                        var foundJpy = asts.AssetList.FirstOrDefault(x => x.Name == "jpy");
                        if (foundJpy != null)
                        {
                            AssetJPYName = foundJpy.Name;
                            AssetJPYAmount = foundJpy.Amount;
                            AssetJPYFreeAmount = foundJpy.FreeAmount;
                        }

                        // ビットコイン建て資産
                        var foundBtc = asts.AssetList.FirstOrDefault(x => x.Name == "btc");
                        if (foundBtc != null)
                        {
                            AssetBTCName = foundBtc.Name;
                            AssetBTCAmount = foundBtc.Amount;
                            AssetBTCFreeAmount = foundBtc.FreeAmount;
                        }
                        */


                        APIResultAssets = "";
                        //NotifyPropertyChanged(nameof(APIResultAssets));


                        await Task.Delay(1000);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("■■■■■ GetAssets: Exception - " + ex.Message);

                        await Task.Delay(1000);
                        return false;
                    }
                }
                else
                {
                    APIResultAssets = "<<取得失敗>>";
                    //NotifyPropertyChanged(nameof(APIResultAssets));

                    await Task.Delay(1000);
                    return false;
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetAssets Exception: " + e);

                await Task.Delay(1000);
                return false;
            }

        }

        // 資産取得の更新ループ
        private async void UpdateAssets()
        {
            while (true)
            {
                // ログインしていなかったらスルー。
                if (LoggedInMode == false)
                {
                    await Task.Delay(2000);
                    continue;
                }

                if (AssetsApiKeyIsSet == false)
                {
                    await Task.Delay(2000);
                    continue;
                }


                // 間隔 1/2
                await Task.Delay(1800);

                try
                {
                    await GetAssets();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAssets Exception: " + e);
                }

                // 間隔 1/2
                await Task.Delay(1500);
            }

        }
        
        // 板情報 取得
        private async Task<bool> GetDepth()
        {

            // まとめ単位 (0=off, 100,1000)
            //int unit = 0;
            int unit = DepthGrouping;

            // 高さ （基本 上売り200、下買い200）
            int half = 200;
            if (unit == 100)
            {
                // まとめ単位が0の時は200、まとめ単位が100の時は少なくする？
                //half = 120;
            }

            int listCount = (half * 2) +1;

            //グルーピング単位が変わったので、一旦クリアする。
            if (DepthGroupingChanged)
            {
                _depth.Clear();

                 DepthGroupingChanged = false;
            }

            if (_depth.Count == 0)
            {
                for (int i = 0; i < listCount; i++)
                {
                    Depth dd = new Depth();
                    dd.DepthPrice = 0;
                    dd.DepthBid = 0;
                    dd.DepthAsk = 0;
                    _depth.Add(dd);
                }
            }
                
            try
            {
                DepthResult dpr = await _pubDepthApi.GetDepth();

                if (dpr != null)
                {
                    if (_depth.Count != 0)
                    {

                        int i = 1;

                        // 100円単位でまとめる
                        // まとめた時の価格
                        decimal c2 = 0;
                        // 100単位ごとにまとめたAsk数量を保持
                        decimal t = 0;
                        // 先送りするAsk
                        decimal d = 0;
                        // 先送りする価格
                        decimal e = 0;

                        // ask をループ
                        foreach (var dp in dpr.DepthAskList)
                        {
                            // まとめ表示On
                            if (unit > 0)
                            {

                                //if (i == half) break;

                                //if (c == 0) c = dp.DepthPrice;
                                if (c2 == 0) c2 = System.Math.Ceiling(dp.DepthPrice / unit);

                                // 100円単位でまとめる
                                if (System.Math.Ceiling(dp.DepthPrice / unit) == c2)
                                {
                                    t = t + dp.DepthAsk;
                                }
                                else
                                {
                                    //Debug.WriteLine(System.Math.Ceiling(dp.DepthPrice / unit).ToString() + " " + System.Math.Ceiling(c / unit).ToString());

                                    // 一時保存
                                    e = dp.DepthPrice;
                                    dp.DepthPrice = (c2 * unit);
                                    // 一時保存
                                    d = dp.DepthAsk;
                                    dp.DepthAsk = t;

                                    // 追加
                                    //_depth.Insert(0, dp);
                                    //_depth[half - i] = dp;

                                    _depth[half - i].DepthAsk = dp.DepthAsk;
                                    _depth[half - i].DepthBid = dp.DepthBid;
                                    _depth[half - i].DepthPrice = dp.DepthPrice;

                                    // 今回のAskは先送り
                                    t = d;
                                    // 今回のPriceが基準になる
                                    c2 = System.Math.Ceiling(e / unit);//e;

                                    i++;

                                }

                            }
                            else
                            {
                                // 元々
                                //_depth.Insert(0, dp);

                                _depth[half - i] = dp;
                                i++;
                            }

                        }

                        //_depth[dpr.DepthAskList.Count - 1].IsAskBest = true;
                        _depth[i].IsAskBest = true;

                        i = half;

                        Depth dd = new Depth();
                        dd.DepthPrice = _ltp;
                        dd.DepthBid = 0;
                        dd.DepthAsk = 0;
                        dd.IsLTP = true;
                        //_depth.Add(dd);
                        _depth[i] = dd;

                        i++;

                        // 100円単位でまとめる
                        // まとめた時の価格
                        decimal c = 0;
                        // 100単位ごとにまとめた数量を保持
                        t = 0;
                        // 先送りするBid
                        d = 0;
                        // 先送りする価格
                        e = 0;

                        // bid をループ
                        foreach (var dp in dpr.DepthBidList)
                        {

                            if (unit > 0)
                            {

                                //if (i >= listCount) break;

                                //if (c == 0) c = dp.DepthPrice;
                                if (c == 0) c = System.Math.Ceiling(dp.DepthPrice / unit);//c = dp.DepthPrice;

                                // 100円単位でまとめる
                                if (System.Math.Ceiling(dp.DepthPrice / unit) == c)
                                {
                                    t = t + dp.DepthBid;
                                }
                                else
                                {
                                    /*
                                    // Bidの一番初めである。
                                    if (i == 0)
                                    {
                                        dp.IsBidBest = true;
                                    }
                                    */

                                    // 一時保存
                                    e = dp.DepthPrice;
                                    dp.DepthPrice = (c * unit) - unit;

                                    // 一時保存
                                    d = dp.DepthBid;
                                    dp.DepthBid = t;

                                    // 追加
                                    //_depth.Add(dp);
                                    //_depth[i] = dp;
                                    _depth[i].DepthAsk = dp.DepthAsk;
                                    _depth[i].DepthBid = dp.DepthBid;
                                    _depth[i].DepthPrice = dp.DepthPrice;

                                    // 今回のBidは先送り
                                    t = d;
                                    // 今回のPriceが基準になる
                                    c = System.Math.Ceiling(e / unit);//e;


                                    i++;
                                }
                            }
                            else
                            {
                                //_depth.Add(dp);
                                _depth[i] = dp;
                                i++;
                            }

                        }

                        /*
                        foreach (var dp in dpr.DepthBidList)
                        {
                            _depth.Add(dp);
                        }
                        */

                        _depth[half+1].IsBidBest = true;

                    }

                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ GetDepth returned null");
                    return false;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetDepth Exception: " + e);
                return false;
            }
        }

        // 板情報の更新ループ
        private async void UpdateDepth()
        {
            while (true)
            {
                // 省エネモードならスルー。
                if (MinMode)
                {
                    await Task.Delay(4000);
                    continue;
                }

                // 間隔 1/2
                await Task.Delay(600);

                try
                {
                    await GetDepth();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ UpdateDepth Exception: " + e);
                }

                // 間隔 1/2
                await Task.Delay(600);
            }

        }

        // トランザクションの取得
        private async Task<bool> GetTransactions()
        {
            try
            {
                TransactionsResult trs = await _pubTransactionsApi.GetTransactions();

                if (trs != null)
                {

                    //Debug.WriteLine(trs.Trans.Count.ToString());

                    if (_transactions.Count == 0)
                    {
                        // 60 で初期化
                        for (int i = 0; i < 60; i++)
                        {
                            Transactions dd = new Transactions();
                            //
                            _transactions.Add(dd);
                        }
                    }

                    int v = 0;
                    foreach (var tr in trs.Trans)
                    {
                        //_transactions[v] = tr;

                        _transactions[v].Amount = tr.Amount;
                        _transactions[v].ExecutedAt = tr.ExecutedAt;
                        _transactions[v].Price = tr.Price;
                        _transactions[v].Side = tr.Side;
                        _transactions[v].TransactionId = tr.TransactionId;

                        v++;
                    }

                    /*
                    _transactions.Clear();

                    foreach (var tr in trs.Trans)
                    {
                        _transactions.Add(tr);
                    }
                    */

                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ GetTransactions returned null");
                    return false;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetTransactions Exception: " + e);
                return false;
            }
        }

        // トランザクションの更新ループ
        private async void UpdateTransactions()
        {
            while (true)
            {
                // 省エネモードならスルー。
                if (MinMode)
                {
                    await Task.Delay(5000);
                    continue;
                }

                // 間隔 1/2
                await Task.Delay(900);

                try
                {
                    await GetTransactions();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ UpdateTransactions Exception: " + e);
                }

                // 間隔 1/2
                await Task.Delay(1300);
            }

        }

        // 取引履歴の取得
        private async Task<bool> GetTradeHistoryList()
        {

            if (TradeHistoryApiKeyIsSet == false)
            {
                // TODO show message?
                return false;
            }

            //System.Diagnostics.Debug.WriteLine("GetTradeHistory......");
            try
            {

                //TradeHistory trd = await _priTradeHistoryApi.GetTradeHistory();
                TradeHistory trd = await _priApi.GetTradeHistory(_getTradeHistoryApiKey, _getTradeHistorySecret);

                if (trd != null)
                {
                    // TODO: tmp
                    //_trades.Clear();

                    // 逆順にする
                    trd.TradeList.Reverse();

                    foreach (var tr in trd.TradeList)
                    {
                        //_trades.Add(tr);

                        var found = _trades.FirstOrDefault(x => x.TradeID == tr.TradeID);
                        if (found == null)
                        {
                            _trades.Insert(0, tr);
                        }

                    }

                    _tradeHistories = _trades.Count;
                    NotifyPropertyChanged(nameof(TradeHistoryTitle));

                    APIResultTradeHistory = "";
                    //NotifyPropertyChanged(nameof(APIResultTradeHistory));

                    return true;
                }
                else
                {
                    _tradeHistories = -1;
                    NotifyPropertyChanged(nameof(TradeHistoryTitle));

                    APIResultTradeHistory = "<<取得失敗>>";
                    //NotifyPropertyChanged(nameof(APIResultTradeHistory));

                    return false;
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetTradeHistoryList Exception: " + e);

                return false;
            }

        }

        // 取引履歴の更新ループ
        private async void UpdateTradeHistory()
        {
            while (true)
            {
                // ログインしていなかったらスルー。
                if (LoggedInMode == false)
                {
                    await Task.Delay(2000);
                    continue;
                }
                // 省エネモードならスルー。
                if (MinMode)
                {
                    await Task.Delay(6000);
                    continue;
                }
                if (TradeHistoryApiKeyIsSet == false)
                {
                    await Task.Delay(6000);
                    continue;
                }

                // 間隔 1/2
                await Task.Delay(1100);

                try
                {
                    await GetTradeHistoryList();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ UpdateTradeHistory Exception: " + e);
                }

                // 間隔 1/2
                await Task.Delay(2500);
            }

        }

        // RSSの取得
        private void GetRss()
        {
            try
            {
                RssResult rs = _rssCli.GetRSS(Langs.en);

                //ords.OrderList.Reverse();

                if (rs != null)
                {
                    foreach (var rss in rs.RssList)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var found = _bitcoinNewsEn.FirstOrDefault(x => x.Link == rss.Link);
                            if (found == null)
                            {
                                _bitcoinNewsEn.Add(rss);
                            }
                        });
                    }

                }


            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetRss Exception: " + e);
            }


            try
            {
                RssResult rs = _rssCli.GetRSS(Langs.ja);

                //ords.OrderList.Reverse();

                if (rs != null)
                {
                    foreach (var rss in rs.RssList)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var found = _bitcoinNewsJa.FirstOrDefault(x => x.Link == rss.Link);
                            if (found == null)
                            {
                                _bitcoinNewsJa.Add(rss);
                            }
                        });
                    }
                }


            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetRss Exception: " + e);
            }
        }

        #endregion

        #endregion

        #region == コマンド ==

        // 最小表示エコ view モード
        public ICommand ViewMinimumCommand { get; }
        public bool ViewMinimumCommand_CanExecute()
        {
            return true;
        }
        public void ViewMinimumCommand_Execute()
        {
            //System.Diagnostics.Debug.WriteLine("■■■■■ ViewMinimumCommand_Execute.");

            if (!MinMode)
            {
                MinMode = true;
            }
        }

        // 最小表示エコ view モード　復帰
        public ICommand ViewRestoreCommand { get; }
        public bool ViewRestoreCommand_CanExecute()
        {
            return true;
        }
        public void ViewRestoreCommand_Execute()
        {

            //System.Diagnostics.Debug.WriteLine("■■■■■ ViewRestoreCommand_Execute.");

            if (MinMode)
            {
                MinMode = false;
            }
        }

        // 買い注文コマンド
        public ICommand BuyOrderCommand { get; }
        public bool BuyOrderCommand_CanExecute()
        {
            if (PublicApiOnlyMode == true) return false;

            return true;
            /*
            if (BuyAmount <= 0.001M)
            {
                return false;
            }

            if (_buyType == BuyTypes.limit)
            {
                if (BuyPrice > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
            */
        }
        public async void BuyOrderCommand_Execute()
        {

            // TODO: Input check.


            APIResultBuyCommandOrderIDString = "";
            APIResultBuyCommandErrorString = "";
            APIResultBuyCommandResult = "";

            string TypeStr;
            if (_buyType == OrderTypes.limit)
            {
                TypeStr = "limit";
            }
            else if (_buyType == OrderTypes.market)
            {
                TypeStr = "market";
                BuyPrice = 0;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ BuyOrderCommand_Execute _buyType undefined");
                return;
            }

            OrderResult result = await ManualOrder("btc_jpy", BuyAmount, BuyPrice, "buy", TypeStr);

            if (result != null)
            {
                if (result.IsSuccess == true)
                {
                    APIResultBuyCommandOrderIDString = result.OrderID.ToString(); // こっち先にセット！
                    APIResultBuyCommandErrorString = "";
                    APIResultBuyCommandResult = "成功"; // ここで結果表示
                }
                else
                {
                    APIResultBuyCommandOrderIDString = "";

                    if (result.HasErrorInfo)
                    {
                        APIResultBuyCommandErrorString = result.Err.ErrorDescription;
                        APIResultBuyCommandResult = "失敗 - " + result.Err.ErrorTitle;
                    }
                    else
                    {
                        APIResultBuyCommandErrorString = "";
                        APIResultBuyCommandResult = "失敗";
                    }
                }
            }
            else
            {
                APIResultBuyCommandErrorString = "通信エラーが起きました。";
                APIResultBuyCommandResult = "失敗";
            }
        }

        // 売り注文コマンド
        public ICommand SellOrderCommand { get; }
        public bool SellOrderCommand_CanExecute()
        {
            if (PublicApiOnlyMode == true) return false;

            return true;
            /*
            if (SellAmount <= 0.001M)
            {
                return false;
            }

            if (_sellType == SellTypes.limit)
            {
                if (SellPrice > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
            */
        }
        public async void SellOrderCommand_Execute()
        {

            // TODO: Input check.

            APIResultSellCommandOrderIDString = "";
            APIResultSellCommandErrorString = "";
            APIResultSellCommandResult = "";


            string TypeStr;
            if (_sellType == OrderTypes.limit)
            {
                TypeStr = "limit";
            }
            else if (_sellType == OrderTypes.market)
            {
                TypeStr = "market";
            }
            else
            {
                return;
            }

            OrderResult result = await ManualOrder("btc_jpy", _sellAmount, _sellPrice, "sell", TypeStr);

            if (result != null)
            {
                if (result.IsSuccess == true)
                {
                    APIResultSellCommandOrderIDString = result.OrderID.ToString();// こっち先にセット！
                    APIResultSellCommandErrorString = "";
                    APIResultSellCommandResult = "成功"; // ここで結果表示
                }
                else
                {
                    APIResultSellCommandOrderIDString = "";

                    if (result.HasErrorInfo)
                    {
                        APIResultSellCommandErrorString = result.Err.ErrorDescription;
                        APIResultSellCommandResult = "失敗 - " + result.Err.ErrorTitle;
                    }
                    else
                    {
                        APIResultSellCommandErrorString = "";
                        APIResultSellCommandResult = "失敗";
                    }
                }
            }
            else
            {
                APIResultSellCommandErrorString = "通信エラーが起きました。";
                APIResultSellCommandResult = "失敗";
            }


        }

        // 注文一覧リストビュー内の：注文キャンセルコマンド
        public ICommand CancelOrderListviewCommand { get; }
        public bool CancelOrderListviewCommand_CanExecute()
        {
            if (PublicApiOnlyMode == true) return false;

            return true;
        }
        public async void CancelOrderListviewCommand_Execute(object obj)
        {
            // obj == System.Windows.Controls.SelectedItemCollection

            if (obj == null) return;

            // 選択注文アイテム保持用
            Orders ords = new Orders();

            // System.Windows.Controls.SelectedItemCollection をキャストして、ループ
            System.Collections.IList items = (System.Collections.IList)obj;
            var collection = items.Cast<Order>();

            foreach (var item in collection)
            {
                // アイテム追加
                ords.OrderList.Add(item as Order);
            }

            // 選択注文アイテムをループして、キャンセル処理
            foreach (var item in ords.OrderList)
            {

                if (item.IsCancelEnabled == false)
                    continue;

                Order ord = item as Order;

                System.Diagnostics.Debug.WriteLine("CancelOrderListviewCommand_Execute...: " + ord.OrderID.ToString());

                OrderResult result = await CancelOrder("btc_jpy", ord.OrderID);


                if (result.HasErrorInfo)
                {
                    ord.HasErrorInfo = true;
                    ord.Err.ErrorTitle = result.Err.ErrorTitle;
                    ord.Err.ErrorDescription = result.Err.ErrorDescription;
                    ord.Err.ErrorCode = result.Err.ErrorCode;

                }
                else
                {
                    ord.HasErrorInfo = false;
                    ord.Err.ErrorTitle = "";
                    ord.Err.ErrorDescription = result.Err.ErrorDescription = "";
                    ord.Err.ErrorCode = 0;

                }

            }


        }

        // 注文一覧リストビュー内の：済みアイテムの削除コマンド
        public ICommand RemoveDoneOrderListviewCommand { get; }
        public bool RemoveDoneOrderListviewCommand_CanExecute()
        {
            return true;
        }
        public void RemoveDoneOrderListviewCommand_Execute()
        {
            int c = ActiveOrders.Count - 1;

            for (int i = c; i >= 0; i--)
            {
                if ((ActiveOrders[i].Status == "FULLY_FILLED") || (ActiveOrders[i].Status == "CANCELED_UNFILLED") || (ActiveOrders[i].Status == "CANCELED_PARTIALLY_FILLED"))
                {
                    ActiveOrders.Remove(ActiveOrders[i]);
                }

            }

        }

        // 手動で 取引履歴の取得コマンド
        public ICommand GetTradeHistoryListCommand { get; }
        public bool GetTradeHistoryListCommand_CanExecute()
        {
            if (PublicApiOnlyMode == true) return false;

            return true;
        }
        public void GetTradeHistoryListCommand_Execute()
        {
            Task.Run(() => GetTradeHistoryList());
        }

        // 手動で 資産の取得コマンド
        public ICommand GetAssetsCommand { get; }
        public bool GetAssetsCommand_CanExecute()
        {
            if (PublicApiOnlyMode == true) return false;

            return true;
        }
        public void GetAssetsCommand_Execute()
        {
            Task.Run(() => GetAssets());
        }

        // 手動で 注文一覧の取得コマンド
        public ICommand GetOrderListCommand { get; }
        public bool GetOrderListCommand_CanExecute()
        {
            if (PublicApiOnlyMode == true) return false;

            return true;
        }
        public void GetOrderListCommand_Execute()
        {
            Task.Run(() => GetOrderList());
        }

        // IDF 注文
        public ICommand IfdOrderCommand { get; }
        public bool IfdOrderCommand_CanExecute()
        {
            if (PublicApiOnlyMode == true) return false;

            return true;
            /*
            bool bln = true;

            if (IFD_IfdAmount <= 0.001M)
            {
                bln = false;
            }

            if (IFD_IfdType == IfdocoTypes.limit)
            {
                if (IFD_IfdPrice > 0)
                {
                    bln = true;
                }
                else
                {
                    bln = false;
                }
            }

            if (IFD_DoAmount <= 0.001M)
            {
                bln = false;
            }

            if (IFD_DoType == IfdocoTypes.limit)
            {
                if (IFD_DoPrice > 0)
                {
                    bln = true;
                }
                else
                {
                    bln = false;
                }
            }

            return bln;
            */
        }
        public async void IfdOrderCommand_Execute()
        {
            //System.Diagnostics.Debug.WriteLine("IfdOrderCommand...");

            IFDOrderCommandResult = "";

            if (IfdocoTradeApiKeyIsSet == false)
            {
                // TODO show message?
                return;
            }

            // Input check.
            if (IFD_IfdAmount <= 0)
            {
                IFDOrderCommandErrorString = "数量が0です。";
                return;
            }

            if (IFD_IfdPrice <= 0)
            {
                IFDOrderCommandErrorString = "価格が0です。";
                return;
            }

            if (IFD_DoAmount <= 0)
            {
                IFDOrderCommandErrorString = "数量が0です。";
                return;
            }

            if (IFD_DoPrice <= 0)
            {
                IFDOrderCommandErrorString = "価格が0です。";
                return;
            }

            if (IFD_DoTriggerPrice <= 0)
            {
                IFDOrderCommandErrorString = "トリガーが0です。";
                return;
            }

            Ifdoco ifdoco = new Ifdoco();

            // IFDで
            ifdoco.Kind = IfdocoKinds.ifd;

            // If done
            ifdoco.IfdoneType = IFD_IfdType;
            ifdoco.IfdoneSide = IFD_IfdSide.ToString();
            ifdoco.IfdoneStartAmount = IFD_IfdAmount;
            ifdoco.IfdonePrice = IFD_IfdPrice;

            // Do
            ifdoco.IfdDoType = IFD_DoType;
            ifdoco.IfdDoSide = IFD_DoSide.ToString();
            ifdoco.IfdDoStartAmount = IFD_DoAmount;
            ifdoco.IfdDoPrice = IFD_DoPrice;

            ifdoco.IfdDoTriggerPrice = IFD_DoTriggerPrice;
            ifdoco.IfdDoTriggerUpDown = IFD_DoTriggerUpDown;

            // Ifd
            OrderResult ord = await _priApi.MakeOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, "btc_jpy", ifdoco.IfdoneStartAmount, ifdoco.IfdonePrice, ifdoco.IfdoneSide, ifdoco.IfdoneType.ToString());

            if (ord != null)
            {
                if (ord.IsSuccess)
                {
                    ifdoco.IfdoneHasError = false;

                    ifdoco.IfdoneOrderID = ord.OrderID;
                    ifdoco.IfdoneOrderedAt = ord.OrderedAt;
                    ifdoco.IfdonePrice = ord.Price;
                    ifdoco.IfdoneAveragePrice = ord.AveragePrice;
                    ifdoco.IfdoneStatus = ord.Status;
                    // TODO

                    // 注文画面に結果を表示
                    IFDOrderCommandResult = "成功。";

                    // 約定
                    if (ord.Status == "FULLY_FILLED")
                    {
                        // フラグをセット
                        ifdoco.IfdoneIsDone = true;

                        // 後は、UpdateIFDOCOループにまかせる

                    }
                }
                else
                {
                    ifdoco.IfdoneHasError = true;
                    if (ifdoco.IfdoneErrorInfo == null)
                    {
                        ifdoco.IfdoneErrorInfo = new ErrorInfo();
                    }
                    ifdoco.IfdoneErrorInfo.ErrorTitle = ord.Err.ErrorTitle;
                    ifdoco.IfdoneErrorInfo.ErrorDescription = ord.Err.ErrorDescription;
                    ifdoco.IfdoneErrorInfo.ErrorCode = ord.Err.ErrorCode;

                    System.Diagnostics.Debug.WriteLine("IfdOrderCommand_Execute - Ifdone MakeOrder API failed");

                    // 注文画面でエラーを表示
                    IFDOrderCommandResult = "エラーが起きました。一覧を参照ください。";
                }

            }
            else
            {
                ifdoco.IfdoneHasError = true;
                if (ifdoco.IfdoneErrorInfo == null)
                {
                    ifdoco.IfdoneErrorInfo = new ErrorInfo();
                }
                ifdoco.IfdoneErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                ifdoco.IfdoneErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                ifdoco.IfdoneErrorInfo.ErrorCode = -1;

                System.Diagnostics.Debug.WriteLine("IfdOrderCommand_Execute - Ifdone MakeOrder returened NULL");

                // 注文画面でエラーを表示
                IFDOrderCommandResult = "エラーが起きました。一覧を参照ください。";
            }

            // エラーをクリア
            IFDOrderCommandErrorString = "";

            // リストビューに追加
            Application.Current.Dispatcher.Invoke(() =>
            {
                _ifdocos.Insert(0, ifdoco);
            });


        }

        // OCO 注文
        public ICommand OcoOrderCommand { get; }
        public bool OcoOrderCommand_CanExecute()
        {
            if (PublicApiOnlyMode == true) return false;

            return true;
            /*
            bool bln = true;

            if (OCO_OneAmount <= 0.001M)
            {
                bln = false;
            }

            if (OCO_OneType == IfdocoTypes.limit)
            {
                if (OCO_OnePrice > 0)
                {
                    bln = true;
                }
                else
                {
                    bln = false;
                }
            }

            if (OCO_OtherAmount <= 0.001M)
            {
                bln = false;
            }

            if (OCO_OtherType == IfdocoTypes.limit)
            {
                if (OCO_OtherPrice > 0)
                {
                    bln = true;
                }
                else
                {
                    bln = false;
                }
            }

            return bln;
            */
        }
        public void OcoOrderCommand_Execute()
        {
            //System.Diagnostics.Debug.WriteLine("OcoOrderCommand...");

            OcoOrderCommandResult = "";

            if (IfdocoTradeApiKeyIsSet == false)
            {
                //System.Diagnostics.Debug.WriteLine("IfdocoTradeApiKeyIsSet not set...");
                return;
            }

            // Input check.
            if (OCO_OneAmount <= 0)
            {
                OcoOrderCommandErrorString = "数量が0です。";
                return;
            }

            if (OCO_OnePrice <= 0)
            {
                OcoOrderCommandErrorString = "価格が0です。";
                return;
            }

            if (OCO_OtherAmount <= 0)
            {
                OcoOrderCommandErrorString = "数量が0です。";
                return;
            }

            if (OCO_OtherPrice <= 0)
            {
                OcoOrderCommandErrorString = "価格が0です。";
                return;
            }

            if (OCO_OneTriggerPrice <= 0)
            {
                OcoOrderCommandErrorString = "トリガーが0です。";
                return;
            }

            if (OCO_OtherTriggerPrice <= 0)
            {
                OcoOrderCommandErrorString = "トリガーが0です。";
                return;
            }

            Ifdoco ifdoco = new Ifdoco();

            // OCOで
            ifdoco.Kind = IfdocoKinds.oco;

            // One
            ifdoco.OcoOneType = OCO_OneType;
            ifdoco.OcoOneSide = OCO_OneSide.ToString();
            ifdoco.OcoOneStartAmount = OCO_OneAmount;
            ifdoco.OcoOnePrice = OCO_OnePrice;

            ifdoco.OcoOneTriggerPrice = OCO_OneTriggerPrice;
            ifdoco.OcoOneTriggerUpDown = OCO_OneTriggerUpDown;

            // Other
            ifdoco.OcoOtherType = OCO_OtherType;
            ifdoco.OcoOtherSide = OCO_OtherSide.ToString();
            ifdoco.OcoOtherStartAmount = OCO_OtherAmount;
            ifdoco.OcoOtherPrice = OCO_OtherPrice;

            ifdoco.OcoOtherTriggerPrice = OCO_OtherTriggerPrice;
            ifdoco.OcoOtherTriggerUpDown = OCO_OtherTriggerUpDown;

            /*

            // OcoOne
            OrderResult ord = await _priApi.MakeOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, "btc_jpy", ifdoco.OcoOneStartAmount, ifdoco.OcoOnePrice, ifdoco.OcoOneSide, ifdoco.OcoOneType.ToString());

            if (ord != null)
            {
                if (ord.IsSuccess)
                {
                    ifdoco.OcoOneHasError = false;

                    ifdoco.OcoOneOrderID = ord.OrderID;
                    ifdoco.OcoOneOrderedAt = ord.OrderedAt;
                    ifdoco.OcoOnePrice = ord.Price;
                    ifdoco.OcoOneAveragePrice = ord.AveragePrice;
                    ifdoco.OcoOneStatus = ord.Status;
                    // TODO

                    // 注文画面に結果を表示
                    OcoOrderCommandResult = "成功。";

                    // 約定
                    if (ord.Status == "FULLY_FILLED")
                    {
                        // フラグをセット
                        ifdoco.OcoOneIsDone = true;

                        // 後は、UpdateIFDOCOループにまかせる
                    }
                }
                else
                {
                    ifdoco.OcoOneHasError = true;
                    if (ifdoco.OcoOneErrorInfo == null)
                    {
                        ifdoco.OcoOneErrorInfo = new ErrorInfo();
                    }
                    ifdoco.OcoOneErrorInfo.ErrorTitle = ord.Err.ErrorTitle;
                    ifdoco.OcoOneErrorInfo.ErrorDescription = ord.Err.ErrorDescription;
                    ifdoco.OcoOneErrorInfo.ErrorCode = ord.Err.ErrorCode;

                    System.Diagnostics.Debug.WriteLine("OcoOrderCommand_Execute - OcoOne MakeOrder API failed");

                    // 注文画面に結果を表示
                    OcoOrderCommandErrorString = "エラーが起きました。一覧を参照ください。";
                }

            }
            else
            {
                ifdoco.OcoOneHasError = true;
                if (ifdoco.OcoOneErrorInfo == null)
                {
                    ifdoco.OcoOneErrorInfo = new ErrorInfo();
                }
                ifdoco.OcoOneErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                ifdoco.OcoOneErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                ifdoco.OcoOneErrorInfo.ErrorCode = -1;

                System.Diagnostics.Debug.WriteLine("OcoOrderCommand_Execute - OcoOne MakeOrder returened NULL");

                // 注文画面に結果を表示
                OcoOrderCommandErrorString = "エラーが起きました。一覧を参照ください。";
            }

            // OcoOther
            OrderResult ord2 = await _priApi.MakeOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, "btc_jpy", ifdoco.OcoOtherStartAmount, ifdoco.OcoOtherPrice, ifdoco.OcoOtherSide, ifdoco.OcoOtherType.ToString());

            if (ord2 != null)
            {

                if (ord2.IsSuccess)
                {
                    ifdoco.OcoOtherHasError = false;

                    ifdoco.OcoOtherOrderID = ord2.OrderID;
                    ifdoco.OcoOtherOrderedAt = ord2.OrderedAt;
                    ifdoco.OcoOtherPrice = ord2.Price;
                    ifdoco.OcoOtherAveragePrice = ord2.AveragePrice;
                    ifdoco.OcoOtherStatus = ord2.Status;
                    // TODO

                    // 約定
                    if (ord2.Status == "FULLY_FILLED")
                    {
                        

                        // フラグをセット
                        ifdoco.OcoOtherIsDone = true;


                        // 後は、UpdateIFDOCOループにまかせる

                    }
                }
                else
                {
                    ifdoco.OcoOtherHasError = true;
                    if (ifdoco.OcoOtherErrorInfo == null)
                    {
                        ifdoco.OcoOtherErrorInfo = new ErrorInfo();
                    }
                    ifdoco.OcoOtherErrorInfo.ErrorTitle = ord2.Err.ErrorTitle;
                    ifdoco.OcoOtherErrorInfo.ErrorDescription = ord2.Err.ErrorDescription;
                    ifdoco.OcoOtherErrorInfo.ErrorCode = ord2.Err.ErrorCode;

                    System.Diagnostics.Debug.WriteLine("OcoOrderCommand_Execute - OcoOther MakeOrder API failed");
                }

            }
            else
            {
                ifdoco.OcoOtherHasError = true;
                if (ifdoco.OcoOtherErrorInfo == null)
                {
                    ifdoco.OcoOtherErrorInfo = new ErrorInfo();
                }
                ifdoco.OcoOtherErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                ifdoco.OcoOtherErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                ifdoco.OcoOtherErrorInfo.ErrorCode = -1;

                System.Diagnostics.Debug.WriteLine("OcoOrderCommand_Execute - OcoOther MakeOrder returened NULL");
            }

            */

            OcoOrderCommandErrorString = "";

            // リストビューに追加
            Application.Current.Dispatcher.Invoke(() =>
            {
                _ifdocos.Insert(0, ifdoco);

                OcoOrderCommandResult = "成功。";
            });

        }

        // IFDOCO注文
        public ICommand IfdocoOrderCommand { get; }
        public bool IfdocoOrderCommand_CanExecute()
        {
            if (PublicApiOnlyMode == true) return false;

            return true;
            /*
            bool bln = true;

            if (IFDOCO_IfdAmount <= 0.001M)
            {
                bln = false;
            }

            if (IFDOCO_IfdType == IfdocoTypes.limit)
            {
                if (IFDOCO_IfdPrice > 0)
                {
                    bln = true;
                }
                else
                {
                    bln = false;
                }
            }

            if (IFDOCO_OneAmount <= 0.001M)
            {
                bln = false;
            }

            if (IFDOCO_OneType == IfdocoTypes.limit)
            {
                if (OCO_OnePrice > 0)
                {
                    bln = true;
                }
                else
                {
                    bln = false;
                }
            }

            if (IFDOCO_OtherAmount <= 0)
            {
                bln = false;
            }

            if (IFDOCO_OtherType == IfdocoTypes.limit)
            {
                if (IFDOCO_OtherPrice > 0)
                {
                    bln = true;
                }
                else
                {
                    bln = false;
                }
            }

            return bln;
            */
        }
        public async void IfdocoOrderCommand_Execute()
        {
            //System.Diagnostics.Debug.WriteLine("IfdocoOrderCommand...");

            IfdocoOrderCommandResult = "";

            if (IfdocoTradeApiKeyIsSet == false)
            {
                // TODO show message?
                return;
            }

            // Input check.

            if (IFDOCO_IfdAmount <= 0)
            {
                IfdocoOrderCommandErrorString = "数量が0です。";
                return;
            }

            if (IFDOCO_IfdPrice <= 0)
            {
                IfdocoOrderCommandErrorString = "価格が0です。";
                return;
            }

            if (IFDOCO_OneAmount <= 0)
            {
                IfdocoOrderCommandErrorString = "数量が0です。";
                return;
            }

            if (IFDOCO_OnePrice <= 0)
            {
                IfdocoOrderCommandErrorString = "価格が0です。";
                return;
            }

            if (IFDOCO_OtherAmount <= 0)
            {
                IfdocoOrderCommandErrorString = "数量が0です。";
                return;
            }

            if (IFDOCO_OtherPrice <= 0)
            {
                IfdocoOrderCommandErrorString = "価格が0です。";
                return;
            }

            if (IFDOCO_OneTriggerPrice <= 0)
            {
                IfdocoOrderCommandErrorString = "トリガーが0です。";
                return;
            }

            if (IFDOCO_OtherTriggerPrice <= 0)
            {
                IfdocoOrderCommandErrorString = "トリガーが0です。";
                return;
            }

            // TODO: 利用可能の額をチェック

            Ifdoco ifdoco = new Ifdoco();

            // IFDOCOで
            ifdoco.Kind = IfdocoKinds.ifdoco;


            /// -
            ifdoco.IfdoneType = IFDOCO_IfdType;
            ifdoco.IfdoneSide = IFDOCO_IfdSide.ToString();
            ifdoco.IfdoneStartAmount = IFDOCO_IfdAmount;
            ifdoco.IfdonePrice = IFDOCO_IfdPrice;

            /// -
            ifdoco.OcoOneType = IFDOCO_OneType;
            ifdoco.OcoOneSide = IFDOCO_OneSide.ToString();
            ifdoco.OcoOneStartAmount = IFDOCO_OneAmount;
            ifdoco.OcoOnePrice = IFDOCO_OnePrice;

            ifdoco.OcoOneTriggerPrice = IFDOCO_OneTriggerPrice;
            ifdoco.OcoOneTriggerUpDown = IFDOCO_OneTriggerUpDown;

            /// -
            ifdoco.OcoOtherType = IFDOCO_OtherType;
            ifdoco.OcoOtherSide = IFDOCO_OtherSide.ToString();
            ifdoco.OcoOtherStartAmount = IFDOCO_OtherAmount;
            ifdoco.OcoOtherPrice = IFDOCO_OtherPrice;

            ifdoco.OcoOtherTriggerPrice = IFDOCO_OtherTriggerPrice;
            ifdoco.OcoOtherTriggerUpDown = IFDOCO_OtherTriggerUpDown;


            System.Diagnostics.Debug.WriteLine("IfdOrderCommand_Execute - " + "start:" + ifdoco.IfdoneStartAmount.ToString() + ", " + ifdoco.IfdonePrice.ToString());
            
            //Ifdoco
            OrderResult ord = await _priApi.MakeOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, "btc_jpy", ifdoco.IfdoneStartAmount, ifdoco.IfdonePrice, ifdoco.IfdoneSide, ifdoco.IfdoneType.ToString());

            if (ord != null)
            {

                if (ord.IsSuccess)
                {
                    ifdoco.IfdoneHasError = false;

                    ifdoco.IfdoneOrderID = ord.OrderID;
                    ifdoco.IfdoneOrderedAt = ord.OrderedAt;
                    ifdoco.IfdoneStartAmount = ord.StartAmount;
                    ifdoco.IfdonePrice = ord.Price;
                    ifdoco.IfdoneAveragePrice = ord.AveragePrice;
                    ifdoco.IfdoneStatus = ord.Status;
                    // TODO

                    IfdocoOrderCommandResult = "成功。";

                    // 約定
                    if (ord.Status == "FULLY_FILLED")
                    {
                        
                        // フラグをセット
                        ifdoco.IfdoneIsDone = true;

                        // 後は、UpdateIFDOCOループにまかせる

                    }
                }
                else
                {
                    ifdoco.IfdoneHasError = true;
                    if (ifdoco.IfdoneErrorInfo == null)
                    {
                        ifdoco.IfdoneErrorInfo = new ErrorInfo();
                    }
                    ifdoco.IfdoneErrorInfo.ErrorTitle = ord.Err.ErrorTitle;
                    ifdoco.IfdoneErrorInfo.ErrorDescription = ord.Err.ErrorDescription;
                    ifdoco.IfdoneErrorInfo.ErrorCode = ord.Err.ErrorCode;

                    System.Diagnostics.Debug.WriteLine("IfdOrderCommand_Execute - Ifdone MakeOrder API failed");
                }

            }
            else
            {
                ifdoco.IfdoneHasError = true;
                if (ifdoco.IfdoneErrorInfo == null)
                {
                    ifdoco.IfdoneErrorInfo = new ErrorInfo();
                }
                ifdoco.IfdoneErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                ifdoco.IfdoneErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                ifdoco.IfdoneErrorInfo.ErrorCode = -1;

                System.Diagnostics.Debug.WriteLine("IfdOrderCommand_Execute - Ifdone MakeOrder returened NULL");
            }

            IfdocoOrderCommandErrorString = "";

            // リストビューに追加
            Application.Current.Dispatcher.Invoke(() =>
            {
                _ifdocos.Insert(0, ifdoco);
            });


        }

        // 特殊注文リストビュー内の：注文キャンセルコマンド
        public ICommand CancelIfdocoListviewCommand { get; }
        public bool CancelIfdocoListviewCommand_CanExecute()
        {

            return true;
        }
        public async void CancelIfdocoListviewCommand_Execute(object obj)
        {
            //Debug.WriteLine("CancelIfdocoListviewCommand_Execute");

            if (IfdocoTradeApiKeyIsSet == false)
            {
                // TODO show message?
                return;
            }

            if (obj == null) return;

            // obj == System.Windows.Controls.SelectedItemCollection

            // 選択注文アイテム保持用
            List<Ifdoco> ifdocoList = new List<Ifdoco>();
            // キャンセルする注文IDを保持
            List<int> cancelIdList = new List<int>();

            // System.Windows.Controls.SelectedItemCollection をキャストして、ループ
            System.Collections.IList items = (System.Collections.IList)obj;
            var collection = items.Cast<Ifdoco>();

            foreach (var item in collection)
            {
                // アイテム追加
                ifdocoList.Add(item as Ifdoco);
            }

            // 選択注文アイテムをループして、キャンセル処理
            foreach (var item in ifdocoList)
            {
                if (item.IsCancelEnabled == false)
                {
                    continue;
                }

                if (item.Kind == IfdocoKinds.ifd)
                {
                    if ((item.IfdoneStatus == "UNFILLED") || (item.IfdoneStatus == "PARTIALLY_FILLED"))
                    {
                        //cancel
                        cancelIdList.Add(item.IfdoneOrderID);
                    }
                    else if (item.IfdoneHasError)
                    {
                        // エラーがあるのを済みにする。
                        item.IfdoneIsDone = true;
                    }

                    if ((item.IfdDoStatus == "UNFILLED") || (item.IfdDoStatus == "PARTIALLY_FILLED"))
                    {
                        //cancel
                        cancelIdList.Add(item.IfdDoOrderID);
                    }
                    else if (item.IfdDoHasError)
                    {
                        // エラーがあるのを済みにする。
                        item.IfdDoIsDone = true;
                    }

                    // 未発注なら済みにする。
                    if (item.IfdDoOrderID == 0)
                    {
                        item.IfdDoIsDone = true;
                    }

                }
                else if (item.Kind == IfdocoKinds.oco)
                {
                    if ((item.OcoOneStatus == "UNFILLED") || (item.OcoOneStatus == "PARTIALLY_FILLED"))
                    {
                        //cancel
                        cancelIdList.Add(item.OcoOneOrderID);
                    }
                    else if (item.OcoOneHasError)
                    {
                        // エラーがあるのを済みにする。
                        item.OcoOneIsDone = true;
                    }

                    if ((item.OcoOtherStatus == "UNFILLED") || (item.OcoOtherStatus == "PARTIALLY_FILLED"))
                    {
                        //cancel
                        cancelIdList.Add(item.OcoOtherOrderID);
                    }
                    else if (item.OcoOtherHasError)
                    {
                        // エラーがあるのを済みにする。
                        item.OcoOtherIsDone = true;
                    }

                    // One未発注なら済みにする。
                    if(item.OcoOneOrderID == 0)
                    {
                        item.OcoOneIsDone = true;
                    }
                    // Other未発注なら済みにする。
                    if (item.OcoOtherOrderID == 0)
                    {
                        item.OcoOtherIsDone = true;
                    }
                    // 両方未発注ならOCO済みにする。
                    if ((item.OcoOneIsDone == false) && (item.OcoOneIsDone == false))
                    {
                        item.OcoIsDone = false;
                    }

                }
                else if (item.Kind == IfdocoKinds.ifdoco)
                {
                    if ((item.IfdoneStatus == "UNFILLED") || (item.IfdoneStatus == "PARTIALLY_FILLED"))
                    {
                        //cancel
                        cancelIdList.Add(item.IfdoneOrderID);
                    }
                    else if (item.IfdoneHasError)
                    {
                        // エラーがあるのを済みにする。
                        item.IfdoneIsDone = true;
                    }

                    if ((item.OcoOneStatus == "UNFILLED") || (item.OcoOneStatus == "PARTIALLY_FILLED"))
                    {
                        //cancel
                        cancelIdList.Add(item.OcoOneOrderID);
                    }
                    else if (item.OcoOneHasError)
                    {
                        // エラーがあるのを済みにする。
                        item.OcoOneIsDone = true;
                    }

                    if ((item.OcoOtherStatus == "UNFILLED") || (item.OcoOtherStatus == "PARTIALLY_FILLED"))
                    {
                        //cancel
                        cancelIdList.Add(item.OcoOtherOrderID);
                    }
                    else if (item.OcoOtherHasError)
                    {
                        // エラーがあるのを済みにする。
                        item.OcoOtherIsDone = true;
                    }

                    // One未発注なら済みにする。
                    if (item.OcoOneOrderID == 0)
                    {
                        item.OcoOneIsDone = true;
                    }
                    // Other未発注なら済みにする。
                    if (item.OcoOtherOrderID == 0)
                    {
                        item.OcoOtherIsDone = true;
                    }
                    // 両方未発注ならOCO済みにする。
                    if ((item.OcoOneIsDone == false) && (item.OcoOneIsDone == false))
                    {
                        item.OcoIsDone = false;
                    }

                }

                if (cancelIdList.Count > 0)
                {
                    // キャンセル実行
                    //Orders oup = await _priMakeOrderApi.GetOrderListByIDs("btc_jpy", cancelIdList);
                    Orders oup = await _priApi.CancelOrders(_ifdocoTradeApiKey, _ifdocoTradeSecret, "btc_jpy", cancelIdList);

                    // 後は update loopに任せる？

                }


            }
        }

        // 特殊注文リストビュー内の：済みアイテムの削除コマンド
        public ICommand RemoveDoneIfdocoListviewCommand { get; }
        public bool RemoveDoneIfdocoListviewCommand_CanExecute()
        {
            return true;
        }
        public void RemoveDoneIfdocoListviewCommand_Execute()
        {
            //Debug.WriteLine("RemoveDoneIfdocoListviewCommand_Execute");

            int c = Ifdocos.Count - 1;

            for (int i = c; i >= 0; i--)
            {
                if (Ifdocos[i].Kind == IfdocoKinds.ifd)
                {
                    if (Ifdocos[i].IfdIsDone)
                    {
                        Ifdocos.Remove(Ifdocos[i]);
                    }
                }
                else if (Ifdocos[i].Kind == IfdocoKinds.oco)
                {
                    if (Ifdocos[i].OcoIsDone)
                    {
                        Ifdocos.Remove(Ifdocos[i]);
                    }
                }
                else if (Ifdocos[i].Kind == IfdocoKinds.ifdoco)
                {
                    if (Ifdocos[i].IfdocoIsDone)
                    {
                        Ifdocos.Remove(Ifdocos[i]);
                    }
                }

            }

        }

        // 自動取引開始コマンド
        public ICommand StartAutoTradeCommand { get; }
        public bool StartAutoTradeCommand_CanExecute()
        {
            if (PublicApiOnlyMode)
                return false;
            else
                return true;
        }
        public async void StartAutoTradeCommand_Execute()
        {
            if (AutoTradeApiKeyIsSet == false)
            {
                // TODO show message?
                return;
            }

            //System.Diagnostics.Debug.WriteLine("Start Auto Trading...");

            // 情報表示数のリセット。
            AutoTradeActiveOrders = 0;
            AutoTradeSellOrders = 0;
            AutoTradeBuyOrders = 0;
            AutoTradeErrOrders = 0;

            // 損益表示リセット
            AutoTradeProfit = 0;

            // 上値制限セット
            if (AutoTradeUpperLimit == 0)
            AutoTradeUpperLimit = ((_ltp / 1000) * 1000) + 10000M;
            // 下値制限セット
            if (AutoTradeLowerLimit == 0)
            AutoTradeLowerLimit = ((_ltp / 1000) * 1000) - 10000M;

            // ベース価格
            decimal basePrice = ((_ask / 1000) * 1000);

            // センチネルの追加
            for (int i = 0; i < 15; i++)
            {

                AutoTrade2 position = new AutoTrade2();

                position.BuySide = "buy";
                position.BuyAmount = 0.0001M;
                position.SellSide = "sell";
                position.SellAmount = 0.0001M;

                if (i == 0)
                {
                    // 500 単位
                    position.BuyPrice = basePrice - 500M;
                    position.SellPrice = position.BuyPrice + 1000M;

                }
                else if (i == 1)
                {
                    // 500 単位
                    position.BuyPrice = basePrice - 1000M;
                    position.SellPrice = position.BuyPrice + 1000M;
                }
                else if (i == 2)
                {
                    // 1000 単位
                    position.BuyPrice = basePrice - 3000M;
                    position.SellPrice = position.BuyPrice + 1000M;
                }
                else if (i == 3)
                {
                    // 1000 単位
                    position.BuyPrice = basePrice - 5000M;
                    position.SellPrice = position.BuyPrice + 1000M;
                }
                else if (i == 4)
                {
                    // 1000 単位
                    position.BuyPrice = basePrice - 7000M;
                    position.SellPrice = position.BuyPrice + 1000M;
                }
                else if (i == 5)
                {
                    // 1000 単位
                    position.BuyPrice = basePrice - 9000M;
                    position.SellPrice = position.BuyPrice + 1000M;
                }
                else if (i == 6)
                {
                    // 1000 単位
                    position.BuyPrice = basePrice - 11000M;
                    position.SellPrice = position.BuyPrice + 1000M;
                }
                else if (i == 7)
                {
                    // 1500 単位
                    position.BuyPrice = basePrice - 12500M;
                    position.SellPrice = position.BuyPrice + 1000M;
                }
                else if (i == 8)
                {
                    // 2500 単位
                    position.BuyPrice = basePrice - 15000M;
                    position.SellPrice = position.BuyPrice + 1000M;
                }
                else if (i == 9)
                {
                    // 5000 単位
                    position.BuyPrice = basePrice - 20000M;
                    position.SellPrice = position.BuyPrice + 1000M;
                }
                else if (i <= 10)
                {
                    // 5000 単位
                    position.BuyPrice = basePrice - 25000M;
                    position.SellPrice = position.BuyPrice + 1000M;
                }
                else if (i == 11)
                {
                    // 5000 単位
                    position.BuyPrice = basePrice - 30000M;
                    position.SellPrice = position.BuyPrice + 1000M;
                }
                else if (i == 12)
                {
                    // 10000 単位
                    position.BuyPrice = basePrice - 35000M;
                    position.SellPrice = position.BuyPrice + 1000M;
                }
                else if (i == 13)
                {
                    // 10000 単位
                    position.BuyPrice = basePrice - 40000M;
                    position.SellPrice = position.BuyPrice + 1000M;
                }
                else if (i == 14)
                {
                    // 10000 単位
                    position.BuyPrice = basePrice - 45000M;
                    position.SellPrice = position.BuyPrice + 1000M;
                }

                position.ShushiAmount = (position.SellPrice * position.SellAmount) - (position.BuyPrice * position.BuyAmount);


                // 注文発注
                OrderResult res = await _priApi.MakeOrder(_autoTradeApiKey, _autoTradeSecret, "btc_jpy", position.BuyAmount, position.BuyPrice, position.BuySide, "limit");

                if (Application.Current == null) { return; }
                Application.Current.Dispatcher.Invoke(() =>
                {

                    if (res != null)
                    {
                        if (res.IsSuccess)
                        {

                            position.BuyHasError = false;

                            position.BuyOrderId = res.OrderID;
                            position.BuyFilledPrice = res.AveragePrice;
                            position.BuyStatus = res.Status;

                            // 約定
                            if (res.Status == "FULLY_FILLED")
                            {
                                // フラグをセット
                                position.BuyIsDone = true;
                            }

                            // リストヘ追加
                            _autoTrades2.Add(position);

                        }
                        else
                        {
                            position.BuyHasError = true;
                            if (position.BuyErrorInfo == null)
                            {
                                position.BuyErrorInfo = new ErrorInfo();
                            }
                            position.BuyErrorInfo.ErrorTitle = res.Err.ErrorTitle;
                            position.BuyErrorInfo.ErrorDescription = res.Err.ErrorDescription;
                            position.BuyErrorInfo.ErrorCode = res.Err.ErrorCode;

                            System.Diagnostics.Debug.WriteLine("■■■■■ StartAutoTradeCommand_Execute 新規注文ループ MakeOrder API returned error code.");
                        }
                    }
                    else
                    {
                        position.BuyHasError = true;
                        if (position.BuyErrorInfo == null)
                        {
                            position.BuyErrorInfo = new ErrorInfo();
                        }
                        position.BuyErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                        position.BuyErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                        position.BuyErrorInfo.ErrorCode = -1;

                        System.Diagnostics.Debug.WriteLine("■■■■■ StartAutoTradeCommand_Execute 新規注文ループ MakeOrder API returned null.");

                    }


                });

                await Task.Delay(100);

            }


            // 注文数のセット
            AutoTradeActiveOrders = _autoTrades2.Count;

            // 開始フラグセット
            AutoTradeStart = true;
            
            // 自動取引ループの開始
            //Task.Run(() => UpdateAutoTrade2());
            UpdateAutoTrade2();

        }

        // 自動取引停止コマンド
        public ICommand StopAutoTradeCommand { get; }
        public bool StopAutoTradeCommand_CanExecute()
        {
            if (PublicApiOnlyMode == true) return false;
            return true;
        }
        public async void StopAutoTradeCommand_Execute()
        {
            //System.Diagnostics.Debug.WriteLine("Stop Auto Trading.");

            if (AutoTradeApiKeyIsSet == false)
            {
                // TODO show message?
                return;
            }

            // 更新ループを止める。
            AutoTradeStart = false;

            await Task.Delay(1000);

            // 買い注文をすべてキャンセルする。

            List<int> needCancelIdsList = new List<int>();

            foreach (var position in _autoTrades2)
            {
                // 注文中だったらリスト追加
                if (position.BuyStatus == "UNFILLED" || position.BuyStatus == "PARTIALLY_FILLED")
                {
                    if (position.BuyOrderId > 0)
                    {
                        needCancelIdsList.Add(position.BuyOrderId);
                    }
                }

            }

            //System.Diagnostics.Debug.WriteLine("Cancelling Buy orders....");

            if (needCancelIdsList.Count > 0)
            {
                // CancelOrders
                Orders ord = await _priApi.CancelOrders(_autoTradeApiKey, _autoTradeSecret, "btc_jpy", needCancelIdsList);

                if (ord != null)
                {
                    if (ord.OrderList.Count > 0)
                    {
                        // TODO

                        await Task.Delay(3000);

                        try { 
                            // Just clear the list.
                            _autoTrades2.Clear();
                        }
                        catch { }
                    }
                }
                
            }

            // 情報表示のリセット
            AutoTradeActiveOrders = 0;
            AutoTradeSellOrders = 0;
            AutoTradeBuyOrders = 0;
            AutoTradeErrOrders = 0;


        }

        // 自動取引リストビュー内の：買い注文キャンセルコマンド
        public ICommand AutoTradeCancelListviewCommand { get; }
        public bool AutoTradeCancelListviewCommand_CanExecute()
        {
            return true;
        }
        public async void AutoTradeCancelListviewCommand_Execute(object obj)
        {
            //Debug.WriteLine("AutoTradeCancelListviewCommand_Execute");

            if (ManualTradeApiKeyIsSet == false)
            {
                // TODO show message?
                return;
            }

            if (obj == null) return;

            // obj == System.Windows.Controls.SelectedItemCollection

            // 選択注文アイテム保持用
            List<AutoTrade2> selectedList = new List<AutoTrade2>();
            // キャンセルする注文IDを保持
            List<int> cancelIdList = new List<int>();

            // System.Windows.Controls.SelectedItemCollection をキャストして、ループ
            System.Collections.IList items = (System.Collections.IList)obj;
            var collection = items.Cast<AutoTrade2>();

            foreach (var item in collection)
            {
                // アイテム追加
                selectedList.Add(item as AutoTrade2);
            }

            // 選択注文アイテムをループして、キャンセル処理
            foreach (var item in selectedList)
            {
                if (item.IsCanceled)
                {
                    continue;
                }

                if ((item.BuyStatus == "UNFILLED") || (item.BuyStatus == "PARTIALLY_FILLED"))
                {
                    //cancel
                    cancelIdList.Add(item.BuyOrderId);
                }

                if ((item.SellStatus == "UNFILLED") || (item.SellStatus == "PARTIALLY_FILLED"))
                {
                    //cancel
                    cancelIdList.Add(item.SellOrderId);
                }

                if (cancelIdList.Count > 0)
                {
                    // キャンセル実行
                    //Orders oup = await _priMakeOrderApi.GetOrderListByIDs("btc_jpy", cancelIdList);
                    Orders oup = await _priApi.CancelOrders(_manualTradeApiKey, _manualTradeSecret, "btc_jpy", cancelIdList);

                    // TODO 結果が。。。

                    // 後は update loopに任せる

                }


            }
        }

        // 自動取引リストビュー内の：エラーをリセットするコマンド
        public ICommand AutoTradeResetErrorListviewCommand { get; }
        public bool AutoTradeResetErrorListviewCommand_CanExecute()
        {
            return true;
        }
        public void AutoTradeResetErrorListviewCommand_Execute(object obj)
        {
            //Debug.WriteLine("AutoTradeResetErrorListviewCommand_Execute");

            if (obj == null) return;

            // obj == System.Windows.Controls.SelectedItemCollection

            // 選択注文アイテム保持用
            List<AutoTrade2> selectedList = new List<AutoTrade2>();
            // キャンセルする注文IDを保持
            List<int> cancelIdList = new List<int>();

            // System.Windows.Controls.SelectedItemCollection をキャストして、ループ
            System.Collections.IList items = (System.Collections.IList)obj;
            var collection = items.Cast<AutoTrade2>();

            foreach (var item in collection)
            {
                // アイテム追加
                selectedList.Add(item as AutoTrade2);
            }

            // 選択注文アイテムをループして、エラーを無かった事にする。
            foreach (var item in selectedList)
            {
                if (item.IsCanceled)
                {
                    continue;
                }

                if (item.BuyHasError == true)
                {
                    item.BuyHasError = false;
                }

                if (item.SellHasError == true)
                {
                    item.SellHasError = false;
                }

            }
        }

        // 自動取引リストビュー内の：エラーを削除するコマンド
        public ICommand AutoTradeDeleteErrorItemListviewCommand { get; }
        public bool AutoTradeDeleteErrorItemListviewCommand_CanExecute()
        {
            return true;
        }
        public void AutoTradeDeleteErrorItemListviewCommand_Execute(object obj)
        {
            //Debug.WriteLine("AutoTradeDeleteErrorItemListviewCommand_Execute");

            if (obj == null) return;

            // obj == System.Windows.Controls.SelectedItemCollection

            // 選択注文アイテム保持用
            List<AutoTrade2> selectedList = new List<AutoTrade2>();
            // キャンセルする注文IDを保持
            List<int> cancelIdList = new List<int>();

            // System.Windows.Controls.SelectedItemCollection をキャストして、ループ
            System.Collections.IList items = (System.Collections.IList)obj;
            var collection = items.Cast<AutoTrade2>();

            foreach (var item in collection)
            {
                // アイテム追加
                selectedList.Add(item as AutoTrade2);
            }

            // 念のため、UIスレッドで。
            if (Application.Current == null) { return; }
            Application.Current.Dispatcher.Invoke(() =>
            {
                // 選択注文アイテムをループして、エラーを削除する
                foreach (var item in selectedList)
                {
                    _autoTrades2.Remove(item);
                }
            });


        }

        // 日本語フィードのアイテムクリックコマンド
        public ICommand FeedOpenJaCommand { get; }
        public bool FeedOpenJaCommand_CanExecute()
        {
            return true;
        }
        public void FeedOpenJaCommand_Execute()
        {
            //Debug.WriteLine("FeedOpenJaCommand_Execute");
            if (SelectedFeedItemJa != null)
            {
                //Debug.WriteLine(SelectedFeedItemJa.Link);
                System.Diagnostics.Process.Start(SelectedFeedItemJa.Link);
            }
        }

        // 英語フィードのアイテムクリックコマンド
        public ICommand FeedOpenEnCommand { get; }
        public bool FeedOpenEnCommand_CanExecute()
        {
            return true;
        }
        public void FeedOpenEnCommand_Execute()
        {
            if (SelectedFeedItemEn != null)
            {
               // Debug.WriteLine(SelectedFeedItemEn.Link);
                System.Diagnostics.Process.Start(SelectedFeedItemEn.Link);

            }
        }

        // 板情報のグルーピングコマンド
        public ICommand DepthGroupingCommand { get; }
        public bool DepthGroupingCommand_CanExecute()
        {
            return true;
        }
        public void DepthGroupingCommand_Execute(object obj)
        {
            //Debug.WriteLine("DepthGroupingCommand_Execute");

            if (obj == null) return;

            int numVal = Int32.Parse(obj.ToString());

            //Debug.WriteLine(numVal.ToString());

            if (DepthGrouping != numVal)
            {
                DepthGrouping = numVal;
                DepthGroupingChanged = true;
            }

        }

        // ログイン処理 (ロック解除)
        public ICommand LogInCommand { get; }
        public bool LogInCommand_CanExecute()
        {
            return true;
        }
        public void LogInCommand_Execute(object obj)
        {   
            if (obj is PasswordBox)
            {
                // for Unbindable PasswordBox.
                var passwordBox = obj as PasswordBox;
                if (!String.IsNullOrEmpty(passwordBox.Password))
                {
                    if (_realPassword == passwordBox.Password)
                    {
                        IsPasswordSet = true;
                        LoggedInMode = true;

                        LogInErrorInfo = "";
                    }
                    else
                    {
                        LogInErrorInfo = "パスワードが違います。";
                    }

                    // パスワードのクリア
                    passwordBox.Password = "";
                }
                else
                {
                    LogInErrorInfo = "パスワードを入力してください。";
                }

            }
        }

        // ログアウト処理　（ロック）
        public ICommand LogOutCommand { get; }
        public bool LogOutCommand_CanExecute()
        {
            return true;
        }
        public void LogOutCommand_Execute()
        {
            if (LoggedInMode)
                LoggedInMode = false;

        }

        // ログイン画面を表示コマンド
        public ICommand ShowLogInCommand { get; }
        public bool ShowLogInCommand_CanExecute()
        {
            return true;
        }
        public void ShowLogInCommand_Execute()
        {
            if (ShowSettings)
                ShowSettings = false;

            if (LoggedInMode == false)
                ShowLogIn = true;
        }
        
        // 新たにパスワードの設定
        public ICommand NewLogInPasswordCommand { get; }
        public bool NewLogInPasswordCommand_CanExecute()
        {
            return true;
        }
        public void NewLogInPasswordCommand_Execute(object obj)
        {
            // MultipleCommandParameterConverter で複数のパラメータを可能にしている。
            var values = (object[])obj;

            if ((values[0] is PasswordBox) && (values[1] is PasswordBox))
            {
                if ((values[0] as PasswordBox).Password == (values[1] as PasswordBox).Password)
                {
                    if (!String.IsNullOrEmpty((values[0] as PasswordBox).Password))
                    {
                        //Debug.WriteLine("ok " + (values[0] as PasswordBox).Password);
                        _realPassword = (values[0] as PasswordBox).Password;

                        IsPasswordSet = true;
                        LoggedInMode = true;

                        (values[0] as PasswordBox).Password = "";
                        (values[1] as PasswordBox).Password = "";

                        LogInErrorInfo = "";
                    }
                    else
                    {
                        LogInErrorInfo = "パスワード欄が空です。";
                    }

                }
                else
                {
                    LogInErrorInfo = "パスワードが違います。";
                    //Debug.WriteLine("not ok " + (values[0] as PasswordBox).Password);
                }
            }
        }

        // ログイン画面キャンセル(ESC)
        public ICommand LoginCancelCommand { get; }
        public bool LoginCancelCommand_CanExecute()
        {
            return true;
        }
        public void LoginCancelCommand_Execute()
        {
            if (LoggedInMode == false)
            {
                if (ShowLogIn)
                {
                    ShowLogIn = false;
                }
                else if (ShowPasswordSet)
                {
                    ShowPasswordSet = false;
                }
            }
        }

        // 設定画面表示
        public ICommand ShowSettingsCommand { get; }
        public bool ShowSettingsCommand_CanExecute()
        {
            return true;
        }
        public void ShowSettingsCommand_Execute()
        {
            //Debug.WriteLine("ShowSettingsCommand_Execute");
            ShowSettings = true;
        }

        // 設定画面キャンセルボタン
        public ICommand SettingsCancelCommand { get; }
        public bool SettingsCancelCommand_CanExecute()
        {
            return true;
        }
        public void SettingsCancelCommand_Execute()
        {
            //Debug.WriteLine("SettingsCancelCommand_Execute");
            ShowSettings = false;
        }

        // 設定画面OKボタン
        public ICommand SettingsOKCommand { get; }
        public bool SettingsOKCommand_CanExecute()
        {
            return true;
        }
        public void SettingsOKCommand_Execute()
        {
            //Debug.WriteLine("SettingsOKCommand_Execute");
            ShowSettings = false;
        }

        // パスワードの変更
        public ICommand ChangeLogInPasswordCommand { get; }
        public bool ChangeLogInPasswordCommand_CanExecute()
        {
            return true;
        }
        public void ChangeLogInPasswordCommand_Execute(object obj)
        {
            // MultipleCommandParameterConverter で複数のパラメータを可能にしている。
            var values = (object[])obj;

            if ((values[0] is PasswordBox) && (values[1] is PasswordBox) && (values[2] is PasswordBox))
            {
                if ((values[0] as PasswordBox).Password == _realPassword)
                {
                    if (!String.IsNullOrEmpty((values[1] as PasswordBox).Password))
                    {
                        //Debug.WriteLine("ok " + (values[0] as PasswordBox).Password);
                        _realPassword = (values[1] as PasswordBox).Password;

                        IsPasswordSet = true;
                        LoggedInMode = true;

                        (values[0] as PasswordBox).Password = "";
                        (values[1] as PasswordBox).Password = "";
                        (values[2] as PasswordBox).Password = "";

                        LogInErrorInfo = "";
                        ChangePasswordResultInfo = "変更されました。";
                    }
                    else
                    {
                        LogInErrorInfo = "パスワード欄が空です。";
                    }

                }
                else
                {
                    LogInErrorInfo = "パスワードが間違いです。";
                    //Debug.WriteLine("not ok " + (values[0] as PasswordBox).Password);
                }

            }


        }

        // 資産情報のAPIキーセット
        public ICommand SetAssetsAPIKeyCommand { get; }
        public bool SetAssetsAPIKeyCommand_CanExecute()
        {
            return true;
        }
        public void SetAssetsAPIKeyCommand_Execute()
        {
            if ((string.IsNullOrEmpty(AssetsApiKey) == false) && (string.IsNullOrEmpty(AssetsApiSecret) == false))
            {
                AssetsApiKeyIsSet = true;
            }
        }

        // 注文情報のAPIキーセット
        public ICommand SetManualTradeAPIKeyCommand { get; }
        public bool SetManualTradeAPIKeyCommand_CanExecute()
        {
            return true;
        }
        public void SetManualTradeAPIKeyCommand_Execute()
        {
            if ((string.IsNullOrEmpty(ManualTradeApiKey) == false) && (string.IsNullOrEmpty(ManualTradeSecret) == false))
            {
                ManualTradeApiKeyIsSet = true;
            }
        }

        // 取引履歴のAPIキーセット
        public ICommand SetTradeHistoryAPIKeyCommand { get; }
        public bool SetTradeHistoryAPIKeyCommand_CanExecute()
        {
            return true;
        }
        public void SetTradeHistoryAPIKeyCommand_Execute()
        {
            if ((string.IsNullOrEmpty(TradeHistoryApiKey) == false) && (string.IsNullOrEmpty(TradeHistorySecret) == false))
            {
                TradeHistoryApiKeyIsSet = true;
            }
        }

        // 自動取引のAPIキーセット
        public ICommand SetAutoTradeAPIKeyCommand { get; }
        public bool SetAutoTradeAPIKeyCommand_CanExecute()
        {
            return true;
        }
        public void SetAutoTradeAPIKeyCommand_Execute()
        {
            if ((string.IsNullOrEmpty(AutoTradeApiKey) == false) && (string.IsNullOrEmpty(AutoTradeSecret) == false))
            {
                AutoTradeApiKeyIsSet = true;
            }
        }

        // 特殊取引のAPIキーセット
        public ICommand SetIfdocoTradeAPIKeyCommand { get; }
        public bool SetIfdocoTradeAPIKeyCommand_CanExecute()
        {
            return true;
        }
        public void SetIfdocoTradeAPIKeyCommand_Execute()
        {
            if ((string.IsNullOrEmpty(IfdocoTradeApiKey) == false) && (string.IsNullOrEmpty(IfdocoTradeSecret) == false))
            {
                IfdocoTradeApiKeyIsSet = true;
            }
        }

        // 注文一覧のAPIキーセット
        public ICommand SetOrdersAPIKeyCommand { get; }
        public bool SetOrdersAPIKeyCommand_CanExecute()
        {
            return true;
        }
        public void SetOrdersAPIKeyCommand_Execute()
        {
            if ((string.IsNullOrEmpty(OrdersApiKey) == false) && (string.IsNullOrEmpty(OrdersSecret) == false))
            {
                OrdersApiKeyIsSet = true;
            }
        }

        // APIキー設定画面を表示
        public ICommand ShowApiKeyCommand { get; }
        public bool ShowApiKeyCommand_CanExecute()
        {
            return true;
        }
        public void ShowApiKeyCommand_Execute(object obj)
        {
            if (obj is PasswordBox)
            {
                var passwordBox = obj as PasswordBox;
                if (!String.IsNullOrEmpty(passwordBox.Password))
                {
                    if (_realPassword == passwordBox.Password)
                    {
                        // 念のため
                        if (IsPasswordSet && LoggedInMode)
                        { 
                            ShowApiKeyLock = false;
                        }

                        LogInErrorInfo = "";
                    }
                    else
                    {
                        LogInErrorInfo = "パスワードが違います。";
                    }
                    // パスワードのクリア
                    passwordBox.Password = "";
                }
                else
                {
                    LogInErrorInfo = "パスワードを入力してください。";
                }

            }
        }
        

        #endregion

    }
}
