using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDesk.Models
{
    public class Trade
    {
        public int trade_id { get; set; }
        public string pair { get; set; }
        public int order_id { get; set; }
        public string side { get; set; }
        public string type { get; set; }
        public string amount { get; set; }
        public string price { get; set; }
        public string maker_taker { get; set; }
        public string fee_amount_base { get; set; }
        public string fee_amount_quote { get; set; }
        public long executed_at { get; set; }
    }

    public class TradeHistoryData
    {
        public List<Trade> trades { get; set; }
    }

    public class JsonTradeHistoryObject
    {
        public int success { get; set; }
        public TradeHistoryData data { get; set; }
    }
}
