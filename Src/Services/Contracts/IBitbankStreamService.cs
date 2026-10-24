using BitDesk.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BitDesk.Services.Contracts
{
    internal interface IBitbankStreamService
    {
        event EventHandler<BitbankTickerData>? TickerUpdated;

        void Initialize(List<PairCodes> pairCodes);

        Task StartAsync();
        Task StopAsync();

        bool IsConnected { get; }

        Task LeaveTickerRoomAsync(PairCodes pairCode);
        Task JoinTickerRoomAsync(PairCodes pairCode);
        Task LeaveDepthRoomAsync(PairCodes pairCode);
        Task JoinDepthRoomAsync(PairCodes pairCode);
    }
}