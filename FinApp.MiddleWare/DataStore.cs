using MongoDB.Driver;
using System;

namespace FinApp.MiddleWare
{
    public class DataStore : IDisposable
    {
        IMongoClient client;
        IMongoDatabase database;
        string _collectionName;
        protected DataStore()
        {
            client = new MongoDB.Driver.MongoClient();
            database = client.GetDatabase("finapp");
        }

        public DataStore(string collectionName) :
            this()
        {
            _collectionName = collectionName;
        }

        public void SaveOne<T>(T model)
        {
            database.GetCollection<T>(_collectionName);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Dispose(bool suppressFinalize = false)
        {
            if (suppressFinalize)
            {
                GC.SuppressFinalize(this);
            }

        }
    }
}
