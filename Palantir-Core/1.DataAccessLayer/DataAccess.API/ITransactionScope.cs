namespace Ix.Palantir.DataAccess.API
{
    using System;

    public interface ITransactionScope : IDisposable
    {
        ITransactionScope Begin();
        void Commit();
        void Rollback();
    }
}
