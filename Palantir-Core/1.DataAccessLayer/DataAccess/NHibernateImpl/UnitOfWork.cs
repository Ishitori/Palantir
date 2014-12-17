namespace Ix.Palantir.DataAccess.NHibernateImpl
{
    using System;
    using Ix.Palantir.DataAccess.API;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDurableDataGateway gateway;
        private readonly IPersistentDataGatewayProvider gatewayProvider;
        private readonly bool isGatewayInitiator;
        private bool isDisposed;

        public UnitOfWork(IPersistentDataGatewayProvider gatewayProvider)
        {
            this.gatewayProvider = gatewayProvider;
            this.gateway = this.gatewayProvider.GetDurableDataGateway();
            this.isGatewayInitiator = this.gateway.IsFresh;
            this.gateway.IsFresh = false;
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

            if (isDisposing && this.isGatewayInitiator)
            {
                this.gatewayProvider.DisposeGateway(this.gateway);
                this.gateway.Dispose(true);
            }

            this.isDisposed = true;
        }
    }
}