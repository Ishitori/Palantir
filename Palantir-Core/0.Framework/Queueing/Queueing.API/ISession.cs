namespace Ix.Palantir.Queueing.API
{
    using System;

    public interface ISession : IDisposable
    {
        bool IsClosed { get; }
        bool IsTransactedMode { get; }

        void Initialize(bool transactionMode);
        void Commit();
        void Rollback();
    }
}
