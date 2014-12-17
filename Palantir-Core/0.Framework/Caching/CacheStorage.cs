namespace Ix.Palantir.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Couchbase;
    using Enyim.Caching.Memcached;
    using Ix.Palantir.Exceptions;

    public class CacheStorage : ICacheStorage
    {
        private const int CONST_DefaultCacheMin = 20;
        private readonly CouchbaseClient client;

        public CacheStorage()
        {
            this.client = new CouchbaseClient("couchbase");
        }

        public T GetItem<T>(string key)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(key));
            return this.client.Get<T>(key);
        }

        public IList<T> GetItems<T>(IEnumerable<string> keys)
        {
            Contract.Requires(keys != null);
            return null; // this.client.Get(keys);
        }

        public bool HasItem<T>(string key)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(key));
            return this.GetItem<T>(key) != null;
        }

        public void PutItem<T>(string key, T item, DateTime? expirationDate = null)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(key));
            var result = this.client.ExecuteStore(StoreMode.Set, key, item, expirationDate.HasValue ? expirationDate.Value : DateTime.UtcNow.AddMinutes(CONST_DefaultCacheMin));

            if (!result.Success)
            {
                throw new PalantirException(string.Format("Unable to put to cache an item with key {0} due to: {1}", key, result.Exception));
            }
        }
        public bool PutItemIfNotExists<T>(string key, T item, DateTime? expirationDate = null)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(key));
            bool result = this.client.Store(StoreMode.Add, key, item, expirationDate.HasValue ? expirationDate.Value : DateTime.UtcNow.AddMinutes(CONST_DefaultCacheMin));
            return result;
        }

        public void RemoveItem(string key)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(key));
            this.client.Remove(key);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.client.Dispose();
            }
        }
    }
}