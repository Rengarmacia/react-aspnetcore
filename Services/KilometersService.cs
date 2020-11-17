using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using kuras.Models;

namespace kuras.Services
{
    public class KilometersService
    {
        private readonly IMongoCollection<Kilometers> _kilometers;

        public KilometersService(IBookstoreDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _kilometers = database.GetCollection<Kilometers>(settings.KilometersCollectionName);
        }

        public List<Kilometers> Get() =>
            _kilometers.Find(kilometers => true).ToList();

        public Kilometers Get(string id) =>
            _kilometers.Find<Kilometers>(kilometers => kilometers.Id == id).FirstOrDefault();

        public Kilometers Create(Kilometers kilometers)
        {
            _kilometers.InsertOne(kilometers);
            return kilometers;
        }

        public void Update(string id, Kilometers kilometersIn) =>
            _kilometers.ReplaceOne(kilometers => kilometers.Id == id, kilometersIn);

        public void Remove(Kilometers kilometersIn) =>
            _kilometers.DeleteOne(kilometers => kilometers.Id == kilometersIn.Id);

        public void Remove(string id) => 
            _kilometers.DeleteOne(kilometers => kilometers.Id == id);
    }
}