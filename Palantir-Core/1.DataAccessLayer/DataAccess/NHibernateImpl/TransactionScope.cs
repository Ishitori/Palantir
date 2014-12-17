namespace Ix.Palantir.DataAccess.NHibernateImpl
{
    using System;
    using Ix.Palantir.DataAccess.API;

    public class TransactionScope : ITransactionScope
    {
        private readonly IPersistentDataGatewayProvider gatewayProvider;

        private bool isDisposed;
        private bool transactionInitiator;

        public TransactionScope(IPersistentDataGatewayProvider gatewayProvider)
        {
            this.gatewayProvider = gatewayProvider;
        }

        public ITransactionScope Begin()
        {
            var dataGateway = this.gatewayProvider.GetDurableDataGateway();

            if (!dataGateway.IsTransactionStarted)
            {
                dataGateway.IsTransactionStarted = true;
                this.transactionInitiator = true;
            }

            return this;
        }

        public void Commit()
        {
            var dataGateway = this.gatewayProvider.GetDurableDataGateway();

            if (dataGateway.IsDisposed || !dataGateway.IsTransactionStarted || !this.transactionInitiator)
            {
                return;
            }

            dataGateway.PersistChanges();
            this.transactionInitiator = false;
            dataGateway.IsTransactionStarted = false;
        }
        public void Rollback()
        {
            var dataGateway = this.gatewayProvider.GetDurableDataGateway();

            if (dataGateway.IsDisposed || !dataGateway.IsTransactionStarted || !this.transactionInitiator)
            {
                return;
            }

            this.Dispose(true);
        }
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool isDisposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                var dataGateway = this.gatewayProvider.GetDurableDataGateway();
                this.transactionInitiator = false;
                dataGateway.IsTransactionStarted = false;
            }

            this.isDisposed = true;
        }
    }
}