using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace kuras.Models
{
    public class Employee
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("fname")]
        public string Fname { get; set; }
        [BsonElement("lname")]
        public string Lname { get; set; }
        [BsonElement("emplid")]
        public string EmplId { get; set; } 
        [BsonElement("city")]
        public string City { get; set; } 
        [BsonElement("group")]
        public string Group { get; set; } 
        // for version control
         [BsonElement("createdAt")]
        public DateTime CreatedAt {get; set; } = DateTime.Now;
        [BsonElement("updatedAt")]
        public DateTime UpdatedAt {get; set; } = DateTime.Now;
    }
}