using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace FinApp.Models
{
    public class QuoteModel
    {
        public string Symbol { get; set; }
        public int? AverageDailyVolume { get; set; }
        public float? Change { get; set; }
        public float? DaysLow { get; set; }
        public float? DaysHigh { get; set; }
        public float? YearLow { get; set; }
        public float? YearHigh { get; set; }
        public string MarketCapitalization { get; set; }
        public float? LastTradePriceOnly { get; set; }
        public string DaysRange { get; set; }
        public string Name { get; set; }
        public string StockExchange { get; set; }

    }

    public class HistoryModel
    {
        [JsonIgnore]
        public string Symbol { get; set; }
        [BsonId]
        public ObjectId _id { get; set; }
        public DateTime Date { get; set; }
        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get; set; }
        public long Volumne { get; set; }
        public float Adj_Close { get; set; }
    }

}
