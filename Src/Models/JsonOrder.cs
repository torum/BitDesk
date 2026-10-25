using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BitDesk.Models;

[JsonSerializable(typeof(JsonOrderData))]
[JsonSerializable(typeof(JsonOrderObject))]
[JsonSerializable(typeof(List<JsonOrderData>))]
[JsonSerializable(typeof(OrderInfoData))]
[JsonSerializable(typeof(JsonOrderInfoObject))]

[JsonSerializable(typeof(List<ulong>))]
[JsonSerializable(typeof(OrderParam))]
[JsonSerializable(typeof(PairOrderIdParam))]
[JsonSerializable(typeof(PairOrderIdList))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
public partial class OrderJsonSerializerContext : JsonSerializerContext
{
}

// /user/spot/order

public class JsonOrderData
{
    [JsonPropertyName("order_id")]
    public ulong Order_id { get; set; }

    [JsonPropertyName("pair")]
    public string? Pair { get; set;}

    [JsonPropertyName("side")]
    public string? Side { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("start_amount")]
    public string? Start_amount { get; set; }

    [JsonPropertyName("remaining_amount")]
    public string? Remaining_amount { get; set; }

    [JsonPropertyName("executed_amount")]
    public string? Executed_amount { get; set; }

    [JsonPropertyName("price")]
    public string? Price { get; set; }

    [JsonPropertyName("average_price")]
    public string? Average_price { get; set; }

    [JsonPropertyName("ordered_at")]
    public long Ordered_at { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }
}

public class JsonOrderObject
{

    [JsonPropertyName("success")]
    public int Success { get; set; }

    [JsonPropertyName("data")]
    public JsonOrderData? Data { get; set; }
}

// /user/spot/active_orders
public class OrderInfoData
{
    public List<JsonOrderData>? Orders { get; set; }
}

public class JsonOrderInfoObject
{
    [JsonPropertyName("success")]
    public int Success { get; set; }

    [JsonPropertyName("data")]
    public OrderInfoData? Data { get; set; }
}

// 発注時クエリパラメーター用クラス Jsonシリアライズ用
public class OrderParam(string pair, string amount, string price, string side, string type, bool postOnly)
{
    [JsonPropertyName("pair")]
    public string Pair
    {
        get; set;
    } = pair;

    [JsonPropertyName("amount")]
    public string Amount
    {
        get; set;
    } = amount;

    [JsonPropertyName("price")]
    public string Price
    {
        get; set;
    } = price;

    [JsonPropertyName("side")]
    public string Side
    {
        get; set;
    } = side;

    [JsonPropertyName("type")]
    public string Type
    {
        get; set;
    } = type;

    [JsonPropertyName("post_only")]
    public bool Post_Only // Post Only (true can be specified only if type = limit. default false)
    {
        get; set;
    } = postOnly;
}

// パラメーター用クラス Jsonシリアライズ用
public class PairOrderIdParam(string pair, ulong orderID)
{
    [JsonPropertyName("pair")]
    public string Pair
    {
        get; set;
    } = pair;

    [JsonPropertyName("order_id")]
    public ulong Order_id
    {
        get; set;
    } = orderID;
}

// パラメーター用クラス Jsonシリアライズ用
public class PairOrderIdList(string pair, List<ulong> orderIds)
{
    [JsonPropertyName("pair")]
    public string Pair
    {
        get; set;
    } = pair;

    [JsonPropertyName("order_ids")]
    public List<ulong> OrderIds
    {
        get; set;
    } = orderIds;
}

