namespace Ix.Palantir.DataAccess
{
    using System.Collections.Generic;
    using System.Threading;
    using Framework.ObjectFactory;

    using Ix.Palantir.DataAccess.API;

    public class DataGatewayProvider : IDataGatewayProvider, IPersistentDataGatewayProvider
    {
        private static readonly object slotLockObject = new object();
        private static readonly object contextLockObject = new object();

        private static readonly IDictionary<int, IDataGateway> dataGatewaysHash = new Dictionary<int, IDataGateway>();

        public IDataGateway GetDataGateway()
        {
            return this.GetDataGateway<IDataGateway>(persistGateway: false);
        }
        public IDurableDataGateway GetDurableDataGateway()
        {
            return this.GetDataGateway<IDurableDataGateway>(persistGateway: true);
        }
        public void DisposeGateway(IDataGateway dataGateway)
        {
            lock (slotLockObject)
            {
                var containsGateway = dataGatewaysHash.ContainsKey(Thread.CurrentThread.ManagedThreadId);

                if (containsGateway)
                {
                    dataGatewaysHash[Thread.CurrentThread.ManagedThreadId].Dispose();
                    dataGatewaysHash.Remove(Thread.CurrentThread.ManagedThreadId);
                }

                if (dataGateway != null && !dataGateway.IsDisposed)
                {
                    dataGateway.Dispose();
                }
            }
        }

        private T GetDataGateway<T>(bool persistGateway) where T : IDataGateway
        {
            IDataGateway gateway = this.GetGateway();

            if (gateway == null || gateway.IsDisposed)
            {
                lock (contextLockObject)
                {
                    gateway = this.GetGateway();

                    if (gateway == null || gateway.IsDisposed)
                    {
                        gateway = Factory.GetInstance<T>();

                        if (persistGateway)
                        {
                            dataGatewaysHash[Thread.CurrentThread.ManagedThreadId] = gateway;
                        }
                    }
                }
            }

            return (T)gateway;
        }
        private IDataGateway GetGateway()
        {
            lock (slotLockObject)
            {
                var containsGateway = dataGatewaysHash.ContainsKey(Thread.CurrentThread.ManagedThreadId);

                if (containsGateway)
                {
                    return dataGatewaysHash[Thread.CurrentThread.ManagedThreadId];
                }
            }

            return null;
        }
    }
}