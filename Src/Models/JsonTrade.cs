using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BitDesk.Models;

[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true
)]
[JsonSerializable(typeof(List<JsonTrade>))]
[JsonSerializable(typeof(JsonTrade))]
[JsonSerializable(typeof(TradeHistoryData))]
[JsonSerializable(typeof(JsonTradeHistoryObject))]
internal partial class TradeHistoryJsonSerializerContext : JsonSerializerContext
{
}

public class JsonTrade
{
    [JsonPropertyName("trade_id")]
    public ulong Trade_id { get; set; }

    [JsonPropertyName("pair")]
    public string? Pair { get; set; }

    [JsonPropertyName("order_id")]
    public ulong Order_id { get; set; }

    [JsonPropertyName("side")]
    public string? Side { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("amount")]
    public string? Amount { get; set; }

    [JsonPropertyName("price")]
    public string? Price { get; set; }

    [JsonPropertyName("maker_taker")]
    public string? Maker_taker { get; set; }

    //public string? Fee_amount_base { get; set; }
    [JsonPropertyName("fee_amount_quote")]
    public string? Fee_amount_quote { get; set; }

    [JsonPropertyName("fee_occurred_amount_quote")]
    public string? Fee_occurred_amount_quote{get; set; }

    [JsonPropertyName("executed_at")]
    public long Executed_at { get; set; }
}

public class TradeHistoryData
{
    public List<JsonTrade>? Trades { get; set; }
}

public class JsonTradeHistoryObject
{
    [JsonPropertyName("success")]
    public int Success { get; set; }

    [JsonPropertyName("data")]
    public TradeHistoryData? Data { get; set; }
}
