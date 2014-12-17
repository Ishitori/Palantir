namespace Ix.Palantir.Security
{
    using System.Collections.Generic;
    using System.Linq;
    using API;

    using Ix.Palantir.DataAccess.API;

    public class UserRepository : IUserRepository
    {
        private readonly IDataGatewayProvider dataGatewayProvider;

        public UserRepository(IDataGatewayProvider dataGatewayProvider)
        {
            this.dataGatewayProvider = dataGatewayProvider;
        }

        public API.User GetUserByEmail(string email)
        {
            string loweredEmail = email.ToLower();

            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<API.User>().FirstOrDefault(u => loweredEmail == u.Email.ToLower());
            }
        }
        public API.User GetUserById(int userId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<API.User>().FirstOrDefault(u => u.Id == userId);
            }
        }
        public IList<API.User> GetUsers()
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<API.User>().ToList();
            }
        }
        public IList<API.User> GetUsers(int accountId)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                return dataGateway.GetEntities<API.User>().Where(u => u.Account.Id == accountId).ToList();
            }
        }
        public void Delete(API.User user)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                dataGateway.DeleteEntity(user);
            }
        }
        public void Save(API.User user)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                dataGateway.SaveEntity(user);
            }
        }
        public void Update(API.User user)
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                dataGateway.UpdateEntity(user);
            }
        }
    }
}