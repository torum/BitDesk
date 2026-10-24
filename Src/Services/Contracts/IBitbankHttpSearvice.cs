using BitDesk.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BitDesk.Services.Contracts
{
    internal interface IBitbankHttpSearvice
    {
        Task<CandlestickResult?> GetCandlestick(string pair, string CandleType, string YYYYMMDDD);

        Task<List<Ohlcv>?> GetLatestCandlesticksAsync(PairCodes pair, CandleTypes ct);
    }
}