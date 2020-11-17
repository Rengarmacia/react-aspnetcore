using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace kuras.Models
{
    public class Card
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("number")]
        public string Number { get; set; }
        
        [BsonElement("numberd")]
        public string NumberD { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string CarId { get; set; }
        
        [BsonRepresentation(BsonType.ObjectId)]
        public string EmployeeId { get; set; }
        
        public double Limit { get; set; }
        
        // for version control
         [BsonElement("createdAt")]
        public DateTime CreatedAt {get; set; } = DateTime.Now;
        [BsonElement("updatedAt")]
        public DateTime UpdatedAt {get; set; } = DateTime.Now;
    }
}