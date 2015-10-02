using MongoDB.Bson.Serialization.Attributes;
using System;

namespace FinApp.Models
{
    public class SectorModel
    {
        public SectorModel()
        {
            _id = Guid.NewGuid();
        }

        [BsonId]
        public Guid _id { get; set; }
        public int SectorId { get; set; }
        public string SectorName { get; set; }
        public int ParentSectorId { get; set; }
    }
}
