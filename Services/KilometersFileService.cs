using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using kuras.Models;

namespace kuras.Services
{
    public class KilometersFileService
    {
        private readonly IMongoCollection<KilometersFile> _carfiles;

        public KilometersFileService(IBookstoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _carfiles = database.GetCollection<KilometersFile>(settings.CarFilesCollectionName);
        }

        public List<KilometersFile> Get() =>
            _carfiles.Find(carfile => true).ToList();

        public KilometersFile Get(string id) =>
            _carfiles.Find<KilometersFile>(carfile => carfile.Id == id).FirstOrDefault();

        public KilometersFile Create(KilometersFile carfile)
        {
            _carfiles.InsertOne(carfile);
            return carfile;
        }

        public void Update(string id, KilometersFile carfileIn) =>
            _carfiles.ReplaceOne(carfile => carfile.Id == id, carfileIn);

        public void Remove(KilometersFile carfileIn) =>
            _carfiles.DeleteOne(carfile => carfile.Id == carfileIn.Id);

        public void Remove(string id) => 
            _carfiles.DeleteOne(carfile => carfile.Id == id);
    }
}