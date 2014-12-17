namespace Ix.Palantir.Security
{
    using System.Collections.Generic;
    using System.Linq;
    using API;

    using Ix.Palantir.DataAccess.API;

    public class AccountRepository : IAccountRepository
    {
        private readonly IDataGatewayProvider dataGatewayProvider;

        public AccountRepository(IDataGatewayProvider dataGatewayProvider)
        {
            this.dataGatewayProvider = dataGatewayProvider;
        }

        public IAccount GetAccount(int accountId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<API.IAccount>().FirstOrDefault(x => x.Id == accountId);
            }
        }

        public IList<IAccount> GetAccounts()
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<API.IAccount>().ToList();
            }
        }

        public void Save(IAccount account)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                dataGateway.SaveEntity(account);
            }
        }
    }
}
