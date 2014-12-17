namespace Ix.Palantir.DataAccess.API.Repositories
{
    using Ix.Palantir.DomainModel;

    public interface IPlaceRepository
    {
        void Save(City city);
        void Save(Country country);
    }
}