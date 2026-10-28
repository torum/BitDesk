using BitDesk.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BitDesk.Services.Contracts
{
    internal interface IBitbankHttpService
    {
        event EventHandler<ErrorInfo>? ErrorOccured;
        event EventHandler<ApiClientError>? ApiErrorOccured;
        Task<CandlestickResult?> GetCandlestick(string pair, string CandleType, string YYYYMMDDD);

        Task<List<Ohlcv>?> GetLatestCandlesticksAsync(PairCodes pair, CandleTypes ct);
    }
}