namespace Ix.Palantir.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.DataAccess.API;
    using Ix.Palantir.DomainModel;
    using Ix.Palantir.Exceptions;
    using Ix.Palantir.Security.API;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Security;

    public class AccountService : IAccountService
    {
        private readonly IUnitOfWorkProvider unitOfWorkProvider;
        private readonly IAccountRepository accountRepository;

        public AccountService(IUnitOfWorkProvider unitOfWorkProvider, IAccountRepository accountRepository)
        {
            this.unitOfWorkProvider = unitOfWorkProvider;
            this.accountRepository = accountRepository;
        }

        public AccountInfo GetAccount(int accountId)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                IAccount account = this.accountRepository.GetAccount(accountId);
                return new AccountInfo(account);
            }
        }
        public IList<AccountInfo> GetAccounts()
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                return this.accountRepository
                    .GetAccounts()
                    .Select(a => new AccountInfo(a))
                    .OrderBy(a => a.Title).ToList();
            }
        }

        public void AddAccount(AccountInfo accountInfo)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                Account account = new Account
                {
                    Title = accountInfo.Title,
                    MaxProjectsCount = accountInfo.MaxProjectsCount,
                    CanDeleteProjects = accountInfo.CanDeleteProjects
                };

                using (ITransactionScope transaction = Factory.GetInstance<ITransactionScope>().Begin())
                {
                    this.accountRepository.Save(account);
                    transaction.Commit();
                }
            }
        }
        public void EditAccount(AccountInfo accountInfo)
        {
            using (this.unitOfWorkProvider.CreateUnitOfWork())
            {
                IAccount account = this.accountRepository.GetAccount(accountInfo.Id);

                if (account == null)
                {
                    throw new PalantirException("Account is not found");
                }

                using (ITransactionScope transaction = Factory.GetInstance<ITransactionScope>().Begin())
                {
                    account.Title = accountInfo.Title;
                    account.MaxProjectsCount = accountInfo.MaxProjectsCount;
                    account.CanDeleteProjects = accountInfo.CanDeleteProjects;

                    this.accountRepository.Save(account);
                    transaction.Commit();
                }
            }
        }
    }
}
