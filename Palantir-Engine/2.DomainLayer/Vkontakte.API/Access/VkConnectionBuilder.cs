namespace Ix.Palantir.Vkontakte.API.Access
{
    using Ix.Palantir.Configuration.API;
    using Ix.Palantir.Engine.Configuration;
    using Ix.Palantir.Utilities;
    using Ix.Palantir.Vkontakte.API;

    public class VkConnectionBuilder : IVkConnectionBuilder
    {
        private readonly IConfigurationProvider configProvider;

        public VkConnectionBuilder(IConfigurationProvider configProvider)
        {
            this.configProvider = configProvider;
        }

        public IVkDataProvider GetVkDataProvider()
        {
            var credentials = this.configProvider.GetConfigurationSection<VkCredentials>();
            ITokenProvider tokenProvider = new TokenWebRequestProvider(credentials.Login, credentials.Password);
            var accessToken = tokenProvider.GetAccessToken();

            ICookieProvider cookieProvider = new CookieProvider(credentials.Login, credentials.Password);

            var vkResponseMapper = new VkResponseMapper();
            IVkAccessor vkAccessor = new VkAccessor(accessToken, vkResponseMapper);
            IHttpAccessor httpAccessor = new HttpAccessor(cookieProvider, new WebPageDownloader());
            IVkDataLimits dataLimits = new VkDataLimits();
            IVkDataProvider vkDataProvider = new VkDataProvider(vkAccessor, httpAccessor, vkResponseMapper, dataLimits);

            return vkDataProvider;
        }
        public IVkCommandExecuter GetVkCommandExecuter()
        {
            var credentials = this.configProvider.GetConfigurationSection<VkCredentials>();
            ITokenProvider tokenProvider = new TokenWebRequestProvider(credentials.Login, credentials.Password);
            var accessToken = tokenProvider.GetAccessToken();

            var vkResponseMapper = new VkResponseMapper();
            IVkAccessor vkAccessor = new VkAccessor(accessToken, vkResponseMapper);
            IVkCommandExecuter commandExecuter = new VkCommandExecuter(vkAccessor);

            return commandExecuter;
        }
    }
}