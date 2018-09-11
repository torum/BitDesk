using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitbank.Models
{
    // /user/spot/order
    public class JsonOrderData
    {
        public int order_id { get; set; }
        public string pair { get; set; }
        public string side { get; set; }
        public string type { get; set; }
        public string start_amount { get; set; }
        public string remaining_amount { get; set; }
        public string executed_amount { get; set; }
        public string price { get; set; }
        public string average_price { get; set; }
        public long ordered_at { get; set; }
        public string status { get; set; }
    }

    public class JsonOrderObject
    {
        public int success { get; set; }
        public JsonOrderData data { get; set; }
    }

    // /user/spot/active_orders
    public class OrderInfoData
    {
        public List<JsonOrderData> orders { get; set; }
    }

    public class JsonOrderInfoObject
    {
        public int success { get; set; }
        public OrderInfoData data { get; set; }
    }

}
