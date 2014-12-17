namespace Ix.Palantir.DataAccess.Repositories
{
    using DomainModel;

    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;

    public class PlaceRepository : IPlaceRepository
    {
        private readonly IDataGatewayProvider dataGatewayProvider;

        public PlaceRepository(IDataGatewayProvider dataGatewayProvider)
        {
            this.dataGatewayProvider = dataGatewayProvider;
        }

        public void Save(City city)
        {
            if (!city.IsTransient())
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                dataGateway.SaveEntity(city);
            }
        }
        public void Save(Country country)
        {
            if (!country.IsTransient())
            {
                return;
            }

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                dataGateway.SaveEntity(country);
            }
        }
    }
}