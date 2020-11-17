using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace kuras.Models
{
    public class KilometersFile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UploadedFileName { get; set; }
        public string FileName { get; set; }
        public bool IsGps { get; set; }
        public bool IsUploaded { get; set; } = false;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public KilometersFile(UploadFile file)
        {
            this.FileName = file.File.FileName;
            this.UploadedFileName = file.UploadedFileName;
            this.IsGps = file.Condition;
        }
        public KilometersFile() { }
    }
}