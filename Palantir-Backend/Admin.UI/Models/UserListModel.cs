namespace Ix.Palantir.Admin.UI.Models
{
    using System.Collections.Generic;
    using Ix.Palantir.Services.API.Security;

    public class UserListModel
    {
        public int AccountId { get; set; }
        public IList<UserInfo> Users { get; set; }
    }
}