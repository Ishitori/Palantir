namespace Ix.Palantir.Security.API
{
    using System.Collections.Generic;

    public interface IAccountRepository
    {
        IAccount GetAccount(int accountId);
        IList<IAccount> GetAccounts();
        void Save(IAccount account);
    }
}
