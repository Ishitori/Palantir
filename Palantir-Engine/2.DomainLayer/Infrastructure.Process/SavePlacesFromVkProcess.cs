namespace Ix.Palantir.Infrastructure.Process
{
    using System.Linq;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Logging;
    using Ix.Palantir.Vkontakte.API.Access;

    using CitiesReponse = Ix.Palantir.Vkontakte.API.Responses.Cities.response;
    using CountriesReponse = Ix.Palantir.Vkontakte.API.Responses.Countries.response;

    public class SavePlacesFromVkProcess
    {
        private readonly ILog log;
        private readonly IPlaceRepository placeRepository;
        private readonly IVkConnectionBuilder vkConnectionBuilder;

        public SavePlacesFromVkProcess(IVkConnectionBuilder vkConnectionBuilder, ILog log, IPlaceRepository placeRepository)
        {
            this.log = log;
            this.placeRepository = placeRepository;
            this.vkConnectionBuilder = vkConnectionBuilder;
        }

        public void SaveCountriesAndCities()
        {
            this.log.Debug("Places parsing started");

            IVkDataProvider vkDataProvider = this.vkConnectionBuilder.GetVkDataProvider();
            CountriesReponse countries = vkDataProvider.GetCountries();
            this.SaveCountries(countries);
            this.log.Debug("Countries are saved");

            int offset = 0;

            while (true)
            {
                CitiesReponse cities = vkDataProvider.GetCities(offset);
                this.SaveCities(cities);
                
                if (cities != null && cities.city != null && cities.city.Length > 0 && this.NonEmptyTitleExists(cities))
                {
                    offset += cities.city.Length;
                }
                else
                {
                    break;
                }
            }

            this.log.Debug("Places parsing finished");
        }

        private bool NonEmptyTitleExists(CitiesReponse cities)
        {
            return cities.city.Any(city => !string.IsNullOrWhiteSpace(city.name));
        }

        private void SaveCountries(CountriesReponse countries)
        {
            if (countries == null || countries.country == null || countries.country.Length <= 0)
            {
                return;
            }

            foreach (var c in countries.country)
            {
                Country country = new Country
                                      {
                                          VkId = c.cid,
                                          Title = c.title
                                      };

                this.placeRepository.Save(country);
            }
        }
        private void SaveCities(CitiesReponse cities)
        {
            if (cities == null || cities.city == null || cities.city.Length <= 0)
            {
                return;
            }

            foreach (var c in cities.city)
            {
                City city = new City
                {
                    VkId = c.cid,
                    Title = c.name
                };

                this.placeRepository.Save(city);
            }
        }
    }
}