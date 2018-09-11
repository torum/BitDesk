using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BitDesk.Models
{
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var jsonAsset = JsonAsset.FromJson(jsonString);

    public partial class JsonAsset
    {
        [JsonProperty("success")]
        public long Success { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("assets")]
        public List<Asset> Assets { get; set; }
    }

    public partial class Asset
    {
        [JsonProperty("asset")]
        public string AssetAsset { get; set; }

        [JsonProperty("amount_precision")]
        public long AmountPrecision { get; set; }

        [JsonProperty("onhand_amount")]
        public string OnhandAmount { get; set; }

        [JsonProperty("locked_amount")]
        public string LockedAmount { get; set; }

        [JsonProperty("free_amount")]
        public string FreeAmount { get; set; }

        [JsonProperty("stop_deposit")]
        public bool StopDeposit { get; set; }

        [JsonProperty("stop_withdrawal")]
        public bool StopWithdrawal { get; set; }

        [JsonProperty("withdrawal_fee")]
        public WithdrawalFeeUnion WithdrawalFee { get; set; }
    }

    public partial class WithdrawalFeeClass
    {
        [JsonProperty("threshold")]
        public string Threshold { get; set; }

        [JsonProperty("under")]
        public string Under { get; set; }

        [JsonProperty("over")]
        public string Over { get; set; }
    }

    public partial struct WithdrawalFeeUnion
    {
        public string String;
        public WithdrawalFeeClass WithdrawalFeeClass;

        public bool IsNull => WithdrawalFeeClass == null && String == null;
    }

    public partial class JsonAsset
    {
        public static JsonAsset FromJson(string json) => JsonConvert.DeserializeObject<JsonAsset>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this JsonAsset self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                WithdrawalFeeUnionConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class WithdrawalFeeUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(WithdrawalFeeUnion) || t == typeof(WithdrawalFeeUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    return new WithdrawalFeeUnion { String = stringValue };
                case JsonToken.StartObject:
                    var objectValue = serializer.Deserialize<WithdrawalFeeClass>(reader);
                    return new WithdrawalFeeUnion { WithdrawalFeeClass = objectValue };
            }
            throw new Exception("Cannot unmarshal type WithdrawalFeeUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (WithdrawalFeeUnion)untypedValue;
            if (value.String != null)
            {
                serializer.Serialize(writer, value.String);
                return;
            }
            if (value.WithdrawalFeeClass != null)
            {
                serializer.Serialize(writer, value.WithdrawalFeeClass);
                return;
            }
            throw new Exception("Cannot marshal type WithdrawalFeeUnion");
        }

        public static readonly WithdrawalFeeUnionConverter Singleton = new WithdrawalFeeUnionConverter();
    }
}

