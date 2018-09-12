using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using BitDesk.Models.Clients;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.IO;
using System.ComponentModel;
using BitDesk.Common;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Media;

namespace BitDesk.ViewModels
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

        //public Color TickHistoryPriceColor { get; set; }

        public bool TickHistoryPriceUp { get; set; }

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

    #region == エラー表示用クラス ==

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

    #region == テーマ用のクラス ==

    /// <summary>
    /// テーマ用のクラス
    /// </summary>
    public class Theme
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
    }

    #endregion

    #endregion

    /// <summary>
    /// メインのビューモデル
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        // テスト用
        private decimal _initPrice = 81800M;

        /// <summary>
        /// 各通貨ペア用のクラス
        /// </summary>
        public class Pair : ViewModelBase
        {
            #region == カラー定義 ==

            // TODO
            // Make them style or binding. 

            public Color PriceUpColor = (Color)ColorConverter.ConvertFromString("#a0d8ef");//Colors.Aqua;
            public Color PriceDownColor = (Color)ColorConverter.ConvertFromString("#e597b2");//Colors.Pink;
            public static Color PriceNeutralColor = Colors.Gray;

            private Color _priceWarningColor = (Color)ColorConverter.ConvertFromString("#FFDFD991");

            private Color _accentCoolColor = (Color)ColorConverter.ConvertFromString("#2c4f54");
            private Color _accentWarmColor = (Color)ColorConverter.ConvertFromString("#e0815e");

            //private Color _chartStrokeColor = System.Windows.Media.Brushes.WhiteSmoke.Color;

            #endregion
            
            // 通貨フォーマット用
            private string _ltpFormstString = "{0:#,0}";
            // 通貨ペア
            private Pairs p;

            // 表示用 通貨ペア名 "BTC/JPY";
            public string CurrentPairString
            {
                get
                {
                    return PairStrings[p];
                }
            }

            public Dictionary<Pairs, string> PairStrings { get; set; } = new Dictionary<Pairs, string>()
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
                    this.NotifyPropertyChanged("LtpString");

                    this.NotifyPropertyChanged("BasePriceIcon");
                    this.NotifyPropertyChanged("BasePriceIconColor");
                }
            }

            public string LtpString
            {
                get
                {
                    if (_ltp == 0)
                    {
                        return "";
                    }
                    else
                    {
                        return String.Format(_ltpFormstString, _ltp);

                    }
                }
            }

            private bool _ltpUpFlag;
            public bool LtpUpFlag
            {
                get
                {
                    return _ltpUpFlag;
                }
                set
                {
                    if (_ltpUpFlag == value)
                        return;

                    _ltpUpFlag = value;
                    this.NotifyPropertyChanged("LtpUpFlag");
                }
            }

            private double _ltpFontSize = 45;
            public double LtpFontSize
            {
                get { return _ltpFontSize; }
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

            #region == アラーム用のプロパティ ==

            // アラーム 警告音再生
            private decimal _alarmPlus;
            public decimal AlarmPlus
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
                    //this.NotifyPropertyChanged(nameof(ChartBlueline));
                }
            }
            private decimal _alarmMinus;
            public decimal AlarmMinus
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
                    //this.NotifyPropertyChanged(nameof(ChartRedline));
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

            private bool _highLowInfoTextColorFlag;
            public bool HighLowInfoTextColorFlag
            {
                get
                {
                    return _highLowInfoTextColorFlag;
                }
                set
                {
                    if (_highLowInfoTextColorFlag == value)
                        return;

                    _highLowInfoTextColorFlag = value;
                    this.NotifyPropertyChanged("HighLowInfoTextColorFlag");
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
                    this.NotifyPropertyChanged("BasePriceString");

                }
            }
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
                        return PriceUpColor;//Colors.Aqua;
                    }
                    else if (_ltp < BasePrice)
                    {
                        return PriceDownColor; //Colors.Pink;
                    }
                    else
                    {
                        return Colors.Gainsboro;
                    }
                }
            }
            public string BasePriceString
            {
                get
                {
                    return String.Format(_ltpFormstString, BasePrice);
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
                    this.NotifyPropertyChanged("AveragePriceString");
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
                        return PriceUpColor;//Colors.Aqua;
                    }
                    else if (_ltp < _averagePrice)
                    {
                        return PriceDownColor;//Colors.Pink;
                    }
                    else
                    {
                        return Colors.Gainsboro;
                    }
                }
            }
            public string AveragePriceString
            {
                get
                {
                    return String.Format(_ltpFormstString, _averagePrice); ;
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
                        return PriceUpColor;//Colors.Aqua;
                    }
                    else if (_ltp < MiddleLast24Price)
                    {
                        return PriceDownColor;//Colors.Pink;
                    }
                    else
                    {
                        return Colors.Gainsboro;
                    }
                }
            }
            public string MiddleLast24PriceString
            {
                get
                {
                    return String.Format(_ltpFormstString, MiddleLast24Price); ;
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
                        return PriceUpColor;//Colors.Aqua;
                    }
                    else if (_ltp < MiddleInitPrice)
                    {
                        return PriceDownColor;//Colors.Pink;
                    }
                    else
                    {
                        return Colors.Gainsboro;
                    }
                }
            }
            public string MiddleInitPriceString
            {
                get
                {
                    return String.Format(_ltpFormstString, MiddleInitPrice); ;
                }
            }

            //high 過去24時間の最高値取引価格
            private decimal _highestIn24Price;
            public decimal HighestIn24Price
            {
                get { return _highestIn24Price; }
                set
                {
                    if (_highestIn24Price == value)
                        return;

                    _highestIn24Price = value;
                    this.NotifyPropertyChanged("HighestIn24Price");
                    this.NotifyPropertyChanged("High24String");
                    //this.NotifyPropertyChanged("ChartMaxValue");


                    this.NotifyPropertyChanged("MiddleLast24Price");
                    this.NotifyPropertyChanged("MiddleLast24PriceIcon");
                    this.NotifyPropertyChanged("MiddleLast24PriceIconColor");
                    this.NotifyPropertyChanged("MiddleLast24PriceString");

                    //if (MinMode) return;
                    // チャートの最高値をセット
                    //ChartAxisY[0].MaxValue = (double)_highestIn24Price + 3000;
                    // チャートの２４最高値ポイントを更新
                    //(ChartSeries[1].Values[0] as ObservableValue).Value = (double)_highestIn24Price;

                }
            }
            public string High24String
            {
                get { return String.Format(_ltpFormstString, _highestIn24Price); }
            }

            //low 過去24時間の最安値取引価格
            private decimal _lowestIn24Price;
            public decimal LowestIn24Price
            {
                get { return _lowestIn24Price; }
                set
                {
                    if (_lowestIn24Price == value)
                        return;

                    _lowestIn24Price = value;
                    this.NotifyPropertyChanged("LowestIn24Price");
                    this.NotifyPropertyChanged("Low24String");
                    //this.NotifyPropertyChanged("ChartMinValue");

                    this.NotifyPropertyChanged("MiddleLast24Price");
                    this.NotifyPropertyChanged("MiddleLast24PriceIcon");
                    this.NotifyPropertyChanged("MiddleLast24PriceIconColor");
                    this.NotifyPropertyChanged("MiddleLast24PriceString");

                    //if (MinMode) return;
                    // チャートの最低値をセット
                    //ChartAxisY[0].MinValue = (double)_lowestIn24Price - 3000;
                    // チャートの２４最低値ポイントを更新
                    //(ChartSeries[2].Values[0] as ObservableValue).Value = (double)_lowestIn24Price;
                }
            }
            public string Low24String
            {
                get { return String.Format(_ltpFormstString, _lowestIn24Price); }
            }

            // 起動後 最高値
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
                    this.NotifyPropertyChanged("MiddleInitPriceString");
                    this.NotifyPropertyChanged("MiddleInitPriceIcon");
                    this.NotifyPropertyChanged("MiddleInitPriceIconColor");

                    //if (MinMode) return;
                    //(ChartSeries[1].Values[0] as ObservableValue).Value = (double)_highestPrice;
                }
            }
            public string HighestPriceString
            {
                get
                {
                    return String.Format(_ltpFormstString, _highestPrice); ;
                }
            }

            // 起動後 最低 値
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
                    this.NotifyPropertyChanged("MiddleInitPriceString");
                    this.NotifyPropertyChanged("MiddleInitPriceIcon");
                    this.NotifyPropertyChanged("MiddleInitPriceIconColor");

                    //if (MinMode) return;
                    // (ChartSeries[2].Values[0] as ObservableValue).Value = (double)_lowestPrice;
                }
            }
            public string LowestPriceString
            {
                get
                {
                    return String.Format(_ltpFormstString, _lowestPrice); ;
                }
            }

            #endregion

            #region == 板情報のプロパティ ==

            private decimal _depthGrouping = 0;
            public decimal DepthGrouping
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

                    if (DepthGrouping100 == _depthGrouping)
                    {
                        IsDepthGrouping100 = true;
                        IsDepthGrouping1000 = false;
                        IsDepthGroupingOff = false;
                    }
                    if (DepthGrouping1000 == _depthGrouping)
                    {
                        IsDepthGrouping100 = false;
                        IsDepthGrouping1000 = true;
                        IsDepthGroupingOff = false;
                    }

                    if (0 == _depthGrouping)
                    {
                        IsDepthGrouping100 = false;
                        IsDepthGrouping1000 = false;
                        IsDepthGroupingOff = true;
                    }

                    this.NotifyPropertyChanged("DepthGrouping");

                }
            }

            private bool _isDepthGroupingOff = true;
            public bool IsDepthGroupingOff
            {
                get
                {
                    return _isDepthGroupingOff;
                }
                set
                {
                    if (_isDepthGroupingOff == value)
                        return;

                    _isDepthGroupingOff = value;
                    this.NotifyPropertyChanged("IsDepthGroupingOff");
                }
            }

            private decimal _depthGrouping100 = 100;
            public decimal DepthGrouping100
            {
                get
                {
                    return _depthGrouping100;
                }
                set
                {
                    if (_depthGrouping100 == value)
                        return;

                    _depthGrouping100 = value;
                    this.NotifyPropertyChanged("DepthGrouping100");

                }
            }

            private bool _isDepthGrouping100;
            public bool IsDepthGrouping100
            {
                get
                {
                    return _isDepthGrouping100;
                }
                set
                {
                    if (_isDepthGrouping100 == value)
                        return;

                    _isDepthGrouping100 = value;
                    this.NotifyPropertyChanged("IsDepthGrouping100");
                }
            }

            private decimal _depthGrouping1000 = 1000;
            public decimal DepthGrouping1000
            {
                get
                {
                    return _depthGrouping1000;
                }
                set
                {
                    if (_depthGrouping1000 == value)
                        return;

                    _depthGrouping1000 = value;
                    this.NotifyPropertyChanged("DepthGrouping1000");

                }
            }

            private bool _isDepthGrouping1000;
            public bool IsDepthGrouping1000
            {
                get
                {
                    return _isDepthGrouping1000;
                }
                set
                {
                    if (_isDepthGrouping1000 == value)
                        return;

                    _isDepthGrouping1000 = value;
                    this.NotifyPropertyChanged("IsDepthGrouping1000");
                }
            }

            #endregion

            // TickHistoryクラス リスト
            private ObservableCollection<TickHistory> _tickHistory = new ObservableCollection<TickHistory>();
            public ObservableCollection<TickHistory> TickHistories
            {
                get { return this._tickHistory; }
            }

            // コンストラクタ
            public Pair(Pairs p, double fontSize, string ltpFormstString, decimal grouping100, decimal grouping1000)
            {
                this.p = p;
                _ltpFontSize = fontSize;
                _ltpFormstString = ltpFormstString;

                _depthGrouping100 = grouping100;
                _depthGrouping1000 = grouping1000;
            }

        }

        #region == 基本 ==

        // Application version
        private string _appVer = "0.0.0.2";

        // Application name
        private string _appName = "BitDesk";

        // Application config file folder
        private string _appDeveloper = "torum";

        // Application Window Title
        public string AppTitle
        {
            get
            {
                return _appName + " " + _appVer;
            }
        }

        #endregion

        #region == 画面切替関係 ==

        // プライベートモード表示切替（自動取引表示）
        public bool ExperimentalMode
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

        // 設定画面表示フラグ
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
                {
                    if (_showAllCharts)
                        _allChartMode = true;
                    else
                        _allChartMode = false;

                    ShowMainContents = false;
                    ShowAllCharts = false;
                }
                else
                {
                    if (_allChartMode)
                    {
                        ShowMainContents = false;
                        ShowAllCharts = true;
                    }
                    else
                    {
                        ShowMainContents = true;
                        ShowAllCharts = false;
                    }
                }

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

        // チャートの一覧モードフラグ
        private bool _allChartMode = false;

        // LiveChartの不明なエクセプションが起きるので、ver1が出るまで使わない。
        private bool _showAllCharts = false;
        public bool ShowAllCharts
        {
            get
            {
                return _showAllCharts;
            }
            set
            {
                if (_showAllCharts == value) return;

                _showAllCharts = value;
                this.NotifyPropertyChanged("ShowAllCharts");

                if (_showAllCharts)
                    _allChartMode = true;
                else
                    _showAllCharts = false;
            }
        }

        #endregion

        #region == 設定画面関係 ==

        // グローバルアラーム音設定
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

        // パスワード変更結果表示文字列
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

        #region == テーマ関係 ==

        // テーマ一覧
        private ObservableCollection<Theme> _themes;
        public ObservableCollection<Theme> Themes
        {
            get { return _themes; }
            set { _themes = value; }
        }

        // テーマ切替
        private Theme _currentTheme;
        public Theme CurrentTheme
        {
            get
            {
                return _currentTheme;
            }
            set
            {
                if (_currentTheme == value) return;

                _currentTheme = value;
                this.NotifyPropertyChanged("CurrentTheme");

                (Application.Current as App).ChangeTheme(_currentTheme.Name);

            }
        }

        // 画面透過率
        private double _windowOpacity = 0.3;
        public double WindowOpacity
        {
            get
            {
                return _windowOpacity;
            }
            set
            {
                if (_windowOpacity == value) return;

                _windowOpacity = value;
                this.NotifyPropertyChanged("WindowOpacity");
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

        #region == 通貨ペア切り替え用のプロパティ ==

        // 左メニュータブの選択インデックス
        private int _activePairIndex = 0;
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
                this.NotifyPropertyChanged("ActivePairIndex");

                if (_activePairIndex == 0)
                {
                    CurrentPair = Pairs.btc_jpy;
                    ActivePair = PairBtcJpy;
                    ActivePair.Ltp = PairBtcJpy.Ltp;

                    DepthGroupingChanged = true;

                    IsBtcVisible = true;
                    IsXrpVisible = false;
                    IsEthVisible = false;
                    IsLtcVisible = false;
                    IsMonaJpyVisible = false;
                    IsBchJpyVisible = false;

                }
                else if (_activePairIndex == 1)
                {
                    CurrentPair = Pairs.xrp_jpy;
                    ActivePair = PairXrpJpy;
                    ActivePair.Ltp = PairXrpJpy.Ltp;

                    DepthGroupingChanged = true;

                    IsBtcVisible = false;
                    IsXrpVisible = true;
                    IsEthVisible = false;
                    IsLtcVisible = false;
                    IsMonaJpyVisible = false;
                    IsBchJpyVisible = false;

                }
                else if (_activePairIndex == 2)
                {
                    CurrentPair = Pairs.ltc_btc;
                    ActivePair = PairLtcBtc;
                    ActivePair.Ltp = PairLtcBtc.Ltp;

                    DepthGroupingChanged = true;

                    IsBtcVisible = false;
                    IsXrpVisible = false;
                    IsEthVisible = false;
                    IsLtcVisible = true;
                    IsMonaJpyVisible = false;
                    IsBchJpyVisible = false;

                }
                else if (_activePairIndex == 3)
                {
                    CurrentPair = Pairs.eth_btc;
                    ActivePair = PairEthBtc;
                    ActivePair.Ltp = PairEthBtc.Ltp;

                    DepthGroupingChanged = true;

                    IsBtcVisible = false;
                    IsXrpVisible = false;
                    IsEthVisible = true;
                    IsLtcVisible = false;
                    IsMonaJpyVisible = false;
                    IsBchJpyVisible = false;

                }
                else if (_activePairIndex == 4)
                {
                    CurrentPair = Pairs.mona_jpy;
                    ActivePair = PairMonaJpy;
                    ActivePair.Ltp = PairMonaJpy.Ltp;

                    DepthGroupingChanged = true;

                    IsBtcVisible = false;
                    IsXrpVisible = false;
                    IsEthVisible = false;
                    IsLtcVisible = false;
                    IsMonaJpyVisible = true;
                    IsBchJpyVisible = false;

                }
                else if (_activePairIndex == 5)
                {
                    CurrentPair = Pairs.bcc_jpy;
                    ActivePair = PairBchJpy;
                    ActivePair.Ltp = PairBchJpy.Ltp;

                    DepthGroupingChanged = true;

                    IsBtcVisible = false;
                    IsXrpVisible = false;
                    IsEthVisible = false;
                    IsLtcVisible = false;
                    IsMonaJpyVisible = false;
                    IsBchJpyVisible = true;

                }


                // チャートの表示
                DisplayChart(CurrentPair);

            }
        }

        // 現在の通貨ペア
        private Pairs _currentPair = Pairs.btc_jpy;
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
                this.NotifyPropertyChanged("CurrentCoinString");
                this.NotifyPropertyChanged("CurrentPairUnitString");

                this.NotifyPropertyChanged("TickTimeStampString");

            }
        }

        // 表示用 通貨 
        public string CurrentCoinString
        {
            get
            {
                return CurrentPairCoin[CurrentPair].ToUpper();//_coin.ToUpper();
            }
        }

        // 表示用 円/BTC 単位
        public string CurrentPairUnitString
        {
            get
            {
                if ((CurrentPair == Pairs.btc_jpy) || (CurrentPair == Pairs.xrp_jpy) || (CurrentPair == Pairs.mona_jpy) || (CurrentPair == Pairs.bcc_jpy))
                {
                    return "円";
                }
                else if ((CurrentPair == Pairs.eth_btc) || (CurrentPair == Pairs.ltc_btc))
                {
                    return "BTC";
                }
                else
                {
                    return "?";
                }

            }
        }

        // TODO いる？
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

        public Dictionary<Pairs, string> PairStrings { get; set; } = new Dictionary<Pairs, string>()
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

        public Dictionary<Pairs, string> CurrentPairCoin { get; set; } = new Dictionary<Pairs, string>()
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

        // デフォの通貨ペアクラス
        private Pair _activePair = new Pair(Pairs.btc_jpy, 45, "{0:#,0}", 100M, 1000M);
        public Pair ActivePair
        {
            get
            {
                return _activePair;
            }
            set
            {
                if (_activePair == value)
                    return;

                _activePair = value;
                this.NotifyPropertyChanged("ActivePair");
            }
        }

        private Pair _pairBtcJpy = new Pair(Pairs.btc_jpy, 45, "{0:#,0}", 100M, 1000M);
        public Pair PairBtcJpy
        {
            get
            {
                return _pairBtcJpy;
            }
        }

        private bool _isBtcVisible;
        public bool IsBtcVisible
        {
            get
            {
                return _isBtcVisible;
            }
            set
            {
                if (_isBtcVisible == value)
                    return;

                _isBtcVisible = value;
                this.NotifyPropertyChanged("IsBtcVisible");
            }
        }

        private Pair _pairXrpJpy = new Pair(Pairs.xrp_jpy, 45, "{0:#,0.000}", 0.1M, 0.01M);
        public Pair PairXrpJpy
        {
            get
            {
                //return Ltps[Pairs.xrp_jpy];
                return _pairXrpJpy;
            }
        }

        private bool _isXrpVisible;
        public bool IsXrpVisible
        {
            get
            {
                return _isXrpVisible;
            }
            set
            {
                if (_isXrpVisible == value)
                    return;

                _isXrpVisible = value;
                this.NotifyPropertyChanged("IsXrpVisible");
            }
        }

        private Pair _pairEthBtc = new Pair(Pairs.eth_btc, 30, "{0:#,0.00000000}", 0.0001M, 0.00001M);
        public Pair PairEthBtc
        {
            get
            {
                //return Ltps[Pairs.eth_btc];
                return _pairEthBtc;
            }
        }

        private bool _isEthVisible;
        public bool IsEthVisible
        {
            get
            {
                return _isEthVisible;
            }
            set
            {
                if (_isEthVisible == value)
                    return;

                _isEthVisible = value;
                this.NotifyPropertyChanged("IsEthVisible");
            }
        }

        private Pair _pairLtcBtc = new Pair(Pairs.ltc_btc ,30, "{0:#,0.00000000}", 0.0001M, 0.00001M);
        public Pair PairLtcBtc
        {
            get
            {
                //return Ltps[Pairs.ltc_btc];
                return _pairLtcBtc;
            }
        }

        private bool _isLtcVisible;
        public bool IsLtcVisible
        {
            get
            {
                return _isLtcVisible;
            }
            set
            {
                if (_isLtcVisible == value)
                    return;

                _isLtcVisible = value;
                this.NotifyPropertyChanged("IsLtcVisible");
            }
        }

        private Pair _pairMonaJpy = new Pair(Pairs.mona_jpy ,45, "{0:#,0.000}", 0.1M, 1M);
        public Pair PairMonaJpy
        {
            get
            {
                //return Ltps[Pairs.mona_jpy];
                return _pairMonaJpy;
            }
        }

        private bool _isMonaJpyVisible;
        public bool IsMonaJpyVisible
        {
            get
            {
                return _isMonaJpyVisible;
            }
            set
            {
                if (_isMonaJpyVisible == value)
                    return;

                _isMonaJpyVisible = value;
                this.NotifyPropertyChanged("IsMonaJpyVisible");
            }
        }

        private Pair _pairBchJpy = new Pair(Pairs.bcc_jpy ,45, "{0:#,0}", 10M, 100M);
        public Pair PairBchJpy
        {
            get
            {
                //return Ltps[Pairs.bcc_jpy];
                return _pairBchJpy;
            }
        }

        private bool _isBchJpyVisible;
        public bool IsBchJpyVisible
        {
            get
            {
                return _isBchJpyVisible;
            }
            set
            {
                if (_isBchJpyVisible == value)
                    return;

                _isBchJpyVisible = value;
                this.NotifyPropertyChanged("IsBchJpyVisible");
            }
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

        #region == クライアントのプロパティ ==

        // 公開API Ticker クライアント
        PublicAPIClient _pubTickerApi = new PublicAPIClient();

        // 公開API Depth クライアント
        PublicAPIClient _pubDepthApi = new PublicAPIClient();

        // 公開API Transactions クライアント
        PublicAPIClient _pubTransactionsApi = new PublicAPIClient();

        // 公開API ロウソク足 クライアント
        PublicAPIClient _pubCandlestickApi = new PublicAPIClient();

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

        // 自動取引2
        private ObservableCollection<AutoTrade2> _autoTrades2 = new ObservableCollection<AutoTrade2>();
        public ObservableCollection<AutoTrade2> AutoTrades2
        {
            get { return this._autoTrades2; }
        }

        #endregion

        #region == 板情報のプロパティ ==

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

        #region == チャート用のプロパティ ==

        // ロウソク足タイプ
        public enum CandleTypes
        {
            OneMin, FiveMin, FifteenMin, ThirteenMin, OneHour, FourHour, EightHour, TwelveHour, OneDay, OneWeek
        }

        // ロウソク足タイプ　コンボボックス表示用
        public Dictionary<CandleTypes, string> CandleTypesDictionary { get; } = new Dictionary<CandleTypes, string>()
        {
            {CandleTypes.OneMin, "1m"},
            //{CandleTypes.FiveMin, "５分" },
            //{CandleTypes.FifteenMin, "１５分"},
            //{CandleTypes.ThirteenMin, "３０分" },
            {CandleTypes.OneHour, "1h" },
            //{CandleTypes.FourHour, "４時間"},
            //{CandleTypes.EightHour, "８時間" },
            //{CandleTypes.TwelveHour, "１２時間"},
            {CandleTypes.OneDay, "1d" },
            //{CandleTypes.OneWeek, "１週間"},

        };

        // 選択されたロウソク足タイプ
        public CandleTypes _selectedCandleType = CandleTypes.OneHour; // デフォ。変更注意。起動時のロードと合わせる。
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

                Debug.WriteLine("SelectedCandleType " + _selectedCandleType.ToString());

                // 
                if (_selectedCandleType == CandleTypes.OneMin)
                {
                    // 一分毎のロウソク足タイプなら

                    if ((SelectedChartSpan != ChartSpans.OneHour) && (SelectedChartSpan != ChartSpans.ThreeHour))
                    {
                        // デフォルト 一時間の期間で表示
                        SelectedChartSpan = ChartSpans.OneHour;

                        //コンボボックスとダブルアップデートにならないようにするためここでreturn.
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

                        //コンボボックスとダブルアップデートにならないようにするためここでreturn.
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
                        SelectedChartSpan = ChartSpans.TwoMonth;

                        //コンボボックスとダブルアップデートにならないようにするためここでreturn.
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

                    Debug.WriteLine("■" + _selectedCandleType.ToString() + " チャート表示、設定範囲外");

                    return;

                    // デフォルト 1日の期間で表示
                    //SelectedChartSpan = ChartSpans.OneDay;
                    //Debug.WriteLine("デフォルト Oops");

                }

                // チャート表示
                if (ShowAllCharts)
                {
                    DisplayCharts();
                }
                else
                {
                    DisplayChart(CurrentPair);
                }

            }
        }

        // 読み込み中状態を知らせる文字列
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

        // チャート表示期間　コンボボックス表示用
        public Dictionary<ChartSpans, string> ChartSpansDictionary { get; } = new Dictionary<ChartSpans, string>()
        {
            {ChartSpans.OneHour, "1h"},
            {ChartSpans.ThreeDay, "3d" },
            {ChartSpans.TwoMonth, "2M" },

        };

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
                //ChangeChartSpan();

                if (ShowAllCharts)
                {
                    ChangeChartSpans();
                    //DisplayCharts();
                }
                else
                {
                    ChangeChartSpan(CurrentPair);
                    //DisplayChart(CurrentPair);
                }

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

        #region == チャートデータ用のプロパティ ==

        #region == 単一メインのデフォルトチャートデータ用のプロパティ ==
        // なし。LiveChartがおかしくなる。
        /*
        private SeriesCollection _chartSeries = new SeriesCollection();
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
        */

        #endregion

        #region == BTCチャートデータ用のプロパティ ==

        // === BTC === 
        private SeriesCollection _chartSeriesBtcJpy = new SeriesCollection();
        public SeriesCollection ChartSeriesBtcJpy
        {
            get
            {
                return _chartSeriesBtcJpy;
            }
            set
            {
                if (_chartSeriesBtcJpy == value)
                    return;

                _chartSeriesBtcJpy = value;
                this.NotifyPropertyChanged("ChartSeriesBtcJpy");
            }
        }

        private AxesCollection _chartAxisXBtcJpy = new AxesCollection();
        public AxesCollection ChartAxisXBtcJpy
        {
            get
            {
                return _chartAxisXBtcJpy;
            }
            set
            {
                if (_chartAxisXBtcJpy == value)
                    return;

                _chartAxisXBtcJpy = value;
                this.NotifyPropertyChanged("ChartAxisXBtcJpy");
            }
        }

        private AxesCollection _chartAxisYBtcJpy = new AxesCollection();
        public AxesCollection ChartAxisYBtcJpy
        {
            get
            {
                return _chartAxisYBtcJpy;
            }
            set
            {
                if (_chartAxisYBtcJpy == value)
                    return;

                _chartAxisYBtcJpy = value;
                this.NotifyPropertyChanged("ChartAxisYBtcJpy");
            }
        }

        // 一時間単位 
        private List<Ohlcv> _ohlcvsOneHourBtc = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneHourBtc
        {
            get { return _ohlcvsOneHourBtc; }
            set
            {
                _ohlcvsOneHourBtc = value;
                this.NotifyPropertyChanged("OhlcvsOneHourBtc");
            }
        }

        // 一分単位 
        private List<Ohlcv> _ohlcvsOneMinBtc = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneMinBtc
        {
            get { return _ohlcvsOneMinBtc; }
            set
            {
                _ohlcvsOneMinBtc = value;
                this.NotifyPropertyChanged("OhlcvsOneMinBtc");
            }
        }

        // 一日単位 
        private List<Ohlcv> _ohlcvsOneDayBtc = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneDayBtc
        {
            get { return _ohlcvsOneDayBtc; }
            set
            {
                _ohlcvsOneDayBtc = value;
                this.NotifyPropertyChanged("OhlcvsOneDayBtc");
            }
        }

        #endregion

        #region == LTCャートデータ用のプロパティ ==

        // === LTC === 
        private SeriesCollection _chartSeriesLtcBtc = new SeriesCollection();
        public SeriesCollection ChartSeriesLtcBtc
        {
            get
            {
                return _chartSeriesLtcBtc;
            }
            set
            {
                if (_chartSeriesLtcBtc == value)
                    return;

                _chartSeriesLtcBtc = value;
                this.NotifyPropertyChanged("ChartSeriesLtcBtc");
            }
        }

        private AxesCollection _chartAxisXLtcBtc = new AxesCollection();
        public AxesCollection ChartAxisXLtcBtc
        {
            get
            {
                return _chartAxisXLtcBtc;
            }
            set
            {
                if (_chartAxisXLtcBtc == value)
                    return;

                _chartAxisXLtcBtc = value;
                this.NotifyPropertyChanged("ChartAxisXLtcBtc");
            }
        }

        private AxesCollection _chartAxisYLtcBtc = new AxesCollection();
        public AxesCollection ChartAxisYLtcBtc
        {
            get
            {
                return _chartAxisYLtcBtc;
            }
            set
            {
                if (_chartAxisYLtcBtc == value)
                    return;

                _chartAxisYLtcBtc = value;
                this.NotifyPropertyChanged("ChartAxisYLtcBtc");
            }
        }

        // 一時間単位 
        private List<Ohlcv> _ohlcvsOneHourLtc = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneHourLtc
        {
            get { return _ohlcvsOneHourLtc; }
            set
            {
                _ohlcvsOneHourLtc = value;
                this.NotifyPropertyChanged("OhlcvsOneHourLtc");
            }
        }

        // 一分単位 
        private List<Ohlcv> _ohlcvsOneMinLtc = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneMinLtc
        {
            get { return _ohlcvsOneMinLtc; }
            set
            {
                _ohlcvsOneMinLtc = value;
                this.NotifyPropertyChanged("OhlcvsOneMinLtc");
            }
        }

        // 一日単位 
        private List<Ohlcv> _ohlcvsOneDayLtc = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneDayLtc
        {
            get { return _ohlcvsOneDayLtc; }
            set
            {
                _ohlcvsOneDayLtc = value;
                this.NotifyPropertyChanged("OhlcvsOneDayLtc");
            }
        }

        #endregion

        #region == XRPャートデータ用のプロパティ ==

        // === XRP === 
        private SeriesCollection _chartSeriesXrpJpy;
        public SeriesCollection ChartSeriesXrpJpy
        {
            get
            {
                return _chartSeriesXrpJpy;
            }
            set
            {
                if (_chartSeriesXrpJpy == value)
                    return;

                _chartSeriesXrpJpy = value;
                this.NotifyPropertyChanged("ChartSeriesXrp");
            }
        }

        private AxesCollection _chartAxisXXrpJpy;
        public AxesCollection ChartAxisXXrpJpy
        {
            get
            {
                return _chartAxisXXrpJpy;
            }
            set
            {
                if (_chartAxisXXrpJpy == value)
                    return;

                _chartAxisXXrpJpy = value;
                this.NotifyPropertyChanged("ChartAxisXXrpJpy");
            }
        }

        private AxesCollection _chartAxisYXrpJpy;
        public AxesCollection ChartAxisYXrpJpy
        {
            get
            {
                return _chartAxisYXrpJpy;
            }
            set
            {
                if (_chartAxisYXrpJpy == value)
                    return;

                _chartAxisYXrpJpy = value;
                this.NotifyPropertyChanged("ChartAxisYXrpJpy");
            }
        }

        // 一時間単位 
        private List<Ohlcv> _ohlcvsOneHourXrp = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneHourXrp
        {
            get { return _ohlcvsOneHourXrp; }
            set
            {
                _ohlcvsOneHourXrp = value;
                this.NotifyPropertyChanged("OhlcvsOneHourXrp");
            }
        }

        // 一分単位 
        private List<Ohlcv> _ohlcvsOneMinXrp = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneMinXrp
        {
            get { return _ohlcvsOneMinXrp; }
            set
            {
                _ohlcvsOneMinXrp = value;
                this.NotifyPropertyChanged("OhlcvsOneMinXrp");
            }
        }

        // 一日単位 
        private List<Ohlcv> _ohlcvsOneDayXrp = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneDayXrp
        {
            get { return _ohlcvsOneDayXrp; }
            set
            {
                _ohlcvsOneDayXrp = value;
                this.NotifyPropertyChanged("OhlcvsOneDayXrp");
            }
        }

        #endregion

        #region == Ethャートデータ用のプロパティ ==

        // === Eth === 
        private SeriesCollection _chartSeriesEthBtc;
        public SeriesCollection ChartSeriesEthBtc
        {
            get
            {
                return _chartSeriesEthBtc;
            }
            set
            {
                if (_chartSeriesEthBtc == value)
                    return;

                _chartSeriesEthBtc = value;
                this.NotifyPropertyChanged("ChartSeriesEthBtc");
            }
        }

        private AxesCollection _chartAxisXEthBtc;
        public AxesCollection ChartAxisXEthBtc
        {
            get
            {
                return _chartAxisXEthBtc;
            }
            set
            {
                if (_chartAxisXEthBtc == value)
                    return;

                _chartAxisXEthBtc = value;
                this.NotifyPropertyChanged("ChartAxisXEthBtc");
            }
        }

        private AxesCollection _chartAxisYEthBtc;
        public AxesCollection ChartAxisYEthBtc
        {
            get
            {
                return _chartAxisYEthBtc;
            }
            set
            {
                if (_chartAxisYEthBtc == value)
                    return;

                _chartAxisYEthBtc = value;
                this.NotifyPropertyChanged("ChartAxisYEthBtc");
            }
        }

        // 一時間単位 
        private List<Ohlcv> _ohlcvsOneHourEth = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneHourEth
        {
            get { return _ohlcvsOneHourEth; }
            set
            {
                _ohlcvsOneHourEth = value;
                this.NotifyPropertyChanged("OhlcvsOneHourEth");
            }
        }

        // 一分単位 
        private List<Ohlcv> _ohlcvsOneMinEth = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneMinEth
        {
            get { return _ohlcvsOneMinEth; }
            set
            {
                _ohlcvsOneMinEth = value;
                this.NotifyPropertyChanged("OhlcvsOneMinEth");
            }
        }

        // 一日単位 
        private List<Ohlcv> _ohlcvsOneDayEth = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneDayEth
        {
            get { return _ohlcvsOneDayEth; }
            set
            {
                _ohlcvsOneDayEth = value;
                this.NotifyPropertyChanged("OhlcvsOneDayEth");
            }
        }

        #endregion

        #region == Monaャートデータ用のプロパティ ==

        // === Mona === 
        private SeriesCollection _chartSeriesMonaJpy;
        public SeriesCollection ChartSeriesMonaJpy
        {
            get
            {
                return _chartSeriesMonaJpy;
            }
            set
            {
                if (_chartSeriesMonaJpy == value)
                    return;

                _chartSeriesMonaJpy = value;
                this.NotifyPropertyChanged("ChartSeriesMonaJpy");
            }
        }

        private AxesCollection _chartAxisXMonaJpy;
        public AxesCollection ChartAxisXMonaJpy
        {
            get
            {
                return _chartAxisXMonaJpy;
            }
            set
            {
                if (_chartAxisXMonaJpy == value)
                    return;

                _chartAxisXMonaJpy = value;
                this.NotifyPropertyChanged("ChartAxisXMonaJpy");
            }
        }

        private AxesCollection _chartAxisYMonaJpy;
        public AxesCollection ChartAxisYMonaJpy
        {
            get
            {
                return _chartAxisYMonaJpy;
            }
            set
            {
                if (_chartAxisYMonaJpy == value)
                    return;

                _chartAxisYMonaJpy = value;
                this.NotifyPropertyChanged("ChartAxisYMonaJpy");
            }
        }

        // 一時間単位 
        private List<Ohlcv> _ohlcvsOneHourMona = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneHourMona
        {
            get { return _ohlcvsOneHourMona; }
            set
            {
                _ohlcvsOneHourMona = value;
                this.NotifyPropertyChanged("OhlcvsOneHourMona");
            }
        }

        // 一分単位 
        private List<Ohlcv> _ohlcvsOneMinMona = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneMinMona
        {
            get { return _ohlcvsOneMinMona; }
            set
            {
                _ohlcvsOneMinMona = value;
                this.NotifyPropertyChanged("OhlcvsOneMinMona");
            }
        }

        // 一日単位 
        private List<Ohlcv> _ohlcvsOneDayMona = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneDayMona
        {
            get { return _ohlcvsOneDayMona; }
            set
            {
                _ohlcvsOneDayMona = value;
                this.NotifyPropertyChanged("OhlcvsOneDayMona");
            }
        }

        #endregion

        #region == Bchャートデータ用のプロパティ ==

        // === Bch === 
        private SeriesCollection _chartSeriesBchJpy;
        public SeriesCollection ChartSeriesBchJpy
        {
            get
            {
                return _chartSeriesBchJpy;
            }
            set
            {
                if (_chartSeriesBchJpy == value)
                    return;

                _chartSeriesBchJpy = value;
                this.NotifyPropertyChanged("ChartSeriesBchJpy");
            }
        }

        private AxesCollection _chartAxisXBchJpy;
        public AxesCollection ChartAxisXBchJpy
        {
            get
            {
                return _chartAxisXBchJpy;
            }
            set
            {
                if (_chartAxisXBchJpy == value)
                    return;

                _chartAxisXBchJpy = value;
                this.NotifyPropertyChanged("ChartAxisXBchJpy");
            }
        }

        private AxesCollection _chartAxisYBchJpy;
        public AxesCollection ChartAxisYBchJpy
        {
            get
            {
                return _chartAxisYBchJpy;
            }
            set
            {
                if (_chartAxisYBchJpy == value)
                    return;

                _chartAxisYBchJpy = value;
                this.NotifyPropertyChanged("ChartAxisYBchJpy");
            }
        }

        // 一時間単位 
        private List<Ohlcv> _ohlcvsOneHourBch = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneHourBch
        {
            get { return _ohlcvsOneHourBch; }
            set
            {
                _ohlcvsOneHourBch = value;
                this.NotifyPropertyChanged("OhlcvsOneHourBch");
            }
        }

        // 一分単位 
        private List<Ohlcv> _ohlcvsOneMinBch = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneMinBch
        {
            get { return _ohlcvsOneMinBch; }
            set
            {
                _ohlcvsOneMinBch = value;
                this.NotifyPropertyChanged("OhlcvsOneMinBch");
            }
        }

        // 一日単位 
        private List<Ohlcv> _ohlcvsOneDayBch = new List<Ohlcv>();
        public List<Ohlcv> OhlcvsOneDayBch
        {
            get { return _ohlcvsOneDayBch; }
            set
            {
                _ohlcvsOneDayBch = value;
                this.NotifyPropertyChanged("OhlcvsOneDayBch");
            }
        }

        #endregion

        #endregion

        #endregion

        #region == タイマー 変数宣言 ==

        System.Windows.Threading.DispatcherTimer dispatcherTimerTickAllPairs = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer dispatcherChartTimer = new System.Windows.Threading.DispatcherTimer();

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

        public MainViewModel()
        {

            #region == コマンドのイニシャライズ ==

            #region == 認証関係コマンドのイニシャライズ ==

            EscCommand = new RelayCommand(EscCommand_Execute, EscCommand_CanExecute);

            ShowSettingsCommand = new RelayCommand(ShowSettingsCommand_Execute, ShowSettingsCommand_CanExecute);
            SettingsCancelCommand = new RelayCommand(SettingsCancelCommand_Execute, SettingsCancelCommand_CanExecute);
            SettingsOKCommand = new RelayCommand(SettingsOKCommand_Execute, SettingsOKCommand_CanExecute);

            LogInCommand = new GenericRelayCommand<object>(
                param => LogInCommand_Execute(param),
                param => LogInCommand_CanExecute());

            LogOutCommand = new RelayCommand(LogOutCommand_Execute, LogOutCommand_CanExecute);
            ShowLogInCommand = new RelayCommand(ShowLogInCommand_Execute, ShowLogInCommand_CanExecute);

            NewLogInPasswordCommand = new GenericRelayCommand<object>(
                param => NewLogInPasswordCommand_Execute(param),
                param => LogInCommand_CanExecute());

            LoginCancelCommand = new RelayCommand(LoginCancelCommand_Execute, LoginCancelCommand_CanExecute);

            ChangeLogInPasswordCommand = new GenericRelayCommand<object>(
                param => ChangeLogInPasswordCommand_Execute(param),
                param => ChangeLogInPasswordCommand_CanExecute());

            #endregion

            FeedOpenJaCommand = new RelayCommand(FeedOpenJaCommand_Execute, FeedOpenJaCommand_CanExecute);
            FeedOpenEnCommand = new RelayCommand(FeedOpenEnCommand_Execute, FeedOpenEnCommand_CanExecute);

            DepthGroupingCommand = new GenericRelayCommand<object>(
                param => DepthGroupingCommand_Execute(param),
                param => DepthGroupingCommand_CanExecute());

            #region == APIKey ==

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

            ViewMinimumCommand = new RelayCommand(ViewMinimumCommand_Execute, ViewMinimumCommand_CanExecute);
            ViewRestoreCommand = new RelayCommand(ViewRestoreCommand_Execute, ViewRestoreCommand_CanExecute);

            #endregion

            #region == テーマのイニシャライズ ==

            // テーマの選択コンボボックスのイニシャライズ
            _themes = new ObservableCollection<Theme>()
            {
                new Theme() { Id = 1, Name = "DefaultTheme", Label = "Default"},
                new Theme() { Id = 2, Name = "LightTheme", Label = "Light"}
            };
            // デフォルトにセット
            _currentTheme = _themes[0];

            #endregion

            #region == APIクライアントのイニシャライズ ==

            // プライベートAPIクライアントのイニシャライズ
            //_priApi = new PrivateAPIClient();

            // エラーイベントにサブスクライブ
            //this._priApi.ErrorOccured += new PrivateAPIClient.ClinetErrorEvent(OnError);

            #endregion

            #region == ObservableCollection collections のスレッド対応 ==

            // スレッド対応 ObservableCollection collection
            BindingOperations.EnableCollectionSynchronization(this._bitcoinNewsJa, new object());
            BindingOperations.EnableCollectionSynchronization(this._bitcoinNewsEn, new object());
            BindingOperations.EnableCollectionSynchronization(this._depth, new object());
            BindingOperations.EnableCollectionSynchronization(this._transactions, new object());
            BindingOperations.EnableCollectionSynchronization(this._errors, new object());

            #endregion

            #region == チャートのイニシャライズ ==

            // 各通貨ペアをループ
            foreach (Pairs pair in Enum.GetValues(typeof(Pairs)))
            {
                // Axes
                AxesCollection chartAxisX = new AxesCollection();
                AxesCollection chartAxisY = new AxesCollection();

                // 日時 X
                Axis caX = new Axis();
                caX.Name = "日時";
                caX.Title = "";
                caX.MaxValue = 60;
                caX.MinValue = 0;
                caX.Labels = new List<string>();
                //caX.Separator.StrokeThickness = 0.1;
                //caX.Separator.StrokeDashArray = new DoubleCollection { 4 };
                caX.Separator.IsEnabled = false;
                caX.IsMerged = false;

                //caX.Foreground = new SolidColorBrush(_chartStrokeColor);
                Style styleX = Application.Current.FindResource("ChartAxisStyle") as Style;
                caX.Style = styleX;

                caX.DisableAnimations = true;

                //ChartAxisX.Add(caX);
                chartAxisX.Add(caX);

                // 価格 Y
                Axis caY = new Axis();
                caY.Name = "価格";
                caY.Title = "";
                caY.MaxValue = double.NaN;
                caY.MinValue = double.NaN;
                caY.Position = AxisPosition.RightTop;
                caY.Separator.StrokeThickness = 0.1;
                caY.Separator.StrokeDashArray = new DoubleCollection { 4 };
                caY.IsMerged = false;

                //caY.Separator.Stroke = new SolidColorBrush(_chartStrokeColor);//System.Windows.Media.Brushes.WhiteSmoke;
                Style styleYSec = Application.Current.FindResource("ChartSeparatorStyle") as Style;
                caY.Separator.Style = styleYSec;

                //caY.Foreground = new SolidColorBrush(_chartStrokeColor);
                Style styleY = Application.Current.FindResource("ChartAxisStyle") as Style;
                caY.Style = styleY;

                caY.DisableAnimations = true;

                //ChartAxisY.Add(caY);
                chartAxisY.Add(caY);

                // 出来高 Y
                Axis vaY = new Axis();
                vaY.Name = "出来高";
                vaY.Title = "";
                vaY.ShowLabels = false;
                vaY.Labels = null;
                vaY.MaxValue = double.NaN;
                vaY.MinValue = double.NaN;
                vaY.Position = AxisPosition.RightTop;
                //vaY.Separator.StrokeThickness = 0;
                vaY.Separator.IsEnabled = false;
                vaY.IsMerged = true;
                vaY.DisableAnimations = true;

                //ChartAxisY.Add(vaY);
                chartAxisY.Add(vaY);

                // sections

                // 現在値セクション
                AxisSection axs = new AxisSection();
                axs.Value = 0;//double.NaN;//(double)_ltp;
                axs.Width = 0;
                //axs.SectionWidth = 0;
                axs.StrokeThickness = 0.6;
                axs.StrokeDashArray = new DoubleCollection { 4 };
                axs.Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(150, 172, 206));
                Style styleSection = Application.Current.FindResource("ChartSectionStyle") as Style;
                axs.Style = styleSection;

                axs.DataLabel = false;
                //axs.DataLabelForeground = new SolidColorBrush(Colors.Black);
                axs.DisableAnimations = true;

                //ChartAxisY[0].Sections.Add(axs);
                chartAxisY[0].Sections.Add(axs);

                // 出来高グラフ色
                SolidColorBrush volumeColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 0));
                volumeColorBrush.Opacity = 0.1;

                // Lines
                SeriesCollection chartSeries = new SeriesCollection()
                {

                    new CandleSeries()
                    {
                        Title = PairStrings[pair],
                        Values = new ChartValues<OhlcPoint>{},
                        Fill = Brushes.Transparent,
                        ScalesYAt = 0,
                        //IncreaseBrush = new SolidColorBrush(_priceUpColor),//System.Windows.Media.Brushes.Aqua,
                        //DecreaseBrush = new SolidColorBrush(_priceDownColor),//System.Windows.Media.Brushes.Pink,
                        //IncreaseBrush = new SolidColorBrush(_chartIncreaseColor)
                        //DecreaseBrush = new SolidColorBrush(_chartDecreaseColor),
                        
                    },

                     new ColumnSeries
                    {
                        Title = "出来高",
                        Values = new ChartValues<double> {},
                        ScalesYAt = 1,
                        //Fill = volumeColorBrush,
                        Style = (Application.Current.FindResource("ChartVolumeStyle") as Style),
                    }

                };

                if (pair == Pairs.btc_jpy)
                {
                    ChartSeriesBtcJpy = chartSeries;
                    ChartAxisXBtcJpy = chartAxisX;
                    ChartAxisYBtcJpy = chartAxisY;
                }
                else if (pair == Pairs.xrp_jpy)
                {
                    ChartSeriesXrpJpy = chartSeries;
                    ChartAxisXXrpJpy = chartAxisX;
                    ChartAxisYXrpJpy = chartAxisY;
                }
                else if (pair == Pairs.eth_btc)
                {
                    ChartSeriesEthBtc = chartSeries;
                    ChartAxisXEthBtc = chartAxisX;
                    ChartAxisYEthBtc = chartAxisY;
                }
                else if (pair == Pairs.mona_jpy)
                {
                    ChartSeriesMonaJpy = chartSeries;
                    ChartAxisXMonaJpy = chartAxisX;
                    ChartAxisYMonaJpy = chartAxisY;
                }
                else if (pair == Pairs.mona_btc)
                {
                    //
                }
                else if (pair == Pairs.ltc_btc)
                {
                    ChartSeriesLtcBtc = chartSeries;
                    ChartAxisXLtcBtc = chartAxisX;
                    ChartAxisYLtcBtc = chartAxisY;
                }
                else if (pair == Pairs.bcc_btc)
                {
                    //
                }
                else if (pair == Pairs.bcc_jpy)
                {
                    ChartSeriesBchJpy = chartSeries;
                    ChartAxisXBchJpy = chartAxisX;
                    ChartAxisYBchJpy = chartAxisY;
                }

            }

            #endregion

            // Tickerのタイマー起動
            dispatcherTimerTickAllPairs.Tick += new EventHandler(TickerTimerAllPairs);
            dispatcherTimerTickAllPairs.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimerTickAllPairs.Start();
            
            // Chart更新のタイマー
            dispatcherChartTimer.Tick += new EventHandler(ChartTimer);
            dispatcherChartTimer.Interval = new TimeSpan(0, 1, 0);
            // Start Timer later.

            // 初回RSS フィードの取得
            Task.Run(() => GetRss());

            // RSSのタイマー起動
            System.Windows.Threading.DispatcherTimer dispatcherTimerRss = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimerRss.Tick += new EventHandler(RssTimer);
            dispatcherTimerRss.Interval = new TimeSpan(0, 15, 0);
            dispatcherTimerRss.Start();

            // ループ再生開始　
            StartLoop();

            // TODO TEMP
            ActivePairIndex = 0;
            CurrentPair = Pairs.btc_jpy;
            ActivePair = PairBtcJpy;
            ActivePair.Ltp = PairBtcJpy.Ltp;

            IsBtcVisible = true;
            IsXrpVisible = false;
            IsEthVisible = false;
            IsLtcVisible = false;
            IsMonaJpyVisible = false;
            IsBchJpyVisible = false;

            ShowAllCharts = false;
            ShowMainContents = true;
        }

        #region == イベント・タイマー系 ==

        // 現在価格取得 Tickerタイマー
        private async void TickerTimerAllPairs(object source, EventArgs e)
        {
            try
            {
                // 各通貨ペアをループ
                //foreach (string pair in Enum.GetNames(typeof(Pairs)))
                foreach (Pairs pair in Enum.GetValues(typeof(Pairs)))
                {
                    if ((pair == Pairs.mona_btc) || pair == Pairs.bcc_btc)
                    {
                        continue;
                    }

                    Ticker tick = await _pubTickerApi.GetTicker(pair.ToString());

                    if (tick != null)
                    {
                        // Ticker 取得エラー表示をクリア
                        APIResultTicker = "";

                        try
                        {

                            if (pair == CurrentPair)
                            {
                                if (tick.LTP > ActivePair.Ltp)
                                {
                                    ActivePair.LtpUpFlag = true;
                                }
                                else if (tick.LTP < ActivePair.Ltp)
                                {
                                    ActivePair.LtpUpFlag = false;
                                }
                                else if (tick.LTP == ActivePair.Ltp)
                                {
                                    //ActivePair.LtpColor = Colors.Gainsboro;
                                }
                              
                            }

                            if (pair == Pairs.btc_jpy)
                            {
                                // 最新の価格をセット
                                PairBtcJpy.Ltp = tick.LTP;
                                PairBtcJpy.Bid = tick.Bid;
                                PairBtcJpy.Ask = tick.Ask;
                                PairBtcJpy.TickTimeStamp = tick.TimeStamp;

                                PairBtcJpy.LowestIn24Price = tick.Low;
                                PairBtcJpy.HighestIn24Price = tick.High;

                                // 起動時価格セット
                                if (PairBtcJpy.BasePrice == 0) PairBtcJpy.BasePrice = tick.LTP;

                                /*
                                // ビットコイン時価評価額の計算
                                if (AssetBTCAmount != 0)
                                {
                                    AssetBTCEstimateAmount = _ltp * AssetBTCAmount;
                                }
                                */

                                // 最安値登録
                                if (PairBtcJpy.LowestPrice == 0)
                                {
                                    PairBtcJpy.LowestPrice = tick.LTP;
                                }
                                if (tick.LTP < PairBtcJpy.LowestPrice)
                                {
                                    //SystemSounds.Beep.Play();
                                    PairBtcJpy.LowestPrice = tick.LTP;
                                }

                                // 最高値登録
                                if (PairBtcJpy.HighestPrice == 0)
                                {
                                    PairBtcJpy.HighestPrice = tick.LTP;
                                }
                                if (tick.LTP > PairBtcJpy.HighestPrice)
                                {
                                    //SystemSounds.Asterisk.Play();
                                    PairBtcJpy.HighestPrice = tick.LTP;
                                }

                                #region == チック履歴 ==

                                TickHistory aym = new TickHistory();
                                aym.Price = tick.LTP;
                                aym.TimeAt = tick.TimeStamp;
                                if (PairBtcJpy.TickHistories.Count > 0)
                                {
                                    if (PairBtcJpy.TickHistories[0].Price > aym.Price)
                                    {
                                        //aym.TickHistoryPriceColor = _priceUpColor;
                                        aym.TickHistoryPriceUp = true;
                                        PairBtcJpy.TickHistories.Insert(0, aym);

                                    }
                                    else if (PairBtcJpy.TickHistories[0].Price < aym.Price)
                                    {
                                        //aym.TickHistoryPriceColor = _priceDownColor;
                                        aym.TickHistoryPriceUp = false;
                                        PairBtcJpy.TickHistories.Insert(0, aym);
                                    }
                                    else
                                    {
                                        //aym.TickHistoryPriceColor = Colors.Gainsboro;
                                        PairBtcJpy.TickHistories.Insert(0, aym);
                                    }
                                }
                                else
                                {
                                    //aym.TickHistoryPriceColor = Colors.Gainsboro;
                                    PairBtcJpy.TickHistories.Insert(0, aym);
                                }

                                // limit the number of the list.
                                if (PairBtcJpy.TickHistories.Count > 60)
                                {
                                    PairBtcJpy.TickHistories.RemoveAt(60);
                                }

                                // 60(1分)の平均値を求める
                                decimal aSum = 0;
                                int c = 0;
                                if (PairBtcJpy.TickHistories.Count > 0)
                                {

                                    if (PairBtcJpy.TickHistories.Count > 60)
                                    {
                                        c = 59;
                                    }
                                    else
                                    {
                                        c = PairBtcJpy.TickHistories.Count - 1;
                                    }

                                    if (c == 0)
                                    {
                                        PairBtcJpy.AveragePrice = PairBtcJpy.TickHistories[0].Price;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < c; i++)
                                        {
                                            aSum = aSum + PairBtcJpy.TickHistories[i].Price;
                                        }
                                        PairBtcJpy.AveragePrice = aSum / c;
                                    }

                                }
                                else if (PairBtcJpy.TickHistories.Count == 1)
                                {
                                    PairBtcJpy.AveragePrice = PairBtcJpy.TickHistories[0].Price;
                                }

                                #endregion

                                #region == アラーム ==

                                // TODO 非表示の通貨の場合はどうする？？？
                                if (pair == CurrentPair)
                                {

                                    bool isPlayed = false;

                                    // アラーム
                                    if (PairBtcJpy.AlarmPlus > 0)
                                    {
                                        if (tick.LTP >= PairBtcJpy.AlarmPlus)
                                        {
                                            PairBtcJpy.HighLowInfoTextColorFlag = true;
                                            PairBtcJpy.HighLowInfoText = "⇑⇑⇑　高値アラーム " + PairBtcJpy.CurrentPairString;

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
                                        PairBtcJpy.AlarmPlus = ((long)(tick.LTP / 1000) * 1000) + 20000;
                                    }

                                    if (PairBtcJpy.AlarmMinus > 0)
                                    {
                                        if (tick.LTP <= PairBtcJpy.AlarmMinus)
                                        {
                                            PairBtcJpy.HighLowInfoTextColorFlag = false;
                                            PairBtcJpy.HighLowInfoText = "⇓⇓⇓　安値アラーム " + PairBtcJpy.CurrentPairString;
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
                                        PairBtcJpy.AlarmMinus = ((long)(tick.LTP / 1000) * 1000) - 10000;
                                    }

                                    // 起動後最高値
                                    if (tick.LTP >= PairBtcJpy.HighestPrice)
                                    {
                                        if ((PairBtcJpy.TickHistories.Count > 25) && ((PairBtcJpy.BasePrice + 2000M) < tick.LTP))
                                        {
                                            PairBtcJpy.HighLowInfoTextColorFlag = true;
                                            PairBtcJpy.HighLowInfoText = "⇑⇑⇑　起動後最高値更新 " + PairBtcJpy.CurrentPairString;

                                            if ((isPlayed == false) && (PairBtcJpy.PlaySoundHighest == true))
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
                                    if (tick.LTP <= PairBtcJpy.LowestPrice)
                                    {
                                        if ((PairBtcJpy.TickHistories.Count > 25) && ((PairBtcJpy.BasePrice - 2000M) > tick.LTP))
                                        {
                                            PairBtcJpy.HighLowInfoTextColorFlag = false;
                                            PairBtcJpy.HighLowInfoText = "⇓⇓⇓　起動後最安値更新 " + PairBtcJpy.CurrentPairString;

                                            if ((isPlayed == false) && (PairBtcJpy.PlaySoundLowest == true))
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
                                    if (tick.LTP >= PairBtcJpy.HighestIn24Price)
                                    {
                                        PairBtcJpy.HighLowInfoTextColorFlag = true;
                                        PairBtcJpy.HighLowInfoText = "⇑⇑⇑⇑⇑⇑　過去24時間最高値更新 " + PairBtcJpy.CurrentPairString;

                                        if ((isPlayed == false) && (PairBtcJpy.PlaySoundHighest24h == true))
                                        {
                                            if (PlaySound)
                                            {
                                                SystemSounds.Hand.Play();
                                                isPlayed = true;
                                            }
                                        }
                                    }
                                    // 過去24時間最安値
                                    if (tick.LTP <= PairBtcJpy.LowestIn24Price)
                                    {
                                        PairBtcJpy.HighLowInfoTextColorFlag = false;
                                        PairBtcJpy.HighLowInfoText = "⇓⇓⇓⇓⇓⇓　過去24時間最安値更新 " + PairBtcJpy.CurrentPairString;

                                        if ((isPlayed == false) && (PairBtcJpy.PlaySoundLowest24h == true))
                                        {
                                            if (PlaySound)
                                            {
                                                SystemSounds.Hand.Play();
                                                isPlayed = true;
                                            }
                                        }
                                    }

                                }

                                #endregion

                                // 省エネモードでなかったら
                                if ((MinMode == false) && (pair == CurrentPair))
                                {
                                    // 最新取引価格のラインを更新
                                    if (ChartAxisYBtcJpy != null)
                                    {
                                        if (ChartAxisYBtcJpy.Count > 0)
                                        {
                                            if (ChartAxisYBtcJpy[0].Sections.Count > 0)
                                            {
                                                ChartAxisYBtcJpy[0].Sections[0].Value = (double)tick.LTP;
                                            }
                                        }

                                        // 最新のロウソク足を更新する。
                                        //＞＞＞重すぎ。負荷掛かり過ぎなので止め。
                                        /*
                                        if (ChartSeriesBtcJpy != null)
                                        {
                                            if (ChartSeriesBtcJpy[0].Values != null)
                                            {
                                                int cb = ChartSeriesBtcJpy[0].Values.Count;

                                                if (cb > 0)
                                                {
                                                    double l = ((OhlcPoint)ChartSeriesBtcJpy[0].Values[cb - 1]).Low;
                                                    double h = ((OhlcPoint)ChartSeriesBtcJpy[0].Values[cb - 1]).High;

                                                    if (Application.Current == null) return;
                                                    Application.Current.Dispatcher.Invoke(() =>
                                                    {

                                                        ((OhlcPoint)ChartSeriesBtcJpy[0].Values[cb - 1]).Close = (double)tick.LTP;

                                                        if (l > (double)tick.LTP)
                                                        {
                                                            ((OhlcPoint)ChartSeriesBtcJpy[0].Values[cb - 1]).Low = (double)tick.LTP;
                                                        }

                                                        if (h < (double)tick.LTP)
                                                        {
                                                            ((OhlcPoint)ChartSeriesBtcJpy[0].Values[cb - 1]).High = (double)tick.LTP;
                                                        }

                                                    });

                                                }
                                            }

                                        }
                                        */

                                    }

                                    /*
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
                                    */



                                }

                            }
                            else if (pair == Pairs.xrp_jpy)
                            {

                                // 最新の価格をセット
                                PairXrpJpy.Ltp = tick.LTP;
                                PairXrpJpy.Bid = tick.Bid;
                                PairXrpJpy.Ask = tick.Ask;
                                PairXrpJpy.TickTimeStamp = tick.TimeStamp;

                                PairXrpJpy.LowestIn24Price = tick.Low;
                                PairXrpJpy.HighestIn24Price = tick.High;

                                // 起動時価格セット
                                if (PairXrpJpy.BasePrice == 0) PairXrpJpy.BasePrice = tick.LTP;

                                /*
                                // ビットコイン時価評価額の計算
                                if (AssetBTCAmount != 0)
                                {
                                    AssetBTCEstimateAmount = _ltp * AssetBTCAmount;
                                }
                                */

                                // 最安値登録
                                if (PairXrpJpy.LowestPrice == 0)
                                {
                                    PairXrpJpy.LowestPrice = tick.LTP;
                                }
                                if (tick.LTP < PairXrpJpy.LowestPrice)
                                {
                                    //SystemSounds.Beep.Play();
                                    PairXrpJpy.LowestPrice = tick.LTP;
                                }

                                // 最高値登録
                                if (PairXrpJpy.HighestPrice == 0)
                                {
                                    PairXrpJpy.HighestPrice = tick.LTP;
                                }
                                if (tick.LTP > PairXrpJpy.HighestPrice)
                                {
                                    //SystemSounds.Asterisk.Play();
                                    PairXrpJpy.HighestPrice = tick.LTP;
                                }

                                #region == チック履歴 ==

                                TickHistory aym = new TickHistory();
                                aym.Price = tick.LTP;
                                aym.TimeAt = tick.TimeStamp;
                                if (PairXrpJpy.TickHistories.Count > 0)
                                {
                                    if (PairXrpJpy.TickHistories[0].Price > aym.Price)
                                    {
                                        //aym.TickHistoryPriceColor = _priceUpColor;
                                        aym.TickHistoryPriceUp = true;
                                        PairXrpJpy.TickHistories.Insert(0, aym);

                                    }
                                    else if (PairXrpJpy.TickHistories[0].Price < aym.Price)
                                    {
                                        //aym.TickHistoryPriceColor = _priceDownColor;
                                        aym.TickHistoryPriceUp = false;
                                        PairXrpJpy.TickHistories.Insert(0, aym);
                                    }
                                    else
                                    {
                                        //aym.TickHistoryPriceColor = Colors.Gainsboro;
                                        PairXrpJpy.TickHistories.Insert(0, aym);
                                    }
                                }
                                else
                                {
                                    //aym.TickHistoryPriceColor = Colors.Gainsboro;
                                    PairXrpJpy.TickHistories.Insert(0, aym);
                                }

                                // limit the number of the list.
                                if (PairXrpJpy.TickHistories.Count > 60)
                                {
                                    PairXrpJpy.TickHistories.RemoveAt(60);
                                }

                                // 60(1分)の平均値を求める
                                decimal aSum = 0;
                                int c = 0;
                                if (PairXrpJpy.TickHistories.Count > 0)
                                {

                                    if (PairXrpJpy.TickHistories.Count > 60)
                                    {
                                        c = 59;
                                    }
                                    else
                                    {
                                        c = PairXrpJpy.TickHistories.Count - 1;
                                    }

                                    if (c == 0)
                                    {
                                        PairXrpJpy.AveragePrice = PairXrpJpy.TickHistories[0].Price;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < c; i++)
                                        {
                                            aSum = aSum + PairXrpJpy.TickHistories[i].Price;
                                        }
                                        PairXrpJpy.AveragePrice = aSum / c;
                                    }

                                }
                                else if (PairXrpJpy.TickHistories.Count == 1)
                                {
                                    PairXrpJpy.AveragePrice = PairXrpJpy.TickHistories[0].Price;
                                }

                                #endregion

                                #region == アラーム ==

                                // TODO 非表示の通貨の場合はどうする？？？
                                if (pair == CurrentPair)
                                {

                                    bool isPlayed = false;

                                    // アラーム
                                    if (PairXrpJpy.AlarmPlus > 0)
                                    {
                                        if (tick.LTP >= PairXrpJpy.AlarmPlus)
                                        {
                                            PairXrpJpy.HighLowInfoTextColorFlag = true;
                                            PairXrpJpy.HighLowInfoText = "⇑⇑⇑　高値アラーム " + PairXrpJpy.CurrentPairString;

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
                                        PairXrpJpy.AlarmPlus = (long)(tick.LTP) + 2M;
                                    }

                                    if (PairXrpJpy.AlarmMinus > 0)
                                    {
                                        if (tick.LTP <= PairXrpJpy.AlarmMinus)
                                        {
                                            PairXrpJpy.HighLowInfoTextColorFlag = false;
                                            PairXrpJpy.HighLowInfoText = "⇓⇓⇓　安値アラーム " + PairXrpJpy.CurrentPairString;
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
                                        PairXrpJpy.AlarmMinus = (long)(tick.LTP) - 2M;
                                    }

                                    // 起動後最高値
                                    if (tick.LTP >= PairXrpJpy.HighestPrice)
                                    {
                                        if ((PairXrpJpy.TickHistories.Count > 25) && ((PairXrpJpy.BasePrice + 0.3M) < tick.LTP))
                                        {
                                            PairXrpJpy.HighLowInfoTextColorFlag = true;
                                            PairXrpJpy.HighLowInfoText = "⇑⇑⇑　起動後最高値更新 " + PairXrpJpy.CurrentPairString;

                                            if ((isPlayed == false) && (PairXrpJpy.PlaySoundHighest == true))
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
                                    if (tick.LTP <= PairXrpJpy.LowestPrice)
                                    {
                                        if ((PairXrpJpy.TickHistories.Count > 25) && ((PairXrpJpy.BasePrice - 0.3M) > tick.LTP))
                                        {
                                            PairXrpJpy.HighLowInfoTextColorFlag = false;
                                            PairXrpJpy.HighLowInfoText = "⇓⇓⇓　起動後最安値更新 " + PairXrpJpy.CurrentPairString;

                                            if ((isPlayed == false) && (PairXrpJpy.PlaySoundLowest == true))
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
                                    if (tick.LTP >= PairXrpJpy.HighestIn24Price)
                                    {
                                        PairXrpJpy.HighLowInfoTextColorFlag = true;
                                        PairXrpJpy.HighLowInfoText = "⇑⇑⇑⇑⇑⇑　過去24時間最高値更新 " + PairXrpJpy.CurrentPairString;

                                        if ((isPlayed == false) && (PairXrpJpy.PlaySoundHighest24h == true))
                                        {
                                            if (PlaySound)
                                            {
                                                SystemSounds.Hand.Play();
                                                isPlayed = true;
                                            }
                                        }
                                    }
                                    // 過去24時間最安値
                                    if (tick.LTP <= PairXrpJpy.LowestIn24Price)
                                    {
                                        PairXrpJpy.HighLowInfoTextColorFlag = false;
                                        PairXrpJpy.HighLowInfoText = "⇓⇓⇓⇓⇓⇓　過去24時間最安値更新 " + PairXrpJpy.CurrentPairString;

                                        if ((isPlayed == false) && (PairXrpJpy.PlaySoundLowest24h == true))
                                        {
                                            if (PlaySound)
                                            {
                                                SystemSounds.Hand.Play();
                                                isPlayed = true;
                                            }
                                        }
                                    }

                                }

                                #endregion

                                // 省エネモードでなかったら
                                if ((MinMode == false) && (pair == CurrentPair))
                                {
                                    // 最新取引価格のラインを更新
                                    if (ChartAxisYXrpJpy != null)
                                    {
                                        if (ChartAxisYXrpJpy[0].Sections.Count > 0)
                                        {
                                            ChartAxisYXrpJpy[0].Sections[0].Value = (double)tick.LTP;
                                        }
                                    }

                                    /*
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
                                    */


                                    // 最新のロウソク足を更新する。＞＞＞重すぎ。負荷掛かり過ぎなので止め。
                                    /*
                                    if (ChartSeriesBtcJpy[0].Values != null)
                                    {
                                        int c = ChartSeriesBtcJpy[0].Values.Count;

                                        if (c > 0)
                                        {
                                            double l = ((OhlcPoint)ChartSeriesBtcJpy[0].Values[c - 1]).Low;
                                            double h = ((OhlcPoint)ChartSeriesBtcJpy[0].Values[c - 1]).High;

                                            if (Application.Current == null) return;
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {

                                                ((OhlcPoint)ChartSeriesBtcJpy[0].Values[c - 1]).Close = (double)tick.LTP;

                                                if (l > (double)tick.LTP)
                                                {
                                                    ((OhlcPoint)ChartSeriesBtcJpy[0].Values[c - 1]).Low = (double)tick.LTP;
                                                }

                                                if (h < (double)tick.LTP)
                                                {
                                                    ((OhlcPoint)ChartSeriesBtcJpy[0].Values[c - 1]).High = (double)tick.LTP;
                                                }

                                            });

                                        }
                                    }
                                    */
                                }

                            }
                            else if (pair == Pairs.eth_btc)
                            {

                                // 最新の価格をセット
                                PairEthBtc.Ltp = tick.LTP;
                                PairEthBtc.Bid = tick.Bid;
                                PairEthBtc.Ask = tick.Ask;
                                PairEthBtc.TickTimeStamp = tick.TimeStamp;

                                PairEthBtc.LowestIn24Price = tick.Low;
                                PairEthBtc.HighestIn24Price = tick.High;

                                // 起動時価格セット
                                if (PairEthBtc.BasePrice == 0) PairEthBtc.BasePrice = tick.LTP;

                                /*
                                // ビットコイン時価評価額の計算
                                if (AssetBTCAmount != 0)
                                {
                                    AssetBTCEstimateAmount = _ltp * AssetBTCAmount;
                                }
                                */

                                // 最安値登録
                                if (PairEthBtc.LowestPrice == 0)
                                {
                                    PairEthBtc.LowestPrice = tick.LTP;
                                }
                                if (tick.LTP < PairEthBtc.LowestPrice)
                                {
                                    //SystemSounds.Beep.Play();
                                    PairEthBtc.LowestPrice = tick.LTP;
                                }

                                // 最高値登録
                                if (PairEthBtc.HighestPrice == 0)
                                {
                                    PairEthBtc.HighestPrice = tick.LTP;
                                }
                                if (tick.LTP > PairEthBtc.HighestPrice)
                                {
                                    //SystemSounds.Asterisk.Play();
                                    PairEthBtc.HighestPrice = tick.LTP;
                                }
                                
                                #region == チック履歴 ==

                                TickHistory aym = new TickHistory();
                                aym.Price = tick.LTP;
                                aym.TimeAt = tick.TimeStamp;
                                if (PairEthBtc.TickHistories.Count > 0)
                                {
                                    if (PairEthBtc.TickHistories[0].Price > aym.Price)
                                    {
                                        //aym.TickHistoryPriceColor = _priceUpColor;
                                        aym.TickHistoryPriceUp = true;
                                        PairEthBtc.TickHistories.Insert(0, aym);

                                    }
                                    else if (PairEthBtc.TickHistories[0].Price < aym.Price)
                                    {
                                        //aym.TickHistoryPriceColor = _priceDownColor;
                                        aym.TickHistoryPriceUp = false;
                                        PairEthBtc.TickHistories.Insert(0, aym);
                                    }
                                    else
                                    {
                                        //aym.TickHistoryPriceColor = Colors.Gainsboro;
                                        PairEthBtc.TickHistories.Insert(0, aym);
                                    }
                                }
                                else
                                {
                                    //aym.TickHistoryPriceColor = Colors.Gainsboro;
                                    PairEthBtc.TickHistories.Insert(0, aym);
                                }

                                // limit the number of the list.
                                if (PairEthBtc.TickHistories.Count > 60)
                                {
                                    PairEthBtc.TickHistories.RemoveAt(60);
                                }

                                // 60(1分)の平均値を求める
                                decimal aSum = 0;
                                int c = 0;
                                if (PairEthBtc.TickHistories.Count > 0)
                                {

                                    if (PairEthBtc.TickHistories.Count > 60)
                                    {
                                        c = 59;
                                    }
                                    else
                                    {
                                        c = PairEthBtc.TickHistories.Count - 1;
                                    }

                                    if (c == 0)
                                    {
                                        PairEthBtc.AveragePrice = PairEthBtc.TickHistories[0].Price;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < c; i++)
                                        {
                                            aSum = aSum + PairEthBtc.TickHistories[i].Price;
                                        }
                                        PairEthBtc.AveragePrice = aSum / c;
                                    }

                                }
                                else if (PairEthBtc.TickHistories.Count == 1)
                                {
                                    PairEthBtc.AveragePrice = PairEthBtc.TickHistories[0].Price;
                                }

                                #endregion

                                #region == アラーム ==

                                // TODO 非表示の通貨の場合はどうする？？？
                                if (pair == CurrentPair)
                                {

                                    bool isPlayed = false;

                                    // アラーム
                                    if (PairEthBtc.AlarmPlus > 0)
                                    {
                                        if (tick.LTP >= PairEthBtc.AlarmPlus)
                                        {
                                            PairEthBtc.HighLowInfoTextColorFlag = true;
                                            PairEthBtc.HighLowInfoText = "⇑⇑⇑　高値アラーム " + PairEthBtc.CurrentPairString;

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
                                        PairEthBtc.AlarmPlus = tick.LTP + 0.002M;
                                    }

                                    if (PairEthBtc.AlarmMinus > 0)
                                    {
                                        if (tick.LTP <= PairEthBtc.AlarmMinus)
                                        {
                                            PairEthBtc.HighLowInfoTextColorFlag = false;
                                            PairEthBtc.HighLowInfoText = "⇓⇓⇓　安値アラーム " + PairEthBtc.CurrentPairString;
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
                                        PairEthBtc.AlarmMinus = tick.LTP - 0.002M;
                                    }

                                    // 起動後最高値
                                    if (tick.LTP >= PairEthBtc.HighestPrice)
                                    {
                                        if ((PairEthBtc.TickHistories.Count > 25) && ((PairEthBtc.BasePrice + 2000M) < tick.LTP))
                                        {
                                            PairEthBtc.HighLowInfoTextColorFlag = true;
                                            PairEthBtc.HighLowInfoText = "⇑⇑⇑　起動後最高値更新 " + PairEthBtc.CurrentPairString;

                                            if ((isPlayed == false) && (PairEthBtc.PlaySoundHighest == true))
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
                                    if (tick.LTP <= PairEthBtc.LowestPrice)
                                    {
                                        if ((PairEthBtc.TickHistories.Count > 25) && ((PairEthBtc.BasePrice - 2000M) > tick.LTP))
                                        {
                                            PairEthBtc.HighLowInfoTextColorFlag = false;
                                            PairEthBtc.HighLowInfoText = "⇓⇓⇓　起動後最安値更新 " + PairEthBtc.CurrentPairString;

                                            if ((isPlayed == false) && (PairEthBtc.PlaySoundLowest == true))
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
                                    if (tick.LTP >= PairEthBtc.HighestIn24Price)
                                    {
                                        PairEthBtc.HighLowInfoTextColorFlag = true;
                                        PairEthBtc.HighLowInfoText = "⇑⇑⇑⇑⇑⇑　過去24時間最高値更新 " + PairEthBtc.CurrentPairString;

                                        if ((isPlayed == false) && (PairEthBtc.PlaySoundHighest24h == true))
                                        {
                                            if (PlaySound)
                                            {
                                                SystemSounds.Hand.Play();
                                                isPlayed = true;
                                            }
                                        }
                                    }
                                    // 過去24時間最安値
                                    if (tick.LTP <= PairEthBtc.LowestIn24Price)
                                    {
                                        PairEthBtc.HighLowInfoTextColorFlag = false;
                                        PairEthBtc.HighLowInfoText = "⇓⇓⇓⇓⇓⇓　過去24時間最安値更新 " + PairEthBtc.CurrentPairString;

                                        if ((isPlayed == false) && (PairEthBtc.PlaySoundLowest24h == true))
                                        {
                                            if (PlaySound)
                                            {
                                                SystemSounds.Hand.Play();
                                                isPlayed = true;
                                            }
                                        }
                                    }

                                }

                                #endregion

                                // 省エネモードでなかったら
                                if ((MinMode == false) && (pair == CurrentPair))
                                {
                                    // 最新取引価格のラインを更新
                                    if (ChartAxisYEthBtc != null)
                                    {
                                        if (ChartAxisYEthBtc[0].Sections.Count > 0)
                                        {
                                            ChartAxisYEthBtc[0].Sections[0].Value = (double)tick.LTP;
                                        }
                                    }

                                    /*
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
                                    */


                                    // 最新のロウソク足を更新する。＞＞＞重すぎ。負荷掛かり過ぎなので止め。
                                    /*
                                    if (ChartSeriesEthBtc[0].Values != null)
                                    {
                                        int c = ChartSeriesEthBtc[0].Values.Count;

                                        if (c > 0)
                                        {
                                            double l = ((OhlcPoint)ChartSeriesEthBtc[0].Values[c - 1]).Low;
                                            double h = ((OhlcPoint)ChartSeriesEthBtc[0].Values[c - 1]).High;

                                            if (Application.Current == null) return;
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {

                                                ((OhlcPoint)ChartSeriesEthBtc[0].Values[c - 1]).Close = (double)tick.LTP;

                                                if (l > (double)tick.LTP)
                                                {
                                                    ((OhlcPoint)ChartSeriesEthBtc[0].Values[c - 1]).Low = (double)tick.LTP;
                                                }

                                                if (h < (double)tick.LTP)
                                                {
                                                    ((OhlcPoint)ChartSeriesEthBtc[0].Values[c - 1]).High = (double)tick.LTP;
                                                }

                                            });

                                        }
                                    }
                                    */
                                }

                            }
                            else if (pair == Pairs.mona_jpy)
                            {

                                // 最新の価格をセット
                                PairMonaJpy.Ltp = tick.LTP;
                                PairMonaJpy.Bid = tick.Bid;
                                PairMonaJpy.Ask = tick.Ask;
                                PairMonaJpy.TickTimeStamp = tick.TimeStamp;

                                PairMonaJpy.LowestIn24Price = tick.Low;
                                PairMonaJpy.HighestIn24Price = tick.High;

                                // 起動時価格セット
                                if (PairMonaJpy.BasePrice == 0) PairMonaJpy.BasePrice = tick.LTP;

                                /*
                                // ビットコイン時価評価額の計算
                                if (AssetBTCAmount != 0)
                                {
                                    AssetBTCEstimateAmount = _ltp * AssetBTCAmount;
                                }
                                */

                                // 最安値登録
                                if (PairMonaJpy.LowestPrice == 0)
                                {
                                    PairMonaJpy.LowestPrice = tick.LTP;
                                }
                                if (tick.LTP < PairMonaJpy.LowestPrice)
                                {
                                    //SystemSounds.Beep.Play();
                                    PairMonaJpy.LowestPrice = tick.LTP;
                                }

                                // 最高値登録
                                if (PairMonaJpy.HighestPrice == 0)
                                {
                                    PairMonaJpy.HighestPrice = tick.LTP;
                                }
                                if (tick.LTP > PairMonaJpy.HighestPrice)
                                {
                                    //SystemSounds.Asterisk.Play();
                                    PairMonaJpy.HighestPrice = tick.LTP;
                                }

                                #region == チック履歴 ==

                                TickHistory aym = new TickHistory();
                                aym.Price = tick.LTP;
                                aym.TimeAt = tick.TimeStamp;
                                if (PairMonaJpy.TickHistories.Count > 0)
                                {
                                    if (PairMonaJpy.TickHistories[0].Price > aym.Price)
                                    {
                                        //aym.TickHistoryPriceColor = _priceUpColor;
                                        aym.TickHistoryPriceUp = true;
                                        PairMonaJpy.TickHistories.Insert(0, aym);

                                    }
                                    else if (PairMonaJpy.TickHistories[0].Price < aym.Price)
                                    {
                                        //aym.TickHistoryPriceColor = _priceDownColor;
                                        aym.TickHistoryPriceUp = false;
                                        PairMonaJpy.TickHistories.Insert(0, aym);
                                    }
                                    else
                                    {
                                        //aym.TickHistoryPriceColor = Colors.Gainsboro;
                                        PairMonaJpy.TickHistories.Insert(0, aym);
                                    }
                                }
                                else
                                {
                                    //aym.TickHistoryPriceColor = Colors.Gainsboro;
                                    PairMonaJpy.TickHistories.Insert(0, aym);
                                }

                                // limit the number of the list.
                                if (PairMonaJpy.TickHistories.Count > 60)
                                {
                                    PairMonaJpy.TickHistories.RemoveAt(60);
                                }

                                // 60(1分)の平均値を求める
                                decimal aSum = 0;
                                int c = 0;
                                if (PairMonaJpy.TickHistories.Count > 0)
                                {

                                    if (PairMonaJpy.TickHistories.Count > 60)
                                    {
                                        c = 59;
                                    }
                                    else
                                    {
                                        c = PairMonaJpy.TickHistories.Count - 1;
                                    }

                                    if (c == 0)
                                    {
                                        PairMonaJpy.AveragePrice = PairMonaJpy.TickHistories[0].Price;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < c; i++)
                                        {
                                            aSum = aSum + PairMonaJpy.TickHistories[i].Price;
                                        }
                                        PairMonaJpy.AveragePrice = aSum / c;
                                    }

                                }
                                else if (PairMonaJpy.TickHistories.Count == 1)
                                {
                                    PairMonaJpy.AveragePrice = PairMonaJpy.TickHistories[0].Price;
                                }

                                #endregion

                                #region == アラーム ==

                                // TODO 非表示の通貨の場合はどうする？？？
                                if (pair == CurrentPair)
                                {

                                    bool isPlayed = false;

                                    // アラーム
                                    if (PairMonaJpy.AlarmPlus > 0)
                                    {
                                        if (tick.LTP >= PairMonaJpy.AlarmPlus)
                                        {
                                            PairMonaJpy.HighLowInfoTextColorFlag = true;
                                            PairMonaJpy.HighLowInfoText = "⇑⇑⇑　高値アラーム " + PairMonaJpy.CurrentPairString;

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
                                        PairMonaJpy.AlarmPlus = tick.LTP + 10M;
                                    }

                                    if (PairMonaJpy.AlarmMinus > 0)
                                    {
                                        if (tick.LTP <= PairMonaJpy.AlarmMinus)
                                        {
                                            PairMonaJpy.HighLowInfoTextColorFlag = false;
                                            PairMonaJpy.HighLowInfoText = "⇓⇓⇓　安値アラーム " + PairMonaJpy.CurrentPairString;
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
                                        PairMonaJpy.AlarmMinus = tick.LTP - 10M;
                                    }

                                    // 起動後最高値
                                    if (tick.LTP >= PairMonaJpy.HighestPrice)
                                    {
                                        if ((PairMonaJpy.TickHistories.Count > 25) && ((PairMonaJpy.BasePrice + 2M) < tick.LTP))
                                        {
                                            PairMonaJpy.HighLowInfoTextColorFlag = true;
                                            PairMonaJpy.HighLowInfoText = "⇑⇑⇑　起動後最高値更新 " + PairMonaJpy.CurrentPairString;

                                            if ((isPlayed == false) && (PairMonaJpy.PlaySoundHighest == true))
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
                                    if (tick.LTP <= PairMonaJpy.LowestPrice)
                                    {
                                        if ((PairMonaJpy.TickHistories.Count > 25) && ((PairMonaJpy.BasePrice - 2M) > tick.LTP))
                                        {
                                            PairMonaJpy.HighLowInfoTextColorFlag = false;
                                            PairMonaJpy.HighLowInfoText = "⇓⇓⇓　起動後最安値更新 " + PairMonaJpy.CurrentPairString;

                                            if ((isPlayed == false) && (PairMonaJpy.PlaySoundLowest == true))
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
                                    if (tick.LTP >= PairMonaJpy.HighestIn24Price)
                                    {
                                        PairMonaJpy.HighLowInfoTextColorFlag = true;
                                        PairMonaJpy.HighLowInfoText = "⇑⇑⇑⇑⇑⇑　過去24時間最高値更新 " + PairMonaJpy.CurrentPairString;

                                        if ((isPlayed == false) && (PairMonaJpy.PlaySoundHighest24h == true))
                                        {
                                            if (PlaySound)
                                            {
                                                SystemSounds.Hand.Play();
                                                isPlayed = true;
                                            }
                                        }
                                    }
                                    // 過去24時間最安値
                                    if (tick.LTP <= PairMonaJpy.LowestIn24Price)
                                    {
                                        PairMonaJpy.HighLowInfoTextColorFlag = false;
                                        PairMonaJpy.HighLowInfoText = "⇓⇓⇓⇓⇓⇓　過去24時間最安値更新 " + PairMonaJpy.CurrentPairString;

                                        if ((isPlayed == false) && (PairMonaJpy.PlaySoundLowest24h == true))
                                        {
                                            if (PlaySound)
                                            {
                                                SystemSounds.Hand.Play();
                                                isPlayed = true;
                                            }
                                        }
                                    }

                                }

                                #endregion

                                // 省エネモードでなかったら
                                if ((MinMode == false) && (pair == CurrentPair))
                                {
                                    // 最新取引価格のラインを更新
                                    if (ChartAxisYMonaJpy != null)
                                    {
                                        if (ChartAxisYMonaJpy[0].Sections.Count > 0)
                                        {
                                            ChartAxisYMonaJpy[0].Sections[0].Value = (double)tick.LTP;
                                        }
                                    }

                                    /*
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
                                    */


                                    // 最新のロウソク足を更新する。＞＞＞重すぎ。負荷掛かり過ぎなので止め。
                                    /*
                                    if (ChartSeriesMonaJpy[0].Values != null)
                                    {
                                        int c = ChartSeriesMonaJpy[0].Values.Count;

                                        if (c > 0)
                                        {
                                            double l = ((OhlcPoint)ChartSeriesMonaJpy[0].Values[c - 1]).Low;
                                            double h = ((OhlcPoint)ChartSeriesMonaJpy[0].Values[c - 1]).High;

                                            if (Application.Current == null) return;
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {

                                                ((OhlcPoint)ChartSeriesMonaJpy[0].Values[c - 1]).Close = (double)tick.LTP;

                                                if (l > (double)tick.LTP)
                                                {
                                                    ((OhlcPoint)ChartSeriesMonaJpy[0].Values[c - 1]).Low = (double)tick.LTP;
                                                }

                                                if (h < (double)tick.LTP)
                                                {
                                                    ((OhlcPoint)ChartSeriesMonaJpy[0].Values[c - 1]).High = (double)tick.LTP;
                                                }

                                            });

                                        }
                                    }
                                    */
                                }

                            }
                            else if (pair == Pairs.mona_btc)
                            {
                                //
                            }
                            else if (pair == Pairs.ltc_btc)
                            {

                                // 最新の価格をセット
                                PairLtcBtc.Ltp = tick.LTP;
                                PairLtcBtc.Bid = tick.Bid;
                                PairLtcBtc.Ask = tick.Ask;
                                PairLtcBtc.TickTimeStamp = tick.TimeStamp;

                                PairLtcBtc.LowestIn24Price = tick.Low;
                                PairLtcBtc.HighestIn24Price = tick.High;

                                // 起動時価格セット
                                if (PairLtcBtc.BasePrice == 0) PairLtcBtc.BasePrice = tick.LTP;

                                /*
                                // ビットコイン時価評価額の計算
                                if (AssetBTCAmount != 0)
                                {
                                    AssetBTCEstimateAmount = _ltp * AssetBTCAmount;
                                }
                                */

                                // 最安値登録
                                if (PairLtcBtc.LowestPrice == 0)
                                {
                                    PairLtcBtc.LowestPrice = tick.LTP;
                                }
                                if (tick.LTP < PairLtcBtc.LowestPrice)
                                {
                                    //SystemSounds.Beep.Play();
                                    PairLtcBtc.LowestPrice = tick.LTP;
                                }

                                // 最高値登録
                                if (PairLtcBtc.HighestPrice == 0)
                                {
                                    PairLtcBtc.HighestPrice = tick.LTP;
                                }
                                if (tick.LTP > PairLtcBtc.HighestPrice)
                                {
                                    //SystemSounds.Asterisk.Play();
                                    PairLtcBtc.HighestPrice = tick.LTP;
                                }

                                #region == チック履歴 ==

                                TickHistory aym = new TickHistory();
                                aym.Price = tick.LTP;
                                aym.TimeAt = tick.TimeStamp;
                                if (PairLtcBtc.TickHistories.Count > 0)
                                {
                                    if (PairLtcBtc.TickHistories[0].Price > aym.Price)
                                    {
                                        //aym.TickHistoryPriceColor = _priceUpColor;
                                        aym.TickHistoryPriceUp = true;
                                        PairLtcBtc.TickHistories.Insert(0, aym);

                                    }
                                    else if (PairLtcBtc.TickHistories[0].Price < aym.Price)
                                    {
                                        //aym.TickHistoryPriceColor = _priceDownColor;
                                        aym.TickHistoryPriceUp = false;
                                        PairLtcBtc.TickHistories.Insert(0, aym);
                                    }
                                    else
                                    {
                                        //aym.TickHistoryPriceColor = Colors.Gainsboro;
                                        PairLtcBtc.TickHistories.Insert(0, aym);
                                    }
                                }
                                else
                                {
                                    //aym.TickHistoryPriceColor = Colors.Gainsboro;
                                    PairLtcBtc.TickHistories.Insert(0, aym);
                                }

                                // limit the number of the list.
                                if (PairLtcBtc.TickHistories.Count > 60)
                                {
                                    PairLtcBtc.TickHistories.RemoveAt(60);
                                }

                                // 60(1分)の平均値を求める
                                decimal aSum = 0;
                                int c = 0;
                                if (PairLtcBtc.TickHistories.Count > 0)
                                {

                                    if (PairLtcBtc.TickHistories.Count > 60)
                                    {
                                        c = 59;
                                    }
                                    else
                                    {
                                        c = PairLtcBtc.TickHistories.Count - 1;
                                    }

                                    if (c == 0)
                                    {
                                        PairLtcBtc.AveragePrice = PairLtcBtc.TickHistories[0].Price;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < c; i++)
                                        {
                                            aSum = aSum + PairLtcBtc.TickHistories[i].Price;
                                        }
                                        PairLtcBtc.AveragePrice = aSum / c;
                                    }

                                }
                                else if (PairLtcBtc.TickHistories.Count == 1)
                                {
                                    PairLtcBtc.AveragePrice = PairLtcBtc.TickHistories[0].Price;
                                }

                                #endregion

                                #region == アラーム ==

                                // TODO 非表示の通貨の場合はどうする？？？
                                if (pair == CurrentPair)
                                {

                                    bool isPlayed = false;

                                    // アラーム
                                    if (PairLtcBtc.AlarmPlus > 0)
                                    {
                                        if (tick.LTP >= PairLtcBtc.AlarmPlus)
                                        {
                                            PairLtcBtc.HighLowInfoTextColorFlag = true;
                                            PairLtcBtc.HighLowInfoText = "⇑⇑⇑　高値アラーム " + PairLtcBtc.CurrentPairString;

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
                                        PairLtcBtc.AlarmPlus = tick.LTP + 0.0005M;
                                    }

                                    if (PairLtcBtc.AlarmMinus > 0)
                                    {
                                        if (tick.LTP <= PairLtcBtc.AlarmMinus)
                                        {
                                            PairLtcBtc.HighLowInfoTextColorFlag = false;
                                            PairLtcBtc.HighLowInfoText = "⇓⇓⇓　安値アラーム " + PairLtcBtc.CurrentPairString;
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
                                        PairLtcBtc.AlarmMinus = tick.LTP - 0.0005M;
                                    }

                                    // 起動後最高値
                                    if (tick.LTP >= PairLtcBtc.HighestPrice)
                                    {
                                        if ((PairLtcBtc.TickHistories.Count > 25) && ((PairLtcBtc.BasePrice + 0.0001M) < tick.LTP))
                                        {
                                            PairLtcBtc.HighLowInfoTextColorFlag = true;
                                            PairLtcBtc.HighLowInfoText = "⇑⇑⇑　起動後最高値更新 " + PairLtcBtc.CurrentPairString;

                                            if ((isPlayed == false) && (PairLtcBtc.PlaySoundHighest == true))
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
                                    if (tick.LTP <= PairLtcBtc.LowestPrice)
                                    {
                                        if ((PairLtcBtc.TickHistories.Count > 25) && ((PairLtcBtc.BasePrice - 0.0001M) > tick.LTP))
                                        {
                                            PairLtcBtc.HighLowInfoTextColorFlag = false;
                                            PairLtcBtc.HighLowInfoText = "⇓⇓⇓　起動後最安値更新 " + PairLtcBtc.CurrentPairString;

                                            if ((isPlayed == false) && (PairLtcBtc.PlaySoundLowest == true))
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
                                    if (tick.LTP >= PairLtcBtc.HighestIn24Price)
                                    {
                                        PairLtcBtc.HighLowInfoTextColorFlag = true;
                                        PairLtcBtc.HighLowInfoText = "⇑⇑⇑⇑⇑⇑　過去24時間最高値更新 " + PairLtcBtc.CurrentPairString;

                                        if ((isPlayed == false) && (PairLtcBtc.PlaySoundHighest24h == true))
                                        {
                                            if (PlaySound)
                                            {
                                                SystemSounds.Hand.Play();
                                                isPlayed = true;
                                            }
                                        }
                                    }
                                    // 過去24時間最安値
                                    if (tick.LTP <= PairLtcBtc.LowestIn24Price)
                                    {
                                        PairLtcBtc.HighLowInfoTextColorFlag = false;
                                        PairLtcBtc.HighLowInfoText = "⇓⇓⇓⇓⇓⇓　過去24時間最安値更新 " + PairLtcBtc.CurrentPairString;

                                        if ((isPlayed == false) && (PairLtcBtc.PlaySoundLowest24h == true))
                                        {
                                            if (PlaySound)
                                            {
                                                SystemSounds.Hand.Play();
                                                isPlayed = true;
                                            }
                                        }
                                    }

                                }

                                #endregion

                                // 省エネモードでなかったら
                                if ((MinMode == false) && (pair == CurrentPair))
                                {
                                    // 最新取引価格のラインを更新
                                    if (ChartAxisYLtcBtc != null)
                                    {
                                        if (ChartAxisYLtcBtc[0].Sections.Count > 0)
                                        {
                                            ChartAxisYLtcBtc[0].Sections[0].Value = (double)tick.LTP;
                                        }
                                    }

                                    /*
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
                                    */


                                    // 最新のロウソク足を更新する。＞＞＞重すぎ。負荷掛かり過ぎなので止め。
                                    /*
                                    if (ChartSeriesLtcBtc[0].Values != null)
                                    {
                                        int c = ChartSeriesLtcBtc[0].Values.Count;

                                        if (c > 0)
                                        {
                                            double l = ((OhlcPoint)ChartSeriesLtcBtc[0].Values[c - 1]).Low;
                                            double h = ((OhlcPoint)ChartSeriesLtcBtc[0].Values[c - 1]).High;

                                            if (Application.Current == null) return;
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {

                                                ((OhlcPoint)ChartSeriesLtcBtc[0].Values[c - 1]).Close = (double)tick.LTP;

                                                if (l > (double)tick.LTP)
                                                {
                                                    ((OhlcPoint)ChartSeriesLtcBtc[0].Values[c - 1]).Low = (double)tick.LTP;
                                                }

                                                if (h < (double)tick.LTP)
                                                {
                                                    ((OhlcPoint)ChartSeriesLtcBtc[0].Values[c - 1]).High = (double)tick.LTP;
                                                }

                                            });

                                        }
                                    }
                                    */
                                }

                            }
                            else if (pair == Pairs.bcc_btc)
                            {
                                //
                            }
                            else if (pair == Pairs.bcc_jpy)
                            {

                                // 最新の価格をセット
                                PairBchJpy.Ltp = tick.LTP;
                                PairBchJpy.Bid = tick.Bid;
                                PairBchJpy.Ask = tick.Ask;
                                PairBchJpy.TickTimeStamp = tick.TimeStamp;

                                PairBchJpy.LowestIn24Price = tick.Low;
                                PairBchJpy.HighestIn24Price = tick.High;

                                // 起動時価格セット
                                if (PairBchJpy.BasePrice == 0) PairBchJpy.BasePrice = tick.LTP;

                                /*
                                // ビットコイン時価評価額の計算
                                if (AssetBTCAmount != 0)
                                {
                                    AssetBTCEstimateAmount = _ltp * AssetBTCAmount;
                                }
                                */

                                // 最安値登録
                                if (PairBchJpy.LowestPrice == 0)
                                {
                                    PairBchJpy.LowestPrice = tick.LTP;
                                }
                                if (tick.LTP < PairBchJpy.LowestPrice)
                                {
                                    //SystemSounds.Beep.Play();
                                    PairBchJpy.LowestPrice = tick.LTP;
                                }

                                // 最高値登録
                                if (PairBchJpy.HighestPrice == 0)
                                {
                                    PairBchJpy.HighestPrice = tick.LTP;
                                }
                                if (tick.LTP > PairBchJpy.HighestPrice)
                                {
                                    //SystemSounds.Asterisk.Play();
                                    PairBchJpy.HighestPrice = tick.LTP;
                                }

                                #region == チック履歴 ==

                                TickHistory aym = new TickHistory();
                                aym.Price = tick.LTP;
                                aym.TimeAt = tick.TimeStamp;
                                if (PairBchJpy.TickHistories.Count > 0)
                                {
                                    if (PairBchJpy.TickHistories[0].Price > aym.Price)
                                    {
                                        //aym.TickHistoryPriceColor = _priceUpColor;
                                        aym.TickHistoryPriceUp = true;
                                        PairBchJpy.TickHistories.Insert(0, aym);

                                    }
                                    else if (PairBchJpy.TickHistories[0].Price < aym.Price)
                                    {
                                        //aym.TickHistoryPriceColor = _priceDownColor;
                                        aym.TickHistoryPriceUp = false;
                                        PairBchJpy.TickHistories.Insert(0, aym);
                                    }
                                    else
                                    {
                                        //aym.TickHistoryPriceColor = Colors.Gainsboro;
                                        PairBchJpy.TickHistories.Insert(0, aym);
                                    }
                                }
                                else
                                {
                                    //aym.TickHistoryPriceColor = Colors.Gainsboro;
                                    PairBchJpy.TickHistories.Insert(0, aym);
                                }

                                // limit the number of the list.
                                if (PairBchJpy.TickHistories.Count > 60)
                                {
                                    PairBchJpy.TickHistories.RemoveAt(60);
                                }

                                // 60(1分)の平均値を求める
                                decimal aSum = 0;
                                int c = 0;
                                if (PairBchJpy.TickHistories.Count > 0)
                                {

                                    if (PairBchJpy.TickHistories.Count > 60)
                                    {
                                        c = 59;
                                    }
                                    else
                                    {
                                        c = PairBchJpy.TickHistories.Count - 1;
                                    }

                                    if (c == 0)
                                    {
                                        PairBchJpy.AveragePrice = PairBchJpy.TickHistories[0].Price;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < c; i++)
                                        {
                                            aSum = aSum + PairBchJpy.TickHistories[i].Price;
                                        }
                                        PairBchJpy.AveragePrice = aSum / c;
                                    }

                                }
                                else if (PairLtcBtc.TickHistories.Count == 1)
                                {
                                    PairBchJpy.AveragePrice = PairBchJpy.TickHistories[0].Price;
                                }

                                #endregion

                                #region == アラーム ==

                                // TODO 非表示の通貨の場合はどうする？？？
                                if (pair == CurrentPair)
                                {

                                    bool isPlayed = false;

                                    // アラーム
                                    if (PairBchJpy.AlarmPlus > 0)
                                    {
                                        if (tick.LTP >= PairBchJpy.AlarmPlus)
                                        {
                                            PairBchJpy.HighLowInfoTextColorFlag = true;
                                            PairBchJpy.HighLowInfoText = "⇑⇑⇑　高値アラーム " + PairBchJpy.CurrentPairString;

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
                                        PairBchJpy.AlarmPlus = ((tick.LTP / 100M) * 100M) + 1000M;
                                    }

                                    if (PairBchJpy.AlarmMinus > 0)
                                    {
                                        if (tick.LTP <= PairBchJpy.AlarmMinus)
                                        {
                                            PairBchJpy.HighLowInfoTextColorFlag = false;
                                            PairBchJpy.HighLowInfoText = "⇓⇓⇓　安値アラーム " + PairBchJpy.CurrentPairString;
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
                                        PairBchJpy.AlarmMinus = ((tick.LTP / 100M) * 100M) - 1000M;
                                    }

                                    // 起動後最高値
                                    if (tick.LTP >= PairBchJpy.HighestPrice)
                                    {
                                        if ((PairBchJpy.TickHistories.Count > 25) && ((PairBchJpy.BasePrice + 200M) < tick.LTP))
                                        {
                                            PairBchJpy.HighLowInfoTextColorFlag = true;
                                            PairBchJpy.HighLowInfoText = "⇑⇑⇑　起動後最高値更新 " + PairBchJpy.CurrentPairString;

                                            if ((isPlayed == false) && (PairBchJpy.PlaySoundHighest == true))
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
                                    if (tick.LTP <= PairBchJpy.LowestPrice)
                                    {
                                        if ((PairBchJpy.TickHistories.Count > 25) && ((PairBchJpy.BasePrice - 200M) > tick.LTP))
                                        {
                                            PairBchJpy.HighLowInfoTextColorFlag = false;
                                            PairBchJpy.HighLowInfoText = "⇓⇓⇓　起動後最安値更新 " + PairBchJpy.CurrentPairString;

                                            if ((isPlayed == false) && (PairBchJpy.PlaySoundLowest == true))
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
                                    if (tick.LTP >= PairBchJpy.HighestIn24Price)
                                    {
                                        PairBchJpy.HighLowInfoTextColorFlag = true;
                                        PairBchJpy.HighLowInfoText = "⇑⇑⇑⇑⇑⇑　過去24時間最高値更新 " + PairBchJpy.CurrentPairString;

                                        if ((isPlayed == false) && (PairBchJpy.PlaySoundHighest24h == true))
                                        {
                                            if (PlaySound)
                                            {
                                                SystemSounds.Hand.Play();
                                                isPlayed = true;
                                            }
                                        }
                                    }
                                    // 過去24時間最安値
                                    if (tick.LTP <= PairBchJpy.LowestIn24Price)
                                    {
                                        PairBchJpy.HighLowInfoTextColorFlag = false;
                                        PairBchJpy.HighLowInfoText = "⇓⇓⇓⇓⇓⇓　過去24時間最安値更新 " + PairBchJpy.CurrentPairString;

                                        if ((isPlayed == false) && (PairBchJpy.PlaySoundLowest24h == true))
                                        {
                                            if (PlaySound)
                                            {
                                                SystemSounds.Hand.Play();
                                                isPlayed = true;
                                            }
                                        }
                                    }

                                }

                                #endregion

                                // 省エネモードでなかったら
                                if ((MinMode == false) && (pair == CurrentPair))
                                {
                                    // 最新取引価格のラインを更新
                                    if (ChartAxisYBchJpy != null)
                                    {
                                        if (ChartAxisYBchJpy[0].Sections.Count > 0)
                                        {
                                            ChartAxisYBchJpy[0].Sections[0].Value = (double)tick.LTP;
                                        }
                                    }

                                    /*
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
                                    */


                                    // 最新のロウソク足を更新する。＞＞＞重すぎ。負荷掛かり過ぎなので止め。
                                    /*
                                    if (ChartSeriesBchJpy[0].Values != null)
                                    {
                                        int c = ChartSeriesBchJpy[0].Values.Count;

                                        if (c > 0)
                                        {
                                            double l = ((OhlcPoint)ChartSeriesBchJpy[0].Values[c - 1]).Low;
                                            double h = ((OhlcPoint)ChartSeriesBchJpy[0].Values[c - 1]).High;

                                            if (Application.Current == null) return;
                                            Application.Current.Dispatcher.Invoke(() =>
                                            {

                                                ((OhlcPoint)ChartSeriesBchJpy[0].Values[c - 1]).Close = (double)tick.LTP;

                                                if (l > (double)tick.LTP)
                                                {
                                                    ((OhlcPoint)ChartSeriesBchJpy[0].Values[c - 1]).Low = (double)tick.LTP;
                                                }

                                                if (h < (double)tick.LTP)
                                                {
                                                    ((OhlcPoint)ChartSeriesBchJpy[0].Values[c - 1]).High = (double)tick.LTP;
                                                }

                                            });

                                        }
                                    }
                                    */
                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("■■■■■ TickerTimerAllPairs: Exception1 - " + ex.Message);
                            break;
                        }
                    }
                    else
                    {
                        APIResultTicker = "<<取得失敗>>";
                        System.Diagnostics.Debug.WriteLine("■■■■■ TickerTimerAllPairs: GetTicker returned null");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ TickerTimerAllPairs Exception2: " + ex);
            }
        }

        // チャート表示 タイマー
        private void ChartTimer(object source, EventArgs e)
        {
            Debug.WriteLine("ChartTimer");

            if (_showAllCharts)
            {
                try
                {
                    // 各通貨ペアをループ
                    foreach (Pairs pair in Enum.GetValues(typeof(Pairs)))
                    {
                        if ((pair == Pairs.mona_btc) || pair == Pairs.bcc_btc)
                        {
                            continue;
                        }

                        UpdateCandlestick(pair, SelectedCandleType);

                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ ChartTimer _showAllCharts Exception: " + ex);
                }
            }
            else
            {

                try
                {
                    
                    UpdateCandlestick(CurrentPair, SelectedCandleType);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ ChartTimer Exception: " + ex);
                }

            }

        }

        // RSS 取得タイマー
        private void RssTimer(object source, EventArgs e)
        {

            // 省エネモードならスルー。
            if (MinMode == false)
            {
                Task.Run(() => GetRss());
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

                        hoge = mainWindow.Attribute("opacity");
                        if (hoge != null)
                        {
                            WindowOpacity = double.Parse(hoge.Value);
                        }

                        hoge = mainWindow.Attribute("theme");
                        if (hoge != null)
                        {
                            // テーマをセット
                            SetCurrentTheme(hoge.Value);
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

                #region == アラーム音設定 ==

                var alarmSetting = xdoc.Root.Element("Alarm");
                if (alarmSetting != null)
                {
                    var hoge = alarmSetting.Attribute("playSound");
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

                #region == 各通貨毎の設定 ==

                var pairs = xdoc.Root.Element("Pairs");
                if (pairs != null)
                {
                    // PairBtcJpy
                    var pair = pairs.Element("BtcJpy");
                    if (pair != null)
                    {
                        var hoge = pair.Attribute("playSoundLowest");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairBtcJpy.PlaySoundLowest = true;
                            }
                            else
                            {
                                PairBtcJpy.PlaySoundLowest = false;
                            }
                        }

                        hoge = pair.Attribute("playSoundHighest");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairBtcJpy.PlaySoundHighest = true;
                            }
                            else
                            {
                                PairBtcJpy.PlaySoundHighest = false;
                            }
                        }

                        hoge = pair.Attribute("playSoundLowest24h");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairBtcJpy.PlaySoundLowest24h = true;
                            }
                            else
                            {
                                PairBtcJpy.PlaySoundLowest24h = false;
                            }
                        }

                        hoge = pair.Attribute("playSoundHighest24h");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairBtcJpy.PlaySoundHighest24h = true;
                            }
                            else
                            {
                                PairBtcJpy.PlaySoundHighest24h = false;
                            }
                        }

                        // 板グルーピング
                        hoge = pair.Attribute("depthGrouping");
                        if (hoge != null)
                        {
                            if (!string.IsNullOrEmpty(hoge.Value))
                            {
                                try
                                {
                                    PairBtcJpy.DepthGrouping = Decimal.Parse(hoge.Value);
                                }
                                catch
                                {
                                    PairBtcJpy.DepthGrouping = 0;
                                }
                                
                            }
                        }


                    }


                    // PairXrpJpy
                    pair = pairs.Element("XrpJpy");
                    if (pair != null)
                    {
                        var hoge = pair.Attribute("playSoundLowest");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairXrpJpy.PlaySoundLowest = true;
                            }
                            else
                            {
                                PairXrpJpy.PlaySoundLowest = false;
                            }
                        }

                        hoge = pair.Attribute("playSoundHighest");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairXrpJpy.PlaySoundHighest = true;
                            }
                            else
                            {
                                PairXrpJpy.PlaySoundHighest = false;
                            }
                        }

                        hoge = pair.Attribute("playSoundLowest24h");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairXrpJpy.PlaySoundLowest24h = true;
                            }
                            else
                            {
                                PairXrpJpy.PlaySoundLowest24h = false;
                            }
                        }

                        hoge = pair.Attribute("playSoundHighest24h");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairXrpJpy.PlaySoundHighest24h = true;
                            }
                            else
                            {
                                PairXrpJpy.PlaySoundHighest24h = false;
                            }
                        }

                        // 板グルーピング
                        hoge = pair.Attribute("depthGrouping");
                        if (hoge != null)
                        {
                            if (!string.IsNullOrEmpty(hoge.Value))
                            {
                                try
                                {
                                    PairXrpJpy.DepthGrouping = Decimal.Parse(hoge.Value);
                                }
                                catch
                                {
                                    PairXrpJpy.DepthGrouping = 0;
                                }

                            }
                        }

                    }

                    // PairEthBtc
                    pair = pairs.Element("EthBtc");
                    if (pair != null)
                    {
                        var hoge = pair.Attribute("playSoundLowest");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairEthBtc.PlaySoundLowest = true;
                            }
                            else
                            {
                                PairEthBtc.PlaySoundLowest = false;
                            }
                        }

                        hoge = pair.Attribute("playSoundHighest");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairEthBtc.PlaySoundHighest = true;
                            }
                            else
                            {
                                PairEthBtc.PlaySoundHighest = false;
                            }
                        }

                        hoge = pair.Attribute("playSoundLowest24h");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairEthBtc.PlaySoundLowest24h = true;
                            }
                            else
                            {
                                PairEthBtc.PlaySoundLowest24h = false;
                            }
                        }

                        hoge = pair.Attribute("playSoundHighest24h");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairEthBtc.PlaySoundHighest24h = true;
                            }
                            else
                            {
                                PairEthBtc.PlaySoundHighest24h = false;
                            }
                        }

                        // 板グルーピング
                        hoge = pair.Attribute("depthGrouping");
                        if (hoge != null)
                        {
                            if (!string.IsNullOrEmpty(hoge.Value))
                            {
                                try
                                {
                                    PairEthBtc.DepthGrouping = Decimal.Parse(hoge.Value);
                                }
                                catch
                                {
                                    PairEthBtc.DepthGrouping = 0;
                                }

                            }
                        }

                    }

                    // PairLtcBtc
                    pair = pairs.Element("LtcBtc");
                    if (pair != null)
                    {
                        var hoge = pair.Attribute("playSoundLowest");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairLtcBtc.PlaySoundLowest = true;
                            }
                            else
                            {
                                PairLtcBtc.PlaySoundLowest = false;
                            }
                        }

                        hoge = pair.Attribute("playSoundHighest");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairLtcBtc.PlaySoundHighest = true;
                            }
                            else
                            {
                                PairLtcBtc.PlaySoundHighest = false;
                            }
                        }

                        hoge = pair.Attribute("playSoundLowest24h");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairLtcBtc.PlaySoundLowest24h = true;
                            }
                            else
                            {
                                PairLtcBtc.PlaySoundLowest24h = false;
                            }
                        }

                        hoge = pair.Attribute("playSoundHighest24h");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairLtcBtc.PlaySoundHighest24h = true;
                            }
                            else
                            {
                                PairLtcBtc.PlaySoundHighest24h = false;
                            }
                        }

                        // 板グルーピング
                        hoge = pair.Attribute("depthGrouping");
                        if (hoge != null)
                        {
                            if (!string.IsNullOrEmpty(hoge.Value))
                            {
                                try
                                {
                                    PairLtcBtc.DepthGrouping = Decimal.Parse(hoge.Value);
                                }
                                catch
                                {
                                    PairLtcBtc.DepthGrouping = 0;
                                }

                            }
                        }
                    }

                    // PairMonaJpy
                    pair = pairs.Element("MonaJpy");
                    if (pair != null)
                    {
                        var hoge = pair.Attribute("playSoundLowest");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairMonaJpy.PlaySoundLowest = true;
                            }
                            else
                            {
                                PairMonaJpy.PlaySoundLowest = false;
                            }
                        }

                        hoge = pair.Attribute("playSoundHighest");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairMonaJpy.PlaySoundHighest = true;
                            }
                            else
                            {
                                PairMonaJpy.PlaySoundHighest = false;
                            }
                        }

                        hoge = pair.Attribute("playSoundLowest24h");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairMonaJpy.PlaySoundLowest24h = true;
                            }
                            else
                            {
                                PairMonaJpy.PlaySoundLowest24h = false;
                            }
                        }

                        hoge = pair.Attribute("playSoundHighest24h");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairMonaJpy.PlaySoundHighest24h = true;
                            }
                            else
                            {
                                PairMonaJpy.PlaySoundHighest24h = false;
                            }
                        }

                        // 板グルーピング
                        hoge = pair.Attribute("depthGrouping");
                        if (hoge != null)
                        {
                            if (!string.IsNullOrEmpty(hoge.Value))
                            {
                                try
                                {
                                    PairMonaJpy.DepthGrouping = Decimal.Parse(hoge.Value);
                                }
                                catch
                                {
                                    PairMonaJpy.DepthGrouping = 0;
                                }

                            }
                        }
                    }

                    // PairBchJpy
                    pair = pairs.Element("BchJpy");
                    if (pair != null)
                    {
                        var hoge = pair.Attribute("playSoundLowest");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairBchJpy.PlaySoundLowest = true;
                            }
                            else
                            {
                                PairBchJpy.PlaySoundLowest = false;
                            }
                        }

                        hoge = pair.Attribute("playSoundHighest");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairBchJpy.PlaySoundHighest = true;
                            }
                            else
                            {
                                PairBchJpy.PlaySoundHighest = false;
                            }
                        }

                        hoge = pair.Attribute("playSoundLowest24h");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairBchJpy.PlaySoundLowest24h = true;
                            }
                            else
                            {
                                PairBchJpy.PlaySoundLowest24h = false;
                            }
                        }

                        hoge = pair.Attribute("playSoundHighest24h");
                        if (hoge != null)
                        {
                            if (hoge.Value == "true")
                            {
                                PairBchJpy.PlaySoundHighest24h = true;
                            }
                            else
                            {
                                PairBchJpy.PlaySoundHighest24h = false;
                            }
                        }

                        // 板グルーピング
                        hoge = pair.Attribute("depthGrouping");
                        if (hoge != null)
                        {
                            if (!string.IsNullOrEmpty(hoge.Value))
                            {
                                try
                                {
                                    PairBchJpy.DepthGrouping = Decimal.Parse(hoge.Value);
                                }
                                catch
                                {
                                    PairBchJpy.DepthGrouping = 0;
                                }

                            }
                        }
                    }


                }


                #endregion

                #region == チャート関連 ==

                var chartSetting = xdoc.Root.Element("Chart");
                if (chartSetting != null)
                {
                    var hoge = chartSetting.Attribute("candleType");
                    if (hoge != null)
                    {
                        if (hoge.Value == CandleTypes.OneMin.ToString())
                        {
                            SelectedCandleType = CandleTypes.OneMin;
                        }
                        else if (hoge.Value == CandleTypes.OneHour.ToString())
                        {
                            SelectedCandleType = CandleTypes.OneHour;
                        }
                        else if (hoge.Value == CandleTypes.OneDay.ToString())
                        {
                            SelectedCandleType = CandleTypes.OneDay;
                        }
                        else
                        {
                            // TODO other candle types

                            SelectedCandleType = CandleTypes.OneHour;
                        }
                    }
                    else
                    {
                        // TODO other candle types

                        SelectedCandleType = CandleTypes.OneHour;
                    }

                }
                else
                {
                    // デフォのチャート、キャンドルタイプ指定
                    SelectedCandleType = CandleTypes.OneHour;
                }

                #endregion

            }
            else
            {
                // デフォのチャート、キャンドルタイプ指定
                SelectedCandleType = CandleTypes.OneHour;
            }

            #endregion

            //SelectedCandleType = で表示できるので、これは不要だが、デフォと同じ場合のみ、手動で表示させる。
            if (SelectedCandleType == CandleTypes.OneHour) // デフォと揃えること。
            {
                Debug.WriteLine("OnWindowLoaded DisplayChart");

                Task.Run(async () =>
                {
                    if (ShowAllCharts)
                    {
                        await Task.Run(() => DisplayCharts());
                    }
                    else
                    {
                        await Task.Run(() => DisplayChart(CurrentPair));
                    }

                });
            }

            // チャート更新のタイマー起動
            dispatcherChartTimer.Start();

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

                attrs = doc.CreateAttribute("opacity");
                attrs.Value = WindowOpacity.ToString();
                mainWindow.SetAttributeNode(attrs);

                attrs = doc.CreateAttribute("theme");
                attrs.Value = CurrentTheme.Name.ToString();
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

            #region == 各通貨毎の設定 ==

            XmlElement pairs = doc.CreateElement(string.Empty, "Pairs", string.Empty);

            // BtcJpy の設定
            XmlElement pairBtcJpy = doc.CreateElement(string.Empty, "BtcJpy", string.Empty);

            attrs = doc.CreateAttribute("playSoundLowest");
            if (PairBtcJpy.PlaySoundLowest)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairBtcJpy.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundHighest");
            if (PairBtcJpy.PlaySoundHighest)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairBtcJpy.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundLowest24h");
            if (PairBtcJpy.PlaySoundLowest24h)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairBtcJpy.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundHighest24h");
            if (PairBtcJpy.PlaySoundHighest24h)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairBtcJpy.SetAttributeNode(attrs);

            // 板グルーピング
            attrs = doc.CreateAttribute("depthGrouping");
            attrs.Value = PairBtcJpy.DepthGrouping.ToString();
            pairBtcJpy.SetAttributeNode(attrs);

            //
            pairs.AppendChild(pairBtcJpy);

            // PairXrpJpy の設定
            XmlElement pairXrpJpy = doc.CreateElement(string.Empty, "XrpJpy", string.Empty);

            attrs = doc.CreateAttribute("playSoundLowest");
            if (PairXrpJpy.PlaySoundLowest)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairXrpJpy.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundHighest");
            if (PairXrpJpy.PlaySoundHighest)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairXrpJpy.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundLowest24h");
            if (PairXrpJpy.PlaySoundLowest24h)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairXrpJpy.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundHighest24h");
            if (PairXrpJpy.PlaySoundHighest24h)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairXrpJpy.SetAttributeNode(attrs);

            // 板グルーピング
            attrs = doc.CreateAttribute("depthGrouping");
            attrs.Value = PairXrpJpy.DepthGrouping.ToString();
            pairXrpJpy.SetAttributeNode(attrs);

            //
            pairs.AppendChild(pairXrpJpy);

            // PairEthBtc の設定
            XmlElement pairEthBtc = doc.CreateElement(string.Empty, "EthBtc", string.Empty);

            attrs = doc.CreateAttribute("playSoundLowest");
            if (PairEthBtc.PlaySoundLowest)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairEthBtc.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundHighest");
            if (PairEthBtc.PlaySoundHighest)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairEthBtc.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundLowest24h");
            if (PairEthBtc.PlaySoundLowest24h)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairEthBtc.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundHighest24h");
            if (PairEthBtc.PlaySoundHighest24h)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairEthBtc.SetAttributeNode(attrs);

            // 板グルーピング
            attrs = doc.CreateAttribute("depthGrouping");
            attrs.Value = PairEthBtc.DepthGrouping.ToString();
            pairEthBtc.SetAttributeNode(attrs);

            //
            pairs.AppendChild(pairEthBtc);

            // PairLtcBtc の設定
            XmlElement pairLtcBtc = doc.CreateElement(string.Empty, "LtcBtc", string.Empty);

            attrs = doc.CreateAttribute("playSoundLowest");
            if (PairLtcBtc.PlaySoundLowest)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairLtcBtc.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundHighest");
            if (PairLtcBtc.PlaySoundHighest)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairLtcBtc.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundLowest24h");
            if (PairLtcBtc.PlaySoundLowest24h)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairLtcBtc.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundHighest24h");
            if (PairLtcBtc.PlaySoundHighest24h)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairLtcBtc.SetAttributeNode(attrs);

            // 板グルーピング
            attrs = doc.CreateAttribute("depthGrouping");
            attrs.Value = PairLtcBtc.DepthGrouping.ToString();
            pairLtcBtc.SetAttributeNode(attrs);

            //
            pairs.AppendChild(pairLtcBtc);

            // PairMonaJpy の設定
            XmlElement pairMonaJpy = doc.CreateElement(string.Empty, "MonaJpy", string.Empty);

            attrs = doc.CreateAttribute("playSoundLowest");
            if (PairMonaJpy.PlaySoundLowest)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairMonaJpy.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundHighest");
            if (PairMonaJpy.PlaySoundHighest)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairMonaJpy.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundLowest24h");
            if (PairMonaJpy.PlaySoundLowest24h)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairMonaJpy.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundHighest24h");
            if (PairMonaJpy.PlaySoundHighest24h)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairMonaJpy.SetAttributeNode(attrs);

            // 板グルーピング
            attrs = doc.CreateAttribute("depthGrouping");
            attrs.Value = PairMonaJpy.DepthGrouping.ToString();
            pairMonaJpy.SetAttributeNode(attrs);

            //
            pairs.AppendChild(pairMonaJpy);

            // PairBchJpy の設定
            XmlElement pairBchJpy = doc.CreateElement(string.Empty, "BchJpy", string.Empty);

            attrs = doc.CreateAttribute("playSoundLowest");
            if (PairBchJpy.PlaySoundLowest)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairBchJpy.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundHighest");
            if (PairBchJpy.PlaySoundHighest)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairBchJpy.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundLowest24h");
            if (PairBchJpy.PlaySoundLowest24h)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairBchJpy.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("playSoundHighest24h");
            if (PairBchJpy.PlaySoundHighest24h)
            {
                attrs.Value = "true";
            }
            else
            {
                attrs.Value = "false";
            }
            pairBchJpy.SetAttributeNode(attrs);

            // 板グルーピング
            attrs = doc.CreateAttribute("depthGrouping");
            attrs.Value = PairBchJpy.DepthGrouping.ToString();
            pairBchJpy.SetAttributeNode(attrs);

            //
            pairs.AppendChild(pairBchJpy);



            // ////
            root.AppendChild(pairs);


            #endregion

            #region == アラーム音設定 ==

            XmlElement alarmSetting = doc.CreateElement(string.Empty, "Alarm", string.Empty);

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

            #region == チャート関連 ==

            XmlElement chartSetting = doc.CreateElement(string.Empty, "Chart", string.Empty);

            attrs = doc.CreateAttribute("candleType");

            if (SelectedCandleType == CandleTypes.OneMin)
            {
                attrs.Value = CandleTypes.OneMin.ToString();
            }
            else if (SelectedCandleType == CandleTypes.OneHour)
            {
                attrs.Value = CandleTypes.OneHour.ToString();
            }
            else if (SelectedCandleType == CandleTypes.OneDay)
            {
                attrs.Value = CandleTypes.OneDay.ToString();
            }
            else
            {
                // TODO
                attrs.Value = "";
            }

            chartSetting.SetAttributeNode(attrs);

            root.AppendChild(chartSetting);

            #endregion

            // 設定ファイルの保存
            doc.Save(AppConfigFilePath);

            #endregion

        }

        #endregion

        #region == メソッド ==

        // ループ再生開始メソッド
        private void StartLoop()
        {

            // 資産情報の更新ループ
            //Task.Run(() => UpdateAssets());

            // 特殊注文リストの更新ループ
            //Task.Run(() => UpdateIfdocos());

            // 板情報の更新ループ
            Task.Run(() => UpdateDepth());

            // 歩み値の更新ループ
            Task.Run(() => UpdateTransactions());

            // 取引履歴のGet
            //Task.Run(() => UpdateTradeHistory());

            // 注文リストの更新ループ
            //Task.Run(() => UpdateOrderList());

        }

        // 板情報 取得
        private async Task<bool> GetDepth(Pairs pair)
        {

            // まとめグルーピング単位 
            decimal unit = ActivePair.DepthGrouping;

            // リスト数 （基本 上売り200、下買い200）
            int half = 200;
            int listCount = (half * 2) + 1;

            // 初期化
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
            else
            {
                //グルーピング単位が変わったので、一旦クリアする。
                if (DepthGroupingChanged)
                {
                    for (int i = 0; i < listCount; i++)
                    {
                        Depth dd = _depth[i];//new Depth();
                        dd.DepthPrice = 0;
                        dd.DepthBid = 0;
                        dd.DepthAsk = 0;
                        //_depth.Add(dd);
                    }

                    DepthGroupingChanged = false;
                }
            }

            try
            {
                DepthResult dpr = await _pubDepthApi.GetDepth(pair.ToString());

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

                                    _depth[half - i].DepthAsk = dp.DepthAsk;
                                    _depth[half - i].DepthBid = dp.DepthBid;
                                    _depth[half - i].DepthPrice = dp.DepthPrice;

                                    // 今回のAskは先送り
                                    t = d;
                                    // 今回のPriceが基準になる
                                    c2 = System.Math.Ceiling(e / unit);

                                    i++;

                                }

                            }
                            else
                            {
                                _depth[half - i] = dp;
                                i++;
                            }

                        }

                        _depth[i].IsAskBest = true;

                        i = half;

                        Depth dd = new Depth();
                        dd.DepthPrice = ActivePair.Ltp;
                        dd.DepthBid = 0;
                        dd.DepthAsk = 0;
                        dd.IsLTP = true;
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

                                if (c == 0) c = System.Math.Ceiling(dp.DepthPrice / unit);

                                // 100円単位でまとめる
                                if (System.Math.Ceiling(dp.DepthPrice / unit) == c)
                                {
                                    t = t + dp.DepthBid;
                                }
                                else
                                {

                                    // 一時保存
                                    e = dp.DepthPrice;
                                    dp.DepthPrice = (c2 * unit);

                                    // 一時保存
                                    d = dp.DepthBid;
                                    dp.DepthBid = t;

                                    // 追加
                                    _depth[i].DepthAsk = dp.DepthAsk;
                                    _depth[i].DepthBid = dp.DepthBid;
                                    _depth[i].DepthPrice = dp.DepthPrice;

                                    // 今回のBidは先送り
                                    t = d;
                                    // 今回のPriceが基準になる
                                    c = System.Math.Ceiling(e / unit);

                                    i++;

                                }
                            }
                            else
                            {
                                _depth[i] = dp;
                                i++;
                            }

                        }

                        _depth[half + 1].IsBidBest = true;

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
                    await GetDepth(CurrentPair);
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
        private async Task<bool> GetTransactions(Pairs pair)
        {
            try
            {
                TransactionsResult trs = await _pubTransactionsApi.GetTransactions(pair.ToString());

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
                    await GetTransactions(CurrentPair);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ UpdateTransactions Exception: " + e);
                }

                // 間隔 1/2
                await Task.Delay(1300);
            }

        }

        // RSSの取得
        private async void GetRss()
        {
            await Task.Delay(2000);

            try
            {
                RssResult rs = await _rssCli.GetRSS(Langs.en);

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
                RssResult rs = await _rssCli.GetRSS(Langs.ja);

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

        #region == 認証関連のメソッド ==

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

        // テーマをセットするメソッド
        private void SetCurrentTheme(string themeName)
        {
            Theme test = _themes.FirstOrDefault(x => x.Name == themeName);
            if (test != null)
            {
                CurrentTheme = test;
            }
        }

        #region == チャート関係のメソッド ==

        // ロウソク足 Candlestick取得メソッド
        private async Task<List<Ohlcv>> GetCandlestick(Pairs pair, CandleTypes ct, DateTime dtTarget)
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
                throw new System.InvalidOperationException("サポートしてないロウソク足単位");
                //return null;
            }
            // 1min 5min 15min 30min 1hour 4hour 8hour 12hour 1day 1week

            CandlestickResult csr = await _pubCandlestickApi.GetCandlestick(pair.ToString(), ctString, dtString);

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
        private async Task<bool> GetCandlesticks(Pairs pair, CandleTypes ct)
        {
            ChartLoadingInfo = "チャートデータを取得中....";
            
            // 今日の日付セット。UTCで。
            DateTime dtToday = DateTime.Now.ToUniversalTime();

            try
            {
                // データは、ローカルタイムで、朝9:00 から翌8:59分まで。8:59分までしか取れないので、 9:00過ぎていたら 最新のデータとるには日付を１日追加する

                #region == OhlcvsOneHour 1hour毎のデータ ==

                List<Ohlcv> ListOhlcvsOneHour = new List<Ohlcv>();

                if (ct == CandleTypes.OneHour)
                {
                    // TODO 取得中フラグセット。

                    Debug.WriteLine("今日の1hour取得開始 " + pair.ToString());

                    // 一時間のロウソク足タイプなら今日、昨日、一昨日、その前の１週間分の1hourデータを取得する必要あり。
                    ListOhlcvsOneHour = await GetCandlestick(pair, CandleTypes.OneHour, dtToday);
                    if (ListOhlcvsOneHour != null)
                    {
                        // 逆順にする
                        ListOhlcvsOneHour.Reverse();

                        Debug.WriteLine("昨日の1hour取得開始 " + pair.ToString());
                        await Task.Delay(200);
                        // 昨日
                        DateTime dtTarget = dtToday.AddDays(-1);

                        List<Ohlcv> res = await GetCandlestick(pair, CandleTypes.OneHour, dtTarget);
                        if (res != null)
                        {
                            // 逆順にする
                            res.Reverse();

                            foreach (var r in res)
                            {
                                ListOhlcvsOneHour.Add(r);
                            }

                            Debug.WriteLine("一昨日の1hour取得開始 " + pair.ToString());
                            await Task.Delay(200);
                            // 一昨日
                            dtTarget = dtTarget.AddDays(-1);
                            List<Ohlcv> last2 = await GetCandlestick(pair, CandleTypes.OneHour, dtTarget);
                            if (last2 != null)
                            {
                                // 逆順にする
                                last2.Reverse();

                                foreach (var l in last2)
                                {
                                    ListOhlcvsOneHour.Add(l);
                                }

                                Debug.WriteLine("３日前の1hour取得開始 " + pair.ToString());
                                await Task.Delay(200);
                                // ３日前
                                dtTarget = dtTarget.AddDays(-1);
                                List<Ohlcv> last3 = await GetCandlestick(pair, CandleTypes.OneHour, dtTarget);
                                if (last3 != null)
                                {
                                    // 逆順にする
                                    last3.Reverse();

                                    foreach (var l in last3)
                                    {
                                        ListOhlcvsOneHour.Add(l);
                                    }


                                    Debug.WriteLine("４日前の1hour取得開始 " + pair.ToString());
                                    await Task.Delay(300);
                                    // 4日前
                                    dtTarget = dtTarget.AddDays(-1);
                                    List<Ohlcv> last4 = await GetCandlestick(pair, CandleTypes.OneHour, dtTarget);
                                    if (last4 != null)
                                    {
                                        // 逆順にする
                                        last4.Reverse();

                                        foreach (var l in last4)
                                        {
                                            ListOhlcvsOneHour.Add(l);
                                        }

                                        await Task.Delay(300);
                                        // 5日前
                                        dtTarget = dtTarget.AddDays(-1);
                                        List<Ohlcv> last5 = await GetCandlestick(pair, CandleTypes.OneHour, dtTarget);
                                        if (last5 != null)
                                        {
                                            // 逆順にする
                                            last5.Reverse();

                                            foreach (var l in last5)
                                            {
                                                ListOhlcvsOneHour.Add(l);
                                            }

                                            await Task.Delay(300);
                                            // 6日前
                                            dtTarget = dtTarget.AddDays(-1);
                                            List<Ohlcv> last6 = await GetCandlestick(pair, CandleTypes.OneHour, dtTarget);
                                            if (last6 != null)
                                            {
                                                // 逆順にする
                                                last6.Reverse();

                                                foreach (var l in last6)
                                                {
                                                    ListOhlcvsOneHour.Add(l);
                                                }

                                                await Task.Delay(300);
                                                // 7日前
                                                dtTarget = dtTarget.AddDays(-1);
                                                List<Ohlcv> last7 = await GetCandlestick(pair, CandleTypes.OneHour, dtTarget);
                                                if (last7 != null)
                                                {
                                                    // 逆順にする
                                                    last7.Reverse();

                                                    foreach (var l in last7)
                                                    {
                                                        ListOhlcvsOneHour.Add(l);
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

                }

                #endregion

                //await Task.Delay(200);

                #region == OhlcvsOneMin 1min毎のデータ ==

                List<Ohlcv> ListOhlcvsOneMin = new List<Ohlcv>();

                if (ct == CandleTypes.OneMin)
                {
                    // TODO 取得中フラグセット。

                    Debug.WriteLine("今日の1min取得開始 " + pair.ToString());

                    // 一分毎のロウソク足タイプなら今日と昨日の1minデータを取得する必要あり。
                    ListOhlcvsOneMin = await GetCandlestick(pair, CandleTypes.OneMin, dtToday);
                    if (ListOhlcvsOneMin != null)
                    {
                        // 逆順にする
                        ListOhlcvsOneMin.Reverse();


                        // 00:00:00から23:59:59分までしか取れないので、 3時間分取るには、00:00:00から3:00までは 最新のデータとるには日付を１日マイナスする
                        if (dtToday.Hour <= 1) // BitWallpaper は一時間で良いので。// < 3
                        {
                            Debug.WriteLine("昨日の1min取得開始");

                            await Task.Delay(200);

                            // 昨日
                            DateTime dtTarget = dtToday.AddDays(-1);

                            List<Ohlcv> res = await GetCandlestick(pair, CandleTypes.OneMin, dtTarget);
                            if (res != null)
                            {
                                // 逆順にする
                                res.Reverse();

                                foreach (var r in res)
                                {
                                    ListOhlcvsOneMin.Add(r);
                                }
                            }
                            else
                            {
                                Debug.WriteLine("■■■■■ " + pair.ToString() + " GetCandlesticks error: 昨日の1min取得 null");
                            }
                        }
                        else
                        {
                            Debug.WriteLine("昨日の1min取得スキップ " + dtToday.Hour.ToString());
                        }
                    }
                    else
                    {
                        Debug.WriteLine("■■■■■ " + pair.ToString() + " GetCandlesticks error: 今日の1min取得 null");
                    }

                    // TODO 取得中フラグ解除。
                }

                #endregion

                //await Task.Delay(200);

                #region == OhlcvsOneDay 1day毎のデータ ==

                List<Ohlcv> ListOhlcvsOneDay = new List<Ohlcv>();

                if (ct == CandleTypes.OneDay)
                {
                    // TODO 取得中フラグセット。

                    // 1日のロウソク足タイプなら今年、去年、２年前、３年前、４年前、５年前の1hourデータを取得する必要あり。(５年前は止めた)

                    Debug.WriteLine("今年のOneDay取得開始 " + pair.ToString());

                    ListOhlcvsOneDay = await GetCandlestick(pair, CandleTypes.OneDay, dtToday);
                    if (ListOhlcvsOneDay != null)
                    {
                        // 逆順にする
                        ListOhlcvsOneDay.Reverse();

                        // 
                        //if (dtToday.Month <= 3)
                        //{
                        Debug.WriteLine("去年のOneDay取得開始 " + pair.ToString());

                        await Task.Delay(300);
                        // 去年
                        DateTime dtTarget = dtToday.AddYears(-1);

                        List<Ohlcv> res = await GetCandlestick(pair, CandleTypes.OneDay, dtTarget);
                        if (res != null)
                        {
                            // 逆順にする
                            res.Reverse();

                            foreach (var r in res)
                            {
                                ListOhlcvsOneDay.Add(r);
                            }

                            /*
                            Debug.WriteLine("一昨年のOneDay取得開始 " + pair.ToString());

                            await Task.Delay(300);
                            // 一昨年
                            dtTarget = dtTarget.AddYears(-1);
                            List<Ohlcv> last = await GetCandlestick(pair, CandleTypes.OneDay, dtTarget);
                            if (last != null)
                            {
                                // 逆順にする
                                last.Reverse();

                                foreach (var l in last)
                                {
                                    ListOhlcvsOneDay.Add(l);
                                }


                                // (５年前は止めた)
                            }
                            */

                        }

                        //}


                    }

                    // TODO 取得中フラグ解除。
                }

                #endregion


                ChartLoadingInfo = "";

                if (pair == Pairs.btc_jpy)
                {
                    if (ListOhlcvsOneHour != null)
                        OhlcvsOneHourBtc = ListOhlcvsOneHour;
                    if (ListOhlcvsOneMin != null)
                        OhlcvsOneMinBtc = ListOhlcvsOneMin;
                    if (ListOhlcvsOneDay != null)
                        OhlcvsOneDayBtc = ListOhlcvsOneDay;
                }
                else if (pair == Pairs.xrp_jpy)
                {
                    if (ListOhlcvsOneHour != null)
                        OhlcvsOneHourXrp = ListOhlcvsOneHour;
                    if (ListOhlcvsOneMin != null)
                        OhlcvsOneMinXrp = ListOhlcvsOneMin;
                    if (ListOhlcvsOneDay != null)
                        OhlcvsOneDayXrp = ListOhlcvsOneDay;
                }
                else if (pair == Pairs.eth_btc)
                {
                    if (ListOhlcvsOneHour != null)
                        OhlcvsOneHourEth = ListOhlcvsOneHour;
                    if (ListOhlcvsOneMin != null)
                        OhlcvsOneMinEth = ListOhlcvsOneMin;
                    if (ListOhlcvsOneDay != null)
                        OhlcvsOneDayEth = ListOhlcvsOneDay;
                }
                else if (pair == Pairs.mona_jpy)
                {
                    if (ListOhlcvsOneHour != null)
                        OhlcvsOneHourMona = ListOhlcvsOneHour;
                    if (ListOhlcvsOneMin != null)
                        OhlcvsOneMinMona = ListOhlcvsOneMin;
                    if (ListOhlcvsOneDay != null)
                        OhlcvsOneDayMona = ListOhlcvsOneDay;
                }
                else if (pair == Pairs.mona_btc)
                {
                    //
                }
                else if (pair == Pairs.ltc_btc)
                {
                    if (ListOhlcvsOneHour != null)
                        OhlcvsOneHourLtc = ListOhlcvsOneHour;
                    if (ListOhlcvsOneMin != null)
                        OhlcvsOneMinLtc = ListOhlcvsOneMin;
                    if (ListOhlcvsOneDay != null)
                        OhlcvsOneDayLtc = ListOhlcvsOneDay;
                }
                else if (pair == Pairs.bcc_btc)
                {
                    //
                }
                else if (pair == Pairs.bcc_jpy)
                {
                    if (ListOhlcvsOneHour != null)
                        OhlcvsOneHourBch = ListOhlcvsOneHour;
                    if (ListOhlcvsOneMin != null)
                        OhlcvsOneMinBch = ListOhlcvsOneMin;
                    if (ListOhlcvsOneDay != null)
                        OhlcvsOneDayBch = ListOhlcvsOneDay;
                }

            }
            catch (Exception ex)
            {

                ChartLoadingInfo = pair.ToString() + " チャートの追加中にエラーが発生しました 1 ";

                Debug.WriteLine("■■■■■ " + pair.ToString() + " GetCandlesticks error: " + ex.ToString());
            }

            return true;

        }

        // チャートを読み込み表示する。
        private void LoadChart(Pairs pair, CandleTypes ct)
        {
            ChartLoadingInfo = "チャートをロード中....";
            Debug.WriteLine("LoadChart... " + pair.ToString());

            try
            {

                List<Ohlcv> lst = null;
                int span = 0;

                if (ct == CandleTypes.OneMin)
                {
                    // 一分毎のロウソク足タイプなら
                    //lst = OhlcvsOneMin;
                    if (pair == Pairs.btc_jpy)
                    {
                        lst = OhlcvsOneMinBtc;
                    }
                    else if (pair == Pairs.xrp_jpy)
                    {
                        lst = OhlcvsOneMinXrp;
                    }
                    else if (pair == Pairs.eth_btc)
                    {
                        lst = OhlcvsOneMinEth;
                    }
                    else if (pair == Pairs.mona_jpy)
                    {
                        lst = OhlcvsOneMinMona;
                    }
                    else if (pair == Pairs.mona_btc)
                    {
                        //
                    }
                    else if (pair == Pairs.ltc_btc)
                    {
                        lst = OhlcvsOneMinLtc;
                    }
                    else if (pair == Pairs.bcc_btc)
                    {
                        //
                    }
                    else if (pair == Pairs.bcc_jpy)
                    {
                        lst = OhlcvsOneMinBch;
                    }

                    // 一時間の期間か１日の期間
                    if (_chartSpan == ChartSpans.OneHour)
                    {
                        span = 60 + 1;
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
                    //lst = OhlcvsOneHour;
                    if (pair == Pairs.btc_jpy)
                    {
                        lst = OhlcvsOneHourBtc;
                    }
                    else if (pair == Pairs.xrp_jpy)
                    {
                        lst = OhlcvsOneHourXrp;
                    }
                    else if (pair == Pairs.eth_btc)
                    {
                        lst = OhlcvsOneHourEth;
                    }
                    else if (pair == Pairs.mona_jpy)
                    {
                        lst = OhlcvsOneHourMona;
                    }
                    else if (pair == Pairs.mona_btc)
                    {
                        //
                    }
                    else if (pair == Pairs.ltc_btc)
                    {
                        lst = OhlcvsOneHourLtc;
                    }
                    else if (pair == Pairs.bcc_btc)
                    {
                        //
                    }
                    else if (pair == Pairs.bcc_jpy)
                    {
                        lst = OhlcvsOneHourBch;
                    }

                    // １日の期間か3日か１週間の期間
                    if (_chartSpan == ChartSpans.OneDay)
                    {
                        span = 24;
                    }
                    else if (_chartSpan == ChartSpans.ThreeDay)
                    {
                        span = (24 * 3);
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
                    //lst = OhlcvsOneDay;
                    if (pair == Pairs.btc_jpy)
                    {
                        lst = OhlcvsOneDayBtc;
                    }
                    else if (pair == Pairs.xrp_jpy)
                    {
                        lst = OhlcvsOneDayXrp;
                    }
                    else if (pair == Pairs.eth_btc)
                    {
                        lst = OhlcvsOneDayEth;
                    }
                    else if (pair == Pairs.mona_jpy)
                    {
                        lst = OhlcvsOneDayMona;
                    }
                    else if (pair == Pairs.mona_btc)
                    {
                        //
                    }
                    else if (pair == Pairs.ltc_btc)
                    {
                        lst = OhlcvsOneDayLtc;
                    }
                    else if (pair == Pairs.bcc_btc)
                    {
                        //
                    }
                    else if (pair == Pairs.bcc_jpy)
                    {
                        lst = OhlcvsOneDayBch;
                    }

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
                }

                //Debug.WriteLine("スパン：" + span.ToString());

                if (span == 0)
                {
                    Debug.WriteLine("スパン 0");
                    return;
                }

                if (lst == null)
                {
                    Debug.WriteLine("リスト Null " + pair.ToString());
                    return;
                }

                if (lst.Count < span - 1)
                {
                    Debug.WriteLine("ロード中？ " + pair.ToString() + " " + lst.Count.ToString() + " " + span.ToString());
                    return;
                }

                Debug.WriteLine("ロード中  " + pair.ToString() + " " + lst.Count.ToString() + " " + span.ToString());

                try
                {

                    SeriesCollection chartSeries = null;
                    AxesCollection chartAxisX = null;
                    AxesCollection chartAxisY = null;

                    if (pair == Pairs.btc_jpy)
                    {
                        chartSeries = ChartSeriesBtcJpy;
                        chartAxisX = ChartAxisXBtcJpy;
                        chartAxisY = ChartAxisYBtcJpy;
                    }
                    else if (pair == Pairs.xrp_jpy)
                    {
                        chartSeries = ChartSeriesXrpJpy;
                        chartAxisX = ChartAxisXXrpJpy;
                        chartAxisY = ChartAxisYXrpJpy;

                    }
                    else if (pair == Pairs.eth_btc)
                    {
                        chartSeries = ChartSeriesEthBtc;
                        chartAxisX = ChartAxisXEthBtc;
                        chartAxisY = ChartAxisYEthBtc;
                    }
                    else if (pair == Pairs.mona_jpy)
                    {
                        chartSeries = ChartSeriesMonaJpy;
                        chartAxisX = ChartAxisXMonaJpy;
                        chartAxisY = ChartAxisYMonaJpy;
                    }
                    else if (pair == Pairs.mona_btc)
                    {
                        //
                    }
                    else if (pair == Pairs.ltc_btc)
                    {
                        chartSeries = ChartSeriesLtcBtc;
                        chartAxisX = ChartAxisXLtcBtc;
                        chartAxisY = ChartAxisYLtcBtc;
                    }
                    else if (pair == Pairs.bcc_btc)
                    {
                        //
                    }
                    else if (pair == Pairs.bcc_jpy)
                    {
                        chartSeries = ChartSeriesBchJpy;
                        chartAxisX = ChartAxisXBchJpy;
                        chartAxisY = ChartAxisYBchJpy;
                    }

                    if (chartSeries == null)
                        return;
                    if (chartAxisX == null)
                        return;
                    if (chartAxisY == null)
                        return;

                    if (Application.Current == null) return;
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        try
                        {
                            // チャート OHLCVのロード
                            if (lst.Count > 0)
                            {
                                // Candlestickクリア
                                chartSeries[0].Values.Clear();

                                // 出来高クリア
                                //ChartSeries[1].Values.Clear();
                                // https://github.com/Live-Charts/Live-Charts/issues/76
                                for (int v = 0; v < chartSeries[1].Values.Count - 1; v++)
                                {
                                    chartSeries[1].Values[v] = (double)0;
                                }

                                // ラベル表示クリア
                                chartAxisX[0].Labels.Clear();

                                // 期間設定
                                chartAxisX[0].MaxValue = span - 1;
                                chartAxisX[0].MinValue = 0;

                                // Temp を作って、後でまとめて追加する。
                                // https://lvcharts.net/App/examples/v1/wpf/Performance%20Tips

                                var temporalCv = new OhlcPoint[span - 1];

                                var tempVol = new double[span - 1];

                                int i = 0;
                                int c = span;
                                foreach (var oh in lst)
                                {
                                    // 全てのポイントが同じ場合、スキップする。変なデータ？ 本家もスキップしている。
                                    if ((oh.Open == oh.High) && (oh.Open == oh.Low) && (oh.Open == oh.Close) && (oh.Volume == 0))
                                    {
                                        // スキップ止め。spanとの関係で表示がおかしくなる
                                        //continue;
                                    }

                                    // 表示数を限る 直近のspan本
                                    if (i < (span - 1))
                                    {

                                        // TODO あとでまとめて追加する。
                                        // ラベル
                                        if (ct == CandleTypes.OneMin)
                                        {
                                            chartAxisX[0].Labels.Insert(0, oh.TimeStamp.ToString("H:mm"));
                                        }
                                        else if (ct == CandleTypes.OneHour)
                                        {
                                            chartAxisX[0].Labels.Insert(0, oh.TimeStamp.ToString("d日 H:mm"));

                                        }
                                        else if (ct == CandleTypes.OneDay)
                                        {
                                            chartAxisX[0].Labels.Insert(0, oh.TimeStamp.ToString("M月d日"));
                                        }
                                        else
                                        {
                                            throw new System.InvalidOperationException("LoadChart: 不正な CandleType");
                                        }

                                        // ポイント作成
                                        OhlcPoint p = new OhlcPoint((double)oh.Open, (double)oh.High, (double)oh.Low, (double)oh.Close);


                                        // 直接追加しないで、
                                        //ChartSeries[0].Values.Add(p);
                                        // 一旦、Tempに追加して、あとでまとめてAddRange
                                        temporalCv[c - 2] = p;

                                        //ChartSeries[3].Values.Add((double)oh.Volume);
                                        tempVol[c - 2] = (double)oh.Volume;

                                        c = c - 1;

                                    }

                                    i = i + 1;
                                }

                                try
                                {
                                    // まとめて追加

                                    // OHLCV
                                    chartSeries[0].Values.AddRange(temporalCv);

                                    // Volume
                                    var cv = new ChartValues<double>();
                                    cv.AddRange(tempVol);
                                    chartSeries[1].Values = cv;

                                }
                                catch (Exception ex)
                                {

                                    ChartLoadingInfo = pair.ToString() + " チャートのロード中にエラーが発生しました 1 ";

                                    Debug.WriteLine("■■■■■ " + pair.ToString() + " Chart loading error: " + ex.ToString());
                                }
                                                            
                            }

                        }
                        catch (Exception ex)
                        {
                            ChartLoadingInfo = "チャートのロード中にエラーが発生しました 2 ";

                            Debug.WriteLine("■■■■■ Chart loading error: " + ex.ToString());
                        }

                    });
                    
                }
                catch (Exception ex)
                {
                    ChartLoadingInfo = "チャートのロード中にエラーが発生しました 3";

                    Debug.WriteLine("■■■■■ Chart loading error: " + ex.ToString());
                }

                Debug.WriteLine("ロード終わり  " + pair.ToString() + " " + lst.Count.ToString() + " " + span.ToString());
                ChartLoadingInfo = "";

            }
            catch (Exception ex)
            {

                ChartLoadingInfo = pair.ToString() + " チャートのロードにエラーが発生しました 1 ";

                Debug.WriteLine("■■■■■ " + pair.ToString() + " GetCandleLoadChartsticks error: " + ex.ToString());
            }

        }

        // 初回、データロードを確認して、チャートをロードする。
        private async void DisplayChart(Pairs pair)
        {
            bool bln = await GetCandlesticks(pair, SelectedCandleType);

            if (bln == true)
            {
                LoadChart(pair, SelectedCandleType);
            }
        }

        private async void DisplayCharts()
        {
            //foreach (DayOfWeek value in Enum.GetValues(typeof(DayOfWeek)))
            foreach (Pairs p in Enum.GetValues(typeof(Pairs)))
            {
                //Debug.WriteLine(p.ToString());

                if ((p == Pairs.mona_btc) || p == Pairs.bcc_btc)
                {
                    //Debug.WriteLine(p.ToString() + " skipping.");
                    continue;
                }
                else
                {
                    bool bln = await GetCandlesticks(p, SelectedCandleType);

                    if (bln == true)
                    {
                        LoadChart(p, SelectedCandleType);
                    }

                }

            }

        }

        // チャート表示期間を変えた時に
        private void ChangeChartSpan(Pairs pair)
        {

            // enum の期間選択からチャートを更新させる。
            // コンボボックスとダブルアップデートにならないようにするためコンボボックスでreturnしている。

            if (_chartSpan == ChartSpans.OneHour)
            {
                if (SelectedCandleType != CandleTypes.OneMin)
                {
                    SelectedCandleType = CandleTypes.OneMin;
                }
                else
                {
                    DisplayChart(pair);
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
                    DisplayChart(pair);
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
                    DisplayChart(pair);
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
                    DisplayChart(pair);
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
                    DisplayChart(pair);
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
                    DisplayChart(pair);
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
                    DisplayChart(pair);
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
                    DisplayChart(pair);
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
                    DisplayChart(pair);
                }
            }


            Debug.WriteLine("ChangeChartSpan");

        }

        private void ChangeChartSpans()
        {
            foreach (Pairs p in Enum.GetValues(typeof(Pairs)))
            {
                //Debug.WriteLine(p.ToString());

                if ((p == Pairs.mona_btc) || p == Pairs.bcc_btc)
                {
                    //Debug.WriteLine(p.ToString() + " skipping.");
                    continue;
                }
                else
                {
                    ChangeChartSpan(p);
                }
            }

        }

        // タイマーで、最新のロウソク足データを取得して追加する。
        private async void UpdateCandlestick(Pairs pair, CandleTypes ct)
        {
            ChartLoadingInfo = "チャートデータの更新中....";
            await Task.Delay(600);

            // 今日の日付セット。UTCで。
            DateTime dtToday = DateTime.Now.ToUniversalTime();

            DateTime dtLastUpdate;


            List<Ohlcv> ListOhlcvsOneMin = null;
            List<Ohlcv> ListOhlcvsOneHour = null;
            List<Ohlcv> ListOhlcvsOneDay = null;

            if (pair == Pairs.btc_jpy)
            {
                ListOhlcvsOneHour = OhlcvsOneHourBtc;
                ListOhlcvsOneMin = OhlcvsOneMinBtc;
                ListOhlcvsOneDay = OhlcvsOneDayBtc;
            }
            else if (pair == Pairs.xrp_jpy)
            {
                ListOhlcvsOneHour = OhlcvsOneHourXrp;
                ListOhlcvsOneMin = OhlcvsOneMinXrp;
                ListOhlcvsOneDay = OhlcvsOneDayXrp;
            }
            else if (pair == Pairs.eth_btc)
            {
                ListOhlcvsOneHour = OhlcvsOneHourEth;
                ListOhlcvsOneMin = OhlcvsOneMinEth;
                ListOhlcvsOneDay = OhlcvsOneDayEth;
            }
            else if (pair == Pairs.mona_jpy)
            {
                ListOhlcvsOneHour = OhlcvsOneHourMona;
                ListOhlcvsOneMin = OhlcvsOneMinMona;
                ListOhlcvsOneDay = OhlcvsOneDayMona;
            }
            else if (pair == Pairs.mona_btc)
            {
                //
            }
            else if (pair == Pairs.ltc_btc)
            {
                ListOhlcvsOneHour = OhlcvsOneHourLtc;
                ListOhlcvsOneMin = OhlcvsOneMinLtc;
                ListOhlcvsOneDay = OhlcvsOneDayLtc;
            }
            else if (pair == Pairs.bcc_btc)
            {
                //
            }
            else if (pair == Pairs.bcc_jpy)
            {
                ListOhlcvsOneHour = OhlcvsOneHourBch;
                ListOhlcvsOneMin = OhlcvsOneMinBch;
                ListOhlcvsOneDay = OhlcvsOneDayBch;
            }

            if (ListOhlcvsOneHour == null)
                return;
            if (ListOhlcvsOneMin == null)
                return;
            if (ListOhlcvsOneDay == null)
                return;

            #region == １分毎のデータ ==

            if (ct == CandleTypes.OneMin)
            {
                if (ListOhlcvsOneMin.Count > 0)
                {
                    dtLastUpdate = ListOhlcvsOneMin[0].TimeStamp;

                    //Debug.WriteLine(dtLastUpdate.ToString());

                    List<Ohlcv> latestOneMin = new List<Ohlcv>();

                    latestOneMin = await GetCandlestick(pair, CandleTypes.OneMin, dtToday);

                    if (latestOneMin != null)
                    {
                        //latestOneMin.Reverse();

                        if (latestOneMin.Count > 0)
                        {
                            foreach (var hoge in latestOneMin)
                            {

                                //Debug.WriteLine(hoge.TimeStamp.ToString()+" : "+ dtLastUpdate.ToString());

                                if (hoge.TimeStamp >= dtLastUpdate)
                                {

                                    // 全てのポイントが同じ場合、スキップする。変なデータ？ 本家もスキップしている。
                                    if ((hoge.Open == hoge.High) && (hoge.Open == hoge.Low) && (hoge.Open == hoge.Close) && (hoge.Volume == 0))
                                    {
                                        Debug.WriteLine("■ UpdateCandlestick 全てのポイントが同じ " + pair.ToString());
                                        //continue;
                                    }

                                    if (hoge.TimeStamp == dtLastUpdate)
                                    {
                                        // 更新前の最後のポイントを更新する。最終データは中途半端なので。

                                        Debug.WriteLine("１分毎のチャートデータ更新: " + hoge.TimeStamp.ToString() + " " + pair.ToString());

                                        ListOhlcvsOneMin[0].Open = hoge.Open;
                                        ListOhlcvsOneMin[0].High = hoge.High;
                                        ListOhlcvsOneMin[0].Low = hoge.Low;
                                        ListOhlcvsOneMin[0].Close = hoge.Close;
                                        ListOhlcvsOneMin[0].TimeStamp = hoge.TimeStamp;

                                        UpdateLastCandle(pair, CandleTypes.OneMin, hoge);

                                        //Debug.WriteLine(hoge.TimeStamp.ToString()+" : "+ dtLastUpdate.ToString());
                                    }
                                    else
                                    {
                                        // 新規ポイントを追加する。

                                        Debug.WriteLine("１分毎のチャートデータ追加: " + hoge.TimeStamp.ToString() + " " + pair.ToString());

                                        ListOhlcvsOneMin.Insert(0, hoge);

                                        AddCandle(pair, CandleTypes.OneMin, hoge);

                                        dtLastUpdate = hoge.TimeStamp;
                                    }


                                }

                            }


                        }

                    }
                }
            }

            #endregion

            #region == １時間毎のデータ ==

            if (ct == CandleTypes.OneHour)
            {
                if (ListOhlcvsOneHour.Count > 0)
                {
                    dtLastUpdate = ListOhlcvsOneHour[0].TimeStamp;

                    //TimeSpan ts = dtLastUpdate - dtToday;

                    //if (ts.TotalHours >= 1)
                    //{
                        //Debug.WriteLine(dtLastUpdate.ToString());

                        List<Ohlcv> latestOneHour = new List<Ohlcv>();

                        latestOneHour = await GetCandlestick(pair, CandleTypes.OneHour, dtToday);

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
                                        Debug.WriteLine("■ UpdateCandlestick 全てのポイントが同じ " + pair.ToString());
                                        //continue;
                                    }

                                    //Debug.WriteLine(hoge.TimeStamp.ToString()+" : "+ dtLastUpdate.ToString());

                                    if (hoge.TimeStamp >= dtLastUpdate)
                                    {

                                        if (hoge.TimeStamp == dtLastUpdate)
                                        {
                                            // 更新前の最後のポイントを更新する。最終データは中途半端なので。

                                            Debug.WriteLine("１時間チャートデータ更新: " + hoge.TimeStamp.ToString() + " " + pair.ToString());

                                            ListOhlcvsOneHour[0].Open = hoge.Open;
                                            ListOhlcvsOneHour[0].High = hoge.High;
                                            ListOhlcvsOneHour[0].Low = hoge.Low;
                                            ListOhlcvsOneHour[0].Close = hoge.Close;
                                            ListOhlcvsOneHour[0].TimeStamp = hoge.TimeStamp;

                                            UpdateLastCandle(pair, CandleTypes.OneHour, hoge);

                                            //Debug.WriteLine(hoge.TimeStamp.ToString() + " : " + dtLastUpdate.ToString());
                                        }
                                        else
                                        {
                                            // 新規ポイントを追加する。

                                            Debug.WriteLine("１時間チャートデータ追加: " + hoge.TimeStamp.ToString());

                                            ListOhlcvsOneHour.Insert(0, hoge);

                                            AddCandle(pair, CandleTypes.OneHour, hoge);

                                            dtLastUpdate = hoge.TimeStamp;
                                        }

                                    }

                                }

                            }

                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("■■■■■ UpdateCandlestick GetCandlestick One hour returned null " + pair.ToString());
                        }


                    //}

                }

            }

            #endregion

            #region == １日毎のデータ ==

            if (ct == CandleTypes.OneDay)
            {
                if (ListOhlcvsOneDay.Count > 0)
                {
                    dtLastUpdate = ListOhlcvsOneDay[0].TimeStamp;

                    //TimeSpan ts = dtLastUpdate - dtToday;

                    //if (ts.TotalDays >= 1)
                    //{
                        //Debug.WriteLine(dtLastUpdate.ToString());

                        List<Ohlcv> latestOneDay = new List<Ohlcv>();

                        latestOneDay = await GetCandlestick(pair, CandleTypes.OneDay, dtToday);

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
                                        //continue;
                                    }

                                    //Debug.WriteLine(hoge.TimeStamp.ToString()+" : "+ dtLastUpdate.ToString());

                                    if (hoge.TimeStamp >= dtLastUpdate)
                                    {

                                        if (hoge.TimeStamp == dtLastUpdate)
                                        {
                                            // 更新前の最後のポイントを更新する。最終データは中途半端なので。

                                            Debug.WriteLine("１日チャートデータ更新: " + hoge.TimeStamp.ToString());

                                            ListOhlcvsOneDay[0].Open = hoge.Open;
                                            ListOhlcvsOneDay[0].High = hoge.High;
                                            ListOhlcvsOneDay[0].Low = hoge.Low;
                                            ListOhlcvsOneDay[0].Close = hoge.Close;
                                            ListOhlcvsOneDay[0].TimeStamp = hoge.TimeStamp;

                                            UpdateLastCandle(pair, CandleTypes.OneDay, hoge);

                                            //Debug.WriteLine(hoge.TimeStamp.ToString() + " : " + dtLastUpdate.ToString());
                                        }
                                        else
                                        {
                                            // 新規ポイントを追加する。

                                            Debug.WriteLine("１日チャートデータ追加: " + hoge.TimeStamp.ToString());

                                            ListOhlcvsOneDay.Insert(0, hoge);

                                            AddCandle(pair, CandleTypes.OneDay, hoge);

                                            dtLastUpdate = hoge.TimeStamp;
                                        }

                                    }

                                }

                            }

                        }


                    //}

                }
            }

            #endregion


            ChartLoadingInfo = "";

        }

        // チャートの最後に最新ポイントを追加して更新表示する。
        private void AddCandle(Pairs pair, CandleTypes ct, Ohlcv newData)
        {
            // 表示されているのだけ更新。それ以外は不要。
            //if (SelectedCandleType != ct) return;

            //Debug.WriteLine("チャートの更新 追加: "+ newData.TimeStamp.ToString());

            SeriesCollection chartSeries = null;
            AxesCollection chartAxisX = null;
            AxesCollection chartAxisY = null;

            if (pair == Pairs.btc_jpy)
            {
                chartSeries = ChartSeriesBtcJpy;
                chartAxisX = ChartAxisXBtcJpy;
                chartAxisY = ChartAxisYBtcJpy;
            }
            else if (pair == Pairs.xrp_jpy)
            {
                chartSeries = ChartSeriesXrpJpy;
                chartAxisX = ChartAxisXXrpJpy;
                chartAxisY = ChartAxisYXrpJpy;
            }
            else if (pair == Pairs.eth_btc)
            {
                chartSeries = ChartSeriesEthBtc;
                chartAxisX = ChartAxisXEthBtc;
                chartAxisY = ChartAxisYEthBtc;
            }
            else if (pair == Pairs.mona_jpy)
            {
                chartSeries = ChartSeriesMonaJpy;
                chartAxisX = ChartAxisXMonaJpy;
                chartAxisY = ChartAxisYMonaJpy;
            }
            else if (pair == Pairs.mona_btc)
            {
                //
            }
            else if (pair == Pairs.ltc_btc)
            {
                chartSeries = ChartSeriesLtcBtc;
                chartAxisX = ChartAxisXLtcBtc;
                chartAxisY = ChartAxisYLtcBtc;
            }
            else if (pair == Pairs.bcc_btc)
            {
                //
            }
            else if (pair == Pairs.bcc_jpy)
            {
                chartSeries = ChartSeriesBchJpy;
                chartAxisX = ChartAxisXBchJpy;
                chartAxisY = ChartAxisYBchJpy;
            }

            if (chartSeries == null)
                return;

            if (chartSeries[0].Values != null)
            {
                if (chartSeries[0].Values.Count > 0)
                {
                    if (Application.Current == null) return;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            // ポイント作成
                            OhlcPoint p = new OhlcPoint((double)newData.Open, (double)newData.High, (double)newData.Low, (double)newData.Close);
                            // 一番古いの削除
                            chartSeries[0].Values.RemoveAt(0);
                            // 追加
                            chartSeries[0].Values.Add(p);

                            // 出来高
                            chartSeries[1].Values.RemoveAt(0);
                            chartSeries[1].Values.Add((double)newData.Volume);

                            // ラベル
                            chartAxisX[0].Labels.RemoveAt(0);
                            if (ct == CandleTypes.OneMin)
                            {
                                chartAxisX[0].Labels.Add(newData.TimeStamp.ToString("HH:mm"));
                            }
                            else if (ct == CandleTypes.OneHour)
                            {
                                chartAxisX[0].Labels.Add(newData.TimeStamp.ToString("dd日 HH:mm"));

                            }
                            else if (ct == CandleTypes.OneDay)
                            {
                                chartAxisX[0].Labels.Add(newData.TimeStamp.ToString("MM月dd日"));
                            }
                            else
                            {
                                throw new System.InvalidOperationException("UpdateChart: 不正な CandleTypes");
                            }

                        }
                        catch (Exception ex)
                        {

                            ChartLoadingInfo = pair.ToString() + " チャートの追加中にエラーが発生しました 1 ";

                            Debug.WriteLine("■■■■■ " + pair.ToString() + " Chart adding error: " + ex.ToString());
                        }

                    });

                }
            }

        }

        // チャートの最後のポイントを最新情報に更新表示する。
        private void UpdateLastCandle(Pairs pair, CandleTypes ct, Ohlcv newData)
        {
            // 表示されているのだけ更新。それ以外は不要。
            //if (SelectedCandleType != ct) return;

            //Debug.WriteLine("チャートの更新 追加: "+ newData.TimeStamp.ToString());

            SeriesCollection chartSeries = null;
            AxesCollection chartAxisX = null;
            AxesCollection chartAxisY = null;

            if (pair == Pairs.btc_jpy)
            {
                chartSeries = ChartSeriesBtcJpy;
                chartAxisX = ChartAxisXBtcJpy;
                chartAxisY = ChartAxisYBtcJpy;
            }
            else if (pair == Pairs.xrp_jpy)
            {
                chartSeries = ChartSeriesXrpJpy;
                chartAxisX = ChartAxisXXrpJpy;
                chartAxisY = ChartAxisYXrpJpy;
            }
            else if (pair == Pairs.eth_btc)
            {
                chartSeries = ChartSeriesEthBtc;
                chartAxisX = ChartAxisXEthBtc;
                chartAxisY = ChartAxisYEthBtc;
            }
            else if (pair == Pairs.mona_jpy)
            {
                chartSeries = ChartSeriesMonaJpy;
                chartAxisX = ChartAxisXMonaJpy;
                chartAxisY = ChartAxisYMonaJpy;
            }
            else if (pair == Pairs.mona_btc)
            {
                //
            }
            else if (pair == Pairs.ltc_btc)
            {
                chartSeries = ChartSeriesLtcBtc;
                chartAxisX = ChartAxisXLtcBtc;
                chartAxisY = ChartAxisYLtcBtc;
            }
            else if (pair == Pairs.bcc_btc)
            {
                //
            }
            else if (pair == Pairs.bcc_jpy)
            {
                chartSeries = ChartSeriesBchJpy;
                chartAxisX = ChartAxisXBchJpy;
                chartAxisY = ChartAxisYBchJpy;
            }

            if (chartSeries == null)
                return;

            if (chartSeries[0].Values != null)
            {
                if (chartSeries[0].Values.Count > 0)
                {

                    if (Application.Current == null) return;
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                        ((OhlcPoint)chartSeries[0].Values[chartSeries[0].Values.Count - 1]).Open = (double)newData.Open;
                        ((OhlcPoint)chartSeries[0].Values[chartSeries[0].Values.Count - 1]).High = (double)newData.High;
                        ((OhlcPoint)chartSeries[0].Values[chartSeries[0].Values.Count - 1]).Low = (double)newData.Low;
                        ((OhlcPoint)chartSeries[0].Values[chartSeries[0].Values.Count - 1]).Close = (double)newData.Close;

                    });

                }
            }

        }

        #endregion

        #endregion

        #region == コマンド ==

        // (ESC)
        public ICommand EscCommand { get; }
        public bool EscCommand_CanExecute()
        {
            return true;
        }
        public void EscCommand_Execute()
        {
            if (ShowSettings)
                SettingsCancelCommand_Execute();

            LoginCancelCommand_Execute();

        }

        #region == 認証・設定画面表示関係 ==

        // 設定画面表示
        public ICommand ShowSettingsCommand { get; }
        public bool ShowSettingsCommand_CanExecute()
        {
            return true;
        }
        public void ShowSettingsCommand_Execute()
        {
            if (ShowSettings)
            {
                ShowSettings = false;
            }
            else
            {
                ShowSettings = true;
            }
        }

        // 設定画面キャンセルボタン
        public ICommand SettingsCancelCommand { get; }
        public bool SettingsCancelCommand_CanExecute()
        {
            return true;
        }
        public void SettingsCancelCommand_Execute()
        {
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
            ShowSettings = false;
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

        // ログイン画面キャンセル
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

        #endregion

        #region == APIキー ==

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
            if (obj == null) return;

            Decimal numVal = Decimal.Parse(obj.ToString());

            if (ActivePair.DepthGrouping != numVal)
            {
                ActivePair.DepthGrouping = numVal;
                DepthGroupingChanged = true;
            }
        }

        // 最小表示エコ view モード
        public ICommand ViewMinimumCommand { get; }
        public bool ViewMinimumCommand_CanExecute()
        {
            return true;
        }
        public void ViewMinimumCommand_Execute()
        {
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
            if (MinMode)
            {
                MinMode = false;
            }
        }


        #endregion

    }

}
