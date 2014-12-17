namespace Ix.Palantir.Security.API
{
    using System.Collections.Generic;

    public interface IUserRepository
    {
        User GetUserByEmail(string email);
        IList<User> GetUsers();
        IList<User> GetUsers(int accountId);
        User GetUserById(int userId);
        void Delete(User user);
        void Save(User user);
        void Update(User user);
    }
}