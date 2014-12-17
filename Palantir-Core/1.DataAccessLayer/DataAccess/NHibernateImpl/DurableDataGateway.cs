namespace Ix.Palantir.DataAccess.NHibernateImpl
{
    using System;

    using Ix.Palantir.DataAccess.API;

    using NHibernate;

    public class DurableDataGateway : DataGateway, IDurableDataGateway
    {
        public DurableDataGateway(ISession dataStorage) : base(dataStorage)
        {
        }

        public bool IsFresh { get; set; }

        public override void Dispose()
        {
        }
        public override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            GC.SuppressFinalize(this);
        }
    }
}