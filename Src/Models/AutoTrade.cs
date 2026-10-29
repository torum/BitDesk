using BitDesk.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDesk.Models
{
    public class AutoTrade : ObservableObject
    {
        private int _index;
        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                if (_index == value)
                    return;

                _index = value;
                this.OnPropertyChanged(nameof(Index));
            }
        }

        /*
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
                this.OnPropertyChanged(nameof(AutoTradeSrvBusyRetryCounter));
            }
        }
        */

        /*
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
                this.OnPropertyChanged(nameof(LossCutCounter));
            }
        }
        */

        // 買い
        private string _buySide = "";
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
                this.OnPropertyChanged(nameof(BuySide));
                this.OnPropertyChanged(nameof(BuySideText));
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
        private string _sellSide ="";
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
                this.OnPropertyChanged(nameof(SellSide));
                this.OnPropertyChanged(nameof(SellSideText));
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
                this.OnPropertyChanged(nameof(BuyAmount));
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
                this.OnPropertyChanged(nameof(SellAmount));
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
                this.OnPropertyChanged(nameof(BuyPrice));
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
                this.OnPropertyChanged(nameof(SellPrice));
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
                this.OnPropertyChanged(nameof(BuyFilledPrice));
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
                this.OnPropertyChanged(nameof(SellFilledPrice));
            }
        }

        // 買い注文ID
        private ulong _buyOrderId;
        public ulong BuyOrderId
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
                this.OnPropertyChanged(nameof(BuyOrderId));
            }
        }

        // 売り注文ID
        private ulong _sellOrderId;
        public ulong SellOrderId
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
                this.OnPropertyChanged(nameof(SellOrderId));
            }
        }

        // 買い注文ステータス
        private string _buyStatus = "";
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
                this.OnPropertyChanged(nameof(BuyStatus));
                this.OnPropertyChanged(nameof(BuyStatusText));
            }
        }

        // 売り注文ステータス
        private string _sellStatus = "";
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
                this.OnPropertyChanged(nameof(SellStatus));
                this.OnPropertyChanged(nameof(SellStatusText));
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
                this.OnPropertyChanged(nameof(ProfitAmount));
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
                this.OnPropertyChanged(nameof(ShushiAmount));
            }
        }

        private decimal _estimateAmount;
        public decimal EstimateAmount
        {
            get
            {
                return _estimateAmount;
            }
            set
            {
                if (_estimateAmount == value)
                    return;

                _estimateAmount = value;
                this.OnPropertyChanged(nameof(EstimateAmount));
            }
        }

        /*
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
                this.OnPropertyChanged(nameof(IsCanceled));
            }
        }
        */

        /*
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
                this.OnPropertyChanged(nameof(BuyIsDone));
            }
        }
        */

        /*
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
                this.OnPropertyChanged(nameof(SellIsDone));
            }
        }
        */

        /*
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
                this.OnPropertyChanged(nameof(IsDone));
            }
        }
        */

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
                this.OnPropertyChanged(nameof(BuyHasError));
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
                this.OnPropertyChanged(nameof(BuyErrorInfo));
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
                this.OnPropertyChanged(nameof(SellHasError));
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
                this.OnPropertyChanged(nameof(SellErrorInfo));
            }
        }

        // ワンタイム売りのみ
        private bool _oneTimeSell;
        public bool OneTimeSell
        {
            get
            {
                return _oneTimeSell;
            }
            set
            {
                if (_oneTimeSell == value)
                    return;

                _oneTimeSell = value;
                this.OnPropertyChanged(nameof(OneTimeSell));
            }
        }

        // ワンタイム買いのみ
        private bool _oneTimeBuy;
        public bool OneTimeBuy
        {
            get
            {
                return _oneTimeBuy;
            }
            set
            {
                if (_oneTimeBuy == value)
                    return;

                _oneTimeBuy = value;
                this.OnPropertyChanged(nameof(OneTimeBuy));
            }
        }

        public AutoTrade()
        {
            _sellErrorInfo = new ErrorInfo(ErrType.ApiError);
            _buyErrorInfo = new ErrorInfo(ErrType.ApiError);
        }
    }

}
