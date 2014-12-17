namespace Ix.Palantir.DataAccess.API
{
    public interface IDataGatewayProvider
    {
        IDataGateway GetDataGateway();
    }
}