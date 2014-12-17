namespace Ix.Palantir.UI.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Ix.Palantir.Security.API;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Security;

    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserService userService;
        private readonly ICurrentUserProvider currentUserProvider;
        public UsersController(IUserService userService, ICurrentUserProvider currentUserProvider)
        {
            this.userService = userService;
            this.currentUserProvider = currentUserProvider;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Create()
        {
            return this.View(new UserInfo());
        }

        [HttpPost]
        public ActionResult Create(UserInfo user)
        {
            if (this.userService.UserExist(user.Email))
            {
                ModelState.AddModelError("UserExist", string.Format("Пользователь с электронной почтой {0} уже существует", user.Email));
                return this.View("Create", user);
            }

            var account = this.currentUserProvider.GetAccountOfCurrentUser();
            this.userService.AddUser(user, account.Id);

            return this.RedirectToAction("List", "Users");
        }

        public ActionResult List()
        {
            var account = this.currentUserProvider.GetAccountOfCurrentUser();

            IList<UserInfo> users = this.userService.GetUserList(account.Id);
            return this.View(users);
        }
    }
}
