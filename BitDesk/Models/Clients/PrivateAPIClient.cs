using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Globalization;
using System.Security.Cryptography;
using System.Collections.ObjectModel;
using BitDesk.Models;
using BitDesk.Common;
using BitDesk.ViewModels;

namespace BitDesk.Models.Clients
{

    #region == クラス定義 ==

    // APIリクエストの結果に含めて返すエラー情報保持クラス
    public class ErrorInfo : ViewModelBase
    {
        private string _errorTitle;
        public string ErrorTitle
        {
            get
            {
                return _errorTitle;
            }
            set
            {
                if (_errorTitle == value) return;
                _errorTitle = value;
                this.NotifyPropertyChanged("ErrorTitle");
            }
        }

        private int _errorCode;
        public int ErrorCode
        {
            get
            {
                return _errorCode;
            }
            set
            {
                if (_errorCode == value) return;
                _errorCode = value;
                this.NotifyPropertyChanged("ErrorCode");
            }
        }

        private string _errorDescription;
        public string ErrorDescription
        {
            get
            {
                return _errorDescription;
            }
            set
            {
                if (_errorDescription == value) return;
                _errorDescription = value;
                this.NotifyPropertyChanged("ErrorDescription");
            }
        }

    }

    // 資産クラス（JsonAssetClassから）
    public class Asset
    {
        public string Name { get; set; }

        public string NameText { get; set; }

        public decimal Amount { get; set; }

        public decimal FreeAmount { get; set; }

        public decimal EstimateYenAmount { get; set; }
    }

    public class Assets
    {
        public List<Asset> AssetList { get; set; }

        public Assets()
        {
            AssetList = new List<Asset>();
        }
    }

    // 取引履歴クラス
    public class Trade
    {
        public int TradeID { get; set; }
        public string Pair { get; set; }
        public int OrderID { get; set; }
        public string Side { get; set; }
        public string SideText
        {
            get
            {
                if (Side == "buy")
                {
                    return "買";
                }
                else if (Side == "sell")
                {
                    return "売";
                }
                else
                {
                    return "";
                }
            }
        }
        public string Type { get; set; }
        public string TypeText
        {
            get
            {
                if (Type == "market")
                {
                    return "成行";
                }
                else if (Type == "limit")
                {
                    return "指値";
                }
                else
                {
                    return "";
                }
            }
        }
        public Decimal Amount { get; set; }
        public Decimal Price { get; set; }
        public string MakerTaker { get; set; }
        public Decimal FeeAmountBase { get; set; }
        public Decimal FeeAmountQuote { get; set; }
        public DateTime ExecutedAt { get; set; } 
    }

    public class TradeHistory
    {
        public List<Trade> TradeList { get; set; }

        public TradeHistory()
        {
            TradeList = new List<Trade>();
        }
    }

    // 注文情報クラス（JsonOrderClassから）
    public class Order : ViewModelBase
    {
        public ulong OrderID { get; set; }
        public string Pair { get; set; } // btc_jpy, xrp_jpy, ltc_btc, eth_btc, mona_jpy, mona_btc, bcc_jpy, bcc_btc

        private string _side;// buy または sell
        public string Side
        {
            get
            {
                return _side;
            }
            set
            {
                if (_side == value) return;
                _side = value;
                this.NotifyPropertyChanged("Side");
                this.NotifyPropertyChanged("SideText");
            }
        }
        public string SideText
        {
            get
            {
                if (_side == "buy")
                {
                    return "買";
                }
                else if (_side == "sell")
                {
                    return "売";
                }
                else
                {
                    return "";
                }
            }
        }

