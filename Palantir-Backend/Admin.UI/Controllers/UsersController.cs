namespace Ix.Palantir.Admin.UI.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Ix.Palantir.Admin.UI.Models;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Security;

    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        public ActionResult List(int accountId)
        {
            IList<UserInfo> userInfos = this.userService.GetUserList();
            UserListModel model = new UserListModel()
                                  {
                                      AccountId = accountId,
                                      Users = userInfos
                                  };

            return this.View(model);
        }

        public ActionResult Create()
        {
            return this.View(new UserInfo());
        }

        public ActionResult Edit(int id)
        {
            UserInfo user = this.userService.GetUser(id);
            return this.View(user);
        }

        public ActionResult Delete(int accountId, int id)
        {
            this.userService.DeleteUser(id);
            return this.RedirectToAction("View", "Accounts", new { accountId });
        }
        [HttpPost]
        public ActionResult DoCreate(int accountId, UserInfo user)
        {
            this.userService.AddUser(user, accountId);
            return this.RedirectToAction("View", "Accounts", new { accountId });
        }
        [HttpPost]
        public ActionResult DoEdit(int accountId, UserInfo user)
        {
            this.userService.EditUser(user);
            return this.RedirectToAction("View", "Accounts", new { accountId });
        }
    }
}