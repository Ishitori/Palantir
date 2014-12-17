namespace Ix.Palantir.Services.API
{
    using System.Collections.Generic;
    using Ix.Palantir.Services.API.Security;

    public interface IUserService
    {
        bool IsUserValid(string email, string password);
        IList<UserInfo> GetUserList();
        IList<UserInfo> GetUserList(int accountId);
        UserInfo GetUser(int userId);
        void DeleteUser(int userId);
        void AddUser(UserInfo user, int accountId);
        void EditUser(UserInfo user);
        bool CanUserCreateProject();
        void SaveUserAudienceFilter(string json);
        string GetUserAudienceFilter();
        bool UserExist(string email);
    }
}