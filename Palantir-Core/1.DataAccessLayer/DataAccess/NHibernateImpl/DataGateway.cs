namespace Ix.Palantir.DataAccess.NHibernateImpl
{
    using System;
    using System.Data;
    using System.Linq;
    using Ix.Palantir.DataAccess.API;
    using NHibernate;
    using NHibernate.Linq;

    public class DataGateway : IDataGateway
    {
        private bool isDisposed;
        private ISession dataStorage;
        private bool isTransactionStarted;

        public DataGateway(ISession dataStorage)
        {
            this.dataStorage = dataStorage;
        }

        public bool IsDisposed
        {
            get
            {
                return this.isDisposed;
            }
        }
        public bool IsTransactionStarted
        {
            get
            {
                return this.isTransactionStarted;
            }
            set
            {
                if (value && !this.isTransactionStarted)
                {
                    this.dataStorage.BeginTransaction();
                }
                
                this.isTransactionStarted = value;
            }
        }
        public IDbConnection Connection
        {
            get { return this.dataStorage.Connection; }
        }

        public IQueryable<T> GetEntities<T>() where T : class
        {
            return this.dataStorage.Query<T>();
        }
        public void SaveEntity<T>(T entity) where T : class
        {
            this.dataStorage.SaveOrUpdate(entity);
        }
        public void UpdateEntity<T>(T entity) where T : class
        {
            this.dataStorage.SaveOrUpdate(entity);
        }
        public void DeleteEntity<T>(T entity) where T : class
        {
            this.dataStorage.Delete(entity);
        }

        public void PersistChanges()
        {
            this.dataStorage.Transaction.Commit();
        }
    
        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        public virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (this.dataStorage.Transaction.IsActive && !this.dataStorage.Transaction.WasRolledBack && !this.dataStorage.Transaction.WasCommitted)
                {
                    try
                    {
                        this.dataStorage.Transaction.Rollback();
                    }
                    catch (TransactionException)
                    {
                        // don't do anything, just dispose transaction safely
                    }
                }

                this.dataStorage.Connection.Close();
                this.dataStorage.Connection.Dispose();
                this.dataStorage.Dispose();
                this.dataStorage = null;
            }

            this.isDisposed = true;
        }
    }
}
