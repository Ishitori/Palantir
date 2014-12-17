namespace Ix.Palantir.UI.Attributes
{
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Ix.Framework.ObjectFactory;
    using Ix.Palantir.Security.API;
    using Ix.Palantir.Services.API;

    public class ProjectOwnerSecurityAttribute : AuthorizeAttribute
    {
        private ICurrentUserProvider CurrentUserProvider
        {
            get
            {
                return Factory.GetInstance<ICurrentUserProvider>();
            }
        }
        private IProjectService ProjectService
        {
            get
            {
                return Factory.GetInstance<IProjectService>();
            }
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if (!isAuthorized)
            {
                return false;
            }

            var account = this.CurrentUserProvider.GetAccountOfCurrentUser();

            var url = httpContext.Request.Url.ToString();
            var elementOfUrl = url.Split('/');
            string id = elementOfUrl.ElementAtOrDefault(4);

            if (id != null)
            {
                int projectId = int.Parse(id);
                var project = this.ProjectService.GetProject(projectId);

                if (project == null)
                {
                    throw new HttpException(404, "Not found");
                }

                if (project.CreatorId == account.Id)
                {
                    return true;
                }
            }

            throw new HttpException(404, "Not found");
        }
    }
}