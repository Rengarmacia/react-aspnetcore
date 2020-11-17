using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace kuras.Models
{
    public class Kilometers
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public int Year { get; set; }
        public int Month { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string CarId { get; set; }
        public string NumberPlate { get; set; }
        public float StartOdometer { get; set; }
        public float EndOdometer { get; set; }
        public float KmGps { get; set; }
        public float StartFuel { get; set; }
        public float EndFuel { get; set; }
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt {get; set; } = DateTime.Now;
        [BsonElement("updatedAt")]
        public DateTime UpdatedAt {get; set; } = DateTime.Now;
        public float GetOdoKm()
        {
            return EndOdometer - StartOdometer;
        }
    }
}