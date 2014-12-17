namespace Ix.Palantir.Services
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Security.Cryptography;
    using Dapper;
    using Framework.ObjectFactory;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DataAccess.API.Repositories;
    using Ix.Palantir.Security;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Security;
    using Security.API;
    using Utilities;
    using Project = DomainModel.Project;

    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly Func<IProjectRepository> projectRepositoryFactory;
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly ICurrentUserProvider currentUserProvider;
        private readonly IProjectRepository projectRepository;
        private readonly IDataGatewayProvider dataGatewayProvider;
        private readonly IAccountRepository accountRepository;

        public UserService(IUserRepository userRepository, IAccountRepository accountRepository, Func<IProjectRepository> projectRepositoryFactory, IUnitOfWorkProvider unitOfWorkProvider, ICurrentUserProvider currentUserProvider, IProjectRepository projectRepository, IDataGatewayProvider dataGatewayProvider)
        {
            this.userRepository = userRepository;
            this.accountRepository = accountRepository;
            this.projectRepositoryFactory = projectRepositoryFactory;
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.currentUserProvider = currentUserProvider;
            this.projectRepository = projectRepository;
            this.dataGatewayProvider = dataGatewayProvider;
        }

        public bool IsUserValid(string email, string password)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                Hasher hasher = new Hasher();
                User user = this.userRepository.GetUserByEmail(email);

                if (user == null)
                {
                    return false;
                }

                string hashedPassword = hasher.ComputeSaltedHash(password, user.Salt);

                bool isUserValid = hashedPassword == user.PasswordHash;
                return isUserValid;
            }
        }
        public UserInfo GetUser(int userId)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                User user = this.userRepository.GetUserById(userId);
                return new UserInfo(user);
            }
        }
        public IList<UserInfo> GetUserList()
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                return this.userRepository.GetUsers().Select(u => new UserInfo(u)).ToList();
            }
        }
        public IList<UserInfo> GetUserList(int accountId)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                return this.userRepository.GetUsers(accountId).Select(u => new UserInfo(u)).ToList();
            }
        }
        public void DeleteUser(int userId)
        {
            // TODO: Implement multi user logic
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                IProjectRepository projectRepository = this.projectRepositoryFactory();
                IList<Project> projects = projectRepository.GetByAccountId(userId);
                User user = this.userRepository.GetUserById(userId);

                using (ITransactionScope transaction = Factory.GetInstance<ITransactionScope>().Begin())
                {
                    foreach (var project in projects)
                    {
                        projectRepository.Delete(project);
                    }

                    this.userRepository.Delete(user);

                    transaction.Commit();
                }
            }
        }
        public void AddUser(UserInfo userInfo, int accountId)
        {
            IAccount account = this.accountRepository.GetAccount(accountId);

            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                User user = new User
                                {
                                    FirstName = userInfo.FirstName.Trim(),
                                    LastName = userInfo.LastName.Trim(),
                                    Email = userInfo.Email.Trim(),
                                    Account = account
                                };

                Hasher hasher = new Hasher();
                user.Salt = this.CreateRandomSalt();
                user.PasswordHash = hasher.ComputeSaltedHash(userInfo.Password.Trim(), user.Salt);

                using (ITransactionScope transaction = Factory.GetInstance<ITransactionScope>().Begin())
                {
                    this.userRepository.Save(user);
                    transaction.Commit();
                }
            }
        }

        public void EditUser(UserInfo userInfo)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                Hasher hasher = new Hasher();

                using (ITransactionScope transaction = Factory.GetInstance<ITransactionScope>().Begin())
                {
                    User user = this.userRepository.GetUserById(userInfo.Id);

                    user.FirstName = userInfo.FirstName.Trim();
                    user.LastName = userInfo.LastName.Trim();
                    user.Email = userInfo.Email.Trim();

                    if (!string.IsNullOrWhiteSpace(userInfo.Password))
                    {
                        user.Salt = this.CreateRandomSalt();
                        user.PasswordHash = hasher.ComputeSaltedHash(userInfo.Password.Trim(), user.Salt);
                    }

                    this.userRepository.Update(user);
                    transaction.Commit();
                }
            }
        }

        public bool CanUserCreateProject()
        {
            var account = this.currentUserProvider.GetCurrentUser().GetAccount();
            int projectsCount = this.projectRepository.GetByAccountId(account.Id).Count;
            
            if (!account.MaxProjectsCount.HasValue)
            {
                var reader = new AppSettingsReader();
                int limit = (int)reader.GetValue("MaximumProjectsCount", typeof(int));

                return projectsCount < limit;
            }
            
            return projectsCount < account.MaxProjectsCount;
        }

        public void SaveUserAudienceFilter(string json)
        {
            var currentUser = this.currentUserProvider.GetCurrentUser().GetId();
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                var existForCurrentUserQuery = string.Format("SELECT * FROM userfilter WHERE userid={0}", currentUser);
                var userExist = dataGateway.Connection.Query<dynamic>(existForCurrentUserQuery) != null;

                var query = !userExist ? 
                    string.Format("INSERT INTO userfilter VALUES ({0}, '{1}')", currentUser, json) : 
                    string.Format("UPDATE userfilter SET json='{0}' WHERE userid={1}", json, currentUser);

                dataGateway.Connection.Execute(query);
            }
        }

        public string GetUserAudienceFilter()
        {
            using (IDataGateway dataGateway = this.dataGatewayProvider.GetDataGateway())
            {
                var currentUser = this.currentUserProvider.GetCurrentUser().GetId();
                var query = string.Format("SELECT json FROM userfilter WHERE userid={0}", currentUser);
                var result = dataGateway.Connection.Query<string>(query).FirstOrDefault();
                return result;
            }
        }

        public bool UserExist(string email)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                User user = this.userRepository.GetUserByEmail(email);

                return user != null;
            }
        }

        private int CreateRandomSalt()
        {
            byte[] saltBytes = new byte[4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(saltBytes);

            return (saltBytes[0] << 24) + (saltBytes[1] << 16) + (saltBytes[2] << 8) + saltBytes[3];
        }
    }
}