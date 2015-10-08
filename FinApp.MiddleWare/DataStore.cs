using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace FinApp.MiddleWare
{
    public class DataStore<T> : IDisposable
        where T : class, new()
    {
        IMongoClient client;
        IMongoDatabase database;
        string _collectionName;
        IMongoCollection<T> _collection;
        protected DataStore()
        {
            client = new MongoDB.Driver.MongoClient();
            database = client.GetDatabase("finapp");
        }

        public DataStore(string collectionName) :
            this()
        {
            _collectionName = collectionName;
            _collection = database.GetCollection<T>(_collectionName);
        }

        public void SaveOne(T model)
        {
            _collection.InsertOneAsync(model);
        }

        public void SaveMany(IEnumerable<T> list)
        {
            _collection.InsertManyAsync(list);
        }
        public void Dispose()
        {
            Dispose(true);
        }

        public List<T> GetCollection()
        {
            var find = _collection.Find<T>(new BsonDocument());
            return find.ToListAsync<T>().Result;
        }

        public void Dispose(bool suppressFinalize = false)
        {
            if (suppressFinalize)
            {
                GC.SuppressFinalize(this);
            }
            _collection = null;
            database = null;
            client = null;
        }
    }
}
