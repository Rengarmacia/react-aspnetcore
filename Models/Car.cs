using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace kuras.Models
{
    public class Car
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("numberplate")]
        public string Numberplate { get; set; }
        [BsonElement("carmodel")]
        public string Carmodel { get; set; }
        [BsonElement("make")]
        public string Make { get; set; } 
        [BsonElement("createdAt")]
        public DateTime CreatedAt {get; set; } = DateTime.Now;
        [BsonElement("updatedAt")]
        public DateTime UpdatedAt {get; set; } = DateTime.Now;
    }
}