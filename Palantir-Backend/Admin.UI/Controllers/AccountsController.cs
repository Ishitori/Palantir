namespace Ix.Palantir.Admin.UI.Controllers
{
    using System.Web.Mvc;
    using Ix.Palantir.Admin.UI.Models;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.Security;

    [Authorize]
    public class AccountsController : Controller
    {
        private readonly IAccountService accountService;
        private readonly IUserService userService;
        
        public AccountsController(IAccountService accountService, IUserService userService)
        {
            this.accountService = accountService;
            this.userService = userService;
        }

        public ActionResult List()
        {
            var accounts = this.accountService.GetAccounts();

            return this.View(accounts);
        }

        public ActionResult Create()
        {
            return this.View(new AccountInfo());
        }

        public ActionResult Edit(int accountId)
        {
            AccountInfo info = this.accountService.GetAccount(accountId);
            return this.View(info);
        }

        [HttpPost]
        public ActionResult DoCreate(AccountInfo account)
        {
            this.accountService.AddAccount(account);
            return this.RedirectToAction("List", "Accounts");
        }

        [HttpPost]
        public ActionResult DoEdit(int accountId, AccountInfo account)
        {
            account.Id = accountId;
            this.accountService.EditAccount(account);
            return this.RedirectToAction("List", "Accounts");
        }

        public ActionResult View(int accountId)
        {
            var users = this.userService.GetUserList(accountId);
            var model = new UserListModel
            {
                AccountId = accountId,
                Users = users
            };

            return this.View(model);
        }
    }
}
