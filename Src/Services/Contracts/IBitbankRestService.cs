using BitDesk.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BitDesk.Services.Contracts
{
    internal interface IBitbankRestService
    {
        event EventHandler<ErrorInfo>? ErrorOccured;
        event EventHandler<ApiClientError>? ApiErrorOccured;

        Task<CandlestickResult?> GetCandlestick(string pair, string CandleType, string YYYYMMDDD);
        Task<List<Ohlcv>?> GetLatestCandlesticksAsync(PairCodes pair, CandleTypes ct);
        Task<TransactionsResult?> GetTransactionsAsync(string pair);
        Task<DepthResult?> GetDepthAsync(string pair);

        Task<OrderResult> CancelOrderAsync(string apiKey, string apiSecret, string pair, ulong orderID);
        Task<OrderListResult?> CancelOrdersAsync(string apiKey, string apiSecret, string pair, List<ulong> orderIDs);
        Task<List<Asset>?> GetAssetListAsync(string apiKey, string apiSecret);
        Task<OrderResult?> GetOrderByIdAsync(string apiKey, string apiSecret, string pair, ulong orderID);
        Task<OrderListResult?> GetOrderListAsync(string apiKey, string apiSecret, string pair);
        Task<OrderListResult?> GetOrderListByIdsAsync(string apiKey, string apiSecret, string pair, List<ulong> orderIDs);
        Task<TradeHistory?> GetTradeHistoryAsync(string apiKey, string apiSecret, string pair);
        Task<OrderResult> MakeOrderAsync(string apiKey, string apiSecret, string pair, decimal amount, decimal price, string side, string type, bool postOnly);
    }
}