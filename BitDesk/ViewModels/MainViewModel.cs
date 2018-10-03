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
using System.Text;
using Microsoft.Win32;

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
                this.NotifyPropertyChanged("IfdDoTriggerPriceString");
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
                this.NotifyPropertyChanged("OcoOneTriggerPriceString");
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
                this.NotifyPropertyChanged("OcoOtherTriggerPriceString");
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

        public string IfdDoTriggerPriceString
        {
            get
            {
                if (IfdDoTriggerUpDown == 0)
                {
                    return ">=" + IfdDoTriggerPrice.ToString();
                }
                else
                {
                    return "<=" + IfdDoTriggerPrice.ToString();
                }
            }
        }
        public string OcoOneTriggerPriceString
        {
            get
            {
                if (OcoOneTriggerUpDown == 0)
                {
                    return ">=" + OcoOneTriggerPrice.ToString();
                }
                else
                {
                    return "<=" + OcoOneTriggerPrice.ToString();
                }
            }
        }
        public string OcoOtherTriggerPriceString
        {
            get
            {
                if (OcoOtherTriggerUpDown == 0)
                {
                    return ">=" + OcoOtherTriggerPrice.ToString();
                }
                else
                {
                    return "<=" + OcoOtherTriggerPrice.ToString();
                }
            }
        }

        public Ifdoco()
        {

        }

    }

    #endregion

    #region == 自動取引 AutoTrade クラス ==

    public class AutoTrade : ViewModelBase
    {
        // 停滞カウンター
        /*
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
        */

        // ビジーリトライ待機カウンター （70010, "ただいまシステム負荷が高まっているため、最小注文数量を一時的に引き上げています。"）
        private int _autoTradeSrvBusyRetryCounter;
        public int AutoTradeSrvBusyRetryCounter
        {
            get
            {
                return _autoTradeSrvBusyRetryCounter;
            }
            set
            {
                if (_autoTradeSrvBusyRetryCounter == value)
                    return;

                _autoTradeSrvBusyRetryCounter = value;
                this.NotifyPropertyChanged("AutoTradeSrvBusyRetryCounter");
            }
        }


        // ロスカットカウンター
        private int _lossCutCounter;
        public int LossCutCounter
        {
            get
            {
                return _lossCutCounter;
            }
            set
            {
                if (_lossCutCounter == value)
                    return;

                _lossCutCounter = value;
                this.NotifyPropertyChanged("LossCutCounter");
            }
        }

        // 買い
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

        // 売り
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

        public AutoTrade()
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
        public string IconData { get; set; }
    }

    #endregion

    #endregion

    /// <summary>
    /// メインのビューモデル
    /// </summary>
    public class MainViewModel : ViewModelBase
    {

        #region == 基本 ==

        // テスト用
        private decimal _initPrice = 101937M;

        // プライベートモード表示切替（自動取引表示）
        public bool ExperimentalMode
        {
            get
            {
                /*
                #if DEBUG
                return true;
                #else
                return false; 
                #endif
                */

                return false;
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

        // ベータモード表示切替フラグ（特殊注文ｖ0.0.3.0）
        public bool BetaMode
        {
            get
            {
                return true;
            }
        }

        // Application version
        private string _appVer = "0.0.0.3";

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
        //（設定画面を表示した後、メインかチャート一覧表示に戻るか、覚えておくフラグ）
        private bool _allChartMode = false;

        // LiveChartの不明なエクセプションが起きるので、ver 1 が出るまで使わない。
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
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                    });

                    DepthGroupingChanged = true;

                    // 主にチャートの切替
                    IsBtcJpyVisible = true;
                    IsXrpJpyVisible = false;
                    IsEthBtcVisible = false;
                    IsLtcBtcVisible = false;
                    IsMonaJpyVisible = false;
                    IsBchJpyVisible = false;

                    // 資産情報の切替
                    IsJpyAssetVisible = true;
                    IsBtcJpyAssetVisible = true;
                    IsXrpJpyAssetVisible = false;
                    IsEthBtcAssetVisible = false;
                    IsLtcBtcAssetVisible = false;
                    IsMonaJpyAssetVisible = false;
                    IsBchJpyAssetVisible = false;

                }
                else if (_activePairIndex == 1)
                {
                    CurrentPair = Pairs.xrp_jpy;
                    ActivePair = PairXrpJpy;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                    });

                    DepthGroupingChanged = true;

                    IsBtcJpyVisible = false;
                    IsXrpJpyVisible = true;
                    IsEthBtcVisible = false;
                    IsLtcBtcVisible = false;
                    IsMonaJpyVisible = false;
                    IsBchJpyVisible = false;

                    IsJpyAssetVisible = true;
                    IsBtcJpyAssetVisible = false;
                    IsXrpJpyAssetVisible = true;
                    IsEthBtcAssetVisible = false;
                    IsLtcBtcAssetVisible = false;
                    IsMonaJpyAssetVisible = false;
                    IsBchJpyAssetVisible = false;

                }
                else if (_activePairIndex == 2)
                {
                    CurrentPair = Pairs.ltc_btc;
                    ActivePair = PairLtcBtc;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                    });

                    DepthGroupingChanged = true;

                    IsBtcJpyVisible = false;
                    IsXrpJpyVisible = false;
                    IsEthBtcVisible = false;
                    IsLtcBtcVisible = true;
                    IsMonaJpyVisible = false;
                    IsBchJpyVisible = false;

                    IsJpyAssetVisible = false;
                    IsBtcJpyAssetVisible = true;
                    IsXrpJpyAssetVisible = false;
                    IsEthBtcAssetVisible = false;
                    IsLtcBtcAssetVisible = true;
                    IsMonaJpyAssetVisible = false;
                    IsBchJpyAssetVisible = false;

                }
                else if (_activePairIndex == 3)
                {
                    CurrentPair = Pairs.eth_btc;
                    ActivePair = PairEthBtc;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                    });

                    DepthGroupingChanged = true;

                    IsBtcJpyVisible = false;
                    IsXrpJpyVisible = false;
                    IsEthBtcVisible = true;
                    IsLtcBtcVisible = false;
                    IsMonaJpyVisible = false;
                    IsBchJpyVisible = false;

                    IsJpyAssetVisible = false;
                    IsBtcJpyAssetVisible = true;
                    IsXrpJpyAssetVisible = false;
                    IsEthBtcAssetVisible = true;
                    IsLtcBtcAssetVisible = false;
                    IsMonaJpyAssetVisible = false;
                    IsBchJpyAssetVisible = false;

                }
                else if (_activePairIndex == 4)
                {
                    CurrentPair = Pairs.mona_jpy;
                    ActivePair = PairMonaJpy;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                    });

                    DepthGroupingChanged = true;

                    IsBtcJpyVisible = false;
                    IsXrpJpyVisible = false;
                    IsEthBtcVisible = false;
                    IsLtcBtcVisible = false;
                    IsMonaJpyVisible = true;
                    IsBchJpyVisible = false;

                    IsJpyAssetVisible = true;
                    IsBtcJpyAssetVisible = false;
                    IsXrpJpyAssetVisible = false;
                    IsEthBtcAssetVisible = false;
                    IsLtcBtcAssetVisible = false;
                    IsMonaJpyAssetVisible = true;
                    IsBchJpyAssetVisible = false;

                }
                else if (_activePairIndex == 5)
                {
                    CurrentPair = Pairs.bcc_jpy;
                    ActivePair = PairBchJpy;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                    });

                    DepthGroupingChanged = true;

                    IsBtcJpyVisible = false;
                    IsXrpJpyVisible = false;
                    IsEthBtcVisible = false;
                    IsLtcBtcVisible = false;
                    IsMonaJpyVisible = false;
                    IsBchJpyVisible = true;

                    IsJpyAssetVisible = true;
                    IsBtcJpyAssetVisible = false;
                    IsXrpJpyAssetVisible = false;
                    IsEthBtcAssetVisible = false;
                    IsLtcBtcAssetVisible = false;
                    IsMonaJpyAssetVisible = false;
                    IsBchJpyAssetVisible = true;

                }

                // 注文の値をクリア
                SellAmount = 0;
                SellPrice = 0;
                BuyAmount = 0;
                BuyPrice = 0;
                this.NotifyPropertyChanged("BuyEstimatePrice");
                this.NotifyPropertyChanged("SellEstimatePrice");

                // タブの「自動取引（On）」を更新
                this.NotifyPropertyChanged("AutoTradeTitle");

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
                return CurrentPairCoin[CurrentPair];//.ToUpper();
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
            {Pairs.btc_jpy, "BTC"},
            {Pairs.xrp_jpy, "XRP"},
            {Pairs.eth_btc, "ETH"},
            {Pairs.ltc_btc, "LTC"},
            {Pairs.mona_jpy, "Mona"},
            {Pairs.mona_btc, "Mona"},
            {Pairs.bcc_jpy, "BCH"},
            {Pairs.bcc_btc, "BCH"},
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

        private bool _isBtcJpyVisible;
        public bool IsBtcJpyVisible
        {
            get
            {
                return _isBtcJpyVisible;
            }
            set
            {
                if (_isBtcJpyVisible == value)
                    return;

                _isBtcJpyVisible = value;
                this.NotifyPropertyChanged("IsBtcJpyVisible");
            }
        }

        private Pair _pairXrpJpy = new Pair(Pairs.xrp_jpy, 45, "{0:#,0.000}", 0.1M, 0.01M);
        public Pair PairXrpJpy
        {
            get
            {
                return _pairXrpJpy;
            }
        }

        private bool _isXrpJpyVisible;
        public bool IsXrpJpyVisible
        {
            get
            {
                return _isXrpJpyVisible;
            }
            set
            {
                if (_isXrpJpyVisible == value)
                    return;

                _isXrpJpyVisible = value;
                this.NotifyPropertyChanged("IsXrpJpyVisible");
            }
        }

        private Pair _pairEthBtc = new Pair(Pairs.eth_btc, 30, "{0:#,0.00000000}", 0.0001M, 0.00001M);
        public Pair PairEthBtc
        {
            get
            {
                return _pairEthBtc;
            }
        }

        private bool _isEthBtcVisible;
        public bool IsEthBtcVisible
        {
            get
            {
                return _isEthBtcVisible;
            }
            set
            {
                if (_isEthBtcVisible == value)
                    return;

                _isEthBtcVisible = value;
                this.NotifyPropertyChanged("IsEthBtcVisible");
            }
        }

        private Pair _pairLtcBtc = new Pair(Pairs.ltc_btc ,30, "{0:#,0.00000000}", 0.0001M, 0.00001M);
        public Pair PairLtcBtc
        {
            get
            {
                return _pairLtcBtc;
            }
        }

        private bool _isLtcBtcVisible;
        public bool IsLtcBtcVisible
        {
            get
            {
                return _isLtcBtcVisible;
            }
            set
            {
                if (_isLtcBtcVisible == value)
                    return;

                _isLtcBtcVisible = value;
                this.NotifyPropertyChanged("IsLtcBtcVisible");
            }
        }

        private Pair _pairMonaJpy = new Pair(Pairs.mona_jpy ,45, "{0:#,0.000}", 0.1M, 1M);
        public Pair PairMonaJpy
        {
            get
            {
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
                if (ActivePair.AutoTradeStart)
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
            {CandleTypes.OneMin, "１分足"},
            //{CandleTypes.FiveMin, "５分" },
            //{CandleTypes.FifteenMin, "１５分"},
            //{CandleTypes.ThirteenMin, "３０分" },
            {CandleTypes.OneHour, "１時間" },
            //{CandleTypes.FourHour, "４時間"},
            //{CandleTypes.EightHour, "８時間" },
            //{CandleTypes.TwelveHour, "１２時間"},
            {CandleTypes.OneDay, "日足" },
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

                //Debug.WriteLine("SelectedCandleType " + _selectedCandleType.ToString());

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
        System.Windows.Threading.DispatcherTimer dispatcherTimerRss = new System.Windows.Threading.DispatcherTimer();

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
                    return _assetJPYAmount * 100;
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
                // 利用可能額は小数点以下を切り捨て（読みやすいように）
                //return _assetJPYFreeAmount;
                return Math.Floor(_assetJPYFreeAmount);
            }
            set
            {
                if (_assetJPYFreeAmount == value)
                    return;

                _assetJPYFreeAmount = value;
                this.NotifyPropertyChanged("AssetJPYFreeAmount");
            }
        }

        // 資産情報の表示切替
        private bool _isJpyAssetVisible;
        public bool IsJpyAssetVisible
        {
            get
            {
                return _isJpyAssetVisible;
            }
            set
            {
                if (_isJpyAssetVisible == value)
                    return;

                _isJpyAssetVisible = value;
                this.NotifyPropertyChanged("IsJpyAssetVisible");
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
                    return _assetBTCAmount * 10000;
                }
                else
                {
                    //return Math.Floor((_assetBTCAmount * 10000M)) / 10000M;
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
                    return _assetBTCFreeAmount * 100;
                }
                else
                {
                    // 売買出来ない桁は、切り捨てで。
                    //return _assetBTCFreeAmount;
                    return Math.Floor((_assetBTCFreeAmount * 10000M)) / 10000M;
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
                this.NotifyPropertyChanged("AssetAllEstimateAmountString");
                this.NotifyPropertyChanged("AssetAllShushi");

            }
        }

        // 円建て総資産時価評価額合計文字列
        public string AssetAllEstimateAmountString
        {
            get { return String.Format("{0:#,0}", _assetBTCEstimateAmount + _assetJPYAmount + AssetXRPEstimateAmount + AssetLtcEstimateAmount + AssetEthEstimateAmount + AssetMonaEstimateAmount + AssetBchEstimateAmount); }
        }

        private bool _isBtcJpyAssetVisible;
        public bool IsBtcJpyAssetVisible
        {
            get
            {
                return _isBtcJpyAssetVisible;
            }
            set
            {
                if (_isBtcJpyAssetVisible == value)
                    return;

                _isBtcJpyAssetVisible = value;
                this.NotifyPropertyChanged("IsBtcJpyAssetVisible");
            }
        }

        // 収支概算
        public string AssetAllShushi
        {
            get
            {
                var assetAll = _assetBTCEstimateAmount + _assetJPYAmount + AssetXRPEstimateAmount + AssetLtcEstimateAmount + AssetEthEstimateAmount + AssetMonaEstimateAmount + AssetBchEstimateAmount;
                if (assetAll > _initPrice)
                {
                    return "+" + String.Format("{0:#,0}", assetAll - _initPrice);
                }
                else if (assetAll < _initPrice)
                {
                    return String.Format("{0:#,0}", _initPrice - assetAll);
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

        private bool _isXrpJpyAssetVisible;
        public bool IsXrpJpyAssetVisible
        {
            get
            {
                return _isXrpJpyAssetVisible;
            }
            set
            {
                if (_isXrpJpyAssetVisible == value)
                    return;

                _isXrpJpyAssetVisible = value;
                this.NotifyPropertyChanged("IsXrpJpyAssetVisible");
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

        private bool _isLtcBtcAssetVisible;
        public bool IsLtcBtcAssetVisible
        {
            get
            {
                return _isLtcBtcAssetVisible;
            }
            set
            {
                if (_isLtcBtcAssetVisible == value)
                    return;

                _isLtcBtcAssetVisible = value;
                this.NotifyPropertyChanged("IsLtcBtcAssetVisible");
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

        private bool _isEthBtcAssetVisible;
        public bool IsEthBtcAssetVisible
        {
            get
            {
                return _isEthBtcAssetVisible;
            }
            set
            {
                if (_isEthBtcAssetVisible == value)
                    return;

                _isEthBtcAssetVisible = value;
                this.NotifyPropertyChanged("IsEthBtcAssetVisible");
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

        private bool _isMonaJpyAssetVisible;
        public bool IsMonaJpyAssetVisible
        {
            get
            {
                return _isMonaJpyAssetVisible;
            }
            set
            {
                if (_isMonaJpyAssetVisible == value)
                    return;

                _isMonaJpyAssetVisible = value;
                this.NotifyPropertyChanged("IsMonaJpyAssetVisible");
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

        private bool _isBchJpyAssetVisible;
        public bool IsBchJpyAssetVisible
        {
            get
            {
                return _isBchJpyAssetVisible;
            }
            set
            {
                if (_isBchJpyAssetVisible == value)
                    return;

                _isBchJpyAssetVisible = value;
                this.NotifyPropertyChanged("IsBchJpyAssetVisible");
            }
        }


        #endregion

        #region == 手動取引用のプロパティ ==

        // Order 発注
        // 売り数量
        private decimal _sellAmount;// = 0.001M; // 通貨別デフォ指定 TODO
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

                APIResultSellCommandOrderIDString = "";
                APIResultSellCommandErrorString = "";
                APIResultSellCommandResult = "";
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

                APIResultSellCommandOrderIDString = "";
                APIResultSellCommandErrorString = "";
                APIResultSellCommandResult = "";
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

                APIResultSellCommandOrderIDString = "";
                APIResultSellCommandErrorString = "";
                APIResultSellCommandResult = "";
            }
        }

        // 売り予想金額
        public decimal SellEstimatePrice
        {
            get
            {
                if (SellType == OrderTypes.market)
                {
                    return SellAmount * ActivePair.Bid;//_bid;
                }
                else
                {
                    return SellAmount * SellPrice;
                }
            }
        }

        // 買い数量
        private decimal _buyAmount;// = 0.001M; // 通貨別デフォ指定 TODO
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

                APIResultBuyCommandOrderIDString = "";
                APIResultBuyCommandErrorString = "";
                APIResultBuyCommandResult = "";
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

                APIResultBuyCommandOrderIDString = "";
                APIResultBuyCommandErrorString = "";
                APIResultBuyCommandResult = "";
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

                APIResultBuyCommandOrderIDString = "";
                APIResultBuyCommandErrorString = "";
                APIResultBuyCommandResult = "";
            }
        }

        // 買い予想金額
        public decimal BuyEstimatePrice
        {
            get
            {
                if (BuyType == OrderTypes.market)
                {
                    return BuyAmount * ActivePair.Ask;//_ask;
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
                // don't 同じ値でも値をセットした時にBlinkさせたい。
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
                // don't 同じ値でも値をセットした時にBlinkさせたい。
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

        #region == 特殊注文(IFDOCO)用のプロパティ ==

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
                        return _iFD_IfdAmount * ActivePair.Ask;
                    }
                    else if (_iFD_IfdSide == IfdocoSide.sell)
                    {
                        return _iFD_IfdAmount * ActivePair.Bid;
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
                        return _iFD_DoAmount * ActivePair.Ask;
                    }
                    else if (_iFD_DoSide == IfdocoSide.sell)
                    {
                        return _iFD_DoAmount * ActivePair.Bid;
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
                this.NotifyPropertyChanged("IFD_DoTriggerPriceString");
            }
        }

        // トリガー[以上(0)以下(1)] // デフォ(0)
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

        public string IFD_DoTriggerPriceString
        {
            get
            {
                if (IFD_DoTriggerUpDown == 0)
                {
                    return ">=" + IFD_DoTriggerPrice.ToString();
                }
                else
                {
                    return "<=" + IFD_DoTriggerPrice.ToString();
                }
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
                        return _oCO_OneAmount * ActivePair.Ask;
                    }
                    else if (_oCO_OneSide == IfdocoSide.sell)
                    {
                        return _oCO_OneAmount * ActivePair.Bid;
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
                this.NotifyPropertyChanged("OCO_OneTriggerPriceString");
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

        public string OCO_OneTriggerPriceString
        {
            get
            {
                if (OCO_OneTriggerUpDown == 0)
                {
                    return ">=" + OCO_OneTriggerPrice.ToString();
                }
                else
                {
                    return "<=" + OCO_OneTriggerPrice.ToString();
                }
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
                        return _oCO_OtherAmount * ActivePair.Ask;
                    }
                    else if (_oCO_OtherSide == IfdocoSide.sell)
                    {
                        return _oCO_OtherAmount * ActivePair.Bid;
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
                this.NotifyPropertyChanged("OCO_OtherTriggerPriceString");
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

        public string OCO_OtherTriggerPriceString
        {
            get
            {
                if (OCO_OtherTriggerUpDown == 0)
                {
                    return ">=" + OCO_OtherTriggerPrice.ToString();
                }
                else
                {
                    return "<=" + OCO_OtherTriggerPrice.ToString();
                }
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
                        return _iFDOCO_IfdAmount * ActivePair.Ask;
                    }
                    else if (_iFDOCO_IfdSide == IfdocoSide.sell)
                    {
                        return _iFDOCO_IfdAmount * ActivePair.Bid;
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
                        return _iFDOCO_OneAmount * ActivePair.Ask;
                    }
                    else if (_iFDOCO_OneSide == IfdocoSide.sell)
                    {
                        return _iFDOCO_OneAmount * ActivePair.Bid;
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
                        return _iFDOCO_OtherAmount * ActivePair.Ask;
                    }
                    else if (_iFDOCO_OtherSide == IfdocoSide.sell)
                    {
                        return _iFDOCO_OtherAmount * ActivePair.Bid;
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

        #region == 各通貨ペア用のクラス ==

        /// <summary>
        /// 各通貨ペア毎の情報を保持するクラス
        /// </summary>
        public class Pair : ViewModelBase
        {

            // 通貨フォーマット用
            private string _ltpFormstString = "{0:#,0}";
            // 通貨ペア
            private Pairs _p;
            public Pairs ThisPair
            {
                get
                {
                    return _p;
                }
            }

            // 表示用 通貨ペア名 "BTC/JPY";
            public string PairString
            {
                get
                {
                    return PairStrings[_p];
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

            #region == Tick系 ==

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

                    if (_ltp > BasePrice)
                    {
                        BasePriceUpFlag = true;
                    }
                    else if (_ltp < BasePrice)
                    {
                        BasePriceUpFlag = false;
                    }
                    this.NotifyPropertyChanged("BasePriceIcon");

                    if (_ltp > MiddleInitPrice)
                    {
                        MiddleInitPriceUpFlag = true;
                    }
                    else if (_ltp < MiddleInitPrice)
                    {
                        MiddleInitPriceUpFlag = false;
                    }
                    this.NotifyPropertyChanged("MiddleInitPriceIcon");

                    if (_ltp > MiddleLast24Price)
                    {
                        MiddleLast24PriceUpFlag = true;
                    }
                    else if (_ltp < MiddleLast24Price)
                    {
                        MiddleLast24PriceUpFlag = false;
                    }
                    this.NotifyPropertyChanged("MiddleLast24PriceIcon");

                    if (_ltp > AveragePrice)
                    {
                        AveragePriceUpFlag = true;
                    }
                    else if (_ltp < AveragePrice)
                    {
                        AveragePriceUpFlag = false;
                    }
                    this.NotifyPropertyChanged("AveragePriceIcon");

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
                get { return PairString + " - " + _tickTimeStamp.ToLocalTime().ToString("yyyy/MM/dd/HH:mm:ss"); }
            }

            #endregion

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
                    this.NotifyPropertyChanged("BasePriceString");

                }
            }
            public string BasePriceString
            {
                get
                {
                    return String.Format(_ltpFormstString, BasePrice);
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

            private bool _basePriceUpFlag;
            public bool BasePriceUpFlag
            {
                get
                {
                    return _basePriceUpFlag;
                }
                set
                {
                    if (_basePriceUpFlag == value)
                        return;

                    _basePriceUpFlag = value;
                    this.NotifyPropertyChanged("BasePriceUpFlag");
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
                    //this.NotifyPropertyChanged("AveragePriceIconColor");
                    this.NotifyPropertyChanged("AveragePriceString");
                }
            }
            public string AveragePriceString
            {
                get
                {
                    return String.Format(_ltpFormstString, _averagePrice); ;
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

            private bool _averagePriceUpFlag;
            public bool AveragePriceUpFlag
            {
                get
                {
                    return _averagePriceUpFlag;
                }
                set
                {
                    if (_averagePriceUpFlag == value)
                        return;

                    _averagePriceUpFlag = value;
                    this.NotifyPropertyChanged("AveragePriceUpFlag");
                }
            }

            // 過去２４時間の中央値
            public decimal MiddleLast24Price
            {
                get
                {
                    return ((_lowestIn24Price + _highestIn24Price) / 2);
                }
            }
            public string MiddleLast24PriceString
            {
                get
                {
                    return String.Format(_ltpFormstString, MiddleLast24Price); ;
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

            private bool _middleLast24PriceUpFlag;
            public bool MiddleLast24PriceUpFlag
            {
                get
                {
                    return _middleLast24PriceUpFlag;
                }
                set
                {
                    if (_middleLast24PriceUpFlag == value)
                        return;

                    _middleLast24PriceUpFlag = value;
                    this.NotifyPropertyChanged("MiddleLast24PriceUpFlag");
                }
            }
            
            // 起動後の中央値
            public decimal MiddleInitPrice
            {
                get
                {
                    return ((_lowestPrice + _highestPrice) / 2);
                }
            }
            public string MiddleInitPriceString
            {
                get
                {
                    return String.Format(_ltpFormstString, MiddleInitPrice); ;
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

            private bool _middleInitPriceUpFlag;
            public bool MiddleInitPriceUpFlag
            {
                get
                {
                    return _middleInitPriceUpFlag;
                }
                set
                {
                    if (_middleInitPriceUpFlag == value)
                        return;

                    _middleInitPriceUpFlag = value;
                    this.NotifyPropertyChanged("MiddleInitPriceUpFlag");
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
                    this.NotifyPropertyChanged("MiddleLast24Price");
                    this.NotifyPropertyChanged("MiddleLast24PriceString");
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
                    this.NotifyPropertyChanged("MiddleLast24Price");
                    this.NotifyPropertyChanged("MiddleLast24PriceString");
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

            // 損益
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

            // 取引単価
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

            // 初期価格幅
            private decimal _autoTradeDefaultHaba = 500M;
            public decimal AutoTradeDefaultHaba
            {
                get
                {
                    return _autoTradeDefaultHaba;
                }
                set
                {
                    if (_autoTradeDefaultHaba == value)
                        return;

                    _autoTradeDefaultHaba = value;
                    this.NotifyPropertyChanged("AutoTradeDefaultHaba");
                }
            }

            // 利確価格幅
            private decimal _autoTradeDefaultRikakuHaba = 500M;
            public decimal AutoTradeDefaultRikakuHaba
            {
                get
                {
                    return _autoTradeDefaultRikakuHaba;
                }
                set
                {
                    if (_autoTradeDefaultRikakuHaba == value)
                        return;

                    _autoTradeDefaultRikakuHaba = value;
                    this.NotifyPropertyChanged("AutoTradeDefaultRikakuHaba");
                }
            }

            // アッパーリミット > デフォルトが欲しい。
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

            // ローワーリミット > デフォルトが欲しい。
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
            private decimal _autoTradeLossCut = 10000M;
            public decimal AutoTradeLossCut
            {
                get
                {
                    return _autoTradeLossCut;
                }
                set
                {
                    if (_autoTradeLossCut == value)
                        return;

                    // 最低リミット（これないと、永久ループに入る）
                    if (value < 1000)
                    {
                        value = 1001;
                        return;
                    }

                    _autoTradeLossCut = value;
                    this.NotifyPropertyChanged("AutoTradeLossCut");
                }
            }

            // ロット数
            private decimal _autoTradeSlots = 15;
            public decimal AutoTradeSlots
            {
                get
                {
                    return _autoTradeSlots;
                }
                set
                {
                    if (_autoTradeSlots == value)
                        return;

                    _autoTradeSlots = value;
                    this.NotifyPropertyChanged("AutoTradeSlots");
                }
            }

            // ロット追加起点
            private decimal _autoTradeAddFrom;
            public decimal AutoTradeAddFrom
            {
                get
                {
                    return _autoTradeAddFrom;
                }
                set
                {
                    if (_autoTradeAddFrom == value)
                        return;

                    _autoTradeAddFrom = value;
                    this.NotifyPropertyChanged("AutoTradeAddFrom");
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

            #region == コレクション ==

            // TickHistoryクラス リスト
            private ObservableCollection<TickHistory> _tickHistory = new ObservableCollection<TickHistory>();
            public ObservableCollection<TickHistory> TickHistories
            {
                get { return this._tickHistory; }
            }

            // アクティブな注文一覧
            private ObservableCollection<Order> _activeOrders = new ObservableCollection<Order>();
            public ObservableCollection<Order> ActiveOrders
            {
                get { return this._activeOrders; }
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

            // 自動取引
            private ObservableCollection<AutoTrade> _autoTrades = new ObservableCollection<AutoTrade>();
            public ObservableCollection<AutoTrade> AutoTrades
            {
                get { return this._autoTrades; }
            }

            #endregion

            // コンストラクタ
            public Pair(Pairs p, double fontSize, string ltpFormstString, decimal grouping100, decimal grouping1000)
            {
                this._p = p;
                _ltpFontSize = fontSize;
                _ltpFormstString = ltpFormstString;

                _depthGrouping100 = grouping100;
                _depthGrouping1000 = grouping1000;

                BindingOperations.EnableCollectionSynchronization(this._tickHistory, new object());
                BindingOperations.EnableCollectionSynchronization(this._activeOrders, new object());
                BindingOperations.EnableCollectionSynchronization(this._trades, new object());
                BindingOperations.EnableCollectionSynchronization(this._autoTrades, new object());
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

            // 注文関係
            BuyOrderCommand = new RelayCommand(BuyOrderCommand_Execute, BuyOrderCommand_CanExecute);
            SellOrderCommand = new RelayCommand(SellOrderCommand_Execute, SellOrderCommand_CanExecute);
            CancelOrderListviewCommand = new GenericRelayCommand<object>(
                param => CancelOrderListviewCommand_Execute(param),
                param => CancelOrderListviewCommand_CanExecute());
            RemoveDoneOrderListviewCommand = new RelayCommand(RemoveDoneOrderListviewCommand_Execute, RemoveDoneOrderListviewCommand_CanExecute);

            // 特殊注文
            IfdOrderCommand = new RelayCommand(IfdOrderCommand_Execute, IfdOrderCommand_CanExecute);
            OcoOrderCommand = new RelayCommand(OcoOrderCommand_Execute, OcoOrderCommand_CanExecute);
            IfdocoOrderCommand = new RelayCommand(IfdocoOrderCommand_Execute, IfdocoOrderCommand_CanExecute);
            CancelIfdocoListviewCommand = new GenericRelayCommand<object>(
                param => CancelIfdocoListviewCommand_Execute(param),
                param => CancelIfdocoListviewCommand_CanExecute());
            RemoveDoneIfdocoListviewCommand = new RelayCommand(RemoveDoneIfdocoListviewCommand_Execute, RemoveDoneIfdocoListviewCommand_CanExecute);

            // 手動で取得系
            GetTradeHistoryListCommand = new RelayCommand(GetTradeHistoryListCommand_Execute, GetTradeHistoryListCommand_CanExecute);
            GetAssetsCommand = new RelayCommand(GetAssetsCommand_Execute, GetAssetsCommand_CanExecute);
            GetOrderListCommand = new RelayCommand(GetOrderListCommand_Execute, GetOrderListCommand_CanExecute);

            // 自動取引系
            StartAutoTradeCommand = new RelayCommand(StartAutoTradeCommand_Execute, StartAutoTradeCommand_CanExecute);
            AutoTradeAddCommand = new RelayCommand(AutoTradeAddCommand_Execute, AutoTradeAddCommand_CanExecute);
            StopAutoTradeCommand = new RelayCommand(StopAutoTradeCommand_Execute, StopAutoTradeCommand_CanExecute);

            AutoTradeCancelListviewCommand = new GenericRelayCommand<object>(
                param => AutoTradeCancelListviewCommand_Execute(param),
                param => AutoTradeCancelListviewCommand_CanExecute());

            AutoTradeDeleteItemListviewCommand = new GenericRelayCommand<object>(
                param => AutoTradeDeleteItemListviewCommand_Execute(param),
                param => AutoTradeDeleteItemListviewCommand_CanExecute());

            AutoTradeResetErrorListviewCommand = new GenericRelayCommand<object>(
                param => AutoTradeResetErrorListviewCommand_Execute(param),
                param => AutoTradeResetErrorListviewCommand_CanExecute());

            #endregion

            #region == テーマのイニシャライズ ==

            // テーマの選択コンボボックスのイニシャライズ
            _themes = new ObservableCollection<Theme>()
            {
                new Theme() { Id = 1, Name = "DefaultTheme", Label = "Dark", IconData="M17.75,4.09L15.22,6.03L16.13,9.09L13.5,7.28L10.87,9.09L11.78,6.03L9.25,4.09L12.44,4L13.5,1L14.56,4L17.75,4.09M21.25,11L19.61,12.25L20.2,14.23L18.5,13.06L16.8,14.23L17.39,12.25L15.75,11L17.81,10.95L18.5,9L19.19,10.95L21.25,11M18.97,15.95C19.8,15.87 20.69,17.05 20.16,17.8C19.84,18.25 19.5,18.67 19.08,19.07C15.17,23 8.84,23 4.94,19.07C1.03,15.17 1.03,8.83 4.94,4.93C5.34,4.53 5.76,4.17 6.21,3.85C6.96,3.32 8.14,4.21 8.06,5.04C7.79,7.9 8.75,10.87 10.95,13.06C13.14,15.26 16.1,16.22 18.97,15.95M17.33,17.97C14.5,17.81 11.7,16.64 9.53,14.5C7.36,12.31 6.2,9.5 6.04,6.68C3.23,9.82 3.34,14.64 6.35,17.66C9.37,20.67 14.19,20.78 17.33,17.97Z"},
                new Theme() { Id = 2, Name = "LightTheme", Label = "Light", IconData="M12,7A5,5 0 0,1 17,12A5,5 0 0,1 12,17A5,5 0 0,1 7,12A5,5 0 0,1 12,7M12,9A3,3 0 0,0 9,12A3,3 0 0,0 12,15A3,3 0 0,0 15,12A3,3 0 0,0 12,9M12,2L14.39,5.42C13.65,5.15 12.84,5 12,5C11.16,5 10.35,5.15 9.61,5.42L12,2M3.34,7L7.5,6.65C6.9,7.16 6.36,7.78 5.94,8.5C5.5,9.24 5.25,10 5.11,10.79L3.34,7M3.36,17L5.12,13.23C5.26,14 5.53,14.78 5.95,15.5C6.37,16.24 6.91,16.86 7.5,17.37L3.36,17M20.65,7L18.88,10.79C18.74,10 18.47,9.23 18.05,8.5C17.63,7.78 17.1,7.15 16.5,6.64L20.65,7M20.64,17L16.5,17.36C17.09,16.85 17.62,16.22 18.04,15.5C18.46,14.77 18.73,14 18.87,13.21L20.64,17M12,22L9.59,18.56C10.33,18.83 11.14,19 12,19C12.82,19 13.63,18.83 14.37,18.56L12,22Z"}
            };
            // デフォルトにセット
            _currentTheme = _themes[0];

            #endregion

            #region == APIクライアントのイニシャライズ ==

            // プライベートAPIクライアントのイニシャライズ
            _priApi = new PrivateAPIClient();

            // エラーイベントにサブスクライブ
            this._priApi.ErrorOccured += new PrivateAPIClient.ClinetErrorEvent(OnError);

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
                caX.Separator.StrokeThickness = 0.1;
                caX.Separator.StrokeDashArray = new DoubleCollection { 4 };
                caX.Separator.IsEnabled = true;
                Style styleXSec = Application.Current.FindResource("ChartSeparatorStyle") as Style;
                caX.Separator.Style = styleXSec;
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

            #region == タイマーのイニシャライズと起動 ==

            // Tickerのタイマー起動
            dispatcherTimerTickAllPairs.Tick += new EventHandler(TickerTimer);
            dispatcherTimerTickAllPairs.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimerTickAllPairs.Start();
            
            // Chart更新のタイマー
            dispatcherChartTimer.Tick += new EventHandler(ChartTimer);
            dispatcherChartTimer.Interval = new TimeSpan(0, 1, 0);
            // Start Timer later.

            // 初回RSS フィードの取得
            Task.Run(() => GetRss());

            // RSSのタイマー起動
            dispatcherTimerRss.Tick += new EventHandler(RssTimer);
            dispatcherTimerRss.Interval = new TimeSpan(0, 15, 0);
            dispatcherTimerRss.Start();

            #endregion

            // 初期値
            ActivePairIndex = 0;
            CurrentPair = Pairs.btc_jpy;
            ActivePair = PairBtcJpy;
            ActivePair.Ltp = PairBtcJpy.Ltp;

            IsBtcJpyVisible = true;
            IsXrpJpyVisible = false;
            IsEthBtcVisible = false;
            IsLtcBtcVisible = false;
            IsMonaJpyVisible = false;
            IsBchJpyVisible = false;

            IsJpyAssetVisible = true;
            IsBtcJpyAssetVisible = true;
            IsXrpJpyAssetVisible = false;
            IsEthBtcAssetVisible = false;
            IsLtcBtcAssetVisible = false;
            IsMonaJpyAssetVisible = false;
            IsBchJpyAssetVisible = false;

            ShowAllCharts = false;
            ShowMainContents = true;

        }

        #region == イベント・タイマー系 ==

        // 現在価格取得 Tickerタイマー
        private async void TickerTimer(object source, EventArgs e)
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

                                // 最安値登録
                                if (PairBtcJpy.LowestPrice == 0)
                                {
                                    PairBtcJpy.LowestPrice = tick.LTP;
                                }
                                if (tick.LTP < PairBtcJpy.LowestPrice)
                                {
                                    PairBtcJpy.LowestPrice = tick.LTP;
                                }

                                // 最高値登録
                                if (PairBtcJpy.HighestPrice == 0)
                                {
                                    PairBtcJpy.HighestPrice = tick.LTP;
                                }
                                if (tick.LTP > PairBtcJpy.HighestPrice)
                                {
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
                                        aym.TickHistoryPriceUp = true;
                                        PairBtcJpy.TickHistories.Insert(0, aym);

                                    }
                                    else if (PairBtcJpy.TickHistories[0].Price < aym.Price)
                                    {
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

                                PairBtcJpy.HighLowInfoText = "";

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
                                            PairBtcJpy.HighLowInfoText = "⇑⇑⇑　高値アラーム " + PairBtcJpy.PairString;

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
                                            PairBtcJpy.HighLowInfoText = "⇓⇓⇓　安値アラーム " + PairBtcJpy.PairString;
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
                                            PairBtcJpy.HighLowInfoText = "⇑⇑⇑　起動後最高値更新 " + PairBtcJpy.PairString;

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
                                            PairBtcJpy.HighLowInfoText = "⇓⇓⇓　起動後最安値更新 " + PairBtcJpy.PairString;

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
                                        PairBtcJpy.HighLowInfoText = "⇑⇑⇑⇑⇑⇑　過去24時間最高値更新 " + PairBtcJpy.PairString;

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
                                        PairBtcJpy.HighLowInfoText = "⇓⇓⇓⇓⇓⇓　過去24時間最安値更新 " + PairBtcJpy.PairString;

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

                                PairXrpJpy.HighLowInfoText = "";

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
                                            PairXrpJpy.HighLowInfoText = "⇑⇑⇑　高値アラーム " + PairXrpJpy.PairString;

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
                                        PairXrpJpy.AlarmPlus = (tick.LTP) + 2M;
                                    }

                                    if (PairXrpJpy.AlarmMinus > 0)
                                    {
                                        if (tick.LTP <= PairXrpJpy.AlarmMinus)
                                        {
                                            PairXrpJpy.HighLowInfoTextColorFlag = false;
                                            PairXrpJpy.HighLowInfoText = "⇓⇓⇓　安値アラーム " + PairXrpJpy.PairString;
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
                                        PairXrpJpy.AlarmMinus = (tick.LTP) - 2M;
                                    }

                                    // 起動後最高値
                                    if (tick.LTP >= PairXrpJpy.HighestPrice)
                                    {
                                        if ((PairXrpJpy.TickHistories.Count > 25) && ((PairXrpJpy.BasePrice + 0.3M) < tick.LTP))
                                        {
                                            PairXrpJpy.HighLowInfoTextColorFlag = true;
                                            PairXrpJpy.HighLowInfoText = "⇑⇑⇑　起動後最高値更新 " + PairXrpJpy.PairString;

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
                                            PairXrpJpy.HighLowInfoText = "⇓⇓⇓　起動後最安値更新 " + PairXrpJpy.PairString;

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
                                        PairXrpJpy.HighLowInfoText = "⇑⇑⇑⇑⇑⇑　過去24時間最高値更新 " + PairXrpJpy.PairString;

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
                                        PairXrpJpy.HighLowInfoText = "⇓⇓⇓⇓⇓⇓　過去24時間最安値更新 " + PairXrpJpy.PairString;

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

                                PairEthBtc.HighLowInfoText = "";

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
                                            PairEthBtc.HighLowInfoText = "⇑⇑⇑　高値アラーム " + PairEthBtc.PairString;

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
                                            PairEthBtc.HighLowInfoText = "⇓⇓⇓　安値アラーム " + PairEthBtc.PairString;
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
                                            PairEthBtc.HighLowInfoText = "⇑⇑⇑　起動後最高値更新 " + PairEthBtc.PairString;

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
                                            PairEthBtc.HighLowInfoText = "⇓⇓⇓　起動後最安値更新 " + PairEthBtc.PairString;

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
                                        PairEthBtc.HighLowInfoText = "⇑⇑⇑⇑⇑⇑　過去24時間最高値更新 " + PairEthBtc.PairString;

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
                                        PairEthBtc.HighLowInfoText = "⇓⇓⇓⇓⇓⇓　過去24時間最安値更新 " + PairEthBtc.PairString;

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

                                PairMonaJpy.HighLowInfoText = "";

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
                                            PairMonaJpy.HighLowInfoText = "⇑⇑⇑　高値アラーム " + PairMonaJpy.PairString;

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
                                            PairMonaJpy.HighLowInfoText = "⇓⇓⇓　安値アラーム " + PairMonaJpy.PairString;
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
                                            PairMonaJpy.HighLowInfoText = "⇑⇑⇑　起動後最高値更新 " + PairMonaJpy.PairString;

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
                                            PairMonaJpy.HighLowInfoText = "⇓⇓⇓　起動後最安値更新 " + PairMonaJpy.PairString;

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
                                        PairMonaJpy.HighLowInfoText = "⇑⇑⇑⇑⇑⇑　過去24時間最高値更新 " + PairMonaJpy.PairString;

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
                                        PairMonaJpy.HighLowInfoText = "⇓⇓⇓⇓⇓⇓　過去24時間最安値更新 " + PairMonaJpy.PairString;

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

                                PairLtcBtc.HighLowInfoText = "";

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
                                            PairLtcBtc.HighLowInfoText = "⇑⇑⇑　高値アラーム " + PairLtcBtc.PairString;

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
                                        PairLtcBtc.AlarmPlus = tick.LTP + 0.001M;
                                    }

                                    if (PairLtcBtc.AlarmMinus > 0)
                                    {
                                        if (tick.LTP <= PairLtcBtc.AlarmMinus)
                                        {
                                            PairLtcBtc.HighLowInfoTextColorFlag = false;
                                            PairLtcBtc.HighLowInfoText = "⇓⇓⇓　安値アラーム " + PairLtcBtc.PairString;
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
                                        PairLtcBtc.AlarmMinus = tick.LTP - 0.001M;
                                    }

                                    // 起動後最高値
                                    if (tick.LTP >= PairLtcBtc.HighestPrice)
                                    {
                                        if ((PairLtcBtc.TickHistories.Count > 25) && ((PairLtcBtc.BasePrice + 0.0001M) < tick.LTP))
                                        {
                                            PairLtcBtc.HighLowInfoTextColorFlag = true;
                                            PairLtcBtc.HighLowInfoText = "⇑⇑⇑　起動後最高値更新 " + PairLtcBtc.PairString;

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
                                            PairLtcBtc.HighLowInfoText = "⇓⇓⇓　起動後最安値更新 " + PairLtcBtc.PairString;

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
                                        PairLtcBtc.HighLowInfoText = "⇑⇑⇑⇑⇑⇑　過去24時間最高値更新 " + PairLtcBtc.PairString;

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
                                        PairLtcBtc.HighLowInfoText = "⇓⇓⇓⇓⇓⇓　過去24時間最安値更新 " + PairLtcBtc.PairString;

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

                                PairBchJpy.HighLowInfoText = "";

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
                                            PairBchJpy.HighLowInfoText = "⇑⇑⇑　高値アラーム " + PairBchJpy.PairString;

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
                                            PairBchJpy.HighLowInfoText = "⇓⇓⇓　安値アラーム " + PairBchJpy.PairString;
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
                                            PairBchJpy.HighLowInfoText = "⇑⇑⇑　起動後最高値更新 " + PairBchJpy.PairString;

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
                                            PairBchJpy.HighLowInfoText = "⇓⇓⇓　起動後最安値更新 " + PairBchJpy.PairString;

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
                                        PairBchJpy.HighLowInfoText = "⇑⇑⇑⇑⇑⇑　過去24時間最高値更新 " + PairBchJpy.PairString;

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
                                        PairBchJpy.HighLowInfoText = "⇓⇓⇓⇓⇓⇓　過去24時間最安値更新 " + PairBchJpy.PairString;

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


                            // TODO
                            // 各通貨の時価評価額の更新
                            if (pair == Pairs.btc_jpy)
                            {
                                if (AssetBTCAmount != 0)
                                {
                                    AssetBTCEstimateAmount = tick.LTP * AssetBTCAmount;
                                }
                            }
                            else if (pair == Pairs.xrp_jpy)
                            {
                                AssetXRPEstimateAmount = AssetXRPAmount * tick.LTP;
                            }
                            else if (pair == Pairs.eth_btc)
                            {
                                AssetEthEstimateAmount = (AssetEthAmount * tick.LTP) * PairBtcJpy.Ltp;
                            }
                            else if (pair == Pairs.mona_jpy)
                            {
                                AssetMonaEstimateAmount = AssetMonaAmount * tick.LTP;
                            }
                            else if (pair == Pairs.mona_btc)
                            {
                                //
                            }
                            else if (pair == Pairs.ltc_btc)
                            {
                                AssetLtcEstimateAmount= (AssetLtcAmount * tick.LTP) * PairBtcJpy.Ltp;
                            }
                            else if (pair == Pairs.bcc_btc)
                            {
                                //
                            }
                            else if (pair == Pairs.bcc_jpy)
                            {
                                AssetBchEstimateAmount = AssetBchAmount * tick.LTP;
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

        // チャートデータ更新 タイマー
        private void ChartTimer(object source, EventArgs e)
        {
            // 省エネモードならスルー。
            if (MinMode)
            {
                return;
            }

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
            var appDataFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var _appDataFolder = appDataFolder + System.IO.Path.DirectorySeparatorChar + _appDeveloper + System.IO.Path.DirectorySeparatorChar + _appName;
            // 存在していなかったら作成
            System.IO.Directory.CreateDirectory(_appDataFolder);

            #region == アプリ設定のロード  ==

            // 設定ファイルのパス
            var AppConfigFilePath = _appDataFolder + System.IO.Path.DirectorySeparatorChar + _appName + ".config";

            try
            {
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

                            // 自動取引
                            hoge = pair.Attribute("autoTradeTama");
                            if (hoge != null)
                            {
                                if (!string.IsNullOrEmpty(hoge.Value))
                                {
                                    try
                                    {
                                        PairBtcJpy.AutoTradeTama = Decimal.Parse(hoge.Value);
                                    }
                                    catch
                                    {
                                        PairBtcJpy.AutoTradeTama = 0;
                                    }
                                }
                            }
                            hoge = pair.Attribute("autoTradeDefaultHaba");
                            if (hoge != null)
                            {
                                if (!string.IsNullOrEmpty(hoge.Value))
                                {
                                    try
                                    {
                                        PairBtcJpy.AutoTradeDefaultHaba = Decimal.Parse(hoge.Value);
                                    }
                                    catch
                                    {
                                        PairBtcJpy.AutoTradeDefaultHaba = 0;
                                    }
                                }
                            }
                            hoge = pair.Attribute("autoTradeDefaultRikakuHaba");
                            if (hoge != null)
                            {
                                if (!string.IsNullOrEmpty(hoge.Value))
                                {
                                    try
                                    {
                                        PairBtcJpy.AutoTradeDefaultRikakuHaba = Decimal.Parse(hoge.Value);
                                    }
                                    catch
                                    {
                                        PairBtcJpy.AutoTradeDefaultRikakuHaba = 0;
                                    }
                                }
                            }
                            hoge = pair.Attribute("autoTradeUpperLimit");
                            if (hoge != null)
                            {
                                if (!string.IsNullOrEmpty(hoge.Value))
                                {
                                    try
                                    {
                                        PairBtcJpy.AutoTradeUpperLimit = Decimal.Parse(hoge.Value);
                                    }
                                    catch
                                    {
                                        PairBtcJpy.AutoTradeUpperLimit = 0;
                                    }
                                }
                            }
                            hoge = pair.Attribute("autoTradeLowerLimit");
                            if (hoge != null)
                            {
                                if (!string.IsNullOrEmpty(hoge.Value))
                                {
                                    try
                                    {
                                        PairBtcJpy.AutoTradeLowerLimit = Decimal.Parse(hoge.Value);
                                    }
                                    catch
                                    {
                                        PairBtcJpy.AutoTradeLowerLimit = 0;
                                    }
                                }
                            }
                            hoge = pair.Attribute("autoTradeLossCut");
                            if (hoge != null)
                            {
                                if (!string.IsNullOrEmpty(hoge.Value))
                                {
                                    try
                                    {
                                        PairBtcJpy.AutoTradeLossCut = Decimal.Parse(hoge.Value);
                                    }
                                    catch
                                    {
                                        PairBtcJpy.AutoTradeLossCut = 0;
                                    }

                                }
                            }
                            hoge = pair.Attribute("autoTradeSlots");
                            if (hoge != null)
                            {
                                if (!string.IsNullOrEmpty(hoge.Value))
                                {
                                    try
                                    {
                                        PairBtcJpy.AutoTradeSlots = Decimal.Parse(hoge.Value);
                                    }
                                    catch
                                    {
                                        PairBtcJpy.AutoTradeSlots = 0;
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
            }
            catch (System.IO.FileNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ Error  設定ファイルの保存中 - FileNotFoundException : " + AppConfigFilePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ Error  設定ファイルの保存中: " + ex + " while opening : " + AppConfigFilePath);
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

            #region == 特殊注文の保存データのロード ==

            LoadIfdocos(PairBtcJpy, _appDataFolder, PairBtcJpy.ThisPair.ToString());
            LoadIfdocos(PairXrpJpy, _appDataFolder, PairXrpJpy.ThisPair.ToString());
            LoadIfdocos(PairLtcBtc, _appDataFolder, PairLtcBtc.ThisPair.ToString());
            LoadIfdocos(PairEthBtc, _appDataFolder, PairEthBtc.ThisPair.ToString());
            LoadIfdocos(PairMonaJpy, _appDataFolder, PairMonaJpy.ThisPair.ToString());
            LoadIfdocos(PairBchJpy, _appDataFolder, PairBchJpy.ThisPair.ToString());

            #endregion

            // TODO test
            //LoadAutoTrades(PairBtcJpy, _appDataFolder, PairBtcJpy.ThisPair.ToString());
            if (PairBtcJpy.AutoTrades.Count > 0)
            {
                //StartAutoTradeCommand_Execute();
            }

            // サスペンド検知イベント
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;


            // ループ再生開始　
            StartLoop();
        }

        // 終了時の処理
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            // TODO
            // 自動取引、特殊注文で、注文中あるならキャンセルしてダイアログを表示。
            //e.Cancel = true;
            //return;

            // データ保存フォルダの取得
            var AppDataFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var _appDataFolder = AppDataFolder + System.IO.Path.DirectorySeparatorChar + _appDeveloper + System.IO.Path.DirectorySeparatorChar + _appName;

            // 存在していなかったら作成
            System.IO.Directory.CreateDirectory(_appDataFolder);

            #region == アプリ設定の保存 ==

            // 設定ファイルのパス
            var AppConfigFilePath = _appDataFolder + System.IO.Path.DirectorySeparatorChar + _appName + ".config";

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

            // 自動取引
            attrs = doc.CreateAttribute("autoTradeTama");
            attrs.Value = PairBtcJpy.AutoTradeTama.ToString();
            pairBtcJpy.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("autoTradeDefaultHaba");
            attrs.Value = PairBtcJpy.AutoTradeDefaultHaba.ToString();
            pairBtcJpy.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("autoTradeDefaultRikakuHaba");
            attrs.Value = PairBtcJpy.AutoTradeDefaultRikakuHaba.ToString();
            pairBtcJpy.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("autoTradeUpperLimit");
            attrs.Value = PairBtcJpy.AutoTradeUpperLimit.ToString();
            pairBtcJpy.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("autoTradeLowerLimit");
            attrs.Value = PairBtcJpy.AutoTradeLowerLimit.ToString();
            pairBtcJpy.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("autoTradeLossCut");
            attrs.Value = PairBtcJpy.AutoTradeLossCut.ToString();
            pairBtcJpy.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("autoTradeSlots");
            attrs.Value = PairBtcJpy.AutoTradeSlots.ToString();
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
            
            try
            {
                // 設定ファイルの保存
                doc.Save(AppConfigFilePath);
            }
            //catch (System.IO.FileNotFoundException) { }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ Error  設定ファイルの保存中: " + ex + " while opening : " + AppConfigFilePath);
            }

            #endregion

            #region == 特殊注文のデータ保存 ==

            SaveIfdocos(PairBtcJpy, _appDataFolder, PairBtcJpy.ThisPair.ToString());
            SaveIfdocos(PairXrpJpy, _appDataFolder, PairXrpJpy.ThisPair.ToString());
            SaveIfdocos(PairLtcBtc, _appDataFolder, PairLtcBtc.ThisPair.ToString());
            SaveIfdocos(PairEthBtc, _appDataFolder, PairEthBtc.ThisPair.ToString());
            SaveIfdocos(PairMonaJpy, _appDataFolder, PairMonaJpy.ThisPair.ToString());
            SaveIfdocos(PairBchJpy, _appDataFolder, PairBchJpy.ThisPair.ToString());

            #endregion

            // 売りを保存
            SaveAutoTrades(PairBtcJpy, _appDataFolder, PairBtcJpy.ThisPair.ToString());

            if (PairBtcJpy.AutoTradeStart)
            {
                //await StopAutoTrade(PairBtcJpy);
            }
        }

        // サスペンド検知
        private async void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Suspend:
                    // スリープ直前

                    // 自動取引を止めて買いをキャンセルする。
                    // TODO
                    await StopAutoTrade(PairBtcJpy);

                    break;
                case PowerModes.Resume:
                    // 復帰直後
                    break;
                case PowerModes.StatusChange:
                    // バッテリーや電源に関する通知があった
                    break;
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

        // ループ再生開始メソッド
        private void StartLoop()
        {

            // 資産情報の更新ループ
            Task.Run(() => UpdateAssets());

            // 特殊注文リストの更新ループ
            //Task.Run(() => UpdateIfdocos());
            Task.Run(() => StartLoopBackground());

            // 板情報の更新ループ
            Task.Run(() => UpdateDepth());

            // 歩み値の更新ループ
            Task.Run(() => UpdateTransactions());

            // 取引履歴のGet
            Task.Run(() => UpdateTradeHistory());

            // 注文リストの更新ループ
            Task.Run(() => UpdateOrderList());

        }

        // ActivePairに関わらず、全ての通貨ペアでループする。
        private void StartLoopBackground()
        {

            UpdateIfdocos(PairBtcJpy);

            UpdateIfdocos(PairXrpJpy);

            UpdateIfdocos(PairLtcBtc);

            UpdateIfdocos(PairEthBtc);

            UpdateIfdocos(PairMonaJpy);

            UpdateIfdocos(PairBchJpy);

            //
            //UpdateAutoTrade(PairBtcJpy);

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
                if (DepthGroupingChanged)
                {
                    //グルーピング単位が変わったので、一旦クリアする。

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

            // LTP を追加
            //Depth ddd = new Depth();
            _depth[half].DepthPrice = ActivePair.Ltp;
            //_depth[half].DepthBid = 0;
            //_depth[half].DepthAsk = 0;
            _depth[half].IsLTP = true;
            //_depth[half] = ddd;

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

                        _depth[half-1].IsAskBest = true;

                        i = half+1;

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
                                    dp.DepthPrice = (c * unit);

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
                await Task.Delay(1300);

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
            await Task.Delay(1000);

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


                        APIResultAssets = "";

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

        // 注文リスト取得
        private async Task<bool> GetOrderList()
        {

            if (OrdersApiKeyIsSet == false)
            {
                // TODO show message?
                return false;
            }

            // TODO NEED TEST
            var pair = ActivePair.ThisPair;
            var orders = ActivePair.ActiveOrders;
            var ltp = ActivePair.Ltp;

            try
            {
                Orders ords = await _priApi.GetOrderList(_getOrdersApiKey, _getOrdersSecret, pair.ToString());

                if (ords != null)
                {
                    // タブの「注文一覧（＊）」を更新
                    ActiveOrdersCount = ords.OrderList.Count;

                    // 逆順にする
                    ords.OrderList.Reverse();

                    if (Application.Current == null) return false;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            foreach (var ord in ords.OrderList)
                            {

                                var found = orders.FirstOrDefault(x => x.OrderID == ord.OrderID);
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
                                        found.Shushi = ((ltp - ord.Price));
                                    }
                                    else
                                    {
                                        // 約定済みなので
                                        found.Shushi = 0;
                                    }
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
                                        ord.Shushi = ((ltp - ord.Price));
                                    }
                                    else
                                    {
                                        // 約定済みなので
                                        ord.Shushi = 0;
                                    }

                                    // リスト追加
                                    orders.Insert(0, ord);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderList: Exception - " + ex.Message);

                        }
                    });

                    // 返ってきた注文リストに存在しない注文リスト
                    List<int> lst = new List<int>();

                    // 返ってきた注文リストに存在しない注文を抽出
                    try
                    {
                        foreach (var ors in orders)
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
                        // リストのリスト（小分けにして分割取得用）
                        List<List<int>> ListOfList = new List<List<int>>();

                        // GetOrderListByIDs 40015 数が多いとエラーになるので、小分けにして。
                        List<int> temp = new List<int>();
                        int c = 0;

                        for (int i = 0; i < lst.Count; i++)
                        {

                            temp.Add(lst[c]);

                            if (temp.Count == 5)
                            {
                                ListOfList.Add(temp);

                                temp = new List<int>();
                            }

                            if (c == lst.Count - 1)
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
                            Orders oup = await _priApi.GetOrderListByIDs(_getOrdersApiKey, _getOrdersSecret, pair.ToString(), list);
                            if (oup != null)
                            {
                                // 注文をアップデート
                                foreach (var ord in oup.OrderList)
                                {
                                    if (Application.Current == null) return false;
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        try
                                        {
                                            var found = orders.FirstOrDefault(x => x.OrderID == ord.OrderID);
                                            if (found != null)
                                            {
                                                int i = orders.IndexOf(found);
                                                if (i > -1)
                                                {
                                                    orders[i].Status = ord.Status;
                                                    orders[i].AveragePrice = ord.AveragePrice;
                                                    orders[i].OrderedAt = ord.OrderedAt;
                                                    orders[i].Type = ord.Type;
                                                    orders[i].StartAmount = ord.StartAmount;
                                                    orders[i].RemainingAmount = ord.RemainingAmount;
                                                    orders[i].ExecutedAmount = ord.ExecutedAmount;
                                                    orders[i].Price = ord.Price;
                                                    orders[i].AveragePrice = ord.AveragePrice;


                                                    // 現在値のセット
                                                    // 投資金額
                                                    if (orders[i].Type == "limit")
                                                    {
                                                        orders[i].ActualPrice = (ord.Price * ord.StartAmount);
                                                    }
                                                    else
                                                    {
                                                        orders[i].ActualPrice = (ord.AveragePrice * ord.StartAmount);
                                                    }

                                                    // 現在値との差額
                                                    if ((orders[i].Status == "UNFILLED") || (orders[i].Status == "PARTIALLY_FILLED"))
                                                    {
                                                        orders[i].Shushi = ((ltp - ord.Price));
                                                    }
                                                    else
                                                    {
                                                        // 約定済みなので
                                                        orders[i].Shushi = 0;
                                                    }

                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            System.Diagnostics.Debug.WriteLine("■■■■■ Order oup: Exception - " + ex.Message);

                                        }
                                    });

                                    await Task.Delay(400);
                                }
                            }
                        }
                    }

                    APIResultActiveOrders = "";

                    await Task.Delay(1000);
                    return true;
                }
                else
                {
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

        // 取引履歴の取得
        private async Task<bool> GetTradeHistoryList()
        {

            if (TradeHistoryApiKeyIsSet == false)
            {
                // TODO show message?
                return false;
            }

            // TODO NEED TEST
            var pair = CurrentPair;
            var trades = ActivePair.Trades;

            //System.Diagnostics.Debug.WriteLine("GetTradeHistory......");

            try
            {

                TradeHistory trd = await _priApi.GetTradeHistory(_getTradeHistoryApiKey, _getTradeHistorySecret, pair.ToString());

                if (trd != null)
                {

                    // 逆順にする
                    trd.TradeList.Reverse();

                    if (Application.Current == null) return false;
                    Application.Current.Dispatcher.Invoke(() =>
                    {

                    foreach (var tr in trd.TradeList)
                    {

                        var found = trades.FirstOrDefault(x => x.TradeID == tr.TradeID);
                        if (found == null)
                        {
                            // "btc_jpy" を "BTC/JPY"に。
                            if (GetPairs.ContainsKey(tr.Pair))
                                tr.Pair = PairStrings[GetPairs[tr.Pair]];

                            trades.Insert(0, tr);
                        }

                    }

                    });

                    _tradeHistories = trades.Count;
                    NotifyPropertyChanged(nameof(TradeHistoryTitle));

                    APIResultTradeHistory = "";

                    return true;
                }
                else
                {
                    _tradeHistories = -1;
                    NotifyPropertyChanged(nameof(TradeHistoryTitle));

                    APIResultTradeHistory = "<<取得失敗>>";

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

        // 特殊注文の更新ループ
        private async void UpdateIfdocos(Pair p)
        {
            var ifdocos = p.Ifdocos;
            var ltp = p.Ltp;

            // 未約定IDリスト
            List<int> unfilledOrderIDsList = new List<int>();
            // 要発注リスト
            List<Ifdoco> needToOrderList = new List<Ifdoco>();


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
                // アクティブな注文数カウンター
                int actOrd = 0;

                // !
                ifdocos = p.Ifdocos;
                ltp = p.Ltp;

                // リストをループして、発注が必要なのを 要発注リストに追加。
                // 未約定のを未約定IDリストに追加して、後でアップデート。
                if (Application.Current == null) break;
                Application.Current.Dispatcher.Invoke(() =>
                {

                    foreach (var ifdoco in ifdocos)
                    {
                        // IFD
                        if (ifdoco.Kind == IfdocoKinds.ifd)
                        {
                            if (ifdoco.IfdIsDone == false)
                            {
                                if (ifdoco.IfdoneIsDone == false)
                                {
                                    if (ifdoco.IfdoneStatus == "FULLY_FILLED")
                                    {
                                        // 済みフラグをセット
                                        ifdoco.IfdoneIsDone = true;
                                    }
                                    else if (ifdoco.IfdoneStatus == "CANCELED_UNFILLED" || ifdoco.IfdoneStatus == "CANCELED_PARTIALLY_FILLED")
                                    {
                                        // キャンセル済み

                                        // 済みフラグをセット

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
                                            unfilledOrderIDsList.Add(ifdoco.IfdoneOrderID);
                                        }
                                        
                                    }

                                }
                                else
                                {
                                    if (ifdoco.IfdDoIsDone == false)
                                    {

                                        if (ifdoco.IfdDoStatus == "FULLY_FILLED")
                                        {
                                            // IFD 注文全部終了タイミング(色を変える？)

                                            // 済みフラグをセット
                                            ifdoco.IfdDoIsDone = true;

                                            // IfdIsDone フラグ！
                                            ifdoco.IfdIsDone = true;

                                        }
                                        else if (ifdoco.IfdDoStatus == "CANCELED_UNFILLED" || ifdoco.IfdDoStatus == "CANCELED_PARTIALLY_FILLED")
                                        {
                                            // キャンセル済み

                                            // 済みフラグをセット(
                                            ifdoco.IfdDoIsDone = true;

                                            // IfdIsDone フラグ！
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

                                        }

                                    }
                                    else
                                    {
                                        ifdoco.IfdIsDone = true;
                                    }
                                }

                            }

                            // アクティブな注文
                            if (ifdoco.IfdDoIsDone == false)
                                actOrd = actOrd + 1;

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

                                    needToOrderList.Add(ifdoco);

                                }
                                else
                                {
                                    // どちらも未約定

                                    // アクティブな注文
                                    actOrd = actOrd + 1;

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


                                }
                            }

                        }
                        // IFDOCO
                        else if (ifdoco.Kind == IfdocoKinds.ifdoco)
                        {

                            if (ifdoco.IfdocoIsDone == false)
                            {
                                if (ifdoco.IfdoneStatus == "FULLY_FILLED")
                                {
                                    // 発注済みを更新リストに追加して後でアップデートする。
                                    // Add to ToBeUpdated List and Update Order info lator.

                                    // 済みフラグをセット
                                    ifdoco.IfdoneIsDone = true;

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

                                }

                                if (ifdoco.OcoIsDone == false)
                                {

                                    if (ifdoco.OcoOneIsDone && ifdoco.OcoOtherIsDone)
                                    {
                                        // タイミングと価格によっては両方同時に約定で返ってくる可能性がある。。

                                        // OcoIsDone フラグをセット
                                        ifdoco.OcoIsDone = true;
                                        ifdoco.IfdocoIsDone = true;
                                    }
                                    else if (ifdoco.OcoOneIsDone || ifdoco.OcoOtherIsDone)
                                    {
                                        // 片方約定

                                        // どちらかをキャンセル

                                        needToOrderList.Add(ifdoco);

                                    }
                                    else
                                    {
                                        // どちらも未約定か、または未発注

                                        // アクティブな注文
                                        //actOrd = actOrd + 1;

                                        // 未発注
                                        if ((ifdoco.OcoOneOrderID == 0) && (ifdoco.OcoOtherOrderID == 0))
                                        //if ((ifdoco.OcoOneOrderID == 0) || (ifdoco.OcoOtherOrderID == 0)) // not good
                                        {

                                            //Debug.WriteLine("□ UpdateIfdocos IFDOCO 要発注");

                                            // 要発注 
                                            needToOrderList.Add(ifdoco);


                                            // アクティブな注文
                                            //actOrd = actOrd + 1;

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


                                    }

                                }
                                else
                                {
                                    ifdoco.IfdocoIsDone = true;
                                }

                                // アクティブな注文
                                if (ifdoco.IfdocoIsDone == false)
                                    actOrd = actOrd + 1;
                            }

                        }

                    }

                });

                if (p.ThisPair == CurrentPair)
                {
                    // タブの「IFDOCO注文（＊）」を更新
                    ActiveIfdocosCount = actOrd;
                }

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
                        Orders ords = await _priApi.GetOrderListByIDs(_ifdocoTradeApiKey, _ifdocoTradeSecret, p.ThisPair.ToString(), list);//unfilledOrderIDsList);

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
                                        var found = ifdocos.FirstOrDefault((x => x.IfdoneOrderID == ord.OrderID));
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
                                        found = ifdocos.FirstOrDefault((x => x.IfdDoOrderID == ord.OrderID));
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
                                        found = ifdocos.FirstOrDefault((x => x.OcoOneOrderID == ord.OrderID));
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
                                        found = ifdocos.FirstOrDefault((x => x.OcoOtherOrderID == ord.OrderID));
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
                            // IFD
                            if (ifdoco.IfdoneIsDone)
                            {
                                //System.Diagnostics.Debug.WriteLine("■IFD IfdoneIsDone");

                                // 条件が揃ったら、IfdDoを発注
                                if ((ifdoco.IfdDoIsDone == false) && (ifdoco.IfdDoOrderID == 0) && (ifdoco.IfdoneHasError == false))
                                {
                                    // トリガー
                                    //System.Diagnostics.Debug.WriteLine("■トリガー:" + ifdoco.IfdDoTriggerPrice.ToString());

                                    bool trigger = false;

                                    if (ifdoco.IfdDoTriggerUpDown == 0)
                                    {
                                        //System.Diagnostics.Debug.WriteLine("■以上");
                                        // 以上
                                        if (ltp >= ifdoco.IfdDoTriggerPrice)
                                        {
                                            System.Diagnostics.Debug.WriteLine("□IfdDoTriggerPrice");
                                            trigger = true;
                                        }

                                    }
                                    else if (ifdoco.IfdDoTriggerUpDown == 1)
                                    {
                                        //System.Diagnostics.Debug.WriteLine("■以下");
                                        // 以下
                                        if (ltp <= ifdoco.IfdDoTriggerPrice)
                                        {
                                            System.Diagnostics.Debug.WriteLine("□IfdDoTriggerPrice");
                                            trigger = true;
                                        }
                                    }

                                    if (trigger)
                                    {
                                        OrderResult res = await _priApi.MakeOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, p.ThisPair.ToString(), ifdoco.IfdDoStartAmount, ifdoco.IfdDoPrice, ifdoco.IfdDoSide, ifdoco.IfdDoType.ToString());

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

                                            OrderResult res = await _priApi.CancelOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, p.ThisPair.ToString(), cancelId);

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

                                            OrderResult res = await _priApi.CancelOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, p.ThisPair.ToString(), cancelId);

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

                                                        //System.Diagnostics.Debug.WriteLine("■ UpdateIfdocos needToOrderList OCO One cancel 約定");

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
                                    //System.Diagnostics.Debug.WriteLine("■OCO どちらも未約定");
                                    // どちらも未約定

                                    // Oco One
                                    if (ifdoco.OcoOneOrderID == 0)
                                    {
                                        bool triggered = false;

                                        if (ifdoco.OcoOneTriggerUpDown == 0)
                                        {
                                            //System.Diagnostics.Debug.WriteLine("■Oco One 以上 " + ifdoco.OcoOneTriggerPrice.ToString());
                                            // 以上
                                            if (ltp >= ifdoco.OcoOneTriggerPrice)
                                            {
                                                System.Diagnostics.Debug.WriteLine("■Oco One 以上: Ltp = " + ltp.ToString() + " Trigger = " + ifdoco.OcoOneTriggerPrice.ToString());
                                                triggered = true;
                                            }
                                        }
                                        else if (ifdoco.OcoOneTriggerUpDown == 1)
                                        {
                                            //System.Diagnostics.Debug.WriteLine("■Oco One 以下");
                                            // 以下
                                            if (ltp <= ifdoco.OcoOneTriggerPrice)
                                            {
                                                System.Diagnostics.Debug.WriteLine("■Oco One 以下: Ltp = " + ltp.ToString() + " Trigger = " + ifdoco.OcoOneTriggerPrice.ToString());
                                                triggered = true;
                                            }
                                        }

                                        if (triggered == true)
                                        {
                                            // OcoOne
                                            OrderResult ord = await _priApi.MakeOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, p.ThisPair.ToString(), ifdoco.OcoOneStartAmount, ifdoco.OcoOnePrice, ifdoco.OcoOneSide, ifdoco.OcoOneType.ToString());

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

                                    }

                                    // Oco Other
                                    if (ifdoco.OcoOtherOrderID == 0)
                                    {
                                        bool triggered = false;

                                        if (ifdoco.OcoOtherTriggerUpDown == 0)
                                        {
                                            //System.Diagnostics.Debug.WriteLine("■ OcoOther TriggerPrice以上");
                                            // 以上
                                            if (ltp >= ifdoco.OcoOtherTriggerPrice)
                                            {
                                                System.Diagnostics.Debug.WriteLine("■Oco Other 以上: Ltp = " + ltp.ToString() + " Trigger = " + ifdoco.OcoOtherTriggerPrice.ToString());
                                                triggered = true;
                                            }
                                        }
                                        else if (ifdoco.OcoOtherTriggerUpDown == 1)
                                        {
                                            //System.Diagnostics.Debug.WriteLine("■OcoOther TriggerPrice以上以下");
                                            // 以下
                                            if (ltp <= ifdoco.OcoOtherTriggerPrice)
                                            {
                                                System.Diagnostics.Debug.WriteLine("■Oco Other 以下: Ltp = " + ltp.ToString() + " Trigger = " + ifdoco.OcoOtherTriggerPrice.ToString());
                                                triggered = true;
                                            }
                                        }

                                        if (triggered == true)
                                        {
                                            OrderResult ord2 = await _priApi.MakeOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, p.ThisPair.ToString(), ifdoco.OcoOtherStartAmount, ifdoco.OcoOtherPrice, ifdoco.OcoOtherSide, ifdoco.OcoOtherType.ToString());

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
                                                if (ltp >= ifdoco.OcoOneTriggerPrice)
                                                {
                                                    System.Diagnostics.Debug.WriteLine("■Oco One 以上: Ltp = " + ltp.ToString() + " Trigger = " + ifdoco.OcoOneTriggerPrice.ToString());
                                                    // good
                                                    isTriggered = true;
                                                }

                                            }
                                            else if (ifdoco.OcoOneTriggerUpDown == 1)
                                            {
                                                //System.Diagnostics.Debug.WriteLine("■以下");
                                                // 以下
                                                if (ltp <= ifdoco.OcoOneTriggerPrice)
                                                {
                                                    System.Diagnostics.Debug.WriteLine("■Oco One 以下: Ltp = " + ltp.ToString() + " Trigger = " + ifdoco.OcoOneTriggerPrice.ToString());
                                                    // good
                                                    isTriggered = true;
                                                }
                                            }

                                            if (ifdoco.OcoOneTriggerPrice <= 0)
                                            {
                                                isTriggered = false;
                                            }

                                            if (isTriggered)
                                            {
                                                // OcoOne
                                                OrderResult ord = await _priApi.MakeOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, p.ThisPair.ToString(), ifdoco.OcoOneStartAmount, ifdoco.OcoOnePrice, ifdoco.OcoOneSide, ifdoco.OcoOneType.ToString());

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

                                        // OCO Other 発注
                                        if ((ifdoco.OcoOtherOrderID == 0) && (ifdoco.OcoOtherHasError == false))
                                        {
                                            bool isTriggered = false;

                                            if (ifdoco.OcoOtherTriggerUpDown == 0)
                                            {
                                                //System.Diagnostics.Debug.WriteLine("■以上");
                                                // 以上
                                                if (ltp >= ifdoco.OcoOtherTriggerPrice)
                                                {
                                                    System.Diagnostics.Debug.WriteLine("■Oco Other 以上: Ltp = " + ltp.ToString() + " Trigger = " + ifdoco.OcoOtherTriggerPrice.ToString());
                                                    // good
                                                    isTriggered = true;
                                                }

                                            }
                                            else if (ifdoco.OcoOtherTriggerUpDown == 1)
                                            {
                                                //System.Diagnostics.Debug.WriteLine("■以下");
                                                // 以下
                                                if (ltp <= ifdoco.OcoOtherTriggerPrice)
                                                {
                                                    System.Diagnostics.Debug.WriteLine("■Oco Other 以下: Ltp = " + ltp.ToString() + " Trigger = " + ifdoco.OcoOtherTriggerPrice.ToString());
                                                    // good
                                                    isTriggered = true;
                                                }
                                            }

                                            if (ifdoco.OcoOtherTriggerPrice <= 0)
                                            {
                                                isTriggered = false;
                                            }

                                            if (isTriggered)
                                            {
                                                // OcoOther
                                                OrderResult ord2 = await _priApi.MakeOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, p.ThisPair.ToString(), ifdoco.OcoOtherStartAmount, ifdoco.OcoOtherPrice, ifdoco.OcoOtherSide, ifdoco.OcoOtherType.ToString());

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

                                    }
                                    else if ((ifdoco.OcoOneIsDone || ifdoco.OcoOtherIsDone) && (ifdoco.OcoIsDone == false))
                                    {
                                        // OCOの片方が約定済み ＞どちらかをキャンセル

                                        if (ifdoco.OcoOneIsDone)
                                        {
                                            // OCO other をキャンセル
                                            int cancelId = ifdoco.OcoOtherOrderID;

                                            //System.Diagnostics.Debug.WriteLine("■■■■■ UpdateIfdocos needToOrderList OCO CancelOrder .....");

                                            OrderResult res = await _priApi.CancelOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, p.ThisPair.ToString(), cancelId);

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
                                                        ifdoco.IfdocoIsDone = true;
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

                                            OrderResult res = await _priApi.CancelOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, p.ThisPair.ToString(), cancelId);

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
                                                        ifdoco.IfdocoIsDone = true;
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

        // 自動取引のループ
        private async void UpdateAutoTrade(Pair p)
        {
            // TODO TEMP
            if (ActivePair.ThisPair != Pairs.btc_jpy)
            {
                await Task.Delay(1500);
                return;
            }

            // TODO TEMP
            var pair = PairBtcJpy;
            var autoTrades = PairBtcJpy.AutoTrades;
            var ltp = PairBtcJpy.Ltp;

            // 試し買いの単位
            //decimal sentinelAmount = 0.0001M;
            // 取引単位
            decimal defaultAmount = 0.0001M;

            decimal defaultHaba = pair.AutoTradeDefaultHaba;
            decimal defaultRikaku = pair.AutoTradeDefaultRikakuHaba;

            while (pair.AutoTradeStart)
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

                // 全注文数のセット
                pair.AutoTradeActiveOrders = autoTrades.Count;
                pair.AutoTradeSellOrders = 0;
                pair.AutoTradeBuyOrders = 0;
                pair.AutoTradeErrOrders = 0;

                // 最新の値をセット
                ltp = pair.Ltp;

                // 取引単位の変更があった場合に新しい値をセット
                defaultAmount = pair.AutoTradeTama;
                defaultHaba = pair.AutoTradeDefaultHaba;
                if (defaultHaba <= 0)
                    defaultHaba = 50M;

                defaultRikaku = pair.AutoTradeDefaultRikakuHaba;
                if (defaultRikaku <= 0)
                    defaultRikaku = 100M;


                // 未約定注文の情報更新用リスト
                List<int> needUpdateIdsList = new List<int>();

                // 未約定注文のリストを作るループ。
                if (Application.Current == null) break;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var pos in autoTrades)
                    {
                        if (pos.IsCanceled)
                        {
                            continue;
                        }

                        if ((pos.BuyHasError == true) || (pos.SellHasError == true))
                        {
                            // エラー注文数をセット
                            pair.AutoTradeErrOrders = pair.AutoTradeErrOrders + 1;

                            continue;
                        }

                        if ((pos.BuyOrderId != 0) && ((pos.BuyStatus == "UNFILLED") || pos.BuyStatus == "PARTIALLY_FILLED"))
                        {
                            // 買い注文数をセット
                            pair.AutoTradeBuyOrders = pair.AutoTradeBuyOrders + 1;

                            needUpdateIdsList.Add(pos.BuyOrderId);
                        }

                        if ((pos.SellOrderId != 0) && ((pos.SellStatus == "UNFILLED") || pos.SellStatus == "PARTIALLY_FILLED"))
                        {
                            // 売り注文数をセット
                            pair.AutoTradeSellOrders = pair.AutoTradeSellOrders + 1;

                            needUpdateIdsList.Add(pos.SellOrderId);
                        }
                    }
                });

                // 未約定注文の最新状態をアップデートする処理
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
                        Orders ords = await _priApi.GetOrderListByIDs(_autoTradeApiKey, _autoTradeSecret, pair.ThisPair.ToString(), list);//needUpdateIdsList);

                        if (ords != null)
                        {
                            if (Application.Current == null) break;
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                // 更新された情報でリストをアップデート
                                foreach (var ord in ords.OrderList)
                                {
                                    try
                                    {
                                        // 買い注文
                                        var found = autoTrades.FirstOrDefault((x => x.BuyOrderId == ord.OrderID));
                                        if (found != null)
                                        {
                                            found.BuyStatus = ord.Status;

                                            found.BuyFilledPrice = ord.AveragePrice;
                                        }

                                        // 売り注文
                                        found = autoTrades.FirstOrDefault((x => x.SellOrderId == ord.OrderID));
                                        if (found != null)
                                        {
                                            found.SellStatus = ord.Status;

                                            found.SellFilledPrice = ord.AveragePrice;
                                        }

                                    }
                                    catch (Exception e)
                                    {
                                        System.Diagnostics.Debug.WriteLine("■■■■■ autoTrades GetOrderListByIDs Exception: " + e);
                                    }

                                }
                            });
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("■■■■■ autoTrades GetOrderListByIDs ords == null");
                        }
                    }

                }

                // 要売り発注リスト
                List<AutoTrade> needSellList = new List<AutoTrade>();

                // 要買い発注リスト
                List<AutoTrade> needBuyList = new List<AutoTrade>();

                // 発注しなおしが必要なリスト
                List<AutoTrade> needReorderList = new List<AutoTrade>();

                // 要発注を見つけるループ。
                if (Application.Current == null) break;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var pos in autoTrades)
                    {
                        // キャンセルはスキップ
                        if (pos.IsCanceled) continue;

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

                        // TODO HasError スキップ または自動リセット
                        if ((pos.BuyHasError == true) || (pos.SellHasError == true))
                        {
                            if (pos.BuyHasError == true)
                            {
                                if (pos.BuyErrorInfo.ErrorCode == 50010)
                                {
                                    // "ご指定の注文はキャンセルできません"
                                    // エラーをリセット
                                    //pos.BuyHasError = false;
                                    //pos.BuyErrorInfo // クリアしなくても大丈夫かな。
                                    // カウンターをリセット
                                    //pos.Counter = 0;
                                }

                                if (pos.BuyErrorInfo.ErrorCode == 20001)
                                {
                                    // "API認証に失敗しました"
                                    // エラーをリセット
                                    pos.BuyHasError = false;
                                    // カウンターをリセット
                                    //pos.Counter = 0;
                                }

                                if ((pos.BuyErrorInfo.ErrorCode == 10003) || (pos.BuyErrorInfo.ErrorCode == 70002) || (pos.BuyErrorInfo.ErrorCode == 70003) || (pos.BuyErrorInfo.ErrorCode == 70001))
                                {
                                    // "システムエラーが発生しました。サポートにお問い合わせ下さい"
                                    // エラーをリセット
                                    pos.BuyHasError = false;
                                    // カウンターをリセット
                                    //pos.Counter = 0;
                                }

                                //
                                if (pos.BuyErrorInfo.ErrorCode == 50009)
                                {
                                    // "ご指定の注文は存在しません"
                                    // エラーをリセット > 要テスト
                                    //pos.BuyHasError = false;
                                    //pos.IsDone = true;
                                }

                                if (pos.BuyErrorInfo.ErrorCode == 70010)
                                {
                                    // "ただいまシステム負荷が高まっているため、最小注文数量を一時的に引き上げています。"
                                    // リトライ待機カウンターアップ
                                    pos.AutoTradeSrvBusyRetryCounter = pos.AutoTradeSrvBusyRetryCounter + 1;

                                    // 30秒待ってからリトライ
                                    if (pos.AutoTradeSrvBusyRetryCounter > 30)
                                    {
                                        // エラーをリセット
                                        pos.AutoTradeSrvBusyRetryCounter = 0;
                                        pos.BuyHasError = false;
                                    }
                                }

                            }

                            if (pos.SellHasError == true)
                            {
                                if (pos.SellErrorInfo.ErrorCode == 20001)
                                {
                                    // "API認証に失敗しました"
                                    // エラーをリセット
                                    pos.SellHasError = false;
                                }

                                if ((pos.SellErrorInfo.ErrorCode == 10003) || (pos.SellErrorInfo.ErrorCode == 70002) || (pos.SellErrorInfo.ErrorCode == 70003) || (pos.SellErrorInfo.ErrorCode == 70001))
                                {
                                    // "システムエラーが発生しました。サポートにお問い合わせ下さい"
                                    // エラーをリセット
                                    pos.SellHasError = false;
                                }

                                if (pos.SellErrorInfo.ErrorCode == 70010)
                                {
                                    // "ただいまシステム負荷が高まっているため、最小注文数量を一時的に引き上げています。"
                                    // リトライ待機カウンターアップ
                                    pos.AutoTradeSrvBusyRetryCounter = pos.AutoTradeSrvBusyRetryCounter + 1;

                                    // 30秒待ってからリトライ
                                    if (pos.AutoTradeSrvBusyRetryCounter > 30)
                                    {
                                        // エラーをリセット
                                        pos.AutoTradeSrvBusyRetryCounter = 0;
                                        pos.SellHasError = false;
                                    }
                                }

                            }

                            continue;

                        }

                        // 買い約定済み
                        if ((pos.BuyOrderId != 0) && (pos.BuyStatus == "FULLY_FILLED"))
                        {
                            // 買いなおし
                            needBuyList.Add(pos);

                            pos.BuyIsDone = true;

                            // 売り未発注 
                            //if ((pos.SellOrderId == 0) && (pos.SellHasError == false) && (string.IsNullOrEmpty(pos.SellStatus))) > 損切再注文の場合ステータス残している為、ここではステータスを無視する。
                            if ((pos.SellOrderId == 0) && (pos.SellHasError == false))
                                {
                                // 要売り発注リストへ追加
                                needSellList.Add(pos);
                            }
                            else
                            {
                                // 売り注文も済みの場合は、済みフラグ
                                if ((pos.SellOrderId != 0) && (pos.SellStatus == "FULLY_FILLED"))
                                {
                                    // 発注し直しリストへ追加
                                    needReorderList.Add(pos);

                                    pos.SellIsDone = true;

                                    pos.IsDone = true;
                                }
                            }
                        }
                        else
                        {
                            // 買い未発注 
                            //if ((pos.BuyOrderId == 0) && (pos.BuyHasError == false) && (string.IsNullOrEmpty(pos.BuyStatus)))
                            if ((pos.BuyOrderId == 0) && (pos.BuyHasError == false))
                            {
                                needBuyList.Add(pos);
                            }
                        }
                    }

                });

                // 売り発注処理
                if (needSellList.Count > 0)
                {
                    foreach(var nsl in needSellList)
                    {

                        // 最新をチェック
                        ltp = pair.Ltp;

                        // 万一、LTPが売りより上がっていたら。
                        if (ltp > nsl.SellPrice)
                        {
                            // ltpよりちょっと上にする。
                            nsl.SellPrice = ltp;

                            // 上がり過ぎを抑える
                            if ((nsl.SellPrice - nsl.BuyPrice) >= (defaultRikaku * 3))
                            {
                                nsl.SellPrice = nsl.BuyPrice + (defaultRikaku * 3);
                            }
                        }

                        // 予想更新
                        nsl.ShushiAmount = (nsl.SellPrice * nsl.SellAmount) - (nsl.BuyPrice * nsl.BuyAmount);

                        // 売り発注
                        OrderResult ord = await _priApi.MakeOrder(_autoTradeApiKey, _autoTradeSecret, pair.ThisPair.ToString(), nsl.SellAmount, nsl.SellPrice, nsl.SellSide, "limit");

                        if (ord != null)
                        {
                            // 売り成功
                            if (ord.IsSuccess)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    nsl.SellHasError = false;

                                    nsl.SellOrderId = ord.OrderID;
                                    nsl.SellStatus = ord.Status;

                                    // 売り注文,済みの場合は、済みフラグ
                                    if ((nsl.SellOrderId != 0) && (nsl.SellStatus == "FULLY_FILLED"))
                                    {
                                        nsl.SellIsDone = true;

                                        nsl.IsDone = true;
                                    }

                                });
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    nsl.SellHasError = true;
                                    if (nsl.SellErrorInfo == null)
                                    {
                                        nsl.SellErrorInfo = new ErrorInfo();
                                    }
                                    nsl.SellErrorInfo.ErrorTitle = ord.Err.ErrorTitle;
                                    nsl.SellErrorInfo.ErrorDescription = ord.Err.ErrorDescription;
                                    nsl.SellErrorInfo.ErrorCode = ord.Err.ErrorCode;
                                });

                                System.Diagnostics.Debug.WriteLine("UpdateAutoTrade - 売り　sell MakeOrder API failed");
                            }
                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                nsl.SellHasError = true;
                                if (nsl.SellErrorInfo == null)
                                {
                                    nsl.SellErrorInfo = new ErrorInfo();
                                }
                                nsl.SellErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                                nsl.SellErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                                nsl.SellErrorInfo.ErrorCode = -1;
                            });

                            System.Diagnostics.Debug.WriteLine("UpdateAutoTrade - 売り　sell MakeOrder returened NULL");
                        }
                    }

                }

                // 買い発注処理 (リミット内で)
                if ((ltp < pair.AutoTradeUpperLimit) && (ltp > pair.AutoTradeLowerLimit))
                {
                    // 買い未発注を発注するループ
                    foreach (var position in needBuyList)
                    {
                        if ((position.BuyIsDone == false) && (position.SellIsDone == false))
                        {
                            if (ltp > position.BuyPrice)
                            {
                                // 注文発注
                                OrderResult res = await _priApi.MakeOrder(_autoTradeApiKey, _autoTradeSecret, pair.ThisPair.ToString(), position.BuyAmount, position.BuyPrice, position.BuySide, "limit");

                                if (res != null)
                                {
                                    if (res.IsSuccess)
                                    {
                                        Application.Current.Dispatcher.Invoke(() =>
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
                                        });
                                    }
                                    else
                                    {
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            position.BuyHasError = true;
                                            if (position.BuyErrorInfo == null)
                                            {
                                                position.BuyErrorInfo = new ErrorInfo();
                                            }
                                            position.BuyErrorInfo.ErrorTitle = res.Err.ErrorTitle;
                                            position.BuyErrorInfo.ErrorDescription = res.Err.ErrorDescription;
                                            position.BuyErrorInfo.ErrorCode = res.Err.ErrorCode;
                                        });

                                        System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade 完了済み注文を、新たに注文し直すループ。途中 MakeOrder API returned error code.");
                                    }
                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        position.BuyHasError = true;
                                        if (position.BuyErrorInfo == null)
                                        {
                                            position.BuyErrorInfo = new ErrorInfo();
                                        }
                                        position.BuyErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                                        position.BuyErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                                        position.BuyErrorInfo.ErrorCode = -1;
                                    });

                                    System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade 完了済み注文を、新たに注文し直すループ。途中 MakeOrder API returned null.");
                                }
                            }
                        }
                    }
                }

                // 発注し直し処理
                if (needReorderList.Count > 0)
                {
                    foreach(var ro in needReorderList)
                    {
                        // 新規
                        AutoTrade position = new AutoTrade();
                        position.BuySide = "buy";
                        position.BuyAmount = defaultAmount;//SortedList[i].BuyAmount;
                        position.SellSide = "sell";
                        position.SellAmount = defaultAmount;//SortedList[i].SellAmount;

                        // 同じ条件で、再度注文。ただし、現在値より下の場合
                        position.BuyPrice = ro.BuyPrice;

                        position.SellPrice = position.BuyPrice + defaultRikaku;

                        // 最新をチェック
                        ltp = pair.Ltp;

                        // 万一、LTPが売りより上がっていたら。
                        if (ltp > position.SellPrice)
                        {
                            // ltpよりちょっと上にする。
                            position.SellPrice = ltp;

                            // 上がり過ぎを抑える
                            if ((position.SellPrice - position.BuyPrice) >= (defaultRikaku * 3))
                            {
                                position.SellPrice = position.BuyPrice + (defaultRikaku * 3);
                            }

                        }

                        // 予想利益額
                        position.ShushiAmount = (position.SellPrice * position.SellAmount) - (position.BuyPrice * position.BuyAmount);

                        if (ltp > position.BuyPrice)
                        {
                            // 注文発注
                            OrderResult res = await _priApi.MakeOrder(_autoTradeApiKey, _autoTradeSecret, pair.ThisPair.ToString(), position.BuyAmount, position.BuyPrice, position.BuySide, "limit");

                            if (res != null)
                            {
                                if (res.IsSuccess)
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
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

                                    });
                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        position.BuyHasError = true;
                                        if (position.BuyErrorInfo == null)
                                        {
                                            position.BuyErrorInfo = new ErrorInfo();
                                        }
                                        position.BuyErrorInfo.ErrorTitle = res.Err.ErrorTitle;
                                        position.BuyErrorInfo.ErrorDescription = res.Err.ErrorDescription;
                                        position.BuyErrorInfo.ErrorCode = res.Err.ErrorCode;

                                    });

                                    System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade 完了済み注文を、新たに注文し直すループ。途中 MakeOrder API returned error code.");
                                }
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    position.BuyHasError = true;
                                    if (position.BuyErrorInfo == null)
                                    {
                                        position.BuyErrorInfo = new ErrorInfo();
                                    }
                                    position.BuyErrorInfo.ErrorTitle = "注文時にエラーが起きました。";
                                    position.BuyErrorInfo.ErrorDescription = "priApi.MakeOrder is null.";
                                    position.BuyErrorInfo.ErrorCode = -1;

                                });

                                System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade 完了済み注文を、新たに注文し直すループ。途中 MakeOrder API returned null.");

                            }

                        }

                        if (Application.Current == null) break;
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            // 新規追加
                            autoTrades.Insert(0, position);

                            // 済みフラグ（後で掃除される）
                            ro.IsDone = true;

                        });
                    }


                }

                // 削除アイテム用リスト
                List<AutoTrade> needDeleteList = new List<AutoTrade>();

                // キャンセル用リスト
                List<AutoTrade> needCancelList = new List<AutoTrade>();

                // ロスカット用リスト
                List<AutoTrade> needLossCutList = new List<AutoTrade>();

                // 掃除ループ
                int h = -1;
                if (Application.Current == null) break;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var pos in autoTrades)
                    {
                        h = h + 1;

                        if (pos.IsCanceled == true)
                        {
                            needDeleteList.Add(pos);
                        }

                        if (pos.BuyHasError == true)
                        {
                            continue;
                        }
                        else if (pos.SellHasError == true)
                        {
                            continue;
                        }

                        if (pos.IsDone == true)
                        {
                            // 損益更新
                            pair.AutoTradeProfit = pair.AutoTradeProfit + ((pos.SellPrice - pos.BuyPrice) * pos.SellAmount);

                            needDeleteList.Add(pos);
                        }

                        // 指定幅　下がったら、損切。
                        if ((pos.SellOrderId != 0) && pos.BuyIsDone && (pos.SellIsDone == false))
                        {
                            if (pos.SellStatus == "UNFILLED")
                            {
                                // ロスカットの幅を超えたらキャンセルして、再発注
                                if ((pos.SellPrice - ltp) > pair.AutoTradeLossCut)
                                {
                                    // 一瞬の下げヒゲ対策
                                    if (pos.LossCutCounter > 7)
                                    {
                                        pos.LossCutCounter = 0;

                                        needLossCutList.Add(pos);
                                    }

                                    // カウントアップ
                                    pos.LossCutCounter = pos.LossCutCounter + 1;

                                }

                            }
                        }

                    }

                });

                // 損切処理
                if (needLossCutList.Count > 0)
                {
                    foreach(var ls in needLossCutList)
                    {
                        // キャンセル発注
                        OrderResult ord = await _priApi.CancelOrder(_autoTradeApiKey, _autoTradeSecret, pair.ThisPair.ToString(), ls.SellOrderId);
                        if (ord != null)
                        {
                            if (ord.IsSuccess)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    var newSellPrice = ltp + Math.Floor(pair.AutoTradeLossCut / 2);

                                    // 新たに売り価格を設定しなおす
                                    ls.SellOrderId = 0;
                                    ls.SellStatus = "";
                                    ls.SellPrice = newSellPrice;
                                    ls.SellStatus = ord.Status;

                                    ls.ShushiAmount = (ls.SellPrice * ls.SellAmount) - (ls.BuyPrice * ls.BuyAmount);

                                });
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    ls.SellHasError = true;
                                    if (ls.SellErrorInfo == null)
                                    {
                                        ls.SellErrorInfo = new ErrorInfo();
                                    }
                                    ls.SellErrorInfo.ErrorTitle = ord.Err.ErrorTitle;
                                    ls.SellErrorInfo.ErrorDescription = ord.Err.ErrorDescription;
                                    ls.SellErrorInfo.ErrorCode = ord.Err.ErrorCode;
                                });

                                System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade 損切売りキャンセルで。MakeOrder API returned error code.");
                            }

                        }
                        else
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                ls.SellHasError = true;
                                if (ls.SellErrorInfo == null)
                                {
                                    ls.SellErrorInfo = new ErrorInfo();
                                }
                                ls.SellErrorInfo.ErrorTitle = ord.Err.ErrorTitle;
                                ls.SellErrorInfo.ErrorDescription = ord.Err.ErrorDescription;
                                ls.SellErrorInfo.ErrorCode = ord.Err.ErrorCode;
                            });

                            System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade 損切売りキャンセルで。MakeOrder API returned null.");
                        }
                    }
                }

                // キャンセルリストを処理
                if (needCancelList.Count > 0)
                {
                    foreach (var ca in needCancelList)
                    {
                        try
                        {
                            // 注文を抜く。
                            OrderResult ord = await _priApi.CancelOrder(_autoTradeApiKey, _autoTradeSecret, pair.ThisPair.ToString(), ca.BuyOrderId);
                            if (ord != null)
                            {
                                if (ord.IsSuccess)
                                {
                                    needDeleteList.Add(ca);
                                }
                                else
                                {
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        ca.BuyHasError = true;
                                        if (ca.BuyErrorInfo == null)
                                        {
                                            ca.BuyErrorInfo = new ErrorInfo();
                                        }
                                        ca.BuyErrorInfo.ErrorTitle = ord.Err.ErrorTitle;
                                        ca.BuyErrorInfo.ErrorDescription = ord.Err.ErrorDescription;
                                        ca.BuyErrorInfo.ErrorCode = ord.Err.ErrorCode;
                                    });

                                    System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade 停滞中を抜く。キャンセルで。MakeOrder API returned error code.");
                                }
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    ca.BuyHasError = true;
                                    if (ca.BuyErrorInfo == null)
                                    {
                                        ca.BuyErrorInfo = new ErrorInfo();
                                    }
                                    ca.BuyErrorInfo.ErrorTitle = ord.Err.ErrorTitle;
                                    ca.BuyErrorInfo.ErrorDescription = ord.Err.ErrorDescription;
                                    ca.BuyErrorInfo.ErrorCode = ord.Err.ErrorCode;
                                });

                                System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade 停滞中を抜く。キャンセルで。MakeOrder API returned null.");
                            }
                        }
                        catch
                        {
                            System.Diagnostics.Debug.WriteLine("■■■■■ UpdateAutoTrade 停滞中を抜く。キャンセルで。");
                        }
                    }

                }
                needCancelList.Clear();

                // 削除リストを処理
                if (Application.Current == null) break;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var aaa in needDeleteList)
                    {
                        autoTrades.Remove(aaa);
                    }
                });
                needDeleteList.Clear();

            }
        }

        #region == 注文関係のメソッド ==

        // 手動発注から発注。その他は、（priApi）から直に呼び出すこと！
        private async Task<OrderResult> ManualOrder(Pair p, decimal amount, decimal price, string side, string type)
        {
            System.Diagnostics.Debug.WriteLine("□ Order - " + p.ThisPair.ToString() + ":" + amount + ":" + price.ToString() + ":" + side + ":" + type);

            if (ManualTradeApiKeyIsSet == false)
            {
                // TODO show message?
                Debug.WriteLine("(ManualTradeApiKeyIsSet == false)");

                return null;
            }

            var orders = p.ActiveOrders;
            var ltp = p.Ltp;

            try
            {
                OrderResult ord = await _priApi.MakeOrder(_manualTradeApiKey, _manualTradeSecret, p.ThisPair.ToString(), amount, price, side, type);

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
                                var found = orders.FirstOrDefault(x => x.OrderID == ord.OrderID);
                                if (found == null)
                                {
                                    // 現在値のセット
                                    try
                                    {
                                        ord.Shushi = ((ltp - ord.Price) * ord.StartAmount);
                                        ord.ActualPrice = (ord.Price * ord.StartAmount);
                                    }
                                    catch (Exception e)
                                    {
                                        System.Diagnostics.Debug.WriteLine("■■■■■ Order 現在値のセット2: " + e);
                                    }
                                    // リスト追加
                                    orders.Insert(0, ord);
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
        private async Task<OrderResult> CancelOrder(Pair p, int orderID)
        {
            if (ManualTradeApiKeyIsSet == false)
            {
                // TODO show message?
                return null;
            }

            var orders = p.ActiveOrders;
            var ltp = p.Ltp;

            try
            {
                // キャンセル注文発注
                OrderResult ord = await _priApi.CancelOrder(_manualTradeApiKey, _manualTradeSecret, p.ThisPair.ToString(), orderID);

                if (ord != null)
                {
                    if (ord.IsSuccess)
                    {
                        /*
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            try
                            {
                                // 注文リストの中から、同一IDをキャンセル注文結果と入れ替える
                                var found = orders.FirstOrDefault(x => x.OrderID == orderID);
                                int i = orders.IndexOf(found);
                                if (i > -1)
                                {
                                    orders[i] = ord;
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
                        */

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
            //ChartLoadingInfo = "チャートデータを取得中....";
            
            // 今日の日付セット。UTCで。
            DateTime dtToday = DateTime.Now.ToUniversalTime();

            try
            {
                // データは、ローカルタイムで、朝9:00 から翌8:59分まで。8:59分までしか取れないので、 9:00過ぎていたら 最新のデータとるには日付を１日追加する

                #region == OhlcvsOneHour 1hour毎のデータ ==

                List<Ohlcv> ListOhlcvsOneHour = new List<Ohlcv>();

                if (ct == CandleTypes.OneHour)
                {
                    //Debug.WriteLine("今日の1hour取得開始 " + pair.ToString());

                    // 一時間のロウソク足タイプなら今日、昨日、一昨日、その前の１週間分の1hourデータを取得する必要あり。
                    ListOhlcvsOneHour = await GetCandlestick(pair, CandleTypes.OneHour, dtToday);
                    if (ListOhlcvsOneHour != null)
                    {
                        // 逆順にする
                        ListOhlcvsOneHour.Reverse();

                        //Debug.WriteLine("昨日の1hour取得開始 " + pair.ToString());
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

                            //Debug.WriteLine("一昨日の1hour取得開始 " + pair.ToString());
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

                                //Debug.WriteLine("３日前の1hour取得開始 " + pair.ToString());
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


                                    //Debug.WriteLine("４日前の1hour取得開始 " + pair.ToString());
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
                    //Debug.WriteLine("今日の1min取得開始 " + pair.ToString());

                    // 一分毎のロウソク足タイプなら今日と昨日の1minデータを取得する必要あり。
                    ListOhlcvsOneMin = await GetCandlestick(pair, CandleTypes.OneMin, dtToday);
                    if (ListOhlcvsOneMin != null)
                    {
                        // 逆順にする
                        ListOhlcvsOneMin.Reverse();
                    }
                    else
                    {
                        ListOhlcvsOneMin = new List<Ohlcv>();
                        Debug.WriteLine("■■■■■ " + pair.ToString() + " GetCandlesticks error: 今日の1min取得 null");
                    }

                    // 00:00:00から23:59:59分までしか取れないので、 3時間分取るには、00:00:00から3:00までは 最新のデータとるには日付を１日マイナスする
                    if (dtToday.Hour <= 2)  // 3時間欲しい場合 2am までは昨日の分も。
                    {
                        //Debug.WriteLine("昨日の1min取得開始");

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
                        //Debug.WriteLine("昨日の1min取得スキップ " + dtToday.Hour.ToString());
                    }

                }

                #endregion

                //await Task.Delay(200);

                #region == OhlcvsOneDay 1day毎のデータ ==

                List<Ohlcv> ListOhlcvsOneDay = new List<Ohlcv>();

                if (ct == CandleTypes.OneDay)
                {
                    // 1日のロウソク足タイプなら今年、去年、２年前、３年前、４年前、５年前の1hourデータを取得する必要あり。(５年前は止めた)

                    //Debug.WriteLine("今年のOneDay取得開始 " + pair.ToString());

                    ListOhlcvsOneDay = await GetCandlestick(pair, CandleTypes.OneDay, dtToday);
                    if (ListOhlcvsOneDay != null)
                    {
                        // 逆順にする
                        ListOhlcvsOneDay.Reverse();

                        // 
                        //if (dtToday.Month <= 3)
                        //{
                        //Debug.WriteLine("去年のOneDay取得開始 " + pair.ToString());

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
            //Debug.WriteLine("LoadChart... " + pair.ToString());

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
                        span = (60 * 3) + 1;
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
                        span = 24 + 1;
                    }
                    else if (_chartSpan == ChartSpans.ThreeDay)
                    {
                        span = (24 * 3) + 1;
                    }
                    else if (_chartSpan == ChartSpans.OneWeek)
                    {
                        span = (24 * 7) + 1;
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
                        span = 30 + 1;//.44
                    }
                    else if (_chartSpan == ChartSpans.TwoMonth)
                    {
                        span = (30 * 2) + 1;
                    }
                    else if (_chartSpan == ChartSpans.OneYear)
                    {
                        span = 365 + 1;//.2425
                    }
                    else if (_chartSpan == ChartSpans.FiveYear)
                    {
                        span = (365 * 5) + 1;
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
                    Debug.WriteLine("チャートのデータロード中？ " + pair.ToString() + " " + lst.Count.ToString() + " " + span.ToString());
                    return;
                }

                Debug.WriteLine("チャートのロード中  " + pair.ToString() + " " + lst.Count.ToString() + " " + span.ToString());

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

                    }, System.Windows.Threading.DispatcherPriority.Background);
                    
                }
                catch (Exception ex)
                {
                    ChartLoadingInfo = "チャートのロード中にエラーが発生しました 3";

                    Debug.WriteLine("■■■■■ Chart loading error: " + ex.ToString());
                }

                Debug.WriteLine("チャートのロード終わり  " + pair.ToString() + " " + lst.Count.ToString() + " " + span.ToString());
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
            //ChartLoadingInfo = "チャートデータの更新中....";

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

            await Task.Delay(600);
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

        // テーマをセットするメソッド
        private void SetCurrentTheme(string themeName)
        {
            Theme test = _themes.FirstOrDefault(x => x.Name == themeName);
            if (test != null)
            {
                CurrentTheme = test;
            }
        }

        // 特殊注文のデータ保存メソッド
        private void SaveIfdocos(Pair p, string appDataFolder, string fileName)
        {
            //BtcJpy
            var IFDOCOs_FilePath = appDataFolder + System.IO.Path.DirectorySeparatorChar + fileName + "_IFDOCOs.csv";

            if (p.Ifdocos.Count > 0)
            {
                var csv = new StringBuilder();

                foreach (var ifdoco in p.Ifdocos)
                {
                    bool test = false;

                    // 未約定のみ保存する
                    if (ifdoco.Kind == IfdocoKinds.ifd)
                    {
                        if (ifdoco.IfdIsDone == false)
                        {
                            test = true;
                        }
                    }
                    else if (ifdoco.Kind == IfdocoKinds.oco)
                    {
                        if (ifdoco.OcoIsDone == false)
                        {
                            test = true;
                        }
                    }
                    else if (ifdoco.Kind == IfdocoKinds.ifdoco)
                    {
                        if (ifdoco.IfdocoIsDone == false)
                        {
                            test = true;
                        }
                    }

                    if (test)
                    {
                        var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22}", ifdoco.Kind.ToString(), ifdoco.IfdoneOrderID.ToString(), ifdoco.IfdDoOrderID.ToString(), ifdoco.IfdDoSide, ifdoco.IfdDoType.ToString(), ifdoco.IfdDoStartAmount.ToString(), ifdoco.IfdDoPrice.ToString(), ifdoco.OcoOneOrderID.ToString(), ifdoco.OcoOneSide, ifdoco.OcoOneType.ToString(), ifdoco.OcoOneStartAmount, ifdoco.OcoOnePrice, ifdoco.OcoOtherOrderID.ToString(), ifdoco.OcoOtherSide, ifdoco.OcoOtherType.ToString(), ifdoco.OcoOtherStartAmount.ToString(), ifdoco.OcoOtherPrice.ToString(), ifdoco.IfdDoTriggerPrice.ToString(), ifdoco.OcoOneTriggerPrice.ToString(), ifdoco.OcoOtherTriggerPrice.ToString(), ifdoco.IfdDoTriggerUpDown.ToString(), ifdoco.OcoOneTriggerUpDown.ToString(), ifdoco.OcoOtherTriggerUpDown.ToString());
                        csv.AppendLine(newLine);
                    }

                }

                File.WriteAllText(IFDOCOs_FilePath, csv.ToString());

            }
            else
            {
                // リストが空なので、ファイルも削除。
                if (File.Exists(IFDOCOs_FilePath))
                {
                    File.Delete(IFDOCOs_FilePath);
                }
            }
        }

        // 特殊注文のデータをロードするメソッド
        private void LoadIfdocos(Pair p, string appDataFolder, string fileName)
        {
            var IFDOCOs_FilePath = appDataFolder + System.IO.Path.DirectorySeparatorChar + fileName + "_IFDOCOs.csv";

            if (File.Exists(IFDOCOs_FilePath))
            {
                try
                {
                    var contents = File.ReadAllText(IFDOCOs_FilePath).Split('\n');
                    var csv = from line in contents select line.Split(',').ToArray();

                    foreach (var row in csv)
                    {
                        if (string.IsNullOrEmpty(row[0]))
                        {
                            break;
                        }

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

                        asdf.IfdDoTriggerUpDown = int.Parse(row[20]);
                        asdf.OcoOneTriggerUpDown = int.Parse(row[21]);
                        asdf.OcoOtherTriggerUpDown = int.Parse(row[22]);

                        // リストへ追加
                        p.Ifdocos.Add(asdf);

                    }
                }
                catch (System.IO.FileNotFoundException) { }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ Error  特殊注文の保存データロード中: " + ex + " while opening : " + IFDOCOs_FilePath);
                }
            }

        }

        // 自動取引のデータ保存メソッド
        private void SaveAutoTrades(Pair p, string appDataFolder, string fileName)
        {
            //BtcJpy
            var AutoTrades_FilePath = appDataFolder + System.IO.Path.DirectorySeparatorChar + fileName + "_AutoTrades.csv";

            if (p.AutoTrades.Count > 0)
            {
                var csv = new StringBuilder();

                foreach (var at in p.AutoTrades)
                {

                    // 未約定のみ保存する
                    if (at.IsDone == false)
                    {
                        if ((at.SellIsDone == false) && (at.SellOrderId != 0))
                        {
                            // side, 注文数、価格、約定価格、ステータス、想定損益
                            //var newLine = string.Format("{0},{1}", at.SellOrderId.ToString());
                            var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}",
                                at.BuyOrderId.ToString(), at.BuySide, at.BuyAmount.ToString(), at.BuyPrice, at.BuyFilledPrice.ToString(), at.BuyStatus,
                                at.SellOrderId.ToString(), at.SellSide, at.SellAmount.ToString(), at.SellPrice, at.SellFilledPrice.ToString(), at.SellStatus,
                                at.ShushiAmount.ToString()
                                );
                            csv.AppendLine(newLine);
                        }
                    }

                }

                File.WriteAllText(AutoTrades_FilePath, csv.ToString());

            }
            else
            {
                // リストが空なので、ファイルも削除。
                if (File.Exists(AutoTrades_FilePath))
                {
                    File.Delete(AutoTrades_FilePath);
                }
            }
        }

        // 自動取引のデータをロードするメソッド
        private void LoadAutoTrades(Pair p, string appDataFolder, string fileName)
        {
            var AutoTrades_FilePath = appDataFolder + System.IO.Path.DirectorySeparatorChar + fileName + "_AutoTrades.csv";

            if (File.Exists(AutoTrades_FilePath))
            {
                try
                {
                    var contents = File.ReadAllText(AutoTrades_FilePath).Split('\n');
                    var csv = from line in contents select line.Split(',').ToArray();

                    foreach (var row in csv)
                    {
                        if (string.IsNullOrEmpty(row[0]))
                        {
                            break;
                        }

                        AutoTrade asdf = new AutoTrade();

                        asdf.BuyOrderId = Int32.Parse(row[0]);
                        asdf.BuySide = row[1];
                        asdf.BuyAmount = Decimal.Parse(row[2]);
                        asdf.BuyPrice = Decimal.Parse(row[3]);
                        asdf.BuyFilledPrice = Decimal.Parse(row[4]);
                        asdf.BuyStatus = row[5];
                        if (asdf.BuyStatus == "FULLY_FILLED")
                        {
                            asdf.BuyIsDone = true;
                        }

                        asdf.SellOrderId = Int32.Parse(row[6]);
                        asdf.SellSide = row[7];
                        asdf.SellAmount = Decimal.Parse(row[8]);
                        asdf.SellPrice = Decimal.Parse(row[9]);
                        asdf.SellFilledPrice = Decimal.Parse(row[10]);
                        asdf.SellStatus = row[11];

                        asdf.ShushiAmount = Decimal.Parse(row[12]);

                        p.AutoTrades.Add(asdf);

                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ Error  自動取引データロード中: " + ex + " while opening : " + AutoTrades_FilePath);
                }
            }

        }

        // 自動取引を止めて、買いをキャンセルするメソッド
        private async Task<bool> StopAutoTrade(Pair p)
        {
            System.Diagnostics.Debug.WriteLine("Stop Auto Trading.");

            if (AutoTradeApiKeyIsSet == false) return true;

            var pair = p;
            var ltp = p.Ltp;
            var autoTrades = p.AutoTrades;

            if (pair.AutoTradeStart == false)
                return true;

            // 更新ループを止める。
            pair.AutoTradeStart = false;

            if (autoTrades.Count > 0)
            {
                // 買い注文をすべてキャンセルする。

                List<int> needCancelIdsList = new List<int>();
                List<AutoTrade> needDeleteList = new List<AutoTrade>();

                foreach (var position in autoTrades)
                {
                    // 注文中だったらリスト追加
                    if (position.BuyStatus == "UNFILLED" || position.BuyStatus == "PARTIALLY_FILLED")
                    {
                        if (position.BuyOrderId > 0)
                        {
                            needCancelIdsList.Add(position.BuyOrderId);
                            needDeleteList.Add(position);
                        }
                    }

                }

                System.Diagnostics.Debug.WriteLine("Cancelling Buy orders....");

                if (needCancelIdsList.Count > 0)
                {

                    // リストのリスト（小分けにして分割取得用）
                    List<List<int>> ListOfList = new List<List<int>>();

                    // GetOrderListByIDs 40015 数が多いとエラーになるので、小分けにして。
                    List<int> temp = new List<int>();
                    int c = 0;

                    for (int i = 0; i < needCancelIdsList.Count; i++)
                    {
                        temp.Add(needCancelIdsList[c]);

                        if (temp.Count == 5)
                        {
                            ListOfList.Add(temp);

                            temp = new List<int>();
                        }

                        if (c == needCancelIdsList.Count - 1)
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

                        // CancelOrders
                        Orders ord = await _priApi.CancelOrders(_manualTradeApiKey, _manualTradeSecret, pair.ThisPair.ToString(), list);

                        if (ord != null)
                        {
                            if (ord.OrderList.Count > 0)
                            {

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    for (int i = 0; i < needDeleteList.Count; i++)
                                    {
                                        autoTrades.Remove(needDeleteList[i]);
                                    }

                                });

                            }
                        }
                    }


                }

            }

            // 情報表示のリセット
            pair.AutoTradeActiveOrders = 0;
            pair.AutoTradeSellOrders = 0;
            pair.AutoTradeBuyOrders = 0;
            pair.AutoTradeErrOrders = 0;

            // タブの「自動取引（On）」を更新
            this.NotifyPropertyChanged("AutoTradeTitle");

            return true;

        }

        #endregion

        #region == コマンド ==

        #region == 認証・設定画面表示関係コマンド ==

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

        #region == APIキー関係コマンド ==

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

        #region == その他コマンド ==

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

        #region == 注文系コマンド ==

        // 買い注文コマンド
        public ICommand BuyOrderCommand { get; }
        public bool BuyOrderCommand_CanExecute()
        {
            if (PublicApiOnlyMode == true) return false;

            return true;

        }
        public async void BuyOrderCommand_Execute()
        {

            if (BuyAmount <= 0)
            {
                APIResultBuyCommandErrorString = "数量が不正です。";
                APIResultBuyCommandResult = "";
                return;
            }
            if (_buyType == OrderTypes.limit)
            {
                if (BuyPrice <= 0)
                {
                    APIResultBuyCommandErrorString = "価格が不正です。";
                    APIResultBuyCommandResult = "";
                    return;
                }
            }

            var pair = ActivePair;

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

            OrderResult result = await ManualOrder(pair, BuyAmount, BuyPrice, "buy", TypeStr);

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
                        APIResultBuyCommandErrorString = "「" + result.Err.ErrorDescription + "」";
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

        }
        public async void SellOrderCommand_Execute()
        {
            if (SellAmount <= 0)
            {
                APIResultSellCommandErrorString = "数量が不正です。";
                APIResultSellCommandResult = "";
                return;
            }
            if (_sellType == OrderTypes.limit)
            {
                if (SellPrice <= 0)
                {
                    APIResultSellCommandErrorString = "価格が不正です。";
                    APIResultSellCommandResult = "";
                    return;
                }
            }

            var pair = ActivePair;

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

            OrderResult result = await ManualOrder(pair, _sellAmount, _sellPrice, "sell", TypeStr);

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
                        APIResultSellCommandErrorString = "「"+result.Err.ErrorDescription+"」";
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
            var pair = ActivePair;
            var orders = pair.ActiveOrders;

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

                //System.Diagnostics.Debug.WriteLine("CancelOrderListviewCommand_Execute...: " + ord.OrderID.ToString());

                OrderResult result = await CancelOrder(pair, ord.OrderID);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (result != null)
                    {
                        if (result.IsSuccess)
                        {
                            try
                            {
                                // 注文リストの中から、同一IDをキャンセル注文結果と入れ替える
                                var found = orders.FirstOrDefault(x => x.OrderID == ord.OrderID);
                                int i = orders.IndexOf(found);
                                if (i > -1)
                                {
                                    orders[i] = result as Order;
                                }

                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrderListviewCommand_Execute: Exception - " + ex.Message);

                            }
                        }
                        else
                        {
                            if (result.HasErrorInfo)
                            {
                                try
                                {
                                    
                                    var found = orders.FirstOrDefault(x => x.OrderID == ord.OrderID);
                                    int i = orders.IndexOf(found);
                                    if (i > -1)
                                    {

                                        if (result.HasErrorInfo)
                                        {
                                            orders[i].HasErrorInfo = true;
                                            orders[i].Err.ErrorTitle = result.Err.ErrorTitle;
                                            orders[i].Err.ErrorDescription = result.Err.ErrorDescription;
                                            orders[i].Err.ErrorCode = result.Err.ErrorCode;

                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrderListviewCommand_Execute: Exception - " + ex.Message);

                                }

                            }

                        }

                    }

                });

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
            var activeOrders = ActivePair.ActiveOrders;

            int c = activeOrders.Count - 1;

            for (int i = c; i >= 0; i--)
            {
                if ((activeOrders[i].Status == "FULLY_FILLED") || (activeOrders[i].Status == "CANCELED_UNFILLED") || (activeOrders[i].Status == "CANCELED_PARTIALLY_FILLED"))
                {
                    activeOrders.Remove(activeOrders[i]);
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
        }
        public async void IfdOrderCommand_Execute()
        {
            //System.Diagnostics.Debug.WriteLine("IfdOrderCommand...");

            var pair = ActivePair;
            var ifdocos = ActivePair.Ifdocos;

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
            OrderResult ord = await _priApi.MakeOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, pair.ThisPair.ToString(), ifdoco.IfdoneStartAmount, ifdoco.IfdonePrice, ifdoco.IfdoneSide, ifdoco.IfdoneType.ToString());

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
                ifdocos.Insert(0, ifdoco);
            });

        }

        // OCO 注文
        public ICommand OcoOrderCommand { get; }
        public bool OcoOrderCommand_CanExecute()
        {
            if (PublicApiOnlyMode == true) return false;

            return true;
        }
        public void OcoOrderCommand_Execute()
        {
            //System.Diagnostics.Debug.WriteLine("OcoOrderCommand...");

            var pair = ActivePair;
            var ifdocos = ActivePair.Ifdocos;

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

            OcoOrderCommandErrorString = "";

            // リストビューに追加
            Application.Current.Dispatcher.Invoke(() =>
            {
                ifdocos.Insert(0, ifdoco);

                OcoOrderCommandResult = "成功。";
            });

        }

        // IFDOCO注文
        public ICommand IfdocoOrderCommand { get; }
        public bool IfdocoOrderCommand_CanExecute()
        {
            if (PublicApiOnlyMode == true) return false;

            return true;
        }
        public async void IfdocoOrderCommand_Execute()
        {
            //System.Diagnostics.Debug.WriteLine("IfdocoOrderCommand...");

            var pair = ActivePair;
            var ifdocos = ActivePair.Ifdocos;

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
            OrderResult ord = await _priApi.MakeOrder(_ifdocoTradeApiKey, _ifdocoTradeSecret, pair.ThisPair.ToString(), ifdoco.IfdoneStartAmount, ifdoco.IfdonePrice, ifdoco.IfdoneSide, ifdoco.IfdoneType.ToString());

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
                ifdocos.Insert(0, ifdoco);
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

            var pair = ActivePair;

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
                    Orders oup = await _priApi.CancelOrders(_ifdocoTradeApiKey, _ifdocoTradeSecret, pair.ThisPair.ToString(), cancelIdList);

                    // 後は update loopに任せる

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

            var ifdocos = ActivePair.Ifdocos;

            int c = ifdocos.Count - 1;

            for (int i = c; i >= 0; i--)
            {
                if (ifdocos[i].Kind == IfdocoKinds.ifd)
                {
                    if (ifdocos[i].IfdIsDone)
                    {
                        ifdocos.Remove(ifdocos[i]);
                    }
                }
                else if (ifdocos[i].Kind == IfdocoKinds.oco)
                {
                    if (ifdocos[i].OcoIsDone)
                    {
                        ifdocos.Remove(ifdocos[i]);
                    }
                }
                else if (ifdocos[i].Kind == IfdocoKinds.ifdoco)
                {
                    if (ifdocos[i].IfdocoIsDone)
                    {
                        ifdocos.Remove(ifdocos[i]);
                    }
                }

            }

        }

        #endregion

        #region == 自動取引のコマンド ==

        // 自動取引開始コマンド
        public ICommand StartAutoTradeCommand { get; }
        public bool StartAutoTradeCommand_CanExecute()
        {
            if (PublicApiOnlyMode)
                return false;
            else
                return true;
        }
        public void StartAutoTradeCommand_Execute()
        {
            if (AutoTradeApiKeyIsSet == false) return;

            System.Diagnostics.Debug.WriteLine("Start Auto Trading...");

            // TEMP
            if (ActivePair.ThisPair != Pairs.btc_jpy)
                return;

            var pair = ActivePair;
            var ltp = ActivePair.Ltp;
            var autoTrades = ActivePair.AutoTrades;

            // 情報表示数のリセット。
            pair.AutoTradeActiveOrders = 0;
            pair.AutoTradeSellOrders = 0;
            pair.AutoTradeBuyOrders = 0;
            pair.AutoTradeErrOrders = 0;

            // 損益表示リセット
            pair.AutoTradeProfit = 0;
            
            // 上値制限セット
            if (pair.AutoTradeUpperLimit == 0)
                if (pair.HighestIn24Price != 0)
                    pair.AutoTradeUpperLimit = pair.HighestIn24Price - 2000M;

            // 下値制限セット
            if (pair.AutoTradeLowerLimit == 0)
                if (pair.LowestIn24Price != 0)
                    pair.AutoTradeLowerLimit = pair.LowestIn24Price - 5000M;



            // 注文数のセット
            pair.AutoTradeActiveOrders = autoTrades.Count;

            // 開始フラグセット
            pair.AutoTradeStart = true;
            // タブの「自動取引（On）」を更新
            this.NotifyPropertyChanged("AutoTradeTitle");

            // 自動取引ループの開始
            // TODO 他の通貨
            UpdateAutoTrade(pair);

        }

        public ICommand AutoTradeAddCommand { get; }
        public bool AutoTradeAddCommand_CanExecute()
        {
            if (PublicApiOnlyMode)
                return false;

            if (ActivePair.AutoTradeStart == false)
                return false;
            else
                return true;
        }
        public async void AutoTradeAddCommand_Execute()
        {
            if (AutoTradeApiKeyIsSet == false) return;
            if (ManualTradeApiKeyIsSet == false) return;


            // TEMP
            if (ActivePair.ThisPair != Pairs.btc_jpy)
                return;

            var pair = ActivePair;
            var ltp = ActivePair.Ltp;
            var autoTrades = ActivePair.AutoTrades;
            //


            if (pair.AutoTradeStart == false)
                return;

            if (pair.AutoTradeSlots == 0)
                return;

            if (pair.AutoTradeSlots <= pair.AutoTrades.Count - 1)
                return;

            // 追加するロット数
            var newSlots = pair.AutoTradeSlots - pair.AutoTrades.Count;

                // 上値制限セット
            if (pair.AutoTradeUpperLimit == 0)
                pair.AutoTradeUpperLimit = pair.HighestIn24Price - 2000M;

            // 下値制限セット
            if (pair.AutoTradeLowerLimit == 0)
                pair.AutoTradeLowerLimit = pair.LowestIn24Price - 5000M;

            // 幅
            if (pair.AutoTradeDefaultHaba <= 0)
                pair.AutoTradeDefaultHaba = 50;
            // 幅
            if (pair.AutoTradeDefaultHaba <= 0)
                pair.AutoTradeDefaultRikakuHaba = 500;

            decimal haba = pair.AutoTradeDefaultHaba;
            decimal rikaku = pair.AutoTradeDefaultRikakuHaba;

            // ベース価格
            decimal basePrice = pair.AutoTradeAddFrom;

            if (basePrice <= 0)
            {
                basePrice = ltp;
            }

            // 追加
            for (int i = 0; i < newSlots; i++)
            {

                AutoTrade position = new AutoTrade();

                position.BuySide = "buy";
                position.BuyAmount = pair.AutoTradeTama;
                position.SellSide = "sell";
                position.SellAmount = pair.AutoTradeTama;

                position.BuyPrice = basePrice - (haba * i);
                position.SellPrice = position.BuyPrice + rikaku;

                if (position.BuyPrice < pair.AutoTradeLowerLimit)
                {
                    break;
                }

                position.ShushiAmount = (position.SellPrice * position.SellAmount) - (position.BuyPrice * position.BuyAmount);

                if (ltp > position.BuyPrice)
                {

                    // 注文発注
                    OrderResult res = await _priApi.MakeOrder(_manualTradeApiKey, _manualTradeSecret, pair.ThisPair.ToString(), position.BuyAmount, position.BuyPrice, position.BuySide, "limit");

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

                }

                if (Application.Current == null) { return; }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // リストヘ追加
                    autoTrades.Add(position);

                });

            }

            // 注文数のセット
            pair.AutoTradeActiveOrders = autoTrades.Count;

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

            await StopAutoTrade(ActivePair);
            
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

            var pair = ActivePair;

            // 選択注文アイテム保持用
            List<AutoTrade> selectedList = new List<AutoTrade>();
            // キャンセルする注文IDを保持
            List<int> needCancelIdsList = new List<int>();

            // System.Windows.Controls.SelectedItemCollection をキャストして、ループ
            System.Collections.IList items = (System.Collections.IList)obj;
            var collection = items.Cast<AutoTrade>();

            foreach (var item in collection)
            {
                // アイテム追加
                selectedList.Add(item as AutoTrade);
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
                    needCancelIdsList.Add(item.BuyOrderId);
                }

                if ((item.SellStatus == "UNFILLED") || (item.SellStatus == "PARTIALLY_FILLED"))
                {
                    //cancel
                    needCancelIdsList.Add(item.SellOrderId);
                }

            }

            if (needCancelIdsList.Count > 0)
            {

                // リストのリスト（小分けにして分割取得用）
                List<List<int>> ListOfList = new List<List<int>>();

                // GetOrderListByIDs 40015 数が多いとエラーになるので、小分けにして。
                List<int> temp = new List<int>();
                int c = 0;

                for (int i = 0; i < needCancelIdsList.Count; i++)
                {
                    temp.Add(needCancelIdsList[c]);

                    if (temp.Count == 5)
                    {
                        ListOfList.Add(temp);

                        temp = new List<int>();
                    }

                    if (c == needCancelIdsList.Count - 1)
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

                    // CancelOrders
                    Orders ord = await _priApi.CancelOrders(_manualTradeApiKey, _manualTradeSecret, pair.ThisPair.ToString(), list);

                    // 後はループに任せる。
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
            List<AutoTrade> selectedList = new List<AutoTrade>();
            // キャンセルする注文IDを保持
            List<int> cancelIdList = new List<int>();

            // System.Windows.Controls.SelectedItemCollection をキャストして、ループ
            System.Collections.IList items = (System.Collections.IList)obj;
            var collection = items.Cast<AutoTrade>();

            foreach (var item in collection)
            {
                // アイテム追加
                selectedList.Add(item as AutoTrade);
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
        public ICommand AutoTradeDeleteItemListviewCommand { get; }
        public bool AutoTradeDeleteItemListviewCommand_CanExecute()
        {
            return true;
        }
        public void AutoTradeDeleteItemListviewCommand_Execute(object obj)
        {
            //Debug.WriteLine("AutoTradeDeleteItemListviewCommand_Execute");

            if (obj == null) return;

            var autoTrades = ActivePair.AutoTrades;

            // 選択注文アイテム保持用
            List<AutoTrade> selectedList = new List<AutoTrade>();
            // キャンセルする注文IDを保持
            List<int> cancelIdList = new List<int>();

            // System.Windows.Controls.SelectedItemCollection をキャストして、ループ
            System.Collections.IList items = (System.Collections.IList)obj;
            var collection = items.Cast<AutoTrade>();

            foreach (var item in collection)
            {
                // 削除リストに追加
                selectedList.Add(item as AutoTrade);
            }

            // 念のため、UIスレッドで。
            if (Application.Current == null) { return; }
            Application.Current.Dispatcher.Invoke(() =>
            {
                // 選択注文アイテムをループして、アイテムを削除する
                foreach (var item in selectedList)
                {
                    autoTrades.Remove(item);
                }
            });


        }

        #endregion

        #endregion

    }

}
