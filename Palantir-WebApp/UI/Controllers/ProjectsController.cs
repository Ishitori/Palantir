namespace Ix.Palantir.UI.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Ix.Palantir.Services.API;
    using Ix.Palantir.Services.API.CreateProject;
    using Ix.Palantir.UI.Models;

    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IProjectService projectService;
        private readonly IUserService userService;
        private readonly IMetricsService metricsService;

        public ProjectsController(IProjectService projectService, IUserService userService, IMetricsService metricsService)
        {
            this.projectService = projectService;
            this.userService = userService;
            this.metricsService = metricsService;
        }

        public ActionResult List()
        {
            IEnumerable<ProjectDetails> projects = this.projectService.GetProjectsDetails();
            var model = new ProjectListModel(projects);
            return this.View(model);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Create()
        {
            CreateProjectViewModel model = new CreateProjectViewModel();
            model.CreateProjectsDisabled = !this.userService.CanUserCreateProject();
            return this.View(model);
        }

        public ActionResult CreateProject(Project project)
        {
            if (this.userService.CanUserCreateProject())
            {
                CreateProjectResult projectResult = this.projectService.CreateProject(project);
                return this.Json(projectResult, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public ActionResult IsReady(string ticketId)
        {
            CreateProjectStatus result = this.projectService.GetCreateProjectStatus(ticketId);

            if (result.IsSuccess)
            {
                result.ProjectUrl = this.Url.Action("Index", "dashboard", new { id = result.ProjectId });
            }

            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectList()
        {
            var projects = this.projectService.GetProjects();

            return this.Json(projects, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CheckAvailability(int id)
        {
            var result = this.metricsService.CheckAvailability(id);
            var checkAvailability = new CheckAvailabilityModel
            {
                IsReady = (bool)result[2],
                Done = (int)result[0],
                Total = (int)result[1]
            };
            return this.Json(checkAvailability);
        }
    }
}
