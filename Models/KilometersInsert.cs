using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace kuras.Models
{
    public class KilometersInsert
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string GpsId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string OdoId { get; set; }
    }
}