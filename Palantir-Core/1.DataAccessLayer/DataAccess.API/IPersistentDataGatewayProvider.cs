namespace Ix.Palantir.DataAccess.API
{
    public interface IPersistentDataGatewayProvider
    {
        IDurableDataGateway GetDurableDataGateway();
        void DisposeGateway(IDataGateway dataGateway);
    }
}