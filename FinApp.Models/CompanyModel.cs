using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;

namespace FinApp.Models
{
    public class CompanyModel
    {
        [BsonId]
        public ObjectId _id { get; set; }
        [JsonIgnore]
        public int SectorId { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
    }
}
