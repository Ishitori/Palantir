namespace Ix.Palantir.Services.API
{
    using System.Collections.Generic;
    using Ix.Palantir.Services.API.Security;

    public interface IAccountService
    {
        IList<AccountInfo> GetAccounts();
        void AddAccount(AccountInfo accountInfo);
        void EditAccount(AccountInfo accountInfo);
        AccountInfo GetAccount(int accountId);
    }
}