        private string _type;// limit または market, 指値注文の場合はlimit、成行注文の場合はmarket
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                if (_type == value) return;
                _type = value;
                this.NotifyPropertyChanged("Type");
                this.NotifyPropertyChanged("TypeText");
            }
        }
        public string TypeText
        {
            get
            {
                if (_type == "market")
                {
                    return "成行";
                }
                else if (_type == "limit")
                {
                    return "指値";
                }
                else
                {
                    return "";
                }
            }
        }

        public Decimal StartAmount { get; set; } // 注文時の数量

        private Decimal _remainingAmount;// 未約定の数量
        public Decimal RemainingAmount
        {
            get
            {
                return _remainingAmount;
            }
            set
            {
                if (_remainingAmount == value) return;
                _remainingAmount = value;
                this.NotifyPropertyChanged("RemainingAmount");
            }
        }
        
        private Decimal _executedAmount;// 約定済み数量
        public Decimal ExecutedAmount
        {
            get
            {
                return _executedAmount;
            }
            set
            {
                if (_executedAmount == value) return;
                _executedAmount = value;
                this.NotifyPropertyChanged("ExecutedAmount");
            }
        }

        private Decimal _price;// 注文価格
        public Decimal Price
        {
            get
            {
                return _price;
            }
            set
            {
                if (_price == value) return;
                _price = value;
                this.NotifyPropertyChanged("Price");
            }
        }

        private Decimal _averagePrice;// 平均約定価格
        public Decimal AveragePrice
        {
            get
            {
                return _averagePrice;
            }
            set
            {
                if (_averagePrice == value) return;
                _averagePrice = value;
                this.NotifyPropertyChanged("AveragePrice");
            }
        }

        private DateTime _orderedAt;// 注文日時(UnixTimeのミリ秒)
        public DateTime OrderedAt
        {
            get
            {
                return _orderedAt;
            }
            set
            {
                if (_orderedAt == value) return;
                _orderedAt = value;
                this.NotifyPropertyChanged("OrderedAt");
            }
        }

        private string _status;// 注文ステータス  -  UNFILLED 注文中, PARTIALLY_FILLED 注文中(一部約定), FULLY_FILLED 約定済み, CANCELED_UNFILLED 取消済, CANCELED_PARTIALLY_FILLED 取消済(一部約定)
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                if (_status == value) return;
                _status = value;

                this.NotifyPropertyChanged("Status");
                this.NotifyPropertyChanged("StatusText");
                this.NotifyPropertyChanged("IsCanceEnabled");
                this.NotifyPropertyChanged("OrderIsDone");
                            }
        }
        public string StatusText
        {
            get
            {
                if (_status == "UNFILLED")
                {
                    return "注文中";
                }
                else if (_status == "PARTIALLY_FILLED")
                {
                    return "注文中(一部約定)";
                }
                else if (_status == "FULLY_FILLED")
                {
                    return "約定済み";
                }
                else if (_status == "CANCELED_UNFILLED")
                {
                    return "取消済";
                }
                else if (_status == "CANCELED_PARTIALLY_FILLED")
                {
                    return "取消済(一部約定)";
                }
                else
                {
                    return "";
                }
            }
        }

        // 現在値差額表示
        private Decimal _shushi;
        public Decimal Shushi
        {
            get
            {
                return _shushi;
            }
            set
            {
                if (_shushi == value) return;
                _shushi = value;
                this.NotifyPropertyChanged("Shushi");
            }
        }

        // 実質価格
        private Decimal _actualPrice;
        public Decimal ActualPrice
        {
            get
            {
                return _actualPrice;
            }
            set
            {
                if (_actualPrice == value) return;
                _actualPrice = value;
                this.NotifyPropertyChanged("ActualPrice");
            }
        }

        // キャンセルが効くかどうかのフラグ
        public bool IsCancelEnabled
        {
            get
            {
                if ((_status == "UNFILLED") || (_status == "PARTIALLY_FILLED"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private ErrorInfo _err;
        public ErrorInfo Err
        {
            get
            {
                return _err;
            }
            set
            {
                if (_err == value) return;
                _err = value;
                this.NotifyPropertyChanged("Err");
            }
        }

        public bool _hasErrorInfo;
        public bool HasErrorInfo
        {
            get
            {
                return _hasErrorInfo;
            }
            set
            {
                if (_hasErrorInfo == value) return;
                _hasErrorInfo = value;
                this.NotifyPropertyChanged("HasErrorInfo");
            }
        }

        public bool OrderIsDone
        {
            get
            {
                if (_status == "UNFILLED")
                {
                    return false;
                }
                else if (_status == "PARTIALLY_FILLED")
                {
                    return false;
                }
                else if (_status == "FULLY_FILLED")
                {
                    return true;
                }
                else if (_status == "CANCELED_UNFILLED")
                {
                    return true;
                }
                else if (_status == "CANCELED_PARTIALLY_FILLED")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public Order()
        {
            Err = new ErrorInfo();
        }

    }

    public class Orders
    {
        public List<Order> OrderList { get; set; }

        public Orders()
        {
            OrderList = new List<Order>();
        }
    }

    public class OrderResult : Order
    {
        public bool IsSuccess { get; set; }
        public int ErrorCode { get; set; }
    }

    // 発注時クエリパラメーター用クラス Jsonシリアライズ用
    [JsonObject]
    public class OrderParam
    {
        [JsonProperty("pair")]
        public string Pair { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("side")]
        public string Side { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        public OrderParam(string pair, string amount, string price, string side, string type)
        {
            this.Pair = pair;
            this.Amount = amount;
            this.Price = price;
            this.Side = side;
            this.Type = type;
        }
    }

    // パラメーター用クラス Jsonシリアライズ用
    [JsonObject]
    public class PairOrderIdParam
    {
        [JsonProperty("pair")]
        public string pair { get; set; }

        [JsonProperty("order_id")]
        public ulong order_id { get; set; }

        public PairOrderIdParam(string pair, ulong orderID)
        {
            this.pair = pair;
            this.order_id = orderID;
        }
    }

    // パラメーター用クラス Jsonシリアライズ用
    [JsonObject]
    public class PairOrderIdList
    {
        [JsonProperty("pair")]
        public string Pair { get; set; }

        [JsonProperty("order_ids")]
        public List<ulong> OrderIds { get; set; }

        public PairOrderIdList(string pair, List<ulong> orderIds)
        {
            this.Pair = pair;
            this.OrderIds = orderIds;
        }
    }

    #endregion

    class PrivateAPIClient : BaseClient
    {
        #region == 変数宣言 ==

        // プライベートAPI　エラーコード
        public Dictionary<long, string> ApiErrorCodesDictionary = new Dictionary<long, string>()
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
                {50003, "現在、このアカウントはご指定の操作を実行できない状態となっております。サポートにお問い合わせ下さい"},
                {50004, "現在、このアカウントは仮登録の状態となっております。アカウント登録完了後、再度お試し下さい"},
                {50005, "現在、このアカウントはロックされております。サポートにお問い合わせ下さい"},
                {50006, "現在、このアカウントはロックされております。サポートにお問い合わせ下さい"},
                {50008, "ユーザの本人確認が完了していません"},
                {50009, "ご指定の注文は存在しません"},
                {50010, "ご指定の注文はキャンセルできません"},
                {50011, "APIが見つかりません"},
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
            };

        // プライベートAPIのベースURL
        private readonly Uri PrivateAPIUri = new Uri("https://api.bitbank.cc/v1");

        // デリゲート
        public delegate void ClinetErrorEvent(PrivateAPIClient sender, ClientError err);

        // エラーイベント
        public event ClinetErrorEvent ErrorOccured;

        #endregion

        // コンストラクタ
        public PrivateAPIClient()
        {
            _HTTPConn.Client.BaseAddress = PrivateAPIUri;
        }

        #region == メソッド ==

        // 資産残高取得メソッド
        public async Task<Assets> GetAssetList(string _ApiKey, string _ApiSecret)
        {

            Uri path = new Uri("/user/assets", UriKind.Relative);

            string json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Get);

            if (!string.IsNullOrEmpty(json))
            {

                var deserialized = JsonAsset.FromJson(json);

                if (deserialized.Success > 0)
                {

                    Assets asts = new Assets();

                    foreach (var ast in deserialized.Data.Assets)
                    {
                        asts.AssetList.Add(new Asset {
                            Name =ast.AssetAsset,
                            Amount = decimal.Parse(ast.OnhandAmount),
                            FreeAmount = decimal.Parse(ast.FreeAmount)
                        });

                    }

                    return asts;
                }
                else
                {
                    var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                    System.Diagnostics.Debug.WriteLine("■■■■■ GetAssetList: API error code - " + jsonResult.data.code.ToString() + " ■■■■■");
                    
                    // エラーイベント発火
                    ClientError er = new ClientError();
                    er.ErrType = "API";
                    er.ErrCode = jsonResult.data.code;
                    if (ApiErrorCodesDictionary.ContainsKey(jsonResult.data.code))
                    {
                        er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult.data.code] + "」";
                    }
                    er.ErrDatetime = DateTime.Now;
                    er.ErrPlace = path.ToString();

                    ErrorOccured?.Invoke(this, er);

                    return null;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetAssetList: Send returned NULL.");
                return null;
            }

        }

        // 注文発注メソッド
        public async Task<OrderResult> MakeOrder(string _ApiKey, string _ApiSecret, string pair, Decimal amount, Decimal price, string side, string type)
        {
            //パラメータ 
            // https://docs.bitbank.cc/#/Order
            /*
                { "pair", "btc_jpy" },//取引する通貨の種類
                { "amount", "0.01" },//ビットコインの注文量
                { "price", "100000" },//ビットコインのレート
                { "side", "buy" },//注文の売買の種類（買い:buy, 売り:sell）
                { "type", "limit" },//指値注文の場合はlimit、成行注文の場合はmarket）
            */
            //System.Diagnostics.Debug.WriteLine("MakingOrder...");

            /*
            ///////////////////////////////////
            // test data
            Order ord = new Order();
            ord.orderID = 1234;
            ord.pair = pair;
            ord.side = side;
            ord.type = type;
            ord.startAmount = amount;
            ord.remainingAmount = 0.001M;
            ord.executedAmount = 0.001M;
            ord.price = price;
            ord.averagePrice = 840001M;
            ord.orderedAt = DateTime.Now;
            ord.status = "UNFILLED";

            return ord;
            ///////////////////////////////////
            */

            Uri path = new Uri("/user/spot/order", UriKind.Relative);//APIの通信URL

            var orderParam = new OrderParam(pair, amount.ToString(), price.ToString(), side, type);

            var body = JsonConvert.SerializeObject(orderParam);

            //System.Diagnostics.Debug.WriteLine("MakingOrder... resquest body = " + body);

            try
            { 
                string json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Post, body, null);

                if (!string.IsNullOrEmpty(json))
                {
                    //System.Diagnostics.Debug.WriteLine("MakeOrder result: " + json);

                    var deserialized = JsonConvert.DeserializeObject<JsonOrderObject>(json);

                    OrderResult ord = new OrderResult();

                    if (deserialized.success > 0)
                    {
                        ord.OrderID = deserialized.data.order_id;
                        ord.Pair = deserialized.data.pair;
                        ord.Side = deserialized.data.side;
                        ord.Type = deserialized.data.type;

                        ord.StartAmount = decimal.Parse(deserialized.data.start_amount);
                        ord.RemainingAmount = decimal.Parse(deserialized.data.remaining_amount);
                        ord.ExecutedAmount = decimal.Parse(deserialized.data.executed_amount);
                        if (deserialized.data.price != null) { 
                            ord.Price = decimal.Parse(deserialized.data.price);
                        }
                        ord.AveragePrice = decimal.Parse(deserialized.data.average_price);

                        DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        DateTime date = start.AddMilliseconds(deserialized.data.ordered_at).ToLocalTime();
                        ord.OrderedAt = date;

                        ord.Status = deserialized.data.status;

                        ord.IsSuccess = true;
                        ord.HasErrorInfo = false;

                        return ord;
                    }
                    else
                    {
                        var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                        ord.IsSuccess = false;
                        ord.ErrorCode = jsonResult.data.code;

                        System.Diagnostics.Debug.WriteLine("■■■■■ MakingOrder: API error code - " + jsonResult.data.code.ToString() + " ■■■■■");

                        // ユーザに表示するエラー情報
                        ord.HasErrorInfo = true;
                        ord.Err.ErrorCode = ord.ErrorCode;
                        ord.Err.ErrorTitle = "発注処理でエラーが返りました。";

                        if (ApiErrorCodesDictionary.ContainsKey(ord.ErrorCode))
                        {
                            ord.Err.ErrorDescription = ApiErrorCodesDictionary[ord.ErrorCode];
                        }
                        ord.Err.ErrorDescription = ord.Err.ErrorDescription + "(エラーコード：" + ord.ErrorCode.ToString() + ")";

                        // エラーイベント発火
                        ClientError er = new ClientError();
                        er.ErrType = "API";
                        er.ErrCode = jsonResult.data.code;
                        if (ApiErrorCodesDictionary.ContainsKey(jsonResult.data.code))
                        {
                            er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult.data.code] + "」";
                        }
                        er.ErrDatetime = DateTime.Now;
                        er.ErrPlace = path.ToString();

                        ErrorOccured?.Invoke(this, er);

                        return ord;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ MakeOrder: Send returned NULL.");
                    return null;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ MakeOrder Exception: " + e + " ■■■■■");
                return null;
            }
        }

        // 注文情報をIDから取得メソッド
        public async Task<OrderResult> GetOrderByID(string _ApiKey, string _ApiSecret, string pair, int orderID)
        {

            Uri path = new Uri("/user/spot/order", UriKind.Relative);

            var param = new Dictionary<string, string>{
                { "pair", pair },//取引する通貨の種類
                { "order_id", orderID.ToString() },
            };

            string json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Get, "", param);

            if (!string.IsNullOrEmpty(json))
            {
                //System.Diagnostics.Debug.WriteLine("GetOrderByID: " + json);

                var deserialized = JsonConvert.DeserializeObject<JsonOrderObject>(json);

                if (deserialized.success > 0)
                {
                    OrderResult ord = new OrderResult();
                    ord.IsSuccess = true;

                    ord.OrderID = deserialized.data.order_id;
                    ord.Pair = deserialized.data.pair;
                    ord.Side = deserialized.data.side;
                    ord.Type = deserialized.data.type;

                    ord.StartAmount = decimal.Parse(deserialized.data.start_amount);
                    ord.RemainingAmount = decimal.Parse(deserialized.data.remaining_amount);
                    ord.ExecutedAmount = decimal.Parse(deserialized.data.executed_amount);
                    if (deserialized.data.price != null)
                    {
                        ord.Price = decimal.Parse(deserialized.data.price);
                    }
                    ord.AveragePrice = decimal.Parse(deserialized.data.average_price);

                    DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    DateTime date = start.AddMilliseconds(deserialized.data.ordered_at).ToLocalTime();
                    ord.OrderedAt = date;

                    ord.Status = deserialized.data.status;

                    return ord;
                }
                else
                {
                    OrderResult ord = new OrderResult();

                    var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                    ord.IsSuccess = false;
                    ord.ErrorCode = jsonResult.data.code;

                    System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderByID: API error code - " + jsonResult.data.code.ToString() + " ■■■■■");

                    // ユーザに表示するエラー情報
                    ord.HasErrorInfo = true;
                    ord.Err.ErrorCode = ord.ErrorCode;
                    ord.Err.ErrorTitle = "注文情報を取得中にエラーが返りました。";

                    if (ApiErrorCodesDictionary.ContainsKey(ord.ErrorCode))
                    {
                        ord.Err.ErrorDescription = ApiErrorCodesDictionary[ord.ErrorCode];
                    }
                    ord.Err.ErrorDescription = ord.Err.ErrorDescription + "(エラーコード：" + ord.ErrorCode.ToString() + ")";

                    // エラーイベント発火
                    ClientError er = new ClientError();
                    er.ErrType = "API";
                    er.ErrCode = jsonResult.data.code;
                    if (ApiErrorCodesDictionary.ContainsKey(jsonResult.data.code))
                    {
                        er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult.data.code] + "」";
                    }
                    er.ErrDatetime = DateTime.Now;
                    er.ErrPlace = path.ToString();

                    ErrorOccured?.Invoke(this, er);

                    return ord;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderByID: Send returned NULL.");
                return null;
            }


        }

        // 注文情報を複数のIDから取得メソッド
        public async Task<Orders> GetOrderListByIDs(string _ApiKey, string _ApiSecret, string pair, List<ulong> orderIDs)
        {

            Uri path = new Uri("/user/spot/orders_info", UriKind.Relative);

            var idParam = new PairOrderIdList(pair, orderIDs);
            var body = JsonConvert.SerializeObject(idParam);

            string json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Post, body);

            try
            {
                if (!string.IsNullOrEmpty(json))
                {
                    //System.Diagnostics.Debug.WriteLine("GetOrderListByIDs: " + json);

                    var deserialized = JsonConvert.DeserializeObject<JsonOrderInfoObject>(json);

                    if (deserialized.success > 0)
                    {
                        Orders ords = new Orders();

                        foreach (var ord in deserialized.data.orders)
                        {
                            try
                            {
                                Order o = new Order
                                {
                                    OrderID = ord.order_id,
                                    Pair = ord.pair,
                                    Side = ord.side,
                                    Type = ord.type,
                                    OrderedAt = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(ord.ordered_at).ToLocalTime(),
                                    Status = ord.status
                                };

                                if (!string.IsNullOrEmpty(ord.start_amount))
                                    o.StartAmount = decimal.Parse(ord.start_amount);
                                if (!string.IsNullOrEmpty(ord.remaining_amount))
                                    o.RemainingAmount = decimal.Parse(ord.remaining_amount);
                                if (!string.IsNullOrEmpty(ord.executed_amount))
                                    o.ExecutedAmount = decimal.Parse(ord.executed_amount);
                                if (!string.IsNullOrEmpty(ord.price))
                                    o.Price = decimal.Parse(ord.price);
                                if (!string.IsNullOrEmpty(ord.average_price))
                                    o.AveragePrice = decimal.Parse(ord.average_price);

                                ords.OrderList.Add(o);

                                //System.Diagnostics.Debug.WriteLine("GetOrderListByIDs ord.status: " + ord.status);
                            }
                            catch
                            {
                                System.Diagnostics.Debug.WriteLine("■■■■■ GGetOrderListByIDs - ords.OrderList.Add ■■■■■");
                            }
                        }

                        return ords;
                    }
                    else
                    {
                        var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                        System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderListByIDs: API error code - " + jsonResult.data.code.ToString() + " ■■■■■");

                        // エラーイベント発火
                        ClientError er = new ClientError();
                        er.ErrType = "API";
                        er.ErrCode = jsonResult.data.code;
                        if (ApiErrorCodesDictionary.ContainsKey(jsonResult.data.code))
                        {
                            er.ErrText = "「"+ ApiErrorCodesDictionary[jsonResult.data.code] + "」";
                        }
                        er.ErrDatetime = DateTime.Now;
                        er.ErrPlace = path.ToString();
                        er.ErrPlaceParent = "GetOrderListByIDs";
                        er.ErrEx = "注文情報を更新出来ませんでした";

                        ErrorOccured?.Invoke(this, er);

                        return null;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderListByIDs: Send returned NULL.");
                    return null;
                }
            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderListByIDs: Exception "+e);
                return null;
            }

        }

        // 注文キャンセルメソッド
        public async Task<OrderResult> CancelOrder(string _ApiKey, string _ApiSecret, string pair, ulong orderID)
        {

            Uri path = new Uri("/user/spot/cancel_order", UriKind.Relative);

            var cancelOrderParam = new PairOrderIdParam(pair, orderID);

            var body = JsonConvert.SerializeObject(cancelOrderParam);

            string json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Post, body);

            if (!string.IsNullOrEmpty(json))
            {
                //System.Diagnostics.Debug.WriteLine("CancelOrder: " + json);

                var deserialized = JsonConvert.DeserializeObject<JsonOrderObject>(json);

                OrderResult ord = new OrderResult();

                if (deserialized.success > 0)
                {

                    ord.OrderID = deserialized.data.order_id;
                    ord.Pair = deserialized.data.pair;
                    ord.Side = deserialized.data.side;
                    ord.Type = deserialized.data.type;

                    //TODO エラーハンドリング
                    ord.StartAmount = decimal.Parse(deserialized.data.start_amount);
                    ord.RemainingAmount = decimal.Parse(deserialized.data.remaining_amount);
                    ord.ExecutedAmount = decimal.Parse(deserialized.data.executed_amount);
                    ord.Price = decimal.Parse(deserialized.data.price);
                    ord.AveragePrice = decimal.Parse(deserialized.data.average_price);

                    DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    DateTime date = start.AddMilliseconds(deserialized.data.ordered_at).ToLocalTime();
                    ord.OrderedAt = date;

                    ord.Status = deserialized.data.status;

                    ord.IsSuccess = true;
                    ord.HasErrorInfo = false;

                    return ord;
                }
                else
                {
                    var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                    ord.IsSuccess = false;
                    ord.ErrorCode = jsonResult.data.code;

                    // ユーザに表示するエラー情報
                    ord.HasErrorInfo = true;
                    ord.Err.ErrorCode = ord.ErrorCode;
                    ord.Err.ErrorTitle = "注文キャンセル処理でエラーが返りました。";

                    if (ApiErrorCodesDictionary.ContainsKey(ord.ErrorCode))
                    {
                        ord.Err.ErrorDescription = ApiErrorCodesDictionary[ord.ErrorCode];
                    }
                    ord.Err.ErrorDescription = ord.Err.ErrorDescription + "(エラーコード：" + ord.ErrorCode.ToString() + ")";


                    // エラーイベント発火
                    ClientError er = new ClientError();
                    er.ErrType = "API";
                    er.ErrCode = jsonResult.data.code;
                    if (ApiErrorCodesDictionary.ContainsKey(jsonResult.data.code))
                    {
                        er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult.data.code] + "」";
                    }
                    er.ErrDatetime = DateTime.Now;
                    er.ErrPlace = path.ToString();

                    ErrorOccured?.Invoke(this, er);

                    System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrder: API error code - " + jsonResult.data.code.ToString() + " ■■■■■");

                    return ord;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrder: Send returned NULL.");
                return null;
            }

        }

        // 文キャンセル（複数）メソッド
        public async Task<Orders> CancelOrders(string _ApiKey, string _ApiSecret, string pair, List<ulong> orderIDs)
        {

            Uri path = new Uri("/user/spot/cancel_orders", UriKind.Relative);

            var idParam = new PairOrderIdList(pair, orderIDs);
            var body = JsonConvert.SerializeObject(idParam);

            string json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Post, body);

            try
            {
                if (!string.IsNullOrEmpty(json))
                {
                    //System.Diagnostics.Debug.WriteLine("GetOrderListByIDs: " + json);

                    var deserialized = JsonConvert.DeserializeObject<JsonOrderInfoObject>(json);

                    if (deserialized.success > 0)
                    {
                        Orders ords = new Orders();

                        foreach (var ord in deserialized.data.orders)
                        {
                            try
                            {
                                Order o = new Order
                                {
                                    OrderID = ord.order_id,
                                    Pair = ord.pair,
                                    Side = ord.side,
                                    Type = ord.type,
                                    OrderedAt = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(ord.ordered_at).ToLocalTime(),
                                    Status = ord.status
                                };

                                if (!string.IsNullOrEmpty(ord.start_amount))
                                    o.StartAmount = decimal.Parse(ord.start_amount);
                                if (!string.IsNullOrEmpty(ord.remaining_amount))
                                    o.RemainingAmount = decimal.Parse(ord.remaining_amount);
                                if (!string.IsNullOrEmpty(ord.executed_amount))
                                    o.ExecutedAmount = decimal.Parse(ord.executed_amount);
                                if (!string.IsNullOrEmpty(ord.price))
                                    o.Price = decimal.Parse(ord.price);
                                if (!string.IsNullOrEmpty(ord.average_price))
                                    o.AveragePrice = decimal.Parse(ord.average_price);

                                ords.OrderList.Add(o);

                                //System.Diagnostics.Debug.WriteLine("GetOrderListByIDs ord.status: " + ord.status);
                            }
                            catch
                            {
                                System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrders - ords.OrderList.Add ■■■■■");
                            }


                        }

                        return ords;
                    }
                    else
                    {
                        var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                        System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrders: API error code - " + jsonResult.data.code.ToString() + " ■■■■■");

                        // エラーイベント発火
                        ClientError er = new ClientError();
                        er.ErrType = "API";
                        er.ErrCode = jsonResult.data.code;
                        if (ApiErrorCodesDictionary.ContainsKey(jsonResult.data.code))
                        {
                            er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult.data.code] + "」";
                        }
                        er.ErrDatetime = DateTime.Now;
                        er.ErrPlace = path.ToString();

                        ErrorOccured?.Invoke(this, er);

                        return null;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrders: Send returned NULL.");
                    return null;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrders: Exception " + e);
                return null;
            }

        }

        // 注文リスト取得メソッド
        public async Task<Orders> GetOrderList(string _ApiKey, string _ApiSecret, string pair)
        {

            Uri path = new Uri("/user/spot/active_orders", UriKind.Relative);

            var param = new Dictionary<string, string>{
                { "pair", pair },//取引する通貨の種類
                //{ "count", "10" },//取得する注文数（double）
                //{ "from_id", "100000" },//取得開始注文ID
                //{ "end_id", "100000" },//取得終了注文ID
                //{ "since", "100000" },//開始UNIXタイムスタンプ
                //{ "end", "0.01" },//終了UNIXタイムスタンプ
            };

            string json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Get,"", param);

            if (!string.IsNullOrEmpty(json))
            {
                //System.Diagnostics.Debug.WriteLine("■■■■■ ■■■■■ ■■■■■ GetOrderList: " + json);

                var deserialized = JsonConvert.DeserializeObject<JsonOrderInfoObject>(json);

                if (deserialized.success > 0)
                {
                    Orders ords = new Orders();

                    foreach (var ord in deserialized.data.orders)
                    {

                        Order o = new Order
                        {
                            OrderID = ord.order_id,
                            Pair = ord.pair,
                            Side = ord.side,
                            Type = ord.type,
                            OrderedAt = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(ord.ordered_at).ToLocalTime(),
                            Status = ord.status
                        };

                        if (!string.IsNullOrEmpty(ord.start_amount)) o.StartAmount = decimal.Parse(ord.start_amount);
                        if (!string.IsNullOrEmpty(ord.remaining_amount)) o.RemainingAmount = decimal.Parse(ord.remaining_amount);
                        if (!string.IsNullOrEmpty(ord.executed_amount)) o.ExecutedAmount = decimal.Parse(ord.executed_amount);
                        if (!string.IsNullOrEmpty(ord.price)) { o.Price = decimal.Parse(ord.price); } else { o.Price = 0; }
                        if (!string.IsNullOrEmpty(ord.average_price)) o.AveragePrice = decimal.Parse(ord.average_price);

                        ords.OrderList.Add(o);

                    }

                    return ords;
                }
                else
                {
                    var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                    System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderList: API error code - " + jsonResult.data.code.ToString() + " ■■■■■");

                    // エラーイベント発火
                    ClientError er = new ClientError();
                    er.ErrType = "API";
                    er.ErrCode = jsonResult.data.code;
                    if (ApiErrorCodesDictionary.ContainsKey(jsonResult.data.code))
                    {
                        er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult.data.code] + "」";
                    }
                    er.ErrDatetime = DateTime.Now;
                    er.ErrPlace = path.ToString();

                    ErrorOccured?.Invoke(this, er);

                    return null;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderList: Send returned NULL.");
                return null;
            }
        }
        
        // 取引履歴取得メソッド
        public async Task<TradeHistory> GetTradeHistory(string _ApiKey, string _ApiSecret, string pair)
        {

            Uri path = new Uri("/user/spot/trade_history", UriKind.Relative);

            var param = new Dictionary<string, string>{
                { "pair", pair },//取得する約定数
                { "count", "10" },//取得する注文数（double）
                //{ "order_id", "100000" },//注文ID
                //{ "since", "100000" },//開始UNIXタイムスタンプ
                //{ "end", "0.01" },//終了UNIXタイムスタンプ
                //{ "order", "asc" },//約定時刻順序(asc: 昇順、desc: 降順、デフォルト降順)
            };

            string json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Get, "", param);

            if (!string.IsNullOrEmpty(json))
            {
                //System.Diagnostics.Debug.WriteLine("GetTradeHistory: " + json);
                
                var deserialized = JsonConvert.DeserializeObject<JsonTradeHistoryObject>(json);

                if (deserialized.success > 0)
                {
                    TradeHistory history = new TradeHistory();

                    foreach (var trd in deserialized.data.trades)
                    {
                      
                        Trade o = new Trade
                        {
                            TradeID = trd.trade_id,
                            OrderID = trd.order_id,
                            Pair = trd.pair,
                            Side = trd.side,
                            Type = trd.type,
                            MakerTaker = trd.maker_taker,
                            ExecutedAt = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(trd.executed_at).ToLocalTime(),

                        };
                        
                        if (!string.IsNullOrEmpty(trd.amount)) o.Amount = decimal.Parse(trd.amount);
                        if (!string.IsNullOrEmpty(trd.price)) { o.Price = decimal.Parse(trd.price); } else { o.Price = 0; }

                        if (!string.IsNullOrEmpty(trd.amount)) o.FeeAmountBase = decimal.Parse(trd.fee_amount_base);
                        if (!string.IsNullOrEmpty(trd.amount)) o.FeeAmountQuote = decimal.Parse(trd.fee_amount_quote);

                        history.TradeList.Add(o);

                    }

                    return history;
                }
                else
                {
                    var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                    System.Diagnostics.Debug.WriteLine("■■■■■ GetTradeHistory: API error code - " + jsonResult.data.code.ToString() + " ■■■■■");

                    // エラーイベント発火
                    ClientError er = new ClientError();
                    er.ErrType = "API";
                    er.ErrCode = jsonResult.data.code;
                    if (ApiErrorCodesDictionary.ContainsKey(jsonResult.data.code))
                    {
                        er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult.data.code] + "」";
                    }
                    er.ErrDatetime = DateTime.Now;
                    er.ErrPlace = path.ToString();

                    ErrorOccured?.Invoke(this, er);

                    return null;
                }
                
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetTradeHistory: Send returned NULL.");
                return null;
            }

        }

        // HTTPリクエスト送信メソッド
        internal async Task<string> Send(Uri path, string apiKey, string secret, HttpMethod method, string body = "", Dictionary<string, string> queries = null)
        {

            try {

                //ACCESS-NONCE
                string _accessNonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(); //NONCE=unixtime

                // メッセージを作成
                string message = "";
                if (method == HttpMethod.Get)
                {
                    if (queries != null)
                    {
                        // パラメータ文字列を作成
                        var pms = new FormUrlEncodedContent(queries);
                        string param = await pms.ReadAsStringAsync();

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
                byte[] hash = new HMACSHA256(Encoding.UTF8.GetBytes(secret)).ComputeHash(Encoding.UTF8.GetBytes(message));
                //ACCESS-SIGNATURE
                string _accessSignature = BitConverter.ToString(hash).ToLower().Replace("-", "");//バイト配列をを16進文字列へ

                //System.Diagnostics.Debug.WriteLine("Sending..." + Environment.NewLine + _HTTPConn.Client.DefaultRequestHeaders.ToString());

                HttpResponseMessage res;
                if (method == HttpMethod.Post)
                {
                    var content = new StringContent(body, Encoding.UTF8, "application/json");

                    content.Headers.Add("ACCESS-KEY", apiKey);
                    content.Headers.Add("ACCESS-NONCE", _accessNonce);
                    content.Headers.Add("ACCESS-SIGNATURE", _accessSignature);

                    res = await _HTTPConn.Client.PostAsync(_HTTPConn.Client.BaseAddress.ToString() + path.ToString(), content);
                }
                else if (method == HttpMethod.Get)
                {

                    if (queries == null)
                    {
                        //
                        var requestMessage = new HttpRequestMessage(HttpMethod.Get, _HTTPConn.Client.BaseAddress.ToString() + path.ToString());
                        requestMessage.Headers.Add("ACCESS-KEY", apiKey);
                        requestMessage.Headers.Add("ACCESS-NONCE", _accessNonce);
                        requestMessage.Headers.Add("ACCESS-SIGNATURE", _accessSignature);
                        
                        res = await _HTTPConn.Client.SendAsync(requestMessage);
                    }
                    else
                    {
                        var pms = new FormUrlEncodedContent(queries);
                        string param = await pms.ReadAsStringAsync();

                        //
                        var requestMessage = new HttpRequestMessage(HttpMethod.Get, _HTTPConn.Client.BaseAddress.ToString() + path.ToString() + "?" + param);
                        requestMessage.Headers.Add("ACCESS-KEY", apiKey);
                        requestMessage.Headers.Add("ACCESS-NONCE", _accessNonce);
                        requestMessage.Headers.Add("ACCESS-SIGNATURE", _accessSignature);

                        res = await _HTTPConn.Client.SendAsync(requestMessage);
                    }
                }
                else
                {
                    throw new ArgumentException("method は POST か GET を指定してください。", method.ToString());
                }


                try
                {
                    //返答内容を取得
                    string text = await res.Content.ReadAsStringAsync();

                    //通信上の失敗
                    if (!res.IsSuccessStatusCode)
                    {
                        //System.Diagnostics.Debug.WriteLine("■■■■■■■■ Send: HTTP Error " + res.StatusCode.ToString() + " " + method + " " + _HTTPConn.Client.BaseAddress.ToString() + path.ToString() + " ■■■■■■■");
                                               
                        ClientError er = new ClientError();
                        er.ErrType = "HTTP " + method;
                        er.ErrCode = (int)res.StatusCode;
                        er.ErrText = res.StatusCode.ToString();
                        er.ErrDatetime = DateTime.Now;
                        er.ErrPlace = path.ToString();

                        ErrorOccured?.Invoke(this, er);

                        return "";
                    }

                    return text;
                }
                catch (HttpRequestException ex)
                {
                    System.Diagnostics.Debug.WriteLine("HTTP Send: HttpRequestException - " + ex.Message + " + 内部例外: " + ex.InnerException.Message);

                    return "";
                }


            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ HTTP GET/POST Error - Exception : " + ex.Message);
                return "";
            }
            finally
            {
                //_httpClientIsBusy = false;
            }



        }

        #endregion
    }

}
