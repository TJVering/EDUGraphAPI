using EDUGraphAPI.Utils;
using EDUGraphAPI.Web.Infrastructure;
using EDUGraphAPI.Web.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace EDUGraphAPI.Web.Controllers
{
    [HandleAdalException, EduAuthorize, LinkedOrO365UsersOnly]
    public class SchoolsController : Controller
    {
        private ApplicationService applicationService;

        public SchoolsController(ApplicationService applicationService)
        {
            this.applicationService = applicationService;
        }

        public async Task<ActionResult> Index()
        {
            var userContext = await applicationService.GetUserContextAsync();
            var schoolsService = await GetSchoolsServiceAsync();
            var model = await schoolsService.GetSchoolsViewModelAsync(userContext);
            return View(model);
        }

        public async Task<ActionResult> Sections(string schoolId)
        {
            var userContext = await applicationService.GetUserContextAsync();
            var schoolsService = await GetSchoolsServiceAsync();
            var model = await schoolsService.GetSectionsViewModelAsync(userContext, schoolId, false);
            return View(model);
        }

        public async Task<ActionResult> MySections(string schoolId)
        {
            var userContext = await applicationService.GetUserContextAsync();
            var schoolsService = await GetSchoolsServiceAsync();
            var model = await schoolsService.GetSectionsViewModelAsync(userContext, schoolId, true);
            ViewBag.IsMySections = true;
            return View("Sections", model);
        }

        public async Task<ActionResult> SectionDetails(string schoolId, string sectionId)
        {
            var graphServiceClient = await AuthenticationHelper.GetGraphServiceClientAsync();
            var group = graphServiceClient.Groups[sectionId];

            var schoolsService = await GetSchoolsServiceAsync();
            var model = await schoolsService.GetSectionDetailsViewModelAsync(schoolId, sectionId, group);
            return View(model);
        }

        private async Task<SchoolsService> GetSchoolsServiceAsync()
        {
            var educationServiceClient = await AuthenticationHelper.GetEducationServiceClientAsync();
            return new SchoolsService(educationServiceClient);
        }
    }
}