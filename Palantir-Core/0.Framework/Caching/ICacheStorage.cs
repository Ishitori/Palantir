namespace Ix.Palantir.Caching
{
    using System;
    using System.Collections.Generic;

    public interface ICacheStorage : IDisposable
    {
        T GetItem<T>(string key);
        IList<T> GetItems<T>(IEnumerable<string> keys);
        bool HasItem<T>(string key);
        void PutItem<T>(string key, T item, DateTime? expirationDate = null);
        void RemoveItem(string key);
        bool PutItemIfNotExists<T>(string key, T item, DateTime? expirationDate = null);
    }
}