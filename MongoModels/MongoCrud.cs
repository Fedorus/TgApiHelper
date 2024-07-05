using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace TelegramFaqBotHost.MongoModels
{
    public class MongoCrud1<T> where T : MongoItem
    {
        private IMongoCollection<T> _client;

        public MongoCrud1(IOptions<MongoSettings> options)
        {
            _client = new MongoClient(options.Value.ConnectionString).GetDatabase(options.Value.DatabaseName).GetCollection<T>(typeof(T).Name);
        }
        
        public async Task<IEnumerable<T>> GetAll()
        {
            var t = await _client.FindAsync(x => true);
            return t.ToEnumerable();
        }

        public async Task<T> GetAsync(string id)
        {
            var t = await _client.FindAsync(x => x.Id == id);
            return t.FirstOrDefault();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
        {
            return (await _client.FindAsync(filter)).FirstOrDefault();
        }

        public async Task<long> CountAsync()
        {
            return await _client.CountDocumentsAsync(x=> true);
        }

        public async Task<T> InsertAsync(T item)
        {
            await _client.InsertOneAsync(item);
            return item;
        }

        public async Task ReplaceOneAsync(T item)
        {
             await _client.ReplaceOneAsync(x=>x.Id == item.Id, item);
        }

        public async Task DeleteAsync(T item)
        {
            await _client.DeleteOneAsync(x => x.Id == item.Id);
        }
        public async Task DeleteAsync(string id)
        {
            await _client.DeleteOneAsync(x => x.Id == id);
        }
    }
}