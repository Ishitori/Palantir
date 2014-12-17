namespace Ix.Palantir.DataAccess.API
{
    public interface IDurableDataGateway : IDataGateway
    {
        bool IsFresh { get; set; }
        void Dispose(bool isDisposing);
    }
}