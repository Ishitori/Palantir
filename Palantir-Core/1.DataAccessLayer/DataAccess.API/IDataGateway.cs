namespace Ix.Palantir.DataAccess.API
{
    using System;
    using System.Data;
    using System.Linq;

    public interface IDataGateway : IDisposable
    {
        bool IsDisposed
        { 
            get;
        }
        bool IsTransactionStarted
        {
            get; set;
        }

        IDbConnection Connection { get; }

        IQueryable<T> GetEntities<T>() where T : class;
        void SaveEntity<T>(T entity) where T : class;
        void UpdateEntity<T>(T entity) where T : class;
        void DeleteEntity<T>(T entity) where T : class;

        void PersistChanges();
    }
}
