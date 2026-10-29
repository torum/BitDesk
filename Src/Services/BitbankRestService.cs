using BitDesk.Models;
using BitDesk.Services.Contracts;
using FluentAvalonia.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BitDesk.Services;

internal class BitbankRestService : IBitbankRestService
{
    private readonly HttpClient? _publicClient;
    private readonly HttpClient? _privateClient;

    private static readonly Uri _publicApiEndpoint = new("https://public.bitbank.cc");

    private readonly Uri _privateApiEndpoint = new("https://api.bitbank.cc/v1");

    private readonly IHttpClientFactory _httpClientFactory;

    public event EventHandler<ErrorInfo>? ErrorOccured;
    public event EventHandler<ApiClientError>? ApiErrorOccured;

    public BitbankRestService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _publicClient = _httpClientFactory.CreateClient("BitDesk");
        _publicClient.BaseAddress = _publicApiEndpoint;
        _privateClient = _httpClientFactory.CreateClient("PrivateBitDesk");
        _privateClient.BaseAddress = _privateApiEndpoint;
    }

    // Public API
    public async Task<CandlestickResult?> GetCandlestick(string pair, string CandleType, string YYYYMMDDD)
    {
        if (_publicClient is null)
        {
            return null;
        }

        if (_publicClient.BaseAddress is null)
        {
            return null;
        }

        Uri _endpoint = new(_publicClient.BaseAddress.ToString() + pair + "/candlestick/" + CandleType + "/" + YYYYMMDDD);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = _endpoint,
        };

        try
        {
            var response = await _publicClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine("GetCandlestick: HTTP Error" + response.StatusCode.ToString() + " URL: " + _endpoint.ToString());

                //TODO
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();
            //System.Diagnostics.Debug.WriteLine("GetCandlestick: " + json);

            //var deserialized = JsonConvert.DeserializeObject<JsonCandlestick>(json);
            var deserialized = JsonSerializer.Deserialize(json, CandlestickJsonSerializerContext.Default.JsonCandlestick);

            if (deserialized == null)
            {
                System.Diagnostics.Debug.WriteLine("GetCandlestick: API error. deserialized is null.");
                //TODO
                return null;
            }
            if (!(deserialized.Success > 0))
            {
                //var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);
                var jsonResult = JsonSerializer.Deserialize(json, ErrorJsonSerializerContext.Default.JsonErrorObject);
                System.Diagnostics.Debug.WriteLine("GetCandlestick: API error code - " + jsonResult?.Data?.Code.ToString());

                // TODO
                return null;
            }
            if (deserialized.Data is null)
            {
                System.Diagnostics.Debug.WriteLine("GetCandlestick: API error. Data is null.");

                // TODO
                return null;
            }
            if (deserialized.Data.Candlestick is null)
            {

                System.Diagnostics.Debug.WriteLine("GetCandlestick: API error. deserialized.Data.Candlestick is null.");

                // TODO
                return null;
            }
            CandlestickResult csr = new();

            if (deserialized.Data.Candlestick.Count > 0)
            {
                //Candlestick cs = new Candlestick();

                if (deserialized.Data.Candlestick[0].Type == "1min")
                {
                    csr.CandleType = CandleTypes.OneMin;
                }
                else if (deserialized.Data.Candlestick[0].Type == "5min")
                {
                    csr.CandleType = CandleTypes.FiveMin;
                }
                else if (deserialized.Data.Candlestick[0].Type == "15min")
                {
                    csr.CandleType = CandleTypes.FifteenMin;
                }
                else if (deserialized.Data.Candlestick[0].Type == "30min")
                {
                    csr.CandleType = CandleTypes.ThirtyMin;
                }
                else if (deserialized.Data.Candlestick[0].Type == "1hour")
                {
                    csr.CandleType = CandleTypes.OneHour;
                }
                else if (deserialized.Data.Candlestick[0].Type == "4hour")
                {
                    csr.CandleType = CandleTypes.FourHour;
                }
                else if (deserialized.Data.Candlestick[0].Type == "8hour")
                {
                    csr.CandleType = CandleTypes.EightHour;
                }
                else if (deserialized.Data.Candlestick[0].Type == "12hour")
                {
                    csr.CandleType = CandleTypes.TwelveHour;
                }
                else if (deserialized.Data.Candlestick[0].Type == "1day")
                {
                    csr.CandleType = CandleTypes.OneDay;
                }
                else if (deserialized.Data.Candlestick[0].Type == "1week")
                {
                    csr.CandleType = CandleTypes.OneWeek;
                }
                else if (deserialized.Data.Candlestick[0].Type == "1month")
                {
                    csr.CandleType = CandleTypes.OneMonth;
                }

                if (deserialized?.Data?.Candlestick != null)
                {
                    foreach (var candlestick in deserialized.Data.Candlestick)
                    {
                        if (candlestick.Ohlcv != null)
                        {
                            foreach (var ohlcv in candlestick.Ohlcv)
                            {
                                //Debug.WriteLine($"  Open: {ohlcv.Open}, High: {ohlcv.High}, Low: {ohlcv.Low}, Close: {ohlcv.Close}, Volume: {ohlcv.Volume}, Timestamp: {ohlcv.Timestamp}");

                                var oh = new Ohlcv();
                                if (decimal.TryParse(ohlcv.Open, out var decimalOpen))
                                {
                                    oh.Open = decimalOpen;
                                }
                                if (decimal.TryParse(ohlcv.High, out var decimalHigh))
                                {
                                    oh.High = decimalHigh;
                                }
                                if (decimal.TryParse(ohlcv.Low, out var decimalLow))
                                {
                                    oh.Low = decimalLow;
                                }
                                if (decimal.TryParse(ohlcv.Close, out var decimalClose))
                                {
                                    oh.Close = decimalClose;
                                }
                                if (decimal.TryParse(ohlcv.Volume, out var decimalVolume))
                                {
                                    oh.Volume = decimalVolume;
                                }
                                oh.TimeStamp = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((double)ohlcv.Timestamp).ToLocalTime();

                                csr.Candlesticks.Add(oh);
                            }
                        }
                    }
                }
            }

            csr.ErrorCode = 0;
            csr.IsSuccess = true;

            return csr;
        }
        catch (HttpRequestException ex)
        {
            if (ex.InnerException is not null)
            {
                System.Diagnostics.Debug.WriteLine("GetCandlestick: HttpRequestException - " + ex.Message + " + InnerException: " + ex.InnerException.Message);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("GetCandlestick: HttpRequestException - " + ex.Message);
            }

            var errorInfo = new ErrorInfo(ErrType.ConnectionError)
            {
                ErrorMessage = $"{ex.Message} @GetCandlestickAsync.",
                ErrorSeverity = FAInfoBarSeverity.Error
            };

            ErrorOccured?.Invoke(this, errorInfo);

            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("GetCandlestick: Exception - " + ex.Message);
            var errorInfo = new ErrorInfo(ErrType.Unexpected)
            {
                ErrorMessage = $"{ex.Message} ({ex.GetType().FullName}) @GetCandlestickAsync.",
                ErrorSeverity = FAInfoBarSeverity.Error
            };
            ErrorOccured?.Invoke(this, errorInfo);
            return null;
        }

    }

    // Public API
    public Task<List<Ohlcv>?> GetLatestCandlesticksAsync(PairCodes pair, CandleTypes ct)
    {
        string pairString = pair.ToString().ToLower();
        string candleTypeString = ct switch
        {
            CandleTypes.OneMin => "1min",
            CandleTypes.FiveMin => "5min",
            CandleTypes.FifteenMin => "15min",
            CandleTypes.ThirtyMin => "30min",
            CandleTypes.OneHour => "1hour",
            CandleTypes.FourHour => "4hour",
            CandleTypes.EightHour => "8hour",
            CandleTypes.TwelveHour => "12hour",
            CandleTypes.OneDay => "1day",
            CandleTypes.OneWeek => "1week",
            CandleTypes.OneMonth => "1month",
            _ => throw new ArgumentOutOfRangeException(nameof(ct), ct, null)
        };
        string yyyymmdd = DateTime.UtcNow.ToString("yyyyMMdd");
        return GetCandlestick(pairString, candleTypeString, yyyymmdd).ContinueWith(task =>
        {
            if (task.Result != null && task.Result.IsSuccess)
            {
                return task.Result.Candlesticks;
            }
            else
            {
                return null;
            }
        });
    }

    // Public API トランザクション(歩み値)取得メソッド
    public async Task<TransactionsResult?> GetTransactionsAsync(string pair)
    {
        if (_publicClient is null)
        {
            return null;
        }

        if (_publicClient.BaseAddress is null)
        {
            return null;
        }

        Uri _endpoint = new((_publicClient.BaseAddress).ToString() + pair + "/transactions");

        //System.Diagnostics.Debug.WriteLine("GettingDepth..." + _endpoint.ToString());

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = _endpoint,
        };

        try
        {
            var response = await _publicClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                //System.Diagnostics.Debug.WriteLine("GetTicker: " + s);

                //var deserialized = JsonConvert.DeserializeObject<JsonTransactions>(json);
                var deserialized = JsonSerializer.Deserialize(json, TransactionsJsonSerializerContext.Default.JsonTransactions);

                if (deserialized?.Success > 0)
                {
                    if (deserialized.Data?.Transactions is not null)
                    {
                        TransactionsResult trs = new();

                        foreach (var tr in deserialized.Data.Transactions)
                        {
                            Transaction dd = new()
                            {
                                TransactionId = tr.TransactionId
                            };
                            if (tr.Side is not null)
                            {
                                dd.Side = tr.Side;
                            }
                            if (!string.IsNullOrEmpty(tr.Price))
                            {
                                dd.Price = decimal.Parse(tr.Price);
                            }

                            if (!string.IsNullOrEmpty(tr.Amount))
                            {
                                dd.Amount = decimal.Parse(tr.Amount);
                            }

                            dd.ExecutedAt = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(tr.ExecutedAt).ToLocalTime();

                            trs.Trans.Add(dd);

                            //Debug.WriteLine(tr.ExecutedAt);
                        }

                        trs.ErrorCode = 0;
                        trs.IsSuccess = true;

                        return trs;
                    }
                    else
                    {
                        Debug.WriteLine("GetTransactions: API error: deserialized.Data?.Transactions is null.");

                        // TODO
                        return null;
                    }
                }
                else
                {
                    //var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);
                    var jsonResult = JsonSerializer.Deserialize(json, ErrorJsonSerializerContext.Default.JsonErrorObject);

                    if (jsonResult?.Data is not null)
                    {
                        Debug.WriteLine("GetTransactions: API error code - " + jsonResult.Data.Code.ToString());
                    }
                    else
                    {
                        Debug.WriteLine("GetTransactions: API error. No error code given.");
                    }

                    // TODO
                    return null;
                }
            }
            else
            {
                Debug.WriteLine("GetTransactions: HTTP Error " + response.StatusCode.ToString());

                //TODO
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            if (ex.InnerException is not null)
            {
                Debug.WriteLine("GetTransactions: HttpRequestException - " + ex.Message + " + InnerException: " + ex.InnerException.Message);
            }
            else
            {
                Debug.WriteLine("GetTransactions: HttpRequestException - " + ex.Message);
            }

            //TODO
            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("GetTransactions: Exception - " + ex.Message);
            return null;
        }
    }

    // Public API 板情報取得メソッド
    public async Task<DepthResult?> GetDepthAsync(string pair)
    {
        if (_publicClient is null)
        {
            return null;
        }

        if (_publicClient.BaseAddress is null)
        {
            return null;
        }

        Uri _endpoint = new(_publicClient.BaseAddress.ToString() + pair + "/depth");

        //System.Diagnostics.Debug.WriteLine("GettingDepth..." + _endpoint.ToString());

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = _endpoint,
        };

        try
        {
            var response = await _publicClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                //System.Diagnostics.Debug.WriteLine("GetTicker: " + s);

                //var deserialized = JsonConvert.DeserializeObject<JsonDepthObject>(json);
                var deserialized = JsonSerializer.Deserialize(json, DepthJsonSerializerContext.Default.JsonDepthObject);

                if (deserialized == null)
                {
                    return null;
                }

                if (deserialized.Success > 0)
                {
                    if (deserialized.Data is not null)
                    {
                        DepthResult dpr = new();

                        if (deserialized.Data.Bids is not null)
                        {
                            foreach (var dp in deserialized.Data.Bids)
                            {
                                if (dp.Count > 1)
                                {
                                    Depth dd = new()
                                    {
                                        DepthPrice = decimal.Parse(dp[0]),
                                        DepthBid = decimal.Parse(dp[1]),
                                        DepthAsk = 0
                                    };
                                    dpr.DepthBidList.Add(dd);
                                }
                            }
                        }

                        if (deserialized.Data.Asks is not null)
                        {
                            foreach (var dp in deserialized.Data.Asks)
                            {
                                if (dp.Count > 1)
                                {
                                    Depth dd = new()
                                    {
                                        DepthPrice = decimal.Parse(dp[0]),
                                        DepthBid = 0,
                                        DepthAsk = decimal.Parse(dp[1])
                                    };
                                    dpr.DepthAskList.Add(dd);
                                }
                            }
                        }

                        dpr.ErrorCode = 0;
                        dpr.IsSuccess = true;

                        return dpr;
                    }
                    else
                    {
                        Debug.WriteLine("GetDepth: API error. Data is null.");
                        return null;
                    }
                }
                else
                {
                    //var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);
                    var jsonResult = JsonSerializer.Deserialize(json, ErrorJsonSerializerContext.Default.JsonErrorObject);

                    if (jsonResult?.Data is not null)
                    {
                        Debug.WriteLine("GetDepth: API error code - " + jsonResult.Data.Code.ToString());
                    }
                    else
                    {
                        Debug.WriteLine("GetDepth: API error. (No error code given.)");
                    }
                    return null;
                }
            }
            else
            {
                Debug.WriteLine("GetDepth: HTTP Error " + response.StatusCode.ToString());
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            if (ex.InnerException is not null)
            {
                Debug.WriteLine("GetDepth: HttpRequestException - " + ex.Message + " + InnerException: " + ex.InnerException.Message);
            }
            else
            {
                Debug.WriteLine("GetDepth: HttpRequestException - " + ex.Message);
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("GetDepth: Exception - " + ex.Message);
            return null;
        }
    }

    // 資産残高取得メソッド
    public async Task<List<Asset>?> GetAssetListAsync(string apiKey, string apiSecret)
    {
        var path = new Uri("/user/assets", UriKind.Relative);

        var resbo = await PrivateSendAsync(path, apiKey, apiSecret, HttpMethod.Get);
        if (resbo == null)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetAssetList: Send returned NULL.");
            return null;
        }
        var json = resbo.BodyText;

        if (!string.IsNullOrEmpty(json))
        {
            //var deserialized = JsonAssetResult.FromJson(json);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<JsonAssetResult>(json, AssetJsonSerializerContext.Default.JsonAssetResult);

            if (deserialized?.Success > 0)
            {
                if (deserialized.Data?.Assets != null)
                {
                    List<Asset> asts = [];

                    foreach (var ast in deserialized.Data.Assets)
                    {
                        asts.Add(new Asset
                        {
                            Name = ast.AssetAsset ?? string.Empty,
                            Amount = decimal.Parse(ast.OnhandAmount ?? string.Empty),
                            FreeAmount = decimal.Parse(ast.FreeAmount ?? string.Empty)
                        });

                    }

                    return asts;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                //var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);
                var jsonResult = System.Text.Json.JsonSerializer.Deserialize(json, ErrorJsonSerializerContext.Default.JsonErrorObject);

                System.Diagnostics.Debug.WriteLine("■■■■■ GetAssetList: API error code - " + jsonResult?.Data?.Code.ToString() + " ■■■■■");

                // エラーイベント発火
                var er = new ApiClientError
                {
                    ErrType = "API",
                    ErrCode = jsonResult?.Data?.Code ?? 0
                };
                if (ApiErrorCodesDictionary.ContainsKey(jsonResult?.Data?.Code ?? 0))
                {
                    er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult?.Data?.Code ?? 0] + "」";
                }
                er.ErrDatetime = DateTime.Now;
                er.ErrPlace = path.ToString();

                ApiErrorOccured?.Invoke(this, er);

                return null;
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetAssetList: Send returned NULL.");

            // HTTPエラー情報を返しても使わないので、Nullを返す。
            //ord.Err.ErrorCode = -1;
            //ord.Err.ErrorType = ErrType.ResposeParseError;
            //ord.Err.ErrorMessage = "GetAssetList JsonOrderObject deserialized.Data != null";

            return null;
        }

    }

    // 注文発注メソッド
    public async Task<OrderResult> MakeOrderAsync(string apiKey, string apiSecret, string pair, decimal amount, decimal price, string side, string type, bool postOnly)
    {
        var ord = new OrderResult();

        var path = new Uri("/user/spot/order", UriKind.Relative);

        var orderParam = new OrderParam(pair, amount.ToString(), price.ToString(), side, type, postOnly);

        //var body = JsonConvert.SerializeObject(orderParam);
        var body = System.Text.Json.JsonSerializer.Serialize(orderParam, OrderJsonSerializerContext.Default.OrderParam);

        //Debug.WriteLine("MakingOrder... resquest body = " + body);

        try
        {
            var resbo = await PrivateSendAsync(path, apiKey, apiSecret, HttpMethod.Post, body);
            if (resbo == null)
            {
                // This should not happen.
                Debug.WriteLine("■■■■■ MakeOrder: Send returned NULL.");

                return ord;
            }
            if ((!resbo.IsSuccess) && (resbo.HTTPError != null))
            {
                ord.IsSuccess = false;
                ord.HasErrorInfo = true;

                ord.Err.ErrorCode = resbo.HTTPError.ErrCode;
                ord.Err.ErrorType = ErrType.HttpError;
                ord.Err.ErrorMessage = resbo.HTTPError.ErrText ?? resbo.HTTPError.ErrCode.ToString();

                return ord;
            }

            var json = resbo.BodyText;

            if (!string.IsNullOrEmpty(json))
            {
                //Debug.WriteLine("MakeOrder result: " + json);

                //var deserialized = JsonConvert.DeserializeObject<JsonOrderObject>(json);
                var deserialized = System.Text.Json.JsonSerializer.Deserialize<JsonOrderObject>(json, OrderJsonSerializerContext.Default.JsonOrderObject);

                if (deserialized?.Success > 0)
                {
                    if (deserialized.Data != null)
                    {
                        ord.OrderID = deserialized.Data.Order_id;
                        ord.Pair = deserialized.Data.Pair ?? string.Empty;
                        ord.Side = deserialized.Data.Side ?? string.Empty;
                        ord.Type = deserialized.Data.Type ?? string.Empty;

                        ord.StartAmount = decimal.Parse(deserialized.Data.Start_amount ?? "0");
                        ord.RemainingAmount = decimal.Parse(deserialized.Data.Remaining_amount ?? "0");
                        ord.ExecutedAmount = decimal.Parse(deserialized.Data.Executed_amount ?? "0");
                        if (deserialized.Data.Price != null)
                        {
                            ord.Price = decimal.Parse(deserialized.Data.Price);
                        }
                        ord.AveragePrice = decimal.Parse(deserialized.Data.Average_price ?? "0");

                        var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        var date = start.AddMilliseconds(deserialized.Data.Ordered_at).ToLocalTime();
                        ord.OrderedAt = date;

                        ord.Status = deserialized.Data.Status ?? string.Empty;

                        ord.IsSuccess = true;
                        ord.HasErrorInfo = false;
                    }
                    else
                    {
                        ord.IsSuccess = false;
                        ord.HasErrorInfo = true;

                        ord.Err.ErrorCode = -1;
                        ord.Err.ErrorType = ErrType.ResposeParseError;
                        ord.Err.ErrorMessage = $"Error on MakeOrder: JsonOrderObject deserialized.Data != null";

                        // エラーイベント発火
                        var er = new ApiClientError
                        {
                            ErrType = "Data",
                            ErrCode = 0,
                            ErrText = "JsonOrderObject deserialized.Data != null @MakeOrder",
                            ErrDatetime = DateTime.Now,
                            ErrPlace = path.ToString()
                        };

                        ApiErrorOccured?.Invoke(this, er);
                    }

                    //return ord;
                }
                else
                {
                    //var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);
                    var jsonResult = System.Text.Json.JsonSerializer.Deserialize(json, ErrorJsonSerializerContext.Default.JsonErrorObject);

                    ord.IsSuccess = false;
                    ord.ApiErrorCode = jsonResult?.Data?.Code ?? 0;

                    Debug.WriteLine("■■■■■ MakingOrder: API error code - " + jsonResult?.Data?.Code.ToString() + " ■■■■■");

                    // ユーザに表示するエラー情報
                    ord.HasErrorInfo = true;

                    ord.Err.ErrorCode = ord.ApiErrorCode;
                    ord.Err.ErrorType = ErrType.ApiError;
                    ord.Err.ErrorSeverity = FAInfoBarSeverity.Warning;
                    if (ApiErrorCodesDictionary.TryGetValue(ord.ApiErrorCode, out var value))
                    {
                        ord.Err.ErrorMessage = value;
                    }
                    ord.Err.ErrorMessage = $"Error on MakeOrder: {ord.Err.ErrorMessage} (Error code: {ord.ApiErrorCode})";

                    // エラーイベント発火
                    var er = new ApiClientError
                    {
                        ErrType = "API",
                        ErrCode = jsonResult?.Data?.Code ?? 0
                    };
                    if (ApiErrorCodesDictionary.ContainsKey(jsonResult?.Data?.Code ?? 0))
                    {
                        er.ErrText = "(" + ApiErrorCodesDictionary[jsonResult?.Data?.Code ?? 0] + ")";
                    }
                    er.ErrDatetime = DateTime.Now;
                    er.ErrPlace = path.ToString();

                    ApiErrorOccured?.Invoke(this, er);

                    //return ord;
                }
            }
            else
            {
                Debug.WriteLine("■■■■■ MakeOrder: Send returned Empty String...Could be HTTP error");

                ord.IsSuccess = false;
                ord.ApiErrorCode = -1;
                ord.HasErrorInfo = true;

                ord.Err.ErrorCode = -1;
                ord.Err.ErrorType = ErrType.ResposeParseError;
                ord.Err.ErrorMessage = $"Error on MakeOrder: /user/spot/order への発注処理で、空のBodyTextが返りました。";

                // エラーイベント発火
                var er = new ApiClientError
                {
                    ErrType = "Data",
                    ErrCode = 0,
                    ErrText = "発注処理で、空のBodyTextが返りました。",
                    ErrDatetime = DateTime.Now,
                    ErrPlace = path.ToString()
                };

                ApiErrorOccured?.Invoke(this, er);

                //return ord;
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine("■■■■■ MakeOrder Exception: " + e + " ■■■■■");

            ord.IsSuccess = false;
            ord.ApiErrorCode = -1;
            ord.HasErrorInfo = true;

            ord.Err.ErrorType = ErrType.Unexpected;
            ord.Err.ErrorCode = -1;
            ord.Err.ErrorMessage = $"Error on MakeOrder: /user/spot/order への発注処理で、予期しないエラーが発生しました。{e.Message}";
        }

        return ord;
    }

    // 注文情報をIDから取得メソッド
    public async Task<OrderResult?> GetOrderByIdAsync(string apiKey, string apiSecret, string pair, ulong orderID)
    {
        var path = new Uri("/user/spot/order", UriKind.Relative);

        var param = new Dictionary<string, string>{
                { "pair", pair },//取引する通貨の種類
                { "order_id", orderID.ToString() },
            };

        var resbo = await PrivateSendAsync(path, apiKey, apiSecret, HttpMethod.Get, "", param);
        if (resbo == null)
        {
            Debug.WriteLine("■■■■■ GetOrderByID: Send returned NULL.");
            return null;
        }
        var json = resbo.BodyText;

        if (!string.IsNullOrEmpty(json))
        {
            //System.Diagnostics.Debug.WriteLine("GetOrderByID: " + json);

            //var deserialized = JsonConvert.DeserializeObject<JsonOrderObject>(json);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<JsonOrderObject>(json, OrderJsonSerializerContext.Default.JsonOrderObject);

            if (deserialized?.Success > 0)
            {
                var ord = new OrderResult
                {
                    IsSuccess = true
                };

                if (deserialized.Data != null)
                {
                    ord.OrderID = deserialized.Data.Order_id;
                    ord.Pair = deserialized.Data.Pair;
                    ord.Side = deserialized.Data.Status ?? string.Empty;
                    ord.Type = deserialized.Data.Status ?? string.Empty;

                    ord.StartAmount = decimal.Parse(deserialized.Data.Start_amount ?? "0");
                    ord.RemainingAmount = decimal.Parse(deserialized.Data.Remaining_amount ?? "0");
                    ord.ExecutedAmount = decimal.Parse(deserialized.Data.Executed_amount ?? "0");
                    if (deserialized.Data.Price != null)
                    {
                        ord.Price = decimal.Parse(deserialized.Data.Price);
                    }
                    ord.AveragePrice = decimal.Parse(deserialized.Data.Average_price ?? "0");

                    var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var date = start.AddMilliseconds(deserialized.Data.Ordered_at).ToLocalTime();
                    ord.OrderedAt = date;

                    ord.Status = deserialized.Data.Status ?? string.Empty;
                }

                return ord;
            }
            else
            {
                var ord = new OrderResult();

                //var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);
                var jsonResult = System.Text.Json.JsonSerializer.Deserialize(json, ErrorJsonSerializerContext.Default.JsonErrorObject);

                ord.IsSuccess = false;
                ord.ApiErrorCode = jsonResult?.Data?.Code ?? 0;

                Debug.WriteLine("■■■■■ GetOrderByID: API error code - " + jsonResult?.Data?.Code.ToString() + " ■■■■■");

                ord.HasErrorInfo = true;

                ord.Err.ErrorCode = ord.ApiErrorCode;
                ord.Err.ErrorType = ErrType.ApiError;
                ord.Err.ErrorSeverity = FAInfoBarSeverity.Warning;
                if (ApiErrorCodesDictionary.TryGetValue(ord.ApiErrorCode, out var value))
                {
                    ord.Err.ErrorMessage = value;
                }
                ord.Err.ErrorMessage = $"Error on GetOrderByID: {ord.Err.ErrorMessage} (Error code: {ord.ApiErrorCode})";


                // エラーイベント発火
                var er = new ApiClientError
                {
                    ErrType = "API",
                    ErrCode = jsonResult?.Data?.Code ?? 0
                };
                if (ApiErrorCodesDictionary.ContainsKey(jsonResult?.Data?.Code ?? 0))
                {
                    er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult?.Data?.Code ?? 0] + "」";
                }
                er.ErrDatetime = DateTime.Now;
                er.ErrPlace = path.ToString();

                ApiErrorOccured?.Invoke(this, er);

                return ord;
            }
        }
        else
        {
            Debug.WriteLine("■■■■■ GetOrderByID: Send returned empty string. Could be a HTTP error.");

            var ord = new OrderResult
            {
                IsSuccess = false,
                ApiErrorCode = -1,
                HasErrorInfo = true
            };

            ord.Err.ErrorType = ErrType.ApiError;
            ord.Err.ErrorCode = -1;
            ord.Err.ErrorType = ErrType.ResposeParseError;
            ord.Err.ErrorMessage = $"Error on GetOrderByID: /user/spot/order への注文情報取得処理で、空のBodyTextが返りました。HTTPエラー: {resbo.HTTPError.ErrCode}";

            return ord;
        }
    }

    // 注文情報を複数のIDから取得メソッド
    public async Task<OrderListResult?> GetOrderListByIdsAsync(string apiKey, string apiSecret, string pair, List<ulong> orderIDs)
    {
        var path = new Uri("/user/spot/orders_info", UriKind.Relative);

        var idParam = new PairOrderIdList(pair, orderIDs);
        //var body = JsonConvert.SerializeObject(idParam);
        var body = System.Text.Json.JsonSerializer.Serialize(idParam, OrderJsonSerializerContext.Default.PairOrderIdList);

        var resbo = await PrivateSendAsync(path, apiKey, apiSecret, HttpMethod.Post, body);
        if (resbo == null)
        {
            Debug.WriteLine("■■■■■ GetOrderListByIDs: Send returned NULL.");
            return null;
        }

        var json = resbo.BodyText;

        try
        {
            if (!string.IsNullOrEmpty(json))
            {
                //System.Diagnostics.Debug.WriteLine("GetOrderListByIDs: " + json);

                //var deserialized = JsonConvert.DeserializeObject<JsonOrderInfoObject>(json);
                var deserialized = System.Text.Json.JsonSerializer.Deserialize<JsonOrderInfoObject>(json, OrderJsonSerializerContext.Default.JsonOrderInfoObject);

                if (deserialized?.Success > 0)
                {
                    var ords = new OrderListResult
                    {
                        IsSuccess = true
                    };

                    if (deserialized.Data != null)
                    {
                        if (deserialized.Data.Orders != null)
                        {
                            foreach (var ord in deserialized.Data.Orders)
                            {
                                try
                                {
                                    var o = new Order
                                    {
                                        OrderID = ord.Order_id,
                                        Pair = ord.Pair,
                                        Side = ord.Side ?? string.Empty,
                                        Type = ord.Type ?? string.Empty,
                                        OrderedAt = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(ord.Ordered_at).ToLocalTime(),
                                        Status = ord.Status ?? string.Empty
                                    };

                                    if (!string.IsNullOrEmpty(ord.Start_amount))
                                    {
                                        o.StartAmount = decimal.Parse(ord.Start_amount);
                                    }

                                    if (!string.IsNullOrEmpty(ord.Remaining_amount))
                                    {
                                        o.RemainingAmount = decimal.Parse(ord.Remaining_amount);
                                    }

                                    if (!string.IsNullOrEmpty(ord.Executed_amount))
                                    {
                                        o.ExecutedAmount = decimal.Parse(ord.Executed_amount);
                                    }

                                    if (!string.IsNullOrEmpty(ord.Price))
                                    {
                                        o.Price = decimal.Parse(ord.Price);
                                    }

                                    if (!string.IsNullOrEmpty(ord.Average_price))
                                    {
                                        o.AveragePrice = decimal.Parse(ord.Average_price);
                                    }

                                    ords.OrderList.Add(o);

                                    //System.Diagnostics.Debug.WriteLine("GetOrderListByIDs ord.status: " + ord.status);
                                }
                                catch
                                {
                                    System.Diagnostics.Debug.WriteLine("■■■■■ GGetOrderListByIDs - ords.OrderList.Add ■■■■■");
                                }
                            }
                        }
                    }

                    return ords;
                }
                else
                {
                    //var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);
                    var jsonResult = System.Text.Json.JsonSerializer.Deserialize(json, ErrorJsonSerializerContext.Default.JsonErrorObject);

                    System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderListByIDs: API error code - " + jsonResult?.Data?.Code.ToString() + " ■■■■■");

                    // エラーイベント発火
                    var er = new ApiClientError
                    {
                        ErrType = "API",
                        ErrCode = jsonResult?.Data?.Code ?? 0
                    };
                    if (ApiErrorCodesDictionary.ContainsKey(jsonResult?.Data?.Code ?? 0))
                    {
                        er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult?.Data?.Code ?? 0] + "」";
                    }
                    er.ErrDatetime = DateTime.Now;
                    er.ErrPlace = path.ToString();
                    er.ErrPlaceParent = "GetOrderListByIDs";
                    er.ErrEx = "注文情報を更新出来ませんでした";

                    ApiErrorOccured?.Invoke(this, er);

                    return null;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderListByIDs: Send returned empty string.");

                var ords = new OrderListResult
                {
                    IsSuccess = false,
                    ApiErrorCode = -1
                };

                ords.Err.ErrorType = ErrType.ApiError;
                ords.Err.ErrorCode = resbo.HTTPError.ErrCode;
                ords.Err.ErrorMessage = $"Error on GetOrderListByIDs: /user/spot/orders_info への情報取得処理で、空のBodyTextが返りました。HTTPエラー: {resbo.HTTPError.ErrCode}";

                return ords;
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderListByIDs: Exception " + e);

            // TODO: これはClientErrorの方を発火すべきでは？
            var ord = new OrderListResult
            {
                IsSuccess = false,
                ApiErrorCode = -1
            };

            ord.Err.ErrorType = ErrType.Unexpected;
            ord.Err.ErrorCode = -1;
            ord.Err.ErrorMessage = $"Error on GetOrderListByIDs: /user/spot/orders_info への情報取得処理で、予期しないエラーが発生しました。{e.Message}";

            return ord;
        }
    }

    // 注文キャンセルメソッド
    public async Task<OrderResult> CancelOrderAsync(string apiKey, string apiSecret, string pair, ulong orderID)
    {
        var ord = new OrderResult();

        var path = new Uri("/user/spot/cancel_order", UriKind.Relative);

        var cancelOrderParam = new PairOrderIdParam(pair, orderID);

        //var body = JsonConvert.SerializeObject(cancelOrderParam);
        var body = System.Text.Json.JsonSerializer.Serialize(cancelOrderParam, OrderJsonSerializerContext.Default.PairOrderIdList);

        //string json = await Send(path, apiKey, apiSecret, HttpMethod.Post, body);
        var resbo = await PrivateSendAsync(path, apiKey, apiSecret, HttpMethod.Post, body);
        if (resbo == null)
        {
            Debug.WriteLine("■■■■■ CancelOrder: Send returned NULL.");
            return ord;
        }
        var json = resbo.BodyText;

        if (!string.IsNullOrEmpty(json))
        {
            //System.Diagnostics.Debug.WriteLine("CancelOrder: " + json);

            //var deserialized = JsonConvert.DeserializeObject<JsonOrderObject>(json);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<JsonOrderObject>(json, OrderJsonSerializerContext.Default.JsonOrderObject);

            if (deserialized?.Success > 0)
            {
                if (deserialized.Data != null)
                {
                    ord.OrderID = deserialized.Data.Order_id;
                    ord.Pair = deserialized.Data.Pair;
                    ord.Side = deserialized.Data.Side ?? string.Empty;
                    ord.Type = deserialized.Data.Type ?? string.Empty;

                    //TODO エラーハンドリング
                    ord.StartAmount = decimal.Parse(deserialized.Data.Start_amount ?? "0");
                    ord.RemainingAmount = decimal.Parse(deserialized.Data.Remaining_amount ?? "0");
                    ord.ExecutedAmount = decimal.Parse(deserialized.Data.Executed_amount ?? "0");
                    ord.Price = decimal.Parse(deserialized.Data.Price ?? "0");
                    ord.AveragePrice = decimal.Parse(deserialized.Data.Average_price ?? "0");

                    var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var date = start.AddMilliseconds(deserialized.Data.Ordered_at).ToLocalTime();
                    ord.OrderedAt = date;

                    ord.Status = deserialized.Data.Status ?? string.Empty;

                    ord.IsSuccess = true;
                    ord.HasErrorInfo = false;
                }
                else
                {
                    ord.IsSuccess = false;
                    ord.HasErrorInfo = true;

                    ord.Err.ErrorCode = -1;
                    ord.Err.ErrorType = ErrType.ResposeParseError;
                    ord.Err.ErrorMessage = "Error on CancelOrder: JsonOrderObject deserialized.Data != null";

                    // エラーイベント発火
                    var er = new ApiClientError
                    {
                        ErrType = "Data",
                        ErrCode = 0,
                        ErrText = "JsonOrderObject deserialized.Data != null @CancelOrder",
                        ErrDatetime = DateTime.Now,
                        ErrPlace = path.ToString()
                    };

                    ApiErrorOccured?.Invoke(this, er);
                }

                //return ord;
            }
            else
            {
                //var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);
                var jsonResult = System.Text.Json.JsonSerializer.Deserialize(json, ErrorJsonSerializerContext.Default.JsonErrorObject);

                ord.IsSuccess = false;
                ord.ApiErrorCode = jsonResult?.Data?.Code ?? 0;

                // ユーザに表示するエラー情報
                ord.HasErrorInfo = true;

                ord.Err.ErrorCode = ord.ApiErrorCode;
                ord.Err.ErrorType = ErrType.ApiError;
                ord.Err.ErrorSeverity = FAInfoBarSeverity.Warning;
                if (ApiErrorCodesDictionary.TryGetValue(ord.ApiErrorCode, out var value))
                {
                    ord.Err.ErrorMessage = value;
                }
                ord.Err.ErrorMessage = $"Error on CancelOrder: {ord.Err.ErrorMessage} (Error code: {ord.ApiErrorCode})";

                // エラーイベント発火
                var er = new ApiClientError
                {
                    ErrType = "API",
                    ErrCode = jsonResult?.Data?.Code ?? 0
                };
                if (ApiErrorCodesDictionary.ContainsKey(jsonResult?.Data?.Code ?? 0))
                {
                    er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult?.Data?.Code ?? 0] + "」";
                }
                er.ErrDatetime = DateTime.Now;
                er.ErrPlace = path.ToString();

                ApiErrorOccured?.Invoke(this, er);

                Debug.WriteLine("■■■■■ CancelOrder: API error code - " + jsonResult?.Data?.Code.ToString() + " ■■■■■");

                //return ord;
            }
        }
        else
        {
            Debug.WriteLine("■■■■■ CancelOrder: Send returned NULL.");

            ord.IsSuccess = false;
            ord.ApiErrorCode = -1;
            ord.HasErrorInfo = true;

            ord.Err.ErrorCode = resbo.HTTPError.ErrCode;
            ord.Err.ErrorType = ErrType.ResposeParseError;
            ord.Err.ErrorMessage = $"Error on CancelOrder: /user/spot/cancel_order への注文キャンセル処理で、空のBodyTextが返りました。HTTP error code: {resbo.HTTPError.ErrCode}";

            // エラーイベント発火
            var er = new ApiClientError
            {
                ErrType = "Data",
                ErrCode = 0,
                ErrText = "注文キャンセル処理で、空のBodyTextが返りました。",
                ErrDatetime = DateTime.Now,
                ErrPlace = path.ToString()
            };

            ApiErrorOccured?.Invoke(this, er);

            //return ord;
        }

        return ord;
    }

    // 文キャンセル（複数）メソッド
    public async Task<OrderListResult?> CancelOrdersAsync(string apiKey, string apiSecret, string pair, List<ulong> orderIDs)
    {
        var path = new Uri("/user/spot/cancel_orders", UriKind.Relative);

        var idParam = new PairOrderIdList(pair, orderIDs);
        //var body = JsonConvert.SerializeObject(idParam);
        var body = System.Text.Json.JsonSerializer.Serialize(idParam, OrderJsonSerializerContext.Default.PairOrderIdList);

        //string json = await Send(path, apiKey, apiSecret, HttpMethod.Post, body);
        var resbo = await PrivateSendAsync(path, apiKey, apiSecret, HttpMethod.Post, body);
        if (resbo == null)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrders: Send returned NULL.");
            return null;
        }
        var json = resbo.BodyText;

        try
        {
            if (!string.IsNullOrEmpty(json))
            {
                //System.Diagnostics.Debug.WriteLine("CancelOrders: " + json);

                //var deserialized = JsonConvert.DeserializeObject<JsonOrderInfoObject>(json);
                var deserialized = System.Text.Json.JsonSerializer.Deserialize<JsonOrderInfoObject>(json, OrderJsonSerializerContext.Default.JsonOrderInfoObject);

                if (deserialized?.Success > 0)
                {
                    var ords = new OrderListResult
                    {
                        IsSuccess = true
                    };

                    if (deserialized.Data != null)
                    {
                        if (deserialized.Data.Orders != null)
                        {
                            foreach (var ord in deserialized.Data.Orders)
                            {
                                try
                                {
                                    var o = new Order
                                    {
                                        OrderID = ord.Order_id,
                                        Pair = ord.Pair,
                                        Side = ord.Side ?? string.Empty,
                                        Type = ord.Type ?? string.Empty,
                                        OrderedAt = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(ord.Ordered_at).ToLocalTime(),
                                        Status = ord.Status ?? string.Empty
                                    };

                                    if (!string.IsNullOrEmpty(ord.Start_amount))
                                    {
                                        o.StartAmount = decimal.Parse(ord.Start_amount);
                                    }

                                    if (!string.IsNullOrEmpty(ord.Remaining_amount))
                                    {
                                        o.RemainingAmount = decimal.Parse(ord.Remaining_amount);
                                    }

                                    if (!string.IsNullOrEmpty(ord.Executed_amount))
                                    {
                                        o.ExecutedAmount = decimal.Parse(ord.Executed_amount);
                                    }

                                    if (!string.IsNullOrEmpty(ord.Price))
                                    {
                                        o.Price = decimal.Parse(ord.Price);
                                    }

                                    if (!string.IsNullOrEmpty(ord.Average_price))
                                    {
                                        o.AveragePrice = decimal.Parse(ord.Average_price);
                                    }

                                    ords.OrderList.Add(o);

                                    //System.Diagnostics.Debug.WriteLine("CancelOrders ord.status: " + ord.status);
                                }
                                catch
                                {
                                    System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrders - ords.OrderList.Add ■■■■■");
                                }
                            }
                        }
                    }

                    return ords;
                }
                else
                {
                    //var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);
                    var jsonResult = System.Text.Json.JsonSerializer.Deserialize(json, ErrorJsonSerializerContext.Default.JsonErrorObject);

                    System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrders: API error code - " + jsonResult?.Data?.Code.ToString() + " ■■■■■");

                    // エラーイベント発火
                    var er = new ApiClientError
                    {
                        ErrType = "API",
                        ErrCode = jsonResult?.Data?.Code ?? 0
                    };
                    if (ApiErrorCodesDictionary.ContainsKey(jsonResult?.Data?.Code ?? 0))
                    {
                        er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult?.Data?.Code ?? 0] + "」";
                    }
                    er.ErrDatetime = DateTime.Now;
                    er.ErrPlace = path.ToString();

                    ApiErrorOccured?.Invoke(this, er);

                    return null;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrders: Send returned NULL.");

                var ords = new OrderListResult
                {
                    IsSuccess = false,
                    ApiErrorCode = -1
                };

                ords.Err.ErrorCode = resbo.HTTPError.ErrCode;
                ords.Err.ErrorType = ErrType.ResposeParseError;
                ords.Err.ErrorMessage = $"Error on CancelOrders: /user/spot/cancel_orders への注文キャンセル処理で、HTTPエラー。HTTP error code: {resbo.HTTPError.ErrCode}";

                return ords;
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrders: Exception " + e);

            // TODO: これはClientErrorの方を発火すべきでは？
            var ord = new OrderListResult
            {
                IsSuccess = false,
                ApiErrorCode = -1
            };

            ord.Err.ErrorCode = -1;
            ord.Err.ErrorType = ErrType.Unexpected;
            ord.Err.ErrorCode = -1;
            ord.Err.ErrorMessage = $"Error on CancelOrders: /user/spot/cancel_orders への情報取得処理で、予期しないエラーが発生しました。{e.Message}";

            return ord;
        }
    }

    // 注文リスト取得メソッド
    public async Task<OrderListResult?> GetOrderListAsync(string apiKey, string apiSecret, string pair)
    {
        var path = new Uri("/user/spot/active_orders", UriKind.Relative);

        var param = new Dictionary<string, string>{
                { "pair", pair },//取引する通貨の種類
                //{ "count", "10" },//取得する注文数（double）
                //{ "from_id", "100000" },//取得開始注文ID
                //{ "end_id", "100000" },//取得終了注文ID
                //{ "since", "100000" },//開始UNIXタイムスタンプ
                //{ "end", "0.01" },//終了UNIXタイムスタンプ
            };

        //string json = await Send(path, apiKey, apiSecret, HttpMethod.Get,"", param);
        var resbo = await PrivateSendAsync(path, apiKey, apiSecret, HttpMethod.Get, "", param);
        if (resbo == null)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderList: Send returned NULL.");
            /*
            var ords = new OrderListResult
            {
                IsSuccess = false,
                ApiErrorCode = -1
            };

            ords.Err.ErrorTitle = "注文リスト取得でHTTPエラーが返りました。";
            //ords.Err.ErrorCode = resbo.HTTPError.ErrCode;
            ords.Err.ErrorDescription = "/user/spot/active_orders への注文リスト取得処理で、HTTPエラー（" + resbo.HTTPError.ErrCode.ToString() + "）が返りました。";

            return ords;
            */

            return null;
        }
        var json = resbo.BodyText;

        if (!string.IsNullOrEmpty(json))
        {
            //System.Diagnostics.Debug.WriteLine("■■■■■ ■■■■■ ■■■■■ GetOrderList: " + json);

            //var deserialized = JsonConvert.DeserializeObject<JsonOrderInfoObject>(json);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<JsonOrderInfoObject>(json, OrderJsonSerializerContext.Default.JsonOrderInfoObject);

            if (deserialized?.Success > 0)
            {
                var ords = new OrderListResult
                {
                    IsSuccess = true
                };

                if (deserialized?.Data?.Orders != null)
                {
                    foreach (var ord in deserialized.Data.Orders)
                    {
                        var o = new Order
                        {
                            OrderID = ord.Order_id,
                            Pair = ord.Pair,
                            Side = ord.Side ?? string.Empty,
                            Type = ord.Type ?? string.Empty,
                            OrderedAt = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(ord.Ordered_at).ToLocalTime(),
                            Status = ord.Status ?? string.Empty
                        };

                        if (!string.IsNullOrEmpty(ord.Start_amount))
                        {
                            o.StartAmount = decimal.Parse(ord.Start_amount);
                        }

                        if (!string.IsNullOrEmpty(ord.Remaining_amount))
                        {
                            o.RemainingAmount = decimal.Parse(ord.Remaining_amount);
                        }

                        if (!string.IsNullOrEmpty(ord.Executed_amount))
                        {
                            o.ExecutedAmount = decimal.Parse(ord.Executed_amount);
                        }

                        if (!string.IsNullOrEmpty(ord.Price)) { o.Price = decimal.Parse(ord.Price); } else { o.Price = 0; }
                        if (!string.IsNullOrEmpty(ord.Average_price))
                        {
                            o.AveragePrice = decimal.Parse(ord.Average_price);
                        }

                        ords.OrderList.Add(o);
                    }
                }
                return ords;
            }
            else
            {
                //var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);
                var jsonResult = System.Text.Json.JsonSerializer.Deserialize(json, ErrorJsonSerializerContext.Default.JsonErrorObject);

                System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderList: API error code - " + jsonResult?.Data?.Code.ToString() + " ■■■■■");

                // エラーイベント発火
                var er = new ApiClientError
                {
                    ErrType = "API",
                    ErrCode = jsonResult?.Data?.Code ?? 0
                };
                if (ApiErrorCodesDictionary.ContainsKey(jsonResult?.Data?.Code ?? 0))
                {
                    er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult?.Data?.Code ?? 0] + "」";
                }
                er.ErrDatetime = DateTime.Now;
                er.ErrPlace = path.ToString();

                ApiErrorOccured?.Invoke(this, er);

                return null;
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderList: Send returned empty Json BodyText.");
            /*
            var ords = new OrderListResult
            {
                IsSuccess = false,
                ApiErrorCode = -1
            };

            ords.Err.ErrorTitle = "注文リスト取得でHTTPエラーが返りました。";
            ords.Err.ErrorCode = resbo.HTTPError.ErrCode;
            ords.Err.ErrorDescription = "/user/spot/active_orders への注文リスト取得処理で、HTTPエラー（" + resbo.HTTPError.ErrCode.ToString() + "）が返りました。";

            return ords;
            */
            return null;
        }
    }

    // 取引履歴取得メソッド
    public async Task<TradeHistory?> GetTradeHistoryAsync(string apiKey, string apiSecret, string pair)
    {

        var path = new Uri("/user/spot/trade_history", UriKind.Relative);

        var param = new Dictionary<string, string>{
                { "pair", pair },//取得する約定数
                { "count", "10" },//取得する注文数（double）
                //{ "order_id", "100000" },//注文ID
                //{ "since", "100000" },//開始UNIXタイムスタンプ
                //{ "end", "0.01" },//終了UNIXタイムスタンプ
                //{ "order", "asc" },//約定時刻順序(asc: 昇順、desc: 降順、デフォルト降順)
            };

        //string json = await Send(path, apiKey, apiSecret, HttpMethod.Get, "", param);
        var resbo = await PrivateSendAsync(path, apiKey, apiSecret, HttpMethod.Get, "", param);
        if (resbo == null)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetTradeHistory: Send returned NULL.");
            return null;
        }
        var json = resbo.BodyText;

        if (!string.IsNullOrEmpty(json))
        {
            //System.Diagnostics.Debug.WriteLine("GetTradeHistory: " + json);

            //var deserialized = JsonConvert.DeserializeObject<JsonTradeHistoryObject>(json);
            var deserialized = System.Text.Json.JsonSerializer.Deserialize<JsonTradeHistoryObject>(json, TradeHistoryJsonSerializerContext.Default.JsonTradeHistoryObject);

            if (deserialized?.Success > 0)
            {
                var history = new TradeHistory();

                if (deserialized?.Data != null)
                {
                    if (deserialized.Data.Trades != null)
                    {
                        foreach (var trd in deserialized.Data.Trades)
                        {
                            var o = new Trade
                            {
                                TradeID = trd.Trade_id,
                                OrderID = trd.Order_id,
                                Pair = trd.Pair ?? string.Empty,
                                Side = trd.Side ?? string.Empty,
                                Type = trd.Type ?? string.Empty,
                                MakerTaker = trd.Maker_taker,
                                ExecutedAt = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(trd.Executed_at).ToLocalTime(),

                            };

                            if (!string.IsNullOrEmpty(trd.Price)) { o.Price = decimal.Parse(trd.Price); } else { o.Price = 0; }

                            if (!string.IsNullOrEmpty(trd.Amount))
                            {
                                o.Amount = decimal.Parse(trd.Amount);
                                //o.FeeAmountBase = decimal.Parse(trd.Fee_amount_base ?? "0");
                                o.FeeOccurredAmountQuote = decimal.Parse(trd.Fee_occurred_amount_quote ?? "0");
                                o.FeeAmountQuote = decimal.Parse(trd.Fee_amount_quote ?? "0");
                            }

                            history.TradeList.Add(o);
                        }
                    }
                }

                return history;
            }
            else
            {
                //var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);
                var jsonResult = System.Text.Json.JsonSerializer.Deserialize(json, ErrorJsonSerializerContext.Default.JsonErrorObject);

                System.Diagnostics.Debug.WriteLine("■■■■■ GetTradeHistory: API error code - " + jsonResult?.Data?.Code.ToString() + " ■■■■■");

                // エラーイベント発火
                var er = new ApiClientError
                {
                    ErrType = "API",
                    ErrCode = jsonResult?.Data?.Code ?? 0
                };
                if (ApiErrorCodesDictionary.ContainsKey(jsonResult?.Data?.Code ?? 0))
                {
                    er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult?.Data?.Code ?? 0] + "」";
                }
                er.ErrDatetime = DateTime.Now;
                er.ErrPlace = path.ToString();

                ApiErrorOccured?.Invoke(this, er);

                return null;
            }

        }
        else
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetTradeHistory: Send returned NULL.");

            // TradeHistryの場合はords返しても使わないので

            return null;
        }

    }

    // HTTPリクエスト送信メソッド
    internal async Task<ResponseBodyWrapper> PrivateSendAsync(Uri path, string apiKey, string secret, HttpMethod method, string body = "", Dictionary<string, string>? queries = null)
    {
        var resbowrap = new ResponseBodyWrapper();
        if (_privateClient is null)
        {
            return resbowrap;
        }

        try
        {
            //ACCESS-NONCE
            var _accessNonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(); //NONCE=unixtime

            // メッセージを作成
            var message = "";
            if (method == HttpMethod.Get)
            {
                if (queries != null)
                {
                    // パラメータ文字列を作成
                    var pms = new FormUrlEncodedContent(queries);
                    var param = await pms.ReadAsStringAsync();

                    message = _accessNonce + "/v1" + path.ToString() + "?" + param;
                }
                else
                {
                    message = _accessNonce + "/v1" + path.ToString();
                }
            }
            else if (method == HttpMethod.Post)
            {
                message = _accessNonce + body;
            }

            // メッセージをHMACSHA256で署名
            var hash = new HMACSHA256(Encoding.UTF8.GetBytes(secret)).ComputeHash(Encoding.UTF8.GetBytes(message));
            //ACCESS-SIGNATURE
            var _accessSignature = Convert.ToHexStringLower(hash);//バイト配列をを16進文字列へ << CA1872

            //System.Diagnostics.Debug.WriteLine("Sending..." + Environment.NewLine + _HTTPConn._client.DefaultRequestHeaders.ToString());

            HttpResponseMessage res;
            try
            {
                if (method == HttpMethod.Post)
                {
                    var content = new StringContent(body, Encoding.UTF8, "application/json");

                    content.Headers.Add("ACCESS-KEY", apiKey);
                    content.Headers.Add("ACCESS-NONCE", _accessNonce);
                    content.Headers.Add("ACCESS-SIGNATURE", _accessSignature);

                    res = await _privateClient.PostAsync(_privateClient.BaseAddress?.ToString() + path.ToString(), content);
                }
                else if (method == HttpMethod.Get)
                {

                    if (queries == null)
                    {
                        //
                        var requestMessage = new HttpRequestMessage(HttpMethod.Get, _privateClient.BaseAddress?.ToString() + path.ToString());
                        requestMessage.Headers.Add("ACCESS-KEY", apiKey);
                        requestMessage.Headers.Add("ACCESS-NONCE", _accessNonce);
                        requestMessage.Headers.Add("ACCESS-SIGNATURE", _accessSignature);

                        res = await _privateClient.SendAsync(requestMessage);
                    }
                    else
                    {
                        var pms = new FormUrlEncodedContent(queries);
                        var param = await pms.ReadAsStringAsync();

                        //
                        var requestMessage = new HttpRequestMessage(HttpMethod.Get, _privateClient.BaseAddress?.ToString() + path.ToString() + "?" + param);
                        requestMessage.Headers.Add("ACCESS-KEY", apiKey);
                        requestMessage.Headers.Add("ACCESS-NONCE", _accessNonce);
                        requestMessage.Headers.Add("ACCESS-SIGNATURE", _accessSignature);

                        res = await _privateClient.SendAsync(requestMessage);
                    }
                }
                else
                {
                    throw new ArgumentException("method は POST か GET を指定してください。", method.ToString());
                }

                //返答内容を取得
                var text = await res.Content.ReadAsStringAsync();

                //通信上の失敗
                if (!res.IsSuccessStatusCode)
                {
                    //System.Diagnostics.Debug.WriteLine("■■■■■■■■ Send: HTTP Error " + res.StatusCode.ToString() + " " + method + " " + _HTTPConn._client.BaseAddress.ToString() + path.ToString() + " ■■■■■■■");

                    var er = new ApiClientError
                    {
                        ErrType = "HTTP " + method,
                        ErrCode = (int)res.StatusCode,
                        ErrText = res.StatusCode.ToString(),
                        ErrDatetime = DateTime.Now,
                        ErrPlace = path.ToString()
                    };
                    // イベント発火
                    ApiErrorOccured?.Invoke(this, er);

                    //ラッパー
                    resbowrap.BodyText = text;
                    resbowrap.HTTPError = er;

                    return resbowrap;
                }

                resbowrap.IsSuccess = true;
                resbowrap.BodyText = text;
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                string errtext;
                if (ex.InnerException != null)
                {
                    Debug.WriteLine("HTTP SocketException - " + ex.Message + " + 内部例外: " + ex.InnerException.Message);
                    errtext = "HTTP SocketException - " + ex.Message + " + InnerException: " + ex.InnerException.Message;
                }
                else
                {
                    Debug.WriteLine("HTTP SocketException - " + ex.Message);
                    errtext = "HTTP SocketException - " + ex.Message;
                }

                var er = new ApiClientError
                {
                    ErrType = "HTTP " + method,
                    ErrCode = 0,
                    ErrText = errtext,
                    ErrDatetime = DateTime.Now,
                    ErrPlace = path.ToString()
                };
                // 
                ApiErrorOccured?.Invoke(this, er);

                //
                resbowrap.BodyText = "";
                resbowrap.HTTPError = er;

            }
            catch (System.IO.IOException ex)
            {
                string errtext;
                if (ex.InnerException != null)
                {
                    Debug.WriteLine("HTTP IOException - " + ex.Message + " + 内部例外: " + ex.InnerException.Message);
                    errtext = "HTTP IOException - " + ex.Message + " + InnerException: " + ex.InnerException.Message;
                }
                else
                {
                    Debug.WriteLine("HTTP IOException - " + ex.Message);
                    errtext = "HTTP IOException - " + ex.Message;
                }

                var er = new ApiClientError
                {
                    ErrType = "HTTP " + method,
                    ErrCode = 0,
                    ErrText = errtext,
                    ErrDatetime = DateTime.Now,
                    ErrPlace = path.ToString()
                };
                // 
                ApiErrorOccured?.Invoke(this, er);

                //
                resbowrap.BodyText = "";
                resbowrap.HTTPError = er;
            }
            catch (HttpRequestException ex)
            {
                string errtext;
                if (ex.InnerException != null)
                {
                    Debug.WriteLine("HTTP HttpRequestException - " + ex.Message + " + 内部例外: " + ex.InnerException.Message);
                    errtext = "HTTP HttpRequestException - " + ex.Message + " + InnerException: " + ex.InnerException.Message;
                }
                else
                {
                    Debug.WriteLine("HTTP HttpRequestException - " + ex.Message);
                    errtext = "HTTP HttpRequestException - " + ex.Message;
                }

                var er = new ApiClientError
                {
                    ErrType = "HTTP " + method,
                    ErrCode = 0,
                    ErrText = errtext,
                    ErrDatetime = DateTime.Now,
                    ErrPlace = path.ToString()
                };
                // 
                ApiErrorOccured?.Invoke(this, er);

                //
                resbowrap.BodyText = "";
                resbowrap.HTTPError = er;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("■■■■■ HTTP GET/POST Error - Exception : " + ex.Message);

            var er = new ApiClientError
            {
                ErrType = "HTTP " + method,
                ErrCode = 0,
                ErrText = "Exception@Send: " + ex.Message,
                ErrDatetime = DateTime.Now,
                ErrPlace = path.ToString()
            };
            // 
            ApiErrorOccured?.Invoke(this, er);

            //
            resbowrap.BodyText = "";
            resbowrap.HTTPError = er;
        }
        finally
        {
            //_httpClientIsBusy = false;
        }

        return resbowrap;
    }

    public Dictionary<int, string> ApiErrorCodesDictionary = new()
    {
        {10000, "URLが存在しません"},
        {10001, "システムエラーが発生しました。サポートにお問い合わせ下さい"},
        {10002, "不正なJSON形式です。送信内容をご確認下さい"},
        {10003, "システムエラーが発生しました。サポートにお問い合わせ下さい"},
        {10005, "タイムアウトエラーが発生しました。しばらく間をおいて再度実行して下さい"},

        // Undocumented
        {10007, "ただいまメンテナンスのため一時サービスを停止しております。 今しばらくお待ちください。"},

        {20001, "API認証に失敗しました"},
        {20002, "APIキーが不正です"},
        {20003, "APIキーが存在しません"},
        {20004, "API Nonceが存在しません"},
        {20005, "APIシグネチャが存在しません"},
        {20011, "２段階認証に失敗しました"},
        {20014, "SMS認証に失敗しました"},
        {30001, "注文数量を指定して下さい"},
        {30006, "注文IDを指定して下さい"},
        {30007, "注文ID配列を指定して下さい"},
        {30009, "銘柄を指定して下さい"},
        {30012, "注文価格を指定して下さい"},
        {30013, "売買どちらかを指定して下さい"},
        {30015, "注文タイプを指定して下さい"},
        {30016, "アセット名を指定して下さい"},
        {30019, "uuidを指定して下さい"},
        {30039, "出金額を指定して下さい"},
        {40001, "注文数量が不正です"},
        {40006, "count値が不正です"},
        {40007, "終了時期が不正です"},
        {40008, "end_id値が不正です"},
        {40009, "from_id値が不正です"},
        {40013, "注文IDが不正です"},
        {40014, "注文ID配列が不正です"},
        {40015, "指定された注文が多すぎます"},
        {40017, "銘柄名が不正です"},
        {40020, "注文価格が不正です"},
        {40021, "売買区分が不正です"},
        {40022, "開始時期が不正です"},
        {40024, "注文タイプが不正です"},
        {40025, "アセット名が不正です"},
        {40028, "uuidが不正です"},
        {40048, "出金額が不正です"},
        {40112, "トリガー価格が不正です"},
        {40113, "post_only値が不正です"},
        {50003, "現在、このアカウントはご指定の操作を実行できない状態となっております。サポートにお問い合わせ下さい"},
        {50004, "現在、このアカウントは仮登録の状態となっております。アカウント登録完了後、再度お試し下さい"},
        {50005, "現在、このアカウントはロックされております。サポートにお問い合わせ下さい"},
        {50006, "現在、このアカウントはロックされております。サポートにお問い合わせ下さい"},
        {50008, "ユーザの本人確認が完了していません"},
        {50009, "ご指定の注文は存在しません"},
        {50010, "ご指定の注文はキャンセルできません"},
        {50011, "APIが見つかりません"},

        {50026, "ご指定の注文は既にキャンセル済みです"},// added 2019-10-18
        {50027, "ご指定の注文は既に約定済みです"},// added 2019-10-18

        {60001, "保有数量が不足しています"},
        {60002, "成行買い注文の数量上限を上回っています"},
        {60003, "指定した数量が制限を超えています"},
        {60004, "指定した数量がしきい値を下回っています"},
        {60005, "指定した価格が上限を上回っています"},
        {60006, "指定した価格が下限を下回っています"},
        {60011, "同時発注制限件数(30件)を上回っています"},
        {70001, "システムエラーが発生しました。サポートにお問い合わせ下さい"},
        {70002, "システムエラーが発生しました。サポートにお問い合わせ下さい"},
        {70003, "システムエラーが発生しました。サポートにお問い合わせ下さい"},
        {70004, "現在取引停止中のため、注文を承ることができません"},
        {70005, "現在買注文停止中のため、注文を承ることができません"},
        {70006, "現在売注文停止中のため、注文を承ることができません"},
        {70009, "ただいま成行注文を一時的に制限しています。指値注文をご利用ください。"},
        {70010, "ただいまシステム負荷が高まっているため、最小注文数量を一時的に引き上げています。"},
        {70011, "ただいまリクエストが混雑してます。しばらく時間を空けてから再度リクエストをお願いします。"},

        {70012, "システムエラーが発生しました。サポートにお問い合わせ下さい。"},// added 2019-10-18
        {70013, "ただいまシステム負荷が高まっているため、注文および注文キャンセルを一時的に制限しています。"},// added 2019-10-18
        {70014, "ただいまシステム負荷が高まっているため、出金申請および出金申請キャンセルを一時的に制限しています。"},// added 2019-10-18
        {70015, "ただいまシステム負荷が高まっているため、貸出申請および貸出申請キャンセルを一時的に制限しています。"},// added 2019-10-18
        {70016, "貸出申請および貸出申請キャンセル停止中のため、リクエストを承ることができません。"},// added 2019-10-18
        {70017, "指定された銘柄は注文停止中のため、リクエストを承ることができません。"},// added 2019-10-18
        {70018, "指定された銘柄は注文およびキャンセル停止中のため、リクエストを承ることができません。"},// added 2019-10-18
        {70019, "注文はキャンセル中です。"},

    };
}
